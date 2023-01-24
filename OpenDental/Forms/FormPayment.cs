using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using DentalXChange.Dps.Pos;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using OpenDental.Bridges;
using OpenDental.UI;
using OpenDentBusiness;
using PayConnectService = OpenDentBusiness.PayConnectService;
using OpenDentBusiness.WebTypes.Shared.XWeb;
using PdfSharp.Pdf;
using EdgeExpressProps=OpenDentBusiness.ProgramProperties.PropertyDescs.EdgeExpress;
using Bridges;

namespace OpenDental {
	///<summary></summary>
	public partial class FormPayment:FormODBase {

		#region Fields - Public
		///<summary>Set this value to a PaySplitNum if you want one of the splits highlighted when opening this form.</summary>
		public long PaySplitNumInitial;
		///<summary>Set to true if this payment is supposed to be an income transfer.</summary>
		public bool IsIncomeTransfer;
		///<summary></summary>
		public bool IsNew=false;
		///<summary>Procedures and payplan charges from account module we want to make splits for on this payment.</summary>
		public List<AccountEntry> ListAccountEntriesPayFirst;
		///<summary>Set to a positive amount if there is an unearned amount for the patient and they want to use it.</summary>
		public double UnearnedAmt;
		#endregion

		#region Fields - Private
		private long[] _acctNumArrayDeposits;
		private CareCreditWebResponse _careCreditWebResponse=null;
		///<summary>A dictionary that stores the selected indices for the Outstanding Charges grid that are associated to a single PaySplit.</summary>
		private Dictionary<PaySplitHelper,List<int>> _dictGridChargesPaySplitIndices=new Dictionary<PaySplitHelper,List<int>>();
		///<summary>A dictionary that stores the selected indices for the Current Payment Splits grid that are associated to a single PaySplit.</summary>
		private Dictionary<PaySplitHelper,List<int>> _dictGridSplitsPaySplitIndices=new Dictionary<PaySplitHelper,List<int>>();
		///<summary>A dictionary that stores the selected indices for the Treat Plan grid that are associated to a single PaySplit.</summary>
		private Dictionary<PaySplitHelper,List<int>> _dictGridTreatPlanPaySplitIndices=new Dictionary<PaySplitHelper,List<int>>();
		///<summary>A dictionary or patients that we may need to reference to fill the grids to eliminate unnecessary calls to the DB.
		///Should contain all patients in the current family along with any patients of payment plans of which a member of this family is the guarantor.</summary>
		private Dictionary<long,Patient> _dictPatients;
		private bool _isCCDeclined;
		private bool _isInit;
		private bool _isCareCredit;
		private bool _isPayConnectPortal;
		///<summary>Direct family members of the current patient.</summary>
		private readonly Family _family;
		///<summary></summary>
		private List<AccountEntry> _listAccountEntriesCharges;
		///<summary>Stored CreditCards for _patCur.</summary>
		private List<CreditCard> _listCreditCards;
		///<summary>List of all clinics.</summary>
		private List<Clinic> _listClinics;
		///<summary>List of paysplit that were deleted and need a securitylog entry.</summary>
		private List<PaySplit> _listPaySplitsForSecLog=new List<PaySplit>();
		///<summary>The original splits that existed when this window was opened.  Empty for new payments.</summary>
		private List<PaySplit> _listPaySplitsOld;
		private List<Def> _listDefsPaymentType;
		///<summary>A current list of splits showing on the left grid.</summary>
		private List<PaySplit> _listPaySplits=new List<PaySplit>();
		///<summary>Holds most all the data needed to load the form.</summary>
		private PaymentEdit.LoadData _loadData;
		private int _originalHeight;
		private Patient _patient;
		private PayConnectService.creditCardRequest _creditCardRequestPayConnect;
		private PayConnectResponseWeb _payConnectResponseWeb;
		private PaySimple.ApiResponse _apiResponsePaySimple;
		private Payment _payment;
		private Payment _paymentOld;
		private System.Drawing.Printing.PrintDocument _pd2;
		private bool _doPreferCurrentPat;
		private bool _doPrintReceipt;
		private bool _doPromptSignature;
		private RigorousAccounting _rigorousAccounting;
		///<summary>This table gets created and filled once at the beginning.  After that, only the last column gets carefully updated.</summary>
		private DataTable _tableBalances;
		///<summary>Set to true when X-Charge or PayConnect makes a successful transaction, except for voids.</summary>
		private bool _wasCreditCardSuccessful;
		///<summary>Used to track position inside the MakeXChargeTransaction(), for troubleshooting purposes.</summary>
		public string XchargeMilestone;
		///<summary>The local override path or normal path for X-Charge.</summary>
		private string _xPath;
		///<summary>Program X-Charge.</summary>
		private Program _programX;
		///<summary>The XWebResponse that created this payment. Will only be set if the payment originated from XWeb or EdgeExpress Card Not Present.</summary>
		private XWebResponse _xWebResponse;
		#endregion

		///<summary>PatCur and FamCur are not for the PatCur of the payment.  They are for the patient and family from which this window was accessed.
		///Use listSelectedProcs to automatically attach payment to specific procedures.</summary>
		public FormPayment(Patient patient,Family family,Payment payment,bool doPreferCurrentPat) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_patient=patient;
			_family=family;
			_payment=payment;
			_doPreferCurrentPat=doPreferCurrentPat;
			Lan.F(this);
			panelXcharge.ContextMenu=contextMenuXcharge;
			butPayConnect.ContextMenu=contextMenuPayConnect;
			butPaySimple.ContextMenu=contextMenuPaySimple;
			_paymentOld=payment.Clone();
			_rigorousAccounting=PrefC.GetEnum<RigorousAccounting>(PrefName.RigorousAccounting);
		}

		#region Properties - Private
		///<summary>Returns either the family or super family of the current patients 
		///depending on whether or not the "Show Charges for Superfamily" checkbox is checked.</summary>
		private Family GetFamilyOrSuperFamily() {
			if(checkShowSuperfamily.Checked) {
				return _loadData.SuperFam;
			}
			else {
				return _family;
			}
		}

		///<summary>List of user-inputed proc codes to filter outstanding charges grid on.</summary>
		private List<long> GetListProcCodesFiltered() {
			List<long> listProcCodesFiltered=new List<long>();
			//Proc codes
			List<string> listCodes=textFilterProcCodes.Text.Split(',').ToList();
			for(int i=0;i<listCodes.Count;i++) {
				long retrievedCode=ProcedureCodes.GetCodeNum(listCodes[i].Trim());  //returns 0 if code not found
				if(retrievedCode!=0) {
					listProcCodesFiltered.Add(retrievedCode);
				}
			}
			return listProcCodesFiltered;
		}
		#endregion

		private void FormPayment_Load(object sender,System.EventArgs e) {
			_loadData=PaymentEdit.GetLoadData(_patient,_payment,IsNew,(IsIncomeTransfer || _payment.PayType==0));
			_isPayConnectPortal= _payment.PaymentSource.In(CreditCardSource.PayConnectPortal,CreditCardSource.PayConnectPortalLogin);
			_isCareCredit= _payment.PaymentSource.In(CreditCardSource.CareCredit);
			if(_isPayConnectPortal) {
				groupXWeb.Text="PayConnect Portal";
			}
			else if(_isCareCredit) {
				groupXWeb.Text="CareCredit";
			}
			if(PrefC.GetEnum<YN>(PrefName.PrePayAllowedForTpProcs)!=YN.Yes) {
				tabControlCharges.TabPages.Remove(tabPageTreatPlan);
			}
			//else {
				/*
				//This won't really work with the current tabControl. 
				//It makes no sense to remove it, only to add it back.
				if(tabControlCharges.TabPageNames().Contains(tabPageTreatPlan.Name)){
					tabControlCharges.TabPageRemove(tabPageTreatPlan);
				}
				tabControlCharges.TabPageAdd(tabPageTreatPlan);*/
			//}
			LayoutManager.Move(gridCharges,new Rectangle(x:1,y:LayoutManager.Scale(22),
				width:tabControlCharges.ClientSize.Width-2,height:tabControlCharges.ClientSize.Height-LayoutManager.Scale(23)));
			LayoutManager.Move(gridTreatPlan,new Rectangle(x:1,y:LayoutManager.Scale(22),
				width:tabControlCharges.ClientSize.Width-2,height:tabControlCharges.ClientSize.Height-LayoutManager.Scale(23)));
			if(IsNew) {
				checkPayTypeNone.Enabled=true;
				if(!Security.IsAuthorized(Permissions.PaymentCreate,_payment.PayDate)) {//to prevent backdating of payments, check for date when this form is loaded
					DialogResult=DialogResult.Cancel;
					return;
				}
				butDeletePayment.Enabled=false;
			}
			else {
				checkPayTypeNone.Enabled=false;
				checkRecurring.Checked=_payment.IsRecurringCC;
				if(checkRecurring.Checked) {
					labelRecurringChargeWarning.Visible=true;
					comboCreditCards.Enabled=false;
				} 
				else {
					labelRecurringChargeWarning.Visible=false;
					comboCreditCards.Enabled=true;
				}
				if(!Security.IsAuthorized(Permissions.PaymentEdit,_payment.PayDate)) {
					butOK.Enabled=false;
					butDeletePayment.Enabled=false;
					butAddManual.Enabled=false;
					gridSplits.Enabled=false;
					butPay.Enabled=false;
					butCreatePartial.Enabled=false;
					butClear.Enabled=false;
					butDelete.Enabled=false;
					checkRecurring.Enabled=false;
					panelXcharge.Enabled=false;
					butPayConnect.Enabled=false;
					butPaySimple.Enabled=false;
					if(Security.IsAuthorized(Permissions.SplitCreatePastLockDate,true)) {
						//Since we are enabling the OK button, we need to make sure everything else is disabled (except for Add).
						butOK.Enabled=true;
						butAddManual.Enabled=true;
						comboClinic.Enabled=false;
						textDate.ReadOnly=true;
						textAmount.ReadOnly=true;
						butPrePay.Enabled=false;
						textCheckNum.ReadOnly=true;
						textBankBranch.ReadOnly=true;
						textNote.ReadOnly=true;
						checkPayTypeNone.Enabled=false;
						listPayType.Enabled=false;
						comboDepositAccount.Enabled=false;
						comboCreditCards.Enabled=false;
						checkProcessed.Enabled=false;
						gridSplits.Enabled=true;
					}
				}
			}
			if(_payment.IsCcCompleted) {
				DisablePaymentControls();
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(Programs.GetProgramNum(ProgramName.CareCredit));
			string careCreditMerchantNum=PIn.String(ProgramProperties.GetPropValFromList(listProgramProperties,
				ProgramProperties.PropertyDescs.CareCredit.CareCreditMerchantNumber,0));//Practice merchant num
			if(PrefC.HasClinicsEnabled) {
				comboClinic.SelectedClinicNum=_payment.ClinicNum;
				_listClinics=Clinics.GetDeepCopy();
			}
			else {//clinics not enabled
				comboClinicFilter.Visible=false;
				labelClinicFilter.Visible=false;
			}
			if(_payment.ProcessStatus==ProcessStat.OfficeProcessed) {
				checkProcessed.Visible=false;//This checkbox will only show if the payment originated online.
			}
			else if(_payment.ProcessStatus==ProcessStat.OnlineProcessed) {
				checkProcessed.Checked=true;
			}
			_listCreditCards=_loadData.ListCreditCards;
			FillCreditCards();
			_tableBalances=_loadData.TableBalances;
			//this works even if patient not in family
			textPaidBy.Text=GetFamilyOrSuperFamily().GetNameInFamFL(_payment.PatNum);
			textDateEntry.Text=_payment.DateEntry.ToShortDateString();
			textDate.Text=_payment.PayDate.ToShortDateString();
			textAmount.Text=_payment.PayAmt.ToString("F");
			textCheckNum.Text=_payment.CheckNum;
			textBankBranch.Text=_payment.BankBranch;
			_listDefsPaymentType=Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			for(int i=0;i<_listDefsPaymentType.Count;i++) {
				listPayType.Items.Add(_listDefsPaymentType[i].ItemName);
				if(IsNew && PrefC.GetBool(PrefName.PaymentsPromptForPayType)) {//skip auto selecting payment type if preference is enabled and payment is new
					continue;//user will be forced to selectan indexbefore closing or clicking ok
				}
				if(_listDefsPaymentType[i].DefNum==_payment.PayType) {
					listPayType.SelectedIndex=i;
				}
			}
			textNote.Text=_payment.PayNote;
			Deposit deposit=null;
			if(_payment.DepositNum!=0) {
				deposit=Deposits.GetOne(_payment.DepositNum);
			}
			if(deposit==null) {//If there was none or it got deleted, disable controls.
				labelDeposit.Visible=false;
				textDeposit.Visible=false;
			}
			else {
				textDeposit.Text=deposit.DateDeposit.ToShortDateString();
				textAmount.ReadOnly=true;
				textAmount.BackColor=SystemColors.Control;
				butPay.Enabled=false;
			}
			_listPaySplits=_loadData.ListSplits;//Count might be 0
			_listPaySplitsOld=new List<PaySplit>();
			for(int i=0;i<_listPaySplits.Count;i++) {
				_listPaySplitsOld.Add(_listPaySplits[i].Copy());
			}
			warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.Payment,Payments.ArePaySplitHashesValid(_payment.PayNum,_listPaySplits));
			if(IsNew && CompareDecimal.IsGreaterThanZero(UnearnedAmt)) {
				_loadData.ListSplits=PaymentEdit.AllocateUnearned(_payment.PayNum,UnearnedAmt,ListAccountEntriesPayFirst,_family);
				_listPaySplits=_loadData.ListSplits;
			}
			if(IsNew) {
				//Fill comboDepositAccount based on autopay for listPayType.SelectedIndex
				SetComboDepositAccounts();
				textDepositAccount.Visible=false;
			}
			else {
				//put a description in the textbox.  If the user clicks on the same or another item in listPayType,
				//then the textbox will go away, and be replaced by comboDepositAccount.
				labelDepositAccount.Visible=false;
				comboDepositAccount.Visible=false;
				Transaction transaction=_loadData.Transaction;
				if(transaction==null) {
					textDepositAccount.Visible=false;
				}
				else {
					//add only the description based on PaymentCur attached to transaction
					List<JournalEntry> listJournalEntries=JournalEntries.GetForTrans(transaction.TransactionNum);
					for(int i=0;i<listJournalEntries.Count;i++) {
						Account account=Accounts.GetAccount(listJournalEntries[i].AccountNum);
						//The account could be null if the AccountNum was never set correctly due to the automatic payment entry setup missing an income account from older versions.
						if(account!=null && account.AcctType==AccountType.Asset) {
							textDepositAccount.Text=listJournalEntries[i].DateDisplayed.ToShortDateString();
							if(listJournalEntries[i].DebitAmt>0) {
								textDepositAccount.Text+=" "+listJournalEntries[i].DebitAmt.ToString("c");
							}
							else {//negative
								textDepositAccount.Text+=" "+(-listJournalEntries[i].CreditAmt).ToString("c");
							}
							break;
						}
					}
				}
			}
			if(!string.IsNullOrEmpty(_payment.Receipt)) {
				if(PrefC.GetBool(PrefName.AllowEmailCCReceipt)) {
					butEmailReceipt.Visible=true;
				}
				butPrintReceipt.Visible=true;
			}
			comboGroupBy.Items.Add("None");
			comboGroupBy.Items.Add("Provider");
			if(PrefC.HasClinicsEnabled) {
				comboGroupBy.Items.Add("Clinic and Provider");
			}
			comboGroupBy.SelectedIndex=0;
			if(IsIncomeTransfer || _payment.PayType==0) {
				checkPayTypeNone.Checked=true;
			}
			if(_patient.SuperFamily<=0) {
				checkShowSuperfamily.Visible=false;
			}
			else { 
				//Check the Super Family box if there are any splits from a member in the super family who is not in the immediate family.
				List<Patient> listPatientsSuperFamExclusive=_loadData.SuperFam.ListPats.Where(x => !_family.IsInFamily(x.PatNum)).ToList();
				if(!IsNew && (_listPaySplits.Any(x => listPatientsSuperFamExclusive.Select(y => y.PatNum).Contains(x.PatNum)))) {
					checkShowSuperfamily.Checked=true;
				}
			}
			comboClinicFilter.IncludeAll=true;
			comboPatientFilter.IncludeAll=true;
			comboTypeFilter.IncludeAll=true;
			bool doAutoSplit=false;
			if(IsNew) {
				doAutoSplit=true;//Only perform auto-split logic when this is a new payment.
			}
			Init(doAutoSplit:doAutoSplit,doPayFirstAcctEntries:true,doPreserveValues:false);
			if(PaySplitNumInitial!=0) {
				gridSplits.SetAll(false);
				PaySplit paySplitInit=_listPaySplits.FirstOrDefault(x => x.SplitNum==PaySplitNumInitial);
				if(paySplitInit!=null) {
					SelectPaySplit(paySplitInit);
				}
				HighlightChargesForSplits();
			}
			CheckUIState();
			_originalHeight=Height;
			if(PrefC.GetBool(PrefName.PaymentWindowDefaultHideSplits)) {
				ToggleShowHideSplits();//set hidden
			}
			textCheckNum.Select();
			Plugins.HookAddCode(this,"FormPayment.Load_end",_payment,IsNew);
		}

		///<summary>The shown event is for code that requires the Payment window to be visible already. E.g. showing MsgBoxCopyPaste.</summary>
		private void FormPayment_Shown(object sender,EventArgs e) {
			//jsalmon - The negative unallocated or unearned warning can come back once there is a preference for the office to opt out of receiving it.
			//if(IsNew && PaymentEdit.IsUnallocatedOrUnearnedNegative(_patCur.PatNum,_famCur,out string warningMessage)) {
			//	string displayText=Lans.g("PaymentEdit","The following pre-existing payment splits may be causing the Total of Outstanding Charges to"
			//		+" differ from the family balance:")+"\r\n"+warningMessage;
			//	MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(displayText);
			//	msgBoxCopyPaste.Show();
			//}
		}

		#region Events - Click
		///<summary>Creates a paysplit for the user to edit manually.</summary>
		private void butAddManualSplit_Click(object sender,EventArgs e) {
			PaySplit paySplit=new PaySplit();
			paySplit.SplitAmt=0;
			paySplit.DatePay=_payment.PayDate;
			paySplit.PayNum=_payment.PayNum;
			paySplit.ProvNum=Patients.GetProvNum(_patient);
			paySplit.ClinicNum=_payment.ClinicNum;
			paySplit.IsNew=true;
			using FormPaySplitEdit formPaySplitEdit=new FormPaySplitEdit(GetFamilyOrSuperFamily());
			formPaySplitEdit.ListPaySplits=_listPaySplits;
			formPaySplitEdit.PaySplitCur=paySplit;
			formPaySplitEdit.IsNew=true;
			if(formPaySplitEdit.ShowDialog()==DialogResult.OK) {
				if(!_dictPatients.ContainsKey(paySplit.PatNum)) {
					//add new patnum to _dictPatients
					Patient patient=Patients.GetLim(paySplit.PatNum);
					if(patient!=null) {
						_dictPatients[paySplit.PatNum]=patient;
					}
				}
				_listPaySplits.Add(paySplit);
				Reinitialize();
			}
		}

