using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
	///<summary>Lite version of the action item that AutoComm will be handling. Used to keep database I/O as small as possible for AutoComm.
	///Will most likely be extended into more concrete class for different flavors of AutoCommAbs.</summary>
	public class AutoCommObj {
		///<summary>AutoCommObj in this status are intended to be sent.</summary>
		public const AutoCommStatus STATUS_DO_SEND=AutoCommStatus.SendNotAttempted;

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
		///<summary>Whether the patient has Premed.</summary>
		public bool Premed;

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
		///<summary>Patient first name. This is the only identifier allowed in order to avoid HIPPA violations.</summary>
		public string NameF;
		///<summary>Patient preferred name, or first name if they don't have a preferred name.</summary>
		public string NamePreferredOrFirst;
		///<summary>The send status of the message.</summary>
		public AutoCommStatus SendStatus=AutoCommStatus.Undefined;
		///<summary>Indicates that a message should be attempted for this patient.</summary>
		public bool TrySend;
		///<summary>Indicates that the rule's 'DoNotSendWithin' should be ignored.</summary>
		public bool IgnoreDoNotSendWithin;
		///<summary>If this is true, then DtSend will not be changed after this field is set to true.</summary>
		public bool IsDtSendFinal;
		[XmlIgnore]
		///<summary>FK to the appropriate message table.</summary>
		public long MessageFk;
		[XmlIgnore]
		///<summary>AutoCommRule used for this message.</summary>
		public AutoCommRule AutoCommRule;
		[XmlIgnore]
		public PatComm PatComm;
		[XmlIgnore]
		public virtual long AptNum => PrimaryKey;
		[XmlIgnore]
		public string Note;

		///<summary>Indicates that a message succeeded for this patient.</summary>
		public bool Sent => IsSent(SendStatus);

		public static bool IsSent(AutoCommStatus status) => new List<AutoCommStatus> { AutoCommStatus.SendSuccessful,AutoCommStatus.SentAwaitingReceipt}.Contains(status);

		public AutoCommObj Copy() {
			return (AutoCommObj)MemberwiseClone();
		}

		public virtual void SetPatientContact(PatComm patComm,Dictionary<long,PatComm> dictPatComms) {
			if(patComm is null) {
				return;
			}
			NameF=patComm.FName;
			NamePreferredOrFirst=patComm.GetFirstOrPreferred();
			PatComm=patComm;
			Language=patComm.Language;
			Premed=patComm.Premed;
			//If patComm.Patnum doesn't match the ACO patnum then our patComm is the guarantor for the ACO.
			//Leave the contact info as the guarantor but change the name back to the patient.
			if(patComm.PatNum!=this.PatNum) {
				if(dictPatComms.TryGetValue(this.PatNum,out PatComm patientPatComm)) {
					NameF=patientPatComm.FName;
					NamePreferredOrFirst=patientPatComm.GetFirstOrPreferred();
				}
			}
		}

		public virtual void InsertCommlog(CommItemTypeAuto commTypeAuto,CommItemMode mode,CommItemSource source,string message) {
			if(mode==CommItemMode.Email) {
				return;//Emails already show in the Chart.
			}
			Commlogs.Insert(new Commlog() {
				CommDateTime=DateTime_.Now,
				CommSource=source,
				CommType=Commlogs.GetTypeAuto(commTypeAuto),
				Mode_=mode,
				PatNum=PatNum,
				SentOrReceived=CommSentOrReceived.Sent,
				UserNum=Security.CurUser?.UserNum??0,
				Note=message,
			});
		}
	}
}
