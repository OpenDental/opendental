using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>A set of actions to take in sequence for each interaction with a patient.  Individual actions are in eRoutingActionDef.  Changing these does not alter any patient records. Only used in eClipboard for now.</summary>
	public class ERoutingDef:TableBase {
		[CrudColumn(IsPriKey=true)]
		public long ERoutingDefNum;
		/// <summary>FK to clinic.ClinicNum. Represents the clinic that the eRouting is tied to, if any. Can be 0.</summary>
		public long ClinicNum;
		/// <summary>The name of the eRouting.</summary>
		public string Description;
		/// <summary>FK to userod.UserNum. The user that created this eRouting. Cannot be edited by user.</summary>
		public long UserNumCreated;
		/// <summary>FK to userod.UserNum. The user that last edited this eRouting. Cannot be edited by user.</summary>
		public long UserNumModified;
		/// <summary>Date Time this eRouting was created. Cannot be edited by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntered;
		/// <summary>Date time this eRouting was last edited. Cannot be edited by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateLastModified;

		public ERoutingDef Copy() {
			return (ERoutingDef)this.MemberwiseClone();
		}
	}

}
