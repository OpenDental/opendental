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
	public class DentX{

		/// <summary></summary>
		public DentX(){
			
		}

		[DllImport("kernel32")]//this is the windows function for reading from ini files.
		private static extern int GetPrivateProfileString(string section,string key,string def
			,StringBuilder retVal,int size,string filePath);

		private static string ReadValue(string fileName,string section,string key){
			StringBuilder strBuild=new StringBuilder(255);
			int i=GetPrivateProfileString(section,key,"",strBuild,255,fileName);
			return strBuild.ToString();
		}

		///<summary>Launches the program using the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);;
			if(pat==null){
				MessageBox.Show("Please select a patient first");
				return;
			}
			//Get the program path from the ini file
			string windir=Environment.GetEnvironmentVariable("windir");// C:\WINDOWS
			string iniFile=windir+"\\dentx.ini";
			if(!File.Exists(iniFile)){
				MessageBox.Show("Could not find "+iniFile);
				return;
			}
			//Make sure the program is running
			string proimagePath=ReadValue(iniFile,"imagemgt","MainFile");
			Process[] proImageInstances=Process.GetProcessesByName("ProImage");
			if(proImageInstances.Length==0){
				ODFileUtils.ProcessStart(proimagePath);
				Thread.Sleep(TimeSpan.FromSeconds(10));
			}
			//command="Xray,PatientNo,FirstName,LastName,Birth Date,Sex,Address,City,State,Code"(zip)
			string command="Xray,";
			//PatientNo can be any string format up to 9 char
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
			if(PPCur.PropertyValue=="0"){
				command+=pat.PatNum.ToString()+",";
			}
			else{
				if(pat.ChartNumber==""){
					MessageBox.Show("ChartNumber for this patient is blank.");
					return;
				}
				command+=pat.ChartNumber.Replace(",","")+",";
			}
			command+=pat.FName.Replace(",","")+","
				+pat.LName.Replace(",","")+","
				+pat.Birthdate.ToShortDateString()+",";
			if(pat.Gender==PatientGender.Female)
				command+="F,";
			else
				command+="M,";
			command+=pat.Address.Replace(",","")+","
				+pat.City.Replace(",","")+","
				+pat.State.Replace(",","")+","
				+pat.Zip.Replace(",","");
			//MessageBox.Show(command);
			try {
				//Create a context that uses a dedicated thread for DDE message pumping.
				using(DdeContext context=new DdeContext()){
					//Create a client.
					using(DdeClient client=new DdeClient("ProImage","Image",context)){
						//Establish the conversation.
						client.Connect();
						//Start ProImage and open to the Xray Chart screen
						client.Execute(command,2000);//timeout 2 secs
					}
				}
			}
			catch{
				//MessageBox.Show(e.Message);
			}
		}

	}
}
#endif