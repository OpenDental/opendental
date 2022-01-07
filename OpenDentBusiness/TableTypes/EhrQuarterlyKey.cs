using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Also used by OD customer support to store and track Ehr Quarterly Keys for customers.</summary>
	[Serializable]
	public class EhrQuarterlyKey:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrQuarterlyKeyNum;
		///<summary>Example 11</summary>
		public int YearValue;
		///<summary>Example 2</summary>
		public int QuarterValue;
		///<summary>The customer must have this exact practice name entered in practice setup.</summary>
		public string PracticeName;
		///<summary>The calculated key value, tied to year, quarter, and practice name.</summary>
		public string KeyValue;
		///<summary>FK to patient.PatNum.  Always zero for customer databases.  When used by OD customer support, this is the customer num.</summary>
		public long PatNum;
		///<summary>Any notes that the tech wishes to include regarding this situation.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Notes;

		///<summary></summary>
		public EhrQuarterlyKey Copy() {
			return (EhrQuarterlyKey)MemberwiseClone();
		}

	}

	

}
