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
	public partial class FormJobManagerDashboard:FormODBase {
		private List<Job> _listJobsAll;
		private JobSprint _jobSprintCur;
		private List<Def> _listJobPriorities;
		private List<JobLog> _listJobLogs;
		private Bitmap pieChartRemaining;
		private Bitmap pieChartCompleted;
		private Bitmap pieChartRemainingBackport;
		private Bitmap pieChartCompletedBackport;

		public FormJobManagerDashboard(List<Job> listJobsAll,JobSprint sprint) {
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
			timerRefreshAll.Start();
		}

		///<summary>Calculates the info for the dashboard</summary>
		private void RefreshAll() {
			//Validation
			if(_jobSprintCur.DateStart>_jobSprintCur.DateEndTarget) {
				return;
			}
			this.Text=_jobSprintCur.Title;
			#region Variables
			DateTime dateStart=_jobSprintCur.DateStart;
			DateTime dateProgress=DateTime.Today.Date;
			DateTime dateEnd=_jobSprintCur.DateEndTarget;
			//Distill the _listJobsAll into two job subsets
			List<Job> listJobsNonBugEnhancement=_listJobsAll.FindAll(x => ListTools.In(x.Category,
			JobCategory.Feature,JobCategory.InternalRequest,JobCategory.HqRequest,JobCategory.ProgramBridge));
			List<Job> listJobsBugEnhancement=_listJobsAll.FindAll(x => ListTools.In(x.Category,JobCategory.Bug,JobCategory.Enhancement));
			//Make the actual lists we will be using
			List<Job> listJobsCompletedNonBugEnhancement=listJobsNonBugEnhancement.FindAll(x => ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)
				&& DateTimeOD.Between(_listJobLogs.FirstOrDefault(y => y.JobNum==x.JobNum && y.Description.Contains("Job implemented"))?.DateTimeEntry??DateTime.MinValue,dateStart,dateEnd));     
			List<Job> listCompletedNonBugJobsThreeMonths=_listJobsAll.FindAll(x => ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete) && ListTools.In(x.Category,JobCategory.Feature,JobCategory.InternalRequest,JobCategory.HqRequest,JobCategory.ProgramBridge,JobCategory.Enhancement)
				&& DateTimeOD.Between(_listJobLogs.FirstOrDefault(y => y.JobNum==x.JobNum && y.Description.Contains("Job implemented"))?.DateTimeEntry??DateTime.MinValue,DateTime.Today.AddMonths(-3),DateTime.Today));
			List<Job> listRemainingNonBugJobs=listJobsNonBugEnhancement.FindAll(x => ListTools.In(x.PhaseCur,
			JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development) && x.ProposedVersion==JobProposedVersion.Current);
			List<Job> listCompletedBackportJobs=listJobsBugEnhancement.FindAll(x => ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)
				&& x.ListJobLinks.Count(y => y.LinkType==JobLinkType.Bug)>0
				&& DateTimeOD.Between(_listJobLogs.FirstOrDefault(y => y.JobNum==x.JobNum && y.Description.Contains("Job implemented"))?.DateTimeEntry??DateTime.MinValue,dateStart,dateEnd));
			List<Job> listRemainingBugJobs=listJobsBugEnhancement.FindAll(x => x.Category==JobCategory.Bug && ListTools.In(x.PhaseCur,
				JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)
				&& x.ListJobLinks.Count(y => y.LinkType==JobLinkType.Bug)>0);
			List<Job> listRemainingEnhancementJobs=listJobsBugEnhancement.FindAll(x => x.Category==JobCategory.Enhancement 
				&& ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development)
				&& x.ProposedVersion==JobProposedVersion.Current);
			double totalDays=(dateEnd - dateStart).TotalDays;
			double totalDaysElapsed=(dateProgress - dateStart).TotalDays;
			double totalDaysLeft=totalDays-totalDaysElapsed;
			double jobPercent=_jobSprintCur.JobPercent;
			double hoursComplete=listJobsCompletedNonBugEnhancement.Sum(x => x.HoursActual)+listRemainingNonBugJobs.Sum(x => x.HoursActual);
			double hoursTotal=listJobsCompletedNonBugEnhancement.Sum(x => x.HoursActual)+listRemainingNonBugJobs.Sum(x => x.HoursEstimate);
			double hoursRemaining=hoursTotal-hoursComplete>0?hoursTotal-hoursComplete:0;
			#endregion
			RefreshTopPanel(listJobsNonBugEnhancement,dateStart,dateEnd);
			#region Left Upper Panel
			//Set all completed job percentages
			List<double> listCompletedPercents=new List<double>();
			//Cast one to a double so the int doesn't truncate to 0.
			double percent=(double)(listJobsCompletedNonBugEnhancement.Where(x => x.Category==JobCategory.Feature).Count())/listJobsCompletedNonBugEnhancement.Count()*100;
			listCompletedPercents.Add(Math.Round(percent,2));
			textCompletedFeaturesForRange.Text=percent.ToString("0.00")+"%";
			percent=(double)(listJobsCompletedNonBugEnhancement.Where(x => x.Category==JobCategory.HqRequest).Count())/listJobsCompletedNonBugEnhancement.Count()*100;
			listCompletedPercents.Add(Math.Round(percent,2));
			textCompletedHQRequestsForRange.Text=percent.ToString("0.00")+"%";
			percent=(double)(listJobsCompletedNonBugEnhancement.Where(x => x.Category==JobCategory.InternalRequest).Count())/listJobsCompletedNonBugEnhancement.Count()*100;
			listCompletedPercents.Add(Math.Round(percent,2));
			textCompletedInternalRequestsForRange.Text=percent.ToString("0.00")+"%";
			percent=(double)(listJobsCompletedNonBugEnhancement.Where(x => x.Category==JobCategory.ProgramBridge).Count())/listJobsCompletedNonBugEnhancement.Count()*100;
			listCompletedPercents.Add(Math.Round(percent,2));
			textCompletedBridgesForRange.Text=percent.ToString("0.00")+"%";
			DrawCompletedPieChart(listCompletedPercents);
			textJobsComplete.Text=listJobsCompletedNonBugEnhancement.Count().ToString();
			#endregion
			#region Right Upper Panel
			//Set all remaining job percentages
			List<double> listRemainingPercents=new List<double>();
			percent=(double)(listRemainingNonBugJobs.Where(x => x.Category==JobCategory.Feature).Count())/listRemainingNonBugJobs.Count()*100;
			listRemainingPercents.Add(Math.Round(percent,2));
			textRemainingFeaturePercent.Text=percent.ToString("0.00")+"%";
			percent=(double)(listRemainingNonBugJobs.Where(x => x.Category==JobCategory.HqRequest).Count())/listRemainingNonBugJobs.Count()*100;
			listRemainingPercents.Add(Math.Round(percent,2));
			textRemainingHQRequestPercent.Text=percent.ToString("0.00")+"%";
			percent=(double)(listRemainingNonBugJobs.Where(x => x.Category==JobCategory.InternalRequest).Count())/listRemainingNonBugJobs.Count()*100;
			listRemainingPercents.Add(Math.Round(percent,2));
			textRemainingInternalRequestPercent.Text=percent.ToString("0.00")+"%";
			percent=(double)(listRemainingNonBugJobs.Where(x => x.Category==JobCategory.ProgramBridge).Count())/listRemainingNonBugJobs.Count()*100;
			listRemainingPercents.Add(Math.Round(percent,2));
			textRemainingBridgePercent.Text=percent.ToString("0.00")+"%";
			DrawRemainingPieChart(listRemainingPercents);
			textJobsRemaining.Text=listRemainingNonBugJobs.Count().ToString();
			#endregion
			#region Left Lower Panel
			//Set all completed backport percentages
			List<double> listCompletedBackportPercents=new List<double>();
			percent=(double)(listCompletedBackportJobs.FindAll(x => x.Category==JobCategory.Bug).Count())/listCompletedBackportJobs.Count()*100;
			listCompletedBackportPercents.Add(Math.Round(percent,2));
			textCompletedBugPercentage.Text=percent.ToString("0.00")+"%";
			percent=(double)(listCompletedBackportJobs.FindAll(x => x.Category==JobCategory.Enhancement).Count())/listCompletedBackportJobs.Count()*100;
			listCompletedBackportPercents.Add(Math.Round(percent,2));
			textCompletedEnhancementPercentage.Text=percent.ToString("0.00")+"%";
			DrawCompletedBackportPieChart(listCompletedBackportPercents);
			textCompletedBackports.Text=listCompletedBackportJobs.Count().ToString();
			#endregion
			#region Right Lower Panel
			//Set all remaining backport percentages
			List<double> listRemainingBackportPercents=new List<double>();
			int countBugEnhancement=listRemainingBugJobs.Count()+listRemainingEnhancementJobs.Count();
			percent=(double)(listRemainingBugJobs.Count())/countBugEnhancement*100;
			listRemainingBackportPercents.Add(Math.Round(percent,2));
			textRemainingBugPercentage.Text=percent.ToString("0.00")+"%";
			percent=(double)(listRemainingEnhancementJobs.Count())/countBugEnhancement*100;
			listRemainingBackportPercents.Add(Math.Round(percent,2));
			textRemainingEnhancementPercentage.Text=percent.ToString("0.00")+"%";
			DrawRemainingBackportPieChart(listRemainingBackportPercents);
			textRemainingBackports.Text=countBugEnhancement.ToString();
			#endregion
			#region Bottom Panel
			List<Job> listStaleBugs = listRemainingBugJobs.FindAll(x => x.ListJobTimeLogs.Count>0 ? x.ListJobTimeLogs.Select(y => y.DateTStamp).Max()<DateTime.Today.AddDays(-7) : x.DateTimeEntry<DateTime.Today.AddDays(-7))
				.OrderBy(x => x.ListJobTimeLogs.Count>0 ? x.ListJobTimeLogs.Select(y => y.DateTStamp).Max() : x.DateTimeEntry).ToList();
			List<Job> listStaleEnhancements = listRemainingEnhancementJobs.FindAll(x => x.ListJobTimeLogs.Count>0 ? x.ListJobTimeLogs.Select(y => y.DateTStamp).Max()<DateTime.Today.AddDays(-7) : x.DateTimeEntry<DateTime.Today.AddDays(-7))
				.OrderBy(x => x.ListJobTimeLogs.Count>0 ? x.ListJobTimeLogs.Select(y => y.DateTStamp).Max() : x.DateTimeEntry).ToList();
			gridBugJobs.Title=listRemainingBugJobs.Count()+" Bugs ("+listStaleBugs.Count()+" Stale)";
			gridEnhancementJobs.Title=listRemainingEnhancementJobs.Count()+" Enhancements ("+listStaleEnhancements.Count()+" Stale)";
			FillGridOldBugs(listStaleBugs);
			FillGridOldEnhancements(listStaleEnhancements);
			#endregion
		}

		private void RefreshTopPanel(List<Job> listJobs, DateTime dateStart, DateTime dateEnd) {
			//Calculate the three month average per day
			//listJobsNonBugEnhancement is used because we do not want to consider bugs/enhancements (Backport jobs) as part of this version
			List<Job> listCompletedJobs=listJobs.FindAll(x => ListTools.In(x.PhaseCur,JobPhase.Documentation,JobPhase.Complete)
				&& DateTimeOD.Between(_listJobLogs.FirstOrDefault(y => y.JobNum==x.JobNum && y.Description.Contains("Job implemented"))?.DateTimeEntry??DateTime.MinValue,dateStart,dateEnd));
			//We only want incomplete jobs that are marked as current version. This disregards priority.
			List<Job> listRemainingJobs=listJobs.FindAll(x => ListTools.In(x.PhaseCur,JobPhase.Concept,JobPhase.Definition,JobPhase.Quote,JobPhase.Development) && x.ProposedVersion==JobProposedVersion.Current);
			//Calculate the remaining hours for the version
			double totalRemainingHours=listRemainingJobs.Sum(x => x.HoursEstimate-x.HoursActual);
			totalRemainingHours=totalRemainingHours < 0 ? 0 : totalRemainingHours;
			//Completed hours is only the hours actual. Should not be negative, but I check for it just in case.
			double totalCompletedHours=listCompletedJobs.Sum(x => x.HoursActual);
			totalCompletedHours=totalCompletedHours < 0 ? 0 : totalCompletedHours;
			//Calculate the total hours value for the version. 
			//We use the estimate from the remaining jobs since that is the perceived total amount of time it will take.
			//We use the hours actual from complete jobs because we know the job is done and will take no more time than what was taken.
			double totalVersionHours=totalCompletedHours+listRemainingJobs.Sum(x => x.HoursEstimate);
			totalVersionHours=totalVersionHours < 0 ? 0 : totalVersionHours;
			//Calculate work complete percent
			double completionPercentage=((totalCompletedHours+listRemainingJobs.Sum(x => x.HoursActual))/totalVersionHours)*100;
			//Validate that the return is a valid percentage
			if(Double.IsNaN(completionPercentage)) {
				completionPercentage=0;
			}
			completionPercentage=completionPercentage>=100 ? 100 : completionPercentage;
			//Calculate day variables
			double totalDays=(dateEnd - dateStart).TotalDays;
			double totalDaysElapsed=(DateTime.Today.Date - dateStart).TotalDays;
			double totalDaysLeft=totalDays-totalDaysElapsed;
			//We don't need to display negative days left
			if(totalDaysLeft<0) {
				totalDaysLeft=0;
			}
			//Calculate target percent
			int targetValuePercent=(int)(totalDaysElapsed/totalDays*100);
			if(targetValuePercent>100) {
				targetValuePercent=100;
			}
			if(targetValuePercent<0) {
				targetValuePercent=0;
			}
			//Calculate average hours/day of job time for the last 3 months
			double hoursLogged=listJobs.Sum(x => x.ListJobTimeLogs.Where(y => y.DateTStamp.Between(DateTime.Today.AddMonths(-3),DateTime.Today)).Sum(y => y.Hours));
			//hoursReviewed is multiplied by 2 to denote that two engineers time was expended per review
			double hoursReviewed=listJobs.Sum(x => x.ListJobReviews.Where(y => y.DateTStamp.Between(DateTime.Today.AddMonths(-3),DateTime.Today)).Sum(y => y.Hours))*2;
			double hoursCompletePerDay=(hoursLogged+hoursReviewed)/(DateTime.Today-DateTime.Today.AddMonths(-3)).TotalDays;
			//Calculate days remaining
			double daysRemainingEstimate=Math.Round(totalRemainingHours/hoursCompletePerDay,0);
			//If for some reason we just divided by 0...
			if(Double.IsNaN(daysRemainingEstimate)) {
				daysRemainingEstimate=0;
			}
			//Set Panel's Controls
			//Dates
			textStartDate.Text=_jobSprintCur.DateStart.ToShortDateString();
			textTargetDate.Text=_jobSprintCur.DateEndTarget.ToShortDateString()+" ("+totalDaysLeft+" days)";
			textProjectedRelease.Text=DateTime.Today.AddDays(daysRemainingEstimate).ToShortDateString()+" ("+Math.Abs(daysRemainingEstimate)+" days)";
			//Work Complete
			textCompletePercentage.Text=(int)completionPercentage+"%";
			progressComplete.Value=(int)completionPercentage;
			progressComplete.TargetValue=(int)completionPercentage;//This could be changed to the target value percent if we want
			//Time elapsed
			textElapsedPercentage.Text=targetValuePercent+"%";
			progressElapsed.Value=targetValuePercent;
			progressElapsed.TargetValue=targetValuePercent;
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(!listSignals.Exists(x => x.IType==InvalidType.Defs)) {
				return;//no job signals;
			}
			_listJobPriorities=Defs.GetDefsForCategory(DefCat.JobPriorities);
		}		
		
		#region FillGrids
		///<summary>Fills with bug jobs have not had a timelog in the last 7 days. Ordered by last non-viewed log date.</summary>
		private void FillGridOldBugs(List<Job> listStaleBugJobs) {
			gridBugJobs.BeginUpdate();
			gridBugJobs.ListGridColumns.Clear();
			gridBugJobs.ListGridColumns.Add(new GridColumn("Last Updated",180,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
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

		///<summary>Fills with enhancement jobs have not had a timelog in the last 7 days. Ordered by last non-viewed log date.</summary>
		private void FillGridOldEnhancements(List<Job> listStaleEnhancementJobs) {
			gridEnhancementJobs.BeginUpdate();
			gridEnhancementJobs.ListGridColumns.Clear();
			gridEnhancementJobs.ListGridColumns.Add(new GridColumn("Last Updated",180,HorizontalAlignment.Center,GridSortingStrategy.DateParse));
			gridEnhancementJobs.ListGridColumns.Add(new GridColumn("",100){ IsWidthDynamic=true });
			gridEnhancementJobs.ListGridRows.Clear();
			foreach(Job job in listStaleEnhancementJobs) {
				gridEnhancementJobs.ListGridRows.Add(
				new GridRow(
					new GridCell(job.ListJobTimeLogs.Count>0 ? job.ListJobTimeLogs.Max(x => x.DateTStamp).ToShortDateString() : job.DateTimeEntry.ToShortDateString()),
					new GridCell(job.ToString())
					) {
					Tag=job
				}
				);
			}
			gridEnhancementJobs.EndUpdate();
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

		private void panelRemainingBackportsPieChart_Paint(object sender,PaintEventArgs e) {
			if(pieChartRemainingBackport==null) {
				return;
			}
			e.Graphics.DrawImage(pieChartRemainingBackport,0,0);

		}

		private void panelCompletedBackportsPieChart_Paint(object sender,PaintEventArgs e) {
			if(pieChartCompletedBackport==null) {
				return;
			}
			e.Graphics.DrawImage(pieChartCompletedBackport,0,0);

		}

		///<summary></summary>
		public void DrawCompletedPieChart(List<double> listPercents) {
			List<Color> listColors=new List<Color>() {
				pictureCompletedFeaturesColor.BackColor,
				pictureCompletedHQRequestsColor.BackColor,
				pictureCompletedInternalRequestsColor.BackColor,
				pictureCompletedBridgesColor.BackColor,
			};
			if(pieChartCompleted!=null) {
				pieChartCompleted.Dispose();
			}
			pieChartCompleted=new Bitmap(panelCompletedPieChart.Width,panelCompletedPieChart.Height);
			using(Graphics g=Graphics.FromImage(pieChartCompleted)) {
				//Hardcoded location and size
				Point location=new Point(10,10);
				Size size = new Size(panelCompletedPieChart.Width-20,panelCompletedPieChart.Height-20);
				DrawPieChart(listPercents,listColors,g,location,size);
			}
			panelCompletedPieChart.Invalidate();
		}

		///<summary></summary>
		public void DrawCompletedBackportPieChart(List<double> listPercents) {
			List<Color> listColors=new List<Color>() {
				pictureCompletedBugsColor.BackColor,
				pictureCompletedEnhancementsColor.BackColor,
			};
			if(pieChartCompletedBackport!=null) {
				pieChartCompletedBackport.Dispose();
			}
			pieChartCompletedBackport=new Bitmap(panelCompletedBackportsPieChart.Width,panelCompletedBackportsPieChart.Height);
			using(Graphics g=Graphics.FromImage(pieChartCompletedBackport)) {
				//Hardcoded location and size
				Point location=new Point(10,10);
				Size size = new Size(panelCompletedBackportsPieChart.Width-20,panelCompletedBackportsPieChart.Height-20);
				DrawPieChart(listPercents,listColors,g,location,size);
			}
			panelCompletedBackportsPieChart.Invalidate();
		}

		///<summary></summary>
		public void DrawRemainingPieChart(List<double> listPercents) {
			List<Color> listColors=new List<Color>() {
				pictureRemainingFeaturesColor.BackColor,
				pictureRemainingHQRequestsColor.BackColor,
				pictureRemainingInternalRequestsColor.BackColor,
				pictureRemainingBridgesColor.BackColor,
			};
			if(pieChartRemaining!=null) {
				pieChartRemaining.Dispose();
			}
			pieChartRemaining=new Bitmap(panelRemainingPieChart.Width,panelRemainingPieChart.Height);
			using(Graphics g=Graphics.FromImage(pieChartRemaining)) {
				//Hardcoded location and size
				Point location=new Point(10,10);
				Size size = new Size(panelRemainingPieChart.Width-20,panelRemainingPieChart.Height-20);
				DrawPieChart(listPercents,listColors,g,location,size);
			}
			panelRemainingPieChart.Invalidate();
		}

		///<summary></summary>
		public void DrawRemainingBackportPieChart(List<double> listPercents) {
			List<Color> listColors=new List<Color>() {
				pictureRemainingBugsColor.BackColor,
				pictureRemainingEnhancementsColor.BackColor,
			};
			if(pieChartRemainingBackport!=null) {
				pieChartRemainingBackport.Dispose();
			}
			pieChartRemainingBackport=new Bitmap(panelRemainingBackportsPieChart.Width,panelRemainingBackportsPieChart.Height);
			using(Graphics g=Graphics.FromImage(pieChartRemainingBackport)) {
				//Hardcoded location and size
				Point location=new Point(10,10);
				Size size = new Size(panelRemainingBackportsPieChart.Width-20,panelRemainingBackportsPieChart.Height-20);
				DrawPieChart(listPercents,listColors,g,location,size);
			}
			panelRemainingBackportsPieChart.Invalidate();
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
			g.DrawEllipse(Pens.Black,location.X,location.Y,size.Width,size.Height);
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

		#endregion

		private void FormJobManagerDashboard_FormClosing(object sender,FormClosingEventArgs e) {
			pieChartRemaining.Dispose();
			pieChartCompleted.Dispose();
			pieChartRemainingBackport.Dispose();
			pieChartCompletedBackport.Dispose();
		}
	}
}
