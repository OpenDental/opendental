using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;
using System.Diagnostics;
using System.Linq;
using CodeBase;
using System.IO;
using OpenDental.Thinfinity;
using Bridges;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormDepositEdit:FormODBase {
		private Deposit _depositCur;
		private Deposit _depositOld;
		///<summary></summary>
		public bool IsNew;
		private List<ClaimPayment> _listClaimPayments;
		private List<Payment> _listPaymentsPat;
		private bool _isChanged;
		///<summary>Only used if linking to accounts</summary>
		private List<long> _listDepositAccounts;
		///<summary>The accounting software selected in Manage Module preferences.</summary>
		private AccountingSoftware _accountingSoftware;
		///<summary>OAuth access token stored in DB for QuickBooks Online API.</summary>
		private ProgramProperty _programPropertyQboAccessToken;
		///<summary>OAuth refresh token stored in DB for QuickBooks Online API.</summary>
		private ProgramProperty _programPropertyQboRefreshToken;
		///<summary>Current OAuth access token for QuickBooks Online API.</summary>
		private QuickBooksOnlineToken _quickBooksOnlineToken;
		///<summary>Class Refs for QuickBooks Online.</summary>
		private ProgramProperty _programPropertyQboClassRefs;
		///<summary>Realm ID for QuickBooks Online.</summary>
		private ProgramProperty _programPropertyQboRealmId;
		///<summary>Used to store DefNums in a 1:1 ratio for listInsPayType</summary>
		private List<long> _listInsPayTypeDefNums;
		///<summary>Used to store DefNums in a 1:1 ratio for listPayType</summary>
		private List<long> _listPayTypeDefNums;
		///<summary>Keeps track of whether the payment has been saved to the database since the form was opened.</summary>
		private bool _hasBeenSavedToDB;
		///<summary>A list of payNums already attached to the deposit.  When printing or showing PDF these were attached to the deposit.
		///Used on OK click to make sure we detach any procedures that might have been unselected after they've been attached in the DB.</summary>
		private List<long> _listPayNumsAttached=new List<long>();
		///<summary>A list of claimPaymentNum already attached to the deposit.  When printing or showing PDF these were attached to the deposit.
		///Used on OK click to make sure we detach any procedures that might have been unselected after they've been attached in the DB.</summary>
		private List<long> _listClaimPaymentNumsAttached=new List<long>();
		///<summary>Used in UpdateToDB to detach any payments that were attached to deposit but have been deselected before clicking OK.</summary>
		private bool _isOnOKClick=false;

		///<summary>True if the accounting software pref is set to QuickBooks.</summary>
		private bool IsQuickBooks() {
			return _accountingSoftware==AccountingSoftware.QuickBooks;
		}

		///<summary>True if the accounting software pref is set to QuickBooks Online.</summary>
		private bool IsQuickBooksOnline() {
			return _accountingSoftware==AccountingSoftware.QuickBooksOnline;
		}

		///<summary></summary>
		public FormDepositEdit(Deposit depositCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_depositCur=depositCur;
			_depositOld=depositCur.Copy();
		}

		private void FormDepositEdit_Load(object sender,System.EventArgs e) {
			butSendQB.Visible=false;
			_accountingSoftware=PrefC.GetEnum<AccountingSoftware>(PrefName.AccountingSoftware);
			if(IsQuickBooks()) {
				butSendQB.Text="&Send QB";
			}
			else if(IsQuickBooksOnline()) {
				butSendQB.Text="&Send QBO";
				Program programQbo=Programs.GetCur(ProgramName.QuickBooksOnline);
				_programPropertyQboAccessToken=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Access Token");
				_programPropertyQboRefreshToken=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Refresh Token");
				_quickBooksOnlineToken=new QuickBooksOnlineToken(_programPropertyQboAccessToken.PropertyValue,_programPropertyQboRefreshToken.PropertyValue);
				_programPropertyQboClassRefs=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Class Refs");
				_programPropertyQboRealmId=ProgramProperties.GetPropForProgByDesc(programQbo.ProgramNum,"Realm ID");
			}
			if(IsNew) {
				if(!Security.IsAuthorized(Permissions.DepositSlips,DateTime.Today)) {
					//we will check the date again when saving
					DialogResult=DialogResult.Cancel;
					return;
				}
			}
			else {
				//We enforce security here based on date displayed, not date entered
				if(!Security.IsAuthorized(Permissions.DepositSlips,_depositCur.DateDeposit)) {
					butOK.Enabled=false;
					butDelete.Enabled=false;
				}
			}
			if(PrefC.GetBool(PrefName.ShowAutoDeposit)) {
				labelDepositAccountNum.Visible=true;
				comboDepositAccountNum.Visible=true;
				//Fill deposit account num drop down
				comboDepositAccountNum.Items.AddDefs(Defs.GetDefsForCategory(DefCat.AutoDeposit,true));
				comboDepositAccountNum.SetSelectedDefNum(_depositCur.DepositAccountNum);
			}
			if(IsNew) {
				textDateStart.Text=PIn.Date(PrefC.GetString(PrefName.DateDepositsStarted)).ToShortDateString();
				if(!PrefC.HasClinicsEnabled) {
					comboClinic.Visible=false;
					labelClinic.Visible=false;
				}
				if(Clinics.ClinicNum==0) {
					comboClinic.IsAllSelected=true;
				}
				else {
					comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				}
				
				List<Def> listDefsPaymentTypes=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
				List<Def> listDefsInsurancePaymentTypes=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
				_listPayTypeDefNums=new List<long>();
				for(int i=0;i<listDefsPaymentTypes.Count;i++) {
					if(listDefsPaymentTypes[i].ItemValue!="") {
						continue;//skip defs not selected for deposit slip
					}
					listPayType.Items.Add(listDefsPaymentTypes[i].ItemName);
					_listPayTypeDefNums.Add(listDefsPaymentTypes[i].DefNum);
					listPayType.SetSelected(listPayType.Items.Count-1,true);
				}
				_listInsPayTypeDefNums=new List<long>();
				for(int i=0;i<listDefsInsurancePaymentTypes.Count;i++) {
					if(listDefsInsurancePaymentTypes[i].ItemValue!="") {
						continue;//skip defs not selected for deposit slip
					}
					listInsPayType.Items.Add(listDefsInsurancePaymentTypes[i].ItemName);
					_listInsPayTypeDefNums.Add(listDefsInsurancePaymentTypes[i].DefNum);
					listInsPayType.SetSelected(listInsPayType.Items.Count-1,true);
				}
				textDepositAccount.Visible=false;//this is never visible for new. It's a description if already attached.
				if(Accounts.DepositsLinked() && !IsQuickBooks() && !IsQuickBooksOnline()) {
					_listDepositAccounts=Accounts.GetDepositAccounts();
					for(int i=0;i<_listDepositAccounts.Count;i++) {
						comboDepositAccount.Items.Add(Accounts.GetDescript(_listDepositAccounts[i]));
					}
					comboDepositAccount.SelectedIndex=0;
				}
				else {
					labelDepositAccount.Visible=false;
					comboDepositAccount.Visible=false;
				}
			}
			else {//Not new.
				groupSelect.Visible=false;
				gridIns.SelectionMode=GridSelectionMode.None;
				gridPat.SelectionMode=GridSelectionMode.None;
				//we never again let user change the deposit linking again from here.
				//They need to detach it from within the transaction
				//Might be enhanced later to allow, but that's very complex.
				Transaction transaction=Transactions.GetAttachedToDeposit(_depositCur.DepositNum);
				if(transaction==null) {
					labelDepositAccount.Visible=false;
					comboDepositAccount.Visible=false;
					textDepositAccount.Visible=false;
				}
				else {
					comboDepositAccount.Enabled=false;
					labelDepositAccount.Text=Lan.g(this,"Deposited into Account");
					List<JournalEntry> listJournalEntries=JournalEntries.GetForTrans(transaction.TransactionNum);
					for(int i=0;i<listJournalEntries.Count;i++) {
						if(Accounts.GetAccount(listJournalEntries[i].AccountNum).AcctType!=AccountType.Asset) {
							continue;
						}
						comboDepositAccount.Items.Add(Accounts.GetDescript(listJournalEntries[i].AccountNum));
						comboDepositAccount.SelectedIndex=0;
						textDepositAccount.Text=listJournalEntries[i].DateDisplayed.ToShortDateString()
							+" "+listJournalEntries[i].DebitAmt.ToString("c");
						break;
					}
				}
			}
			//If in QuickBooks or QuickBooks Online mode, hide dropdown because its handled in FormQBAccountSelect.cs.
			if(IsQuickBooks() || IsQuickBooksOnline()) {
				textDepositAccount.Visible=false;
				labelDepositAccount.Visible=false;
				comboDepositAccount.Visible=false;
				comboDepositAccount.Enabled=false;
				if(Accounts.DepositsLinked() && !IsNew) {
					//Show SendQB button so that users can send old deposits into QB.
					butSendQB.Visible=true;
				}
			}
			FillClassRefs();
			textDate.Text=_depositCur.DateDeposit.ToShortDateString();
			textAmount.Text=_depositCur.Amount.ToString("F");
			textBankAccountInfo.Text=_depositCur.BankAccountInfo;
			textMemo.Text=_depositCur.Memo;
			textBatch.Text=_depositCur.Batch;
			FillGrids();
			if(IsNew) {
				gridPat.SetAll(true);
				gridIns.SetAll(true);
			}
			ComputeAmt();
		}

		private void FillClassRefs() {
			bool isQBAndClassRefsEnabled=IsQuickBooks() && PrefC.GetBool(PrefName.QuickBooksClassRefsEnabled);
			bool isQBOAndClassRefsEnabled=IsQuickBooksOnline() && _programPropertyQboClassRefs.PropertyValue!="";
			if(!isQBAndClassRefsEnabled && !isQBOAndClassRefsEnabled){//Neither QB nor QBO and their respective class refs feature is on, nothing to fill.
				return;
			}
			if(!IsNew) {
				//Show groupbox and hide all the controls except for labelClassRef and comboClassRefs
				groupSelect.Visible=true;
				label5.Visible=false;
				textDateStart.Visible=false;
				labelClinic.Visible=false;
				comboClinic.Visible=false;
				label2.Visible=false;
				label6.Visible=false;
				listInsPayType.Visible=false;
				listPayType.Visible=false;
				butRefresh.Visible=false;
			}
			labelClassRef.Visible=true;
			comboClassRefs.Visible=true;
			List<string> listClassNames=new List<string>();
			if(IsQuickBooks()) {
				listClassNames=PrefC.GetString(PrefName.QuickBooksClassRefs).Split(',').ToList();
			}
			else if(IsQuickBooksOnline()) {
				listClassNames=ProgramProperties.GetQuickBooksOnlineEntityNames(_programPropertyQboClassRefs.PropertyValue);
			}
			for(int i=0;i<listClassNames.Count;i++) {
				if(listClassNames[i]=="") {
					continue;
				}
				comboClassRefs.Items.Add(listClassNames[i]);
			}
		}

		///<summary></summary>
		private void FillGrids(){
			if(IsNew){
				DateTime dateTimeStart=PIn.Date(textDateStart.Text);
				long clinicNum=0;
				if(!comboClinic.IsAllSelected){
					clinicNum=comboClinic.SelectedClinicNum;
				}
				List<long> listPayTypes=new List<long>();//[listPayType.SelectedIndices.Count];
				for(int i=0;i<listPayType.SelectedIndices.Count;i++) {
					listPayTypes.Add(_listPayTypeDefNums[listPayType.SelectedIndices[i]]);
				}
				List<long> listInsPayTypes=new List<long>();
				for(int i=0;i<listInsPayType.SelectedIndices.Count;i++) {
					listInsPayTypes.Add(_listInsPayTypeDefNums[listInsPayType.SelectedIndices[i]]);
				}
				_listPaymentsPat=new List<Payment>();
				if(listPayTypes.Count!=0) {
					_listPaymentsPat=Payments.GetForDeposit(dateTimeStart,clinicNum,listPayTypes);
				}
				_listClaimPayments=new List<ClaimPayment>();
				if(listInsPayTypes.Count!=0) {
					_listClaimPayments=ClaimPayments.GetForDeposit(dateTimeStart,clinicNum,listInsPayTypes);
				}
				//new deposit, but has been saved to db (pressed print/PDF/email buttons), get trans already attached to deposit in db as well as unattached
				if(_hasBeenSavedToDB && _depositCur.DepositNum>0) {
					_listPaymentsPat=_listPaymentsPat.Concat(Payments.GetForDeposit(_depositCur.DepositNum))
						.OrderBy(x => x.PayDate).ThenBy(x => x.PayNum).ToList();
					_listClaimPayments=_listClaimPayments.Concat(ClaimPayments.GetForDeposit(_depositCur.DepositNum))
						.OrderBy(x => x.CheckDate).ThenBy(x => x.ClaimPaymentNum).ToList();
				}
			}
			else{
				_listPaymentsPat=Payments.GetForDeposit(_depositCur.DepositNum);
				_listClaimPayments=ClaimPayments.GetForDeposit(_depositCur.DepositNum).ToList();
			}
			//Fill Patient Payment Grid---------------------------------------
			List<long> listPatNums=new List<long>();
			for(int i=0;i<_listPaymentsPat.Count;i++){
				listPatNums.Add(_listPaymentsPat[i].PatNum);
			}
			Patient[] patientArray=Patients.GetMultPats(listPatNums);
			gridPat.BeginUpdate();
			gridPat.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableDepositSlipPat","Date"),80);
			gridPat.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipPat","Patient"),150);
			gridPat.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipPat","Type"),70);
			gridPat.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipPat","Check Number"),95);
			gridPat.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipPat","Bank-Branch"),80);
			gridPat.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipPat","Amount"),80);
			gridPat.ListGridColumns.Add(col);
			gridPat.ListGridRows.Clear();
			OpenDental.UI.GridRow row;
			for(int i=0;i<_listPaymentsPat.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listPaymentsPat[i].PayDate.ToShortDateString());
				row.Cells.Add(Patients.GetOnePat(patientArray,_listPaymentsPat[i].PatNum).GetNameLF());
				row.Cells.Add(Defs.GetName(DefCat.PaymentTypes,_listPaymentsPat[i].PayType));
				row.Cells.Add(_listPaymentsPat[i].CheckNum);
				row.Cells.Add(_listPaymentsPat[i].BankBranch);
				row.Cells.Add(_listPaymentsPat[i].PayAmt.ToString("F"));
				gridPat.ListGridRows.Add(row);
			}
			gridPat.EndUpdate();
			//Fill Insurance Payment Grid-------------------------------------
			gridIns.BeginUpdate();
			gridIns.ListGridColumns.Clear();
			col=new GridColumn(Lan.g("TableDepositSlipIns","Date"),80);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipIns","Carrier"),150);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipIns","Type"),70);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipIns","Check Number"),95);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipIns","Bank-Branch"),80);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableDepositSlipIns","Amount"),90);
			gridIns.ListGridColumns.Add(col);
			gridIns.ListGridRows.Clear();
			for(int i=0;i<_listClaimPayments.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listClaimPayments[i].CheckDate.ToShortDateString());
				row.Cells.Add(_listClaimPayments[i].CarrierName);
				row.Cells.Add(Defs.GetName(DefCat.InsurancePaymentType,_listClaimPayments[i].PayType));
				row.Cells.Add(_listClaimPayments[i].CheckNum);
				row.Cells.Add(_listClaimPayments[i].BankBranch);
				row.Cells.Add(_listClaimPayments[i].CheckAmt.ToString("F"));
				gridIns.ListGridRows.Add(row);
			}
			gridIns.EndUpdate();
		}

		///<summary>Usually run after any selected items changed. Recalculates amt based on selected items or row count.  May get fired twice when click
		///and mouse up, harmless.</summary>
		private void ComputeAmt(){
			if(IsNew) {
				textItemNum.Text=(gridIns.SelectedIndices.Length+gridPat.SelectedIndices.Length).ToString();
			}
			else {//if not new, amount cannot be changed, return
				textItemNum.Text=(gridIns.ListGridRows.Count+gridPat.ListGridRows.Count).ToString();
				return;
			}
			decimal amount=0;
			for(int i=0;i<gridPat.SelectedIndices.Length;i++){
				amount+=(decimal)_listPaymentsPat[gridPat.SelectedIndices[i]].PayAmt;
			}
			for(int i=0;i<gridIns.SelectedIndices.Length;i++){
				amount+=(decimal)_listClaimPayments[gridIns.SelectedIndices[i]].CheckAmt;
			}
			textAmount.Text=amount.ToString("F");
			_depositCur.Amount=(double)amount;
		}

		private void Search() {
			bool isScrollSet=false;
			for(int i=0;i<gridIns.ListGridRows.Count;i++) {
				bool isBold=false;
				if(textAmountSearch.Text!="" && gridIns.ListGridRows[i].Cells[5].Text.ToUpper().Contains(textAmountSearch.Text.ToUpper())) {
					isBold=true;
				}
				if(textCheckNumSearch.Text!="" && gridIns.ListGridRows[i].Cells[3].Text.ToUpper().Contains(textCheckNumSearch.Text.ToUpper())) {
					isBold=true;
				}
				gridIns.ListGridRows[i].Bold=isBold;
				if(isBold) {
					gridIns.ListGridRows[i].ColorText=Color.Red;					
					if(!isScrollSet) {//scroll to the first match in the list.
						gridIns.ScrollToIndex(i);
						isScrollSet=true;
					}
				}
				else {//Standard row.
					gridIns.ListGridRows[i].ColorText=Color.Black;
				}
			}//end i
			gridIns.Invalidate();
			bool isScrollSetPat=false;
			for(int i=0;i<gridPat.ListGridRows.Count;i++) {
				bool isBold=false;
				if(textAmountSearch.Text!="" && gridPat.ListGridRows[i].Cells[5].Text.ToUpper().Contains(textAmountSearch.Text.ToUpper())) {
					isBold=true;
				}
				if(textCheckNumSearch.Text!="" && gridPat.ListGridRows[i].Cells[3].Text.ToUpper().Contains(textCheckNumSearch.Text.ToUpper())) {
					isBold=true;
				}
				gridPat.ListGridRows[i].Bold=isBold;
				if(isBold) {
					gridPat.ListGridRows[i].ColorText=Color.Red;
					if(!isScrollSetPat) {//scroll to the first match in the list.
						gridPat.ScrollToIndex(i);
						isScrollSetPat=true;
					}
				}
				else {//Standard row.
					gridPat.ListGridRows[i].ColorText=Color.Black;
				}
			}//end i
			gridPat.Invalidate();
		}

		///<summary>Returns true if a deposit was created.</summary>
		private bool CreateDepositQbOrQbo(bool allowContinue) {
			if(IsQuickBooks()) {
				return CreateDepositQB(allowContinue);
			}
			else if(IsQuickBooksOnline()) {
				return CreateDepositQBO(allowContinue);
			}
			return false;
		}

		///<summary>Returns true if a deposit was created OR if the user clicked continue anyway on pop up.</summary>
		private bool CreateDepositQB(bool allowContinue) {
			using FormQBAccountSelect formQBAccountSelect=new FormQBAccountSelect(false);
			formQBAccountSelect.ShowDialog();
			if(formQBAccountSelect.DialogResult!=DialogResult.OK) {//Close/Cancel Send QB
				return false;
			}
			Cursor.Current=Cursors.WaitCursor;
			string classRef="";
			if(PrefC.GetBool(PrefName.QuickBooksClassRefsEnabled) && comboClassRefs.SelectedItem!=null) {
				classRef=comboClassRefs.SelectedItem.ToString();
			}
			try {
				if(CompareDouble.IsLessThanOrEqualToZero(_depositCur.Amount)) {
					throw new ODException(Lan.g(this,"Only deposits greater than zero can be sent to QuickBooks."));
				}
				QuickBooks.CreateDeposit(_depositCur.DateDeposit
					,formQBAccountSelect.DepositAccountSelected
					,formQBAccountSelect.IncomeAccountSelected
					,_depositCur.Amount
					,textMemo.Text
					,classRef);//if classRef=="" then it will be safely ignored here
			}
			catch(Exception ex) {
				Cursor.Current=Cursors.Default;
				if(allowContinue) {
					if(MessageBox.Show(ex.Message+"\r\n\r\n"
						+Lan.g(this,"The deposit has not been created in QuickBooks. Would you like to create the deposit locally anyway?")
						,Lan.g(this,"QuickBooks Deposit Create Failed")
						,MessageBoxButtons.YesNo)==DialogResult.Yes) 
					{
						return true;
					}
				}
				else {
					MessageBox.Show(ex.Message,Lan.g(this,"QuickBooks Deposit Create Failed"));
				}
				return false;//Did not want to continue creating regular deposit or failed creating QuickBooks Deposit with no option to continue.
			}
			//Everything below only happens when QuickBooks deposit was successful.
			SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,Lan.g(this,"Deposit slip sent to QuickBooks.")+"\r\n"
				+Lan.g(this,"Deposit date")+": "+_depositCur.DateDeposit.ToShortDateString()+" "+Lan.g(this,"for")+" "+_depositCur.Amount.ToString("c"));
			Cursor.Current=Cursors.Default;
			MsgBox.Show(this,"Deposit successfully sent to QuickBooks.");
			butSendQB.Enabled=false;//Don't let user send same deposit more than once.  
			return true;
		}

		private bool CreateDepositQBO(bool allowContinue) {
			if(_quickBooksOnlineToken.AccessToken.IsNullOrEmpty()) {
				MsgBox.Show(this,"You don't have a QuickBooks Online access token. Please go to the QuickBooks Online Setup window, and click the Authenticate button to obtain one.");
				return false;
			}
			using FormQBAccountSelect formQBAccountSelect=new FormQBAccountSelect(true);
			formQBAccountSelect.ShowDialog();
			if(formQBAccountSelect.DialogResult!=DialogResult.OK) {//Close/Cancel send QBO
				return false;
			}
			Cursor.Current=Cursors.WaitCursor;
			QuickBooksOnlineClassRef quickBooksOnlineClassRef=null;
			QuickBooksOnlineAccount quickBooksOnlineAccountDeposit=null;
			QuickBooksOnlineAccount quickBooksOnlineAccountIncome=null;
			QuickBooksOnlineDepositResponse quickBooksOnlineDepositResponse=null;
			List<decimal> listSelectedPaymentAmounts=GetListSelectedPaymentAmounts();
			try {
				if(CompareDecimal.IsLessThanOrEqualToZero(listSelectedPaymentAmounts.Sum())) {
					throw new ODException(Lan.g(this,"Only deposits greater than zero can be sent to QuickBooks Online."));
				}
				if(_programPropertyQboClassRefs.PropertyValue!="" && comboClassRefs.SelectedItem!=null) {
					string classRefName=comboClassRefs.SelectedItem.ToString();
					string classRefId=ProgramProperties.GetQuickBooksOnlineEntityIdByName(_programPropertyQboClassRefs.PropertyValue,classRefName);
					quickBooksOnlineClassRef=new QuickBooksOnlineClassRef(classRefName,classRefId);
				}
				quickBooksOnlineAccountDeposit=new QuickBooksOnlineAccount(formQBAccountSelect.DepositAccountSelected,formQBAccountSelect.DepositAccountId);
				quickBooksOnlineAccountIncome=new QuickBooksOnlineAccount(formQBAccountSelect.IncomeAccountSelected,formQBAccountSelect.IncomeAccountId);
				quickBooksOnlineDepositResponse=QuickBooksOnline.CreateDeposit(_programPropertyQboRealmId.PropertyValue,_quickBooksOnlineToken
					,quickBooksOnlineAccountDeposit,quickBooksOnlineAccountIncome,quickBooksOnlineClassRef,listSelectedPaymentAmounts,textMemo.Text,_depositCur.DateDeposit);
				if(quickBooksOnlineDepositResponse.RequestException!=null) {
					if(QuickBooksOnline.RefreshTokensIfNeeded(quickBooksOnlineDepositResponse.RequestException,_quickBooksOnlineToken)) {
						SaveTokens();
						//Try again after token refresh
						quickBooksOnlineDepositResponse=QuickBooksOnline.CreateDeposit(_programPropertyQboRealmId.PropertyValue,_quickBooksOnlineToken
							,quickBooksOnlineAccountDeposit,quickBooksOnlineAccountIncome,quickBooksOnlineClassRef,listSelectedPaymentAmounts,textMemo.Text,_depositCur.DateDeposit);
					}
					//Check again as a different error may have occured.
					if(quickBooksOnlineDepositResponse.RequestException!=null) {
						throw quickBooksOnlineDepositResponse.RequestException;
					}
				}
			}
			catch(Exception ex) {
				Cursor.Current=Cursors.Default;
				if(allowContinue) {
					if(MessageBox.Show(ex.Message+"\r\n\r\n"
						+Lan.g(this,"The deposit has not been created in QuickBooks Online. Would you like to create the deposit locally anyway?")
						,Lan.g(this,"QuickBooks Online Deposit Create Failed")
						,MessageBoxButtons.YesNo)==DialogResult.Yes)
					{
						return true;
					}
				}
				else {
					MessageBox.Show(ex.Message,Lan.g(this,"QuickBooks Online Deposit Create Failed"));
				}
				return false;//Did not want to continue creating regular deposit or failed creating QuickBooks Online deposit with no option to continue.
			}
			//Everything below only happens when QuickBooks Online deposit was successful.
			SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,Lan.g(this,"Deposit slip sent to QuickBooks Online.")+"\r\n"
				+Lan.g(this,"Deposit date")+": "+_depositCur.DateDeposit.ToShortDateString()+" "+Lan.g(this,"for")+" "+_depositCur.Amount.ToString("c"));
			Cursor.Current=Cursors.Default;
			if(quickBooksOnlineClassRef!=null && quickBooksOnlineDepositResponse.IsClassRefNull()) {
				//We sent a class ref, the deposit was successful, but the class ref was not applied to the deposit.
				//The user may have not enabled classes.
				MsgBox.Show(this,"Deposit sent successfully, but QuickBooks Online did not use the Class Ref selected. Make sure that Class Tracking is turned on in QuickBooks Online.");
			}
			else {
				MsgBox.Show(this,"Deposit successfully sent to QuickBooks Online.");
			}
			Deposit depositCopy=_depositCur.Copy();
			_depositCur.IsSentToQuickBooksOnline=true;
			if(IsNew==false) {
				Deposits.Update(_depositCur,depositCopy);
			}
			butSendQB.Enabled=false;//Don't let user send same deposit more than once.
			return true;
		}

		///<summary>Save the QuickBooks Online access and refresh tokens if they have changed.</summary>
		private void SaveTokens() {
			bool isChanged=false;
			if(!_quickBooksOnlineToken.AccessToken.IsNullOrEmpty()) {
				isChanged|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropertyQboAccessToken,_quickBooksOnlineToken.AccessToken);
				_programPropertyQboAccessToken.PropertyValue=_quickBooksOnlineToken.AccessToken;
			}
			if(!_quickBooksOnlineToken.RefreshToken.IsNullOrEmpty()) {
				isChanged|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropertyQboRefreshToken,_quickBooksOnlineToken.RefreshToken);
				_programPropertyQboRefreshToken.PropertyValue=_quickBooksOnlineToken.RefreshToken;
			}
			if(isChanged) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
		}

		///<summary>Gets the selected patient and insurance payments. If this is an existing deposit, this gets all payments listed.</summary>
		private List<decimal> GetListSelectedPaymentAmounts() {
			List<decimal> listPaymentAmounts=new List<decimal>();
			if(IsNew) {
				for(int i=0;i<gridPat.SelectedIndices.Length;i++) {
					listPaymentAmounts.Add((decimal)_listPaymentsPat[gridPat.SelectedIndices[i]].PayAmt);
				}
				for(int i=0;i<gridIns.SelectedIndices.Length;i++) {
					listPaymentAmounts.Add((decimal)_listClaimPayments[gridIns.SelectedIndices[i]].CheckAmt);
				}
			}
			else {
				for(int i=0;i<_listPaymentsPat.Count;i++) {
					listPaymentAmounts.Add((decimal)_listPaymentsPat[i].PayAmt);
				}
				for(int i=0;i<_listClaimPayments.Count;i++) {
					listPaymentAmounts.Add((decimal)_listClaimPayments[i].CheckAmt);
				}
			}
			return listPaymentAmounts;
		}

		///<summary>Saves the selected rows to database.</summary>
		private bool SaveToDB(){
			if(!textDate.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(IsNew && gridPat.SelectedIndices.Length==0 && gridIns.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one payment for this deposit first.");
				return false;
			}
			//Prevent backdating----------------------------------------------------------------------------------------
			DateTime dateTime=PIn.Date(textDate.Text);
			//We enforce security here based on date displayed, not date entered
			if(!Security.IsAuthorized(Permissions.DepositSlips,dateTime)) {
				return false;
			}
			_depositCur.DateDeposit=dateTime;
			//amount already handled.
			_depositCur.BankAccountInfo=PIn.String(textBankAccountInfo.Text);
			_depositCur.Memo=PIn.String(textMemo.Text);
			_depositCur.Batch=PIn.String(textBatch.Text);
			if(comboDepositAccountNum.SelectedIndex > -1) {
				_depositCur.DepositAccountNum=comboDepositAccountNum.GetSelectedDefNum();
			}
			if(IsNew){
				if(gridPat.SelectedIndices.Length+gridIns.SelectedIndices.Length>18 && (IsQuickBooks() || IsQuickBooksOnline())) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"No more than 18 items will fit on a QuickBooks deposit slip. Continue anyway?")) {
						return false;
					}
				}
				else if(gridPat.SelectedIndices.Length+gridIns.SelectedIndices.Length>32) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"No more than 32 items will fit on a deposit slip. Continue anyway?")) {
						return false;
					}
				}
			}
			//Check DB to see if payments have been linked to another deposit already.  Build list of currently selected PayNums
			List<long> listSelectedPayNums=gridPat.SelectedIndices.OfType<int>().Select(x => _listPaymentsPat[x].PayNum).ToList();
			if(listSelectedPayNums.Count>0) {
				int countAlreadyAttached=Payments.GetCountAttachedToDeposit(listSelectedPayNums,_depositCur.DepositNum);//Depositnum might be 0
				if(countAlreadyAttached>0) {
					MessageBox.Show(this,countAlreadyAttached+" "+Lan.g(this,"patient payments are already attached to another deposit")+".");
					//refresh
					return false;
				}
			}
			//Check DB to see if payments have been linked to another deposit already.  Build list of currently selected ClaimPaymentNums.
			List<long> listSelectedClaimPaymentNums=gridIns.SelectedIndices.OfType<int>().Select(x => _listClaimPayments[x].ClaimPaymentNum).ToList();
			if(listSelectedClaimPaymentNums.Count>0) {
				int countAlreadyAttached=ClaimPayments.GetCountAttachedToDeposit(listSelectedClaimPaymentNums,_depositCur.DepositNum);//Depositnum might be 0
				if(countAlreadyAttached>0) {
					MessageBox.Show(this,countAlreadyAttached+" "+Lan.g(this,"insurance payments are already attached to another deposit")+".");
					//refresh
					return false;
				}
			}
			if(IsNew && !_hasBeenSavedToDB){
				//Create a deposit in QuickBooks or QuickBooks Online
				bool hasDepositInfo=Accounts.DepositsLinked();
				bool isQbOrQbo=(IsQuickBooks() && !ODBuild.IsWeb()) || IsQuickBooksOnline();
				//Short circuit so that CreateDepositQbOrQbo() is only called if hasDepositInfo and isQbOrQbo are both true.
				if(hasDepositInfo && isQbOrQbo && !CreateDepositQbOrQbo(true)) {
					return false;
				}
				Deposits.Insert(_depositCur);
				_depositOld=_depositCur.Copy();//fresh copy to old so if changes are made they will be saved to db
			}
			else{
				Deposits.Update(_depositCur,_depositOld);
				_depositOld=_depositCur.Copy();//fresh copy to old so if changes are made they will be saved to db
			}
			if(!IsNew){
				_hasBeenSavedToDB=true;//So that we don't insert the deposit slip again when clicking Print or PDF or OK
				return true;
			}
			//IsNew, never allowed to change or attach more checks after initial creation of deposit slip
			for(int i=0;i<gridPat.SelectedIndices.Length;i++){
				Payment paymentSelected=_listPaymentsPat[gridPat.SelectedIndices[i]];
				paymentSelected.DepositNum=_depositCur.DepositNum;
				Payments.Update(paymentSelected,false);//This could be enhanced with a multi row update.
				if(!_isOnOKClick) {//Print/Create PDF
					if(!_listPayNumsAttached.Contains(paymentSelected.PayNum)) {
						_listPayNumsAttached.Add(paymentSelected.PayNum);//Add this payment to list to check when clicking OK.
					}
				}
				else {//OK Click
					_listPayNumsAttached.Remove(paymentSelected.PayNum);//Remove from the list because we don't need to detach.
				}
			}
			for(int i=0;i<gridIns.SelectedIndices.Length;i++){
				ClaimPayment claimPaymentSelected=_listClaimPayments[gridIns.SelectedIndices[i]];
				claimPaymentSelected.DepositNum=_depositCur.DepositNum;
				try {
					ClaimPayments.Update(claimPaymentSelected,IsNew);//This could be enhanced with a multi row update.
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
					return false;
				}
				if(!_isOnOKClick) {//Print/Create PDF
					if(!_listClaimPaymentNumsAttached.Contains(claimPaymentSelected.ClaimPaymentNum)) {
						_listClaimPaymentNumsAttached.Add(claimPaymentSelected.ClaimPaymentNum);//Add this payment to list to check when clicking OK.
					}
				}
				else {//OK Click
					_listClaimPaymentNumsAttached.Remove(claimPaymentSelected.ClaimPaymentNum);//Remove from the list because we don't need to detach.
				}
			}
			if(_isOnOKClick && (_listPayNumsAttached.Count!=0 || _listClaimPaymentNumsAttached.Count!=0)) {
				//Detach any payments or claimpayments that were attached in the DB but no longer selected.
				Deposits.DetachFromDeposit(_depositCur.DepositNum,_listPayNumsAttached,_listClaimPaymentNumsAttached);
			}
			if(!_isOnOKClick) {
				//The user has not saved the deposit. Check for payments that are no longer selected.
				List<long> listPayNumsDeselected=_listPayNumsAttached.Except(listSelectedPayNums).ToList();
				List<long> listClaimPaymentNumsDelected=_listClaimPaymentNumsAttached.Except(listSelectedClaimPaymentNums).ToList();
				if(!listPayNumsDeselected.IsNullOrEmpty() || !listClaimPaymentNumsDelected.IsNullOrEmpty()) {
					Deposits.DetachFromDeposit(_depositCur.DepositNum,listPayNumsDeselected,listClaimPaymentNumsDelected);
					//The deselected payments are no longer attached. Remove from class wide list.
					listPayNumsDeselected.ForEach(x => _listPayNumsAttached.Remove(x));
					listClaimPaymentNumsDelected.ForEach(x => _listClaimPaymentNumsAttached.Remove(x));
				}
			}
			_hasBeenSavedToDB=true;//So that we don't insert the deposit slip again when clicking Print or PDF or OK
			return true;
		}

		private void gridPat_CellClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			ComputeAmt();
		}

		private void gridPat_MouseUp(object sender,MouseEventArgs e) {
			ComputeAmt();
		}

		private void gridIns_CellClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			ComputeAmt();
		}

		private void gridIns_MouseUp(object sender,MouseEventArgs e) {
			ComputeAmt();
		}

		private void textCheckNumSearch_KeyUp(object sender,KeyEventArgs e) {
			Search();
		}

		private void textCheckNumSearch_MouseUp(object sender,MouseEventArgs e) {
			Search();
		}

		private void textAmountSearch_KeyUp(object sender,KeyEventArgs e) {
			Search();
		}

		private void textAmountSearch_MouseUp(object sender,MouseEventArgs e) {
			Search();
		}

		private void butSendQB_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb() && IsQuickBooks()) {
				MsgBox.Show(this,"QuickBooks is not available while viewing through the web.");
				return;
			}
			DateTime dateTime=PIn.Date(textDate.Text);//We use security on the date showing.
			if(!Security.IsAuthorized(Permissions.DepositSlips,dateTime)) {
				return;
			}
			_depositCur.DateDeposit=dateTime;
			if(IsQuickBooksOnline() && _depositCur.IsSentToQuickBooksOnline) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This deposit has already been sent to QuickBooks Online and should only be sent again if it is missing from your account. Continue?")) {
					return;
				}
			}
			CreateDepositQbOrQbo(false);
		}

		///<summary>Return true if it's a new deposit and saved successfully to DB, or user has permission to edit existing deposit and saved successfully to DB.
		///Returns false for any other reason.</summary>
		private bool ValidateDatePermission() {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(IsNew){
				if(!SaveToDB()) {
					return false;
				}
				return true;
			}
			//Not new.
			//Only allowed to change date and bank account info, NOT attached checks.
			//We enforce security here based on date displayed, not date entered.
			//If user is trying to change date without permission:
			DateTime dateTime=PIn.Date(textDate.Text);
			if(Security.IsAuthorized(Permissions.DepositSlips,dateTime,true)){
				if(!SaveToDB()) {
					return false;
				}
			}
			//if security.NotAuthorized, then it simply skips the save process before printing
			return true;
		}

		///<summary>Create and fill a Sheet for PDF-related button clicks and leaves the rest to the calling method to figure out what it wants to do with the Sheet.</summary>
		private Sheet CreateAndFillSheet() {
			SheetDef sheetDef=null;
			List<SheetDef> listSheetDefsDeposit=SheetDefs.GetCustomForType(SheetTypeEnum.DepositSlip);
			if(listSheetDefsDeposit.Count>0){
				sheetDef=listSheetDefsDeposit[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			else{
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.DepositSlip);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,0);//Does not insert.
			SheetParameter.SetParameter(sheet,"DepositNum",_depositCur.DepositNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			//This is all that is identically done in the button clicks, so we return here and let the calling method do the rest of the work.
			return sheet;
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(!ValidateDatePermission()) {
				return;
			}
			Sheet sheet=CreateAndFillSheet();
			SheetPrinting.Print(sheet);
		}
		
		private void butPDF_Click(object sender,EventArgs e) {
			if(!ValidateDatePermission()) {
				return;
			}
			Sheet sheet=CreateAndFillSheet();
			string filePathAndName=PrefC.GetRandomTempFile(".pdf");
			SheetPrinting.CreatePdf(sheet,filePathAndName,null);
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.HandleFile(filePathAndName);
			}
			else {
				Process.Start(filePathAndName);
			}
		}

		private void butEmailPDF_Click(object sender,EventArgs e) {
			if(!ValidateDatePermission()) {
				return;
			}
			Sheet sheet=CreateAndFillSheet();
			string sheetName=sheet.Description+"_"+DateTime.Now.ToString("yyyyMMdd_hhmmssfff")+".pdf";
			string tempFile=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),sheetName);
			string filePathAndName=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),sheetName);
			SheetPrinting.CreatePdf(sheet,tempFile,null);
			FileAtoZ.Copy(tempFile,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ);
			EmailMessage emailMessage=new EmailMessage();
			EmailAddress emailAddress=EmailAddresses.GetByClinic(Clinics.ClinicNum);
			emailMessage.FromAddress=emailAddress.GetFrom();
			emailMessage.Subject=sheet.Description;
			emailMessage.MsgType=EmailMessageSource.Sheet;
			EmailAttach emailAttach=new EmailAttach();
			emailAttach.ActualFileName=sheetName;
			emailAttach.DisplayedFileName=sheetName;
			emailMessage.Attachments.Add(emailAttach);
			using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,emailAddress);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.ShowDialog();
		}

		///<summary>Remember that this can only happen if IsNew</summary>
		private void butRefresh_Click(object sender, System.EventArgs e) {
			if(!textDateStart.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listInsPayType.SelectedIndices.Count==0 && listPayType.SelectedIndices.Count==0) {
				for(int i=0;i<listInsPayType.Items.Count;i++) {
					listInsPayType.SetSelected(i,true);
				}
				for(int j=0;j<listPayType.Items.Count;j++) {
					listPayType.SetSelected(j,true);
				}
			}
			FillGrids();
			gridPat.SetAll(true);
			gridIns.SetAll(true);
			ComputeAmt();
			if(!PrefC.HasClinicsEnabled || comboClinic.IsAllSelected){
				textBankAccountInfo.Text=PrefC.GetString(PrefName.PracticeBankNumber);
			}
			else{
				textBankAccountInfo.Text=Clinics.GetClinic(comboClinic.SelectedClinicNum).BankNumber;
			}
			_isChanged |= Prefs.UpdateString(PrefName.DateDepositsStarted,POut.Date(PIn.Date(textDateStart.Text),false));
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			//If deposit is attached to a transaction which is more than 48 hours old, then not allowed to delete.
			//This is hard coded.  User would have to delete or detach from within transaction rather than here.
			Transaction transaction=Transactions.GetAttachedToDeposit(_depositCur.DepositNum);
			if(transaction!=null){
				if(transaction.DateTimeEntry < MiscData.GetNowDateTime().AddDays(-2) ){
					MsgBox.Show(this,"Not allowed to delete.  This deposit is already attached to an accounting transaction.  You will need to detach it from within the accounting section of the program.");
					return;
				}
				if(Transactions.IsReconciled(transaction)) {
					MsgBox.Show(this,"Not allowed to delete.  This deposit is attached to an accounting transaction that has been reconciled.  You will need to detach it from within the accounting section of the program.");
					return;
				}
				try{
					Transactions.Delete(transaction);
				}
				catch(ApplicationException ex){
					MessageBox.Show(ex.Message);
					return;
				}
			}
			try {
				Deposits.Delete(_depositCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);//Already translated.
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			_isOnOKClick=true;
			if(!SaveToDB()){
				_isOnOKClick=false;
				return;
			}
			DialogResult=DialogResult.OK;
			//Existing deposit, no need to create a new transaction, deposit and income entry.
			if(!IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,_depositCur.DateDeposit.ToShortDateString()+" "+_depositCur.Amount.ToString("c"));
				return;
			}
			//New deposit, but has faulty deposit criteria or is Quickbooks/Quickbooks Online, can't make transaction, deposit and income entry.
			bool meetsDepositCriteria=Accounts.DepositsLinked() && _depositCur.Amount>0;
			bool isQbOrQbo=IsQuickBooks() || IsQuickBooksOnline();
			if(!meetsDepositCriteria || isQbOrQbo) {
				return;
			}
			//New deposit not through Quickbooks/Quickbooks Online.
			//create a transaction here
			Transaction transaction=new Transaction();
			transaction.DepositNum=_depositCur.DepositNum;
			transaction.UserNum=Security.CurUser.UserNum;
			Transactions.Insert(transaction);
			//first the deposit entry
			JournalEntry journalEntry=new JournalEntry();
			journalEntry.AccountNum=_listDepositAccounts[comboDepositAccount.SelectedIndex];
			journalEntry.CheckNumber=Lan.g(this,"DEP");
			journalEntry.DateDisplayed=_depositCur.DateDeposit;//it would be nice to add security here.
			journalEntry.DebitAmt=_depositCur.Amount;
			journalEntry.Memo=Lan.g(this,"Deposit");
			journalEntry.Splits=Accounts.GetDescript(PrefC.GetLong(PrefName.AccountingIncomeAccount));
			journalEntry.TransactionNum=transaction.TransactionNum;
			JournalEntries.Insert(journalEntry);
			//then, the income entry
			journalEntry=new JournalEntry();
			journalEntry.AccountNum=PrefC.GetLong(PrefName.AccountingIncomeAccount);
			//journalEntry.CheckNumber=;
			journalEntry.DateDisplayed=_depositCur.DateDeposit;//it would be nice to add security here.
			journalEntry.CreditAmt=_depositCur.Amount;
			journalEntry.Memo=Lan.g(this,"Deposit");
			journalEntry.Splits=Accounts.GetDescript(_listDepositAccounts[comboDepositAccount.SelectedIndex]);
			journalEntry.TransactionNum=transaction.TransactionNum;
			JournalEntries.Insert(journalEntry);
			SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,_depositCur.DateDeposit.ToShortDateString()+" New "+_depositCur.Amount.ToString("c"));
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			//Deletion and detaching payments is done on Closing.
			DialogResult=DialogResult.Cancel;
		}

		private void FormDepositEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(IsNew && DialogResult==DialogResult.Cancel) {
				//User might have printed this, causing an insert into the DB.
				Deposits.Delete(_depositCur);//This will handle unattaching payments from this deposit. A Transaction should not have been made yet.
			}
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
	}
}





















