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
	public class Irys {

		/// <summary></summary>
		public Irys() {

		}

		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start iRYS without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
			else {
				string info="/PATID \"";
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
					info+=pat.PatNum.ToString();
				}
				else {
					info+=pat.ChartNumber;
				}
				info+="\" /NAME \""+Tidy(pat.FName)+"\" /SURNAME \""+Tidy(pat.LName)+"\"";
				info+=" /DATEB \""+pat.Birthdate.ToString(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Birthdate format (default dd,MM,yyyy)"))+"\"";
				if(pat.Gender.ToString()=="Female"){
					info+=" /SEX \"F\"";
				}
				else{
					info+=" /SEX \"M\"";
				}
				try {
					ODFileUtils.ProcessStart(path,info);
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
		}

		private static string Tidy(string input) {
			string retVal=input.Replace("\"","");
			retVal=retVal.Replace(" ","");
			return retVal;
		}
	}
}










