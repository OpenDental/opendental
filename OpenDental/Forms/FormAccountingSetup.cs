using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Linq;

namespace OpenDental{
	///<summary></summary>
	public partial class FormAccountingSetup : FormODBase {
		///<summary>Each item in the list is a long for an AccountNum for the deposit accounts.</summary>
		private List<long> _listDepAccountNums;
		private long _selectedDepAccountNum;
		//private ArrayList cashAL;
		private long _selectedPayAccountNum;
		///<summary>Arraylist of AccountingAutoPays.</summary>
		private List<AccountingAutoPay> _listAccountingAutoPays;

		///<summary></summary>
		public FormAccountingSetup()
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountingSetup_Load(object sender,EventArgs e) {
			string strAccountingDepositAccounts=PrefC.GetString(PrefName.AccountingDepositAccounts);
			List<string> listStrings=strAccountingDepositAccounts.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
			_listDepAccountNums=new List<long>();
			for(int i=0;i<listStrings.Count;i++) {
				_listDepAccountNums.Add(PIn.Long(listStrings[i]));
			}
			FillDepList();
			_selectedDepAccountNum=PrefC.GetLong(PrefName.AccountingIncomeAccount);
			textAccountInc.Text=Accounts.GetDescript(_selectedDepAccountNum);
			//pay----------------------------------------------------------
			_listAccountingAutoPays=AccountingAutoPays.GetDeepCopy();
			FillPayGrid();
			_selectedPayAccountNum=PrefC.GetLong(PrefName.AccountingCashIncomeAccount);
			textAccountCashInc.Text=Accounts.GetDescript(_selectedPayAccountNum);
		}

		private void FillDepList(){
			listAccountsDep.Items.Clear();
			for(int i=0;i<_listDepAccountNums.Count;i++){
				listAccountsDep.Items.Add(Accounts.GetDescript(_listDepAccountNums[i]));
			}
		}

		private void FillPayGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAccountingAutoPay","Payment Type"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAccountingAutoPay","Pick List"),250);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAccountingAutoPays.Count;i++){
				row=new GridRow();
				row.Cells.Add(Defs.GetName(DefCat.PaymentTypes,_listAccountingAutoPays[i].PayType));
				row.Cells.Add(AccountingAutoPays.GetPickListDesc(_listAccountingAutoPays[i]));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAccountPick formAccountPick=new FormAccountPick();
			formAccountPick.ShowDialog();
			if(formAccountPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_listDepAccountNums.Add(formAccountPick.SelectedAccount.AccountNum);
			FillDepList();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listAccountsDep.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			_listDepAccountNums.RemoveAt(listAccountsDep.SelectedIndex);
			FillDepList();
		}

		private void butChange_Click(object sender,EventArgs e) {
			using FormAccountPick formAccountPick=new FormAccountPick();
			formAccountPick.ShowDialog();
			if(formAccountPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_selectedDepAccountNum=formAccountPick.SelectedAccount.AccountNum;
			textAccountInc.Text=Accounts.GetDescript(_selectedDepAccountNum);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAccountingAutoPayEdit formAccountingAutoPayEdit=new FormAccountingAutoPayEdit();
			formAccountingAutoPayEdit.AccountingAutoPayCur=_listAccountingAutoPays[e.Row];
			formAccountingAutoPayEdit.ShowDialog();
			if(formAccountingAutoPayEdit.AccountingAutoPayCur==null){//user deleted
				_listAccountingAutoPays.RemoveAt(e.Row);
			}
			FillPayGrid();
		}

		private void butAddPay_Click(object sender,EventArgs e) {
			AccountingAutoPay accountingAutoPay=new AccountingAutoPay();
			using FormAccountingAutoPayEdit formAccountingAutoPayEdit=new FormAccountingAutoPayEdit();
			formAccountingAutoPayEdit.AccountingAutoPayCur=accountingAutoPay;
			formAccountingAutoPayEdit.IsNew=true;
			if(formAccountingAutoPayEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_listAccountingAutoPays.Add(accountingAutoPay);
			FillPayGrid();
		}

		private void butChangeCash_Click(object sender,EventArgs e) {
			using FormAccountPick formAccountPick=new FormAccountPick();
			formAccountPick.ShowDialog();
			if(formAccountPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_selectedPayAccountNum=formAccountPick.SelectedAccount.AccountNum;
			textAccountCashInc.Text=Accounts.GetDescript(_selectedPayAccountNum);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			string accountingDepositAccounts="";
			for(int i=0;i<_listDepAccountNums.Count;i++) {
				if(i>0) {
					accountingDepositAccounts+=",";
				}
				accountingDepositAccounts+=_listDepAccountNums[i].ToString();
			}
			if(Prefs.UpdateString(PrefName.AccountingDepositAccounts,accountingDepositAccounts)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(Prefs.UpdateLong(PrefName.AccountingIncomeAccount,_selectedDepAccountNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//pay------------------------------------------------------------------------------------------
			AccountingAutoPays.SaveList(_listAccountingAutoPays);//just deletes them all and starts over
			DataValid.SetInvalid(InvalidType.AccountingAutoPays);
			if(Prefs.UpdateLong(PrefName.AccountingCashIncomeAccount,_selectedPayAccountNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















