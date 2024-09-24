
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
			this.groupBoxHIPAA = new OpenDental.UI.GroupBox();
			this.checkFamPhiAccess = new OpenDental.UI.CheckBox();
			this.checkPatientSSNMasked = new OpenDental.UI.CheckBox();
			this.checkPatientDOBMasked = new OpenDental.UI.CheckBox();
			this.groupBoxPatientEdit = new OpenDental.UI.GroupBox();
			this.checkAddressVerifyWithUSPS = new OpenDental.UI.CheckBox();
			this.checkTextMsgOkStatusTreatAsNo = new OpenDental.UI.CheckBox();
			this.checkDoSendOptOutText = new OpenDental.UI.CheckBox();
			this.checkAutoFillPatEmail = new OpenDental.UI.CheckBox();
			this.checkPreferredPronouns = new OpenDental.UI.CheckBox();
			this.checkSameForFamily = new OpenDental.UI.CheckBox();
			this.checkPatientPhoneUsePhonenumberTable = new OpenDental.UI.CheckBox();
			this.butSyncPhNums = new OpenDental.UI.Button();
			this.checkPreferredReferrals = new OpenDental.UI.CheckBox();
			this.checkAllowPatsAtHQ = new OpenDental.UI.CheckBox();
			this.checkSelectProv = new OpenDental.UI.CheckBox();
			this.checkGoogleAddress = new OpenDental.UI.CheckBox();
			this.groupBoxClaimSnapshot = new OpenDental.UI.GroupBox();
			this.comboClaimSnapshotTriggerType = new OpenDental.UI.ComboBox();
			this.textClaimSnapshotRunTime = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.groupBoxSuperFamily = new OpenDental.UI.GroupBox();
			this.comboSuperFamSort = new OpenDental.UI.ComboBox();
			this.labelSuperFamSort = new System.Windows.Forms.Label();
			this.checkSuperFamSync = new OpenDental.UI.CheckBox();
			this.checkSuperFamAddIns = new OpenDental.UI.CheckBox();
			this.checkCloneCreateSuperFamily = new OpenDental.UI.CheckBox();
			this.groupBoxHIPAA.SuspendLayout();
			this.groupBoxPatientEdit.SuspendLayout();
			this.groupBoxClaimSnapshot.SuspendLayout();
			this.groupBoxSuperFamily.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxHIPAA
			// 
			this.groupBoxHIPAA.Controls.Add(this.checkFamPhiAccess);
			this.groupBoxHIPAA.Controls.Add(this.checkPatientSSNMasked);
			this.groupBoxHIPAA.Controls.Add(this.checkPatientDOBMasked);
			this.groupBoxHIPAA.Location = new System.Drawing.Point(20, 258);
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
			// 
			// groupBoxPatientEdit
			// 
			this.groupBoxPatientEdit.Controls.Add(this.checkAddressVerifyWithUSPS);
			this.groupBoxPatientEdit.Controls.Add(this.checkTextMsgOkStatusTreatAsNo);
			this.groupBoxPatientEdit.Controls.Add(this.checkDoSendOptOutText);
			this.groupBoxPatientEdit.Controls.Add(this.checkAutoFillPatEmail);
			this.groupBoxPatientEdit.Controls.Add(this.checkPreferredPronouns);
			this.groupBoxPatientEdit.Controls.Add(this.checkSameForFamily);
			this.groupBoxPatientEdit.Controls.Add(this.checkPatientPhoneUsePhonenumberTable);
			this.groupBoxPatientEdit.Controls.Add(this.butSyncPhNums);
			this.groupBoxPatientEdit.Controls.Add(this.checkPreferredReferrals);
			this.groupBoxPatientEdit.Controls.Add(this.checkAllowPatsAtHQ);
			this.groupBoxPatientEdit.Controls.Add(this.checkSelectProv);
			this.groupBoxPatientEdit.Controls.Add(this.checkGoogleAddress);
			this.groupBoxPatientEdit.Location = new System.Drawing.Point(20, 10);
			this.groupBoxPatientEdit.Name = "groupBoxPatientEdit";
			this.groupBoxPatientEdit.Size = new System.Drawing.Size(450, 245);
			this.groupBoxPatientEdit.TabIndex = 309;
			this.groupBoxPatientEdit.Text = "Patient Edit";
			// 
			// checkAddressVerifyWithUSPS
			// 
			this.checkAddressVerifyWithUSPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAddressVerifyWithUSPS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAddressVerifyWithUSPS.Location = new System.Drawing.Point(18, 217);
			this.checkAddressVerifyWithUSPS.Name = "checkAddressVerifyWithUSPS";
			this.checkAddressVerifyWithUSPS.Size = new System.Drawing.Size(422, 17);
			this.checkAddressVerifyWithUSPS.TabIndex = 309;
			this.checkAddressVerifyWithUSPS.Text = "Verify Addresses with USPS";
			// 
			// checkTextMsgOkStatusTreatAsNo
			// 
			this.checkTextMsgOkStatusTreatAsNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkTextMsgOkStatusTreatAsNo.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTextMsgOkStatusTreatAsNo.Location = new System.Drawing.Point(65, 10);
			this.checkTextMsgOkStatusTreatAsNo.Name = "checkTextMsgOkStatusTreatAsNo";
			this.checkTextMsgOkStatusTreatAsNo.Size = new System.Drawing.Size(375, 17);
			this.checkTextMsgOkStatusTreatAsNo.TabIndex = 296;
			this.checkTextMsgOkStatusTreatAsNo.Text = "Text Msg OK, assume default is \'No\' for patients with no selection";
			// 
			// checkDoSendOptOutText
			// 
			this.checkDoSendOptOutText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkDoSendOptOutText.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDoSendOptOutText.Location = new System.Drawing.Point(5, 30);
			this.checkDoSendOptOutText.Name = "checkDoSendOptOutText";
			this.checkDoSendOptOutText.Size = new System.Drawing.Size(435, 17);
			this.checkDoSendOptOutText.TabIndex = 310;
			this.checkDoSendOptOutText.Text = "Send opt-out msg when changing \'Text OK\' status from \'Yes\' to \'No\'";
			// 
			// checkAutoFillPatEmail
			// 
			this.checkAutoFillPatEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAutoFillPatEmail.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoFillPatEmail.Location = new System.Drawing.Point(15, 110);
			this.checkAutoFillPatEmail.Name = "checkAutoFillPatEmail";
			this.checkAutoFillPatEmail.Size = new System.Drawing.Size(425, 17);
			this.checkAutoFillPatEmail.TabIndex = 301;
			this.checkAutoFillPatEmail.Text = "Autofill patient\'s email with the guarantor\'s when adding many new patients";
			// 
			// checkPreferredPronouns
			// 
			this.checkPreferredPronouns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPreferredPronouns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreferredPronouns.Location = new System.Drawing.Point(18, 170);
			this.checkPreferredPronouns.Name = "checkPreferredPronouns";
			this.checkPreferredPronouns.Size = new System.Drawing.Size(422, 17);
			this.checkPreferredPronouns.TabIndex = 308;
			this.checkPreferredPronouns.Text = "Show Preferred Pronouns for Patients";
			// 
			// checkSameForFamily
			// 
			this.checkSameForFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSameForFamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSameForFamily.Location = new System.Drawing.Point(1, 150);
			this.checkSameForFamily.Name = "checkSameForFamily";
			this.checkSameForFamily.Size = new System.Drawing.Size(439, 17);
			this.checkSameForFamily.TabIndex = 305;
			this.checkSameForFamily.Text = "In Patient Edit window, checks for \"Same for Entire Family\" default to unchecked";
			// 
			// checkPatientPhoneUsePhonenumberTable
			// 
			this.checkPatientPhoneUsePhonenumberTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPatientPhoneUsePhonenumberTable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientPhoneUsePhonenumberTable.Location = new System.Drawing.Point(25, 193);
			this.checkPatientPhoneUsePhonenumberTable.Name = "checkPatientPhoneUsePhonenumberTable";
			this.checkPatientPhoneUsePhonenumberTable.Size = new System.Drawing.Size(360, 17);
			this.checkPatientPhoneUsePhonenumberTable.TabIndex = 306;
			this.checkPatientPhoneUsePhonenumberTable.Text = "Store patient phone numbers in a separate table for patient search";
			this.checkPatientPhoneUsePhonenumberTable.Click += new System.EventHandler(this.checkPatientPhoneUsePhoneNumberTable_Click);
			// 
			// butSyncPhNums
			// 
			this.butSyncPhNums.Location = new System.Drawing.Point(391, 190);
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
			this.checkPreferredReferrals.Location = new System.Drawing.Point(15, 90);
			this.checkPreferredReferrals.Name = "checkPreferredReferrals";
			this.checkPreferredReferrals.Size = new System.Drawing.Size(425, 17);
			this.checkPreferredReferrals.TabIndex = 300;
			this.checkPreferredReferrals.Text = "In Select Referral window, only show preferred referrals by default";
			// 
			// checkAllowPatsAtHQ
			// 
			this.checkAllowPatsAtHQ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowPatsAtHQ.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowPatsAtHQ.Location = new System.Drawing.Point(15, 130);
			this.checkAllowPatsAtHQ.Name = "checkAllowPatsAtHQ";
			this.checkAllowPatsAtHQ.Size = new System.Drawing.Size(425, 17);
			this.checkAllowPatsAtHQ.TabIndex = 302;
			this.checkAllowPatsAtHQ.Text = "Allow new patients to be added with an unassigned clinic";
			// 
			// checkSelectProv
			// 
			this.checkSelectProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSelectProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSelectProv.Location = new System.Drawing.Point(1, 70);
			this.checkSelectProv.Name = "checkSelectProv";
			this.checkSelectProv.Size = new System.Drawing.Size(439, 17);
			this.checkSelectProv.TabIndex = 295;
			this.checkSelectProv.Text = "Pri Prov defaults to \'Select Provider\' in Patient Edit and Add Family windows";
			// 
			// checkGoogleAddress
			// 
			this.checkGoogleAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkGoogleAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGoogleAddress.Location = new System.Drawing.Point(15, 50);
			this.checkGoogleAddress.Name = "checkGoogleAddress";
			this.checkGoogleAddress.Size = new System.Drawing.Size(425, 17);
			this.checkGoogleAddress.TabIndex = 299;
			this.checkGoogleAddress.Text = "Show Google Maps in Patient Edit window";
			// 
			// groupBoxClaimSnapshot
			// 
			this.groupBoxClaimSnapshot.BackColor = System.Drawing.Color.White;
			this.groupBoxClaimSnapshot.Controls.Add(this.comboClaimSnapshotTriggerType);
			this.groupBoxClaimSnapshot.Controls.Add(this.textClaimSnapshotRunTime);
			this.groupBoxClaimSnapshot.Controls.Add(this.label30);
			this.groupBoxClaimSnapshot.Controls.Add(this.label31);
			this.groupBoxClaimSnapshot.Location = new System.Drawing.Point(20, 442);
			this.groupBoxClaimSnapshot.Name = "groupBoxClaimSnapshot";
			this.groupBoxClaimSnapshot.Size = new System.Drawing.Size(450, 65);
			this.groupBoxClaimSnapshot.TabIndex = 284;
			this.groupBoxClaimSnapshot.Text = "Claim Snapshot";
			// 
			// comboClaimSnapshotTriggerType
			// 
			this.comboClaimSnapshotTriggerType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClaimSnapshotTriggerType.Location = new System.Drawing.Point(292, 10);
			this.comboClaimSnapshotTriggerType.Name = "comboClaimSnapshotTriggerType";
			this.comboClaimSnapshotTriggerType.Size = new System.Drawing.Size(148, 21);
			this.comboClaimSnapshotTriggerType.TabIndex = 221;
			this.comboClaimSnapshotTriggerType.SelectionChangeCommitted += new System.EventHandler(this.comboClaimSnapshotTriggerType_ChangeCommitted);
			// 
			// textClaimSnapshotRunTime
			// 
			this.textClaimSnapshotRunTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimSnapshotRunTime.Location = new System.Drawing.Point(330, 35);
			this.textClaimSnapshotRunTime.Name = "textClaimSnapshotRunTime";
			this.textClaimSnapshotRunTime.Size = new System.Drawing.Size(110, 20);
			this.textClaimSnapshotRunTime.TabIndex = 222;
			this.textClaimSnapshotRunTime.Validating += new System.ComponentModel.CancelEventHandler(this.textClaimSnapShotRunTime_Validating);
			// 
			// label30
			// 
			this.label30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label30.Location = new System.Drawing.Point(164, 38);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(165, 17);
			this.label30.TabIndex = 223;
			this.label30.Text = "Service Run Time";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label31
			// 
			this.label31.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label31.Location = new System.Drawing.Point(164, 13);
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
			this.groupBoxSuperFamily.Controls.Add(this.checkCloneCreateSuperFamily);
			this.groupBoxSuperFamily.Location = new System.Drawing.Point(20, 338);
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
			this.labelSuperFamSort.Location = new System.Drawing.Point(157, 13);
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
			this.checkSuperFamSync.Location = new System.Drawing.Point(15, 34);
			this.checkSuperFamSync.Name = "checkSuperFamSync";
			this.checkSuperFamSync.Size = new System.Drawing.Size(425, 17);
			this.checkSuperFamSync.TabIndex = 219;
			this.checkSuperFamSync.Text = "Allow syncing patient information to all super family members";
			// 
			// checkSuperFamAddIns
			// 
			this.checkSuperFamAddIns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSuperFamAddIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFamAddIns.Location = new System.Drawing.Point(2, 54);
			this.checkSuperFamAddIns.Name = "checkSuperFamAddIns";
			this.checkSuperFamAddIns.Size = new System.Drawing.Size(438, 17);
			this.checkSuperFamAddIns.TabIndex = 221;
			this.checkSuperFamAddIns.Text = "Copy super guarantor\'s primary insurance to all new super family members";
			// 
			// checkCloneCreateSuperFamily
			// 
			this.checkCloneCreateSuperFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkCloneCreateSuperFamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCloneCreateSuperFamily.Location = new System.Drawing.Point(15, 74);
			this.checkCloneCreateSuperFamily.Name = "checkCloneCreateSuperFamily";
			this.checkCloneCreateSuperFamily.Size = new System.Drawing.Size(425, 17);
			this.checkCloneCreateSuperFamily.TabIndex = 227;
			this.checkCloneCreateSuperFamily.Text = "New patient clones use super family instead of regular family";
			this.checkCloneCreateSuperFamily.Click += new System.EventHandler(this.checkCloneCreateSuperFamily_Click);
			// 
			// UserControlFamilyGeneral
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxHIPAA);
			this.Controls.Add(this.groupBoxPatientEdit);
			this.Controls.Add(this.groupBoxClaimSnapshot);
			this.Controls.Add(this.groupBoxSuperFamily);
			this.Name = "UserControlFamilyGeneral";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupBoxHIPAA.ResumeLayout(false);
			this.groupBoxPatientEdit.ResumeLayout(false);
			this.groupBoxClaimSnapshot.ResumeLayout(false);
			this.groupBoxClaimSnapshot.PerformLayout();
			this.groupBoxSuperFamily.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GroupBox groupBoxClaimSnapshot;
		private UI.ComboBox comboClaimSnapshotTriggerType;
		private System.Windows.Forms.TextBox textClaimSnapshotRunTime;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private UI.GroupBox groupBoxSuperFamily;
		private UI.ComboBox comboSuperFamSort;
		private System.Windows.Forms.Label labelSuperFamSort;
		private OpenDental.UI.CheckBox checkSuperFamSync;
		private OpenDental.UI.CheckBox checkSuperFamAddIns;
		private OpenDental.UI.CheckBox checkCloneCreateSuperFamily;
		private UI.Button butSyncPhNums;
		private OpenDental.UI.CheckBox checkPatientPhoneUsePhonenumberTable;
		private OpenDental.UI.CheckBox checkSameForFamily;
		private OpenDental.UI.CheckBox checkPatientDOBMasked;
		private OpenDental.UI.CheckBox checkPatientSSNMasked;
		private OpenDental.UI.CheckBox checkAllowPatsAtHQ;
		private OpenDental.UI.CheckBox checkAutoFillPatEmail;
		private OpenDental.UI.CheckBox checkPreferredReferrals;
		private OpenDental.UI.CheckBox checkTextMsgOkStatusTreatAsNo;
		private OpenDental.UI.CheckBox checkFamPhiAccess;
		private OpenDental.UI.CheckBox checkSelectProv;
		private OpenDental.UI.CheckBox checkGoogleAddress;
		private OpenDental.UI.CheckBox checkPreferredPronouns;
		private UI.GroupBox groupBoxPatientEdit;
		private UI.GroupBox groupBoxHIPAA;
		private UI.CheckBox checkAddressVerifyWithUSPS;
		private UI.CheckBox checkDoSendOptOutText;
	}
}
