using System;

namespace OpenDentBusiness{
	///<summary>A linker table that holds generic "field def" links (e.g. patient fields, appointment fields, etc).
	///Each row will have a corresponding FieldLocation, where this def type needs to be hidden from the user (All field defs are shown by default).
	///The presence of an entry in this table will cause field defs of that particular field type not to show up in the specified location.</summary>
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




