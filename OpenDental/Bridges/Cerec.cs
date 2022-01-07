using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class Cerec {

		/// <summary></summary>
		public Cerec(){

		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			//Example CerPI.exe -<patNum>;<fname>;<lname>;<birthday DD.MM.YYYY>; (Date format specified in the windows Regional Settings)
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start Cerec without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info= " -" ;
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				info+=pat.PatNum.ToString()+";";
			}
			else {
				info+=pat.ChartNumber.ToString()+";";
			}
			info+=pat.FName+";"+pat.LName+";"+pat.Birthdate.ToShortDateString()+";";
			try {
				ODFileUtils.ProcessStart(path,info);
			}
			catch {
				MessageBox.Show(path+" is not available, or there is an error in the command line options.");
			}
		}

	}
}










