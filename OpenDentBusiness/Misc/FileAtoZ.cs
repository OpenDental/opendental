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

namespace OpenDentBusiness.FileIO {
	///<summary>This class is used to access files in the A to Z folder. Depending on the storage type in use, it will read/write to a local 
	///location or it will download/upload from the cloud. All methods are synchronous.</summary>
	public class FileAtoZ {
		///<summary>Remembers the computerpref.AtoZpath.  Set to empty string on startup.  If set to something else, this path will override all other paths.</summary>
		public static string LocalAtoZpath=null;

		///<summary>Only makes a call to the database on startup.  After that, just uses cached data.
		///Does not validate that the path exists except if the main one is used.  ONLY used from Client layer or S class methods that have
		///"No need to check RemotingRole; no call to db" and which also make sure PrefC.AtoZfolderUsed.
		///Returns Cloud AtoZ path if CloudStorage.IsCloudStorage</summary>
		public static string GetPreferredAtoZpath() {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				return null;
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				if(LocalAtoZpath==null) {//on startup
					try {
						LocalAtoZpath=ComputerPrefs.LocalComputer.AtoZpath;
					}
					catch {//fails when loading plugins after switching to version 15.1 because of schema change.
						LocalAtoZpath="";
					}
				}
				//Override path.  Because it overrides all other paths, we evaluate it first.
				if(!string.IsNullOrEmpty(LocalAtoZpath)) {
					return LocalAtoZpath.Trim();
				}
				string replicationAtoZ=ReplicationServers.GetAtoZpath();
				if(!string.IsNullOrEmpty(replicationAtoZ)) {
					return GetValidPathFromString(replicationAtoZ)?.Trim();
				}
				//use this to handle possible multiple paths separated by semicolons.
				return GetValidPathFromString(PrefC.GetString(PrefName.DocPath))?.Trim();
			}
			//If you got here you are using a cloud storage method.
			return CloudStorage.AtoZPath?.Trim();
		}

		///<summary>Returns the first valid AtoZ path from the paths passed in.
		///Set documentPaths to a single path or a semicolon delimited string representing multiple paths.
		///A valid path is considered as the first path that contains a folder named 'A'.</summary>
		public static string GetValidPathFromString(string documentPaths) {
			foreach(string path in documentPaths.Split(new char[] { ';' })) {
				string tryPath=ODFileUtils.CombinePaths(path,"A");
				if(Directory.Exists(tryPath)) {
					return path;
				}
			}
			return null;
		}

		///<summary>Creates the specified directory.  Throws exceptions.</summary>
		public static void CreateDirectory(string folder) {
			if(CloudStorage.IsCloudStorage) {
				return;//There is no need to create directories in cloud mode.  Folders automatically get created when creating files.
			}
			Directory.CreateDirectory(folder);
		}

		///<summary>Creates the specified directory.  Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might create the folder '\\server\OpenDentImages\wiki\'.
		///folder can also be set to a relative path like 'wiki\lists\images' which creates '\\server\OpenDentImages\wiki\lists\images'.</summary>
		public static void CreateDirectoryRelative(string folder) {
			if(CloudStorage.IsCloudStorage) {
				return;//There is no need to create directories in cloud mode.  Folders automatically get created when creating files.
			}
			Directory.CreateDirectory(CombinePaths(GetPreferredAtoZpath(),folder));
		}

		///<summary>Returns the string contents of the file. Sychronous for cloud storage.</summary>
		public static string ReadAllText(string fileFullPath) {
			if(CloudStorage.IsCloudStorage) {
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(Path.GetDirectoryName(fileFullPath),Path.GetFileName(fileFullPath));
				return Encoding.UTF8.GetString(state.FileContent);
			}
			else {//Not cloud
				return File.ReadAllText(fileFullPath);
			}
		}

		///<summary>Returns the byte contents of the file. Sychronous for cloud storage.</summary>
		public static byte[] ReadAllBytes(string fileFullPath) {
			if(CloudStorage.IsCloudStorage) {
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(Path.GetDirectoryName(fileFullPath),Path.GetFileName(fileFullPath));
				return state.FileContent;
			}
			else {//Not cloud
				return File.ReadAllBytes(fileFullPath);
			}
		}

