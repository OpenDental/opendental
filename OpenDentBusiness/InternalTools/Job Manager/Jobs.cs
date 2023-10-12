using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Jobs {
		///<summary>Defines delegate signature to be used for Jobs.NavTaskDelegate.</summary>
		public delegate void NavToJobDelegate(long JobNum);
		///<summary>Sent in from FormOpenDental. Allows static method for business layer to cause Job navigation in FormOpenDental.</summary>
		public static NavToJobDelegate NavJobDelegate;
		///<summary>Gets one Job from the db.</summary>
		public static Job GetOne(long jobNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Job>(MethodBase.GetCurrentMethod(),jobNum);
			}
			return Crud.JobCrud.SelectOne(jobNum);
		}

		///<summary>Gets one Job from the db. Fills all respective object lists from the DB too.</summary>
		public static Job GetOneFilled(long jobNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Job>(MethodBase.GetCurrentMethod(),jobNum);
			}
			Job job=Crud.JobCrud.SelectOne(jobNum);
			if(job!=null) {
				FillInMemoryLists(new List<Job>() { job });
			}
			return job;
		}

		///<summary>Gets the first Job that matches the search criteria passed in.  Fills all respective object lists from the DB too.</summary>
		public static Job GetOneFilled(string search) {
			if(search==null) {
				return null;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Job>(MethodBase.GetCurrentMethod(),search);
			}
			string[] arraySearch=search.Split(' ');
			string searchScrubbed=string.Join("%",arraySearch.Select(x => POut.String(x)));
			string command=$@"SELECT * FROM job
				WHERE Title LIKE '%{searchScrubbed}%'
				OR Requirements LIKE '%{searchScrubbed}%'
				OR RequirementsJSON LIKE '%{searchScrubbed}%'
				OR Implementation LIKE '%{searchScrubbed}%'
				LIMIT 1";
			Job job=Crud.JobCrud.SelectOne(command);
			if(job!=null) {
				FillInMemoryLists(new List<Job>() { job });
			}
			return job;
		}

		///<summary>Gets the top level parent for the given Job using the ParentNum instead of the TopParentNum.</summary>
		public static Job GetTopParentByParent(Job job,List<Job> listJobs) {
			while(job!=null && job.ParentNum!=0) {
				job=listJobs.FirstOrDefault(x => x.JobNum==job.ParentNum);
			}
			return job;
		}

		///<summary>Inserts job into database. Sets the TopParentNum (and updates its TopParentList) if necessary. </summary>
		public static long Insert(Job job) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				job.JobNum=Meth.GetLong(MethodBase.GetCurrentMethod(),job);
				return job.JobNum;
			}
			long jobNum=Crud.JobCrud.Insert(job);
			if(job.TopParentNum==0) { 
				job.TopParentNum=jobNum;
				UpdateTopParentList(job.TopParentNum,new List<long>{jobNum});
			}
			return jobNum;
		}

		///<summary></summary>
		public static void Update(Job job) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),job);
				return;
			}
			Crud.JobCrud.Update(job);
		}

		///<summary>Updates one Job in the database.  Uses an old object to compare to, and only alters changed fields.
		///This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Job jobCur,Job jobOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),jobCur,jobOld);
			}
			return Crud.JobCrud.Update(jobCur,jobOld);
		}

		///<summary>Updates all passed in jobs with the topParentNumNew.</summary>
		public static void UpdateTopParentList(long topParentNumNew,List<long> listJobNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),topParentNumNew,listJobNums);
				return;
			}
			string command=$"UPDATE job SET TopParentNum={ POut.Long(topParentNumNew)} WHERE JobNum IN ({string.Join(",",listJobNums.Select(x => POut.Long(x)))})";
			Db.NonQ(command);
		}

		///<summary>You must surround with a try-catch when calling this method.  Deletes one job from the database.  
		///Also deletes all JobLinks, Job Events, and Job Notes associated with the job.  Jobs that have reviews or quotes on them may not be deleted and will throw an exception.</summary>
		public static void Delete(long jobNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),jobNum);
				return;
			}
			if(JobReviews.GetReviewsForJobs(jobNum).Count>0 || JobQuotes.GetForJob(jobNum).Count>0) {
				throw new Exception(Lans.g("Jobs","Not allowed to delete a job that has attached reviews or quotes.  Set the status to deleted instead."));//The exception is caught in FormJobEdit.
			}
			//JobReviews.DeleteForJob(jobNum);//do not delete, blocked above
			//JobQuotes.DeleteForJob(jobNum);//do not delete, blocked above
			JobLinks.DeleteForJob(jobNum);
			JobLogs.DeleteForJob(jobNum);
			JobNotes.DeleteForJob(jobNum);
			Crud.JobCrud.Delete(jobNum); //Finally, delete the job itself.
		}

		///<summary>Get completed job num for the date range based on the "job implemented" JobLog.</summary>
		public static List<long> GetCompletedJobNumsForRange(DateTime dateStart,DateTime dateEnd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateStart,dateEnd);
			}
			string command=$@"SELECT DISTINCT JobNum FROM joblog
				WHERE Description LIKE '%Job implemented%'
				AND DateTimeEntry BETWEEN {POut.Date(dateStart)} AND {POut.Date(dateEnd)}";
			return Db.GetListLong(command);
		}

		public static List<Job> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM job";
			return Crud.JobCrud.SelectMany(command);
		}

		public static List<JobShort> GetJobShortNoTopParent() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<JobShort>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT JobNum,ParentNum,TopParentNum FROM job WHERE TopParentNum=0";
			List<JobShort> listJobTree=new List<JobShort>();
			DataTable tableJobs=Db.GetTable(command);
			for(int i=0;i<tableJobs.Rows.Count;i++) {
				DataRow row=tableJobs.Rows[i];
				JobShort jobTreeStructure=new JobShort();
				jobTreeStructure.JobNum=row.GetLong("JobNum");
				jobTreeStructure.ParentNum=row.GetLong("ParentNum");
				jobTreeStructure.TopParentNum=row.GetLong("TopParentNum");
				listJobTree.Add(jobTreeStructure);
			}
			return listJobTree;
		}

		///<summary>Gets all the jobs necessary for loading the Job Manager window.  This method will NOT invoke FillInMemoryLists() due to middle tier.
		///Returns a list of jobs that are not cancelled and not complete (unless in Category "Feature" or "HqRequest" and have a UserNumCustContact=0).</summary>
		public static List<Job> GetAllForManager() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod());
			}
			string command=$@"SELECT * FROM job
				WHERE PhaseCur!={POut.Enum(JobPhase.Cancelled)}
				AND (
					PhaseCur!={POut.Enum(JobPhase.Complete)} 
					OR (Category IN ({POut.Enum(JobCategory.Feature)},{POut.Enum(JobCategory.HqRequest)}) AND UserNumCustContact=0)
				)";
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all cancelled/completed jobs with some specific UI filters applied for the Jobs Report panel in Job Manager Overview window.
		///Essentially fills in the blanks for the cached job list since it does not contain cancelled/completed jobs.</summary>
		public static List<Job> GetUncachedForReport(long lastXDays,DateTime dateCompletedFrom,DateTime dateCompletedTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),lastXDays,dateCompletedFrom,dateCompletedTo);
			}
			//Select all jobs that are cancelled or completed (if completed, completed between a date range) 
			//that had the job expert or assigned engineer log time within the last X days.
			string command=$@"SELECT job.* FROM job
				INNER JOIN jobreview ON job.JobNum=jobreview.JobNum
				WHERE (
					job.PhaseCur={POut.Enum(JobPhase.Cancelled)}
					OR (
						job.PhaseCur IN ({POut.Enum(JobPhase.Complete)},{POut.Enum(JobPhase.Documentation)})
						AND job.DateTimeImplemented BETWEEN {POut.Date(dateCompletedFrom)} AND {POut.Date(dateCompletedTo)}
					)
				)
				AND jobreview.ReviewerNum IN (job.UserNumExpert,job.UserNumEngineer) 
				AND jobreview.DateTStamp BETWEEN {POut.Date(DateTime.Now.AddDays(-lastXDays))} AND CURDATE()
				GROUP BY job.JobNum";
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all jobs for the date range and phases passed in.  An empty list of phases will be treated as "Any" phase.
		///Optionally pass in a list of JobNums to exclude from the result set.</summary>
		public static List<Job> GetForSearch(DateTime dateFrom,DateTime dateTo,List<JobPhase> listPhases,List<long> listPriorities,
			List<long> listExcludeJobNums=null)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listPhases,listPriorities,listExcludeJobNums);
			}
			string command=$@"SELECT * FROM job
				WHERE {DbHelper.BetweenDates("DateTimeEntry",dateFrom,dateTo)} ";
			if(!listPhases.IsNullOrEmpty()) {
				command+=$"AND PhaseCur IN ({string.Join(",",listPhases.Select(x => POut.Enum(x)))}) ";
			}
			if(!listExcludeJobNums.IsNullOrEmpty()) {
				command+=$"AND JobNum NOT IN ({string.Join(",",listExcludeJobNums.Select(x => POut.Long(x)))}) ";
			}
			if(!listPriorities.IsNullOrEmpty()) {
				command+=$"AND Priority IN ({string.Join(",",listPriorities.Select(x => POut.Long(x)))})";
			}
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all jobs that the testing department would care about in regards to the version string passed in.
		///This method will only return jobs that are in certain categories and are in the Complete or Documentaion phases.
		///Optionally pass in a list of JobNums to exclude from the result set.</summary>
		public static List<Job> GetForTesting(string version,List<long> listExcludeJobNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),version,listExcludeJobNums);
			}
			string command=$@"SELECT * FROM job
				WHERE PhaseCur IN ({POut.Enum(JobPhase.Complete)},{POut.Enum(JobPhase.Documentation)})
				AND Category NOT IN ({POut.Enum(JobCategory.Query)},{POut.Enum(JobCategory.Research)},{POut.Enum(JobCategory.Conversion)},{POut.Enum(JobCategory.MarketingDesign)}) 
				AND JobVersion LIKE '%{POut.String(version)}%' ";
			if(!listExcludeJobNums.IsNullOrEmpty()) {
				command+=$"AND JobNum NOT IN ({string.Join(",",listExcludeJobNums.Select(x => POut.Long(x)))})";
			}
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all jobs that the Query department would care about in regards to the date range passed in.
		///This method will only return jobs that are in certain categories and are in the Complete or Documentaion phases.
		///Optionally pass in a list of JobNums to exclude from the result set.</summary>
		public static List<Job> GetForQueries(List<long> listExcludeJobNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),listExcludeJobNums);
			}
			string command=$@"SELECT * FROM job
				WHERE Category={POut.Enum(JobCategory.Query)}
				AND PhaseCur IN ({POut.Enum(JobPhase.Complete)},{POut.Enum(JobPhase.Cancelled)}) ";
			if(!listExcludeJobNums.IsNullOrEmpty()) {
				command+=$"AND JobNum NOT IN ({string.Join(",",listExcludeJobNums.Select(x => POut.Long(x)))})";
			}
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all jobs that the Marketing department would care about in regards to the date range passed in.
		///This method will only return jobs that are in certain categories and are in the Complete or Documentaion phases.
		///Optionally pass in a list of JobNums to exclude from the result set.</summary>
		public static List<Job> GetForMarketing(List<long> listExcludeJobNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),listExcludeJobNums);
			}
			string command=$@"SELECT * FROM job
				WHERE Category={POut.Enum(JobCategory.MarketingDesign)}
				AND PhaseCur IN ({POut.Enum(JobPhase.Complete)},{POut.Enum(JobPhase.Cancelled)}) ";
			if(!listExcludeJobNums.IsNullOrEmpty()) {
				command+=$"AND JobNum NOT IN ({string.Join(",",listExcludeJobNums.Select(x => POut.Long(x)))})";
			}
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all jobs that the Unresolved Issues system needs.
		///This method will only return jobs that are in the Unresolved Issues category and are Cancelled.
		///Optionally pass in a list of JobNums to exclude from the result set.</summary>
		public static List<Job> GetForUnresolvedIssues(List<long> listExcludeJobNums=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),listExcludeJobNums);
			}
			string command=$@"SELECT * FROM job
				WHERE Category={POut.Enum(JobCategory.UnresolvedIssue)}
				AND PhaseCur = {POut.Enum(JobPhase.Cancelled)} ";
			if(!listExcludeJobNums.IsNullOrEmpty()) {
				command+=$"AND JobNum NOT IN ({string.Join(",",listExcludeJobNums.Select(x => POut.Long(x)))})";
			}
			return Crud.JobCrud.SelectMany(command);
		}

		public static List<Job> GetMany(List<long> listJobNums) {
			if(listJobNums.IsNullOrEmpty()) {
				return new List<Job>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),listJobNums);
			}
			string command="SELECT * FROM job WHERE JobNum IN ("+string.Join(",",listJobNums.Select(x => POut.Long(x)))+")";
			return Crud.JobCrud.SelectMany(command);
		}

		public static List<Job> GetReleaseCalculatorJobs(List<long> listPriorities,List<JobPhase> listPhases,List<JobCategory> listCategories) {
			if(listPriorities.IsNullOrEmpty() || listPhases.IsNullOrEmpty() || listCategories.IsNullOrEmpty()) {
				return new List<Job>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),listPriorities,listPhases,listCategories);
			}
			string command=$@"SELECT * FROM job
				WHERE job.Priority IN ({string.Join(",",listPriorities.Select(x => POut.Long(x)))})
				AND job.PhaseCur IN ({string.Join(",",listPhases.Select(x => POut.Enum(x)))})
				AND job.Category IN ({string.Join(",",listCategories.Select(x => POut.Enum(x)))})
				GROUP BY job.JobNum
				ORDER BY job.PhaseCur,job.Category,job.DateTimeEntry";
			return Crud.JobCrud.SelectMany(command);
		}

		public static bool ValidateJobNum(long jobNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),jobNum);
			}
			string command="SELECT COUNT(*) FROM job WHERE JobNum="+POut.Long(jobNum);
			return Db.GetScalar(command)!="0";
		}

		///<summary></summary>
		private static TreeNode GetNodeHierarchyAll(Job job,List<Job> listJobs) {
			TreeNode[] children=listJobs.FindAll(x => x.ParentNum==job.JobNum).Select(x => GetNodeHierarchyAll(x,listJobs)).ToArray();//can be enhanced by removing matches from the search set.
			TreeNode node=new TreeNode(job.ToString()) { Tag=job };
			if(children.Length>0) {
				node.Nodes.AddRange(children);
			}
			return node;
		}

		///<summary>Similar to GetNodeHeirarchy, but used to build tree to be passed to job control. Updates the JobManagerCore cache.</summary>
		public static TreeNode GetJobTreeTop(Job job,bool includeCompletedCanceled) {
			if(job==null) {
				return null;
			}
			List<Job> listJobsByTopParent=GetAllByTopParentNum(job.TopParentNum);
			if(!includeCompletedCanceled) {
				listJobsByTopParent=listJobsByTopParent.FindAll(x => !x.PhaseCur.In(JobPhase.Cancelled,JobPhase.Complete));
			}
			JobManagerCore.AddTreeJobsToList(listJobsByTopParent);
			List<Job> jobHierarchy=new List<Job>{job};
			for(int i=0;i<jobHierarchy.Count;i++) {
				Job jobNode=JobManagerCore.GetJob(jobHierarchy[i].ParentNum);
				if(jobNode==null) {
					break;
				}
				jobHierarchy.Add(jobNode);
			}
			return GetNodeHierarchyAll(jobHierarchy.Last(),listJobsByTopParent);
		}

		///<summary>Recursively travels through the given list of jobs to find all of the jobs that are direct descendants of the given job.</summary>
		public static List<Job> GetChildJobs(Job job,List<Job> listJobs) {
			List<Job> listChildJobs=listJobs.FindAll(x => x.ParentNum==job.JobNum);
			for(int i=0;i<listChildJobs.Count();i++) {
				listChildJobs.AddRange(GetChildJobs(listChildJobs[i],listJobs));
			}
			return listChildJobs;
		}

		///<summary>Recursively travels through the given list of jobTreeStructures to find all of the jobTreeStructures that are direct descendants of the given jobTreeStructures. This version uses JobShort instead of Job. </summary>
		public static List<JobShort> GetChildJobs(JobShort jobTreeStructure,List<JobShort> listJobShorts) {
			List<JobShort> listChildJobs=listJobShorts.FindAll(x => x.ParentNum==jobTreeStructure.JobNum);
			for(int i=0;i<listChildJobs.Count();i++) {
				listChildJobs.AddRange(GetChildJobs(listChildJobs[i],listJobShorts));
			}
			return listChildJobs;
		}

		///<summary></summary>
		public static List<Job> GetAllByTopParentNum(long topParentNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),topParentNum);
			}
			string command = "SELECT * FROM job WHERE TopParentNum="+POut.Long(topParentNum);
			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Returns all jobs that were reviewed by the user(Num) in the date range.</summary>
		public static List<Job> GetJobsWithReviewsByUser(long userNum,DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Job>>(MethodBase.GetCurrentMethod(),userNum,dateFrom,dateTo);
			}
			List<JobPhase> listPhases=new List<JobPhase>(){ JobPhase.Complete,JobPhase.Documentation };
			List<JobReviewStatus> listStatuses=new List<JobReviewStatus>(){ JobReviewStatus.NeedsAdditionalWork,JobReviewStatus.NeedsAdditionalReview,JobReviewStatus.Done };
			string command=$@"SELECT * FROM job
				INNER JOIN jobreview ON jobreview.JobNum=job.JobNum
				WHERE job.PhaseCur IN({string.Join(",",listPhases.Select(x => POut.Enum(x)))})
				AND jobreview.ReviewStatus IN('{string.Join("','",listStatuses.Select(x => x.ToString()))}')
				AND jobreview.ReviewerNum={POut.Long(userNum)}
				AND jobreview.DateTStamp BETWEEN {POut.Date(dateFrom)} AND {POut.Date(dateTo)}
				GROUP BY job.JobNum
				ORDER BY jobreview.DateTStamp DESC;";

			return Crud.JobCrud.SelectMany(command);
		}

		///<summary>Removes invalid jobs from the list of jobs passed in and then queries the DB to fill corresponding in-memory lists for remaining jobs.
		///Set isForSearch true in order to only fill in-memory lists that are required for the Job Search window.</summary>
		public static void FillInMemoryLists(List<Job> listJobsAll,bool isForSearch=false) {
			//No need for remoting call here.
			listJobsAll.RemoveAll(x => x==null || x.JobNum==0);
			List<long> jobNums=listJobsAll.Select(x => x.JobNum).ToList();
			Dictionary<long,List<JobLink>> dictJobLinksAll=new Dictionary<long,List<JobLink>>();
			Dictionary<long,List<JobNote>> dictJobNotesAll=new Dictionary<long,List<JobNote>>();
			Dictionary<long,List<JobReview>> dictJobReviewsAll=new Dictionary<long,List<JobReview>>();
			Dictionary<long,List<JobReview>> dictJobTimeLogsAll=new Dictionary<long,List<JobReview>>();
			Dictionary<long,List<JobQuote>> dictJobQuotesAll=new Dictionary<long,List<JobQuote>>();
			Dictionary<long,List<JobNotification>> dictJobNotificationsAll=new Dictionary<long,List<JobNotification>>();
			Dictionary<long,List<JobActiveLink>> dictJobActiveLinksAll=new Dictionary<long,List<JobActiveLink>>();
			//The job search only needs ListJobLinks and ListJobReviews to be filled.
			dictJobLinksAll=JobLinks.GetJobLinksForJobs(jobNums).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
			dictJobReviewsAll=JobReviews.GetReviewsForJobs(jobNums.ToArray()).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
			//Fill all other dictionaries when not filling in-memory lists for job search results (saves db calls and memory usage).
			if(!isForSearch) {
				dictJobNotesAll=JobNotes.GetJobNotesForJobs(jobNums).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
				dictJobTimeLogsAll=JobReviews.GetTimeLogsForJobs(jobNums.ToArray()).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
				dictJobQuotesAll=JobQuotes.GetJobQuotesForJobs(jobNums).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
				dictJobNotificationsAll=JobNotifications.GetNotificationsForJobs(jobNums).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
				dictJobActiveLinksAll=JobActiveLinks.GetJobActiveLinksForJobNums(jobNums).GroupBy(x => x.JobNum).ToDictionary(x => x.Key,x => x.ToList());
			}
			foreach(Job job in listJobsAll) {
				if(!dictJobLinksAll.TryGetValue(job.JobNum,out job.ListJobLinks)) {
					job.ListJobLinks=new List<JobLink>();//empty list if not found
				}
				if(!dictJobNotesAll.TryGetValue(job.JobNum,out job.ListJobNotes)) {
					job.ListJobNotes=new List<JobNote>();//empty list if not found
				}
				if(!dictJobReviewsAll.TryGetValue(job.JobNum,out job.ListJobReviews)) {
					job.ListJobReviews=new List<JobReview>();//empty list if not found
				}
				if(!dictJobTimeLogsAll.TryGetValue(job.JobNum,out job.ListJobTimeLogs)) {
					job.ListJobTimeLogs=new List<JobReview>();//empty list if not found
				}
				if(!dictJobQuotesAll.TryGetValue(job.JobNum,out job.ListJobQuotes)) {
					job.ListJobQuotes=new List<JobQuote>();//empty list if not found
				}
				if(!dictJobNotificationsAll.TryGetValue(job.JobNum,out job.ListJobNotifications)) {
					job.ListJobNotifications=new List<JobNotification>();//empty list if not found
				}
				if(!dictJobActiveLinksAll.TryGetValue(job.JobNum,out job.ListJobActiveLinks)) {
					job.ListJobActiveLinks=new List<JobActiveLink>();//empty list if not found
				}
			}
		}

		///<summary>Must be called after job is filled using Jobs.FillInMemoryLists(). Returns list of user nums associated with this job.
		/// Currently that is Expert, Owner, and Watchers.</summary>
		public static List<long> GetUserNums(Job job,bool HasApprover=false) {
			List<long> retVal = new List<long> {
				job.UserNumConcept,
				job.UserNumExpert,
				job.UserNumEngineer,
				job.UserNumDocumenter,
				job.UserNumCustContact,
				job.UserNumCheckout,
				job.UserNumInfo
			};
			if(HasApprover) {
				retVal.AddRange(new[]{
					job.UserNumApproverConcept,
					job.UserNumApproverJob,
					job.UserNumApproverChange,
				});
			}
			job.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Subscriber).ForEach(x => retVal.Add(x.FKey));
			job.ListJobReviews.ForEach(x => retVal.Add(x.ReviewerNum));
			return retVal;
		}

		///<summary>Attempts to find infinite loop when changing job parent. Can be optimized to reduce trips to DB since we have all jobs in memory in the job manager.</summary>
		public static bool CheckForLoop(long jobNum,long jobNumParent) {
			List<long> lineage=new List<long>(){jobNum};
			long parentNumNext=jobNumParent;
			while(parentNumNext!=0){
				if(lineage.Contains(parentNumNext)) {
					return true;//loop found
				}
				Job jobNext=Jobs.GetOne(parentNumNext);
				lineage.Add(parentNumNext);
				parentNumNext=jobNext.ParentNum;
			} 
			return false;//no loop detected
		}

		public static List<JobEmail> GetCustomerEmails(Job job) {
			//No direct call to db in this method, but there is in private called methods. Check remoting roles here.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<JobEmail>>(MethodBase.GetCurrentMethod(),job);
			}
			Dictionary<long,JobEmail> retVal = new Dictionary<long,JobEmail>();
			foreach(JobQuote jobQuote in job.ListJobQuotes) {
				long patNum = jobQuote.PatNum;
				if(!retVal.ContainsKey(patNum)) {
					Patient pat = Patients.GetPat(patNum);
					if(pat==null) {
						continue;
					}
					string phones = "Hm:"+pat.HmPhone+"\r\nMo:"+pat.WirelessPhone+"\r\nWk:"+pat.WkPhone;
					retVal[patNum]=new JobEmail() { Pat=pat,EmailAddress=pat.Email,PhoneNums=phones,IsSend=false };
				}
				retVal[patNum].IsQuote=true;
			}
			foreach(JobLink jobLink in job.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Task)) {
				Task task = Tasks.GetOne(jobLink.FKey);
				if(task==null || task.KeyNum==0 || task.ObjectType!=TaskObjectType.Patient) {
					continue;
				}
				long patNum = task.KeyNum;
				if(!retVal.ContainsKey(patNum)) {
					Patient pat = Patients.GetPat(patNum);
					if(pat==null) {
						continue;
					}
					string phones = "Hm:"+pat.HmPhone+"\r\nMo:"+pat.WirelessPhone+"\r\nWk:"+pat.WkPhone;
					retVal[patNum]=new JobEmail() { Pat=pat,EmailAddress=pat.Email,PhoneNums=phones,IsSend=true };
				}
				retVal[patNum].IsTask=true;
			}
			foreach(JobLink jobLink in job.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request)) {
				DataTable tableFR = GetFeatureRequestContact(jobLink.FKey);
				foreach(DataRow row in tableFR.Rows) {
					long patNum = PIn.Long(row["ODPatNum"].ToString());
					Patient pat = Patients.GetPat(patNum);
					if(!retVal.ContainsKey(patNum)) {
						string phones = "Hm:"+row["HmPhone"].ToString()+"\r\nMo:"+row["WirelessPhone"].ToString()+"\r\nWk:"+row["WkPhone"].ToString();
						retVal[patNum]=new JobEmail() { Pat=pat,EmailAddress=row["Email"].ToString(),PhoneNums=phones,IsSend=true };
					}
					retVal[patNum].IsFeatureReq=true;
					retVal[patNum].Votes+=PIn.Int(row["Votes"].ToString());
					retVal[patNum].PledgeAmount+=PIn.Double(row["AmountPledged_"].ToString());
				}
			}
			return retVal.Select(x=>x.Value).ToList();
		}

		///<summary>This is the query that was used prior to the job manager to lookup customer votes, pledges, and contact information for feature requests.</summary>
		private static DataTable GetFeatureRequestContact(long featureRequestNum) {
			//No remoting role check needed; private method.
			string command = "SELECT A.RequestID, A.LName, A.FName, A.ODPatNum, A.BillingType, A.Email, A.HmPhone, "
				+"A.WkPhone, A.WirelessPhone, A.Votes, A.AmountPledged AS AmountPledged_, A.DateVote "
				+"FROM "
				+"(SELECT 1 AS ItemOrder,	request.RequestId, p.LName,	p.FName,	p.PatNum AS 'ODPatNum',	'' AS BillingType, "
				+"  p.Email,	p.HmPhone,	p.WkPhone,	p.WirelessPhone,	'' AS Votes,	'' AS AmountPledged,	request.DateTimeEntry AS 'DateVote' "
				+"FROM bugs.request	INNER JOIN customers.Patient p ON p.PatNum = request.PatNum "
				+"WHERE bugs.request.RequestId ="+POut.Long(featureRequestNum)+" "
				+" UNION ALL "
				+"SELECT 2 AS ItemOrder, vote.RequestID AS RequestID,	p.LName,	p.FName,	p.PatNum AS 'ODPatNum',	def.ItemName AS BillingType, "
				+"  p.Email,	p.HmPhone,	p.WkPhone,	p.WirelessPhone,	vote.Points AS Votes,	vote.AmountPledged,	vote.DateTStamp AS 'DateVote' "
				+"FROM bugs.vote INNER JOIN customers.Patient p ON p.PatNum = vote.PatNum INNER JOIN customers.definition def ON def.DefNum = p.BillingType "
				+" WHERE vote.RequestId ="+POut.Long(featureRequestNum)+" "
				+") A "
				+"ORDER BY CAST(A.RequestID AS UNSIGNED INTEGER), A.ItemOrder";
			return Db.GetTable(command);
		}

		public static DataTable GetActiveJobsForUser(long UserNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),UserNum);
			}
			string command=$@"SELECT Priority, DateTimeEntry AS 'Date Entered', PhaseCur AS 'Phase', Title
				FROM job
				WHERE UserNumEngineer={UserNum}
				AND PhaseCur NOT IN({POut.Enum(JobStatus.Complete)},{POut.Enum(JobStatus.Cancelled)},{POut.Enum(JobStatus.Documentation)})
				AND Priority!={POut.Long(Defs.GetDefsForCategory(DefCat.JobPriorities,true).First(x => x.ItemValue.Contains("OnHold")).DefNum)}";
			return Db.GetTable(command);
		}

		public static List<Userod> GetAssociatedUsers(Job job) {
			List<Userod> listUsers=new List<Userod>();
			foreach(Userod user in Userods.GetDeepCopy()) {
				JobAction action;
				action=job.ActionForUser(user);
				if(action.In(JobAction.Document,JobAction.NeedsTechnicalWriter,JobAction.ContactCustomer,JobAction.ContactCustomerPreDoc,JobAction.Undefined,JobAction.UnknownJobPhase,JobAction.None)) {
					continue;
				}
				if(job.Category==JobCategory.Query || job.Category==JobCategory.MarketingDesign) {
					continue;
				}
				listUsers.Add(user);
			}
			return listUsers;
		}

		///<summary>Generates a git branch name for the job that follows the required syntax.</summary>
		public static string GetGitBranchName(Job job) {
			List<string> listTitle=job.Title.Split(' ').ToList();//Split job title on spaces
			List<string> listStrippedTitle=new List<string>();
			for(int i=0;i<listTitle.Count;i++) {
				string result=StringTools.StripSpecialChars(listTitle[i]);//Remove all special characters from each individual string in our split job title
				if(!string.IsNullOrWhiteSpace(result)) {//If after stripping the string, it is not an empty string, then add it to our list
					listStrippedTitle.Add(result);
				}
			}
			//Format and return the GIT branch name
			return job.Category.ToString().Substring(0,1)+job.JobNum+"_"+string.Join("_",listStrippedTitle);
		}

		public class JobEmail {
			public Patient Pat;
			public string EmailAddress;
			public double PledgeAmount;
			public string PhoneNums;
			public int Votes;
			public bool IsQuote;
			public bool IsTask;
			public bool IsFeatureReq;
			public bool IsSend;
			///<summary>UI field to display send errors.</summary>
			public string StatusMsg;
		}
	}
}

