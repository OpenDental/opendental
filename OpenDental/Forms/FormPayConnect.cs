using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using DentalXChange.Dps.Pos;
using MigraDoc.DocumentObjectModel;
using OpenDental.Bridges;
using OpenDentBusiness;
using PayConnectService = OpenDentBusiness.PayConnectService;

namespace OpenDental {
	public partial class FormPayConnect:FormODBase {
		private Patient _patient;
		private decimal _amountInit;
		private PayConnectService.transResponse _transResponse;
		private MagstripCardParser _magstripCardParser=null;
		public string ReceiptStr;
		public PayConnectService.transType TransType=PayConnectService.transType.SALE;
		private CreditCard _creditCard;
		public PayConnectService.creditCardRequest CreditCardRequest;
		private bool _isAddingCard;
		private long _clinicNum;
		private Program _program;
		private PayConnectResponse _payConnectResponse;
		///<summary>Some card readers have CR/Enter track separators, 
		///which would cause our parsing logic to happen before the magstripe reader finished outputting the data.
		///We add a timer delay to attempt to compensate for this functionality.</summary>
		private bool _hasSwipedCard;
		private int _expYear;
		private int _expMonth;
		///<summary>Opening the PayConnect or PaySimple window from FormPayment, and then closing them, can set isCcDeclined to True.
		///This is because FormPayment didn't know if a transaction was attempted or not, and was assuming it was.
		///This can cause the payment amount to be reset to $0. So, this bool indicates if we have actually attempted a transaction.</summary>
		public bool WasPaymentAttempted=false;

		///<summary>Can handle CreditCard being null.</summary>
		public FormPayConnect(long clinicNum,Patient patient,decimal amount,CreditCard creditCard,bool isAddingCard=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
			_patient=patient;
			_amountInit=amount;
			ReceiptStr="";
			_creditCard=creditCard;
			_isAddingCard=isAddingCard;
		}

