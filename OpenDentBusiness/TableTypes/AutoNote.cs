using System;
using System.Collections;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>A single autonote template.</summary>
	[Serializable()]
	[CrudTable(HasBatchWriteMethods=true)]
	public class AutoNote:TableBase{
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutoNoteNum;
		///<summary>Name of AutoNote</summary>
		public string AutoNoteName;
		///<summary>Was 'ControlsToInc' in previous versions.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MainText;
		// <summary></summary>
		//public string AutoNoteOutput;
		///<summary>FK to definition.DefNum.  This is the AutoNoteCat definition category (DefCat=41), for categorizing autonotes.
		///Uncategorized autonotes will be set to 0.</summary>
		public long Category;

		///<summary></summary>
		public AutoNote Copy() {
			return (AutoNote)this.MemberwiseClone();
		}
		
	}
}
