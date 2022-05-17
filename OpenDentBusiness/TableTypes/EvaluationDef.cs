using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>An evaluation def is the entire form that the instructor sets up ahead of time.  Actual evaluations for students are copied from these 'templates', so an evaluation def can be altered or deleted without damaging any student record.  Evaluation defs are usually not specific to instructors, but if different instructors want different evaluation forms, they can use the description column to differentiate.  For example, the description can include the instructor's name or even the year.  But most commonly, the same evaluation will be used from year to year.  There should be a duplicate function to make a copy an entire evaluation def and then allow user to alter the SchoolCourseNum.</summary>
	[Serializable]
	public class EvaluationDef:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EvaluationDefNum;
		///<summary>FK to schoolcourse.SchoolCourseNum.  For example to PEDO 732.</summary>
		public long SchoolCourseNum;
		///<summary>Description of this evaluation form.</summary>
		public string EvalTitle;
		///<summary>FK to gradingscale.GradingScaleNum.  The default grading scale for this evaluation.  Each criterion will typically use the same scale, but that is not required.</summary>
		public long GradingScaleNum;

		///<summary></summary>
		public EvaluationDef Copy() {
			return (EvaluationDef)this.MemberwiseClone();
		}

	}

	
}




