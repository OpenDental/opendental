using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Deprecated. This table is not part of the general release.  User would have to add it manually. 
	///All schema changes are done directly on our live database as needed.
	///This table is used to keep track of user preferences in regards to which 'controls' they want to have open within the Job Manager Dashboard
	///window and their location.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class JobControl:TableBase {//deprecated
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobControlNum;
		///<summary></summary>
		public long UserNum;
		///<summary></summary>
		public JobControlType JobControlType;  
		///<summary></summary>
		public string ControlData;
		///<summary></summary>
		public int XPos;
		///<summary></summary>
		public int YPos;
		///<summary></summary>
		public int Width;
		///<summary></summary>
		public int Height;

		///<summary></summary>
		public JobControl Copy() {
			return (JobControl)this.MemberwiseClone();
		}
	}

	public enum JobControlType {
		///<summary>0 -</summary>
		Projects,
		///<summary>1 -</summary>
		Jobs,
		///<summary>2 -</summary>
		Documentation,
		///<summary>3 -</summary>
		Bugs,
		///<summary>4 -</summary>
		FeatureRequests,
		///<summary>5 -</summary>
		QueryRequests
	}

}

/*
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS jobcontrol";
					Db.NonQ(command);
					command=@"CREATE TABLE jobcontrol (
						JobControlNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						JobControlType tinyint NOT NULL,
						ControlData varchar(255) NOT NULL,
						XPos int NOT NULL,
						YPos int NOT NULL,
						Width int NOT NULL,
						Height int NOT NULL,
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE jobcontrol'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE jobcontrol (
						JobControlNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						JobControlType number(3) NOT NULL,
						ControlData varchar2(255),
						XPos number(11) NOT NULL,
						YPos number(11) NOT NULL,
						Width number(11) NOT NULL,
						Height number(11) NOT NULL,
						CONSTRAINT jobcontrol_JobControlNum PRIMARY KEY (JobControlNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX jobcontrol_UserNum ON jobcontrol (UserNum)";
					Db.NonQ(command);
				}
				*/
