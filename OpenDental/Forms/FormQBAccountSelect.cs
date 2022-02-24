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
		private ProgramProperty _programPropQboDepositAccounts;
		///<summary>Contains the string of income accounts for QuickBooks Online.</summary>
		private ProgramProperty _programPropQboIncomeAccounts;

		///<summary>The ID associated to the QuickBooks Online Depesoit account selected.</summary>
		public string DepositAccountId {
			get {
				return ProgramProperties.GetQuickBooksOnlineEntityIdByName(_programPropQboDepositAccounts.PropertyValue,DepositAccountSelected);
			}
		}

		///<summary>The ID associated to the QuickBooks Online Income account selected.</summary>
		public string IncomeAccountId {
			get {
				return ProgramProperties.GetQuickBooksOnlineEntityIdByName(_programPropQboIncomeAccounts.PropertyValue,IncomeAccountSelected);
			}
		}

		public FormQBAccountSelect(bool isQuickBooksOnline) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isQuickBooksOnline=isQuickBooksOnline;
		}

		private void FormQBAccountSelect_Load(object sender,EventArgs e) {
			if(_isQuickBooksOnline) {
				Program progQbo=Programs.GetCur(ProgramName.QuickBooksOnline);
				_programPropQboDepositAccounts=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Deposit Accounts");
				_programPropQboIncomeAccounts=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Income Accounts");
				FillQuickBooksOnlineAccounts();
			}
			else {
				FillQuickBooksAccounts();
			}
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
			FillAccountsFromProgramProperty(_programPropQboDepositAccounts,comboDepositAccount);
			comboDepositAccount.SelectedIndex=0;
			FillAccountsFromProgramProperty(_programPropQboIncomeAccounts,comboIncomeAccount);
			comboIncomeAccount.SelectedIndex=0;//change this combo box name.
		}

		private void FillAccountsFromProgramProperty(ProgramProperty progProp,ComboBox comboBox) {
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