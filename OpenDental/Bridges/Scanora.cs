using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental.Bridges{
	///<summary></summary>
	public class Scanora {

		///<summary></summary>
		public Scanora() {
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			string iniFile=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Import.ini path");
			if(pat==null) {
				MsgBox.Show("Scanora","Please select a patient first.");
				return;
			}
			//Start with a blank line per Scanora's example ini file, and then another blank line following the Header.
			string iniText="\r\n[PracticeManagementInterface]\r\n\r\n"
				+"CLEAR_PRACTICE_MANAGEMENT_AUTOMATICALLY = 1\r\n"
				+"USE_PRACTICE_MANAGEMENT = 1\r\n"
				+"PATID = ";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				iniText+=pat.PatNum.ToString();
			}
			else{
				iniText+=Tidy(pat.ChartNumber);
			}
			iniText+="\r\n";
			iniText+="PATLNAME = "+Tidy(pat.LName)+"\r\n";
			iniText+="PATMNAME = "+Tidy(pat.MiddleI)+"\r\n";
			iniText+="PATFNAME = "+Tidy(pat.FName)+"\r\n";
			iniText+="PATSOCSEC = ";
			if(pat.SSN.Replace("0","").Trim()!="") {
				iniText+=pat.SSN;
			}
			iniText+="\r\n";
			//We changed the date format from yyyy-MM-dd to ToShortDateString() because of an email from Chris Bope (Product Manager for Sorodex).
			//Chris said that a valid date must be in ToShortDateString() because their program assumes that is the format when it gets saved.
			//The email copy can be found on PatNum 23172 on a commlog dated 12/29/2015.
			iniText+="PATBD = "+pat.Birthdate.ToShortDateString()+"\r\n";
			iniText+="PROVIDER1 = "+Providers.GetFormalName(pat.PriProv)+"\r\n";
			iniText+="PROVIDER2 = "+Providers.GetFormalName(pat.SecProv)+"\r\n";
			iniText+="ADDRESS1 = "+Tidy(pat.Address)+"\r\n";
			iniText+="ADDRESS2 = "+Tidy(pat.Address2)+"\r\n";
			iniText+="CITY = "+Tidy(pat.City)+"\r\n";
			iniText+="STATE = "+Tidy(pat.State)+"\r\n";
			iniText+="ZIP = "+Tidy(pat.Zip)+"\r\n";
			iniText+="HOMEPHONE = "+new string(pat.HmPhone.Where(x => char.IsDigit(x)).ToArray())+"\r\n";
			iniText+="WORKPHONE = "+new string(pat.WkPhone.Where(x => char.IsDigit(x)).ToArray())+"\r\n";
			iniText+="EMAIL1 = "+Tidy(pat.Email)+"\r\n";
			try {
				//Chris Bope (Product Manager for Sorodex) said "ANSI" is the preferred encoding.
				//Code page 1252 is the most commonly used ANSI code page, and we use 1252 in other bridges as well.
				ODFileUtils.WriteAllTextThenStart(iniFile,iniText,Encoding.GetEncoding(1252),path,"");
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			return retVal;
		}

	}
}







