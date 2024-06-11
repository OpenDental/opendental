using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
		///<summary>HQ only table. Represents one of the physical rooms teachers and children will be located in. Teachers get assigned to childroom in userod.ChildRoomNum. Children get assigned to the childroom in child.ChildRoomNum.</summary>
		[Serializable]
		[CrudTable(IsMissingInGeneral=true)]
		public class ChildRoom:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChildRoomNum;
		///<summary>The room identification comprised of a building letter and a room number . Example H1-3.</summary>
		public string RoomId;
		///<summary>Any notes for a classroom.</summary>
		public string Notes;

		public ChildRoom Copy(){
			return (ChildRoom)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childroom";
		Db.NonQ(command);
		command=@"CREATE TABLE childroom (
			ChildRoomNum bigint NOT NULL auto_increment PRIMARY KEY,
			RoomId varchar(255) NOT NULL,
			Notes varchar(255) NOT NULL
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}