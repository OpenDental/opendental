using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Introspection {
		///<summary>The dictionary of testing overrides.  This variable should ONLY be used from within the DictOverrides property.</summary>
		private static Dictionary<IntrospectionEntity,string> _dictOverrides;

		///<summary>Fills _dictOverrides when it is null and returns it.  Will always return null if the IntrospectionItems preference is not present in the database.
		///This getter will check the preference cache for the aforementioned preference until it finds it.
		///Once found, _dictOverrides will be instantiated and filled with the contents of the preference.  Once instatiated, this getter will never repopulate the dictionary.
		///If the preference is present in the database but is malformed JSON, _dictOverrides will be an empty dictionary which will throw exceptions later on in the program.</summary>
		private static Dictionary<IntrospectionEntity,string> DictOverrides {
			get {
				if(_dictOverrides!=null || !Prefs.GetContainsKey(nameof(PrefName.IntrospectionItems))) {
					return _dictOverrides;
				}
				//Try to extract the introspection overrides from the preference. 
				try {
					string introspectionItems=PrefC.GetString(PrefName.IntrospectionItems);//Cache call so it is fine to do this a lot.  Purposefully throws exceptions.
					//Catch deserialization errors and skip any invalid keys
					JsonSerializerSettings settings=new JsonSerializerSettings { Error=HandleDeserializationError };
					//At this point we know the database has the IntrospectionItems preference so we need to instantiate _dictOverrides.
					_dictOverrides=JsonConvert.DeserializeObject<Dictionary<IntrospectionEntity,string>>(introspectionItems,settings);
					//If the pref is there but contained invalid or no data, create a new, empty dictionary
					if(_dictOverrides==null) {
						_dictOverrides=new Dictionary<IntrospectionEntity,string>();
					}
				}
				catch(Exception ex) {
					throw new ApplicationException("Error encountered while deserializing introspection JSON. \r\nError: "+ex.Message);
				}
				return _dictOverrides;
			}
		}

		///<summary>When deserializing, don't crash if there's an invalid key in the db; just skip over it instead.</summary>
		public static void HandleDeserializationError(object sender,Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs) {
			var currentError = errorArgs.ErrorContext.Error.Message;
			if(currentError.StartsWith("Could not convert string") && currentError.Contains("to dictionary key type")) {
				//Skip the invalid key and continue deserializing	
				errorArgs.ErrorContext.Handled=true;
			}
		}

		///<summary>Returns true if the IntrospectionItems preference is present within the preference cache.  Otherwise; false.</summary>
		public static bool IsTestingMode {
			get {
				return (DictOverrides!=null);
			}
		}

		public static bool SetOverride(IntrospectionEntity entity,string overrideValue) {
			bool changed=false;
			if(DictOverrides!=null) {
				//If key is not in dict, add it
				if(!DictOverrides.ContainsKey(entity)) {
					DictOverrides.Add(entity,overrideValue);
				}
				else {
					DictOverrides[entity]=overrideValue;
				}
				string serializedDict=JsonConvert.SerializeObject(DictOverrides,Formatting.Indented);
				changed=Prefs.UpdateString(PrefName.IntrospectionItems,serializedDict);
				if(changed) {
					Signalods.SetInvalid(InvalidType.Prefs);
				}
			}
			return changed;
		}

		public static bool TryGetOverride(IntrospectionEntity entity,out string overrideValue) {
			if(DictOverrides!=null && DictOverrides.TryGetValue(entity,out overrideValue)) {
				return true;
			}
			overrideValue="";
			return false;
		}

		///<summary>Returns the defaultValue passed in if the entity cannot be found in the global dictionary of testing overrides.
		///Purposefully throws an exception (not meant to be caught) if the IntrospectionItems preference is present in the database (should be missing in general)
		///and does not contain the entity that was passed in.  This will mean that the preference is malformed or is out of date and the preference value needs to be updated.</summary>
		public static string GetOverride(IntrospectionEntity entity,string defaultValue="") {
			if(DictOverrides!=null) {
				//DictOverrides was not null so we can assume it has the IntrospectionEntity passed in.
				//If the dictionary does not have the entity passed in then we will purposefully throw an exception tailored for engineers.
				string overrideStr;
				if(!DictOverrides.TryGetValue(entity,out overrideStr)) {
					throw new ApplicationException("Testing mode is on and the following introspection entity is not present in the IntrospectionItems preference: "+entity.ToString());
				}
				return overrideStr;
			}
			//The database does not have the IntrospectionItems preference so return defaultValue.
			return defaultValue;
		}

		///<summary>Only used for unit tests. SHOULD NOT be used otherwise.</summary>
		public static void ClearDictOverrides() {
			//This wouldn't be the end of the world if a non-unit test class does this because it just causes the dictionary to automatically refresh itself later.
			_dictOverrides=null;
		}

		///<summary>Holds 3rd party API information.</summary>
		public enum IntrospectionEntity {
			///<summary></summary>
			DentalXChangeDwsURL,
			///<summary></summary>
			DentalXChangeDeaURL,
			///<summary></summary>
			DoseSpotURL,
			///<summary></summary>
			PayConnectRestURL,
			///<summary></summary>
			PDMPTestUrlIL,
			///<summary></summary>
			PaySimpleApiURL,
			///<summary></summary>
			DoseSpotSingleSignOnURL,
			///<summary></summary>
			PayConnectWebServiceURL,
			///<summary></summary>
			CareCreditApiURL,
			///<summary></summary>
			NewCropRxEntryURL,
			///<summary></summary>
			PDMPTestUserIL,
			///<summary></summary>
			PDMPTestPasswordIL,
			///<summary></summary>
			PDMPTestFacilityIdIL,
			///<summary></summary>
			PDMPTestUserCA,
			///<summary></summary>
			PDMPTestPasswordCA,
			///<summary></summary>
			PDMPTestFacilityIdCA,
			///<summary></summary>
			PDMPTestUrlCA,
			///<summary></summary>
			PDMPTestUserUT,
			///<summary></summary>
			PDMPTestPasswordUT,
			///<summary></summary>
			PDMPTestFacilityIdUT,
			///<summary></summary>
			PDMPTestUrlUT,
			///<summary></summary>
			PDMPTestUserMD,
			///<summary></summary>
			PDMPTestPasswordMD,
			///<summary></summary>
			PDMPTestFacilityIdMD,
			///<summary></summary>
			PDMPTestUrlMD,
			///<summary></summary>
			PDMPTestUserKY,
			///<summary></summary>
			PDMPTestPasswordKY,
			///<summary></summary>
			PDMPTestFacilityIdKY,
			///<summary></summary>
			PDMPTestUrlKY,
			///<summary></summary>
			PDMPTestUserWA,
			///<summary></summary>
			PDMPTestPasswordWA,
			///<summary></summary>
			PDMPTestFacilityIdWA,
			///<summary></summary>
			PDMPTestUrlWA,
			///<summary></summary>
			ApprissTestUser,
			///<summary></summary>
			ApprissTestPassword,
			///<summary></summary>
			ApprissTestUrl,
			///<summary></summary>
			ApprissClientKey,
			///<summary></summary>
			ApprissClientPassword,
			///<summary></summary>
			QuickBooksOnlineSandboxUrl,
			///<summary></summary>
			QuickBooksOnlineSandboxEnvironment,
			///<summary></summary>
			QuickBooksOnlineSandboxClientIdAndSecret,
			///<summary></summary>
			EdgeExpressHostPay,
			///<summary></summary>
			EdgeExpressDirectPay,
		}
	}
}

/*
 INSERT INTO preference (PrefName,ValueString) VALUES('IntrospectionItems','');
*/
