using System;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class VistaDent {

		/// <summary></summary>
		public VistaDent() {
			
		}

		/// <summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {//Launch program without any patient.
				try {
					ODFileUtils.ProcessStart(path);
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			//Documentation for command line arguments is very vague.
			//See \\serverfiles\Storage\OPEN DENTAL\Programmers Documents\Bridge Info\RemoteExecuter for documentation on how this is being used by NADG
			List<string> listArgs=new List<string>();
			listArgs.Add(ProgramCur.CommandLine);
			listArgs.Add("-first=\""+Tidy(pat.FName)+"\"");
			listArgs.Add("-last=\""+Tidy(pat.LName)+"\"");
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				listArgs.Add("-id=\""+pat.PatNum.ToString()+"\"");
			}
			else{
				listArgs.Add("-id=\""+Tidy(pat.ChartNumber)+"\"");
			}
			//Required. Should update automatically based on pat id, in the case where first bridged with b-date 01/01/0001, although we don't know for sure
			listArgs.Add("-DOB=\""+pat.Birthdate.ToString("yyyy-MM-dd")+"\"");
			if(pat.Gender==PatientGender.Female) {
				//Probably what they use for female, based on their example for male, although we do not know for sure because the specification does not say.
				listArgs.Add("-sex=\"f\"");
			}
			else if(pat.Gender==PatientGender.Male) {
				listArgs.Add("-sex=\"m\"");//This option is valid, because it is part of the example inside the specification.
			}
			else {
				//Probably what they use for unknown (if unknown is even an option), based on their example for male, although we do not know for sure because
				//the specification does not say.
				listArgs.Add("-sex=\"u\"");
			}
			try {
				ODFileUtils.ProcessStart(path,string.Join(" ",listArgs));
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=input.Replace("\"","");//get rid of any quotation marks.
			retVal=retVal.Replace(" ","");
			return retVal;
		}

	}
}