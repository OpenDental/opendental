using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormReleaseCalculator:FormODBase {
		private List<Tuple<long,double>> _listTopJobs=new List<Tuple<long,double>>();
		private double _avgJobHours=9.43;
		private double _jobTimePercent=0.173;
		private double _avgBreakHours=0.85;
		private List<Job> _listJobsAll;


		public FormReleaseCalculator(List<Job> listJobsAll) {
			_listJobsAll=listJobsAll;
			InitializeComponent();
			InitializeLayoutManager(isLayoutMS:true);
			Lan.F(this);
		}

		private void FormReleaseCalculator_Load(object sender,EventArgs e) {
			textAvgJobHours.Text=_avgJobHours.ToString();
			textEngJobPercent.Text=_jobTimePercent.ToString();
			textBreakHours.Text=_avgBreakHours.ToString();
			foreach(Def def in Defs.GetDefsForCategory(DefCat.JobPriorities,true).OrderBy(x => x.ItemOrder).ToList()) {
				listPriorities.Items.Add(def.ItemName,def);
				if(ListTools.In(def.DefNum,597,601)) {
					listPriorities.SelectedIndices.Add(listPriorities.Items.Count-1);
				}
			}
			foreach(JobPhase phase in Enum.GetValues(typeof(JobPhase))) {
				listPhases.Items.Add(phase.ToString(),phase);
				if(ListTools.In(phase,JobPhase.Definition,JobPhase.Development)) {
					listPhases.SelectedIndices.Add(listPhases.Items.Count-1);
				}
			}
			foreach(JobCategory category in Enum.GetValues(typeof(JobCategory))) {
				listCategories.Items.Add(category.ToString(),category);
				if(ListTools.In(category,JobCategory.Enhancement,JobCategory.Feature,JobCategory.HqRequest,JobCategory.InternalRequest,JobCategory.ProgramBridge)) {
					listCategories.SelectedIndices.Add(listCategories.Items.Count-1);
				}
			}
			foreach(Employee emp in JobHelper.ListEngineerEmployees) {
				listEngineers.Items.Add(emp.FName,emp);
				listEngineers.SelectedIndices.Add(listEngineers.Items.Count-1);
			}
		}

		private void butCalculate_Click(object sender,EventArgs e) {
			_listTopJobs.Clear();
			listEngNoJobs.Items.Clear();
			List<long> listEngNums=listEngineers.GetListSelected<Employee>().Select(x => x.EmployeeNum).ToList();
			List<long> listUserNums=listEngNums.Select(x => Userods.GetUserByEmployeeNum(x).UserNum).ToList();
			//Get 6 months of scheduled engineering time. Arbitrary because there should be no way we have a 6 month release cycle.
			List<Schedule> listSchedules=Schedules.RefreshPeriodForEmps(DateTime.Today,DateTime.Today.AddMonths(6),listEngNums);
			//Get all the jobs according to the selected criteria.
			//No need to fill currently, but I may want to add reviews into this to improve accuracy for unfinished jobs
			List<Job> listJobs=_listJobsAll.Where(x => ListTools.In(x.Priority,listPriorities.GetListSelected<Def>().Select(y => y.DefNum)) 
				&& ListTools.In(x.PhaseCur,listPhases.GetListSelected<JobPhase>())
				&& ListTools.In(x.Category,listCategories.GetListSelected<JobCategory>())).ToList();
			double totalJobHours=0;
			DateTime releaseDate=DateTime.Today;
			double avgJobHours=_avgJobHours;
			double jobTimePercent=_jobTimePercent;
			double avgBreakHours=_avgBreakHours;
			Double.TryParse(textAvgJobHours.Text,out avgJobHours);
			Double.TryParse(textEngJobPercent.Text,out jobTimePercent);
			Double.TryParse(textBreakHours.Text,out avgBreakHours);
			gridCalculatedJobs.Visible=true;
			gridCalculatedJobs.BeginUpdate();
			gridCalculatedJobs.ListGridColumns.Clear();
			gridCalculatedJobs.ListGridColumns.Add(new GridColumn("EstHrs",60) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridCalculatedJobs.ListGridColumns.Add(new GridColumn("ActHrs",60) { TextAlign=HorizontalAlignment.Center,SortingStrategy=GridSortingStrategy.AmountParse });
			gridCalculatedJobs.ListGridColumns.Add(new GridColumn("",300));
			gridCalculatedJobs.ListGridRows.Clear();
			foreach(Job job in listJobs.OrderBy(x => x.HoursEstimate)) {
				if(job.UserNumEngineer==0 && listUserNums.Contains(job.UserNumExpert)) {
					listUserNums.Remove(job.UserNumExpert);
				}
				if(job.UserNumEngineer!=0 && listUserNums.Contains(job.UserNumEngineer)) {
					listUserNums.Remove(job.UserNumEngineer);
				}
				//If hrsEst is 0 then use the avgJobHours as a base.
				double hrsEst=job.HoursEstimate==0?avgJobHours:job.HoursEstimate;
				//Remove the actual hours spent on the job currently
				//If negative then just use 0 (We aren't in a dimension where negative time estimates can be used for other jobs)
				double hrsCalculated=(hrsEst-job.HoursActual)<0?0:hrsEst-job.HoursActual;
				totalJobHours+=hrsCalculated;
				if(job.PhaseCur==JobPhase.Development) {
					_listTopJobs.Add(new Tuple<long,double>(job.JobNum,hrsCalculated));
				}
				gridCalculatedJobs.ListGridRows.Add(
					new GridRow(
						new GridCell(job.HoursEstimate==0?"0("+_avgJobHours+")":job.HoursEstimate.ToString()),
						new GridCell(job.HoursActual.ToString()),
						new GridCell(job.ToString())
						) {
						Tag=job
					}
					);
			}
			gridCalculatedJobs.EndUpdate();
			foreach(long engNum in listUserNums) {
				Userod eng=Userods.GetUser(engNum);
				listEngNoJobs.Items.Add(eng.UserName,eng);
			}
			labelJobHours.Text=Math.Round(totalJobHours).ToString();
			labelJobNumber.Text=listJobs.Count.ToString();
			double schedHoursTotal=0;
			double schedHoursBreaksTotal=0;
			double schedHoursPercentTotal=0;
			foreach(Schedule sched in listSchedules) {
				//Calculate actual scheduled time
				double schedHours=(sched.StopTime-sched.StartTime).TotalHours;
				schedHoursTotal+=schedHours;
				//Remove average break time
				schedHours-=avgBreakHours;
				schedHoursBreaksTotal+=schedHours;
				//Multiply the scheduled time by the percentage of coding time for the jobs we care about
				schedHours=schedHours*jobTimePercent;
				schedHoursPercentTotal+=schedHours;
				//Remove the scheduled hours from the total job hours
				totalJobHours-=schedHours;
				if(totalJobHours<0) {
					releaseDate=sched.SchedDate;//Add a week as a buffer
					break;
				}
			}
			labelEngHours.Text=Math.Round(schedHoursTotal).ToString();
			labelAfterBreak.Text=Math.Round(schedHoursBreaksTotal).ToString();
			labelRatioHours.Text=Math.Round(schedHoursPercentTotal).ToString();
			labelReleaseDate.Text=releaseDate.ToShortDateString()+" - "+releaseDate.AddDays(7).ToShortDateString();
			labelReleaseDate.Visible=true;
			panelCalc.Visible=true;
		}

		private void butJob1_Click(object sender,EventArgs e) {
			FormOpenDental.S_GoToJob(_listTopJobs[0].Item1);
		}

		private void butJob2_Click(object sender,EventArgs e) {
			FormOpenDental.S_GoToJob(_listTopJobs[1].Item1);
		}

		private void butJob3_Click(object sender,EventArgs e) {
			FormOpenDental.S_GoToJob(_listTopJobs[2].Item1);
		}

		private void gridCalculatedJobs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormOpenDental.S_GoToJob(((Job)gridCalculatedJobs.ListGridRows[e.Row].Tag).JobNum);
		}
	}
}