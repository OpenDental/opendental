using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges{
	///<summary>This class is just an example template that we use when we build a new bridge.  Start with a copy of this.</summary>
	public class ThreeShape {

		///<summary></summary>
		public ThreeShape(){
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path); //Should start 3Shape without bringing up a patient.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info="";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				info+="-integrationid=\""+pat.PatNum.ToString()+"\" ";
			}
			else{
				info+="-integrationid=\""+Tidy(pat.ChartNumber)+"\" ";
			}
			info+="-patientid=\""+Tidy(pat.SSN)+"\" ";
			info+="-firstname=\""+Tidy(pat.GetNameFirst())+"\" ";
			info+="-lastname=\""+Tidy(pat.LName)+"\" ";
			info+="-birthday=\""+Tidy(pat.Birthdate.ToString("yyyyMMdd"))+"\" ";
			info+="-merge";
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
			retVal=retVal.Replace("\"","");
			return retVal;
		}
	}
}







