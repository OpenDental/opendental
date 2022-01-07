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
		/// <summary>A list of Patient.PatientNames, filled with _listCareCreditWebResponses. Gets refulled every time FillGrid() is invoked.</summary>
		private List<Patients.PatientName> _listPatientNames;
		///<summary>If null, we will get transactions for all patients. If set, we will only show transactions for the patient passed in.</summary>
		private Patient _patient;

		///<summary>Menuitem events are shared between the grids in each tab. This returns the grid based on the current tab selected.</summary>
		private GridOD getGrid() {
			if(tabControl.SelectedTab==tabTrans) { // transaction tab selected
				return gridMain;
			}
			if(tabControl.SelectedTab==tabQSBatchTrans) { //quickscreen batch tab selected
				return gridQSBatchTrans;
			}
			return gridError;
		}

		public FormCareCreditTransactions(Patient patient=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
		}

		private void FormCareCreditTransactions_Load(object sender,EventArgs e) {
			if(_patient!=null) {
				Text+=$" - {_patient.GetNameFL()}";
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
			if(getGrid().SelectedIndices.Length<=0) {
				e.Cancel=true;
				return;
			}
			menuItemGoTo.Available=(_patient?.PatNum??0)==0;
			if(tabControl.SelectedTab==tabTrans) { 
				CareCreditWebResponse careCreditWebResponseSelected=getGrid().SelectedTag<CareCreditWebResponse>();
				acknowledgeToolStripMenuItem.Available=false; 
				viewErrorMessageMenuItem.Available=false;
				openPaymentToolStripMenuItem.Available=careCreditWebResponseSelected.PayNum!=0;
				if(careCreditWebResponseSelected.TransType==CareCreditTransType.Purchase) {
					processRefundToolStripMenuItem.Available=true;
				}
				else {
					processRefundToolStripMenuItem.Available=false;
				}
			}
			else if(tabControl.SelectedTab==tabQSBatchTrans) { 
				acknowledgeToolStripMenuItem.Available=false;
				openPaymentToolStripMenuItem.Available=false;
				processRefundToolStripMenuItem.Available=false;
				viewErrorMessageMenuItem.Available=false;
			}
			else if(tabControl.SelectedTab==tabErrors) { 
				acknowledgeToolStripMenuItem.Available=true;
				viewErrorMessageMenuItem.Available=true;
				openPaymentToolStripMenuItem.Available=false;
				processRefundToolStripMenuItem.Available=false;
				List<CareCreditWebResponse> listCareCreditWebResponseSelected=getGrid().SelectedTags<CareCreditWebResponse>();
				if(listCareCreditWebResponseSelected.All(x => x.ProcessingStatus==CareCreditWebStatus.ErrorAcknowledged)) {
					acknowledgeToolStripMenuItem.Enabled=false;
				}
			}
		}

		private void acknowledgeToolStripMenuItem_Click(object sender,EventArgs e) {
			AcknowledgeErrors();
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(getGrid().SelectedIndices.Length<1 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			GotoModule.GotoAccount(getGrid().SelectedTag<CareCreditWebResponse>().PatNum);
		}

		private void openPaymentToolStripMenuItem_Click(object sender,EventArgs e) {
			if(getGrid().SelectedIndices.Length<1) {
				return;
			}
			Payment payment=Payments.GetPayment(getGrid().SelectedTag<CareCreditWebResponse>().PayNum);
			if(payment==null) {//The payment has been deleted
				MsgBox.Show(this,"This payment no longer exists.");
				return;
			}
			Patient patient=Patients.GetPat(payment.PatNum);
			Family family=Patients.GetFamily(patient.PatNum);
			using FormPayment formPayment=new FormPayment(patient,family,payment,false);
			formPayment.ShowDialog();
			FillGrids();
		}

		private void viewErrorMessage_Click(object sender,EventArgs e) {
			CareCreditWebResponse careCreditWebResponseError=getGrid().SelectedTag<CareCreditWebResponse>();
			if(careCreditWebResponseError==null) {
				return;
			}
			new MsgBoxCopyPaste(careCreditWebResponseError.LastResponseStr).Show();
		}

		private void processReturnToolStripMenuItem_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today)) {//CreateFromPrefill(...) date defaults to MiscData.GetNowDateTime(); 
				return;
			}
			if(getGrid().SelectedIndices.Length<1) {
				return;
			}
			CareCreditWebResponse careCreditWebResponseSelected=getGrid().SelectedTag<CareCreditWebResponse>();
			CareCreditL.RefundTransaction(careCreditWebResponseSelected);
			FillGrids();
		}

		private void butAck_Click(object sender,EventArgs e) {
			AcknowledgeErrors();
		}

		private void butAll_Click(object sender,EventArgs e) {
			if(getGrid()!=gridError) {
				return;
			}
			getGrid().SetAll(true);
		}

		private void butNone_Click(object sender,EventArgs e) {
			if(getGrid()!=gridError) {
				return;
			}
			getGrid().SetAll(false);
		}

		private void butButAllQSTrans_Click(object sender,EventArgs e) {
			if(getGrid()!=gridQSBatchTrans) {
				return;
			}
			getGrid().SetAll(true);
		}

		private void butNoneQSBatch_Click(object sender,EventArgs e) {
			if(getGrid()!=gridQSBatchTrans) {
				return;
			}
			getGrid().SetAll(false);
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
			GotoModule.GotoAccount(getGrid().SelectedTag<CareCreditWebResponse>().PatNum);
		}

		private void tabControl_SelectedIndexChanged(object sender,EventArgs e) {
			ShowFilters();
		}

		private void butProcessBatchQS_Click(object sender,EventArgs e) {
			if(getGrid()!=gridQSBatchTrans) {
				return;
			}
			List<CareCreditWebResponse> listCareCreditWebResponseExpiredBatches=getGrid().SelectedTags<CareCreditWebResponse>()
					.FindAll(x => x.ProcessingStatus==CareCreditWebStatus.ExpiredBatch);
			if(listCareCreditWebResponseExpiredBatches.IsNullOrEmpty()) {
				MsgBox.Show(this,"No expired batches are selected to reprocess.");
				return;
			}
			for(int i=0;i<listCareCreditWebResponseExpiredBatches.Count;i++) {
				listCareCreditWebResponseExpiredBatches[i].ProcessingStatus=CareCreditWebStatus.Pending;
				listCareCreditWebResponseExpiredBatches[i].DateTimePending=DateTime.Now;
				CareCreditWebResponses.Update(listCareCreditWebResponseExpiredBatches[i]);
			}
			FillGrids();
			MsgBox.Show(this,"Expired batches reprocessed.");
		}

		private void AcknowledgeErrors() {
			if(getGrid()!=gridError) {
				return;
			}
			List<CareCreditWebResponse> listCareCreditWebResponses=getGrid().SelectedTags<CareCreditWebResponse>();
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
			long patNum=_patient?.PatNum??0;
			List<CareCreditWebResponse> listCareCreditWebResponses=CareCreditWebResponses.GetApprovedTransactions(comboClinic.ListSelectedClinicNums,
				dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),patNum);
			List<long> listPatNums=listCareCreditWebResponses.Select(x => x.PatNum).ToList();
			_listPatientNames=Patients.GetPatientNameList(listPatNums).Distinct().ToList();
			listCareCreditWebResponses=listCareCreditWebResponses.OrderBy(x => _listPatientNames.Find(y => y.PatNum == x.PatNum).Name).ToList();
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
			for(int i = 0;i<listCareCreditWebResponses.Count;++i) {
				GridRow row=new GridRow();
				row.Cells.Add(_listPatientNames.Find(x => x.PatNum == listCareCreditWebResponses[i].PatNum).Name);
				row.Cells.Add(listCareCreditWebResponses[i].Amount.ToString("f"));
				row.Cells.Add(listCareCreditWebResponses[i].DateTimeCompleted.ToShortDateString());
				row.Cells.Add(listCareCreditWebResponses[i].TransType.GetDescription());
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(listCareCreditWebResponses[i].ClinicNum);
					row.Cells.Add(string.IsNullOrEmpty(clinicAbbr) ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				row.Cells.Add(listCareCreditWebResponses[i].RefNumber);
				row.Tag=listCareCreditWebResponses[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridQSBatchTrans() {
			long patNum=_patient?.PatNum??0;
			List<CareCreditWebResponse> listCareCreditWebResponseQsBatchTrans=CareCreditWebResponses.GetBatchQSTransactions(comboClinic.ListSelectedClinicNums,
				comboStatuses.GetListSelected<CareCreditWebStatus>(),dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),patNum);
			List<long> listPatNums=listCareCreditWebResponseQsBatchTrans.Select(x => x.PatNum).ToList();
			_listPatientNames=Patients.GetPatientNameList(listPatNums).Distinct().ToList();
			listCareCreditWebResponseQsBatchTrans=listCareCreditWebResponseQsBatchTrans
				.OrderBy(x => x.DateTimeEntry)
				.ThenBy(x => _listPatientNames.Find(y => y.PatNum == x.PatNum).Name)
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
			for(int i = 0;i<listCareCreditWebResponseQsBatchTrans.Count;++i) {
				GridRow row=new GridRow();
				row.Cells.Add(listCareCreditWebResponseQsBatchTrans[i].DateTimeEntry.ToShortDateString());
				row.Cells.Add(_listPatientNames.Find(x => x.PatNum == listCareCreditWebResponseQsBatchTrans[i].PatNum).Name);
				DateTime dateProcessed=listCareCreditWebResponseQsBatchTrans[i].GetDateTimeForProcessingStatus();
				string dateProcessedStr=dateProcessed.ToShortDateString();
				if(dateProcessed.Year<1880) {
					dateProcessedStr="";
				}
				row.Cells.Add(dateProcessedStr);
				row.Cells.Add(listCareCreditWebResponseQsBatchTrans[i].ProcessingStatus.ToString());
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(listCareCreditWebResponseQsBatchTrans[i].ClinicNum);
					row.Cells.Add(string.IsNullOrEmpty(clinicAbbr) ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				row.Cells.Add(listCareCreditWebResponseQsBatchTrans[i].RefNumber);
				row.Tag=listCareCreditWebResponseQsBatchTrans[i];
				gridQSBatchTrans.ListGridRows.Add(row);
			}
			gridQSBatchTrans.EndUpdate();
		}

		private void FillGridError() {
			long patNum=_patient?.PatNum??0;
			List<CareCreditWebResponse> listCareCreditWebResponsesErrors=CareCreditWebResponses.GetBatchErrors(comboClinic.ListSelectedClinicNums,
				dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo(),checkIncludeAck.Checked,patNum);
			tabErrors.Text=$"Errors({listCareCreditWebResponsesErrors.Count})";
			List<long> listPatNumsSelected=listCareCreditWebResponsesErrors.Select(x => x.PatNum).ToList();
			_listPatientNames=Patients.GetPatientNameList(listPatNumsSelected).Distinct().ToList();
			listCareCreditWebResponsesErrors=listCareCreditWebResponsesErrors.OrderBy(x => _listPatientNames.Find(y => y.PatNum == x.PatNum).Name).ToList();
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
			for(int i = 0;i<listCareCreditWebResponsesErrors.Count;++i) {
				GridRow row=new GridRow();
				row.Cells.Add(_listPatientNames.Find(x => x.PatNum == listCareCreditWebResponsesErrors[i].PatNum).Name);
				row.Cells.Add(listCareCreditWebResponsesErrors[i].DateTimeLastError.ToShortDateString());
				if(checkIncludeAck.Checked) {
					row.Cells.Add(listCareCreditWebResponsesErrors[i].DateTimeCompleted.ToShortDateString());
				}
				if(PrefC.HasClinicsEnabled) {
					string clinicAbbr=Clinics.GetAbbr(listCareCreditWebResponsesErrors[i].ClinicNum);
					row.Cells.Add(string.IsNullOrEmpty(clinicAbbr) ? Lan.g(this,"Unassigned") : clinicAbbr);
				}
				CareCreditQSBatchResponse careCreditQSBatchResponse=CareCredit.GetBatchResponse(listCareCreditWebResponsesErrors[i]);
				string descript="";
				string detail="";
				if(careCreditQSBatchResponse!=null) {
					descript=careCreditQSBatchResponse.ResponseDesc;
					detail=careCreditQSBatchResponse.Parameter;
				}
				row.Cells.Add(listCareCreditWebResponsesErrors[i].ProcessingStatus.GetDescription());
				row.Cells.Add(descript);
				row.Cells.Add(detail);
				row.Tag=listCareCreditWebResponsesErrors[i];
				gridError.ListGridRows.Add(row);
			}
			gridError.EndUpdate();
		}

		private void ShowFilters() {
			if(tabControl.SelectedTab==tabErrors) { 
				checkIncludeAck.Visible=true;
			}
			else {
				checkIncludeAck.Visible=false;
			}
			bool isQuickScreenBatchTab=tabControl.SelectedTab==tabQSBatchTrans;
			comboStatuses.Visible=isQuickScreenBatchTab;
			labelStatus.Visible=isQuickScreenBatchTab;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

	}
}