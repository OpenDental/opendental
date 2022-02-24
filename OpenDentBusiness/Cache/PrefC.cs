using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenDentBusiness;
using CodeBase;
using System.IO;
using System.Net;

namespace OpenDentBusiness {
	public class PrefC {
		private static bool _isTreatPlanSortByTooth;
		private static YN _isVerboseLoggingSession;
	
		///<summary>This property is just a shortcut to this pref to make typing faster.  This pref is used a lot.</summary>
		public static bool RandomKeys {
			get {
				return GetBool(PrefName.RandomPrimaryKeys);
			}
		}

		///<summary>Logical shortcut to the ClaimPaymentNoShowZeroDate pref.  Returns 0001-01-01 if pref is disabled.</summary>
		public static DateTime DateClaimReceivedAfter {
			get {
				DateTime date=DateTime.MinValue;
				int days=GetInt(PrefName.ClaimPaymentNoShowZeroDate);
				if(days>=0) {
					date=DateTime.Today.AddDays(-days);
				}
				return date;
			}
		}

		///<summary>This property is just a shortcut to this pref to make typing faster.  This pref is used a lot.</summary>
		public static DataStorageType AtoZfolderUsed {
			get {
				return PIn.Enum<DataStorageType>(GetInt(PrefName.AtoZfolderUsed));
			}
		}

		///<summary>This property returns true if the preference for clinics is on and there is at least one non-hidden clinic.</summary>
		public static bool HasClinicsEnabled {
			get {
				//If changing this, also change Prefs.HasClinicsEnabledNoCache.
				return (!GetBool(PrefName.EasyNoClinics) && Clinics.GetCount(true)>0);
			}
		}

		///<summary>Returns true if DockPhonePanelShow is enabled. Convenience function that should be used if for ODHQ only, and not resellers.</summary>
		/// <returns></returns>
		public static bool IsODHQ {
			get {
				return GetBool(PrefName.DockPhonePanelShow);
			}
		}

		///<summary>Returns a list of DefNums that represent WSEP Generally Allowed blockout types.</summary>
		public static List<long> GetWebSchedExistingPatAllowedBlockouts {
			get {
				string prefString=PrefC.GetString(PrefName.WebSchedExistingPatIgnoreBlockoutTypes);
				if(prefString.IsNullOrEmpty()) {
					return new List<long>();
				}
				return prefString.Split(',').Select(long.Parse).ToList();
			}
		}

		///<summary>Returns a list of DefNums that represent WSNPA Generally Allowed blockout types.</summary>
		public static List<long> GetWebSchedNewPatAllowedBlockouts {
			get {
				string prefString=PrefC.GetString(PrefName.WebSchedNewPatApptIgnoreBlockoutTypes);
				if(prefString.IsNullOrEmpty()) {
					return new List<long>();
				}
				return prefString.Split(',').Select(long.Parse).ToList();
			}
		}

		///<summary>Returns a list of DefNums that represent Web Sched Recall Generally Allowed blockout types.</summary>
		public static List<long> GetWebSchedRecallAllowedBlockouts {
			get {
				string prefString=PrefC.GetString(PrefName.WebSchedRecallIgnoreBlockoutTypes);
				if(prefString.IsNullOrEmpty()) {
					return new List<long>();
				}
				return prefString.Split(',').Select(long.Parse).ToList();
			}
		}
		///<summary>True if the computer name of this session is included in the HasVerboseLogging PrefValue.</summary>
		public static bool IsVerboseLoggingSession() {			
			try {
				if(Prefs.DictIsNull()) { //Do not allow PrefC.GetString below if we haven't loaded the Pref cache yet. This would cause a recursive loop and stack overflow.
					throw new Exception("Prefs cache is null");
				}
				if(_isVerboseLoggingSession==YN.Unknown) {
					if(PrefC.GetString(PrefName.HasVerboseLogging).ToLower().Split(",",StringSplitOptions.RemoveEmptyEntries).ToList()
						.Exists(x => x==ODEnvironment.MachineName.ToLower())) 
					{
						_isVerboseLoggingSession=YN.Yes;
						//Switch logger to a directory that won't have permissions issues.
						Logger.UseMyDocsDirectory();
					}
					else {
						_isVerboseLoggingSession=YN.No;
					}
				}
				return _isVerboseLoggingSession==YN.Yes;
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}			
		}

