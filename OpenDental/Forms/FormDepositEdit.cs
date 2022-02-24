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
		private ClaimPayment[] ClaimPayList;
		private List<Payment> PatPayList;
		private bool changed;
		///<summary>Only used if linking to accounts</summary>
		private long[] DepositAccounts;
		///<summary>The accounting software selected in Manage Module preferences.</summary>
		private AccountingSoftware _accountingSoftware;
		///<summary>OAuth access token stored in DB for QuickBooks Online API.</summary>
		private ProgramProperty _programPropQboAccessToken;
		///<summary>OAuth refresh token stored in DB for QuickBooks Online API.</summary>
		private ProgramProperty _programPropQboRefreshToken;
		///<summary>Current OAuth access tokens for QuickBooks Online API.</summary>
		private QuickBooksOnlineToken _qboTokens;
		///<summary>Class Refs for QuickBooks Online.</summary>
		private ProgramProperty _programPropQboClassRefs;
		///<summary>Realm ID for QuickBooks Online.</summary>
		private ProgramProperty _programPropQboRealmId;
		///<summary>Used to store DefNums in a 1:1 ratio for listInsPayType</summary>
		private List<long> _insPayDefNums;
		///<summary>Used to store DefNums in a 1:1 ratio for listPayType</summary>
		private List<long> _payTypeDefNums;
		///<summary>Keeps track of whether the payment has been saved to the database since the form was opened.</summary>
		private bool _hasBeenSavedToDB;
		///<summary>A list of payNums already attached to the deposit.  When printing or showing PDF these were attached to the deposit.
		///Used on OK click to make sure we detach any procedures that might have been unselected after they've been attached in the DB.</summary>
		private List<long> _listPayNumsAttached=new List<long>();
		///<summary>A list of claimPaymentNum already attached to the deposit.  When printing or showing PDF these were attached to the deposit.
		///Used on OK click to make sure we detach any procedures that might have been unselected after they've been attached in the DB.</summary>
		private List<long> _listClaimPaymentNumAttached=new List<long>();
		///<summary>Used in UpdateToDB to detach any payments that were attached to deposit but have been deselected before clicking OK.</summary>
		private bool _isOnOKClick=false;

		///<summary>True if the accounting software pref is set to QuickBooks.</summary>
		private bool _isQuickBooks {
			get {
				return _accountingSoftware==AccountingSoftware.QuickBooks;
			}
		}

		///<summary>True if the accounting software pref is set to QuickBooks Online.</summary>
		private bool _isQuickBooksOnline {
			get {
				return _accountingSoftware==AccountingSoftware.QuickBooksOnline;
			}
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
			if(_isQuickBooks) {
				butSendQB.Text="&Send QB";
			}
			else if(_isQuickBooksOnline) {
				butSendQB.Text="&Send QBO";
				Program progQbo=Programs.GetCur(ProgramName.QuickBooksOnline);
				_programPropQboAccessToken=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Access Token");
				_programPropQboRefreshToken=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Refresh Token");
				_qboTokens=new QuickBooksOnlineToken(_programPropQboAccessToken.PropertyValue,_programPropQboRefreshToken.PropertyValue);
				_programPropQboClassRefs=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Class Refs");
				_programPropQboRealmId=ProgramProperties.GetPropForProgByDesc(progQbo.ProgramNum,"Realm ID");
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
				List<Def> listAutoDepositDefsAll=Defs.GetDefsForCategory(DefCat.AutoDeposit);
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
				
				List<Def> listPaymentTypeDefs=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
				List<Def> listInsurancePaymentTypeDefs=Defs.GetDefsForCategory(DefCat.InsurancePaymentType,true);
				_payTypeDefNums=new List<long>();
				for(int i=0;i<listPaymentTypeDefs.Count;i++) {
					if(listPaymentTypeDefs[i].ItemValue!="") {
						continue;//skip defs not selected for deposit slip
					}
					listPayType.Items.Add(listPaymentTypeDefs[i].ItemName);
					_payTypeDefNums.Add(listPaymentTypeDefs[i].DefNum);
					listPayType.SetSelected(listPayType.Items.Count-1,true);
				}
				_insPayDefNums=new List<long>();
				for(int i=0;i<listInsurancePaymentTypeDefs.Count;i++) {
					if(listInsurancePaymentTypeDefs[i].ItemValue!="") {
						continue;//skip defs not selected for deposit slip
					}
					listInsPayType.Items.Add(listInsurancePaymentTypeDefs[i].ItemName);
					_insPayDefNums.Add(listInsurancePaymentTypeDefs[i].DefNum);
					listInsPayType.SetSelected(listInsPayType.Items.Count-1,true);
				}
				textDepositAccount.Visible=false;//this is never visible for new. It's a description if already attached.
				if(Accounts.DepositsLinked() && !_isQuickBooks && !_isQuickBooksOnline) {
					DepositAccounts=Accounts.GetDepositAccounts();
					for(int i=0;i<DepositAccounts.Length;i++) {
						comboDepositAccount.Items.Add(Accounts.GetDescript(DepositAccounts[i]));
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
				Transaction trans=Transactions.GetAttachedToDeposit(_depositCur.DepositNum);
				if(trans==null) {
					labelDepositAccount.Visible=false;
					comboDepositAccount.Visible=false;
					textDepositAccount.Visible=false;
				}
				else {
					comboDepositAccount.Enabled=false;
					labelDepositAccount.Text=Lan.g(this,"Deposited into Account");
					List<JournalEntry> jeL=JournalEntries.GetForTrans(trans.TransactionNum);
					for(int i=0;i<jeL.Count;i++) {
						if(Accounts.GetAccount(jeL[i].AccountNum).AcctType==AccountType.Asset) {
							comboDepositAccount.Items.Add(Accounts.GetDescript(jeL[i].AccountNum));
							comboDepositAccount.SelectedIndex=0;
							textDepositAccount.Text=jeL[i].DateDisplayed.ToShortDateString()
								+" "+jeL[i].DebitAmt.ToString("c");
							break;
						}
					}
				}
			}
			//If in QuickBooks or QuickBooks Online mode, hide dropdown because its handled in FormQBAccountSelect.cs.
			if(_isQuickBooks || _isQuickBooksOnline) {
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
			if((_isQuickBooks && PrefC.GetBool(PrefName.QuickBooksClassRefsEnabled)) || _isQuickBooksOnline && _programPropQboClassRefs.PropertyValue!="") {
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
				if(_isQuickBooks) {
					listClassNames=PrefC.GetString(PrefName.QuickBooksClassRefs).Split(',').ToList();
				}
				else if(_isQuickBooksOnline){
					listClassNames=ProgramProperties.GetQuickBooksOnlineEntityNames(_programPropQboClassRefs.PropertyValue);
				}
				for(int i=0;i<listClassNames.Count;i++) {
					if(listClassNames[i]=="") {
						continue;
					}
					comboClassRefs.Items.Add(listClassNames[i]);
				}
			}
		}

		///<summary></summary>
		private void FillGrids(){
			if(IsNew){
				DateTime dateStart=PIn.Date(textDateStart.Text);
				long clinicNum=0;
				if(!comboClinic.IsAllSelected){
					clinicNum=comboClinic.SelectedClinicNum;
				}
				List<long> payTypes=new List<long>();//[listPayType.SelectedIndices.Count];
				for(int i=0;i<listPayType.SelectedIndices.Count;i++) {
					payTypes.Add(_payTypeDefNums[listPayType.SelectedIndices[i]]);
				}
				List<long> insPayTypes=new List<long>();
				for(int i=0;i<listInsPayType.SelectedIndices.Count;i++) {
					insPayTypes.Add(_insPayDefNums[listInsPayType.SelectedIndices[i]]);
				}
				PatPayList=new List<Payment>();
				if(payTypes.Count!=0) {
					PatPayList=Payments.GetForDeposit(dateStart,clinicNum,payTypes);
				}
				ClaimPayList=new ClaimPayment[0];
				if(insPayTypes.Count!=0) {
					ClaimPayList=ClaimPayments.GetForDeposit(dateStart,clinicNum,insPayTypes);
				}
				//new deposit, but has been saved to db (pressed print/PDF/email buttons), get trans already attached to deposit in db as well as unattached
				if(_hasBeenSavedToDB && _depositCur.DepositNum>0) {
					PatPayList=PatPayList.Concat(Payments.GetForDeposit(_depositCur.DepositNum))
						.OrderBy(x => x.PayDate).ThenBy(x => x.PayNum).ToList();
					ClaimPayList=ClaimPayList.Concat(ClaimPayments.GetForDeposit(_depositCur.DepositNum))
						.OrderBy(x => x.CheckDate).ThenBy(x => x.ClaimPaymentNum).ToArray();
				}
			}
			else{
				PatPayList=Payments.GetForDeposit(_depositCur.DepositNum);
				ClaimPayList=ClaimPayments.GetForDeposit(_depositCur.DepositNum);
			}
			//Fill Patient Payment Grid---------------------------------------
			List<long> patNums=new List<long>();
			for(int i=0;i<PatPayList.Count;i++){
				patNums.Add(PatPayList[i].PatNum);
			}
			Patient[] pats=Patients.GetMultPats(patNums);
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
			for(int i=0;i<PatPayList.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(PatPayList[i].PayDate.ToShortDateString());
				row.Cells.Add(Patients.GetOnePat(pats,PatPayList[i].PatNum).GetNameLF());
				row.Cells.Add(Defs.GetName(DefCat.PaymentTypes,PatPayList[i].PayType));
				row.Cells.Add(PatPayList[i].CheckNum);
				row.Cells.Add(PatPayList[i].BankBranch);
				row.Cells.Add(PatPayList[i].PayAmt.ToString("F"));
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
			for(int i=0;i<ClaimPayList.Length;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(ClaimPayList[i].CheckDate.ToShortDateString());
				row.Cells.Add(ClaimPayList[i].CarrierName);
				row.Cells.Add(Defs.GetName(DefCat.InsurancePaymentType,ClaimPayList[i].PayType));
				row.Cells.Add(ClaimPayList[i].CheckNum);
				row.Cells.Add(ClaimPayList[i].BankBranch);
				row.Cells.Add(ClaimPayList[i].CheckAmt.ToString("F"));
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
				amount+=(decimal)PatPayList[gridPat.SelectedIndices[i]].PayAmt;
			}
			for(int i=0;i<gridIns.SelectedIndices.Length;i++){
				amount+=(decimal)ClaimPayList[gridIns.SelectedIndices[i]].CheckAmt;
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
			if(_isQuickBooks) {
				return CreateDepositQB(allowContinue);
			}
			else if(_isQuickBooksOnline) {
				return CreateDepositQBO(allowContinue);
			}
			return false;
		}

		///<summary>Returns true if a deposit was created OR if the user clicked continue anyway on pop up.</summary>
		private bool CreateDepositQB(bool allowContinue) {
			try {
				using FormQBAccountSelect formQBAS=new FormQBAccountSelect(false);
				formQBAS.ShowDialog();
				if(formQBAS.DialogResult!=DialogResult.OK) {
					throw new ApplicationException(Lans.g(this,"Deposit accounts not selected")+".");
				}
				Cursor.Current=Cursors.WaitCursor;
				string classRef="";
				if(PrefC.GetBool(PrefName.QuickBooksClassRefsEnabled)) {
					classRef=comboClassRefs.SelectedItem.ToString();
				}
				if(CompareDouble.IsLessThanOrEqualToZero(_depositCur.Amount)) {
					throw new ODException(Lan.g(this,"Only deposits greater than zero can be sent to QuickBooks."));
				}
				QuickBooks.CreateDeposit(_depositCur.DateDeposit
					,formQBAS.DepositAccountSelected
					,formQBAS.IncomeAccountSelected
					,_depositCur.Amount
					,textMemo.Text
					,classRef);//if classRef=="" then it will be safely ignored here
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,Lan.g(this,"Deposit slip sent to QuickBooks.")+"\r\n"
					+Lan.g(this,"Deposit date")+": "+_depositCur.DateDeposit.ToShortDateString()+" "+Lan.g(this,"for")+" "+_depositCur.Amount.ToString("c"));
				Cursor.Current=Cursors.Default;
				MsgBox.Show(this,"Deposit successfully sent to QuickBooks.");
				butSendQB.Enabled=false;//Don't let user send same deposit more than once.  
			}
			catch(Exception ex) {
				Cursor.Current=Cursors.Default;
				if(allowContinue) {
					if(MessageBox.Show(ex.Message+"\r\n\r\n"
						+Lan.g(this,"The deposit has not been created in QuickBooks. Would you like to create the deposit locally anyway?")
						,Lan.g(this,"QuickBooks Deposit Create Failed")
						,MessageBoxButtons.YesNo)!=DialogResult.Yes)
					{
						return false;
					}
				}
				else {
					MessageBox.Show(ex.Message,Lan.g(this,"QuickBooks Deposit Create Failed"));
					return false;
				}
			}
			return true;
		}

		private bool CreateDepositQBO(bool allowContinue) {
			try {
				if(_qboTokens.AccessToken.IsNullOrEmpty()) {
					MsgBox.Show(this,"You don't have a QuickBooks Online access token. Please go to the QuickBooks Online Setup window, "
						+"and click the Authenticate button to obtain one.");
					return false;
				}
				using FormQBAccountSelect formQBAS=new FormQBAccountSelect(true);
				formQBAS.ShowDialog();
				if(formQBAS.DialogResult!=DialogResult.OK) {
					if(!allowContinue) {//Close/Cancel send QBO
						return false;
					}
					throw new ApplicationException(Lans.g(this,"Deposit accounts not selected")+".");
				}
				Cursor.Current=Cursors.WaitCursor;
				QuickBooksOnlineClassRef classRef=null;
				if(_programPropQboClassRefs.PropertyValue!="" && comboClassRefs.SelectedItem!=null) {
					string classRefName=comboClassRefs.SelectedItem.ToString();
					string classRefId=ProgramProperties.GetQuickBooksOnlineEntityIdByName(_programPropQboClassRefs.PropertyValue,classRefName);
					classRef=new QuickBooksOnlineClassRef(classRefName,classRefId);
				}
				QuickBooksOnlineAccount depositAccount=new QuickBooksOnlineAccount(formQBAS.DepositAccountSelected,formQBAS.DepositAccountId);
				QuickBooksOnlineAccount incomeAccount=new QuickBooksOnlineAccount(formQBAS.IncomeAccountSelected,formQBAS.IncomeAccountId);
				List<decimal> selectedPaymentAmounts=GetListSelectedPaymentAmounts();
				if(CompareDecimal.IsLessThanOrEqualToZero(selectedPaymentAmounts.Sum())) {
					throw new ODException(Lan.g(this,"Only deposits greater than zero can be sent to QuickBooks Online."));
				}
				QuickBooksOnlineDepositResponse depositResponse=QuickBooksOnline.CreateDeposit(_programPropQboRealmId.PropertyValue,_qboTokens
					,depositAccount,incomeAccount,classRef,selectedPaymentAmounts,textMemo.Text,_depositCur.DateDeposit);
				if(depositResponse.RequestException!=null) {
					if(QuickBooksOnline.RefreshTokensIfNeeded(depositResponse.RequestException,_qboTokens)) {
						SaveTokens();
						//Try again after token refresh
						depositResponse=QuickBooksOnline.CreateDeposit(_programPropQboRealmId.PropertyValue,_qboTokens
							,depositAccount,incomeAccount,classRef,selectedPaymentAmounts,textMemo.Text,_depositCur.DateDeposit);
					}
					//Check again as a different error may have occured.
					if(depositResponse.RequestException!=null) {
						throw depositResponse.RequestException;
					}
				}
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,Lan.g(this,"Deposit slip sent to QuickBooks Online.")+"\r\n"
					+Lan.g(this,"Deposit date")+": "+_depositCur.DateDeposit.ToShortDateString()+" "+Lan.g(this,"for")+" "+_depositCur.Amount.ToString("c"));
				Cursor.Current=Cursors.Default;
				if(classRef!=null && depositResponse.IsClassRefNull()) {
					//We sent a class ref, the deposit was successful, but the class ref was not applied to the deposit.
					//The user may have not enabled classes.
					MsgBox.Show(this,"Deposit sent successfully, but QuickBooks Online did not use the Class Ref selected. Make sure that Class Tracking is "
						+"turned on in QuickBooks Online.");
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
			}
			catch(Exception ex) {
				Cursor.Current=Cursors.Default;
				if(allowContinue) {
					if(MessageBox.Show(ex.Message+"\r\n\r\n"
						+Lan.g(this,"The deposit has not been created in QuickBooks Online. Would you like to create the deposit locally anyway?")
						,Lan.g(this,"QuickBooks Online Deposit Create Failed")
						,MessageBoxButtons.YesNo)!=DialogResult.Yes) {
						return false;
					}
				}
				else {
					MessageBox.Show(ex.Message,Lan.g(this,"QuickBooks Online Deposit Create Failed"));
					return false;
				}
			}
			return true;
		}

		///<summary>Save the QuickBooks Online access and refresh tokens if they have changed.</summary>
		private void SaveTokens() {
			bool changed=false;
			if(!_qboTokens.AccessToken.IsNullOrEmpty()) {
				changed|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropQboAccessToken,_qboTokens.AccessToken);
				_programPropQboAccessToken.PropertyValue=_qboTokens.AccessToken;
			}
			if(!_qboTokens.RefreshToken.IsNullOrEmpty()) {
				changed|=ProgramProperties.UpdateProgramPropertyWithValue(_programPropQboRefreshToken,_qboTokens.RefreshToken);
				_programPropQboRefreshToken.PropertyValue=_qboTokens.RefreshToken;
			}
			if(changed) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
		}

		///<summary>Gets the selected patient and insurance payments. If this is an existing deposit, this gets all payments listed.</summary>
		private List<decimal> GetListSelectedPaymentAmounts() {
			List<decimal> listPaymentAmounts=new List<decimal>();
			if(IsNew) {
				for(int i=0;i<gridPat.SelectedIndices.Length;i++) {
					listPaymentAmounts.Add(((decimal)PatPayList[gridPat.SelectedIndices[i]].PayAmt));
				}
				for(int i=0;i<gridIns.SelectedIndices.Length;i++) {
					listPaymentAmounts.Add(((decimal)ClaimPayList[gridIns.SelectedIndices[i]].CheckAmt));
				}
			}
			else {
				for(int i=0;i<PatPayList.Count;i++) {
					listPaymentAmounts.Add(((decimal)PatPayList[i].PayAmt));
				}
				for(int i=0;i<ClaimPayList.Length;i++) {
					listPaymentAmounts.Add(((decimal)ClaimPayList[i].CheckAmt));
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
			DateTime date=PIn.Date(textDate.Text);
			//We enforce security here based on date displayed, not date entered
			if(!Security.IsAuthorized(Permissions.DepositSlips,date)) {
				return false;
			}
			_depositCur.DateDeposit=date;
			//amount already handled.
			_depositCur.BankAccountInfo=PIn.String(textBankAccountInfo.Text);
			_depositCur.Memo=PIn.String(textMemo.Text);
			_depositCur.Batch=PIn.String(textBatch.Text);
			if(comboDepositAccountNum.SelectedIndex > -1) {
				_depositCur.DepositAccountNum=comboDepositAccountNum.GetSelectedDefNum();
			}
			if(IsNew){
				if(gridPat.SelectedIndices.Length+gridIns.SelectedIndices.Length>18 && (_isQuickBooks || _isQuickBooksOnline)) {
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
			List<long> listSelectedPayNums=gridPat.SelectedIndices.OfType<int>().Select(x => PatPayList[x].PayNum).ToList();
			if(listSelectedPayNums.Count>0) {
				int alreadyAttached=Payments.GetCountAttachedToDeposit(listSelectedPayNums,_depositCur.DepositNum);//Depositnum might be 0
				if(alreadyAttached>0) {
					MessageBox.Show(this,alreadyAttached+" "+Lan.g(this,"patient payments are already attached to another deposit")+".");
					//refresh
					return false;
				}
			}
			//Check DB to see if payments have been linked to another deposit already.  Build list of currently selected ClaimPaymentNums.
			List<long> listSelectedClaimPaymentNums=gridIns.SelectedIndices.OfType<int>().Select(x => ClaimPayList[x].ClaimPaymentNum).ToList();
			if(listSelectedClaimPaymentNums.Count>0) {
				int alreadyAttached=ClaimPayments.GetCountAttachedToDeposit(listSelectedClaimPaymentNums,_depositCur.DepositNum);//Depositnum might be 0
				if(alreadyAttached>0) {
					MessageBox.Show(this,alreadyAttached+" "+Lan.g(this,"insurance payments are already attached to another deposit")+".");
					//refresh
					return false;
				}
			}
			if(IsNew && !_hasBeenSavedToDB){
				//Create a deposit in QuickBooks or QuickBooks Online
				if(Accounts.DepositsLinked()
					&& ((_isQuickBooks && !ODBuild.IsWeb()) || _isQuickBooksOnline) && !CreateDepositQbOrQbo(true))
				{
					return false;
				}
				Deposits.Insert(_depositCur);
				_depositOld=_depositCur.Copy();//fresh copy to old so if changes are made they will be saved to db
			}
			else{
				Deposits.Update(_depositCur,_depositOld);
				_depositOld=_depositCur.Copy();//fresh copy to old so if changes are made they will be saved to db
			}
			if(IsNew){//never allowed to change or attach more checks after initial creation of deposit slip
				for(int i=0;i<gridPat.SelectedIndices.Length;i++){
					Payment selectedPayment=PatPayList[gridPat.SelectedIndices[i]];
					selectedPayment.DepositNum=_depositCur.DepositNum;
					Payments.Update(selectedPayment,false);//This could be enhanced with a multi row update.
					if(!_isOnOKClick) {//Print/Create PDF
						if(!_listPayNumsAttached.Contains(selectedPayment.PayNum)) {
							_listPayNumsAttached.Add(selectedPayment.PayNum);//Add this payment to list to check when clicking OK.
						}
					}
					else {//OK Click
						_listPayNumsAttached.Remove(selectedPayment.PayNum);//Remove from the list because we don't need to detach.
					}
				}
				for(int i=0;i<gridIns.SelectedIndices.Length;i++){
					ClaimPayment selectedClaimPayment=ClaimPayList[gridIns.SelectedIndices[i]];
					selectedClaimPayment.DepositNum=_depositCur.DepositNum;
					try {
						ClaimPayments.Update(selectedClaimPayment,IsNew);//This could be enhanced with a multi row update.
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return false;
					}
					if(!_isOnOKClick) {//Print/Create PDF
						if(!_listClaimPaymentNumAttached.Contains(selectedClaimPayment.ClaimPaymentNum)) {
							_listClaimPaymentNumAttached.Add(selectedClaimPayment.ClaimPaymentNum);//Add this payment to list to check when clicking OK.
						}
					}
					else {//OK Click
						_listClaimPaymentNumAttached.Remove(selectedClaimPayment.ClaimPaymentNum);//Remove from the list because we don't need to detach.
					}
				}
				if(_isOnOKClick && (_listPayNumsAttached.Count!=0 || _listClaimPaymentNumAttached.Count!=0)) {
					//Detach any payments or claimpayments that were attached in the DB but no longer selected.
					Deposits.DetachFromDeposit(_depositCur.DepositNum,_listPayNumsAttached,_listClaimPaymentNumAttached);
				}
				if(!_isOnOKClick) {
					//The user has not saved the deposit. Check for payments that are no longer selected.
					List<long> listDeselectedPayNums=_listPayNumsAttached.Except(listSelectedPayNums).ToList();
					List<long> listDelectedClaimPaymentNums=_listClaimPaymentNumAttached.Except(listSelectedClaimPaymentNums).ToList();
					if(!listDeselectedPayNums.IsNullOrEmpty() || !listDelectedClaimPaymentNums.IsNullOrEmpty()) {
						Deposits.DetachFromDeposit(_depositCur.DepositNum,listDeselectedPayNums,listDelectedClaimPaymentNums);
						//The deselected payments are no longer attached. Remove from class wide list.
						listDeselectedPayNums.ForEach(x => _listPayNumsAttached.Remove(x));
						listDelectedClaimPaymentNums.ForEach(x => _listClaimPaymentNumAttached.Remove(x));
					}
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
			if(ODBuild.IsWeb() && _isQuickBooks) {
				MsgBox.Show(this,"QuickBooks is not available while viewing through the web.");
				return;
			}
			DateTime date=PIn.Date(textDate.Text);//We use security on the date showing.
			if(!Security.IsAuthorized(Permissions.DepositSlips,date)) {
				return;
			}
			_depositCur.DateDeposit=date;
			if(_isQuickBooksOnline && _depositCur.IsSentToQuickBooksOnline) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This deposit has already been sent to QuickBooks Online and should only be sent again if it is missing from your account. Continue?")) {
					return;
				}
			}
			CreateDepositQbOrQbo(false);
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(IsNew){
				if(!SaveToDB()) {
					return;
				}
			}
			else{//not new
				//Only allowed to change date and bank account info, NOT attached checks.
				//We enforce security here based on date displayed, not date entered.
				//If user is trying to change date without permission:
				DateTime date=PIn.Date(textDate.Text);
				if(Security.IsAuthorized(Permissions.DepositSlips,date,true)){
					if(!SaveToDB()) {
						return;
					}
				}
				//if security.NotAuthorized, then it simply skips the save process before printing
			}
			SheetDef sheetDef=null;
			List <SheetDef> depositSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.DepositSlip);
			if(depositSheetDefs.Count>0){
				sheetDef=depositSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			else{
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.DepositSlip);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,0);
			SheetParameter.SetParameter(sheet,"DepositNum",_depositCur.DepositNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			SheetPrinting.Print(sheet);
		}
		
		private void butPDF_Click(object sender,EventArgs e) {
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(IsNew){
				if(!SaveToDB()) {
					return;
				}
			}
			else{//not new
				//Only allowed to change date and bank account info, NOT attached checks.
				//We enforce security here based on date displayed, not date entered.
				//If user is trying to change date without permission:
				DateTime date=PIn.Date(textDate.Text);
				if(Security.IsAuthorized(Permissions.DepositSlips,date,true)){
					if(!SaveToDB()) {
						return;
					}
				}
				//if security.NotAuthorized, then it simply skips the save process before printing
			}
			SheetDef sheetDef=null;
			List <SheetDef> depositSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.DepositSlip);
			if(depositSheetDefs.Count>0){
				sheetDef=depositSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			else{
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.DepositSlip);
			}
			//The below mimics FormSheetFillEdit.butPDF_Click() and the above butPrint_Click().
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,0);//Does not insert.
			SheetParameter.SetParameter(sheet,"DepositNum",_depositCur.DepositNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
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
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(IsNew) {
				if(!SaveToDB()) {
					return;
				}
			}
			else {//not new
						//Only allowed to change date and bank account info, NOT attached checks.
						//We enforce security here based on date displayed, not date entered.
						//If user is trying to change date without permission:
				DateTime date = PIn.Date(textDate.Text);
				if(Security.IsAuthorized(Permissions.DepositSlips,date,true)) {
					if(!SaveToDB()) {
						return;
					}
				}
				//if security.NotAuthorized, then it simply skips the save process before printing
			}
			SheetDef sheetDef=null;
			List<SheetDef> listDepositSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.DepositSlip);
			if(listDepositSheetDefs.Count>0) {
				sheetDef=listDepositSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			else {
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.DepositSlip);
			}
			//The below mimics FormSheetFillEdit.butPDF_Click() and the above butPrint_Click().
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,0);//Does not insert.
			SheetParameter.SetParameter(sheet,"DepositNum",_depositCur.DepositNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			string sheetName=sheet.Description+"_"+DateTime.Now.ToString("yyyyMMdd_hhmmssfff")+".pdf";
			string tempFile=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),sheetName);
			string filePathAndName=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),sheetName);
			SheetPrinting.CreatePdf(sheet,tempFile,null);
			FileAtoZ.Copy(tempFile,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ);
			EmailMessage message=new EmailMessage();
			EmailAddress address=EmailAddresses.GetByClinic(Clinics.ClinicNum);
			message.FromAddress=address.GetFrom();
			message.Subject=sheet.Description;
			message.MsgType=EmailMessageSource.Sheet;
			EmailAttach attach=new EmailAttach();
			attach.ActualFileName=sheetName;
			attach.DisplayedFileName=sheetName;
			message.Attachments.Add(attach);
			using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message,address);
			FormE.IsNew=true;
			FormE.ShowDialog();
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
			if(Prefs.UpdateString(PrefName.DateDepositsStarted,POut.Date(PIn.Date(textDateStart.Text),false))){
				changed=true;
			}
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
			Transaction trans=Transactions.GetAttachedToDeposit(_depositCur.DepositNum);
			if(trans != null){
				if(trans.DateTimeEntry < MiscData.GetNowDateTime().AddDays(-2) ){
					MsgBox.Show(this,"Not allowed to delete.  This deposit is already attached to an accounting transaction.  You will need to detach it from within the accounting section of the program.");
					return;
				}
				if(Transactions.IsReconciled(trans)) {
					MsgBox.Show(this,"Not allowed to delete.  This deposit is attached to an accounting transaction that has been reconciled.  You will need to detach it from within the accounting section of the program.");
					return;
				}
				try{
					Transactions.Delete(trans);
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
			if(IsNew) {
				if(Accounts.DepositsLinked() && _depositCur.Amount>0 && !_isQuickBooks && !_isQuickBooksOnline) {
					//create a transaction here
					Transaction trans=new Transaction();
					trans.DepositNum=_depositCur.DepositNum;
					trans.UserNum=Security.CurUser.UserNum;
					Transactions.Insert(trans);
					//first the deposit entry
					JournalEntry je=new JournalEntry();
					je.AccountNum=DepositAccounts[comboDepositAccount.SelectedIndex];
					je.CheckNumber=Lan.g(this,"DEP");
					je.DateDisplayed=_depositCur.DateDeposit;//it would be nice to add security here.
					je.DebitAmt=_depositCur.Amount;
					je.Memo=Lan.g(this,"Deposit");
					je.Splits=Accounts.GetDescript(PrefC.GetLong(PrefName.AccountingIncomeAccount));
					je.TransactionNum=trans.TransactionNum;
					JournalEntries.Insert(je);
					//then, the income entry
					je=new JournalEntry();
					je.AccountNum=PrefC.GetLong(PrefName.AccountingIncomeAccount);
					//je.CheckNumber=;
					je.DateDisplayed=_depositCur.DateDeposit;//it would be nice to add security here.
					je.CreditAmt=_depositCur.Amount;
					je.Memo=Lan.g(this,"Deposit");
					je.Splits=Accounts.GetDescript(DepositAccounts[comboDepositAccount.SelectedIndex]);
					je.TransactionNum=trans.TransactionNum;
					JournalEntries.Insert(je);
				}
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,_depositCur.DateDeposit.ToShortDateString()+" New "+_depositCur.Amount.ToString("c"));
			}
			else {//Not new
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0,_depositCur.DateDeposit.ToShortDateString()+" "+_depositCur.Amount.ToString("c"));
			}
			DialogResult=DialogResult.OK;
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
			if(changed){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}
	}
}





















