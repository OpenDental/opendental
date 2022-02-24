using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class SendNewEmailRequest {

		///<summary>The email to send.</summary>
		public EmailResource EmailToSend { get; set; }

		///<summary>Send notifications from the ChainOwner EmailAddress instead of default EmailAddress.</summary>
		public bool DoSendNotificationsAsOwner { get; set; }

		///<summary>The email addresses to include in this email chain.</summary>
		public List<EmailAddressResource> ListEmailAddresses { get; set; }

	}
}
