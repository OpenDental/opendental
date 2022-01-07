using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBase;
using DataConnectionBase;

namespace MySQLManager {
	public partial class FormMySQLUserManager:Form {
		///<summary>The current connection to the database.</summary>
		private DataConnection _dataConnection;

		public FormMySQLUserManager() {
			InitializeComponent();
		}

		private void FormMySQLUserManager_Load(object sender,EventArgs e) {
			SetUsersEnabled(false);
		}

		private void SetUsersEnabled(bool isEnabled) {
			listBoxUsers.Enabled=isEnabled;
			butAdd.Enabled=isEnabled;
			butEdit.Enabled=isEnabled;
			butDrop.Enabled=isEnabled;
		}

		private void butConnect_Click(object sender,EventArgs e) {
			if(textUser.Text=="") {
				MessageBox.Show("Please enter a MySQL user.");
				return;
			}
			if(!ConnectToDatabase(textUser.Text,textPassword.Text)) {
				return;
			}
			FillUsers();
		}

		private bool ConnectToDatabase(string userName,string password) {
			int portNum=3306;//This is the default port for MySQL.
			if(textPort.Text!="" && !Int32.TryParse(textPort.Text,out portNum)) {
				MessageBox.Show("Please enter a valid number for the Port.");
				return false;
			}
			string server=textServer.Text;
			if(portNum!=3306) {
				server+=":"+portNum;
			}
			try {
				_dataConnection=DbAdminMysql.ConnectAndTest(userName,password,server);
			}
			catch(Exception ex) {
				MessageBox.Show("Unable to connect to the database. Error message: "+ex.Message);
				return false;
			}
			SetUsersEnabled(true);
			return true;
		}

		private void FillUsers() {
			List<string> listUsers;
			try {
				listUsers=DbAdminMysql.GetUsers(_dataConnection)
					.FindAll(x => !ListTools.In(x,"mysql.sys","mysql.session","mariadb.sys","mariadb.session"));//These are reserved MySQL/MariaDB accounts.
			}
			catch(Exception ex) {
				MessageBox.Show("Error getting users: "+ex.Message);
				return;
			}
			listBoxUsers.Items.Clear();
			foreach(string user in listUsers) {
				listBoxUsers.Items.Add(user);
			}
		}

		private void listBoxUsers_DoubleClick(object sender,EventArgs e) {
			EditUser();
		}

		private void EditUser() {
			if(listBoxUsers.SelectedIndex==-1) {
				MessageBox.Show("Please select a user to edit.");
				return;
			}
			string user=(string)listBoxUsers.Items[listBoxUsers.SelectedIndex];
			using FormMySQLUserEdit formMySQLUserEdit=new FormMySQLUserEdit(user,false,_dataConnection);
			formMySQLUserEdit.ShowDialog();
			if(formMySQLUserEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(user.ToLower()==_dataConnection.UserCur.ToLower()) {
				SetUsersEnabled(false);//In case we can't reconnect.
				string password=formMySQLUserEdit.WasPasswordChanged ? formMySQLUserEdit.NewPassword : textPassword.Text;
				if(!ConnectToDatabase(formMySQLUserEdit.NewUserName,password)) {
					return;	
				}
			}
			FillUsers();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormMySQLUserEdit formMySQLUserEdit=new FormMySQLUserEdit("",true,_dataConnection);
			formMySQLUserEdit.ShowDialog();
			if(formMySQLUserEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FillUsers();
		}

		private void butEdit_Click(object sender,EventArgs e) {
			EditUser();
		}

		private void butDrop_Click(object sender,EventArgs e) {
			if(listBoxUsers.SelectedIndex==-1) {
				MessageBox.Show("Please select a user to drop.");
				return;
			}
			string user=(string)listBoxUsers.Items[listBoxUsers.SelectedIndex];
			if(user.ToLower()==_dataConnection.UserCur.ToLower()) {
				MessageBox.Show("Cannot drop the current MySQL user.");
				return;
			}
			if(MessageBox.Show($"Are you sure you want to drop '{user}'?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			try {
				DbAdminMysql.DropUser(_dataConnection,user);
			}
			catch(Exception ex) {
				MessageBox.Show("Error dropping user: "+ex.Message);
				return;
			}
			FillUsers();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}
