using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{
	///<summary>Tracks all forms of communications with patients, including emails, phonecalls, postcards, etc.  Any changes made to this table need to be added to CommlogHist and CommlogHists.CreateFromCommlog().</summary>
	//CommlogHist table changes require a manual query to update. H34896
	[Serializable]
	[CrudTable(IsLargeTable=true)]
	public class Commlog:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CommlogNum;
		///<summary>FK to patient.PatNum. This will be 0 if Referral.</summary>
		public long PatNum;
		///<summary>Date and time of entry</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime CommDateTime;
		///<summary>FK to definition.DefNum. This will be 0 if Referral.</summary>
		public long CommType;
		///<summary>Note for this commlog entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText | CrudSpecialColType.CleanText)]
		public string Note;
		///<summary>Enum:CommItemMode Phone, email, etc.</summary>
		public CommItemMode Mode_;
		///<summary>Enum:CommSentOrReceived Neither=0,Sent=1,Received=2.</summary>
		public CommSentOrReceived SentOrReceived;
		/////<Summary>No longer used.  Use the statement table instead.</Summary>
		//public bool IsStatementSent;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Signature.  For details, see procnote.Signature.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Signature;
		///<summary>True if signed using the Topaz signature pad, false otherwise.</summary>
		public bool SigIsTopaz;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Date and time when commlog ended.  Mainly for internal use.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeEnd;
		///<summary>Enum:CommItemSource Set to the source of the entity that created this commlog.  E.g. WebSched.</summary>
		public CommItemSource CommSource;
		///<summary>FK to program.ProgramNum.  This will be 0 unless CommSource is set to ProgramLink.</summary>
		public long ProgramNum;
		///<summary>Track Date Created for commlogs. Value for existing commlogs show as blank in the UI. Not editable by user.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntry;
		///<summary>FK to referral.ReferralNum.</summary>
		public long ReferralNum;
		///<summary>Enum:EnumCommReferralBehavior Changes how this referral commlog displays within grids.</summary>
		public EnumCommReferralBehavior CommReferralBehavior;

		///<summary></summary>
		public Commlog Copy(){
			return (Commlog)this.MemberwiseClone();
		}

		public bool Compare(Commlog commlogCur) {
			if(PatNum!=commlogCur.PatNum
				|| CommDateTime!=commlogCur.CommDateTime
				|| CommType!=commlogCur.CommType
				|| Note!=commlogCur.Note
				|| Mode_!=commlogCur.Mode_
				|| SentOrReceived!=commlogCur.SentOrReceived
				|| UserNum!=commlogCur.UserNum)
				//Do we want to try and save the signature?
			{
				return false;
			}
			return true;
		}
	}

	///<summary></summary>
	public enum CommItemMode {
		///<summary>0- </summary>
		None,
		///<summary>1- </summary>
		Email,
		///<summary>2</summary>
		Mail,
		///<summary>3</summary>
		Phone,
		///<summary>4</summary>
		[Description("In Person")]
		InPerson,
		///<summary>5</summary>
		Text,
		///<summary>6</summary>
		[Description("Email and Text")]
		EmailAndText,
		///<summary>7</summary>
		[Description("Phone and Text")]
		PhoneAndText,
	}

	///<summary>0=neither, 1=sent, 2=received.</summary>
	public enum CommSentOrReceived {
		///<summary>0</summary>
		Neither,
		///<summary>1</summary>
		Sent,
		///<summary>2</summary>
		Received
	}

	///<summary></summary>
	public enum CommItemSource {
		///<summary>0</summary>
		User,
		///<summary>1</summary>
		WebSched,
		///<summary>2</summary>
		ProgramLink,
		///<summary>3</summary>
		ApptReminder,
		///<summary>4 - HQ Only</summary>
		EServices,
		///<summary>5</summary>
		SupplementalBackup,
		///<summary>6</summary>
		ApptThankYou,
		///<summary>7 - Includes non-FHIR API.</summary>
		FHIR,
		///<summary>8</summary>
		NewPatThankYou
	}

	///<summary></summary>
	public enum EnumCommReferralBehavior {
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		TopAnchored,
		///<summary>2</summary>
		Hidden
	}


}