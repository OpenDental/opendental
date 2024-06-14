using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	public class PracticeBooster {

		///<summary></summary>
		public PracticeBooster(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			try {
				Process.Start(path);//should start PracticeBooster without bringing up a pt.
			}
			catch {
				MessageBox.Show(path+" is not available.");
			}
			return;
		}


	}
}







