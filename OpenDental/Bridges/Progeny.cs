using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenDental.Bridges {
	public class Progeny {

		///<summary>Launches the program using command line.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			Process pibridge;
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			try{
				if(pat!=null){
					ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");
					string id="";
					if(PPCur.PropertyValue=="0"){
						id=pat.PatNum.ToString();
					}
					else{
						id=pat.ChartNumber;
					}
					string lname=pat.LName.Replace("\"","").Replace(",","");
					string fname=pat.FName.Replace("\"","").Replace(",","");
					if (pat.Birthdate.Year<1880) {
						throw new ODException(Lans.g("Progeny","Invalid birthdate for")+" "+pat.GetNameFL());
					}
					//Progeny software uses local computer's date format settings, per PIBridge.exe documentation (launch exe to view).
					string birthdate=pat.Birthdate.ToShortDateString();
					pibridge=new Process();
					pibridge.StartInfo.CreateNoWindow=false;
					pibridge.StartInfo.UseShellExecute=true;
					pibridge.StartInfo.FileName=path;
					//Double-quotes are removed from id and name to prevent malformed command. ID could have double-quote if chart number.
					pibridge.StartInfo.Arguments="cmd=open id=\""+id.Replace("\"","")+"\" first=\""+fname.Replace("\"","")+"\" last=\""
						+lname.Replace("\"","")+"\" dob=\""+birthdate.Replace("\"","")+"\"";
					ODFileUtils.ProcessStart(pibridge);
				}//if patient is loaded
				else{
					//Should start Progeny without bringing up a pt.
					pibridge=new Process();
					pibridge.StartInfo.CreateNoWindow=false;
					pibridge.StartInfo.UseShellExecute=true;
					pibridge.StartInfo.FileName=path;
					pibridge.StartInfo.Arguments="cmd=start";
					ODFileUtils.ProcessStart(pibridge);
				}
			}
			catch(ODException ex) {
				MessageBox.Show(ex.Message);
			}
			catch{
				MessageBox.Show(path+" is not available.");
			}
		}

	}
}
