using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCareCreditTransactions:FormODBase {
		///<summary>A dictionary of every patient name in _listCareCreditWebResponses.  Gets refilled every time FillGrid() is invoked.</summary>
		private Dictionary<long,string> _dictPatientNames=new Dictionary<long,string>();
		///<summary>If null, we will get transactions for all patients. If set, we will only show transactions for the patient passed in.</summary>
		private Patient _patCur;
		
		///<summary>Menuitem events are shared between the grids in each tab. This returns the grid based on the current tab selected.</summary>
		private GridOD _gridCur {
			get {
				if(IsTransactionTab) {
					return gridMain;
				}
				if(IsQuickScreenBatchTab) {
					return gridQSBatchTrans;
				}
				return gridError;
			}
		}

		///<summary>Returns true if the current Tab is transactions.</summary>
		private bool IsTransactionTab => tabControl.SelectedTab==tabTrans;

		///<summary>Returns true if the current Tab is error.</summary>
		private bool IsErrorTab => tabControl.SelectedTab==tabErrors;

		///<summary>Returns true if the current Tab is quickscreen batch transactions.</summary>
		private bool IsQuickScreenBatchTab => tabControl.SelectedTab==tabQSBatchTrans;

		private long PatNumCur => _patCur?.PatNum??0;

		public FormCareCreditTransactions(Patient pat=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=pat;
		}

		private void FormCareCreditTransactions_Load(object sender,EventArgs e) {
			if(_patCur!=null) {
				Text+=$" - {_patCur.GetNameFL()}";
			}
			SetFilterControlsAndAction(() => { RefreshData(); }
				,dateRangePicker,comboClinic,checkIncludeAck,comboStatuses
			);
			dateRangePicker.SetDateTimeFrom(DateTime.Now);
			dateRangePicker.SetDateTimeTo(DateTime.Now);
			comboStatuses.Items.AddList(CareCreditWebResponses.ListQSBatchTransactions,x => x.ToString());
			comboStatuses.SetAll(true);
			ShowFilters();
			FillGrids();
		}

		private void contextMenu_Opening(object sender,System.ComponentModel.CancelEventArgs e) {
			if(_gridCur.SelectedIndices.Length<=0) {
				e.Cancel=true;
				return;
			}
			menuItemGoTo.Available=(PatNumCur==0);
			if(IsTransactionTab) {
				CareCreditWebResponse resSelected=_gridCur.SelectedTag<CareCreditWebResponse>();
				acknowledgeToolStripMenuItem.Available=false; 
				viewErrorMessageMenuItem.Available=false;
				openPaymentToolStripMenuItem.Available=resSelected.PayNum!=0;
				if(resSelected.TransType==CareCreditTransType.Purchase) {
					processRefundToolStripMenuItem.Available=true;
				}
				else {
					processRefundToolStripMenuItem.Available=false;
				}
			}
			else if(IsQuickScreenBatchTab) {
				acknowledgeToolStripMenuItem.Available=false;
				openPaymentToolStripMenuItem.Available=false;
				processRefundToolStripMenuItem.Available=false;
				viewErrorMessageMenuItem.Available=false;
			}
			else if(IsErrorTab) {
				acknowledgeToolStripMenuItem.Available=true;
				viewErrorMessageMenuItem.Available=true;
				openPaymentToolStripMenuItem.Available=false;
				processRefundToolStripMenuItem.Available=false;
				List<CareCreditWebResponse> listCareCreditWebResSelected=_gridCur.SelectedTags<CareCreditWebResponse>();
				if(listCareCreditWebResSelected.All(x => x.ProcessingStatus==CareCreditWebStatus.ErrorAcknowledged)) {
					acknowledgeToolStripMenuItem.Enabled=false;
				}
			}
		}

		private void acknowledgeToolStripMenuItem_Click(object sender,EventArgs e) {
			AcknowledgeErrors();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(_gridCur.SelectedIndices.Length<1 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			GotoModule.GotoAccount(_gridCur.SelectedTag<CareCreditWebResponse>().PatNum);
		}

		private void openPaymentToolStripMenuItem_Click(object sender,EventArgs e) {
			if(_gridCur.SelectedIndices.Length<1) {
				return;
			}
			Payment pay=Payments.GetPayment(_gridCur.SelectedTag<CareCreditWebResponse>().PayNum);
			if(pay==null) {//The payment has been deleted
				MsgBox.Show(this,"This payment no longer exists.");
				return;
			}
			Patient pat=Patients.GetPat(pay.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			using FormPayment FormP=new FormPayment(pat,fam,pay,false);
			FormP.ShowDialog();
			FillGrids();
		}

		private void viewErrorMessage_Click(object sender,EventArgs e) {
			CareCreditWebResponse ccError=_gridCur.SelectedTag<CareCreditWebResponse>();
			if(ccError==null) {
				return;
			}
			new MsgBoxCopyPaste(ccError.LastResponseStr).Show();
		}

		private void processReturnToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today)) {//CreateFromPrefill(...) date defaults to MiscData.GetNowDateTime(); 
				return;
			}
			if(_gridCur.SelectedIndices.Length<1) {
				return;
			}
			CareCreditWebResponse webResponseSelected=_gridCur.SelectedTag<CareCreditWebResponse>();
			CareCreditL.RefundTransaction(webResponseSelected);
			FillGrids();
		}

		private void butAck_Click(object sender,EventArgs e) {
			AcknowledgeErrors();
		}

		private void butAll_Click(object sender,EventArgs e) {
			if(_gridCur!=gridError) {
				return;
			}
			_gridCur.SetAll(true);
		}

		private void butNone_Click(object sender,EventArgs e) {
			if(_gridCur!=gridError) {
				return;
			}
			_gridCur.SetAll(false);
		}

		private void butButAllQSTrans_Click(object sender,EventArgs e) {
			if(_gridCur!=gridQSBatchTrans) {
				return;
			}
			_gridCur.SetAll(true);
		}

		private void butNoneQSBatch_Click(object sender,EventArgs e) {
			if(_gridCur!=gridQSBatchTrans) {
				return;
			}
			_gridCur.SetAll(false);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshData();
		}

		private void RefreshData() {
			if(dateRangePicker.GetDateTimeFrom().Date>dateRangePicker.GetDateTimeTo().Date) {
				dateRangePicker.SetDateTimeFrom(DateTime.Now);
			}
			FillGrids();
		}

		private void gridCur_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row<0 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			GotoModule.GotoAccount(_gridCur.SelectedTag<CareCreditWebResponse>().PatNum);
		}

		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			ShowFilters();
		}

		private void butProcessBatchQS_Click(object sender,EventArgs e) {
			if(_gridCur!=gridQSBatchTrans) {
				return;
			}
			List<CareCreditWebResponse> listExpiredBatches=_gridCur.SelectedTags<CareCreditWebResponse>()
					.FindAll(x => x.ProcessingStatus==CareCreditWebStatus.ExpiredBatch);
			if(listExpiredBatches.IsNullOrEmpty()) {
				MsgBox.Show(this,"No expired batches are selected to reprocess.");
				return;
			}
			for(int i=0;i<listExpiredBatches.Count;i++) {
				listExpiredBatches[i].ProcessingStatus=CareCreditWebStatus.Pending;
				listExpiredBatches[i].DateTimePending=DateTime.Now;
				CareCreditWebResponses.Update(listExpiredBatches[i]);
			}
			FillGrids();
			MsgBox.Show(this,"Expired batches reprocessed.");
		}

		private void AcknowledgeErrors() {
			if(_gridCur!=gridError) {
				return;
			}
			List<CareCreditWebResponse> listCareCreditWebResponses=_gridCur.SelectedTags<CareCreditWebResponse>();
			if(listCareCreditWebResponses.IsNullOrEmpty()) {
				MsgBox.Show(this,"No items selected.");
				return;
			}
			for(int i=0;i<listCareCreditWebResponses.Count;i++) {
				if(listCareCreditWebResponses[i].ProcessingStatus==CareCreditWebStatus.ErrorAcknowledged) {
					continue;
				}
				CareCreditWebResponses.UpdateProcessingWebStatus(listCareCreditWebResponses[i],CareCreditWebStatus.ErrorAcknowledged);
			}
			FillGrids();
		}

		private void FillGrids() {
			FillGridMain();
			FillGridQSBatchTrans();
			FillGridError();
		}

		private void FillGridMain() {
			List<CareCreditWebResponse> listCareCreditWebResponses=CareCreditWebResponses.GetApprovedTransactions(comboClinic.ListSelectedClinicNums,
				dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),PatNumCur);
			List<long> listPatNums=listCareCreditWebResponses.Select(x => x.PatNum).ToList();
			if(listPatNums.Any(x => !_dictPatientNames.ContainsKey(x))) {
				Patients.GetPatientNames(listPatNums).ForEach(x => _dictPatientNames[x.Key]=x.Value);
			}
			listCareCreditWebResponses=listCareCreditWebResponses.OrderBy(x => _dictPatientNames[x.PatNum]).ToList();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),150));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Amount"),80,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date"),100));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Tran Type"),120));
			if(PrefC.HasClinicsEnabled) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Clinic"),120));
			}
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Ref ID"),110));
			gridMain.ListGridRows.Clear();
			foreach(CareCreditWebResponse webRes in listCareCreditWebResponses) {
				GridRow row=new GridRow();
				row.Cells.Add(_dictPatientNames[webRes.PatNum]);
				row.Cells.Add(webRes.Amount.ToString("f"));
				row.Cells.Add(webRes.DateTimeCompleted.ToShortDateString());
				row.Cells.Add(webRes.TransType.GetDescription());
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(webRes.ClinicNum);
					row.Cells.Add(string.IsNullOrEmpty(clinicAbbr) ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				row.Cells.Add(webRes.RefNumber);
				row.Tag=webRes;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridQSBatchTrans() {
			List<CareCreditWebResponse> listQsBatchTrans=CareCreditWebResponses.GetBatchQSTransactions(comboClinic.ListSelectedClinicNums,
				comboStatuses.GetListSelected<CareCreditWebStatus>(),dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),PatNumCur);
			List<long> listPatNums=listQsBatchTrans.Select(x => x.PatNum).ToList();
			if(listPatNums.Any(x => !_dictPatientNames.ContainsKey(x))) {
				Patients.GetPatientNames(listPatNums).ForEach(x => _dictPatientNames[x.Key]=x.Value);
			}
			listQsBatchTrans=listQsBatchTrans
				.OrderBy(x => x.DateTimeEntry)
				.ThenBy(x => _dictPatientNames[x.PatNum])
				.ThenBy(x => x.ProcessingStatus).ToList();
			gridQSBatchTrans.BeginUpdate();
			gridQSBatchTrans.ListGridColumns.Clear();
			gridQSBatchTrans.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date"),90,HorizontalAlignment.Right));
			gridQSBatchTrans.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),175));
			gridQSBatchTrans.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date Processed"),90,HorizontalAlignment.Right));
			gridQSBatchTrans.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),200));
			if(PrefC.HasClinicsEnabled) {
				gridQSBatchTrans.ListGridColumns.Add(new GridColumn(Lan.g(this,"Clinic"),120));
			}
			gridQSBatchTrans.ListGridColumns.Add(new GridColumn(Lan.g(this,"Ref ID"),70));
			gridQSBatchTrans.ListGridRows.Clear();
			foreach(CareCreditWebResponse webRes in listQsBatchTrans) {
				GridRow row=new GridRow();
				row.Cells.Add(webRes.DateTimeEntry.ToShortDateString());
				row.Cells.Add(_dictPatientNames[webRes.PatNum]);
				DateTime dateProcessed=webRes.GetDateTimeForProcessingStatus();
				string dateProcessedStr=dateProcessed.ToShortDateString();
				if(dateProcessed.Year<1880) {
					dateProcessedStr="";
				}
				row.Cells.Add(dateProcessedStr);
				row.Cells.Add(webRes.ProcessingStatus.ToString());
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(webRes.ClinicNum);
					row.Cells.Add(string.IsNullOrEmpty(clinicAbbr) ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				row.Cells.Add(webRes.RefNumber);
				row.Tag=webRes;
				gridQSBatchTrans.ListGridRows.Add(row);
			}
			gridQSBatchTrans.EndUpdate();
		}

		private void FillGridError() {
			List<CareCreditWebResponse> listCareCreditWebResponsesErrors=CareCreditWebResponses.GetBatchErrors(comboClinic.ListSelectedClinicNums,
				dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),checkIncludeAck.Checked,PatNumCur);
			tabErrors.Text=$"Errors({listCareCreditWebResponsesErrors.Count})";
			List<long> listPatNumsSelected=listCareCreditWebResponsesErrors.Select(x => x.PatNum).ToList();
			if(listPatNumsSelected.Any(x => !_dictPatientNames.ContainsKey(x))) {
				Patients.GetPatientNames(listPatNumsSelected).ForEach(x => _dictPatientNames[x.Key]=x.Value);
			}
			listCareCreditWebResponsesErrors=listCareCreditWebResponsesErrors.OrderBy(x => _dictPatientNames[x.PatNum]).ToList();
			gridError.BeginUpdate();
			gridError.ListGridColumns.Clear();
			gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),120));
			gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date"),80));
			if(checkIncludeAck.Checked) {
				gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Date Ack"),80));
			}
			if(PrefC.HasClinicsEnabled) {
				gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Clinic"),100));
			}
			gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),110));
			gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Description"),170));
			gridError.ListGridColumns.Add(new GridColumn(Lan.g(this,"Parameter"),110));
			gridError.ListGridRows.Clear();
			foreach(CareCreditWebResponse webRes in listCareCreditWebResponsesErrors) {
				GridRow row=new GridRow();
				row.Cells.Add(_dictPatientNames[webRes.PatNum]);
				row.Cells.Add(webRes.DateTimeLastError.ToShortDateString());
				if(checkIncludeAck.Checked) {
					row.Cells.Add(webRes.DateTimeCompleted.ToShortDateString());
				}
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(webRes.ClinicNum);
					row.Cells.Add(string.IsNullOrEmpty(clinicAbbr) ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				CareCreditQSBatchResponse batchRes=CareCredit.GetBatchResponse(webRes);
				string descript="";
				string detail="";
				if(batchRes!=null) {
					descript=batchRes.ResponseDesc;
					detail=batchRes.Parameter;
				}
				row.Cells.Add(webRes.ProcessingStatus.GetDescription());
				row.Cells.Add(descript);
				row.Cells.Add(detail);
				row.Tag=webRes;
				gridError.ListGridRows.Add(row);
			}
			gridError.EndUpdate();
		}

		private void ShowFilters() {
			if(IsErrorTab) {
				checkIncludeAck.Visible=true;
			}
			else {
				checkIncludeAck.Visible=false;
			}
			comboStatuses.Visible=IsQuickScreenBatchTab;
			labelStatus.Visible=IsQuickScreenBatchTab;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

	}
}