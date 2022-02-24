using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Like a rolodex for businesses that the office interacts with.  Used to store pharmacies, etc.</summary>
	[Serializable]
	public class Contact:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ContactNum;
		///<summary>Last name or, frequently, the entire name.</summary>
		public string LName;
		///<summary>First name is optional.</summary>
		public string FName;
		///<summary>Work phone.</summary>
		public string WkPhone;
		///<summary>Fax number.</summary>
		public string Fax;
		///<summary>FK to definition.DefNum</summary>
		public long Category;
		///<summary>Note for this contact.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Notes;
	}

	
}