using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	public class Triana {

		///<summary></summary>
		public Triana() {

		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			string iniFile=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Import.ini path");
			string cmd="-F"+iniFile;//No space between -F and the ini path on purpose per Genoray Triana documentation.
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			string iniText="[OPERATION]\r\n"
				+"EXECUTE=3\r\n"//EXECUTE=3 will register the patient and then open them. If patient is already registered, it will just open them.
				+"[PATIENT]\r\n"
				+"PATIENTID=";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				iniText+=pat.PatNum.ToString()+"\r\n";
			}
			else {
				iniText+=Tidy(pat.ChartNumber)+"\r\n";
			}
			iniText+="FIRSTNAME="+Tidy(pat.FName)+"\r\n";
			iniText+="LASTNAME="+Tidy(pat.LName)+"\r\n";
			iniText+="SOCIAL_SECURITY=";
			//If SSN is allowed by the bridge software, then it will always be optional, because SSNs are not common enough to require.
			//Additionally, Open Dental does not require SSN when creating a patient, so we cannot guarantee that the SSN exists.
			if(pat.SSN.Replace("0","").Trim()!="") {
				iniText+=pat.SSN;
			}
			iniText+="\r\n";
			iniText+="BIRTHDAY=";
			if(pat.Birthdate.Year>1880) {
				iniText+=pat.Birthdate.ToString("yyyyMMdd");
			}
			iniText+="\r\n";
			iniText+="PATIENTCOMMENT=\r\n";//No need to import patient comments
			iniText+="GENDER=";
			if(pat.Gender==PatientGender.Male) {
				iniText+="1";
			}
			else if(pat.Gender==PatientGender.Female) {
				iniText+="2";
			}
			else {
				iniText+="3";
			}
			try {
				ODFileUtils.WriteAllTextThenStart(iniFile,iniText,Encoding.GetEncoding(1252),path,cmd);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");
			return retVal;
		}

	}
}







