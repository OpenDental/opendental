using System;

namespace OpenDentBusiness {

	/// <summary>Each row is an item in a PatFieldDef picklist. Not used unless the PatFieldDef is a Picklist type. These objects are created and managed by user.</summary>
	[Serializable]
	public class PatFieldPickItem:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PatFieldPickItemNum;
		///<summary>FK to patfielddef.PatFieldDefNum</summary>
		public long PatFieldDefNum;
		/// <summary>Full text of PickList item.</summary>
		public string Name;
		/// <summary>Abbr to show when PickList item is displayed in cramped spaces like columns. Only implemented in Superfamily grid so far.</summary>
		public string Abbreviation;
		/// <summary>False for normal PickList items. Even if true/hidden, this item will still show in all the various windows where patient fields show. A hidden item will not normally show when picking from list for a patient unless the patient has already been assigned this item.</summary>
		public bool IsHidden;
		///<summary>0-based.</summary>
		public int ItemOrder;

		///<summary></summary>
		public PatFieldPickItem Copy() {
			return (PatFieldPickItem)this.MemberwiseClone();
		}
	}
}
