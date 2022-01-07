using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{

	public class AiDental {
		///<summary></summary>
		public AiDental(){
			
		}

		///<summary></summary>
		public static void SendData(Program program,Patient patient) {
			string processPath=Programs.GetProgramPath(program);
			if(patient==null) {
				try {
					Process.Start(processPath);
				}
				catch {
					MessageBox.Show(processPath+" is not available.");
				}
				return;
			}
			string fileText="";
			//Patient last name.
			fileText+=Tidy(patient.LName)+",";
			//Patient first name.
			fileText+=Tidy(patient.FName)+",";
			//Patient birthdate.
			if(patient.Birthdate.Year>1880) {
				fileText+="Birthdate_"+patient.Birthdate.ToString("yyyyMMdd");
			}
			//Output filepath.
			string filePath=ProgramProperties.GetPropVal(program.ProgramNum,"Text file path for Ai-Dental");
			try {
				ODFileUtils.WriteAllTextThenStart(filePath,fileText,processPath);
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







