using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental {
	partial class FormShowFeatures {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShowFeatures));
			this.butSave = new OpenDental.UI.Button();
			this.checkCapitation = new OpenDental.UI.CheckBox();
			this.checkMedicaid = new OpenDental.UI.CheckBox();
			this.checkClinical = new OpenDental.UI.CheckBox();
			this.checkBasicModules = new OpenDental.UI.CheckBox();
			this.checkPublicHealth = new OpenDental.UI.CheckBox();
			this.checkEnableClinics = new OpenDental.UI.CheckBox();
			this.checkDentalSchools = new OpenDental.UI.CheckBox();
			this.checkRepeatCharges = new OpenDental.UI.CheckBox();
			this.checkInsurance = new OpenDental.UI.CheckBox();
			this.checkHospitals = new OpenDental.UI.CheckBox();
			this.checkMedicalIns = new OpenDental.UI.CheckBox();
			this.checkEhr = new OpenDental.UI.CheckBox();
			this.checkSuperFam = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkPatClone = new OpenDental.UI.CheckBox();
			this.checkShowEnterprise = new OpenDental.UI.CheckBox();
			this.checkShowReactivations = new OpenDental.UI.CheckBox();
			this.checkEraShowControlId = new OpenDental.UI.CheckBox();
			this.groupEnterprise = new OpenDental.UI.GroupBox();
			this.groupFamilyModule = new OpenDental.UI.GroupBox();
			this.groupGeneral = new OpenDental.UI.GroupBox();
			this.groupInsurance = new OpenDental.UI.GroupBox();
			this.groupSpecialUseOnly = new OpenDental.UI.GroupBox();
			this.groupChargeTools = new OpenDental.UI.GroupBox();
			this.groupBoxCharges = new OpenDental.UI.GroupBox();
			this.radioBillingFinance = new System.Windows.Forms.RadioButton();
			this.radioLateCharges = new System.Windows.Forms.RadioButton();
			this.groupEnterprise.SuspendLayout();
			this.groupFamilyModule.SuspendLayout();
			this.groupGeneral.SuspendLayout();
			this.groupInsurance.SuspendLayout();
			this.groupSpecialUseOnly.SuspendLayout();
			this.groupChargeTools.SuspendLayout();
			this.groupBoxCharges.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(432, 341);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 19;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkCapitation
			// 
			this.checkCapitation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCapitation.Location = new System.Drawing.Point(6, 14);
			this.checkCapitation.Name = "checkCapitation";
			this.checkCapitation.Size = new System.Drawing.Size(222, 19);
			this.checkCapitation.TabIndex = 9;
			this.checkCapitation.Text = "Capitation";
			// 
			// checkMedicaid
			// 
			this.checkMedicaid.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicaid.Location = new System.Drawing.Point(6, 39);
			this.checkMedicaid.Name = "checkMedicaid";
			this.checkMedicaid.Size = new System.Drawing.Size(222, 19);
			this.checkMedicaid.TabIndex = 10;
			this.checkMedicaid.Text = "Medicaid";
			// 
			// checkClinical
			// 
			this.checkClinical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinical.Location = new System.Drawing.Point(6, 42);
			this.checkClinical.Name = "checkClinical";
			this.checkClinical.Size = new System.Drawing.Size(222, 19);
			this.checkClinical.TabIndex = 14;
			this.checkClinical.Text = "Clinical (computers in operatories)";
			// 
			// checkBasicModules
			// 
			this.checkBasicModules.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBasicModules.Location = new System.Drawing.Point(6, 17);
			this.checkBasicModules.Name = "checkBasicModules";
			this.checkBasicModules.Size = new System.Drawing.Size(222, 19);
			this.checkBasicModules.TabIndex = 13;
			this.checkBasicModules.Text = "Basic Modules Only";
			// 
			// checkPublicHealth
			// 
			this.checkPublicHealth.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPublicHealth.Location = new System.Drawing.Point(6, 142);
			this.checkPublicHealth.Name = "checkPublicHealth";
			this.checkPublicHealth.Size = new System.Drawing.Size(222, 19);
			this.checkPublicHealth.TabIndex = 18;
			this.checkPublicHealth.Text = "Public Health";
			this.checkPublicHealth.Click += new System.EventHandler(this.checkPublicHealth_Click);
			// 
			// checkEnableClinics
			// 
			this.checkEnableClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableClinics.Location = new System.Drawing.Point(39, 14);
			this.checkEnableClinics.Name = "checkEnableClinics";
			this.checkEnableClinics.Size = new System.Drawing.Size(189, 19);
			this.checkEnableClinics.TabIndex = 3;
			this.checkEnableClinics.Text = "Clinics (multiple office locations)";
			this.checkEnableClinics.Click += new System.EventHandler(this.checkEnableClinics_Click);
			// 
			// checkDentalSchools
			// 
			this.checkDentalSchools.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDentalSchools.Location = new System.Drawing.Point(6, 67);
			this.checkDentalSchools.Name = "checkDentalSchools";
			this.checkDentalSchools.Size = new System.Drawing.Size(222, 19);
			this.checkDentalSchools.TabIndex = 15;
			this.checkDentalSchools.Text = "Dental Schools";
			this.checkDentalSchools.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkRepeatCharges
			// 
			this.checkRepeatCharges.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatCharges.Location = new System.Drawing.Point(39, 67);
			this.checkRepeatCharges.Name = "checkRepeatCharges";
			this.checkRepeatCharges.Size = new System.Drawing.Size(189, 19);
			this.checkRepeatCharges.TabIndex = 2;
			this.checkRepeatCharges.Text = "Repeating Charges";
			this.checkRepeatCharges.Click += new System.EventHandler(this.checkRepeatCharges_Click);
			// 
			// checkInsurance
			// 
			this.checkInsurance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsurance.Location = new System.Drawing.Point(6, 89);
			this.checkInsurance.Name = "checkInsurance";
			this.checkInsurance.Size = new System.Drawing.Size(222, 19);
			this.checkInsurance.TabIndex = 12;
			this.checkInsurance.Text = "Use Insurance";
			this.checkInsurance.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkHospitals
			// 
			this.checkHospitals.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHospitals.Location = new System.Drawing.Point(6, 117);
			this.checkHospitals.Name = "checkHospitals";
			this.checkHospitals.Size = new System.Drawing.Size(222, 19);
			this.checkHospitals.TabIndex = 17;
			this.checkHospitals.Text = "Hospitals";
			// 
			// checkMedicalIns
			// 
			this.checkMedicalIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicalIns.Location = new System.Drawing.Point(6, 64);
			this.checkMedicalIns.Name = "checkMedicalIns";
			this.checkMedicalIns.Size = new System.Drawing.Size(222, 19);
			this.checkMedicalIns.TabIndex = 11;
			this.checkMedicalIns.Text = "Medical Insurance";
			// 
			// checkEhr
			// 
			this.checkEhr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEhr.Location = new System.Drawing.Point(6, 92);
			this.checkEhr.Name = "checkEhr";
			this.checkEhr.Size = new System.Drawing.Size(222, 19);
			this.checkEhr.TabIndex = 16;
			this.checkEhr.Text = "EHR";
			this.checkEhr.Click += new System.EventHandler(this.checkEhr_Click);
			// 
			// checkSuperFam
			// 
			this.checkSuperFam.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFam.Location = new System.Drawing.Point(39, 39);
			this.checkSuperFam.Name = "checkSuperFam";
			this.checkSuperFam.Size = new System.Drawing.Size(189, 19);
			this.checkSuperFam.TabIndex = 7;
			this.checkSuperFam.Text = "Super Families";
			this.checkSuperFam.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(501, 18);
			this.label1.TabIndex = 30;
			this.label1.Text = "The following settings will affect all computers.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// checkPatClone
			// 
			this.checkPatClone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatClone.Location = new System.Drawing.Point(39, 14);
			this.checkPatClone.Name = "checkPatClone";
			this.checkPatClone.Size = new System.Drawing.Size(189, 19);
			this.checkPatClone.TabIndex = 6;
			this.checkPatClone.Text = "Patient Clone";
			this.checkPatClone.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkShowEnterprise
			// 
			this.checkShowEnterprise.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowEnterprise.Location = new System.Drawing.Point(39, 39);
			this.checkShowEnterprise.Name = "checkShowEnterprise";
			this.checkShowEnterprise.Size = new System.Drawing.Size(189, 19);
			this.checkShowEnterprise.TabIndex = 4;
			this.checkShowEnterprise.Text = "Enterprise Setup";
			// 
			// checkShowReactivations
			// 
			this.checkShowReactivations.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowReactivations.Location = new System.Drawing.Point(39, 14);
			this.checkShowReactivations.Name = "checkShowReactivations";
			this.checkShowReactivations.Size = new System.Drawing.Size(189, 19);
			this.checkShowReactivations.TabIndex = 8;
			this.checkShowReactivations.Text = "Reactivation List";
			this.checkShowReactivations.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkEraShowControlId
			// 
			this.checkEraShowControlId.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraShowControlId.Location = new System.Drawing.Point(39, 64);
			this.checkEraShowControlId.Name = "checkEraShowControlId";
			this.checkEraShowControlId.Size = new System.Drawing.Size(189, 19);
			this.checkEraShowControlId.TabIndex = 5;
			this.checkEraShowControlId.Text = "ERA Control ID Filter";
			// 
			// groupEnterprise
			// 
			this.groupEnterprise.Controls.Add(this.checkEnableClinics);
			this.groupEnterprise.Controls.Add(this.checkEraShowControlId);
			this.groupEnterprise.Controls.Add(this.checkShowEnterprise);
			this.groupEnterprise.Location = new System.Drawing.Point(6, 129);
			this.groupEnterprise.Name = "groupEnterprise";
			this.groupEnterprise.Size = new System.Drawing.Size(241, 87);
			this.groupEnterprise.TabIndex = 24;
			this.groupEnterprise.TabStop = false;
			this.groupEnterprise.Text = "Enterprise";
			// 
			// groupFamilyModule
			// 
			this.groupFamilyModule.Controls.Add(this.checkSuperFam);
			this.groupFamilyModule.Controls.Add(this.checkPatClone);
			this.groupFamilyModule.Location = new System.Drawing.Point(6, 222);
			this.groupFamilyModule.Name = "groupFamilyModule";
			this.groupFamilyModule.Size = new System.Drawing.Size(241, 62);
			this.groupFamilyModule.TabIndex = 25;
			this.groupFamilyModule.TabStop = false;
			this.groupFamilyModule.Text = "Family Module";
			// 
			// groupGeneral
			// 
			this.groupGeneral.Controls.Add(this.checkShowReactivations);
			this.groupGeneral.Location = new System.Drawing.Point(6, 290);
			this.groupGeneral.Name = "groupGeneral";
			this.groupGeneral.Size = new System.Drawing.Size(241, 44);
			this.groupGeneral.TabIndex = 26;
			this.groupGeneral.TabStop = false;
			this.groupGeneral.Text = "General";
			// 
			// groupInsurance
			// 
			this.groupInsurance.Controls.Add(this.checkCapitation);
			this.groupInsurance.Controls.Add(this.checkMedicaid);
			this.groupInsurance.Controls.Add(this.checkMedicalIns);
			this.groupInsurance.Controls.Add(this.checkInsurance);
			this.groupInsurance.Location = new System.Drawing.Point(266, 33);
			this.groupInsurance.Name = "groupInsurance";
			this.groupInsurance.Size = new System.Drawing.Size(241, 117);
			this.groupInsurance.TabIndex = 27;
			this.groupInsurance.TabStop = false;
			this.groupInsurance.Text = "Insurance";
			// 
			// groupSpecialUseOnly
			// 
			this.groupSpecialUseOnly.Controls.Add(this.checkBasicModules);
			this.groupSpecialUseOnly.Controls.Add(this.checkClinical);
			this.groupSpecialUseOnly.Controls.Add(this.checkDentalSchools);
			this.groupSpecialUseOnly.Controls.Add(this.checkHospitals);
			this.groupSpecialUseOnly.Controls.Add(this.checkPublicHealth);
			this.groupSpecialUseOnly.Controls.Add(this.checkEhr);
			this.groupSpecialUseOnly.Location = new System.Drawing.Point(266, 161);
			this.groupSpecialUseOnly.Name = "groupSpecialUseOnly";
			this.groupSpecialUseOnly.Size = new System.Drawing.Size(241, 173);
			this.groupSpecialUseOnly.TabIndex = 28;
			this.groupSpecialUseOnly.TabStop = false;
			this.groupSpecialUseOnly.Text = "Special Use Only";
			// 
			// groupChargeTools
			// 
			this.groupChargeTools.Controls.Add(this.groupBoxCharges);
			this.groupChargeTools.Controls.Add(this.checkRepeatCharges);
			this.groupChargeTools.Location = new System.Drawing.Point(6, 33);
			this.groupChargeTools.Name = "groupChargeTools";
			this.groupChargeTools.Size = new System.Drawing.Size(241, 90);
			this.groupChargeTools.TabIndex = 23;
			this.groupChargeTools.TabStop = false;
			this.groupChargeTools.Text = "Charge Tools";
			// 
			// groupBoxCharges
			// 
			this.groupBoxCharges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxCharges.Controls.Add(this.radioBillingFinance);
			this.groupBoxCharges.Controls.Add(this.radioLateCharges);
			this.groupBoxCharges.Location = new System.Drawing.Point(47, 15);
			this.groupBoxCharges.Name = "groupBoxCharges";
			this.groupBoxCharges.Size = new System.Drawing.Size(190, 47);
			this.groupBoxCharges.TabIndex = 29;
			this.groupBoxCharges.TabStop = false;
			// 
			// radioBillingFinance
			// 
			this.radioBillingFinance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioBillingFinance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBillingFinance.Location = new System.Drawing.Point(31, 5);
			this.radioBillingFinance.Name = "radioBillingFinance";
			this.radioBillingFinance.Size = new System.Drawing.Size(150, 19);
			this.radioBillingFinance.TabIndex = 0;
			this.radioBillingFinance.TabStop = true;
			this.radioBillingFinance.Text = "Billing/Finance Charges";
			this.radioBillingFinance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBillingFinance.UseVisualStyleBackColor = true;
			// 
			// radioLateCharges
			// 
			this.radioLateCharges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioLateCharges.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioLateCharges.Location = new System.Drawing.Point(51, 25);
			this.radioLateCharges.Name = "radioLateCharges";
			this.radioLateCharges.Size = new System.Drawing.Size(130, 19);
			this.radioLateCharges.TabIndex = 1;
			this.radioLateCharges.TabStop = true;
			this.radioLateCharges.Text = "Late Charges";
			this.radioLateCharges.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioLateCharges.UseVisualStyleBackColor = true;
			// 
			// FormShowFeatures
			// 
			this.ClientSize = new System.Drawing.Size(514, 377);
			this.Controls.Add(this.groupChargeTools);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.groupSpecialUseOnly);
			this.Controls.Add(this.groupInsurance);
			this.Controls.Add(this.groupGeneral);
			this.Controls.Add(this.groupFamilyModule);
			this.Controls.Add(this.groupEnterprise);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormShowFeatures";
			this.ShowInTaskbar = false;
			this.Text = "Show Features";
			this.Load += new System.EventHandler(this.FormShowFeatures_Load);
			this.groupEnterprise.ResumeLayout(false);
			this.groupFamilyModule.ResumeLayout(false);
			this.groupGeneral.ResumeLayout(false);
			this.groupInsurance.ResumeLayout(false);
			this.groupSpecialUseOnly.ResumeLayout(false);
			this.groupChargeTools.ResumeLayout(false);
			this.groupBoxCharges.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkClinical;
		private OpenDental.UI.CheckBox checkBasicModules;
		private OpenDental.UI.CheckBox checkPublicHealth;
		private OpenDental.UI.CheckBox checkEnableClinics;
		private OpenDental.UI.CheckBox checkDentalSchools;
		private OpenDental.UI.CheckBox checkRepeatCharges;
		private OpenDental.UI.CheckBox checkInsurance;
		private OpenDental.UI.CheckBox checkHospitals;
		private OpenDental.UI.CheckBox checkMedicalIns;
		private OpenDental.UI.CheckBox checkEhr;
		private OpenDental.UI.CheckBox checkSuperFam;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.CheckBox checkPatClone;
		///<summary>Keeps track the clinic preference state when the window was loaded.</summary>
		private OpenDental.UI.CheckBox checkShowEnterprise;
		private OpenDental.UI.CheckBox checkMedicaid;
		private OpenDental.UI.CheckBox checkCapitation;
		private OpenDental.UI.CheckBox checkShowReactivations;
		private OpenDental.UI.CheckBox checkEraShowControlId;
		private OpenDental.UI.GroupBox groupEnterprise;
		private OpenDental.UI.GroupBox groupFamilyModule;
		private OpenDental.UI.GroupBox groupGeneral;
		private OpenDental.UI.GroupBox groupInsurance;
		private OpenDental.UI.GroupBox groupSpecialUseOnly;
		private OpenDental.UI.GroupBox groupChargeTools;
		private System.Windows.Forms.RadioButton radioLateCharges;
		private System.Windows.Forms.RadioButton radioBillingFinance;
		private OpenDental.UI.GroupBox groupBoxCharges;
	}
}
