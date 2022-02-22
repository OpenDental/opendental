using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobNotifications{
		#region Get Methods
		///<summary></summary>
		public static List<JobNotification> Refresh(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobNotification>>(MethodBase.GetCurrentMethod(),jobNum);
			}
			string command = "SELECT * FROM jobnotification WHERE JobNum = "+POut.Long(jobNum);
			return Crud.JobNotificationCrud.SelectMany(command);
		}

		public static List<JobNotification> GetForUser(long userNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobNotification>>(MethodBase.GetCurrentMethod(),userNum);
			}
			string command="SELECT * FROM jobnotification WHERE UserNum = "+POut.Long(userNum);
			return Crud.JobNotificationCrud.SelectMany(command);
		}
		
		///<summary>Gets one JobNotification from the db.</summary>
		public static JobNotification GetOne(long jobNotificationNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<JobNotification>(MethodBase.GetCurrentMethod(),jobNotificationNum);
			}
			return Crud.JobNotificationCrud.SelectOne(jobNotificationNum);
		}
		#endregion
		#region Modification Methods
			#region Insert
		///<summary></summary>
		public static long Insert(JobNotification jobNotification){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				jobNotification.JobNotificationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobNotification);
				return jobNotification.JobNotificationNum;
			}
			return Crud.JobNotificationCrud.Insert(jobNotification);
		}
			#endregion
			#region Update
		///<summary></summary>
		public static void Update(JobNotification jobNotification){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNotification);
				return;
			}
			Crud.JobNotificationCrud.Update(jobNotification);
		}
			#endregion
		#region Delete
		///<summary></summary>
		public static void Delete(long jobNotificationNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNotificationNum);
				return;
			}
			Crud.JobNotificationCrud.Delete(jobNotificationNum);
		}

		public static void DeleteForJobAndUser(long jobNum,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum,userNum);
				return;
			}
			string command="DELETE FROM jobnotification WHERE JobNum = "+POut.Long(jobNum)+" AND UserNum = "+POut.Long(userNum);
			Db.NonQ(command);
		}

		public static void DeleteForJob(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			string command="DELETE FROM jobnotification WHERE JobNum = "+POut.Long(jobNum);
			Db.NonQ(command);
		}

		#endregion
		#endregion
		#region Misc Methods
		public static void UpsertAllNotifications(Job job,long userNumExcluded,JobNotificationChanges changes) {
			if(ListTools.In(job.PhaseCur,JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation)) {
				if(job.ListJobNotifications.Count>0) {
					DeleteForJob(job.JobNum);//Delete all notifications that were on jobs once they are no longer in development
				}
				return;//Don't send notifications after a job has gotten past development
			}
			List<long> listUserNums=job.ListJobLinks.Where(x => x.LinkType==JobLinkType.Subscriber).Select(x => x.FKey)
				.Union(Jobs.GetAssociatedUsers(job).Select(y => y.UserNum)).Distinct().ToList();
			foreach(long userNum in listUserNums) {
				if(userNum==userNumExcluded) {
					continue;
				}
				UpsertNotification(job.JobNum,userNum,changes);
			}
		}

		public static long UpsertNotification(long jobNum,long userNum,JobNotificationChanges changes) {
			//No need for remoting call here.
			JobNotification notification=GetForJobAndUser(jobNum,userNum);
			if(notification==null) {
				notification=new JobNotification();
				notification.JobNum=jobNum;
				notification.UserNum=userNum;
				notification.Changes=changes;
				return Insert(notification);
			}
			else {
				notification.Changes=notification.Changes|changes;
				Update(notification);
				return notification.JobNotificationNum;
			}
		}

		///<summary>Gets a notification for the job and user if it exists. Should only ever be one per Job/User combo. Can return null.</summary>
		public static JobNotification GetForJobAndUser(long jobNum,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<JobNotification>(MethodBase.GetCurrentMethod(),jobNum,userNum);
			}
			string command="SELECT * FROM jobnotification WHERE JobNum = "+POut.Long(jobNum)+" AND UserNum = "+POut.Long(userNum);
			return Crud.JobNotificationCrud.SelectOne(command);
		}

		///<summary>Gets all notifications for a list of jobs.</summary>
		public static List<JobNotification> GetNotificationsForJobs(List<long> listJobNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobNotification>>(MethodBase.GetCurrentMethod(),listJobNums);
			}
			if(listJobNums==null || listJobNums.Count==0) {
				return new List<JobNotification>();
			}
			string command = "SELECT * FROM jobnotification WHERE JobNum IN ("+string.Join(",",listJobNums)+") ";
			return Crud.JobNotificationCrud.SelectMany(command);
		}

		public static string GetNoteForChanges(JobNotificationChanges changes) {
			//No need for remoting call here.
			string note="";
			if(changes.HasFlag(JobNotificationChanges.NoteAdded)) {
				note+="A note was added.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.ConceptChange)) {
				note+="Concept was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.WriteupChange)) {
				note+="Writeup was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.ApprovalChange)) {
				note+="Approval Status was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.EngineerChange)) {
				note+="Engineer was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.ExpertChange)) {
				note+="Expert was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.CategoryChange)) {
				note+="Category was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.PhaseChange)) {
				note+="Phase was changed.\r\n";
			}
			if(changes.HasFlag(JobNotificationChanges.PriorityChange)) {
				note+="Priority was changed.\r\n";
			}
			return note.Trim();
		}
		
		#endregion




	}
}