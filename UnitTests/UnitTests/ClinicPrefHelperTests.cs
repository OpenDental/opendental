using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.ClinicPrefHelper_Tests {
	[TestClass]
	public class ClinicPrefHelperTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			ClinicT.ClearClinicTable();
			ClinicPrefT.ClearClinicPrefTable();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			ClinicT.ClearClinicTable();
			ClinicPrefT.ClearClinicPrefTable();
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void ClinicPrefHelper_GetClinicsWithChanges() {
			#region Test Setup
			//Set up clinics
			Prefs.UpdateBool(PrefName.EasyNoClinics,false);
			Clinic clinicA=ClinicT.CreateClinic(); //This clinic will always use defaults
			Clinic clinicB=ClinicT.CreateClinic(); //This clinic will start using defaults and then use its own values
			Clinic clinicC=ClinicT.CreateClinic(); //This clinic will always use its own values, which will not change
			Clinic clinicD=ClinicT.CreateClinic(); //This clinic will always use its own values, which will change
			Clinic clinicE=ClinicT.CreateClinic(); //This clinic will start using its own values, and then change to the defaults
			//Set up the baseline preferences
			//using EClipboard prefs because they are actually used as clinic prefs, but any pref could be used
			Prefs.UpdateBool(PrefName.EClipboardAllowSelfCheckIn,true);
			Prefs.UpdateBool(PrefName.EClipboardAllowSelfPortraitOnCheckIn,true);
			//Set up the clinic preferences
			ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfCheckIn,clinicC.ClinicNum,POut.Bool(false));
			ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicC.ClinicNum,POut.Bool(false));
			ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfCheckIn,clinicD.ClinicNum,POut.Bool(false));
			ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicD.ClinicNum,POut.Bool(false));
			ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfCheckIn,clinicE.ClinicNum,POut.Bool(false));
			ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicE.ClinicNum,POut.Bool(false));
			ClinicPrefs.RefreshCache();
			//Set up the ClinicPrefHelper
			OpenDental.ClinicPrefHelper clinicPrefHelper=new OpenDental.ClinicPrefHelper(PrefName.EClipboardAllowSelfCheckIn,PrefName.EClipboardAllowSelfPortraitOnCheckIn);
			#endregion Test Setup
			#region Test Body
			//Make the changes to the preferences
			//Update one of the default preference values
			clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,0,POut.Bool(false));
			//Update ClinicB to use one of its own values
			clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicB.ClinicNum,POut.Bool(false));
			//Update clinicD to change its own values
			clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicD.ClinicNum,POut.Bool(true));
			//Remove all clinicE prefs to indicate it should use the default
			clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,clinicE.ClinicNum,POut.Bool(clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardAllowSelfCheckIn)));
			clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfPortraitOnCheckIn,clinicE.ClinicNum,POut.Bool(clinicPrefHelper.GetDefaultBoolVal(PrefName.EClipboardAllowSelfPortraitOnCheckIn)));
			#endregion Test Body
			#region Test Results
			//Test that GetClinicsWithChanges gets the right clinics for each EClipboardAllowSelfCheckIn
			List<long> listExpectedClinicNumsChanged_A=new List<long>() {
				0, //We changed the default preference
				clinicD.ClinicNum, //We changed this preference
				clinicE.ClinicNum //We removed these preferences
			};
			List<long> results=clinicPrefHelper.GetClinicsWithChanges(PrefName.EClipboardAllowSelfCheckIn);
			Assert.AreEqual(3,results.Union(listExpectedClinicNumsChanged_A).Distinct().Count());
			//Test that GetClinicsWithChanges gets the right clinics for each EClipboardAllowSelfCheckIn
			List<long> listExpectedClinicNumsChanged_B=new List<long>() {
				clinicB.ClinicNum, //We changed this preference
				clinicE.ClinicNum //We removed these preferences
			};
			results=clinicPrefHelper.GetClinicsWithChanges(PrefName.EClipboardAllowSelfPortraitOnCheckIn);
			Assert.AreEqual(2,results.Union(listExpectedClinicNumsChanged_B).Distinct().Count());
			#endregion Test Results
			#region Test Clean Up
			ClinicPrefs.Sync(new List<ClinicPref>(),ClinicPrefs.GetPrefAllClinics(PrefName.EClipboardAllowSelfCheckIn));
			ClinicPrefs.Sync(new List<ClinicPref>(),ClinicPrefs.GetPrefAllClinics(PrefName.EClipboardAllowSelfPortraitOnCheckIn));
			Clinics.Delete(clinicA);
			Clinics.Delete(clinicB);
			Clinics.Delete(clinicC);
			Clinics.Delete(clinicD);
			Clinics.Delete(clinicE);
			#endregion Test Clean Up
		}

		[TestMethod]
		public void ClinicPrefHelper_SyncPrefs() {
			#region Test Setup
			//Set up the initial preference value
			Prefs.UpdateBool(PrefName.EClipboardAllowSelfCheckIn,true);
			//Create several clinics to use
			Prefs.UpdateBool(PrefName.EasyNoClinics,false);
			List<Clinic> listClinics=new List<Clinic>();
			for(int i = 0;i<10;i++) {
				listClinics.Add(ClinicT.CreateClinic());
			}
			//Set up the initial preference values
			System.Random rnd=new System.Random();
			foreach(Clinic cl in listClinics) {
				ClinicPrefs.InsertPref(PrefName.EClipboardAllowSelfCheckIn,cl.ClinicNum,POut.Bool(rnd.Next(2)==0));
			}
			ClinicPrefs.RefreshCache();
			//Set up the ClinicPrefHelper
			OpenDental.ClinicPrefHelper clinicPrefHelper=new OpenDental.ClinicPrefHelper(PrefName.EClipboardAllowSelfCheckIn);
			#endregion Test Setup
			#region Test Body
			//Randomyly change the clinic preferences
			List<Tuple<long,string>> listChanges=new List<Tuple<long, string>>();
			foreach(Clinic cl in listClinics) {
				string val=POut.Bool(rnd.Next(2)==0);
				clinicPrefHelper.ValChangedByUser(PrefName.EClipboardAllowSelfCheckIn,cl.ClinicNum,val);
				listChanges.Add(Tuple.Create(cl.ClinicNum,val));
			}
			//Sync the prefs and handle the other elements this method does
			clinicPrefHelper.SyncPref(PrefName.EClipboardAllowSelfCheckIn);
			#endregion Test Body 
			#region Test Results
			//SyncPref should have refreshed the cache so we are good to get the list
			List<ClinicPref> listActual=ClinicPrefs.GetPrefAllClinics(PrefName.EClipboardAllowSelfCheckIn);
			foreach(ClinicPref cp in listActual) {
				Assert.AreEqual(cp.ValueString,listChanges.First(x => x.Item1==cp.ClinicNum).Item2);
			}
			#endregion Test Results
		}

	}
}
