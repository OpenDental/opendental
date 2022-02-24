using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class GetChainEmailsResponse {

		///<summary>The email address that owns this Email Chain.</summary>
		public EmailAddressResource ChainOwnerEmailAddress { get; set; }

		///<summary>A list of all email addresses included on the chain.</summary>
		public List<EmailAddressResource> ListEmailAddresses { get; set; }

		///<summary>A list of emails within the chain.</summary>
		public List<EmailResource> ListEmails { get; set; }

	}
}
