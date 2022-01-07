using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Operatories_Tests {
	[TestClass]
	public class OperatoriesTests:TestBase {

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

		///<summary>Helper method that mimics the Update All button click from the Operatory Edit window.</summary>
		private void UpdateAll(Operatory op) {
			OpenDental.ControlAppt controlAppt=new OpenDental.ControlAppt() { Visible=false };
			List<Appointment> listAppts=Appointments.GetAppointmentsForOpsByPeriod(new List<long>() {op.OperatoryNum},DateTime.Now);//no end date, so all future
			List<Appointment> listApptsOld=listAppts.Select(x => x.Copy()).ToList();
			//Update the appointment 
			controlAppt.MoveAppointments(listAppts,listApptsOld,op);
		}

		[TestMethod]
		public void Operatory_UpdateAll_DifferentProviderOnAppt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//create provider,patient, and appointment
			long provNumOp=ProviderT.CreateProvider(suffix+"ProvDentTest");
			Patient pat=PatientT.CreatePatient(suffix,provNumOp);
			//create an operatory where the dentist is the only provider set.
			Operatory op=OperatoryT.CreateOperatory(provDentist:provNumOp);
			long provNumApt=ProviderT.CreateProvider(suffix+"ProvDentTestWrong");
			//Don't mark the appointment as isHygiene
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(1),op.OperatoryNum,provNumApt,aptStatus:ApptStatus.Scheduled);
			//Update The appointment 
			UpdateAll(op);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.AreEqual(apt.ProvNum,provNumOp);
		}

		[TestMethod]
		public void Operatory_UpdateAll_DifferentProviderAndHygieneOnAppt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//create provider,patient, and appointment
			long provNumOp=ProviderT.CreateProvider(suffix+"ProvDentTest");
			long provNumHyg=ProviderT.CreateProvider(suffix+"ProvHygTest");
			Patient pat=PatientT.CreatePatient(suffix,provNumOp);
			//create an operatory where the dentist and the hygienist are set.
			Operatory op=OperatoryT.CreateOperatory(provDentist:provNumOp,provHygienist:provNumHyg);
			long provNumApt=ProviderT.CreateProvider(suffix+"ProvDentTestWrong");
			//Don't mark the appointment as isHygiene
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(1),op.OperatoryNum,provNumApt,aptStatus:ApptStatus.Scheduled);
			//Update The appointment 
			UpdateAll(op);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsFalse(apt.IsHygiene);
			Assert.AreEqual(apt.ProvNum,provNumOp);
			Assert.AreEqual(apt.ProvHyg,provNumHyg);
		}

		[TestMethod]
		public void Operatory_UpdateAll_PreserveDentistOnAppt() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//create provider,patient, and appointment
			long provNumOp=ProviderT.CreateProvider(suffix+"ProvDentTest");
			long provNumHyg=ProviderT.CreateProvider(suffix+"ProvHygTest");
			Patient pat=PatientT.CreatePatient(suffix,provNumOp);
			//create an operatory where the dentist and the hygienist are set.
			Operatory op=OperatoryT.CreateOperatory(provDentist:provNumOp,provHygienist:provNumHyg,isHygiene:true);
			//Don't mark the appointment as isHygiene
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(1),op.OperatoryNum,provNumOp,provHyg:provNumHyg,aptStatus:ApptStatus.Scheduled);
			//remove the hygiene ProvNum from operatory
			op.ProvDentist=0;
			OperatoryT.Update(op);
			//Update The appointment 
			UpdateAll(op);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsTrue(apt.IsHygiene);//Because the operatory is flagged as IsHyiene
			Assert.AreEqual(apt.ProvNum,provNumOp);//It is not possible to remove the dentist from the appointment, so it should have been preserved.
			Assert.AreEqual(apt.ProvHyg,provNumHyg);
		}

		[TestMethod]
		public void Operatory_UpdateAll_RemoveHygOnOp() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//create provider,patient, and appointment
			long provNumOp=ProviderT.CreateProvider(suffix+"ProvDentTest");
			long provNumHyg=ProviderT.CreateProvider(suffix+"ProvHygTest");
			Patient pat=PatientT.CreatePatient(suffix,provNumOp);
			//create an operatory where the dentist and the hygienist are set.
			Operatory op=OperatoryT.CreateOperatory(provDentist:provNumOp,provHygienist:provNumHyg);
			//Don't mark the appointment as isHygiene
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(1),op.OperatoryNum,provNumOp,provHyg:provNumHyg,aptStatus:ApptStatus.Scheduled);
			//remove the hygiene ProvNum from operatory
			op.ProvHygienist=0;
			OperatoryT.Update(op);
			//Update The appointment 
			UpdateAll(op);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsFalse(apt.IsHygiene);
			Assert.AreEqual(apt.ProvNum,provNumOp);
			Assert.AreEqual(apt.ProvHyg,0);
		}

		[TestMethod]
		public void Operatory_UpdateAll_SameProviders_IsHygiene() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			//create provider,patient, and appointment
			long provNum=ProviderT.CreateProvider(suffix+"ProvDentTest");
			long provNumHyg=ProviderT.CreateProvider(suffix+"HygDentTest");
			Patient pat=PatientT.CreatePatient(suffix,provNum);
			//create an operatory where the dentist and the hygienist are set and is flagged as IsHygiene.
			//Set Op to be is Hygiene
			Operatory op=OperatoryT.CreateOperatory(provDentist:provNum,provHygienist:provNumHyg,isHygiene:true);
			//Don't mark the appointment as isHygiene
			Appointment apt=AppointmentT.CreateAppointment(pat.PatNum,DateTime.Now.AddDays(1),op.OperatoryNum,provNum,provHyg:provNumHyg,aptStatus:ApptStatus.Scheduled);
			//Since the operatory is flagged as IsHygiene, the appointment needs to get set as IsHygiene after performing UpdateAll even though the provs didn't change.
			UpdateAll(op);
			apt=Appointments.GetOneApt(apt.AptNum);
			Assert.IsTrue(apt.IsHygiene);
		}
	}
}
