using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table for daycare. One entry is made in this table each time a child or teacher joins or leaves a room. A major purpose is to track child/teacher ratios. Each row will either be an entry for a child or a teacher. For example, if the entry if for a child then the ChildNum column will have a number and the ChildTeacher column will be zero.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class ChildRoomLog:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildRoomLogNum;
		///<summary>The actual time that this entry was entered.  Cannot be 01-01-0001.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntered;
		///<summary>The time to display and to use in all calculations.  Cannot be 01-01-0001. Usually the same as DateTEntered unless user needs to adjust for some reason.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime DateTDisplayed;
		///<summary>FK to child.ChildNum. Will be 0 if this is for a teacher.</summary>
		public long ChildNum;
		///<summary>FK to childteacher.ChildTeacherNum. Will be 0 if this is for a child.</summary>
		public long ChildTeacherNum;
		///<summary>True if coming into the room, false if leaving.</summary>
		public bool IsComing;
		///<summary>FK to childroom.ChildRoomNum.</summary>
		public long ChildRoomNum;
		
		public ChildRoomLog Copy(){
			return (ChildRoomLog)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childroomlog";
		Db.NonQ(command);
		command=@"CREATE TABLE childroomlog (
			ChildRoomLogNum bigint NOT NULL auto_increment PRIMARY KEY,
			DateTEntered datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
			DateTDisplayed datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
			ChildNum bigint NOT NULL,
			ChildTeacherNum bigint NOT NULL,
			IsComing tinyint NOT NULL,
			ChildRoomNum bigint NOT NULL,
			INDEX(DateTDisplayed)
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}