		///<summary>Returns the credentials (user name and password) used to access the voicemail share via SMB2.
		///Used by HQ only.  These preference will not be present in typical databases and this property will then throw an exception.</summary>
		public static NetworkCredential VoiceMailNetworkCredentialsSMB2 {
			get {
				return new NetworkCredential(PrefC.GetString(PrefName.VoiceMailSMB2UserName),PrefC.GetString(PrefName.VoiceMailSMB2Password));
			}
		}

		///<summary>Call this when we have a new Pref cache in order to re-establish logging preference from this computer.</summary>
		public static void InvalidateVerboseLogging() {
			_isVerboseLoggingSession=YN.Unknown;
		}

		///<summary>True if the practice has set a window to restrict the times that automatic communications will be sent out.</summary>
		public static bool DoRestrictAutoSendWindow {
			get {
				//Setting the auto start window equal to the auto stop window is how the restriction is removed.
				return GetDateT(PrefName.AutomaticCommunicationTimeStart).TimeOfDay!=GetDateT(PrefName.AutomaticCommunicationTimeEnd).TimeOfDay;
			}
		}

		///<summary>Returns a valid DateFormat for patient communications.
		///If the current preference is invalid, returns "d" which is equivalent to .ToShortDateString()</summary>
		public static string PatientCommunicationDateFormat {
			get {
				string format=GetString(PrefName.PatientCommunicationDateFormat);
				try {
					DateTime.Today.ToString(format);
				}
				catch(Exception ex) {
					ex.DoNothing();
					format="d";//Default to "d" which is equivalent to .ToShortDateString()
				}
				return format;
			}
		}

		///<summary>Gets a pref of type long.</summary>
		public static long GetLong(PrefName prefName) {
			return PIn.Long(Prefs.GetOne(prefName).ValueString);
		}

		///<summary>Gets a pref of type int32.  Used when the pref is an enumeration, itemorder, etc.  Also used for historical queries in ConvertDatabase.</summary>
		public static int GetInt(PrefName prefName) {
			return PIn.Int(Prefs.GetOne(prefName).ValueString);
		}

		///<summary>Gets a pref of type byte.  Used when the pref is a very small integer (0-255).</summary>
		public static byte GetByte(PrefName prefName) {
			return PIn.Byte(Prefs.GetOne(prefName).ValueString);
		}

		///<summary>Gets a pref of type double.</summary>
		public static double GetDouble(PrefName prefName) {
			return PIn.Double(Prefs.GetOne(prefName).ValueString);
		}
		
		///<summary>Gets a pref of type double.</summary>
		public static double GetDouble(PrefName prefName,bool doUseEnUSFormat) {
			return PIn.Double(Prefs.GetOne(prefName).ValueString,doUseEnUSFormat);
		}

		///<summary>Gets a pref of type bool.</summary>
		public static bool GetBool(PrefName prefName) {
			return PIn.Bool(Prefs.GetOne(prefName).ValueString);
		}
		
		///<summary>Gets the bool value for a YN pref.  If Unknown, then returns the default.  If you want the 3 state version, then use PrefC.GetEnum&lt;YN&gt; or PrefC.GetCheckState.</summary>
		public static bool GetYN(PrefName prefName) {
			YN yn=(YN)PIn.Int(Prefs.GetOne(prefName).ValueString);
			if(yn==YN.Yes) {
				return true;
			}
			if(yn==YN.No) {
				return false;
			}
			//unknown, so use the default
			PrefValueType prefValueType=prefName.GetValueType();
			if(prefValueType==PrefValueType.YN_DEFAULT_FALSE){
				return false;
			}
			if(prefValueType==PrefValueType.YN_DEFAULT_TRUE){
				return true;
			}
			throw new ArgumentException("Invalid type");
		}

