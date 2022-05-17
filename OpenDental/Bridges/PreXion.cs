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
	/// <summary>This class is not necessary.  See the manual on how to set up this bridge.</summary>
	public class PreXion{

		/// <summary></summary>
		public PreXion() {
			
		}

		///<summary>Launches the program using command line and chartnumber.</summary>
		public static void SendDataViewer(Program programCur, Patient pat){
			string path=Programs.GetProgramPath(programCur);
			string cmdline="-l "+ProgramProperties.GetPropVal(programCur.ProgramNum,"Username");
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(programCur.ProgramNum,"Password"));
			cmdline+=" -p "+password;
			if(pat!=null) {
				cmdline+=" -pid ";
				if(ProgramProperties.GetPropVal(programCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
					cmdline+=pat.PatNum.ToString();
				}
				else {
					cmdline+=Tidy(pat.ChartNumber);
				}
			}
			cmdline+=" "+ProgramProperties.GetPropVal(programCur.ProgramNum,"Server Name");
			cmdline+=" "+ProgramProperties.GetPropVal(programCur.ProgramNum,"Port");
			try {
				ODFileUtils.ProcessStart(path,cmdline);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Launches the program using command line and chartnumber.</summary>
		public static void SendDataAcquire(Program programCur, Patient pat){
			string path=Programs.GetProgramPath(programCur);
			if(pat==null) {
				MsgBox.Show("PreXion Acquire","Please select a patient first.");
				return;
			}
			string cmdline="/BRIDGE -pid \"";
			if(ProgramProperties.GetPropVal(programCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				cmdline+=pat.PatNum.ToString();
			}
			else {
				cmdline+=Tidy(pat.ChartNumber);
			}
			cmdline+="\" -first \"";
			if(pat.FName!="") {
				cmdline+=Tidy(pat.FName);
			}
			else {
				cmdline+="NA";//not sure if necessary or not
			}
			cmdline+="\" -last \"";
			if(pat.LName!="") {
				cmdline+=Tidy(pat.LName);
			}
			else {
				cmdline+="NA";
			}
			cmdline+="\" -s \"";
			if(pat.Gender==PatientGender.Female) {
				cmdline+="F";
			}
			else if(pat.Gender==PatientGender.Male) {
				cmdline+="M";
			}
			else {
				cmdline+="U";
			}
			cmdline+="\" -dob \"";
			string birthdateFormat=ProgramProperties.GetPropVal(programCur.ProgramNum,"Birthdate format");
			cmdline+=pat.Birthdate.ToString(birthdateFormat)+"\"";
			try {
				ODFileUtils.ProcessStart(path,cmdline);
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons. Underscores aren't supported.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace("\"","");
			return retVal;
		}

	}
}










