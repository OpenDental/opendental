using System;
using CodeBase;

namespace OpenDentBusiness {
	public class WebChatMisc {
		public delegate void DbActionDelegate();
		///<summary>The WebClient site runs on a different server than CustomersHQ.  As such, the WebChatClient must be configured with a ConnectionStore, 
		///whereas HQ must use Pref.ConnectionSettingsHQ for data connections (this centralizes all HQ workstations to use a single set of data 
		///connection settings).</summary>
		public static bool UseConnectionStore { get; set; } = false;

		///<summary>Creates an ODThread so that we can safely change the database connection settings without affecting the calling method's connection.</summary>
		public static void DbAction(DbActionDelegate actionDelegate,bool isWebChatDb=true) {
			try {				
				if(isWebChatDb) {
					DataAction.RunWebChat(() => actionDelegate(),useConnectionStore:UseConnectionStore);
				}
				else {//Customers
					DataAction.RunCustomers(() => actionDelegate(),useConnectionStore:UseConnectionStore);
				}
			}
			catch(Exception ex) {
				LogException(ex);
			}
		}

		public static void LogException(Exception ex) {
			Logger.WriteException(ex,"WebChatLogs");
		}

		public static void LogError(string errorMsg) {
			Logger.WriteError(errorMsg,"WebChatLogs");
		}

	}
}