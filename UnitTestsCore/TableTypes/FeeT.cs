using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class FeeT {
		
		///<summary>Inserts a new fee and then returns FeeNum.</summary>
		public static long CreateFee(long feeSchedNum,long codeNum,double amount=0,long clinicNum=0,long provNum=0) {
			long feeNum=GetNewFee(feeSchedNum,codeNum,amount,clinicNum,provNum).FeeNum;
			return feeNum;
		}

		public static Fee GetNewFee(long feeSchedNum,long codeNum,double amount=0,long clinicNum=0,long provNum=0,bool doInsert=true) {
			Fee fee=new Fee();
			fee.CodeNum=codeNum;
			fee.FeeSched=feeSchedNum;
			fee.Amount=amount;
			fee.ClinicNum=clinicNum;
			fee.ProvNum=provNum;
			if(doInsert) {
				fee.FeeNum=Fees.Insert(fee);
			}
			return fee;
		}

		///<summary>Updates an existing fee.</summary>
		public static void UpdateFee(long feeSchedNum,long codeNum,double amount,long clinicNum=0,long provNum=0) {
			Fee fee=Fees.GetFee(codeNum,feeSchedNum,clinicNum,provNum);
			if(fee==null) {
				return;
			}
			fee.Amount=amount;
			Fees.Update(fee);
		}

	}
}
