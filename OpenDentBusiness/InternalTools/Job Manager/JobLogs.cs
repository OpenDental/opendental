using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class JobLogs{
		#region Get Methods
		#endregion

		#region Modification Methods
		
		#region Insert
		#endregion

		#region Update
		#endregion

		#region Delete
		#endregion

		#endregion

		#region Misc Methods
		#endregion

		/// <summary>Inserts log entry to DB and returns the resulting JobLog.  Returns null if no log was needed.</summary>
		public static JobLog MakeLogEntry(Job jobNew,Job jobOld,bool isManualLog=false) {
			if(jobNew==null) {
				return null;
			}
			JobNotificationChanges changes=new JobNotificationChanges();
			JobLog jobLog = new JobLog() {
				JobNum=jobNew.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobNew.UserNumExpert,
				UserNumEngineer=jobNew.UserNumEngineer,
				Description="",
				TimeEstimate=TimeSpan.FromHours(jobNew.HoursEstimate)
			};
			if(isManualLog) {
				jobLog.Description="Manual \"last worked on\" update";
				JobLogs.Insert(jobLog);
				return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
			}
			List<string> logDescriptions = new List<string>();
			if(jobOld.IsApprovalNeeded && !jobNew.IsApprovalNeeded) {
				if(jobOld.PhaseCur==JobPhase.Concept  && (jobNew.PhaseCur==JobPhase.Definition || jobNew.PhaseCur==JobPhase.Development)) {
					logDescriptions.Add("Concept approved.");
					jobLog.MainRTF=jobNew.Implementation;
					jobLog.RequirementsRTF+=jobNew.Requirements;
					changes=changes|JobNotificationChanges.ApprovalChange;
				}
				if((jobOld.PhaseCur==JobPhase.Concept || jobOld.PhaseCur==JobPhase.Definition) && jobNew.PhaseCur==JobPhase.Development) {
					logDescriptions.Add("Job approved.");
					jobLog.MainRTF=jobNew.Implementation;
					jobLog.RequirementsRTF+=jobNew.Requirements;
					changes=changes|JobNotificationChanges.ApprovalChange;
				}
				if(jobOld.PhaseCur==JobPhase.Development && jobNew.PhaseCur==JobPhase.Development) {
					logDescriptions.Add("Changes approved.");
					jobLog.MainRTF=jobNew.Implementation;
					jobLog.RequirementsRTF+=jobNew.Requirements;
					changes=changes|JobNotificationChanges.ApprovalChange;
				}
			}
			else if(ListTools.In(jobNew.PhaseCur,JobPhase.Documentation,JobPhase.Complete) && !ListTools.In(jobOld.PhaseCur,JobPhase.Documentation,JobPhase.Complete)) {
				logDescriptions.Add("Job implemented.");
				jobLog.MainRTF+=jobNew.Implementation;
				jobLog.RequirementsRTF+=jobNew.Requirements;
			}
			if(jobOld.PhaseCur>jobNew.PhaseCur && jobOld.PhaseCur!=JobPhase.Cancelled) {
				logDescriptions.Add("Job Unapproved.");//may be a chance for a false positive when using override permission.
			}
			if(jobOld.PhaseCur!=JobPhase.Cancelled && jobNew.PhaseCur==JobPhase.Cancelled) {
				logDescriptions.Add("Job Cancelled.");//may be a chance for a false positive when using override permission.
			}
			if(jobNew.UserNumExpert!=jobOld.UserNumExpert) {
				logDescriptions.Add("Expert changed.");
				changes=changes|JobNotificationChanges.ExpertChange;
			}
			if(jobNew.UserNumEngineer!=jobOld.UserNumEngineer) {
				logDescriptions.Add("Engineer changed.");
				changes=changes|JobNotificationChanges.EngineerChange;
			}
			if(jobOld.Requirements!=jobNew.Requirements) {
				logDescriptions.Add("Job Requirements Changed.");
				jobLog.RequirementsRTF+=jobNew.Requirements;
				changes=changes|JobNotificationChanges.ConceptChange;
			}
			if(jobOld.Implementation!=jobNew.Implementation) {
				logDescriptions.Add("Job Implementation Changed.");
				jobLog.MainRTF+=jobNew.Implementation;
				changes=changes|JobNotificationChanges.WriteupChange;
			}
			//Do not log RequirementsJSON changes here.
			//if(jobOld.RequirementsJSON!=jobNew.RequirementsJSON) {
			//	logDescriptions.Add("Job Requirements List Changed.");
			//	changes=changes|JobNotificationChanges.ConceptChange;
			//}
			if(jobOld.Title!=jobNew.Title) {
				logDescriptions.Add("Job Title Changed.");
			}
			if(jobOld.HoursEstimate!=jobNew.HoursEstimate) {
				logDescriptions.Add("Job Estimate Changed from "+jobOld.HoursEstimate.ToString()+" hour(s) to "+jobNew.HoursEstimate.ToString()+" hour(s).");
			}
			jobLog.Title=jobNew.Title;
			jobLog.Description=string.Join("\r\n",logDescriptions);
			if(string.IsNullOrEmpty(jobLog.Description)) {
				return null;
			}
			JobLogs.Insert(jobLog);
			JobNotifications.UpsertAllNotifications(jobNew,Security.CurUser.UserNum,changes);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForRequirementApproval(Job job) {
			List<JobRequirement> listRequirements=JsonConvert.DeserializeObject<List<JobRequirement>>(job.RequirementsJSON);
			string logText="Requirements Approved\r\n";
			foreach(JobRequirement jobReq in listRequirements) {
				logText+="Requirement: "+jobReq.Description+"\r\n";
			}
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=job.Title,
				Description=logText,
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForTesting(Job job,string logText) {
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=job.Title,
				Description=logText,
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		///<summary>Makes a joblog entry for the passed in job, queries the database for the joblog that was just created and then returns it.
		///Does not make a joblog for Complete or Cancelled jobs.  Instead, simply returns null indicating that nothing happened.</summary>
		public static JobLog MakeLogEntryForView(Job job) {
			if(ListTools.In(job.PhaseCur,JobPhase.Complete,JobPhase.Cancelled)) {
				return null;
			}
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=job.Title,
				Description="Job Viewed",
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForSaveCommit(Job job) {
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=job.Title,
				Description="Job Save Committed",
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForNote(Job job,JobNote jobNoteNew,JobNote jobNoteOld) {
			string note="";
			if(jobNoteNew==null) {
				note="Discussion note by user: "+Userods.GetName(jobNoteOld.UserNum)+" on "+jobNoteOld.DateTimeNote.ToString()+" was deleted.\r\nMessage Text:\r\n"
					+jobNoteOld.Note;
			}
			else {
				note="Discussion note by user: "+Userods.GetName(jobNoteOld.UserNum)+" on "+jobNoteOld.DateTimeNote.ToString()+" was edited.";
			}
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=job.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForPriority(Job jobNew,Job jobOld) {
			string note="Job Priority was changed from "+Defs.GetName(DefCat.JobPriorities,jobOld.Priority)+" to "+Defs.GetName(DefCat.JobPriorities,jobNew.Priority)+".";
			JobLog jobLog = new JobLog() {
				JobNum=jobOld.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobOld.UserNumExpert,
				UserNumEngineer=jobOld.UserNumEngineer,
				Title=jobOld.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(jobNew.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			JobNotifications.UpsertAllNotifications(jobNew,Security.CurUser.UserNum,JobNotificationChanges.PriorityChange);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForPhase(Job jobNew,Job jobOld) {
			string note="Job Phase was changed from "+jobOld.PhaseCur.ToString()+" to "+jobNew.PhaseCur.ToString()+".";
			JobLog jobLog = new JobLog() {
				JobNum=jobOld.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobOld.UserNumExpert,
				UserNumEngineer=jobOld.UserNumEngineer,
				Title=jobOld.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(jobNew.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			JobNotifications.UpsertAllNotifications(jobNew,Security.CurUser.UserNum,JobNotificationChanges.PhaseChange);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForProject(Job jobNew,Job jobOld) {
			string note=$@"Job Project was manually changed from {jobOld.PatternReviewProject.ToString()} to {jobNew.PatternReviewProject.ToString()}.";
			JobLog jobLog = new JobLog() {
				JobNum=jobOld.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobOld.UserNumExpert,
				UserNumEngineer=jobOld.UserNumEngineer,
				Title=jobOld.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(jobNew.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			JobNotifications.UpsertAllNotifications(jobNew,Security.CurUser.UserNum,JobNotificationChanges.PhaseChange);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForPatternReview(Job jobNew,Job jobOld) {
			string note="Job Pattern Review Status was manually changed from "+jobOld.PatternReviewStatus.ToString()+" to "+jobNew.PatternReviewStatus.ToString()+".";
			JobLog jobLog = new JobLog() {
				JobNum=jobOld.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobOld.UserNumExpert,
				UserNumEngineer=jobOld.UserNumEngineer,
				Title=jobOld.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(jobNew.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			JobNotifications.UpsertAllNotifications(jobNew,Security.CurUser.UserNum,JobNotificationChanges.PhaseChange);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static JobLog MakeLogEntryForCategory(Job jobNew,Job jobOld) {
			string note="Job Category was changed from "+jobOld.Category.ToString()+" to "+jobNew.Category.ToString()+".";
			JobLog jobLog = new JobLog() {
				JobNum=jobOld.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobOld.UserNumExpert,
				UserNumEngineer=jobOld.UserNumEngineer,
				Title=jobOld.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(jobNew.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			JobNotifications.UpsertAllNotifications(jobNew,Security.CurUser.UserNum,JobNotificationChanges.CategoryChange);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		///<summary>Will return null if no changes were made.</summary>
		public static bool MakeLogEntryForEstimateChange(Job jobNew,Job jobOld,string note) {
			JobLog jobLog = new JobLog() {
				JobNum=jobNew.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=jobNew.UserNumExpert,
				UserNumEngineer=jobNew.UserNumEngineer,
				Title=jobNew.Title,
				Description="",
				TimeEstimate=jobNew.TimeEstimateConcept+jobNew.TimeEstimateWriteup+jobNew.TimeEstimateDevelopment+jobNew.TimeEstimateReview+jobNew.TimeEstimateReview
			};//Add review twice
			if(jobOld.HoursEstimateConcept!=jobNew.HoursEstimateConcept) {
				if(!string.IsNullOrEmpty(jobLog.Description)) {
					jobLog.Description+="\r\n";
				}
				jobLog.Description+="Concept Estimate Changed From "+jobOld.HoursEstimateConcept+" hours To "+jobNew.HoursEstimateConcept+" hours.";
			}
			if(jobOld.HoursEstimateWriteup!=jobNew.HoursEstimateWriteup) {
				if(!string.IsNullOrEmpty(jobLog.Description)) {
					jobLog.Description+="\r\n";
				}
				jobLog.Description+="Writeup Estimate Changed From "+jobOld.HoursEstimateWriteup+" hours To "+jobNew.HoursEstimateWriteup+" hours.";
			}
			if(jobOld.HoursEstimateDevelopment!=jobNew.HoursEstimateDevelopment) {
				if(!string.IsNullOrEmpty(jobLog.Description)) {
					jobLog.Description+="\r\n";
				}
				jobLog.Description+="Development Estimate Changed From "+jobOld.HoursEstimateDevelopment+" hours To "+jobNew.HoursEstimateDevelopment+" hours.";
			}
			if(jobOld.HoursEstimateReview!=jobNew.HoursEstimateReview) {
				if(!string.IsNullOrEmpty(jobLog.Description)) {
					jobLog.Description+="\r\n";
				}
				jobLog.Description+="Review Estimate Changed From "+jobOld.HoursEstimateReview+" hours To "+jobNew.HoursEstimateReview+" hours.";
			}
			if(!String.IsNullOrEmpty(note)) {
				jobLog.Description+="\r\nReason: "+note;
			}
			if(string.IsNullOrEmpty(jobLog.Description)) {
				return false;
			}
			JobLogs.Insert(jobLog);
			return true;
		}

		public static JobLog MakeLogEntryForTitleChange(Job job,string oldTitle,string newTitle) {
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=newTitle,
				Description="Job Title Changed From\r\n"+oldTitle+"\r\nTo\r\n"+newTitle+".",
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}		
		
		public static JobLog MakeLogEntryForActive(Job job,long userNum,bool isActive) {
			string note=$@"Job was set to {(isActive?"active":"inactive")}";
			JobLog jobLog = new JobLog() {
				JobNum=job.JobNum,
				UserNumChanged=Security.CurUser.UserNum,
				UserNumExpert=job.UserNumExpert,
				UserNumEngineer=job.UserNumEngineer,
				Title=job.Title,
				Description=note,
				TimeEstimate=TimeSpan.FromHours(job.HoursEstimate)
			};
			JobLogs.Insert(jobLog);
			return JobLogs.GetOne(jobLog.JobLogNum);//to get new timestamp.
		}

		public static void DeleteForJob(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			string command = "DELETE FROM joblog WHERE JobNum="+POut.Long(jobNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static List<JobLog> GetJobLogsForJobNum(long jobNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLog>>(MethodBase.GetCurrentMethod(),jobNum);
			}
			if(jobNum==0) {
				return new List<JobLog>();
			}
			string command = "SELECT * FROM joblog WHERE JobNum="+POut.Long(jobNum)
					+" ORDER BY DateTimeEntry";
			return Crud.JobLogCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<JobLog> GetJobLogsForJobs(List<long> listJobNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<JobLog>>(MethodBase.GetCurrentMethod(),listJobNums);
			}
			if(listJobNums==null || listJobNums.Count==0) {
				return new List<JobLog>();
			}
			string command = "SELECT * FROM joblog WHERE JobNum IN ("+string.Join(",",listJobNums)+") "
					+"ORDER BY DateTimeEntry";
			return Crud.JobLogCrud.SelectMany(command);
		}

		public static bool HasViewLogForToday(long jobNum,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),jobNum,userNum);
			}
			
			string command = "SELECT COUNT(*) FROM joblog WHERE JobNum="+POut.Long(jobNum)
				+" AND UserNumChanged="+POut.Long(userNum)
				+" AND DateTimeEntry>="+POut.DateT(DateTime.Today);
			return Db.GetCount(command)=="1";//1 means there is a ViewLog for today that has already been created for this JobNum/UserNum combo.
		}

		///<summary>Gets one JobLog from the db.</summary>
		public static JobLog GetOne(long jobLogNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<JobLog>(MethodBase.GetCurrentMethod(),jobLogNum);
			}
			return Crud.JobLogCrud.SelectOne(jobLogNum);
		}

		///<summary></summary>
		public static long Insert(JobLog jobLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				jobLog.JobLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),jobLog);
				return jobLog.JobLogNum;
			}
			return Crud.JobLogCrud.Insert(jobLog);
		}

	}
}