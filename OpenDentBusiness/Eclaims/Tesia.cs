using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDentBusiness.Eclaims
{
	/// <summary></summary>
	public class Tesia{
		///<summary></summary>
		public Tesia(){
			
		}
		/*
		///<summary>Returns true if the communications were successful, and false if they failed. If they failed, a rollback will happen automatically by deleting the previously created X12 file. The batchnum is supplied for the possible rollback.</summary>
		public static bool Launch(Clearinghouse clearhouse,int batchNum){
			if(!Directory.Exists(clearhouse.ExportPath)){
				MsgBox.Show("Tesia","Could not find export path.  Please check clearinghouse paths.");
				return false;
			}
			string[] filenames=Directory.GetFiles(clearhouse.ExportPath);
			if(filenames.Length==0){
				MsgBox.Show("Tesia","No files are present to upload.");
				return false;
			}
			else if(filenames.Length>1){
				for(int i=0;i<filenames.Length;i++){
					File.Delete(filenames[i]);
				}
				MsgBox.Show("Tesia","Multiple files were present for upload, where only one was expected.  They have now been deleted.  Please try upload again.");
				return false;
			}
			string oldFileNameFull=filenames[0];//fullyqualified
			if(Path.GetExtension(oldFileNameFull)!=".txt"){
				MsgBox.Show("Tesia","File does not end in '.txt' as expected.");
				return false;
			}
			string fileNameFull=oldFileNameFull.Substring(0,oldFileNameFull.Length-4)+".837";
			try{
				File.Move(oldFileNameFull,fileNameFull);
			}
			catch(Exception e){
				MessageBox.Show(e.Message);
				File.Delete(oldFileNameFull);
				return false;
			}
			//encrypt
			//example usage: "C:\temp\7z.exe" a -tzip -pmypass "C:\temp\temp.zip" "C:\temp\temp.txt"
			string zipNameFull=fileNameFull.Substring(0,fileNameFull.Length-4)+".zip";
			string arguments="a -tzip -p"+clearhouse.Password+" \""+zipNameFull+"\" \""+fileNameFull+"\"";
			Process.Start("7z.exe",arguments);
			File.Delete(fileNameFull);
			string fileNameOnly=Path.GetFileName(zipNameFull);
			//upload
			try{
				FtpWebRequest request;
				FtpWebResponse response;
				Stream streamRequest;
				FileStream streamRead;
				string ftpsite="ftp://ftp.realtimeclaims.com";
				string inFolder=ftpsite+"/test/in";
				string inFile=inFolder+"/"+fileNameOnly;
				request=(FtpWebRequest)WebRequest.Create(inFile);
				request.Credentials=new NetworkCredential(clearhouse.LoginID,clearhouse.Password);
				request.Method=WebRequestMethods.Ftp.UploadFile;
				streamRequest=request.GetRequestStream();
				const int bufferLength = 2048;
				byte[] buffer = new byte[bufferLength];
				int readBytes = 0;
				streamRead=File.OpenRead(zipNameFull);
				do{
					readBytes = streamRead.Read(buffer,0,bufferLength);
					streamRequest.Write(buffer,0,readBytes);
				}
				while(readBytes != 0);
				streamRequest.Close();				
				response=(FtpWebResponse)request.GetResponse();
			}
			catch(Exception e) {
				MessageBox.Show(e.Message);
				File.Delete(zipNameFull);
				return false;
			}
			return true;
		}*/

		public static void Eligibility270(){

		}

		public static void GetReports(){

		}

		


	}
}
