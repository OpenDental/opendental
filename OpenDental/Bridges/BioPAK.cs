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
	public class BioPAK{

		/// <summary></summary>
		public BioPAK() {
			
		}

		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start rayMage without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
			else {
				string info=" -n";
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
					info+=pat.PatNum.ToString();
				}
				else{
					info+=pat.ChartNumber;
				}
				info+=" -l"+pat.LName.Replace(" ","").Replace("\"","")+" -f"+pat.FName.Replace(" ","").Replace("\"","")+" -i"+pat.MiddleI.Replace(" ","").Replace("\"","");
				if(pat.Gender==PatientGender.Female) {
					info+=" -sF";
				}
				else if(pat.Gender==PatientGender.Male) { 
					info+=" -sM";
				}
				if(pat.Birthdate.Year>1880) {
					info+=" -m"+pat.Birthdate.Month+" -d"+pat.Birthdate.Day+" -y"+pat.Birthdate.Year;
				}
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










