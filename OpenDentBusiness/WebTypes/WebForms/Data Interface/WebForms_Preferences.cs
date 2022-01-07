using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using WebServiceSerializer;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_Preferences {
		
		/// <summary>Attempts to set preferences on web forms server using the currently saved connection url, or urlOverride if specified.</summary>
		public static bool SetPreferences(WebForms_Preference pref,string regKey=null,string urlOverride=null) {
			bool retVal=false;
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				List<PayloadItem> listPayloadItems=new List<PayloadItem> {
					new PayloadItem(regKey,"RegKey"),
					new PayloadItem(pref,nameof(WebForms_Preference))
				};
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
				SheetsSynchProxy.UrlOverride=urlOverride;//SheetsSynchProxy.GetWebServiceInstance() gracefully handles null.
				retVal=WebSerializer.DeserializeTag<bool>(SheetsSynchProxy.GetWebServiceInstance().SetPreferences(payload),"Success");
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			return retVal;
		}

		/// <summary></summary>
		/// <param name="regKey"></param>
		/// <returns></returns>
		public static bool TryGetPreference(out WebForms_Preference pref,string regKey=null) {
			pref=new WebForms_Preference();
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,new PayloadItem(regKey,"RegKey"));
				pref=WebSerializer.DeserializeTag<WebForms_Preference>(SheetsSynchProxy.GetWebServiceInstance().GetPreferences(payload),"Success");
			}
			catch (Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}
	}
}
