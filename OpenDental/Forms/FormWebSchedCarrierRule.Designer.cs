namespace OpenDental{
	partial class FormWebSchedCarrierRule {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebSchedCarrierRule));
			this.butClose = new OpenDental.UI.Button();
			this.gridWebSchedCarrierRules = new OpenDental.UI.GridOD();
			this.labelCarriers = new System.Windows.Forms.Label();
			this.listBoxCarriers = new OpenDental.UI.ListBoxOD();
			this.checkNewPatRequestIns = new System.Windows.Forms.CheckBox();
			this.checkExistingPatRequestIns = new System.Windows.Forms.CheckBox();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butCopyRules = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelTitle = new System.Windows.Forms.Label();
			this.butSuggest = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(957, 473);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 9;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridWebSchedCarrierRules
			// 
			this.gridWebSchedCarrierRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridWebSchedCarrierRules.Location = new System.Drawing.Point(12, 58);
			this.gridWebSchedCarrierRules.Name = "gridWebSchedCarrierRules";
			this.gridWebSchedCarrierRules.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridWebSchedCarrierRules.Size = new System.Drawing.Size(775, 393);
			this.gridWebSchedCarrierRules.TabIndex = 3;
			this.gridWebSchedCarrierRules.Title = "Web Sched Carrier Rules";
			this.gridWebSchedCarrierRules.DoubleClick += new System.EventHandler(this.gridWebSchedCarrierRules_DoubleClick);
			// 
			// labelCarriers
			// 
			this.labelCarriers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCarriers.Location = new System.Drawing.Point(831, 41);
			this.labelCarriers.Name = "labelCarriers";
			this.labelCarriers.Size = new System.Drawing.Size(182, 14);
			this.labelCarriers.TabIndex = 409;
			this.labelCarriers.Text = "Insurance Carriers";
			this.labelCarriers.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxCarriers
			// 
			this.listBoxCarriers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxCarriers.Location = new System.Drawing.Point(834, 58);
			this.listBoxCarriers.Name = "listBoxCarriers";
			this.listBoxCarriers.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxCarriers.Size = new System.Drawing.Size(194, 394);
			this.listBoxCarriers.TabIndex = 6;
			this.listBoxCarriers.Text = "listBoxOD1";
			// 
			// checkNewPatRequestIns
			// 
			this.checkNewPatRequestIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNewPatRequestIns.Location = new System.Drawing.Point(12, 34);
			this.checkNewPatRequestIns.Name = "checkNewPatRequestIns";
			this.checkNewPatRequestIns.Size = new System.Drawing.Size(220, 18);
			this.checkNewPatRequestIns.TabIndex = 1;
			this.checkNewPatRequestIns.Text = "Enable For Web Sched New Patient";
			this.checkNewPatRequestIns.Click += new System.EventHandler(this.checkNewPatRequestIns_Click);
			// 
			// checkExistingPatRequestIns
			// 
			this.checkExistingPatRequestIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExistingPatRequestIns.Location = new System.Drawing.Point(238, 34);
			this.checkExistingPatRequestIns.Name = "checkExistingPatRequestIns";
			this.checkExistingPatRequestIns.Size = new System.Drawing.Size(240, 18);
			this.checkExistingPatRequestIns.TabIndex = 2;
			this.checkExistingPatRequestIns.Text = "Enable For Web Sched Existing Patient";
			this.checkExistingPatRequestIns.Click += new System.EventHandler(this.checkExistingPatRequestIns_Click);
			// 
			// butRight
			// 
			this.butRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(794, 225);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 26);
			this.butRight.TabIndex = 4;
			this.butRight.UseVisualStyleBackColor = true;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(793, 256);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 26);
			this.butLeft.TabIndex = 5;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butCopyRules
			// 
			this.butCopyRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopyRules.Location = new System.Drawing.Point(103, 457);
			this.butCopyRules.Name = "butCopyRules";
			this.butCopyRules.Size = new System.Drawing.Size(85, 24);
			this.butCopyRules.TabIndex = 7;
			this.butCopyRules.Text = "Copy Rules";
			this.butCopyRules.Click += new System.EventHandler(this.butCopyRules_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinics.Location = new System.Drawing.Point(830, 9);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(198, 21);
			this.comboClinics.TabIndex = 0;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.comboWebSchedClinic_SelectionChangeCommitted);
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(9, 5);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(778, 21);
			this.labelTitle.TabIndex = 413;
			this.labelTitle.Text = "Set up a list of insurance carriers for patients to choose from when scheduling o" +
    "nline appointments. Rules can be set up to allow or block specific carriers.";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butSuggest
			// 
			this.butSuggest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSuggest.Location = new System.Drawing.Point(12, 457);
			this.butSuggest.Name = "butSuggest";
			this.butSuggest.Size = new System.Drawing.Size(85, 24);
			this.butSuggest.TabIndex = 8;
			this.butSuggest.Text = "Suggest Rules";
			this.butSuggest.Click += new System.EventHandler(this.butSuggest_Click);
			// 
			// FormWebSchedCarrierRule
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1044, 509);
			this.Controls.Add(this.butSuggest);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.butCopyRules);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.gridWebSchedCarrierRules);
			this.Controls.Add(this.labelCarriers);
			this.Controls.Add(this.listBoxCarriers);
			this.Controls.Add(this.checkNewPatRequestIns);
			this.Controls.Add(this.checkExistingPatRequestIns);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebSchedCarrierRule";
			this.Text = "Web Sched Carrier Rule";
			this.Load += new System.EventHandler(this.FormWebSchedCarrierRule_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridWebSchedCarrierRules;
		private System.Windows.Forms.Label labelCarriers;
		private UI.ListBoxOD listBoxCarriers;
		private System.Windows.Forms.CheckBox checkNewPatRequestIns;
		private System.Windows.Forms.CheckBox checkExistingPatRequestIns;
		private UI.Button butRight;
		private UI.Button butLeft;
		private UI.Button butCopyRules;
		private UI.ComboBoxClinicPicker comboClinics;
		private System.Windows.Forms.Label labelTitle;
		private UI.Button butSuggest;
	}
}