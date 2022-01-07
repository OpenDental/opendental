using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {

	/// <summary>A list of diseases that can be assigned to patients.</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class DiseaseDefm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long DiseaseDefNum;
		///<summary>.</summary>
		public string DiseaseName;

		///<summary></summary>
		public DiseaseDefm Copy() {
			return (DiseaseDefm)this.MemberwiseClone();
		}
	}
}
