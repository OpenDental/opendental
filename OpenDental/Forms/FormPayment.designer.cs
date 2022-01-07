namespace OpenDental {
	public partial class FormPayment {
		#region UI Elements
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textCheckNum;
		private System.Windows.Forms.TextBox textBankBranch;
		private System.ComponentModel.IContainer components=null;
		private OpenDental.ValidDate textDate;
		private OpenDental.ValidDouble textAmount;
		private OpenDental.UI.ListBoxOD listPayType;
		private OpenDental.UI.Button butDeletePayment;
		private OpenDental.ODtextBox textNote;//(not including discounts)
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textPaidBy;
		private UI.ComboBoxClinicPicker comboClinic;
		private OpenDental.ValidDate textDateEntry;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label labelDepositAccount;
		private System.Windows.Forms.ComboBox comboDepositAccount;
		private System.Windows.Forms.Panel panelXcharge;
		private System.Windows.Forms.ContextMenu contextMenuXcharge;
		private System.Windows.Forms.MenuItem menuXcharge;
		private System.Windows.Forms.TextBox textDepositAccount;
		private System.Windows.Forms.TextBox textDeposit;
		private System.Windows.Forms.Label labelDeposit;
		private System.Windows.Forms.CheckBox checkPayTypeNone;
		private OpenDental.UI.Button butPayConnect;
		private System.Windows.Forms.ContextMenu contextMenuPayConnect;
		private System.Windows.Forms.MenuItem menuPayConnect;
		private UI.ComboBoxOD comboCreditCards;
		private System.Windows.Forms.Label labelCreditCards;
		private System.Windows.Forms.CheckBox checkRecurring;
		private UI.Button butPrintReceipt;
		private System.Windows.Forms.GroupBox groupXWeb;
		private UI.Button butReturn;
		private UI.Button butVoid;
		private UI.Button butPrePay;
		private System.Windows.Forms.CheckBox checkProcessed;
		private UI.Button butEmailReceipt;
		private System.Windows.Forms.Label labelPayPlan;
		private System.Windows.Forms.TabControl tabControlSplits;
		private System.Windows.Forms.TabPage tabPageSplits;
		private UI.GridOD gridSplits;
		private UI.GridOD gridCharges;
		private UI.Button butAddManual;
		private UI.Button butCreatePartial;
		private System.Windows.Forms.TextBox textSplitTotal;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox checkShowAll;
		private UI.Button butPay;
		private UI.Button butClear;
		private UI.Button butDelete;
		private System.Windows.Forms.ComboBox comboGroupBy;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textChargeTotal;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textFilterProcCodes;
		private System.Windows.Forms.Label labelProcCodes;
		private System.Windows.Forms.Label labelProvFilter;
		private System.Windows.Forms.Label labelClinicFilter;
		private UI.ComboBoxOD comboClinicFilter;
		private System.Windows.Forms.Label labelPatFilter;
		private UI.ComboBoxOD comboPatientFilter;
		private System.Windows.Forms.NumericUpDown amtMaxEnd;
		private System.Windows.Forms.Label labelMaxAmount;
		private System.Windows.Forms.NumericUpDown amtMinEnd;
		private UI.Button button1;
		private System.Windows.Forms.Label labelMinFilter;
		private System.Windows.Forms.Label labelTypeFilter;
		private UI.ComboBoxOD comboTypeFilter;
		private System.Windows.Forms.GroupBox groupBoxFiltering;
		private System.Windows.Forms.Label labelDateFrom;
		private System.Windows.Forms.DateTimePicker datePickTo;
		private System.Windows.Forms.DateTimePicker datePickFrom;
		private UI.ComboBoxOD comboProviderFilter;
		private System.Windows.Forms.CheckBox checkShowSuperfamily;
		private System.Windows.Forms.Label label9;
		private UI.Button butShowHide;
		private System.Windows.Forms.CheckBox checkIncludeExplicitCreditsOnly;
		private System.Windows.Forms.TabControl tabProcCharges;
		private System.Windows.Forms.TabPage tabPageCharges;
		private System.Windows.Forms.TabPage tabPageTreatPlan;
		private System.Windows.Forms.Panel butPaySimple;
		private System.Windows.Forms.ContextMenu contextMenuPaySimple;
		private System.Windows.Forms.MenuItem menuPaySimple;
		private UI.GridOD gridTreatPlan;
		private System.Windows.Forms.Label labelTPProcWarning;
		private System.Windows.Forms.Panel butCareCredit;
		#endregion

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayment));
			this.contextMenuXcharge = new System.Windows.Forms.ContextMenu();
			this.menuXcharge = new System.Windows.Forms.MenuItem();
			this.contextMenuPayConnect = new System.Windows.Forms.ContextMenu();
			this.menuPayConnect = new System.Windows.Forms.MenuItem();
			this._pd2 = new System.Drawing.Printing.PrintDocument();
			this.label9 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butDeletePayment = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butPrintReceipt = new OpenDental.UI.Button();
			this.butEmailReceipt = new OpenDental.UI.Button();
			this.butCareCredit = new System.Windows.Forms.Panel();
			this.butPaySimple = new System.Windows.Forms.Panel();
			this.butShowHide = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.textDeposit = new System.Windows.Forms.TextBox();
			this.butPayConnect = new OpenDental.UI.Button();
			this.labelDepositAccount = new System.Windows.Forms.Label();
			this.labelDeposit = new System.Windows.Forms.Label();
			this.textDepositAccount = new System.Windows.Forms.TextBox();
			this.comboCreditCards = new OpenDental.UI.ComboBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.panelXcharge = new System.Windows.Forms.Panel();
			this.panelEdgeExpress = new System.Windows.Forms.Panel();
			this.labelCreditCards = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboDepositAccount = new System.Windows.Forms.ComboBox();
			this.checkRecurring = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateEntry = new OpenDental.ValidDate();
			this.butPrePay = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.checkProcessed = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupXWeb = new System.Windows.Forms.GroupBox();
			this.butReturn = new OpenDental.UI.Button();
			this.butVoid = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.textPaidBy = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.listPayType = new OpenDental.UI.ListBoxOD();
			this.textNote = new OpenDental.ODtextBox();
			this.textAmount = new OpenDental.ValidDouble();
			this.textCheckNum = new System.Windows.Forms.TextBox();
			this.textDate = new OpenDental.ValidDate();
			this.textBankBranch = new System.Windows.Forms.TextBox();
			this.checkPayTypeNone = new System.Windows.Forms.CheckBox();
			this.tabProcCharges = new System.Windows.Forms.TabControl();
			this.tabPageCharges = new System.Windows.Forms.TabPage();
			this.checkShowSuperfamily = new System.Windows.Forms.CheckBox();
			this.gridCharges = new OpenDental.UI.GridOD();
			this.checkIncludeExplicitCreditsOnly = new System.Windows.Forms.CheckBox();
			this.checkShowAll = new System.Windows.Forms.CheckBox();
			this.tabPageTreatPlan = new System.Windows.Forms.TabPage();
			this.labelTPProcWarning = new System.Windows.Forms.Label();
			this.gridTreatPlan = new OpenDental.UI.GridOD();
			this.butPay = new OpenDental.UI.Button();
			this.labelPayPlan = new System.Windows.Forms.Label();
			this.tabControlSplits = new System.Windows.Forms.TabControl();
			this.tabPageSplits = new System.Windows.Forms.TabPage();
			this.gridSplits = new OpenDental.UI.GridOD();
			this.textChargeTotal = new System.Windows.Forms.TextBox();
			this.butCreatePartial = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.comboGroupBy = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBoxFiltering = new System.Windows.Forms.GroupBox();
			this.comboProviderFilter = new OpenDental.UI.ComboBoxOD();
			this.datePickTo = new System.Windows.Forms.DateTimePicker();
			this.labelDateFrom = new System.Windows.Forms.Label();
			this.datePickFrom = new System.Windows.Forms.DateTimePicker();
			this.textFilterProcCodes = new System.Windows.Forms.TextBox();
			this.labelMaxAmount = new System.Windows.Forms.Label();
			this.labelMinFilter = new System.Windows.Forms.Label();
			this.labelTypeFilter = new System.Windows.Forms.Label();
			this.amtMinEnd = new System.Windows.Forms.NumericUpDown();
			this.amtMaxEnd = new System.Windows.Forms.NumericUpDown();
			this.button1 = new OpenDental.UI.Button();
			this.comboPatientFilter = new OpenDental.UI.ComboBoxOD();
			this.labelProcCodes = new System.Windows.Forms.Label();
			this.comboTypeFilter = new OpenDental.UI.ComboBoxOD();
			this.comboClinicFilter = new OpenDental.UI.ComboBoxOD();
			this.labelPatFilter = new System.Windows.Forms.Label();
			this.labelClinicFilter = new System.Windows.Forms.Label();
			this.labelProvFilter = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.textSplitTotal = new System.Windows.Forms.TextBox();
			this.butAddManual = new OpenDental.UI.Button();
			this.contextMenuPaySimple = new System.Windows.Forms.ContextMenu();
			this.menuPaySimple = new System.Windows.Forms.MenuItem();
			this.panelSplits = new System.Windows.Forms.Panel();
			this.labelRecurringChargeWarning = new System.Windows.Forms.Label();
			this.groupXWeb.SuspendLayout();
			this.tabProcCharges.SuspendLayout();
			this.tabPageCharges.SuspendLayout();
			this.tabPageTreatPlan.SuspendLayout();
			this.tabControlSplits.SuspendLayout();
			this.tabPageSplits.SuspendLayout();
			this.groupBoxFiltering.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.amtMinEnd)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.amtMaxEnd)).BeginInit();
			this.panelSplits.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuXcharge
			// 
			this.contextMenuXcharge.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuXcharge});
			// 
			// menuXcharge
			// 
			this.menuXcharge.Index = 0;
			this.menuXcharge.Text = "Settings";
			this.menuXcharge.Click += new System.EventHandler(this.menuXcharge_Click);
			// 
			// contextMenuPayConnect
			// 
			this.contextMenuPayConnect.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuPayConnect});
			// 
			// menuPayConnect
			// 
			this.menuPayConnect.Index = 0;
			this.menuPayConnect.Text = "Settings";
			this.menuPayConnect.Click += new System.EventHandler(this.menuPayConnect_Click);
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label9.Location = new System.Drawing.Point(100, 658);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(165, 30);
			this.label9.TabIndex = 141;
			this.label9.Text = "Deletes entire payment \r\nand all splits";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(943, 664);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 998;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDeletePayment
			// 
			this.butDeletePayment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeletePayment.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeletePayment.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeletePayment.Location = new System.Drawing.Point(11, 664);
			this.butDeletePayment.Name = "butDeletePayment";
			this.butDeletePayment.Size = new System.Drawing.Size(84, 24);
			this.butDeletePayment.TabIndex = 997;
			this.butDeletePayment.Text = "&Delete";
			this.butDeletePayment.Click += new System.EventHandler(this.butDeletePayment_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1022, 664);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 999;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPrintReceipt
			// 
			this.butPrintReceipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintReceipt.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrintReceipt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintReceipt.Location = new System.Drawing.Point(389, 664);
			this.butPrintReceipt.Name = "butPrintReceipt";
			this.butPrintReceipt.Size = new System.Drawing.Size(101, 24);
			this.butPrintReceipt.TabIndex = 135;
			this.butPrintReceipt.TabStop = false;
			this.butPrintReceipt.Text = "&Print Receipt";
			this.butPrintReceipt.Visible = false;
			this.butPrintReceipt.Click += new System.EventHandler(this.butPrintReceipt_Click);
			// 
			// butEmailReceipt
			// 
			this.butEmailReceipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEmailReceipt.Icon = OpenDental.UI.EnumIcons.Email;
			this.butEmailReceipt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmailReceipt.Location = new System.Drawing.Point(499, 664);
			this.butEmailReceipt.Name = "butEmailReceipt";
			this.butEmailReceipt.Size = new System.Drawing.Size(113, 24);
			this.butEmailReceipt.TabIndex = 140;
			this.butEmailReceipt.Text = "&E-Mail Receipt";
			this.butEmailReceipt.Visible = false;
			this.butEmailReceipt.Click += new System.EventHandler(this.butEmailReceipt_Click);
			// 
			// butCareCredit
			// 
			this.butCareCredit.BackgroundImage = global::OpenDental.Properties.Resources.CareCredit_Button_NoTag_26x90;
			this.butCareCredit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.butCareCredit.Location = new System.Drawing.Point(651, 17);
			this.butCareCredit.Name = "butCareCredit";
			this.butCareCredit.Size = new System.Drawing.Size(90, 26);
			this.butCareCredit.TabIndex = 143;
			this.butCareCredit.Visible = false;
			this.butCareCredit.MouseClick += new System.Windows.Forms.MouseEventHandler(this.butCareCredit_Click);
			// 
			// butPaySimple
			// 
			this.butPaySimple.BackgroundImage = global::OpenDental.Properties.Resources.PaySimple_Button_2019_26x75;
			this.butPaySimple.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.butPaySimple.Location = new System.Drawing.Point(877, 17);
			this.butPaySimple.Name = "butPaySimple";
			this.butPaySimple.Size = new System.Drawing.Size(75, 26);
			this.butPaySimple.TabIndex = 140;
			this.butPaySimple.Visible = false;
			this.butPaySimple.MouseClick += new System.Windows.Forms.MouseEventHandler(this.butPaySimple_Click);
			// 
			// butShowHide
			// 
			this.butShowHide.Image = global::OpenDental.Properties.Resources.arrowDownTriangle;
			this.butShowHide.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butShowHide.Location = new System.Drawing.Point(1, 215);
			this.butShowHide.Name = "butShowHide";
			this.butShowHide.Size = new System.Drawing.Size(91, 24);
			this.butShowHide.TabIndex = 139;
			this.butShowHide.Text = "Hide Splits";
			this.butShowHide.UseVisualStyleBackColor = true;
			this.butShowHide.Click += new System.EventHandler(this.butShowHide_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.ForceShowUnassigned = true;
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(67, 12);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(235, 21);
			this.comboClinic.TabIndex = 92;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// textDeposit
			// 
			this.textDeposit.Location = new System.Drawing.Point(780, 165);
			this.textDeposit.Name = "textDeposit";
			this.textDeposit.ReadOnly = true;
			this.textDeposit.Size = new System.Drawing.Size(100, 20);
			this.textDeposit.TabIndex = 125;
			// 
			// butPayConnect
			// 
			this.butPayConnect.Location = new System.Drawing.Point(958, 17);
			this.butPayConnect.Name = "butPayConnect";
			this.butPayConnect.Size = new System.Drawing.Size(75, 26);
			this.butPayConnect.TabIndex = 129;
			this.butPayConnect.Text = "PayConnect";
			this.butPayConnect.Visible = false;
			this.butPayConnect.Click += new System.EventHandler(this.butPayConnect_Click);
			// 
			// labelDepositAccount
			// 
			this.labelDepositAccount.Location = new System.Drawing.Point(492, 148);
			this.labelDepositAccount.Name = "labelDepositAccount";
			this.labelDepositAccount.Size = new System.Drawing.Size(260, 17);
			this.labelDepositAccount.TabIndex = 114;
			this.labelDepositAccount.Text = "Pay into Account";
			this.labelDepositAccount.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelDeposit
			// 
			this.labelDeposit.ForeColor = System.Drawing.Color.Firebrick;
			this.labelDeposit.Location = new System.Drawing.Point(777, 146);
			this.labelDeposit.Name = "labelDeposit";
			this.labelDeposit.Size = new System.Drawing.Size(199, 16);
			this.labelDeposit.TabIndex = 126;
			this.labelDeposit.Text = "Attached to deposit";
			this.labelDeposit.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDepositAccount
			// 
			this.textDepositAccount.Location = new System.Drawing.Point(492, 187);
			this.textDepositAccount.Name = "textDepositAccount";
			this.textDepositAccount.ReadOnly = true;
			this.textDepositAccount.Size = new System.Drawing.Size(260, 20);
			this.textDepositAccount.TabIndex = 119;
			// 
			// comboCreditCards
			// 
			this.comboCreditCards.Location = new System.Drawing.Point(780, 69);
			this.comboCreditCards.Name = "comboCreditCards";
			this.comboCreditCards.Size = new System.Drawing.Size(270, 21);
			this.comboCreditCards.TabIndex = 130;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(489, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Payment Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// panelXcharge
			// 
			this.panelXcharge.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelXcharge.BackgroundImage")));
			this.panelXcharge.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panelXcharge.Location = new System.Drawing.Point(812, 17);
			this.panelXcharge.Name = "panelXcharge";
			this.panelXcharge.Size = new System.Drawing.Size(59, 26);
			this.panelXcharge.TabIndex = 118;
			this.panelXcharge.Visible = false;
			this.panelXcharge.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelXcharge_MouseClick);
			// 
			// panelEdgeExpress
			// 
			this.panelEdgeExpress.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelEdgeExpress.BackgroundImage")));
			this.panelEdgeExpress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panelEdgeExpress.Location = new System.Drawing.Point(747, 17);
			this.panelEdgeExpress.Name = "panelEdgeExpress";
			this.panelEdgeExpress.Size = new System.Drawing.Size(59, 26);
			this.panelEdgeExpress.TabIndex = 119;
			this.panelEdgeExpress.Visible = false;
			this.panelEdgeExpress.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelEdgeExpress_MouseClick);
			// 
			// labelCreditCards
			// 
			this.labelCreditCards.Location = new System.Drawing.Point(780, 49);
			this.labelCreditCards.Name = "labelCreditCards";
			this.labelCreditCards.Size = new System.Drawing.Size(198, 17);
			this.labelCreditCards.TabIndex = 131;
			this.labelCreditCards.Text = "Credit Card";
			this.labelCreditCards.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 156);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboDepositAccount
			// 
			this.comboDepositAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDepositAccount.FormattingEnabled = true;
			this.comboDepositAccount.Location = new System.Drawing.Point(492, 165);
			this.comboDepositAccount.Name = "comboDepositAccount";
			this.comboDepositAccount.Size = new System.Drawing.Size(260, 21);
			this.comboDepositAccount.TabIndex = 113;
			// 
			// checkRecurring
			// 
			this.checkRecurring.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRecurring.Location = new System.Drawing.Point(780, 108);
			this.checkRecurring.Name = "checkRecurring";
			this.checkRecurring.Size = new System.Drawing.Size(196, 18);
			this.checkRecurring.TabIndex = 132;
			this.checkRecurring.Text = "Apply to Recurring Charge";
			this.checkRecurring.Click += new System.EventHandler(this.checkRecurring_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(2, 138);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 9;
			this.label3.Text = "Bank-Branch";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(104, 54);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(100, 20);
			this.textDateEntry.TabIndex = 93;
			// 
			// butPrePay
			// 
			this.butPrePay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrePay.Location = new System.Drawing.Point(205, 94);
			this.butPrePay.Name = "butPrePay";
			this.butPrePay.Size = new System.Drawing.Size(61, 20);
			this.butPrePay.TabIndex = 136;
			this.butPrePay.Text = "Prepay";
			this.butPrePay.Click += new System.EventHandler(this.butPrePay_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(2, 118);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "Check #";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(2, 58);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(100, 16);
			this.label12.TabIndex = 94;
			this.label12.Text = "Entry Date";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkProcessed
			// 
			this.checkProcessed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProcessed.Location = new System.Drawing.Point(780, 127);
			this.checkProcessed.Name = "checkProcessed";
			this.checkProcessed.Size = new System.Drawing.Size(196, 18);
			this.checkProcessed.TabIndex = 137;
			this.checkProcessed.Text = "Mark as Processed";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(2, 98);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 11;
			this.label5.Text = "Amount";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupXWeb
			// 
			this.groupXWeb.Controls.Add(this.butReturn);
			this.groupXWeb.Controls.Add(this.butVoid);
			this.groupXWeb.Location = new System.Drawing.Point(642, 59);
			this.groupXWeb.Name = "groupXWeb";
			this.groupXWeb.Size = new System.Drawing.Size(110, 85);
			this.groupXWeb.TabIndex = 138;
			this.groupXWeb.TabStop = false;
			this.groupXWeb.Text = "XWeb";
			// 
			// butReturn
			// 
			this.butReturn.Location = new System.Drawing.Point(17, 20);
			this.butReturn.Name = "butReturn";
			this.butReturn.Size = new System.Drawing.Size(75, 24);
			this.butReturn.TabIndex = 140;
			this.butReturn.Text = "Return";
			this.butReturn.Click += new System.EventHandler(this.butReturn_Click);
			// 
			// butVoid
			// 
			this.butVoid.Location = new System.Drawing.Point(17, 49);
			this.butVoid.Name = "butVoid";
			this.butVoid.Size = new System.Drawing.Size(75, 24);
			this.butVoid.TabIndex = 139;
			this.butVoid.Text = "Void";
			this.butVoid.Click += new System.EventHandler(this.butVoid_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(2, 78);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 16);
			this.label6.TabIndex = 12;
			this.label6.Text = "Payment Date";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPaidBy
			// 
			this.textPaidBy.Location = new System.Drawing.Point(104, 34);
			this.textPaidBy.Name = "textPaidBy";
			this.textPaidBy.ReadOnly = true;
			this.textPaidBy.Size = new System.Drawing.Size(242, 20);
			this.textPaidBy.TabIndex = 32;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(2, 36);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(100, 16);
			this.label11.TabIndex = 33;
			this.label11.Text = "Paid By";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listPayType
			// 
			this.listPayType.Location = new System.Drawing.Point(492, 49);
			this.listPayType.Name = "listPayType";
			this.listPayType.Size = new System.Drawing.Size(120, 95);
			this.listPayType.TabIndex = 4;
			this.listPayType.Click += new System.EventHandler(this.listPayType_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(104, 156);
			this.textNote.MaxLength = 4000;
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Payment;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(361, 83);
			this.textNote.SpellCheckIsEnabled = false;
			this.textNote.TabIndex = 1;
			this.textNote.TabStop = false;
			this.textNote.Text = "";
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(104, 94);
			this.textAmount.MaxVal = 100000000D;
			this.textAmount.MinVal = -100000000D;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(100, 20);
			this.textAmount.TabIndex = 2;
			// 
			// textCheckNum
			// 
			this.textCheckNum.Location = new System.Drawing.Point(104, 114);
			this.textCheckNum.Name = "textCheckNum";
			this.textCheckNum.Size = new System.Drawing.Size(100, 20);
			this.textCheckNum.TabIndex = 0;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(104, 74);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 4;
			// 
			// textBankBranch
			// 
			this.textBankBranch.Location = new System.Drawing.Point(104, 134);
			this.textBankBranch.Name = "textBankBranch";
			this.textBankBranch.Size = new System.Drawing.Size(100, 20);
			this.textBankBranch.TabIndex = 1;
			// 
			// checkPayTypeNone
			// 
			this.checkPayTypeNone.Location = new System.Drawing.Point(492, 31);
			this.checkPayTypeNone.Name = "checkPayTypeNone";
			this.checkPayTypeNone.Size = new System.Drawing.Size(155, 18);
			this.checkPayTypeNone.TabIndex = 128;
			this.checkPayTypeNone.Text = "None (Income Transfer)";
			this.checkPayTypeNone.UseVisualStyleBackColor = true;
			this.checkPayTypeNone.CheckedChanged += new System.EventHandler(this.checkPayTypeNone_CheckedChanged);
			this.checkPayTypeNone.Click += new System.EventHandler(this.checkPayTypeNone_Click);
			// 
			// tabProcCharges
			// 
			this.tabProcCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabProcCharges.Controls.Add(this.tabPageCharges);
			this.tabProcCharges.Controls.Add(this.tabPageTreatPlan);
			this.tabProcCharges.Location = new System.Drawing.Point(498, 78);
			this.tabProcCharges.Name = "tabProcCharges";
			this.tabProcCharges.SelectedIndex = 0;
			this.tabProcCharges.Size = new System.Drawing.Size(608, 296);
			this.tabProcCharges.TabIndex = 1;
			// 
			// tabPageCharges
			// 
			this.tabPageCharges.Controls.Add(this.checkShowSuperfamily);
			this.tabPageCharges.Controls.Add(this.gridCharges);
			this.tabPageCharges.Controls.Add(this.checkIncludeExplicitCreditsOnly);
			this.tabPageCharges.Controls.Add(this.checkShowAll);
			this.tabPageCharges.Location = new System.Drawing.Point(4, 22);
			this.tabPageCharges.Name = "tabPageCharges";
			this.tabPageCharges.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCharges.Size = new System.Drawing.Size(600, 270);
			this.tabPageCharges.TabIndex = 0;
			this.tabPageCharges.Text = "Outstanding";
			this.tabPageCharges.UseVisualStyleBackColor = true;
			this.tabProcCharges.SelectedIndexChanged+=new System.EventHandler(this.TabProcChargesSelectedIndexChanged);
			// 
			// checkShowSuperfamily
			// 
			this.checkShowSuperfamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowSuperfamily.Location = new System.Drawing.Point(230, 2);
			this.checkShowSuperfamily.Name = "checkShowSuperfamily";
			this.checkShowSuperfamily.Size = new System.Drawing.Size(193, 20);
			this.checkShowSuperfamily.TabIndex = 204;
			this.checkShowSuperfamily.Text = "Show Super Family Charges";
			this.checkShowSuperfamily.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowSuperfamily.UseVisualStyleBackColor = true;
			this.checkShowSuperfamily.Click += new System.EventHandler(this.checkShowSuperfamily_Click);
			// 
			// gridCharges
			// 
			this.gridCharges.AllowSortingByColumn = true;
			this.gridCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridCharges.Location = new System.Drawing.Point(1, 22);
			this.gridCharges.Name = "gridCharges";
			this.gridCharges.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridCharges.Size = new System.Drawing.Size(598, 242);
			this.gridCharges.TabIndex = 144;
			this.gridCharges.Title = "Outstanding Charges";
			this.gridCharges.TranslationName = "TableOutstandingCharges";
			this.gridCharges.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCharges_CellClick);
			// 
			// checkIncludeExplicitCreditsOnly
			// 
			this.checkIncludeExplicitCreditsOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeExplicitCreditsOnly.Location = new System.Drawing.Point(4, 2);
			this.checkIncludeExplicitCreditsOnly.Name = "checkIncludeExplicitCreditsOnly";
			this.checkIncludeExplicitCreditsOnly.Size = new System.Drawing.Size(218, 20);
			this.checkIncludeExplicitCreditsOnly.TabIndex = 205;
			this.checkIncludeExplicitCreditsOnly.Text = "Show Only Allocated Credits";
			this.checkIncludeExplicitCreditsOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeExplicitCreditsOnly.UseVisualStyleBackColor = true;
			this.checkIncludeExplicitCreditsOnly.Click += new System.EventHandler(this.CheckIncludeExplicitCreditsOnly_Click);
			// 
			// checkShowAll
			// 
			this.checkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAll.Location = new System.Drawing.Point(431, 2);
			this.checkShowAll.Name = "checkShowAll";
			this.checkShowAll.Size = new System.Drawing.Size(148, 20);
			this.checkShowAll.TabIndex = 147;
			this.checkShowAll.Text = "Show All Charges";
			this.checkShowAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAll.UseVisualStyleBackColor = true;
			this.checkShowAll.Click += new System.EventHandler(this.checkShowAll_Clicked);
			// 
			// tabPageTreatPlan
			// 
			this.tabPageTreatPlan.Controls.Add(this.labelTPProcWarning);
			this.tabPageTreatPlan.Controls.Add(this.gridTreatPlan);
			this.tabPageTreatPlan.Location = new System.Drawing.Point(4, 22);
			this.tabPageTreatPlan.Name = "tabPageTreatPlan";
			this.tabPageTreatPlan.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageTreatPlan.Size = new System.Drawing.Size(600, 270);
			this.tabPageTreatPlan.TabIndex = 1;
			this.tabPageTreatPlan.Text = "Treat Plan";
			this.tabPageTreatPlan.UseVisualStyleBackColor = true;
			// 
			// labelTPProcWarning
			// 
			this.labelTPProcWarning.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelTPProcWarning.Location = new System.Drawing.Point(2, 4);
			this.labelTPProcWarning.Name = "labelTPProcWarning";
			this.labelTPProcWarning.Size = new System.Drawing.Size(596, 17);
			this.labelTPProcWarning.TabIndex = 146;
			this.labelTPProcWarning.Text = "Treatment Planned Procedures will not be reflected in the total for this account." +
    "";
			this.labelTPProcWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelTPProcWarning.Visible = false;
			// 
			// gridTreatPlan
			// 
			this.gridTreatPlan.AllowSortingByColumn = true;
			this.gridTreatPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridTreatPlan.Location = new System.Drawing.Point(1, 22);
			this.gridTreatPlan.Name = "gridTreatPlan";
			this.gridTreatPlan.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridTreatPlan.Size = new System.Drawing.Size(598, 242);
			this.gridTreatPlan.TabIndex = 145;
			this.gridTreatPlan.Title = "Treatment Planned Procedures";
			this.gridTreatPlan.TranslationName = "TableOutstandingCharges";
			this.gridTreatPlan.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTreatPlan_CellClick);
			// 
			// butPay
			// 
			this.butPay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPay.Image = global::OpenDental.Properties.Resources.Left;
			this.butPay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPay.Location = new System.Drawing.Point(504, 378);
			this.butPay.Name = "butPay";
			this.butPay.Size = new System.Drawing.Size(79, 23);
			this.butPay.TabIndex = 146;
			this.butPay.Text = "Pay";
			this.butPay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butPay.Click += new System.EventHandler(this.butPay_Click);
			// 
			// labelPayPlan
			// 
			this.labelPayPlan.Location = new System.Drawing.Point(258, 101);
			this.labelPayPlan.Name = "labelPayPlan";
			this.labelPayPlan.Size = new System.Drawing.Size(231, 17);
			this.labelPayPlan.TabIndex = 141;
			this.labelPayPlan.Text = "splits attached to payment plan.";
			this.labelPayPlan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelPayPlan.Visible = false;
			// 
			// tabControlSplits
			// 
			this.tabControlSplits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.tabControlSplits.Controls.Add(this.tabPageSplits);
			this.tabControlSplits.Location = new System.Drawing.Point(9, 100);
			this.tabControlSplits.Name = "tabControlSplits";
			this.tabControlSplits.SelectedIndex = 0;
			this.tabControlSplits.Size = new System.Drawing.Size(487, 274);
			this.tabControlSplits.TabIndex = 5;
			// 
			// tabPageSplits
			// 
			this.tabPageSplits.Controls.Add(this.gridSplits);
			this.tabPageSplits.Location = new System.Drawing.Point(4, 22);
			this.tabPageSplits.Name = "tabPageSplits";
			this.tabPageSplits.Size = new System.Drawing.Size(479, 248);
			this.tabPageSplits.TabIndex = 0;
			this.tabPageSplits.Text = "Splits";
			this.tabPageSplits.UseVisualStyleBackColor = true;
			// 
			// gridSplits
			// 
			this.gridSplits.AllowSortingByColumn = true;
			this.gridSplits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridSplits.Location = new System.Drawing.Point(0, 0);
			this.gridSplits.Name = "gridSplits";
			this.gridSplits.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridSplits.Size = new System.Drawing.Size(479, 242);
			this.gridSplits.TabIndex = 0;
			this.gridSplits.Title = "Current Payment Splits";
			this.gridSplits.TranslationName = "TableCurrentSplits";
			this.gridSplits.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSplits_CellDoubleClick);
			this.gridSplits.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSplits_CellClick);
			// 
			// textChargeTotal
			// 
			this.textChargeTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textChargeTotal.Location = new System.Drawing.Point(1051, 379);
			this.textChargeTotal.Name = "textChargeTotal";
			this.textChargeTotal.ReadOnly = true;
			this.textChargeTotal.Size = new System.Drawing.Size(51, 20);
			this.textChargeTotal.TabIndex = 156;
			this.textChargeTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butCreatePartial
			// 
			this.butCreatePartial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCreatePartial.Icon = OpenDental.UI.EnumIcons.Add;
			this.butCreatePartial.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCreatePartial.Location = new System.Drawing.Point(593, 378);
			this.butCreatePartial.Name = "butCreatePartial";
			this.butCreatePartial.Size = new System.Drawing.Size(114, 23);
			this.butCreatePartial.TabIndex = 150;
			this.butCreatePartial.Text = "Add Partials";
			this.butCreatePartial.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butCreatePartial.Click += new System.EventHandler(this.butCreatePartialSplit_Click);
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(992, 380);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(58, 18);
			this.label8.TabIndex = 155;
			this.label8.Text = "Total";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(11, 377);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(100, 24);
			this.butDelete.TabIndex = 144;
			this.butDelete.Text = "Delete Splits";
			this.butDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butDelete.Click += new System.EventHandler(this.butDeleteSplits_Click);
			// 
			// comboGroupBy
			// 
			this.comboGroupBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGroupBy.Location = new System.Drawing.Point(795, 379);
			this.comboGroupBy.MaxDropDownItems = 30;
			this.comboGroupBy.Name = "comboGroupBy";
			this.comboGroupBy.Size = new System.Drawing.Size(111, 21);
			this.comboGroupBy.TabIndex = 153;
			this.comboGroupBy.SelectionChangeCommitted += new System.EventHandler(this.comboGroupBy_SelectionChangeCommitted);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(717, 380);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 18);
			this.label7.TabIndex = 154;
			this.label7.Text = "Group By";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxFiltering
			// 
			this.groupBoxFiltering.Controls.Add(this.comboProviderFilter);
			this.groupBoxFiltering.Controls.Add(this.datePickTo);
			this.groupBoxFiltering.Controls.Add(this.labelDateFrom);
			this.groupBoxFiltering.Controls.Add(this.datePickFrom);
			this.groupBoxFiltering.Controls.Add(this.textFilterProcCodes);
			this.groupBoxFiltering.Controls.Add(this.labelMaxAmount);
			this.groupBoxFiltering.Controls.Add(this.labelMinFilter);
			this.groupBoxFiltering.Controls.Add(this.labelTypeFilter);
			this.groupBoxFiltering.Controls.Add(this.amtMinEnd);
			this.groupBoxFiltering.Controls.Add(this.amtMaxEnd);
			this.groupBoxFiltering.Controls.Add(this.button1);
			this.groupBoxFiltering.Controls.Add(this.comboPatientFilter);
			this.groupBoxFiltering.Controls.Add(this.labelProcCodes);
			this.groupBoxFiltering.Controls.Add(this.comboTypeFilter);
			this.groupBoxFiltering.Controls.Add(this.comboClinicFilter);
			this.groupBoxFiltering.Controls.Add(this.labelPatFilter);
			this.groupBoxFiltering.Controls.Add(this.labelClinicFilter);
			this.groupBoxFiltering.Controls.Add(this.labelProvFilter);
			this.groupBoxFiltering.Location = new System.Drawing.Point(498, -1);
			this.groupBoxFiltering.Name = "groupBoxFiltering";
			this.groupBoxFiltering.Size = new System.Drawing.Size(603, 80);
			this.groupBoxFiltering.TabIndex = 203;
			this.groupBoxFiltering.TabStop = false;
			this.groupBoxFiltering.Text = "Filtering";
			// 
			// comboProviderFilter
			// 
			this.comboProviderFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboProviderFilter.Location = new System.Drawing.Point(106, 33);
			this.comboProviderFilter.Name = "comboProviderFilter";
			this.comboProviderFilter.SelectionModeMulti = true;
			this.comboProviderFilter.Size = new System.Drawing.Size(90, 21);
			this.comboProviderFilter.TabIndex = 207;
			this.comboProviderFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// datePickTo
			// 
			this.datePickTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickTo.Location = new System.Drawing.Point(199, 56);
			this.datePickTo.Name = "datePickTo";
			this.datePickTo.Size = new System.Drawing.Size(90, 20);
			this.datePickTo.TabIndex = 204;
			this.datePickTo.ValueChanged += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// labelDateFrom
			// 
			this.labelDateFrom.Location = new System.Drawing.Point(15, 58);
			this.labelDateFrom.Name = "labelDateFrom";
			this.labelDateFrom.Size = new System.Drawing.Size(90, 16);
			this.labelDateFrom.TabIndex = 206;
			this.labelDateFrom.Text = "From/To Dates:";
			this.labelDateFrom.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// datePickFrom
			// 
			this.datePickFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickFrom.Location = new System.Drawing.Point(106, 56);
			this.datePickFrom.Name = "datePickFrom";
			this.datePickFrom.Size = new System.Drawing.Size(90, 20);
			this.datePickFrom.TabIndex = 203;
			this.datePickFrom.ValueChanged += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// textFilterProcCodes
			// 
			this.textFilterProcCodes.Location = new System.Drawing.Point(395, 56);
			this.textFilterProcCodes.Name = "textFilterProcCodes";
			this.textFilterProcCodes.Size = new System.Drawing.Size(90, 20);
			this.textFilterProcCodes.TabIndex = 187;
			this.textFilterProcCodes.TextChanged += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// labelMaxAmount
			// 
			this.labelMaxAmount.Location = new System.Drawing.Point(488, 14);
			this.labelMaxAmount.Name = "labelMaxAmount";
			this.labelMaxAmount.Size = new System.Drawing.Size(89, 18);
			this.labelMaxAmount.TabIndex = 198;
			this.labelMaxAmount.Text = "Amt End Max";
			this.labelMaxAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMinFilter
			// 
			this.labelMinFilter.Location = new System.Drawing.Point(395, 14);
			this.labelMinFilter.Name = "labelMinFilter";
			this.labelMinFilter.Size = new System.Drawing.Size(87, 18);
			this.labelMinFilter.TabIndex = 200;
			this.labelMinFilter.Text = "Amt End Min.";
			this.labelMinFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTypeFilter
			// 
			this.labelTypeFilter.Location = new System.Drawing.Point(199, 14);
			this.labelTypeFilter.Name = "labelTypeFilter";
			this.labelTypeFilter.Size = new System.Drawing.Size(60, 18);
			this.labelTypeFilter.TabIndex = 202;
			this.labelTypeFilter.Text = "Type";
			this.labelTypeFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// amtMinEnd
			// 
			this.amtMinEnd.DecimalPlaces = 2;
			this.amtMinEnd.Location = new System.Drawing.Point(395, 33);
			this.amtMinEnd.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
			this.amtMinEnd.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
			this.amtMinEnd.Name = "amtMinEnd";
			this.amtMinEnd.Size = new System.Drawing.Size(90, 20);
			this.amtMinEnd.TabIndex = 199;
			this.amtMinEnd.ThousandsSeparator = true;
			this.amtMinEnd.ValueChanged += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// amtMaxEnd
			// 
			this.amtMaxEnd.DecimalPlaces = 2;
			this.amtMaxEnd.Location = new System.Drawing.Point(488, 33);
			this.amtMaxEnd.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.amtMaxEnd.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
			this.amtMaxEnd.Name = "amtMaxEnd";
			this.amtMaxEnd.Size = new System.Drawing.Size(90, 20);
			this.amtMaxEnd.TabIndex = 197;
			this.amtMaxEnd.ThousandsSeparator = true;
			this.amtMaxEnd.ValueChanged += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(503, 55);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 24);
			this.button1.TabIndex = 141;
			this.button1.Text = "Refresh";
			this.button1.Click += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// comboPatientFilter
			// 
			this.comboPatientFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboPatientFilter.Location = new System.Drawing.Point(13, 33);
			this.comboPatientFilter.Name = "comboPatientFilter";
			this.comboPatientFilter.SelectionModeMulti = true;
			this.comboPatientFilter.Size = new System.Drawing.Size(90, 21);
			this.comboPatientFilter.TabIndex = 195;
			this.comboPatientFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// labelProcCodes
			// 
			this.labelProcCodes.Location = new System.Drawing.Point(301, 56);
			this.labelProcCodes.Name = "labelProcCodes";
			this.labelProcCodes.Size = new System.Drawing.Size(93, 18);
			this.labelProcCodes.TabIndex = 188;
			this.labelProcCodes.Text = "Proc Codes:";
			this.labelProcCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTypeFilter
			// 
			this.comboTypeFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboTypeFilter.Location = new System.Drawing.Point(199, 33);
			this.comboTypeFilter.Name = "comboTypeFilter";
			this.comboTypeFilter.SelectionModeMulti = true;
			this.comboTypeFilter.Size = new System.Drawing.Size(100, 21);
			this.comboTypeFilter.TabIndex = 201;
			this.comboTypeFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// comboClinicFilter
			// 
			this.comboClinicFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboClinicFilter.Location = new System.Drawing.Point(302, 33);
			this.comboClinicFilter.Name = "comboClinicFilter";
			this.comboClinicFilter.SelectionModeMulti = true;
			this.comboClinicFilter.Size = new System.Drawing.Size(90, 21);
			this.comboClinicFilter.TabIndex = 193;
			this.comboClinicFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommitted);
			// 
			// labelPatFilter
			// 
			this.labelPatFilter.Location = new System.Drawing.Point(13, 14);
			this.labelPatFilter.Name = "labelPatFilter";
			this.labelPatFilter.Size = new System.Drawing.Size(67, 18);
			this.labelPatFilter.TabIndex = 196;
			this.labelPatFilter.Text = "Patients";
			this.labelPatFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClinicFilter
			// 
			this.labelClinicFilter.Location = new System.Drawing.Point(302, 14);
			this.labelClinicFilter.Name = "labelClinicFilter";
			this.labelClinicFilter.Size = new System.Drawing.Size(60, 18);
			this.labelClinicFilter.TabIndex = 194;
			this.labelClinicFilter.Text = "Clinics";
			this.labelClinicFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvFilter
			// 
			this.labelProvFilter.Location = new System.Drawing.Point(106, 14);
			this.labelProvFilter.Name = "labelProvFilter";
			this.labelProvFilter.Size = new System.Drawing.Size(67, 18);
			this.labelProvFilter.TabIndex = 192;
			this.labelProvFilter.Text = "Providers";
			this.labelProvFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butClear
			// 
			this.butClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClear.Location = new System.Drawing.Point(117, 377);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(89, 24);
			this.butClear.TabIndex = 145;
			this.butClear.Text = "Delete All";
			this.butClear.Click += new System.EventHandler(this.butDeleteAllSplits_Click);
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label13.Location = new System.Drawing.Point(382, 380);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(58, 18);
			this.label13.TabIndex = 148;
			this.label13.Text = "Total";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSplitTotal
			// 
			this.textSplitTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSplitTotal.Location = new System.Drawing.Point(441, 379);
			this.textSplitTotal.Name = "textSplitTotal";
			this.textSplitTotal.ReadOnly = true;
			this.textSplitTotal.Size = new System.Drawing.Size(51, 20);
			this.textSplitTotal.TabIndex = 149;
			this.textSplitTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butAddManual
			// 
			this.butAddManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddManual.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddManual.Location = new System.Drawing.Point(212, 377);
			this.butAddManual.Name = "butAddManual";
			this.butAddManual.Size = new System.Drawing.Size(92, 24);
			this.butAddManual.TabIndex = 151;
			this.butAddManual.Text = "Add Split";
			this.butAddManual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butAddManual.Click += new System.EventHandler(this.butAddManualSplit_Click);
			// 
			// contextMenuPaySimple
			// 
			this.contextMenuPaySimple.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuPaySimple});
			// 
			// menuPaySimple
			// 
			this.menuPaySimple.Index = 0;
			this.menuPaySimple.Text = "Settings";
			this.menuPaySimple.Click += new System.EventHandler(this.menuPaySimple_Click);
			// 
			// panelSplits
			// 
			this.panelSplits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelSplits.Controls.Add(this.tabProcCharges);
			this.panelSplits.Controls.Add(this.butAddManual);
			this.panelSplits.Controls.Add(this.butPay);
			this.panelSplits.Controls.Add(this.textSplitTotal);
			this.panelSplits.Controls.Add(this.labelPayPlan);
			this.panelSplits.Controls.Add(this.label13);
			this.panelSplits.Controls.Add(this.tabControlSplits);
			this.panelSplits.Controls.Add(this.butClear);
			this.panelSplits.Controls.Add(this.textChargeTotal);
			this.panelSplits.Controls.Add(this.groupBoxFiltering);
			this.panelSplits.Controls.Add(this.butCreatePartial);
			this.panelSplits.Controls.Add(this.label7);
			this.panelSplits.Controls.Add(this.label8);
			this.panelSplits.Controls.Add(this.comboGroupBy);
			this.panelSplits.Controls.Add(this.butDelete);
			this.panelSplits.Location = new System.Drawing.Point(0, 252);
			this.panelSplits.Name = "panelSplits";
			this.panelSplits.Size = new System.Drawing.Size(1111, 403);
			this.panelSplits.TabIndex = 206;
			// 
			// labelRecurringChargeWarning
			// 
			this.labelRecurringChargeWarning.Location = new System.Drawing.Point(780, 92);
			this.labelRecurringChargeWarning.Name = "labelRecurringChargeWarning";
			this.labelRecurringChargeWarning.Size = new System.Drawing.Size(280, 13);
			this.labelRecurringChargeWarning.TabIndex = 1000;
			this.labelRecurringChargeWarning.Text = "Uncheck \'Apply to Recurring Charge\' to unlock.";
			this.labelRecurringChargeWarning.Visible = false;
			// 
			// FormPayment
			// 
			this.ClientSize = new System.Drawing.Size(1111, 696);
			this.Controls.Add(this.labelRecurringChargeWarning);
			this.Controls.Add(this.panelSplits);
			this.Controls.Add(this.butCareCredit);
			this.Controls.Add(this.butPaySimple);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.butShowHide);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.textDeposit);
			this.Controls.Add(this.butDeletePayment);
			this.Controls.Add(this.butPayConnect);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelDepositAccount);
			this.Controls.Add(this.butPrintReceipt);
			this.Controls.Add(this.labelDeposit);
			this.Controls.Add(this.butEmailReceipt);
			this.Controls.Add(this.textDepositAccount);
			this.Controls.Add(this.checkPayTypeNone);
			this.Controls.Add(this.comboCreditCards);
			this.Controls.Add(this.textBankBranch);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.panelXcharge);
			this.Controls.Add(this.textCheckNum);
			this.Controls.Add(this.panelEdgeExpress);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.labelCreditCards);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listPayType);
			this.Controls.Add(this.comboDepositAccount);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.checkRecurring);
			this.Controls.Add(this.textPaidBy);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.groupXWeb);
			this.Controls.Add(this.butPrePay);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.checkProcessed);
			this.Controls.Add(this.label12);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayment";
			this.ShowInTaskbar = false;
			this.Text = "Payment";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPayment_FormClosing);
			this.Load += new System.EventHandler(this.FormPayment_Load);
			this.Shown += new System.EventHandler(this.FormPayment_Shown);
			this.groupXWeb.ResumeLayout(false);
			this.tabProcCharges.ResumeLayout(false);
			this.tabPageCharges.ResumeLayout(false);
			this.tabPageTreatPlan.ResumeLayout(false);
			this.tabControlSplits.ResumeLayout(false);
			this.tabPageSplits.ResumeLayout(false);
			this.groupBoxFiltering.ResumeLayout(false);
			this.groupBoxFiltering.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.amtMinEnd)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.amtMaxEnd)).EndInit();
			this.panelSplits.ResumeLayout(false);
			this.panelSplits.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Panel panelEdgeExpress;
		private System.Windows.Forms.Panel panelSplits;
		private System.Windows.Forms.Label labelRecurringChargeWarning;
	}
}
