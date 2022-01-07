using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Reactivations_Tests {
	[TestClass]
	public class ReactivationsTests:TestBase {

		private static long _reactivationCommLogType=0;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Set all the reactivations prefs
			PrefName.ShowFeatureReactivations.Update(true);
			PrefName.ReactivationContactInterval.Update(180); //~6 months
			PrefName.ReactivationCountContactMax.Update(2);
			PrefName.ReactivationDaysPast.Update(730); //Two years
			PrefName.ReactivationEmailFamMsg.Update(""); //Set these later if they're needed for a test
			PrefName.ReactivationEmailMessage.Update(""); //Set these later if they're needed for a test
			PrefName.ReactivationEmailSubject.Update(""); //Set these later if they're needed for a test
			PrefName.ReactivationGroupByFamily.Update(false);
			PrefName.ReactivationPostcardFamMsg.Update(""); //Set these later if they're needed for a test
			PrefName.ReactivationPostcardMessage.Update(""); //Set these later if they're needed for a test
			PrefName.ReactivationPostcardsPerSheet.Update(3);
			//Set the defs for reactivation statuses
			long reactEmailed=Defs.Insert(new Def() { Category=DefCat.RecallUnschedStatus,ItemName="Reactivation E-Mailed" });
			PrefName.ReactivationStatusEmailed.Update(reactEmailed);
			long reactEmailedTexted=Defs.Insert(new Def() { Category=DefCat.RecallUnschedStatus,ItemName="Reactivation E-Mailed/Texted" });
			PrefName.ReactivationStatusEmailedTexted.Update(reactEmailedTexted);
			long reactMailed=Defs.Insert(new Def() { Category=DefCat.RecallUnschedStatus,ItemName="Reactivation Mailed" });
			PrefName.ReactivationStatusMailed.Update(reactMailed);
			long reactTexted=Defs.Insert(new Def() { Category=DefCat.RecallUnschedStatus,ItemName="Reactivation Texted" });
			PrefName.ReactivationStatusTexted.Update(reactTexted);
			//Create a commlog type for reactivations
			_reactivationCommLogType=Defs.Insert(new Def() { Category=DefCat.CommLogTypes,ItemName="ReactivationsComm",ItemValue="REACT" });
			Defs.RefreshCache();
		}

		[TestInitialize]
		public void SetupTest() {
			PatientT.ClearPatientTable();
			AppointmentT.ClearAppointmentTable();
			ReactivationT.ClearReactivationTable();
			ClinicT.ClearClinicTable();
		}

		[ClassCleanup]
		public static void TearDownClass() {
			PrefName.ShowFeatureReactivations.Update(false);
		}

		[TestMethod]
		public void Reactivations_GetReactivationList_HappyPath() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Patient has been contacted, and the ReactivationContactInterval has elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddYears(-1),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			DateTime dateSince=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.ReactivationDaysPast));
			DateTime dateStop=dateSince.AddMonths(-36);
			//Confirm that the patient appears in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//Only one patient should be in the list
			Assert.AreEqual(1,tbl.Rows.Count);
			Assert.AreEqual(pat.PatNum,PIn.Int(tbl.Rows[0]["PatNum"].ToString()));
		}

		[TestMethod]
		public void Reactivations_GetReactivationList_DaysPastHasFutureAppt() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Operatory op=OperatoryT.CreateOperatory(name,provDentist:provNum,clinicNum:clinic.ClinicNum);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Patient has been contacted, and the ReactivationContactInterval has elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddYears(-1),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			//Patient has a future appointment scheduled.
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddDays(1),op.OperatoryNum,provNum);
			DateTime dateSince=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.ReactivationDaysPast));
			DateTime dateStop=dateSince.AddMonths(-36);
			//Confirm that the patient does not in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//No patients in the list
			Assert.AreEqual(0,tbl.Rows.Count);
		}

		[TestMethod]
		public void Reactivations_GetReactivationList_ContactIntervalNotElapsed() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Patient has been contacted, but the ReactivationContactInterval has not elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddDays(-30),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			DateTime dateSince=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.ReactivationDaysPast));
			DateTime dateStop=dateSince.AddMonths(-36);
			//Confirm that the patient does not in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//No patients in the list
			Assert.AreEqual(0,tbl.Rows.Count);
		}

		[TestMethod]
		public void Reactivations_GetReactivationList_ExceedsContactMax() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Patient has been contacted the max amount of times
			for(int i=0;i<PrefC.GetInt(PrefName.ReactivationCountContactMax);i++) {
				//Patient has been contacted, and the ReactivationContactInterval has elapsed.
				Commlog comm=new Commlog()
				{
					PatNum=pat.PatNum,
					CommDateTime=DateTime.Now.AddYears(-2),
					CommType=_reactivationCommLogType,
					Mode_=CommItemMode.Email,
					SentOrReceived=CommSentOrReceived.Sent,
					CommSource=CommItemSource.ApptReminder,
				};
				comm.CommlogNum=Commlogs.Insert(comm);
			}
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			DateTime dateSince=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.ReactivationDaysPast));
			DateTime dateStop=dateSince.AddMonths(-36);
			//Confirm that the patient does not show in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.All);
			//No patients in the list
			Assert.AreEqual(0,tbl.Rows.Count);
		}

		[TestMethod]
		public void Reactivations_GetReactivationList_PatientMarkedDoNotContact() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Patient has been contacted, and the ReactivationContactInterval has elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddYears(-1),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=true,
			});
			DateTime dateSince=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.ReactivationDaysPast));
			DateTime dateStop=dateSince.AddMonths(-36);
			//Confirm that the patient does not in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//No patients in the list
			Assert.AreEqual(0,tbl.Rows.Count);
		}

		/// <summary>Tests that if the patient's most recent procedure is before the earliest date allowed in the query, that the patient does not show
		/// in the Reactivation List.</summary>
		[TestMethod]
		public void Reactivations_GetReactivationList_DateStop() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail);
			DateTime dateSince=new DateTime(2019,9,3);
			DateTime dateStop=dateSince.AddDays(-1);
			DateTime dateProc=dateStop.AddDays(-1);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:dateProc,provNum:provNum);
			//Patient has been contacted, and the ReactivationContactInterval has elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddYears(-1),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//Confirm that no patient appears in the Reactivation List
			tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//No patient should be in the list
			Assert.AreEqual(0,tbl.Rows.Count);
		}

		/// <summary>Tests that Inactive patients are not included when passing isInactiveShow=false.
		/// </summary>
		[TestMethod]
		public void Reactivations_GetReactivationList_ExcludeInactive() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail
				,patStatus:PatientStatus.Inactive);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			Procedure proc=ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Patient has been contacted, and the ReactivationContactInterval has elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddYears(-1),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			DateTime dateSince=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.ReactivationDaysPast));
			DateTime dateStop=dateSince.AddMonths(-36);
			//Confirm that no patient appears in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,false,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//No patient should be in the list
			Assert.AreEqual(0,tbl.Rows.Count);
		}

		///<summary>Test to make sure missed or broken appointment procedures do not count as true completed proceudres in terms of getting 
		///the reactivation list.</summary>
		[TestMethod]
		public void Reactivations_GetReactivationList_ExcludeMissedOrBrokenProcedures() {
			string name=MethodBase.GetCurrentMethod().Name;
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Patient pat=PatientT.CreatePatient(name,provNum,clinic.ClinicNum,TestEmaiAddress,TestPatPhone,ContactMethod.Mail
				,patStatus:PatientStatus.Inactive);
			//Patient has not been seen since further in the past than the ReactivationDaysPast preference.
			ProcedureT.CreateProcedure(pat,"D0120",ProcStat.C,"",50,procDate:DateTime.Now.AddYears(-3),provNum:provNum); //3 year old proc
			//Create another procedure that the patient was scheduled for but ended up being a no-show (missed/cancelled)
			ProcedureT.CreateProcedure(pat,"D9986",ProcStat.C,"",0,procDate:DateTime.Now.AddYears(-1),provNum:provNum);
			ProcedureT.CreateProcedure(pat,"D9987",ProcStat.C,"",0,procDate:DateTime.Now.AddYears(-1),provNum:provNum);
			//Patient has been contacted, and the ReactivationContactInterval has elapsed.
			Commlog comm=new Commlog()
			{
				PatNum=pat.PatNum,
				CommDateTime=DateTime.Now.AddYears(-1),
				CommType=_reactivationCommLogType,
				Mode_=CommItemMode.Email,
				SentOrReceived=CommSentOrReceived.Sent,
				CommSource=CommItemSource.ApptReminder,
			};
			comm.CommlogNum=Commlogs.Insert(comm);
			//Patient has not been marked "Do Not Contact"
			Reactivations.Insert(new Reactivation() {
				PatNum=pat.PatNum,
				DoNotContact=false,
			});
			//Reactivation range will be 3 years ago to 2 years ago
			DateTime dateSince=DateTime.Today.AddYears(-2);
			DateTime dateStop=DateTime.Today.AddYears(-3);
			//Confirm that the patient appears in the Reactivation List
			DataTable tbl=Reactivations.GetReactivationList(dateSince,dateStop,false,false,true,provNum,clinic.ClinicNum,0,0
				,ReactivationListSort.LastContacted,RecallListShowNumberReminders.One);
			//Verify that the patient still shows up in the list and that those missed procedures don't exclude them
			Assert.AreEqual(1,tbl.Rows.Count);
			Assert.AreEqual(pat.PatNum,PIn.Int(tbl.Rows[0]["PatNum"].ToString()));
		}

	}
}
