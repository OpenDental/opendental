using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>A dental supply or office supply item.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class Supply : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SupplyNum;
		/// <summary>FK to supplier.SupplierNum</summary>
		public long SupplierNum;
		/// <summary>The catalog item number that the supplier uses to identify the supply.</summary>
		public string CatalogNumber;
		/// <summary>The description can be similar to the catalog, but not required.  Typically includes qty per box/case, etc.</summary>
		public string Descript;
		/// <summary>FK to definition.DefNum.  User can define their own categories for supplies.</summary>
		public long Category;
		///<summary>The zero-based order of this supply within it's category.  Hidden supplies can be included in this order.</summary>
		public int ItemOrder;
		/// <summary>Aka Stock Level.  The level that a fresh order should bring item back up to.  Can include fractions.  If this is 0, then it will be displayed as having this field blank rather than showing 0.  This simply gives a cleaner look.</summary>
		public float LevelDesired;
		/// <summary>If hidden, then this supply item won't normally show in the main list.</summary>
		public bool IsHidden;
		/// <summary>The price per unit that the supplier charges for this supply.  If this is 0.00, then no price will be displayed.</summary>
		public double Price;
		/// <summary>Scanned code from a reader.</summary>
		public string BarCodeOrID;
		/// <summary>Only used for dental schools.  This is the typical quantity dispensed at the window.</summary>
		public float DispDefaultQuant;
		/// <summary>Only used in dental schools.  For example, 20 capsules composite per container.</summary>
		public int DispUnitsCount;
		/// <summary>Only used in dental schools.  Description of the units when dispensing for use.  For example: Capsule, cartridge, carpule, glove, or needle.</summary>
		public string DispUnitDesc;
		///<summary>Deprecated.</summary>
		public float LevelOnHand;
		///<summary>The amount to order when the next SupplyOrder is created.  Creating a SupplyOrder then zeroes this out, so it's just a temporary value.</summary>
		public int OrderQty;

		///<summary></summary>
		public Supply Copy() {
			return (Supply)this.MemberwiseClone();
		}
			
	}

	

}









