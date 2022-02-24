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

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAccountingSetup : FormODBase {
		///<summary>Each item in the array list is an int for an AccountNum for the deposit accounts.</summary>
		private ArrayList depAL;
		private long PickedDepAccountNum;
		//private ArrayList cashAL;
		private long PickedPayAccountNum;
		///<summary>Arraylist of AccountingAutoPays.</summary>
		private List<AccountingAutoPay> payList;

		///<summary></summary>
		public FormAccountingSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountingSetup_Load(object sender,EventArgs e) {
			string depStr=PrefC.GetString(PrefName.AccountingDepositAccounts);
			string[] depStrArray=depStr.Split(new char[] { ',' });
			depAL=new ArrayList();
			for(int i=0;i<depStrArray.Length;i++) {
				if(depStrArray[i]=="") {
					continue;
				}
				depAL.Add(PIn.Long(depStrArray[i]));
			}
			FillDepList();
			PickedDepAccountNum=PrefC.GetLong(PrefName.AccountingIncomeAccount);
			textAccountInc.Text=Accounts.GetDescript(PickedDepAccountNum);
			//pay----------------------------------------------------------
			payList=AccountingAutoPays.GetDeepCopy();
			FillPayGrid();
			PickedPayAccountNum=PrefC.GetLong(PrefName.AccountingCashIncomeAccount);
			textAccountCashInc.Text=Accounts.GetDescript(PickedPayAccountNum);
		}

		private void FillDepList(){
			listAccountsDep.Items.Clear();
			for(int i=0;i<depAL.Count;i++){
				listAccountsDep.Items.Add(Accounts.GetDescript((long)depAL[i]));
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
			for(int i=0;i<payList.Count;i++){
				row=new GridRow();
				row.Cells.Add(Defs.GetName(DefCat.PaymentTypes,payList[i].PayType));
				row.Cells.Add(AccountingAutoPays.GetPickListDesc(payList[i]));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAccountPick FormA=new FormAccountPick();
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			depAL.Add(FormA.SelectedAccount.AccountNum);
			FillDepList();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listAccountsDep.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			depAL.RemoveAt(listAccountsDep.SelectedIndex);
			FillDepList();
		}

		private void butChange_Click(object sender,EventArgs e) {
			using FormAccountPick FormA=new FormAccountPick();
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			PickedDepAccountNum=FormA.SelectedAccount.AccountNum;
			textAccountInc.Text=Accounts.GetDescript(PickedDepAccountNum);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAccountingAutoPayEdit FormA=new FormAccountingAutoPayEdit();
			FormA.AutoPayCur=payList[e.Row];
			FormA.ShowDialog();
			if(FormA.AutoPayCur==null){//user deleted
				payList.RemoveAt(e.Row);
			}
			FillPayGrid();
		}

		private void butAddPay_Click(object sender,EventArgs e) {
			AccountingAutoPay autoPay=new AccountingAutoPay();
			using FormAccountingAutoPayEdit FormA=new FormAccountingAutoPayEdit();
			FormA.AutoPayCur=autoPay;
			FormA.IsNew=true;
			if(FormA.ShowDialog()!=DialogResult.OK) {
				return;
			}
			payList.Add(autoPay);
			FillPayGrid();
		}

		private void butChangeCash_Click(object sender,EventArgs e) {
			using FormAccountPick FormA=new FormAccountPick();
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			PickedPayAccountNum=FormA.SelectedAccount.AccountNum;
			textAccountCashInc.Text=Accounts.GetDescript(PickedPayAccountNum);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			string depStr="";
			for(int i=0;i<depAL.Count;i++) {
				if(i>0) {
					depStr+=",";
				}
				depStr+=depAL[i].ToString();
			}
			if(Prefs.UpdateString(PrefName.AccountingDepositAccounts,depStr)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			if(Prefs.UpdateLong(PrefName.AccountingIncomeAccount,PickedDepAccountNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//pay------------------------------------------------------------------------------------------
			AccountingAutoPays.SaveList(payList);//just deletes them all and starts over
			DataValid.SetInvalid(InvalidType.AccountingAutoPays);
			if(Prefs.UpdateLong(PrefName.AccountingCashIncomeAccount,PickedPayAccountNum)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		


	}
}





















