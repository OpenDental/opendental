using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlMainWindowMisc:UserControl {

		#region Fields - Private
		private List<string> _listTrackLastClinicBys;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlMainWindowMisc() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void butLanguages_Click(object sender,EventArgs e) {
			using FormLanguagesUsed formLanguagesUsed=new FormLanguagesUsed();
			formLanguagesUsed.ShowDialog();
			if(formLanguagesUsed.DialogResult==DialogResult.OK){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void butPickLanguageAndRegion_Click(object sender,EventArgs e) {
			using FormLanguageAndRegion formLanguageAndRegion=new FormLanguageAndRegion();//FormLanguageAndRegion saves pref to DB.
			formLanguageAndRegion.ShowDialog();
			if(PrefC.GetString(PrefName.LanguageAndRegion)!="") {
				textLanguageAndRegion.Text=PrefC.GetLanguageAndRegion().DisplayName;
			}
			else {
				textLanguageAndRegion.Text=Lan.g(this,"None");
			}
		}

		private void butDecimal_Click(object sender,EventArgs e) {
			FrmDecimalSettings frmDecimalSettings=new FrmDecimalSettings();
			frmDecimalSettings.ShowDialog();
		}

		private void butClearCode_Click(object sender,EventArgs e) {
			textSyncCode.Text="";
		}
		#endregion Methods - Event Handlers

		#region Methods - Event Handlers Sync
		private void textSignalInactiveMinutes_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.SignalInactiveMinutes);
			if(!textSignalInactiveMinutes.IsValid()) {
				string errorMsg="Disable signal interval must be a valid number or blank.\r\n";
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return;
			}
			prefValSync.PrefVal=POut.Int(textSignalInactiveMinutes.Value);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void textProcessSigsIntervalInSecs_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ProcessSigsIntervalInSecs);
			if(!textProcessSigsIntervalInSecs.IsValid()) {
				string errorMsg="Signal interval must be a valid number or blank.\r\n";
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return;
			}
			prefValSync.PrefVal=POut.Int(textProcessSigsIntervalInSecs.Value);
			SyncChanged?.Invoke(this,new EventArgs());
		}
		#endregion Methods - Event Handlers Sync

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillMainWindowMisc() {
			//if(PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)==0){
			//	textProcessSigsIntervalInSecs.Text="";
			//}
			//else{
			//	textProcessSigsIntervalInSecs.Text=PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs).ToString();
			//}
			//if(PrefC.GetLong(PrefName.SignalInactiveMinutes)==0) {
			//	textSignalInactiveMinutes.Text="";
			//}
			//else {
			//	textSignalInactiveMinutes.Text=PrefC.GetLong(PrefName.SignalInactiveMinutes).ToString();
			//}
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
			checkImeCompositionCompatibility.Checked=PrefC.GetBool(PrefName.ImeCompositionCompatibility);
			textSyncCode.Text=PrefC.GetString(PrefName.CentralManagerSyncCode);
			textNumDecimals.Text=CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits.ToString();
			textWebServiceServerName.Text=PrefC.GetString(PrefName.WebServiceServerName);
			textAlertCloudSessions.Text=PrefC.GetString(PrefName.CloudAlertWithinLimit);
			_listTrackLastClinicBys=new List<string> { "None","Workstation","User" };//must be in english because these values are stored in DB.
			for(int i=0;i<_listTrackLastClinicBys.Count;i++) {
				comboTrackClinic.Items.Add(Lan.g(this,_listTrackLastClinicBys[i]));//translation is for display only.
			}
			comboTrackClinic.SelectedIndex=_listTrackLastClinicBys.FindIndex(x => x==PrefC.GetString(PrefName.ClinicTrackLast));
			if(comboTrackClinic.SelectedIndex==-1) {
				comboTrackClinic.SelectedIndex=0;
			}
			if(!PrefC.HasClinicsEnabled) {
				labelTrackClinic.Visible=false;
				comboTrackClinic.Visible=false;
			}
			checkSubmitExceptions.Checked=PrefC.GetBool(PrefName.SendUnhandledExceptionsToHQ);
			textAuditEntries.Text=PrefC.GetString(PrefName.AuditTrailEntriesDisplayed);
			//if(PrefC.GetString(PrefName.ReportingServerCompName)=="" && PrefC.GetString(PrefName.ReportingServerURI)=="") {
			//	checkAuditTrailUseReportingServer.Visible=false;
			//}
			checkAuditTrailUseReportingServer.Checked=PrefC.GetBool(PrefName.AuditTrailUseReportingServer);
		}

		public bool SaveMainWindowMisc() {
			if(!textProcessSigsIntervalInSecs.IsValid() || !textSignalInactiveMinutes.IsValid() || !textAlertInterval.IsValid() || !textInactiveAlert.IsValid() || !textAlertCloudSessions.IsValid() || !textAuditEntries.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(string.IsNullOrWhiteSpace(textProcessSigsIntervalInSecs.Text) && PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs)!=0) {
				bool proceed=MsgBox.Show(MsgBoxButtons.YesNo,"Disabling the process signal interval prevents the use of kiosks.\r\n"
					+"This should not be done if there are multiple workstations in the office.\r\n"
					+"Proceed?");
				if (!proceed) {
					textProcessSigsIntervalInSecs.Text=PrefC.GetLong(PrefName.ProcessSigsIntervalInSecs).ToString();
					return false;
				}
			}
			if(PIn.Long(textProcessSigsIntervalInSecs.Text)>=(5+(PIn.Long(textSignalInactiveMinutes.Text)*60)) && PIn.Long(textSignalInactiveMinutes.Text)!=0) {//Signal Refresh time is less than or equal to 5 seconds plus the number of seconds in textSigInterval
				string question=Lans.g(this,"The inactive signal time is less than or equal to the signal refresh time.")+"\r\n"
					+Lans.g(this,"This could inadvertently cause signals to not correctly refresh.  Continue?");
				if(MessageBox.Show(question,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return false;
				}
			}
			if(comboTrackClinic.SelectedIndex<0) {
				comboTrackClinic.SelectedIndex=0;
			}
			bool hasChanged=false;
			//if(textProcessSigsIntervalInSecs.Text==""){
			//	hasChanged |=Prefs.UpdateLong(PrefName.ProcessSigsIntervalInSecs,0);
			//}
			//else{
			//	hasChanged |=Prefs.UpdateLong(PrefName.ProcessSigsIntervalInSecs,PIn.Long(textProcessSigsIntervalInSecs.Text));
			//}
			//if(textSignalInactiveMinutes.Text=="") {
			//	hasChanged |=Prefs.UpdateLong(PrefName.SignalInactiveMinutes,0);
			//}
			//else {
			//	hasChanged |=Prefs.UpdateLong(PrefName.SignalInactiveMinutes,PIn.Long(textSignalInactiveMinutes.Text));
			//}
			if(textAlertInterval.Text=="") {
				hasChanged|=Prefs.UpdateLong(PrefName.AlertCheckFrequencySeconds,0);
			}
			else {
				hasChanged|=Prefs.UpdateLong(PrefName.AlertCheckFrequencySeconds,PIn.Long(textAlertInterval.Text));
			}
			if(textInactiveAlert.Text=="") {
				hasChanged|=Prefs.UpdateLong(PrefName.AlertInactiveMinutes,0);
			}
			else {
				hasChanged|=Prefs.UpdateLong(PrefName.AlertInactiveMinutes,PIn.Long(textInactiveAlert.Text));
			}
			hasChanged |=Prefs.UpdateBool(PrefName.ImeCompositionCompatibility,checkImeCompositionCompatibility.Checked);
			hasChanged |=Prefs.UpdateString(PrefName.CentralManagerSyncCode,textSyncCode.Text);
			hasChanged |=Prefs.UpdateString(PrefName.WebServiceServerName,textWebServiceServerName.Text);
			hasChanged |=Prefs.UpdateLong(PrefName.CloudAlertWithinLimit,PIn.Long(textAlertCloudSessions.Text));
			hasChanged |=Prefs.UpdateString(PrefName.ClinicTrackLast,_listTrackLastClinicBys[comboTrackClinic.SelectedIndex]);
			hasChanged |=Prefs.UpdateBool(PrefName.SendUnhandledExceptionsToHQ,checkSubmitExceptions.Checked);
			hasChanged |=Prefs.UpdateString(PrefName.AuditTrailEntriesDisplayed,textAuditEntries.Text);
			if(checkAuditTrailUseReportingServer.Visible) {
				hasChanged |=Prefs.UpdateBool(PrefName.AuditTrailUseReportingServer,checkAuditTrailUseReportingServer.Checked);
			}
			if(hasChanged){
				//ComputerPrefs may not need to be invalidated here, since task computer settings moved to FormTaskSetup.  Leaving here for now just in case.
				DataValid.SetInvalid(InvalidType.Prefs, InvalidType.Computers);
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			return true;
		}

		public void FillSynced(){
			//This will revert invalid Values back to the PrefVal if other prefs are updated before invalid Values are fixed
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.SignalInactiveMinutes);
			textSignalInactiveMinutes.Value=PIn.Int(prefValSync.PrefVal);//0 shows as empty
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ProcessSigsIntervalInSecs);
			textProcessSigsIntervalInSecs.Value=PIn.Int(prefValSync.PrefVal);//0 shows as empty
			PrefValSync prefValSyncCompName=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerCompName);
			PrefValSync prefValSyncURI=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ReportingServerURI);
			if(prefValSyncCompName.PrefVal=="" && prefValSyncURI.PrefVal=="") {
				checkAuditTrailUseReportingServer.Visible=false;
			}
			else{
				checkAuditTrailUseReportingServer.Visible=true;
			}
		}
		#endregion Methods - Public

		
	}
}
