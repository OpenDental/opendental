using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Makes an entry every time Open Dental has successfully updated to a newer version.
	///New entries will always be for the newest version being used so that users can see a "history" of how long they used previous versions.
	///This will also help EHR customers when attesting or when they get audited.</summary>
	[Serializable]
	public class UpdateHistory:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long UpdateHistoryNum;
		///<summary>DateTime that OD was updated to the Version.</summary>
		[CrudColumn(SpecialType =CrudSpecialColType.DateTEntry)]
		public DateTime DateTimeUpdated;
		///<summary>The version that OD was updated to.</summary>
		public string ProgramVersion;

		public UpdateHistory() { }

		public UpdateHistory(string version) {
			ProgramVersion=version;
		}

		///<summary></summary>
		public UpdateHistory Clone() {
			return (UpdateHistory)this.MemberwiseClone();
		}

	}

	
}




