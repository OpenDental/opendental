using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class iDixel {

		/// <summary></summary>
		public iDixel(){
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path = Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start i-Dixel without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
			else {
				string args=" ";
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
					args+=pat.PatNum+" ";
				}
				else {
					args+=pat.ChartNumber+" ";
				}
				args+= pat.FName+" "+pat.LName+" "+(pat.Gender == PatientGender.Female ? "F" : "M");
				try {
					ODFileUtils.ProcessStart(path,args);
				}
				catch {
					MessageBox.Show(path + " is not available.");
				}
			}
		}

	}
}










