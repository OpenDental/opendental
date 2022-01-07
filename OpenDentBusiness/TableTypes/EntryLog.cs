using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Stores entries made for AppointmentCreate. Acts as an additional securitylog entry.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class EntryLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey = true)]
		public long EntryLogNum;
		///<summary>FK to userod.UserNum</summary>
		public long UserNum;
		///<summary>Enum:EntryLogFKeyType </summary>
		public EntryLogFKeyType FKeyType;
		///<summary>A foreign key to a table associated with the EntryLogFKeyType.</summary>
		public long FKey;
		///<summary>Enum:LogSources</summary>
		public LogSources LogSource;
		///<summary>The date and time of the entry.  Its value is set when inserting and can never change.  Even if a user changes the date on their computer, 
		///this remains accurate because it uses server time.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateTEntry)]
		public DateTime EntryDateTime;

		public EntryLog() {

		}

		public EntryLog(long userNum,EntryLogFKeyType keyType,long Fkey,LogSources logSource) {
			UserNum=userNum;
			FKeyType=keyType;
			FKey=Fkey;
			LogSource=logSource;
		}

		///<summary></summary>
		public EntryLog Copy() {
			return (EntryLog)this.MemberwiseClone();
		}
	}

	///<summary>These FKey Types are to be used as an identifier for what table the Fkey column is associated to</summary>
	public enum EntryLogFKeyType {
		///<summary>0</summary>
		Appointment,
	}
}
