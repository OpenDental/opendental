namespace OpenDental{
	partial class FormInsVerificationSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsVerificationSetup));
			this.checkExcludePatientClones = new OpenDental.UI.CheckBox();
			this.checkInsVerifyExcludePatVerify = new OpenDental.UI.CheckBox();
			this.groupStandard = new OpenDental.UI.GroupBox();
			this.labelPastDueDaysStandard = new System.Windows.Forms.Label();
			this.textPastDueDaysStandard = new OpenDental.ValidNum();
			this.labelPastDueStandard = new System.Windows.Forms.Label();
			this.labelSchedApptDaysStandard = new System.Windows.Forms.Label();
			this.textPatientEnrollmentDaysStandard = new OpenDental.ValidNum();
			this.labelScheduledAppointmentStandard = new System.Windows.Forms.Label();
			this.textScheduledAppointmentDaysStandard = new OpenDental.ValidNum();
			this.labelPatientEligibilityDaysStandard = new System.Windows.Forms.Label();
			this.labelPlanBenefitsStandard = new System.Windows.Forms.Label();
			this.labelPatientEligibilityStandard = new System.Windows.Forms.Label();
			this.labelPlanBenefitsDaysStandard = new System.Windows.Forms.Label();
			this.textInsBenefitEligibilityDaysStandard = new OpenDental.ValidNum();
			this.checkInsVerifyUseCurrentUser = new OpenDental.UI.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkFutureDateBenefitYear = new OpenDental.UI.CheckBox();
			this.checkFutureDatePatEnrollmentYear = new OpenDental.UI.CheckBox();
			this.groupMedicaid = new OpenDental.UI.GroupBox();
			this.labelInsuranceFilingCodeInMedicaid = new System.Windows.Forms.Label();
			this.listBoxInsFilingCodes = new OpenDental.UI.ListBox();
			this.labelPastDueDaysMedicaid = new System.Windows.Forms.Label();
			this.textPastDueDaysMedicaid = new OpenDental.ValidNum();
			this.labelPastDueMedicaid = new System.Windows.Forms.Label();
			this.labelSchedApptDaysMedicaid = new System.Windows.Forms.Label();
			this.textPatientEnrollmentDaysMedicaid = new OpenDental.ValidNum();
			this.labelScheduledAppointmentMedicaid = new System.Windows.Forms.Label();
			this.textScheduledAppointmentDaysMedicaid = new OpenDental.ValidNum();
			this.labelPatientEligibilityDaysMedicaid = new System.Windows.Forms.Label();
			this.labelPlanBenefitsMedicaid = new System.Windows.Forms.Label();
			this.labelPatientEligibilityMedicaid = new System.Windows.Forms.Label();
			this.labelPlanBenefitsDaysMedicaid = new System.Windows.Forms.Label();
			this.textInsBenefitEligibilityDaysMedicaid = new OpenDental.ValidNum();
			this.groupStandard.SuspendLayout();
			this.groupMedicaid.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkExcludePatientClones
			// 
			this.checkExcludePatientClones.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludePatientClones.Location = new System.Drawing.Point(1, 243);
			this.checkExcludePatientClones.Name = "checkExcludePatientClones";
			this.checkExcludePatientClones.Size = new System.Drawing.Size(346, 16);
			this.checkExcludePatientClones.TabIndex = 233;
			this.checkExcludePatientClones.Text = "Exclude patient clones";
			// 
			// checkInsVerifyExcludePatVerify
			// 
			this.checkInsVerifyExcludePatVerify.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsVerifyExcludePatVerify.Location = new System.Drawing.Point(1, 189);
			this.checkInsVerifyExcludePatVerify.Name = "checkInsVerifyExcludePatVerify";
			this.checkInsVerifyExcludePatVerify.Size = new System.Drawing.Size(346, 16);
			this.checkInsVerifyExcludePatVerify.TabIndex = 232;
			this.checkInsVerifyExcludePatVerify.Text = "Exclude patients with insurance plans marked as Do Not Verify";
			// 
			// groupStandard
			// 
			this.groupStandard.Controls.Add(this.labelPastDueDaysStandard);
			this.groupStandard.Controls.Add(this.textPastDueDaysStandard);
			this.groupStandard.Controls.Add(this.labelPastDueStandard);
			this.groupStandard.Controls.Add(this.labelSchedApptDaysStandard);
			this.groupStandard.Controls.Add(this.textPatientEnrollmentDaysStandard);
			this.groupStandard.Controls.Add(this.labelScheduledAppointmentStandard);
			this.groupStandard.Controls.Add(this.textScheduledAppointmentDaysStandard);
			this.groupStandard.Controls.Add(this.labelPatientEligibilityDaysStandard);
			this.groupStandard.Controls.Add(this.labelPlanBenefitsStandard);
			this.groupStandard.Controls.Add(this.labelPatientEligibilityStandard);
			this.groupStandard.Controls.Add(this.labelPlanBenefitsDaysStandard);
			this.groupStandard.Controls.Add(this.textInsBenefitEligibilityDaysStandard);
			this.groupStandard.Location = new System.Drawing.Point(99, 24);
			this.groupStandard.Name = "groupStandard";
			this.groupStandard.Size = new System.Drawing.Size(288, 125);
			this.groupStandard.TabIndex = 231;
			this.groupStandard.Text = "Show In Standard List When";
			// 
			// labelPastDueDaysStandard
			// 
			this.labelPastDueDaysStandard.Location = new System.Drawing.Point(252, 94);
			this.labelPastDueDaysStandard.Name = "labelPastDueDaysStandard";
			this.labelPastDueDaysStandard.Size = new System.Drawing.Size(35, 20);
			this.labelPastDueDaysStandard.TabIndex = 239;
			this.labelPastDueDaysStandard.Text = "days";
			this.labelPastDueDaysStandard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPastDueDaysStandard
			// 
			this.textPastDueDaysStandard.Location = new System.Drawing.Point(216, 94);
			this.textPastDueDaysStandard.MaxVal = 99999;
			this.textPastDueDaysStandard.MinVal = 1;
			this.textPastDueDaysStandard.Name = "textPastDueDaysStandard";
			this.textPastDueDaysStandard.ShowZero = false;
			this.textPastDueDaysStandard.Size = new System.Drawing.Size(32, 20);
			this.textPastDueDaysStandard.TabIndex = 238;
			this.textPastDueDaysStandard.Text = "1";
			this.textPastDueDaysStandard.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPastDueStandard
			// 
			this.labelPastDueStandard.Location = new System.Drawing.Point(14, 94);
			this.labelPastDueStandard.Name = "labelPastDueStandard";
			this.labelPastDueStandard.Size = new System.Drawing.Size(200, 20);
			this.labelPastDueStandard.TabIndex = 237;
			this.labelPastDueStandard.Text = "Past due appointments up to";
			this.labelPastDueStandard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSchedApptDaysStandard
			// 
			this.labelSchedApptDaysStandard.Location = new System.Drawing.Point(252, 16);
			this.labelSchedApptDaysStandard.Name = "labelSchedApptDaysStandard";
			this.labelSchedApptDaysStandard.Size = new System.Drawing.Size(35, 20);
			this.labelSchedApptDaysStandard.TabIndex = 231;
			this.labelSchedApptDaysStandard.Text = "days";
			this.labelSchedApptDaysStandard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPatientEnrollmentDaysStandard
			// 
			this.textPatientEnrollmentDaysStandard.Location = new System.Drawing.Point(216, 68);
			this.textPatientEnrollmentDaysStandard.MaxVal = 99999;
			this.textPatientEnrollmentDaysStandard.Name = "textPatientEnrollmentDaysStandard";
			this.textPatientEnrollmentDaysStandard.ShowZero = false;
			this.textPatientEnrollmentDaysStandard.Size = new System.Drawing.Size(32, 20);
			this.textPatientEnrollmentDaysStandard.TabIndex = 84;
			this.textPatientEnrollmentDaysStandard.Text = "0";
			this.textPatientEnrollmentDaysStandard.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelScheduledAppointmentStandard
			// 
			this.labelScheduledAppointmentStandard.Location = new System.Drawing.Point(5, 16);
			this.labelScheduledAppointmentStandard.Name = "labelScheduledAppointmentStandard";
			this.labelScheduledAppointmentStandard.Size = new System.Drawing.Size(209, 20);
			this.labelScheduledAppointmentStandard.TabIndex = 230;
			this.labelScheduledAppointmentStandard.Text = "Scheduled appointment in";
			this.labelScheduledAppointmentStandard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textScheduledAppointmentDaysStandard
			// 
			this.textScheduledAppointmentDaysStandard.Location = new System.Drawing.Point(216, 16);
			this.textScheduledAppointmentDaysStandard.MaxVal = 99999;
			this.textScheduledAppointmentDaysStandard.Name = "textScheduledAppointmentDaysStandard";
			this.textScheduledAppointmentDaysStandard.ShowZero = false;
			this.textScheduledAppointmentDaysStandard.Size = new System.Drawing.Size(32, 20);
			this.textScheduledAppointmentDaysStandard.TabIndex = 86;
			this.textScheduledAppointmentDaysStandard.Text = "0";
			this.textScheduledAppointmentDaysStandard.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPatientEligibilityDaysStandard
			// 
			this.labelPatientEligibilityDaysStandard.Location = new System.Drawing.Point(252, 68);
			this.labelPatientEligibilityDaysStandard.Name = "labelPatientEligibilityDaysStandard";
			this.labelPatientEligibilityDaysStandard.Size = new System.Drawing.Size(35, 20);
			this.labelPatientEligibilityDaysStandard.TabIndex = 229;
			this.labelPatientEligibilityDaysStandard.Text = "days";
			this.labelPatientEligibilityDaysStandard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPlanBenefitsStandard
			// 
			this.labelPlanBenefitsStandard.Location = new System.Drawing.Point(2, 42);
			this.labelPlanBenefitsStandard.Name = "labelPlanBenefitsStandard";
			this.labelPlanBenefitsStandard.Size = new System.Drawing.Size(212, 20);
			this.labelPlanBenefitsStandard.TabIndex = 75;
			this.labelPlanBenefitsStandard.Text = "Plan benefits haven\'t been verified in";
			this.labelPlanBenefitsStandard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPatientEligibilityStandard
			// 
			this.labelPatientEligibilityStandard.Location = new System.Drawing.Point(2, 68);
			this.labelPatientEligibilityStandard.Name = "labelPatientEligibilityStandard";
			this.labelPatientEligibilityStandard.Size = new System.Drawing.Size(212, 20);
			this.labelPatientEligibilityStandard.TabIndex = 83;
			this.labelPatientEligibilityStandard.Text = "Patient eligibility hasn\'t been verified in";
			this.labelPatientEligibilityStandard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPlanBenefitsDaysStandard
			// 
			this.labelPlanBenefitsDaysStandard.Location = new System.Drawing.Point(252, 42);
			this.labelPlanBenefitsDaysStandard.Name = "labelPlanBenefitsDaysStandard";
			this.labelPlanBenefitsDaysStandard.Size = new System.Drawing.Size(35, 20);
			this.labelPlanBenefitsDaysStandard.TabIndex = 228;
			this.labelPlanBenefitsDaysStandard.Text = "days";
			this.labelPlanBenefitsDaysStandard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textInsBenefitEligibilityDaysStandard
			// 
			this.textInsBenefitEligibilityDaysStandard.Location = new System.Drawing.Point(216, 42);
			this.textInsBenefitEligibilityDaysStandard.MaxVal = 99999;
			this.textInsBenefitEligibilityDaysStandard.Name = "textInsBenefitEligibilityDaysStandard";
			this.textInsBenefitEligibilityDaysStandard.ShowZero = false;
			this.textInsBenefitEligibilityDaysStandard.Size = new System.Drawing.Size(32, 20);
			this.textInsBenefitEligibilityDaysStandard.TabIndex = 76;
			this.textInsBenefitEligibilityDaysStandard.Text = "0";
			this.textInsBenefitEligibilityDaysStandard.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkInsVerifyUseCurrentUser
			// 
			this.checkInsVerifyUseCurrentUser.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkInsVerifyUseCurrentUser.Location = new System.Drawing.Point(1, 171);
			this.checkInsVerifyUseCurrentUser.Name = "checkInsVerifyUseCurrentUser";
			this.checkInsVerifyUseCurrentUser.Size = new System.Drawing.Size(346, 16);
			this.checkInsVerifyUseCurrentUser.TabIndex = 226;
			this.checkInsVerifyUseCurrentUser.Text = "Insurance Verification List defaults to the current user";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(642, 399);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(723, 399);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkFutureDateBenefitYear
			// 
			this.checkFutureDateBenefitYear.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFutureDateBenefitYear.Location = new System.Drawing.Point(1, 207);
			this.checkFutureDateBenefitYear.Name = "checkFutureDateBenefitYear";
			this.checkFutureDateBenefitYear.Size = new System.Drawing.Size(346, 16);
			this.checkFutureDateBenefitYear.TabIndex = 234;
			this.checkFutureDateBenefitYear.Text = "Always reverify service year insurance benefits";
			// 
			// checkFutureDatePatEnrollmentYear
			// 
			this.checkFutureDatePatEnrollmentYear.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkFutureDatePatEnrollmentYear.Location = new System.Drawing.Point(1, 225);
			this.checkFutureDatePatEnrollmentYear.Name = "checkFutureDatePatEnrollmentYear";
			this.checkFutureDatePatEnrollmentYear.Size = new System.Drawing.Size(346, 16);
			this.checkFutureDatePatEnrollmentYear.TabIndex = 235;
			this.checkFutureDatePatEnrollmentYear.Text = "Always reverify service year patient eligibility";
			// 
			// groupMedicaid
			// 
			this.groupMedicaid.Controls.Add(this.labelInsuranceFilingCodeInMedicaid);
			this.groupMedicaid.Controls.Add(this.listBoxInsFilingCodes);
			this.groupMedicaid.Controls.Add(this.labelPastDueDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.textPastDueDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.labelPastDueMedicaid);
			this.groupMedicaid.Controls.Add(this.labelSchedApptDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.textPatientEnrollmentDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.labelScheduledAppointmentMedicaid);
			this.groupMedicaid.Controls.Add(this.textScheduledAppointmentDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.labelPatientEligibilityDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.labelPlanBenefitsMedicaid);
			this.groupMedicaid.Controls.Add(this.labelPatientEligibilityMedicaid);
			this.groupMedicaid.Controls.Add(this.labelPlanBenefitsDaysMedicaid);
			this.groupMedicaid.Controls.Add(this.textInsBenefitEligibilityDaysMedicaid);
			this.groupMedicaid.Location = new System.Drawing.Point(418, 24);
			this.groupMedicaid.Name = "groupMedicaid";
			this.groupMedicaid.Size = new System.Drawing.Size(350, 351);
			this.groupMedicaid.TabIndex = 236;
			this.groupMedicaid.Text = "Show In Medicaid List When";
			// 
			// labelInsuranceFilingCodeInMedicaid
			// 
			this.labelInsuranceFilingCodeInMedicaid.Location = new System.Drawing.Point(3, 122);
			this.labelInsuranceFilingCodeInMedicaid.Name = "labelInsuranceFilingCodeInMedicaid";
			this.labelInsuranceFilingCodeInMedicaid.Size = new System.Drawing.Size(173, 20);
			this.labelInsuranceFilingCodeInMedicaid.TabIndex = 241;
			this.labelInsuranceFilingCodeInMedicaid.Text = "Insurance filing code in";
			this.labelInsuranceFilingCodeInMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxInsFilingCodes
			// 
			this.listBoxInsFilingCodes.Location = new System.Drawing.Point(182, 122);
			this.listBoxInsFilingCodes.Name = "listBoxInsFilingCodes";
			this.listBoxInsFilingCodes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxInsFilingCodes.Size = new System.Drawing.Size(150, 212);
			this.listBoxInsFilingCodes.TabIndex = 240;
			this.listBoxInsFilingCodes.Text = "listBoxOD1";
			// 
			// labelPastDueDaysMedicaid
			// 
			this.labelPastDueDaysMedicaid.Location = new System.Drawing.Point(297, 94);
			this.labelPastDueDaysMedicaid.Name = "labelPastDueDaysMedicaid";
			this.labelPastDueDaysMedicaid.Size = new System.Drawing.Size(35, 20);
			this.labelPastDueDaysMedicaid.TabIndex = 239;
			this.labelPastDueDaysMedicaid.Text = "days";
			this.labelPastDueDaysMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPastDueDaysMedicaid
			// 
			this.textPastDueDaysMedicaid.Location = new System.Drawing.Point(262, 94);
			this.textPastDueDaysMedicaid.MaxVal = 99999;
			this.textPastDueDaysMedicaid.MinVal = 1;
			this.textPastDueDaysMedicaid.Name = "textPastDueDaysMedicaid";
			this.textPastDueDaysMedicaid.ShowZero = false;
			this.textPastDueDaysMedicaid.Size = new System.Drawing.Size(32, 20);
			this.textPastDueDaysMedicaid.TabIndex = 238;
			this.textPastDueDaysMedicaid.Text = "1";
			this.textPastDueDaysMedicaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPastDueMedicaid
			// 
			this.labelPastDueMedicaid.Location = new System.Drawing.Point(62, 94);
			this.labelPastDueMedicaid.Name = "labelPastDueMedicaid";
			this.labelPastDueMedicaid.Size = new System.Drawing.Size(199, 20);
			this.labelPastDueMedicaid.TabIndex = 237;
			this.labelPastDueMedicaid.Text = "Past due appointments up to";
			this.labelPastDueMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSchedApptDaysMedicaid
			// 
			this.labelSchedApptDaysMedicaid.Location = new System.Drawing.Point(297, 16);
			this.labelSchedApptDaysMedicaid.Name = "labelSchedApptDaysMedicaid";
			this.labelSchedApptDaysMedicaid.Size = new System.Drawing.Size(35, 20);
			this.labelSchedApptDaysMedicaid.TabIndex = 231;
			this.labelSchedApptDaysMedicaid.Text = "days";
			this.labelSchedApptDaysMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPatientEnrollmentDaysMedicaid
			// 
			this.textPatientEnrollmentDaysMedicaid.Location = new System.Drawing.Point(262, 68);
			this.textPatientEnrollmentDaysMedicaid.MaxVal = 99999;
			this.textPatientEnrollmentDaysMedicaid.Name = "textPatientEnrollmentDaysMedicaid";
			this.textPatientEnrollmentDaysMedicaid.ShowZero = false;
			this.textPatientEnrollmentDaysMedicaid.Size = new System.Drawing.Size(32, 20);
			this.textPatientEnrollmentDaysMedicaid.TabIndex = 84;
			this.textPatientEnrollmentDaysMedicaid.Text = "0";
			this.textPatientEnrollmentDaysMedicaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelScheduledAppointmentMedicaid
			// 
			this.labelScheduledAppointmentMedicaid.Location = new System.Drawing.Point(53, 16);
			this.labelScheduledAppointmentMedicaid.Name = "labelScheduledAppointmentMedicaid";
			this.labelScheduledAppointmentMedicaid.Size = new System.Drawing.Size(208, 20);
			this.labelScheduledAppointmentMedicaid.TabIndex = 230;
			this.labelScheduledAppointmentMedicaid.Text = "Scheduled appointment in";
			this.labelScheduledAppointmentMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textScheduledAppointmentDaysMedicaid
			// 
			this.textScheduledAppointmentDaysMedicaid.Location = new System.Drawing.Point(262, 16);
			this.textScheduledAppointmentDaysMedicaid.MaxVal = 99999;
			this.textScheduledAppointmentDaysMedicaid.Name = "textScheduledAppointmentDaysMedicaid";
			this.textScheduledAppointmentDaysMedicaid.ShowZero = false;
			this.textScheduledAppointmentDaysMedicaid.Size = new System.Drawing.Size(32, 20);
			this.textScheduledAppointmentDaysMedicaid.TabIndex = 86;
			this.textScheduledAppointmentDaysMedicaid.Text = "0";
			this.textScheduledAppointmentDaysMedicaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPatientEligibilityDaysMedicaid
			// 
			this.labelPatientEligibilityDaysMedicaid.Location = new System.Drawing.Point(297, 68);
			this.labelPatientEligibilityDaysMedicaid.Name = "labelPatientEligibilityDaysMedicaid";
			this.labelPatientEligibilityDaysMedicaid.Size = new System.Drawing.Size(35, 20);
			this.labelPatientEligibilityDaysMedicaid.TabIndex = 229;
			this.labelPatientEligibilityDaysMedicaid.Text = "days";
			this.labelPatientEligibilityDaysMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPlanBenefitsMedicaid
			// 
			this.labelPlanBenefitsMedicaid.Location = new System.Drawing.Point(50, 42);
			this.labelPlanBenefitsMedicaid.Name = "labelPlanBenefitsMedicaid";
			this.labelPlanBenefitsMedicaid.Size = new System.Drawing.Size(211, 20);
			this.labelPlanBenefitsMedicaid.TabIndex = 75;
			this.labelPlanBenefitsMedicaid.Text = "Plan benefits haven\'t been verified in";
			this.labelPlanBenefitsMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPatientEligibilityMedicaid
			// 
			this.labelPatientEligibilityMedicaid.Location = new System.Drawing.Point(50, 68);
			this.labelPatientEligibilityMedicaid.Name = "labelPatientEligibilityMedicaid";
			this.labelPatientEligibilityMedicaid.Size = new System.Drawing.Size(211, 20);
			this.labelPatientEligibilityMedicaid.TabIndex = 83;
			this.labelPatientEligibilityMedicaid.Text = "Patient eligibility hasn\'t been verified in";
			this.labelPatientEligibilityMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPlanBenefitsDaysMedicaid
			// 
			this.labelPlanBenefitsDaysMedicaid.Location = new System.Drawing.Point(297, 42);
			this.labelPlanBenefitsDaysMedicaid.Name = "labelPlanBenefitsDaysMedicaid";
			this.labelPlanBenefitsDaysMedicaid.Size = new System.Drawing.Size(35, 20);
			this.labelPlanBenefitsDaysMedicaid.TabIndex = 228;
			this.labelPlanBenefitsDaysMedicaid.Text = "days";
			this.labelPlanBenefitsDaysMedicaid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textInsBenefitEligibilityDaysMedicaid
			// 
			this.textInsBenefitEligibilityDaysMedicaid.Location = new System.Drawing.Point(262, 42);
			this.textInsBenefitEligibilityDaysMedicaid.MaxVal = 99999;
			this.textInsBenefitEligibilityDaysMedicaid.Name = "textInsBenefitEligibilityDaysMedicaid";
			this.textInsBenefitEligibilityDaysMedicaid.ShowZero = false;
			this.textInsBenefitEligibilityDaysMedicaid.Size = new System.Drawing.Size(32, 20);
			this.textInsBenefitEligibilityDaysMedicaid.TabIndex = 76;
			this.textInsBenefitEligibilityDaysMedicaid.Text = "0";
			this.textInsBenefitEligibilityDaysMedicaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// FormInsVerificationSetup
			// 
			this.ClientSize = new System.Drawing.Size(810, 435);
			this.Controls.Add(this.groupMedicaid);
			this.Controls.Add(this.checkFutureDatePatEnrollmentYear);
			this.Controls.Add(this.checkFutureDateBenefitYear);
			this.Controls.Add(this.checkExcludePatientClones);
			this.Controls.Add(this.checkInsVerifyExcludePatVerify);
			this.Controls.Add(this.groupStandard);
			this.Controls.Add(this.checkInsVerifyUseCurrentUser);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsVerificationSetup";
			this.Text = "Insurance Verification Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormInsVerificationSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormInsVerificationSetup_Load);
			this.groupStandard.ResumeLayout(false);
			this.groupStandard.PerformLayout();
			this.groupMedicaid.ResumeLayout(false);
			this.groupMedicaid.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelPlanBenefitsStandard;
		private ValidNum textInsBenefitEligibilityDaysStandard;
		private System.Windows.Forms.Label labelPatientEligibilityStandard;
		private ValidNum textScheduledAppointmentDaysStandard;
		private ValidNum textPatientEnrollmentDaysStandard;
		private OpenDental.UI.CheckBox checkInsVerifyUseCurrentUser;
		private System.Windows.Forms.Label labelPlanBenefitsDaysStandard;
		private System.Windows.Forms.Label labelPatientEligibilityDaysStandard;
		private System.Windows.Forms.Label labelScheduledAppointmentStandard;
		private OpenDental.UI.GroupBox groupStandard;
		private System.Windows.Forms.Label labelSchedApptDaysStandard;
		private OpenDental.UI.CheckBox checkInsVerifyExcludePatVerify;
		private OpenDental.UI.CheckBox checkExcludePatientClones;
		private OpenDental.UI.CheckBox checkFutureDateBenefitYear;
		private System.Windows.Forms.Label labelPastDueDaysStandard;
		private ValidNum textPastDueDaysStandard;
		private System.Windows.Forms.Label labelPastDueStandard;
		private OpenDental.UI.CheckBox checkFutureDatePatEnrollmentYear;
		private UI.GroupBox groupMedicaid;
		private System.Windows.Forms.Label labelPastDueDaysMedicaid;
		private ValidNum textPastDueDaysMedicaid;
		private System.Windows.Forms.Label labelPastDueMedicaid;
		private System.Windows.Forms.Label labelSchedApptDaysMedicaid;
		private ValidNum textPatientEnrollmentDaysMedicaid;
		private System.Windows.Forms.Label labelScheduledAppointmentMedicaid;
		private ValidNum textScheduledAppointmentDaysMedicaid;
		private System.Windows.Forms.Label labelPatientEligibilityDaysMedicaid;
		private System.Windows.Forms.Label labelPlanBenefitsMedicaid;
		private System.Windows.Forms.Label labelPatientEligibilityMedicaid;
		private System.Windows.Forms.Label labelPlanBenefitsDaysMedicaid;
		private ValidNum textInsBenefitEligibilityDaysMedicaid;
		private System.Windows.Forms.Label labelInsuranceFilingCodeInMedicaid;
		private UI.ListBox listBoxInsFilingCodes;
	}
}