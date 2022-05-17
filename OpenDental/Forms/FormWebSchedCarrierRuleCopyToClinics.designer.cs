namespace OpenDental{
	partial class FormWebSchedCarrierRuleCopyToClinics{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebSchedCarrierRuleCopyToClinics));
			this.butClose = new OpenDental.UI.Button();
			this.labelCopyFrom = new System.Windows.Forms.Label();
			this.listClinicsCopyTo = new OpenDental.UI.ListBoxOD();
			this.labelCopyTo = new System.Windows.Forms.Label();
			this.comboClinicsFrom = new OpenDental.UI.ComboBoxOD();
			this.butCopyRules = new OpenDental.UI.Button();
			this.butCopyToAll = new OpenDental.UI.Button();
			this.gridWebSchedCarrierRules = new OpenDental.UI.GridOD();
			this.labelGridRules = new System.Windows.Forms.Label();
			this.checkOverride = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(943, 453);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelCopyFrom
			// 
			this.labelCopyFrom.Location = new System.Drawing.Point(16, 15);
			this.labelCopyFrom.Name = "labelCopyFrom";
			this.labelCopyFrom.Size = new System.Drawing.Size(102, 17);
			this.labelCopyFrom.TabIndex = 11;
			this.labelCopyFrom.Text = "Copy From Clinic";
			// 
			// listClinicsCopyTo
			// 
			this.listClinicsCopyTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listClinicsCopyTo.Location = new System.Drawing.Point(19, 79);
			this.listClinicsCopyTo.Name = "listClinicsCopyTo";
			this.listClinicsCopyTo.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClinicsCopyTo.Size = new System.Drawing.Size(218, 368);
			this.listClinicsCopyTo.TabIndex = 2;
			this.listClinicsCopyTo.Text = "listClinicsCopyTo";
			// 
			// labelCopyTo
			// 
			this.labelCopyTo.Location = new System.Drawing.Point(16, 59);
			this.labelCopyTo.Name = "labelCopyTo";
			this.labelCopyTo.Size = new System.Drawing.Size(102, 17);
			this.labelCopyTo.TabIndex = 16;
			this.labelCopyTo.Text = "Copy To Clinic(s)";
			// 
			// comboClinicsFrom
			// 
			this.comboClinicsFrom.Location = new System.Drawing.Point(19, 35);
			this.comboClinicsFrom.Name = "comboClinicsFrom";
			this.comboClinicsFrom.Size = new System.Drawing.Size(218, 21);
			this.comboClinicsFrom.TabIndex = 1;
			this.comboClinicsFrom.Text = "comboClinicsFrom";
			this.comboClinicsFrom.SelectedIndexChanged += new System.EventHandler(this.comboClinicsFrom_SelectedIndexChanged);
			// 
			// butCopyRules
			// 
			this.butCopyRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopyRules.Location = new System.Drawing.Point(19, 453);
			this.butCopyRules.Name = "butCopyRules";
			this.butCopyRules.Size = new System.Drawing.Size(105, 24);
			this.butCopyRules.TabIndex = 3;
			this.butCopyRules.Text = "Copy Rules";
			this.butCopyRules.Click += new System.EventHandler(this.butCopyRules_Click);
			// 
			// butCopyToAll
			// 
			this.butCopyToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopyToAll.Location = new System.Drawing.Point(132, 453);
			this.butCopyToAll.Name = "butCopyToAll";
			this.butCopyToAll.Size = new System.Drawing.Size(105, 24);
			this.butCopyToAll.TabIndex = 4;
			this.butCopyToAll.Text = "Copy To All Clinics";
			this.butCopyToAll.Click += new System.EventHandler(this.butCopyToAll_Click);
			// 
			// gridWebSchedCarrierRules
			// 
			this.gridWebSchedCarrierRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridWebSchedCarrierRules.Location = new System.Drawing.Point(243, 35);
			this.gridWebSchedCarrierRules.Name = "gridWebSchedCarrierRules";
			this.gridWebSchedCarrierRules.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridWebSchedCarrierRules.Size = new System.Drawing.Size(775, 412);
			this.gridWebSchedCarrierRules.TabIndex = 5;
			this.gridWebSchedCarrierRules.Title = "Web Sched Carrier Rules";
			// 
			// labelGridRules
			// 
			this.labelGridRules.Location = new System.Drawing.Point(240, 15);
			this.labelGridRules.Name = "labelGridRules";
			this.labelGridRules.Size = new System.Drawing.Size(300, 17);
			this.labelGridRules.TabIndex = 18;
			this.labelGridRules.Text = "Select which rules to copy from the chosen clinic";
			// 
			// checkOverride
			// 
			this.checkOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkOverride.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.checkOverride.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOverride.Location = new System.Drawing.Point(243, 456);
			this.checkOverride.Name = "checkOverride";
			this.checkOverride.Size = new System.Drawing.Size(413, 18);
			this.checkOverride.TabIndex = 6;
			this.checkOverride.Text = "Delete all rules for \"Copy To\" clinics and replace with \"Copy From\"";
			// 
			// FormWebSchedCarrierRuleCopyToClinics
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1031, 486);
			this.Controls.Add(this.checkOverride);
			this.Controls.Add(this.labelGridRules);
			this.Controls.Add(this.gridWebSchedCarrierRules);
			this.Controls.Add(this.butCopyToAll);
			this.Controls.Add(this.butCopyRules);
			this.Controls.Add(this.labelCopyTo);
			this.Controls.Add(this.listClinicsCopyTo);
			this.Controls.Add(this.comboClinicsFrom);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelCopyFrom);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebSchedCarrierRuleCopyToClinics";
			this.Text = "Web Sched Carrier Rule Copy";
			this.Load += new System.EventHandler(this.FormWebSchedCarrierRuleCopyToClinics_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label labelCopyFrom;
		private UI.ListBoxOD listClinicsCopyTo;
		private System.Windows.Forms.Label labelCopyTo;
		private UI.ComboBoxOD comboClinicsFrom;
		private UI.Button butCopyRules;
		private UI.Button butCopyToAll;
		private UI.GridOD gridWebSchedCarrierRules;
		private System.Windows.Forms.Label labelGridRules;
		private System.Windows.Forms.CheckBox checkOverride;
	}
}