using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	public class OrthoCad {

		///<summary></summary>
		public OrthoCad(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			if(pat==null) {
				MsgBox.Show("OrthoCAD","Please select a patient first.");
				return;
			}
			string path=Programs.GetProgramPath(ProgramCur);
			string cmd="";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				cmd+="-patient_id="+POut.Long(pat.PatNum);
			}
			else {
				cmd+="-chart_number="+pat.ChartNumber;
			}
			try {
				ODFileUtils.ProcessStart(path,cmd);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

	}
}







