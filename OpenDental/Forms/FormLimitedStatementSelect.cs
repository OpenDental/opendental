using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormLimitedStatementSelect:FormODBase {
		private DataTable _tableAccount;
		private List<LimitedRow> _listLimitedRows=new List<LimitedRow>();
		///<summary>Used for reselecting rows after sorting by column or filling the grid on load etc.</summary>
		private Dictionary<AccountEntryType,List<long>> _dictSelectedRows;

		public List<long> ListSelectedPayClaimNums { get; private set; } = new List<long>();

		public List<long> ListSelectedAdjNums { get; private set; } = new List<long>();

		public List<long> ListSelectedPayNums { get; private set; } = new List<long>();

		public List<long> ListSelectedProcNums { get; private set; } = new List<long>();

		public List<long> ListSelectedPatNums { get; private set; } = new List<long>();
		public List<long> ListSelectedPayPlanChargeNums { get; private set; } = new List<long>();

		public FormLimitedStatementSelect(DataTable tableAccount,List<long> listPaymentClaimNums=null,List<long> listAdjustments=null,
			List<long> listPayNums=null,List<long> listProcNums=null,List<long> listPatNums=null,List<long> listPayPlanChargeNums=null)
		{
			InitializeComponent();
			InitializeLayoutManager();
			_tableAccount=tableAccount.Copy();
			ListSelectedPayClaimNums=listPaymentClaimNums??new List<long>();
			ListSelectedAdjNums=listAdjustments??new List<long>();
			ListSelectedPayNums=listPayNums??new List<long>();
			ListSelectedProcNums=listProcNums??new List<long>();
			ListSelectedPatNums=listPatNums??new List<long>();
			ListSelectedPayPlanChargeNums=listPayPlanChargeNums??new List<long>();
			Lan.F(this);
		}

		private void FormLimitedStatementSelect_Load(object sender,EventArgs e) {
			FillTransType();
			ConstructListFromTable();
			odDatePickerFrom.SetDateTime(_listLimitedRows
				.Where(x => x.DateTime!=DateTime.MinValue && IsRowPreSelected(x.AccountEntryType_,x.PrimaryKey))
				.Select(x => x.DateTime)
				.Concat(new[] { DateTime.Today.AddMonths(-1) }).Min());
			odDatePickerTo.SetDateTime(DateTime.Today);
			SetFilterControlsAndAction(() => FillGrid(),listBoxTransTypes,odDatePickerFrom,odDatePickerTo);
			_dictSelectedRows=_listLimitedRows
				.Where(x => IsRowPreSelected(x.AccountEntryType_,x.PrimaryKey))
				.GroupBy(x => x.AccountEntryType_,x => x.PrimaryKey)
				.ToDictionary(x => x.Key,x => x.Distinct().ToList());
			FillGrid();
		}

		private void FillTransType() {
			listBoxTransTypes.Items.Clear();
			listBoxTransTypes.Items.Add(Lan.g(this,"Adjustment"),AccountEntryType.Adjustment);
			listBoxTransTypes.SetSelected(listBoxTransTypes.Items.Count-1,true);
			listBoxTransTypes.Items.Add(Lan.g(this,"Claim Payment"),AccountEntryType.ClaimPayment);
			listBoxTransTypes.SetSelected(listBoxTransTypes.Items.Count-1,true);
			listBoxTransTypes.Items.Add(Lan.g(this,"Payment"),AccountEntryType.Payment);
			listBoxTransTypes.SetSelected(listBoxTransTypes.Items.Count-1,true);
			listBoxTransTypes.Items.Add(Lan.g(this,"Pay Plan Charge"),AccountEntryType.PayPlanCharge);
			listBoxTransTypes.SetSelected(listBoxTransTypes.Items.Count-1,true);
			listBoxTransTypes.Items.Add(Lan.g(this,"Procedure"),AccountEntryType.Procedure);
			listBoxTransTypes.SetSelected(listBoxTransTypes.Items.Count-1,true);
		}

		private void ConstructListFromTable() {
			foreach(DataRow tableRow in _tableAccount.Rows) {
				LimitedRow limitedRow=new LimitedRow();
				limitedRow.PatNum=PIn.Long(tableRow["PatNum"].ToString());
				limitedRow.PatientName=tableRow["patient"].ToString();
				limitedRow.DateTime=PIn.DateT(tableRow["DateTime"].ToString());
				limitedRow.Description=tableRow["description"].ToString();
				limitedRow.ProcCode=tableRow["ProcCode"].ToString();//isn't just a proc code. Can be "Claim" etc...
				limitedRow.Charges=tableRow["charges"].ToString();
				limitedRow.Credits=tableRow["credits"].ToString();
				limitedRow.ProvName=tableRow["prov"].ToString();
				limitedRow.Tooth=Tooth.Display(tableRow["ToothNum"].ToString());
				limitedRow.ColorText=Color.FromArgb(PIn.Int(tableRow["colorText"].ToString()));
				if(PrefC.HasClinicsEnabled) {
					limitedRow.ClinicNum=PIn.Long(tableRow["ClinicNum"].ToString());
				}
				if(tableRow["AdjNum"].ToString()!="0") {
					limitedRow.PrimaryKey=PIn.Long(tableRow["AdjNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Adjustment;
				}
				else if(tableRow["ProcNum"].ToString()!="0") {
					limitedRow.PrimaryKey=PIn.Long(tableRow["ProcNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Procedure;
				}
				else if(tableRow["PayNum"].ToString()!="0") {
					limitedRow.PrimaryKey=PIn.Long(tableRow["PayNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Payment;
				}
				else if(tableRow["ClaimNum"].ToString()!="0") {
					//can mean that this is either a claim or a claim payment.
					//we really only care about claim payments, but we need procedure from the claim.
					limitedRow.PrimaryKey=PIn.Long(tableRow["ClaimNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Claim;
					if(tableRow["ClaimPaymentNum"].ToString()=="1") {
						limitedRow.AccountEntryType_=AccountEntryType.ClaimPayment;
					}		
				}
				else if(tableRow["PayPlanChargeNum"].ToString()!="0" && tableRow["credits"].ToString()=="") {//We only want debits.
					limitedRow.PrimaryKey=PIn.Long(tableRow["PayPlanChargeNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.PayPlanCharge;
				}
				else {
					//type is not one that is currently supported, skip it.
					continue;
				}
				limitedRow.ListProcsOnObject=tableRow["procsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				limitedRow.ListAdjustsOnObj=tableRow["adjustsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				limitedRow.ListPaymentsOnObj=tableRow["paymentsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				limitedRow.ProcNumLab=PIn.Long(tableRow["ProcNumLab"].ToString());
				_listLimitedRows.Add(limitedRow);
			}
		}

		private bool IsRowPreSelected(AccountEntryType type,long priKey) {
			return type==AccountEntryType.Adjustment && ListSelectedAdjNums.Contains(priKey)
				|| type==AccountEntryType.Procedure && ListSelectedProcNums.Contains(priKey)
				|| type==AccountEntryType.Payment && ListSelectedPayNums.Contains(priKey)
				|| type==AccountEntryType.ClaimPayment && ListSelectedPayClaimNums.Contains(priKey)//AccountEntryType.Claim won't be in the grid
				|| type==AccountEntryType.PayPlanCharge && ListSelectedPayPlanChargeNums.Contains(priKey);
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.AccountModule);
			if(!PrefC.HasClinicsEnabled) {
				//remove clinics from displayfields if clinics are disabled
				listDisplayFields.RemoveAll(x => x.InternalName.ToLower().Contains("clinic"));
			}
			listDisplayFields.RemoveAll(x => x.InternalName.In("Abbr","Balance","Signed"));
			HorizontalAlignment align;
			GridSortingStrategy sort;
			for(int i=0;i<listDisplayFields.Count;i++) {
				align=HorizontalAlignment.Left;
				sort=GridSortingStrategy.StringCompare;
				if(listDisplayFields[i].InternalName.In("Charges","Credits")) {
					align=HorizontalAlignment.Right;
					sort=GridSortingStrategy.AmountParse;
				}
				if(listDisplayFields[i].InternalName=="Tth") {
					sort=GridSortingStrategy.ToothNumberParse;
				}
				if(listDisplayFields[i].InternalName=="Date") {
					sort=GridSortingStrategy.DateParse;
				}
				gridMain.Columns.Add(new GridColumn(listDisplayFields[i].Description==""?listDisplayFields[i].InternalName:listDisplayFields[i].Description,
					listDisplayFields[i].ColumnWidth,align,sort));
			}
			if(gridMain.Columns.Sum(x => x.ColWidth) > gridMain.Width) {
				gridMain.HScrollVisible=true;
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listLimitedRows.Count;i++) {
				LimitedRow limitedRow=_listLimitedRows[i];
				if(!listBoxTransTypes.GetListSelected<AccountEntryType>().Contains(limitedRow.AccountEntryType_)) {
					continue;
				}
				DateTime date=limitedRow.DateTime.Date;
				if(date.Date<odDatePickerFrom.GetDateTime().Date || date.Date>odDatePickerTo.GetDateTime().Date) {
					continue;//do not add to grid if it is outside the filtered date range. 
				}
				row=new GridRow();
				for(int f=0;f<listDisplayFields.Count;f++) {
					switch(listDisplayFields[f].InternalName) {
						case "Date":
							row.Cells.Add(date.ToShortDateString());
							break;
						case "Patient":
							row.Cells.Add(limitedRow.PatientName);
							break;
						case "Prov":
							row.Cells.Add(limitedRow.ProvName);
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(limitedRow.ClinicNum));
							break;
						case "ClinicDesc":
							row.Cells.Add(Clinics.GetDesc(limitedRow.ClinicNum));
							break;
						case "Code":
							row.Cells.Add(limitedRow.ProcCode);
							break;
						case "Tth":
							row.Cells.Add(limitedRow.Tooth);
							break;
						case "Description":
							row.Cells.Add(limitedRow.Description);
							break;
						case "Charges":
							row.Cells.Add(limitedRow.Charges);
							break;
						case "Credits":
							row.Cells.Add(limitedRow.Credits);
							break;
						default:
							row.Cells.Add("");
							break;
					}
				}
				row.ColorText=limitedRow.ColorText;
				if(i==_listLimitedRows.Count-1 || limitedRow.DateTime.Date!=_listLimitedRows[i+1].DateTime.Date) {
					row.ColorLborder=Color.Black;
				}
				row.Tag=limitedRow;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				LimitedRow limitedRow=gridMain.ListGridRows[i].Tag as LimitedRow;
				gridMain.SetSelected(i,_dictSelectedRows.TryGetValue(limitedRow.AccountEntryType_,out List<long> listPriKeys) && listPriKeys.Contains(limitedRow.PrimaryKey));
			}
			//this will refresh _dictSelectedRows and add any associated trans to the previously selected rows
			SelectAssociatedTrans();
		}

		private void odDatePickerFrom_CalendarSelectionChanged(object sender,EventArgs e) {
			if(odDatePickerFrom.HideCalendarOnLeave && odDatePickerFrom.IsCalendarOpen) {
				odDatePickerFrom.ToggleCalendar();
			}
		}

		private void odDatePickerTo_CalendarSelectionChanged(object sender,EventArgs e) {
			if(odDatePickerTo.HideCalendarOnLeave && odDatePickerTo.IsCalendarOpen) {
				odDatePickerTo.ToggleCalendar();
			}
		}

		private void gridMain_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Left) {
				SelectAssociatedTrans();
			}
		}

		private void SelectAssociatedTrans() {
			List<LimitedRow> listLimitedRowsSelected=gridMain.SelectedTags<LimitedRow>();
			List<long> listSelectedPayClaimNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.ClaimPayment).Select(x => x.PrimaryKey).Distinct().ToList();
			List<long> listSelectedProcNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Procedure).Select(x => x.PrimaryKey).Distinct().ToList();
			List<long> listSelectedPayNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Payment).Select(x => x.PrimaryKey).Distinct().ToList();
			List<long> listSelectedAdjNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Adjustment).Select(x => x.PrimaryKey).Distinct().ToList();
			List<int> listSelectedIndices=gridMain.SelectedIndices.ToList();
			for(int i=0;i<listSelectedIndices.Count;i++) {
				LimitedRow limitedRowSelected=gridMain.ListGridRows[listSelectedIndices[i]].Tag as LimitedRow;
				if(limitedRowSelected.AccountEntryType_==AccountEntryType.ClaimPayment) {
					listSelectedProcNums.AddRange(limitedRowSelected.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)));
					List<LimitedRow> listLimitedRowsClaim=_listLimitedRows.FindAll(x => x.AccountEntryType_==AccountEntryType.Claim && listSelectedPayClaimNums.Contains(x.PrimaryKey));
					listSelectedProcNums.AddRange(listLimitedRowsClaim.SelectMany(x => x.ListProcsOnObject.Where(y => y>0 && !listSelectedProcNums.Contains(y))).Distinct());
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow limitedRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(limitedRow.AccountEntryType_==AccountEntryType.ClaimPayment && listSelectedPayClaimNums.Contains(limitedRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
							listSelectedProcNums.AddRange(limitedRow.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)).Distinct());
						}
					}
					listSelectedProcNums=listSelectedProcNums.Distinct().Where(x => x>0).ToList();
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow limitedRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(limitedRow.AccountEntryType_==AccountEntryType.Procedure && listSelectedProcNums.Any(x => limitedRow.PrimaryKey==x || limitedRow.ProcNumLab==x)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
					}
				}
				else if(limitedRowSelected.AccountEntryType_==AccountEntryType.Payment) {
					listSelectedProcNums.AddRange(limitedRowSelected.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)));
					listSelectedPayNums.AddRange(limitedRowSelected.ListPaymentsOnObj.Where(x => x>0 && !listSelectedPayNums.Contains(x)));
					listSelectedAdjNums.AddRange(limitedRowSelected.ListAdjustsOnObj.Where(x => x>0 && !listSelectedAdjNums.Contains(x)));
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow limitedRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(limitedRow.AccountEntryType_==AccountEntryType.Payment && listSelectedPayNums.Contains(limitedRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
							listSelectedProcNums.AddRange(limitedRow.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)));
							listSelectedPayNums.AddRange(limitedRow.ListPaymentsOnObj.Where(x => x>0 && !listSelectedPayNums.Contains(x)));
							listSelectedAdjNums.AddRange(limitedRow.ListAdjustsOnObj.Where(x => x>0 && !listSelectedAdjNums.Contains(x)));
						}
					}
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow limitedRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(limitedRow.AccountEntryType_==AccountEntryType.Procedure && listSelectedProcNums.Contains(limitedRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
						else if(limitedRow.AccountEntryType_==AccountEntryType.Payment && listSelectedPayNums.Contains(limitedRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
						else if(limitedRow.AccountEntryType_==AccountEntryType.Adjustment && listSelectedAdjNums.Contains(limitedRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
					}
				}
				else if(limitedRowSelected.AccountEntryType_==AccountEntryType.Adjustment && limitedRowSelected.ListProcsOnObject.Count==1) {
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow limitedRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(limitedRow.AccountEntryType_==AccountEntryType.Procedure && limitedRow.PrimaryKey==limitedRowSelected.ListProcsOnObject[0]) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
							break;
						}
					}
				}
				else if(limitedRowSelected.AccountEntryType_==AccountEntryType.Procedure || limitedRowSelected.ProcNumLab!=0) {
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow limitedRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(limitedRowSelected.ProcNumLab>0) {
							if(limitedRow.PrimaryKey==limitedRowSelected.ProcNumLab || limitedRow.ProcNumLab==limitedRowSelected.ProcNumLab) {
								if(!listSelectedIndices.Contains(j)) {
									listSelectedIndices.Add(j);
									gridMain.SetSelected(j,true);
								}
							}
						}
						else if(limitedRowSelected.PrimaryKey==limitedRow.ProcNumLab) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
					}
				}
			}
			_dictSelectedRows=gridMain.SelectedTags<LimitedRow>()
				.GroupBy(x => x.AccountEntryType_,x => x.PrimaryKey)
				.ToDictionary(x => x.Key,x => x.Distinct().ToList());
		}

		private void gridMain_ColumnSorted(object sender,EventArgs e) {
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				LimitedRow limitedRow=gridMain.ListGridRows[i].Tag as LimitedRow;
				gridMain.SetSelected(i,_dictSelectedRows.TryGetValue(limitedRow.AccountEntryType_,out List<long> listPriKeys) && listPriKeys.Contains(limitedRow.PrimaryKey));
			}
		}

		private void butAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
			_dictSelectedRows=gridMain.SelectedTags<LimitedRow>()
				.GroupBy(x => x.AccountEntryType_,x => x.PrimaryKey)
				.ToDictionary(x => x.Key,x => x.Distinct().ToList());
		}

		private void butToday_Click(object sender,EventArgs e) {
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				gridMain.SetSelected(i,(gridMain.ListGridRows[i].Tag as LimitedRow).DateTime.Date==DateTime.Today.Date);
			}
			SelectAssociatedTrans();
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"There are no transactions with today's date.");
			}
		}

		private void butNone_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
			_dictSelectedRows=new Dictionary<AccountEntryType,List<long>>();
		}

		private void butOK_Click(object sender,EventArgs e) {
			List<LimitedRow> listLimitedRowsSelected=gridMain.SelectedTags<LimitedRow>();
			if(listLimitedRowsSelected.Count==0) {
				MsgBox.Show(this,"Please select procedures, adjustments, payments, claim payments or payment plan charges first.");
				return;
			}
			ListSelectedPatNums=listLimitedRowsSelected.Select(x => x.PatNum).Distinct().ToList();
			ListSelectedAdjNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Adjustment).Select(x => x.PrimaryKey).Distinct().ToList();
			ListSelectedPayNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Payment).Select(x => x.PrimaryKey).Distinct().ToList();
			ListSelectedPayClaimNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.ClaimPayment).Select(x => x.PrimaryKey).Distinct().ToList();
			//get selected procs and any procs for the selected claimpayments and combine them into a single list of selected ProcNums
			ListSelectedProcNums=listLimitedRowsSelected
				.FindAll(x => x.AccountEntryType_==AccountEntryType.Procedure)
				.Select(x => x.PrimaryKey)
				.Concat(_listLimitedRows
					.FindAll(x => x.AccountEntryType_==AccountEntryType.Claim && ListSelectedPayClaimNums.Contains(x.PrimaryKey))
					.SelectMany(x => x.ListProcsOnObject)
				).Distinct().ToList();
			ListSelectedPayPlanChargeNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.PayPlanCharge).Select(x => x.PrimaryKey).Distinct().ToList();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		/// <summary>Class to represent items coming in from the account module grid.</summary>
		private class LimitedRow {
			//Can be paymentNum,adjustmentNum,procedureNum,claimNum,payplanNum. NOTE: ClaimPayments will hold the ClaimNum. 
			public long PrimaryKey;
			public AccountEntryType AccountEntryType_;
			//List of procedures attached to the object (if any)
			public List<long> ListProcsOnObject=new List<long>();
			public List<long> ListPaymentsOnObj=new List<long>();
			public List<long> ListAdjustsOnObj=new List<long>();
			public DateTime DateTime;
			public string Description;
			public string Charges;
			public string Credits;
			public string PatientName;
			public long PatNum;
			public string ProcCode;
			public string Tooth;
			public string ProvName;
			public long ClinicNum;
			public Color ColorText;
			public long ProcNumLab;
		}

	}

	
}