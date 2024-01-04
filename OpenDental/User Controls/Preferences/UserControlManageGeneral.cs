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
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void comboDepositSoftware_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboDepositSoftware.GetSelected<AccountingSoftware>()==AccountingSoftware.QuickBooksOnline
				&& !Programs.IsEnabled(ProgramName.QuickBooksOnline)) {
				//If users attempts to select QuickBooks Online but the program is not enabled, set accounting software back and alert them.
				comboDepositSoftware.SetSelected(PrefC.GetInt(PrefName.AccountingSoftware));
				MsgBox.Show(this,"QuickBooks Online must be enabled in Program Links before it can be selected as your Deposit Software.");
			}
		}

		private void linkLabelClaimPaymentBatchOnly_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			try {
				Process.Start("https://opendental.com/manual/claimedit.html");
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+"https://opendental.com/manual/claimedit.html"+"\r\n"
					+Lan.g(this,"Please set up a default web browser."));
			}
		}

		private void linkLabelClaimsReceivedDaysDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			try {
				Process.Start("https://www.opendental.com/manual/claimpaymentbatch.html");
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+"https://www.opendental.com/manual/claimpaymentbatch.html"+"\r\n"
					+Lan.g(this,"Please set up a default web browser."));
			}
		}

		private void linkLabelShowAutoDepositDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			try {
				Process.Start("https://opendental.com/manual/claimpayfinalize.html");
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Could not find")+" "+"https://opendental.com/manual/claimpayfinalize.html"+"\r\n"
					+Lan.g(this,"Please set up a default web browser."));
			}
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

		/// <summary>Fills the CHK, ACH, FWT, and ERADefault comboboxes. If no def is chosen yet "None" is put in there</summary>
		private void FillEraPaymentTypeCombos(){
			//Add each of the defs and set the default for each ERA combobox
			FillComboEraPaymentTypeHelper(comboEraCheckPaymentType, PrefName.EraChkPaymentType);
			FillComboEraPaymentTypeHelper(comboAchPaymentType, PrefName.EraAchPaymentType);
			FillComboEraPaymentTypeHelper(comboFwtPaymentType, PrefName.EraFwtPaymentType);
			FillComboEraPaymentTypeHelper(comboEraDefaultPaymentType, PrefName.EraDefaultPaymentType);
		}

		private void FillComboEraPaymentTypeHelper(UI.ComboBox comboBox, PrefName prefName){
			comboBox.Items.Clear();
			comboBox.Items.AddDefNone();
			comboBox.Items.AddDefs(Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true));
			comboBox.SetSelectedDefNum(PrefC.GetLong(prefName:prefName));
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
			FillEraPaymentTypeCombos();
		}

		public bool SaveManageGeneral() {
			if(!textClaimsReceivedDays.IsValid()) {
				MsgBox.Show(this,"Show claims received after days must be a positive integer (max 50,000) or blank.");
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
			Changed|=Prefs.UpdateLong(PrefName.EraChkPaymentType, comboEraCheckPaymentType.GetSelectedDefNum());
			Changed|=Prefs.UpdateLong(PrefName.EraAchPaymentType, comboAchPaymentType.GetSelectedDefNum());
			Changed|=Prefs.UpdateLong(PrefName.EraFwtPaymentType, comboFwtPaymentType.GetSelectedDefNum());
			Changed|=Prefs.UpdateLong(PrefName.EraDefaultPaymentType, comboEraDefaultPaymentType.GetSelectedDefNum());
			return true;
		}
		#endregion Methods - Public
	}
}
