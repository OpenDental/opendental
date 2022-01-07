using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class Dolphin{

		/// <summary></summary>
		public Dolphin(){
			
		}

		//"DOLCTRL patnum" launches Dolphin with selected patient.
		//C:\DOLPHIN\DOLDB.EXE imports patient info from a file
		///<summary>Launches the program using a command line tools.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			string pathDolDb=Path.Combine(path,"dolDb.exe");
			string pathDolCtrl=Path.Combine(path,"dolCtrl.exe");
			if(pat==null){
				try{
					Process.Start(pathDolCtrl);//should start Dolphin without bringing up a pt.
				}
				catch (Exception e){
					MessageBox.Show(e.Message+"\r\n"+pathDolCtrl);
				}
				return;
			}
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");
			string patientId=pat.PatNum.ToString();
			if(PPCur.PropertyValue=="1"){
				patientId=pat.ChartNumber;
			}
			PPCur=ProgramProperties.GetCur(ForProgram,"Filename");
			string filename=PPCur.PropertyValue;
			StringBuilder txt=new StringBuilder();
			txt.Append("[Patient Info]\r\n");
			txt.Append("PatientID="+Tidy(patientId,10)+"\r\n");
			txt.Append("LastName="+Tidy(pat.LName,50)+"\r\n");
			txt.Append("FirstName="+Tidy(pat.FName,50)+"\r\n");
			if(pat.Birthdate.Year>1880){
				txt.Append("Birthdate="+pat.Birthdate.ToString("MM-dd-yyyy")+"\r\n");//mm-dd-yyyy
			}
			string gender="0";
			if(pat.Gender==PatientGender.Female){
				gender="1";
			}
			txt.Append("Gender="+gender+"\r\n");
			txt.Append("NickName="+Tidy(pat.Preferred,30)+"\r\n");
			txt.Append("Title="+Tidy(pat.Title,10)+"\r\n");
			txt.Append("Email="+Tidy(pat.Email,60)+"\r\n");
			//txt.Append("Notes="+Tidy(,)+"\r\n");//no limit?
			//there are more fields, but that's enough for now.
			try{
				File.WriteAllText(filename,txt.ToString());
				Process proc=new Process();
				proc.StartInfo=new ProcessStartInfo(pathDolDb,"Find -i"+patientId);
				proc.Start();
				proc.WaitForExit();//proc.WaitForExit(5000);//5 seconds
				int exitcode=proc.ExitCode;//0=notfound, 135=found
				if(exitcode==0){//not found
					Process.Start(pathDolDb,"AddPatient -f\""+filename+"\" -i"+patientId);
				}
				else if(exitcode==135){//found
					Process.Start(pathDolDb,"UpdatePatient -f\""+filename+"\" -i"+patientId);
				}
				else{
					MessageBox.Show("Error in dolDb.exe Find.  Code="+exitcode.ToString());
				}
				Process.Start(pathDolCtrl,patientId);
			}
			catch (Exception e){
				MessageBox.Show(e.Message);
			}
		}

		private static string Tidy(string txt,int maxLength){
			if(txt.Length>maxLength){
				return txt.Substring(0,maxLength);
			}
			return txt;
		}

	}
}










