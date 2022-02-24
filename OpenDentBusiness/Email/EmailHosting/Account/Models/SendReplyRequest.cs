using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class SendReplyRequest {

		///<summary>The primary key of the email chain that is being replied to.</summary>
		public long EmailChainNum { get; set; }

		///<summary>Send notifications from the ChainOwner EmailAddress instead of default EmailAddress.</summary>
		public bool DoSendNotificationsAsOwner { get; set; }

		///<summary>The contents of the reply. The From/To address should match the To/From address on the email that they are replying to.</summary>
		public EmailResource EmailToSend { get; set; }

	}
}
