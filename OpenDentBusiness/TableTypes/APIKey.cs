using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Used to keep track of Customer's API Key and Developer's name. Just a copy from OD HQ for convenience.</summary>
	[Serializable]
	public class APIKey:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn (IsPriKey=true)]
		public long APIKeyNum;
		///<summary>Customer's API key.</summary>
		public string CustApiKey;
		///<summary>Developer's name, exactly as they entered it in FHIR developer portal.</summary>
		public string DevName;
	}

	///<summary>A customer key and info about it and the developer. Provided by ODHQ.</summary>
	public class ApiKeyVisibleInfo {
		///<summary>Just the customer key. Does not include developer key.</summary>
		public string CustomerKey;
		///<summary>The status of the key.</summary>
		public FHIRKeyStatus FHIRKeyStatusCur;
		///<summary>The permissions that an APIKey possesses.</summary>
		public List<APIPermission> ListAPIPermissions=new List<APIPermission>();
		///<summary>The name of the developer that owns this key.</summary>
		public string DeveloperName;
		///<summary>The email of the developer that owns this key.</summary>
		public string DeveloperEmail;
		///<summary>The phone number of the developer that owns this key.</summary>
		public string DeveloperPhone;
		///<summary>The phone number of the developer that owns this key.</summary>
		public DateTime DateDisabled;
		///<summary>FK to fhirdeveloper.FHIRDeveloperNum. This table only exists in apihq database.</summary>
		public long FHIRDeveloperNum;
		///<summary>FK to fhirapikey.FHIRAPIKeyNum. This table only exists in apihq database.</summary>
		public long FHIRAPIKeyNum;

		public ApiKeyVisibleInfo Copy() {
			ApiKeyVisibleInfo apiKeyVisibleInfo=(ApiKeyVisibleInfo)MemberwiseClone();
			apiKeyVisibleInfo.ListAPIPermissions=ListAPIPermissions.ToList();
			return apiKeyVisibleInfo;
		}
	}

}
