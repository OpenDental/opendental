using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>The PhoneTrackingServer will be the only entity inserting data into this table.  It is also the only entity that deletes "old" entries.
	///Every workstation in the office will be selecting the most recent entry at its leisure.  New entries are made every ~1.6 seconds.
	///The table gets cleaned up every ~5 mintues.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class TriageMetric:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long TriageMetricNum;
		///<summary></summary>
		public int CountBlueTasks;
		///<summary></summary>
		public int CountWhiteTasks;
		///<summary></summary>
		public int CountRedTasks;
		///<summary>Time of oldest triage task or tasknote if one exists.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeOldestTriageTaskOrTaskNote;
		///<summary>Time of oldest urgent task or tasknote if one exists.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeOldestUrgentTaskOrTaskNote;
		///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>JSON serialized list of the active WebChatSessions at the time this row was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string WebChatSessions;
		///<summary>JSON serialized list of the active Remote Support sessions at the time this row was created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string RemoteSupportSessions;
	}
}


/*
CREATE TABLE triagemetric (
	TriageMetricNum bigint NOT NULL auto_increment PRIMARY KEY,
	CountBlueTasks int NOT NULL,
	CountWhiteTasks int NOT NULL,
	CountRedTasks int NOT NULL,
	DateTimeOldestTriageTaskOrTaskNote datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
	DateTimeOldestUrgentTaskOrTaskNote datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
	DateTStamp timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
	WebChatSessions mediumtext NOT NULL,
	RemoteSupportSessions mediumtext NOT NULL
	) DEFAULT CHARSET=utf8
*/
