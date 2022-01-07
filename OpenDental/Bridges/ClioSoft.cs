using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class ClioSoft{

		/// <summary></summary>
		public ClioSoft() {
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data. 
		///Arguments: ClioSoft.exe “-Id;FirstName;LastName;DateOfBirth;SocialSecurityNumber;”
		///Only Id, FirstName and LastName are required.
		///Example 1: ClioSoft.exe "-100;Jim;Jones;01/02/2000;123-456-789;"
		///Example 2: ClioSoft.exe "-200;Jane;Smith;;;"</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start ClioSoft without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info="\"";
			//Patient id can be any string format
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				info+="-"+pat.PatNum.ToString()+";";
			}
			else{
				info+="-"+Tidy(pat.ChartNumber)+";";
			}
			info+=Tidy(pat.FName)+";";
			info+=Tidy(pat.LName)+";";
			//Birthdate is optional, so we only send if valid.
			if(pat.Birthdate.Year>1880) {
				info+=pat.Birthdate.ToString("MM/dd/yyyy")+";";
			}
			else {
				info+=";";
			}
			info+=Tidy(pat.SSN)+";";//They don't care about dashes and there is no length restriction, just not semi-colons.
			info+="\"";
			try {
				ODFileUtils.ProcessStart(path,info);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semi-colons.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			return retVal;
		}

	}
}
