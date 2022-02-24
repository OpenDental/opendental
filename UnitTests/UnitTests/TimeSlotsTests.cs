using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.WebSched.TimeSlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;

namespace UnitTests.TimeSlots_Tests {
	[TestClass]
	public class TimeSlotsTests:TestBase {

		///<summary>Every test in this class needs to start with fresh time slot related tables so that their logic is extremely predictable.</summary>
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

		///<summary>Web Sched should not fail to find time slots for clinic A when clinic B has invalid settings or other invalid things.
		///This particular test is to verify that an invalid appointment (that spans past midnight) for clinic B doesn't affect clinic A.
		///This was a real life scenario that happened (due to a conversion).</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableNewPatApptTimeSlots_ClinicWithInvalidAppointment() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Make sure that clinics are enabled for this test.
			//Create two clinics.  Clinic B will have an invalid appointment scheduled (spans past midnight) and Clinic A should still return time slots.
			PrefT.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			long clinicNumA=ClinicT.CreateClinic("Clinic A-"+suffix).ClinicNum;
			long clinicNumB=ClinicT.CreateClinic("Clinic B-"+suffix).ClinicNum;
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDocA=ProviderT.CreateProvider("Doc A-"+suffix);
			long provNumDocB=ProviderT.CreateProvider("Doc B-"+suffix);
			//Create a patient and have them associated to clinic B and provider B (they will have the invalid appt for clinic B).
			Patient patientB=PatientT.CreatePatient("Pat B-"+suffix,provNumDocB,clinicNumB);
			//Create an operatory for each clinic for the corresponding providers.
			Operatory opDocA=OperatoryT.CreateOperatory("A-"+suffix,"Doc Op A-"+suffix,provNumDocA,clinicNum: clinicNumA);
			Operatory opDocB=OperatoryT.CreateOperatory("B-"+suffix,"Doc Op B-"+suffix,provNumDocB,clinicNum: clinicNumB);
			//Create a new patient appointment type that has a pattern that will fit within an hour block (40 mins).
			AppointmentType appointmentType=AppointmentTypeT.CreateAppointmentType(suffix,pattern:"/XXXXXX/");
			Def defApptType=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix);
			DefLinkT.CreateDefLink(defApptType.DefNum,appointmentType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Associate the new patient appointment type to the operatories above so that they are valid "new pat appt" ops.
			DefLinkT.CreateDefLink(defApptType.DefNum,opDocA.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(defApptType.DefNum,opDocB.OperatoryNum,DefLinkType.Operatory);
			//Create schedules for the doctors from 09:00 - 10:00 on the same day.
			Schedule schedDocA=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDocA,clinicNum: clinicNumA
				,listOpNums: new List<long>() { opDocA.OperatoryNum });
			Schedule schedDocB=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType: ScheduleType.Provider,provNum: provNumDocB,clinicNum: clinicNumB
				,listOpNums: new List<long>() { opDocB.OperatoryNum });
			//Create an invalid appointment at 20:00 that spans to 02:00 the next day for patientB within operatory B that is associated to clinic B.
			DateTime dateTimeApptStart=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,20,0,0);
			AppointmentT.CreateAppointment(patientB.PatNum,dateTimeApptStart,opDocB.OperatoryNum,provNumDocB
				,pattern: "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",clinicNum: clinicNumB);
			//An open time slot should be returned for clinic A.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(dateTimeSchedule,dateTimeSchedule.AddDays(5),clinicNumA
				,defApptType.DefNum);
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.AreEqual(listTimeSlots[0].DateTimeStart.Date,dateTimeSchedule.Date);
			Assert.AreEqual(listTimeSlots[0].DateTimeStart,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0));
			Assert.AreEqual(listTimeSlots[0].DateTimeStop,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,40,0));
			Assert.AreEqual(listTimeSlots[0].OperatoryNum,opDocA.OperatoryNum);
			Assert.AreEqual(listTimeSlots[0].ProvNum,provNumDocA);
			//Finding a time slot for clinic B should throw an exception due to the invalid appointment.
			bool isExpectedResult=false;
			try {
				//The following method call should throw an exception OR return no results (if we end up fixing the core issue) for clinic B.
				listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(dateTimeSchedule,dateTimeSchedule.AddDays(5),clinicNumB
					,defApptType.DefNum);
				if(listTimeSlots.Count==0) {
					isExpectedResult=true;
				}
			}
			catch {
				isExpectedResult=true;
			}
			Assert.IsTrue(isExpectedResult);
		}

		///<summary>Web Sched New Pat Appts - Double Booking.  Providers should not be overwhelmed.  
		///An open time slot does not mean that the slot should always be returned to the patient as available.
		///If the provider is double booked, the time slot should not be offered as a choice.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableNewPatApptTimeSlots_DoubleBooking() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Prefs.UpdateBool(PrefName.WebSchedNewPatApptDoubleBooking,true);
			//Create a date that will always be in the future.  This date will be used for schedules and appointments.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			DateTime dateTimeScheduleNextDay=dateTimeSchedule.AddDays(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			//Create operatories for the providers.
			//Firt op will ONLY have provNumDoc associated, NOT the hygienist because we want to keep it simple with one provider to consider.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,itemOrder:2);
			//Now for two non-Web Sched ops that the provNumDoc will be double booked for.  Hyg provs on these ops doesn't affect anything.
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,itemOrder:1,isHygiene:true);
			Operatory opExtra=OperatoryT.CreateOperatory("3-"+suffix,"Extra Op - "+suffix,provNumDoc,provNumHyg,itemOrder:0);
			Patient pat=PatientT.CreatePatient(suffix,provNumHyg);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			//Create a ProcedureCode specifically for the Web Sched New Pat appointment type.
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("T5743");
			//Create a new patient appointment type that has a pattern that will fit within an hour block (40 mins).
			AppointmentType appointmentType=AppointmentTypeT.CreateAppointmentType(suffix,codeStr:procCode.ProcCode,pattern:"/XXXXXX/");
			Def defApptType=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix);
			DefLinkT.CreateDefLink(defApptType.DefNum,appointmentType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Associate the new patient appointment type to the operatories above so that they are valid "new pat appt" ops.
			DefLinkT.CreateDefLink(defApptType.DefNum,opDoc.OperatoryNum,DefLinkType.Operatory);
			//Create an appointment rule for the appointment type procedure code which will cause double booking to be considered.
			AppointmentRuleT.CreateAppointmentRule(suffix,"T5743","T5743");
			//Create two schedules for the doctor from 09:00 - 10:00 on two different days.
			Schedule schedDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			Schedule schedDocNextDay=ScheduleT.CreateSchedule(dateTimeScheduleNextDay,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Now link up the schedule entries to all of the operatories
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocNextDay.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocNextDay.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opExtra.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opExtra.OperatoryNum,schedDocNextDay.ScheduleNum);
			//Create two appointments for the two non-Web Sched operatories during the scheduled times on the first day.
			Appointment apptHyg=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opHyg.OperatoryNum,provNumDoc,pattern: "//XXXXXXXX");
			Appointment apptExtra=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opExtra.OperatoryNum,provNumDoc,pattern: "//XXXXXXXX");
			//Associate a new procedure for the appointment type ProcCode to each appointment.
			ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.TP,"",0,provNum:provNumDoc,aptNum:apptHyg.AptNum);
			ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.TP,"",0,provNum:provNumDoc,aptNum:apptExtra.AptNum);
			//The open time slot returned should be for the Web Sched operatory on the NEXT DAY due to double booking appointments on the current day.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(dateTimeSchedule,dateTimeSchedule.AddDays(30),0,defApptType.DefNum);
			//There should be 1 time slot returned that spans from 09:00 - 09:40 for the next day.
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.AreEqual(opDoc.OperatoryNum,listTimeSlots[0].OperatoryNum);
			Assert.AreEqual(dateTimeScheduleNextDay.Date,listTimeSlots[0].DateTimeStart.Date);
			Assert.AreEqual(new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,0,0),listTimeSlots[0].DateTimeStart);
			Assert.AreEqual(new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,40,0),listTimeSlots[0].DateTimeStop);
		}

		///<summary>Web Sched New Pat Appts - Double Booking for hygienist.  Providers and hygienists should not be overwhelmed.
		///An open time slot does not mean that the slot should always be returned to the patient as available.
		///If the provider is double booked, the time slot should not be offered as a choice.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableNewPatApptTimeSlots_DoubleBooking_IsHygiene() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Prefs.UpdateBool(PrefName.WebSchedNewPatApptDoubleBooking,true);
			//Create a date that will always be in the future.  This date will be used for schedules and appointments.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			DateTime dateTimeScheduleNextDay=dateTimeSchedule.AddDays(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			#region Operatories
			//Create two operatories for the providers that are flagged as hygiene.
			Operatory opHyg1=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op 1- "+suffix,provNumDoc,provNumHyg,itemOrder:2,isHygiene:true);
			Operatory opHyg2=OperatoryT.CreateOperatory("3-"+suffix,"Hyg Op 2- "+suffix,provNumDoc,provNumHyg,itemOrder:1,isHygiene:true);
			#endregion
			#region Appointment Types
			//Create a ProcedureCode specifically for the Web Sched New Pat appointment type.
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("T5743");
			//Create a new patient appointment type that has a pattern that will fit within an hour block (40 mins).
			AppointmentType appointmentType=AppointmentTypeT.CreateAppointmentType(suffix,codeStr:procCode.ProcCode,pattern:"XXXXXXXX");
			Def defApptType=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix);
			DefLinkT.CreateDefLink(defApptType.DefNum,appointmentType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Associate the new patient appointment type to the hygiene operatories above so that they are valid "new pat appt" ops.
			DefLinkT.CreateDefLink(defApptType.DefNum,opHyg1.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(defApptType.DefNum,opHyg2.OperatoryNum,DefLinkType.Operatory);
			#endregion
			#region Schedules
			//Create two schedules for the hygienist from 09:00 - 10:00 on two different days.
			Schedule schedHyg=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			Schedule schedHygNextDay=ScheduleT.CreateSchedule(dateTimeScheduleNextDay,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			//Now link up the schedule entries to all of the operatories
			ScheduleOpT.CreateScheduleOp(opHyg1.OperatoryNum,schedHyg.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg1.OperatoryNum,schedHygNextDay.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg2.OperatoryNum,schedHyg.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg2.OperatoryNum,schedHygNextDay.ScheduleNum);
			#endregion
			#region Appointments
			Appointment apptExtra=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opHyg2.OperatoryNum,provNumDoc,provHyg: provNumHyg,pattern: "//XXXXXXXX",isHygiene: true);
			//Associate a new procedure for the appointment type ProcCode to each appointment.
			ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.TP,"",0,provNum: provNumDoc,aptNum: apptExtra.AptNum);
			#endregion
			//The open time slot returned should be for the Web Sched operatory on the NEXT DAY due to double booking appointments on the current day.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(dateTimeSchedule,dateTimeSchedule.AddDays(30),0,defApptType.DefNum);
			//There should be 1 time slot returned that spans from 09:00 - 09:40 for the next day.
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.AreEqual(opHyg2.OperatoryNum,listTimeSlots[0].OperatoryNum);
			Assert.AreEqual(dateTimeScheduleNextDay.Date,listTimeSlots[0].DateTimeStart.Date);
			Assert.AreEqual(new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,0,0),listTimeSlots[0].DateTimeStart);
			Assert.AreEqual(new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,40,0),listTimeSlots[0].DateTimeStop);
		}

		///<summary>Web Sched New Pat Appts - Double Booking for hygienist.  Providers should not be overwhelmed.  
		///An open time slot does not mean that the slot should always be returned to the patient as available.
		///If the provider is double booked via an Appointment Rule (regardless of Double Booking pref), the time slot should not be offered.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlotsForNewOrExistingPat_DoubleBooking_IsHygiene_AppointmentRule() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Patient pat=PatientT.CreatePatient(suffix);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Prefs.UpdateBool(PrefName.WebSchedNewPatApptDoubleBooking,false);//Set the pref false so that the Appt Rule restricts via double booking.
			//Create a date that will always be in the future.  This date will be used for schedules and appointments.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			DateTime dateTimeScheduleNextDay=dateTimeSchedule.AddDays(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			#region Operatories
			//Create two operatories for the providers that are flagged as hygiene.
			Operatory opHyg1=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op 1- "+suffix,provNumDoc,provNumHyg,itemOrder:2,isHygiene:true);
			Operatory opHyg2=OperatoryT.CreateOperatory("3-"+suffix,"Hyg Op 2- "+suffix,provNumDoc,provNumHyg,itemOrder:1,isHygiene:true);
			#endregion
			#region Appointment Types
			//Create a ProcedureCode specifically for the Web Sched New Pat appointment type.
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode("T5743");
			//Create an appointment rule for the appointment type procedure code which will cause double booking to be considered (regardless of pref).
			AppointmentRuleT.CreateAppointmentRule(suffix,procCode.ProcCode,procCode.ProcCode);
			//Create a new patient appointment type that has a pattern that will fit within an hour block (40 mins).
			AppointmentType appointmentType=AppointmentTypeT.CreateAppointmentType(suffix,codeStr:procCode.ProcCode,pattern:"XXXXXXXX");
			Def defApptType=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix);
			DefLinkT.CreateDefLink(defApptType.DefNum,appointmentType.AppointmentTypeNum,DefLinkType.AppointmentType);
			//Associate the new patient appointment type to the hygiene operatories above so that they are valid "new pat appt" ops.
			DefLinkT.CreateDefLink(defApptType.DefNum,opHyg1.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(defApptType.DefNum,opHyg2.OperatoryNum,DefLinkType.Operatory);
			#endregion
			#region Schedules
			//Create two schedules for the hygienist from 09:00 - 10:00 on two different days.
			Schedule schedHyg=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			Schedule schedHygNextDay=ScheduleT.CreateSchedule(dateTimeScheduleNextDay,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			//Now link up the schedule entries to all of the operatories
			ScheduleOpT.CreateScheduleOp(opHyg1.OperatoryNum,schedHyg.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg1.OperatoryNum,schedHygNextDay.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg2.OperatoryNum,schedHyg.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg2.OperatoryNum,schedHygNextDay.ScheduleNum);
			#endregion
			#region Appointments
			Appointment apptExtra=AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opHyg2.OperatoryNum,provNumDoc,provHyg: provNumHyg,pattern: "//XXXXXXXX",isHygiene: true);
			//Associate a new procedure for the appointment type ProcCode to each appointment.
			ProcedureT.CreateProcedure(pat,procCode.ProcCode,ProcStat.TP,"",0,provNum: provNumDoc,aptNum: apptExtra.AptNum);
			#endregion
			//The open time slot returned should be for the Web Sched operatory on the NEXT DAY due to double booking appointments on the current day.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(dateTimeSchedule,dateTimeSchedule.AddDays(30),0,defApptType.DefNum);
			//There should be 1 time slot returned that spans from 09:00 - 09:40 for the next day.
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.AreEqual(opHyg2.OperatoryNum,listTimeSlots[0].OperatoryNum);
			Assert.AreEqual(dateTimeScheduleNextDay.Date,listTimeSlots[0].DateTimeStart.Date);
			Assert.AreEqual(new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,0,0),listTimeSlots[0].DateTimeStart);
			Assert.AreEqual(new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,40,0),listTimeSlots[0].DateTimeStop);
		}

		///<summary>Web Sched - Clinic priority.  Time slots should be found only for operatories for the patients clinic.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_ClinicPriority() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Turn clinics ON!
			Prefs.UpdateBool(PrefName.EasyNoClinics,false);//Not no clinics.
			long clinicNum1=ClinicT.CreateClinic("1 - "+suffix).ClinicNum;
			long clinicNum2=ClinicT.CreateClinic("2 - "+suffix).ClinicNum;
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			//Create the patient and have them associated to the second clinic.
			Patient pat=PatientT.CreatePatient(suffix,provNumDoc,clinicNum2);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Def defLunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"Lunch-"+suffix,itemColor:System.Drawing.Color.Azure);
			//Create a psudo prophy recall type that lasts 40 mins and has an interval of every 6 months and 1 day.
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330","//X/",new Interval(1,0,6,0));
			//Create a recall for our patient.
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeSchedule,new Interval(1,0,6,0));
			//Create operatories for the providers but make the each op assigned to a different clinic.  Hyg will be assigned to clinicNum2.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,provNumHyg,clinicNum1,isWebSched:true,itemOrder:0);
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,clinicNum2,isWebSched:true,itemOrder:1);
			//Create a schedule for the doctor from 09:00 - 11:30 with a 30 min break and then back to work from 12:00 - 17:00
			Schedule schedDocMorning=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,11,30,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Create a blockout for lunch because why not.
			Schedule schedDocLunch=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,11,30,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),schedType:ScheduleType.Blockout,blockoutType:defLunchBlockout.DefNum);
			//Schedule for closing from 12:00 - 17:00
			Schedule schedDocEvening=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType:ScheduleType.Provider,provNum: provNumDoc);
			//Now link up the schedule entries to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocMorning.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocLunch.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocEvening.ScheduleNum);
			//Create a crazy schedule for the clinicNum2 operatory which should be the time sltos that get returned.
			//02:00 - 04:00 with a 15 hour break and then back to work from 19:00 - 23:20
			Schedule schedDocMorning2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,2,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Create a European length lunch.
			Schedule schedDocLunch2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks),schedType:ScheduleType.Blockout,blockoutType:defLunchBlockout.DefNum);
			//Schedule for closing from 19:00 - 23:20
			Schedule schedDocEvening2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,23,20,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Link the crazy schedule up to the non-Web Sched operatory.
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocMorning2.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocLunch2.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocEvening2.ScheduleNum);
			//The open time slots returned should all return for the Web Sched operatory and not the other one.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,dateTimeSchedule,dateTimeSchedule.AddDays(30));
			//There should be 10 time slots returned that span from 09:00 - 16:40.
			//The 11:00 - 12:00 hour should be open (can't fit 40 min appt over lunch break).
			if(listTimeSlots.Count!=9) {
				throw new Exception("Incorrect number of time slots returned.  Expected 9, received "+listTimeSlots.Count+".");
			}
			if(listTimeSlots.Any(x => x.OperatoryNum!=opHyg.OperatoryNum)) {
				throw new Exception("Invalid operatory time slot returned.  Expected all time slots to be in operatory #"+opHyg.OperatoryNum);
			}
			//Just check 4 specific time slots.  Don't worry about checking all of them cause that is a little overkill.
			//First slot
			Assert.IsFalse(listTimeSlots[0].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[0].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,2,0,0)
				|| listTimeSlots[0].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,2,40,0)
				|| listTimeSlots[0].OperatoryNum!=opHyg.OperatoryNum);
			//Slot @ 03:20 - 04:00
			Assert.IsFalse(listTimeSlots[2].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[2].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,3,20,0)
				|| listTimeSlots[2].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,4,0,0)
				|| listTimeSlots[2].OperatoryNum!=opHyg.OperatoryNum);
			//Slot @ 19:00 - 19:40
			Assert.IsFalse(listTimeSlots[3].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[3].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,19,0,0)
				|| listTimeSlots[3].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,19,40,0)
				|| listTimeSlots[3].OperatoryNum!=opHyg.OperatoryNum);
			//Last slot.
			Assert.IsFalse(listTimeSlots[8].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[8].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,22,20,0)
				|| listTimeSlots[8].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,23,0,0)
				|| listTimeSlots[8].OperatoryNum!=opHyg.OperatoryNum);
		}

		///<summary>Web Sched - Overflow. Multiple Web Sched operatories should flow into eachother nicely meaning that if the 9 - 10 slot is taken
		///in the first operatory, then the next operatory in line should show the 9 - 10 slot as open.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_Overflow() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			DateTime dateTimeScheduleNextDay=dateTimeSchedule.AddDays(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			Patient pat=PatientT.CreatePatient(suffix,provNumHyg);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Def defLunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"Lunch-"+suffix,itemColor:System.Drawing.Color.Azure);
			//Create a psudo prophy recall type that lasts 40 mins and has an interval of every 6 months and 1 day.
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330","//X/",new Interval(1,0,6,0));
			//Create a recall for our patient.
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeSchedule,new Interval(1,0,6,0));
			//Create operatories for the providers.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,provNumHyg,isWebSched:true,itemOrder:0);
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,isWebSched:true,itemOrder:1,isHygiene:true);
			//Create two schedules for the doctor from 09:00 - 10:00 on two different days.
			Schedule schedDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			Schedule schedDocNextDay=ScheduleT.CreateSchedule(dateTimeScheduleNextDay,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Now link up the schedule entries to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocNextDay.ScheduleNum);
			//Create the same schedule for the other provider but only for the first day.
			Schedule schedHyg=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			//Link the crazy schedule up to the non-Web Sched operatory.
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedHyg.ScheduleNum);
			//Create two appointments for both of the operatories during the scheduled times on the first day.
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opDoc.OperatoryNum,provNumDoc,pattern: "//XXXX//");
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opHyg.OperatoryNum,provNumDoc,provNumHyg,pattern: "//XXXX//",isHygiene: true);
			//The open time slot returned should be for the Web Sched operatory on the NEXT DAY due to appointments blocking the current day.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,dateTimeSchedule,dateTimeSchedule.AddDays(30));
			//There should be 1 time slot returned that spans from 09:00 - 09:40.
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.IsFalse(listTimeSlots.Any(x => x.OperatoryNum!=opDoc.OperatoryNum));
			Assert.IsFalse(listTimeSlots[0].DateTimeStart.Date!=dateTimeScheduleNextDay.Date
				|| listTimeSlots[0].DateTimeStart!=new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,0,0)
				|| listTimeSlots[0].DateTimeStop!=new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,40,0)
				|| listTimeSlots[0].OperatoryNum!=opDoc.OperatoryNum);
		}

		///<summary>Web Sched - Double Booking.  Providers should not be overwhelmed.  An open time slot does not mean that the slot should always be 
		///returned to the patient as available.  If the provider is double booked, the time slot should not be offered as a choice.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_DoubleBooking() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			Prefs.UpdateBool(PrefName.WebSchedRecallDoubleBooking,true);
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			DateTime dateTimeScheduleNextDay=dateTimeSchedule.AddDays(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			Patient pat=PatientT.CreatePatient(suffix,provNumHyg);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Def defLunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"Lunch-"+suffix,itemColor:System.Drawing.Color.Azure);
			//Create a psudo prophy recall type that lasts 40 mins and has an interval of every 6 months and 1 day.
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330","//X/",new Interval(1,0,6,0));
			//Create a recall for our patient.
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeSchedule,new Interval(1,0,6,0));
			//Create operatories for the providers.
			//Firt op will ONLY have provNumDoc associated, NOT the hygienist because we want to keep it simple with one provider to consider.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,isWebSched:true,itemOrder:2);
			//Now for two non-Web Sched ops that the provNumDoc will be double booked for.  Hyg provs on these ops doesn't affect anything.
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,itemOrder:1,isHygiene:true);
			Operatory opExtra=OperatoryT.CreateOperatory("3-"+suffix,"Extra Op - "+suffix,provNumDoc,provNumHyg,itemOrder:0);
			//Create two schedules for the doctor from 09:00 - 10:00 on two different days.
			Schedule schedDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			Schedule schedDocNextDay=ScheduleT.CreateSchedule(dateTimeScheduleNextDay,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Now link up the schedule entries to all of the operatories
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocNextDay.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocNextDay.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opExtra.OperatoryNum,schedDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opExtra.OperatoryNum,schedDocNextDay.ScheduleNum);
			//Create two appointments for the two non-Web Sched operatories during the scheduled times on the first day.
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opHyg.OperatoryNum,provNumDoc,pattern: "//XXXXXXXX");
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				,opExtra.OperatoryNum,provNumDoc,pattern: "//XXXXXXXX");
			//The open time slot returned should be for the Web Sched operatory on the NEXT DAY due to double booking appointments on the current day.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,dateTimeSchedule,dateTimeSchedule.AddDays(30));
			//There should be 1 time slot returned that spans from 09:00 - 09:40.
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.IsFalse(listTimeSlots.Any(x => x.OperatoryNum!=opDoc.OperatoryNum));
			//First slot.
			Assert.IsFalse(listTimeSlots[0].DateTimeStart.Date!=dateTimeScheduleNextDay.Date
				|| listTimeSlots[0].DateTimeStart!=new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,0,0)
				|| listTimeSlots[0].DateTimeStop!=new DateTime(dateTimeScheduleNextDay.Year,dateTimeScheduleNextDay.Month,dateTimeScheduleNextDay.Day,9,40,0)
				|| listTimeSlots[0].OperatoryNum!=opDoc.OperatoryNum);
		}

		///<summary>Web Sched - Basic time slot finding.  No complex scenarios, just making sure small offices find open slots.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_Basic() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			Patient pat=PatientT.CreatePatient(suffix,provNumDoc);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Def defLunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"Lunch-"+suffix,itemColor:System.Drawing.Color.Azure);
			//Create a psudo prophy recall type that lasts 40 mins and has an interval of every 6 months and 1 day.
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330","//X/",new Interval(1,0,6,0));
			//Create a recall for our patient.
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeSchedule,new Interval(1,0,6,0));
			//Create operatories for the providers but make the Hygiene op NON-WEB SCHED.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,provNumHyg,isWebSched:true,itemOrder:0);
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,itemOrder:1,isHygiene:true);
			//Create a schedule for the doctor from 09:00 - 11:30 with a 30 min break and then back to work from 12:00 - 17:00
			Schedule schedDocMorning=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,11,30,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Create a blockout for lunch because why not.
			Schedule schedDocLunch=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,11,30,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),schedType:ScheduleType.Blockout,blockoutType:defLunchBlockout.DefNum);
			//Schedule for closing from 12:00 - 17:00
			Schedule schedDocEvening=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType:ScheduleType.Provider,provNum: provNumDoc);
			//Now link up the schedule entries to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocMorning.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocLunch.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocEvening.ScheduleNum);
			//Create a crazy schedule for the non-Web Sched operatory which should not return ANY of the available time slots for that op.
			//02:00 - 04:00 with a 15 hour break and then back to work from 19:00 - 23:20
			Schedule schedDocMorning2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,2,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Create a European length lunch.
			Schedule schedDocLunch2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks),schedType:ScheduleType.Blockout,blockoutType:defLunchBlockout.DefNum);
			//Schedule for closing from 19:00 - 23:20
			Schedule schedDocEvening2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,23,20,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Link the crazy schedule up to the non-Web Sched operatory.
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocMorning2.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocLunch2.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocEvening2.ScheduleNum);
			//The open time slots returned should all return for the Web Sched operatory and not the other one.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,dateTimeSchedule,dateTimeSchedule.AddDays(30));
			//There should be 10 time slots returned that span from 09:00 - 16:40.
			//The 11:00 - 12:00 hour should be open (can't fit 40 min appt over lunch break).
			if(listTimeSlots.Count!=10) {
				throw new Exception("Incorrect number of time slots returned.  Expected 10, received "+listTimeSlots.Count+".");
			}
			if(listTimeSlots.Any(x => x.OperatoryNum!=opDoc.OperatoryNum)) {
				throw new Exception("Invalid operatory time slot returned.  Expected all time slots to be in operatory #"+opDoc.OperatoryNum);
			}
			//Just check 4 specific time slots.  Don't worry about checking all of them cause that is a little overkill.
			//First slot.
			Assert.IsFalse(listTimeSlots[0].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[0].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0)
				|| listTimeSlots[0].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,40,0)
				|| listTimeSlots[0].OperatoryNum!=opDoc.OperatoryNum);
			//Slot @ 10:20 - 11:00
			Assert.IsFalse(listTimeSlots[2].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[2].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,10,20,0)
				|| listTimeSlots[2].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,11,0,0)
				|| listTimeSlots[2].OperatoryNum!=opDoc.OperatoryNum);
			//Slot @ 12:00 - 12:40
			Assert.IsFalse(listTimeSlots[3].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[3].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,12,0,0)
				|| listTimeSlots[3].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,12,40,0)
				|| listTimeSlots[3].OperatoryNum!=opDoc.OperatoryNum);
			//Last slot.
			Assert.IsFalse(listTimeSlots[9].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[9].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,16,0,0)
				|| listTimeSlots[9].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,16,40,0)
				|| listTimeSlots[9].OperatoryNum!=opDoc.OperatoryNum);
		}

		///<summary>Web Sched - Operatory priority.  Time slots should be scheduled based on operatory item order (left to right on sched).</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_OperatoryPriority() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			Patient pat=PatientT.CreatePatient(suffix,provNumHyg);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Def defLunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"Lunch-"+suffix,itemColor:System.Drawing.Color.Azure);
			//Create a psudo prophy recall type that lasts 40 mins and has an interval of every 6 months and 1 day.
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330","//X/",new Interval(1,0,6,0));
			//Create a recall for our patient.
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeSchedule,new Interval(1,0,6,0));
			//Create operatories for the providers but make the Hygiene operatory show up FIRST (item order = 0) before the doc's op.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,provNumHyg,isWebSched:true,itemOrder:1);
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,isWebSched:true,itemOrder:0,isHygiene:true);
			//Create a schedule for the doctor from 09:00 - 10:00
			Schedule schedDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Now link up the schedule entries to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDoc.ScheduleNum);
			//Create the exact same schedule for the other provider.
			Schedule schedHyg=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,10,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			//Link the crazy schedule up to the non-Web Sched operatory.
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedHyg.ScheduleNum);
			//The first open time slot returned should be for the first Web Sched operatory and not the second one due to item order.
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,dateTimeSchedule,dateTimeSchedule.AddDays(30));
			//Any time slot returned should span from 09:00 - 09:40.
			Assert.AreEqual(1,listTimeSlots.DistinctBy(x => x.DateTimeStart).Count());
			//First slot.
			Assert.AreEqual(dateTimeSchedule.Date,listTimeSlots[0].DateTimeStart.Date);
			Assert.AreEqual(new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0),listTimeSlots[0].DateTimeStart);
			Assert.AreEqual(new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,40,0),listTimeSlots[0].DateTimeStop);
			Assert.AreEqual(opHyg.OperatoryNum,listTimeSlots[0].OperatoryNum);
		}

		///<summary>Web Sched - Appointments should be able to start at their earliest available times.
		///The first phase of Web Sched only allowed users to schedule appointments on top of the hour, one per hour.
		///Phase two logic would go through the schedule one time increment at a time until it found a slot big enough to fit the appointment.
		///Once a slot big enough was found it would check for double booking.  If there was double booking then it would continue from the end of the apt.
		///This unit test is for the logic that is applied in phase three where the opening logic should not continue from the end of the apt but instead
		///should scoot the entire appointment along the schedule one time increment at a time until a suitable slot is found.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_EarliestTime() {
			//E.g. 10 min increments, provider scheduled 8 - 9, provider scheduled in another operatory from 8:00 - 8:20 and then from 8:30 - 8:50.
			//A Web Sched appointment that is 30 minutes long (//XX//) should be able to schedule an appointment @ 8:10 - 8:40
			//Old logic would not find this appointment slot because it would find 8:00 - 8:30 open but then would find a double booking collision.
			//It would then continue looking for available openings from 8:30 onward.  We need it to instead scoot one time increment (8:10 - 8:40).
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			Patient pat=PatientT.CreatePatient(suffix,provNumHyg);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			//Create a psudo prophy recall type that lasts 30 mins with prov time in the middle and has an interval of every 6 months and 1 day.
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330","/X/",new Interval(1,0,6,0));
			//Create a recall for our patient.
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,dateTimeSchedule,new Interval(1,0,6,0));
			//Create operatories for the providers.
			//Firt op will ONLY have provNumDoc associated, NOT the hygienist because we want to keep it simple with one provider to consider.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,isWebSched:true,itemOrder:1);
			//Now for an extra non-Web Sched op that the provNumDoc will be double booked for.  Hyg provs on this op doesn't affect anything.
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,itemOrder:2,isHygiene:true);
			//Create one schedule for the doctor from 08:00 - 09:00.
			Schedule schedOpDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Now link up the schedule entries to all of the operatories
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedOpDoc.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedOpDoc.ScheduleNum);
			//Create an appointment that will leave an opening for the doctor @8:30 (middle of the appointment) in our Web Sched operatory.
			//The appointment that we are about to create needs to be in the extra operatory so that Web Sched is able to find the valid opening.
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,8,0,0)
				,opHyg.OperatoryNum,provNumDoc,pattern: "XXXX//XXXX//");
			List<TimeSlot> listTimeSlots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,dateTimeSchedule,dateTimeSchedule.AddDays(30));
			//There should be exactly one time slot returned that spans from 08:10 - 08:30.
			Assert.AreEqual(1,listTimeSlots.Count);
			Assert.IsFalse(listTimeSlots.Any(x => x.OperatoryNum!=opDoc.OperatoryNum));
			//First slot.
			Assert.IsFalse(listTimeSlots[0].DateTimeStart.Date!=dateTimeSchedule.Date
				|| listTimeSlots[0].DateTimeStart!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,8,10,0)
				|| listTimeSlots[0].DateTimeStop!=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,8,40,0)
				|| listTimeSlots[0].OperatoryNum!=opDoc.OperatoryNum);
		}

		///<summary>All of the Web Sched applications have gone through lots of phases in regards to how many days / months are considered.
		///This unit test is strictly here to make sure that time slots outside of the date range provided are NOT returned.</summary>
		[TestMethod]
		public void TimeSlots_GetTimeSlotsForRange_DateRange() {
			string suffix="TimeSlots_GetTimeSlotsForRange_DateRange";
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix,isSecondary:true);
			Patient pat=PatientT.CreatePatient(suffix,provNumDoc);
			//Make sure the that Appointment View time increment is set to 10 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,10);
			Def defLunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"Lunch-"+suffix,itemColor:System.Drawing.Color.Azure);
			//Create operatories for the providers but make the Hygiene op NON-WEB SCHED.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,provNumHyg,isWebSched:true,itemOrder:0);
			Operatory opHyg=OperatoryT.CreateOperatory("2-"+suffix,"Hyg Op - "+suffix,provNumDoc,provNumHyg,itemOrder:1,isHygiene:true);
			//Create a schedule for the doctor from 09:00 - 11:30 with a 30 min break and then back to work from 12:00 - 17:00
			Schedule schedDocMorning=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,11,30,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Create a blockout for lunch because why not.
			Schedule schedDocLunch=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,11,30,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks),schedType:ScheduleType.Blockout,blockoutType:defLunchBlockout.DefNum);
			//Schedule for closing from 12:00 - 17:00
			Schedule schedDocEvening=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,12,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType:ScheduleType.Provider,provNum: provNumDoc);
			//Now link up the schedule entries to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocMorning.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocLunch.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocEvening.ScheduleNum);
			//Create a crazy schedule for the non-Web Sched operatory which should not return ANY of the available time slots for that op.
			//02:00 - 04:00 with a 15 hour break and then back to work from 19:00 - 23:20
			Schedule schedDocMorning2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,2,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Create a European length lunch.
			Schedule schedDocLunch2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,4,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks),schedType:ScheduleType.Blockout,blockoutType:defLunchBlockout.DefNum);
			//Schedule for closing from 19:00 - 23:20
			Schedule schedDocEvening2=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,23,20,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Link the crazy schedule up to the non-Web Sched operatory.
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocMorning2.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocLunch2.ScheduleNum);
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDocEvening2.ScheduleNum);
			//Create some simple schedules that will fall outside of our date range (before and after).
			DateTime dateTimeScheduleBefore=dateTimeSchedule.AddMonths(-1);
			Schedule schedDocMonthBefore=ScheduleT.CreateSchedule(dateTimeScheduleBefore,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocMonthBefore.ScheduleNum);
			//Now for the schedule that falls after.
			DateTime dateTimeScheduleAfter=dateTimeSchedule.AddMonths(1);
			Schedule schedDocMonthAfter=ScheduleT.CreateSchedule(dateTimeScheduleAfter,new TimeSpan(new DateTime(1,1,1,9,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,17,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDocMonthAfter.ScheduleNum);
			//Make the date range for the entire month that we landed on.
			DateTime dateStart=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,1);
			DateTime dateEnd=dateStart.AddMonths(1).AddDays(-1);//Will always return the last day of the month.
			//Refresh all of the schedules that we just created above so that their non db columns are filled correctly.
			List<Schedule> listSchedules=Schedules.GetByScheduleNum(new List<long>() {
				schedDocMorning.ScheduleNum,
				schedDocMorning2.ScheduleNum,
				schedDocLunch.ScheduleNum,
				schedDocLunch2.ScheduleNum,
				schedDocEvening.ScheduleNum,
				schedDocEvening2.ScheduleNum,
				schedDocMonthBefore.ScheduleNum,
				schedDocMonthAfter.ScheduleNum,
			});
			List<TimeSlot> listTimeSlots=TimeSlots.GetTimeSlotsForRange(dateStart,dateEnd
				,"/X/" //The time pattern is not very important here (other than being short enough to actually return at least one time slot).
				,new List<long>() { provNumDoc,provNumHyg }
				,new List<Operatory>() { opDoc,opHyg }
				,listSchedules
				,null);//Null clinic will only consider ops with ClinicNum set to 0.
			//There should not be ANY time slots returned that fall outside of our start and end date ranges.
			Assert.IsTrue(listTimeSlots.All(x => x.DateTimeStart.Date.Between(dateStart,dateEnd)));
		}

		///<summary>Web Sched applications were written with the intent that they were the only entity scheduling appointments within the operatory.
		///This unit test is for making sure that our time slot finding logic always returns expected results when a manually scheduled appointment
		///finds its way into the Web Sched operatory.</summary>
		[TestMethod]
		public void TimeSlots_GetTimeSlotsForRange_ManualAppointments() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create a date that will always be in the future.  This date will be used for schedules and recalls.
			DateTime dateTimeSchedule=DateTime.Now.AddYears(1);
			long provNumDoc=ProviderT.CreateProvider("Doc-"+suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNumDoc);
			//Make sure the that Appointment View time increment is set to 5 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,5);
			//Create operatories for the provider.
			Operatory opDoc=OperatoryT.CreateOperatory("1-"+suffix,"Doc Op - "+suffix,provNumDoc,isWebSched:true,itemOrder:0);
			//Create a schedule for the doctor from 08:00 - 18:00
			Schedule schedDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,18,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumDoc);
			//Now link up the schedule entry to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opDoc.OperatoryNum,schedDoc.ScheduleNum);
			//Make the date range for the entire month that we landed on.
			DateTime dateStart=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,1);
			DateTime dateEnd=dateStart.AddMonths(1).AddDays(-1);//Will always return the last day of the month.
			//Refresh the schedule that we created above so that the non db columns are filled correctly.
			List<Schedule> listSchedules=Schedules.GetByScheduleNum(new List<long>() {
				schedDoc.ScheduleNum,
			});
			//Create two appointments that will simulate the office manually scheduling appointments within the Web Sched operatory.
			//15 min long appointment from 09:55 - 10:10
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,55,0)
				,opDoc.OperatoryNum,provNumDoc,pattern:"/X/");
			//20 min long appointment from 14:45 - 15:05
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,14,45,0)
				,opDoc.OperatoryNum,provNumDoc,pattern: "/XX/");
			//Search for hour long time slot openings.
			List<TimeSlot> listTimeSlots=TimeSlots.GetTimeSlotsForRange(dateStart,dateEnd
				,"/XXXXXXXXXX/"//60 min appt
				,new List<long>() { provNumDoc }
				,new List<Operatory>() { opDoc }
				,listSchedules
				,null);//Null clinic will only consider ops with ClinicNum set to 0.
			//Make sure that the returned time slots are EXACTLY as follows:
			/*
				08:00 - 09:00
				<-- 15 min appt at 09:55 - 10:10 causes the 09:00 - 10:00 to not show and pushes the next available slot to 10:10  -->
				10:10 - 11:10
				<-- Always try and dynamically look ahead in order to bring the next available time slot back onto the hour -->
				11:00 - 12:00
				12:00 - 13:00
				13:00 - 14:00
				<--- 20 min appt at 14:45 - 15:05 causes this big gap because we do not dynamically look behind, always dynamically looking ahead -->
				15:05 - 16:05
				16:00 - 17:00
				17:00 - 18:00
			*/
			Assert.AreEqual(listTimeSlots.Count,8);
			//08:00 - 09:00
			Assert.IsTrue(listTimeSlots[0].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,8,0,0)
				&& listTimeSlots[0].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,9,0,0));
			//10:10 - 11:10
			Assert.IsTrue(listTimeSlots[1].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,10,10,0)
				&& listTimeSlots[1].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,11,10,0));
			//11:00 - 12:00
			Assert.IsTrue(listTimeSlots[2].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,11,0,0)
				&& listTimeSlots[2].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,12,0,0));
			//12:00 - 13:00
			Assert.IsTrue(listTimeSlots[3].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,12,0,0)
				&& listTimeSlots[3].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,13,0,0));
			//13:00 - 14:00
			Assert.IsTrue(listTimeSlots[4].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,13,0,0)
				&& listTimeSlots[4].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,14,0,0));
			//15:05 - 16:05
			Assert.IsTrue(listTimeSlots[5].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,15,5,0)
				&& listTimeSlots[5].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,16,5,0));
			//16:00 - 17:00
			Assert.IsTrue(listTimeSlots[6].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,16,0,0)
				&& listTimeSlots[6].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,17,0,0));
			//17:00 - 18:00
			Assert.IsTrue(listTimeSlots[7].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,17,0,0)
				&& listTimeSlots[7].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,18,0,0));
		}

		///<summary>The last available time slot for today is not always getting returned in a very specific setup.
		///This unit test is for helping make sure that our time slot finding logic returns expected results for same day time slots.</summary>
		[TestMethod]
		public void TimeSlots_GetTimeSlotsForRange_LastSlotSameDay() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Have the date we prefer be today.  This date will be used for schedules time slot finding.
			DateTime dateTimeSchedule=DateTime.Now;
			long provNumHyg=ProviderT.CreateProvider("Hyg-"+suffix);
			Patient pat=PatientT.CreatePatient(suffix,provNumHyg);
			//Make sure the that Appointment View time increment is set to 15 min.
			Prefs.UpdateInt(PrefName.AppointmentTimeIncrement,15);
			//Create an operatory for the provider.
			Operatory opHyg=OperatoryT.CreateOperatory("1-"+suffix,"Hyg Op - "+suffix,provHygienist:provNumHyg,isWebSched:true,itemOrder:0);
			//Create a schedule for the provider from 08:00 - 19:00
			Schedule schedDoc=ScheduleT.CreateSchedule(dateTimeSchedule,new TimeSpan(new DateTime(1,1,1,8,0,0).Ticks)
				,new TimeSpan(new DateTime(1,1,1,19,0,0).Ticks),schedType:ScheduleType.Provider,provNum:provNumHyg);
			//Now link up the schedule entry to the Web Sched operatory
			ScheduleOpT.CreateScheduleOp(opHyg.OperatoryNum,schedDoc.ScheduleNum);
			//Make the date range for the entire month that we landed on.
			DateTime dateStart=new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,1);
			DateTime dateEnd=dateStart.AddMonths(1).AddDays(-1);//Will always return the last day of the month.
			//Refresh the schedule that we created above so that the non db columns are filled correctly.
			List<Schedule> listSchedules=Schedules.GetByScheduleNum(new List<long>() {
				schedDoc.ScheduleNum,
			});
			//Create two appointments that take up the majority of the operatory except the last two hour blocks.
			//This is the specific scenario that the eServices team setup in order to duplicate the issue.
			//390 min long appointment from 08:00 - 14:30
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,8,0,0)
				,opHyg.OperatoryNum,provNumHyg,pattern: "//////////////////////////////////////////////////////////////////////////////");
			//150 min long appointment from 14:30 - 17:00
			AppointmentT.CreateAppointment(pat.PatNum,new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,14,30,0)
				,opHyg.OperatoryNum,provNumHyg,pattern: "//////////////////////////////");
			//Search for hour long time slot openings.
			List<TimeSlot> listTimeSlots=TimeSlots.GetTimeSlotsForRange(dateStart,dateEnd
				,"/XXXXXXXXXX/"//60 min appt
				,new List<long>() { provNumHyg }
				,new List<Operatory>() { opHyg }
				,listSchedules
				,null);//Null clinic will only consider ops with ClinicNum set to 0.
			//The time slots returned will depend on what time of day this unit test is actually ran.
			if(dateTimeSchedule.Hour < 17) {
				Assert.AreEqual(2,listTimeSlots.Count);
				//There should only be two time slots available and it is the last two hours of the schedule; 17:00 - 18:00 and 18:00 - 19:00.
				Assert.IsTrue(listTimeSlots[0].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,17,0,0)
					&& listTimeSlots[0].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,18,0,0));
				Assert.IsTrue(listTimeSlots[1].DateTimeStart==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,18,0,0)
					&& listTimeSlots[1].DateTimeStop==new DateTime(dateTimeSchedule.Year,dateTimeSchedule.Month,dateTimeSchedule.Day,19,0,0));
			}
			else if(dateTimeSchedule.Hour < 18) {
				//Only one time slot will be available because the engineer is working too late and needs to go home to their family.
				Assert.AreEqual(1,listTimeSlots.Count);
				//It doesn't matter what time this slot is available, just the fact that only one is available is good enough.
			}
			else {
				//There won't be any time slots for today available.  This unit test is kind of pointless to run so late in the day.
				Assert.AreEqual(0,listTimeSlots.Count);
			}
		}

		/// <summary>Tests whether the AddtimeSlotsFromSchedule logic will only look for openings within a specified set of blockouts when the user
		/// is looking for appointments that are restricted to a set of blockout types.</summary>
		[TestMethod]
		public void TimeSlots_AddTimeSlotsFromSchedule_OnlyScheduleInRestrictedToBlockouts() {
			//General Variables
			string suffix=MethodBase.GetCurrentMethod().Name;
			string timePattern="XXXXXXXXXXXX"; //1 hour time pattern
			DateTime tomorrow=DateTime.Today.AddDays(1);
			Def blockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix);
			long prov=ProviderT.CreateProvider("DOC");
			Operatory op=OperatoryT.CreateOperatory(provDentist:prov);
			//Set the blockout to only be open for one hour starting at an arbitrary point during the open schedule
			TimeSpan start=new TimeSpan(tomorrow.Hour+3,tomorrow.Minute,0);
			TimeSpan stop=new TimeSpan(tomorrow.Hour+4,tomorrow.Minute,0);
			Schedule blockoutSchedule=ScheduleT.CreateSchedule(tomorrow,start,stop,schedType:ScheduleType.Blockout,blockoutType:blockout.DefNum, 
				listOpNums:new List<long>(){ op.OperatoryNum });
			Schedule openSchedule=ScheduleT.CreateSchedule(tomorrow,new TimeSpan(tomorrow.Ticks),new TimeSpan(tomorrow.AddHours(23).Ticks),provNum:prov,
				listOpNums:new List<long>(){ op.OperatoryNum });
			List<TimeSlot> listAvailableTimeSlots=new List<TimeSlot>();
			List<Schedule> listRestrictToBlockouts=new List<Schedule>(){ blockoutSchedule };

			TimeSlots.AddTimeSlotsFromSchedule(listAvailableTimeSlots,openSchedule,op.OperatoryNum,openSchedule.StartTime,openSchedule.StopTime,
				new List<Schedule>(),new Dictionary<DateTime,List<ApptSearchProviderSchedule>>(),new List<Appointment>(),timePattern,
				listRestrictToBlockouts:listRestrictToBlockouts);
			Assert.IsTrue(listAvailableTimeSlots.Count==1 
				&& listAvailableTimeSlots[0].DateTimeStart==tomorrow.AddHours(3) 
				&& listAvailableTimeSlots[0].DateTimeStop==tomorrow.AddHours(4));
		}

		/// <summary>Test that restricted-to blockouts won't be added to the list of available timeslots if the blockout is shorter than the appointment length.</summary>
		[TestMethod]
		public void TimeSlots_AddTimeSlotsFromSchedule_DoNotAddTimeSlotsForRestrictedToBlockoutsThatAreTooShort() {
			//General Variables
			string timePattern="XXXXXXXXXXXX"; //1 hour time pattern
			DateTime tomorrow=DateTime.Today.AddDays(1);
			//Object setup
			AppointmentType apptType=AppointmentTypeT.CreateAppointmentType("",pattern:timePattern);
			Def blockout=DefT.CreateDefinition(DefCat.BlockoutTypes,"");
			long prov=ProviderT.CreateProvider("DOC");
			Operatory op=OperatoryT.CreateOperatory(provDentist:prov,isWebSched:true);
			//Set the blockout to only be open for one hour starting at an arbitrary point during the open schedule
			TimeSpan start=new TimeSpan(tomorrow.Hour+3,tomorrow.Minute,0);
			TimeSpan stop=new TimeSpan(tomorrow.Hour+3,tomorrow.Minute+15,0);
			Schedule blockoutSchedule=ScheduleT.CreateSchedule(tomorrow,start,stop,schedType:ScheduleType.Blockout,blockoutType:blockout.DefNum,listOpNums:new List<long>(){ op.OperatoryNum });
			Schedule openSchedule=ScheduleT.CreateSchedule(tomorrow,new TimeSpan(tomorrow.Ticks),new TimeSpan(tomorrow.AddHours(23).Ticks),provNum:prov,listOpNums:new List<long>(){ op.OperatoryNum });
			List<TimeSlot> listAvailableTimeSlots=new List<TimeSlot>();
			List<Schedule> listRestrictToBlockouts=new List<Schedule>(){ blockoutSchedule };

			TimeSlots.AddTimeSlotsFromSchedule(listAvailableTimeSlots,openSchedule,op.OperatoryNum,openSchedule.StartTime,openSchedule.StopTime,new List<Schedule>(),
				new Dictionary<DateTime,List<ApptSearchProviderSchedule>>(),new List<Appointment>(),timePattern,listRestrictToBlockouts: listRestrictToBlockouts);
			Assert.IsTrue(listAvailableTimeSlots.Count==0);
		}

		///<summary>(WSNPA Integration Test) Verifies that Web Sched Appt Types that are restricted to blockouts are only provided timeslots that contain those blockouts.
		///Also verifies that a non-blockout-restricted new pat appt only has timeslots generated that fall within an open schedule or on top of "generally allowed"
		///blockouts.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlotsForNewOrExistingPat_IntegrationTestForRestrictingWebSchedToBlockouts() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			string timePattern="XXXXXXXXXXXX"; //1 hour time pattern
			DateTime tomorrow=DateTime.Today.AddDays(1);
			long prov=ProviderT.CreateProvider("DOC-"+suffix);
			Operatory op=OperatoryT.CreateOperatory(provDentist:prov,opName:suffix);

			AppointmentType apptTypeA=AppointmentTypeT.CreateAppointmentType(suffix+"restricted",pattern:timePattern);
			AppointmentType apptTypeB=AppointmentTypeT.CreateAppointmentType(suffix+"allowed",pattern:timePattern);
			Def restrictedBlockoutNewPat=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"restrictedNewPat");
			Def restrictedBlockoutExisting=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"restrictedExisting");
			Def allowedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"allowed");
			Def lunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"lunch");

			//Create a WSNPA Definition and make its ApptType and Blockout associations (blockout restrictions)
			Def wsnpaDefA=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix+"a"); //RESTRICTED-TO BLOCKOUTS
			DefLinkT.CreateDefLink(wsnpaDefA.DefNum,restrictedBlockoutNewPat.DefNum,DefLinkType.BlockoutType);//This def link causes it to be restricted.
			DefLinkT.CreateDefLink(wsnpaDefA.DefNum,apptTypeA.AppointmentTypeNum,DefLinkType.AppointmentType);

			//Create a second WSNPA Definition and make its ApptType (no blockout restrictions)
			Def wsnpaDefB=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix+"b"); //GENERALLY ALLOWED BLOCKOUTS
			DefLinkT.CreateDefLink(wsnpaDefB.DefNum,apptTypeB.AppointmentTypeNum,DefLinkType.AppointmentType);
			PrefT.UpdateString(PrefName.WebSchedNewPatApptIgnoreBlockoutTypes,$"{allowedBlockout.DefNum}");//This pref causes it to be generally allowed

			//Create a WSEP Definition and make its ApptType and Blockout associations (blockout restrictions)
			Def wsepDefA=DefT.CreateDefinition(DefCat.WebSchedExistingApptTypes,suffix+"a"); //RESTRICTED-TO BLOCKOUTS
			DefLinkT.CreateDefLink(wsepDefA.DefNum,restrictedBlockoutNewPat.DefNum,DefLinkType.BlockoutType);//This def link causes it to be restricted.
			DefLinkT.CreateDefLink(wsepDefA.DefNum,apptTypeA.AppointmentTypeNum,DefLinkType.AppointmentType);

			//Create a second WSEP Definition and make its ApptType (no blockout restrictions)
			Def wsepDefB=DefT.CreateDefinition(DefCat.WebSchedExistingApptTypes,suffix+"b"); //GENERALLY ALLOWED BLOCKOUTS
			DefLinkT.CreateDefLink(wsepDefB.DefNum,apptTypeB.AppointmentTypeNum,DefLinkType.AppointmentType);
			PrefT.UpdateString(PrefName.WebSchedExistingPatIgnoreBlockoutTypes,$"{allowedBlockout.DefNum}");//This pref causes it to be generally allowed

			//Associate the new patient appointment types to the operatory so that it is a valid "new pat appt" op.
			DefLinkT.CreateDefLink(wsnpaDefA.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsnpaDefB.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsepDefA.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsepDefB.DefNum,op.OperatoryNum,DefLinkType.Operatory);

			//Put the blockouts on the schedule.
			//First blockout will be the 'restricted to' blockout
			Schedule restrictedBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(3).TimeOfDay,
				tomorrow.AddHours(4).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:restrictedBlockoutNewPat.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Second blockout will be the 'generally allowed' blockout
			Schedule allowedBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(5).TimeOfDay,
				tomorrow.AddHours(6).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:allowedBlockout.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Third blockout will be the 'lunch' blockout
			Schedule lunchBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(7).TimeOfDay,
				tomorrow.AddHours(8).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:lunchBlockout.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Create a provider schedule that is open 'all day'.
			Schedule openSchedule=ScheduleT.CreateSchedule(tomorrow,
				new TimeSpan(tomorrow.Ticks),
				new TimeSpan(tomorrow.AddHours(23).Ticks),
				schedType:ScheduleType.Provider,
				provNum:prov,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Assert that this works for WSNPA
			List<TimeSlot> listAvailableTimeSlotsNewPat=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),0,wsnpaDefA.DefNum);
			List<TimeSlot> listAvailableTimeSlotsExisting=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),0,wsepDefA.DefNum,false);
			//Assert that the only available time slot is from 3 - 4 when searching for restricted blockouts
			Assert.AreEqual(1,listAvailableTimeSlotsNewPat.Count);
			Assert.AreEqual(1,listAvailableTimeSlotsExisting.Count);
			Assert.IsTrue(listAvailableTimeSlotsNewPat.All(x => x.DateTimeStart==tomorrow.AddHours(3)
				&& x.DateTimeStop==tomorrow.AddHours(4)
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsnpaDefA.DefNum));
			Assert.IsTrue(listAvailableTimeSlotsExisting.All(x => x.DateTimeStart==tomorrow.AddHours(3)
				&& x.DateTimeStop==tomorrow.AddHours(4)
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsepDefA.DefNum));
			//Assert that this works for WSEP
			listAvailableTimeSlotsNewPat=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),0,wsnpaDefB.DefNum);
			listAvailableTimeSlotsExisting=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),0,wsepDefB.DefNum,false);
			//Assert that all day, except from 3 - 4 and 7 - 8, is available when not searching for restricted blockouts
			Assert.AreEqual(21,listAvailableTimeSlotsNewPat.Count);//Prov scheduled for 23 hours, two hour blocks are not available, thus 21 open time slots expected.
			Assert.AreEqual(21,listAvailableTimeSlotsExisting.Count);
			Assert.IsFalse(listAvailableTimeSlotsNewPat.Any(x => ((x.DateTimeStart==tomorrow.AddHours(3) && x.DateTimeStop==tomorrow.AddHours(4))
					|| (x.DateTimeStart==tomorrow.AddHours(7) && x.DateTimeStop==tomorrow.AddHours(8)))
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsnpaDefA.DefNum));
			Assert.IsFalse(listAvailableTimeSlotsExisting.Any(x => ((x.DateTimeStart==tomorrow.AddHours(3) && x.DateTimeStop==tomorrow.AddHours(4))
		|| (x.DateTimeStart==tomorrow.AddHours(7) && x.DateTimeStop==tomorrow.AddHours(8)))
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsnpaDefA.DefNum));
		}

		///<summary>(WSNPA Integration Test) Verifies that Web Sched Appt Types that are restricted to blockouts are only provided timeslots that contain those blockouts.
		///Also verifies that a non-blockout-restricted new pat appt only has timeslots generated that fall within an open schedule or on top of "generally allowed"
		///blockouts for a specific clinic.</summary>
		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlotsForNewOrExistingPat_IntegrationTestForRestrictingWebSchedToBlockouts_WithClinics() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			string timePattern="XXXXXXXXXXXX"; //1 hour time pattern
			DateTime tomorrow=DateTime.Today.AddDays(1);
			long prov=ProviderT.CreateProvider("DOC-"+suffix);
			Clinic clinic=ClinicT.CreateClinic(description:suffix);
			Operatory op=OperatoryT.CreateOperatory(provDentist:prov,opName:suffix, clinicNum:clinic.ClinicNum);

			AppointmentType apptTypeA=AppointmentTypeT.CreateAppointmentType(suffix+"restricted",pattern:timePattern);
			AppointmentType apptTypeB=AppointmentTypeT.CreateAppointmentType(suffix+"allowed",pattern:timePattern);
			Def restrictedBlockoutNewPat=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"restrictedNewPat");
			Def restrictedBlockoutExisting=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"restrictedExisting");
			Def allowedBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"allowed");
			Def lunchBlockout=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"lunch");

			//Create a WSNPA Definition and make its ApptType and Blockout associations (blockout restrictions)
			Def wsnpaDefA=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix+"a"); //RESTRICTED-TO BLOCKOUTS
			DefLinkT.CreateDefLink(wsnpaDefA.DefNum,restrictedBlockoutNewPat.DefNum,DefLinkType.BlockoutType);//This def link causes it to be restricted.
			DefLinkT.CreateDefLink(wsnpaDefA.DefNum,apptTypeA.AppointmentTypeNum,DefLinkType.AppointmentType);

			//Create a second WSNPA Definition and make its ApptType (no blockout restrictions)
			Def wsnpaDefB=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix+"b"); //GENERALLY ALLOWED BLOCKOUTS
			DefLinkT.CreateDefLink(wsnpaDefB.DefNum,apptTypeB.AppointmentTypeNum,DefLinkType.AppointmentType);
			PrefT.UpdateString(PrefName.WebSchedNewPatApptIgnoreBlockoutTypes,$"{allowedBlockout.DefNum}");//This pref causes it to be generally allowed

			//Create a WSEP Definition and make its ApptType and Blockout associations (blockout restrictions)
			Def wsepDefA=DefT.CreateDefinition(DefCat.WebSchedExistingApptTypes,suffix+"a"); //RESTRICTED-TO BLOCKOUTS
			DefLinkT.CreateDefLink(wsepDefA.DefNum,restrictedBlockoutNewPat.DefNum,DefLinkType.BlockoutType);//This def link causes it to be restricted.
			DefLinkT.CreateDefLink(wsepDefA.DefNum,apptTypeA.AppointmentTypeNum,DefLinkType.AppointmentType);

			//Create a second WSEP Definition and make its ApptType (no blockout restrictions)
			Def wsepDefB=DefT.CreateDefinition(DefCat.WebSchedExistingApptTypes,suffix+"b"); //GENERALLY ALLOWED BLOCKOUTS
			DefLinkT.CreateDefLink(wsepDefB.DefNum,apptTypeB.AppointmentTypeNum,DefLinkType.AppointmentType);
			PrefT.UpdateString(PrefName.WebSchedExistingPatIgnoreBlockoutTypes,$"{allowedBlockout.DefNum}");//This pref causes it to be generally allowed

			//Associate the new patient appointment types to the operatory so that it is a valid "new pat appt" op.
			DefLinkT.CreateDefLink(wsnpaDefA.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsnpaDefB.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsepDefA.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			DefLinkT.CreateDefLink(wsepDefB.DefNum,op.OperatoryNum,DefLinkType.Operatory);

			//Put the blockouts on the schedule.
			//First blockout will be the 'restricted to' blockout
			Schedule restrictedBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(3).TimeOfDay,
				tomorrow.AddHours(4).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:restrictedBlockoutNewPat.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Second blockout will be the 'generally allowed' blockout
			Schedule allowedBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(5).TimeOfDay,
				tomorrow.AddHours(6).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:allowedBlockout.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Third blockout will be the 'lunch' blockout
			Schedule lunchBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(7).TimeOfDay,
				tomorrow.AddHours(8).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:lunchBlockout.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Create a provider schedule that is open 'all day'.
			Schedule openSchedule=ScheduleT.CreateSchedule(tomorrow,
				new TimeSpan(tomorrow.Ticks),
				new TimeSpan(tomorrow.AddHours(23).Ticks),
				schedType:ScheduleType.Provider,
				provNum:prov,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Assert that this works for WSNPA
			List<TimeSlot> listAvailableTimeSlotsNewPat=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),clinic.ClinicNum,wsnpaDefA.DefNum);
			List<TimeSlot> listAvailableTimeSlotsExisting=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),clinic.ClinicNum,wsepDefA.DefNum,false);
			//Assert that the only available time slot is from 3 - 4 when searching for restricted blockouts
			Assert.AreEqual(1,listAvailableTimeSlotsNewPat.Count);
			Assert.AreEqual(1,listAvailableTimeSlotsExisting.Count);
			Assert.IsTrue(listAvailableTimeSlotsNewPat.All(x => x.DateTimeStart==tomorrow.AddHours(3)
				&& x.DateTimeStop==tomorrow.AddHours(4)
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsnpaDefA.DefNum));
			Assert.IsTrue(listAvailableTimeSlotsExisting.All(x => x.DateTimeStart==tomorrow.AddHours(3)
				&& x.DateTimeStop==tomorrow.AddHours(4)
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsepDefA.DefNum));
			//Assert that this works for WSEP
			listAvailableTimeSlotsNewPat=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),clinic.ClinicNum,wsnpaDefB.DefNum);
			listAvailableTimeSlotsExisting=TimeSlots.GetAvailableWebSchedTimeSlotsForNewOrExistingPat(DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),clinic.ClinicNum,wsepDefB.DefNum,false);
			//Assert that all day, except from 3 - 4 and 7 - 8, is available when not searching for restricted blockouts
			Assert.AreEqual(21,listAvailableTimeSlotsNewPat.Count);//Prov scheduled for 23 hours, two hour blocks are not available, thus 21 open time slots expected.
			Assert.AreEqual(21,listAvailableTimeSlotsExisting.Count);
			Assert.IsFalse(listAvailableTimeSlotsNewPat.Any(x => ((x.DateTimeStart==tomorrow.AddHours(3) && x.DateTimeStop==tomorrow.AddHours(4))
					|| (x.DateTimeStart==tomorrow.AddHours(7) && x.DateTimeStop==tomorrow.AddHours(8)))
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsnpaDefA.DefNum));
			Assert.IsFalse(listAvailableTimeSlotsExisting.Any(x => ((x.DateTimeStart==tomorrow.AddHours(3) && x.DateTimeStop==tomorrow.AddHours(4))
		|| (x.DateTimeStart==tomorrow.AddHours(7) && x.DateTimeStop==tomorrow.AddHours(8)))
				&& x.OperatoryNum==op.OperatoryNum
				&& x.ProvNum==prov
				&& x.DefNumApptType==wsnpaDefA.DefNum));
		}

		[TestMethod]
		public void TimeSlots_GetAvailableWebSchedTimeSlots_BlockScheduling_WebSchedRecallCanScheduleOverWSNPARestrictedToBlockouts() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			PrefT.UpdateInt(PrefName.AppointmentTimeIncrement,5); //Force 5-minute time pattern for this test
			string timePattern="XXXXXXXXXXXX"; //1 hour time pattern
			DateTime tomorrow=DateTime.Today.AddDays(1);
			long prov=ProviderT.CreateProvider("DOC-"+suffix);
			Clinic clinic=ClinicT.CreateClinic(description:suffix);
			Operatory op=OperatoryT.CreateOperatory(provDentist:prov,opName:suffix, clinicNum:clinic.ClinicNum,isWebSched:true);
			Patient pat=PatientT.CreatePatient(suffix,prov,clinic.ClinicNum,birthDate: new DateTime(1950,1,1));
			RecallType recallType=RecallTypeT.CreateRecallType("Prophy-"+suffix,"D1110,D1330",timePattern,new Interval(1,0,6,0));
			Recall recall=RecallT.CreateRecall(pat.PatNum,recallType.RecallTypeNum,DateTime.Today.AddDays(-1),new Interval(1,0,6,0));

			AppointmentType apptTypeA=AppointmentTypeT.CreateAppointmentType(suffix+"restricted",pattern:timePattern);
			Def restrictedBlockoutNewPat=DefT.CreateDefinition(DefCat.BlockoutTypes,suffix+"restrictedNewPat");

			//Create a WSNPA Definition and make its ApptType and Blockout associations (blockout restrictions)
			Def wsnpaDef=DefT.CreateDefinition(DefCat.WebSchedNewPatApptTypes,suffix+"a"); //WSNPA RESTRICTED-TO BLOCKOUT
			DefLinkT.CreateDefLink(wsnpaDef.DefNum,restrictedBlockoutNewPat.DefNum,DefLinkType.BlockoutType);//This def link causes it to be restricted.
			DefLinkT.CreateDefLink(wsnpaDef.DefNum,apptTypeA.AppointmentTypeNum,DefLinkType.AppointmentType);

			//Associate the new patient appointment types to the operatory so that it is a valid "new pat appt" op.
			DefLinkT.CreateDefLink(wsnpaDef.DefNum,op.OperatoryNum,DefLinkType.Operatory);
			//Add the WSNPA Restricted-To blockout to the Web Sched Recall list of Allowed blockouts
			PrefT.UpdateString(PrefName.WebSchedRecallIgnoreBlockoutTypes,restrictedBlockoutNewPat.DefNum.ToString());

			//Put the blockouts on the schedule.
			//First blockout will be the 'restricted to' blockout
			Schedule restrictedBlockoutSched=ScheduleT.CreateSchedule(tomorrow,
				tomorrow.AddHours(3).TimeOfDay,
				tomorrow.AddHours(4).TimeOfDay,
				schedType:ScheduleType.Blockout,
				blockoutType:restrictedBlockoutNewPat.DefNum,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Create a provider schedule that is open 'all day'.
			Schedule openSchedule=ScheduleT.CreateSchedule(tomorrow,
				new TimeSpan(tomorrow.AddHours(3).Ticks),
				new TimeSpan(tomorrow.AddHours(4).Ticks),
				schedType:ScheduleType.Provider,
				provNum:prov,
				listOpNums:new List<long>(){ op.OperatoryNum });
			//Assert that Web Sched Recall shows a timeslot for tomorrow during the blockout hours
			List<TimeSlot> listAvailableWebSchedTimeslots=TimeSlots.GetAvailableWebSchedTimeSlots(recall.RecallNum,tomorrow,tomorrow.AddDays(1));
			Assert.AreEqual(1,listAvailableWebSchedTimeslots.Count);
			//Assert that Web Sched Recall was able to schedule over the top of a Web Sched New Pat Appt Restricted-To blockout
			Assert.IsTrue(listAvailableWebSchedTimeslots.All(x => 
				x.DateTimeStart==tomorrow.AddHours(3) 
				&& x.DateTimeStop==tomorrow.AddHours(4)));
		}
	}
}
