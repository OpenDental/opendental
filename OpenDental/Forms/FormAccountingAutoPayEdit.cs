using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary>Allows user to edit automatic payment entries. Form can be found at Manage->Accounting->Setup->Open Dental->Double click on entry in table</summary>
	public partial class FormAccountingAutoPayEdit : FormODBase {
		///<summary></summary>
		public AccountingAutoPay AccountingAutoPayCur;
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		private List<long> _listAccountNums;
		private List<Def> _listDefsPaymentTypes;

		///<summary></summary>
		public FormAccountingAutoPayEdit()
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountingAutoPayEdit_Load(object sender,EventArgs e) {
			if(AccountingAutoPayCur==null) {
				MessageBox.Show("Autopay cannot be null.");//just for debugging
			}
			_listDefsPaymentTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			for(int i=0;i<_listDefsPaymentTypes.Count;i++){
				comboPayType.Items.Add(_listDefsPaymentTypes[i].ItemName);
				if(_listDefsPaymentTypes[i].DefNum==AccountingAutoPayCur.PayType){
					comboPayType.SelectedIndex=i;
				}
			}
			if(AccountingAutoPayCur.PickList==null){
				AccountingAutoPayCur.PickList="";
			}
			List<string> listStrings=AccountingAutoPayCur.PickList.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList();
			_listAccountNums=new List<long>();
			for(int i=0;i<listStrings.Count;i++) {
				_listAccountNums.Add(PIn.Long(listStrings[i]));
			}
			FillList();
		}

		private void FillList() {
			listAccounts.Items.Clear();
			for(int i=0;i<_listAccountNums.Count;i++) {
				listAccounts.Items.Add(Accounts.GetDescript((long)_listAccountNums[i]));
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAccountPick formAccountPick=new FormAccountPick();
			formAccountPick.ShowDialog();
			if(formAccountPick.DialogResult!=DialogResult.OK) {
				return;
			}
			_listAccountNums.Add(formAccountPick.SelectedAccount.AccountNum);
			FillList();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listAccounts.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			_listAccountNums.RemoveAt(listAccounts.SelectedIndex);
			FillList();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			AccountingAutoPayCur=null;
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(comboPayType.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a pay type first.");
				return;
			}
			if(_listAccountNums.Count==0) {
				MsgBox.Show(this,"Please add at least one account to the pick list first.");
				return;
			}
			AccountingAutoPayCur.PayType=_listDefsPaymentTypes[comboPayType.SelectedIndex].DefNum;
			AccountingAutoPayCur.PickList="";
			for(int i=0;i<_listAccountNums.Count;i++){
				if(i>0){
					AccountingAutoPayCur.PickList+=",";
				}
				AccountingAutoPayCur.PickList+=_listAccountNums[i].ToString();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(IsNew) {
				AccountingAutoPayCur=null;
			}
			DialogResult=DialogResult.Cancel;
		}
	}
}





















