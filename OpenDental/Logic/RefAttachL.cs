using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public class RefAttachL{

		///<summary>Pass in all the refattaches for the patient.  This funtion finds the first referral from a Dr and returns that Dr's name.  Used in specialty practices.  Function is only used right now in the Dr. Ceph bridge.</summary>
		public static string GetReferringDr(List<RefAttach> listRefAttaches){
			if(listRefAttaches.Count==0){
				return "";
			}
			if(listRefAttaches[0].RefType!=ReferralType.RefFrom){
				return "";
			}
			Referral referral=WpfControls.ReferralL.GetReferral(listRefAttaches[0].ReferralNum);
			if(referral==null || referral.PatNum!=0){
				return "";
			}
			string retVal=referral.FName+" "+referral.MName+" "+referral.LName;
			if(referral.Title!=""){
				retVal+=", "+referral.Title;
			}
			return retVal;
		}		

	}
}