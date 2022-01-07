using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class UpdateAccountStatusRequest {

		///<summary>The external ID for the account guarantor.</summary>
		public string GuarantorExternalID { get; set; }

		///<summary>The external ID for the account.</summary>
		public string ExternalID { get; set; }

		///<summary>Indicates if the account is enabled or not.</summary>
		public bool IsAccountEnabled { get; set; }

		///<summary>The type of email that is being enabled / disabled.</summary>
		public int ServiceType { get; set; }

	}
}
