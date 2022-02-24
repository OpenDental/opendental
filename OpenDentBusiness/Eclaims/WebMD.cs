using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeBase;
using Ionic.Zip;

namespace OpenDentBusiness.Eclaims {
	public class WebMD {

		public static string ErrorMessage="";

		///<summary>Used for both sending claims and receiving reports.  Set isSilent to true to hide the cmd window popup.
		///Returns true if the communications were successful, and false if they failed.  If they failed, a rollback will happen automatically by 
		///deleting the previously created X12 file.  The batchnum is supplied for the possible rollback.  Also used for mail retrieval.</summary>
		public static bool Launch(Clearinghouse clearinghouseClin,int batchNum,bool isSilent=false,IODProgressExtended progress=null) {
			//called from Eclaims and FormClaimReports.cs. Clinic-level clearinghouse passed in.
			progress=progress??new ODProgressExtendedNull();
			string arguments="";
			try {
				if(!Directory.Exists(clearinghouseClin.ExportPath)) {
					throw new Exception(Lans.g(progress.LanThis,"Clearinghouse export path is invalid."));
				}
				if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
					throw new Exception(Lans.g(progress.LanThis,"Clearinghouse response path is invalid."));
				}
				if(!File.Exists(clearinghouseClin.ClientProgram)) {
					throw new Exception(Lans.g(progress.LanThis,"Client program not installed properly."));
				}
				arguments="\""+ODFileUtils.RemoveTrailingSeparators(clearinghouseClin.ExportPath)+"\\"+"*.*\" "//upload claims path
					+"\""+ODFileUtils.RemoveTrailingSeparators(clearinghouseClin.ResponsePath)+"\" "//Mail path
					+"316 "//vendor number.  
								 //LoginID is client number.  Assigned by us, and we have to coordinate for all other 'vendors' of Open Dental,
								 //because there is only one vendor number for OD for now.
					+clearinghouseClin.LoginID+" "
					+clearinghouseClin.Password;
				//call the WebMD client program
				Process process=new Process();
				process.EnableRaisingEvents=true;
				if(isSilent) {
					process.StartInfo.UseShellExecute=false;//Required to redirect standard input on the next line.
					process.StartInfo.RedirectStandardInput=true;//Required to send a newline character into the input stream below.
					process.StartInfo.CreateNoWindow=true;
					process.StartInfo.WindowStyle=ProcessWindowStyle.Hidden;
				}
				process.StartInfo.FileName=clearinghouseClin.ClientProgram;
				process.StartInfo.Arguments=arguments;
				progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server and downloading reports"),"reports","17%",17);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				process.Start();
				if(isSilent) {
					//If the LoginID or password are incorrect, then the WebMD client program will show an error message and wait for the user to click enter.
					//Above we redirected standard input so that we could send a newline into the input.
					//This way if the LoginID or password are incorrect, then the WebMD client will still exit.
					//Write an excessive amount of newlines in case the WebMD client asks multiple questions.
					//If we send the input before the WebMD client is ready, then the input is queued until it is needed.
					//If no input is needed, then the input will be ignored.
					for(int i = 0;i<10;i++) {
						process.StandardInput.WriteLine();
					}
				}
				process.WaitForExit();
				//delete the uploaded claims
				progress.UpdateProgress(Lans.g(progress.LanThis,"Contacting web server sucessful."));
				progress.UpdateProgress(Lans.g(progress.LanThis,"Deleting uploaded claims"),"reports","33%",33);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				string[] files=Directory.GetFiles(clearinghouseClin.ExportPath);
				for(int i=0;i<files.Length;i++) {
					float overallpercent=33+(i/files.Length)*11;//33 is starting point. 11 is the amount of bar space we have before our next major spot (44%)
					progress.UpdateProgress(Lans.g(progress.LanThis,"Getting files"),"reports",overallpercent+"%",(int)overallpercent);
					//string t=files[i];
					File.Delete(files[i]);
				}
				//rename the downloaded mail files to end with txt
				progress.UpdateProgress(Lans.g(progress.LanThis,"Deleteing uploaded claims successful."));
				progress.UpdateProgress("Renaming downloaded files","reports","44%",44);
				if(progress.IsPauseOrCancel()) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
					return false;
				}
				files=Directory.GetFiles(clearinghouseClin.ResponsePath);
				for(int i=0;i<files.Length;i++) {
					float overallpercent=44+(i/files.Length)*11;//44 is starting point. 11 is the amount of bar space we have before our next major spot (55%)
					progress.UpdateProgress(Lans.g(progress.LanThis,"Getting files"),"reports",overallpercent+"%",(int)overallpercent);
					//string t=files[i];
					if(Path.GetExtension(files[i])!=".txt") {
						File.Move(files[i],files[i]+".txt");
					}
				}
				progress.UpdateProgress(Lans.g(progress.LanThis,"File rename successful."));
			}
			catch(Exception e) {
				ErrorMessage=e.Message;
				progress.UpdateProgress(Lans.g(progress.LanThis,"Error encountered:")+"\r\n"+ErrorMessage);
				if(batchNum!=0) {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Rolling back batch."));
					progress.UpdateProgressDetailed(Lans.g(progress.LanThis,"Rolling back batch"),tagString:"reports",marqSpeed:20,progStyle:ProgBarStyle.Marquee);
					x837Controller.Rollback(clearinghouseClin,batchNum);
					progress.UpdateProgressDetailed(Lans.g(progress.LanThis,"Done rolling back"),tagString:"reports",marqSpeed:20,progStyle:ProgBarStyle.Marquee);
					progress.UpdateProgress(Lans.g(progress.LanThis,"Rolling back batch complete."));		
				}
				return false;
			}
			return true;
		}


		private const string vendorId="";//TODO: Get from Emdeon!
		private const string testMode="true";//TODO: Set to 'false' on release.
		private const string emdeonServerUrl="";//TODO: Get from Emdeon!

		///<summary>If you call this, make sure you pass in a clinic-level clearinghouse if one exists. </summary>
		private static void SubmitBatch(Clearinghouse clearinghouseClin,int batchNum) {//not called from anywhere currently.
			string[] files=Directory.GetFiles(clearinghouseClin.ExportPath);
			for(int i=0;i<files.Length;i++) {
				ZipFile zip=null;
				try {
					zip=new ZipFile();
					zip.AddFile(files[i]);
					MemoryStream ms=new MemoryStream();
					zip.Save(ms);
					string fileTextZippedBase64=Convert.ToBase64String(ms.GetBuffer());
					FileInfo fi=new FileInfo(files[i]);
					string claimXML="<?xml version=\"1.0\" ?>"
						+"<claim_submission_api xmlns=\"Emdeon_claim_submission_api\" revision=\"001\">"
							+"<authentication>"
								+"<vendor_id>"+vendorId+"</vendor_id>"
								+"<user_id>"+clearinghouseClin.LoginID+"</user_id>"
								+"<password>"+clearinghouseClin.Password+"</password>"
							+"</authentication>"
							+"<transaction>"
							+"<trace_id>"+batchNum+"</trace_id>"//TODO: Is this the right number to use?
							+"<trx_type>submit_claim_file_request</trx_type>"
							+"<test_mode>"+testMode+"</test_mode>"
							+"<trx_data>"
								+"<claim_file>"
									+"<file_name>"+Path.GetFileName(files[i])+"</file_name>"
									+"<file_format>DCDS2</file_format>"
									+"<file_size>"+fi.Length+"</file_size>"
									+"<file_compression>pkzip</file_compression>"
									+"<file_encoding>base64</file_encoding>"
									+"<file_data>"+fileTextZippedBase64+"</file_data>"
								+"</claim_file>"
							+"</trx_data>"
						+"</transaction>"
					+"</claim_submission_api>";
					byte[] claimXMLbytes=Encoding.UTF8.GetBytes(claimXML);
					WebClient myWebClient=new WebClient();
					myWebClient.Headers.Add("Content-Type","text/xml");
					byte[] responseBytes=myWebClient.UploadData(emdeonServerUrl,claimXMLbytes);




				}
				finally {
					if(zip!=null) {
						zip.Dispose();
					}
				}
			}
		}

		///<summary>Sends an X12 270 request and returns X12 271 response or an error message.</summary>
		public static string Benefits270(Clearinghouse clearhouse,string x12message) {
			x12message=Regex.Replace(x12message,"\r\n","");//Emdeon specifically requires that each segment ends with ~ and does not include a newline.
			string retVal="";
			string url="https://eligibility.webmddental.com/WebMD_Eligibility_002/Request_002X12.aspx";
			//We used the WebClient class here, because that is what they used in their example Visual Basic code.
			WebClient client=new WebClient();
			try {
				byte[] postData=Encoding.ASCII.GetBytes(x12message);
				byte[] response=client.UploadData(url,postData);
				retVal=Encoding.ASCII.GetString(response);
			}
			catch(Exception ex) {
				retVal=ex.Message;
			}
			return retVal;
		}

	}
}
