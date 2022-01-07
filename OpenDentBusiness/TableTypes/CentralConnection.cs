using System;
using System.Collections;
using System.Xml.Serialization;
using DataConnectionBase;

namespace OpenDentBusiness{

	///<summary>Used by the Central Manager.  Stores the information needed to establish a connection to a remote database.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class CentralConnection:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
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
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
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
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsAutomaticLogin;
		///<summary>This is a helper variable used for Reports. If we want to start supporting connection string for the Reporting Server, we need to add this as a db column. This was needed for the scenario where a customer connected to OD using a connection string.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string ConnectionString;
		///<summary>Helper variable to keep track of the password hash that was passed in as a command line argument. This is necessary for automatically logging eCW users in when they are utilizing the Middle Tier.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public string OdPassHash;
		///<summary>Contains the most recent eConnector status. Not stored in the DB since we want to pull this data dynamically.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public eServiceSignalSeverity EServicesSignalSeverity=eServiceSignalSeverity.None;

		///<summary>Returns a copy.</summary>
		public CentralConnection Copy() {
			return (CentralConnection)this.MemberwiseClone();
		}

		public bool IsConnectionValid() {
			return this!=null && ConnectionStatus=="OK";
		}

		///<summary>Converts a CentralConnection to a CentralConnectionBase.</summary>
		public static explicit operator CentralConnectionBase(CentralConnection conn) {
			return new CentralConnectionBase {
				CentralConnectionNum=conn.CentralConnectionNum,
				ConnectionStatus=conn.ConnectionStatus,
				ConnectionString=conn.ConnectionString,
				DatabaseName=conn.DatabaseName,
				HasClinicBreakdownReports=conn.HasClinicBreakdownReports,
				IsAutomaticLogin=conn.IsAutomaticLogin,
				ItemOrder=conn.ItemOrder,
				MySqlPassword=conn.MySqlPassword,
				MySqlUser=conn.MySqlUser,
				Note=conn.Note,
				OdPassHash=conn.OdPassHash,
				OdPassword=conn.OdPassword,
				OdUser=conn.OdUser,
				ServerName=conn.ServerName,
				ServiceURI=conn.ServiceURI,
				WebServiceIsEcw=conn.WebServiceIsEcw,
			};
		}

		///<summary>Converts a CentralConnectionBase to a CentralConnection.</summary>
		public static explicit operator CentralConnection(CentralConnectionBase conn) {
			if(conn is null){
				return null;
			}
			return new CentralConnection {
				CentralConnectionNum=conn.CentralConnectionNum,
				ConnectionStatus=conn.ConnectionStatus,
				ConnectionString=conn.ConnectionString,
				DatabaseName=conn.DatabaseName,
				HasClinicBreakdownReports=conn.HasClinicBreakdownReports,
				IsAutomaticLogin=conn.IsAutomaticLogin,
				ItemOrder=conn.ItemOrder,
				MySqlPassword=conn.MySqlPassword,
				MySqlUser=conn.MySqlUser,
				Note=conn.Note,
				OdPassHash=conn.OdPassHash,
				OdPassword=conn.OdPassword,
				OdUser=conn.OdUser,
				ServerName=conn.ServerName,
				ServiceURI=conn.ServiceURI,
				WebServiceIsEcw=conn.WebServiceIsEcw,
			};
		}
	}


}













