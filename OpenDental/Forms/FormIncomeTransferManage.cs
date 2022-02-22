using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Text;

namespace OpenDental {
	public partial class FormIncomeTransferManage:FormODBase {
		private Family _family;
		private Patient _patient;
		private PaymentEdit.ConstructResults _constructResults;

		public FormIncomeTransferManage(Family family,Patient patient) {
			_family=family;
			_patient=patient;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormIncomeTransferManage_Load(object sender,EventArgs e) {
			//Intentionally check by the payment create permission even though it is claim supplementals (InsPayCreate).
			if(Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today,true)) {
				try {
					PaymentEdit.TransferClaimsPayAsTotal(_patient.PatNum,_family.GetPatNums(),"Automatic transfer of claims pay as total from income transfer.");
				}
				catch(ApplicationException ex) {
					FriendlyException.Show(ex.Message,ex);
					return;
				}
			}
			RefreshWindow();
		}

		///<summary>Refreshes all of the data from the database and updates the UI accordingly.</summary>
		private void RefreshWindow() {
			FillTransfers();
			FillGridCharges();
			//IsIncomeTransferNeeded must be called after FillGridCharges has been invoked so that _constructResults is not null.
			butTransfer.Enabled=IsTransferRigorousNeeded();
			//Don't allow FIFO income transfers when RigorousAccounting is set to EnforceFully.
			RigorousAccounting rigorousAccounting=(RigorousAccounting)PrefC.GetInt(PrefName.RigorousAccounting);
			if(rigorousAccounting==RigorousAccounting.EnforceFully) {
				butFIFO.Enabled=false;
			}
			else {
				butFIFO.Enabled=IsTransferFifoNeeded();
			}
		}

