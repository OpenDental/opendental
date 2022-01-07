using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary>Represents a domain that can be sent from.</summary>
	public class IdentityResource {

		///<summary>The string representation of the domain that can be sent from.</summary>
		public string Identity { get; set; }

		///<summary>The verification status of the domain. This will be verified when the VerificationToken is put in the users DNS record.</summary>
		public IdentityVerificationStatus VerificationStatus { get; set; }

		///<summary>The verification status of the DKIM aspect .</summary>
		public IdentityVerificationStatus DKIMVerificationStatus { get; set; }

		///<summary>Indicates if DKIM is enabled for this domain.</summary>
		public bool DKIMEnabled { get; set; }

		///<summary>The verification token for this domain. This is the token that should be a TXT entry in the domain's DNS.</summary>
		public string VerificationToken { get; set; }

	}
}
