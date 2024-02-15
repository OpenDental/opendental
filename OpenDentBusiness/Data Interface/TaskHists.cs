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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<TaskHist>(MethodBase.GetCurrentMethod(),taskHistNum);
			}
			return Crud.TaskHistCrud.SelectOne(taskHistNum);
		}

		public static string GetChangesDescription(TaskHist taskHist,TaskHist taskHistNext) {
			if(taskHist.Descript.StartsWith("This task was cut from task list ") || taskHist.Descript.StartsWith("This task was copied from task ")) {
				return taskHist.Descript;
			}
			if(taskHist.DateTimeEntry==DateTime.MinValue) {
				return Lans.g("TaskHists","New task.");
			}
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.Append("");
			if(taskHistNext.TaskListNum!=taskHist.TaskListNum){
				string descOne=Lans.g("TaskHists","(DELETED)");
				string descTwo=Lans.g("TaskHists","(DELETED)");
				TaskList taskList=TaskLists.GetOne(taskHist.TaskListNum);
				if(taskList!=null) {
					descOne=taskList.Descript;
				}
				taskList=TaskLists.GetOne(taskHistNext.TaskListNum);
				if(taskList!=null) {
					descTwo=taskList.Descript;
				}
				stringBuilder.Append(Lans.g("TaskHists","Task list changed from")+" "+descOne+" "+Lans.g("TaskHists","to")+" "+descTwo+".\r\n");
			}
			if(taskHistNext.ObjectType!=taskHist.ObjectType){
				stringBuilder.Append(Lans.g("TaskHists","Task attachment changed from")+" "
					+taskHist.ObjectType.ToString()+" "+Lans.g("TaskHists","to")+" "+taskHistNext.ObjectType.ToString()+".\r\n");
			}
			if(taskHistNext.KeyNum!=taskHist.KeyNum){
				stringBuilder.Append(Lans.g("TaskHists","Task account attachment changed.")+"\r\n");
			}
			if(taskHistNext.Descript!=taskHist.Descript && !taskHistNext.Descript.StartsWith("This task was cut from task list ") 
				&& !taskHistNext.Descript.StartsWith("This task was copied from task "))
			{
				//We change the description of a task when it is cut/copied. 
				//This prevents the history grid from showing a description changed when it wasn't changed by the user.
				stringBuilder.Append(Lans.g("TaskHists","Task description changed.")+"\r\n");
			}
			if(taskHistNext.TaskStatus!=taskHist.TaskStatus){
				stringBuilder.Append(Lans.g("TaskHists","Task status changed from")+" "
					+taskHist.TaskStatus.ToString()+" "+Lans.g("TaskHists","to")+" "+taskHistNext.TaskStatus.ToString()+".\r\n");
			}
			if(taskHistNext.DateTimeEntry!=taskHist.DateTimeEntry){
				stringBuilder.Append(Lans.g("TaskHists","Task date added changed from")+" "
					+taskHist.DateTimeEntry.ToString()
					+" "+Lans.g("TaskHists","to")+" "
					+taskHistNext.DateTimeEntry.ToString()+".\r\n");
			}
			if(taskHistNext.UserNum!=taskHist.UserNum){
				stringBuilder.Append(Lans.g("TaskHists","Task author changed from ")+GetUserName(taskHist.UserNum)+" "
					+Lans.g("TaskHists","to")+" "+GetUserName(taskHistNext.UserNum)+".\r\n");
			}
			if(taskHistNext.DateTimeFinished!=taskHist.DateTimeFinished){
				stringBuilder.Append(Lans.g("TaskHists","Task date finished changed from")+" "
					+taskHist.DateTimeFinished.ToString()
					+" "+Lans.g("TaskHists","to")+" "
					+taskHistNext.DateTimeFinished.ToString()+".\r\n");
			}
			if(taskHistNext.PriorityDefNum!=taskHist.PriorityDefNum){
				stringBuilder.Append(Lans.g("TaskHists","Task priority changed from")+" "
					+Defs.GetDef(DefCat.TaskPriorities,taskHist.PriorityDefNum).ItemName
					+" "+Lans.g("TaskHists","to")+" "
					+Defs.GetDef(DefCat.TaskPriorities,taskHistNext.PriorityDefNum).ItemName+".\r\n");
			}
			if(taskHist.IsNoteChange) { //Using taskOld because the notes changed from the old one to the new one.
				stringBuilder.Append(Lans.g("TaskHists","Task notes changed."));
			}
			if(taskHist.IsReadOnly!=taskHistNext.IsReadOnly) {
				stringBuilder.Append(Lans.g("TaskHists","Task IsReadOnly changed from")+" "
					+taskHist.IsReadOnly.ToString()
					+" "+Lans.g("TaskHists","to")+" "
					+taskHistNext.IsReadOnly.ToString()+".\r\n");
			}
			return stringBuilder.ToString();
		}

		///<summary>Retrieves a user's name based on the passed userNum, otherwise return INVALID.</summary>
		public static string GetUserName(long userNum) {
			Userod userod=Userods.GetUser(userNum);
			if(userod==null) {
				return $"INVALID ({userNum})";
			}
			return userod.UserName;
		}

		///<summary></summary>
		public static long Insert(TaskHist taskHist){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				taskHist.TaskHistNum=Meth.GetLong(MethodBase.GetCurrentMethod(),taskHist);
				return taskHist.TaskHistNum;
			}
			return Crud.TaskHistCrud.Insert(taskHist);
		}

		///<summary>Updates TaskHist references when an old task is cut (not copied) and pasted somewhere so history is continuous.</summary>
		public static void UpdateTaskNums(Task taskOld,Task taskNew) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskOld,taskNew);
				return;
			}
			string command="UPDATE taskhist SET TaskNum="+POut.Long(taskNew.TaskNum)+" WHERE TaskNum="+POut.Long(taskOld.TaskNum);
			Db.NonQ(command);
		}

		///<summary>Gets a list of task histories for a given taskNum.</summary>
		public static List<TaskHist> GetArchivesForTask(long taskNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskHist>>(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command="SELECT * FROM taskhist WHERE TaskNum="+POut.Long(taskNum)+" ORDER BY DateTStamp";
			return Crud.TaskHistCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Delete(long taskHistNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskHistNum);
				return;
			}
			Crud.TaskHistCrud.Delete(taskHistNum);
		}
	}
}