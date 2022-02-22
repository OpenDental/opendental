using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Always attached to covcats, this describes the span of procedure codes to which the category applies.</summary>
	[Serializable]
	public class CovSpan:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CovSpanNum;
		///<summary>FK to covcat.CovCatNum.</summary>
		public long CovCatNum;
		///<summary>Lower range of the span.  Does not need to be a valid code.</summary>
		public string FromCode;
		///<summary>Upper range of the span.  Does not need to be a valid code.</summary>
		public string ToCode;

		///<summary></summary>
		public CovSpan Copy() {
			CovSpan c=new CovSpan();
			c.CovSpanNum=CovSpanNum;
			c.CovCatNum=CovCatNum;
			c.FromCode=FromCode;
			c.ToCode=ToCode;
			return c;
		}
	}

	


}









