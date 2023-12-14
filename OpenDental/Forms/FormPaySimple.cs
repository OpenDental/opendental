using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using CodeBase;
using MigraDoc.DocumentObjectModel;

namespace OpenDental {
	public partial class FormPaySimple:FormODBase {

		///<summary>Only to be used outside of the form.  
		///Set on OK_Click to discourage using it in the form.
		///Contains the important details of what happened with PaySimple</summary>
		public PaySimple.ApiResponse ApiResponseOut;
		///<summary>Opening the PayConnect or PaySimple window from FormPayment, and then closing them, can set isCcDeclined to True.
		///This is because FormPayment didn't know if a transaction was attempted or not, and was assuming it was.
		///This can cause the payment amount to be reset to $0. So, this bool indicates if we have actually attempted a transaction.</summary>
		public bool WasPaymentAttempted=false;

		private Patient _patient;
		private MagstripCardParser _magstripCardParser=null;
		private PaySimple.TransType _transType=PaySimple.TransType.SALE;
		private CreditCard _creditCard;
		private bool _isAddingCard;
		private long _clinicNum;
		private Program _program;
		///<summary>Some card readers have CR/Enter track separators, 
		///which would cause our parsing logic to happen before the magstripe reader finished outputting the data.
		///We add a timer delay to attempt to compensate for this functionality.</summary>
		private bool _hasSwipedCard;
		private string _carrierName;

		public FormPaySimple(long clinicNum,Patient patient,decimal amount,CreditCard creditCard,bool isAddingCard=false,string carrierName="") {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textAmount.Text=POut.Decimal(amount);
			textAmountACH.Text=POut.Decimal(amount);
			_clinicNum=clinicNum;
			_patient=patient;
			_creditCard=creditCard;
			_isAddingCard=isAddingCard;
			_carrierName=carrierName;
		}

