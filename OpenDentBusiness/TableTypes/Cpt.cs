using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Other tables generally use the CptCode as their foreign key.</summary>
	[Serializable]
	public class Cpt:TableBase {
		///<summary>Primary key. .</summary>
		[CrudColumn(IsPriKey=true)]
		public long CptNum;
		///<summary>Cpt code. Not allowed to edit this column once saved in the database.</summary>
		public string CptCode;
		///<summary>Short Description provided by Cpt documentation.</summary>
		public string Description;
		///<summary>Comma delimited list of years the Cpt code existed in that have been imported into this table.</summary>
		public string VersionIDs;


		///<summary></summary>
		public Cpt Copy() {
			return (Cpt)this.MemberwiseClone();
		}

	}
}