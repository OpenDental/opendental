using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.  There should be exactly one PhoneGraph entry for each date/employee combo. Some may already be entered as exceptions to the default. Others are added automatically, but only for today's date. This creates a historical record.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class PhoneGraph:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PhoneGraphNum;
		///<summary>FK to employee.EmployeeNum. 0 indicates single daily note.</summary>
		public long EmployeeNum;
		///<summary>Overrides PhoneEmpDefault.IsGraphed for the given DateEntry</summary>
		public bool IsGraphed;
		///<summary>Date pertaining to this entry.</summary>
		public DateTime DateEntry;
		///<summary>Usually empty 0-0-0. If the first one (this one) is entered, then these entries override the normal schedule for the purposes of graphing.  Date component would always be same as DateEntry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStart1;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStop1;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStart2;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStop2;
		///<summary>A note regarding the altered schedule.  Examples: Late, out sick, leaving early, etc.  If EmployeeNum=0, then this is a single daily note.</summary>
		public string Note;
		///<summary>True if scheduled day off in advance.</summary>
		public bool PreSchedOff;
		///<summary>True if called in sick for the day.</summary>
		public bool Absent;
		///<summary>Limit on the number of PreSchedOff without getting a warning.  EmployeeNum=0 in this case. One for a day, optional.</summary>
		public int DailyLimit;
		///<summary>Presched,ShortNotice, or NotTracked</summary>
		public EnumPresched PreSchedTimes;

		public PhoneGraph Copy() {
			return (PhoneGraph)this.MemberwiseClone();
		}
	}

	public enum EnumPresched{
		Presched,
		ShortNotice,
		NotTracked
	}
}

/*
2020-07-10 3:20 PM -I ran these queries on our main Customers Db.  Any other alpha test db will need to have the same queries run if they are testing anything phone graph related.
ALTER TABLE phonegraph ADD DateTimeStart1 datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
ALTER TABLE phonegraph ADD DateTimeStop1 datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
ALTER TABLE phonegraph ADD DateTimeStart2 datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
ALTER TABLE phonegraph ADD DateTimeStop2 datetime NOT NULL DEFAULT '0001-01-01 00:00:00';
ALTER TABLE phonegraph ADD Note text NOT NULL;
2020-07-24 2:13 PM More queries while at version 20.3.8:
ALTER TABLE phonegraph ADD PreSchedOff tinyint NOT NULL;
ALTER TABLE phonegraph ADD Absent tinyint NOT NULL;
ALTER TABLE phonegraph ADD DailyLimit int NOT NULL;
ALTER TABLE phonegraph ADD INDEX (DateEntry);
ALTER TABLE phonegraph ADD PreSchedTimes tinyint NOT NULL;


 */
