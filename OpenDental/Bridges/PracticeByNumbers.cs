using System;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	///<summary>Link to Practice By Numbers Report Provider.</summary>
	public class PracticeByNumbers {

		///<summary></summary>
		public PracticeByNumbers() {}

		///<summary></summary>
		public static void ShowPage() {
			try {
				if(!Programs.IsEnabledByHq(ProgramName.PracticeByNumbers,out string err)) {
					MsgBox.Show(err);
					return;
				}
				if(Programs.IsEnabled(ProgramName.PracticeByNumbers)) {
					ODFileUtils.ProcessStart("http://www.opendental.com/manual/portalpracticebynumbers.html");
				}
				else {
					ODFileUtils.ProcessStart("http://www.opendental.com/resources/redirects/redirectpracticebynumbers.html");
				}
			}
			catch(Exception ex) {
				MsgBox.Show("PracticeByNumbers","Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again.");
				ex.DoNothing();
			}
		}
	}
}







