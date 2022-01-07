using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>DEPRECATED. As of 17.1, this table is no longer used by HQ. It has been replaced by EServiceBilling.
	///Only used internally by OpenDental, Inc.  Not used by anyone else. Aggregates customer charges for integrated texting.</summary>
	[Serializable()]
	[CrudTable(IsMissingInGeneral=true)]//Remove this line to perform one-time CRUD updated to this class. Place back before committing changes.
	public class SmsBilling:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SmsBillingNum;
		///<summary>Should be unique in this table per DateUsage.</summary>
		public long RegistrationKeyNum;
		///<summary>Should be unique in this table per DateUsage.</summary>
		public long CustPatNum;
		///<summary>Indicates which calendar month this usage metric is for. Should be 1st of month at midnight. Example '2012-01-01 00:00:00'</summary>
		public DateTime DateUsage;
		///<summary>The aggregate cost of all message parts charged by Open Dental to the customer. Will need to be summed as the sum of all parts of multi-part messages.
		///Is a function of MsgCost and is calculated from SMSResponse as a result of SendSMS. Maps to OpenDentBusiness.SmsToMobile.MsgCostUSD.</summary>
		public float MsgChargeTotalUSD;
		///<summary>The aggregate cost of all clinic access charges. Calculated using ClinicsActive * RepeatingCharge.ChargeAmt WHERE ProcCode='038'.</summary>
		public float AccessChargeTotalUSD;
		///<summary>Total number of clinics that have used Integrated Texting in the past for this customer.</summary>
		public int ClinicsTotalCount;		
		///<summary>Subset of ClinicsTotal. Represents number of clinics which have no inactive date or an inactive date on or after 1st of the given calendar month. 
		///These clinics should each be included in the repeating charge for the given calendar month.</summary>
		public int ClinicsActiveCount;
		///<summary>Subset of ClinicsTotal. Represents number of clinics which actually accrued messaging charges for the given calendar month.</summary>
		public int ClinicsUsedCount;
		///<summary>Total number of phones for this customer, active or archived.</summary>
		public int PhonesTotalCount;
		///<summary>Subset of PhonesTotal. Represents number of Phones which have no inactive date or an inactive date on or after 1st of the given calendar month.</summary>
		public int PhonesActiveCount;
		///<summary>Subset of PhonesTotal. Represents number of Phones which actually accrued messaging charges for the given calendar month.</summary>
		public int PhonesUsedCount;
		///<summary>Sum of messages in MTerminate and MTSend which were succesfully transmitted to patients for this customer for the given calendar month.</summary>
		public int MsgSentOkCount;
		///<summary>Sum of messages in MOTerminate which were succesfully transmitted to the customer for the given calendar month. MOUnsent messages will not be included in this count as they have not technically been sent on to the customer yet.</summary>
		public int MsgRcvOkCount;
		///<summary>Sum of messages in MTerminate which expired before being transmitted to patients for this customer for the given calendar month.</summary>
		public int MsgSentFailCount;
		///<summary>Sum of messages in MOerminate which expired before being transmitted to the customer for the given calendar month. MOUnsent messages will not be included in this count as they have not technically been sent on to the customer yet.</summary>
		public int MsgRcvFailCount;
		///<summary>Number of clinics which used confirmations for the giving billing cycle. This will be used to determine total charge.</summary>
		public int ConfirmationClinics;		
		///<summary>Sum of ConfirmationsPending and ConfirmationsTerminated.</summary>
		public int ConfirmationsTotal;
		///<summary>Sum of ConfirmationsPending and ConfirmationsTerminated which were sent by email.</summary>
		public int ConfirmationsEmail;
		///<summary>Sum of ConfirmationsPending and ConfirmationsTerminated which were sent by SMS.</summary>
		public int ConfirmationsSms;
		///<summary>RegKey will be charged RepeatCharge '040' multiplied by number of clinics which participated in confirmations for the given month.
		///If no clinics participate then there is no charge for confirmations.
		///Any clinic which uses texting but no confirmations will be charged '038' proc fee. 
		///Any clinic which uses confirmation and texting will be charged only the '040' proc fee.
		///In short, texting is included for free if you use confirmations.</summary>
		public float ConfirmationChargeTotalUSD;
		///<summary>Human readable explanation of how the SMS charges were calculated.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BillingDescSms;
		///<summary>Human readable explanation of how the confirmation charges were calculated.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string BillingDescConfirmation;

		public SmsBilling Clone() {
			return (SmsBilling)this.MemberwiseClone();
		}	
	}
}

/*
CREATE TABLE smsbilling (
	SmsBillingNum bigint NOT NULL auto_increment PRIMARY KEY,
	RegistrationKeyNum bigint NOT NULL,
	CustPatNum bigint NOT NULL,
	DateUsage date NOT NULL DEFAULT '0001-01-01',
	MsgChargeTotalUSD float NOT NULL,
	AccessChargeTotalUSD float NOT NULL,
	ClinicsTotalCount int NOT NULL,
	ClinicsActiveCount int NOT NULL,
	ClinicsUsedCount int NOT NULL,
	PhonesTotalCount int NOT NULL,
	PhonesActiveCount int NOT NULL,
	PhonesUsedCount int NOT NULL,
	MsgSentOkCount int NOT NULL,
	MsgRcvOkCount int NOT NULL,
	MsgSentFailCount int NOT NULL,
	MsgRcvFailCount int NOT NULL,
	INDEX(RegistrationKeyNum),
	INDEX(CustPatNum)
	) DEFAULT CHARSET=utf8
*/






