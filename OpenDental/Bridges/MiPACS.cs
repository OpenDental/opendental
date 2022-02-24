using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	class MiPACS {
		///<summary>Launches the program using a combination of command line characters and the patient.Cur data.///</summary>
/*The command line integration works as follows:

C:\Program Files\MiDentView\Cmdlink.exe /ID=12345 /FN=JOHN /LN=DOE /BD=10/01/1985 /Sex=M

Parameters:'/ID=' for ID number, '/FN=' for Firstname, '/LN=' for Lastname, '/BD=' for Birthdate, '/Sex=' for Sex.

Example of a name with special characters (in this case, spaces):
C:\Program Files\MiDentView\Cmdlink.exe /ID=12345 /FN=Oscar /LN=De La Hoya /BD=10/01/1985 /Sex=M
		 */
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				try {
					ODFileUtils.ProcessStart(path);//should start MiPACS without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}
			string gender=(pat.Gender==PatientGender.Female)?"F":"M";//M for Male, F for Female, M for Unknown.
			string info="";
			if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				info+="/ID="+pat.PatNum.ToString();
			}
			else{
				info+="/ID="+pat.ChartNumber;
			}
			info+= " /FN=" + pat.FName //special characters claimed to be ok
				+ " /LN=" + pat.LName 
				+ " /BD=" + pat.Birthdate.ToShortDateString() 
				+ " /Sex=" + gender;
			try {
				ODFileUtils.ProcessStart(path,info);
			}
			catch {
				MessageBox.Show(path + " is not available.");
			}
		}
	}
}
