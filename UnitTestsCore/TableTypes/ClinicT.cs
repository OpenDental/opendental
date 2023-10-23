using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ClinicT {

		///<summary>Inserts the new clinic, refreshes the cache and then returns the clinic.</summary>
		public static Clinic CreateClinic(string description="",long emailAddressNum=0,string address="",Def regionDef=null,bool isHidden=false) {
			Clinic clinic=new Clinic();
			clinic.Description=description;
			clinic.Abbr=description;
			//Texting is off by default. Use OpenDentalWebTests.TableTypes.EServiceAccountT.SetupEServiceAccount() to turn on texting for this clinic.
			clinic.SmsContractDate=DateTime.MinValue;
			clinic.EmailAddressNum=emailAddressNum;
			clinic.Address=address;
			if(address=="") {
				clinic.Address="3995 Fairview Ind Dr SE Ste 110";
			}
			clinic.City="Salem";
			clinic.State="OR";
			clinic.Zip="97302-1288";
			clinic.Phone="5033635432";
			clinic.Region=regionDef?.DefNum??0;
			clinic.IsHidden=isHidden;
			Clinics.Insert(clinic);
			if(description=="") {
				clinic.Description="Clinic "+clinic.ClinicNum.ToString();
				clinic.Abbr="Clinic "+clinic.ClinicNum.ToString();
				Clinics.Update(clinic);
			}
			Clinics.RefreshCache();
			return clinic;
		}

		///<summary>Returns the practice as clinic zero.</summary>
		public static Clinic CreatePracticeClinic(string practiceTitle="The Land of Mordor",long emailAddressNum=0) {
			if(emailAddressNum!=0) {
				Prefs.UpdateLong(PrefName.EmailDefaultAddressNum,emailAddressNum);
			}
			Prefs.UpdateString(PrefName.PracticeTitle,practiceTitle);
			return Clinics.GetPracticeAsClinicZero();
		}

		///<summary>Deletes everything from the clinic table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearClinicTable() {
			string command="DELETE FROM clinic WHERE ClinicNum > 0";
			DataCore.NonQ(command);
			Clinics.RefreshCache();
		}

		/// <summary>Returns a list of three fake clinics for testing. Does not insert into testing DB</summary>
		public static List<Clinic> CreatClinicList() {
			List<Clinic> listClinics=new List<Clinic>();
			Clinic clinicOne=new Clinic();
			clinicOne.Description="France Regional";
			clinicOne.Abbr=clinicOne.Description;
			//Texting is off by default. Use OpenDentalWebTests.TableTypes.EServiceAccountT.SetupEServiceAccount() to turn on texting for this clinic.
			clinicOne.SmsContractDate=DateTime.MinValue;
			clinicOne.EmailAddressNum=1;
			clinicOne.Address="75004 Boulevard Garibaldi";
			clinicOne.City="Paris";
			clinicOne.State="Île-de-France";
			clinicOne.Phone="100555555";
			clinicOne.Region=0;
			Clinic clinicTwo= new Clinic();
			clinicTwo.Description="Mexico Regional";
			clinicTwo.Abbr=clinicTwo.Description;
			clinicTwo.SmsContractDate=DateTime.MinValue;
			clinicTwo.EmailAddressNum=2;
			clinicTwo.Address="01000 San Angel";
			clinicTwo.City="Mexico City";
			clinicTwo.State="Ciudad de Mexico";
			clinicTwo.Phone="1003555520004";
			clinicTwo.Region=0;
			Clinic clinicThree=new Clinic();
			clinicThree.Description="Japan Regional";
			clinicThree.Abbr=clinicTwo.Description;
			clinicThree.SmsContractDate=DateTime.MinValue;
			clinicThree.EmailAddressNum=2;
			clinicThree.Address="113-0033 Hongo";
			clinicThree.City="Tokyo";
			clinicThree.State="Kanto";
			clinicThree.Phone="100255555555";
			listClinics.Add(clinicOne);
			listClinics.Add(clinicTwo);
			listClinics.Add(clinicThree);
			return listClinics;
		}

		/// <summary>Returns a list of three fake clinics for testing with some empty fields. Does not insert into testing DB</summary>
		public static List<Clinic> CreatClinicListEmpties() {
			List<Clinic> listClinics=new List<Clinic>();
			Clinic clinicOne=new Clinic();
			clinicOne.Description="France Regional";
			//Texting is off by default. Use OpenDentalWebTests.TableTypes.EServiceAccountT.SetupEServiceAccount() to turn on texting for this clinic.
			clinicOne.SmsContractDate=DateTime.MinValue;
			clinicOne.EmailAddressNum=1;
			clinicOne.Address="75004 Boulevard Garibaldi";
			clinicOne.City="Paris";
			clinicOne.State="Île-de-France";
			Clinic clinicTwo= new Clinic();
			clinicTwo.Description="Mexico Regional";
			clinicTwo.SmsContractDate=DateTime.MinValue;
			clinicTwo.EmailAddressNum=2;
			clinicTwo.Address="01000 San Angel";
			clinicTwo.City="Mexico City";
			clinicTwo.State="Ciudad de Mexico";
			Clinic clinicThree=new Clinic();
			clinicThree.Description="Japan Regional";
			clinicThree.SmsContractDate=DateTime.MinValue;
			clinicThree.EmailAddressNum=2;
			clinicThree.Address="113-0033 Hongo";
			clinicThree.City="Tokyo";
			clinicThree.State="Kanto";
			listClinics.Add(clinicOne);
			listClinics.Add(clinicTwo);
			listClinics.Add(clinicThree);
			return listClinics;
		}

	}
}
