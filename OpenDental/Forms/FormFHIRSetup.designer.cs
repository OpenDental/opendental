namespace OpenDental{
	partial class FormFHIRSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFHIRSetup));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelPerm = new System.Windows.Forms.Label();
			this.listPermissions = new OpenDental.UI.ListBoxOD();
			this.butGenerateKey = new OpenDental.UI.Button();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textSubInterval = new OpenDental.ValidDouble();
			this.label1 = new System.Windows.Forms.Label();
			this.comboPayType = new OpenDental.UI.ComboBoxOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(694, 477);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 102);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(493, 355);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "API Keys";
			this.gridMain.TranslationName = "tableAPIKeys";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// labelPerm
			// 
			this.labelPerm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPerm.Location = new System.Drawing.Point(511, 35);
			this.labelPerm.Name = "labelPerm";
			this.labelPerm.Size = new System.Drawing.Size(236, 63);
			this.labelPerm.TabIndex = 6;
			this.labelPerm.Text = "Permissions for the key";
			this.labelPerm.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listPermissions
			// 
			this.listPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listPermissions.Location = new System.Drawing.Point(511, 102);
			this.listPermissions.Name = "listPermissions";
			this.listPermissions.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listPermissions.Size = new System.Drawing.Size(233, 355);
			this.listPermissions.TabIndex = 7;
			// 
			// butGenerateKey
			// 
			this.butGenerateKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGenerateKey.Location = new System.Drawing.Point(12, 461);
			this.butGenerateKey.Name = "butGenerateKey";
			this.butGenerateKey.Size = new System.Drawing.Size(79, 24);
			this.butGenerateKey.TabIndex = 8;
			this.butGenerateKey.Text = "&Add Key";
			this.butGenerateKey.Click += new System.EventHandler(this.butAddKey_Click);
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.Location = new System.Drawing.Point(223, 9);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(104, 20);
			this.checkEnabled.TabIndex = 14;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label3.Location = new System.Drawing.Point(67, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(243, 29);
			this.label3.TabIndex = 58;
			this.label3.Text = "Process subscription interval in minutes. Leave blank to disable subscriptions.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubInterval
			// 
			this.textSubInterval.Location = new System.Drawing.Point(313, 35);
			this.textSubInterval.MaxVal = 100000000D;
			this.textSubInterval.MinVal = 0D;
			this.textSubInterval.Name = "textSubInterval";
			this.textSubInterval.Size = new System.Drawing.Size(70, 20);
			this.textSubInterval.TabIndex = 59;
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(95, 68);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(215, 17);
			this.label1.TabIndex = 63;
			this.label1.Text = "Payment type for created payments";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPayType
			// 
			this.comboPayType.Location = new System.Drawing.Point(313, 65);
			this.comboPayType.Name = "comboPayType";
			this.comboPayType.Size = new System.Drawing.Size(136, 21);
			this.comboPayType.TabIndex = 62;
			// 
			// FormFHIRSetup
			// 
			this.ClientSize = new System.Drawing.Size(781, 513);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboPayType);
			this.Controls.Add(this.textSubInterval);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.butGenerateKey);
			this.Controls.Add(this.listPermissions);
			this.Controls.Add(this.labelPerm);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFHIRSetup";
			this.Text = "API Setup";
			this.Load += new System.EventHandler(this.FormFHIRSetup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelPerm;
		private UI.ListBoxOD listPermissions;
		private UI.Button butGenerateKey;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.Label label3;
		private ValidDouble textSubInterval;
		private System.Windows.Forms.Label label1;
		private UI.ComboBoxOD comboPayType;
	}
}