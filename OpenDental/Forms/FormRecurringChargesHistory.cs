using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormRecurringChargesHistory:FormODBase {
		///<summary>The list of charges that have most recently been fetched from the database.</summary>
		private List<RecurringCharge> _listRecurringCharges;
		///<summary>List of patients supplied by GetLimForPats</summary>
		private List<Patient> _listPatients;
		///<summary>The date range that was selected the last time the charges were fetched from the database.</summary>
		private DateRange _dateRangePrevious;
		///<summary>The clinic nums that were selected the last time the charges were fetched from the database.</summary>
		private List<long> _listClinicNumsPrevious;

		public FormRecurringChargesHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=contextMenu;
		}

		private void FormRecurringChargesHistory_Load(object sender,EventArgs e) {
			datePicker.SetDateTimeFrom(DateTime.Today.AddMonths(-1));
			datePicker.SetDateTimeTo(DateTime.Today);
			comboStatuses.Items.AddEnums<RecurringChargeStatus>();
			comboStatuses.SetAll(true);
			comboAutomated.Items.Add(Lans.g(this,"Automated and Manual"));
			comboAutomated.Items.Add(Lans.g(this,"Automated Only"));
			comboAutomated.Items.Add(Lans.g(this,"Manual Only"));
			comboAutomated.SelectedIndex=0;
			RefreshRecurringCharges();
			FillGrid();
		}

		///<summary>Gets recurring charges from the database.</summary>
		private void RefreshRecurringCharges() {
			Cursor=Cursors.WaitCursor;
			List<SQLWhere> listSQLWheres=new List<SQLWhere> {
				SQLWhere.CreateBetween(nameof(RecurringCharge.DateTimeCharge),datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo(),true)
			};
			if(PrefC.HasClinicsEnabled) {
				listSQLWheres.Add(SQLWhere.CreateIn(nameof(RecurringCharge.ClinicNum),comboClinics.ListClinicNumsSelected));
			}
			_listRecurringCharges=RecurringCharges.GetMany(listSQLWheres);
			_listPatients=Patients.GetLimForPats(_listRecurringCharges.Select(x => x.PatNum).ToList());
			_dateRangePrevious=new DateRange(datePicker.GetDateTimeFrom(),datePicker.GetDateTimeTo());
			_listClinicNumsPrevious=comboClinics.ListClinicNumsSelected;
			Cursor=Cursors.Default;
		}

		///<summary>Will refresh charges from the database if necessary.</summary>
		private void FillGrid() {
			if(!_dateRangePrevious.IsInRange(datePicker.GetDateTimeFrom())){
				RefreshRecurringCharges();
			}
			else if(!_dateRangePrevious.IsInRange(datePicker.GetDateTimeTo())) {
				RefreshRecurringCharges();
			}
			else if(comboClinics.ListClinicNumsSelected.Any(x => !_listClinicNumsPrevious.Contains(x))) {
				RefreshRecurringCharges();
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"PatNum"),55,GridSortingStrategy.AmountParse));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Name"),185));
			if(PrefC.HasClinicsEnabled) {
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Clinic"),65));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Date Charge"),135,HorizontalAlignment.Center,
				GridSortingStrategy.DateParse));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Charge Status"),90));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"User"),90));
			if(PrefC.HasClinicsEnabled){
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Family Bal"),70,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"PayPlan Due"),80,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Total Due"),65,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Repeat Amt"),75,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Charge Amt"),85,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
			}
			else {
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Family Bal"),85,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"PayPlan Due"),90,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Total Due"),80,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Repeat Amt"),90,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Charge Amt"),95,HorizontalAlignment.Right,
					GridSortingStrategy.AmountParse));
			}
			gridMain.ListGridRows.Clear();
			List<RecurringCharge> listRecurringCharges=_listRecurringCharges.OrderBy(x => x.DateTimeCharge).ToList();
			for(int i=0;i<listRecurringCharges.Count;i++) {
				bool isAutomated=(listRecurringCharges[i].UserNum==0);
				if(!datePicker.IsInDateRange(listRecurringCharges[i].DateTimeCharge)) {
					continue;
				}
				if(PrefC.HasClinicsEnabled && !comboClinics.ListClinicNumsSelected.Contains(listRecurringCharges[i].ClinicNum)){
					continue;
				}
				if(!comboStatuses.GetListSelected<RecurringChargeStatus>().Contains(listRecurringCharges[i].ChargeStatus)) {
					continue;
				}
				if(comboAutomated.SelectedIndex==1 && !isAutomated){
					continue;
				}
				if(comboAutomated.SelectedIndex==2 && isAutomated){
					continue;
				}
				GridRow row=new GridRow();
				row.Cells.Add(listRecurringCharges[i].PatNum.ToString());
				Patient patient = _listPatients.Find(x => x.PatNum == listRecurringCharges[i].PatNum);
				if(patient is null){
					row.Cells.Add(Lans.g(this,"UNKNOWN"));
				}
				else{
					row.Cells.Add(patient.GetNameFL());
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetFirstOrDefault(x => x.ClinicNum==listRecurringCharges[i].ClinicNum)?.Description??"");
				}
				row.Cells.Add(listRecurringCharges[i].DateTimeCharge.ToString());
				row.Cells.Add(Lans.g(this,listRecurringCharges[i].ChargeStatus.GetDescription()));
				row.Cells.Add(Userods.GetFirstOrDefault(x => x.UserNum==listRecurringCharges[i].UserNum)?.UserName??"");
				row.Cells.Add(listRecurringCharges[i].FamBal.ToString("c"));
				row.Cells.Add(listRecurringCharges[i].PayPlanDue.ToString("c"));
				row.Cells.Add(listRecurringCharges[i].TotalDue.ToString("c"));
				row.Cells.Add(listRecurringCharges[i].RepeatAmt.ToString("c"));
				row.Cells.Add(listRecurringCharges[i].ChargeAmt.ToString("c"));
				row.Tag=listRecurringCharges[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(gridMain.Columns.WidthAll() > gridMain.Width) {
				gridMain.HScrollVisible=true;
			}
		}

		private void FilterChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshRecurringCharges();
			FillGrid();
		}
		
		private void contextMenu_Popup(object sender,EventArgs e) {
			RecurringCharge recurringCharge=gridMain.SelectedTag<RecurringCharge>();
			if(recurringCharge==null) {
				return;
			}
			menuItemOpenPayment.Visible=(recurringCharge.PayNum!=0);
			menuItemViewError.Visible=(recurringCharge.ChargeStatus==RecurringChargeStatus.ChargeFailed || recurringCharge.ChargeStatus==RecurringChargeStatus.ChargeDeclined);
			menuItemDeletePending.Visible=(recurringCharge.ChargeStatus==RecurringChargeStatus.NotYetCharged);
		}
		
		private void menuItemGoTo_Click(object sender,EventArgs e) {
			RecurringCharge recurringCharge=gridMain.SelectedTag<RecurringCharge>();
			if(recurringCharge==null) {
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.AccountModule)) {
				return;
			}
			GotoModule.GotoAccount(recurringCharge.PatNum);
		}

		private void menuItemOpenPayment_Click(object sender,EventArgs e) {
			RecurringCharge recurringCharge=gridMain.SelectedTag<RecurringCharge>();
			if(recurringCharge==null) {
				return;
			}
			if(recurringCharge.PayNum==0) {
				return;
			}
			Payment payment=Payments.GetPayment(recurringCharge.PayNum);
			if(payment==null) {//The payment has been deleted
				MsgBox.Show(this,"This payment no longer exists.");
				return;
			}
			Patient patient=Patients.GetPat(payment.PatNum);
			Family family=Patients.GetFamily(patient.PatNum);
			using FormPayment formPayment=new FormPayment(patient,family,payment,false);
			formPayment.ShowDialog();
		}

		private void menuItemViewError_Click(object sender,EventArgs e) {
			RecurringCharge recurringCharge=gridMain.SelectedTag<RecurringCharge>();
			if(recurringCharge==null) {
				return;
			}
			new MsgBoxCopyPaste(recurringCharge.ErrorMsg).Show();
		}

		private void menuItemDeletePending_Click(object sender,EventArgs e) {
			RecurringCharge recurringCharge=gridMain.SelectedTag<RecurringCharge>();
			if(recurringCharge==null) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this pending recurring charge?"
				+"\r\n\r\nAnother user or service may be processing this card right now."))
			{
				return;
			}
			RecurringCharge reccuringChargeDb=RecurringCharges.GetOne(recurringCharge.RecurringChargeNum);
			if(reccuringChargeDb==null || reccuringChargeDb.ChargeStatus!=RecurringChargeStatus.NotYetCharged) {
				MsgBox.Show(this,"This recurring charge is no longer pending. Unable to delete.");
				return;
			}
			RecurringCharges.Delete(recurringCharge.RecurringChargeNum);
			RefreshRecurringCharges();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}