		///<summary>Gets YN value for use in pref setup windows with a 3 state checkbox.</summary>
		public static System.Windows.Forms.CheckState GetYNCheckState(PrefName prefName){
			YN yn=(YN)PIn.Int(Prefs.GetOne(prefName).ValueString);
			if(yn==YN.Yes) {
				return System.Windows.Forms.CheckState.Checked;
			}
			if(yn==YN.No) {
				return System.Windows.Forms.CheckState.Unchecked;
			}
			return System.Windows.Forms.CheckState.Indeterminate;
		}

		///<summary>Gets a pref of the specified enum type.</summary>
		public static T GetEnum<T>(PrefName prefName) where T:struct,Enum {
			return PIn.Enum<T>(GetInt(prefName));
		}

		///<Summary>Gets a pref of type bool, but will not throw an exception if null or not found.  Indicate whether the silent default is true or false.</Summary>
		public static bool GetBoolSilent(PrefName prefName,bool silentDefault) {
			if(Prefs.DictIsNull()) {
				return silentDefault;
			}
			Pref pref=null;
			ODException.SwallowAnyException(() => {
				pref=Prefs.GetOne(prefName);
			});
			return (pref==null ? silentDefault : PIn.Bool(pref.ValueString));
		}

		///<summary>Gets a pref of type string.</summary>
		public static string GetString(PrefName prefName) {
			return Prefs.GetOne(prefName).ValueString;
		}

