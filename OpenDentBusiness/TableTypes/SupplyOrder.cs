using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>One supply order to one supplier.  Contains SupplyOrderItems.</summary>
	[Serializable()]
	public class SupplyOrder : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SupplyOrderNum;
		/// <summary>FK to supplier.SupplierNum.</summary>
		public long SupplierNum;
		/// <summary>A date greater than 2200 (eg 2500), is considered a max date.  A max date is used for an order that was started but has not yet been placed.  This puts it at the end of the list where it belongs, but it will display as blank.  Only one unplaced order is allowed per supplier.</summary>
		public DateTime DatePlaced;
		/// <summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		/// <summary>The sum of all the amounts of each item on the order.  If any of the item prices are zero, then it won't auto calculate this total.  This will allow the user to manually put in the total without having it get deleted.</summary>
		public double AmountTotal;
		/// <summary>FK to userod.UserNum. User that placed the order, is editable.</summary>
		public long UserNum;
		/// <summary>The order's shipping charge.</summary>
		public double ShippingCharge;
		/// <summary>The date the order was received. If the SupplyOrder existed before updating to version 19.4, then this value will be set to the date
		/// the office updated to 19.4.</summary>
		public DateTime DateReceived;

		public SupplyOrder Copy(){
			return (SupplyOrder)this.MemberwiseClone();
		}
		
		

			
	}

	

}









