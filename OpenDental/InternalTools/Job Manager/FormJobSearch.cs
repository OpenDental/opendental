using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Drawing.Text;
using System.Linq;
using System.Xml;

namespace OpenDental {
	public partial class FormJobSearch:FormODBase {
		public string InitialSearchString="";
		///<summary>List of all jobs currently loaded into JobManagerHelper</summary>
		private List<Job> _listJobsAll;
		private List<Job> _listJobsSearch;
		private List<FeatureRequest> _listFeatureRequestsAll=new List<FeatureRequest>();
		private List<Bug> _listBugsAll=new List<Bug>();
		private Job _selectedJob;
		private List<Job> _listSelectedJobs;
		private bool _isMultiSelect;
		private List<Def> _listJobPriorities;

		///<summary>Returns the JobNum for the selected job.  Returns 0 if no job selected.
		///This getter is helpful when the only information needed is the JobNum which will save db calls due to not filling the Job object.</summary>
		public long SelectedJobNum {
			get {
				return _selectedJob?.JobNum??0;
			}
		}

		///<summary>Returns the selected job after filling all of the in-memory lists from the database.</summary>
		public Job SelectedJob {
			get {
				//Fill the in-memory lists from the database
				Jobs.FillInMemoryLists(new List<Job>() { _selectedJob });
				return _selectedJob;
			}
		}

		public FormJobSearch(bool isMultiSelect=false) {
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			//Make a deep copy of the list of jobs itself (not a deep copy of the jobs within the list).
			//This is so that whatever we do to _listJobsAll will not affect listJobsAll that was passed in.
			_listJobsAll=new List<Job>(JobManagerCore.ListJobsAll);
			dateFrom.Value=DateTime.Now.AddYears(-1);
			dateTo.Value=DateTime.Now;
			_isMultiSelect=isMultiSelect;
		}

