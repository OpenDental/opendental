using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table. Links one child to one parent. Example 2 parents with 2 kids will require 4 entries in this table. "Parent" really includes any adult who is authorized for pickup. So grandparents, uncles, friends, etc could all be listed here for a child.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class ChildParentLink:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildParentLinkNum;
		///<summary>FK to child.ChildNum.</summary>
		public long ChildNum;
		///<summary>FK to childparent.ChildParentNum.</summary>
		public long ChildParentNum;
		///<summary>Who the parent is to the child. This may include Mother, Father, Uncle, Friend, Grandparent...</summary>
		public string Relationship;

		public ChildParentLink Copy(){
			return (ChildParentLink)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childparentlink";
		Db.NonQ(command);
		command=@"CREATE TABLE childparentlink (
			ChildParentLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
			ChildNum bigint NOT NULL,
			ChildParentNum bigint NOT NULL,
			Relationship varchar(255) NOT NULL,
			INDEX(ChildNum),
			INDEX(ChildParentNum)
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}