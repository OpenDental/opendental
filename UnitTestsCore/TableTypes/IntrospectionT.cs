using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class IntrospectionT {
		///<summary>Removes the IntrospectionItems preference from the database.
		///Manually refreshes the preference cache so that the change instantly takes place.</summary>
		public static void DeletePref() {
			string command="DELETE FROM preference WHERE prefname='"+nameof(PrefName.IntrospectionItems)+"'";
			DataCore.NonQ(command);
			Prefs.RefreshCache();
			Introspection.ClearDictOverrides();
		}

		///<summary>Inserts the IntrospectionItems preference into the database if it is missing.  Runs an update statement if the pref already exists.
		///Always sets ValueString to the value passed in.  Manually refreshes the preference cache so that the change instantly takes place.</summary>
		public static void UpsertPref(Dictionary<Introspection.IntrospectionEntity,string> dictOverrides) {
			UpsertPref(JsonConvert.SerializeObject(dictOverrides));
		}

		///<summary>Inserts the IntrospectionItems preference into the database if it is missing.  Runs an update statement if the pref already exists.
		///Always sets ValueString to the value passed in.  Manually refreshes the preference cache so that the change instantly takes place.</summary>
		public static void UpsertPref(string valueString="") {
			string command="SELECT COUNT(*) FROM preference WHERE PrefName='IntrospectionItems'";
			if(DataCore.GetScalar(command)=="0") {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('IntrospectionItems','"+POut.String(valueString)+"')";
				DataCore.NonQ(command);
			}
			else {
				command="UPDATE preference SET ValueString='"+POut.String(valueString)+"' WHERE PrefName='IntrospectionItems'";
				DataCore.NonQ(command);
			}
			Prefs.RefreshCache();
			Introspection.ClearDictOverrides();
		}
	}
}
