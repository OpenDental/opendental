using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormQBAccountSelect:FormODBase {
		///<summary>List of deposit accounts from pref. for QuickBooks</summary>
		private List<string> _listDepositAccountsQB;
		///<summary>List of income accounts from pref. for QuickBooks</summary>
		private List<string> _listIncomeAccountsQB;
		///<summary>The selected account when clicking OK.  Used in FormDepositEdit to pass to QuickBooks or QuickBooks Online.</summary>
		public string DepositAccountSelected="";
		///<summary>The selected account when clicking OK.  Used in FormDepositEdit to pass to QuickBooks or QuickBooks Online.</summary>
		public string IncomeAccountSelected="";
		///<summary>True if user is using QuickBooks Online instead of regular QuickBooks.</summary>
		private bool _isQuickBooksOnline;
		///<summary>Contains the string of deposit accounts for QuickBooks Online.</summary>
		private ProgramProperty _programPropertyQboDepositAccounts;
		///<summary>Contains the string of income accounts for QuickBooks Online.</summary>
		private ProgramProperty _programPropertyQboIncomeAccounts;

		public FormQBAccountSelect(bool isQuickBooksOnline) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isQuickBooksOnline=isQuickBooksOnline;
		}

		private void FormQBAccountSelect_Load(object sender,EventArgs e) {
			if(_isQuickBooksOnline) {
				Program programQbo=Programs.GetCur(ProgramName.QuickBooksOnline);
				_programPropertyQboDepositAccounts=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Deposit Accounts");
				_programPropertyQboIncomeAccounts=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Income Accounts");
				FillQuickBooksOnlineAccounts();
				return;
			}
			FillQuickBooksAccounts();
			
		}

		private void FillQuickBooksAccounts() {
			_listDepositAccountsQB=Accounts.GetDepositAccountsQB();
			for(int i=0;i<_listDepositAccountsQB.Count;i++) {
				comboDepositAccount.Items.Add(_listDepositAccountsQB[i]);
			}
			comboDepositAccount.SelectedIndex=0;
			_listIncomeAccountsQB=Accounts.GetIncomeAccountsQB();
			for(int i=0;i<_listIncomeAccountsQB.Count;i++) {
				comboIncomeAccount.Items.Add(_listIncomeAccountsQB[i]);
			}
			comboIncomeAccount.SelectedIndex=0;
		}

		private void FillQuickBooksOnlineAccounts() {
			FillAccountsFromProgramProperty(_programPropertyQboDepositAccounts,comboDepositAccount);
			comboDepositAccount.SelectedIndex=0;
			FillAccountsFromProgramProperty(_programPropertyQboIncomeAccounts,comboIncomeAccount);
			comboIncomeAccount.SelectedIndex=0;//change this combo box name.
		}

		private void FillAccountsFromProgramProperty(ProgramProperty progProp,OpenDental.UI.ComboBox comboBox) {
			List<string> listAccountsNames=ProgramProperties.GetQuickBooksOnlineEntityNames(progProp.PropertyValue);
			for(int i=0;i<listAccountsNames.Count;i++) {
				comboBox.Items.Add(listAccountsNames[i]);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_isQuickBooksOnline) {
				DepositAccountSelected=comboDepositAccount.SelectedItem.ToString();
				IncomeAccountSelected=comboIncomeAccount.SelectedItem.ToString();
			}
			else {
				DepositAccountSelected=_listDepositAccountsQB[comboDepositAccount.SelectedIndex];
				IncomeAccountSelected=_listIncomeAccountsQB[comboIncomeAccount.SelectedIndex];
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}