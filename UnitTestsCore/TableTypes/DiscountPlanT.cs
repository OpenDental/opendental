using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class DiscountPlanT {

		///<summary></summary>
		public static DiscountPlan CreateDiscountPlan(string description,long defNum=0,long feeSchedNum=0,bool isHidden=false,double annualMax=-1,
			int examFreqLimit=-1,int xrayFreqLimit=-1,int prophyFreqLimit=-1,int fluorideFreqLimit=-1,int perioFreqLimit=-1,int limitedFreqLimit=-1,int paFreqLimit=-1) {
			DiscountPlan discountPlan=new DiscountPlan() {
				Description=description,
				DefNum=defNum,
				FeeSchedNum=feeSchedNum,
				IsHidden=isHidden,
				AnnualMax=annualMax,
				ExamFreqLimit=examFreqLimit,
				XrayFreqLimit=xrayFreqLimit,
				ProphyFreqLimit=prophyFreqLimit,
				FluorideFreqLimit=fluorideFreqLimit,
				PerioFreqLimit=perioFreqLimit,
				LimitedExamFreqLimit=limitedFreqLimit,
				PAFreqLimit=paFreqLimit,
			};
			DiscountPlans.Insert(discountPlan);
			return discountPlan;
		}

		public static void ClearDiscountPlanPrefs() {
			PrefT.UpdateString(PrefName.DiscountPlanExamCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanXrayCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanProphyCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanFluorideCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanPerioCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanLimitedCodes,"");
			PrefT.UpdateString(PrefName.DiscountPlanPACodes,"");
		}

	}
}
