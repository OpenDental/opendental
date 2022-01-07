using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class Ewoo{

		/// <summary></summary>
		public Ewoo() {
			
		}

		///<summary>Sends data for the patient to linkage.xml and launches the program.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(!File.Exists(path)){
				MessageBox.Show(path+" could not be found.");
				return;
			}
			string dir=Path.GetDirectoryName(path);
			string linkage=CodeBase.ODFileUtils.CombinePaths(dir,"linkage.xml");
			if(!ODBuild.IsWeb() && File.Exists(linkage)){//Will never exist for WEB version.
				File.Delete(linkage);
			}
			if(pat!=null) {
				StringBuilder strb=new StringBuilder();
				XmlWriterSettings settings=new XmlWriterSettings();
				settings.Indent=true;
				settings.IndentChars="   ";
				settings.NewLineChars="\r\n";
				settings.OmitXmlDeclaration=true;
				XmlWriter writer=XmlWriter.Create(strb,settings);
				writer.WriteStartElement("LinkageParameter");
				writer.WriteStartElement("Patient");
				writer.WriteAttributeString("LastName",pat.LName);
				writer.WriteAttributeString("FirstName",pat.FName);
				if(ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 0 to use PatientNum, or 1 to use ChartNum")=="0"){
					writer.WriteAttributeString("ChartNumber",pat.PatNum.ToString());
				}
				else{
					writer.WriteAttributeString("ChartNumber",pat.ChartNumber);
				}
				writer.WriteElementString("Birthday",pat.Birthdate.ToString("dd/MM/yyyy"));
				string addr=pat.Address;
				if(pat.Address2!=""){
					addr+=", "+pat.Address2;
				}
				addr+=", "+pat.City+", "+pat.State;
				writer.WriteElementString("Address",addr);
				writer.WriteElementString("ZipCode",pat.Zip);
				writer.WriteElementString("Phone",pat.HmPhone);
				writer.WriteElementString("Mobile",pat.WirelessPhone);
				writer.WriteElementString("SocialID",pat.SSN);
				if(pat.Gender==PatientGender.Female){
					writer.WriteElementString("Gender","Female");
				}
				else{
					writer.WriteElementString("Gender","Male");
				}
				writer.WriteEndElement();//Patient
				writer.WriteEndElement();//LinkageParameter
				writer.Flush();
				writer.Close();
				try {
					ODFileUtils.WriteAllTextThenStart(linkage,strb.ToString(),path);
					return;
				}
				catch(Exception e) {
					e.DoNothing();
				}
			}
			try {
				//whether there is a patient or not.
				ODFileUtils.ProcessStart(path);
			} 
			catch {
				MessageBox.Show("Error launching "+path);
			}
		}

	}
}










