using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;
using System.Diagnostics;
using System.IO;

namespace OpenDental {
	public partial class FormJobManagerOverview:FormODBase {
		private bool _headingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;
		///<summary>List of jobs filtered by UI elements. Used to keep contents of gridEngineerHours and gridJobs in synch. </summary>
		private List<Job> _listJobsFiltered;
		private List<JobTeam> _listJobTeams;
		private List<JobTeamUser> _listJobTeamUsers;

		public FormJobManagerOverview() {
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
		}

		private void FormJobManagerOverview_Load(object sender,EventArgs e) {
			WindowState=FormWindowState.Maximized;
			MinimumSize=Size;
			MaximumSize=Size;
			foreach(Def def in Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList()) {
				listPriorities.Items.Add(def.ItemName,def);
			}
			listPriorities.SelectedIndex=0;
			foreach(JobPhase phase in Enum.GetValues(typeof(JobPhase))) {
				listPhases.Items.Add(phase.ToString(),phase);
			}
			listPhases.SelectedIndex=0;
			foreach(JobCategory category in Enum.GetValues(typeof(JobCategory))) {
				listCategories.Items.Add(category.ToString(),category);
			}
			listCategories.SelectedIndex=0;
			FillComboEngineers(JobHelper.ListEngineerUsers);//fill engineers first on load for all engineers (includes users that may have not yet been associated to a team)
			_listJobTeams=JobTeams.GetDeepCopy();
			FillComboTeams(_listJobTeams);
			_listJobTeamUsers=JobTeamUsers.GetDeepCopy();
			dateRangeJobCompleted.SetDateTimeFrom(DateTime.Now.AddMonths(-1));
			dateRangeJobCompleted.SetDateTimeTo(DateTime.Now);
			SetFilters();
			_listJobsFiltered=GetFilteredJobList();
			FillGridEngineerHours(_listJobsFiltered);
			FillGridJobs(_listJobsFiltered);
			FillGridSprints();
			if(!JobPermissions.IsAuthorized(JobPerm.Approval,true)) {
				butFixTopParents.Visible=false;
			}
		}

		private void FillComboEngineers(List<Userod> listUsers) {
			comboEngineers.Items.Clear();
			comboEngineers.Items.Add("All",new Userod() { UserNum=-1 });
			comboEngineers.Items.Add("Unassigned",new Userod() { UserNum=0 });
			comboEngineers.Items.AddList(listUsers,x => x.UserName);
			comboEngineers.SelectedIndex=0;
		}

		private void FillComboTeams(List<JobTeam> listTeams) {
			comboTeams.Items.Clear();
			comboTeams.Items.Add("All");
			comboTeams.Items.AddList(listTeams,x => x.TeamName);
			comboTeams.SelectedIndex=0;
		}

		#region Menu Clicks
		private void butSprintManager_Click(object sender,EventArgs e) {
			panelJobsReport.Visible=false;
			panelSprintManager.Visible=true;
		}

		private void butJobsReport_Click(object sender,EventArgs e) {
			panelJobsReport.Visible=true;
			panelSprintManager.Visible=false;
		}
		#endregion

		#region Sprint Manager
		private void FillGridSprints()
		{
			gridSprints.BeginUpdate();
			gridSprints.Columns.Clear();
			gridSprints.Columns.Add(new GridColumn("Start Date", 70));
			gridSprints.Columns.Add(new GridColumn("Title", 50));
			gridSprints.ListGridRows.Clear();
			foreach (JobSprint sprint in JobSprints.GetAll().OrderByDescending(x => x.DateStart))
			{
				GridRow row = new GridRow() { Tag = sprint };
				row.Cells.Add(sprint.DateStart.ToShortDateString());
				row.Cells.Add(sprint.Title.ToString());
				gridSprints.ListGridRows.Add(row);
			}
			gridSprints.EndUpdate();
		}

