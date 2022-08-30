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
		#endregion Fields - Public

		#region Constructors
		public UserControlAccountPayments() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butIncTxfrTreatNegProdAsIncomeDetails_Click(object sender,EventArgs e) {
			string html=@"See <a href='https://opendental.com/manual/incometransfermanager.html' target='_blank' rel='noopener noreferrer'>Income Transfer Manager.</a> Not recommended
				for offices that pay providers based on collections. If using the FIFO button (rare), then this will also reallocate overpayments.";
			using FormWebBrowserPrefs formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			formWebBrowserPrefs.PointStart=PointToScreen(butIncTxfrTreatNegProdAsIncomeDetails.Location);
			formWebBrowserPrefs.ShowDialog();
		}

		private void butPayPlanTermsAndConditions_Click(object sender,EventArgs e) {
			using FormMessageReplacementTextEdit formMessageReplacementTextEdit=new FormMessageReplacementTextEdit();
			formMessageReplacementTextEdit.TextEditorText=_payPlanTermsAndConditions;
			if(formMessageReplacementTextEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_payPlanTermsAndConditions=formMessageReplacementTextEdit.TextEditorText;
		}

		private void butPayPlansVersionDetails_Click(object sender,EventArgs e) {
			string html=@"Determines how charges and credits for Patient Payment Plans show in the patient account ledger and whether they affect balances, aging, and reports. 
				This logic does not apply to insurance payment plans and will continue to use the old logic:
				<br><br><FONT COLOR='RED'>Do Not Age (Legacy):</FONT> Payment plan debits (amounts due) and payments only show within the payment plan and will not affect balance or aging.
				<br>-Payment plan debits are totaled in the Payment Plans grid under Due Now.
				<br>-Payment plan payments do not show in the ledger but in the payment plan. Double-clicking the plan row is the only way to view payment plan payments.
				<br>-One payment plan credit (PayPln) will show as a single line item in the patient account ledger, thus reducing the total account balance by the amount. The credit 
				amount is based on the Tx Completed Amt set in the payment plan.
				<br>-Other payment plan credits, debits, and payments do not show in the ledger nor do they affect balances or aging.
				<br>-The total A/R in the Aging of A/R report will not include payment plan due amounts.
				<br>-Only changes to the Tx Completed Amount affect aging and production and income reports.
				<br>-Payment plan amounts are not included on the Receivables Breakdown Report.
				<br><FONT COLOR='RED'>Age Credits and Debits (Default):</FONT> Payment plan debits, credits, and payments will show as line items in patient account ledger and affect 
				balances and aging. 
				<br>When the patient is in the same family as the payment plan guarantor, the behavior is as follows.
				<br>-Payment plan amounts due (PayPln: Debit) and credits (PayPln: Credit) show as line items in the patient account ledger.
				<br>-Payment plan payments show in the account ledger.
				<br>-Payment plan due amounts are included the patient's balance.
				<br>-Payment plan amounts due and payments are considered when calculating aging.
				<br>-Payment plan credits and debits are included on the Receivables Breakdown report.
				<br>-Changes made to historical payment plan charges will affect historical information (e.g. Aging of A/R, Production and Income reports).
				<br>When the patient is in a different family than the payment plan guarantor, the behavior is as follows.
				<br>-Payment plan amounts due (PayPln: Debit) show as line items in the guarantor's account ledger.
				<br>-Payment plan credits (PayPln: Credit) show as line items in the patient's account ledger.
				<br>-Payment plan payments show in the guarantor's account ledger.
				<br>-Payment plan due amounts are included in the guarantor's balance.
				<br><FONT COLOR='RED'>Age Credits Only:</FONT> Patients are credited for payment plans when the credit comes due, but debits all exist separately from the account ledger.
				<br>-Each payment plan credit line item will show in the account ledger, sorted by Tx Credit date.
				<br>-Payment plan amounts due only show in the Payment Plan grid. They do not show in the account ledger.
				<br>-Payment plan amounts due will not be considered when calculating balances and aging.
				<br>-Payment plan credits and debits will not be included on the Receivables Breakdown report.
				<br>-Changes made to historical payment plan credits will affect historical information (e.g. Aging of A/R, Production and Income reports).
				<br><FONT COLOR='RED'>No Charges to Account (Rarely Used):</FONT> Patients are not credited for payment plans so the account balance is aged normally.
				<br>-Payment plan amounts due only show in the Payment Plan grid. They do not show in the account ledger.
				<br>-Payments to payment plans show in ledger and payment plan.
				<br>-Payment plan amounts will not be included on the Receivables Breakdown report.
				<br><br>See <a href='https://opendental.com/manual/paymentplanpatientfaq.html' target='_blank' rel='noopener noreferrer'>Payment Plan Q and A</a> for additional details.";
			using FormWebBrowserPrefs formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			formWebBrowserPrefs.SizeWindow=new Size(1200,575);
			formWebBrowserPrefs.ShowDialog();
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
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillAccountPayments() {
			checkStoreCCTokens.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			foreach(PayClinicSetting prompt in Enum.GetValues(typeof(PayClinicSetting))) {
				comboPaymentClinicSetting.Items.Add(Lan.g(this,prompt.GetDescription()));
			}
			comboPaymentClinicSetting.SelectedIndex=PrefC.GetInt(PrefName.PaymentClinicSetting);
			checkPaymentsPromptForPayType.Checked=PrefC.GetBool(PrefName.PaymentsPromptForPayType);
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
			foreach(PayPlanVersions version in Enum.GetValues(typeof(PayPlanVersions))) {
				comboPayPlansVersion.Items.Add(Lan.g("enumPayPlanVersions",version.GetDescription()));
			}
			comboPayPlansVersion.SelectedIndex=PrefC.GetInt(PrefName.PayPlansVersion) - 1;
			if(comboPayPlansVersion.SelectedIndex==(int)PayPlanVersions.AgeCreditsAndDebits-1) {//Minus 1 because the enum starts at 1.
				checkHideDueNow.Visible=true;
				checkHideDueNow.Checked=PrefC.GetBool(PrefName.PayPlanHideDueNow);
			} 
			else {
				checkHideDueNow.Visible=false;
				checkHideDueNow.Checked=false;
			}
			textDynamicPayPlan.Text=PrefC.GetDateT(PrefName.DynamicPayPlanRunTime).TimeOfDay.ToShortTimeString();
			_payPlanTermsAndConditions=PrefC.GetString(PrefName.PayPlanTermsAndConditions);
			checkShowAllocateUnearnedPaymentPrompt.Checked=PrefC.GetBool(PrefName.ShowAllocateUnearnedPaymentPrompt);
			checkOnlinePaymentsMarkAsProcessed.Checked=PrefC.GetBool(PrefName.OnlinePaymentsMarkAsProcessed);
		}

		public bool SaveAccountPayments() {
			if(string.IsNullOrWhiteSpace(textDynamicPayPlan.Text) || !textDynamicPayPlan.IsValid()) {
				MsgBox.Show(this,"Dynamic payment plan time must be a valid time.");
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.PayPlansUseSheets,checkPayPlansUseSheets.Checked);
			Changed|=Prefs.UpdateInt(PrefName.PayPlansVersion,comboPayPlansVersion.SelectedIndex+1);
			Changed|=Prefs.UpdateBool(PrefName.PayPlansExcludePastActivity,checkPayPlansExcludePastActivity.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PayPlanHideDueNow,checkHideDueNow.Checked);
			Changed|=Prefs.UpdateDateT(PrefName.DynamicPayPlanRunTime,PIn.DateT(textDynamicPayPlan.Text));
			Changed|=Prefs.UpdateLong(PrefName.DynamicPayPlanPrepaymentUnearnedType,comboDppUnearnedType.GetSelectedDefNum());
			Changed|=Prefs.UpdateBool(PrefName.PayPlanSaveSignedToPdf,checkPayPlanSaveSignedPdf.Checked);
			Changed|=Prefs.UpdateString(PrefName.PayPlanTermsAndConditions,_payPlanTermsAndConditions);
			Changed|=Prefs.UpdateBool(PrefName.StoreCCtokens,checkStoreCCTokens.Checked);
			Changed|=Prefs.UpdateInt(PrefName.PaymentClinicSetting,comboPaymentClinicSetting.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.PaymentsPromptForPayType,checkPaymentsPromptForPayType.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowEmailCCReceipt,checkAllowEmailCCReceipt.Checked);
			Changed|=Prefs.UpdateBool(PrefName.IncomeTransfersTreatNegativeProductionAsIncome,checkIncTxfrTreatNegProdAsIncome.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PaymentsCompletedDisableMerchantButtons,checkPaymentCompletedDisableMerchantButtons.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AllowPrepayProvider,checkAllowPrepayProvider.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowAllocateUnearnedPaymentPrompt,checkShowAllocateUnearnedPaymentPrompt.Checked);
			Changed|=Prefs.UpdateBool(PrefName.OnlinePaymentsMarkAsProcessed,checkOnlinePaymentsMarkAsProcessed.Checked);
			if(comboUnallocatedSplits.SelectedIndex!=-1) {
				Changed|=Prefs.UpdateLong(PrefName.PrepaymentUnearnedType,comboUnallocatedSplits.GetSelectedDefNum());
			}
			return true;
		}
		#endregion Methods - Public
	}
}
