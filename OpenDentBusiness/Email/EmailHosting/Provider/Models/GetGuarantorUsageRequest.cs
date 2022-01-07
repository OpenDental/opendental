using System;
using System.Collections.Generic;
using System.IO;

//This file is auto-generated. Do not change.
namespace OpenDentBusiness {
	///<summary></summary>
	public class GetGuarantorUsageRequest {

		///<summary>The lower bound of the usages returned. Only considers the date. Inclusive.</summary>
		public DateTime TimeLowerBound { get; set; }

		///<summary>The upper bound of the usages returned. Only considers the date. Inclusive.</summary>
		public DateTime TimeUpperBound { get; set; }

		///<summary>A list of external IDs to get usages for.</summary>
		public List<string> ListExternalIDs { get; set; }

	}
}
