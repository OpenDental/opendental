using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using DataConnectionBase;
using MySqlConnector;
using Newtonsoft.Json;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskTakens{
		#region Insert

		///<summary>Throws exceptions. Inserts a row into the tasktaken table that resides on the 'TriageHQ' database for the TaskNum passed in.
		///Set retryInsert to true in order to recursively call this method one more time in the event of an "unable to connect" MySQL UE.
		///Throws an exception if the insert failed. Otherwise, no excpetion will be thrown thus indicating success.</summary>
		public static void InsertForTaskNum(long taskNum,bool retryInsert=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum,retryInsert);
				return;
			}
			try {
				string prefCustomersHQServer=PrefC.GetString(PrefName.CustomersHQServer);
				string prefCustomersHQMySqlUser=PrefC.GetString(PrefName.CustomersHQMySqlUser);
				DataAction.RunTriageHQ(() => {
					CentralConnectionBase cn=ConnectionStoreBase.GetTriageHQ();
					string logJson=JsonConvert.SerializeObject(new TaskTakenLogJson() {
						DateTimeWorkstation=DateTime.Now,
						WorkstationName=Environment.MachineName,
						DataConUser=DataConnection.GetMysqlUser(),
						MysqlHostName=Db.GetScalar("SELECT @@hostname"),
						MysqlCurUser=Db.GetScalar("SELECT CURRENT_USER()"),
						PrefCustomersHQServer=prefCustomersHQServer,
						PrefCustomersHQMySqlUser=prefCustomersHQMySqlUser,
						ConStoreServerName=cn?.ServerName??"NULL",
						ConStoreMySqlUser=cn?.MySqlUser??"NULL",
						RetryInsert=retryInsert,
					});
					Crud.TaskTakenCrud.Insert(new TaskTaken { TaskNum=taskNum,LogJson=logJson });
				});
			}
			catch(Exception ex) {
				MySqlException mysqlException=null;
				if(ex is MySqlException) {
					mysqlException=(MySqlException)ex;
				}
				else if(ex.InnerException is MySqlException) {
					mysqlException=(MySqlException)ex.InnerException;
				}
				if(mysqlException!=null && mysqlException.Number==1062 && mysqlException.Message.ToLower().Contains("duplicate entry")) {
					//Someone else has already taken this task so there is already a row in the tasktaken table for this task.
					throw new Exception("Not allowed to save changes because the task has been claimed by someone else.");
				}
				if(mysqlException!=null && mysqlException.Number==1042 && mysqlException.Message.ToLower().Contains("unable to connect")) {
					if(retryInsert) {
						//Could be a network hiccup so try inserting the tasktaken once more.
						InsertForTaskNum(taskNum,false);
						return;
					}
					throw new Exception("Failed to save changes to the database. Please try again.\r\n\r\nNotify your supervisor if this keeps happening.");
				}
				throw new Exception("Failed to save changes due to a generic error. Please try again.\r\n\r\nError: "+ex.Message);
			}
		}

		#endregion

		#region Delete
		///<summary>Deletes any TaskTaken for the given taskNum. Runs on the primary customers database.</summary>
		public static void DeleteForTask(long taskNum,bool runOnPrimaryCustomers=true,bool retryOnLocal=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum,runOnPrimaryCustomers,retryOnLocal);
				return;
			}
			string command="DELETE FROM tasktaken WHERE TaskNum="+POut.Long(taskNum);			
			try {
				if(runOnPrimaryCustomers) {
					DataAction.RunTriageHQ(() => Db.NonQ(command));
				}
				else {
					Db.NonQ(command);
				}
			}
			catch(Exception ex) {
				MySqlException mysqlEx=null;
				if(ex is MySqlException) {
					mysqlEx=(MySqlException)ex;
				}
				else if(ex.InnerException is MySqlException) {
					mysqlEx=(MySqlException)ex.InnerException;
				}
				if(mysqlEx!=null && mysqlEx.Number==1042 && mysqlEx.Message.ToLower().Contains("unable to connect") && retryOnLocal) {
					//Unable to connect to the primary customers database. We will still delete the tasktaken on the local database.
					DeleteForTask(taskNum,false,false);
					return;
				}
				throw;
			}
		}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		
		///<summary>Gets one TaskTaken from the db.</summary>
		public static TaskTaken GetOne(long taskTakenNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TaskTaken>(MethodBase.GetCurrentMethod(),taskTakenNum);
			}
			return Crud.TaskTakenCrud.SelectOne(taskTakenNum);
		}
		#endregion

		#region Modification Methods
			#region Update
		///<summary></summary>
		public static void Update(TaskTaken taskTaken){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskTaken);
				return;
			}
			Crud.TaskTakenCrud.Update(taskTaken);
		}
			#endregion
		#endregion
		*/
	}

	public class TaskTakenLogJson {
		public DateTime DateTimeWorkstation;
		///<summary>The name of the workstation from which the insert command was issued, usually a tech name.</summary>
		public string WorkstationName;
		///<summary>The name of the MySQL user OD according to the OD DataConnection.</summary>
		public string DataConUser;
		///<summary>The name of the server from which the insert command was executed.
		///This data comes from the @@hostname variable directly through the MySQL connection.</summary>
		public string MysqlHostName;
		///<summary>The name of the MySQL user from the MySQL connection, according to the MySQL server.
		///This data comes from the CURRENT_USER() function directly through the MySQL connection.</summary>
		public string MysqlCurUser;
		///<summary>The value of the CustomersHQServer pref at the time the TriageHQ connection was created.</summary>
		public string PrefCustomersHQServer;
		///<summary>The value of the CustomersHQServer pref at the time the TriageHQ connection was created.</summary>
		public string PrefCustomersHQMySqlUser;
		///<summary>The value inside the ConnectionStoreBase.GetTriageHQ().ServerName</summary>
		public string ConStoreServerName;
		///<summary>The value inside the ConnectionStoreBase.GetTriageHQ().MySqlName</summary>
		public string ConStoreMySqlUser;
		public bool RetryInsert;
	}
}