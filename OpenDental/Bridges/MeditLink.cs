using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	///<summary></summary>
	public class MeditLink {

		private const string UUID="E02698AC-C1AE-4699-9307-F4A58DED47FE";

		///<summary></summary>
		public MeditLink(){
			
		}

		///<summary></summary>
		public static void SendData(Program program,Patient patient) {
			string commandArgs="--commandUuid \""+UUID+"\"";
			string path=Programs.GetProgramPath(program);
			if(patient!=null) {
				string optionalParameters=program.CommandLine;
				optionalParameters=Patients.ReplacePatient(optionalParameters, patient);
				commandArgs+=" --commandParam \""+optionalParameters+"\"";
			}
			try {
				Process.Start(path,commandArgs);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return;
		}
	}
}








