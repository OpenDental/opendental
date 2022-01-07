using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using Tamir.SharpSsh.jsch;
using System.Collections;
using System.Text.RegularExpressions;

namespace OpenDentHL7 {
	public partial class ServiceHL7:ServiceBase {
		private bool IsVerboseLogging;
		private System.Threading.Timer timerSendFiles;
		private System.Threading.Timer timerReceiveFiles;
		private System.Threading.Timer timerSendTCP;
		private System.Threading.Timer timerSendConnectTCP;
		private System.Threading.Timer _timerMedLabSftpGetFiles;
		private System.Threading.Timer _timerSftpGetFiles;
		private Socket _socketIncomingMain;
		private string hl7FolderIn;
		private string hl7FolderOut;
		private static bool isReceivingFiles;
		private const char MLLP_START_CHAR=(char)11;// HEX 0B
		private const char MLLP_END_CHAR=(char)28;// HEX 1C
		private const char MLLP_ENDMSG_CHAR=(char)13;// HEX 0D
		private const char MLLP_ACK_CHAR=(char)6;// HEX 06
		private const char MLLP_NACK_CHAR=(char)21;// HEX 15
		private HL7Def HL7DefEnabled;
		private static bool IsSendTCPConnected;
		private static bool _ecwFileModeIsSending;
		private static DateTime _ecwDateTimeOldMsgsDeleted;
		private static bool _ecwTCPModeIsSending;
		private static bool _ecwTCPSendSocketIsConnected;
		// ManualResetEvent instances signal completion.
		private static bool _ecwTCPModeIsReceiving;//this is set to true right before an asynchronous BeginReceive call.  Right before closing the worker socket, it is set to false.  Otherwise the socket.Close() triggers the callback function and the EndReceive is referencing a disposed object.
		private static ManualResetEvent connectDone=new ManualResetEvent(false);
		private static ManualResetEvent receiveDone=new ManualResetEvent(false);
		private static bool _medLabSftpModeIsReceiving;
		private HL7Def _medLabHL7DefEnabled;
		private static bool _sftpModeIsReceiving;

		public ServiceHL7() {
			InitializeComponent();
			CanStop = true;
			IsSendTCPConnected=true;//bool to keep track of whether connection attempts have failed in the past. At startup, the 'previous' attempt is assumed successful.
			EventLog.WriteEntry("OpenDentHL7",DateTime.Now.ToLongTimeString()+" - Initialized.");
		}

		protected override void OnStart(string[] args) {
			StartManually();
		}

