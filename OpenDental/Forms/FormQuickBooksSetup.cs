using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.Bridges;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormQuickBooksSetup:FormODBase {
		private List<string> _listDepositAccountsQB;
		private List<string> _listIncomeAccountsQB;
		///<summary>The list of currently available QuickBook classes.  Stored in a preference.</summary>
		private List<string> _listClassRefsQB;

		public FormQuickBooksSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQuickBooksSetup_Load(object sender,EventArgs e) {
			textCompanyFileQB.Text=PrefC.GetString(PrefName.QuickBooksCompanyFile);
			checkQuickBooksClassRefsEnabled.Checked=PrefC.GetBool(PrefName.QuickBooksClassRefsEnabled);
			string acctStr=PrefC.GetString(PrefName.QuickBooksIncomeAccount);
			string[] acctStrArray=acctStr.Split(new char[] {','});
			_listIncomeAccountsQB=new List<string>();
			for(int i=0;i<acctStrArray.Length;i++) {
				if(acctStrArray[i]=="") {
					continue;
				}
				_listIncomeAccountsQB.Add(acctStrArray[i]);
			}
			string depStr=PrefC.GetString(PrefName.QuickBooksDepositAccounts);
			string[] depStrArray=depStr.Split(new char[] {','});
			_listDepositAccountsQB=new List<string>();
			for(int i=0;i<depStrArray.Length;i++) {
				if(depStrArray[i]=="") {
					continue;
				}
				_listDepositAccountsQB.Add(depStrArray[i]);
			}
			string classStr=PrefC.GetString(PrefName.QuickBooksClassRefs);
			string[] classStrArray=classStr.Split(new char[] {','});
			_listClassRefsQB=new List<string>();
			for(int i=0;i<classStrArray.Length;i++) {
				if(classStrArray[i]=="") {
					continue;
				}
				_listClassRefsQB.Add(classStrArray[i]);
			}
			FillQBLists();
		}

		private void FillQBLists() {
			listBoxDepositAccountsQB.Items.Clear();
			listBoxDepositAccountsQB.Items.AddList(_listDepositAccountsQB,x => x.ToString());
			listBoxIncomeAccountsQB.Items.Clear();
			listBoxIncomeAccountsQB.Items.AddList(_listIncomeAccountsQB,x => x.ToString());
			listBoxClassRefsQB.Items.Clear();
			listBoxClassRefsQB.Items.AddList(_listClassRefsQB,x => x.ToString());
		}

		private void butBrowseQB_Click(object sender,EventArgs e) {
			using OpenFileDialog fdlg=new OpenFileDialog();
			fdlg.Title="QuickBooks Company File";
			fdlg.InitialDirectory=@"C:\";
			fdlg.Filter="QuickBooks|*.qbw";
			fdlg.RestoreDirectory=true;
			if(fdlg.ShowDialog()==DialogResult.OK) {
				textCompanyFileQB.Text=fdlg.FileName;
			}
		}

		private void butConnectQB_Click(object sender,EventArgs e) {
			if(textCompanyFileQB.Text.Trim()=="") {
				MsgBox.Show(this,"Browse to your QuickBooks company file first.");
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			string result=QuickBooks.TestConnection(textCompanyFileQB.Text);
			Cursor.Current=Cursors.Default;
			MessageBox.Show(result);
		}

		private void butAddDepositQB_Click(object sender,EventArgs e) {
			List<string> depositList=GetAccountsQB();
			if(depositList!=null) {
				_listDepositAccountsQB.AddRange(depositList);
				FillQBLists();
			}
		}

		private void butRemoveDepositQB_Click(object sender,EventArgs e) {
			if(listBoxDepositAccountsQB.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			_listDepositAccountsQB.RemoveAt(listBoxDepositAccountsQB.SelectedIndex);
			FillQBLists();
		}

		private void butAddIncomeQB_Click(object sender,EventArgs e) {
			List<string> incomeList=GetAccountsQB();
			if(incomeList!=null) {
				_listIncomeAccountsQB.AddRange(incomeList);
				FillQBLists();
			}
		}

		private void butRemoveIncomeQB_Click(object sender,EventArgs e) {
			if(listBoxIncomeAccountsQB.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			_listIncomeAccountsQB.RemoveAt(listBoxIncomeAccountsQB.SelectedIndex);
			FillQBLists();
		}

		private void buttonAddClassRefQB_Click(object sender,EventArgs e) {
			if(textCompanyFileQB.Text.Trim()=="") {
				MsgBox.Show(this,"Browse to your QuickBooks company file first.");
				return;
			}
			if(Prefs.UpdateString(PrefName.QuickBooksCompanyFile,textCompanyFileQB.Text)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			List<string> listClasses=new List<string>();
			try {
				listClasses=QuickBooks.GetListOfClasses();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			using InputBox FormChooseClasses=new InputBox(Lan.g(this,"Choose a class"),listClasses,true);
			FormChooseClasses.TopLevel=true;
			if(FormChooseClasses.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(FormChooseClasses.SelectedIndices.Count < 1) {
				MsgBox.Show(this,"You must choose a class.");
				return;
			}
			foreach(int i in FormChooseClasses.SelectedIndices) {
				string classCur=listClasses[i];
				if(_listClassRefsQB.Contains(classCur)) {
					continue;
				}
				_listClassRefsQB.Add(classCur);
			}
			FillQBLists();
		}

		private void buttonRemoveClassRefQB_Click(object sender,EventArgs e) {
			if(listBoxClassRefsQB.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			_listClassRefsQB.RemoveAt(listBoxClassRefsQB.SelectedIndex);
			FillQBLists();
		}

		///<summary>Launches the account pick window and lets user choose several accounts.  Returns null if anything went wrong or user canceled out.</summary>
		private List<string> GetAccountsQB() {
			if(textCompanyFileQB.Text.Trim()=="") {
				MsgBox.Show(this,"Browse to your QuickBooks company file first.");
				return null;
			}
			if(Prefs.UpdateString(PrefName.QuickBooksCompanyFile,textCompanyFileQB.Text)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			using FormAccountPick FormA=new FormAccountPick();
			FormA.IsQuickBooks=true;
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK) {
				return null;
			}
			if(FormA.ListSelectedAccountsQB!=null) {
				return FormA.ListSelectedAccountsQB;
			}
			return null;
		}

		private void checkQuickBooksClassRefsEnabled_CheckedChanged(object sender,EventArgs e) {
			if(checkQuickBooksClassRefsEnabled.Checked) {
				labelClass.Visible=true;
				listBoxClassRefsQB.Visible=true;
				buttonAddClassRefQB.Visible=true;
				buttonRemoveClassRefQB.Visible=true;
			}
			else {
				labelClass.Visible=false;
				listBoxClassRefsQB.Visible=false;
				buttonAddClassRefQB.Visible=false;
				buttonRemoveClassRefQB.Visible=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			string depStr="";
			for(int i=0;i<listBoxDepositAccountsQB.Items.Count;i++) {
				if(i>0) {
					depStr+=",";
				}
				depStr+=listBoxDepositAccountsQB.Items.GetTextShowingAt(i);
			}
			string incomeStr="";
			for(int i=0;i<listBoxIncomeAccountsQB.Items.Count;i++) {
				if(i>0) {
					incomeStr+=",";
				}
				incomeStr+=listBoxIncomeAccountsQB.Items.GetTextShowingAt(i);
			}
			string classStr="";
			for(int i=0;i<listBoxClassRefsQB.Items.Count;i++) {
				if(i>0) {
					classStr+=",";
				}
				classStr+=listBoxClassRefsQB.Items.GetTextShowingAt(i);
			}
			if(Prefs.UpdateString(PrefName.QuickBooksCompanyFile,textCompanyFileQB.Text)
				| Prefs.UpdateString(PrefName.QuickBooksDepositAccounts,depStr)
				| Prefs.UpdateString(PrefName.QuickBooksIncomeAccount,incomeStr)
				| Prefs.UpdateBool(PrefName.QuickBooksClassRefsEnabled,checkQuickBooksClassRefsEnabled.Checked)
				| Prefs.UpdateString(PrefName.QuickBooksClassRefs,classStr)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}