using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class UpdateAccountGuarantorStatusRequest {

		///<summary>The external ID of this Account Guarantor.</summary>
		public string ExternalID { get; set; }

		///<summary>Flag that indicates the new status of the accounts linked to this guarantor.</summary>
		public bool AreAccountsEnabled { get; set; }

		///<summary>The type of email that is being signed up for.</summary>
		public int SignupType { get; set; }

	}
}
