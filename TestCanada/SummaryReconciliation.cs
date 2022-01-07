using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;
using OpenDental.Eclaims;

namespace TestCanada {
	class SummaryReconciliation {

		private static string Run(int scriptNum,Carrier carrier,CanadianNetwork network,Provider prov,out Etrans etrans,DateTime reconciliationDate) {
			string retVal="";
			Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			etrans=CanadianOutput.GetSummaryReconciliation(clearinghouseClin,carrier,network,prov,reconciliationDate);
			retVal+="Summary Reconciliation#"+scriptNum.ToString()+" successful.\r\n";
			return retVal;
		}

		public static string RunOne() {
			long carrierNum=CarrierTC.GetCarrierNumById("666666");
			Carrier carrier=Carriers.GetCarrier(carrierNum);
			Provider prov=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			Etrans etransAck;
			return Run(1,carrier,null,prov,out etransAck,new DateTime(1999,6,1));
		}

		public static string RunTwo() {
			CanadianNetwork network=new CanadianNetwork();
			network.Descript="Network 2";
			network.Abbrev="Network 2";
			network.CanadianNetworkNum=2;
			network.CanadianTransactionPrefix="A";
			Provider prov=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			Etrans etransAck;
			return Run(2,null,network,prov,out etransAck,new DateTime(1999,6,1));
		}

		public static string RunThree() {
			long carrierNum=CarrierTC.GetCarrierNumById("111555");
			Carrier carrier=Carriers.GetCarrier(carrierNum);
			Provider prov=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			Etrans etransAck;
			return Run(3,carrier,null,prov,out etransAck,new DateTime(1999,6,1));
		}

	}
}
