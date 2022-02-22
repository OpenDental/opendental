using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using OpenDentBusiness;


namespace OpenDental.Bridges {
	public class RayBridge {
		///<summary>RayBridge is the new way to bridge to SMARTDent.</summary>
		public RayBridge() {

		}
		///<summary>Sends data for the patient to PatientInfo.xml and launches the program.</summary>
		public static void SendData(Program programCur,Patient pat) {
			//Check if there is a selected patient.
			if(pat==null) {
				MessageBox.Show(Lan.g("RayBridge","Please select a patient first."));
				return;
			}
			string path=Programs.GetProgramPath(programCur);
			if(!File.Exists(path)) {
				MessageBox.Show($"{path} {Lan.g("RayBridge","could not be found.")}");
				return;
			}
			string strFilePath=ProgramProperties.GetPropVal(programCur.ProgramNum,"Xml output file path");
			if(!ODBuild.IsWeb() && File.Exists(strFilePath)) {//Will never exist for WEB version.
				File.Delete(strFilePath);
			}
			string patientId;
			if(ProgramProperties.GetPropVal(programCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
				patientId=pat.PatNum.ToString();
			}
			else{
				patientId=pat.ChartNumber;//ChartNumber maxes out at 15 characters which RayBridge allows up to 20 chars.
			}
			string gender="O";//RayBridge uses "M","F","O" for gender.
			if(pat.Gender==PatientGender.Male) {
				gender="M";
			}
			else if(pat.Gender==PatientGender.Female) {
				gender="F";
			}
			StringBuilder strb=new StringBuilder();
			XmlWriterSettings settings=new XmlWriterSettings();
			settings.Indent=true;
			settings.IndentChars="  ";
			settings.NewLineChars="\r\n";
			settings.OmitXmlDeclaration=true;
			XmlWriter writer=XmlWriter.Create(strb,settings);
			writer.WriteStartElement("RAYSCAN");
			writer.WriteStartElement("PatientInfo");
			writer.WriteStartElement("ID");
			writer.WriteAttributeString("value",patientId);
			writer.WriteEndElement();//ID
			writer.WriteStartElement("Name");
			writer.WriteAttributeString("LastName",pat.LName);
			writer.WriteAttributeString("FirstName",pat.FName);
			writer.WriteAttributeString("MiddleName",pat.MiddleI);
			writer.WriteEndElement();//Name
			writer.WriteStartElement("Birthday");
			writer.WriteAttributeString("value",pat.Birthdate.ToString("yyyy-MM-dd"));
			writer.WriteEndElement();//Birthday
			writer.WriteStartElement("Sex");
			writer.WriteAttributeString("value",gender);
			writer.WriteEndElement();//Sex
			writer.WriteEndElement();//PatientInfo
			writer.WriteEndElement();//RAYSCAN
			writer.Flush();
			writer.Close();
			try {
				ODFileUtils.WriteAllTextThenStart(strFilePath,strb.ToString(),path);
				return;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g("RayBridge","Error launching program:")+"\r\n"+MiscUtils.GetExceptionText(ex));
			}
		}
	}
}
