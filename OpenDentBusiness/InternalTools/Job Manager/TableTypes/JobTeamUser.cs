using System;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually. Most schema changes are done directly on our live database as needed.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	public class JobTeamUser:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobTeamUserNum;
		///<summary>FK to JobTeam.JobTeamNum.</summary>
		public long JobTeamNum;
		///<summary>FK to userod.UserNum. Indicates which engineer is associated to this JobTeamUser.</summary>
		public long UserNumEngineer;
		///<summary>Indicates whether the user is a team lead.</summary>
		public bool IsTeamLead;

		public JobTeamUser Copy() {
			return (JobTeamUser)MemberwiseClone();
		}
	}
}
