namespace OpenDental{
	partial class FormSecureEmailSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSecureEmailSetup));
			this.groupSetup = new OpenDental.UI.GroupBox();
			this.butAddresses = new OpenDental.UI.Button();
			this.labelSenders = new System.Windows.Forms.Label();
			this.butSignatures = new OpenDental.UI.Button();
			this.labelSignatures = new System.Windows.Forms.Label();
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.gridClinics = new OpenDental.UI.GridOD();
			this.checkEnabled = new OpenDental.UI.CheckBox();
			this.comboPlatform = new OpenDental.UI.ComboBox();
			this.labelPlatform = new System.Windows.Forms.Label();
			this.butSignup = new OpenDental.UI.Button();
			this.labelStatements = new System.Windows.Forms.Label();
			this.comboStatements = new OpenDental.UI.ComboBox();
			this.groupSetup.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupSetup
			// 
			this.groupSetup.Controls.Add(this.butAddresses);
			this.groupSetup.Controls.Add(this.labelSenders);
			this.groupSetup.Controls.Add(this.butSignatures);
			this.groupSetup.Controls.Add(this.labelSignatures);
			this.groupSetup.Location = new System.Drawing.Point(12, 6);
			this.groupSetup.Name = "groupSetup";
			this.groupSetup.Size = new System.Drawing.Size(444, 86);
			this.groupSetup.TabIndex = 134;
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
			this.labelSenders.Size = new System.Drawing.Size(325, 18);
			this.labelSenders.TabIndex = 137;
			this.labelSenders.Text = "Addresses used when sending secure email notifications";
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
			this.labelSignatures.Text = "Signatures for secure email notifications";
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
			this.gridClinics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridClinics.HasAutoWrappedHeaders = true;
			this.gridClinics.HasMultilineHeaders = true;
			this.gridClinics.Location = new System.Drawing.Point(12, 98);
			this.gridClinics.Name = "gridClinics";
			this.gridClinics.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridClinics.Size = new System.Drawing.Size(444, 586);
			this.gridClinics.TabIndex = 137;
			this.gridClinics.CellSelectionCommitted += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_CellSelectionCommitted);
			this.gridClinics.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClinics_CellClick);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(355, 98);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(96, 21);
			this.checkEnabled.TabIndex = 138;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.Click += new System.EventHandler(this.checkEnabled_Click);
			// 
			// comboPlatform
			// 
			this.comboPlatform.Location = new System.Drawing.Point(330, 125);
			this.comboPlatform.Name = "comboPlatform";
			this.comboPlatform.Size = new System.Drawing.Size(121, 21);
			this.comboPlatform.TabIndex = 139;
			this.comboPlatform.Text = "comboPlatform";
			this.comboPlatform.SelectionChangeCommitted += new System.EventHandler(this.comboPlatform_SelectionChangeCommitted);
			// 
			// labelPlatform
			// 
			this.labelPlatform.Location = new System.Drawing.Point(161, 125);
			this.labelPlatform.Name = "labelPlatform";
			this.labelPlatform.Size = new System.Drawing.Size(165, 23);
			this.labelPlatform.TabIndex = 140;
			this.labelPlatform.Text = "Default Email Platform";
			this.labelPlatform.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSignup
			// 
			this.butSignup.Location = new System.Drawing.Point(389, 98);
			this.butSignup.Name = "butSignup";
			this.butSignup.Size = new System.Drawing.Size(62, 24);
			this.butSignup.TabIndex = 138;
			this.butSignup.Text = "Sign up";
			this.butSignup.Click += new System.EventHandler(this.butSignup_Click);
			// 
			// labelStatements
			// 
			this.labelStatements.Location = new System.Drawing.Point(161, 157);
			this.labelStatements.Name = "labelStatements";
			this.labelStatements.Size = new System.Drawing.Size(165, 23);
			this.labelStatements.TabIndex = 141;
			this.labelStatements.Text = "Statement Send";
			this.labelStatements.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatements
			// 
			this.comboStatements.Location = new System.Drawing.Point(330, 159);
			this.comboStatements.Name = "comboStatements";
			this.comboStatements.Size = new System.Drawing.Size(121, 21);
			this.comboStatements.TabIndex = 142;
			this.comboStatements.Text = "comboBox1";
			this.comboStatements.SelectionChangeCommitted += new System.EventHandler(this.comboStatements_SelectionChangeCommitted);
			// 
			// FormSecureEmailSetup
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.comboStatements);
			this.Controls.Add(this.labelStatements);
			this.Controls.Add(this.butSignup);
			this.Controls.Add(this.labelPlatform);
			this.Controls.Add(this.comboPlatform);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.gridClinics);
			this.Controls.Add(this.webBrowser);
			this.Controls.Add(this.groupSetup);
			this.DoubleBuffered = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSecureEmailSetup";
			this.Text = "Secure Email Setup";
			this.Load += new System.EventHandler(this.FormMassEmailSetup_Load);
			this.groupSetup.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.GroupBox groupSetup;
		private UI.Button butAddresses;
		private System.Windows.Forms.Label labelSenders;
		private UI.Button butSignatures;
		private System.Windows.Forms.Label labelSignatures;
		private System.Windows.Forms.WebBrowser webBrowser;
		private UI.GridOD gridClinics;
		private OpenDental.UI.CheckBox checkEnabled;
		private UI.ComboBox comboPlatform;
		private System.Windows.Forms.Label labelPlatform;
		private UI.Button butSignup;
		private System.Windows.Forms.Label labelStatements;
		private UI.ComboBox comboStatements;
	}
}