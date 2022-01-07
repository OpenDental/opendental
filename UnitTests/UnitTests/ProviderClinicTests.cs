using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class ProviderClinicTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			string command="DELETE FROM providerclinic";
			DataCore.NonQ(command);
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

		[TestMethod]
		public void ProviderClinics_GetStateLicenseForProv_UseRxID() {
			long provNum=ProviderT.CreateProvider("Test_Prov",fName:"Jessica",lName:"Jones");
			string stateWhereLicensed="OR";
			string stateLicense="A123456";
			string rxID="B123456";
			ProviderClinic provClinic=new ProviderClinic() {
				ProvNum=provNum,
				StateWhereLicensed=stateWhereLicensed,
				StateLicense=stateLicense,
				StateRxID=rxID
			};
			ProviderClinicCrud.Insert(provClinic);
			string stateLic=ProviderClinics.GetStateLicenseForProv(provNum,stateWhereLicensed,0,useRxId:true);
			Assert.AreEqual(provClinic.StateRxID,stateLic);
			stateLic=ProviderClinics.GetStateLicenseForProv(provNum,stateWhereLicensed,0);
			Assert.AreEqual(provClinic.StateLicense,stateLic);
			stateLic=ProviderClinics.GetStateLicenseForProv(provNum,stateWhereLicensed,0,useRxId:false);
			Assert.AreEqual(provClinic.StateLicense,stateLic);
			Assert.AreNotEqual(provClinic.StateRxID,stateLic);
		}
	}
}
