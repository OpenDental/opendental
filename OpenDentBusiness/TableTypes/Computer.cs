using System;
using System.Collections;
using System.Xml.Serialization;

namespace OpenDentBusiness{

	///<summary>Keeps track of the computers in an office.  The list will eventually become cluttered with the names of old computers that are no longer in service.  The old rows can be safely deleted.  Although the primary key is used in at least one table, this will probably be changed, and the computername will become the primary key.</summary>
	[Serializable]
	public class Computer:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ComputerNum;
		///<summary>Name of the computer.</summary>
		public string CompName;
		///<summary>Allows us to tell which computers are running.  All workstations record a heartbeat here at an interval of 3 minutes.  
		///So if the heartbeat is fairly fresh, then that's an accurate indicator of whether Open Dental is running on that computer.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastHeartBeat;

		public Computer Copy() {
			return (Computer)this.MemberwiseClone();
		}		
	}

	

	



}









