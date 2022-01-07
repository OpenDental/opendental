using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class SendMassEmailResponse {

		///<summary>A dictionary of unique ids sent to the request to a unique identifier for this email from email hosting. These unique ids will be used later in the provider's webhook to return notifications about the status of this email.</summary>
		public Dictionary<string,long> DictionaryUniqueIDToHostingID { get; set; }

	}
}
