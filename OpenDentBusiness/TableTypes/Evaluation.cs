using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>An evaluation is for one student and is copied from an EvaluationDef.</summary>
	[Serializable]
	public class Evaluation:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EvaluationNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long InstructNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long StudentNum;
		///<summary>FK to schoolcourse.SchoolCourseNum.  For example to PEDO 732.</summary>
		public long SchoolCourseNum;
		///<summary>Copied from evaluation def.  Not editable.</summary>
		public string EvalTitle;
		///<summary>Date of the evaluation.</summary>
		public DateTime DateEval;
		///<summary>FK to gradingscale.GradingScaleNum.  The overall grading scale for this evaluation.  Copied from EvaluationDef.  Criteria will not all necessarily have the same scale.</summary>
		public long GradingScaleNum;
		///<summary>OverallGradeNumber is calculated as described below.  Once the nearest number on the scale is found, the corresponding gradescaleitem.GradeShowing is used here.</summary>
		public string OverallGradeShowing;
		///<summary>Always recalculated as each individual criterion is changed, so no risk of getting out of synch.  Only considers criteria on the evaluation that use the same grading scale as the evaluation itself.  It's an average of all those criteria.  When averaging, the result will almost never exactly equal one of the numbers in the scale, so the nearest one must be found and used here.  For example, if the average is 3.6 on a 4 point scale, this will show 4.  Percentages will be rounded to the nearest whole number.  This is the value that will be returned in reports and also used in calculations of the student's grade for the term.</summary>
		public float OverallGradeNumber;
		///<summary>Any note that the instructor wishes to place at the bottom of this evaluation.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Notes;

		///<summary></summary>
		public Evaluation Copy() {
			return (Evaluation)this.MemberwiseClone();
		}

	}

	
}




