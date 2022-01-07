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

		public FormLimitedStatementSelect(DataTable accountTable,List<long> listPaymentClaimNums=null,List<long> listAdjustments=null,
			List<long> listPayNums=null,List<long> listProcNums=null,List<long> listPatNums=null)
		{
			InitializeComponent();
			InitializeLayoutManager();
			_tableAccount=accountTable.Copy();
			ListSelectedPayClaimNums=listPaymentClaimNums??new List<long>();
			ListSelectedAdjNums=listAdjustments??new List<long>();
			ListSelectedPayNums=listPayNums??new List<long>();
			ListSelectedProcNums=listProcNums??new List<long>();
			ListSelectedPatNums=listPatNums??new List<long>();
			Lan.F(this);
		}

		private void FormLimitedStatementSelect_Load(object sender,EventArgs e) {
			FillTransType();
			ConstructListFromTable();
			odDatePickerFrom.SetDateTime(_listLimitedRows
				.Where(x => x.DateTime!=DateTime.MinValue && IsRowPreSelected(x.Type,x.PrimaryKey))
				.Select(x => x.DateTime)
				.Concat(new[] { DateTime.Today.AddMonths(-1) }).Min());
			odDatePickerTo.SetDateTime(DateTime.Today);
			SetFilterControlsAndAction(() => FillGrid(),listBoxTransTypes,odDatePickerFrom,odDatePickerTo);
			_dictSelectedRows=_listLimitedRows
				.Where(x => IsRowPreSelected(x.Type,x.PrimaryKey))
				.GroupBy(x => x.Type,x => x.PrimaryKey)
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
			listBoxTransTypes.Items.Add(Lan.g(this,"Procedure"),AccountEntryType.Procedure);
			listBoxTransTypes.SetSelected(listBoxTransTypes.Items.Count-1,true);
			//add for payplans later
		}

		private void ConstructListFromTable() {
			foreach(DataRow tableRow in _tableAccount.Rows) {
				LimitedRow row=new LimitedRow();
				row.PatNum=PIn.Long(tableRow["PatNum"].ToString());
				row.PatientName=tableRow["patient"].ToString();
				row.DateTime=PIn.DateT(tableRow["DateTime"].ToString());
				row.Description=tableRow["description"].ToString();
				row.ProcCode=tableRow["ProcCode"].ToString();//isn't just a proc code. Can be "Claim" etc...
				row.Charges=tableRow["charges"].ToString();
				row.Credits=tableRow["credits"].ToString();
				row.ProvName=tableRow["prov"].ToString();
				row.Tooth=Tooth.ToInternat(tableRow["ToothNum"].ToString());
				row.ColorText=Color.FromArgb(PIn.Int(tableRow["colorText"].ToString()));
				if(PrefC.HasClinicsEnabled) {
					row.ClinicNum=PIn.Long(tableRow["ClinicNum"].ToString());
				}
				if(tableRow["AdjNum"].ToString()!="0") {
					row.PrimaryKey=PIn.Long(tableRow["AdjNum"].ToString());
					row.Type=AccountEntryType.Adjustment;
				}
				else if(tableRow["ProcNum"].ToString()!="0") {
					row.PrimaryKey=PIn.Long(tableRow["ProcNum"].ToString());
					row.Type=AccountEntryType.Procedure;
				}
				else if(tableRow["PayNum"].ToString()!="0") {
					row.PrimaryKey=PIn.Long(tableRow["PayNum"].ToString());
					row.Type=AccountEntryType.Payment;
				}
				else if(tableRow["ClaimNum"].ToString()!="0") {
					//can mean that this is either a claim or a claim payment.
					//we really only care about claim payments, but we need procedure from the claim.
					row.PrimaryKey=PIn.Long(tableRow["ClaimNum"].ToString());
					row.Type=AccountEntryType.Claim;
					if(tableRow["ClaimPaymentNum"].ToString()=="1") {
						row.Type=AccountEntryType.ClaimPayment;
					}		
				}
				else {
					//type is not one that is currently supported, skip it.
					continue;
				}
				row.ListProcsOnObject=tableRow["procsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				row.ListAdjustsOnObj=tableRow["adjustsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				row.ListPaymentsOnObj=tableRow["paymentsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				row.ProcNumLab=PIn.Long(tableRow["ProcNumLab"].ToString());
				_listLimitedRows.Add(row);
			}
		}

		private bool IsRowPreSelected(AccountEntryType type,long priKey) {
			return type==AccountEntryType.Adjustment && ListSelectedAdjNums.Contains(priKey)
				|| type==AccountEntryType.Procedure && ListSelectedProcNums.Contains(priKey)
				|| type==AccountEntryType.Payment && ListSelectedPayNums.Contains(priKey)
				|| type==AccountEntryType.ClaimPayment && ListSelectedPayClaimNums.Contains(priKey);//AccountEntryType.Claim won't be in the grid
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			List<DisplayField> fieldsForGrid=DisplayFields.GetForCategory(DisplayFieldCategory.AccountModule);
			if(!PrefC.HasClinicsEnabled) {
				//remove clinics from displayfields if clinics are disabled
				fieldsForGrid.RemoveAll(x => x.InternalName.ToLower().Contains("clinic"));
			}
			fieldsForGrid.RemoveAll(x => ListTools.In(x.InternalName,"Abbr","Balance","Signed"));
			HorizontalAlignment align;
			GridSortingStrategy sort;
			for(int i=0;i<fieldsForGrid.Count;i++) {
				align=HorizontalAlignment.Left;
				sort=GridSortingStrategy.StringCompare;
				if(ListTools.In(fieldsForGrid[i].InternalName,"Charges","Credits")) {
					align=HorizontalAlignment.Right;
					sort=GridSortingStrategy.AmountParse;
				}
				if(fieldsForGrid[i].InternalName=="Tth") {
					sort=GridSortingStrategy.ToothNumberParse;
				}
				if(fieldsForGrid[i].InternalName=="Date") {
					sort=GridSortingStrategy.DateParse;
				}
				gridMain.ListGridColumns.Add(new GridColumn(fieldsForGrid[i].Description==""?fieldsForGrid[i].InternalName:fieldsForGrid[i].Description,
					fieldsForGrid[i].ColumnWidth,align,sort));
			}
			if(gridMain.ListGridColumns.Sum(x => x.ColWidth) > gridMain.Width) {
				gridMain.HScrollVisible=true;
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listLimitedRows.Count;i++) {
				LimitedRow limitedRow=_listLimitedRows[i];
				if(!ListTools.In(limitedRow.Type,listBoxTransTypes.GetListSelected<AccountEntryType>())) {
					continue;
				}
				DateTime date=limitedRow.DateTime.Date;
				if(date.Date<odDatePickerFrom.GetDateTime().Date || date.Date>odDatePickerTo.GetDateTime().Date) {
					continue;//do not add to grid if it is outside the filtered date range. 
				}
				row=new GridRow();
				for(int f=0;f<fieldsForGrid.Count;f++) {
					switch(fieldsForGrid[f].InternalName) {
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
				LimitedRow lRow=gridMain.ListGridRows[i].Tag as LimitedRow;
				gridMain.SetSelected(i,_dictSelectedRows.TryGetValue(lRow.Type,out List<long> listPriKeys) && listPriKeys.Contains(lRow.PrimaryKey));
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
			List<LimitedRow> listSelectedRows=gridMain.SelectedTags<LimitedRow>();
			List<long> listSelectedPayClaimNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.ClaimPayment).Select(x => x.PrimaryKey).Distinct().ToList();
			List<long> listSelectedProcNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.Procedure).Select(x => x.PrimaryKey).Distinct().ToList();
			List<long> listSelectedPayNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.Payment).Select(x => x.PrimaryKey).Distinct().ToList();
			List<long> listSelectedAdjNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.Adjustment).Select(x => x.PrimaryKey).Distinct().ToList();
			List<int> listSelectedIndices=gridMain.SelectedIndices.ToList();
			for(int i=0;i<listSelectedIndices.Count;i++) {
				LimitedRow lSelectedRow=gridMain.ListGridRows[listSelectedIndices[i]].Tag as LimitedRow;
				if(lSelectedRow.Type==AccountEntryType.ClaimPayment) {
					listSelectedProcNums.AddRange(lSelectedRow.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)));
					List<LimitedRow> listClaimRows=_listLimitedRows.FindAll(x => x.Type==AccountEntryType.Claim && ListTools.In(x.PrimaryKey,listSelectedPayClaimNums));
					listSelectedProcNums.AddRange(listClaimRows.SelectMany(x => x.ListProcsOnObject.Where(y => y>0 && !listSelectedProcNums.Contains(y))).Distinct());
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow lRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(lRow.Type==AccountEntryType.ClaimPayment && ListTools.In(lRow.PrimaryKey,listSelectedPayClaimNums)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
							listSelectedProcNums.AddRange(lRow.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)).Distinct());
						}
					}
					listSelectedProcNums=listSelectedProcNums.Distinct().Where(x => x>0).ToList();
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow lRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(lRow.Type==AccountEntryType.Procedure && listSelectedProcNums.Any(x => lRow.PrimaryKey==x || lRow.ProcNumLab==x)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
					}
				}
				else if(lSelectedRow.Type==AccountEntryType.Payment) {
					listSelectedProcNums.AddRange(lSelectedRow.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)));
					listSelectedPayNums.AddRange(lSelectedRow.ListPaymentsOnObj.Where(x => x>0 && !listSelectedPayNums.Contains(x)));
					listSelectedAdjNums.AddRange(lSelectedRow.ListAdjustsOnObj.Where(x => x>0 && !listSelectedAdjNums.Contains(x)));
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow lRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(lRow.Type==AccountEntryType.Payment && listSelectedPayNums.Contains(lRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
							listSelectedProcNums.AddRange(lRow.ListProcsOnObject.Where(x => x>0 && !listSelectedProcNums.Contains(x)));
							listSelectedPayNums.AddRange(lRow.ListPaymentsOnObj.Where(x => x>0 && !listSelectedPayNums.Contains(x)));
							listSelectedAdjNums.AddRange(lRow.ListAdjustsOnObj.Where(x => x>0 && !listSelectedAdjNums.Contains(x)));
						}
					}
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow lRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(lRow.Type==AccountEntryType.Procedure && listSelectedProcNums.Contains(lRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
						else if(lRow.Type==AccountEntryType.Payment && listSelectedPayNums.Contains(lRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
						else if(lRow.Type==AccountEntryType.Adjustment && listSelectedAdjNums.Contains(lRow.PrimaryKey)) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
					}
				}
				else if(lSelectedRow.Type==AccountEntryType.Adjustment && lSelectedRow.ListProcsOnObject.Count==1) {
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow lRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(lRow.Type==AccountEntryType.Procedure && lRow.PrimaryKey==lSelectedRow.ListProcsOnObject[0]) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
							break;
						}
					}
				}
				else if(lSelectedRow.Type==AccountEntryType.Procedure || lSelectedRow.ProcNumLab!=0) {
					for(int j=0;j<gridMain.ListGridRows.Count;j++) {
						LimitedRow lRow=gridMain.ListGridRows[j].Tag as LimitedRow;
						if(lSelectedRow.ProcNumLab>0) {
							if(lRow.PrimaryKey==lSelectedRow.ProcNumLab || lRow.ProcNumLab==lSelectedRow.ProcNumLab) {
								if(!listSelectedIndices.Contains(j)) {
									listSelectedIndices.Add(j);
									gridMain.SetSelected(j,true);
								}
							}
						}
						else if(lSelectedRow.PrimaryKey==lRow.ProcNumLab) {
							if(!listSelectedIndices.Contains(j)) {
								listSelectedIndices.Add(j);
								gridMain.SetSelected(j,true);
							}
						}
					}
				}
			}
			_dictSelectedRows=gridMain.SelectedTags<LimitedRow>()
				.GroupBy(x => x.Type,x => x.PrimaryKey)
				.ToDictionary(x => x.Key,x => x.Distinct().ToList());
		}

		private void gridMain_ColumnSorted(object sender,EventArgs e) {
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				LimitedRow lRow=gridMain.ListGridRows[i].Tag as LimitedRow;
				gridMain.SetSelected(i,_dictSelectedRows.TryGetValue(lRow.Type,out List<long> listPriKeys) && listPriKeys.Contains(lRow.PrimaryKey));
			}
		}

		private void butAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
			_dictSelectedRows=gridMain.SelectedTags<LimitedRow>()
				.GroupBy(x => x.Type,x => x.PrimaryKey)
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
			List<LimitedRow> listSelectedRows=gridMain.SelectedTags<LimitedRow>();
			if(listSelectedRows.Count==0) {
				MsgBox.Show(this,"Please select procedures, adjustments, or payments first.");
				return;
			}
			ListSelectedPatNums=listSelectedRows.Select(x => x.PatNum).Distinct().ToList();
			ListSelectedAdjNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.Adjustment).Select(x => x.PrimaryKey).Distinct().ToList();
			ListSelectedPayNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.Payment).Select(x => x.PrimaryKey).Distinct().ToList();
			ListSelectedPayClaimNums=listSelectedRows.FindAll(x => x.Type==AccountEntryType.ClaimPayment).Select(x => x.PrimaryKey).Distinct().ToList();
			//get selected procs and any procs for the selected claimpayments and combine them into a single list of selected ProcNums
			ListSelectedProcNums=listSelectedRows
				.FindAll(x => x.Type==AccountEntryType.Procedure)
				.Select(x => x.PrimaryKey)
				.Concat(_listLimitedRows
					.FindAll(x => x.Type==AccountEntryType.Claim && ListTools.In(x.PrimaryKey,ListSelectedPayClaimNums))
					.SelectMany(x => x.ListProcsOnObject)
				).Distinct().ToList();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		/// <summary>Class to represent items coming in from the account module grid.</summary>
		private class LimitedRow {
			//Can be paymentNum,adjustmentNum,procedureNum,claimNum,payplanNum. NOTE: ClaimPayments will hold the ClaimNum. 
			public long PrimaryKey;
			public AccountEntryType Type;
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