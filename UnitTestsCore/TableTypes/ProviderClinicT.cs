using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ProviderClinicT {

		public static ProviderClinic CreateProviderClinic(Provider provider,long clinicNum=0,string dEANum="",string stateLicense="OR",string stateRxID="",
			string stateWhereLicensed="OR",string careCreditMerchNum="") {
			return new ProviderClinic() {
				ProvNum=provider.ProvNum,
				ClinicNum=clinicNum,
				DEANum=dEANum,
				StateLicense=stateLicense,
				StateRxID=stateRxID,
				StateWhereLicensed=stateWhereLicensed,
				CareCreditMerchantId=careCreditMerchNum,
			};
		}

		public static void ClearProviderClinicTable() {
			string command="DELETE FROM providerclinic WHERE ProviderClinicNum > 0";
			DataCore.NonQ(command);
		}

		public static void Update(ProviderClinic providerClinic) {
			List<ProviderClinic> curProvClinic=ProviderClinics.GetListForProvider(providerClinic.ProvNum);
			ProviderClinics.Sync(new List<ProviderClinic>{ providerClinic },curProvClinic);
		}
	}
}
