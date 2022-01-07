using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class HdxWill{

		/// <summary></summary>
		public HdxWill(){
			
		}
		
		 /*
		  Example Argument.ini
			[Patient Info = 00]
			PATIENT_APPLYNO=0120525150324003
			PATIENT_ID=20120525
			PATIENT_NAME=김유성
			PATIENT_ENAME=
			PATIENT_SEX=M
			PATIENT_AGE=30
			PATIENT_BIRTH_DATE=19831020
			PATIENT_ADDR=
			PATIENT_PID=
			PATIENT_IPDOPD=
			PATIENT_DOCTOR=
			PATIENT_PHON1=
			PATIENT_PHON2=
			PATIENT_EXAMNAME=TESTEXAM
			INPUT_DATE=20120531
		 */

		///<summary>Launches the program using a command line tools.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			try {
				if(pat!=null) {
					List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);
					ProgramProperty propIniFileLocation=ProgramProperties.GetCur(listProgramProperties,"System path to HDX WILL Argument ini file");
					string patientId=pat.PatNum.ToString();
					string patientId2=pat.ChartNumber.ToString();
					if(ProgramProperties.GetPropValFromList(listProgramProperties, "Enter 0 to use PatientNum, or 1 to use ChartNum")=="1") {
						patientId=pat.ChartNumber;
						patientId2=pat.PatNum.ToString();
					}
					string filename=propIniFileLocation.PropertyValue;
					StringBuilder txt=new StringBuilder();
					txt.AppendLine("[Patient Info = 00]");
					txt.AppendLine("PATIENT_APPLYNO="+StringTools.TruncateBeginning(MiscData.GetNowDateTimeWithMilli().ToString("yyyyMMddhhmmssfff"),16));
					txt.AppendLine("PATIENT_ID="+patientId);
					txt.AppendLine("PATIENT_NAME="+pat.FName+" "+pat.LName);
					txt.AppendLine("PATIENT_ENAME=");
					txt.AppendLine("PATIENT_SEX="+StringTools.Truncate(pat.Gender.ToString(),1));
					txt.AppendLine("PATIENT_AGE="+pat.Age);
					txt.AppendLine("PATIENT_BIRTH_DATE="+pat.Birthdate.ToString("yyyyMMdd"));
					txt.AppendLine("PATIENT_ADDR="+pat.Address);
					txt.AppendLine("PATIENT_PID="+patientId2);
					txt.AppendLine("PATIENT_IPDOPD=");
					txt.AppendLine("PATIENT_DOCTOR="+Providers.GetFormalName(pat.PriProv));
					txt.AppendLine("PATIENT_PHON1="+pat.WirelessPhone);
					txt.AppendLine("PATIENT_PHON2="+pat.HmPhone);
					txt.AppendLine("PATIENT_EXAMNAME=");
					txt.AppendLine("INPUT_DATE="+MiscData.GetNowDateTime().ToString("yyyyMMdd"));
					ODFileUtils.WriteAllTextThenStart(filename,txt.ToString(),path);
					return;
				}
			}
			catch(Exception e) {
				MessageBox.Show(e.Message);
			}
			try {
				ODFileUtils.ProcessStart(path);
			}
			catch (Exception e) {
				MessageBox.Show(path+" is not available.");
			}
		}
	}
}
