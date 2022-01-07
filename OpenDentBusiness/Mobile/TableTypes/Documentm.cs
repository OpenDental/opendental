using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {

		///<summary>Links allergies to patients. Patient portal version</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class Documentm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long DocNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>The raw file data encoded as base64.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string RawBase64;
		///<summary></summary>
		public Documentm Copy() {
			return (Documentm)this.MemberwiseClone();
		}

	}
}

