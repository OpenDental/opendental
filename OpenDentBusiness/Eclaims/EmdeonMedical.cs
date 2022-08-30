using System;
using System.Collections.Generic;
using System.IO;
using OpenDentBusiness;
using Ionic.Zip;
using CodeBase;

namespace OpenDentBusiness.Eclaims {
	public class EmdeonMedical{
		
		private static string emdeonITSUrlTest="https://cert.its.changehealthcare.com/ITS/itsws.asmx";//test url
		private static string emdeonITSUrl="https://its.changehealthcare.com/ITS/ITSWS.asmx";//production url
		public static string ErrorMessage="";

		///<summary></summary>
		public EmdeonMedical()
		{			
		}

		///<summary>Returns true if the communications were successful, and false if they failed. If they failed, a rollback will happen automatically by deleting the previously created X12 file. The batchnum is supplied for the possible rollback.  Also used for mail retrieval.</summary>
		public static bool Launch(Clearinghouse clearinghouseClin,int batchNum,EnumClaimMedType medType){ //called from Eclaims.cs. Clinic-level clearinghouse passed in.
			string batchFile="";
			try {
				if(!Directory.Exists(clearinghouseClin.ExportPath)) {
					throw new ODException("Clearinghouse export path is invalid.");
				}
				//We make sure to only send the X12 batch file for the current batch, so that if there is a failure, then we know
				//for sure that we need to reverse the batch. This will also help us avoid any exterraneous/old batch files in the
				//same directory which might be left if there is a permission issue when trying to delete the batch files after processing.
				batchFile=Path.Combine(clearinghouseClin.ExportPath,"claims"+batchNum+".txt");
				//byte[] fileBytes=File.ReadAllBytes(batchFile);//unused
				MemoryStream zipMemoryStream=new MemoryStream();
				ZipFile tempZip=new ZipFile();
				tempZip.AddFile(batchFile,"");
				tempZip.Save(zipMemoryStream);
				tempZip.Dispose();
				zipMemoryStream.Position=0;
				byte[] zipFileBytes=zipMemoryStream.GetBuffer();
				string zipFileBytesBase64=Convert.ToBase64String(zipFileBytes);
				zipMemoryStream.Dispose();
				bool isTest=(clearinghouseClin.ISA15=="T");
				string messageType=(isTest?"MCT":"MCD");//medical
				if(medType==EnumClaimMedType.Institutional) {
					messageType=(isTest?"HCT":"HCD");
				}
				else if(medType==EnumClaimMedType.Dental) {
					//messageType=(isTest?"DCT":"DCD");//not used/tested yet, but planned for future.
				}
				EmdeonITS.ITSWS itsws=new EmdeonITS.ITSWS();
				itsws.Url=(isTest?emdeonITSUrlTest:emdeonITSUrl);
				EmdeonITS.ItsReturn response=itsws.PutFileExt(clearinghouseClin.LoginID,clearinghouseClin.Password,messageType,Path.GetFileName(batchFile),zipFileBytesBase64);
				if(response.ErrorCode!=0) { //Batch submission successful.
					throw new ODException("Emdeon rejected all claims in the current batch file "+batchFile+". Error number from Emdeon: "+response.ErrorCode+". Error message from Emdeon: "+response.Response);
				}
			}
			catch(Exception e) {
				ErrorMessage=e.Message;
				x837Controller.Rollback(clearinghouseClin,batchNum);
				return false;
			}
			finally {
				try {
					if(batchFile!="") {
						File.Delete(batchFile);
					}
				}
				catch {
					ErrorMessage="Failed to remove batch file"+batchFile+". Probably due to a permission issue.  Check folder permissions and manually delete.";
				}
			}
			return true;
		}

