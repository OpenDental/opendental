using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>A code system used in EHR.  Healhtcare Common Procedure Coding System.  Another system used to describe procedure codes.</summary>
	[Serializable()]
	public class Hcpcs:TableBase{
		///<summary>Primary key..</summary>
		[CrudColumn(IsPriKey=true)]
		public long HcpcsNum;
		///<summary>Examples: AQ, J1040</summary>
		public string HcpcsCode;
		///<summary>Short description.  This is the HCPCS supplied abbreviated description.</summary>
		public string DescriptionShort;

		///<summary></summary>
		public Hcpcs Clone() {
			return (Hcpcs)this.MemberwiseClone();
		}

	}

	
}




