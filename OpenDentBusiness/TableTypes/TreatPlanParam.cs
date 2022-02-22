using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Stores check box information for each treatment plan, so that when a signed treatment plan PDF needs to be saved from
	///eClipboard, it can correctly save and generate the PDF.</summary>
	[Serializable]
	public class TreatPlanParam:TableBase {
		[CrudColumn(IsPriKey=true)]
		public long TreatPlanParamNum;
		///<summary>FK to Patient.</summary>
		public long PatNum;
		///<summary>FK to TreatPlan.</summary>
		public long TreatPlanNum;
		///<summary>Value is set by the Discount check box in the Tx Module.</summary>
		public bool ShowDiscount;
		///<summary>Value is set by the Use Ins Max and Deduct check box in the Tx Module.</summary>
		public bool ShowMaxDed;
		///<summary>Value is set by the Subtotals check box in the Tx Module.</summary>
		public bool ShowSubTotals;
		///<summary>Value is set by the Totals check box in the Tx Module.</summary>
		public bool ShowTotals;
		///<summary>Value is set by the Graphical Completed Tx check box in the Tx Module.</summary>
		public bool ShowCompleted;
		///<summary>Value is set by the Fees check box in the Tx Module.</summary>
		public bool ShowFees;
		///<summary>Value is set by the Insurances Estimates check box in the Tx Module.</summary>
		public bool ShowIns;
	}
}
