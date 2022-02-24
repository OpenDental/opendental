using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAccountingAutoPayEdit : FormODBase {
		///<summary></summary>
		public AccountingAutoPay AutoPayCur;
		///<summary></summary>
		public bool IsNew;
		///<summary>Array List of AccountNums.</summary>
		private ArrayList accountAL;
		private List<Def> _listPaymentTypeDefs;

		///<summary></summary>
		public FormAccountingAutoPayEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountingAutoPayEdit_Load(object sender,EventArgs e) {
			if(AutoPayCur==null) {
				MessageBox.Show("Autopay cannot be null.");//just for debugging
			}
			_listPaymentTypeDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			for(int i=0;i<_listPaymentTypeDefs.Count;i++){
				comboPayType.Items.Add(_listPaymentTypeDefs[i].ItemName);
				if(_listPaymentTypeDefs[i].DefNum==AutoPayCur.PayType){
					comboPayType.SelectedIndex=i;
				}
			}
			if(AutoPayCur.PickList==null){
				AutoPayCur.PickList="";
			}
			string[] strArray=AutoPayCur.PickList.Split(new char[] { ',' });
			accountAL=new ArrayList();
			for(int i=0;i<strArray.Length;i++) {
				if(strArray[i]=="") {
					continue;
				}
				accountAL.Add(PIn.Long(strArray[i]));
			}
			FillList();
		}

		private void FillList() {
			listAccounts.Items.Clear();
			for(int i=0;i<accountAL.Count;i++) {
				listAccounts.Items.Add(Accounts.GetDescript((long)accountAL[i]));
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormAccountPick FormA=new FormAccountPick();
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return;
			}
			accountAL.Add(FormA.SelectedAccount.AccountNum);
			FillList();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listAccounts.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			accountAL.RemoveAt(listAccounts.SelectedIndex);
			FillList();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			AutoPayCur=null;
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
			if(accountAL.Count==0) {
				MsgBox.Show(this,"Please add at least one account to the pick list first.");
				return;
			}
			AutoPayCur.PayType=_listPaymentTypeDefs[comboPayType.SelectedIndex].DefNum;
			AutoPayCur.PickList="";
			for(int i=0;i<accountAL.Count;i++){
				if(i>0){
					AutoPayCur.PickList+=",";
				}
				AutoPayCur.PickList+=accountAL[i].ToString();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(IsNew) {
				AutoPayCur=null;
			}
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















