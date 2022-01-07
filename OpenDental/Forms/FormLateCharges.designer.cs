namespace OpenDental{
	partial class FormLateCharges {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLateCharges));
			this.butRun = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDateLastRun = new OpenDental.ValidDate();
			this.labelDateLastRun = new System.Windows.Forms.Label();
			this.textDateNewCharges = new OpenDental.ValidDate();
			this.labelDateNewCharges = new System.Windows.Forms.Label();
			this.labelBillType = new System.Windows.Forms.Label();
			this.groupBoxUndo = new OpenDental.UI.GroupBoxOD();
			this.textDateUndo = new OpenDental.ValidDate();
			this.labelDateUndo = new System.Windows.Forms.Label();
			this.butUndo = new OpenDental.UI.Button();
			this.checkExcludeAccountNoTil = new System.Windows.Forms.CheckBox();
			this.listBillType = new OpenDental.UI.ListBoxOD();
			this.textExcludeLessThan = new OpenDental.ValidDouble();
			this.labelExcludeBalanceLessThan = new System.Windows.Forms.Label();
			this.textMinCharge = new OpenDental.ValidDouble();
			this.labelMinCharge = new System.Windows.Forms.Label();
			this.textMaxCharge = new OpenDental.ValidDouble();
			this.labelMaxCharge = new System.Windows.Forms.Label();
			this.labelChargePercent = new System.Windows.Forms.Label();
			this.textChargePercent = new OpenDental.ValidNum();
			this.labelDateRange1 = new System.Windows.Forms.Label();
			this.textDateRangeEnd = new OpenDental.ValidNum();
			this.labelDateRange2 = new System.Windows.Forms.Label();
			this.textDateRangeStart = new OpenDental.ValidNum();
			this.labelDateRange3 = new System.Windows.Forms.Label();
			this.butSaveDefaults = new OpenDental.UI.Button();
			this.groupLateCharge = new OpenDental.UI.GroupBoxOD();
			this.checkExcludeExistingLateCharges = new System.Windows.Forms.CheckBox();
			this.radioPatPriProv = new System.Windows.Forms.RadioButton();
			this.radioSpecificProv = new System.Windows.Forms.RadioButton();
			this.comboSpecificProv = new OpenDental.UI.ComboBoxOD();
			this.groupBoxAssignCharge = new OpenDental.UI.GroupBoxOD();
			this.groupBoxUndo.SuspendLayout();
			this.groupLateCharge.SuspendLayout();
			this.groupBoxAssignCharge.SuspendLayout();
			this.SuspendLayout();
			// 
			// butRun
			// 
			this.butRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRun.Location = new System.Drawing.Point(434, 407);
			this.butRun.Name = "butRun";
			this.butRun.Size = new System.Drawing.Size(75, 24);
			this.butRun.TabIndex = 5;
			this.butRun.Text = "Run";
			this.butRun.Click += new System.EventHandler(this.butRun_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(515, 407);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDateLastRun
			// 
			this.textDateLastRun.Location = new System.Drawing.Point(124, 7);
			this.textDateLastRun.Name = "textDateLastRun";
			this.textDateLastRun.ReadOnly = true;
			this.textDateLastRun.Size = new System.Drawing.Size(78, 20);
			this.textDateLastRun.TabIndex = 0;
			// 
			// labelDateLastRun
			// 
			this.labelDateLastRun.Location = new System.Drawing.Point(53, 10);
			this.labelDateLastRun.Name = "labelDateLastRun";
			this.labelDateLastRun.Size = new System.Drawing.Size(70, 14);
			this.labelDateLastRun.TabIndex = 32;
			this.labelDateLastRun.Text = "Date last run";
			this.labelDateLastRun.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateNewCharges
			// 
			this.textDateNewCharges.Location = new System.Drawing.Point(124, 34);
			this.textDateNewCharges.Name = "textDateNewCharges";
			this.textDateNewCharges.Size = new System.Drawing.Size(78, 20);
			this.textDateNewCharges.TabIndex = 1;
			// 
			// labelDateNewCharges
			// 
			this.labelDateNewCharges.Location = new System.Drawing.Point(3, 37);
			this.labelDateNewCharges.Name = "labelDateNewCharges";
			this.labelDateNewCharges.Size = new System.Drawing.Size(120, 14);
			this.labelDateNewCharges.TabIndex = 30;
			this.labelDateNewCharges.Text = "Date of new charges";
			this.labelDateNewCharges.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelBillType
			// 
			this.labelBillType.Location = new System.Drawing.Point(399, 37);
			this.labelBillType.Name = "labelBillType";
			this.labelBillType.Size = new System.Drawing.Size(176, 16);
			this.labelBillType.TabIndex = 34;
			this.labelBillType.Text = "Only apply to these Billing Types";
			this.labelBillType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBoxUndo
			// 
			this.groupBoxUndo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxUndo.Controls.Add(this.textDateUndo);
			this.groupBoxUndo.Controls.Add(this.labelDateUndo);
			this.groupBoxUndo.Controls.Add(this.butUndo);
			this.groupBoxUndo.Location = new System.Drawing.Point(12, 310);
			this.groupBoxUndo.Name = "groupBoxUndo";
			this.groupBoxUndo.Size = new System.Drawing.Size(167, 79);
			this.groupBoxUndo.TabIndex = 3;
			this.groupBoxUndo.TabStop = false;
			this.groupBoxUndo.Text = "Undo late charges";
			// 
			// textDateUndo
			// 
			this.textDateUndo.Location = new System.Drawing.Point(80, 19);
			this.textDateUndo.Name = "textDateUndo";
			this.textDateUndo.ReadOnly = true;
			this.textDateUndo.Size = new System.Drawing.Size(78, 20);
			this.textDateUndo.TabIndex = 31;
			// 
			// labelDateUndo
			// 
			this.labelDateUndo.Location = new System.Drawing.Point(3, 21);
			this.labelDateUndo.Name = "labelDateUndo";
			this.labelDateUndo.Size = new System.Drawing.Size(76, 16);
			this.labelDateUndo.TabIndex = 32;
			this.labelDateUndo.Text = "Date to undo";
			this.labelDateUndo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butUndo
			// 
			this.butUndo.Location = new System.Drawing.Point(80, 45);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(78, 24);
			this.butUndo.TabIndex = 0;
			this.butUndo.Text = "Undo";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// checkExcludeAccountNoTil
			// 
			this.checkExcludeAccountNoTil.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeAccountNoTil.Location = new System.Drawing.Point(7, 27);
			this.checkExcludeAccountNoTil.Name = "checkExcludeAccountNoTil";
			this.checkExcludeAccountNoTil.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkExcludeAccountNoTil.Size = new System.Drawing.Size(303, 16);
			this.checkExcludeAccountNoTil.TabIndex = 0;
			this.checkExcludeAccountNoTil.Text = "Exclude accounts (guarantor) without Truth in Lending";
			this.checkExcludeAccountNoTil.UseVisualStyleBackColor = true;
			// 
			// listBillType
			// 
			this.listBillType.Location = new System.Drawing.Point(399, 54);
			this.listBillType.Name = "listBillType";
			this.listBillType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBillType.Size = new System.Drawing.Size(158, 147);
			this.listBillType.TabIndex = 8;
			this.listBillType.Text = "listBoxOD1";
			// 
			// textExcludeLessThan
			// 
			this.textExcludeLessThan.BackColor = System.Drawing.SystemColors.Window;
			this.textExcludeLessThan.Location = new System.Drawing.Point(297, 73);
			this.textExcludeLessThan.MaxVal = 100000000D;
			this.textExcludeLessThan.MinVal = 0.01D;
			this.textExcludeLessThan.Name = "textExcludeLessThan";
			this.textExcludeLessThan.Size = new System.Drawing.Size(42, 20);
			this.textExcludeLessThan.TabIndex = 2;
			// 
			// labelExcludeBalanceLessThan
			// 
			this.labelExcludeBalanceLessThan.Location = new System.Drawing.Point(124, 75);
			this.labelExcludeBalanceLessThan.Name = "labelExcludeBalanceLessThan";
			this.labelExcludeBalanceLessThan.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelExcludeBalanceLessThan.Size = new System.Drawing.Size(172, 16);
			this.labelExcludeBalanceLessThan.TabIndex = 55;
			this.labelExcludeBalanceLessThan.Text = "Exclude if balance is less than";
			this.labelExcludeBalanceLessThan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMinCharge
			// 
			this.textMinCharge.BackColor = System.Drawing.SystemColors.Window;
			this.textMinCharge.Location = new System.Drawing.Point(297, 127);
			this.textMinCharge.MaxVal = 100000000D;
			this.textMinCharge.MinVal = 0D;
			this.textMinCharge.Name = "textMinCharge";
			this.textMinCharge.Size = new System.Drawing.Size(42, 20);
			this.textMinCharge.TabIndex = 4;
			// 
			// labelMinCharge
			// 
			this.labelMinCharge.Location = new System.Drawing.Point(203, 129);
			this.labelMinCharge.Name = "labelMinCharge";
			this.labelMinCharge.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelMinCharge.Size = new System.Drawing.Size(93, 16);
			this.labelMinCharge.TabIndex = 57;
			this.labelMinCharge.Text = "Minimum charge";
			this.labelMinCharge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMaxCharge
			// 
			this.textMaxCharge.BackColor = System.Drawing.SystemColors.Window;
			this.textMaxCharge.Location = new System.Drawing.Point(297, 154);
			this.textMaxCharge.MaxVal = 100000000D;
			this.textMaxCharge.MinVal = 0D;
			this.textMaxCharge.Name = "textMaxCharge";
			this.textMaxCharge.Size = new System.Drawing.Size(42, 20);
			this.textMaxCharge.TabIndex = 5;
			// 
			// labelMaxCharge
			// 
			this.labelMaxCharge.Location = new System.Drawing.Point(200, 156);
			this.labelMaxCharge.Name = "labelMaxCharge";
			this.labelMaxCharge.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelMaxCharge.Size = new System.Drawing.Size(96, 16);
			this.labelMaxCharge.TabIndex = 59;
			this.labelMaxCharge.Text = "Maximum charge";
			this.labelMaxCharge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelChargePercent
			// 
			this.labelChargePercent.Location = new System.Drawing.Point(123, 102);
			this.labelChargePercent.Name = "labelChargePercent";
			this.labelChargePercent.Size = new System.Drawing.Size(173, 14);
			this.labelChargePercent.TabIndex = 61;
			this.labelChargePercent.Text = "Percentage (% fee on amt due)";
			this.labelChargePercent.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textChargePercent
			// 
			this.textChargePercent.Location = new System.Drawing.Point(297, 100);
			this.textChargePercent.MaxVal = 100000000;
			this.textChargePercent.Name = "textChargePercent";
			this.textChargePercent.ShowZero = false;
			this.textChargePercent.Size = new System.Drawing.Size(42, 20);
			this.textChargePercent.TabIndex = 3;
			// 
			// labelDateRange1
			// 
			this.labelDateRange1.Location = new System.Drawing.Point(33, 183);
			this.labelDateRange1.Name = "labelDateRange1";
			this.labelDateRange1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelDateRange1.Size = new System.Drawing.Size(191, 16);
			this.labelDateRange1.TabIndex = 63;
			this.labelDateRange1.Text = "Apply to statements sent between";
			this.labelDateRange1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateRangeEnd
			// 
			this.textDateRangeEnd.Location = new System.Drawing.Point(225, 181);
			this.textDateRangeEnd.MaxVal = 100000000;
			this.textDateRangeEnd.Name = "textDateRangeEnd";
			this.textDateRangeEnd.Size = new System.Drawing.Size(42, 20);
			this.textDateRangeEnd.TabIndex = 6;
			// 
			// labelDateRange2
			// 
			this.labelDateRange2.Location = new System.Drawing.Point(268, 182);
			this.labelDateRange2.Name = "labelDateRange2";
			this.labelDateRange2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelDateRange2.Size = new System.Drawing.Size(30, 16);
			this.labelDateRange2.TabIndex = 65;
			this.labelDateRange2.Text = "and";
			this.labelDateRange2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textDateRangeStart
			// 
			this.textDateRangeStart.Location = new System.Drawing.Point(297, 181);
			this.textDateRangeStart.MaxVal = 100000000;
			this.textDateRangeStart.Name = "textDateRangeStart";
			this.textDateRangeStart.Size = new System.Drawing.Size(42, 20);
			this.textDateRangeStart.TabIndex = 7;
			// 
			// labelDateRange3
			// 
			this.labelDateRange3.Location = new System.Drawing.Point(340, 183);
			this.labelDateRange3.Name = "labelDateRange3";
			this.labelDateRange3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelDateRange3.Size = new System.Drawing.Size(57, 16);
			this.labelDateRange3.TabIndex = 67;
			this.labelDateRange3.Text = "days ago";
			this.labelDateRange3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSaveDefaults
			// 
			this.butSaveDefaults.Location = new System.Drawing.Point(458, 209);
			this.butSaveDefaults.Name = "butSaveDefaults";
			this.butSaveDefaults.Size = new System.Drawing.Size(99, 24);
			this.butSaveDefaults.TabIndex = 9;
			this.butSaveDefaults.Text = "&Save As Default";
			this.butSaveDefaults.Click += new System.EventHandler(this.butSaveDefaults_Click);
			// 
			// groupLateCharge
			// 
			this.groupLateCharge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupLateCharge.Controls.Add(this.checkExcludeExistingLateCharges);
			this.groupLateCharge.Controls.Add(this.checkExcludeAccountNoTil);
			this.groupLateCharge.Controls.Add(this.listBillType);
			this.groupLateCharge.Controls.Add(this.butSaveDefaults);
			this.groupLateCharge.Controls.Add(this.labelExcludeBalanceLessThan);
			this.groupLateCharge.Controls.Add(this.labelBillType);
			this.groupLateCharge.Controls.Add(this.labelDateRange3);
			this.groupLateCharge.Controls.Add(this.textExcludeLessThan);
			this.groupLateCharge.Controls.Add(this.textDateRangeStart);
			this.groupLateCharge.Controls.Add(this.labelMinCharge);
			this.groupLateCharge.Controls.Add(this.labelDateRange2);
			this.groupLateCharge.Controls.Add(this.textMinCharge);
			this.groupLateCharge.Controls.Add(this.textDateRangeEnd);
			this.groupLateCharge.Controls.Add(this.labelMaxCharge);
			this.groupLateCharge.Controls.Add(this.labelDateRange1);
			this.groupLateCharge.Controls.Add(this.textMaxCharge);
			this.groupLateCharge.Controls.Add(this.labelChargePercent);
			this.groupLateCharge.Controls.Add(this.textChargePercent);
			this.groupLateCharge.Location = new System.Drawing.Point(12, 62);
			this.groupLateCharge.Name = "groupLateCharge";
			this.groupLateCharge.Size = new System.Drawing.Size(578, 241);
			this.groupLateCharge.TabIndex = 2;
			this.groupLateCharge.TabStop = false;
			this.groupLateCharge.Text = "Late charge settings";
			// 
			// checkExcludeExistingLateCharges
			// 
			this.checkExcludeExistingLateCharges.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeExistingLateCharges.Location = new System.Drawing.Point(7, 50);
			this.checkExcludeExistingLateCharges.Name = "checkExcludeExistingLateCharges";
			this.checkExcludeExistingLateCharges.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkExcludeExistingLateCharges.Size = new System.Drawing.Size(303, 16);
			this.checkExcludeExistingLateCharges.TabIndex = 1;
			this.checkExcludeExistingLateCharges.Text = "Exclude existing late charges";
			this.checkExcludeExistingLateCharges.UseVisualStyleBackColor = true;
			// 
			// radioPatPriProv
			// 
			this.radioPatPriProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioPatPriProv.Location = new System.Drawing.Point(12, 24);
			this.radioPatPriProv.Name = "radioPatPriProv";
			this.radioPatPriProv.Size = new System.Drawing.Size(211, 16);
			this.radioPatPriProv.TabIndex = 0;
			this.radioPatPriProv.Text = "Guarantor\'s Primary Provider";
			this.radioPatPriProv.UseVisualStyleBackColor = true;
			this.radioPatPriProv.Click += new System.EventHandler(this.radioPatPriProv_Click);
			// 
			// radioSpecificProv
			// 
			this.radioSpecificProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioSpecificProv.Location = new System.Drawing.Point(12, 47);
			this.radioSpecificProv.Name = "radioSpecificProv";
			this.radioSpecificProv.Size = new System.Drawing.Size(108, 16);
			this.radioSpecificProv.TabIndex = 1;
			this.radioSpecificProv.Text = "Specific Provider";
			this.radioSpecificProv.UseVisualStyleBackColor = true;
			this.radioSpecificProv.Click += new System.EventHandler(this.radioSpecificProv_Click);
			// 
			// comboSpecificProv
			// 
			this.comboSpecificProv.Location = new System.Drawing.Point(121, 45);
			this.comboSpecificProv.Name = "comboSpecificProv";
			this.comboSpecificProv.Size = new System.Drawing.Size(174, 21);
			this.comboSpecificProv.TabIndex = 2;
			// 
			// groupBoxAssignCharge
			// 
			this.groupBoxAssignCharge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxAssignCharge.Controls.Add(this.comboSpecificProv);
			this.groupBoxAssignCharge.Controls.Add(this.radioSpecificProv);
			this.groupBoxAssignCharge.Controls.Add(this.radioPatPriProv);
			this.groupBoxAssignCharge.Location = new System.Drawing.Point(186, 310);
			this.groupBoxAssignCharge.Name = "groupBoxAssignCharge";
			this.groupBoxAssignCharge.Size = new System.Drawing.Size(301, 79);
			this.groupBoxAssignCharge.TabIndex = 4;
			this.groupBoxAssignCharge.TabStop = false;
			this.groupBoxAssignCharge.Text = "Assign charges to";
			// 
			// FormLateCharges
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(602, 443);
			this.Controls.Add(this.groupLateCharge);
			this.Controls.Add(this.groupBoxAssignCharge);
			this.Controls.Add(this.groupBoxUndo);
			this.Controls.Add(this.textDateLastRun);
			this.Controls.Add(this.labelDateLastRun);
			this.Controls.Add(this.textDateNewCharges);
			this.Controls.Add(this.labelDateNewCharges);
			this.Controls.Add(this.butRun);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormLateCharges";
			this.Text = "Late Charges";
			this.Load += new System.EventHandler(this.FormLateCharges_Load);
			this.Shown += new System.EventHandler(this.FormLateCharges_Shown);
			this.groupBoxUndo.ResumeLayout(false);
			this.groupBoxUndo.PerformLayout();
			this.groupLateCharge.ResumeLayout(false);
			this.groupLateCharge.PerformLayout();
			this.groupBoxAssignCharge.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butRun;
		private OpenDental.UI.Button butCancel;
		private ValidDate textDateLastRun;
		private System.Windows.Forms.Label labelDateLastRun;
		private ValidDate textDateNewCharges;
		private System.Windows.Forms.Label labelDateNewCharges;
		private System.Windows.Forms.Label labelBillType;
		private OpenDental.UI.GroupBoxOD groupBoxUndo;
		private ValidDate textDateUndo;
		private System.Windows.Forms.Label labelDateUndo;
		private UI.Button butUndo;
		private System.Windows.Forms.CheckBox checkExcludeAccountNoTil;
		private UI.ListBoxOD listBillType;
		private ValidDouble textExcludeLessThan;
		private System.Windows.Forms.Label labelExcludeBalanceLessThan;
		private ValidDouble textMinCharge;
		private System.Windows.Forms.Label labelMinCharge;
		private ValidDouble textMaxCharge;
		private System.Windows.Forms.Label labelMaxCharge;
		private System.Windows.Forms.Label labelChargePercent;
		private ValidNum textChargePercent;
		private System.Windows.Forms.Label labelDateRange1;
		private ValidNum textDateRangeEnd;
		private System.Windows.Forms.Label labelDateRange2;
		private ValidNum textDateRangeStart;
		private System.Windows.Forms.Label labelDateRange3;
		private UI.Button butSaveDefaults;
		private OpenDental.UI.GroupBoxOD groupLateCharge;
		private System.Windows.Forms.RadioButton radioPatPriProv;
		private System.Windows.Forms.RadioButton radioSpecificProv;
		private UI.ComboBoxOD comboSpecificProv;
		private OpenDental.UI.GroupBoxOD groupBoxAssignCharge;
		private System.Windows.Forms.CheckBox checkExcludeExistingLateCharges;
	}
}