using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskAncestors {	
		///<summary></summary>
		public static long Insert(TaskAncestor ancestor) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ancestor.TaskAncestorNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ancestor);
				return ancestor.TaskAncestorNum;
			}
			return Crud.TaskAncestorCrud.Insert(ancestor);
		}

		/*
		///<summary></summary>
		public static void Update(TaskAncestor ancestor) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ancestor);
				return;
			}
			Crud.TaskAncestorCrud.Update(ancestor);
		}*/

		public static void Synch(Task task){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),task);
				return;
			}
			string command="DELETE FROM taskancestor WHERE TaskNum="+POut.Long(task.TaskNum);
			Db.NonQ(command);
			long taskListNum=0;
			long parentNum=task.TaskListNum;
			DataTable table;
			TaskAncestor ancestor;
			while(true){
				if(parentNum==0){
					break;//no parent to mark
				}
				//get the parent
				command="SELECT TaskListNum,Parent FROM tasklist WHERE TaskListNum="+POut.Long(parentNum);
				table=Db.GetTable(command);
				if(table.Rows.Count==0){//in case of database inconsistency
					break;
				}
				taskListNum=PIn.Long(table.Rows[0]["TaskListNum"].ToString());
				parentNum=PIn.Long(table.Rows[0]["Parent"].ToString());
				ancestor=new TaskAncestor();
				ancestor.TaskNum=task.TaskNum;
				ancestor.TaskListNum=taskListNum;
				Insert(ancestor);
			}
		}

		///<summary>This should only be used when synching ancestors for multiple tasks in the same tasklist.
		///Limits DELETE, SELECT and INSERT calls to DB.</summary>
		public static void SynchManyForSameTasklist(List<Task> listTasks,long taskListNum, long taskListParent) {
			//Return if the task list passed in is invalid or trying to manipulate ancestors associated to the trunk (main).
			if(listTasks==null || listTasks.Count < 1 || taskListNum==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTasks,taskListNum,taskListParent);
				return;
			}
			string command="DELETE FROM taskancestor WHERE TaskNum IN ("+string.Join(",",listTasks.Select(x => POut.Long(x.TaskNum)))+")";
			Db.NonQ(command);
			DataTable table;
			while(true){
				List<TaskAncestor> listTaskAncestors = new List<TaskAncestor>();
				foreach(Task t in listTasks) {
					TaskAncestor ancestor = new TaskAncestor();
					ancestor.TaskNum=t.TaskNum;
					ancestor.TaskListNum=taskListNum;
					listTaskAncestors.Add(ancestor);
				}
				Crud.TaskAncestorCrud.InsertMany(listTaskAncestors);
				if(taskListParent==0) {
					break;//No more work needed
				}
				//get the parent
				command="SELECT TaskListNum,Parent FROM tasklist WHERE TaskListNum="+POut.Long(taskListParent);
				table=Db.GetTable(command);
				if(table.Rows.Count==0) {//in case of database inconsistency
					break;
				}
				taskListNum=PIn.Long(table.Rows[0]["TaskListNum"].ToString());
				taskListParent=PIn.Long(table.Rows[0]["Parent"].ToString());
			}
		}
		
		///<summary>Only run once after the upgrade to version 5.5.</summary>
		public static void SynchAll(){
			//No need to check RemotingRole; no call to db.
			List<Task> listTasks=Tasks.RefreshAll();
			for(int i=0;i<listTasks.Count;i++){
				Synch(listTasks[i]);
			}
		}
	}

	


	


}









