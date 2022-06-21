using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlManageGeneral:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlManageGeneral() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butEraAutomationDetails_Click(object sender,EventArgs e) {
			string html=$"Determines if ERAs are processed automatically or manually.<br><br>" +
				"Review All: All ERAs must be processed manually.<br><br>" +
				"Semi-automatic: ERAs can be processed with a single click of the Auto Process button on the ERA window. This will receive the claims associated with the" +
				"ERA, and finalize the payment. They can also be processed manually, if needed.<br><br>" +
				"Fully-automatic: ERAs will be automatically processed when imported, receiving the claims associated with the ERA, and finalizing the payment. " +
				"If an ERA does not get automatically processed while being imported for any reason, the user can still attempt to process them by clicking the " +
				"Auto Process button on the ERA window, or process them manually.<br><br>" +
				"Note: This preference can also be set on a Carrier level. See " +
				"<a href='https://opendental.com/manual/carriers.html' target='_blank' rel='noopener noreferrer'>Carriers</a>.";
			using FormWebBrowserPrefs formWebBrowserPrefs=new FormWebBrowserPrefs();
			formWebBrowserPrefs.HtmlContent=html;
			formWebBrowserPrefs.PointStart=PointToScreen(butIEraAutomationDetails.Location);
			formWebBrowserPrefs.SizeWindow=new Size(500,350);
			formWebBrowserPrefs.ShowDialog();
		}

		private void comboDepositSoftware_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboDepositSoftware.GetSelected<AccountingSoftware>()==AccountingSoftware.QuickBooksOnline
				&& !Programs.IsEnabled(ProgramName.QuickBooksOnline)) {
				//If users attempts to select QuickBooks Online but the program is not enabled, set accounting software back and alert them.
				comboDepositSoftware.SetSelected(PrefC.GetInt(PrefName.AccountingSoftware));
				MsgBox.Show(this,"QuickBooks Online must be enabled in Program Links before it can be selected as your Deposit Software.");
			}
		}

		private void linkLabelClaimPaymentBatchOnly_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://opendental.com/manual/claimedit.html");
		}

		private void linkLabelClaimsReceivedDaysDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://www.opendental.com/manual/claimpaymentbatch.html");
		}

		private void linkLabelShowAutoDepositDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			Process.Start("https://opendental.com/manual/claimpayfinalize.html");
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>Fills ComboEraAutomation with EraAutomationMode enum values. Excludes EraAutomationMode.UseGlobal because that value only applies to Carrier.EraAutomationOverride.</summary>
		private void FillComboEraAutomation() {
			List<EraAutomationMode> listEraAutomationModes=typeof(EraAutomationMode).GetEnumValues()
				.AsEnumerable<EraAutomationMode>()
				.Where(x => x!=EraAutomationMode.UseGlobal)
				.ToList();
			comboEraAutomation.Items.AddListEnum(listEraAutomationModes);
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillManageGeneral() {
			checkRxSendNewToQueue.Checked=PrefC.GetBool(PrefName.RxSendNewToQueue);
			int claimZeroPayRollingDays=PrefC.GetInt(PrefName.ClaimPaymentNoShowZeroDate);
			if(claimZeroPayRollingDays>=0) {
				textClaimsReceivedDays.Text=(claimZeroPayRollingDays+1).ToString();//The minimum value is now 1 ("today"), to match other areas of OD.
			}
			for(int i=0;i<7;i++) {
				comboTimeCardOvertimeFirstDayOfWeek.Items.Add(Lan.g("enumDayOfWeek",Enum.GetNames(typeof(DayOfWeek))[i]));
			}
			comboTimeCardOvertimeFirstDayOfWeek.SelectedIndex=PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek);
			checkTimeCardADP.Checked=PrefC.GetBool(PrefName.TimeCardADPExportIncludesName);
			checkClaimsSendWindowValidateOnLoad.Checked=PrefC.GetBool(PrefName.ClaimsSendWindowValidatesOnLoad);
			checkScheduleProvEmpSelectAll.Checked=PrefC.GetBool(PrefName.ScheduleProvEmpSelectAll);
			checkClockEventAllowBreak.Checked=PrefC.GetBool(PrefName.ClockEventAllowBreak);
			checkEraAllowTotalPayment.Checked=PrefC.GetBool(PrefName.EraAllowTotalPayments);
			checkClaimPaymentBatchOnly.Checked=PrefC.GetBool(PrefName.ClaimPaymentBatchOnly);
			checkEraOneClaimPerPage.Checked=PrefC.GetBool(PrefName.EraPrintOneClaimPerPage);
			checkIncludeEraWOPercCoPay.Checked=PrefC.GetBool(PrefName.EraIncludeWOPercCoPay);
			checkShowAutoDeposit.Checked=PrefC.GetBool(PrefName.ShowAutoDeposit);
			FillComboEraAutomation();
			comboEraAutomation.SetSelectedEnum(PrefC.GetEnum<EraAutomationMode>(PrefName.EraAutomationBehavior));
			comboDepositSoftware.Items.AddEnums<AccountingSoftware>();
			comboDepositSoftware.SetSelectedEnum(PrefC.GetEnum<AccountingSoftware>(PrefName.AccountingSoftware));
			checkRxHideProvsWithoutDEA.Checked=PrefC.GetBool(PrefName.RxHideProvsWithoutDEA);
			checkAccountingInvoiceAttachmentsSaveInDatabase.Checked=PrefC.GetBool(PrefName.AccountingInvoiceAttachmentsSaveInDatabase);
		}

		public bool SaveManageGeneral() {
			if(!textClaimsReceivedDays.IsValid()) {
				MsgBox.Show(this,"Show claims received after days must be a positive integer or blank.");
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.RxSendNewToQueue,checkRxSendNewToQueue.Checked);
			Changed|=Prefs.UpdateInt(PrefName.TimeCardOvertimeFirstDayOfWeek,comboTimeCardOvertimeFirstDayOfWeek.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.TimeCardADPExportIncludesName,checkTimeCardADP.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClaimsSendWindowValidatesOnLoad,checkClaimsSendWindowValidateOnLoad.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ScheduleProvEmpSelectAll,checkScheduleProvEmpSelectAll.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ClockEventAllowBreak,checkClockEventAllowBreak.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EraAllowTotalPayments,checkEraAllowTotalPayment.Checked);
			Changed|=Prefs.UpdateInt(PrefName.ClaimPaymentNoShowZeroDate,(textClaimsReceivedDays.Text=="")?-1:(PIn.Int(textClaimsReceivedDays.Text)-1));
			Changed|=Prefs.UpdateBool(PrefName.ClaimPaymentBatchOnly,checkClaimPaymentBatchOnly.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EraPrintOneClaimPerPage,checkEraOneClaimPerPage.Checked);
			Changed|=Prefs.UpdateBool(PrefName.EraIncludeWOPercCoPay,checkIncludeEraWOPercCoPay.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ShowAutoDeposit,checkShowAutoDeposit.Checked);
			Changed|=Prefs.UpdateInt(PrefName.EraAutomationBehavior,(int)comboEraAutomation.GetSelected<EraAutomationMode>());
			Changed|=Prefs.UpdateInt(PrefName.AccountingSoftware,(int)comboDepositSoftware.GetSelected<AccountingSoftware>());
			Changed|=Prefs.UpdateBool(PrefName.RxHideProvsWithoutDEA,checkRxHideProvsWithoutDEA.Checked);
			Changed|=Prefs.UpdateBool(PrefName.AccountingInvoiceAttachmentsSaveInDatabase,checkAccountingInvoiceAttachmentsSaveInDatabase.Checked);
			return true;
		}
		#endregion Methods - Public
	}
}
