using System;
using System.Collections;
using OpenDentBusiness;

namespace OpenDentBusiness{
	
	///<summary>This schema is copied directly from JRMT. Do not rename columns here.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudExcludePrefC=true)]
	public class Bug{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long BugId;
		///<summary>Includes time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime CreationDate;
		public BugStatus Status_;
		public BugType Type_;
		public int PriorityLevel;
		public string VersionsFound;
		public string VersionsFixed;
		public string Description;
		public string LongDesc;
		public string PrivateDesc;
		public string Discussion;
		///<summary>NOT UserNum</summary>
		public long Submitter;

		///<summary>Returns a copy of this Bug.</summary>
		public Bug Copy(){
			return (Bug)this.MemberwiseClone();
		}

		public Bug() {
			//Needed for middle tier serialization.	
		}

	}

	///<Summary>This is stored in the db as an enum rather than an int.</Summary>
	public enum BugStatus {
		DontUnderstand,
		Fixed,
		ByDesign,
		Deleted,
		WorksForMe,
		WontFix,
		Verified,
		Duplicate,
		Accepted,
		None
	}

	public enum BugType {
		Bug,
		Request
	}

	


	


}









