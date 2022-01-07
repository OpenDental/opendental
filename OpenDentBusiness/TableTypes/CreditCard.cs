using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>One credit card along with any recurring charge information.</summary>
	[Serializable]
	public class CreditCard:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CreditCardNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>.</summary>
		public string Address;
		///<summary>Postal code.</summary>
		public string Zip;
		///<summary>Token for X-Charge. Alphanumeric, upper and lower case, about 15 char long.  Passed into Xcharge instead of the actual card number.
		///Used for EdgeExpress as well.</summary>
		public string XChargeToken;
		///<summary>Credit Card Number.  Will be stored masked: XXXXXXXXXXXX1234.</summary>
		public string CCNumberMasked;
		///<summary>Only month and year are used, the day will usually be 1.</summary>
		public DateTime CCExpiration;
		///<summary>The order that multiple cards will show.  Zero-based.  First one will be default.</summary>
		public int ItemOrder;
		///<summary>Amount set for recurring charges.</summary>
		public Double ChargeAmt;
		///<summary>Start date for recurring charges.</summary>
		public DateTime DateStart;
		///<summary>Stop date for recurring charges.</summary>
		public DateTime DateStop;
		///<summary>Any notes about the credit card or account goes here.</summary>
		public string Note;
		///<summary>FK to payplan.PayPlanNum.</summary>
		public long PayPlanNum;
		///<summary>Token for PayConnect.  PayConnect returns a token and token expiration, when requested by the merchant's system, to be used instead
		///of actual credit card number in subsequent transactions.</summary>
		public string PayConnectToken;
		///<summary>Expiration for the PayConnect token.  Used with the PayConnect token instead of the actual credit card number and expiration.</summary>
		public DateTime PayConnectTokenExp;
		///<summary>What procedures will go on this card as a recurring charge.  Comma delimited list of ProcCodes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Procedures;
		///<summary>Enum:CreditCardSource Indicates which application made this credit card and token.</summary>
		public CreditCardSource CCSource;
		///<summary>FK to clinic.ClinicNum. The clinic where this card was added. Each clinic could have a different AuthKey and different
		///AuthKeys could generate overlapping tokens.</summary>
		public long ClinicNum;
		///<summary>Only used at OD HQ.  Excludes credit card from syncing default procedures.  False by default.</summary>
		public bool ExcludeProcSync;
		///<summary>Token for PaySimple.  PaySimple returns a token, when requested by the merchant's system, to be used instead
		///of actual credit card number in subsequent transactions.</summary>
		public string PaySimpleToken;
		///<summary>Stores how often the credit card gets charged for a recurring charge. The card can either be charged fixed days of the month or
		///fixed week days of the month. Some examples of the former are "4th day of the month" or "1st and 16th day of the month". Some examples of
		///the latter are "Third Monday of the month" or "Every other Friday of the month". If the first character of this column is a 0, then
		///the frequency is fixed day of the month. If the first character is 1, the frequency is fixed week days of the month. The next character
		///is a pipe for separation. If fixed day of the month, the remaining characters will be a comma-separated list of days of the month. 
		///If fixed week days, the next character will represent type of frequency (Every, EveryOther, First, etc.). Then a pipe follows. The last
		///character will be the day of the week (0 for Sunday, 1 for Monday, etc.).</summary>
		public string ChargeFrequency;
		///<summary>Set true to indicate the Credit Card in question can be charged when the Patient account balance is $0, which corresponds directly
		///to a preference called "RecurringChargesAllowedWhenPatNoBal" (true by default) which must be turned on via Module>Account>Misc to be 
		///available.</summary>
		public bool CanChargeWhenNoBal;
		///<summary>FK to definition.DefNum. Payment type override for recurring charges.</summary>
		public long PaymentType;
		///<summary>True by default.  Set to false to inactivate this specific credit card from the recurring charges (both manual and via the service).</summary>
		public bool IsRecurringActive=true;

		public bool IsXWeb() {
			return CCSource==CreditCardSource.XWeb || CCSource==CreditCardSource.XWebPortalLogin;
		}

		//TODO: hook this up to FormPayment for returns and voiding.
		public bool IsPayConnectPortal() {
			return CCSource==CreditCardSource.PayConnectPortal || CCSource==CreditCardSource.PayConnectPortalLogin;
		}

		///<summary>Gets a string of the companies the credit cards has tokens for.</summary>
		public string GetTokenString() {
			if(!Programs.HasMultipleCreditCardProgramsEnabled()
				|| (string.IsNullOrEmpty(XChargeToken) && string.IsNullOrEmpty(PayConnectToken) && string.IsNullOrEmpty(PaySimpleToken))) 
			{
				if(CCSource==CreditCardSource.PaySimpleACH) {
					return "("+Lans.g(this,"ACH")+")";
				}
				return "";
			}
			List<string> listTokens=new List<string>();
			if(!string.IsNullOrEmpty(XChargeToken)) {
				if(CCSource==CreditCardSource.EdgeExpressRCM || CCSource==CreditCardSource.EdgeExpressCNP) {
					listTokens.Add("EdgeExpress");
				}
				else {
					listTokens.Add("XCharge");
				}
			}
			if(!string.IsNullOrEmpty(PayConnectToken)) {
				listTokens.Add("PayConnect"+(IsPayConnectPortal() ? " Portal" : ""));
			}
			if(!string.IsNullOrEmpty(PaySimpleToken)) {
				listTokens.Add("PaySimple"+(CCSource==CreditCardSource.PaySimpleACH ? " "+Lans.g(this,"ACH") : ""));
			}
			return "("+string.Join(", ",listTokens)+")";
		}

		///<summary></summary>
		public CreditCard Copy() {
			return (CreditCard)this.MemberwiseClone();
		}
	}

	public enum CreditCardSource {
		///<summary>0 - This is used when the payment is not a Credit Card. If CC, then this means we are storing the actual credit card number. Not recommended.</summary>
		None,
		///<summary>1 - Local installation of X-Charge</summary>
		XServer,
		///<summary>2 - Credit card created via X-Web (an eService)</summary>
		XWeb,
		///<summary>3 - PayConnect web service (from within OD).</summary>
		PayConnect,
		///<summary>4 - Credit card has been added through the local installation of X-Charge and the PayConnect web service.</summary>
		XServerPayConnect,
		///<summary>5 - Made from the login screen of the Patient Portal.</summary>
		XWebPortalLogin,
		///<summary>6 - PaySimple web service (from within OD).</summary>
		PaySimple,
		///<summary>7 - PaySimple ACH web service (from within OD).</summary>
		PaySimpleACH,
		///<summary>8 - PayConnect credit card (made from Patient Portal)</summary>
		PayConnectPortal,
		///<summary>9 - PayConnect credit card (made from Patient Portal Login screen).</summary>
		PayConnectPortalLogin,
		///<summary>10 - CareCredit.</summary>
		CareCredit,
		///<summary>11 - EdgeExpress Cloud when calling the RCM program.</summary>
		EdgeExpressRCM,
		///<summary>12 - EdgeExpress Card Not Present API.</summary>
		EdgeExpressCNP,
		///<summary>13 - Payment taken through Open Dental API.</summary>
		API,
	}

	public enum ChargeFrequencyType {
		///<summary>0 - Charge occurs on a specific day or set of days.</summary>
		FixedDayOfMonth,
		///<summary>1 - Charge occurs on a specified weekday of every month. See DayOfWeekFrequency for how often on that day.</summary>
		FixedWeekDay,
	}

	public enum DayOfWeekFrequency {
		///<summary>0 - Charge occurs on every specified weekday of every month.</summary>
		Every,
		///<summary>1 - Charge occurs on every other specified weekday of every month.</summary>
		[Description("Every Other")]
		EveryOther,
		///<summary>2 - Charge occurs on 1st specified weekday of every month.</summary>
		First,
		///<summary>3 - Charge occurs on 2st specified weekday of every month.</summary>
		Second,
		///<summary>4 - Charge occurs on 3st specified weekday of every month.</summary>
		Third,
		///<summary>5 - Charge occurs on 4st specified weekday of every month.</summary>
		Fourth,
		///<summary>6 - Charge occurs on 5st specified weekday of every month.</summary>
		Fifth,
	}
}
