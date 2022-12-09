using CodeBase;
using DataConnectionBase;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OpenDentBusiness {
	///<summary>Should only be used for running user queries.
	///A list of current MySqlConnections is kept so that we can cancel queries running on specific server threads in that list if need be.</summary>
	public class DataConnectionCancelable {
		///<summary>A static dictionary of connections via their ServerThread IDs.
		///Necessary for Middle Tier to be able to close the corresponding connection.</summary>
		private static Dictionary<int,MySqlConnection> _dictCons=new Dictionary<int, MySqlConnection>();
		///<summary>The lock object that is used to lock the dictionary of MySqlConnections.  Only used when adding and removing from the dict.</summary>
		private static object _lockObj=new object();


		///<summary>Opens a new and reserved connection to the DBMS (pooling=false) and leaves the connection open so that a query can be ran at a later time.
		///Returns the ServerThread of the open connection to the DBMS so that other methods within this class can take actions against this specific connection.</summary>
		public static int GetServerThread(bool useReportServer) {
			//Directly call the report server if desired and the current instance is a client (ClientDirect or ClientMT).
			if(useReportServer && RemotingClient.MiddleTierRole.In(MiddleTierRole.ClientDirect,MiddleTierRole.ClientMT)) {
				//Recursively invoke this method against the report server BUT tell the method to NOT use the report server.
				//This is because we are taking care of the report server by recursively executing the method within RunFuncOnReportServer().
				return ReportsComplex.RunFuncOnReportServer(() => GetServerThread(false));
			}
			//At this point it is safe to perform the typical S class Middle Tier remoting role check.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),useReportServer);
			}
			//Either not using a report server or the call stack has finally reached the correct server.
			string connectStr="";
			//Use the database user with lower permissions when in Middle Tier since this method is explicitly designed for the User Query window.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT) {
				connectStr=DataConnection.GetLowConnectionString();
			}
			else {
				connectStr=DataConnection.GetCurrentConnectionString();
			}
			//Append pooling=false to the connection string.
			MySqlConnection con=new MySqlConnection(connectStr+";pooling=false");
			con.Open();
			int serverThread=con.ServerThread;
			//If the dictionary already contains the ServerThread key, then something went wrong. Just stop and throw.
			if(_dictCons.ContainsKey(serverThread)) {
				con.Close();
				throw new ApplicationException("Critical error in GetServerThread: A duplicate connection was found via the server thread ID.");
			}
			lock(_lockObj) {
				_dictCons[serverThread]=con;
			}
			return serverThread;
		}

		///<summary>Fills and returns a DataTable from the database using an exiting database connection.
		///Currently only for user queries. The connection must already be opened before calling this method; See GetServerThread()
		///Throws an exception if a connection could not be found via the passed in server thread.</summary>
		public static DataTable GetTableConAlreadyOpen(int serverThread,string command,bool wasSqlValidated,bool isSqlAllowedReportServer=false,bool hasStackTrace=false,bool suppressMessage=false,bool useReportServer=false) {
			//Directly call the report server if desired and the current instance is a client (ClientDirect or ClientMT).
			if(useReportServer && RemotingClient.MiddleTierRole.In(MiddleTierRole.ClientDirect,MiddleTierRole.ClientMT)) {
				//Recursively invoke this method against the report server BUT tell the method to NOT use the report server.
				//This is because we are taking care of the report server by recursively executing the method within RunFuncOnReportServer().
				return ReportsComplex.RunFuncOnReportServer(() => GetTableConAlreadyOpen(serverThread,command,wasSqlValidated,isSqlAllowedReportServer,hasStackTrace,suppressMessage,useReportServer:false));
			}
			//At this point it is safe to perform the typical S class Middle Tier remoting role check.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),serverThread,command,wasSqlValidated,isSqlAllowedReportServer,hasStackTrace,suppressMessage,useReportServer);
			}
			//If the dictionary does not contain the ServerThread key, then something went wrong. Just stop and throw.
			MySqlConnection con;
			if(!_dictCons.TryGetValue(serverThread,out con)) {
				throw new ApplicationException("Critical error in GetTableConAlreadyOpen: A connection could not be found via the given server thread ID.");
			}
			//Throws Exception if Sql is not allowed, which is handled by the ExceptionThreadHandler and output in a MsgBox
			if(!wasSqlValidated && !Db.IsSqlAllowed(command,suppressMessage:suppressMessage,isRunningOnReportServer:isSqlAllowedReportServer)) {
				throw new ApplicationException("Error: Command is either not safe or user does not have permission.");
			}
			//At this point, we know that _dictCons contains the current connection's ServerThread ID.
			DataTable table=new DataTable();
			MySqlDataAdapter da=new MySqlDataAdapter(new MySqlCommand(command,con));
			try {
				Db.LastCommand=command;
				QueryMonitor.Monitor.RunMonitoredQuery(() => DataCore.ExecuteQueryFunc(() => da.Fill(table)),da.SelectCommand,hasStackTrace);
			}
			finally {
				con.Close(); //if the query was stopped or has finished executing, this will close the connection that it was executing on.
				lock(_lockObj) {
					_dictCons.Remove(serverThread);
				}
			}
			return table;
		}

		///<summary>Currently only for user queries.  Tries to cancel the connection that corresponds to the passed in server thread. 
		///Does not close the connection as that is taken care of in GetTableConAlreadyOpen() in a finally statement.
		///Optionally throws an exception if a connection could not be found via the passed in server thread.</summary>
		public static void CancelQuery(int serverThread,bool hasExceptions=true,bool useReportServer=false) {
			//Directly call the report server if desired and the current instance is a client (ClientDirect or ClientMT).
			if(useReportServer && RemotingClient.MiddleTierRole.In(MiddleTierRole.ClientDirect,MiddleTierRole.ClientMT)) {
				//Recursively invoke this method against the report server BUT tell the method to NOT use the report server.
				//This is because we are taking care of the report server by recursively executing the method within RunFuncOnReportServer().
				ReportsComplex.RunFuncOnReportServer(() => {
					CancelQuery(serverThread,hasExceptions,false);
					return "";//Not used. Func has to return something.
				});
				return;
			}
			//At this point it is safe to perform the typical S class Middle Tier remoting role check.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),serverThread,hasExceptions,useReportServer);
				return;
			}
			if(!_dictCons.ContainsKey(serverThread)) {
				//This could happen if the user clicked 'Cancel' and by the time it got here, the query finished executing on a different thread.
				//Since this race condition could happen more frequently within the middle tier environment, we just won't do anything about it.
				return;//Should be harmless so don't throw a critical exception here.
			}
			//At this point we want to stop the query currently running on the MySQL connection that corresponds to the server thread passed in.
			try {
				string command = "KILL QUERY "+serverThread;
				Db.NonQ(command);
			}
			catch(MySql.Data.MySqlClient.MySqlException ex) {
				if(ex.Number==1094) {
					return; //suppress errors about the thread not existing -- that means the query finished executing already.
				}
				if(hasExceptions) {
					throw ex; //otherwise, throw.
				}
			}
			catch(Exception e) {
				if(hasExceptions) {
					throw e; //and throw all other exceptions.
				}
			}
			finally {
				lock(_lockObj) {
					_dictCons.Remove(serverThread);
				}
			}
		}
	}

}