		///<summary></summary>
		private void FillTransfers() {
			string translationName=gridTransfers.TranslationName;
			gridTransfers.BeginUpdate();
			gridTransfers.Columns.Clear();
			gridTransfers.Columns.Add(new GridColumn(Lan.g(translationName,"Date"),65,HorizontalAlignment.Center));
			if(PrefC.HasClinicsEnabled) {//Clinics
				gridTransfers.Columns.Add(new GridColumn(Lan.g(translationName,"Clinic"),80){ IsWidthDynamic=true });
			}
			gridTransfers.Columns.Add(new GridColumn(Lan.g(translationName,"Paid By"),80){ IsWidthDynamic=true });
			gridTransfers.ListGridRows.Clear();
			List<Payment> listPaymentTransfers=Payments.GetTransfers(_family.GetPatNums()).OrderBy(x => x.PayDate).ToList();
			for(int i = 0;i<listPaymentTransfers.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listPaymentTransfers[i].PayDate.ToShortDateString());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(listPaymentTransfers[i].ClinicNum));
				}
				row.Cells.Add(_family.GetNameInFamFL(listPaymentTransfers[i].PatNum));
				row.Tag=listPaymentTransfers[i];
				gridTransfers.ListGridRows.Add(row);
			}
			gridTransfers.EndUpdate();
			gridTransfers.ScrollToEnd();
		}

		///<summary></summary>
		private void FillGridCharges(bool doRefreshData=true) {
			gridImbalances.BeginUpdate();
			gridImbalances.Columns.Clear();
			gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Prov"),80){ IsWidthDynamic=true });
			gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Patient"),80){ IsWidthDynamic=true });
			if(PrefC.HasClinicsEnabled) {
				gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Clinic"),80){ IsWidthDynamic=true });
			}
			if(checkShowBreakdown.Checked) {
				gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Type"),80) { IsWidthDynamic=true });
			}
			gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Charges"),80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Credits"),80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridImbalances.Columns.Add(new GridColumn(Lan.g(this,"Balance"),80,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridImbalances.ListGridRows.Clear();
			if(_constructResults==null || doRefreshData) {
				_constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_family.GetPatNums(),_patient.PatNum,
					new List<PaySplit>(),new Payment(),new List<AccountEntry>(),isIncomeTxfr:true,dateAsOf:datePickerAsOf.Value);
			}
			List<List<AccountEntry>> listListsAccountEntries=_constructResults.ListAccountEntries
				.GroupBy(x => new { x.PatNum,x.ProvNum,x.ClinicNum })
				.Select(x => new List<AccountEntry>(x.ToList())).ToList();
			for(int i = 0;i<listListsAccountEntries.Count;i++) {
				//Do not display any groups that have their AmountEnd sum to zero.
				if(CompareDecimal.IsZero(listListsAccountEntries[i].Sum(x => x.AmountEnd))) {
					continue;
				}
				gridImbalances.ListGridRows.AddRange(GetRowsForGroup(listListsAccountEntries[i]));
			}
			gridImbalances.EndUpdate();
		}

		private List<GridRow> GetRowsForGroup(List<AccountEntry> listAccountEntries) {
			List<GridRow> listGridRows=new List<GridRow>();
			AccountEntry accountEntryFirst=listAccountEntries.First();
			GridRow row=new GridRow();
			row.Bold=checkShowBreakdown.Checked;
			row.Cells.Add(Providers.GetAbbr(accountEntryFirst.ProvNum,includeHidden: true));
			Patient patient=_family.GetPatient(accountEntryFirst.PatNum);
			row.Cells.Add(patient.GetNameFLnoPref());
			if(PrefC.HasClinicsEnabled) {
				row.Cells.Add(Clinics.GetAbbr(accountEntryFirst.ClinicNum));
			}
			if(checkShowBreakdown.Checked) {
				row.Cells.Add("");
			}
			List<AccountEntry> listPositiveEntries = listAccountEntries.FindAll(x => CompareDecimal.IsGreaterThanZero(x.AmountEnd));
			row.Cells.Add(listPositiveEntries.Sum(x => x.AmountEnd).ToString("c"));
			List<AccountEntry> listNegativeEntries = listAccountEntries.FindAll(x => CompareDecimal.IsLessThanZero(x.AmountEnd));
			row.Cells.Add(listNegativeEntries.Sum(x => x.AmountEnd).ToString("c"));
			row.Cells.Add(listAccountEntries.Sum(x => x.AmountEnd).ToString("c"));
			row.Tag=listAccountEntries;
			listGridRows.Add(row);
			if(!checkShowBreakdown.Checked) {
				return listGridRows;
			}
			decimal runningTotal=0;
			for(int i = 0;i<listAccountEntries.Count;i++) {
				//Adjustments that are explicitly linked to procedures correctly with have an AmountEnd of $0.
				//These adjustment entries have had their value attributed to the linked procedure entries and can be hidden.
				if(listAccountEntries[i].AmountEnd==0 && listAccountEntries[i].GetType()==typeof(Adjustment) && listAccountEntries[i].ProcNum > 0) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add("");
				row.Cells.Add("");
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add("");
				}
				row.Cells.Add(listAccountEntries[i].DescriptionForGrid);
				if(CompareDecimal.IsGreaterThanZero(listAccountEntries[i].AmountEnd)) {
					row.Cells.Add(listAccountEntries[i].AmountEnd.ToString("c"));
				}
				else {
					row.Cells.Add(0.ToString("c"));
				}
				if(CompareDecimal.IsLessThanZero(listAccountEntries[i].AmountEnd)) {
					row.Cells.Add(listAccountEntries[i].AmountEnd.ToString("c"));
				}
				else {
					row.Cells.Add(0.ToString("c"));
				}
				runningTotal+=listAccountEntries[i].AmountEnd;
				row.Cells.Add(runningTotal.ToString("c"));
				row.Tag=listAccountEntries[i];
				listGridRows.Add(row);
			}
			return listGridRows;
		}

		///<summary>Performs a FIFO income transfer but doesn't save the results.  Instead, returns true if any PaySplits are suggested for transfer
		///Otherwise returns false.  This method does not check permissions on purpose (so the user can get permission specific messages).</summary>
		private bool IsTransferFifoNeeded() {
			List<PaySplit> listPaySplits=null;
			ODException.SwallowAnyException(() => {
				PaymentEdit.IncomeTransferData incomeTransferData=PaymentEdit.GetIncomeTransferDataFIFO(_patient.PatNum,DateTime.Today);
				listPaySplits=incomeTransferData.ListSplitsCur;
			});
			return (listPaySplits==null || listPaySplits.Count > 0);
		}

		///<summary>Performs a rigorous income transfer but doesn't save the results.  Instead, returns true if any PaySplits are suggested for transfer
		///or if there is something wrong with the transfer in general so that the user can click the Transfer button and get the error message.
		///Otherwise returns false.  This method does not check permissions on purpose (so the user can get permission specific messages).</summary>
		private bool IsTransferRigorousNeeded() {
			//Make a deep copy of the current charges because the income transfer can manipulate the values within the objects.
			List<AccountEntry> listAccountEntries=_constructResults.ListAccountEntries.Select(x => x.Copy()).ToList();
			if(!PaymentEdit.TryCreateIncomeTransfer(listAccountEntries,DateTime.Today,out PaymentEdit.IncomeTransferData results)) {
				return true;//Something is wrong and a transfer cannot be performed.  Let the user click the Transfer button in order to get this error.
			}
			if(results.HasInvalidSplits) {
				return true;//Let the user click the Transfer button in order to get this error.
			}
			if(results.ListSplitsCur.Count>0) {
				return true;//A transfer needs to be made for this account.
			}
			//The income transfer code successfully ran and had no suggested splits to transfer therefore a transfer is not necessary.
			return false;
		}

		private bool IsValid() {
			if(!Security.IsAuthorized(Permissions.PaymentCreate,datePickerAsOf.Value)) {
				return false;
			}
			string dbmLog=DatabaseMaintenances.ProcedurelogDeletedWithAttachedIncome(false,DbmMode.Breakdown,_patient.PatNum);
			if(dbmLog!="") {
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(dbmLog);
				msgBoxCopyPaste.Show();
				return false;
			}
			return true;
		}

		private void CreatePaymentAndRefresh(List<PaySplit> listPaySplits,bool isRigorous) {
			if(listPaySplits.IsNullOrEmpty()) {
				return;
			}
			Payment payment=new Payment();
			payment.PayDate=datePickerAsOf.Value;
			payment.PatNum=_patient.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			payment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				payment.ClinicNum=Clinics.ClinicNum;
				if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					payment.ClinicNum=_patient.ClinicNum;
				}
				else if((PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.SelectedExceptHQ) {
					payment.ClinicNum=(Clinics.ClinicNum==0 ? _patient.ClinicNum : Clinics.ClinicNum);
				}
			}
			payment.DateEntry=DateTime.Today;//So that it will show properly in the new window.
			payment.PaymentSource=CreditCardSource.None;
			payment.ProcessStatus=ProcessStat.OfficeProcessed;
			payment.PayAmt=0;
			payment.PayType=0;
			Payments.Insert(payment,listPaySplits);
			string strLogic="FIFO logic";
			if(isRigorous) {
				strLogic="Rigorous logic";
			}
			string logText=Payments.GetSecuritylogEntryText(payment,payment,isNew:true)+", "+Lans.g(this,$"from Income Transfer Manager using {strLogic}.");
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,payment.PatNum,logText);
			string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patient.PatNum,listPaySplits);
			if(!string.IsNullOrEmpty(strErrorMsg)) {
				MessageBox.Show(strErrorMsg);
			}
			RefreshWindow();
		}

		private void checkShowBreakdown_CheckedChanged(object sender,EventArgs e) {
			FillGridCharges(doRefreshData:false);
		}

		private void datePickerTransfer_ValueChanged(object sender,EventArgs e) {
			RefreshWindow();
		}

		private void gridTransfers_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormPayment formPayment=new FormPayment(_patient,_family,(Payment)gridTransfers.ListGridRows[e.Row].Tag,false);
			formPayment.IsNew=false;
			formPayment.ShowDialog();
			RefreshWindow();//The user could have done anything, refresh the UI just to be safe.
		}

		///<summary>Creates paysplits for selected charges if there is enough payment left.</summary>
		private void butTransfer_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			List<AccountEntry> listAccountEntries=_constructResults.ListAccountEntries.Select(x => x.Copy()).ToList();
			if(!PaymentEdit.TryCreateIncomeTransfer(listAccountEntries,datePickerAsOf.Value,out PaymentEdit.IncomeTransferData results)) {
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(results.StringBuilderErrors.ToString().TrimEnd());
				msgBoxCopyPaste.Show();
				return;
			}
			if(results.HasInvalidSplits) {
				MsgBox.Show(this,"One or more transfers were not created due to 'Allow prepayments to providers' being turned off."
					+"\r\nPlease create them manually.");
			}
			CreatePaymentAndRefresh(results.ListSplitsCur,true);
			//Display any warning messages to the user.
			if(results.StringBuilderWarnings.Length > 0) {
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste("The following warnings happened during the income transfer process."
					+"\r\n"+results.StringBuilderWarnings.ToString());
				msgBoxCopyPaste.Show();
			}
		}

		private void butFIFO_Click(object sender, EventArgs e) {
			if(!IsValid()) {
				return;
			}
			try {
				PaymentEdit.IncomeTransferData incomeTransferData=PaymentEdit.GetIncomeTransferDataFIFO(_patient.PatNum,datePickerAsOf.Value,dateAsOf:datePickerAsOf.Value);
				CreatePaymentAndRefresh(incomeTransferData.ListSplitsCur,false);
				//Display any warning messages to the user.
				if(incomeTransferData.StringBuilderWarnings.Length > 0) {
					MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste("The following warnings happened during the income transfer process."
						+"\r\n"+incomeTransferData.StringBuilderWarnings.ToString());
					msgBoxCopyPaste.Show();
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem making a FIFO transfer. Please call support."),ex);
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			this.Close();
		}

	
	}
}