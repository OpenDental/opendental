using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	///<summary></summary>
	public partial class FormMisc : FormODBase {
		private List<string> _trackLastClinicBy;
		//private List<Def> posAdjTypes;

		///<summary></summary>
		public FormMisc(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMisc_Load(object sender, System.EventArgs e) {
			if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0){
				textSigInterval.Text="";
			}
			else{
				textSigInterval.Text=PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs).ToString();
			}
			if(PrefC.GetLong(PrefName.SignalInactiveMinutes)==0) {
				textInactiveSignal.Text="";
			}
			else {
				textInactiveSignal.Text=PrefC.GetLong(PrefName.SignalInactiveMinutes).ToString();
			}
			checkTimeCardUseLocal.Checked=PrefC.GetBool(PrefName.LocalTimeOverridesServerTime);
			checkRefresh.Checked=!PrefC.GetBool(PrefName.PatientSelectUsesSearchButton);
			checkPrefFName.Checked=PrefC.GetBool(PrefName.PatientSelectUseFNameForPreferred);
			checkAllowRefreshWhileTyping.Checked=PrefC.GetBool(PrefName.EnterpriseAllowRefreshWhileTyping);
			if(PrefC.HasClinicsEnabled){
				checkPatientSelectFilterRestrictedClinics.Visible=true;
				checkPatientSelectFilterRestrictedClinics.Checked=PrefC.GetBool(PrefName.PatientSelectFilterRestrictedClinics);
			}
			checkShowInactivePatientsDefault.Checked=PrefC.GetBool(PrefName.PatientSelectShowInactive);
			textMainWindowTitle.Text=PrefC.GetString(PrefName.MainWindowTitle);
			checkUseClinicAbbr.Checked=PrefC.GetBool(PrefName.TitleBarClinicUseAbbr);
			checkTitleBarShowSpecialty.Checked=PrefC.GetBool(PrefName.TitleBarShowSpecialty);
			comboShowID.Items.Add(Lan.g(this,"None"));
			comboShowID.Items.Add(Lan.g(this,"PatNum"));
			comboShowID.Items.Add(Lan.g(this,"ChartNumber"));
			comboShowID.Items.Add(Lan.g(this,"Birthdate"));
			comboShowID.SelectedIndex=PrefC.GetInt(PrefName.ShowIDinTitleBar);
			checkImeCompositionCompatibility.Checked=PrefC.GetBool(PrefName.ImeCompositionCompatibility);
			checkTitleBarShowSite.Checked=PrefC.GetBool(PrefName.TitleBarShowSite);
			textWebServiceServerName.Text=PrefC.GetString(PrefName.WebServiceServerName);
			if(PrefC.GetLong(PrefName.AlertCheckFrequencySeconds)==0) {
				textAlertInterval.Text="";
			}
			else {
				textAlertInterval.Text=PrefC.GetString(PrefName.AlertCheckFrequencySeconds);
			}
			if(PrefC.GetLong(PrefName.AlertInactiveMinutes)==0) {
				textInactiveAlert.Text="";
			}
			else {
				textInactiveAlert.Text=PrefC.GetString(PrefName.AlertInactiveMinutes);
			}
			if(PrefC.GetString(PrefName.LanguageAndRegion)!="") {
				textLanguageAndRegion.Text=PrefC.GetLanguageAndRegion().DisplayName;
			}
			else {
				textLanguageAndRegion.Text=Lan.g(this,"None");
			}
			textNumDecimals.Text=CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits.ToString();
			_trackLastClinicBy=new List<string> { "None","Workstation","User" };//must be in english because these values are stored in DB.
			_trackLastClinicBy.ForEach(x => comboTrackClinic.Items.Add(Lan.g(this,x)));//translation is for display only.
			comboTrackClinic.SelectedIndex=_trackLastClinicBy.FindIndex(x => x==PrefC.GetString(PrefName.ClinicTrackLast));
			if(comboTrackClinic.SelectedIndex==-1) {
				comboTrackClinic.SelectedIndex=0;
			}
			if(!PrefC.HasClinicsEnabled) {
				labelTrackClinic.Visible=false;
				comboTrackClinic.Visible=false;
				checkUseClinicAbbr.Visible=false;
			}
			textSyncCode.Text=PrefC.GetString(PrefName.CentralManagerSyncCode);
			textAuditEntries.Text=PrefC.GetString(PrefName.AuditTrailEntriesDisplayed);
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				//Enterprise
				checkAgingIsEnterprise.Checked=true;
				DateTime agingBeginDateT=PrefC.GetDateT(PrefName.AgingBeginDateTime);
				if(agingBeginDateT>DateTime.MinValue) {
					textAgingBeginDateT.Text=agingBeginDateT.ToString();
				}
			}
			else {
				//Classic
				labelAgingBeginDateT.Visible=false;
				textAgingBeginDateT.Visible=false;
				butClearAgingBeginDateT.Visible=false;
			}
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
				textAutoAgingRunTime.Enabled=false;
			}
			else {
				DateTime agingDateTDue=PrefC.GetDateT(PrefName.AgingServiceTimeDue);
				if(agingDateTDue!=DateTime.MinValue) {
					textAutoAgingRunTime.Text=agingDateTDue.ToShortTimeString();
				}
			}
			checkSubmitExceptions.Checked=PrefC.GetBool(PrefName.SendUnhandledExceptionsToHQ);
			if(PrefC.IsCloudMode) {
				textWebServiceServerName.ReadOnly=true;
			}
		}

		private void butLanguages_Click(object sender,EventArgs e) {
			using FormLanguagesUsed FormL=new FormLanguagesUsed();
			FormL.ShowDialog();
			if(FormL.DialogResult==DialogResult.OK){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void butPickLanguageAndRegion_Click(object sender,EventArgs e) {
			using FormLanguageAndRegion FormLAR=new FormLanguageAndRegion();//FormLanguageAndRegion saves pref to DB.
			FormLAR.ShowDialog();
			if(PrefC.GetString(PrefName.LanguageAndRegion)!="") {
				textLanguageAndRegion.Text=PrefC.GetLanguageAndRegion().DisplayName;
			}
			else {
				textLanguageAndRegion.Text=Lan.g(this,"None");
			}
		}

		private void butDecimal_Click(object sender,EventArgs e) {
			using FormDecimalSettings FormDS=new FormDecimalSettings();
			FormDS.ShowDialog();
		}

		private void butClearCode_Click(object sender,EventArgs e) {
			textSyncCode.Text="";
		}

		private void checkUseFamAgingTable_Click(object sender,EventArgs e) {
			if(checkAgingIsEnterprise.Checked && !PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				//Enabling feature that is turned off according to pref table.
				string msgTxt=Lan.g(this,"In order to enable enterprise aging, enter the password from our manual below.");
				if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
					//Conditional warning that only affects Monthly Aging customers.
					msgTxt+="\r\n"+Lan.g(this,"Note: This will change your aging from calculated monthly to calculated daily.");
				}
				using InputBox iBox=new InputBox(msgTxt) { Text=Lan.g(this,"Enter Password") };
				iBox.ShowDialog();
				if(iBox.DialogResult!=DialogResult.OK) {
					checkAgingIsEnterprise.Checked=false;
					return;
				}
				if(iBox.textResult.Text!="abracadabra"){//to keep non-enterprise customers from unwittingly enabling this feature 
					checkAgingIsEnterprise.Checked=false;
					MsgBox.Show(this,"Incorrect password.  Refer to the online manual or contact Open Dental support for assistance.");
					return;
				}
			}
		}
		
		///<summary>This button and the associated label and textbox are only visible if AgingIsEnterprise is set in the database.  When the user first
		///enables the AgingIsEnterprise feature, there will be no need to clear the DateTime since it will already be blank.  Once the form closes and
		///AgingIsEnterprise is enabled, the next time the form is accessed they may have the option to clear the DateTime.  Requires SecurityAdmin permission.</summary>
		private void butClearAgingBeginDateT_Click(object sender,EventArgs e) {
			//user doesn't have permission or the pref is already cleared
			if(!Security.IsAuthorized(Permissions.SecurityAdmin) || PrefC.GetDateT(PrefName.AgingBeginDateTime)==DateTime.MinValue) {//blank=DateTime.MinValue
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will override the lock on the famaging table, potentially allowing a second connection to start "
				+"the aging calculations which could cause both calculations to fail.  If this happens, this flag will need to be cleared again and aging "
				+"started by a single connection.\r\nAre you sure you want to clear this value?"))
			{
				return;
			}
			textAgingBeginDateT.Text="";
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textAuditEntries.IsValid() || !textAlertInterval.IsValid() || !textInactiveAlert.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(string.IsNullOrWhiteSpace(textSigInterval.Text) && PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)!=0) {
				bool proceed=MsgBox.Show(sender,MsgBoxButtons.YesNo,"Disabling the process signal interval prevents the use of kiosks.\r\n"
					+"This should not be done if there are multiple workstations in the office.\r\n"
					+"Proceed?");
				if (!proceed) {
					textSigInterval.Text=PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs).ToString();
					return;
				}
			}
			if(PIn.Long(textSigInterval.Text)>=(5+(PIn.Long(textInactiveSignal.Text)*60)) && PIn.Long(textInactiveSignal.Text)!=0) {//Signal Refresh time is less than or equal to 5 seconds plus the number of seconds in textSigInterval
				string question=Lans.g(this,"The inactive signal time is less than or equal to the signal refresh time.")+"\r\n"
					+Lans.g(this,"This could inadvertently cause signals to not correctly refresh.  Continue?");
				if(MessageBox.Show(question,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
			}
			DateTime autoAgingRunTime=DateTime.MinValue;
			if(!string.IsNullOrWhiteSpace(textAutoAgingRunTime.Text)
				&& !PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)
				&& !DateTime.TryParse(textAutoAgingRunTime.Text,out autoAgingRunTime))
			{
				MsgBox.Show(this,"Automated Aging Run Time must be blank or a valid time of day.");
				return;
			}
			if(comboTrackClinic.SelectedIndex<0) {
				comboTrackClinic.SelectedIndex=0;
			}
			bool changed=false;
			if( Prefs.UpdateString(PrefName.MainWindowTitle,textMainWindowTitle.Text)
				| Prefs.UpdateLong(PrefName.ShowIDinTitleBar,comboShowID.SelectedIndex)
				| Prefs.UpdateBool(PrefName.TitleBarShowSite, checkTitleBarShowSite.Checked)
				| Prefs.UpdateString(PrefName.WebServiceServerName,textWebServiceServerName.Text)
				| Prefs.UpdateBool(PrefName.LocalTimeOverridesServerTime,checkTimeCardUseLocal.Checked)
				| Prefs.UpdateBool(PrefName.PatientSelectUseFNameForPreferred,checkPrefFName.Checked)
				| Prefs.UpdateBool(PrefName.PatientSelectUsesSearchButton,!checkRefresh.Checked)
				| Prefs.UpdateBool(PrefName.ImeCompositionCompatibility,checkImeCompositionCompatibility.Checked)
				| Prefs.UpdateString(PrefName.ClinicTrackLast,_trackLastClinicBy[comboTrackClinic.SelectedIndex])
				| Prefs.UpdateString(PrefName.CentralManagerSyncCode,textSyncCode.Text)
				| Prefs.UpdateString(PrefName.AuditTrailEntriesDisplayed,textAuditEntries.Text)
				| Prefs.UpdateBool(PrefName.AgingIsEnterprise,checkAgingIsEnterprise.Checked)
				| Prefs.UpdateBool(PrefName.TitleBarClinicUseAbbr,checkUseClinicAbbr.Checked)
				| Prefs.UpdateBool(PrefName.TitleBarShowSpecialty,checkTitleBarShowSpecialty.Checked)
				| Prefs.UpdateBool(PrefName.SendUnhandledExceptionsToHQ,checkSubmitExceptions.Checked)
				| Prefs.UpdateBool(PrefName.PatientSelectFilterRestrictedClinics,checkPatientSelectFilterRestrictedClinics.Checked)
				| Prefs.UpdateBool(PrefName.PatientSelectShowInactive,checkShowInactivePatientsDefault.Checked)
				| Prefs.UpdateLong(PrefName.CloudAlertWithinLimit,PIn.Long(textAlertCloudSessions.Text))
				| Prefs.UpdateBool(PrefName.EnterpriseAllowRefreshWhileTyping,checkAllowRefreshWhileTyping.Checked)
			)
			{
				changed=true;
			}
			if(textSigInterval.Text==""){
				if(Prefs.UpdateLong(PrefName.ProcessSigsIntervalInSecs,0)){
					changed=true;
				}
			}
			else{
				if(Prefs.UpdateLong(PrefName.ProcessSigsIntervalInSecs,PIn.Long(textSigInterval.Text))){
					changed=true;
				}
			}
			if(textInactiveSignal.Text=="") {
				if(Prefs.UpdateLong(PrefName.SignalInactiveMinutes,0)) {
					changed=true;
				}
			}
			else {
				if(Prefs.UpdateLong(PrefName.SignalInactiveMinutes,PIn.Long(textInactiveSignal.Text))) {
					changed=true;
				}
			}
			if(textAlertInterval.Text=="") {
				changed|=Prefs.UpdateLong(PrefName.AlertCheckFrequencySeconds,0);
			}
			else {
				changed|=Prefs.UpdateLong(PrefName.AlertCheckFrequencySeconds,PIn.Long(textAlertInterval.Text));
			}
			if(textInactiveAlert.Text=="") {
				changed|=Prefs.UpdateLong(PrefName.AlertInactiveMinutes,0);
			}
			else {
				changed|=Prefs.UpdateLong(PrefName.AlertInactiveMinutes,PIn.Long(textInactiveAlert.Text));
			}
			//if AgingIsEnterprise, change the aging to daily aging if it is currently set to monthly.  AgingIsEnterprise requires daily aging.
			if(checkAgingIsEnterprise.Checked && Prefs.UpdateBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily,false)) {
				changed=true;
			}
			if(autoAgingRunTime==DateTime.MinValue) {
				if(Prefs.UpdateString(PrefName.AgingServiceTimeDue,"")) {
					changed=true;
				}
			}
			else {
				if(Prefs.UpdateString(PrefName.AgingServiceTimeDue,POut.DateT(autoAgingRunTime,false))) {
					changed=true;
				}
			}
			if(changed){
				//ComputerPrefs may not need to be invalidated here, since task computer settings moved to FormTaskSetup.  Leaving here for now just in case.
				DataValid.SetInvalid(InvalidType.Prefs, InvalidType.Computers);
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





