using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Used to keep track of Customer's API Key and Developer's name. Just a copy from OD HQ for convenience.</summary>
	[Serializable]
	public class APIKey:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn (IsPriKey=true)]
		public long APIKeyNum;
		///<summary>Customer's API key.</summary>
		public string CustApiKey;
		///<summary>Developer's name, exactly as they entered it in FHIR developer portal.</summary>
		public string DevName;
	}

}
