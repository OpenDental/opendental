using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAtoZFoldersCreate : FormODBase {

		///<summary></summary>
		public FormAtoZFoldersCreate()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAtoZFoldersCreate_Load(object sender,EventArgs e) {

		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!Directory.Exists(textLocation.Text)){
				MsgBox.Show(this,"Location does not exist.");
				return;
			}
			if(Directory.Exists(ODFileUtils.CombinePaths(textLocation.Text,textName.Text))) {
				MsgBox.Show(this,"Folder already exists.");
				return;
			}
			try {
				FileSystemAccessRule fsar=new FileSystemAccessRule("everyone",FileSystemRights.FullControl,AccessControlType.Allow);
				DirectorySecurity ds=new DirectorySecurity();
				ds.AddAccessRule(fsar);
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
						Directory.CreateDirectory(pathToCreate,ds);
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
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Created AtoZ Folder");
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















