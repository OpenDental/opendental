namespace CentralManager {
	partial class FormCentralUserPasswordEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.checkShow = new System.Windows.Forms.CheckBox();
			this.textCurrent = new System.Windows.Forms.TextBox();
			this.labelCurrent = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// checkShow
			// 
			this.checkShow.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.checkShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShow.Location = new System.Drawing.Point(65, 98);
			this.checkShow.Name = "checkShow";
			this.checkShow.Size = new System.Drawing.Size(104, 18);
			this.checkShow.TabIndex = 18;
			this.checkShow.Text = "Show";
			this.checkShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShow.UseVisualStyleBackColor = true;
			this.checkShow.Click += new System.EventHandler(this.checkShow_Click);
			// 
			// textCurrent
			// 
			this.textCurrent.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textCurrent.Location = new System.Drawing.Point(155, 42);
			this.textCurrent.Name = "textCurrent";
			this.textCurrent.PasswordChar = '*';
			this.textCurrent.Size = new System.Drawing.Size(203, 20);
			this.textCurrent.TabIndex = 10;
			// 
			// labelCurrent
			// 
			this.labelCurrent.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.labelCurrent.Location = new System.Drawing.Point(30, 43);
			this.labelCurrent.Name = "labelCurrent";
			this.labelCurrent.Size = new System.Drawing.Size(123, 18);
			this.labelCurrent.TabIndex = 17;
			this.labelCurrent.Text = "Current Password";
			this.labelCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUserName
			// 
			this.textUserName.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textUserName.Location = new System.Drawing.Point(155, 14);
			this.textUserName.Name = "textUserName";
			this.textUserName.ReadOnly = true;
			this.textUserName.Size = new System.Drawing.Size(203, 20);
			this.textUserName.TabIndex = 15;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label3.Location = new System.Drawing.Point(30, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(123, 18);
			this.label3.TabIndex = 16;
			this.label3.Text = "User";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textPassword.Location = new System.Drawing.Point(155, 70);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(203, 20);
			this.textPassword.TabIndex = 11;
			// 
			// butOK
			// 
			this.butOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butOK.Location = new System.Drawing.Point(255, 136);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.butCancel.Location = new System.Drawing.Point(336, 136);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label1.Location = new System.Drawing.Point(30, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 18);
			this.label1.TabIndex = 14;
			this.label1.Text = "New Password";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCentralUserPasswordEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(423, 174);
			this.Controls.Add(this.checkShow);
			this.Controls.Add(this.textCurrent);
			this.Controls.Add(this.labelCurrent);
			this.Controls.Add(this.textUserName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCentralUserPasswordEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Change Password";
			this.Load += new System.EventHandler(this.FormCentralUserPasswordEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkShow;
		private System.Windows.Forms.TextBox textCurrent;
		private System.Windows.Forms.Label labelCurrent;
		private System.Windows.Forms.TextBox textUserName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPassword;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
	}
}