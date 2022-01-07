using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary></summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class ClaimTracking:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClaimTrackingNum;
		///<summary>FK to claim.ClaimNum</summary>
		public long ClaimNum;
		///<summary>Enum:ClaimTrackingType Identifies the type of claimtracking row.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public ClaimTrackingType TrackingType;
		///<summary>FK to user.UserNum</summary>
		public long UserNum;
		///<summary>When the row was inserted.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTimeEntry;
		///<summary>Generic column for additional info.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>FK to definition.DefNum for custom tracking when TrackingType=StatusHistory</summary>
		public long TrackingDefNum;
		///<summary>FK to definition.DefNum for custom tracking errors when TrackingType=StatusHistory</summary>
		public long TrackingErrorDefNum;

		///<summary></summary>
		public ClaimTracking Copy() {
			return (ClaimTracking)this.MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum ClaimTrackingType {
		///<summary></summary>
		StatusHistory,
		///<summary></summary>
		ClaimUser,
		///<summary></summary>
		ClaimProcReceived
	}
}

	/*
		if(DataConnection.DBtype==DatabaseType.MySql) {
			command="DROP TABLE IF EXISTS claimtracking";
			Db.NonQ(command);
			command=@"CREATE TABLE claimtracking (
				ClaimTrackingNum bigint NOT NULL auto_increment PRIMARY KEY,
				ClaimNum bigint NOT NULL,
				TrackingType varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				DateTimeEntry timestamp,
				Note text NOT NULL,
				TrackingDefNum bigint NOT NULL,
				INDEX(ClaimNum),
				INDEX(UserNum),
				INDEX(TrackingDefNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
		}
		else {//oracle
			command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE claimtracking'; EXCEPTION WHEN OTHERS THEN NULL; END;";
			Db.NonQ(command);
			command=@"CREATE TABLE claimtracking (
				ClaimTrackingNum number(20) NOT NULL,
				ClaimNum number(20) NOT NULL,
				TrackingType varchar2(255),
				UserNum number(20) NOT NULL,
				DateTimeEntry timestamp,
				Note clob,
				TrackingDefNum number(20) NOT NULL,
				CONSTRAINT claimtracking_ClaimTrackingNum PRIMARY KEY (ClaimTrackingNum)
				)";
			Db.NonQ(command);
			command=@"CREATE INDEX claimtracking_ClaimNum ON claimtracking (ClaimNum)";
			Db.NonQ(command);
			command=@"CREATE INDEX claimtracking_UserNum ON claimtracking (UserNum)";
			Db.NonQ(command);
			command=@"CREATE INDEX claimtracking_TrackingDefNum ON claimtracking (TrackingDefNum)";
			Db.NonQ(command);
		}
*/