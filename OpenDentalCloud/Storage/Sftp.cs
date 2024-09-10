using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using OpenDentalCloud.Core;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace OpenDentalCloud {

	public static class Sftp {

		private static SftpClient Init(string host,string user,string pass,int port=22) {
			return new SftpClient(new ConnectionInfo(host,port,user,new AuthenticationMethod[] { new PasswordAuthenticationMethod(user,pass) }));
		}

		public class Upload : TaskStateUpload {
			private SftpClient _client;

			public Upload(string host,string user,string pass,int port=22) {
				_client=Init(host,user,pass,port);
			}

			protected override async Task PerformIO() {
				bool hadToConnect=_client.ConnectIfNeeded();
				_client.CreateDirectoriesIfNeeded(Folder);
				string fullFilePath=ODFileUtils.CombinePaths(Folder,FileName,'/');
				using(MemoryStream uploadStream=new MemoryStream(ByteArray)) {
					SftpUploadAsyncResult res=(SftpUploadAsyncResult)_client.BeginUploadFile(uploadStream,fullFilePath);
					while(!res.AsyncWaitHandle.WaitOne(100)) {
						if(DoCancel) {
							res.IsUploadCanceled=true;
						}
					}
					_client.EndUploadFile(res);
					if(res.IsUploadCanceled) {
						TaskStateDelete state=new Delete() {
							_client=this._client,
							Path=fullFilePath
						};
						state.Execute();
					}
				}
				_client.DisconnectIfNeeded(hadToConnect);
				await Task.Run(() => { });//Gets rid of a compiler warning and does nothing.
			}
		}

		public class Download : TaskStateDownload {
			private static string _stashRootFolderPath=ODFileUtils.CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"OpenDental","Stash");
			private static string _stashInstanceFolderPath=ODFileUtils.CombinePaths(_stashRootFolderPath,Process.GetCurrentProcess().Id.ToString());
			internal SftpClient _client;
			public static bool DoUseStash=true;

			public Download(string host,string user,string pass,int port=22) {
				_client=Init(host,user,pass,port);
			}

			internal Download() {

			}

			protected override async Task PerformIO() {
				bool hadToConnect=_client.ConnectIfNeeded();
				string fullFilePath=ODFileUtils.CombinePaths(Folder,FileName,'/');
				SftpFileAttributes attribute=_client.GetAttributes(fullFilePath);
				string stashFilePath=null;
				if(DoUseStash) {
					try {
						stashFilePath=ODFileUtils.CombinePaths(_stashInstanceFolderPath,fullFilePath);
						string stashFileParentDir=Directory.GetParent(stashFilePath).FullName;
						if(!Directory.Exists(stashFileParentDir)) {
							Directory.CreateDirectory(stashFileParentDir);
						}
						if(File.Exists(stashFilePath)) {
							FileInfo stashFileInfo=new FileInfo(stashFilePath);
							while((DateTime.Now-stashFileInfo.CreationTime).TotalSeconds < 15) {//while recently created (currently being written to)
								if(stashFileInfo.LastWriteTime==attribute.LastWriteTime) {
									break;
								}
								Thread.Sleep(10);
							}
							if(stashFileInfo.LastWriteTime==attribute.LastWriteTime) {
								ByteArray=ProtectedData.Unprotect(File.ReadAllBytes(stashFilePath),null,DataProtectionScope.CurrentUser);
								return;
							}
						}
						else {
							//Create immediately as an empty file so that any read attempts will know that the file is in the process of being downloaded.
							FileStream fileStream=File.Create(stashFilePath);
							fileStream.Dispose();
						}
					}
					catch(Exception ex) {
						ex.DoNothing();//If anything goes wrong, use SFTP normally without relying on the stash.
						stashFilePath="";
					}
				}
				using(MemoryStream stream=new MemoryStream()) {
					SftpDownloadAsyncResult res=(SftpDownloadAsyncResult)_client.BeginDownloadFile(fullFilePath,stream);
					while(!res.AsyncWaitHandle.WaitOne(100)) {
						if(DoCancel) {
							res.IsDownloadCanceled=true;
							_client.DisconnectIfNeeded(hadToConnect);
							return;
						}
					}
					_client.EndDownloadFile(res);
					ByteArray=stream.ToArray();
					if(DoUseStash && !stashFilePath.IsNullOrEmpty()) {
						File.WriteAllBytes(stashFilePath,ProtectedData.Protect(ByteArray,null,DataProtectionScope.CurrentUser));
						File.SetLastWriteTime(stashFilePath,attribute.LastWriteTime);//Must be last operation
					}
				}
				_client.DisconnectIfNeeded(hadToConnect);
				await Task.Run(() => { });//Gets rid of a compiler warning and does nothing.
			}

			public static void CleanupStashes(bool canCleanCurrentInstance) {
				try {
					if(!Directory.Exists(_stashRootFolderPath)) {
						return;
					}
					string[] arrayStashInstanceFolderPaths=Directory.GetDirectories(_stashRootFolderPath,"*",SearchOption.TopDirectoryOnly);
					List<string> listCleanupStashInstanceFolderPaths=new List<string>();
					if(canCleanCurrentInstance) {
						listCleanupStashInstanceFolderPaths.Add(_stashInstanceFolderPath);//Always cleanup current instance stash first.
					}
					foreach(string stashInstanceFolderPath in arrayStashInstanceFolderPaths) {
						DirectoryInfo stashInstanceFolderPathInfo=new DirectoryInfo(stashInstanceFolderPath);
						if(!int.TryParse(stashInstanceFolderPathInfo.Name,out int processId)) {
							continue;
						}
						bool isRunning=false;
						try {
							Process process=Process.GetProcessById(processId);//This line throws an exception if there is no such process running.
							if(process!=null && process.ProcessName.StartsWith("OpenDental")) {//The process for this stash instance folder is still running.
								isRunning=true;
							}
						}
						catch(Exception ex) {
							ex.DoNothing();
						}
						if(!isRunning) {
							listCleanupStashInstanceFolderPaths.Add(stashInstanceFolderPath);
						}
					}
					foreach(string stashInstanceFolderPath in listCleanupStashInstanceFolderPaths) {
						try {
							//Could fail for two main reasons: 1) File(s) still in use. 2) Stash instance folder was created by another user.
							Directory.Delete(stashInstanceFolderPath,true);//Recursive. Deletes subfolders.
						}
						catch(Exception ex) {
							ex.DoNothing();
						}
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Not an issue because we will try again when closing or opening the application next.
				}
			}

		}

		public class Delete : TaskStateDelete {
			internal SftpClient _client;

			public Delete(string host,string user,string pass) {
				_client=Init(host,user,pass);
			}

			internal Delete() {

			}

			protected override async Task PerformIO() {
				bool hadToConnect=_client.ConnectIfNeeded();
				await Task.Run(() => { _client.Delete(Path); });
				_client.DisconnectIfNeeded(hadToConnect);
			}
		}

		public class Thumbnail : TaskStateThumbnail {
			private SftpClient _client;

			public Thumbnail(string host,string user,string pass) {
				_client=Init(host,user,pass);
			}

			protected override async Task PerformIO() {
				TaskStateDownload stateDownload=new Download() {
					_client=this._client,
					Folder=this.Folder,
					FileName=this.FileName
				};
				stateDownload.Execute();//We could get cute later and make this match isAsync to update the main thread if they are downloading a large file.
				ByteArray=stateDownload.ByteArray;
				await Task.Run(() => { });//This gets rid of a compiler warning.
			}
		}

		public class ListFolders : TaskStateListFolders {
			internal SftpClient _client;

			public ListFolders(string host,string user,string pass) {
				_client=Init(host,user,pass);
			}

			internal ListFolders() {
			}

			protected override async Task PerformIO() {
				bool hadToConnect=_client.ConnectIfNeeded();
				_client.CreateDirectoriesIfNeeded(FolderPath);
				List<SftpFile> listFiles=(await Task.Factory.FromAsync(_client.BeginListDirectory(FolderPath,null,null),_client.EndListDirectory)).ToList();
				listFiles=listFiles.FindAll(x => !x.FullName.EndsWith("."));//Sftp has 2 "extra" files that are "." and "..".  I think it's for explorer navigation.
				ListFolderPathLower=listFiles.Select(x => x.FullName.ToLower()).ToList();
				ListFolderPathsDisplay=listFiles.Select(x => x.FullName).ToList();
				_client.DisconnectIfNeeded(hadToConnect);
			}
		}

		public class Move : TaskStateMove {
			private SftpClient _client;
			internal bool IsCopy=false;
			private string _host;
			private string _user;
			private string _pass;

			public Move(string host,string user,string pass) {
				_client=Init(host,user,pass);
				_host=host;
				_user=user;
				_pass=pass;
			}			

			protected override async Task PerformIO() {
				bool hadToConnect=_client.ConnectIfNeeded();
				List<string> listFilePaths;
				bool isDirectory=_client.GetAttributes(FromPath).IsDirectory;
				if(isDirectory) {
					if(DoCancel) {
						return;
					}
					//Only include files (not sub-directories) in the list
					listFilePaths=(await Task.Factory.FromAsync(_client.BeginListDirectory(FromPath,null,null),_client.EndListDirectory))
						.Where(x => !x.IsDirectory)
						.Select(x => x.FullName).ToList();
				}
				else {//Copying a file
					listFilePaths=new List<string> { FromPath };
				}
				CountTotal=listFilePaths.Count;
				for(int i=0;i<CountTotal;i++) {
					if(DoCancel) {
						_client.DisconnectIfNeeded(hadToConnect);
						return;
					}
					string path=listFilePaths[i];
					try {
						string fileName=Path.GetFileName(path);
						string toPathFull=ToPath;
						if(isDirectory) {
							toPathFull=ODFileUtils.CombinePaths(ToPath,fileName,'/');
						}
						if(FileExists(_client,toPathFull)) {
							throw new Exception();//Throw so that we can iterate CountFailed
						}
						string fromPathFull=FromPath;
						if(isDirectory) {
							fromPathFull=ODFileUtils.CombinePaths(FromPath,fileName,'/');
						}
						if(IsCopy) {
							//Throws if fails.
							await Task.Run(() => {
								TaskStateDownload stateDown=new Download(_host,_user,_pass) {
									Folder=Path.GetDirectoryName(fromPathFull).Replace('\\','/'),
									FileName=Path.GetFileName(fromPathFull),
									HasExceptions=HasExceptions
								};
								stateDown.Execute();
								while(!stateDown.IsDone) {
									stateDown.DoCancel=DoCancel;
								}
								if(DoCancel) {
									_client.DisconnectIfNeeded(hadToConnect);
									return;
								}
								TaskStateUpload stateUp=new Upload(_host,_user,_pass) {
									Folder=Path.GetDirectoryName(toPathFull).Replace('\\','/'),
									FileName=Path.GetFileName(toPathFull),
									ByteArray=stateDown.ByteArray,
									HasExceptions=HasExceptions
								};
								stateUp.Execute();
								while(!stateUp.IsDone) {
									stateUp.DoCancel=DoCancel;
								}
								if(DoCancel) {
									_client.DisconnectIfNeeded(hadToConnect);
									return;
								}
							});
						}
						else {//Moving
							await Task.Run(() => {
								SftpFile file=_client.Get(path);
								_client.CreateDirectoriesIfNeeded(ToPath);
								file.MoveTo(toPathFull);
							});
						}
						CountSuccess++;
					}
					catch(Exception) {
						CountFailed++;
					}
				}
				_client.DisconnectIfNeeded(hadToConnect);
			}
		}

		public class Copy:TaskStateCopy {
			TaskStateMove _stateMove;

			public Copy(string host,string user,string pass) {
				_stateMove=new Move(host,user,pass) {
					IsCopy=true
				};
			}

			protected override async Task PerformIO() {
				_stateMove.FromPath=FromPath;
				_stateMove.ToPath=ToPath;
				_stateMove.Execute(isAsync:true);
				while(!_stateMove.IsDone) {
					Thread.Sleep(10);
					if(DoCancel) {
						_stateMove.DoCancel=true;
					}
				}
				await Task.Run(() => { });
			}
		}

		///<summary>Synchronous.  Returns true if a file exists in the passed in filePath</summary>
		public static bool FileExists(string host,string user,string pass,string filePath) {
			SftpClient client=Init(host,user,pass);
			return FileExists(client,filePath);
		}

		///<summary>Synchronous.  Returns true if a file exists in the passed in filePath</summary>
		private static bool FileExists(SftpClient client,string filePath) {
			bool retVal=false;
			try {
				bool hadToConnect=client.ConnectIfNeeded();
				client.Get(filePath);
				client.DisconnectIfNeeded(hadToConnect);
				retVal=true;
			}
			catch(Exception) {
			}
			return retVal;
		}

		public static bool IsConnectionValid(string host,string user,string pass,int port=22) {
			bool retval=false;
			try {
				SftpClient client=Init(host,user,pass,port);
				client.Connect();
				if(client.IsConnected) {
					retval=true;
					client.Disconnect();
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return retval;
		}

	}

	public static class SftpExtension {

		///<summary>Loops through the entire path and sees if any directory along the way doesn't exist.  If any don't exist, they get created.</summary>
		public static void CreateDirectoriesIfNeeded(this SftpClient client,string path) {
			bool hadToConnect=client.ConnectIfNeeded();
			if(string.IsNullOrEmpty(path)) {
				return;
			}
			string currentDir="";
			string[] directories=path.Split("/",StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<directories.Length;i++) {
				if(i>0 || path[0]=='/') {
					currentDir+="/";
				}
				currentDir+=directories[i];
				try {
					//This will throw an exception of SftpPathNotFoundException if the directory does not exist
					SftpFileAttributes attributes=client.GetAttributes(currentDir);
					//Check to see if it's a directory.  This will not throw an exception of SftpPathNotFoundException, so we want to break out if it's a file path.
					//This would be a weird permission issue or implementation error, but it doesn't hurt anything.
					if(!attributes.IsDirectory) {
						break;
					}
				}
				catch(SftpPathNotFoundException) {
					client.CreateDirectory(currentDir);
				}
			}
			client.DisconnectIfNeeded(hadToConnect);
		}

		///<summary>Returns true if it was not connected and had to connect.</summary>
		public static bool ConnectIfNeeded(this SftpClient client) {
			if(!client.IsConnected) {
				try {
					client.Connect();
				}
				catch(Exception e) {
					throw new Exception("Connecting to "+client.ConnectionInfo.Host+" has failed with user: "+client.ConnectionInfo.Username+"\r\n"+e.Message,e);
				}
				return true;
			}
			return false;
		}

		///<summary>Will disconnect the client if the client was the one that determined it needed to be connected.</summary>
		public static void DisconnectIfNeeded(this SftpClient client,bool hadToConnect) {
			if(hadToConnect && client.IsConnected) {
				client.Disconnect();
			}
		}
	}
}
