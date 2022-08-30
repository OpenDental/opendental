using System;
using System.Collections;

namespace OpenDentBusiness {
	///<summary>Table to link referrals and clinics together.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class ReferralClinicLink:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ReferralClinicLinkNum;
		///<summary>FK to referral.ReferralNum.</summary>
		public long ReferralNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
	}
}
