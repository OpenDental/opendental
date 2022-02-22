using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class CreateAccountResponse {

		///<summary>A unique identifier for the account. This will be used to validate an account when making api calls.</summary>
		public string AccountGuid { get; set; }

		///<summary>A secret used to verify an account when making api calls. This is the only place this is returned.</summary>
		public string AccountSecret { get; set; }

	}
}
