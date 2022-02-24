using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	public class Carestream {

		///<summary></summary>
		public Carestream(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("Carestream","Please select a patient first.");
				return;
			}
			string infoFile=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Patient.ini path");
			if(infoFile.Length>150) {
				MsgBox.Show("Carestream","Patient.ini file folder path too long.  Must be 150 characters or less.");
				return;
			}
			string id="";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0") {
				id=pat.PatNum.ToString();
			}
			else {
				id=pat.ChartNumber;
			}
			StringBuilder sw=new StringBuilder();
			sw.AppendLine("[PATIENT]");
			sw.AppendLine("ID="+Tidy(id,15));
			sw.AppendLine("FIRSTNAME="+Tidy(pat.FName,255));
			if(!string.IsNullOrEmpty(pat.Preferred)) {
				sw.AppendLine("COMMONNAME="+Tidy(pat.Preferred,255));
			}
			sw.AppendLine("LASTNAME="+Tidy(pat.LName,255));
			if(!string.IsNullOrEmpty(pat.MiddleI)) {
				sw.AppendLine("MIDDLENAME="+Tidy(pat.MiddleI,255));
			}
			if(pat.Birthdate.Year>1880) {
				sw.AppendLine("DOB="+pat.Birthdate.ToString("yyyyMMdd"));
			}
			if(pat.Gender==PatientGender.Female) {
				sw.Append("GENDER=F");
			}
			else if(pat.Gender==PatientGender.Male) {
				sw.Append("GENDER=M");
			}
			try {
				string arguments=@"-I """+infoFile+@"""";
				ODFileUtils.WriteAllTextThenStart(infoFile,sw.ToString(),path,arguments);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input,int maxLength) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");//remove spaces
			if(maxLength>0 && retVal.Length>maxLength) {
				retVal=retVal.Substring(0,maxLength);
			}
			return retVal;
		}

	}
}