		private void FormJobSearch_Load(object sender,EventArgs e) {
			if(_isMultiSelect) {
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			textSearch.Text=InitialSearchString;
			listBoxUsers.Items.Add("Any");
			listBoxUsers.Items.AddList(Userods.GetDeepCopy(true),x => x.UserName);
			listBoxUsers.SetSelected(0);
			//Statuses
			listBoxPhases.Items.Add("Any");
			listBoxPhases.Items.AddEnums<JobPhase>();
			listBoxPhases.SetAll(true);
			listBoxPhases.SetSelected(0,false);
			listBoxPhases.SetSelected((int)JobPhase.Cancelled+1,false);
			listBoxPhases.SetSelected((int)JobPhase.Complete+1,false);
			//Categories
			listBoxCategory.Items.Add("Any");
			listBoxCategory.Items.AddEnums<JobCategory>();
			listBoxCategory.SetSelected(0);
			//Priorities
			listBoxPriorities.Items.Add("Any");
			listBoxPriorities.SelectedIndex=0;
			foreach(Def def in Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList()) {
				listBoxPriorities.Items.Add(def.ItemName,def);
			}
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormJobSearch_Shown(object sender,EventArgs e) {
			FillGridMain();
		}

		private void FillGridMain() {
			string[] searchTokens=textSearch.Text.ToLower().Split(new[] { " " },StringSplitOptions.RemoveEmptyEntries);
			long[] userNums=new long[0];
			JobCategory[] jobCats=new JobCategory[0];
			JobPhase[] jobPhases=new JobPhase[0];
			long[] jobPriorities=new long[0];
			if(listBoxUsers.SelectedIndices.Count>0 && !listBoxUsers.SelectedIndices.Contains(0)) {
				userNums=listBoxUsers.GetListSelected<Userod>().Select(x => x.UserNum).ToArray();//This excludes 'Any' so is safe
			}
			if(listBoxCategory.SelectedIndices.Count>0 && !listBoxCategory.SelectedIndices.Contains(0)) {
				jobCats=listBoxCategory.GetListSelected<JobCategory>().ToArray();//This excludes 'Any' so is safe
			}
			if(listBoxPhases.SelectedIndices.Count>0 && !listBoxPhases.SelectedIndices.Contains(0)) {
				jobPhases=listBoxPhases.GetListSelected<JobPhase>().ToArray();//This excludes 'Any' so is safe
			}
			if(listBoxPriorities.SelectedIndices.Count>0 && !listBoxPriorities.SelectedIndices.Contains(0)) {
				jobPriorities=listBoxPriorities.GetListSelected<Def>().Select(x => x.DefNum).ToArray();
			}
			Action actionCloseProgress=ODProgress.Show(ODEventType.Job,typeof(JobEvent),"Getting job data...");
			#region Get Missing Data
			//This entire section will go out to the database and get any data that is unknown based on some of the filters.
			//The other filters will be applied later via the cached lists.
			try {
				List<Job> listJobs=Jobs.GetForSearch(dateFrom.Value,dateTo.Value,jobPhases.ToList(),jobPriorities.ToList(),_listJobsAll.Select(x => x.JobNum).ToList());
				Jobs.FillInMemoryLists(listJobs,true);
				_listJobsAll.AddRange(listJobs);
			}
			catch(OutOfMemoryException oome) {
				actionCloseProgress();
				oome.DoNothing();
				MsgBox.Show(this,"Not enough memory to complete the search.  Please refine search filters.");
				return;
			}
			//Only get the feature request entries that we care about.
			JobEvent.Fire(ODEventType.Job,"Getting feature request data...");
			List<long> listFeatureRequestNums=_listJobsAll.SelectMany(x => x.ListJobLinks)
				.Where(x => x.LinkType==JobLinkType.Request)
				.Select(x => x.FKey)
				.Distinct()
				.ToList();
			//Don't download any feature requests that we already know about.
			listFeatureRequestNums.RemoveAll(x => ListTools.In(x,_listFeatureRequestsAll.Select(y => y.FeatReqNum)));
			if(!listFeatureRequestNums.IsNullOrEmpty()) {
				_listFeatureRequestsAll.AddRange(FeatureRequests.GetAll(listFeatureRequestNums));
			}
			//Only get the bug entries that we care about.
			JobEvent.Fire(ODEventType.Job,"Getting bug data...");
			List<long> listBugIds=_listJobsAll.SelectMany(x => x.ListJobLinks)
				.Where(x => x.LinkType==JobLinkType.Bug)
				.Select(x => x.FKey)
				.Distinct()
				.ToList();
			//Don't download any bugs that we already know about.
			listBugIds.RemoveAll(x => ListTools.In(x,_listBugsAll.Select(y => y.BugId)));
			if(!listBugIds.IsNullOrEmpty()) {
				_listBugsAll.AddRange(Bugs.GetMany(listBugIds));
			}
			#endregion
			JobEvent.Fire(ODEventType.Job,"Filling grid...");
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn("Job\r\nNum",50,GridSortingStrategy.AmountParse));
			gridMain.ListGridColumns.Add(new GridColumn("Priority",50,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn("Phase",85));
			gridMain.ListGridColumns.Add(new GridColumn("Category",80));
			gridMain.ListGridColumns.Add(new GridColumn("Job Title",160){ IsWidthDynamic=true });
			gridMain.ListGridColumns.Add(new GridColumn("Version",80));
			gridMain.ListGridColumns.Add(new GridColumn("Est. Version",80));
			gridMain.ListGridColumns.Add(new GridColumn("Expert",75));
			gridMain.ListGridColumns.Add(new GridColumn("Engineer",75));
			gridMain.ListGridColumns.Add(new GridColumn("Est.\r\nHours",60,GridSortingStrategy.AmountParse));
			gridMain.ListGridColumns.Add(new GridColumn("Act.\r\nHours",60,GridSortingStrategy.AmountParse));
			gridMain.ListGridColumns.Add(new GridColumn("Job\r\nMatch",45,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn("Bug\r\nMatch",45,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new GridColumn("FR\r\nMatch",45,HorizontalAlignment.Center));
			gridMain.ListGridRows.Clear();
			_listJobsSearch=new List<Job>();
			foreach(Job jobCur in _listJobsAll) {
				if(jobCats.Length>0 && !jobCats.Contains(jobCur.Category)) {
					continue;
				}
				if(jobPhases.Length>0 && !jobPhases.Contains(jobCur.PhaseCur)) {
					continue;
				}
				if(jobPriorities.Length>0 && !jobPriorities.Contains(jobCur.Priority)) {
					continue;
				}
				if(userNums.Length>0 && !userNums.All(x => Jobs.GetUserNums(jobCur).Contains(x))) {
					continue;
				}
				if(!jobCur.DateTimeEntry.Between(dateFrom.Value,dateTo.Value)) {
					continue;
				}
				bool isJobMatch=false;
				bool isBugMatch=false;
				bool isFeatureReqMatch=false;
				if(searchTokens.Length>0) {
					bool addRow=false;
					List<Bug> listBugs=jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Bug)
						.Select(x => _listBugsAll.FirstOrDefault(y => x.FKey==y.BugId))
						.Where(x => x!=null)
						.ToList();
					List<FeatureRequest> listFeatures=jobCur.ListJobLinks.FindAll(x => x.LinkType==JobLinkType.Request)
						.Select(x => _listFeatureRequestsAll.FirstOrDefault(y => x.FKey==y.FeatReqNum))
						.Where(x => x!=null)
						.ToList();
					foreach(string token in searchTokens.Distinct()) {
						bool isFound=false;
						//JOB MATCHES
						if(jobCur.Title.ToLower().Contains(token) 
							|| jobCur.Implementation.ToLower().Contains(token)
							|| jobCur.Requirements.ToLower().Contains(token) 
							|| jobCur.Documentation.ToLower().Contains(token)
							|| jobCur.JobNum.ToString().Contains(token)) 
						{
							isFound=true;
							isJobMatch=true;
						}
						//BUG MATCHES
						if(!isFound || !isBugMatch) {
							if(listBugs.Any(x => x.Description.ToLower().Contains(token) || x.Discussion.ToLower().Contains(token))) {
								isFound=true;
								isBugMatch=true;
							}
						}
						//FEATURE REQUEST MATCHES
						if(!isFound || !isFeatureReqMatch) {
							if(listFeatures.Any(x => x.Description.Contains(token) || x.FeatReqNum.ToString().ToLower().Contains(token))) {
								isFound=true;
								isFeatureReqMatch=true;
							}
						}
						addRow=isFound;
						if(!isFound) {
							break;//stop looking for additional tokens, we didn't find this one.
						}
					}
					if(!addRow) {
						continue;//we did not find one of the search terms.
					}
				}
				_listJobsSearch.Add(jobCur);
				Def jobPriority=_listJobPriorities.FirstOrDefault(y => y.DefNum==jobCur.Priority);
				GridRow row=new GridRow();
				row.Cells.Add(jobCur.JobNum.ToString());
				row.Cells.Add(new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(jobCur.Priority==_listJobPriorities.FirstOrDefault(y => y.ItemValue.Contains("Urgent")).DefNum) ? Color.White : Color.Black,
						});
				row.Cells.Add(jobCur.PhaseCur.ToString());
				row.Cells.Add(jobCur.Category.ToString());
				row.Cells.Add(jobCur.Title);
				row.Cells.Add(jobCur.JobVersion.ToString());
				row.Cells.Add(jobCur.ProposedVersion.ToString());
				row.Cells.Add(Userods.GetName(jobCur.UserNumExpert));
				row.Cells.Add(Userods.GetName(jobCur.UserNumEngineer));
				row.Cells.Add(jobCur.HoursEstimate.ToString());
				row.Cells.Add(jobCur.HoursActual.ToString());
				row.Cells.Add(isJobMatch ? "X" : "");
				row.Cells.Add(isBugMatch ? "X" : "");
				row.Cells.Add(new GridCell(isFeatureReqMatch ? "X" : "") { ColorBackG=_listFeatureRequestsAll.Count==0 ? Control.DefaultBackColor : Color.Empty });
				row.Tag=jobCur;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			actionCloseProgress();
			//Keep the focus on the search box
			textSearch.Focus();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(_isMultiSelect) {
				return;
			}
			if(e.Row<0 || e.Row>gridMain.ListGridRows.Count || !(gridMain.ListGridRows[e.Row].Tag is Job)) {
				_selectedJob=null;
				return;
			}
			_selectedJob=(Job)gridMain.ListGridRows[e.Row].Tag;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(e.Row<0 || e.Row>gridMain.ListGridRows.Count || !(gridMain.ListGridRows[e.Row].Tag is Job)) {
				_selectedJob=null;
				return;
			}
			if(_isMultiSelect) {
				_listSelectedJobs=gridMain.SelectedGridRows.Select(x => (Job)x.Tag).ToList();
			}
			_selectedJob=(Job)gridMain.ListGridRows[e.Row].Tag;
			DialogResult=DialogResult.OK;
		}

		private void butSearch_Click(object sender,EventArgs e) {
			FillGridMain();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_isMultiSelect) {
				if(gridMain.SelectedGridRows.Count==0) {
					MsgBox.Show(this,"Please select at least one job before clicking OK.");
					return;
				}
				_listSelectedJobs=gridMain.SelectedGridRows.Select(x => (Job)x.Tag).ToList();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormJobSearch_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				JobManagerCore.AddSearchJobsToList(_listJobsSearch);
			}
		}
	}
}