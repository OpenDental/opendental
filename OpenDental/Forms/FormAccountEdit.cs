using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>Allows user to edit account. Form can be found at Manage->Accounting->Click on any Account->Edit</summary>
	public partial class FormAccountEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Account _account;
		private Account _accountOld;

		///<summary></summary>
		public FormAccountEdit(Account accountCur)
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_account=accountCur;
		}

		private void FormAccountEdit_Load(object sender, System.EventArgs e) {
			_accountOld=_account.Clone();
			textDescription.Text=_account.Description;
			for(int i=0;i<Enum.GetNames(typeof(AccountType)).Length;i++){
				listAcctType.Items.Add(Lan.g("enumAccountType",Enum.GetNames(typeof(AccountType))[i]));
				if((int)_account.AcctType==i){
					listAcctType.SelectedIndex=i;
				}
			}
			textBankNumber.Text=_account.BankNumber;
			checkInactive.Checked=_account.Inactive;
			butColor.BackColor=_account.AccountColor;
			if(_account.IsRetainedEarnings){
				checkRetainedEarnings.Visible=true;
				checkRetainedEarnings.Checked=true;
				checkRetainedEarnings.Enabled=false;
				labelRetainedEarnings.Visible=true;
				listAcctType.Enabled=false;
				checkInactive.Enabled=false;
				butDelete.Enabled=false;
			}
			else{
				checkRetainedEarnings.Visible=false;
				labelRetainedEarnings.Visible=false;
			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			colorDialog.ShowDialog();
			butColor.BackColor=colorDialog.Color;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			try{
				Accounts.Delete(_account);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description is required.");
				return;
			}
			if(!IsNew
				&& _account.Description != textDescription.Text
				&& JournalEntries.IsInUse(_account.AccountNum))
			{ 
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will update the Splits column for all Transactions attached to this account that have a date "
					+"after the Accounting Lock Date. Are you sure you want to continue?")) 
				{ 
					return;
				}
			}
			_account.Description=textDescription.Text;
			_account.AcctType=(AccountType)listAcctType.SelectedIndex;
			_account.BankNumber=textBankNumber.Text;
			_account.Inactive=checkInactive.Checked;
			_account.AccountColor=butColor.BackColor;
			if(IsNew){
				Accounts.Insert(_account);
			}
			else{
				Accounts.Update(_account,_accountOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}





















