using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Keeps track of one file attached to an email.  Multiple files can be attached to an email using this method.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true,HasBatchWriteMethods=true)]
	public class EmailAttach:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailAttachNum;
		///<summary>FK to emailmessage.EmailMessageNum.  0 if EmailTemplateNum is set, otherwise must have a value.</summary>
		public long EmailMessageNum;
		///<summary>The name of the file that shows on the email.  For example: tooth2.jpg.</summary>
		public string DisplayedFileName;
		///<summary>The actual file is stored in the A-Z folder in EmailAttachments.  This field stores the sub directories and name of the file.  The files are named automatically based on Date/time along with a random number.  This ensures that they will be sequential as well as unique.</summary>
		public string ActualFileName;
		///<summary>FK to emailtemplate.EmailTemplateNum.  0 if EmailMessageNum is set, otherwise must have a value.</summary>
		public long EmailTemplateNum;

		public EmailAttach Copy() {
			return (EmailAttach)MemberwiseClone();
		}
	}




}
