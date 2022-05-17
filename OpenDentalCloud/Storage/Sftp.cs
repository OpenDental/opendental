using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				using(MemoryStream uploadStream=new MemoryStream(FileContent)) {
					SftpUploadAsyncResult res=(SftpUploadAsyncResult)_client.BeginUploadFile(uploadStream,fullFilePath);
					while(!res.AsyncWaitHandle.WaitOne(100)) {
						if(DoCancel) {
							res.IsUploadCanceled=true;
						}
						OnProgress((double)res.UploadedBytes/(double)1024/(double)1024,"?currentVal MB of ?maxVal MB uploaded",(double)FileContent.Length/(double)1024/(double)1024,"");
					}
					_client.EndUploadFile(res);
					if(res.IsUploadCanceled) {
						TaskStateDelete state=new Delete() {
							_client=this._client,
							Path=fullFilePath
						};
						state.Execute(false);
					}
				}
				_client.DisconnectIfNeeded(hadToConnect);
				await Task.Run(() => { });//Gets rid of a compiler warning and does nothing.
			}
		}

		public class Download : TaskStateDownload {
			internal SftpClient _client;

			public Download(string host,string user,string pass,int port=22) {
				_client=Init(host,user,pass,port);
			}

			internal Download() {

			}

			protected override async Task PerformIO() {
				bool hadToConnect=_client.ConnectIfNeeded();
				string fullFilePath=ODFileUtils.CombinePaths(Folder,FileName,'/');
				SftpFileAttributes attribute=_client.GetAttributes(fullFilePath);
				using(MemoryStream stream=new MemoryStream()) {
					SftpDownloadAsyncResult res=(SftpDownloadAsyncResult)_client.BeginDownloadFile(fullFilePath,stream);
					while(!res.AsyncWaitHandle.WaitOne(100)) {
						if(DoCancel) {
							res.IsDownloadCanceled=true;
							_client.DisconnectIfNeeded(hadToConnect);
							return;
						}
						OnProgress((double)res.DownloadedBytes/(double)1024/(double)1024,"?currentVal MB of ?maxVal MB downloaded",(double)attribute.Size/(double)1024/(double)1024,"");
					}
					_client.EndDownloadFile(res);
					FileContent=stream.ToArray();
				}
				_client.DisconnectIfNeeded(hadToConnect);
				await Task.Run(() => { });//Gets rid of a compiler warning and does nothing.
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
				stateDownload.Execute(false);//We could get cute later and make this match isAsync to update the main thread if they are downloading a large file.
				FileContent=stateDownload.FileContent;
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
									ProgressHandler=ProgressHandler,
									HasExceptions=HasExceptions
								};
								stateDown.Execute(ProgressHandler!=null);
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
									FileContent=stateDown.FileContent,
									ProgressHandler=ProgressHandler,
									HasExceptions=HasExceptions
								};
								stateUp.Execute(ProgressHandler!=null);
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
					finally {
						if(IsCopy) {
							OnProgress(i+1,"?currentVal files of ?maxVal files copied",CountTotal,"");
						}
						else {
							OnProgress(i+1,"?currentVal files of ?maxVal files moved",CountTotal,"");
						}
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
				_stateMove.ProgressHandler=ProgressHandler;
				_stateMove.Execute(true);
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
