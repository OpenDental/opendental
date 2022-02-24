using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  
	///All schema changes are done directly on our live database as needed.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class ChatUser:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ChatUserNum;
		///<summary></summary>
		public int Extension;
		///<summary></summary>
		public int CurrentSessions;
		///<summary>Milliseconds.</summary>
		public long SessionTime;
		///<summary></summary>

		public ChatUser Copy() {
			return (ChatUser)this.MemberwiseClone();
		}

	}

	
}

/*CREATE TABLE chatuser (
	ChatUserNum bigint NOT NULL auto_increment PRIMARY KEY,
	Extension int NOT NULL,
	CurrentSessions int NOT NULL,
	SessionTime bigint NOT NULL
	) DEFAULT CHARSET=utf8*/


