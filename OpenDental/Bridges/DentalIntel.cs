using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	///<summary>Link to Dental Intel Premium Report Provider.</summary>
	public class DentalIntel {

		///<summary></summary>
		public DentalIntel() {
			
		}

		///<summary></summary>
		public static void ShowPage() {
			try {
				if(!Programs.IsEnabledByHq(ProgramName.DentalIntel,out string err)) {
					MessageBox.Show(err);
					return;
				}
				if(Programs.IsEnabled(ProgramName.DentalIntel)) {
					ODFileUtils.ProcessStart("http://www.opendental.com/manual/portaldentalintel.html");
				}
				else {
					ODFileUtils.ProcessStart("http://www.opendental.com/resources/redirects/redirectdentalintel.html");
				}
			}
			catch {
				MsgBox.Show("DentalIntel","Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again.");
			}
		}


	}
}







