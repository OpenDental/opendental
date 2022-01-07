using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBackup : FormODBase {
		//private bool usesInternalImages;
		private double _amtCopied = 0; // used for reporting amount copied for backup and restore of ODI in recursive calls
		///<summary></summary>
		public FormBackup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBackup_Load(object sender, System.EventArgs e) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {//Archive not supported over MT connection. Show warning if Middle Tier is in use.
				butArchive.Enabled=false;
				labelWarning.Visible=true;
			}
			#region Backup Tab
			//usesInternalImages=(PrefC.GetString(PrefName.ImageStore)=="OpenDental.Imaging.SqlStore");
			checkExcludeImages.Checked=PrefC.GetBool(PrefName.BackupExcludeImageFolder);
			checkArchiveDoBackupFirst.Checked=PrefC.GetBool(PrefName.ArchiveDoBackupFirst);
			textBackupFromPath.Text=PrefC.GetString(PrefName.BackupFromPath);
			textBackupToPath.Text=PrefC.GetString(PrefName.BackupToPath);
			textBackupRestoreFromPath.Text=PrefC.GetString(PrefName.BackupRestoreFromPath);
			textBackupRestoreToPath.Text=PrefC.GetString(PrefName.BackupRestoreToPath);
			textBackupRestoreAtoZToPath.Text=PrefC.GetString(PrefName.BackupRestoreAtoZToPath);
			textBackupRestoreAtoZToPath.Enabled=ShouldUseAtoZFolder();
			butBrowseRestoreAtoZTo.Enabled=ShouldUseAtoZFolder();
			if(ProgramProperties.IsAdvertisingDisabled(ProgramName.CentralDataStorage)) {
				groupManagedBackups.Visible=false;
			}
			#endregion
			#region Archive Tab
			string decryptedPassword;
			CDT.Class1.Decrypt(PrefC.GetString(PrefName.ArchivePassHash),out decryptedPassword);
			textArchivePass.Text=decryptedPassword;
			textArchivePass.PasswordChar=(textArchivePass.Text=="" ? default(char) : '*');
			textArchiveServerName.Text=PrefC.GetString(PrefName.ArchiveServerName);
			textArchiveUser.Text=PrefC.GetString(PrefName.ArchiveUserName);
			//If pref is set, use it.  Otherwise, 3 years ago.
			if(PrefC.GetDate(PrefName.ArchiveDate)==DateTime.MinValue) {
				dateTimeArchive.Value=DateTime.Today.AddYears(-3);
			}
			else {
				dateTimeArchive.Value=PrefC.GetDate(PrefName.ArchiveDate);
			}
			ToggleBackupSettings();
		#endregion
			#region Supplemental Tab
			checkSupplementalBackupEnabled.Checked=PrefC.GetBool(PrefName.SupplementalBackupEnabled);
			if(PrefC.GetDate(PrefName.SupplementalBackupDateLastComplete).Year > 1880) {
				textSupplementalBackupDateLastComplete.Text=PrefC.GetDate(PrefName.SupplementalBackupDateLastComplete).ToString();
			}
			textSupplementalBackupCopyNetworkPath.Text=PrefC.GetString(PrefName.SupplementalBackupNetworkPath);
			#endregion Supplemental Tab
			if(ODBuild.IsWeb()) {
				//OD Cloud users cannot use this tool because they're InnoDb.
				tabControl1.TabPages.Remove(tabPageBackup);
				//We don't want to allow the user to connect to another server.
				checkArchiveDoBackupFirst.Visible=false;
				checkArchiveDoBackupFirst.Checked=false;
				groupBoxBackupConnection.Visible=false;
				//We don't want the user to be able to tell if a directory exists.
				tabControl1.TabPages.Remove(tabPageSupplementalBackups);
			}
		}

		#region Backup Tab

		private bool IsBackupTabValid() {
			//test for trailing slashes
			if(textBackupFromPath.Text!="" && !textBackupFromPath.Text.EndsWith(""+Path.DirectorySeparatorChar)){
				MsgBox.Show(this,"Paths must end with "+Path.DirectorySeparatorChar+".");
				return false;
			}
			if(textBackupToPath.Text!="" && !textBackupToPath.Text.EndsWith(""+Path.DirectorySeparatorChar)){
				MsgBox.Show(this,"Paths must end with "+Path.DirectorySeparatorChar+".");
				return false;
			}
			if(textBackupRestoreFromPath.Text!="" && !textBackupRestoreFromPath.Text.EndsWith(""+Path.DirectorySeparatorChar)) {
				MsgBox.Show(this,"Paths must end with "+Path.DirectorySeparatorChar+".");
				return false;
			}
			if(textBackupRestoreToPath.Text!="" && !textBackupRestoreToPath.Text.EndsWith(""+Path.DirectorySeparatorChar)) {
				MsgBox.Show(this,"Paths must end with "+Path.DirectorySeparatorChar+".");
				return false;
			}
			if(textBackupRestoreAtoZToPath.Text!="" && !textBackupRestoreAtoZToPath.Text.EndsWith(""+Path.DirectorySeparatorChar)) {
				MsgBox.Show(this,"Paths must end with "+Path.DirectorySeparatorChar+".");
				return false;
			}
			return true;
		}

		private bool SaveTabPrefs() {
			bool hasChanged=false;
			hasChanged |= Prefs.UpdateBool(PrefName.BackupExcludeImageFolder,checkExcludeImages.Checked);
			hasChanged |= Prefs.UpdateBool(PrefName.ArchiveDoBackupFirst,checkArchiveDoBackupFirst.Checked);
			hasChanged |= Prefs.UpdateString(PrefName.BackupFromPath,textBackupFromPath.Text);
			hasChanged |= Prefs.UpdateString(PrefName.BackupToPath,textBackupToPath.Text);
			hasChanged |= Prefs.UpdateString(PrefName.BackupRestoreFromPath,textBackupRestoreFromPath.Text);
			hasChanged |= Prefs.UpdateString(PrefName.BackupRestoreToPath,textBackupRestoreToPath.Text);
			hasChanged |= Prefs.UpdateString(PrefName.BackupRestoreAtoZToPath,textBackupRestoreAtoZToPath.Text);
			hasChanged |= Prefs.UpdateString(PrefName.ArchiveServerName,textArchiveServerName.Text);
			hasChanged |= Prefs.UpdateString(PrefName.ArchiveUserName,textArchiveUser.Text);
			string encryptedPassword;
      CDT.Class1.Encrypt(textArchivePass.Text,out encryptedPassword);
			hasChanged |= Prefs.UpdateString(PrefName.ArchivePassHash,encryptedPassword);
			return hasChanged;
		}

		private bool ShouldUseAtoZFolder() {
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				return false;
			}
			if(checkExcludeImages.Checked) {
				return false;
			}
			return true;
		}

		private void butBrowseFrom_Click(object sender, System.EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textBackupFromPath.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textBackupFromPath.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBrowseTo_Click(object sender, System.EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textBackupToPath.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textBackupToPath.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBrowseRestoreFrom_Click(object sender, System.EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textBackupRestoreFromPath.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textBackupRestoreFromPath.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBrowseRestoreTo_Click(object sender, System.EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textBackupRestoreToPath.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textBackupRestoreToPath.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBrowseRestoreAtoZTo_Click(object sender, System.EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textBackupRestoreAtoZToPath.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textBackupRestoreAtoZToPath.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBackup_Click(object sender, System.EventArgs e) {
			if(!IsBackupTabValid()) {
				return;
			}
			//Ensure that the backup from and backup to paths are different. This is to prevent the live database
			//from becoming corrupt.
			if(this.textBackupFromPath.Text.Trim().ToLower()==this.textBackupToPath.Text.Trim().ToLower()) {
				MsgBox.Show(this,"The backup from path and backup to path must be different.");
				return;
			}
			//test saving defaults
			if(textBackupFromPath.Text!=PrefC.GetString(PrefName.BackupFromPath)
				|| textBackupToPath.Text!=PrefC.GetString(PrefName.BackupToPath)
				|| textBackupRestoreFromPath.Text!=PrefC.GetString(PrefName.BackupRestoreFromPath)
				|| textBackupRestoreToPath.Text!=PrefC.GetString(PrefName.BackupRestoreToPath)
				|| textBackupRestoreAtoZToPath.Text!=PrefC.GetString(PrefName.BackupRestoreAtoZToPath)) 
			{
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Set as default?") && SaveTabPrefs()) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			string dbName=MiscData.GetCurrentDatabase();
			if(InnoDb.HasInnoDbTables(dbName)) {
				//Database has innodb tables. Backup tool does not work on dbs with InnoDb tables. 
				MsgBox.Show(this,"InnoDb tables detected. Backup tool cannot run with InnoDb tables.");
				return;
			}
			if(!Directory.Exists(ODFileUtils.CombinePaths(textBackupFromPath.Text,dbName))){// C:\mysql\data\opendental
				MsgBox.Show(this,"Backup FROM path is invalid.");
				return;
			}
			if(!Directory.Exists(textBackupToPath.Text)){// D:\
				MsgBox.Show(this,"Backup TO path is invalid.");
				return;
			}
			double dbSize=GetFileSizes(textBackupFromPath.Text+dbName)/1024;
			if(!hasDriveSpace(textBackupToPath.Text,dbSize)) { // ensure drive space before attempting backup
				MsgBox.Show(this,Lan.g(this,"Not enough free disk space available on the destination drive to backup the database."));
				return;
			}
			//there is enough drive space, show progress bar and make backup
			ProgressOD progressOD=new ProgressOD();
			progressOD.ProgStyle=ProgressBarStyle.Blocks;
			progressOD.ActionMain=() => InstanceMethodDatabaseBackup(dbName,textBackupFromPath.Text,textBackupToPath.Text,dbSize);
			progressOD.StartingMessage=Lan.g(this,"Preparing backup");
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				//catch error from InstanceMethodDatabaseBackup thrown from inside the ProgressOD thread
				MsgBox.Show(ex.Message);
				return;
			}
			if(progressOD.IsCancelled) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.Backup,0,Lan.g(this,"Database backup created at ")+textBackupToPath.Text);
			MessageBox.Show(Lan.g(this,"Database backup complete."));
			//AtoZ folder if selected for backup.=================================================================================================================
			if(!ShouldUseAtoZFolder()) {
				Close();
				return;
			}
			string aToZFullPath=ODFileUtils.RemoveTrailingSeparators(ImageStore.GetPreferredAtoZpath());
			string aToZDirectory=aToZFullPath.Substring(aToZFullPath.LastIndexOf(Path.DirectorySeparatorChar)+1);
			double aToZSize=GetFileSizes(ODFileUtils.CombinePaths(aToZFullPath,""),
				ODFileUtils.CombinePaths(new string[] { textBackupToPath.Text,aToZDirectory,"" }))/1024;
			if(!hasDriveSpace(textBackupToPath.Text,aToZSize)) {
				MsgBox.Show(this,Lan.g(this,"Not enough free disk space available on the destination drive to backup the A to Z folder."));
				return;
			}
			//there is enough space, show progress bar and make backup
			progressOD=new ProgressOD();
			progressOD.ProgStyle=ProgressBarStyle.Blocks;
			progressOD.ActionMain=() => InstanceMethodAtoZBackup(textBackupToPath.Text,aToZDirectory,aToZFullPath,aToZSize);
			progressOD.StartingMessage=Lan.g(this,"Backing up A to Z Folder");
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				//catch error from InstanceMethodAtoZBackup thrown from inside the ProgressOD thread
				MsgBox.Show(ex.Message);
				return;
			}
			if(progressOD.IsCancelled) {
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.Backup,0,Lan.g(this,"A to Z folder backup created at ")+textBackupToPath.Text);
			MessageBox.Show(Lan.g(this,"Backup complete."));
			Close();
		}

		/// <summary>
		/// used to check drive space before performing a backup. Happens on main thread before passing the backup method to ProgressOD.
		/// </summary>
		/// <returns>bool: True if there is enough disk space, or disk space cannot be determined. Otherwise false.</returns>
		private bool hasDriveSpace(string backupToPath,double fileSize) {
			ulong driveFreeSpace=0;
			//Attempt to get the free disk space on the drive or share of the destination folder.
			//If the free space cannot be determined the backup will be attempted anyway (old behavior).
			if(ODFileUtils.GetDiskFreeSpace(backupToPath,out driveFreeSpace)) {
				if((ulong)fileSize*1024*1024>=driveFreeSpace) {//dbSize is in megabytes, cast to ulong to compare. It will never be negative so this is safe.
					return false;
				}
			}
			return true;
		}

		private void InstanceMethodDatabaseBackup(string databaseName,string backupFromPath,string backupToPath,double databaseSize) {
			double currentValue=0;
			string dbBackupToPath=ODFileUtils.CombinePaths(backupToPath,databaseName);
			if(Directory.Exists(dbBackupToPath)) {// D:\opendental
				int loopCount=1;
				while(Directory.Exists(dbBackupToPath+"backup_"+loopCount)) {
					loopCount++;
				}
				try {
					Directory.Move(dbBackupToPath,dbBackupToPath+"backup_"+loopCount);
				}
				catch {
					throw new Exception(Lan.g(this,"Failed to move directory."));
				}
			}
			string fromPath=ODFileUtils.CombinePaths(backupFromPath,databaseName);
			string toPath=backupToPath;
			DirectoryInfo directoryInfo=new DirectoryInfo(fromPath);//does not check to see if dir exists
			try {
				Directory.CreateDirectory(ODFileUtils.CombinePaths(toPath,directoryInfo.Name));
			}
			catch {
				throw new Exception(Lan.g(this,"Failed to create directory for backup."));
			}
			FileInfo[] fileInfoArray=directoryInfo.GetFiles();
			for(int i = 0;i<fileInfoArray.Length;i++) {
				string fromFile=fileInfoArray[i].FullName;
				string toFile=ODFileUtils.CombinePaths(new string[] { toPath,directoryInfo.Name,fileInfoArray[i].Name });
				bool doOverwrite=false;
				if(File.Exists(toFile)) {
					if(fileInfoArray[i].LastWriteTime!=File.GetLastWriteTime(toFile)) {//if modification dates don't match
						FileAttributes fileAttributes=File.GetAttributes(toFile);
						bool isReadOnly=(fileAttributes&FileAttributes.ReadOnly)==FileAttributes.ReadOnly;
						if(isReadOnly) {
							//If the destination file exists and is marked as read only, then we must mark it as a
							//normal read/write file before it may be overwritten.
							File.SetAttributes(toFile,FileAttributes.Normal);//Remove read only from the destination file.
						}
						doOverwrite=true;
					}
				}
				try {
					File.Copy(fromFile,toFile,doOverwrite);
				}
				catch {
					throw new Exception(Lan.g(this,"Failed to copy ")+fromFile+Lan.g(this," to ")+toFile+".");
				}
				//update progress bar with current value copied.
				currentValue+=(double)fileInfoArray[i].Length/(double)1024/(double)1024;
				string progressBarMessage=Lan.g(this,"Copied ")+Math.Round(currentValue,2).ToString()+"MB "+Lan.g(this,"of ")+databaseSize.ToString()+"MB";
				ProgressBarHelper progressBarHelper=new ProgressBarHelper(progressBarMessage,blockValue:(int)(currentValue/databaseSize*100),blockMax:100);
				ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
			}
		}

		private void InstanceMethodAtoZBackup(string backupToPath, string aToZDirectory,string aToZFullPath, double aToZSize) {
			if(!Directory.Exists(ODFileUtils.CombinePaths(backupToPath,aToZDirectory))) {// ex. D:\OpenDentalData
				try {
					Directory.CreateDirectory(ODFileUtils.CombinePaths(textBackupToPath.Text,aToZDirectory));// ex. D:\OpenDentalData
				}
				catch {
					throw new Exception(Lan.g(this,"Failed to create backup directory for A to Z folder."));
				}
			}
			_amtCopied = 0;
			CopyDirectoryIncremental(ODFileUtils.CombinePaths(aToZFullPath,""),// C:\OpenDentalData\
				ODFileUtils.CombinePaths(new string[] { backupToPath,aToZDirectory,"" }),// D:\OpenDentalData\
				aToZSize);
		}

		///<summary>This is the function that the worker thread uses to restore the A-Z folder.</summary>
		private void InstanceMethodRestore(string aToZFullPath, string restoreFromPath){
			string aToZDirectory=aToZFullPath.Substring(aToZFullPath.LastIndexOf(Path.DirectorySeparatorChar)+1);// OpenDentalData
			long aToZSize=GetFileSizes(ODFileUtils.CombinePaths(new string[] {restoreFromPath,aToZDirectory,""}),
				ODFileUtils.CombinePaths(aToZFullPath,""))/1024;// C:\OpenDentalData\
			if(!Directory.Exists(aToZFullPath)){// C:\OpenDentalData\
				Directory.CreateDirectory(aToZFullPath);// C:\OpenDentalData\
			}
			_amtCopied = 0;
			CopyDirectoryIncremental(ODFileUtils.CombinePaths(new string[] {restoreFromPath,aToZDirectory,""}),
				ODFileUtils.CombinePaths(aToZFullPath,""),// C:\OpenDentalData\
				aToZSize);
		}

		///<summary>Counts the total KB of all files that will need to be copied from one directory to another recursively. Used to display the progress bar.  Supplied paths must end in \. toPath might not exist.</summary>
		private long GetFileSizes(string fromPath,string toPath){
			long retVal=0;
			DirectoryInfo directoryInfo=new DirectoryInfo(fromPath);
			DirectoryInfo[] directoryInfoArray=directoryInfo.GetDirectories();
			for(int i=0;i<directoryInfoArray.Length;i++){
				retVal+=GetFileSizes(ODFileUtils.CombinePaths(directoryInfoArray[i].FullName,""),
					ODFileUtils.CombinePaths(new string[] {toPath,directoryInfoArray[i].Name,""}));
			}
			FileInfo[] fileInfoArray=directoryInfo.GetFiles();//of fromPath
			for(int i=0;i<fileInfoArray.Length;i++){
					retVal+=(long)(fileInfoArray[i].Length/1024);
			}
			return retVal;
		}

		///<summary>Counts the total KB of all files in the given directory.  Not recursive since it's just used for db files.  Used to display the progress bar.</summary>
		private long GetFileSizes(string fromPath) {
			long returnValue=0;
			DirectoryInfo directoryInfo=new DirectoryInfo(fromPath);
			FileInfo[] fileInfoArray=directoryInfo.GetFiles();
			for(int i=0;i<fileInfoArray.Length;i++){
				returnValue+=(long)(fileInfoArray[i].Length/1024);
			}
			return returnValue;
		}

		///<summary>A recursive fuction that copies any new or changed files or folders from one directory to another.  An exception will be thrown if either directory does not already exist.  fromPath is the fully qualified path of the directory to copy.  toPath is the fully qualified path of the destination directory.  Both paths must include a trailing \.  The max size should be calculated ahead of time.  It's passed in for use in progress bar.</summary>
		private void CopyDirectoryIncremental(string fromPath,string toPath, double maxSize){
			if(!Directory.Exists(fromPath)){
				throw new Exception(fromPath+" does not exist.");
			}
			if(!Directory.Exists(toPath)){
				throw new Exception(toPath+" does not exist.");
			}
			DirectoryInfo directoryInfo=new DirectoryInfo(fromPath);
			DirectoryInfo[] directoryInfoArray=directoryInfo.GetDirectories();
			for(int i=0;i<directoryInfoArray.Length;i++){
				string destinationPath=ODFileUtils.CombinePaths(toPath,directoryInfoArray[i].Name);
				if(!Directory.Exists(destinationPath)){
					Directory.CreateDirectory(destinationPath);
				}
				CopyDirectoryIncremental(ODFileUtils.CombinePaths(directoryInfoArray[i].FullName,""),
					ODFileUtils.CombinePaths(destinationPath,""),maxSize);
			}
			FileInfo[] fileInfoArray=directoryInfo.GetFiles();//of fromPath
			for(int i=0;i<fileInfoArray.Length;i++){
				string fromFile=fileInfoArray[i].FullName;
				string toFile=ODFileUtils.CombinePaths(toPath,fileInfoArray[i].Name);
				if(File.Exists(toFile)){
					if(fileInfoArray[i].LastWriteTime!=File.GetLastWriteTime(toFile)){//if modification dates don't match
						FileAttributes fileAttributes=File.GetAttributes(toFile);
						bool isReadOnly=((fileAttributes&FileAttributes.ReadOnly)==FileAttributes.ReadOnly);
						if(isReadOnly){
							//If the destination file exists and is marked as read only, then we must mark it as a
							//normal read/write file before it may be overwritten.
							File.SetAttributes(toFile,FileAttributes.Normal);//Remove read only from the destination file.
						}
						File.Copy(fromFile,toFile,true);
					}
				}
				else{//file doesn't exist, so just copy
					File.Copy(fromFile,toFile);
				}
				_amtCopied+=(double)fileInfoArray[i].Length/1048576.0; //Number of megabytes.
				string progressBarMessage=Lan.g(this,"Copied ")+Math.Round(_amtCopied,2).ToString()+"MB "+Lan.g(this,"of ")+maxSize.ToString()+"MB";
				ProgressBarHelper progressBarHelper=new ProgressBarHelper(progressBarMessage,blockValue:(int)(_amtCopied/maxSize*100),blockMax:100);
				ProgressBarEvent.Fire(ODEventType.ProgressBar,progressBarHelper);
			}
		}

		private void butRestore_Click(object sender, System.EventArgs e) {			
			if(textBackupRestoreFromPath.Text!="" && !textBackupRestoreFromPath.Text.EndsWith(""+Path.DirectorySeparatorChar)){
				MessageBox.Show(Lan.g(this,"Paths must end with ")+Path.DirectorySeparatorChar+".");
				return;
			}
			if(textBackupRestoreToPath.Text!="" && !textBackupRestoreToPath.Text.EndsWith(""+Path.DirectorySeparatorChar)){
				MessageBox.Show(Lan.g(this,"Paths must end with ")+Path.DirectorySeparatorChar+".");
				return;
			}
			if(ShouldUseAtoZFolder()) {
				if(textBackupRestoreAtoZToPath.Text!="" && !textBackupRestoreAtoZToPath.Text.EndsWith(""+Path.DirectorySeparatorChar)){
					MessageBox.Show(Lan.g(this,"Paths must end with ")+Path.DirectorySeparatorChar+".");
					return;
				}
			}
			if(Environment.OSVersion.Platform!=PlatformID.Unix){
				//dmg This check will not work on Linux, because mapped drives exist as regular (mounted) paths. Perhaps there
				//is another way to check for this on Linux.
				if(textBackupRestoreToPath.Text!="" && textBackupRestoreToPath.Text.StartsWith(""+Path.DirectorySeparatorChar)){
					MsgBox.Show(this,"The restore database TO folder must be on this computer.");
					return;
				}
			}
			//pointless to save defaults
			string dbName=MiscData.GetCurrentDatabase();
			if(InnoDb.HasInnoDbTables(dbName)) {
				//Database has innodb tables. Restore tool does not work on dbs with InnoDb tables. 
				MsgBox.Show(this,"InnoDb tables detected. Restore tool cannot run with InnoDb tables.");
				return;
			}
			if(!Directory.Exists(ODFileUtils.CombinePaths(textBackupRestoreFromPath.Text,dbName))){// D:\opendental
				MessageBox.Show(Lan.g(this,"Restore FROM path is invalid.  Unable to find folder named ")+dbName);
				return;
			}
			if(!Directory.Exists(ODFileUtils.CombinePaths(textBackupRestoreToPath.Text,dbName))) {// C:\mysql\data\opendental
				MessageBox.Show(Lan.g(this,"Restore TO path is invalid.  Unable to find folder named ")+dbName);
				return;
			}
			if(ShouldUseAtoZFolder()) {
				if(!Directory.Exists(textBackupRestoreAtoZToPath.Text)) {// C:\OpenDentalData\
					MsgBox.Show(this,"Restore A-Z images TO path is invalid.");
					return;
				}
				string aToZFullPath=textBackupRestoreAtoZToPath.Text;// C:\OpenDentalData\
				//remove the trailing \
				aToZFullPath=aToZFullPath.Substring(0,aToZFullPath.Length-1);// C:\OpenDentalData
				string aToZDirectory=aToZFullPath.Substring(aToZFullPath.LastIndexOf(Path.DirectorySeparatorChar)+1);// OpenDentalData
				if(!Directory.Exists(ODFileUtils.CombinePaths(textBackupRestoreFromPath.Text,aToZDirectory))){// D:\OpenDentalData
					MsgBox.Show(this,"Restore A-Z images FROM path is invalid.");
					return;
				}
			}
			string fromPath=ODFileUtils.CombinePaths(new string[] {textBackupRestoreFromPath.Text,dbName,""});// D:\opendental\
			DirectoryInfo directoryInfo=new DirectoryInfo(fromPath);//does not check to see if dir exists
			if(MessageBox.Show(Lan.g(this,"Restore from backup created on")+"\r\n"
				+directoryInfo.LastWriteTime.ToString("dddd")+"  "+directoryInfo.LastWriteTime.ToString()
				,"",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)==DialogResult.Cancel) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			//stop the service--------------------------------------------------------------------------------------
			ServiceController serviceController=new ServiceController("MySQL");
			if(!ServicesHelper.Stop(serviceController)) {
				MsgBox.Show(this,"Unable to stop MySQL service.");
				Cursor=Cursors.Default;
				return;
			}
			//rename the current database---------------------------------------------------------------------------
			//Get a name for the new directory
			string newDb=dbName+"backup_"+DateTime.Today.ToString("MM_dd_yyyy");
			if(Directory.Exists(ODFileUtils.CombinePaths(textBackupRestoreToPath.Text,newDb))){//if the new database name already exists
				//find a unique one
				int uniqueID=1;
				string originalNewDb=newDb;
				do{
					newDb=originalNewDb+"_"+uniqueID.ToString();
					uniqueID++;
				}
				while(Directory.Exists(ODFileUtils.CombinePaths(textBackupRestoreToPath.Text,newDb)));
			}
			//move the current db (rename)
			Directory.Move(ODFileUtils.CombinePaths(textBackupRestoreToPath.Text,dbName)
				,ODFileUtils.CombinePaths(textBackupRestoreToPath.Text,newDb));
			//Restore----------------------------------------------------------------------------------------------
			string toPath=textBackupRestoreToPath.Text;// C:\mysql\data\
			Directory.CreateDirectory(ODFileUtils.CombinePaths(toPath,directoryInfo.Name));
			FileInfo[] fileInfoArray=directoryInfo.GetFiles();
			for(int i=0;i<fileInfoArray.Length;i++){
				File.Copy(fileInfoArray[i].FullName,ODFileUtils.CombinePaths(new string[] {toPath,directoryInfo.Name,fileInfoArray[i].Name}));
			}
			//start the service--------------------------------------------------------------------------------------
			ServicesHelper.Start(serviceController);
			Cursor=Cursors.Default;
			//restore A-Z folder, and give user a chance to cancel it.
			if(ShouldUseAtoZFolder()) {
				string aToZFullPath=ODFileUtils.RemoveTrailingSeparators(ImageStore.GetPreferredAtoZpath());
				ProgressOD progressOD=new ProgressOD();
				progressOD.ProgStyle=ProgressBarStyle.Blocks;
				progressOD.ActionMain=() => InstanceMethodRestore(aToZFullPath,textBackupRestoreFromPath.Text);
				progressOD.StartingMessage=Lan.g(this,"Database restored.\r\nRestoring A to Z folder.");
				try {
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex) {
					//catch error from InstanceMethodRestore thrown from inside the ProgressOD thread
					MsgBox.Show(ex.Message);
					return;
				}
				if(progressOD.IsCancelled) {
					return;
				}
				MessageBox.Show(Lan.g(this,"Restore complete."));
			}
			Version versionProgramVersionDb=new Version(PrefC.GetStringNoCache(PrefName.ProgramVersion));
			Version versionProductVersion=new Version(Application.ProductVersion);
			if(versionProgramVersionDb!=versionProductVersion) {
				MsgBox.Show(this,"The restored database version is different than the version installed and requires a restart.  The program will now close.");
				FormOpenDental.S_ProcessKillCommand();
				return;
			}
			else {
				DataValid.SetInvalid(Cache.GetAllCachedInvalidTypes().ToArray());
			}
			MsgBox.Show(this,"Done");
			Close();
			return;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!IsBackupTabValid()) {
				return;
			}
			if(SaveTabPrefs()) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void checkExcludeImages_Click(object sender,EventArgs e) {
			textBackupRestoreAtoZToPath.Enabled=ShouldUseAtoZFolder();
			butBrowseRestoreAtoZTo.Enabled=ShouldUseAtoZFolder();
		}

		private void pictureCDS_Click(object sender,EventArgs e) {
			if(!Programs.IsEnabledByHq(ProgramName.CentralDataStorage,out string err)) {
				MsgBox.Show(err);
				return;
			}
			CDS.ShowPage();
		}

		#endregion

		#region Archive Tab

		private void ToggleBackupSettings() {
			UIHelper.GetAllControls(groupBoxBackupConnection).ForEach(x => x.Enabled=checkArchiveDoBackupFirst.Checked);
		}

		private void checkMakeBackup_CheckedChanged(object sender,EventArgs e) {
			ToggleBackupSettings();
		}
		
		private void butArchive_Click(object sender,EventArgs e) {
			#region Validation
			if(checkArchiveDoBackupFirst.Checked) { //We only need to validate the backup settings if the user wants to make a backup first
				if(!MsgBox.Show(MsgBoxButtons.YesNo,"To make a backup of the database, ensure no other machines are currently using OpenDental. Proceed?")) {
					return;
				}
				//Validation
				if(string.IsNullOrWhiteSpace(textArchiveServerName.Text)) {
					MsgBox.Show(this,"Please specify a Server Name.");
					return;
				}
				if(string.IsNullOrWhiteSpace(textArchiveUser.Text)) {
					MsgBox.Show(this,"Please enter a User.");
					return;
				}
				if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ArchiveKey))) {//If archive key isn't set, generate a new one.
					string archiveKey=MiscUtils.CreateRandomAlphaNumericString(10);
					Prefs.UpdateString(PrefName.ArchiveKey,archiveKey);
				}
			}
			#endregion
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => {
				//Make a backup if needed
				if(checkArchiveDoBackupFirst.Checked) {
					MiscData.MakeABackup(serverName:textArchiveServerName.Text,
						user:textArchiveUser.Text,
						pass:textArchivePass.Text,
						doVerify:true);
				}
				//Delete the unnecessary data
				SecurityLogs.DeleteBeforeDateInclusive(dateTimeArchive.Value);//this fires events
				SecurityLogs.MakeLogEntry(Permissions.Backup,0,$"SecurityLog and SecurityLogHashes on/before {dateTimeArchive.Value} deleted.");
				EmailMessages.DeleteBeforeDate(dateTimeArchive.Value);
			};
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				FriendlyException.Show("An error occurred backing up the old database. Old data was not removed from the database. "+
					"Ensure no other machines are currently using OpenDental and try again.",ex);
			}
		}

		private void butSaveArchive_Click(object sender,EventArgs e) {
			if(SaveTabPrefs()) {
				DataValid.SetInvalid(InvalidType.Prefs);
				MsgBox.Show(this,"Saved");
			}
			PrefName.ArchiveDate.Update(dateTimeArchive.Value);
		}

		#endregion

		#region Supplemental Tab

		private void TabControl1Tab_Selected(object sender,TabControlEventArgs e) {
			if(e.TabPage==tabPageSupplementalBackups) {
				if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
					tabControl1.SelectedTab=tabPageBackup;
					return;
				}
			}
		}

		private void ButSupplementalBrowse_Click(object sender,EventArgs e) {
			if(folderBrowserSupplementalCopyNetworkPath.ShowDialog()==DialogResult.OK) {
				textSupplementalBackupCopyNetworkPath.Text=folderBrowserSupplementalCopyNetworkPath.SelectedPath;
			}
		}

		private void ButSupplementalSaveDefaults_Click(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(textSupplementalBackupCopyNetworkPath.Text) && !Directory.Exists(textSupplementalBackupCopyNetworkPath.Text)) {
				MsgBox.Show(this,"Invalid or inaccessible "+labelSupplementalBackupCopyNetworkPath.Text+".");//This label text will rarely change.
				return;
			}
			if(Prefs.UpdateBool(PrefName.SupplementalBackupEnabled,checkSupplementalBackupEnabled.Checked)) {
				try {
					//Inform HQ when the supplemental backups are enabled/disabled and which security admin performed the change.
					PayloadItem payloadItemStatus=new PayloadItem(
						(int)(checkSupplementalBackupEnabled.Checked?SupplementalBackupStatuses.Enabled:SupplementalBackupStatuses.Disabled),
						"SupplementalBackupStatus");
					PayloadItem payloadItemAdminUserName=new PayloadItem(Security.CurUser.UserName,"AdminUserName");
					string officeData=PayloadHelper.CreatePayload(new List<PayloadItem>() { payloadItemStatus,payloadItemAdminUserName },eServiceCode.SupplementalBackup);
					WebServiceMainHQProxy.GetWebServiceMainHQInstance().SetSupplementalBackupStatus(officeData);
				}
				catch(Exception ex) {
					ex.DoNothing();//Internet probably is unavailble right now.
				}
				SecurityLogs.MakeLogEntry(Permissions.SupplementalBackup,0,
					"Supplemental backup has been "+(checkSupplementalBackupEnabled.Checked?"Enabled":"Disabled")+".");
			}
			if(Prefs.UpdateString(PrefName.SupplementalBackupNetworkPath,textSupplementalBackupCopyNetworkPath.Text)) {
				SecurityLogs.MakeLogEntry(Permissions.SupplementalBackup,0,
					labelSupplementalBackupCopyNetworkPath.Text+" changed to '"+textSupplementalBackupCopyNetworkPath.Text+"'.");
			}
			MsgBox.Show(this,"Saved");
		}

		#endregion Supplemental Tab

	}

	///<summary>Backing up can fail at two points, when backing up the database or the A to Z images.  This delegate lets the backup thread manipulate a local variable so that we can let the user know at what point the backup failed.</summary>
	public delegate void ErrorMessageDelegate(string errorMessage);
}





















