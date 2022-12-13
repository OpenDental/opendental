using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.LimitedBetaFeatureTests {
	[TestClass]
	public class LimitedBetaFeatureTests:TestBase {
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {

		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		protected static void SetupTest() {
			LimitedBetaFeatures.SyncFromHQ(new List<LimitedBetaFeature>());
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		protected static void TearDownTest() {

		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {

		}
		#region Clinic Tests
		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_InDevlopment_RegisteredClinicsHideUI() {
			Clinic clinicSignedUp = new Clinic(){ ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1=(long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice=new List<LimitedBetaFeature>(){ 
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		}

		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_LimitedBeta_RegisteredClinicsShowsUI() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		}

		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_Finished_ClinicsShowsUI() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		}
		#endregion

		#region Clinic Independant
		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_InDevlopment_ClinicIndependantHidesUI() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=-1,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		}

		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_LimitedBeta_ClinicIndependantShowsUI() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=-1,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		}

		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_Finished_ClinicIndependantShowsUI() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=-1,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		}
		#endregion

		#region No / Disabled eConnector
		//We can uncomment this chunk when we get a limited beta feature enum with a completed attribute
		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_InDevelopment_NoEconnHidesUI() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice=new List<LimitedBetaFeature>();
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio, clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio, clinicNotSignedUp.ClinicNum));
		}
		
		///<summary>When we have a feature marked as completed we can test this scenario. the attribute will need to be palced on the completed enum.</summary>
		//[TestMethod]
		//public void EServiceFeatureInfoHQ_Finished_NoEconnHidesUI() {
		//	Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
		//	Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
		//	long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
		//	List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>();
		//	//Upsert the Office Beta features returned from HQ.
		//	LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
		//	//Assert that we dont have access to eClipboard - Perio for the first clinic
		//	Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
		//	//Assert that we dont have access to eClipboard - Perio for the second clinic
		//	Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
		//}
		#endregion

		#region Multiple Limited Beta Features
		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_InDevlopment_RegisteredClinicHidesUI_MultipleFeatures() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			long featureKey2 = (long)EServiceFeatureInfoEnum.SecureEmail;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey2,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey2,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the first clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.SecureEmail,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.SecureEmail,clinicNotSignedUp.ClinicNum));
		}

		///<summary></summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_LimitedBeta_RegisteringClinicShowsUI_MultipleFeatures() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			long featureKey2 = (long)EServiceFeatureInfoEnum.SecureEmail;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey2,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=false,
					LimitedBetaFeatureTypeNum=featureKey2,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we have access to eClipboard - Perio for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to eClipboard - Perio for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
			//Assert that we have access to secure email for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.SecureEmail,clinicSignedUp.ClinicNum));
			//Assert that we dont have access to secure email for the second clinic
			Assert.IsFalse(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.SecureEmail,clinicNotSignedUp.ClinicNum));
		}

		///<summary>The concensus was made that if a feature enum is marked as completed, the row can </summary>
		[TestMethod]
		public void EServiceFeatureInfoHQ_Finished_AlwaysShowsUI_MultipleFeatures() {
			Clinic clinicSignedUp = new Clinic() { ClinicNum=1 };
			Clinic clinicNotSignedUp = new Clinic() { ClinicNum=2 };
			long featureKey1 = (long)EServiceFeatureInfoEnum.EClipPerio;
			long featureKey2 = (long)EServiceFeatureInfoEnum.SecureEmail;
			List<LimitedBetaFeature> limitedBetaFeaturesforOffice = new List<LimitedBetaFeature>(){
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey1,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey2,
				},
				new LimitedBetaFeature() {
					ClinicNum=clinicNotSignedUp.ClinicNum,
					IsSignedUp=true,
					LimitedBetaFeatureTypeNum=featureKey2,
				},
			};
			//Upsert the Office Beta features returned from HQ.
			LimitedBetaFeatures.SyncFromHQ(limitedBetaFeaturesforOffice);
			//Assert that we have access to eClipboard - Perio for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicSignedUp.ClinicNum));
			//Assert that we have access to eClipboard - Perio for the second clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.EClipPerio,clinicNotSignedUp.ClinicNum));
			//Assert that we have access to secure email for the first clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.SecureEmail,clinicSignedUp.ClinicNum));
			//Assert that we have access to secure email for the second clinic
			Assert.IsTrue(LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.SecureEmail,clinicNotSignedUp.ClinicNum));
		}
		#endregion
	}
}
