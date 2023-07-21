using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using OpenDental;
using UnitTestsCore;

namespace UnitTests.SetupWizardTests {
	[TestClass]
	public class SetupWizardTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
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
		public void SetupWizard_RegKeySetupStatus_Complete() {
			SetupWizard.RegKeySetup regKeySetup=new SetupWizard.RegKeySetup();
			string regKey="12567-248287932-83748389";
			Assert.AreEqual(ODSetupStatus.Complete,regKeySetup.GetSetupStatus(regKey));
		}

		[TestMethod]
		public void SetupWizard_RegKeySetupStatus_NeedsAttenton() {
			SetupWizard.RegKeySetup regKeySetup=new SetupWizard.RegKeySetup();
			string regKey="";
			Assert.AreEqual(ODSetupStatus.NotStarted,regKeySetup.GetSetupStatus(regKey));
		}

		[TestMethod] 
		public void Setupwizard_ProvSetupStatus_Complete() {
			SetupWizard.ProvSetup provSetup=new SetupWizard.ProvSetup();
			List<Provider> listProviders=ProviderT.CreateProviderList();
			Assert.AreEqual(ODSetupStatus.Complete,provSetup.GetSetupStatus(listProviders));
		}

		[TestMethod]
		public void SetupWizard_ProvSetupStatus_NeedsAttention() {
			SetupWizard.ProvSetup provSetup=new SetupWizard.ProvSetup();
			List<Provider> listProviders=ProviderT.CreateProviderListEmpties();
			Assert.AreEqual(ODSetupStatus.NeedsAttention,provSetup.GetSetupStatus(listProviders));
		}

		[TestMethod]
		public void SetupWizard_ProvSetupStatus_NotStarted() {
			SetupWizard.ProvSetup provSetup=new SetupWizard.ProvSetup();
			List<Provider> listProviders=new List<Provider>();
			Assert.AreEqual(ODSetupStatus.NotStarted,provSetup.GetSetupStatus(listProviders));
		}
		
		[TestMethod]
		public void SetupWizard_FeatureSetupStatus_Optional() {
			SetupWizard.FeatureSetup featureSetup=new SetupWizard.FeatureSetup();
			Assert.AreEqual(ODSetupStatus.Optional,featureSetup.GetStatus);
		}

		[TestMethod]
		public void SetupWizard_EmployeeSetupStatus_Complete() {
			SetupWizard.EmployeeSetup employeeSetup=new SetupWizard.EmployeeSetup();
			List<Employee> listEmployees=EmployeeT.CreateEmployeeList();
			Assert.AreEqual(ODSetupStatus.Complete,employeeSetup.GetSetupStatus(listEmployees));
		}

		[TestMethod]
		public void SetupWizard_EmployeeSetupStatus_NeedsAttention() {
			SetupWizard.EmployeeSetup employeeSetup=new SetupWizard.EmployeeSetup();
			List<Employee> listEmployees=EmployeeT.CreateEmployeeListEmpties();
			Assert.AreEqual(ODSetupStatus.NeedsAttention,employeeSetup.GetSetupStatus(listEmployees));
		}

		[TestMethod]
		public void SetupWizard_EmployeeSetupStatus_NotStarted() {
			SetupWizard.EmployeeSetup employeeSetup=new SetupWizard.EmployeeSetup();
			List<Employee> listEmployees=new List<Employee>();
			Assert.AreEqual(ODSetupStatus.NotStarted,employeeSetup.GetSetupStatus(listEmployees));
		}