		///<summary>Writes or uploads the text to the specified file name.  Sychronous for cloud storage.  Throws exceptions.</summary>
		public static void WriteAllText(string fileFullPath,string contents) {
			if(CloudStorage.IsCloudStorage) {
				CloudStorage.Upload(Path.GetDirectoryName(fileFullPath),Path.GetFileName(fileFullPath),Encoding.UTF8.GetBytes(contents));
			}
			else {//Not cloud
				File.WriteAllText(fileFullPath,contents);
			}
		}

		///<summary>Writes or uploads the text to the specified file name.  Sychronous for cloud storage.  Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might create a file within the folder '\\server\OpenDentImages\wiki\[fileName]'.
		///folder can be set to a relative path like 'wiki\lists\images' which creates '\\server\OpenDentImages\wiki\lists\images\[fileName]'.</summary>
		public static void WriteAllTextRelative(string folder,string fileName,string contents) {
			WriteAllText(CombinePaths(GetPreferredAtoZpath(),folder,fileName),contents);
		}

		///<summary>Writes or uploads the bytes to the specified file name. Sychronous for cloud storage.</summary>
		public static void WriteAllBytes(string fileFullPath,byte[] byteArray) {
			if(CloudStorage.IsCloudStorage) {
				CloudStorage.Upload(Path.GetDirectoryName(fileFullPath),Path.GetFileName(fileFullPath),byteArray);
			}
			else {//Not cloud
				File.WriteAllBytes(fileFullPath,byteArray);
			}
		}

		///<summary>Gets a list of the files in the specified directory.  Throws exceptions.</summary>
		public static List<string> GetFilesInDirectory(string folderFullPath) {
			if(CloudStorage.IsCloudStorage) {
				return CloudStorage.ListFolderContents(folderFullPath).ListFolderPathsDisplay;
			}
			return Directory.GetFiles(folderFullPath).ToList();
		}

		///<summary>Gets a list of the files in the specified directory.  Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might return files within the folder '\\server\OpenDentImages\wiki\'.
		///folder can also be set to a relative path like 'wiki\lists\images' which searches '\\server\OpenDentImages\wiki\lists\images'.</summary>
		public static List<string> GetFilesInDirectoryRelative(string folder) {
			return GetFilesInDirectory(CombinePaths(GetPreferredAtoZpath(),folder));
		}

		///<summary>Use this instead of ODFileUtils.CombinePaths when the path is in the A to Z folder.</summary>
		public static string CombinePaths(params string[] paths) {
			return CloudStorage.PathTidy(ODFileUtils.CombinePaths(paths));
		}

		///<summary>Use this instead of ODFileUtils.AppendSuffix when the path is in the A to Z folder.</summary>
		public static string AppendSuffix(string fileFullPath,string suffix) {
			return CloudStorage.PathTidy(ODFileUtils.AppendSuffix(fileFullPath,suffix));
		}

		///<summary>Returns true if the file exists.  Sychronous for cloud storage. Throws exceptions.</summary>
		public static bool Exists(string fileFullPath) {
			if(CloudStorage.IsCloudStorage) {
				return CloudStorage.FileExists(fileFullPath);
			}
			return File.Exists(fileFullPath);
		}

		///<summary>Returns true if the file exists.  Sychronous for cloud storage. Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might create the folder '\\server\OpenDentImages\wiki\'.
		///folder can also be set to a relative path like 'wiki\lists\images' which creates '\\server\OpenDentImages\wiki\lists\images'.</summary>
		public static bool ExistsRelative(string folder,string fileName) {
			return Exists(CombinePaths(GetPreferredAtoZpath(),folder,fileName));
		}

		///<summary>The first parameter, 'sourceFileName', must be a file that exists. Sychronous for cloud storage.</summary>
		public static void Copy(string sourceFileName,string destinationFileName,FileAtoZSourceDestination sourceDestination,bool isFolder=false,
			bool doOverwrite=false) {
			if(CloudStorage.IsCloudStorage) {
				sourceFileName=CloudStorage.PathTidy(sourceFileName);
				destinationFileName=CloudStorage.PathTidy(destinationFileName);
				OpenDentalCloud.TaskState state;
				if(sourceDestination==FileAtoZSourceDestination.AtoZToAtoZ) {
					state=CloudStorage.Copy(sourceFileName,destinationFileName);
				}
				else if(sourceDestination==FileAtoZSourceDestination.LocalToAtoZ) {
					state=CloudStorage.Upload(Path.GetDirectoryName(destinationFileName),Path.GetFileName(destinationFileName),
						File.ReadAllBytes(sourceFileName));
				}
				else if(sourceDestination==FileAtoZSourceDestination.AtoZToLocal) {
					state=CloudStorage.Download(Path.GetDirectoryName(sourceFileName),Path.GetFileName(sourceFileName));
				}
				else {
					throw new Exception("Unsupported "+nameof(FileAtoZSourceDestination)+": "+sourceDestination);
				}
				if(sourceDestination==FileAtoZSourceDestination.AtoZToLocal) {
					File.WriteAllBytes(destinationFileName,((OpenDentalCloud.Core.TaskStateDownload)state).FileContent);
				}
			}
			else {//Not cloud
				File.Copy(sourceFileName,destinationFileName,doOverwrite);
			}
		}

