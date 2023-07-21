using CodeBase;
using OpenDentBusiness;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace OpenDental.Bridges {
	///<summary>Launches the program using a combination of command line args and the patient data.</summary>
	public class JazzClassicCapture {

		///<summary></summary>
		public JazzClassicCapture(){
			
		}

		///<summary></summary>
		public static void SendData(Program program,Patient patient) {
			string pathToExe=Programs.GetProgramPath(program);
			if(patient==null) {
				MsgBox.Show("JazzClassic","Please select a patient first.");
				return;
			}
			string patientInfoFilePath=ProgramProperties.GetPropVal(program.ProgramNum,"XML output file path");
			if(File.Exists(patientInfoFilePath)) {
				File.Delete(patientInfoFilePath);
			}
			StringBuilder strb=new StringBuilder();
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars="   ";
			settings.NewLineChars="\r\n";
			XmlWriter writer=XmlWriter.Create(strb,settings);
			writer.WriteProcessingInstruction("xml","version='1.0'");
			writer.WriteStartElement("PatientInfo");
			//writer.WriteAttributeString("xsd","xmlns","http://www.w3.org/2001/XMLSchema");
			//writer.WriteAttributeString("xsi","xmlns","http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteElementString("FirstName","[FName]");
			writer.WriteElementString("LastName","[LName]");
			writer.WriteElementString("Identifier","[PatNum]");
			writer.WriteElementString("Gender","[PatientGenderMF]");
			writer.WriteElementString("Address","[Address]");
			writer.WriteElementString("Address2","[Address2]");
			writer.WriteElementString("Ciy","[City]");
			writer.WriteElementString("State","[State]");
			writer.WriteElementString("Zip","[Zip]");
			writer.WriteElementString("SSN","[SSN]");
			writer.WriteElementString("DOB","[Birthdate]");
			writer.WriteElementString("HomePhone","[HmPhone]");
			writer.WriteElementString("CellPhone","[WirelessPhone]");
			writer.WriteElementString("WorkPhone","[WkPhone]");
			writer.WriteElementString("Email","~");
			writer.WriteElementString("Notes","~");
			writer.WriteEndElement();//PatientInfo
			writer.Flush();
			writer.Close();
			string xmlPatientInfo=Patients.ReplacePatient(strb.ToString(),patient);
			try {
				ODFileUtils.WriteAllTextThenStart(patientInfoFilePath,xmlPatientInfo,pathToExe,"\""+patientInfoFilePath+"\" "+program.CommandLine);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}







