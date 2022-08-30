using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class Tscan{

		/// <summary></summary>
		public Tscan() {
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.
		///Arguments: tscan.exe [-fFirstname [-mMiddlename] -lLastname -iPatientid [-dBirthday] [-jBirthmonth] [-yBirthyear] [-gGender]]
		///Example: tscan.exe -fBrent -lThompson -iBT1000 -d07 -j02 -y1962 -g2</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start Tscan without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info="-f"+Tidy(pat.FName)+" ";//First name can only be alpha-numeric, so we remove non-alpha-numeric characters.
			if(Tidy(pat.MiddleI)!="") {//Only send middle name if available, since it is optional.
				info+="-m"+Tidy(pat.MiddleI)+" ";//Middle name can only be alpha-numeric, so we remove non-alpha-numeric characters.
			}
			info+="-l"+Tidy(pat.LName)+" ";//Last name can only be alpha-numeric, so we remove non-alpha-numeric characters.
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				info+="-i"+pat.PatNum.ToString()+" ";
			}
			else{
				info+="-i"+Tidy(pat.ChartNumber)+" ";//Patient id only alpha-numeric as required.
			}
			//Birthdate is optional, so we only send if valid.
			if(pat.Birthdate.Year>1880) {
				info+="-d"+pat.Birthdate.Day.ToString().PadLeft(2,'0')+" ";//The example in specification shows days with two digits, so we pad.
				info+="-j"+pat.Birthdate.Month.ToString().PadLeft(2,'0')+" ";//The example in specification shows months with two digits, so we pad.
				info+="-y"+pat.Birthdate.Year.ToString()+" ";//The example specification shows years 4 digits long.
			}
			//Gender is optional, so we only send if not Unknown.
			if(pat.Gender==PatientGender.Female) {
				info+="-g1 ";
			}
			else if(pat.Gender==PatientGender.Male) {
				info+="-g2 ";
			}
			try {
				ODFileUtils.ProcessStart(path,info);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes non-alpha-numeric characters.</summary>
		private static string Tidy(string input) {
			string retVal=Regex.Replace(input,"[^a-zA-Z0-9]","");//get rid of any non-alpha-numeric characters.
			return retVal;
		}

	}
}










