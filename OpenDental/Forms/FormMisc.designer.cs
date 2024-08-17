﻿using System;
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
			this.comboTrackClinic = new OpenDental.UI.ComboBox();
			this.labelTrackClinic = new System.Windows.Forms.Label();
			this.checkTimeCardUseLocal = new OpenDental.UI.CheckBox();
			this.butPickLanguageAndRegion = new OpenDental.UI.Button();
			this.textLanguageAndRegion = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.checkImeCompositionCompatibility = new OpenDental.UI.CheckBox();
			this.checkRefresh = new OpenDental.UI.CheckBox();
			this.checkPrefFName = new OpenDental.UI.CheckBox();
			this.textInactiveSignal = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.textWebServiceServerName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox6 = new OpenDental.UI.GroupBox();
			this.checkTitleBarShowSpecialty = new OpenDental.UI.CheckBox();
			this.checkUseClinicAbbr = new OpenDental.UI.CheckBox();
			this.textMainWindowTitle = new System.Windows.Forms.TextBox();
			this.checkTitleBarShowSite = new OpenDental.UI.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.comboShowID = new OpenDental.UI.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.groupSelectPatient = new OpenDental.UI.GroupBox();
			this.checkAllowRefreshWhileTyping = new OpenDental.UI.CheckBox();
			this.checkShowInactivePatientsDefault = new OpenDental.UI.CheckBox();
			this.checkPatientSelectFilterRestrictedClinics = new OpenDental.UI.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butLanguages = new OpenDental.UI.Button();
			this.textSigInterval = new OpenDental.ValidNum();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.checkSubmitExceptions = new OpenDental.UI.CheckBox();
			this.textInactiveAlert = new OpenDental.ValidNum();
			this.label10 = new System.Windows.Forms.Label();
			this.textAlertInterval = new OpenDental.ValidNum();
			this.label11 = new System.Windows.Forms.Label();
			this.textAlertCloudSessions = new OpenDental.ValidNum();
			this.labelAlertCloudSessions = new System.Windows.Forms.Label();
			this.checkAuditTrailUseReportingServer = new OpenDental.UI.CheckBox();
			this.groupBoxPopups = new OpenDental.UI.GroupBox();
			this.textPopupsDisableTimeSpan = new System.Windows.Forms.TextBox();
			this.labelTimeSpan = new System.Windows.Forms.Label();
			this.textPopupsDisableDays = new System.Windows.Forms.TextBox();
			this.labelDays = new System.Windows.Forms.Label();
			this.groupBox6.SuspendLayout();
			this.groupSelectPatient.SuspendLayout();
			this.groupBoxPopups.SuspendLayout();
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
			this.textAuditEntries.Location = new System.Drawing.Point(899, 337);
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
			this.label9.Location = new System.Drawing.Point(547, 338);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(350, 17);
			this.label9.TabIndex = 224;
			this.label9.Text = "Number of Audit Trail entries displayed";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClearCode
			// 
			this.butClearCode.Location = new System.Drawing.Point(979, 312);
			this.butClearCode.Name = "butClearCode";
			this.butClearCode.Size = new System.Drawing.Size(43, 21);
			this.butClearCode.TabIndex = 222;
			this.butClearCode.Text = "Clear";
			this.butClearCode.Click += new System.EventHandler(this.butClearCode_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(547, 313);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(350, 17);
			this.label8.TabIndex = 221;
			this.label8.Text = "Sync code for CEMT.  Clear if you want to sync from a different source";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSyncCode
			// 
			this.textSyncCode.Location = new System.Drawing.Point(899, 312);
			this.textSyncCode.Name = "textSyncCode";
			this.textSyncCode.ReadOnly = true;
			this.textSyncCode.Size = new System.Drawing.Size(74, 20);
			this.textSyncCode.TabIndex = 220;
			// 
			// butDecimal
			// 
			this.butDecimal.Location = new System.Drawing.Point(455, 343);
			this.butDecimal.Name = "butDecimal";
			this.butDecimal.Size = new System.Drawing.Size(23, 21);
			this.butDecimal.TabIndex = 219;
			this.butDecimal.Text = "...";
			this.butDecimal.Click += new System.EventHandler(this.butDecimal_Click);
			// 
			// textNumDecimals
			// 
			this.textNumDecimals.Location = new System.Drawing.Point(402, 342);
			this.textNumDecimals.Name = "textNumDecimals";
			this.textNumDecimals.ReadOnly = true;
			this.textNumDecimals.Size = new System.Drawing.Size(47, 20);
			this.textNumDecimals.TabIndex = 217;
			this.textNumDecimals.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(36, 342);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(364, 17);
			this.label7.TabIndex = 218;
			this.label7.Text = "Currency number of digits after decimal";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTrackClinic
			// 
			this.comboTrackClinic.Location = new System.Drawing.Point(843, 286);
			this.comboTrackClinic.Name = "comboTrackClinic";
			this.comboTrackClinic.Size = new System.Drawing.Size(130, 21);
			this.comboTrackClinic.TabIndex = 215;
			// 
			// labelTrackClinic
			// 
			this.labelTrackClinic.Location = new System.Drawing.Point(547, 289);
			this.labelTrackClinic.Name = "labelTrackClinic";
			this.labelTrackClinic.Size = new System.Drawing.Size(294, 17);
			this.labelTrackClinic.TabIndex = 216;
			this.labelTrackClinic.Text = "Track Last Clinic By";
			this.labelTrackClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkTimeCardUseLocal
			// 
			this.checkTimeCardUseLocal.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeCardUseLocal.Location = new System.Drawing.Point(6, 147);
			this.checkTimeCardUseLocal.Name = "checkTimeCardUseLocal";
			this.checkTimeCardUseLocal.Size = new System.Drawing.Size(443, 17);
			this.checkTimeCardUseLocal.TabIndex = 214;
			this.checkTimeCardUseLocal.Text = "Time Cards use local time";
			// 
			// butPickLanguageAndRegion
			// 
			this.butPickLanguageAndRegion.Location = new System.Drawing.Point(455, 316);
			this.butPickLanguageAndRegion.Name = "butPickLanguageAndRegion";
			this.butPickLanguageAndRegion.Size = new System.Drawing.Size(23, 21);
			this.butPickLanguageAndRegion.TabIndex = 206;
			this.butPickLanguageAndRegion.Text = "...";
			this.butPickLanguageAndRegion.Click += new System.EventHandler(this.butPickLanguageAndRegion_Click);
			// 
			// textLanguageAndRegion
			// 
			this.textLanguageAndRegion.Location = new System.Drawing.Point(284, 317);
			this.textLanguageAndRegion.Name = "textLanguageAndRegion";
			this.textLanguageAndRegion.ReadOnly = true;
			this.textLanguageAndRegion.Size = new System.Drawing.Size(165, 20);
			this.textLanguageAndRegion.TabIndex = 204;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(36, 318);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(246, 17);
			this.label6.TabIndex = 205;
			this.label6.Text = "Language and region used by program";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkImeCompositionCompatibility
			// 
			this.checkImeCompositionCompatibility.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkImeCompositionCompatibility.Location = new System.Drawing.Point(53, 367);
			this.checkImeCompositionCompatibility.Name = "checkImeCompositionCompatibility";
			this.checkImeCompositionCompatibility.Size = new System.Drawing.Size(396, 17);
			this.checkImeCompositionCompatibility.TabIndex = 203;
			this.checkImeCompositionCompatibility.Text = "Text boxes use foreign language Input Method Editor (IME) composition";
			// 
			// checkRefresh
			// 
			this.checkRefresh.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRefresh.Location = new System.Drawing.Point(55, 15);
			this.checkRefresh.Name = "checkRefresh";
			this.checkRefresh.Size = new System.Drawing.Size(382, 17);
			this.checkRefresh.TabIndex = 202;
			this.checkRefresh.Text = "New Computers default to refresh while typing in Select Patient window";
			// 
			// checkPrefFName
			// 
			this.checkPrefFName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPrefFName.Location = new System.Drawing.Point(55, 32);
			this.checkPrefFName.Name = "checkPrefFName";
			this.checkPrefFName.Size = new System.Drawing.Size(382, 17);
			this.checkPrefFName.TabIndex = 79;
			this.checkPrefFName.Text = "Search for preferred name in first name field in Select Patient window";
			// 
			// textInactiveSignal
			// 
			this.textInactiveSignal.Location = new System.Drawing.Point(375, 200);
			this.textInactiveSignal.MaxVal = 1000000;
			this.textInactiveSignal.MinVal = 1;
			this.textInactiveSignal.Name = "textInactiveSignal";
			this.textInactiveSignal.Size = new System.Drawing.Size(74, 20);
			this.textInactiveSignal.TabIndex = 201;
			this.textInactiveSignal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(36, 196);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(337, 27);
			this.label5.TabIndex = 200;
			this.label5.Text = "Disable signal interval after this many minutes of user inactivity\r\nLeave blank t" +
    "o disable";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWebServiceServerName
			// 
			this.textWebServiceServerName.Location = new System.Drawing.Point(808, 244);
			this.textWebServiceServerName.Name = "textWebServiceServerName";
			this.textWebServiceServerName.Size = new System.Drawing.Size(165, 20);
			this.textWebServiceServerName.TabIndex = 197;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(508, 236);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(298, 42);
			this.label2.TabIndex = 198;
			this.label2.Text = "Update Server (pref.WebServiceServerName): Version updates can only be performed " +
    "from this computer, and the eConnector can only be installed on this computer.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(453, 17);
			this.label1.TabIndex = 196;
			this.label1.Text = "See Setup | Preferences for setup options that were previously in this window.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.groupBox6.Location = new System.Drawing.Point(12, 24);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(453, 119);
			this.groupBox6.TabIndex = 195;
			this.groupBox6.Text = "Main Window Title";
			// 
			// checkTitleBarShowSpecialty
			// 
			this.checkTitleBarShowSpecialty.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTitleBarShowSpecialty.Location = new System.Drawing.Point(6, 62);
			this.checkTitleBarShowSpecialty.Name = "checkTitleBarShowSpecialty";
			this.checkTitleBarShowSpecialty.Size = new System.Drawing.Size(431, 17);
			this.checkTitleBarShowSpecialty.TabIndex = 234;
			this.checkTitleBarShowSpecialty.Text = "Show patient specialty in main title bar and account patient select";
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
			// groupSelectPatient
			// 
			this.groupSelectPatient.BackColor = System.Drawing.Color.White;
			this.groupSelectPatient.Controls.Add(this.checkAllowRefreshWhileTyping);
			this.groupSelectPatient.Controls.Add(this.checkShowInactivePatientsDefault);
			this.groupSelectPatient.Controls.Add(this.checkPatientSelectFilterRestrictedClinics);
			this.groupSelectPatient.Controls.Add(this.checkRefresh);
			this.groupSelectPatient.Controls.Add(this.checkPrefFName);
			this.groupSelectPatient.Location = new System.Drawing.Point(536, 24);
			this.groupSelectPatient.Name = "groupSelectPatient";
			this.groupSelectPatient.Size = new System.Drawing.Size(453, 106);
			this.groupSelectPatient.TabIndex = 235;
			this.groupSelectPatient.Text = "Select Patient Window";
			// 
			// checkAllowRefreshWhileTyping
			// 
			this.checkAllowRefreshWhileTyping.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowRefreshWhileTyping.Location = new System.Drawing.Point(55, 83);
			this.checkAllowRefreshWhileTyping.Name = "checkAllowRefreshWhileTyping";
			this.checkAllowRefreshWhileTyping.Size = new System.Drawing.Size(382, 18);
			this.checkAllowRefreshWhileTyping.TabIndex = 293;
			this.checkAllowRefreshWhileTyping.Text = "Allow Refresh while typing in Select Patient window";
			// 
			// checkShowInactivePatientsDefault
			// 
			this.checkShowInactivePatientsDefault.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowInactivePatientsDefault.Location = new System.Drawing.Point(55, 66);
			this.checkShowInactivePatientsDefault.Name = "checkShowInactivePatientsDefault";
			this.checkShowInactivePatientsDefault.Size = new System.Drawing.Size(382, 18);
			this.checkShowInactivePatientsDefault.TabIndex = 292;
			this.checkShowInactivePatientsDefault.Text = "Show Inactive Patients by default";
			// 
			// checkPatientSelectFilterRestrictedClinics
			// 
			this.checkPatientSelectFilterRestrictedClinics.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPatientSelectFilterRestrictedClinics.Location = new System.Drawing.Point(55, 49);
			this.checkPatientSelectFilterRestrictedClinics.Name = "checkPatientSelectFilterRestrictedClinics";
			this.checkPatientSelectFilterRestrictedClinics.Size = new System.Drawing.Size(382, 18);
			this.checkPatientSelectFilterRestrictedClinics.TabIndex = 291;
			this.checkPatientSelectFilterRestrictedClinics.Text = "Hide patients from restricted clinics when viewing \"All\" clinics";
			this.checkPatientSelectFilterRestrictedClinics.Visible = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(36, 290);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(322, 17);
			this.label4.TabIndex = 64;
			this.label4.Text = "Languages used by patients";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butLanguages
			// 
			this.butLanguages.Location = new System.Drawing.Point(360, 287);
			this.butLanguages.Name = "butLanguages";
			this.butLanguages.Size = new System.Drawing.Size(88, 24);
			this.butLanguages.TabIndex = 63;
			this.butLanguages.Text = "Edit Languages";
			this.butLanguages.Click += new System.EventHandler(this.butLanguages_Click);
			// 
			// textSigInterval
			// 
			this.textSigInterval.Location = new System.Drawing.Point(375, 170);
			this.textSigInterval.MaxVal = 1000000;
			this.textSigInterval.MinVal = 1;
			this.textSigInterval.Name = "textSigInterval";
			this.textSigInterval.ShowZero = false;
			this.textSigInterval.Size = new System.Drawing.Size(74, 20);
			this.textSigInterval.TabIndex = 57;
			this.textSigInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(968, 493);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(887, 493);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(33, 167);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(340, 27);
			this.label3.TabIndex = 56;
			this.label3.Text = "Process signal interval in seconds.  Usually every 6 to 20 seconds\r\nLeave blank t" +
    "o disable autorefresh";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSubmitExceptions
			// 
			this.checkSubmitExceptions.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSubmitExceptions.Location = new System.Drawing.Point(547, 380);
			this.checkSubmitExceptions.Name = "checkSubmitExceptions";
			this.checkSubmitExceptions.Size = new System.Drawing.Size(426, 17);
			this.checkSubmitExceptions.TabIndex = 232;
			this.checkSubmitExceptions.Text = "Automatically submit unhandled exceptions";
			// 
			// textInactiveAlert
			// 
			this.textInactiveAlert.Location = new System.Drawing.Point(375, 260);
			this.textInactiveAlert.MaxVal = 1000000;
			this.textInactiveAlert.MinVal = 1;
			this.textInactiveAlert.Name = "textInactiveAlert";
			this.textInactiveAlert.ShowZero = false;
			this.textInactiveAlert.Size = new System.Drawing.Size(74, 20);
			this.textInactiveAlert.TabIndex = 240;
			this.textInactiveAlert.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(36, 256);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(337, 27);
			this.label10.TabIndex = 239;
			this.label10.Text = "Disable alert interval after this many minutes of user inactivity\r\nLeave blank to" +
    " disable";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAlertInterval
			// 
			this.textAlertInterval.Location = new System.Drawing.Point(375, 230);
			this.textAlertInterval.MaxVal = 1000000;
			this.textAlertInterval.MinVal = 1;
			this.textAlertInterval.Name = "textAlertInterval";
			this.textAlertInterval.ShowZero = false;
			this.textAlertInterval.Size = new System.Drawing.Size(74, 20);
			this.textAlertInterval.TabIndex = 238;
			this.textAlertInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(36, 227);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(337, 27);
			this.label11.TabIndex = 237;
			this.label11.Text = "Check alert interval in seconds.  Leave blank to disable";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAlertCloudSessions
			// 
			this.textAlertCloudSessions.Location = new System.Drawing.Point(898, 209);
			this.textAlertCloudSessions.Name = "textAlertCloudSessions";
			this.textAlertCloudSessions.ShowZero = false;
			this.textAlertCloudSessions.Size = new System.Drawing.Size(74, 20);
			this.textAlertCloudSessions.TabIndex = 241;
			// 
			// labelAlertCloudSessions
			// 
			this.labelAlertCloudSessions.Location = new System.Drawing.Point(547, 211);
			this.labelAlertCloudSessions.Name = "labelAlertCloudSessions";
			this.labelAlertCloudSessions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelAlertCloudSessions.Size = new System.Drawing.Size(350, 23);
			this.labelAlertCloudSessions.TabIndex = 242;
			this.labelAlertCloudSessions.Text = "Alert when within this value of the maximum allowed Cloud Sessions";
			// 
			// checkAuditTrailUseReportingServer
			// 
			this.checkAuditTrailUseReportingServer.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAuditTrailUseReportingServer.Location = new System.Drawing.Point(744, 357);
			this.checkAuditTrailUseReportingServer.Name = "checkAuditTrailUseReportingServer";
			this.checkAuditTrailUseReportingServer.Size = new System.Drawing.Size(229, 24);
			this.checkAuditTrailUseReportingServer.TabIndex = 243;
			this.checkAuditTrailUseReportingServer.Text = "Audit Trail uses Reporting Server";
			// 
			// groupBoxPopups
			// 
			this.groupBoxPopups.BackColor = System.Drawing.Color.White;
			this.groupBoxPopups.Controls.Add(this.textPopupsDisableTimeSpan);
			this.groupBoxPopups.Controls.Add(this.labelTimeSpan);
			this.groupBoxPopups.Controls.Add(this.textPopupsDisableDays);
			this.groupBoxPopups.Controls.Add(this.labelDays);
			this.groupBoxPopups.Location = new System.Drawing.Point(536, 136);
			this.groupBoxPopups.Name = "groupBoxPopups";
			this.groupBoxPopups.Size = new System.Drawing.Size(453, 66);
			this.groupBoxPopups.TabIndex = 244;
			this.groupBoxPopups.Text = "Popups Disable Timespan";
			// 
			// textTimeSpan
			// 
			this.textPopupsDisableTimeSpan.Location = new System.Drawing.Point(362, 38);
			this.textPopupsDisableTimeSpan.Name = "textTimeSpan";
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
			// textDays
			// 
			this.textPopupsDisableDays.Location = new System.Drawing.Point(390, 12);
			this.textPopupsDisableDays.Name = "textDays";
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
			// FormMisc
			// 
			this.ClientSize = new System.Drawing.Size(1055, 523);
			this.Controls.Add(this.groupBoxPopups);
			this.Controls.Add(this.checkAuditTrailUseReportingServer);
			this.Controls.Add(this.textInactiveAlert);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textAlertInterval);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.groupSelectPatient);
			this.Controls.Add(this.checkSubmitExceptions);
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
			this.groupBoxPopups.ResumeLayout(false);
			this.groupBoxPopups.PerformLayout();
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
		private OpenDental.UI.ComboBox comboShowID;
		private Label label15;
		private Label label17;
		private OpenDental.UI.GroupBox groupBox6;
		private OpenDental.UI.CheckBox checkTitleBarShowSite;
		private TextBox textWebServiceServerName;
		private Label label2;
		private ValidNum textInactiveSignal;
		private Label label5;
		private OpenDental.UI.CheckBox checkPrefFName;
		private OpenDental.UI.CheckBox checkRefresh;
		private OpenDental.UI.CheckBox checkImeCompositionCompatibility;
		private TextBox textLanguageAndRegion;
		private Label label6;
		private UI.Button butPickLanguageAndRegion;
    private OpenDental.UI.CheckBox checkTimeCardUseLocal;
		private OpenDental.UI.ComboBox comboTrackClinic;
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
		private OpenDental.UI.CheckBox checkUseClinicAbbr;
		private OpenDental.UI.CheckBox checkSubmitExceptions;
		private OpenDental.UI.CheckBox checkTitleBarShowSpecialty;
		private OpenDental.UI.GroupBox groupSelectPatient;
		private OpenDental.UI.CheckBox checkPatientSelectFilterRestrictedClinics;
		private OpenDental.UI.CheckBox checkShowInactivePatientsDefault;
		private ValidNum textInactiveAlert;
		private Label label10;
		private ValidNum textAlertInterval;
		private Label label11;
		private ValidNum textAlertCloudSessions;
		private Label labelAlertCloudSessions;
		private OpenDental.UI.CheckBox checkAllowRefreshWhileTyping;
		private OpenDental.UI.CheckBox checkAuditTrailUseReportingServer;
		private UI.GroupBox groupBoxPopups;
		private TextBox textPopupsDisableTimeSpan;
		private Label labelTimeSpan;
		private TextBox textPopupsDisableDays;
		private Label labelDays;
	}
}
