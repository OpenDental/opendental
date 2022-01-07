using System;
using System.Collections;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods = true)]
	public class PatientPortalInvite:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatientPortalInviteNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to appointment.AptNum.</summary>
		public long AptNum;
		///<summary>FK to clinic.ClinicNum. The clinic that is sending this invite.</summary>
		public long ClinicNum;
		///<summary>When this invite was entered into the database.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>This was the TSPrior used to send this invite.</summary>
		[XmlIgnore]
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong)]
		public TimeSpan TSPrior;
		///<summary>Enum:AutoCommStatus The status of sending the email for this invite.</summary>
		public AutoCommStatus EmailSendStatus;
		///<summary>FK to emailmessage.EmailMessageNum. The email message that was sent to the patient.</summary>
		public long EmailMessageNum;
		///<summary>The template that will be used when creating the body of the email message.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string TemplateEmail;
		///<summary>The template that will be used for the email subject line.</summary>
		public string TemplateEmailSubj;
		///<summary>Any notes regarding this invite.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("TSPrior",typeof(long))]
		public long TSPriorXml {
			get {
				return TSPrior.Ticks;
			}
			set {
				TSPrior=TimeSpan.FromTicks(value);
			}
		}

		///<summary></summary>
		public PatientPortalInvite Copy() {
			return (PatientPortalInvite)this.MemberwiseClone();
		}
	}
}

