using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Bridges;
using OpenDentBusiness;
using OpenDentBusiness.PayConnectService;
using static OpenDentBusiness.PayConnect2;
using PayConnectService=OpenDentBusiness.PayConnectService;

namespace OpenDental {
	public partial class FormPayConnect2:FormODBase {
		private Patient _patient;
		public string ReceiptStr;
		private CreditCard _creditCard;
		///<summary>For iFrame only.</summary>
		private bool _isAddingCard;
		private long _clinicNum;
		private decimal _amount;
		private Program _program;
		private PayConnectResponse _response;
		public PayConnectService.transType TransType=PayConnectService.transType.SALE;//Backwards compatability with PayConnect v1
		///<summary>Opening the PayConnect or PaySimple window from FormPayment, and then closing them, can set isCcDeclined to True.
		///This is because FormPayment didn't know if a transaction was attempted or not, and was assuming it was.
		///This can cause the payment amount to be reset to $0. So, this bool indicates if we have actually attempted a transaction.</summary>
		public bool WasPaymentAttempted=false;

		public FormPayConnect2(long clinicNum,Patient patient,CreditCard creditCard,decimal amount,bool isAddingCard=false) {
			_clinicNum=clinicNum;
			_creditCard=creditCard;
			_patient=patient;
			_isAddingCard=isAddingCard;
			_amount=amount;
			_response=new PayConnectResponse();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayConnect2_Load(object sender,EventArgs e) {
			_program=Programs.GetCur(ProgramName.PayConnect);
			if(_program==null) {
				MsgBox.Show(this,"PayConnect does not exist in the database.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(ODEnvironment.IsCloudServer){
				sigBoxWrapper.Enabled=false;
			}
			comboTerminal.Items.Clear();
			List<PayTerminal> listPayTerminals = PayTerminals.Refresh(_clinicNum);
			for(int i = 0;i<listPayTerminals.Count;i++) {
				string name = listPayTerminals[i].Name;
				if(name.IsNullOrEmpty()) {
					name=listPayTerminals[i].TerminalID;
				}
				comboTerminal.Items.Add(name,listPayTerminals[i]);
				if(i==0) {
					comboTerminal.SelectedIndex=0;
				}
			}
			textAmount.Text=_amount.ToString();
			//Hide it here to override the enable/disable changes between terminal/webservice checked methods.
			checkSaveToken.Visible=!PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum));
			if(_patient==null) {//Prepaid card
				radioAuthorization.Enabled=false;
				radioVoid.Enabled=false;
				radioRefund.Enabled=false;
				checkSaveToken.Visible=false;
				sigBoxWrapper.Enabled=false;
			}
			else {//Other cards
				checkSaveToken.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
			}
			if(!PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,"TerminalProcessingEnabled",_clinicNum))){
				groupProcessMethod.Visible=false;
				//it is hidden but is still "checked" so the process transaction method knows which service to use.
				radioWebService.Checked=true;
			}
			else {
				string procMethod=ProgramProperties.GetPropValForClinicOrDefault(_program.ProgramNum,
					PayConnect.ProgramProperties.DefaultProcessingMethod,_clinicNum);
				if(procMethod=="0") {
					radioWebService.Checked=true;
				}
				else if(procMethod=="1") {
					radioTerminal.Checked=true;
				}
			}
			if(radioWebService.Checked) {
				comboTerminal.Enabled=false;
			}
			else if(radioTerminal.Checked) {
				comboTerminal.Enabled=true;
			}
		}

		/// <summary>Returns -1 if an error occured. Takes whatever is in textAmount and tries to prepare it for use with PayConnect.</summary>
		private int GetAmountFieldAsCents() {
			if(textAmount.Text.IsNullOrEmpty()){
				return 0;
			}
			double amount=textAmount.Value;
			try {
				return PayConnect2.FormatAmountForApi(amount);
			}
			catch(Exception ex) { 
				ex.DoNothing();
				MsgBox.Show(this,"Transaction amount is formatted incorrectly.");
				return -1;
			}
		}

		/// <summary>Returns true if we contacted and got a response from PayConnect.</summary>
		private bool ProcessTransaction() {
			WasPaymentAttempted=true;
			PayConnect2Response payConnect2Response=new PayConnect2Response();
			int amountInCents=GetAmountFieldAsCents();
			if(radioTerminal.Checked) {
				if(comboTerminal.SelectedItem==null) {
					MsgBox.Show(this,"Please select a terminal from the dropdown.");
					return false;
				}
				string terminalId=((PayTerminal)comboTerminal.SelectedItem).TerminalID;
				decimal amount=(decimal)textAmount.Value;//textAmount will be a value between 0 and 100,000,000
				if(amount>0) {
					CreateTerminalTransactionRequest request=new CreateTerminalTransactionRequest();
					request.Amount=amountInCents;
					request.Terminal=terminalId;
					request.Frequency=TransactionFrequency.OneTime;
					request.Signature=true;//For now always request signature on the terminal, PayConnect will handle if the terminal does not support signatures.
					if(radioSale.Checked) {
						request.TransType=TransactionType.Sale;
						TransType=PayConnectService.transType.SALE;
					}
					else if(radioAuthorization.Checked) {
							request.TransType=TransactionType.AuthorizeOnly;
							TransType=PayConnectService.transType.AUTH;
					}
					UI.ProgressWin progressOD=new UI.ProgressWin();
					progressOD.ActionMain=()=> {payConnect2Response=PayConnect2.PostCreateTerminalTransaction(request,_clinicNum);};
					progressOD.ShowCancelButton=false;//This is neccessary as cancelling on our side will not cancel the transaction on the terminal, which could lead to the payment being processed but not show in the patients account. This also matches what we do in FormPayConnect.cs.
					progressOD.StartingMessage=Lan.g(this,"Processing payment on terminal");
					progressOD.StopNotAllowedMessage=Lan.g(this,"Not allowed to stop. Please wait up to 2 minutes.");
					try{
						progressOD.ShowDialog();
					}
					catch(Exception ex){
						SecurityLogs.MakeLogEntry(EnumPermType.CreditCardTerminal,_patient.PatNum,"No response received.");
						MessageBox.Show(Lan.g(this,"A payment was initiated but no response was received. The payment may or may not have processed."
							+" Verify payment with your Credit Card merchant."),ex.Message);
						return false;
					}
					if(progressOD.IsCancelled){
						return false;
					}
				}
				else {
						MsgBox.Show(this,"Amount must be greater than 0.00");
						return false;
				}
				//The New Terminal API endpoint only allows for Sale and Auths, keeping the below block of code around incase PayConnect adds functionality later.
				//else if(radioVoid.Checked) {
				//	if(textRefNumber.Text.IsNullOrEmpty()) {
				//		MsgBox.Show(this,"Reference Number is required to void a transaction.");
				//		return false;
				//	}
				//	_response=PayConnect2L.CreateVoid(textRefNumber.Text,apiKey,terminalId,forceDuplicate:checkForceDuplicate.Checked);
				//	TransType=PayConnectService.transType.VOID;
				//}
				//else if(radioRefund.Checked) {
				//	if(textRefNumber.Text.IsNullOrEmpty()) {
				//		MsgBox.Show(this,"Reference Number is required to refund a transaction.");
				//		return false;
				//	}
				//	if(amount>0) {
				//		_response=PayConnect2L.CreateRefund(amount,apiKey,terminalId,textRefNumber.Text,forceDuplicate:checkForceDuplicate.Checked);
				//		TransType=PayConnectService.transType.RETURN;
				//	}
				//	else {
				//		MsgBox.Show(this,"Amount must be greater than 0.00");
				//		return false;
				//	}
				//}
			}
			else if(radioWebService.Checked) {
				if(radioSale.Checked) {
					if(amountInCents < 0) {
						MsgBox.Show(this,"Amount must be greater than 0.00");
						return false;
					}
					if(_creditCard==null || _creditCard.PayConnectToken.IsNullOrEmpty()) {
						using FormPayConnect2iFrame formPayConnect2IFrame=new FormPayConnect2iFrame(_clinicNum,amountInCents,_isAddingCard);
						formPayConnect2IFrame.ShowDialog();
						payConnect2Response=formPayConnect2IFrame.GetResponse();
						if(payConnect2Response.GetStatusResponse==null) {
							return false;
						}
					}
					//We have a saved card with a token
					else {
						try {
							payConnect2Response=PayConnect2.PostCreateTransactionByToken(_patient,_creditCard,amountInCents,_clinicNum);
						}
						catch(Exception ex) {
							ex.DoNothing();
							MsgBox.Show(this,"Invalid data, unable to send sale transaction");
							return false;
						}
					}
					TransType=PayConnectService.transType.SALE;
				}
				else if(radioAuthorization.Checked) {
					if(_creditCard==null || _creditCard.PayConnectToken.IsNullOrEmpty()) { 
						MsgBox.Show(this,"A saved card is required to run an authorization transaction. Return to the previous window and select a saved card.");
						return false;
					}
					if(amountInCents<0) {
						MsgBox.Show(this,"Amount must be greater than 0.00");
						return false;
					}
					try {
						payConnect2Response=PayConnect2.PostCreateTransactionByToken(_patient,_creditCard,amountInCents,_clinicNum,PayConnect2.TransactionType.AuthorizeOnly);
					}
					catch(Exception ex) {
							ex.DoNothing();
							MsgBox.Show(this,"Invalid data, unable to send auth transaction");
							return false;
					}
					TransType=PayConnectService.transType.AUTH;
				}
				else if(radioRefund.Checked) {
					if(textRefNumber.Text.IsNullOrEmpty()) { 
						MsgBox.Show(this,"Reference Number is required to refund a transaction.");
						return false;
					}
					RefundReferenceIDRequest refundReferenceIDRequest=new RefundReferenceIDRequest();
					refundReferenceIDRequest.ReferenceId=textRefNumber.Text;
					//Amount can be used to do partial refunds. Not including the amount field will refund the entire amount of the original transaction.
					if(amountInCents>0) {
						refundReferenceIDRequest.Amount=amountInCents;
					}
					payConnect2Response=PayConnect2.PostRefundWithReferenceID(refundReferenceIDRequest,_clinicNum);
					TransType=PayConnectService.transType.RETURN;
				}
				else if(radioVoid.Checked) {
					if(textRefNumber.Text.IsNullOrEmpty()) { 
						MsgBox.Show(this,"Reference Number is required to void a transaction.");
						return false;
					}
					VoidReferenceIDRequest voidReferenceIDRequest=new VoidReferenceIDRequest();
					voidReferenceIDRequest.ReferenceId=textRefNumber.Text;
					payConnect2Response=PayConnect2.PutVoidWithReferenceID(voidReferenceIDRequest,_clinicNum);
					TransType=PayConnectService.transType.VOID;
				}	
			}
			_response=PayConnect2.ApiResponseToPayConnectResponse(payConnect2Response);
			bool doShowSignatureLine=true;
			if(TransType==transType.SALE || TransType==transType.AUTH) {
				PayConnect2Response signatureResponse=SendSignature();
				if(signatureResponse!=null && signatureResponse.SignatureResponse!=null && signatureResponse.SignatureResponse.Status=="Processed") {
					doShowSignatureLine=false;
				}
			}
			ReceiptStr=PayConnect.BuildReceiptString(TransType,_response.RefNumber,_patient.GetNameFLnoPref(),_response.CardNumber,magData:"",_response.AuthCode,_response.Description,messages:null,_response.Amount,doShowSignatureLine,_clinicNum,_response.CardType);
			return true;
		}

		/// <summary>Does validation before sending. Will not make an API call if a signature has not been entered.</summary>
		private PayConnect2Response SendSignature() {
			if(!sigBoxWrapper.GetSigChanged() || string.IsNullOrEmpty(sigBoxWrapper.GetSignature(""))) {
				return null;
			}
			AddSignatureReferenceIDRequest addSignatureRequest=new AddSignatureReferenceIDRequest();
			addSignatureRequest.ReferenceId=_response.RefNumber;
			using(Bitmap bitmapSigImage=sigBoxWrapper.GetSigImage())
			using(MemoryStream memoryStream=new MemoryStream()) {
				bitmapSigImage.Save(memoryStream,ImageFormat.Jpeg);
				byte[] byteArrayImageBytes=memoryStream.ToArray();
				addSignatureRequest.Signature=Convert.ToBase64String(byteArrayImageBytes);
			}
			return PayConnect2.PutSignatureWithReferenceID(addSignatureRequest,_clinicNum);
		}

		///<summary>Checks all UI state and preconditions needed to save a card.</summary>
		private void SaveCreditCard() {
			if(!checkSaveToken.Checked || !checkSaveToken.Enabled || !checkSaveToken.Visible) { 
				return;
			}
			if(_response.StatusCode!="0") {
				//error occured, let calling form handle it.
				return;
			}
			PayConnect2Response statusResponse=PayConnect2.GetTransactionStatus(_clinicNum,_response.RefNumber);
			if(statusResponse.GetStatusResponse.PaymentMethod.CardPaymentMethod==null) {
				return;
			}
			CardPaymentMethod cardPaymentMethod=statusResponse.GetStatusResponse.PaymentMethod.CardPaymentMethod;
			if(_creditCard==null) {//user selected Add new card from the payment window, save it or its token depending on settings
				_creditCard=new CreditCard();
				_creditCard.IsNew=true;
				_creditCard.PatNum=_patient.PatNum;
				List<CreditCard> listCreditCardsItemOrderCount=CreditCards.RefreshAll(_patient.PatNum);
				_creditCard.ItemOrder=listCreditCardsItemOrderCount.Count;
			}
			DateTime expiryDate;
			bool didParse=DateTime.TryParseExact(cardPaymentMethod.Expiry,new string[] {"MMyy","yyyyMM"},CultureInfo.CurrentCulture.DateTimeFormat,DateTimeStyles.None,out expiryDate);
			if(didParse) {
				_creditCard.CCExpiration=expiryDate;
			}
			_creditCard.CCNumberMasked=cardPaymentMethod.CardLast4Digits.PadLeft(16,'X');
			_creditCard.Zip=cardPaymentMethod.ZipCode;
			_creditCard.PayConnectToken="";
			_creditCard.PayConnectTokenExp=expiryDate;//Token expiration is the same as CC expiration.
			_creditCard.PayConnectToken=_response.PaymentToken;
			_creditCard.CCSource=CreditCardSource.PayConnect;
			if(_creditCard.IsNew) {
				_creditCard.ClinicNum=_clinicNum;
				_creditCard.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
				CreditCards.Insert(_creditCard);
				SecurityLogs.MakeLogEntry(EnumPermType.CreditCardEdit,_patient.PatNum,"Credit Card Added");
			}
			else {
				if(_creditCard.CCSource==CreditCardSource.XServer) {//This card has also been added for XCharge.
					_creditCard.CCSource=CreditCardSource.XServerPayConnect;
				}
				CreditCards.Update(_creditCard);
			}
		}

		///<summary>Only call this method once FormPayConnect2 has closed and the dialog result is OK. Uses the old PayConnectResponse class for backwards compatability.</summary>
		public PayConnectResponse GetPayConnectResponse() {
			return _response;
		}
		
		private void radioTerminal_CheckedChanged(object sender,EventArgs e) {
			if(radioTerminal.Checked) {
				if(radioRefund.Checked || radioVoid.Checked) {
					MsgBox.Show(this,"Not allowed to send refunds or voids via Terminal. Please select Web Service to process a refund or void.");
					radioWebService.Checked=true;
					return;
				}
				comboTerminal.Enabled=true;
				radioVoid.Enabled=false;
				radioRefund.Enabled=false;
				
			}
		}

		private void radioWebService_CheckedChanged(object sender,EventArgs e) {
			if(radioWebService.Checked) {
				checkSaveToken.Enabled=true;
				comboTerminal.Enabled=false;
				radioVoid.Enabled=true;
				radioRefund.Enabled=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textAmount.IsValid()) {
				MsgBox.Show(this,"Please enter a valid amount or clear the amount field.");
				return;
			}
			if(!ProcessTransaction()) {
				return;
			}
			if(TransType==transType.SALE || TransType==transType.AUTH) {
				try {
					SaveCreditCard();
				}
				catch(Exception ex) {
					FriendlyException.Show("Unable to save card to database.",ex);
				}
			}
			if(!ReceiptStr.IsNullOrEmpty()) {
				PayConnectL.PrintReceipt(ReceiptStr,_patient);
			}
			DialogResult=DialogResult.OK;
		}

		///<summary>Not able to save a card during a void or refund</summary>
		private void transactionTypeCheckedChanged(object sender,EventArgs e) {
			if(radioVoid.Checked || radioRefund.Checked) {
				checkSaveToken.Visible=false;
				return;
			}
			checkSaveToken.Visible=true;
		}

	}
}