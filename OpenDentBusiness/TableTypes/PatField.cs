using System;
using System.Collections;

namespace OpenDentBusiness{

	/// <summary>These are custom fields added and managed by the user.</summary>
	[Serializable]
	[CrudTable(IsSecurityStamped=true)]
	public class PatField:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatFieldNum;
		///<summary>FK to patient.PatNum</summary>
		public long PatNum;
		///<summary>FK to patfielddef.FieldName.  The full name is shown here for ease of use when running queries.  But the user is only allowed to change fieldNames in the patFieldDef setup window.</summary>
		public string FieldName;
		///<summary>Any text that the user types in.  For picklists, this will contain the picked text.  For dates, this is stored as the user typed it, after validating that it could be parsed.  So queries that involve dates won't work very well.  If we want better handling of date fields, we should add a column to this table.  Checkbox will either have a value of 1, or else the row will be deleted from the db.  Currency is handled in a culture neutral way, just like other currency in the db.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob|CrudSpecialColType.CleanText)]
		public string FieldValue;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;


		///<summary></summary>
		public PatField Copy() {
			return (PatField)this.MemberwiseClone();
		}

	}

		



		
	

	

	


}










