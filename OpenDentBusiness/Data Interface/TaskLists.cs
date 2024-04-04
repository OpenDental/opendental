using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TaskLists {
		#region Get Methods

		public static List<TaskList> GetMany(List<long> listTaskListNums) {
			if(listTaskListNums.Count==0) {
				return new List<TaskList>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),listTaskListNums);
			}
			string command="SELECT * FROM tasklist WHERE TaskListNum IN("+string.Join(",",listTaskListNums.Select(x => POut.Long(x)))+")";
			return Crud.TaskListCrud.SelectMany(command);
		}

		#endregion

		///<summary>Gets all task lists for the trunk of the user tab.  filterClinicFkey and filterRegionFkey are only used for NewTaskCount and do not 
		///affect which TaskLists are returned by this method.  Pass filterClinicFkey=0 and filterRegionFkey=0 to intentionally bypass filters.</summary>
		public static List<TaskList> RefreshUserTrunk(long userNum,long filterClinicFkey=0,long filterRegionFkey=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),userNum,filterClinicFkey,filterRegionFkey);
			}
			string command=@"SELECT tasklist.*,COALESCE(unreadtasks.Count,0) 'NewTaskCount',t2.Descript 'ParentDesc1',t3.Descript 'ParentDesc2'
				FROM tasklist
				LEFT JOIN tasksubscription ON tasksubscription.TaskListNum=tasklist.TaskListNum
				LEFT JOIN tasklist t2 ON t2.TaskListNum=tasklist.Parent 
				LEFT JOIN tasklist t3 ON t3.TaskListNum=t2.Parent 
				LEFT JOIN (
				SELECT taskancestor.TaskListNum,COUNT(*) 'Count'
				FROM taskancestor
				INNER JOIN task ON task.TaskNum=taskancestor.TaskNum
				AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") ";//no future reminders
			command+=BuildFilterJoins(filterClinicFkey);
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				command+=@"
					INNER JOIN taskunread ON taskunread.TaskNum=task.TaskNum 
					WHERE taskunread.UserNum = "+POut.Long(userNum)+@"
					AND task.TaskStatus!="+POut.Int((int)TaskStatusEnum.Done);
			}
			else {
				command+=@"
					WHERE task.TaskStatus="+POut.Int((int)TaskStatusEnum.New);
			}
			command+=BuildFilterWhereClause(userNum,filterClinicFkey,filterRegionFkey);
			command+=@"
				GROUP BY taskancestor.TaskListNum) unreadtasks ON unreadtasks.TaskListNum = tasklist.TaskListNum 
				WHERE tasksubscription.UserNum="+POut.Long(userNum)+@"
				AND tasksubscription.TaskListNum!=0 
				ORDER BY tasklist.Descript,tasklist.DateTimeEntry";
			return TableToList(Db.GetTable(command));
		}

		///<summary>Gets all task lists for the main trunk.  Pass in the current user.  filterClinicFkey and filterRegionFkey are only used for 
		///NewTaskCount and do not affect which TaskLists are returned by this method.  Pass filterClinicFkey=0 and filterRegionFkey=0  to intentionally
		///bypass filtering.</summary>
		public static List<TaskList> RefreshMainTrunk(long userNum,TaskType taskType,long filterClinicFkey=0,long filterRegionFkey=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),userNum,taskType,filterClinicFkey,filterRegionFkey);
			}
			string command=@"SELECT tasklist.*,COALESCE(unreadtasks.Count,0) 'NewTaskCount' 
				FROM tasklist 
				LEFT JOIN (SELECT tasklist.TaskListNum,COUNT(*) Count 
				FROM tasklist
				INNER JOIN taskancestor ON taskancestor.TaskListNum = tasklist.TaskListNum
				INNER JOIN task ON task.TaskNum = taskancestor.TaskNum ";
			if(taskType==TaskType.Reminder) {
				command+="AND COALESCE(task.ReminderGroupId,'') != '' ";//reminders only
			}
			else if(taskType==TaskType.Normal){
				command+="AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") ";//no future reminders
			}
			else {
				//No filter.
			}
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				command+="AND task.TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
				command+=@"
					LEFT JOIN (
						SELECT TaskListInBox,UserNum 
						FROM userod
						GROUP BY TaskListInBox
					) usr ON usr.TaskListInBox = tasklist.TaskListNum 
					INNER JOIN (
						SELECT TaskNum,UserNum
						FROM taskunread
					) isUnread ON isUnread.TaskNum = task.TaskNum AND (CASE WHEN usr.UserNum IS NOT NULL THEN isUnread.UserNum=usr.UserNum ELSE isUnread.UserNum = "+POut.Long(userNum)+@" END) 
					";
			}
			else {
				command+="AND task.TaskStatus=0 ";
			}
			command+=BuildFilterJoins(filterClinicFkey);
			command+="WHERE TRUE ";//gross.  But makes this query easier to write.
			command+=BuildFilterWhereClause(userNum,filterClinicFkey,filterRegionFkey);
			command+="GROUP BY tasklist.TaskListNum) unreadtasks ON unreadtasks.TaskListNum=tasklist.TaskListNum "
				+"WHERE Parent=0 "
				+"AND DateTL < "+POut.Date(new DateTime(1880,01,01))+" "
				+"AND IsRepeating=0 "
				+"ORDER BY tasklist.Descript,tasklist.DateTimeEntry";
			return TableToList(Db.GetTable(command));
		}

		///<summary>Gets all task lists for the repeating trunk.  filterClinicFkey and filterRegionFkey are only used for NewTaskCount and do not affect
		///which TaskLists are returned by this method.  Pass filterClinicFkey=0 and filterRegionFkey=0 to intentionally bypass filtering.</summary>
		public static List<TaskList> RefreshRepeatingTrunk(long userNum,long filterClinicFkey=0,long filterRegionFkey=0) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),userNum,filterClinicFkey,filterRegionFkey);
			}
			string command="SELECT tasklist.*,"
				+"(SELECT COUNT(*) FROM taskancestor,task ";
			command+=BuildFilterJoins(filterClinicFkey);
			command+="WHERE taskancestor.TaskListNum=tasklist.TaskListNum "
				+"AND task.TaskNum=taskancestor.TaskNum AND task.TaskStatus="+POut.Int((int)TaskStatusEnum.New)+" "
				+"AND COALESCE(task.ReminderGroupId,'')='' ";
			command+=BuildFilterWhereClause(userNum,filterClinicFkey,filterRegionFkey);
			command+=") NewTaskCount "//No reminder tasks
				//I don't think the repeating trunk would ever track by user, so no special treatment here.
				//Acutual behavior in both cases needs to be tested.
				+"FROM tasklist "
				+"WHERE Parent=0 "
				+"AND DateTL < "+POut.Date(new DateTime(1880,01,01))+" "
				+"AND IsRepeating=1 "
				+"ORDER BY tasklist.Descript,tasklist.DateTimeEntry";
			return TableToList(Db.GetTable(command));
		}

		///<summary>0 is not allowed, because that would be a trunk.  Pass in the current user.  Also, if this is in someone's inbox, then pass in the 
		///userNum whose inbox it is in.  If not in an inbox, pass in 0.  filterClinicFkey and filterRegionFkey are only used for NewTaskCount and do 
		///not affect which TaskLists are returned by this method.  Pass filterClinicFkey=0 and filterRegionFkey=0 to intentionally bypass filtering.
		///</summary>
		public static List<TaskList> RefreshChildren(long parent,long userNum,long userNumInbox,TaskType taskType,long filterClinicFkey=0
			,long filterRegionFkey=0) 
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),parent,userNum,userNumInbox,taskType,filterClinicFkey,filterRegionFkey);
			}
			string command="SELECT tasklist.*,"
				+"(SELECT COUNT(*) FROM taskancestor INNER JOIN task ON task.TaskNum=taskancestor.TaskNum ";
			command+=BuildFilterJoins(filterClinicFkey);
			command+="WHERE taskancestor.TaskListNum=tasklist.TaskListNum ";
			if(taskType==TaskType.Reminder) {
				command+="AND COALESCE(task.ReminderGroupId,'') != '' ";//reminders only
			}
			else if(taskType==TaskType.Normal){
				command+="AND NOT(COALESCE(task.ReminderGroupId,'') != '' AND task.DateTimeEntry > "+DbHelper.Now()+") ";//no future reminders
			}
			else {
				//No filter.
			}
			if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
				command+="AND EXISTS(SELECT * FROM taskunread WHERE taskunread.TaskNum=task.TaskNum ";
				//If a task is marked done, we don't care if it is unread.  Usually if a task is done all the taskunreads will be cleared.
				//Added this for an HQ issue where tasklists always showed you had an unread task, even though there were not any open unread tasks.
				command+="AND task.TaskStatus!="+POut.Int((int)TaskStatusEnum.Done)+" ";
				//if a list is someone's inbox,
				if(userNumInbox>0) {
					//then restrict by that user
					command+="AND taskunread.UserNum="+POut.Long(userNumInbox)+") ";
				}
				else {
					//otherwise, restrict by current user
					command+="AND taskunread.UserNum="+POut.Long(userNum)+") ";
				}
			}
			else {
				command+="AND task.TaskStatus="+POut.Int((int)TaskStatusEnum.New);
			}
			command+=BuildFilterWhereClause(userNum,filterClinicFkey,filterRegionFkey);
			command+=") NewTaskCount "
				+"FROM tasklist "
				+"WHERE Parent="+POut.Long(parent)+" "
				+"ORDER BY tasklist.Descript,tasklist.DateTimeEntry";
			return TableToList(Db.GetTable(command));
		}

		///<summary>All repeating items for one date type with no heirarchy.  filterClinicFkey and filterRegionFkey are only used for NewTaskCount and do
		///not affect which TaskLists are returned by this method.  Pass filterClinicFkey=0 and filterRegionFkey=0 to intentionally bypass filtering.</summary>
		public static List<TaskList> RefreshRepeating(TaskDateType taskDateType,long userNum,long filterClinicFkey=0,long filterRegionFkey=0)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),taskDateType,userNum,filterClinicFkey,filterRegionFkey);
			}
			string command=
				"SELECT tasklist.*,"
				+"(SELECT COUNT(*) FROM taskancestor,task ";
			command+=BuildFilterJoins(filterClinicFkey);
			command+="WHERE taskancestor.TaskListNum=tasklist.TaskListNum "
				+"AND task.TaskNum=taskancestor.TaskNum AND task.TaskStatus="+POut.Int((int)TaskStatusEnum.New)+" "
				+"AND COALESCE(task.ReminderGroupId,'')='' ";
			command+=BuildFilterWhereClause(userNum,filterClinicFkey,filterRegionFkey);
			command+=") 'NewTaskCount' "//No reminder tasks
				//See the note in RefreshRepeatingTrunk.  Behavior needs to be tested.
				+"FROM tasklist "
				+"WHERE IsRepeating=1 "
				+"AND DateType="+POut.Long((int)taskDateType)+" "
				+"ORDER BY tasklist.Descript,tasklist.DateTimeEntry";
			return TableToList(Db.GetTable(command));
		}

		///<summary>Gets all task lists for one of the 3 dated trunks.  filterClinicFkey and filterRegionFkey are only used for NewTaskCount and do not 
		///affect which TaskLists are returned by this method.  Pass filterClinicFkey=0 and filterRegionFkey=0 to intentionally bypass filtering.</summary>
		public static List<TaskList> RefreshDatedTrunk(DateTime date,TaskDateType taskDateType,long userNum,long filterClinicFkey=0,long filterRegionFkey=0)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),date,taskDateType,userNum,filterClinicFkey,filterRegionFkey);
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
				"SELECT tasklist.*,"
				+"(SELECT COUNT(*) FROM taskancestor,task ";
			command+=BuildFilterJoins(filterClinicFkey);
			command+="WHERE taskancestor.TaskListNum=tasklist.TaskListNum "
				+"AND task.TaskNum=taskancestor.TaskNum "
				+"AND COALESCE(task.ReminderGroupId,'')='' ";//No reminder tasks
			//if(PrefC.GetBool(PrefName.TasksNewTrackedByUser)) {
			//	command+="AND EXISTS(SELECT * FROM taskunread WHERE taskunread.TaskNum=task.TaskNum)";
			//}
			//else {
			command+="AND task.TaskStatus="+POut.Int((int)TaskStatusEnum.New);
			//}
			command+=BuildFilterWhereClause(userNum,filterClinicFkey,filterRegionFkey);
			command+=") 'NewTaskCount' "
				+"FROM tasklist "
				+"WHERE DateTL >= "+POut.Date(dateFrom)
				+" AND DateTL <= "+POut.Date(dateTo)
				+" AND DateType="+POut.Long((int)taskDateType)
				+" ORDER BY tasklist.Descript,tasklist.DateTimeEntry";
			return TableToList(Db.GetTable(command));
		}

			///<summary>Builds JOIN clauses appropriate to the type of GlobalFilterType.  Returns empty string if not filtering.  Pass filterClinicFkey=0 
			///to intentionally bypass filtering.</summary>
		public static string BuildFilterJoins(long filterClinicFkey) {
			string command=string.Empty;
			//Only add JOINs if filtering.  Filtering will never happen if clinics are turned off, because regions link via clinics.
			if((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==EnumTaskFilterType.Disabled || filterClinicFkey==0 
				|| !PrefC.HasClinicsEnabled) 
			{
				return command;
			}
			command+=" LEFT JOIN patient ON task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" AND task.KeyNum=patient.PatNum ";
			command+=" INNER JOIN tasklist tasklistfortask ON task.TaskListNum=tasklistfortask.TaskListNum ";
			command+=" LEFT JOIN appointment ON task.ObjectType="+POut.Int((int)TaskObjectType.Appointment)+" AND task.KeyNum=appointment.AptNum ";
			return command;
		}

		///<summary>Builds WHERE clauses appropriate to the type of GlobalFilterType.  Returns empty string if not filtering.  Pass filterClinicFkey=0 
		///and filterRegionFkey=0 to intentionally bypass filtering.</summary>
		public static string BuildFilterWhereClause(long userNum,long filterClinicFkey,long filterRegionFkey) {
			string command=string.Empty;
			//Only add WHERE clauses if filtering.  Filtering will never happen if clinics are turned off, because regions link via clinics.
			if((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType)==EnumTaskFilterType.Disabled 
				|| (filterClinicFkey==0 && filterRegionFkey==0) || !PrefC.HasClinicsEnabled) 
			{
				return command;
			}
			List<Clinic> listClinicsUnrestricted=Clinics.GetAllForUserod(Userods.GetUser(userNum));
			List<long> listClinicNums=new List<long>() { 0 };//All users can see Tasks associated to HQ clinic or "0" region.
			List<long> listClinicNumsInRegion=new List<long>() { 0 };//All users can see Tasks associated to HQ clinic or "0" region.
			List<long> listClinicNumsUnrestricted=listClinicsUnrestricted.Select(x => x.ClinicNum).ToList();//User can view these clinicnums.
			List<long> listClinicNumsUnrestrictedInRegion=listClinicsUnrestricted.FindAll(x => x.Region==filterRegionFkey).Select(x => x.ClinicNum).ToList();
			if(listClinicNumsUnrestricted.Contains(filterClinicFkey)) {//Make sure user is not restricted for this clinic.
				listClinicNums.Add(filterClinicFkey);
			}
			listClinicNumsInRegion.AddRange(listClinicNumsUnrestrictedInRegion);
			string strClinicFilterNums=string.Join(",",listClinicNums.Select(x => POut.Long(x)));
			string strRegionFilterNums=string.Join(",",listClinicNumsInRegion.Select(x => POut.Long(x)));
			//Clause for TaskLists that have Default filter.
			string cmdFilterTaskListByDefault="(tasklistfortask.GlobalTaskFilterType IN ("
				+POut.Long((long)EnumTaskFilterType.Disabled)+","+POut.Long((long)EnumTaskFilterType.Default)+")"//Disabled is treated as Default for tasklists.
				+GetDefaultFilterTypeString((EnumTaskFilterType)PrefC.GetInt(PrefName.TasksGlobalFilterType),strClinicFilterNums,strRegionFilterNums)+") ";
			//Clause for TaskLists that have None filter.
			string cmdFilterTaskListByNone="(tasklistfortask.GlobalTaskFilterType="+POut.Long((long)EnumTaskFilterType.None)+")";
			//Clause for TaskLists that have Clinic filter.
			string cmdFilterTaskListByClinic="(tasklistfortask.GlobalTaskFilterType="+POut.Long((long)EnumTaskFilterType.Clinic)
				+" AND (patient.ClinicNum IN ("+strClinicFilterNums+") OR appointment.ClinicNum IN ("+strClinicFilterNums+"))) ";
			//Clause for TaskLists that have Region filter.
			string cmdFilterTaskListByRegion="(tasklistfortask.GlobalTaskFilterType="+POut.Long((long)EnumTaskFilterType.Region)
				+" AND (patient.ClinicNum IN ("+strRegionFilterNums+") OR appointment.ClinicNum IN ("+strRegionFilterNums+"))) ";
			//Clause for Tasks that are not connected to a patient or clinic.
			string cmdTaskClinicIsNull="((patient.ClinicNum IS NULL) AND (appointment.ClinicNum IS NULL))";
			command=" AND ("+cmdFilterTaskListByDefault+" OR "+cmdFilterTaskListByNone+" OR "+cmdFilterTaskListByClinic+" OR "
				+cmdFilterTaskListByRegion+"OR "+cmdTaskClinicIsNull+") ";
			return command;
		}

		///<summary>Builds a short section of the GlobalTaskFilterType WHERE clause.  Determines which clinics to filter by depending on the global 
		///default GlobalTaskFilterType.</summary>
		private static string GetDefaultFilterTypeString(EnumTaskFilterType globalTaskFilterTypeDefault,string strClinicNums,string strClinicNumsInRegion) {
			string command="";
			switch(globalTaskFilterTypeDefault) {
				case EnumTaskFilterType.Clinic:
					command=" AND (patient.ClinicNum IN ("+strClinicNums+") OR appointment.ClinicNum IN ("+strClinicNums+"))";
					break;
				case EnumTaskFilterType.Region:
					command=" AND (patient.ClinicNum IN ("+strClinicNumsInRegion+") OR appointment.ClinicNum IN ("+strClinicNumsInRegion+"))";
					break;
				case EnumTaskFilterType.None:
				default:
					break;
			}
			return command;
		}
		
		///<summary></summary>
		public static TaskList GetOne(long taskListNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<TaskList>(MethodBase.GetCurrentMethod(),taskListNum);
			}
			if(taskListNum==0){
				return null;
			}
			string command="SELECT * FROM tasklist WHERE TaskListNum="+POut.Long(taskListNum);
			return Crud.TaskListCrud.SelectOne(command);
		}

		///<summary>Gets all task lists from the database.</summary>
		public static List<TaskList> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM tasklist";
			return Crud.TaskListCrud.SelectMany(command);
		}

		///<summary>Gets all task lists from the database for a certain DateType.
		///If doIncludeArchived is false, also excludes child lists of archived lists.</summary>
		public static List<TaskList> GetForDateType(TaskDateType taskDateType,bool doIncludeArchived) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),taskDateType,doIncludeArchived);
			}
			List<TaskList> listTaskLists=GetAll();
			Dictionary<long,TaskList> dictAllTaskLists=listTaskLists.ToDictionary(x => x.TaskListNum);
			listTaskLists.RemoveAll(x => x.DateType!=taskDateType ||
				//We are excluding archived lists AND The current task list or any of its ancestors are archived
				(!doIncludeArchived && (x.TaskListStatus==TaskListStatusEnum.Archived || IsAncestorTaskListArchived(ref dictAllTaskLists,x,true))));
			return listTaskLists;
		}

		///<summary>Get TaskListNums based on description.</summary>
		public static List<long> GetNumsByDescription(string descript,bool doRunOnReportServer) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),descript,doRunOnReportServer);
			}
			string command="SELECT TaskListNum FROM tasklist WHERE Descript LIKE '%"+POut.String(descript)+"%'";
			return ReportsComplex.RunFuncOnReportServer(() => Db.GetListLong(command),doRunOnReportServer);
		}

		/*
		///<Summary>Gets all task lists in the general tab with no heirarchy.  This allows us to loop through the list to grab useful heirarchy info.  Only used when viewing user tab.  Not guaranteed to get all tasklists, because we exclude those with a DateType.</Summary>
		public static List<TaskList> GetAllGeneral(){
//THIS WON'T WORK BECAUSE THERE ARE TOO MANY REPEATING TASKLISTS.
			string command="SELECT * FROM tasklist WHERE DateType=0 ";
		}*/

		///<summary>The table passed in can contain additional columns: "NewTaskCount", "ParentDesc1", "ParentDesc2".  These additional columns are used 
		///when getting a list of task lists for trunks.</summary>
		private static List<TaskList> TableToList(DataTable table){
			//No need to check MiddleTierRole; no call to db.
			List<TaskList> listTaskLists=Crud.TaskListCrud.TableToList(table);
			string desc;
			//Check if the table passed in contains any of the special columns used to fill task lists for trunks.  If not, simply return.
			if(!table.Columns.Contains("NewTaskCount")) {
				return listTaskLists;
			}
			//The following ParentDesc columns are optional.
			bool hasParentDesc=false;
			if(table.Columns.Contains("ParentDesc1")
				&& table.Columns.Contains("ParentDesc2")) 
			{
				hasParentDesc=true;
			}
			//There were special non db columns passed in that need to be set.
			//Loop through the entire list of task lists and set their corresponding non db columns to the values of the table passed in.
			for(int i=0;i<listTaskLists.Count;i++) {
				//We know at this point that the table passed in contains a NewTaskCount column.
				listTaskLists[i].NewTaskCount=PIn.Int(table.Rows[i]["NewTaskCount"].ToString());
				if(!hasParentDesc) {
					continue;
				}
				//Check for optional parent descriptions.
				//Create visual heirarchy of tasklists
				desc=PIn.String(table.Rows[i]["ParentDesc1"].ToString());
				if(desc!="") {
					listTaskLists[i].ParentDesc=desc+"/";
				}
				desc=PIn.String(table.Rows[i]["ParentDesc2"].ToString());
				if(desc!="") {
					listTaskLists[i].ParentDesc=desc+"/"+listTaskLists[i].ParentDesc;
				}
			}
			return listTaskLists;
		}

		/// <summary>Gets all task lists with the given object type.
		/// Used in TaskListSelect when assigning an object to a task list. If doIncludeArchived is false, also excludes child lists of archived lists.
		/// </summary>
		public static List<TaskList> GetForObjectType(TaskObjectType taskObjectType,bool doIncludeArchived) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TaskList>>(MethodBase.GetCurrentMethod(),taskObjectType,doIncludeArchived);
			}
			List<TaskList> listTaskLists=GetAll();
			Dictionary<long,TaskList> dictAllTaskLists=listTaskLists.ToDictionary(x => x.TaskListNum);
			listTaskLists.RemoveAll(x => x.ObjectType!=taskObjectType ||
				//We are excluding archived lists AND The current task list or any of its ancestors are archived
				(!doIncludeArchived && (x.TaskListStatus==TaskListStatusEnum.Archived || IsAncestorTaskListArchived(ref dictAllTaskLists,x,true))));
			return listTaskLists.OrderBy(x => x.Descript).ToList();
		}

		private static void ValidateTaskList(TaskList taskList) {
			if(taskList.IsRepeating && taskList.DateTL.Year>1880) {
				throw new Exception(Lans.g("TaskLists","TaskList cannot be tagged repeating and also have a date."));
			}
			if(taskList.Parent==0 && taskList.DateTL.Year>1880 && taskList.DateType==TaskDateType.None) {//it would not show anywhere, so it would be 'lost'
				throw new Exception(Lans.g("TaskLists","A TaskList with a date must also have a type selected."));
			}
			if(taskList.IsRepeating && taskList.Parent!=0 && taskList.DateType!=TaskDateType.None) {//In repeating, children not allowed to repeat.
				throw new Exception(Lans.g("TaskLists","In repeating tasklists, only the main parents can have a task status."));
			}
		}

		///<summary></summary>
		public static void Update(TaskList taskList){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskList);
				return;
			}
			ValidateTaskList(taskList);
			Crud.TaskListCrud.Update(taskList);
		}

		public static void Update(TaskList taskList,TaskList taskListOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskList,taskListOld);
				return;
			}
			ValidateTaskList(taskList);
			Crud.TaskListCrud.Update(taskList,taskListOld);
		}

		///<summary></summary>
		public static long Insert(TaskList taskList) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				taskList.TaskListNum=Meth.GetLong(MethodBase.GetCurrentMethod(),taskList);
				return taskList.TaskListNum;
			}
			ValidateTaskList(taskList);
			return Crud.TaskListCrud.Insert(taskList);
		}

		///<summary>Throws exception if any child tasklists or tasks.</summary>
		public static void Delete(TaskList taskList){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),taskList);
				return;
			}
			string command="SELECT COUNT(*) FROM tasklist WHERE Parent="+POut.Long(taskList.TaskListNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="0"){
				throw new Exception(Lans.g("TaskLists","Not allowed to delete task list because it still has child lists attached."));
			}
			command="SELECT COUNT(*) FROM task WHERE TaskListNum="+POut.Long(taskList.TaskListNum);
			table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="0"){
				throw new Exception(Lans.g("TaskLists","Not allowed to delete task list because it still has child tasks attached."));
			}
			command="SELECT COUNT(*) FROM userod WHERE TaskListInBox="+POut.Long(taskList.TaskListNum);
			table=Db.GetTable(command);
			if(table.Rows[0][0].ToString()!="0"){
				throw new Exception(Lans.g("TaskLists","Not allowed to delete task list because it is attached to a user inbox."));
			}
			command= "DELETE from tasklist WHERE TaskListNum = '"
				+POut.Long(taskList.TaskListNum)+"'";
 			Db.NonQ(command);
		}

		///<summary>Returns true if the first TaskListNum passed in has a child list with the second TaskListNum passed in.</summary>
		public static bool IsAncestor(long taskListNum,long taskListNumChild) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),taskListNum,taskListNumChild);
			}
			long parentNum=taskListNumChild;
			while(true) {
				parentNum=PIn.Long(Db.GetScalar("SELECT parent FROM tasklist WHERE TaskListNum="+POut.Long(parentNum)));
				if(parentNum==0) {
					return false;//Got to the top level of the tree for this list and it is the main list.
				}
				if(parentNum==taskListNum) {
					return true;//Got to the TaskList whose parent is the one we are looking for.
				}
			}
		}

		///<summary>Returns true if taskList or one of its children TaskLists have a GlobalFilterType.</summary>
		public static bool HasGlobalFilterTypeInTree(TaskList taskList,List<TaskList> listTaskListsAll=null) {
			if(taskList.GlobalTaskFilterType!=EnumTaskFilterType.None) {
				return true;
			}
			List<TaskList> listTaskListsChild=(listTaskListsAll??GetAll()).FindAll(x => x.Parent==taskList.TaskListNum).ToList();
			for(int i=0;i<listTaskListsAll.Count;i++) {
				return HasGlobalFilterTypeInTree(listTaskListsAll[i],listTaskListsAll);
			}
			return false;
		}

		///<summary>Will return 0 if not anyone's inbox.</summary>
		public static long GetMailboxUserNum(long taskListNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),taskListNum);
			}
			string command="SELECT UserNum FROM userod WHERE TaskListInBox="+POut.Long(taskListNum);
			return PIn.Long(Db.GetScalar(command));
		}

		///<summary>Checks all ancestors of a task.  Will return 0 if no ancestor is anyone's inbox.</summary>
		public static long GetMailboxUserNumByAncestor(long taskNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),taskNum);
			}
			string command="SELECT UserNum FROM taskancestor,userod "
				+"WHERE taskancestor.TaskListNum=userod.TaskListInBox "
				+"AND taskancestor.TaskNum="+POut.Long(taskNum);
			return PIn.Long(Db.GetScalar(command));
		}
		
		///<summary>Build the full path to the passed in task list.  Returns the string in the standard Windows path format.</summary>
		public static string GetFullPath(long tasklistNum,List<TaskList> listTaskLists=null) {
			//No need to check MiddleTierRole; no call to db.
			StringBuilder stringBuilder=new StringBuilder();
			TaskList taskList=null;
			if(listTaskLists!=null && listTaskLists.Count>0) {
				taskList=listTaskLists.FirstOrDefault(x => x.TaskListNum==tasklistNum);
			}
			else {
				taskList=GetOne(tasklistNum);
			}
			if(taskList==null) {
				return "";
			}
			stringBuilder.Append(taskList.Descript);
			while(true) {
				if(taskList.Parent==0) {
					break;
				}
				if(listTaskLists!=null) {
					taskList=listTaskLists.FirstOrDefault(x => x.TaskListNum==taskList.Parent);
				}
				else {
					taskList=GetOne(taskList.Parent);
				}
				if(taskList==null) {
					break;
				}
				stringBuilder.Insert(0,taskList.Descript+"/");
			}
			return stringBuilder.ToString();
		}

		///<summary>TaskListStatus to 1 - Archived, and set all Task List Inboxes that reference this Task List to 0.</summary>
		public static void Archive(TaskList taskList) {
			//No need to check MiddleTierRole; no call to db.
			if(taskList.TaskListStatus!=TaskListStatusEnum.Active) {
				return;
			}
			TaskList taskListOld=taskList.Copy();
			taskList.TaskListStatus=TaskListStatusEnum.Archived;
			Update(taskList,taskListOld);
			Tasks.DisableRemindersFromTasklist(taskList.TaskListNum);
			TaskSubscriptions.RemoveAllSubscribers(taskList.TaskListNum);
			Userods.DisassociateTaskListInBox(taskList.TaskListNum);
			Signalods.SetInvalid(InvalidType.Security);//Send a signal in case any userod was associated to the task list.
		}

		///<summary>Set the TaskListStatus to 0 - Active.</summary>
		public static void Unarchive(TaskList taskList) {
			//No need to check MiddleTierRole; no call to db.
			if(taskList.TaskListStatus!=TaskListStatusEnum.Archived) {
				return;
			}
			TaskList taskListOld=taskList.Copy();
			taskList.TaskListStatus=TaskListStatusEnum.Active;
			Update(taskList,taskListOld);
		}

		/// <summary>False if taskList has no parent, all of taskList's ancestors are not archived, or taskList ancestor can't be found.</summary>
		public static bool IsAncestorTaskListArchived(ref Dictionary<long,TaskList> dictAllTaskLists,TaskList taskList,bool isDictionaryRefreshed=false) {
			//No need to check MiddleTierRole; no call to db.
			//==Jordan This is a bad pattern. Should be done without dictionary or ref.
			if(taskList.Parent==0) {//If list has no parent return false.
				return false;
			}
			if(dictAllTaskLists.ContainsKey(taskList.Parent)) {//If list parent is in dictionary
				if(dictAllTaskLists[taskList.Parent].TaskListStatus==TaskListStatusEnum.Archived) {//and parent is archived return true
					return true;
				}
				//Check if grandparent is archived with current value of isDictionaryRefreshed
				return IsAncestorTaskListArchived(ref dictAllTaskLists,dictAllTaskLists[taskList.Parent],isDictionaryRefreshed);
			}
			if(!isDictionaryRefreshed) {//Refresh dictionary once in recursion if parent is missing from dictionary and try again with current list
				dictAllTaskLists=GetAll().ToDictionary(x => x.TaskListNum);
				return IsAncestorTaskListArchived(ref dictAllTaskLists,taskList,true);
			}
			return false;//List was refreshed and parent couldn't be found.
		}

		/// <summary>False if taskList has no parent, all of taskList's ancestors are not archived, or taskList ancestor can't be found.</summary>
		public static bool IsAnchorTaskListArchived2(List<TaskList> listTaskListsAll,TaskList taskList) {
			//No need to check MiddleTierRole; no call to db.
			if(taskList.Parent==0) {//If list has no parent return false.
				return false;
			}
			TaskList taskListParent=listTaskListsAll.Find(x => x.TaskListNum==taskList.Parent);
			if(taskListParent!=null) {//If list parent is present
				if(taskListParent.TaskListStatus==TaskListStatusEnum.Archived) {//and parent is archived return true
					return true;
				}
				//Check if grandparent is archived
				return IsAnchorTaskListArchived2(listTaskListsAll,taskListParent);
			}
			return false;//Parent couldn't be found
		}

	}
}