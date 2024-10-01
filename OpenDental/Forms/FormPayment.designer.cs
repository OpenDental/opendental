namespace OpenDental {
	public partial class FormPayment {
		#region UI Elements
		private OpenDental.UI.Button butSave;
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
		private OpenDental.UI.ListBox listPayType;
		private OpenDental.UI.Button butDeletePayment;
		private OpenDental.ODtextBox textNote;//(not including discounts)
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textPaidBy;
		private UI.ComboBoxClinicPicker comboClinic;
		private OpenDental.ValidDate textDateEntry;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label labelDepositAccount;
		private OpenDental.UI.ComboBox comboDepositAccount;
		private System.Windows.Forms.Panel panelXcharge;
		private System.Windows.Forms.ContextMenu contextMenuXcharge;
		private System.Windows.Forms.MenuItem menuXcharge;
		private System.Windows.Forms.TextBox textDepositAccount;
		private System.Windows.Forms.TextBox textDeposit;
		private System.Windows.Forms.Label labelDeposit;
		private OpenDental.UI.CheckBox checkPayTypeNone;
		private System.Windows.Forms.ContextMenu contextMenuPayConnect;
		private System.Windows.Forms.MenuItem menuPayConnect;
		private UI.ComboBox comboCreditCards;
		private System.Windows.Forms.Label labelCreditCards;
		private OpenDental.UI.CheckBox checkRecurring;
		private UI.Button butPrintReceipt;
		private OpenDental.UI.GroupBox groupXWeb;
		private UI.Button butReturn;
		private UI.Button butVoid;
		private UI.Button butPrePay;
		private OpenDental.UI.CheckBox checkProcessed;
		private UI.Button butEmailReceipt;
		private System.Windows.Forms.Label labelPayPlan;
		private OpenDental.UI.TabControl tabControlSplits;
		private OpenDental.UI.TabPage tabPageSplits;
		private UI.GridOD gridSplits;
		private UI.GridOD gridCharges;
		private UI.Button butAddManual;
		private UI.Button butCreatePartial;
		private System.Windows.Forms.TextBox textSplitTotal;
		private System.Windows.Forms.Label label13;
		private OpenDental.UI.CheckBox checkShowAll;
		private UI.Button butPay;
		private UI.Button butClear;
		private UI.Button butDelete;
		private OpenDental.UI.ComboBox comboGroupBy;
		private System.Windows.Forms.Label labelGroupBy;
		private System.Windows.Forms.TextBox textChargeTotal;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textFilterProcCodes;
		private System.Windows.Forms.Label labelProcCodes;
		private System.Windows.Forms.Label labelProvOutstandingFilter;
		private System.Windows.Forms.Label labelClinicOutstandingFilter;
		private UI.ComboBox comboClinicOutstandingFilter;
		private System.Windows.Forms.Label labelPatOutstandingFilter;
		private UI.ComboBox comboPatientOutstandingFilter;
		private System.Windows.Forms.NumericUpDown amtMaxEndOutstanding;
		private System.Windows.Forms.Label labelMaxOutstandingFilter;
		private System.Windows.Forms.NumericUpDown amtMinEndOutstanding;
		private UI.Button butRefreshOutstanding;
		private System.Windows.Forms.Label labelMinOutstandingFilter;
		private System.Windows.Forms.Label labelTypeOutstandingFilter;
		private UI.ComboBox comboTypeOutstandingFilter;
		private OpenDental.UI.GroupBox groupBoxFilteringOutstanding;
		private System.Windows.Forms.Label labelDateFrom;
		private System.Windows.Forms.DateTimePicker datePickTo;
		private System.Windows.Forms.DateTimePicker datePickFrom;
		private UI.ComboBox comboProviderOutstandingFilter;
		private OpenDental.UI.CheckBox checkShowSuperfamily;
		private System.Windows.Forms.Label label9;
		private UI.Button butShowHide;
		private OpenDental.UI.CheckBox checkIncludeExplicitCreditsOnly;
		private OpenDental.UI.TabControl tabControlCharges;
		private OpenDental.UI.TabPage tabPageOutstanding;
		private OpenDental.UI.TabPage tabPageTreatPlan;
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
			this.butSave = new OpenDental.UI.Button();
			this.butDeletePayment = new OpenDental.UI.Button();
			this.butPrintReceipt = new OpenDental.UI.Button();
			this.butEmailReceipt = new OpenDental.UI.Button();
			this.butCareCredit = new System.Windows.Forms.Panel();
			this.butPaySimple = new System.Windows.Forms.Panel();
			this.butShowHide = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.textDeposit = new System.Windows.Forms.TextBox();
			this.labelDepositAccount = new System.Windows.Forms.Label();
			this.labelDeposit = new System.Windows.Forms.Label();
			this.textDepositAccount = new System.Windows.Forms.TextBox();
			this.comboCreditCards = new OpenDental.UI.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panelXcharge = new System.Windows.Forms.Panel();
			this.panelEdgeExpress = new System.Windows.Forms.Panel();
			this.labelCreditCards = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboDepositAccount = new OpenDental.UI.ComboBox();
			this.checkRecurring = new OpenDental.UI.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateEntry = new OpenDental.ValidDate();
			this.butPrePay = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.checkProcessed = new OpenDental.UI.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupXWeb = new OpenDental.UI.GroupBox();
			this.butReturn = new OpenDental.UI.Button();
			this.butVoid = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.textPaidBy = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.listPayType = new OpenDental.UI.ListBox();
			this.textNote = new OpenDental.ODtextBox();
			this.textAmount = new OpenDental.ValidDouble();
			this.textCheckNum = new System.Windows.Forms.TextBox();
			this.textDate = new OpenDental.ValidDate();
			this.textBankBranch = new System.Windows.Forms.TextBox();
			this.checkPayTypeNone = new OpenDental.UI.CheckBox();
			this.tabControlCharges = new OpenDental.UI.TabControl();
			this.tabPageOutstanding = new OpenDental.UI.TabPage();
			this.checkShowSuperfamily = new OpenDental.UI.CheckBox();
			this.gridCharges = new OpenDental.UI.GridOD();
			this.checkIncludeExplicitCreditsOnly = new OpenDental.UI.CheckBox();
			this.checkShowAll = new OpenDental.UI.CheckBox();
			this.tabPageTreatPlan = new OpenDental.UI.TabPage();
			this.labelTPProcWarning = new System.Windows.Forms.Label();
			this.gridTreatPlan = new OpenDental.UI.GridOD();
			this.butPay = new OpenDental.UI.Button();
			this.labelPayPlan = new System.Windows.Forms.Label();
			this.tabControlSplits = new OpenDental.UI.TabControl();
			this.tabPageSplits = new OpenDental.UI.TabPage();
			this.gridSplits = new OpenDental.UI.GridOD();
			this.textChargeTotal = new System.Windows.Forms.TextBox();
			this.butCreatePartial = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.comboGroupBy = new OpenDental.UI.ComboBox();
			this.labelGroupBy = new System.Windows.Forms.Label();
			this.groupBoxFilteringOutstanding = new OpenDental.UI.GroupBox();
			this.comboProviderOutstandingFilter = new OpenDental.UI.ComboBox();
			this.datePickTo = new System.Windows.Forms.DateTimePicker();
			this.labelDateFrom = new System.Windows.Forms.Label();
			this.datePickFrom = new System.Windows.Forms.DateTimePicker();
			this.textFilterProcCodes = new System.Windows.Forms.TextBox();
			this.labelMaxOutstandingFilter = new System.Windows.Forms.Label();
			this.labelMinOutstandingFilter = new System.Windows.Forms.Label();
			this.labelTypeOutstandingFilter = new System.Windows.Forms.Label();
			this.amtMinEndOutstanding = new System.Windows.Forms.NumericUpDown();
			this.amtMaxEndOutstanding = new System.Windows.Forms.NumericUpDown();
			this.butRefreshOutstanding = new OpenDental.UI.Button();
			this.comboPatientOutstandingFilter = new OpenDental.UI.ComboBox();
			this.labelProcCodes = new System.Windows.Forms.Label();
			this.comboTypeOutstandingFilter = new OpenDental.UI.ComboBox();
			this.comboClinicOutstandingFilter = new OpenDental.UI.ComboBox();
			this.labelPatOutstandingFilter = new System.Windows.Forms.Label();
			this.labelClinicOutstandingFilter = new System.Windows.Forms.Label();
			this.labelProvOutstandingFilter = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.textSplitTotal = new System.Windows.Forms.TextBox();
			this.butAddManual = new OpenDental.UI.Button();
			this.contextMenuPaySimple = new System.Windows.Forms.ContextMenu();
			this.menuPaySimple = new System.Windows.Forms.MenuItem();
			this.panelSplits = new System.Windows.Forms.Panel();
			this.groupBoxFilteringPaySplits = new OpenDental.UI.GroupBox();
			this.labelIncludesNegativeEntries = new System.Windows.Forms.Label();
			this.comboProviderPaySplitsFilter = new OpenDental.UI.ComboBox();
			this.labelMaxPaySplitsFilter = new System.Windows.Forms.Label();
			this.labelMinPaySplitsFilter = new System.Windows.Forms.Label();
			this.labelClinicsPaySplitsFilter = new System.Windows.Forms.Label();
			this.amtMinPaySplits = new System.Windows.Forms.NumericUpDown();
			this.amtMaxPaySplits = new System.Windows.Forms.NumericUpDown();
			this.butRefreshPaySplits = new OpenDental.UI.Button();
			this.comboPatientPaySplitsFilter = new OpenDental.UI.ComboBox();
			this.comboClinicsPaySplitsFilter = new OpenDental.UI.ComboBox();
			this.labelPatPaySplitsFilter = new System.Windows.Forms.Label();
			this.labelProvPaySplitsFilter = new System.Windows.Forms.Label();
			this.labelRecurringChargeWarning = new System.Windows.Forms.Label();
			this.warningIntegrity1 = new OpenDental.UI.WarningIntegrity();
			this.labelTransactionCompleted = new System.Windows.Forms.Label();
			this.labelSurchargeFee = new System.Windows.Forms.Label();
			this.textSurcharge = new System.Windows.Forms.TextBox();
			this.butPayConnect = new System.Windows.Forms.Panel();
			this.groupXWeb.SuspendLayout();
			this.tabControlCharges.SuspendLayout();
			this.tabPageOutstanding.SuspendLayout();
			this.tabPageTreatPlan.SuspendLayout();
			this.tabControlSplits.SuspendLayout();
			this.tabPageSplits.SuspendLayout();
			this.groupBoxFilteringOutstanding.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.amtMinEndOutstanding)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.amtMaxEndOutstanding)).BeginInit();
			this.panelSplits.SuspendLayout();
			this.groupBoxFilteringPaySplits.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.amtMinPaySplits)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.amtMaxPaySplits)).BeginInit();
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
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(1031, 664);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 998;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
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
			this.comboDepositAccount.Location = new System.Drawing.Point(492, 165);
			this.comboDepositAccount.Name = "comboDepositAccount";
			this.comboDepositAccount.Size = new System.Drawing.Size(260, 21);
			this.comboDepositAccount.TabIndex = 113;
			// 
			// checkRecurring
			// 
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
			this.textNote.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Payment;
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
			this.checkPayTypeNone.CheckedChanged += new System.EventHandler(this.checkPayTypeNone_CheckedChanged);
			this.checkPayTypeNone.Click += new System.EventHandler(this.checkPayTypeNone_Click);
			// 
			// tabControlCharges
			// 
			this.tabControlCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlCharges.Controls.Add(this.tabPageOutstanding);
			this.tabControlCharges.Controls.Add(this.tabPageTreatPlan);
			this.tabControlCharges.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControlCharges.Location = new System.Drawing.Point(498, 84);
			this.tabControlCharges.Name = "tabControlCharges";
			this.tabControlCharges.Size = new System.Drawing.Size(608, 296);
			this.tabControlCharges.TabIndex = 1;
			this.tabControlCharges.SelectedIndexChanged += new System.EventHandler(this.TabControlCharges_SelectedIndexChanged);
			// 
			// tabPageOutstanding
			// 
			this.tabPageOutstanding.Controls.Add(this.checkShowSuperfamily);
			this.tabPageOutstanding.Controls.Add(this.gridCharges);
			this.tabPageOutstanding.Controls.Add(this.checkIncludeExplicitCreditsOnly);
			this.tabPageOutstanding.Controls.Add(this.checkShowAll);
			this.tabPageOutstanding.Location = new System.Drawing.Point(2, 21);
			this.tabPageOutstanding.Name = "tabPageOutstanding";
			this.tabPageOutstanding.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageOutstanding.Size = new System.Drawing.Size(604, 273);
			this.tabPageOutstanding.TabIndex = 0;
			this.tabPageOutstanding.Text = "Outstanding";
			// 
			// checkShowSuperfamily
			// 
			this.checkShowSuperfamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowSuperfamily.Location = new System.Drawing.Point(230, 2);
			this.checkShowSuperfamily.Name = "checkShowSuperfamily";
			this.checkShowSuperfamily.Size = new System.Drawing.Size(193, 20);
			this.checkShowSuperfamily.TabIndex = 204;
			this.checkShowSuperfamily.Text = "Show Super Family Charges";
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
			this.gridCharges.Size = new System.Drawing.Size(602, 249);
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
			this.checkIncludeExplicitCreditsOnly.Click += new System.EventHandler(this.CheckIncludeExplicitCreditsOnly_Click);
			// 
			// checkShowAll
			// 
			this.checkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAll.Location = new System.Drawing.Point(435, 2);
			this.checkShowAll.Name = "checkShowAll";
			this.checkShowAll.Size = new System.Drawing.Size(148, 20);
			this.checkShowAll.TabIndex = 147;
			this.checkShowAll.Text = "Show All Charges";
			this.checkShowAll.Click += new System.EventHandler(this.checkShowAll_Clicked);
			// 
			// tabPageTreatPlan
			// 
			this.tabPageTreatPlan.Controls.Add(this.labelTPProcWarning);
			this.tabPageTreatPlan.Controls.Add(this.gridTreatPlan);
			this.tabPageTreatPlan.Location = new System.Drawing.Point(2, 21);
			this.tabPageTreatPlan.Name = "tabPageTreatPlan";
			this.tabPageTreatPlan.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageTreatPlan.Size = new System.Drawing.Size(604, 273);
			this.tabPageTreatPlan.TabIndex = 1;
			this.tabPageTreatPlan.Text = "Treat Plan";
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
			this.gridTreatPlan.Size = new System.Drawing.Size(602, 245);
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
			this.butPay.Location = new System.Drawing.Point(504, 385);
			this.butPay.Name = "butPay";
			this.butPay.Size = new System.Drawing.Size(79, 23);
			this.butPay.TabIndex = 146;
			this.butPay.Text = "Pay";
			this.butPay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.butPay.Click += new System.EventHandler(this.butPay_Click);
			// 
			// labelPayPlan
			// 
			this.labelPayPlan.Location = new System.Drawing.Point(258, 104);
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
			this.tabControlSplits.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControlSplits.Location = new System.Drawing.Point(9, 106);
			this.tabControlSplits.Name = "tabControlSplits";
			this.tabControlSplits.Size = new System.Drawing.Size(487, 274);
			this.tabControlSplits.TabIndex = 5;
			// 
			// tabPageSplits
			// 
			this.tabPageSplits.Controls.Add(this.gridSplits);
			this.tabPageSplits.Location = new System.Drawing.Point(2, 21);
			this.tabPageSplits.Name = "tabPageSplits";
			this.tabPageSplits.Size = new System.Drawing.Size(483, 251);
			this.tabPageSplits.TabIndex = 0;
			this.tabPageSplits.Text = "Splits";
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
			this.gridSplits.Size = new System.Drawing.Size(483, 249);
			this.gridSplits.TabIndex = 0;
			this.gridSplits.Title = "Current Payment Splits";
			this.gridSplits.TranslationName = "TableCurrentSplits";
			this.gridSplits.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSplits_CellDoubleClick);
			this.gridSplits.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSplits_CellClick);
			// 
			// textChargeTotal
			// 
			this.textChargeTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textChargeTotal.Location = new System.Drawing.Point(1051, 386);
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
			this.butCreatePartial.Location = new System.Drawing.Point(593, 385);
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
			this.label8.Location = new System.Drawing.Point(992, 387);
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
			this.butDelete.Location = new System.Drawing.Point(11, 384);
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
			this.comboGroupBy.Location = new System.Drawing.Point(795, 386);
			this.comboGroupBy.Name = "comboGroupBy";
			this.comboGroupBy.Size = new System.Drawing.Size(111, 21);
			this.comboGroupBy.TabIndex = 153;
			this.comboGroupBy.SelectionChangeCommitted += new System.EventHandler(this.comboGroupBy_SelectionChangeCommitted);
			// 
			// labelGroupBy
			// 
			this.labelGroupBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelGroupBy.Location = new System.Drawing.Point(717, 387);
			this.labelGroupBy.Name = "labelGroupBy";
			this.labelGroupBy.Size = new System.Drawing.Size(77, 18);
			this.labelGroupBy.TabIndex = 154;
			this.labelGroupBy.Text = "Group By";
			this.labelGroupBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxFilteringOutstanding
			// 
			this.groupBoxFilteringOutstanding.Controls.Add(this.comboProviderOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.datePickTo);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelDateFrom);
			this.groupBoxFilteringOutstanding.Controls.Add(this.datePickFrom);
			this.groupBoxFilteringOutstanding.Controls.Add(this.textFilterProcCodes);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelMaxOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelMinOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelTypeOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.amtMinEndOutstanding);
			this.groupBoxFilteringOutstanding.Controls.Add(this.amtMaxEndOutstanding);
			this.groupBoxFilteringOutstanding.Controls.Add(this.butRefreshOutstanding);
			this.groupBoxFilteringOutstanding.Controls.Add(this.comboPatientOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelProcCodes);
			this.groupBoxFilteringOutstanding.Controls.Add(this.comboTypeOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.comboClinicOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelPatOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelClinicOutstandingFilter);
			this.groupBoxFilteringOutstanding.Controls.Add(this.labelProvOutstandingFilter);
			this.groupBoxFilteringOutstanding.Location = new System.Drawing.Point(498, 2);
			this.groupBoxFilteringOutstanding.Name = "groupBoxFilteringOutstanding";
			this.groupBoxFilteringOutstanding.Size = new System.Drawing.Size(603, 80);
			this.groupBoxFilteringOutstanding.TabIndex = 203;
			this.groupBoxFilteringOutstanding.Text = "Filtering";
			// 
			// comboProviderOutstandingFilter
			// 
			this.comboProviderOutstandingFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboProviderOutstandingFilter.Location = new System.Drawing.Point(106, 33);
			this.comboProviderOutstandingFilter.Name = "comboProviderOutstandingFilter";
			this.comboProviderOutstandingFilter.SelectionModeMulti = true;
			this.comboProviderOutstandingFilter.Size = new System.Drawing.Size(90, 21);
			this.comboProviderOutstandingFilter.TabIndex = 207;
			this.comboProviderOutstandingFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// datePickTo
			// 
			this.datePickTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.datePickTo.Location = new System.Drawing.Point(199, 56);
			this.datePickTo.Name = "datePickTo";
			this.datePickTo.Size = new System.Drawing.Size(90, 20);
			this.datePickTo.TabIndex = 204;
			this.datePickTo.ValueChanged += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
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
			this.datePickFrom.ValueChanged += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// textFilterProcCodes
			// 
			this.textFilterProcCodes.Location = new System.Drawing.Point(395, 56);
			this.textFilterProcCodes.Name = "textFilterProcCodes";
			this.textFilterProcCodes.Size = new System.Drawing.Size(90, 20);
			this.textFilterProcCodes.TabIndex = 187;
			this.textFilterProcCodes.TextChanged += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// labelMaxOutstandingFilter
			// 
			this.labelMaxOutstandingFilter.Location = new System.Drawing.Point(488, 14);
			this.labelMaxOutstandingFilter.Name = "labelMaxOutstandingFilter";
			this.labelMaxOutstandingFilter.Size = new System.Drawing.Size(89, 18);
			this.labelMaxOutstandingFilter.TabIndex = 198;
			this.labelMaxOutstandingFilter.Text = "Amt End Max";
			this.labelMaxOutstandingFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMinOutstandingFilter
			// 
			this.labelMinOutstandingFilter.Location = new System.Drawing.Point(395, 14);
			this.labelMinOutstandingFilter.Name = "labelMinOutstandingFilter";
			this.labelMinOutstandingFilter.Size = new System.Drawing.Size(87, 18);
			this.labelMinOutstandingFilter.TabIndex = 200;
			this.labelMinOutstandingFilter.Text = "Amt End Min.";
			this.labelMinOutstandingFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTypeOutstandingFilter
			// 
			this.labelTypeOutstandingFilter.Location = new System.Drawing.Point(295, 14);
			this.labelTypeOutstandingFilter.Name = "labelTypeOutstandingFilter";
			this.labelTypeOutstandingFilter.Size = new System.Drawing.Size(60, 18);
			this.labelTypeOutstandingFilter.TabIndex = 202;
			this.labelTypeOutstandingFilter.Text = "Type";
			this.labelTypeOutstandingFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// amtMinEndOutstanding
			// 
			this.amtMinEndOutstanding.DecimalPlaces = 2;
			this.amtMinEndOutstanding.Location = new System.Drawing.Point(395, 33);
			this.amtMinEndOutstanding.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
			this.amtMinEndOutstanding.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
			this.amtMinEndOutstanding.Name = "amtMinEndOutstanding";
			this.amtMinEndOutstanding.Size = new System.Drawing.Size(90, 20);
			this.amtMinEndOutstanding.TabIndex = 199;
			this.amtMinEndOutstanding.ThousandsSeparator = true;
			this.amtMinEndOutstanding.ValueChanged += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// amtMaxEndOutstanding
			// 
			this.amtMaxEndOutstanding.DecimalPlaces = 2;
			this.amtMaxEndOutstanding.Location = new System.Drawing.Point(488, 33);
			this.amtMaxEndOutstanding.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.amtMaxEndOutstanding.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
			this.amtMaxEndOutstanding.Name = "amtMaxEndOutstanding";
			this.amtMaxEndOutstanding.Size = new System.Drawing.Size(90, 20);
			this.amtMaxEndOutstanding.TabIndex = 197;
			this.amtMaxEndOutstanding.ThousandsSeparator = true;
			this.amtMaxEndOutstanding.ValueChanged += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// butRefreshOutstanding
			// 
			this.butRefreshOutstanding.Location = new System.Drawing.Point(503, 55);
			this.butRefreshOutstanding.Name = "butRefreshOutstanding";
			this.butRefreshOutstanding.Size = new System.Drawing.Size(75, 24);
			this.butRefreshOutstanding.TabIndex = 141;
			this.butRefreshOutstanding.Text = "Refresh";
			this.butRefreshOutstanding.Click += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// comboPatientOutstandingFilter
			// 
			this.comboPatientOutstandingFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboPatientOutstandingFilter.Location = new System.Drawing.Point(13, 33);
			this.comboPatientOutstandingFilter.Name = "comboPatientOutstandingFilter";
			this.comboPatientOutstandingFilter.SelectionModeMulti = true;
			this.comboPatientOutstandingFilter.Size = new System.Drawing.Size(90, 21);
			this.comboPatientOutstandingFilter.TabIndex = 195;
			this.comboPatientOutstandingFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
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
			// comboTypeOutstandingFilter
			// 
			this.comboTypeOutstandingFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboTypeOutstandingFilter.Location = new System.Drawing.Point(292, 33);
			this.comboTypeOutstandingFilter.Name = "comboTypeOutstandingFilter";
			this.comboTypeOutstandingFilter.SelectionModeMulti = true;
			this.comboTypeOutstandingFilter.Size = new System.Drawing.Size(100, 21);
			this.comboTypeOutstandingFilter.TabIndex = 201;
			this.comboTypeOutstandingFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// comboClinicOutstandingFilter
			// 
			this.comboClinicOutstandingFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboClinicOutstandingFilter.Location = new System.Drawing.Point(199, 33);
			this.comboClinicOutstandingFilter.Name = "comboClinicOutstandingFilter";
			this.comboClinicOutstandingFilter.SelectionModeMulti = true;
			this.comboClinicOutstandingFilter.Size = new System.Drawing.Size(90, 21);
			this.comboClinicOutstandingFilter.TabIndex = 193;
			this.comboClinicOutstandingFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedChargesTreatPlan);
			// 
			// labelPatOutstandingFilter
			// 
			this.labelPatOutstandingFilter.Location = new System.Drawing.Point(13, 14);
			this.labelPatOutstandingFilter.Name = "labelPatOutstandingFilter";
			this.labelPatOutstandingFilter.Size = new System.Drawing.Size(67, 18);
			this.labelPatOutstandingFilter.TabIndex = 196;
			this.labelPatOutstandingFilter.Text = "Patients";
			this.labelPatOutstandingFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClinicOutstandingFilter
			// 
			this.labelClinicOutstandingFilter.Location = new System.Drawing.Point(198, 14);
			this.labelClinicOutstandingFilter.Name = "labelClinicOutstandingFilter";
			this.labelClinicOutstandingFilter.Size = new System.Drawing.Size(60, 18);
			this.labelClinicOutstandingFilter.TabIndex = 194;
			this.labelClinicOutstandingFilter.Text = "Clinics";
			this.labelClinicOutstandingFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvOutstandingFilter
			// 
			this.labelProvOutstandingFilter.Location = new System.Drawing.Point(106, 14);
			this.labelProvOutstandingFilter.Name = "labelProvOutstandingFilter";
			this.labelProvOutstandingFilter.Size = new System.Drawing.Size(67, 18);
			this.labelProvOutstandingFilter.TabIndex = 192;
			this.labelProvOutstandingFilter.Text = "Providers";
			this.labelProvOutstandingFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butClear
			// 
			this.butClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClear.Location = new System.Drawing.Point(117, 384);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(89, 24);
			this.butClear.TabIndex = 145;
			this.butClear.Text = "Delete All";
			this.butClear.Click += new System.EventHandler(this.butDeleteAllSplits_Click);
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label13.Location = new System.Drawing.Point(382, 387);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(58, 18);
			this.label13.TabIndex = 148;
			this.label13.Text = "Total";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSplitTotal
			// 
			this.textSplitTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textSplitTotal.Location = new System.Drawing.Point(441, 386);
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
			this.butAddManual.Location = new System.Drawing.Point(212, 384);
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
			this.panelSplits.Controls.Add(this.groupBoxFilteringPaySplits);
			this.panelSplits.Controls.Add(this.tabControlCharges);
			this.panelSplits.Controls.Add(this.butAddManual);
			this.panelSplits.Controls.Add(this.butPay);
			this.panelSplits.Controls.Add(this.textSplitTotal);
			this.panelSplits.Controls.Add(this.labelPayPlan);
			this.panelSplits.Controls.Add(this.label13);
			this.panelSplits.Controls.Add(this.tabControlSplits);
			this.panelSplits.Controls.Add(this.butClear);
			this.panelSplits.Controls.Add(this.textChargeTotal);
			this.panelSplits.Controls.Add(this.groupBoxFilteringOutstanding);
			this.panelSplits.Controls.Add(this.butCreatePartial);
			this.panelSplits.Controls.Add(this.labelGroupBy);
			this.panelSplits.Controls.Add(this.label8);
			this.panelSplits.Controls.Add(this.comboGroupBy);
			this.panelSplits.Controls.Add(this.butDelete);
			this.panelSplits.Location = new System.Drawing.Point(0, 245);
			this.panelSplits.Name = "panelSplits";
			this.panelSplits.Size = new System.Drawing.Size(1111, 410);
			this.panelSplits.TabIndex = 206;
			// 
			// groupBoxFilteringPaySplits
			// 
			this.groupBoxFilteringPaySplits.Controls.Add(this.labelIncludesNegativeEntries);
			this.groupBoxFilteringPaySplits.Controls.Add(this.comboProviderPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.labelMaxPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.labelMinPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.labelClinicsPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.amtMinPaySplits);
			this.groupBoxFilteringPaySplits.Controls.Add(this.amtMaxPaySplits);
			this.groupBoxFilteringPaySplits.Controls.Add(this.butRefreshPaySplits);
			this.groupBoxFilteringPaySplits.Controls.Add(this.comboPatientPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.comboClinicsPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.labelPatPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Controls.Add(this.labelProvPaySplitsFilter);
			this.groupBoxFilteringPaySplits.Location = new System.Drawing.Point(3, 2);
			this.groupBoxFilteringPaySplits.Name = "groupBoxFilteringPaySplits";
			this.groupBoxFilteringPaySplits.Size = new System.Drawing.Size(493, 80);
			this.groupBoxFilteringPaySplits.TabIndex = 208;
			this.groupBoxFilteringPaySplits.Text = "Filtering Current Payment Splits";
			// 
			// labelIncludesNegativeEntries
			// 
			this.labelIncludesNegativeEntries.Location = new System.Drawing.Point(163, 58);
			this.labelIncludesNegativeEntries.Name = "labelIncludesNegativeEntries";
			this.labelIncludesNegativeEntries.Size = new System.Drawing.Size(246, 16);
			this.labelIncludesNegativeEntries.TabIndex = 1001;
			this.labelIncludesNegativeEntries.Text = "(Includes negative entries in the same range)";
			this.labelIncludesNegativeEntries.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProviderPaySplitsFilter
			// 
			this.comboProviderPaySplitsFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboProviderPaySplitsFilter.Location = new System.Drawing.Point(106, 33);
			this.comboProviderPaySplitsFilter.Name = "comboProviderPaySplitsFilter";
			this.comboProviderPaySplitsFilter.SelectionModeMulti = true;
			this.comboProviderPaySplitsFilter.Size = new System.Drawing.Size(90, 21);
			this.comboProviderPaySplitsFilter.TabIndex = 207;
			this.comboProviderPaySplitsFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedPaySplits);
			// 
			// labelMaxPaySplitsFilter
			// 
			this.labelMaxPaySplitsFilter.Location = new System.Drawing.Point(401, 12);
			this.labelMaxPaySplitsFilter.Name = "labelMaxPaySplitsFilter";
			this.labelMaxPaySplitsFilter.Size = new System.Drawing.Size(89, 18);
			this.labelMaxPaySplitsFilter.TabIndex = 198;
			this.labelMaxPaySplitsFilter.Text = "Amt Max";
			this.labelMaxPaySplitsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelMinPaySplitsFilter
			// 
			this.labelMinPaySplitsFilter.Location = new System.Drawing.Point(308, 12);
			this.labelMinPaySplitsFilter.Name = "labelMinPaySplitsFilter";
			this.labelMinPaySplitsFilter.Size = new System.Drawing.Size(87, 18);
			this.labelMinPaySplitsFilter.TabIndex = 200;
			this.labelMinPaySplitsFilter.Text = "Amt Min";
			this.labelMinPaySplitsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClinicsPaySplitsFilter
			// 
			this.labelClinicsPaySplitsFilter.Location = new System.Drawing.Point(199, 14);
			this.labelClinicsPaySplitsFilter.Name = "labelClinicsPaySplitsFilter";
			this.labelClinicsPaySplitsFilter.Size = new System.Drawing.Size(60, 18);
			this.labelClinicsPaySplitsFilter.TabIndex = 202;
			this.labelClinicsPaySplitsFilter.Text = "Clinics";
			this.labelClinicsPaySplitsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// amtMinPaySplits
			// 
			this.amtMinPaySplits.DecimalPlaces = 2;
			this.amtMinPaySplits.Location = new System.Drawing.Point(305, 33);
			this.amtMinPaySplits.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
			this.amtMinPaySplits.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
			this.amtMinPaySplits.Name = "amtMinPaySplits";
			this.amtMinPaySplits.Size = new System.Drawing.Size(90, 20);
			this.amtMinPaySplits.TabIndex = 199;
			this.amtMinPaySplits.ThousandsSeparator = true;
			this.amtMinPaySplits.ValueChanged += new System.EventHandler(this.FilterChangeCommittedPaySplits);
			// 
			// amtMaxPaySplits
			// 
			this.amtMaxPaySplits.DecimalPlaces = 2;
			this.amtMaxPaySplits.Location = new System.Drawing.Point(401, 33);
			this.amtMaxPaySplits.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
			this.amtMaxPaySplits.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
			this.amtMaxPaySplits.Name = "amtMaxPaySplits";
			this.amtMaxPaySplits.Size = new System.Drawing.Size(90, 20);
			this.amtMaxPaySplits.TabIndex = 197;
			this.amtMaxPaySplits.ThousandsSeparator = true;
			this.amtMaxPaySplits.ValueChanged += new System.EventHandler(this.FilterChangeCommittedPaySplits);
			// 
			// butRefreshPaySplits
			// 
			this.butRefreshPaySplits.Location = new System.Drawing.Point(415, 55);
			this.butRefreshPaySplits.Name = "butRefreshPaySplits";
			this.butRefreshPaySplits.Size = new System.Drawing.Size(75, 24);
			this.butRefreshPaySplits.TabIndex = 141;
			this.butRefreshPaySplits.Text = "Refresh";
			this.butRefreshPaySplits.Click += new System.EventHandler(this.FilterChangeCommittedPaySplits);
			// 
			// comboPatientPaySplitsFilter
			// 
			this.comboPatientPaySplitsFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboPatientPaySplitsFilter.Location = new System.Drawing.Point(13, 33);
			this.comboPatientPaySplitsFilter.Name = "comboPatientPaySplitsFilter";
			this.comboPatientPaySplitsFilter.SelectionModeMulti = true;
			this.comboPatientPaySplitsFilter.Size = new System.Drawing.Size(90, 21);
			this.comboPatientPaySplitsFilter.TabIndex = 195;
			this.comboPatientPaySplitsFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedPaySplits);
			// 
			// comboClinicsPaySplitsFilter
			// 
			this.comboClinicsPaySplitsFilter.BackColor = System.Drawing.SystemColors.Window;
			this.comboClinicsPaySplitsFilter.Location = new System.Drawing.Point(199, 33);
			this.comboClinicsPaySplitsFilter.Name = "comboClinicsPaySplitsFilter";
			this.comboClinicsPaySplitsFilter.SelectionModeMulti = true;
			this.comboClinicsPaySplitsFilter.Size = new System.Drawing.Size(100, 21);
			this.comboClinicsPaySplitsFilter.TabIndex = 201;
			this.comboClinicsPaySplitsFilter.SelectionChangeCommitted += new System.EventHandler(this.FilterChangeCommittedPaySplits);
			// 
			// labelPatPaySplitsFilter
			// 
			this.labelPatPaySplitsFilter.Location = new System.Drawing.Point(13, 14);
			this.labelPatPaySplitsFilter.Name = "labelPatPaySplitsFilter";
			this.labelPatPaySplitsFilter.Size = new System.Drawing.Size(67, 18);
			this.labelPatPaySplitsFilter.TabIndex = 196;
			this.labelPatPaySplitsFilter.Text = "Patients";
			this.labelPatPaySplitsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvPaySplitsFilter
			// 
			this.labelProvPaySplitsFilter.Location = new System.Drawing.Point(106, 14);
			this.labelProvPaySplitsFilter.Name = "labelProvPaySplitsFilter";
			this.labelProvPaySplitsFilter.Size = new System.Drawing.Size(67, 18);
			this.labelProvPaySplitsFilter.TabIndex = 192;
			this.labelProvPaySplitsFilter.Text = "Providers";
			this.labelProvPaySplitsFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			// warningIntegrity1
			// 
			this.warningIntegrity1.Location = new System.Drawing.Point(1, -1);
			this.warningIntegrity1.Name = "warningIntegrity1";
			this.warningIntegrity1.Size = new System.Drawing.Size(18, 18);
			this.warningIntegrity1.TabIndex = 1001;
			// 
			// labelTransactionCompleted
			// 
			this.labelTransactionCompleted.ForeColor = System.Drawing.Color.Firebrick;
			this.labelTransactionCompleted.Location = new System.Drawing.Point(272, 94);
			this.labelTransactionCompleted.Name = "labelTransactionCompleted";
			this.labelTransactionCompleted.Size = new System.Drawing.Size(190, 57);
			this.labelTransactionCompleted.TabIndex = 1002;
			this.labelTransactionCompleted.Text = "This transaction is already complete, you must make a new payment to process a ne" +
    "w transaction.";
			this.labelTransactionCompleted.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.labelTransactionCompleted.Visible = false;
			// 
			// labelSurchargeFee
			// 
			this.labelSurchargeFee.Location = new System.Drawing.Point(259, 62);
			this.labelSurchargeFee.Name = "labelSurchargeFee";
			this.labelSurchargeFee.Size = new System.Drawing.Size(100, 16);
			this.labelSurchargeFee.TabIndex = 1004;
			this.labelSurchargeFee.Text = "Surcharge Fee";
			this.labelSurchargeFee.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelSurchargeFee.Visible = false;
			// 
			// textSurcharge
			// 
			this.textSurcharge.Location = new System.Drawing.Point(362, 59);
			this.textSurcharge.Name = "textSurcharge";
			this.textSurcharge.ReadOnly = true;
			this.textSurcharge.Size = new System.Drawing.Size(100, 20);
			this.textSurcharge.TabIndex = 1005;
			this.textSurcharge.Visible = false;
			// 
			// butPayConnect
			// 
			this.butPayConnect.BackgroundImage = global::OpenDental.Properties.Resources.payconnect_btn_26;
			this.butPayConnect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.butPayConnect.Location = new System.Drawing.Point(958, 17);
			this.butPayConnect.Name = "butPayConnect";
			this.butPayConnect.Size = new System.Drawing.Size(75, 26);
			this.butPayConnect.TabIndex = 141;
			this.butPayConnect.Visible = false;
			this.butPayConnect.Click += new System.EventHandler(this.butPayConnect_Click);
			// 
			// FormPayment
			// 
			this.ClientSize = new System.Drawing.Size(1111, 696);
			this.Controls.Add(this.butPayConnect);
			this.Controls.Add(this.textSurcharge);
			this.Controls.Add(this.labelSurchargeFee);
			this.Controls.Add(this.labelTransactionCompleted);
			this.Controls.Add(this.warningIntegrity1);
			this.Controls.Add(this.labelRecurringChargeWarning);
			this.Controls.Add(this.panelSplits);
			this.Controls.Add(this.butCareCredit);
			this.Controls.Add(this.butPaySimple);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.butShowHide);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.textDeposit);
			this.Controls.Add(this.butDeletePayment);
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
			this.tabControlCharges.ResumeLayout(false);
			this.tabPageOutstanding.ResumeLayout(false);
			this.tabPageTreatPlan.ResumeLayout(false);
			this.tabControlSplits.ResumeLayout(false);
			this.tabPageSplits.ResumeLayout(false);
			this.groupBoxFilteringOutstanding.ResumeLayout(false);
			this.groupBoxFilteringOutstanding.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.amtMinEndOutstanding)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.amtMaxEndOutstanding)).EndInit();
			this.panelSplits.ResumeLayout(false);
			this.panelSplits.PerformLayout();
			this.groupBoxFilteringPaySplits.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.amtMinPaySplits)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.amtMaxPaySplits)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Panel panelEdgeExpress;
		private System.Windows.Forms.Panel panelSplits;
		private System.Windows.Forms.Label labelRecurringChargeWarning;
		private UI.WarningIntegrity warningIntegrity1;
		private System.Windows.Forms.Label labelTransactionCompleted;
		private UI.GroupBox groupBoxFilteringPaySplits;
		private UI.ComboBox comboProviderPaySplitsFilter;
		private System.Windows.Forms.Label labelMaxPaySplitsFilter;
		private System.Windows.Forms.Label labelMinPaySplitsFilter;
		private System.Windows.Forms.Label labelClinicsPaySplitsFilter;
		private System.Windows.Forms.NumericUpDown amtMinPaySplits;
		private System.Windows.Forms.NumericUpDown amtMaxPaySplits;
		private UI.Button butRefreshPaySplits;
		private UI.ComboBox comboPatientPaySplitsFilter;
		private UI.ComboBox comboClinicsPaySplitsFilter;
		private System.Windows.Forms.Label labelPatPaySplitsFilter;
		private System.Windows.Forms.Label labelProvPaySplitsFilter;
		private System.Windows.Forms.Label labelSurchargeFee;
		private System.Windows.Forms.TextBox textSurcharge;
		private System.Windows.Forms.Label labelIncludesNegativeEntries;
		private System.Windows.Forms.Panel butPayConnect;
	}
}
