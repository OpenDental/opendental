using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {

	///<summary>There is either one or zero per claim.</summary>
	[Serializable()]
	public class ClaimCondCodeLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClaimCondCodeLogNum;
		///<summary>FK to claim.ClaimNum.</summary>
		public long ClaimNum;
		///<summary>Corresponds with condition code 18 on the UB04.</summary>
		public string Code0;
		///<summary>Corresponds with condition code 19 on the UB04.</summary>
		public string Code1;
		///<summary>Corresponds with condition code 20 on the UB04.</summary>
		public string Code2;
		///<summary>Corresponds with condition code 21 on the UB04.</summary>
		public string Code3;
		///<summary>Corresponds with condition code 22 on the UB04.</summary>
		public string Code4;
		///<summary>Corresponds with condition code 23 on the UB04.</summary>
		public string Code5;
		///<summary>Corresponds with condition code 24 on the UB04.</summary>
		public string Code6;
		///<summary>Corresponds with condition code 25 on the UB04.</summary>
		public string Code7;
		///<summary>Corresponds with condition code 26 on the UB04.</summary>
		public string Code8;
		///<summary>Corresponds with condition code 27 on the UB04.</summary>
		public string Code9;
		///<summary>Corresponds with condition code 28 on the UB04.</summary>
		public string Code10;
	}
}
