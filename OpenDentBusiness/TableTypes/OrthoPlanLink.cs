using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	/// <summary>Used to attach payment plans and ortho schedules to an Ortho Case. </summary>
	[Serializable]
	public class OrthoPlanLink:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long OrthoPlanLinkNum;
		///<summary>FK to orthocase.OrthoCaseNum.</summary>
		public long OrthoCaseNum;
		///<summary>Enum:OrthoPlanLinkType  Holds the type of object that is being linked. </summary>
		public OrthoPlanLinkType LinkType;
		///<summary>Holds the FKey of the object from the LinkType. </summary>
		public long FKey;
		///<summary>Denotes if plan link is active or not.</summary>
		public bool IsActive;
		///<summary>DateTime. Date plan link was added. Not editable by user. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;
		///<summary>FK to userod.UseNum. User that added the plan link.</summary>
		public long SecUserNumEntry;


		///<summary></summary>
		public OrthoPlanLink Copy(){
			return (OrthoPlanLink)this.MemberwiseClone();
		}

	}

	public enum OrthoPlanLinkType {
		///<summary>0 - OrthoSchedule </summary>
		OrthoSchedule,
		///<summary>1 - Insurance Payment Plan </summary>
		InsPayPlan,
		///<summary>2 - Patient Payment Plan </summary>
		PatPayPlan,
	}
}