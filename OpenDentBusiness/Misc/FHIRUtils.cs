using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class FHIRUtils {
		///<summary>Converts a APIKeyStatus to a FHIRKeyStatus.</summary>
		public static FHIRKeyStatus ToFHIRKeyStatus(APIKeyStatus statusOld) {
			if(ListTools.In(statusOld,APIKeyStatus.ReadEnabled,APIKeyStatus.WritePending)) {
				return FHIRKeyStatus.EnabledReadOnly;
			}
			if(statusOld==APIKeyStatus.WriteEnabled) {
				return FHIRKeyStatus.Enabled;
			}
			return FHIRKeyStatus.DisabledByHQ;
		}

		///<summary>Converts a FHIRKeyStatus to a APIKeyStatus.</summary>
		public static APIKeyStatus ToAPIKeyStatus(FHIRKeyStatus status) {
			if(status==FHIRKeyStatus.Enabled) {
				return APIKeyStatus.WriteEnabled;
			}
			if(status==FHIRKeyStatus.EnabledReadOnly) {
				return APIKeyStatus.ReadEnabled;
			}
			return APIKeyStatus.Disabled;
		}
	}
}
