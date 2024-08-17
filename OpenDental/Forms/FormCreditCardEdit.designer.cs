namespace OpenDental{
	partial class FormCreditCardEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreditCardEdit));
			this.labelClinic = new System.Windows.Forms.Label();
			this.textClinic = new System.Windows.Forms.TextBox();
			this.groupChargeFrequency = new OpenDental.UI.GroupBox();
			this.comboDays = new OpenDental.UI.ComboBox();
			this.comboFrequency = new OpenDental.UI.ComboBox();
			this.butAddDay = new OpenDental.UI.Button();
			this.butClearDayOfMonth = new OpenDental.UI.Button();
			this.textDayOfMonth = new System.Windows.Forms.TextBox();
			this.radioWeekDay = new System.Windows.Forms.RadioButton();
			this.radioDayOfMonth = new System.Windows.Forms.RadioButton();
			this.labelFrequencyInWords = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textAccountType = new System.Windows.Forms.TextBox();
			this.labelAcctType = new System.Windows.Forms.Label();
			this.groupProcedures = new OpenDental.UI.GroupBox();
			this.checkExcludeProcSync = new OpenDental.UI.CheckBox();
			this.listProcs = new OpenDental.UI.ListBox();
			this.butRemoveProc = new OpenDental.UI.Button();
			this.label15 = new System.Windows.Forms.Label();
			this.butAddProc = new OpenDental.UI.Button();
			this.groupRecurringCharges = new OpenDental.UI.GroupBox();
			this.textDateStop = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelNextChargeDate = new System.Windows.Forms.Label();
			this.textNextChargeDate = new OpenDental.ValidDate();
			this.labelPreviousStartDate = new System.Windows.Forms.Label();
			this.textPreviousStartDate = new OpenDental.ValidDate();
			this.checkIsRecurringActive = new OpenDental.UI.CheckBox();
			this.comboPaymentType = new OpenDental.UI.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.checkChrgWithNoBal = new OpenDental.UI.CheckBox();
			this.labelPayPlan = new System.Windows.Forms.Label();
			this.comboPaymentPlans = new OpenDental.UI.ComboBox();
			this.butToday = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.textNote = new OpenDental.ODtextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textChargeAmt = new OpenDental.ValidDouble();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textZip = new System.Windows.Forms.TextBox();
			this.labelAddress = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.labelZip = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textExpDate = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textCardNumber = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupChargeFrequency.SuspendLayout();
			this.groupProcedures.SuspendLayout();
			this.groupRecurringCharges.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(287, 14);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(50, 16);
			this.labelClinic.TabIndex = 138;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelClinic.Visible = false;
			// 
			// textClinic
			// 
			this.textClinic.Location = new System.Drawing.Point(338, 13);
			this.textClinic.Name = "textClinic";
			this.textClinic.ReadOnly = true;
			this.textClinic.Size = new System.Drawing.Size(169, 20);
			this.textClinic.TabIndex = 2;
			this.textClinic.TabStop = false;
			this.textClinic.Visible = false;
			// 
			// groupChargeFrequency
			// 
			this.groupChargeFrequency.Controls.Add(this.comboDays);
			this.groupChargeFrequency.Controls.Add(this.comboFrequency);
			this.groupChargeFrequency.Controls.Add(this.butAddDay);
			this.groupChargeFrequency.Controls.Add(this.butClearDayOfMonth);
			this.groupChargeFrequency.Controls.Add(this.textDayOfMonth);
			this.groupChargeFrequency.Controls.Add(this.radioWeekDay);
			this.groupChargeFrequency.Controls.Add(this.radioDayOfMonth);
			this.groupChargeFrequency.Controls.Add(this.labelFrequencyInWords);
			this.groupChargeFrequency.Controls.Add(this.label8);
			this.groupChargeFrequency.Location = new System.Drawing.Point(12, 360);
			this.groupChargeFrequency.Name = "groupChargeFrequency";
			this.groupChargeFrequency.Size = new System.Drawing.Size(495, 114);
			this.groupChargeFrequency.TabIndex = 19;
			this.groupChargeFrequency.TabStop = true;
			this.groupChargeFrequency.Text = "Charge Frequency";
			// 
			// comboDays
			// 
			this.comboDays.Location = new System.Drawing.Point(319, 55);
			this.comboDays.Name = "comboDays";
			this.comboDays.Size = new System.Drawing.Size(93, 21);
			this.comboDays.TabIndex = 25;
			this.comboDays.SelectionChangeCommitted += new System.EventHandler(this.comboDays_SelectionChangeCommitted);
			// 
			// comboFrequency
			// 
			this.comboFrequency.Location = new System.Drawing.Point(220, 55);
			this.comboFrequency.Name = "comboFrequency";
			this.comboFrequency.Size = new System.Drawing.Size(93, 21);
			this.comboFrequency.TabIndex = 24;
			this.comboFrequency.SelectionChangeCommitted += new System.EventHandler(this.comboFrequency_SelectionChangeCommitted);
			// 
			// butAddDay
			// 
			this.butAddDay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddDay.Location = new System.Drawing.Point(63, 81);
			this.butAddDay.Name = "butAddDay";
			this.butAddDay.Size = new System.Drawing.Size(60, 22);
			this.butAddDay.TabIndex = 21;
			this.butAddDay.Text = "Add Day";
			this.butAddDay.Click += new System.EventHandler(this.butAddDay_Click);
			// 
			// butClearDayOfMonth
			// 
			this.butClearDayOfMonth.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClearDayOfMonth.Location = new System.Drawing.Point(128, 81);
			this.butClearDayOfMonth.Name = "butClearDayOfMonth";
			this.butClearDayOfMonth.Size = new System.Drawing.Size(60, 22);
			this.butClearDayOfMonth.TabIndex = 22;
			this.butClearDayOfMonth.Text = "Clear";
			this.butClearDayOfMonth.Click += new System.EventHandler(this.butClearDayOfMonth_Click);
			// 
			// textDayOfMonth
			// 
			this.textDayOfMonth.Location = new System.Drawing.Point(63, 56);
			this.textDayOfMonth.Name = "textDayOfMonth";
			this.textDayOfMonth.ReadOnly = true;
			this.textDayOfMonth.Size = new System.Drawing.Size(125, 20);
			this.textDayOfMonth.TabIndex = 20;
			this.textDayOfMonth.TabStop = false;
			this.textDayOfMonth.TextChanged += new System.EventHandler(this.textDayOfMonth_TextChanged);
			// 
			// radioWeekDay
			// 
			this.radioWeekDay.AutoSize = true;
			this.radioWeekDay.Location = new System.Drawing.Point(270, 35);
			this.radioWeekDay.Name = "radioWeekDay";
			this.radioWeekDay.Size = new System.Drawing.Size(99, 17);
			this.radioWeekDay.TabIndex = 23;
			this.radioWeekDay.Text = "Fixed week day";
			this.radioWeekDay.UseVisualStyleBackColor = true;
			// 
			// radioDayOfMonth
			// 
			this.radioDayOfMonth.AutoSize = true;
			this.radioDayOfMonth.Checked = true;
			this.radioDayOfMonth.Location = new System.Drawing.Point(65, 35);
			this.radioDayOfMonth.Name = "radioDayOfMonth";
			this.radioDayOfMonth.Size = new System.Drawing.Size(125, 17);
			this.radioDayOfMonth.TabIndex = 19;
			this.radioDayOfMonth.TabStop = true;
			this.radioDayOfMonth.Text = "Fixed day(s) of month";
			this.radioDayOfMonth.UseVisualStyleBackColor = true;
			this.radioDayOfMonth.CheckedChanged += new System.EventHandler(this.radioDayOfMonth_CheckedChanged);
			// 
			// labelFrequencyInWords
			// 
			this.labelFrequencyInWords.Location = new System.Drawing.Point(21, 14);
			this.labelFrequencyInWords.Name = "labelFrequencyInWords";
			this.labelFrequencyInWords.Size = new System.Drawing.Size(467, 16);
			this.labelFrequencyInWords.TabIndex = 134;
			this.labelFrequencyInWords.Text = "This will display the frequency in words";
			this.labelFrequencyInWords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(414, 59);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(76, 16);
			this.label8.TabIndex = 144;
			this.label8.Text = " of the month";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textAccountType
			// 
			this.textAccountType.Location = new System.Drawing.Point(338, 39);
			this.textAccountType.MaxLength = 100;
			this.textAccountType.Name = "textAccountType";
			this.textAccountType.ReadOnly = true;
			this.textAccountType.Size = new System.Drawing.Size(80, 20);
			this.textAccountType.TabIndex = 4;
			this.textAccountType.TabStop = false;
			this.textAccountType.Visible = false;
			// 
			// labelAcctType
			// 
			this.labelAcctType.Location = new System.Drawing.Point(250, 40);
			this.labelAcctType.Name = "labelAcctType";
			this.labelAcctType.Size = new System.Drawing.Size(87, 16);
			this.labelAcctType.TabIndex = 135;
			this.labelAcctType.Text = "Account Type";
			this.labelAcctType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelAcctType.Visible = false;
			// 
			// groupProcedures
			// 
			this.groupProcedures.Controls.Add(this.checkExcludeProcSync);
			this.groupProcedures.Controls.Add(this.listProcs);
			this.groupProcedures.Controls.Add(this.butRemoveProc);
			this.groupProcedures.Controls.Add(this.label15);
			this.groupProcedures.Controls.Add(this.butAddProc);
			this.groupProcedures.Location = new System.Drawing.Point(14, 480);
			this.groupProcedures.Name = "groupProcedures";
			this.groupProcedures.Size = new System.Drawing.Size(493, 142);
			this.groupProcedures.TabIndex = 26;
			this.groupProcedures.TabStop = true;
			this.groupProcedures.Text = "Authorized Procedures";
			this.groupProcedures.Visible = false;
			// 
			// checkExcludeProcSync
			// 
			this.checkExcludeProcSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkExcludeProcSync.Location = new System.Drawing.Point(355, 8);
			this.checkExcludeProcSync.Name = "checkExcludeProcSync";
			this.checkExcludeProcSync.Size = new System.Drawing.Size(133, 36);
			this.checkExcludeProcSync.TabIndex = 26;
			this.checkExcludeProcSync.Text = "Exclude from proc sync";
			// 
			// listProcs
			// 
			this.listProcs.Location = new System.Drawing.Point(129, 19);
			this.listProcs.Name = "listProcs";
			this.listProcs.Size = new System.Drawing.Size(220, 108);
			this.listProcs.TabIndex = 29;
			// 
			// butRemoveProc
			// 
			this.butRemoveProc.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butRemoveProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemoveProc.Location = new System.Drawing.Point(355, 103);
			this.butRemoveProc.Name = "butRemoveProc";
			this.butRemoveProc.Size = new System.Drawing.Size(78, 24);
			this.butRemoveProc.TabIndex = 28;
			this.butRemoveProc.Text = "Remove";
			this.butRemoveProc.Click += new System.EventHandler(this.butRemoveProc_Click);
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(25, 19);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(98, 67);
			this.label15.TabIndex = 133;
			this.label15.Text = "Procedures that will cause a recurring charge on this card";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butAddProc
			// 
			this.butAddProc.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddProc.Location = new System.Drawing.Point(355, 73);
			this.butAddProc.Name = "butAddProc";
			this.butAddProc.Size = new System.Drawing.Size(78, 24);
			this.butAddProc.TabIndex = 27;
			this.butAddProc.Text = "Add";
			this.butAddProc.Click += new System.EventHandler(this.butAddProc_Click);
			// 
			// groupRecurringCharges
			// 
			this.groupRecurringCharges.Controls.Add(this.textDateStop);
			this.groupRecurringCharges.Controls.Add(this.textDateStart);
			this.groupRecurringCharges.Controls.Add(this.labelNextChargeDate);
			this.groupRecurringCharges.Controls.Add(this.textNextChargeDate);
			this.groupRecurringCharges.Controls.Add(this.labelPreviousStartDate);
			this.groupRecurringCharges.Controls.Add(this.textPreviousStartDate);
			this.groupRecurringCharges.Controls.Add(this.checkIsRecurringActive);
			this.groupRecurringCharges.Controls.Add(this.comboPaymentType);
			this.groupRecurringCharges.Controls.Add(this.label9);
			this.groupRecurringCharges.Controls.Add(this.checkChrgWithNoBal);
			this.groupRecurringCharges.Controls.Add(this.labelPayPlan);
			this.groupRecurringCharges.Controls.Add(this.comboPaymentPlans);
			this.groupRecurringCharges.Controls.Add(this.butToday);
			this.groupRecurringCharges.Controls.Add(this.butClear);
			this.groupRecurringCharges.Controls.Add(this.textNote);
			this.groupRecurringCharges.Controls.Add(this.label7);
			this.groupRecurringCharges.Controls.Add(this.textChargeAmt);
			this.groupRecurringCharges.Controls.Add(this.label1);
			this.groupRecurringCharges.Controls.Add(this.label5);
			this.groupRecurringCharges.Controls.Add(this.label2);
			this.groupRecurringCharges.Location = new System.Drawing.Point(12, 119);
			this.groupRecurringCharges.Name = "groupRecurringCharges";
			this.groupRecurringCharges.Size = new System.Drawing.Size(495, 235);
			this.groupRecurringCharges.TabIndex = 7;
			this.groupRecurringCharges.TabStop = true;
			this.groupRecurringCharges.Text = "Authorized Recurring Charges";
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(129, 137);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.Size = new System.Drawing.Size(88, 20);
			this.textDateStop.TabIndex = 13;
			this.textDateStop.Leave += new System.EventHandler(this.textDateStop_Leave);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(129, 109);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(88, 20);
			this.textDateStart.TabIndex = 11;
			this.textDateStart.Leave += new System.EventHandler(this.textDateStart_Leave);
			// 
			// labelNextChargeDate
			// 
			this.labelNextChargeDate.Location = new System.Drawing.Point(305, 81);
			this.labelNextChargeDate.Name = "labelNextChargeDate";
			this.labelNextChargeDate.Size = new System.Drawing.Size(101, 16);
			this.labelNextChargeDate.TabIndex = 145;
			this.labelNextChargeDate.Text = "Next Charge Date";
			this.labelNextChargeDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNextChargeDate
			// 
			this.textNextChargeDate.Location = new System.Drawing.Point(406, 79);
			this.textNextChargeDate.Name = "textNextChargeDate";
			this.textNextChargeDate.ReadOnly = true;
			this.textNextChargeDate.Size = new System.Drawing.Size(82, 20);
			this.textNextChargeDate.TabIndex = 17;
			this.textNextChargeDate.TabStop = false;
			// 
			// labelPreviousStartDate
			// 
			this.labelPreviousStartDate.Location = new System.Drawing.Point(302, 107);
			this.labelPreviousStartDate.Name = "labelPreviousStartDate";
			this.labelPreviousStartDate.Size = new System.Drawing.Size(104, 16);
			this.labelPreviousStartDate.TabIndex = 143;
			this.labelPreviousStartDate.Text = "Previous Start Date";
			this.labelPreviousStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelPreviousStartDate.Visible = false;
			// 
			// textPreviousStartDate
			// 
			this.textPreviousStartDate.Location = new System.Drawing.Point(406, 105);
			this.textPreviousStartDate.Name = "textPreviousStartDate";
			this.textPreviousStartDate.ReadOnly = true;
			this.textPreviousStartDate.Size = new System.Drawing.Size(82, 20);
			this.textPreviousStartDate.TabIndex = 18;
			this.textPreviousStartDate.TabStop = false;
			this.textPreviousStartDate.Visible = false;
			// 
			// checkIsRecurringActive
			// 
			this.checkIsRecurringActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsRecurringActive.Location = new System.Drawing.Point(322, 49);
			this.checkIsRecurringActive.Name = "checkIsRecurringActive";
			this.checkIsRecurringActive.Size = new System.Drawing.Size(128, 13);
			this.checkIsRecurringActive.TabIndex = 16;
			this.checkIsRecurringActive.Text = "Is Recurring Active";
			// 
			// comboPaymentType
			// 
			this.comboPaymentType.Location = new System.Drawing.Point(129, 53);
			this.comboPaymentType.Name = "comboPaymentType";
			this.comboPaymentType.Size = new System.Drawing.Size(167, 21);
			this.comboPaymentType.TabIndex = 8;
			this.comboPaymentType.Text = "comboPaymentType";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(2, 54);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(124, 16);
			this.label9.TabIndex = 139;
			this.label9.Text = "Payment Type Override";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkChrgWithNoBal
			// 
			this.checkChrgWithNoBal.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkChrgWithNoBal.Location = new System.Drawing.Point(322, 17);
			this.checkChrgWithNoBal.Name = "checkChrgWithNoBal";
			this.checkChrgWithNoBal.Size = new System.Drawing.Size(128, 26);
			this.checkChrgWithNoBal.TabIndex = 15;
			this.checkChrgWithNoBal.Text = "Run charge even if no family balance present";
			// 
			// labelPayPlan
			// 
			this.labelPayPlan.Location = new System.Drawing.Point(21, 27);
			this.labelPayPlan.Name = "labelPayPlan";
			this.labelPayPlan.Size = new System.Drawing.Size(106, 16);
			this.labelPayPlan.TabIndex = 132;
			this.labelPayPlan.Text = "Payment Plan";
			this.labelPayPlan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPaymentPlans
			// 
			this.comboPaymentPlans.Location = new System.Drawing.Point(129, 26);
			this.comboPaymentPlans.Name = "comboPaymentPlans";
			this.comboPaymentPlans.Size = new System.Drawing.Size(167, 21);
			this.comboPaymentPlans.TabIndex = 7;
			// 
			// butToday
			// 
			this.butToday.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butToday.Location = new System.Drawing.Point(233, 106);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(63, 22);
			this.butToday.TabIndex = 12;
			this.butToday.TabStop = false;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butClear
			// 
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(233, 80);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(63, 22);
			this.butClear.TabIndex = 10;
			this.butClear.TabStop = false;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(129, 165);
			this.textNote.MaxLength = 10000;
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.FinancialNotes;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(344, 64);
			this.textNote.TabIndex = 14;
			this.textNote.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(27, 165);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(99, 16);
			this.label7.TabIndex = 74;
			this.label7.Text = "Note";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textChargeAmt
			// 
			this.textChargeAmt.Location = new System.Drawing.Point(129, 81);
			this.textChargeAmt.MaxVal = 100000000D;
			this.textChargeAmt.MinVal = -100000000D;
			this.textChargeAmt.Name = "textChargeAmt";
			this.textChargeAmt.Size = new System.Drawing.Size(88, 20);
			this.textChargeAmt.TabIndex = 9;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(21, 137);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 16);
			this.label1.TabIndex = 72;
			this.label1.Text = "Date Stop";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(21, 82);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(106, 16);
			this.label5.TabIndex = 67;
			this.label5.Text = "Charge Amount";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(21, 109);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 16);
			this.label2.TabIndex = 70;
			this.label2.Text = "Date Start";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(21, 627);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 32;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(141, 90);
			this.textZip.MaxLength = 100;
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(136, 20);
			this.textZip.TabIndex = 6;
			this.textZip.TabStop = false;
			// 
			// labelAddress
			// 
			this.labelAddress.Location = new System.Drawing.Point(40, 65);
			this.labelAddress.Name = "labelAddress";
			this.labelAddress.Size = new System.Drawing.Size(99, 16);
			this.labelAddress.TabIndex = 63;
			this.labelAddress.Text = "Address";
			this.labelAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(141, 64);
			this.textAddress.MaxLength = 100;
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(365, 20);
			this.textAddress.TabIndex = 5;
			// 
			// labelZip
			// 
			this.labelZip.Location = new System.Drawing.Point(44, 91);
			this.labelZip.Name = "labelZip";
			this.labelZip.Size = new System.Drawing.Size(96, 16);
			this.labelZip.TabIndex = 66;
			this.labelZip.Text = "Zip";
			this.labelZip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(351, 627);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 30;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(432, 627);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 31;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textExpDate
			// 
			this.textExpDate.Location = new System.Drawing.Point(141, 38);
			this.textExpDate.Name = "textExpDate";
			this.textExpDate.Size = new System.Drawing.Size(71, 20);
			this.textExpDate.TabIndex = 3;
			this.textExpDate.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(56, 39);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "Exp (MMYY)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCardNumber
			// 
			this.textCardNumber.Location = new System.Drawing.Point(141, 12);
			this.textCardNumber.Name = "textCardNumber";
			this.textCardNumber.ReadOnly = true;
			this.textCardNumber.Size = new System.Drawing.Size(136, 20);
			this.textCardNumber.TabIndex = 1;
			this.textCardNumber.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Card Number";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCreditCardEdit
			// 
			this.ClientSize = new System.Drawing.Size(524, 657);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.textClinic);
			this.Controls.Add(this.groupChargeFrequency);
			this.Controls.Add(this.textAccountType);
			this.Controls.Add(this.labelAcctType);
			this.Controls.Add(this.groupProcedures);
			this.Controls.Add(this.groupRecurringCharges);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textZip);
			this.Controls.Add(this.labelAddress);
			this.Controls.Add(this.textAddress);
			this.Controls.Add(this.labelZip);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textExpDate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textCardNumber);
			this.Controls.Add(this.label3);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCreditCardEdit";
			this.Text = "Credit Card Edit";
			this.Load += new System.EventHandler(this.FormCreditCardEdit_Load);
			this.groupChargeFrequency.ResumeLayout(false);
			this.groupChargeFrequency.PerformLayout();
			this.groupProcedures.ResumeLayout(false);
			this.groupRecurringCharges.ResumeLayout(false);
			this.groupRecurringCharges.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textCardNumber;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textExpDate;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.Label labelAddress;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.Label labelZip;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private ValidDouble textChargeAmt;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.GroupBox groupRecurringCharges;
		private ODtextBox textNote;
		private System.Windows.Forms.Label label7;
		private UI.Button butClear;
		private UI.Button butToday;
		private System.Windows.Forms.Label labelPayPlan;
		private OpenDental.UI.ComboBox comboPaymentPlans;
		private UI.Button butRemoveProc;
		private UI.Button butAddProc;
		private OpenDental.UI.ListBox listProcs;
		private System.Windows.Forms.Label label15;
		private OpenDental.UI.GroupBox groupProcedures;
		private OpenDental.UI.CheckBox checkExcludeProcSync;
		private System.Windows.Forms.TextBox textAccountType;
		private System.Windows.Forms.Label labelAcctType;
		private OpenDental.UI.GroupBox groupChargeFrequency;
		private System.Windows.Forms.Label labelFrequencyInWords;
		private System.Windows.Forms.RadioButton radioWeekDay;
		private System.Windows.Forms.RadioButton radioDayOfMonth;
		private System.Windows.Forms.TextBox textDayOfMonth;
		private OpenDental.UI.ComboBox comboDays;
		private OpenDental.UI.ComboBox comboFrequency;
		private UI.Button butAddDay;
		private UI.Button butClearDayOfMonth;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.CheckBox checkChrgWithNoBal;
		private UI.ComboBox comboPaymentType;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textClinic;
		private System.Windows.Forms.Label labelClinic;
		private OpenDental.UI.CheckBox checkIsRecurringActive;
		private System.Windows.Forms.Label labelNextChargeDate;
		private ValidDate textNextChargeDate;
		private System.Windows.Forms.Label labelPreviousStartDate;
		private ValidDate textPreviousStartDate;
		private ValidDate textDateStop;
		private ValidDate textDateStart;
	}
}