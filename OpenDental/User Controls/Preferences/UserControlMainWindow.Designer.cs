
namespace OpenDental {
	partial class UserControlMainWindow {
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
			this.groupBoxPopups = new OpenDental.UI.GroupBox();
			this.textPopupsDisableTimeSpan = new System.Windows.Forms.TextBox();
			this.labelTimeSpan = new System.Windows.Forms.Label();
			this.textPopupsDisableDays = new System.Windows.Forms.TextBox();
			this.labelDays = new System.Windows.Forms.Label();
			this.groupSelectPatient = new OpenDental.UI.GroupBox();
			this.checkEnterpriseAllowRefreshWhileTyping = new OpenDental.UI.CheckBox();
			this.checkShowInactivePatientsDefault = new OpenDental.UI.CheckBox();
			this.checkPatientSelectFilterRestrictedClinics = new OpenDental.UI.CheckBox();
			this.checkRefresh = new OpenDental.UI.CheckBox();
			this.checkPrefFName = new OpenDental.UI.CheckBox();
			this.groupBox6 = new OpenDental.UI.GroupBox();
			this.checkTitleBarShowSpecialty = new OpenDental.UI.CheckBox();
			this.checkUseClinicAbbr = new OpenDental.UI.CheckBox();
			this.textMainWindowTitle = new System.Windows.Forms.TextBox();
			this.checkTitleBarShowSite = new OpenDental.UI.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.comboShowID = new OpenDental.UI.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupBoxPopups.SuspendLayout();
			this.groupSelectPatient.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxPopups
			// 
			this.groupBoxPopups.BackColor = System.Drawing.Color.White;
			this.groupBoxPopups.Controls.Add(this.textPopupsDisableTimeSpan);
			this.groupBoxPopups.Controls.Add(this.labelTimeSpan);
			this.groupBoxPopups.Controls.Add(this.textPopupsDisableDays);
			this.groupBoxPopups.Controls.Add(this.labelDays);
			this.groupBoxPopups.Location = new System.Drawing.Point(20, 295);
			this.groupBoxPopups.Name = "groupBoxPopups";
			this.groupBoxPopups.Size = new System.Drawing.Size(450, 66);
			this.groupBoxPopups.TabIndex = 247;
			this.groupBoxPopups.Text = "Popups Disable Timespan";
			// 
			// textPopupsDisableTimeSpan
			// 
			this.textPopupsDisableTimeSpan.Location = new System.Drawing.Point(362, 38);
			this.textPopupsDisableTimeSpan.Name = "textPopupsDisableTimeSpan";
			this.textPopupsDisableTimeSpan.Size = new System.Drawing.Size(75, 20);
			this.textPopupsDisableTimeSpan.TabIndex = 42;
			// 
			// labelTimeSpan
			// 
			this.labelTimeSpan.Location = new System.Drawing.Point(123, 38);
			this.labelTimeSpan.Name = "labelTimeSpan";
			this.labelTimeSpan.Size = new System.Drawing.Size(234, 19);
			this.labelTimeSpan.TabIndex = 43;
			this.labelTimeSpan.Text = "Default timespan (hh:mm:ss) until disabled";
			this.labelTimeSpan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPopupsDisableDays
			// 
			this.textPopupsDisableDays.Location = new System.Drawing.Point(390, 12);
			this.textPopupsDisableDays.Name = "textPopupsDisableDays";
			this.textPopupsDisableDays.Size = new System.Drawing.Size(47, 20);
			this.textPopupsDisableDays.TabIndex = 40;
			// 
			// labelDays
			// 
			this.labelDays.Location = new System.Drawing.Point(187, 12);
			this.labelDays.Name = "labelDays";
			this.labelDays.Size = new System.Drawing.Size(197, 17);
			this.labelDays.TabIndex = 41;
			this.labelDays.Text = "Default number of days until disabled";
			this.labelDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupSelectPatient
			// 
			this.groupSelectPatient.BackColor = System.Drawing.Color.White;
			this.groupSelectPatient.Controls.Add(this.checkEnterpriseAllowRefreshWhileTyping);
			this.groupSelectPatient.Controls.Add(this.checkShowInactivePatientsDefault);
			this.groupSelectPatient.Controls.Add(this.checkPatientSelectFilterRestrictedClinics);
			this.groupSelectPatient.Controls.Add(this.checkRefresh);
			this.groupSelectPatient.Controls.Add(this.checkPrefFName);
			this.groupSelectPatient.Location = new System.Drawing.Point(20, 172);
			this.groupSelectPatient.Name = "groupSelectPatient";
			this.groupSelectPatient.Size = new System.Drawing.Size(450, 110);
			this.groupSelectPatient.TabIndex = 246;
			this.groupSelectPatient.Text = "Select Patient Window";
			// 
			// checkEnterpriseAllowRefreshWhileTyping
			// 
			this.checkEnterpriseAllowRefreshWhileTyping.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseAllowRefreshWhileTyping.Location = new System.Drawing.Point(55, 87);
			this.checkEnterpriseAllowRefreshWhileTyping.Name = "checkEnterpriseAllowRefreshWhileTyping";
			this.checkEnterpriseAllowRefreshWhileTyping.Size = new System.Drawing.Size(382, 18);
			this.checkEnterpriseAllowRefreshWhileTyping.TabIndex = 293;
			this.checkEnterpriseAllowRefreshWhileTyping.Text = "Allow ‘Refresh while typing’ in Select Patient window";
			this.checkEnterpriseAllowRefreshWhileTyping.Click += new System.EventHandler(this.checkEnterpriseAllowRefreshWhileTyping_Click);
			// 
			// checkShowInactivePatientsDefault
			// 
			this.checkShowInactivePatientsDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowInactivePatientsDefault.Location = new System.Drawing.Point(55, 70);
			this.checkShowInactivePatientsDefault.Name = "checkShowInactivePatientsDefault";
			this.checkShowInactivePatientsDefault.Size = new System.Drawing.Size(382, 18);
			this.checkShowInactivePatientsDefault.TabIndex = 292;
			this.checkShowInactivePatientsDefault.Text = "Show Inactive patients by default";
			// 
			// checkPatientSelectFilterRestrictedClinics
			// 
			this.checkPatientSelectFilterRestrictedClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectFilterRestrictedClinics.Location = new System.Drawing.Point(55, 53);
			this.checkPatientSelectFilterRestrictedClinics.Name = "checkPatientSelectFilterRestrictedClinics";
			this.checkPatientSelectFilterRestrictedClinics.Size = new System.Drawing.Size(382, 18);
			this.checkPatientSelectFilterRestrictedClinics.TabIndex = 291;
			this.checkPatientSelectFilterRestrictedClinics.Text = "Hide patients from restricted clinics when viewing \"All\" clinics";
			this.checkPatientSelectFilterRestrictedClinics.Visible = false;
			this.checkPatientSelectFilterRestrictedClinics.Click += new System.EventHandler(this.checkPatientSelectFilterRestrictedClinics_Click);
			// 
			// checkRefresh
			// 
			this.checkRefresh.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRefresh.Location = new System.Drawing.Point(34, 19);
			this.checkRefresh.Name = "checkRefresh";
			this.checkRefresh.Size = new System.Drawing.Size(403, 17);
			this.checkRefresh.TabIndex = 202;
			this.checkRefresh.Text = "New computers default to ‘Refresh while typing’ in Select Patient window";
			// 
			// checkPrefFName
			// 
			this.checkPrefFName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrefFName.Location = new System.Drawing.Point(55, 36);
			this.checkPrefFName.Name = "checkPrefFName";
			this.checkPrefFName.Size = new System.Drawing.Size(382, 17);
			this.checkPrefFName.TabIndex = 79;
			this.checkPrefFName.Text = "Search for preferred name in first name field in Select Patient window";
			// 
			// groupBox6
			// 
			this.groupBox6.BackColor = System.Drawing.Color.White;
			this.groupBox6.Controls.Add(this.checkTitleBarShowSpecialty);
			this.groupBox6.Controls.Add(this.checkUseClinicAbbr);
			this.groupBox6.Controls.Add(this.textMainWindowTitle);
			this.groupBox6.Controls.Add(this.checkTitleBarShowSite);
			this.groupBox6.Controls.Add(this.label15);
			this.groupBox6.Controls.Add(this.comboShowID);
			this.groupBox6.Controls.Add(this.label17);
			this.groupBox6.Location = new System.Drawing.Point(20, 40);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(450, 119);
			this.groupBox6.TabIndex = 245;
			this.groupBox6.Text = "Main Window Title";
			// 
			// checkTitleBarShowSpecialty
			// 
			this.checkTitleBarShowSpecialty.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTitleBarShowSpecialty.Location = new System.Drawing.Point(6, 62);
			this.checkTitleBarShowSpecialty.Name = "checkTitleBarShowSpecialty";
			this.checkTitleBarShowSpecialty.Size = new System.Drawing.Size(431, 17);
			this.checkTitleBarShowSpecialty.TabIndex = 234;
			this.checkTitleBarShowSpecialty.Text = "Show patient specialty in main title bar and Account, Select Patient area";
			// 
			// checkUseClinicAbbr
			// 
			this.checkUseClinicAbbr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseClinicAbbr.Location = new System.Drawing.Point(6, 96);
			this.checkUseClinicAbbr.Name = "checkUseClinicAbbr";
			this.checkUseClinicAbbr.Size = new System.Drawing.Size(431, 17);
			this.checkUseClinicAbbr.TabIndex = 233;
			this.checkUseClinicAbbr.Text = "Use clinic abbreviation in main title bar (clinics must be turned on)";
			// 
			// textMainWindowTitle
			// 
			this.textMainWindowTitle.Location = new System.Drawing.Point(170, 13);
			this.textMainWindowTitle.Name = "textMainWindowTitle";
			this.textMainWindowTitle.Size = new System.Drawing.Size(267, 20);
			this.textMainWindowTitle.TabIndex = 38;
			// 
			// checkTitleBarShowSite
			// 
			this.checkTitleBarShowSite.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTitleBarShowSite.Location = new System.Drawing.Point(6, 79);
			this.checkTitleBarShowSite.Name = "checkTitleBarShowSite";
			this.checkTitleBarShowSite.Size = new System.Drawing.Size(431, 17);
			this.checkTitleBarShowSite.TabIndex = 74;
			this.checkTitleBarShowSite.Text = "Show Site (public health must also be turned on)";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(6, 14);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(163, 17);
			this.label15.TabIndex = 39;
			this.label15.Text = "Title Text";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboShowID
			// 
			this.comboShowID.Location = new System.Drawing.Point(307, 37);
			this.comboShowID.Name = "comboShowID";
			this.comboShowID.Size = new System.Drawing.Size(130, 21);
			this.comboShowID.TabIndex = 72;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(6, 39);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(299, 17);
			this.label17.TabIndex = 73;
			this.label17.Text = "Show ID in title bar";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// UserControlMainWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxPopups);
			this.Controls.Add(this.groupSelectPatient);
			this.Controls.Add(this.groupBox6);
			this.Name = "UserControlMainWindow";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupBoxPopups.ResumeLayout(false);
			this.groupBoxPopups.PerformLayout();
			this.groupSelectPatient.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GroupBox groupBoxPopups;
		private System.Windows.Forms.TextBox textPopupsDisableTimeSpan;
		private System.Windows.Forms.Label labelTimeSpan;
		private System.Windows.Forms.TextBox textPopupsDisableDays;
		private System.Windows.Forms.Label labelDays;
		private UI.GroupBox groupSelectPatient;
		private UI.CheckBox checkEnterpriseAllowRefreshWhileTyping;
		private UI.CheckBox checkShowInactivePatientsDefault;
		private UI.CheckBox checkPatientSelectFilterRestrictedClinics;
		private UI.CheckBox checkRefresh;
		private UI.CheckBox checkPrefFName;
		private UI.GroupBox groupBox6;
		private UI.CheckBox checkTitleBarShowSpecialty;
		private UI.CheckBox checkUseClinicAbbr;
		private System.Windows.Forms.TextBox textMainWindowTitle;
		private UI.CheckBox checkTitleBarShowSite;
		private System.Windows.Forms.Label label15;
		private UI.ComboBox comboShowID;
		private System.Windows.Forms.Label label17;
	}
}
