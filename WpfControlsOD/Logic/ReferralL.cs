using System;
using OpenDentBusiness;
using CodeBase;
using OpenDental;

namespace WpfControls {
	public class ReferralL {

		///<summary>Attemmpts to get a referral.
		///Returns null and shows a MsgBox if there was an error.</summary>
		public static Referral GetReferral(long referralNum,bool isMsgShown=true) {
			Referral referral=null;
			try{
				referral=Referrals.GetReferral(referralNum);
			}
			catch(ApplicationException appEx){ 
				appEx.DoNothing();
				if(isMsgShown){
					MsgBox.Show("Referrals","Could not retrieve referral. Please run Database Maintenance or call support.");
				}
			}
			return referral;
		}
	}
}
