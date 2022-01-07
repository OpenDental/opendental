using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using OpenDental;

namespace UnitTests.MassEmail_Tests {
	[TestClass]
	public class MassEmailTests:TestBase {
		//filters for form query
		private List<PatientStatus> _listPatStatus=new List<PatientStatus>();
		private bool _doExcludePatsWithFutureAppts=false;
		//int because we use 'any' in the form which does not exist as an enum. Options are Any(-1),None,and Email.
		private int	_preferredContactMethod=-1;
		private List<long> _listClinicNum=new List<long>();
		//defaults to negative one if not checked. When set, excludes patients that have received a mass email within the day range.
		private int	_numDays=-1;
		
		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//insert the following patients into the database to more efficiently test the filters in FormSendMassEmail and who gets sent an email.
			//All patients here must have an email address. Not sure if we want 1 for all for testing purposes or if it doesn't matter.
			//May want some of these with various clinics as well. Maybe can leave that up to additional tests on if they want clinics or not. 
			long provNum=ProviderT.CreateProvider("LSEmail");
			//Prospective patient with an upcoming appointment
			Patient pat=PatientT.CreatePatient(fName:"Ella",lName:"Vader",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.Prospective
				,birthDate:new DateTime(1988,5,21));
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddDays(2),0,provNum);
			//Non-Patient with no upcoming appointments 
			pat=PatientT.CreatePatient(fName:"Pepper",lName:"Tony",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.Inactive
				,birthDate:new DateTime(1968,3,20));
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddYears(-2),0,provNum);
			//Non-Patient with no previous appointments and no upcoming appointments.
			pat=PatientT.CreatePatient(fName:"Mike",lName:"Rophone",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.NonPatient
				,birthDate:new DateTime(1957,4,19));
			//Patient with both previous and upcoming
			pat=PatientT.CreatePatient(fName:"Linda",lName:"Zee",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.Patient
				,birthDate:new DateTime(1948,5,18));
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddYears(-1),0,provNum,aptStatus:ApptStatus.Complete);
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddDays(15),0,provNum);
			//Patient with a preferred contact method of email
			pat=PatientT.CreatePatient(fName:"Jerry",lName:"Attric",email:"That@this.com",contactMethod:ContactMethod.Email,patStatus:PatientStatus.Patient
				,birthDate:new DateTime(1990,6,17));
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddMonths(-6),0,provNum,aptStatus:ApptStatus.Complete);
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddMonths(2),0,provNum);
			//Patient with a preferred contact method that is not email
			pat=PatientT.CreatePatient(fName:"Ken",lName:"Tucky",email:"That@this.com",contactMethod:ContactMethod.HmPhone,patStatus:PatientStatus.Patient
				,birthDate:new DateTime(2001,7,16));
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddMonths(-5),0,provNum,aptStatus:ApptStatus.Complete);
			AppointmentT.CreateAppointment(pat.PatNum,DateTime.Today.AddMonths(3),0,provNum);	
			//Patient with no upcoming appointment (newly signed up patient).
			pat=PatientT.CreatePatient(fName:"Ana",lName:"Gram",email:"That@this.com",contactMethod:ContactMethod.Email,patStatus:PatientStatus.Patient,
				birthDate:new DateTime(1975,8,15));
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			//Set Defaults
			_listPatStatus=new List<PatientStatus>();
			_listPatStatus.Add(PatientStatus.Patient);
			_preferredContactMethod=-1;
			_doExcludePatsWithFutureAppts=false;
			_listClinicNum=new List<long>{0};
			_numDays=-1;
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			_listPatStatus=new List<PatientStatus>();
			_preferredContactMethod=-1;
			_doExcludePatsWithFutureAppts=false;
			_listClinicNum=new List<long>{0};
			_numDays=-1;
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointment_TableReturnedIsCorrectNoFilters() {
			//From patients class but to get information specifically for the mass email form
			//Patients that will be retrieved upon loading the window
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
				0,110,DateTime.MinValue,DateTime.MinValue,new List<Def>(),_preferredContactMethod,_numDays);
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.AreEqual(3,listPatInfo.FindAll(x => x.Name=="Zee, Linda" || x.Name=="Attric, Jerry" || x.Name=="Tucky, Ken").Count);
			Assert.IsFalse(listPatInfo.Exists(x => x.Name=="Vader, Ella"));
			Assert.IsFalse(listPatInfo.Exists(x => x.Name=="Tony, Pepper"));
			Assert.IsFalse(listPatInfo.Exists(x => x.Name=="Rophone, Mike"));
		}

		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointment_TableReturnedIsCorrectForNonPatientStatus() {
			//Set filters
			_listPatStatus.Remove(PatientStatus.Patient);
			_listPatStatus.Add(PatientStatus.NonPatient);
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
				0,110,DateTime.MinValue,DateTime.MinValue,new List<Def>(),_preferredContactMethod,_numDays);
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Count>0);
			Assert.IsTrue(listPatInfo.FindAll(x => x.Status!=PatientStatus.NonPatient).Count==0);
			Assert.IsTrue(listPatInfo.All(x => x.Status==PatientStatus.NonPatient));
		}

		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointment_TableReturnedIsCorrectForExcludingAppts() {
			//Set filters
			_doExcludePatsWithFutureAppts=true;
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
				0,110,DateTime.MinValue,DateTime.MinValue,new List<Def>(),_preferredContactMethod,_numDays);
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Count>0);
			Assert.IsTrue(listPatInfo.All(x => x.DateTimeNextAppt==DateTime.MinValue));
		}

		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointment_TableReturnedIsCorrectForExcludingRecentEmails() {
			//Make two additional patients who have recently been sent mass emails
			//Patient that was sent a mass email yesterday.
			long provNum=ProviderT.CreateProvider("massEmail");
			Patient pat1=PatientT.CreatePatient(fName:"Jan",lName:"Yuari",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.Patient,
				birthDate:new DateTime(1995,9,14));
			PromotionLogT.CreatePromotionLog(pat1.PatNum,DateTime.Today.AddDays(-1));
			//Patient that was sent a mass email two days ago.
			Patient pat2=PatientT.CreatePatient(fName:"Cat",lName:"Astrophy",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.Patient,
				birthDate:new DateTime(1996,10,13));
			PromotionLogT.CreatePromotionLog(pat2.PatNum,DateTime.Today.AddDays(-2));
			//Set filters
			_numDays=1;//exclude patients who received a mass email within the last day (yesterday or today)
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
				0,110,DateTime.MinValue,DateTime.MinValue,new List<Def>(),_preferredContactMethod,_numDays);
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Count>0);
			Assert.IsTrue(!listPatInfo.Exists(x => x.PatNum==pat1.PatNum));
			Assert.IsTrue(listPatInfo.Exists(x => x.PatNum==pat2.PatNum));
			Assert.IsTrue(listPatInfo.Exists(x => x.Name=="Zee, Linda"));//check to make sure a patient w/o emails is still included.
		}

		///<summary>Patient's last appointment was exactly one year ago.  Exclude patients seen since one year ago.</summary>
		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_ExcludeSeenSinceOneYear() {
			DateTime dateExcludeSeenSince=		DateTime.Today.AddYears(-1);
			DateTime dateExcludeNotSeenSince=	DateTime.MinValue;
			//Linda's last appointment was exactly one year ago
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,dateExcludeSeenSince,dateExcludeNotSeenSince,new List<Def>());
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsFalse(listPatInfo.Exists(x => x.Name=="Zee, Linda"));
		}

		///<summary>Patient's has never had an appointment.  Exclude patients seen since one year ago.</summary>
		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_ExcludeSeenSinceOneYearNewPat() {
			Patient pat=PatientT.CreatePatient(email:"email@address.com");
			DateTime dateExcludeSeenSince=		DateTime.Today.AddYears(-1);
			DateTime dateExcludeNotSeenSince=	DateTime.MinValue;
			//Linda's last appointment was exactly one year ago
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,dateExcludeSeenSince,dateExcludeNotSeenSince,new List<Def>());
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Exists(x => x.Name==$"{pat.LName}, {pat.FName}"));
		}

		///<summary>Patient's last appointment was exactly 5 months ago.  Exclude patients seen since one year minus 1 day ago.</summary>
		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_ExcludeSeenSinceOneYearMinusOneDay() {
			DateTime dateExcludeSeenSince=		DateTime.Today.AddYears(-1).AddDays(1);
			DateTime dateExcludeNotSeenSince=	DateTime.MinValue;
			//Linda's last appointment was exactly one year ago
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,dateExcludeSeenSince,dateExcludeNotSeenSince,new List<Def>());
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Exists(x => x.Name=="Zee, Linda"));
		}

		///<summary>Patient's last appointment was exactly one year ago.  Exclude patients not seen since a year ago.</summary>
		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_ExcludeNotSeenSinceOneYear() {
			DateTime dateExcludeSeenSince=		DateTime.MinValue;
			DateTime dateExcludeNotSeenSince=	DateTime.Today.AddYears(-1);//365 days ago
			//Linda's last appointment was exactly one year ago
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,dateExcludeSeenSince,dateExcludeNotSeenSince,new List<Def>());
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Exists(x => x.Name=="Zee, Linda"));
		}

		///<summary>Patient's last appointment was exactly one year ago.  Exclude patients not seen since a year minus a day ago.</summary>
		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_ExcludeNotSeenSinceOneYearMinusOneDay() {
			DateTime dateExcludeSeenSince=		DateTime.MinValue;
			DateTime dateExcludeNotSeenSince=	DateTime.Today.AddYears(-1).AddDays(1);//364 days ago
			//Linda's last appointment was exactly one year ago
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,dateExcludeSeenSince,dateExcludeNotSeenSince,new List<Def>());
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsFalse(listPatInfo.Exists(x => x.Name=="Zee, Linda"));
		}

		///<summary>Patient's last appointment was exactly one year ago.  Exclude patients seen since 1 year minus 1 day ago and not seen since a year ago.</summary>
		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_ExcludeSeenSinceOneYearOneDayExcludeNotSeenSinceOneYear() {
			DateTime dateExcludeSeenSince=		DateTime.Today.AddYears(-1).AddDays(1);//364 days
			DateTime dateExcludeNotSeenSince=	DateTime.Today.AddYears(-1);//365 days
			//Linda's last appointment was exactly one year ago
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,dateExcludeSeenSince,dateExcludeNotSeenSince,new List<Def>());
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Exists(x => x.Name=="Zee, Linda"));
		}

		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointments_TableReturnsCorrectBillingType() {
			Patient pat=PatientT.CreatePatient(fName:"Jane",lName:"Doe",email:"That@this.com",contactMethod:ContactMethod.None,patStatus:PatientStatus.Patient,
				birthDate:new DateTime(1996,10,13));
			long billingTypeDefNum=Defs.GetFirstForCategory(DefCat.BillingTypes).DefNum;
			pat.BillingType=billingTypeDefNum;
			PromotionLogT.CreatePromotionLog(pat.PatNum,DateTime.Today.AddDays(-4));
			List<Def> listAllBillinTpes=new List<Def>();
				foreach(Def def in Defs.GetDefsForCategory(DefCat.BillingTypes)) {
					listAllBillinTpes.Add(def);
				}
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
			 0,110,DateTime.MinValue,DateTime.MinValue,listAllBillinTpes);
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Count>0);
			Assert.IsTrue(listPatInfo.Exists(x => x.Name=="Doe, Jane"));
		}

		[TestMethod]
		public void Patients_GetPatientsWithFirstLastAppointment_TableReturnedIsCorrectWithNoEmail() {
			Patient pat=PatientT.CreatePatient(fName:"John",lName:"Bob",email:"",contactMethod:ContactMethod.Email,patStatus:PatientStatus.Patient,
				birthDate:new DateTime(1996,10,13));
			DataTable table=Patients.GetPatientsWithFirstLastAppointments(_listPatStatus,_doExcludePatsWithFutureAppts,_listClinicNum,
				0,110,DateTime.MinValue,DateTime.MinValue,new List<Def>(),_preferredContactMethod,_numDays);
			List<FormMassEmailList.PatientInfo> listPatInfo=FormMassEmailList.PatientInfo.GetListPatientInfos(table);
			Assert.IsTrue(listPatInfo.Count>0);
			Assert.IsFalse(listPatInfo.Exists(x => x.Name=="Bob, John"));
		}
	}
}
