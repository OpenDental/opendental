using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges {
	public class Sopro {

		///<summary>Launches the program using command line.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			if(pat!=null){
				string info="";				
				//Patient id can be any string format
				ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");
				if(PPCur.PropertyValue=="0"){
					info+=" "+pat.PatNum.ToString();
				}
				else{
					info+=" "+pat.ChartNumber;
				}
				//We remove double-quotes from the first and last name of the patient so extra double-quotes don't
				//cause confusion in the command line parameters for Sopro.
				info+=" "+pat.LName.Replace("\"","")+" "+pat.FName.Replace("\"","");
				try{
					ODFileUtils.ProcessStart(path,ProgramCur.CommandLine+info);
				}
				catch{
					MessageBox.Show(path+" is not available, or there is an error in the command line options.");
				}
			}//if patient is loaded
			else{
				try{
					ODFileUtils.ProcessStart(path);//should start Sopro without bringing up at pt.
				}
				catch{
					MessageBox.Show(path+" is not available.");
				}
			}
		}

	}
}
