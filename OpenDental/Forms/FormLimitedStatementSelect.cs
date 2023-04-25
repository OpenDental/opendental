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
		public DataTable TableAccount;
		private List<LimitedRow> _listLimitedRows=new List<LimitedRow>();
		///<summary>Used for reselecting rows after sorting by column or filling the grid on load etc.</summary>
		private Lookup<AccountEntryType,long> _lookupSelectedRows;
		public List<long> ListPayClaimNums=new List<long>();
		public List<long> ListAdjNums=new List<long>();
		public List<long> ListPayNums=new List<long>();
		public List<long> ListProcNums=new List<long>();
		public List<long> ListPatNums=new List<long>();
		public List<long> ListPayPlanChargeNums=new List<long>();
		public Patient PatCur=null;
		public List<long> ListPatNumsFamily=new List<long>();
		public List<long> ListPatNumsSuperFamily=new List<long>();

		public FormLimitedStatementSelect()
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLimitedStatementSelect_Load(object sender,EventArgs e) {
			List<EnumLimitedCustomFamily> listLimitedCustomFamily=new List<EnumLimitedCustomFamily>() {
				EnumLimitedCustomFamily.Patient,
				EnumLimitedCustomFamily.Family,
				EnumLimitedCustomFamily.SuperFamily,
			};
			for(int i=0; i<listLimitedCustomFamily.Count; i++) {
				listBoxShowFamilyMembers.Items.Add(Lan.g("EnumLimitedCustomFamily",listLimitedCustomFamily[i].ToString()),item:listLimitedCustomFamily[i]);
			}
			FillTransType();
			ConstructListFromTable();
			odDatePickerFrom.SetDateTime(_listLimitedRows
				.Where(x => x.DateTime!=DateTime.MinValue && IsRowPreSelected(x.AccountEntryType_,x.PrimaryKey))
				.Select(x => x.DateTime)
				.Concat(new[] { DateTime.Today.AddMonths(-1) }).Min());
			odDatePickerTo.SetDateTime(DateTime.Today);
			//If not in a superfamily, remove superfamily. We must remove from the end of the listbox to not affect order
			if(ListPatNumsSuperFamily.Count<=1) {
				listBoxShowFamilyMembers.Items.RemoveAt(listLimitedCustomFamily.IndexOf(EnumLimitedCustomFamily.SuperFamily));
			}
			//If not in a family, remove family
			if(ListPatNumsFamily.Count<=1) {
				listBoxShowFamilyMembers.Items.RemoveAt(listLimitedCustomFamily.IndexOf(EnumLimitedCustomFamily.Family));
			}
			//Preselect filter for superheads, guarantors, and patients
			if(PatCur!=null && PatCur.SuperFamily==PatCur.PatNum && !ListPatNumsSuperFamily.IsNullOrEmpty()) {
				//Preselect SuperFamily
				listBoxShowFamilyMembers.SetSelectedEnum(EnumLimitedCustomFamily.SuperFamily);
			}
			else if(PatCur!=null && PatCur.Guarantor==PatCur.PatNum && !ListPatNumsFamily.IsNullOrEmpty()) {
				//Preselect Family
				listBoxShowFamilyMembers.SetSelectedEnum(EnumLimitedCustomFamily.Family);
			}
			else {
				//Preselect Patient
				listBoxShowFamilyMembers.SetSelectedEnum(EnumLimitedCustomFamily.Patient);
			}
			SetFilterControlsAndAction(() => FillGrid(),listBoxTransTypes,odDatePickerFrom,odDatePickerTo);
			_lookupSelectedRows= (Lookup<AccountEntryType,long>)_listLimitedRows
			 .Where(x => IsRowPreSelected(x.AccountEntryType_,x.PrimaryKey))
			 .ToLookup(x => x.AccountEntryType_,x => x.PrimaryKey);
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
			for(int i=0;i<TableAccount.Rows.Count;i++){
				LimitedRow limitedRow=new LimitedRow();
				limitedRow.PatNum=PIn.Long(TableAccount.Rows[i]["PatNum"].ToString());
				limitedRow.PatientName=TableAccount.Rows[i]["patient"].ToString();
				limitedRow.DateTime=PIn.DateT(TableAccount.Rows[i]["DateTime"].ToString());
				limitedRow.Description=TableAccount.Rows[i]["description"].ToString();
				limitedRow.ProcCode=TableAccount.Rows[i]["ProcCode"].ToString();//isn't just a proc code. Can be "Claim" etc...
				limitedRow.Charges=TableAccount.Rows[i]["charges"].ToString();
				limitedRow.Credits=TableAccount.Rows[i]["credits"].ToString();
				limitedRow.ProvName=TableAccount.Rows[i]["prov"].ToString();
				limitedRow.Tooth=Tooth.Display(TableAccount.Rows[i]["ToothNum"].ToString());
				limitedRow.ColorText=Color.FromArgb(PIn.Int(TableAccount.Rows[i]["colorText"].ToString()));
				if(PrefC.HasClinicsEnabled) {
					limitedRow.ClinicNum=PIn.Long(TableAccount.Rows[i]["ClinicNum"].ToString());
				}
				if(TableAccount.Rows[i]["AdjNum"].ToString()!="0") {
					limitedRow.PrimaryKey=PIn.Long(TableAccount.Rows[i]["AdjNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Adjustment;
				}
				else if(TableAccount.Rows[i]["ProcNum"].ToString()!="0") {
					limitedRow.PrimaryKey=PIn.Long(TableAccount.Rows[i]["ProcNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Procedure;
				}
				else if(TableAccount.Rows[i]["PayNum"].ToString()!="0") {
					limitedRow.PrimaryKey=PIn.Long(TableAccount.Rows[i]["PayNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Payment;
				}
				else if(TableAccount.Rows[i]["ClaimNum"].ToString()!="0") {
					//can mean that this is either a claim or a claim payment.
					//we really only care about claim payments, but we need procedure from the claim.
					limitedRow.PrimaryKey=PIn.Long(TableAccount.Rows[i]["ClaimNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.Claim;
					if(TableAccount.Rows[i]["ClaimPaymentNum"].ToString()=="1") {
						limitedRow.AccountEntryType_=AccountEntryType.ClaimPayment;
					}
				}
				else if(TableAccount.Rows[i]["PayPlanChargeNum"].ToString()!="0" && TableAccount.Rows[i]["credits"].ToString()=="") {//We only want debits.
					limitedRow.PrimaryKey=PIn.Long(TableAccount.Rows[i]["PayPlanChargeNum"].ToString());
					limitedRow.AccountEntryType_=AccountEntryType.PayPlanCharge;
				}
				else {
					//type is not one that is currently supported, skip it.
					continue;
				}
				limitedRow.ListProcsOnObject=TableAccount.Rows[i]["procsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				limitedRow.ListAdjustsOnObj=TableAccount.Rows[i]["adjustsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				limitedRow.ListPaymentsOnObj=TableAccount.Rows[i]["paymentsOnObj"].ToString().Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries).Select(x => PIn.Long(x)).ToList();
				limitedRow.ProcNumLab=PIn.Long(TableAccount.Rows[i]["ProcNumLab"].ToString());
				_listLimitedRows.Add(limitedRow);
			}
		}

		private bool IsRowPreSelected(AccountEntryType accountEntryType,long priKey) {
			if(accountEntryType==AccountEntryType.Adjustment && ListAdjNums.Contains(priKey)){
				return true;
			}
			if(accountEntryType==AccountEntryType.Procedure && ListProcNums.Contains(priKey)){
				return true;
			}
			if(accountEntryType==AccountEntryType.Payment && ListPayNums.Contains(priKey)){
				return true;
			}
			if(accountEntryType==AccountEntryType.ClaimPayment && ListPayClaimNums.Contains(priKey)){
				return true;
			}
			if(accountEntryType==AccountEntryType.PayPlanCharge && ListPayPlanChargeNums.Contains(priKey)){
				return true;
			}
			return false;
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.LimitedCustomStatement);
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
				//Showing Family Entries.
				if(listBoxShowFamilyMembers.GetSelected<EnumLimitedCustomFamily>()==EnumLimitedCustomFamily.Family
					&& !ListPatNumsFamily.Contains(_listLimitedRows[i].PatNum)) 
				{
					continue;
				}
				//Showing SuperFamily Entries.
				if(listBoxShowFamilyMembers.GetSelected<EnumLimitedCustomFamily>()==EnumLimitedCustomFamily.SuperFamily
					&& !ListPatNumsSuperFamily.Contains(_listLimitedRows[i].PatNum)) 
				{
					continue;
				}
				//Showing Patient Entries.
				if(listBoxShowFamilyMembers.GetSelected<EnumLimitedCustomFamily>()==EnumLimitedCustomFamily.Patient 
					&& _listLimitedRows[i].PatNum!=PatCur.PatNum) 
				{
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
						case "Guarantor":
							row.Cells.Add(Patients.GetGuarForPat(limitedRow.PatNum).GetNameLF());
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
			for(int i = 0;i<gridMain.ListGridRows.Count;i++) {
				LimitedRow limitedRow = gridMain.ListGridRows[i].Tag as LimitedRow;
				bool isSelected=false;
				if(_lookupSelectedRows.Contains(limitedRow.AccountEntryType_)){
					if(_lookupSelectedRows[limitedRow.AccountEntryType_].Contains(limitedRow.PrimaryKey)){
						isSelected=true;
					}
				}
				gridMain.SetSelected(i,isSelected);
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

		private void listBoxShowFamilyMembers_SelectedIndexCommitted(object sender,EventArgs e) { 
			FillGrid();
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
			_lookupSelectedRows= (Lookup<AccountEntryType,long>)gridMain.SelectedTags<LimitedRow>()
				.ToLookup(x => x.AccountEntryType_,x => x.PrimaryKey);
		}

		private void gridMain_ColumnSorted(object sender,EventArgs e) {
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				LimitedRow limitedRow=gridMain.ListGridRows[i].Tag as LimitedRow;
				bool isSelected=false;
				if(_lookupSelectedRows.Contains(limitedRow.AccountEntryType_)){
					if(_lookupSelectedRows[limitedRow.AccountEntryType_].Contains(limitedRow.PrimaryKey)){
						isSelected=true;
					}
				}
				gridMain.SetSelected(i,isSelected);
			}
		}

		private void butAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
			_lookupSelectedRows=(Lookup<AccountEntryType,long>)gridMain.SelectedTags<LimitedRow>()
				.ToLookup(x => x.AccountEntryType_,x => x.PrimaryKey);
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
			List<LimitedRow> listLimitedRows=new List<LimitedRow>();//clear the lookup
			_lookupSelectedRows=(Lookup<AccountEntryType,long>)listLimitedRows.ToLookup(x => x.AccountEntryType_,x => x.PrimaryKey);
		}

		private void butOK_Click(object sender,EventArgs e) {
			List<LimitedRow> listLimitedRowsSelected=gridMain.SelectedTags<LimitedRow>();
			if(listLimitedRowsSelected.Count==0) {
				MsgBox.Show(this,"Please select procedures, adjustments, payments, claim payments or payment plan charges first.");
				return;
			}
			ListPatNums=listLimitedRowsSelected.Select(x => x.PatNum).Distinct().ToList();
			ListAdjNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Adjustment).Select(x => x.PrimaryKey).Distinct().ToList();
			ListPayNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.Payment).Select(x => x.PrimaryKey).Distinct().ToList();
			ListPayClaimNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.ClaimPayment).Select(x => x.PrimaryKey).Distinct().ToList();
			//get selected procs and any procs for the selected claimpayments and combine them into a single list of selected ProcNums
			ListProcNums=listLimitedRowsSelected
				.FindAll(x => x.AccountEntryType_==AccountEntryType.Procedure)
				.Select(x => x.PrimaryKey)
				.Concat(_listLimitedRows
					.FindAll(x => x.AccountEntryType_==AccountEntryType.Claim && ListPayClaimNums.Contains(x.PrimaryKey))
					.SelectMany(x => x.ListProcsOnObject)
				).Distinct().ToList();
			ListPayPlanChargeNums=listLimitedRowsSelected.FindAll(x => x.AccountEntryType_==AccountEntryType.PayPlanCharge).Select(x => x.PrimaryKey).Distinct().ToList();
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