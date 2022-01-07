using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class GetAllTemplatesByGuarantorResponse {

		///<summary>A dictionary of dictionaries of templates. The first key is the ExternalID of the account. The value is a dictionary of primary keys to templates that belong to this account.</summary>
		public Dictionary<string,Dictionary<long,Template>> DictionaryTemplates { get; set; }

	}
}
