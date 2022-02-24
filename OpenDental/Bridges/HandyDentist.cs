using System;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	///<summary></summary>
	public class HandyDentist {

		///<summary></summary>
		public HandyDentist() {

		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start HandyDentist without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info="-no:\"";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				info+=pat.PatNum.ToString();
			}
			else {
				if(pat.ChartNumber==null || pat.ChartNumber=="") {
					MsgBox.Show("HandyDentist","This patient does not have a chart number.");
					return;
				}
				info+=Tidy(pat.ChartNumber);
			}
			info+="\" -fname:\""+Tidy(pat.FName)+"\" ";
			info+=" -lname:\""+Tidy(pat.LName)+"\" ";
			try {
				ODFileUtils.ProcessStart(path,info);
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




