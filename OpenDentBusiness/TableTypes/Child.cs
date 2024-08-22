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
		///<summary>A unique number that corresponds to the number on a child badge. The last numbers on an child badge. Will be 1 to 4 digits. These numbers are assigned to the badges by the factory.</summary>
		public string BadgeId;
		///<summary>Set true to hide a child. False by default.</summary>
		public bool IsHidden;

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
			Notes varchar(255) NOT NULL,
			BadgeId varchar(255) NOT NULL,
			IsHidden tinyint NOT NULL
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}

/*
The daycare has its own database. They enter their own users from scratch for anyone who needs to use a badge. This can include teachers, kids, parents, and guardians.

The daycare will have 8 classrooms. 5 of the classrooms will support 10 children and staff. The other three will support 20 children and staff. Per Oregon law, there are teacher/child ratios that must be met. Example: 10 preschool kids to 1 teacher equals a ratio of 10. Any more children than the allowed ratio would exceed the legal limit.

The daycare has a "map" window that contains grids for each of the classrooms and a grid to store absent children. Daycare staff can move both children and teachers around by right clicking on a grid and using one of the right click options. If a staff member attempts to add a child or teacher to a room they are currently in, they will be given a popup and no new log will be sent to the database.

Parents will be able to check in/out their children. If they have their employee badge they will be able to scan it and have their children come up in a listbox to check in or out. If they do not have their badge, they will be able to pick their children from a list of all the children and check them in or out.

Children will also have their own badges and be able to check themselves in/out as well.

The following SQL was run on the MServer206 daycare database on 8/21/2024 instead of adding to ConvertDatabase8
SQL
ALTER TABLE grouppermission MODIFY PermType smallint NOT NULL;
SELECT DISTINCT UserGroupNum FROM grouppermission;
INSERT INTO grouppermission (UserGroupNum,PermType) VALUES (1,257); //AdminGroup
INSERT INTO grouppermission (UserGroupNum,PermType) VALUES (2,257); //Regular Users (teachers)
//Parents do not get this permission
*/