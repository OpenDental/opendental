using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskNotes{
		#region Misc Methods
		///<summary>Returns true if there are any rows that have a Note with char length greater than 65,535</summary>
		public static bool HasAnyLongNotes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM tasknote WHERE CHAR_LENGTH(tasknote.Note)>65535";
			return (Db.GetCount(command)!="0");
		}
		#endregion

		///<summary>A list of notes for one task, ordered by datetime.</summary>
		public static List<TaskNote> GetForTask(long taskNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TaskNote>>(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command="SELECT * FROM tasknote WHERE TaskNum = "+POut.Long(taskNum)+" ORDER BY DateTimeNote";
			return Crud.TaskNoteCrud.SelectMany(command);
		}

		///<summary>A list of notes for many tasks.</summary>
		public static List<TaskNote> GetForTasks(List<long> taskNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TaskNote>>(MethodBase.GetCurrentMethod(),taskNums);
			}
			if(taskNums==null || taskNums.Count==0) {
				return new List<TaskNote>();
			}
			string command = "SELECT * FROM tasknote WHERE TaskNum IN ("+string.Join(",",taskNums)+")";
			return Crud.TaskNoteCrud.SelectMany(command);
		}
		
		///<summary>A list of notes for multiple tasks, ordered by date time.</summary>
		public static List<TaskNote> RefreshForTasks(List<long> taskNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TaskNote>>(MethodBase.GetCurrentMethod(),taskNums);
			}
			if(taskNums.Count==0){
				return new List<TaskNote>();
			}
			string command="SELECT * FROM tasknote WHERE TaskNum IN (";
			for(int i=0;i<taskNums.Count;i++){
				if(i>0) {
					command+=",";
				}
				command+=POut.Long(taskNums[i]);
			}
			command+=") ORDER BY DateTimeNote";
			return Crud.TaskNoteCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(TaskNote taskNote){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				taskNote.TaskNoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),taskNote);
				return taskNote.TaskNoteNum;
			}
			return Crud.TaskNoteCrud.Insert(taskNote);
		}

		///<summary></summary>
		public static void Update(TaskNote taskNote){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNote);
				return;
			}
			Crud.TaskNoteCrud.Update(taskNote);
		}

		///<summary></summary>
		public static void Delete(long taskNoteNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNoteNum);
				return;
			}
			string command= "DELETE FROM tasknote WHERE TaskNoteNum = "+POut.Long(taskNoteNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary>Gets one TaskNote from the db.</summary>
		public static TaskNote GetOne(long taskNoteNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TaskNote>(MethodBase.GetCurrentMethod(),taskNoteNum);
			}
			return Crud.TaskNoteCrud.SelectOne(taskNoteNum);
		}

		

	
		*/
	}
}