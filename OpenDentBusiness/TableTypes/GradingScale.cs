using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Used in Evaluations.  Describes a scale to be used in grading.  Freeform scales are not allowed.  Percentage scales are handled a little differently than the other scales.</summary>
	[Serializable]
	public class GradingScale:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long GradingScaleNum;
		///<summary>Enum:EnumScaleType Used to determine method of assigning grades.  PickList will be the only type that has GradingScaleItems.</summary>
		public EnumScaleType ScaleType;
		///<summary>For example, A-F or Pass/Fail.</summary>
		public string Description;

		///<summary></summary>
		public GradingScale Copy() {
			return (GradingScale)this.MemberwiseClone();
		}

		

	}

	///<summary>Used in GradingScale to determine how grades are assigned.</summary>
		public enum EnumScaleType {
			///<summary>0- User-Defined list of possible grades.  Grade is calculated as an average.</summary>
			PickList,
			///<summary>1- Percentage Scale 0-100.  Grade is calculated as an average.</summary>
			Percentage,
			///<summary>2- Allows point values for grades.  Grade is calculated as a sum of all points out of points possible.</summary>
			Weighted
		}
}




