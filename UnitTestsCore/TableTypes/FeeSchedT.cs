using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class FeeSchedT {
		///<summary>Returns feeSchedNum. Refreshes FeeScheds cache.</summary>
		public static long CreateFeeSched(FeeScheduleType feeSchedType,string suffix,bool isGlobal=true,bool isHidden=false) {
			FeeSched feeSched=GetNewFeeSched(feeSchedType,suffix,isGlobal,isHidden);
			FeeScheds.RefreshCache();
			return feeSched.FeeSchedNum;
		}

		public static FeeSched GetNewFeeSched(FeeScheduleType feeSchedType,string suffix,bool isGlobal=true,bool isHidden=false) {
			FeeSched feeSched=new FeeSched();
			feeSched.FeeSchedType=feeSchedType;
			feeSched.Description="FeeSched"+suffix;
			feeSched.IsHidden=isHidden;
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

		///<summary>Deletes everything from the feesched table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearFeeSchedTable() {
			string command="DELETE FROM feesched WHERE FeeSchedNum<>53"; //53 is the Standard/Office Fees feesched, and should always exist.
			DataCore.NonQ(command);
			FeeScheds.RefreshCache();
		}

		///<summary>Deletes a single FeeSched from the table by FeeSchedNum</summary>
		public static void Delete(long feeSchedNum) {
			string command="DELETE FROM feesched WHERE FeeSchedNum="+POut.Long(feeSchedNum);
			DataCore.NonQ(command);
			FeeScheds.RefreshCache();
		}

	}
}
