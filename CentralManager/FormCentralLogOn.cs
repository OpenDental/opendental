using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace CentralManager{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormCentralLogOn : System.Windows.Forms.Form{
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.ListBox listUser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPassword;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private TextBox textUser;
		private List<Userod> _listUsers;
		public bool IsMiddleTierSync;

		///<summary></summary>
		public FormCentralLogOn()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralLogOn));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.listUser = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(361, 321);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Exit";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(361, 280);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listUser
			// 
			this.listUser.Location = new System.Drawing.Point(31, 31);
			this.listUser.Name = "listUser";
			this.listUser.Size = new System.Drawing.Size(141, 316);
			this.listUser.TabIndex = 2;
			this.listUser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listUser_MouseUp);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(30, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "User";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(188, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(122, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(189, 31);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(215, 20);
			this.textPassword.TabIndex = 0;
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(31, 31);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(141, 20);
			this.textUser.TabIndex = 8;
			this.textUser.Visible = false;
			// 
			// FormCentralLogOn
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(464, 378);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listUser);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormCentralLogOn";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Log On";
			this.Load += new System.EventHandler(this.FormLogOn_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormLogOn_Load(object sender, System.EventArgs e) {
			if(PrefC.GetBool(PrefName.UserNameManualEntry) || IsMiddleTierSync) {
				listUser.Visible=false;
				textUser.Visible=true;
				textUser.Select();//Give focus to the user name text box.
			}
			FillListBox();
		}

		private void listUser_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			textPassword.Focus();
		}

		private void FillListBox(){
			Userods.RefreshCache();
			UserGroups.RefreshCache();
			GroupPermissions.RefreshCache();
			listUser.BeginUpdate();
			listUser.Items.Clear();
			if(PrefC.GetBool(PrefName.UserNameManualEntry)) {
				//Because _listUsers is used to verify the user name typed in, we need to include both non-hidden and CEMT users for offices that type in their credentials instead of picking.
				_listUsers=Userods.GetUsers(true);
			}
			else {//This will be the most common way to fill the user list.  Includes CEMT users.
				_listUsers=Userods.GetUsers(true);
			}
			for(int i=0;i<_listUsers.Count;i++){
				listUser.Items.Add(_listUsers[i]);
			}
			listUser.SelectedIndex=0;
			listUser.EndUpdate();
		}

		private void checkShowCEMTUsers_CheckedChanged(object sender,EventArgs e) {
			FillListBox();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			Userod selectedUser=null;
			if(IsMiddleTierSync) {
				selectedUser=new Userod();
				selectedUser.UserName=textUser.Text;
				selectedUser.LoginDetails=Authentication.GenerateLoginDetails(textPassword.Text,HashTypes.SHA3_512);
				Security.CurUser=selectedUser;
				Security.PasswordTyped=textPassword.Text;
			}
			else { 
				if(PrefC.GetBool(PrefName.UserNameManualEntry)) {
					for(int i=0;i<listUser.Items.Count;i++) {
						//Check the user name typed in using ToLower and Trim because Open Dental is case insensitive and does not allow white-space in regards to user names.
						if(textUser.Text.Trim().ToLower()==listUser.Items[i].ToString().Trim().ToLower()) {
							selectedUser=(Userod)listUser.Items[i];//Found the typed username
							break;
						}
					}
					if(selectedUser==null) {
						MessageBox.Show(this,"Login failed");
						return;
					}
				}
				else {
					selectedUser=(Userod)listUser.SelectedItem;
				}
				try {
					Userods.CheckUserAndPassword(selectedUser.UserName,textPassword.Text,false);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				if(RemotingClient.RemotingRole==RemotingRole.ClientWeb && selectedUser.PasswordHash=="" && textPassword.Text=="") {
					MessageBox.Show(this,"When using the web service, not allowed to log in with no password.  A password should be added for this user.");
					return;
				}
				Security.CurUser=selectedUser.Copy();
				Security.PasswordTyped=textPassword.Text;
			}
			//if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){//Not sure we need this when connecting to CEMT, but not sure enough to delete.
			//	string password=textPassword.Text;
			//	if(Programs.UsingEcwTightOrFullMode()) {//ecw requires hash, but non-ecw requires actual password
			//		password=Userods.EncryptPassword(password,true);
			//	}
			//	Security.PasswordTyped=password;
			//}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		
		


	}

	


}





















