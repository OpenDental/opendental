using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	public class Benco {

		///<summary></summary>
		public Benco(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur) {
			string path=ProgramCur.Path;
			try {
				Process.Start(path);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g("Benco","Unable to launch")+" "+ProgramCur.ProgDesc+".",ex);
			}
		}

	}
}