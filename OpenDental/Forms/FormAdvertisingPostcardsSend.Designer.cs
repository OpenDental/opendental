namespace OpenDental{
	partial class FormAdvertisingPostcardsSend {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdvertisingPostcardsSend));
			this.butUploadPatients = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textNumberOfRecipients = new System.Windows.Forms.TextBox();
			this.labelRecipients = new System.Windows.Forms.Label();
			this.butSelectPatients = new OpenDental.UI.Button();
			this.textListName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboPostcardAccount = new OpenDental.UI.ComboBoxOD();
			this.butViewAccount = new OpenDental.UI.Button();
			this.menuAccountSetup = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// butUploadPatients
			// 
			this.butUploadPatients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUploadPatients.Location = new System.Drawing.Point(208, 179);
			this.butUploadPatients.Name = "butUploadPatients";
			this.butUploadPatients.Size = new System.Drawing.Size(98, 24);
			this.butUploadPatients.TabIndex = 3;
			this.butUploadPatients.Text = "&Upload List";
			this.butUploadPatients.Click += new System.EventHandler(this.butUploadPatients_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(312, 179);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(12, 32);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(266, 17);
			this.labelEmail.TabIndex = 4;
			this.labelEmail.Text = "Advertising - Postcards Account";
			// 
			// textNumberOfRecipients
			// 
			this.textNumberOfRecipients.Enabled = false;
			this.textNumberOfRecipients.Location = new System.Drawing.Point(15, 91);
			this.textNumberOfRecipients.Name = "textNumberOfRecipients";
			this.textNumberOfRecipients.Size = new System.Drawing.Size(160, 20);
			this.textNumberOfRecipients.TabIndex = 7;
			// 
			// labelRecipients
			// 
			this.labelRecipients.Location = new System.Drawing.Point(12, 75);
			this.labelRecipients.Name = "labelRecipients";
			this.labelRecipients.Size = new System.Drawing.Size(163, 17);
			this.labelRecipients.TabIndex = 6;
			this.labelRecipients.Text = "Number of recipients";
			// 
			// butSelectPatients
			// 
			this.butSelectPatients.Location = new System.Drawing.Point(181, 87);
			this.butSelectPatients.Name = "butSelectPatients";
			this.butSelectPatients.Size = new System.Drawing.Size(97, 24);
			this.butSelectPatients.TabIndex = 8;
			this.butSelectPatients.Text = "&Select Patients";
			this.butSelectPatients.Click += new System.EventHandler(this.butSelectPatients_Click);
			// 
			// textListName
			// 
			this.textListName.Location = new System.Drawing.Point(15, 137);
			this.textListName.Name = "textListName";
			this.textListName.Size = new System.Drawing.Size(160, 20);
			this.textListName.TabIndex = 10;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 117);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(163, 17);
			this.label1.TabIndex = 9;
			this.label1.Text = "List Name";
			// 
			// comboPostcardAccount
			// 
			this.comboPostcardAccount.Location = new System.Drawing.Point(15, 50);
			this.comboPostcardAccount.Name = "comboPostcardAccount";
			this.comboPostcardAccount.Size = new System.Drawing.Size(160, 21);
			this.comboPostcardAccount.TabIndex = 234;
			this.comboPostcardAccount.Text = "comboBoxOD1";
			// 
			// butViewAccount
			// 
			this.butViewAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butViewAccount.Location = new System.Drawing.Point(15, 179);
			this.butViewAccount.Name = "butViewAccount";
			this.butViewAccount.Size = new System.Drawing.Size(121, 24);
			this.butViewAccount.TabIndex = 235;
			this.butViewAccount.Text = "View Account Details";
			this.butViewAccount.Click += new System.EventHandler(this.butViewAccount_Click);
			// 
			// menuAccountSetup
			// 
			this.menuAccountSetup.Location = new System.Drawing.Point(0, 0);
			this.menuAccountSetup.Name = "menuAccountSetup";
			this.menuAccountSetup.Size = new System.Drawing.Size(399, 24);
			this.menuAccountSetup.TabIndex = 236;
			this.menuAccountSetup.Text = "Menu";
			// 
			// FormAdvertisingPostcardsSend
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(399, 215);
			this.Controls.Add(this.menuAccountSetup);
			this.Controls.Add(this.butViewAccount);
			this.Controls.Add(this.comboPostcardAccount);
			this.Controls.Add(this.textListName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSelectPatients);
			this.Controls.Add(this.textNumberOfRecipients);
			this.Controls.Add(this.labelRecipients);
			this.Controls.Add(this.labelEmail);
			this.Controls.Add(this.butUploadPatients);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAdvertisingPostcardsSend";
			this.Text = "Upload Postcard Recipients";
			this.Load += new System.EventHandler(this.FormMassPostcardSend_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butUploadPatients;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelEmail;
		private System.Windows.Forms.TextBox textNumberOfRecipients;
		private System.Windows.Forms.Label labelRecipients;
		private UI.Button butSelectPatients;
		private System.Windows.Forms.TextBox textListName;
		private System.Windows.Forms.Label label1;
		private UI.ComboBoxOD comboPostcardAccount;
		private UI.Button butViewAccount;
		private UI.MenuOD menuAccountSetup;
	}
}