using OpenDentBusiness;
using System;
using System.Diagnostics;

namespace OpenDental.Bridges {
	///<summary>Shining 3D Bridge</summary>
	public class Shining3D {

		///<summary></summary>
		public Shining3D(){
			
		}

		///<summary></summary>
		public static void SendData(Program program,Patient patient) {
			string path=Programs.GetProgramPath(program);
			if(patient==null) {
				MsgBox.Show("Shining3D","Please select a patient first.");
				return;
			}
			string commandLine=Patients.ReplacePatient(program.CommandLine,patient);
			try {
				Process.Start(path,commandLine);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}







