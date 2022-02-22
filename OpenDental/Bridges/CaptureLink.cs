using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class CaptureLink{

		///<summary></summary>
		public CaptureLink(){

		}

		///<summary>This bridge has not yet been added to the database.  CaptureLink reads the bridge parameters from the clipboard.</summary>
		//Command format: LName FName PatID
		public static void SendData(Program ProgramCur, Patient pat){
			if(pat==null){
				MessageBox.Show("No patient selected.");
				return;
			}
			string path=Programs.GetProgramPath(ProgramCur);
			string info=Tidy(pat.LName)+" ";
			info+=Tidy(pat.FName)+" ";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				info+=pat.PatNum.ToString();
			}
			else{
				if(pat.ChartNumber==null || pat.ChartNumber=="") {
					MsgBox.Show("CaptureLink","This patient does not have a chart number.");
					return;
				}
				info+=Tidy(pat.ChartNumber);
			}
			Clipboard.Clear();
			ODClipboard.SetClipboard(info);
			try {
				ODFileUtils.ProcessStart(path);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes double-quotes and spaces.</summary>
		private static string Tidy(string str){
			str=str.Replace("\"","");//Remove double-quotes.
			return str.Replace(" ","");//Remove spaces.
		}

	}
}

