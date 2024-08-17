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
		public JournalEntry JournalEntryCur;
		private Account _accountPicked;

		///<summary></summary>
		public FormJournalEntryEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJournalEntryEdit_Load(object sender, System.EventArgs e) {
			if(JournalEntryCur==null){
				MessageBox.Show("Entry cannot be null.");
			}
			_accountPicked=Accounts.GetAccount(JournalEntryCur.AccountNum);//might be null
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
			if(JournalEntryCur.DebitAmt>0){
				textDebit.Text=JournalEntryCur.DebitAmt.ToString("n");
			}
			if(JournalEntryCur.CreditAmt>0) {
				textCredit.Text=JournalEntryCur.CreditAmt.ToString("n");
			}
			textMemo.Text=JournalEntryCur.Memo;
			textCheckNumber.Text=JournalEntryCur.CheckNumber;
			if(JournalEntryCur.ReconcileNum==0){//not attached
				labelReconcile.Visible=false;
				textReconcile.Visible=false;
				return;
			}
			//attached
			textReconcile.Text=Reconciles.GetOne(JournalEntryCur.ReconcileNum).DateReconcile.ToShortDateString();
			textDebit.ReadOnly=true;
			textCredit.ReadOnly=true;
			butDelete.Enabled=false;
			butChange.Enabled=false;
		}

		///<summary>Need to set AccountPicked before calling this.</summary>
		private void FillAccount(){
			if(_accountPicked==null){
				textAccount.Text="";
				butChange.Text=Lan.g(this,"Pick");
				labelDebit.Text=Lan.g(this,"Debit");
				labelCredit.Text=Lan.g(this,"Credit");
				return;
			}
			//AccountCur=Accounts.ListShort[comboAccount.SelectedIndex];
			textAccount.Text=_accountPicked.Description;
			butChange.Text=Lan.g(this,"Change");
			if(Accounts.DebitIsPos(_accountPicked.AcctType)) {
				labelDebit.Text=Lan.g(this,"Debit")+Lan.g(this,"(+)");
				labelCredit.Text=Lan.g(this,"Credit")+Lan.g(this,"(-)");
				return;
			}
			labelDebit.Text=Lan.g(this,"Debit")+Lan.g(this,"(-)");
			labelCredit.Text=Lan.g(this,"Credit")+Lan.g(this,"(+)");
		}

		/*private void comboAccount_SelectedIndexChanged(object sender,EventArgs e) {
			FillAccount();
		}*/

		private void butChange_Click(object sender,EventArgs e) {
			using FormAccountPick formAccountPick=new FormAccountPick();
			formAccountPick.ShowDialog();
			if(formAccountPick.DialogResult!=DialogResult.OK){
				return;
			}
			_accountPicked=formAccountPick.SelectedAccount;
			FillAccount();
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			JournalEntryCur=null;
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
			if(_accountPicked==null || _accountPicked.AccountNum==0) {
				MsgBox.Show(this,"Please select an account.");
				return;
			}
			JournalEntryCur.AccountNum=_accountPicked.AccountNum;
			JournalEntryCur.DebitAmt=PIn.Double(textDebit.Text);
			JournalEntryCur.CreditAmt=PIn.Double(textCredit.Text);
			JournalEntryCur.Memo=textMemo.Text;
			JournalEntryCur.CheckNumber=textCheckNumber.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(IsNew){
				JournalEntryCur=null;
			}
			DialogResult=DialogResult.Cancel;
		}

		

		

	

	}


}
