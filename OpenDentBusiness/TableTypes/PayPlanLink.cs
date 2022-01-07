using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{

	/// <summary>Links production (Adjustments,Procedures,and OrthoCases) to Dynamic Payment Plans which will be credited based on amount
	/// from the linked production or from the amount override if specified.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class PayPlanLink:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayPlanLinkNum;
		///<summary>FK to payplan.PayPlanNum</summary>
		public long PayPlanNum;
		///<summary>Enum:PayPlanLinkType  The object type being linked to be credited. </summary>
		public PayPlanLinkType LinkType;
		///<summary>Stores the FKey of object being linked, known from link type. </summary>
		public long FKey;
		///<summary>Optional override if full amount of object is not desired. </summary>
		public double AmountOverride;
		///<summary>DateTime. Date the link was created.  </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime SecDateTEntry;

		///<summary></summary>
		public PayPlanLink Copy(){
			return (PayPlanLink)this.MemberwiseClone();
		}

	}

	public enum PayPlanLinkType {
		///<summary>0 - None. Should only be this when charges/credits are for regular static payment plans. </summary>
		None,
		///<summary>1 - Adjustment </summary>
		Adjustment,
		///<summary>2 - Procedure </summary>
		Procedure,
		///<summary>3 - OrthoCase </summary>
		OrthoCase,

	}
}
