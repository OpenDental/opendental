using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMisc {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMisc));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textAuditEntries = new OpenDental.ValidNum();
			this.label9 = new System.Windows.Forms.Label();
			this.butClearCode = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.textSyncCode = new System.Windows.Forms.TextBox();
			this.butDecimal = new OpenDental.UI.Button();
			this.textNumDecimals = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.comboTrackClinic = new System.Windows.Forms.ComboBox();
			this.labelTrackClinic = new System.Windows.Forms.Label();
			this.checkTimeCardUseLocal = new System.Windows.Forms.CheckBox();
			this.butPickLanguageAndRegion = new OpenDental.UI.Button();
			this.textLanguageAndRegion = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.checkImeCompositionCompatibility = new System.Windows.Forms.CheckBox();
			this.checkRefresh = new System.Windows.Forms.CheckBox();
			this.checkPrefFName = new System.Windows.Forms.CheckBox();
			this.textInactiveSignal = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.textWebServiceServerName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.checkTitleBarShowSpecialty = new System.Windows.Forms.CheckBox();
			this.checkUseClinicAbbr = new System.Windows.Forms.CheckBox();
			this.textMainWindowTitle = new System.Windows.Forms.TextBox();
			this.checkTitleBarShowSite = new System.Windows.Forms.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.comboShowID = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupSelectPatient = new System.Windows.Forms.GroupBox();
			this.checkShowInactivePatientsDefault = new System.Windows.Forms.CheckBox();
			this.checkPatientSelectFilterRestrictedClinics = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butLanguages = new OpenDental.UI.Button();
			this.textSigInterval = new OpenDental.ValidNum();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.checkAgingIsEnterprise = new System.Windows.Forms.CheckBox();
			this.butClearAgingBeginDateT = new OpenDental.UI.Button();
			this.labelAgingBeginDateT = new System.Windows.Forms.Label();
			this.textAgingBeginDateT = new System.Windows.Forms.TextBox();
			this.labelAutoAgingRunTime = new System.Windows.Forms.Label();
			this.textAutoAgingRunTime = new System.Windows.Forms.TextBox();
			this.checkSubmitExceptions = new System.Windows.Forms.CheckBox();
			this.textInactiveAlert = new OpenDental.ValidNum();
			this.label10 = new System.Windows.Forms.Label();
			this.textAlertInterval = new OpenDental.ValidNum();
			this.label11 = new System.Windows.Forms.Label();
			this.textAlertCloudSessions = new OpenDental.ValidNum();
			this.labelAlertCloudSessions = new System.Windows.Forms.Label();
			this.checkAllowRefreshWhileTyping = new System.Windows.Forms.CheckBox();
			this.groupBox6.SuspendLayout();
			this.groupSelectPatient.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 0;
			this.toolTip1.AutoPopDelay = 600000;
			this.toolTip1.InitialDelay = 0;
			this.toolTip1.IsBalloon = true;
			this.toolTip1.ReshowDelay = 0;
			this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.toolTip1.ToolTipTitle = "Help";
			// 
			// textAuditEntries
			// 
			this.textAuditEntries.Location = new System.Drawing.Point(375, 464);
			this.textAuditEntries.MaxLength = 5;
			this.textAuditEntries.MaxVal = 10000;
			this.textAuditEntries.MinVal = 1;
			this.textAuditEntries.Name = "textAuditEntries";
			this.textAuditEntries.Size = new System.Drawing.Size(74, 20);
			this.textAuditEntries.TabIndex = 225;
			this.textAuditEntries.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 465);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(367, 17);
			this.label9.TabIndex = 224;
			this.label9.Text = "Number of Audit Trail entries displayed";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClearCode
			// 
			this.butClearCode.Location = new System.Drawing.Point(455, 441);
			this.butClearCode.Name = "butClearCode";
			this.butClearCode.Size = new System.Drawing.Size(43, 21);
			this.butClearCode.TabIndex = 222;
			this.butClearCode.Text = "Clear";
			this.butClearCode.Click += new System.EventHandler(this.butClearCode_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 442);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(367, 17);
			this.label8.TabIndex = 221;
			this.label8.Text = "Sync code for CEMT.  Clear if you want to sync from a different source";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSyncCode
			// 
			this.textSyncCode.Location = new System.Drawing.Point(375, 441);
			this.textSyncCode.Name = "textSyncCode";
			this.textSyncCode.ReadOnly = true;
			this.textSyncCode.Size = new System.Drawing.Size(74, 20);
			this.textSyncCode.TabIndex = 220;
			// 
			// butDecimal
			// 
			this.butDecimal.Location = new System.Drawing.Point(455, 329);
			this.butDecimal.Name = "butDecimal";
			this.butDecimal.Size = new System.Drawing.Size(23, 21);
			this.butDecimal.TabIndex = 219;
			this.butDecimal.Text = "...";
			this.butDecimal.Click += new System.EventHandler(this.butDecimal_Click);
			// 
			// textNumDecimals
			// 
			this.textNumDecimals.Location = new System.Drawing.Point(402, 328);
			this.textNumDecimals.Name = "textNumDecimals";
			this.textNumDecimals.ReadOnly = true;
			this.textNumDecimals.Size = new System.Drawing.Size(47, 20);
			this.textNumDecimals.TabIndex = 217;
			this.textNumDecimals.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 328);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(394, 17);
			this.label7.TabIndex = 218;
			this.label7.Text = "Currency number of digits after decimal";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTrackClinic
			// 
			this.comboTrackClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboTrackClinic.FormattingEnabled = true;
			this.comboTrackClinic.Location = new System.Drawing.Point(319, 416);
			this.comboTrackClinic.Name = "comboTrackClinic";
			this.comboTrackClinic.Size = new System.Drawing.Size(130, 21);
			this.comboTrackClinic.TabIndex = 215;
			// 
			// labelTrackClinic
			// 
			this.labelTrackClinic.Location = new System.Drawing.Point(6, 419);
			this.labelTrackClinic.Name = "labelTrackClinic";
			this.labelTrackClinic.Size = new System.Drawing.Size(311, 17);
			this.labelTrackClinic.TabIndex = 216;
			this.labelTrackClinic.Text = "Track Last Clinic By";
			this.labelTrackClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTimeCardUseLocal
			// 
			this.checkTimeCardUseLocal.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeCardUseLocal.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTimeCardUseLocal.Location = new System.Drawing.Point(6, 139);
			this.checkTimeCardUseLocal.Name = "checkTimeCardUseLocal";
			this.checkTimeCardUseLocal.Size = new System.Drawing.Size(443, 17);
			this.checkTimeCardUseLocal.TabIndex = 214;
			this.checkTimeCardUseLocal.Text = "Time Cards use local time";
			this.checkTimeCardUseLocal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickLanguageAndRegion
			// 
			this.butPickLanguageAndRegion.Location = new System.Drawing.Point(455, 304);
			this.butPickLanguageAndRegion.Name = "butPickLanguageAndRegion";
			this.butPickLanguageAndRegion.Size = new System.Drawing.Size(23, 21);
			this.butPickLanguageAndRegion.TabIndex = 206;
			this.butPickLanguageAndRegion.Text = "...";
			this.butPickLanguageAndRegion.Click += new System.EventHandler(this.butPickLanguageAndRegion_Click);
			// 
			// textLanguageAndRegion
			// 
			this.textLanguageAndRegion.Location = new System.Drawing.Point(284, 305);
			this.textLanguageAndRegion.Name = "textLanguageAndRegion";
			this.textLanguageAndRegion.ReadOnly = true;
			this.textLanguageAndRegion.Size = new System.Drawing.Size(165, 20);
			this.textLanguageAndRegion.TabIndex = 204;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 303);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(276, 17);
			this.label6.TabIndex = 205;
			this.label6.Text = "Language and region used by program";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkImeCompositionCompatibility
			// 
			this.checkImeCompositionCompatibility.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkImeCompositionCompatibility.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkImeCompositionCompatibility.Location = new System.Drawing.Point(6, 375);
			this.checkImeCompositionCompatibility.Name = "checkImeCompositionCompatibility";
			this.checkImeCompositionCompatibility.Size = new System.Drawing.Size(443, 17);
			this.checkImeCompositionCompatibility.TabIndex = 203;
			this.checkImeCompositionCompatibility.Text = "Text boxes use foreign language Input Method Editor (IME) composition";
			this.checkImeCompositionCompatibility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRefresh
			// 
			this.checkRefresh.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRefresh.Location = new System.Drawing.Point(55, 13);
			this.checkRefresh.Name = "checkRefresh";
			this.checkRefresh.Size = new System.Drawing.Size(382, 17);
			this.checkRefresh.TabIndex = 202;
			this.checkRefresh.Text = "New Computers default to refresh while typing in Select Patient window";
			this.checkRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRefresh.UseVisualStyleBackColor = true;
			// 
			// checkPrefFName
			// 
			this.checkPrefFName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrefFName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPrefFName.Location = new System.Drawing.Point(55, 29);
			this.checkPrefFName.Name = "checkPrefFName";
			this.checkPrefFName.Size = new System.Drawing.Size(382, 17);
			this.checkPrefFName.TabIndex = 79;
			this.checkPrefFName.Text = "Search for preferred name in first name field in Select Patient window";
			this.checkPrefFName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInactiveSignal
			// 
			this.textInactiveSignal.Location = new System.Drawing.Point(375, 191);
			this.textInactiveSignal.MaxVal = 1000000;
			this.textInactiveSignal.MinVal = 1;
			this.textInactiveSignal.Name = "textInactiveSignal";
			this.textInactiveSignal.Size = new System.Drawing.Size(74, 20);
			this.textInactiveSignal.TabIndex = 201;
			this.textInactiveSignal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 187);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(367, 27);
			this.label5.TabIndex = 200;
			this.label5.Text = "Disable signal interval after this many minutes of user inactivity\r\nLeave blank t" +
    "o disable";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWebServiceServerName
			// 
			this.textWebServiceServerName.Location = new System.Drawing.Point(284, 394);
			this.textWebServiceServerName.Name = "textWebServiceServerName";
			this.textWebServiceServerName.Size = new System.Drawing.Size(165, 20);
			this.textWebServiceServerName.TabIndex = 197;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 395);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(276, 17);
			this.label2.TabIndex = 198;
			this.label2.Text = "Update Server Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(453, 17);
			this.label1.TabIndex = 196;
			this.label1.Text = "See Setup | Module Preferences for setup options that were previously in this win" +
    "dow.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.checkTitleBarShowSpecialty);
			this.groupBox6.Controls.Add(this.checkUseClinicAbbr);
			this.groupBox6.Controls.Add(this.textMainWindowTitle);
			this.groupBox6.Controls.Add(this.checkTitleBarShowSite);
			this.groupBox6.Controls.Add(this.label15);
			this.groupBox6.Controls.Add(this.comboShowID);
			this.groupBox6.Controls.Add(this.label17);
			this.groupBox6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox6.Location = new System.Drawing.Point(12, 24);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(453, 113);
			this.groupBox6.TabIndex = 195;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Main Window Title";
			// 
			// checkTitleBarShowSpecialty
			// 
			this.checkTitleBarShowSpecialty.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTitleBarShowSpecialty.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTitleBarShowSpecialty.Location = new System.Drawing.Point(6, 60);
			this.checkTitleBarShowSpecialty.Name = "checkTitleBarShowSpecialty";
			this.checkTitleBarShowSpecialty.Size = new System.Drawing.Size(431, 17);
			this.checkTitleBarShowSpecialty.TabIndex = 234;
			this.checkTitleBarShowSpecialty.Text = "Show patient specialty in main title bar and account patient select";
			this.checkTitleBarShowSpecialty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUseClinicAbbr
			// 
			this.checkUseClinicAbbr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseClinicAbbr.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseClinicAbbr.Location = new System.Drawing.Point(6, 94);
			this.checkUseClinicAbbr.Name = "checkUseClinicAbbr";
			this.checkUseClinicAbbr.Size = new System.Drawing.Size(431, 17);
			this.checkUseClinicAbbr.TabIndex = 233;
			this.checkUseClinicAbbr.Text = "Use clinic abbreviation in main title bar (clinics must be turned on)";
			this.checkUseClinicAbbr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkTitleBarShowSite.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTitleBarShowSite.Location = new System.Drawing.Point(6, 77);
			this.checkTitleBarShowSite.Name = "checkTitleBarShowSite";
			this.checkTitleBarShowSite.Size = new System.Drawing.Size(431, 17);
			this.checkTitleBarShowSite.TabIndex = 74;
			this.checkTitleBarShowSite.Text = "Show Site (public health must also be turned on)";
			this.checkTitleBarShowSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.comboShowID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboShowID.FormattingEnabled = true;
			this.comboShowID.Location = new System.Drawing.Point(307, 35);
			this.comboShowID.Name = "comboShowID";
			this.comboShowID.Size = new System.Drawing.Size(130, 21);
			this.comboShowID.TabIndex = 72;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(6, 37);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(299, 17);
			this.label17.TabIndex = 73;
			this.label17.Text = "Show ID in title bar";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupSelectPatient
			// 
			this.groupSelectPatient.Controls.Add(this.checkAllowRefreshWhileTyping);
			this.groupSelectPatient.Controls.Add(this.checkShowInactivePatientsDefault);
			this.groupSelectPatient.Controls.Add(this.checkPatientSelectFilterRestrictedClinics);
			this.groupSelectPatient.Controls.Add(this.checkRefresh);
			this.groupSelectPatient.Controls.Add(this.checkPrefFName);
			this.groupSelectPatient.Location = new System.Drawing.Point(12, 569);
			this.groupSelectPatient.Name = "groupSelectPatient";
			this.groupSelectPatient.Size = new System.Drawing.Size(453, 104);
			this.groupSelectPatient.TabIndex = 235;
			this.groupSelectPatient.TabStop = false;
			this.groupSelectPatient.Text = "Select Patient Window";
			// 
			// checkShowInactivePatientsDefault
			// 
			this.checkShowInactivePatientsDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowInactivePatientsDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowInactivePatientsDefault.Location = new System.Drawing.Point(55, 61);
			this.checkShowInactivePatientsDefault.Name = "checkShowInactivePatientsDefault";
			this.checkShowInactivePatientsDefault.Size = new System.Drawing.Size(382, 18);
			this.checkShowInactivePatientsDefault.TabIndex = 292;
			this.checkShowInactivePatientsDefault.Text = "Show Inactive Patients by default";
			this.checkShowInactivePatientsDefault.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkPatientSelectFilterRestrictedClinics
			// 
			this.checkPatientSelectFilterRestrictedClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectFilterRestrictedClinics.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientSelectFilterRestrictedClinics.Location = new System.Drawing.Point(55, 45);
			this.checkPatientSelectFilterRestrictedClinics.Name = "checkPatientSelectFilterRestrictedClinics";
			this.checkPatientSelectFilterRestrictedClinics.Size = new System.Drawing.Size(382, 18);
			this.checkPatientSelectFilterRestrictedClinics.TabIndex = 291;
			this.checkPatientSelectFilterRestrictedClinics.Text = "Hide patients from restricted clinics when viewing \"All\" clinics";
			this.checkPatientSelectFilterRestrictedClinics.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectFilterRestrictedClinics.Visible = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 282);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(352, 17);
			this.label4.TabIndex = 64;
			this.label4.Text = "Languages used by patients";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butLanguages
			// 
			this.butLanguages.Location = new System.Drawing.Point(360, 279);
			this.butLanguages.Name = "butLanguages";
			this.butLanguages.Size = new System.Drawing.Size(88, 24);
			this.butLanguages.TabIndex = 63;
			this.butLanguages.Text = "Edit Languages";
			this.butLanguages.Click += new System.EventHandler(this.butLanguages_Click);
			// 
			// textSigInterval
			// 
			this.textSigInterval.Location = new System.Drawing.Point(375, 161);
			this.textSigInterval.MaxVal = 1000000;
			this.textSigInterval.MinVal = 1;
			this.textSigInterval.Name = "textSigInterval";
			this.textSigInterval.Size = new System.Drawing.Size(74, 20);
			this.textSigInterval.TabIndex = 57;
			this.textSigInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textSigInterval.ShowZero = false;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(455, 679);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(374, 679);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 158);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(367, 27);
			this.label3.TabIndex = 56;
			this.label3.Text = "Process signal interval in seconds.  Usually every 6 to 20 seconds\r\nLeave blank t" +
    "o disable autorefresh";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAgingIsEnterprise
			// 
			this.checkAgingIsEnterprise.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingIsEnterprise.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAgingIsEnterprise.Location = new System.Drawing.Point(6, 509);
			this.checkAgingIsEnterprise.Name = "checkAgingIsEnterprise";
			this.checkAgingIsEnterprise.Size = new System.Drawing.Size(443, 17);
			this.checkAgingIsEnterprise.TabIndex = 226;
			this.checkAgingIsEnterprise.Text = "Aging for enterprise Galera cluster environments using FamAging table";
			this.checkAgingIsEnterprise.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAgingIsEnterprise.Click += new System.EventHandler(this.checkUseFamAgingTable_Click);
			// 
			// butClearAgingBeginDateT
			// 
			this.butClearAgingBeginDateT.Location = new System.Drawing.Point(455, 530);
			this.butClearAgingBeginDateT.Name = "butClearAgingBeginDateT";
			this.butClearAgingBeginDateT.Size = new System.Drawing.Size(43, 21);
			this.butClearAgingBeginDateT.TabIndex = 229;
			this.butClearAgingBeginDateT.Text = "Clear";
			this.butClearAgingBeginDateT.Click += new System.EventHandler(this.butClearAgingBeginDateT_Click);
			// 
			// labelAgingBeginDateT
			// 
			this.labelAgingBeginDateT.Location = new System.Drawing.Point(6, 527);
			this.labelAgingBeginDateT.Name = "labelAgingBeginDateT";
			this.labelAgingBeginDateT.Size = new System.Drawing.Size(291, 27);
			this.labelAgingBeginDateT.TabIndex = 228;
			this.labelAgingBeginDateT.Text = "DateTime the currently running aging started\r\nUsually blank";
			this.labelAgingBeginDateT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAgingBeginDateT
			// 
			this.textAgingBeginDateT.Location = new System.Drawing.Point(299, 530);
			this.textAgingBeginDateT.Name = "textAgingBeginDateT";
			this.textAgingBeginDateT.ReadOnly = true;
			this.textAgingBeginDateT.Size = new System.Drawing.Size(150, 20);
			this.textAgingBeginDateT.TabIndex = 227;
			this.textAgingBeginDateT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelAutoAgingRunTime
			// 
			this.labelAutoAgingRunTime.Location = new System.Drawing.Point(6, 484);
			this.labelAutoAgingRunTime.Name = "labelAutoAgingRunTime";
			this.labelAutoAgingRunTime.Size = new System.Drawing.Size(367, 27);
			this.labelAutoAgingRunTime.TabIndex = 231;
			this.labelAutoAgingRunTime.Text = "Automated aging run time, only applies if using Daily aging \r\nLeave blank to disa" +
    "ble";
			this.labelAutoAgingRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAutoAgingRunTime
			// 
			this.textAutoAgingRunTime.Location = new System.Drawing.Point(375, 487);
			this.textAutoAgingRunTime.Name = "textAutoAgingRunTime";
			this.textAutoAgingRunTime.Size = new System.Drawing.Size(74, 20);
			this.textAutoAgingRunTime.TabIndex = 230;
			this.textAutoAgingRunTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkSubmitExceptions
			// 
			this.checkSubmitExceptions.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSubmitExceptions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSubmitExceptions.Location = new System.Drawing.Point(6, 554);
			this.checkSubmitExceptions.Name = "checkSubmitExceptions";
			this.checkSubmitExceptions.Size = new System.Drawing.Size(443, 17);
			this.checkSubmitExceptions.TabIndex = 232;
			this.checkSubmitExceptions.Text = "Automatically submit unhandled exceptions";
			this.checkSubmitExceptions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInactiveAlert
			// 
			this.textInactiveAlert.Location = new System.Drawing.Point(375, 251);
			this.textInactiveAlert.MaxVal = 1000000;
			this.textInactiveAlert.MinVal = 1;
			this.textInactiveAlert.Name = "textInactiveAlert";
			this.textInactiveAlert.Size = new System.Drawing.Size(74, 20);
			this.textInactiveAlert.TabIndex = 240;
			this.textInactiveAlert.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textInactiveAlert.ShowZero = false;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 247);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(367, 27);
			this.label10.TabIndex = 239;
			this.label10.Text = "Disable alert interval after this many minutes of user inactivity\r\nLeave blank to" +
    " disable";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAlertInterval
			// 
			this.textAlertInterval.Location = new System.Drawing.Point(375, 221);
			this.textAlertInterval.MaxVal = 1000000;
			this.textAlertInterval.MinVal = 1;
			this.textAlertInterval.Name = "textAlertInterval";
			this.textAlertInterval.Size = new System.Drawing.Size(74, 20);
			this.textAlertInterval.TabIndex = 238;
			this.textAlertInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textAlertInterval.ShowZero = false;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 218);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(367, 27);
			this.label11.TabIndex = 237;
			this.label11.Text = "Check alert interval in seconds.  Leave blank to disable";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAlertCloudSessions
			// 
			this.textAlertCloudSessions.Location = new System.Drawing.Point(374, 353);
			this.textAlertCloudSessions.MaxVal = 255;
			this.textAlertCloudSessions.MinVal = 0;
			this.textAlertCloudSessions.Name = "textAlertCloudSessions";
			this.textAlertCloudSessions.Size = new System.Drawing.Size(74, 20);
			this.textAlertCloudSessions.TabIndex = 241;
			this.textAlertCloudSessions.ShowZero = false;
			// 
			// labelAlertCloudSessions
			// 
			this.labelAlertCloudSessions.Location = new System.Drawing.Point(6, 355);
			this.labelAlertCloudSessions.Name = "labelAlertCloudSessions";
			this.labelAlertCloudSessions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelAlertCloudSessions.Size = new System.Drawing.Size(367, 23);
			this.labelAlertCloudSessions.TabIndex = 242;
			this.labelAlertCloudSessions.Text = "Alert when within this value of the maximum allowed Cloud Sessions";
			// 
			// checkAllowRefreshWhileTyping
			// 
			this.checkAllowRefreshWhileTyping.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowRefreshWhileTyping.Location = new System.Drawing.Point(55, 77);
			this.checkAllowRefreshWhileTyping.Name = "checkAllowRefreshWhileTyping";
			this.checkAllowRefreshWhileTyping.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkAllowRefreshWhileTyping.Size = new System.Drawing.Size(382, 18);
			this.checkAllowRefreshWhileTyping.TabIndex = 293;
			this.checkAllowRefreshWhileTyping.Text = "Allow Refresh While Typing in Select Patient Window";
			this.checkAllowRefreshWhileTyping.UseVisualStyleBackColor = true;
			// 
			// FormMisc
			// 
			this.ClientSize = new System.Drawing.Size(542, 709);
			this.Controls.Add(this.textInactiveAlert);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textAlertInterval);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.groupSelectPatient);
			this.Controls.Add(this.checkSubmitExceptions);
			this.Controls.Add(this.butClearAgingBeginDateT);
			this.Controls.Add(this.labelAgingBeginDateT);
			this.Controls.Add(this.textAgingBeginDateT);
			this.Controls.Add(this.checkAgingIsEnterprise);
			this.Controls.Add(this.textAuditEntries);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.butClearCode);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textSyncCode);
			this.Controls.Add(this.butDecimal);
			this.Controls.Add(this.textNumDecimals);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.comboTrackClinic);
			this.Controls.Add(this.labelTrackClinic);
			this.Controls.Add(this.checkTimeCardUseLocal);
			this.Controls.Add(this.butPickLanguageAndRegion);
			this.Controls.Add(this.textLanguageAndRegion);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.checkImeCompositionCompatibility);
			this.Controls.Add(this.textInactiveSignal);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textWebServiceServerName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butLanguages);
			this.Controls.Add(this.textSigInterval);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelAutoAgingRunTime);
			this.Controls.Add(this.textAutoAgingRunTime);
			this.Controls.Add(this.labelAlertCloudSessions);
			this.Controls.Add(this.textAlertCloudSessions);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMisc";
			this.ShowInTaskbar = false;
			this.Text = "Miscellaneous Setup";
			this.Load += new System.EventHandler(this.FormMisc_Load);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupSelectPatient.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textMainWindowTitle;
		private System.Windows.Forms.Label label3;
		private OpenDental.ValidNum textSigInterval;
		private OpenDental.UI.Button butLanguages;
		private Label label4;
		private ToolTip toolTip1;
		private ComboBox comboShowID;
		private Label label15;
		private Label label17;
		private GroupBox groupBox6;
		private CheckBox checkTitleBarShowSite;
		private TextBox textWebServiceServerName;
		private Label label2;
		private ValidNum textInactiveSignal;
		private Label label5;
		private CheckBox checkPrefFName;
		private CheckBox checkRefresh;
		private CheckBox checkImeCompositionCompatibility;
		private TextBox textLanguageAndRegion;
		private Label label6;
		private UI.Button butPickLanguageAndRegion;
    private CheckBox checkTimeCardUseLocal;
		private ComboBox comboTrackClinic;
		private Label labelTrackClinic;
		private Label label1;
		private UI.Button butDecimal;
		private TextBox textNumDecimals;
		private Label label7;
		private TextBox textSyncCode;
		private Label label8;
		private UI.Button butClearCode;
		private Label label9;
		private ValidNum textAuditEntries;
		private CheckBox checkAgingIsEnterprise;
		private UI.Button butClearAgingBeginDateT;
		private Label labelAgingBeginDateT;
		private TextBox textAgingBeginDateT;
		private Label labelAutoAgingRunTime;
		private TextBox textAutoAgingRunTime;
		private CheckBox checkUseClinicAbbr;
		private CheckBox checkSubmitExceptions;
		private CheckBox checkTitleBarShowSpecialty;
		private GroupBox groupSelectPatient;
		private CheckBox checkPatientSelectFilterRestrictedClinics;
		private CheckBox checkShowInactivePatientsDefault;
		private ValidNum textInactiveAlert;
		private Label label10;
		private ValidNum textAlertInterval;
		private Label label11;
		private ValidNum textAlertCloudSessions;
		private Label labelAlertCloudSessions;
		private CheckBox checkAllowRefreshWhileTyping;
	}
}
