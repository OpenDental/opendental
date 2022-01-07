using CodeBase;
using DataConnectionBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Computers {
		#region CachePattern

		private class ComputerCache : CacheListAbs<Computer> {
			protected override List<Computer> GetCacheFromDb() {
				string command="SELECT * FROM computer ORDER BY CompName";
				return Crud.ComputerCrud.SelectMany(command);
			}
			protected override List<Computer> TableToList(DataTable table) {
				return Crud.ComputerCrud.TableToList(table);
			}
			protected override Computer Copy(Computer computer) {
				return computer.Copy();
			}
			protected override DataTable ListToTable(List<Computer> listComputers) {
				return Crud.ComputerCrud.ListToTable(listComputers,"Computer");
			}
			protected override void FillCacheIfNeeded() {
				Computers.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ComputerCache _computerCache=new ComputerCache();

		public static List<Computer> GetDeepCopy(bool isShort=false) {
			return _computerCache.GetDeepCopy(isShort);
		}

		public static Computer GetFirstOrDefault(Func<Computer,bool> match,bool isShort=false) {
			return _computerCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_computerCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			//It is important to call EnsureComputerInDB prior to the remoting role check.
			EnsureComputerInDB(ODEnvironment.MachineName);
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_computerCache.FillCacheFromTable(table);
				return table;
			}
			return _computerCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		public static void EnsureComputerInDB(string computerName){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerName);
				return;
			}
			string command=
				"SELECT * from computer "
				+"WHERE compname = '"+computerName+"'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				Computer Cur=new Computer();
				Cur.CompName=computerName;
				Computers.Insert(Cur);
			}
		}

		///<summary>ONLY use this if compname is not already present</summary>
		public static long Insert(Computer comp) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				comp.ComputerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),comp);
				return comp.ComputerNum;
			}
			return Crud.ComputerCrud.Insert(comp);
		}

		/*
		///<summary></summary>
		public static void Update(){
			string command= "UPDATE computer SET "
				+"compname = '"    +POut.PString(CompName)+"' "
				//+"printername = '" +POut.PString(PrinterName)+"' "
				+"WHERE ComputerNum = '"+POut.PInt(ComputerNum)+"'";
			//MessageBox.Show(string command);
			DataConnection dcon=new DataConnection();
 			Db.NonQ(command);
		}*/

		///<summary></summary>
		public static void Delete(Computer comp){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),comp);
				return;
			}
			string command= "DELETE FROM computer WHERE computernum = '"+comp.ComputerNum.ToString()+"'";
 			Db.NonQ(command);
		}

		///<summary>Only called from Printers.GetForSit</summary>
		public static Computer GetCur(){
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.CompName.ToUpper()==ODEnvironment.MachineName.ToUpper());
		}

		///<summary>Returns all computers with an active heart beat.  A heart beat less than 4 minutes old is considered active.</summary>
		public static List<Computer> GetRunningComputers() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Computer>>(MethodBase.GetCurrentMethod());
			}
			//heartbeat is every three minutes.  We'll allow four to be generous.
			string command="SELECT * FROM computer WHERE LastHeartBeat > SUBTIME(NOW(),'00:04:00')";
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				command="SELECT * FROM computer WHERE LastHeartBeat > SYSDATE - (4/1440)";
			}
			return Crud.ComputerCrud.SelectMany(command);
		}

		/// <summary>When starting up, in an attempt to be fast, it will not add a new computer to the list.</summary>
		public static void UpdateHeartBeat(string computerName,bool isStartup) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerName,isStartup);
				return;
			}
			string command;
			if(!isStartup) {
				if(_computerCache.ListIsNull()) {
					RefreshCache();//adds new computer to list
				}
				command="SELECT LastHeartBeat<"+DbHelper.DateAddMinute(DbHelper.Now(),"-3")+" FROM computer WHERE CompName='"+POut.String(computerName)+"'";
				if(!PIn.Bool(Db.GetScalar(command))) {//no need to update if LastHeartBeat is already within the last 3 mins
					return;//remote app servers with multiple connections would fight over the lock on a single row to update the heartbeat unnecessarily
				}
			}
			command="UPDATE computer SET LastHeartBeat="+DbHelper.Now()+" WHERE CompName = '"+POut.String(computerName)+"'";
			Db.NonQ(command);
		}

		public static void ClearHeartBeat(string computerName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerName);
				return;
			}
			string command= "UPDATE computer SET LastHeartBeat="+POut.Date(new DateTime(0001,1,1),true)+" WHERE CompName = '"+POut.String(computerName)+"'";
			Db.NonQ(command);
		}

		public static void ClearAllHeartBeats(string machineNameException) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),machineNameException);
				return;
			}
			string command= "UPDATE computer SET LastHeartBeat="+POut.Date(new DateTime(0001,1,1),true)+" "
				+"WHERE CompName != '"+POut.String(machineNameException)+"'";
			Db.NonQ(command);
		}

		///<summary>Returns a list of strings in a specific order.  
		///The strings are as follows; socket (service name), version_comment (service comment), hostname (server name), MySQL version,
		///and database name. Oracle is not supported and will throw an exception to have the customer call us to add support.</summary>
		public static List<string> GetServiceInfo() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				throw new Exception(Lans.g("Computer","Currently not Oracle compatible.  Please call support."));
			}
			List<string> retVal=new List<string>();
			DataTable table=Db.GetTable("SHOW VARIABLES WHERE Variable_name='socket'");//service name
			if(table.Rows.Count>0) {
				retVal.Add(table.Rows[0]["VALUE"].ToString());
			}
			else {
				retVal.Add("Not Found");
			}
			table=Db.GetTable("SHOW VARIABLES WHERE Variable_name='version_comment'");//service comment
			if(table.Rows.Count>0) {
				retVal.Add(table.Rows[0]["VALUE"].ToString());
			}
			else {
				retVal.Add("Not Found");
			}
			try { 
				table=Db.GetTable("SELECT @@hostname");//server name
				if(table.Rows.Count>0) {
					retVal.Add(table.Rows[0][0].ToString());
				}
				else {
					retVal.Add("Not Found");
				}
			}
			catch {
				retVal.Add("Not Found");//hostname variable doesn't exist
			}
			retVal.Add(MiscData.GetMySqlVersion());
			try {
				string dbName="";
				dbName=MiscData.GetCurrentDatabase();//database name
				if(string.IsNullOrEmpty(dbName)) {
					retVal.Add("Not Found");
				}
				else {
					retVal.Add(dbName);
				}
			}
			catch {
				retVal.Add("Not Found.");//database variable doesn't exist
			}
			return retVal;
		}
	}
}