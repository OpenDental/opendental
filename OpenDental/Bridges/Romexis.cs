using System.Collections;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class Romexis{

		/// <summary></summary>
		public Romexis(){
			
		}

		///<summary>Launches the program using the patient.Cur data.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			//DxStart.exe ”PatientID” ”FamilyName” ”FirstName” ”BirthDate”
			List<ProgramProperty> ForProgram=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			if(pat==null){
				MessageBox.Show("Please select a patient first");
				return;
			}
			string info="";
			//Patient id can be any string format
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");
			if(PPCur.PropertyValue=="0"){
				info+="\""+pat.PatNum.ToString()+"\" ";
			}
			else{
				info+="\""+pat.ChartNumber.Replace("\"","")+"\" ";
			}
			info+="\""+pat.LName.Replace("\"","")+"\" "
				+"\""+pat.FName.Replace("\"","")+"\" "
				+"\""+pat.Birthdate.ToString("yyyyMMdd")+"\"";
			try{
				ODFileUtils.ProcessStart(path,info);
			}
			catch{
				MessageBox.Show(path+" is not available.");
			}
			
		}

	}
}










