using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>This table is not part of the general release.  User would have to add it manually.  There is one entry for each employee. Keeps track of phone defaults for the employee, such as extension and ring group.  It's designed to not change much.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class PhoneEmpDefault:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmployeeNum;
		///<summary></summary>
		public bool IsGraphed;
		///<summary></summary>
		public bool HasColor;
		///<summary>Enum:AsteriskRingGroups 0=all, 1=none, 2=backup</summary>
		public AsteriskQueues RingGroups;
		///<summary>Used for sorting, filtering, etc. of this PhoneEmpDefault.</summary>
		public string EmpName;
		///<summary>The phone extension for the employee.  e.g. 101,102,etc.  Used to be in the employee table.  This can be changed daily by staff who float from workstation to workstation.  Can be 0 in order to keep two rows from sharing the same extension.</summary>
		public int PhoneExt;
		///<summary>Enum:PhoneEmpStatusOverride: None, Unavailable, OfflineAssist </summary>
		public PhoneEmpStatusOverride StatusOverride;
		///<summary>Used to be stored as phoneoverride.Explanation.</summary>
		public string Notes;
		///<summary>Deprecated.  Always set to true because we no longer capture screen shots.</summary>
		public bool IsPrivateScreen;
		///<summary>Used to launch a task window instead of a commlog window when user clicks on name/phone number on the bottom left.</summary>
		public bool IsTriageOperator;
		///<summary>DEPRECATED. Order of escalation importantance. Employees are ranked 1-n in order of importance. 1 is most important, 'n' is least important. -1 means employee is not included in escalation.</summary>
		public int EscalationOrder;
		///<summary>FK to site.SiteNum  This represents the "location" of which this PhoneExt is at.  Helps with reserving conference rooms.</summary>
		public long SiteNum;

		///<summary></summary>
		public PhoneEmpDefault Clone() {
			return (PhoneEmpDefault)this.MemberwiseClone();
		}

		///<summary></summary>
		public PhoneEmpDefault Copy(){
			return (PhoneEmpDefault)MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum AsteriskQueues {
		///<summary>0 - Adds ext into ODQueueReception and then removes ext from ODQueueTriage and ODQueueBackup.</summary>
		Tech,
		///<summary>1 - Removes ext from ODQueueReception, ODQueueTriage, and ODQueueBackup.</summary>
		None,
		///<summary>2 - Adds ext into ODQueueBackup and then removes ext from ODQueueReception and ODQueueTriage.</summary>
		Backup,
		///<summary>3 - Adds ext into ODQueueTriage and ODQueueBackup then removes ext from ODQueueReception.</summary>
		Triage,
	}

	public enum PhoneEmpStatusOverride {
		///<summary>0 - None.</summary>
		None,
		///<summary>1 </summary>
		Unavailable,
		///<summary>2</summary>
		OfflineAssist
	}

	/*CREATE TABLE `phoneempdefault` (
		`EmployeeNum` bigint(20) NOT NULL,
		`IsGraphed` tinyint(4) NOT NULL,
		`HasColor` tinyint(4) NOT NULL,
		`RingGroups` int(11) NOT NULL,
		`EmpName` varchar(255) NOT NULL DEFAULT '',
		`PhoneExt` int(11) NOT NULL,
		`StatusOverride` tinyint(4) NOT NULL,
		`Notes` text NOT NULL,
		`ComputerName` varchar(255) NOT NULL,
		`IsPrivateScreen` tinyint(4) NOT NULL,
		`IsTriageOperator` tinyint(4) NOT NULL,
		`EscalationOrder` int(11) NOT NULL DEFAULT '-1',
		`SiteNum` bigint(20) NOT NULL,
		PRIMARY KEY (`EmployeeNum`)
	) ENGINE=MyISAM DEFAULT CHARSET=utf8 */

}


//2020-09-24-This table was causing slowness issues.  PhoneEmpDefaults.Refresh was being called from all workstations every 1.6 seconds.
//If I did my math right (10hrs x 60min x 60sec /1.6sec x 250comps), that's something like 5.6 million queries per day just for this one table.
//I verified that none of the fields are set very often except StatusOverride.
//I'm going to assume StatusOverride and IsGraphed are set about 300 times per day, or about once every two minutes.
//A standard cache pattern would, therefore, reduce traffic by a factor of 75, getting us down to 75k queries per day.
//A more elegant pattern for later would be to move IsGraphed and StatusOverride out to another table so that PhoneEmpDefault only changes as new employees are added.
//The only table that should be getting hit every 1.6 seconds should be the phone table, and we are a long way from that ideal.


