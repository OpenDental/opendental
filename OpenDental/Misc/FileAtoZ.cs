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
				FormProgress FormP=CreateFormProgress("Downloading...");
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(Path.GetDirectoryName(fileName),Path.GetFileName(fileName),
					new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
					return "";
				}
				return Encoding.UTF8.GetString(state.FileContent);
			}
			else {//Not cloud
				return File.ReadAllText(fileName);
			}
		}

		///<summary>Writes or uploads the text to the specified file name.</summary>
		public static void WriteAllText(string fileName,string textForFile,string uploadMessage="Uploading file") {
			if(CloudStorage.IsCloudStorage) {
				FormProgress FormP=CreateFormProgress(uploadMessage);
				OpenDentalCloud.Core.TaskStateUpload state=CloudStorage.UploadAsync(Path.GetDirectoryName(fileName),Path.GetFileName(fileName),
					Encoding.UTF8.GetBytes(textForFile),new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
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
					FormProgress FormP=CreateFormProgress("Downloading...");
					OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(Path.GetDirectoryName(actualFilePath),
						Path.GetFileName(actualFilePath),new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
					FormP.ShowDialog();
					if(FormP.DialogResult==DialogResult.Cancel) {
						state.DoCancel=true;
						return;
					}
					File.WriteAllBytes(tempFile,state.FileContent);
				}
				else { //Not Cloud
					//We have to create a copy of the file because the name is different.
					//There is also a high probability that the attachment no longer exists if
					//images are stored in the database, since the file will have originally been
					//placed in the temporary directory.			
					File.Copy(actualFilePath,tempFile,true);
				}
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					Process.Start(tempFile);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
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
				FormProgress FormP=CreateFormProgress("Downloading...");
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(Path.GetDirectoryName(imagePath),Path.GetFileName(imagePath),
					new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				if(FormP.ShowDialog()==DialogResult.Cancel) {
					state.DoCancel=true;
					return null;
				}
				if(state==null || state.FileContent==null || state.FileContent.Length < 2) {
						return null;
				}
				else {
					using(MemoryStream stream=new MemoryStream(state.FileContent)) {
						return new Bitmap(stream);
					}
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
				FormProgress FormP=CreateFormProgress("Downloading...");
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(Path.GetDirectoryName(fileFullPath),Path.GetFileName(fileFullPath),
					new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				filePathToOpen=PrefC.GetRandomTempFile(Path.GetExtension(fileFullPath));
				File.WriteAllBytes(filePathToOpen,state.FileContent);
			}
			else {
				filePathToOpen=fileFullPath;
			}
			if(ODBuild.IsWeb()) {
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
				FormProgress FormP=CreateFormProgress(uploadMessage,isFolder);
				OpenDentalCloud.TaskState state;
				if(sourceDestination==FileAtoZSourceDestination.AtoZToAtoZ) {
					state=CloudStorage.CopyAsync(sourceFileName,destinationFileName,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				}
				else if(sourceDestination==FileAtoZSourceDestination.LocalToAtoZ) {
					state=CloudStorage.UploadAsync(Path.GetDirectoryName(destinationFileName),Path.GetFileName(destinationFileName),
						File.ReadAllBytes(sourceFileName),new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				}
				else if(sourceDestination==FileAtoZSourceDestination.AtoZToLocal) {
					state=CloudStorage.DownloadAsync(Path.GetDirectoryName(sourceFileName),Path.GetFileName(sourceFileName),
						new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				}
				else {
					throw new Exception("Unsupported "+nameof(FileAtoZSourceDestination)+": "+sourceDestination);
				}
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				if(sourceDestination==FileAtoZSourceDestination.AtoZToLocal) {
					File.WriteAllBytes(destinationFileName,((OpenDentalCloud.Core.TaskStateDownload)state).FileContent);
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

		///<summary>Method to create a FormProgress. Set isSingleFile to false if copying or moving a folder.</summary>
		private static FormProgress CreateFormProgress(string displayMessage,bool isFolder=false) {
			FormProgress FormP=new FormProgress();
			FormP.DisplayText=Lan.g(CloudStorage.LanThis,displayMessage);
			if(isFolder) {
				FormP.NumberFormat="";//Display whole numbers
			}
			else {
				FormP.NumberFormat="F";//Display decimal places
			}
			FormP.NumberMultiplication=1;
			FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
			FormP.TickMS=1000;
			return FormP;
		}

		///<summary>Uploads an A to Z file to the local machine.</summary>
		public static void Download(string AtoZFilePath,string localFilePath,string downloadMessage="Downloading file...") {
			Copy(AtoZFilePath,localFilePath,FileAtoZSourceDestination.AtoZToLocal,downloadMessage);
		}

		///<summary>Uploads a local file to the A to Z folder.</summary>
		public static void Upload(string sourceFileName,string destinationFileName) {
			Copy(sourceFileName,destinationFileName,FileAtoZSourceDestination.LocalToAtoZ,"Uploading file...");
		}

	}
}
