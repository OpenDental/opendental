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
	public class EzDenti{

		/// <summary></summary>
		public EzDenti() {
			
		}

		///<summary>Sends data for the patient to linkage.xml and launches the program.</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			if(!ODEnvironment.IsCloudServer && !File.Exists(path)) {//If ODCloud, this check is performed by the Cloud Client
					MessageBox.Show(path+" could not be found.");
					return;
			}
			string dir=Path.GetDirectoryName(path);
			string linkage=CodeBase.ODFileUtils.CombinePaths(dir,"linkage.xml");
			if(!ODEnvironment.IsCloudServer && File.Exists(linkage)){//Will never exist for Thinfinity or AppStream version.
				try {
					File.Delete(linkage);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
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
				//Ewoo EZDent requires slashes for date separators: this format string enforces the syntax regardless of user's regional settings.
				writer.WriteElementString("Birthday",pat.Birthdate.ToString("dd'/'MM'/'yyyy"));
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
					ODFileUtils.WriteAllTextThenStart(linkage,strb.ToString(),path,doStartWithoutExtraFile:true);
					return;
				}
				catch(Exception e) {
					if(ODEnvironment.IsCloudServer) {
						//If ODCloud, the Cloud Client will start the process even if writing the extra file fails so the ProcessStart below is redundant.
						MessageBox.Show("Error launching "+path);
						return;
					}
					e.DoNothing();
				}
			}
			try {
				//whether there is a patient or not.
				ODFileUtils.ProcessStart(path,doWaitForODCloudClientResponse:true);
			} 
			catch {
				MessageBox.Show("Error launching "+path);
			}
		}
	}
}










