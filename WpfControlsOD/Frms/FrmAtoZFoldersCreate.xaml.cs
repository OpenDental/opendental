using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmAtoZFoldersCreate : FrmODBase {

		///<summary></summary>
		public FrmAtoZFoldersCreate()
		{
			InitializeComponent();
			Load+=FrmAtoZFoldersCreate_Load;
			PreviewKeyDown+=FrmAtoZFoldersCreate_PreviewKeyDown;
		}

		private void FrmAtoZFoldersCreate_Load(object sender,EventArgs e) {
			Lang.F(this);
		}

		private void FrmAtoZFoldersCreate_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!Directory.Exists(textLocation.Text)){
				MsgBox.Show(this,"Location does not exist.");
				return;
			}
			if(Directory.Exists(ODFileUtils.CombinePaths(textLocation.Text,textName.Text))) {
				MsgBox.Show(this,"Folder already exists.");
				return;
			}
			try {
				FileSystemAccessRule fileSystemAccessRule=new FileSystemAccessRule("everyone",FileSystemRights.FullControl,AccessControlType.Allow);
				DirectorySecurity directorySecurity=new DirectorySecurity();
				directorySecurity.AddAccessRule(fileSystemAccessRule);
				string requestDir=textLocation.Text;
				string rootFolderName=textName.Text;
				string rootDir=ODFileUtils.CombinePaths(requestDir,rootFolderName);
				//Enable file sharing for the A to Z folder.
				if(Environment.OSVersion.Platform==PlatformID.Unix) {
					//Process.Start("net","usershare add OpenDentImages \""+rootDir+"\"");//for future use.
				}
				else {//Windows
					Process.Start("NET","SHARE OpenDentImages=\""+rootDir+"\"");
				}
				//All folder names to be created should be put in this list, so that each folder is created exactly
				//the same way.
				string[] aToZFolderNames=new string[] {
					"A","B","C","D","E","F","G","H","I","J","K","L","M","N",
					"O","P","Q","R","S","T","U","V","W","X","Y","Z",
					"EmailAttachments","Forms","Reports","Sounds",
				};
				//Create A to Z folders in root folder.
				for(int i=0;i<aToZFolderNames.Length;i++) {
					string pathToCreate=ODFileUtils.CombinePaths(rootDir,aToZFolderNames[i]);
					if(!Directory.Exists(pathToCreate)) {
						// Mono does support Directory.CreateDirectory(string, DirectorySecurity)
						Directory.CreateDirectory(pathToCreate,directorySecurity);
					}
				}
				//Save new image path into the DocPath and 
				//set "use A to Z folders" check-box to checked.
				Prefs.UpdateString(PrefName.DocPath,rootDir);
				Prefs.UpdateString(PrefName.AtoZfolderUsed,"1");
				Cache.Refresh(InvalidType.Prefs);
				//Prefs_client.RefreshClient();
			}
			catch(Exception ex) {
				Logger.openlog.LogMB("Failed to create A to Z folders: "+ex.ToString(),Logger.Severity.ERROR);
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Created AtoZ Folder");
			IsDialogOK=true;
		}

	}
}