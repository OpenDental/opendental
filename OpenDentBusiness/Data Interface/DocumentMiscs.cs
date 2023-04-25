using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class DocumentMiscs{
		public static DocumentMisc GetByTypeAndFileName(string fileName,DocumentMiscType docMiscType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<DocumentMisc>(MethodBase.GetCurrentMethod(),fileName,docMiscType);
			}
			string command="SELECT * FROM documentmisc "
				+"WHERE DocMiscType="+POut.Int((int)docMiscType)+" "
				+"AND FileName='"+POut.String(fileName)+"'";
			return Crud.DocumentMiscCrud.SelectOne(command);
		}

		public static List<DocumentMisc> GetByType(DocumentMiscType docMiscType,bool canIncludeRawBase64=false,string nameStartWith="") {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DocumentMisc>>(MethodBase.GetCurrentMethod(),docMiscType,canIncludeRawBase64,nameStartWith);
			}
			string command="SELECT ";
			if(canIncludeRawBase64) {
				command+="* ";
			}
			else {
				command+="DocMiscNum,DateCreated,FileName,DocMiscType,'' RawBase64 ";
			}
			command+="FROM documentmisc WHERE DocMiscType="+POut.Int((int)docMiscType);
			if(!string.IsNullOrEmpty(nameStartWith)) {
				command+=" AND FileName LIKE '"+POut.String(nameStartWith)+"%'";
			}
			return Crud.DocumentMiscCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(DocumentMisc documentMisc) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				documentMisc.DocMiscNum=Meth.GetLong(MethodBase.GetCurrentMethod(),documentMisc);
				return documentMisc.DocMiscNum;
			}
			return Crud.DocumentMiscCrud.Insert(documentMisc);
		}

		///<summary>Appends the passed in rawBase64 string to the RawBase64 column in the db for the UpdateFiles DocMiscType row.</summary>
		public static void AppendRawBase64ForUpdateFiles(string rawBase64) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),rawBase64);
				return;
			}
			string command="UPDATE documentmisc SET RawBase64=CONCAT("+DbHelper.IfNull("RawBase64","")+","+DbHelper.ParamChar+"paramRawBase64) "
				+"WHERE DocMiscType="+POut.Int((int)DocumentMiscType.UpdateFiles);
			OdSqlParameter paramRawBase64=new OdSqlParameter("paramRawBase64",OdDbType.Text,rawBase64);
			Db.NonQ(command,paramRawBase64);
		}

		///<summary></summary>
		public static void DeleteAllForType(DocumentMiscType docMiscType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),docMiscType);
				return;
			}
			string command="DELETE FROM documentmisc WHERE DocMiscType="+POut.Int((int)docMiscType);
			Db.NonQ(command);
		}

		public static void DeleteMany(List<long> listDocMiscNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDocMiscNums);
				return;
			}
			Crud.DocumentMiscCrud.DeleteMany(listDocMiscNums);
		}

		///<summary>Returns a non-empty and translated error message on failure. Returns null on success.</summary>
		public static string LaunchShareScreen() {
			//No need to check MiddleTierRole; no call to db.
			string shareScreenUriBase="https://opendental.com/resources/OpenDentalShareScreen";
			string shareScreenVersion="";
			try {
				XmlDocument xmlDocument=new XmlDocument();
				xmlDocument.Load(shareScreenUriBase+".xml");
				XmlNode xmlNodeRoot=xmlDocument.ChildNodes[0];
				XmlNode xmlNodeVersion=xmlNodeRoot.ChildNodes.AsEnumerable<XmlNode>().FirstOrDefault(x => x.Name.ToLower()=="version");
				shareScreenVersion=xmlNodeVersion.InnerText;
			}
			catch(Exception ex) {
				return Lans.g("DocumentMiscs","Could not download share screen configuration file")+": "+ex.Message;
			}
			string exeUri=shareScreenUriBase+".exe";
			string exeName=Path.GetFileName(exeUri);
			string exeFilePath=ODFileUtils.CombinePaths(ODFileUtils.GetProgramDirectory(),exeName);
			bool isDownloadNeeded=true;
			if(File.Exists(exeFilePath)) {
				try {
					FileVersionInfo fileVersionInfo=FileVersionInfo.GetVersionInfo(exeFilePath);
					if(fileVersionInfo?.FileVersion==shareScreenVersion) {//FileVersion can be null if exe is corrupt or empty from failed previous download.
						isDownloadNeeded=false;
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			if(isDownloadNeeded) {//local exe is missing, corrupt, or an old version.
				DocumentMisc docMiscExisting=GetByType(DocumentMiscType.ShareScreenExeSegment,nameStartWith:shareScreenVersion)
					.FirstOrDefault(x => x.FileName.EndsWith("}"));//Empty segment indicating a complete exe ends with }
				if(docMiscExisting==null) {
					//A complete OpenDentalShareScreen.exe of this version does not exist in the database yet. Download from website.
					try {
						WebClient client=new WebClient();
						client.DownloadFile(exeUri,exeFilePath);
						client.Dispose();
					}
					catch(Exception ex) {
						return Lans.g("DocumentMiscs","Could not download "+exeName+" from website")+":\r\n"
							+ex.Message+"\r\n"
							+Lans.g("DocumentMiscs","Close any existing "+exeName+" instances, check permissions, and try again.");
					}
					try {
						//Make the exe available for the local network to prevent additional web downloads.
						int charsPerPayload=MiscData.GetMaxAllowedPacket()-8192;//Arbitrarily subtracted 8KB from max allowed bytes for MySQL "header" information.
						charsPerPayload=Math.Min(charsPerPayload,1048575);//1048575 is divisible by 3 which is important for Base64 "appending" logic.
						charsPerPayload=(3*charsPerPayload)/4;//Scale down to account for bloating when converting to base64.
						charsPerPayload-=(charsPerPayload % 3);//Use the closest amount of bytes divisible by 3 for compatibility with base64 enconding.
						byte[] arrayBufferBytes=new byte[charsPerPayload];
						FileStream fileStream=File.OpenRead(exeFilePath);
						int segmentCount=0;
						DateTime dateCreated=DateTime.Today;
						Guid guid=Guid.NewGuid();//We need this guid to separate segments from a broader set of segments if multiple workstaions inserted at the same time.
						int readCount=fileStream.Read(arrayBufferBytes,0,arrayBufferBytes.Length);
						while(readCount > 0) {
							DocumentMisc segment=new DocumentMisc();
							segment.DateCreated=dateCreated;
							string segmentCountStr=segmentCount.ToString().PadLeft(5,'0');
							segment.FileName=shareScreenVersion+"{"+guid+"}"+segmentCountStr;
							segmentCount++;
							segment.DocMiscType=DocumentMiscType.ShareScreenExeSegment;
							segment.RawBase64=Convert.ToBase64String(arrayBufferBytes,0,readCount);
							Insert(segment);
							readCount=fileStream.Read(arrayBufferBytes,0,arrayBufferBytes.Length);
						}
						fileStream.Close();
						DocumentMisc lastSegment=new DocumentMisc();//This empty segment indicates that the exe has been completely inserted into the database.
						lastSegment.DateCreated=dateCreated;
						lastSegment.FileName=shareScreenVersion+"{"+guid+"}";
						lastSegment.DocMiscType=DocumentMiscType.ShareScreenExeSegment;
						lastSegment.RawBase64="";
						Insert(lastSegment);
						List<DocumentMisc> listOldSegments=GetByType(DocumentMiscType.ShareScreenExeSegment)
							.FindAll(x => !x.FileName.StartsWith(shareScreenVersion));
						List<long> listDocMiscNums=listOldSegments.Select(x => x.DocMiscNum).ToList();
						DeleteMany(listDocMiscNums);
					}
					catch(Exception ex) {
						ex.DoNothing();//We were able to download locally, which means we can still launch the exe below.
					}
				}
				else {//The exe is pre-existing in the database. Unpack from database instead of downloading from web.
					try {
						List<DocumentMisc> listSegments=
							GetByType(DocumentMiscType.ShareScreenExeSegment,canIncludeRawBase64:true,nameStartWith:docMiscExisting.FileName)
							.OrderBy(x => x.FileName).ToList();
						FileStream fileStream=File.Open(exeFilePath,FileMode.Create);//Creates or overwrites.
						for(int i=0;i<listSegments.Count;i++) {
							byte[] arraySegmentBytes=Convert.FromBase64String(listSegments[i].RawBase64);
							fileStream.Write(arraySegmentBytes,0,arraySegmentBytes.Length);
						}
						fileStream.Close();
					}
					catch(Exception ex) {
						return Lans.g("DocumentMiscs","Could not unpack "+exeName+" from database")+":\r\n"
							+ex.Message+"\r\n"
							+Lans.g("DocumentMiscs","Close any existing "+exeName+" instances, check permissions, and try again.");
					}
				}
			}
			try {
				Process.Start(exeFilePath);
			}
			catch(Exception ex) {
				return Lans.g("DocumentMiscs","Failed to launch "+exeName)+": "+ex.Message;
			}
			return null;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<DocumentMisc> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DocumentMisc>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM documentmisc WHERE PatNum = "+POut.Long(patNum);
			return Crud.DocumentMiscCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(DocumentMisc documentMisc){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				documentMisc.DocMiscNum=Meth.GetLong(MethodBase.GetCurrentMethod(),documentMisc);
				return documentMisc.DocMiscNum;
			}
			return Crud.DocumentMiscCrud.Insert(documentMisc);
		}

		///<summary></summary>
		public static void Update(DocumentMisc documentMisc){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),documentMisc);
				return;
			}
			Crud.DocumentMiscCrud.Update(documentMisc);
		}

		///<summary></summary>
		public static void Delete(long docMiscNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),docMiscNum);
				return;
			}
			string command= "DELETE FROM documentmisc WHERE DocMiscNum = "+POut.Long(docMiscNum);
			Db.NonQ(command);
		}
		*/
	}
}