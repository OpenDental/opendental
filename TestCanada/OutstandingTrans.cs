using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using OpenDental.Eclaims;

namespace TestCanada {
	class OutstandingTrans {

		private static string Run(int scriptNum,bool version2,bool sendToItrans,Carrier carrier,out List <Etrans> etransRequests) { 
			string retVal="";
			Provider prov=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			etransRequests=CanadianOutput.GetOutstandingTransactions(clearinghouseClin,version2,sendToItrans,carrier,prov,false);
			retVal+="Outstanding Transactions#"+scriptNum.ToString()+" successful.\r\n";
			return retVal;
		}

		public static string RunOne() {
			List<Etrans> etransRequests;
			//Claim claim=Claims.GetClaim(ClaimTC.ClaimNums[0]);
			Carrier carrier=Carriers.GetByElectId("666666");
			carrier.CanadianEncryptionMethod=1;
			string retval=Run(1,false,false,carrier,out etransRequests);
			////EOB
			//etransRequests[0].PatNum=claim.PatNum;
			//etransRequests[0].PlanNum=claim.PlanNum;
			//etransRequests[0].InsSubNum=claim.InsSubNum;
			//string message=EtransMessageTexts.GetMessageText(etransRequests[0].EtransMessageTextNum);
			//FormCCDPrint FormP=new FormCCDPrint(etransRequests[0],message);//Print the form. 
			//FormP.Print();
			////Email
			//etransRequests[1].PatNum=claim.PatNum;
			//etransRequests[1].PlanNum=claim.PlanNum;
			//etransRequests[1].InsSubNum=claim.InsSubNum;
			//message=EtransMessageTexts.GetMessageText(etransRequests[1].EtransMessageTextNum);
			//FormP=new FormCCDPrint(etransRequests[1],message);//Print the form. 
			//FormP.Print();
			return retval;
		}

		public static string RunTwo() {
			List<Etrans> etransRequests;
			//Claim claim=Claims.GetClaim(ClaimTC.ClaimNums[6]);
			Carrier carrier=Carriers.GetByElectId("888888");
			string retval=Run(2,false,false,carrier,out etransRequests);
			////EOB
			//etransRequests[0].PatNum=claim.PatNum;
			//etransRequests[0].PlanNum=claim.PlanNum;
			//etransRequests[0].InsSubNum=claim.InsSubNum;
			//string message=EtransMessageTexts.GetMessageText(etransRequests[0].EtransMessageTextNum);
			//FormCCDPrint FormP=new FormCCDPrint(etransRequests[0],message);//Print the form. 
			//FormP.Print();
			return retval;
		}

		public static string RunThree() {
			List<Etrans> etransRequests;
			//Claim claim=Claims.GetClaim(ClaimTC.ClaimNums[]);
			Carrier carrier=Carriers.GetByElectId("777777");
			string retval=Run(3,false,false,carrier,out etransRequests);
			////EOB
			//etransRequests[0].PatNum=claim.PatNum;
			//etransRequests[0].PlanNum=claim.PlanNum;
			//etransRequests[0].InsSubNum=claim.InsSubNum;
			//string message=EtransMessageTexts.GetMessageText(etransRequests[0].EtransMessageTextNum);
			//FormCCDPrint FormP=new FormCCDPrint(etransRequests[0],message);//Print the form. 
			//FormP.Print();
			return retval;
		}

	}
}