		public void StartManually() {
			//connect to OD db.
			XmlDocument document=new XmlDocument();
			string pathXml=Path.Combine(Application.StartupPath,"FreeDentalConfig.xml");
			try {
				document.Load(pathXml);
			}
			catch {
				EventLog.WriteEntry("OpenDentHL7",DateTime.Now.ToLongTimeString()+" - Could not find "+pathXml,EventLogEntryType.Error);
				throw new ApplicationException("Could not find "+pathXml);
			}
			XPathNavigator Navigator=document.CreateNavigator();
			XPathNavigator nav;
			DataConnection.DBtype=DatabaseType.MySql;
			nav=Navigator.SelectSingleNode("//DatabaseConnection");
			string computerName=nav.SelectSingleNode("ComputerName").Value;
			string database=nav.SelectSingleNode("Database").Value;
			string user=nav.SelectSingleNode("User").Value;
			string password=nav.SelectSingleNode("Password").Value;
			XPathNavigator encryptedPwdNode=nav.SelectSingleNode("MySQLPassHash");//only use the encrypted pwd node is the password node is empty
			string decryptedPwd;
			if(password=="" && encryptedPwdNode!=null && encryptedPwdNode.Value!="" && CDT.Class1.Decrypt(encryptedPwdNode.Value,out decryptedPwd)) {
				password=decryptedPwd;//password will still be blank if a blank password was encrypted
			}
			XPathNavigator verboseNav=Navigator.SelectSingleNode("//HL7verbose");
			if(verboseNav!=null && verboseNav.Value=="True") {
				IsVerboseLogging=true;
				EventLog.WriteEntry("OpenDentHL7","Verbose mode.",EventLogEntryType.Information);
			}
			DataConnection dcon=new DataConnection();
			//Try to connect to the database directly
			try {
				dcon.SetDb(computerName,database,user,password,"","",DataConnection.DBtype);
				//a direct connection does not utilize lower privileges.
				RemotingClient.RemotingRole=RemotingRole.ClientDirect;
			}
			catch {//(Exception ex){
				throw new ApplicationException("Connection to database failed.");
			}
			//check db version
			string dbVersion=PrefC.GetString(PrefName.ProgramVersion);
			if(Application.ProductVersion.ToString() != dbVersion) {
				EventLog.WriteEntry("OpenDentHL7","Versions do not match.  Db version:"+dbVersion+".  Application version:"+Application.ProductVersion.ToString(),EventLogEntryType.Error);
				throw new ApplicationException("Versions do not match.  Db version:"+dbVersion+".  Application version:"+Application.ProductVersion.ToString());
			}
			//connected to the database, set the current user
			//Security.CurUser=Userods.GetUser(PrefC.GetLong(PrefName.ODServiceSecUserNum));
			Security.CurUser=new Userod() { UserName="ODServiceSecUser" };
			#region MedLab HL7
			_medLabHL7DefEnabled=HL7Defs.GetOneDeepEnabled(true);
			if(_medLabHL7DefEnabled!=null) {
				if(_medLabHL7DefEnabled.HL7Server=="") {
					_medLabHL7DefEnabled.HL7Server=System.Environment.MachineName;
					HL7Defs.Update(_medLabHL7DefEnabled);
				}
				if(_medLabHL7DefEnabled.HL7ServiceName=="") {
					_medLabHL7DefEnabled.HL7ServiceName=this.ServiceName;
					HL7Defs.Update(_medLabHL7DefEnabled);
				}
				if(_medLabHL7DefEnabled.HL7Server.ToLower()!=System.Environment.MachineName.ToLower()) {
					EventLog.WriteEntry("OpenDentHL7","The HL7 Server name does not match the name in the enabled MedLab HL7Def Setup.  Server name: "
						+System.Environment.MachineName+", Server name in MedLab HL7Def: "+_medLabHL7DefEnabled.HL7Server,EventLogEntryType.Error);
					throw new ApplicationException("The HL7 Server name does not match the name in the enabled MedLab HL7Def Setup.  Server name: "
						+System.Environment.MachineName+", Server name in MedLab HL7Def: "+_medLabHL7DefEnabled.HL7Server);
				}
				if(_medLabHL7DefEnabled.HL7ServiceName.ToLower()!=this.ServiceName.ToLower()) {
					EventLog.WriteEntry("OpenDentHL7","The MedLab HL7 Service Name does not match the name in the enabled MedLab HL7Def Setup.  Service name: "
						+this.ServiceName+", Service name in MedLab HL7Def: "+_medLabHL7DefEnabled.HL7ServiceName,EventLogEntryType.Error);
					throw new ApplicationException("The MedLab HL7 Service Name does not match the name in the enabled MedLab HL7Def Setup.  Service name: "
						+this.ServiceName+", Service name in MedLab HL7Def: "+_medLabHL7DefEnabled.HL7ServiceName);
				}
				_medLabSftpModeIsReceiving=false;
				TimerCallback timerCallbackLabSftpGetFiles=new TimerCallback(TimerCallbackSftpGetFiles);
				_timerMedLabSftpGetFiles=new System.Threading.Timer(timerCallbackLabSftpGetFiles,true,1000,60000);//attempt to connect to the sftp server once a minute
			}
			#endregion MedLab HL7
			#region eCW Send and Receive OLD
			if(Programs.IsEnabled(ProgramName.eClinicalWorks) && !HL7Defs.IsExistingHL7Enabled()) {//eCW enabled, and no HL7def enabled.
				//prevent startup:
				long progNum=Programs.GetProgramNum(ProgramName.eClinicalWorks);
				string hl7Server=ProgramProperties.GetPropVal(progNum,"HL7Server");//this property will not exist if using Oracle, eCW will never use Oracle
				string hl7ServiceName=ProgramProperties.GetPropVal(progNum,"HL7ServiceName");//this property will not exist if using Oracle, eCW will never use Oracle
				if(hl7Server=="") {//for the first time run
					ProgramProperties.SetProperty(progNum,"HL7Server",System.Environment.MachineName);//this property will not exist if using Oracle, eCW will never use Oracle
					hl7Server=System.Environment.MachineName;
				}
				if(hl7ServiceName=="") {//for the first time run
					ProgramProperties.SetProperty(progNum,"HL7ServiceName",this.ServiceName);//this property will not exist if using Oracle, eCW will never use Oracle
					hl7ServiceName=this.ServiceName;
				}
				if(hl7Server.ToLower()!=System.Environment.MachineName.ToLower()) {
					EventLog.WriteEntry("OpenDentHL7","The HL7 Server name does not match the name set in Program Links eClinicalWorks Setup.  Server name: "+System.Environment.MachineName
						+", Server name in Program Links: "+hl7Server,EventLogEntryType.Error);
					throw new ApplicationException("The HL7 Server name does not match the name set in Program Links eClinicalWorks Setup.  Server name: "+System.Environment.MachineName
						+", Server name in Program Links: "+hl7Server);
				}
				if(hl7ServiceName.ToLower()!=this.ServiceName.ToLower()) {
					EventLog.WriteEntry("OpenDentHL7","The HL7 Service Name does not match the name set in Program Links eClinicalWorks Setup.  Service name: "+this.ServiceName+", Service name in Program Links: "
						+hl7ServiceName,EventLogEntryType.Error);
					throw new ApplicationException("The HL7 Service Name does not match the name set in Program Links eClinicalWorks Setup.  Service name: "+this.ServiceName+", Service name in Program Links: "
						+hl7ServiceName);
				}
				EcwOldSendAndReceive();
				return;
			}
			#endregion eCW Send and Receive OLD
			#region HL7 Send and Receive New Defs
			HL7Def hL7Def=HL7Defs.GetOneDeepEnabled();
			if(hL7Def==null) {
				return;
			}
			if(hL7Def.HL7Server=="") {
				hL7Def.HL7Server=System.Environment.MachineName;
				HL7Defs.Update(hL7Def);
			}
			if(hL7Def.HL7ServiceName=="") {
				hL7Def.HL7ServiceName=this.ServiceName;
				HL7Defs.Update(hL7Def);
			}
			if(hL7Def.HL7Server.ToLower()!=System.Environment.MachineName.ToLower()) {
				EventLog.WriteEntry("OpenDentHL7","The HL7 Server name does not match the name in the enabled HL7Def Setup.  Server name: "+System.Environment.MachineName+", Server name in HL7Def: "+hL7Def.HL7Server,
					EventLogEntryType.Error);
				throw new ApplicationException("The HL7 Server name does not match the name in the enabled HL7Def Setup.  Server name: "+System.Environment.MachineName+", Server name in HL7Def: "+hL7Def.HL7Server);
			}
			if(hL7Def.HL7ServiceName.ToLower()!=this.ServiceName.ToLower()) {
				EventLog.WriteEntry("OpenDentHL7","The HL7 Service Name does not match the name in the enabled HL7Def Setup.  Service name: "+this.ServiceName+", Service name in HL7Def: "+hL7Def.HL7ServiceName,
					EventLogEntryType.Error);
				throw new ApplicationException("The HL7 Service Name does not match the name in the enabled HL7Def Setup.  Service name: "+this.ServiceName+", Service name in HL7Def: "+hL7Def.HL7ServiceName);
			}
			HL7DefEnabled=hL7Def;//so we can access it later from other methods
			#region File Mode
			if(HL7DefEnabled.ModeTx==ModeTxHL7.File) {
				hl7FolderOut=HL7DefEnabled.OutgoingFolder;
				hl7FolderIn=HL7DefEnabled.IncomingFolder;
				if(!Directory.Exists(hl7FolderOut)) {
					EventLog.WriteEntry("OpenDentHL7","The outgoing HL7 folder does not exist.  Path is set to: "+hl7FolderOut,EventLogEntryType.Error);
					throw new ApplicationException("The outgoing HL7 folder does not exist.  Path is set to: "+hl7FolderOut);
				}
				if(!Directory.Exists(hl7FolderIn)) {
					EventLog.WriteEntry("OpenDentHL7","The incoming HL7 folder does not exist.  Path is set to: "+hl7FolderIn,EventLogEntryType.Error);
					throw new ApplicationException("The incoming HL7 folder does not exist.  Path is set to: "+hl7FolderIn);
				}
				_ecwDateTimeOldMsgsDeleted=DateTime.MinValue;
				_ecwFileModeIsSending=false;
				//start polling the folder for waiting messages to import.  Every 5 seconds.
				TimerCallback timercallbackReceive=new TimerCallback(TimerCallbackReceiveFiles);
				timerReceiveFiles=new System.Threading.Timer(timercallbackReceive,null,5000,5000);
				//start polling the db for new HL7 messages to send. Every 1.8 seconds.
				TimerCallback timercallbackSend=new TimerCallback(TimerCallbackSendFiles);
				timerSendFiles=new System.Threading.Timer(timercallbackSend,null,1800,1800);
			}
			#endregion File Mode
			#region TCP/IP Mode
			else if(HL7DefEnabled.ModeTx==ModeTxHL7.TcpIp) {
				CreateIncomingTcpListener();//this method spawns a new thread for receiving, the main thread returns to perform the sending below
				_ecwTCPSendSocketIsConnected=false;
				_ecwTCPModeIsSending=false;
				//start a timer to connect to the send socket every 20 seconds.  The socket will be reused, so if _ecwTCPSendSocketIsConnected is true, this will just return
				TimerCallback timercallbackSendConnectTCP=new TimerCallback(TimerCallbackSendConnectTCP);
				timerSendConnectTCP=new System.Threading.Timer(timercallbackSendConnectTCP,null,1800,20000);//every 20 seconds, re-connect to the socket if the connection has been closed
			}
			#endregion TCP/IP Mode
			#region SFTP Mode
			else if(HL7DefEnabled.ModeTx==ModeTxHL7.Sftp) {
				_sftpModeIsReceiving=false;
				TimerCallback timerCallbackSftpGetFiles=new TimerCallback(TimerCallbackSftpGetFiles);
				_timerSftpGetFiles=new System.Threading.Timer(timerCallbackSftpGetFiles,false,1000,60000);//attempt to connect to the sftp server once a minute
			}
			#endregion SFTP Mode
			#endregion HL7 Send and Receive New Defs
		}

