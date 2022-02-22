using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class Dxis{

		/// <summary></summary>
		public Dxis(){
			
		}

		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			//usage: C:\Dxis\Dxis.exe /i /t:UniqueID - Practice Name
			//The UniqueID can be a combo of patient name and id.  I think we'll combine Lname,Fname,PatNum
			if(pat==null){
				MsgBox.Show("Dxis","Please select a patient first.");
				return;
			}
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			string info="/i /t:"+pat.LName+" "+pat.FName+" "+pat.PatNum.ToString()+" - "+PrefC.GetString(PrefName.PracticeTitle);
			try{
				Process process=ODFileUtils.ProcessStart(path,info);
				//Don't wait for exit in WEB mode.  Since it opens a browser tab and then an unrelated process on the client,
				//we probably don't have a valid process to wait for, and the resources from Open Dental aren't the same resources from Dxis.
				if(!ODBuild.IsWeb()) {
					process.WaitForExit();//puts OD in sleep mode because the pano is so resource intensive.
				}
			}
			catch{
				MessageBox.Show(path+" is not available.");
			}
		}

		/*
		///<summary>Clips the length of the string as well as making sure invalid characters don't slip through.</summary>
		private static string ClipTo(string input,int length){
			string retVal=input.Replace(";","");//get rid of any semicolons.
			if(retVal.Length>length){
				retVal=retVal.Substring(0,length);
			}
			return retVal;
		}*/

	}
}










