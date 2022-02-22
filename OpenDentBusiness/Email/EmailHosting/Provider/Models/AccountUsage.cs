using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class AccountUsage {

		///<summary>The external ID of the account.</summary>
		public string ExternalID { get; set; }

		///<summary>The total number of emails that have been sent for the date for mass emails. This includes bounced, flagged for spam, delivered, etc.</summary>
		public int MassEmailsSent { get; set; }

		///<summary>The total number of emails that have bounced for the date for mass emails.</summary>
		public int MassEmailsBounced { get; set; }

		///<summary>The total number of secure emails sent.</summary>
		public int SecureEmailsSent { get; set; }

		///<summary>The total number of unsecure email sent for secure emails.</summary>
		public int SecureUnsecureEmailsSent { get; set; }

		///<summary>The total number of secure emails that have bounced.</summary>
		public int SecureEmailsBounced { get; set; }

	}
}
