#if !DISABLE_WINDOWS_BRIDGES
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NDde;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class DentalEye{

		/// <summary></summary>
		public DentalEye(){
			
		}

		///<summary>Launches the program. Then passes patient data using command line args.</summary>
		public static void SendData(Program ProgramCur, Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);;
			if(pat==null){
				MessageBox.Show("Please select a patient first");
				return;
			}
			//The path is available in the registry, but we'll just make the user enter it.
			if(!File.Exists(path)){
				MessageBox.Show("Could not find "+path);
				return;
			}
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
			string patID;
			if(PPCur.PropertyValue=="0"){
				patID=pat.PatNum.ToString();
			}
			else{
				if(pat.ChartNumber==""){
					MessageBox.Show("ChartNumber for this patient is blank.");
					return;
				}
				patID=pat.ChartNumber;
			}
			string command="/ID="+patID
				+" /FN="+pat.FName
				+" /LN="+pat.LName
				+" /BD="+pat.Birthdate.ToString("yyyy-MM-dd");
			if(pat.Gender==PatientGender.Female)
				command+=" /Sex=F";
			else
				command+=" /Sex=M";
			try{
				//commandline default is /p
				ODFileUtils.ProcessStart(path,ProgramCur.CommandLine+command);
			}
			catch{
				//MessageBox.Show(e.Message);
			}
		}

	}
}
#endif