using OpenDentBusiness;

namespace OpenDental {
	class ReferralL {

		///<summary>Attemmpts to get a referral.
		///Returns null and shows a MsgBox if there was an error.</summary>
		public static Referral GetReferral(long referralNum,bool isMsgShown=true) {
			Referral referral;
			if(!Referrals.TryGetReferral(referralNum,out referral) && isMsgShown) {//Failed to retrieve referral
				ShowReferralErrorMsg();
			}
			return referral;
		}

		public static void ShowReferralErrorMsg() {
			MsgBox.Show("Referrals","Could not retrieve referral. Please run Database Maintenance or call support.");
		}
	}
}
