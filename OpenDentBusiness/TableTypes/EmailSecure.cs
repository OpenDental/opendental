using System;

namespace OpenDentBusiness {
	///<summary>Tracks every secure email sent and received from or to a patient.</summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true)]
	public class EmailSecure:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailSecureNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to emailmessage.EmailMessageNum.  0 indicates email has not been successfully downloaded from API yet.</summary>
		public long EmailMessageNum;
		///<summary>FK to emailchain, as hosted by API.  Table does not exist at dental office.</summary>
		public long EmailChainFK;
		///<summary>FK to email, as hosted by API.  Table does not exist at dental office.</summary>
		public long EmailFK;
		///<summary>DateTime the entry was inserted</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntry;
		///<summary>DateTime the entry was edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		public EmailSecure() {
		}
	}
}
