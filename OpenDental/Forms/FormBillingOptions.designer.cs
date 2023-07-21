using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormBillingOptions {
	private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBillingOptions));
			this.butCancel = new OpenDental.UI.Button();
			this.butCreate = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butSaveDefault = new OpenDental.UI.Button();
			this.textExcludeLessThan = new OpenDental.ValidDouble();
			this.checkExcludeInactive = new OpenDental.UI.CheckBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.labelExcludeIfPendingDays = new System.Windows.Forms.Label();
			this.textDaysInsPendingExclude = new OpenDental.ValidNum();
			this.comboInsFilingCodes = new OpenDental.UI.ComboBox();
			this.labelInsFilingCodes = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkUseClinicDefaults = new OpenDental.UI.CheckBox();
			this.checkExcludeIfProcs = new OpenDental.UI.CheckBox();
			this.checkIgnoreInPerson = new OpenDental.UI.CheckBox();
			this.labelSaveDefaults = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboAge = new OpenDental.UI.ComboBox();
			this.checkExcludeInsPending = new OpenDental.UI.CheckBox();
			this.checkIncludeChanged = new OpenDental.UI.CheckBox();
			this.textLastStatement = new OpenDental.ValidDate();
			this.label5 = new System.Windows.Forms.Label();
			this.checkShowNegative = new OpenDental.UI.CheckBox();
			this.checkBadAddress = new OpenDental.UI.CheckBox();
			this.listBillType = new OpenDental.UI.ListBox();
			this.checkBoxBillShowTransSinceZero = new OpenDental.UI.CheckBox();
			this.gridDunning = new OpenDental.UI.GridOD();
			this.butDunningSetup = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.groupDateRange = new OpenDental.UI.GroupBox();
			this.but30days = new OpenDental.UI.Button();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.labelEndDate = new System.Windows.Forms.Label();
			this.textDateEnd = new OpenDental.ValidDate();
			this.but45days = new OpenDental.UI.Button();
			this.but90days = new OpenDental.UI.Button();
			this.butDatesAll = new OpenDental.UI.Button();
			this.checkIntermingled = new OpenDental.UI.CheckBox();
			this.butDefaults = new OpenDental.UI.Button();
			this.butUndo = new OpenDental.UI.Button();
			this.checkSuperFam = new OpenDental.UI.CheckBox();
			this.checkSinglePatient = new OpenDental.UI.CheckBox();
			this.labelModesToText = new System.Windows.Forms.Label();
			this.listModeToText = new OpenDental.UI.ListBox();
			this.labelMultiClinicGenMsg = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.groupDateRange.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(826, 664);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(79, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butCreate
			// 
			this.butCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCreate.Location = new System.Drawing.Point(713, 664);
			this.butCreate.Name = "butCreate";
			this.butCreate.Size = new System.Drawing.Size(92, 24);
			this.butCreate.TabIndex = 11;
			this.butCreate.Text = "Create &List";
			this.butCreate.Click += new System.EventHandler(this.butCreate_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(5, 203);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 16);
			this.label1.TabIndex = 18;
			this.label1.Text = "Exclude if Balance is less than";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSaveDefault
			// 
			this.butSaveDefault.Location = new System.Drawing.Point(169, 588);
			this.butSaveDefault.Name = "butSaveDefault";
			this.butSaveDefault.Size = new System.Drawing.Size(108, 24);
			this.butSaveDefault.TabIndex = 14;
			this.butSaveDefault.Text = "&Save As Default";
			this.butSaveDefault.Click += new System.EventHandler(this.butSaveDefault_Click);
			// 
			// textExcludeLessThan
			// 
			this.textExcludeLessThan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textExcludeLessThan.Location = new System.Drawing.Point(199, 202);
			this.textExcludeLessThan.MaxVal = 100000000D;
			this.textExcludeLessThan.MinVal = -100000000D;
			this.textExcludeLessThan.Name = "textExcludeLessThan";
			this.textExcludeLessThan.Size = new System.Drawing.Size(77, 20);
			this.textExcludeLessThan.TabIndex = 7;
			// 
			// checkExcludeInactive
			// 
			this.checkExcludeInactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkExcludeInactive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeInactive.Location = new System.Drawing.Point(45, 117);
			this.checkExcludeInactive.Name = "checkExcludeInactive";
			this.checkExcludeInactive.Size = new System.Drawing.Size(231, 18);
			this.checkExcludeInactive.TabIndex = 4;
			this.checkExcludeInactive.Text = "Exclude inactive families";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.labelExcludeIfPendingDays);
			this.groupBox2.Controls.Add(this.textDaysInsPendingExclude);
			this.groupBox2.Controls.Add(this.comboInsFilingCodes);
			this.groupBox2.Controls.Add(this.labelInsFilingCodes);
			this.groupBox2.Controls.Add(this.comboClinic);
			this.groupBox2.Controls.Add(this.checkUseClinicDefaults);
			this.groupBox2.Controls.Add(this.checkExcludeIfProcs);
			this.groupBox2.Controls.Add(this.checkIgnoreInPerson);
			this.groupBox2.Controls.Add(this.labelSaveDefaults);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.comboAge);
			this.groupBox2.Controls.Add(this.checkExcludeInsPending);
			this.groupBox2.Controls.Add(this.checkIncludeChanged);
			this.groupBox2.Controls.Add(this.textLastStatement);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.checkShowNegative);
			this.groupBox2.Controls.Add(this.checkBadAddress);
			this.groupBox2.Controls.Add(this.checkExcludeInactive);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.textExcludeLessThan);
			this.groupBox2.Controls.Add(this.butSaveDefault);
			this.groupBox2.Controls.Add(this.listBillType);
			this.groupBox2.Location = new System.Drawing.Point(7, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(295, 641);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.Text = "Filter";
			// 
			// labelExcludeIfPendingDays
			// 
			this.labelExcludeIfPendingDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExcludeIfPendingDays.Location = new System.Drawing.Point(2, 158);
			this.labelExcludeIfPendingDays.Name = "labelExcludeIfPendingDays";
			this.labelExcludeIfPendingDays.Size = new System.Drawing.Size(220, 16);
			this.labelExcludeIfPendingDays.TabIndex = 261;
			this.labelExcludeIfPendingDays.Text = "Exclude if insurance pending _ days or less";
			this.labelExcludeIfPendingDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDaysInsPendingExclude
			// 
			this.textDaysInsPendingExclude.Location = new System.Drawing.Point(222, 157);
			this.textDaysInsPendingExclude.MaxVal = 50000;
			this.textDaysInsPendingExclude.Name = "textDaysInsPendingExclude";
			this.textDaysInsPendingExclude.ShowZero = false;
			this.textDaysInsPendingExclude.Size = new System.Drawing.Size(54, 20);
			this.textDaysInsPendingExclude.TabIndex = 257;
			// 
			// comboInsFilingCodes
			// 
			this.comboInsFilingCodes.Location = new System.Drawing.Point(131, 269);
			this.comboInsFilingCodes.Name = "comboInsFilingCodes";
			this.comboInsFilingCodes.SelectionModeMulti = true;
			this.comboInsFilingCodes.Size = new System.Drawing.Size(145, 21);
			this.comboInsFilingCodes.TabIndex = 260;
			this.comboInsFilingCodes.SelectionChangeCommitted += new System.EventHandler(this.comboInsFilingCodes_SelectionChangeCommitted);
			// 
			// labelInsFilingCodes
			// 
			this.labelInsFilingCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelInsFilingCodes.Location = new System.Drawing.Point(2, 271);
			this.labelInsFilingCodes.Name = "labelInsFilingCodes";
			this.labelInsFilingCodes.Size = new System.Drawing.Size(129, 16);
			this.labelInsFilingCodes.TabIndex = 259;
			this.labelInsFilingCodes.Text = "Exclude Filing Codes";
			this.labelInsFilingCodes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(70, 532);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.SelectionModeMulti = true;
			this.comboClinic.Size = new System.Drawing.Size(206, 21);
			this.comboClinic.TabIndex = 12;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// checkUseClinicDefaults
			// 
			this.checkUseClinicDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUseClinicDefaults.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseClinicDefaults.Location = new System.Drawing.Point(45, 560);
			this.checkUseClinicDefaults.Name = "checkUseClinicDefaults";
			this.checkUseClinicDefaults.Size = new System.Drawing.Size(231, 18);
			this.checkUseClinicDefaults.TabIndex = 13;
			this.checkUseClinicDefaults.Text = "Use clinic default billing options";
			this.checkUseClinicDefaults.CheckedChanged += new System.EventHandler(this.checkUseClinicDefaults_CheckedChanged);
			// 
			// checkExcludeIfProcs
			// 
			this.checkExcludeIfProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkExcludeIfProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeIfProcs.Location = new System.Drawing.Point(45, 180);
			this.checkExcludeIfProcs.Name = "checkExcludeIfProcs";
			this.checkExcludeIfProcs.Size = new System.Drawing.Size(231, 18);
			this.checkExcludeIfProcs.TabIndex = 6;
			this.checkExcludeIfProcs.Text = "Exclude if unsent dental procedures";
			// 
			// checkIgnoreInPerson
			// 
			this.checkIgnoreInPerson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIgnoreInPerson.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIgnoreInPerson.Location = new System.Drawing.Point(2, 246);
			this.checkIgnoreInPerson.Name = "checkIgnoreInPerson";
			this.checkIgnoreInPerson.Size = new System.Drawing.Size(274, 18);
			this.checkIgnoreInPerson.TabIndex = 9;
			this.checkIgnoreInPerson.Text = "Ignore walkout (InPerson) statements";
			// 
			// labelSaveDefaults
			// 
			this.labelSaveDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSaveDefaults.Location = new System.Drawing.Point(7, 617);
			this.labelSaveDefaults.Name = "labelSaveDefaults";
			this.labelSaveDefaults.Size = new System.Drawing.Size(270, 16);
			this.labelSaveDefaults.TabIndex = 246;
			this.labelSaveDefaults.Text = "(except the date at the top)";
			this.labelSaveDefaults.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(5, 298);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(101, 16);
			this.label7.TabIndex = 245;
			this.label7.Text = "Billing Types";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(3, 75);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(128, 16);
			this.label6.TabIndex = 243;
			this.label6.Text = "Age of Account";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboAge
			// 
			this.comboAge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboAge.Location = new System.Drawing.Point(132, 73);
			this.comboAge.Name = "comboAge";
			this.comboAge.Size = new System.Drawing.Size(145, 21);
			this.comboAge.TabIndex = 2;
			// 
			// checkExcludeInsPending
			// 
			this.checkExcludeInsPending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkExcludeInsPending.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeInsPending.Location = new System.Drawing.Point(45, 137);
			this.checkExcludeInsPending.Name = "checkExcludeInsPending";
			this.checkExcludeInsPending.Size = new System.Drawing.Size(231, 18);
			this.checkExcludeInsPending.TabIndex = 5;
			this.checkExcludeInsPending.Text = "Exclude if insurance pending";
			this.checkExcludeInsPending.Click += new System.EventHandler(this.checkExcludeInsPending_Click);
			// 
			// checkIncludeChanged
			// 
			this.checkIncludeChanged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeChanged.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeChanged.Location = new System.Drawing.Point(3, 39);
			this.checkIncludeChanged.Name = "checkIncludeChanged";
			this.checkIncludeChanged.Size = new System.Drawing.Size(273, 28);
			this.checkIncludeChanged.TabIndex = 1;
			this.checkIncludeChanged.Text = "Include any accounts with insurance payments, procedures, or payplan charges sinc" +
    "e the last bill";
			// 
			// textLastStatement
			// 
			this.textLastStatement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textLastStatement.Location = new System.Drawing.Point(183, 13);
			this.textLastStatement.Name = "textLastStatement";
			this.textLastStatement.Size = new System.Drawing.Size(94, 20);
			this.textLastStatement.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(6, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(176, 16);
			this.label5.TabIndex = 24;
			this.label5.Text = "Include anyone not billed since";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowNegative
			// 
			this.checkShowNegative.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowNegative.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowNegative.Location = new System.Drawing.Point(45, 226);
			this.checkShowNegative.Name = "checkShowNegative";
			this.checkShowNegative.Size = new System.Drawing.Size(231, 18);
			this.checkShowNegative.TabIndex = 8;
			this.checkShowNegative.Text = "Show negative balances (credits)";
			// 
			// checkBadAddress
			// 
			this.checkBadAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBadAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBadAddress.Location = new System.Drawing.Point(45, 97);
			this.checkBadAddress.Name = "checkBadAddress";
			this.checkBadAddress.Size = new System.Drawing.Size(231, 18);
			this.checkBadAddress.TabIndex = 3;
			this.checkBadAddress.Text = "Exclude bad addresses (no zipcode)";
			// 
			// listBillType
			// 
			this.listBillType.Location = new System.Drawing.Point(107, 298);
			this.listBillType.Name = "listBillType";
			this.listBillType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBillType.Size = new System.Drawing.Size(169, 225);
			this.listBillType.TabIndex = 10;
			// 
			// checkBoxBillShowTransSinceZero
			// 
			this.checkBoxBillShowTransSinceZero.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxBillShowTransSinceZero.Location = new System.Drawing.Point(5, 65);
			this.checkBoxBillShowTransSinceZero.Name = "checkBoxBillShowTransSinceZero";
			this.checkBoxBillShowTransSinceZero.Size = new System.Drawing.Size(330, 18);
			this.checkBoxBillShowTransSinceZero.TabIndex = 252;
			this.checkBoxBillShowTransSinceZero.Text = "Show all transactions since zero or negative balance";
			this.checkBoxBillShowTransSinceZero.CheckedChanged += new System.EventHandler(this.checkBoxBillShowTransSinceZero_CheckedChanged);
			// 
			// gridDunning
			// 
			this.gridDunning.Location = new System.Drawing.Point(331, 31);
			this.gridDunning.Name = "gridDunning";
			this.gridDunning.Size = new System.Drawing.Size(561, 362);
			this.gridDunning.TabIndex = 1;
			this.gridDunning.Title = "Dunning Messages";
			this.gridDunning.TranslationName = "TableBillingMessages";
			this.gridDunning.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridDun_CellDoubleClick);
			// 
			// butDunningSetup
			// 
			this.butDunningSetup.Location = new System.Drawing.Point(806, 399);
			this.butDunningSetup.Name = "butDunningSetup";
			this.butDunningSetup.Size = new System.Drawing.Size(86, 24);
			this.butDunningSetup.TabIndex = 3;
			this.butDunningSetup.Text = "Setup Dunning";
			this.butDunningSetup.Click += new System.EventHandler(this.butDunningSetup_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(328, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(564, 16);
			this.label3.TabIndex = 25;
			this.label3.Text = "Items higher in the list are more general.  Items lower in the list take preceden" +
    "ce .";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(328, 545);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(564, 16);
			this.label4.TabIndex = 26;
			this.label4.Text = "General Message (in addition to any dunning messages and appointment reminders, [" +
    "InstallmentPlanTerms] allowed)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(331, 564);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Statement;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(561, 90);
			this.textNote.TabIndex = 9;
			this.textNote.Text = "";
			// 
			// groupDateRange
			// 
			this.groupDateRange.Controls.Add(this.but30days);
			this.groupDateRange.Controls.Add(this.textDateStart);
			this.groupDateRange.Controls.Add(this.checkBoxBillShowTransSinceZero);
			this.groupDateRange.Controls.Add(this.labelStartDate);
			this.groupDateRange.Controls.Add(this.labelEndDate);
			this.groupDateRange.Controls.Add(this.textDateEnd);
			this.groupDateRange.Controls.Add(this.but45days);
			this.groupDateRange.Controls.Add(this.but90days);
			this.groupDateRange.Controls.Add(this.butDatesAll);
			this.groupDateRange.Location = new System.Drawing.Point(331, 399);
			this.groupDateRange.Name = "groupDateRange";
			this.groupDateRange.Size = new System.Drawing.Size(344, 86);
			this.groupDateRange.TabIndex = 2;
			this.groupDateRange.Text = "Account History Date Range";
			// 
			// but30days
			// 
			this.but30days.Location = new System.Drawing.Point(154, 13);
			this.but30days.Name = "but30days";
			this.but30days.Size = new System.Drawing.Size(77, 24);
			this.but30days.TabIndex = 2;
			this.but30days.Text = "Last 30 Days";
			this.but30days.Click += new System.EventHandler(this.but30days_Click);
			// 
			// textDateStart
			// 
			this.textDateStart.BackColor = System.Drawing.SystemColors.Window;
			this.textDateStart.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textDateStart.Location = new System.Drawing.Point(75, 16);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 0;
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(6, 19);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(69, 14);
			this.labelStartDate.TabIndex = 221;
			this.labelStartDate.Text = "Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelEndDate
			// 
			this.labelEndDate.Location = new System.Drawing.Point(6, 42);
			this.labelEndDate.Name = "labelEndDate";
			this.labelEndDate.Size = new System.Drawing.Size(69, 14);
			this.labelEndDate.TabIndex = 222;
			this.labelEndDate.Text = "End Date";
			this.labelEndDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(75, 39);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 1;
			// 
			// but45days
			// 
			this.but45days.Location = new System.Drawing.Point(154, 37);
			this.but45days.Name = "but45days";
			this.but45days.Size = new System.Drawing.Size(77, 24);
			this.but45days.TabIndex = 3;
			this.but45days.Text = "Last 45 Days";
			this.but45days.Click += new System.EventHandler(this.but45days_Click);
			// 
			// but90days
			// 
			this.but90days.Location = new System.Drawing.Point(233, 37);
			this.but90days.Name = "but90days";
			this.but90days.Size = new System.Drawing.Size(77, 24);
			this.but90days.TabIndex = 5;
			this.but90days.Text = "Last 90 Days";
			this.but90days.Click += new System.EventHandler(this.but90days_Click);
			// 
			// butDatesAll
			// 
			this.butDatesAll.Location = new System.Drawing.Point(233, 13);
			this.butDatesAll.Name = "butDatesAll";
			this.butDatesAll.Size = new System.Drawing.Size(77, 24);
			this.butDatesAll.TabIndex = 4;
			this.butDatesAll.Text = "All Dates";
			this.butDatesAll.Click += new System.EventHandler(this.butDatesAll_Click);
			// 
			// checkIntermingled
			// 
			this.checkIntermingled.Location = new System.Drawing.Point(331, 489);
			this.checkIntermingled.Name = "checkIntermingled";
			this.checkIntermingled.Size = new System.Drawing.Size(231, 20);
			this.checkIntermingled.TabIndex = 5;
			this.checkIntermingled.Text = "Intermingle family members";
			this.checkIntermingled.Click += new System.EventHandler(this.checkIntermingled_Click);
			// 
			// butDefaults
			// 
			this.butDefaults.Location = new System.Drawing.Point(681, 434);
			this.butDefaults.Name = "butDefaults";
			this.butDefaults.Size = new System.Drawing.Size(76, 24);
			this.butDefaults.TabIndex = 4;
			this.butDefaults.Text = "Defaults";
			this.butDefaults.Click += new System.EventHandler(this.butDefaults_Click);
			// 
			// butUndo
			// 
			this.butUndo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUndo.Location = new System.Drawing.Point(7, 664);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(88, 24);
			this.butUndo.TabIndex = 10;
			this.butUndo.Text = "Undo Billing";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// checkSuperFam
			// 
			this.checkSuperFam.Location = new System.Drawing.Point(331, 508);
			this.checkSuperFam.Name = "checkSuperFam";
			this.checkSuperFam.Size = new System.Drawing.Size(258, 20);
			this.checkSuperFam.TabIndex = 6;
			this.checkSuperFam.Text = "Group by Super Family";
			this.checkSuperFam.CheckedChanged += new System.EventHandler(this.checkSuperFam_CheckedChanged);
			// 
			// checkSinglePatient
			// 
			this.checkSinglePatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSinglePatient.Location = new System.Drawing.Point(331, 527);
			this.checkSinglePatient.Name = "checkSinglePatient";
			this.checkSinglePatient.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkSinglePatient.Size = new System.Drawing.Size(219, 20);
			this.checkSinglePatient.TabIndex = 7;
			this.checkSinglePatient.Text = "Single patient only";
			this.checkSinglePatient.Visible = false;
			this.checkSinglePatient.Click += new System.EventHandler(this.checkSinglePatient_Click);
			// 
			// labelModesToText
			// 
			this.labelModesToText.Location = new System.Drawing.Point(563, 488);
			this.labelModesToText.Name = "labelModesToText";
			this.labelModesToText.Size = new System.Drawing.Size(200, 27);
			this.labelModesToText.TabIndex = 254;
			this.labelModesToText.Text = "Send text message for these modes (Patients will need a Patient Portal login)";
			this.labelModesToText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listModeToText
			// 
			this.listModeToText.Location = new System.Drawing.Point(760, 488);
			this.listModeToText.Name = "listModeToText";
			this.listModeToText.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listModeToText.Size = new System.Drawing.Size(113, 56);
			this.listModeToText.TabIndex = 8;
			// 
			// labelMultiClinicGenMsg
			// 
			this.labelMultiClinicGenMsg.Location = new System.Drawing.Point(353, 605);
			this.labelMultiClinicGenMsg.Name = "labelMultiClinicGenMsg";
			this.labelMultiClinicGenMsg.Size = new System.Drawing.Size(517, 16);
			this.labelMultiClinicGenMsg.TabIndex = 256;
			this.labelMultiClinicGenMsg.Text = "Practice General Message or Clinic General message(s) will be used, filter for on" +
    "ly one clinic to see message";
			this.labelMultiClinicGenMsg.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelMultiClinicGenMsg.Visible = false;
			// 
			// FormBillingOptions
			// 
			this.AcceptButton = this.butCreate;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(918, 696);
			this.Controls.Add(this.labelMultiClinicGenMsg);
			this.Controls.Add(this.listModeToText);
			this.Controls.Add(this.labelModesToText);
			this.Controls.Add(this.checkSinglePatient);
			this.Controls.Add(this.checkSuperFam);
			this.Controls.Add(this.butUndo);
			this.Controls.Add(this.butDefaults);
			this.Controls.Add(this.checkIntermingled);
			this.Controls.Add(this.groupDateRange);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butCreate);
			this.Controls.Add(this.butDunningSetup);
			this.Controls.Add(this.gridDunning);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormBillingOptions";
			this.ShowInTaskbar = false;
			this.Text = "Billing Options";
			this.Load += new System.EventHandler(this.FormBillingOptions_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupDateRange.ResumeLayout(false);
			this.groupDateRange.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butSaveDefault;
		private OpenDental.ValidDouble textExcludeLessThan;
		private OpenDental.UI.CheckBox checkExcludeInactive;
		private OpenDental.UI.GroupBox groupBox2;
		private OpenDental.UI.Button butDunningSetup;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.GridOD gridDunning;
		private System.Windows.Forms.Label label4;
		private OpenDental.ODtextBox textNote;
		private OpenDental.UI.CheckBox checkBadAddress;
		private OpenDental.UI.CheckBox checkShowNegative;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.CheckBox checkIncludeChanged;
		private OpenDental.ValidDate textLastStatement;
		private OpenDental.UI.Button butCreate;
		private OpenDental.UI.CheckBox checkExcludeInsPending;
		private OpenDental.UI.GroupBox groupDateRange;
		private ValidDate textDateStart;
		private Label labelStartDate;
		private Label labelEndDate;
		private ValidDate textDateEnd;
		private OpenDental.UI.Button but45days;
		private OpenDental.UI.Button but90days;
		private OpenDental.UI.Button butDatesAll;
		private OpenDental.UI.CheckBox checkIntermingled;
		private OpenDental.UI.Button butDefaults;
		private OpenDental.UI.Button but30days;
		private OpenDental.UI.ComboBox comboAge;
		private Label label6;
		private Label label7;
		private OpenDental.UI.ListBox listBillType;
		private Label labelSaveDefaults;
		private OpenDental.UI.Button butUndo;
		private OpenDental.UI.CheckBox checkIgnoreInPerson;
		private OpenDental.UI.CheckBox checkExcludeIfProcs;
		private OpenDental.UI.CheckBox checkSuperFam;
		private OpenDental.UI.CheckBox checkUseClinicDefaults;
		private OpenDental.UI.CheckBox checkBoxBillShowTransSinceZero;
		private OpenDental.UI.CheckBox checkSinglePatient;
		private Label labelModesToText;
		private OpenDental.UI.ListBox listModeToText;
		private Label labelMultiClinicGenMsg;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private Label labelInsFilingCodes;
		private UI.ComboBox comboInsFilingCodes;
		private ValidNum textDaysInsPendingExclude;
		private Label labelExcludeIfPendingDays;
	}
}
