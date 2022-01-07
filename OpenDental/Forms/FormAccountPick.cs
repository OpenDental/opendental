using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormAccountPick:FormODBase {
		///<summary>Upon closing with OK, this will be the selected account.</summary>
		public Account SelectedAccount;
		public bool IsQuickBooks;
		public List<string> ListSelectedAccountsQB;

		///<summary></summary>
		public FormAccountPick()
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountPick_Load(object sender,EventArgs e) {
			if(IsQuickBooks) {
				ListSelectedAccountsQB=new List<string>();
				checkInactive.Visible=false;
				FillGridQB();
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			else {
				FillGrid();
			}
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableChartOfAccounts","Type"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Description"),170);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Balance"),65,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Bank Number"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Inactive"),70);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<Account> listAccounts=Accounts.GetDeepCopy(false);
			if(!checkInactive.Checked) { 
				listAccounts=listAccounts.FindAll(x=>!x.Inactive);
			}
			for(int i=0;i<listAccounts.Count;i++){
				row=new GridRow();
				row.Cells.Add(Lan.g("enumAccountType",listAccounts[i].AcctType.ToString()));
				row.Cells.Add(listAccounts[i].Description);
				if(listAccounts[i].AcctType==AccountType.Asset){
					row.Cells.Add(Accounts.GetBalance(listAccounts[i].AccountNum,listAccounts[i].AcctType).ToString("n"));
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(listAccounts[i].BankNumber);
				if(listAccounts[i].Inactive){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(i<listAccounts.Count-1//if not the last row
					&& listAccounts[i].AcctType != listAccounts[i+1].AcctType){
						row.ColorLborder=Color.Black;
				}
				row.Tag=listAccounts[i].Clone();
				row.ColorBackG=listAccounts[i].AccountColor;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridQB(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableChartOfAccountsQB","Description"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//Get the list of accounts from QuickBooks.
			Cursor.Current=Cursors.WaitCursor;
			List<string> listAccounts=new List<string>();
			try {
				listAccounts=QuickBooks.GetListOfAccounts();
			}
			catch(Exception e) {
				MessageBox.Show(e.Message);
			}
			Cursor.Current=Cursors.Default;
			for(int i=0;i<listAccounts.Count;i++){
				row=new GridRow();
				row.Cells.Add(listAccounts[i]);
				row.Tag=listAccounts[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsQuickBooks) {
				ListSelectedAccountsQB.Add((string)gridMain.ListGridRows[e.Row].Tag);
			}
			else {
				SelectedAccount=((Account)gridMain.ListGridRows[e.Row].Tag).Clone();
			}
			DialogResult=DialogResult.OK;
		}

		private void checkInactive_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an account first.");
				return;
			}
			if(IsQuickBooks) {
				for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
					ListSelectedAccountsQB.Add((string)(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag));
				}
			}
			else {
				SelectedAccount=((Account)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).Clone();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















