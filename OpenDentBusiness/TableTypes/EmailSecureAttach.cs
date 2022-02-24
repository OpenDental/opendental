using System;

namespace OpenDentBusiness {
	///<summary>Tracks every attachment linked to a secure email.</summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true)]
	public class EmailSecureAttach:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailSecureAttachNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>FK to emailattach.EmailAttachNum.  0 indicates attachment has not been successfully downloaded from API yet.</summary>
		public long EmailAttachNum;
		///<summary>FK to emailsecure.EmailSecureNum.</summary>
		public long EmailSecureNum;
		///<summary>Attachment identifier, as hosted by API.  Table does not exist at dental office.</summary>
		public string AttachmentGuid;
		///<summary>The displayed name of the file/object.</summary>
		public string DisplayedFileName;
		///<summary>The extension of the object (i.e. png).</summary>
		public string Extension;
		///<summary>FK to email, as hosted by API.  Table does not exist at dental office.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntry;
		///<summary>DateTime the entry was edited.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		public EmailSecureAttach() {
		}
	}
}
