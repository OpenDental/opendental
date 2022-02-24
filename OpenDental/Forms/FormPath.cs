using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public partial class FormPath : FormODBase {
		///<summary>If this is set to true before opening this form, then the program cannot find the AtoZ path and needs user input.</summary>
		public bool IsStartingUp;
		
		#region Dropbox Private Variables
		private Program _progCur;
		private ProgramProperty _ppDropboxPathAtoZ;
		private ProgramProperty _ppDropboxAccessToken;
		///<summary>Set to true if the Dropbox API has been loaded already.</summary>
		private bool _hasDropboxLoaded;
		#endregion

		#region Sftp Private Variables
		///<summary>Set to true if the Sftp stuff has been loaded already.</summary>
		private bool _hasSftpLoaded;
		private ProgramProperty _ppSftpPathAtoZ;
		private ProgramProperty _ppSftpHostname;
		private ProgramProperty _ppSftpUsername;
		private ProgramProperty _ppSftpPassword;
		#endregion

		///<summary>This is the database storage type that the user has chosen (or was pulled from the database.
		///DO NOT change the value of this variable outside of SetRadioButtonChecked() or there is a chance for a stack overflow exception</summary>
		private DataStorageType _storageType=DataStorageType.LocalAtoZ;

		///<summary></summary>
		public FormPath(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//We only show the tabs in the designer for development purposes.  We want to hide them for our users.
			//Because the tab control is in "flat buttons" appearance and "fixed size" style the tabs will not show even if they are one pixel tall.
			//0,0 does not work because some size is required.
			tabControlDataStorageType.ItemSize=new Size(0,1);
		}

		private void FormPath_Load(object sender, System.EventArgs e){
			if(!IsStartingUp && !Security.IsAuthorized(Permissions.Setup)) {//Verify user has Setup permission to change paths, after user has logged in.
				butOK.Enabled=false;
			}
			textDocPath.Text=PrefC.GetString(PrefName.DocPath);
			//ComputerPref compPref=ComputerPrefs.GetForLocalComputer();
			if(ReplicationServers.Server_id==0) {
				labelServerPath.Visible=false;
				textServerPath.Visible=false;
				butBrowseServer.Visible=false;
			}
			else {
				labelServerPath.Text="Path override for this server.  Server id = "+ReplicationServers.Server_id.ToString();
				textServerPath.Text=ReplicationServers.GetAtoZpath();
			}
			textLocalPath.Text=OpenDentBusiness.FileIO.FileAtoZ.LocalAtoZpath;//This was set on startup.  //compPref.AtoZpath;
			textExportPath.Text=PrefC.GetString(PrefName.ExportPath);
			textLetterMergePath.Text=PrefC.GetString(PrefName.LetterMergePath);
			SetRadioButtonChecked(PrefC.AtoZfolderUsed);
			// The opt***_checked event will enable/disable the appropriate UI elements.
			checkMultiplePaths.Checked=(textDocPath.Text.LastIndexOf(';')!=-1);	
			//Also set the "multiple paths" checkbox at startup based on the current image folder list format. No need to store this info in the db.
			if(IsStartingUp) {//and failed to find path
				MsgBox.Show(this,"Could not find the path for the AtoZ folder.");
				if(Security.CurUser==null || !Security.IsAuthorized(Permissions.Setup)) {
					//The user is still allowed to set the "Path override for this computer", thus the user has a way to temporariliy get into OD in worst case.
					//For example, if the primary folder path is wrong or has changed, the user can set the path override for this computer to get into OD, then
					//can to to Setup | Data Paths to fix the primary path.
					DisableMostControls();
					textLocalPath.ReadOnly=false;
					butBrowseLocal.Enabled=true;
					ActiveControl=textLocalPath;//Focus on textLocalPath, since this is the only textbox the user can edit in this case.
				}
			}
			if(ODBuild.IsWeb()) {
				textSftpUsername.UseSystemPasswordChar=true;
				butOK.Enabled=false;
				DisableMostControls();
			}
		}

		private void DisableMostControls() {
			radioUseFolder.Enabled=false;
			textDocPath.ReadOnly=true;
			butBrowseDoc.Enabled=false;
			checkMultiplePaths.Enabled=false;
			textServerPath.ReadOnly=true;
			butBrowseServer.Enabled=false;
			radioAtoZfolderNotRequired.Enabled=false;
			textExportPath.ReadOnly=true;
			butBrowseExport.Enabled=false;
			textLetterMergePath.ReadOnly=true;
			butBrowseLetter.Enabled=false;
			textLocalPath.ReadOnly=true;
			butBrowseLocal.Enabled=false;
			radioDropboxStorage.Enabled=false;
			radioSftp.Enabled=false;
			textAtoZPath.ReadOnly=true;
			butAuthorize.Enabled=false;
			textSftpAtoZ.ReadOnly=true;
			textSftpHostname.ReadOnly=true;
			textSftpPassword.ReadOnly=true;
			textSftpUsername.ReadOnly=true;
			butSftpClear.Enabled=false;
		}

		private void SetRadioButtonChecked(DataStorageType storageType) {
			string error="";
			switch(storageType) {
				case DataStorageType.LocalAtoZ:
					_storageType=DataStorageType.LocalAtoZ;
					radioUseFolder.Checked=true;//Will only do something when SetRadioButtonChecked is called on Load
					tabControlDataStorageType.SelectedTab=tabAtoZ;
					break;
				case DataStorageType.InDatabase:
					_storageType=DataStorageType.InDatabase;
					radioAtoZfolderNotRequired.Checked=true;//Will only do something when SetRadioButtonChecked is called on Load
					tabControlDataStorageType.SelectedTab=tabInDatabase;
					break;
				case DataStorageType.DropboxAtoZ:
					error="";
					if(!LoadDropboxSetup(out error)) {
						SetRadioButtonChecked(_storageType);//This can cause a stack overflow exception if someone sets _storageType from outside of this method.
						MessageBox.Show(error);
						return;
					}
					radioDropboxStorage.Checked=true;//Will only do something when SetRadioButtonChecked is called on Load
					tabControlDataStorageType.SelectedTab=tabDropbox;
					_storageType=DataStorageType.DropboxAtoZ;
					break;
				case DataStorageType.SftpAtoZ:
					error="";
					if(!LoadSftpSetup(out error)) {
						SetRadioButtonChecked(_storageType);//This can cause a stack overflow exception if someone sets _storageType from outside of this method.
						MessageBox.Show(error);
						return;
					}
					radioSftp.Checked=true;//Will only do something when SetRadioButtonChecked is called on Load
					tabControlDataStorageType.SelectedTab=tabSftp;
					_storageType=DataStorageType.SftpAtoZ;
					break;
				default:
					MsgBox.Show(this,"There was an error retrieving your preferred data storage method.  Please call support to solve this issue.");
					break;
			}
		}

		///<summary>Tries to show the file browser dialog to the user.  Returns true if the user actually selected a path from the dialog.
		///Returns false if the user cancels out.  Also, shows a warning message and returns false if an exception occurred.</summary>
		private bool ShowFileBrowserDialog() {
			//A customer is having a "Unable to retrieve root folder" unhandled exception occur when trying to show the file browser dialog.
			//Therefore, try to show the dialog and if any exception occurs simply show a message box giving some suggestions to the user.
			try {
				return (fb.ShowDialog()==DialogResult.OK);
			}
			catch(Exception) {
				MsgBox.Show(this,"There was an error showing the Browse window.\r\nTry running as an Administrator or manually typing in a path.");
				return false;
			}
		}

		///<summary>Returns the given path with the local OS path separators as necessary.</summary>
		public static string FixDirSeparators(string path){
			if(Environment.OSVersion.Platform==PlatformID.Unix){
				path.Replace('\\',Path.DirectorySeparatorChar);
			}
			else{//Windows
				path.Replace('/',Path.DirectorySeparatorChar);
			}
			return path;
		}

		private void butBrowseDoc_Click(object sender,EventArgs e) {
			if(!ShowFileBrowserDialog()) {
				return;
			}
			//Ensure that the path entered has slashes matching the current OS (in case entered manually).
			string path=FixDirSeparators(fb.SelectedPath);
			if(checkMultiplePaths.Checked && textDocPath.Text.Length>0) {
				string messageText=Lan.g(this,"Replace existing document paths? Click No to add path to existing document paths.");
				switch(MessageBox.Show(messageText,"",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Yes:
						textDocPath.Text=path;//Replace existing paths with new path.
						break;
					case DialogResult.No://Append to existing paths?
						//Do not append a path which is already present in the list.
						if(!IsPathInList(path,textDocPath.Text)) {
							textDocPath.Text=textDocPath.Text+";"+path;
						}
						break;
					default://Cancel button.
						break;
				}
			}
			else{
				textDocPath.Text=path;//Just replace existing paths with new path.
			}
		}

		private void butBrowseServer_Click(object sender,EventArgs e) {
			if(ShowFileBrowserDialog()) {
				textServerPath.Text=fb.SelectedPath;
			}
		}

		private void butBrowseLocal_Click(object sender,EventArgs e) {
			if(ShowFileBrowserDialog()) {
				textLocalPath.Text=fb.SelectedPath;
			}
		}

		private void butBrowseExport_Click(object sender, System.EventArgs e) {
			if(ShowFileBrowserDialog()) {
				textExportPath.Text=fb.SelectedPath;
			}
		}

		private void butBrowseLetter_Click(object sender, System.EventArgs e) {
			if(ShowFileBrowserDialog()) {
				textLetterMergePath.Text=fb.SelectedPath;
			}
		}

		///<summary>Returns true if the given path is part of the imagePaths list, false otherwise.</summary>
		private static bool IsPathInList(string path,string imagePaths){
			string[] pathArray=imagePaths.Split(new char[] { ';' });
			for(int i=0;i<pathArray.Length;i++){
				if(pathArray[i]==path){//Case sensitive (since these could be unix paths).
					return true;
				}
			}
			return false;
		}

		private void radioUseFolder_Click(object sender,EventArgs e) {
			labelPathSameForAll.Enabled = radioUseFolder.Checked;
			textDocPath.Enabled = radioUseFolder.Checked;
			butBrowseDoc.Enabled = radioUseFolder.Checked;
			checkMultiplePaths.Enabled = radioUseFolder.Checked;
			//even though server path might not be visible:
			labelServerPath.Enabled=radioUseFolder.Checked;
			textServerPath.Enabled=radioUseFolder.Checked;
			butBrowseServer.Enabled=radioUseFolder.Checked;
			//
			labelLocalPath.Enabled=radioUseFolder.Checked;
			textLocalPath.Enabled=radioUseFolder.Checked;
			butBrowseLocal.Enabled=radioUseFolder.Checked;
			SetRadioButtonChecked(DataStorageType.LocalAtoZ);
		}

		private void radioAtoZfolderNotRequired_Click(object sender,EventArgs e) {
			if(radioAtoZfolderNotRequired.Checked){//user attempting to use db to store images
				using InputBox inputbox=new InputBox("Please enter password");
				inputbox.ShowDialog();
				if(inputbox.DialogResult!=DialogResult.OK){
					SetRadioButtonChecked(_storageType);
					return;
				}
				if(inputbox.textResult.Text!="abracadabra"){//to keep ignorant people from clicking this box.
					SetRadioButtonChecked(_storageType);
					MsgBox.Show(this,"Wrong password");
					return;
				}
			}
			SetRadioButtonChecked(DataStorageType.InDatabase);
		}

		#region Dropbox Methods and Events

		///<summary>Returns true if loading Dropbox settings was successful.  False is something went wrong.
		///errorMsg will contain translated details about what went wrong in the case of a failure.</summary>
		private bool LoadDropboxSetup(out string errorMsg) {
			errorMsg="";
			if(_hasDropboxLoaded) {
				return true;
			}
			_progCur=Programs.GetCur(ProgramName.Dropbox);
			if(_progCur==null) {//Should never happen.
				errorMsg=Lan.g(this,"The Dropbox bridge is missing from the database.");
				return false;
			}
			try {
				List<ProgramProperty> listProperties=ProgramProperties.GetForProgram(_progCur.ProgramNum);
				_ppDropboxPathAtoZ=listProperties.FirstOrDefault(x => x.PropertyDesc==Dropbox.PropertyDescs.AtoZPath);
				_ppDropboxAccessToken=listProperties.FirstOrDefault(x => x.PropertyDesc==Dropbox.PropertyDescs.AccessToken);
				textAtoZPath.Text=_ppDropboxPathAtoZ.PropertyValue;
				textAccessToken.Text=_ppDropboxAccessToken.PropertyValue;
			}
			catch(Exception e) {
				errorMsg=Lan.g(this,"You are missing a program property for Dropbox.  Please contact support to resolve this issue.")+"\r\n"+e.StackTrace;
				return false;
			}
			_hasDropboxLoaded=true;
			return true;
		}

		private void butAuthorize_Click(object sender,EventArgs e) {
			using FormDropboxAuthorize FormDA=new FormDropboxAuthorize();
			FormDA.ProgramPropertyAccessToken=_ppDropboxAccessToken;
			FormDA.ShowDialog();
			if(FormDA.DialogResult==DialogResult.OK) {
				_ppDropboxAccessToken=FormDA.ProgramPropertyAccessToken.Copy();
				textAccessToken.Text=FormDA.ProgramPropertyAccessToken.PropertyValue;
			}
		}

		private void radioDropboxStorage_Click(object sender,EventArgs e) {
			if(_storageType==DataStorageType.DropboxAtoZ) {
				return;
			}
			SetRadioButtonChecked(DataStorageType.DropboxAtoZ);
		}
		#endregion

		#region Sftp

		///<summary>Returns true if loading Dropbox settings was successful.  False is something went wrong.
		///errorMsg will contain translated details about what went wrong in the case of a failure.</summary>
		private bool LoadSftpSetup(out string errorMsg) {
			errorMsg="";
			if(_hasSftpLoaded) {
				return true;
			}
			_progCur=Programs.GetCur(ProgramName.SFTP);
			if(_progCur==null) {//Should never happen.
				errorMsg=Lan.g(this,"The SFTP bridge is missing from the database.");
				return false;
			}
			try {
				List<ProgramProperty> listProperties=ProgramProperties.GetForProgram(_progCur.ProgramNum);
				_ppSftpPathAtoZ=listProperties.FirstOrDefault(x => x.PropertyDesc==ODSftp.PropertyDescs.AtoZPath);
				_ppSftpHostname=listProperties.FirstOrDefault(x => x.PropertyDesc==ODSftp.PropertyDescs.SftpHostname);
				_ppSftpUsername=listProperties.FirstOrDefault(x => x.PropertyDesc==ODSftp.PropertyDescs.UserName);
				_ppSftpPassword=listProperties.FirstOrDefault(x => x.PropertyDesc==ODSftp.PropertyDescs.Password);
				textSftpAtoZ.Text=_ppSftpPathAtoZ.PropertyValue;
				textSftpHostname.Text=_ppSftpHostname.PropertyValue;
				textSftpUsername.Text=_ppSftpUsername.PropertyValue;
				textSftpPassword.Text=_ppSftpPassword.PropertyValue;
				if(textSftpPassword.Text.Length>0) {
					string pass="";
					if(CDT.Class1.DecryptSftp(textSftpPassword.Text,out pass)) {
						textSftpPassword.Text=pass;
					}
					textSftpPassword.UseSystemPasswordChar=true;
					textSftpPassword.ReadOnly=true;
				}
			}
			catch(Exception e) {
				errorMsg=Lan.g(this,"You are missing a program property for SFTP.  Please contact support to resolve this issue.")+"\r\n"+e.StackTrace;
				return false;
			}
			_hasSftpLoaded=true;
			return true;
		}

		private void butSftpClear_Click(object sender,EventArgs e) {
			textSftpPassword.Text="";
			textSftpPassword.UseSystemPasswordChar=false;
			textSftpPassword.ReadOnly=false;
		}
		
		private void radioSftp_Click(object sender,EventArgs e) {
			if(_storageType==DataStorageType.SftpAtoZ) {
				return;
			}
			SetRadioButtonChecked(DataStorageType.SftpAtoZ);
		}

		#endregion

		private void butOK_Click(object sender, System.EventArgs e){
			//remember that user might be using a website or a linux box to store images, therefore must allow forward slashes.
			if(radioUseFolder.Checked){
				if(textLocalPath.Text!="") {
					if(OpenDentBusiness.FileIO.FileAtoZ.GetValidPathFromString(textLocalPath.Text)==null) {
						MsgBox.Show(this,"The path override for this computer is invalid.  The folder must exist and must contain all 26 A through Z folders.");
						return;
					}
				}
				else if(textServerPath.Text!="") {
					if(OpenDentBusiness.FileIO.FileAtoZ.GetValidPathFromString(textServerPath.Text)==null) {
						MsgBox.Show(this,"The path override for this server is invalid.  The folder must exist and must contain all 26 A through Z folders.");
						return;
					}
				}
				else {
					if(OpenDentBusiness.FileIO.FileAtoZ.GetValidPathFromString(textDocPath.Text)==null) {
						MsgBox.Show(this,"The path is invalid.  The folder must exist and must contain all 26 A through Z folders.");
						return;
					}
				}				
    		}
			if(radioDropboxStorage.Checked && PrefC.AtoZfolderUsed!=DataStorageType.DropboxAtoZ
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning: Updating workstations older than 16.3 while using Dropbox may cause issues."
					+"\r\nIf experienced, use Setup.exe located in AtoZ folder on DropBox to reinstall."))
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(radioDropboxStorage.Checked && !OpenDentalCloud.Dropbox.FileExists(textAccessToken.Text,
				ODFileUtils.CombinePaths(textAtoZPath.Text,"A",'/'))) 
			{
				Cursor=Cursors.Default;
				MsgBox.Show(this,"The Dropbox folder cannot be accessed or does not exist. The folder must contain all 26 A through Z folders.");
				return;
			}
			Cursor=Cursors.Default;
			if(radioDropboxStorage.Checked && ProgramProperties.UpdateProgramPropertyWithValue(_ppDropboxPathAtoZ,textAtoZPath.Text)) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			Cursor=Cursors.WaitCursor;
			if(radioSftp.Checked) { 
				try {
					if(!OpenDentalCloud.Sftp.FileExists(textSftpHostname.Text,textSftpUsername.Text,textSftpPassword.Text,
						ODFileUtils.CombinePaths(textSftpAtoZ.Text,"A",'/'))) 
					{
						Cursor=Cursors.Default;
						MsgBox.Show(this,"The SFTP folder cannot be accessed or does not exist. The folder must contain all 26 A through Z folders.");
						return;
					}
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					MessageBox.Show(Lan.g(this,"Error connecting to SFTP host: ")+ex.Message);
					return;
				}
			}
			Cursor=Cursors.Default;
			string sftpWarningMsg=Lan.g(this,"Warning: Updating workstations older than 16.3 while using SFTP may cause issues."
				+"\r\nIf experienced, use the Setup.exe located in the AtoZ folder on your SFTP server to reinstall.");
			if(radioSftp.Checked && PrefC.AtoZfolderUsed!=DataStorageType.SftpAtoZ
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,sftpWarningMsg))
			{
				return;
			}
			string sftpPass="";
			if(radioSftp.Checked && (ProgramProperties.UpdateProgramPropertyWithValue(_ppSftpPathAtoZ,textSftpAtoZ.Text)
					| ProgramProperties.UpdateProgramPropertyWithValue(_ppSftpHostname,textSftpHostname.Text)
					| ProgramProperties.UpdateProgramPropertyWithValue(_ppSftpUsername,textSftpUsername.Text)
					| (CDT.Class1.EncryptSftp(textSftpPassword.Text,out sftpPass) 
						&& ProgramProperties.UpdateProgramPropertyWithValue(_ppSftpPassword,sftpPass)))) 
			{
				DataValid.SetInvalid(InvalidType.Programs);
			}
			if(	Prefs.UpdateInt(PrefName.AtoZfolderUsed,(int)_storageType)
				| Prefs.UpdateString(PrefName.DocPath,textDocPath.Text)
				| Prefs.UpdateString(PrefName.ExportPath,textExportPath.Text)
				| Prefs.UpdateString(PrefName.LetterMergePath,textLetterMergePath.Text))
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(OpenDentBusiness.FileIO.FileAtoZ.LocalAtoZpath!=textLocalPath.Text) {//if local path changed
				OpenDentBusiness.FileIO.FileAtoZ.LocalAtoZpath=textLocalPath.Text;
				//ComputerPref compPref=ComputerPrefs.GetForLocalComputer();
				ComputerPrefs.LocalComputer.AtoZpath=OpenDentBusiness.FileIO.FileAtoZ.LocalAtoZpath;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			if(ReplicationServers.GetAtoZpath()!=textServerPath.Text) {
				ReplicationServer server=ReplicationServers.GetForLocalComputer();
				server.AtoZpath=textServerPath.Text;
				ReplicationServers.Update(server);
				DataValid.SetInvalid(InvalidType.ReplicationServers);
			}
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Data Path");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormPath_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			fb.Dispose();
			/*
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(!IsStartingUp) {
				return;
			}
			//no need to check paths here.  If user hits cancel when starting up, it should always notify and exit.
			if(radioUseFolder.Checked 
				&& ImageStore.GetValidPathFromString(textDocPath.Text)==null 
				&& ImageStore.GetValidPathFromString(textLocalPath.Text)==null) 
			{
				MsgBox.Show(this,"Invalid A to Z path.  Closing program.");
				Application.Exit();
			}*/
		}
	}
}
