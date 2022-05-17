using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class VixWinOld{

		/// <summary></summary>
		public VixWinOld(){
			
		}


		///<summary>Sends data for Patient.Cur to the QuikLink directory. No further action is required.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "QuikLink directory.");
			string quikLinkDir=PPCur.PropertyValue;
			if(pat==null){
				return;
			}
			if(!Directory.Exists(quikLinkDir)){
				MessageBox.Show(quikLinkDir+" is not a valid folder.");
				return;
			}
			try{
				string patID;
				PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
				if(PPCur.PropertyValue=="0"){
					patID=pat.PatNum.ToString().PadLeft(6,'0');
				}
				else{
					patID=pat.ChartNumber.PadLeft(6,'0');
				}
				if(patID.Length>6){
					MessageBox.Show("Patient ID is longer than six digits, so link failed.");
					return;
				}
				string fileName=quikLinkDir+patID+".DDE";
				//MessageBox.Show(fileName);
				using(StreamWriter sw=new StreamWriter(fileName,false)){
					sw.WriteLine("\""+pat.FName+"\","
						+"\""+pat.LName+"\","
						+"\""+patID+"\"");
				}
			}
			catch{
				MessageBox.Show("Error creating file.");
			}
		}
			

	}
}










