using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary>Not part of cache refresh.</summary>
	public class Tasks {
		#region Misc Methods
		///<summary>Returns true if there are any rows that have a Descript with char length greater than 65,535</summary>
		public static bool HasAnyLongDescripts() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM task WHERE CHAR_LENGTH(task.Descript)>65535";
			return (Db.GetCount(command)!="0");
		}

		/// <summary>Returns true if either the current user created this task, or if they have permission to edit read only tasks, otherwise false.</summary>
		public static bool IsAuthorizedOrOwner(Task task) {
			//If the task is new then there is no point in checking permissions or for other task notes (even if other people added them somehow).
			if(task.IsNew) {
				return true;
			}
			//If the user is authorized to edit read-only tasks, don't waste time checking ownership.
			if(Security.IsAuthorized(EnumPermType.EditReadOnlyTasks,suppressMessage:true)) {
				return true;
			}
			//If the current user didn't write this task, block them.
			if(task.UserNum!=Security.CurUser.UserNum) {
				return false;
			}
			//If the task is not in the logged-in user's inbox.
			if(task.TaskListNum!=Security.CurUser.TaskListInBox && task.TaskListNum!=-1) {
				return false;
			}
			//Check to see if other users have added notes.
			List<TaskNote> listTaskNotes = TaskNotes.GetForTask(task.TaskNum);
			for(int i = 0;i<listTaskNotes.Count;i++) {
				if(Security.CurUser.UserNum!=listTaskNotes[i].UserNum) {
					return false;
				}
			}
			return true;
		}

		///<summary>Returns true if task does not exist in the database.</summary>
		public static bool IsTaskDeleted(long taskNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command = "SELECT COUNT(*) FROM task WHERE TaskNum="+POut.Long(taskNum)+"";
			return Db.GetCount(command)=="0";
		}

		///<summary>Returns true if task is a Reminder Task.</summary>
		public static bool IsReminderTask(Task task) {
			Meth.NoCheckMiddleTierRole();
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
		///<summary>Only used from UI.  The index of the last open tab.</summary>
		public static int LastOpenGroup;
		///<summary>Only used from UI.</summary>
		public static DateTime dateLastOpen;

		///<summary>This is needed because of the extra column that is not part of the database.</summary>
		private static List<Task> TableToList(DataTable table) {
			Meth.NoCheckMiddleTierRole();
			List<Task> listTasks=Crud.TaskCrud.TableToList(table);
			for(int i=0;i<listTasks.Count;i++) {
				if(table.Columns.Contains("IsUnread")) {
					listTasks[i].IsUnread=PIn.Bool(table.Rows[i]["IsUnread"].ToString());//1 or more will result in true.
				}
				if(table.Columns.Contains("ParentDesc")) {
					listTasks[i].ParentDesc=PIn.String(table.Rows[i]["ParentDesc"].ToString());
				}
				if(table.Columns.Contains("LName")
					&& table.Columns.Contains("FName")
					&& table.Columns.Contains("Preferred"))
				{
					string lname=PIn.String(table.Rows[i]["LName"].ToString());
					string fname=PIn.String(table.Rows[i]["FName"].ToString());
					string preferred=PIn.String(table.Rows[i]["Preferred"].ToString());
					listTasks[i].PatientName=Patients.GetNameLF(lname,fname,preferred,"");
				}
			}
			return listTasks;
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Task>(MethodBase.GetCurrentMethod(),TaskNum);
			}
			string command="SELECT * FROM task WHERE TaskNum = "+POut.Long(TaskNum);
			return Crud.TaskCrud.SelectOne(command);
		}
		
		///<summary>Gets all tasks for the Task Search function, limited to 50 by default.</summary>
		public static DataTable GetDataSet(long userNum,List<long> listTaskListNums,List<long> listTaskNums,string taskDateCreatedFrom,
			string taskDateCreatedTo,string taskDateCompletedFrom,string taskDateCompletedTo,string taskIncluding,string taskExcluding,
			long taskPriorityNum,long patNum,bool doIncludeTaskNote,bool doIncludeCompleted,bool doIncludeAttachments,bool reachedLimit,
			bool doRunOnReportServer) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),userNum,listTaskListNums,listTaskNums,taskDateCreatedFrom,taskDateCreatedTo,	taskDateCompletedFrom,taskDateCompletedTo,taskIncluding,taskExcluding,taskPriorityNum,patNum,doIncludeTaskNote,doIncludeCompleted,doIncludeAttachments,reachedLimit,doRunOnReportServer);
			}
			DateTime dateCreatedFrom=PIn.Date(taskDateCreatedFrom);//will be DateTime.MinValue if not set, i.e. if " "
			DateTime dateCreatedTo=PIn.Date(taskDateCreatedTo);//will be DateTime.MinValue if not set, i.e. if " "
			DateTime dateCompletedFrom=PIn.Date(taskDateCompletedFrom);//will be DateTime.MinValue if not set, i.e. if " "
			DateTime dateCompletedTo=PIn.Date(taskDateCompletedTo);//will be DateTime.MinValue if not set, i.e. if " "
			List<long> listTaskNumsSearch=ReportsComplex.RunFuncOnReportServer(() => GetTasksNumsForSearch(userNum,listTaskListNums,listTaskNums,dateCreatedFrom,dateCreatedTo,dateCompletedFrom,dateCompletedTo,taskIncluding,taskExcluding,taskPriorityNum,patNum,doIncludeTaskNote,doIncludeCompleted,doIncludeAttachments,reachedLimit),doRunOnReportServer);
			DataTable table=new DataTable();
			table.Columns.Add(new DataColumn("description"));
			table.Columns.Add(new DataColumn("note"));
			table.Columns.Add(new DataColumn("PatNum"));
			table.Columns.Add(new DataColumn("procTime"));
			table.Columns.Add(new DataColumn("dateCreate"));
			table.Columns.Add(new DataColumn("dateComplete"));
			table.Columns.Add(new DataColumn("TaskNum"));
			table.Columns.Add(new DataColumn("color"));
			if(listTaskNumsSearch.Count==0) {
				return table;//empty table with correct structure.
			}
			//listTaskNums contains too many items. Tasks found from matching task notes must be filtered too. (This prevents a costly join in the query.)
			List<Task> listTasks=ReportsComplex.RunFuncOnReportServer(() => Tasks.GetMany(listTaskNumsSearch),doRunOnReportServer)//All tasks for the notes and tasks
				.Where(x => listTaskListNums.Count==0 || listTaskListNums.Contains(x.TaskListNum))//filter by TaskListNum, if neccesary
				.Where(x => taskPriorityNum==0 || taskPriorityNum==x.PriorityDefNum)//filter by priority, if neccesary
				.Where(x => patNum==0 || (x.ObjectType==TaskObjectType.Patient && x.KeyNum==patNum))//filter by patnum, if neccesary
				.Where(x => dateCompletedFrom==DateTime.MinValue || x.DateTimeFinished.Date>=dateCompletedFrom.Date)//filter by dateFrom, if neccesary
				.Where(x => dateCompletedTo==DateTime.MinValue || x.DateTimeFinished.Date<=dateCompletedTo.Date)
				.OrderByDescending(x=> (x.DateTimeOriginal==DateTime.MinValue ? x.DateTimeEntry : x.DateTimeOriginal)).ToList();//Order results by DateTimeOriginal if it exists, DateTimeEntry if not
			List<TaskNote> listTaskNotes=new List<TaskNote>();
			if(doIncludeTaskNote) {
				//All notes for the tasks.	(Ordered by dateTime)		
				listTaskNotes=ReportsComplex.RunFuncOnReportServer(() => TaskNotes.RefreshForTasks(listTaskNumsSearch),doRunOnReportServer);
			}
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors,true);
			int textColor=Defs.GetColor(DefCat.ProgNoteColors,listDefs[18].DefNum).ToArgb();//18="Patient Note Text"
			int textCompletedColor=Defs.GetColor(DefCat.ProgNoteColors,listDefs[20].DefNum).ToArgb();//20="Completed Pt Note Text"
			List<TaskList> listTaskLists=ReportsComplex.RunFuncOnReportServer(() => TaskLists.GetMany(listTaskListNums),doRunOnReportServer);
			string txt;
			DataRow row;
			for(int i=0;i<listTasks.Count;i++) {
				txt="";
				row=table.NewRow();
				//Build data row
				if(listTasks[i].TaskStatus==TaskStatusEnum.Done) {
					row["color"]=textCompletedColor;
					txt+=Lans.g("TaskSearch","Completed")+" ";
				}
				else {
					row["color"]=textColor;
				}
				if(listTasks[i].TaskListNum!=0) {
					row["description"]=txt+Lans.g("TaskSearch","In List")+": "+TaskLists.GetFullPath(listTasks[i].TaskListNum,listTaskLists);
				}
				else {
					row["description"]=txt+Lans.g("TaskSearch","Not in list");
				}
				txt="";
				if(!listTasks[i].Descript.StartsWith("==") && listTasks[i].UserNum!=0) {
					txt+=Userods.GetName(PIn.Long(listTasks[i].UserNum.ToString()))+" - ";
				}
				txt+=listTasks[i].Descript;
				if(doIncludeTaskNote) {
					listTaskNotes.FindAll(x => x.TaskNum==listTasks[i].TaskNum)
					.ForEach(x => txt+="\r\n"//even on the first loop
						+"=="+Userods.GetName(x.UserNum)+" - "
						+x.DateTimeNote.ToShortDateString()+" "
						+x.DateTimeNote.ToShortTimeString()
						+" - "+x.Note);
				}
				row["note"]=txt;
				if(listTasks[i].ObjectType==TaskObjectType.Patient) {
					row["PatNum"]=listTasks[i].KeyNum;
				}
				if(listTasks[i].DateTask.Year>1880) {//check if due date set for task or note
					row["dateCreate"]=listTasks[i].DateTask.ToString(Lans.GetShortDateTimeFormat());
				}
				else if(listTasks[i].DateTimeOriginal.Year>1880) {
					row["dateCreate"]=listTasks[i].DateTimeOriginal.ToShortDateString();
				}
				if(listTasks[i].DateTask.TimeOfDay!=TimeSpan.Zero) {
					row["procTime"]=listTasks[i].DateTask.ToString("h:mm")+listTasks[i].DateTask.ToString("%t").ToLower();
				}
				else if(listTasks[i].DateTimeEntry.TimeOfDay!=TimeSpan.Zero) {
					row["procTime"]=listTasks[i].DateTimeEntry.ToString("h:mm")+listTasks[i].DateTimeEntry.ToString("%t").ToLower();
				}
				if(listTasks[i].DateTimeFinished.Year>1880) {
					row["dateComplete"]=listTasks[i].DateTimeFinished.ToString(Lans.GetShortDateTimeFormat());
				}
				row["TaskNum"]=listTasks[i].TaskNum;
				table.Rows.Add(row);
			}
			return table;
		}

		///<summary>Gets the task nums for the tasks based on the search parameters passed in.</summary>
		public static List<long> GetTasksNumsForSearch(long userNum,List<long> listTaskListNums,List<long> listTaskNums,DateTime dateCreatedFrom,
			DateTime dateCreatedTo,DateTime dateCompletedFrom,DateTime dateCompletedTo,string taskIncluding,string taskExcluding,long taskPriorityNum,
			long patNum,bool doIncludeTaskNote,bool doIncludeCompleted,bool doIncludeAttachments,bool reachedLimit) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),userNum,listTaskListNums,listTaskNums,dateCreatedFrom,dateCreatedTo,dateCompletedFrom,dateCompletedTo,taskIncluding,taskExcluding,
					taskPriorityNum,patNum,doIncludeTaskNote,doIncludeCompleted,doIncludeAttachments,reachedLimit);
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
			bool doIncludeKeywordSearch=false;
			if(taskIncluding!="") {
				doIncludeKeywordSearch=true;
				List<string> listSearchesIncluding=TaskQuoteHelper(taskSearch:taskIncluding);
				for(int i=0;i<listSearchesIncluding.Count;i++) {
					if(doIncludeAttachments) {
						listWhereClauses.Add("(task.Descript LIKE '%"+POut.String(listSearchesIncluding[i])+"%' OR taskattachment.Description LIKE '%"+POut.String(listSearchesIncluding[i])+"%' "
							+"OR taskattachment.TextValue LIKE '%"+POut.String(listSearchesIncluding[i])+"%')");
					}
					else {
						listWhereClauses.Add("task.Descript LIKE '%"+POut.String(listSearchesIncluding[i])+"%'");
					}
					listWhereNoteClauses.Add("tasknote.Note LIKE '%"+POut.String(listSearchesIncluding[i])+"%'");
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
			//Here be query assembly:
			bool doExcludeKeywordSearch=false;
			string command="SELECT TaskNum FROM (";
			#region exclude
			//Build exclude keyword query
			if(taskExcluding!="") {
				List<string> listSearchesExcluding=TaskQuoteHelper(taskSearch:taskExcluding);
				doExcludeKeywordSearch=true;
				command+="SELECT task.TaskNum, task.DateTimeOriginal DateTimeSearch "
					+"FROM task "
					+"WHERE task.Descript NOT LIKE '%"+POut.String(listSearchesExcluding[0])+"%' ";
				if(listSearchesExcluding.Count>1) {
					for(int i=1;i<listSearchesExcluding.Count;i++) {
						command+="AND task.Descript NOT LIKE '%"+POut.String(listSearchesExcluding[i])+"%' ";
					}
				}
				if(!doIncludeCompleted) {
					command+="AND task.TaskStatus!="+POut.Long((int)TaskStatusEnum.Done)+" ";
				}
				if(doIncludeTaskNote) {
					command+="AND task.TaskNum "
					+"NOT IN (SELECT tasknote.TaskNum FROM tasknote "
					+"WHERE tasknote.Note LIKE '%"+POut.String(listSearchesExcluding[0])+"%' ";
					if(listSearchesExcluding.Count>1) {
						for(int i=1;i<listSearchesExcluding.Count;i++) {
							command+="OR tasknote.Note LIKE '%"+POut.String(listSearchesExcluding[i])+"%' ";
						}
					}
					command+=") ";
				}
				if(doIncludeAttachments) {
					command+="AND task.TaskNum "
						+"NOT IN (SELECT taskattachment.TaskNum FROM taskattachment "
						+"WHERE taskattachment.Description LIKE '%"+POut.String(listSearchesExcluding[0])+"%' "
						+"OR taskattachment.TextValue LIKE '%"+POut.String(listSearchesExcluding[0])+"%' ";
					if(listSearchesExcluding.Count>1) {
						for(int i=1;i<listSearchesExcluding.Count;i++) {
							command+="OR taskattachment.Description LIKE '%"+POut.String(listSearchesExcluding[i])+"%' "
								+"OR taskattachment.TextValue LIKE '%"+POut.String(listSearchesExcluding[i])+"%' ";
						}
					}
					command+=") ";
				}
				if(doIncludeKeywordSearch) {
					command+="AND task.TaskNum IN ";
				}
			}
			#endregion
			#region include
			//Build include-exclude query
			if(doIncludeKeywordSearch && doExcludeKeywordSearch //User supplied both include and exclude keywords.
				|| listWhereClauses.Count>0 && doExcludeKeywordSearch) {//User filtered by patNum, userNum, date range, etc. and supplied exclude keyword(s).
				if(!doIncludeKeywordSearch) {
					command+="AND task.TaskNum IN ";
				}
				command+="(SELECT task.TaskNum "
					+"FROM task ";
				if(doIncludeAttachments) {
					command+="LEFT JOIN taskattachment ON task.TaskNum=taskattachment.TaskNum ";
				}
				command+=whereClause;
				if(doIncludeTaskNote) {
					command+=" UNION "
					+"(SELECT tasknote.TaskNum "
					+"FROM tasknote "
					+noteJoinClause
					+whereNoteClause
					+")";
				}
				//User supplied exclude keyword and other filter(s). Requires additional perenths for grouping.
				if(!doIncludeCompleted
					|| userNum!=0
					|| patNum!=0
					|| listTaskListNums.Count>0
					|| dateCreatedFrom>DateTime.MinValue
					|| dateCreatedTo>DateTime.MinValue
					|| dateCompletedFrom>DateTime.MinValue
					|| dateCompletedTo>DateTime.MinValue
					|| doIncludeAttachments
					|| doExcludeKeywordSearch) 
				{
					command+=")";
				}
			}
			else if(doExcludeKeywordSearch) {//User supplied only exclude keyword(s).
				//Do nothing. No need to build include query.
			}
			else {//Existing behavior. User supplied no keyword, or only include keyword(s), and/or filtered by patNum, userNum, date range, etc.
				command+="(SELECT task.TaskNum,task.DateTimeOriginal DateTimeSearch "
					+"FROM task ";
				if(doIncludeAttachments) {
					command+="LEFT JOIN taskattachment ON task.TaskNum=taskattachment.TaskNum ";
				}
				command+=whereClause
					+")";
				if(doIncludeTaskNote) {
					command+=" UNION "
					+"(SELECT tasknote.TaskNum,tasknote.DateTimeNote DateTimeSearch "
					+"FROM tasknote "
				+noteJoinClause
				+whereNoteClause
				+")";
				}
			}
			#endregion
			command+=") tasksearch GROUP BY tasksearch.TaskNum ORDER BY MIN(tasksearch.DateTimeSearch) DESC ";
			if(reachedLimit) {
				command+=" LIMIT 50";
			}
			return Db.GetListLong(command);
		}

		///<summary>Returns a string array parsed by one or many quoted elements.</summary>
		private static List<string> TaskQuoteHelper(string taskSearch) {
			#region Escape Special Characters
			//taskSearch string contains a (\). A textBox escapes this by adding an additional (\). \+\=\\
			//MySQL requires 4 backslashes to parse 1 backslash correctly. \+\=\\=\\\\
			//MySQL requires 2 backslashes to escape percent sign correctly.
			//MySQL requires 2 backslashes to escape underscore correctly.
			taskSearch=taskSearch.Replace("\\","\\\\").Replace("%","\\%").Replace("_","\\_");
			#endregion
			#region Regex Explanation
			//Will not parse a quoted string that contains a quote.		Ex:"1 "2 3" "	Result:1,2 3
			//Will not parse a quoted string with leading whitespace.	Ex:" one two"	Result:one,two

			//Find a sequence of 0 or more characters that are not a (") but are preceded by a (") and followed by a ("),
			//OR a sequence of 1 or more characters that are not (") or ( ). Meaning, special characters are valid.

			//(?<="")	:A positive lookbehind for (").
			//[^""]		:One character except (").
			//*				:Zero or more.
			//(?="")	:A positive lookahead for (").
			//|				:OR.
			//[^"" ]	:One character except (") or ( ).
			//+				:One or more.
			#endregion
			Regex regex=new Regex(@"(?<="")[^""]*(?="")|[^"" ]+");
			List<string> listTaskSearches=regex.Matches(taskSearch)
				.Cast<Match>()
				.Select(x => x.Value)
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToList();
			//Example string: zero "1 2" "3 4" 5  six "seven eight" 9ten 11
			//In this example, " 5  six " will be its own element in listTaskSearches. We do not want this.
			//Instead, break that element up, remove it from the list, then add the individual elements back.
			for(int i=0;i<listTaskSearches.Count;i++) {
				if(char.IsWhiteSpace(listTaskSearches[i],0)) {//0th position is whitespace.
					List<string> listSubstrings=listTaskSearches[i].Trim().Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
					listTaskSearches.RemoveAt(i--);
					listTaskSearches.AddRange(listSubstrings);
					continue;
				}
				listTaskSearches[i]=listTaskSearches[i].Trim();
			}
			return listTaskSearches;
		}

		///<summary>Gets all tasks for a supplied list of task nums.</summary>
		public static List<Task> GetMany(List<long> listTaskNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),listTaskNums);
			}
			if(listTaskNums==null || listTaskNums.Count==0) {
				return new List<Task>();
			}
			string command="SELECT * FROM task WHERE TaskNum IN("+String.Join(",",listTaskNums)+") ORDER BY DateTimeEntry";
			return Crud.TaskCrud.SelectMany(command);
		}

		///<summary>Gets all tasks for a supplied AptNum.</summary>
		public static List<Task> GetMany(long AptNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),AptNum);
			}
			string command=$@"SELECT * FROM task WHERE ObjectType={POut.Int((int)TaskObjectType.Appointment)} AND task.KeyNum={POut.Long(AptNum)}";
			return Crud.TaskCrud.SelectMany(command);
		}

		///<summary>Gets multiple Tasks from database. Returns empty list if not found.</summary>
		public static List<Task> GetTasksForApi(int limit,int offset,long taskListNum,long keyNum,int objectType,int taskStatus,DateTime dateTimeOriginal) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),limit,offset,taskListNum,keyNum,objectType,taskStatus,dateTimeOriginal);
			}
			string command="SELECT * FROM task WHERE DateTimeOriginal >= "+POut.DateT(dateTimeOriginal)+" ";
			if(taskListNum>-1) {
				command+="AND TaskListNum="+POut.Long(taskListNum)+" ";
			}
			if(keyNum>-1) {
				command+="AND KeyNum="+POut.Long(keyNum)+" ";
			}
			if(objectType>-1) {
				command+="AND ObjectType="+POut.Int(objectType)+" ";
			}
			if(taskStatus>-1) {
				command+="AND TaskStatus="+POut.Int(taskStatus)+" ";
			}
			command+="ORDER BY TaskNum "//same fixed order each time
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.TaskCrud.SelectMany(command);
		}

		///<summary>Gets the count of reminder tasks on or after the specified dateTimeAsOf.</summary>
		public static int GetCountReminderTasks(string reminderGroupId,DateTime dateTimeAsOf) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			command+="INNER JOIN tasklist ON tasklist.TaskListNum=taskancestor.TaskListNum "
				+"INNER JOIN tasksubscription ON tasksubscription.TaskListNum=tasklist.TaskListNum AND tasksubscription.UserNum="+POut.Long(userNum)+" "
				+"LEFT JOIN taskunread ON taskunread.TaskNum=task.TaskNum AND taskunread.UserNum="+POut.Long(userNum);
			if(Clinics.ClinicNum!=0 || !PrefC.HasClinicsEnabled) {
				command+=TaskLists.BuildFilterJoins(clinicNum);
				command+=" WHERE TRUE "+TaskLists.BuildFilterWhereClause(userNum,clinicNum,Clinics.GetClinic(clinicNum)?.Region??0);
			}
			List<Task> listTasks=TableToList(Db.GetTable(command));//This is how we set the IsUnread column.
			Logger.LogToPath("",LogPath.Signals,LogPhase.End);
			return listTasks;
		}

		///<summary>Gets a string using the aptNum as the key. String consits of patient name and some appointment information.</summary>
		public static SerializableDictionary<long,string> GetApptObjDescripts(List<long> listPatApts){
			if(listPatApts.Count==0) {
				return new SerializableDictionary<long,string>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,string>(MethodBase.GetCurrentMethod(),listPatApts);
			}
			string command=@"SELECT patient.LName,patient.FName,patient.Preferred,patient.MiddleI,appointment.AptNum,appointment.AptDateTime,
				appointment.ProcDescript,appointment.Note,appointment.AptStatus 
				FROM appointment 
				INNER JOIN patient ON patient.PatNum=appointment.PatNum 
				WHERE appointment.AptNum IN ("+string.Join(",",listPatApts)+")";
			DataTable table= Db.GetTable(command);
			SerializableDictionary<long,string> dictTaskString=new SerializableDictionary<long, string>();
			for(int i=0;i<table.Rows.Count;i++) {
				string patName=Patients.GetNameLF(table.Rows[i]["LName"].ToString(),table.Rows[i]["FName"].ToString(),
					table.Rows[i]["Preferred"].ToString(),table.Rows[i]["MiddleI"].ToString()); //no call to db
				Appointment appointment=new Appointment();
				appointment.AptNum=PIn.Long(table.Rows[i]["AptNum"].ToString());
				appointment.AptDateTime=PIn.DateT(table.Rows[i]["AptDateTime"].ToString());
				string dateTimeStr=appointment.AptDateTime.ToString();
				if(PIn.Enum<ApptStatus>(table.Rows[i]["AptStatus"].ToString())==ApptStatus.UnschedList) {
					dateTimeStr=Lans.g("Tasks","Unscheduled");
				}
				appointment.ProcDescript=PIn.String(table.Rows[i]["ProcDescript"].ToString());
				appointment.Note=PIn.String(table.Rows[i]["Note"].ToString());
				dictTaskString.Add(appointment.AptNum,patName+" "+dateTimeStr+" "+appointment.ProcDescript+" "+appointment.Note+" - ");
			}
			return dictTaskString;
		}

		///<summary>Sets the task.ReminderGroupId to a brand new and unique value.</summary>
		public static void SetReminderGroupId(Task task) {
			Meth.NoCheckMiddleTierRole();
			task.ReminderGroupId=CodeBase.MiscUtils.CreateRandomAlphaNumericString(20);
			while(true) {
				//Verify that the new group id does not exist.
				//query:
				if(Tasks.GetCountReminderTasks(task.ReminderGroupId,DateTime.MinValue) == 0) {
					break;
				}
				task.ReminderGroupId=CodeBase.MiscUtils.CreateRandomAlphaNumericString(20);
			}
		}

		/// <summary>Sets ReminderType to NoReminder for all tasks in a task list </summary>
		public static void DisableRemindersFromTasklist(long taskListNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskListNum);
				return;
			}
			string command="UPDATE task SET ReminderType="+POut.Long((long)TaskReminderType.NoReminder)+" "
				+"WHERE TaskListNum="+POut.Long(taskListNum);
			Db.NonQ(command);
		}

		///<summary>Gets all tasks for the main trunk.</summary>
		public static List<Task> RefreshMainTrunk(bool showDone,DateTime dateStart,long userNum,TaskType taskType,
			List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),showDone,dateStart,userNum,taskType,listClinicNumsFilter,listDefNumsRegionFilter);
			}
			//startDate only applies if showing Done tasks.
			string command="SELECT task.*,"
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum AND taskunread.UserNum="+POut.Long(userNum)+") IsUnread, "
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter,listDefNumsRegionFilter);
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
					+" OR DateTimeFinished > "+POut.Date(dateStart)+")";//of if done, then restrict date
			}
			else{
				command+=" AND TaskStatus !="+POut.Long((int)TaskStatusEnum.Done);
			}
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			command+=" ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all 'new' tasks for a user.</summary>
		public static List<Task> RefreshUserNew(long userNum,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),userNum,listClinicNumsFilter,listDefNumsRegionFilter);
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
			command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter:listClinicNumsFilter,listDefNumsRegionFilter:listDefNumsRegionFilter);
			command+="WHERE NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") "//no future reminders
				+"AND task.TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="GROUP BY task.TaskNum "//in case there are duplicate unreads
				+"ORDER BY task.DateTimeEntry";
			DataTable table=Db.GetTable(command);
			List<DataRow> listDataRows=new List<DataRow>();
			for(int i=0;i<table.Rows.Count;i++) {
				listDataRows.Add(table.Rows[i]);
			}
			#region Set Sort Variables. This greatly increases sort speed.
			_isHQ=PrefC.GetBool(PrefName.DockPhonePanelShow);//increases speed of the sort function performed below.
			List<Def> listDefsTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);
			for(int i=0;i<listDefsTaskPriorities.Count;i++) {
				if(listDefsTaskPriorities[i].ItemValue.ToUpper()=="D") {
					_defaultTaskPriorityDefNum=listDefsTaskPriorities[i].DefNum;
					break;
				}
			}
			#endregion
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TaskPriorities);
			List<TaskCompareObj> listTaskCompareObjs=new List<TaskCompareObj>();
			for(int i=0;i<listDataRows.Count;i++) {
				TaskCompareObj taskCompareObj=new TaskCompareObj();
				taskCompareObj.DataRowTask=listDataRows[i];
				taskCompareObj.ListDefsTaskPriority=listDefs;
				listTaskCompareObjs.Add(taskCompareObj);
			}
			listTaskCompareObjs.Sort(TaskComparer);
			DataTable tableSorted=table.Clone();//Easy way to copy the columns.
			tableSorted.Rows.Clear();
			for(int i=0;i<listTaskCompareObjs.Count;i++) {
				tableSorted.Rows.Add(listTaskCompareObjs[i].DataRowTask.ItemArray);
			}
			List<Task> listTasks=TableToList(tableSorted);
			return listTasks;
		}

		///<summary>Gets all 'open ticket' tasks for a user.  An open ticket is a task that was created by this user, is attached to a patient, and is not done.</summary>
		public static List<Task> RefreshOpenTickets(long userNum,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),userNum,listClinicNumsFilter,listDefNumsRegionFilter);
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
			command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter,listDefNumsRegionFilter);
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
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all 'open ticket' tasks for a patient.  An open ticket is a task that was created with the attached patient and is not done.</summary>
		public static List<Task> RefreshPatientTickets(long patNum,long userNum=0,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),patNum,userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			}
			string command="SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
				+"AND taskunread.UserNum="+POut.Long(Security.CurUser.UserNum)+") AS IsUnread, "
				+"tasklist.Descript AS ParentDesc "
				+"FROM task "
				+"LEFT JOIN tasklist ON task.TaskListNum=tasklist.TaskListNum ";
			command+=BuildFilterJoins(hasPatientJoinAlready:false,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="WHERE task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"AND task.KeyNum="+POut.Long(patNum)+" "
				+"AND TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all tasks for the repeating trunk.  Always includes "done".</summary>
		public static List<Task> RefreshRepeatingTrunk(long userNum,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			}
			string command="SELECT task.*, "
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="WHERE TaskListNum=0 "
				+"AND DateTask < "+POut.Date(new DateTime(1880,01,01))+" "
				+"AND IsRepeating=1 "
				+"AND COALESCE(task.ReminderGroupId,'')='' ";//no reminders
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>0 is not allowed, because that would be a trunk.  
		///Also, if this is in someone's inbox, then pass in the userNum whose inbox it is in.  If not in an inbox, pass in 0.</summary>
		public static List<Task> RefreshChildren(long listNum,bool showDone,DateTime dateStart,long userNum,long userNumInbox,
			TaskType taskType,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null)
		{
			return RefreshChildren(listNum,showDone,dateStart,userNum,userNumInbox,taskType,false,listClinicNumsFilter,listDefNumsRegionFilter);
		}

		///<summary>0 is not allowed, because that would be a trunk.
		///Also, if this is in someone's inbox, then pass in the userNum whose inbox it is in.  If not in an inbox, pass in 0.
		///If isTaskSortApptDateTime==true and parent task list is an appointment type task list, TaskComparer oders appointment tasks to the top and
		///then by AptDateTime.</summary>
		public static List<Task> RefreshChildren(long listNum,bool showDone,DateTime dateStart,long userNum,long userNumInbox,TaskType taskType,
			bool isTaskSortApptDateTime,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null,
			DateTime dateStartFilter=new DateTime(),DateTime dateEndFilter=new DateTime(),Patient patientFilter=null)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),listNum,showDone,dateStart,userNum,userNumInbox,taskType,
					isTaskSortApptDateTime,listClinicNumsFilter,listDefNumsRegionFilter,dateStartFilter,dateEndFilter,patientFilter);
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
				command+="AND taskunread.UserNum="+POut.Long(userNum)+") IsUnread, ";
			}
			if(isTaskSortApptDateTime) {
				command+="appointment.AptNum, appointment.AptStatus, appointment.AptDateTime, ";
			}
			command+="patient.LName,patient.FName,patient.Preferred, "
				+"COALESCE(MAX(tasknote.DateTimeNote),task.DateTimeEntry) AS 'LastUpdated',"
				+"CASE WHEN tasknote.TaskNoteNum IS NULL THEN 0 ELSE 1 END AS 'HasNotes' "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"LEFT JOIN tasknote ON task.TaskNum=tasknote.TaskNum ";
			if(isTaskSortApptDateTime) {
				command+="LEFT JOIN appointment ON task.ObjectType="+POut.Int((int)TaskObjectType.Appointment)+" AND task.KeyNum=appointment.AptNum ";
			}
			else {
				command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter,listDefNumsRegionFilter,dateStartFilter,dateEndFilter,patientFilter);
			}
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
					+" OR DateTimeFinished > "+POut.Date(dateStart)+")" //or if done, then restrict date
					+" OR DateTimeFinished = '0001-01-01 00:00:00')"; //Include tasks that have a finished date time as MinValue so they can be edited.
			}
			else {
				command+=" AND TaskStatus !="+POut.Long((int)TaskStatusEnum.Done);
			}
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter,dateStartFilter,dateEndFilter,patientFilter);
			command+=" GROUP BY task.TaskNum "//Sorting happens below
							+" ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			List<Task> listTasks=new List<Task>();
			//Note: Only used for HQ, Oracle does not matter.
			List<DataRow> listDataRows=new List<DataRow>();
			for(int i=0;i<table.Rows.Count;i++) {
				listDataRows.Add(table.Rows[i]);
			}
			#region Set Sort Variables. This greatly increases sort speed.
			_isHQ=PrefC.GetBool(PrefName.DockPhonePanelShow);//increases speed of the sort function performed below.
			List<Def> listDefsTaskPriorities=Defs.GetDefsForCategory(DefCat.TaskPriorities,true);
			for(int i=0;i<listDefsTaskPriorities.Count;i++) {
				if(listDefsTaskPriorities[i].ItemValue.ToUpper()=="D") {
					_defaultTaskPriorityDefNum=listDefsTaskPriorities[i].DefNum;
					break;
				}
			}
			_isSortApptDateTime=false;
			if(isTaskSortApptDateTime) {
				TaskList taskListParent=TaskLists.GetOne(listNum);
				if(taskListParent!=null && taskListParent.ObjectType==TaskObjectType.Appointment) {//If parent tasklist is an appointment tasklist.
					_isSortApptDateTime=true;//Sets flag for sorting
				}
			}
			#endregion
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TaskPriorities);
			List<TaskCompareObj> listTaskCompareObjs=new List<TaskCompareObj>();
			for(int i=0;i<listDataRows.Count;i++) {
				TaskCompareObj taskCompareObj=new TaskCompareObj();
				taskCompareObj.DataRowTask=listDataRows[i];
				taskCompareObj.ListDefsTaskPriority=listDefs;
				listTaskCompareObjs.Add(taskCompareObj);
			}
			listTaskCompareObjs.Sort(TaskComparer);
			_isSortApptDateTime=false;//Turn special sorting back off
			DataTable tableSorted=table.Clone();//Easy way to copy the columns.
			tableSorted.Rows.Clear();
			for(int i=0;i<listTaskCompareObjs.Count;i++) {
				tableSorted.Rows.Add(listTaskCompareObjs[i].DataRowTask.ItemArray);
			}
			listTasks=TableToList(tableSorted);
			return listTasks;
		}

		///<summary>All repeating items for one date type with no heirarchy.</summary>
		public static List<Task> RefreshRepeating(TaskDateType taskDataType,long userNum,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),taskDataType,userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			}
			string command=
				"SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
				+"AND taskunread.UserNum="+POut.Long(userNum)+") IsUnread, "//Not sure if this makes sense here
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter:listClinicNumsFilter,listDefNumsRegionFilter:listDefNumsRegionFilter);
			command+="WHERE IsRepeating=1 "
				+"AND COALESCE(task.ReminderGroupId,'')='' "//no reminders
				+"AND DateType="+POut.Long((int)taskDataType)+" ";
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Gets all tasks for one of the 3 dated trunks. startDate only applies if showing Done.</summary>
		public static List<Task> RefreshDatedTrunk(DateTime date,TaskDateType taskDateType,bool showDone,DateTime dateStart,long userNum
			,List<long> listClinicNumsFilter=null,List<long> listDefNumsRegionFilter=null) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod(),date,taskDateType,showDone,dateStart,userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			}
			DateTime dateFrom=DateTime.MinValue;
			DateTime dateTo=DateTime.MaxValue;
			if(taskDateType==TaskDateType.Day) {
				dateFrom=date;
				dateTo=date;
			}
			else if(taskDateType==TaskDateType.Week) {
				dateFrom=date.AddDays(-(int)date.DayOfWeek);
				dateTo=dateFrom.AddDays(6);
			}
			else if(taskDateType==TaskDateType.Month) {
				dateFrom=new DateTime(date.Year,date.Month,1);
				dateTo=dateFrom.AddMonths(1).AddDays(-1);
			}
			string command=
				"SELECT task.*, "
				+"(SELECT COUNT(*) FROM taskunread WHERE task.TaskNum=taskunread.TaskNum "
				+"AND taskunread.UserNum="+POut.Long(userNum)+") IsUnread, "//Not sure if this makes sense here
				+"patient.LName,patient.FName,patient.Preferred "
				+"FROM task "
				+"LEFT JOIN patient ON task.KeyNum=patient.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" ";
			command+=BuildFilterJoins(hasPatientJoinAlready:true,listClinicNumsFilter,listDefNumsRegionFilter);
			command+="WHERE DateTask >= "+POut.Date(dateFrom)
				+" AND DateTask <= "+POut.Date(dateTo)
				+" AND DateType="+POut.Long((int)taskDateType)
				+" AND COALESCE(task.ReminderGroupId,'')='' ";//no reminders
			command+=BuildFilterWhereClause(userNum,listClinicNumsFilter,listDefNumsRegionFilter);
			if(showDone){
				command+=" AND (TaskStatus !="+POut.Long((int)TaskStatusEnum.Done)
					+" OR DateTimeFinished > "+POut.Date(dateStart)+")";//of if done, then restrict date
			}
			else{
				command+=" AND TaskStatus !="+POut.Long((int)TaskStatusEnum.Done);
			}
			command+=" ORDER BY DateTimeEntry";
			DataTable table=Db.GetTable(command);
			return TableToList(table);
		}

		///<summary>Builds JOIN clauses appropriate to the type of GlobalFilterType.  Returns empty string if not filtering.</summary>
		private static string BuildFilterJoins(bool hasPatientJoinAlready,List<long> listClinicNumsFilter=null,
			List<long> listDefNumsRegionFilter=null,DateTime dateStartFilter=new DateTime(),DateTime dateEndFilter=new DateTime(),Patient patientFilter=null) {
			string command=string.Empty;
			//Only add JOINs if filtering.  Clinic/Region filtering will never happen if clinics are turned off, because regions link via clinics.
			if((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==EnumTaskFilterType.Disabled  || !PrefC.HasClinicsEnabled) {
				if(listClinicNumsFilter==null 
					&& listDefNumsRegionFilter==null 
					&& dateStartFilter==DateTime.MinValue 
					&& dateEndFilter==DateTime.MinValue 
					&& patientFilter==null) 
				{//Check for all filters
					return command;
				}
			}
			if(!hasPatientJoinAlready) {
				command+=" LEFT JOIN patient ON task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" AND task.KeyNum=patient.PatNum ";
			}
			command+=" LEFT JOIN appointment ON task.ObjectType="+POut.Int((int)TaskObjectType.Appointment)+" AND task.KeyNum=appointment.AptNum ";
			return command;
		}

		///<summary>Builds WHERE clauses appropriate to the task filters that are applied.  Returns empty string if not filtering.</summary>
		private static string BuildFilterWhereClause(long currentUserNum,List<long> listClinicNumsFilter=null,
			List<long> listDefNumsRegionFilter=null,DateTime dateStartFilter=new DateTime(),DateTime dateEndFilter=new DateTime(),Patient patientFilter=null) {
			if(currentUserNum==0) {//The currentUserNum will be zero when merging patients; cannot build the filter without a valid patnum
				return "";
			}
			if(listClinicNumsFilter==null) {
				listClinicNumsFilter=new List<long>();
			}
			if(listDefNumsRegionFilter==null) {
				listDefNumsRegionFilter=new List<long>();
			}
			string clinicAnd="";
			string dateRangeAnd="";
			string patientAnd="";
			//Only add WHERE clauses if filtering.  Clinic filtering will never happen if clinics are turned off, because regions link via clinics.
			if((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==EnumTaskFilterType.Disabled || !PrefC.HasClinicsEnabled) {
				clinicAnd="";
			}
			List<Clinic> listClinicsUnrestricted=Clinics.GetAllForUserod(Userods.GetUser(currentUserNum));
			List<long> listClinicNumsUnrestricted=listClinicsUnrestricted.Select(x => x.ClinicNum).ToList();//User can view these clinicnums.
			//All users can see Tasks associated to HQ clinic or "0" region.
			List<long> listClinicNumsForQuery=new List<long>() { 0 };//if filtering by regions, this list will include all clinics within filtered regions
			bool isHQClinic=true;
			bool isHQRegion=true;
			if(listClinicNumsFilter.Count!=1 || !listClinicNumsFilter.Contains(0)) {
				isHQClinic=false;
			}
			if(listDefNumsRegionFilter.Count!=1 || !listDefNumsRegionFilter.Contains(0)) {
				isHQRegion=false;
			}
			if(listClinicNumsFilter.Count>0 && !isHQClinic) {
				//Make sure user is not restricted for these clinics.
				listClinicNumsForQuery.AddRange(listClinicsUnrestricted.Where(x => listClinicNumsFilter.Contains(x.ClinicNum)).Select(x => x.ClinicNum));
			}
			else if(listDefNumsRegionFilter.Count>0 && !isHQRegion) {
				listClinicNumsForQuery.AddRange(listClinicsUnrestricted.FindAll(x => listDefNumsRegionFilter.Contains(x.Region)).Select(x => x.ClinicNum));
			}
			else if(isHQClinic && isHQRegion) {
				listClinicNumsForQuery.AddRange(listClinicNumsUnrestricted);
			}
			else {//No clinic/region filtering
				clinicAnd="";
			}
			if(listClinicNumsForQuery.Count>1) {//check if it is more than HQ and if clinic/region filter is needed
				string strFkeys=string.Join(",",listClinicNumsForQuery.Select(x => POut.Long(x)));
				clinicAnd=" AND (patient.ClinicNum IN ("+strFkeys+") OR appointment.ClinicNum IN ("+strFkeys+") "
				+"OR ((patient.ClinicNum IS NULL) AND (appointment.ClinicNum IS NULL))) ";
			}
			if(dateStartFilter!=DateTime.MinValue || dateEndFilter!=DateTime.MinValue) {
				if(dateEndFilter==DateTime.MinValue) {
					dateEndFilter=DateTime.MaxValue;
				}
				else {//these dates are at 12:00AM, so add a day to actually include the dateEnd
					dateEndFilter=dateEndFilter.AddDays(1);
				}
				dateRangeAnd=" AND (appointment.AptDateTime BETWEEN"+POut.Date(dateStartFilter)+" AND "+POut.Date(dateEndFilter)+" "
				+"OR (appointment.AptDateTime IS NULL AND task.DateTimeEntry BETWEEN"+POut.Date(dateStartFilter)+" AND "+POut.Date(dateEndFilter)+")) ";
			}
			if(patientFilter!=null) {
				patientAnd=" AND (patient.PatNum="+patientFilter.PatNum+" OR appointment.PatNum="+patientFilter.PatNum+") ";
			}
			return clinicAnd+dateRangeAnd+patientAnd;
		}

		///<summary>The full refresh is only used once when first synching all the tasks for taskAncestors.</summary>
		public static List<Task> RefreshAll(){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Task>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM task WHERE TaskListNum != 0";
			return Crud.TaskCrud.SelectMany(command);
		}

		/*
		public static List<Task> RefreshAndFill(DataTable table){
			Meth.NoCheckMiddleTierRole();
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
		public static void Update(Task task,Task taskOld){
			Meth.NoCheckMiddleTierRole();
			Validate(task,taskOld);//No try/catch here, we want the exception to be thrown back to the calling form.
			if(task.TaskStatus!=taskOld.TaskStatus && task.TaskStatus==TaskStatusEnum.Done && !String.IsNullOrEmpty(task.ReminderGroupId)) {
				//A reminder task status was changed to Done.
				CopyReminderToNextDueDate(task);
			}
			Update(task);
			if(task.TaskListNum==taskOld.TaskListNum) {
				return;
			}
			TaskAncestors.Synch(task);
			if(PrefC.IsODHQ && task.TaskListNum==TriageTaskListNum) {//Sending the task TO triage
				TaskTakens.DeleteForTask(task.TaskNum);
			}
		}

		///<summary>Creates a copy of reminderTask with DateTimeEntry set to the next date due in the future.  Ensures new copy is marked new.
		///Returns the newly created task, or null if the new task could not be created.</summary>
		public static Task CopyReminderToNextDueDate(Task taskReminder) {
			//Do not copy reminder task if a copy already exists in the future.
			if(taskReminder.ReminderType==TaskReminderType.Once //Never make a copy of a 'once' reminder.
				|| (EnumTools.HasAnyFlag(taskReminder.ReminderType,TaskReminderType.Daily
					,TaskReminderType.Weekly,TaskReminderType.Monthly,TaskReminderType.Yearly)//Is repeating
					&& !taskReminder.IsNew//and is existing reminder task,
					&& GetCountReminderTasks(taskReminder.ReminderGroupId,taskReminder.DateTimeEntry) > 0))//with copies in the future
			{
				return null;
			}
			Task taskNext=taskReminder.Copy();//This is where taskNext.DateTimeEntry is initially set.
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
		public static DateTime CalcTaskForwardDate(Task task) {
			DateTime dateMin=DateTime.Today;
			if(task.DateTimeEntry.Date>DateTime.Today) {
				dateMin=task.DateTimeEntry.Date;
			}
			if(task.ReminderType.HasFlag(TaskReminderType.Daily)) {
				//Find the first day in the schedule which is also in the future.
				while(true) {
					if(task.DateTimeEntry.Date>dateMin) {
						break;
					}
					task.DateTimeEntry=task.DateTimeEntry.AddDays(task.ReminderFrequency);
				}
			}
			else if(task.ReminderType.HasFlag(TaskReminderType.Weekly)) {
				//Find the first day in the schedule which is also in the future.
				while(true) {
					if(task.DateTimeEntry.Date>dateMin && IsWeekDayFound(task.DateTimeEntry,task.ReminderType)) {
						break;
					}
					if(task.DateTimeEntry.DayOfWeek==DayOfWeek.Sunday) {
						task.DateTimeEntry=task.DateTimeEntry.AddDays(-6 + 7*task.ReminderFrequency);//Increment to monday of next week in schedule.
					}
					else {
						task.DateTimeEntry=task.DateTimeEntry.AddDays(1);
					}
				}
			}
			else if(task.ReminderType.HasFlag(TaskReminderType.Monthly)) {
				//Find the first day in the schedule which is also in the future.
				while(true) {
					if(task.DateTimeEntry.Date>dateMin) {
						break;
					}
					DateTime dateTimeMonthStart=new DateTime(task.DateTimeEntry.Year,task.DateTimeEntry.Month,1);
					DateTime dateTimeMonthNext=dateTimeMonthStart.AddMonths(task.ReminderFrequency);
					int dayNext=Math.Min(task.DateTimeEntry.Day,DateTime.DaysInMonth(dateTimeMonthNext.Year,dateTimeMonthNext.Month));
					task.DateTimeEntry=dateTimeMonthNext.AddDays(dayNext-1).AddTicks(task.DateTimeEntry.TimeOfDay.Ticks);//-1 day since already on 1st.
				}
			}
			else if(task.ReminderType.HasFlag(TaskReminderType.Yearly)) {
				//Find the first day in the schedule which is also in the future.
				while(true) {
					if(task.DateTimeEntry.Date>dateMin) {
						break;
					}
					//We use the following algorithm to handle the edge case when the task was created on 02/29 of a leap year.
					//For this case, the task should be copied to 02/28 in a future year, unless that future year is also a leap year.
					DateTime dateTimeYearMonthStart=new DateTime(task.DateTimeEntry.Year,task.DateTimeEntry.Month,1);
					DateTime dateTimeYearMonthNext=dateTimeYearMonthStart.AddYears(task.ReminderFrequency);
					int dayNext=Math.Min(task.DateTimeEntry.Day,DateTime.DaysInMonth(dateTimeYearMonthNext.Year,dateTimeYearMonthNext.Month));
					task.DateTimeEntry=dateTimeYearMonthNext.AddDays(dayNext-1).AddTicks(task.DateTimeEntry.TimeOfDay.Ticks);//-1 day since already on 1st.
				}
			}
			return task.DateTimeEntry;
		}

		///<summary>Returns true if the dateTimeEntry is on a day of the week specified by the day schedule inside reminderType.</summary>
		private static bool IsWeekDayFound(DateTime dateTimeEntry,TaskReminderType taskReminderType) {
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Monday && taskReminderType.HasFlag(TaskReminderType.Monday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Tuesday && taskReminderType.HasFlag(TaskReminderType.Tuesday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Wednesday && taskReminderType.HasFlag(TaskReminderType.Wednesday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Thursday && taskReminderType.HasFlag(TaskReminderType.Thursday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Friday && taskReminderType.HasFlag(TaskReminderType.Friday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Saturday && taskReminderType.HasFlag(TaskReminderType.Saturday)) {
				return true;
			}
			if(dateTimeEntry.Date.DayOfWeek==DayOfWeek.Sunday && taskReminderType.HasFlag(TaskReminderType.Sunday)) {
				return true;
			}
			return false;
		}

		public static void Validate(Task task,Task taskOld) {
			Meth.NoCheckMiddleTierRole();
			if(task.IsRepeating && task.DateTask.Year>1880) {
				throw new Exception(Lans.g("Tasks","Task cannot be tagged repeating and also have a date."));
			}
			if(task.IsRepeating && task.TaskStatus!=TaskStatusEnum.New) {//and any status but new
				throw new Exception(Lans.g("Tasks","Tasks that are repeating must have a status of New."));
			}
			if(task.IsRepeating && task.TaskListNum!=0 && task.DateType!=TaskDateType.None) {//In repeating, children not allowed to repeat.
				throw new Exception(Lans.g("Tasks","In repeating tasks, only the main parents can have a task status."));
			}
			if(WasTaskAltered(taskOld)){
				throw new Exception(Lans.g("Tasks","Not allowed to save changes because the task has been altered by someone else."));
			}
			//HQ Only - Every workstation needs to communicate with the same server when moving triage tasks out of the task list (e.g. trying to claim the task).
			if(PrefC.IsODHQ && taskOld.TaskListNum==TriageTaskListNum && task.TaskListNum!=TriageTaskListNum) {
				//Try and insert a tasktaken entry using the data action for the 'TriageHQ' database.
				//If no error occurs then this user has claimed the task. Otherwise, something went wrong and the user needs to be made aware.
				TaskTakens.InsertForTaskNum(task.TaskNum);
			}
			if(task.IsNew) {
				TaskEditCreateLog(Lans.g("Tasks","New task added"),task);
				task.IsNew=false;
				return;
			}
			if(task.TaskStatus!=taskOld.TaskStatus) {
				if(task.TaskStatus==TaskStatusEnum.Done) {
					TaskEditCreateLog(Lans.g("Tasks","Task marked done"),task);
				}
				if(task.TaskStatus==TaskStatusEnum.New) {
					TaskEditCreateLog(Lans.g("Tasks","Task marked new"),task);
				}
				//Nothing for case when Not New and Not Done. Put here in future is wanted
			}
			if(task.Descript!=taskOld.Descript) {
				TaskEditCreateLog(Lans.g("Tasks","Task description edited"),task);
			}
			if(task.UserNum!=taskOld.UserNum) {
				TaskEditCreateLog(Lans.g("Tasks","Changed user from")+" "+Userods.GetName(taskOld.UserNum),task);//+" To "+Userods.GetName(task.UserNum)),task);
			}
			if(task.KeyNum!=taskOld.KeyNum) {//We know at this point that SOMETHING with the task association changed.
				Patient patientOld=null;
				Patient patientNew=null;
				string log="";
				#region Old Task Object Type
				if(taskOld.KeyNum > 0) {//Old task had a patient/appointment
					if(taskOld.ObjectType==TaskObjectType.Patient) {//It was a patient
						patientOld=Patients.GetLim(taskOld.KeyNum);
						log+=Lans.g("Tasks","Task object type changed from patient")+" "+patientOld.GetNameFL()+" ";
					}
					else {//It was an appointment
						log+=Lans.g("Tasks","Task object type changed from appointment for")+" ";
						Appointment appointmentOld=Appointments.GetOneApt(taskOld.KeyNum);
						if(appointmentOld==null) {
							log+=Lans.g("Tasks","(appointment deleted)")+" ";
						}
						else {
							patientOld=Patients.GetLim(appointmentOld.PatNum);
							log+=Lans.g("Tasks",patientOld.GetNameLF()
								+"  "+appointmentOld.AptDateTime.ToString()
								+"  "+appointmentOld.ProcDescript+" ");
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
						patientNew=Patients.GetLim(task.KeyNum);
						log+=Lans.g("Tasks","to object type patient")+" "+patientNew.GetNameFL();
					}
					else {//It was an appointment
						log+=Lans.g("Tasks","to object type appointment for")+" ";
						Appointment appointmentNew=Appointments.GetOneApt(task.KeyNum);
						patientNew=Patients.GetLim(appointmentNew.PatNum);
						if(appointmentNew==null) {
							log+=Lans.g("Tasks","(appointment deleted)");
						}
						else {
							log+=Lans.g("Tasks",patientNew.GetNameLF()
								+"  "+appointmentNew.AptDateTime.ToString()
								+"  "+appointmentNew.ProcDescript);
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
			if(task.TaskListNum!=taskOld.TaskListNum && taskOld.TaskListNum==0) {
				TaskEditCreateLog(Lans.g("Tasks","Task moved from Main"),task);
				return;
			}
			if(task.TaskListNum!=taskOld.TaskListNum) {
				TaskList taskListOld = TaskLists.GetOne(taskOld.TaskListNum)??new TaskList() { Descript="<TaskListNum:"+taskOld.TaskListNum+">" };
				TaskEditCreateLog(Lans.g("Tasks","Task moved from")+" "+taskListOld.Descript,task);
			}
		}

		///<summary>This update method doesn't do any of the typical checks for the Task update.Do not use this method. Instead use Update(Task task,Task oldTask).</summary>
		public static void Update(Task task) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),task);
				return;
			}
			Crud.TaskCrud.Update(task);
		}

		///<summary></summary>
		public static long Insert(Task task) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),task);
			}
			string command="SELECT * FROM task WHERE TaskNum="+POut.Long(task.TaskNum);
			Task taskOld=Crud.TaskCrud.SelectOne(command);
			if(taskOld==null
				|| taskOld.DateTask!=task.DateTask
				|| taskOld.DateType!=task.DateType
				|| taskOld.Descript!=task.Descript
				|| taskOld.FromNum!=task.FromNum
				|| taskOld.IsRepeating!=task.IsRepeating
				|| taskOld.KeyNum!=task.KeyNum
				|| taskOld.ObjectType!=task.ObjectType
				|| taskOld.TaskListNum!=task.TaskListNum
				|| taskOld.TaskStatus!=task.TaskStatus
				|| taskOld.UserNum!=task.UserNum
				|| taskOld.DateTimeEntry!=task.DateTimeEntry
				|| taskOld.DateTimeFinished!=task.DateTimeFinished)
			{
				return true;
			}
			return false;
		}

		///<summary>Deleting a task never causes a problem, so no dependencies are checked.</summary>
		public static void Delete(long taskNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			command="DELETE FROM taskattachment WHERE TaskNum = "+POut.Long(taskNum);
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
			Meth.NoCheckMiddleTierRole();
			Append(taskNum,text,-1);
		}

		///<summary>Appends a carriage return as well as the text to any task.  If a taskListNum is specified, then it also changes the taskList.    Must call TaskAncestors.Synch after this.</summary>
		public static void Append(long taskNum,string text,long taskListNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			TaskEditCreateLog(EnumPermType.TaskEdit,logText,task);
		}

		///<summary>Makes audit trail entry for the task passed in.
		///If this task has an object type set, the log will show up under the corresponding patient for the selected object type.
		///Used for both TaskEdit and TaskNoteEdit permissions.</summary>
		public static void TaskEditCreateLog(EnumPermType permissions,string logText,Task task) {
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
					Appointment appointment=Appointments.GetOneApt(task.KeyNum);
					if(appointment!=null) { //appointment was deleted so don't worry about logging the patient.
						patNum=appointment.PatNum;
					}
				}
			}
			SecurityLogs.MakeLogEntry(permissions,patNum,logText,task.TaskNum,DateTime.MinValue); //tasks do not track DateTStamp
		}

		///<summary>Sorted in Ascending order: Unread/Read, </summary>
		public static int TaskComparer(TaskCompareObj taskCompareObjX,TaskCompareObj taskCompareObjY) {
			if(_isSortApptDateTime) {
				bool isObjectTypeAptx = (PIn.Int(taskCompareObjX.DataRowTask["ObjectType"].ToString())==(int)TaskObjectType.Appointment);
				bool isObjectTypeAptY = (PIn.Int(taskCompareObjY.DataRowTask["ObjectType"].ToString())==(int)TaskObjectType.Appointment);
				if(isObjectTypeAptx ^ isObjectTypeAptY) {//XOR. One is Appt object type and one isnt.
					//Show the Appointment type at the top.
					return(-isObjectTypeAptx.CompareTo(isObjectTypeAptY));
				}
				if(isObjectTypeAptx && isObjectTypeAptY) {//Both are Appt object type.
					//Use AptDateTime to sort.
					return(CompareAptDateTimes(taskCompareObjX.DataRowTask,taskCompareObjY.DataRowTask));
				}
				//Else if neither are appt object type
				//Use normal sorting logic, continue below.
			}
			//1)Sort by IsUnread status
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				if(taskCompareObjX.DataRowTask["IsUnread"].ToString()!=taskCompareObjY.DataRowTask["IsUnread"].ToString()) {
					//Note: we are returning the negative of x.CompareTo(y)
					return -(PIn.Long(taskCompareObjX.DataRowTask["IsUnread"].ToString()).CompareTo(PIn.Long(taskCompareObjY.DataRowTask["IsUnread"].ToString())));//sort unread to top.
				}
			}
			//2)Sort by Task Priority
			if(taskCompareObjX.DataRowTask["PriorityDefNum"].ToString()!=taskCompareObjY.DataRowTask["PriorityDefNum"].ToString()) {//we only care about task priority if they are different
				long xTaskPriorityDefNum=PIn.Long(taskCompareObjX.DataRowTask["PriorityDefNum"].ToString());
				long yTaskPriorityDefNum=PIn.Long(taskCompareObjY.DataRowTask["PriorityDefNum"].ToString());
				//0 will always be considered like the default task priority.
				if(xTaskPriorityDefNum==0) {
					xTaskPriorityDefNum=_defaultTaskPriorityDefNum;
				}
				if(yTaskPriorityDefNum==0) {
					yTaskPriorityDefNum=_defaultTaskPriorityDefNum;
				}
				//x.ItemOrder.CompareTo(y.ItemOrder)
				return Defs.GetDef(DefCat.TaskPriorities,xTaskPriorityDefNum,taskCompareObjX.ListDefsTaskPriority).ItemOrder.CompareTo(Defs.GetDef(DefCat.TaskPriorities,yTaskPriorityDefNum,taskCompareObjX.ListDefsTaskPriority).ItemOrder);
			}
			//3)Sort by Date Time
			return CompareTimes(taskCompareObjX.DataRowTask,taskCompareObjY.DataRowTask);
		}

		///<summary>Compares the most recent times of the task or task notes associated to the tasks passed in.  Most recently updated tasks will be farther down in the list.</summary>
		public static int CompareTimes(DataRow dataRowX,DataRow dataRowY) {
			DateTime dateTimeXMax;
			DateTime dateTimeYMax;
			if(_isHQ
				&& PIn.Long(dataRowX["TaskListNum"].ToString())==TriageTaskListNum
				&& PIn.Long(dataRowX["PriorityDefNum"].ToString())==RedTaskDefNum)//Red tasks in triage only, sort by lastUpdated
			{
				dateTimeXMax=PIn.DateT(dataRowX["LastUpdated"].ToString());
				dateTimeYMax=PIn.DateT(dataRowY["LastUpdated"].ToString());
				return dateTimeXMax.CompareTo(dateTimeYMax);
			}
			//Sort everything else based on task creation date
			dateTimeXMax=PIn.DateT(dataRowX["DateTimeEntry"].ToString());
			dateTimeYMax=PIn.DateT(dataRowY["DateTimeEntry"].ToString());
			return dateTimeXMax.CompareTo(dateTimeYMax);
		}

		///<summary>Compares the AptDateTime of appointments attached to tasks.  Most recently updated tasks will be farther down in the list.
		///If there is no appointment attached, it appears at the bottom. When the ApptStatus is UnschedList then the AptDateTime will be DateTime.MaxValue.</summary>
		public static int CompareAptDateTimes(DataRow dataRowX,DataRow dataRowY) {
			DateTime dateAptX=PIn.DateT(dataRowX["AptDateTime"].ToString());
			DateTime dateAptY=PIn.DateT(dataRowY["AptDateTime"].ToString());
			if(dateAptX==null
				|| (ApptStatus)PIn.Int(dataRowX["AptStatus"].ToString())==ApptStatus.UnschedList)
			{
				dateAptX=DateTime.MaxValue;
			}
			if(dateAptY==null
				|| (ApptStatus)PIn.Int(dataRowY["AptStatus"].ToString())==ApptStatus.UnschedList)
			{
				dateAptY=DateTime.MaxValue;
			}
			return dateAptX.CompareTo(dateAptY);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching taskNum as FKey and are related to Task.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Task table type.</summary>
		public static void ClearFkey(long taskNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskNum);
				return;
			}
			Crud.TaskCrud.ClearFkey(taskNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching taskNums as FKey and are related to Task.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Task table type.</summary>
		public static void ClearFkey(List<long> listTaskNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTaskNums);
				return;
			}
			Crud.TaskCrud.ClearFkey(listTaskNums);
		}

		///<summary>Helper object so that TaskComparer() doesn't have to make deep copies of caches.</summary>
		public class TaskCompareObj {
			public DataRow DataRowTask;
			public List<Def> ListDefsTaskPriority;
		}

		public class TaskAptShort {
			public DateTime AptDateTime;
			public ApptStatus AptStatus;
		}
	}
}
