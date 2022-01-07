using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {

	public static class JobManagerCore {
		///<summary>All jobs</summary>
		private static List<Job> _listJobsAll = new List<Job>();
		///<summary>Jobs to be displayed in search.</summary>
		private static List<Job> _listJobsSearch = new List<Job>();
		///<summary>DateTime the last refresh was done to the core.</summary>
		private static DateTime _lastRefreshDateTime = DateTime.MinValue;
		///<summary>This lock object is used to lock all static _listJob... entities in this class.</summary>
		private static object _lock=new object();

		public static List<Job> ListJobsAll {
			get {
				bool isNull=false;
				lock(_lock) {
					if(_listJobsAll==null) {
						isNull=true;
					}
				}
				if(isNull) {
					RefreshAndFill();
				}
				List<Job> retVal;
				lock(_lock) {
					retVal=new List<Job>(_listJobsAll);
				}
				return retVal;
			}
		}

		public static List<Job> ListJobsSearch {
			get {
				lock(_lock) {
					if(_listJobsSearch==null) {
						_listJobsSearch=new List<Job>();
					}
					return new List<Job>(_listJobsSearch);
				}
			}
		}

		public static List<long> ListJobNumsAll {
			get {
				return ListJobsAll.Select(x => x.JobNum).ToList();
			}
		}		
		
		public static DateTime LastRefreshDateTime {
			get {
				return _lastRefreshDateTime;
			}
		}

		#region Refresh Methods
		private static void RefreshAndFill(List<long> listJobNums=null) {
			//New List for listJobs since it will be getting overwritten
			List<Job> listJobsForRefresh=new List<Job>();
			//Make a dictionary that will hold a key for every known JobNum (especially for when specific JobNums are passed in).
			//This is mainly for removing jobs that have been deleted when refreshing for signal processing purposes.
			Dictionary<long,Job> dictRefreshJobs;
			//Get all pertinent jobs related to the job manager if no specific JobNums were specified.
			if(listJobNums.IsNullOrEmpty()) {
				listJobsForRefresh=Jobs.GetAllForManager();
				Jobs.FillInMemoryLists(listJobsForRefresh);
				dictRefreshJobs=listJobsForRefresh.ToDictionary(x => x.JobNum,x => x);
			}
			else {
				listJobNums=listJobNums.Distinct().ToList();
				listJobsForRefresh=Jobs.GetMany(listJobNums);
				Jobs.FillInMemoryLists(listJobsForRefresh);
				//Make the dictionary out of listJobNums but fill it with jobs that were returned from the database.
				//This will allow us to leave some values as null if the job was deleted. This will cause us to remove the job from our cache later.
				dictRefreshJobs=listJobNums.ToDictionary(x => x,x => listJobsForRefresh.FirstOrDefault(y => y.JobNum==x));
			}
			UpdateCachedLists(dictRefreshJobs);
		}
		
		public static void UpdateForSingleJob(Job job) {
			Dictionary<long,Job> dictRefreshJob=new Dictionary<long, Job>();
			dictRefreshJob.Add(job.JobNum,job);
			UpdateCachedLists(dictRefreshJob);
		}

		public static void UpdateCachedLists(Dictionary<long,Job> dictRefreshJobs) {
			List<Job> listJobsAll=ListJobsAll;
			//Copy of the ListJobSearch since the list is either empty (On initial load) or it has already been filled and we don't need to worry about it
			List<Job> listJobsSearch=ListJobsSearch;
			foreach(KeyValuePair<long,Job> kvp in dictRefreshJobs) {
				if(kvp.Value==null) {//deleted job
					listJobsAll.RemoveAll(x => x.JobNum==kvp.Key);
					listJobsSearch.RemoveAll(x => x.JobNum==kvp.Key);
					continue;
				}
				//Master Job List
				Job jobOld=listJobsAll.FirstOrDefault(x => x.JobNum==kvp.Key);
				if(jobOld==null) {//new job entirely, no need to update anything in memory, just add to jobs list.
					listJobsAll.Add(kvp.Value);
					continue;
				}
				listJobsAll[listJobsAll.IndexOf(jobOld)]=kvp.Value;
				//Filtered Job List
				jobOld=listJobsSearch.FirstOrDefault(x => x.JobNum==kvp.Key);
				if(jobOld!=null) {//update item in filtered list.
					listJobsSearch[listJobsSearch.IndexOf(jobOld)]=kvp.Value;
				}
			}
			lock(_lock) {
				_listJobsAll=listJobsAll;
				_listJobsSearch=listJobsSearch;
			}
		}

		///<summary>Fills all in memory data from the DB on a seperate thread and then refills controls.
		///Optionally pass in a list of job nums to only refresh those specific jobs.</summary>
		public static void RefreshAndFillThreaded(List<long> listJobNums=null) {
			ODThread thread=new ODThread((o) => {
				RefreshAndFill(listJobNums);
				_lastRefreshDateTime=DateTime.Now;
			});
			thread.GroupName="RefreshAndFillJobManager";
			thread.Name="RefreshAndFillJobManager";
			thread.AddExceptionHandler((ex) => { });
			thread.Start();
		}

		public static void AddTestingJobsToList(string versionText) {
			//Completed jobs are not included in _listJobsAll by default
			List<Job> listJobs=Jobs.GetForTesting(versionText,listExcludeJobNums:ListJobNumsAll);
			if(!listJobs.IsNullOrEmpty()) {
				Jobs.FillInMemoryLists(listJobs);
				lock(_lock) {
					_listJobsAll.AddRange(listJobs);
				}
			}
		}

		public static void AddQueryJobsToList() {
			List<Job> listJobs = Jobs.GetForQueries(listExcludeJobNums:ListJobNumsAll);
			if(!listJobs.IsNullOrEmpty()) {
				Jobs.FillInMemoryLists(listJobs);
				lock(_lock) {
					_listJobsAll.AddRange(listJobs);
				}
			}
		}

		public static void AddMarketingJobsToList() {
			List<Job> listJobs = Jobs.GetForMarketing(listExcludeJobNums:ListJobNumsAll);
			if(!listJobs.IsNullOrEmpty()) {
				Jobs.FillInMemoryLists(listJobs);
				lock(_lock) {
					_listJobsAll.AddRange(listJobs);
				}
			}
		}

		public static void AddUnresolvedIssueJobsToList() {
			List<Job> listJobs = Jobs.GetForUnresolvedIssues(listExcludeJobNums:ListJobNumsAll);
			if(!listJobs.IsNullOrEmpty()) {
				Jobs.FillInMemoryLists(listJobs);
				lock(_lock) {
					_listJobsAll.AddRange(listJobs);
				}
			}
		}
		#endregion

		public static void ClearJobCache() {
			lock(_lock) {
				_listJobsAll=new List<Job>();
				_listJobsSearch=new List<Job>();
				_lastRefreshDateTime=DateTime.MinValue;
			}
		}

		///<summary>Returns all remaining jobs that after applying UI filters.  Fills any in-memory lists from the database.</summary>
		public static void AddSearchJobsToList(List<Job> listJobSearch) {
			if(listJobSearch.IsNullOrEmpty()) {
				return;
			}
			List<Job> listJobs=_listJobsAll;
			List<long> listJobNums=ListJobNumsAll;
			//Fill the in-memory lists from the database
			Jobs.FillInMemoryLists(listJobSearch.FindAll(x => !listJobNums.Exists(y => x.JobNum==y)).ToList());
			List<Job> listJobsToAdd=listJobSearch.Where(x => !ListTools.In(x.JobNum,listJobs.Select(x => x.JobNum))).ToList();
			lock(_lock) {
				_listJobsSearch=listJobSearch;
				_listJobsAll.AddRange(listJobsToAdd);
			}
		}

		///<summary>Returns the job based on the given JobNum. If the job is not in the in-memory list, this method will retrieve it from the database and fill it. If the job does not exist in the database then this method will return null.</summary>
		public static Job GetJob(long jobNum) {
			Job job=ListJobsAll.FirstOrDefault(x => x.JobNum==jobNum);
			if(job==null) {
				job=Jobs.GetOneFilled(jobNum);
				if(job==null) {
					return job;
				}
				UpdateForSingleJob(job);
			}
			return job;
		}

		///<summary>Check for heirarchical loops when moving a child job to a parent job. Throws an exception if a loop is found. Example A>B>C>A would be a loop.</summary>
		public static void ValidateJobLoop(Job jobChild,long jobNumParent) {
			List<long> lineage=new List<long>(){jobChild.JobNum};
			Job jobCur=jobChild.Copy();
			jobCur.ParentNum=jobNumParent;
			while(jobCur.ParentNum!=0) {
				if(lineage.Contains(jobCur.ParentNum)) {
					throw new Exception("Invalid hierarchy detected. Moving the job there would create an infinite loop.");
				}
				Job jobNext=ListJobsAll.FirstOrDefault(x=>x.JobNum==jobCur.ParentNum);
				if(jobNext==null) {
					throw new Exception("Invalid hierarchy detected. Cannot find job "+jobCur.ParentNum);
				}
				jobCur=jobNext;
				lineage.Add(jobCur.JobNum);
			} 
			return;//no loop detected
		}

	}

	#region Enums
	///<summary>For UI only. Never saved to DB.</summary>
	public enum GroupJobsBy {
		None,
		MyHierarchy,
		Hierarchy,
		User,
		Status,
		Owner
	}

	///<summary>For UI only. Never saved to DB.</summary>
	public enum LoadJobAction {
		Select,
		Back,
		Forward
	}
	#endregion
}
