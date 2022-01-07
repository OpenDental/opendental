using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This schema is created directly from the Requests.GetTable() return value within WebServiceCustomersUpdates.sln
	///Do not rename the columns here.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class FeatureRequest {
		public long FeatReqNum;
		public long Votes;
		public long Critical;
		public float Pledge;
		public long Difficulty;
		public float Weight;
		public string Approval;
		public string Description;
	}

	///<summary>This enum is take directly from the WebServiceCustomerUpdates.sln
	///Do not add / move / remove enum values here unless they are manipulated within that solution as well.</summary>
	public enum ApprovalEnum {
		///<summary>0 - </summary>
		New,
		///<summary>1 - </summary>
		NeedsClarification,
		///<summary>2 - </summary>
		Redundant,
		///<summary>3 - </summary>
		TooBroad,
		///<summary>4 - </summary>
		NotARequest,
		///<summary>5 - </summary>
		AlreadyDone,
		///<summary>6 - </summary>
		Obsolete,
		///<summary>7 - </summary>
		Approved,
		///<summary>8 - </summary>
		InProgress,
		///<summary>9 - </summary>
		Complete
	}
}