		public static void Move(string sourceFileFullPath,string destinationFileFullPath) {
			if(CloudStorage.IsCloudStorage) {
				CloudStorage.Move(sourceFileFullPath,destinationFileFullPath,hasExceptions:true);
			}
			else {
				File.Move(sourceFileFullPath,destinationFileFullPath);
			}
		}

		///<summary>Deletes the file. Sychronous for cloud storage.</summary>
		public static void Delete(string fileFullPath) {
			if(CloudStorage.IsCloudStorage) {
				CloudStorage.Delete(fileFullPath);
			}
			else {
				File.Delete(fileFullPath);
			}
		}

		///<summary>Returns true if the directory exists.  Sychronous for cloud storage.  Throws exceptions.</summary>
		public static bool DirectoryExists(string folderFullPath) {
			if(CloudStorage.IsCloudStorage) {
				return CloudStorage.FileExists(folderFullPath);
			}
			return Directory.Exists(folderFullPath);
		}

		///<summary>Returns true if the directory exists.  Sychronous for cloud storage.  Throws exceptions.
		///This method will dynamically prepend the correct AtoZ path to the folder path provided.
		///E.g. passing in 'wiki' as the value for folder might search for the folder '\\server\OpenDentImages\wiki\'.
		///folder can also be set to a relative path like 'wiki\lists\images' which searches for '\\server\OpenDentImages\wiki\lists\images'.</summary>
		public static bool DirectoryExistsRelative(string folder) {
			return DirectoryExists(CombinePaths(GetPreferredAtoZpath(),folder));
		}

		///<summary>Returns null if the the image could not be downloaded. Sychronous.</summary>
		public static Bitmap GetImage(string imagePath) {
			if(CloudStorage.IsCloudStorage) {
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.Download(Path.GetDirectoryName(imagePath),Path.GetFileName(imagePath));
				if(state==null || state.FileContent==null || state.FileContent.Length < 2) {
					return null;
				}
				using(MemoryStream stream=new MemoryStream(state.FileContent)) {
					return new Bitmap(Image.FromStream(stream));
				}
			}
			//Not cloud
			return new Bitmap(Image.FromFile(imagePath));
		}

		///<summary>Returns null if the the image could not be downloaded. Sychronous.</summary>
		public static Bitmap GetBitmap128(string imagePath) {
			if(CloudStorage.IsCloudStorage) {
				OpenDentalCloud.Core.TaskStateThumbnail state=CloudStorage.GetThumbnail(Path.GetDirectoryName(imagePath),Path.GetFileName(imagePath));
				if(state==null || state.FileContent==null || state.FileContent.Length < 2) {
					return null;
				}
				using(MemoryStream stream=new MemoryStream(state.FileContent)) {
					return ImageHelper.GetBitmapResized((Bitmap)Image.FromStream(stream),128);
				}
			}
			//Not cloud
			return ImageHelper.GetBitmapResized((Bitmap)Image.FromFile(imagePath),128);
		}

	}
}

namespace OpenDentBusiness {
	///<summary>Used to specify where the files are coming from and going when copying.</summary>
	public enum FileAtoZSourceDestination {
		///<summary>Copying a local file to AtoZ folder. Equivalent to 'upload.'</summary>
		LocalToAtoZ,
		///<summary>Copying an AtoZ file to a local file. Equivalent to 'download'.</summary>
		AtoZToLocal,
		///<summary>Copying an AtoZ file to another AtoZ file. Equivalent to 'download' then 'upload'.</summary>
		AtoZToAtoZ
	}

}
