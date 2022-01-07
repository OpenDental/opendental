using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.ComponentModel;

namespace OpenDental {
	public partial class FormSprintManagerDashboard:FormODBase {
		private List<Job> _listJobsAll;
		private JobSprint _jobSprintCur;
		///<summary>Managed list of JobSprintLinks. Gets filled on LoadSprint and then added and removed from thereafter.</summary>
		private List<JobSprintLink> _listJobSprintLinks=new List<JobSprintLink>();
		private List<Job> _listAttachedJobs=new List<Job>();
		private List<Def> _listJobPriorities;
		private List<JobLog> _listJobLogs;
		private Bitmap pieChartRemaining;
		private Bitmap pieChartCompleted;

		public FormSprintManagerDashboard(List<Job> listJobsAll,JobSprint sprint) {
			_listJobsAll=listJobsAll;
			_jobSprintCur=sprint;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormJobManagerDashboard_Load(object sender,EventArgs e) {
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
			_listJobLogs=JobLogs.GetJobLogsForJobs(_listJobsAll.Select(x => x.JobNum).ToList());
			RefreshAll();
			timerScrollGrid.Start();
			timerRefreshAll.Start();
		}

		///<summary>Calculates the info for the dashboard</summary>
		private void RefreshAll() {
			//Validation
			if(_jobSprintCur.DateStart>_jobSprintCur.DateEndTarget) {
				return;
			}
			#region Variables
			_listJobSprintLinks=JobSprintLinks.GetForSprint(_jobSprintCur.JobSprintNum);
			_listAttachedJobs=_listJobsAll.FindAll(x => _listJobSprintLinks.Select(y => y.JobNum).Contains(x.JobNum)
				&& ListTools.In(x.Category,JobCategory.Feature,JobCategory.Enhancement,JobCategory.InternalRequest,JobCategory.HqRequest,JobCategory.ProgramBridge));
			List<Job> listBugJobs=_listJobsAll.FindAll(x => x.Category==JobCategory.Bug);
			DateTime dateStart=_jobSprintCur.DateStart;
			DateTime dateProgress=DateTime.Today.Date;
			DateTime dateEnd=_jobSprintCur.DateEndTarget;
			List<Job> listCompletedBugJobs=listBugJobs.FindAll(x => ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)
				&& x.ListJobLinks.Count(y => y.LinkType==JobLinkType.Bug)>0
				&& DateTimeOD.Between(_listJobLogs.FirstOrDefault(y => y.JobNum==x.JobNum && y.Description.Contains("Job implemented"))?.DateTimeEntry??DateTime.MinValue,dateStart,dateEnd));
			List<Job> listRemainingBugJobs=listBugJobs.FindAll(x => ListTools.In(x.PhaseCur,
				JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)
				&& x.ListJobLinks.Count(y => y.LinkType==JobLinkType.Bug)>0);
			double totalDays=(dateEnd - dateStart).TotalDays;
			double totalDaysElapsed=(dateProgress - dateStart).TotalDays;
			double totalDaysLeft=totalDays-totalDaysElapsed;
			int targetValuePercent=(int)(totalDaysElapsed/totalDays*100);
			double jobPercent=_jobSprintCur.JobPercent;
			double avgDevHours=_jobSprintCur.HoursAverageDevelopment;
			double avgBreakHours=_jobSprintCur.HoursAverageBreak;
			double totalAllocatedHours=0;
			double totalCompletedHours=0;
			double hoursComplete=_listAttachedJobs.Sum(x => x.HoursActual);
			double hoursTotal=_listAttachedJobs.Sum(x => x.HoursEstimate);
			double hoursRemaining=hoursTotal-hoursComplete>0?hoursTotal-hoursComplete:0;
			double totalJobsRemaining=_listAttachedJobs.Where(x => ListTools.In(x.PhaseCur,
				JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)).Count()+listRemainingBugJobs.Count;
			double totalJobsCompleted=_listAttachedJobs.Where(x => ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)).Count()+listCompletedBugJobs.Count();
			#endregion
			#region Top Panel
			this.Text=_jobSprintCur.Title;
			textStartDate.Text=_jobSprintCur.DateStart.ToShortDateString();
			textEndDate.Text=_jobSprintCur.DateEndTarget.ToShortDateString();
			//Calculate hours for the list of jobs attached to the sprint
			foreach(Job job in _listAttachedJobs) {
				//Do not include "completed" jobs
				if(ListTools.In(job.PhaseCur,JobPhase.Documentation,JobPhase.Complete,JobPhase.Cancelled)) {
					continue;
				}
				double estimate=job.HoursEstimate;
				if(job.HoursEstimateDevelopment==0) {
					estimate+=avgDevHours;
				}
				totalAllocatedHours+=estimate;
				totalAllocatedHours-=job.HoursActual;
				totalCompletedHours+=job.HoursActual;
			}
			//Calculate and Set Work Complete Percent
			double completionPercentage=0;
			completionPercentage=(totalCompletedHours/totalAllocatedHours)*100;
			if(Double.IsNaN(completionPercentage)) {
				completionPercentage=0;
			}
			completionPercentage=completionPercentage>=100?100:completionPercentage;
			//Set Progress Bar Value and TargetValue
			textCompletePercentage.Text=(int)completionPercentage+"%";
			progressComplete.Value=(int)completionPercentage;
			progressComplete.TargetValue=(int)completionPercentage;
			textElapsedPercentage.Text=targetValuePercent+"%";
			progressElapsed.Value=targetValuePercent;
			progressElapsed.TargetValue=targetValuePercent;
			//Set Days Left
			textDaysLeft.Text=totalDaysLeft+" Days Left";
			//textScopeChange.Text=(_listAttachedJobs.Where(x => x.ListJobLogs.Exists(y => y.Description.Contains("Changes approved."))).Count()/_listAttachedJobs.Count*100)+"%";
			#endregion
			#region Left Panel
			//Set all completed job percentages
			List<double> listCompletedPercents=new List<double>();
			listCompletedPercents.Add(Math.Round((listCompletedBugJobs.Count()/totalJobsCompleted*100),2));
			textCompletedBugsForRange.Text=listCompletedPercents.Last().ToString("0.00")+"%";
			listCompletedPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.Feature 
				&& ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)).Count()/totalJobsCompleted*100),2));
			textCompletedFeaturesForRange.Text=listCompletedPercents.Last().ToString("0.00")+"%";
			listCompletedPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.Enhancement 
				&& ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)).Count()/totalJobsCompleted*100),2));
			textCompletedEnhancementsForRange.Text=listCompletedPercents.Last().ToString("0.00")+"%";
			listCompletedPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.InternalRequest 
				&& ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)).Count()/totalJobsCompleted*100),2));
			textCompletedInternalRequestsForRange.Text=listCompletedPercents.Last().ToString("0.00")+"%";
			listCompletedPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.HqRequest 
				&& ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)).Count()/totalJobsCompleted*100),2));
			textCompletedHQRequestsForRange.Text=listCompletedPercents.Last().ToString("0.00")+"%";
			listCompletedPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.ProgramBridge 
				&& ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)).Count()/totalJobsCompleted*100),2));
			textCompletedBridgesForRange.Text=listCompletedPercents.Last().ToString("0.00")+"%";
			DrawCompletedPieChart(listCompletedPercents);
			textJobsComplete.Text=totalJobsCompleted.ToString();
			#endregion
			#region Right Panel
			//Set all remaining job percentages
			List<double> listRemainingPercents=new List<double>();
			listRemainingPercents.Add(Math.Round((listRemainingBugJobs.Count()/totalJobsRemaining*100),2));
			textRemainingBugPercent.Text=listRemainingPercents.Last().ToString("0.00")+"%";
			listRemainingPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.Feature 
				&& ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)).Count()/totalJobsRemaining*100),2));
			textRemainingFeaturePercent.Text=listRemainingPercents.Last().ToString("0.00")+"%";
			listRemainingPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.Enhancement 
				&& ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)).Count()/totalJobsRemaining*100),2));
			textRemainingEnhancementPercent.Text=listRemainingPercents.Last().ToString("0.00")+"%";
			listRemainingPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.InternalRequest 
				&& ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)).Count()/totalJobsRemaining*100),2));
			textRemainingInternalRequestPercent.Text=listRemainingPercents.Last().ToString("0.00")+"%";
			listRemainingPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.HqRequest 
				&& ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)).Count()/totalJobsRemaining*100),2));
			textRemainingHQRequestPercent.Text=listRemainingPercents.Last().ToString("0.00")+"%";
			listRemainingPercents.Add(Math.Round((_listAttachedJobs.Where(x => x.Category==JobCategory.ProgramBridge 
				&& ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)).Count()/totalJobsRemaining*100),2));
			textRemainingBridgePercent.Text=listRemainingPercents.Last().ToString("0.00")+"%";
			DrawRemainingPieChart(listRemainingPercents);
			textJobsRemaining.Text=(totalJobsRemaining-listRemainingBugJobs.Count).ToString();//Don't count bugs here
			#endregion
			#region Bottom Panel
			List<Job> listStaleBugs=listRemainingBugJobs.FindAll(x => x.ListJobTimeLogs.Count>0?x.ListJobTimeLogs.Select(y => y.DateTStamp).Max()<DateTime.Today.AddDays(-5):x.DateTimeEntry<DateTime.Today.AddDays(-5))
				.OrderBy(x => x.ListJobTimeLogs.Count>0?x.ListJobTimeLogs.Select(y => y.DateTStamp).Max():x.DateTimeEntry).ToList();
			gridBugJobs.Title=listRemainingBugJobs.Count()+" Bugs ("+listStaleBugs.Count()+" Stale)";
			FillGridOldBugs(listStaleBugs);
			#endregion
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(!listSignals.Exists(x => x.IType==InvalidType.Defs)) {
				return;//no job signals;
			}
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
		}

		#region FillGrids
		///<summary>Fills with bug jobs have not had a timelog in the last 5 days. Ordered by last non-viewed log date.</summary>
		private void FillGridOldBugs(List<Job> listStaleBugJobs) {
			gridBugJobs.BeginUpdate();
			gridBugJobs.ListGridColumns.Clear();
			gridBugJobs.ListGridColumns.Add(new GridColumn("Last Updated",200,HorizontalAlignment.Center));
			gridBugJobs.ListGridColumns.Add(new GridColumn("",100){ IsWidthDynamic=true });
			gridBugJobs.ListGridRows.Clear();
			foreach(Job job in listStaleBugJobs) {
				gridBugJobs.ListGridRows.Add(
				new GridRow(
					new GridCell(job.ListJobTimeLogs.Count>0 ? job.ListJobTimeLogs.Max(x => x.DateTStamp).ToShortDateString() : job.DateTimeEntry.ToShortDateString()),
					new GridCell(job.ToString())
					) {
					Tag=job
				}
				);
			}
			gridBugJobs.EndUpdate();
		}

		///<summary>Fills with jobs that are attached to the selected sprint and are not complete. Ordered by owner and then category.</summary>
		private void FillGridQueue() {
			gridSprintQueue.BeginUpdate();
			gridSprintQueue.ListGridColumns.Clear();
			gridSprintQueue.ListGridColumns.Add(new GridColumn("Priority",90,HorizontalAlignment.Center));
			gridSprintQueue.ListGridColumns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));
			gridSprintQueue.ListGridColumns.Add(new GridColumn("Owner Action",110));
			gridSprintQueue.ListGridColumns.Add(new GridColumn("EstHrs",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridSprintQueue.ListGridColumns.Add(new GridColumn("ActHrs",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridSprintQueue.ListGridColumns.Add(new GridColumn("",300));
			gridSprintQueue.ListGridRows.Clear();
			List<Job> listQueue=_listAttachedJobs.Where(x => ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote)).ToList();
			listQueue=listQueue.OrderBy(x => x.OwnerNum!=0)
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
					.ThenBy(x => x.Category==JobCategory.MarketingDesign)
					.ThenBy(x => x.Category==JobCategory.ProgramBridge)
					.ThenBy(x => x.Category==JobCategory.Enhancement)
					.ThenBy(x => x.Category==JobCategory.Bug)
					.ThenBy(x => _listJobPriorities.FirstOrDefault(y => y.DefNum==x.Priority).ItemOrder).ToList();
			Dictionary<JobPhase,List<Job>> dictPhases=new Dictionary<JobPhase,List<Job>>();
			foreach(Job job in listQueue) {
				JobPhase phase=job.PhaseCur;
				if(!dictPhases.ContainsKey(phase)) {
					dictPhases[phase]=new List<Job>();
				}
				dictPhases[phase].Add(job);
			}
			//sort dictionary so actions will appear in same order
			//This is in reverse order in the code so it is correct in the UI
			dictPhases=dictPhases.OrderBy(x => x.Key==JobPhase.Concept)
					.ThenBy(x => x.Key==JobPhase.Quote)
					.ThenBy(x => x.Key==JobPhase.Definition).ToDictionary(x => x.Key,x => x.Value);
			foreach(KeyValuePair<JobPhase,List<Job>> kvp in dictPhases) {
				if(listQueue.Count==0) {
					continue;
				}
				gridSprintQueue.ListGridRows.Add(new GridRow("","","","","",kvp.Key.ToString()) { ColorBackG=Color.FromArgb(223,234,245),Bold=true });
				foreach(Job job in kvp.Value) {
					Color backColor=Color.White;
					Def jobPriority=_listJobPriorities.FirstOrDefault(y => y.DefNum==job.Priority);
					gridSprintQueue.ListGridRows.Add(
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
			gridSprintQueue.EndUpdate();
		}

		///<summary>Fills with jobs that are in development and attached to the selected sprint. Ordered by last timelog.</summary>
		private void FillGridActiveDevelopment() {
			gridActiveDevelopment.BeginUpdate();
			gridActiveDevelopment.ListGridColumns.Clear();
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("Priority",90,HorizontalAlignment.Center));
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("Last Updated",90,HorizontalAlignment.Center));
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("Owner",55,HorizontalAlignment.Center));
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("Owner Action",110));
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("EstHrs",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("ActHrs",60,HorizontalAlignment.Center,GridSortingStrategy.AmountParse));
			gridActiveDevelopment.ListGridColumns.Add(new GridColumn("",300));
			gridActiveDevelopment.ListGridRows.Clear();
			List<Job> listActiveJobs=_listAttachedJobs.Where(x => x.PhaseCur==JobPhase.Development).OrderByDescending(x => x.ListJobTimeLogs.Count>0?x.ListJobTimeLogs.Select(y => y.DateTStamp).Max():DateTime.MinValue)
					.ThenBy(x => _listJobPriorities.FirstOrDefault(y => y.DefNum==x.Priority).ItemOrder).ToList();
			foreach(Job job in listActiveJobs) {
				Color backColor=Color.White;
				Def jobPriority=_listJobPriorities.FirstOrDefault(y => y.DefNum==job.Priority);
				gridActiveDevelopment.ListGridRows.Add(
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
			gridActiveDevelopment.EndUpdate();
		}
		#endregion

		#region PieChart Drawing
		
		private void panelCompletedPieChart_Paint(object sender,PaintEventArgs e) {
			if(pieChartCompleted==null) {
				return;
			}
			e.Graphics.DrawImage(pieChartCompleted,0,0);
		}

		private void panelRemainingPieChart_Paint(object sender,PaintEventArgs e) {
			if(pieChartRemaining==null) {
				return;
			}
			e.Graphics.DrawImage(pieChartRemaining,0,0);
		}

		///<summary></summary>
		public void DrawCompletedPieChart(List<double> listPercents) {
			List<Color> listColors=new List<Color>() {
				pictureCompletedBugsColor.BackColor,
				pictureCompletedFeaturesColor.BackColor,
				pictureCompletedEnhancementsColor.BackColor,
				pictureCompletedInternalRequestsColor.BackColor,
				pictureCompletedHQRequestsColor.BackColor,
				pictureCompletedBridgesColor.BackColor,
			};
			if(pieChartCompleted!=null) {
				pieChartCompleted.Dispose();
			}
			pieChartCompleted=new Bitmap(panelCompletedPieChart.Width,panelCompletedPieChart.Height);
			using(Graphics g=Graphics.FromImage(pieChartCompleted)) {
				//Hardcoded location and size
				Point location=new Point(10,10);
				Size size = new Size(400,400);
				DrawPieChart(listPercents,listColors,g,location,size);
			}
		}

		///<summary></summary>
		public void DrawRemainingPieChart(List<double> listPercents) {
			List<Color> listColors=new List<Color>() {
				pictureRemainingBugsColor.BackColor,
				pictureRemainingFeaturesColor.BackColor,
				pictureRemainingEnhancementsColor.BackColor,
				pictureRemainingInternalRequestsColor.BackColor,
				pictureRemainingHQRequestsColor.BackColor,
				pictureRemainingBridgesColor.BackColor,
			};
			if(pieChartRemaining!=null) {
				pieChartRemaining.Dispose();
			}
			pieChartRemaining=new Bitmap(panelRemainingPieChart.Width,panelRemainingPieChart.Height);
			using(Graphics g=Graphics.FromImage(pieChartRemaining)) {
				//Hardcoded location and size
				Point location=new Point(10,10);
				Size size = new Size(400,400);
				DrawPieChart(listPercents,listColors,g,location,size);
			}
		}

		public void DrawPieChart(List<double> listPercents,List<Color> listColors,Graphics g,Point location,Size size) {
			//Make sure the lists are the same count. Shouldn't happen
			if(listPercents.Count!=listColors.Count) {
				return;
			}
			//Check if sections add up to 100.
			if(listPercents.Sum()>100) {
				//Fudge the highest percent
				listPercents[listPercents.IndexOf(listPercents.Max())]=listPercents.Max()-(listPercents.Sum()-100);
			}
			if(listPercents.Sum()<100) {
				//Fudge the highest percent
				listPercents[listPercents.IndexOf(listPercents.Max())]=listPercents.Max()+(listPercents.Sum()-100);
			}
			double percentUsed=0;
			for(int i=0;i<listPercents.Count;i++) {
				using(SolidBrush brush = new SolidBrush(listColors[i])) {
					g.FillPie(brush,new Rectangle(new Point(10,10),size),Convert.ToSingle(percentUsed*360/100),Convert.ToSingle(listPercents[i]*360/100));
				}
				percentUsed += listPercents[i];
			}
			return;
		}
		#endregion

		#region Timers

		private void timerRefreshAll_Tick(object sender,EventArgs e) {
			timerRefreshAll.Stop();
			_jobSprintCur=JobSprints.GetOne(_jobSprintCur.JobSprintNum);//Get the latest version from the database
			RefreshAll();
			timerRefreshAll.Start();
		}

		private void timerScrollGrid_Tick(object sender,EventArgs e) {
			timerScrollGrid.Stop();
			int scrollValue=gridBugJobs.ScrollValue;
			gridBugJobs.ScrollValue+=gridBugJobs.ScrollSmallChange;
			//Check if we are at the bottom by checking to see if the scrollvalue changed
			if(scrollValue==gridBugJobs.ScrollValue) {
				//Scroll back to the top since we are at the bottom
				gridBugJobs.ScrollToTop();
			}
			timerScrollGrid.Start();
		}

		private void butToggleScrollGrid_Click(object sender,EventArgs e) {
			if(butToggleScrollGrid.Text=="Pause Cycle") {
				butToggleScrollGrid.Text="Resume Cycle";
				timerScrollGrid.Stop();
			}
			else {
				butToggleScrollGrid.Text="Pause Cycle";
				timerScrollGrid.Start();
			}
		}

		#endregion

		private void FormJobManagerDashboard_FormClosing(object sender,FormClosingEventArgs e) {
			pieChartRemaining.Dispose();
			pieChartCompleted.Dispose();
		}
	}
}
