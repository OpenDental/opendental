using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormUserPassword {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserPassword));
			this.labelNew = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textCurrent = new System.Windows.Forms.TextBox();
			this.labelCurrent = new System.Windows.Forms.Label();
			this.checkShow = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelNew
			// 
			this.labelNew.Location = new System.Drawing.Point(13, 80);
			this.labelNew.Name = "labelNew";
			this.labelNew.Size = new System.Drawing.Size(157, 18);
			this.labelNew.TabIndex = 2;
			this.labelNew.Text = "New Password";
			this.labelNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(172, 79);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(203, 20);
			this.textPassword.TabIndex = 1;
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(172, 23);
			this.textUserName.Name = "textUserName";
			this.textUserName.ReadOnly = true;
			this.textUserName.Size = new System.Drawing.Size(203, 20);
			this.textUserName.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(13, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(157, 18);
			this.label3.TabIndex = 6;
			this.label3.Text = "User";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCurrent
			// 
			this.textCurrent.Location = new System.Drawing.Point(172, 51);
			this.textCurrent.Name = "textCurrent";
			this.textCurrent.PasswordChar = '*';
			this.textCurrent.Size = new System.Drawing.Size(203, 20);
			this.textCurrent.TabIndex = 0;
			// 
			// labelCurrent
			// 
			this.labelCurrent.Location = new System.Drawing.Point(13, 52);
			this.labelCurrent.Name = "labelCurrent";
			this.labelCurrent.Size = new System.Drawing.Size(157, 18);
			this.labelCurrent.TabIndex = 8;
			this.labelCurrent.Text = "Current Password";
			this.labelCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShow
			// 
			this.checkShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShow.Location = new System.Drawing.Point(82, 107);
			this.checkShow.Name = "checkShow";
			this.checkShow.Size = new System.Drawing.Size(104, 18);
			this.checkShow.TabIndex = 9;
			this.checkShow.Text = "Show";
			this.checkShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShow.UseVisualStyleBackColor = true;
			this.checkShow.Click += new System.EventHandler(this.checkShow_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(264, 158);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(357, 158);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormUserPassword
			// 
			this.ClientSize = new System.Drawing.Size(484, 209);
			this.Controls.Add(this.checkShow);
			this.Controls.Add(this.textCurrent);
			this.Controls.Add(this.labelCurrent);
			this.Controls.Add(this.textUserName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelNew);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUserPassword";
			this.ShowInTaskbar = false;
			this.Text = "Change Password";
			this.Load += new System.EventHandler(this.FormUserPassword_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelNew;
		private System.Windows.Forms.TextBox textPassword;
		private TextBox textUserName;
		private Label label3;
		private TextBox textCurrent;
		private Label labelCurrent;
		private CheckBox checkShow;
	}
}
