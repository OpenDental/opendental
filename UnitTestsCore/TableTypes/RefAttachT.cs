using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class RefAttachT {

		public static RefAttach CreateRefAttach(Referral referral,long patNum,DateTime dateReferral,ReferralType referralType,ReferralToStatus referralToStatus,
			string note="",long procNum=0,long provNum=0) 
		{
			RefAttach refAttach=new RefAttach();
			refAttach.ReferralNum=referral.ReferralNum;
			refAttach.PatNum=patNum;
			refAttach.RefDate=dateReferral;
			refAttach.RefType=referralType;
			refAttach.RefToStatus=referralToStatus;
			refAttach.Note=note;
			refAttach.IsTransitionOfCare=referral.IsDoctor;
			refAttach.ProcNum=procNum;
			refAttach.ProvNum=provNum;
			refAttach.ItemOrder=0;
			RefAttaches.Insert(refAttach);
			return refAttach;
		}

	}
}
