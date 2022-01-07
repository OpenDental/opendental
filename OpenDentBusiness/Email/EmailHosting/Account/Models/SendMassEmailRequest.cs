using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class SendMassEmailRequest {

		///<summary>The primary key of the template that is being used for sending.</summary>
		public long TemplateNum { get; set; }

		///<summary>The "from" email address these requests will be sent from. If is null or blank, will be sent from our global no-reply email address.</summary>
		public string FromEmailAddress { get; set; }

		///<summary>The name of the sender that will appear on the emails. Optional.</summary>
		public string SenderName { get; set; }

		///<summary>The address that will be set in the Reply To header. If null or blank, will default to the FromEmailAddress.</summary>
		public string ReplyToAddress { get; set; }

		///<summary>A list of template destinations for this mass email request. These include address and replacement tags.</summary>
		public List<TemplateDestination> ListDestinations { get; set; }

		///<summary></summary>
		public ExternalTag ExternalTag { get; set; }

	}
}
