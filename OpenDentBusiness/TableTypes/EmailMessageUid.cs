using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Used to track which email messages have been downloaded into the inbox for a particular recipient address.
	///Not linked to the email message itself because no link is needed.
	///If we decide to add a foreign key to a EmailMessage later, we should consider what do to when an email message is deleted (set the foreign key to 0 perhaps).</summary>
	[Serializable]
	public class EmailMessageUid:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmailMessageUidNum;
		///<summary>The unique id for the associated EmailMessage.  </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MsgId;
		///<summary>Copied from the EmailAddress.EmailUsername field when a message is received into the inbox.
		///Similar to the ToAddress of the EmailMessage, except the ToAddress could contain multiple recipient addresses
		///or group email address instead. The recipient address helps match the EmailMessageUid to a particular EmailAddress.</summary>
		public string RecipientAddress;

		public EmailMessageUid Clone() {
			return (EmailMessageUid)this.MemberwiseClone();
		}

	}
}