		private void FormPaySimple_Load(object sender,EventArgs e) {
			_program=Programs.GetCur(ProgramName.PaySimple);
			if(_program==null) {
				MsgBox.Show(this,"PaySimple does not exist in the database.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_patient==null || _patient.PatNum==0) {//Prepaid card
				radioAuthorization.Enabled=false;
				checkOneTimePayment.Checked=true;
				checkOneTimePayment.Enabled=false;
				tabControl.TabPages.Remove(tabACH);
			}
			else {
				checkOneTimePayment.Checked=!PrefC.GetBool(PrefName.StoreCCtokens);
				checkOneTimePaymentACH.Checked=!PrefC.GetBool(PrefName.StoreCCtokens);
				textZipCode.Text=_patient.Zip;
				textNameOnCard.Text=_patient.GetNameFL();
				if(_creditCard!=null) {
					FillFieldsFromCard();
				}
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePrintReceipt,_clinicNum))) {
				checkPrintReceipt.Checked=true;
				checkPrintReceiptACH.Checked=true;
			}
			if(_isAddingCard) {
				radioAuthorization.Checked=true;
				_transType=PaySimple.TransType.AUTH;
				groupTransType.Enabled=false;
				labelAmount.Visible=false;
				textAmount.Visible=false;
				labelAmountACH.Visible=false;
				textAmountACH.Visible=false;
				checkOneTimePayment.Checked=false;
				checkOneTimePayment.Enabled=false;
				checkOneTimePaymentACH.Checked=false;
				checkOneTimePaymentACH.Enabled=false;
				checkPrintReceipt.Checked=false;
				checkPrintReceipt.Enabled=false;
				checkPrintReceiptACH.Checked=false;
				checkPrintReceiptACH.Enabled=false;
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePreventSavingNewCC,_clinicNum))) {
				textCardNumber.ReadOnly=true;
				textRoutingNumber.ReadOnly=true;
				textCheckSaveNumber.ReadOnly=true;
				textBankName.ReadOnly=true;
			}
			if(_creditCard==null || _creditCard.IsPaySimpleACH()) {
				textCardNumber.Select();
				return;
			}
			tabControl.SelectedTab=tabACH;
			textRoutingNumber.Select();
		}

		private void FillFieldsFromCard() {
			//User selected a credit card from drop down.
			if(_creditCard.CCNumberMasked!="") {
				if(_creditCard.IsPaySimpleACH()) {
					textCheckSaveNumber.Text=_creditCard.CCNumberMasked;
				}
				else {
					string ccNum=_creditCard.CCNumberMasked;
					if(Regex.IsMatch(ccNum,"^\\d{12}(\\d{0,7})")) { //Minimum of 12 digits, maximum of 19
						int idxLast4Digits=(ccNum.Length-4);
						ccNum=(new string('X',12))+ccNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
					}
					textCardNumber.Text=ccNum;
				}
			}
			if(_creditCard.CCExpiration!=null && _creditCard.CCExpiration.Year>2005) {
				textExpDate.Text=_creditCard.CCExpiration.ToString("MMyy");
			}
			if(_creditCard.Zip!="") {
				textZipCode.Text=_creditCard.Zip;
			}
			if(!string.IsNullOrWhiteSpace(_creditCard.PaySimpleToken)) {
				checkOneTimePayment.Checked=false;
				checkOneTimePayment.Enabled=false;
				textSecurityCode.ReadOnly=true;
				textZipCode.ReadOnly=true;
				textNameOnCard.ReadOnly=true;
				textCardNumber.ReadOnly=true;
				textExpDate.ReadOnly=true;
				radioAuthorization.Enabled=false;
				textCheckSaveNumber.ReadOnly=true;
				textRoutingNumber.ReadOnly=true;
				textBankName.ReadOnly=true;
				checkOneTimePaymentACH.Checked=false;
				checkOneTimePaymentACH.Enabled=false;
				//We don't store whether it's a checkings or savings account.
				radioCheckings.Checked=false;
				radioSavings.Checked=false;
				groupBankAccountType.Enabled=false;
			}
			else if(!string.IsNullOrEmpty(_creditCard.XChargeToken) || !string.IsNullOrEmpty(_creditCard.PayConnectToken)) {
				//No token for this cc. Have the user enter the cc info.
				textCardNumber.Text="";
			}
		}

		private void radioSale_Click(object sender,EventArgs e) {
			radioSale.Checked=true;
			radioAuthorization.Checked=false;
			radioVoid.Checked=false;
			radioReturn.Checked=false;
			textRefNumber.Visible=false;
			labelRefNumber.Visible=false;
			textAmount.Visible=true;
			_transType=PaySimple.TransType.SALE;
			if(!checkPrintReceipt.Enabled) {
				checkPrintReceipt.Enabled=true;
				checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePrintReceipt,_clinicNum));
			}
			textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
		}

		private void radioAuthorization_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=true;
			radioVoid.Checked=false;
			radioReturn.Checked=false;
			textRefNumber.Visible=false;
			labelRefNumber.Visible=false;
			textAmount.Visible=false;
			_transType=PaySimple.TransType.AUTH;
			checkPrintReceipt.Checked=false;
			checkPrintReceipt.Enabled=false;
			textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
		}

		private void radioVoid_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=false;
			radioVoid.Checked=true;
			radioReturn.Checked=false;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Ref Number");
			textAmount.Visible=false;
			_transType=PaySimple.TransType.VOID;
			if(!checkPrintReceipt.Enabled) {
				checkPrintReceipt.Enabled=true;
				checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePrintReceipt,_clinicNum));
			}
			textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
		}

		private void radioReturn_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=false;
			radioVoid.Checked=false;
			radioReturn.Checked=true;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Ref Number");
			textAmount.Visible=false;
			_transType=PaySimple.TransType.RETURN;
			if(!checkPrintReceipt.Enabled) {
				checkPrintReceipt.Enabled=true;
				checkPrintReceipt.Checked=PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePrintReceipt,_clinicNum));
			}
			textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
		}

		private void textCardNumber_KeyPress(object sender,KeyPressEventArgs e) {
			if(String.IsNullOrEmpty(textCardNumber.Text)) {
				return;
			}
			if(textCardNumber.Text.StartsWith("%") && e.KeyChar == 13) {
				e.Handled=true;
				_hasSwipedCard=true;
			}
			//Restart the timer if data is still coming in after enter was pressed
			if(_hasSwipedCard) {
				timerParseCardSwipe.Stop();
				timerParseCardSwipe.Start();
			}
		}

		private void timerParseCardSwipe_Tick(object sender,EventArgs e) {
			timerParseCardSwipe.Stop();
			ParseSwipedCard(textCardNumber.Text);
			_hasSwipedCard=false;
		}

		private void ParseSwipedCard(string data) {
			Clear();
			try {
				_magstripCardParser=new MagstripCardParser(data);
			}
			catch(MagstripCardParseException) {
				MessageBox.Show(this,"Could not read card, please try again.","Card Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			if(_magstripCardParser!=null) {
				textCardNumber.Text=_magstripCardParser.AccountNumber;
				textExpDate.Text=_magstripCardParser.ExpirationMonth.ToString().PadLeft(2,'0')+(_magstripCardParser.ExpirationYear%100).ToString().PadLeft(2,'0');
				textNameOnCard.Text=_magstripCardParser.FirstName+" "+_magstripCardParser.LastName;
				GetNextControl(textNameOnCard,true).Focus();//Move forward to the next control in the tab order.
			}
		}

		private void Clear() {
			textCardNumber.Text="";
			textExpDate.Text="";
			textNameOnCard.Text="";
			textSecurityCode.Text="";
			textZipCode.Text="";
		}

		///<summary>Processes a PaySimple payment via the PaySimple API.</summary>
		private PaySimple.ApiResponse ProcessPayment(int expYear,int expMonth) {
			PaySimple.ApiResponse apiResponseRetVal=null;
			string refNumber="";
			if(_transType==PaySimple.TransType.VOID || _transType==PaySimple.TransType.RETURN) {
				refNumber=textRefNumber.Text;
			}
			string magData=null;
			if(_magstripCardParser!=null) {
				magData=_magstripCardParser.Track2;
			}
			string cardNumber=textCardNumber.Text;
			//if using a stored CC and there is an X-Charge token saved for the CC and the user enters the whole card number to get a PaySimple token
			//and the number entered doesn't have the same last 4 digits and exp date, then assume it's not the same card and clear out the X-Charge token.
			if(_creditCard!=null //using a saved CC
				&& !string.IsNullOrEmpty(_creditCard.XChargeToken) //there is an X-Charge token saved
				&& (StringTools.TruncateBeginning(cardNumber,4)!=StringTools.TruncateBeginning(_creditCard.CCNumberMasked,4) //the card number entered doesn't have the same last 4 digits
					|| expYear!=_creditCard.CCExpiration.Year //the card exp date entered doesn't have the same year
					|| expMonth!=_creditCard.CCExpiration.Month)) //the card exp date entered doesn't have the same month
			{
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"The card number or expiration date entered does not match the X-Charge card on file.  Do you wish "
					+"to replace the X-Charge card with this one?"))
				{
					_creditCard.XChargeToken="";
				}
				else {
					Cursor=Cursors.Default;
					return null;
				}
			}
			//if the user has chosen to store CC tokens and the stored CC has a token and the token is not expired,
			//then use it instead of the CC number and CC expiration.
			if(!checkOneTimePayment.Checked
				&& _creditCard!=null //if the user selected a saved CC
				&& !string.IsNullOrWhiteSpace(_creditCard.PaySimpleToken)) //there is a stored token for this card
			{
				cardNumber=_creditCard.PaySimpleToken;
				expYear=_creditCard.CCExpiration.Year;
				expMonth=_creditCard.CCExpiration.Month;
			}
			else if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePreventSavingNewCC,_clinicNum))) {
				MsgBox.Show(this,"Cannot add a new credit card.");
				return null;
			}
			try {
				switch(_transType) {
					case PaySimple.TransType.SALE:
						//If _patCur is null or the PatNum is 0, we will make a one time payment for an UNKNOWN patient.  
						//This is currently only intended for prepaid insurance cards.
						apiResponseRetVal=PaySimple.MakePayment((_patient==null ? 0 : _patient.PatNum),_creditCard,PIn.Decimal(textAmount.Text),textCardNumber.Text
							,new DateTime(expYear,expMonth,1),checkOneTimePayment.Checked,textZipCode.Text,textSecurityCode.Text,_clinicNum,_carrierName);
						break;
					case PaySimple.TransType.AUTH:
						//Will retreive a new customer id from PaySimple if the patient doesn't exist already.
						long paySimpleCustomerId=PaySimple.GetCustomerIdForPat(_patient.PatNum,_patient.FName,_patient.LName,_clinicNum);
						//I have no idea if an insurance can make an auth payment but incase they can I check for it.
						if(paySimpleCustomerId==0) {//Insurance payment, make a new customer id every time per Nathan on 04/26/2018
							if((_patient==null || _patient.PatNum==0)) {
								paySimpleCustomerId=PaySimple.AddCustomer("UNKNOWN","UNKNOWN","",_clinicNum);
							}
							else {
								throw new ODException(Lan.g(this,"Invalid PaySimple Customer Id found."));
							}
						}
						try {
							apiResponseRetVal=PaySimple.AddCreditCard(paySimpleCustomerId,textCardNumber.Text,new DateTime(expYear,expMonth,1),textZipCode.Text,_clinicNum);
						}
						catch(PaySimpleException ex) {
							PaySimple.HandlePaySimpleException(ex,paySimpleCustomerId);
						}
						break;
					case PaySimple.TransType.RETURN:
						if(string.IsNullOrWhiteSpace(textRefNumber.Text)) {
							throw new ODException(Lan.g(this,"Invalid PaySimple Payment ID."));
						}
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are about to return a payment.  This action is irreversible.  Continue?")) {
							throw new ODException(Lan.g(this,"Payment return was cancelled by user."));
						}
						apiResponseRetVal=PaySimple.ReversePayment(textRefNumber.Text,_clinicNum);
						break;
					case PaySimple.TransType.VOID:
						if(string.IsNullOrWhiteSpace(textRefNumber.Text)) {
							throw new ODException(Lan.g(this,"Invalid PaySimple Payment ID."));
						}
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are about to void a payment.  This action is irreversible.  Continue?")) {
							throw new ODException(Lan.g(this,"Payment void was cancelled by user."));
						}
						apiResponseRetVal=PaySimple.VoidPayment(textRefNumber.Text,_clinicNum);
						break;
					default:
						throw new Exception("Invalid transmission type: "+_transType.ToString());
				}
			}
			catch(PaySimpleException ex) {
				MessageBox.Show(ex.Message);
				if(ex.ErrorType==PaySimpleError.CustomerDoesNotExist && MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"Delete the link to the customer id for this patient?")) 
				{
					PatientLinks.DeletePatNumTos(ex.CustomerId,PatientLinkType.PaySimple);
				}
				return null;
			}
			catch(ODException wex) {
				MessageBox.Show(wex.Message);//This should have already been Lans.g if applicable.
				return null;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error:")+" "+ex.Message);
				return null;
			}
			if(_transType.In(PaySimple.TransType.SALE,PaySimple.TransType.RETURN,PaySimple.TransType.VOID)) {//Only print a receipt if transaction is an approved SALE, RETURN, or VOID			
				//The isSwiped boolean could be incorrectly set if the user swipes a card and then changes the data that was entered to a different card.
				apiResponseRetVal.BuildReceiptString(cardNumber,expMonth,expYear,textNameOnCard.Text,_clinicNum,_magstripCardParser!=null);
				if(checkPrintReceipt.Checked) {
					PrintReceipt(apiResponseRetVal.TransactionReceipt);
				}
			}
			if(checkOneTimePayment.Checked) {//not storing the card token
				return apiResponseRetVal;
			}
			UpsertCreditCard(apiResponseRetVal,StringTools.TruncateBeginning(textCardNumber.Text,4).PadLeft(textCardNumber.Text.Length,'X'),CreditCardSource.PaySimple,
				new DateTime(expYear,expMonth,DateTime.DaysInMonth(expYear,expMonth)));
			return apiResponseRetVal;
		}

		///<summary>Processes a PaySimple ACH payment via the PaySimple API.</summary>
		private PaySimple.ApiResponse ProcessPaymentACH() {
			PaySimple.ApiResponse apiResponseRetVal=null;
			string accountNumber=textCheckSaveNumber.Text;
			//if the user has chosen to store CC tokens and the stored CC has a token and the token is not expired,
			//then use it instead of the CC number and CC expiration.
			if(!checkOneTimePaymentACH.Checked
				&& _creditCard!=null //if the user selected a saved CC
				&& !string.IsNullOrWhiteSpace(_creditCard.PaySimpleToken) //there is a stored token for this card
				&& _creditCard.IsPaySimpleACH())
			{
				accountNumber=_creditCard.PaySimpleToken;
			}
			else if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePreventSavingNewCC,_clinicNum))) {
				MsgBox.Show(this,"Cannot add a new ACH payment.");
				return null;
			}
			try {
				if(_isAddingCard) {
					apiResponseRetVal=PaySimple.AddACHAccount(_patient,textRoutingNumber.Text,textCheckSaveNumber.Text,textBankName.Text,radioCheckings.Checked,_clinicNum);
				}
				else {
					apiResponseRetVal=PaySimple.MakePaymentACH(_patient,_creditCard,PIn.Decimal(textAmountACH.Text),textRoutingNumber.Text,textCheckSaveNumber.Text,
						textBankName.Text,radioCheckings.Checked,checkOneTimePaymentACH.Checked,_clinicNum);
				}
			}
			catch(PaySimpleException ex) {
				MessageBox.Show(ex.Message);
				if(ex.ErrorType==PaySimpleError.CustomerDoesNotExist && MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"Delete the link to the customer id for this patient?")) 
				{
					PatientLinks.DeletePatNumTos(ex.CustomerId,PatientLinkType.PaySimple);
				}
				return null;
			}
			catch(ODException ex) {
				MessageBox.Show(ex.Message);//This should have already been Lans.g if applicable.
				return null;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error:")+" "+ex.Message,ex);
				return null;
			}
			try {
				string result=WebServiceMainHQProxy.GetWebServiceMainHQInstance()
					.InsertPaySimpleACHId(PayloadHelper.CreatePayload(
						PayloadHelper.CreatePayloadContent(apiResponseRetVal.RefNumber.ToString(),"PaymentId"),eServiceCode.PaySimple));
				PayloadHelper.CheckForError(result);
			}
			catch(Exception ex) {
				FriendlyException.Show("Unable to register for ACH Settled event",ex);
			}	
			if(!_isAddingCard) {
				apiResponseRetVal.BuildReceiptString(accountNumber,-1,-1,_patient?.GetNameFL(),_clinicNum,wasSwiped: false,isACH:true);
				if(checkPrintReceiptACH.Checked) {
					PrintReceipt(apiResponseRetVal.TransactionReceipt);
				}
			}
			if(checkOneTimePaymentACH.Checked) {//not storing the account token
				return apiResponseRetVal;
			}
			CreditCardSource source=_creditCard?.CCSource??CreditCardSource.PaySimpleACH;
			UpsertCreditCard(apiResponseRetVal,StringTools.TruncateBeginning(textCheckSaveNumber.Text,4).PadLeft(textCheckSaveNumber.Text.Length,'*'),source,DateTime.MinValue);
			return apiResponseRetVal;
		}

		private void UpsertCreditCard(PaySimple.ApiResponse apiResponse,string creditCardNumberMasked,CreditCardSource creditCardSource,DateTime dateCCExp) {
			if(_creditCard==null) {//new account
				_creditCard=new CreditCard();
				_creditCard.IsNew=true;
				_creditCard.PatNum=_patient.PatNum;
				List<CreditCard> listCreditCardsItemOrderCount=CreditCards.Refresh(_patient.PatNum);
				_creditCard.ItemOrder=listCreditCardsItemOrderCount.Count;
			}
			if(dateCCExp.Year > 1880) {
				_creditCard.CCExpiration=dateCCExp;
			}
			_creditCard.CCNumberMasked=creditCardNumberMasked;
			_creditCard.Zip=textZipCode.Text;
			_creditCard.PaySimpleToken=apiResponse.PaySimpleToken;
			_creditCard.CCSource=creditCardSource;
			if(_creditCard.IsNew) {
				_creditCard.ClinicNum=_clinicNum;
				_creditCard.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
				CreditCards.Insert(_creditCard);
			}
			else {
				CreditCards.Update(_creditCard);
			}
		}

		private void PrintReceipt(string receiptStr) {
			string[] stringArrayReceiptLines=receiptStr.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
			MigraDoc.DocumentObjectModel.Document document=new MigraDoc.DocumentObjectModel.Document();
			document.DefaultPageSetup.PageWidth=Unit.FromInch(3.0);
			document.DefaultPageSetup.PageHeight=Unit.FromInch(0.181*stringArrayReceiptLines.Length+0.56);//enough to print receipt text plus 9/16 inch (0.56) extra space at bottom.
			document.DefaultPageSetup.TopMargin=Unit.FromInch(0.25);
			document.DefaultPageSetup.LeftMargin=Unit.FromInch(0.25);
			document.DefaultPageSetup.RightMargin=Unit.FromInch(0.25);
			MigraDoc.DocumentObjectModel.Font fontXBody=MigraDocHelper.CreateFont(8,false);
			fontXBody.Name=FontFamily.GenericMonospace.Name;
			MigraDoc.DocumentObjectModel.Section section=document.AddSection();
			Paragraph paragraph=section.AddParagraph();
			ParagraphFormat paragraphFormat=new ParagraphFormat();
			paragraphFormat.Alignment=ParagraphAlignment.Left;
			paragraphFormat.Font=fontXBody;
			paragraph.Format=paragraphFormat;
			paragraph.AddFormattedText(receiptStr,fontXBody);
			MigraDoc.Rendering.Printing.MigraDocPrintDocument migraDocPrintDocument=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
			MigraDoc.Rendering.DocumentRenderer documentRenderer=new MigraDoc.Rendering.DocumentRenderer(document);
			documentRenderer.PrepareDocument();
			migraDocPrintDocument.Renderer=documentRenderer;
			//TODO: Implement ODprintout pattern - MigraDoc
			if(ODBuild.IsDebug()) {
				using FormRpPrintPreview formRpPrintPreview=new FormRpPrintPreview(migraDocPrintDocument);
				formRpPrintPreview.ShowDialog();
				return;
			}
			if(PrinterL.SetPrinter(pd2,PrintSituation.Receipt,_patient.PatNum,"PaySimple receipt printed")) {
				migraDocPrintDocument.PrinterSettings=pd2.PrinterSettings;
				try {
					migraDocPrintDocument.Print();
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Printer not available.")+"\r\n"+Lan.g(this,"Original error")+": "+ex.Message);
				}
			}
		}

		private bool VerifyData(out int expYear,out int expMonth) {
			expYear=0;
			expMonth=0;
			if(_transType==PaySimple.TransType.SALE && !Regex.IsMatch(textAmount.Text,"^[0-9]+$") && !Regex.IsMatch(textAmount.Text,"^[0-9]*\\.[0-9]+$")) {
				MsgBox.Show(this,"Invalid amount.");
				return false;
			}
			if((_transType==PaySimple.TransType.VOID || _transType==PaySimple.TransType.RETURN)//The reference number is optional for terminal returns. 
				&& textRefNumber.Text=="") 
			{
				MsgBox.Show(this,"Ref Number required.");
				return false;
			}
			string paytype=ProgramProperties.GetPropValForClinicOrDefault(_program.ProgramNum,PaySimple.PropertyDescs.PaySimplePayTypeCC,_clinicNum);
			if(!Defs.GetDefsForCategory(DefCat.PaymentTypes,true).Any(x => x.DefNum.ToString()==paytype)) { //paytype is not a valid DefNum
				MsgBox.Show(this,"The PaySimple payment type has not been set.");
				return false;
			}
			//Processing through Web Service
			// Consider adding more advanced verification methods using PaySimple validation requests.
			if(textCardNumber.Text.Trim().Length<5) {
				MsgBox.Show(this,"Invalid Card Number.");
				return false;
			}	
			if(Regex.IsMatch(textExpDate.Text,@"^\d\d[/\- ]\d\d$")) {//08/07 or 08-07 or 08 07
				try {//PIn.Int will throw an exception if not a valid format
					expYear=PIn.Int("20"+textExpDate.Text.Substring(3,2));
					expMonth=PIn.Int(textExpDate.Text.Substring(0,2));
				}
				catch(Exception) {
					MsgBox.Show(this,"Expiration format invalid.");
					return false;
				}
			}
			else if(Regex.IsMatch(textExpDate.Text,@"^\d{4}$")) {//0807
				try {//PIn.Int will throw an exception if not a valid format
					expYear=PIn.Int("20"+textExpDate.Text.Substring(2,2));
					expMonth=PIn.Int(textExpDate.Text.Substring(0,2));
				}
				catch(Exception) {
					MsgBox.Show(this,"Expiration format invalid.");
					return false;
				}
			}
			else {
				MsgBox.Show(this,"Expiration format invalid.");
				return false;
			}
			if(_creditCard==null) {//if the user selected a new CC, verify through PaySimple
				//using a new CC and the card number entered contains something other than digits
				if(textCardNumber.Text.Any(x => !char.IsDigit(x))) {
					MsgBox.Show(this,"Invalid card number.");
					return false;
				}
			}
			else if(_creditCard.PaySimpleToken=="" && Regex.IsMatch(textCardNumber.Text,@"X+[0-9]{4}")) {//using a stored CC
				MsgBox.Show(this,"There is no saved PaySimple token for this credit card.  The card number and expiration must be re-entered.");
				return false;
			}
			if(textNameOnCard.Text.Trim()=="" && _patient!=null && _patient.PatNum>0) {//Name required for patient credit cards, not prepaid cards.
				MsgBox.Show(this,"Name On Card required.");
				return false;
			}
			return IsPaySimpleSetup();
		}

		private bool VerifyDataACH() {
			// If Credit Card is not null, contains a token, and is currently set as a credit card of some sort in CCsource...
			// AKA: If this thing is not an ACH account (but is/was a valid CreditCard)...
			// Then validate the user input fields because apparently we are trying to convert the CC entry into an ACH.
			if(string.IsNullOrEmpty(_creditCard?.PaySimpleToken) || !_creditCard.IsPaySimpleACH()) {
				if(!Regex.IsMatch(textRoutingNumber.Text,"^[0-9]+$")) {
					MsgBox.Show(this,"Invalid Routing Number.");
					return false;
				}
				if(!Regex.IsMatch(textCheckSaveNumber.Text,"^[0-9]+$") && !Regex.IsMatch(textCheckSaveNumber.Text,"^[0-9]*\\.[0-9]+$")) {
					MsgBox.Show(this,"Invalid Account Number.");
					return false;
				}
				if(string.IsNullOrWhiteSpace(textBankName.Text)) {
					MsgBox.Show(this,"Bank Name required.");
					return false;
				}
			}
			return IsPaySimpleSetup();
		}

		private bool IsPaySimpleSetup() {
			//verify the selected clinic has a username and API key entered
			if(string.IsNullOrWhiteSpace(ProgramProperties.GetPropValForClinicOrDefault(_program.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiUserName,_clinicNum))
				|| string.IsNullOrWhiteSpace(ProgramProperties.GetPropValForClinicOrDefault(_program.ProgramNum,PaySimple.PropertyDescs.PaySimpleApiKey,_clinicNum))) 
			{
				MsgBox.Show(this,"The PaySimple username and/or key has not been set.");
				return false;
			}
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(tabControl.SelectedTab==tabCredit) {
				int expYear;
				int expMonth;
				if(!VerifyData(out expYear,out expMonth)) {
					Cursor=Cursors.Default;
					return;
				}
				ApiResponseOut=ProcessPayment(expYear,expMonth);
			}
			else {
				if(!VerifyDataACH()) {
					Cursor=Cursors.Default;
					return;
				}
				ApiResponseOut=ProcessPaymentACH();
			}
			WasPaymentAttempted=true;
			bool isSuccess=(ApiResponseOut!=null);
			Cursor=Cursors.Default;
			if(isSuccess) {
				DialogResult=DialogResult.OK;
			}
			else if(!_isAddingCard) {//If adding the card, leave the window open so the user can try again.
				DialogResult=DialogResult.Cancel;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}