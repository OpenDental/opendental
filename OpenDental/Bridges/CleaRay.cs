using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	///<summary></summary>
	public class CleaRay {

		///<summary></summary>
		public CleaRay() {
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string str="";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				str+="/ID:"+TidyAndEncapsulate(pat.PatNum.ToString())+" ";
			}
			else{
				str+="/ID:"+TidyAndEncapsulate(pat.ChartNumber)+" ";
			}
			str+="/LN:"+TidyAndEncapsulate(pat.LName)+" ";
			str+="/N:"+TidyAndEncapsulate(pat.FName)+" ";
			try {
				ODFileUtils.ProcessStart(path,str);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons, forward slashes, and spaces.</summary>
		private static string TidyAndEncapsulate(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace("/","");//get rid of any forward slashes.
			return "\""+retVal+"\"";
		}

	}
}







