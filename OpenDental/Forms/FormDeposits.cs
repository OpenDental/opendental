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
		private Deposit[] DList;
		///<summary>Use this from Transaction screen when attaching a source document.</summary>
		public bool IsSelectionMode;
		///<summary>List of Clinics the user has access to.</summary>
		private List<Clinic> _listClinics=new List<Clinic>();
		///<summary>In selection mode, when closing form with OK, this contains selected deposit.</summary>
		public Deposit SelectedDeposit;

		///<summary></summary>
		public FormDeposits()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDeposits_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode){
				butAdd.Visible=false;
			}
			else{
				butOK.Visible=false;
			}
			FillGrid();
		}

		private void FillGrid(){
			if(!PrefC.HasClinicsEnabled) {
				if(IsSelectionMode) {
					DList=Deposits.GetUnattached();
				}
				else {
					DList=Deposits.Refresh();
				}
			}
			else {
				//GetForClinics uses an empty list to indicate "all", which is a loophole if user doesn't select an item.  So:
				if(comboClinics.ListSelectedClinicNums.Count==0){
					DList=Deposits.GetForClinics(new List<long>(){Clinics.ClinicNum },IsSelectionMode);//restrict to current clinic
				}
				else{
					DList=Deposits.GetForClinics(comboClinics.ListSelectedClinicNums,IsSelectionMode);
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
			OpenDental.UI.GridRow row;
			for(int i=0;i<DList.Length;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(DList[i].DateDeposit.ToShortDateString());
				row.Cells.Add(DList[i].Amount.ToString("F"));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(" "+DList[i].ClinicAbbr);//padding left with space to add separation between amount and clinic abbr
				}
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollToEnd();
		}

		private void ComboClinics_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void grid_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			if(IsSelectionMode){
				SelectedDeposit=DList[e.Row];
				DialogResult=DialogResult.OK;
				return;
			}
			//not selection mode.
			using FormDepositEdit FormD=new FormDepositEdit(DList[e.Row]);
			FormD.ShowDialog();
			if(FormD.DialogResult==DialogResult.Cancel){
				return;
			}
			FillGrid();
		}

		///<summary>Not available in selection mode.</summary>
		private void butAdd_Click(object sender, System.EventArgs e) {
			Deposit deposit=new Deposit();
			deposit.DateDeposit=DateTime.Today;
			deposit.BankAccountInfo=PrefC.GetString(PrefName.PracticeBankNumber);
			using FormDepositEdit FormD=new FormDepositEdit(deposit);
			FormD.IsNew=true;
			FormD.ShowDialog();
			if(FormD.DialogResult==DialogResult.Cancel){
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
			SelectedDeposit=DList[grid.GetSelectedIndex()];
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















