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
	public class Apteryx{

		/// <summary></summary>
		public Apteryx(){
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			if(pat!=null){
				//We remove double-quotes from the first and last name of the patient so extra double-quotes don't
				//cause confusion in the command line parameters for Apteryx.
				string info="\""+pat.LName.Replace("\"","")+", "+pat.FName.Replace("\"","")+"::";
				if(pat.SSN.Length==9 && pat.SSN!="000000000"){//SSN is optional.  eCW customers often use 000-00-0000 for any patient that does not have an SSN (mostly young children).
					info+=pat.SSN.Substring(0,3)+"-"
						+pat.SSN.Substring(3,2)+"-"
						+pat.SSN.Substring(5,4);
				}
				//Patient id can be any string format
				ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");
				if(PPCur.PropertyValue=="0"){
					info+="::"+pat.PatNum.ToString();
				}
				else{
					info+="::"+pat.ChartNumber;
				}
				info+="::"+pat.Birthdate.ToShortDateString()+"::";
				if(pat.Gender==PatientGender.Female)
					info+="F";
				else
					info+="M";
				info+="\"";
				try{
					//commandline default is /p
					ODFileUtils.ProcessStart(path,ProgramCur.CommandLine+info);
				}
				catch{
					MessageBox.Show(path+" is not available, or there is an error in the command line options.");
				}
			}//if patient is loaded
			else{
				try{
					ODFileUtils.ProcessStart(path);//should start Apteryx without bringing up at pt.
				}
				catch{
					MessageBox.Show(path+" is not available.");
				}
			}
		}

	}
}










