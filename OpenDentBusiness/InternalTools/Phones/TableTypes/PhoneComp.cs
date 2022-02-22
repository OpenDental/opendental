using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  Links computers to extensions.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	public class PhoneComp:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PhoneCompNum;
		///<summary>The phone extension that is assigned to the corresponding computer.</summary>
		public int PhoneExt;
		///<summary>This name of the computer that has been assigned to the corresponding extension.</summary>
		public string ComputerName;

		///<summary></summary>
		public PhoneComp Copy() {
			return (PhoneComp)this.MemberwiseClone();
		}
	}

}

/*
CREATE TABLE phonecomp (
	PhoneCompNum bigint NOT NULL auto_increment PRIMARY KEY,
	PhoneExt int NOT NULL,
	ComputerName varchar(255) NOT NULL
	) DEFAULT CHARSET=utf8
*/
