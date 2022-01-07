using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class TriageMetrics {
		#region Get Methods
		///<summary>Gets the most recent TriageMetric from the db. Can return null.</summary>
		public static TriageMetric GetMostRecent() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<TriageMetric>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM triagemetric ORDER BY triagemetric.DateTStamp DESC LIMIT 1";
			return Crud.TriageMetricCrud.SelectOne(command);
		}
		#endregion
		#region Modification Methods
			#region Insert
		///<summary>Should only be called from the PhoneTrackingServer which will be invoking this every ~1.6 seconds.
		///Inserts a new entry into the triage metric table that all workstations will start to select from in order to fill local metrics.</summary>
		public static void InsertTriageMetric() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//The following query was being run by every workstation in the office every 1.6 seconds which was causing slowness issues.
			//The query might need to be improved but for now we are just removing it from the workstations and having the PTS be the only entity running it.
			string command=@"SELECT 
				COALESCE(SUM(CASE WHEN PriorityDefNum=502 THEN 1 END),0) AS CountBlueTasks,-- triage blue
				COALESCE(SUM(CASE WHEN PriorityDefNum=503 THEN 1 END),0) AS CountWhiteTasks,-- triage white
				COALESCE(SUM(CASE WHEN PriorityDefNum=501 THEN 1 END),0) AS CountRedTasks,-- triage red
				-- time of oldest triage task or the oldest tasknote if one exists
				COALESCE(MIN(CASE WHEN PriorityDefNum=502 THEN (SELECT GREATEST(IFNULL(task.DateTimeEntry,'0001-01-01'), 
					IFNULL((SELECT MAX(DateTimeNote) 
						FROM tasknote WHERE tasknote.tasknum=task.tasknum),'0001-01-01'))) END),'0001-01-01') AS TimeOfOldestBlueTaskNote,
				-- time of oldest urgent task or the oldest tasknote if one exists
				COALESCE(MIN(CASE WHEN PriorityDefNum=501 THEN (SELECT GREATEST(IFNULL(task.DateTimeEntry,'0001-01-01'), 
					IFNULL((SELECT MAX(DateTimeNote) 
						FROM tasknote WHERE tasknote.tasknum=task.tasknum),'0001-01-01'))) END),'0001-01-01') AS TimeOfOldestRedTaskNote
				FROM task 
				WHERE TaskListNum=1697  -- Triage task list
				AND TaskStatus!=2  -- Not done (new or viewed)";
			DataTable table=Db.GetTable(command);
			if(table==null || table.Rows==null) {
				return;
			}
			TriageMetric triageMetric=new TriageMetric() {
				DateTimeOldestTriageTaskOrTaskNote=DateTime.MinValue,
				DateTimeOldestUrgentTaskOrTaskNote=DateTime.MinValue,
			};
			if(table.Rows.Count > 0) {
				triageMetric.CountBlueTasks=PIn.Int(table.Rows[0]["CountBlueTasks"].ToString());
				triageMetric.CountWhiteTasks=PIn.Int(table.Rows[0]["CountWhiteTasks"].ToString());
				triageMetric.CountRedTasks=PIn.Int(table.Rows[0]["CountRedTasks"].ToString());
				triageMetric.DateTimeOldestTriageTaskOrTaskNote=PIn.DateT(table.Rows[0]["TimeOfOldestBlueTaskNote"].ToString());
				triageMetric.DateTimeOldestUrgentTaskOrTaskNote=PIn.DateT(table.Rows[0]["TimeOfOldestRedTaskNote"].ToString());
			}
			//Don't let the chat session information bring down the triage metric system.
			try {
				//Get all of the active web chat sessions.
				JsonSerializerSettings settings=new JsonSerializerSettings();
				settings.DefaultValueHandling=DefaultValueHandling.Ignore;
				triageMetric.WebChatSessions=JsonConvert.SerializeObject(WebChatSessions.GetActiveSessions(),settings);
			}
			catch(Exception ex) {
				ex.DoNothing();
				triageMetric.WebChatSessions="";
			}
			try {
				//Get all of the active Remote Support sessions.
				JsonSerializerSettings settings=new JsonSerializerSettings();
				settings.DefaultValueHandling=DefaultValueHandling.Ignore;
				triageMetric.RemoteSupportSessions=JsonConvert.SerializeObject(PeerInfos.GetActiveSessions(false),settings);
			}
			catch(Exception ex) {
				ex.DoNothing();
				triageMetric.RemoteSupportSessions="";
			}
			Crud.TriageMetricCrud.Insert(triageMetric);
		}
			#endregion
			#region Update
			#endregion
			#region Delete
		///<summary>Should only be called from the PhoneTrackingServer which will be invoking this every ~5 minutes.</summary>
		public static void DeleteAllButMostRecent() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//Do a two step process for simplicity.  Get the most recent triagemetric row from the table and delete all rows that come after it.
			TriageMetric triageMetric=GetMostRecent();
			if(triageMetric==null || triageMetric.DateTStamp==null || triageMetric.DateTStamp.Year < 1880) {
				return;//Nothing to do.
			}
			//Delete all rows in the table that are older than the most recent triagemetric.
			string command="DELETE FROM triagemetric WHERE DateTStamp < "+POut.DateT(triageMetric.DateTStamp);
			Db.NonQ(command);
		}
			#endregion
		#endregion
		#region Misc Methods
		#endregion



	}
}