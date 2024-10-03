using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Thinfinity;

namespace OpenDental {
	///<summary>This class is used to access files in the AtoZ folder. Depending on the storage type in use, it will read/write to a local 
	///location or it will download/upload from the cloud.</summary>
	public class FileAtoZ {

		///<summary>Returns the string contents of the file.</summary>
		public static string ReadAllText(string fileName) {
			if(CloudStorage.IsCloudStorage) {
				UI.ProgressWin progressWin=new UI.ProgressWin();
				progressWin.StartingMessage="Downloading...";
				byte[] byteArray=null;
				progressWin.ActionMain=() => {
					byteArray=CloudStorage.Download(Path.GetDirectoryName(fileName),Path.GetFileName(fileName));
				};
				progressWin.ShowDialog();
				if(progressWin.IsCancelled){//user clicked cancel
					return "";
				}
				return Encoding.UTF8.GetString(byteArray);
			}
			else {//Not cloud
				return File.ReadAllText(fileName);
			}
		}

		///<summary>Writes or uploads the text to the specified file name.</summary>
		public static void WriteAllText(string fileName,string textForFile,string uploadMessage="Uploading file") {
			if(CloudStorage.IsCloudStorage) {
				UI.ProgressWin progressWin=new UI.ProgressWin();
				progressWin.StartingMessage="Uploading...";
				progressWin.ActionMain=() => {
					CloudStorage.Upload(Path.GetDirectoryName(fileName),Path.GetFileName(fileName),Encoding.UTF8.GetBytes(textForFile));
				};
				progressWin.ShowDialog();
				if(progressWin.IsCancelled){
					return;
				}
			}
			else {//Not cloud
				File.WriteAllText(fileName,textForFile);
			}
		}

		///<summary>Gets a list of the files in the specified directory.  Throws exceptions.
		///An absolute path is required, call GetFilesInDirectoryRelative() for passing in a relative AtoZ folder path.</summary>
		public static List<string> GetFilesInDirectory(string folderFullPath) {
			return OpenDentBusiness.FileIO.FileAtoZ.GetFilesInDirectory(folderFullPath);
		}

		///<summary>Gets a list of the files in the specified directory.  Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might return files within the folder '\\server\OpenDentImages\wiki\'.
		///folder can also be set to a relative path like 'wiki\lists\images' which searches '\\server\OpenDentImages\wiki\lists\images'.</summary>
		public static List<string> GetFilesInDirectoryRelative(string folder) {
			return OpenDentBusiness.FileIO.FileAtoZ.GetFilesInDirectoryRelative(folder);
		}

