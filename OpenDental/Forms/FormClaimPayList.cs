using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using OpenDentBusiness.Crud;

namespace OpenDental {
	public partial class FormClaimPayList:FormODBase {
		private List<ClaimPayment> _listClaimPayments;
		///<summary>If this is not zero upon closing, then we will jump to the account module of that patient and highlight the claim.</summary>
		public long ClaimNumGoto;
		///<summary>If this is not zero upon closing, then we will jump to the account module of that patient and highlight the claim.</summary>
		public long PatNumGoto;
		//<summary>Set to true if the batch list was accessed originally by going through a claim.  This disables the GotoAccount feature.</summary>
		//public bool IsFromClaim;
		///<summary>List of defs of type ClaimPaymentGroup</summary>
		private List<Def> _listDefs;

		public FormClaimPayList() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimPayList_Load(object sender,EventArgs e) {
			textDateFrom.Text=DateTime.Now.AddDays(-10).ToShortDateString();
			textDateTo.Text=DateTime.Now.ToShortDateString();
			comboClinic.IsAllSelected=true;
			_listDefs=Defs.GetDefsForCategory(DefCat.ClaimPaymentGroups,isShort:true);
			FillComboPaymentGroup();
			FillGrid();
		}

		private void FillGrid(){
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			long clinicNum=0;
			if(!comboClinic.IsAllSelected) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			long defNum=0;
			if(comboPayGroup.SelectedIndex!=0) {
				defNum=_listDefs[comboPayGroup.SelectedIndex-1].DefNum;
			}
			DataTable table=ClaimPayments.GetForDateRange(dateFrom,dateTo,clinicNum,defNum);
			_listClaimPayments=ClaimPaymentCrud.TableToList(table);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),65);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),75,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Partial"),40,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Carrier"),180);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PayGroup"),80);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),80);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),180);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Scanned"),40,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);			
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listClaimPayments.Count;i++){
				row=new GridRow();
				if(_listClaimPayments[i].CheckDate.Year<1800) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listClaimPayments[i].CheckDate.ToShortDateString());
				}
				row.Cells.Add(Defs.GetName(DefCat.InsurancePaymentType,_listClaimPayments[i].PayType));
				row.Cells.Add(_listClaimPayments[i].CheckAmt.ToString("c"));
				row.Cells.Add(_listClaimPayments[i].IsPartial?"X":"");
				row.Cells.Add(_listClaimPayments[i].CarrierName);
				row.Cells.Add(Defs.GetName(DefCat.ClaimPaymentGroups,_listClaimPayments[i].PayGroup));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listClaimPayments[i].ClinicNum));
				}
				row.Cells.Add(_listClaimPayments[i].Note);
				row.Cells.Add((table.Rows[i]["hasEobAttach"].ToString()=="1")?"X":"");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollToEnd();
		}
		
		private void butAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {//date not checked here, but it will be checked when saving the check to prevent backdating
				return;
			}
			ClaimPayment claimPayment=new ClaimPayment();
			claimPayment.CheckDate=DateTime.Now;
			claimPayment.IsPartial=true;
			using FormClaimPayEdit formClaimPayEdit=new FormClaimPayEdit(claimPayment);
			formClaimPayEdit.IsNew=true;
			formClaimPayEdit.ShowDialog();
			if(formClaimPayEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			FormClaimPayBatch formClaimPayBatch=new FormClaimPayBatch(claimPayment,isRefreshNeeded:true);
			formClaimPayBatch.Show();
			formClaimPayBatch.FormClosed+=FormCPB_FormClosed;
		}

		private void FormCPB_FormClosed(object sender,FormClosedEventArgs e) {
			if(IsDisposed) {//Auto-Logoff was causing an unhandled exception below.  Can't use dialogue result check here because we want to referesh the grid below even if user clicked cancel.
				return; //Don't refresh the grid, as the form is already disposed.
			}
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {
				return;
			}
			FormClaimPayBatch formClaimPayBatch=new FormClaimPayBatch(_listClaimPayments[gridMain.GetSelectedIndex()]);
			formClaimPayBatch.Show();
			formClaimPayBatch.FormClosed+=FormCPBEdit_FormClosed;
		}

		private void FormCPBEdit_FormClosed(object sender,FormClosedEventArgs e) {
			if(IsDisposed) {//Auto-Logoff was causing an unhandled exception below.  Can't use dialogue result check here because we want to referesh the grid below even if user clicked cancel.
				return; //Don't refresh the grid, as the form is already disposed.
			}
			FormClaimPayBatch formClaimPayBatch=(FormClaimPayBatch)sender;
			if(formClaimPayBatch.GotoClaimNum!=0) {
				ClaimNumGoto=formClaimPayBatch.GotoClaimNum;
				PatNumGoto=formClaimPayBatch.GotoPatNum;
				Close();
			}
			else {
				FillGrid();
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FillComboPaymentGroup(long selectedDefNum=0) {
			comboPayGroup.Items.Clear();
			comboPayGroup.Items.Add("All");
			comboPayGroup.SelectedIndex=0;
			for(int i=0;i<_listDefs.Count;i++) {
				Def def=_listDefs[i];
				comboPayGroup.Items.Add(def.ItemName);
				if(selectedDefNum!=0 && selectedDefNum==def.DefNum) {
					comboPayGroup.SelectedIndex=i+1; //+1 to account for the "All" option already added to the combobox
				}
			}
		}

		private void butPickPaymentGroup_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.ClaimPaymentGroups);
			formDefinitionPicker.ShowDialog();
			if(formDefinitionPicker.DialogResult==DialogResult.OK) {
				if(formDefinitionPicker.ListDefsSelected.Count<1) {
					FillComboPaymentGroup();
				}
				else { 
					FillComboPaymentGroup(formDefinitionPicker.ListDefsSelected[0].DefNum);
				}
			}
		}
	}
}