using System;

namespace OpenDentBusiness{
	///<summary>A better name would be FieldHide. This specifies places where PatFields or ApptFields should be hidden. PatFieldDefs already have an IsHidden field, so this is redundant there. But it's powerful for letting PatFields show in some places but not other places.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class FieldDefLink:TableBase{
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long FieldDefLinkNum;
		///<summary>A generic FieldDefNum FK to any particular field def item that will be defined by the FieldDefType column.</summary>
		public long FieldDefNum;
		///<summary>Enum:FieldDefTypes Defines what FieldDefNum represents.</summary>
		public FieldDefTypes FieldDefType;
		///<summary>Enum:FieldLocations Defines where this particular field def needs to be hidden.</summary>
		public FieldLocations FieldLocation;

		///<summary></summary>
		public FieldDefLink Clone() {
			return (FieldDefLink)this.MemberwiseClone();
		}
	}

	///<summary>Enum representing different types of field defs.</summary>
	public enum FieldDefTypes {
		///<summary>0</summary>
		Appointment,
		///<summary>1</summary>
		Patient
	}

	///<summary>Enum representing where the field def should be hidden.</summary>
	public enum FieldLocations {
		///<summary>0</summary>
		Account,
		///<summary>1</summary>
		AppointmentEdit,
		///<summary>2</summary>
		Chart,
		///<summary>3</summary>
		Family,
		///<summary>4</summary>
		OrthoChart,
		///<summary>5</summary>
		GroupNote
	}
	
}




