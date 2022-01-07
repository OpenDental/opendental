using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class PhoneConf:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PhoneConfNum;
		///<summary>0 to 19.  Set to -1 for conference rooms that are not associated to the messaging buttons.</summary>
		public int ButtonIndex;
		///<summary>The current number of people in the conference room.  This value is automatically manipulated by the PhoneTrackingServer.</summary>
		public int Occupants;
		///<summary>Acts like a FKey to Asterisk phone extentions. Manually manipulated to change behavior.</summary>
		public int Extension;
		///<summary>The DateTime that a user reserved this particular conference room.
		///Once this time exceeds 5 minutes and is no longer occupied it will be considered available to the next user that tries to reserve it.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeReserved;
		///<summary>FK to userod.UserNum  Indicates the user that reserved this conference room last.</summary>
		public long UserNum;
		///<summary>FK to site.SiteNum</summary>
		public long SiteNum;

		public PhoneConf Copy() {
			return (PhoneConf)this.MemberwiseClone();
		}
	}
}



