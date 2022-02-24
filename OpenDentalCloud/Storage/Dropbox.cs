using CodeBase;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;
using OpenDentalCloud.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDentalCloud {
	public class Dropbox {

		///<summary>Called by OAuth web app to display this URL in their browser.</summary>
		public static string GetDropboxAuthorizationUrl(string appkey) {
			try {
				string ret=DropboxOAuth2Helper.GetAuthorizeUri(appkey).ToString();
				if(string.IsNullOrEmpty(ret)) {
					throw new Exception("Invalid URL returned by Dropbox");
				}
				return ret;
			}
			catch(Exception e) {
				throw new ApplicationException(e.Message,e);
			}
		}

		///<summary>Called by Open Dental Proper to get the real access code form the code given by Dropbox.  Returns empty string if something went wrong.</summary>
		public static string GetDropboxAccessToken(string code,string appkey,string appsecret) {
			string ret="";
			ApplicationException ae=null;
			ManualResetEvent wait=new ManualResetEvent(false);
			new Task(async () => {
				try {
					OAuth2Response resp=await DropboxOAuth2Helper.ProcessCodeFlowAsync(code,appkey,appsecret);
					if(string.IsNullOrEmpty(resp.AccessToken)) {
						throw new Exception("Empty token returned by Dropbox.");
					}
					ret=resp.AccessToken;
				}
				catch(Exception ex) {
					ae=new ApplicationException(ex.Message,ex);
				}
				wait.Set();
			}).Start();
			wait.WaitOne(10000);
			if(ae!=null) {
				throw ae;
			}
			return ret;
		}

		public class Upload:TaskStateUpload {
			DropboxClient _client;

			///<summary>Upload only supports WriteMode.Overwrite as of right now.</summary>
			public Upload(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			protected override async Task PerformIO() {
				int numOfChunks=Math.Max(FileContent.Length/MAX_FILE_SIZE_BYTES+1,2);//Add 1 so that we are under the max file size limit, since an integer will truncate remainders.
				int chunkSize=FileContent.Length/numOfChunks;
				string sessionId=null;
				int index=0;
				for(int i=1;i<=numOfChunks;i++) {
					if(DoCancel) {
						throw new Exception("Operation cancelled by user");
					}
					bool lastChunk=i==numOfChunks;
					int curChunkSize=chunkSize;
					if(lastChunk) {
						curChunkSize=FileContent.Length-index;
					}
					using(MemoryStream memStream=new MemoryStream(FileContent,index,curChunkSize)) {
						if(i==1) {
							UploadSessionStartResult result=await _client.Files.UploadSessionStartAsync(false,memStream);
							sessionId=result.SessionId;
						}
						else {
							UploadSessionCursor cursor=new UploadSessionCursor(sessionId,(ulong)index);
							if(lastChunk) {
								//Always forcing Dropbox to overwrite any conflicting files.  
								//Otherwise a Dropbox.Api.Files.UploadSessionFinishError.Path error will be returned by Dropbox if there is a "path/conflict/file/..."
								await _client.Files.UploadSessionFinishAsync(cursor
									,new CommitInfo(ODFileUtils.CombinePaths(Folder,FileName,'/'),WriteMode.Overwrite.Instance)
									,memStream);
							}
							else {
								await _client.Files.UploadSessionAppendV2Async(cursor,false,memStream);
							}
						}
						index+=curChunkSize;
					}
					OnProgress((double)(chunkSize*i)/(double)1024/(double)1024,"?currentVal MB of ?maxVal MB uploaded",(double)FileContent.Length/(double)1024/(double)1024,"");					
				}
			}

		}

		public class Download : TaskStateDownload {
			DropboxClient _client;

			public Download(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			protected override async Task PerformIO() {
				if(!FileExists(_client,ODFileUtils.CombinePaths(Folder,FileName,'/'))) {
					throw new Exception("File not found.");
				}
				IDownloadResponse<FileMetadata> response=await _client.Files.DownloadAsync(ODFileUtils.CombinePaths(Folder,FileName,'/'));
				DownloadFileSize=response.Response.Size;
				ulong numChunks=DownloadFileSize/MAX_FILE_SIZE_BYTES+1;
				int chunkSize=(int)DownloadFileSize/(int)numChunks;
				byte[] buffer=new byte[chunkSize];
				byte[] finalBuffer=new byte[DownloadFileSize];
				int index=0;
				using(Stream stream=await response.GetContentAsStreamAsync()) {
					int length=0;
					do {
						if(DoCancel) {
							throw new Exception("Operation cancelled by user");
						}
						length=stream.Read(buffer,0,chunkSize);
						//Convert each chunk to a MemoryStream. This plays nicely with garbage collection.
						using(MemoryStream memstream=new MemoryStream()) {
							memstream.Write(buffer,0,length);
							Array.Copy(memstream.ToArray(),0,finalBuffer,index,length);
							index+=length;
							OnProgress((double)index/(double)1024/(double)1024,"?currentVal MB of ?maxVal MB downloaded",(double)DownloadFileSize/(double)1024/(double)1024,"");
						}
					} while(length>0);
				}
				FileContent=finalBuffer;
			}

		}

		public class Delete : TaskStateDelete {
			DropboxClient _client;

			public Delete(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			protected override async Task PerformIO() {
				//If path is a folder, all contents will be deleted.
				await _client.Files.DeleteAsync(Path);
			}
		}

		public class Thumbnail:TaskStateThumbnail {
			DropboxClient _client;

			public Thumbnail(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			protected override async Task PerformIO() {
				IDownloadResponse<FileMetadata> data=await _client.Files.GetThumbnailAsync(ODFileUtils.CombinePaths(Folder,FileName,'/')
					,size:ThumbnailSize.W128h128.Instance);
				FileContent=await data.GetContentAsByteArrayAsync();
			}
		}

		public class ListFolders:TaskStateListFolders {
			//client needs to be internal so that it can be set from TaskStateMove when getting a list of files to move
			internal DropboxClient _client;

			internal ListFolders() {
			}

			public ListFolders(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			protected override async Task PerformIO() {
				ListFolderResult data =await _client.Files.ListFolderAsync(FolderPath);
				ListFolderPathLower=data.Entries.Select(x => x.PathLower).ToList();
				ListFolderPathsDisplay=data.Entries.Select(x => x.PathDisplay).ToList();
			}
		}

		public class Move:TaskStateMove {
			internal DropboxClient _client;
			internal bool IsCopy=false;

			public Move(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			internal Move() {
			}

			protected override async Task PerformIO() {
				List<string> listFilePaths;
				bool isFile=(await _client.Files.GetMetadataAsync(FromPath)).IsFile;
				if(isFile) {
					listFilePaths=new List<string> { FromPath };
				}
				else {//Copying a directory
					if(DoCancel) {
						return;
					}
					//Only include files (not sub-directories) in the list
					listFilePaths=(await _client.Files.ListFolderAsync(FromPath)).Entries
						.Where(x => x.IsFile)
						.Select(x => x.Name).ToList();
				}
				CountTotal=listFilePaths.Count;
				for(int i=0;i<CountTotal;i++) {
					if(DoCancel) {
						return;
					}
					string path=listFilePaths[i];
					try {
						string fileName=Path.GetFileName(path);
						string toPathFull=ToPath;
						if(!isFile) {
							toPathFull=ODFileUtils.CombinePaths(ToPath,fileName,'/');
						}
						if(FileExists(_client,toPathFull)) {
							throw new Exception();//Throw so that we can iterate CountFailed
						}
						string fromPathFull=FromPath;
						if(!isFile) {
							fromPathFull=ODFileUtils.CombinePaths(FromPath,fileName,'/');
						}
						//Throws if fails.
						if(IsCopy) {
							await _client.Files.CopyAsync(fromPathFull,toPathFull);
						}
						else {
							await _client.Files.MoveAsync(ODFileUtils.CombinePaths(FromPath,fromPathFull,'/'),toPathFull);
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
			}
		}

		public class Copy:TaskStateCopy {
			protected DropboxClient _client;

			public Copy(string accessToken) {
				_client=new DropboxClient(accessToken);
			}

			protected override async Task PerformIO() {
				TaskStateMove stateMove=new Move() { _client=this._client,IsCopy=true };
				stateMove.FromPath=FromPath;
				stateMove.ToPath=ToPath;
				stateMove.ProgressHandler=ProgressHandler;
				stateMove.Execute(true);
				while(!stateMove.IsDone) {
					Thread.Sleep(10);
					if(DoCancel) {
						stateMove.DoCancel=true;
					}
				}
				await Task.Run(() => {});
			}
		}	

		///<summary>Synchronous.  Returns true if a file exists in the passed in filePath</summary>
		public static bool FileExists(string accessToken,string filePath) {
			return FileExists(new DropboxClient(accessToken),filePath);			
		}

		///<summary>Synchronous.  Returns true if a file exists in the passed in filePath</summary>
		private static bool FileExists(DropboxClient client,string filePath) {
			bool retVal=false;
			ManualResetEvent wait=new ManualResetEvent(false);
			new System.Threading.Tasks.Task(async () => {
				try {
					Metadata data=await client.Files.GetMetadataAsync(filePath);
					retVal=true;
				}
				catch(Exception) {
				}
				wait.Set();
			}).Start();
			if(!wait.WaitOne(10000)) {
				throw new Exception("Checking if file exists in Dropbox timed out.");
			}
			return retVal;
		}
	}
}
