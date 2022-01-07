using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskHists{
		///<summary>Gets one TaskHist from the db.</summary>
		public static TaskHist GetOne(long taskHistNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TaskHist>(MethodBase.GetCurrentMethod(),taskHistNum);
			}
			return Crud.TaskHistCrud.SelectOne(taskHistNum);
		}

		public static string GetChangesDescription(TaskHist taskCur,TaskHist taskNext) {
			if(taskCur.Descript.StartsWith("This task was cut from task list ") || taskCur.Descript.StartsWith("This task was copied from task ")) {
				return taskCur.Descript;
			}
			if(taskCur.DateTimeEntry==DateTime.MinValue) {
				return Lans.g("TaskHists","New task.");
			}
			StringBuilder strb=new StringBuilder();
			strb.Append("");
			if(taskNext.TaskListNum!=taskCur.TaskListNum){
				string descOne=Lans.g("TaskHists","(DELETED)");
				string descTwo=Lans.g("TaskHists","(DELETED)");
				TaskList taskList=TaskLists.GetOne(taskCur.TaskListNum);
				if(taskList!=null) {
					descOne=taskList.Descript;
				}
				taskList=TaskLists.GetOne(taskNext.TaskListNum);
				if(taskList!=null) {
					descTwo=taskList.Descript;
				}
				strb.Append(Lans.g("TaskHists","Task list changed from")+" "+descOne+" "+Lans.g("TaskHists","to")+" "+descTwo+".\r\n");
			}
			if(taskNext.ObjectType!=taskCur.ObjectType){
				strb.Append(Lans.g("TaskHists","Task attachment changed from")+" "
					+taskCur.ObjectType.ToString()+" "+Lans.g("TaskHists","to")+" "+taskNext.ObjectType.ToString()+".\r\n");
			}
			if(taskNext.KeyNum!=taskCur.KeyNum){
				strb.Append(Lans.g("TaskHists","Task account attachment changed.")+"\r\n");
			}
			if(taskNext.Descript!=taskCur.Descript && !taskNext.Descript.StartsWith("This task was cut from task list ") 
				&& !taskNext.Descript.StartsWith("This task was copied from task "))
			{
				//We change the description of a task when it is cut/copied. 
				//This prevents the history grid from showing a description changed when it wasn't changed by the user.
				strb.Append(Lans.g("TaskHists","Task description changed.")+"\r\n");
			}
			if(taskNext.TaskStatus!=taskCur.TaskStatus){
				strb.Append(Lans.g("TaskHists","Task status changed from")+" "
					+taskCur.TaskStatus.ToString()+" "+Lans.g("TaskHists","to")+" "+taskNext.TaskStatus.ToString()+".\r\n");
			}
			if(taskNext.DateTimeEntry!=taskCur.DateTimeEntry){
				strb.Append(Lans.g("TaskHists","Task date added changed from")+" "
					+taskCur.DateTimeEntry.ToString()
					+" "+Lans.g("TaskHists","to")+" "
					+taskNext.DateTimeEntry.ToString()+".\r\n");
			}
			if(taskNext.UserNum!=taskCur.UserNum){
				strb.Append(Lans.g("TaskHists","Task author changed from ")+GetUserName(taskCur.UserNum)+" "
					+Lans.g("TaskHists","to")+" "+GetUserName(taskNext.UserNum)+".\r\n");
			}
			if(taskNext.DateTimeFinished!=taskCur.DateTimeFinished){
				strb.Append(Lans.g("TaskHists","Task date finished changed from")+" "
					+taskCur.DateTimeFinished.ToString()
					+" "+Lans.g("TaskHists","to")+" "
					+taskNext.DateTimeFinished.ToString()+".\r\n");
			}
			if(taskNext.PriorityDefNum!=taskCur.PriorityDefNum){
				strb.Append(Lans.g("TaskHists","Task priority changed from")+" "
					+Defs.GetDef(DefCat.TaskPriorities,taskCur.PriorityDefNum).ItemName
					+" "+Lans.g("TaskHists","to")+" "
					+Defs.GetDef(DefCat.TaskPriorities,taskNext.PriorityDefNum).ItemName+".\r\n");
			}
			if(taskCur.IsNoteChange) { //Using taskOld because the notes changed from the old one to the new one.
				strb.Append(Lans.g("TaskHists","Task notes changed."));
			}
			return strb.ToString();
		}

		///<summary>Retrieves a user's name based on the passed userNum, otherwise return INVALID.</summary>
		public static string GetUserName(long userNum) {
			Userod user=Userods.GetUser(userNum);
			if(user==null) {
				return $"INVALID ({userNum})";
			}
			else {
				return user.UserName;
			}
		}

		///<summary></summary>
		public static long Insert(TaskHist taskHist){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				taskHist.TaskHistNum=Meth.GetLong(MethodBase.GetCurrentMethod(),taskHist);
				return taskHist.TaskHistNum;
			}
			return Crud.TaskHistCrud.Insert(taskHist);
		}

		///<summary>Updates TaskHist references when an old task is cut (not copied) and pasted somewhere so history is continuous.</summary>
		public static void UpdateTaskNums(Task oldTask,Task newTask) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),oldTask,newTask);
				return;
			}
			string command="UPDATE taskhist SET TaskNum="+POut.Long(newTask.TaskNum)+" WHERE TaskNum="+POut.Long(oldTask.TaskNum);
			Db.NonQ(command);
		}

		///<summary>Gets a list of task histories for a given taskNum.</summary>
		public static List<TaskHist> GetArchivesForTask(long taskNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TaskHist>>(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command="SELECT * FROM taskhist WHERE TaskNum="+POut.Long(taskNum)+" ORDER BY DateTStamp";
			return Crud.TaskHistCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long taskHistNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskHistNum);
				return;
			}
			Crud.TaskHistCrud.Delete(taskHistNum);
		}
	}
}