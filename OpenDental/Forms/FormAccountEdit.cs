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
		private Account _accountCur;
		private Account _accountOld;

		///<summary></summary>
		public FormAccountEdit(Account accountCur)
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_accountCur=accountCur;
		}

		private void FormAccountEdit_Load(object sender, System.EventArgs e) {
			_accountOld=_accountCur.Clone();
			textDescription.Text=_accountCur.Description;
			for(int i=0;i<Enum.GetNames(typeof(AccountType)).Length;i++){
				listAcctType.Items.Add(Lan.g("enumAccountType",Enum.GetNames(typeof(AccountType))[i]));
				if((int)_accountCur.AcctType==i){
					listAcctType.SelectedIndex=i;
				}
			}
			textBankNumber.Text=_accountCur.BankNumber;
			checkInactive.Checked=_accountCur.Inactive;
			butColor.BackColor=_accountCur.AccountColor;
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
				Accounts.Delete(_accountCur);
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
			if(_accountCur.Description != textDescription.Text) {  
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will update the Splits column for all Transactions attached to this account that have a date "
					+"after the Accounting Lock Date. Are you sure you want to continue?")) 
				{ 
					return;
				}
			}
			_accountCur.Description=textDescription.Text;
			_accountCur.AcctType=(AccountType)listAcctType.SelectedIndex;
			_accountCur.BankNumber=textBankNumber.Text;
			_accountCur.Inactive=checkInactive.Checked;
			_accountCur.AccountColor=butColor.BackColor;
			if(IsNew){
				Accounts.Insert(_accountCur);
			}
			else{
				Accounts.Update(_accountCur,_accountOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