		#region SFTP Mode
		///<summary>Runs in a separate thread, called once per minute.  If not already retrieving and processing files, this will connect to
		///the SFTP server, check for new files to retrieve, process the files, and disconnect from the server.  Once the files are read,
		///they are deleted by the SFTP server automatically, so it is not necessary to call a remove/delete function.
		///If stateInfo is true, this is a MedLab HL7 definition call, so process through MessageParserMedLab.Process.
		///If stateInfo is false, this is a normal HL7 definition, so process the files through MessageParser.Process like the other HL7 defs.</summary>
		private void TimerCallbackSftpGetFiles(Object stateInfo) {
			bool isMedLab=(bool)stateInfo;
			if(IsVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","A new thread is spawned once per minute to connect to the SFTP server.  "
						+"If a connection has already been established, this thread will return.",EventLogEntryType.Information);
			}
			if(isMedLab) {
				if(_medLabSftpModeIsReceiving) {
					return;
				}
				_medLabSftpModeIsReceiving=true;
			}
			else {//not MedLab
				if(_sftpModeIsReceiving) {
					return;
				}
				_sftpModeIsReceiving=true;
			}
			//get the message archive processed and failed paths for storing the inbound messages
			string msgArchiveProcessedPath="";
			string msgArchivePath="";
			try {
				if(isMedLab && PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {//only MedLab HL7 interfaces will archive inbound messages
					msgArchivePath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"MedLabHL7");
					msgArchiveProcessedPath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"MedLabHL7","Processed");
					if(!Directory.Exists(msgArchiveProcessedPath)) {
						Directory.CreateDirectory(msgArchiveProcessedPath);
					}
				}
				else if(isMedLab && CloudStorage.IsCloudStorage) {
					msgArchivePath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"MedLabHL7").Replace("\\","/");
					msgArchiveProcessedPath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"MedLabHL7","Processed").Replace("\\","/");
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				//if the above fails, clear out the paths, the messages will not be archived
				msgArchivePath="";
				msgArchiveProcessedPath="";
				EventLog.WriteEntry("OpenDentHL7","The SFTP HL7 message archive directory is not accessible or could not be created.  "
					+"The inbound HL7 messages will not be archived.  Once processed, the original message text will be deleted.",EventLogEntryType.Warning);
			}
			Session session=null;
			Channel ch=null;
			ChannelSftp chsftp=null;
			JSch jsch=new JSch();
			string[] serverHostPort;
			string sftpUsername;
			string sftpPassword;
			if(isMedLab) {
				serverHostPort=_medLabHL7DefEnabled.SftpInSocket.Split(':');
				sftpUsername=_medLabHL7DefEnabled.SftpUsername;
				sftpPassword=CDT.Class1.TryDecrypt(_medLabHL7DefEnabled.SftpPassword);
			}
			else {
				serverHostPort=HL7DefEnabled.SftpInSocket.Split(':');
				sftpUsername=HL7DefEnabled.SftpUsername;
				sftpPassword=CDT.Class1.TryDecrypt(HL7DefEnabled.SftpPassword);
			}
			bool isConnected=false;
			try {
				if(serverHostPort.Length==1) {
					session=jsch.getSession(sftpUsername,serverHostPort[0]);
				}
				else if(serverHostPort.Length>=2) {
					session=jsch.getSession(sftpUsername,serverHostPort[0],int.Parse(serverHostPort[1]));//port verified to be an int
				}
				session.setPassword(sftpPassword);
				Hashtable config=new Hashtable();
				config.Add("StrictHostKeyChecking","no");
				session.setConfig(config);
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Connecting to the SFTP server to retrieve HL7 message files.",EventLogEntryType.Information);
				}
				session.connect(15000);//timeout after 15 seconds
				ch=session.openChannel("sftp");
				ch.connect();
				chsftp=(ChannelSftp)ch;
				//this is the only place this flag is set to true.  If anything above here fails, whether an exception or not, the next line will not run
				//and the finally will set the receiving flag to false for the next thread
				isConnected=true;
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","Could not connect to the SFTP server using the values in the enabled HL7 definition.  "
					+"Another connection attempt will occur in 1 minute.\r\n"+ex.Message,EventLogEntryType.Information);
				if(isMedLab) {
					_medLabSftpModeIsReceiving=false;
				}
				else {
					_sftpModeIsReceiving=false;
				}
				return;
			}
			finally {
				if(!isConnected) {//an error happened, make sure the session and channels are disconnected and reset the receiving flag
					if(chsftp!=null) {
						chsftp.disconnect();
					}
					if(ch!=null) {
						ch.disconnect();
					}
					if(session!=null) {
						session.disconnect();
					}
				}
			}
			if(!isConnected) {
				if(isMedLab) {
					_medLabSftpModeIsReceiving=false;
				}
				else {
					_sftpModeIsReceiving=false;
				}
				return;
			}
			string resultsPath=".";//normally with results only interfaces the results will just be in the root directory
			//we allow the user to specify a path to a different results directory.  Monitor this folder for new result files if the directory exists.
			//if the directory specified does not exist, monitor the root or home directory
			string sftpInFolder="";
			if(isMedLab) {
				sftpInFolder=_medLabHL7DefEnabled.IncomingFolder;
			}
			else {
				sftpInFolder=HL7DefEnabled.IncomingFolder;
			}
			if(sftpInFolder!="") {
				string[] pathDirs=sftpInFolder.Split(new string[] { "/","\\" },StringSplitOptions.RemoveEmptyEntries);//allow for "\" or "/" in the path
				for(int i=0;i<pathDirs.Length;i++) {
					if(pathDirs[i]==".") {//already added the '.', skip if they specified one in the results path field
						continue;
					}
					resultsPath+="/"+pathDirs[i];
				}
			}
			Tamir.SharpSsh.java.util.Vector fileList=null;
			try {//test to see if the path specified is a valid path, otherwise revert to the default root directory
				fileList=chsftp.ls(resultsPath);//if this throws an exception, it is probably because the directory specified doesn't exist
			}
			catch(Exception ex) {
				ex.DoNothing();
				EventLog.WriteEntry("OpenDentHL7","Could not locate the results directory specified in the enabled SFTP HL7 definition.  "
					+"Retrieving files from the root directory instead.",EventLogEntryType.Information);
				resultsPath=".";
			}
			int countFilesFound=0;
			int countFilesProcessed=0;
			try {
				//At this point we are connected to the LabCorp SFTP server.
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Getting list of file names to retrieve from the SFTP server.",EventLogEntryType.Information);
				}
				if(fileList==null) {//fileList will be initialized if the path specified in the def was valid, otherwise default to root dir and fill here
					fileList=chsftp.ls(resultsPath);//fileList will be ArrayList filled with LsEntry objects
				}
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Processing SFTP HL7 message files.",EventLogEntryType.Information);
				}
				if(fileList.Count==0) {
					return;//no files to process, return to try again in 60 seconds
				}
				for(int i=0;i<fileList.Count;i++) {
					StringBuilder sb=new StringBuilder();
					string listItem=fileList[i].ToString().Trim();//LsEntry.ToString() overridden to return the file's longname
					if(listItem[0]=='d') {
						continue;//skip directories
					}
					countFilesFound++;
					Match filenameMatch=Regex.Match(listItem,".*\\s+(.*)$");
					string filePath=resultsPath+"/"+filenameMatch.Result("$1");
					Tamir.SharpSsh.java.io.InputStream fileStream=null;
					try {
						if(IsVerboseLogging) {
							EventLog.WriteEntry("OpenDentHL7","Begin reading in the next SFTP HL7 file.",EventLogEntryType.Information);
						}
						fileStream=chsftp.get(filePath);
						byte[] fileBytes=new byte[100000];//100,000 byte chunks, 350 KB file size, average time to retrieve file 3.2 seconds
						int numBytes=fileStream.Read(fileBytes,0,fileBytes.Length);
						while(numBytes>0) {
							sb.Append(Tamir.SharpSsh.java.String.getStringUTF8(fileBytes,0,numBytes));
							numBytes=fileStream.Read(fileBytes,0,fileBytes.Length);
						}
						string msgArchiveFilePath="";
						string fileName="";
						if(isMedLab && msgArchivePath!="") {
							if(IsVerboseLogging) {
								EventLog.WriteEntry("OpenDentHL7","Create the archive of the SFTP HL7 message "+msgArchivePath,EventLogEntryType.Information);
							}
							try {
								if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
									msgArchiveFilePath=ODFileUtils.CreateRandomFile(msgArchivePath,".txt");
									File.WriteAllText(msgArchiveFilePath,sb.ToString());
								}
								else {//Cloud AtoZ
									//Upload file to the cloud
									//Create random file name, this will create the name in the same manner as ODFileUtils.CreateRandomFile().
									string randChrs="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
									System.Random rand=new System.Random();
									for(int j=0;j<6;j++){
										fileName+=randChrs[rand.Next(0,randChrs.Length-1)];
									}
									CloudStorage.Upload(msgArchivePath,fileName,Encoding.ASCII.GetBytes(sb.ToString()));
								}
							}
							catch(Exception ex) {
								ex.DoNothing();
								//do nothing, don't archive the message, might be a permission issue
							}
						}
						if(IsVerboseLogging) {
							EventLog.WriteEntry("OpenDentHL7","Create the SFTP HL7 Message Object from the file text.",EventLogEntryType.Information);
						}
						MessageHL7 messageHl7Object=new MessageHL7(sb.ToString());//this creates an entire heirarchy of objects.
						if(IsVerboseLogging) {
							EventLog.WriteEntry("OpenDentHL7","Process the SFTP HL7 message.",EventLogEntryType.Information);
						}
						List<long> medLabNumList=new List<long>();
						if(isMedLab) {
							medLabNumList=MessageParserMedLab.Process(messageHl7Object,msgArchiveFilePath,IsVerboseLogging);
						}
						else {
							MessageParser.Process(messageHl7Object,IsVerboseLogging);
						}
						if(isMedLab && msgArchiveProcessedPath!="" && msgArchiveFilePath!="" && PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
							try {
								string msgArchiveFilePathProcessed=ODFileUtils.CombinePaths(msgArchiveProcessedPath,Path.GetFileName(msgArchiveFilePath));
								File.Move(msgArchiveFilePath,msgArchiveFilePathProcessed);
								MedLabs.UpdateFileNames(medLabNumList,ODFileUtils.CombinePaths("MedLabHL7","Processed",Path.GetFileName(msgArchiveFilePathProcessed)));
							}
							catch(Exception ex) {
								ex.DoNothing();
								//do nothing, the file will remain in the root location
							}
						}
						if(isMedLab && msgArchiveProcessedPath!="" && CloudStorage.IsCloudStorage) {//MsgArchiveFilePath will be blank here
							MedLabs.UpdateFileNames(medLabNumList,ODFileUtils.CombinePaths("MedLabHL7","Processed",fileName).Replace("\\","/"));
						}
						countFilesProcessed++;
						//chsftp.rm(filePath);//not necessary to remove the file for MedLab, once it is read it is deleted.
					}
					catch(Exception ex) {
						EventLog.WriteEntry("OpenDentHL7","Error retrieving or processing a SFTP HL7 message.  If the file was not processed, it will remain "
							+"on the server and another attempt will be made later.\r\n"+ex.Message,EventLogEntryType.Information);
						continue;
					}
					finally {
						if(fileStream!=null) {
							fileStream.Dispose();
						}
					}
				}
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","Error retrieving or processing a SFTP HL7 message.  Will retry in 1 minute.\r\n"
					+ex.Message,EventLogEntryType.Information);
				return;
			}
			finally {
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Files found: "+countFilesFound.ToString()+", Files Processed: "
						+countFilesProcessed.ToString()+"\r\nDisconnecting from the SFTP server.",EventLogEntryType.Information);
				}
				//Disconnect from the SFTP server.
				if(chsftp!=null) {
					chsftp.disconnect();
				}
				if(ch!=null) {
					ch.disconnect();
				}
				if(session!=null) {
					session.disconnect();
				}
				if(isMedLab) {
					_medLabSftpModeIsReceiving=false;
				}
				else {
					_sftpModeIsReceiving=false;
				}
			}
		}
		#endregion SFTP Mode

		#region File Mode
		private void TimerCallbackReceiveFiles(Object stateInfo) {
			//process all waiting messages
			if(isReceivingFiles) {
				return;//already in the middle of processing files
			}
			isReceivingFiles=true;
			string[] existingFiles=Directory.GetFiles(hl7FolderIn);
			for(int i=0;i<existingFiles.Length;i++) {
				ProcessMessageFile(existingFiles[i]);
			}
			isReceivingFiles=false;
		}

		private void ProcessMessageFile(string fullPath) {
			string msgtext="";
			int i=0;
			while(i<5) {
				try {
					msgtext=File.ReadAllText(fullPath);
					break;
				}
				catch {
				}
				Thread.Sleep(200);
				i++;
				if(i==5) {
					EventLog.WriteEntry("Could not read text from file due to file locking issues.",EventLogEntryType.Error);
					return;
				}
			}
			try {
				MessageHL7 msg=new MessageHL7(msgtext);//this creates an entire heirarchy of objects.
				MessageParser.Process(msg,IsVerboseLogging);
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Processed message "+msg.MsgType.ToString(),EventLogEntryType.Information);
				}
			}
			catch(Exception ex) {
				EventLog.WriteEntry(ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Error);
				return;
			}
			try {
				File.Delete(fullPath);
			}
			catch(Exception ex) {
				EventLog.WriteEntry("Delete failed for "+fullPath+"\r\n"+ex.Message,EventLogEntryType.Error);
			}
		}

		protected override void OnStop() {
			//later: inform od via signal that this service has shut down
			EcwOldStop();
			if(timerSendFiles!=null) {
				timerSendFiles.Dispose();
			}
		}

		private void TimerCallbackSendFiles(Object stateInfo) {
			if(_ecwFileModeIsSending) {//if there is a thread that is still sending, return
				return;
			}
			try {
				_ecwFileModeIsSending=true;
				if(IsVerboseLogging) {
					EventLog.WriteEntry("GetOnePending Start");
				}
				List<HL7Msg> list=HL7Msgs.GetOnePending();
				if(IsVerboseLogging) {
					EventLog.WriteEntry("GetOnePending Finished");
				}
				string filename;
				for(int i=0;i<list.Count;i++) {//Right now, there will only be 0 or 1 item in the list.
					filename=ODFileUtils.CreateRandomFile(hl7FolderOut,".txt");
					File.WriteAllText(filename,list[i].MsgText);
					list[i].HL7Status=HL7MessageStatus.OutSent;
					HL7Msgs.Update(list[i]);//set the status to sent.
				}
				if(_ecwDateTimeOldMsgsDeleted.Date<DateTime.Now.Date) {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("DeleteOldMsgText Starting");
					}
					_ecwDateTimeOldMsgsDeleted=DateTime.Now;//If DeleteOldMsgText fails for any reason.  This will cause it to not get called until the next day instead of with every msg.
					HL7Msgs.DeleteOldMsgText();//this function deletes if DateTStamp is less than CURDATE-INTERVAL 4 MONTH.  That means it will delete message text only once a day, not time based.
					if(IsVerboseLogging) {
						EventLog.WriteEntry("DeleteOldMsgText Finished");
					}
				}
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7 error when sending HL7 message: "+ex.Message,EventLogEntryType.Warning);//Warning because the service will spawn a new thread in 1.8 seconds
			}
			finally {
				_ecwFileModeIsSending=false;
			}
		}
		#endregion File Mode

		#region TCP/IP Mode
		private void CreateIncomingTcpListener() {
			//Use Minimal Lower Layer Protocol (MLLP):
			//To send a message:              StartBlockChar(11) -          Payload            - EndBlockChar(28) - EndDataChar(13).
			//An ack message looks like this: StartBlockChar(11) - AckChar(0x06)/NakChar(0x15) - EndBlockChar(28) - EndDataChar(13).
			//Ack is part of MLLP V2.  In it, every message requires an ack or nak.  It's unclear when a nak would be useful.
			//Also in V2, every incoming message must be persisted by storing in our db.
			//We will just start with version 1 and not do acks at first unless needed.
			try {
				_socketIncomingMain=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
				IPEndPoint endpointLocal=new IPEndPoint(IPAddress.Any,int.Parse(HL7DefEnabled.IncomingPort));
				_socketIncomingMain.Bind(endpointLocal);
				_socketIncomingMain.Listen(1);//Listen for and queue incoming connection requests.  There should only be one.
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Listening",EventLogEntryType.Information);
				}
				//Asynchronously process incoming connection attempts:
				_socketIncomingMain.BeginAccept(new AsyncCallback(OnConnectionAccepted),_socketIncomingMain);
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","Error creating incoming TCP listener\r\n"+ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Error);
				throw ex;
				//service will stop working at this point.
			}
		}

		///<summary>Runs in a separate thread.  Considered the main thread for receiving messages.</summary>
		private void OnConnectionAccepted(IAsyncResult asyncResult) {
			if(IsVerboseLogging) {
				EventLog.WriteEntry("OpenDentHL7","Connection Accepted",EventLogEntryType.Information);
			}
			try {
				receiveDone.Reset();//manual reset event to tell the main thread to accept an incoming connection
				Socket socketIncomingHandler=((Socket)asyncResult.AsyncState).EndAccept(asyncResult);//end the BeginAccept.  Get reference to new Socket.
				//Use the worker socket to wait for data.
				//We will keep reusing the same workerSocket instead of maintaining a list of worker sockets
				//because this program is guaranteed to only have one incoming connection at a time.
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","BeginReceive",EventLogEntryType.Information);
				}
				StateObject state=new StateObject();
				state.workSocket=socketIncomingHandler;
				_ecwTCPModeIsReceiving=true;
				socketIncomingHandler.BeginReceive(state.buffer,0,StateObject.BufferSize,SocketFlags.None,new AsyncCallback(OnDataReceived),state);
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","The main receive thread is waiting for the manual reset event to be set before accepting any new connections.",EventLogEntryType.Information);
				}
				if(!receiveDone.WaitOne(new TimeSpan(0,20,0))) {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","The main receive thread timed out waiting for the manual reset event to be set.  A new connection will be accepted.",EventLogEntryType.Information);
					}
					//the main receive thread will wait for OnDataReceived to set the manual reset event receiveDone before accepting any new connections
					//if the receiveDone waits for 20 minutes and there is no reset event set, shutdown the socket and accept a new connection
					try {
						socketIncomingHandler.Shutdown(SocketShutdown.Both);
					}
					catch {
						//Do nothing if the shutdown command fails, just begin accepting incoming connection attempts again.
					}
					socketIncomingHandler.Close();
				}
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","The manual reset event has been set.  The main receive thread is now accepting new connections.",EventLogEntryType.Information);
				}
				//the main socket is now free to wait for another connection.
				_ecwTCPModeIsReceiving=false;
				_socketIncomingMain.BeginAccept(new AsyncCallback(OnConnectionAccepted),_socketIncomingMain);
			}
			catch(ObjectDisposedException ex) {
				EventLog.WriteEntry("OpenDentHL7","Error in OnConnectionAccepted.  Attempting to call CreateIncomingTcpListener again.\r\nException: "+ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Warning);
				//Socket has been closed.  Try to start over.
				_socketIncomingMain.Close();
				_ecwTCPModeIsReceiving=false;
				CreateIncomingTcpListener();//If this fails, service stops running
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","Error in OnConnectionAccpeted:\r\n"+ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Warning);
				_socketIncomingMain.Close();
				_ecwTCPModeIsReceiving=false;
				CreateIncomingTcpListener();//If this fails, service stops running
			}
		}

		///<summary>Runs in a separate thread</summary>
		private void OnDataReceived(IAsyncResult asyncResult) {
			try {
				if(!_ecwTCPModeIsReceiving) {//this will only be false if a socket.Close() call is made, which will trigger this call back function to finish, so just return if false
					return;
				}
				StateObject state=(StateObject)asyncResult.AsyncState;
				if(state==null) {
					throw new Exception("Error in OnDataReceived: The IAsyncResult parameter could not be cast to a StateObject.");
				}
				Socket socketIncomingHandler=state.workSocket;
				int byteCountReceived=0;
				try {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","EndReceive is starting.",EventLogEntryType.Information);
					}
					byteCountReceived=socketIncomingHandler.EndReceive(asyncResult);//blocks until data is received.
				}
				catch(Exception ex) {
					//Socket has been disposed or is null or something went wrong.
					_ecwTCPModeIsReceiving=false;
					socketIncomingHandler.Close();
					throw new Exception("Error in OnDataReceived:\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				}
				char[] chars=new char[byteCountReceived];
				Decoder decoder=Encoding.UTF8.GetDecoder();
				decoder.GetChars(state.buffer,0,byteCountReceived,chars,0);//doesn't necessarily get all bytes from the buffer because buffer could be half full.
				state.strbFullMsg.Append(chars);//sb might already have partial data
				Array.Clear(state.buffer,0,StateObject.BufferSize);//Clear the buffer, ready to receive more
				//I think we are guaranteed to have received at least one char.
				bool isFullMsg=false;
				bool isMalformed=false;
				if(state.strbFullMsg.Length==1 && state.strbFullMsg[0]==MLLP_ENDMSG_CHAR) {//the only char in the message is the end char
					state.strbFullMsg.Clear();//this must be the very end of a previously processed message.  Discard.
					isFullMsg=false;
				}
				//else if(strbFullMsg[0]!=MLLP_START_CHAR) {
				else if(state.strbFullMsg.Length>0 && state.strbFullMsg[0]!=MLLP_START_CHAR) {
					//Malformed message. 
					isFullMsg=true;//we're going to do this so that the error gets saved in the database further down.
					isMalformed=true;
				}
				else if(state.strbFullMsg.Length>=3//so that the next two lines won't crash
					&& state.strbFullMsg[state.strbFullMsg.Length-1]==MLLP_ENDMSG_CHAR//last char is the endmsg char.
					&& state.strbFullMsg[state.strbFullMsg.Length-2]==MLLP_END_CHAR)//the second-to-the-last char is the end char.
				{
					//we have a complete message
					state.strbFullMsg.Remove(0,1);//strip off the start char
					state.strbFullMsg.Remove(state.strbFullMsg.Length-2,2);//strip off the end chars
					isFullMsg=true;
				}
				else if(state.strbFullMsg.Length>=2//so that the next line won't crash
					&& state.strbFullMsg[state.strbFullMsg.Length-1]==MLLP_END_CHAR)//the last char is the end char.
				{
					//we will treat this as a complete message, because the endmsg char is optional.
					//if the endmsg char gets sent in a subsequent block, the code above will discard it.
					state.strbFullMsg.Remove(0,1);//strip off the start char
					state.strbFullMsg.Remove(state.strbFullMsg.Length-1,1);//strip off the end char
					isFullMsg=true;
				}
				else {
					isFullMsg=false;//this is an incomplete message.  Continue to receive more blocks.
				}
				//end of big if statement-------------------------------------------------
				if(!isFullMsg) {
					try {
						if(IsVerboseLogging) {
							EventLog.WriteEntry("OpenDentHL7","Not a full message, BeginReceive called again.",EventLogEntryType.Information);
						}
						//the buffer was cleared after appending the chars to the string builder
						_ecwTCPModeIsReceiving=true;
						IAsyncResult inResultIncomplMsg=socketIncomingHandler.BeginReceive(state.buffer,0,StateObject.BufferSize,SocketFlags.None,new AsyncCallback(OnDataReceived),state);
						if(!inResultIncomplMsg.AsyncWaitHandle.WaitOne(new TimeSpan(0,0,30))) {//WaitOne will return true if data was received, false if the timeout is reached with no data received
							//if we have received part of a message and 30 seconds goes by with no additional data received
							//close the socket and set the receiveDone manual reset event.  A new socket connection will be accepted.
							if(IsVerboseLogging) {
								EventLog.WriteEntry("OpenDentHL7","Setting manual reset event so the main receive thread will accept a new incoming connection.",EventLogEntryType.Information);
							}
							inResultIncomplMsg.AsyncWaitHandle.Close();
							_ecwTCPModeIsReceiving=false;
							socketIncomingHandler.Close();
							receiveDone.Set();
						}
						else {
							inResultIncomplMsg.AsyncWaitHandle.Close();
						}
						return;//get another block or in the event the 30 second timeout is reached, a new socket will be created
					}
					catch(Exception ex) {
						_ecwTCPModeIsReceiving=false;
						socketIncomingHandler.Close();
						throw new Exception("An error occurred with BeginReceive on an incoming TCP/IP HL7 message.\r\nException: "+ex.Message+"\r\n"+ex.StackTrace);
					}
				}
				//Prepare to save message to database if malformed and not processed
				HL7Msg hl7Msg=new HL7Msg();
				hl7Msg.MsgText=state.strbFullMsg.ToString();
				state.strbFullMsg.Clear();//just in case, ready for the next message
				bool isProcessed=true;
				string messageControlId="";
				string ackEvent="";
				if(isMalformed) {
					hl7Msg.HL7Status=HL7MessageStatus.InFailed;
					hl7Msg.Note="This message is malformed so it was not processed.";
					HL7Msgs.Insert(hl7Msg);
					isProcessed=false;
				}
				else {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Create the HL7 Message Object from the message text.",EventLogEntryType.Information);
					}
					MessageHL7 messageHl7Object=new MessageHL7(hl7Msg.MsgText);//this creates an entire heirarchy of objects.
					try {
						if(IsVerboseLogging) {
							EventLog.WriteEntry("OpenDentHL7","Process the HL7 message.",EventLogEntryType.Information);
						}
						ackEvent=messageHl7Object.AckEvent;
						MessageParser.Process(messageHl7Object,IsVerboseLogging);//also saves the message to the db and retrieves the control ID from the MSH segment
						messageControlId=messageHl7Object.ControlId;
					}
					catch(Exception ex) {
						EventLog.WriteEntry("OpenDentHL7","Error in OnDataReceived when processing message:\r\n"+ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Information);
						isProcessed=false;
					}
				}
				MessageHL7 hl7Ack=MessageConstructor.GenerateACK(messageControlId,isProcessed,ackEvent);
				if(hl7Ack==null) {
					_ecwTCPModeIsReceiving=false;
					socketIncomingHandler.Close();
					throw new Exception("No ACK defined for the enabled HL7 definition or no HL7 definition enabled.");
				}
				byte[] ackByteOutgoing=Encoding.ASCII.GetBytes(MLLP_START_CHAR+hl7Ack.ToString()+MLLP_END_CHAR+MLLP_ENDMSG_CHAR);
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Beginning to send ACK.\r\n"+MLLP_START_CHAR+hl7Ack.ToString()+MLLP_END_CHAR+MLLP_ENDMSG_CHAR,EventLogEntryType.Information);
				}
				socketIncomingHandler.SendTimeout=5000;//timeout after 5 seconds of trying to send an acknowledgment
				try {
					socketIncomingHandler.Send(ackByteOutgoing);//this is a blocking call, timeout in 5 seconds
				}
				catch(Exception ex) {
					_ecwTCPModeIsReceiving=false;
					socketIncomingHandler.Close();
					throw new Exception("Timeout or other error waiting to send an acknowledgment.\r\nException: "+ex.Message+"\r\n"+ex.StackTrace);
				}
				//eCW uses the same worker socket to send the next message. Without this call to BeginReceive, they would attempt to send again
				//and the send would fail since we were no longer listening in this thread. eCW would timeout after 30 seconds of waiting for their
				//acknowledgement, then they would close their end and create a new socket for the next message. With this call, we can accept message
				//after message without waiting for a new connection.
				//the buffer was cleared after appending the chars to the string builder
				//the string builder was cleared after setting the message text in the table
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Message was received and acknowledgment was sent.  BeginReceive called again.",EventLogEntryType.Information);
				}
				_ecwTCPModeIsReceiving=true;
				IAsyncResult inSocketResult=socketIncomingHandler.BeginReceive(state.buffer,0,StateObject.BufferSize,SocketFlags.None,new AsyncCallback(OnDataReceived),state);
				if(!inSocketResult.AsyncWaitHandle.WaitOne(new TimeSpan(0,5,0))) {//WaitOne will return true if data was received, false if the timeout is reached with no data received
					//if 5 minutes goes by with no received data, close the socket and set the receiveDone manual reset event.  A new socket connection will be accepted.
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","Setting manual reset event due to inactive timeout.  The main receive thread will accept a new incoming connection.",EventLogEntryType.Information);
					}
					inSocketResult.AsyncWaitHandle.Close();
					_ecwTCPModeIsReceiving=false;
					socketIncomingHandler.Close();
					receiveDone.Set();
				}
				else {
					inSocketResult.AsyncWaitHandle.Close();
				}
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","Error in OnDataReceived.  Setting receiveDone manual reset event for main thread to accept new incoming connections.\r\n"+ex.Message+"\r\n"+ex.StackTrace,EventLogEntryType.Warning);
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","Setting manual reset event so the main receive thread will accept a new incoming connection.",EventLogEntryType.Information);
				}
				_ecwTCPModeIsReceiving=false;
				((StateObject)asyncResult.AsyncState).workSocket.Close();
				receiveDone.Set();//this will trigger the main thread to accept a new incoming connection and try to receive data again
			}
		}

		private void TimerCallbackSendConnectTCP(Object stateInfo) {
			if(_ecwTCPSendSocketIsConnected) {
				return;
			}
			_ecwTCPSendSocketIsConnected=true;
			Socket socketMain=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			IPEndPoint endpoint=null;
			try {
				string[] strIpPort=HL7DefEnabled.OutgoingIpPort.Split(':');//this was already validated in the HL7DefEdit window.
				IPAddress ipaddress=IPAddress.Parse(strIpPort[0]);//already validated
				int port=int.Parse(strIpPort[1]);//already validated
				endpoint=new IPEndPoint(ipaddress,port);
			}
			catch(Exception ex) {//not likely, but want to make sure to reset the bool that the send socket is connected
				EventLog.WriteEntry("OpenDentHL7","The HL7 send TCP/IP socket connection failed during IPEndPoint creation.\r\nException message: "+ex.Message,EventLogEntryType.Warning);
				socketMain.Close();
				_ecwTCPSendSocketIsConnected=false;
				return;//will try to create a socket again in 20 seconds
			}
			try {
				connectDone.Reset();
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","The send socket is beginning to connect.",EventLogEntryType.Information);
				}
				socketMain.BeginConnect(endpoint,new AsyncCallback(ConnectCallback),socketMain);
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","The main send thread is waiting for the manual reset event to be set in the connect callback function before sending messages.",EventLogEntryType.Information);
				}
				connectDone.WaitOne();//connection attempt will timeout and set the manual reset event
				if(IsVerboseLogging) {
					EventLog.WriteEntry("OpenDentHL7","The main send thread is now able to check the socket and if connected start sending.",EventLogEntryType.Information);
				}
				if(!socketMain.Connected) {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("OpenDentHL7","The send socket was not connected.  Disposing of the socket object and signaling for a new socket to be connected.",EventLogEntryType.Information);
					}
					socketMain.Close();
					_ecwTCPSendSocketIsConnected=false;
					return;//will try to connect again in 20 seconds
				}
				if(!IsSendTCPConnected) {//Previous run failed to connect. This time connected so make log entry that connection was successful
					EventLog.WriteEntry("OpenDentHL7","The HL7 send TCP/IP socket connection failed to connect previously and was successful this time.",EventLogEntryType.Information);
					IsSendTCPConnected=true;
				}
			}
			catch(SocketException ex) {
				if(IsSendTCPConnected) {//Previous run connected fine, make log entry and set bool to false
					EventLog.WriteEntry("OpenDentHL7","The HL7 send TCP/IP socket connection failed to connect.\r\nSocket Exception: "+ex.Message,EventLogEntryType.Warning);
					IsSendTCPConnected=false;
				}
				socketMain.Close();
				_ecwTCPSendSocketIsConnected=false;
				return;//will try to connect again in 20 seconds
			}
			catch(Exception ex) {
				if(IsSendTCPConnected) {//Previous run connected fine, make log entry and set bool to false
					EventLog.WriteEntry("OpenDentHL7","The HL7 send TCP/IP socket connection failed to connect.\r\nException: "+ex.Message,EventLogEntryType.Warning);
					IsSendTCPConnected=false;
				}
				socketMain.Close();
				_ecwTCPSendSocketIsConnected=false;
				return;//will try to connect again in 20 seconds
			}
			//start a timer to poll the database and to send messages as needed.  Every 6 seconds.  We increased the time between polling the database from 3 seconds to 6 seconds because we are now waiting 5 seconds for a message acknowledgment from eCW.
			TimerCallback timercallbackSendTCP=new TimerCallback(TimerCallbackSendTCP);
			timerSendTCP=new System.Threading.Timer(timercallbackSendTCP,socketMain,1800,6000);
		}

		private void ConnectCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket)ar.AsyncState;
				// Complete the connection.
				client.EndConnect(ar);
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","The TCP send socket encountered an issue attempting to connect.\r\nException: "+ex.Message,EventLogEntryType.Warning);
			}
			connectDone.Set();
		}

		private void TimerCallbackSendTCP(Object socketObject) {
			if(_ecwTCPModeIsSending) {
				return;
			}
			_ecwTCPModeIsSending=true;
			Socket socketWorker=(Socket)socketObject;
			if(socketWorker==null//null socket object
				|| !socketWorker.Connected)//or socket object is no longer connected (based on last activity, like last send/receive)
				//|| !socketWorker.Poll(500000,SelectMode.SelectWrite))//blocking poll (time in microseconds, so half a second) to attempt to write to socket.
			{
				//If socket is null, or not connected, socket was likely not shutdown gracefully so we will dispose of the send timer and worker socket
				//the connect timer will initiate a new connection with a new socket every 20 seconds if _ecwTCPSendSocketIsConnected=false;
				timerSendTCP.Dispose();
				socketWorker.Close();
				EventLog.WriteEntry("OpenDentHL7","The TCP send socket has been closed.  A new socket connection attempt will occur within 20 seconds.",EventLogEntryType.Warning);
				_ecwTCPModeIsSending=false;
				_ecwTCPSendSocketIsConnected=false;
				return;
			}
			try {
				Send(socketWorker);
			}
			catch(Exception ex) {
				EventLog.WriteEntry("OpenDentHL7","The TCP HL7 outgoing message socket encountered a problem during a send.  Another attempt to send will occur within 6 seconds.\r\nException message: "+ex.Message,EventLogEntryType.Warning);
			}
			_ecwTCPModeIsSending=false;
		}

		private void Send(Socket socketWorker) {
			List<HL7Msg> list=HL7Msgs.GetOnePending();
			while(list.Count>0) {
				string sendMsgControlId=HL7Msgs.GetControlId(list[0]);//could be empty string
				string data=MLLP_START_CHAR+list[0].MsgText+MLLP_END_CHAR+MLLP_ENDMSG_CHAR;
				byte[] byteData=Encoding.ASCII.GetBytes(data);
				socketWorker.SendTimeout=5000;//timeout in 5 seconds, send will retry after 6 second wait
				try {
					socketWorker.Send(byteData,byteData.Length,SocketFlags.None);//this is a blocking call, will timeout in 5 seconds
				}
				catch(Exception ex) {
					throw new Exception("The outbound socket encountered a problem sending a message.\r\nException: "+ex.Message);
				}
				#region RecieveAndProcessAck
				//For MLLP V2, do a blocking Receive here, along with a timeout.
				byte[] ackBuffer=new byte[256];//plenty big enough to receive the entire ack/nack response
				socketWorker.ReceiveTimeout=5000;//5 second timeout. Database is polled every 6 seconds for a new message to send, but if already sending and waiting for an ack, the new thread will just return
				int byteCountReceived=0;
				try {
					byteCountReceived=socketWorker.Receive(ackBuffer);//blocking Receive
				}
				catch(Exception ex) {
					throw new Exception("Timeout or other error waiting to receive an acknowledgment.\r\nException: "+ex.Message);
				}
				char[] chars=new char[byteCountReceived];
				Encoding.UTF8.GetDecoder().GetChars(ackBuffer,0,byteCountReceived,chars,0);
				StringBuilder strbAckMsg=new StringBuilder();
				strbAckMsg.Append(chars);
				if(strbAckMsg.Length>0 && strbAckMsg[0]!=MLLP_START_CHAR) {
					list[0].Note=list[0].Note+"Malformed acknowledgment (length "+strbAckMsg.Length+"):\r\n"+strbAckMsg.ToString()+"\r\n================\r\n";
					HL7Msgs.Update(list[0]);
					throw new Exception("Malformed acknowledgment.");
				}
				else if(strbAckMsg.Length>=3
					&& strbAckMsg[strbAckMsg.Length-1]==MLLP_ENDMSG_CHAR//last char is the endmsg char.
					&& strbAckMsg[strbAckMsg.Length-2]==MLLP_END_CHAR)//the second-to-the-last char is the end char.
				{
					//we have a complete message
					strbAckMsg.Remove(0,1);//strip off the start char
					strbAckMsg.Remove(strbAckMsg.Length-2,2);//strip off the end chars
				}
				else if(strbAckMsg.Length>=2//so that the next line won't crash
					&& strbAckMsg[strbAckMsg.Length-1]==MLLP_END_CHAR)//the last char is the end char.
				{
					//we will treat this as a complete message, because the endmsg char is optional.
					strbAckMsg.Remove(0,1);//strip off the start char
					strbAckMsg.Remove(strbAckMsg.Length-1,1);//strip off the end char
				}
				else {
					list[0].Note=list[0].Note+"Malformed acknowledgment (length "+strbAckMsg.Length+"):\r\n"+strbAckMsg.ToString()+"\r\n================\r\n";
					HL7Msgs.Update(list[0]);
					throw new Exception("Malformed acknowledgment.");
				}
				MessageHL7 ackMsg=new MessageHL7(strbAckMsg.ToString());
				try {
					MessageParser.ProcessAck(ackMsg,IsVerboseLogging);
				}
				catch(Exception ex) {
					list[0].Note=list[0].Note+ackMsg.ToString()+"\r\nError processing acknowledgment.\r\n";
					HL7Msgs.Update(list[0]);
					throw new Exception("Error processing acknowledgment.\r\n"+ex.Message);
				}
				if(ackMsg.AckCode=="" || (sendMsgControlId!="" && ackMsg.ControlId=="")) {
					list[0].Note=list[0].Note+ackMsg.ToString()+"\r\nInvalid ACK message.  Attempt to resend.\r\n";
					HL7Msgs.Update(list[0]);
					throw new Exception("Invalid ACK message received.");
				}
				if(ackMsg.ControlId==sendMsgControlId && ackMsg.AckCode=="AA") {//acknowledged received (Application acknowledgment: Accept)
					list[0].Note=list[0].Note+ackMsg.ToString()+"\r\nMessage ACK (acknowledgment) received.\r\n";
				}
				else if(ackMsg.ControlId==sendMsgControlId && ackMsg.AckCode!="AA") {//ACK received for this message, but ack code was not acknowledgment accepted
					if(list[0].Note.Contains("NACK4")) {//this is the 5th negative acknowledgment, don't try again
						list[0].Note=list[0].Note+"Ack code: "+ackMsg.AckCode+"\r\nThis is NACK5, the message status has been changed to OutFailed. We will not attempt to send again.\r\n";
						list[0].HL7Status=HL7MessageStatus.OutFailed;
					}
					else if(list[0].Note.Contains("NACK")) {//failed sending at least once already
						list[0].Note=list[0].Note+"Ack code: "+ackMsg.AckCode+"\r\nNACK"+list[0].Note.Split(new string[] { "NACK" },StringSplitOptions.None).Length+"\r\n";
					}
					else {
						list[0].Note=list[0].Note+"Ack code: "+ackMsg.AckCode+"\r\nMessage NACK (negative acknowlegment) received. We will try to send again.\r\n";
					}
					HL7Msgs.Update(list[0]);//Add NACK note to hl7msg table entry
					return;
				}
				else {//ack received for control ID that does not match the control ID of message just sent
					list[0].Note=list[0].Note+"Sent message control ID: "+sendMsgControlId+"\r\nAck message control ID: "+ackMsg.ControlId
					+"\r\nAck received for message other than message just sent.  We will try to send again.\r\n";
					HL7Msgs.Update(list[0]);
					return;
				}
				#endregion
				list[0].HL7Status=HL7MessageStatus.OutSent;
				HL7Msgs.Update(list[0]);//set the status to sent and save ack message in Note field.
				if(_ecwDateTimeOldMsgsDeleted.Date<DateTime.Now.Date) {
					if(IsVerboseLogging) {
						EventLog.WriteEntry("DeleteOldMsgText Starting");
					}
					_ecwDateTimeOldMsgsDeleted=DateTime.Now;//If DeleteOldMsgText fails for any reason.  This will cause it to not get called until the next day instead of with every msg.
					HL7Msgs.DeleteOldMsgText();//this function deletes if DateTStamp is less than CURDATE-INTERVAL 4 MONTH.  That means it will delete message text only once a day, not time based.
					if(IsVerboseLogging) {
						EventLog.WriteEntry("DeleteOldMsgText Finished");
					}
				}
				list=HL7Msgs.GetOnePending();//returns 0 or 1 pending message
			}
		}
		#endregion TCP/IP Mode
	}

	///<summary>State object for reading client data asynchronously</summary>
	public class StateObject {
		//Client socket
		public Socket workSocket=null;
		//Size of receive buffer
		public const int BufferSize=256;
		//Receive buffer
		public byte[] buffer=new byte[BufferSize];
		//Received data string
		public StringBuilder strbFullMsg=new StringBuilder();
	}
}
