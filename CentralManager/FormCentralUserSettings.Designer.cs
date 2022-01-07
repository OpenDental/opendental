namespace CentralManager {
	partial class FormCentralUserSettings {
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
			this.checkAutoLogin = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.labelAutoLogin = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// checkAutoLogin
			// 
			this.checkAutoLogin.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.checkAutoLogin.Location = new System.Drawing.Point(62, 50);
			this.checkAutoLogin.Name = "checkAutoLogin";
			this.checkAutoLogin.Size = new System.Drawing.Size(203, 18);
			this.checkAutoLogin.TabIndex = 18;
			this.checkAutoLogin.Text = "Log me in automatically";
			this.checkAutoLogin.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 18);
			this.label3.TabIndex = 16;
			this.label3.Text = "User";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(62, 24);
			this.textUserName.Name = "textUserName";
			this.textUserName.ReadOnly = true;
			this.textUserName.Size = new System.Drawing.Size(203, 20);
			this.textUserName.TabIndex = 15;
			// 
			// labelAutoLogin
			// 
			this.labelAutoLogin.Location = new System.Drawing.Point(59, 69);
			this.labelAutoLogin.Name = "labelAutoLogin";
			this.labelAutoLogin.Size = new System.Drawing.Size(206, 32);
			this.labelAutoLogin.TabIndex = 19;
			this.labelAutoLogin.Text = "Enable automatic log in by checking box \"Log me in automatically\" at next log in." +
    "";
			this.labelAutoLogin.Visible = false;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(126, 107);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(207, 107);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormCentralUserSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(294, 145);
			this.Controls.Add(this.labelAutoLogin);
			this.Controls.Add(this.checkAutoLogin);
			this.Controls.Add(this.textUserName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCentralUserSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "User Settings";
			this.Load += new System.EventHandler(this.FormCentralUserSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkAutoLogin;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textUserName;
		private System.Windows.Forms.Label labelAutoLogin;
	}
}