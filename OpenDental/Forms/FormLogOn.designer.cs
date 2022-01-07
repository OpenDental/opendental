using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental {
	partial class FormLogOn {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogOn));
			this.butExit = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.listUser = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.checkShowCEMTUsers = new System.Windows.Forms.CheckBox();
			this.timerShutdownInstance = new System.Windows.Forms.Timer(this.components);
			this.labelFilterName = new System.Windows.Forms.Label();
			this.textFilterName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butExit
			// 
			this.butExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butExit.Location = new System.Drawing.Point(445, 329);
			this.butExit.Name = "butExit";
			this.butExit.Size = new System.Drawing.Size(75, 26);
			this.butExit.TabIndex = 2;
			this.butExit.Text = "Exit";
			this.butExit.Click += new System.EventHandler(this.butExit_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(445, 288);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listUser
			// 
			this.listUser.Location = new System.Drawing.Point(74, 38);
			this.listUser.Name = "listUser";
			this.listUser.Size = new System.Drawing.Size(141, 316);
			this.listUser.TabIndex = 2;
			this.listUser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listUser_MouseUp);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "User";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(222, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(291, 38);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(215, 20);
			this.textPassword.TabIndex = 0;
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(74, 38);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(141, 20);
			this.textUser.TabIndex = 8;
			this.textUser.Visible = false;
			// 
			// checkShowCEMTUsers
			// 
			this.checkShowCEMTUsers.Location = new System.Drawing.Point(291, 64);
			this.checkShowCEMTUsers.Name = "checkShowCEMTUsers";
			this.checkShowCEMTUsers.Size = new System.Drawing.Size(213, 24);
			this.checkShowCEMTUsers.TabIndex = 9;
			this.checkShowCEMTUsers.Text = "Show CEMT users";
			this.checkShowCEMTUsers.UseVisualStyleBackColor = true;
			this.checkShowCEMTUsers.Visible = false;
			this.checkShowCEMTUsers.CheckedChanged += new System.EventHandler(this.checkShowCEMTUsers_CheckedChanged);
			// 
			// timerShutdownInstance
			// 
			this.timerShutdownInstance.Interval = 300000;
			this.timerShutdownInstance.Tick += new System.EventHandler(this.timerShutdownInstance_Tick);
			// 
			// labelFilterName
			// 
			this.labelFilterName.Location = new System.Drawing.Point(22, 13);
			this.labelFilterName.Name = "labelFilterName";
			this.labelFilterName.Size = new System.Drawing.Size(51, 18);
			this.labelFilterName.TabIndex = 26;
			this.labelFilterName.Text = "Filter";
			this.labelFilterName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFilterName
			// 
			this.textFilterName.Location = new System.Drawing.Point(74, 12);
			this.textFilterName.Name = "textFilterName";
			this.textFilterName.Size = new System.Drawing.Size(141, 20);
			this.textFilterName.TabIndex = 27;
			this.textFilterName.TextChanged += new System.EventHandler(this.textFilterName_TextChanged);
			// 
			// FormLogOn
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(532, 367);
			this.Controls.Add(this.textFilterName);
			this.Controls.Add(this.labelFilterName);
			this.Controls.Add(this.checkShowCEMTUsers);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butExit);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listUser);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormLogOn";
			this.Text = "Log On";
			this.Load += new System.EventHandler(this.FormLogOn_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butExit;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ListBoxOD listUser;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.CheckBox checkShowCEMTUsers;
		private System.Windows.Forms.Timer timerShutdownInstance;
		private System.Windows.Forms.Label labelFilterName;
		private System.Windows.Forms.TextBox textFilterName;
	}
}
