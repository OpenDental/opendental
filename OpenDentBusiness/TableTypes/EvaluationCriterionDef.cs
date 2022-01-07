using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Rows on an evaluation def.  The individual items that will be graded. Criterion Defs</summary>
	[Serializable]
	public class EvaluationCriterionDef:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EvaluationCriterionDefNum;
		///<summary>FK to evaluationdef.EvaluationDefNum.</summary>
		public long EvaluationDefNum;
		///<summary>Description that is displayed for the criterion.</summary>
		public string CriterionDescript;
		///<summary>This row will show in bold and will not have a grade attached to it.</summary>
		public bool IsCategoryName;
		///<summary>FK to gradingscale.GradingScaleNum.  The grading scale used for this criterion.  As a user builds an evaluationDef, each criterion should default to the GradingScaleNum of the EvaluationDef, and then the user can change if needed.  The individual criteria do not have to be the same scale as the evaluation.</summary>
		public long GradingScaleNum;
		///<summary>Defines the order that all the criteria show on the evaluation.  Copied to ItemOrder of actual criterion.</summary>
		public int ItemOrder;
		///<summary>For ScaleType=Points, sets the maximum value of points for this criterion.</summary>
		public float MaxPointsPoss;

		///<summary></summary>
		public EvaluationCriterionDef Copy() {
			return (EvaluationCriterionDef)this.MemberwiseClone();
		}

	}

	
}




