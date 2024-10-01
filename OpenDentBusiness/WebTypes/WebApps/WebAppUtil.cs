using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes.WebApps {
	public class WebAppUtil {

		public static string GetWebAppUrl(eServiceCode eserviceCode,long clinicNum=0) {
			switch(eserviceCode) {
				case eServiceCode.PaymentPortalUI:
					return BuildPaymentPortalUrl(clinicNum);
				default:
					throw new Exception("Cannot build URL for the specified eService code.");
			}
		}

		public static string BuildPaymentPortalUrl(long clinicNum=0) {
			string url="https://"+PrefC.GetString(PrefName.PaymentPortalSubDomain)+"."+PrefC.GetString(PrefName.WebAppDomain);
			if(clinicNum==0) {
				url+="?cid="+PrefC.GetString(PrefName.WebAppClinicGuid);
			}
			else {
				url+="?cid="+ClinicPrefs.GetPrefValue(PrefName.WebAppClinicGuid,clinicNum);
			}
			return url;
		}
	}
}
