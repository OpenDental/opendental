using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>A supply freeform typed in by a user.</summary>
	[Serializable()]
	public class SupplyNeeded : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SupplyNeededNum;
		/// <summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Description;
		/// <summary>.</summary>
		public DateTime DateAdded;

		

			
	}

	

}









