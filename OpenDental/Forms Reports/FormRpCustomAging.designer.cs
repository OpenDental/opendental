namespace OpenDental {
	partial class FormRpCustomAging {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpCustomAging));
			this.butCancel = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.goToAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butPrint = new OpenDental.UI.Button();
			this.gridTotals = new OpenDental.UI.GridOD();
			this.checkExcludeBadAddresses = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.checkAllBillType = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.labelClinic = new System.Windows.Forms.Label();
			this.listBoxClins = new OpenDental.UI.ListBoxOD();
			this.listBoxProvs = new OpenDental.UI.ListBoxOD();
			this.listBoxBillTypes = new OpenDental.UI.ListBoxOD();
			this.checkAgeCredits = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioWriteoffClaimDate = new System.Windows.Forms.RadioButton();
			this.radioWriteoffInsPayDate = new System.Windows.Forms.RadioButton();
			this.radioWriteoffProcDate = new System.Windows.Forms.RadioButton();
			this.checkExcludeInactive = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupGroupBy = new System.Windows.Forms.GroupBox();
			this.radioGroupByPat = new System.Windows.Forms.RadioButton();
			this.radioGroupByFam = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioExcludeNeg = new System.Windows.Forms.RadioButton();
			this.radioIncludeNeg = new System.Windows.Forms.RadioButton();
			this.radioShowOnlyNeg = new System.Windows.Forms.RadioButton();
			this.textDate = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radio30 = new System.Windows.Forms.RadioButton();
			this.radio90 = new System.Windows.Forms.RadioButton();
			this.radio60 = new System.Windows.Forms.RadioButton();
			this.radioAny = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkAgePayPlanCredits = new System.Windows.Forms.CheckBox();
			this.checkAgeWriteoffEsts = new System.Windows.Forms.CheckBox();
			this.checkAgeProcedures = new System.Windows.Forms.CheckBox();
			this.checkAgePayPlanCharges = new System.Windows.Forms.CheckBox();
			this.checkAgePatPayments = new System.Windows.Forms.CheckBox();
			this.checkAgeWriteoffs = new System.Windows.Forms.CheckBox();
			this.checkAgeInsPayments = new System.Windows.Forms.CheckBox();
			this.checkAgeInsEsts = new System.Windows.Forms.CheckBox();
			this.checkAgeAdjustments = new System.Windows.Forms.CheckBox();
			this.checkExcludeArchive = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupGroupBy.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(966, 710);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butRefresh.Location = new System.Drawing.Point(721, 17);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 86;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.contextMenuStrip;
			this.gridMain.Location = new System.Drawing.Point(9, 17);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(703, 695);
			this.gridMain.TabIndex = 96;
			this.gridMain.Title = "Aging";
			this.gridMain.TranslationName = "TableCustomAging";
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToAccountToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(154, 26);
			// 
			// goToAccountToolStripMenuItem
			// 
			this.goToAccountToolStripMenuItem.Name = "goToAccountToolStripMenuItem";
			this.goToAccountToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.goToAccountToolStripMenuItem.Text = "Go To Account";
			this.goToAccountToolStripMenuItem.Click += new System.EventHandler(this.goToAccountToolStripMenuItem_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(718, 710);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 97;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// gridTotals
			// 
			this.gridTotals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridTotals.ContextMenuStrip = this.contextMenuStrip;
			this.gridTotals.HeadersVisible = false;
			this.gridTotals.Location = new System.Drawing.Point(9, 714);
			this.gridTotals.Name = "gridTotals";
			this.gridTotals.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridTotals.Size = new System.Drawing.Size(703, 20);
			this.gridTotals.TabIndex = 98;
			this.gridTotals.Title = "Aging";
			this.gridTotals.TitleVisible = false;
			this.gridTotals.TranslationName = "TableCustomAgingTotals";
			// 
			// checkExcludeBadAddresses
			// 
			this.checkExcludeBadAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkExcludeBadAddresses.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeBadAddresses.Location = new System.Drawing.Point(884, 654);
			this.checkExcludeBadAddresses.Name = "checkExcludeBadAddresses";
			this.checkExcludeBadAddresses.Size = new System.Drawing.Size(157, 18);
			this.checkExcludeBadAddresses.TabIndex = 117;
			this.checkExcludeBadAddresses.Text = "Exclude bad addresses";
			// 
			// checkAllClin
			// 
			this.checkAllClin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllClin.Checked = true;
			this.checkAllClin.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(884, 418);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(157, 18);
			this.checkAllClin.TabIndex = 116;
			this.checkAllClin.Text = "All";
			// 
			// checkAllProv
			// 
			this.checkAllProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(884, 231);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(157, 18);
			this.checkAllProv.TabIndex = 115;
			this.checkAllProv.Text = "All";
			// 
			// checkAllBillType
			// 
			this.checkAllBillType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllBillType.Checked = true;
			this.checkAllBillType.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllBillType.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllBillType.Location = new System.Drawing.Point(884, 44);
			this.checkAllBillType.Name = "checkAllBillType";
			this.checkAllBillType.Size = new System.Drawing.Size(157, 18);
			this.checkAllBillType.TabIndex = 114;
			this.checkAllBillType.Text = "All";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(881, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(139, 14);
			this.label3.TabIndex = 113;
			this.label3.Text = "Billing Types";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClinic
			// 
			this.labelClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClinic.Location = new System.Drawing.Point(881, 401);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(139, 14);
			this.labelClinic.TabIndex = 112;
			this.labelClinic.Text = "Clinics";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxClins
			// 
			this.listBoxClins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxClins.Location = new System.Drawing.Point(884, 438);
			this.listBoxClins.Name = "listBoxClins";
			this.listBoxClins.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxClins.Size = new System.Drawing.Size(157, 147);
			this.listBoxClins.TabIndex = 111;
			this.listBoxClins.Click += new System.EventHandler(this.listBoxClins_Click);
			// 
			// listBoxProvs
			// 
			this.listBoxProvs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxProvs.Location = new System.Drawing.Point(884, 251);
			this.listBoxProvs.Name = "listBoxProvs";
			this.listBoxProvs.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxProvs.Size = new System.Drawing.Size(157, 147);
			this.listBoxProvs.TabIndex = 110;
			this.listBoxProvs.Click += new System.EventHandler(this.listBoxProvs_Click);
			// 
			// listBoxBillTypes
			// 
			this.listBoxBillTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxBillTypes.Location = new System.Drawing.Point(884, 64);
			this.listBoxBillTypes.Name = "listBoxBillTypes";
			this.listBoxBillTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxBillTypes.Size = new System.Drawing.Size(157, 147);
			this.listBoxBillTypes.TabIndex = 109;
			this.listBoxBillTypes.Click += new System.EventHandler(this.listBoxBillTypes_Click);
			// 
			// checkAgeCredits
			// 
			this.checkAgeCredits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAgeCredits.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeCredits.Location = new System.Drawing.Point(884, 588);
			this.checkAgeCredits.Name = "checkAgeCredits";
			this.checkAgeCredits.Size = new System.Drawing.Size(157, 18);
			this.checkAgeCredits.TabIndex = 108;
			this.checkAgeCredits.Text = "Age Credits";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.radioWriteoffClaimDate);
			this.groupBox4.Controls.Add(this.radioWriteoffInsPayDate);
			this.groupBox4.Controls.Add(this.radioWriteoffProcDate);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox4.Location = new System.Drawing.Point(721, 535);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(157, 97);
			this.groupBox4.TabIndex = 103;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Age Write-offs by";
			// 
			// radioWriteoffClaimDate
			// 
			this.radioWriteoffClaimDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioWriteoffClaimDate.Location = new System.Drawing.Point(5, 61);
			this.radioWriteoffClaimDate.Name = "radioWriteoffClaimDate";
			this.radioWriteoffClaimDate.Size = new System.Drawing.Size(146, 30);
			this.radioWriteoffClaimDate.TabIndex = 2;
			this.radioWriteoffClaimDate.Text = "Initial claim date for est.\r\nIns pay date for adj.";
			this.radioWriteoffClaimDate.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffInsPayDate
			// 
			this.radioWriteoffInsPayDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioWriteoffInsPayDate.Location = new System.Drawing.Point(5, 19);
			this.radioWriteoffInsPayDate.Name = "radioWriteoffInsPayDate";
			this.radioWriteoffInsPayDate.Size = new System.Drawing.Size(146, 18);
			this.radioWriteoffInsPayDate.TabIndex = 1;
			this.radioWriteoffInsPayDate.Text = "Ins Payment Date";
			this.radioWriteoffInsPayDate.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffProcDate
			// 
			this.radioWriteoffProcDate.Checked = true;
			this.radioWriteoffProcDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioWriteoffProcDate.Location = new System.Drawing.Point(5, 41);
			this.radioWriteoffProcDate.Name = "radioWriteoffProcDate";
			this.radioWriteoffProcDate.Size = new System.Drawing.Size(146, 18);
			this.radioWriteoffProcDate.TabIndex = 0;
			this.radioWriteoffProcDate.TabStop = true;
			this.radioWriteoffProcDate.Text = "Procedure Date";
			this.radioWriteoffProcDate.UseVisualStyleBackColor = true;
			// 
			// checkExcludeInactive
			// 
			this.checkExcludeInactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkExcludeInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeInactive.Location = new System.Drawing.Point(884, 610);
			this.checkExcludeInactive.Name = "checkExcludeInactive";
			this.checkExcludeInactive.Size = new System.Drawing.Size(157, 18);
			this.checkExcludeInactive.TabIndex = 107;
			this.checkExcludeInactive.Text = "Exclude inactive patients";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(881, 214);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(139, 14);
			this.label4.TabIndex = 106;
			this.label4.Text = "Providers";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupGroupBy
			// 
			this.groupGroupBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupGroupBy.Controls.Add(this.radioGroupByPat);
			this.groupGroupBy.Controls.Add(this.radioGroupByFam);
			this.groupGroupBy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupGroupBy.Location = new System.Drawing.Point(721, 634);
			this.groupGroupBy.Name = "groupGroupBy";
			this.groupGroupBy.Size = new System.Drawing.Size(157, 70);
			this.groupGroupBy.TabIndex = 102;
			this.groupGroupBy.TabStop = false;
			this.groupGroupBy.Text = "Group By";
			// 
			// radioGroupByPat
			// 
			this.radioGroupByPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioGroupByPat.Location = new System.Drawing.Point(6, 41);
			this.radioGroupByPat.Name = "radioGroupByPat";
			this.radioGroupByPat.Size = new System.Drawing.Size(121, 18);
			this.radioGroupByPat.TabIndex = 1;
			this.radioGroupByPat.Text = "Individual";
			this.radioGroupByPat.UseVisualStyleBackColor = true;
			// 
			// radioGroupByFam
			// 
			this.radioGroupByFam.Checked = true;
			this.radioGroupByFam.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioGroupByFam.Location = new System.Drawing.Point(6, 19);
			this.radioGroupByFam.Name = "radioGroupByFam";
			this.radioGroupByFam.Size = new System.Drawing.Size(121, 18);
			this.radioGroupByFam.TabIndex = 0;
			this.radioGroupByFam.TabStop = true;
			this.radioGroupByFam.Text = "Family";
			this.radioGroupByFam.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.radioExcludeNeg);
			this.groupBox2.Controls.Add(this.radioIncludeNeg);
			this.groupBox2.Controls.Add(this.radioShowOnlyNeg);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(721, 329);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(157, 91);
			this.groupBox2.TabIndex = 105;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Negative Balances";
			// 
			// radioExcludeNeg
			// 
			this.radioExcludeNeg.Checked = true;
			this.radioExcludeNeg.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioExcludeNeg.Location = new System.Drawing.Point(6, 19);
			this.radioExcludeNeg.Name = "radioExcludeNeg";
			this.radioExcludeNeg.Size = new System.Drawing.Size(120, 18);
			this.radioExcludeNeg.TabIndex = 4;
			this.radioExcludeNeg.TabStop = true;
			this.radioExcludeNeg.Text = "Exclude";
			// 
			// radioIncludeNeg
			// 
			this.radioIncludeNeg.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioIncludeNeg.Location = new System.Drawing.Point(6, 41);
			this.radioIncludeNeg.Name = "radioIncludeNeg";
			this.radioIncludeNeg.Size = new System.Drawing.Size(120, 18);
			this.radioIncludeNeg.TabIndex = 5;
			this.radioIncludeNeg.Text = "Include";
			// 
			// radioShowOnlyNeg
			// 
			this.radioShowOnlyNeg.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioShowOnlyNeg.Location = new System.Drawing.Point(6, 63);
			this.radioShowOnlyNeg.Name = "radioShowOnlyNeg";
			this.radioShowOnlyNeg.Size = new System.Drawing.Size(120, 18);
			this.radioShowOnlyNeg.TabIndex = 6;
			this.radioShowOnlyNeg.Text = "Show Only";
			// 
			// textDate
			// 
			this.textDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDate.Location = new System.Drawing.Point(721, 68);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(157, 20);
			this.textDate.TabIndex = 101;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(718, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 17);
			this.label1.TabIndex = 100;
			this.label1.Text = "As Of Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.radio30);
			this.groupBox3.Controls.Add(this.radio90);
			this.groupBox3.Controls.Add(this.radio60);
			this.groupBox3.Controls.Add(this.radioAny);
			this.groupBox3.Location = new System.Drawing.Point(721, 423);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(157, 111);
			this.groupBox3.TabIndex = 104;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Age of Account";
			// 
			// radio30
			// 
			this.radio30.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio30.Location = new System.Drawing.Point(6, 41);
			this.radio30.Name = "radio30";
			this.radio30.Size = new System.Drawing.Size(120, 18);
			this.radio30.TabIndex = 1;
			this.radio30.Text = "Over 30 Days";
			// 
			// radio90
			// 
			this.radio90.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio90.Location = new System.Drawing.Point(6, 85);
			this.radio90.Name = "radio90";
			this.radio90.Size = new System.Drawing.Size(120, 18);
			this.radio90.TabIndex = 3;
			this.radio90.Text = "Over 90 Days";
			// 
			// radio60
			// 
			this.radio60.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio60.Location = new System.Drawing.Point(6, 63);
			this.radio60.Name = "radio60";
			this.radio60.Size = new System.Drawing.Size(120, 18);
			this.radio60.TabIndex = 2;
			this.radio60.Text = "Over 60 Days";
			// 
			// radioAny
			// 
			this.radioAny.Checked = true;
			this.radioAny.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAny.Location = new System.Drawing.Point(6, 19);
			this.radioAny.Name = "radioAny";
			this.radioAny.Size = new System.Drawing.Size(120, 18);
			this.radioAny.TabIndex = 0;
			this.radioAny.TabStop = true;
			this.radioAny.Text = "Any Balance";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkAgePayPlanCredits);
			this.groupBox1.Controls.Add(this.checkAgeWriteoffEsts);
			this.groupBox1.Controls.Add(this.checkAgeProcedures);
			this.groupBox1.Controls.Add(this.checkAgePayPlanCharges);
			this.groupBox1.Controls.Add(this.checkAgePatPayments);
			this.groupBox1.Controls.Add(this.checkAgeWriteoffs);
			this.groupBox1.Controls.Add(this.checkAgeInsPayments);
			this.groupBox1.Controls.Add(this.checkAgeInsEsts);
			this.groupBox1.Controls.Add(this.checkAgeAdjustments);
			this.groupBox1.Location = new System.Drawing.Point(721, 92);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(157, 234);
			this.groupBox1.TabIndex = 99;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Include the following:";
			// 
			// checkAgePayPlanCredits
			// 
			this.checkAgePayPlanCredits.Checked = true;
			this.checkAgePayPlanCredits.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgePayPlanCredits.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgePayPlanCredits.Location = new System.Drawing.Point(6, 91);
			this.checkAgePayPlanCredits.Name = "checkAgePayPlanCredits";
			this.checkAgePayPlanCredits.Size = new System.Drawing.Size(145, 18);
			this.checkAgePayPlanCredits.TabIndex = 18;
			this.checkAgePayPlanCredits.Text = "Pay Plan Credits";
			// 
			// checkAgeWriteoffEsts
			// 
			this.checkAgeWriteoffEsts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeWriteoffEsts.Location = new System.Drawing.Point(6, 206);
			this.checkAgeWriteoffEsts.Name = "checkAgeWriteoffEsts";
			this.checkAgeWriteoffEsts.Size = new System.Drawing.Size(145, 18);
			this.checkAgeWriteoffEsts.TabIndex = 17;
			this.checkAgeWriteoffEsts.Text = "Write-off Estimates";
			// 
			// checkAgeProcedures
			// 
			this.checkAgeProcedures.Checked = true;
			this.checkAgeProcedures.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgeProcedures.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeProcedures.Location = new System.Drawing.Point(6, 22);
			this.checkAgeProcedures.Name = "checkAgeProcedures";
			this.checkAgeProcedures.Size = new System.Drawing.Size(145, 18);
			this.checkAgeProcedures.TabIndex = 9;
			this.checkAgeProcedures.Text = "Procedure Fees";
			// 
			// checkAgePayPlanCharges
			// 
			this.checkAgePayPlanCharges.Checked = true;
			this.checkAgePayPlanCharges.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgePayPlanCharges.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgePayPlanCharges.Location = new System.Drawing.Point(6, 68);
			this.checkAgePayPlanCharges.Name = "checkAgePayPlanCharges";
			this.checkAgePayPlanCharges.Size = new System.Drawing.Size(145, 18);
			this.checkAgePayPlanCharges.TabIndex = 16;
			this.checkAgePayPlanCharges.Text = "Pay Plan Charges";
			// 
			// checkAgePatPayments
			// 
			this.checkAgePatPayments.Checked = true;
			this.checkAgePatPayments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgePatPayments.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgePatPayments.Location = new System.Drawing.Point(6, 114);
			this.checkAgePatPayments.Name = "checkAgePatPayments";
			this.checkAgePatPayments.Size = new System.Drawing.Size(145, 18);
			this.checkAgePatPayments.TabIndex = 11;
			this.checkAgePatPayments.Text = "Patient Payments";
			// 
			// checkAgeWriteoffs
			// 
			this.checkAgeWriteoffs.Checked = true;
			this.checkAgeWriteoffs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgeWriteoffs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeWriteoffs.Location = new System.Drawing.Point(6, 183);
			this.checkAgeWriteoffs.Name = "checkAgeWriteoffs";
			this.checkAgeWriteoffs.Size = new System.Drawing.Size(145, 18);
			this.checkAgeWriteoffs.TabIndex = 15;
			this.checkAgeWriteoffs.Text = "Write-offs";
			// 
			// checkAgeInsPayments
			// 
			this.checkAgeInsPayments.Checked = true;
			this.checkAgeInsPayments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgeInsPayments.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeInsPayments.Location = new System.Drawing.Point(6, 137);
			this.checkAgeInsPayments.Name = "checkAgeInsPayments";
			this.checkAgeInsPayments.Size = new System.Drawing.Size(145, 18);
			this.checkAgeInsPayments.TabIndex = 12;
			this.checkAgeInsPayments.Text = "Insurance Payments";
			// 
			// checkAgeInsEsts
			// 
			this.checkAgeInsEsts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeInsEsts.Location = new System.Drawing.Point(6, 160);
			this.checkAgeInsEsts.Name = "checkAgeInsEsts";
			this.checkAgeInsEsts.Size = new System.Drawing.Size(145, 18);
			this.checkAgeInsEsts.TabIndex = 14;
			this.checkAgeInsEsts.Text = "Insurance Estimates";
			// 
			// checkAgeAdjustments
			// 
			this.checkAgeAdjustments.Checked = true;
			this.checkAgeAdjustments.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAgeAdjustments.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgeAdjustments.Location = new System.Drawing.Point(6, 45);
			this.checkAgeAdjustments.Name = "checkAgeAdjustments";
			this.checkAgeAdjustments.Size = new System.Drawing.Size(145, 18);
			this.checkAgeAdjustments.TabIndex = 13;
			this.checkAgeAdjustments.Text = "Adjustments";
			// 
			// checkExcludeArchive
			// 
			this.checkExcludeArchive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkExcludeArchive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeArchive.Location = new System.Drawing.Point(884, 632);
			this.checkExcludeArchive.Name = "checkExcludeArchive";
			this.checkExcludeArchive.Size = new System.Drawing.Size(157, 18);
			this.checkExcludeArchive.TabIndex = 119;
			this.checkExcludeArchive.Text = "Exclude archived patients";
			// 
			// FormRpCustomAging
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1059, 742);
			this.Controls.Add(this.checkExcludeArchive);
			this.Controls.Add(this.checkExcludeBadAddresses);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.checkAllBillType);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.listBoxClins);
			this.Controls.Add(this.listBoxProvs);
			this.Controls.Add(this.listBoxBillTypes);
			this.Controls.Add(this.checkAgeCredits);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.checkExcludeInactive);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupGroupBy);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridTotals);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpCustomAging";
			this.Text = "Custom Aging Report";
			this.Load += new System.EventHandler(this.FormRpCustomAging_Load);
			this.contextMenuStrip.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupGroupBy.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.Button butRefresh;
		private UI.GridOD gridMain;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem goToAccountToolStripMenuItem;
		private UI.Button butPrint;
		private UI.GridOD gridTotals;
		private System.Windows.Forms.CheckBox checkExcludeBadAddresses;
		private System.Windows.Forms.CheckBox checkAllClin;
		private System.Windows.Forms.CheckBox checkAllProv;
		private System.Windows.Forms.CheckBox checkAllBillType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelClinic;
		private OpenDental.UI.ListBoxOD listBoxClins;
		private OpenDental.UI.ListBoxOD listBoxProvs;
		private OpenDental.UI.ListBoxOD listBoxBillTypes;
		private System.Windows.Forms.CheckBox checkAgeCredits;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioWriteoffInsPayDate;
		private System.Windows.Forms.RadioButton radioWriteoffProcDate;
		private System.Windows.Forms.CheckBox checkExcludeInactive;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupGroupBy;
		private System.Windows.Forms.RadioButton radioGroupByPat;
		private System.Windows.Forms.RadioButton radioGroupByFam;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioExcludeNeg;
		private System.Windows.Forms.RadioButton radioIncludeNeg;
		private System.Windows.Forms.RadioButton radioShowOnlyNeg;
		private ValidDate textDate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radio30;
		private System.Windows.Forms.RadioButton radio90;
		private System.Windows.Forms.RadioButton radio60;
		private System.Windows.Forms.RadioButton radioAny;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkAgePayPlanCredits;
		private System.Windows.Forms.CheckBox checkAgeWriteoffEsts;
		private System.Windows.Forms.CheckBox checkAgeProcedures;
		private System.Windows.Forms.CheckBox checkAgePayPlanCharges;
		private System.Windows.Forms.CheckBox checkAgePatPayments;
		private System.Windows.Forms.CheckBox checkAgeWriteoffs;
		private System.Windows.Forms.CheckBox checkAgeInsPayments;
		private System.Windows.Forms.CheckBox checkAgeInsEsts;
		private System.Windows.Forms.CheckBox checkAgeAdjustments;
		private System.Windows.Forms.CheckBox checkExcludeArchive;
		private System.Windows.Forms.RadioButton radioWriteoffClaimDate;
	}
}