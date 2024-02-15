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

		public static void Upload(string folder,string fileName,byte[] byteArray,bool hasExceptions=false){
			/*
			//How to use:
			//Exceptions are never thrown. They always get caught inside of Execute except in one spot where hasExceptions is set to true.
			//1. Example with no progress window. 
							CloudStorage.Upload(folder,fileName,byteArray);
			//2. Example with a progress window. 
							UI.ProgressWin progressWin=new UI.ProgressWin();
							progressWin.StartingMessage="Uploading...";
							progressWin.ActionMain=() => CloudStorage.Upload(folder,fileName,byteArray);
							progressWin.ShowDialog();
							if(progressWin.IsCancelled){
								return;
							}
			*/
			TaskStateUpload taskStateUpload=null;
			switch(PrefC.AtoZfolderUsed) {					
				case DataStorageType.DropboxAtoZ:
					//Dropbox will always overwrite conflicting files.  This can be changed in the future by implementing an optional "Dropbox.WriteMode" param.
					taskStateUpload=new OpenDentalCloud.Dropbox.Upload(Dropbox.AccessToken) {
						Folder=PathTidy(folder),
						FileName=fileName,
						ByteArray=byteArray,
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					taskStateUpload=new OpenDentalCloud.Sftp.Upload(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Folder=PathTidy(folder),
						FileName=fileName,
						ByteArray=byteArray,
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
			taskStateUpload.Execute();
		}

		///<summary></summary>
		public static byte[] Download(string folder,string fileName) {
			/*
			//How to use:
			//Exceptions are never thrown. They always get caught inside of Execute
			//1. Example with no progress window. 
							byte[] byteArray=CloudStorage.Download(folder,fileName);
			//2. Example with a progress window. 
							UI.ProgressWin progressWin=new UI.ProgressWin();
							progressWin.StartingMessage="Downloading...";
							byte[] byteArray=null;
							progressWin.ActionMain=() => byteArray=CloudStorage.Download(folder,fileName);
							progressWin.ShowDialog();
							if(progressWin.IsCancelled){//user clicked cancel. Not usually needed because you can just test the byte array below.
								return;
							}
							if(byteArray==null || byteArray.Length==0){
								//show error msg, etc.
							}
			//In both cases, once you have the bytes, the two general approaches are:
							File.WriteAllBytes(tempFile,byteArray);
			//or
							using MemoryStream memoryStream=new MemoryStream(byteArray);
			*/
			TaskStateDownload taskStateDownload=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					taskStateDownload=new OpenDentalCloud.Dropbox.Download(Dropbox.AccessToken);
					taskStateDownload.Folder=PathTidy(folder);
					taskStateDownload.FileName=fileName;
					//The purpose of this complex wrapper is to handle the fact that the Dropbox API is async.
					break;
				case DataStorageType.SftpAtoZ:
					taskStateDownload=new OpenDentalCloud.Sftp.Download(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password);
					taskStateDownload.Folder=PathTidy(folder);
					taskStateDownload.FileName=fileName;
					break;
				case DataStorageType.InDatabase:
				case DataStorageType.LocalAtoZ:
					//Local storage methods should never be calling this method, throw an exception so it's more obvious that this was the issue,
					//rather than returning null and having the method throw a null exception later.
					throw new Exception("Unknown cloud storage type: "+PrefC.AtoZfolderUsed.ToString());
				default:
					throw new Exception("DataStorageType: "+PrefC.AtoZfolderUsed.ToString()+" not implemented.");
			}
			taskStateDownload.Execute();
			return taskStateDownload.ByteArray;
		}

		public static void Delete(string path){
			TaskStateDelete taskStateDelete=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					taskStateDelete=new OpenDentalCloud.Dropbox.Delete(Dropbox.AccessToken) {
						Path=PathTidy(path)
					};
					break;
				case DataStorageType.SftpAtoZ:
					if(!FileExists(path)) {
						return;
					}
					taskStateDelete=new OpenDentalCloud.Sftp.Delete(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Path=PathTidy(path)
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
			taskStateDelete.Execute();
		}

		///<summary></summary>
		public static byte[] GetThumbnail(string folder,string fileName) {
			//No need to check MiddleTierRole; no call to db.
			TaskStateThumbnail taskStateThumbnail=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					taskStateThumbnail=new OpenDentalCloud.Dropbox.Thumbnail(Dropbox.AccessToken) {
						Folder=PathTidy(folder),
						FileName=fileName
					};
					break;
				case DataStorageType.SftpAtoZ:
					taskStateThumbnail=new OpenDentalCloud.Sftp.Thumbnail(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						Folder=PathTidy(folder),
						FileName=fileName
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
			taskStateThumbnail.Execute();
			return taskStateThumbnail.ByteArray;
		}

		///<summary>Returns true if successful</summary>
		public static bool Move(string fromPath,string toPath,bool hasExceptions=false) {
			//No need to check MiddleTierRole; no call to db.
			TaskStateMove taskStateMove=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					taskStateMove=new OpenDentalCloud.Dropbox.Move(Dropbox.AccessToken) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath),
						HasExceptions=hasExceptions
					};
					break;
				case DataStorageType.SftpAtoZ:
					taskStateMove=new OpenDentalCloud.Sftp.Move(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath),
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
			taskStateMove.Execute();
			if(taskStateMove.CountFailed==0){
				return true;
			}
			return false;
		}
		
		///<summary></summary>
		public static void Copy(string fromPath,string toPath) {
			//No need to check MiddleTierRole; no call to db.
			TaskStateMove state=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					state=new OpenDentalCloud.Dropbox.Copy(Dropbox.AccessToken) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath)
					};
					break;
				case DataStorageType.SftpAtoZ:
					state=new OpenDentalCloud.Sftp.Copy(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
						FromPath=PathTidy(fromPath),
						ToPath=PathTidy(toPath)
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
			state.Execute();
		}

		///<summary></summary>
		public static List<string> ListFolderContents(string folderPath) {
			//No need to check MiddleTierRole; no call to db.
			TaskStateListFolders taskStateListFolders=null;
			switch(PrefC.AtoZfolderUsed) {
				case DataStorageType.DropboxAtoZ:
					taskStateListFolders=new OpenDentalCloud.Dropbox.ListFolders(Dropbox.AccessToken) {
						FolderPath=PathTidy(folderPath)
					};
					break;
				case DataStorageType.SftpAtoZ:
					try {
						taskStateListFolders=new OpenDentalCloud.Sftp.ListFolders(ODSftp.Hostname,ODSftp.UserName,ODSftp.Password) {
							FolderPath=PathTidy(folderPath)
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
			taskStateListFolders.Execute();
			return taskStateListFolders.ListFolderPathsDisplay;
		}

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

	

}
