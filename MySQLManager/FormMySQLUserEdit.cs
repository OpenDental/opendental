using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;

namespace MySQLManager {
	public partial class FormMySQLUserEdit:Form {
		///<summary>The MySQL username.</summary>
		private string _userName;
		///<summary>True if this is a new user.</summary>
		private bool _isNew;
		///<summary>An established connection for a user that has admin privileges.</summary>
		private DataConnection _dataConnection;
		///<summary>True if the user has all permissions when the form loads.</summary>
		private bool _hasFullPermission;
		///<summary>Will be true if the user's permissions does not fit our "Full" or "Low" categories.</summary>
		private bool _hasNonStandardPermissions;
		///<summary>True if the user only exists for host 'localhost'.</summary>
		private bool _hasLocalhostUserOnly;
		///<summary>The new username.</summary>
		public string NewUserName => textUser.Text;
		///<summary>The new password. May not be the actual password if WasPasswordChanged is false.</summary>
		public string NewPassword => textPassword.Text;
		///<summary>True if the password was changed.</summary>
		public bool WasPasswordChanged { get; private set; }

		public FormMySQLUserEdit(string userName,bool isNew,DataConnection dataConnection) {
			InitializeComponent();
			_userName=userName;
			_isNew=isNew;
			_dataConnection=dataConnection;
		}

		private void FormMySQLUserManager_Load(object sender,EventArgs e) {
			textUser.Text=_userName;
			if(!_isNew) {
				textPassword.Text=new string('*',12);
				WasPasswordChanged=false;//Undo textPassword_TextChanged
			}
			SetPermissions();
		}

		private void SetPermissions() {
			if(_isNew) {
				return;
			}
			List<string> listGrants;
			try {
				listGrants=DbAdminMysql.ShowGrants(_dataConnection,_userName);
			}
			catch(Exception ex) {
				try {
					//Sometimes a user will only exist for the localhost.
					listGrants=DbAdminMysql.ShowGrants(_dataConnection,_userName,"localhost");
					_hasLocalhostUserOnly=true;
				}
				catch(Exception e) {
					MessageBox.Show($"Unable to read permissions for user '{_userName}': "+e.Message);
					radioLow.Checked=true;
					return;
				}
				ex.DoNothing();
			}
			//MariaDB 10.5 uses ` instead of '. Replace all	` with ' so the logic works for both MySQL and MariaDB
			listGrants=listGrants.Select(x => x.Replace("`","'")).ToList();
			_hasFullPermission=listGrants.Any(x => x.StartsWith($"GRANT ALL PRIVILEGES ON *.* TO '{_userName}'@'") && x.EndsWith("WITH GRANT OPTION"));
			foreach(string grant in listGrants) {
				if(grant.Contains($" ON *.* TO '{_userName}'@'")) {
					//The root user is typically granted all 27 permissions explicitly. This also should be considered full permissions.
					int countPrivileges=StringTools.SubstringBefore(grant.Substring("GRANT ".Length),$" ON *.* TO '{_userName}'@'").Split(',').Length;
					if(countPrivileges>=27 && grant.EndsWith("WITH GRANT OPTION")) {
						_hasFullPermission=true;
					}
				}
			}
			if(_hasFullPermission) {
				radioFull.Checked=true;
			}
			else if(listGrants.Any(x => x==$"GRANT SELECT ON *.* TO '{_userName}'@'%'")) {
				radioLow.Checked=true;
			}
			else {//This user has neither the 'Full' nor 'Low' permission set.
				radioLow.Checked=true;//We will check this radio button but warn the user before we overwrite their permissions.
				_hasNonStandardPermissions=true;
			}
		}

		private void textPassword_TextChanged(object sender,EventArgs e) {
			WasPasswordChanged=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textUser.Text=="" && (_isNew || _userName!="")) {//If the existing username is '', we will allow it.
				MessageBox.Show("Please enter a MySQL user.");
				return;
			}
			if(textPassword.Text=="") {
				MessageBox.Show("Please enter a password.");
				return;
			}
			if(WasPasswordChanged && textPassword.Text!=textRetypePassword.Text) {
				MessageBox.Show("Passwords do not match.");
				return;
			}
			if(textUser.Text!=_userName && !WasPasswordChanged) {
				MessageBox.Show("Please enter a password if changing the username.");
				return;
			}
			if(_hasNonStandardPermissions && radioLow.Checked && MessageBox.Show("This user has more permissions than the 'Low' permissions set. Continuing"
				+" will reduce the user's permission to just the SELECT permission. Do you want to continue?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) 
			{
				return;
			}
			if(_userName.ToLower()==_dataConnection.UserCur.ToLower() && radioLow.Checked) {
				MessageBox.Show("Cannot change the current user to low permission.");
				return;
			}
			try {
				bool doCreateNew=_isNew;
				if(textUser.Text!=_userName) {//It doesn't really work to update a username, so we'll drop and recreate.
					DbAdminMysql.DropUser(_dataConnection,_userName);
					doCreateNew=true;
				}
				if(doCreateNew) {
					DbAdminMysql.CreateUser(_dataConnection,textUser.Text,_hasLocalhostUserOnly);
				}
				if(WasPasswordChanged || doCreateNew) {
					DbAdminMysql.SetPassword(_dataConnection,textUser.Text,textPassword.Text);
				}
				if(_hasFullPermission!=radioFull.Checked || _hasNonStandardPermissions || doCreateNew) {
					DbAdminMysql.GrantToUser(_dataConnection,textUser.Text,radioFull.Checked);
				}
			}
			catch(Exception ex) {
				MessageBox.Show("Error updating user: "+ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
