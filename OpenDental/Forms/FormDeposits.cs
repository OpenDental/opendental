using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormDeposits : FormODBase {
		private List<Deposit> _listDeposits;
		///<summary>Use this from Transaction screen when attaching a source document.</summary>
		public bool IsSelectionMode;
		///<summary>In selection mode, when closing form with OK, this contains selected deposit.</summary>
		public Deposit DepositSelected;

		///<summary></summary>
		public FormDeposits() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDeposits_Load(object sender,EventArgs e) {
			if(IsSelectionMode){
				butAdd.Visible=false;
			}
			else{
				butOK.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid(){
			if(PrefC.HasClinicsEnabled) {
				//GetForClinics uses an empty list to indicate "all", which is a loophole if user doesn't select an item.  So:
				if(comboClinics.ListSelectedClinicNums.Count==0) {
					_listDeposits=Deposits.GetForClinics(new List<long>(){ Clinics.ClinicNum },IsSelectionMode);//restrict to current clinic
				}
				else {
					_listDeposits=Deposits.GetForClinics(comboClinics.ListSelectedClinicNums,IsSelectionMode);
				} 
			}
			else {
				if(IsSelectionMode) {
					_listDeposits=Deposits.GetUnattached();
				}
				else {
					_listDeposits=Deposits.Refresh();
				}
			}
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableDepositSlips","Date"),80);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlips","Amount"),90,HorizontalAlignment.Right);
			grid.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableDepositSlips","Clinic"),150);
				grid.ListGridColumns.Add(col);
			}
			grid.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDeposits.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listDeposits[i].DateDeposit.ToShortDateString());
				row.Cells.Add(_listDeposits[i].Amount.ToString("F"));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(" "+_listDeposits[i].ClinicAbbr);//padding left with space to add separation between amount and clinic abbr
				}
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollToEnd();
		}

		private void ComboClinics_SelectionChangeCommitted(object sender,EventArgs e){
			FillGrid();
		}

		private void grid_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			if(IsSelectionMode){
				DepositSelected=_listDeposits[e.Row];
				DialogResult=DialogResult.OK;
				return;
			}
			//not selection mode.
			using FormDepositEdit formDepositEdit=new FormDepositEdit(_listDeposits[e.Row]);
			formDepositEdit.ShowDialog();
			if(formDepositEdit.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}

		///<summary>Not available in selection mode.</summary>
		private void butAdd_Click(object sender,EventArgs e) {
			Deposit deposit=new Deposit();
			deposit.DateDeposit=DateTime.Today;
			deposit.BankAccountInfo=PrefC.GetString(PrefName.PracticeBankNumber);
			using FormDepositEdit formDepositEdit=new FormDepositEdit(deposit);
			formDepositEdit.IsNew=true;
			formDepositEdit.ShowDialog();
			if(formDepositEdit.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}

		///<summary>Only available in selection mode.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			if(grid.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select a deposit first.");
				return;
			}
			DepositSelected=_listDeposits[grid.GetSelectedIndex()];
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















