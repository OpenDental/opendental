using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ReferralT {

		///<summary></summary>
		public static Referral CreateReferral(long patNum,bool isDoctor=true)
		{
			Referral refNew=new Referral() {
				IsDoctor=isDoctor,
				PatNum=patNum,
			};
			Referrals.Insert(refNew);
			RefAttach refattach=new RefAttach();
			refattach.ReferralNum=refNew.ReferralNum;
			refattach.PatNum=patNum;
			refattach.RefType=ReferralType.RefFrom;
			refattach.RefDate=DateTime.Today;
			refattach.IsTransitionOfCare=refNew.IsDoctor;
			refattach.ItemOrder=0;
			RefAttaches.Insert(refattach);
			Referrals.RefreshCache();
			return refNew;
		}
		
		public static Referral CreateReferralWithoutRefAttach(long patNum,bool isDoctor=false,bool isHidden=false,bool notPerson=false,string title="",
			bool isPreferred=false,string businessName="",string fName="",string lName="",string mName="",long specialty=0)
		{
			Referral refNew=new Referral();
			refNew.IsDoctor=isDoctor;
			refNew.FName=fName;
			refNew.LName=lName;
			refNew.MName=mName;
			refNew.Specialty=specialty;
			refNew.PatNum=patNum;
			refNew.IsHidden=isHidden;
			refNew.NotPerson=notPerson;
			refNew.Title=title;
			refNew.IsPreferred=isPreferred;
			refNew.BusinessName=businessName;
			Referrals.Insert(refNew);
			Referrals.RefreshCache();
			return refNew;
		}

		///<summary>Deletes everything from the referral and refattach tables.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearReferralTable() {
			string command="DELETE FROM referral WHERE ReferralNum > 0";
			DataCore.NonQ(command);
			command="DELETE FROM refattach WHERE RefAttachNum > 0";
			DataCore.NonQ(command);
			Referrals.RefreshCache();
		}
	}
}
