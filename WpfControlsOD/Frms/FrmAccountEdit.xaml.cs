using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>Allows user to edit account. Form can be found at Manage->Accounting->Click on any Account->Edit</summary>
	public partial class FrmAccountEdit : FrmODBase {
		///<summary></summary>
		public bool IsNew;
		private Account _account;
		private Account _accountOld;

		///<summary></summary>
		public FrmAccountEdit(Account accountCur)
		{
			InitializeComponent();
			//Lan.F(this);
			_account=accountCur;
		}

		private void FrmAccountEdit_Loaded(object sender, RoutedEventArgs e) {
			_accountOld=_account.Clone();
			textDescription.Text=_account.Description;
			for(int i=0;i<Enum.GetNames(typeof(AccountType)).Length;i++){
				listAcctType.Items.Add(Lans.g("enumAccountType",Enum.GetNames(typeof(AccountType))[i]));
				if((int)_account.AcctType==i){
					listAcctType.SelectedIndex=i;
				}
			}
			textBankNumber.Text=_account.BankNumber;
			checkInactive.Checked=_account.Inactive;
			panelColor.ColorBack=ColorOD.ToWpf(_account.AccountColor);
			if(_account.IsRetainedEarnings){
				checkRetainedEarnings.Visible=true;
				checkRetainedEarnings.Checked=true;
//todo: IsEnabled not showing properly
				checkRetainedEarnings.IsEnabled=false;
				labelRetainedEarnings.Visible=true;
				listAcctType.IsEnabled=false;
				checkInactive.IsEnabled=false;
				butDelete.IsEnabled=false;
			}
			else{
				checkRetainedEarnings.Visible=false;
				labelRetainedEarnings.Visible=false;
			}
		}

		private void butColor_Click(object sender,EventArgs e) {
			FrmColorDialog colorDialog=new FrmColorDialog();
			colorDialog.Color=panelColor.ColorBack;
			colorDialog.ShowDialog();
			panelColor.ColorBack=colorDialog.Color;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				return;
			}
			try{
				Accounts.Delete(_account);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			IsDialogOK=true;
		}

		private void butSave_Click(object sender, EventArgs e) {
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
			_account.Inactive=checkInactive.Checked.Value;
			_account.AccountColor=ColorOD.FromWpf(panelColor.ColorBack);
			if(IsNew){
				Accounts.Insert(_account);
			}
			else{
				Accounts.Update(_account,_accountOld);
			}
			IsDialogOK=true;
		}
	}
}





















