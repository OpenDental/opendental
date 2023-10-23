using CodeBase;
using OpenDentBusiness;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace OpenDental.Bridges {
	///<summary>Launches the program using a combination of command line args and the patient data.</summary>
	public class JazzClassicExamView {

		///<summary></summary>
		public JazzClassicExamView(){
			
		}

		///<summary></summary>
		public static void SendData(Program program,Patient patient) {
			JazzClassicCapture.SendData(program,patient);
		}
	}
}







