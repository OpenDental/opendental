using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary>Represents a secure email stored or in transit to/from at the Email Hosting endpoint.</summary>
	public class EmailResource {

		///<summary>The address this email was sent from.</summary>
		public EmailAddressResource FromAddress { get; set; }

		///<summary>DateTime of the email.  Not used when creating a new email.</summary>
		public DateTime DateTimeEmail { get; set; }

		///<summary></summary>
		public ExternalTag ExternalTag { get; set; }

		///<summary>The subject of this email.</summary>
		public string Subject { get; set; }

		///<summary>The body of this email.</summary>
		public string BodyHtml { get; set; }

		///<summary>List of S3Object Guids that represent the email attachments.</summary>
		public List<AttachmentResource> ListAttachments { get; set; }

	}
}
