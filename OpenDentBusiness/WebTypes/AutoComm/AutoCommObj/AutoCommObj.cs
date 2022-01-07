using System;
using System.Collections.Generic;
using System.Data;

namespace OpenDentBusiness.AutoComm {
	///<summary>Lite version of the action item that AutoComm will be handling. Used to keep database I/O as small as possible for AutoComm.
	///Will most likely be extended into more concrete class for different flavors of AutoCommAbs.</summary>
	public class AutoCommObj {
		///<summary>Could be AptNum, RecallNum, etc.</summary>
		public long PrimaryKey;
		///<summary>The clinic where the action item is to take place.</summary>
		public long ClinicNum;
		///<summary>The PatNum for the action item.</summary>
		public long PatNum;
		///<summary>The provider for the action item.</summary>
		public long ProvNum;
		///<summary>When the action is supposed to take place.</summary>
		public DateTime DateTimeEvent;
		///<summary>Language the patient speaks. Could be blank if the patient doesn't have a language set.</summary>
		public string Language;

		//COMPUTED/AGGREGATE COLUMNS
		///<summary>Time at which a rule should be sent.</summary>
		private DateTime _dtSend=DateTime.MaxValue;
		///<summary>Time at which a rule should be sent. This value could be wrong after this object is deserialized.</summary>
		public DateTime DtSend {
			get {
				return _dtSend;
			}
			set {
				if(IsDtSendFinal) {
					return;//We are not allowed to change _dtSend.
				}
				_dtSend=value;
			}
		}
		///<summary>The recipient SMS phone number. If non-blank then assume this number can be texted.</summary>
		public string PhoneContact;
		///<summary>The recipient email. If non-blank then assume this email can be sent.</summary>
		public string EmailContact;
		///<summary>Patient first name. This is the only identifier allowed in order to avoid HIPPA violations.</summary>
		public string NameF;
		///<summary>The send status of the SMS.</summary>
		public AutoCommStatus SMSSendStatus=AutoCommStatus.Undefined;
		///<summary>The send status of the email.</summary>
		public AutoCommStatus EmailSendStatus=AutoCommStatus.Undefined;
		///<summary>Indicates that an SMS should be attempted for this patient.</summary>
		public bool TrySendSMS;
		///<summary>Indicates that an email should be attempted for this patient.</summary>
		public bool TrySendEmail;
		///<summary>Indicates that the rule's 'DoNotSendWithin' should be ignored.</summary>
		public bool IgnoreDoNotSendWithin;
		///<summary>If this is true, then DtSend will not be changed after this field is set to true.</summary>
		public bool IsDtSendFinal;
		///<summary>Used to keep track of the language that was sent to the patient in their email.</summary>
		public string LanguageEmail;
		///<summary>Used to keep track of the language that was sent to the patient in their SMS text.</summary>
		public string LanguageSMS;

		///<summary>Indicates that an SMS succeeded for this patient.</summary>
		public bool SentSMS { get { return SMSSendStatus==AutoCommStatus.SendSuccessful; } }

		///<summary>Indicates that an email succeeded for this patient.</summary>
		public bool SentEmail { get { return EmailSendStatus==AutoCommStatus.SendSuccessful; } }

		public AutoCommObj Copy() {
			return (AutoCommObj)MemberwiseClone();
		}

		public virtual void SetPatientContact(PatComm patComm,Dictionary<long,PatComm> dictPatComms) {
			if(patComm is null) {
				return;
			}
			NameF=patComm.GetFirstOrPreferred();
			PhoneContact=patComm.IsSmsAnOption ? patComm.SmsPhone : "";
			EmailContact=patComm.IsEmailAnOption ? patComm.Email : "";
			Language=patComm.Language;
		}
	}
}
