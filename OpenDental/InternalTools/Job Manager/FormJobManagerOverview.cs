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
			comboProposedVersion.Items.Add("All");
			comboProposedVersion.SelectedIndex=0;
			comboProposedVersion.Items.AddEnums<JobProposedVersion>();
			listCategories.SelectedIndex=0;
			comboEngineers.Items.Add("All",new Userod() { UserNum=-1 });
			comboEngineers.Items.Add("Unassigned",new Userod() { UserNum=0 });
			foreach(Userod eng in JobHelper.ListEngineerUsers) {
				comboEngineers.Items.Add(eng.UserName,eng);
			}
			comboEngineers.SelectedIndex=0;
			dateRangeJobCompleted.SetDateTimeFrom(VersionReleases.GetBetaDevelopmentStartDate());
			dateRangeJobCompleted.SetDateTimeTo(VersionReleases.GetBetaReleaseDate());
			SetFilters();
			FillGridJobs(GetFilteredJobList());
			FillGridSprints();
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
			gridSprints.ListGridColumns.Clear();
			gridSprints.ListGridColumns.Add(new GridColumn("Start Date", 70));
			gridSprints.ListGridColumns.Add(new GridColumn("Title", 50));
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
			gridEngineers.ListGridColumns.Clear();
			gridEngineers.ListGridColumns.Add(new GridColumn("Engineer",80){ IsWidthDynamic=true });
			gridEngineers.ListGridColumns.Add(new GridColumn("Scheduled Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.ListGridColumns.Add(new GridColumn("- Est. Breaks",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.ListGridColumns.Add(new GridColumn("= Hrs Total",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.ListGridColumns.Add(new GridColumn("Assigned Dev Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.ListGridColumns.Add(new GridColumn("Free Dev Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
			gridEngineers.ListGridColumns.Add(new GridColumn("Completed Hrs",150) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse  });
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
		private void FillGridJobs(List<Job> listJobs) {
			int totalJobs=listJobs.Count();
			int totalBugs=0;
			int totalFeatures=0;
			double totalHrsEst=0;
			double totalHrsSpent=0;
			double totalQuotedDollars=0;
			List<Def> listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			List<string> listProposedVersions=Enum.GetNames(typeof(JobProposedVersion)).ToList();
			gridJobs.BeginUpdate();
			gridJobs.ListGridColumns.Clear();
			gridJobs.ListGridColumns.Add(new GridColumn("Job",70){ IsWidthDynamic=true });
			gridJobs.ListGridColumns.Add(new GridColumn("Owner",70) { TextAlign=HorizontalAlignment.Center });
			gridJobs.ListGridColumns.Add(new GridColumn("Owner Action",90));
			gridJobs.ListGridColumns.Add(new GridColumn("Phase",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.ListGridColumns.Add(new GridColumn("Priority",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.ListGridColumns.Add(new GridColumn("Expert",75) { TextAlign=HorizontalAlignment.Center });
			gridJobs.ListGridColumns.Add(new GridColumn("Engineer",75) { TextAlign=HorizontalAlignment.Center });
			//Est. Version has a combobox, but it currently does not work.
			//TODO: Centralize a save method so job saving can be called from anywhere and perform the same functionality
			gridJobs.ListGridColumns.Add(new GridColumn("Est. Version",90) { ListDisplayStrings=listProposedVersions,TextAlign=HorizontalAlignment.Center });
			gridJobs.ListGridColumns.Add(new GridColumn("Hrs Est",90) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.ListGridColumns.Add(new GridColumn("Hrs Last 7 Days",90) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.ListGridColumns.Add(new GridColumn("Hrs Total",90) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.ListGridColumns.Add(new GridColumn("Est Completion %",110) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridJobs.ListGridRows.Clear();
			foreach(Job job in listJobs) {
				Def jobPriority = listJobPriorities.FirstOrDefault(y => y.DefNum==job.Priority);
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
				cell=new GridCell(job.ProposedVersion.ToString());
				cell.ComboSelectedIndex=listProposedVersions.IndexOf(job.ProposedVersion.ToString());
				row.Cells.Add(cell);
				row.Cells.Add(job.HoursEstimate.ToString());
				row.Cells.Add(job.ListJobTimeLogs.Where(x => x.DateTStamp>=DateTime.Today.AddDays(-7)).Sum(y => y.TimeReview.TotalHours).ToString());
				row.Cells.Add(job.HoursActual.ToString());
				row.Cells.Add(Math.Round((job.HoursActual/job.HoursEstimate*100),0).ToString());
				gridJobs.ListGridRows.Add(row);
			}
			gridJobs.EndUpdate();
			textTotalJobs.Text=totalJobs.ToString();
			textTotalHrsEst.Text=totalHrsEst.ToString();
			textTotalHrsSpent.Text=totalHrsSpent.ToString();
			textTotalQuote.Text=totalQuotedDollars.ToString();
			textTotalBugs.Text=totalBugs.ToString();
			textTotalFeatures.Text=totalFeatures.ToString();
			textCompletionPercent.Text=Math.Round(totalHrsSpent/totalHrsEst*100,0).ToString();
		}

		private void radioCurrentVersion_CheckedChanged(object sender,EventArgs e) {
			SetFilters();
		}

		private void radioNewVersion_CheckedChanged(object sender,EventArgs e) {
			SetFilters();
		}

		private void radioStaleJobs_CheckedChanged(object sender,EventArgs e) {
			SetFilters();
		}

		private void radioCustom_CheckedChanged(object sender,EventArgs e) {
			SetFilters();
		}

		///<summary>This is a very hard-coded method. We can change it when we need to though.</summary>
		private void SetFilters() {
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.JobPriorities);
			if(radioCurrentVersion.Checked) {
				listPhases.ClearSelected();
				listCategories.ClearSelected();
				listPriorities.ClearSelected();
				comboEngineers.SelectedIndex=0;
				textPatNum.Text="";
				checkRemoveApprovalJobs.Checked=false;
				checkRemoveNoQuote.Checked=false;
				List<JobPhase> listJobPhases=listPhases.Items.GetAll<JobPhase>();
				for(int i=0; i<listJobPhases.Count;i++) {
					if(ListTools.In(listJobPhases[i],
						JobPhase.Definition,
						JobPhase.Development)) 
					{
						listPhases.SetSelected(i,true);
					}
				}
				List<JobCategory> listJobCats=listCategories.Items.GetAll<JobCategory>();
				for(int i=0; i<listJobCats.Count;i++) {
					if(ListTools.In(listJobCats[i],
						JobCategory.Bug,
						JobCategory.Enhancement,
						JobCategory.Feature,
						JobCategory.HqRequest,
						JobCategory.InternalRequest,
						JobCategory.ProgramBridge,
						JobCategory.Research)) 
					{
						listCategories.SetSelected(i,true);
					}
				}
				List<Def> listDefPriorities=listPriorities.Items.GetAll<Def>();
				for(int i=0; i<listDefPriorities.Count;i++) {
					if(ListTools.In(listDefPriorities[i].DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("Urgent")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("High")).DefNum))
					{
						listPriorities.SetSelected(i,true);
					}
				}
			}
			if(radioNewVersion.Checked) {
				listPhases.ClearSelected();
				listCategories.ClearSelected();
				listPriorities.ClearSelected();
				comboEngineers.SelectedIndex=0;
				textPatNum.Text="";
				checkRemoveApprovalJobs.Checked=false;
				checkRemoveNoQuote.Checked=false;
				List<JobPhase> listJobPhases=listPhases.Items.GetAll<JobPhase>();
				for(int i=0; i<listJobPhases.Count;i++) {
					if(ListTools.In(listJobPhases[i],
						JobPhase.Concept,
						JobPhase.Quote,
						JobPhase.Definition,
						JobPhase.Development)) 
					{
						listPhases.SetSelected(i,true);
					}
				}
				List<JobCategory> listJobCats=listCategories.Items.GetAll<JobCategory>();
				for(int i=0; i<listJobCats.Count;i++) {
					if(ListTools.In(listJobCats[i],
						JobCategory.Bug,
						JobCategory.Enhancement,
						JobCategory.Feature,
						JobCategory.HqRequest,
						JobCategory.InternalRequest,
						JobCategory.ProgramBridge,
						JobCategory.Research)) 
					{
						listCategories.SetSelected(i,true);
					}
				}
				List<Def> listDefPriorities=listPriorities.Items.GetAll<Def>();
				for(int i=0; i<listDefPriorities.Count;i++) {
					if(ListTools.In(listDefPriorities[i].DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("MediumHigh")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("Normal")).DefNum))
					{
						listPriorities.SetSelected(i,true);
					}
				}
			}
			if(radioStaleJobs.Checked) {
				listPhases.ClearSelected();
				listCategories.ClearSelected();
				listPriorities.ClearSelected();
				comboEngineers.SelectedIndex=0;
				textPatNum.Text="";
				checkRemoveApprovalJobs.Checked=false;
				checkRemoveNoQuote.Checked=false;
				List<JobPhase> listJobPhases=listPhases.Items.GetAll<JobPhase>();
				for(int i=0; i<listJobPhases.Count;i++) {
					if(ListTools.In(listJobPhases[i],
						JobPhase.Concept,
						JobPhase.Quote,
						JobPhase.Definition,
						JobPhase.Development)) 
					{
						listPhases.SetSelected(i,true);
					}
				}
				List<JobCategory> listJobCats=listCategories.Items.GetAll<JobCategory>();
				for(int i=0; i<listJobCats.Count;i++) {
					if(ListTools.In(listJobCats[i],
						JobCategory.Bug,
						JobCategory.Enhancement,
						JobCategory.Feature,
						JobCategory.HqRequest,
						JobCategory.InternalRequest,
						JobCategory.ProgramBridge,
						JobCategory.Research)) 
					{
						listCategories.SetSelected(i,true);
					}
				}
				List<Def> listDefPriorities=listPriorities.Items.GetAll<Def>();
				for(int i=0; i<listDefPriorities.Count;i++) {
					if(ListTools.In(listDefPriorities[i].DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("Urgent")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("High")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("MediumHigh")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("Normal")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("Low")).DefNum,
						listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("OnHold")).DefNum))
					{
						listPriorities.SetSelected(i,true);
					}
				}
			}
			if(radioCustom.Checked) {
				//comboEngineers.SelectedIndex=0;
				//textPatNum.Text="";
				//checkRemoveApprovalJobs.Checked=false;
				//checkRemoveNoQuote.Checked=false;
				//textWeeksStale.Text="";
				//listPhases.SelectedTags<JobPhase>().Add(JobPhase.Concept);
				//listPhases.SelectedTags<JobPhase>().Add(JobPhase.Quote);
				//listPhases.SelectedTags<JobPhase>().Add(JobPhase.Definition);
				//listPhases.SelectedTags<JobPhase>().Add(JobPhase.Development);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.Bug);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.Enhancement);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.Feature);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.HqRequest);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.InternalRequest);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.ProgramBridge);
				//listCategories.SelectedTags<JobCategory>().Add(JobCategory.Research);
				//listPriorities.SetSelectedItem<Def>(x => x.DefNum.In(
				//	listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("Urgent")).DefNum,
				//	listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("High")).DefNum,
				//	listDefs.FirstOrDefault(y => y.ItemValue.Split(',').Contains("MediumHigh")).DefNum));
			}
		}

		private List<Job> GetFilteredJobList() {
			Userod selectedUser=comboEngineers.GetSelected<Userod>();
			List<Job> listJobs=JobManagerCore.ListJobsAll;
			if(checkRemoveNoQuote.Checked) {
				listJobs=listJobs.Where(x => x.ListJobQuotes.Count()!=0).ToList();
			}
			if(checkRemoveApprovalJobs.Checked) {
				listJobs=listJobs.Where(x => !x.IsApprovalNeeded).ToList();
			}
			if(selectedUser.UserNum!=-1) {//All is not selected
				listJobs = listJobs.Where(x => (ListTools.In(x.UserNumConcept,selectedUser.UserNum)
					|| ListTools.In(x.UserNumEngineer,selectedUser.UserNum)
					|| ListTools.In(x.UserNumExpert,selectedUser.UserNum)
					|| ListTools.In(x.UserNumQuoter,selectedUser.UserNum))).ToList();
			}
			if(!string.IsNullOrEmpty(textPatNum.Text)) {
				listJobs=listJobs.Where(x => x.ListJobLinks.Any(y => y.LinkType==JobLinkType.Customer && y.FKey.ToString()==textPatNum.Text.Trim())
				|| x.ListJobQuotes.Any(y => y.PatNum.ToString()==textPatNum.Text.Trim())).ToList();
			}
			//TODO Make this work -- maybe add a dateimplemented column
			//if(dateRangeJobCompleted.GetDateTimeFrom()!=DateTime.MinValue || dateRangeJobCompleted.GetDateTimeTo()!=DateTime.MinValue) {
			//	listJobs=listJobs.RemoveAll(x => x.PhaseCur==JobPhase.Completed && x.ListJobLogs.
			//}
			if (comboProposedVersion.SelectedIndex>0) {
				listJobs=listJobs.FindAll(x => x.ProposedVersion==comboProposedVersion.GetSelected<JobProposedVersion>());
			}
			return listJobs=listJobs.Where(x => ListTools.In(x.Priority,listPriorities.GetListSelected<Def>().Select(y => y.DefNum))
					&& ListTools.In(x.PhaseCur,listPhases.GetListSelected<JobPhase>())
					&& ListTools.In(x.Category,listCategories.GetListSelected<JobCategory>())).OrderByDescending(x => x.HoursActual).ToList();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGridJobs(GetFilteredJobList());
		}

		private void gridJobs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormJobManager.OpenNonModalJob((Job)gridJobs.ListGridRows[e.Row].Tag);
		}

		private void butPatSelect_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.SelectionModeOnly=true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			textPatNum.Text=FormPS.SelectedPatNum.ToString();
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
					for(int i=0;i<gridJobs.ListGridColumns.Count;i++) {
						line+=gridJobs.ListGridColumns[i].Heading+"\t";
					}
					sw.WriteLine(line);
					for(int i=0;i<gridJobs.ListGridRows.Count;i++) {
						line="";
						for(int j=0;j<gridJobs.ListGridColumns.Count;j++) {
							line+=gridJobs.ListGridRows[i].Cells[j].Text;
							if(j<gridJobs.ListGridColumns.Count-1) {
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
			if(!Security.IsAuthorized(Permissions.Schedules)) {
				return;
			}
			using FormScheduleDayEdit FormSDE=new FormScheduleDayEdit(DateTime.Now,Clinics.ClinicNum);
			FormSDE.ShowOkSchedule=true;
			FormSDE.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Schedules,0,"");
			if(FormSDE.GotoScheduleOnClose) {
				using FormSchedule FormS = new FormSchedule();
				FormS.ShowDialog();
			}
		}
		#endregion
	}
}