using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Represents one bracket, wire, or elastic.</summary>
	[Serializable]
	public class OrthoHardware:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoHardwareNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Every hardware entry is tied to a single date. At each exam, a copy can be made of the hardware from the previous exam, and then it can be edited. It normally shows the most recent exam, and the hardware items showing in the ortho grid only include the most recent exam. Not sure yet how we will show hardware for previous exams/dates.</summary>
		public DateTime DateExam;
		///<summary>Enum:EnumOrthoHardwareType Bracket, Wire, or Elastic.</summary>
		public EnumOrthoHardwareType OrthoHardwareType;
		///<summary>FK to orthohardwarespec.OrthoHardwareSpecNum. This is where the description and color come from.</summary>
		public long OrthoHardwareSpecNum;
		///<summary>Tooth numbers stored here are always stored in Universal (1-32) notation. They are displayed to the user as Palmer notation. For brackets, always use single tooth numbers, like 8. For wires, must use a range like 2-15. For elastics, typically use 2 teeth separated with commas, but more are allowed.</summary>
		public string ToothRange;
		///<summary></summary>
		public string Note;

		///<summary></summary>
		public OrthoHardware Clone() {
			return (OrthoHardware)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum EnumOrthoHardwareType	{
		///<summary>0</summary>
		Bracket,
		///<summary>1</summary>
		Wire,
		///<summary>2</summary>
		Elastic
	}
}

