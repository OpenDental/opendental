using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PatientT {
		///<summary>Creates a patient.  Practice default provider and billing type.</summary>
		public static Patient CreatePatient(string suffix="",long priProvNum=0,long clinicNum=0,string email="",string phone="",
			ContactMethod contactMethod=ContactMethod.Email,string lName="",string fName="",string preferredName="",DateTime birthDate=default(DateTime)
			,long secProvNum=0,long guarantor=0,bool setPortalAccessInfo=false,PatientStatus patStatus=PatientStatus.Patient,bool hasSignedTil=false
			,bool doInsert=true,int billingCycleDay=1,string language="",string wkPhone="",string address="",string city="",
			string zip="",string state="",long superfamily=0,string hasIns="",string wirelessPhone="") 
		{
			Patient pat=new Patient {
				Email=email,
				PreferConfirmMethod=contactMethod,
				PreferContactConfidential=contactMethod,
				PreferContactMethod=contactMethod,
				PreferRecallMethod=contactMethod,
				HmPhone=phone,
				WirelessPhone=string.IsNullOrWhiteSpace(wirelessPhone) ? phone : wirelessPhone,
				WkPhone=string.IsNullOrWhiteSpace(wkPhone) ? phone : wkPhone,
				IsNew=true,
				LName=lName+suffix,
				FName=fName+suffix,
				BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType),
				ClinicNum=clinicNum,
				Preferred=preferredName,
				Birthdate=birthDate,
				SecProv=secProvNum,
				PatStatus=patStatus,
				HasSignedTil=hasSignedTil,
				BillingCycleDay=billingCycleDay,
				Language=language,
				Address=address,
				City=city,
				Zip=zip,
				State=state,
				HasIns=hasIns,
			};
			if(priProvNum!=0) {
				pat.PriProv=priProvNum;
			}
			else {
				long practiceProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
				//Check the provider cache to see if the ProvNum set for the PracticeDefaultProv pref actually exists.
				if(!Providers.GetExists(x => x.ProvNum==practiceProvNum)) {
					practiceProvNum=Providers.GetFirst().ProvNum;
					//Update the preference in the database NOT the unit test preference cache to a valid provider if the current pref value is invalid.
					Prefs.UpdateLong(PrefName.PracticeDefaultProv,practiceProvNum);
				}
				pat.PriProv=practiceProvNum;
			}
			if(setPortalAccessInfo) {
				pat.Address="666 Church St NE";
				pat.City="Salem";
				pat.State="OR";
				pat.Zip="97301";
				if(pat.Birthdate.Year<1880) {
					pat.Birthdate=new DateTime(1970,1,1);
				}
				if(string.IsNullOrEmpty(pat.WirelessPhone)) {
					pat.WirelessPhone="5555555555";
				}
			}
			if(!doInsert) {
				return pat;
			}
			Patients.Insert(pat,false);
			Patient oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			if(guarantor > 0) {
				pat.Guarantor=guarantor;
			}
			if(lName=="") {
				pat.LName=pat.PatNum.ToString()+"L";
			}
			if(fName=="") {
				pat.FName=pat.PatNum.ToString()+"F";
			}
			pat.SuperFamily=superfamily;
			Patients.Update(pat,oldPatient);
			return pat;
		}

		public static void SetGuarantor(Patient pat,long guarantorNum){
			Patient oldPatient=pat.Copy();
			pat.Guarantor=guarantorNum;
			Patients.Update(pat,oldPatient);
		}
		
		///<summary>Deletes everything from the patient table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearPatientTable() {
			string command="DELETE FROM patient WHERE PatNum > 0";
			DataCore.NonQ(command);
		}

		/// <summary>Creates patient objects corresponding to the totalPat parameter. Each patient has a procedure
		/// and statement created on the specified date. Aging is run for each patient.</summary>
		public static void CreatePatWithProcAndStatement(int totalPat, DateTime dateTimeSentStmt=default(DateTime),bool hasPortalAccessInfo=false,
			PatientStatus patStatus=PatientStatus.Patient,StatementMode stmtMode=StatementMode.Mail,bool hasSignedTil=false,double procFee=0) {
			for(int i=0;i<totalPat;i++) {
				Patient patient=CreatePatient("",0,0,"","",ContactMethod.Email,"","","",default(DateTime),0,0,hasPortalAccessInfo,patStatus,hasSignedTil);
				DateTime dateProc=DateTime.Today.AddDays(-1);
				//Create a completed procedure that was completed the day before the first payplan charge date AND before the payment plan creation date.
				ProcedureT.CreateProcedure(patient,"D1100",ProcStat.C,"",procFee,dateProc);
				//Run Ledgers to update the patient balance from the procedure fee
				Ledgers.ComputeAging(patient.PatNum,dateTimeSentStmt);
				//Insert a statement that was sent during the "bill in advance days" for the payment plan charge AND before the payment plan creation date.
				StatementT.CreateStatement(patient.PatNum,mode_: stmtMode,isSent: true,dateSent: dateTimeSentStmt);
			}
		}

		///<summary>Creates a patient object that has insurance attached.</summary>
		public static Patient CreatePatientWithInsurance(string lName="", string fName="") {
			Patient patient=CreatePatient(lName: lName, fName: fName);
			Carrier carrier=CarrierT.CreateCarrier("TrustECarrier");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(patient.PatNum,insPlan.PlanNum);
			PatPlanT.CreatePatPlan(1,patient.PatNum,insSub.InsSubNum);
			return patient;
		}

		///<summary>Creates a patient object that has a discount plan attached.</summary>
		public static Patient CreatePatientWithDiscountPlan(string lName="",string fName="") {
			Patient patient=CreatePatient(lName:lName,fName:fName);
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan("Plan for "+lName+", "+fName);
			DiscountPlanSubT.CreateDiscountPlanSub(patient.PatNum,discountPlan.DiscountPlanNum);
			return patient;
		}

		public static void InsertMany(List<Patient> listPats) {
			OpenDentBusiness.Crud.PatientCrud.InsertMany(listPats);
		}

		public static void SetGuarantorToSelf(List<long> listPatNums) {
			string command=$"UPDATE patient SET Guarantor=PatNum WHERE PatNum IN("+string.Join(",",listPatNums)+")";
			DataCore.NonQ(command);
		}


		///<summary>Creates  and returns a list of 5 patients with all fields utilized and varied. Useful for testing a patient search for different fields.  </summary>
		public static List<OpenDentBusiness.Patient> CreateVariedPatientSet() {
			List<OpenDentBusiness.Patient> listPatients=new List<OpenDentBusiness.Patient>();
			listPatients.Add(new OpenDentBusiness.Patient {
				LName="Smith",
				FName="Clara",
				HmPhone="555-554-5543",
				PatStatus=OpenDentBusiness.PatientStatus.Patient,
				Address="132 Elm St",
				City="Providence",
				State="RI",
				SSN="132567890",
				ChartNumber="145",
				Birthdate=DateTime.Today,
				Email="name@email.com",
				Country="USA",
				ClinicNum=1,
			});
			listPatients.Add(new OpenDentBusiness.Patient {
				LName="Smith",
				FName="Grant",
				HmPhone="555-554-5543",
				PatStatus=OpenDentBusiness.PatientStatus.Patient,
				Address="132 Elm St",
				City="Providence",
				State="RI",
				SSN="456790123",
				Birthdate=DateTime.Today.AddDays(10),
				Email="name2@email.com",
				Country="USA",
				ClinicNum=1,
			});
			listPatients.Add(new OpenDentBusiness.Patient {
				LName="Franklin",
				FName="Nicole",
				WkPhone="222-222-2222",
				PatStatus=OpenDentBusiness.PatientStatus.Patient,
				Address="111 Spruce Ln",
				City="Ontario",
				SSN="333333333",
				Birthdate=DateTime.Today.AddMonths(1).AddDays(10),
				Email="name3@email.com",
				SiteNum=5,
				Country="CA",
				ClinicNum=2,
			});
			listPatients.Add(new OpenDentBusiness.Patient {
				LName="Ruthers",
				FName="Lorenzo",
				HmPhone="511-511-5111",
				PatStatus=OpenDentBusiness.PatientStatus.Inactive,
				Address="6532 Maple Ct",
				City="Santa Clara",
				State="CA",
				SSN="489450123",
				Birthdate=DateTime.Today.AddMonths(2).AddDays(-10),
				Country="USA",
				ClinicNum=0,
			});
			listPatients.Add(new OpenDentBusiness.Patient {
				LName="Sage",
				FName="Summer",
				WirelessPhone="645-454-5011",
				PatStatus=OpenDentBusiness.PatientStatus.Archived,
				Address="68882 Walnut Dr",
				City="Glacene",
				State="WI",
				SSN="840332523",
				Birthdate=DateTime.Today.AddMonths(1).AddDays(5),
				Country="USA",
			});
			PatientT.InsertMany(listPatients);
			return listPatients;
		}
	}
}
