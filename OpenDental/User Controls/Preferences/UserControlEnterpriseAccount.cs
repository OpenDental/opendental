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
	public partial class UserControlEnterpriseAccount:UserControl {

		#region Fields - Private
		private int _claimReportReceiveInterval;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlEnterpriseAccount() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void radioInterval_CheckedChanged(object sender,EventArgs e) {
			//Copied from FormClearingHouses
			if(radioReceiveAtAnInterval.Checked) {
				labelReportheckUnits.Enabled=true;
				textClaimReportReceiveInterval.Enabled=true;
				textClaimReportReceiveTime.Text="";
				textClaimReportReceiveTime.Enabled=false;
				textClaimReportReceiveTime.ClearError();
			}
			else {
				labelReportheckUnits.Enabled=false;
				textClaimReportReceiveInterval.Text="";
				textClaimReportReceiveInterval.Enabled=false;
				textClaimReportReceiveTime.Enabled=true;
			}
		}

		private void butReplacements_Click(object sender,EventArgs e) {
			//Copied from FormModuleSetup.
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			FrmMessageReplacements frmMessageReplcements=new FrmMessageReplacements(listMessageReplaceTypes);
			frmMessageReplcements.IsSelectionMode=true;
			frmMessageReplcements.ShowDialog();
			if(frmMessageReplcements.IsDialogCancel) {
				return;
			}
			textClaimIdPrefix.Focus();
			int cursorIndex=textClaimIdPrefix.SelectionStart;
			textClaimIdPrefix.Text=textClaimIdPrefix.Text.Insert(cursorIndex,frmMessageReplcements.ReplacementTextSelected);
			textClaimIdPrefix.SelectionStart=cursorIndex+frmMessageReplcements.ReplacementTextSelected.Length;
		}

		#region Methods - Event Handlers Sync
		private void comboPaymentClinicSetting_ChangeCommitted(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentClinicSetting);
			prefValSync.PrefVal=POut.Int(comboPaymentClinicSetting.SelectedIndex);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkPaymentsPromptForPayType_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentsPromptForPayType);
			prefValSync.PrefVal=POut.Bool(checkPaymentsPromptForPayType.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void textClaimIdPrefix_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimIdPrefix);
			prefValSync.PrefVal=textClaimIdPrefix.Text;
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void comboPayPlansVersion_ChangeCommitted(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PayPlansVersion);
			prefValSync.PrefVal=POut.Int((int)comboPayPlansVersion.GetSelected<PayPlanVersions>());
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void textBillingElectBatchMax_Validating(object sender,CancelEventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.BillingElectBatchMax);
			if(!textBillingElectBatchMax.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			prefValSync.PrefVal=POut.Int(textBillingElectBatchMax.Value);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		private void checkBillingShowSendProgress_Click(object sender,EventArgs e) {
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.BillingShowSendProgress);
			prefValSync.PrefVal=POut.Bool(checkBillingShowSendProgress.Checked);
			SyncChanged?.Invoke(this,new EventArgs());
		}

		#endregion Methods - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		private bool ValidateEntries() {
			string errorMsg="";
			//SecurityLogOffAfterMinutes
			//ClaimReportReceiveInterval
			int reportCheckIntervalMinuteCount=0;
			reportCheckIntervalMinuteCount=PIn.Int(textClaimReportReceiveInterval.Text,false);
			if(textClaimReportReceiveInterval.Enabled && (reportCheckIntervalMinuteCount<5 || reportCheckIntervalMinuteCount>60)) {
				errorMsg+="Report check interval must be between 5 and 60 inclusive.\r\n";
			}
			//ClaimReportReceiveTime
			if(radioReceiveAtASetTime.Checked && (textClaimReportReceiveTime.Text=="" || !textClaimReportReceiveTime.IsValid())) {
				errorMsg+="Please enter a time to receive reports.";
			}
			//BillingElectBatchMax
			if(!textBillingElectBatchMax.IsValid()) {
				errorMsg+="The maximum number of statements per batch must be a valid number or blank.\r\n";
			}
			if(errorMsg!="") {
				MsgBox.Show(this,"Please fix the following errors:\r\n"+errorMsg);
				return false;
			}
			return true;
		}

		///<summary>Load values from database for hidden preferences if they exist.  If a pref doesn't exist then the corresponding UI is hidden.</summary>
		private void FillHiddenPrefs() {
			FillOptionalPrefBool(checkAgingReportShowAgePatPayplanPayments,PrefName.AgingReportShowAgePatPayplanPayments);
			//validDateAgingServiceTimeDue.Text=PrefC.GetDateT(PrefName.AgingServiceTimeDue).ToShortTimeString();
		}

		///<summary>Helper method for setting UI for boolean preferences.  Some of the preferences calling this may not exist in the database.</summary>
		private void FillOptionalPrefBool(UI.CheckBox checkBox,PrefName prefName) {
			string valueString=GetHiddenPrefString(prefName);
			if(valueString==null) {
				checkBox.Visible=false;
				return;
			}
			checkBox.Checked=PIn.Bool(valueString);
		}

		///<summary>Returns the ValueString of a pref or null if that pref is not found in the database.</summary>
		private string GetHiddenPrefString(PrefName prefName) {
			Pref prefHidden=null;
			try {
				prefHidden=Prefs.GetOne(prefName);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return null;
			}
			return prefHidden.ValueString;
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillEnterpriseAccount() {
			try {
				FillHiddenPrefs();
			}
			catch(Exception ex) {
				ex.DoNothing();//Suppress unhandled exceptions from hidden preferences, since they are read only.
			}
			checkAgingCalculateOnBatchClaimReceipt.Checked=PrefC.GetBool(PrefName.AgingCalculateOnBatchClaimReceipt);
			comboPaymentClinicSetting.Items.AddEnums<PayClinicSetting>();
			//comboPaymentClinicSetting.SelectedIndex=PrefC.GetInt(PrefName.PaymentClinicSetting);
			//checkPaymentsPromptForPayType.Checked=PrefC.GetBool(PrefName.PaymentsPromptForPayType);
			checkBillingShowTransSinceBalZero.Checked=PrefC.GetBool(PrefName.BillingShowTransSinceBalZero);
			//textClaimIdPrefix.Text=PrefC.GetString(PrefName.ClaimIdPrefix);
			checkClaimReportReceivedByService.Checked=PrefC.GetBool(PrefName.ClaimReportReceivedByService);
			_claimReportReceiveInterval=PrefC.GetInt(PrefName.ClaimReportReceiveInterval);
			if(_claimReportReceiveInterval==0) {
				radioReceiveAtASetTime.Checked=true;
				DateTime dateClaimReportReceiveTime=PrefC.GetDateT(PrefName.ClaimReportReceiveTime);
				textClaimReportReceiveTime.Text=dateClaimReportReceiveTime.ToShortTimeString();
			}
			else {
				textClaimReportReceiveInterval.Text=POut.Int(_claimReportReceiveInterval);
				radioReceiveAtAnInterval.Checked=true;
			}
			List<RigorousAccounting> listRigorousAccountings=Enum.GetValues(typeof(RigorousAccounting)).OfType<RigorousAccounting>().ToList();
			for(int i=0;i<listRigorousAccountings.Count;i++) {
				comboRigorousAccounting.Items.Add(listRigorousAccountings[i].GetDescription());
			}
			comboRigorousAccounting.SelectedIndex=PrefC.GetInt(PrefName.RigorousAccounting);
			List<RigorousAdjustments> listRigorousAdjustments=Enum.GetValues(typeof(RigorousAdjustments)).OfType<RigorousAdjustments>().ToList();
			for(int i=0;i<listRigorousAdjustments.Count;i++) {
				comboRigorousAdjustments.Items.Add(listRigorousAdjustments[i].GetDescription());
			}
			comboRigorousAdjustments.SelectedIndex=PrefC.GetInt(PrefName.RigorousAdjustments);
			checkPaymentWindowDefaultHideSplits.Checked=PrefC.GetBool(PrefName.PaymentWindowDefaultHideSplits);
			comboPayPlansVersion.Items.AddEnums<PayPlanVersions>();
			//comboPayPlansVersion.SetSelectedEnum(PrefC.GetInt(PrefName.PayPlansVersion));
			//textBillingElectBatchMax.Text=PrefC.GetInt(PrefName.BillingElectBatchMax).ToString();
			//checkBillingShowSendProgress.Checked=PrefC.GetBool(PrefName.BillingShowSendProgress);
		}

		public bool SaveEnterpriseAccount() {
			if(!ValidateEntries()) {
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.AgingCalculateOnBatchClaimReceipt,checkAgingCalculateOnBatchClaimReceipt.Checked);
			//(synced) Changed|=Prefs.UpdateInt(PrefName.PaymentClinicSetting,comboPaymentClinicSetting.SelectedIndex);
			//(synced) Changed|= Prefs.UpdateBool(PrefName.PaymentsPromptForPayType,checkPaymentsPromptForPayType.Checked);
			Changed|= Prefs.UpdateBool(PrefName.BillingShowTransSinceBalZero,checkBillingShowTransSinceBalZero.Checked);
			//Changed|=Prefs.UpdateString(PrefName.ClaimIdPrefix,textClaimIdPrefix.Text);
			Changed|=Prefs.UpdateBool(PrefName.ClaimReportReceivedByService,checkClaimReportReceivedByService.Checked);
			Changed|=Prefs.UpdateDateT(PrefName.ClaimReportReceiveTime,PIn.DateT(textClaimReportReceiveTime.Text));
			Changed|=Prefs.UpdateInt(PrefName.ClaimReportReceiveInterval,PIn.Int(textClaimReportReceiveInterval.Text));
			int prefRigorousAccounting=PrefC.GetInt(PrefName.RigorousAccounting);
			//Copied logging for RigorousAccounting and RigorousAdjustments from FormModuleSetup.
			if(Prefs.UpdateInt(PrefName.RigorousAccounting,comboRigorousAccounting.SelectedIndex)) {
				Changed=true;
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Rigorous accounting changed from "+
					((RigorousAccounting)prefRigorousAccounting).GetDescription()+" to "
					+((RigorousAccounting)comboRigorousAccounting.SelectedIndex).GetDescription()+".");
			}
			int prefRigorousAdjustments=PrefC.GetInt(PrefName.RigorousAdjustments);
			if(Prefs.UpdateInt(PrefName.RigorousAdjustments,comboRigorousAdjustments.SelectedIndex)) {
				Changed=true;
				SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Rigorous adjustments changed from "+
					((RigorousAdjustments)prefRigorousAdjustments).GetDescription()+" to "
					+((RigorousAdjustments)comboRigorousAdjustments.SelectedIndex).GetDescription()+".");
			}
			Changed|=Prefs.UpdateBool(PrefName.PaymentWindowDefaultHideSplits,checkPaymentWindowDefaultHideSplits.Checked);
			//(synced) Changed|=Prefs.UpdateInt(PrefName.PayPlansVersion,(int)comboPayPlansVersion.GetSelected<PayPlanVersions>());
			//Changed|=Prefs.UpdateInt(PrefName.BillingElectBatchMax,PIn.Int(textBillingElectBatchMax.Text));
			//Changed|=Prefs.UpdateBool(PrefName.BillingShowSendProgress,checkBillingShowSendProgress.Checked);
			return true;
		}

		public void FillSynced(){
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.AgingServiceTimeDue);
			textAgingServiceTimeDue.Text=PIn.DateT(prefValSync.PrefVal).ToShortTimeString();
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentClinicSetting);
			comboPaymentClinicSetting.SelectedIndex=PIn.Int(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentsPromptForPayType);
			checkPaymentsPromptForPayType.Checked=PIn.Bool(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.ClaimIdPrefix);
			textClaimIdPrefix.Text=prefValSync.PrefVal;
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PayPlansVersion);
			comboPayPlansVersion.SetSelectedEnum(PIn.Int(prefValSync.PrefVal));
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.BillingElectBatchMax);
			textBillingElectBatchMax.Value=PIn.Int(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.BillingShowSendProgress);
			checkBillingShowSendProgress.Checked=PIn.Bool(prefValSync.PrefVal);
		}
		#endregion Methods - Public
	}
}
