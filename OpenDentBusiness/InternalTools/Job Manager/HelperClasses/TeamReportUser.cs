using CodeBase;
using OpenDental.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.InternalTools.Job_Manager.HelperClasses {
	public class TeamReportUser {
		#region Fields/Properties
		public string UserName { get; private set; }
		private List<TeamReportJob> _listTeamReportJobs=new List<TeamReportJob>();
		#endregion Fields/Properties

		#region Create List For Job Team
		private TeamReportUser(string userName,List<TeamReportJob> listTeamReportJobs) {
			UserName=userName;
			_listTeamReportJobs=listTeamReportJobs;
		}

		///<summary>Fetches all needed data for passed in teams and date range,
		///and creates a TeamReportUser for each team member. Returns them in a list.</summary>
		public static List<TeamReportUser> CreateListForTeam(List<JobTeam> listJobTeamsToReport,DateTime dateTimeFrom,DateTime dateTimeTo,List<JobTeam> listJobTeamsAll) {
			List<Userod> listUserods=Userods.GetListByJobTeams(listJobTeamsToReport);
			List<Job> listJobs=GetCachedJobsForTeam(listUserods);
			List<JobReview> listJobReviews=JobReviews.GetListForTeamReport(listJobs,listUserods,dateTimeFrom,dateTimeTo);
			listJobs.AddRange(GetUncachedJobsForTeam(listJobs,listJobReviews));
			FillLogListsForJobs(listJobs,listJobReviews);
			List<JobReview> listJobReviewsInDateRange=listJobReviews.FindAll(x => x.DateTStamp.Between(dateTimeFrom,dateTimeTo) && x.TimeReview > TimeSpan.Zero);
			List<Def> listPriorityDefs=Defs.GetDefsForCategory(DefCat.JobPriorities);
			List<JobLink> listTeamJobLinks=JobLinks.GetListForJobsByType(listJobs,JobLinkType.JobTeam);
			return CreateTeamReportUsersFromLists(listUserods,listJobReviewsInDateRange,listJobs,listTeamJobLinks,listJobTeamsAll,listPriorityDefs);
		}

		private static List<Job> GetCachedJobsForTeam(List<Userod> listUserods) {
			long onHoldDefNum=Defs.GetDefsForCategory(DefCat.JobPriorities).First(x => x.ItemValue=="OnHold").DefNum;
			List<long> listUserNums=listUserods.Select(x => x.UserNum).ToList();
			return JobManagerCore.ListJobsAll.FindAll(x => (
					//User is job owner
					listUserNums.Contains(x.UserNumEngineer)
					|| (listUserNums.Contains(x.UserNumExpert) && x.UserNumEngineer == 0)
					|| (listUserNums.Contains(x.UserNumConcept) && x.UserNumEngineer == 0 && x.UserNumExpert == 0)
				)
				// Job is not project, not on hold, and in an active phase
				&&	x.Priority != onHoldDefNum
				&&	x.Category != JobCategory.Project
				&&	x.PhaseCur.In(JobPhase.Concept,JobPhase.Definition,JobPhase.Development,JobPhase.Quote)
			).ToList();
		}

		private static List<Job> GetUncachedJobsForTeam(List<Job> listCachedJobs,List<JobReview> listJobReviews) {
			List<long> listCachedJobNums=listCachedJobs.Select(x => x.JobNum).ToList();
			List<long> listUncachedJobNums=listJobReviews
				.FindAll(x => !listCachedJobNums.Contains(x.JobNum))
				.Select(x => x.JobNum)
				.ToList();
			return Jobs.GetMany(listUncachedJobNums);
		}
		
		private static List<JobReview> FillLogListsForJobs(List<Job> listJobs,List<JobReview> listJobReviews) {
			foreach(Job job in listJobs) {
				job.ListJobTimeLogs=listJobReviews.FindAll(x=>x.JobNum==job.JobNum && x.ReviewStatus==JobReviewStatus.TimeLog);
				job.ListJobReviews=listJobReviews.FindAll(x => x.JobNum==job.JobNum && x.ReviewStatus!=JobReviewStatus.TimeLog);
			}
			return listJobReviews;
		}

		private static List<TeamReportUser> CreateTeamReportUsersFromLists(
			List<Userod> listUserods,
			List<JobReview> listJobReviewsInDateRange,
			List<Job> listJobs,
			List<JobLink> listTeamJobLinks,
			List<JobTeam> listJobTeams,
			List<Def> listJobPriorityDefs)
		{
			List<TeamReportUser> listTeamReportUsers=new List<TeamReportUser>();
			foreach(Userod userod in listUserods) {
				List<JobReview> listJobReviewsByUserInDateRange=listJobReviewsInDateRange.FindAll(x=> x.ReviewerNum==userod.UserNum);
				List<Job> listJobsForJobReportUser=FilterJobsForJobReportUser(userod.UserNum,listJobs,listJobReviewsByUserInDateRange);
				List<TeamReportJob> listTeamReportJobs=TeamReportJob.CreateListForUser(
					userod.UserNum,
					listJobsForJobReportUser,
					listTeamJobLinks,
					listJobTeams,
					listJobPriorityDefs,
					listJobReviewsByUserInDateRange
				);
				listTeamReportUsers.Add(new TeamReportUser(userod.UserName,listTeamReportJobs));
			}
			return listTeamReportUsers;
		}

		///<summary>Gets all jobs with time logged by user in date range and all jobs without time logged by user in the date range 
		/// that user owns. Here, ownership is when the user is the submitter on a job with no expert or engineer,
		/// the expert on a job with no engineer, or the engineer.</summary>
		private static List<Job> FilterJobsForJobReportUser(long userNum,List<Job>listJobs,List<JobReview> listUserJobReviewsInDateRange) {
			List<long> listJobNumsForUserReviewsInDateRange=listUserJobReviewsInDateRange.Select(x => x.JobNum).ToList();
			List<Job> listJobsWithUserReviewsInDateRange=listJobs.FindAll(x => listJobNumsForUserReviewsInDateRange.Contains(x.JobNum)).ToList();
			List<Job> listJobsOwnedByUserNoReviewsInDateRange=listJobs.FindAll(x => 
				!listJobNumsForUserReviewsInDateRange.Contains(x.JobNum)
				&& (//user is owner of job
					(userNum==x.UserNumConcept && x.UserNumExpert==0 && x.UserNumEngineer==0)
					|| (userNum==x.UserNumExpert && x.UserNumEngineer==0)
					|| userNum==x.UserNumEngineer
				)
			);
			List<Job> listJobsForJobReportUser=listJobsWithUserReviewsInDateRange.Concat(listJobsOwnedByUserNoReviewsInDateRange).ToList();
			return listJobsForJobReportUser;
		}
		#endregion Create List For Job Team

		#region GridRow Creation
		public List<GridRow> CreateGridRows(bool doIncludeNoLogsThirtyDays,DateTime dateTimeFrom) {
			List<GridRow> listGridRows=new List<GridRow>(){ CreateHeaderRow() };
			listGridRows.AddRange(TeamReportJob.CreateGridRowsForList(_listTeamReportJobs,doIncludeNoLogsThirtyDays,dateTimeFrom));
			return listGridRows;
		}

		private GridRow CreateHeaderRow() {
			GridRow row=new GridRow();
			row.ColorBackG=Color.FromArgb(223,234,245);
			row.Bold=true;
			row.Cells.Add(UserName);
			return row;
		}
		#endregion GridRow Creation

		public string GenerateSummary() {
			return TeamReportJob.CreateSummaryForList(_listTeamReportJobs);
		}
	}
}
