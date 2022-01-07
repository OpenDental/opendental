using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Clearinghouse_Tests {
	[TestClass]
	public class ClearinghouseTests:TestBase {

		///<summary>Add anything here that you want to run once before the tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			
		}
		
		///<summary>Add anything here that you want to run before every test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {

		}

		///<summary>Cleans up all Clearinghouse rows in the database between each test.</summary>
		[TestCleanup]
		public void TearDownTest() {
			ClearinghouseT.ClearClearinghouseTable();
		}
		
		///<summary>Add anything here that you want to run after all the tests in this class have been run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {

		}

		///<summary>Tests that the UpdateOverridesForClinic method used in FormClearinghouseEdit.cs properly
		///handles duplicate rows in the database. Every row entered for a given clinicNum should be identical.
		///For more information see jobnum 11387</summary>
		[TestMethod]
		public void Clearinghouses_UpdateOverridesForClinic_UpdateAllDuplicateOverrideRows() {
			string methodName=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(methodName);
			//Insert a default clearinghouse that is not associated to any clinics.  This will be used as the "HqClearinghouseNum" for creating duplicate overrides later.
			Clearinghouse clearinghouseHQ=ClearinghouseT.CreateClearinghouse(methodName);
			//Insert two blank Clearinghouse override rows associated to the same clinic into the DB to mimic the real life duplicate row issue.
			Clearinghouse clearinghouseOverride1=ClearinghouseT.CreateClearinghouse(methodName,clinic.ClinicNum,EclaimsCommBridge.ClaimConnect,
				ElectronicClaimFormat.x837D_5010_dental,clearinghouseHQ.HqClearinghouseNum,false,"test","pass");
			Clearinghouse clearinghouseOverride2=ClearinghouseT.CreateClearinghouse(methodName,clinic.ClinicNum,EclaimsCommBridge.ClaimConnect,
				ElectronicClaimFormat.x837D_5010_dental,clearinghouseHQ.HqClearinghouseNum,false,"test","pass");
			//Get all override rows from the DB
			List<Clearinghouse> listClearinghouseOverrides=Clearinghouses.GetAllNonHq();
			//Mimic the user making some changes to one of the overrides which was causing issues prior to the UpdateOverridesForClinic() paradigm.
			string passwordNew="Password1!";
			clearinghouseOverride2.Password=passwordNew;
			//FormClearinghouseEdit manipulates the clearinghouses in memory first but then calls the following method to "sync" all overrides before calling the clearinghouse sync.
			Clearinghouses.SyncOverridesForClinic(ref listClearinghouseOverrides,clearinghouseOverride2);
			//Check that both rows had their values updated to the Clearinghouse override's values.
			Clearinghouse co1=listClearinghouseOverrides.First(x => x.ClearinghouseNum==clearinghouseOverride1.ClearinghouseNum);
			Clearinghouse co2=listClearinghouseOverrides.First(x => x.ClearinghouseNum==clearinghouseOverride2.ClearinghouseNum);
			Assert.AreEqual(passwordNew,co1.Password);
			Assert.AreEqual(passwordNew,co2.Password);
		}

		///<summary>Tests that the UpdateOverridesForClinic method to make sure it only updates information relating to the clearinghouse passed in.
		///E.g. we don't want all clearinghouse overrides for a clinic to get updated, only some of them.</summary>
		[TestMethod]
		public void Clearinghouses_UpdateOverridesForClinic_UpdateOneOfMultiple() {
			string methodName=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(methodName);
			//Insert a default clearinghouse that is not associated to any clinics.  This will be used as the "HqClearinghouseNum" for creating duplicate overrides later.
			Clearinghouse clearinghouseHQClaimConnect=ClearinghouseT.CreateClearinghouse(methodName,commBridge:EclaimsCommBridge.ClaimConnect);
			//Insert two blank Clearinghouse override rows associated to the same clinic into the DB to mimic the real life duplicate row issue.
			Clearinghouse clearinghouseOverrideClaimConnect1=ClearinghouseT.CreateClearinghouse(methodName,clinic.ClinicNum,EclaimsCommBridge.ClaimConnect,
				ElectronicClaimFormat.x837D_5010_dental,clearinghouseHQClaimConnect.HqClearinghouseNum,false,"test","pass");
			Clearinghouse clearinghouseOverrideClaimConnect2=ClearinghouseT.CreateClearinghouse(methodName,clinic.ClinicNum,EclaimsCommBridge.ClaimConnect,
				ElectronicClaimFormat.x837D_5010_dental,clearinghouseHQClaimConnect.HqClearinghouseNum,false,"test","pass");
			//Make another HQ clearinghouse that is meant for a different eClaims comm bridge to make sure that all overrides associated to this clearinghouse do not change.
			Clearinghouse clearinghouseHQAOS=ClearinghouseT.CreateClearinghouse(methodName,commBridge:EclaimsCommBridge.AOS);
			//Insert two blank Clearinghouse override rows associated to the same clinic into the DB to mimic the real life duplicate row issue.
			Clearinghouse clearinghouseOverrideAOS=ClearinghouseT.CreateClearinghouse(methodName,clinic.ClinicNum,EclaimsCommBridge.AOS,
				ElectronicClaimFormat.x837D_5010_dental,clearinghouseHQAOS.HqClearinghouseNum,false,"test","pass");
			//Get all override rows from the DB
			List<Clearinghouse> listClearinghouseOverrides=Clearinghouses.GetAllNonHq();
			//Mimic the user making some changes to one of the overrides which was causing issues prior to the UpdateOverridesForClinic() paradigm.
			string passwordNew="Password1!";
			clearinghouseOverrideClaimConnect2.Password=passwordNew;
			//FormClearinghouseEdit manipulates the clearinghouses in memory first but then calls the following method to "sync" all overrides before calling the clearinghouse sync.
			Clearinghouses.SyncOverridesForClinic(ref listClearinghouseOverrides,clearinghouseOverrideClaimConnect2);
			//Check that both rows had their values updated to the Clearinghouse override's values.
			Assert.IsFalse(listClearinghouseOverrides.All(x => x.Password==passwordNew));
		}

	}
}
