
namespace OpenDental {
	partial class UserControlFamilyGeneral {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.labelUsePhoneNumTableDetails = new System.Windows.Forms.Label();
			this.labelSelectProvDetails = new System.Windows.Forms.Label();
			this.labelCobSendPaidByInsAtDetails = new System.Windows.Forms.Label();
			this.linkLabelCobRuleDetails = new System.Windows.Forms.LinkLabel();
			this.labelSuperFamSyncDetails = new System.Windows.Forms.Label();
			this.butSuperFamSortDetails = new OpenDental.UI.Button();
			this.groupBoxHIPAA = new OpenDental.UI.GroupBoxOD();
			this.checkFamPhiAccess = new System.Windows.Forms.CheckBox();
			this.checkPatientSSNMasked = new System.Windows.Forms.CheckBox();
			this.checkPatientDOBMasked = new System.Windows.Forms.CheckBox();
			this.groupBoxPatientEdit = new OpenDental.UI.GroupBoxOD();
			this.checkTextMsgOkStatusTreatAsNo = new System.Windows.Forms.CheckBox();
			this.checkAutoFillPatEmail = new System.Windows.Forms.CheckBox();
			this.checkPreferredPronouns = new System.Windows.Forms.CheckBox();
			this.checkSameForFamily = new System.Windows.Forms.CheckBox();
			this.checkUsePhoneNumTable = new System.Windows.Forms.CheckBox();
			this.butSyncPhNums = new OpenDental.UI.Button();
			this.checkPreferredReferrals = new System.Windows.Forms.CheckBox();
			this.checkAllowPatsAtHQ = new System.Windows.Forms.CheckBox();
			this.checkSelectProv = new System.Windows.Forms.CheckBox();
			this.checkGoogleAddress = new System.Windows.Forms.CheckBox();
			this.groupBoxCOB = new OpenDental.UI.GroupBoxOD();
			this.comboCobSendPaidByInsAt = new OpenDental.UI.ComboBoxOD();
			this.labelCobSendPaidByOtherInsAt = new System.Windows.Forms.Label();
			this.comboCobRule = new OpenDental.UI.ComboBoxOD();
			this.labelCobRule = new System.Windows.Forms.Label();
			this.groupBoxClaimSnapshot = new OpenDental.UI.GroupBoxOD();
			this.comboClaimSnapshotTrigger = new OpenDental.UI.ComboBoxOD();
			this.textClaimSnapshotRunTime = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.groupBoxSuperFamily = new OpenDental.UI.GroupBoxOD();
			this.comboSuperFamSort = new OpenDental.UI.ComboBoxOD();
			this.labelSuperFamSort = new System.Windows.Forms.Label();
			this.checkSuperFamSync = new System.Windows.Forms.CheckBox();
			this.checkSuperFamAddIns = new System.Windows.Forms.CheckBox();
			this.checkSuperFamCloneCreate = new System.Windows.Forms.CheckBox();
			this.groupBoxHIPAA.SuspendLayout();
			this.groupBoxPatientEdit.SuspendLayout();
			this.groupBoxCOB.SuspendLayout();
			this.groupBoxClaimSnapshot.SuspendLayout();
			this.groupBoxSuperFamily.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelUsePhoneNumTableDetails
			// 
			this.labelUsePhoneNumTableDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUsePhoneNumTableDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelUsePhoneNumTableDetails.Location = new System.Drawing.Point(476, 256);
			this.labelUsePhoneNumTableDetails.Name = "labelUsePhoneNumTableDetails";
			this.labelUsePhoneNumTableDetails.Size = new System.Drawing.Size(498, 17);
			this.labelUsePhoneNumTableDetails.TabIndex = 355;
			this.labelUsePhoneNumTableDetails.Text = "strips out non-digit characters, useful to speed up the search in large databases" +
    "";
			this.labelUsePhoneNumTableDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelSelectProvDetails
			// 
			this.labelSelectProvDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSelectProvDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelSelectProvDetails.Location = new System.Drawing.Point(476, 133);
			this.labelSelectProvDetails.Name = "labelSelectProvDetails";
			this.labelSelectProvDetails.Size = new System.Drawing.Size(498, 17);
			this.labelSelectProvDetails.TabIndex = 356;
			this.labelSelectProvDetails.Text = "require user to select a provider";
			this.labelSelectProvDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCobSendPaidByInsAtDetails
			// 
			this.labelCobSendPaidByInsAtDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCobSendPaidByInsAtDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelCobSendPaidByInsAtDetails.Location = new System.Drawing.Point(476, 52);
			this.labelCobSendPaidByInsAtDetails.Name = "labelCobSendPaidByInsAtDetails";
			this.labelCobSendPaidByInsAtDetails.Size = new System.Drawing.Size(498, 17);
			this.labelCobSendPaidByInsAtDetails.TabIndex = 357;
			this.labelCobSendPaidByInsAtDetails.Text = "Claim Level means just the total for the claim, Procedure Level means the amount " +
    "for each procedure";
			this.labelCobSendPaidByInsAtDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// linkLabelCobRuleDetails
			// 
			this.linkLabelCobRuleDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelCobRuleDetails.LinkArea = new System.Windows.Forms.LinkArea(4, 24);
			this.linkLabelCobRuleDetails.LinkColor = System.Drawing.Color.MidnightBlue;
			this.linkLabelCobRuleDetails.Location = new System.Drawing.Point(476, 28);
			this.linkLabelCobRuleDetails.Name = "linkLabelCobRuleDetails";
			this.linkLabelCobRuleDetails.Size = new System.Drawing.Size(498, 17);
			this.linkLabelCobRuleDetails.TabIndex = 369;
			this.linkLabelCobRuleDetails.TabStop = true;
			this.linkLabelCobRuleDetails.Text = "see Coordination of Benefits";
			this.linkLabelCobRuleDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkLabelCobRuleDetails.UseCompatibleTextRendering = true;
			this.linkLabelCobRuleDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCobRuleDetails_LinkClicked);
			// 
			// labelSuperFamSyncDetails
			// 
			this.labelSuperFamSyncDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSuperFamSyncDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelSuperFamSyncDetails.Location = new System.Drawing.Point(476, 401);
			this.labelSuperFamSyncDetails.Name = "labelSuperFamSyncDetails";
			this.labelSuperFamSyncDetails.Size = new System.Drawing.Size(498, 17);
			this.labelSuperFamSyncDetails.TabIndex = 370;
			this.labelSuperFamSyncDetails.Text = "show \"Same for entire super family\" checkbox in Edit Patient Information window";
			this.labelSuperFamSyncDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSuperFamSortDetails
			// 
			this.butSuperFamSortDetails.ForeColor = System.Drawing.Color.Black;
			this.butSuperFamSortDetails.Location = new System.Drawing.Point(476, 378);
			this.butSuperFamSortDetails.Name = "butSuperFamSortDetails";
			this.butSuperFamSortDetails.Size = new System.Drawing.Size(64, 21);
			this.butSuperFamSortDetails.TabIndex = 371;
			this.butSuperFamSortDetails.Text = "Details";
			this.butSuperFamSortDetails.Click += new System.EventHandler(this.butSuperFamSortDetails_Click);
			// 
			// groupBoxHIPAA
			// 
			this.groupBoxHIPAA.Controls.Add(this.checkFamPhiAccess);
			this.groupBoxHIPAA.Controls.Add(this.checkPatientSSNMasked);
			this.groupBoxHIPAA.Controls.Add(this.checkPatientDOBMasked);
			this.groupBoxHIPAA.Location = new System.Drawing.Point(20, 288);
			this.groupBoxHIPAA.Name = "groupBoxHIPAA";
			this.groupBoxHIPAA.Size = new System.Drawing.Size(450, 77);
			this.groupBoxHIPAA.TabIndex = 310;
			this.groupBoxHIPAA.Text = "HIPAA";
			// 
			// checkFamPhiAccess
			// 
			this.checkFamPhiAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkFamPhiAccess.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFamPhiAccess.Location = new System.Drawing.Point(50, 10);
			this.checkFamPhiAccess.Name = "checkFamPhiAccess";
			this.checkFamPhiAccess.Size = new System.Drawing.Size(390, 17);
			this.checkFamPhiAccess.TabIndex = 298;
			this.checkFamPhiAccess.Text = "Allow guarantor access to family health information in the Patient Portal";
			this.checkFamPhiAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatientSSNMasked
			// 
			this.checkPatientSSNMasked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPatientSSNMasked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSSNMasked.Location = new System.Drawing.Point(15, 30);
			this.checkPatientSSNMasked.Name = "checkPatientSSNMasked";
			this.checkPatientSSNMasked.Size = new System.Drawing.Size(425, 17);
			this.checkPatientSSNMasked.TabIndex = 303;
			this.checkPatientSSNMasked.Text = "Mask patient Social Security Numbers";
			this.checkPatientSSNMasked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatientDOBMasked
			// 
			this.checkPatientDOBMasked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPatientDOBMasked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientDOBMasked.Location = new System.Drawing.Point(15, 50);
			this.checkPatientDOBMasked.Name = "checkPatientDOBMasked";
			this.checkPatientDOBMasked.Size = new System.Drawing.Size(425, 17);
			this.checkPatientDOBMasked.TabIndex = 304;
			this.checkPatientDOBMasked.Text = "Mask patient date of birth";
			this.checkPatientDOBMasked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxPatientEdit
			// 
			this.groupBoxPatientEdit.Controls.Add(this.checkTextMsgOkStatusTreatAsNo);
			this.groupBoxPatientEdit.Controls.Add(this.checkAutoFillPatEmail);
			this.groupBoxPatientEdit.Controls.Add(this.checkPreferredPronouns);
			this.groupBoxPatientEdit.Controls.Add(this.checkSameForFamily);
			this.groupBoxPatientEdit.Controls.Add(this.checkUsePhoneNumTable);
			this.groupBoxPatientEdit.Controls.Add(this.butSyncPhNums);
			this.groupBoxPatientEdit.Controls.Add(this.checkPreferredReferrals);
			this.groupBoxPatientEdit.Controls.Add(this.checkAllowPatsAtHQ);
			this.groupBoxPatientEdit.Controls.Add(this.checkSelectProv);
			this.groupBoxPatientEdit.Controls.Add(this.checkGoogleAddress);
			this.groupBoxPatientEdit.Location = new System.Drawing.Point(20, 84);
			this.groupBoxPatientEdit.Name = "groupBoxPatientEdit";
			this.groupBoxPatientEdit.Size = new System.Drawing.Size(450, 201);
			this.groupBoxPatientEdit.TabIndex = 309;
			this.groupBoxPatientEdit.Text = "Patient Edit";
			// 
			// checkTextMsgOkStatusTreatAsNo
			// 
			this.checkTextMsgOkStatusTreatAsNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTextMsgOkStatusTreatAsNo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTextMsgOkStatusTreatAsNo.Location = new System.Drawing.Point(90, 10);
			this.checkTextMsgOkStatusTreatAsNo.Name = "checkTextMsgOkStatusTreatAsNo";
			this.checkTextMsgOkStatusTreatAsNo.Size = new System.Drawing.Size(350, 17);
			this.checkTextMsgOkStatusTreatAsNo.TabIndex = 296;
			this.checkTextMsgOkStatusTreatAsNo.Text = "Text Msg OK status, treat ?? as No instead of Yes";
			this.checkTextMsgOkStatusTreatAsNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAutoFillPatEmail
			// 
			this.checkAutoFillPatEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAutoFillPatEmail.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoFillPatEmail.Location = new System.Drawing.Point(15, 90);
			this.checkAutoFillPatEmail.Name = "checkAutoFillPatEmail";
			this.checkAutoFillPatEmail.Size = new System.Drawing.Size(425, 17);
			this.checkAutoFillPatEmail.TabIndex = 301;
			this.checkAutoFillPatEmail.Text = "Autofill patient\'s email with the guarantor\'s when adding many new patients";
			this.checkAutoFillPatEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPreferredPronouns
			// 
			this.checkPreferredPronouns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPreferredPronouns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreferredPronouns.Location = new System.Drawing.Point(18, 150);
			this.checkPreferredPronouns.Name = "checkPreferredPronouns";
			this.checkPreferredPronouns.Size = new System.Drawing.Size(422, 17);
			this.checkPreferredPronouns.TabIndex = 308;
			this.checkPreferredPronouns.Text = "Show Preferred Pronouns for Patients";
			this.checkPreferredPronouns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSameForFamily
			// 
			this.checkSameForFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSameForFamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSameForFamily.Location = new System.Drawing.Point(5, 130);
			this.checkSameForFamily.Name = "checkSameForFamily";
			this.checkSameForFamily.Size = new System.Drawing.Size(435, 17);
			this.checkSameForFamily.TabIndex = 305;
			this.checkSameForFamily.Text = "In Patient Edit window, checkboxes for \"Same for Entire Family\" default to unchec" +
    "ked";
			this.checkSameForFamily.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUsePhoneNumTable
			// 
			this.checkUsePhoneNumTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUsePhoneNumTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePhoneNumTable.Checked = true;
			this.checkUsePhoneNumTable.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkUsePhoneNumTable.Location = new System.Drawing.Point(25, 173);
			this.checkUsePhoneNumTable.Name = "checkUsePhoneNumTable";
			this.checkUsePhoneNumTable.Size = new System.Drawing.Size(360, 17);
			this.checkUsePhoneNumTable.TabIndex = 306;
			this.checkUsePhoneNumTable.Text = "Store patient phone numbers in a separate table for patient search";
			this.checkUsePhoneNumTable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSyncPhNums
			// 
			this.butSyncPhNums.Location = new System.Drawing.Point(391, 170);
			this.butSyncPhNums.Name = "butSyncPhNums";
			this.butSyncPhNums.Size = new System.Drawing.Size(49, 21);
			this.butSyncPhNums.TabIndex = 307;
			this.butSyncPhNums.Text = "Sync";
			this.butSyncPhNums.Click += new System.EventHandler(this.butSyncPhNums_Click);
			// 
			// checkPreferredReferrals
			// 
			this.checkPreferredReferrals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPreferredReferrals.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreferredReferrals.Location = new System.Drawing.Point(15, 70);
			this.checkPreferredReferrals.Name = "checkPreferredReferrals";
			this.checkPreferredReferrals.Size = new System.Drawing.Size(425, 17);
			this.checkPreferredReferrals.TabIndex = 300;
			this.checkPreferredReferrals.Text = "Show preferred referrals only in the Select Referral window by default";
			this.checkPreferredReferrals.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllowPatsAtHQ
			// 
			this.checkAllowPatsAtHQ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowPatsAtHQ.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPatsAtHQ.Location = new System.Drawing.Point(15, 110);
			this.checkAllowPatsAtHQ.Name = "checkAllowPatsAtHQ";
			this.checkAllowPatsAtHQ.Size = new System.Drawing.Size(425, 17);
			this.checkAllowPatsAtHQ.TabIndex = 302;
			this.checkAllowPatsAtHQ.Text = "Allow new patients to be added with an unassigned clinic";
			this.checkAllowPatsAtHQ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSelectProv
			// 
			this.checkSelectProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSelectProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSelectProv.Location = new System.Drawing.Point(15, 50);
			this.checkSelectProv.Name = "checkSelectProv";
			this.checkSelectProv.Size = new System.Drawing.Size(425, 17);
			this.checkSelectProv.TabIndex = 295;
			this.checkSelectProv.Text = "Primary Provider defaults to \'Select Provider\' in Patient Edit and Add Family win" +
    "dows";
			this.checkSelectProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGoogleAddress
			// 
			this.checkGoogleAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkGoogleAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGoogleAddress.Location = new System.Drawing.Point(15, 30);
			this.checkGoogleAddress.Name = "checkGoogleAddress";
			this.checkGoogleAddress.Size = new System.Drawing.Size(425, 17);
			this.checkGoogleAddress.TabIndex = 299;
			this.checkGoogleAddress.Text = "Show Google Maps in Patient Edit window";
			this.checkGoogleAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxCOB
			// 
			this.groupBoxCOB.BackColor = System.Drawing.Color.White;
			this.groupBoxCOB.Controls.Add(this.comboCobSendPaidByInsAt);
			this.groupBoxCOB.Controls.Add(this.labelCobSendPaidByOtherInsAt);
			this.groupBoxCOB.Controls.Add(this.comboCobRule);
			this.groupBoxCOB.Controls.Add(this.labelCobRule);
			this.groupBoxCOB.Location = new System.Drawing.Point(20, 10);
			this.groupBoxCOB.Name = "groupBoxCOB";
			this.groupBoxCOB.Size = new System.Drawing.Size(450, 71);
			this.groupBoxCOB.TabIndex = 285;
			this.groupBoxCOB.Text = "Coordination of Benefits (COB)";
			// 
			// comboCobSendPaidByInsAt
			// 
			this.comboCobSendPaidByInsAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboCobSendPaidByInsAt.Location = new System.Drawing.Point(312, 39);
			this.comboCobSendPaidByInsAt.Name = "comboCobSendPaidByInsAt";
			this.comboCobSendPaidByInsAt.Size = new System.Drawing.Size(128, 21);
			this.comboCobSendPaidByInsAt.TabIndex = 287;
			// 
			// labelCobSendPaidByOtherInsAt
			// 
			this.labelCobSendPaidByOtherInsAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCobSendPaidByOtherInsAt.Location = new System.Drawing.Point(67, 42);
			this.labelCobSendPaidByOtherInsAt.Name = "labelCobSendPaidByOtherInsAt";
			this.labelCobSendPaidByOtherInsAt.Size = new System.Drawing.Size(242, 17);
			this.labelCobSendPaidByOtherInsAt.TabIndex = 286;
			this.labelCobSendPaidByOtherInsAt.Text = "Send Paid By Other Insurance At";
			this.labelCobSendPaidByOtherInsAt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCobRule
			// 
			this.comboCobRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboCobRule.Location = new System.Drawing.Point(312, 15);
			this.comboCobRule.Name = "comboCobRule";
			this.comboCobRule.Size = new System.Drawing.Size(128, 21);
			this.comboCobRule.TabIndex = 262;
			this.comboCobRule.SelectionChangeCommitted += new System.EventHandler(this.comboCobRule_SelectionChangeCommitted);
			// 
			// labelCobRule
			// 
			this.labelCobRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCobRule.Location = new System.Drawing.Point(67, 18);
			this.labelCobRule.Name = "labelCobRule";
			this.labelCobRule.Size = new System.Drawing.Size(242, 17);
			this.labelCobRule.TabIndex = 264;
			this.labelCobRule.Text = "Coordination of Benefits (COB) rule";
			this.labelCobRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimSnapshot
			// 
			this.groupBoxClaimSnapshot.BackColor = System.Drawing.Color.White;
			this.groupBoxClaimSnapshot.Controls.Add(this.comboClaimSnapshotTrigger);
			this.groupBoxClaimSnapshot.Controls.Add(this.textClaimSnapshotRunTime);
			this.groupBoxClaimSnapshot.Controls.Add(this.label30);
			this.groupBoxClaimSnapshot.Controls.Add(this.label31);
			this.groupBoxClaimSnapshot.Location = new System.Drawing.Point(20, 472);
			this.groupBoxClaimSnapshot.Name = "groupBoxClaimSnapshot";
			this.groupBoxClaimSnapshot.Size = new System.Drawing.Size(450, 65);
			this.groupBoxClaimSnapshot.TabIndex = 284;
			this.groupBoxClaimSnapshot.Text = "Claim Snapshot";
			// 
			// comboClaimSnapshotTrigger
			// 
			this.comboClaimSnapshotTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClaimSnapshotTrigger.Location = new System.Drawing.Point(292, 10);
			this.comboClaimSnapshotTrigger.Name = "comboClaimSnapshotTrigger";
			this.comboClaimSnapshotTrigger.Size = new System.Drawing.Size(148, 21);
			this.comboClaimSnapshotTrigger.TabIndex = 221;
			// 
			// textClaimSnapshotRunTime
			// 
			this.textClaimSnapshotRunTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimSnapshotRunTime.Location = new System.Drawing.Point(330, 35);
			this.textClaimSnapshotRunTime.Name = "textClaimSnapshotRunTime";
			this.textClaimSnapshotRunTime.Size = new System.Drawing.Size(110, 20);
			this.textClaimSnapshotRunTime.TabIndex = 222;
			// 
			// label30
			// 
			this.label30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label30.Location = new System.Drawing.Point(162, 38);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(165, 17);
			this.label30.TabIndex = 223;
			this.label30.Text = "Service Run Time";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label31
			// 
			this.label31.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label31.Location = new System.Drawing.Point(162, 13);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(127, 17);
			this.label31.TabIndex = 224;
			this.label31.Text = "Claim Snapshot Trigger";
			this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxSuperFamily
			// 
			this.groupBoxSuperFamily.BackColor = System.Drawing.Color.White;
			this.groupBoxSuperFamily.Controls.Add(this.comboSuperFamSort);
			this.groupBoxSuperFamily.Controls.Add(this.labelSuperFamSort);
			this.groupBoxSuperFamily.Controls.Add(this.checkSuperFamSync);
			this.groupBoxSuperFamily.Controls.Add(this.checkSuperFamAddIns);
			this.groupBoxSuperFamily.Controls.Add(this.checkSuperFamCloneCreate);
			this.groupBoxSuperFamily.Location = new System.Drawing.Point(20, 368);
			this.groupBoxSuperFamily.Name = "groupBoxSuperFamily";
			this.groupBoxSuperFamily.Size = new System.Drawing.Size(450, 101);
			this.groupBoxSuperFamily.TabIndex = 283;
			this.groupBoxSuperFamily.Text = "Super Family";
			// 
			// comboSuperFamSort
			// 
			this.comboSuperFamSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSuperFamSort.Location = new System.Drawing.Point(312, 10);
			this.comboSuperFamSort.Name = "comboSuperFamSort";
			this.comboSuperFamSort.Size = new System.Drawing.Size(128, 21);
			this.comboSuperFamSort.TabIndex = 217;
			// 
			// labelSuperFamSort
			// 
			this.labelSuperFamSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSuperFamSort.Location = new System.Drawing.Point(155, 13);
			this.labelSuperFamSort.Name = "labelSuperFamSort";
			this.labelSuperFamSort.Size = new System.Drawing.Size(154, 17);
			this.labelSuperFamSort.TabIndex = 218;
			this.labelSuperFamSort.Text = "Super family sorting strategy";
			this.labelSuperFamSort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFamSync
			// 
			this.checkSuperFamSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamSync.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamSync.Location = new System.Drawing.Point(125, 34);
			this.checkSuperFamSync.Name = "checkSuperFamSync";
			this.checkSuperFamSync.Size = new System.Drawing.Size(315, 17);
			this.checkSuperFamSync.TabIndex = 219;
			this.checkSuperFamSync.Text = "Allow syncing patient information to all super family members";
			this.checkSuperFamSync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFamAddIns
			// 
			this.checkSuperFamAddIns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamAddIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamAddIns.Location = new System.Drawing.Point(69, 54);
			this.checkSuperFamAddIns.Name = "checkSuperFamAddIns";
			this.checkSuperFamAddIns.Size = new System.Drawing.Size(371, 17);
			this.checkSuperFamAddIns.TabIndex = 221;
			this.checkSuperFamAddIns.Text = "Copy super guarantor\'s primary insurance to all new super family members";
			this.checkSuperFamAddIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSuperFamCloneCreate
			// 
			this.checkSuperFamCloneCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamCloneCreate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamCloneCreate.Location = new System.Drawing.Point(125, 74);
			this.checkSuperFamCloneCreate.Name = "checkSuperFamCloneCreate";
			this.checkSuperFamCloneCreate.Size = new System.Drawing.Size(315, 17);
			this.checkSuperFamCloneCreate.TabIndex = 227;
			this.checkSuperFamCloneCreate.Text = "New patient clones use super family instead of regular family";
			this.checkSuperFamCloneCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// UserControlFamilyGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.butSuperFamSortDetails);
			this.Controls.Add(this.labelSuperFamSyncDetails);
			this.Controls.Add(this.linkLabelCobRuleDetails);
			this.Controls.Add(this.labelCobSendPaidByInsAtDetails);
			this.Controls.Add(this.labelSelectProvDetails);
			this.Controls.Add(this.labelUsePhoneNumTableDetails);
			this.Controls.Add(this.groupBoxHIPAA);
			this.Controls.Add(this.groupBoxPatientEdit);
			this.Controls.Add(this.groupBoxCOB);
			this.Controls.Add(this.groupBoxClaimSnapshot);
			this.Controls.Add(this.groupBoxSuperFamily);
			this.Name = "UserControlFamilyGeneral";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxHIPAA.ResumeLayout(false);
			this.groupBoxPatientEdit.ResumeLayout(false);
			this.groupBoxCOB.ResumeLayout(false);
			this.groupBoxClaimSnapshot.ResumeLayout(false);
			this.groupBoxClaimSnapshot.PerformLayout();
			this.groupBoxSuperFamily.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBoxOD groupBoxCOB;
		private UI.ComboBoxOD comboCobSendPaidByInsAt;
		private System.Windows.Forms.Label labelCobSendPaidByOtherInsAt;
		private UI.ComboBoxOD comboCobRule;
		private System.Windows.Forms.Label labelCobRule;
		private UI.GroupBoxOD groupBoxClaimSnapshot;
		private UI.ComboBoxOD comboClaimSnapshotTrigger;
		private System.Windows.Forms.TextBox textClaimSnapshotRunTime;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private UI.GroupBoxOD groupBoxSuperFamily;
		private UI.ComboBoxOD comboSuperFamSort;
		private System.Windows.Forms.Label labelSuperFamSort;
		private System.Windows.Forms.CheckBox checkSuperFamSync;
		private System.Windows.Forms.CheckBox checkSuperFamAddIns;
		private System.Windows.Forms.CheckBox checkSuperFamCloneCreate;
		private UI.Button butSyncPhNums;
		private System.Windows.Forms.CheckBox checkUsePhoneNumTable;
		private System.Windows.Forms.CheckBox checkSameForFamily;
		private System.Windows.Forms.CheckBox checkPatientDOBMasked;
		private System.Windows.Forms.CheckBox checkPatientSSNMasked;
		private System.Windows.Forms.CheckBox checkAllowPatsAtHQ;
		private System.Windows.Forms.CheckBox checkAutoFillPatEmail;
		private System.Windows.Forms.CheckBox checkPreferredReferrals;
		private System.Windows.Forms.CheckBox checkTextMsgOkStatusTreatAsNo;
		private System.Windows.Forms.CheckBox checkFamPhiAccess;
		private System.Windows.Forms.CheckBox checkSelectProv;
		private System.Windows.Forms.CheckBox checkGoogleAddress;
		private System.Windows.Forms.CheckBox checkPreferredPronouns;
		private UI.GroupBoxOD groupBoxPatientEdit;
		private UI.GroupBoxOD groupBoxHIPAA;
		private System.Windows.Forms.Label labelUsePhoneNumTableDetails;
		private System.Windows.Forms.Label labelSelectProvDetails;
		private System.Windows.Forms.Label labelCobSendPaidByInsAtDetails;
		private System.Windows.Forms.LinkLabel linkLabelCobRuleDetails;
		private System.Windows.Forms.Label labelSuperFamSyncDetails;
		private UI.Button butSuperFamSortDetails;
	}
}
