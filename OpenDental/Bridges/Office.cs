using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges {
	///<summary>This is a generic document editor, not a bridge to a specific software.</summary>
	public class Office {

		///<summary></summary>
		public Office() {

		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("Office","Please select a patient first.");
				return;
			}
			string fileName=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Document folder");
			string oldFileName=ODFileUtils.CombinePaths(fileName,Tidy(pat.LName+pat.FName));
			//ProgramProperties.GetPropVal() is the way to get program properties.
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				fileName=ODFileUtils.CombinePaths(fileName,pat.PatNum.ToString());
				oldFileName+=pat.PatNum.ToString();
			}
			else {
				fileName=ODFileUtils.CombinePaths(fileName,Tidy(pat.ChartNumber));
				oldFileName+=Tidy(pat.ChartNumber);
			}
			fileName+=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"File extension");
			oldFileName+=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"File extension");
			Process process=new Process();
			ProcessStartInfo startInfo=new ProcessStartInfo();
			if(!File.Exists(fileName)) {
				if(File.Exists(oldFileName)) {
					fileName=oldFileName;
				}
				else {
					try {
						startInfo.WindowStyle=ProcessWindowStyle.Hidden;
						string cmdLocation=Environment.GetFolderPath(Environment.SpecialFolder.Windows)+@"\system32\cmd.exe";//This is incase the user doesn't have C: as their default drive letter.
						startInfo.FileName=cmdLocation;//Path for the cmd prompt
						startInfo.Arguments="/c copy nul "+fileName;
						process.StartInfo=startInfo;
						ODFileUtils.ProcessStart(process);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			try {
				ODFileUtils.ProcessStart(path,fileName);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes any character that isn't a letter or number.</summary>
		private static string Tidy(string input) {
			string retVal="";
			for(int i=0;i<input.Length;i++) {
				if(Char.IsLetterOrDigit(input,i)) {
					retVal+=input.Substring(i,1);
				}
			}
			return retVal;
		}

	}
}







