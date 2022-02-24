using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This table will never delete records, only upsert.  PayConnectResponseWeb rows are records of all payments made from the
	///Patient Portal via either PayConnect's Web Portal, or PayConnect's Merchant Services WebService if using a credit card token as a result of PayConnect's Web Portal.</summary>
	[Serializable]
	public class PayConnectResponseWeb:TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayConnectResponseWebNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to payment.PayNum.</summary>
		public long PayNum;
		///<summary>Enum:CreditCardSource .</summary>
		public CreditCardSource CCSource;
		///<summary>The amount of the payment that is attempting to be made.</summary>
		public double Amount;
		///<summary>The note entered when making a payment.</summary>
		public string PayNote;
		///<summary>The account token used to poll the processing status.</summary>
		public string AccountToken;
		///<summary>The payment token used to poll the processing status.</summary>
		public string PayToken;
		///<summary>Enum:PayConnectWebStatus Used to determine if the payment is pending, needs action, or is completed and attached to a payment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public PayConnectWebStatus ProcessingStatus;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual datetime of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>DateTime that the payment went to the pending status.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimePending;
		///<summary>DateTime that the payment went to the completed status and is attached to a payment.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeCompleted;
		///<summary>DateTime that the payment opportunity time expired.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeExpired;
		///<summary>DateTime of the last time that the payment had an error.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeLastError;
		///<summary>Raw JSON response (or error) from PayConnect.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string LastResponseStr;
		///<summary>Whether or not the credit card token can be saved for future uses.</summary>
		public bool IsTokenSaved;
		///<summary>The payment token used for future payments.</summary>
		public string PaymentToken;
		///<summary>Provides the Expiration Date of the account being accessed. Format is yyMM from XWeb gateway. Will be converted to ExpirationDate.</summary>
		public string ExpDateToken;
		///<summary>The RefNumber associated to this transaction.  Will only be set for Completed PayConnectWebStatuses.</summary>
		public string RefNumber;
		///<summary>The Transaction Type associated to this transaction.  Will only be set for Completed PayConnectWebStatuses.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public PayConnectService.transType TransType;

		///<summary>Returns whether or not this payment was made from PayConnect's Web Portal (where they would need to enter their credit card information).
		///If false, the payment was made via existing credit card token that was saved from PayConnect's Web Portal.</summary>
		public bool IsFromWebPortal {
			get {
				return !string.IsNullOrWhiteSpace(AccountToken) && !string.IsNullOrWhiteSpace(LastResponseStr);
			}
		}

		///<summary>Returns the DateTime of the greatest value between DateTimeEntry, DateTimePending, and DateTimeLastError.
		///This does not consider DateTimeCompleted or DateTimeExpired, as those are DateTimes that make the PayConnectResponseWeb done.</summary>
		public DateTime GetLastPendingUpdateDateTime() {
			return CodeBase.ODMathLib.Max(DateTimeEntry,CodeBase.ODMathLib.Max(DateTimePending,DateTimeLastError));
		}

		public CreditCard ToCreditCard() {
			try {
				//This is the class layout for the fields to be pulled out from the successful response string stored in the table
				var responseTypeParial=new {
					CreditCardNumber="",
					CreditCardExpireDate="",
				};
				//Pull out all of the field values we care about from the response string
				var responseValues=JsonConvert.DeserializeAnonymousType(LastResponseStr,responseTypeParial);
				return new CreditCard() {
					PatNum=PatNum,
					PayConnectToken=PaymentToken,
					CCNumberMasked=responseValues.CreditCardNumber,
					CCExpiration=new DateTime(2000+int.Parse(responseValues.CreditCardExpireDate.Substring(2,2)),int.Parse(responseValues.CreditCardExpireDate.Substring(0,2)),1),//CCExpDate is stored as MMyy from PayConnect
					CCSource=CCSource,
					ClinicNum=Patients.GetPat(PatNum).ClinicNum,
					ItemOrder=CreditCards.Refresh(PatNum).Count,
					Address="",
					Zip="",
					ChargeAmt=0,
					DateStart=DateTime.MinValue,
					DateStop=DateTime.MinValue,
					Note="",
					PayPlanNum=0,
					XChargeToken="",
					PayConnectTokenExp=new DateTime(2000+int.Parse(ExpDateToken.Substring(0,2)),int.Parse(ExpDateToken.Substring(2,2)),1),//ExpDateToken is stored as yyMM
					PaySimpleToken="",
					Procedures="",
				};
			}
			catch(Exception e) {
				throw new Exception("Error creating credit card from PayConnect web payment: "+e.Message,e);
			}
		}

		///<summary>Formats a note that can be used as a PayNote on a payment. If the PayConnectResponseWeb is a return or a void of a positive payment, pass in
		///false for keepAmountPositive.</summary>
		public string GetFormattedNote(bool keepAmountPositive) {
			//This is the class layout for the fields to be pulled out from the successful response string stored in the table
			var responseTypeParial=new {
				CreditCardNumber="",
				TransactionID=0,
			};
			//Pull out all of the field values we care about from the response string
			var responseValues=JsonConvert.DeserializeAnonymousType(LastResponseStr,responseTypeParial);
			DateTime dateTimeProcessed;
			if(DateTimeEntry.Year>1880 || DateTimeCompleted.Year>1880) {
				dateTimeProcessed=(DateTimeEntry>DateTimeCompleted ? DateTimeEntry : DateTimeCompleted);//The greater of the two dates
			}
			else {
				dateTimeProcessed=DateTime.Now;
			}
			return Lans.g(this,"Amount:")+" "+(keepAmountPositive ? Amount : -Amount).ToString("f")+"\r\n"
				+Lans.g(this,"Card Number:")+" "+responseValues.CreditCardNumber+"\r\n"
				+Lans.g(this,"Transaction ID:")+" "+responseValues.TransactionID+"\r\n"
				+Lans.g(this,"Processed:")+" "+dateTimeProcessed.ToShortDateString()+" "+dateTimeProcessed.ToShortTimeString()+"\r\n"
				+Lans.g(this,"Note:")+" "+PayNote;
		}
	}


	public enum PayConnectWebStatus {
		///<summary>0.</summary>
		Created,
		///<summary>1.</summary>
		CreatedError,
		///<summary>2.</summary>
		Pending,
		///<summary>3.</summary>
		PendingError,
		///<summary>4.</summary>
		Expired,
		///<summary>5.</summary>
		Completed,
		///<summary>6.</summary>
		Cancelled,
		///<summary>7.</summary>
		Declined,
		///<summary>8.</summary>
		Unknown,
		///<summary>9.</summary>
		UnknownError,
	}
}
