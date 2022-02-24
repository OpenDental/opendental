using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	///<summary>Stores the default note and time increments for one procedure code for one provider.  That way, an unlimited number of providers can each have different notes and times.  These notes and times override the defaults which are part of the procedurecode table.  So, for single provider offices, there will be no change to the current interface.</summary>
	[Serializable]
	public class ProcCodeNote:TableBase {
		///<summary>Primary Key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProcCodeNoteNum;
		///<summary>FK to procedurecode.CodeNum.</summary>
		public long CodeNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>The note.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>X's and /'s describe Dr's time and assistant's time in the same increments as the user has set.</summary>
		public string ProcTime;
		///<summary>Enum:ProcStat Indicates which status the procedure has to be set to in order for this note to take affect.
		///Should only ever be 1 (TP) or 2 (C).  See procedurelog.ProcStatus for more info.</summary>
		public ProcStat ProcStatus;

		///<summary>Returns a copy of this ProcCodeNote</summary>
		public ProcCodeNote Copy(){
			return (ProcCodeNote)MemberwiseClone();
		}


	}

	
	
	


}










