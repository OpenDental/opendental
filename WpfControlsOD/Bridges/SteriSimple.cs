using System;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class SteriSimple{

		/// <summary></summary>
		public SteriSimple() {
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("SteriSimple","Please select a patient first.");
				return;
			}
			string str="";
			str+=Tidy(pat.LName.ToUpper())+",";
			str+=Tidy(pat.FName.ToUpper())+",";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				str+=pat.PatNum.ToString();
			}
			else{
				str+=Tidy(pat.ChartNumber);
			}
			try {
				ODFileUtils.ProcessStart(path,str);
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
			}
		}

		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");
			return retVal;
		}
	}
}
