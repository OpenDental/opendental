using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  
	///All schema changes are done directly on our live database as needed.
	///Used to store a brief history of a job including approved job states and cahnges to expert or engineer.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	public class JobLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobLogNum;
		///<summary>FK to job.JobNum.  Links this event to the source job.</summary>
		public long JobNum;
		///<summary>FK to userod.UserNum. Usernum of user that caused the change</summary>
		public long UserNumChanged;
		///<summary>FK to userod.UserNum. Usernum of the expert on the job at the time of the log entry.</summary>
		public long UserNumExpert;
		///<summary>FK to userod.UserNum. Usernum of the engineer on the job at the time of the log entry.</summary>
		public long UserNumEngineer;
		///<summary>Date/Time the event was created.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeEntry;
		///<summary>Human readable description of what was changed.</summary>
		public string Description;
		///<summary>Copy of the job implementation rtf</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string MainRTF; 
		///<summary>Copy of the job title.</summary>
		public string Title;
		///<summary>Copy of the job requirements rtf</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TextIsClob)]
		public string RequirementsRTF;
		///<summary>Copy of the job Time Estimate</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeSpanLong)]
		[XmlIgnore]
		public TimeSpan TimeEstimate;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("TimeEstimate",typeof(long))]
		public long TimeEstimateXml {
			get {
				return TimeEstimate.Ticks;
			}
			set {
				TimeEstimate=TimeSpan.FromTicks(value);
			}
		}

		///<summary>The estimated hours a job will take.</summary>
		[XmlIgnore,JsonIgnore]
		public double HoursEstimate {
			get {
				return TimeEstimate.TotalHours;
			}
			set {
				TimeEstimate=TimeSpan.FromHours(value);
			}
		}

		///<summary></summary>
		public JobLog Copy() {
			return (JobLog)this.MemberwiseClone();
		}
	}

}



//CREATE TABLE joblog(
//JobLogNum bigint NOT NULL auto_increment PRIMARY KEY,
//	JobNum bigint NOT NULL,
//	UserNumChanged bigint NOT NULL,
//	UserNumExpert bigint NOT NULL,
//	UserNumEngineer bigint NOT NULL,
//	DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
//	Description varchar(255) NOT NULL,
//	MainRTF text NOT NULL,
//	INDEX(JobNum),
//  INDEX(UserNumChanged),
//  INDEX(UserNumExpert),
//  INDEX(UserNumEngineer)
//) DEFAULT CHARSET = utf8;

//#To convert old jobEvent rows into JobLog rows.
//INSERT INTO joblog(jobnum,usernumchanged,DateTimeEntry,Description,MainRTF)
//SELECT jobnum, usernumevent, DateTimeEntry, 'Old job event entry. Description not available.', Description AS mRTF FROM jobevent;