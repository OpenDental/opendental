namespace OpenDental{
	partial class FormEServicesMobileSettings {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesMobileSettings));
			this.butVerifyAndSave = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboBoxClinicPicker1 = new OpenDental.UI.ComboBoxClinicPicker();
			this.label1 = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.groupAccountRecovery = new System.Windows.Forms.GroupBox();
			this.textValidPhone = new OpenDental.ValidPhone();
			this.label5 = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textConfirmPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelConfirmPassword = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelPermissionRequired = new System.Windows.Forms.Label();
			this.groupAccountRecovery.SuspendLayout();
			this.SuspendLayout();
			// 
			// butVerifyAndSave
			// 
			this.butVerifyAndSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butVerifyAndSave.Location = new System.Drawing.Point(12, 275);
			this.butVerifyAndSave.Name = "butVerifyAndSave";
			this.butVerifyAndSave.Size = new System.Drawing.Size(109, 24);
			this.butVerifyAndSave.TabIndex = 3;
			this.butVerifyAndSave.Text = "Verify and Save";
			this.butVerifyAndSave.Click += new System.EventHandler(this.butVerifyAndSave_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(208, 275);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboBoxClinicPicker1
			// 
			this.comboBoxClinicPicker1.Location = new System.Drawing.Point(15, 12);
			this.comboBoxClinicPicker1.Name = "comboBoxClinicPicker1";
			this.comboBoxClinicPicker1.Size = new System.Drawing.Size(268, 21);
			this.comboBoxClinicPicker1.TabIndex = 4;
			this.comboBoxClinicPicker1.SelectionChangeCommitted += new System.EventHandler(this.comboBoxClinicPicker1_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "User Name";
			// 
			// textUserName
			// 
			this.textUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textUserName.Location = new System.Drawing.Point(127, 46);
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(156, 20);
			this.textUserName.TabIndex = 1;
			// 
			// groupAccountRecovery
			// 
			this.groupAccountRecovery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAccountRecovery.Controls.Add(this.label3);
			this.groupAccountRecovery.Controls.Add(this.textValidPhone);
			this.groupAccountRecovery.Controls.Add(this.label5);
			this.groupAccountRecovery.Controls.Add(this.textEmail);
			this.groupAccountRecovery.Controls.Add(this.label2);
			this.groupAccountRecovery.Location = new System.Drawing.Point(15, 124);
			this.groupAccountRecovery.Name = "groupAccountRecovery";
			this.groupAccountRecovery.Size = new System.Drawing.Size(268, 118);
			this.groupAccountRecovery.TabIndex = 14;
			this.groupAccountRecovery.TabStop = false;
			this.groupAccountRecovery.Text = "Account Recovery";
			// 
			// textValidPhone
			// 
			this.textValidPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textValidPhone.Location = new System.Drawing.Point(112, 85);
			this.textValidPhone.Name = "textValidPhone";
			this.textValidPhone.Size = new System.Drawing.Size(150, 20);
			this.textValidPhone.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "Phone Number";
			// 
			// textEmail
			// 
			this.textEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEmail.Location = new System.Drawing.Point(112, 59);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(150, 20);
			this.textEmail.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Email Address";
			// 
			// textPassword
			// 
			this.textPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPassword.Location = new System.Drawing.Point(127, 72);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(156, 20);
			this.textPassword.TabIndex = 2;
			// 
			// textConfirmPassword
			// 
			this.textConfirmPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textConfirmPassword.Location = new System.Drawing.Point(127, 98);
			this.textConfirmPassword.Name = "textConfirmPassword";
			this.textConfirmPassword.PasswordChar = '*';
			this.textConfirmPassword.Size = new System.Drawing.Size(156, 20);
			this.textConfirmPassword.TabIndex = 3;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(12, 75);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(95, 13);
			this.labelPassword.TabIndex = 17;
			this.labelPassword.Text = "Password";
			// 
			// labelConfirmPassword
			// 
			this.labelConfirmPassword.Location = new System.Drawing.Point(12, 101);
			this.labelConfirmPassword.Name = "labelConfirmPassword";
			this.labelConfirmPassword.Size = new System.Drawing.Size(109, 13);
			this.labelConfirmPassword.TabIndex = 18;
			this.labelConfirmPassword.Text = "Re-Enter Password";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(256, 38);
			this.label3.TabIndex = 15;
			this.label3.Text = "The below information is also used for verifying the user\'s identity when initial" +
	"ly registering.";
			// 
			// labelPermissionRequired
			// 
			this.labelPermissionRequired.Location = new System.Drawing.Point(12, 245);
			this.labelPermissionRequired.Name = "labelPermissionRequired";
			this.labelPermissionRequired.Size = new System.Drawing.Size(271, 27);
			this.labelPermissionRequired.TabIndex = 19;
			this.labelPermissionRequired.Text = "EServicesSetup permission is required to set up / modify mobile settings.";
			this.labelPermissionRequired.Visible = false;
			// 
			// FormEServicesMobileSettings
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(295, 311);
			this.Controls.Add(this.labelPermissionRequired);
			this.Controls.Add(this.textConfirmPassword);
			this.Controls.Add(this.labelConfirmPassword);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.groupAccountRecovery);
			this.Controls.Add(this.textUserName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxClinicPicker1);
			this.Controls.Add(this.butVerifyAndSave);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesMobileSettings";
			this.Text = "Mobile Settings";
			this.Load += new System.EventHandler(this.FormEServicesMobileSettings_Load);
			this.groupAccountRecovery.ResumeLayout(false);
			this.groupAccountRecovery.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butVerifyAndSave;
		private OpenDental.UI.Button butCancel;
		private UI.ComboBoxClinicPicker comboBoxClinicPicker1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textUserName;
		private System.Windows.Forms.GroupBox groupAccountRecovery;
		private ValidPhone textValidPhone;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.TextBox textConfirmPassword;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Label labelConfirmPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelPermissionRequired;
	}
}