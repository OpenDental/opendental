using System;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	public class RapidCall {

		///<summary></summary>
		public RapidCall(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur) {
			string path=Programs.GetProgramPath(ProgramCur);
			try {
				ODFileUtils.ProcessStart(path,ProgramCur.CommandLine);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Throws exceptions.</summary>
		public static void ShowPage() {
			try {
				if(Programs.IsEnabled(ProgramName.RapidCall)) {
					SendData(Programs.GetCur(ProgramName.RapidCall));
				}
				else {
					ODFileUtils.ProcessStart("http://www.opendental.com/resources/redirects/redirectdentaltekrapidcall.html");
				}
			}
			catch {
				throw new Exception(Lans.g("RapidCall","Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet and then try again."));
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







