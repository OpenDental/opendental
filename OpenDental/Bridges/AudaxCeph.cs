using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Xml;
using CodeBase;

namespace OpenDental.Bridges {
	///<summary></summary>
	public class AudaxCeph {

		///<summary></summary>
		public AudaxCeph() {

		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(!File.Exists(path)) {
				MessageBox.Show(path+" could not be found.");
				return;
			}
			Process[] checkProg=Process.GetProcessesByName("AxCeph");
			string dir=Path.GetDirectoryName(path);
			string updatexml=CodeBase.ODFileUtils.CombinePaths(dir,"update.xml");
			if(File.Exists(updatexml)) {
				File.Delete(updatexml);
			}
			if(checkProg.Length==0) {
				MessageBox.Show("AxCeph.exe not found. Please make sure AudaxCeph is running and try again.");
			}
			else if(pat==null) {
				ODFileUtils.ProcessStart(path);
			}
			else {
				StringBuilder strb=new StringBuilder();
				XmlWriterSettings settings=new XmlWriterSettings();
				settings.Indent=true;
				settings.IndentChars="   ";
				settings.NewLineChars="\r\n";
				settings.OmitXmlDeclaration=true;
				XmlWriter writer=XmlWriter.Create(strb,settings);
				writer.WriteProcessingInstruction("xml","version='1.0' encoding='utf-8'");
				writer.WriteStartElement("AxCephComData");
				writer.WriteStartElement("Patients");
				writer.WriteStartElement("Patient");
				writer.WriteElementString("PIDOutside",pat.PatNum.ToString());
				writer.WriteElementString("NameOfPatient",Tidy(pat.LName)+", "+Tidy(pat.FName));
				writer.WriteElementString("DateOfBirth",pat.Birthdate.ToString("yyyyMMdd"));
				if(pat.Gender.ToString()=="Female")	{
					writer.WriteElementString("Sex","F");
				}
				else {
					writer.WriteElementString("Sex","M");
				}
				writer.WriteElementString("Active","1");
				writer.WriteEndElement();//Patient
				writer.WriteEndElement();//Patients
				writer.WriteElementString("Command","UpdateOrInsertPatient");
				writer.WriteElementString("ResultXMLFileName","result.xml");
				writer.WriteElementString("ResultStatus","");
				writer.WriteElementString("ResultMessage","0");
				writer.WriteEndElement();//AxCephComData
				writer.Flush();
				writer.Close();
				try {
					ODFileUtils.WriteAllTextThenStart(updatexml,strb.ToString(),path," update.xml");
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
		}

		///<summary>Removes unnecessary characters and spaces.</summary>
		private static string Tidy(string input) {
			//Get rid of any character that isn't A-Z, a hyphen or a period.
			string retVal=input.Replace(";","");
			retVal=retVal.Replace(" ","");
			retVal=retVal.Replace("1","");
			retVal=retVal.Replace("2","");
			retVal=retVal.Replace("3","");
			retVal=retVal.Replace("4","");
			retVal=retVal.Replace("5","");
			retVal=retVal.Replace("6","");
			retVal=retVal.Replace("7","");
			retVal=retVal.Replace("8","");
			retVal=retVal.Replace("9","");
			retVal=retVal.Replace("0","");
			retVal=retVal.Replace("_","");
			retVal=retVal.Replace("+","");
			retVal=retVal.Replace("=","");
			retVal=retVal.Replace("!","");
			retVal=retVal.Replace("@","");
			retVal=retVal.Replace("#","");
			retVal=retVal.Replace("$","");
			retVal=retVal.Replace("%","");
			retVal=retVal.Replace("^","");
			retVal=retVal.Replace("&","");
			retVal=retVal.Replace("*","");
			retVal=retVal.Replace("(","");
			retVal=retVal.Replace(")","");
			retVal=retVal.Replace("`","");
			retVal=retVal.Replace("~","");
			retVal=retVal.Replace("[","");
			retVal=retVal.Replace("]","");
			retVal=retVal.Replace("{","");
			retVal=retVal.Replace("}","");
			retVal=retVal.Replace("\\","");
			retVal=retVal.Replace("|","");
			retVal=retVal.Replace("/","");
			retVal=retVal.Replace(":","");
			retVal=retVal.Replace("'","");
			retVal=retVal.Replace("\"","");
			retVal=retVal.Replace("<","");
			retVal=retVal.Replace(">","");
			retVal=retVal.Replace(",","");
			retVal=retVal.Replace("?","");
			return retVal;
		}

	}
}







