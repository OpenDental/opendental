using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>One row on an evaluation.</summary>
	[Serializable]
	public class EvaluationCriterion:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EvaluationCriterionNum;
		///<summary>FK to evaluation.EvaluationNum</summary>
		public long EvaluationNum;
		///<summary>Description that is displayed for the criterion.</summary>
		public string CriterionDescript;
		///<summary>This row will show in bold and will not have a grade attached to it.</summary>
		public bool IsCategoryName;
		///<summary>FK to gradingscale.GradingScaleNum.  The grading scale used for this criterion.  Having this here allows the instructor to edit saved grades and also allows the evaluation overall grade to consider whether to include this criterion in the calculation.</summary>
		public long GradingScaleNum;
		///<summary>Copied from gradingscaleitem.GradeShowing.  Required.  For example A, B, C, D, F, or 1-10, pass, fail, 89, etc.  Except for percentages, must come from pick list.</summary>
		public string GradeShowing;
		///<summary>Copied from gradingscaleitem.GradeNumber.  Required.  For example A=4, A-=3.8, pass=1, percentages stored as 89, etc.  Except for percentages, must come from pick list.</summary>
		public float GradeNumber;
		///<summary>A note about why this student received this particular grade on this criterion.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Notes;
		///<summary>Copied from item order of def.  Defines the order that all the criteria show on the evaluation.  User not allowed to change here, only in the def.</summary>
		public int ItemOrder;
		///<summary>For ScaleType=Points, sets the maximum value of points for this criterion.</summary>
		public float MaxPointsPoss;

		///<summary></summary>
		public EvaluationCriterion Copy() {
			return (EvaluationCriterion)this.MemberwiseClone();
		}

	}

	
}




