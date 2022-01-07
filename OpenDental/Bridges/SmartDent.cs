using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	///<summary></summary>
	public class SmartDent {

		///<summary></summary>
		public SmartDent() {

		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start SMARTDent without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info="";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				info+="\""+pat.PatNum.ToString()+"\" ";
			}
			else {
				info+="\""+Tidy(pat.ChartNumber)+"\" ";
			}
			if(pat.FName!="") {
				info+="\""+Tidy(pat.FName)+" ";
			}
			else {
				info+="\"";
			}
			info+=Tidy(pat.LName)+"\"";
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
			retVal=retVal.Replace("\"","");
			return retVal;
		}

	}
}







