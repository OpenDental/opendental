using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class UpdateAccountGuarantorStatusRequest {

		///<summary>The external ID of this Account Guarantor.</summary>
		public string ExternalID { get; set; }

		///<summary>Indicates if the accounts associated to this account guarantor account should be updated to enabled or disabled.</summary>
		public bool AreAccountsEnabled { get; set; }
		///<summary>The type of email that is being signed up for. 0=MassEmail, 1=SecureEmail</summary>
		public int SignupType { get; set; }

	}
}
