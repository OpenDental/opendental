using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class VisionX {
		/// <summary></summary>
		public VisionX(){
			
		}

		///<summary>Sends data for Patient.Cur to an import file which VisionX will automatically recognize.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			if(pat==null){
				MsgBox.Show("VisionX","Please select a patient first.");
				return;
			}
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Text file path");
			string infoFile=PPCur.PropertyValue;
			try{
				using(StreamWriter sw=new StreamWriter(infoFile,false)){
					//PATLASTNAME;PATFIRSTNAME;PATBIRTHDAY;PATCARDNUMBER;PATTOWN;PATSTREET;PATPHONENUMBER;PATTITLE;PATSEX;PATPOSTALCODE;
					//everything after birthday is optional
					sw.Write(Tidy(pat.LName));
					sw.Write(Tidy(pat.FName));
					sw.Write(pat.Birthdate.ToString("d.M.yyyy")+";");
					PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
					if(PPCur.PropertyValue=="0"){
						sw.Write(Tidy(pat.PatNum.ToString()));
					}
					else{
						sw.Write(Tidy(pat.ChartNumber));
					}
					sw.Write(Tidy(pat.City));
					sw.Write(Tidy(pat.Address));
					sw.Write(Tidy(pat.HmPhone));
					sw.Write(";");//title
					if(pat.Gender==PatientGender.Female) { 
						sw.Write(Tidy("f"));
					}
					else {
						sw.Write(Tidy("m"));
					}
					sw.Write(Tidy(pat.Zip));
				}
			}
			catch(Exception e){
				MessageBox.Show(e.Message);
			}
			try {
				ODFileUtils.ProcessStart(path);//launch the program after writing the input file
			}
			catch {
				MessageBox.Show(path+" is not available.");
			}
		}

		///<summary>Strips out the semicolons.</summary>
		private static string Tidy(string str){
			str=str.Replace(";","");
			return str+";";
		}

	}
}
