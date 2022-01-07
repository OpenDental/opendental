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
		private Patient _patCur;
		private decimal _amountInit;
		private PayConnectService.transResponse _response;
		private MagstripCardParser _parser=null;
		private string _receiptStr;
		private PayConnectService.transType _trantype=PayConnectService.transType.SALE;
		private CreditCard _creditCardCur;
		private PayConnectService.creditCardRequest _request;
		private bool _isAddingCard;
		private long _clinicNum;
		private Program _progCur;
		private PosResponse _posResponse;
		///<summary>Some card readers have CR/Enter track separators, 
		///which would cause our parsing logic to happen before the magstripe reader finished outputting the data.
		///We add a timer delay to attempt to compensate for this functionality.</summary>
		private bool _hasSwipedCard;

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public string AmountCharged {
			get { return PIn.Decimal(textAmount.Text).ToString("F"); }
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public PayConnectService.creditCardRequest Request {
			get { return _request; }
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public string ReceiptStr {
			get { return _receiptStr; }
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public PayConnectService.transType TranType {
			get { return _trantype; }
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public string CardNumber {
			get { return textCardNumber.Text; }
		}

		///<summary>Only call after the form is closed and the DialogResult is DialogResult.OK.</summary>
		public PayConnectResponse Response {
			get {
				if(_response != null) {
					return new PayConnectResponse(_response,_request);
				}
				if(_posResponse != null) {
					return PayConnectTerminal.ToPayConnectResponse(_posResponse);
				}
				return null;
			}
		}

		///<summary>Can handle CreditCard being null.</summary>
		public FormPayConnect(long clinicNum,Patient pat,decimal amount,CreditCard creditCard,bool isAddingCard=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
			_patCur=pat;
			_amountInit=amount;
			_receiptStr="";
			_creditCardCur=creditCard;
			_isAddingCard=isAddingCard;
		}

		private void FormPayConnect_Load(object sender,EventArgs e) {
			_progCur=Programs.GetCur(ProgramName.PayConnect);
			if(_progCur==null) {
				MsgBox.Show(this,"PayConnect does not exist in the database.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(_progCur.ProgramNum,"TerminalProcessingEnabled",_clinicNum))) {
				try {
					//If the config file for the DentalXChange credit card processing .dll doesn't exist, construct it from the included resource.
					if(!File.Exists("DpsPos.dll.config")) {
						File.WriteAllText("DpsPos.dll.config",Properties.Resources.DpsPos_dll_config);
					}
				}
				catch(Exception ex) {
					FriendlyException.Show("Unable to create the config file for the terminal. Trying running the program as an administrator.",ex);
					//We will still allow them to run the transaction. Probably the worse that will happen is the timeout variable will be less than desired.
				}
			}
			if(!PIn.Bool(ProgramProperties.GetPropVal(_progCur.ProgramNum,"TerminalProcessingEnabled",_clinicNum))
				|| _isAddingCard) //When adding a card, the web service must be used.
			{
				groupProcessMethod.Visible=false;
				Height-=55;//All the controls except for the Transaction Type group box should be anchored to the bottom, so they will move themselves up.
			}
			else {
				string procMethod=ProgramProperties.GetPropValForClinicOrDefault(_progCur.ProgramNum,
					PayConnect.ProgramProperties.DefaultProcessingMethod,_clinicNum);
				if(procMethod=="0") {
					radioWebService.Checked=true;
				}
				else if(procMethod=="1") {
					radioTerminal.Checked=true;
				}
			}
			textAmount.Text=POut.Decimal(_amountInit);
			if(_patCur==null) {//Prepaid card
				radioAuthorization.Enabled=false;
				radioVoid.Enabled=false;
				radioReturn.Enabled=false;
				textZipCode.ReadOnly=true;
				textNameOnCard.ReadOnly=true;
				checkSaveToken.Enabled=false;
				sigBoxWrapper.Enabled=false;
			}
			else {//Other cards
				textZipCode.Text=_patCur.Zip;
				textNameOnCard.Text=_patCur.GetNameFL();
				checkSaveToken.Checked=PrefC.GetBool(PrefName.StoreCCtokens);
				if(PrefC.GetBool(PrefName.StoreCCnumbers)) {
					labelStoreCCNumWarning.Visible=true;
				}
				FillFieldsFromCard();
			}
			if(_isAddingCard) {//We will run a 0.01 authorization so we will not allow the user to change the transaction type or the amount.
				radioAuthorization.Checked=true;
				_trantype=PayConnectService.transType.AUTH;
				groupTransType.Enabled=false;
				labelAmount.Visible=false;
				textAmount.Visible=false;
				checkSaveToken.Checked=true;
				checkSaveToken.Enabled=false;
				checkForceDuplicate.Checked=true;
				checkForceDuplicate.Enabled=false;
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(_progCur.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum))) {
				textCardNumber.ReadOnly=true;
			}
		}

		private void FillFieldsFromCard() {
			if(_creditCardCur!=null) {//User selected a credit card from drop down.
				if(_creditCardCur.CCNumberMasked!="") {
					string ccNum=_creditCardCur.CCNumberMasked;
					if(Regex.IsMatch(ccNum,"^\\d{12}(\\d{0,7})")) {	//Minimum of 12 digits, maximum of 19
						int idxLast4Digits=(ccNum.Length-4);
						ccNum=(new string('X',12))+ccNum.Substring(idxLast4Digits);//replace the first 12 with 12 X's
					}
					textCardNumber.Text=ccNum;
				}
				if(_creditCardCur.CCExpiration!=null && _creditCardCur.CCExpiration.Year>2005) {
					textExpDate.Text=_creditCardCur.CCExpiration.ToString("MMyy");
				}
				if(_creditCardCur.Zip!="") {
					textZipCode.Text=_creditCardCur.Zip;
				}
				if(_creditCardCur.PayConnectToken!="" && _creditCardCur.PayConnectTokenExp>DateTime.MinValue) {
					checkSaveToken.Checked=true;
					checkSaveToken.Enabled=false;
					textNameOnCard.ReadOnly=true;
					textCardNumber.ReadOnly=true;
					textExpDate.ReadOnly=true;
				}
				else if(!string.IsNullOrEmpty(_creditCardCur.XChargeToken) || !string.IsNullOrEmpty(_creditCardCur.PaySimpleToken)) {
					//No token for this cc. Have the user enter the cc info.
					textCardNumber.Text="";
				}
			}
		}

		private void radioSale_Click(object sender,EventArgs e) {
			radioSale.Checked=true;
			radioAuthorization.Checked=false;
			radioVoid.Checked=false;
			radioReturn.Checked=false;
			radioForce.Checked=false;
			textRefNumber.Visible=false;
			labelRefNumber.Visible=false;
			_trantype=PayConnectService.transType.SALE;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioAuthorization_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=true;
			radioVoid.Checked=false;
			radioReturn.Checked=false;
			radioForce.Checked=false;
			textRefNumber.Visible=false;
			labelRefNumber.Visible=false;
			_trantype=PayConnectService.transType.AUTH;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioVoid_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=false;
			radioVoid.Checked=true;
			radioReturn.Checked=false;
			radioForce.Checked=false;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Ref Number");
			_trantype=PayConnectService.transType.VOID;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioReturn_Click(object sender,EventArgs e) {
			radioSale.Checked=false;
			radioAuthorization.Checked=false;
			radioVoid.Checked=false;
			radioReturn.Checked=true;
			radioForce.Checked=false;
			textRefNumber.Visible=true;
			labelRefNumber.Visible=true;
			labelRefNumber.Text=Lan.g(this,"Ref Number");
			_trantype=PayConnectService.transType.RETURN;
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
			_trantype=PayConnectService.transType.FORCE;
			if(radioWebService.Checked) {
				textCardNumber.Focus();//Usually transaction type is chosen before card number is entered, but textCardNumber box must be selected in order for card swipe to work.
			}
			else {
				textAmount.Focus();
			}
		}

		private void radioWebService_CheckedChanged(object sender,EventArgs e) {
			radioTerminal.Checked=!radioWebService.Checked;
			if(!radioWebService.Checked) {
				return;
			}
			foreach(TextBox textBox in Controls.OfType<TextBox>()) {
				textBox.ReadOnly=false;
			}
			radioForce.Enabled=true;
			checkSaveToken.Enabled=true;
			checkForceDuplicate.Enabled=true;
			FillFieldsFromCard();
			textNameOnCard.Text=_patCur.GetNameFL();
			if(PIn.Bool(ProgramProperties.GetPropVal(_progCur.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum))) {
				textCardNumber.ReadOnly=true;
			}
		}

		private void radioTerminal_CheckedChanged(object sender,EventArgs e) {
			radioWebService.Checked=!radioTerminal.Checked;
			if(!radioTerminal.Checked) {
				return;
			}
			foreach(TextBox textBox in Controls.OfType<TextBox>()) {
				if(textBox==textRefNumber || textBox==textAmount) {
					continue;
				}
				textBox.ReadOnly=true;
				textBox.Text="";
			}
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
				_parser=new MagstripCardParser(data);
			}
			catch(MagstripCardParseException) {
				MessageBox.Show(this,"Could not read card, please try again.","Card Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			if(_parser!=null) {
				textCardNumber.Text=_parser.AccountNumber;
				textExpDate.Text=_parser.ExpirationMonth.ToString().PadLeft(2,'0')+(_parser.ExpirationYear%100).ToString().PadLeft(2,'0');
				textNameOnCard.Text=_parser.FirstName+" "+_parser.LastName;
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

		private bool VerifyData(out int expYear,out int expMonth){
			expYear=0;
			expMonth=0;
			if(ODBuild.IsWeb() && radioTerminal.Checked) {
				MsgBox.Show(this,"Terminal payments are not available while viewing through the web.");
				return false;
			}
			if(!Regex.IsMatch(textAmount.Text,"^[0-9]+$") && !Regex.IsMatch(textAmount.Text,"^[0-9]*\\.[0-9]+$")) {
				MsgBox.Show(this,"Invalid amount.");
				return false;
			}
			if((_trantype==PayConnectService.transType.VOID || 
				(_trantype==PayConnectService.transType.RETURN && radioWebService.Checked))//The reference number is optional for terminal returns. 
				&& textRefNumber.Text=="") 
			{
				MsgBox.Show(this,"Ref Number required.");
				return false;
			}
			string paytype=ProgramProperties.GetPropVal(_progCur.ProgramNum,"PaymentType",_clinicNum);
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
					expYear=PIn.Int("20"+textExpDate.Text.Substring(3,2));
					expMonth=PIn.Int(textExpDate.Text.Substring(0,2));
				}
				else if(Regex.IsMatch(textExpDate.Text,@"^\d{4}$")) {//0807
					expYear=PIn.Int("20"+textExpDate.Text.Substring(2,2));
					expMonth=PIn.Int(textExpDate.Text.Substring(0,2));
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
			if(_creditCardCur==null) {//if the user selected a new CC, verify through PayConnect
				//using a new CC and the card number entered contains something other than digits
				if(textCardNumber.Text.Any(x => !char.IsDigit(x))) {
					MsgBox.Show(this,"Invalid card number.");
					return false;
				}
				if(!PayConnect.IsValidCardAndExp(textCardNumber.Text,expYear,expMonth,x => MessageBox.Show(x))) {//if exception happens, a message box will show with the error
					MsgBox.Show(this,"Card number or expiration date failed validation with PayConnect.");
					return false;
				}
			}
			else if(_creditCardCur.PayConnectToken=="" && Regex.IsMatch(textCardNumber.Text,@"X+[0-9]{4}")) {//using a stored CC
				MsgBox.Show(this,"There is no saved PayConnect token for this credit card.  The card number and expiration must be re-entered.");
				return false;
			}
			if(textNameOnCard.Text.Trim()=="" && _patCur!=null){//Name required for patient credit cards, not prepaid cards.
				MsgBox.Show(this,"Name On Card required.");
				return false;
			}
			if(_trantype==PayConnectService.transType.FORCE && textRefNumber.Text=="") {
				MsgBox.Show(this,"Authorization Code required.");
				return false;
			}
			//verify the selected clinic has a username and password type entered
			//Decrypt password first because empty string decrypted does not mean empty string when encrypted.
			string password=CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(_progCur.ProgramNum,"Password",_clinicNum));
			if(ProgramProperties.GetPropVal(_progCur.ProgramNum,"Username",_clinicNum)=="" || password=="") {//if username or password is blank
				MsgBox.Show(this,"The PayConnect username and/or password has not been set.");
				return false;
			}
			return true;
		}

		private void PrintReceipt(string receiptStr) {
			string[] receiptLines=receiptStr.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
			MigraDoc.DocumentObjectModel.Document doc=new MigraDoc.DocumentObjectModel.Document();
			doc.DefaultPageSetup.PageWidth=Unit.FromInch(3.0);
			doc.DefaultPageSetup.PageHeight=Unit.FromInch(0.181*receiptLines.Length+0.56);//enough to print receipt text plus 9/16 inch (0.56) extra space at bottom.
			doc.DefaultPageSetup.TopMargin=Unit.FromInch(0.25);
			doc.DefaultPageSetup.LeftMargin=Unit.FromInch(0.25);
			doc.DefaultPageSetup.RightMargin=Unit.FromInch(0.25);
			MigraDoc.DocumentObjectModel.Font bodyFontx=MigraDocHelper.CreateFont(8,false);
			bodyFontx.Name=FontFamily.GenericMonospace.Name;
			MigraDoc.DocumentObjectModel.Section section=doc.AddSection();
			Paragraph par=section.AddParagraph();
			ParagraphFormat parformat=new ParagraphFormat();
			parformat.Alignment=ParagraphAlignment.Left;
			parformat.Font=bodyFontx;
			par.Format=parformat;
			par.AddFormattedText(receiptStr,bodyFontx);
			MigraDoc.Rendering.Printing.MigraDocPrintDocument printdoc=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
			MigraDoc.Rendering.DocumentRenderer renderer=new MigraDoc.Rendering.DocumentRenderer(doc);
			renderer.PrepareDocument();
			printdoc.Renderer=renderer;
			if(ODBuild.IsDebug()) {
				using FormRpPrintPreview pView=new FormRpPrintPreview(printdoc);
				pView.ShowDialog();
			}
			else {
				try {
					ODprintout printout=PrinterL.CreateODprintout(
						printSit:PrintSituation.Receipt,
						auditPatNum:_patCur.PatNum,
						auditDescription:Lans.g(this,"PayConnect receipt printed")
					);
					if(PrinterL.TrySetPrinter(printout)) {
						printdoc.PrinterSettings=printout.PrintDoc.PrinterSettings;
						printdoc.Print();
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Printer not available.")+"\r\n"+Lan.g(this,"Original error")+": "+ex.Message);
				}
			}
		}

		private PayConnectService.signatureResponse SendSignature(string refNumber) {
			if(!sigBoxWrapper.GetSigChanged() || string.IsNullOrEmpty(sigBoxWrapper.GetSignature(""))) {
				return null;
			}
			PayConnectService.signatureRequest sigRequest=new PayConnectService.signatureRequest();
			sigRequest.RefNumber=refNumber;
			sigRequest.SignatureType=PayConnectService.signatureType.JPEG;
			using(Bitmap sigImage=sigBoxWrapper.GetSigImage())
			using(MemoryStream memStream=new MemoryStream()) {
				sigImage.Save(memStream,ImageFormat.Jpeg);
				byte[] imageBytes=memStream.ToArray();
				sigRequest.SignatureData=Convert.ToBase64String(imageBytes);
			}
			return PayConnect.ProcessSignature(sigRequest,_clinicNum,x => MessageBox.Show(x));
		}

		///<summary>Processes a PayConnect payment via the PayConnect web service.</summary>
		private bool ProcessPaymentWebService(int expYear,int expMonth) {
			string refNumber="";
			if(_trantype==PayConnectService.transType.VOID || _trantype==PayConnectService.transType.RETURN) {
				refNumber=textRefNumber.Text;
			}
			string magData=null;
			if(_parser!=null) {
				magData=_parser.Track2;
			}
			string cardNumber=textCardNumber.Text;
			//if using a stored CC and there is an X-Charge token saved for the CC and the user enters the whole card number to get a PayConnect token
			//and the number entered doesn't have the same last 4 digits and exp date, then assume it's not the same card and clear out the X-Charge token.
			if(_creditCardCur!=null //using a saved CC
				&& !string.IsNullOrEmpty(_creditCardCur.XChargeToken) //there is an X-Charge token saved
				&& (StringTools.TruncateBeginning(cardNumber,4)!=StringTools.TruncateBeginning(_creditCardCur.CCNumberMasked,4) //the card number entered doesn't have the same last 4 digits
					|| expYear!=_creditCardCur.CCExpiration.Year //the card exp date entered doesn't have the same year
					|| expMonth!=_creditCardCur.CCExpiration.Month)) //the card exp date entered doesn't have the same month
			{
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"The card number or expiration date entered does not match the X-Charge card on file.  Do you wish "
					+"to replace the X-Charge card with this one?"))
				{
					_creditCardCur.XChargeToken="";
				}
				else {
					Cursor=Cursors.Default;
					return false;
				}
			}
			//if the user has chosen to store CC tokens and the stored CC has a token and the token is not expired,
			//then use it instead of the CC number and CC expiration.
			if(checkSaveToken.Checked
				&& _creditCardCur!=null //if the user selected a saved CC
				&& _creditCardCur.PayConnectToken!="" //there is a stored token for this card
				&& _creditCardCur.PayConnectTokenExp.Date>=DateTime.Today.Date) //the token is not expired
			{
				cardNumber=_creditCardCur.PayConnectToken;
				expYear=_creditCardCur.PayConnectTokenExp.Year;
				expMonth=_creditCardCur.PayConnectTokenExp.Month;
			}
			else if(PIn.Bool(ProgramProperties.GetPropVal(_progCur.ProgramNum,PayConnect.ProgramProperties.PayConnectPreventSavingNewCC,_clinicNum))) {
				MsgBox.Show(this,"Cannot add a new credit card.");
				return false;
			}
			string authCode="";
			if(_trantype==PayConnectService.transType.FORCE) {
				authCode=textRefNumber.Text;
			}
			_request=PayConnect.BuildSaleRequest(PIn.Decimal(textAmount.Text),cardNumber,expYear,
				expMonth,textNameOnCard.Text,textSecurityCode.Text,textZipCode.Text,magData,_trantype,refNumber,checkSaveToken.Checked,authCode,checkForceDuplicate.Checked);
			_response=PayConnect.ProcessCreditCard(_request,_clinicNum,x => MessageBox.Show(x));
			if(_response==null || _response.Status.code!=0) {//error in transaction
				return false;
			}
			PayConnectService.signatureResponse sigResponse=SendSignature(_response.RefNumber);			
			if((ListTools.In(_trantype,PayConnectService.transType.SALE,PayConnectService.transType.RETURN,PayConnectService.transType.VOID))
					&& _response.Status.code==0) {//Only print a receipt if transaction is an approved SALE, RETURN, or VOID			
				_receiptStr=PayConnect.BuildReceiptString(_request,_response,sigResponse,_clinicNum);
				PrintReceipt(_receiptStr);
			}
			if(!PrefC.GetBool(PrefName.StoreCCnumbers) && !checkSaveToken.Checked) {//not storing the card number or the token
				return true;
			}
			//response must be non-null and the status code must be 0=Approved
			//also, the user must have the pref StoreCCnumbers enabled or they have the checkSaveTokens checked
			if(_creditCardCur==null) {//user selected Add new card from the payment window, save it or its token depending on settings
				_creditCardCur=new CreditCard();
				_creditCardCur.IsNew=true;
				_creditCardCur.PatNum=_patCur.PatNum;
				List<CreditCard> itemOrderCount=CreditCards.Refresh(_patCur.PatNum);
				_creditCardCur.ItemOrder=itemOrderCount.Count;
			}
			_creditCardCur.CCExpiration=new DateTime(expYear,expMonth,DateTime.DaysInMonth(expYear,expMonth));
			if(PrefC.GetBool(PrefName.StoreCCnumbers)) {
				_creditCardCur.CCNumberMasked=textCardNumber.Text;
			}
			else {
				_creditCardCur.CCNumberMasked=StringTools.TruncateBeginning(textCardNumber.Text,4).PadLeft(textCardNumber.Text.Length,'X');
			}
			_creditCardCur.Zip=textZipCode.Text;
			_creditCardCur.PayConnectToken="";
			_creditCardCur.PayConnectTokenExp=DateTime.MinValue;
			//Store the token and the masked CC number (only last four digits).
			if(checkSaveToken.Checked && _response.PaymentToken!=null) {
				_creditCardCur.PayConnectToken=_response.PaymentToken.TokenId;
				_creditCardCur.PayConnectTokenExp=new DateTime(_response.PaymentToken.Expiration.year,_response.PaymentToken.Expiration.month,
				DateTime.DaysInMonth(_response.PaymentToken.Expiration.year,_response.PaymentToken.Expiration.month));
			}
			_creditCardCur.CCSource=CreditCardSource.PayConnect;
			if(_creditCardCur.IsNew) {
				_creditCardCur.ClinicNum=_clinicNum;
				_creditCardCur.Procedures=PrefC.GetString(PrefName.DefaultCCProcs);
				CreditCards.Insert(_creditCardCur);
			}
			else {
				if(_creditCardCur.CCSource==CreditCardSource.XServer) {//This card has also been added for XCharge.
					_creditCardCur.CCSource=CreditCardSource.XServerPayConnect;
				}
				CreditCards.Update(_creditCardCur);
			}
			return true;
		}

		///<summary>Processes a PayConnect payment via a credit card terminal.</summary>
		private bool ProcessPaymentTerminal() {
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
			progressOD.ActionMain=() => _posResponse=DpsPos.ProcessCreditCard(posRequest);
			progressOD.ShowCancelButton=false;
			progressOD.StartingMessage=Lan.g(this,"Processing payment on terminal");
			progressOD.StopNotAllowedMessage=Lan.g(this,"Not allowed to stop. Please wait up to 2 minutes.");
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				MessageBox.Show(Lan.g(this,"Error processing card:")+" "+ex.Message);
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			if(_posResponse==null) {
				MessageBox.Show(Lan.g(this,"Error processing card"));
				return false;
			}
			if(_posResponse.ResponseCode!="0") {//"0" indicates success. May need to check the AuthCode field too to determine if this was a success.
				MessageBox.Show(Lan.g(this,"Error message from Pay Connect:")+"\r\n"+_posResponse.ResponseDescription);
				return false;
			}
			PayConnectService.signatureResponse sigResponse=null;
			try {
				Cursor=Cursors.WaitCursor;
				sigResponse=SendSignature(_posResponse.ReferenceNumber.ToString());
				Cursor=Cursors.Default;
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Card successfully charged. Error processing signature:")+" "+ex.Message);
			}
			textCardNumber.Text=_posResponse.CardNumber;
			textAmount.Text=_posResponse.Amount.ToString("f");
			_receiptStr=PayConnectTerminal.BuildReceiptString(posRequest,_posResponse,sigResponse,_clinicNum);
			PrintReceipt(_receiptStr);
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			int expYear;
			int expMonth;
			if(!VerifyData(out expYear,out expMonth)) {
				Cursor=Cursors.Default;
				return;
			}
			bool isSuccess;
			if(radioWebService.Checked) {
				isSuccess=ProcessPaymentWebService(expYear,expMonth);
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
	}
}