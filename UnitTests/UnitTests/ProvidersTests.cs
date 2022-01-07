using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Providers_Tests {
	[TestClass]
	public class ProvidersTests:TestBase {
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
		public void Providers_GetAll_BlankBirthdate() {
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name);//New Provider with default Provider.Birthdate.
			List<Provider> listProviders=Providers.GetAll();
			Provider prov=listProviders.FirstOrDefault(x => x.ProvNum==provNum);
			Assert.AreEqual(DateTime.MinValue,prov.Birthdate);
		}

		[TestMethod]
		public void Providers_GetAll_BlankDateTerm() {
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name);//New Provider with default Provider.Birthdate.
			List<Provider> listProviders=Providers.GetAll();
			Provider prov=listProviders.FirstOrDefault(x => x.ProvNum==provNum);
			Assert.AreEqual(DateTime.MinValue,prov.DateTerm);
		}

		///<summary> Tests the providers a user has access to based on a clinic number. Must refresh providercliniclink caches inside method to use.  </summary>
		[TestMethod]
		public void Providers_GetProvsForClinic_UserProviderAcessForClinicNum() {
			Clinic clinic1=ClinicT.CreateClinic("Clinic1_"+MethodBase.GetCurrentMethod().Name);
			Clinic clinic2=ClinicT.CreateClinic("Clinic2_"+MethodBase.GetCurrentMethod().Name);
			Clinic clinic3=ClinicT.CreateClinic("Clinic3_"+MethodBase.GetCurrentMethod().Name);
			long provNum1=ProviderT.CreateProvider("Prov1", "FName1_"+MethodBase.GetCurrentMethod().Name);
			long provNum2=ProviderT.CreateProvider("Prov2", "FName2_"+MethodBase.GetCurrentMethod().Name);
			long provNum3=ProviderT.CreateProvider("Prov3", "FName3_"+MethodBase.GetCurrentMethod().Name); //Will not exist in provcliniclink (associated with 'All')
			long provNum4=ProviderT.CreateProvider("Prov4", "FName4_"+MethodBase.GetCurrentMethod().Name); //Will not exist in provcliniclink (associated with 'All')
			List<long> listLocalProvNums = new List<long>() { provNum1,provNum2,provNum3,provNum4 };
			Userod user1=UserodT.CreateUser("User1_"+MethodBase.GetCurrentMethod().Name,clinicNum:clinic3.ClinicNum,isClinicIsRestricted:true);
			user1.ProvNum=provNum3;
			UserClinics.Insert(new UserClinic(clinic3.ClinicNum,user1.UserNum));
			ProviderClinicLink pcLink1=new ProviderClinicLink(clinic1.ClinicNum,provNum1);
			ProviderClinicLink pcLink2=new ProviderClinicLink(clinic2.ClinicNum,provNum2);
			List<ProviderClinicLink> listProvClinicLinks=new List<ProviderClinicLink>(){ pcLink1,pcLink2 };
			ProviderClinicLinks.Sync(listProvClinicLinks,ProviderClinicLinks.GetDeepCopy());    //inserts new links into DB
																																													//Calling GetProvsForClinic returns all providers that clinic can access (both the ones associated with the specific clinic and ones associated to all clinics)
			List<Provider> listReturnedProvs=Providers.GetProvsForClinic(0);  //get providers for clinicNum=0 (HQ, all providers)
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum)); //exclude providers created in other unit tests
																																											//Returns all 4 providers, clinicNum=0 is HQ
			Assert.AreEqual(4,listReturnedProvs.Count());   //providers 1, 2, 3, and 4

			listReturnedProvs=Providers.GetProvsForClinic(clinic1.ClinicNum);
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic1 cannot access prov2 (because it is linked to clinic2)
			Assert.AreEqual(3,listReturnedProvs.Count());   //providers 1, 3, and 4

			listReturnedProvs=Providers.GetProvsForClinic(clinic2.ClinicNum);
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic2 cannot access prov1 (because it is linked to clinic1)
			Assert.AreEqual(3,listReturnedProvs.Count());   //providers 2, 3, and 4

			listReturnedProvs=Providers.GetProvsForClinic(clinic3.ClinicNum);
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic3 cannot access providers 1 and 2 (because they are linked to clinic1 and clinic2
			Assert.AreEqual(2,listReturnedProvs.Count());   //providers 3 and 4

		}

		///<summary> Tests the providers a user has access to based on a clinic number. Must refresh providercliniclink caches inside method to use.  </summary>
		[TestMethod]
		public void Providers_GetProvsForClinicList_UseProviderAccessForClinicList() {
			Clinic clinic1=ClinicT.CreateClinic("Clinic1_"+MethodBase.GetCurrentMethod().Name);
			Clinic clinic2=ClinicT.CreateClinic("Clinic2_"+MethodBase.GetCurrentMethod().Name);
			Clinic clinic3=ClinicT.CreateClinic("Clinic3_"+MethodBase.GetCurrentMethod().Name);
			Clinic clinic4=ClinicT.CreateClinic("Clinic4_"+MethodBase.GetCurrentMethod().Name);
			long provNum1=ProviderT.CreateProvider("Prov1", "FName1_"+MethodBase.GetCurrentMethod().Name);
			long provNum2=ProviderT.CreateProvider("Prov2", "FName2_"+MethodBase.GetCurrentMethod().Name);
			long provNum3=ProviderT.CreateProvider("Prov3", "FName3_"+MethodBase.GetCurrentMethod().Name); //Will not exist in provcliniclink (associated with 'All')
			long provNum4=ProviderT.CreateProvider("Prov4", "FName4_"+MethodBase.GetCurrentMethod().Name); //Will not exist in provcliniclink (associated with 'All')
			long provNum5=ProviderT.CreateProvider("Prov5", "FName5_"+MethodBase.GetCurrentMethod().Name); //Will not exist in provcliniclink (associated with 'All')
			List<long> listLocalProvNums=new List<long>() { provNum1,provNum2,provNum3,provNum4,provNum5 };
			//restrict User1(who is also provider1) to clinic1
			Userod user1=UserodT.CreateUser("User1",clinicNum:clinic1.ClinicNum,isClinicIsRestricted:true);
			user1.ProvNum=provNum1;
			UserClinics.Insert(new UserClinic(clinic1.ClinicNum,user1.UserNum));
			//link two of the providers to clinics
			ProviderClinicLink pcLink1=new ProviderClinicLink(clinic1.ClinicNum,provNum1);
			ProviderClinicLink pcLink2=new ProviderClinicLink(clinic2.ClinicNum,provNum2);
			ProviderClinicLink pcLink3=new ProviderClinicLink(clinic4.ClinicNum,provNum1);
			List<ProviderClinicLink> listProvClinic1and2Links=new List<ProviderClinicLink>(){ pcLink1,pcLink2,pcLink3};
			ProviderClinicLinks.Sync(listProvClinic1and2Links,ProviderClinicLinks.GetDeepCopy());    //inserts new link into DB

			//Calling GetProvsForClinicList returns all providers that each clinic can access (both the ones associated with a specific clinic and ones associated to all clinics)
			List<Provider> listReturnedProvs=Providers.GetProvsForClinicList(new List<long>() { 0 });
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Returns all 5 providers, clinicNum=0 is HQ
			Assert.AreEqual(5,listReturnedProvs.Count());

			listReturnedProvs=Providers.GetProvsForClinicList(new List<long>() { clinic1.ClinicNum });
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic1 cannot access prov2 (because it is linked to clinic2)
			Assert.AreEqual(4,listReturnedProvs.Count());

			listReturnedProvs=Providers.GetProvsForClinicList(new List<long>() { clinic2.ClinicNum });
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic2 cannot access prov1 (because it is linked to clinic1/clinic4)
			Assert.AreEqual(4,listReturnedProvs.Count());

			listReturnedProvs=Providers.GetProvsForClinicList(new List<long>() { clinic3.ClinicNum });
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic3 cannot access providers 1 and 2 (because they are linked to clinic1/clinic4 and clinic2)
			Assert.AreEqual(3,listReturnedProvs.Count());

			listReturnedProvs=Providers.GetProvsForClinicList(new List<long>() { clinic1.ClinicNum,clinic2.ClinicNum });
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Together, clinic1 and clinic2 can access all providers
			Assert.AreEqual(5,listReturnedProvs.Count());

			listReturnedProvs=Providers.GetProvsForClinicList(new List<long>() { clinic1.ClinicNum,clinic4.ClinicNum });
			listReturnedProvs.RemoveAll(x => !listLocalProvNums.Exists(y => y==x.ProvNum));
			//Clinic1 and clinic4 cannot access prov2 (because it is linked to clinic2)
			Assert.AreEqual(4,listReturnedProvs.Count());
		}
	}
}
