namespace OpenDental{
	partial class FormMassEmailSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMassEmailSetup));
			this.butAnalytics = new OpenDental.UI.Button();
			this.butBirthdays = new OpenDental.UI.Button();
			this.labelAnalytics = new System.Windows.Forms.Label();
			this.labelBirthdays = new System.Windows.Forms.Label();
			this.groupSetup = new OpenDental.UI.GroupBoxOD();
			this.butTemplates = new OpenDental.UI.Button();
			this.labelTemplates = new System.Windows.Forms.Label();
			this.butAddresses = new OpenDental.UI.Button();
			this.labelSenders = new System.Windows.Forms.Label();
			this.butSignatures = new OpenDental.UI.Button();
			this.labelSignatures = new System.Windows.Forms.Label();
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.gridClinics = new OpenDental.UI.GridOD();
			this.butSignup = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.groupSetup.SuspendLayout();
			this.SuspendLayout();
			// 
			// butAnalytics
			// 
			this.butAnalytics.Location = new System.Drawing.Point(6, 147);
			this.butAnalytics.Name = "butAnalytics";
			this.butAnalytics.Size = new System.Drawing.Size(107, 24);
			this.butAnalytics.TabIndex = 3;
			this.butAnalytics.Text = "Analytics";
			this.butAnalytics.Click += new System.EventHandler(this.butAnalytics_Click);
			// 
			// butBirthdays
			// 
			this.butBirthdays.Location = new System.Drawing.Point(6, 115);
			this.butBirthdays.Name = "butBirthdays";
			this.butBirthdays.Size = new System.Drawing.Size(107, 24);
			this.butBirthdays.TabIndex = 5;
			this.butBirthdays.Text = "Birthday Messages";
			this.butBirthdays.Click += new System.EventHandler(this.butBirthdays_Click);
			// 
			// labelAnalytics
			// 
			this.labelAnalytics.Location = new System.Drawing.Point(116, 148);
			this.labelAnalytics.Name = "labelAnalytics";
			this.labelAnalytics.Size = new System.Drawing.Size(322, 18);
			this.labelAnalytics.TabIndex = 6;
			this.labelAnalytics.Text = "View data on sent mass emails.";
			this.labelAnalytics.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBirthdays
			// 
			this.labelBirthdays.Location = new System.Drawing.Point(116, 119);
			this.labelBirthdays.Name = "labelBirthdays";
			this.labelBirthdays.Size = new System.Drawing.Size(322, 18);
			this.labelBirthdays.TabIndex = 8;
			this.labelBirthdays.Text = "Setup automated messages for patient birthdays\r\n";
			this.labelBirthdays.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupSetup
			// 
			this.groupSetup.Controls.Add(this.butTemplates);
			this.groupSetup.Controls.Add(this.labelTemplates);
			this.groupSetup.Controls.Add(this.butAddresses);
			this.groupSetup.Controls.Add(this.labelSenders);
			this.groupSetup.Controls.Add(this.labelAnalytics);
			this.groupSetup.Controls.Add(this.butSignatures);
			this.groupSetup.Controls.Add(this.labelSignatures);
			this.groupSetup.Controls.Add(this.butAnalytics);
			this.groupSetup.Controls.Add(this.labelBirthdays);
			this.groupSetup.Controls.Add(this.butBirthdays);
			this.groupSetup.Location = new System.Drawing.Point(12, 6);
			this.groupSetup.Name = "groupSetup";
			this.groupSetup.Size = new System.Drawing.Size(444, 182);
			this.groupSetup.TabIndex = 134;
			this.groupSetup.Text = "Setup";
			// 
			// butTemplates
			// 
			this.butTemplates.Location = new System.Drawing.Point(6, 83);
			this.butTemplates.Name = "butTemplates";
			this.butTemplates.Size = new System.Drawing.Size(107, 24);
			this.butTemplates.TabIndex = 9;
			this.butTemplates.Text = "Templates";
			this.butTemplates.Click += new System.EventHandler(this.butTemplates_Click);
			// 
			// labelTemplates
			// 
			this.labelTemplates.Location = new System.Drawing.Point(116, 87);
			this.labelTemplates.Name = "labelTemplates";
			this.labelTemplates.Size = new System.Drawing.Size(322, 18);
			this.labelTemplates.TabIndex = 10;
			this.labelTemplates.Text = "Templates for mass email";
			this.labelTemplates.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.labelSenders.Size = new System.Drawing.Size(325, 18);
			this.labelSenders.TabIndex = 137;
			this.labelSenders.Text = "Addresses used when sending mass email";
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
			this.labelSignatures.Size = new System.Drawing.Size(325, 18);
			this.labelSignatures.TabIndex = 135;
			this.labelSignatures.Text = "Signatures for mass email";
			this.labelSignatures.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// webBrowser
			// 
			this.webBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowser.Location = new System.Drawing.Point(457, 0);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(772, 695);
			this.webBrowser.TabIndex = 10;
			// 
			// gridClinics
			// 
			this.gridClinics.AllowSortingByColumn = true;
			this.gridClinics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClinics.HasAutoWrappedHeaders = true;
			this.gridClinics.HasMultilineHeaders = true;
			this.gridClinics.Location = new System.Drawing.Point(12, 194);
			this.gridClinics.Name = "gridClinics";
			this.gridClinics.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridClinics.Size = new System.Drawing.Size(444, 490);
			this.gridClinics.TabIndex = 137;
			this.gridClinics.CellSelectionCommitted += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_CellSelectionCommitted);
			this.gridClinics.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_CellClick);
			// 
			// butSignup
			// 
			this.butSignup.Location = new System.Drawing.Point(391, 193);
			this.butSignup.Name = "butSignup";
			this.butSignup.Size = new System.Drawing.Size(62, 24);
			this.butSignup.TabIndex = 139;
			this.butSignup.Text = "Sign up";
			this.butSignup.Click += new System.EventHandler(this.butSignup_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.Location = new System.Drawing.Point(357, 196);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEnabled.Size = new System.Drawing.Size(96, 21);
			this.checkEnabled.TabIndex = 140;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.UseVisualStyleBackColor = true;
			this.checkEnabled.Click += new System.EventHandler(this.checkEnabled_Click);
			// 
			// FormMassEmailSetup
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.butSignup);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.gridClinics);
			this.Controls.Add(this.webBrowser);
			this.Controls.Add(this.groupSetup);
			this.DoubleBuffered = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMassEmailSetup";
			this.Text = "Advertising - Mass Email Setup";
			this.Load += new System.EventHandler(this.FormMassEmailSetup_Load);
			this.groupSetup.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butAnalytics;
		private UI.Button butBirthdays;
		private System.Windows.Forms.Label labelAnalytics;
		private System.Windows.Forms.Label labelBirthdays;
		private OpenDental.UI.GroupBoxOD groupSetup;
		private UI.Button butAddresses;
		private System.Windows.Forms.Label labelSenders;
		private UI.Button butSignatures;
		private System.Windows.Forms.Label labelSignatures;
		private System.Windows.Forms.WebBrowser webBrowser;
		private UI.GridOD gridClinics;
		private UI.Button butSignup;
		private System.Windows.Forms.CheckBox checkEnabled;
		private UI.Button butTemplates;
		private System.Windows.Forms.Label labelTemplates;
	}
}