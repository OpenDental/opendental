using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace OpenDental {
	public partial class FormUpdate : FormODBase {
		private string _buildAvailable;
		private string _buildAvailableCode;
		private string _buildAvailableDisplay;
		private string _stableAvailable;
		private string _stableAvailableCode;
		private string _stableAvailableDisplay;
		private string _betaAvailable;
		private string _betaAvailableCode;
		private string _betaAvailableDisplay;
		private string _alphaAvailable;
		private string _alphaAvailableCode;
		private string _alphaAvailableDisplay;
		private DateTime _updateTime;//Updated in SetButtonVisibility.

		///<summary></summary>
		public FormUpdate() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUpdate_Load(object sender, System.EventArgs e) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Updates are not allowed manually from within the program. Please call support.");
				Close();
				return;
			}
			LayoutMenu();
			SetButtonVisibility();
			labelVersion.Text=Lan.g(this,"Using Version:")+" "+Application.ProductVersion;
			UpdateHistory updateHistory=UpdateHistories.GetForVersion(Application.ProductVersion);
			if(updateHistory!=null) {
				labelVersion.Text+="  "+Lan.g(this,"Since")+": "+updateHistory.DateTimeUpdated.ToShortDateString();
			}
			if(PrefC.GetBool(PrefName.UpdateWindowShowsClassicView)) {
				//Default location is (74,35).  We move it 5 pixels up since butShowPrev is 5 pixels bigger then labelVersion
				LayoutManager.MoveLocation(butShowPrev,new Point(LayoutManager.Scale(76)+labelVersion.Width,LayoutManager.Scale(30)));
				panelClassic.Visible=true;
				LayoutManager.MoveLocation(panelClassic,new Point(LayoutManager.Scale(67),LayoutManager.Scale(57)));
				textConnectionMessage.SendToBack();
				textUpdateCode.Text=PrefC.GetString(PrefName.UpdateCode);
				textWebsitePath.Text=PrefC.GetString(PrefName.UpdateWebsitePath);//should include trailing /
				butDownload.Enabled=false;
				if(!Security.IsAuthorized(Permissions.Setup)) {//gives a message box if no permission
					butCheck.Enabled=false;
				}
			}
			else{
				if(Security.IsAuthorized(Permissions.Setup,true)) {
					butCheck2.Visible=true;
				}
				else {
					textConnectionMessage.Text=Lan.g(this,"Not authorized for")+" "+GroupPermissions.GetDesc(Permissions.Setup);
				}
			}
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			if(PrefC.GetBool(PrefName.UpdateWindowShowsClassicView)) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormUpdateSetup FormU=new FormUpdateSetup();
			FormU.ShowDialog();
			SetButtonVisibility();
		}

		private void SetButtonVisibility() {
			_updateTime=PrefC.GetDateT(PrefName.UpdateDateTime);
			bool showMsi=PrefC.GetBool(PrefName.UpdateShowMsiButtons);
			bool showDownloadAndInstall=(_updateTime < DateTime.Now);
			butDownloadMsiBuild.Visible=showMsi;
			butDownloadMsiStable.Visible=showMsi;
			butDownloadMsiBeta.Visible=showMsi;
			butDownloadMsiAlpha.Visible=showMsi;
			butDownloadMsiBuild.Enabled=showDownloadAndInstall;
			butDownloadMsiStable.Enabled=showDownloadAndInstall;
			butDownloadMsiBeta.Enabled=showDownloadAndInstall&&checkAcknowledgeBeta.Checked;
			butDownloadMsiAlpha.Enabled=showDownloadAndInstall;
			butInstallBuild.Enabled=showDownloadAndInstall;
			butInstallStable.Enabled=showDownloadAndInstall;
			butInstallBeta.Enabled=showDownloadAndInstall&&checkAcknowledgeBeta.Checked;
			butInstallAlpha.Enabled=showDownloadAndInstall;
		}

		private void butCheckForUpdates_Click(object sender,EventArgs e) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				MsgBox.Show(this,"Updates are only allowed from the web server");
				return;
			}
			if(PrefC.GetString(PrefName.WebServiceServerName)!="" //using web service
				&&!ODEnvironment.IdIsThisComputer(PrefC.GetString(PrefName.WebServiceServerName).ToLower()))//and not on web server 
			{
				MessageBox.Show(Lan.g(this,"Updates are only allowed from the web server")+": "+PrefC.GetString(PrefName.WebServiceServerName));
				return;
			}
			if(ReplicationServers.ServerIsBlocked()) {
				MsgBox.Show(this,"Updates are not allowed on this replication server");
				return;
			}
			Cursor=Cursors.WaitCursor;
			groupBuild.Visible=false;
			groupStable.Visible=false;
			groupBeta.Visible=false;
			groupAlpha.Visible=false;
			textConnectionMessage.Text=Lan.g(this,"Attempting to connect to web service......");
			Application.DoEvents();
			string result="";
			try {
				result=CustomerUpdatesProxy.SendAndReceiveUpdateRequestXml();
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				string friendlyMessage=Lan.g(this,"Error checking for updates.");
				FriendlyException.Show(friendlyMessage,ex);
				textConnectionMessage.Text=friendlyMessage;
				return;
			}
			textConnectionMessage.Text=Lan.g(this,"Connection successful.");
			Cursor=Cursors.Default;
			try {
				ParseXml(result);
			}
			catch(Exception ex) {
				string friendlyMessage=Lan.g(this,"Error checking for updates.");
				FriendlyException.Show(friendlyMessage,ex);
				textConnectionMessage.Text=friendlyMessage;
				return;
			}
			if(!string.IsNullOrEmpty(_buildAvailableDisplay)) {
				groupBuild.Visible=true;
				textBuild.Text=_buildAvailableDisplay;
			}
			if(!string.IsNullOrEmpty(_stableAvailableDisplay)) {
				groupStable.Visible=true;
				textStable.Text=_stableAvailableDisplay;
			}
			if(!string.IsNullOrEmpty(_betaAvailableDisplay)) {
				groupBeta.Visible=true;
				bool canUpdate=(_updateTime < DateTime.Now);
				butInstallBeta.Enabled=canUpdate&&checkAcknowledgeBeta.Checked;
				butDownloadMsiBeta.Enabled=canUpdate&&checkAcknowledgeBeta.Checked;
				textBeta.Text=_betaAvailableDisplay;
			}
			if(!string.IsNullOrEmpty(_alphaAvailableDisplay)) {
				groupAlpha.Visible=true;
				textAlpha.Text=_alphaAvailableDisplay;
			}
			if(string.IsNullOrEmpty(_betaAvailable)
				&& string.IsNullOrEmpty(_stableAvailable)
				&& string.IsNullOrEmpty(_buildAvailable) 
				&& string.IsNullOrEmpty(_alphaAvailable))
			{
				textConnectionMessage.Text+=Lan.g(this,"  There are no downloads available.");
			}
			else {
				textConnectionMessage.Text+=Lan.g(this,"  The following downloads are available.\r\n"
					+"Be sure to stop the program on all other computers in the office before installing.");
			}
		}

		///<summary>Parses the xml result from the server and uses it to fill class wide variables.  Throws exceptions.</summary>
		private void ParseXml(string result) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(result);
			XmlNode node=doc.SelectSingleNode("//Error");
			if(node!=null) {
				throw new Exception(node.InnerText);
			}
			node=doc.SelectSingleNode("//KeyDisabled");
			if(node==null) {
				//no error, and no disabled message
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			else {
				if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
				throw new Exception(node.InnerText);
			}
			#region Build
			node=doc.SelectSingleNode("//BuildAvailable");
			_buildAvailable="";
			_buildAvailableCode="";
			_buildAvailableDisplay="";
			if(node!=null) {
				node=doc.SelectSingleNode("//BuildAvailable/Display");
				if(node!=null) {
					_buildAvailableDisplay=node.InnerText;
				}
				node=doc.SelectSingleNode("//BuildAvailable/MajMinBuildF");
				if(node!=null) {
					_buildAvailable=node.InnerText;
				}
				node=doc.SelectSingleNode("//BuildAvailable/UpdateCode");
				if(node!=null) {
					_buildAvailableCode=node.InnerText;
				}
			}
			#endregion
			#region Stable
			node=doc.SelectSingleNode("//StableAvailable");
			_stableAvailable="";
			_stableAvailableCode="";
			_stableAvailableDisplay="";
			if(node!=null) {
				node=doc.SelectSingleNode("//StableAvailable/Display");
				if(node!=null) {
					_stableAvailableDisplay=node.InnerText;
				}
				node=doc.SelectSingleNode("//StableAvailable/MajMinBuildF");
				if(node!=null) {
					_stableAvailable=node.InnerText;
				}
				node=doc.SelectSingleNode("//StableAvailable/UpdateCode");
				if(node!=null) {
					_stableAvailableCode=node.InnerText;
				}
			}
			#endregion
			#region Beta
			node=doc.SelectSingleNode("//BetaAvailable");
			_betaAvailable="";
			_betaAvailableCode="";
			_betaAvailableDisplay="";
			if(node!=null) {
				node=doc.SelectSingleNode("//BetaAvailable/Display");
				if(node!=null) {
					_betaAvailableDisplay=node.InnerText;
				}
				node=doc.SelectSingleNode("//BetaAvailable/MajMinBuildF");
				if(node!=null) {
					_betaAvailable=node.InnerText;
				}
				node=doc.SelectSingleNode("//BetaAvailable/UpdateCode");
				if(node!=null) {
					_betaAvailableCode=node.InnerText;
				}
			}
			#endregion
			#region Alpha
			_alphaAvailable="";
			_alphaAvailableCode="";
			_alphaAvailableDisplay="";
			//Never let the program crash for alpha version related code.  It is never THAT important.
			ODException.SwallowAnyException(()=> {
				node=doc.SelectSingleNode("//AlphaAvailable");
				if(node!=null) {
					groupAlpha.Visible=true;
					node=doc.SelectSingleNode("//AlphaAvailable/Display");
					if(node!=null) {
						_alphaAvailableDisplay=node.InnerText;
					}
					node=doc.SelectSingleNode("//AlphaAvailable/MajMinBuildF");
					if(node!=null) {
						_alphaAvailable=node.InnerText;
					}
					node=doc.SelectSingleNode("//AlphaAvailable/UpdateCode");
					if(node!=null) {
						_alphaAvailableCode=node.InnerText;
					}
				}
			});
			#endregion
		}

		private void butShowPrev_Click(object sender,EventArgs e) {
			using FormPreviousVersions FormSPV=new FormPreviousVersions();
			FormSPV.ShowDialog();
		}

		///<summary>Determines if the current application is ran within the dynamic folder (startup path contains "DynamicMode").
		///If so, shows a message to the user and then returns true so that the calling method can stop the user from updating.</summary>
		private bool IsDynamicMode() {
			if(Application.StartupPath.Contains("DynamicMode")) {
				MsgBox.Show(this,"Cannot perform update when using Dynamic Mode.");
				return true;
			}
			return false;
		}

		#region Installs

		private void DownloadInstallPatchForVersion(string version,string updateCode,bool showFormUpdateInstallMsg) {
			if(IsDynamicMode()) {
				return;
			}
			if(showFormUpdateInstallMsg) {
				using FormUpdateInstallMsg FormUIM=new FormUpdateInstallMsg();
				FormUIM.ShowDialog();
				if(FormUIM.DialogResult!=DialogResult.OK) {
					return;
				}
			}
			string patchName="Setup.exe";
			string fileNameWithVers=version;//6.9.23F
			fileNameWithVers=fileNameWithVers.Replace("F","");//6.9.23
			fileNameWithVers=fileNameWithVers.Replace(".","_");//6_9_23
			fileNameWithVers="Setup_"+fileNameWithVers+".exe";//Setup_6_9_23.exe
			string destDir=ImageStore.GetPreferredAtoZpath();
			string destPath2=null;
			if(destDir==null) {//Not using A to Z folders?
				destDir=PrefC.GetTempFolderPath();
			}
			else {//using A to Z folders.
				destPath2=ODFileUtils.CombinePaths(destDir,"SetupFiles");
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(destPath2)) {
					Directory.CreateDirectory(destPath2);
				}
				else if(CloudStorage.IsCloudStorage) {
					destDir=PrefC.GetTempFolderPath();//Cloud needs it to be downloaded to a local temp folder
				}
				destPath2=ODFileUtils.CombinePaths(destPath2,fileNameWithVers);
			}
			PrefL.DownloadInstallPatchFromURI(PrefC.GetString(PrefName.UpdateWebsitePath)+updateCode+"/"+patchName,//Source URI
				ODFileUtils.CombinePaths(destDir,patchName),//Local destination file.
				true,true,
				destPath2);//second destination file.  Might be null.
		}

		private void butInstallBuild_Click(object sender,EventArgs e) {
			DownloadInstallPatchForVersion(_buildAvailable,_buildAvailableCode,false);
		}

		private void butInstallStable_Click(object sender,EventArgs e) {
			DownloadInstallPatchForVersion(_stableAvailable,_stableAvailableCode,true);
		}

		private void butInstallBeta_Click(object sender,EventArgs e) {
			if(!checkAcknowledgeBeta.Checked) {
				MsgBox.Show("You must check the acknowledgement in order to install a beta version.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you really want to install a beta version?"
				+"  Do NOT do this unless you are OK with some bugs.  Continue?"))
			{
				return;
			}
			DownloadInstallPatchForVersion(_betaAvailable,_betaAvailableCode,true);
		}

		private void butInstallAlpha_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you really want to install a alpha version?\r\n"
				+"Do NOT do this unless you enjoy bugs.\r\n\r\n"
				+"Continue?"))
			{
				return;
			}
			DownloadInstallPatchForVersion(_alphaAvailable,_alphaAvailableCode,true);
		}

		private void checkAcknowledgeBeta_CheckedChanged(object sender,EventArgs e) {
			if(_updateTime < DateTime.Now) {
				butInstallBeta.Enabled=checkAcknowledgeBeta.Checked;
				butDownloadMsiBeta.Enabled=checkAcknowledgeBeta.Checked;
			}
			else {
				butInstallBeta.Enabled=false;
				butDownloadMsiBeta.Enabled=false;
			}
		}

		#endregion

		#region Download MSIs

		private void DownloadAndStartMSI(string updateCode) {
			if(IsDynamicMode()) {
				return;
			}
			string fileName=PrefC.GetString(PrefName.UpdateWebsitePath)+updateCode+"/OpenDental.msi";
			try {
				Process.Start(fileName);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem launching")+" "+fileName,ex);
			}
		}

		///<summary>Downloads the build update MSI file and starts the install, closing OpenDental.</summary>
		private void butDownMsiBuild_Click(object sender,EventArgs e) {
			DownloadAndStartMSI(_buildAvailableCode);
		}

		///<summary>Downloads the stable update MSI file and starts the install, closing OpenDental.</summary>
		private void butDownloadMsiStable_Click(object sender,EventArgs e) {
			DownloadAndStartMSI(_stableAvailableCode);
		}

		///<summary>Downloads the beta update MSI file and starts the install, closing OpenDental.</summary>
		private void butDownloadMsiBeta_Click(object sender,EventArgs e) {
			if(!checkAcknowledgeBeta.Checked) {
				MsgBox.Show("You must check the acknowledgement in order to download a beta version.");
				return;
			}
			DownloadAndStartMSI(_betaAvailableCode);
		}

		private void butDownloadMsiAlpha_Click(object sender,EventArgs e) {
			DownloadAndStartMSI(_alphaAvailableCode);
		}

		#endregion

		#region Classic View

		private void butCheck_Click(object sender, System.EventArgs e) {
			if(IsDynamicMode()) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			SavePrefs();
			CheckMain();
			Cursor=Cursors.Default;
		}

		private void CheckMain() {
			butDownload.Enabled=false;
			textResult.Text="";
			textResult2.Text="";
			if(textUpdateCode.Text.Length==0) {
				textResult.Text+=Lan.g(this,"Registration number not valid.");
				return;
			}
			string updateInfoMajor="";
			string updateInfoMinor="";
			butDownload.Enabled=PrefL.ShouldDownloadUpdate(textWebsitePath.Text,textUpdateCode.Text,out updateInfoMajor,out updateInfoMinor);
			textResult.Text=updateInfoMajor;
			textResult2.Text=updateInfoMinor;
		}

		private void butDownload_Click(object sender, System.EventArgs e) {
			if(IsDynamicMode()) {
				return;
			}
			string patchName="Setup.exe";
			string destDir=ImageStore.GetPreferredAtoZpath();
			if(destDir==null || CloudStorage.IsCloudStorage) {
				destDir=PrefC.GetTempFolderPath();
			}
			PrefL.DownloadInstallPatchFromURI(textWebsitePath.Text+textUpdateCode.Text+"/"+patchName,//Source URI
				ODFileUtils.CombinePaths(destDir,patchName),true,false,null);//Local destination file.
		}

		private void SavePrefs() {
			bool changed=false;
			if(Prefs.UpdateString(PrefName.UpdateCode,textUpdateCode.Text)) {
				changed=true;
			}
			if(Prefs.UpdateString(PrefName.UpdateWebsitePath,textWebsitePath.Text)) {
				changed=true;
			}
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		///<summary>No longer used here. Moved to FormAbout.</summary>
		private void butLicense_Click(object sender,EventArgs e) {
			using FormLicense FormL=new FormLicense();
			FormL.ShowDialog();
		}

		#endregion

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormUpdate_FormClosing(object sender,FormClosingEventArgs e) {
			if(Security.IsAuthorized(Permissions.Setup,DateTime.Now,true)	&& PrefC.GetBool(PrefName.UpdateWindowShowsClassicView)) {
				SavePrefs();
			}
		}
	}

	
}





















