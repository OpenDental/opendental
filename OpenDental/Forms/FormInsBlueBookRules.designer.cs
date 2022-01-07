namespace OpenDental{
	partial class FormInsBlueBookRules {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsBlueBookRules));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkEnableAnonFeeShare = new System.Windows.Forms.CheckBox();
			this.checkUsePlanNumInHierarchy = new System.Windows.Forms.CheckBox();
			this.listAllowedFeeMethod = new OpenDental.UI.ListBoxOD();
			this.labelAllowedFeeMethod = new System.Windows.Forms.Label();
			this.labelBlueBookFeature = new System.Windows.Forms.Label();
			this.listBlueBookFeature = new OpenDental.UI.ListBoxOD();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.labelArrowButtons = new System.Windows.Forms.Label();
			this.textUcrFeePercent = new OpenDental.ValidNum();
			this.labelUcrFeePercent = new System.Windows.Forms.Label();
			this.groupBlueBookSettings = new System.Windows.Forms.GroupBox();
			this.gridInsBlueBookRules = new OpenDental.UI.GridOD();
			this.groupBlueBookSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(491, 365);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(573, 365);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.ButCancel_Click);
			// 
			// checkEnableAnonFeeShare
			// 
			this.checkEnableAnonFeeShare.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.checkEnableAnonFeeShare.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnableAnonFeeShare.Location = new System.Drawing.Point(259, 239);
			this.checkEnableAnonFeeShare.Name = "checkEnableAnonFeeShare";
			this.checkEnableAnonFeeShare.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEnableAnonFeeShare.Size = new System.Drawing.Size(210, 18);
			this.checkEnableAnonFeeShare.TabIndex = 260;
			this.checkEnableAnonFeeShare.Text = "Allow anonymous fee sharing";
			// 
			// checkUsePlanNumInHierarchy
			// 
			this.checkUsePlanNumInHierarchy.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.checkUsePlanNumInHierarchy.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePlanNumInHierarchy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUsePlanNumInHierarchy.Location = new System.Drawing.Point(10, 21);
			this.checkUsePlanNumInHierarchy.Name = "checkUsePlanNumInHierarchy";
			this.checkUsePlanNumInHierarchy.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkUsePlanNumInHierarchy.Size = new System.Drawing.Size(197, 18);
			this.checkUsePlanNumInHierarchy.TabIndex = 261;
			this.checkUsePlanNumInHierarchy.Text = "Use Insurance Plan in hierarchy";
			this.checkUsePlanNumInHierarchy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePlanNumInHierarchy.Click += new System.EventHandler(this.checkUsePlanNumInHierarchy_Click);
			// 
			// listAllowedFeeMethod
			// 
			this.listAllowedFeeMethod.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.listAllowedFeeMethod.Location = new System.Drawing.Point(10, 195);
			this.listAllowedFeeMethod.Name = "listAllowedFeeMethod";
			this.listAllowedFeeMethod.Size = new System.Drawing.Size(145, 56);
			this.listAllowedFeeMethod.TabIndex = 262;
			// 
			// labelAllowedFeeMethod
			// 
			this.labelAllowedFeeMethod.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelAllowedFeeMethod.Location = new System.Drawing.Point(10, 176);
			this.labelAllowedFeeMethod.Name = "labelAllowedFeeMethod";
			this.labelAllowedFeeMethod.Size = new System.Drawing.Size(145, 18);
			this.labelAllowedFeeMethod.TabIndex = 263;
			this.labelAllowedFeeMethod.Text = "*Allowed Fee Method";
			this.labelAllowedFeeMethod.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelBlueBookFeature
			// 
			this.labelBlueBookFeature.Location = new System.Drawing.Point(12, 9);
			this.labelBlueBookFeature.Name = "labelBlueBookFeature";
			this.labelBlueBookFeature.Size = new System.Drawing.Size(124, 18);
			this.labelBlueBookFeature.TabIndex = 265;
			this.labelBlueBookFeature.Text = "Blue Book Feature";
			this.labelBlueBookFeature.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBlueBookFeature
			// 
			this.listBlueBookFeature.Location = new System.Drawing.Point(12, 28);
			this.listBlueBookFeature.Name = "listBlueBookFeature";
			this.listBlueBookFeature.Size = new System.Drawing.Size(203, 56);
			this.listBlueBookFeature.TabIndex = 264;
			this.listBlueBookFeature.SelectedIndexChanged += new System.EventHandler(this.listBlueBookFeature_SelectedIndexChanged);
			// 
			// butDown
			// 
			this.butDown.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(475, 117);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 24);
			this.butDown.TabIndex = 269;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.ButDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(475, 85);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 24);
			this.butUp.TabIndex = 268;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.ButUp_Click);
			// 
			// labelArrowButtons
			// 
			this.labelArrowButtons.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelArrowButtons.Location = new System.Drawing.Point(475, 144);
			this.labelArrowButtons.Name = "labelArrowButtons";
			this.labelArrowButtons.Size = new System.Drawing.Size(157, 36);
			this.labelArrowButtons.TabIndex = 270;
			this.labelArrowButtons.Text = "Select a rule and use arrow\r\nbuttons to adjust priority.\r\n";
			// 
			// textUcrFeePercent
			// 
			this.textUcrFeePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.textUcrFeePercent.Location = new System.Drawing.Point(430, 201);
			this.textUcrFeePercent.MaxVal = 100;
			this.textUcrFeePercent.MinVal = 0;
			this.textUcrFeePercent.Name = "textUcrFeePercent";
			this.textUcrFeePercent.Size = new System.Drawing.Size(39, 20);
			this.textUcrFeePercent.TabIndex = 271;
			this.textUcrFeePercent.ShowZero = false;
			// 
			// labelUcrFeePercent
			// 
			this.labelUcrFeePercent.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelUcrFeePercent.Location = new System.Drawing.Point(306, 201);
			this.labelUcrFeePercent.Name = "labelUcrFeePercent";
			this.labelUcrFeePercent.Size = new System.Drawing.Size(123, 21);
			this.labelUcrFeePercent.TabIndex = 272;
			this.labelUcrFeePercent.Text = "UCR Fee Percent";
			this.labelUcrFeePercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBlueBookSettings
			// 
			this.groupBlueBookSettings.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.groupBlueBookSettings.Controls.Add(this.gridInsBlueBookRules);
			this.groupBlueBookSettings.Controls.Add(this.checkUsePlanNumInHierarchy);
			this.groupBlueBookSettings.Controls.Add(this.listAllowedFeeMethod);
			this.groupBlueBookSettings.Controls.Add(this.labelArrowButtons);
			this.groupBlueBookSettings.Controls.Add(this.textUcrFeePercent);
			this.groupBlueBookSettings.Controls.Add(this.checkEnableAnonFeeShare);
			this.groupBlueBookSettings.Controls.Add(this.butDown);
			this.groupBlueBookSettings.Controls.Add(this.butUp);
			this.groupBlueBookSettings.Controls.Add(this.labelAllowedFeeMethod);
			this.groupBlueBookSettings.Controls.Add(this.labelUcrFeePercent);
			this.groupBlueBookSettings.Location = new System.Drawing.Point(12, 90);
			this.groupBlueBookSettings.Name = "groupBlueBookSettings";
			this.groupBlueBookSettings.Size = new System.Drawing.Size(636, 268);
			this.groupBlueBookSettings.TabIndex = 273;
			this.groupBlueBookSettings.TabStop = false;
			this.groupBlueBookSettings.Text = "Blue Book Settings";
			// 
			// gridInsBlueBookRules
			// 
			this.gridInsBlueBookRules.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.gridInsBlueBookRules.Location = new System.Drawing.Point(10, 40);
			this.gridInsBlueBookRules.Name = "gridInsBlueBookRules";
			this.gridInsBlueBookRules.Size = new System.Drawing.Size(459, 135);
			this.gridInsBlueBookRules.TabIndex = 266;
			this.gridInsBlueBookRules.Title = "Insurance Blue Book Rules Hierarchy";
			this.gridInsBlueBookRules.VScrollVisible = false;
			this.gridInsBlueBookRules.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridInsBlueBookRules_CellDoubleClick);
			// 
			// FormInsBlueBookRules
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(660, 401);
			this.Controls.Add(this.groupBlueBookSettings);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelBlueBookFeature);
			this.Controls.Add(this.listBlueBookFeature);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsBlueBookRules";
			this.Text = "Insurance Blue Book Setup";
			this.Load += new System.EventHandler(this.FormInsBlueBookRules_Load);
			this.groupBlueBookSettings.ResumeLayout(false);
			this.groupBlueBookSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkEnableAnonFeeShare;
		private System.Windows.Forms.CheckBox checkUsePlanNumInHierarchy;
		private OpenDental.UI.ListBoxOD listAllowedFeeMethod;
		private System.Windows.Forms.Label labelAllowedFeeMethod;
		private System.Windows.Forms.Label labelBlueBookFeature;
		private OpenDental.UI.ListBoxOD listBlueBookFeature;
		private UI.GridOD gridInsBlueBookRules;
		private UI.Button butDown;
		private UI.Button butUp;
		private System.Windows.Forms.Label labelArrowButtons;
		private ValidNum textUcrFeePercent;
		private System.Windows.Forms.Label labelUcrFeePercent;
		private System.Windows.Forms.GroupBox groupBlueBookSettings;
	}
}