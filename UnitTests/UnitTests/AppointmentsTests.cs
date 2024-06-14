using System;
using System.Collections.Generic;
using System.Linq;
using OpenDentBusiness;
using UnitTestsCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using CodeBase;
using System.Data;
using System.Collections;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;

namespace UnitTests.Appointments_Tests {
	//All tests are assumed to be non dynamic unless specified. 
	[TestClass]
	public class AppointmentsTests:TestBase {
		[TestInitialize]
		public void SetupTest() {
			//Add anything here that you want to run before every test in this class.
			AppointmentT.ClearAppointmentTable();
			AppointmentRuleT.ClearAppointmentRuleTable();
			DefLinkT.ClearDefLinkTable();
			DefT.DeleteAllForCategory(DefCat.WebSchedNewPatApptTypes);
			OperatoryT.ClearOperatoryTable();
			RecallT.ClearRecallTable();
			RecallTypeT.ClearRecallTypeTable();
			ScheduleT.ClearScheduleTable();
			ScheduleOpT.ClearScheduleOpTable();
		}
	
		#region appointment search
		#region non dynamic
		[TestMethod]
		public void Appointments_GetSearchResults_GetBasicSearchResults() {
			//Very simple base test that just uses one provider, one operatory, one base schedule repeated, one appointment (the one adding, no conflicts) 
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			//manipulate the test data
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNum); 
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,opScheduling.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {opScheduling.OperatoryNum };
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
				,listOpsInView,new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			//assert that they all show results for 8 am. 
			Assert.AreEqual(10,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		///<summary>Simple base test that just uses one provider, one operatory, one base schedule repeated, with one existing scheduled appointment. 
		///Ensures Broken appointments are ignored and ApptSearch considers them as scheduable times for new appointments.</summary>
		[TestMethod]
		public void Appointments_GetSearchResults_IgnoreBrokenAppointments() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNum);
			//Create appointment that the search will be to find a spot for.
			Appointment apptOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,opNum:0,provNum,aptStatus:ApptStatus.UnschedList);
			//Create existing appointment on the schedule tomorrow 8-9am
			Appointment apptScheduled=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Today.AddDays(1).AddHours(8),opScheduling.OperatoryNum,provNum,pattern:"////XXXX////");
			//Create ApptSearch params
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime(hour:8);
			List<long> listOpsInView=new List<long>() {opScheduling.OperatoryNum};
			List<long> listProvNums=new List<long>() {provNum};
			List<long> listClinicNums=new List<long>() { }; //No Clinics
			//First - confirm first possible opening for this appointment is after existing appointment (9am)
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(apptOnPinBoard.AptNum,dateStart:DateTime.Today,dateEnd:DateTime.Now.AddMonths(1)
				,listProvNums,listOpsInView,listClinicNums,beforeTime,afterTime);
			//Assert first opening for 9 am.
			ScheduleOpening scheduleOpeningFirstAvailable=searchResults.FirstOrDefault();
			Assert.AreEqual(9,scheduleOpeningFirstAvailable.DateTimeAvail.Hour);
			Assert.AreEqual(00,scheduleOpeningFirstAvailable.DateTimeAvail.Minute);
			//Second - Break existing appointment
			Appointment apptBroken=apptScheduled.Copy();
			apptBroken.AptStatus=ApptStatus.Broken; //Emulates right-clicking to Break in Appointment Module and "Leave on Appointment Book"
			Appointments.Update(apptBroken,apptScheduled);
			//Search again for openings.
			searchResults=ApptSearch.GetSearchResults(apptOnPinBoard.AptNum,dateStart:DateTime.Today,dateEnd:DateTime.Now.AddMonths(1)
				,listProvNums,listOpsInView,listClinicNums,beforeTime,afterTime);
			//Assert first opening is now 8am (Broken appointment ignored)
			scheduleOpeningFirstAvailable=searchResults.FirstOrDefault();
			Assert.AreEqual(8,scheduleOpeningFirstAvailable.DateTimeAvail.Hour);
			Assert.AreEqual(00,scheduleOpeningFirstAvailable.DateTimeAvail.Minute);
		}

		#region ProviderTime
		[TestMethod]
		public void Appointments_GetSearchResults_GetBasicSearchResultsWithConflictsForProviderTime() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			//manipulate the test data
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNum);//get "one" of the ops for the provider. 
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,opScheduling.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView = new List<long>() {opScheduling.OperatoryNum };
			//make other appointments to see we don't get results during that time
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),opScheduling.OperatoryNum,provNum);
			Patient patConflict2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict2");
			AppointmentT.CreateAppointment(patConflict2.PatNum
				,new DateTime(appSearchData.ListSchedules[2].SchedDate.Year,appSearchData.ListSchedules[2].SchedDate.Month
				,appSearchData.ListSchedules[2].SchedDate.Day,8,20,0),opScheduling.OperatoryNum,provNum);
			Patient patConflict3=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict3");
			AppointmentT.CreateAppointment(patConflict3.PatNum
				,new DateTime(appSearchData.ListSchedules[3].SchedDate.Year,appSearchData.ListSchedules[3].SchedDate.Month
				,appSearchData.ListSchedules[3].SchedDate.Day,8,30,0),opScheduling.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long> { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(10,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==30).Count);
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==00).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvWithOneOpConflictProvTime() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);//starts with today, go one more than what shows in result
			//manipulate the test data
			Operatory primaryOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory secondaryOp=appSearchData.ListOps[1];//additonal operatory for the provider
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,primaryOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {primaryOp.OperatoryNum,secondaryOp.OperatoryNum };
			//create conflicts for the first schedule time. 
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),secondaryOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long> { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==30 
			&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvWithOpConflictsProvTime() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);//starts with today, go one more than what shows in result
			//manipulate the test data
			Operatory primaryOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory secondaryOp=appSearchData.ListOps[1];//additonal operatory for the provider
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,primaryOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {primaryOp.OperatoryNum,secondaryOp.OperatoryNum };
			//create conflicts for the first schedule time. 
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),primaryOp.OperatoryNum,provNum);
			Patient patConflict2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict2");
			AppointmentT.CreateAppointment(patConflict2.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,30,0),secondaryOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long> { },beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time Operatory
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==50 
				&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
		} 

		[TestMethod]
		public void Appointments_TryAddPerVisitProcCodesToAppt_PlannedToScheduledToComplete() {
			//Models and appointment going from Planned to Scheduled to complete. Adds both procs.
			ProcedureCodeT.AddIfNotPresent("D9430");
			ProcedureCodeT.AddIfNotPresent("D9440");
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"D9430");//Set Per Visit Prefs.
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"D9440");
			//create provider and schedule.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			//Create a new primary insurance plan for this patient.
			Carrier carrier=CarrierT.CreateCarrier("fake carrier");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(0,patient.PatNum,insSub.InsSubNum);
			insPlan.PerVisitPatAmount=1;//Set Per Visit copay amounts.
			insPlan.PerVisitInsAmount=2;
			InsPlans.Update(insPlan);
			Appointment appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Planned);//Create appointment with AptStatus=Complete.
			//Creates a planned appointment then sets it to scheduled which adds the PerVisit Procs, then sets the appointment complete and does not add the procs again because they were already added when scheduled.
			Appointment appointmentOld=appointment.Copy();
			appointment.AptStatus=ApptStatus.Scheduled;
			Appointments.Update(appointment,appointmentOld);
			List<Procedure> listProcedures=Appointments.TryAddPerVisitProcCodesToAppt(appointment,appointmentOld.AptStatus);//This will add both Per Visit Procs and return list of per visit procs added.
			//Assert that appointment only has two procedures.
			Assert.AreEqual(2,listProcedures.Count);
			Appointments.CompleteClick(appointment,new List<Procedure>(),false);//Calls TryAddPerVisitProcCodesToAppt() and does not add Per Visit Procs.
			//Assert that there are only two procs for the appointment. (PerVisitPat and PerVisitIns)
			listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,false);
			//Assert that appointment only has two procedures.
			Assert.AreEqual(2,listProcedures.Count);
			//Assert that appointment CodenNums match.
			Assert.AreEqual(ProcedureCodes.GetCodeNum("D9430"),listProcedures[0].CodeNum);
			Assert.AreEqual(ProcedureCodes.GetCodeNum("D9440"),listProcedures[1].CodeNum);
		} 

		[TestMethod]
		public void Appointments_TryAddPerVisitProcCodesToAppt_ScheduledToScheduled() {
			//Models and appointment going from Scheduled to Scheduled so it will not add procs.
			ProcedureCodeT.AddIfNotPresent("D9430");
			ProcedureCodeT.AddIfNotPresent("D9440");
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"D9430");//Sets Per Visit Prefs.
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"D9440");
			//create provider and schedule.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			//Create a new primary insurance plan for this patient.
			Carrier carrier=CarrierT.CreateCarrier("fake carrier");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(0,patient.PatNum,insSub.InsSubNum);
			insPlan.PerVisitPatAmount=1;//Set Per Visit copay amounts.
			insPlan.PerVisitInsAmount=2;
			InsPlans.Update(insPlan);
			Appointment appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Scheduled);//Create appointment with AptStatus=Scheduled.
			List<Procedure> listProcedures=Appointments.TryAddPerVisitProcCodesToAppt(appointment,ApptStatus.Scheduled);//This will not add both Per Visit Procs and return an empty list.
			//Assert that appointment does not have any procedures.
			Assert.AreEqual(0,listProcedures.Count);
		} 

		[TestMethod]
		public void Appointments_TryAddPerVisitProcCodesToAppt_ScheduledToCompletePerVisitInsOnly() {
			//Models and appointment going from Scheduled to complete. Only adds Per Visit Ins proc.
			ProcedureCodeT.AddIfNotPresent("D9430");
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"");//Sets Per Visit Prefs.
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"D9430");//Sets Per Visit Prefs.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			//Create a new primary insurance plan for this patient.
			Carrier carrier=CarrierT.CreateCarrier("fake carrier");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(0,patient.PatNum,insSub.InsSubNum);
			insPlan.PerVisitInsAmount=2;//Set Per Visit copay amounts.
			InsPlans.Update(insPlan);
			Appointment appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Scheduled);//Create appointment with AptStatus=Scheduled.
			Appointments.CompleteClick(appointment,new List<Procedure>(),false);//Calls TryAddPerVisitProcCodesToAppt() and only adds Per Visit Ins proc.
			List<Procedure> listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,false);
			//Assert that appointment only has one procedures.
			Assert.AreEqual(1,listProcedures.Count);
			//Assert that appointment CodenNum matches.
			Assert.AreEqual(ProcedureCodes.GetCodeNum("D9430"),listProcedures[0].CodeNum);
		} 

		[TestMethod]
		public void Appointments_TryAddPerVisitProcCodesToAppt_ScheduledToCompleteNoPrefs() {
			//Models and appointment going from Scheduled to complete. Per Visit Prefs are empty so procs will not be added.
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"");//Sets Per Visit Prefs to empty.
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"");
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			//Create a new primary insurance plan for this patient.
			Carrier carrier=CarrierT.CreateCarrier("fake carrier");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(0,patient.PatNum,insSub.InsSubNum);
			insPlan.PerVisitInsAmount=2;//Set Per Visit copay amounts.
			InsPlans.Update(insPlan);
			Appointment appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Scheduled);//Create appointment with AptStatus=Scheduled.
			Appointments.CompleteClick(appointment,new List<Procedure>(),false);//Calls TryAddPerVisitProcCodesToAppt() and does not add Per Visit Procs.
			List<Procedure> listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,false);
			//Assert that appointment only has one procedures.
			Assert.AreEqual(0,listProcedures.Count);
		} 

		[TestMethod]
		public void Appointments_TryAddPerVisitProcCodesToAppt_ScheduledToCompleteNoInsSub() {
			//Models and appointment going from Scheduled to complete. There is not InsSub so procs won't be added.
			ProcedureCodeT.AddIfNotPresent("D9430");
			ProcedureCodeT.AddIfNotPresent("D9440");
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"D9430");//Sets Per Visit Prefs.
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"D9440");
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			//Create a new primary insurance plan for this patient.
			Carrier carrier=CarrierT.CreateCarrier("fake carrier");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			//InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum); // No insSub.
			//PatPlanT.CreatePatPlan(0,patient.PatNum,insSub.InsSubNum);
			insPlan.PerVisitPatAmount=1;//Set Per Visit copay amounts.
			insPlan.PerVisitInsAmount=2;
			InsPlans.Update(insPlan);
			Appointment appointment=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Scheduled);//Create appointment with AptStatus=Scheduled.
			Appointments.CompleteClick(appointment,new List<Procedure>(),false);//Calls TryAddPerVisitProcCodesToAppt() and does not add Per Visit Procs.
			//Assert that appointment only has one procedures.
			List<Procedure> listProcedures=Procedures.GetProcsForSingle(appointment.AptNum,false);
			Assert.AreEqual(0,listProcedures.Count);
		} 

		[TestMethod]
		public void Appointments_GetDataForSearch_SearchResultsForHygenist() {
			//Prov is scheduled in two ops. There are appointment conflicts in both ops, but only one of the ops is in the current appointment view.
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provHyg=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvHygienTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today, go one more than what shows in result
			AppointmentSearchData hygAppData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provHyg,11);
			//manipulate the test data
			Operatory provOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory hygOp=hygAppData.ListOps[0];//op for the hygienist
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,provOp.OperatoryNum,provNum,isHygiene:true);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			//Make an appointment view that does not have the provider in it at all. Only the hyg op. 
			List<long> listOpsInView=new List<long>() {provOp.OperatoryNum,hygOp.OperatoryNum };
			ApptSearchData data=ApptSearch.GetDataForSearch(patApt.AptNum,DateTime.Now,DateTime.Now.AddYears(2),new List<long>() {provHyg},listOpsInView
				,new List<long>(),new List<long>(){0});
			//Assert that appointment time is correct for provider time
			//No search results should be returned. 
			Assert.AreEqual(1,data.ListSchedules.Select(x => x.ProvNum).Distinct().Count());
			Assert.AreEqual(provHyg,data.ListSchedules[0].ProvNum);//all schedules should be for the hygienist
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvOnlyOneInClinicWithOpConflictsProvTime() {
			//Prov is scheduled in two ops, that serve as two clinics for this test. There are appointment conflicts in both ops, but only one of the ops is in the current "clinic".
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);//starts with today, go one more than what shows in result
			//manipulate the test data
			Operatory primaryOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory secondaryOp=appSearchData.ListOps[1];//additonal operatory for the provider
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,primaryOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {primaryOp.OperatoryNum };//only primaryOp is in the current "clinic" or "appointment view"
			//create conflicts for the first schedule time. 
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),primaryOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time
			//8:30 since only looking at available provider time, operatory in view doesn't matter here.  
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForProvButProvIsNotInCurrentClinic() {
			//Prov is scheduled in two ops. There are appointment conflicts in both ops, but only one of the ops is in the current clinic.
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provHyg=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvHygienTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today, go one more than what shows in result
			AppointmentSearchData hygAppData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provHyg,11);
			//manipulate the test data
			Operatory provOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory hygOp=hygAppData.ListOps[0];//op for the hygienist
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,provOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			//Make an appointment view that does not have the provider in it at all. Only the hyg op. 
			List<long> listOpsForClinic=new List<long>() {hygOp.OperatoryNum }; //only hyg op in current clinic
			//create conflicts for the first schedule time. 
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsForClinic,new List<long>() { }, beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time
			//No search results should be returned. 
			Assert.AreEqual(0,searchResults.Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForBlockoutByProvTime() {
			//Test to find a single blockout slot. Blockout schedule is over the provider's schedule. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//provider will be 0 in this test - simulating just searching for a blockout, no provider specified. 
			//This provider will be the prov that will be seeing the patient on the appointment date.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(3,1,provNum,11);
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,appSearchData.ListOps[0].OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,"SearchBlockout1").DefNum;
			//create a schedule for both the reg provider and a blockout sched that goes over it.
			DateTime date=appSearchData.ListSchedules[1].SchedDate;
			Schedule blockoutSched=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:13),TestT.SetDateTime(date,hour:15),ScheduleType.Blockout
				,blockoutType:blockoutType,listOpNums:new List<long>() {appSearchData.ListOps[1].OperatoryNum });
			List<long> listOpNums=appSearchData.ListOps.Select(x => x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){blockoutType});
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==blockoutSched.SchedDate 
				&& x.DateTimeAvail.TimeOfDay==blockoutSched.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResultsForBlockoutAndProvider_SearchBlockoutProviderByProvTime() {
			//Test to find a blockout slot that is in a provider's scheduled operatory.
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//Two providers, Prov 0 is the blockout provider and the other will be the prov who's op we want to schedule in.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(3,1,provNum,11);
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,appSearchData.ListOps[0].OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,"SearchBlockout2").DefNum;
			//create a schedule for both the reg provider and a blockout sched that goes over it.
			DateTime date=appSearchData.ListSchedules[1].SchedDate;
			Schedule blockoutSched=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:13),TestT.SetDateTime(date,hour:15),ScheduleType.Blockout
				,blockoutType:blockoutType,listOpNums:new List<long>() {appSearchData.ListOps[1].OperatoryNum });
			List<long> listOpNums=appSearchData.ListOps.Select(x => x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResultsForBlockoutAndProvider(new List<long> {provNum},patApt.AptNum,DateTime.Today,
				DateTime.Today.AddDays(11),listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){blockoutType});
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==blockoutSched.SchedDate 
				&& x.DateTimeAvail.TimeOfDay==blockoutSched.StartTime).Count);
		}

		[TestMethod]
		public void ApptSearch_GetSearchResults_NoResultsWhenProvNotScheduledAndBlockoutIsOnSchedule() {
			//Goal of this test is to verify that we do not suggest times for providers on days that they do not work. 
			//To address a bug where a time was suggested because there was a blockout that returned an opening. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			//make our schedules for the provider
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,1,provNum,daysForSchedule:10,skipWeekends:false);
			//add the appointment we're  trying to find an opening for
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,appSearchData.ListOps[0].OperatoryNum,provNum);
			//date and time filters to find the appointment for
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime(hour:7);//7 am
			//record the schedule that we're going to remove for this test. We want to verify that we will not show time results for this day.
			Schedule scheduleNoProvider=appSearchData.ListSchedules[1];
			DateTime date=scheduleNoProvider.SchedDate;
			ScheduleT.DeleteSchedule(scheduleNoProvider.ScheduleNum);
			appSearchData.ListSchedules.Remove(scheduleNoProvider);//do not schedule prov on the first day.
			//make our blockout that is does not allow scheduling. 
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,"SearchBlockout"+MethodBase.GetCurrentMethod().Name,"NS").DefNum;//Do Not Schedule
			Schedule blockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:8),TestT.SetDateTime(date,hour:11),ScheduleType.Blockout,
				blockoutType:blockoutType,listOpNums:new List<long>() {appSearchData.ListOps[0].OperatoryNum });
			//run the appointment search
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Today.AddDays(15),new List<long> {provNum},
				appSearchData.ListOps.Select(x => x.OperatoryNum).ToList(),new List<long>() { },beforeTime,afterTime);
			//We should only have 8 results. We created 10, -1 for today (we do not return results for day of), -1 for the day we removed. 
			Assert.AreEqual(8,searchResults.Count);
			//Verify that there are no schedules for the day that we removed. 
			Assert.AreEqual(0,searchResults.FindAll(x => x.DateTimeAvail.Date==scheduleNoProvider.SchedDate).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTime_OneOp)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in one operatory for one provider. There are two total operatories in the practice. The provider is the default provider for one operatory and the other operatory is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTime search logic. The first available time slot for tomorrow should be 8:30am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTime_OneOp() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			//long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			//IsDynamic will create schedules that have no default ops assigned.
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:2,numClinics:0,provNum:provNumSearching,daysForSchedule:11,isDynamic:true,skipWeekends:false);
			//assign the second appointment to the other provider.
			appSearchData.ListOps[1].ProvDentist=0;
			Operatories.Update(appSearchData.ListOps[1]);
			Operatories.RefreshCache();
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNumSearching); 
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,opScheduling.OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=appSearchData.ListOps.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching},listOpsInView,new List<long>() { },beforeTime,afterTime);
			//30mins of provider time starting at 8am should leave the first opening at 8:30am same day as preexisting appt.
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==8 
				&& x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTime_TwoOps)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in 2 operatories for 1 provider. There are three total operatories in the practice. The provider is the default provider for two operatories and the third operatory is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTime search logic. The first available time slot for tomorrow should be 8:30am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTime_TwoOps() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			//long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			//IsDynamic will create schedules that have no default ops assigned.
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:3,numClinics:0,provNum:provNumSearching,daysForSchedule:11,isDynamic:true,skipWeekends:false);
			//assign the second appointment to the other provider.
			appSearchData.ListOps[1].ProvDentist=0;
			Operatories.Update(appSearchData.ListOps[1]);
			Operatories.RefreshCache();
			List<Operatory> listOpsScheduling=appSearchData.ListOps.FindAll(x => x.ProvDentist==provNumSearching);
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,listOpsScheduling.First().OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=appSearchData.ListOps.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching},listOpsInView,listClinicNums:new List<long>() { },beforeTime,afterTime);
			//30mins of provider time starting at 8am should leave the first opening at 8:30am.
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==8 
				&& x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTime_OneOp)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in one operatory for one provider. There are two total operatories in the practice. The provider is not the default provider for the operatory either operatory. One operatory is instead assigned to the provider in the provider's schedule, while the other operatory is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTime search logic. The first available time slot for tomorrow should be 8:30am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTime_OneOp() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumSearching,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			AppointmentSearchData appSearchDataOther=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumOther,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			List<Operatory> listOpsScheduling=appSearchData.ListOps.FindAll(x => x.ProvDentist==0);//Operatories won't have default providers	
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,listOpsScheduling.First().OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);			
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=listOpsScheduling.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching,provNumOther},listOpsInView,listClinicNums:new List<long>() { },beforeTime,afterTime);
			//ProviderTimeOperatory first opening should be at 8:30am
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==8
				&& x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTime_TwoOps)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in two operatories (B and C) for one provider A. There are three total operatories in the practice. Provider A is not the default provider for either operatory. Both A and B operatories are assigned to the provider in their schedule while the third operatory, D, is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in operatory A for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTime search logic. The first available time slot for tomorrow should be 8:30am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTime_TwoOps() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:2,numClinics:0,provNum:provNumSearching,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			AppointmentSearchData appSearchDataOther=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumOther,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			List<Operatory> listOpsScheduling=appSearchData.ListOps.FindAll(x => x.ProvDentist==0);//Operatories won't have default providers	
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,listOpsScheduling.First().OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=listOpsScheduling.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching,provNumOther},listOpsInView,listClinicNums:new List<long>() { },beforeTime,afterTime);
			//ProviderTimeOperatory first opening should be at 8:30am
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==8
				&& x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}
		#endregion
		#region ProviderTimeOperatory
		[TestMethod]
		public void Appointments_GetSearchResults_GetBasicSearchResultsWithConflictsForProviderTimeOperatory() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);//just look at the op conflicts,no prov time
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			//manipulate the test data
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNum);//get "one" of the ops for the provider. 
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,opScheduling.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView = new List<long>() {opScheduling.OperatoryNum };
			//make other appointments to see we don't get results during that time
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),opScheduling.OperatoryNum,provNum);
			Patient patConflict2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict2");
			AppointmentT.CreateAppointment(patConflict2.PatNum
				,new DateTime(appSearchData.ListSchedules[2].SchedDate.Year,appSearchData.ListSchedules[2].SchedDate.Month
				,appSearchData.ListSchedules[2].SchedDate.Day,8,20,0),opScheduling.OperatoryNum,provNum);
			Patient patConflict3=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict3");
			AppointmentT.CreateAppointment(patConflict3.PatNum
				,new DateTime(appSearchData.ListSchedules[3].SchedDate.Year,appSearchData.ListSchedules[3].SchedDate.Month
				,appSearchData.ListSchedules[3].SchedDate.Day,8,30,0),opScheduling.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long> { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(10,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==50).Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==9 && x.DateTimeAvail.Minute==00).Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==9 && x.DateTimeAvail.Minute==10).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvWithBlockouts() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long patNum=PatientT.CreatePatient("MOFPWB").PatNum;
			//Create a provider, two operatories, and two provider schedules--one for each operatory. Schedules are for today.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Operatory operatory1=OperatoryT.CreateOperatory("abbr-MOFPWB1","opName--MOFPWB1");
			Operatory operatory2=OperatoryT.CreateOperatory("abbr-MOFPWB2","opName--MOFPWB2");
			Schedule schedule1=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,8,0,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,16,0,0).TimeOfDay
				,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>{operatory1.OperatoryNum});//Op 1 is 8a-4p
			Schedule schedule2=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,10,0,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,14,0,0).TimeOfDay
				,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>{operatory2.OperatoryNum});//Op 2 is 10a-2p
			//Create an "NS" blockout on the first operatory, which will not allow scheduling. 
			long defNumNoSchedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"NS-MOFPWB",BlockoutType.NoSchedule.GetDescription()).DefNum;
			Schedule scheduleNoSchedBlockout=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,8,0,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,12,0,0).TimeOfDay
				,ScheduleType.Blockout,blockoutType:defNumNoSchedBlockout,listOpNums:new List<long>{operatory1.OperatoryNum}); //NS blockout on Op 1 from 8a-12p
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(patNum,DateTime.Now,operatory1.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {operatory1.OperatoryNum,operatory2.OperatoryNum };
			//Search starting yesterday, so that we see today's schedules.
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today.AddDays(-1),DateTime.Today.AddDays(2),new List<long>() {provNum,0}
			,listOpsInView,new List<long> {0},beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==10 && x.DateTimeAvail.Minute==0 
				&& x.DateTimeAvail.Date==DateTime.Today.Date).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForDynamicProvWithBlockouts() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long patNum=PatientT.CreatePatient("MOFPWB").PatNum;
			//Create a provider, two operatories with the provider assigned to them, and two provider schedules. Schedules are for today.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Operatory operatory1=OperatoryT.CreateOperatory("abbr-MOFDPWB1","opName--MOFDPWB1",provDentist:provNum);
			Operatory operatory2=OperatoryT.CreateOperatory("abbr-MOFDPWB2","opName--MOFDPWB2",provDentist:provNum);
			Schedule schedule1=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,8,0,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,12,0,0).TimeOfDay
				,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>{}); //Dynamic, 8a-12p
			Schedule schedule2=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,14,0,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,16,0,0).TimeOfDay
				,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>{}); //Dynamic, 2p-4p
			//Create an "NS" blockout on the first operatory, which will not allow scheduling. 
			long defNumNoSchedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"NS-MOFDPWB",BlockoutType.NoSchedule.GetDescription()).DefNum;
			Schedule scheduleNoSchedBlockout=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,8,0,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,12,0,0).TimeOfDay
				,ScheduleType.Blockout,blockoutType:defNumNoSchedBlockout,listOpNums:new List<long>{operatory1.OperatoryNum}); //NS blockout on Op 1 from 8a-12p
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(patNum,DateTime.Now,operatory1.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {operatory1.OperatoryNum,operatory2.OperatoryNum };
			//Search starting yesterday, so that we see today's schedules.
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today.AddDays(-1),DateTime.Today.AddDays(2),new List<long>() {provNum,0}
			,listOpsInView,new List<long> {0},beforeTime,afterTime,new List<long>(){0});
			//Should be an opening at 8am, since the provider is scheduled for both operatories at the same times, and only one of the two has a blockout.
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0 
				&& x.DateTimeAvail.Date==DateTime.Today.Date).Count);
			//Put another "NS" blockout down, this time on the second operatory from 8:30-10:30.
			Schedule scheduleNoSchedBlockout2=ScheduleT.CreateSchedule(DateTime.Today
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,8,30,0).TimeOfDay
				,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,10,30,0).TimeOfDay
				,ScheduleType.Blockout,blockoutType:defNumNoSchedBlockout,listOpNums:new List<long>{operatory2.OperatoryNum}); //NS blockout on Op 2 from 830a-1030a
			//Try search again.
			searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today.AddDays(-1),DateTime.Today.AddDays(2),new List<long>() {provNum,0}
			,listOpsInView,new List<long> {0},beforeTime,afterTime,new List<long>(){0});
			//Should be an opening at 10:30am, since the provider is scheduled for both operatories at the same times, and both are blocked out in the morning until 1030, and the appointment is too large to fit at 8.
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==10 && x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==DateTime.Today.Date).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvWithOneOpConflict() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);//just look at the op conflicts,no prov time 
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);//starts with today
			//manipulate the test data
			Operatory primaryOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory secondaryOp=appSearchData.ListOps[1];//additonal operatory for the provider
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,primaryOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {primaryOp.OperatoryNum,secondaryOp.OperatoryNum };
			//create conflicts for the first schedule time. 
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),secondaryOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long> { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvWithOpConflicts() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);//just look at the op conflicts,no prov time 
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);//starts with today, go one more than what shows in result
			//manipulate the test data
			Operatory primaryOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory secondaryOp=appSearchData.ListOps[1];//additonal operatory for the provider
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,primaryOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {primaryOp.OperatoryNum,secondaryOp.OperatoryNum };
			//create conflicts for the first schedule time. 
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),primaryOp.OperatoryNum,provNum);
			Patient patConflict2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict2");
			AppointmentT.CreateAppointment(patConflict2.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,30,0),secondaryOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long> { },beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time Operatory
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==50 
				&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_MultipleOpsForProvOnlyOneInClinicWithOpConflictsProvOpTime() {
			//Prov is scheduled in two ops. There are appointment conflicts in both ops, but only one of the ops is in the current appointment view/clinic.
			//Clinics and appointment views can both be reprsented by this test, just depends on the appropriate operatories being passed in.  
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);//starts with today
			//manipulate the test data
			Operatory primaryOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory secondaryOp=appSearchData.ListOps[1];//additonal operatory for the provider
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,primaryOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInClinic=new List<long>() {primaryOp.OperatoryNum };//only one op in View / "Clinic"
			//create conflicts for the first schedule time. 
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month
				,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),primaryOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInClinic,new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time
			//8:50 since even though there is an opening in the second op at 8:30, it is not in view so it should not be taken into consideration.
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==50 
				&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForProvButProvIsNotInCurrentViewOrClinic() {
			//Prov is scheduled in two ops. There are appointment conflicts in both ops, but only one of the ops is in the current appointment view/clinic.
			//Apppointment View and clinics can both be represented in this test by passing in the appropriate operatories.  
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provHyg=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvHygienTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			AppointmentSearchData hygAppData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provHyg,11);
			//manipulate the test data
			Operatory provOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory hygOp=hygAppData.ListOps[0];//op for the hygienist
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,provOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			//Make an appointment view that does not have the provider in it at all. Only the hyg op. 
			List<long> listOpsInView=new List<long>() {hygOp.OperatoryNum };//only hyg op in view
			//create conflicts for the first schedule time. 
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long>() { }, beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time
			//No search results should be returned. 
			Assert.AreEqual(0,searchResults.Count);
		}

		[TestMethod]
		public void Appointments_GetAllForDate_NoResultsForDoNotScheduleBlockouts() {
			//Blockouts need to be assigned to operatories so this should only not return results when using non-dynamic schedules and provider time operatory 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today
			Operatory provOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			//Create a blockout in the op for the provider from 8 am to 9:40 am.
			Def blockoutDef=DefT.CreateDefinition(DefCat.BlockoutTypes,"DoNotSchedule",BlockoutType.NoSchedule.GetDescription());
			ScheduleT.CreateSchedule(appSearchData.ListSchedules[1].SchedDate,TestT.SetDateTime(),TestT.SetDateTime(hour:9,minute:40),ScheduleType.Blockout,
				blockoutType:blockoutDef.DefNum,listOpNums:new List<long>() {provOp.OperatoryNum });
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,provOp.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum }
			,new List<long>() {provOp.OperatoryNum },new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==9 && x.DateTimeAvail.Minute==40 
				&& x.DateTimeAvail.Date==appSearchData.ListSchedules[1].SchedDate).Count);
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForBlockoutByProvTimeOperatory() {
			//Test to find a single blockout slot. Blockout schedule is over the provider's schedule. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//provider will be 0 in this test - simulating just searching for a blockout, no provider specified. 
			//This provider will be the prov that will be seeing the patient on the appointment date.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(3,1,provNum,11);
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,appSearchData.ListOps[0].OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,"SearchBlockout3").DefNum;
			//create a schedule for both the reg provider and a blockout sched that goes over it.
			DateTime date=appSearchData.ListSchedules[1].SchedDate;
			Schedule blockoutSched=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:13),TestT.SetDateTime(date,hour:15),ScheduleType.Blockout
				,blockoutType:blockoutType,listOpNums:new List<long>() {appSearchData.ListOps[1].OperatoryNum });
			List<long> listOpNums=appSearchData.ListOps.Select(x => x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){blockoutType});
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==blockoutSched.SchedDate 
				&& x.DateTimeAvail.TimeOfDay==blockoutSched.StartTime).Count);
		}

		///<summary>Bug prevented any search results from being retured when AppointmentSearchBehavior is set to ProviderTimeOperatory,
		///the operatory has both a provider and a hygienist assigned, and the user is searching for a blockout with no provider specified.</summary>
		[TestMethod]
		public void Appointments_GetSearchResults_SearchForBlockoutByProvTimeOperatoryWhenOpHasProviderAndHygienistAssigned() {
			//Test to find a single blockout slot. Blockout schedule is over the provider's schedule. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//provider will be 0 in this test - simulating just searching for a blockout, no provider specified. 
			//This provider will be the prov that will be seeing the patient on the appointment date.
			long provNumProvider=ProviderT.CreateProvider("Provider");
			long provNumHygienist=ProviderT.CreateProvider("Hygienist");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,1,provNumProvider,11,hygNum:provNumHygienist);
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,appSearchData.ListOps[0].OperatoryNum,provNumProvider);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,"SearchBlockout").DefNum;
			//create a schedule for both the reg provider and a blockout sched that goes over it.
			DateTime date=appSearchData.ListSchedules[1].SchedDate;
			Schedule blockoutSched=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:13),TestT.SetDateTime(date,hour:15),ScheduleType.Blockout
				,blockoutType:blockoutType,listOpNums:new List<long>() {appSearchData.ListOps[0].OperatoryNum });
			List<long> listOpNums=appSearchData.ListOps.Select(x => x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){blockoutType});
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==blockoutSched.SchedDate
				&& x.DateTimeAvail.TimeOfDay==blockoutSched.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResultsForBlockoutAndProvider_SearchBlockoutProviderByProvTimeOperatory() {
			//Test to find a blockout slot that is in a provider's scheduled operatory.
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Two providers, Prov 0 is the blockout provider and the other will be the prov who's op we want to schedule in.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(3,1,provNum,11);
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,appSearchData.ListOps[0].OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,"SearchBlockout4").DefNum;
			//create a schedule for both the reg provider and a blockout sched that goes over it.
			DateTime date=appSearchData.ListSchedules[1].SchedDate;
			Schedule blockoutSched=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:13),TestT.SetDateTime(date,hour:15),ScheduleType.Blockout
				,blockoutType:blockoutType,listOpNums:new List<long>() {appSearchData.ListOps[1].OperatoryNum });
			List<long> listOpNums=appSearchData.ListOps.Select(x => x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResultsForBlockoutAndProvider(new List<long> {0,provNum},patApt.AptNum,DateTime.Today,
				DateTime.Today.AddDays(11),listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){blockoutType});
			Assert.AreEqual(1,searchResults.Count);//getting no data back for search results. 
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==blockoutSched.SchedDate 
				&& x.DateTimeAvail.TimeOfDay==blockoutSched.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchResultsForBlockoutsOnlyForDifferentOperatories() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Test for case when a no schedule blockout exists on the same day, but different operatory and different time than searching blockout.
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			AppointmentSearchData dataForProvA=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumA,11);
			AppointmentSearchData dataForProvB=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumB,11);
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Appointment patApt=AppointmentT.CreateAppointment(dataForProvA.Patient.PatNum,DateTime.Now,dataForProvA.ListOps[0].OperatoryNum,provNumA
				,pattern:"XXXXXX");
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			DateTime date=dataForProvA.ListSchedules[1].SchedDate;
			//create the blockout we want to find
			long desiredBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			Schedule schedForDesiredBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10)
				,ScheduleType.Blockout,blockoutType:desiredBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//create the blockout for the other provider, that is not supposed to allow scheduling. 
			long noSchedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"NS",BlockoutType.NoSchedule.GetDescription()).DefNum;
			Schedule schedForNsBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:8),TestT.SetDateTime(date,hour:9),ScheduleType.Blockout,
				blockoutType:noSchedBlockout,listOpNums:dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			//run the search
			List<long> listOpNums=dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList();
			listOpNums.AddRange(dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){desiredBlockout});
			//compare results. We should only have one valid result returned for the blockout we created. 
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedForDesiredBlockout.SchedDate
				&& x.DateTimeAvail.TimeOfDay==schedForDesiredBlockout.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchResultsForBlockoutsOnlyForDifferentOperatoriesAtTheSameTime() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Test for case when a no schedule blockout exists on the same day, but different operatory and same time as searching blockout.
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			AppointmentSearchData dataForProvA=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumA,11);
			AppointmentSearchData dataForProvB=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumB,11);
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Appointment patApt=AppointmentT.CreateAppointment(dataForProvA.Patient.PatNum,DateTime.Now,dataForProvA.ListOps[0].OperatoryNum,provNumA
				,pattern:"XXXXXX");
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			DateTime date=dataForProvA.ListSchedules[1].SchedDate;
			//create the blockout we want to find
			long desiredBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			Schedule schedForDesiredBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10)
				,ScheduleType.Blockout,blockoutType:desiredBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//create the blockout for the other provider, that is not supposed to allow scheduling. 
			long noSchedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"NS",BlockoutType.NoSchedule.GetDescription()).DefNum;
			Schedule schedForNsBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10),ScheduleType.Blockout,
				blockoutType:noSchedBlockout,listOpNums:dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			//run the search
			List<long> listOpNums=dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList();
			listOpNums.AddRange(dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){desiredBlockout});
			//compare results. We should have one valid result returned for the blockout we created. 
			//Issue to safeguard against here was that nothing could potentially show up. 
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedForDesiredBlockout.SchedDate
				&& x.DateTimeAvail.TimeOfDay==schedForDesiredBlockout.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchResultsForBlockoutsOnlyForSameProvider() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Test for case when a no schedule blockout exists on the same day, same operatory and different time than searching blockout.
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			AppointmentSearchData dataForProvA=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumA,11);
			AppointmentSearchData dataForProvB=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumB,11);
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Appointment patApt=AppointmentT.CreateAppointment(dataForProvA.Patient.PatNum,DateTime.Now,dataForProvA.ListOps[0].OperatoryNum,provNumA
				,pattern:"XXXXXX");
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			DateTime date=dataForProvA.ListSchedules[1].SchedDate;
			//create the blockout we want to find
			long desiredBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			Schedule schedForDesiredBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:10),TestT.SetDateTime(date,hour:11)
				,ScheduleType.Blockout,blockoutType:desiredBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//create the blockout for the same provider, that is not supposed to allow scheduling. 
			long noSchedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"NS",BlockoutType.NoSchedule.GetDescription()).DefNum;
			Schedule schedForNsBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:8),TestT.SetDateTime(date,hour:9),ScheduleType.Blockout,
				blockoutType:noSchedBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//For some reason, another NS blockout needs to be in another operatory to create the bug.
			ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10),ScheduleType.Blockout,
				blockoutType:noSchedBlockout,listOpNums:dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			//run the search
			List<long> listOpNums=dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList();
			listOpNums.AddRange(dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){desiredBlockout});
			//compare results. We should only have one valid result returned for the blockout we created. 
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedForDesiredBlockout.SchedDate
				&& x.DateTimeAvail.TimeOfDay==schedForDesiredBlockout.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_DoNotIncludeUndesiredBlockoutsInSearchResults() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Test with two regular blockouts to make sure correct blockout gets returned. 
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			AppointmentSearchData dataForProvA=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumA,11);
			AppointmentSearchData dataForProvB=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumB,11);
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Appointment patApt=AppointmentT.CreateAppointment(dataForProvA.Patient.PatNum,DateTime.Now,dataForProvA.ListOps[0].OperatoryNum,provNumA
				,pattern:"XXXXXX");
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			DateTime date=dataForProvA.ListSchedules[1].SchedDate;
			//create the blockout we want to find
			long desiredBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			Schedule schedForDesiredBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:10),TestT.SetDateTime(date,hour:11)
				,ScheduleType.Blockout,blockoutType:desiredBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//create the blockout for the same provider, that is not supposed to allow scheduling. 
			long noSchedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"NS",BlockoutType.NoSchedule.GetDescription()).DefNum;
			Schedule schedForNsBlockout=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:8),TestT.SetDateTime(date,hour:9),ScheduleType.Blockout,
				blockoutType:noSchedBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//For some reason, another NS blockout needs to be in another operatory to create the bug.
			ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10),ScheduleType.Blockout,
				blockoutType:noSchedBlockout,listOpNums:dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			//create yet another blockout that is ok to schedule, but a different type so we can verify correct blockout is getting returned.
			long otherBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10)
				,ScheduleType.Blockout,blockoutType:otherBlockout,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//run the search
			List<long> listOpNums=dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList();
			listOpNums.AddRange(dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {0},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){desiredBlockout});
			//compare results. We should only have one valid result returned for the blockout we created. 
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedForDesiredBlockout.SchedDate
				&& x.DateTimeAvail.TimeOfDay==schedForDesiredBlockout.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForSchedulesAndOperatoriesForSpecificDays() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,5);
			//Test when a provider is scheduled in multilple ops for different days that ops are only suggested for where the op the provider is in,
			//for the current day. Ex (Day1 in Op1, Day2 in Op 2 so when searching for opening for Day1 only Op1 is searched for). 
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			Operatory op1=OperatoryT.CreateOperatory("op1",suffix+"1");//provs not directly assinged to ops, they get assigned by schedule.
			Operatory op2=OperatoryT.CreateOperatory("op2",suffix+"2");//provs not directly assinged to ops, they get assigned by schedule.
			Schedule schedDay1ProvA=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumA,listOpNums:new List<long>() {op1.OperatoryNum });
			Schedule schedDay1ProvB=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumB,listOpNums:new List<long>() {op2.OperatoryNum });
			//create schedule for the next day, in a different op than today.
			Schedule schedDay2ProvA=ScheduleT.CreateSchedule(DateTime.Today.AddDays(2)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumA,listOpNums:new List<long>() {op2.OperatoryNum });
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Patient pat=new Patient();
			Appointment patApt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(-1),op1.OperatoryNum,provNumA,pattern:"/XX/");
			//create conflicting appointments for provA for the first day
			Patient conflictingPat=new Patient();
			AppointmentT.CreateAppointment(conflictingPat.PatNum,TestT.SetDateT(schedDay1ProvA.SchedDate),op1.OperatoryNum,provNumA,pattern:"/XX/");
			AppointmentT.CreateAppointment(conflictingPat.PatNum,TestT.SetDateT(schedDay1ProvA.SchedDate,minute:20),op1.OperatoryNum,provNumA,pattern:"/XX///");
			TimeSpan beforeTime=TestT.SetDateTime(hour:9);
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			//run the search
			List<long> listOpNums=new List<long>() {op1.OperatoryNum,op2.OperatoryNum };
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {provNumA},
				listOpNums,new List<long>() { },beforeTime,afterTime);
			//compare results. We should not have any results for the first day since the provider is booked in the time frame specified.
			Assert.AreEqual(0,searchResults.FindAll(x => x.DateTimeAvail.Date==schedDay1ProvA.SchedDate).Count);
			//Nothing is booked for our prov on the second day, in op2 yet so it should show as being first available. 
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedDay2ProvA.SchedDate
				&& x.DateTimeAvail.TimeOfDay==schedDay2ProvA.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForOpeningsWhenProvidersShareAnOperatory() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,5);
			//Test when multiple providers are scheduled in the same operatory (ex provA is there from 8-9 and provB from 9-10).
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			//important because of the OR when we get appointments. We are trying to get blockouts, but we will get all appointments that do not have a hyg
			//In this case, appointments did have a primary and secondary. Since we were only getting appointments with provs we were searching for
			//we were not getting other appointments in the operatory. So when several providers shared an op it didn't know about the other appointments.
			long provHyg=ProviderT.CreateProvider(suffix+"Hyg",isSecondary:true);
			Operatory op1=OperatoryT.CreateOperatory("op1",suffix+"1");//provs not directly assinged to ops, they get assigned by schedule. (non-dynamic)
			Operatory op2=OperatoryT.CreateOperatory("op2",suffix+"2");//provs not directly assinged to ops, they get assigned by schedule.
			//create schedules for first operatory
			Schedule schedProvAOp1=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumA,listOpNums:new List<long>() {op1.OperatoryNum });
			ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow from 9 am to 11 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,11,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumB,listOpNums:new List<long>() {op1.OperatoryNum });
			//create schedules for the second operatory
			ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumB,listOpNums:new List<long>() {op2.OperatoryNum });
			ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow from 9 to 11
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,11,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumA,listOpNums:new List<long>() {op2.OperatoryNum });
			//create an available opening for the next day that we should find.
			Schedule schedProvADay2=ScheduleT.CreateSchedule(DateTime.Today.AddDays(2)//day after tomorrow from 8 am to 10 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(2).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(2).Day,10,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumA,listOpNums:new List<long>() {op1.OperatoryNum });
			//create the conflicting appointments that are already scheduled.
			Patient patConflict1=new Patient();
			Patient patConflict2=new Patient();
			DateTime schedDate=schedProvAOp1.SchedDate.Date;//all are for the same date, different times.
			//appointment from 8-8:50 for provA in op1
			AppointmentT.CreateAppointment(patConflict1.PatNum,TestT.SetDateT(schedDate,8),op1.OperatoryNum,provNumA,provHyg,pattern:"//XXXX////");
			//appointment from 8-8:50 for provB in op2
			AppointmentT.CreateAppointment(patConflict2.PatNum,TestT.SetDateT(schedDate,8),op2.OperatoryNum,provNumB,provHyg,pattern:"//XXXX////");
			//appointment from 9:10-11 for prov B in op 1
			AppointmentT.CreateAppointment(patConflict1.PatNum,TestT.SetDateT(schedDate,9,10),op1.OperatoryNum,provNumB,provHyg,pattern:"//XX////XX//XXXXXXXX//");
			//appointment from 9:20-11 for prov A in op 2
			AppointmentT.CreateAppointment(patConflict2.PatNum,TestT.SetDateT(schedDate,9,20),op2.OperatoryNum,provNumA,provHyg,pattern:"//XXXX//XX//XXXX////");
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Patient pat=new Patient();
			Appointment patApt=AppointmentT.CreateAppointment(pat.PatNum,TestT.SetDateT(DateTime.Today.AddDays(-1)),op1.OperatoryNum,provNumA,provHyg
				,pattern:"//XXXX//");
			//create conflicting appointments for provA for first half of the day. 
			TimeSpan beforeTime=TestT.SetDateTime(hour:11);
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			//run the search
			List<long> listOpNums=new List<long>() { op1.OperatoryNum,op2.OperatoryNum };
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(3),new List<long>() {provNumA},
				listOpNums,new List<long>() { },beforeTime,afterTime);
			//compare results. We should not have any results for the first day since the provider is booked in the time frame specified.
			Assert.AreEqual(0,searchResults.FindAll(x => x.DateTimeAvail.Date==schedDate).Count);
			//Nothing is booked for our prov on the second day, in op2 yet so it should show as being first available.
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedProvADay2.SchedDate
				&& x.DateTimeAvail.TimeOfDay==schedProvADay2.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResultsForBlockoutAndProvider_FindResultsForBlockoutInSpecificProviderOperatory() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			//Test scenario where ProvA and ProvB are scheduled to work. ProvA has blockout1 in their op at 9am and ProvB has the same blockout at 8am.
			//When searching by provider AND blockout for ProvA and blockout1 we should get results for 9am in ProvA's operatory.
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			long provNumB=ProviderT.CreateProvider(suffix+"B");
			AppointmentSearchData dataForProvA=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumA,11);
			AppointmentSearchData dataForProvB=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNumB,11);
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Appointment patApt=AppointmentT.CreateAppointment(dataForProvA.Patient.PatNum,DateTime.Now,dataForProvA.ListOps[0].OperatoryNum,provNumA
				,pattern:"XXXXXX");
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);//6 pm
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			DateTime date=dataForProvA.ListSchedules[1].SchedDate;
			//create the blockout type we want to find
			long blockoutType=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			//create blockout in ProviderB's op for 8 am
			ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:8),TestT.SetDateTime(date,hour:9)
				,ScheduleType.Blockout,blockoutType:blockoutType,listOpNums:dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			//create blockout in ProviderA's op for 9 am
			Schedule blockoutForProvA=ScheduleT.CreateSchedule(date,TestT.SetDateTime(date,hour:9),TestT.SetDateTime(date,hour:10)
				,ScheduleType.Blockout,blockoutType:blockoutType,listOpNums:dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList());
			//run the search
			List<long> listOpNums=dataForProvA.ListOps.Select(x => x.OperatoryNum).ToList();
			listOpNums.AddRange(dataForProvB.ListOps.Select(x => x.OperatoryNum).ToList());
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResultsForBlockoutAndProvider(new List<long> {provNumA},patApt.AptNum,DateTime.Today,
				DateTime.Now.AddDays(11),listOpNums,new List<long> { },beforeTime,afterTime,new List<long>(){blockoutType});
			//compare results. We should only have one valid result returned for the blockout we created. 
			Assert.AreEqual(1,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==blockoutForProvA.SchedDate
				&& x.DateTimeAvail.TimeOfDay==blockoutForProvA.StartTime).Count);
		}

		[TestMethod]
		public void Appointments_GetSearchResults_DifferentBlockoutsSameOp() {
			//Test is specifically for provider time operatory as there is no way to determine operatory when only seraching provider time. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,5);
			//Test when a provider is scheduled in multilple ops for different days that ops are only suggested for where the op the provider is in,
			//for the current day. Ex (Day1 in Op1, Day2 in Op 2 so when searching for opening for Day1 only Op1 is searched for). 
			//create providers and schedules
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNumA=ProviderT.CreateProvider(suffix+"A");
			Operatory op1=OperatoryT.CreateOperatory("op1",suffix+"1");//provs not directly assinged to ops, they get assigned by schedule.
			Schedule schedProvA=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow (default first search day) from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,30,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNumA,listOpNums:new List<long>() {op1.OperatoryNum });
			//create one blockout of a specific type and put in providers op. Blockout length should not be able to fit the appointment.
			long blockoutType1=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			ScheduleT.CreateSchedule(schedProvA.SchedDate,TestT.SetDateTime(schedProvA.SchedDate,hour:8),TestT.SetDateTime(schedProvA.SchedDate,hour:8,minute:30)
				,ScheduleType.Blockout,blockoutType:blockoutType1,listOpNums:new List<long>() {op1.OperatoryNum });
			//create one blockout of a different specific type and put in providers op. Blockout length should not be able to fit the appointment.
			long blockoutType2=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix).DefNum;
			ScheduleT.CreateSchedule(schedProvA.SchedDate,TestT.SetDateTime(schedProvA.SchedDate,hour:8,minute:30),TestT.SetDateTime(schedProvA.SchedDate,hour:9)
				,ScheduleType.Blockout,blockoutType:blockoutType2,listOpNums:new List<long>() {op1.OperatoryNum });
			//create the appointment and search criteria for the appointment trying to be scheduled.
			Patient pat=new Patient();
			Appointment patApt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(-1),op1.OperatoryNum,provNumA,pattern:"//XXXX//");
			TimeSpan beforeTime=TestT.SetDateTime(hour:9);
			TimeSpan afterTime=TestT.SetDateTime();//8 am
			//run the search
			List<long> listOpNums=new List<long>() {op1.OperatoryNum};
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(11),new List<long>() {provNumA},
				listOpNums,new List<long>() { },beforeTime,afterTime,new List<long>(){blockoutType1});
			//compare results. We should not have any results for the first day since the provider is booked in the time frame specified.
			Assert.AreEqual(0,searchResults.FindAll(x => x.DateTimeAvail.Date==schedProvA.SchedDate).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_OneOp)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in one operatory for one provider. There are two total operatories in the practice. The provider is the default provider for one operatory and the other operatory is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTimeOperatory search logic. The first available time slot for tomorrow should be 9am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_OneOp() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			//long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			//IsDynamic will create schedules that have no default ops assigned.
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:2,numClinics:0,provNum:provNumSearching,daysForSchedule:11,isDynamic:true,skipWeekends:false);
			//assign the second op to the other provider.
			appSearchData.ListOps[1].ProvDentist=0;
			Operatories.Update(appSearchData.ListOps[1]);
			Operatories.RefreshCache();
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNumSearching);
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,opScheduling.OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=appSearchData.ListOps.Select(x=>x.OperatoryNum).ToList();//new List<long>() {provNumSearching}
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching},listOpsInView,new List<long>() { },beforeTime,afterTime);
			//30mins of provider time starting at 8am should leave the first opening at 8:30am same day as preexisting appt.
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==9 
				&& x.DateTimeAvail.Minute==0 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_OneOp_Only)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in 1 operatory for 1 provider. The operatory is the only operatory in the practice. The provider is the default provider for the operatory. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTimeOperatory search logic. The first available time slot for tomorrow should be 9am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_OneOp_Only() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			//IsDynamic will create schedules that have no default ops assigned.
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumSearching,daysForSchedule:11,isDynamic:true,skipWeekends:false);
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNumSearching); 
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,opScheduling.OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=appSearchData.ListOps.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching},listOpsInView,new List<long>() { },beforeTime,afterTime);
			//30mins of provider time starting at 8am should leave the first opening at 8:30am same day as preexisting appt.
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==9 
				&& x.DateTimeAvail.Minute==0 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_TwoOps)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in 2 operatories for 1 provider. There are three total operatories in the practice. The provider is the default provider for two operatories and the third operatory is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTimeOperatory search logic. The first available time slot for tomorrow should be 8:30am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_DefaultProvNoSched_ProvTimeOp_TwoOps() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			//long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			//IsDynamic will create schedules that have no default ops assigned.
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:3,numClinics:0,provNum:provNumSearching,daysForSchedule:11,isDynamic:true,skipWeekends:false);
			//assign the second appointment to the other provider.
			appSearchData.ListOps[1].ProvDentist=0;
			Operatories.Update(appSearchData.ListOps[1]);
			Operatories.RefreshCache();
			List<Operatory> listOpsScheduling=appSearchData.ListOps.FindAll(x => x.ProvDentist==provNumSearching);
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,listOpsScheduling.First().OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=appSearchData.ListOps.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching},listOpsInView,listClinicNums:new List<long>() { },beforeTime,afterTime);
			//30mins of provider time starting at 8am should leave the first opening at 8:30am.
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==8 
				&& x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTimeOp_OneOp)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in one operatory for one provider. There are two total operatories in the practice. The provider is not the default provider for the operatory either operatory. One operatory is instead assigned to the provider in the provider's schedule, while the other operatory is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in the provider's operatory for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTimeOperatory search logic. The first available time slot for tomorrow should be 9am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTimeOp_OneOp() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumSearching,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			AppointmentSearchData appSearchDataOther=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumOther,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			List<Operatory> listOpsScheduling=appSearchData.ListOps.FindAll(x => x.ProvDentist==0);//Operatories won't have default providers	
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,listOpsScheduling.First().OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=listOpsScheduling.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching,provNumOther},listOpsInView,listClinicNums:new List<long>() { },beforeTime,afterTime);
			//ProviderTimeOperatory first opening should be at 8:30am
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==9
				&& x.DateTimeAvail.Minute==0 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTimeOp_TwoOps)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Searches for available time slots in two operatories (B and C) for one provider A. There are three total operatories in the practice. Provider A is not the default provider for either operatory. Both A and B operatories are assigned to the provider in their schedule while the third operatory, D, is not assigned to anyone. The provider is scheduled 8am-4pm for the next 10 days. There is an appointment in operatory A for tomorrow from 8am-9am with provider time 8am-8:30am. The preference for 'Appointment Search Behavior' is set to ProviderTimeOperatory search logic. The first available time slot for tomorrow should be 8:30am. Since there are no preexisting appointments on subsequent days, the remaining 9 openings should all be 8am on their respective days.")]
		public void ApptSearch_GetSearchResults_SchedProvNoDefault_ProvTimeOp_TwoOps() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			long provNumSearching=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provNumOther=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest2");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(numOps:2,numClinics:0,provNum:provNumSearching,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			AppointmentSearchData appSearchDataOther=AppointmentT.CreateScheduleAndOpsForProv(numOps:1,numClinics:0,provNum:provNumOther,daysForSchedule:11,doAssignProviderOps:false,skipWeekends:false);
			List<Operatory> listOpsScheduling=appSearchData.ListOps.FindAll(x => x.ProvDentist==0);//Operatories won't have default providers	
			DateTime dateTPreExistingAppointment=DateTime.Today.AddDays(1).AddHours(8);
			Appointment appointmentPreExsiting=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,dateTPreExistingAppointment,listOpsScheduling.First().OperatoryNum,provNumSearching,pattern:"XXXXXX//////");
			Appointment appointmentOnPinBoard=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.MinValue,0,provNumSearching,pattern:"XXXXXX//////",aptStatus:ApptStatus.UnschedList);			
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=listOpsScheduling.Select(x=>x.OperatoryNum).ToList();
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(appointmentOnPinBoard.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNumSearching,provNumOther},listOpsInView,listClinicNums:new List<long>() { },beforeTime,afterTime);
			//ProviderTimeOperatory first opening should be at 8:30am
			Assert.AreEqual(1,searchResults.Count(x => x.DateTimeAvail.Hour==8
				&& x.DateTimeAvail.Minute==30 
				&& x.DateTimeAvail.Date==dateTPreExistingAppointment.Date));
			//The other 9 appts should be at 8am since there are no other appointments scheduled on those days.
			Assert.AreEqual(9,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==0).Count);
		}
		#endregion
		
		#endregion

		#region Dynamic scheduling tests
		//test for dynamic scheduling
		[TestMethod]
		public void Appointments_GetSearchResults_DynamicGetSearchTimesWithConflictsProvTimeOp() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);//just look at the op conflicts,no prov time
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11,isDynamic:true);
			//manaully make operatories and assign them to providers
			//manipulate the test data
			Operatory opScheduling=appSearchData.ListOps.FirstOrDefault(x => x.ProvDentist==provNum);//get "one" of the ops for the provider. 
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,opScheduling.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {opScheduling.OperatoryNum };
			//make other appointments to see we don't get results during that time
			Patient patConflict1=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict1");
			AppointmentT.CreateAppointment(patConflict1.PatNum
				,new DateTime(appSearchData.ListSchedules[1].SchedDate.Year,appSearchData.ListSchedules[1].SchedDate.Month,appSearchData.ListSchedules[1].SchedDate.Day,8,10,0),opScheduling.OperatoryNum,provNum);
			Patient patConflict2=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict2");
			AppointmentT.CreateAppointment(patConflict2.PatNum
				,new DateTime(appSearchData.ListSchedules[2].SchedDate.Year,appSearchData.ListSchedules[2].SchedDate.Month,appSearchData.ListSchedules[2].SchedDate.Day,8,20,0),opScheduling.OperatoryNum,provNum);
			Patient patConflict3=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name+"Conflict3");
			AppointmentT.CreateAppointment(patConflict3.PatNum
				,new DateTime(appSearchData.ListSchedules[3].SchedDate.Year,appSearchData.ListSchedules[3].SchedDate.Month,appSearchData.ListSchedules[3].SchedDate.Day,8,30,0),opScheduling.OperatoryNum,provNum);
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
				,listOpsInView,new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(10,searchResults.Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==8 && x.DateTimeAvail.Minute==50).Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==9 && x.DateTimeAvail.Minute==00).Count);
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Hour==9 && x.DateTimeAvail.Minute==10).Count);
		}
		
		//test for dynamic scheduling by provider operatory time
		[TestMethod]
		public void Appointments_GetSearchResults_DynamicBasics() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTimeOperatory);
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Operatory op1=OperatoryT.CreateOperatory(provDentist:provNum);
			Schedule schedProvDynamic=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>() { });
			Schedule schedProv=ScheduleT.CreateSchedule(DateTime.Today.AddDays(2)//day after tomorrow from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(2).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(2).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>() {op1.OperatoryNum });
			Appointment patApt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,op1.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {op1.OperatoryNum }; 
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(5)
				,new List<long>() {provNum}
				,listOpsInView,new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedProvDynamic.SchedDate.Date).Count);
		}

		//basic test for dynamic scheduling by provider time
		[TestMethod]
		public void Appointments_GetSearchResults_DynamicBasicsProviderTime() {
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			Patient pat=PatientT.CreatePatient();
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Operatory op1=OperatoryT.CreateOperatory(provDentist:provNum);
			Schedule schedProvDynamic=ScheduleT.CreateSchedule(DateTime.Today.AddDays(1)//tomorrow from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(1).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>() { });
			Schedule schedProv=ScheduleT.CreateSchedule(DateTime.Today.AddDays(2)//day after tomorrow from 8 am to 9 am
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(2).Day,8,0,0).TimeOfDay
						,new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(2).Day,9,0,0).TimeOfDay
						,ScheduleType.Provider,provNum:provNum,listOpNums:new List<long>() {op1.OperatoryNum });
			Appointment patApt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,op1.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			List<long> listOpsInView=new List<long>() {op1.OperatoryNum }; 
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddDays(5)
				,new List<long>() {provNum}
				,listOpsInView,new List<long>() { },beforeTime,afterTime,new List<long>(){0});
			Assert.AreEqual(1,searchResults.FindAll(x => x.DateTimeAvail.Date==schedProvDynamic.SchedDate.Date).Count);
		}
		#endregion

		#region Tests for Appointment Search by Appointment View. 
		//These tests are specifically written to mimic and test appointment views. They work because they are simply dependant upon
		//the operatories that are passed int (i.e. the ones for the current view as opposed to all for the clinic).
		//Leaving because they currently work and we plan on possibly adding the functionality in the future. Currently we do this just for Headquarters.  

		[TestMethod]
		public void Appointments_GetSearchResults_SearchForProvButProvIsNotInCurrentViewProvTime() {
			//Prov is scheduled in two ops. There are appointment conflicts in both ops, but only one of the ops is in the current appointment view. 
			PrefT.UpdateInt(PrefName.AppointmentSearchBehavior,(int)SearchBehaviorCriteria.ProviderTime);
			//create provider and schedule for provider that the search will be for
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			long provHyg=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvHygienTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provNum,11);//starts with today, go one more than what shows in result
			AppointmentSearchData hygAppData=AppointmentT.CreateScheduleAndOpsForProv(1,0,provHyg,11);
			//manipulate the test data
			Operatory provOp=appSearchData.ListOps[0];//get one of the ops for the provider. 
			Operatory hygOp=hygAppData.ListOps[0];//op for the hygienist
			//Create appointment that the search will be to find a spot for.
			Appointment patApt=AppointmentT.CreateAppointment(appSearchData.Patient.PatNum,DateTime.Now,provOp.OperatoryNum,provNum);
			TimeSpan beforeTime=TestT.SetDateTime(hour:18);
			TimeSpan afterTime=TestT.SetDateTime();//8 AM
			//Make an appointment view that does not have the provider in it at all. Only the hyg op. 
			List<long> listOpsInView=new List<long>() {hygOp.OperatoryNum }; //only hyg op in appointment view
			//create conflicts for the first schedule time. 
			List<ScheduleOpening> searchResults=ApptSearch.GetSearchResults(patApt.AptNum,DateTime.Today,DateTime.Now.AddYears(2),new List<long>() {provNum}
			,listOpsInView,new List<long>() { }, beforeTime,afterTime,new List<long>(){0});
			//Assert that appointment time is correct for provider time
			//No search results should be returned. 
			Assert.AreEqual(0,searchResults.Count);
		}
		#endregion
		#endregion

		[TestMethod]
		public void Appointments_CopyStructure() {
			long patNum=1;
			DateTime apptDateTime=new DateTime();
			long opNum=7;
			long provNum=2;
			long appointmentTypeNum=63;//Arbitrary number, not necessarily a real appointment type
			string pattern="//XXXX//";
			string note="test note";
			ApptStatus apptStatus=ApptStatus.Complete;
			Appointment apptOrig=AppointmentT.CreateAppointment(patNum,apptDateTime,opNum,provNum,pattern:pattern,aptStatus:apptStatus,aptNote:note,
				appointmentTypeNum:appointmentTypeNum);
			long apptLength=apptOrig.Length;
			Appointment apptStructureCopy=Appointments.CopyStructure(apptOrig);
			Assert.AreEqual(patNum,apptStructureCopy.PatNum);
			Assert.AreEqual(provNum,apptStructureCopy.ProvNum);
			Assert.AreEqual(pattern,apptStructureCopy.Pattern);
			Assert.AreEqual(apptLength,apptStructureCopy.Length);
			Assert.AreEqual(note,apptStructureCopy.Note);
			Assert.AreEqual(ApptStatus.UnschedList,apptStructureCopy.AptStatus);
			Assert.AreEqual(appointmentTypeNum,apptStructureCopy.AppointmentTypeNum);
		}

		///<summary>Testing appointment time pattern logic.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Appointments_GetApptTimePatternFromProcPatterns_PatternLogic)]
		[Documentation.Description("Testing appointment time pattern logic.")]

		public void Appointments_GetApptTimePatternFromProcPatterns_PatternLogic() {
			string suffix="79";
			string pattern01="X";
			string pattern02="X/";
			string pattern03="/X";
			string pattern04="/X/";
			string pattern05="/X//";
			string pattern06="/XX/";
			string pattern07="/XXX/";
			string pattern08="//XXX";
			string pattern09="//XXX/";
			string pattern10="/XXXX////";
			string pattern11="/XX/////XXX//";
			/*  X + X = XX  */
			string result1=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {	pattern01	/*  X  */
														, pattern01	/*  X  */
				});
			/*  /X + X = /XX  */
			string result2=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {	pattern03	/*  /X  */
														, pattern01	/*  X  */
				});
			/*  //XXX + X/ = //XXXX/  */
			string result3=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {	pattern08	/*  //XXX  */
														, pattern02	/*  X/  */
				});
			/*  X + X + X/ = XXX/  */
			string result4=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {	pattern01	/*  X  */
														, pattern01	/*  X  */
														, pattern02	/*  X/  */
				});
			/*  /X/ + /X/ + /XX/ + /XX/ = /XXXXXX/  */
			string result5=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {  pattern04	/*  /X/  */
														, pattern04	/*  /X/  */
														, pattern06	/*  /XX/  */
														, pattern06	/*  /XX/  */
				});
			/*  /XXX/ + /X/ + /XXXX//// = /XXXXXXXX////  */
			string result6=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {  pattern07	/*  /XXX/  */
														, pattern04	/*  /X/  */
														, pattern10	/*  /XXXX////  */
				});
			/*  //XXX/ + X/ = //XXXX/  */
			string result7=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {  pattern09	/*  //XXX/  */
														, pattern02	/*  X/  */
				});
			/*  //XXX/ + /X// = //XXXX//  */
			string result8=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {  pattern09	/*  //XXX/  */
														, pattern05	/*  /X//  */
				});
			/*  /X/ + /XX/////XXX// = /XXX/////XXX//  */
			string result9=Appointments.GetApptTimePatternFromProcPatterns(
				new List<string>() {  pattern04	/*  /X/  */
														, pattern11	/*  /XX/////XXX//  */
				});
			string error="";
			if(result1!="XX") {
				error+="\r\n	Time pattern result 1 not converted as expected.  Got: "+result1+"  Expected: XX";
			}
			if(result2!="/XX") {
				error+="\r\n	Time pattern result 2 not converted as expected.  Got: "+result2+"  Expected: /XX";
			}
			if(result3!="//XXXX/") {
				error+="\r\n	Time pattern result 3 not converted as expected.  Got: "+result3+"  Expected: //XXXX/";
			}
			if(result4!="XXX/") {
				error+="\r\n	Time pattern result 4 not converted as expected.  Got: "+result4+"  Expected: XXX/";
			}
			if(result5!="/XXXXXX/") {
				error+="\r\n	Time pattern result 5 not converted as expected.  Got: "+result5+"  Expected: /XXXXXX/";
			}
			if(result6!="/XXXXXXXX////") {
				error+="\r\n	Time pattern result 6 not converted as expected.  Got: "+result6+"  Expected: /XXXXXXXX////";
			}
			if(result7!="//XXXX/") {
				error+="\r\n	Time pattern result 7 not converted as expected.  Got: "+result7+"  Expected: //XXXX/";
			}
			if(result8!="//XXXX//") {
				error+="\r\n	Time pattern result 8 not converted as expected.  Got: "+result8+"  Expected: //XXXX//";
			}
			if(result9!="/XXX/////XXX//") {
				error+="\r\n	Time pattern result 8 not converted as expected.  Got: "+result9+"  Expected: /XXX/////XXX//";
			}
			Assert.IsTrue(string.IsNullOrEmpty(error));
		}

		[TestMethod]
		public void Appointments_IsRecallAppointment_DatesDoNotMatch() {
			long provNum=2;
			long opNum=7;
			string note="test note";
			string strApptProcCode="D0150";
			string strRecallProcCode=strApptProcCode;
			Interval intervalRecall=new Interval(0,0,6,0);//6 Month Interval.
			DateTime dateTimeAppt=DateTime.Today;
			DateTime dateTimeRecall=DateTime.Today.AddMonths(intervalRecall.Months);
			ApptStatus apptStatus=ApptStatus.Scheduled;
			ProcStat procStatus=ProcStat.TP;
			string toothNum="5";
			double procFee=100d;
			string strDesc="RecallType_"+MethodBase.GetCurrentMethod().Name;
			RecallType recallType=RecallTypeT.CreateRecallType(description:strDesc,procedures:strRecallProcCode);
			PrefT.UpdateString(PrefName.RecallTypesShowingInList,string.Join(",",PrefC.GetString(PrefName.RecallTypesShowingInList),recallType.RecallTypeNum));
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,lName:MethodBase.GetCurrentMethod().Name);
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeRecall,intervalRecall,dateScheduled:dateTimeRecall);
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,dateTimeAppt,opNum,provNum,aptStatus:apptStatus,aptNote:note);
			ProcedureT.CreateProcedure(pat,strApptProcCode,procStatus,toothNum,procFee,dateTimeAppt,provNum:provNum,aptNum:appt.AptNum);
			Assert.IsFalse(Appointments.IsRecallAppointment(appt));
		}

		[TestMethod]
		public void Appointments_IsRecallAppointment_ProcCodesDoNotMatch() {
			long provNum=2;
			long opNum=7;
			string note="test note";
			string strApptProcCode="D0150";
			string strRecallProcCode="D1110";
			Interval intervalRecall=new Interval(0,0,6,0);//6 Month Interval.
			DateTime dateTimeAppt=DateTime.Today;
			DateTime dateTimeRecall=DateTime.Today;
			ApptStatus apptStatus=ApptStatus.Scheduled;
			ProcStat procStatus=ProcStat.TP;
			string toothNum="5";
			double procFee=100d;
			string strDesc="RecallType_"+MethodBase.GetCurrentMethod().Name;
			RecallType recallType=RecallTypeT.CreateRecallType(description:strDesc,procedures:strRecallProcCode);
			PrefT.UpdateString(PrefName.RecallTypesShowingInList,string.Join(",",PrefC.GetString(PrefName.RecallTypesShowingInList),recallType.RecallTypeNum));
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,lName:MethodBase.GetCurrentMethod().Name);
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeRecall,intervalRecall,dateScheduled:dateTimeRecall);
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,dateTimeAppt,opNum,provNum,aptStatus:apptStatus,aptNote:note);
			ProcedureT.CreateProcedure(pat,strApptProcCode,procStatus,toothNum,procFee,dateTimeAppt,provNum:provNum,aptNum:appt.AptNum);
			Assert.IsFalse(Appointments.IsRecallAppointment(appt));
		}

		[TestMethod]
		public void Appointments_IsRecallAppointment_IsRecall() {
			long provNum=2;
			long opNum=7;
			string note="test note";
			string strApptProcCode="D1110";
			string strRecallProcCode="D1110";
			Interval intervalRecall=new Interval(0,0,6,0);//6 Month Interval.
			DateTime dateTimeAppt=DateTime.Today;
			DateTime dateTimeRecall=DateTime.Today;
			ApptStatus apptStatus=ApptStatus.Scheduled;
			ProcStat procStatus=ProcStat.TP;
			string toothNum="5";
			double procFee=100d;
			string strDesc="RecallType_"+MethodBase.GetCurrentMethod().Name;
			RecallType recallType=RecallTypeT.CreateRecallType(description:strDesc,procedures:strRecallProcCode);
			Patient pat=PatientT.CreatePatient(priProvNum:provNum,lName:MethodBase.GetCurrentMethod().Name);
			PrefT.UpdateString(PrefName.RecallTypesShowingInList,string.Join(",",PrefC.GetString(PrefName.RecallTypesShowingInList),recallType.RecallTypeNum));
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeRecall,intervalRecall,dateScheduled:dateTimeRecall);
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,dateTimeAppt,opNum,provNum,aptStatus:apptStatus,aptNote:note);
			ProcedureT.CreateProcedure(pat,strApptProcCode,procStatus,toothNum,procFee,dateTimeAppt,provNum:provNum,aptNum:appt.AptNum);
			Cache.Refresh(InvalidType.RecallTypes);
			Assert.IsTrue(Appointments.IsRecallAppointment(appt));
		}

		[TestMethod]
		public void Appointments_UpdateProcDescriptionForAppt_Appt() {
			//create provider,patient, and appointment
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			//manipulate the test data
			Operatory op=OperatoryT.CreateOperatory();
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,op.OperatoryNum,provNum);
			ProcedureCodeT.CreateProcCode("UPDFA",abbrDesc:"Code Descript");
			Procedure proc=ProcedureT.CreateProcedure(pat,"UPDFA",ProcStat.C,"2",100);
			Procedure procOld=proc.Copy();
			//procedure now has an appointment attached to it.
			Procedures.UpdateAptNum(proc.ProcNum,apt.AptNum);
			proc=Procedures.GetOneProc(proc.ProcNum,false);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsTrue(string.IsNullOrEmpty(apt.ProcDescript));
			Appointments.UpdateProcDescriptionForAppt(proc,procOld);
			//Check appointment has proc descript
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsFalse(string.IsNullOrEmpty(apt.ProcDescript));
		}

		[TestMethod]
		public void Appointments_UpdateProcDescriptionForAppt_PlannedAppt() {
			//create provider,patient, and appointment
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			//manipulate the test data
			Operatory op=OperatoryT.CreateOperatory();
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,op.OperatoryNum,provNum,aptStatus:ApptStatus.Planned);
			ProcedureCodeT.CreateProcCode("UPDFA2",abbrDesc:"Code Descript");
			Procedure proc=ProcedureT.CreateProcedure(pat,"UPDFA2",ProcStat.C,"2",100);
			Procedure procOld=proc.Copy();
			//procedure now has an appointment attached to it.
			Procedures.UpdateAptNums(new List<long>() { proc.ProcNum },apt.AptNum,isPlannedAptNum:true);
			proc=Procedures.GetOneProc(proc.ProcNum,false);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsTrue(string.IsNullOrEmpty(apt.ProcDescript));
			Appointments.UpdateProcDescriptionForAppt(proc,procOld);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsFalse(string.IsNullOrEmpty(apt.ProcDescript));
		}

		[TestMethod]
		public void Appointments_UpdateProcDescriptionForAppt_ApptRemoved() {
			//create provider,patient, and appointment
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			//manipulate the test data
			Operatory op=OperatoryT.CreateOperatory();
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,op.OperatoryNum,provNum);
			ProcedureCodeT.CreateProcCode("UPDFA3",abbrDesc:"Code Descript");
			Procedure proc=ProcedureT.CreateProcedure(pat,"UPDFA3",ProcStat.C,"2",100,aptNum:apt.AptNum);
			Procedure procOld=proc.Copy();
			Appointments.UpdateProcDescriptionForAppt(proc,procOld);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsFalse(string.IsNullOrEmpty(apt.ProcDescript));
			proc.AptNum=0;
			Procedures.UpdateAptNum(proc.ProcNum,proc.AptNum);
			Appointments.UpdateProcDescriptionForAppt(proc,procOld);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsTrue(string.IsNullOrEmpty(apt.ProcDescript));
		}

		[TestMethod]
		public void Appointments_UpdateProcDescriptionForAppt_PlannedRemoved() {
			//create provider,patient, and appointment
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			Patient pat=PatientT.CreatePatient(MethodBase.GetCurrentMethod().Name,provNum);
			//manipulate the test data
			Operatory op=OperatoryT.CreateOperatory();
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now,op.OperatoryNum,provNum,aptStatus:ApptStatus.Planned);
			ProcedureCodeT.CreateProcCode("UPDFA4",abbrDesc:"Code Descript");
			Procedure proc=ProcedureT.CreateProcedure(pat,"UPDFA4",ProcStat.C,"2",100,plannedAptNum:apt.AptNum);
			Procedure procOld=proc.Copy();
			Appointments.UpdateProcDescriptionForAppt(proc,procOld);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsFalse(string.IsNullOrEmpty(apt.ProcDescript));
			proc.PlannedAptNum=0;
			Procedures.UpdateAptNums(new List<long>() { proc.ProcNum },proc.PlannedAptNum,isPlannedAptNum:true);
			Appointments.UpdateProcDescriptionForAppt(proc,procOld);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsTrue(string.IsNullOrEmpty(apt.ProcDescript));
		}

		#region Appointment Double Booking

		[TestMethod]
		public void Appointments_DoubleBooking_ApptRule() {
			string desc=MethodBase.GetCurrentMethod().Name;
			//Clear appointment rules & create a new rule
			AppointmentRuleT.CreateAppointmentRule(desc,"D0110","D150");
			//Setup create two operatories with the same provider
			Clinic clinic=ClinicT.CreateClinic(desc);
			long provNum=ProviderT.CreateProvider(desc);
			Operatory opA=OperatoryT.CreateOperatory(desc+"A",desc+"A",provNum,clinicNum:clinic.ClinicNum);
			Operatory opB=OperatoryT.CreateOperatory(desc+"B",desc+"B",provNum,clinicNum:clinic.ClinicNum);
			//Create two patients
			Patient patA=PatientT.CreatePatient(desc+"A",provNum,clinic.ClinicNum);
			Patient patB=PatientT.CreatePatient(desc+"B",provNum,clinic.ClinicNum);
			//Create two overlapping appointments with a procedure code from that appointment rule in the different ops
			Appointment apptA=AppointmentT.CreateAppointment(patA.PatNum,DateTime.Today.AddHours(8),opA.OperatoryNum,provNum);
			ProcedureT.CreateProcedure(patA,"D0120",ProcStat.TP,"",50,provNum:provNum,aptNum:apptA.AptNum);
			//Create an appointment that would overlap
			Appointment apptB=new Appointment()
			{
				AptDateTime=DateTime.Today.AddHours(8),
				ProvNum=provNum,
				ClinicNum=clinic.ClinicNum,
				PatNum=patB.PatNum,
				Op=opB.OperatoryNum,
				Pattern=apptA.Pattern,
			};
			DataTable dtAppts=Appointments.GetPeriodApptsTableMini(apptB.AptDateTime,provNum);
			List<long> listAppts=new List<long>();
			foreach(DataRow row in dtAppts.Rows) {
				listAppts.Add(PIn.Long(row["AptNum"].ToString()));
			}
			List<Procedure> procsMultApts=Procedures.GetProcsMultApts(listAppts);
			List<string> listProcCodesDB=Appointments.GetDoubleBookedCodes(apptB,dtAppts,procsMultApts,new Procedure[] { });
			//Ensure that we found that there would be a double booking issue
			Assert.IsTrue(listProcCodesDB.Count>0);
		}

		[TestMethod]
		public void Appointments_DoubleBooking_WebSchedNewPat_PreventApptRule() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			string desc=MethodBase.GetCurrentMethod().Name;
			DateTime tomorrow=DateTime.Today.AddDays(1);
			AppointmentRuleT.CreateAppointmentRule(desc,"D0110","D1110");
			//Set the webschednewpat Pref to not allow double booking
			Prefs.UpdateInt(PrefName.WebSchedNewPatApptDoubleBooking,1);
			//Set the appttype as a webschednewpat appt type
			AppointmentType apptType=AppointmentTypeT.CreateAppointmentType(desc,codeStr:"D0120",pattern:"//XXXX//");
			Def wsnpApptTypeDef=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,desc,POut.Long(apptType.AppointmentTypeNum));
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,apptType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Setup create two operatories with the same provider and set these available for wsnp
			Clinic clinic=ClinicT.CreateClinic(desc);
			long provNum=ProviderT.CreateProvider(desc);
			Operatory opA=OperatoryT.CreateOperatory(desc+"A",desc+"A",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Operatory opB=OperatoryT.CreateOperatory(desc+"B",desc+"B",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,opA.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,opB.OperatoryNum,DefLinkType.Operatory);
			//Set schedules, and confirm we can get all the timeslots we expect
			Schedule provSched=ScheduleT.CreateSchedule(tomorrow,TimeSpan.FromHours(8),TimeSpan.FromHours(16),schedType:ScheduleType.Provider,clinicNum:clinic.ClinicNum,provNum:provNum,listOpNums:new List<long>() { opA.OperatoryNum,opB.OperatoryNum });
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(tomorrow.Date,tomorrow.AddHours(23),clinic.ClinicNum,wsnpApptTypeDef.DefNum);
			Assert.IsTrue(listTimeSlots.Any(x => x.DateTimeStart==tomorrow.AddHours(12)));
			//Create an appointment that would use the appointment rule we created
			Patient patA=PatientT.CreatePatient(desc+"A",provNum,clinic.ClinicNum);
			Appointment apptA=AppointmentT.CreateAppointment(patA.PatNum,tomorrow.AddHours(12),opA.OperatoryNum,provNum);
			ProcedureT.CreateProcedure(patA,"D0120",ProcStat.TP,"",50,provNum:provNum,aptNum:apptA.AptNum);
			//Refetch timeslots and make sure that the list does not contain the double booked slot
			List<TimeSlot> listTimeSlotsAfter=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(tomorrow.Date,tomorrow.AddHours(23),clinic.ClinicNum,wsnpApptTypeDef.DefNum);
			Assert.IsFalse(listTimeSlotsAfter.Any(x => x.DateTimeStart==tomorrow.AddHours(12)));
		}

		[TestMethod]
		public void Appointments_DoubleBooking_WebSchedNewPat_AllowedNoApptRule() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			string desc=MethodBase.GetCurrentMethod().Name;
			DateTime tomorrow=DateTime.Today.AddDays(1);
			//Set the webschednewpat Pref to allow double booking
			Prefs.UpdateInt(PrefName.WebSchedNewPatApptDoubleBooking,0);
			//Set the appttype as a webschednewpat appt type
			AppointmentType apptType=AppointmentTypeT.CreateAppointmentType(desc,codeStr:"D0120",pattern:"//XXXX//");
			Def wsnpApptTypeDef=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,desc,POut.Long(apptType.AppointmentTypeNum));
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,apptType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Setup create two operatories with the same provider and set these available for wsnp
			Clinic clinic=ClinicT.CreateClinic(desc);
			long provNum=ProviderT.CreateProvider(desc);
			Operatory opA=OperatoryT.CreateOperatory(desc+"A",desc+"A",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Operatory opB=OperatoryT.CreateOperatory(desc+"B",desc+"B",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,opA.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,opB.OperatoryNum,DefLinkType.Operatory);
			//Set schedules, and confirm we can get all the timeslots we expect
			Schedule provSched=ScheduleT.CreateSchedule(tomorrow,TimeSpan.FromHours(8),TimeSpan.FromHours(16),schedType:ScheduleType.Provider,clinicNum:clinic.ClinicNum,provNum:provNum,listOpNums:new List<long>() { opA.OperatoryNum,opB.OperatoryNum });
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(tomorrow.Date,tomorrow.AddHours(23),clinic.ClinicNum,wsnpApptTypeDef.DefNum);
			Assert.IsTrue(listTimeSlots.Any(x => x.DateTimeStart==tomorrow.AddHours(12)));
			//Create an appointment that would use the appointment rule we created
			Patient patA=PatientT.CreatePatient(desc+"A",provNum,clinic.ClinicNum);
			Appointment apptA=AppointmentT.CreateAppointment(patA.PatNum,tomorrow.AddHours(12),opA.OperatoryNum,provNum);
			ProcedureT.CreateProcedure(patA,"D0120",ProcStat.TP,"",50,provNum:provNum,aptNum:apptA.AptNum);
			//Refetch timeslots and make sure that the list does contain the double booked slot
			List<TimeSlot> listTimeSlotsAfter=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(tomorrow.Date,tomorrow.AddHours(23),clinic.ClinicNum,wsnpApptTypeDef.DefNum);
			Assert.IsTrue(listTimeSlotsAfter.Any(x => x.DateTimeStart==tomorrow.AddHours(12)));
		}

		[TestMethod]
		public void Appointments_DoubleBooking_WebSchedNewPat_PreventNoApptRule() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			string desc=MethodBase.GetCurrentMethod().Name;
			DateTime tomorrow=DateTime.Today.AddDays(1);
			//Set the webschednewpat Pref to not allow double booking
			Prefs.UpdateInt(PrefName.WebSchedNewPatApptDoubleBooking,1);
			//Set the appttype as a webschednewpat appt type
			AppointmentType apptType=AppointmentTypeT.CreateAppointmentType(desc,codeStr:"D0120",pattern:"//XXXX//");
			Def wsnpApptTypeDef=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,desc,POut.Long(apptType.AppointmentTypeNum));
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,apptType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Setup create two operatories with the same provider and set these available for wsnp
			Clinic clinic=ClinicT.CreateClinic(desc);
			long provNum=ProviderT.CreateProvider(desc);
			Operatory opA=OperatoryT.CreateOperatory(desc+"A",desc+"A",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Operatory opB=OperatoryT.CreateOperatory(desc+"B",desc+"B",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,opA.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsnpApptTypeDef.DefNum,opB.OperatoryNum,DefLinkType.Operatory);
			//Set schedules, and confirm we can get all the timeslots we expect
			Schedule provSched=ScheduleT.CreateSchedule(tomorrow,TimeSpan.FromHours(8),TimeSpan.FromHours(16),schedType:ScheduleType.Provider,clinicNum:clinic.ClinicNum,provNum:provNum,listOpNums:new List<long>() { opA.OperatoryNum,opB.OperatoryNum });
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(tomorrow.Date,tomorrow.AddHours(23),clinic.ClinicNum,wsnpApptTypeDef.DefNum);
			Assert.IsTrue(listTimeSlots.Any(x => x.DateTimeStart==tomorrow.AddHours(12)));
			//Create an appointment that would use the appointment rule we created
			Patient patA=PatientT.CreatePatient(desc+"A",provNum,clinic.ClinicNum);
			Appointment apptA=AppointmentT.CreateAppointment(patA.PatNum,tomorrow.AddHours(12),opA.OperatoryNum,provNum);
			ProcedureT.CreateProcedure(patA,"D0120",ProcStat.TP,"",50,provNum:provNum,aptNum:apptA.AptNum);
			//Refetch timeslots and make sure that the list does not contain the double booked slot
			List<TimeSlot> listTimeSlotsAfter=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(tomorrow.Date,tomorrow.AddHours(23),clinic.ClinicNum,wsnpApptTypeDef.DefNum);
			Assert.IsFalse(listTimeSlotsAfter.Any(x => x.DateTimeStart==tomorrow.AddHours(12)));
		}

		[TestMethod]
		public void Appointments_DoubleBooking_WebSchedRecall_AllowedNoApptRule() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			string desc=MethodBase.GetCurrentMethod().Name;
			DateTime tomorrow=DateTime.Today.AddDays(1);
			//Set the webschednewpat Pref to allow double booking
			Prefs.UpdateInt(PrefName.WebSchedRecallDoubleBooking,0);
			//Create a recall type
			RecallTypeT.ClearRecallTypeTable();
			RecallType recallType=RecallTypeT.CreateRecallType(timePattern:"XXXX");
			//Setup create two operatories with the same provider and set these available for web sched
			Clinic clinic=ClinicT.CreateClinic(desc);
			long provNum=ProviderT.CreateProvider(desc);
			Operatory opA=OperatoryT.CreateOperatory(desc+"A",desc+"A",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Operatory opB=OperatoryT.CreateOperatory(desc+"B",desc+"B",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Schedule provSched=ScheduleT.CreateSchedule(tomorrow,TimeSpan.FromHours(8),TimeSpan.FromHours(16),schedType:ScheduleType.Provider,clinicNum:clinic.ClinicNum,provNum:provNum,listOpNums:new List<long>() { opA.OperatoryNum,opB.OperatoryNum });
			//Create a patient with a recall
			Patient patA=PatientT.CreatePatient(desc+"A",provNum,clinic.ClinicNum);
			Recall recallA=RecallT.CreateRecall(patA.PatNum,recallType.RecallTypeNum,tomorrow,new Interval());
			//Check we get the correct time slots
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recallA.RecallNum,tomorrow,tomorrow,provNum,false);
			Assert.IsTrue(listTimeSlots.Any(x => x.DateTimeStart==tomorrow.AddHours(8)));
			//Create the recall appointment for this patient
			Appointment apptA=AppointmentT.CreateAppointmentFromRecall(recallA,patA,tomorrow.AddHours(8),opA.OperatoryNum,provNum);
			//Create next patient
			Patient patB=PatientT.CreatePatient(desc+"B",provNum,clinic.ClinicNum);
			Recall recallB=RecallT.CreateRecall(patA.PatNum,recallType.RecallTypeNum,tomorrow,new Interval());
			//Check we get the correct time slots
			List<TimeSlot> listTimeSlotsAfter=TimeSlots.GetAvailableWebSchedTimeSlots(recallB.RecallNum,tomorrow,tomorrow,provNum,false);
			Assert.IsTrue(listTimeSlotsAfter.Any(x => x.DateTimeStart==tomorrow.AddHours(8)));
		}

		[TestMethod]
		public void Appointments_DoubleBooking_WebhSchedRecall_PreventNoApptRule() {
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			string desc=MethodBase.GetCurrentMethod().Name;
			DateTime tomorrow=DateTime.Today.AddDays(1);
			//Set the webschednewpat Pref to not allow double booking
			Prefs.UpdateInt(PrefName.WebSchedRecallDoubleBooking,1);
			//Create a recall type
			RecallTypeT.ClearRecallTypeTable();
			RecallType recallType=RecallTypeT.CreateRecallType(timePattern:"XXXX");
			//Setup create two operatories with the same provider and set these available for web sched
			Clinic clinic=ClinicT.CreateClinic(desc);
			long provNum=ProviderT.CreateProvider(desc);
			Operatory opA=OperatoryT.CreateOperatory(desc+"A",desc+"A",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Operatory opB=OperatoryT.CreateOperatory(desc+"B",desc+"B",provNum,clinicNum:clinic.ClinicNum,isWebSched:true);
			Schedule provSched=ScheduleT.CreateSchedule(tomorrow,TimeSpan.FromHours(8),TimeSpan.FromHours(16),schedType:ScheduleType.Provider,clinicNum:clinic.ClinicNum,provNum:provNum,listOpNums:new List<long>() { opA.OperatoryNum,opB.OperatoryNum });
			//Create a patient with a recall
			Patient patA=PatientT.CreatePatient(desc+"A",provNum,clinic.ClinicNum);
			Recall recallA=RecallT.CreateRecall(patA.PatNum,recallType.RecallTypeNum,tomorrow,new Interval());
			//Check we get the correct time slots
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recallA.RecallNum,tomorrow,tomorrow,provNum,false);
			Assert.IsTrue(listTimeSlots.Any(x => x.DateTimeStart==tomorrow.AddHours(8)));
			//Create the recall appointment for this patient
			Appointment apptA=AppointmentT.CreateAppointmentFromRecall(recallA,patA,tomorrow.AddHours(8),opA.OperatoryNum,provNum);
			//Create next patient
			Patient patB=PatientT.CreatePatient(desc+"B",provNum,clinic.ClinicNum);
			Recall recallB=RecallT.CreateRecall(patA.PatNum,recallType.RecallTypeNum,tomorrow,new Interval());
			//Check we get the correct time slots
			List<TimeSlot> listTimeSlotsAfter=TimeSlots.GetAvailableWebSchedTimeSlots(recallB.RecallNum,tomorrow,tomorrow,provNum,false);
			Assert.IsFalse(listTimeSlotsAfter.Any(x => x.DateTimeStart==tomorrow.AddHours(8)));
		}


		#endregion 

		[TestMethod]
		public void UserContrApptsPanelJ_RoundTimeToNearestIncrement(){
			float minPerIncr=10;
			TimeSpan timeSpanInput=new TimeSpan(11,50,0);
			TimeSpan timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(11,50,0));
			timeSpanInput=new TimeSpan(11,52,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(11,50,0));
			timeSpanInput=new TimeSpan(11,48,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(11,50,0));
			timeSpanInput=new TimeSpan(10,08,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(10,10,0));
			minPerIncr=15;
			timeSpanInput=new TimeSpan(10,08,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(10,15,0));
			minPerIncr=5;
			timeSpanInput=new TimeSpan(10,07,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeToNearestIncrement(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(10,5,0));
			//If any new tests are needed, it will quickly become obvious
		}

		[TestMethod]
		public void UserContrApptsPanelJ_RoundTimeDown(){
			float minPerIncr=10;
			TimeSpan timeSpanInput=new TimeSpan(11,50,0);
			TimeSpan timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeDown(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(11,50,0));
			timeSpanInput=new TimeSpan(11,52,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeDown(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(11,50,0));
			timeSpanInput=new TimeSpan(11,48,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeDown(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(11,40,0));
			timeSpanInput=new TimeSpan(10,08,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeDown(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(10,0,0));
			minPerIncr=15;
			timeSpanInput=new TimeSpan(10,17,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeDown(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(10,15,0));
			minPerIncr=5;
			timeSpanInput=new TimeSpan(10,07,0);
			timeSpanResult=OpenDental.UI.ControlApptPanel.RoundTimeDown(timeSpanInput,minPerIncr);
			Assert.IsTrue(timeSpanResult==new TimeSpan(10,5,0));
		}

		[TestMethod]
		public void ChartModules_GetPlannedAppts_PlannedToScheduledToBrokenToScheduled() {
			//Models and appointment going from Planned to Scheduled to complete. Adds both procs.
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"D9430");//Set Per Visit Prefs.
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"D9440");
			//create provider and schedule.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			Appointment appointmentPlanned=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Planned);
			//Create planned appt
			Appointment appointmentPlannedScheduled=Appointments.SchedulePlannedApt(appointmentPlanned,patient,new List<ApptField>(),DateTime.Now,appointmentPlanned.Op);
			//Break the scheduled appointment. 
			Appointment appointmentBroken=appointmentPlannedScheduled.Copy();
			appointmentPlannedScheduled.AptStatus=ApptStatus.Broken;
			Appointments.Update(appointmentPlannedScheduled,appointmentBroken);
			//Schedule the planned appointment again.
			Appointment appointmentPlannedScheduled2=Appointments.SchedulePlannedApt(appointmentPlanned,patient,new List<ApptField>(),DateTime.Now,appointmentPlanned.Op);
			//Need to call this method to set the patient appointments.
			ChartModules.GetProgNotes(patient.PatNum,false);
			//Get the planned appointment
			DataTable tablePlannedAppt=ChartModules.GetPlannedApt(patient.PatNum);
			Assert.AreEqual(1,tablePlannedAppt.Rows.Count);
			//The planned appointment should be linked to the appointment
			Assert.AreEqual(appointmentPlannedScheduled2.AptNum,long.Parse(tablePlannedAppt.Rows[0]["SchedAptNum"].ToString()));
			Assert.AreEqual((int)appointmentPlannedScheduled2.AptStatus,int.Parse(tablePlannedAppt.Rows[0]["AptStatus"].ToString()));
		}

		[TestMethod]
		public void ChartModules_GetPlannedAppts_PlannedToScheduledToBrokenToScheduled_MultiplePlannedAppt() {
			//Models and appointment going from Planned to Scheduled to complete. Adds both procs.
			PrefT.UpdateString(PrefName.PerVisitInsAmountProcCode,"D9430");//Set Per Visit Prefs.
			PrefT.UpdateString(PrefName.PerVisitPatAmountProcCode,"D9440");
			//create provider and schedule.
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name+"ProvDentTest");
			AppointmentSearchData appSearchData=AppointmentT.CreateScheduleAndOpsForProv(2,0,provNum,11);
			Operatory operatory=appSearchData.ListOps[0];//Get one of the ops for the provider. 
			Patient patient=PatientT.CreatePatient("pat1");
			Appointment appointmentPlannedNotUsed=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now.AddDays(-1),operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Planned);
			Appointment appointmentPlanned=AppointmentT.CreateAppointment(patient.PatNum,DateTime.Now,operatory.OperatoryNum,provNum,aptStatus:ApptStatus.Planned);
			Appointment appointmentPlannedScheduled=Appointments.SchedulePlannedApt(appointmentPlanned,patient,new List<ApptField>(),DateTime.Now,appointmentPlanned.Op);
			//Break the scheduled appointment. 
			Appointment appointmentBroken=appointmentPlannedScheduled.Copy();
			appointmentPlannedScheduled.AptStatus=ApptStatus.Broken;
			Appointments.Update(appointmentPlannedScheduled,appointmentBroken);
			//Schedule the planned appointment again.
			Appointment appointmentPlannedScheduled2=Appointments.SchedulePlannedApt(appointmentPlanned,patient,new List<ApptField>(),DateTime.Now,appointmentPlanned.Op);
			//Need to call this method to set the patient appointments.
			ChartModules.GetProgNotes(patient.PatNum,false);
			//Get the planned appointment
			DataTable tablePlannedAppt=ChartModules.GetPlannedApt(patient.PatNum);
			Assert.AreEqual(2,tablePlannedAppt.Rows.Count);
			DataRow dataRow=tablePlannedAppt.Select().FirstOrDefault(x => PIn.Long(x["AptNum"].ToString())==appointmentPlanned.AptNum);
			//The planned appointment should be linked to the appointment
			Assert.AreEqual(appointmentPlannedScheduled2.AptNum,PIn.Long(dataRow["SchedAptNum"].ToString()));
			Assert.AreEqual((int)appointmentPlannedScheduled2.AptStatus,PIn.Int(dataRow["AptStatus"].ToString()));
		}

		[TestMethod]
		public void Appointments_GetPeriodApptsTable_OneLabcaseAssociatedToAppt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider("prov1","terry","smith",ssn:"123456789",isUsingTIN:true,nationalProvID:"0123456789");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum,birthDate:DateTime.Today);
			Operatory op=OperatoryT.CreateOperatory();
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,op.OperatoryNum,provNum);
			LabCase labCase=new LabCase();
			labCase.AptNum=appt.AptNum;
			labCase.ProvNum=pat.PriProv;
			labCase.PatNum=pat.PatNum;
			labCase.DateTimeChecked=DateTime.Now;
			LabCases.Insert(labCase);
			DataTable tableAppt=Appointments.GetPeriodApptsTable(DateTime.Today,DateTime.Today,0,false);
			Assert.AreEqual(1,tableAppt.Rows.Count);
			Assert.IsTrue(!string.IsNullOrEmpty(tableAppt.Rows[0]["lab"].ToString()));
		}

		[TestMethod]
		public void Appointments_GetPeriodApptsTable_OneLabcaseAssociatedToAppt_PlannedApt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider("prov1","terry","smith",ssn:"123456789",isUsingTIN:true,nationalProvID:"0123456789");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum,birthDate:DateTime.Today);
			Operatory op=OperatoryT.CreateOperatory();
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,op.OperatoryNum,provNum);
			LabCase labCase=new LabCase();
			labCase.PlannedAptNum=appt.AptNum;
			labCase.ProvNum=pat.PriProv;
			labCase.PatNum=pat.PatNum;
			labCase.DateTimeChecked=DateTime.Now;
			LabCases.Insert(labCase);
			DataTable tableAppt=Appointments.GetPeriodApptsTable(DateTime.Today,DateTime.Today,0,true);
			Assert.AreEqual(1,tableAppt.Rows.Count);
			Assert.IsTrue(!string.IsNullOrEmpty(tableAppt.Rows[0]["lab"].ToString()));
		}

		[TestMethod]
		public void Appointments_GetPeriodApptsTable_MoreThanOneLabcaseAssociatedToAppt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider("prov1","terry","smith",ssn:"123456789",isUsingTIN:true,nationalProvID:"0123456789");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum,birthDate:DateTime.Today);
			Operatory op=OperatoryT.CreateOperatory();
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,op.OperatoryNum,provNum);
			LabCase labCase=new LabCase();
			labCase.AptNum=appt.AptNum;
			labCase.ProvNum=pat.PriProv;
			labCase.PatNum=pat.PatNum;
			labCase.DateTimeChecked=DateTime.Now;
			LabCases.Insert(labCase);
			labCase=new LabCase();
			labCase.AptNum=appt.AptNum;
			labCase.ProvNum=pat.PriProv;
			labCase.PatNum=pat.PatNum;
			labCase.DateTimeChecked=DateTime.Now;
			LabCases.Insert(labCase);
			DataTable tableAppt=Appointments.GetPeriodApptsTable(DateTime.Today,DateTime.Today,0,false);
			Assert.AreEqual(1,tableAppt.Rows.Count);
			Assert.IsTrue(!string.IsNullOrEmpty(tableAppt.Rows[0]["lab"].ToString()));
		}

		[TestMethod]
		public void Appointments_GetPeriodApptsTable_MoreThanOneLabcaseAssociatedToAppt_PlannedApt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			long provNum=ProviderT.CreateProvider("prov1","terry","smith",ssn:"123456789",isUsingTIN:true,nationalProvID:"0123456789");
			Patient pat=PatientT.CreatePatient(suffix,priProvNum:provNum,birthDate:DateTime.Today);
			Operatory op=OperatoryT.CreateOperatory();
			Appointment appt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today,op.OperatoryNum,provNum);
			LabCase labCase=new LabCase();
			labCase.PlannedAptNum=appt.AptNum;
			labCase.ProvNum=pat.PriProv;
			labCase.PatNum=pat.PatNum;
			labCase.DateTimeChecked=DateTime.Now;
			LabCases.Insert(labCase);
			labCase=new LabCase();
			labCase.PlannedAptNum=appt.AptNum;
			labCase.ProvNum=pat.PriProv;
			labCase.PatNum=pat.PatNum;
			labCase.DateTimeChecked=DateTime.Now;
			LabCases.Insert(labCase);
			DataTable tableAppt=Appointments.GetPeriodApptsTable(DateTime.Today,DateTime.Today,0,true);
			Assert.AreEqual(1,tableAppt.Rows.Count);
			Assert.IsTrue(!string.IsNullOrEmpty(tableAppt.Rows[0]["lab"].ToString()));
		}
	}
}