		///<summary>Throws exceptions.</summary>
		public static bool Retrieve(Clearinghouse clearinghouseClin,IODProgressExtended progress=null) {//called from FormClaimReports. clinic-level clearinghouse passed in.
			progress=progress??new ODProgressExtendedNull();
			try {
				if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
					throw new ODException(Lans.g(progress.LanThis,"Clearinghouse response path is invalid."));
				}
				progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server"),"reports","17%",17);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				bool reportsDownloaded=false;
				bool isTest=(clearinghouseClin.ISA15=="T");
				//See guide pages 21 through 25 for "batch download message types"
				List <EmdeonMedicalReportType> listReportTypes=new List<EmdeonMedicalReportType>();
				//Also known and Institutional Claims.  Might not need this report type, but leaving here for backwards compatibility.
				listReportTypes.Add(new EmdeonMedicalReportType("Hospital Claims","HCTG","HCDG","HCTD","HCDD"));
				//Might not need this report type, but leaving here for backwards compatibility.
				listReportTypes.Add(new EmdeonMedicalReportType("Medical Claims","MCTG","MCDG","MCTD","MCDD"));
				listReportTypes.Add(new EmdeonMedicalReportType("Automated Verification (270,276,278)","AETG","AEVG","AETD","AEVD"));
				if(clearinghouseClin.IsEraDownloadAllowed!=EraBehaviors.None) {//ERA download is enabled.
					listReportTypes.Add(new EmdeonMedicalReportType("Electronic Remittance Acceptance (ERA) 835","EDT","EDP","EAT","EAP"));
				}
				progress.UpdateProgress(Lans.g(progress.LanThis,"Downloading files"),"reports","33%",33);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				for(int i=0;i<listReportTypes.Count;i++) {
					float overallpercent=33+(i/listReportTypes.Count)*17;//33 is starting point. 17 is the amount of bar space we have before our next major spot (50%)
					progress.UpdateProgress(Lans.g(progress.LanThis,"Downloading files"),"reports",overallpercent+"%",(int)overallpercent);
					EmdeonITS.ITSWS itsws=new EmdeonITS.ITSWS();
					itsws.Url=(isTest?emdeonITSUrlTest:emdeonITSUrl);
					EmdeonMedicalReportType reportType=listReportTypes[i];
					//Download the most up to date reports, but do not delete them from the server yet.
					EmdeonITS.ItsReturn response=itsws.GetFile(clearinghouseClin.LoginID,clearinghouseClin.Password,reportType.GetCodeForGetReport(isTest));
					//Change Health Care ITS User Guide 3.0.6B_20160107.pdf, page 20 for GetFile() states:
					//"If ErrorCode is zero, Response will contain a Base64 encoded string of the binary download content or a text based message, depending on
					//the type of download requested (see discussions below regarding Message Types and Client File Acknowledgement). If the Response is a
					//Base64 encoded string, the client application must perform Base64 decoding on the Response. The result will be the binary content of a
					//PK-Zip compatible file which can then be written to disk. If ErrorCode is greater than zero,
					//Response will contain a text based error message"
					if(response.ErrorCode==0) { //Report retrieval successful.
						string reportFileDataBase64=response.Response;
						byte[] reportFileDataBytes=Convert.FromBase64String(reportFileDataBase64);
						string reportFilePath=CodeBase.ODFileUtils.CreateRandomFile(clearinghouseClin.ResponsePath,".zip");
						File.WriteAllBytes(reportFilePath,reportFileDataBytes);
						string errorMsg="";
						ZipFile zipFile=null;
						try {
							zipFile=new ZipFile(reportFilePath);//Open zip file.
							zipFile.ExtractAll(clearinghouseClin.ResponsePath);
						}
						catch(Exception ex) {
							errorMsg=Lans.g(progress.LanThis,"Report zip file downloaded but could not be extracted.")+"\r\n"
								+"File: "+reportFilePath+"\r\n"
								+ex.Message;
						}
						try {
							if(zipFile!=null) {
								zipFile.Dispose();//This releases the file handle created when opening the file above.
							}
							File.Delete(reportFilePath);
						}
						catch(Exception ex) {
							ex.DoNothing();//Could not be deleted.  Not a big deal, because at least we were able to extract it, making it available for import.
						}
						if(errorMsg!="") {
							throw new ODException(errorMsg);
						}
						reportsDownloaded=true;
						//Now that the file has been saved, remove the report file from the Emdeon production server.
						//If deleting the report fails, we don't care because that will simply mean that we download it again next time.
						//Thus we don't need to check the status after this next call.
						progress.UpdateProgress(Lans.g(progress.LanThis,"Removing report file from server"));
						itsws.GetFile(clearinghouseClin.LoginID,clearinghouseClin.Password,reportType.GetCodeForDeleteReport(isTest));
					}
					else if(response.ErrorCode!=209) {//Error 209 means mailbox is empty for requested report type.
						throw new ODException(Lans.g(progress.LanThis,"Failed to get reports. Error number from Emdeon:")+" "+response.ErrorCode+". "
							+Lans.g(progress.LanThis,"Error message from Emdeon: ")+response.Response);
					}
				}
				progress.UpdateProgress(Lans.g(progress.LanThis,"Download successful."));
				progress.UpdateProgress(Lans.g(progress.LanThis,"Finalizing"),"reports","50%",50);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				if(!reportsDownloaded) {
					ErrorMessage=Lans.g(progress.LanThis,"Report mailbox is empty.");
				}
			}
			catch(Exception ex) {
				ErrorMessage=ex.Message; 
				return false;
			}
			return true;
		}

	}

	///<summary>See guide pages 21 through 25 for "batch download message types"</summary>
	internal class EmdeonMedicalReportType {
		///<summary>Conveys intent for internal documentation.  Mostly used for debugging.</summary>
		public string Description;
		///<summary>Code to retrieve reports from the test web service.</summary>
		public string TestGetCode;
		///<summary>Code to retrieve reports from the live web service.</summary>
		public string ProductionGetCode;
		///<summary>Code to remove reports from the test web service.</summary>
		public string TestDeleteCode;
		///<summary>Code to remove reports from the live web service.</summary>
		public string ProductionDeleteCode;

		public EmdeonMedicalReportType(string description,string testGetCode,string productionGetCode,string testDeleteCode,string productionDeleteCode){
			Description=description;
			TestGetCode=testGetCode;
			ProductionGetCode=productionGetCode;
			TestDeleteCode=testDeleteCode;
			ProductionDeleteCode=productionDeleteCode;
		}

		public string GetCodeForGetReport(bool isTest) {
			if(isTest) {
				return TestGetCode;
			}
			return ProductionGetCode;
		}

		public string GetCodeForDeleteReport(bool isTest) {
			if(isTest) {
				return TestDeleteCode;
			}
			return ProductionDeleteCode;
		}

	}

}