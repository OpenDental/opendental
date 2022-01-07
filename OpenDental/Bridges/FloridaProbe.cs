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
	public class FloridaProbe{

		/// <summary></summary>
		public FloridaProbe(){
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.  They also have an available file based method which passes more information like missing teeth, but we don't use it yet.  Their bridge specs are freely posted on their website.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			if(pat==null){
				try{
					ODFileUtils.ProcessStart(path);//should start Florida Probe without bringing up a pt.
				}
				catch{
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string info="/search ";
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");
			if(PPCur.PropertyValue=="0"){
				info+="/chart \""+pat.PatNum.ToString()+"\" ";
			}
			else{
				info+="/chart \""+Cleanup(pat.ChartNumber)+"\" ";
			}
			info+="/first \""+Cleanup(pat.FName)+"\" "
				+"/last \""+Cleanup(pat.LName)+"\"";
			//MessageBox.Show(info);
			//not used yet: /inputfile "path to file"
			try{
				ODFileUtils.ProcessStart(path,info);
			}
			catch{
				MessageBox.Show(path+" is not available.");
			}
		}

		///<summary>Makes sure invalid characters don't slip through.</summary>
		private static string Cleanup(string input){
			return input.Replace("\"","");//get rid of any quotes
		}

	}
}










