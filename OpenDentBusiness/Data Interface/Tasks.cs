using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary>Not part of cache refresh.</summary>
	public class Tasks {
		#region Misc Methods
		///<summary>Returns true if there are any rows that have a Descript with char length greater than 65,535</summary>
		public static bool HasAnyLongDescripts() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM task WHERE CHAR_LENGTH(task.Descript)>65535";
			return (Db.GetCount(command)!="0");
		}

		///<summary>Returns true if task does not exist in the database.</summary>
		public static bool IsTaskDeleted(long taskNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command = "SELECT COUNT(*) FROM task WHERE TaskNum="+POut.Long(taskNum)+"";
			return Db.GetCount(command)=="0";
		}

		///<summary>Returns true if task is a Reminder Task.</summary>
		public static bool IsReminderTask(Task task) {
			//No remoting role check; no call to db
			if(!PrefC.GetBool(PrefName.TasksUseRepeating) && !String.IsNullOrEmpty(task.ReminderGroupId) 
				&& task.ReminderType!=TaskReminderType.NoReminder) 
			{
				return true;
			}
			return false;
		}
		#endregion

		///<summary>Defines delegate signature to be used for Tasks.NavTaskDelegate.</summary>
		public delegate void NavToTaskDelegate(long taskNum);
		///<summary>Sent in from FormOpenDental. Allows static method for business layer to cause task navigation in FormOpenDental.</summary>
		public static NavToTaskDelegate NavTaskDelegate;
		///<summary>Internal HQ only, FK to tasklist.TaskListNum for the Triage task list.</summary>
		public const long TriageTaskListNum=1697;
		///<summary>Internal HQ only, FK to the definition.DefNum for the task priority color for blue.</summary>
		public const long TriageBlueNum=502;
		///<summary>Internal HQ only, FK to the definition.DefNum for the task priority color for red.</summary>
		public const long RedTaskDefNum=501;
		///<summary>Internal HQ only, FK to tasklist.TaskListNum for the Office Down task list.</summary>
		public const long OfficeDownTaskListNum=2576;
		private static bool _isHQ;
		private static long _defaultTaskPriorityDefNum;
		private static bool _isSortApptDateTime=false;
		///<summary>Key=AptNum, Value=AptDateTime</summary>
		private static Dictionary<long,DateTime> _dictTaskApts;

		///<summary>Only used from UI.</summary>
		public static ArrayList LastOpenList;
		///<summary>Only used from UI.  The index of the last open tab.</summary>
		public static int LastOpenGroup;
		///<summary>Only used from UI.</summary>
		public static DateTime LastOpenDate;

		///<summary>This is needed because of the extra column that is not part of the database.</summary>
		private static List<Task> TableToList(DataTable table) {
			//No need to check RemotingRole; no call to db.
			List<Task> retVal=Crud.TaskCrud.TableToList(table);
			for(int i=0;i<retVal.Count;i++) {
				if(table.Columns.Contains("IsUnread")) {
					retVal[i].IsUnread=PIn.Bool(table.Rows[i]["IsUnread"].ToString());//1 or more will result in true.
				}
				if(table.Columns.Contains("ParentDesc")) {
					retVal[i].ParentDesc=PIn.String(table.Rows[i]["ParentDesc"].ToString());
				}
				if(table.Columns.Contains("LName")
					&& table.Columns.Contains("FName")
					&& table.Columns.Contains("Preferred")
					) 
				{
					string lname=PIn.String(table.Rows[i]["LName"].ToString());
					string fname=PIn.String(table.Rows[i]["FName"].ToString());
					string preferred=PIn.String(table.Rows[i]["Preferred"].ToString());
					retVal[i].PatientName=Patients.GetNameLF(lname,fname,preferred,"");
				}
			}
			return retVal;
		}

		/*
		///<summary>There are NO tasks on the user trunk, so this is not needed.</summary>
		public static List<Task> RefreshUserTrunk(int userNum) {
			string command="SELECT task.* FROM tasksubscription "
				+"LEFT JOIN task ON task.TaskNum=tasksubscription.TaskNum "
				+"WHERE tasksubscription.UserNum="+POut.PInt(userNum)
				+" AND tasksubscription.TaskNum!=0 "
				+"ORDER BY DateTimeEntry";
			return RefreshAndFill(command);
		}*/

		///<summary>Gets one Task from database.</summary>
		public static Task GetOne(long TaskNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Task>(MethodBase.GetCurrentMethod(),TaskNum);
			}
			string command="SELECT * FROM task WHERE TaskNum = "+POut.Long(TaskNum);
			return Crud.TaskCrud.SelectOne(command);
		}
		
		///<summary>Gets all tasks for the Task Search function, limited to 50 by default.</summary>
		public static DataTable GetDataSet(long userNum,List<long> listTaskListNums,List<long> listTaskNums,string taskDateCreatedFrom,string taskDateCreatedTo,
			string taskDateCompletedFrom,string taskDateCompletedTo,string taskDescription,long taskPriorityNum,long patNum,bool doIncludeTaskNote,
			bool doIncludeCompleted,bool limit,bool doRunOnReportServer) 
		{			
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),userNum,listTaskListNums,listTaskNums,taskDateCreatedFrom,taskDateCreatedTo,
					taskDateCompletedFrom,taskDateCompletedTo,taskDescription,taskPriorityNum,patNum,doIncludeTaskNote,doIncludeCompleted,limit,
					doRunOnReportServer);
			}
			DateTime dateCreatedFrom=PIn.Date(taskDateCreatedFrom);//will be DateTime.MinValue if not set, i.e. if " "
			DateTime dateCreatedTo=PIn.Date(taskDateCreatedTo);//will be DateTime.MinValue if not set, i.e. if " "
			DateTime dateCompletedFrom=PIn.Date(taskDateCompletedFrom);//will be DateTime.MinValue if not set, i.e. if " "
			DateTime dateCompletedTo=PIn.Date(taskDateCompletedTo);//will be DateTime.MinValue if not set, i.e. if " "
			List<long> listSearchTaskNums=ReportsComplex.RunFuncOnReportServer(() => GetTasksNumsForSearch(userNum,listTaskListNums,listTaskNums,dateCreatedFrom,
					dateCreatedTo,dateCompletedFrom,dateCompletedTo,taskDescription,taskPriorityNum,patNum,doIncludeTaskNote,doIncludeCompleted,limit),
				doRunOnReportServer);			
			DataTable table=new DataTable();
			table.Columns.Add(new DataColumn("description"));
			table.Columns.Add(new DataColumn("note"));
			table.Columns.Add(new DataColumn("PatNum"));
			table.Columns.Add(new DataColumn("procTime"));
			table.Columns.Add(new DataColumn("dateCreate"));
			table.Columns.Add(new DataColumn("dateComplete"));
			table.Columns.Add(new DataColumn("TaskNum"));
			table.Columns.Add(new DataColumn("color"));
			if(listSearchTaskNums.Count==0) {
				return table;//empty table with correct structure.
			}
			//listTaskNums contains too many items. Tasks found from matching task notes must be filtered too. (This prevents a costly join in the query.)
			List<Task> listTasks=ReportsComplex.RunFuncOnReportServer(() => Tasks.GetMany(listSearchTaskNums),doRunOnReportServer)//All tasks for the notes and tasks
				.Where(x => listTaskListNums.Count==0 || listTaskListNums.Contains(x.TaskListNum))//filter by TaskListNum, if neccesary
				.Where(x => taskPriorityNum==0 || taskPriorityNum==x.PriorityDefNum)//filter by priority, if neccesary
				.Where(x => patNum==0 || (x.ObjectType==TaskObjectType.Patient && x.KeyNum==patNum))//filter by patnum, if neccesary
				.Where(x => dateCompletedFrom==DateTime.MinValue || x.DateTimeFinished.Date>=dateCompletedFrom.Date)//filter by dateFrom, if neccesary
				.Where(x => dateCompletedTo==DateTime.MinValue || x.DateTimeFinished.Date<=dateCompletedTo.Date)
				.OrderByDescending(x=> (x.DateTimeOriginal==DateTime.MinValue ? x.DateTimeEntry : x.DateTimeOriginal)).ToList();//Order results by DateTimeOriginal if it exists, DateTimeEntry if not
			List<TaskNote> listTaskNotes=new List<TaskNote>();
			if(doIncludeTaskNote) {
				//All notes for the tasks.	(Ordered by dateTime)		
				listTaskNotes=ReportsComplex.RunFuncOnReportServer(() => TaskNotes.RefreshForTasks(listSearchTaskNums),doRunOnReportServer);
			}
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors,true);
			int textColor=Defs.GetColor(DefCat.ProgNoteColors,listDefs[18].DefNum).ToArgb();//18="Patient Note Text"
			int textCompletedColor=Defs.GetColor(DefCat.ProgNoteColors,listDefs[20].DefNum).ToArgb();//20="Completed Pt Note Text"
			List<TaskList> listTaskLists=ReportsComplex.RunFuncOnReportServer(() => TaskLists.GetMany(listTaskListNums),doRunOnReportServer);
			string txt;
			DataRow row;
			foreach(Task taskCur in listTasks) {
				txt="";
				row=table.NewRow();
				//Build data row
				if(taskCur.TaskStatus==TaskStatusEnum.Done) {
					row["color"]=textCompletedColor;
					txt+=Lans.g("TaskSearch","Completed")+" ";
				}
				else {
					row["color"]=textColor;
				}
				if(taskCur.TaskListNum!=0) {
					row["description"]=txt+Lans.g("TaskSearch","In List")+": "+TaskLists.GetFullPath(taskCur.TaskListNum,listTaskLists);
				}
				else {
					row["description"]=txt+Lans.g("TaskSearch","Not in list");
				}
				txt="";
				if(!taskCur.Descript.StartsWith("==") && taskCur.UserNum!=0) {
					txt+=Userods.GetName(PIn.Long(taskCur.UserNum.ToString()))+" - ";
				}
				txt+=taskCur.Descript;
				if(doIncludeTaskNote) {
					listTaskNotes.FindAll(x => x.TaskNum==taskCur.TaskNum)
					.ForEach(x => txt+="\r\n"//even on the first loop
						+"=="+Userods.GetName(x.UserNum)+" - "
						+x.DateTimeNote.ToShortDateString()+" "
						+x.DateTimeNote.ToShortTimeString()
						+" - "+x.Note);
				}
				row["note"]=txt;
				if(taskCur.ObjectType==TaskObjectType.Patient) {
					row["PatNum"]=taskCur.KeyNum;
				}
				if(taskCur.DateTask.Year>1880) {//check if due date set for task or note
					row["dateCreate"]=taskCur.DateTask.ToString(Lans.GetShortDateTimeFormat());
				}
				else if(taskCur.DateTimeOriginal.Year>1880) {
					row["dateCreate"]=taskCur.DateTimeOriginal.ToShortDateString();
				}
				if(taskCur.DateTask.TimeOfDay!=TimeSpan.Zero) {
					row["procTime"]=taskCur.DateTask.ToString("h:mm")+taskCur.DateTask.ToString("%t").ToLower();
				}
				else if(taskCur.DateTimeEntry.TimeOfDay!=TimeSpan.Zero) {
					row["procTime"]=taskCur.DateTimeEntry.ToString("h:mm")+taskCur.DateTimeEntry.ToString("%t").ToLower();
				}
				if(taskCur.DateTimeFinished.Year>1880) {
					row["dateComplete"]=taskCur.DateTimeFinished.ToString(Lans.GetShortDateTimeFormat());
				}
				row["TaskNum"]=taskCur.TaskNum;
				table.Rows.Add(row);
			}
			return table;
		}

		///<summary>Gets the task nums for the tasks based on the search parameters passed in.</summary>
		public static List<long> GetTasksNumsForSearch(long userNum,List<long> listTaskListNums,List<long> listTaskNums,DateTime dateCreatedFrom,
			DateTime dateCreatedTo,DateTime dateCompletedFrom,DateTime dateCompletedTo,string taskDescription,long taskPriorityNum,long patNum,
			bool doIncludeTaskNote,bool doIncludeCompleted,bool limit) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),userNum,listTaskListNums,listTaskNums,dateCreatedFrom,dateCreatedTo,
					dateCompletedFrom,dateCompletedTo,taskDescription,taskPriorityNum,patNum,doIncludeTaskNote,doIncludeCompleted,limit);
			}
			List<string> listWhereClauses=new List<string>();
			List<string> listWhereNoteClauses=new List<string>();
			bool doJoinTaskOnTaskNote=false;
			if(!doIncludeCompleted) {
				listWhereClauses.Add("task.TaskStatus!="+POut.Long((int)TaskStatusEnum.Done));
				listWhereNoteClauses.Add("task.TaskStatus!="+POut.Long((int)TaskStatusEnum.Done));
				doJoinTaskOnTaskNote=true;
			}
			if(userNum!=0) {
				listWhereClauses.Add("task.UserNum="+POut.Long(userNum));
				listWhereNoteClauses.Add("tasknote.UserNum="+POut.Long(userNum));
			}
			if(listTaskListNums.Count>0) {
				listWhereClauses.Add("task.TaskListNum IN("+string.Join(",",listTaskListNums)+")");
				listWhereNoteClauses.Add("task.TaskListNum IN("+string.Join(",",listTaskListNums)+")");
				doJoinTaskOnTaskNote=true;
			}
			if(listTaskNums.Count>0) {
				listWhereClauses.Add("task.TaskNum IN ("+string.Join(",",listTaskNums.Select(x => POut.Long(x)))+")");
				listWhereNoteClauses.Add("tasknote.TaskNum IN ("+string.Join(",",listTaskNums.Select(x => POut.Long(x)))+")");
			}
			//Note: DateTime strings that are empty actually are " " due to how the empty datetime control behaves.
			if(dateCreatedFrom>DateTime.MinValue) {
				listWhereClauses.Add("DATE(task.DateTimeOriginal)>="+POut.Date(dateCreatedFrom)); 
				listWhereNoteClauses.Add("DATE(tasknote.DateTimeNote)>="+POut.Date(dateCreatedFrom)); 
			}
			if(dateCreatedTo>DateTime.MinValue) {
				listWhereClauses.Add("DATE(task.DateTimeOriginal)<="+POut.Date(dateCreatedTo)); 
				listWhereNoteClauses.Add("DATE(tasknote.DateTimeNote)<="+POut.Date(dateCreatedTo)); 
			}
			if(dateCompletedFrom>DateTime.MinValue) {
				listWhereClauses.Add("DATE(task.DateTimeFinished)>="+POut.Date(dateCompletedFrom));
				listWhereNoteClauses.Add("DATE(task.DateTimeFinished)>="+POut.Date(dateCompletedFrom));
				doJoinTaskOnTaskNote=true;
			}
			if(dateCompletedTo>DateTime.MinValue) {
				listWhereClauses.Add("DATE(task.DateTimeFinished)<="+POut.Date(dateCompletedTo));
				listWhereNoteClauses.Add("DATE(task.DateTimeFinished)<="+POut.Date(dateCompletedTo));
				doJoinTaskOnTaskNote=true;
			}
			if(taskDescription!="") {
        foreach(string param in taskDescription.Split(' ')) {
				  listWhereClauses.Add("task.Descript LIKE '%"+POut.String(param)+"%'");
				  listWhereNoteClauses.Add("tasknote.Note LIKE '%"+POut.String(param)+"%'");
        }
			}
			if(taskPriorityNum!=0) {
				listWhereClauses.Add("task.PriorityDefNum="+POut.Long(taskPriorityNum));
				listWhereNoteClauses.Add("task.PriorityDefNum="+POut.Long(taskPriorityNum));
				doJoinTaskOnTaskNote=true;
			}
			if(patNum!=0) {
				listWhereClauses.Add("task.ObjectType="+POut.Int((int)TaskObjectType.Patient));
				listWhereClauses.Add("task.KeyNum="+POut.Long(patNum));
				listWhereNoteClauses.Add("task.ObjectType="+POut.Int((int)TaskObjectType.Patient));
				listWhereNoteClauses.Add("task.KeyNum="+POut.Long(patNum));
				doJoinTaskOnTaskNote=true;
			}
			string whereClause="";
			if(listWhereClauses.Count>0) {
				whereClause="WHERE "+string.Join(" AND ",listWhereClauses)+" ";
			}
			string noteJoinClause="";
			if(doJoinTaskOnTaskNote) {
				noteJoinClause="INNER JOIN task ON tasknote.TaskNum=task.TaskNum ";
			}
			string whereNoteClause="";
			if(listWhereNoteClauses.Count>0 && doIncludeTaskNote) {
				whereNoteClause="WHERE "+string.Join(" AND ",listWhereNoteClauses)+" ";
			}
			string command="SELECT TaskNum FROM ("
				+"(SELECT task.TaskNum,task.DateTimeOriginal DateTimeSearch "
				+"FROM task "
				+whereClause
				+")";
			if(doIncludeTaskNote) {
				command+=" UNION "
				+"(SELECT tasknote.TaskNum,tasknote.DateTimeNote DateTimeSearch "
					+"FROM tasknote "
					+noteJoinClause
					+whereNoteClause
				+")";
			}
			command+=") tasksearch GROUP BY tasksearch.TaskNum ORDER BY MIN(tasksearch.DateTimeSearch) DESC ";
			if(limit) {
				command+=" LIMIT 50";
			}
			return Db.GetListLong(command);
		}

		///<summary>Gets all tasks for a supplied list of task nums.</summary>
		public static List<Task> GetMany(List<long> listTaskNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),listTaskNums);
			}
			if(listTaskNums==null || listTaskNums.Count==0) {
				return new List<Task>();
			}
			string command="SELECT * FROM task WHERE TaskNum IN("+String.Join(",",listTaskNums)+") ORDER BY DateTimeEntry";
			return Crud.TaskCrud.SelectMany(command);
		}

		///<summary>Gets the count of reminder tasks on or after the specified dateTimeAsOf.</summary>
		public static int GetCountReminderTasks(string reminderGroupId,DateTime dateTimeAsOf) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),reminderGroupId,dateTimeAsOf);
			}
			string command="SELECT COUNT(*) FROM task "
				+"WHERE task.ReminderGroupId='"+POut.String(reminderGroupId)+"' AND DateTimeEntry > "+POut.DateT(dateTimeAsOf);
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>After a refresh, this is used to determine whether the Current user has received any new tasks through subscription.
		///Must supply the current usernum.  If the listTaskNums is null, then all subscribed tasks for the user will be returned.
		///The signal list will include any task changes including status changes and deletions.</summary>
		public static List<Task> GetNewTasksThisUser(long userNum,long clinicNum,List<long> listTaskNums=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),userNum,clinicNum,listTaskNums);
			}
			Logger.LogToPath("",LogPath.Signals,LogPhase.Start);
			if(userNum==0) {
				return new List<Task>();//Return early because userNum is invalid.
			}
			if(listTaskNums!=null && listTaskNums.Count==0) {//no task popup signals
				return new List<Task>();//Return early to avoid running a query.
			}
			string command="SELECT task.*,CASE WHEN(taskunread.TaskNum IS NOT NULL) THEN 1 ELSE 0 END IsUnread "
			+"FROM taskancestor "
			+"INNER JOIN task ON task.TaskNum=taskancestor.TaskNum AND TaskStatus != "+POut.Int((int)TaskStatusEnum.Done)+" ";
			if(listTaskNums!=null) {
				command+="AND task.TaskNum IN ("+String.Join(",",listTaskNums)+") ";
			}
			command+=
			"INNER JOIN tasklist ON tasklist.TaskListNum=taskancestor.TaskListNum "
			+"INNER JOIN tasksubscription ON tasksubscription.TaskListNum=tasklist.TaskListNum AND tasksubscription.UserNum="+POut.Long(userNum)+" "
			+"LEFT JOIN taskunread ON taskunread.TaskNum=task.TaskNum AND taskunread.UserNum="+POut.Long(userNum);
			if(Clinics.ClinicNum!=0 || !PrefC.HasClinicsEnabled) {
				command+=TaskLists.BuildFilterJoins(clinicNum);
				command+=" WHERE TRUE "+TaskLists.BuildFilterWhereClause(userNum,clinicNum,Clinics.GetClinic(clinicNum)?.Region??0);
			}
			List<Task> ret=TableToList(Db.GetTable(command));//This is how we set the IsUnread column.
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
			return ret;
		}

		///<summary>Gets a string using the aptNum as the key. String consits of patient name and some appointment information.</summary>
		public static SerializableDictionary<long,string> GetApptObjDescripts(List<long> listPatApts){
			if(listPatApts.Count==0) {
				return new SerializableDictionary<long,string>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetSerializableDictionary<long,string>(MethodBase.GetCurrentMethod(),listPatApts);
			}
			string command=@"SELECT patient.LName,patient.FName,patient.Preferred,patient.MiddleI,appointment.AptNum,appointment.AptDateTime,
				appointment.ProcDescript,appointment.Note 
				FROM appointment 
				INNER JOIN patient ON patient.PatNum=appointment.PatNum 
				WHERE appointment.AptNum IN ("+string.Join(",",listPatApts)+")";
			DataTable table= Db.GetTable(command);
			SerializableDictionary<long,string> dictTaskString=new SerializableDictionary<long, string>();
			foreach(DataRow aptRow in table.Rows) {
				string patname=Patients.GetNameLF(aptRow["LName"].ToString(),aptRow["FName"].ToString()
					,aptRow["Preferred"].ToString(),aptRow["MiddleI"].ToString()); //no call to db
				Appointment apt=new Appointment();
				apt.AptNum=PIn.Long(aptRow["AptNum"].ToString());
				apt.AptDateTime=PIn.DateT(aptRow["AptDateTime"].ToString());
				apt.ProcDescript=PIn.String(aptRow["ProcDescript"].ToString());
				apt.Note=PIn.String(aptRow["Note"].ToString());
				dictTaskString.Add(apt.AptNum,patname+" "+apt.AptDateTime.ToString()+" "+apt.ProcDescript+" "+apt.Note+" - ");
			}
			return dictTaskString;
		}

		///<summary>Sets the task.ReminderGroupId to a brand new and unique value.</summary>
		public static void SetReminderGroupId(Task task) {
			//No need to check RemotingRole; no call to db.
			task.ReminderGroupId=CodeBase.MiscUtils.CreateRandomAlphaNumericString(20);
			while(Tasks.GetCountReminderTasks(task.ReminderGroupId,DateTime.MinValue) > 0) {//Verify that the new group id does not exist.
				task.ReminderGroupId=CodeBase.MiscUtils.CreateRandomAlphaNumericString(20);
			}
		}

		///<summary>Gets all tasks for the main trunk.</summary>
		public static List<Task> RefreshMainTrunk(bool showDone,DateTime startDate,long currentUserNum,TaskType taskType
			,GlobalTaskFilterType globalFilterType,List<long> listFilterFkeys=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),showDone,startDate,currentUserNum,taskType,globalFilterType,listFilterFkeys);
			}
			//startDate only applies if showing Done tasks.
			string command="SELECT task.*,"
					+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum AND taskunread.UserNum="+POut.Long(currentUserNum)+") IsUnread, "
					+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE TaskListNum=0 "
				+"AND DateTask < "+POut.Date(new DateTime(1880,01,01))+" "
				+"AND IsRepeating=0 ";
			if(taskType==TaskType.Reminder) {
				command+="AND COALESCE(task.ReminderGroupId,'') != '' ";//reminders only
			}
			else if(taskType==TaskType.Normal) {
				command+="AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") ";//no future reminders
			}
			else {
				//No filter.
			}
			if(showDone){
				command+=" AND (TaskStatus !="+POut.Long((int)TaskStatusEnum.Done)
					+" OR DateTimeFinished > "+POut.Date(startDate)+")";//of if done, then restrict date
			}
			else{
				command+=" AND TaskStatus !="+POut.Long((int)TaskStatusEnum.Done);
			}
			command+=BuildFilterWhereClause(currentUserNum,globalFilterType,listFilterFkeys);
			command+=" ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all 'new' tasks for a user.</summary>
		public static List<Task> RefreshUserNew(long userNum,GlobalTaskFilterType globalFilterType,List<long> listFilterFkey=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),userNum,globalFilterType,listFilterFkey);
			}
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT task.*,1 AS IsUnread,";//we fill the IsUnread column with 1's because we already know that they are all unread
			}
			else {//Oracle
				//Since this statement has a GROUP BY clause and the table has a clob column, we have to do some Oracle magic with the descript column.
				command="SELECT task.TaskNum,task.TaskListNum,task.DateTask,task.KeyNum,(SELECT Descript FROM task taskdesc WHERE task.TaskNum=taskdesc.TaskNum) Descript,task.TaskStatus"
					+",task.IsRepeating,task.DateType,task.FromNum,task.ObjectType,task.DateTimeEntry,task.UserNum,task.DateTimeFinished"
					+",1 AS IsUnread,";//we fill the IsUnread column with 1's because we already know that they are all unread
			}
			command+="tasklist.Descript ParentDesc, "	/*Renamed to keep same column name as old query*/
					+"patient.LName,patient.FName,patient.Preferred, "
					+"COALESCE(MAX(tasknote.DateTimeNote),task.DateTimeEntry) AS 'LastUpdated' "
				+"FROM task "
				+"INNER JOIN taskunread ON task.TaskNum=taskunread.TaskNum "
					+"AND taskunread.UserNum = "+POut.Long(userNum)+" "
				+"LEFT JOIN tasklist ON task.TaskListNum=tasklist.TaskListNum "
				+"LEFT JOIN tasknote ON task.TaskNum=tasknote.TaskNum "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum "
					+"AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") "//no future reminders
				+"AND task.TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
			command+=BuildFilterWhereClause(userNum,globalFilterType,listFilterFkey);
			command+="GROUP BY task.TaskNum "//in case there are duplicate unreads
				+"ORDER BY task.DateTimeEntry";
			DataTable table=Db.GetTable(command);
			List<DataRow> listRows=new List<DataRow>();
			for(int i=0;i<table.Rows.Count;i++) {
				listRows.Add(table.Rows[i]);
			}
			#region Set Sort Variables. This greatly increases sort speed.
			_isHQ=PrefC.GetBool(PrefName.DockPhonePanelShow);//increases speed of the sort function performed below.
			List<Def> listTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);
			for(int i=0;i<listTaskPriorities.Count;i++) {
				if(listTaskPriorities[i].ItemValue.ToUpper()=="D") {
					_defaultTaskPriorityDefNum=listTaskPriorities[i].DefNum;
					break;
				}
			}
			#endregion
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TaskPriorities);
			List<TaskCompareObj> listTaskCompareObjs=new List<TaskCompareObj>();
			foreach(DataRow row in listRows) {
				listTaskCompareObjs.Add(new TaskCompareObj() {
					RowTask=row,
					ListTaskPriorityDefs=listDefs,
				});
			}
			listTaskCompareObjs.Sort(TaskComparer);
			DataTable tableSorted=table.Clone();//Easy way to copy the columns.
			tableSorted.Rows.Clear();
			for(int i=0;i<listTaskCompareObjs.Count;i++) {
				tableSorted.Rows.Add(listTaskCompareObjs[i].RowTask.ItemArray);
			}
			List<Task> listTasks=TableToList(tableSorted);
			return listTasks;
		}

		///<summary>Gets all 'open ticket' tasks for a user.  An open ticket is a task that was created by this user, is attached to a patient, and is not done.</summary>
		public static List<Task> RefreshOpenTickets(long userNum,GlobalTaskFilterType globalFilterType,List<long> listFilterFkeys=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),userNum,globalFilterType,listFilterFkeys);
			}
			string command="SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
				+"AND taskunread.UserNum="+POut.Long(userNum)+") AS IsUnread, "
				+"tasklist.Descript AS ParentDesc, "
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN tasklist ON task.TaskListNum=tasklist.TaskListNum "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum "
					+"AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE NOT EXISTS( "
					+"SELECT * FROM taskancestor "
					+"LEFT JOIN tasklist ON tasklist.TaskListNum=taskancestor.TaskListNum "
					+"WHERE taskancestor.TaskNum=task.TaskNum "
					+"AND tasklist.DateType!=0) "//if any ancestor is a dated list, then we don't want that task
				+"AND task.DateType=0 "//this only handles tasks directly in the dated trunks
				+"AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"AND task.IsRepeating=0 "
				+"AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") "//no future reminders
				+"AND task.UserNum="+POut.Long(userNum)+" "
				+"AND TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
			command+=BuildFilterWhereClause(userNum,globalFilterType,listFilterFkeys);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all 'open ticket' tasks for a patient.  An open ticket is a task that was created with the attached patient and is not done.</summary>
		public static List<Task> RefreshPatientTickets(long patNum,long currentUserNum=0,
			GlobalTaskFilterType globalFilterType=GlobalTaskFilterType.None,List<long> listFilterFkeys=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),patNum,currentUserNum,globalFilterType,listFilterFkeys);
			}
			string command="SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
				+"AND taskunread.UserNum="+POut.Long(Security.CurUser.UserNum)+") AS IsUnread, "
				+"tasklist.Descript AS ParentDesc "
				+"FROM task "
				+"LEFT JOIN tasklist ON task.TaskListNum=tasklist.TaskListNum ";
			command+=BuildFilterJoins(globalFilterType,false);
			command+="WHERE task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"AND task.KeyNum="+POut.Long(patNum)+" "
				+"AND TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
			command+=BuildFilterWhereClause(currentUserNum,globalFilterType,listFilterFkeys);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all tasks for the repeating trunk.  Always includes "done".</summary>
		public static List<Task> RefreshRepeatingTrunk(long currentUserNum,GlobalTaskFilterType globalFilterType,List<long> listFilterFkeys=null) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),currentUserNum,globalFilterType,listFilterFkeys);
			}
			string command="SELECT task.*, "
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE TaskListNum=0 "
				+"AND DateTask < "+POut.Date(new DateTime(1880,01,01))+" "
				+"AND IsRepeating=1 "
				+"AND COALESCE(task.ReminderGroupId,'')='' ";//no reminders
			command+=BuildFilterWhereClause(currentUserNum,globalFilterType,listFilterFkeys);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>0 is not allowed, because that would be a trunk.  
		///Also, if this is in someone's inbox, then pass in the userNum whose inbox it is in.  If not in an inbox, pass in 0.</summary>
		public static List<Task> RefreshChildren(long listNum,bool showDone,DateTime startDate,long currentUserNum,long userNumInbox,TaskType taskType,
			GlobalTaskFilterType globalFilterType=GlobalTaskFilterType.None,List<long> listFilterFkey=null)
		{
			return RefreshChildren(listNum,showDone,startDate,currentUserNum,userNumInbox,taskType,false,globalFilterType,listFilterFkey);
		}

		///<summary>0 is not allowed, because that would be a trunk.
		///Also, if this is in someone's inbox, then pass in the userNum whose inbox it is in.  If not in an inbox, pass in 0.
		///If isTaskSortApptDateTime==true and parent task list is an appointment type task list, TaskComparer oders appointment tasks to the top and
		///then by AptDateTime.</summary>
		public static List<Task> RefreshChildren(long listNum,bool showDone,DateTime startDate,long currentUserNum,long userNumInbox,TaskType taskType,
			bool isTaskSortApptDateTime,GlobalTaskFilterType globalFilterType,List<long> listFilterFkey=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),listNum,showDone,startDate,currentUserNum,userNumInbox,taskType,
					isTaskSortApptDateTime,globalFilterType,listFilterFkey);
			}
			//startDate only applies if showing Done tasks.
			string command="SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum ";//the count turns into a bool
			//if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {//we don't bother with this.  Always get IsUnread
			//if a task is someone's inbox,
			if(userNumInbox>0) {
				//then restrict by that user
				command+="AND taskunread.UserNum="+POut.Long(userNumInbox)+") IsUnread, ";
			}
			else {
				//otherwise, restrict by current user
				command+="AND taskunread.UserNum="+POut.Long(currentUserNum)+") IsUnread, ";
			}
			command+="patient.LName,patient.FName,patient.Preferred, "
							+"COALESCE(MAX(tasknote.DateTimeNote),task.DateTimeEntry) AS 'LastUpdated',"
							+"CASE WHEN tasknote.TaskNoteNum IS NULL THEN 0 ELSE 1 END AS 'HasNotes' "
							+"FROM task "
							+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
							+"LEFT JOIN tasknote ON task.TaskNum=tasknote.TaskNum ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE TaskListNum="+POut.Long(listNum)+" ";
			if(taskType==TaskType.Reminder) {
				command+="AND COALESCE(task.ReminderGroupId,'') != '' ";//reminders only
			}
			else if(taskType==TaskType.Normal){
				command+="AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") ";//no future reminders
			}
			else {
				//No filter.
			}
			if(showDone) {
				command+=" AND ((TaskStatus !="+POut.Long((int)TaskStatusEnum.Done)
					+" OR DateTimeFinished > "+POut.Date(startDate)+")" //or if done, then restrict date
					+" OR DateTimeFinished = '0001-01-01 00:00:00')"; //Include tasks that have a finished date time as MinValue so they can be edited.
			}
			else {
				command+=" AND TaskStatus !="+POut.Long((int)TaskStatusEnum.Done);
			}
			command+=BuildFilterWhereClause(currentUserNum,globalFilterType,listFilterFkey);
			command+=" GROUP BY task.TaskNum "//Sorting happens below
							+" ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			List<Task> taskList=new List<Task>();
			//Note: Only used for HQ, Oracle does not matter.
			List<DataRow> listRows=new List<DataRow>();
			for(int i=0;i<table.Rows.Count;i++) {
				listRows.Add(table.Rows[i]);
			}
			#region Set Sort Variables. This greatly increases sort speed.
			_isHQ=PrefC.GetBool(PrefName.DockPhonePanelShow);//increases speed of the sort function performed below.
			List<Def> listTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);
			for(int i=0;i<listTaskPriorities.Count;i++) {
				if(listTaskPriorities[i].ItemValue.ToUpper()=="D") {
					_defaultTaskPriorityDefNum=listTaskPriorities[i].DefNum;
					break;
				}
			}
			_isSortApptDateTime=false;
			if(isTaskSortApptDateTime) {
				TaskList parentTaskList=TaskLists.GetOne(listNum);
				if(parentTaskList!=null && parentTaskList.ObjectType==TaskObjectType.Appointment) {//If parent tasklist is an appointment tasklist.
					_isSortApptDateTime=true;//Sets flag for sorting
					//The LINQ below creates a dictionary with the key of AptNum and value of AptDateTime to use when sorting (from the KeyNum).
					//Look through the tasks and find a distinct list of any attached appointment AptNums.
					List<long> listAptNums = table.Select()
						.Where(x => PIn.Int(x["ObjectType"].ToString())==(int)TaskObjectType.Appointment && x["KeyNum"].ToString()!="0")
						.Select(x => PIn.Long(x["KeyNum"].ToString())).Distinct().ToList();
					_dictTaskApts=new Dictionary<long,DateTime>();//Clear the dictionary for good measure.
					if(listAptNums.Count>0) {//If there was at least one apt attached to the tasks in the table.
						//Fill the dictionary with the key of AptNum and the value of AptDateTime.
						_dictTaskApts=Appointments.GetAptDateTimeForAptNums(listAptNums)
							.Select()
							.ToDictionary(x => PIn.Long(x["AptNum"].ToString()),x => PIn.DateT(x["AptDateTime"].ToString()));
					}
				}
			}
			#endregion
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TaskPriorities);
			List<TaskCompareObj> listTaskCompareObjs=new List<TaskCompareObj>();
			foreach(DataRow row in listRows) {
				listTaskCompareObjs.Add(new TaskCompareObj() {
					RowTask=row,
					ListTaskPriorityDefs=listDefs,
				});
			}
			listTaskCompareObjs.Sort(TaskComparer);
			_dictTaskApts=null;//Clear the dictionary used for sorting
			_isSortApptDateTime=false;//Turn special sorting back off
			DataTable tableSorted=table.Clone();//Easy way to copy the columns.
			tableSorted.Rows.Clear();
			for(int i=0;i<listTaskCompareObjs.Count;i++) {
				tableSorted.Rows.Add(listTaskCompareObjs[i].RowTask.ItemArray);
			}
			taskList=TableToList(tableSorted);
			return taskList;
		}

		///<summary>All repeating items for one date type with no heirarchy.</summary>
		public static List<Task> RefreshRepeating(TaskDateType dateType,long currentUserNum,GlobalTaskFilterType globalFilterType,
			List<long> listFilterFkeys=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),dateType,currentUserNum,globalFilterType,listFilterFkeys);
			}
			string command=
				"SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
					+"AND taskunread.UserNum="+POut.Long(currentUserNum)+") IsUnread, "//Not sure if this makes sense here
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE IsRepeating=1 "
				+"AND COALESCE(task.ReminderGroupId,'')='' "//no reminders
				+"AND DateType="+POut.Long((int)dateType)+" ";
			command+=BuildFilterWhereClause(currentUserNum,globalFilterType,listFilterFkeys);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all tasks for one of the 3 dated trunks. startDate only applies if showing Done.</summary>
		public static List<Task> RefreshDatedTrunk(DateTime date,TaskDateType dateType,bool showDone,DateTime startDate,long currentUserNum
			,GlobalTaskFilterType globalFilterType,List<long> listFilterFkeys=null) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),date,dateType,showDone,startDate,currentUserNum,globalFilterType,listFilterFkeys);
			}
			DateTime dateFrom=DateTime.MinValue;
			DateTime dateTo=DateTime.MaxValue;
			if(dateType==TaskDateType.Day) {
				dateFrom=date;
				dateTo=date;
			}
			else if(dateType==TaskDateType.Week) {
				dateFrom=date.AddDays(-(int)date.DayOfWeek);
				dateTo=dateFrom.AddDays(6);
			}
			else if(dateType==TaskDateType.Month) {
				dateFrom=new DateTime(date.Year,date.Month,1);
				dateTo=dateFrom.AddMonths(1).AddDays(-1);
			}
			string command=
				"SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
					+"AND taskunread.UserNum="+POut.Long(currentUserNum)+") IsUnread, "//Not sure if this makes sense here
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(globalFilterType,true);
			command+="WHERE DateTask >= "+POut.Date(dateFrom)
				+" AND DateTask <= "+POut.Date(dateTo)
				+" AND DateType="+POut.Long((int)dateType)
				+" AND COALESCE(task.ReminderGroupId,'')='' ";//no reminders
			command+=BuildFilterWhereClause(currentUserNum,globalFilterType,listFilterFkeys);
			if(showDone){
				command+=" AND (TaskStatus !="+POut.Long((int)TaskStatusEnum.Done)
					+" OR DateTimeFinished > "+POut.Date(startDate)+")";//of if done, then restrict date
			}
			else{
				command+=" AND TaskStatus !="+POut.Long((int)TaskStatusEnum.Done);
			}
			command+=" ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Builds JOIN clauses appropriate to the type of GlobalFilterType.  Returns empty string if not filtering.</summary>
		private static string BuildFilterJoins(GlobalTaskFilterType globalFilterType,bool hasPatientJoinAlready) {
			string command=string.Empty;
			//Only add JOINs if filtering.  Filtering will never happen if clinics are turned off, because regions link via clinics.
			if((GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==GlobalTaskFilterType.Disabled 
				|| globalFilterType==GlobalTaskFilterType.None || !PrefC.HasClinicsEnabled)
			{
				return command;
			}
			if(!hasPatientJoinAlready) {
				command+=" LEFT JOIN patient ON task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" AND task.KeyNum=patient.PatNum ";
			}
			command+=" LEFT JOIN appointment ON task.ObjectType="+POut.Int((int)TaskObjectType.Appointment)+" AND task.KeyNum=appointment.AptNum ";
			return command;
		}

		///<summary>Builds WHERE clauses appropriate to the type of GlobalFilterType.  Returns empty string if not filtering.</summary>
		private static string BuildFilterWhereClause(long currentUserNum,GlobalTaskFilterType globalFilterType,List<long> listFilterFkeys) {
			//Only add WHERE clauses if filtering.  Filtering will never happen if clinics are turned off, because regions link via clinics.
			if((GlobalTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==GlobalTaskFilterType.Disabled 
				|| globalFilterType==GlobalTaskFilterType.None || !PrefC.HasClinicsEnabled) 
			{
				return "";
			}
			List<Clinic> listUnrestrictedClinics=Clinics.GetAllForUserod(Userods.GetUser(currentUserNum));
			List<long> listFkeys=new List<long>() { 0 };//All users can see Tasks associated to HQ clinic or "0" region.
			listFilterFkeys=listFilterFkeys??new List<long>() { 0 };
			switch(globalFilterType) {
				case GlobalTaskFilterType.Clinic:
					List<long> listUnrestrictedClinicNums=listUnrestrictedClinics.Select(x => x.ClinicNum).ToList();//User can view these clinicnums.
					if(listFilterFkeys.Count==1 && listFilterFkeys.Contains(0)) {//filtering using HQ.  Let all unrestricted clinics through the fitler.
						listFkeys.AddRange(listUnrestrictedClinicNums);
					}
					else {//Make sure user is not restricted for these clinics.
						listFkeys.AddRange(listUnrestrictedClinics.Where(x => ListTools.In(x.ClinicNum,listFilterFkeys)).Select(x => x.ClinicNum));
					}
					break;
				case GlobalTaskFilterType.Region:
					listFkeys.AddRange(listUnrestrictedClinics.FindAll(x => ListTools.In(x.Region,listFilterFkeys)).Select(x => x.ClinicNum));
					break;
				case GlobalTaskFilterType.None:
				default:
					return "";
			}
			string strFkeys=string.Join(",",listFkeys.Select(x => POut.Long(x)));
			return " AND (patient.ClinicNum IN ("+strFkeys+") OR appointment.ClinicNum IN ("+strFkeys+") "
				+"OR ((patient.ClinicNum IS NULL) AND (appointment.ClinicNum IS NULL))) ";
		}

		///<summary>The full refresh is only used once when first synching all the tasks for taskAncestors.</summary>
		public static List<Task> RefreshAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM task WHERE TaskListNum != 0";
			return Crud.TaskCrud.SelectMany(command);
		}

		/*
		public static List<Task> RefreshAndFill(DataTable table){
			//No need to check RemotingRole; no call to db.
			List<Task> retVal=new List<Task>();
			Task task;
			for(int i=0;i<table.Rows.Count;i++) {
				task=new Task();
				task.TaskNum        = PIn.Long(table.Rows[i][0].ToString());
				task.TaskListNum    = PIn.Long(table.Rows[i][1].ToString());
				task.DateTask       = PIn.Date(table.Rows[i][2].ToString());
				task.KeyNum         = PIn.Long(table.Rows[i][3].ToString());
				task.Descript       = PIn.String(table.Rows[i][4].ToString());
				task.TaskStatus     = (TaskStatusEnum)PIn.Long(table.Rows[i][5].ToString());
				task.IsRepeating    = PIn.Bool(table.Rows[i][6].ToString());
				task.DateType       = (TaskDateType)PIn.Long(table.Rows[i][7].ToString());
				task.FromNum        = PIn.Long(table.Rows[i][8].ToString());
				task.ObjectType     = (TaskObjectType)PIn.Long(table.Rows[i][9].ToString());
				task.DateTimeEntry  = PIn.DateT(table.Rows[i][10].ToString());
				task.UserNum        = PIn.Long(table.Rows[i][11].ToString());
				task.DateTimeFinished= PIn.DateT(table.Rows[i][12].ToString());
				retVal.Add(task);
			}
			return retVal;
		}*/

		///<summary>Surround with try/catch.  Must supply the supposedly unaltered oldTask.  Will throw an exception if oldTask does not exactly match the database state.  Keeps users from overwriting each other's changes.</summary>
		public static void Update(Task task,Task oldTask){
			//No need to check RemotingRole; no call to db.
			Validate(task,oldTask);//No try/catch here, we want the exception to be thrown back to the calling form.
			if(task.TaskStatus!=oldTask.TaskStatus && task.TaskStatus==TaskStatusEnum.Done && !String.IsNullOrEmpty(task.ReminderGroupId)) {
				//A reminder task status was changed to Done.
				CopyReminderToNextDueDate(task);
			}
			Update(task);
			if(task.TaskListNum!=oldTask.TaskListNum) {
				TaskAncestors.Synch(task);
				if(PrefC.IsODHQ && task.TaskListNum==TriageTaskListNum) {//Sending the task TO triage
					TaskTakens.DeleteForTask(task.TaskNum);
				}
			}
		}

		///<summary>Creates a copy of reminderTask with DateTimeEntry set to the next date due in the future.  Ensures new copy is marked new.
		///Returns the newly created task, or null if the new task could not be created.</summary>
		public static Task CopyReminderToNextDueDate(Task reminderTask) {
			//Do not copy reminder task if a copy already exists in the future.
			if(reminderTask.ReminderType==TaskReminderType.Once //Never make a copy of a 'once' reminder.
				|| (EnumTools.HasAnyFlag(reminderTask.ReminderType,TaskReminderType.Daily
					,TaskReminderType.Weekly,TaskReminderType.Monthly,TaskReminderType.Yearly)//Is repeating
					&& !reminderTask.IsNew//and is existing reminder task,
					&& GetCountReminderTasks(reminderTask.ReminderGroupId,reminderTask.DateTimeEntry) > 0))//with copies in the future
			{
				return null;
			}
			Task taskNext=reminderTask.Copy();//This is where taskNext.DateTimeEntry is initially set.
			taskNext.TaskNum=0;//This causes a new PK to be created for the new task.
			taskNext.TaskStatus=TaskStatusEnum.New;
			taskNext.DateTimeFinished=DateTime.MinValue;
			taskNext.DateTimeEntry=CalcTaskForwardDate(taskNext);
			long newTaskNum=Tasks.Insert(taskNext);
			//If we could we'd just call DataValid.SetInvalidTask(newTaskNum,true); but we're in ODBuisness so we'll do what we can to emulate it
			TaskUnreads.AddUnreads(taskNext,Security.CurUser.UserNum);  //We need the new copy to marked as unread for everyone for when it is "due"
			//Here we do our best to follow the signal logic in OpenDental namespace.  This may be unneccessary because the copied task isn't due 
			//for at least a day.  There will already be one signal for the old task being marked due, this is just for the copied task.
			Signalods.SetInvalid(InvalidType.TaskPopup,KeyType.Task,newTaskNum);
			return taskNext;
		}

		///<summary>Calculates the forward date for the task's DateTimeEntry field based on its ReminderType. Returns the forwarded date.</summary>
		public static DateTime CalcTaskForwardDate(Task curTask) {
			DateTime dateMin=DateTime.Today;
			if(curTask.DateTimeEntry.Date>DateTime.Today) {
				dateMin=curTask.DateTimeEntry.Date;
			}
			if(curTask.ReminderType.HasFlag(TaskReminderType.Daily)) {
				//Find the first day in the schedule which is also in the future.
				while(curTask.DateTimeEntry.Date<=dateMin) {
					curTask.DateTimeEntry=curTask.DateTimeEntry.AddDays(curTask.ReminderFrequency);
				}
			}
			else if(curTask.ReminderType.HasFlag(TaskReminderType.Weekly)) {
				//Find the first day in the schedule which is also in the future.
				while(curTask.DateTimeEntry.Date<=dateMin || !IsWeekDayFound(curTask.DateTimeEntry,curTask.ReminderType)) {
					if(curTask.DateTimeEntry.DayOfWeek==DayOfWeek.Sunday) {
						curTask.DateTimeEntry=curTask.DateTimeEntry.AddDays(-6 + 7*curTask.ReminderFrequency);//Increment to monday of next week in schedule.
					}
					else {
						curTask.DateTimeEntry=curTask.DateTimeEntry.AddDays(1);
					}
				}
			}
			else if(curTask.ReminderType.HasFlag(TaskReminderType.Monthly)) {
				//Find the first day in the schedule which is also in the future.
				while(curTask.DateTimeEntry.Date<=dateMin) {
					DateTime dtMonthStart=new DateTime(curTask.DateTimeEntry.Year,curTask.DateTimeEntry.Month,1);
					DateTime dtMonthNext=dtMonthStart.AddMonths(curTask.ReminderFrequency);
					int dayNext=Math.Min(curTask.DateTimeEntry.Day,DateTime.DaysInMonth(dtMonthNext.Year,dtMonthNext.Month));
					curTask.DateTimeEntry=dtMonthNext.AddDays(dayNext-1).AddTicks(curTask.DateTimeEntry.TimeOfDay.Ticks);//-1 day since already on 1st.
				}
			}
			else if(curTask.ReminderType.HasFlag(TaskReminderType.Yearly)) {
				//Find the first day in the schedule which is also in the future.
				while(curTask.DateTimeEntry.Date<=dateMin) {
					//We use the following algorithm to handle the edge case when the task was created on 02/29 of a leap year.
					//For this case, the task should be copied to 02/28 in a future year, unless that future year is also a leap year.
					DateTime dtYearMonthStart=new DateTime(curTask.DateTimeEntry.Year,curTask.DateTimeEntry.Month,1);
					DateTime dtYearMonthNext=dtYearMonthStart.AddYears(curTask.ReminderFrequency);
					int dayNext=Math.Min(curTask.DateTimeEntry.Day,DateTime.DaysInMonth(dtYearMonthNext.Year,dtYearMonthNext.Month));
					curTask.DateTimeEntry=dtYearMonthNext.AddDays(dayNext-1).AddTicks(curTask.DateTimeEntry.TimeOfDay.Ticks);//-1 day since already on 1st.
				}
			}
			return curTask.DateTimeEntry;
		}

		///<summary>Returns true if the dateTimeEntry is on a day of the week specified by the day schedule inside reminderType.</summary>
		private static bool IsWeekDayFound(DateTime dateTimeEntry,TaskReminderType reminderType) {
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Monday && reminderType.HasFlag(TaskReminderType.Monday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Tuesday && reminderType.HasFlag(TaskReminderType.Tuesday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Wednesday && reminderType.HasFlag(TaskReminderType.Wednesday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Thursday && reminderType.HasFlag(TaskReminderType.Thursday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Friday && reminderType.HasFlag(TaskReminderType.Friday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Saturday && reminderType.HasFlag(TaskReminderType.Saturday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Sunday && reminderType.HasFlag(TaskReminderType.Sunday)) {
				return true;
			}
			return false;
		}

		public static void Validate(Task task,Task oldTask) {
			//No need to check RemotingRole; no call to db.
			if(task.IsRepeating && task.DateTask.Year>1880) {
				throw new Exception(Lans.g("Tasks","Task cannot be tagged repeating and also have a date."));
			}
			if(task.IsRepeating && task.TaskStatus!=TaskStatusEnum.New) {//and any status but new
				throw new Exception(Lans.g("Tasks","Tasks that are repeating must have a status of New."));
			}
			if(task.IsRepeating && task.TaskListNum!=0 && task.DateType!=TaskDateType.None) {//In repeating, children not allowed to repeat.
				throw new Exception(Lans.g("Tasks","In repeating tasks, only the main parents can have a task status."));
			}
			if(WasTaskAltered(oldTask)){
				throw new Exception(Lans.g("Tasks","Not allowed to save changes because the task has been altered by someone else."));
			}
			if(PrefC.IsODHQ && oldTask.TaskListNum==TriageTaskListNum && task.TaskListNum!=TriageTaskListNum //Trying to claim a Triage task
				&& !TaskTakens.TryInsert(task.TaskNum)) 
			{				
				throw new Exception(Lans.g("Tasks","Not allowed to save changes because the task has been claimed by someone else."));
			}
			if(task.IsNew) {
				TaskEditCreateLog(Lans.g("Tasks","New task added"),task);
				task.IsNew=false;
			}
			else {
				if(task.TaskStatus!=oldTask.TaskStatus) {
					if(task.TaskStatus==TaskStatusEnum.Done) {
						TaskEditCreateLog(Lans.g("Tasks","Task marked done"),task);
					}
					if(task.TaskStatus==TaskStatusEnum.New) {
						TaskEditCreateLog(Lans.g("Tasks","Task marked new"),task);
					}
					//Nothing for case when Not New and Not Done. Put here in future is wanted
				}
				if(task.Descript!=oldTask.Descript) {
					TaskEditCreateLog(Lans.g("Tasks","Task description edited"),task);
				}
				if(task.UserNum!=oldTask.UserNum) {
					TaskEditCreateLog(Lans.g("Tasks","Changed user from")+" "+Userods.GetName(oldTask.UserNum),task);//+" To "+Userods.GetName(task.UserNum)),task);
				}
				if(task.KeyNum!=oldTask.KeyNum) {//We know at this point that SOMETHING with the task association changed.
					Patient patOld=null;
					Patient patNew=null;
					string log="";
					#region Old Task Object Type
					if(oldTask.KeyNum > 0) {//Old task had a patient/appointment
						if(oldTask.ObjectType==TaskObjectType.Patient) {//It was a patient
							patOld=Patients.GetLim(oldTask.KeyNum);
							log+=Lans.g("Tasks","Task object type changed from patient")+" "+patOld.GetNameFL()+" ";
						}
						else {//It was an appointment
							log+=Lans.g("Tasks","Task object type changed from appointment for")+" ";
							Appointment aptOld=Appointments.GetOneApt(oldTask.KeyNum);
							if(aptOld==null) {
								log+=Lans.g("Tasks","(appointment deleted)")+" ";
							}
							else {
								patOld=Patients.GetLim(aptOld.PatNum);
								log+=Lans.g("Tasks",patOld.GetNameLF()
									+"  "+aptOld.AptDateTime.ToString()
									+"  "+aptOld.ProcDescript+" ");
							}
						}
					}
					else {//Old task had "None"
						log+=Lans.g("Tasks","Task object type changed from none")+" ";
					}
					#endregion
					#region New Task Object Type
					if(task.KeyNum > 0) {//New task has a patient/appointment
						if(task.ObjectType==TaskObjectType.Patient) {//It was a patient
							patNew=Patients.GetLim(task.KeyNum);
							log+=Lans.g("Tasks","to object type patient")+" "+patNew.GetNameFL();
						}
						else {//It was an appointment
							log+=Lans.g("Tasks","to object type appointment for")+" ";
							Appointment aptNew=Appointments.GetOneApt(task.KeyNum);
							patNew=Patients.GetLim(aptNew.PatNum);
							if(aptNew==null) {
								log+=Lans.g("Tasks","(appointment deleted)");
							}
							else {
								log+=Lans.g("Tasks",patNew.GetNameLF()
									+"  "+aptNew.AptDateTime.ToString()
									+"  "+aptNew.ProcDescript);
							}
						}
					}
					else {
						log+=Lans.g("Tasks","to object type none.");
					}
					#endregion
					//Make a log depending on what happened with the object type association.
					TaskEditCreateLog(log,task);
				}
				if(task.TaskListNum!=oldTask.TaskListNum && oldTask.TaskListNum==0) {
					TaskEditCreateLog(Lans.g("Tasks","Task moved from Main"),task);
				}
				else if(task.TaskListNum!=oldTask.TaskListNum) {
					TaskList taskListOld = TaskLists.GetOne(oldTask.TaskListNum)??new TaskList() { Descript="<TaskListNum:"+oldTask.TaskListNum+">" };
					TaskEditCreateLog(Lans.g("Tasks","Task moved from")+" "+taskListOld.Descript,task);
				}
			}
		}

		///<summary>This update method doesn't do any of the typical checks for the Task update.Do not use this method. Instead use Update(Task task,Task oldTask).</summary>
		public static void Update(Task task) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),task);
				return;
			}
			Crud.TaskCrud.Update(task);
		}

		///<summary></summary>
		public static long Insert(Task task) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				task.TaskNum=Meth.GetLong(MethodBase.GetCurrentMethod(),task);
				return task.TaskNum;
			}
			if(task.IsRepeating && task.DateTask.Year>1880) {
				throw new Exception(Lans.g("Tasks","Task cannot be tagged repeating and also have a date."));
			}
			if(task.IsRepeating && task.TaskStatus!=TaskStatusEnum.New) {//and any status but new
				throw new Exception(Lans.g("Tasks","Tasks that are repeating must have a status of New."));
			}
			if(task.IsRepeating && task.TaskListNum!=0 && task.DateType!=TaskDateType.None) {//In repeating, children not allowed to repeat.
				throw new Exception(Lans.g("Tasks","In repeating tasks, only the main parents can have a task status."));
			}
			Crud.TaskCrud.Insert(task);
			TaskAncestors.Synch(task);
			return task.TaskNum;
		}

		///<summary></summary>
		public static bool WasTaskAltered(Task task){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),task);
			}
			string command="SELECT * FROM task WHERE TaskNum="+POut.Long(task.TaskNum);
			Task oldtask=Crud.TaskCrud.SelectOne(command);
			if(oldtask==null
				|| oldtask.DateTask!=task.DateTask
				|| oldtask.DateType!=task.DateType
				|| oldtask.Descript!=task.Descript
				|| oldtask.FromNum!=task.FromNum
				|| oldtask.IsRepeating!=task.IsRepeating
				|| oldtask.KeyNum!=task.KeyNum
				|| oldtask.ObjectType!=task.ObjectType
				|| oldtask.TaskListNum!=task.TaskListNum
				|| oldtask.TaskStatus!=task.TaskStatus
				|| oldtask.UserNum!=task.UserNum
				|| oldtask.DateTimeEntry!=task.DateTimeEntry
				|| oldtask.DateTimeFinished!=task.DateTimeFinished)
			{
				return true;
			}
			return false;
		}

		///<summary>Deleting a task never causes a problem, so no dependencies are checked.</summary>
		public static void Delete(long taskNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum);
				return;
			}
			Tasks.ClearFkey(taskNum);//Zero securitylog FKey column for rows to be deleted.
			string command= "DELETE FROM task WHERE TaskNum = "+POut.Long(taskNum);
 			Db.NonQ(command);
			command="DELETE FROM taskancestor WHERE TaskNum = "+POut.Long(taskNum);
			Db.NonQ(command);
			command="DELETE FROM tasknote WHERE TaskNum = "+POut.Long(taskNum);
			Db.NonQ(command);
			command="DELETE FROM taskunread WHERE TaskNum = "+POut.Long(taskNum);
			Db.NonQ(command);
			//Remove all references from the joblink table for HQ only.
			if(Prefs.GetBoolNoCache(PrefName.DockPhonePanelShow)) {
				command="DELETE FROM joblink "
					+"WHERE FKey = "+POut.Long(taskNum)+" "
					+"AND LinkType = "+POut.Int((int)JobLinkType.Task);
				Db.NonQ(command);
			}
		}

		/*
		///<summary>Appends a carriage return as well as the text to any task.  If a taskListNum is specified, then it also changes the taskList.</summary>
		public static void Append(long taskNum,string text) {
			//No need to check RemotingRole; no call to db.
			Append(taskNum,text,-1);
		}

		///<summary>Appends a carriage return as well as the text to any task.  If a taskListNum is specified, then it also changes the taskList.    Must call TaskAncestors.Synch after this.</summary>
		public static void Append(long taskNum,string text,long taskListNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum,text,taskListNum);
				return;
			}
			string command;
			if(taskListNum==-1) {
				command="UPDATE task SET Descript=CONCAT(Descript,'"+POut.String("\r\n"+text)+"') WHERE TaskNum="+POut.Long(taskNum);
			}
			else {
				command="UPDATE task SET Descript=CONCAT(Descript,'"+POut.String("\r\n"+text)+"'), "
					+"TaskListNum="+POut.Long(taskListNum)+" "
					+"WHERE TaskNum="+POut.Long(taskNum);
			}
			Db.NonQ(command);
		}*/

		public static int GetCountOpenTickets(long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT COUNT(*) "
				+"FROM task "
				+"WHERE NOT EXISTS(SELECT * FROM taskancestor,tasklist "
				+"WHERE taskancestor.TaskNum=task.TaskNum "
				+"AND tasklist.TaskListNum=taskancestor.TaskListNum "
				+"AND tasklist.DateType!=0) "//if any ancestor is a dated list, then we don't want that task
				+"AND task.DateType=0 "//this only handles tasks directly in the dated trunks
				+"AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"AND task.IsRepeating=0 "
				+"AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") "//no future reminders
				+"AND task.UserNum="+POut.Long(userNum)+" "
				+"AND TaskStatus != "+POut.Int((int)TaskStatusEnum.Done);
			return PIn.Int(Db.GetCount(command));
		}

		public static int GetCountPatientTickets(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT COUNT(*) "
				+"FROM task "
				+"WHERE task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"AND task.IsRepeating=0 "
				+"AND task.KeyNum="+POut.Long(patNum)+" "
				+"AND TaskStatus != "+POut.Int((int)TaskStatusEnum.Done);
			return PIn.Int(Db.GetCount(command));
		}
		
		///<summary>Only called for ODHQ. Gets the tasks that are in the Office Down task list. This method only returns the Task table row, not the
		///extra non-DB columns.</summary>
		public static List<Task> GetOfficeDowns() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * "
				+"FROM task "
				+"WHERE TaskListNum="+OfficeDownTaskListNum+" "
				+"AND TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" "
				+"ORDER BY DateTimeEntry";
			return Crud.TaskCrud.SelectMany(command);
		}

		public static void TaskEditCreateLog(string logText,Task task) {
			TaskEditCreateLog(Permissions.TaskEdit,logText,task);
		}

		///<summary>Makes audit trail entry for the task passed in.
		///If this task has an object type set, the log will show up under the corresponding patient for the selected object type.
		///Used for both TaskEdit and TaskNoteEdit permissions.</summary>
		public static void TaskEditCreateLog(Permissions perm,string logText,Task task) {
			if(task==null) {  //Something went wrong before calling this function, and somehow task wasn't passed in
				//Do nothing.  This was added because in very intermittent situations this function would throw a UE and crash OD.
				//	this is just a simple securitylog entry so it is fine to skip it in this case.  We should try to solve the issues
				//	causing null to be passed in, but we should not let this throw a UE.
				return;
			}
			long patNum=0;//Task type of none defaults to 0.
			if(task.KeyNum!=0) {  //Either no object attached, or object hasn't been commited to db yet (Changed the object but haven't clicked OK on TaskEdit).
				if(task.ObjectType==TaskObjectType.Patient) {//Task type of patient we can use the task.KeyNum for patNum
					patNum=task.KeyNum;
				}
				else if(task.ObjectType==TaskObjectType.Appointment) {//Task type of appointment we have to look up the patient from the apt.
					Appointment AptCur=Appointments.GetOneApt(task.KeyNum);
					if(AptCur!=null) { //appointment was deleted so don't worry about logging the patient.
						patNum=AptCur.PatNum;
					}
				}
			}
			SecurityLogs.MakeLogEntry(perm,patNum,logText,task.TaskNum,DateTime.MinValue); //tasks do not track DateTStamp
		}

		///<summary>Sorted in Ascending order: Unread/Read, </summary>
		public static int TaskComparer(TaskCompareObj x,TaskCompareObj y) {
			if(_isSortApptDateTime) {
				bool xIsObjectTypeApt = (PIn.Int(x.RowTask["ObjectType"].ToString())==(int)TaskObjectType.Appointment);
				bool yIsObjectTypeApt = (PIn.Int(y.RowTask["ObjectType"].ToString())==(int)TaskObjectType.Appointment);
				if(xIsObjectTypeApt ^ yIsObjectTypeApt) {//XOR. One is Appt object type and one isnt.
					//Show the Appointment type at the top.
					return(-xIsObjectTypeApt.CompareTo(yIsObjectTypeApt));
				}
				else if(xIsObjectTypeApt && yIsObjectTypeApt) {//Both are Appt object type.
					//Use AptDateTime to sort.
					return(CompareAptDateTimes(x.RowTask,y.RowTask));
				}
				else {//Neither are appt object type
					//Use normal sorting logic, continue below.
				}
			}
			//1)Sort by IsUnread status
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				if(x.RowTask["IsUnread"].ToString()!=y.RowTask["IsUnread"].ToString()) {
					//Note: we are returning the negative of x.CompareTo(y)
					return -(PIn.Long(x.RowTask["IsUnread"].ToString()).CompareTo(PIn.Long(y.RowTask["IsUnread"].ToString())));//sort unread to top.
				}
			}
			//2)Sort by Task Priority
			if(x.RowTask["PriorityDefNum"].ToString()!=y.RowTask["PriorityDefNum"].ToString()) {//we only care about task priority if they are different
				long xTaskPriorityDefNum=PIn.Long(x.RowTask["PriorityDefNum"].ToString());
				long yTaskPriorityDefNum=PIn.Long(y.RowTask["PriorityDefNum"].ToString());
				//0 will always be considered like the default task priority.
				if(xTaskPriorityDefNum==0) {
					xTaskPriorityDefNum=_defaultTaskPriorityDefNum;
				}
				if(yTaskPriorityDefNum==0) {
					yTaskPriorityDefNum=_defaultTaskPriorityDefNum;
				}
				//x.ItemOrder.CompareTo(y.ItemOrder)
				return Defs.GetDef(DefCat.TaskPriorities,xTaskPriorityDefNum,x.ListTaskPriorityDefs).ItemOrder.CompareTo(Defs.GetDef(DefCat.TaskPriorities,yTaskPriorityDefNum,x.ListTaskPriorityDefs).ItemOrder);
			}
			//3)Sort by Date Time
			return CompareTimes(x.RowTask,y.RowTask);
		}

		///<summary>Compares the most recent times of the task or task notes associated to the tasks passed in.  Most recently updated tasks will be farther down in the list.</summary>
		public static int CompareTimes(DataRow x,DataRow y) {
			if(_isHQ
				&& PIn.Long(x["TaskListNum"].ToString())==TriageTaskListNum
				&& PIn.Long(x["PriorityDefNum"].ToString())==RedTaskDefNum)//Red tasks in triage only, sort by lastUpdated
			{
				DateTime xMaxDateTime=PIn.DateT(x["LastUpdated"].ToString());
				DateTime yMaxDateTime=PIn.DateT(y["LastUpdated"].ToString());
				return xMaxDateTime.CompareTo(yMaxDateTime);
			}
			else {//Sort everything else based on task creation date
				DateTime xMaxDateTime=PIn.DateT(x["DateTimeEntry"].ToString());
				DateTime yMaxDateTime=PIn.DateT(y["DateTimeEntry"].ToString());
				return xMaxDateTime.CompareTo(yMaxDateTime);
			}
		}

		///<summary>Compares the AptDateTime of appointments attached to tasks.  Most recently updated tasks will be farther down in the list.
		///If there is no appointment attached, it appears at the bottom.</summary>
		public static int CompareAptDateTimes(DataRow x,DataRow y) {
			DateTime xDateTime = DateTime.MaxValue;
			DateTime yDateTime = DateTime.MaxValue;
			if(_dictTaskApts.ContainsKey(PIn.Long(x["KeyNum"].ToString()))) {
				xDateTime=_dictTaskApts[PIn.Long(x["KeyNum"].ToString())];
			}
			if(_dictTaskApts.ContainsKey(PIn.Long(y["KeyNum"].ToString()))) {
				yDateTime=_dictTaskApts[PIn.Long(y["KeyNum"].ToString())];
			}
			return xDateTime.CompareTo(yDateTime);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching taskNum as FKey and are related to Task.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Task table type.</summary>
		public static void ClearFkey(long taskNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum);
				return;
			}
			Crud.TaskCrud.ClearFkey(taskNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching taskNums as FKey and are related to Task.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Task table type.</summary>
		public static void ClearFkey(List<long> listTaskNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTaskNums);
				return;
			}
			Crud.TaskCrud.ClearFkey(listTaskNums);
		}

		///<summary>Helper object so that TaskComparer() doesn't have to make deep copies of caches.</summary>
		public class TaskCompareObj {
			public DataRow RowTask;
			public List<Def> ListTaskPriorityDefs;
		}
	}
}
