using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	///<summary>A simple program link to launch Midway Dental's website.</summary>
	public class Midway {

		///<summary></summary>
		public Midway(){
			
		}

		///<summary>Opens the homepage for Midway Dental's website</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			try {
				ODFileUtils.ProcessStart(path);
			}
			catch {
				MessageBox.Show(path+" is not available.");
			}
			return;
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");
			return retVal;
		}

	}
}







