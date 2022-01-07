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
	public class RayMage{

		/// <summary></summary>
		public RayMage() {
			
		}

		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//shold start rayMage without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
			else {
				string info=" /PATID \"";
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
					info+=pat.PatNum.ToString();
				}
				else{
					info+=pat.ChartNumber;
				}
				info+="\" /NAME \""+pat.FName.Replace(" ","").Replace("\"","")+"\" /SURNAME \""+pat.LName.Replace(" ","").Replace("\"","")+"\"";
				try {
					ODFileUtils.ProcessStart(path,ProgramCur.CommandLine+info);
				}
				catch {
					MessageBox.Show(path+" is not available, or there is an error in the command line options.");
				}
			}
		}

	}
}










