using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms.Crud;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_Logs {
		///<summary>Thread-safe dictionary of RegistrationKeyNums and if they should be making logs or not.</summary>
		private static ConcurrentDictionary<long,bool> _dictRegKeyNumIsLogging=new ConcurrentDictionary<long,bool>();
		///<summary>Thread-safe dictionary of PatNums and if they should be making logs or not. For backwards compatibility purposes.
		///Older versions of Open Dental used PatNum (DentalOfficeID) instead of RegistrationKeyNum.</summary>
		private static ConcurrentDictionary<long,bool> _dictPatNumIsLogging=new ConcurrentDictionary<long,bool>();
		///<summary>The global log level of this application or service is set here. This logger is only used when making logs to the hard drive.</summary>
		public static LogWriter LogWriter=new LogWriter(LogLevel.Error,"WebForms");
		///<summary>Do not use. Invoke GetServerName() instead.</summary>
		private static string _serverName=null;

		///<summary>The value from Environment.MachineName or UNKNOWN if there was an error getting that value.</summary>
		private static string GetServerName() {
			if(_serverName==null) {
				try {
					_serverName=Environment.MachineName;
				}
				catch(Exception) {
					_serverName="UNKNOWN";
				}
			}
			return _serverName;
		}

		private static string GetUserHostAddress() {
			string userHostAddress;
			try {
				//HAProxy will include a header 'X-Forwarded-For' that contains the client IP address.
				userHostAddress=HttpContext.Current.Request.Headers.GetValues("X-Forwarded-For")?.FirstOrDefault()??"";
				if(!string.IsNullOrEmpty(userHostAddress)) {
					return userHostAddress;
				}
				//If this request came via HAProxy, then Context.Request.UserHostAddress will be the address of the HAProxy server.
				userHostAddress=HttpContext.Current.Request.UserHostAddress;
			}
			catch(Exception) { 
				userHostAddress="UNKNOWN";
			}
			return userHostAddress;
		}

		private static string GetUserAgent() {
			string userAgent;
			try {
				userAgent=HttpContext.Current.Request.UserAgent;
			}
			catch(Exception) {
				userAgent="UNKNOWN";
			}
			return userAgent;
		}

		///<summary>Returns all logs for the dental office where the timestamp on the log is greater than or equal to the date and time passed in.</summary>
		public static List<WebForms_Log> GetForDentalOfficeID(long dentalOfficeID,DateTime dateTimeSince) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_Log>>(MethodBase.GetCurrentMethod(),dentalOfficeID,dateTimeSince);
			}
			//Grab the logs for the dental office passed in and then order them by most recent log towards the top.
			//Utilize the PK column as a tiebreaker for logs that were made within the same second.
			string command=$@"SELECT * FROM webforms_log 
				WHERE DentalOfficeID = {POut.Long(dentalOfficeID)}
				AND DateTStamp >= {POut.DateT(dateTimeSince)}
				ORDER BY DateTStamp DESC, LogNum DESC";
			return DataAction.GetT(() => WebForms_LogCrud.SelectMany(command),ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static List<WebForms_Log> GetForRegistrationKeyNum(long registrationKeyNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_Log>>(MethodBase.GetCurrentMethod(),registrationKeyNum);
			}
			return DataAction.GetT(() => {
				string command="SELECT * FROM webforms_log WHERE RegistrationKeyNum = "+POut.Long(registrationKeyNum);
				return WebForms_LogCrud.SelectMany(command);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static long Insert(WebForms_Log webForms_Log) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				webForms_Log.LogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),webForms_Log);
				return webForms_Log.LogNum;
			}
			return DataAction.GetT(() => WebForms_LogCrud.Insert(webForms_Log),ConnectionNames.WebForms);
		}

		///<summary>Returns true if logging is turned on for the dental office or globally via the LogLevelOfApplication set within the OpenDentalWebConfig.xml file.</summary>
		private static bool IsLoggingEnabled(long dentalOfficeID,long registrationKeyNum,LogLevel logLevel=LogLevel.Verbose) {
			//Check to see if the registration key passed in has logging enabled.
			//RegistrationKeyNum has more specificity and should be checked before DentalOfficeID (aka PatNum).
			if(_dictRegKeyNumIsLogging.TryGetValue(registrationKeyNum,out bool isRegKeyLogging) && isRegKeyLogging==true) {
				return true;
			}
			//Check to see if the dental office ID passed in has logging enabled.
			//PatNum should be checked for backwards compatibility.
			if(_dictRegKeyNumIsLogging.TryGetValue(dentalOfficeID,out bool isPatNumLogging) && isPatNumLogging==true) {
				return true;
			}
			//Check the global log level of this server (set within the config file).
			if(logLevel<=LogWriter.LogLevel) {
				return true;
			}
			//Do not insert if logLevel is higher than current log level of application.
			return false;
		}

		///<summary>Caches the IsLogging value for the dental office. Used to determine whether log entries should be made in the webforms_log table.</summary>
		public static void SetIsLogging(long dentalOfficeID,long registrationKeyNum,bool isLogging) {
			if(dentalOfficeID > 0) {
				if(_dictPatNumIsLogging.TryGetValue(dentalOfficeID,out bool isPatNumLogging)) {
					_dictPatNumIsLogging.TryUpdate(dentalOfficeID,isLogging,isPatNumLogging);
				}
				else {
					_dictPatNumIsLogging.TryAdd(dentalOfficeID,isLogging);
				}
			}
			if(registrationKeyNum > 0) {
				if(_dictRegKeyNumIsLogging.TryGetValue(registrationKeyNum,out bool isRegKeyNumLogging)) {
					_dictRegKeyNumIsLogging.TryUpdate(registrationKeyNum,isLogging,isRegKeyNumLogging);
				}
				else {
					_dictRegKeyNumIsLogging.TryAdd(registrationKeyNum,isLogging);
				}
			}
		}

		///<summary>Tries to make a log entry in the webforms_log table if logging is enabled for the customer or globally via the config file on this server.
		///Returns true if the log was successfully made to the database; Otherwise, false.</summary>
		public static bool TryInsertLog(long dentalOfficeID,long registrationKeyNum,string logMessage,List<long> listWebSheetDefIDs=null,LogLevel logLevel=LogLevel.Verbose) {
			//No remoting role check; no call to database.
			if(!IsLoggingEnabled(dentalOfficeID,registrationKeyNum,logLevel)) {
				return false;
			}
			try {
				if(listWebSheetDefIDs.IsNullOrEmpty()) {
					listWebSheetDefIDs=new List<long>();
				}
				WebForms_Log webForms_Log=new WebForms_Log() {
					DentalOfficeID=dentalOfficeID,
					LogMessage=logMessage,
					RegistrationKeyNum=registrationKeyNum,
					WebSheetDefIDs=string.Join(",",listWebSheetDefIDs),
					ServerName=GetServerName(),
					UserAgent=GetUserAgent(),
					UserHostAddress=GetUserHostAddress(),
				};
				Insert(webForms_Log);
			}
			catch(Exception ex) {//Never throw exceptions when trying to make log entries into the database.
				//Attempt to log the error to the hard drive so that HQ can be made aware of this problem.
				ODException.SwallowAnyException(() => LogWriter.WriteLine(MiscUtils.GetExceptionText(ex),LogLevel.Error));
				return false;
			}
			return true;
		}
	}
}