		[TestMethod]
		public void SetupWizard_FeeSchedSetupStatus_Complete() {
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix:"Unit Test Complete", isGlobal:true);
			Fee fee=new Fee();
			fee.FeeNum=100000007;
			fee.FeeSched=feeSchedNum;
			Fees.Insert(fee);
			FeeSched feeSched=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==feeSchedNum);
			Assert.IsNotNull(feeSched);
			SetupWizard.FeeSchedSetup feeSchedSetup=new SetupWizard.FeeSchedSetup();
			List<FeeSched> listFeeSchedules=new List<FeeSched>();
			listFeeSchedules.Add(feeSched);
			Assert.AreEqual(ODSetupStatus.Complete,feeSchedSetup.GetSetupStatus(listFeeSchedules));
			//cleanup
			Fees.Delete(100000007);
			FeeSchedT.Delete(feeSchedNum);
		}

		[TestMethod]
		public void SetupWizard_FeeSchedSetupStatus_NeedsAttention() {
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix:"Unit Test Complete", isGlobal:true);
			FeeSched feeSched=FeeScheds.GetFirstOrDefault(x => x.FeeSchedNum==feeSchedNum);
			Assert.IsNotNull(feeSched);
			SetupWizard.FeeSchedSetup feeSchedSetup=new SetupWizard.FeeSchedSetup();
			List<FeeSched> listFeeSchedules=new List<FeeSched>();
			listFeeSchedules.Add(feeSched);
			Assert.AreEqual(ODSetupStatus.NeedsAttention,feeSchedSetup.GetSetupStatus(listFeeSchedules));
			FeeSchedT.Delete(feeSchedNum); // cleanup
		}

		[TestMethod] 
		public void SetupWizard_FeeSchedSetupStatus_NotStarted() {
			SetupWizard.FeeSchedSetup feeSchedSetup=new SetupWizard.FeeSchedSetup();
			List<FeeSched> listFeeSchedules=new List<FeeSched>();
			Assert.AreEqual(ODSetupStatus.NotStarted,feeSchedSetup.GetSetupStatus(listFeeSchedules));
		}

		[TestMethod]
		public void SetupWizard_OperatorySetupStatus_Complete() {
			SetupWizard.OperatorySetup operatorySetup=new SetupWizard.OperatorySetup();
			List<Operatory> listOperatories=OperatoryT.CreateListOperatories();
			Assert.AreEqual(ODSetupStatus.Complete,operatorySetup.GetSetupStatus(listOperatories));
		}

		[TestMethod]
		public void SetupWizard_OperatorySetupStatus_NeedsAttention() {
			SetupWizard.OperatorySetup operatorySetup=new SetupWizard.OperatorySetup();
			List<Operatory> listOperatories=OperatoryT.CreateListOperatoriesEmpties();
			Assert.AreEqual(ODSetupStatus.NeedsAttention,operatorySetup.GetSetupStatus(listOperatories));
		}

		[TestMethod]
		public void SetupWizard_OperatorySetupStatus_NotStarted() {
			SetupWizard.OperatorySetup operatorySetup=new SetupWizard.OperatorySetup();
			List<Operatory> listOperatories=new List<Operatory>();
			Assert.AreEqual(ODSetupStatus.NotStarted,operatorySetup.GetSetupStatus(listOperatories));
		}

		[TestMethod]
		public void SetupWizard_PracticeSetupStatus_Complete() {
			SetupWizard.PracticeSetup practiceSetup=new SetupWizard.PracticeSetup();
			Assert.AreEqual(ODSetupStatus.Complete,practiceSetup.GetStatus);
		}

		[TestMethod]
		public void SetupWizard_PrinterSetupStatus_Complete() {
			SetupWizard.PrinterSetup printerSetup=new SetupWizard.PrinterSetup();
			Assert.AreEqual(ODSetupStatus.Optional,printerSetup.GetStatus);
		}

		[TestMethod]
		public void SetupWizard_DefinitionSetupStatus_Optional() {
			SetupWizard.DefinitionSetup definitionSetup=new SetupWizard.DefinitionSetup();
			Assert.AreEqual(ODSetupStatus.Optional,definitionSetup.GetStatus);
		}

		[TestMethod]
		public void SetUpWizard_ClinicSetupStatus_Complete() {
			SetupWizard.ClinicSetup clinicSetup=new SetupWizard.ClinicSetup();
			List<Clinic> listClinics=ClinicT.CreatClinicList();
			Assert.AreEqual(ODSetupStatus.Complete,clinicSetup.GetSetupStatus(listClinics));
		}

		[TestMethod]
		public void SetupWizard_ClinicSetupStatus_NeedsAttention() {
			SetupWizard.ClinicSetup clinicSetup=new SetupWizard.ClinicSetup();
			List<Clinic> listClinics=ClinicT.CreatClinicListEmpties();
			Assert.AreEqual(ODSetupStatus.NeedsAttention,clinicSetup.GetSetupStatus(listClinics));
		}

	}
}