using System;

namespace OpenDentBusiness {
	///<summary>This table is similar to the GroupPermission table.  However, unlike the GroupPermissions table, existence of a row for a patient in
	///this table means the action is NOT allowed.  Hence the name PatRestrictions.</summary>
	[Serializable]
	public class PatRestriction:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatRestrictionNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Enum:PatRestrict </summary>
		public PatRestrict PatRestrictType;

		///<summary></summary>
		public PatRestriction Copy() {
			return (PatRestriction)this.MemberwiseClone();
		}

	}

	/// <summary></summary>
	public enum PatRestrict {
		///<summary>0</summary>
		None,
		///<summary>1 - Patient cannot be scheduled nor have schedule edited. This PatRestrict should probably be checked every place the group 
		///permissions AppointmentCreate, AppointmentMove, and AppointmentEdit are checked.</summary>
		ApptSchedule,
		/////<summary>2 - Possibly handle this separate from ApptSchedule or perhaps block web scheduling if ApptSchedule is blocked, but allow regular
		/////scheduling if only WebSchedule is blocked.</summary>
		//WebSchedule
	}
}