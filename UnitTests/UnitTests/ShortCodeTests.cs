using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class ShortCodeTests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			if(Clinics.GetCount()==0) {
				for(int i=1;i<5;i++) {
					Clinics.Insert(new Clinic());
				}
				Clinics.RefreshCache();
			}
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			ClinicPrefT.ClearClinicPrefTable();
			ToggleTexting(true);
			TurnOffAllPrefs();
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			ClinicPrefT.ClearClinicPrefTable();
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>Turns off clinics.  Ensures all eServices implemented for Short Codes are not enabled.  Then iterates through each Short Code
		///eService, toggling each associated preference into an enabled state, and tests that the service is detected as enabled.</summary>
		[TestMethod]
		public void ShortCode_Attribute_IsServiceEnabledClinicsOff() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,true);
			Prefs.RefreshCache();
			foreach(ShortCodeAttribute sc in GetAllShortCodeAttributes()) {
				if(sc.EServicePrefNames.Contains(PrefName.NotApplicable)) {
					//When NotApplicable is used as the pref, having texting enabled is the only gatekeeper.  We are trying to test the other preferences here,
					//not simply if Texting is enabled.
					ToggleTexting(false);
				}
				Assert.IsFalse(sc.IsServiceEnabled(0),$"{sc.SmsMessageSource[0]} should not be enabled.");
				if(sc.EServicePrefNames.Contains(PrefName.NotApplicable)) {
					ToggleTexting(true);
				}
			}
			foreach(ShortCodeAttribute sc in GetAllShortCodeAttributes()) {
				foreach(PrefName pref in sc.EServicePrefNames) {
					int[] arrValues=sc.PrefIsEnabledValues??new int[] { 1 };
					foreach(int value in arrValues) {
						ToggleTexting(true);
						UpdatePref(pref,0,value,true);//Turn pref on.
						//Detect Short Codes are enabled.
						Assert.IsTrue(sc.IsServiceEnabled(0),$"PrefName.{pref.ToString()} set to {value}, but {sc.SmsMessageSource[0]} not enabled.");
						int reset=0;
						while(ListTools.In(reset,arrValues)) { reset++; }
						ToggleTexting(false);
						UpdatePref(pref,0,reset,true);//Turn pref off.
						Assert.IsFalse(sc.IsServiceEnabled(0),$"PrefName.{pref.ToString()} set to {value}, but {sc.SmsMessageSource[0]} is enabled.");
					}
				}
			}
		}

		///<summary>Turns on clinics.  Ensures all eServices implemented for Short Codes are not enabled.  Then iterates through each Short Code
		///eService on each clinic, toggling each associated preference into an enabled state, and tests that the service is detected as enabled.
		///</summary>
		[TestMethod]
		public void ShortCode_Attribute_IsServiceEnabledClinicsOn() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);			
			List<long> listClinicNums=new List<long>() { 0 };
			listClinicNums.AddRange(Clinics.GetDeepCopy().Select(x => x.ClinicNum));
			foreach(long clinicNum in listClinicNums) {
				ToggleTexting(false);
				foreach(ShortCodeAttribute sc in GetAllShortCodeAttributes()) {
					if(sc.EServicePrefNames.Contains(PrefName.NotApplicable)) {
						//When NotApplicable is used as the pref, having texting enabled is the only gatekeeper.  We are trying to test the other preferences here,
						//not simply if Texting is enabled.
						ToggleTexting(false);
					}
					Assert.IsFalse(sc.IsServiceEnabled(clinicNum),$"{sc.SmsMessageSource[0]} should not be enabled.");					
					if(sc.EServicePrefNames.Contains(PrefName.NotApplicable)) {
						ToggleTexting(true);
					}
				}
				foreach(ShortCodeAttribute sc in GetAllShortCodeAttributes()) {
					foreach(PrefName pref in sc.EServicePrefNames) {
						int[] arrValues=sc.PrefIsEnabledValues??new int[] { 1 };
						foreach(int value in arrValues) {
							UpdatePref(pref,clinicNum,value,true);//Turn pref on.
							ToggleTexting(true);
							//Detect Short Codes are enabled.
							Assert.IsTrue(sc.IsServiceEnabled(clinicNum),$"PrefName.{pref.ToString()} set to {value}, but {sc.SmsMessageSource[0]} not enabled.");
							int reset=0;
							while(ListTools.In(reset,arrValues)) { reset++; }
							UpdatePref(pref,clinicNum,reset,true);//Turn pref off.
							ToggleTexting(false);
							Assert.IsFalse(sc.IsServiceEnabled(clinicNum),$"PrefName.{pref.ToString()} set to {value}, but {sc.SmsMessageSource[0]} is enabled.");
						}
					}
				}
			}
		}

		private void ToggleTexting(bool isOn) {
			DateTime contractDate=isOn ? DateTime.Today.AddDays(-1) : DateTime.MinValue;
			PrefT.UpdateDateT(PrefName.SmsContractDate,contractDate);
			PrefT.UpdateLong(PrefName.TextingDefaultClinicNum,Clinics.GetFirst().ClinicNum);
			foreach(Clinic clinic in Clinics.GetDeepCopy()) {
				clinic.SmsContractDate=contractDate;
				Clinics.Update(clinic);
			}
			Prefs.RefreshCache();
			Clinics.RefreshCache();
		}

		private void TurnOffAllPrefs() {			
			List<long> listClinicNums=new List<long>() { 0 };
			listClinicNums.AddRange(Clinics.GetDeepCopy().Select(x => x.ClinicNum));
			foreach(long clinicNum in listClinicNums) {
				foreach(ShortCodeAttribute sc in GetAllShortCodeAttributes()) {
					foreach(PrefName pref in sc.EServicePrefNames) {
						int[] arrValues=sc.PrefIsEnabledValues??new int[] { 1 };
						foreach(int value in arrValues) {
							int reset=0;
							while(ListTools.In(reset,arrValues)) { reset++; }
							UpdatePref(pref,0,reset);//Turn pref off.
						}
					}
				}
			}
			ClinicPrefs.RefreshCache();
			Prefs.RefreshCache();
		}

		private void UpdatePref(PrefName pref,long clinicNum,int value,bool doRefreshCache=false) {
			if(clinicNum>0){
				ClinicPrefs.Upsert(pref,clinicNum,value.ToString());
				if(doRefreshCache) {
					ClinicPrefs.RefreshCache();
				}
			}
			else {
				PrefT.UpdateString(pref,value.ToString());
				if(doRefreshCache) {
					Prefs.RefreshCache();
				}
			}
		}

		private List<ShortCodeAttribute> GetAllShortCodeAttributes() {
			return Enum.GetValues(typeof(ShortCodeTypeFlag))
				.AsEnumerable<ShortCodeTypeFlag>()
				.Where(x => x!=ShortCodeTypeFlag.None)
				.Select(x => EnumTools.GetAttributeOrDefault<ShortCodeAttribute>(x))
				.ToList();
		}

	}
}
