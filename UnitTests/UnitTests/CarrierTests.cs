﻿﻿using System;
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
			string json=GetCustomJson();
			ItransNCpl itrans=null;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(json);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			List<Carrier> listFromDb=Carriers.GetAllByElectId("123456789");
			Assert.IsTrue(listFromDb.Count==0);
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			listFromDb=Carriers.GetAllByElectId("123456789");
			Assert.IsTrue(listFromDb.Count==1);

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
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Phone);
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
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Phone);
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
			Carrier telusBefore=CarrierT.CreateCACarrier("TELUS",electID:"000034",phone:"");
			ItransNCpl itrans=null;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(Properties.Resources.test_n_cpl);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Phone);
			}
			catch (Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			Carrier telusAfter=Carriers.GetCarrierByName("TELUS");//from JsonCarrierList string
			Assert.IsFalse(telusAfter.Phone.IsNullOrEmpty());
			Assert.IsTrue(TelephoneNumbers.AreNumbersEqual("1(866)272-2204",telusAfter.Phone));
		}

		[TestMethod]
		///<summary>Sometimes carriers drop support for Reversals. Checks the drop of support for claim reversals.</summary>
		public void Carrier_CanadianUpdateCarriers_RemoveClaimReversalsSupport() {
			ItransNCpl itrans=null;
			string itransResourceJson=Properties.Resources.test_n_cpl;
			try {
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(itransResourceJson);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			try {
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false,ItransImportFields.AddMissing|ItransImportFields.Address|ItransImportFields.Phone);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			Carrier origJohnsonCarrier=Carriers.GetCarrierByName("Johnson Inc.");
			try {
				//Change the json file to drop support of claim reversals.
				string updatedItransResourceJson=itransResourceJson.Replace(@"""reversal_02"": ""Y""",@"""reversal_02"": ""N""");
				updatedItransResourceJson=updatedItransResourceJson.Replace(@"""reversal_12"": ""Y""",@"""reversal_12"": ""N""");
				itrans=JsonConvert.DeserializeObject<ItransNCpl>(updatedItransResourceJson);
				ItransNCpl.UpdateCarriersFromJsonList(itrans,false);
			}
			catch(Exception ex) {
				Assert.Fail(ex.Message);
			}
			Carriers.RefreshCache();
			Carrier updatedJohnsonCarrier=Carriers.GetCarrierByName("Johnson Inc."); 
			Assert.AreEqual(updatedJohnsonCarrier.CanadianNetworkNum,origJohnsonCarrier.CanadianNetworkNum);
			Assert.AreNotEqual(updatedJohnsonCarrier.CanadianSupportedTypes,origJohnsonCarrier.CanadianSupportedTypes);
			Assert.AreNotEqual(CanSupTransTypes.ClaimReversal_02,(updatedJohnsonCarrier.CanadianSupportedTypes & CanSupTransTypes.ClaimReversal_02));
			Assert.AreNotEqual(CanSupTransTypes.ClaimReversalResponse_12,(updatedJohnsonCarrier.CanadianSupportedTypes & CanSupTransTypes.ClaimReversalResponse_12));
		}

		public string GetCustomJson() {
			string ret=@"{
		""carriers"": [
    {
      ""name"": {
        ""en"": ""TEST"",
        ""fr"": ""TEST""
      },
      ""change_date"": ""2020-02-28"",
      ""telephone"": [
        {
          ""change_date"": ""2020-02-28"",
          ""value"": ""1-800-661-7671"",
          ""name"": {
            ""en"": ""Alberta Blue Cross"",
            ""fr"": ""Alberta Blue Cross""
          }
        },
        {
          ""change_date"": ""2020-02-28"",
          ""value"": """",
          ""name"": {
            ""en"": """",
            ""fr"": """"
          }
        }
      ],
      ""bin"": ""123456789"",
      ""versions"": [
        ""2"",
        ""4""
      ],
      ""batch"": ""N"",
      ""age_days"": 365,
      ""policy_number"": {
        ""en"": ""1-5 digits, alphanum."",
        ""fr"": ""1-5 (alphanumérique)""
      },
      ""division_number"": {
        ""en"": ""1-3 digits alphanum"",
        ""fr"": ""1-3 (alphanumérique)""
      },
      ""certificate_number"": {
        ""en"": ""1-10 digits, alphanum"",
        ""fr"": ""1-10 (alphanumérique)""
      },
      ""claim_01"": ""Y"",
      ""claim_11"": ""Y"",
      ""claim_21"": ""X"",
      ""reversal_02"": ""Y"",
      ""reversal_12"": ""Y"",
      ""predetermination_03"": ""Y"",
      ""predetermination_13"": ""Y"",
      ""predetermination_23"": ""X"",
      ""predetermination_multi"": ""Y"",
      ""outstanding_04"": ""Y"",
      ""outstanding_14"": ""Y"",
      ""summary_reconciliation_05"": ""N"",
      ""summary_reconciliation_15"": ""N"",
      ""payment_reconciliation_06"": ""N"",
      ""payment_reconciliation_16"": ""N"",
      ""cob_07"": ""Y"",
      ""eligibility_08"": ""N"",
      ""eligibility_18"": ""N"",
      ""attachment_09"": ""Y"",
      ""attachment_19"": ""Y"",
      ""cob_instructions"": {
        ""en"": ""Different Primary: Send the secondary claim using the COB 07 transaction. The secondary claim benefits will be adjudicated based on the returned primary claim EOB.\n<br>\n<br>\nNo action required. The secondary claim & predetermination will be adjudicated. There is no need to submit a paper claim form."",
        ""fr"": ""Régime primaire différent : Soumettre la demande de prestations secondaire au moyen de la transaction 07 relative à la coordination des prestations. Alberta Blue Cross évaluera la demande de prestations secondaire en fonction des détails des prestations transmis en réponse à la demande primaire.\n\nDemande et prédétermination « bleu contre bleu » : Aucune mesure n’est requise. Alberta Blue Cross traitera la demande de prestations/prédétermination secondaire et communiquera la réponse à la demande de prestations/prédétermination primaire avec un second document Détails des prestations.""
      },
      ""notes"": {
        ""en"": """",
        ""fr"": """"
      },
      ""claims_processor"": {
        ""change_date"": ""2017-10-12"",
        ""name"": {
          ""en"": ""TEST"",
          ""fr"": ""TEST""
        },
        ""short_name"": {
          ""en"": ""ABC"",
          ""fr"": ""ABC""
        }
      },
      ""address"": [
        {
          ""street1"": ""10009-108 St. NW"",
          ""street2"": """",
          ""city"": ""Edmonton"",
          ""province"": ""AB"",
          ""postal_code"": ""T5J 3C5"",
          ""attention"": ""-"",
          ""notes"": {
            ""en"": ""Mail your paper claims to this address"",
            ""fr"": ""Postez vos réclamations papier à cette adresse""
          }
        }
      ],
      ""network"": [
        {
          ""telephone"": [
            {
              ""name"": {
                ""en"": ""Alberta Blue Cross Help Desk"",
                ""fr"": ""Alberta Blue Cross Help Desk FR""
              },
              ""change_date"": ""2017-10-12"",
              ""phone"": ""1-800-661-7671""
            }
          ],
          ""load"": {
            ""change_date"": ""2020-02-28"",
            ""percent"": 100
          },
          ""network_folder"": ""ABC"",
          ""name"": {
            ""en"": ""Alberta Blue Cross"",
            ""fr"": ""Alberta Blue Cross""
          },
          ""change_date"": ""2018-08-28""
        },
        {
          ""telephone"": [
            {
              ""name"": {
                ""en"": ""instream Help Desk"",
                ""fr"": ""instream Help Desk""
              },
              ""change_date"": ""2017-08-14"",
              ""phone"": ""1-855-521-1121""
            }
          ],
          ""load"": {
            ""change_date"": ""2020-02-28"",
            ""percent"": 0
          },
          ""network_folder"": ""INS"",
          ""name"": {
            ""en"": ""instream Canada"",
            ""fr"": ""instream Canada""
          },
          ""change_date"": ""2019-02-13""
        }
      ]
		}
		],
		""rot_bins"": [
				""610361"",
				""000016"",
				""000103"",
				""000090""
		],
		""change_log"": [
    {
      ""change_date"": ""2018-08-15"",
      ""description"": {
        ""en"": ""610650 - ESORSE Corporation  - COB 07 transaction is not supported"",
        ""fr"": ""610650 - ESORSE Corporation  - La transaction relative à coordination des prestations (07) n'est pas acceptée""
      }
		}
		]
	}";
			return ret;
		}
	}
}
