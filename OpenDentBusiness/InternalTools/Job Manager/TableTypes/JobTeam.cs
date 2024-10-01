using System;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually. Most schema changes are done directly on our live database as needed.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	public class JobTeam:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobTeamNum;
		///<summary>The name of the team.</summary>
		public string TeamName;
		///<summary>The description of the team.</summary>
		public string TeamDescription;
		///<summary>Enum:JobTeamFocus The team's area of expertise/focus of work. ODProper, eServices, Cloud, etc.</summary>
		public JobTeamFocus TeamFocus;

		///<summary></summary>
		public JobTeam Copy() {
			return (JobTeam)MemberwiseClone();
		}
	}

	///<summary>Team's area of expertise/focus of work.</summary>
	public enum JobTeamFocus {
		///<summary>0 - None</summary>
		None,
		///<summary>1 - ODProper</summary>
		ODProper,
		///<summary>2 - eServices</summary>
		eServices,
		///<summary>3 - Cloud</summary>
		Cloud
	}
}