		private void FillGridEngineers(JobSprint jobSprint) {
			gridEngineers.BeginUpdate();
			gridEngineers.Columns.Clear();
			gridEngineers.Columns.Add(new GridColumn("Engineer",80){ IsWidthDynamic=true });
			gridEngineers.Columns.Add(new GridColumn("Scheduled Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.Columns.Add(new GridColumn("- Est. Breaks",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.Columns.Add(new GridColumn("= Hrs Total",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.Columns.Add(new GridColumn("Assigned Dev Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.Columns.Add(new GridColumn("Free Dev Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.Columns.Add(new GridColumn("Completed Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.ListGridRows.Clear();
			double jobPercent=jobSprint.JobPercent;
			double avgDevHours=jobSprint.HoursAverageDevelopment;
			double avgBreakHours=jobSprint.HoursAverageBreak;
			List<Schedule> listEngSchedules=Schedules.RefreshPeriodForEmps(jobSprint.DateStart,jobSprint.DateEndTarget,JobHelper.ListEngineerEmployeeNums);
			foreach(Userod userEng in JobHelper.ListEngineerUsers) {
				double schedHoursTotal=0;
				double schedHoursBreaksTotal=0;
				double schedHoursPercentTotal=0;
				double allocatableHours=0;
				foreach(Schedule sched in listEngSchedules.FindAll(x => x.EmployeeNum==userEng.EmployeeNum)) {
					//Calculate actual scheduled time
					double schedHours=(sched.StopTime-sched.StartTime).TotalHours;
					schedHoursTotal+=schedHours;
					//Remove average break time
					schedHours-=avgBreakHours;
					schedHoursBreaksTotal+=schedHours;
					//Multiply the scheduled time by the percentage of coding time for the jobs we care about
					schedHours=schedHours*jobPercent;
					schedHoursPercentTotal+=schedHours;
					//Add the sched hours to the allocatable hours
					allocatableHours+=schedHours;
				}
				double totalAllocatedHours=0;
				double totalCompletedHours=0;
				//TODO: OwnerNum is not a good enough check here. 
				//Make a method to get a sublist of jobs that legit apply to an engineer (In their writeup/development, not waiting for approval)
				List<Job> listEngJobs=JobManagerCore.ListJobsAll.FindAll(x => !x.IsApprovalNeeded 
					&& ((x.PhaseCur==JobPhase.Development && x.UserNumEngineer==userEng.UserNum)
						|| (x.PhaseCur==JobPhase.Definition && x.UserNumExpert==userEng.UserNum)));
				foreach(Job job in listEngJobs) {
					double estimate=job.HoursEstimate;
					if(job.HoursEstimateDevelopment==0) {
						estimate+=avgDevHours;
					}
					totalAllocatedHours+=estimate;
					totalAllocatedHours-=job.HoursActual;
					totalCompletedHours+=job.HoursActual;
				}
				GridRow row=new GridRow() { Tag=userEng };
				row.Cells.Add(userEng.UserName);
				row.Cells.Add(Math.Round(schedHoursTotal).ToString());
				row.Cells.Add(Math.Round(schedHoursTotal-schedHoursBreaksTotal).ToString());
				row.Cells.Add(Math.Round(schedHoursBreaksTotal).ToString());
				row.Cells.Add(Math.Round(totalAllocatedHours).ToString());
				double freeHrs=Math.Round(schedHoursBreaksTotal-totalAllocatedHours);
				GridCell cell=new GridCell(freeHrs.ToString());
				cell.ColorBackG=freeHrs<20?Color.LightSalmon:Color.LightGreen;//Arbitrary 20 hours.
				row.Cells.Add(cell);
				row.Cells.Add(Math.Round(totalCompletedHours).ToString());
				gridEngineers.ListGridRows.Add(row);
			}
			gridEngineers.EndUpdate();
		}

		private void gridSprints_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			ContextMenu menu=new ContextMenu();
			menu.MenuItems.Add("Remove Sprint",(o,arg) => {
				int selectedIndex=gridSprints.GetSelectedIndex();
				if(selectedIndex==-1) {
					return;//Nothing to remove.
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will permanently delete this sprint. Continue?")) {
					return;
				}
				long sprintNum=((JobSprint)gridSprints.ListGridRows[selectedIndex].Tag).JobSprintNum;
				userControlSprintManager.Enabled=false;
				JobSprints.Delete(sprintNum);
				FillGridSprints();
			});
			menu.MenuItems.Add("Copy Sprint",(o,arg) => {
				int selectedIndex=gridSprints.GetSelectedIndex();
				if(selectedIndex==-1) {
					return;//Nothing to copy.
				}
				JobSprint sprint=((JobSprint)gridSprints.ListGridRows[selectedIndex].Tag);
				sprint.Title+="_Copy";
				List<JobSprintLink> listSprintLinks=JobSprintLinks.GetForSprint(sprint.JobSprintNum);
				JobSprints.Insert(sprint);
				foreach(JobSprintLink sprintLink in listSprintLinks) {
					sprintLink.JobSprintNum=sprint.JobSprintNum;
					JobSprintLinks.Insert(sprintLink);
				}
				FillGridSprints();
				userControlSprintManager.LoadSprint(sprint);
			});
			menu.MenuItems.Add("End Sprint",(o,arg) => {
				int selectedIndex=gridSprints.GetSelectedIndex();
				if(selectedIndex==-1) {
					return;//Nothing to end.
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will set the sprints actual end date to today. Continue?")) {
					return;
				}
				JobSprint sprint;
				//This gets the most recent sprint instance to save and unloads it so we can load it in
				if(!userControlSprintManager.Enabled || !userControlSprintManager.UnloadSprint(out sprint)) {
					return;
				}
				else {
					sprint=((JobSprint)gridSprints.ListGridRows[selectedIndex].Tag);
				}
				sprint.DateEndActual=DateTime.Today;
				JobSprints.Update(sprint);
				FillGridSprints();
				userControlSprintManager.LoadSprint(sprint);
				FillGridEngineers(userControlSprintManager.JobSprintCur);
			});
			menu.Show(gridSprints,gridSprints.PointToClient(Cursor.Position));
		}

		private void gridSprints_CellClick(object sender,ODGridClickEventArgs e) {
			userControlSprintManager.LoadSprint((JobSprint)gridSprints.ListGridRows[e.Row].Tag);
			FillGridEngineers(userControlSprintManager.JobSprintCur);
		}

		private void gridSprints_TitleAddClick(object sender,EventArgs e) {
			JobSprint jobSprintNew=new JobSprint();
			jobSprintNew.Title="Version";
			jobSprintNew.Note="";
			jobSprintNew.DateStart=DateTime.Today;
			jobSprintNew.DateEndTarget=DateTime.Today.AddDays(70);//Add 10 weeks
			jobSprintNew.DateEndActual=DateTime.MinValue;
			jobSprintNew.JobPercent=0.173;
			jobSprintNew.HoursAverageDevelopment=9.43;
			jobSprintNew.HoursAverageBreak=0.85;
			JobSprints.Insert(jobSprintNew);
			FillGridSprints();
			userControlSprintManager.LoadSprint(jobSprintNew);
			FillGridEngineers(userControlSprintManager.JobSprintCur);
		}

		private void userControlSprintManager_SaveClick(object sender,EventArgs e) {
			FillGridSprints();
			FillGridEngineers(userControlSprintManager.JobSprintCur);
		}
		#endregion

		#region Jobs Report
		private void FillGridEngineerHours(List<Job> listJobs) {
			long lastXDays=PIn.Long(textWithinLastXDays.Text);
			List<JobReview> listJobTimeLogs=listJobs
				.SelectMany(x => x.ListJobTimeLogs)														//Add Time entries for all passed in jobs
				.Where(x => x.DateTStamp >= DateTime.Now.AddDays(-lastXDays))	//that occured within the date range
				.ToList();
			List<JobReview> listJobReviews=listJobs
				.SelectMany(x => x.ListJobReviews)														//Review entries for all passed in jobs
				.Where(x => x.DateTStamp >= DateTime.Now.AddDays(-lastXDays))	//that occured within the date range
				.ToList();
			//Default all users
			List<long> listUserNums=JobHelper.ListEngineerUsers.Select(x => x.UserNum).ToList();
			//Filter by Team
			if(comboTeams.SelectedIndex > 0) {
				listUserNums=_listJobTeamUsers
					.Where(x => x.JobTeamNum==comboTeams.GetSelected<JobTeam>().JobTeamNum) 
					.Select(x => x.UserNumEngineer).ToList();
			} 
			//Filter by single engineer
			if(comboEngineers.SelectedIndex > 0) {
				listUserNums=ListTools.FromSingle(comboEngineers.GetSelected<Userod>().UserNum);
			}
			gridEngineerHours.BeginUpdate();
			gridEngineerHours.Columns.Clear();
			gridEngineerHours.Columns.Add(new GridColumn("Engineer", 150) { TextAlign=HorizontalAlignment.Center });
			gridEngineerHours.Columns.Add(new GridColumn("Hours", 150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridEngineerHours.ListGridRows.Clear();
			foreach(long userNum in listUserNums) {
				GridRow row=new GridRow() { Tag=userNum };
				row.Cells.Add(Userods.GetName(userNum));			//Column - Engineer
				double hoursTimeLogs=listJobTimeLogs
					.Where(x => x.ReviewerNum==userNum)			//time added to jobs by this engineer
					.Sum(x => x.TimeReview.TotalHours);			//sum logged time for the engineer
				double hoursReviewer=listJobReviews
					.Where(x => x.ReviewerNum==userNum)			//reviews done by this engineer
					.Sum(x => x.TimeReview.TotalHours);			//sum review time for the reviewer
				double hoursReviewee=listJobs
					.Where(x => x.UserNumEngineer==userNum)	//jobs for this engineer
					.SelectMany(y => y.ListJobReviews)			//that have review time
					.Sum(y => y.TimeReview.TotalHours);			//sum review time for the reviewee
				double hoursTotal=hoursTimeLogs+hoursReviewer+hoursReviewee;
				row.Cells.Add(hoursTotal.ToString("F2"));			//Column - Hours
				gridEngineerHours.ListGridRows.Add(row);
			}
			gridEngineerHours.EndUpdate();
		}

		private void FillGridJobs(List<Job> listJobs) {
			int totalBugs=0;
			int totalFeatures=0;
			double lastXDays=PIn.Long(textWithinLastXDays.Text);
			double totalHrsEst=0;
			double totalHrsSpent=0;
			double totalQuotedDollars=0;
			List<Def> listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			List<string> listProposedVersions=Enum.GetNames(typeof(JobProposedVersion)).ToList();
			gridJobs.BeginUpdate();
			gridJobs.Columns.Clear();
			gridJobs.Columns.Add(new GridColumn("Job",70){ IsWidthDynamic=true });
			gridJobs.Columns.Add(new GridColumn("Owner",70) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new GridColumn("Owner Action",90));
			gridJobs.Columns.Add(new GridColumn("Phase",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new GridColumn("Priority",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new GridColumn("Expert",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new GridColumn("Engineer",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.Columns.Add(new GridColumn("Hrs Est",75) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.Columns.Add(new GridColumn($"Expert Last {lastXDays} Days",140) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.Columns.Add(new GridColumn($"Engineer Last {lastXDays} Days",140) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.Columns.Add(new GridColumn("Hrs Total",75) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.ListGridRows.Clear();
			foreach(Job job in listJobs) {
				Def jobPriority=listJobPriorities.FirstOrDefault(y => y.DefNum==job.Priority);
				totalBugs+=job.ListJobLinks.Count(x => x.LinkType==JobLinkType.Bug);
				totalFeatures+=job.ListJobLinks.Count(x => x.LinkType==JobLinkType.Request);
				totalHrsEst+=job.HoursEstimate;
				totalHrsSpent+=job.HoursActual;
				foreach(JobQuote quote in job.ListJobQuotes) {
					totalQuotedDollars+=PIn.Double(quote.Amount);
				}
				GridRow row=new GridRow() { Tag=job };
				row.Cells.Add(job.ToString());
				row.Cells.Add(Userods.GetName(job.OwnerNum));
				row.Cells.Add(job.OwnerAction.GetDescription());
				row.Cells.Add(job.PhaseCur.ToString());
				GridCell cell=new GridCell(jobPriority.ItemName);
				cell.ColorBackG=jobPriority.ItemColor;
				cell.ColorText=(job.Priority==listJobPriorities.FirstOrDefault(y => y.ItemValue.Contains("Urgent")).DefNum) ? Color.White : Color.Black;
				row.Cells.Add(cell);
				row.Cells.Add(Userods.GetName(job.UserNumExpert));
				row.Cells.Add(Userods.GetName(job.UserNumEngineer));
				row.Cells.Add(job.HoursEstimate.ToString());
				double hrsLastXDaysTimeLogsExpert=job.ListJobTimeLogs
					.Where(x => x.DateTStamp>=DateTime.Today.AddDays(-lastXDays) && x.ReviewerNum==job.UserNumExpert)
					.Sum(y => y.Hours);
				double hrsLastXDaysReviews=job.ListJobReviews
					.Where(x => x.DateTStamp>=DateTime.Today.AddDays(-lastXDays) && x.ReviewerNum==job.UserNumExpert)
					.Sum(y => y.Hours);
				row.Cells.Add((hrsLastXDaysTimeLogsExpert+hrsLastXDaysReviews).ToString());//Expert
				double hrsLastXDaysTimeLogsEngineer=job.ListJobTimeLogs
					.Where(x => x.DateTStamp>=DateTime.Today.AddDays(-lastXDays) && x.ReviewerNum==job.UserNumEngineer)
					.Sum(y => y.Hours);
				row.Cells.Add((hrsLastXDaysTimeLogsEngineer+hrsLastXDaysReviews).ToString());//Engineer
				row.Cells.Add(job.HoursActual.ToString());
				gridJobs.ListGridRows.Add(row);
			}
			gridJobs.EndUpdate();
		}

		private void radioLastSevenDays_Click(object sender,EventArgs e) {
			SetFilters();
		}

		private void radioLastThirtyDays_Click(object sender,EventArgs e) {
			SetFilters();
		}


		private void radioCurrentVersion_Click(object sender,EventArgs e) {
			SetFilters();
		}

		private void gridEngineerHours_CellClick(object sender, ODGridClickEventArgs e) {
			//Bring jobs for engineer selected in gridEngineerHours to top of gridJobs
			long engineerNumSelected=(long)gridEngineerHours.ListGridRows[e.Row].Tag;
			List<Job> listJobsByEngineer=_listJobsFiltered.OrderByDescending(x => x.OwnerNum==engineerNumSelected).ToList();
			FillGridJobs(listJobsByEngineer);
		}

		///<summary>Populates UI with preset/selected filters. </summary>
		private void SetFilters() {
			listPhases.SetAll(true);
			listCategories.SetAll(true);
			listPriorities.SetAll(true);
			comboEngineers.SelectedIndex=0;
			textPatNum.Text="";
			checkRemoveApprovalJobs.Checked=false;
			checkRemoveNoQuote.Checked=false;
			if(radioLastSevenDays.Checked) {
				textWithinLastXDays.Text="7";
			}
			if(radioLastThirtyDays.Checked) {
				textWithinLastXDays.Text="30";
			}
			if(radioCurrentVersion.Checked) {
				textWithinLastXDays.Text="";
			}
		}

		///<summary>Query the DB for cancelled/complete jobs for specific UI filters, add them to the cached list, and then return a filtered cached list.
		///This is because the cached list does not originally hold cancelled/completed jobs.</summary>
		private List<Job> GetFilteredJobList() {
			long lastXDays=PIn.Long(textWithinLastXDays.Text);
			DateTime dateTimeJobCompletedFrom=dateRangeJobCompleted.GetDateTimeFrom();
			DateTime dateTimeJobCompletedTo=dateRangeJobCompleted.GetDateTimeTo();
			List<Job> listJobsNew=Jobs.GetUncachedForReport(lastXDays,dateTimeJobCompletedFrom,dateTimeJobCompletedTo);
			if(listJobsNew.Count > 0) {
				JobManagerCore.AddSearchJobsToList(listJobsNew);//Add queried jobs to the cached list.
			}
			List<Job> listJobs=JobManagerCore.ListJobsAll;
			listJobs=ApplyFilters(listJobs);
			return listJobs.OrderByDescending(x => x.HoursActual).ToList();
		}

		///<summary>Helper method to apply UI filters to our passed in list.</summary>
		private List<Job> ApplyFilters(List<Job> listJobs) {
			JobTeam jobTeamSelected=comboTeams.GetSelected<JobTeam>();
			Userod selectedUser=comboEngineers.GetSelected<Userod>();
			List<JobPhase> listJobPhases=listPhases.GetListSelected<JobPhase>();
			List<JobCategory> listJobCategories=listCategories.GetListSelected<JobCategory>();
			List<long> listJobPrioritiesDefNums=listPriorities.GetListSelected<Def>().Select(x => x.DefNum).ToList();
			if(checkRemoveApprovalJobs.Checked) {
				listJobs=listJobs.Where(x => !x.IsApprovalNeeded).ToList();
			}
			if(comboTeams.SelectedIndex > 0 && comboEngineers.SelectedIndex==0) {//All engineers on one team selected.
				List<long> listUserNums=_listJobTeamUsers.Where(x => x.JobTeamNum==jobTeamSelected.JobTeamNum).Select(x => x.UserNumEngineer).ToList();
				listJobs=listJobs.Where(x	=> 
						 x.ListJobTimeLogs.Exists(y => listUserNums.Contains(y.ReviewerNum)) //Selected engineers added time to the job
					|| x.ListJobReviews.Exists(y => listUserNums.Contains(y.ReviewerNum))  //Selected engineers reviewed the job
					|| listUserNums.Contains(x.UserNumConcept)
					|| listUserNums.Contains(x.UserNumEngineer)
					|| listUserNums.Contains(x.UserNumExpert)
					|| listUserNums.Contains(x.UserNumQuoter)).ToList();
			}
			else if(selectedUser.UserNum!=-1) {//One engineer selected.
				listJobs=listJobs.Where(x =>
						 x.ListJobTimeLogs.Exists(y => y.ReviewerNum==selectedUser.UserNum) //Selected engineer added time to the job
					|| x.ListJobReviews.Exists(y => y.ReviewerNum==selectedUser.UserNum)  //Selected engineer reviewed the job
					|| x.UserNumConcept.In(selectedUser.UserNum)
					|| x.UserNumEngineer.In(selectedUser.UserNum)
					|| x.UserNumExpert.In(selectedUser.UserNum)
					|| x.UserNumQuoter.In(selectedUser.UserNum)).ToList();
			}
			if(checkRemoveNoQuote.Checked) {
				listJobs=listJobs.Where(x => x.ListJobQuotes.Count()!=0).ToList();
			}
			if(!string.IsNullOrEmpty(textPatNum.Text)) {
				listJobs=listJobs.Where(x => x.ListJobLinks.Any(y => y.LinkType==JobLinkType.Customer && y.FKey.ToString()==textPatNum.Text.Trim())
				|| x.ListJobQuotes.Any(y => y.PatNum.ToString()==textPatNum.Text.Trim())).ToList();
			}
			if(!string.IsNullOrWhiteSpace(textWithinLastXDays.Text)) {
				double withinLastXDays = PIn.Double(textWithinLastXDays.Text);
				//Jobs that were reviewed or had time logged within the last X days.
				listJobs=listJobs
					.Where(x => x.ListJobTimeLogs.Any(y => y.DateTStamp>=DateTime.Now.AddDays(-withinLastXDays)) || x.ListJobReviews.Any(y => y.DateTStamp>=DateTime.Now.AddDays(-withinLastXDays)))
					.ToList();
			}
			listJobs=listJobs.Where(x => listJobPrioritiesDefNums.Contains(x.Priority)
				&& listJobPhases.Contains(x.PhaseCur)
				&& listJobCategories.Contains(x.Category)).OrderByDescending(x => x.HoursActual).ToList();
			DateTime dateTimeFrom=dateRangeJobCompleted.GetDateTimeFrom();
			DateTime dateTimeTo=dateRangeJobCompleted.GetDateTimeTo();
			//Filter out all jobs that are completed but not between desired date range.
			if(listJobPhases.Contains(JobPhase.Complete) || listJobPhases.Contains(JobPhase.Documentation)) {
				listJobs.RemoveAll(x => (x.PhaseCur==JobPhase.Complete || x.PhaseCur==JobPhase.Documentation) && !x.DateTimeImplemented.Between(dateTimeFrom,dateTimeTo));
			}
			return listJobs;
		}

		///<summary>Refill list of Engineers to those associated to the selected team.</summary>
		private void comboTeams_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboTeams.SelectedIndex==0) {//All Teams
				FillComboEngineers(JobHelper.ListEngineerUsers);
				return;
			}
			long jobTeamNumSelected=comboTeams.GetSelected<JobTeam>().JobTeamNum;
			List<JobTeamUser> listJobTeamUsers=_listJobTeamUsers.Where(x => x.JobTeamNum==jobTeamNumSelected).ToList();
			List<long> listUserNumsEngineer=listJobTeamUsers.Select(x => x.UserNumEngineer).ToList();
			List<Userod> listUsers=JobHelper.ListEngineerUsers.FindAll(x => listUserNumsEngineer.Contains(x.UserNum));
			FillComboEngineers(listUsers);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			_listJobsFiltered=GetFilteredJobList();
			FillGridEngineerHours(_listJobsFiltered);
			FillGridJobs(_listJobsFiltered);
		}

		private void gridJobs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormJobManager.OpenNonModalJob((Job)gridJobs.ListGridRows[e.Row].Tag);
		}

		private void butPatSelect_Click(object sender,EventArgs e) {
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel) {
				return;
			}
			textPatNum.Text=frmPatientSelect.PatNumSelected.ToString();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;	
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Job List report printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text=Lan.g(this,"Job List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridJobs.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butExport_Click(object sender,System.EventArgs e) {
			using SaveFileDialog saveFileDialog=new SaveFileDialog();
			saveFileDialog.AddExtension=true;
			saveFileDialog.FileName="Jobs List";
			if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
				try {
					Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
					saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				catch {
					//initialDirectory will be blank
				}
			}
			else {
				saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			saveFileDialog.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
			saveFileDialog.FilterIndex=0;
			if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			try {
				using(StreamWriter sw=new StreamWriter(saveFileDialog.FileName,false))
				//new FileStream(,FileMode.Create,FileAccess.Write,FileShare.Read)))
				{
					String line="";
					for(int i=0;i<gridJobs.Columns.Count;i++) {
						line+=gridJobs.Columns[i].Heading+"\t";
					}
					sw.WriteLine(line);
					for(int i=0;i<gridJobs.ListGridRows.Count;i++) {
						line="";
						for(int j=0;j<gridJobs.Columns.Count;j++) {
							line+=gridJobs.ListGridRows[i].Cells[j].Text;
							if(j<gridJobs.Columns.Count-1) {
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
			}
			catch {
				MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			MessageBox.Show(Lan.g(this,"File created successfully"));
		}
		#endregion

		#region Shortcuts
		private void butHomePage_Click(object sender,EventArgs e) {
			try{
				Process.Start("https://www.opendental.com");
			}
			catch{
				MsgBox.Show(this,"Could not open Home Page");
			}
		}

		private void butForum_Click(object sender,EventArgs e) {
			try{
				Process.Start("http://opendentalsoft.com/forum/");
			}
			catch{
				MsgBox.Show(this,"Could not open Forum");
			}
		}

		private void butBugManager_Click(object sender,EventArgs e) {
			try{
				Process.Start("http://opendentalsoft.com:1942/ODBugTracker/BugList.aspx");
			}
			catch{
				MsgBox.Show(this,"Could not open Bugs Manager");
			}
		}

		private void butSchema_Click(object sender,EventArgs e) {
			try{
				Process.Start("https://www.opendental.com/OpenDentalDocumentation18-2.xml");
			}
			catch{
				MsgBox.Show(this,"Could not open Schema");
			}
		}

		private void butBugsList_Click(object sender,EventArgs e) {
			try{
				Process.Start("http://opendentalsoft.com:1942/ODBugTracker/PreviousVersions.aspx");
			}
			catch{
				MsgBox.Show(this,"Could not open Bugs List");
			}
		}

		private void butUserGroup_Click(object sender,EventArgs e) {
			try{
				Process.Start("https://www.facebook.com/groups/OpenDentalUsers/");
			}
			catch{
				MsgBox.Show(this,"Could not open User Group");
			}
		}

		private void butSchedules_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.Schedules)) {
				return;
			}
			using FormScheduleDayEdit FormSDE=new FormScheduleDayEdit(DateTime.Now,Clinics.ClinicNum);
			FormSDE.ShowOkSchedule=true;
			FormSDE.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.Schedules,0,"");
			if(FormSDE.GotoScheduleOnClose) {
				using FormSchedule FormS = new FormSchedule();
				FormS.ShowDialog();
			}
		}
		#endregion
		
		private void butFixTopParents_Click(object sender,EventArgs e) {
			List<JobShort> listJobTreeMain=Jobs.GetJobShortNoTopParent();
			while(listJobTreeMain.Count>0) {
				JobShort jobShort=listJobTreeMain.FirstOrDefault();
				List<JobShort> listJobTreeOld=new List<JobShort>();
				while(jobShort.ParentNum!=0) {
					jobShort=listJobTreeMain.FirstOrDefault(x => x.JobNum==jobShort.ParentNum);
				}
				listJobTreeOld=Jobs.GetChildJobs(jobShort,listJobTreeMain);
				listJobTreeOld.Add(jobShort);
				Jobs.UpdateTopParentList(jobShort.JobNum,listJobTreeOld.Select(x => x.JobNum).ToList());
				listJobTreeMain.RemoveAll(x => x.JobNum.In(listJobTreeOld.Select(x => x.JobNum).ToArray()));
			}
		}
	}
}