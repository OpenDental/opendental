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
			//Get MachineName before the remoting role check so Environment.MachineName is not the middle tier server.
			EnsureComputerInDB(ODEnvironment.MachineName,Environment.MachineName);//ODEnvironment.MachineName and Environment.MachineName are different in RDP and ODCloud.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_computerCache.FillCacheFromTable(table);
				return table;
			}
			return _computerCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		public static void EnsureComputerInDB(string clientComputerName,string hostComputerName){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clientComputerName,hostComputerName);
				return;
			}
			string command="SELECT COUNT(*) FROM computer WHERE CompName ='"+POut.String(clientComputerName)+"'";
			long count=Db.GetLong(command);
			if(count == 0) {
				Computer Cur=new Computer();
				Cur.CompName=clientComputerName;
				long computerNum = Computers.Insert(Cur);
				//Never copy the printer rows for ODCloud
				if(!ODBuild.IsWeb() && clientComputerName.ToLower() != hostComputerName.ToLower()) {
					CopyPrinterRowsForComputer(computerNum,hostComputerName);//This computer is an RDP remote client. Copy the host computer's printer settings for the new computer.
				}
			}
		}

		///<summary>
		///Called when a new computer is added and OD is running on a remote application server. 
		///This copies any printer settings associated with the application server's computer and applies them to the new computer.
		///21.3 introduces per-client-computer printer settings for RDP app servers and this prevents printer settings from being reset for client computers when updating to 21.3.
		///</summary>
		public static void CopyPrinterRowsForComputer(long computerNum,string hostComputerName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerNum,hostComputerName);
				return;
			}
			//computerName is the client computer of a remote connection.
			string command=$"SELECT ComputerNum FROM computer WHERE CompName='{POut.String(hostComputerName)}'";
			long hostComputerNum=Db.GetLong(command);
			if(hostComputerNum == 0) {
				return;//Could not find the host computer in the database, no printer settings to copy.
			}
			//Copy the host computer's printer settings for the client computer.
			command="INSERT INTO printer (ComputerNum,PrintSit,PrinterName,DisplayPrompt) " +
				$"SELECT {POut.Long(computerNum)},PrintSit,PrinterName,DisplayPrompt FROM printer WHERE ComputerNum={POut.Long(hostComputerNum)}";
			Db.NonQ(command);
		}

		///<summary>ONLY use this if compname is not already present</summary>
		public static long Insert(Computer comp) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),comp);
				return;
			}
			//Delete any accociated printer settings from the printer table
			string command=$"DELETE FROM printer WHERE ComputerNum={POut.Long(comp.ComputerNum)}";
 			Db.NonQ(command);
			command=$"DELETE FROM computer WHERE ComputerNum={POut.Long(comp.ComputerNum)}";
 			Db.NonQ(command);
		}

		///<summary>Only called from Printers.GetForSit</summary>
		public static Computer GetCur(){
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.CompName.ToUpper()==ODEnvironment.MachineName.ToUpper());
		}

		///<summary>Returns all computers with an active heart beat.  A heart beat less than 4 minutes old is considered active.</summary>
		public static List<Computer> GetRunningComputers() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),computerName);
				return;
			}
			string command= "UPDATE computer SET LastHeartBeat="+POut.Date(new DateTime(0001,1,1),true)+" WHERE CompName = '"+POut.String(computerName)+"'";
			Db.NonQ(command);
		}

		public static void ClearAllHeartBeats(string machineNameException) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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