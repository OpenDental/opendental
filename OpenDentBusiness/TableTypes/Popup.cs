using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>If an existing popup message gets changed, then an archive first gets created that's a copy of the original.  This is so that we can track historical changes.  When a new one gets created, all the archived popups will get automatically repointed to the new one.  If you "delete" a popup, it actually archives that popup.  All the other archives of that popup still point to the newly archived popup, but now there is no popup in that group with the IsArchived flag not set.</summary>
	[Serializable]
	public class Popup:TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PopupNum;
		/// <summary>FK to patient.PatNum.</summary>
		public long PatNum;
		/// <summary>The text of the popup.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Description;
		///<summary>If true, then the popup won't automatically show when a patient is selected.  Kind of useless except for offices that want to still show historical popups.</summary>
		public bool IsDisabled;
		///<summary>Enum:EnumPopupLevel 0=Patient, 1=Family, 2=Superfamily. If Family, then this Popup will apply to the entire family.  If Superfamily, then this popup will apply to the entire superfamily.</summary>
		public EnumPopupLevel PopupLevel;//rename to PopupLevel
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>The server time that this note was entered.  Cannot be changed by user.  Does not get changed automatically when level or isDisabled gets changed.  If note itself changes, then a new popup is created along with a new DateTimeEntry. Current popup's edit date gets set to the previous entry's DateTimeEntry</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>Indicates that this is not the most current popup and that it is an archive.  True for any archived or "deleted" popups.</summary>
		public bool IsArchived;
		///<summary>This will be zero for current popups that show when a patient is selected.  Archived popups will have a value which is the FK to its parent Popup.  The parent popup could be the most recent popup or another archived popup.  Will be zero for current and "deleted" popups.</summary>
		public long PopupNumArchive;
		//We will consider later adding Guarantor and SuperFamily FK's to speed up queries.  The disadvantage is that popups would then have to be synched every time guarantors or sf heads changed.

		public Popup Copy() {
			return (Popup)this.MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum EnumPopupLevel {
		/// <summary>0=Patient</summary>
		Patient,
		/// <summary>1=Family</summary>
		Family,
		/// <summary>2=SuperFamily</summary>
		SuperFamily,
		/// <summary>3=Automation. Not in db. This is only used in FormPopupsForFam as a dummy status for temporary display objects that will not be in db.</summary>
		Automation
	}

}









