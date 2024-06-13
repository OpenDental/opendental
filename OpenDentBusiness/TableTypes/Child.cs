using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table. Represents an individual child in the daycare.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class Child:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildNum;
		///<summary>FK to childroom.ChildRoomNum. Primary room that this child is usually assigned to.</summary>
		public long ChildRoomNumPrimary;
		///<summary>First name.</summary>
		public string FName;
		///<summary>Last name.</summary>
		public string LName;
		///<summary>Age is not stored in the database. Age is always calculated as needed from birthdate.</summary>
		public DateTime BirthDate;
		///<summary>Any notes for a child, such as allergies.</summary>
		public string Notes;

		public Child Copy(){
			return (Child)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS child";
		Db.NonQ(command);
		command=@"CREATE TABLE child (
			ChildNum bigint NOT NULL auto_increment PRIMARY KEY,
			ChildRoomNumPrimary bigint NOT NULL,
			FName varchar(255) NOT NULL,
			LName varchar(255) NOT NULL,
			BirthDate date NOT NULL DEFAULT '0001-01-01',
			Notes varchar(255) NOT NULL
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}