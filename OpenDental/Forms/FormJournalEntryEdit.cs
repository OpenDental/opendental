/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormJournalEntryEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		//private ArrayList PosIndex=new ArrayList();
		//private ArrayList NegIndex=new ArrayList();
		///<summary></summary>
		public JournalEntry EntryCur;
		private Account AccountPicked;

		///<summary></summary>
		public FormJournalEntryEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJournalEntryEdit_Load(object sender, System.EventArgs e) {
			if(EntryCur==null){
				MessageBox.Show("Entry cannot be null.");
			}
			AccountPicked=Accounts.GetAccount(EntryCur.AccountNum);//might be null
			/*
			for(int i=0;i<Accounts.ListShort.Length;i++) {
				comboAccount.Items.Add(Accounts.ListShort[i].Description);
				if(Accounts.ListShort[i].AccountNum==EntryCur.AccountNum){
					comboAccount.SelectedIndex=i;
				}
			}
			if(EntryCur.AccountNum !=0 && comboAccount.SelectedIndex==-1){//must be an inactive account
				
			}*/
			FillAccount();
			if(EntryCur.DebitAmt>0){
				textDebit.Text=EntryCur.DebitAmt.ToString("n");
			}
			if(EntryCur.CreditAmt>0) {
				textCredit.Text=EntryCur.CreditAmt.ToString("n");
			}
			textMemo.Text=EntryCur.Memo;
			textCheckNumber.Text=EntryCur.CheckNumber;
			if(EntryCur.ReconcileNum==0){//not attached
				labelReconcile.Visible=false;
				textReconcile.Visible=false;
			}
			else{//attached
				textReconcile.Text=Reconciles.GetOne(EntryCur.ReconcileNum).DateReconcile.ToShortDateString();
				textDebit.ReadOnly=true;
				textCredit.ReadOnly=true;
				butDelete.Enabled=false;
				butChange.Enabled=false;
			}
		}

		///<summary>Need to set AccountPicked before calling this.</summary>
		private void FillAccount(){
			if(AccountPicked==null){
				textAccount.Text="";
				butChange.Text=Lan.g(this,"Pick");
				labelDebit.Text=Lan.g(this,"Debit");
				labelCredit.Text=Lan.g(this,"Credit");
				return;
			}
			//AccountCur=Accounts.ListShort[comboAccount.SelectedIndex];
			textAccount.Text=AccountPicked.Description;
			butChange.Text=Lan.g(this,"Change");
			if(Accounts.DebitIsPos(AccountPicked.AcctType)) {
				labelDebit.Text=Lan.g(this,"Debit")+Lan.g(this,"(+)");
				labelCredit.Text=Lan.g(this,"Credit")+Lan.g(this,"(-)");
			}
			else {
				labelDebit.Text=Lan.g(this,"Debit")+Lan.g(this,"(-)");
				labelCredit.Text=Lan.g(this,"Credit")+Lan.g(this,"(+)");
			}
		}

		/*private void comboAccount_SelectedIndexChanged(object sender,EventArgs e) {
			FillAccount();
		}*/

		private void butChange_Click(object sender,EventArgs e) {
			using FormAccountPick FormA=new FormAccountPick();
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK){
				return;
			}
			AccountPicked=FormA.SelectedAccount;
			FillAccount();
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			EntryCur=null;
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDebit.IsValid() || !textCredit.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			/*if(comboAccount.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an account first.");
				return;
			}*/
			if(PIn.Double(textDebit.Text)<0 || PIn.Double(textCredit.Text)<0){
				MsgBox.Show(this,"Both amounts not allowed to be less than 0.");
				return;
			}
			if(PIn.Double(textDebit.Text)==0 && PIn.Double(textCredit.Text)==0) {
				MsgBox.Show(this,"One amount must be filled in.");
				return;
			}
			if(PIn.Double(textDebit.Text)>0 && PIn.Double(textCredit.Text)>0) {
				MsgBox.Show(this,"Only one amount can be filled in.");
				return;
			}
			if(AccountPicked==null || AccountPicked.AccountNum==0) {
				MsgBox.Show(this,"Please select an account.");
				return;
			}
			EntryCur.AccountNum=AccountPicked.AccountNum;
			EntryCur.DebitAmt=PIn.Double(textDebit.Text);
			EntryCur.CreditAmt=PIn.Double(textCredit.Text);
			EntryCur.Memo=textMemo.Text;
			EntryCur.CheckNumber=textCheckNumber.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(IsNew){
				EntryCur=null;
			}
			DialogResult=DialogResult.Cancel;
		}

		

		

	

	}


}
