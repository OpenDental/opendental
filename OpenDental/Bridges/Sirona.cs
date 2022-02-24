using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class Sirona {

		/// <summary></summary>
		public Sirona() {
			
		}

		///<summary>Sends data for Patient to a mailbox file and launches the program.</summary>
		public static void SendData(Program ProgramCur, Patient pat) {
			OpenDentBusiness.Shared.Sirona.Lans_g=Lans.g;
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			List<string> listIniLines=new List<string>();
			if(pat!=null) {
				try {					
					#region Construct ini info
					//line formats: first two bytes are the length of line including first two bytes and \r\n
					//each field is terminated by null (byte 0).
					//Append U token to siomin.sdx file
					StringBuilder line=new StringBuilder();
					char nTerm=(char)0;//Convert.ToChar(0);
					line.Append("U");//U signifies Update patient in sidexis. Gets ignored if new patient.
					line.Append(nTerm);
					line.Append(pat.LName);
					line.Append(nTerm);
					line.Append(pat.FName);
					line.Append(nTerm);
					line.Append(pat.Birthdate.ToString("dd.MM.yyyy"));
					line.Append(nTerm);
					//leave initial patient id blank. This updates sidexis to patNums used in Open Dental
					line.Append(nTerm);
					line.Append(pat.LName);
					line.Append(nTerm);
					line.Append(pat.FName);
					line.Append(nTerm);
					line.Append(pat.Birthdate.ToString("dd.MM.yyyy"));
					line.Append(nTerm);
					//Patient id:
					ProgramProperty PPCur=ProgramProperties.GetCur(listProgramProperties, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
					if(PPCur.PropertyValue=="0"){
						line.Append(pat.PatNum.ToString());
					}
					else{
						line.Append(pat.ChartNumber);
					}
					line.Append(nTerm);
					if(pat.Gender==PatientGender.Female)
						line.Append("F");
					else
						line.Append("M");
					line.Append(nTerm);
					line.Append(Providers.GetAbbr(Patients.GetProvNum(pat)));
					line.Append(nTerm);
					line.Append("OpenDental");
					line.Append(nTerm);
					line.Append("Sidexis");
					line.Append(nTerm);
					line.Append("\r\n");
					listIniLines.Add(line.ToString());
					//Append N token to siomin.sdx file
					//N signifies create New patient in sidexis. If patient already exists,
					//then it simply updates any old data.
					line=new StringBuilder();
					line.Append("N");
					line.Append(nTerm);
					line.Append(pat.LName);
					line.Append(nTerm);
					line.Append(pat.FName);
					line.Append(nTerm);
					line.Append(pat.Birthdate.ToString("dd.MM.yyyy"));
					line.Append(nTerm);
					//Patient id:
					if(PPCur.PropertyValue=="0"){
						line.Append(pat.PatNum.ToString());
					}
					else{
						line.Append(pat.ChartNumber);
					}
					line.Append(nTerm);
					if(pat.Gender==PatientGender.Female)
						line.Append("F");
					else
						line.Append("M");
					line.Append(nTerm);
					line.Append(Providers.GetAbbr(Patients.GetProvNum(pat)));
					line.Append(nTerm);
					line.Append("OpenDental");
					line.Append(nTerm);
					line.Append("Sidexis");
					line.Append(nTerm);
					line.Append("\r\n");
					listIniLines.Add(line.ToString());
					//Append A token to siomin.sdx file
					//A signifies Autoselect patient. 
					line=new StringBuilder();
					line.Append("A");
					line.Append(nTerm);
					line.Append(pat.LName);
					line.Append(nTerm);
					line.Append(pat.FName);
					line.Append(nTerm);
					line.Append(pat.Birthdate.ToString("dd.MM.yyyy"));
					line.Append(nTerm);
					if(PPCur.PropertyValue=="0"){
						line.Append(pat.PatNum.ToString());
					}
					else{
						line.Append(pat.ChartNumber);
					}
					line.Append(nTerm);
					if(ODBuild.IsWeb()) {
						line.Append("{{SystemInformation.ComputerName}}");//Will be replaced on the client side
					}
					else {
						line.Append(SystemInformation.ComputerName);
					}
					line.Append(nTerm);
					line.Append(DateTime.Now.ToString("dd.MM.yyyy"));
					line.Append(nTerm);
					line.Append(DateTime.Now.ToString("HH.mm.ss"));
					line.Append(nTerm);
					line.Append("OpenDental");
					line.Append(nTerm);
					line.Append("Sidexis");
					line.Append(nTerm);
					line.Append("0");//0=no image selection
					line.Append(nTerm);
					line.Append("\r\n");
					listIniLines.Add(line.ToString());
					#endregion
					if(!ODBuild.IsWeb()) {
						OpenDentBusiness.Shared.Sirona.WriteToSendBoxFile(path,listIniLines);
					}
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g("Sirona","Error preparing Sidexis for patient message."),ex);
					return;
				}
			}//if patient is loaded
			//Start Sidexis.exe whether patient loaded or not.
			try {
				if(ODBuild.IsWeb()) {
					ODCloudClient.SendToSirona(path,listIniLines);
				}
				else {
					ODFileUtils.ProcessStart(path);
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(path+" is not available.",ex);
			}
		}

	}
}