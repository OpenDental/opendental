using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ClinicT {

		///<summary>Inserts the new clinic, refreshes the cache and then returns the clinic.</summary>
		public static Clinic CreateClinic(string description="",long emailAddressNum=0,string address="",Def regionDef=null) {
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
	}
}
