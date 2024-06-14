using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlAccountPayments:UserControl {

		#region Fields - Private
		private string _payPlanTermsAndConditions;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public List<PrefValSync> ListPrefValSyncs;
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountPayments() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Events
		public event EventHandler SyncChanged;
		#endregion Events

		#region Methods - Event Handlers
		private void butPayPlanTermsAndConditions_Click(object sender,EventArgs e) {
			using FormMessageReplacementTextEdit formMessageReplacementTextEdit=new FormMessageReplacementTextEdit();
			formMessageReplacementTextEdit.TextEditorText=_payPlanTermsAndConditions;
			if(formMessageReplacementTextEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_payPlanTermsAndConditions=formMessageReplacementTextEdit.TextEditorText;
		}

		private void comboPayPlansVersion_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboPayPlansVersion.SelectedIndex==(int)PayPlanVersions.AgeCreditsAndDebits-1) {//Minus 1 because the enum starts at 1.
				checkHideDueNow.Visible=true;
				checkHideDueNow.Checked=PrefC.GetBool(PrefName.PayPlanHideDueNow);
			}
			else {
				checkHideDueNow.Visible=false;
				checkHideDueNow.Checked=false;
			}
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PayPlansVersion);
			prefValSync.PrefVal=POut.Int(comboPayPlansVersion.SelectedIndex+1);//PrefVal is the enum which starts at 1 so add 1 to SelectedIndex to get Enum
			SyncChanged?.Invoke(this,new EventArgs());
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

		#endregion Methods - Event Handlers Sync

		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountPayments() {
			checkStoreCCTokens.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			foreach(PayClinicSetting prompt in Enum.GetValues(typeof(PayClinicSetting))) {
				comboPaymentClinicSetting.Items.Add(Lan.g(this,prompt.GetDescription()));
			}
			//comboPaymentClinicSetting.SelectedIndex=PrefC.GetInt(PrefName.PaymentClinicSetting);
			//checkPaymentsPromptForPayType.Checked=PrefC.GetBool(PrefName.PaymentsPromptForPayType);
			comboUnallocatedSplits.Items.AddDefs(Defs.GetDefsForCategory(DefCat.PaySplitUnearnedType,true));
			comboUnallocatedSplits.SetSelectedDefNum(PrefC.GetLong(PrefName.PrepaymentUnearnedType));
			checkAllowEmailCCReceipt.Checked=PrefC.GetBool(PrefName.AllowEmailCCReceipt);
			checkIncTxfrTreatNegProdAsIncome.Checked=PrefC.GetBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome);
			checkPaymentCompletedDisableMerchantButtons.Checked=PrefC.GetBool(PrefName.PaymentsCompletedDisableMerchantButtons);
			checkAllowPrepayProvider.Checked=PrefC.GetBool(PrefName.AllowPrepayProvider);
			comboDppUnearnedType.Items.AddDefs(Defs.GetHiddenUnearnedDefs(true));
			comboDppUnearnedType.SetSelectedDefNum(PrefC.GetLong(PrefName.DynamicPayPlanPrepaymentUnearnedType));
			checkPayPlanSaveSignedPdf.Checked=PrefC.GetBool(PrefName.PayPlanSaveSignedToPdf);
			checkPayPlansExcludePastActivity.Checked=PrefC.GetBool(PrefName.PayPlansExcludePastActivity);
			checkPayPlansUseSheets.Checked=PrefC.GetBool(PrefName.PayPlansUseSheets);
			checkPayPlanRequireLockForAPR.Checked=PrefC.GetBool(PrefName.PayPlanRequireLockForAPR);
			foreach(PayPlanVersions version in Enum.GetValues(typeof(PayPlanVersions))) {
				comboPayPlansVersion.Items.Add(Lan.g("enumPayPlanVersions",version.GetDescription()));
			}
			//comboPayPlansVersion.SelectedIndex=PrefC.GetInt(PrefName.PayPlansVersion) - 1;
			//if(comboPayPlansVersion.SelectedIndex==(int)PayPlanVersions.AgeCreditsAndDebits-1) {//Minus 1 because the enum starts at 1.
			//	checkHideDueNow.Visible=true;
			//	checkHideDueNow.Checked=PrefC.GetBool(PrefName.PayPlanHideDueNow);
			//} 
			//else {
			//	checkHideDueNow.Visible=false;
			//	checkHideDueNow.Checked=false;
			//}
			textDynamicPayPlan.Text=PrefC.GetDateT(PrefName.DynamicPayPlanRunTime).TimeOfDay.ToShortTimeString();
			_payPlanTermsAndConditions=PrefC.GetString(PrefName.PayPlanTermsAndConditions);
			checkShowAllocateUnearnedPaymentPrompt.Checked=PrefC.GetBool(PrefName.ShowAllocateUnearnedPaymentPrompt);
			checkOnlinePaymentsMarkAsProcessed.Checked=PrefC.GetBool(PrefName.OnlinePaymentsMarkAsProcessed);
		}

		public bool SaveAccountPayments() {
			if(string.IsNullOrWhiteSpace(textDynamicPayPlan.Text) || !textDynamicPayPlan.IsValid()) {
				MsgBox.Show(this,"Payment plan time must be a valid time.");
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.PayPlansUseSheets,checkPayPlansUseSheets.Checked);
			//(synced) Changed|=Prefs.UpdateInt(PrefName.PayPlansVersion,comboPayPlansVersion.SelectedIndex+1);
			Changed|=Prefs.UpdateBool(PrefName.PayPlansExcludePastActivity,checkPayPlansExcludePastActivity.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PayPlanHideDueNow,checkHideDueNow.Checked);
			Changed|=Prefs.UpdateDateT(PrefName.DynamicPayPlanRunTime,PIn.DateT(textDynamicPayPlan.Text));
			Changed|=Prefs.UpdateLong(PrefName.DynamicPayPlanPrepaymentUnearnedType,comboDppUnearnedType.GetSelectedDefNum());
			Changed|=Prefs.UpdateBool(PrefName.PayPlanSaveSignedToPdf,checkPayPlanSaveSignedPdf.Checked);
			Changed|=Prefs.UpdateString(PrefName.PayPlanTermsAndConditions,_payPlanTermsAndConditions);
			Changed|=Prefs.UpdateBool(PrefName.StoreCCtokens,checkStoreCCTokens.Checked);
			//(synced) Changed|=Prefs.UpdateInt(PrefName.PaymentClinicSetting,comboPaymentClinicSetting.SelectedIndex);
			//(synced) Changed|=Prefs.UpdateBool(PrefName.PaymentsPromptForPayType,checkPaymentsPromptForPayType.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowEmailCCReceipt,checkAllowEmailCCReceipt.Checked);
			Changed|=Prefs.UpdateBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome,checkIncTxfrTreatNegProdAsIncome.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PaymentsCompletedDisableMerchantButtons,checkPaymentCompletedDisableMerchantButtons.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowPrepayProvider,checkAllowPrepayProvider.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowAllocateUnearnedPaymentPrompt,checkShowAllocateUnearnedPaymentPrompt.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OnlinePaymentsMarkAsProcessed,checkOnlinePaymentsMarkAsProcessed.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PayPlanRequireLockForAPR,checkPayPlanRequireLockForAPR.Checked);
			if(comboUnallocatedSplits.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.PrepaymentUnearnedType,comboUnallocatedSplits.GetSelectedDefNum());
			}
			return true;
		}

		public void FillSynced(){
			PrefValSync prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentClinicSetting);
			comboPaymentClinicSetting.SelectedIndex=PIn.Int(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PaymentsPromptForPayType);
			checkPaymentsPromptForPayType.Checked=PIn.Bool(prefValSync.PrefVal);
			prefValSync=ListPrefValSyncs.Find(x=>x.PrefName_==PrefName.PayPlansVersion);
			comboPayPlansVersion.SelectedIndex=PIn.Int(prefValSync.PrefVal)-1;//PrefVal is the enum which starts at 1 so subtract 1 for SelectedIndex
			if(comboPayPlansVersion.SelectedIndex==(int)PayPlanVersions.AgeCreditsAndDebits-1) {//Minus 1 because the enum starts at 1.
				checkHideDueNow.Visible=true;
				checkHideDueNow.Checked=PrefC.GetBool(PrefName.PayPlanHideDueNow);
			} 
			else {
				checkHideDueNow.Visible=false;
				checkHideDueNow.Checked=false;
			}
		}
		#endregion Methods - Public
	}
}
