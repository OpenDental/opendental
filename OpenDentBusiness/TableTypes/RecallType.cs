using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>All recalls are based on these recall types.  Recall triggers are in their own table.</summary>
	[Serializable()]
	public class RecallType:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RecallTypeNum;
		///<summary>.</summary>
		public string Description;
		///<summary>The interval between recalls.  The Interval struct combines years, months, weeks, and days into a single integer value.</summary>
		public Interval DefaultInterval;
		///<summary>Stores the length of the appointment in /'s and X's.  Used when scheduling the appointment.
		///Each / or X represents one unit in regards to the global 'Time Increments' appointment view setting.
		///This means that recall appointment lengths change along with the 'Time Increments' preference.  /X/ could rep 15 mins, 30 mins, etc.</summary>
		public string TimePattern;
		///<summary>What procedures to put on the recall appointment.  Comma delimited set of ProcCodes.  (We may change this to CodeNums).</summary>
		public string Procedures;
		///<summary>Set to true if this recall type should be automatically appended to the appointment when scheduling a special recall type.
		///This boolean only gets considered if this recall type is a "manual" or "custom" recall type.
		///If this recall type is flagged as a special recall type then variable is ignored.</summary>
		public bool AppendToSpecial;
		
		public RecallType Copy(){
			return (RecallType)this.MemberwiseClone();
		}	
	}
}

