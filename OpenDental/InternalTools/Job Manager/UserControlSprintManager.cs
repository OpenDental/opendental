using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class UserControlSprintManager:UserControl {
		private List<long> _listEngEmpNums = new List<long>() {15,34,36,64,72,74,88,94,118,121,163,173,177,179,253,257,299};
		private List<Schedule> _listSchedules=new List<Schedule>();
		///<summary>Managed list of JobSprintLinks. Gets filled on LoadSprint and then added and removed from thereafter.</summary>
		private List<JobSprintLink> _listJobSprintLinks=new List<JobSprintLink>();
		private JobSprint _jobSprintCur=new JobSprint();
		private List<Job> _listAttachedJobs=new List<Job>();
		private List<Def> _listJobPriorities;
		private bool _hasUnsavedChanges=false;

		///<summary>Occurs whenever this control saves changes to DB, after the control has redrawn itself. 
		/// Usually connected to either a form close or refresh.</summary>
		[Category("Action"),Description("Whenever this control saves changes to DB, after the control has redrawn itself. Usually connected to either a form close or refresh.")]
		public event EventHandler SaveClick=null;

		public UserControlSprintManager() {
			InitializeComponent();
			if(_jobSprintCur.JobSprintNum==0) {
				Enabled=false;
			}
		}

		public JobSprint JobSprintCur {
			get {
				return _jobSprintCur;
			}
		}

		public void LoadSprint(JobSprint jobSprint) {
			if(_hasUnsavedChanges) {
				switch(MessageBox.Show("The current sprint has unsaved changes. Would you like to save them?","",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Yes:
						if(!SaveJobSprint()) {
							return;
						}
						break;
					case DialogResult.No:
						_hasUnsavedChanges=false;
						break;
					case DialogResult.Cancel:
						return;
					default:
						return;
				}
			}
			//If this fails then the JM should also be failing
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			_jobSprintCur=jobSprint;
			_listJobSprintLinks=JobSprintLinks.GetForSprint(_jobSprintCur.JobSprintNum);
			textTitle.Text=jobSprint.Title;
			textNote.Text=jobSprint.Note;
			textDateStart.Text=jobSprint.DateStart.ToShortDateString();
			textDateEnd.Text=jobSprint.DateEndTarget.ToShortDateString();
			textDateEndActual.Text=jobSprint.DateEndActual.ToShortDateString();
			if(jobSprint.DateEndActual==DateTime.MinValue) {
				labelDateEndActual.Visible=false;
				textDateEndActual.Visible=false;
			}
			else {
				labelDateEndActual.Visible=true;
				textDateEndActual.Visible=true;
			}
			textEngJobPercent.Text=jobSprint.JobPercent.ToString();
			textAvgDevelopmentHours.Text=jobSprint.HoursAverageDevelopment.ToString();
			textBreakHours.Text=jobSprint.HoursAverageBreak.ToString();
			//Reset _hasUnsavedChanges since setting the text cause it to set itself to true.
			_hasUnsavedChanges=false;
			FillGridJobs();
			Enabled=true;
		}

		public bool UnloadSprint(out JobSprint jobSprint) {
			jobSprint=_jobSprintCur;
			if(_hasUnsavedChanges) {
				switch(MessageBox.Show("The current sprint has unsaved changes. Would you like to save them?","",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.Yes:
						if(!SaveJobSprint()) {
							//Reset jobSprint to get the new changes
							jobSprint=_jobSprintCur;
							Enabled=false;
							return true;
						}
						break;
					case DialogResult.No:
						Enabled=false;
						_hasUnsavedChanges=false;
						return true;
					case DialogResult.Cancel:
						return false;
					default:
						return false;
				}
			}
			return true;
		}

		private void FillGridJobs() {
			gridSprintJobs.BeginUpdate();
			gridSprintJobs.ListGridColumns.Clear();
			gridSprintJobs.ListGridColumns.Add(new GridColumn("Priority",90,HorizontalAlignment.Center));
			gridSprintJobs.ListGridColumns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));
			gridSprintJobs.ListGridColumns.Add(new GridColumn("Owner Action",110));
			gridSprintJobs.ListGridColumns.Add(new GridColumn("EstHrs",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridSprintJobs.ListGridColumns.Add(new GridColumn("ActHrs",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridSprintJobs.ListGridColumns.Add(new GridColumn("",300));
			gridSprintJobs.ListGridRows.Clear();
			List<long> listLinkedJobNums=_listJobSprintLinks.Select(x => x.JobNum).ToList();
			_listAttachedJobs=JobManagerCore.ListJobsAll.Where(x => listLinkedJobNums.Contains(x.JobNum)).ToList();
			_listAttachedJobs=_listAttachedJobs.OrderBy(x => x.OwnerNum!=0)
					//This is the reverse order of the actual priority of different categories of jobs
					//Purposefully put in this order so they appear correctly in the list.
					.ThenBy(x => x.Category==JobCategory.NeedNoApproval)
					.ThenBy(x => x.Category==JobCategory.Research)
					.ThenBy(x => x.Category==JobCategory.Conversion)
					.ThenBy(x => x.Category==JobCategory.UnresolvedIssue)
					.ThenBy(x => x.Category==JobCategory.HqRequest)
					.ThenBy(x => x.Category==JobCategory.InternalRequest)
					.ThenBy(x => x.Category==JobCategory.Feature)
					.ThenBy(x => x.Category==JobCategory.Query)
					.ThenBy(x => x.Category==JobCategory.ProgramBridge)
					.ThenBy(x => x.Category==JobCategory.Enhancement)
					.ThenBy(x => x.Category==JobCategory.Bug)
					.ThenBy(x => _listJobPriorities.FirstOrDefault(y => y.DefNum==x.Priority).ItemOrder).ToList();
			Dictionary<JobPhase,List<Job>> dictPhases=new Dictionary<JobPhase,List<Job>>();
			foreach(Job job in _listAttachedJobs) {
				JobPhase phase=job.PhaseCur;
				if(!dictPhases.ContainsKey(phase)) {
					dictPhases[phase]=new List<Job>();
				}
				dictPhases[phase].Add(job);
			}
			//sort dictionary so actions will appear in same order
			//This is in reverse order in the code so it is correct in the UI
			dictPhases=dictPhases.OrderBy(x => x.Key==JobPhase.Cancelled)
					.ThenBy(x => x.Key==JobPhase.Complete)
					.ThenBy(x => x.Key==JobPhase.Development)
					.ThenBy(x => x.Key==JobPhase.Quote)
					.ThenBy(x => x.Key==JobPhase.Definition)
					.ThenBy(x => x.Key==JobPhase.Concept).ToDictionary(x => x.Key,x => x.Value);
			foreach(KeyValuePair<JobPhase,List<Job>> kvp in dictPhases) {
				if(_listAttachedJobs.Count==0) {
					continue;
				}
				gridSprintJobs.ListGridRows.Add(new GridRow("","","","","",kvp.Key.ToString()) { ColorBackG=Color.FromArgb(223,234,245),Bold=true });
				foreach(Job job in kvp.Value) {
					Color backColor=Color.White;
					Def jobPriority=_listJobPriorities.FirstOrDefault(y => y.DefNum==job.Priority);
					gridSprintJobs.ListGridRows.Add(
					new GridRow(
						new GridCell(jobPriority.ItemName) {
							ColorBackG=jobPriority.ItemColor,
							ColorText=(job.Priority==_listJobPriorities.FirstOrDefault(y => y.ItemValue.Contains("Urgent")).DefNum) ? Color.White : Color.Black,
						},
						new GridCell(job.OwnerNum==0 ? "-" : Userods.GetName(job.OwnerNum)),
						new GridCell(job.OwnerAction.GetDescription()),
						new GridCell(job.HoursEstimate.ToString()),
						new GridCell(job.HoursActual.ToString()),
						new GridCell(job.ToString())
						) {
						Tag=job,
						ColorBackG=backColor
					}
					);
				}
			}
			gridSprintJobs.EndUpdate();
			RecalculateSprint();
		}

		private void butSave_Click(object sender,EventArgs e) {
			SaveJobSprint();
		}

		private bool SaveJobSprint() {
			if(!textDateStart.IsValid()
				|| !textDateEnd.IsValid()
				|| !textEngJobPercent.IsValid()
				|| !textAvgDevelopmentHours.IsValid()
				|| !textBreakHours.IsValid())
			{
				MsgBox.Show(this,"Please fix sprint settings before saving.");
				return false;
			}
			_jobSprintCur.Title=textTitle.Text;
			_jobSprintCur.DateStart=PIn.Date(textDateStart.Text);
			_jobSprintCur.DateEndTarget=PIn.Date(textDateEnd.Text);
			_jobSprintCur.JobPercent=PIn.Double(textEngJobPercent.Text);
			_jobSprintCur.HoursAverageDevelopment=PIn.Double(textAvgDevelopmentHours.Text);
			_jobSprintCur.HoursAverageBreak=PIn.Double(textBreakHours.Text);
			_jobSprintCur.Note=textNote.Text;
			JobSprints.Update(_jobSprintCur);
			_hasUnsavedChanges=false;
			if(SaveClick!=null) {
				SaveClick(this,new EventArgs());
			}
			return true;
		}

		private void textTitle_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
		}

		private void textNote_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
		}

		private void dateStart_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
			RecalculateSprint(true);
		}

		private void dateEnd_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
			RecalculateSprint(true);
		}

		private void textEngJobPercent_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
			RecalculateSprint();
		}

		private void textAvgJobHours_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
			RecalculateSprint();
		}

		private void textBreakHours_TextChanged(object sender,EventArgs e) {
			_hasUnsavedChanges=true;
			RecalculateSprint();
		}

		private void checkProgressMode_CheckedChanged(object sender,EventArgs e) {
			RecalculateSprint(true);
		}

		///<summary>Calculates the info for sprints</summary>
		private void RecalculateSprint(bool hasDateChanges=false) {
			DateTime dateStart=PIn.Date(textDateStart.Text);//Min date if parse fails
			if(checkProgressMode.Checked) {
				dateStart=DateTime.Today.Date;
			}
			DateTime dateEnd=PIn.Date(textDateEnd.Text);//Min date if parse fails
			if(dateStart>dateEnd) {
				return;
			}
			if(dateStart!=DateTime.MinValue && dateEnd!=DateTime.MinValue && hasDateChanges) {
				_listSchedules=Schedules.RefreshPeriodForEmps(dateStart,dateEnd,_listEngEmpNums);
			}
			double jobPercent=PIn.Double(textEngJobPercent.Text);//0 if parse fails
			double avgDevHours=PIn.Double(textAvgDevelopmentHours.Text);//0 if parse fails
			double avgBreakHours=PIn.Double(textBreakHours.Text);//0 if parse fails
			double schedHoursTotal=0;
			double schedHoursBreaksTotal=0;
			double schedHoursPercentTotal=0;
			double allocatableHours=0;
			foreach(Schedule sched in _listSchedules) {
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
			foreach(Job job in _listAttachedJobs) {
				if(checkProgressMode.Checked && ListTools.In(job.PhaseCur,JobPhase.Documentation,JobPhase.Complete,JobPhase.Cancelled)) {
					continue;
				}
				double estimate=job.HoursEstimate;
				if(job.HoursEstimateDevelopment==0) {
					estimate+=avgDevHours;
				}
				totalAllocatedHours+=estimate;
				if(checkProgressMode.Checked) {
					totalAllocatedHours-=job.HoursActual;
				}
				totalCompletedHours+=job.HoursActual;
			}
			textMaxHours.Text=Math.Round(allocatableHours,0).ToString();
			//Set maximum progress bar to allocatable hours
			progressBarAllocatedHours.Maximum=PIn.Int(textMaxHours.Text);
			textAllocatedHours.Text=Math.Round(totalAllocatedHours,0).ToString();
			//Set value of progress bar to allocated hours
			progressBarAllocatedHours.Value=(PIn.Int(textAllocatedHours.Text)>progressBarAllocatedHours.Maximum)?progressBarAllocatedHours.Maximum:PIn.Int(textAllocatedHours.Text);
			//Update info
			textEngHours.Text=Math.Round(schedHoursTotal).ToString();
			textAfterBreak.Text=Math.Round(schedHoursBreaksTotal).ToString();
			textRatioHours.Text=Math.Round(schedHoursPercentTotal).ToString();
			textJobNumber.Text=_listAttachedJobs.Count.ToString();
			int days=(dateEnd-dateStart).Days;
			textDays.Text=days.ToString();  
			textAvgAllocatableHours.Text=Math.Round(allocatableHours/days).ToString();
			double completionPercentage=0;
			completionPercentage=(totalCompletedHours/totalAllocatedHours)*100;
			if(Double.IsNaN(completionPercentage)) {
				return;
			}
			completionPercentage=completionPercentage>100?100:completionPercentage;
			textCompletionPercentage.Text=Math.Round(completionPercentage).ToString();
			progressBarCompletionPercent.Value=(int)completionPercentage;
		}

		private void gridSprintJobs_TitleAddClick(object sender,EventArgs e) {
			using FormJobSearch FormJS = new FormJobSearch(false);
			FormJS.ShowDialog();
			if(FormJS.DialogResult!=DialogResult.OK) {
				return;
			}
			Job job=FormJS.SelectedJob;
			if(_listJobSprintLinks.Exists(x => x.JobNum==job.JobNum)) {
				return;
			}
			JobSprintLink sprintLink = new JobSprintLink();
			sprintLink.JobNum=job.JobNum;
			sprintLink.JobSprintNum=_jobSprintCur.JobSprintNum;
			JobSprintLinks.Insert(sprintLink);
			_listJobSprintLinks.Add(sprintLink);
			FillGridJobs();
		}

		private void gridSprintJobs_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			FormJobManager.OpenNonModalJob((Job)gridSprintJobs.ListGridRows[e.Row].Tag);
		}

		private void gridSprintJobs_MouseClick(object sender,MouseEventArgs e) {
			if(e.Button!=MouseButtons.Right) {
				return;
			}
			ContextMenu menu=new ContextMenu();
			menu.MenuItems.Add("Remove Job",(o,arg) => {
				int selectedIndex=gridSprintJobs.GetSelectedIndex();
				if(selectedIndex==-1) {
					return;//Nothing to remove.
				}
				long jobNum=((Job)gridSprintJobs.ListGridRows[selectedIndex].Tag).JobNum;
				_listJobSprintLinks.RemoveAll(x => x.JobNum==jobNum);
				JobSprintLinks.DeleteForJobAndSprint(jobNum,_jobSprintCur.JobSprintNum);
				FillGridJobs();
			});
			menu.Show(gridSprintJobs,gridSprintJobs.PointToClient(Cursor.Position));
		}

		private void butDashboard_Click(object sender,EventArgs e) {
			//FormSprintManagerDashboard FormSMD=new FormSprintManagerDashboard(JobManagerCore.ListJobsAll,_jobSprintCur);
			//FormSMD.Show(this);
		}
	}
}
