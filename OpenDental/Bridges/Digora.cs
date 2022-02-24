using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class Digora{

		/// <summary></summary>
		public Digora(){
			
		}

		/*string InvalidCommandLine="999";
		string GeneralError="998";
		string NoConnect="997";
		string PatientNotFound="989";
		string ManyPatientFound="988";
		string PatientLocked="987";
		string ImageNotFound="979";
		string FileError="969";
		string OpenManyFoundOK="1";
		string OpenManyFoundCancel="2";
		string CreatePatientExist="11";
		string ChangePatientExist="21";
		string Successful="0";*/

		///<summary>We will use the clipboard interface, although there is an ole automation interface available.</summary>
		//Command format: $$DFWIN$$ <Command> <Options>
		//Return value: $$DFWOUT$$<Return value>[\nReturn string] (we will ignore this value for now)
		//$$DFWIN$$ OPEN -n"LName, FName" -c"PatNum" -r -a
		//option -r creates patient if not found, -a changes focus to Digora
		public static void SendData(Program ProgramCur, Patient pat){
			if(pat==null) {
				MsgBox.Show("Digora","No patient selected.");
				return;
			}
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			string info="$$DFWIN$$ OPEN -n\""+Tidy(pat.LName)+", "+Tidy(pat.FName)+"\" -c\"";
			ProgramProperty PPCur=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum");;
			if(PPCur.PropertyValue=="0"){
				info+=pat.PatNum.ToString();
			}
			else{
				info+=pat.ChartNumber;
			}
			info+="\" -r -a";
			try {
				ODClipboard.SetClipboard(info);
			}
			catch(Exception) {
				//The clipboard will sometimes fail to SetText for many different reasons.  Often times another attempt will be successful.
				MsgBox.Show("Digora","Error accessing the clipboard, please try again.");
				return;
			}
		}

		private static string Tidy(string str){
			return str.Replace("\"","");
		}

	}
}










