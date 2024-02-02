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
		private static long _serverId=-1;

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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_replicationServerCache.FillCacheFromTable(table);
				return table;
			}
			return _replicationServerCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_replicationServerCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static long Insert(ReplicationServer replicationServer) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				replicationServer.ReplicationServerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),replicationServer);
				return replicationServer.ReplicationServerNum;
			}
			return Crud.ReplicationServerCrud.Insert(replicationServer);
		}

		///<summary></summary>
		public static void Update(ReplicationServer replicationServer) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),replicationServer);
				return;
			}
			Crud.ReplicationServerCrud.Update(replicationServer);
		}

		public static void DeleteObject(long replicationServerNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),replicationServerNum);
				return;
			}
			Crud.ReplicationServerCrud.Delete(replicationServerNum);
		}
		
		/// <summary>The first time this is accessed, the value is obtained using a query.  Will be 0 unless a server id was set in my.ini.</summary>
		public static long GetServerId() {
			//No need to check MiddleTierRole; no call to db.
			if(_serverId==-1) {
				_serverId=GetServerIdFromDb();
			}
			return _serverId;
		}

		public static void SetServerId(long serverId) {
			//No need to check MiddleTierRole; no call to db.
			_serverId=serverId;
		}

		///<summary>Gets the MySQL server_id variable for the current connection.</summary>
		public static long GetServerIdFromDb() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		public static long GetKey(string tableName,string field) {
			//No need to check MiddleTierRole; no call to db.
			//establish the range for this server
			long rangeStart=10000;
			long rangeEnd=long.MaxValue;
			//the following line triggers a separate call to db if server_id=-1.  Must be cap.
			if(GetServerId()!=0) {//if it IS 0, then there is no server_id set.
				ReplicationServer replicationServer=GetFirstOrDefault(x => x.ServerId==GetServerId());
				if(replicationServer!=null) {//a ReplicationServer row was found for this server_id
					if(replicationServer.RangeEnd-replicationServer.RangeStart >= 999999){//and a valid range was entered that was at least 1,000,000
						rangeStart=replicationServer.RangeStart;
						rangeEnd=replicationServer.RangeEnd;
					}
				}
			}
			long key;
			long span=rangeEnd-rangeStart;
			while(true) {
				key=(long)(ODRandom.NextDouble()*span) + rangeStart;
				if (key!=0
					&& key > rangeStart
					&& key < rangeEnd
					&& !KeyInUse(tableName,field,key)) 
				{
					break;
				}
			}
			return key;
		}

		///<summary>Generates a random primary key without using the cache.</summary>
		public static long GetKeyNoCache(string tableName,string field) {
			long rangeStart=10000;
			long rangeEnd=long.MaxValue;
			long server_id=GetServerIdFromDb();
			if(server_id!=0) {
				ReplicationServer replicationServer=ReplicationServers.GetServer(server_id);
				if(replicationServer!=null && replicationServer.RangeEnd-replicationServer.RangeStart >= 999999) {
					rangeStart=replicationServer.RangeStart;
					rangeEnd=replicationServer.RangeEnd;
				}
			}
			long span=rangeEnd-rangeStart;
			long key=(long)(ODRandom.NextDouble()*span)+rangeStart;
			while(true) {
				if (key!=0 
					&& key>rangeStart 
					&& key<rangeEnd
					&& !KeyInUse(tableName,field,key)) 
				{
					break;
				}
				key=(long)(ODRandom.NextDouble()*span)+rangeStart;
			}
			return key;
		}

		///<summary>Gets a single ReplicationServer based on server_id.  Used to avoid cache issues.</summary>
		public static ReplicationServer GetServer(long server_id) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ReplicationServer>(MethodBase.GetCurrentMethod(),server_id);
			}
			string command="SELECT * FROM replicationserver WHERE ServerId="+POut.Long(server_id);
			return Crud.ReplicationServerCrud.SelectOne(command);
		}

		public static bool KeyInUse(string tableName,string field,long keynum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),tableName,field,keynum);
			}
			string command="SELECT COUNT(*) FROM "+tableName+" WHERE "+field+"="+keynum.ToString();
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;//already in use
		}

		///<summary>If this server id is 0, or if no AtoZ entered for this server, then returns empty string.</summary>
		public static string GetAtoZpath() {
			//No need to check MiddleTierRole; no call to db.
			ReplicationServer replicationServer=GetFirstOrDefault(x => x.ServerId==GetServerId());
			if (replicationServer==null) {
				return "";
			}
			return replicationServer.AtoZpath;
		}

		///<summary>If this server id is 0, this returns null.  Or if there is no ReplicationServer object for this server id, then this returns null.</summary>
		public static ReplicationServer GetForLocalComputer() {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.ServerId==GetServerId());
		}

		///<summary>Used during database maint and from update window. We cannot use objects.</summary>
		public static bool ServerIsBlocked() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				//even though we are supposed to be guaranteed to not be a web client
				return true;
			}
			string command="SELECT COUNT(*) FROM replicationserver WHERE ServerId="+POut.Long(GetServerId())//does trigger another query if during startup
				+" AND UpdateBlocked=1";
			try {
				bool isServerBlocked=Db.GetScalar(command)!="0";
				return isServerBlocked;
			}
			catch {
				return false;
			}
		}

		///<summary>Checks if the current database connected to is the replication report server.  Allows users to run dangerous custom queries that could potentially break replication.  We will allow these queries to be run on exactly one replication server (the report server), because our custom queries contain CREATE TABLE statements for static temporary table names which can cause replication failure if multiple users run the same query at the same time.</summary>
		public static bool IsConnectedReportServer() {
			//No need to check MiddleTierRole; no call to db.
			if(PrefC.GetLong(PrefName.ReplicationUserQueryServer)==0) {//Report server not set up.
				return false;
			}
			ReplicationServer replicationServer=GetForLocalComputer();
			if(replicationServer==null || replicationServer.ReplicationServerNum!=PrefC.GetLong(PrefName.ReplicationUserQueryServer)) {
				return false;
			}
			return true;
		}

		///<summary>Get the status of the replication server.</summary>
		public static DataTable GetSlaveStatus() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod());
			}
			string command="SHOW SLAVE STATUS";
			return Db.GetTable(command);
		}

		///<summary>Detects if an office is using replication. There is no singular way to tell if an office is using replication, this method makes a best guess.</summary>
		public static bool IsUsingReplication() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			//ODCloud does not use replication
			if(ODBuild.IsWeb()) {
				return false;
			}
			//For the majority of calling methods, they should treat the program as if replication is not being used. 
			//For the few places in the program that the database is cloud hosted it needs to check DatabaseGlobalVariablesDontSet to know if replication should be skipped.
			if(PrefC.GetBool(PrefName.DatabaseGlobalVariablesDontSet)) {
				return false;
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