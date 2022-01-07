namespace OpenDental{
	partial class FormOrthoSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkPatClone = new System.Windows.Forms.CheckBox();
			this.checkApptModuleShowOrthoChartItem = new System.Windows.Forms.CheckBox();
			this.butOrthoDisplayFields = new OpenDental.UI.Button();
			this.checkOrthoFinancialInfoInChart = new System.Windows.Forms.CheckBox();
			this.checkOrthoClaimUseDatePlacement = new System.Windows.Forms.CheckBox();
			this.checkOrthoClaimMarkAsOrtho = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkOrthoEnabled = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textOrthoAutoProc = new System.Windows.Forms.TextBox();
			this.butPickOrthoProc = new OpenDental.UI.Button();
			this.checkConsolidateInsPayment = new System.Windows.Forms.CheckBox();
			this.textOrthoMonthsTreat = new OpenDental.ValidNum();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.butDelete = new OpenDental.UI.Button();
			this.listboxOrthoPlacementProcs = new OpenDental.UI.ListBoxOD();
			this.label29 = new System.Windows.Forms.Label();
			this.butPlacementProcsEdit = new OpenDental.UI.Button();
			this.groupOrthoCaseProcedures = new System.Windows.Forms.GroupBox();
			this.labelCommaSeparate = new System.Windows.Forms.Label();
			this.textDebondCodes = new System.Windows.Forms.TextBox();
			this.labelVisitCodes = new System.Windows.Forms.Label();
			this.textBandingCodes = new System.Windows.Forms.TextBox();
			this.labelDebondCodes = new System.Windows.Forms.Label();
			this.textVisitCodes = new System.Windows.Forms.TextBox();
			this.labelBandingCodes = new System.Windows.Forms.Label();
			this.groupBox4.SuspendLayout();
			this.groupOrthoCaseProcedures.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(176, 496);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 11;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(257, 496);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkPatClone
			// 
			this.checkPatClone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatClone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatClone.Location = new System.Drawing.Point(6, 156);
			this.checkPatClone.Name = "checkPatClone";
			this.checkPatClone.Size = new System.Drawing.Size(327, 19);
			this.checkPatClone.TabIndex = 6;
			this.checkPatClone.Text = "Patient Clone";
			this.checkPatClone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkApptModuleShowOrthoChartItem
			// 
			this.checkApptModuleShowOrthoChartItem.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleShowOrthoChartItem.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkApptModuleShowOrthoChartItem.Location = new System.Drawing.Point(6, 134);
			this.checkApptModuleShowOrthoChartItem.Name = "checkApptModuleShowOrthoChartItem";
			this.checkApptModuleShowOrthoChartItem.Size = new System.Drawing.Size(327, 17);
			this.checkApptModuleShowOrthoChartItem.TabIndex = 5;
			this.checkApptModuleShowOrthoChartItem.Text = "Show Ortho Chart in appointment options";
			this.checkApptModuleShowOrthoChartItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOrthoDisplayFields
			// 
			this.butOrthoDisplayFields.Location = new System.Drawing.Point(258, 231);
			this.butOrthoDisplayFields.Name = "butOrthoDisplayFields";
			this.butOrthoDisplayFields.Size = new System.Drawing.Size(75, 24);
			this.butOrthoDisplayFields.TabIndex = 10;
			this.butOrthoDisplayFields.Text = "Edit";
			this.butOrthoDisplayFields.Click += new System.EventHandler(this.butOrthoDisplayFields_Click);
			// 
			// checkOrthoFinancialInfoInChart
			// 
			this.checkOrthoFinancialInfoInChart.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrthoFinancialInfoInChart.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOrthoFinancialInfoInChart.Location = new System.Drawing.Point(6, 39);
			this.checkOrthoFinancialInfoInChart.Name = "checkOrthoFinancialInfoInChart";
			this.checkOrthoFinancialInfoInChart.Size = new System.Drawing.Size(327, 17);
			this.checkOrthoFinancialInfoInChart.TabIndex = 1;
			this.checkOrthoFinancialInfoInChart.Text = "Show Auto Ortho information in the ortho chart";
			this.checkOrthoFinancialInfoInChart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkOrthoClaimUseDatePlacement
			// 
			this.checkOrthoClaimUseDatePlacement.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrthoClaimUseDatePlacement.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOrthoClaimUseDatePlacement.Location = new System.Drawing.Point(6, 85);
			this.checkOrthoClaimUseDatePlacement.Name = "checkOrthoClaimUseDatePlacement";
			this.checkOrthoClaimUseDatePlacement.Size = new System.Drawing.Size(327, 17);
			this.checkOrthoClaimUseDatePlacement.TabIndex = 3;
			this.checkOrthoClaimUseDatePlacement.Text = "Use the first ortho procedure date as Date of Placement";
			this.checkOrthoClaimUseDatePlacement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkOrthoClaimMarkAsOrtho
			// 
			this.checkOrthoClaimMarkAsOrtho.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrthoClaimMarkAsOrtho.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOrthoClaimMarkAsOrtho.Location = new System.Drawing.Point(6, 62);
			this.checkOrthoClaimMarkAsOrtho.Name = "checkOrthoClaimMarkAsOrtho";
			this.checkOrthoClaimMarkAsOrtho.Size = new System.Drawing.Size(327, 17);
			this.checkOrthoClaimMarkAsOrtho.TabIndex = 2;
			this.checkOrthoClaimMarkAsOrtho.Text = "Mark claims as Ortho if they have ortho procedures";
			this.checkOrthoClaimMarkAsOrtho.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 109);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(270, 17);
			this.label1.TabIndex = 234;
			this.label1.Text = "Default months treatment";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkOrthoEnabled
			// 
			this.checkOrthoEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrthoEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOrthoEnabled.Location = new System.Drawing.Point(6, 16);
			this.checkOrthoEnabled.Name = "checkOrthoEnabled";
			this.checkOrthoEnabled.Size = new System.Drawing.Size(327, 17);
			this.checkOrthoEnabled.TabIndex = 0;
			this.checkOrthoEnabled.Text = "Show Auto Ortho in account module";
			this.checkOrthoEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 235);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(245, 17);
			this.label2.TabIndex = 237;
			this.label2.Text = "Ortho Chart Display Fields:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 178);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(216, 20);
			this.label3.TabIndex = 238;
			this.label3.Text = "Default Ortho Auto Proc:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOrthoAutoProc
			// 
			this.textOrthoAutoProc.Location = new System.Drawing.Point(226, 178);
			this.textOrthoAutoProc.Name = "textOrthoAutoProc";
			this.textOrthoAutoProc.ReadOnly = true;
			this.textOrthoAutoProc.Size = new System.Drawing.Size(78, 20);
			this.textOrthoAutoProc.TabIndex = 7;
			// 
			// butPickOrthoProc
			// 
			this.butPickOrthoProc.Location = new System.Drawing.Point(310, 177);
			this.butPickOrthoProc.Name = "butPickOrthoProc";
			this.butPickOrthoProc.Size = new System.Drawing.Size(23, 21);
			this.butPickOrthoProc.TabIndex = 8;
			this.butPickOrthoProc.Text = "...";
			this.butPickOrthoProc.Click += new System.EventHandler(this.butPickOrthoProc_Click);
			// 
			// checkConsolidateInsPayment
			// 
			this.checkConsolidateInsPayment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkConsolidateInsPayment.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkConsolidateInsPayment.Location = new System.Drawing.Point(6, 204);
			this.checkConsolidateInsPayment.Name = "checkConsolidateInsPayment";
			this.checkConsolidateInsPayment.Size = new System.Drawing.Size(327, 19);
			this.checkConsolidateInsPayment.TabIndex = 9;
			this.checkConsolidateInsPayment.Text = "Consolidate Ortho Insurance Payments";
			this.checkConsolidateInsPayment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOrthoMonthsTreat
			// 
			this.textOrthoMonthsTreat.Location = new System.Drawing.Point(282, 108);
			this.textOrthoMonthsTreat.Name = "textOrthoMonthsTreat";
			this.textOrthoMonthsTreat.ShowZero = false;
			this.textOrthoMonthsTreat.Size = new System.Drawing.Size(51, 20);
			this.textOrthoMonthsTreat.TabIndex = 4;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.butDelete);
			this.groupBox4.Controls.Add(this.listboxOrthoPlacementProcs);
			this.groupBox4.Controls.Add(this.label29);
			this.groupBox4.Controls.Add(this.butPlacementProcsEdit);
			this.groupBox4.Location = new System.Drawing.Point(19, 262);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(314, 104);
			this.groupBox4.TabIndex = 240;
			this.groupBox4.Text = "Ortho Placement Procedures";
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(143, 73);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(72, 24);
			this.butDelete.TabIndex = 12;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// listboxOrthoPlacementProcs
			// 
			this.listboxOrthoPlacementProcs.IntegralHeight = false;
			this.listboxOrthoPlacementProcs.Location = new System.Drawing.Point(216, 13);
			this.listboxOrthoPlacementProcs.Name = "listboxOrthoPlacementProcs";
			this.listboxOrthoPlacementProcs.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listboxOrthoPlacementProcs.Size = new System.Drawing.Size(93, 84);
			this.listboxOrthoPlacementProcs.TabIndex = 197;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(6, 14);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(209, 31);
			this.label29.TabIndex = 223;
			this.label29.Text = "Current Procedures to Consider for Patients\' Date Placement";
			this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPlacementProcsEdit
			// 
			this.butPlacementProcsEdit.Icon = OpenDental.UI.EnumIcons.Add;
			this.butPlacementProcsEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPlacementProcsEdit.Location = new System.Drawing.Point(143, 47);
			this.butPlacementProcsEdit.Name = "butPlacementProcsEdit";
			this.butPlacementProcsEdit.Size = new System.Drawing.Size(72, 24);
			this.butPlacementProcsEdit.TabIndex = 11;
			this.butPlacementProcsEdit.Text = "Add";
			this.butPlacementProcsEdit.Click += new System.EventHandler(this.butPlacementProcsEdit_Click);
			// 
			// groupOrthoCaseProcedures
			// 
			this.groupOrthoCaseProcedures.Controls.Add(this.labelCommaSeparate);
			this.groupOrthoCaseProcedures.Controls.Add(this.textDebondCodes);
			this.groupOrthoCaseProcedures.Controls.Add(this.labelVisitCodes);
			this.groupOrthoCaseProcedures.Controls.Add(this.textBandingCodes);
			this.groupOrthoCaseProcedures.Controls.Add(this.labelDebondCodes);
			this.groupOrthoCaseProcedures.Controls.Add(this.textVisitCodes);
			this.groupOrthoCaseProcedures.Controls.Add(this.labelBandingCodes);
			this.groupOrthoCaseProcedures.Location = new System.Drawing.Point(19, 370);
			this.groupOrthoCaseProcedures.Name = "groupOrthoCaseProcedures";
			this.groupOrthoCaseProcedures.Size = new System.Drawing.Size(314, 109);
			this.groupOrthoCaseProcedures.TabIndex = 241;
			this.groupOrthoCaseProcedures.Text = "Ortho Case Procedures";
			// 
			// labelCommaSeparate
			// 
			this.labelCommaSeparate.Location = new System.Drawing.Point(66, 16);
			this.labelCommaSeparate.Name = "labelCommaSeparate";
			this.labelCommaSeparate.Size = new System.Drawing.Size(246, 17);
			this.labelCommaSeparate.TabIndex = 248;
			this.labelCommaSeparate.Text = "(all codes should be comma separated)";
			this.labelCommaSeparate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelCommaSeparate.LocationChanged += new System.EventHandler(this.labelCommaSeparate_LocationChanged);
			// 
			// textDebondCodes
			// 
			this.textDebondCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDebondCodes.Location = new System.Drawing.Point(135, 83);
			this.textDebondCodes.Name = "textDebondCodes";
			this.textDebondCodes.Size = new System.Drawing.Size(173, 20);
			this.textDebondCodes.TabIndex = 15;
			// 
			// labelVisitCodes
			// 
			this.labelVisitCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelVisitCodes.Location = new System.Drawing.Point(2, 60);
			this.labelVisitCodes.Name = "labelVisitCodes";
			this.labelVisitCodes.Size = new System.Drawing.Size(132, 17);
			this.labelVisitCodes.TabIndex = 229;
			this.labelVisitCodes.Text = "Visit Codes";
			this.labelVisitCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBandingCodes
			// 
			this.textBandingCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBandingCodes.Location = new System.Drawing.Point(135, 37);
			this.textBandingCodes.Name = "textBandingCodes";
			this.textBandingCodes.Size = new System.Drawing.Size(173, 20);
			this.textBandingCodes.TabIndex = 13;
			// 
			// labelDebondCodes
			// 
			this.labelDebondCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDebondCodes.Location = new System.Drawing.Point(2, 83);
			this.labelDebondCodes.Name = "labelDebondCodes";
			this.labelDebondCodes.Size = new System.Drawing.Size(132, 17);
			this.labelDebondCodes.TabIndex = 228;
			this.labelDebondCodes.Text = "Debond Codes";
			this.labelDebondCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVisitCodes
			// 
			this.textVisitCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textVisitCodes.Location = new System.Drawing.Point(135, 60);
			this.textVisitCodes.Name = "textVisitCodes";
			this.textVisitCodes.Size = new System.Drawing.Size(173, 20);
			this.textVisitCodes.TabIndex = 14;
			// 
			// labelBandingCodes
			// 
			this.labelBandingCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelBandingCodes.Location = new System.Drawing.Point(2, 37);
			this.labelBandingCodes.Name = "labelBandingCodes";
			this.labelBandingCodes.Size = new System.Drawing.Size(132, 17);
			this.labelBandingCodes.TabIndex = 227;
			this.labelBandingCodes.Text = "Banding Codes";
			this.labelBandingCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormOrthoSetup
			// 
			this.ClientSize = new System.Drawing.Size(344, 532);
			this.Controls.Add(this.groupOrthoCaseProcedures);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.textOrthoMonthsTreat);
			this.Controls.Add(this.checkConsolidateInsPayment);
			this.Controls.Add(this.butPickOrthoProc);
			this.Controls.Add(this.textOrthoAutoProc);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkOrthoEnabled);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkOrthoClaimUseDatePlacement);
			this.Controls.Add(this.checkOrthoClaimMarkAsOrtho);
			this.Controls.Add(this.checkOrthoFinancialInfoInChart);
			this.Controls.Add(this.butOrthoDisplayFields);
			this.Controls.Add(this.checkApptModuleShowOrthoChartItem);
			this.Controls.Add(this.checkPatClone);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoSetup";
			this.Text = "Ortho Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOrthoSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormOrthoSetup_Load);
			this.groupBox4.ResumeLayout(false);
			this.groupOrthoCaseProcedures.ResumeLayout(false);
			this.groupOrthoCaseProcedures.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkPatClone;
		private System.Windows.Forms.CheckBox checkApptModuleShowOrthoChartItem;
		private UI.Button butOrthoDisplayFields;
		private System.Windows.Forms.CheckBox checkOrthoFinancialInfoInChart;
		private System.Windows.Forms.CheckBox checkOrthoClaimUseDatePlacement;
		private System.Windows.Forms.CheckBox checkOrthoClaimMarkAsOrtho;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkOrthoEnabled;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textOrthoAutoProc;
		private UI.Button butPickOrthoProc;
		private System.Windows.Forms.CheckBox checkConsolidateInsPayment;
		private ValidNum textOrthoMonthsTreat;
		private System.Windows.Forms.GroupBox groupBox4;
		private UI.ListBoxOD listboxOrthoPlacementProcs;
		private System.Windows.Forms.Label label29;
		private UI.Button butPlacementProcsEdit;
		private UI.Button butDelete;
		private System.Windows.Forms.GroupBox groupOrthoCaseProcedures;
		private System.Windows.Forms.Label labelCommaSeparate;
		private System.Windows.Forms.TextBox textDebondCodes;
		private System.Windows.Forms.Label labelVisitCodes;
		private System.Windows.Forms.TextBox textBandingCodes;
		private System.Windows.Forms.Label labelDebondCodes;
		private System.Windows.Forms.TextBox textVisitCodes;
		private System.Windows.Forms.Label labelBandingCodes;
	}
}