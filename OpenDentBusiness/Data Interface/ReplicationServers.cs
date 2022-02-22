using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Reflection;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ReplicationServers{
		///<summary>This value is only retrieved once upon startup.  This variable is a long because Google's cloud services have server id's that
		///are of a higher value than a signed int can contained.  Additionally, 0 is a valid server id based on MySQL so we need to use -1 and can't
		///use a uint data type.</summary>
		private static long server_id=-1;

		/// <summary>The first time this is accessed, the value is obtained using a query.  Will be 0 unless a server id was set in my.ini.</summary>
		public static long Server_id {
			get{
				if(server_id==-1) {
					server_id=GetServer_id();
				}
				return server_id;
			}
			set{
				server_id=value;
			}
		}

		#region CachePattern

		private class ReplicationServerCache : CacheListAbs<ReplicationServer> {
			protected override List<ReplicationServer> GetCacheFromDb() {
				string command="SELECT * FROM replicationserver ORDER BY ServerId";
				return Crud.ReplicationServerCrud.SelectMany(command);
			}
			protected override List<ReplicationServer> TableToList(DataTable table) {
				return Crud.ReplicationServerCrud.TableToList(table);
			}
			protected override ReplicationServer Copy(ReplicationServer replicationServer) {
				return replicationServer.Copy();
			}
			protected override DataTable ListToTable(List<ReplicationServer> listReplicationServers) {
				return Crud.ReplicationServerCrud.ListToTable(listReplicationServers,"ReplicationServer");
			}
			protected override void FillCacheIfNeeded() {
				ReplicationServers.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ReplicationServerCache _replicationServerCache=new ReplicationServerCache();

		public static bool GetExists(Predicate<ReplicationServer> match,bool isShort=false) {
			return _replicationServerCache.GetExists(match,isShort);
		}

		public static List<ReplicationServer> GetDeepCopy(bool isShort=false) {
			return _replicationServerCache.GetDeepCopy(isShort);
		}

		public static ReplicationServer GetFirstOrDefault(Func<ReplicationServer,bool> match,bool isShort=false) {
			return _replicationServerCache.GetFirstOrDefault(match,isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _replicationServerCache.GetCount(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_replicationServerCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_replicationServerCache.FillCacheFromTable(table);
				return table;
			}
			return _replicationServerCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static long Insert(ReplicationServer serv) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				serv.ReplicationServerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),serv);
				return serv.ReplicationServerNum;
			}
			return Crud.ReplicationServerCrud.Insert(serv);
		}

		///<summary></summary>
		public static void Update(ReplicationServer serv) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),serv);
				return;
			}
			Crud.ReplicationServerCrud.Update(serv);
		}

		public static void DeleteObject(long replicationServerNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),replicationServerNum);
				return;
			}
			Crud.ReplicationServerCrud.Delete(replicationServerNum);
		}

		///<summary>Gets the MySQL server_id variable for the current connection.</summary>
		public static long GetServer_id() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			if(DataConnection.DBtype!=DatabaseType.MySql) {
				return 0;
			}
			string command="SHOW VARIABLES LIKE 'server_id'";
			DataTable table=Db.GetTable(command);
			return PIn.Long(table.Rows[0][1].ToString());
		}

		///<summary>Generates a random primary key.  Tests to see if that key already exists before returning it for use.  The range of returned values is greater than 0, and less than or equal to 9223372036854775807.</summary>
		public static long GetKey(string tablename,string field) {
			//No need to check RemotingRole; no call to db.
			//establish the range for this server
			long rangeStart=10000;
			long rangeEnd=long.MaxValue;
			//the following line triggers a separate call to db if server_id=-1.  Must be cap.
			if(Server_id!=0) {//if it IS 0, then there is no server_id set.
				ReplicationServer thisServer=GetFirstOrDefault(x => x.ServerId==Server_id);
				if(thisServer!=null) {//a ReplicationServer row was found for this server_id
					if(thisServer.RangeEnd-thisServer.RangeStart >= 999999){//and a valid range was entered that was at least 1,000,000
						rangeStart=thisServer.RangeStart;
						rangeEnd=thisServer.RangeEnd;
					}
				}
			}
			long rndLong;
			long span=rangeEnd-rangeStart;
			do {
				rndLong=(long)(ODRandom.NextDouble()*span) + rangeStart;
			}
			while(rndLong==0  
				|| rndLong < rangeStart
				|| rndLong > rangeEnd
				|| KeyInUse(tablename,field,rndLong));
			return rndLong;
		}

		///<summary>Generates a random primary key without using the cache.</summary>
		public static long GetKeyNoCache(string tablename,string field) {
			long rangeStart=10000;
			long rangeEnd=long.MaxValue;
			long server_id=GetServer_id();
			if(server_id!=0) {
				ReplicationServer thisServer=ReplicationServers.GetServer(server_id);
				if(thisServer!=null && thisServer.RangeEnd-thisServer.RangeStart >= 999999) {
					rangeStart=thisServer.RangeStart;
					rangeEnd=thisServer.RangeEnd;
				}
			}
			long span=rangeEnd-rangeStart;
			long rndLong=(long)(ODRandom.NextDouble()*span)+rangeStart;
			while(rndLong==0 
				|| rndLong<rangeStart 
				|| rndLong>rangeEnd
				|| KeyInUse(tablename,field,rndLong))
			{
				rndLong=(long)(ODRandom.NextDouble()*span)+rangeStart;
			}
			return rndLong;
		}

		///<summary>Gets a single ReplicationServer based on server_id.  Used to avoid cache issues.</summary>
		public static ReplicationServer GetServer(long server_id) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ReplicationServer>(MethodBase.GetCurrentMethod(),server_id);
			}
			string command="SELECT * FROM replicationserver WHERE ServerId="+POut.Long(server_id);
			return Crud.ReplicationServerCrud.SelectOne(command);
		}

		public static bool KeyInUse(string tablename,string field,long keynum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),tablename,field,keynum);
			}
			string command="SELECT COUNT(*) FROM "+tablename+" WHERE "+field+"="+keynum.ToString();
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;//already in use
		}

		///<summary>If this server id is 0, or if no AtoZ entered for this server, then returns empty string.</summary>
		public static string GetAtoZpath() {
			//No need to check RemotingRole; no call to db.
			ReplicationServer replicationServer=GetFirstOrDefault(x => x.ServerId==Server_id);
			return (replicationServer==null ? "" : replicationServer.AtoZpath);
		}

		///<summary>If this server id is 0, this returns null.  Or if there is no ReplicationServer object for this server id, then this returns null.</summary>
		public static ReplicationServer GetForLocalComputer() {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ServerId==Server_id);
		}

		///<summary>Used during database maint and from update window. We cannot use objects.</summary>
		public static bool ServerIsBlocked() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				//even though we are supposed to be guaranteed to not be a web client
				return true;
			}
			string command="SELECT COUNT(*) FROM replicationserver WHERE ServerId="+POut.Long(Server_id)//does trigger another query if during startup
				+" AND UpdateBlocked=1";
			try {
				if(Db.GetScalar(command)=="0") {
					return false;
				}
				else {
					return true;
				}
			}
			catch {
				return false;
			}
		}

		///<summary>Checks if the current database connected to is the replication report server.  Allows users to run dangerous custom queries that could potentially break replication.  We will allow these queries to be run on exactly one replication server (the report server), because our custom queries contain CREATE TABLE statements for static temporary table names which can cause replication failure if multiple users run the same query at the same time.</summary>
		public static bool IsConnectedReportServer() {
			//No need to check RemotingRole; no call to db.
			if(PrefC.GetLong(PrefName.ReplicationUserQueryServer)==0) {//Report server not set up.
				return false;
			}
			ReplicationServer repServer=GetForLocalComputer();
			if(repServer==null || repServer.ReplicationServerNum!=PrefC.GetLong(PrefName.ReplicationUserQueryServer)) {
				return false;
			}
			return true;
		}

		///<summary>Get the status of the replication server.</summary>
		public static DataTable GetSlaveStatus() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command="SHOW SLAVE STATUS";
			return Db.GetTable(command);
		}

		///<summary>Detects if an office is using replication. There is no singular way to tell if an office is using replication, this method makes a best guess.</summary>
		public static bool IsUsingReplication() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			//First ask OD
			if(ReplicationServers.GetCount() > 0) {
				return true;
			}
			//Second ask Database
			string command="SHOW MASTER STATUS ";
			DataTable tableReplicationStatus=Db.GetTable(command);
			if(tableReplicationStatus.Rows.Count > 0 || GetSlaveStatus().Rows.Count > 0) {
				return true;
			}
			//Last check Galera cluster (NADG)
			command="SHOW GLOBAL VARIABLES WHERE Variable_name='wsrep_on' ";
			tableReplicationStatus=Db.GetTable(command);
			for(int i=0;i<tableReplicationStatus.Rows.Count;i++) {
				DataRow row=tableReplicationStatus.Rows[i];
				if(PIn.String(row["Value"].ToString())=="ON") {
					command=$"SELECT COUNT(DISTINCT wcm.node_uuid) ";
					command+="FROM mysql.wsrep_cluster wc ";
					command+="INNER JOIN mysql.wsrep_cluster_members wcm ON wc.cluster_uuid=wcm.cluster_uuid ";
					int count=Db.GetInt(command);
					if(count>0) {
						return true;
					}
				}
			}
			return false;
		}
	}
}