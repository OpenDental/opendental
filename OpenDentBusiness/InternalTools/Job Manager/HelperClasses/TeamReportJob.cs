using CodeBase;
using OpenDental.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenDentBusiness.InternalTools.Job_Manager.HelperClasses {
		public class TeamReportJob {
			#region Fields/Properties/Enums
			private static readonly string _DAYS_NO_LOG="Days No Log";
			private Job _job; 
			private string _jobTeamName;
			private Def _priority;
			private double _hoursLoggedInDateRange;
			private TeamReportLogType _logType;
			private DateTime _dateTimeLastLog;
			private bool _hasReviewMarkedDone;
			private string _statusLastReview;
			private bool _isUserOwner;

			private string _jobDescriptionWithStatus {
				get {
					return _job.Category.ToString()[0]+_job.JobNum.ToString()+" ("+GetStatus()+") - "+_job.Title;
				}
			}
			///<summary>The priority's value as an integer or -1 if it contains no digits.</summary>
			private int _priorityIntValue {
				get {
					if(_priority==null) {
						return -1;
					}
					//Replace all non-digit characters with blank string.
					string priorityValue=Regex.Replace(_priority.ItemName,@"\D","");
					return (int.TryParse(priorityValue,out int result)) ? result : -1;
				}
			}

			private enum TeamReportLogType {
				None,
				TimeLog,
				Review
			}
			#endregion Fields/Properties/Enums

			private TeamReportJob(double hoursLoggedInDateRange,TeamReportLogType logType) {
				_hoursLoggedInDateRange=hoursLoggedInDateRange;
				_logType=logType;
			}

			#region Create List For User
			///<summary>Receives data for multiple jobs relevant to a user, and creates one or two TeamReportJobs per job.
			///A job can have one created for the user's reviews logged in date range and one for their
			///time logged in date range. If both are zero then one will be created to indicate that the user owns the job
			///and it's active (not in complete or documentation phases and not on hold).</summary>
			public static List<TeamReportJob> CreateListForUser(
				long userNum,
				List<Job> listJobsForUser,
				List<JobLink> listTeamJobLinks,
				List<JobTeam> listJobTeams,
				List<Def> listDefsJobPriority,
				List<JobReview> listJobReviewsByUserInDateRange)
			{
				List<TeamReportJob> listTeamReportJobs=new List<TeamReportJob>();
				foreach(Job job in listJobsForUser) {
					List<TeamReportJob> listTeamReportJobsForJob=CreateTeamReportJobsForJob(userNum,job.JobNum,listJobReviewsByUserInDateRange);
					SetCommonFieldsForList(
						listTeamReportJobsForJob,
						job,
						listTeamJobLinks,
						listJobTeams,
						listDefsJobPriority,
						userNum
					);
					listTeamReportJobs.AddRange(listTeamReportJobsForJob);
				}
				return GetSortedList(listTeamReportJobs);
			}

			///<summary>Sums all of the user's JobReview time for a job, separated into TimeLog and Review buckets.
			///If the sum for both is zero, a TeamReportJob is made representing the user's ownership of an active job.
			///Otherwise, a TeamReportJob is made for each bucket with time greater than zero to represent
			///the Review and TimeLog time logged by the user within the date range.</summary>
			private static List<TeamReportJob> CreateTeamReportJobsForJob(long userNum,long jobNum,List<JobReview> listJobReviewsByUserInDateRange) {
				TimeSpan timeSpanTimeLogs=TimeSpan.Zero;
				TimeSpan timeSpanReviewLogs=TimeSpan.Zero;
				foreach(JobReview jobReview in listJobReviewsByUserInDateRange) {
					if(jobReview.JobNum!=jobNum) {
						continue;
					}
					//User logged time on this job.
					if(jobReview.ReviewStatus==JobReviewStatus.TimeLog) {
						timeSpanTimeLogs+=jobReview.TimeReview;
					}
					//User is engineer on job and another user reviewed it.
					else if(jobReview.ReviewerNum!=userNum) {
						if(!jobReview.IsAsyncReview) {
							timeSpanTimeLogs+=jobReview.TimeReview;
						}
					}
					else {//All other logs are for reviews by user.
						timeSpanReviewLogs+=jobReview.TimeReview;
					}
				}
				List<TeamReportJob> listTeamReportJobsForJob=new List<TeamReportJob>();
				if(timeSpanTimeLogs==TimeSpan.Zero && timeSpanReviewLogs==TimeSpan.Zero) {
					listTeamReportJobsForJob.Add(new TeamReportJob(0,TeamReportLogType.None));
				}
				if(timeSpanTimeLogs > TimeSpan.Zero) {
					listTeamReportJobsForJob.Add(new TeamReportJob(timeSpanTimeLogs.TotalHours,TeamReportLogType.TimeLog));
				}
				if(timeSpanReviewLogs > TimeSpan.Zero) {
					listTeamReportJobsForJob.Add(new TeamReportJob(timeSpanReviewLogs.TotalHours,TeamReportLogType.Review));	
				}
				return listTeamReportJobsForJob;
			}

			///<summary>Searches the job's ListJobReviews and ListJobTimeLogs for the most recent one with time added.
			///and returns its DateTSTamp. Defaults to MinDate if those lists are empty.</summary>
			private static DateTime GetDateTimeLastLogOrMinDate(Job job) {
				DateTime dateTimeLastReview=job.ListJobReviews
					.FindAll(x => x.TimeReview > TimeSpan.Zero)
					.Select(x => x.DateTStamp)
					.DefaultIfEmpty(DateTime.MinValue)
					.Max();
				DateTime dateTimeLastTimeLog=job.ListJobTimeLogs
					.FindAll(x => x.TimeReview > TimeSpan.Zero)
					.Select(x => x.DateTStamp)
					.DefaultIfEmpty(DateTime.MinValue)
					.Max();
				return (dateTimeLastReview > dateTimeLastTimeLog) ? dateTimeLastReview : dateTimeLastTimeLog;
			}

			///<summary>Returns true if the user is the submitter and no expert or engineer are assigned,
			///the user is the expert and no engineer is assigned, or the user is the engineer.</summary>
			private static bool IsUserJobOwner(Job job,long userNum) {
				return (
					(userNum==job.UserNumConcept && job.UserNumExpert==0 && job.UserNumEngineer==0)
					|| (userNum==job.UserNumExpert && job.UserNumEngineer==0)
					|| userNum==job.UserNumEngineer
				);
			}

			private static void SetCommonFieldsForList(List<TeamReportJob> listTeamReportJobs,Job job,List<JobLink> listTeamJobLinks,List<JobTeam> listJobTeams,List<Def> listDefsJobPriority,long userNum) {
				long jobTeamNum=listTeamJobLinks.FirstOrDefault(x => x.JobNum==job.JobNum)?.FKey ?? 0;
				string jobTeamName=listJobTeams.FirstOrDefault(x => x.JobTeamNum==jobTeamNum)?.TeamName ?? "";
				Def priority=listDefsJobPriority.FirstOrDefault(x => x.DefNum==job.Priority) ?? new Def();
				bool hasReviewMarkedDone=job.ListJobReviews.Any(x => x.ReviewStatus==JobReviewStatus.Done);
				string statusLastReview=job.ListJobReviews
					.OrderByDescending(x => x.DateTStamp)
					.ThenByDescending(x => x.JobReviewNum)
					.FirstOrDefault()?.ReviewStatus.ToString() ?? "";
				DateTime dateTimeLastLog=GetDateTimeLastLogOrMinDate(job);
				bool isUserOwner=IsUserJobOwner(job,userNum);
				foreach(TeamReportJob teamReportJob in listTeamReportJobs) {
					//Set members common to all TeamReportJobs for job.
					teamReportJob._job=job;
					teamReportJob._jobTeamName=jobTeamName;
					teamReportJob._dateTimeLastLog=dateTimeLastLog;
					teamReportJob._priority=priority;
					teamReportJob._hasReviewMarkedDone=hasReviewMarkedDone;
					teamReportJob._statusLastReview=statusLastReview;
					teamReportJob._isUserOwner=isUserOwner;
				}
			}

			private static List<TeamReportJob> GetSortedList(List<TeamReportJob> listTeamReportJobs) {
				return listTeamReportJobs
					//Jobs with zero hours to bottom
					.OrderBy(x => x._hoursLoggedInDateRange==0)
					//Then review logs above those
					.ThenBy(x => x._logType==TeamReportLogType.Review)
					//Then by hours logged descending above those
					.ThenByDescending(x => x._hoursLoggedInDateRange)
					//then send jobs with pending priority to the bottom of each group
					.ThenBy(x => x._priority.ItemOrder==0)
					//then remaining jobs are sorted by other priorities
					.ThenBy(x => x._priority.ItemOrder)
					//JobNum breaks ties
					.ThenBy(x => x._job.JobNum)
					.ToList();
			}
			#endregion Create List For User

			public static string CreateSummaryForList(List<TeamReportJob> listTeamReportJobs) {
				List<string> listSummaryLines=new List<string>();
				foreach(TeamReportJob teamReportJob in listTeamReportJobs) {
					//Skip jobs with no hours logged in date range.
					if(teamReportJob._hoursLoggedInDateRange==0) {
						continue;
					}
					listSummaryLines.Add(teamReportJob._hoursLoggedInDateRange.ToString("F2")
						+" Hours | "+teamReportJob._jobDescriptionWithStatus);
				}
				if(listSummaryLines.Count==0) {
					listSummaryLines.Add("No hours logged");
				}
				return string.Join("\n",listSummaryLines);
			}

			public static List<GridRow> CreateGridRowsForList(List<TeamReportJob> listTeamReportJobs,bool doIncludeNoLogsThirtyDays,DateTime dateTimeFrom) {
				List<GridRow> listGridRows=new List<GridRow>();
				listGridRows.AddRange(CreateGridRowsForType(listTeamReportJobs,doIncludeNoLogsThirtyDays,dateTimeFrom,TeamReportLogType.TimeLog));
				listGridRows.AddRange(CreateGridRowsForType(listTeamReportJobs,doIncludeNoLogsThirtyDays,dateTimeFrom,TeamReportLogType.Review));
				listGridRows.AddRange(CreateGridRowsForType(listTeamReportJobs,doIncludeNoLogsThirtyDays,dateTimeFrom,TeamReportLogType.None));
				return listGridRows;
			}

			private static List<GridRow> CreateGridRowsForType(List<TeamReportJob> listTeamReportJobs,bool doIncludeNoLogsThirtyDays,DateTime dateTimeFrom,TeamReportLogType logType) {
				List<TeamReportJob>listTeamReportJobsForType=listTeamReportJobs?.FindAll(x => x._logType == logType) ?? new List<TeamReportJob>();
				List<GridRow> listGridRows=new List<GridRow>();
				double typeTotal=0;
				foreach(TeamReportJob teamReportJob in listTeamReportJobsForType) {
					string discussionPrompt=teamReportJob.GetDiscussionPrompt(dateTimeFrom);
					//The job doesn't have hours logged in date range, and we have no discussion prompts for it.
					if (teamReportJob._hoursLoggedInDateRange==0 && string.IsNullOrWhiteSpace(discussionPrompt)) {
						continue;
					}
					if(!doIncludeNoLogsThirtyDays && discussionPrompt.Contains(_DAYS_NO_LOG)) {
						continue;
					}
					typeTotal+=teamReportJob._hoursLoggedInDateRange;
					GridRow row=new GridRow();
					row.Cells.Add(teamReportJob._job.ToString());
					row.Cells.Add(teamReportJob._jobTeamName);
					row.Cells.Add(teamReportJob.GetStatus());
					row.Cells.Add(teamReportJob._priority.ItemName);
					row.Cells.Add(teamReportJob._hoursLoggedInDateRange.ToString("F2"));
					row.Cells.Add(discussionPrompt);
					row.Tag=teamReportJob._job;
					listGridRows.Add(row);
				}
				if(logType.In(TeamReportLogType.TimeLog,TeamReportLogType.Review)) {
					listGridRows.Add(CreateSummaryRow(logType,typeTotal));
				}
				return listGridRows;
			}

			private static GridRow CreateSummaryRow(TeamReportLogType logType, double hours) {
				string label="Total ";
				switch (logType) {
					case TeamReportLogType.TimeLog:
					label+="TimeLogs";
					break;
					case TeamReportLogType.Review:
					label+="ReviewLogs";
					break;
					default:
					label+="Unknown";
					break;
				}
				GridRow row=new GridRow();
				row.ColorBackG=Color.FromArgb(240,240,240);
				row.Bold=true;
				row.Cells.Add(label);
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add(hours.ToString("F2"));
				row.Cells.Add("");
				row.Tag=null;
				return row;
			}

			///<summary>Status is the job's current phase unless it is for review time or a research job in definition phase.
			///A note indicating approval status or review status and whether or not user is owner can be appended.</summary>
			private string GetStatus() {
				string status;
				if(_logType==TeamReportLogType.Review) {
					status="Reviewer";
				}
				else if(_job.Category==JobCategory.Research && _job.PhaseCur==JobPhase.Definition) {
					status="Research";
				}
				else {
					status=_job.PhaseCur.ToString();
				}
				string statusNote="";
				if(!_isUserOwner && _logType!=TeamReportLogType.Review) {
					statusNote=" / not owner";
				}
				else if(_isUserOwner && _job.IsApprovalNeeded) {
					statusNote=" / awaiting approval";
				}
				else if(!_job.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete,JobPhase.Documentation) && _logType!=TeamReportLogType.Review) {
					if(_hasReviewMarkedDone) {
						statusNote=" / done";
					}
					else if(!_statusLastReview.IsNullOrEmpty()) {
						statusNote=$" / {_statusLastReview}";
					}
				}
				return status+statusNote;
			}

			private string GetDiscussionPrompt(DateTime dateTimeFrom) {
				List<string> listPrompts=new List<string>();
				if(_logType==TeamReportLogType.Review 
					|| _job.PhaseCur==JobPhase.Complete 
					|| _job.PhaseCur==JobPhase.Documentation 
					|| _job.PhaseCur==JobPhase.Cancelled) 
				{
					return "";
				}
				//Use today's date if from date is in the future.
				DateTime baseDate = (DateTime.Today < dateTimeFrom) ? DateTime.Today : dateTimeFrom; 
				DateTime oneMonthBeforeBaseDate=baseDate.AddDays(-30).Date;
				if(_job.DateTimeEntry.Date <= oneMonthBeforeBaseDate && _dateTimeLastLog <= oneMonthBeforeBaseDate) {
					DateTime mostRecentDate=(_job.DateTimeEntry >= _dateTimeLastLog) ? _job.DateTimeEntry : _dateTimeLastLog;
					TimeSpan difference=baseDate - mostRecentDate;
					int daysSinceLastLog=difference.Days;
					listPrompts.Add($"{daysSinceLastLog} {_DAYS_NO_LOG}");
				}
				//Excludes pending priority
				else if(_priorityIntValue >= 75 && _hoursLoggedInDateRange==0) {
					listPrompts.Add("High Priority No Log");
				}
				else if(_hoursLoggedInDateRange >= 15) {
					listPrompts.Add("Many Hours Logged");
				}
				if(_job.HoursActual > _job.HoursEstimate && _job.Category!=JobCategory.Bug) {
					listPrompts.Add("Over Estimate");
				}
				return String.Join(" / ",listPrompts);
			}
		}
}
