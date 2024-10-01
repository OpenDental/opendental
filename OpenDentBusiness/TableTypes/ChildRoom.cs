using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>HQ only table for daycare. Represents one of the physical rooms teachers and children will be located in. Teachers and Children all get attached to ChildRooms in ChildRoomLog. Cache this table.</summary>
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
		///<summary>Tracks the allowed ratio of children to teachers for a given classroom. The actual ratio is calculated during runtime. Example: 10 or 5. Can be zero if they didn't set anything. Will be -1 for mixed age. Calculated ratio (not this one) could have decimal in it.</summary>
		public double Ratio;

		public ChildRoom Copy(){
			return (ChildRoom)this.MemberwiseClone();
		}

		/*
		command="DROP TABLE IF EXISTS childroom";
		Db.NonQ(command);
		command=@"CREATE TABLE childroom (
			ChildRoomNum bigint NOT NULL auto_increment PRIMARY KEY,
			RoomId varchar(255) NOT NULL,
			Notes varchar(255) NOT NULL,
			Ratio double NOT NULL
			) DEFAULT CHARSET=utf8";
		Db.NonQ(command);
		*/
	}
}

//We need a new Enumerations.InvalidType: Child
