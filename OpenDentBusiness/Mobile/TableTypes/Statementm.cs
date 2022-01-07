using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Mobile {

		///<summary>Links allergies to patients. Patient portal version</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class Statementm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long StatementNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		/// <summary>This will always be a valid and reasonable date regardless of whether it's actually been sent yet.</summary>
		public DateTime DateSent;
		/// <summary>FK to document.DocNum when a pdf has been archived.</summary>
		public long DocNum;

		///<summary></summary>
		public Statementm Copy() {
			return (Statementm)this.MemberwiseClone();
		}

	}
}

