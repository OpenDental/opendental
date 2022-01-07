using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Discount plans will automatically create adjustments when procedures are completed.
	///The fee schedule associated to the discount plan will be used with the UCR fee schedule in order to determine the "discount".
	///The associated DefNum will be the adjustment type that is used so that users can quickly query adjustments to see discount plan usage.</summary>
	[Serializable()]
	public class DiscountPlan:TableBase{
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long DiscountPlanNum;
		///<summary>Description of this discount plan</summary>
		public string Description;
		///<summary>FK to feesched.FeeSchedNum</summary>
		public long FeeSchedNum;
		///<summary>FK to definition.DefNum.  Represents the adjustment type of the feesched plan.</summary>
		public long DefNum;
		///<summary>Set true to hide in Discount Plan list.</summary>
		public bool IsHidden;
		///<summary>Note for this plan.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PlanNote;
		///<summary>Number of Procedures allowed for a discount plans Exam category.</summary>
		public int ExamFreqLimit=-1;
		///<summary>Number of Procedures allowed for a discount plans X-Ray category.</summary>
		public int XrayFreqLimit=-1;
		///<summary>Number of Procedures allowed for a discount plans Prophylaxis category.</summary>
		public int ProphyFreqLimit=-1;
		///<summary>Number of Procedures allowed for a discount plans Fluoride category.</summary>
		public int FluorideFreqLimit=-1;
		///<summary>Number of Procedures allowed for a discount plans Periodontal category.</summary>
		public int PerioFreqLimit=-1;
		///<summary>Number of Procedures allowed for a discount plans Limited Exam category.</summary>
		public int LimitedExamFreqLimit=-1;
		///<summary>Number of Procedures allowed for a discount plans Periapical X-Ray category.</summary>
		public int PAFreqLimit=-1;
		///<summary>Annual discount maximum for frequency limitations. -1 indicates blank or no annual max limitation.</summary>
		public double AnnualMax=-1;

		///<summary></summary>
		public DiscountPlan Copy() {
			return (DiscountPlan)this.MemberwiseClone();
		}

	}
}




