using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class FeeSchedT {
		///<summary>Returns feeSchedNum</summary>
		public static long CreateFeeSched(FeeScheduleType feeSchedType,string suffix,bool isGlobal=true) {
			FeeSched feeSched=GetNewFeeSched(feeSchedType,suffix,isGlobal);
			FeeScheds.RefreshCache();
			return feeSched.FeeSchedNum;
		}

		public static FeeSched GetNewFeeSched(FeeScheduleType feeSchedType,string suffix,bool isGlobal=true) {
			FeeSched feeSched=new FeeSched();
			feeSched.FeeSchedType=feeSchedType;
			feeSched.Description="FeeSched"+suffix;
			feeSched.IsGlobal=isGlobal;
			FeeScheds.Insert(feeSched);
			return feeSched;
		}

		public static long GetUCRFeeSched(Patient pat) {
			return Providers.GetProv(Patients.GetProvNum(pat)).FeeSched;
		}

		public static void UpdateUCRFeeSched(Patient pat,long feeSched) {
			Provider prov=Providers.GetProv(Patients.GetProvNum(pat));
			prov.FeeSched=feeSched;
			Providers.Update(prov);
			Providers.RefreshCache();
		}

	}
}
