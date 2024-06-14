using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using WebServiceSerializer;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_Preferences {

		///<summary>Attempts to set preferences on web forms server using the currently saved connection url, or urlOverride if specified.</summary>
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

		#region WebHostSynch

		///<summary></summary>
		public static bool GetIsLoggingForDentalOfficeID(long dentalOfficeID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),dentalOfficeID);
			}
			string command=$"SELECT IsLogging FROM webforms_preference WHERE DentalOfficeID = {POut.Long(dentalOfficeID)}";
			DataTable table=DataAction.GetT(() => DataCore.GetTable(command),ConnectionNames.WebForms);
			if(table.Rows.Count==0) {
				return false;//The dental office doesn't have a row in the preference table yet and that is okay.
			}
			//All preference rows for the dental office share the same IsLogging value ATM (regardless if there are several rows per RegistrationKeyNum).
			return PIn.Bool(table.Rows[0]["IsLogging"].ToString());
		}

		///<summary></summary>
		public static List<WebForms_Preference> GetPrefsForOfficeId(long dentalOfficeID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_Preference>>(MethodBase.GetCurrentMethod(),dentalOfficeID);
			}
			return DataAction.GetT(() => {
				string command="SELECT * FROM webforms_preference "
					+"WHERE DentalOfficeID = "+POut.Long(dentalOfficeID);
				return WebForms_PreferenceCrud.SelectMany(command);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static List<WebForms_Preference> GetPrefsForRegistrationKeyNum(long registrationKeyNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_Preference>>(MethodBase.GetCurrentMethod(),registrationKeyNum);
			}
			return DataAction.GetT(() => {
				string command="SELECT * FROM webforms_preference "
					+"WHERE RegistrationKeyNum = "+POut.Long(registrationKeyNum);
				return WebForms_PreferenceCrud.SelectMany(command);
			},ConnectionNames.WebForms);
		}

		///<summary>Pass in a web forms preference object to insert into the database.  All fields must be considered before invoking this method.
		///Throws an exception if DentalOfficeID hasn't been set prior to invoking (PK which is technically a FK which is required at minimum).</summary>
		public static long Insert(WebForms_Preference webForms_Preference) {
			if(webForms_Preference.DentalOfficeID < 1) {
				throw new ApplicationException("Invalid DentalOfficeID for web forms preference.");
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				webForms_Preference.WebFormPrefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webForms_Preference);
				return webForms_Preference.WebFormPrefNum;
			}
			return DataAction.GetT(() => {
				return WebForms_PreferenceCrud.Insert(webForms_Preference,true);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static void Update(WebForms_Preference webForms_Preference) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webForms_Preference);
				return;
			}
			DataAction.Run(() => {
				WebForms_PreferenceCrud.Update(webForms_Preference);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static void UpdateIsLoggingForDentalOfficeID(long dentalOfficeID,bool isLogging) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dentalOfficeID,isLogging);
				return;
			}
			string command=$"UPDATE webforms_preference SET IsLogging = {POut.Bool(isLogging)} WHERE DentalOfficeID = {POut.Long(dentalOfficeID)}";
			DataAction.Run(() => DataCore.NonQ(command),ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static WebForms_Preference GetPreferences(long dentalOfficeID,long registrationKeyNum,string officeData) {
			List<WebForms_Preference> listWebFormPrefs=GetPrefsForRegistrationKeyNum(registrationKeyNum);
			if(listWebFormPrefs.Count>0) {
				//Found a match on the RegistraionKeyNum, there should only ever be 1 entry per RegistraionKeyNum.
				return listWebFormPrefs.First();
			}
			WebForms_Preference pref=null;
			//If there are no regkey specific prefs look at the dentalofficeID
			listWebFormPrefs=GetPrefsForOfficeId(dentalOfficeID);
			//Only look at prefs with a RegistraionKeyNum of 0 so we don't accidentally steal a different office's pref.
			if(listWebFormPrefs.Count()>0 && listWebFormPrefs.Exists(x => x.RegistrationKeyNum==0)) {
				//Clone our default pref with a RegistraionKeyNum of 0 into a new pref that has a RegistraionKeyNum set.
				WebForms_Preference prefDefault=listWebFormPrefs.Find(x => x.RegistrationKeyNum==0);
				pref=new WebForms_Preference();
				pref.DentalOfficeID=dentalOfficeID;
				pref.RegistrationKeyNum=registrationKeyNum;
				pref.ColorBorder=prefDefault.ColorBorder;
				pref.CultureName=prefDefault.CultureName;//empty string because null is not allowed
				pref.DisableSignatures=prefDefault.DisableSignatures;
				Insert(pref);
			}
			else {// if there is no 0 entry for that dental office with a RegistraionKeyNum of 0, make a new entry.
				pref=new WebForms_Preference();
				pref.DentalOfficeID=dentalOfficeID;
				pref.RegistrationKeyNum=registrationKeyNum;
				pref.ColorBorder=Color.FromArgb(-12550016);
				pref.CultureName="";//empty string because null is not allowed
				pref.DisableSignatures=false;
				Insert(pref);
			}
			return pref;
		}

		///<summary></summary>
		public static void SetPreferences(long dentalOfficeID,long registrationKeyNum,string officeData) {
			WebForms_Preference pref=WebSerializer.DeserializeTag<WebForms_Preference>(officeData,nameof(WebForms_Preference));
			List<WebForms_Preference> listPrefs=GetPrefsForRegistrationKeyNum(registrationKeyNum);//Should only ever be a single WebForms_Preference for any given RegKey.
			if(listPrefs.Count()==0) {//If there is not one look for a pref with a 0 regkey to maintain backwards compatibility 
				List<WebForms_Preference> listPrefsOfficeID=GetPrefsForOfficeId(dentalOfficeID);
				if(listPrefsOfficeID.Exists(x => x.RegistrationKeyNum==0)) {
					//Clone our default pref with a RegistraionKeyNum of 0 into a new pref that has a RegistraionKeyNum set.
					WebForms_Preference prefDefault=listPrefsOfficeID.Find(x => x.RegistrationKeyNum==0);
					pref.DentalOfficeID=dentalOfficeID;
					pref.RegistrationKeyNum=registrationKeyNum;
					pref.ColorBorder=pref.ColorBorder.ToArgb()!=0 ? pref.ColorBorder : prefDefault.ColorBorder;
					pref.CultureName=!pref.CultureName.IsNullOrEmpty() ? pref.CultureName : prefDefault.CultureName;//empty string because null is not allowed
					pref.DisableSignatures=prefDefault.DisableSignatures;
					pref.IsLogging=prefDefault.IsLogging;
					Insert(pref);
				}
				else {// if there is no 0 entry for that dental office with a RegistraionKeyNum of 0, make a new entry.
					pref.DentalOfficeID=dentalOfficeID;
					pref.RegistrationKeyNum=registrationKeyNum;
					pref.ColorBorder=pref.ColorBorder.ToArgb()!=0 ? pref.ColorBorder : Color.FromArgb(-12550016);
					pref.CultureName="";//empty string because null is not allowed
					pref.DisableSignatures=false;
					pref.IsLogging=false;
					Insert(pref);
				}
			}
			else {
				WebForms_Preference prefPersistent=listPrefs.First();
				pref.WebFormPrefNum=prefPersistent.WebFormPrefNum;
				pref.IsLogging=prefPersistent.IsLogging;
				pref.DentalOfficeID=dentalOfficeID;
				pref.RegistrationKeyNum=registrationKeyNum;
				Update(pref);
			}
		}

		#endregion

	}
}
