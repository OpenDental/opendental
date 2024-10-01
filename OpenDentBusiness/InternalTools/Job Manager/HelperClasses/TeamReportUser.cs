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
			List<Job> listJobs=Jobs.GetListForTeamReport(listUserods,dateTimeFrom,dateTimeTo);
			List<JobReview> listJobReviewsInDateRange=GetJobReviewsInDateRangeFromJobs(listJobs,dateTimeFrom,dateTimeTo);
			List<Def> listPriorityDefs=Defs.GetDefsForCategory(DefCat.JobPriorities);
			List<JobLink> listTeamJobLinks=JobLinks.GetListForJobsByType(listJobs,JobLinkType.JobTeam);
			List<TeamReportUser> listTeamReportUsers=new List<TeamReportUser>();
			foreach(Userod userod in listUserods) {
				List<JobReview> listJobReviewsForUserInDateRange=FilterJobReviewsForUser(userod.UserNum,listJobs,listJobReviewsInDateRange);
				List<Job> listJobsForJobReportUser=FilterJobsForJobReportUser(userod.UserNum,listJobs,listJobReviewsForUserInDateRange);
				List<TeamReportJob> listTeamReportJobs=TeamReportJob.CreateListForUser(
					userod.UserNum,
					listJobsForJobReportUser,
					listTeamJobLinks,
					listJobTeamsAll,
					listPriorityDefs,
					listJobReviewsForUserInDateRange
				);
				listTeamReportUsers.Add(new TeamReportUser(userod.UserName,listTeamReportJobs));
			}
			return listTeamReportUsers;
		}

		private static List<JobReview> GetJobReviewsInDateRangeFromJobs(List<Job> listJobs,DateTime dateTimeFrom,DateTime dateTimeTo) {
			if(listJobs == null) { 
				return new List<JobReview>();
			}
			List<JobReview> listReviewsAndTimeLogs=new List<JobReview>();
			listReviewsAndTimeLogs.AddRange(
				listJobs
				.SelectMany(x => x.ListJobReviews)
				.Where(x => x.DateTStamp.Between(dateTimeFrom,dateTimeTo))
				.ToList()
			);
			listReviewsAndTimeLogs.AddRange(
				listJobs
				.SelectMany(x => x.ListJobTimeLogs)
				.Where(x => x.DateTStamp.Between(dateTimeFrom,dateTimeTo))
				.ToList()
			);
			return listReviewsAndTimeLogs;
		}

		private static List<JobReview> FilterJobReviewsForUser(long userNum,List<Job> listJobs,List<JobReview> listJobReviews) {
			if(listJobs == null || listJobReviews == null) { 
				return new List<JobReview>();
			}
			List<long> listJobNumsWithUserAsEngineer=listJobs
			.Where(x => x.UserNumEngineer==userNum)
			.Select(x => x.JobNum)
			.ToList();
			//Time logs made by user or reviews made for job's on which user is engineer.
			return listJobReviews.FindAll(x => 
				x.ReviewerNum==userNum
				|| (listJobNumsWithUserAsEngineer.Contains(x.JobNum) && x.ReviewStatus!=JobReviewStatus.TimeLog)
			);
		}

		///<summary>Gets all jobs with time logged by user in date range and all jobs without time logged by user in the date range 
		/// that user owns. Here, ownership is when the user is the submitter on a job with no expert or engineer,
		/// the expert on a job with no engineer, or the engineer.</summary>
		private static List<Job> FilterJobsForJobReportUser(long userNum,List<Job> listJobs,List<JobReview> listUserJobReviewsInDateRange) {
			if(listJobs == null || listUserJobReviewsInDateRange == null) { 
				return new List<Job>();
			}
			List<long> listJobNumsForUserReviewsInDateRange=listUserJobReviewsInDateRange.Select(x => x.JobNum).ToList();
			return  listJobs.FindAll(x => 
				//Job has review related to user in date range
				listJobNumsForUserReviewsInDateRange.Contains(x.JobNum) ||
				(//OR user is owner of job
					(userNum==x.UserNumConcept && x.UserNumExpert==0 && x.UserNumEngineer==0)
					|| (userNum==x.UserNumExpert && x.UserNumEngineer==0)
					|| userNum==x.UserNumEngineer
				)
			);

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
