using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.
	/// Gives permission if a row exists.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,IsSynchable=true)]
	//[CrudTable(IsSynchable=true)]
	public class JobPermission:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long JobPermissionNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>Enum:JobPermissions The role type that this user has.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public JobPerm JobPermType;

		///<summary></summary>
		public JobPermission Copy() {
			return (JobPermission)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum JobPerm {
		///<summary>0 -</summary>
		Writeup,
		///<summary>1 -</summary>
		Assignment,
		///<summary>2 -</summary>
		Approval,
		///<summary>3 -</summary>
		Documentation,
		///<summary>4 -</summary>
		Review,
		///<summary>5 -</summary>
		Engineer,
		///<summary>6 -</summary>
		Concept,
		///<summary>7 -</summary>
		SeniorQueryCoordinator,
		///<summary>8 -</summary>
		FeatureManager,
		///<summary>9 -</summary>
		NotifyCustomer,
		///<summary>10 -</summary>
		Quote,
		///<summary>11 -</summary>
		Override,
		///<summary>12 -</summary>
		QueryCoordinator,
		///<summary>13 -</summary>
		QueryTech,
		///<summary>14 -</summary>
		DocumentationManager,
		///<summary>15 -</summary>
		TestingCoordinator,
		///<summary>16 -</summary>
		SpecialProject,
		///<summary>17 -</summary>
		PatternReview,
		///<summary>18 -</summary>
		ProjectManager,
		///<summary>19 -</summary>
		DesignTech,
		///<summary>20 -</summary>
		DesignCoordinator,
		///<summary>21 -</summary>
		MarketingManager,
		///<summary>22 -</summary>
		UnresolvedIssues,
	}

}