		///<summary>Copies or downloads the file and opens it. acutalFileName should be a full path, displayedFileName should be a file name only.
		///</summary>
		public static void OpenFile(string actualFilePath,string displayedFileName="") {
			try {
				string tempFile;
				if(displayedFileName=="") {
					tempFile=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),Path.GetFileName(actualFilePath));
				}
				else {
					tempFile=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),displayedFileName);
				}
				if(CloudStorage.IsCloudStorage) {
					UI.ProgressWin progressWin=new UI.ProgressWin();
					progressWin.StartingMessage="Downloading...";
					byte[] byteArray=null;
					progressWin.ActionMain=() => {
						byteArray=CloudStorage.Download(Path.GetDirectoryName(actualFilePath),Path.GetFileName(actualFilePath));
					};
					progressWin.ShowDialog();
					if(progressWin.IsCancelled){//user clicked cancel
						return;
					}
					File.WriteAllBytes(tempFile,byteArray);
				}
				else { //Not Cloud
					//We have to create a copy of the file because the name is different.
					//There is also a high probability that the attachment no longer exists if
					//images are stored in the database, since the file will have originally been
					//placed in the temporary directory.			
					File.Copy(actualFilePath,tempFile,true);
				}
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else if(ODCloudClient.IsAppStream) {
					CloudClientL.ExportForCloud(tempFile,doPromptForName:false);
				}
				else {
					Process.Start(tempFile);
				}
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
			}
		}

		///<summary>Use this instead of ODFileUtils.CombinePaths when the path is in the A to Z folder.</summary>
		public static string CombinePaths(params string[] paths) {
			return OpenDentBusiness.FileIO.FileAtoZ.CombinePaths(paths);
		}

		///<summary>Use this instead of ODFileUtils.AppendSuffix when the path is in the A to Z folder.</summary>
		public static string AppendSuffix(string filePath,string suffix) {
			return OpenDentBusiness.FileIO.FileAtoZ.AppendSuffix(filePath,suffix);
		}

		///<summary>Returns true if the file exists. If cloud, checks if the file exists in the cloud.</summary>
		public static bool Exists(string filePath) {
			return OpenDentBusiness.FileIO.FileAtoZ.Exists(filePath);
		}

		///<summary>Returns null if the the image could not be downloaded or the user canceled the download.</summary>
		public static Bitmap GetImage(string imagePath) {
			if(CloudStorage.IsCloudStorage) {
				UI.ProgressWin progressWin=new UI.ProgressWin();
				progressWin.StartingMessage="Downloading...";
				byte[] byteArray=null;
				progressWin.ActionMain=() => {
					byteArray=CloudStorage.Download(Path.GetDirectoryName(imagePath),Path.GetFileName(imagePath));
				};
				progressWin.ShowDialog();
				if(byteArray==null || byteArray.Length<2){
					return null;
				}
				using(MemoryStream stream=new MemoryStream(byteArray)) {
					return new Bitmap(stream);
				}
			}
			else {//Not cloud
				return new Bitmap(imagePath);
			}
		}

		///<summary>This function will throw if the Process fails to start. Catch in calling class. Runs the file. 
		///Downloads it from the cloud if necessary.</summary>
		public static void StartProcess(string fileFullPath) {
			string filePathToOpen;
			if(CloudStorage.IsCloudStorage) {
				UI.ProgressWin progressWin=new UI.ProgressWin();
				progressWin.StartingMessage="Downloading...";
				byte[] byteArray=null;
				progressWin.ActionMain=() => {
					byteArray=CloudStorage.Download(Path.GetDirectoryName(fileFullPath),Path.GetFileName(fileFullPath));
				};
				progressWin.ShowDialog();
				if(progressWin.IsCancelled){//user clicked cancel
					return;
				}
				filePathToOpen=PrefC.GetRandomTempFile(Path.GetExtension(fileFullPath));
				File.WriteAllBytes(filePathToOpen,byteArray);
			}
			else {
				filePathToOpen=fileFullPath;
			}
			if(ODBuild.IsThinfinity()) {
				ThinfinityUtils.HandleFile(filePathToOpen);
			}
			else {
				Process.Start(filePathToOpen);
			}
		}

		///<summary>Runs the file.  If necessary, downloads the file from the cloud to the temp directory.  Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might open a file within the folder '\\server\OpenDentImages\wiki\[fileName]'.
		///folder can be set to a relative path like 'wiki\lists\images' which opens '\\server\OpenDentImages\wiki\lists\images\[fileName]'.</summary>
		public static void StartProcessRelative(string folder,string fileName) {
			StartProcess(CombinePaths(OpenDentBusiness.FileIO.FileAtoZ.GetPreferredAtoZpath(),folder,fileName));
		}

		///<summary>The first parameter, 'sourceFileName', must be a file that exists.</summary>
		public static void Copy(string sourceFileName,string destinationFileName,FileAtoZSourceDestination sourceDestination,
			string uploadMessage="Copying file...",bool isFolder=false,bool doOverwrite=false) 
		{
			if(CloudStorage.IsCloudStorage) {
				sourceFileName=CloudStorage.PathTidy(sourceFileName);
				destinationFileName=CloudStorage.PathTidy(destinationFileName);
				UI.ProgressWin progressWin = new UI.ProgressWin();
				progressWin.StartingMessage=uploadMessage;
				OpenDentalCloud.TaskState state;
				if(sourceDestination==FileAtoZSourceDestination.AtoZToAtoZ) {
					progressWin.ActionMain=() => CloudStorage.Copy(sourceFileName,destinationFileName);
					progressWin.ShowDialog();
					return;
				}
				else if(sourceDestination==FileAtoZSourceDestination.LocalToAtoZ) {
					progressWin.ActionMain=() => CloudStorage.Upload(Path.GetDirectoryName(destinationFileName),Path.GetFileName(destinationFileName),File.ReadAllBytes(sourceFileName));
					progressWin.ShowDialog();
					return;
				}
				else if(sourceDestination==FileAtoZSourceDestination.AtoZToLocal) {
					byte[] byteArray = null;
					progressWin.ActionMain=() => {
						byteArray=CloudStorage.Download(Path.GetDirectoryName(sourceFileName),Path.GetFileName(sourceFileName));
					};
					progressWin.ShowDialog();
					if(progressWin.IsCancelled){//user clicked cancel
						return;
					}
					File.WriteAllBytes(destinationFileName,byteArray);
					return;
				}
				else {
					throw new Exception("Unsupported "+nameof(FileAtoZSourceDestination)+": "+sourceDestination);
				}
			}
			else {//Not cloud
				File.Copy(sourceFileName,destinationFileName,doOverwrite);
			}
		}

		///<summary>Deletes the file.</summary>
		public static void Delete(string fileName) {
			OpenDentBusiness.FileIO.FileAtoZ.Delete(fileName);
		}

		///<summary>Returns true if the directory exists. If cloud, checks if that directory exists in the cloud.</summary>
		public static bool DirectoryExists(string folderName) {
			return OpenDentBusiness.FileIO.FileAtoZ.DirectoryExists(folderName);
		}

		///<summary>Opens the directory. If cloud, opens the directory in FormFilePicker.</summary>
		public static void OpenDirectory(string folderName,bool doHideLocalButton=true) {
			if(CloudStorage.IsCloudStorage) {
				using FormFilePicker FormFP=new FormFilePicker(folderName);
				FormFP.DoHideLocalButton=doHideLocalButton;
				if(FormFP.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			else {
				Process.Start(folderName);
			}
		}

		///<summary>Downloads an A to Z file to the local machine.</summary>
		public static void Download(string AtoZFilePath,string localFilePath,string downloadMessage="Downloading file...") {
			Copy(AtoZFilePath,localFilePath,FileAtoZSourceDestination.AtoZToLocal,downloadMessage);
		}

		///<summary>Uploads a local file to the A to Z folder.</summary>
		public static void Upload(string sourceFileName,string destinationFileName) {
			Copy(sourceFileName,destinationFileName,FileAtoZSourceDestination.LocalToAtoZ,"Uploading file...");
		}

	}
}
