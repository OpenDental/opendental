using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormLogOn : FormODBase {
		///<summary>Used when temporarily switching users. Currently only used when overriding signed notes.
		///The user will not be logged on (Security.CurUser is untouched) but CurUserSimpleSwitch will be set to the desired user.</summary>
		private bool _isSimpleSwitch;
		///<summary>Gets set to the user that just successfully logged in when in Simple Switch mode.</summary>
		public Userod CurUserSimpleSwitch;
		///<summary>If set AND available, this will be the user automatically selected when the form opens.</summary>
		private string _userNameAutoSelect;
		///<summary>This form will not always be able to directly refresh the userod cache (Security) so it will be up to calling methods.</summary>
		private bool _refreshSecurityCache=false;
		///<summary>Set to true when the calling method has indicated that it will take care of any Security cache refreshing.</summary>
		private bool _doRefreshSecurityCache=false;

		///<summary>Will be true when the calling method needs to refresh the security cache themselves due to changes.</summary>
		public bool RefreshSecurityCache {
			get {
				return _refreshSecurityCache;
			}
		}

		///<summary>Set userNumSelected to automatically select the corresponding user in the list (if available).  Set isSimpleSwitch true if temporarily switching users for some reason.  This will leave Security.CurUser alone and will instead indicate which user was chosen / successfully logged in via CurUserSimpleSwitch.</summary>
		public FormLogOn(long userNumSelected=0,bool isSimpleSwitch=false,bool doRefreshSecurityCache=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Plugins.HookAddCode(this,"FormLogOn.InitializeComponent_end");
			Lan.F(this);
			if(userNumSelected > 0) {
				_userNameAutoSelect=Userods.GetUserNameNoCache(userNumSelected);
			}
			else if(Security.CurUser!=null) {
				_userNameAutoSelect=Security.CurUser.UserName;
			}
			_isSimpleSwitch=isSimpleSwitch;
			_doRefreshSecurityCache=doRefreshSecurityCache;
		}

		private void FormLogOn_Load(object sender,EventArgs e) {
			TextBox textSelectOnLoad=textPassword;
			if(PrefC.GetBool(PrefName.UserNameManualEntry)) {
				listUser.Visible=false;
				labelFilterName.Visible=false;
				textFilterName.Visible=false;
				textUser.Visible=true;
				textSelectOnLoad=textUser;//Focus should start with user name text box.
			}
			else {//Show a list of users.
				//Only show the show CEMT user check box if not manually typing user names and there are CEMT users present in the db.
				checkShowCEMTUsers.Visible=Userods.HasUsersForCEMTNoCache();
			}
			if(ODBuild.IsWeb()) {
				timerShutdownInstance.Enabled=true;
			}
			FillListBox();
			this.Focus();//Attempted fix, customers had issue with UI not defaulting focus to this form on startup.
			textSelectOnLoad.Select();//Give focus to appropriate text box.
			Plugins.HookAddCode(this,"FormLogOn.Load_end",_isSimpleSwitch);
		}

		private void listUser_MouseUp(object sender,MouseEventArgs e) {
			textPassword.Focus();
		}

		///<summary>Fills the User list with non-hidden, non-CEMT user names.  Only shows non-hidden CEMT users if Show CEMT users is checked.</summary>
		private void FillListBox() {
			listUser.Items.Clear();
			List<string> listUserNames=Userods.GetUserNamesNoCache(checkShowCEMTUsers.Checked);
			for(int i=0;i<listUserNames.Count;i++) {
				if(textFilterName.Text!="" && !listUserNames[i].ToLower().StartsWith(textFilterName.Text.Trim().ToLower())) {
					continue;
				}
				listUser.Items.Add(listUserNames[i]);
				if(_userNameAutoSelect!=null && _userNameAutoSelect.Trim().ToLower()==listUserNames[i].Trim().ToLower()) {
					listUser.SelectedIndex=listUser.Items.Count-1;
				}
			}
			if(listUser.SelectedIndex==-1 && listUser.Items.Count>0){//It is possible there are no users in the list if all users are CEMT users.
				listUser.SelectedIndex=0;
			}
		}

		private void checkShowCEMTUsers_CheckedChanged(object sender,EventArgs e) {
			FillListBox();
		}

		private void textFilterName_TextChanged(object sender,EventArgs e) {
			FillListBox();
		}

		private void timerShutdownInstance_Tick(object sender,EventArgs e) {
			//If the timer ticks that means IsWeb() is enabled and the user has sat at the login window too long. We want this to behave exactly like
			//the user clicked Exit to shut down the software.
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool isEcw=Programs.UsingEcwTightOrFullMode();
			string userName="";
			if(PrefC.GetBool(PrefName.UserNameManualEntry)) {
				//Check the user name using ToLower and Trim because Open Dental is case insensitive and does not allow white-space in regards to user names.
				userName=listUser.Items.GetAll<string>().FirstOrDefault(x => x.Trim().ToLower()==textUser.Text.Trim().ToLower());
			}
			else {
				userName=listUser.SelectedItem?.ToString();
			}
			if(string.IsNullOrEmpty(userName)) {
				MsgBox.Show(this,"Login failed");
				return;
			}
			string passwordTyped=textPassword.Text;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb && string.IsNullOrEmpty(passwordTyped)) {
				MsgBox.Show(this,"When using the web service, not allowed to log in with no password.  A password should be added for this user.");
				return;
			}
			Userod userCur=null;
			if(isEcw) {//ecw requires hash, but non-ecw requires actual password
				passwordTyped=Authentication.HashPasswordMD5(passwordTyped,true);
			}
			if(userName=="Stay Open" && _isSimpleSwitch && PrefC.IsODHQ) {
				// No need to check password when changing task users at HQ to user "Stay Open".
				userCur=Userods.GetUserByNameNoCache(userName);
			}
			else {//Not HQ (most common scenario)
				//Middle Tier sessions should not fire the CheckUserAndPasswordFailed exception code in FormLogOn.
				//That event would cause a second login window to pop with strange behavior.
				//Invoke the overload for CheckUserAndPassword that does not throw exceptions and give the user a generic error message if necessary.
				if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
					userCur=Userods.CheckUserAndPassword(userName,passwordTyped,isEcw,false);
					if(userCur==null) {
						MsgBox.Show("Userods","Invalid username, password, or the account has been locked due to failed log in attempts.");
						return;
					}
				}
				else {//Directly connected to the database.  This code will give a more accurate error message to the user when failing to log in.
					try {
						userCur=Userods.CheckUserAndPassword(userName,passwordTyped,isEcw);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			//successful login.
			if(_isSimpleSwitch) {
				CurUserSimpleSwitch=userCur;
			}
			else {//Not a temporary login.
				Security.CurUser=userCur;//Need to set for SecurityL.ChangePassword and calls.
				if(PrefC.GetBool(PrefName.PasswordsMustBeStrong) && PrefC.GetBool(PrefName.PasswordsWeakChangeToStrong)){
					if(Userods.IsPasswordStrong(passwordTyped)!="") {//Password is not strong
						MsgBox.Show(this,"You must change your password to a strong password due to the current Security settings.");
						if(!SecurityL.ChangePassword(true,_doRefreshSecurityCache)) {
							return;//Failed password update.
						}
						_refreshSecurityCache=true;//Indicate to calling method that they should manually refresh the Security cache.
					}
				}
				Security.IsUserLoggedIn=true;
				//Jason approved always storing the cleartext password that the user typed in 
				//since this is necessary for Reporting Servers over middle tier and was already happening when a user logged in over middle tier.
				Security.PasswordTyped=passwordTyped;
				SecurityLogs.MakeLogEntry(Permissions.UserLogOnOff,0,Lan.g(this,"User:")+" "+Security.CurUser.UserName+" "+Lan.g(this,"has logged on."));
			}
			Plugins.HookAddCode(this,"FormLogOn.butOK_Click_end");
			DialogResult=DialogResult.OK;
		}

		private void butExit_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}