using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnitTests.Carrier_Tests {
	[TestClass]
	public class CarrierTests:TestBase {
		

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			CarrierT.ClearCarrierTable();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>Per job 24416 we are checking if the correct missing carrier is added after running the TryCarrierUpdate method</summary>
		[TestMethod]
		public void Carrier_CanadianUpdateCarriers_AddMissing() {
			CarrierT.CreateCACarrier("Medavie Blue Cross",electID:"610047",phone:"(888)614-1880");
			CarrierT.CreateCACarrier("Interim Federal Health Program",electID:"610047",phone:"(888)614-1880");
			List<Carrier> listCarrierDbBeforeUpdate=Carriers.GetAllByElectId("610047");
			ItransNCpl itrans=null;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(Properties.Resources.test_n_cpl);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			List<Carrier> listCarrierDbAfterUpdate=Carriers.GetAllByElectId("610047");
			Assert.AreEqual(listCarrierDbBeforeUpdate.Count,listCarrierDbAfterUpdate.Count);
			Assert.IsNotNull(listCarrierDbAfterUpdate.Select(x=>x.CarrierName).FirstOrDefault(x=>x.Equals("Medavie Blue Cross")));
			Assert.IsNotNull(listCarrierDbAfterUpdate.Select(x=>x.CarrierName).FirstOrDefault(x=>x.Equals("Interim Federal Health Program")));
			Carriers.Delete(listCarrierDbAfterUpdate.FirstOrDefault());
			Carriers.RefreshCache();
			List<Carrier> listFromDb=Carriers.GetAllByElectId("610047");
			Assert.IsTrue(listFromDb.Count==1);
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Name|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			listFromDb=Carriers.GetAllByElectId("610047");
			Assert.IsTrue(listFromDb.Count==2);

		}

		///<summary>If a user creates their own carrier and sets the elect ID, we want to be able to give that custom carrier the same supported transaction types, version, and network. See task num 2286023 and 2722230</summary>
		[TestMethod]
		public void Carrier_CanadianUpdateCarriers_UpdateFlagsForSameElectID() {
			ItransNCpl itrans=null;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(Properties.Resources.test_n_cpl);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Name|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carrier carrierLocalBefore=CarrierT.CreateCACarrier("Local184",electID:"000034");
			Carrier carrierTelus=Carriers.GetCarrierByName("TELUS AdjudiCare");//from JsonCarrierList string
			Assert.AreNotEqual(carrierTelus.CanadianNetworkNum,carrierLocalBefore.CanadianNetworkNum);
			Assert.AreNotEqual(carrierTelus.CanadianSupportedTypes,carrierLocalBefore.CanadianSupportedTypes);
			Assert.AreNotEqual(carrierTelus.CDAnetVersion,carrierLocalBefore.CDAnetVersion);
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Name|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			Carrier carrierLocalAfter=Carriers.GetCarrierByName("Local184");
			Assert.AreEqual(carrierTelus.CanadianNetworkNum,carrierLocalAfter.CanadianNetworkNum);
			Assert.AreEqual(carrierTelus.CanadianSupportedTypes,carrierLocalAfter.CanadianSupportedTypes);
			Assert.AreEqual(carrierTelus.CDAnetVersion,carrierLocalAfter.CDAnetVersion);
		}

		///<summary>Checks jsoncarrier->telephone->name->en first as the preferred number. Uses value in jsoncarrier->telephone->value if not found. Can return an empty string. See task num 2286023 and 2722230</summary>
		[TestMethod]
		public void Carrier_CanadianUpdateCarriers_UpdatePhone() {
			CarrierT.CreateCACarrier("Medavie Blue Cross",electID:"610047",phone:"");
			ItransNCpl itrans=null;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(Properties.Resources.test_n_cpl);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Name|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			Carrier medavieAfter=Carriers.GetCarrierByName("Medavie Blue Cross");//from JsonCarrierList string
			Assert.IsFalse(medavieAfter.Phone.IsNullOrEmpty());
			Assert.IsTrue(TelephoneNumbers.AreNumbersEqual("1(800)667-4511",medavieAfter.Phone));
		}

		[TestMethod]
		///<summary>Claims resellers with the same elect id as a user created carrier may exist in the .json file. Instead, we want to make sure we'll only pulling from the actual claims processor to update flags and set the network for Canadian users</summary>
		public void Carrier_CanadianUpdateCarriers_GetBestMatch() {
			ItransNCpl itrans=null;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(Properties.Resources.test_n_cpl);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Name|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			CarrierT.CreateCACarrier("CustomMedavie",electID:"610047");
			Carrier origMedavieCarrier=Carriers.GetCarrierByName("Medavie Blue Cross");
			Carrier origInterimCarrier=Carriers.GetCarrierByName("Interim Federal Health Program");
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false);//Just checking for flags, no need to update any other import fields
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			Carrier custMedavieCarrier=Carriers.GetCarrierByName("CustomMedavie");
			Assert.AreEqual(custMedavieCarrier.CanadianNetworkNum,origMedavieCarrier.CanadianNetworkNum);
			Assert.AreEqual(custMedavieCarrier.CanadianSupportedTypes,origMedavieCarrier.CanadianSupportedTypes);
			Assert.AreNotEqual(custMedavieCarrier.CanadianSupportedTypes,origInterimCarrier.CanadianSupportedTypes);
		}
	}
}
