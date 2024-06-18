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

/*
The daycare will be given its own database. There is information in the live customers database that the daycare database will need to have and be synced with. Specifically this will be information from the userod table for the ChildTeacher and ChildParent tables in the daycare database. It should also be noted that a "parent" is an employee over in the live Customers db, but could also just be an entry for a badge given to another user authorized to pickup a child. In that case, the UserNum will not be present in Customers db and will be outside their normal range.
*/