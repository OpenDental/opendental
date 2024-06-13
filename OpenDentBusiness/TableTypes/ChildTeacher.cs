using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table. Represents an individual teacher in the daycare.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class ChildTeacher:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildTeacherNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Any notes for a teacher.</summary>
		public string Notes;

		public ChildTeacher Copy(){
			return (ChildTeacher)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childteacher";
		Db.NonQ(command);
		command=@"CREATE TABLE childteacher (
			ChildTeacherNum bigint NOT NULL auto_increment PRIMARY KEY,
			UserNum bigint NOT NULL,
			Notes varchar(255) NOT NULL,
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}