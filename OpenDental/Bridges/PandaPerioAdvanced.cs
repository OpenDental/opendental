using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using OpenDentBusiness;
using CodeBase;
using System.IO;

namespace OpenDental.Bridges {
	public class PandaPerioAdvanced {

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		/*Data like the following:
		[PATIENT]
		PatID=100011PC        <--- This is the key from the PMS that we will use to locate the Patient in Panda.
		Title=Miss
		FirstName=Janet
		LastName=Wang
		MiddleInitial=L
		NickName=
		Sex=F
		BirthDate=12/28/1950
		HomePhone=(360)897-5555
		WorkPhone=(360)748-7777
		WorkPhoneExt=
		SSN=697-12-8888      
		ProviderName=David S North, DDS
		[ACCOUNT]
		AccountNO=100003
		Title=
		FirstName=Pamela
		LastName=Courtney
		MiddleInitial=J
		Suffix=
		HomePhone=(360)987-5555
		WorkPhone=(360)748-7777
		WorkPhoneExt=
		Street=23665 Malmore Rd
		City=Rochester
		State=WA
		Zip=98579
		[REFERDR]
		RefdrID=NELS12345          <--- This is the key from the PMS that we will use to locate the Referring Doctor in Panda.
		RefdrLastName=Nelson
		RefdrfirstName=Michael
		RefdrMiddleInitial=
		RefdrNickName=Mike
		RefdrStreet=1234 Anywhere St.
		RefdrStreet2=Suite 214
		RefdrCity=Centralia
		RefdrState=WA
		RefdrZip=98531
		RefdrWorkPhone=(360)256-3258
		RefdrFax= 

		Should appear in the following file: C:/Program Files/Panda Perio/PandaPerio.ini
		*/
		public static void SendData(Program programCur,Patient pat) {
			string path=Programs.GetProgramPath(programCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			if(pat.Birthdate.Year<1880) {
				MessageBox.Show("Patient must have a valid birthdate.");
				return;
			}
			string iniPath=GetIniFromRegistry();
			if(string.IsNullOrEmpty(iniPath)) {
				MsgBox.Show("PandaPerioAdvanced","The ini file is not available.");
				return;
			}
			try {
				ODFileUtils.WriteAllTextThenStart(iniPath,GetIniString(pat,programCur),path);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Contructs the ini file string.</summary>
		private static string GetIniString(Patient pat,Program programCur) {
			const string  nl="\r\n";
			string iniString="[PATIENT]"+nl;
			if(ProgramProperties.GetPropVal(programCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				iniString+="PatID="+pat.PatNum.ToString()+nl;
			}
			else {
				iniString+="PatID="+pat.ChartNumber+nl;
			}
			iniString+="Title="+pat.Title+nl;
			iniString+="FirstName="+pat.FName+nl;
			iniString+="LastName="+pat.LName+nl;
			iniString+="MiddleInitial="+pat.MiddleI+nl;
			iniString+="NickName="+pat.Preferred+nl;
			iniString+="Sex="+Patients.ReplacePatient("[PatientGenderMF]",pat)+nl;
			iniString+="BirthDate="+pat.Birthdate.ToShortDateString()+nl;
			iniString+="HomePhone="+pat.HmPhone+nl;
			iniString+="WorkPhone="+pat.WkPhone+nl;
			iniString+="WorkPhoneExt="+nl;
			string ssn="";
			//SSNs which are all zeros will be treated as blank.  Needed for eCW, since eCW sets SSN to 000-00-0000 if the patient does not have an SSN.
			if(pat.SSN.Replace("0","").Trim()!="") {
				//Otherwise, output raw SSN:
				ssn=pat.SSN;
			}
			iniString+="SSN="+ssn+nl;
			iniString+="ProviderName="+Providers.GetFormalName(pat.PriProv)+nl;
			//The patient's guarantor
			Patient guar=Patients.GetPat(pat.Guarantor);
			iniString+="[ACCOUNT]"+nl;
			iniString+="AccountNO="+(guar!=null ? guar.PatNum.ToString() : "")+nl;
			iniString+="Title="+(guar!=null ? guar.Title : "")+nl;
			iniString+="FirstName="+(guar!=null ? guar.FName : "")+nl;
			iniString+="LastName="+(guar!=null ? guar.LName : "")+nl;
			iniString+="MiddleInitial="+(guar!=null ? guar.MiddleI : "")+nl;
			iniString+="Suffix="+nl;
			iniString+="HomePhone="+(guar!=null ? guar.HmPhone : "")+nl;
			iniString+="WorkPhone="+(guar!=null ? guar.WkPhone : "")+nl;
			iniString+="WorkPhoneExt="+nl;
			iniString+="Street="+(guar!=null ? guar.Address : "") + (guar!=null && guar.Address2!="" ? " "+guar.Address2 : "") +nl;
			iniString+="City="+(guar!=null ? guar.City : "")+nl;
			iniString+="State="+(guar!=null ? guar.State : "")+nl;
			iniString+="Zip="+(guar!=null ? guar.Zip : "")+nl;
			//The patient's most recent referral that is marked as 'IsDoctor'
			Referral patRefMostRecent = Referrals.GetIsDoctorReferralsForPat(pat.PatNum).LastOrDefault();
			iniString+="[REFERDR]"+nl;
			iniString+="RefdrID="+(patRefMostRecent!=null ? patRefMostRecent.ReferralNum.ToString() : "")+nl;
			iniString+="RefdrLastName="+(patRefMostRecent!=null ? patRefMostRecent.LName : "")+nl;
			iniString+="RefdrFirstName="+(patRefMostRecent!=null ? patRefMostRecent.FName : "")+nl;
			iniString+="RefdrMiddleInitial="+(patRefMostRecent!=null ? patRefMostRecent.MName : "")+nl;
			iniString+="RefdrNickName="+nl;
			iniString+="RefdrStreet="+(patRefMostRecent!=null ? patRefMostRecent.Address : "")+nl;
			iniString+="RefdrStreet2="+(patRefMostRecent!=null ? patRefMostRecent.Address2 : "")+nl;
			iniString+="RefdrCity="+(patRefMostRecent!=null ? patRefMostRecent.City : "")+nl;
			iniString+="RefdrState="+(patRefMostRecent!=null ? patRefMostRecent.ST : "")+nl;
			iniString+="RefdrZip="+(patRefMostRecent!=null ? patRefMostRecent.Zip : "")+nl;
			iniString+="RefdrWorkPhone="+(patRefMostRecent!=null ? patRefMostRecent.Telephone : "")+nl;
			iniString+="RefdrFax="+nl;
			return iniString;
		}

		private static string GetIniFromRegistry() {
			string retVal="";
			ODException.SwallowAnyException(() => {
				using(RegistryKey key=Registry.CurrentUser.OpenSubKey(@"Software\Panda Perio\Panda")) {
					if(key!=null) {
						object o=key.GetValue("PassfilePath");
						if(o!=null) {
							retVal=o.ToString();
						}
					}
				}
			});
			return retVal;
		}
	}
}
