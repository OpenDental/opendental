using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace OpenDental.Bridges{
	///<summary></summary>
	public class One2 {

		///<summary></summary>
		public One2() {
			
		}

		///<summary></summary>
		public static void SendData(Program program,Patient patient) {
			if(patient==null) {
				MsgBox.Show("One2","Please select a patient first.");
				return;
			}
			string patientInfoFilePath=ProgramProperties.GetPropVal(program.ProgramNum,"XML output file path");
			string fileExtension=null;
			try {
				fileExtension=Path.GetExtension(patientInfoFilePath);
			}
			catch(Exception e) {
				e.DoNothing();
			}
			//Verify the file extension is xml.
			if(string.IsNullOrEmpty(fileExtension) || fileExtension.ToLower()!=".xml") {
				MsgBox.Show("One2", "'XML output file path' must end in '.xml'");
				return;
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(program.ProgramNum);
			ProgramProperty programProperty=ProgramProperties.GetCur(listProgramProperties,"Enter 0 to use PatientNum, or 1 to use ChartNum");
			string patientID=patient.PatNum.ToString();
			if(programProperty.PropertyValue=="1") {
				patientID=patient.ChartNumber;
			}
			try {
				WriteXmlFile(patientInfoFilePath,patientID,patient);
			}
			catch(Exception) {
				MsgBox.Show("One2", "'XML output file path' is in use or has incorrect file permissions.");
				return;
			}
			//Create the required command line args.
			//E.g. --inbridge_patient=""C:\osstem\patient.xml"" --inbridge_from=""xml""
			string commandLineArgs=@$"--inbridge_patient=""{patientInfoFilePath}"" --inbridge_from=""xml""";
			try {
				Process.Start(program.Path,program.CommandLine+commandLineArgs);
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
			}
		}

		private static void WriteXmlFile(string patientInfoFilePath,string patientID,Patient patient) {
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="    ";
			xmlWriterSettings.Encoding=Encoding.UTF8;
			XmlWriter xmlWriter=XmlWriter.Create(patientInfoFilePath,xmlWriterSettings);//Document is auto saved at the path.
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("patient");
			xmlWriter.WriteElementString("chart_id",Tidy(patientID,256));//required
			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteElementString("first_name",Tidy(patient.FName,128));//required
			xmlWriter.WriteElementString("last_name",Tidy(patient.LName,128));
			xmlWriter.WriteElementString("middle_name",Tidy(patient.MiddleI,128));
			xmlWriter.WriteEndElement();//end name
			xmlWriter.WriteStartElement("birthday");
			xmlWriter.WriteElementString("year",patient.Birthdate.ToString("yyyy"));//required
			xmlWriter.WriteElementString("month",patient.Birthdate.ToString("MM"));//required
			xmlWriter.WriteElementString("day",patient.Birthdate.ToString("dd"));//required
			xmlWriter.WriteEndElement();//end birthday
			string gender;
			switch(patient.Gender){//Documentation shows One2 only allows M, F, and O
				case PatientGender.Male:
					gender="M";
					break;
				case PatientGender.Female:
					gender="F";
					break;
				default:
					gender="O";
					break;
			}
			xmlWriter.WriteElementString("gender",gender);//required
			xmlWriter.WriteElementString("mobile",Tidy(patient.WirelessPhone,60));
			xmlWriter.WriteElementString("phone",Tidy(patient.HmPhone,60));
			xmlWriter.WriteElementString("address",Tidy(patient.Address,600));
			//Official patient linkage guide does not mention address_detail as a supported node name.
			//However, all examples provided include an address_detail node name.
			if(!string.IsNullOrWhiteSpace(patient.Address2)) {
				xmlWriter.WriteElementString("address_detail",Tidy(patient.Address2,600));
			}
			xmlWriter.WriteElementString("email",Tidy(patient.Email,100));
			xmlWriter.WriteEndElement();//end patient
			xmlWriter.Flush();
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
		}

		///<summary></summary>
		public static string Tidy(string content,int max) {
			if(content.Length>max) {
				content=content.Substring(0, max);
			}
			return content;
		}
	}
}