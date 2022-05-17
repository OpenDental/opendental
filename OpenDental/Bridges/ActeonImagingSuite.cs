using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental.Bridges {
	public class ActeonImagingSuite {

		///<summary>Launches the program using command line.</summary>
		public static void SendData(Program ProgramCur, Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> ForProgram =ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
			if(pat!=null){
				string info="";				
				string propertyId=ProgramProperties.GetCur(ForProgram, "Enter 0 to use PatientNum, or 1 to use ChartNum").PropertyValue;
				string dobFormat=ProgramProperties.GetCur(ForProgram, "Birthdate format (default yyyyMMdd)").PropertyValue;
				info=(propertyId=="0"?pat.PatNum.ToString():pat.ChartNumber);
				info+=" \""+pat.LName.Replace("\"","")+"\" \""+pat.FName.Replace("\"","")+"\" \""+pat.Birthdate.ToString(dobFormat)+"\"";
				try{
					ODFileUtils.ProcessStart(path,ProgramCur.CommandLine+info);
				}
				catch{
					MessageBox.Show(path+" is not available, or there is an error in the command line options.");
				}
			}//if patient is loaded
			else{
				try{
					ODFileUtils.ProcessStart(path);
				}
				catch{
					MessageBox.Show(path+" is not available.");
				}
			}
		}

	}
}

			////Insert ActeonImagingSuite bridge-----------------------------------------------------------------
			//if(DataConnection.DBtype==DatabaseType.MySql) {
			//	command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
			//		+") VALUES("
			//		+"'ActeonImagingSuite', "
			//		+"'AIS. from www.acteongroup.com/', "
			//		+"'0', "
			//		+"'"+POut.String(@"C:\Program Files\Acteon\ActeonImagingSuite\AIS.exe")+"', "
			//		+"'', "//leave blank if none
			//		+"'')";
			//	long programNum=Db.NonQ(command,true);
			//	command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
			//		+") VALUES("
			//		+"'"+POut.Long(programNum)+"', "
			//		+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
			//		+"'0')";
			//	Db.NonQ(command);
			//	command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
			//		+") VALUES("
			//		+"'"+POut.Long(programNum)+"', "
			//		+"'Birthdate format (default yyyyMMdd)', "
			//		+"'yyyyMMdd')";
			//	Db.NonQ(command);
			//}
			//else {//oracle
			//	command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
			//		+") VALUES("
			//		+"(SELECT MAX(ProgramNum)+1 FROM program),"
			//		+"'ActeonImagingSuite', "
			//		+"'AIS. from www.acteongroup.com/', "
			//		+"'0', "
			//		+"'"+POut.String(@"C:\Program Files\Acteon\ActeonImagingSuite\AIS.exe")+"', "
			//		+"'', "//leave blank if none
			//		+"'')";
			//	long programNum=Db.NonQ(command,true);
			//	command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
			//		+") VALUES("
			//		+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
			//		+"'"+POut.Long(programNum)+"', "
			//		+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
			//		+"'0', "
			//		+"'0')";
			//	command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
			//		+") VALUES("
			//		+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
			//		+"'"+POut.Long(programNum)+"', "
			//		+"'Birthdate format (default yyyyMMdd)', "
			//		+"'yyyyMMdd', "
			//		+"'0')";
			//	Db.NonQ(command);
			//}//end ActeonImagingSuite bridge
