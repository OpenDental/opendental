using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{
	
	/// <summary>One electronic transaction.  Typically, one claim or response.  Or one benefit request or response.  Is constantly being expanded to include more types of transactions with clearinghouses.  Also stores printing of paper claims.  Sometimes stores a copy of what was sent.</summary>
	[Serializable]
	public class Etrans:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EtransNum;
		///<summary>The date and time of the transaction.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime DateTimeTrans;
		///<summary>FK to clearinghouse.ClearinghouseNum .  Can be 0 if no clearinghouse was involved.</summary>
		public long ClearingHouseNum;
		///<summary>Enum:EtransType</summary>
		public EtransType Etype;
		///<summary>FK to claim.ClaimNum if a claim. Otherwise 0.  Warning.  Original claim might have been deleted.  But if Canadian claim was successfully sent, then deletion will be blocked.</summary>
		public long ClaimNum;
		///<summary>For Canada.  Unique for every transaction sent.  Increments by one until 999999, then resets to 1.</summary>
		public int OfficeSequenceNumber;
		///<summary>For Canada.  Separate counter for each carrier.  Increments by one until 99999, then resets to 1.</summary>
		public int CarrierTransCounter;
		///<summary>For Canada.  If this claim includes secondary, then this is the counter for the secondary carrier.</summary>
		public int CarrierTransCounter2;
		///<summary>FK to carrier.CarrierNum.</summary>
		public long CarrierNum;
		///<summary>FK to carrier.CarrierNum Only used if secondary insurance info is provided on a claim.  Necessary for Canada.</summary>
		public long CarrierNum2;
		///<summary>FK to patient.PatNum This is useful in case the original claim has been deleted.  Now, we can still tell who the patient was.</summary>
		public long PatNum;
		///<summary>Maxes out at 999, then loops back to 1.  This is not a good key, but is a restriction of (canadian?).  So dates must also be used to isolate the correct BatchNumber key.  Specific to one clearinghouse.  Only used with e-claims.  Claim will have BatchNumber, and 997 will have matching BatchNumber. (In X12 lingo, it's a functional group number)</summary>
		public int BatchNumber;
		///<summary>A=Accepted, R=Rejected, blank if not able to parse, Recd=Received (835s only).  More options will be added later.  The incoming 997 or 999 sets this flag automatically.  To find the 997 or 999, look for a matching BatchNumber with a similar date, since both the claims and the 997 or 999 will both have the same batch number.  The 997 or 999 does not have this flag set on itself.</summary>
		public string AckCode;
		///<summary>For sent e-claims, within each batch (functional group), each carrier gets it's own transaction set.  Since 997s and 999s acknowledge transaction sets rather than batches, we need to keep track of which transaction set each claim is part of as well as which batch it's part of.  This field can't be set as part of 997 or 999, because one 997 or 999 refers to multiple trans sets.</summary>
		public int TransSetNum;
		///<summary>Typical uses include indicating that the report was printed, the claim was resent, reason for rejection, etc.  For a 270, this contains the automatically generated short summary of the response.  The response could include the reason for failure, or it could be a short summary of the 271.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>FK to etransmessagetext.EtransMessageTextNum.  Can be 0 if there is no message text.  Multiple Etrans objects can refer to the same message text, very common in a batch.</summary>
		public long EtransMessageTextNum;
		///<summary>FK to etrans.EtransNum.  Only has a non-zero value if there exists an ack etrans, like a 997, 999, 277ack, 271, 835, or ackError.
		///There can be only one ack for any given etrans, but one ack can apply to multiple etran's that were sent as one batch.
		///999 FK can be replaced by 277ack FK, and then by 835 FK.  This column does triple duty.
		///The AckEtransNum can be used to chain together related etrans entries.  For example,
		///if this is a 270 request, then AckEtransNum points to the 271 response.
		///If this is a 271, then AckEtransNum points to the HTML (if any) for the response.</summary>
		public long AckEtransNum;
		///<summary>FK to insplan.PlanNum.  Used if EtransType.BenefitInquiry270 and BenefitResponse271 and Eligibility_CA.</summary>
		public long PlanNum;
		///<summary>FK to inssub.InsSubNum.  Used if EtransType.BenefitInquiry270 and BenefitResponse271 and Eligibility_CA.</summary>
		public long InsSubNum;
		///<summary>X12 ST02 Transaction Set Identifier for an 835.  Specifies the unique transaction id within the 835 that this etrans record corresponds to.  This column will always be set for 835s imported in version 14.3 or greater.  For 835s imported in version 14.2, this column will alway be blank.  If blank, and there is more than one transaction id within the 835, then FormEtrans835PickEob will show and allow the user to select the desired EOB from a list.  The X12 guide states that there is only one transaction (EOB) allowed per 835, but ClaimConnect returns multiple transactions (EOBs) within a single 835 and other clearinghouses probably do as well.  When an 835 is imported, it is examined to determine the number of transactions within it.  One etrans entry is created for each EOB within the 835.  We may have a similar issue with multiple transactions within 277s as well, but we have not seen any evidence yet.  Our current 277 implementation expects a single transaction, just as the X12 standard specifies.</summary>
		public string TranSetId835;
		///<summary>Only used if the CarrierNum is 0.  If CarrierNum is not 0, the name associated to CarrierNum will override
		///CarrierNameRaw in the FormClaimsSend history grid.  Added for 835s so that customer databases are not cluttered with dummy carriers and
		///so there is no extra processing time when FormClaimsSend is loading.  Size is 60 bytes to match 835 carrier name length.</summary>
		public string CarrierNameRaw;
		///<summary>Only used if the PatNum is 0.  If PatNum is not 0, the name associated to PatNum will override PatientNameRaw
		///in the FormClaimsSend history grid.  Added for 835s so that there is no extra processing time when FormClaimsSend is loading,
		///and so text representing the patient count can be used instead of an actual patient name.  Size is 133 bytes to match X12 specs for 
		///last name (60), first name (35), middle name (25), suffix (10), and spaces in between (3).</summary>
		public string PatientNameRaw;
		///<summary>FK to userod.UserNum</summary>
		public long UserNum;
		///<summary>Not a database column.  Must be manually set, typically used to save queries.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public Etrans AckEtrans;
		///<summary>Not a database column.  Must be manually set, typically used to save queries.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string MessageText;

		///<summary></summary>
		public Etrans Copy(){
			return (Etrans)this.MemberwiseClone();
		}

	}

	///<summary>The _CA of some types should get stripped off when displaying to users.</summary>
	public enum EtransType {
		///<summary>0 X12-837.  Should we differenitate between different kinds of 837s and 4010 vs 5010?</summary>
		ClaimSent,
		///<summary>1 claim</summary>
		ClaimPrinted,
		///<summary>2 Canada. Type 01</summary>
		[EtransTypeAttr(true,true)]
		Claim_CA,
		///<summary>3 Renaissance</summary>
		Claim_Ren,
		///<summary>4 Canada. Type 11</summary>
		[EtransTypeAttr(true,false,true)]
		ClaimAck_CA,
		///<summary>5 Canada. Type 21</summary>
		[EtransTypeAttr(true,false,true)]
		ClaimEOB_CA,
		///<summary>6 Canada. Type 08</summary>
		[EtransTypeAttr(true,true)]
		Eligibility_CA,
		///<summary>7 Canada. Type 18. V02 type 10.</summary>
		[EtransTypeAttr(true,false)]
		EligResponse_CA,
		///<summary>8 Canada. Type 02</summary>
		[EtransTypeAttr(true,true)]
		ClaimReversal_CA,
		///<summary>9 Canada. Type 03</summary>
		[EtransTypeAttr(true,true)]
		Predeterm_CA,
		///<summary>10 Canada. Type 04</summary>
		[EtransTypeAttr(true,true)]
		RequestOutstand_CA,
		///<summary>11 Canada. Type 05</summary>
		[EtransTypeAttr(true,true)]
		RequestSumm_CA,
		///<summary>12 Canada. Type 06</summary>
		[EtransTypeAttr(true,true)]
		RequestPay_CA,
		///<summary>13 Canada. Type 07</summary>
		[EtransTypeAttr(true,true)]
		ClaimCOB_CA,
		///<summary>14 Canada. Type 12</summary>
		[EtransTypeAttr(true,false)]
		ReverseResponse_CA,
		///<summary>15 Canada. Type 13</summary>
		[EtransTypeAttr(true,false,true)]
		PredetermAck_CA,
		///<summary>16 Canada. Type 23</summary>
		[EtransTypeAttr(true,false,true)]
		PredetermEOB_CA,
		///<summary>17 Canada. Type 14</summary>
		[EtransTypeAttr(true,false,true)]
		OutstandingAck_CA,
		///<summary>18 Canada. Type 24</summary>
		[EtransTypeAttr(true,false,true)]
		EmailResponse_CA,
		///<summary>19 Canada. Type 16</summary>
		[EtransTypeAttr(true,false)]
		PaymentResponse_CA,
		///<summary>20 Canada. Type 15</summary>
		[EtransTypeAttr(true,false)]
		SummaryResponse_CA,
		///<summary>21 Ack from clearinghouse. X12-997.</summary>
		Acknowledge_997,
		///<summary>22 X12-277. Unsolicited claim status notification.</summary>
		StatusNotify_277,
		///<summary>23 Text report from clearinghouse in human readable format.</summary>
		TextReport,
		///<summary>24 X12-270.</summary>
		BenefitInquiry270,
		///<summary>25 X12-271</summary>
		BenefitResponse271,
		///<summary>26 When a Canadian message is sent, and an error comes back instead of a message.  This stores information about the error.  The etrans with this type is attached it to the original etrans as an ack.</summary>
		AckError,
		///<summary>27 X12-835. Electronic Remittance Advice (ERA).  Also known an an electronic EOB.</summary>
		ERA_835,
		///<summary>28 Ack from clearinghouse. X12-999.</summary>
		Acknowledge_999,
		///<summary>29 Simple and generic ack from clearinghouse which is used to replace 997s, 999s, or 277s.</summary>
		Ack_Interchange,
		///<summary>30 Carrier RAMQ located in Quebec Canada.</summary>
		[EtransTypeAttr(true,true)]
		Claim_Ramq,
		///<summary>31 Canadian iTrans 2.0 users can download carrier information.</summary>
		ItransNcpl,
		///<summary>32 HTML response from clearinghouse.  Usually in addition to a 271 used to import benefits.</summary>
		HTML,
		///<summary>33 DXC Attachments. We make etrans entries for all communication with DXC's API.</summary>
		DXCAttachments,
	}

	public class EtransTypeAttr:Attribute {
		[DefaultValue(false)]
		public bool IsCanadaType;
		///<summary>True if the etrans type is used for outgoing requests, false if the etrans type is for received responses.</summary>
		public bool IsRequestType;
		///<summary>The only allowed response transaction types are defined on page 38 in version 4.1b and page 18 in version 2.8.9.9.6: 21 EOB Response, 11 Claim Ack, 14 Outstanding Transactions Response, 23 Predetermination EOB, 13 Predetermination Ack, 24 E-Mail Response</summary>
		public bool IsRotResponseType;

		public EtransTypeAttr() {
			//Need empty constructor for ODPrimitiveExtensions.GetAttributeOrDefault(...)		
		}

		public EtransTypeAttr(bool isCanadaType = false,bool isRequestType = false,bool isRotResponseType = false) {
			IsCanadaType=isCanadaType;
			IsRequestType=isRequestType;
			IsRotResponseType=isRotResponseType;
		}
	}

}

















