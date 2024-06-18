using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table. Links one child to one parent. Example 2 parents with 2 kids will require 4 entries in this table. If a parent is not an employee, they will still be entered as an OD user with a badge. "Parent" really includes any adult who is authorized for pickup. So grandparents, uncles, friends, etc could all be listed here for a child.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class ChildParent:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildParentNum;
		///<summary>FK to child.ChildNum.</summary>
		public long ChildNum;
		///<summary>FK to userod.UserNum. Typically an employee over in the Customers db, but could also just be for a badge given to another guardian. In that case, the UserNum will not be present in Customers db and will be outside their normal range.</summary>
		public long Parent;
		//We will probably soon add a Name field here for when we don't want to actually link to a usernum and they don't need a card.

		public ChildParent Copy(){
			return (ChildParent)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childparent";
		Db.NonQ(command);
		command=@"CREATE TABLE childparent (
			ChildParentNum bigint NOT NULL auto_increment PRIMARY KEY,
			ChildNum bigint NOT NULL,
			Parent bigint NOT NULL
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}