		private void butCareCredit_Click(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
      if(!ShowOverridePrompt()) {
				return;
      }
			Program program=Programs.GetCur(ProgramName.CareCredit);
			if(!program.Enabled) {
				ODException.SwallowAnyException(() =>
					Process.Start(CareCredit.ProviderSignupURL)
				);
				return;
			}
			double payAmt=PIn.Double(textAmount.Text);
			if(CompareDouble.IsZero(payAmt)) {
				MsgBox.Show(this,"Amount must be greater than zero.");
				return;
			}
			long provNum=0;
			if(CareCredit.IsMerchantNumberByProv) {
				provNum=GetProvNum();
			}
			if(provNum==-1) {
				return;
			}
			try {
				CareCredit.GetMerchantNumber(_payment.ClinicNum,provNum,CareCredit.IsMerchantNumberByProv);
			}
			catch(ODException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			//Enforce Latest IE Version Available.
			if(MiscUtils.TryUpdateIeEmulation()) {
				MsgBox.Show(this,"Browser emulation version updated.\r\nYou must restart this application before making a CareCredit payment.");
				return;
			}
			//Force the payment type to the default CareCredit PayType that was set in the CareCredit Setup window.
			string careCreditPayType=ProgramProperties.GetPropVal(program.ProgramNum,ProgramProperties.PropertyDescs.CareCredit.CareCreditPaymentType,
				_payment.ClinicNum);
			int defCareCredit=Defs.GetOrder(DefCat.PaymentTypes,PIn.Long(careCreditPayType));
			if(defCareCredit==-1) {
				MsgBox.Show(this,$"The CareCredit Setup window does not have a Payment Type set{(PrefC.HasClinicsEnabled ? " for this clinic." : ".")}");
				return;
			}
			if(!PrefC.GetBool(PrefName.PaymentsPromptForPayType)) {
				listPayType.SelectedIndex=defCareCredit;
			}
			SetComboDepositAccounts();
			//Prevent SavePaymentToDb() from setting textAmount.Text to zero. _isCCDeclined is irrelevant here.
			_isCCDeclined=false;
			if(!SavePaymentToDb()) {
				return;
			}
			//After this point, we are closing this form no matter what.
			string urlPurchasePage=CareCreditL.GetPurchasePageUrl(_patient,provNum,_payment.ClinicNum,payAmt,estimatedFeeAmt:payAmt,payNum:_payment.PayNum);
			if(string.IsNullOrEmpty(urlPurchasePage)) {
				//Error occurred when trying to get url. Message already displayed to the user. Return
				DialogResult=DialogResult.OK;
				return;
			}
			using FormCareCreditWeb formCareCreditWeb=new FormCareCreditWeb(urlPurchasePage,_patient);
			formCareCreditWeb.ShowDialog();
			if(string.IsNullOrEmpty(formCareCreditWeb.SessionId)) {
				MsgBox.Show("Error retrieving CareCredit web page.");
				DialogResult=DialogResult.OK;
				return;
			}
			CareCreditWebResponse careCreditWebResponse=CareCreditWebResponses.GetBySessionId(formCareCreditWeb.SessionId);
			if(careCreditWebResponse==null) {
				//This shouldn't happen
				MsgBox.Show("CareCredit web response no longer exist. This payment will not be associated to the CareCredit transaction.");
				return;
			}
			if(IsCareCreditTransStatusCompleted(careCreditWebResponse)) {
				if(careCreditWebResponse.TransType==CareCreditTransType.Purchase) {
					_payment.PayNote=_payment.PayNote+"\r\n"+CareCredit.GetFormattedNote(careCreditWebResponse);
					_payment.PaymentSource=CreditCardSource.CareCredit;
					_payment.IsCcCompleted=true;
					Payments.Update(_payment,true);
					DisablePaymentControls();
				}
			}
			else {
				MsgBox.Show("CareCredit transaction could not be completed. This payment will not be associated to the CareCredit Transactions.");
				CareCreditWebResponses.ClearPayment(careCreditWebResponse.CareCreditWebResponseNum);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCreatePartialSplit_Click(object sender,EventArgs e) {
			if(tabControlCharges.SelectedTab==tabPageOutstanding){
				CreatePartialSplitClickHelper(gridCharges);
			}
			else if(tabControlCharges.SelectedTab==tabPageTreatPlan){
				CreatePartialSplitClickHelper(gridTreatPlan);
			}
			Reinitialize();
		}

		private void butEmailReceipt_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)){
				return;
			}
			if(string.IsNullOrWhiteSpace(_payment.Receipt)) {
				MsgBox.Show(this,"There is no receipt to send for this payment.");
				return;
			}
			List<string> listErrors=new List<string>();
			if(!EmailAddresses.ExistsValidEmail()) {
				listErrors.Add(Lan.g(this,"SMTP server name missing in e-mail setup."));
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase){
				listErrors.Add(Lan.g(this,"No AtoZ folder."));
			}
			if(listErrors.Count>0) {
				MessageBox.Show(this,Lan.g(this,"The following errors need to be resolved before creating an email")+":\r\n"+string.Join("\r\n",listErrors));
				return;
			}
			string attachPath=EmailAttaches.GetAttachPath();
			Random random=new Random();
			string tempFile=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),
				DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+random.Next(1000).ToString()+".pdf");
			PdfDocumentRenderer pdfDocumentRenderer=new PdfDocumentRenderer(true,PdfFontEmbedding.Always);
			pdfDocumentRenderer.Document=CreatePDFDoc(_payment.Receipt);
			pdfDocumentRenderer.RenderDocument();
			pdfDocumentRenderer.PdfDocument.Save(tempFile);
			FileAtoZ.Copy(tempFile,FileAtoZ.CombinePaths(attachPath,Path.GetFileName(tempFile)),FileAtoZSourceDestination.LocalToAtoZ);
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.PatNum=_payment.PatNum;
			emailMessage.ToAddress=_patient.Email;
			EmailAddress emailAddress=EmailAddresses.GetByClinic(_patient.ClinicNum);
			emailMessage.FromAddress=emailAddress.GetFrom();
			emailMessage.Subject=Lan.g(this,"Receipt for payment received ")+_payment.PayDate.ToShortDateString();
			EmailAttach emailAttachRcpt=new EmailAttach() {
				DisplayedFileName="Receipt.pdf",
				ActualFileName=Path.GetFileName(tempFile)
			};
			emailMessage.Attachments=new List<EmailAttach>() { emailAttachRcpt };
			emailMessage.MsgType=EmailMessageSource.PaymentReceipt;
			using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,emailAddress);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.ShowDialog();
		}

		private void butPay_Click(object sender,EventArgs e) {
			if(checkPayTypeNone.Checked) {
				if(!gridSplits.ListGridRows.IsNullOrEmpty()) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Performing a transfer will overwrite all Current Payment Splits.  Continue?")) {
						return;
					}
				}
				_listPaySplits.Clear();//Ignore any splits the user has made / manipulated.
				PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(_family.GetPatNums(),_patient.PatNum,_listPaySplits,
					_payment,ListAccountEntriesPayFirst,isIncomeTxfr:true);
				if(PaymentEdit.TryCreateIncomeTransfer(constructResults.ListAccountEntries,DateTime.Now,out PaymentEdit.IncomeTransferData incomeTransferData)) {
					_listAccountEntriesCharges=constructResults.ListAccountEntries;
					incomeTransferData.ListSplitsCur.ForEach(x => x.PayNum=_payment.PayNum);
					_listPaySplits=incomeTransferData.ListSplitsCur;
					Reinitialize(doRefreshConstructData:true);
				}
				else {
					MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(incomeTransferData.StringBuilderErrors.ToString().TrimEnd());
					msgBoxCopyPaste.Show();
				}
				//Display any warning messages to the user.
				if(incomeTransferData.StringBuilderWarnings.Length > 0) {
					MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste("The following warnings happened during the income transfer process."
						+"\r\n"+incomeTransferData.StringBuilderWarnings.ToString());
					msgBoxCopyPaste.Show();
				}
				return;
			}
			double payAmtOrig=PIn.Double(textAmount.Text);
			_payment.PayAmt=payAmtOrig;//Just in case some other entity set the PayAmt field to a different value or the user manually changed it.
			if(!CompareDouble.IsZero(payAmtOrig)) {
				double payAmtRemaining=(payAmtOrig-_listPaySplits.Sum(x => x.SplitAmt));
				//When the remaining amount is in the negative let the user do whatever they want by treating the PayAmt as 0.
				//Otherwise, this will make it so that the MakePayment method will only suggest PaySplits up to the amount remaining on the payment.
				_payment.PayAmt=Math.Max(0,payAmtRemaining);
			}
			tabControlSplits.SelectedIndex=0;
			List<List<AccountEntry>> listListsAccountEntriesGrid;
			if(tabControlCharges.SelectedTab==tabPageOutstanding){
				listListsAccountEntriesGrid=GetAccountEntriesFromGrid(gridCharges);
			}
			else{
				listListsAccountEntriesGrid=GetAccountEntriesFromGrid(gridTreatPlan);
			}
			//Remove groups of account entries that sum up to an AmountEnd of zero.  There is no reason to be making zero dollar PaySplits.
			//If the user is trying to make a transfer then there needs to be offsetting negative and positive splits.
			listListsAccountEntriesGrid.RemoveAll(x => CompareDecimal.IsZero(x.Sum(y => y.AmountEnd)));
			if(listListsAccountEntriesGrid.Count==0) {
				return;//No need to display a message, no PaySplits showing up in the grid is enough for the user to know that nothing happened.
			}
			PaymentEdit.PayResults payResultsCreatedSplits=PaymentEdit.MakePayment(listListsAccountEntriesGrid,_payment,PIn.Decimal(textAmount.Text),
				_listAccountEntriesCharges);
			_listPaySplits.AddRange(payResultsCreatedSplits.ListSplitsCur);
			_listAccountEntriesCharges=payResultsCreatedSplits.ListAccountCharges;
			Reinitialize();
		}

		private void butPayConnect_Click(object sender,EventArgs e) {
			if(!CanAddNewCreditCard(Programs.GetCur(ProgramName.PayConnect),PayConnect.ProgramProperties.PayConnectPreventSavingNewCC)) {
				return;
			}
      if(!ShowOverridePrompt()) {
				return;
      }
			if(comboCreditCards.SelectedIndex < _listCreditCards.Count) {
				CreditCard creditCard=_listCreditCards[comboCreditCards.SelectedIndex];
				if(creditCard!=null && creditCard.CCSource==CreditCardSource.PayConnectPortal) {
					MsgBox.Show(this,"The selected credit card can only be used to void and return payments made with this card.  Use the Void and Return buttons in this window instead.");
					return;
				}
			}
			MakePayConnectTransaction();
			if(_payment.IsCcCompleted) {
				DisablePaymentControls();
			}
		}

		private void butPaySimple_Click(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(!CanAddNewCreditCard(Programs.GetCur(ProgramName.PaySimple),PaySimple.PropertyDescs.PaySimplePreventSavingNewCC)) {
				return;
			}
      if(!ShowOverridePrompt()) {
				return;
      }
			MakePaySimpleTransaction();
			if(_payment.IsCcCompleted) {
				DisablePaymentControls();
			}
		}

		private void butPrePay_Click(object sender,EventArgs e) {
			if(PIn.Double(textAmount.Text)==0) {
				MsgBox.Show(this,"Amount cannot be zero.");
				return;
			}
			if(_listPaySplits.Count>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will replace all Payment Splits with one split for the total amount.  Continue?")) {
					return;
				}
			}
			_listPaySplits.Clear();
			PaySplit paySplit=new PaySplit();
			paySplit.PatNum=_patient.PatNum;
			paySplit.PayNum=_payment.PayNum;
			paySplit.SplitAmt=PIn.Double(textAmount.Text);
			paySplit.DatePay=DateTime.Now;
			paySplit.ClinicNum=_payment.ClinicNum;
			paySplit.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);
			_listPaySplits.Add(paySplit);
			Reinitialize();
			Application.DoEvents();
			if(!SavePaymentToDb()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butPrintReceipt_Click(object sender,EventArgs e) {
			PrintReceipt(_payment.Receipt,Lan.g(this,"Receipt printed"));
		}

		private void butReturn_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"Are you sure you want to return this transaction?"))) {
				return;
			}
			if(_xWebResponse!=null) {
				XWebReturn();
			}
			else if(_payConnectResponseWeb!=null) {
				PayConnectReturn();
			}
			else if(_careCreditWebResponse!=null) {
				CareCreditL.RefundTransaction(_careCreditWebResponse);
				DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
			}
		}

		private void butShowHide_Click(object sender,EventArgs e) {
			ToggleShowHideSplits();
		}

		private void butVoid_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(MsgBoxButtons.YesNo,Lan.g(this,"Are you sure you want to void this transaction?"))) {
				return;
			}
			if(_xWebResponse!=null) {
				XWebVoid();
			}
			else if(_payConnectResponseWeb!=null) {
				PayConnectVoid();
			}
		}

		private void CheckIncludeExplicitCreditsOnly_Click(object sender,EventArgs e) {
			Reinitialize(doRefreshConstructData:true);
		}

		private void checkPayTypeNone_Click(object sender,EventArgs e) {
			Reinitialize(doRefreshConstructData:true);
		}

		private void checkRecurring_Click(object sender,EventArgs e) {
			if(!checkRecurring.Checked) {
				comboCreditCards.Enabled=true;
				labelRecurringChargeWarning.Visible=false;
				_payment.PayDate=_paymentOld.PayDate;
				textDate.Text=_payment.PayDate.ToShortDateString();
				_payment.RecurringChargeDate=DateTime.MinValue;
				return;
			}
			//User chose to have a recurring payment so we need to know if the card has recurring setup and which month to apply the payment to.
			if(comboCreditCards.SelectedIndex==_listCreditCards.Count) {
				MsgBox.Show(this,"Cannot apply a recurring charge to a new card.");
				checkRecurring.Checked=false;
				return;
			}
			//Check if a recurring charge is setup for the selected card.
			if(_listCreditCards[comboCreditCards.SelectedIndex].ChargeAmt==0 
				|| _listCreditCards[comboCreditCards.SelectedIndex].DateStart.Year < 1880) 
			{
				MsgBox.Show(this,"The selected credit card has not been setup for recurring charges.");
				checkRecurring.Checked=false;
				return;
			}
			//Check if a stop date was set and if that date falls in on today or in the past.
			if(_listCreditCards[comboCreditCards.SelectedIndex].DateStop.Year > 1880
				&& _listCreditCards[comboCreditCards.SelectedIndex].DateStop<=DateTime.Now) 
			{
				MsgBox.Show(this,"This card is no longer accepting recurring charges based on the stop date.");
				checkRecurring.Checked=false;
				return;
			}
			//Have the user decide what month to apply the recurring charge towards.
			using FormCreditRecurringDateChoose formCreditRecurringDateChoose=new FormCreditRecurringDateChoose(_listCreditCards[comboCreditCards.SelectedIndex],_patient);
			formCreditRecurringDateChoose.ShowDialog();
			if(formCreditRecurringDateChoose.DialogResult!=DialogResult.OK) {
				checkRecurring.Checked=false;
				return;
			}
			//This will change the PayDate to work better with the recurring charge automation.  User was notified in previous window.
			if(!PrefC.GetBool(PrefName.RecurringChargesUseTransDate)) {
				_payment.PayDate=formCreditRecurringDateChoose.DatePay;
				textDate.Text=_payment.PayDate.ToShortDateString();
				//Discuss: Should we alert user that we changed the PayDate.
			}
			comboCreditCards.Enabled=false;
			labelRecurringChargeWarning.Visible=true;
			_payment.RecurringChargeDate=formCreditRecurringDateChoose.DatePay;
		}

		private void checkShowAll_Clicked(object sender,EventArgs e) {
			Reinitialize(doSelectAllSplits:true);
		}

		///<summary>Constructs a list of AccountCharges and goes through and links those charges to credits.</summary>
		private void checkShowSuperfamily_Click(object sender,EventArgs e) {
			if(_patient.SuperFamily==0) { //if no super family, just return.
				return;
			}
			Reinitialize(doRefreshConstructData:true);
		}

		///<summary>When a charge is selected this method highlights all paysplits associated with it.</summary>
		private void gridCharges_CellClick(object sender,ODGridClickEventArgs e) {
			CellClickHelper(gridCharges);
		}
		
		///<summary>When a paysplit is selected this method highlights all charges associated with it.</summary>
		private void gridSplits_CellClick(object sender,ODGridClickEventArgs e) {
			HighlightChargesForSplits();
		}

		///<summary>Allows editing of an individual double clicked paysplit entry.</summary>
		private void gridSplits_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			PaySplit paySplitOld=(PaySplit)gridSplits.ListGridRows[e.Row].Tag;
			PaySplit paySplit=paySplitOld.Copy();
			if(paySplit.DateEntry!=DateTime.MinValue && !Security.IsAuthorized(Permissions.PaymentEdit,paySplit.DatePay,false)) {
				return;
			}
			using FormPaySplitEdit formPaySplitEdit=new FormPaySplitEdit(GetFamilyOrSuperFamily());
			formPaySplitEdit.ListPaySplits=_listPaySplits;
			formPaySplitEdit.PaySplitCur=paySplit;
			if(formPaySplitEdit.ShowDialog()!=DialogResult.OK) {
				HighlightChargesForSplits();
				return;
			}
			//paySplit contains all the info we want.
			DeleteSelected(doCreateSecLog:false);
			if(paySplit!=null && !_dictPatients.ContainsKey(paySplit.PatNum)) {
				//add new patnum to _dictPatients
				Patient patient=Patients.GetLim(paySplit.PatNum);
				if(patient!=null) {
					_dictPatients[paySplit.PatNum]=patient;
				}
			}
			if(formPaySplitEdit.PaySplitCur==null) {//Deleted the paysplit, just return here.
				return;
			}
			//A shallow copy of the list of splits was passed into the PaySplit edit window so it could have been manipulated within.
			//The user most likely changed something about the split which would cause paySplitOld to no longer be in the list.
			if(!_listPaySplits.Contains(paySplitOld)) {
				//At this point we know that paySplit is not null (it is a shallow copy of FormPSE.PaySplitCur) and it was being manipulated by the user.
				//Add it to the list of splits and try to associate the 'new-ish' split to an AccountEntry if possible.
				//E.g. if the user attached the split to a procedure, adjustment, etc. it should then be associated to the corresponding account entry.
				_listPaySplits.Add(paySplit);
				Reinitialize();
				//Try and select the PaySplit that was double clicked if it is still around.
				SelectPaySplit(paySplitOld);
				_payment.PayAmt-=paySplit.SplitAmt;
			}
			HighlightChargesForSplits();
		}

		///<summary>When a Treatment Plan Procedure is selected this method highlights all paysplits associated with it.</summary>
		private void gridTreatPlan_CellClick(object sender,ODGridClickEventArgs e) {
			CellClickHelper(gridTreatPlan);
		}

		private void listPayType_Click(object sender,EventArgs e) {
			if(Plugins.HookMethod(this,"FormPayment.listPayType_Click",listPayType.SelectedItem)) {
				return;
			}
			textDepositAccount.Visible=false;
			SetComboDepositAccounts();
		}

		private void menuPayConnect_Click(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Setup)) {
				using FormPayConnectSetup formPayConnectSetup=new FormPayConnectSetup();
				formPayConnectSetup.ShowDialog();
				CheckUIState();
			}
		}

		private void menuPaySimple_Click(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Setup)) {
				using FormPaySimpleSetup formPaySimpleSetup=new FormPaySimpleSetup();
				formPaySimpleSetup.ShowDialog();
				CheckUIState();
			}
		}

		private void menuXcharge_Click(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Setup)) {
				using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
				formXchargeSetup.ShowDialog();
				CheckUIState();
			}
		}

		private void panelXcharge_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(!CanAddNewCreditCard(Programs.GetCur(ProgramName.Xcharge),ProgramProperties.PropertyDescs.XCharge.XChargePreventSavingNewCC)) {
				return;
			}
      if(!ShowOverridePrompt()) {
				return;
      }
			XchargeMilestone="";
			try {
				MakeXChargeTransaction();
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error processing transaction.\r\n\r\nPlease contact support with the details of this error:")
					//The rest of the message is not translated on purpose because we here at HQ need to always be able to quickly read this part.
					+"\r\nLast valid milestone reached: "+XchargeMilestone,ex);
			}
			if(_payment.IsCcCompleted) {
				DisablePaymentControls();
			}
		}

		private void panelEdgeExpress_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(!CanAddNewCreditCard(Programs.GetCur(ProgramName.EdgeExpress),ProgramProperties.PropertyDescs.EdgeExpress.PreventSavingNewCC)) {
				return;
			}
      if(!ShowOverridePrompt()) {
				return;
      }
			try {
				MakeEdgeExpressTransaction();
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error processing transaction.\r\n\r\nPlease contact support with the details of this error:")+"\r\n"+ex.Message,ex);
			}
			//Either cancel was clicked or the window was closed.
			if(_payment.IsCcCompleted) {
				DisablePaymentControls();
			}
		}

		#endregion

		#region Events - Change
		private void checkPayTypeNone_CheckedChanged(object sender,EventArgs e) {
			//this fires before the click event.  The Checked property also reflects the new value.
			if(checkPayTypeNone.Checked) {
				listPayType.Visible=false;
				butPay.Text=Lan.g(this,"Transfer");
				if(PrefC.HasClinicsEnabled) {
					comboGroupBy.SelectedIndex=2;
				}
				else { 
					comboGroupBy.SelectedIndex=1;
				}
				butCreatePartial.Visible=false;
				checkIncludeExplicitCreditsOnly.Enabled=false;
				checkIncludeExplicitCreditsOnly.Checked=false;
				//Clear out all of the values in the Filtering group box and then disable it.
				FillFilters(doPreserveValues:false);
				groupBoxFiltering.Enabled=false;
				gridCharges.AllowSelection=false;
				CheckUIState();
				SetComboDepositAccounts();
				return;
			}
			listPayType.Visible=true;
			butPay.Text=Lan.g(this,"Pay");
			butCreatePartial.Visible=true;
			butCreatePartial.Text=Lan.g(this,"Add Partials");
			checkIncludeExplicitCreditsOnly.Enabled=true;
			groupBoxFiltering.Enabled=true;
			gridCharges.AllowSelection=true;
			CheckUIState();
			SetComboDepositAccounts();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			//_listUserClinicNums contains all clinics the user has access to as well as ClinicNum 0 for 'none'
			_payment.ClinicNum=comboClinic.SelectedClinicNum;
			if(_listPaySplits.Count>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Change clinic for all splits?")) {
					return;
				}
				_listPaySplits.ForEach(x => x.ClinicNum=_payment.ClinicNum);
				Reinitialize();
			}
			CheckUIState();
		}

		private void comboGroupBy_SelectionChangeCommitted(object sender,EventArgs e) {
			//Go through and disable/enable filters depending on the group by state.
			if(comboGroupBy.SelectedIndex==1) {	//Group By Providers
				comboTypeFilter.IsAllSelected=true;
				comboClinicFilter.IsAllSelected=true;
				comboTypeFilter.Enabled=false;
				comboClinicFilter.Enabled=false;
				Reinitialize();
				return;
			}
			if(comboGroupBy.SelectedIndex==2) {	//Group by providers and clinics
				comboTypeFilter.Enabled=false;
				comboClinicFilter.Enabled=true;
				Reinitialize();
				return;
			}
			//Not grouping by anything
			comboTypeFilter.Enabled=true;
			comboClinicFilter.Enabled=true;
			Reinitialize();
		}
		#endregion

		#region Methods - Private
		///<summary>Prompt the user if this payment already had IsCcCompleted set to true.</summary>
		private bool ShowOverridePrompt() {
			if(_payment.IsCcCompleted) {
				string prompt = "Warning: This payment already contains data from a previously successful card transaction.  "
					+"Some data may be overwritten if you choose to charge a card again.  Continue?";
				return MessageBox.Show(Lan.g(this,prompt),"Alert",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Exclamation)==DialogResult.Yes;
			}
			return true;//If there's no need to ask for an override, just return true
    }

		///<summary>Returns the selected provider or first provider. Otherwise returns -1.</summary>
		private long GetProvNum() {
			List<long> listProvNums=_listPaySplits.Where(x => x.ProvNum!=0).Select(x => x.ProvNum).Distinct().ToList();
			long provNum;
			if(listProvNums.IsNullOrEmpty()) {
				if(_rigorousAccounting==RigorousAccounting.DontEnforce) {
					listProvNums.Add(Patients.GetProvNum(_patient));
				}
				else {
					//Paysplits should automatically get created.
					//Add all providers for the clinic on the payment so user can choose the provider.
					listProvNums=Providers.GetProvsForClinic(_payment.ClinicNum).Select(x => x.ProvNum).ToList();
				}
			}
			if(listProvNums.Count>1) { 
				//Paysplits are empty or more than one paysplit provider are attached to provider. Choose provider
				List<Provider>listProviders=Providers.GetProvsByProvNums(listProvNums);
				if(listProviders.IsNullOrEmpty()) {
					MsgBox.Show(this,"No providers found.");
					return -1;
				}
				using FormProviderPick formProviderPick=new FormProviderPick(listProviders);
				if(listProvNums.Contains(_patient.PriProv)){
					formProviderPick.ProvNumSelected=_patient.PriProv;
				}
				if(formProviderPick.ShowDialog()!=DialogResult.OK) {
					return -1;
				}
				return formProviderPick.ProvNumSelected;
			}
			return listProvNums.First();//default provNum to the first provider
		}

		private void AddCreditCardsToCombo(List<CreditCard> listCreditCards,Func<CreditCard,bool> funcSelectCard=null) {
			comboCreditCards.Items.Clear();
			for(int i=0;i<listCreditCards.Count;i++) {
				string cardNum=listCreditCards[i].CCNumberMasked;
				if(Regex.IsMatch(cardNum,"^\\d{12}(\\d{0,7})")) { //Credit cards can have a minimum of 12 digits, maximum of 19
					int idxLast4Digits=(cardNum.Length-4);
					cardNum=(new string('X',12))+cardNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
				}
				if(listCreditCards[i].IsXWeb()) {
					cardNum+=" (XWeb)";
				}
				string tokensForCC=listCreditCards[i].GetTokenString();
				cardNum+=(string.IsNullOrEmpty(tokensForCC) ? "" : " "+tokensForCC);
				if(PrefC.HasClinicsEnabled) {
					Clinic clinic=_listClinics.FirstOrDefault(x => x.ClinicNum==listCreditCards[i].ClinicNum);
					if(clinic!=null) {  //adds abbr if cc has clinic attached
						cardNum+=" - ("+clinic.Abbr.ToString()+")";
					}
				}
				comboCreditCards.Items.Add(cardNum,listCreditCards[i]);
				if(funcSelectCard!=null && funcSelectCard(listCreditCards[i])) {
					comboCreditCards.SelectedIndex=i;
				}
			}
			comboCreditCards.Items.Add(Lan.g(this,"New Card"),new CreditCard()); //CreditCardNum=0
			if(comboCreditCards.SelectedIndex < 0) {
				comboCreditCards.SelectedIndex=comboCreditCards.Items.Count-1;
			}
		}

		private void AddIndexToDictForPaySplitMultiple(int index,List<AccountEntry> listAccountEntries,Dictionary<PaySplitHelper,List<int>> dictPaySplitIndices) {
			//Loop through all splits in the collection and make sure that each split is associated to this account entry index.
			for(int i=0;i<listAccountEntries.Count;i++) {
				AddIndexToDictForPaySplit(index,listAccountEntries[i],dictPaySplitIndices);
			}
		}

		private void AddIndexToDictForPaySplit(int index,AccountEntry accountEntry,Dictionary<PaySplitHelper,List<int>> dictPaySplitIndices) {
			//Loop through all splits in the collection and make sure that each split is associated to this account entry index.
			List<PaySplit> listPaySplitsCollection=accountEntry.SplitCollection.ToList();
			for(int i=0;i<listPaySplitsCollection.Count;i++) {
				PaySplitHelper paySplitHelper=new PaySplitHelper(listPaySplitsCollection[i]);
				if(dictPaySplitIndices.TryGetValue(paySplitHelper,out List<int> listIndices)) {
					if(!listIndices.Contains(index)) {
						dictPaySplitIndices[paySplitHelper].Add(index);
					}
					continue;
				}
				dictPaySplitIndices[paySplitHelper]=new List<int>() { index };
			}
		}

		///<summary>Adds one split to _listPaySplits to work with.  Does not link the payment plan, that must be done outside this method.
		///Called when checkPayPlan click, or upon load if auto attaching to payplan, or upon OK click if no splits were created.</summary>
		private bool AddOneSplit(bool doPromptForPayPlan=false) {
			PaySplit paySplit=new PaySplit();
			paySplit.PatNum=_patient.PatNum;
			paySplit.PayNum=_payment.PayNum;
			paySplit.DatePay=_payment.PayDate;//this may be updated upon closing
			if(_rigorousAccounting==RigorousAccounting.DontEnforce) {
				paySplit.ProvNum=Patients.GetProvNum(_patient);
			}
			else {
				paySplit.ProvNum=0;
				paySplit.UnearnedType=PrefC.GetLong(PrefName.PrepaymentUnearnedType);//Use default unallocated type
			}
			paySplit.ClinicNum=_payment.ClinicNum;
			paySplit.SplitAmt=PIn.Double(textAmount.Text);
			if(doPromptForPayPlan && _loadData.ListValidPayPlans.Count > 0) {
				using FormPayPlanSelect formPayPlanSelect=new FormPayPlanSelect(_loadData.ListValidPayPlans,true);
				formPayPlanSelect.ShowDialog();
				if(formPayPlanSelect.DialogResult!=DialogResult.OK) {
					return false;
				}
				paySplit.PayPlanNum=formPayPlanSelect.PayPlanNumSelected;
			}
			_listPaySplits.Add(paySplit);
			_payment.PayAmt=PIn.Double(textAmount.Text);
			return true;
		}

		///<summary>Executes auto-split logic for the current state of the window and returns the suggested payment splits.</summary>
		private List<PaySplit> GetSuggestedAutoSplits() {
			//Construct and link charges as if this payment window was loaded (but using the current state).
			List<long> listPatNums=GetFamilyOrSuperFamily().ListPats.Select(x => x.PatNum).ToList();
			PaymentEdit.ConstructResults constructResults=PaymentEdit.ConstructAndLinkChargeCredits(listPatNums,_patient.PatNum,_listPaySplits,_payment
				,ListAccountEntriesPayFirst,checkPayTypeNone.Checked,_doPreferCurrentPat,_loadData);
			//Execute auto-split logic and return the suggested splits.
			return PaymentEdit.AutoSplitForPayment(constructResults).ListPaySplitsSuggested;
		}

		///<summary>Returns true if the user can add a new credit card.</summary>
		private bool CanAddNewCreditCard(Program program,string progPropertyDescription) {
			if(!Programs.IsEnabledByHq(program,out string err)) {
				MsgBox.Show(err);
				return false;
			}
			if(comboCreditCards.GetSelected<CreditCard>()==null) {
				MsgBox.Show(this,"Invalid credit card selected.");
				return false;
			}
			bool hasPreventCcAdd=PIn.Bool(ProgramProperties.GetPropVal(program.ProgramNum,progPropertyDescription,_payment.ClinicNum));
			CreditCard creditCardSelected=comboCreditCards.GetSelected<CreditCard>();
			if(creditCardSelected==null) {
				return !hasPreventCcAdd;
			}
			bool hasToken=false;
			if(program.ProgName==ProgramName.Xcharge.ToString() && !string.IsNullOrEmpty(creditCardSelected.XChargeToken)) {
				hasToken=true;
			}
			else if(program.ProgName==ProgramName.EdgeExpress.ToString() && !string.IsNullOrEmpty(creditCardSelected.XChargeToken)) {
				hasToken=true;
			}
			else if(program.ProgName==ProgramName.PayConnect.ToString() && !string.IsNullOrEmpty(creditCardSelected.PayConnectToken)) {
				hasToken=true;
			}
			else if(program.ProgName==ProgramName.PaySimple.ToString() && !string.IsNullOrEmpty(creditCardSelected.PaySimpleToken)) {
				hasToken=true;
			}
			if(hasPreventCcAdd && (creditCardSelected.CreditCardNum==0 || !hasToken)) {
				MsgBox.Show(this,"Cannot add a new credit card.");
				return false;
			}
			return true;
		}

		///<summary>Takes an ODGrid and evaluates its single click events - proccessing selections based on paySplit type and entry type.
		///This method requires the passed in grids to have the same number of rows.</summary>
		private void CellClickHelper(GridOD grid) {
			gridSplits.SetAll(false);
			List<AccountEntry> listSelectedCharges=new List<AccountEntry>();
			if(comboGroupBy.SelectedIndex!=0) {	//Grouping is active
				List<List<AccountEntry>> listListsAccountEntries=grid.SelectedTags<List<AccountEntry>>();
				for(int i=0;i<listListsAccountEntries.Count;i++) {
					listSelectedCharges.AddRange(listListsAccountEntries[i]);
				}
				SelectPaySplitsForAccountEntries(listSelectedCharges);
				UpdateChargeTotalWithSelectedEntries();
				return;
			}
			//No grouping
			List<AccountEntry> listAccountEntries=grid.SelectedTags<AccountEntry>();
			for(int i=0;i<listAccountEntries.Count;i++) {
				listSelectedCharges.Add(listAccountEntries[i]);
			}
			SelectPaySplitsForAccountEntries(listSelectedCharges);
			UpdateChargeTotalWithSelectedEntries();
		}

		///<summary>Mimics FormClaimPayEdit.CheckUIState().</summary>
		private void CheckUIState() {
			_programX=Programs.GetCur(ProgramName.Xcharge);
			_xPath=Programs.GetProgramPath(_programX);
			Program programEdgeExpress=Programs.GetCur(ProgramName.EdgeExpress);
			Program programPayConnect=Programs.GetCur(ProgramName.PayConnect);
			Program programPaySimple=Programs.GetCur(ProgramName.PaySimple);
			Program programCareCredit=Programs.GetCur(ProgramName.CareCredit);
			if(_programX==null || programPayConnect==null || programPaySimple==null || programCareCredit==null || programEdgeExpress==null) {//Should not happen.
				panelXcharge.Visible=(_programX!=null);
				butPayConnect.Visible=(programPayConnect!=null);
				butPaySimple.Visible=(programPaySimple!=null);
				butCareCredit.Visible=(programCareCredit!=null);
				panelEdgeExpress.Visible=(programEdgeExpress!=null);
				return;
			}
			panelXcharge.Visible=false;
			panelEdgeExpress.Visible=false;
			butPayConnect.Visible=false;
			butPaySimple.Visible=false;
			butCareCredit.Visible=false;
			if(checkPayTypeNone.Checked) {
				return;
			}
			butCareCredit.Visible=!ProgramProperties.IsAdvertisingDisabled(programCareCredit);
			if(!programPayConnect.Enabled && !_programX.Enabled && !programEdgeExpress.Enabled && !programPaySimple.Enabled) {//if none enabled
				//show all so user can pick
				panelEdgeExpress.Visible=true;
				butPayConnect.Visible=true;
				butPaySimple.Visible=true;
				return;
			}
			//show if enabled.  User could have all enabled.
			if(programPayConnect.Enabled) {
				//if clinics are disabled, PayConnect is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					butPayConnect.Visible=true;
				}
				else {//if clinics are enabled, PayConnect is enabled if the PaymentType is valid and the Username and Password are not blank
					string paymentType=ProgramProperties.GetPropVal(programPayConnect.ProgramNum,"PaymentType",_payment.ClinicNum);
					string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(programPayConnect.ProgramNum,"Password",_payment.ClinicNum));
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropVal(programPayConnect.ProgramNum,"Username",_payment.ClinicNum))
						&& !string.IsNullOrEmpty(password)
						&& _listDefsPaymentType.Any(x => x.DefNum.ToString()==paymentType))
					{
						butPayConnect.Visible=true;
					}
				}
			}
			if(programEdgeExpress.Enabled) {
				//if clinics are disabled, EdgeExpress is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					panelEdgeExpress.Visible=true;
				}
				else {//if clinics are enabled, EdgeExpress is enabled if the XWeb creds are not blank
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropVal(programEdgeExpress.ProgramNum,EdgeExpressProps.XWebID,_payment.ClinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropVal(programEdgeExpress.ProgramNum,EdgeExpressProps.AuthKey,_payment.ClinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropVal(programEdgeExpress.ProgramNum,EdgeExpressProps.TerminalID,_payment.ClinicNum))) 
					{
						panelEdgeExpress.Visible=true;
						panelEdgeExpress.BringToFront();
					}
				}
			}
			if(_programX.Enabled) {
				//if clinics are disabled, X-Charge is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					panelXcharge.Visible=true;
				}
				else {//if clinics are enabled, X-Charge is enabled if the PaymentType is valid and the Username and Password are not blank
					string paymentType=ProgramProperties.GetPropVal(_programX.ProgramNum,"PaymentType",_payment.ClinicNum);
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropVal(_programX.ProgramNum,"Username",_payment.ClinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropVal(_programX.ProgramNum,"Password",_payment.ClinicNum))
						&& _listDefsPaymentType.Any(x => x.DefNum.ToString()==paymentType))
					{
						panelXcharge.Visible=true;
					}
				}
			}
			if(programPaySimple.Enabled) {
				//if clinics are disabled, PaySimple is enabled if marked enabled
				if(!PrefC.HasClinicsEnabled) {
					butPaySimple.Visible=true;
				}
				else {//if clinics are enabled, PaySimple is enabled if the PaymentType is valid and the Username and Key are not blank
					string paymentType=ProgramProperties.GetPropValForClinicOrDefault(programPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimplePayTypeCC,_payment.ClinicNum);
					if(!string.IsNullOrEmpty(ProgramProperties.GetPropValForClinicOrDefault(programPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiUserName,_payment.ClinicNum))
						&& !string.IsNullOrEmpty(ProgramProperties.GetPropValForClinicOrDefault(programPaySimple.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiKey,_payment.ClinicNum))
						&& _listDefsPaymentType.Any(x => x.DefNum.ToString()==paymentType)) {
						butPaySimple.Visible=true;
					}
				}
			}
			if(panelXcharge.Visible==false && butPayConnect.Visible==false && butPaySimple.Visible==false && panelEdgeExpress.Visible==false) {
				//This is an office with clinics and one of the payment processing bridges is enabled but this particular clinic doesn't have one set up.
				if(_programX.Enabled) {
					panelXcharge.Visible=true;
				}
				if(programEdgeExpress.Enabled) {
					panelEdgeExpress.Visible=true;
				}
				if(programPayConnect.Enabled) {
					butPayConnect.Visible=true;
				}
				if(programPaySimple.Enabled) {
					butPaySimple.Visible=true;
				}
			}
		}

		///<summary>Creates a split similar to how CreateSplitsForPayment does it, but with selected rows of the grid.
		///If payAmt==0, attempt to pay charge in full.</summary>
		private void CreateSplit(AccountEntry accountEntryCharge,decimal payAmt,bool isManual=false) {
			PaymentEdit.PayResults payResultsCreatedSplit=PaymentEdit.CreatePaySplit(accountEntryCharge,payAmt,_payment,PIn.Decimal(textAmount.Text),_listAccountEntriesCharges,
				isManual);
			_listPaySplits.AddRange(payResultsCreatedSplit.ListSplitsCur);
			_listAccountEntriesCharges=payResultsCreatedSplit.ListAccountCharges;
			_payment=payResultsCreatedSplit.Payment;
		}

		///<summary>A method which, for a given grid, allows the user to split a payment between procedures on it.</summary>
		private void CreatePartialSplitClickHelper(GridOD grid) {
			if(comboGroupBy.SelectedIndex > 0) {
				List<List<AccountEntry>> listListsAccountEntriesSelected=grid.SelectedTags<List<AccountEntry>>();
				for(int i=0;i<listListsAccountEntriesSelected.Count;i++) {
					CreatPartialSplitForAccountEntries(listListsAccountEntriesSelected[i].ToArray());
				}
				return;
			}
			List<AccountEntry> listAccountEntriesSelected=grid.SelectedTags<AccountEntry>();
			for(int i=0;i<listAccountEntriesSelected.Count;i++) {
				CreatPartialSplitForAccountEntries(listAccountEntriesSelected[i]);
			}
		}

		///<summary></summary>
		private void CreatPartialSplitForAccountEntries(params AccountEntry[] accountEntryArray) {
			using FormAmountEdit formAmountEdit=new FormAmountEdit(GetCodesDescriptForEntries(10,accountEntryArray));
			//Suggest the maximum amount remaining for all of the account entries passed in.
			decimal amountEndTotal=accountEntryArray.Sum(x => x.AmountEnd);
			formAmountEdit.Amount=amountEndTotal;
			formAmountEdit.ShowDialog();
			if(formAmountEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			decimal amount=formAmountEdit.Amount;
			//Warn the user if they chose to overpay the selected account entries which will put them into the negative.
			if(amountEndTotal < amount) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"One or more Outstanding Charges will be negative.  Continue?","Overpaid Charge Warning")) {
					return;
				}
			}
			List<AccountEntry> listAccountEntries=accountEntryArray.ToList();
			for(int i=0;i<listAccountEntries.Count;i++) {
				int splitCountOld=_listPaySplits.Count;
				CreateSplit(listAccountEntries[i],amount,true);
				if(_listPaySplits.Count > 0 && splitCountOld!=_listPaySplits.Count) {
					amount-=(decimal)_listPaySplits.Last().SplitAmt;
				}
			}
		}

		private MigraDoc.DocumentObjectModel.Document CreatePDFDoc(string receiptStr) {
			string[] stringArrayReceiptLines=receiptStr.Split("\r\n",StringSplitOptions.None);
			MigraDoc.DocumentObjectModel.Document document=new MigraDoc.DocumentObjectModel.Document();
			document.DefaultPageSetup.PageWidth=Unit.FromInch(3.0);
			document.DefaultPageSetup.PageHeight=Unit.FromInch(0.181*stringArrayReceiptLines.Length+0.56);//enough to print text plus 9/16 in. (0.56) extra space at bottom.
			document.DefaultPageSetup.TopMargin=Unit.FromInch(0.25);
			document.DefaultPageSetup.LeftMargin=Unit.FromInch(0.25);
			document.DefaultPageSetup.RightMargin=Unit.FromInch(0.25);
			MigraDoc.DocumentObjectModel.Font fontBodyX=MigraDocHelper.CreateFont(8,false);
			fontBodyX.Name=FontFamily.GenericMonospace.Name;
			Section section=document.AddSection();
			Paragraph paragraph=section.AddParagraph();
			ParagraphFormat paragraphFormat=new ParagraphFormat();
			paragraphFormat.Alignment=ParagraphAlignment.Left;
			paragraphFormat.Font=fontBodyX;
			paragraph.Format=paragraphFormat;
			paragraph.AddFormattedText(receiptStr,fontBodyX);
			return document;
		}

		///<summary>Deletes selected paysplits from the grid and attributes amounts back to where they originated from.
		///This will return a list of payment plan charges that were affected. This is so that splits can be correctly re-attributed to the payplancharge
		///when the user edits the paysplit. There should only ever be one payplancharge in that list, since the user can only edit one split at a time.</summary>
		private void DeleteSelected(bool doCreateSecLog=true) {
			bool suppressMessage=false;
			List<PaySplit> listPaySplits=gridSplits.SelectedTags<PaySplit>();
			for(int i=0;i<listPaySplits.Count;i++) {
				if(listPaySplits[i].DateEntry!=DateTime.MinValue && !Security.IsAuthorized(Permissions.PaymentEdit,listPaySplits[i].DatePay,suppressMessage)) {
					suppressMessage=true;
					continue;//Don't delete this paysplit
				}
				_loadData.ListSplits.Remove(listPaySplits[i]);
				_listPaySplits.Remove(listPaySplits[i]);
				if(doCreateSecLog && listPaySplits[i].SplitNum!=0) { 
					_listPaySplitsForSecLog.Add(listPaySplits[i]);
				}
			}
			Reinitialize(doRefreshConstructData:true);
		}

		///<summary>Disables merchant buttons if the PaymentsCompletedDisableMerchantButtons pref is true.</summary>
		private void DisablePaymentControls() {
			if(PrefC.GetBool(PrefName.PaymentsCompletedDisableMerchantButtons)) {
				labelTransactionCompleted.Visible=true;
				panelEdgeExpress.Enabled=false;
				panelXcharge.Enabled=false;
				butPaySimple.Enabled=false;
				butPayConnect.Enabled=false;
			}
		}

		///<summary>Returns true if the AccountEntry matches the currently selected filters.</summary>
		private bool DoShowAccountEntry(AccountEntry accountEntryCharge) {
			//Never show future payment plan charges that have no value (future patient payment plan debits).
			if(CompareDecimal.IsZero(accountEntryCharge.AmountEnd) && accountEntryCharge.GetType()==typeof(FauxAccountEntry) && accountEntryCharge.Date > DateTime.Today) {
				return false;
			}
			//Never show offsetting payment plan charges.
			if(accountEntryCharge.GetType()==typeof(FauxAccountEntry) && ((FauxAccountEntry)accountEntryCharge).IsOffset) {
				return false;
			}
			//Never show PaySplits or PayAsTotal rows within the Outstanding Charges grid (those are income and charges are typically production).
			//These types of objects will only be present when viewing income transfer payments.
			if(accountEntryCharge.GetType().In(typeof(PaySplit),typeof(PayAsTotal))) {
				return false;
			}
			//Never show procedures or adjustments for patients outside of the family or super family.
			if(!GetFamilyOrSuperFamily().GetPatNums().Contains(accountEntryCharge.PatNum) 
				&& accountEntryCharge.GetType().In(typeof(Adjustment),typeof(Procedure)))
			{
				//Payment plan charges are the only outstanding charges that should be shown for patients outside of the family.
				return false;
			}
			List<long> listPatNumsFiltered=comboPatientFilter.GetListSelected<Patient>().Select(x => x.PatNum).ToList();
			if(!listPatNumsFiltered.Contains(accountEntryCharge.PatNum)) {
				return false;
			}
			List<long> listProvNums=comboProviderFilter.GetSelectedProvNums();
			if(!listProvNums.Contains(accountEntryCharge.ProvNum)) {
				return false;
			}
			List<long> listClinicNumsFiltered=comboClinicFilter.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			if(PrefC.HasClinicsEnabled && comboGroupBy.SelectedIndex!=1 && !listClinicNumsFiltered.Contains(accountEntryCharge.ClinicNum)) {
				return false;
			}
			//proc code filter
			if(GetListProcCodesFiltered().Count > 0) {
				if(accountEntryCharge.Tag.GetType()!=typeof(Procedure)) {
					return false;
				}
				if(!GetListProcCodesFiltered().Contains(((Procedure)accountEntryCharge.Tag).CodeNum)) {
					return false;
				}
			}
			//Charge Amount Filter
			if(amtMaxEnd.Value!=0 && accountEntryCharge.AmountEnd > amtMaxEnd.Value) {
				return false;
			}
			//Charge Amount Filter
			if(amtMinEnd.Value!=0 && accountEntryCharge.AmountEnd < amtMinEnd.Value) {
				return false;
			}
			//daterange filter
			if((accountEntryCharge.Date.Date < datePickFrom.Value.Date) || (accountEntryCharge.Date.Date > datePickTo.Value.Date)) { 
				return false;
			}
			//Type Filter
			List<string> listTypeFiltered=comboTypeFilter.GetListSelected<string>();
			if(!listTypeFiltered.Contains(accountEntryCharge.GetType().Name)) {
				return false;
			}
			return true;
		}

		private GridRow FillChargesHelper(List<AccountEntry> listAccountEntriesForRow,bool doIncludeClinic) {
			AccountEntry accountEntryCharge=listAccountEntriesForRow.First();
			GridRow row=new GridRow();
			row.Tag=listAccountEntriesForRow;
			row.Cells.Add(Providers.GetAbbr(accountEntryCharge.ProvNum));//Provider
			if(checkPayTypeNone.Checked) {
				if(!_dictPatients.TryGetValue(accountEntryCharge.PatNum,out Patient pat)) {
					pat=Patients.GetLim(accountEntryCharge.PatNum);
					_dictPatients[pat.PatNum]=pat;
				}
				row.Cells.Add(pat.GetNameLFnoPref());//patient
			}
			if(doIncludeClinic) {
				row.Cells.Add(Clinics.GetAbbr(accountEntryCharge.ClinicNum));
			}
			int procCodeLimit=(doIncludeClinic ? 9 : 10);//this column is shorter when filtering by prov + clinic
			row.Cells.Add(GetCodesDescriptForEntries(procCodeLimit,listAccountEntriesForRow.ToArray()));//ProcCodes
			row.Cells.Add(listAccountEntriesForRow.Sum(x => x.AmountEnd).ToString("f"));//Amount End
			return row;
		}

		private void FillCreditCards() {
			AddCreditCardsToCombo(_listCreditCards);
			comboCreditCards.SelectedIndex=0;
			bool isXWebCardPresent=_listCreditCards.Any(x => x.IsXWeb());
			bool isPayConnectPortalCardPresent=_listCreditCards.Any(x => x.IsPayConnectPortal());
			_xWebResponse=_loadData.XWebResponse;
			_payConnectResponseWeb=_loadData.PayConnectResponseWeb;
			_careCreditWebResponse=_loadData.CareCreditWebResponse;
			string _payConnectTransType="";
			if(_payConnectResponseWeb!=null && _payConnectResponseWeb.IsFromWebPortal) {
				_payConnectTransType=_payConnectResponseWeb.TransType.ToString();
			}
			groupXWeb.Visible=false;
			if(!IsNew && (isXWebCardPresent || _xWebResponse!=null 
				|| _isPayConnectPortal || _payConnectResponseWeb!=null || _careCreditWebResponse!=null)) 
			{
				groupXWeb.Visible=true;
			}
			//PayConnect will only let you void a payment in the first 25 minutes.
			if((_payConnectResponseWeb==null || _payConnectTransType==PayConnectService.transType.VOID.ToString() || DateTime.Now > _payConnectResponseWeb.DateTimeEntry.AddMinutes(25))
				&& (_xWebResponse==null || _xWebResponse.XTransactionType==XWebTransactionType.CreditVoidTransaction)
				|| _isCareCredit)
			{
				//Can't run an XWeb/PayConnect void unless this payment is attached to a non-void transaction.
				//CareCredit can never void
				butVoid.Visible=false;
				LayoutManager.MoveHeight(groupXWeb,55);
			}
			if(!isXWebCardPresent && _payConnectResponseWeb==null && _careCreditWebResponse==null) {
				butReturn.Visible=false;
				LayoutManager.MoveLocation(butVoid,butReturn.Location);
				LayoutManager.MoveHeight(groupXWeb,55);
			}
		}

		///<summary>Fills the filter controls and preserves the current values based on the parameter passed in.</summary>
		private void FillFilters(bool doPreserveValues) {
			if(_listAccountEntriesCharges==null) {
				return;
			}
			FillFilterFromToDates(doPreserveValues);
			FillFilterMinMaxAmts(doPreserveValues);
			FillFilterPatients(doPreserveValues);
			FillFilterProviders(doPreserveValues);
			FillFilterClinics(doPreserveValues);
			FillFilterTypes(doPreserveValues);
		}

		private void FillFilterFromToDates(bool doPreserveValues) {
			if(doPreserveValues) {
				return;//It's fine to have values set in these date boxes that fall outside of the range of the current account entries.
			}
			//If there are no account charges, the date time will be DateTime.MinimumDate
			//Get latest proc, or tomorrows date, whichever is later.
			//Ignore account entries with a Date set to MaxValue (TP PayPlanCharge account entries).
			List<DateTime> listDatesAccountEntry=_listAccountEntriesCharges
				.Where(x => x.Date>=datePickFrom.MinDate && x.Date.Date!=DateTime.MaxValue.Date)
				.Select(x => x.Date)
				.ToList();
			if(listDatesAccountEntry.IsNullOrEmpty()) {
				datePickFrom.Value=datePickFrom.Value;
				datePickTo.Value=DateTime.Today;
				return;
			}
			datePickFrom.Value=listDatesAccountEntry.Min();
			datePickTo.Value=ODMathLib.Max(listDatesAccountEntry.Max(),DateTime.Today);
		}

		private void FillFilterMinMaxAmts(bool doPreserveValues) {
			if(doPreserveValues) {
				return;//It's fine to have values set in these amount boxes that fall outside of the range of the current account entries.
			}
			amtMinEnd.Value=0;
			amtMaxEnd.Value=0;
		}

		private void FillFilterPatients(bool doPreserveValues) {
			bool wasAllSelected=comboPatientFilter.IsAllSelected;
			List<long> listPatNumsSelected=comboPatientFilter.GetListSelected<Patient>().Select(x => x.PatNum).ToList();
			comboPatientFilter.Items.Clear();
			//Fill the patient filter combo box with the known patients relating to the list of account charges.
			List<long> listPatNums=_listAccountEntriesCharges.Select(x => x.PatNum).Distinct().ToList();
			for(int i=0;i<listPatNums.Count;i++) {
				if(_dictPatients.TryGetValue(listPatNums[i],out Patient pat)) {
					comboPatientFilter.Items.Add(pat.GetNameFirstOrPreferred(),pat);
					if(doPreserveValues && listPatNumsSelected.Contains(pat.PatNum)) {
						comboPatientFilter.SetSelected(comboPatientFilter.Items.Count-1);
					}
				}
			}
			if(wasAllSelected || !doPreserveValues) {
				comboPatientFilter.IsAllSelected=true;
			}
		}

		private void FillFilterProviders(bool doPreserveValues) {
			bool wasAllSelected=comboPatientFilter.IsAllSelected;
			List<long> listProvNumsSelected=comboProviderFilter.GetSelectedProvNums();
			comboProviderFilter.Items.Clear();
			comboProviderFilter.IncludeAll=true;
			comboProviderFilter.Items.AddProvNone();
			List<Provider> listProviders=Providers.GetProvsByProvNums(_listAccountEntriesCharges.Select(x => x.ProvNum).Distinct().ToList());
			comboProviderFilter.Items.AddProvsAbbr(listProviders);
			if(!wasAllSelected && doPreserveValues) {
				//Reselect providers that were selected before refilling the combo box.
				for(int i=0;i<=comboProviderFilter.Items.Count;i++) {
					if(comboProviderFilter.Items.GetObjectAt(i) is Provider provider && listProvNumsSelected.Contains(provider.ProvNum)) {
						comboProviderFilter.SetSelected(i);
					}
				}
			}
			if(wasAllSelected || !doPreserveValues) {
				comboProviderFilter.IsAllSelected=true;
			}
		}

		private void FillFilterClinics(bool doPreserveValues) {
			if(!PrefC.HasClinicsEnabled) {
				return;
			}
			bool wasAllSelected=comboClinicFilter.IsAllSelected;
			List<long> listClinicNumsSelected=comboClinicFilter.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			if(!_listClinics.IsNullOrEmpty() && !_listClinics.Any(x => x.Abbr==Lan.g(this,"Unassigned"))) {
				_listClinics.Add(new Clinic() { Abbr=Lan.g(this,"Unassigned") });
			}
			comboClinicFilter.Items.Clear();
			List<long> listClinicNums=_listAccountEntriesCharges.Select(x => x.ClinicNum).Distinct().ToList();
			List<Clinic> listClinics=_listClinics.FindAll(x => listClinicNums.Contains(x.ClinicNum));
			for(int i=0;i<listClinics.Count;i++) {
				comboClinicFilter.Items.Add(listClinics[i].Abbr,listClinics[i]);
				if(doPreserveValues && listClinicNumsSelected.Contains(listClinics[i].ClinicNum)) {
					comboClinicFilter.SetSelected(comboClinicFilter.Items.Count-1);
				}
			}
			if(wasAllSelected || !doPreserveValues) {
				comboClinicFilter.IsAllSelected=true;
			}
		}

		private void FillFilterTypes(bool doPreserveValues) {
			bool wasAllSelected=comboTypeFilter.IsAllSelected;
			List<string> listTypesSelected=comboTypeFilter.GetListSelected<string>();
			comboTypeFilter.Items.Clear();
			comboTypeFilter.Items.AddList(_listAccountEntriesCharges.Select(x => x.GetType().Name).Distinct().ToList(),x=>GetDisplayStringForType(x));
			if(!wasAllSelected && doPreserveValues) {
				//Reselect providers that were selected before refilling the combo box.
				for(int i=0;i<=comboTypeFilter.Items.Count;i++) {
					if(comboTypeFilter.Items.GetObjectAt(i) is string type && listTypesSelected.Contains(type)) {
						comboTypeFilter.SetSelected(i);
					}
				}
			}
			if(wasAllSelected || !doPreserveValues) {
				comboTypeFilter.IsAllSelected=true;
			}
		}

		private struct ProvPatClinic {
			public long ProvNum;
			public long PatNum;
			public long ClinicNum;
		}

		///<summary></summary>
		private void FillGridCharges() {
			//Fill right-hand grid with all the charges, filtered based on checkbox and filters.
			gridCharges.BeginUpdate();
			gridCharges.Columns.Clear();
			_dictGridChargesPaySplitIndices.Clear();
			GridColumn col;
			decimal chargeTotal=0;
			List<AccountEntry> listAccountEntriesOutstandingCharges=_listAccountEntriesCharges.FindAll(x => DoShowAccountEntry(x)
				&& (x.GetType()!=typeof(Procedure) || (x.GetType()==typeof(Procedure) && ((Procedure)x.Tag).ProcStatus==ProcStat.C)));
			#region Group By Provider
			if(comboGroupBy.SelectedIndex==1) {//Group by 'Provider'
				col=new GridColumn(Lan.g(this,"Prov"),checkPayTypeNone.Checked?70:110);
				gridCharges.Columns.Add(col);
				if(checkPayTypeNone.Checked) {
					col=new GridColumn(Lan.g(this,"Patient"),119);
					gridCharges.Columns.Add(col);
				}
				col=new GridColumn(Lan.g(this,"Codes"),50){ IsWidthDynamic=true,DynamicWeight=1 };
				gridCharges.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Amt End"),70,HorizontalAlignment.Right,GridSortingStrategy.AmountParse);
				gridCharges.Columns.Add(col);
				gridCharges.ListGridRows.Clear();
				List<AccountEntry> listAccountEntriesFiltered=listAccountEntriesOutstandingCharges.FindAll(x => (checkShowAll.Checked || !CompareDecimal.IsZero(x.AmountEnd)));
				List<ProvPatClinic> listProvPatClinics=new List<ProvPatClinic>();
				for(int i=0;i<listAccountEntriesFiltered.Count;i++) {
					ProvPatClinic provPatClinic=new ProvPatClinic();
					provPatClinic.ProvNum=listAccountEntriesFiltered[i].ProvNum;
					provPatClinic.PatNum=listAccountEntriesFiltered[i].PatNum;
					if(listProvPatClinics.Contains(provPatClinic)) { 
						continue;
					}
					listProvPatClinics.Add(provPatClinic);
				}
				for(int i=0;i<listProvPatClinics.Count;i++) {
					List<AccountEntry> listAccountEntriesProvPat=listAccountEntriesFiltered.FindAll(x => x.PatNum==listProvPatClinics[i].PatNum && x.ProvNum==listProvPatClinics[i].ProvNum);
					for(int j=0;j<listAccountEntriesProvPat.Count;j++) {
						AddIndexToDictForPaySplit(gridCharges.ListGridRows.Count,listAccountEntriesProvPat[j],_dictGridChargesPaySplitIndices);
					}
					gridCharges.ListGridRows.Add(FillChargesHelper(listAccountEntriesProvPat,false));
					chargeTotal+=listAccountEntriesProvPat.Sum(x => x.AmountEnd);
				}
				textChargeTotal.Text=chargeTotal.ToString("f");
				gridCharges.EndUpdate();
				return;
			}
			#endregion
			#region Group By Clinic and Provider
			if(comboGroupBy.SelectedIndex==2) {//Group by 'Clinic and Provider'
				col=new GridColumn(Lan.g(this,"Prov"),checkPayTypeNone.Checked?70:100);
				gridCharges.Columns.Add(col);
				if(checkPayTypeNone.Checked) {
					col=new GridColumn(Lan.g(this,"Patient"),100);
					gridCharges.Columns.Add(col);
				}
				col=new GridColumn(Lan.g(this,"Clinic"),60);
				gridCharges.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Codes"),50){ IsWidthDynamic=true };
				gridCharges.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Amt End"),70,HorizontalAlignment.Right,GridSortingStrategy.AmountParse);
				gridCharges.Columns.Add(col);
				gridCharges.ListGridRows.Clear();
				List<AccountEntry> listAccountEntriesFiltered=listAccountEntriesOutstandingCharges.FindAll(x => (checkShowAll.Checked || !CompareDecimal.IsZero(x.AmountEnd)));
				List<ProvPatClinic> listProvPatClinics=new List<ProvPatClinic>();
				for(int i=0;i<listAccountEntriesFiltered.Count;i++) {
					ProvPatClinic provPatClinic=new ProvPatClinic();
					provPatClinic.ProvNum=listAccountEntriesFiltered[i].ProvNum;
					provPatClinic.PatNum=listAccountEntriesFiltered[i].PatNum;
					provPatClinic.ClinicNum=listAccountEntriesFiltered[i].ClinicNum;
					if(listProvPatClinics.Contains(provPatClinic)) { 
						continue;
					}
					listProvPatClinics.Add(provPatClinic);
				}
				for(int i=0;i<listProvPatClinics.Count;i++) {
					List<AccountEntry> listAccountEntriesPPC=listAccountEntriesFiltered.FindAll(x => x.PatNum==listProvPatClinics[i].PatNum && x.ProvNum==listProvPatClinics[i].ProvNum && x.ClinicNum==listProvPatClinics[i].ClinicNum);
					for(int j=0;j<listAccountEntriesPPC.Count;j++) {
						AddIndexToDictForPaySplit(gridCharges.ListGridRows.Count,listAccountEntriesPPC[j],_dictGridChargesPaySplitIndices);
					}
					gridCharges.ListGridRows.Add(FillChargesHelper(listAccountEntriesPPC,true));
					chargeTotal+=listAccountEntriesPPC.Sum(x => x.AmountEnd);
				}
				textChargeTotal.Text=chargeTotal.ToString("f");
				gridCharges.EndUpdate();
				return;
			}
			#endregion
			#region Group By None
			//Group by 'None'
			col=new GridColumn(Lan.g(this,"Date"),65,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),120,GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=2 };
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Prov"),40,GridSortingStrategy.StringCompare) { IsWidthDynamic=true };
			gridCharges.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {//Clinics
				col=new GridColumn(Lan.g(this,"Clinic"),55,GridSortingStrategy.StringCompare) { IsWidthDynamic=true };
				gridCharges.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Code"),45,GridSortingStrategy.StringCompare) { IsWidthDynamic=true };
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Tth"),25,GridSortingStrategy.ToothNumberParse);
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),120,GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=2 };
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"AmtOrig"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse) { IsWidthDynamic=true };
			gridCharges.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"AmtEnd"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse) { IsWidthDynamic=true };
			gridCharges.Columns.Add(col);
			gridCharges.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listAccountEntriesOutstandingCharges.Count;i++) {
				//Filter out those that are paid in full and from other payments if checkbox unchecked.
				if(!checkShowAll.Checked && CompareDecimal.IsZero(listAccountEntriesOutstandingCharges[i].AmountEnd)) {
					bool doShowZeroCharge=false;
					List<PaySplit> listPaySplits=gridSplits.GetTags<PaySplit>();
					for(int j=0;j<listPaySplits.Count;j++) {
						if(listAccountEntriesOutstandingCharges[i].SplitCollection.Contains(listPaySplits[j])) {
							//Charge is paid for by a split in this payment, display it.
							if(listAccountEntriesOutstandingCharges[i].GetType()==typeof(Procedure) && listPaySplits[j].PayPlanNum!=0) {
								//Don't show the charge if it's a proc being paid by a payplan split.
								//From the user's perspective they're paying the "debits" not the procs.
							}
							else {
								doShowZeroCharge=true;
								break;
							}
						}
						else if(listAccountEntriesOutstandingCharges[i].GetType()==typeof(FauxAccountEntry)
							&& listPaySplits[j].PayPlanNum==((FauxAccountEntry)listAccountEntriesOutstandingCharges[i]).PayPlanNum
							&& !CompareDecimal.IsZero(listAccountEntriesOutstandingCharges[i].AmountEnd))
						{
							doShowZeroCharge=true;
							break;
						}
					}
					if(!doShowZeroCharge) {
						continue;
					}
				}
				row=new GridRow();
				row.Tag=listAccountEntriesOutstandingCharges[i];
				row.Cells.Add(listAccountEntriesOutstandingCharges[i].Date.ToShortDateString());//Date
				if(!_dictPatients.TryGetValue(listAccountEntriesOutstandingCharges[i].PatNum,out Patient patCur)) {
					patCur=Patients.GetLim(listAccountEntriesOutstandingCharges[i].PatNum);
					_dictPatients[patCur.PatNum]=patCur;
				}
				string patName=patCur.GetNameLFnoPref();
				if(listAccountEntriesOutstandingCharges[i].Tag.GetType()==typeof(FauxAccountEntry)
					&& ((FauxAccountEntry)listAccountEntriesOutstandingCharges[i]).Guarantor > 0
					&& ((FauxAccountEntry)listAccountEntriesOutstandingCharges[i]).Guarantor!=patCur.PatNum)
				{
					if(!_dictPatients.TryGetValue(((FauxAccountEntry)listAccountEntriesOutstandingCharges[i]).Guarantor,out Patient guarantor)) {
						guarantor=Patients.GetLim(((FauxAccountEntry)listAccountEntriesOutstandingCharges[i]).Guarantor);
						_dictPatients[guarantor.PatNum]=guarantor;
					}
					patName+="\r\n"+Lan.g(this,"Guar")+": "+guarantor.GetNameLFnoPref();
				}
				row.Cells.Add(patName);//Patient
				row.Cells.Add(Providers.GetAbbr(listAccountEntriesOutstandingCharges[i].ProvNum));//Provider
				if(PrefC.HasClinicsEnabled) {//Clinics
					row.Cells.Add(Clinics.GetAbbr(listAccountEntriesOutstandingCharges[i].ClinicNum));
				}
				string procCode="";
				string tth="";
				Procedure procedure=null;
				if(listAccountEntriesOutstandingCharges[i].Tag.GetType()==typeof(Procedure)) {
					procedure=(Procedure)listAccountEntriesOutstandingCharges[i].Tag;
					tth=procedure.ToothNum=="" ? procedure.Surf : Tooth.Display(procedure.ToothNum);
					procCode+=ProcedureCodes.GetStringProcCode(procedure.CodeNum);
				}
				row.Cells.Add(procCode);//ProcCode
				row.Cells.Add(tth);
				row.Cells.Add(listAccountEntriesOutstandingCharges[i].DescriptionForGrid);//Type
				row.Cells.Add(listAccountEntriesOutstandingCharges[i].AmountOriginal.ToString("f"));//Amount Original
				row.Cells.Add(listAccountEntriesOutstandingCharges[i].AmountEnd.ToString("f"));//Amount End
				chargeTotal+=listAccountEntriesOutstandingCharges[i].AmountEnd;
				//Associate every single split for every single account entry that matches this PayPlanChargeNum
				if(listAccountEntriesOutstandingCharges[i].PayPlanChargeNum > 0) {
					AddIndexToDictForPaySplitMultiple(gridCharges.ListGridRows.Count,
						listAccountEntriesOutstandingCharges.FindAll(x => x.PayPlanChargeNum==listAccountEntriesOutstandingCharges[i].PayPlanChargeNum),
						_dictGridChargesPaySplitIndices);
				}
				else {//Just add the index for the splits associated to the current entryCharge.
					AddIndexToDictForPaySplit(gridCharges.ListGridRows.Count,listAccountEntriesOutstandingCharges[i],_dictGridChargesPaySplitIndices);
				}
				gridCharges.ListGridRows.Add(row);
			}
			textChargeTotal.Text=chargeTotal.ToString("f");
			gridCharges.EndUpdate();
			#endregion
		}

		///<summary>Fills the Current Payment Splits grid and then invokes methods to refresh the charges, treat plan, and allocated grids.</summary>
		private void FillGridSplits() {
			//Fill left grid with paysplits created
			List<long> listProcsNumsMissing=_listPaySplits.Where(x => x.ProcNum!=0 && !_loadData.ListProcsForSplits.Any(y => y.ProcNum==x.ProcNum))
				.Select(x => x.ProcNum).ToList();
			_loadData.ListProcsForSplits.AddRange(Procedures.GetManyProc(listProcsNumsMissing,false));
			gridSplits.BeginUpdate();
			gridSplits.Columns.Clear();
			_dictGridSplitsPaySplitIndices.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Prov"),0,GridSortingStrategy.StringCompare) { IsWidthDynamic=true, DynamicWeight=1 };
			gridSplits.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {//Clinics
				col=new GridColumn(Lan.g(this,"Clinic"),0,GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=1 };
				gridSplits.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Patient"),0,GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=2 };
			gridSplits.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Code"),60, GridSortingStrategy.StringCompare);
			gridSplits.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),0, GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=2 };
			gridSplits.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),55,HorizontalAlignment.Right, GridSortingStrategy.AmountParse);
			gridSplits.Columns.Add(col);
			gridSplits.ListGridRows.Clear();
			GridRow row;
			decimal splitTotal=0;
			for(int i=0;i<_listPaySplits.Count;i++) {
				PaySplitHelper paySplitHelper=new PaySplitHelper(_listPaySplits[i]);
				int index=gridSplits.ListGridRows.Count;
				splitTotal+=(decimal)_listPaySplits[i].SplitAmt;
				row=new GridRow();
				row.Tag=_listPaySplits[i];
				_dictGridSplitsPaySplitIndices[paySplitHelper]=new List<int>() { index };
				row.Cells.Add(Providers.GetAbbr(_listPaySplits[i].ProvNum));//Prov
				if(PrefC.HasClinicsEnabled) {//Clinics
					if(_listPaySplits[i].ClinicNum!=0) {
						row.Cells.Add(Clinics.GetAbbr(_listPaySplits[i].ClinicNum));//Clinic
					}
					else {
						row.Cells.Add("");//Clinic
					}
				}
				Patient patient;
				if(!_dictPatients.TryGetValue(_listPaySplits[i].PatNum,out patient)) {
					patient=Patients.GetLim(_listPaySplits[i].PatNum);
					_dictPatients[patient.PatNum]=patient;
				}
				string patName=patient.LName + ", " + patient.FName;
				row.Cells.Add(patName);//Patient
				Procedure procedure=new Procedure();
				if(_listPaySplits[i].ProcNum!=0) {
					procedure=_loadData.ListProcsForSplits.FirstOrDefault(x => x.ProcNum==_listPaySplits[i].ProcNum)??new Procedure();
				}
				row.Cells.Add(ProcedureCodes.GetStringProcCode(procedure.CodeNum));//ProcCode
				bool isUnallocated=false;
				List<string> listTypeStrs=new List<string>();
				if(_listPaySplits[i].PayPlanNum > 0) {
					listTypeStrs.Add("PayPlanCharge");
				}
				if(_listPaySplits[i].ProcNum > 0) {//Procedure
					listTypeStrs.Add("Proc: "+Procedures.GetDescription(procedure));
				}
				else if(_listPaySplits[i].PayPlanChargeNum > 0) {//Might be payment plan interest when not a procedure.
					//Newer payment splits can explicitly indicate if they are applied towards principal or interest.
					//However, old (legacy) payment splits did not have a paradigm for explicitly specifying where they were supposed to be applied.
					//Find any corresponding account entries for this PayPlanChargeNum and see if any have an interest amount that matches this amount.
					//Always use the AmountAvailable value to equate to the SplitAmt just in case they paid for a portion of the interest in another payment.
					bool isLegacyInterest=(_listPaySplits[i].PayPlanDebitType==PayPlanDebitTypes.Unknown 
						&& _listAccountEntriesCharges.Where(x => x.GetType()==typeof(FauxAccountEntry) && x.PayPlanChargeNum==_listPaySplits[i].PayPlanChargeNum)
							.Cast<FauxAccountEntry>()
							.Any(x => !CompareDecimal.IsZero(x.Interest) && CompareDecimal.IsEqual(x.AmountAvailable,(decimal)_listPaySplits[i].SplitAmt)));
					if(_listPaySplits[i].PayPlanDebitType==PayPlanDebitTypes.Interest || isLegacyInterest) {
						listTypeStrs.Add("(interest)");
					}
				}
				if(_listPaySplits[i].AdjNum > 0) {
					listTypeStrs.Add("Adjustment");
				}
				if(_listPaySplits[i].UnearnedType > 0) {
					listTypeStrs.Add(Defs.GetName(DefCat.PaySplitUnearnedType,_listPaySplits[i].UnearnedType));
				}
				if(listTypeStrs.Count==0) {
					isUnallocated=true;
					listTypeStrs.Add("Unallocated");
				}
				row.Cells.Add(string.Join("\r\n",listTypeStrs));
				if(isUnallocated) {
					row.Cells.Last().ColorText=System.Drawing.Color.Red;
				}
				if(!_family.GetPatNums().Contains(_listPaySplits[i].PatNum)) {
					listTypeStrs.Add("(split to another family)");
				}
				row.Cells.Add(_listPaySplits[i].SplitAmt.ToString("f"));//Amount
				gridSplits.ListGridRows.Add(row);
			}
			textSplitTotal.Text=splitTotal.ToString("f");
			gridSplits.EndUpdate();
		}

		///<summary>Fills Treatment Plan Procedures grid.</summary>
		private void FillGridTreatPlan() {
			//Fill right-hand grid with all the TP procedures.
			gridTreatPlan.BeginUpdate();
			gridTreatPlan.Columns.Clear();
			_dictGridTreatPlanPaySplitIndices.Clear();
			GridColumn col;
			#region Group By None
			col=new GridColumn(Lan.g(this,"Date"),65,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			gridTreatPlan.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),120,GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=2 };
			gridTreatPlan.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Prov"),40,GridSortingStrategy.StringCompare) { IsWidthDynamic=true };
			gridTreatPlan.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {//Clinics
				col=new GridColumn(Lan.g(this,"Clinic"),55,GridSortingStrategy.StringCompare) { IsWidthDynamic=true };
				gridTreatPlan.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Code"),45,GridSortingStrategy.StringCompare) { IsWidthDynamic=true };
			gridTreatPlan.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Tth"),25,GridSortingStrategy.ToothNumberParse);
			gridTreatPlan.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),120,GridSortingStrategy.StringCompare) { IsWidthDynamic=true,DynamicWeight=2 };
			gridTreatPlan.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"AmtOrig"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse) { IsWidthDynamic=true };
			gridTreatPlan.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"AmtEnd"),60,HorizontalAlignment.Right,GridSortingStrategy.AmountParse) { IsWidthDynamic=true };
			gridTreatPlan.Columns.Add(col);
			gridTreatPlan.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAccountEntriesCharges.Count;i++) {
				if(_listAccountEntriesCharges[i].GetType()!=typeof(Procedure) || ((Procedure)_listAccountEntriesCharges[i].Tag).ProcStatus!=ProcStat.TP) {
					continue;
				}
				if(!DoShowAccountEntry(_listAccountEntriesCharges[i])) {
					continue;
				}
				row=new GridRow();
				row.Tag=_listAccountEntriesCharges[i];
				row.Cells.Add(_listAccountEntriesCharges[i].Date.ToShortDateString());//Date
				if(!_dictPatients.TryGetValue(_listAccountEntriesCharges[i].PatNum,out Patient patCur)) {
					patCur=Patients.GetLim(_listAccountEntriesCharges[i].PatNum);
					_dictPatients[patCur.PatNum]=patCur;
				}
				string patName=patCur.GetNameLFnoPref();
				if(_listAccountEntriesCharges[i].Tag.GetType()==typeof(FauxAccountEntry)) {
					if(!_dictPatients.TryGetValue(((FauxAccountEntry)_listAccountEntriesCharges[i]).Guarantor,out Patient guarantor)) {
						guarantor=Patients.GetLim(((FauxAccountEntry)_listAccountEntriesCharges[i]).Guarantor);
						_dictPatients[patCur.PatNum]=guarantor;
					}
					patName+="\r\n"+Lan.g(this,"Guar")+": "+guarantor.GetNameLFnoPref();
				}
				row.Cells.Add(patName);//Patient
				row.Cells.Add(Providers.GetAbbr(_listAccountEntriesCharges[i].ProvNum));//Provider
				if(PrefC.HasClinicsEnabled) {//Clinics
					row.Cells.Add(Clinics.GetAbbr(_listAccountEntriesCharges[i].ClinicNum));
				}
				string procCode="";
				string tth="";
				Procedure procedure=null;
				if(_listAccountEntriesCharges[i].Tag.GetType()==typeof(Procedure)) {
					procedure=(Procedure)_listAccountEntriesCharges[i].Tag;
					tth=procedure.ToothNum=="" ? procedure.Surf : Tooth.Display(procedure.ToothNum);
					procCode+=ProcedureCodes.GetStringProcCode(procedure.CodeNum);
				}
				row.Cells.Add(procCode);//ProcCode
				row.Cells.Add(tth);//Tooth Number
				if(_listAccountEntriesCharges[i].GetType()==typeof(PaySplit)) {
					row.Cells.Add("Unallocated");
				}
				else {
					row.Cells.Add(_listAccountEntriesCharges[i].GetType().Name);//Type
				}
				if(_listAccountEntriesCharges[i].GetType()==typeof(Procedure)) {
					//Get the proc and add its description if the row is a proc.
					row.Cells[row.Cells.Count-1].Text=Lan.g(this,"Proc")+": "+Procedures.GetDescription(procedure);
				}
				row.Cells.Add(_listAccountEntriesCharges[i].AmountOriginal.ToString("f"));//Amount Original
				row.Cells.Add(_listAccountEntriesCharges[i].AmountEnd.ToString("f"));//Amount End
				AddIndexToDictForPaySplit(gridTreatPlan.ListGridRows.Count,_listAccountEntriesCharges[i],_dictGridTreatPlanPaySplitIndices);
				gridTreatPlan.ListGridRows.Add(row);
			}
			#endregion
			gridTreatPlan.EndUpdate();
		}
		
		///<summary>Called whenever any of the filtering objects are changed.  Rechecks filtering and refreshes the grid.</summary>
		private void FilterChangeCommitted(object sender, EventArgs e) {
			if(_isInit) {
				return;
			}
			FillGridCharges();
			FillGridTreatPlan();
		}

		///<summary>This method goes through the passed in grid and returns the selected account entries in a normalized list of list of account entries.
		///If there are no current selections in the grid passed in then all rows will be treated as selected.
		///This is considered normalized because the Outstanding Charges and Treatment Planned Procedures grids can have a single AccountEntry object
		///per row (not grouped) or a list of AccountEntry objects per row (when using the Group By combo box).</summary>
		private List<List<AccountEntry>> GetAccountEntriesFromGrid(GridOD grid) {
			List<List<AccountEntry>> listListsAccountEntriesSelectedCharges=new List<List<AccountEntry>>();
			bool hasSelectedIndices=(grid.SelectedIndices.Length > 0);
			if(!hasSelectedIndices) {
				grid.SetAll(true);//Artificially select every row in the grid.
			}
			if(comboGroupBy.SelectedIndex > 0) {
				List<List<AccountEntry>> listListsAccountEntries=grid.SelectedTags<List<AccountEntry>>();
				for(int i=0;i<listListsAccountEntries.Count;i++) {
					listListsAccountEntriesSelectedCharges.Add(listListsAccountEntries[i]);
				}
			}
			else {
				List<AccountEntry> listAccountEntries= grid.SelectedTags<AccountEntry>();
				for(int i=0;i<listAccountEntries.Count;i++) {
					listListsAccountEntriesSelectedCharges.Add(new List<AccountEntry>() { listAccountEntries[i] });
				}
			}
			if(!hasSelectedIndices) {
				grid.SetAll(false);//Deselect all of the rows to preserve old behavior.
			}
			return listListsAccountEntriesSelectedCharges;
		}

		private string GetCodesDescriptForEntries(int procCodeLimit,params AccountEntry[] accountEntryArray) {
			List<string> listProcCodes=new List<string>();
			List<AccountEntry> listAccountEntries=accountEntryArray.Where(x => x.GetType()==typeof(Procedure)).ToList();
			for(int i=0;i<listAccountEntries.Count;i++) {
				if(listProcCodes.Count>=procCodeLimit) {
					listProcCodes.Add("(...)");
					break;
				}
				string procCode="";
				Procedure procedure=(Procedure)listAccountEntries[i].Tag;
				if(procedure.ProcStatus==ProcStat.TP) {
					procCode+="(TP)";//this needs to be handled differently. TP Procs need to be in their own provider grouping
				}
				procCode+=ProcedureCodes.GetStringProcCode(procedure.CodeNum);
				listProcCodes.Add(procCode);
			}
			return string.Join(", ",listProcCodes);
		}

		private string GetXChargeTransactionTypeCommands(int tranType,bool hasXToken,bool isNotRecurring,CreditCard creditCard,string cashBack) {
			string tranText="";
			switch(tranType) {
				case 0:
					tranText+="/TRANSACTIONTYPE:PURCHASE /LOCKTRANTYPE /LOCKAMOUNT ";
					if(hasXToken && creditCard!=null) {
						tranText+="/XCACCOUNTID:"+creditCard.XChargeToken+" ";
						tranText+="/AUTOPROCESS ";
						tranText+="/GETXCACCOUNTIDSTATUS ";
					}
					if(isNotRecurring && creditCard!=null) {
						tranText+="/ACCOUNT:"+creditCard.CCNumberMasked+" ";
						tranText+="/AUTOPROCESS ";
					}
					break;
				case 1:
					tranText+="/TRANSACTIONTYPE:RETURN /LOCKTRANTYPE /LOCKAMOUNT ";
					if(hasXToken) {
						tranText+="/XCACCOUNTID:"+creditCard.XChargeToken+" ";
						tranText+="/AUTOPROCESS ";
						tranText+="/GETXCACCOUNTIDSTATUS ";
					}
					if(isNotRecurring) {
						tranText+="/ACCOUNT:"+creditCard.CCNumberMasked+" ";
						tranText+="/AUTOPROCESS ";
					}
					break;
				case 2:
					tranText+="/TRANSACTIONTYPE:DEBITPURCHASE /LOCKTRANTYPE /LOCKAMOUNT ";
					tranText+="/CASHBACK:"+cashBack+" ";
					break;
				case 3:
					tranText+="/TRANSACTIONTYPE:DEBITRETURN /LOCKTRANTYPE /LOCKAMOUNT ";
					break;
				case 4:
					tranText+="/TRANSACTIONTYPE:FORCE /LOCKTRANTYPE /LOCKAMOUNT ";
					break;
				case 5:
					tranText+="/TRANSACTIONTYPE:PREAUTH /LOCKTRANTYPE /LOCKAMOUNT ";
					if(hasXToken) {
						tranText+="/XCACCOUNTID:"+creditCard.XChargeToken+" ";
						tranText+="/AUTOPROCESS ";
						tranText+="/GETXCACCOUNTIDSTATUS ";
					}
					if(isNotRecurring) {
						tranText+="/ACCOUNT:"+creditCard.CCNumberMasked+" ";
						tranText+="/AUTOPROCESS ";
					}
					break;
				case 6:
					tranText+="/TRANSACTIONTYPE:ADJUSTMENT /LOCKTRANTYPE ";//excluding /LOCKAMOUNT, amount must be editable in X-Charge to make an adjustment
					string adjustTransactionID="";
					string[] stringArrayNoteSplit=Regex.Split(textNote.Text,"\r\n");
					List<string> listNoteSplits=stringArrayNoteSplit.ToList();
					for(int i=0;i<listNoteSplits.Count;i++) {
						if(listNoteSplits[i].StartsWith("XCTRANSACTIONID=")) {
							adjustTransactionID=listNoteSplits[i].Substring(16);
						}
					}
					if(adjustTransactionID!="") {
						tranText+="/XCTRANSACTIONID:"+adjustTransactionID+" ";
						tranText+="/AUTOPROCESS ";
					}
					break;
				case 7:
					tranText+="/TRANSACTIONTYPE:VOID /LOCKTRANTYPE /LOCKAMOUNT ";
					break;
			}
			if(_doPromptSignature) {
				tranText+="/PROMPTSIGNATURE:T /SAVESIGNATURE:T ";
			}
			else {
				tranText+="/PROMPTSIGNATURE:F ";
			}
			tranText+="/RECEIPTINRESULT ";//So that we can make a few changes to the receipt ourselves
			return tranText;
		}

		///<summary>Prints receipt, adds splits, etc. Closes the current window.</summary>
		private void HandleVoidPayment(string payNote,double approvedAmt,string receipt,CreditCardSource creditCardSource) {
			if(IsNew) {
				if(!_wasCreditCardSuccessful) {
					textAmount.Text="-"+approvedAmt.ToString("F");
					textNote.Text+=payNote;
				}
				_payment.Receipt=receipt;
				if(_doPrintReceipt && receipt!="") {
					PrintReceipt(receipt,Lan.g(this, creditCardSource.In(CreditCardSource.EdgeExpressRCM,CreditCardSource.EdgeExpressCNP) 
						? "EdgeExpress receipt printed" : "X-Charge receipt printed"));
					_doPrintReceipt=false;
				}
				if(SavePaymentToDb()) {
					DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
				}
				return;
			}
			XchargeMilestone="Create Negative Payment";
			if(!IsNew || _wasCreditCardSuccessful) {//Create a new negative payment if the void is being run from an existing payment
				if(_listPaySplits.Count==0) {
					AddOneSplit();
					Reinitialize();
				}
				else if(_listPaySplits.Count==1//if one split
					&& _listPaySplits[0].PayPlanNum!=0//and split is on a payment plan
					&& _listPaySplits[0].SplitAmt!=_payment.PayAmt)//and amount doesn't match payment
				{
					_listPaySplits[0].SplitAmt=_payment.PayAmt;//make amounts match automatically
					textSplitTotal.Text=textAmount.Text;
				}
				_payment.IsSplit=_listPaySplits.Count>1;
				Payments.InsertVoidPayment(_payment,_listPaySplits,receipt,payNote,creditCardSource);
				if(_doPrintReceipt && receipt!="") {
					PrintReceipt(receipt,Lan.g(this,creditCardSource==CreditCardSource.EdgeExpressRCM ? "EdgeExpress receipt printed" : "X-Charge receipt printed"));
				}
				string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patient.PatNum,_listPaySplits);
				if(!string.IsNullOrEmpty(strErrorMsg)) {
					MessageBox.Show(strErrorMsg);
				}
			}
			DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
		}

		///<summary>Only for when the user hits Cancel and causes the previous payment to be voided.</summary>
		private void HandleVoidPaymentForFormClosing(string payNote,string receipt,bool showApprovedAmtNotice,double approvedAmt,
			CreditCardSource creditCardSource) 
		{
			Payment paymentVoid=Payments.InsertVoidPayment(_payment,_listPaySplits,receipt,payNote,creditCardSource);
			if(showApprovedAmtNotice) {
				MessageBox.Show(Lan.g(this,"The amount of the original transaction")+": "+_payment.PayAmt.ToString("C")+"\r\n"+Lan.g(this,"does not match "
					+"the approved amount returned")+": "+approvedAmt.ToString("C")+".\r\n"+Lan.g(this,"The amount will be changed to reflect the approved "
					+"amount charged."),"Alert",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				paymentVoid.PayAmt=approvedAmt;
			}
			if(_doPrintReceipt && receipt!="") {
				PrintReceipt(receipt,Lan.g(this,creditCardSource==CreditCardSource.EdgeExpressRCM ? "EdgeExpress receipt printed" : "X-Charge receipt printed"));
			}
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,paymentVoid.PatNum,Patients.GetLim(paymentVoid.PatNum).GetNameLF()+", "
				+paymentVoid.PayAmt.ToString("c"));
		}

		///<summary>Returns true if payconnect is enabled and completely setup.</summary>
		private bool HasPayConnect() {
			_listDefsPaymentType=_listDefsPaymentType??Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			Program program=Programs.GetCur(ProgramName.PayConnect);
			bool isSetupRequired=false;
			if(program.Enabled) {
				//If clinics are disabled, _paymentCur.ClinicNum will be 0 and the Username and Password will be the 'Headquarters' or practice credentials
				string paymentType=ProgramProperties.GetPropVal(program.ProgramNum,"PaymentType",_payment.ClinicNum);
				string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(program.ProgramNum,"Password",_payment.ClinicNum));
				if(string.IsNullOrEmpty(ProgramProperties.GetPropVal(program.ProgramNum,"Username",_payment.ClinicNum))
					|| string.IsNullOrEmpty(password)
					|| !_listDefsPaymentType.Any(x => x.DefNum.ToString()==paymentType)) 
				{
					isSetupRequired=true;
				}
			}
			else {//Program link not enabled.  Launch a promo site.
				ODException.SwallowAnyException(() =>
					Process.Start("http://www.opendental.com/resources/redirects/redirectpayconnect.html")
				);
				return false;
			}
			if(isSetupRequired) {
				if(!Security.IsAuthorized(Permissions.Setup)) {
					return false;
				}
				using FormPayConnectSetup formPayConnectSetup=new FormPayConnectSetup();
				formPayConnectSetup.ShowDialog();
				if(formPayConnectSetup.DialogResult!=DialogResult.OK) {
					return false;
				}
				//The user could have corrected the PayConnect bridge, recursively try again.
				return HasPayConnect();
			}
			return true;
		}

		///<summary>Returns true if PaySimple is enabled and completely setup.</summary>
		private bool HasPaySimple() {
			_listDefsPaymentType=_listDefsPaymentType??Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			Program program=Programs.GetCur(ProgramName.PaySimple);
			bool isSetupRequired=false;
			if(program.Enabled) {
				//If clinics are disabled, _paymentCur.ClinicNum will be 0 and the Username and Key will be the 'Headquarters' or practice credentials
				string paymentType=ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,PaySimple.PropertyDescs.PaySimplePayTypeCC,_payment.ClinicNum);
				if(string.IsNullOrEmpty(ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiUserName,_payment.ClinicNum))
					|| string.IsNullOrEmpty(ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiKey,_payment.ClinicNum))
					|| !_listDefsPaymentType.Any(x => x.DefNum.ToString()==paymentType)) 
				{
					isSetupRequired=true;
				}
			}
			else {//Program link not enabled.  Launch a promo website.
				ODException.SwallowAnyException(() =>
					Process.Start("http://www.opendental.com/resources/redirects/redirectpaysimple.html")//
				);
				return false;
			}
			if(isSetupRequired) {
				if(!Security.IsAuthorized(Permissions.Setup)) {
					return false;
				}
				using FormPaySimpleSetup formPaySimpleSetup=new FormPaySimpleSetup();
				formPaySimpleSetup.ShowDialog();
				if(formPaySimpleSetup.DialogResult!=DialogResult.OK) {
					return false;
				}
				//The user could have corrected the PaySimple bridge, recursively try again.
				return HasPaySimple();
			}
			return true;
		}

		private bool HasXCharge() {
			_listDefsPaymentType=_listDefsPaymentType??Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			if(_programX==null) {
				MsgBox.Show(this,"X-Charge entry is missing from the database.");//should never happen
				return false;
			}
			bool isSetupRequired=false;
			//if X-Charge is enabled, but the Username or Password are blank or the PaymentType is not a valid DefNum, setup is required
			if(_programX.Enabled) {
				//X-Charge is enabled if the username and password are set and the PaymentType is a valid DefNum
				//If clinics are disabled, _paymentCur.ClinicNum will be 0 and the Username and Password will be the 'Headquarters' or practice credentials
				string paymentType=ProgramProperties.GetPropVal(_programX.ProgramNum,"PaymentType",_payment.ClinicNum);
				if(string.IsNullOrEmpty(ProgramProperties.GetPropVal(_programX.ProgramNum,"Username",_payment.ClinicNum))
					|| string.IsNullOrEmpty(ProgramProperties.GetPropVal(_programX.ProgramNum,"Password",_payment.ClinicNum))
					|| !_listDefsPaymentType.Any(x => x.DefNum.ToString()==paymentType))
				{
					isSetupRequired=true;
				}
			}
			else {//Program link not enabled.  Launch a promo site.
				ODException.SwallowAnyException(() =>
					Process.Start("http://www.opendental.com/resources/redirects/redirectopenedge.html")
				);
				return false;
			}
			//if X-Charge is enabled and the Username and Password is set and the PaymentType is a valid DefNum,
			//make sure the path (either local override or program path) is valid
			if(!isSetupRequired && !File.Exists(_xPath)) {
				MsgBox.Show(this,"Path is not valid.");
				isSetupRequired=true;
			}
			//if setup is required and the user is authorized for setup, load the X-Charge setup form, but return false so the validation can happen again
			if(isSetupRequired && Security.IsAuthorized(Permissions.Setup)) {
				using FormXchargeSetup formXchargeSetup=new FormXchargeSetup();
				formXchargeSetup.ShowDialog();
				CheckUIState();//user may have made a change in setup that affects the state of the UI, e.g. X-Charge is no longer enabled for this clinic
				return false;
			}
			return true;
		}

		private bool HasEdgeExpress() {
			_listDefsPaymentType=_listDefsPaymentType??Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			Program program=Programs.GetCur(ProgramName.EdgeExpress);
			if(!program.Enabled) {
				ODException.SwallowAnyException(() =>
					Process.Start("http://www.opendental.com/resources/redirects/redirectopenedge.html"));
				return false;
			}
			return true;
		}

		///<summary>Highlights the charges that corresponds to the selected paysplit.</summary>
		private void HighlightChargesForSplits() {
			gridCharges.SetAll(false);
			gridTreatPlan.SetAll(false);
			List<PaySplit> listPaySplits=gridSplits.SelectedTags<PaySplit>();
			for(int i=0;i<listPaySplits.Count;i++) {
				PaySplitHelper paySplitHelper=new PaySplitHelper(listPaySplits[i]);
				if(gridCharges.AllowSelection && _dictGridChargesPaySplitIndices.TryGetValue(paySplitHelper,out List<int> listChargeIndices)) {
					for(int j=0;j<listChargeIndices.Count;j++) {
						gridCharges.SetSelected(listChargeIndices[j],true);
					}
				}
				if(_dictGridTreatPlanPaySplitIndices.TryGetValue(paySplitHelper,out List<int> listTreatPlanIndices)) {
					for(int j=0;j<listTreatPlanIndices.Count;j++) {
						gridTreatPlan.SetSelected(listTreatPlanIndices[j],true);
					}
				}
			}
		}

		///<summary>Performs all of the Load functionality.</summary>
		private void Init(bool doAutoSplit=false,bool doSelectAllSplits=false,bool doPayFirstAcctEntries=false,bool doPreserveValues=false) {
			_isInit=true;
			List<AccountEntry> listAccountEntriesPayFirst=new List<AccountEntry>();
			if(doPayFirstAcctEntries && ListAccountEntriesPayFirst!=null) {
				listAccountEntriesPayFirst=ListAccountEntriesPayFirst;
			}
			bool doShowExplicitCreditsOnly=checkIncludeExplicitCreditsOnly.Checked && checkIncludeExplicitCreditsOnly.Enabled;
			PaymentEdit.InitData initData=PaymentEdit.Init(_loadData,listAccountEntriesPayFirst,_dictPatients,checkPayTypeNone.Checked,_doPreferCurrentPat,
				doAutoSplit,doShowExplicitCreditsOnly);
			textSplitTotal.Text=initData.SplitTotal.ToString("f");
			_dictPatients=initData.DictPats;
			//Get data from constructing charges list, linking credits, and auto splitting.
			_listPaySplits=initData.AutoSplitData.ListPaySplitsForPayment;
			_listAccountEntriesCharges=initData.AutoSplitData.ListAccountEntries;
			FillFilters(doPreserveValues);
			FillGridSplits();
			FillGridCharges();
			FillGridTreatPlan();
			if(doSelectAllSplits) {
				gridSplits.SetAll(true);
				HighlightChargesForSplits();
			}
			UpdateChargeTotalWithSelectedEntries();
			_isInit=false;
		}

		private bool IsCareCreditTransStatusCompleted(CareCreditWebResponse careCreditWebResponse) {
			if(careCreditWebResponse==null) {
				return false;
			}
			if(careCreditWebResponse.ProcessingStatus==CareCreditWebStatus.Completed) {
				return true;
			}
			//Call pullback one more time;
			CareCredit.UpdateWebResponse(careCreditWebResponse);
			return careCreditWebResponse.ProcessingStatus==CareCreditWebStatus.Completed;
		}

		///<summary>Processes a credit card transaction using EdgeExpress.</summary>
		private string MakeEdgeExpressTransactionRCM(EdgeExpressTransType edgeExpressTransType,double amt,bool doCreateToken,
			string aliasToken,string transactionId,decimal cashBackAmt,CreditCard creditCard)
		{
			string payNote="";
			EdgeExpress.RcmResponse rcmResponse;
			try {
				rcmResponse=EdgeExpress.RCM.SendEdgeExpressRequest(_patient,_payment.ClinicNum,edgeExpressTransType,false,amt,
					_doPromptSignature,doCreateToken,aliasToken,transactionId,cashBackAmt);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Error processing card:")+" "+ex.Message+"\r\n\r\n"+Lans.g(this,"Please verify that RCM and pin-pad device are properly installed"
					+" on this computer, then try again."),ex);
				return payNote;
			}
			if(creditCard==null && doCreateToken) {
				if(!string.IsNullOrEmpty(rcmResponse.ALIAS)) {
					creditCard=CreditCards.CreateNewOpenEdgeCard(_patient.PatNum,_payment.ClinicNum,rcmResponse.ALIAS,rcmResponse.EXPMONTH,rcmResponse.EXPYEAR,
						rcmResponse.ACCOUNT,CreditCardSource.EdgeExpressRCM);
				}
				else if(_wasCreditCardSuccessful) {
					MsgBox.Show(this,"EdgeExpress didn't return a token so credit card information couldn't be saved.");
				}
			}
			double approvedAmt=PIn.Double(rcmResponse.APPROVEDAMOUNT);
			if(CompareDouble.IsGreaterThan(approvedAmt,0) && !CompareDouble.IsEqual(approvedAmt,amt) && !edgeExpressTransType.In(EdgeExpressTransType.CreditVoid,EdgeExpressTransType.CreditReturn)) {
				MessageBox.Show(Lan.g(this,"The amount you typed in")+": "+amt.ToString("C")+"\r\n"+Lan.g(this,"does not match the approved amount returned")
					+": "+approvedAmt.ToString("C")+".\r\n"+Lan.g(this,"The amount will be changed to reflect the approved amount charged."),"Alert",
					MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				textAmount.Text=approvedAmt.ToString("F");
			}
			string receipt=rcmResponse.RECEIPTTEXT;
			payNote=rcmResponse.GetPayNote();
			if(edgeExpressTransType==EdgeExpressTransType.CreditReturn && CompareDouble.IsGreaterThan(approvedAmt,0)) {
				textAmount.Text="-"+approvedAmt.ToString("F");
			}
			else if(edgeExpressTransType==EdgeExpressTransType.CreditVoid) {
				HandleVoidPayment(payNote,approvedAmt,receipt,CreditCardSource.EdgeExpressRCM);
				return payNote;
			}
			_wasCreditCardSuccessful=rcmResponse.IsSuccess;
			_isCCDeclined=rcmResponse.RESULT!="0";
			textNote.AppendText(rcmResponse.GetPayNote());
			_payment.Receipt=receipt;
			_payment.IsCcCompleted=_wasCreditCardSuccessful;
			if(!string.IsNullOrEmpty(receipt)) {
				butPrintReceipt.Visible=true;
				if(PrefC.GetBool(PrefName.AllowEmailCCReceipt)) {
					butEmailReceipt.Visible=true;
				}
				if(_doPrintReceipt) {
					PrintReceipt(receipt,Lan.g(this,"EdgeExpress receipt printed"));
				}
			}
			if(creditCard!=null && !string.IsNullOrEmpty(creditCard.XChargeToken) && creditCard.CCExpiration!=null) {
				//Refresh comboCreditCards and select the index of the card used for this payment if the token was saved
				List<CreditCard> listCreditCards=CreditCards.Refresh(_patient.PatNum);
				AddCreditCardsToCombo(listCreditCards,x => x.XChargeToken==creditCard.XChargeToken
					&& x.CCExpiration.Year==creditCard.CCExpiration.Year
					&& x.CCExpiration.Month==creditCard.CCExpiration.Month);
			}
			return payNote;
		}

		private string MakeEdgeExpressTransactionCNP(EdgeExpressTransType edgeExpressTransType,double amt,bool doCreateToken,
			string aliasToken,string transactionId, double prepaidAmount=0,long clinicNum=-1)
		{
			XWebResponse xWebResponse;
			XWebResponse xWebResponseProcessed;
			string payNote="";
			switch(edgeExpressTransType) {
				case EdgeExpressTransType.CreditSale:
					xWebResponse=EdgeExpress.CNP.GetUrlForPaymentPage(_patient.PatNum,textNote.Text,amt,
						doCreateToken,CreditCardSource.EdgeExpressCNP,false,aliasToken,paymentNum:_payment.PayNum,clinicNum:clinicNum);
					using(FormWebBrowser formWebBrowser=new FormWebBrowser(xWebResponse.HpfUrl)) {//Braces required within switch statements.
						formWebBrowser.ShowDialog();
					}
					xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(xWebResponse,_payment);
					if(xWebResponseProcessed.TransactionStatus==XWebTransactionStatus.EdgeExpressPending){
						return null; //FormWebBrowser closed early
					}
					payNote=xWebResponseProcessed.GetFormattedNote(true,false);
					_payment.PaymentSource=xWebResponseProcessed.CCSource;
					if(xWebResponseProcessed.TransactionStatus==XWebTransactionStatus.EdgeExpressCompletePaymentApproved) {
						_wasCreditCardSuccessful=true;//void payment on cancel.
					}
					_xWebResponse=xWebResponseProcessed;
					_payment.Receipt=EdgeExpress.CNP.BuildReceiptString(xWebResponseProcessed,false);
					if(xWebResponseProcessed.XWebResponseCode==XWebResponseCodes.Approval) {
						_payment.IsCcCompleted=true;
						textNote.Text+=payNote;
						if(_doPrintReceipt) {
							PrintReceipt(_payment.Receipt,Lan.g(this,"EdgeExpress receipt printed"));
						}
					}
					break;
				case EdgeExpressTransType.CreditAuth:
					xWebResponse=EdgeExpress.CNP.GetUrlForCreditCardAlias(_patient.PatNum,CreditCardSource.EdgeExpressCNP,false,amt,doCreateToken);
					using(FormWebBrowser formWebBrowser=new FormWebBrowser(xWebResponse.HpfUrl)) {//Braces required within switch statements.
						formWebBrowser.ShowDialog();
					}
					xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(xWebResponse,_payment);
					if(xWebResponseProcessed.TransactionStatus==XWebTransactionStatus.EdgeExpressPending) {
						return null; //FormWebBrowser closed early
					}
					payNote=xWebResponseProcessed.GetFormattedNote(true,false);
					_payment.Receipt=EdgeExpress.CNP.BuildReceiptString(xWebResponseProcessed,false);
					if(xWebResponseProcessed.XWebResponseCode==XWebResponseCodes.Approval) {// only print receipt if its an approved transaction
						_payment.IsCcCompleted=true;
						textNote.Text+=payNote;
						if(_doPrintReceipt) {
							PrintReceipt(_payment.Receipt,Lan.g(this,"EdgeExpress receipt printed"));
						}
					}
					break;
				case EdgeExpressTransType.CreditReturn:
					xWebResponse=EdgeExpress.CNP.ReturnTransaction(_patient.PatNum,transactionId,amt,false);
					xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(xWebResponse,_payment);
					payNote=xWebResponseProcessed.GetFormattedNote(false,false);
					_xWebResponse=xWebResponseProcessed;
					_payment.Receipt=EdgeExpress.CNP.BuildReceiptString(xWebResponseProcessed,false);
					if(xWebResponseProcessed.XWebResponseCode==XWebResponseCodes.Approval) {// only print receipt if its an approved transaction
						_payment.IsCcCompleted=true;
						textNote.Text+=payNote;
						if(_doPrintReceipt) {
							PrintReceipt(_payment.Receipt,Lan.g(this,"EdgeExpress receipt printed"));
						}
					}
					break;
				case EdgeExpressTransType.CreditVoid:
					xWebResponse=EdgeExpress.CNP.VoidTransaction(_patient.PatNum,transactionId,amt,false);
					payNote=xWebResponse.GetFormattedNote(false);
					if(xWebResponse.XWebResponseCode==XWebResponseCodes.Approval) {// only continue if we got a approval code back from Edge Express
						//This matches what we do for PaySimple. We return early for transactions from the FormClainPayEdit.cs window to prevent an error in HandleVoidPayment.
						if(prepaidAmount!=0) {
							return payNote;
						}
						_payment.IsCcCompleted=true;
						textNote.Text+=payNote;
						HandleVoidPayment(xWebResponse.GetFormattedNote(false,true),xWebResponse.Amount,EdgeExpress.CNP.BuildReceiptString(xWebResponse,false),CreditCardSource.EdgeExpressCNP);
					}
					//Not an approval response and also not an insurance payment.
					else if(prepaidAmount==0) {
						textNote.Text=xWebResponse.GetFormattedNote(false,true,true);
					}
					break;
				case EdgeExpressTransType.CreditOnlineCapture://Force
					xWebResponse=EdgeExpress.CNP.ForceTransaction(_patient.PatNum,transactionId,amt,false);
					xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(xWebResponse,_payment);
					payNote=xWebResponseProcessed.GetFormattedNote(false,false);
					_xWebResponse=xWebResponseProcessed;
					_payment.Receipt=EdgeExpress.CNP.BuildReceiptString(xWebResponseProcessed,false);
					_payment.PaymentSource=xWebResponseProcessed.CCSource;
					if(xWebResponseProcessed.TransactionStatus==XWebTransactionStatus.EdgeExpressCompletePaymentApproved) {
						_wasCreditCardSuccessful=true;//void payment on cancel.
					}
					if(xWebResponseProcessed.XWebResponseCode==XWebResponseCodes.Approval) {// only print receipt if its an approved transaction
						_payment.IsCcCompleted=true;
						textNote.Text+=payNote;
						if(_doPrintReceipt) {
							PrintReceipt(_payment.Receipt,Lan.g(this,"EdgeExpress receipt printed"));
						}
					}
					break;
			}
			return payNote;
		}

		private void PayConnectReturn() {
			string refNum=_payConnectResponseWeb.RefNumber;
			if(refNum.IsNullOrEmpty()) {
				MsgBox.Show(this,"Missing PayConnect Reference Number. This return cannot be processed.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(!PayConnectL.VoidOrRefundPayConnectPortalTransaction(_payConnectResponseWeb,_payment,PayConnectService.transType.RETURN,refNum,(decimal)_payment.PayAmt)) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Return successful.");
		}

		private void PayConnectVoid() {
			string refNum=_payConnectResponseWeb.RefNumber;
			double amountCharged=PIn.Double(textAmount.Text);
			if(amountCharged>0) {
				amountCharged*=-1;
			}
			if(refNum.IsNullOrEmpty()) {
				MsgBox.Show(this,"Missing PayConnect Reference Number. This void cannot be processed.");
				butVoid.Enabled=false;
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(!PayConnectL.VoidOrRefundPayConnectPortalTransaction(_payConnectResponseWeb,_payment,PayConnectService.transType.VOID,refNum,(decimal)amountCharged)) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Void successful.");
		}

		private void PrintReceipt(string receiptStr,string strAuditDescription) {//TODO: Implement ODprintout pattern - MigraDoc
			MigraDocPrintDocument migraDocPrintDocument=new MigraDocPrintDocument(new DocumentRenderer(CreatePDFDoc(receiptStr)));
			migraDocPrintDocument.Renderer.PrepareDocument();
			if(ODBuild.IsDebug()) {
				using FormRpPrintPreview formRpPrintPreview=new FormRpPrintPreview(migraDocPrintDocument);
				formRpPrintPreview.ShowDialog();
				return;
			}
			if(!PrinterL.SetPrinter(_pd2,PrintSituation.Receipt,_patient.PatNum,strAuditDescription)) {
				return;
			}
			migraDocPrintDocument.PrinterSettings=_pd2.PrinterSettings;
			try {
				migraDocPrintDocument.Print();
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to print receipt")+". "+ex.Message);
			}
		}

		private void Reinitialize(bool doRefreshConstructData=false,bool doSelectAllSplits=false) {
			List<long> listPatNumsFamily=_family.GetPatNums();
			if(_patient.SuperFamily > 0 && checkShowSuperfamily.Checked) {
				listPatNumsFamily=listPatNumsFamily.Union(_loadData.SuperFam.GetPatNums()).ToList();
			}
			//Preserve all PaySplits that are not part of this current payment.
			_loadData.ListSplits.RemoveAll(x => x.PayNum==_payment.PayNum);
			//Add back all PaySplits showing to the user.  Keep in mind that they may have deleted some splits or even added new ones (SplitNum=0).
			_loadData.ListSplits.AddRange(_listPaySplits);
			if(doRefreshConstructData) {
				_loadData.ConstructChargesData=PaymentEdit.GetConstructChargesData(listPatNumsFamily,_patient.PatNum,_listPaySplits,_payment.PayNum,
					checkPayTypeNone.Checked);
			}
			Init(doSelectAllSplits:doSelectAllSplits,doPreserveValues:true);
		}

		/// <summary>Helper for EdgeExpress.CleanString()</summary>
		private string CleanString(string str) {
			return EdgeExpress.CleanString(str);
		}

		///<summary>Checks if the dynamic payment plan has any charges with overpaid interest or principal. If it does, prompts the user to balance on prepay, principal, or return to payment page. Returns false if the user wants to stay in the Payment window.</summary>
		private bool CheckDynamicPaymentPlanRebalance() {
			if(!PayPlanEdit.AreAnyPayPlansOverpaid(_listPaySplits)) {
				return true;
			}
			DialogResult dialogResult=MessageBox.Show(Lan.g(this,"One or more Current Payment Splits are overpaying interest or principal for dynamic payment plan charges."
				+"\r\n\r\nDo you want to re-apply the overpayment to principal?"
				+"\r\n\r\nYes pays on principal, No makes a prepayment, and Cancel returns to the Payment window."),Lan.g(this,"Payment Plan Overpayment Detected"),MessageBoxButtons.YesNoCancel);
			if(dialogResult==DialogResult.Cancel) {
				return false;
			}
			bool isPayOnPrincipal=(dialogResult==DialogResult.Yes);
			List<PayPlanEdit.PayPlanRecalculationData> listPayPlanRecalculationDatas=PayPlanEdit.GetRecalculationDataForDynamicPaymentPlans(_patient,_listPaySplits);
			PayPlanEdit.CreateTransferForDynamicPaymentPlans(listPayPlanRecalculationDatas,isPayOnPrincipal);
			return true;
		}

		private bool SavePaymentToDb() {
			if(!textDate.IsValid() || !textAmount.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(PIn.Date(textDate.Text).Date > DateTime.Today.Date
					&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed) && !PrefC.GetBool(PrefName.AccountAllowFutureDebits))
			{
				MsgBox.Show(this,"Payment date cannot be in the future.");
				return false;
			}
			if(checkPayTypeNone.Checked) {
				if(PIn.Double(textAmount.Text)!=0) {
					MsgBox.Show(this,"Amount must be zero for a transfer.");
					return false;
				}
			}
			else {
				double amt=PIn.Double(textAmount.Text);
				if(amt==0 && _listPaySplits.Count==0) {
					MessageBox.Show(Lan.g(this,"Please enter an amount or create payment splits."));
					return false;
				}
				if(amt!=0 && (listPayType.SelectedIndex==-1 || listPayType.SelectedIndex>=_listDefsPaymentType.Count)) {
					MsgBox.Show(this,"A payment type must be selected.");
					return false;
				}
			}
			if(_rigorousAccounting==RigorousAccounting.EnforceFully) {
				if(_listPaySplits.Any(x => x.ProcNum==0 && x.UnearnedType==0 && x.AdjNum==0 && x.PayPlanChargeNum==0)) {//if no procs, no adjust, not an unearned type, and not a payment plan.
					MsgBox.Show(this,"A procedure, adjustment, unearned type, or payment plan must be selected for each of the payment splits.");
					return false;
				}
			}
			List<long> listHiddenUnearnedTypes=PaySplits.GetHiddenUnearnedDefNums();
			double unearnedCur=_listPaySplits.FindAll(x => x.UnearnedType>0 && !listHiddenUnearnedTypes.Contains(x.UnearnedType)).Sum(x => x.SplitAmt);
			double unearnedTotal=(double)PaySplits.GetTotalAmountOfUnearnedForPats(_family.GetPatNums(),payNumExcluded:_payment.PayNum);
			if(CompareDouble.IsLessThan(unearnedCur,0)
				&& !(CompareDecimal.IsGreaterThanOrEqualToZero(unearnedCur+unearnedTotal))
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"You are attempting to create a negative unearned amount. Continue?"))
			{
				return false;
			}
			string paymentTypeParam;
			//Check to see if this payment will transfer money away from an account entry that still wants money when making an income transfer.
			if((PIn.Double(textAmount.Text)==0 && listPayType.SelectedIndex==-1) || checkPayTypeNone.Checked) {
				//Income transfers will not have a payment type selected (they aren't really payments) so an empty string should be passed to plug-ins. 
				//Older versions would actually pass along the text of the last selected item from the Payment Type list box but that is misleading.
				paymentTypeParam="";
				Dictionary<long,List<PaySplit>> dictProcNegSplits=_listPaySplits.FindAll(x => x.ProcNum > 0)
					.GroupBy(x => x.ProcNum)
					.ToDictionary(x => x.Key,x => x.ToList());
				List<long> listWarnForProcNums=new List<long>();
				foreach(long procNum in dictProcNegSplits.Keys) {
					//Check to see if the procedure is having money taken from it.
					if(CompareDecimal.IsGreaterThanOrEqualToZero(dictProcNegSplits[procNum].Sum(x => x.SplitAmt))) {
						continue;
					}
					//At this point, value is being taken away from the procedure in question.
					//Find the Account Entry for the procNum in question and see if the procedure still wants value added to it.
					AccountEntry accountEntryProc=_listAccountEntriesCharges.FirstOrDefault(x => x.GetType()==typeof(Procedure) && x.ProcNum==procNum);
					if(accountEntryProc==null || CompareDecimal.IsLessThanOrEqualToZero(accountEntryProc.AmountEnd)) {
						continue;//Only warn the user when the procedure has a positive AmountEnd.
					}
					listWarnForProcNums.Add(procNum);
				}
				//Warn the user if they are about to transfer money away from a procedure that still has a value.
				if(listWarnForProcNums.Count > 0) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You are attempting to transfer value away from a procedure with a remaining amount. Continue?")) {
						return false;
					}
				}
			}
			else {//Not an income transfer
				paymentTypeParam=_listDefsPaymentType[listPayType.SelectedIndex].ItemName;
			}
			object[] objectArrayParameters={ paymentTypeParam,textNote.Text,_isCCDeclined,_payment };
			Plugins.HookAddCode(this,"FormPayment.SavePaymentToDb_afterUnearnedCurCheck",objectArrayParameters);
			textNote.Text=(string)objectArrayParameters[1];
			_isCCDeclined=(bool)objectArrayParameters[2];
			if(_isCCDeclined) {
				textAmount.Text=0.ToString("f");//So that a declined transaction does not affect account balance
				_listPaySplits.ForEach(x => x.SplitAmt=0);
				textSplitTotal.Text=0.ToString("f");
			}
			if(IsNew) {
				//prevents backdating of initial payment
				if(!Security.IsAuthorized(Permissions.PaymentCreate,PIn.Date(textDate.Text))) {
					return false;
				}
			}
			else {
				//Editing an old entry will already be blocked if the date was too old, and user will not be able to click OK button
				//This catches it if user changed the date to be older. If user has SplitCreatePastLockDate permission and has not changed the date, then
				//it is okay to save the payment.
				if((!Security.IsAuthorized(Permissions.SplitCreatePastLockDate,true)
					|| _paymentOld.PayDate!=PIn.Date(textDate.Text))
					&& !Security.IsAuthorized(Permissions.PaymentEdit,PIn.Date(textDate.Text))) {
					return false;
				}
			}
			bool isAccountingSynchRequired=false;
			double accountingOldAmt=_paymentOld.PayAmt;
			long accountingNewAcct=-1;//the old acctNum will be retrieved inside the validation code.
			if(textDepositAccount.Visible) {//Not visable when IsNew or _loadData.Transaction is null or if listPayType is clicked.
				accountingNewAcct=-1;//indicates no change
			}
			else if(comboDepositAccount.Visible && comboDepositAccount.Items.Count>0 && comboDepositAccount.SelectedIndex!=-1) {
				//comboDepositAccount is set invisible when IsNew is false 
				//or if listPayType.SelectedIndex==-1 || checkPayTypeNone.Checked and IsNew and PrefName.PaymentClinicSetting is PayClinicSetting.PatientDefaultClinic
				//or if AccountingAutoPay can not be found based on listPayType.SelectedIndex.
				accountingNewAcct=_acctNumArrayDeposits[comboDepositAccount.SelectedIndex];
			}
			else {//neither textbox nor combo visible. Or something's wrong with combobox
				accountingNewAcct=0;
			}
			try {
				isAccountingSynchRequired=Payments.ValidateLinkedEntries(accountingOldAmt,PIn.Double(textAmount.Text),IsNew,
					_payment.PayNum,accountingNewAcct);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);//not able to alter, so must not allow user to continue.
				return false;
			}
			if(_payment.ProcessStatus!=ProcessStat.OfficeProcessed) {
				if(checkProcessed.Checked) {
					_payment.ProcessStatus=ProcessStat.OnlineProcessed;
				}
				else {
					_payment.ProcessStatus=ProcessStat.OnlinePending;
				}
			}
			_payment.PayAmt=PIn.Double(textAmount.Text);//handles blank
			_payment.PayDate=PIn.Date(textDate.Text);
			_payment.CheckNum=textCheckNum.Text;
			_payment.BankBranch=textBankBranch.Text;
			_payment.PayNote=textNote.Text;
			_payment.IsRecurringCC=checkRecurring.Checked;
			if((PIn.Double(textAmount.Text)==0 && listPayType.SelectedIndex==-1) || checkPayTypeNone.Checked) {
				_payment.PayType=0;
			}
			else {
				_payment.PayType=_listDefsPaymentType[listPayType.SelectedIndex].DefNum;
			}
			if(_listPaySplits.Count==0) {//Existing payment with no splits.
				if(!_isCCDeclined && _rigorousAccounting!=RigorousAccounting.DontEnforce) {
					_listPaySplits=GetSuggestedAutoSplits();//PayAmt needs to be set first
				}
				else if(!_isCCDeclined
					&& Payments.AllocationRequired(_payment.PayAmt,_payment.PatNum)
					&& GetFamilyOrSuperFamily().ListPats.Length>1 //Has other family members
					&& MsgBox.Show(this,MsgBoxButtons.YesNo,"Apply part of payment to other family members?"))
				{
					_listPaySplits=Payments.Allocate(_payment);//PayAmt needs to be set first
				}
				else {//Either no allocation required, or user does not want to allocate.  Just add one split.
					if(checkPayTypeNone.Checked) {//No splits created and it's an income transfer.  Delete payment? (it's not a useful payment)
						Payments.Delete(_payment);
						return true;
					}
					else {
						if(!AddOneSplit(true)) {
							return false;
						}
					}
				}
				if(_listPaySplits.Count==0) {//There's still no split.
					if(!AddOneSplit(true)) {
						return false;
					}
				}
			}
			else {//A new or existing payment with splits.
				if(_listPaySplits.Count==1//if one split
					&& _listPaySplits[0].PayPlanNum!=0//and split is on a payment plan
					&& PIn.Double(textAmount.Text) != _listPaySplits[0].SplitAmt)//and amount doesn't match payment
				{
					_listPaySplits[0].SplitAmt=PIn.Double(textAmount.Text);//make amounts match automatically
					textSplitTotal.Text=textAmount.Text;
				}
				if(_payment.PayAmt!=PIn.Double(textSplitTotal.Text)) {
					MsgBox.Show(this,"Split totals must equal payment amount.");
					//work on reallocation schemes here later
					return false;
				}
			}
			if(!CheckDynamicPaymentPlanRebalance()) {
				return false;
			}
			if(IsNew && !_isCCDeclined) {
				//Currently we do not want to modify historical data or credit transaction values. Moving forward zero dollar splits are not valid.
				_listPaySplits.RemoveAll(x => CompareDouble.IsZero(x.SplitAmt));
			}
			//At this point there better be a split in the list of Current Payment Splits.
			//There is no such thing as a payment with no payment splits.  If there is then the DBM 'PaymentMissingPaySplit' needs to be removed.
			if(_listPaySplits.Count==0) {
				MsgBox.Show(this,"Please create payment splits.");
				return false;
			}
			if(_listPaySplits.Count>1) {
				_payment.IsSplit=true;
			}
			else {
				_payment.IsSplit=false;
			}
			try {
				Payments.Update(_payment,true);
			}
			catch(ApplicationException ex) {//this catches bad dates.
				MessageBox.Show(ex.Message);
				return false;
			}
			//Set all DatePays the same.
			for(int i=0;i<_listPaySplits.Count;i++) {
				_listPaySplits[i].DatePay=_payment.PayDate;
			}
			bool hasChanged=PaySplits.Sync(_listPaySplits,_listPaySplitsOld);
			for(int i=0;i<_listPaySplitsForSecLog.Count;i++) {
				//Split was deleted. Add Securitylog Entry
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_listPaySplitsForSecLog[i].PatNum,PaySplits.GetSecurityLogMsgDelete(_listPaySplitsForSecLog[i],_payment),0,
					_listPaySplitsForSecLog[i].SecDateTEdit);
			}
			//Accounting synch is done here.  All validation was done further up
			//If user is trying to change the amount or linked account of an entry that was already copied and linked to accounting section
			if(isAccountingSynchRequired && !checkPayTypeNone.Checked) {
				Payments.AlterLinkedEntries(accountingOldAmt,_payment.PayAmt,IsNew,_payment.PayNum,accountingNewAcct,_payment.PayDate,
					GetFamilyOrSuperFamily().GetNameInFamFL(_payment.PatNum));
			}
			if(IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,_payment.PatNum,Payments.GetSecuritylogEntryText(_payment,_paymentOld,IsNew,_listDefsPaymentType));
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_payment.PatNum,Payments.GetSecuritylogEntryText(_payment,_paymentOld,IsNew,_listDefsPaymentType),
					0,_paymentOld.SecDateTEdit);
			}
			if(hasChanged) {
				string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patient.PatNum,_listPaySplits.Union(_listPaySplitsOld).ToList());
				if(!string.IsNullOrEmpty(strErrorMsg)) {
					MessageBox.Show(strErrorMsg);
				}
			}
			return true;
		}
		
		private string GetDisplayStringForType(string name) {
			if(name==nameof(FauxAccountEntry)) {
				return Lans.g(this,"PayPlanCharge");
			}
			return name;
		}

		private string SecurityLogEntryHelper(string oldVal,string newVal,string textInLog) {
			if(oldVal!=newVal) {
				return "\r\n"+textInLog+" changed back to '"+oldVal+"' from '"+newVal+"'";
			}
			return "";
		}

		private void SelectPaySplit(PaySplit paySplit) {
			if(paySplit==null) {
				return;
			}
			PaySplitHelper paySplitHelper=new PaySplitHelper(paySplit);
			if(_dictGridSplitsPaySplitIndices.TryGetValue(paySplitHelper,out List<int> listSplitIndices)) {
				for(int i=0;i<listSplitIndices.Count;i++) {
					gridSplits.SetSelected(listSplitIndices[i],true);
				}
			}
		}

		private void SelectPaySplitsForAccountEntries(List<AccountEntry> listAccountEntries) {
			List<PaySplit> listPaySplits=new List<PaySplit>();
			//Select all splits associated to any PayPlanCharge because interest and principal are always treated as one entity.
			List<long> listPayPlanChargeNums=listAccountEntries.Where(x => x.PayPlanChargeNum > 0).Select(x => x.PayPlanChargeNum).ToList();
			if(listPayPlanChargeNums.Count > 0) {
				List<AccountEntry> listPayPlanChargeEntries=_listAccountEntriesCharges.FindAll(x => listPayPlanChargeNums.Contains(x.PayPlanChargeNum));
				listAccountEntries.AddRange(listPayPlanChargeEntries.Except(listAccountEntries));
			}
			for(int i=0;i<listAccountEntries.Count;i++) {
				List<PaySplit> listPaySplitsCollections=listAccountEntries[i].SplitCollection.ToList();
				for(int j=0;j<listPaySplitsCollections.Count;j++) {
					listPaySplits.Add(listPaySplitsCollections[j]);
				}
			}
			for(int i=0;i<listPaySplits.Count;i++) {
				SelectPaySplit(listPaySplits[i]);
			}
		}

		///<summary>Call this method when listPayType changes.</summary>
		private void SetComboDepositAccounts() {
			if(listPayType.SelectedIndex==-1 || checkPayTypeNone.Checked) {
				if(IsNew && (PayClinicSetting)PrefC.GetInt(PrefName.PaymentClinicSetting)==PayClinicSetting.PatientDefaultClinic) {
					labelDepositAccount.Visible=false;
					comboDepositAccount.Visible=false;
				}
				return;
			}
			AccountingAutoPay accountingAutoPay=AccountingAutoPays.GetForPayType(
				_listDefsPaymentType[listPayType.SelectedIndex].DefNum);
			if(accountingAutoPay==null) {
				labelDepositAccount.Visible=false;
				comboDepositAccount.Visible=false;
				return;
			}
			labelDepositAccount.Visible=true;
			comboDepositAccount.Visible=true;
			_acctNumArrayDeposits=AccountingAutoPays.GetPickListAccounts(accountingAutoPay);
			comboDepositAccount.Items.Clear();
			comboDepositAccount.Items.AddList(_acctNumArrayDeposits.Select(x => Accounts.GetDescript(x)).ToArray());
			if(comboDepositAccount.Items.Count>0) {
				comboDepositAccount.SelectedIndex=0;
			}
		}

		private void TabControlCharges_SelectedIndexChanged(object sender,EventArgs e) {
			//There is a weird timing issue where GridOD.vScroll.Visible is not being set to true between tab controls being first initialized and filling grid on load.
			//Thus, on the first FillGridTreatPlan() during load, the vScroll is not taken into account when computing column widths, causing some visual differences.
			//Forcing the grid to re-fill and draw when we change tabs will compute the column widths again, but with vScroll.Visible updated to being true.
			if(tabControlCharges.SelectedTab==tabPageOutstanding) {
				FillGridCharges();
			}
			else {//Treat' Plan
				FillGridTreatPlan();
			}
			UpdateChargeTotalWithSelectedEntries();
		}

		private void ToggleShowHideSplits() {
			if(panelSplits.Visible){
				panelSplits.Visible=false;
				butShowHide.Text=Lan.g(this,"Show Splits");
				Height = LayoutManager.Scale(251+100);//Plus 100 to give room for the buttons
				this.butShowHide.Image = global::OpenDental.Properties.Resources.arrowDownTriangle;
				return;
			}
			panelSplits.Visible=true;
			butShowHide.Text=Lan.g(this,"Hide Splits");
			Height = _originalHeight;
			this.butShowHide.Image = global::OpenDental.Properties.Resources.arrowUpTriangle;
		}

		///<summary>Updates the 'Total' text box that displays underneath the Outstanding Charges and Treat Plan grids with their selected rows. Totals all rows when no rows are selected.</summary>
		private void UpdateChargeTotalWithSelectedEntries() {
			decimal total=0;
			GridOD grid;
			if(tabControlCharges.SelectedTab==tabPageOutstanding) {
				grid=gridCharges;
			}
			else {//Treat' Plan
				grid=gridTreatPlan;
			}
			if(grid.ListGridRows.Count==0) {
				textChargeTotal.Text=total.ToString("f");
				return;
			}
			List<GridRow> listGridRowsSelected=grid.SelectedGridRows;
			if(listGridRowsSelected.IsNullOrEmpty()) {
				listGridRowsSelected=grid.ListGridRows;
			}
			if(listGridRowsSelected.First().Tag is AccountEntry) {
				total=listGridRowsSelected.Sum(x => ((AccountEntry)x.Tag).AmountEnd);
			}
			else if(listGridRowsSelected.First().Tag is List<AccountEntry>) {
				total=listGridRowsSelected.Sum(x => ((List<AccountEntry>)x.Tag).Sum(y => y.AmountEnd));
			}
			textChargeTotal.Text=total.ToString("f");
		}

		///<summary>Returns false if this payment cannot be processed.</summary>
		///<param name="creditCardSelected">The credit card selected in the combo box. Will be null if nothing is selected.</param>
		private bool ValidateForCreditCardPayment(bool isPrepaidCard,out CreditCard creditCardSelected) {
			creditCardSelected=null;
			if(!isPrepaidCard) {//Validation for regular credit cards (not prepaid cards).
				if(textAmount.Text.IsNullOrEmpty()) { // make sure there is an entry here, 0 is valid entry.
					MsgBox.Show(this,"Please enter an amount first.");
					textAmount.Focus();
					return false;
				}
				List<CreditCard> listCreditCards=CreditCards.Refresh(_patient.PatNum);
				if(comboCreditCards.SelectedIndex < listCreditCards.Count && comboCreditCards.SelectedIndex >-1) {
					creditCardSelected=listCreditCards[comboCreditCards.SelectedIndex];
				}
				if(_listPaySplits.Count>0 && PIn.Double(textAmount.Text)!=PIn.Double(textSplitTotal.Text)
					&& (_listPaySplits.Count!=1 || _listPaySplits[0].PayPlanNum==0)) //Not one paysplit attached to payplan
				{
					MsgBox.Show(this,"Split totals must equal payment amount before running a credit card transaction.");
					return false;
				}
			}
			if(PIn.Date(textDate.Text).Date > DateTime.Today.Date
					&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed) && !PrefC.GetBool(PrefName.AccountAllowFutureDebits)) 
			{
				MsgBox.Show(this,"Payment date cannot be in the future.");
				return false;
			}
			return true;
		}

		///<summary>Only used to void a transaction that has just been completed when the user hits Cancel. Uses the same Print Receipt settings as the 
		///original transaction.</summary>
		private void VoidEdgeExpressTransaction(string transactionId) {
			Cursor=Cursors.WaitCursor;
			bool hasVoided=false;
			if(EdgeExpress.RCM.IsRCMRunning) {
				EdgeExpress.RcmResponse rcmResponse=null;
				try {
					rcmResponse=EdgeExpress.RCM.VoidTransaction(_patient,_payment.ClinicNum,transactionId,false);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				Cursor=Cursors.Default;
				if(rcmResponse is null) {
					//failed, so we will try the alternate method of voiding transaction below
				}
				else if(!rcmResponse.IsSuccess) {
					//throw new ODException(Lans.g(this,"Error from EdgeExpress:")+" "+rcmResponse.RESULTMSG);
				}
				else { 
					double approvedAmt=PIn.Double(rcmResponse.APPROVEDAMOUNT);
					bool showApprovedAmtNotice=false;
					if(approvedAmt!=_payment.PayAmt) {
						showApprovedAmtNotice=true;
					}
					HandleVoidPaymentForFormClosing(rcmResponse.GetPayNote(),rcmResponse.RECEIPTTEXT,showApprovedAmtNotice,approvedAmt,CreditCardSource.EdgeExpressRCM);
					hasVoided=true;
				}
			}
			//Void using the Card Not Present API if RCM didn't void the transaction.
			if(!hasVoided) {
				double amount=_payment.PayAmt;
				if(_xWebResponse!=null) {
					//If the payment has an _xWebReponse we know it how much the transaction was processed for. 
					//Otherwise the transaction was most likely made via RCM and we just need to use the payment amount as a best guess.
					amount=_xWebResponse.Amount;
				}
				XWebResponse xWebResponseVoid;
				XWebResponse xWebResponseProcessed;
				try {
					xWebResponseVoid=EdgeExpress.CNP.VoidTransaction(_patient.PatNum,transactionId,amount,false);
					xWebResponseProcessed=EdgeExpress.CNP.ProcessTransaction(xWebResponseVoid,_payment);
				}
				catch {
					MsgBox.Show(Lans.g(this,"There was a problem voiding the transaction.  Please try again or attempt a return instead."));
					return;
				}
				if(xWebResponseProcessed.ResponseCode==(int)XWebResponseCodes.Approval) {
					HandleVoidPaymentForFormClosing(xWebResponseProcessed.GetFormattedNote(false),
						EdgeExpress.CNP.BuildReceiptString(xWebResponseProcessed,false),
						false,
						xWebResponseProcessed.Amount,
						CreditCardSource.EdgeExpressCNP);
					hasVoided=true;
				}
			}
			if(!hasVoided) {
				MsgBox.Show(Lans.g(this,"There was a problem voiding the transaction.  Please try again or attempt a return instead."));
			}
		}

		private void VoidPayConnectTransaction(string refNum,string amount) {
			PayConnectResponse payConnectResponse=null;
			string receiptStr="";
			if(_creditCardRequestPayConnect==null) {//The payment was made through the terminal.
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => {
					PosRequest posRequest=PosRequest.CreateVoidByReference(refNum);
					PosResponse posResponse=DpsPos.ProcessCreditCard(posRequest);
					payConnectResponse=PayConnectTerminal.ToPayConnectResponse(posResponse);
					receiptStr=PayConnectTerminal.BuildReceiptString(PayConnectTerminal.ToPayConnectResponse(posResponse),null,0);
				};
				progressOD.ShowCancelButton=false;
				progressOD.StartingMessage=Lan.g(this,"Processing void on terminal.");
				progressOD.StopNotAllowedMessage=Lan.g(this,"Not allowed to stop. Please wait up to 2 minutes.");
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception ex){
					MessageBox.Show(Lan.g(this,"Error voiding payment:")+" "+ex.Message);
				}
				if(progressOD.IsCancelled){
					//do nothing.  The code below handles it.
				}
			}
			else {//The payment was made through the web service.
				Cursor=Cursors.WaitCursor;
				_creditCardRequestPayConnect.TransType=PayConnectService.transType.VOID;
				_creditCardRequestPayConnect.RefNumber=refNum;
				_creditCardRequestPayConnect.Amount=PIn.Decimal(amount);
				PayConnectService.transResponse transResponse=PayConnect.ProcessCreditCard(_creditCardRequestPayConnect,_payment.ClinicNum,x => MessageBox.Show(x));
				payConnectResponse=PayConnectREST.ToPayConnectResponse(transResponse,_creditCardRequestPayConnect);
				receiptStr=PayConnect.BuildReceiptString(_creditCardRequestPayConnect,transResponse,null,0);
				Cursor=Cursors.Default;
			}
			if(payConnectResponse==null || payConnectResponse.StatusCode!="0") {//error in transaction
				MsgBox.Show(this,"This credit card payment has already been processed and will have to be voided manually through the web interface.");
				return;
			}
			//Record a new payment for the voided transaction
			double amountCharged=PIn.Double(textAmount.Text);
			if(amountCharged>0) {
				amountCharged*=-1;
			}
			string payNote=Lan.g(this,"Transaction Type")+": "+Enum.GetName(typeof(PayConnectService.transType),PayConnectService.transType.VOID)
				+Environment.NewLine+Lan.g(this,"Status")+": "+payConnectResponse.Description+Environment.NewLine
				+Lan.g(this,"Amount")+": "+amountCharged.ToString("C")+Environment.NewLine
				+Lan.g(this,"Auth Code")+": "+payConnectResponse.AuthCode+Environment.NewLine
				+Lan.g(this,"Ref Number")+": "+payConnectResponse.RefNumber;
			Payment paymentVoid=Payments.InsertVoidPayment(_payment,_listPaySplits,receiptStr,payNote,CreditCardSource.PayConnect,payAmt:amountCharged);
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,paymentVoid.PatNum,
				Patients.GetLim(paymentVoid.PatNum).GetNameLF()+", "+paymentVoid.PayAmt.ToString("c"));
		}

		private void VoidPaySimpleTransaction(string refNum,string originalReceipt) {
			PaySimple.ApiResponse apiResponse=null;
			string receiptStr="";
			Cursor=Cursors.WaitCursor;
			try {
				apiResponse=PaySimple.VoidPayment(refNum,_payment.ClinicNum);
			}
			catch(PaySimpleException ex) {
				MessageBox.Show(ex.Message);
				if(ex.ErrorType==PaySimpleError.CustomerDoesNotExist && MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"Delete the link to the customer id for this patient?")) 
				{
					PatientLinks.DeletePatNumTos(ex.CustomerId,PatientLinkType.PaySimple);
				}
				return;
			}
			catch(ODException wex) {
				MessageBox.Show(wex.Message);//This should have already been Lans.g if applicable.
				return;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error:")+" "+ex.Message);
				return;
			}
			string[] stringArrayReceiptFields=originalReceipt.Replace("\r\n","\n").Replace("\r","\n").Split("\n",StringSplitOptions.RemoveEmptyEntries);
			string ccNum="";
			string expDateStr="";
			string nameOnCard="";
			List<string> listReceiptFields=stringArrayReceiptFields.ToList();
			for(int i=0;i<listReceiptFields.Count;i++) {
				if(listReceiptFields[i].StartsWith("Name")) {
					nameOnCard=listReceiptFields[i].Substring(4).Replace(".","");
				}
				if(listReceiptFields[i].StartsWith("Account")) {
					ccNum=listReceiptFields[i].Substring(7).Replace(".","");
				}
				if(listReceiptFields[i].StartsWith("Exp Date")) {
					expDateStr=listReceiptFields[i].Substring(8).Replace(".","");
				}
			}
			//ACH payments do not have expDates. ACH payments can be voided as long as the void occurs before the batch is closed.
			int expMonth=-1;
			int expYear=-1;
			if(!string.IsNullOrEmpty(expDateStr) && expDateStr.Length > 2) {
				expMonth=PIn.Int(expDateStr.Substring(0,2));
				expYear=PIn.Int(expDateStr.Substring(2));
			}
			apiResponse.BuildReceiptString(ccNum,expMonth,expYear,nameOnCard,_payment.ClinicNum);
			receiptStr=apiResponse.TransactionReceipt;
			Cursor=Cursors.Default;
			Payment paymentVoid=Payments.InsertVoidPayment(_payment,_listPaySplits,receiptStr,apiResponse.ToNoteString(),CreditCardSource.PaySimple);
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,paymentVoid.PatNum,
				Patients.GetLim(paymentVoid.PatNum).GetNameLF()+", "+paymentVoid.PayAmt.ToString("c"));
		}

		///<summary>Only used to void a transaction that has just been completed when the user hits Cancel. Uses the same Print Receipt settings as the 
		///original transaction.</summary>
		private void VoidXChargeTransaction(string transID,string amount,bool isDebit) {
			ProcessStartInfo processStartInfo=new ProcessStartInfo(_programX.Path);
			string resultfile=PrefC.GetRandomTempFile("txt");
			File.Delete(resultfile);//delete the old result file.
			processStartInfo.Arguments="";
			if(isDebit) {
				processStartInfo.Arguments+="/TRANSACTIONTYPE:DEBITRETURN /LOCKTRANTYPE ";
			}
			else {
				processStartInfo.Arguments+="/TRANSACTIONTYPE:VOID /LOCKTRANTYPE ";
			}
			processStartInfo.Arguments+="/XCTRANSACTIONID:"+transID+" /LOCKXCTRANSACTIONID ";
			processStartInfo.Arguments+="/AMOUNT:"+amount+" /LOCKAMOUNT ";
			processStartInfo.Arguments+="/RECEIPT:Pat"+_payment.PatNum.ToString()+" ";//aka invoice#
			processStartInfo.Arguments+="\"/CLERK:"+Security.CurUser.UserName+"\" /LOCKCLERK ";
			processStartInfo.Arguments+="/RESULTFILE:\""+resultfile+"\" ";
			processStartInfo.Arguments+="/USERID:"+ProgramProperties.GetPropVal(_programX.ProgramNum,"Username",_payment.ClinicNum)+" ";
			processStartInfo.Arguments+="/PASSWORD:"+CodeBase.MiscUtils.Decrypt(ProgramProperties.GetPropVal(_programX.ProgramNum,"Password",_payment.ClinicNum))+" ";
			processStartInfo.Arguments+="/AUTOCLOSE ";
			processStartInfo.Arguments+="/HIDEMAINWINDOW /SMALLWINDOW ";
			if(!isDebit) {
				processStartInfo.Arguments+="/AUTOPROCESS ";
			}
			processStartInfo.Arguments+="/PROMPTSIGNATURE:F ";
			processStartInfo.Arguments+="/RECEIPTINRESULT ";
			Cursor=Cursors.WaitCursor;
			Process process=new Process();
			process.StartInfo=processStartInfo;
			process.EnableRaisingEvents=true;
			process.Start();
			process.WaitForExit();
			Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
			Cursor=Cursors.Default;
			//Next, record the voided payment within Open Dental.  We use to delete the payment but Nathan wants us to negate voids with another payment.
			string resultText="";
			string line="";
			bool showApprovedAmtNotice=false;
			double approvedAmt=0;
			string receipt="";
			TextReader textReader;//disposed at the bottom
			try {
				textReader=new StreamReader(resultfile);
			}
			catch {
				MessageBox.Show(Lan.g(this,"There was a problem voiding this transaction.")+"\r\n"+Lan.g(this,"Please run the credit card report from inside "
					+"X-Charge to verify that the transaction was voided.")+"\r\n"+Lan.g(this,"If the transaction was not voided, please create a new payment "
					+"to void the transaction."));
				return;
			}
			line=textReader.ReadLine();
			/*Example of successful void transaction:
				RESULT=SUCCESS
				TYPE=Void
				APPROVALCODE=000000
				SWIPED=F
				CLERK=Admin
				XCACCOUNTID=XAWpQPwLm7MXZ
				XCTRANSACTIONID=15042616
				ACCOUNT=XXXXXXXXXXXX6781
				EXPIRATION=1215
				ACCOUNTTYPE=VISA
				APPROVEDAMOUNT=11.00
			*/
			while(line!=null) {
				if(!line.StartsWith("RECEIPT=")) {//Don't include the receipt string in the PayNote
					if(resultText!="") {
						resultText+="\r\n";
					}
					resultText+=line;
				}
				if(line.StartsWith("RESULT=")) {
					if(line!="RESULT=SUCCESS") {
						//Void was a failure and there might be a description as to why it failed. Continue to loop through line.
						while(line!=null) {
							line=textReader.ReadLine();
							resultText+="\r\n"+line;
						}
						break;
					}
				}
				if(line.StartsWith("APPROVEDAMOUNT=")) {
					approvedAmt=PIn.Double(line.Substring(15));
					if(approvedAmt != _payment.PayAmt) {
						showApprovedAmtNotice=true;
					}
				}
				if(line.StartsWith("RECEIPT=") && line.Length>8) {
					receipt=PIn.String(line.Substring(8));
					receipt=receipt.Replace("\\n","\r\n");//The receipt from X-Charge escapes the newline characters
				}
				line=textReader.ReadLine();
			}
			HandleVoidPaymentForFormClosing(resultText,receipt,showApprovedAmtNotice,approvedAmt,CreditCardSource.XServer);
			textReader.Dispose();
		}

		private void XWebReturn() {
			CreditCard creditCard=null;
			List<CreditCard> listCreditCards=CreditCards.Refresh(_patient.PatNum);
			if(comboCreditCards.SelectedIndex < listCreditCards.Count && comboCreditCards.SelectedIndex >-1) {
				creditCard=listCreditCards[comboCreditCards.SelectedIndex];
			}
			if(creditCard==null) {
				MsgBox.Show(this,"Card no longer available. Return cannot be processed.");
				return;
			}
			if(!creditCard.IsXWeb()) {
				MsgBox.Show(this,"Only cards that were created from XWeb can process an XWeb return.");
				return;
			}
			using FormXWeb formXWeb=new FormXWeb(_patient.PatNum,creditCard,XWebTransactionType.CreditReturnTransaction,createPayment:false,_payment.PayAmt);
			formXWeb.LockCardInfo=true;
			if(formXWeb.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(formXWeb.ResponseResult==null) {
				MsgBox.Show(this,"Return failed.");
				return;
			}
			if(textNote.Text!="") {
				textNote.AppendText(Environment.NewLine);
			}
			Payment paymentReturn=Payments.InsertReturnXWebPayment(_payment,formXWeb.ResponseResult.GetFormattedNote(false),(-formXWeb.ResponseResult.Amount));
			formXWeb.ResponseResult.PaymentNum=paymentReturn.PayNum;
			XWebResponses.Update(formXWeb.ResponseResult);
			SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,paymentReturn.PatNum,
				Patients.GetLim(paymentReturn.PatNum).GetNameLF() + ", " + paymentReturn.PayAmt.ToString("c"));
			butVoid.Visible=true;
			LayoutManager.MoveHeight(groupXWeb,85);
			MsgBox.Show(this,"Return successful.");
		}

		private void XWebVoid() {
			//Uses DateTime.Today because a void is a new transaction that is created upon entering this function
			if(!Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today)) {
				return;
			}
			double amount=_xWebResponse.Amount;
			if(_xWebResponse.XTransactionType==XWebTransactionType.CreditReturnTransaction 
				|| _xWebResponse.XTransactionType==XWebTransactionType.DebitReturnTransaction) 
			{
				amount=-amount;//The amount in an xwebresponse is always stored as a positive number.
			}
			if(MessageBox.Show(Lan.g(this,"Void the XWeb transaction of amount")+" "+amount.ToString("f")+" "+Lan.g(this,"attached to this payment?"),
				"",MessageBoxButtons.YesNo)==DialogResult.No)
			{
				return;
			}
			Cursor=Cursors.WaitCursor;
			string payNote=Lan.g(this,"Void XWeb payment made from within Open Dental");
			try {
				XWebs.VoidPayment(_patient.PatNum,payNote,_xWebResponse.XWebResponseNum);
			}
			catch(ODException ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Void successful. A new payment has been created for this void transaction.");
		}
		#endregion

		#region Methods - Public
		///<summary>Launches the PayConnect transaction window.  Returns null upon failure, otherwise returns the transaction detail as a string.
		///If prepaidAmt is not zero, then will show the PayConnect window with the given prepaid amount and let the user enter card # and exp.
		///A patient is not required for prepaid cards.</summary>
		public string MakePayConnectTransaction(double prepaidAmt=0) {
			if(!HasPayConnect()) {
				return null;
			}
			if(prepaidAmt==0) {//Validation for regular credit cards (not prepaid cards).
				if(textAmount.Text=="") {
					MsgBox.Show(this,"Please enter an amount first.");
					textAmount.Focus();
					return null;
				}
				if(_listPaySplits.Count>0 && PIn.Double(textAmount.Text)!=PIn.Double(textSplitTotal.Text)
					&& (_listPaySplits.Count!=1 || _listPaySplits[0].PayPlanNum==0)) //Not one paysplit attached to payplan
				{
					MsgBox.Show(this, "Split totals must equal payment amount before running a credit card transaction.");
					return null;
				}
			}
			if(PIn.Date(textDate.Text).Date > DateTime.Today.Date
					&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed) && !PrefC.GetBool(PrefName.AccountAllowFutureDebits))
			{
				MsgBox.Show(this,"Payment date cannot be in the future.");
				return null;
			}
			CreditCard creditCard=null;
			List<CreditCard> listCreditCards=null;
			decimal amount=Math.Abs(PIn.Decimal(textAmount.Text));//PayConnect always wants a positive number even for voids and returns.
			if(prepaidAmt==0) {
				listCreditCards=CreditCards.Refresh(_patient.PatNum);
				if(comboCreditCards.SelectedIndex < listCreditCards.Count) {
					creditCard=listCreditCards[comboCreditCards.SelectedIndex];
				}
			}
			else {//Prepaid card
				amount=(decimal)prepaidAmt;
			}
			using FormPayConnect formPayConnect=new FormPayConnect(_payment.ClinicNum,_patient,amount,creditCard);
			formPayConnect.ShowDialog();
			if(prepaidAmt==0 && formPayConnect.GetResponse()!=null) {//Regular credit cards (not prepaid cards).
				//If PayConnect response is not null, refresh comboCreditCards and select the index of the card used for this payment if the token was saved
				listCreditCards=CreditCards.Refresh(_patient.PatNum);
				AddCreditCardsToCombo(listCreditCards,x => x.PayConnectToken==formPayConnect.GetResponse().PaymentToken
					&&x.PayConnectTokenExp.Year==formPayConnect.GetResponse().TokenExpiration.Year
					&&x.PayConnectTokenExp.Month==formPayConnect.GetResponse().TokenExpiration.Month);
				Program program=Programs.GetCur(ProgramName.PayConnect);
				//still need to add functionality for accountingAutoPay
				string paytype=ProgramProperties.GetPropVal(program.ProgramNum,"PaymentType",_payment.ClinicNum);//paytype could be an empty string
				if(!PrefC.GetBool(PrefName.PaymentsPromptForPayType)) { 
					listPayType.SelectedIndex=Defs.GetOrder(DefCat.PaymentTypes,PIn.Long(paytype));
				}
				SetComboDepositAccounts();
			}
			double amountCharged=(double)amount;
			if(amountCharged>0 && formPayConnect.TransType==PayConnectService.transType.VOID) {
				amountCharged*=-1;
			}
			string resultNote=null;
			if(formPayConnect.GetResponse()!=null) {
				resultNote=Lan.g(this,"Transaction Type")+": "+Enum.GetName(typeof(PayConnectService.transType),formPayConnect.TransType)+Environment.NewLine+
					Lan.g(this,"Status")+": "+formPayConnect.GetResponse().Description+Environment.NewLine+
					Lan.g(this,"Amount")+": "+amountCharged.ToString("C")+Environment.NewLine+
					Lan.g(this,"Card Type")+": "+formPayConnect.GetResponse().CardType+Environment.NewLine+
					Lan.g(this,"Account")+": "+StringTools.TruncateBeginning(formPayConnect.GetCardNumber(),4).PadLeft(formPayConnect.GetCardNumber().Length,'X')+Environment.NewLine+
					Lan.g(this,"Auth Code")+": "+formPayConnect.GetResponse().AuthCode+Environment.NewLine+
					Lan.g(this,"Ref Number")+": "+formPayConnect.GetResponse().RefNumber;
			}
			if(prepaidAmt!=0) {
				if(formPayConnect.GetResponse()!=null && formPayConnect.GetResponse().StatusCode=="0") { //The transaction succeeded.
					_payment.IsCcCompleted=true;
					return resultNote;
				}
				return null;
			}
			if(formPayConnect.GetResponse()!=null) {
				if(formPayConnect.GetResponse().StatusCode=="0") { //The transaction succeeded.
					_payment.IsCcCompleted=true;
					_isCCDeclined=false;
					if(formPayConnect.TransType==PayConnectService.transType.RETURN) {
						textAmount.Text="-"+formPayConnect.GetAmountCharged();
						_payment.Receipt=formPayConnect.ReceiptStr;
					}
					else if(formPayConnect.TransType==PayConnectService.transType.AUTH) {
						textAmount.Text=formPayConnect.GetAmountCharged();
					}
					else if(formPayConnect.TransType==PayConnectService.transType.SALE) {
						textAmount.Text=formPayConnect.GetAmountCharged();
						_payment.Receipt=formPayConnect.ReceiptStr;
					}
					if(formPayConnect.TransType==PayConnectService.transType.VOID) {//Close FormPayment window now so the user will not have the option to hit Cancel
						if(IsNew) {
							if(!_wasCreditCardSuccessful) {
								textAmount.Text="-"+formPayConnect.GetAmountCharged();
								textNote.Text+=((textNote.Text=="")?"":Environment.NewLine)+resultNote;
							}
							_payment.Receipt=formPayConnect.ReceiptStr;
							if(SavePaymentToDb()) {
								MsgBox.Show(this,"Void successful.");
								DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
							}
							return resultNote;
						}
						if(!IsNew || _wasCreditCardSuccessful) {//Create a new negative payment if the void is being run from an existing payment
							if(_listPaySplits.Count==0) {
								AddOneSplit();
								Reinitialize();
							}
							else if(_listPaySplits.Count==1//if one split
								&& _listPaySplits[0].PayPlanNum!=0//and split is on a payment plan
								&& _listPaySplits[0].SplitAmt!=_payment.PayAmt)//and amount doesn't match payment
							{
								_listPaySplits[0].SplitAmt=_payment.PayAmt;//make amounts match automatically
								textSplitTotal.Text=textAmount.Text;
							}
							_payment.IsSplit=_listPaySplits.Count>1;
							Payments.InsertVoidPayment(_payment,_listPaySplits,formPayConnect.ReceiptStr,resultNote,CreditCardSource.PayConnect,payAmt:amountCharged);
							string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patient.PatNum,_listPaySplits);
							if(!string.IsNullOrEmpty(strErrorMsg)) {
								MessageBox.Show(strErrorMsg);
							}
						}
						MsgBox.Show(this,"Void successful.");
						DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
						return resultNote;
					}
					else {//Not Void
						_wasCreditCardSuccessful=true; //Will void the transaction if user cancels out of window.
					}
					_creditCardRequestPayConnect=formPayConnect.CreditCardRequest;
				}
				textNote.Text+=((textNote.Text=="")?"":Environment.NewLine)+resultNote;
				textNote.SelectionStart=textNote.TextLength;
				textNote.ScrollToCaret();//Scroll to the end of the text box to see the newest notes.
				_payment.PayNote=textNote.Text;
				_payment.PaymentSource=CreditCardSource.PayConnect;
				_payment.ProcessStatus=ProcessStat.OfficeProcessed;
				Payments.Update(_paymentOld,true);
			}
			if(!string.IsNullOrEmpty(_payment.Receipt)) {
				butPrintReceipt.Visible=true;
				if(PrefC.GetBool(PrefName.AllowEmailCCReceipt)) {
					butEmailReceipt.Visible=true;
				}
			}
			if(formPayConnect.WasPaymentAttempted && !_payment.IsCcCompleted && (formPayConnect.GetResponse()==null || formPayConnect.GetResponse().StatusCode!="0")) { //The transaction failed.
				if(formPayConnect.TransType==PayConnectService.transType.SALE || formPayConnect.TransType==PayConnectService.transType.AUTH) {
					textAmount.Text=formPayConnect.GetAmountCharged();//Preserve the amount so the user can try the payment again more easily.
				}
				_isCCDeclined=true;
				_wasCreditCardSuccessful=false;
				return formPayConnect.GetResponse()?.Description??resultNote??Lan.g(this,"PayConnect charge failed to process.");
			}
			return resultNote;
		}

		///<summary>Launches the PaySimple transaction window.  Returns null upon failure, otherwise returns the transaction detail as a string.
		///If prepaidAmt is not zero, then will show the PaySimple window with the given prepaid amount and let the user enter card # and exp.
		///A patient is not required for prepaid cards.</summary>
		public string MakePaySimpleTransaction(double prepaidAmt=0,string carrierName="") {
			if(!HasPaySimple()) {
				return null;
			}
			CreditCard creditCard=null;
			List<CreditCard> listCreditCards=null;
			decimal amount=Math.Abs(PIn.Decimal(textAmount.Text));//PaySimple always wants a positive number even for voids and returns.
			if(prepaidAmt==0) {
				listCreditCards=CreditCards.Refresh(_patient.PatNum);
				if(comboCreditCards.SelectedIndex < listCreditCards.Count) {
					creditCard=listCreditCards[comboCreditCards.SelectedIndex];
				}
			}
			else {//Prepaid card
				amount=(decimal)prepaidAmt;
			}
			using FormPaySimple formPaySimple=new FormPaySimple(_payment.ClinicNum,_patient,amount,creditCard,carrierName:carrierName);
			formPaySimple.ShowDialog();
			Program program=Programs.GetCur(ProgramName.PaySimple);
			if(prepaidAmt==0) {//Regular credit cards (not prepaid cards).
				//If PaySimple response is not null, refresh comboCreditCards and select the index of the card used for this payment if the token was saved
				listCreditCards=CreditCards.Refresh(_patient.PatNum);
				string paySimpleToken=creditCard==null ? "" : creditCard.PaySimpleToken;
				if(formPaySimple.ApiResponseOut!=null) {
					paySimpleToken=formPaySimple.ApiResponseOut.PaySimpleToken;
				}
				AddCreditCardsToCombo(listCreditCards,x => x.PaySimpleToken==paySimpleToken && !string.IsNullOrEmpty(paySimpleToken));
				//still need to add functionality for accountingAutoPay
				//paytype could be an empty string
				string paytype=ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,PaySimple.PropertyDescs.PaySimplePayTypeCC,_payment.ClinicNum);
				if(!PrefC.GetBool(PrefName.PaymentsPromptForPayType)) {
					listPayType.SelectedIndex=Defs.GetOrder(DefCat.PaymentTypes,PIn.Long(paytype));
				}
				SetComboDepositAccounts();
			}
			if(prepaidAmt!=0) {
				if(formPaySimple.ApiResponseOut!=null) { //The transaction succeeded.
					_payment.IsCcCompleted=true;
					if(formPaySimple.ApiResponseOut.CCSource==CreditCardSource.PaySimpleACH) {
						_payment.PaymentStatus=PaymentStatus.PaySimpleAchPosted;
						_payment.ExternalId=formPaySimple.ApiResponseOut.RefNumber;
					}
					return formPaySimple.ApiResponseOut.ToNoteString();
				}
				return null;
			}
			string resultNote=null;
			if(formPaySimple.ApiResponseOut!=null) { //The transaction succeeded.
				_payment.IsCcCompleted=true;
				_isCCDeclined=false;
				resultNote=formPaySimple.ApiResponseOut.ToNoteString();
				_payment.PaymentSource=formPaySimple.ApiResponseOut.CCSource;
				if(formPaySimple.ApiResponseOut.CCSource==CreditCardSource.PaySimpleACH) {
					string paytype=ProgramProperties.GetPropValForClinicOrDefault(program.ProgramNum,PaySimple.PropertyDescs.PaySimplePayTypeACH,
						_payment.ClinicNum);
					_payment.PaymentStatus=PaymentStatus.PaySimpleAchPosted;
					_payment.ExternalId=formPaySimple.ApiResponseOut.RefNumber;
					int defOrder=Defs.GetOrder(DefCat.PaymentTypes,PIn.Long(paytype));
					//paytype could be an empty string, so then leave listPayType as it was.
					if(defOrder>=-1 && !PrefC.GetBool(PrefName.PaymentsPromptForPayType)) {
						listPayType.SelectedIndex=defOrder;
					}
				}
				if(formPaySimple.ApiResponseOut.TransType==PaySimple.TransType.RETURN) {
					textAmount.Text="-"+formPaySimple.ApiResponseOut.Amount.ToString("F");
					_payment.Receipt=formPaySimple.ApiResponseOut.TransactionReceipt;
				}
				else if(formPaySimple.ApiResponseOut.TransType==PaySimple.TransType.AUTH) {
					textAmount.Text=formPaySimple.ApiResponseOut.Amount.ToString("F");
				}
				else if(formPaySimple.ApiResponseOut.TransType==PaySimple.TransType.SALE) {
					textAmount.Text=formPaySimple.ApiResponseOut.Amount.ToString("F");
					_payment.Receipt=formPaySimple.ApiResponseOut.TransactionReceipt;
				}
				if(formPaySimple.ApiResponseOut.TransType==PaySimple.TransType.VOID) {//Close FormPayment window now so the user will not have the option to hit Cancel
					if(IsNew) {
						if(!_wasCreditCardSuccessful) {
							textAmount.Text="-"+formPaySimple.ApiResponseOut.Amount.ToString("F");
							textNote.Text+=((textNote.Text=="") ? "" : Environment.NewLine)+resultNote;
						}
						_payment.Receipt=formPaySimple.ApiResponseOut.TransactionReceipt;
						if(SavePaymentToDb()) {
							MsgBox.Show(this,"Void successful.");
							DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
						}
						return resultNote;
					}
					if(!IsNew || _wasCreditCardSuccessful) {//Create a new negative payment if the void is being run from an existing payment
						if(_listPaySplits.Count==0) {
							AddOneSplit();
							Reinitialize();
						}
						else if(_listPaySplits.Count==1//if one split
							&& _listPaySplits[0].PayPlanNum!=0//and split is on a payment plan
							&& _listPaySplits[0].SplitAmt!=_payment.PayAmt)//and amount doesn't match payment
						{
							_listPaySplits[0].SplitAmt=_payment.PayAmt;//make amounts match automatically
							textSplitTotal.Text=textAmount.Text;
						}
						_payment.IsSplit=_listPaySplits.Count>1;
						Payments.InsertVoidPayment(_payment,_listPaySplits,formPaySimple.ApiResponseOut.TransactionReceipt,resultNote,CreditCardSource.PaySimple);
						string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patient.PatNum,_listPaySplits);
						if(!string.IsNullOrEmpty(strErrorMsg)) {
							MessageBox.Show(strErrorMsg);
						}
					}
					MsgBox.Show(this,"Void successful.");
					DialogResult=DialogResult.OK;//Close FormPayment window now so the user will not have the option to hit Cancel
					return resultNote;
				}
				else {//Not Void
					_wasCreditCardSuccessful=true; //Will void the transaction if user cancels out of window.
				}
				_apiResponsePaySimple=formPaySimple.ApiResponseOut;
			}
			if(!string.IsNullOrWhiteSpace(resultNote)) {
				textNote.Text+=((textNote.Text=="") ? "" : Environment.NewLine)+resultNote;
			}
			textNote.SelectionStart=textNote.TextLength;
			textNote.ScrollToCaret();//Scroll to the end of the text box to see the newest notes.
			_payment.PayNote=textNote.Text;
			if(_payment.PaymentSource==CreditCardSource.None) {
				_payment.PaymentSource=CreditCardSource.PaySimple;
			}
			_payment.ProcessStatus=ProcessStat.OfficeProcessed;
			Payments.Update(_payment,true);
			if(!string.IsNullOrEmpty(_payment.Receipt)) {
				butPrintReceipt.Visible=true;
				if(PrefC.GetBool(PrefName.AllowEmailCCReceipt)) {
					butEmailReceipt.Visible=true;
				}
			}
			if(formPaySimple.WasPaymentAttempted && !_payment.IsCcCompleted && (formPaySimple.ApiResponseOut==null || formPaySimple.ApiResponseOut.Status.ToLower()=="failed")) { //The transaction failed.
				//PaySimple checks the transaction type here and sets the amount the user chose to the textAmount textbox. 
				//We don't have that information here so do nothing.
				_isCCDeclined=true;
				_wasCreditCardSuccessful=false;
				return formPaySimple.ApiResponseOut?.FailureDescription??resultNote??Lan.g(this,"PaySimple charge failed to process.");
			}
			return resultNote;
		}

		///<summary>Launches the XCharge transaction window and then executes whatever type of transaction was selected for the current payment.
		///This is to help troubleshooting. Returns null upon failure, otherwise returns the transaction detail as a string.
		///If prepaidAmt is not zero, then will show the xcharge window with the given prepaid amount and let the user enter card # and exp.
		///A patient is not required for prepaid cards.</summary>
		public string MakeXChargeTransaction(double prepaidAmt=0) {
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"XCharge is not available while viewing through the web.");
				return null;
			}
			//Need to refresh this list locally in case we are coming from another form
			_listDefsPaymentType=_listDefsPaymentType??Defs.GetDefsForCategory(DefCat.PaymentTypes,true);
			XchargeMilestone="Validation";
			CreditCard creditCard=null;
			List<CreditCard> listCreditCards=null;
			if(prepaidAmt!=0) {
				CheckUIState();//To ensure that _xProg is set and _xPath is set.  Normally this would happen when loading.  Needed for HasXCharge().
			}
			if(!HasXCharge()) {//Will show setup window if xcharge is not enabled or not completely setup yet.
				return null;
			}
			if(!ValidateForCreditCardPayment(prepaidAmt!=0,out creditCard)) {
				return null;
			}
			if(creditCard!=null && creditCard.IsXWeb()) {
				MsgBox.Show(this,"Cards saved through XWeb cannot be used with the XCharge client program.");
				return null;
			}
			XchargeMilestone="XResult File";
			string resultfile=PrefC.GetRandomTempFile("txt");
			try {
				File.Delete(resultfile);//delete the old result file.
			}
			catch {
				MsgBox.Show(this,"Could not delete XResult.txt file.  It may be in use by another program, flagged as read-only, or you might not have "
					+"sufficient permissions.");
				return null;
			}
			XchargeMilestone="Properties";
			bool doNeedToken=false;
			bool isNewCard=false;
			bool hasXToken=false;
			bool isNotRecurring=false;
			if(prepaidAmt==0) {
				//These UI changes only need to happen for regular credit cards when the payment window is displayed.
				string xPayTypeNum=ProgramProperties.GetPropVal(_programX.ProgramNum,"PaymentType",_payment.ClinicNum);
				//still need to add functionality for accountingAutoPay
				if(!PrefC.GetBool(PrefName.PaymentsPromptForPayType)) { 
					listPayType.SelectedIndex=Defs.GetOrder(DefCat.PaymentTypes,PIn.Long(xPayTypeNum));
				}
				SetComboDepositAccounts();
			}
			/*XCharge.exe [/TRANSACTIONTYPE:type] [/AMOUNT:amount] [/ACCOUNT:account] [/EXP:exp]
				[“/TRACK:track”] [/ZIP:zip] [/ADDRESS:address] [/RECEIPT:receipt] [/CLERK:clerk]
				[/APPROVALCODE:approval] [/AUTOPROCESS] [/AUTOCLOSE] [/STAYONTOP] [/MID]
				[/RESULTFILE:”C:\Program Files\X-Charge\LocalTran\XCResult.txt”*/
			ProcessStartInfo processStartInfo=new ProcessStartInfo(_xPath);
			Patient patient=null;
			if(prepaidAmt==0) {
				patient=Patients.GetPat(_payment.PatNum);
				if(patient==null) {
					MsgBox.Show(this,"Invalid patient associated to this payment.");
					return null;
				}
			}
			processStartInfo.Arguments="";
			double amt=PIn.Double(textAmount.Text);
			if(prepaidAmt != 0) {
				amt=prepaidAmt;
			}
			if(amt<0) {//X-Charge always wants a positive number, even for returns.
				amt*=-1;
			}
			processStartInfo.Arguments+="/AMOUNT:"+amt.ToString("F2")+" ";
			XchargeMilestone="Get Selected Credit Card";
			using FormXchargeTrans formXchargeTrans=new FormXchargeTrans();
			int tranType=0;//Default to 0 "Purchase" for prepaid cards.
			string cashBack=null;
			if(prepaidAmt==0) {//All regular cards (not prepaid)
				XchargeMilestone="Transaction Window Launch";
				//Show window to lock in the transaction type.
				formXchargeTrans.PrintReceipt=PIn.Bool(ProgramProperties.GetPropVal(_programX.ProgramNum,"PrintReceipt",_payment.ClinicNum));
				formXchargeTrans.PromptSignature=PIn.Bool(ProgramProperties.GetPropVal(_programX.ProgramNum,"PromptSignature",_payment.ClinicNum));
				formXchargeTrans.ClinicNum=_payment.ClinicNum;
				formXchargeTrans.ShowDialog();
				if(formXchargeTrans.DialogResult!=DialogResult.OK) {
					return null;
				}
				XchargeMilestone="Transaction Window Digest";
				_payment.PaymentSource=CreditCardSource.XServer;
				_payment.ProcessStatus=ProcessStat.OfficeProcessed;
				tranType=formXchargeTrans.TransactionType;
				decimal cashAmt=formXchargeTrans.CashBackAmount;
				cashBack=cashAmt.ToString("F2");
				_doPromptSignature=formXchargeTrans.PromptSignature;
				_doPrintReceipt=formXchargeTrans.PrintReceipt;
			}
			XchargeMilestone="Check Duplicate Cards";
			if(creditCard!=null && !string.IsNullOrEmpty(creditCard.XChargeToken)) {//Have CC on file with an XChargeToken
				hasXToken=true;
				if(CreditCards.GetXChargeTokenCount(creditCard.XChargeToken,false)!=1) {
					MsgBox.Show(this,"This card shares a token with another card. Delete it from the Credit Card Manage window and re-add it.");
					return null;
				}
				/*       ***** An example of how recurring charges work***** 
				C:\Program Files\X-Charge\XCharge.exe /TRANSACTIONTYPE:Purchase /LOCKTRANTYPE
				/AMOUNT:10.00 /LOCKAMOUNT /XCACCOUNTID:XAW0JWtx5kjG8 /RECEIPT:RC001
				/LOCKRECEIPT /CLERK:Clerk /LOCKCLERK /RESULTFILE:C:\ResultFile.txt /USERID:system
				/PASSWORD:system /STAYONTOP /AUTOPROCESS /AUTOCLOSE /HIDEMAINWINDOW
				/RECURRING /SMALLWINDOW /NORESULTDIALOG
				*/
			}
			else if(creditCard!=null) {//Have CC on file, no XChargeToken so not a recurring charge, and might need a token.
				isNotRecurring=true;
				if(!PrefC.GetBool(PrefName.StoreCCnumbers)) {//Use token only if user has has pref unchecked in module setup (allow store credit card nums).
					doNeedToken=true;//Will create a token from result file so credit card info isn't saved in our db.
				}
			}
			else {//CC is null, add card option was selected in credit card drop down, no other possibility.
				isNewCard=true;
			}
			XchargeMilestone="Arguments Fill Card Info";
			processStartInfo.Arguments+=GetXChargeTransactionTypeCommands(tranType,hasXToken,isNotRecurring,creditCard,cashBack);
			if(prepaidAmt!=0) {
				//Zip and address are optional fields and for prepaid cards this information is probably not provided to the user.
			}
			else if(isNewCard) {
				processStartInfo.Arguments+="\"/ZIP:"+CleanString(patient.Zip)+"\" ";
				processStartInfo.Arguments+="\"/ADDRESS:"+CleanString(patient.Address)+"\" ";
			}
			else {
				if(creditCard.CCExpiration!=null && creditCard.CCExpiration.Year>2005) {
					processStartInfo.Arguments+="/EXP:"+creditCard.CCExpiration.ToString("MMyy")+" ";
				}
				if(!string.IsNullOrEmpty(creditCard.Zip)) {
					processStartInfo.Arguments+="\"/ZIP:"+CleanString(creditCard.Zip)+"\" ";
				}
				else {
					processStartInfo.Arguments+="\"/ZIP:"+CleanString(patient.Zip)+"\" ";
				}
				if(!string.IsNullOrEmpty(creditCard.Address)) {
					processStartInfo.Arguments+="\"/ADDRESS:"+CleanString(creditCard.Address)+"\" ";
				}
				else {
					processStartInfo.Arguments+="\"/ADDRESS:"+CleanString(patient.Address)+"\" ";
				}
				if(hasXToken) {//Special parameter for tokens.
					processStartInfo.Arguments+="/RECURRING ";
				}
			}
			XchargeMilestone="Arguments Fill X-Charge Settings";
			if(prepaidAmt==0) {
				processStartInfo.Arguments+="/RECEIPT:Pat"+_payment.PatNum.ToString()+" ";//aka invoice#
			}
			else {
				processStartInfo.Arguments+="/RECEIPT:PREPAID ";//aka invoice#
			}
			processStartInfo.Arguments+="\"/CLERK:"+Security.CurUser.UserName+"\" /LOCKCLERK ";
			processStartInfo.Arguments+="/RESULTFILE:\""+resultfile+"\" ";
			processStartInfo.Arguments+="/USERID:"+ProgramProperties.GetPropVal(_programX.ProgramNum,"Username",_payment.ClinicNum)+" ";
			processStartInfo.Arguments+="/PASSWORD:"+CodeBase.MiscUtils.Decrypt(ProgramProperties.GetPropVal(_programX.ProgramNum,"Password",_payment.ClinicNum))+" ";
			processStartInfo.Arguments+="/PARTIALAPPROVALSUPPORT:T ";
			processStartInfo.Arguments+="/AUTOCLOSE ";
			processStartInfo.Arguments+="/HIDEMAINWINDOW ";
			processStartInfo.Arguments+="/SMALLWINDOW ";
			processStartInfo.Arguments+="/GETXCACCOUNTID ";
			processStartInfo.Arguments+="/NORESULTDIALOG ";
			XchargeMilestone="X-Charge Launch";
			Cursor=Cursors.WaitCursor;
			Process process=new Process();
			process.StartInfo=processStartInfo;
			process.EnableRaisingEvents=true;
			process.Start();
			process.WaitForExit();
			XchargeMilestone="X-Charge Complete";
			Thread.Sleep(200);//Wait 2/10 second to give time for file to be created.
			Cursor=Cursors.Default;
			string resultText="";
			string line="";
			bool showApprovedAmtNotice=false;
			bool xAdjust=false;
			bool xReturn=false;
			bool xVoid=false;
			double approvedAmt=0;
			double additionalFunds=0;
			string xChargeToken="";
			string accountMasked="";
			string expiration="";
			string signatureResult="";
			string receipt="";
			bool isDigitallySigned=false;
			bool doUpdateCard=false;
			string newAccount="";
			DateTime dateNewExpiration=new DateTime();
			XchargeMilestone="Digest XResult";
			TextReader textReader;//disposed at the bottom
			try {
				textReader=new StreamReader(resultfile);
			}
			catch {
				MessageBox.Show(Lan.g(this,"There was a problem charging the card.  Please run the credit card report from inside X-Charge to verify that "
					+"the card was not actually charged.")+"\r\n"+Lan.g(this,"If the card was charged, you need to make sure that the payment amount matches.")
					+"\r\n"+Lan.g(this,"If the card was not charged, please try again."));
				return null;
			}
			line=textReader.ReadLine();
			/*Example of successful transaction:
				RESULT=SUCCESS
				TYPE=Purchase
				APPROVALCODE=000064
				ACCOUNT=XXXXXXXXXXXX6781
				ACCOUNTTYPE=VISA*
				AMOUNT=1.77
				AVSRESULT=Y
				CVRESULT=M
			*/
			while(line!=null) {
				if(!line.StartsWith("RECEIPT=")) {//Don't include the receipt string in the PayNote
					if(resultText!="") {
						resultText+="\r\n";
					}
					resultText+=line;
				}
				if(line.StartsWith("RESULT=")) {
					if(line!="RESULT=SUCCESS") {
						//Charge was a failure and there might be a description as to why it failed. Continue to loop through line.
						while(line!=null) {
							line=textReader.ReadLine();
							if(line!=null && !line.StartsWith("RECEIPT=")) {//Don't include the receipt string in the PayNote
								resultText+="\r\n"+line;
							}
						}
						doNeedToken=false;//Don't update CCard due to failure
						isNewCard=false;//Don't insert CCard due to failure
            if(!_payment.IsCcCompleted) {
							_isCCDeclined=true;
            }
						break;
					}
					if(tranType==1) {
						xReturn=true;
					}
					if(tranType==6) {
						xAdjust=true;
					}
					if(tranType==7) {
						xVoid=true;
					}
					_isCCDeclined=false;
				}
				if(line.StartsWith("APPROVEDAMOUNT=")) {
					approvedAmt=PIn.Double(line.Substring(15));
					if(approvedAmt != amt) {
						showApprovedAmtNotice=true;
					}
				}
				if(line.StartsWith("XCACCOUNTID=")) {
					xChargeToken=PIn.String(line.Substring(12));
				}
				if(line.StartsWith("ACCOUNT=")) {
					accountMasked=PIn.String(line.Substring(8));
				}
				if(line.StartsWith("EXPIRATION=")) {
					expiration=PIn.String(line.Substring(11));
				}
				if(line.StartsWith("ADDITIONALFUNDSREQUIRED=")) {
					additionalFunds=PIn.Double(line.Substring(24));
				}
				if(line.StartsWith("SIGNATURE=") && line.Length>10) {
					signatureResult=PIn.String(line.Substring(10));
					//A successful digitally signed signature will say SIGNATURE=C:\Users\Folder\Where\The\Signature\Is\Stored.bmp
					if(signatureResult!="NOT SUPPORTED" && signatureResult!="FAILED") {
						isDigitallySigned=true;
					}
				}
				if(line.StartsWith("RECEIPT=")) {
					receipt=PIn.String(line.Replace("RECEIPT=","").Replace("\\n","\n"));//The receipt from X-Charge escapes the newline characters
					if(isDigitallySigned) {
						//Replace X____________________________ with 'Electronically signed'
						receipt.Split('\n').ToList().FindAll(x => x.StartsWith("X___")).ForEach(x => x="Electronically signed");
					}
					receipt=receipt.Replace("\r","").Replace("\n","\r\n");//remove any existing \r's before replacing \n's with \r\n's
				}
				if(line=="XCACCOUNTIDUPDATED=T") {//Decline minimizer updated the account information since the last time this card was charged
					doUpdateCard=true;
				}
				if(line.StartsWith("ACCOUNT=")) {
					newAccount=line.Substring("ACCOUNT=".Length);
				}
				if(line.StartsWith("EXPIRATION=")) {
					string expStr=line.Substring("EXPIRATION=".Length);//Expiration should be MMYY
					dateNewExpiration=new DateTime(PIn.Int("20"+expStr.Substring(2)),PIn.Int(expStr.Substring(0,2)),1);//First day of the month
				}
				line=textReader.ReadLine();
			}
			if(doNeedToken && !string.IsNullOrEmpty(xChargeToken) && prepaidAmt==0) {//never save token for prepaid cards
				XchargeMilestone="Update Token";
				DateTime dateExp=new DateTime(PIn.Int("20"+StringTools.TruncateBeginning(expiration,2)),PIn.Int(StringTools.Truncate(expiration,2)),1);
				//If the stored CC used for this X-Charge payment has a PayConnect token, and X-Charge returns a different masked number or exp date, we
				//will clear out the PayConnect token since this CC no longer refers to the same card that was used to generate the PayConnect token.
				if(!string.IsNullOrEmpty(creditCard.PayConnectToken) //there is a PayConnect token for this saved CC
					&& Regex.IsMatch(creditCard.CCNumberMasked,@"X+[0-9]{4}") //the saved CC has a masked number with the pattern XXXXXXXXXXXX1234
					&& (StringTools.TruncateBeginning(creditCard.CCNumberMasked,4)!=StringTools.TruncateBeginning(accountMasked,4) //and either the last four digits don't match what X-Charge returned
							|| creditCard.CCExpiration.Year!=dateExp.Year //or the exp year doesn't match that returned by X-Charge
							|| creditCard.CCExpiration.Month!=dateExp.Month)) //or the exp month doesn't match that returned by X-Charge
				{
					creditCard.PayConnectToken="";
					creditCard.PayConnectTokenExp=DateTime.MinValue;
				}
				//Only way this code can be hit is if they have set up a credit card and it does not have a token.
				//So we'll use the created token from result file and assign it to the coresponding account.
				//Also will delete the credit card number and replace it with secure masked number.
				creditCard.XChargeToken=xChargeToken;
				creditCard.CCNumberMasked=accountMasked;
				creditCard.CCExpiration=dateExp;
				creditCard.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
				creditCard.CCSource=CreditCardSource.XServer;
				CreditCards.Update(creditCard);
			}
			if(isNewCard && prepaidAmt==0) {//never save card information to the patient account for prepaid cards
				if(!string.IsNullOrEmpty(xChargeToken) && formXchargeTrans.SaveToken) {
					XchargeMilestone="Create New Credit Card Entry";
					creditCard=CreditCards.CreateNewOpenEdgeCard(_patient.PatNum,_payment.ClinicNum,xChargeToken,expiration.Substring(0,2),
						expiration.Substring(2,2),accountMasked,CreditCardSource.XServer);
				}
				else if(string.IsNullOrEmpty(xChargeToken)) {//Shouldn't happen again but leaving just in case.
					MsgBox.Show(this,"X-Charge didn't return a token so credit card information couldn't be saved.");
				}
			}
			if(doUpdateCard && newAccount!="" && dateNewExpiration.Year>1880 && prepaidAmt==0) {//Never save credit card info to patient for prepaid cards.
				if(textNote.Text!="") {
					textNote.Text+="\r\n";
				}
				if(creditCard.CCNumberMasked != newAccount) {
					textNote.Text+=Lan.g(this,"Account number changed from")+" "+creditCard.CCNumberMasked+" "
						+Lan.g(this,"to")+" "+newAccount;
				}
				if(creditCard.CCExpiration != dateNewExpiration) {
					textNote.Text+=Lan.g(this,"Expiration changed from")+" "+creditCard.CCExpiration.ToString("MMyy")+" "
						+Lan.g(this,"to")+" "+dateNewExpiration.ToString("MMyy");
				}
				creditCard.CCNumberMasked=newAccount;
				creditCard.CCExpiration=dateNewExpiration;
				CreditCards.Update(creditCard);
			}
			XchargeMilestone="Check Approved Amount";
			if(showApprovedAmtNotice && !xVoid && !xAdjust && !xReturn) {
				MessageBox.Show(Lan.g(this,"The amount you typed in")+": "+amt.ToString("C")+"\r\n"+Lan.g(this,"does not match the approved amount returned")
					+": "+approvedAmt.ToString("C")+".\r\n"+Lan.g(this,"The amount will be changed to reflect the approved amount charged."),"Alert",
					MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				textAmount.Text=approvedAmt.ToString("F");
			}
			if(xAdjust) {
				XchargeMilestone="Check Adjust";
				MessageBox.Show(Lan.g(this,"The amount will be changed to the X-Charge approved amount")+": "+approvedAmt.ToString("C"));
				textNote.Text="";
				textAmount.Text=approvedAmt.ToString("F");
			}
			else if(xReturn) {
				XchargeMilestone="Check Return";
				textAmount.Text="-"+approvedAmt.ToString("F");
			}
			else if(xVoid) {//For prepaid cards, tranType is set to 0 "Purchase", therefore xVoid will be false.
				XchargeMilestone="Check Void";
				HandleVoidPayment(resultText,approvedAmt,receipt,CreditCardSource.XServer);
				return resultText;
			}
			XchargeMilestone="Check Additional Funds";
			_wasCreditCardSuccessful=!_isCCDeclined;//If the transaction is not a void transaction, we will void this transaction if the user hits Cancel
			_payment.IsCcCompleted=_wasCreditCardSuccessful;
			if(additionalFunds>0) {
				MessageBox.Show(Lan.g(this,"Additional funds required")+": "+additionalFunds.ToString("C"));
			}
			if(textNote.Text!="") {
				textNote.Text+="\r\n";
			}
			textNote.Text+=resultText;
			XchargeMilestone="Receipt";
			_payment.Receipt=receipt;
			if(!string.IsNullOrEmpty(receipt)) {
				butPrintReceipt.Visible=true;
				if(PrefC.GetBool(PrefName.AllowEmailCCReceipt)) {
					butEmailReceipt.Visible=true;
				}
				if(_doPrintReceipt && prepaidAmt==0) {
					PrintReceipt(receipt,Lan.g(this,"X-Charge receipt printed"));
				}
			}
			XchargeMilestone="Reselect Credit Card in Combo";
			if(creditCard!=null && !string.IsNullOrEmpty(creditCard.XChargeToken) && creditCard.CCExpiration!=null) {
				//Refresh comboCreditCards and select the index of the card used for this payment if the token was saved
				listCreditCards=CreditCards.Refresh(_patient.PatNum);
				AddCreditCardsToCombo(listCreditCards,x => x.XChargeToken==creditCard.XChargeToken
					&& x.CCExpiration.Year==creditCard.CCExpiration.Year
					&& x.CCExpiration.Month==creditCard.CCExpiration.Month);
			}
			textReader.Dispose();
			return resultText;
		}

		///<summary>Returns null upon failure, otherwise returns the transaction detail as a string.</summary>
		public string MakeEdgeExpressTransaction(double prepaidAmt=0,long clinicNum=-1) {
			if(!HasEdgeExpress()) {
				return null;
			}
			if(!ValidateForCreditCardPayment(prepaidAmt!=0,out CreditCard cc)) {
				return null;
			}
			using FormEdgeExpressTrans formEdgeExpressTrans=new FormEdgeExpressTrans();
			//If this is a prepaid card don't give the option to save the token.
			formEdgeExpressTrans.DoShowSaveTokenBox=prepaidAmt==0;
			formEdgeExpressTrans.ShowDialog();
			if(formEdgeExpressTrans.DialogResult!=DialogResult.OK) {
				return null;
			}
			if(prepaidAmt==0) {
				Program program=Programs.GetCur(ProgramName.EdgeExpress);
				string payType=ProgramProperties.GetPropVal(program.ProgramNum,ProgramProperties.PropertyDescs.EdgeExpress.PaymentType,_payment.ClinicNum);//payType could be an empty string
				if(!PrefC.GetBool(PrefName.PaymentsPromptForPayType)) { 
					listPayType.SelectedIndex=Defs.GetOrder(DefCat.PaymentTypes,PIn.Long(payType));
				}
			}
			_payment.ProcessStatus=ProcessStat.OfficeProcessed;
			EdgeExpressTransType edgeExpressTransType=formEdgeExpressTrans.EdgeExpressTransTypeCur;
			_doPromptSignature=formEdgeExpressTrans.DoPromptSignature;
			_doPrintReceipt=formEdgeExpressTrans.DoPrintReceipt;
			string aliasToken=cc?.XChargeToken;
			bool doCreateToken=formEdgeExpressTrans.DoSaveToken && aliasToken.IsNullOrEmpty() && prepaidAmt==0;
			double amt=PIn.Double(textAmount.Text);
			if(prepaidAmt!=0) {
				amt=prepaidAmt;
			}
			string transactionId=formEdgeExpressTrans.TransactionId;
			decimal cashBackAmt=formEdgeExpressTrans.AmtCashBack;
			EdgeExpressApiType edgeExpressApiTypeSelection=formEdgeExpressTrans.EdgeExpressApiTypeCur;
			if(amt<0) {//EdgeExpress always wants a positive number, even for returns.
				amt*=-1;
			}
			string note=null;
			//Web entry - CNP
			if(edgeExpressApiTypeSelection==EdgeExpressApiType.Web) {
				try {
					note=MakeEdgeExpressTransactionCNP(edgeExpressTransType,amt,doCreateToken,aliasToken,transactionId,prepaidAmt,clinicNum);
				}
				catch(Exception ex) {
					FormFriendlyException formFriendlyException=new FormFriendlyException("Error processing EdgeExpress request",ex,false);
					formFriendlyException.ShowDialog();
					return null;
				}
				_payment.PaymentSource=CreditCardSource.EdgeExpressCNP;
			}
			//Terminal entry - RCM
			else if(edgeExpressApiTypeSelection==EdgeExpressApiType.Terminal) {
				try {
					note=MakeEdgeExpressTransactionRCM(edgeExpressTransType,amt,doCreateToken,aliasToken,transactionId,cashBackAmt,cc);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Error processing transaction.\r\n\r\nPlease contact support with the details of this error."),ex);
				}
				_payment.PaymentSource=CreditCardSource.EdgeExpressRCM;
			}
			return note;
		}
		#endregion

		///<summary>Deletes all payment splits in the grid.</summary>
		private void butDeleteAllSplits_Click(object sender,EventArgs e) {
			gridSplits.SetAll(true);
			DeleteSelected();
		}

		private void butDeletePayment_Click(object sender,System.EventArgs e) {
			if(textDeposit.Visible) {//this will get checked again by the middle layer
				MsgBox.Show(this,"This payment is attached to a deposit.  Not allowed to delete.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will delete the entire payment and all splits.")) {
				return;
			}
			//If payment is attached to a transaction which is more than 48 hours old, then not allowed to delete.
			//This is hard coded.  User would have to delete or detach from within transaction rather than here.
			Transaction transaction=Transactions.GetAttachedToPayment(_payment.PayNum);
			if(transaction != null) {
				if(transaction.DateTimeEntry < MiscData.GetNowDateTime().AddDays(-2)) {
					MsgBox.Show(this,"Not allowed to delete.  This payment is already attached to an accounting transaction.  You will need to detach it from "
						+"within the accounting section of the program.");
					return;
				}
				if(Transactions.IsReconciled(transaction)) {
					MsgBox.Show(this,"Not allowed to delete.  This payment is attached to an accounting transaction that has been reconciled.  You will need "
						+"to detach it from within the accounting section of the program.");
					return;
				}
				try {
					Transactions.Delete(transaction);
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			try {
				Payments.Delete(_payment);
			}
			catch(ApplicationException ex) {//error if attached to deposit slip
				MessageBox.Show(ex.Message);
				return;
			}
			if(!IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_payment.PatNum,"Delete for: "+Patients.GetLim(_payment.PatNum).GetNameLF()+", "
					+_paymentOld.PayAmt.ToString("c")+", with payment type '"+Payments.GetPaymentTypeDesc(_paymentOld,_listDefsPaymentType)+"'",
					0,_paymentOld.SecDateTEdit);
			}
			DialogResult=DialogResult.OK;
		}

		private void butDeleteSplits_Click(object sender,EventArgs e) {
			DeleteSelected();
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!SavePaymentToDb()) {
				return;
			}
			DialogResult=DialogResult.OK;
			Plugins.HookAddCode(this,"FormPayment.butOK_Click_end",_payment,_listPaySplits);
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Plugins.HookAddCode(this,"FormPayment.butCancel_Click_end");
		}

		private void FormPayment_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			//make additional logging so user knows changes they had just made to any paysplits were rolled back.
			//individual audit trails for splits
			for(int i=0;i<_listPaySplits.Count;i++) {
				PaySplit paySplit=_listPaySplitsOld.FirstOrDefault(x => x.SplitNum==_listPaySplits[i].SplitNum);
				string secLogText="Payment changes canceled:";
				string changesMade="";
				if(paySplit==null) {//null when splits are new
					secLogText="New paysplit canceled.";
					SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_listPaySplits[i].PatNum,secLogText,0);
					continue;
				}
				changesMade+=SecurityLogEntryHelper(Providers.GetAbbr(paySplit.ProvNum),Providers.GetAbbr(_listPaySplits[i].ProvNum),"Provider");
				changesMade+=SecurityLogEntryHelper(Clinics.GetAbbr(paySplit.ClinicNum),Clinics.GetAbbr(_listPaySplits[i].ClinicNum),"Clinic");
				changesMade+=SecurityLogEntryHelper(paySplit.SplitAmt.ToString("F"),_listPaySplits[i].SplitAmt.ToString("F"),"Amount");
				changesMade+=SecurityLogEntryHelper(paySplit.PatNum.ToString(),_listPaySplits[i].PatNum.ToString(),"Patient number");
				if(changesMade!="") {
					SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_listPaySplits[i].PatNum,secLogText+changesMade,0,paySplit.SecDateTEdit);
				}
			}
			if(!IsNew && !_wasCreditCardSuccessful) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!_wasCreditCardSuccessful) {//new payment that was not a credit card payment that has already been processed
				try {
					Payments.Delete(_payment);
				}
				catch(ApplicationException ex) {
					MsgBox.Show(ex.Message);
					e.Cancel=true;//they must either OK the payment and complete, or go back and detach the deposit that was just made. 
					return;
				}
				DialogResult=DialogResult.Cancel;
				return;
			}
			//Successful CC payment
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will void the transaction that has just been completed. Are you sure you want to continue?")) {
				e.Cancel=true;//Stop the form from closing
				return;
			}
			//make user select a payment type, prevents exception when trying to set payment type of voided CC payment.
			if(!checkPayTypeNone.Checked && (listPayType.SelectedIndex==-1 || listPayType.SelectedIndex>=_listDefsPaymentType.Count)) {
				MsgBox.Show(this,"A payment type must be selected.");
				e.Cancel=true;//Stop the form from closing
				return;
			}
			DateTime datePay=PIn.Date(textDate.Text);
			if(datePay==null || datePay==DateTime.MinValue) {
				MsgBox.Show(this,"Invalid Payment Date");
				e.Cancel=true;//Stop the form from closing
				return;
			}
			if(datePay.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.AccountAllowFutureDebits) && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Payment Date must not be a future date.");
				e.Cancel=true;//Stop the form from closing
				return;
			}
			//Save the credit card transaction as a new payment
			_payment.PayAmt=PIn.Double(textAmount.Text);//handles blank
			_payment.PayDate=datePay;
			_payment.CheckNum=textCheckNum.Text;
			_payment.BankBranch=textBankBranch.Text;
			_payment.IsRecurringCC=false;
			_payment.PayNote=textNote.Text;
			if(checkPayTypeNone.Checked) {
				_payment.PayType=0;
			}
			else {
				_payment.PayType=_listDefsPaymentType[listPayType.SelectedIndex].DefNum;
			}
			if(_listPaySplits.Count==0) {
				AddOneSplit();
				//FillMain();
			}
			else if(_listPaySplits.Count==1//if one split
				&& _listPaySplits[0].PayPlanNum!=0//and split is on a payment plan
				&& _listPaySplits[0].SplitAmt!=_payment.PayAmt)//and amount doesn't match payment
			{
				_listPaySplits[0].SplitAmt=_payment.PayAmt;//make amounts match automatically
				textSplitTotal.Text=textAmount.Text;
			}
			if(_payment.PayAmt!=_listPaySplits.Sum(x=>x.SplitAmt)) {
				MsgBox.Show(this,"Split totals must equal payment amount.");
				DialogResult=DialogResult.None;
				e.Cancel=true;//Stop the form from closing
				return;
			}
			if(_listPaySplits.Count>1) {
				_payment.IsSplit=true;
			}
			else {
				_payment.IsSplit=false;
			}
			try {
				Payments.Update(_payment,true);
			}
			catch(ApplicationException ex) {//this catches bad dates.
				MessageBox.Show(ex.Message);
				e.Cancel=true;
				return;
			}
			//Set all DatePays the same.
			for(int i=0;i<_listPaySplits.Count;i++) {
				_listPaySplits[i].DatePay=_payment.PayDate;
			}
			bool hasChanged=PaySplits.Sync(_listPaySplits,_listPaySplitsOld);
			if(IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.PaymentCreate,_payment.PatNum,Payments.GetSecuritylogEntryText(_payment,_paymentOld,IsNew,
					_listDefsPaymentType));
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.PaymentEdit,_payment.PatNum,Payments.GetSecuritylogEntryText(_payment,_paymentOld,IsNew,
					_listDefsPaymentType),0,_paymentOld.SecDateTEdit);
			}
			if(hasChanged) {
				string strErrorMsg=Ledgers.ComputeAgingForPaysplitsAllocatedToDiffPats(_patient.PatNum,_listPaySplits.Union(_listPaySplitsOld).ToList());
				if(!string.IsNullOrEmpty(strErrorMsg)) {
					MessageBox.Show(strErrorMsg);
				}
			}
			string refNum="";
			string amount="";
			string transactionID="";
			string paySimplePaymentId="";
			bool isDebit=false;
			bool isEdgeExpress=false;
			string[] stringArrayNoteFields=textNote.Text.Replace("\r\n","\n").Replace("\r","\n").Split("\n",StringSplitOptions.RemoveEmptyEntries);
			List<string> listNoteFields=stringArrayNoteFields.ToList();
			for(int i=0;i<listNoteFields.Count;i++) {
				if(listNoteFields[i].StartsWith("Amount: ")) {
					amount=listNoteFields[i].Substring(8);
				}
				if(listNoteFields[i].StartsWith("Ref Number: ")) {
					refNum=listNoteFields[i].Substring(12);
				}
				if(listNoteFields[i].StartsWith("XCTRANSACTIONID=")) {
					transactionID=listNoteFields[i].Substring(16);
				}
				if(listNoteFields[i].StartsWith("TRANSACTIONID: ")) {//EdgeExpress
					transactionID=StringTools.SubstringAfter(listNoteFields[i],"TRANSACTIONID: ");
					isEdgeExpress=true;
				}
				if(listNoteFields[i].StartsWith("Transaction ID: ")) {//EdgeExpress
					transactionID=StringTools.SubstringAfter(listNoteFields[i],"Transaction ID: ");
					isEdgeExpress=true;
				}
				if(listNoteFields[i].StartsWith("APPROVEDAMOUNT=")) {
					amount=listNoteFields[i].Substring(15);
				}
				if(listNoteFields[i].StartsWith("TYPE=") && listNoteFields[i].Substring(5)=="Debit Purchase") {
					isDebit=true;
				}
				if(listNoteFields[i].StartsWith(Lan.g("PaySimple","PaySimple Transaction Number"))) {
					paySimplePaymentId=listNoteFields[i].Split(':')[1].Trim();//Better than substring 28, because we do not know how long the translation will be.
				}
			}
			if(refNum!="") {//Void the PayConnect transaction if there is one
				VoidPayConnectTransaction(refNum,amount);
			}
			else if(transactionID!="" && isEdgeExpress) {
				VoidEdgeExpressTransaction(transactionID);
			}
			else if(transactionID!="" && HasXCharge()) {//Void the X-Charge transaction if there is one
				VoidXChargeTransaction(transactionID,amount,isDebit);
			}
			else if(!string.IsNullOrWhiteSpace(paySimplePaymentId)) {
				string originalReceipt=_payment.Receipt;
				if(_apiResponsePaySimple!=null) {
					originalReceipt=_apiResponsePaySimple.TransactionReceipt;
				}
				VoidPaySimpleTransaction(paySimplePaymentId,originalReceipt);
			}
			else {
				MsgBox.Show(this,"Unable to void transaction");
			}
			DialogResult=DialogResult.Cancel;
		}

		///<summary>Helper struct to accommodate equating PaySplit objects to one another.
		///This window shows PaySplits in one grid and AccountEntries in another grid and selections in one grid need to affect the other.
		///Matching is done by looking for and selecting any related entries to the PaySplit in question.
		///This helper struct is required because editing old payments won't have equating PaySplit objects but will have equating SplitNums
		///and new PaySplits won't have equating SplitNums but will have equating TagOD objects.  Both need to be supported at the same time.
		///E.g. if a PaySplit is selected, loop through the account entries in the other grid looking for a match in the SplitCollection.</summary>
		private struct PaySplitHelper {
			///<summary>The primary key of PaySplitCur.  Can be 0 when the PaySplit is new (user just added it and hasn't been inserted yet).</summary>
			public long SplitNum;
			///<summary>The GUID from the TagOD on the PaySplit object that this PaySplitHelper represents.</summary>
			public string SplitGUID;

			public PaySplitHelper(PaySplit paySplitCur) {
				SplitNum=paySplitCur.SplitNum;
				SplitGUID=(string)paySplitCur.TagOD;
			}

			public override bool Equals(object obj) {
				if(!(obj is PaySplitHelper)) {
					return false;
				}
				if(SplitNum > 0) {
					return SplitNum.Equals(((PaySplitHelper)obj).SplitNum);
				}
				return SplitGUID.Equals(((PaySplitHelper)obj).SplitGUID);
			}

			public override int GetHashCode() {
				if(SplitNum > 0) {
					return SplitNum.GetHashCode();
				}
				return SplitGUID.GetHashCode();
			}
		}


	}
}
