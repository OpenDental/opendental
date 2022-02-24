using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>A dental supply or office supply item that has been dispensed.</summary>
	[Serializable]
	public class DispSupply : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DispSupplyNum;
		/// <summary>FK to supply.SupplyNum</summary>
		public long SupplyNum;
		/// <summary>FK to provider.ProvNum</summary>
		public long ProvNum;
		/// <summary></summary>
		public DateTime DateDispensed;
		/// <summary>Quantity given out.</summary>
		public float DispQuantity;
		///<summary>Notes on the dispensed supply.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob | CrudSpecialColType.CleanText)]
		public string Note;
	}

	

}









