using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;
using PDMPScript;
using System.Xml.Serialization;
using System.IO;

namespace UnitTests.Erx_Tests {
	[TestClass]
	public class ErxTests:TestBase {
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
		}

		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
		}

		[TestCleanup]
		public void TearDownTest() {
			//Add anything here that you want to run after every test in this class.
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		[TestMethod]
		public void DoseSpot_IsAddressAPOBox() {
			Assert.AreEqual(false,DoseSpot.IsAddressPOBox("196 Ramapo Road"));
			Assert.AreEqual(false,DoseSpot.IsAddressPOBox("196 RamaPO Box"));
			Assert.AreEqual(false,DoseSpot.IsAddressPOBox("2668 South Road"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("PO Box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("P.O Box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("po box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("p.o. box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("po. box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("PO. box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("196 P.O Box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("196 PO Box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("196 po box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("196 p.o box"));
			Assert.AreEqual(true,DoseSpot.IsAddressPOBox("196 po. box"));
		}

		[TestMethod]
		public void DoseSpot_IsPhoneNumberValid() {
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)-230-4007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("9592304007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("959-230-4007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)230-4007"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)230-4007X1234"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)230-4007x1234"));
			Assert.AreEqual(true,DoseSpot.IsPhoneNumberValid("(959)222-2222"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-959-230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-4007X1234"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-4007x1234"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("1-(959)230-40071"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(959)230t-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(555)230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(059)230-4007"));
			Assert.AreEqual(false,DoseSpot.IsPhoneNumberValid("(159)230-4007"));
		}

		#region NewCrop

		[TestMethod]
		///<summary>Tests that, when creating Patient Characteristics for submission to NewCrop, the Patient's height, heightUnits, weight 
		///and weightUnits are included when specified.</summary>
		public void NewCrop_AddingHeightandWeight() {
			long provNum=ProviderT.CreateProvider("DOC","Beverly","Crusher");//Provider
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,email:"brucewayne@wayntech.com",
				phone:"1234567",lName:"Wayne",fName:"Bruce",preferredName:"Batman",setPortalAccessInfo:true);//Patient
			Employee emp=EmployeeT.CreateEmployee("");//Employee
			ProviderClinic providerClinic=ProviderClinicT.CreateProviderClinic(Providers.GetProv(provNum),dEANum:"93485",stateRxID:"439578");
			ProviderClinicT.Update(providerClinic);
			Vitalsign vitals=VitalsignT.CreateVitalsign(patNum:pat.PatNum,height:6,weight:200,heightExamCode:"3137-7",weightExamCode:"3141-9");//Vitalsigns
			VitalsignT.Update(vitals:vitals,patNum:pat.PatNum);
			ErxXml.BuildNewCropClickThroughXml(Providers.GetProv(provNum),emp,pat,out NCScript nCScript);
			Assert.AreEqual("6",nCScript.Patient.PatientCharacteristics.height);
			Assert.AreEqual("inches",nCScript.Patient.PatientCharacteristics.heightUnits);
			Assert.AreEqual("200",nCScript.Patient.PatientCharacteristics.weight);
			Assert.AreEqual(WeightUnitType.lbs1,nCScript.Patient.PatientCharacteristics.weightUnits);
		}

		[TestMethod]
		///<summary>Tests that, when creating Patient Characteristics for submission to NewCrop, the Patient's height, heightUnits, weight 
		///and weightUnits are _only_ included when specified. In the below case, only the height information is gvien and thus only the height
		///information should be in the final results.</summary>
		public void NewCrop_AddingHeightOnly() {
			long provNum=ProviderT.CreateProvider("DOC","Beverly","Crusher");//Provider
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,email:"brucewayne@wayntech.com",
				phone:"1234567",lName:"Wayne",fName:"Bruce",preferredName:"Batman",setPortalAccessInfo:true);//Patient
			Employee emp=EmployeeT.CreateEmployee("");//Employee
			ProviderClinic providerClinic=ProviderClinicT.CreateProviderClinic(Providers.GetProv(provNum),dEANum:"93485",stateRxID:"439578");
			ProviderClinicT.Update(providerClinic);
			Vitalsign vitals=VitalsignT.CreateVitalsign(patNum:pat.PatNum,height:6,heightExamCode:"3137-7",weightExamCode:"3141-9");//Vitalsigns
			VitalsignT.Update(vitals:vitals,patNum:pat.PatNum);
			ErxXml.BuildNewCropClickThroughXml(Providers.GetProv(provNum),emp,pat,out NCScript nCScript);
			Assert.AreEqual("6",nCScript.Patient.PatientCharacteristics.height);
			Assert.AreEqual("inches",nCScript.Patient.PatientCharacteristics.heightUnits);
			Assert.AreEqual(null,nCScript.Patient.PatientCharacteristics.weight);
			Assert.AreEqual(WeightUnitType.Item,nCScript.Patient.PatientCharacteristics.weightUnits);//WeightUnitType.Item is default for enum (0)
		}

		#endregion
	}
}
