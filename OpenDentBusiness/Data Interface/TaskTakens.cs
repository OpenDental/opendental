using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskTakens{
		#region Insert

		///<summary>Tries to insert a TaskTaken for the given tasknum. Returns true if successful, returns false if a row with that tasknum already
		///exists. Defaults to run on the primary customers database to prevent the triage tasks from getting claimed twice.</summary>
		///<param name="doRetryOnLocal">If true and a connection to the primary customers database cannot be established, the insert will be attempted
		///on the local database.</param>
		public static bool TryInsert(long taskNum,bool doRunOnPrimaryCustomers=true,bool doRetryOnLocal=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),taskNum,doRunOnPrimaryCustomers,doRetryOnLocal);
			}
			try {
				if(doRunOnPrimaryCustomers) {
					DataAction.RunCustomers(() => Crud.TaskTakenCrud.Insert(new TaskTaken { TaskNum=taskNum }));
				}
				else {
					Crud.TaskTakenCrud.Insert(new TaskTaken { TaskNum=taskNum });
				}
				return true;
			}
			catch(Exception ex) {
				MySqlException mysqlEx=null;
				if(ex is MySqlException) {
					mysqlEx=(MySqlException)ex;
				}
				else if(ex.InnerException is MySqlException) {
					mysqlEx=(MySqlException)ex.InnerException;
				}
				if(mysqlEx!=null && mysqlEx.Number==1062 && mysqlEx.Message.ToLower().Contains("duplicate entry")) {
					//Someone else has already taken this task so there is already a row in the tasktaken table for this task.
					return false;
				}
				if(mysqlEx!=null && mysqlEx.Number==1042 && mysqlEx.Message.ToLower().Contains("unable to connect") && doRetryOnLocal) {
					//Unable to connect to the primary customers database. We will still allow them to claim the task, and we will record that the task was 
					//taken in the local database.
					return TryInsert(taskNum,false,false);
				}
				throw;
			}
		}

		#endregion

		#region Delete
		///<summary>Deletes any TaskTaken for the given taskNum. Runs on the primary customers database.</summary>
		public static void DeleteForTask(long taskNum,bool doRunOnPrimaryCustomers=true,bool doRetryOnLocal=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum,doRunOnPrimaryCustomers,doRetryOnLocal);
				return;
			}
			string command="DELETE FROM tasktaken WHERE TaskNum="+POut.Long(taskNum);			
			try {
				if(doRunOnPrimaryCustomers) {
					DataAction.RunCustomers(() => Db.NonQ(command));
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
				if(mysqlEx!=null && mysqlEx.Number==1042 && mysqlEx.Message.ToLower().Contains("unable to connect") && doRetryOnLocal) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TaskTaken>(MethodBase.GetCurrentMethod(),taskTakenNum);
			}
			return Crud.TaskTakenCrud.SelectOne(taskTakenNum);
		}
		#endregion
		#region Modification Methods
			#region Update
		///<summary></summary>
		public static void Update(TaskTaken taskTaken){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskTaken);
				return;
			}
			Crud.TaskTakenCrud.Update(taskTaken);
		}
			#endregion
		#endregion
		*/
	}
}