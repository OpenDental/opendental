using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table. Represents a "Parent", which can be any person authorized to pick up specific children.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class ChildParent:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildParentNum;
		///<summary>First name.</summary>
		public string FName;
		///<summary>Last name.</summary>
		public string LName;
		///<summary>Any notes for a parent.</summary>
		public string Notes;
		///<summary>Set true to hide a parent. False by default.</summary>
		public bool IsHidden;

		public ChildParent Copy(){
			return (ChildParent)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childparent";
		Db.NonQ(command);
		command=@"CREATE TABLE childparent (
			ChildParentNum bigint NOT NULL auto_increment PRIMARY KEY,
			FName varchar(255) NOT NULL,
			LName varchar(255) NOT NULL,
			Notes varchar(255) NOT NULL,
			IsHidden tinyint NOT NULL
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}