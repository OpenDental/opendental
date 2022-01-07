namespace OpenDental{
	partial class FormMassEmails {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmails));
			this.butAnalytics = new OpenDental.UI.Button();
			this.butMassEmail = new OpenDental.UI.Button();
			this.butBirthdays = new OpenDental.UI.Button();
			this.labelAnalytics = new System.Windows.Forms.Label();
			this.labelMassEmail = new System.Windows.Forms.Label();
			this.labelBirthdays = new System.Windows.Forms.Label();
			this.groupSetup = new System.Windows.Forms.GroupBox();
			this.butAddresses = new OpenDental.UI.Button();
			this.labelSenders = new System.Windows.Forms.Label();
			this.butSignatures = new OpenDental.UI.Button();
			this.labelSignatures = new System.Windows.Forms.Label();
			this.groupMassEmail = new System.Windows.Forms.GroupBox();
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.gridClinics = new OpenDental.UI.GridOD();
			this.groupSetup.SuspendLayout();
			this.groupMassEmail.SuspendLayout();
			this.SuspendLayout();
			// 
			// butAnalytics
			// 
			this.butAnalytics.Location = new System.Drawing.Point(6, 83);
			this.butAnalytics.Name = "butAnalytics";
			this.butAnalytics.Size = new System.Drawing.Size(107, 24);
			this.butAnalytics.TabIndex = 3;
			this.butAnalytics.Text = "Analytics";
			this.butAnalytics.Click += new System.EventHandler(this.butAnalytics_Click);
			// 
			// butMassEmail
			// 
			this.butMassEmail.Location = new System.Drawing.Point(6, 19);
			this.butMassEmail.Name = "butMassEmail";
			this.butMassEmail.Size = new System.Drawing.Size(107, 24);
			this.butMassEmail.TabIndex = 4;
			this.butMassEmail.Text = "Mass Email List";
			this.butMassEmail.Click += new System.EventHandler(this.butManualMassEmail_Click);
			// 
			// butBirthdays
			// 
			this.butBirthdays.Location = new System.Drawing.Point(6, 51);
			this.butBirthdays.Name = "butBirthdays";
			this.butBirthdays.Size = new System.Drawing.Size(107, 24);
			this.butBirthdays.TabIndex = 5;
			this.butBirthdays.Text = "Birthday Messages";
			this.butBirthdays.Click += new System.EventHandler(this.butBirthdays_Click);
			// 
			// labelAnalytics
			// 
			this.labelAnalytics.Location = new System.Drawing.Point(116, 84);
			this.labelAnalytics.Name = "labelAnalytics";
			this.labelAnalytics.Size = new System.Drawing.Size(439, 18);
			this.labelAnalytics.TabIndex = 6;
			this.labelAnalytics.Text = "View data on sent mass emails.";
			this.labelAnalytics.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMassEmail
			// 
			this.labelMassEmail.Location = new System.Drawing.Point(116, 22);
			this.labelMassEmail.Name = "labelMassEmail";
			this.labelMassEmail.Size = new System.Drawing.Size(439, 18);
			this.labelMassEmail.TabIndex = 7;
			this.labelMassEmail.Text = "Select patients and send emails from templates";
			this.labelMassEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBirthdays
			// 
			this.labelBirthdays.Location = new System.Drawing.Point(116, 55);
			this.labelBirthdays.Name = "labelBirthdays";
			this.labelBirthdays.Size = new System.Drawing.Size(439, 18);
			this.labelBirthdays.TabIndex = 8;
			this.labelBirthdays.Text = "Setup automated messages for patient birthdays\r\n";
			this.labelBirthdays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupSetup
			// 
			this.groupSetup.Controls.Add(this.butAddresses);
			this.groupSetup.Controls.Add(this.labelSenders);
			this.groupSetup.Controls.Add(this.butSignatures);
			this.groupSetup.Controls.Add(this.labelSignatures);
			this.groupSetup.Location = new System.Drawing.Point(12, 6);
			this.groupSetup.Name = "groupSetup";
			this.groupSetup.Size = new System.Drawing.Size(610, 86);
			this.groupSetup.TabIndex = 134;
			this.groupSetup.TabStop = false;
			this.groupSetup.Text = "Setup";
			// 
			// butAddresses
			// 
			this.butAddresses.Location = new System.Drawing.Point(6, 51);
			this.butAddresses.Name = "butAddresses";
			this.butAddresses.Size = new System.Drawing.Size(107, 24);
			this.butAddresses.TabIndex = 136;
			this.butAddresses.Text = "Sender Addresses";
			this.butAddresses.Click += new System.EventHandler(this.butAddresses_Click);
			// 
			// labelSenders
			// 
			this.labelSenders.Location = new System.Drawing.Point(116, 54);
			this.labelSenders.Name = "labelSenders";
			this.labelSenders.Size = new System.Drawing.Size(439, 18);
			this.labelSenders.TabIndex = 137;
			this.labelSenders.Text = "Addresses used when sending mass email and notifications";
			this.labelSenders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSignatures
			// 
			this.butSignatures.Location = new System.Drawing.Point(6, 19);
			this.butSignatures.Name = "butSignatures";
			this.butSignatures.Size = new System.Drawing.Size(107, 24);
			this.butSignatures.TabIndex = 134;
			this.butSignatures.Text = "Email Signatures";
			this.butSignatures.Click += new System.EventHandler(this.butSignatures_Click);
			// 
			// labelSignatures
			// 
			this.labelSignatures.Location = new System.Drawing.Point(116, 22);
			this.labelSignatures.Name = "labelSignatures";
			this.labelSignatures.Size = new System.Drawing.Size(439, 18);
			this.labelSignatures.TabIndex = 135;
			this.labelSignatures.Text = "Signatures for mass email and notifications";
			this.labelSignatures.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupMassEmail
			// 
			this.groupMassEmail.Controls.Add(this.labelAnalytics);
			this.groupMassEmail.Controls.Add(this.butAnalytics);
			this.groupMassEmail.Controls.Add(this.butMassEmail);
			this.groupMassEmail.Controls.Add(this.butBirthdays);
			this.groupMassEmail.Controls.Add(this.labelMassEmail);
			this.groupMassEmail.Controls.Add(this.labelBirthdays);
			this.groupMassEmail.Location = new System.Drawing.Point(12, 100);
			this.groupMassEmail.Name = "groupMassEmail";
			this.groupMassEmail.Size = new System.Drawing.Size(610, 119);
			this.groupMassEmail.TabIndex = 135;
			this.groupMassEmail.TabStop = false;
			this.groupMassEmail.Text = "Mass Email";
			// 
			// webBrowser
			// 
			this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser.Location = new System.Drawing.Point(1, 225);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(1230, 468);
			this.webBrowser.TabIndex = 10;
			// 
			// gridClinics
			// 
			this.gridClinics.AllowSortingByColumn = true;
			this.gridClinics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClinics.HasAutoWrappedHeaders = true;
			this.gridClinics.HasMultilineHeaders = true;
			this.gridClinics.Location = new System.Drawing.Point(628, 12);
			this.gridClinics.Name = "gridClinics";
			this.gridClinics.Size = new System.Drawing.Size(590, 207);
			this.gridClinics.TabIndex = 137;
			this.gridClinics.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_CellClick);
			this.gridClinics.CellSelectionCommitted += gridClinics_CellSelectionCommitted;
			this.gridClinics.SelectionMode=UI.GridSelectionMode.OneCell;
			// 
			// FormMassEmails
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.gridClinics);
			this.Controls.Add(this.webBrowser);
			this.Controls.Add(this.groupMassEmail);
			this.Controls.Add(this.groupSetup);
			this.DoubleBuffered = false;
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmails";
			this.Text = "Hosted Email";
			this.Load += new System.EventHandler(this.FormMassEmailSetup_Load);
			this.groupSetup.ResumeLayout(false);
			this.groupMassEmail.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butAnalytics;
		private UI.Button butMassEmail;
		private UI.Button butBirthdays;
		private System.Windows.Forms.Label labelAnalytics;
		private System.Windows.Forms.Label labelMassEmail;
		private System.Windows.Forms.Label labelBirthdays;
		private System.Windows.Forms.GroupBox groupSetup;
		private System.Windows.Forms.GroupBox groupMassEmail;
		private UI.Button butAddresses;
		private System.Windows.Forms.Label labelSenders;
		private UI.Button butSignatures;
		private System.Windows.Forms.Label labelSignatures;
		private System.Windows.Forms.WebBrowser webBrowser;
		private UI.GridOD gridClinics;
	}
}