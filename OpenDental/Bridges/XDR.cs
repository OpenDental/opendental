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
using System.Linq;

namespace OpenDental.Bridges{
	///<summary>Dexis bridge was no longer satisfactory for this bridge.</summary>
	public class XDR {

		///<summary></summary>
		public XDR(){
			
		}

		///<summary></summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			List<ProgramProperty> listXDRProperties=ProgramProperties.GetListForProgramAndClinicWithDefault(ProgramCur.ProgramNum,Clinics.ClinicNum);
			//Look for a locationID for the current clinic, use that locationID if present else just leave blank
			string locationID=listXDRProperties.FirstOrDefault(x => x.ClinicNum==Clinics.ClinicNum && x.PropertyDesc==XDR.PropertyDescs.LocationID)?.PropertyValue;
			string infoFile=listXDRProperties.FirstOrDefault(x => x.PropertyDesc==XDR.PropertyDescs.InfoFilePath)?.PropertyValue;
			if(infoFile.Trim()=="") {
				if(ODBuild.IsWeb()) {
					MsgBox.Show("XDR","InfoFile path must not be empty.");
					return;
				}
				infoFile=CodeBase.ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),"infofile.txt");
			}
			if(pat!=null) {
				try {
					//XDR's PatientID can be any string format, max 8 char.
					//There is no validation to ensure that length is 8 char or less.
					string id="";
					//If we can exactly match the pat/chart program link to ChartNum, use ChartNum, for all other cases fall back on PatNum as a fail safe.
					if(listXDRProperties.FirstOrDefault(x => x.PropertyDesc==XDR.PropertyDescs.PatNumOrChartNum)?.PropertyValue=="1") {
						id=pat.ChartNumber;
					}
					else {
						id=pat.PatNum.ToString();
					}
					//Encoding 1252 was specifically requested by the XDR development team to help with accented characters (ex Canadian customers).
					//On 05/19/2015, a reseller noticed UTF8 encoding in the Dexis bridge caused a similar issue.
					//06/01/2015 A customer tested and confirmed that using the XDR bridge and thus coding page 1252, solved the special characters issue.
					Encoding enc=Encoding.GetEncoding(1252);
					using MemoryStream memStream=new MemoryStream();
					using(StreamWriter sw=new StreamWriter(memStream,enc)) {
						sw.WriteLine($"{pat.LName}, {pat.FName}  {pat.Birthdate.ToShortDateString()}  ({id})");
						sw.WriteLine($"PN={id}");
						sw.WriteLine($"LN={pat.LName}");
						sw.WriteLine($"FN={pat.FName}");
						sw.WriteLine($"BD={pat.Birthdate.ToShortDateString()}");
						if(pat.Gender==PatientGender.Female) {
							sw.WriteLine("SX=F");
						}
						else {
							sw.WriteLine("SX=M");
						}
						sw.WriteLine($"LO={locationID}");
						sw.WriteLine($"UN={Security.CurUser.UserName}");
					}
					ODFileUtils.WriteAllBytesThenStart(infoFile,memStream.ToArray(),path,"\"@"+infoFile+"\"");
				}
				catch {
					MessageBox.Show("Error writing to infoFile.");
				}
			}
			else {
				try {
					ODFileUtils.ProcessStart(path);//should start XDR without bringing up a pt.
				}
				catch {
					MessageBox.Show(path+" is not available.");
				}
			}	
		}

		public class PropertyDescs {
			public static string InfoFilePath="InfoFile path";
			public static string PatNumOrChartNum="Enter 0 to use PatientNum, or 1 to use ChartNum";
			public static string LocationID="Location ID";
		}
	}

}







