using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestsCore;

namespace UnitTests.ChartModules_Tests {
	//All tests are assumed to be non dynamic unless specified. 
	[TestClass]
	public class ChartModulesTests:TestBase {

		[TestInitialize]
		private void SetupTest() {
			AppointmentT.ClearAppointmentTable();
			ProcedureT.ClearProcedureTable();
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasProcsAllPlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is also an attached appointment to this planned appointment.  The attached appointment is set to complete, and both procedures on the planned appointment are set complete as well.  The 'ProcDescript' for the planned appointment should be '(Completed) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasProcsAllPlannedApptProcsComplete() {
			Patient patient=PatientT.CreatePatient();
			Procedure procedurePlannedAppt1=ProcedureT.CreateProcedure(patient,"D0170",ProcStat.C,"1",0);
			Procedure procedurePlannedAppt2=ProcedureT.CreateProcedure(patient,"D0274",ProcStat.C,"1",0);
			List<Procedure> listProceduresPlannedAppt=new List<Procedure>{procedurePlannedAppt1,procedurePlannedAppt2 };
			DateTime dateSched=new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned=AppointmentT.CreateAppointment(patient.PatNum,dateSched,1,1,aptStatus:ApptStatus.Complete);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Completed) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasProcsSomePlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is also an attached appointment to this planned appointment.  The attached appointment is set to complete.  The D0170 procedure is set complete, but the D0274 procedure is incomplete.  The 'ProcDescript' for the planned appointment should be '(Completed) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasProcsSomePlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.C,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.TP,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Complete);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Completed) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasProcsNoPlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is also an attached appointment to this planned appointment.  The attached appointment is set to complete.  The D0170 procedure and the D0274 procedure are both incomplete.  The 'ProcDescript' for the planned appointment should be '(Completed) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasProcsNoPlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.TP,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.TP,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Complete);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Completed) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasNoProcs)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with no procedures attached.  There is also an attached appointment to this planned appointment.  The attached appointment is set to complete.  The 'ProcDescript' for the planned appointment should be '(Completed) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_CompleteApptAttachedPlannedApptHasNoProcs() {
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Complete);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"");
			Assert.AreEqual("(Completed) ",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.GetChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasProcsAllPlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is also an attached appointment to this planned appointment.  The attached appointment is still incomplete.  The D0170 procedure and the D0274 procedure are both set complete.  The 'ProcDescript' for the planned appointment should be '(Completed) ReEval, bitewings - four radiographic images'.")]
		public void GetChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasProcsAllPlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.C,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.C,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Scheduled);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Completed) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasProcsSomePlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is also an attached appointment to this planned appointment.  The attached appointment is incomplete.  The D0170 procedure is set complete, but the D0274 procedure is incomplete.  The 'ProcDescript' for the planned appointment should be '(Some Procs Complete) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasProcsSomePlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.C,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.TP,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Scheduled);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Some Procs Complete) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasProcsNoPlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is also an attached appointment to this planned appointment.  The attached appointment is incomplete.  The D0170 procedure and the D0274 procedure are both incomplete.  The 'ProcDescript' for the planned appointment should be 'ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasProcsNoPlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.TP,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.TP,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Scheduled);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasNoProcs)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with no procedures.  There is also an attached appointment to this planned appointment.  The attached appointment is incomplete.  The 'ProcDescript' for the planned appointment should be ''.")]
		public void ChartModules_GetProcDescriptPlanned_UncompleteApptAttachedPlannedApptHasNoProcs() {
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { };
			DateTime dateSched = new DateTime(2023,11,11);
			Appointment appointmentLinkToPlanned = AppointmentT.CreateAppointment(1,dateSched,1,1,aptStatus: ApptStatus.Scheduled);
			appointmentLinkToPlanned.AptNum=2;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"");
			Assert.AreEqual("",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasProcsAllPlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is no attached appointment to this planned appointment.  Both of the procedures are set complete.  The 'ProcDescript' for the planned appointment should be '(Completed) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasProcsAllPlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.C,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.C,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			Appointment appointmentLinkToPlanned = null;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Completed) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasProcsSomePlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is no attached appointment to this planned appointment.  The D0170 procedure is set complete, but the D0274 procedure is stil incomplete.  The 'ProcDescript' for the planned appointment should be '(Some Procs Complete) ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasProcsSomePlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.C,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.TP,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			Appointment appointmentLinkToPlanned = null;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("(Some Procs Complete) ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasProcsNoPlannedApptProcsComplete)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with a D0170 procedure and a D0274 procedure.  There is no attached appointment to this planned appointment.  Both of the procedures are stil incomplete.  The 'ProcDescript' for the planned appointment should be 'ReEval, bitewings - four radiographic images'.")]
		public void ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasProcsNoPlannedApptProcsComplete() {
			Patient patient = PatientT.CreatePatient();
			Procedure procedurePlannedAppt1 = ProcedureT.CreateProcedure(patient,"D0170",ProcStat.TP,"1",0);
			Procedure procedurePlannedAppt2 = ProcedureT.CreateProcedure(patient,"D0274",ProcStat.TP,"1",0);
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { procedurePlannedAppt1,procedurePlannedAppt2 };
			Appointment appointmentLinkToPlanned = null;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"ReEval, bitewings - four radiographic images");
			Assert.AreEqual("ReEval, bitewings - four radiographic images",strActual);
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasNoProcs)]
		[Documentation.VersionAdded("24.1.62")]
		[Documentation.Description("A patient has a planned appointment with no procedures.  There is no attached appointment to this planned appointment.  The 'ProcDescript' for the planned appointment should be ''.")]
		public void ChartModules_GetProcDescriptPlanned_NoApptAttachedPlannedApptHasNoProcs() {
			List<Procedure> listProceduresPlannedAppt = new List<Procedure> { };
			Appointment appointmentLinkToPlanned = null;
			string strActual=ChartModules.GetProcDescriptPlanned(appointmentLinkToPlanned,listProceduresPlannedAppt,DateTime.MinValue,"");
			Assert.AreEqual("",strActual);
		}

	}
}