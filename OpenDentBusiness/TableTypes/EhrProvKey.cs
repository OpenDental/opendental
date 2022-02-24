using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Used to store and track Ehr Provider Keys.  There can be multiple EhrProvKeys per provider.</summary>
	[Serializable]
	public class EhrProvKey:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrProvKeyNum;
		///<summary>FK to patient.PatNum.  Only used by HQ for generating keys for customers.  Will always be 0 for non-HQ users.</summary>
		public long PatNum;
		///<summary>The provider LName.</summary>
		public string LName;
		///<summary>The provider FName.</summary>
		public string FName;
		///<summary>The key assigned to the provider</summary>
		public string ProvKey;
		///<summary>Usually 1.  Can be less, like .5 or .25 to indicate possible discount is justified.</summary>
		public float FullTimeEquiv;
		///<summary>Any notes that the tech wishes to include regarding this situation.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Notes;
		///<summary>Required when generating a new provider key.  It is used to determine annual EHR eligibility.  Format will always be YY.</summary>
		public int YearValue;

		///<summary></summary>
		public EhrProvKey Copy() {
			return (EhrProvKey)MemberwiseClone();
		}

	}

	

}
