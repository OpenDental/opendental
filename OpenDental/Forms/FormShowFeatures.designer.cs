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
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkCapitation = new System.Windows.Forms.CheckBox();
			this.checkMedicaid = new System.Windows.Forms.CheckBox();
			this.checkClinical = new System.Windows.Forms.CheckBox();
			this.checkBasicModules = new System.Windows.Forms.CheckBox();
			this.checkPublicHealth = new System.Windows.Forms.CheckBox();
			this.checkEnableClinics = new System.Windows.Forms.CheckBox();
			this.checkDentalSchools = new System.Windows.Forms.CheckBox();
			this.checkRepeatCharges = new System.Windows.Forms.CheckBox();
			this.checkInsurance = new System.Windows.Forms.CheckBox();
			this.checkHospitals = new System.Windows.Forms.CheckBox();
			this.checkMedicalIns = new System.Windows.Forms.CheckBox();
			this.checkEhr = new System.Windows.Forms.CheckBox();
			this.checkSuperFam = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkPatClone = new System.Windows.Forms.CheckBox();
			this.checkShowEnterprise = new System.Windows.Forms.CheckBox();
			this.checkShowReactivations = new System.Windows.Forms.CheckBox();
			this.checkEraShowControlId = new System.Windows.Forms.CheckBox();
			this.groupEnterprise = new System.Windows.Forms.GroupBox();
			this.groupFamilyModule = new System.Windows.Forms.GroupBox();
			this.groupGeneral = new System.Windows.Forms.GroupBox();
			this.groupInsurance = new System.Windows.Forms.GroupBox();
			this.groupSpecialUseOnly = new System.Windows.Forms.GroupBox();
			this.groupChargeTools = new System.Windows.Forms.GroupBox();
			this.groupBoxCharges = new OpenDental.UI.GroupBoxOD();
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
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(433, 342);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 20;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(352, 342);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 19;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkCapitation
			// 
			this.checkCapitation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCapitation.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkCapitation.Location = new System.Drawing.Point(6, 14);
			this.checkCapitation.Name = "checkCapitation";
			this.checkCapitation.Size = new System.Drawing.Size(222, 19);
			this.checkCapitation.TabIndex = 9;
			this.checkCapitation.Text = "Capitation";
			this.checkCapitation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkMedicaid
			// 
			this.checkMedicaid.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicaid.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMedicaid.Location = new System.Drawing.Point(6, 39);
			this.checkMedicaid.Name = "checkMedicaid";
			this.checkMedicaid.Size = new System.Drawing.Size(222, 19);
			this.checkMedicaid.TabIndex = 10;
			this.checkMedicaid.Text = "Medicaid";
			this.checkMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkClinical
			// 
			this.checkClinical.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinical.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClinical.Location = new System.Drawing.Point(6, 42);
			this.checkClinical.Name = "checkClinical";
			this.checkClinical.Size = new System.Drawing.Size(222, 19);
			this.checkClinical.TabIndex = 14;
			this.checkClinical.Text = "Clinical (computers in operatories)";
			this.checkClinical.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBasicModules
			// 
			this.checkBasicModules.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBasicModules.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBasicModules.Location = new System.Drawing.Point(6, 17);
			this.checkBasicModules.Name = "checkBasicModules";
			this.checkBasicModules.Size = new System.Drawing.Size(222, 19);
			this.checkBasicModules.TabIndex = 13;
			this.checkBasicModules.Text = "Basic Modules Only";
			this.checkBasicModules.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPublicHealth
			// 
			this.checkPublicHealth.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPublicHealth.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPublicHealth.Location = new System.Drawing.Point(6, 142);
			this.checkPublicHealth.Name = "checkPublicHealth";
			this.checkPublicHealth.Size = new System.Drawing.Size(222, 19);
			this.checkPublicHealth.TabIndex = 18;
			this.checkPublicHealth.Text = "Public Health";
			this.checkPublicHealth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEnableClinics
			// 
			this.checkEnableClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnableClinics.Location = new System.Drawing.Point(39, 14);
			this.checkEnableClinics.Name = "checkEnableClinics";
			this.checkEnableClinics.Size = new System.Drawing.Size(189, 19);
			this.checkEnableClinics.TabIndex = 3;
			this.checkEnableClinics.Text = "Clinics (multiple office locations)";
			this.checkEnableClinics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnableClinics.Click += new System.EventHandler(this.checkEnableClinics_Click);
			// 
			// checkDentalSchools
			// 
			this.checkDentalSchools.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDentalSchools.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDentalSchools.Location = new System.Drawing.Point(6, 67);
			this.checkDentalSchools.Name = "checkDentalSchools";
			this.checkDentalSchools.Size = new System.Drawing.Size(222, 19);
			this.checkDentalSchools.TabIndex = 15;
			this.checkDentalSchools.Text = "Dental Schools";
			this.checkDentalSchools.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDentalSchools.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkRepeatCharges
			// 
			this.checkRepeatCharges.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatCharges.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRepeatCharges.Location = new System.Drawing.Point(39, 67);
			this.checkRepeatCharges.Name = "checkRepeatCharges";
			this.checkRepeatCharges.Size = new System.Drawing.Size(189, 19);
			this.checkRepeatCharges.TabIndex = 2;
			this.checkRepeatCharges.Text = "Repeating Charges";
			this.checkRepeatCharges.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRepeatCharges.Click += new System.EventHandler(this.checkRepeatCharges_Click);
			// 
			// checkInsurance
			// 
			this.checkInsurance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsurance.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkInsurance.Location = new System.Drawing.Point(6, 89);
			this.checkInsurance.Name = "checkInsurance";
			this.checkInsurance.Size = new System.Drawing.Size(222, 19);
			this.checkInsurance.TabIndex = 12;
			this.checkInsurance.Text = "Use Insurance";
			this.checkInsurance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsurance.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkHospitals
			// 
			this.checkHospitals.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHospitals.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHospitals.Location = new System.Drawing.Point(6, 117);
			this.checkHospitals.Name = "checkHospitals";
			this.checkHospitals.Size = new System.Drawing.Size(222, 19);
			this.checkHospitals.TabIndex = 17;
			this.checkHospitals.Text = "Hospitals";
			this.checkHospitals.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkMedicalIns
			// 
			this.checkMedicalIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMedicalIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMedicalIns.Location = new System.Drawing.Point(6, 64);
			this.checkMedicalIns.Name = "checkMedicalIns";
			this.checkMedicalIns.Size = new System.Drawing.Size(222, 19);
			this.checkMedicalIns.TabIndex = 11;
			this.checkMedicalIns.Text = "Medical Insurance";
			this.checkMedicalIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEhr
			// 
			this.checkEhr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEhr.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEhr.Location = new System.Drawing.Point(6, 92);
			this.checkEhr.Name = "checkEhr";
			this.checkEhr.Size = new System.Drawing.Size(222, 19);
			this.checkEhr.TabIndex = 16;
			this.checkEhr.Text = "EHR";
			this.checkEhr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEhr.Click += new System.EventHandler(this.checkEhr_Click);
			// 
			// checkSuperFam
			// 
			this.checkSuperFam.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperFam.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperFam.Location = new System.Drawing.Point(39, 39);
			this.checkSuperFam.Name = "checkSuperFam";
			this.checkSuperFam.Size = new System.Drawing.Size(189, 19);
			this.checkSuperFam.TabIndex = 7;
			this.checkSuperFam.Text = "Super Families";
			this.checkSuperFam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkPatClone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatClone.Location = new System.Drawing.Point(39, 14);
			this.checkPatClone.Name = "checkPatClone";
			this.checkPatClone.Size = new System.Drawing.Size(189, 19);
			this.checkPatClone.TabIndex = 6;
			this.checkPatClone.Text = "Patient Clone";
			this.checkPatClone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatClone.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkShowEnterprise
			// 
			this.checkShowEnterprise.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowEnterprise.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowEnterprise.Location = new System.Drawing.Point(39, 39);
			this.checkShowEnterprise.Name = "checkShowEnterprise";
			this.checkShowEnterprise.Size = new System.Drawing.Size(189, 19);
			this.checkShowEnterprise.TabIndex = 4;
			this.checkShowEnterprise.Text = "Enterprise Setup";
			this.checkShowEnterprise.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowReactivations
			// 
			this.checkShowReactivations.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowReactivations.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowReactivations.Location = new System.Drawing.Point(39, 14);
			this.checkShowReactivations.Name = "checkShowReactivations";
			this.checkShowReactivations.Size = new System.Drawing.Size(189, 19);
			this.checkShowReactivations.TabIndex = 8;
			this.checkShowReactivations.Text = "Reactivation List";
			this.checkShowReactivations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowReactivations.Click += new System.EventHandler(this.checkRestart_Click);
			// 
			// checkEraShowControlId
			// 
			this.checkEraShowControlId.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraShowControlId.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEraShowControlId.Location = new System.Drawing.Point(39, 64);
			this.checkEraShowControlId.Name = "checkEraShowControlId";
			this.checkEraShowControlId.Size = new System.Drawing.Size(189, 19);
			this.checkEraShowControlId.TabIndex = 5;
			this.checkEraShowControlId.Text = "ERA Control ID Filter";
			this.checkEraShowControlId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(514, 377);
			this.Controls.Add(this.groupChargeTools);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
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

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.CheckBox checkClinical;
		private System.Windows.Forms.CheckBox checkBasicModules;
		private System.Windows.Forms.CheckBox checkPublicHealth;
		private System.Windows.Forms.CheckBox checkEnableClinics;
		private System.Windows.Forms.CheckBox checkDentalSchools;
		private System.Windows.Forms.CheckBox checkRepeatCharges;
		private System.Windows.Forms.CheckBox checkInsurance;
		private System.Windows.Forms.CheckBox checkHospitals;
		private System.Windows.Forms.CheckBox checkMedicalIns;
		private System.Windows.Forms.CheckBox checkEhr;
		private System.Windows.Forms.CheckBox checkSuperFam;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkPatClone;
		///<summary>Keeps track the clinic preference state when the window was loaded.</summary>
		private System.Windows.Forms.CheckBox checkShowEnterprise;
		private System.Windows.Forms.CheckBox checkMedicaid;
		private System.Windows.Forms.CheckBox checkCapitation;
		private System.Windows.Forms.CheckBox checkShowReactivations;
		private System.Windows.Forms.CheckBox checkEraShowControlId;
		private System.Windows.Forms.GroupBox groupEnterprise;
		private System.Windows.Forms.GroupBox groupFamilyModule;
		private System.Windows.Forms.GroupBox groupGeneral;
		private System.Windows.Forms.GroupBox groupInsurance;
		private System.Windows.Forms.GroupBox groupSpecialUseOnly;
		private System.Windows.Forms.GroupBox groupChargeTools;
		private System.Windows.Forms.RadioButton radioLateCharges;
		private System.Windows.Forms.RadioButton radioBillingFinance;
		private UI.GroupBoxOD groupBoxCharges;
	}
}