		private void FormPayConnect_Load(object sender,EventArgs e) {
			_program=Programs.GetCur(ProgramName.PayConnect);
			if(_program==null) {
				MsgBox.Show(this,"PayConnect does not exist in the database.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(ODBuild.IsWeb()){
				sigBoxWrapper.Enabled=false;
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,"TerminalProcessingEnabled",_clinicNum))) {
				try {
					//If the config file for the DentalXChange credit card processing .dll doesn't exist, construct it from the included resource.
					ODFileUtils.WriteAllText("DpsPos.dll.config",Properties.Resources.DpsPos_dll_config,false);
				}
				catch(Exception ex) {
					FriendlyException.Show("Unable to create the config file for the terminal. Trying running the program as an administrator.",ex);
					//We will still allow them to run the transaction. Probably the worse that will happen is the timeout variable will be less than desired.
				}
			}
			textAmount.Text=POut.Decimal(_amountInit);
			if(_patient==null) {//Prepaid card
				radioAuthorization.Enabled=false;
				radioVoid.Enabled=false;
				radioReturn.Enabled=false;
				textZipCode.ReadOnly=true;
				textNameOnCard.ReadOnly=true;
				checkSaveToken.Enabled=false;
				sigBoxWrapper.Enabled=false;
			}
			else {//Other cards
				textZipCode.Text=_patient.Zip;
				textNameOnCard.Text=_patient.GetNameFL();
				checkSaveToken.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
				if(PrefC.GetBool(PrefName.StoreCCnumbers)) {
					labelStoreCCNumWarning.Visible=true;
				}
				FillFieldsFromCard();
			}
			if(!PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,"TerminalProcessingEnabled",_clinicNum))
				|| _isAddingCard) //When adding a card, the web service must be used.
			{
				groupProcessMethod.Visible=false;
				Height-=LayoutManager.Scale(55);//All the controls except for the Transaction Type group box should be anchored to the bottom, so they will move themselves up.
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
			if(_isAddingCard) {//We will run a 0.01 authorization so we will not allow the user to change the transaction type or the amount.
				radioAuthorization.Checked=true;
				TransType=PayConnectService.transType.AUTH;
				groupTransType.Enabled=false;
				labelAmount.Visible=false;
				textAmount.Visible=false;
				checkSaveToken.Checked=true;
				checkSaveToken.Enabled=false;
				checkForceDuplicate.Checked=true;
				checkForceDuplicate.Enabled=false;
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum))) {
				textCardNumber.ReadOnly=true;
			}
		}

		private void FillFieldsFromCard() {
			if(_creditCard!=null) {//User selected a credit card from drop down.
				if(_creditCard.CCNumberMasked!="") {
					string ccNum=_creditCard.CCNumberMasked;
					if(Regex.IsMatch(ccNum,"^\\d{12}(\\d{0,7})")) {	//Minimum of 12 digits, maximum of 19
						int idxLast4Digits=(ccNum.Length-4);
						ccNum=(new string('X',12))+ccNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
					}
					textCardNumber.Text=ccNum;
				}
				if(_creditCard.CCExpiration!=null && _creditCard.CCExpiration.Year>2005) {
					textExpDate.Text=_creditCard.CCExpiration.ToString("MMyy");
				}
				if(_creditCard.Zip!="") {
					textZipCode.Text=_creditCard.Zip;
				}
				if(_creditCard.PayConnectToken!="" && _creditCard.PayConnectTokenExp>DateTime.MinValue) {
					checkSaveToken.Checked=true;
					checkSaveToken.Enabled=false;
					textNameOnCard.ReadOnly=true;
					textCardNumber.ReadOnly=true;
					textExpDate.ReadOnly=true;
				}
				else if(!string.IsNullOrEmpty(_creditCard.XChargeToken) || !string.IsNullOrEmpty(_creditCard.PaySimpleToken)) {
					//No token for this cc. Have the user enter the cc info.
					textCardNumber.Text="";
				}
			}
		}

		private void radioSale_Click(object sender,EventArgs e) {
			//radioSale.Checked=true;
			//radioAuthorization.Checked=false;
			//radioVoid.Checked=false;
			//radioReturn.Checked=false;
			radioForce.Checked=false;
			textRefNumber.Visible=false;
			labelRefNumber.Visible=false;
			TransType=PayConnectService.transType.SALE;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioAuthorization_Click(object sender,EventArgs e) {
			radioForce.Checked=false;
			textRefNumber.Visible=false;
			labelRefNumber.Visible=false;
			TransType=PayConnectService.transType.AUTH;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioVoid_Click(object sender,EventArgs e) {
			radioForce.Checked=false;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Ref Number");
			TransType=PayConnectService.transType.VOID;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioReturn_Click(object sender,EventArgs e) {
			radioForce.Checked=false;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Ref Number");
			TransType=PayConnectService.transType.RETURN;
			//If a security code (cvv) is provided for returns, PayConnect will throw an error saying it shouldn't have been provided.
			textSecurityCode.Text="";
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioReturn_Changed(object sender,EventArgs e) {
			textSecurityCode.Enabled=!radioReturn.Checked;
		}

		private void radioForce_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=false;
			radioVoid.Checked=false;
			radioReturn.Checked=false;
			radioForce.Checked=true;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Authorization Code");
			TransType=PayConnectService.transType.FORCE;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioWebService_CheckedChanged(object sender,EventArgs e) {
			if(!radioWebService.Checked) {
				return;
			}
			new[] { textCardNumber,textExpDate,textNameOnCard,textSecurityCode,textZipCode,textRefNumber,textAmount }.ForEach(x => x.ReadOnly=false);
			radioForce.Enabled=true;
			checkSaveToken.Enabled=true;
			checkForceDuplicate.Enabled=true;
			FillFieldsFromCard();
			textNameOnCard.Text=_patient.GetNameFL();
			if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum))) {
				textCardNumber.ReadOnly=true;
			}
		}

		private void radioTerminal_CheckedChanged(object sender,EventArgs e) {
			if(!radioTerminal.Checked) {
				return;
			}
			new[] { textCardNumber,textExpDate,textNameOnCard,textSecurityCode,textZipCode }.ForEach(x => x.ReadOnly=true);
			Clear();
			radioForce.Enabled=false;
			checkSaveToken.Checked=false;
			checkSaveToken.Enabled=false;
			textAmount.Focus();
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

		private bool VerifyData(){
			_expYear=0;
			_expMonth=0;
			if(!Regex.IsMatch(textAmount.Text,"^[0-9]+$") && !Regex.IsMatch(textAmount.Text,"^[0-9]*\\.[0-9]+$")) {
				MsgBox.Show(this,"Invalid amount.");
				return false;
			}
			if((TransType==PayConnectService.transType.VOID || 
				(TransType==PayConnectService.transType.RETURN && radioWebService.Checked))//The reference number is optional for terminal returns. 
				&& textRefNumber.Text=="") 
			{
				MsgBox.Show(this,"Ref Number required.");
				return false;
			}
			string paytype=ProgramProperties.GetPropVal(_program.ProgramNum,"PaymentType",_clinicNum);
			if(!Defs.GetDefsForCategory(DefCat.PaymentTypes,true).Any(x => x.DefNum.ToString()==paytype)) { //paytype is not a valid DefNum
				MsgBox.Show(this,"The PayConnect payment type has not been set.");
				return false;
			}
			if(radioTerminal.Checked) {
				return true;
			}
			//Processing through Web Service
			// Consider adding more advanced verification methods using PayConnect validation requests.
			if(textCardNumber.Text.Trim().Length<5){
				MsgBox.Show(this,"Invalid Card Number.");
				return false;
			}
			try {//PIn.Int will throw an exception if not a valid format
				if(Regex.IsMatch(textExpDate.Text,@"^\d\d[/\- ]\d\d$")) {//08/07 or 08-07 or 08 07
					_expYear=PIn.Int("20"+textExpDate.Text.Substring(3,2));
					_expMonth=PIn.Int(textExpDate.Text.Substring(0,2));
				}
				else if(Regex.IsMatch(textExpDate.Text,@"^\d{4}$")) {//0807
					_expYear=PIn.Int("20"+textExpDate.Text.Substring(2,2));
					_expMonth=PIn.Int(textExpDate.Text.Substring(0,2));
				}
				else {
					MsgBox.Show(this,"Expiration format invalid.");
					return false;
				}
			}
			catch(Exception) {
				MsgBox.Show(this,"Expiration format invalid.");
				return false;
			}
			if(_creditCard==null) {//if the user selected a new CC, verify through PayConnect
				//using a new CC and the card number entered contains something other than digits
				if(textCardNumber.Text.Any(x => !char.IsDigit(x))) {
					MsgBox.Show(this,"Invalid card number.");
					return false;
				}
				if(!PayConnect.IsValidCardAndExp(textCardNumber.Text,_expYear,_expMonth,x => MessageBox.Show(x))) {//if exception happens, a message box will show with the error
					MsgBox.Show(this,"Card number or expiration date failed validation with PayConnect.");
					return false;
				}
			}
			else if(_creditCard.PayConnectToken=="" && Regex.IsMatch(textCardNumber.Text,@"X+[0-9]{4}")) {//using a stored CC
				MsgBox.Show(this,"There is no saved PayConnect token for this credit card.  The card number and expiration must be re-entered.");
				return false;
			}
			if(textNameOnCard.Text.Trim()=="" && _patient!=null){//Name required for patient credit cards, not prepaid cards.
				MsgBox.Show(this,"Name On Card required.");
				return false;
			}
			if(TransType==PayConnectService.transType.FORCE && textRefNumber.Text=="") {
				MsgBox.Show(this,"Authorization Code required.");
				return false;
			}
			//verify the selected clinic has a username and password type entered
			//Decrypt password first because empty string decrypted does not mean empty string when encrypted.
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(_program.ProgramNum,"Password",_clinicNum));
			if(ProgramProperties.GetPropVal(_program.ProgramNum,"Username",_clinicNum)=="" || password=="") {//if username or password is blank
				MsgBox.Show(this,"The PayConnect username and/or password has not been set.");
				return false;
			}
			return true;
		}

		private PayConnectService.signatureResponse SendSignature(string refNumber) {
			if(!sigBoxWrapper.GetSigChanged() || string.IsNullOrEmpty(sigBoxWrapper.GetSignature(""))) {
				return null;
			}
			PayConnectService.signatureRequest signatureRequest=new PayConnectService.signatureRequest();
			signatureRequest.RefNumber=refNumber;
			signatureRequest.SignatureType=PayConnectService.signatureType.JPEG;
			using(Bitmap bitmapSigImage=sigBoxWrapper.GetSigImage())
			using(MemoryStream memoryStream=new MemoryStream()) {
				bitmapSigImage.Save(memoryStream,ImageFormat.Jpeg);
				byte[] byteArrayImageBytes=memoryStream.ToArray();
				signatureRequest.SignatureData=Convert.ToBase64String(byteArrayImageBytes);
			}
			return PayConnect.ProcessSignature(signatureRequest,_clinicNum,x => MessageBox.Show(x));
		}

		///<summary>Processes a PayConnect payment via the PayConnect web service.</summary>
		private bool ProcessPaymentWebService(int expYear,int expMonth) {
			string refNumber="";
			if(TransType==PayConnectService.transType.VOID || TransType==PayConnectService.transType.RETURN) {
				refNumber=textRefNumber.Text;
			}
			string magData=null;
			if(_magstripCardParser!=null) {
				magData=_magstripCardParser.Track2;
			}
			string cardNumber=textCardNumber.Text;
			//if using a stored CC and there is an X-Charge token saved for the CC and the user enters the whole card number to get a PayConnect token
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
					return false;
				}
			}
			//if stored CC has a token and the token is not expired use it instead of the CC number and CC expiration.
			if(_creditCard!=null //if the user selected a saved CC
				&& _creditCard.PayConnectToken!="")//there is a stored token for this card
				
			{
				if(_creditCard.PayConnectTokenExp.Date>=DateTime.Today.Date) {//the token is not expired
					expYear=_creditCard.PayConnectTokenExp.Year;
					expMonth=_creditCard.PayConnectTokenExp.Month;
				}
				if(_creditCard.PayConnectTokenExp==DateTime.MinValue) {//The token exp date is invalid
					expYear=_creditCard.CCExpiration.Year;
					expMonth=_creditCard.CCExpiration.Month;
				}
				cardNumber=_creditCard.PayConnectToken;
			}
			else if(PIn.Bool(ProgramProperties.GetPropVal(_program.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum))) {
				MsgBox.Show(this,"Cannot add a new credit card.");
				return false;
			}
			string authCode="";
			if(TransType==PayConnectService.transType.FORCE) {
				authCode=textRefNumber.Text;
			}
			CreditCardRequest=PayConnect.BuildSaleRequest(PIn.Decimal(textAmount.Text),cardNumber,expYear,
				expMonth,textNameOnCard.Text,textSecurityCode.Text,textZipCode.Text,magData,TransType,refNumber,
				checkSaveToken.Checked,authCode,checkForceDuplicate.Checked);
			_transResponse=PayConnect.ProcessCreditCard(CreditCardRequest,_clinicNum,x => MessageBox.Show(x));
			if(_transResponse==null || _transResponse.Status.code!=0) {//error in transaction
				return false;
			}
			if(_creditCard!=null && _creditCard.PayConnectTokenExp.Year<1880) {
				_creditCard.PayConnectTokenExp=_creditCard.CCExpiration;
				CreditCards.Update(_creditCard);//Updating here for bugfix where PayConnectTokenExp fields were being invalidated. making sure that it is updated after adding the new value.
			}
			
			PayConnectService.signatureResponse signatureResponse=SendSignature(_transResponse.RefNumber);			
			if((TransType.In(PayConnectService.transType.SALE,PayConnectService.transType.RETURN,PayConnectService.transType.VOID))
					&& _transResponse.Status.code==0) {//Only print a receipt if transaction is an approved SALE, RETURN, or VOID			
				ReceiptStr=PayConnect.BuildReceiptString(CreditCardRequest,_transResponse,signatureResponse,_clinicNum);
				PayConnectL.PrintReceipt(ReceiptStr,_patient);
			}
			if(!PrefC.GetBool(PrefName.StoreCCnumbers) && !checkSaveToken.Checked) {//not storing the card number or the token
				return true;
			}
			//response must be non-null and the status code must be 0=Approved
			//also, the user must have the pref StoreCCnumbers enabled or they have the checkSaveTokens checked
			if(_creditCard==null) {//user selected Add new card from the payment window, save it or its token depending on settings
				_creditCard=new CreditCard();
				_creditCard.IsNew=true;
				_creditCard.PatNum=_patient.PatNum;
				List<CreditCard> listCreditCardsItemOrderCount=CreditCards.Refresh(_patient.PatNum);
				_creditCard.ItemOrder=listCreditCardsItemOrderCount.Count;
			}
			_creditCard.CCExpiration=new DateTime(expYear,expMonth,DateTime.DaysInMonth(expYear,expMonth));
			if(PrefC.GetBool(PrefName.StoreCCnumbers)) {
				_creditCard.CCNumberMasked=textCardNumber.Text;
			}
			else {
				_creditCard.CCNumberMasked=StringTools.TruncateBeginning(textCardNumber.Text,4).PadLeft(textCardNumber.Text.Length,'X');
			}
			if(!_creditCard.IsNew && _transResponse.PaymentToken!=null) {
				DateTime transactionTokenExpField=new DateTime(_transResponse.PaymentToken.Expiration.year,_transResponse.PaymentToken.Expiration.month,
					DateTime.DaysInMonth(_transResponse.PaymentToken.Expiration.year,_transResponse.PaymentToken.Expiration.month));
				//If the token expiration doesn't match what is returned by PayConnect, update the token expiration
				if(_creditCard.PayConnectTokenExp!=transactionTokenExpField) {
					_creditCard.PayConnectTokenExp=transactionTokenExpField;
					CreditCards.Update(_creditCard);
				}
			}
			//Store the token and the masked CC number (only last four digits).
			_creditCard.CCSource=CreditCardSource.PayConnect;
			if(_creditCard.IsNew && checkSaveToken.Checked && _transResponse.PaymentToken!=null) {
				_creditCard.Zip=textZipCode.Text;
				_creditCard.ClinicNum=_clinicNum;
				_creditCard.PayConnectToken=_transResponse.PaymentToken.TokenId;
				_creditCard.PayConnectTokenExp=new DateTime(_transResponse.PaymentToken.Expiration.year,_transResponse.PaymentToken.Expiration.month,
					DateTime.DaysInMonth(_transResponse.PaymentToken.Expiration.year,_transResponse.PaymentToken.Expiration.month));
				_creditCard.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
				CreditCards.Insert(_creditCard);
			}
			else {
				if(_creditCard.CCSource==CreditCardSource.XServer) {//This card has also been added for XCharge.
					_creditCard.CCSource=CreditCardSource.XServerPayConnect;
				}
				CreditCards.Update(_creditCard);
			}
			return true;
		}

		///<summary>Processes a PayConnect payment via a credit card terminal.</summary>
		private bool ProcessPaymentTerminal() {
			if(ODBuild.IsWeb()) {
				if(!CloudClientL.IsCloudClientRunning()) {
					return false;
				}
				if(radioSale.Checked) {
					_payConnectResponse=ODCloudClient.ProcessPaymentTerminal("SALE",PIn.Decimal(textAmount.Text),checkForceDuplicate.Checked);
				}
				else if(radioAuthorization.Checked) {
					_payConnectResponse=ODCloudClient.ProcessPaymentTerminal("AUTH",PIn.Decimal(textAmount.Text),checkForceDuplicate.Checked);
				}
				else if(radioVoid.Checked) {
					_payConnectResponse=ODCloudClient.ProcessPaymentTerminal("VOID",PIn.Decimal(textAmount.Text),checkForceDuplicate.Checked,textRefNumber.Text);
				}
				else if(radioReturn.Checked) {
					_payConnectResponse=ODCloudClient.ProcessPaymentTerminal("RETURN",PIn.Decimal(textAmount.Text),checkForceDuplicate.Checked,textRefNumber.Text);
				}
				else {//Shouldn't happen
					MsgBox.Show(this,"Error creating request: Please select a transaction type");
					return false;
				}
				if(_payConnectResponse==null) {
					SecurityLogs.MakeLogEntry(EnumPermType.CreditCardTerminal,_patient.PatNum,"No response received.");
					return false;
				}
				textCardNumber.Text=_payConnectResponse.CardNumber;
				textAmount.Text=_payConnectResponse.Amount.ToString("f");
				ReceiptStr=PayConnectTerminal.BuildReceiptString(_payConnectResponse,false,_clinicNum);
				PayConnectL.PrintReceipt(ReceiptStr,_patient);
				return true;
			}//end of IsWeb()
			PosRequest posRequest=null;
			try {
				if(radioSale.Checked) {
					posRequest=PosRequest.CreateSale(PIn.Decimal(textAmount.Text));
				}
				else if(radioAuthorization.Checked) {
					posRequest=PosRequest.CreateAuth(PIn.Decimal(textAmount.Text));
				}
				else if(radioVoid.Checked) {
					posRequest=PosRequest.CreateVoidByReference(textRefNumber.Text);
				}
				else if(radioReturn.Checked) {
					if(textRefNumber.Text=="") {
						posRequest=PosRequest.CreateRefund(PIn.Decimal(textAmount.Text));
					}
					else {
						posRequest=PosRequest.CreateRefund(PIn.Decimal(textAmount.Text),textRefNumber.Text);
					}
				}
				else {//Shouldn't happen
					MsgBox.Show(this,"Please select a transaction type");
					return false;
				}
				posRequest.ForceDuplicate=checkForceDuplicate.Checked;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error creating request:")+" "+ex.Message);
				return false;
			}
			UI.ProgressOD progressOD=new UI.ProgressOD();
			progressOD.ActionMain=() => _payConnectResponse=PayConnectTerminal.ToPayConnectResponse(DpsPos.ProcessCreditCard(posRequest));
			progressOD.ShowCancelButton=false;
			progressOD.StartingMessage=Lan.g(this,"Processing payment on terminal");
			progressOD.StopNotAllowedMessage=Lan.g(this,"Not allowed to stop. Please wait up to 2 minutes.");
			try{
				progressOD.ShowDialogProgress();
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
			if(_payConnectResponse==null) {
				MessageBox.Show(Lan.g(this,"Error processing card"));
				return false;
			}
			if(_payConnectResponse.StatusCode!="0") {//"0" indicates success. May need to check the AuthCode field too to determine if this was a success.
				MessageBox.Show(Lan.g(this,"Error message from Pay Connect:")+"\r\n"+_payConnectResponse.Description);
				return false;
			}
			PayConnectService.signatureResponse signatureResponse=null;
			try {
				Cursor=Cursors.WaitCursor;
				signatureResponse=SendSignature(_payConnectResponse.RefNumber.ToString());
				Cursor=Cursors.Default;
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Card successfully charged. Error processing signature:")+" "+ex.Message);
			}
			textCardNumber.Text=_payConnectResponse.CardNumber;
			textAmount.Text=_payConnectResponse.Amount.ToString("f");
			bool wasSigned=true;
			if(signatureResponse==null || signatureResponse.Status==null || signatureResponse.Status.code!=0) {
				wasSigned=false;
			}
			ReceiptStr=PayConnectTerminal.BuildReceiptString(_payConnectResponse,wasSigned,_clinicNum);
			PayConnectL.PrintReceipt(ReceiptStr,_patient);
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			if(!VerifyData()) {
				Cursor=Cursors.Default;
				return;
			}
			bool isSuccess;
			WasPaymentAttempted=true;
			if(radioWebService.Checked) {
				isSuccess=ProcessPaymentWebService(_expYear,_expMonth);
			}
			else {
				isSuccess=ProcessPaymentTerminal();
			}
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

		private void FormPayConnect_FormClosing(object sender,FormClosingEventArgs e) {
			sigBoxWrapper?.SetTabletState(0);
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public string GetAmountCharged() {
			if(TransType==PayConnectService.transType.RETURN) {
				return PIn.Decimal("-"+textAmount.Text).ToString("F");
			}
			return PIn.Decimal(textAmount.Text).ToString("F");
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public string GetCardNumber() {
			return textCardNumber.Text;
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public PayConnectResponse GetResponse() {
			if(_transResponse != null) {
				return PayConnectREST.ToPayConnectResponse(_transResponse,CreditCardRequest);
			}
			if(_payConnectResponse != null) {
				return _payConnectResponse;
			}
			return null;
		}


	}
}