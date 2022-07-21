using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental.Bridges {
	public class PORTRAY {///<summary></summary>
		public PORTRAY() {

		}
		///<summary></summary>
		public static void SendData(Program programCur,Patient pat) {
			string path = Programs.GetProgramPath(programCur);
			if(pat==null) {
				// Tell user to pick a patient first.
				MsgBox.Show("PORTRAY","Please select a patient first.");
				//return in both cases
				return;
			}
			if(pat.Birthdate==null || pat.Birthdate.Year<=1880) {
				// Tell user to enter a birthdate.
				MsgBox.Show("PORTRAY","Please enter a birthdate for your selected patient.");
				return;
			}
			//The path is available in the registry, but we'll just make the user enter it.
			/*if(!File.Exists(path)) {
				MessageBox.Show("Could not find "+path);
				return;
			}*/
			//ProgramProperties.GetPropVal() is the way to get program properties.
			if(ProgramProperties.GetPropVal(programCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="1") {
				programCur.CommandLine=programCur.CommandLine.Replace("[PatNum]","[ChartNumber]");
			}
			else {
				programCur.CommandLine=programCur.CommandLine.Replace("[ChartNumber]","[PatNum]");
			}
			string str=Patients.ReplacePatient(programCur.CommandLine,pat);
			str=str.Replace("<>","__ignore__");
			str=str.Replace("<","");
			str=str.Replace(">","");
			try {
				ODFileUtils.ProcessStart(path,str);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}