using System;
using System.Collections;

namespace OpenDentBusiness{

	/// <summary>These are custom fields added to appointments and managed by the user.</summary>
	[Serializable]
	public class ApptField:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApptFieldNum;
		///<summary>FK to appointment.AptNum</summary>
		public long AptNum;
		///<summary>FK to apptfielddef.FieldName.  The full name is shown here for ease of use when running queries.  But the user is only allowed to change fieldNames in the patFieldDef setup window.</summary>
		public string FieldName;
		///<summary>Any text that the user types in.  Will later allow some automation.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string FieldValue;

		///<summary></summary>
		public ApptField Copy() {
			return (ApptField)this.MemberwiseClone();
		}

	}

		



		
	

	

	


}










