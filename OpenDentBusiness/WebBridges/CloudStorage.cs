using CodeBase;
using OpenDentalCloud;
using OpenDentalCloud.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OpenDentBusiness {
	public class CloudStorage {

		public static char DirectorySeparatorChar {
			get {
				switch(PrefC.AtoZfolderUsed) {
					case DataStorageType.DropboxAtoZ:
					case DataStorageType.SftpAtoZ:
						return '/';
					case DataStorageType.InDatabase:
					case DataStorageType.LocalAtoZ:
					default:
						return Path.DirectorySeparatorChar;
				}
			}
		}

		public const string LanThis="CloudStorage";

		public static bool IsCloudStorage {
			get {
				switch(PrefC.AtoZfolderUsed) {
					case DataStorageType.SftpAtoZ:
					case DataStorageType.DropboxAtoZ:
						return true;
					case DataStorageType.InDatabase:
					case DataStorageType.LocalAtoZ:
					default:
						return false;
				}
			}
		}

		public static string AtoZPath {
			get {
				string retVal="";
				switch(PrefC.AtoZfolderUsed) {
					case DataStorageType.DropboxAtoZ:
						retVal=Dropbox.AtoZPath;
						break;
					case DataStorageType.SftpAtoZ:
						retVal=ODSftp.AtoZPath;
						break;
					case DataStorageType.InDatabase:
					case DataStorageType.LocalAtoZ:
						//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
						//rather than returning null and having the method throw a null exception later.
						throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
					default:
						throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
				}
				return retVal;
			}
		}

		#region Upload
		///<summary>Asynchronous.  Uploads the passed in file to Dropbox.  Will overwrite the passed in file if it already exists.
		///Pass in onProgress to hook up to a progress bar.  If onProgress is null, this method will break.</summary>
		public static TaskStateUpload UploadAsync(string folder,string fileName,byte[] fileContent,ProgressHandler progressHandler,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return UploadExecute(folder,fileName,fileContent,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.</summary>
		public static TaskStateUpload Upload(string folder,string fileName,byte[] fileContent,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return UploadExecute(folder,fileName,fileContent,null,hasExceptions);
		}

		///<summary>Runs the upload logic for the cloud storage type used.  
		///Will be Asynchronous or Synchronous depending on if progressHandler is null</summary>
		private static TaskStateUpload UploadExecute(string folder,string fileName,byte[] fileContent,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateUpload state=null;
			switch(PrefC.AtoZfolderUsed) {					
				case DataStorageType.DropboxAtoZ:
					//Dropbox will always overwrite conflicting files.  This can be changed in the future by implementing an optional "Dropbox.WriteMode" param.
					state=new OpenDentalCloud.Dropbox.Upload(Dropbox.AccessToken) {
						Folder=PathTidy(folder),
						FileName=fileName,
						FileContent=fileContent,
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					state=new OpenDentalCloud.Sftp.Upload(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Folder=PathTidy(folder),
						FileName=fileName,
						FileContent=fileContent,
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		#region Download
		///<summary>Asynchronous.  Downloads the file from the cloud with the passed in folder path and file name.
		///Pass in updateProgress to hook up to a progress bar.  If onProgress is null, this method will break.</summary>
		public static TaskStateDownload DownloadAsync(string folder,string fileName,ProgressHandler progressHandler,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return DownloadExecute(folder,fileName,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.  Downloads the file from the cloud with the passed in folder path and file name.</summary>
		public static TaskStateDownload Download(string folder,string fileName,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return DownloadExecute(folder,fileName,null,hasExceptions);
		}
		
		///<summary>Runs the download logic for the cloud storage type used.
		///Will be Asynchronous or Synchronous depending on if progressHandler is null</summary>
		private static TaskStateDownload DownloadExecute(string folder,string fileName,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateDownload state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.Download(Dropbox.AccessToken) {
						Folder=PathTidy(folder),
						FileName=fileName,
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					state=new OpenDentalCloud.Sftp.Download(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Folder=PathTidy(folder),
						FileName=fileName,
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		#region Delete
		///<summary>Asynchronous.  TaskStateDelete holds the Ids for the deleted file(s).</summary>
		public static TaskStateDelete DeleteAsync(string path,ProgressHandler progressHandler,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return DeleteExecute(path,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.  TaskStateDelete holds the Ids for the deleted file(s).</summary>
		public static TaskStateDelete Delete(string path,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return DeleteExecute(path,null,hasExceptions);
		}
		
		///<summary>Runs the delete logic for the cloud storage type used.
		///Will be Asynchronous or Synchronous depending on if progressHandler is null</summary>
		private static TaskStateDelete DeleteExecute(string path,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateDelete state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.Delete(Dropbox.AccessToken) {
						Path=PathTidy(path),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					if(!FileExists(path)) {
						return null;
					}
					state=new OpenDentalCloud.Sftp.Delete(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Path=PathTidy(path),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		#region Get Thumbnail
		///<summary>Asynchronous.  TaskStateThumbnail holds the thumbnail in bytes after the task thread is finished.</summary>
		public static TaskStateThumbnail GetThumbnailAsync(string folder,string fileName,ProgressHandler progressHandler,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return GetThumbnailExecute(folder,fileName,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.  TaskStateThumbnail holds the thumbnail in bytes after the task thread is finished.</summary>
		public static TaskStateThumbnail GetThumbnail(string folder,string fileName,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return GetThumbnailExecute(folder,fileName,null,hasExceptions);
		}
		
		///<summary>Runs the thumbnail logic for the cloud storage type used.
		///Will be Asynchronous or Synchronous depending on if progressHandler is null</summary>
		private static TaskStateThumbnail GetThumbnailExecute(string folder,string fileName,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateThumbnail state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.Thumbnail(Dropbox.AccessToken) {
						Folder=PathTidy(folder),
						FileName=fileName,
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					state=new OpenDentalCloud.Sftp.Thumbnail(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Folder=PathTidy(folder),
						FileName=fileName,
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		#region Move
		///<summary>Asynchronous.  Use TaskStateMove to get the end result information from running Move.</summary>
		public static TaskStateMove MoveAsync(string fromPath,string toPath,ProgressHandler progressHandler,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return MoveExecute(fromPath,toPath,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.  Use TaskStateMove to get the end result information from running Move.</summary>
		public static TaskStateMove Move(string fromPath,string toPath,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return MoveExecute(fromPath,toPath,null,hasExceptions);
		}

		private static TaskStateMove MoveExecute(string fromPath,string toPath,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateMove state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.Move(Dropbox.AccessToken) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					state=new OpenDentalCloud.Sftp.Move(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		#region Copy
		///<summary>Asynchronous.  Use TaskStateCopy to get the end result information from running Copy.</summary>
		public static TaskStateMove CopyAsync(string fromPath,string toPath,ProgressHandler progressHandler,bool hasExceptions = false) {
			//No need to check RemotingRole; no call to db.
			return CopyExecute(fromPath,toPath,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.  Use TaskStateCopy to get the end result information from running Copy.</summary>
		public static TaskStateMove Copy(string fromPath,string toPath,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return CopyExecute(fromPath,toPath,null,hasExceptions);
		}

		private static TaskStateMove CopyExecute(string fromPath,string toPath,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateMove state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.Copy(Dropbox.AccessToken) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					state=new OpenDentalCloud.Sftp.Copy(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		#region ListFolderContents
		///<summary>Asynchronous.  TaskStateListFolders will hold the result from the passed in path.  Throws exceptions.</summary>
		public static TaskStateListFolders ListFolderContentsAsync(string folderPath,ProgressHandler progressHandler,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return ListFolderContentsExecute(folderPath,progressHandler,hasExceptions);
		}

		///<summary>Synchronous.  TaskStateListFolders will hold the result from the passed in path.  Throws exceptions.</summary>
		public static TaskStateListFolders ListFolderContents(string folderPath,bool hasExceptions=false) {
			//No need to check RemotingRole; no call to db.
			return ListFolderContentsExecute(folderPath,null,hasExceptions);
		}

		private static TaskStateListFolders ListFolderContentsExecute(string folderPath,ProgressHandler progressHandler,bool hasExceptions) {
			TaskStateListFolders state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.ListFolders(Dropbox.AccessToken) {
						FolderPath=PathTidy(folderPath),
						ProgressHandler=progressHandler,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					try {
						state=new OpenDentalCloud.Sftp.ListFolders(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
							FolderPath=PathTidy(folderPath),
							ProgressHandler=progressHandler,
							HasExceptions=hasExceptions
						};
					}
					catch(ArgumentException ae) {
						throw new Exception("Invalid SFTP hostname, username, or password.  Please verify SFTP settings and try again.",ae);
					}
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			state.Execute(progressHandler!=null);
			return state;
		}
		#endregion

		///<summary>Synchronous.  Returns true if the file for the given path exists.</summary>
		public static bool FileExists(string path) {
			bool retVal=false;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					retVal=OpenDentalCloud.Dropbox.FileExists(Dropbox.AccessToken,PathTidy(path));
					break;
				case DataStorageType.SftpAtoZ:
					retVal=OpenDentalCloud.Sftp.FileExists(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password,path);
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			return retVal;
		}

		///<summary>Synchronous.  Returns true if the file for the given folder exists.</summary>
		public static bool FileExists(string folder,string fileName) {
			return FileExists(PathTidy(ODFileUtils.CombinePaths(folder,fileName)));
		}
		
		public static string PathTidy(string path) {
			string retVal=path;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
				case DataStorageType.SftpAtoZ:
					retVal=retVal.Replace(@"\","/");
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
				default:
					break;
			}
			return retVal;
		}
	}

	public interface IProgressHandler {
		void UpdateBytesRead(long numBytes);
		void DisplayError(string error);
		void CloseProgress();
	}

}
