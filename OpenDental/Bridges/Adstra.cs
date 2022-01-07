using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	public class Adstra {

		///<summary></summary>
		public Adstra(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("Adstra","Please select a patient first.");
				return;
			}
			string str="";
			str+=Tidy(pat.LName)+",";
			str+=Tidy(pat.FName)+",";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				str+=pat.PatNum.ToString()+",,";
			}
			else{
				str+=","+Tidy(pat.ChartNumber)+",";
			}
			//If birthdates are optional, only send them if they are valid.
			str+=pat.Birthdate.ToString("yyyy/MM/dd"); //changed to match bridge format. Was MM/dd/yyy
			try {
				ODFileUtils.ProcessStart(path,str);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");
			return retVal;
		}

	}
}