		///<summary>Gets a pref of type string without using the cache.</summary>
		public static string GetStringNoCache(PrefName prefName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),prefName);
			}
			string command="SELECT ValueString FROM preference WHERE PrefName='"+POut.String(prefName.ToString())+"'";
			return Db.GetScalar(command);
		}

		///<summary>Gets a pref of type string.  Will not throw an exception if null or not found.</summary>
		public static string GetStringSilent(PrefName prefName) {
			if(Prefs.DictIsNull()) {
				return "";
			}
			Pref pref=null;
			ODException.SwallowAnyException(() => {
				pref=Prefs.GetOne(prefName);
			});
			return (pref==null ? "" : pref.ValueString);
		}

		///<summary>Gets a pref of type date.</summary>
		public static DateTime GetDate(PrefName prefName) {
			return PIn.Date(Prefs.GetOne(prefName).ValueString);
		}

		///<summary>Gets a pref of type datetime.</summary>
		public static DateTime GetDateT(PrefName prefName) {
			return PIn.DateT(Prefs.GetOne(prefName).ValueString);
		}

		///<summary>Gets a color from an int32 pref.</summary>
		public static Color GetColor(PrefName prefName) {
			return Color.FromArgb(PIn.Int(Prefs.GetOne(prefName).ValueString));
		}

		///<summary>Used sometimes for prefs that are not part of the enum, especially for outside developers.</summary>
		public static string GetRaw(string prefName) {
			return Prefs.GetOne(prefName).ValueString;
		}

		///<summary>Gets culture info from DB if possible, if not returns current culture.</summary>
		public static CultureInfo GetLanguageAndRegion() {
			CultureInfo cultureInfo=CultureInfo.CurrentCulture;
			ODException.SwallowAnyException(() => {
				Pref pref=Prefs.GetOne("LanguageAndRegion");
				if(!string.IsNullOrEmpty(pref.ValueString)) {
					cultureInfo=CultureInfo.GetCultureInfo(pref.ValueString);
				}
			});
			return cultureInfo;
		}

		///<summary>Returns true if either the XCharge program or PayConnect program is enabled and at least one clinic has online payments enabled.
		///progEnabledForPayments will return the program that is enabled for online payments if it is allowed.  Both programs cannot be enabled at the same time</summary>
		public static bool HasOnlinePaymentEnabled(out ProgramName progEnabledForPayments) {
			progEnabledForPayments=ProgramName.None;
			Program progXCharge=Programs.GetCur(ProgramName.Xcharge);
			Program progEdgeExpress=Programs.GetCur(ProgramName.EdgeExpress);
			Program progPayConnect=Programs.GetCur(ProgramName.PayConnect);
			if(!progXCharge.Enabled && !progPayConnect.Enabled && !progEdgeExpress.Enabled) {
				return false;
			}
			if(progEdgeExpress.Enabled) {
				List<ProgramProperty> listXChargeProps=ProgramProperties.GetForProgram(progEdgeExpress.ProgramNum);
				if(listXChargeProps.Any(x => x.PropertyDesc==ProgramProperties.PropertyDescs.EdgeExpress.IsOnlinePaymentsEnabled && x.PropertyValue=="1")) {
					progEnabledForPayments=ProgramName.EdgeExpress;
					return true;
				}
			}
			else if(progXCharge.Enabled) {
				List<ProgramProperty> listXChargeProps=ProgramProperties.GetForProgram(progXCharge.ProgramNum);
				if(listXChargeProps.Any(x => x.PropertyDesc=="IsOnlinePaymentsEnabled" && x.PropertyValue=="1")) {
					progEnabledForPayments=ProgramName.Xcharge;
					return true;
				}
			}
			if(progPayConnect.Enabled) {
				List<ProgramProperty> listPayConnectProps=ProgramProperties.GetForProgram(progPayConnect.ProgramNum);
				if(listPayConnectProps.Any(x => x.PropertyDesc==PayConnect.ProgramProperties.PatientPortalPaymentsEnabled && x.PropertyValue=="1")) {
					progEnabledForPayments=ProgramName.PayConnect;
					return true;
				}
			}
			return false;
		}

		///<summary>Used by an outside developer.</summary>
		public static bool ContainsKey(string prefName) {
			return Prefs.GetContainsKey(prefName);
		}

		///<summary>Used by an outside developer.</summary>
		public static bool HListIsNull() {
			return Prefs.DictIsNull();
		}
		
		///<summary>Static variable used to always reflect FormOpenDental.IsTreatPlanSortByTooth.  
		///This setter should only be called in FormOpenDental.IsTreatPlanSortByTooth.  
		///This getter should only be called from the Client side when used with MiddleTier.</summary>
		public static bool IsTreatPlanSortByTooth {
			get {
				return _isTreatPlanSortByTooth;
			}
			set {
				_isTreatPlanSortByTooth=value;
			}
		}

		///<summary>Returns the path to the temporary opendental directory, temp/opendental.  Also performs one-time cleanup, if necessary.  In FormOpenDental_FormClosing, the contents of temp/opendental get cleaned up.</summary>
		public static string GetTempFolderPath() {
			//Will clean up entire temp folder for a month after the enhancement of temp file cleanups as long as the temp\opendental folder doesn't already exist.
			string tempPathOD=ODFileUtils.CombinePaths(Path.GetTempPath(),"opendental");
			if(Directory.Exists(tempPathOD)) {
				//Cleanup has already run for the old temp folder.  Do nothing.
				return tempPathOD;
			}
			Directory.CreateDirectory(tempPathOD);
			if(DateTime.Today>PrefC.GetDate(PrefName.TempFolderDateFirstCleaned).AddMonths(1)) {
				return tempPathOD;
			}
			//This might be used if this is the first time running this version on the computer that did the db update.
			//This might also be used if this is a computer that was turned off for a few weeks around the time of update conversion.
			//We need some sort of time limit just in case it's annoying and keeps happening.
			//So this will have a small risk of missing a computer, but the benefit of limiting outweighs the risk.
			//Empty entire temp folder.  Blank folders will be left behind because they do not matter.
			string[] arrayFileNames=Directory.GetFiles(Path.GetTempPath());
			for(int i=0;i<arrayFileNames.Length;i++) {
				try {
					if(arrayFileNames[i].Substring(arrayFileNames[i].LastIndexOf('.'))==".exe" || arrayFileNames[i].Substring(arrayFileNames[i].LastIndexOf('.'))==".cs") {
						//Do nothing.  We don't care about .exe or .cs files and don't want to interrupt other programs' files.
					}
					else {
						File.Delete(arrayFileNames[i]);
					}
				}
				catch {
					//Do nothing because the file could have been in use or there were not sufficient permissions.
					//This file will most likely get deleted next time a temp file is created.
				}
			}
			return tempPathOD;
		}

		///<summary>Creates a new randomly named file in the given directory path with the given extension and returns the full path to the new file.</summary>
		public static string GetRandomTempFile(string ext) {
			return ODFileUtils.CreateRandomFile(GetTempFolderPath(),ext);
		}

		///<summary>Throws exception.</summary>
		public static long GetDefaultSheetDefNum(SheetTypeEnum sheetType) {
			return GetLong(Prefs.GetSheetDefPref(sheetType));
		}

		///<summary>Using Pay Plan Version 2 (Aged Credits and Debits).</summary>
		public static bool IsPayPlanVersion2 {
			get {
				return PrefC.GetInt(PrefName.PayPlansVersion)==(int)PayPlanVersions.AgeCreditsAndDebits;
			}
		}

		///<summary>Returns true if the database hosted by Open Dental.</summary>
		public static bool IsCloudMode {
			get {
				return PrefC.GetInt(PrefName.DatabaseMode)==(int)DatabaseModeEnum.Cloud;
			}
		}

		///<summary>Returns true if the office has a report server set up.</summary>
		public static bool HasReportServer {
			get {
				return !string.IsNullOrEmpty(ReportingServer.Server) || !string.IsNullOrEmpty(ReportingServer.URI);
			}
		}

		///<summary>Returns the value (in minutes) of how long to wait prior to automatically logging the user off.
		///Runs a query if SecurityLogOffAllowUserOverride is true in order to get the log off time override for the current user.
		///Throws an exception if an invalid user override is found or an invalid global value is found (SecurityLogOffAfterMinutes).</summary>
		public static int LogOffTimer {
			get {
				if(Security.CurUser!=null && GetBool(PrefName.SecurityLogOffAllowUserOverride)) {
					UserOdPref userOverride=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.LogOffTimerOverride).FirstOrDefault();
					if(userOverride!=null) {
						if(!int.TryParse(userOverride.ValueString,out int logOffMins)) {
							throw new ODException($"Invalid LogOffTimerOverride set for user.\r\n"
								+$"UserNum: {Security.CurUser.UserNum}\r\n"
								+$"ValueString: {userOverride.ValueString}");
						}
						return logOffMins;
					}
				}
				return GetInt(PrefName.SecurityLogOffAfterMinutes);
			}
		}
		
		///<summary>A helper class to get Reporting Server preferences.</summary>
		public static class ReportingServer {
			public static string DisplayStr {
				get {
					if(Server=="") {
						if(URI!="") {
							return "Remote Server"; //will be blank if there is no reporting server set up.
						}
						else {
							return "";
						}
					}
					else {
						return Server+": "+Database;
					}
				}
			}
			public static string URI {
				get {
					return PrefC.GetString(PrefName.ReportingServerURI);
				}
			}
			public static string Server {
				get {
					return PrefC.GetString(PrefName.ReportingServerCompName);
				}
			}
			public static string Database {
				get {
					return PrefC.GetString(PrefName.ReportingServerDbName);
				}
			}
			public static string MySqlUser {
				get {
					return PrefC.GetString(PrefName.ReportingServerMySqlUser);
				}
			}
			public static string MySqlPass {
				get {
					string pass;
					CDT.Class1.Decrypt(PrefC.GetString(PrefName.ReportingServerMySqlPassHash),out pass);
					return pass;
				}
			}
			public static bool IsMiddleTier {
				get {
					return URI!="";
				}
			}
			public static string ConnectionString {
				get {
					return "";//Connection string is not currently supported for ReportingServers.
				}
			}
		}

	}
}
