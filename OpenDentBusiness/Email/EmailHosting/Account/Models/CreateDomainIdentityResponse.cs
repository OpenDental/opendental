using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class CreateDomainIdentityResponse {

		///<summary>The primary key of the newly created email identity.</summary>
		public long EmailIdentityNum { get; set; }

		///<summary>The verification token for this domain that was just created.</summary>
		public string VerificationToken { get; set; }

	}
}
