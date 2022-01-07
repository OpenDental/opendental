namespace DataConnectionBase {
	///<summary>Copy of OpenDentBusiness.CentralConnection.</summary>
	public class CentralConnectionBase {
		///<summary>Primary key.</summary>
		public long CentralConnectionNum;
		///<summary>If direct db connection.  Can be ip address.</summary>
		public string ServerName;
		///<summary>If direct db connection.</summary>
		public string DatabaseName;
		///<summary>If direct db connection.</summary>
		public string MySqlUser;
		///<summary>If direct db connection.  Symmetrically encrypted.</summary>
		public string MySqlPassword;
		///<summary>If connecting to the web service. Can be on VPN, or can be over https.</summary>
		public string ServiceURI;
		///<summary>Deprecated.  If connecting to the web service.</summary>
		public string OdUser;
		///<summary>Deprecated.  If connecting to the web service.  Symmetrically encrypted.</summary>
		public string OdPassword;
		///<summary>When being used by ConnectionStore xml file, must deserialize to a ConnectionNames enum value. Otherwise just used as a generic notes field.</summary>
		public string Note;
		///<summary>0-based.</summary>
		public int ItemOrder;
		///<summary>If set to true, the password hash is calculated differently.</summary>
		public bool WebServiceIsEcw;
		///<summary>Contains the most recent information about this connection.  OK if no problems, version information if version mismatch, nothing for not checked, and OFFLINE if previously couldn't connect.</summary>
		public string ConnectionStatus;
		///<summary>If set to True, display clinic breakdown in reports, else only show practice totals.</summary>
		public bool HasClinicBreakdownReports;
		///<summary>Set when reading from the config file. Not an actual DB column.</summary>
		public bool IsAutomaticLogin;
		///<summary>This is a helper variable used for Reports. If we want to start supporting connection string for the Reporting Server, we need to add this as a db column. This was needed for the scenario where a customer connected to OD using a connection string.</summary>
		public string ConnectionString;
		///<summary>Helper variable to keep track of the password hash that was passed in as a command line argument. This is necessary for automatically logging eCW users in when they are utilizing the Middle Tier.</summary>
		public string OdPassHash;

		public CentralConnectionBase() {
		}

		///<summary>Used in Jordan's Remote Management Tool.</summary>
		public CentralConnectionBase(string server,string database,string user,string password,ConnectionNames connectionName) : this() {
			ServerName=server;
			DatabaseName=database;
			MySqlUser=user;
			MySqlPassword=password;
			Note=connectionName.ToString();
		}
	}

	///<summary>List of typically available database names. Used in conjunction with ConnectionStore.GetConnection().</summary>
	public enum ConnectionNames {
		///<summary>No database connection.</summary>
		None,
		///<summary>HQ serviceshq.</summary>
		ServicesHQ,
		///<summary>HQ bugs.</summary>
		BugsHQ,
		///<summary>HQ customers. This may be filled from a config file or if the config file does not include it, it will be filled from the preference
		///table.</summary>
		CustomersHQ,
		///<summary>Only used when running in DEBUG demo mode. The OD proper database which holds demo data.</summary>
		DentalOffice,
		///<summary>The OD proper database connection that is comprised of preferences via the currently connected database.</summary>
		DentalOfficeReportServer,
		///<summary></summary>
		MobileWebOld,
		///<summary></summary>
		WebForms,
		///<summary>Database containing information about the devices monitored by the Headmaster app.</summary>
		Headmaster,
		///<summary>Database used by the Documentation department and ODHelp.</summary>
		ManualPublisher,
		///<summary>The database behind the chat system at HQ.</summary>
		WebChat,
		///<summary>Database containing information regarding offices hosted by OD.</summary>
		Hosting,
		///<summary>Database containing archives of HQ serviceshq</summary>
		ServicesHqArchive,
		///<summary>Only used when running tests. The OD proper database which holds test data for 19.1.</summary>
		DentalOffice_19_1,
		///<summary>Services HQ on replication server</summary>
		ServicesHqReplicated,
		///<summary>Database containing a few API and FHIR tables.</summary>
		ApiHQ,
		///<summary>Database containing Remote Support sessions.</summary>
		RemoteSupport,
		///<summary>Database containing speed test scores.</summary>
		SpeedTest,
	}
}
