using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes.Shared.XWeb;

namespace OpenDental {
	public partial class FormXWeb:FormODBase {
		///<summary>The patient who is making this transaction.</summary>
		private long _patNum;
		///<summary>The credit card being used for this transaction.</summary>
		private CreditCard _creditCard;
		///<summary>The currently select transaction type.</summary>
		private XWebTransactionType _tranType;
		///<summary>Set to true to not allow the user to change the credit card information.</summary>
		public bool LockCardInfo;
		///<summary>True if a payment should be created from the transaction.</summary>
		private bool _createPayment;
		///<summary>The XWebResponse from the transaction. Will be null if the transaction was not successful.</summary>
		public XWebResponse ResponseResult;

		public FormXWeb(long patNum,CreditCard creditCard,XWebTransactionType tranType,bool createPayment) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
			_creditCard=creditCard;
			_tranType=tranType;
			_createPayment=createPayment;
		}

		private void FormXWeb_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today)) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(_tranType==XWebTransactionType.CreditReturnTransaction) {
				radioReturn.Checked=true;
			}
			if(_creditCard!=null) {
				textCardNumber.Text=_creditCard.CCNumberMasked;
				textExpDate.Text=_creditCard.CCExpiration.ToString("MMy");
				textZipCode.Text=_creditCard.Zip;
			}
			if(LockCardInfo) {
				textCardNumber.ReadOnly=true;
				textCardNumber.BackColor=SystemColors.Control;
				textExpDate.ReadOnly=true;
				textExpDate.BackColor=SystemColors.Control;
				textNameOnCard.ReadOnly=true;
				textNameOnCard.BackColor=SystemColors.Control;
				textSecurityCode.ReadOnly=true;
				textSecurityCode.BackColor=SystemColors.Control;
				textZipCode.ReadOnly=true;
				textZipCode.BackColor=SystemColors.Control;
				textAmount.Focus();
			}
		}

		///<summary>Returns true if all data is entered correctly, false otherwise.</summary>
		private bool VerifyData() {
			int expYear=0;
			int expMonth=0;
			if(textCardNumber.Text.Trim().Length<5) {
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
			if(_creditCard==null) {
				//using a new CC and the card number entered contains something other than digits
				if(textCardNumber.Text.Any(x => !char.IsDigit(x))) {
					MsgBox.Show(this,"Invalid card number.");
					return false;
				}
			}
			else if(_creditCard.XChargeToken=="" && Regex.IsMatch(textCardNumber.Text,@"X+[0-9]{4}")) {//using a stored CC
				MsgBox.Show(this,"There is no saved XWeb token for this credit card.  The card number and expiration must be re-entered.");
				return false;
			}
			if(!Regex.IsMatch(textAmount.Text,"^[0-9]+$") && !Regex.IsMatch(textAmount.Text,"^[0-9]*\\.[0-9]+$")) {
				MsgBox.Show(this,"Invalid amount.");
				return false;
			}
			if(_tranType==XWebTransactionType.CreditVoidTransaction && textRefNumber.Text=="") {
				MsgBox.Show(this,"Ref Number required.");
				return false;
			}
			if(textPayNote.Text=="") {
				MsgBox.Show(this,"Payment note required.");
				return false;
			}
			return true;
		}

		///<summary>Processes the selected XWeb transaction. Returns true if the payment was successful, false otherwise.</summary>
		private bool ProcessSelectedTransaction() {			
			double amount=PIn.Double(textAmount.Text);
			try {
				Cursor=Cursors.WaitCursor;
				if(_tranType==XWebTransactionType.CreditReturnTransaction) {
					ResponseResult=XWebs.ReturnPayment(_creditCard.PatNum,textPayNote.Text,amount,_creditCard.CreditCardNum,_createPayment);
				}
			}
			catch(ODException ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return false;
			}
			Cursor=Cursors.Default;
			return true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(radioReturn.Checked) {
				_tranType=XWebTransactionType.CreditReturnTransaction;
			}
			else {
				_tranType=XWebTransactionType.Undefined;
			}
			if(!VerifyData()) {
				return;
			}
			if(ProcessSelectedTransaction()) {
				DialogResult=DialogResult.OK;
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}