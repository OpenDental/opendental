using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;
using OpenDental;
using System.Linq;

namespace UnitTestsCore {
	public class ClaimPaymentT {

		///<summary>Creates a ClaimPayment. Does not attach to claimprocs. Use daysPrevious to set the SecDateTEdit to the past.</summary>
		public static ClaimPayment CreateClaimPayment(DateTime checkDate,double checkAmt,string checkNum,string bankBranch,string note,string carrierName,DateTime dateIssued,long payType,long payGroup,int daysPrevious) {
			ClaimPayment claimPayment=new ClaimPayment() {
				CheckDate=checkDate,
				CheckAmt=checkAmt,
				CheckNum=checkNum,
				BankBranch=bankBranch,
				Note=note,
				ClinicNum=0,
				DepositNum=0,
				CarrierName=carrierName,
				DateIssued=dateIssued,
				PayType=payType,
				PayGroup=payGroup
			};
			claimPayment.ClaimPaymentNum=ClaimPayments.Insert(claimPayment);
			SetSecDateTEditToDaysPast(claimPayment,daysPrevious);
			claimPayment=ClaimPayments.GetOne(claimPayment.ClaimPaymentNum);//Get back for accurate DateTs
			return claimPayment;
		}

		///<summary>Creates and returns a list of 7 ClaimPayments with all fields utilized and varied. Useful for testing a ClaimPayment search for different fields.</summary>
		public static List<ClaimPayment> CreateVariedClaimPaymentSet() {
			//PayType Defs
			Def defPayTypeBasic=DefT.CreateDefinition(DefCat.InsurancePaymentType,"BasicPayType");
			Def defPayTypeStandard=DefT.CreateDefinition(DefCat.InsurancePaymentType,"StandardPayType");
			Def defPayTypeFancy=DefT.CreateDefinition(DefCat.InsurancePaymentType,"FancyPayType");
			//PayGroup Defs
			Def defPayGroupBasic=DefT.CreateDefinition(DefCat.ClaimPaymentGroups,"BasicPaymentGroup");
			Def defPayGroupStandard=DefT.CreateDefinition(DefCat.ClaimPaymentGroups,"StandardPaymentGroup");
			Def defPayGroupFancy=DefT.CreateDefinition(DefCat.ClaimPaymentGroups,"FancyPaymentGroup");
			List<ClaimPayment> listClaimPayments=new List<ClaimPayment>();
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-90),1952.99,"1234","South Bank","Payment Delayed","Great Insurance",DateTime.Today.AddDays(-95),defPayTypeBasic.DefNum,defPayGroupBasic.DefNum,90));
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-80),75.98,"6666","North Bank","Physical Check","OK Insurance",DateTime.Today.AddDays(-85),defPayTypeStandard.DefNum,defPayGroupStandard.DefNum,80));
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-70),9001.00,"9876","East Bank","Might Charge Interest","Bad Insurance",DateTime.Today.AddDays(-75),defPayTypeFancy.DefNum,defPayGroupFancy.DefNum,70));
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-60),0.99,"5555","West Bank","Small Payment","Subpar Insurance",DateTime.Today.AddDays(-65),defPayTypeBasic.DefNum,defPayGroupBasic.DefNum,60));
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-50),56.75,"2023","Bad Bank","Random Payment","Awesome Insurance",DateTime.Today.AddDays(-55),defPayTypeStandard.DefNum,defPayGroupStandard.DefNum,50));
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-40),9.99,"1111","Good Bank","Need more money","Fine Insurance",DateTime.Today.AddDays(-45),defPayTypeFancy.DefNum,defPayGroupFancy.DefNum,40));
			listClaimPayments.Add(CreateClaimPayment(DateTime.Today.AddDays(-20),7,"9999","Normal Bank","Old Check Bounced","Horrible Insurance",DateTime.Today.AddDays(-25),defPayTypeFancy.DefNum,defPayGroupStandard.DefNum,20));
			return listClaimPayments;
		}

		///<summary>Updates the SecDateTEdit of a ClaimPayment to the number of daysPrevious. Unable to set the SecDateTEdit to anything but DateTime.Now in the CRUD Layer.</summary>
		public static void SetSecDateTEditToDaysPast(ClaimPayment claimPayment,int daysPrevious) {
			string command="UPDATE claimpayment SET SecDateTEdit = "+POut.Date(DateTime.Now.AddDays(-daysPrevious))
				+" WHERE ClaimPaymentNum = "+POut.Long(claimPayment.ClaimPaymentNum);
			DataCore.NonQ(command);
    }

		///<summary>Deletes everything from the ClaimPayment table. Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearClaimPaymentTable() {
			string command="DELETE FROM claimpayment";
			DataCore.NonQ(command);
		}

		///<summary>Updates List of ClaimProcs to contain the ClaimPaymentNum.</summary>
		public static void AttachClaimPaymentToClaimProcs(List<ClaimProc> listClaimProcs, long claimPaymentNum) {
			for(int i=0;i<listClaimProcs.Count;i++) {
				listClaimProcs[i].ClaimPaymentNum=claimPaymentNum;
				ClaimProcs.Update(listClaimProcs[i]);
			}
		}
	}
}
