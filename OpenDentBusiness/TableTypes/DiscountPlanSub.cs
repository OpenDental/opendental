using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Table used to determine discount plan subscribers, as well as the effective date range of a discount plan.</summary>
	[Serializable]
	public class DiscountPlanSub:TableBase {
		///<summary>PK</summary>
		[CrudColumn(IsPriKey=true)]
		public long DiscountSubNum;
		///<summary>FK to discountplan.DiscountPlanNum, represents which plan the patient is subscribed to.</summary>
		public long DiscountPlanNum;
		///<summary>FK to patient.PatNum which represents the subscriber</summary>
		public long PatNum;
		///<summary>When the discount plan should start to impact procedure fees.</summary>
		public DateTime DateEffective;
		///<summary>When the discount plan should no longer impact procedure fees.</summary>
		public DateTime DateTerm;
		///<summary>Note for this sub.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string SubNote;

		///<summary>Returns a copy of this DiscountPlanSub.</summary>
		public DiscountPlanSub Copy() {
			return (DiscountPlanSub)this.MemberwiseClone();
		}

		///<summary>Returns true if the given date is within the effective and term dates.</summary>
		public bool IsValidForDate(DateTime date) {
			if((date>=DateEffective) && (DateTerm.Year<1880 || date<=DateTerm)) {
				return true;
			}
			return false;
		}
	}
}