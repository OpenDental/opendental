using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Only used when GradingScale.ScaleType=PickList, not Percentage or Points.  The specific grades allowed on a scale.  Contains both the GradeShowing and the equivalent number.  There are no FKs to these items.  The values are all copied from here into student records as they are used.</summary>
	[Serializable]
	public class GradingScaleItem:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long GradingScaleItemNum;
		///<summary>FK to gradingscale.GradingScaleNum</summary>
		public long GradingScaleNum;
		///<summary>For example A, B, C, D, F.  Optional.  If not specified, it shows the number.</summary>
		public string GradeShowing;
		///<summary>For example A=4, A-=3.8, pass=1, etc.  Required.  Enforced to be equal to or less than GradingScale.MaxPointsPoss.</summary>
		public float GradeNumber;
		///<summary>Optional additional info about what this particular grade means.  Just used as guidance and does not get copied to the individual student record.</summary>
		public string Description;

		///<summary></summary>
		public GradingScaleItem Copy() {
			return (GradingScaleItem)this.MemberwiseClone();
		}

	}

	
}




