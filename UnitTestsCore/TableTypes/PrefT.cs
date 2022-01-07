using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	///<summary>All update methods will save which changes have been made so that they can be undone by RevertPrefChanges().</summary>
	public class PrefT {
		///<summary>Key: Pref.PrefName, Value: Pref.ValueString</summary>
		private static Dictionary<string,string> _dictPrefsOrig=new Dictionary<string, string>();

		private static void AddToDict(PrefName prefName,string newValue) {
			AddToDict(prefName.ToString(),newValue);
		}

		private static void AddToDict(string prefName,string newValue) {
			string oldValue=Prefs.GetPref(prefName.ToString()).ValueString;
			if(oldValue==newValue || _dictPrefsOrig.ContainsKey(prefName)) {
				return;
			}
			_dictPrefsOrig[prefName]=oldValue;
		}

		///<summary>Deletes the preference from the database and from the local unit test pref dictionary.  Also, causes pref cache refresh.
		///This is useful (and necessary) when testing helper methods within the convert script.</summary>
		public static void Delete(string prefName) {
			//Remove the preference from the local dictionary.
			_dictPrefsOrig.Remove(prefName);
			//Manaully delete the preference from the database here within the T class and not out in an S class.
			//Deleting preferences should never be a thing in the real world so we don't want to create a method that easily does so.
			DataCore.NonQ($"DELETE FROM preference WHERE PrefName='{POut.String(prefName)}'");
			//Refresh the local cache so that the preference is completely "removed", even from memory.
			Prefs.RefreshCache();
		}

		///<summary>Resets the preferences to what they were before any update methods in this class were called.</summary>
		public static void RevertPrefChanges() {
			foreach(string prefName in _dictPrefsOrig.Keys) {
				Prefs.UpdateRaw(prefName,_dictPrefsOrig[prefName]);
			}
			_dictPrefsOrig.Clear();
		}

		///<summary>Updates a pref of type int.  Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateInt(PrefName prefName,int newValue) {
			return UpdateInt(prefName.ToString(),newValue);
		}

		///<summary>Updates a pref of type int.  Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateInt(string prefName,int newValue) {
			return UpdateLong(prefName,newValue);
		}

		///<summary>Updates a pref of type byte.  Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateByte(PrefName prefName,byte newValue) {
			return UpdateLong(prefName,newValue);
		}

		///<summary>Updates a pref of type long.  Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateLong(PrefName prefName,long newValue) {
			return UpdateLong(prefName.ToString(),newValue);
		}

		///<summary>Updates a pref of type long.  Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateLong(string prefName,long newValue) {
			AddToDict(prefName,POut.Long(newValue));
			return Prefs.UpdateRaw(prefName,POut.Long(newValue));
		}

		///<summary>Updates a pref of type double.  Returns true if a change was required, or false if no change needed.
		///Set doRounding false when the double passed in needs to be Multiple Precision Floating-Point Reliable (MPFR).</summary>
		public static bool UpdateDouble(PrefName prefName,double newValue,bool doRounding=true) {
			AddToDict(prefName,POut.Double(newValue));
			return Prefs.UpdateDouble(prefName,newValue,doRounding);
		}

		///<summary>Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateBool(PrefName prefName,bool newValue) {
			AddToDict(prefName,POut.Bool(newValue));
			return Prefs.UpdateBool(prefName,newValue);
		}

		///<summary>Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateString(PrefName prefName,string newValue) {
			AddToDict(prefName,newValue);
			return Prefs.UpdateString(prefName,newValue);
		}

		///<summary>Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateDateT(PrefName prefName,DateTime newValue) {
			AddToDict(prefName,POut.DateT(newValue));
			return Prefs.UpdateDateT(prefName,newValue);
		}

		///<summary>Updates a pref of type YN.  Returns true if a change was required, or false if no change needed.</summary>
		public static bool UpdateYN(PrefName prefName,YN newValue) {
			return UpdateLong(prefName,(int)newValue);
		}
	}
}
