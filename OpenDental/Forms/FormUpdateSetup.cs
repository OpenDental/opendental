using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using Ionic.Zip;
using System.Xml;
using System.Text;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormUpdateSetup : FormODBase {
		private DateTime _updateTime;

		///<summary></summary>
		public FormUpdateSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUpdateSetup_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				butChangeRegKey.Enabled=false;
				butOK.Enabled=false;
			}
			textUpdateServerAddress.Text=PrefC.GetString(PrefName.UpdateServerAddress);
			textWebsitePath.Text=PrefC.GetString(PrefName.UpdateWebsitePath);
			textWebProxyAddress.Text=PrefC.GetString(PrefName.UpdateWebProxyAddress);
			textWebProxyUserName.Text=PrefC.GetString(PrefName.UpdateWebProxyUserName);
			textWebProxyPassword.Text=PrefC.GetString(PrefName.UpdateWebProxyPassword);
			string regkey=PrefC.GetString(PrefName.RegistrationKey);
			if(regkey.Length==16){
				textRegKey.Text=regkey.Substring(0,4)+"-"+regkey.Substring(4,4)+"-"+regkey.Substring(8,4)+"-"+regkey.Substring(12,4);
			}
			else{
				textRegKey.Text=regkey;
			}
			textMultiple.Text=PrefC.GetString(PrefName.UpdateMultipleDatabases);
			checkShowMsi.Checked=PrefC.GetBool(PrefName.UpdateShowMsiButtons);
			_updateTime=PrefC.GetDateT(PrefName.UpdateDateTime);
			textUpdateTime.Text=_updateTime.ToString();
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				labelRecopy.Text=@"Recopy all of the files from C:\Program Files\Open Dental\ into a special place in the database for future use in updating other computers.";
			}
		}

		private void textRegKey_KeyUp(object sender,KeyEventArgs e) {
			int cursor=textRegKey.SelectionStart;
			//textRegKey.Text=textRegKey.Text.ToUpper();
			int length=textRegKey.Text.Length;
			if(Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{5}$")) {
				textRegKey.Text=textRegKey.Text.Substring(0,4)+"-"+textRegKey.Text.Substring(4);
			}
			else if(Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{4}-[A-Z0-9]{5}$")) {
				textRegKey.Text=textRegKey.Text.Substring(0,9)+"-"+textRegKey.Text.Substring(9);
			}
			else if(Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{5}$")) {
				textRegKey.Text=textRegKey.Text.Substring(0,14)+"-"+textRegKey.Text.Substring(14);
			}
			if(textRegKey.Text.Length>length) {
				cursor++;
			}
			textRegKey.SelectionStart=cursor;
		}

		private void textRegKey_TextChanged(object sender,EventArgs e) {
			int cursor=textRegKey.SelectionStart;
			textRegKey.Text=textRegKey.Text.ToUpper();
			textRegKey.SelectionStart=cursor;
		}

		private void butChangeRegKey_Click(object sender,EventArgs e) {
			using FormRegistrationKey formR=new FormRegistrationKey();
			formR.ShowDialog();
			DataValid.SetInvalid(InvalidType.Prefs);
			string regkey=PrefC.GetString(PrefName.RegistrationKey);
			if(regkey.Length==16){
				textRegKey.Text=regkey.Substring(0,4)+"-"+regkey.Substring(4,4)+"-"+regkey.Substring(8,4)+"-"+regkey.Substring(12,4);
			}
			else{
				textRegKey.Text=regkey;
			}
		}

		private void butRecopy_Click(object sender,EventArgs e) {
			Version versionCurrent=new Version(Application.ProductVersion);
			string folderUpdate=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),"UpdateFiles");
			if(Directory.Exists(folderUpdate)) {
				try {
					Directory.Delete(folderUpdate,true);
				}
				catch {
					MsgBox.Show(this,"Recopy failed.  Please run as administrator then try again.");
					return;
				}
			}
			Cursor=Cursors.WaitCursor;
			if(!PrefL.CopyFromHereToUpdateFiles(versionCurrent,false,true,true)) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Recopied.");
		}

		private void butChangeTime_Click(object sender,EventArgs e) {
			using FormTimePick FormTP=new FormTimePick(true);
			if(_updateTime!=DateTime.MinValue) {
				FormTP.SelectedDTime=_updateTime;
			}
			FormTP.ShowDialog();
			if(FormTP.DialogResult==DialogResult.OK) {
				_updateTime=FormTP.SelectedDTime;
				textUpdateTime.Text=_updateTime.ToString();
				if(Prefs.UpdateDateT(PrefName.UpdateDateTime,_updateTime)) {
					//Updating to db now in case the user does not have enough permission to click the OK button on this form.			
					Cursor=Cursors.WaitCursor;
					DataValid.SetInvalid(InvalidType.Prefs);
					Cursor=Cursors.Default;
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textRegKey.Text!="" 
				&& !Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")
				&& !Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{16}$"))
			{
				MsgBox.Show(this,"Invalid registration key format.");
				return;
			}
			if(textMultiple.Text.Contains(" ")) {
				MsgBox.Show(this,"No spaces allowed in the database list.");
				return;
			}
			string regkey="";
			if(Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")){
				regkey=textRegKey.Text.Substring(0,4)+textRegKey.Text.Substring(5,4)
					+textRegKey.Text.Substring(10,4)+textRegKey.Text.Substring(15,4);
			}
			else if(Regex.IsMatch(textRegKey.Text,@"^[A-Z0-9]{16}$")){
				regkey=textRegKey.Text;
			}
			bool refreshCache=false;
			if(Prefs.UpdateString(PrefName.UpdateServerAddress,textUpdateServerAddress.Text)
				| Prefs.UpdateBool(PrefName.UpdateShowMsiButtons,checkShowMsi.Checked)
				| Prefs.UpdateString(PrefName.UpdateWebsitePath,textWebsitePath.Text)
				| Prefs.UpdateString(PrefName.UpdateWebProxyAddress,textWebProxyAddress.Text)
				| Prefs.UpdateString(PrefName.UpdateWebProxyUserName,textWebProxyUserName.Text)
				| Prefs.UpdateString(PrefName.UpdateWebProxyPassword,textWebProxyPassword.Text)
				| Prefs.UpdateString(PrefName.UpdateMultipleDatabases,textMultiple.Text)) 
			{
				refreshCache=true;
			}
			if(Prefs.UpdateString(PrefName.RegistrationKey,regkey)) {
				FormOpenDental.IsRegKeyForTesting=PrefL.IsRegKeyForTesting();
				refreshCache=true;
			}
			if(refreshCache) {
				Cursor=Cursors.WaitCursor;
				DataValid.SetInvalid(InvalidType.Prefs);
				Cursor=Cursors.Default;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormUpdateSetup_FormClosing(object sender,FormClosingEventArgs e) {
			Permissions perm=(DialogResult==DialogResult.OK ? Permissions.SecurityAdmin : Permissions.Setup);
			SecurityLogs.MakeLogEntry(perm,0,"Update Setup window accesssed.");
		}
	}
}





















