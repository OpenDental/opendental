using System;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.IO;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class VixWinNumbered {

		/// <summary></summary>
		public VixWinNumbered() {

		}

		///<summary>Sends data for Patient.Cur by command line interface.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("VixWinNumbered","Please select a patient first.");
				return;
			}
			string ppImagePath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Image Path");
			if(ppImagePath.Trim()=="") {
				MsgBox.Show("VixWinNumbered","Missing Image Path.");
				return;
			}
			string subDirNumbers=(pat.PatNum%100).ToString().PadLeft(2,'0');//Take the rightmost 2 numbers, preceeding with 0 if patnum<10
			string fullPath=ODFileUtils.CombinePaths(ppImagePath.Trim(),subDirNumbers,pat.PatNum.ToString());
			if(!ODBuild.IsWeb() && !Directory.Exists(fullPath)){
				try {
					Directory.CreateDirectory(fullPath);
				}
				catch {
					MessageBox.Show(Lan.g("VixWinNumbered","Patient image path could not be created.  This usually indicates a permission issue.  Path")+":\r\n"
						+fullPath);
					return;
				}
			}
			//Example: c:\vixwin\vixwin -I 123ABC -N Bill^Smith -P X:\VXImages\02\196402\
			string info="-I ";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				info+=pat.PatNum.ToString();
			}
			else {
				info+=pat.ChartNumber;//max 64 char
			}
			info+=" -N "+pat.FName.Replace(" ","")+"^"+pat.LName.Replace(" ","");//no spaces allowed
			info+=" -P "+fullPath;//This is the Numbered Mode subdirectory
			try {
				ODFileUtils.ProcessStart(path,info,createDirIfNeeded:fullPath);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message+"\r\nFile and command line:\r\n"+path+" "+info);
			}
		}

	}
}











