
namespace OpenDental {
	partial class UserControlAccountClaimSend {
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
			this.textInsWriteoffDescript = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupBoxClaimsMedical = new OpenDental.UI.GroupBox();
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical = new OpenDental.UI.CheckBox();
			this.checkEclaimsMedicalProvTreatmentAsOrdering = new OpenDental.UI.CheckBox();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.textClaimAttachPath = new System.Windows.Forms.TextBox();
			this.comboZeroDollarProcClaimBehavior = new OpenDental.UI.ComboBox();
			this.checkEclaimsSubscIDUsesPatID = new OpenDental.UI.CheckBox();
			this.checkPpoUseUcr = new OpenDental.UI.CheckBox();
			this.checkClaimFormTreatDentSaysSigOnFile = new OpenDental.UI.CheckBox();
			this.labelClaimAttachmentExportPath = new System.Windows.Forms.Label();
			this.checkEclaimsSeparateTreatProv = new OpenDental.UI.CheckBox();
			this.labelCreatingClaimsZeroProcedures = new System.Windows.Forms.Label();
			this.checkPriClaimAllowSetToHoldUntilPriReceived = new OpenDental.UI.CheckBox();
			this.groupBoxClaimIdPrefix = new OpenDental.UI.GroupBox();
			this.butReplacements = new OpenDental.UI.Button();
			this.textClaimIdPrefix = new System.Windows.Forms.TextBox();
			this.checkClaimsValidateACN = new OpenDental.UI.CheckBox();
			this.checkCanadianPpoLabEst = new OpenDental.UI.CheckBox();
			this.groupBoxClaimsMedical.SuspendLayout();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxClaimIdPrefix.SuspendLayout();
			this.SuspendLayout();
			// 
			// textInsWriteoffDescript
			// 
			this.textInsWriteoffDescript.Location = new System.Drawing.Point(330, 301);
			this.textInsWriteoffDescript.Name = "textInsWriteoffDescript";
			this.textInsWriteoffDescript.Size = new System.Drawing.Size(130, 20);
			this.textInsWriteoffDescript.TabIndex = 4;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(75, 303);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(254, 17);
			this.label17.TabIndex = 284;
			this.label17.Text = "PPO write-off description (blank for \"Write-off\")";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxClaimsMedical
			// 
			this.groupBoxClaimsMedical.Controls.Add(this.checkClaimMedTypeIsInstWhenInsPlanIsMedical);
			this.groupBoxClaimsMedical.Controls.Add(this.checkEclaimsMedicalProvTreatmentAsOrdering);
			this.groupBoxClaimsMedical.Location = new System.Drawing.Point(20, 243);
			this.groupBoxClaimsMedical.Name = "groupBoxClaimsMedical";
			this.groupBoxClaimsMedical.Size = new System.Drawing.Size(450, 51);
			this.groupBoxClaimsMedical.TabIndex = 3;
			this.groupBoxClaimsMedical.Text = "Claims Medical";
			// 
			// checkClaimMedTypeIsInstWhenInsPlanIsMedical
			// 
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Location = new System.Drawing.Point(85, 10);
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Name = "checkClaimMedTypeIsInstWhenInsPlanIsMedical";
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Size = new System.Drawing.Size(355, 17);
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.TabIndex = 0;
			this.checkClaimMedTypeIsInstWhenInsPlanIsMedical.Text = "Set medical claims to institutional when using medical insurance";
			// 
			// checkEclaimsMedicalProvTreatmentAsOrdering
			// 
			this.checkEclaimsMedicalProvTreatmentAsOrdering.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Location = new System.Drawing.Point(15, 29);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Name = "checkEclaimsMedicalProvTreatmentAsOrdering";
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Size = new System.Drawing.Size(425, 17);
			this.checkEclaimsMedicalProvTreatmentAsOrdering.TabIndex = 1;
			this.checkEclaimsMedicalProvTreatmentAsOrdering.Text = "On medical e-claims, send treating provider as ordering provider by default";
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.Controls.Add(this.textClaimAttachPath);
			this.groupBoxOD1.Controls.Add(this.comboZeroDollarProcClaimBehavior);
			this.groupBoxOD1.Controls.Add(this.checkEclaimsSubscIDUsesPatID);
			this.groupBoxOD1.Controls.Add(this.checkPpoUseUcr);
			this.groupBoxOD1.Controls.Add(this.checkClaimFormTreatDentSaysSigOnFile);
			this.groupBoxOD1.Controls.Add(this.labelClaimAttachmentExportPath);
			this.groupBoxOD1.Controls.Add(this.checkEclaimsSeparateTreatProv);
			this.groupBoxOD1.Controls.Add(this.labelCreatingClaimsZeroProcedures);
			this.groupBoxOD1.Controls.Add(this.checkPriClaimAllowSetToHoldUntilPriReceived);
			this.groupBoxOD1.Controls.Add(this.groupBoxClaimIdPrefix);
			this.groupBoxOD1.Controls.Add(this.checkClaimsValidateACN);
			this.groupBoxOD1.Location = new System.Drawing.Point(20, 10);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(450, 227);
			this.groupBoxOD1.TabIndex = 0;
			this.groupBoxOD1.Text = "Claims Send";
			// 
			// textClaimAttachPath
			// 
			this.textClaimAttachPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimAttachPath.Location = new System.Drawing.Point(243, 29);
			this.textClaimAttachPath.Name = "textClaimAttachPath";
			this.textClaimAttachPath.Size = new System.Drawing.Size(197, 20);
			this.textClaimAttachPath.TabIndex = 1;
			// 
			// comboZeroDollarProcClaimBehavior
			// 
			this.comboZeroDollarProcClaimBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboZeroDollarProcClaimBehavior.Location = new System.Drawing.Point(272, 89);
			this.comboZeroDollarProcClaimBehavior.Name = "comboZeroDollarProcClaimBehavior";
			this.comboZeroDollarProcClaimBehavior.Size = new System.Drawing.Size(168, 21);
			this.comboZeroDollarProcClaimBehavior.TabIndex = 3;
			// 
			// checkEclaimsSubscIDUsesPatID
			// 
			this.checkEclaimsSubscIDUsesPatID.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsSubscIDUsesPatID.Location = new System.Drawing.Point(50, 70);
			this.checkEclaimsSubscIDUsesPatID.Name = "checkEclaimsSubscIDUsesPatID";
			this.checkEclaimsSubscIDUsesPatID.Size = new System.Drawing.Size(390, 17);
			this.checkEclaimsSubscIDUsesPatID.TabIndex = 293;
			this.checkEclaimsSubscIDUsesPatID.Text = "On e-claims, use Optional Patient ID instead of Subscriber ID";
			// 
			// checkPpoUseUcr
			// 
			this.checkPpoUseUcr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPpoUseUcr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPpoUseUcr.Location = new System.Drawing.Point(89, 10);
			this.checkPpoUseUcr.Name = "checkPpoUseUcr";
			this.checkPpoUseUcr.Size = new System.Drawing.Size(351, 17);
			this.checkPpoUseUcr.TabIndex = 0;
			this.checkPpoUseUcr.Text = "Use UCR fee for billed fee even if PPO fee is higher";
			// 
			// checkClaimFormTreatDentSaysSigOnFile
			// 
			this.checkClaimFormTreatDentSaysSigOnFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimFormTreatDentSaysSigOnFile.Location = new System.Drawing.Point(40, 112);
			this.checkClaimFormTreatDentSaysSigOnFile.Name = "checkClaimFormTreatDentSaysSigOnFile";
			this.checkClaimFormTreatDentSaysSigOnFile.Size = new System.Drawing.Size(400, 17);
			this.checkClaimFormTreatDentSaysSigOnFile.TabIndex = 4;
			this.checkClaimFormTreatDentSaysSigOnFile.Text = "Claim form treating provider shows \'Signature On File\' rather than name";
			// 
			// labelClaimAttachmentExportPath
			// 
			this.labelClaimAttachmentExportPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimAttachmentExportPath.Location = new System.Drawing.Point(58, 32);
			this.labelClaimAttachmentExportPath.Name = "labelClaimAttachmentExportPath";
			this.labelClaimAttachmentExportPath.Size = new System.Drawing.Size(184, 17);
			this.labelClaimAttachmentExportPath.TabIndex = 279;
			this.labelClaimAttachmentExportPath.Text = "Claim attachment export path";
			this.labelClaimAttachmentExportPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEclaimsSeparateTreatProv
			// 
			this.checkEclaimsSeparateTreatProv.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEclaimsSeparateTreatProv.Location = new System.Drawing.Point(50, 51);
			this.checkEclaimsSeparateTreatProv.Name = "checkEclaimsSeparateTreatProv";
			this.checkEclaimsSeparateTreatProv.Size = new System.Drawing.Size(390, 17);
			this.checkEclaimsSeparateTreatProv.TabIndex = 2;
			this.checkEclaimsSeparateTreatProv.Text = "On e-claims, send treating provider info for each separate procedure";
			// 
			// labelCreatingClaimsZeroProcedures
			// 
			this.labelCreatingClaimsZeroProcedures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCreatingClaimsZeroProcedures.Location = new System.Drawing.Point(58, 91);
			this.labelCreatingClaimsZeroProcedures.Name = "labelCreatingClaimsZeroProcedures";
			this.labelCreatingClaimsZeroProcedures.Size = new System.Drawing.Size(213, 17);
			this.labelCreatingClaimsZeroProcedures.TabIndex = 292;
			this.labelCreatingClaimsZeroProcedures.Text = "Creating claims with $0 procedures";
			this.labelCreatingClaimsZeroProcedures.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPriClaimAllowSetToHoldUntilPriReceived
			// 
			this.checkPriClaimAllowSetToHoldUntilPriReceived.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Location = new System.Drawing.Point(73, 150);
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Name = "checkPriClaimAllowSetToHoldUntilPriReceived";
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Size = new System.Drawing.Size(367, 17);
			this.checkPriClaimAllowSetToHoldUntilPriReceived.TabIndex = 6;
			this.checkPriClaimAllowSetToHoldUntilPriReceived.Text = "Allow primary claim status to be \'Hold Until Pri Received\'";
			// 
			// groupBoxClaimIdPrefix
			// 
			this.groupBoxClaimIdPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxClaimIdPrefix.BackColor = System.Drawing.Color.White;
			this.groupBoxClaimIdPrefix.Controls.Add(this.butReplacements);
			this.groupBoxClaimIdPrefix.Controls.Add(this.textClaimIdPrefix);
			this.groupBoxClaimIdPrefix.Location = new System.Drawing.Point(40, 169);
			this.groupBoxClaimIdPrefix.Name = "groupBoxClaimIdPrefix";
			this.groupBoxClaimIdPrefix.Size = new System.Drawing.Size(407, 55);
			this.groupBoxClaimIdPrefix.TabIndex = 7;
			this.groupBoxClaimIdPrefix.Text = "Claim Identification Prefix";
			// 
			// butReplacements
			// 
			this.butReplacements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butReplacements.Location = new System.Drawing.Point(309, 27);
			this.butReplacements.Name = "butReplacements";
			this.butReplacements.Size = new System.Drawing.Size(91, 23);
			this.butReplacements.TabIndex = 1;
			this.butReplacements.Text = "Replacements";
			this.butReplacements.UseVisualStyleBackColor = true;
			this.butReplacements.Click += new System.EventHandler(this.butReplacements_Click);
			// 
			// textClaimIdPrefix
			// 
			this.textClaimIdPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimIdPrefix.Location = new System.Drawing.Point(229, 5);
			this.textClaimIdPrefix.Name = "textClaimIdPrefix";
			this.textClaimIdPrefix.Size = new System.Drawing.Size(171, 20);
			this.textClaimIdPrefix.TabIndex = 0;
			this.textClaimIdPrefix.Validating += new System.ComponentModel.CancelEventHandler(this.textClaimIdPrefix_Validating);
			// 
			// checkClaimsValidateACN
			// 
			this.checkClaimsValidateACN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimsValidateACN.Location = new System.Drawing.Point(73, 131);
			this.checkClaimsValidateACN.Name = "checkClaimsValidateACN";
			this.checkClaimsValidateACN.Size = new System.Drawing.Size(367, 17);
			this.checkClaimsValidateACN.TabIndex = 5;
			this.checkClaimsValidateACN.Text = "Require ACN# in remarks on claims with ADDP group name";
			// 
			// checkCanadianPpoLabEst
			// 
			this.checkCanadianPpoLabEst.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCanadianPpoLabEst.Location = new System.Drawing.Point(93, 323);
			this.checkCanadianPpoLabEst.Name = "checkCanadianPpoLabEst";
			this.checkCanadianPpoLabEst.Size = new System.Drawing.Size(367, 17);
			this.checkCanadianPpoLabEst.TabIndex = 5;
			this.checkCanadianPpoLabEst.Text = "Canadian PPO insurance plans create lab estimates";
			// 
			// UserControlAccountClaimSend
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.textInsWriteoffDescript);
			this.Controls.Add(this.groupBoxClaimsMedical);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.checkCanadianPpoLabEst);
			this.Name = "UserControlAccountClaimSend";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupBoxClaimsMedical.ResumeLayout(false);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.groupBoxClaimIdPrefix.ResumeLayout(false);
			this.groupBoxClaimIdPrefix.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.CheckBox checkPriClaimAllowSetToHoldUntilPriReceived;
		private OpenDental.UI.CheckBox checkCanadianPpoLabEst;
		private System.Windows.Forms.Label labelCreatingClaimsZeroProcedures;
		private UI.ComboBox comboZeroDollarProcClaimBehavior;
		private UI.GroupBox groupBoxClaimIdPrefix;
		private UI.Button butReplacements;
		private System.Windows.Forms.TextBox textClaimIdPrefix;
		private OpenDental.UI.CheckBox checkClaimFormTreatDentSaysSigOnFile;
		private OpenDental.UI.CheckBox checkClaimMedTypeIsInstWhenInsPlanIsMedical;
		private OpenDental.UI.CheckBox checkEclaimsMedicalProvTreatmentAsOrdering;
		private System.Windows.Forms.Label labelClaimAttachmentExportPath;
		private System.Windows.Forms.TextBox textClaimAttachPath;
		private OpenDental.UI.CheckBox checkClaimsValidateACN;
		private System.Windows.Forms.TextBox textInsWriteoffDescript;
		private System.Windows.Forms.Label label17;
		private UI.GroupBox groupBoxOD1;
		private OpenDental.UI.CheckBox checkPpoUseUcr;
		private UI.GroupBox groupBoxClaimsMedical;
		private OpenDental.UI.CheckBox checkEclaimsSeparateTreatProv;
		private UI.CheckBox checkEclaimsSubscIDUsesPatID;
	}
        
}

