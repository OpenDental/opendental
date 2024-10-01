namespace OpenDental{
	partial class FormInsVerificationList {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsVerificationList));
			this.tabControl1 = new OpenDental.UI.TabControl();
			this.tabVerify = new OpenDental.UI.TabPage();
			this.tabControlVerificationList = new OpenDental.UI.TabControl();
			this.tabCurrent = new OpenDental.UI.TabPage();
			this.gridMain = new OpenDental.UI.GridOD();
			this.tabPastDue = new OpenDental.UI.TabPage();
			this.gridPastDue = new OpenDental.UI.GridOD();
			this.butVerifyPat = new OpenDental.UI.Button();
			this.textPatBirthdate = new System.Windows.Forms.TextBox();
			this.labelPatBirthdate = new System.Windows.Forms.Label();
			this.comboSetVerifyStatus = new OpenDental.UI.ComboBox();
			this.butVerifyPlan = new OpenDental.UI.Button();
			this.labelVerifyStatusSet = new System.Windows.Forms.Label();
			this.textInsVerifyReadOnlyNote = new System.Windows.Forms.TextBox();
			this.labelStatusNote = new System.Windows.Forms.Label();
			this.groupSubscriber = new OpenDental.UI.GroupBox();
			this.textSubscriberID = new System.Windows.Forms.TextBox();
			this.textSubscriberSSN = new System.Windows.Forms.TextBox();
			this.textSubscriberBirthdate = new System.Windows.Forms.TextBox();
			this.textSubscriberName = new System.Windows.Forms.TextBox();
			this.labelSubscriberID = new System.Windows.Forms.Label();
			this.labelSSN = new System.Windows.Forms.Label();
			this.labelBirthdate = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.groupInsurancePlan = new OpenDental.UI.GroupBox();
			this.textInsPlanNote = new System.Windows.Forms.TextBox();
			this.textCarrierPhoneNumber = new OpenDental.ValidPhone();
			this.textInsPlanGroupNumber = new System.Windows.Forms.TextBox();
			this.textInsPlanGroupName = new System.Windows.Forms.TextBox();
			this.textInsPlanEmployer = new System.Windows.Forms.TextBox();
			this.textCarrierName = new System.Windows.Forms.TextBox();
			this.labelPlanNote = new System.Windows.Forms.Label();
			this.labelCarrierPhone = new System.Windows.Forms.Label();
			this.labelGroupNumber = new System.Windows.Forms.Label();
			this.labelGroupName = new System.Windows.Forms.Label();
			this.labelEmployerName = new System.Windows.Forms.Label();
			this.labelCarrierName = new System.Windows.Forms.Label();
			this.tabAssignStandard = new OpenDental.UI.TabPage();
			this.gridAssignStandard = new OpenDental.UI.GridOD();
			this.groupAssignVerificationStandard = new OpenDental.UI.GroupBox();
			this.textInsVerifyNoteStandard = new System.Windows.Forms.TextBox();
			this.labelNoteStandard = new System.Windows.Forms.Label();
			this.textAssignUserStandard = new System.Windows.Forms.TextBox();
			this.labelToUserStandard = new System.Windows.Forms.Label();
			this.butAssignUserPickStandard = new OpenDental.UI.Button();
			this.butAssignUserStandard = new OpenDental.UI.Button();
			this.tabAssignMedicaid = new OpenDental.UI.TabPage();
			this.butAssignUserMedicaid = new OpenDental.UI.Button();
			this.groupAssignVerificationMedicaid = new OpenDental.UI.GroupBox();
			this.textInsVerifyNoteMedicaid = new System.Windows.Forms.TextBox();
			this.labelNoteMedicaid = new System.Windows.Forms.Label();
			this.textAssignUserMedicaid = new System.Windows.Forms.TextBox();
			this.labelToUserMedicaid = new System.Windows.Forms.Label();
			this.butAssignUserPickMedicaid = new OpenDental.UI.Button();
			this.gridAssignMedicaid = new OpenDental.UI.GridOD();
			this.groupVerificationFilters = new OpenDental.UI.GroupBox();
			this.labelMedicaid = new System.Windows.Forms.Label();
			this.labelStandard = new System.Windows.Forms.Label();
			this.textAppointmentScheduledDaysMedicaid = new OpenDental.ValidNum();
			this.textPatientEnrollmentDaysMedicaid = new OpenDental.ValidNum();
			this.textInsBenefitEligibilityDaysMedicaid = new OpenDental.ValidNum();
			this.listBoxVerifyRegions = new OpenDental.UI.ListBox();
			this.listBoxVerifyClinics = new OpenDental.UI.ListBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboVerifyUser = new OpenDental.UI.ComboBox();
			this.labelPlanBenefitsNotVerifiedIn = new System.Windows.Forms.Label();
			this.labelClinic = new System.Windows.Forms.Label();
			this.comboFilterVerifyStatus = new OpenDental.UI.ComboBox();
			this.labelVerifyStatus = new System.Windows.Forms.Label();
			this.textVerifyCarrier = new System.Windows.Forms.TextBox();
			this.labelCarrier = new System.Windows.Forms.Label();
			this.labelRegion = new System.Windows.Forms.Label();
			this.labelForUser = new System.Windows.Forms.Label();
			this.butVerifyUserPick = new OpenDental.UI.Button();
			this.labelDaysUntilSchedAppt = new System.Windows.Forms.Label();
			this.labelDaysSincePatEligibility = new System.Windows.Forms.Label();
			this.textAppointmentScheduledDaysStandard = new OpenDental.ValidNum();
			this.textPatientEnrollmentDaysStandard = new OpenDental.ValidNum();
			this.textInsBenefitEligibilityDaysStandard = new OpenDental.ValidNum();
			this.tabControl1.SuspendLayout();
			this.tabVerify.SuspendLayout();
			this.tabControlVerificationList.SuspendLayout();
			this.tabCurrent.SuspendLayout();
			this.tabPastDue.SuspendLayout();
			this.groupSubscriber.SuspendLayout();
			this.groupInsurancePlan.SuspendLayout();
			this.tabAssignStandard.SuspendLayout();
			this.groupAssignVerificationStandard.SuspendLayout();
			this.tabAssignMedicaid.SuspendLayout();
			this.groupAssignVerificationMedicaid.SuspendLayout();
			this.groupVerificationFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabVerify);
			this.tabControl1.Controls.Add(this.tabAssignStandard);
			this.tabControl1.Controls.Add(this.tabAssignMedicaid);
			this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControl1.Location = new System.Drawing.Point(3, 125);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.Size = new System.Drawing.Size(969, 532);
			this.tabControl1.TabIndex = 101;
			this.tabControl1.Selected += new System.EventHandler(this.tabControl1_Selected);
			// 
			// tabVerify
			// 
			this.tabVerify.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabVerify.Controls.Add(this.tabControlVerificationList);
			this.tabVerify.Controls.Add(this.butVerifyPat);
			this.tabVerify.Controls.Add(this.textPatBirthdate);
			this.tabVerify.Controls.Add(this.labelPatBirthdate);
			this.tabVerify.Controls.Add(this.comboSetVerifyStatus);
			this.tabVerify.Controls.Add(this.butVerifyPlan);
			this.tabVerify.Controls.Add(this.labelVerifyStatusSet);
			this.tabVerify.Controls.Add(this.textInsVerifyReadOnlyNote);
			this.tabVerify.Controls.Add(this.labelStatusNote);
			this.tabVerify.Controls.Add(this.groupSubscriber);
			this.tabVerify.Controls.Add(this.groupInsurancePlan);
			this.tabVerify.Location = new System.Drawing.Point(2, 21);
			this.tabVerify.Name = "tabVerify";
			this.tabVerify.Padding = new System.Windows.Forms.Padding(3);
			this.tabVerify.Size = new System.Drawing.Size(965, 509);
			this.tabVerify.TabIndex = 0;
			this.tabVerify.Text = "Verification List";
			// 
			// tabControlVerificationList
			// 
			this.tabControlVerificationList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabControlVerificationList.Controls.Add(this.tabCurrent);
			this.tabControlVerificationList.Controls.Add(this.tabPastDue);
			this.tabControlVerificationList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControlVerificationList.Location = new System.Drawing.Point(3, 3);
			this.tabControlVerificationList.Name = "tabControlVerificationList";
			this.tabControlVerificationList.Size = new System.Drawing.Size(957, 340);
			this.tabControlVerificationList.TabIndex = 231;
			this.tabControlVerificationList.Selected += new System.EventHandler(this.tabControlVerificationList_Selected);
			// 
			// tabCurrent
			// 
			this.tabCurrent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabCurrent.Controls.Add(this.gridMain);
			this.tabCurrent.Location = new System.Drawing.Point(2, 21);
			this.tabCurrent.Name = "tabCurrent";
			this.tabCurrent.Padding = new System.Windows.Forms.Padding(3);
			this.tabCurrent.Size = new System.Drawing.Size(953, 317);
			this.tabCurrent.TabIndex = 0;
			this.tabCurrent.Text = "Current";
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Location = new System.Drawing.Point(1, 1);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(946, 312);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Insurance Verify List";
			this.gridMain.TranslationName = "TableInsVerify";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// tabPastDue
			// 
			this.tabPastDue.Controls.Add(this.gridPastDue);
			this.tabPastDue.Location = new System.Drawing.Point(2, 21);
			this.tabPastDue.Name = "tabPastDue";
			this.tabPastDue.Padding = new System.Windows.Forms.Padding(3);
			this.tabPastDue.Size = new System.Drawing.Size(953, 317);
			this.tabPastDue.TabIndex = 1;
			this.tabPastDue.Text = "Past Due";
			// 
			// gridPastDue
			// 
			this.gridPastDue.AllowSortingByColumn = true;
			this.gridPastDue.Location = new System.Drawing.Point(1, 1);
			this.gridPastDue.Name = "gridPastDue";
			this.gridPastDue.Size = new System.Drawing.Size(946, 312);
			this.gridPastDue.TabIndex = 5;
			this.gridPastDue.Title = "Insurance Verify List";
			this.gridPastDue.TranslationName = "TableInsVerify";
			this.gridPastDue.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPastDue_CellDoubleClick);
			this.gridPastDue.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPastDue_CellClick);
			// 
			// butVerifyPat
			// 
			this.butVerifyPat.Location = new System.Drawing.Point(294, 483);
			this.butVerifyPat.Name = "butVerifyPat";
			this.butVerifyPat.Size = new System.Drawing.Size(174, 24);
			this.butVerifyPat.TabIndex = 230;
			this.butVerifyPat.Text = "Mark Patient Eligibility Verified";
			this.butVerifyPat.UseVisualStyleBackColor = true;
			this.butVerifyPat.Click += new System.EventHandler(this.butVerifyPat_Click);
			// 
			// textPatBirthdate
			// 
			this.textPatBirthdate.Location = new System.Drawing.Point(594, 458);
			this.textPatBirthdate.Name = "textPatBirthdate";
			this.textPatBirthdate.ReadOnly = true;
			this.textPatBirthdate.Size = new System.Drawing.Size(116, 20);
			this.textPatBirthdate.TabIndex = 229;
			// 
			// labelPatBirthdate
			// 
			this.labelPatBirthdate.Location = new System.Drawing.Point(494, 458);
			this.labelPatBirthdate.Name = "labelPatBirthdate";
			this.labelPatBirthdate.Size = new System.Drawing.Size(99, 20);
			this.labelPatBirthdate.TabIndex = 228;
			this.labelPatBirthdate.Text = "Pat Birthdate";
			this.labelPatBirthdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSetVerifyStatus
			// 
			this.comboSetVerifyStatus.Location = new System.Drawing.Point(801, 359);
			this.comboSetVerifyStatus.Name = "comboSetVerifyStatus";
			this.comboSetVerifyStatus.Size = new System.Drawing.Size(152, 21);
			this.comboSetVerifyStatus.TabIndex = 227;
			this.comboSetVerifyStatus.SelectionChangeCommitted += new System.EventHandler(this.comboSetVerifyStatus_SelectionChangeCommitted);
			// 
			// butVerifyPlan
			// 
			this.butVerifyPlan.Location = new System.Drawing.Point(515, 483);
			this.butVerifyPlan.Name = "butVerifyPlan";
			this.butVerifyPlan.Size = new System.Drawing.Size(153, 24);
			this.butVerifyPlan.TabIndex = 100;
			this.butVerifyPlan.Text = "Mark Ins Benefits Verified";
			this.butVerifyPlan.UseVisualStyleBackColor = true;
			this.butVerifyPlan.Click += new System.EventHandler(this.butVerifyPlan_Click);
			// 
			// labelVerifyStatusSet
			// 
			this.labelVerifyStatusSet.Location = new System.Drawing.Point(717, 360);
			this.labelVerifyStatusSet.Name = "labelVerifyStatusSet";
			this.labelVerifyStatusSet.Size = new System.Drawing.Size(83, 20);
			this.labelVerifyStatusSet.TabIndex = 226;
			this.labelVerifyStatusSet.Text = "Verify Status";
			this.labelVerifyStatusSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsVerifyReadOnlyNote
			// 
			this.textInsVerifyReadOnlyNote.Location = new System.Drawing.Point(801, 382);
			this.textInsVerifyReadOnlyNote.Multiline = true;
			this.textInsVerifyReadOnlyNote.Name = "textInsVerifyReadOnlyNote";
			this.textInsVerifyReadOnlyNote.ReadOnly = true;
			this.textInsVerifyReadOnlyNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textInsVerifyReadOnlyNote.Size = new System.Drawing.Size(152, 84);
			this.textInsVerifyReadOnlyNote.TabIndex = 225;
			// 
			// labelStatusNote
			// 
			this.labelStatusNote.Location = new System.Drawing.Point(716, 384);
			this.labelStatusNote.Name = "labelStatusNote";
			this.labelStatusNote.Size = new System.Drawing.Size(83, 20);
			this.labelStatusNote.TabIndex = 224;
			this.labelStatusNote.Text = "Status Note";
			this.labelStatusNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupSubscriber
			// 
			this.groupSubscriber.Controls.Add(this.textSubscriberID);
			this.groupSubscriber.Controls.Add(this.textSubscriberSSN);
			this.groupSubscriber.Controls.Add(this.textSubscriberBirthdate);
			this.groupSubscriber.Controls.Add(this.textSubscriberName);
			this.groupSubscriber.Controls.Add(this.labelSubscriberID);
			this.groupSubscriber.Controls.Add(this.labelSSN);
			this.groupSubscriber.Controls.Add(this.labelBirthdate);
			this.groupSubscriber.Controls.Add(this.labelName);
			this.groupSubscriber.Location = new System.Drawing.Point(483, 349);
			this.groupSubscriber.Name = "groupSubscriber";
			this.groupSubscriber.Size = new System.Drawing.Size(232, 107);
			this.groupSubscriber.TabIndex = 91;
			this.groupSubscriber.Text = "Subscriber";
			// 
			// textSubscriberID
			// 
			this.textSubscriberID.Location = new System.Drawing.Point(111, 79);
			this.textSubscriberID.Name = "textSubscriberID";
			this.textSubscriberID.ReadOnly = true;
			this.textSubscriberID.Size = new System.Drawing.Size(116, 20);
			this.textSubscriberID.TabIndex = 219;
			// 
			// textSubscriberSSN
			// 
			this.textSubscriberSSN.Location = new System.Drawing.Point(111, 57);
			this.textSubscriberSSN.Name = "textSubscriberSSN";
			this.textSubscriberSSN.ReadOnly = true;
			this.textSubscriberSSN.Size = new System.Drawing.Size(116, 20);
			this.textSubscriberSSN.TabIndex = 218;
			// 
			// textSubscriberBirthdate
			// 
			this.textSubscriberBirthdate.Location = new System.Drawing.Point(111, 35);
			this.textSubscriberBirthdate.Name = "textSubscriberBirthdate";
			this.textSubscriberBirthdate.ReadOnly = true;
			this.textSubscriberBirthdate.Size = new System.Drawing.Size(116, 20);
			this.textSubscriberBirthdate.TabIndex = 217;
			// 
			// textSubscriberName
			// 
			this.textSubscriberName.Location = new System.Drawing.Point(111, 13);
			this.textSubscriberName.Name = "textSubscriberName";
			this.textSubscriberName.ReadOnly = true;
			this.textSubscriberName.Size = new System.Drawing.Size(116, 20);
			this.textSubscriberName.TabIndex = 216;
			// 
			// labelSubscriberID
			// 
			this.labelSubscriberID.Location = new System.Drawing.Point(11, 79);
			this.labelSubscriberID.Name = "labelSubscriberID";
			this.labelSubscriberID.Size = new System.Drawing.Size(99, 20);
			this.labelSubscriberID.TabIndex = 98;
			this.labelSubscriberID.Text = "SubscriberID";
			this.labelSubscriberID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSSN
			// 
			this.labelSSN.Location = new System.Drawing.Point(11, 57);
			this.labelSSN.Name = "labelSSN";
			this.labelSSN.Size = new System.Drawing.Size(99, 20);
			this.labelSSN.TabIndex = 97;
			this.labelSSN.Text = "SSN";
			this.labelSSN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBirthdate
			// 
			this.labelBirthdate.Location = new System.Drawing.Point(11, 35);
			this.labelBirthdate.Name = "labelBirthdate";
			this.labelBirthdate.Size = new System.Drawing.Size(99, 20);
			this.labelBirthdate.TabIndex = 96;
			this.labelBirthdate.Text = "Birthdate";
			this.labelBirthdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(11, 13);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(99, 20);
			this.labelName.TabIndex = 87;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupInsurancePlan
			// 
			this.groupInsurancePlan.Controls.Add(this.textInsPlanNote);
			this.groupInsurancePlan.Controls.Add(this.textCarrierPhoneNumber);
			this.groupInsurancePlan.Controls.Add(this.textInsPlanGroupNumber);
			this.groupInsurancePlan.Controls.Add(this.textInsPlanGroupName);
			this.groupInsurancePlan.Controls.Add(this.textInsPlanEmployer);
			this.groupInsurancePlan.Controls.Add(this.textCarrierName);
			this.groupInsurancePlan.Controls.Add(this.labelPlanNote);
			this.groupInsurancePlan.Controls.Add(this.labelCarrierPhone);
			this.groupInsurancePlan.Controls.Add(this.labelGroupNumber);
			this.groupInsurancePlan.Controls.Add(this.labelGroupName);
			this.groupInsurancePlan.Controls.Add(this.labelEmployerName);
			this.groupInsurancePlan.Controls.Add(this.labelCarrierName);
			this.groupInsurancePlan.Location = new System.Drawing.Point(3, 349);
			this.groupInsurancePlan.Name = "groupInsurancePlan";
			this.groupInsurancePlan.Size = new System.Drawing.Size(480, 131);
			this.groupInsurancePlan.TabIndex = 99;
			this.groupInsurancePlan.Text = "Insurance Plan";
			// 
			// textInsPlanNote
			// 
			this.textInsPlanNote.Location = new System.Drawing.Point(249, 37);
			this.textInsPlanNote.Multiline = true;
			this.textInsPlanNote.Name = "textInsPlanNote";
			this.textInsPlanNote.ReadOnly = true;
			this.textInsPlanNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textInsPlanNote.Size = new System.Drawing.Size(225, 86);
			this.textInsPlanNote.TabIndex = 221;
			// 
			// textCarrierPhoneNumber
			// 
			this.textCarrierPhoneNumber.Location = new System.Drawing.Point(127, 37);
			this.textCarrierPhoneNumber.Name = "textCarrierPhoneNumber";
			this.textCarrierPhoneNumber.ReadOnly = true;
			this.textCarrierPhoneNumber.Size = new System.Drawing.Size(116, 20);
			this.textCarrierPhoneNumber.TabIndex = 220;
			// 
			// textInsPlanGroupNumber
			// 
			this.textInsPlanGroupNumber.Location = new System.Drawing.Point(127, 103);
			this.textInsPlanGroupNumber.Name = "textInsPlanGroupNumber";
			this.textInsPlanGroupNumber.ReadOnly = true;
			this.textInsPlanGroupNumber.Size = new System.Drawing.Size(116, 20);
			this.textInsPlanGroupNumber.TabIndex = 219;
			// 
			// textInsPlanGroupName
			// 
			this.textInsPlanGroupName.Location = new System.Drawing.Point(127, 81);
			this.textInsPlanGroupName.Name = "textInsPlanGroupName";
			this.textInsPlanGroupName.ReadOnly = true;
			this.textInsPlanGroupName.Size = new System.Drawing.Size(116, 20);
			this.textInsPlanGroupName.TabIndex = 218;
			// 
			// textInsPlanEmployer
			// 
			this.textInsPlanEmployer.Location = new System.Drawing.Point(127, 59);
			this.textInsPlanEmployer.Name = "textInsPlanEmployer";
			this.textInsPlanEmployer.ReadOnly = true;
			this.textInsPlanEmployer.Size = new System.Drawing.Size(116, 20);
			this.textInsPlanEmployer.TabIndex = 217;
			// 
			// textCarrierName
			// 
			this.textCarrierName.Location = new System.Drawing.Point(127, 15);
			this.textCarrierName.Name = "textCarrierName";
			this.textCarrierName.ReadOnly = true;
			this.textCarrierName.Size = new System.Drawing.Size(116, 20);
			this.textCarrierName.TabIndex = 216;
			// 
			// labelPlanNote
			// 
			this.labelPlanNote.Location = new System.Drawing.Point(249, 15);
			this.labelPlanNote.Name = "labelPlanNote";
			this.labelPlanNote.Size = new System.Drawing.Size(225, 20);
			this.labelPlanNote.TabIndex = 104;
			this.labelPlanNote.Text = "Plan Note";
			this.labelPlanNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCarrierPhone
			// 
			this.labelCarrierPhone.Location = new System.Drawing.Point(13, 37);
			this.labelCarrierPhone.Name = "labelCarrierPhone";
			this.labelCarrierPhone.Size = new System.Drawing.Size(113, 20);
			this.labelCarrierPhone.TabIndex = 99;
			this.labelCarrierPhone.Text = "Carrier Phone";
			this.labelCarrierPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelGroupNumber
			// 
			this.labelGroupNumber.Location = new System.Drawing.Point(13, 103);
			this.labelGroupNumber.Name = "labelGroupNumber";
			this.labelGroupNumber.Size = new System.Drawing.Size(113, 20);
			this.labelGroupNumber.TabIndex = 98;
			this.labelGroupNumber.Text = "Group Number";
			this.labelGroupNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelGroupName
			// 
			this.labelGroupName.Location = new System.Drawing.Point(13, 81);
			this.labelGroupName.Name = "labelGroupName";
			this.labelGroupName.Size = new System.Drawing.Size(113, 20);
			this.labelGroupName.TabIndex = 97;
			this.labelGroupName.Text = "Group Name";
			this.labelGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEmployerName
			// 
			this.labelEmployerName.Location = new System.Drawing.Point(13, 59);
			this.labelEmployerName.Name = "labelEmployerName";
			this.labelEmployerName.Size = new System.Drawing.Size(113, 20);
			this.labelEmployerName.TabIndex = 96;
			this.labelEmployerName.Text = "Employer Name";
			this.labelEmployerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCarrierName
			// 
			this.labelCarrierName.Location = new System.Drawing.Point(13, 16);
			this.labelCarrierName.Name = "labelCarrierName";
			this.labelCarrierName.Size = new System.Drawing.Size(113, 16);
			this.labelCarrierName.TabIndex = 87;
			this.labelCarrierName.Text = "Carrier Name";
			this.labelCarrierName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabAssignStandard
			// 
			this.tabAssignStandard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.tabAssignStandard.Controls.Add(this.gridAssignStandard);
			this.tabAssignStandard.Controls.Add(this.groupAssignVerificationStandard);
			this.tabAssignStandard.Controls.Add(this.butAssignUserStandard);
			this.tabAssignStandard.Location = new System.Drawing.Point(2, 21);
			this.tabAssignStandard.Name = "tabAssignStandard";
			this.tabAssignStandard.Padding = new System.Windows.Forms.Padding(3);
			this.tabAssignStandard.Size = new System.Drawing.Size(965, 509);
			this.tabAssignStandard.TabIndex = 1;
			this.tabAssignStandard.Text = "Assign Standard Verification";
			// 
			// gridAssignStandard
			// 
			this.gridAssignStandard.AllowSortingByColumn = true;
			this.gridAssignStandard.Location = new System.Drawing.Point(3, 3);
			this.gridAssignStandard.Name = "gridAssignStandard";
			this.gridAssignStandard.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAssignStandard.Size = new System.Drawing.Size(950, 343);
			this.gridAssignStandard.TabIndex = 5;
			this.gridAssignStandard.Title = "Insurance Verification Assignment List";
			this.gridAssignStandard.TranslationName = "TableInsVerifyAssign";
			// 
			// groupAssignVerificationStandard
			// 
			this.groupAssignVerificationStandard.Controls.Add(this.textInsVerifyNoteStandard);
			this.groupAssignVerificationStandard.Controls.Add(this.labelNoteStandard);
			this.groupAssignVerificationStandard.Controls.Add(this.textAssignUserStandard);
			this.groupAssignVerificationStandard.Controls.Add(this.labelToUserStandard);
			this.groupAssignVerificationStandard.Controls.Add(this.butAssignUserPickStandard);
			this.groupAssignVerificationStandard.Location = new System.Drawing.Point(3, 349);
			this.groupAssignVerificationStandard.Name = "groupAssignVerificationStandard";
			this.groupAssignVerificationStandard.Size = new System.Drawing.Size(950, 134);
			this.groupAssignVerificationStandard.TabIndex = 82;
			this.groupAssignVerificationStandard.Text = "Assign Standard Verification";
			// 
			// textInsVerifyNoteStandard
			// 
			this.textInsVerifyNoteStandard.Location = new System.Drawing.Point(410, 17);
			this.textInsVerifyNoteStandard.Multiline = true;
			this.textInsVerifyNoteStandard.Name = "textInsVerifyNoteStandard";
			this.textInsVerifyNoteStandard.Size = new System.Drawing.Size(193, 107);
			this.textInsVerifyNoteStandard.TabIndex = 223;
			// 
			// labelNoteStandard
			// 
			this.labelNoteStandard.Location = new System.Drawing.Point(310, 16);
			this.labelNoteStandard.Name = "labelNoteStandard";
			this.labelNoteStandard.Size = new System.Drawing.Size(94, 20);
			this.labelNoteStandard.TabIndex = 222;
			this.labelNoteStandard.Text = "Note:";
			this.labelNoteStandard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAssignUserStandard
			// 
			this.textAssignUserStandard.Location = new System.Drawing.Point(109, 18);
			this.textAssignUserStandard.Name = "textAssignUserStandard";
			this.textAssignUserStandard.ReadOnly = true;
			this.textAssignUserStandard.Size = new System.Drawing.Size(165, 20);
			this.textAssignUserStandard.TabIndex = 217;
			// 
			// labelToUserStandard
			// 
			this.labelToUserStandard.Location = new System.Drawing.Point(6, 18);
			this.labelToUserStandard.Name = "labelToUserStandard";
			this.labelToUserStandard.Size = new System.Drawing.Size(97, 20);
			this.labelToUserStandard.TabIndex = 86;
			this.labelToUserStandard.Text = "To User:";
			this.labelToUserStandard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAssignUserPickStandard
			// 
			this.butAssignUserPickStandard.Location = new System.Drawing.Point(275, 18);
			this.butAssignUserPickStandard.Name = "butAssignUserPickStandard";
			this.butAssignUserPickStandard.Size = new System.Drawing.Size(27, 20);
			this.butAssignUserPickStandard.TabIndex = 85;
			this.butAssignUserPickStandard.Text = "...";
			this.butAssignUserPickStandard.Click += new System.EventHandler(this.butAssignUserPick_Click);
			// 
			// butAssignUserStandard
			// 
			this.butAssignUserStandard.Location = new System.Drawing.Point(441, 485);
			this.butAssignUserStandard.Name = "butAssignUserStandard";
			this.butAssignUserStandard.Size = new System.Drawing.Size(75, 24);
			this.butAssignUserStandard.TabIndex = 81;
			this.butAssignUserStandard.Text = "Assign";
			this.butAssignUserStandard.UseVisualStyleBackColor = true;
			this.butAssignUserStandard.Click += new System.EventHandler(this.butAssignUser_Click);
			// 
			// tabAssignMedicaid
			// 
			this.tabAssignMedicaid.Controls.Add(this.butAssignUserMedicaid);
			this.tabAssignMedicaid.Controls.Add(this.groupAssignVerificationMedicaid);
			this.tabAssignMedicaid.Controls.Add(this.gridAssignMedicaid);
			this.tabAssignMedicaid.Location = new System.Drawing.Point(2, 21);
			this.tabAssignMedicaid.Name = "tabAssignMedicaid";
			this.tabAssignMedicaid.Size = new System.Drawing.Size(965, 509);
			this.tabAssignMedicaid.TabIndex = 2;
			this.tabAssignMedicaid.Text = "Assign Medicaid Verification";
			// 
			// butAssignUserMedicaid
			// 
			this.butAssignUserMedicaid.Location = new System.Drawing.Point(441, 485);
			this.butAssignUserMedicaid.Name = "butAssignUserMedicaid";
			this.butAssignUserMedicaid.Size = new System.Drawing.Size(75, 24);
			this.butAssignUserMedicaid.TabIndex = 84;
			this.butAssignUserMedicaid.Text = "Assign";
			this.butAssignUserMedicaid.UseVisualStyleBackColor = true;
			this.butAssignUserMedicaid.Click += new System.EventHandler(this.butAssignUser_Click);
			// 
			// groupAssignVerificationMedicaid
			// 
			this.groupAssignVerificationMedicaid.BackColor = System.Drawing.Color.White;
			this.groupAssignVerificationMedicaid.Controls.Add(this.textInsVerifyNoteMedicaid);
			this.groupAssignVerificationMedicaid.Controls.Add(this.labelNoteMedicaid);
			this.groupAssignVerificationMedicaid.Controls.Add(this.textAssignUserMedicaid);
			this.groupAssignVerificationMedicaid.Controls.Add(this.labelToUserMedicaid);
			this.groupAssignVerificationMedicaid.Controls.Add(this.butAssignUserPickMedicaid);
			this.groupAssignVerificationMedicaid.Location = new System.Drawing.Point(3, 349);
			this.groupAssignVerificationMedicaid.Name = "groupAssignVerificationMedicaid";
			this.groupAssignVerificationMedicaid.Size = new System.Drawing.Size(950, 134);
			this.groupAssignVerificationMedicaid.TabIndex = 83;
			this.groupAssignVerificationMedicaid.Text = "Assign Medicaid Verification";
			// 
			// textInsVerifyNoteMedicaid
			// 
			this.textInsVerifyNoteMedicaid.Location = new System.Drawing.Point(410, 17);
			this.textInsVerifyNoteMedicaid.Multiline = true;
			this.textInsVerifyNoteMedicaid.Name = "textInsVerifyNoteMedicaid";
			this.textInsVerifyNoteMedicaid.Size = new System.Drawing.Size(193, 107);
			this.textInsVerifyNoteMedicaid.TabIndex = 223;
			// 
			// labelNoteMedicaid
			// 
			this.labelNoteMedicaid.Location = new System.Drawing.Point(310, 16);
			this.labelNoteMedicaid.Name = "labelNoteMedicaid";
			this.labelNoteMedicaid.Size = new System.Drawing.Size(94, 20);
			this.labelNoteMedicaid.TabIndex = 222;
			this.labelNoteMedicaid.Text = "Note:";
			this.labelNoteMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAssignUserMedicaid
			// 
			this.textAssignUserMedicaid.Location = new System.Drawing.Point(109, 18);
			this.textAssignUserMedicaid.Name = "textAssignUserMedicaid";
			this.textAssignUserMedicaid.ReadOnly = true;
			this.textAssignUserMedicaid.Size = new System.Drawing.Size(165, 20);
			this.textAssignUserMedicaid.TabIndex = 217;
			// 
			// labelToUserMedicaid
			// 
			this.labelToUserMedicaid.Location = new System.Drawing.Point(6, 18);
			this.labelToUserMedicaid.Name = "labelToUserMedicaid";
			this.labelToUserMedicaid.Size = new System.Drawing.Size(97, 20);
			this.labelToUserMedicaid.TabIndex = 86;
			this.labelToUserMedicaid.Text = "To User:";
			this.labelToUserMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAssignUserPickMedicaid
			// 
			this.butAssignUserPickMedicaid.Location = new System.Drawing.Point(275, 18);
			this.butAssignUserPickMedicaid.Name = "butAssignUserPickMedicaid";
			this.butAssignUserPickMedicaid.Size = new System.Drawing.Size(27, 20);
			this.butAssignUserPickMedicaid.TabIndex = 85;
			this.butAssignUserPickMedicaid.Text = "...";
			this.butAssignUserPickMedicaid.Click += new System.EventHandler(this.butAssignUserPick_Click);
			// 
			// gridAssignMedicaid
			// 
			this.gridAssignMedicaid.AllowSortingByColumn = true;
			this.gridAssignMedicaid.Location = new System.Drawing.Point(3, 3);
			this.gridAssignMedicaid.Name = "gridAssignMedicaid";
			this.gridAssignMedicaid.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAssignMedicaid.Size = new System.Drawing.Size(950, 343);
			this.gridAssignMedicaid.TabIndex = 6;
			this.gridAssignMedicaid.Title = "Insurance Verification Assignment List";
			this.gridAssignMedicaid.TranslationName = "TableInsVerifyAssign";
			// 
			// groupVerificationFilters
			// 
			this.groupVerificationFilters.Controls.Add(this.labelMedicaid);
			this.groupVerificationFilters.Controls.Add(this.labelStandard);
			this.groupVerificationFilters.Controls.Add(this.textAppointmentScheduledDaysMedicaid);
			this.groupVerificationFilters.Controls.Add(this.textPatientEnrollmentDaysMedicaid);
			this.groupVerificationFilters.Controls.Add(this.textInsBenefitEligibilityDaysMedicaid);
			this.groupVerificationFilters.Controls.Add(this.listBoxVerifyRegions);
			this.groupVerificationFilters.Controls.Add(this.listBoxVerifyClinics);
			this.groupVerificationFilters.Controls.Add(this.butRefresh);
			this.groupVerificationFilters.Controls.Add(this.comboVerifyUser);
			this.groupVerificationFilters.Controls.Add(this.labelPlanBenefitsNotVerifiedIn);
			this.groupVerificationFilters.Controls.Add(this.labelClinic);
			this.groupVerificationFilters.Controls.Add(this.comboFilterVerifyStatus);
			this.groupVerificationFilters.Controls.Add(this.labelVerifyStatus);
			this.groupVerificationFilters.Controls.Add(this.textVerifyCarrier);
			this.groupVerificationFilters.Controls.Add(this.labelCarrier);
			this.groupVerificationFilters.Controls.Add(this.labelRegion);
			this.groupVerificationFilters.Controls.Add(this.labelForUser);
			this.groupVerificationFilters.Controls.Add(this.butVerifyUserPick);
			this.groupVerificationFilters.Controls.Add(this.labelDaysUntilSchedAppt);
			this.groupVerificationFilters.Controls.Add(this.labelDaysSincePatEligibility);
			this.groupVerificationFilters.Controls.Add(this.textAppointmentScheduledDaysStandard);
			this.groupVerificationFilters.Controls.Add(this.textPatientEnrollmentDaysStandard);
			this.groupVerificationFilters.Controls.Add(this.textInsBenefitEligibilityDaysStandard);
			this.groupVerificationFilters.Location = new System.Drawing.Point(3, 4);
			this.groupVerificationFilters.Name = "groupVerificationFilters";
			this.groupVerificationFilters.Size = new System.Drawing.Size(969, 119);
			this.groupVerificationFilters.TabIndex = 90;
			this.groupVerificationFilters.Text = "Verification Filters";
			// 
			// labelMedicaid
			// 
			this.labelMedicaid.Location = new System.Drawing.Point(242, 10);
			this.labelMedicaid.Name = "labelMedicaid";
			this.labelMedicaid.Size = new System.Drawing.Size(60, 20);
			this.labelMedicaid.TabIndex = 239;
			this.labelMedicaid.Text = "Medicaid";
			this.labelMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelStandard
			// 
			this.labelStandard.Location = new System.Drawing.Point(184, 10);
			this.labelStandard.Name = "labelStandard";
			this.labelStandard.Size = new System.Drawing.Size(60, 20);
			this.labelStandard.TabIndex = 238;
			this.labelStandard.Text = "Standard";
			this.labelStandard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textAppointmentScheduledDaysMedicaid
			// 
			this.textAppointmentScheduledDaysMedicaid.Location = new System.Drawing.Point(252, 31);
			this.textAppointmentScheduledDaysMedicaid.MaxVal = 99999;
			this.textAppointmentScheduledDaysMedicaid.Name = "textAppointmentScheduledDaysMedicaid";
			this.textAppointmentScheduledDaysMedicaid.ShowZero = false;
			this.textAppointmentScheduledDaysMedicaid.Size = new System.Drawing.Size(40, 20);
			this.textAppointmentScheduledDaysMedicaid.TabIndex = 235;
			this.textAppointmentScheduledDaysMedicaid.Text = "0";
			// 
			// textPatientEnrollmentDaysMedicaid
			// 
			this.textPatientEnrollmentDaysMedicaid.Location = new System.Drawing.Point(252, 77);
			this.textPatientEnrollmentDaysMedicaid.MaxVal = 99999;
			this.textPatientEnrollmentDaysMedicaid.Name = "textPatientEnrollmentDaysMedicaid";
			this.textPatientEnrollmentDaysMedicaid.ShowZero = false;
			this.textPatientEnrollmentDaysMedicaid.Size = new System.Drawing.Size(40, 20);
			this.textPatientEnrollmentDaysMedicaid.TabIndex = 237;
			this.textPatientEnrollmentDaysMedicaid.Text = "0";
			// 
			// textInsBenefitEligibilityDaysMedicaid
			// 
			this.textInsBenefitEligibilityDaysMedicaid.Location = new System.Drawing.Point(252, 54);
			this.textInsBenefitEligibilityDaysMedicaid.MaxVal = 99999;
			this.textInsBenefitEligibilityDaysMedicaid.Name = "textInsBenefitEligibilityDaysMedicaid";
			this.textInsBenefitEligibilityDaysMedicaid.ShowZero = false;
			this.textInsBenefitEligibilityDaysMedicaid.Size = new System.Drawing.Size(40, 20);
			this.textInsBenefitEligibilityDaysMedicaid.TabIndex = 236;
			this.textInsBenefitEligibilityDaysMedicaid.Text = "0";
			// 
			// listBoxVerifyRegions
			// 
			this.listBoxVerifyRegions.Location = new System.Drawing.Point(371, 31);
			this.listBoxVerifyRegions.Name = "listBoxVerifyRegions";
			this.listBoxVerifyRegions.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxVerifyRegions.Size = new System.Drawing.Size(147, 82);
			this.listBoxVerifyRegions.TabIndex = 234;
			this.listBoxVerifyRegions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBoxVerifyRegions_MouseUp);
			// 
			// listBoxVerifyClinics
			// 
			this.listBoxVerifyClinics.Location = new System.Drawing.Point(572, 5);
			this.listBoxVerifyClinics.Name = "listBoxVerifyClinics";
			this.listBoxVerifyClinics.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxVerifyClinics.Size = new System.Drawing.Size(155, 108);
			this.listBoxVerifyClinics.TabIndex = 233;
			this.listBoxVerifyClinics.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBoxVerifyClinics_MouseUp);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(882, 89);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 232;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboVerifyUser
			// 
			this.comboVerifyUser.Location = new System.Drawing.Point(810, 10);
			this.comboVerifyUser.Name = "comboVerifyUser";
			this.comboVerifyUser.Size = new System.Drawing.Size(124, 21);
			this.comboVerifyUser.TabIndex = 231;
			this.comboVerifyUser.SelectionChangeCommitted += new System.EventHandler(this.comboVerifyUser_SelectionChangeCommitted);
			// 
			// labelPlanBenefitsNotVerifiedIn
			// 
			this.labelPlanBenefitsNotVerifiedIn.Location = new System.Drawing.Point(6, 54);
			this.labelPlanBenefitsNotVerifiedIn.Name = "labelPlanBenefitsNotVerifiedIn";
			this.labelPlanBenefitsNotVerifiedIn.Size = new System.Drawing.Size(186, 20);
			this.labelPlanBenefitsNotVerifiedIn.TabIndex = 230;
			this.labelPlanBenefitsNotVerifiedIn.Text = "Plan benefits not verified in";
			this.labelPlanBenefitsNotVerifiedIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(521, 5);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(49, 20);
			this.labelClinic.TabIndex = 228;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFilterVerifyStatus
			// 
			this.comboFilterVerifyStatus.Location = new System.Drawing.Point(810, 33);
			this.comboFilterVerifyStatus.Name = "comboFilterVerifyStatus";
			this.comboFilterVerifyStatus.Size = new System.Drawing.Size(152, 21);
			this.comboFilterVerifyStatus.TabIndex = 227;
			this.comboFilterVerifyStatus.SelectionChangeCommitted += new System.EventHandler(this.comboFilterVerifyStatus_SelectionChangeCommitted);
			// 
			// labelVerifyStatus
			// 
			this.labelVerifyStatus.Location = new System.Drawing.Point(731, 34);
			this.labelVerifyStatus.Name = "labelVerifyStatus";
			this.labelVerifyStatus.Size = new System.Drawing.Size(79, 20);
			this.labelVerifyStatus.TabIndex = 226;
			this.labelVerifyStatus.Text = "Verify Status";
			this.labelVerifyStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVerifyCarrier
			// 
			this.textVerifyCarrier.Location = new System.Drawing.Point(371, 5);
			this.textVerifyCarrier.Name = "textVerifyCarrier";
			this.textVerifyCarrier.Size = new System.Drawing.Size(147, 20);
			this.textVerifyCarrier.TabIndex = 225;
			// 
			// labelCarrier
			// 
			this.labelCarrier.Location = new System.Drawing.Point(315, 5);
			this.labelCarrier.Name = "labelCarrier";
			this.labelCarrier.Size = new System.Drawing.Size(56, 20);
			this.labelCarrier.TabIndex = 223;
			this.labelCarrier.Text = "Carrier";
			this.labelCarrier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRegion
			// 
			this.labelRegion.Location = new System.Drawing.Point(312, 31);
			this.labelRegion.Name = "labelRegion";
			this.labelRegion.Size = new System.Drawing.Size(59, 20);
			this.labelRegion.TabIndex = 221;
			this.labelRegion.Text = "Region";
			this.labelRegion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelForUser
			// 
			this.labelForUser.Location = new System.Drawing.Point(731, 11);
			this.labelForUser.Name = "labelForUser";
			this.labelForUser.Size = new System.Drawing.Size(79, 20);
			this.labelForUser.TabIndex = 220;
			this.labelForUser.Text = "For User";
			this.labelForUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butVerifyUserPick
			// 
			this.butVerifyUserPick.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.butVerifyUserPick.Location = new System.Drawing.Point(935, 10);
			this.butVerifyUserPick.Name = "butVerifyUserPick";
			this.butVerifyUserPick.Size = new System.Drawing.Size(27, 20);
			this.butVerifyUserPick.TabIndex = 219;
			this.butVerifyUserPick.Text = "...";
			this.butVerifyUserPick.Click += new System.EventHandler(this.butVerifyUserPick_Click);
			// 
			// labelDaysUntilSchedAppt
			// 
			this.labelDaysUntilSchedAppt.Location = new System.Drawing.Point(6, 31);
			this.labelDaysUntilSchedAppt.Name = "labelDaysUntilSchedAppt";
			this.labelDaysUntilSchedAppt.Size = new System.Drawing.Size(186, 20);
			this.labelDaysUntilSchedAppt.TabIndex = 77;
			this.labelDaysUntilSchedAppt.Text = "Days until scheduled appointment";
			this.labelDaysUntilSchedAppt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDaysSincePatEligibility
			// 
			this.labelDaysSincePatEligibility.Location = new System.Drawing.Point(6, 77);
			this.labelDaysSincePatEligibility.Name = "labelDaysSincePatEligibility";
			this.labelDaysSincePatEligibility.Size = new System.Drawing.Size(186, 20);
			this.labelDaysSincePatEligibility.TabIndex = 88;
			this.labelDaysSincePatEligibility.Text = "Days since patient eligibility";
			this.labelDaysSincePatEligibility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAppointmentScheduledDaysStandard
			// 
			this.textAppointmentScheduledDaysStandard.Location = new System.Drawing.Point(195, 31);
			this.textAppointmentScheduledDaysStandard.MaxVal = 99999;
			this.textAppointmentScheduledDaysStandard.Name = "textAppointmentScheduledDaysStandard";
			this.textAppointmentScheduledDaysStandard.ShowZero = false;
			this.textAppointmentScheduledDaysStandard.Size = new System.Drawing.Size(40, 20);
			this.textAppointmentScheduledDaysStandard.TabIndex = 78;
			this.textAppointmentScheduledDaysStandard.Text = "0";
			// 
			// textPatientEnrollmentDaysStandard
			// 
			this.textPatientEnrollmentDaysStandard.Location = new System.Drawing.Point(195, 77);
			this.textPatientEnrollmentDaysStandard.MaxVal = 99999;
			this.textPatientEnrollmentDaysStandard.Name = "textPatientEnrollmentDaysStandard";
			this.textPatientEnrollmentDaysStandard.ShowZero = false;
			this.textPatientEnrollmentDaysStandard.Size = new System.Drawing.Size(40, 20);
			this.textPatientEnrollmentDaysStandard.TabIndex = 89;
			this.textPatientEnrollmentDaysStandard.Text = "0";
			// 
			// textInsBenefitEligibilityDaysStandard
			// 
			this.textInsBenefitEligibilityDaysStandard.Location = new System.Drawing.Point(195, 54);
			this.textInsBenefitEligibilityDaysStandard.MaxVal = 99999;
			this.textInsBenefitEligibilityDaysStandard.Name = "textInsBenefitEligibilityDaysStandard";
			this.textInsBenefitEligibilityDaysStandard.ShowZero = false;
			this.textInsBenefitEligibilityDaysStandard.Size = new System.Drawing.Size(40, 20);
			this.textInsBenefitEligibilityDaysStandard.TabIndex = 87;
			this.textInsBenefitEligibilityDaysStandard.Text = "0";
			// 
			// FormInsVerificationList
			// 
			this.ClientSize = new System.Drawing.Size(974, 661);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.groupVerificationFilters);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsVerificationList";
			this.Text = "Insurance Verification List";
			this.Load += new System.EventHandler(this.FormInsVerificationList_Load);
			this.Shown += new System.EventHandler(this.FormInsVerificationList_Shown);
			this.tabControl1.ResumeLayout(false);
			this.tabVerify.ResumeLayout(false);
			this.tabVerify.PerformLayout();
			this.tabControlVerificationList.ResumeLayout(false);
			this.tabCurrent.ResumeLayout(false);
			this.tabPastDue.ResumeLayout(false);
			this.groupSubscriber.ResumeLayout(false);
			this.groupSubscriber.PerformLayout();
			this.groupInsurancePlan.ResumeLayout(false);
			this.groupInsurancePlan.PerformLayout();
			this.tabAssignStandard.ResumeLayout(false);
			this.groupAssignVerificationStandard.ResumeLayout(false);
			this.groupAssignVerificationStandard.PerformLayout();
			this.tabAssignMedicaid.ResumeLayout(false);
			this.groupAssignVerificationMedicaid.ResumeLayout(false);
			this.groupAssignVerificationMedicaid.PerformLayout();
			this.groupVerificationFilters.ResumeLayout(false);
			this.groupVerificationFilters.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelDaysUntilSchedAppt;
		private ValidNum textAppointmentScheduledDaysStandard;
		private ValidNum textInsBenefitEligibilityDaysStandard;
		private System.Windows.Forms.Label labelDaysSincePatEligibility;
		private ValidNum textPatientEnrollmentDaysStandard;
		private OpenDental.UI.GroupBox groupVerificationFilters;
		private OpenDental.UI.GroupBox groupSubscriber;
		private System.Windows.Forms.Label labelSubscriberID;
		private System.Windows.Forms.Label labelSSN;
		private System.Windows.Forms.Label labelBirthdate;
		private System.Windows.Forms.Label labelName;
		private OpenDental.UI.GroupBox groupInsurancePlan;
		private System.Windows.Forms.Label labelPlanNote;
		private System.Windows.Forms.Label labelCarrierPhone;
		private System.Windows.Forms.Label labelGroupNumber;
		private System.Windows.Forms.Label labelGroupName;
		private System.Windows.Forms.Label labelEmployerName;
		private System.Windows.Forms.Label labelCarrierName;
		private System.Windows.Forms.TextBox textSubscriberID;
		private System.Windows.Forms.TextBox textSubscriberSSN;
		private System.Windows.Forms.TextBox textSubscriberBirthdate;
		private System.Windows.Forms.TextBox textSubscriberName;
		private System.Windows.Forms.TextBox textInsPlanNote;
		private ValidPhone textCarrierPhoneNumber;
		private System.Windows.Forms.TextBox textInsPlanGroupNumber;
		private System.Windows.Forms.TextBox textInsPlanGroupName;
		private System.Windows.Forms.TextBox textInsPlanEmployer;
		private System.Windows.Forms.TextBox textCarrierName;
		private OpenDental.UI.GroupBox groupAssignVerificationStandard;
		private System.Windows.Forms.TextBox textAssignUserStandard;
		private System.Windows.Forms.Label labelToUserStandard;
		private UI.Button butAssignUserPickStandard;
		private UI.Button butAssignUserStandard;
		private UI.Button butVerifyUserPick;
		private System.Windows.Forms.Label labelForUser;
		private System.Windows.Forms.Label labelRegion;
		private System.Windows.Forms.Label labelCarrier;
		private System.Windows.Forms.TextBox textVerifyCarrier;
		private System.Windows.Forms.Label labelVerifyStatus;
		private OpenDental.UI.ComboBox comboFilterVerifyStatus;
		private UI.Button butVerifyPlan;
		private System.Windows.Forms.TextBox textInsVerifyNoteStandard;
		private System.Windows.Forms.Label labelNoteStandard;
		private OpenDental.UI.TabControl tabControl1;
		private OpenDental.UI.TabPage tabVerify;
		private OpenDental.UI.TabPage tabAssignStandard;
		private UI.GridOD gridAssignStandard;
		private System.Windows.Forms.TextBox textInsVerifyReadOnlyNote;
		private System.Windows.Forms.Label labelStatusNote;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.Label labelPlanBenefitsNotVerifiedIn;
		private OpenDental.UI.ComboBox comboVerifyUser;
		private System.Windows.Forms.Label labelVerifyStatusSet;
		private OpenDental.UI.ComboBox comboSetVerifyStatus;
		private UI.Button butRefresh;
		private System.Windows.Forms.TextBox textPatBirthdate;
		private System.Windows.Forms.Label labelPatBirthdate;
		private UI.Button butVerifyPat;
		private OpenDental.UI.TabControl tabControlVerificationList;
		private OpenDental.UI.TabPage tabCurrent;
		private OpenDental.UI.TabPage tabPastDue;
		private UI.GridOD gridPastDue;
		private UI.ListBox listBoxVerifyClinics;
		private UI.ListBox listBoxVerifyRegions;
		private OpenDental.UI.TabPage tabAssignMedicaid;
		private UI.GroupBox groupAssignVerificationMedicaid;
		private System.Windows.Forms.TextBox textInsVerifyNoteMedicaid;
		private System.Windows.Forms.Label labelNoteMedicaid;
		private System.Windows.Forms.TextBox textAssignUserMedicaid;
		private System.Windows.Forms.Label labelToUserMedicaid;
		private UI.Button butAssignUserPickMedicaid;
		private UI.GridOD gridAssignMedicaid;
		private UI.Button butAssignUserMedicaid;
		private System.Windows.Forms.Label labelMedicaid;
		private System.Windows.Forms.Label labelStandard;
		private ValidNum textAppointmentScheduledDaysMedicaid;
		private ValidNum textPatientEnrollmentDaysMedicaid;
		private ValidNum textInsBenefitEligibilityDaysMedicaid;
	}
}