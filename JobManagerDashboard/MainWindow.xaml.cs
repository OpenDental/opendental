using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JobManagerDashboard {
	/// <summary>
	/// Interaction logic for JobManagerDashboard.xaml
	/// </summary>
	public partial class JobManagerDashboardTiles : Window, ISignalProcessor {

		private List<string> _listActualEngineers=new List<string>{"Allen","Andrew","BrendanB","Cameron","Chris","David","Derek","Jason","Joe","Josh","KendraS","Lindsay","Matherin","Sam","Saul","Travis"};
		private List<EngInformation> _engInfoList=new List<EngInformation>();
		private List<Job> _listJobsAll=Jobs.GetAll();
		private DateTime startOfWeek=DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek));//gives us the sunday of the current week
		private DateTime curWeek=DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek));
		private Userod selectedEng=new Userod();
		private List<JobPermission> _listJobPermissionsAll=new List<JobPermission>();
		private List<Job> _listAllWriteJobsForEngineer=new List<Job>();
		private List<Job> _listJobsFiltered=new List<Job>();

		///<summary>This is the load method for the class. It initializes the engineer tiles by creating a model and binding it to the ItemsControl template in the xaml code.</summary>
		public JobManagerDashboardTiles() {
			InitializeComponent();
			//labelActiveJobs.Foreground=
			Signalods.SubscribeWPF(this);
			GridToolbar.Visibility=Visibility.Hidden;
			EngSpecificInfo.Visibility=Visibility.Collapsed;
			Jobs.FillInMemoryLists(_listJobsAll);
			_engInfoList.Add(
				new EngInformation {
					EngName="Total Unfinished Quote Jobs: "+JobQuotes.GetUnfinishedJobQuotes().Count,
					EngClockStatus="Total Unfinished Quote: $"+JobQuotes.GetUnfinishedJobQuotes().Sum(x => PIn.Double(x.Amount)),
					EngWorkStatus="Total Unfinished Jobs: "+_listJobsAll.Where(x => x.PhaseCur!=JobPhase.Complete && x.Priority!=JobPriority.OnHold).ToList().Count,
					StatField1="Total Jobs being worked on: "+_listJobsAll.Where(x => x.OwnerAction==JobAction.WriteCode).ToList().Count
				});
			//list of jobs in WriteCode status
			List<Job> listWriteCodeJobs=_listJobsAll.Where(x => x.OwnerAction==JobAction.WriteCode && x.Priority!=JobPriority.OnHold).ToList();
			foreach(Userod user in Userods.GetUsersByJobRole(JobPerm.Engineer,false)) {
				if(!_listActualEngineers.Contains(user.UserName)) {
					continue;
				}
				List<TextBlock> jobTitles=new List<TextBlock>();
				//get only write code jobs
				List<string> listEngJobs=listWriteCodeJobs.Where(x => x.UserNumEngineer==user.UserNum).Select(x => x.Title).ToList();
				foreach(string j in listEngJobs) {
					//TODO: Template this in xaml and pass in the object for the template to autogen, save for after initial working commit.
					TextBlock tb=new TextBlock();
					tb.Text=j;
					tb.TextWrapping=TextWrapping.WrapWithOverflow;
					jobTitles.Add(tb);
				}
				string needsWork="";
				int devHours=listWriteCodeJobs.Where(x => x.UserNumEngineer==user.UserNum).Sum(x => x.HoursEstimate);
				if(devHours<20) {
					needsWork="Needs Work";
				}
				else {
					needsWork="~"+devHours.ToString()+" Dev Hours";
				}
				_engInfoList.Add(new EngInformation { EngName=user.UserName,
					EngClockStatus=ClockEvents.GetEmployeeClockStatus(user.EmployeeNum),
					EngWorkStatus=needsWork,
					EngJobs=jobTitles
				});
			}
			EngTiles.ItemsSource=_engInfoList;
		}

		///<summary>The click event for each tile. This initializes and loads the engineers schedule, jobs grid, and history of clock events. Only clickable if the engineer is the same
		///as the current user or Allen and Nathan.</summary>
		private void button_ClickEngInfo(object sender,RoutedEventArgs e) {
			//reset class date variables
			startOfWeek=DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek));
			curWeek=DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek));
			//retrieve our engineer tag based on the tile clicked
			System.Windows.Controls.Button bt=(System.Windows.Controls.Button) sender;
			if(bt.Tag.ToString().StartsWith("Total")) {
				return;
			}
			selectedEng=UserodC.ShortList.Where(x => x.UserName==bt.Tag.ToString()).First();
			if((Security.CurUser.UserNum!=58 && Security.CurUser.UserNum!=9) && Security.CurUser.UserNum!=selectedEng.UserNum) {//if you aren't Nathan or Allen and are not the engineer you selected, you can't view their information.
				System.Windows.MessageBox.Show(Lans.g("Security","Not authorized for viewing"));
				return;
			}
			buttonGrid.Visibility=Visibility.Collapsed;
			EngSpecificInfo.Visibility=Visibility.Visible;
			GridToolbar.Visibility=Visibility.Visible;
			_listAllWriteJobsForEngineer=_listJobsAll.Where(x => x.UserNumEngineer==selectedEng.UserNum).ToList();
			LabelEngName.Content=selectedEng.UserName;
			long engUserNum=selectedEng.UserNum;
			//Fill our controls for the next view
			FillActiveJobs(engUserNum);
			ClockEventsInRange(startOfWeek.ToShortDateString(),startOfWeek.AddDays(6).ToShortDateString());
			FillEngineerMetrics(engUserNum);
			FillEngineerCalendar(startOfWeek.ToShortDateString(),startOfWeek.AddDays(6).ToShortDateString());
			//TODO: Call the today click here, however this will require waiting for the UI to draw first otherwise the height value we animate to is 0.
			//This will require additional research on methods/techniques to wait for the UI to draw.
		}

		///<summary>Fills the Active Jobs datagrid with the respective engineer's current jobs.</summary>
		private void FillActiveJobs(long userNum) {
			DataTable engJobs=Jobs.GetActiveJobsForUser(userNum);
			//If the datatables are empty then we will hide it and change the label to indicate this.
			if(engJobs.Rows.Count==0) {
				dataGridEngJobs.Visibility=Visibility.Hidden;
				labelActiveJobs.Content="No Active Jobs";
			}
			dataGridEngJobs.ItemsSource=engJobs.DefaultView;
		}

		///<summary>The calendar control's event. Makes heavy use of the helper method below.</summary>
		private void calendar_SelectedDatesChanged(object sender,SelectionChangedEventArgs e) {
			Mouse.Capture(null); //release the mouse focus
			OnCalendarDateChanged();
		}

		///<summary>When the date is changed on the calendar control we initialize and start our animation for the daily metrics and calculate the user's clock events for the given week selected.
		///This method also grabs the user's schedule and populates the calendar grid.</summary>
		private void OnCalendarDateChanged() {
			labelArrived.Foreground=Brushes.Gray;
			DailyMetrics.Visibility=Visibility.Visible;
			DateTime selectedDate=(DateTime)dailyCalendar.SelectedDate;
			//starts animation and sets the values of the fields being animated
			DoubleAnimation da=new DoubleAnimation(0,AnimationRow.ActualHeight,TimeSpan.FromMilliseconds(200));
			DailyMetrics.BeginAnimation(Grid.HeightProperty,da);
			DataTable dt=ClockEvents.GetTimeClockedInOnDate(selectedEng.EmployeeNum,selectedDate);
			//reflect the change in selected date in our date range
			string weekStart=selectedDate.AddDays(-(int)selectedDate.DayOfWeek).ToShortDateString();
			string weekEnd=selectedDate.AddDays(7-((int)selectedDate.DayOfWeek)).ToShortDateString();
			string dateRange=weekStart+"-"+weekEnd;
			ClockEventsInRange(weekStart,weekEnd);
			FillEngineerCalendar(weekStart,weekEnd);
			DateForSched.Content=dateRange;
			ClearDailyLabels();
			if(dt.Rows.Count==0) {
				labelArrived.Content="--";
				labelLeftAt.Content="--";
				labelHoursWorked.Content="--";
				return;
			}
			labelArrived.Content=dt.Rows[0][0].ToString();
			labelLeftAt.Content=dt.Rows[0][1].ToString();
			labelHoursWorked.Content=dt.Rows[0][2].ToString();
			//check to see if the clock in time is past 10 minutes and mark red to indicate tardiness
			string[] clockedInAt=dt.Rows[0][0].ToString().Split(null);//This split removes the 'AM' string at the end of the clock in time
			if(clockedInAt[0]==null) {
				return;
			}
			List<Schedule> timeScheduled=Schedules.GetSchedListForDateRange(selectedEng.EmployeeNum, dailyCalendar.SelectedDate.ToString(), dailyCalendar.SelectedDate.ToString());
			if(timeScheduled.Count==0) {//If the employee is not scheduled they can't be tardy.
				return;
			}
			TimeSpan duration=TimeSpan.Parse(clockedInAt[0]).Subtract(TimeSpan.Parse(timeScheduled.First().StartTime.ToString()));
			if(duration.Minutes>10) {
				labelArrived.Foreground=Brushes.Red;
			}
		}

		///<summary>Animations don't play well with resizing so we just hide the control being animated while the window is resized.</summary>
		private void Window_SizeChanged(object sender,SizeChangedEventArgs e) {
			DailyMetrics.Visibility=Visibility.Hidden;
		}

		///<summary>This method calculates the given engineer statistics in the engineer's data view.</summary>
		private void FillEngineerMetrics(long UserNum) {
			//list of write code jobs for a specific user
			List<Job> listJobs=_listJobsAll.Where(x => x.OwnerAction==JobAction.WriteCode).Where(x=>x.UserNumEngineer==UserNum).ToList();
			labeltotDev.Content=listJobs.Sum(x => x.HoursEstimate).ToString();
			labelLongEst.Content=listJobs.Max(x => x.HoursEstimate).ToString();
			labelNoEst.Content=listJobs.Count(x => x.HoursEstimate==0).ToString();
			labelRewiewReq.Content=listJobs.Sum(x => x.ListJobReviews.Count()).ToString();
			labelPriorityJobs.Content=listJobs.Count(x => x.Priority==JobPriority.High).ToString();
			labelQuoteTotal.Content=listJobs.Sum(x => x.ListJobQuotes.Sum(y => PIn.Double(y.Amount))).ToString("C");
			//all jobs for a given user
			List<Job> listAllJobsForUser=_listJobsAll.Where(x=>x.UserNumEngineer==UserNum).ToList();
			labelConceptJobs.Content=listJobs.Count(x => x.PhaseCur==JobPhase.Concept).ToString();
			labelWriteupJobs.Content=listJobs.Count(x => x.OwnerAction==JobAction.WriteConcept || x.OwnerAction==JobAction.WriteJob).ToString();
			labelDevJobs.Content=listJobs.Count(x => x.PhaseCur==JobPhase.Development).ToString();
			labelAdvisorJobs.Content=listJobs.Count(x => x.OwnerAction==JobAction.Advise).ToString();
			labelJobsOnHold.Content=listJobs.Count(x => x.Priority==JobPriority.OnHold).ToString();
		}

		///<summary>Finds the selected engineers schedule and populates the appropriate labels in the view.</summary>
		private void FillEngineerCalendar(string weekStart,string weekEnd) {
			ClearSchedLabels();
			DoubleAnimation da=new DoubleAnimation(0,1,TimeSpan.FromMilliseconds(200));
			DateForSched.Content=weekStart+"-"+weekEnd;
			List<Schedule> empSchedForWeek=Schedules.GetSchedListForDateRange(selectedEng.EmployeeNum, weekStart, weekEnd);
			foreach(Schedule sched in empSchedForWeek) {
				switch(sched.SchedDate.DayOfWeek) {
					case DayOfWeek.Monday:
						titleMonday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleMonday.FontWeight=FontWeights.Bold;
						LabelMonday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelMonday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
					case DayOfWeek.Tuesday:
						titleTuesday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleTuesday.FontWeight=FontWeights.Bold;
						LabelTuesday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelTuesday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
					case DayOfWeek.Wednesday:
						titleWednesday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleWednesday.FontWeight=FontWeights.Bold;
						LabelWednesday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelWednesday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
					case DayOfWeek.Thursday:
						titleThursday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleThursday.FontWeight=FontWeights.Bold;
						LabelThursday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelThursday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
					case DayOfWeek.Friday:
						titleFriday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleFriday.FontWeight=FontWeights.Bold;
						LabelFriday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelFriday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
					case DayOfWeek.Saturday:
						titleSaturday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleSaturday.FontWeight=FontWeights.Bold;
						LabelSaturday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelSaturday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
					case DayOfWeek.Sunday:
						titleSunday.Foreground=(SolidColorBrush)(new BrushConverter().ConvertFrom("#3C5A96"));
						titleSunday.FontWeight=FontWeights.Bold;
						LabelSunday.Content=sched.StartTime.ToShortTimeString()+"-"+sched.StopTime.ToShortTimeString();
						LabelSunday.BeginAnimation(System.Windows.Controls.Label.OpacityProperty,da);
						break;
				}
			}
		}

		///<summary>Clears schedule labels in the view.</summary>
		private void ClearSchedLabels() {
			LabelMonday.Content="";
			LabelTuesday.Content="";
			LabelWednesday.Content="";
			LabelThursday.Content="";
			LabelFriday.Content="";
			LabelSaturday.Content="";
			LabelSunday.Content="";
			//clear label colors
			titleMonday.Foreground=Brushes.LightGray;
			titleTuesday.Foreground=Brushes.LightGray;
			titleWednesday.Foreground=Brushes.LightGray;
			titleThursday.Foreground=Brushes.LightGray;
			titleFriday.Foreground=Brushes.LightGray;
			titleSaturday.Foreground=Brushes.LightGray;
			titleSunday.Foreground=Brushes.LightGray;
		}

		///<summary></summary>
		private void ClearDailyLabels() {
			labelArrived.Content="";
			labelLeftAt.Content="";
			labelHoursWorked.Content="";
		}

		///<summary>Hits the DB for the clockevents in a given range for the selected engineer tile and databinds to the datagrid in the view.</summary>
		public void ClockEventsInRange(string fromDate, string toDate) {
			labelClockEventsWeek.Content="Clock Events for the Week";
			ClockEventsWeek.Visibility=Visibility.Visible;
			DataTable tableClockEvents=ClockEvents.GetEmployeeClockEventsForDateRange(selectedEng.EmployeeNum,fromDate,toDate);
			if(tableClockEvents.Rows.Count==0) {
				labelClockEventsWeek.Content="No Clock Events for the Week";
				ClockEventsWeek.Visibility=Visibility.Hidden;
			}
			else {
				ClockEventsWeek.ItemsSource=tableClockEvents.AsDataView();
			}
		}

		///<summary>The back button click event below the calendar. Moves the date range back one week and recalculates the clock events and schedule for the selected engineer.</summary>
		private void BackOneWeek_Click(object sender,RoutedEventArgs e) {
			ClockEventsInRange(curWeek.AddDays(-7).ToShortDateString(),curWeek.AddDays(-1).ToShortDateString());//update clock events grid
			DateForSched.Content=curWeek.AddDays(-7).ToShortDateString()+"-"+curWeek.AddDays(-1).ToShortDateString();//update sched label
			FillEngineerCalendar(curWeek.AddDays(-7).ToShortDateString(),curWeek.AddDays(-1).ToShortDateString());//update engineer schedule
			curWeek=curWeek.AddDays(-7);
			dailyCalendar.SelectedDate=curWeek;
			dailyCalendar.DisplayDate=curWeek;
		}

		///<summary>Same as above method, but moves our date range forward one week.</summary>
		private void ForwardOneWeek_Click(object sender,RoutedEventArgs e) {
			ClockEventsInRange(curWeek.AddDays(7).ToShortDateString(),curWeek.AddDays(13).ToShortDateString());//update clock events grid
			DateForSched.Content=curWeek.AddDays(7).ToShortDateString()+"-"+curWeek.AddDays(13).ToShortDateString();//update sched label
			FillEngineerCalendar(curWeek.AddDays(7).ToShortDateString(),curWeek.AddDays(13).ToShortDateString());//update engineer schedule
			curWeek=curWeek.AddDays(7);
			dailyCalendar.SelectedDate=curWeek;
			dailyCalendar.DisplayDate=curWeek;
		}

		///<summary>Interface override so that the dashboard updates in real time with the job manager.</summary>
		public void ProcessSignals(List<Signalod> listSignals) {
			if(!listSignals.Exists(x => x.IType==InvalidType.Jobs)) {
				return;//no job signals;
			}
			//Get the latest jobs that have been updated by the signal.
			//Initialized to <jobNum,null>
			Dictionary<long,Job> dictNewJobs = listSignals.FindAll(x => x.IType==InvalidType.Jobs && x.FKeyType==KeyType.Job)
				.Select(x => x.FKey)
				.Distinct()
				.ToDictionary(x => x,x => (Job)null);
			List<Job> newJobs = Jobs.GetMany(dictNewJobs.Keys.ToList());
			Jobs.FillInMemoryLists(newJobs);
			newJobs.ForEach(x => dictNewJobs[x.JobNum]=x);
			//Update in memory lists.
			foreach(KeyValuePair<long,Job> kvp in dictNewJobs) {
				if(kvp.Value==null) {//deleted job
					_listJobsAll.RemoveAll(x => x.JobNum==kvp.Key);
					continue;
				}
				//Master Job List
				Job jobOld = _listJobsAll.FirstOrDefault(x => x.JobNum==kvp.Key);
				if(jobOld==null) {//new job entirely, no need to update anything in memory, just add to jobs list.
					_listJobsAll.Add(kvp.Value);
					continue;
				}
				_listJobsAll[_listJobsAll.IndexOf(jobOld)]=kvp.Value;
			}
			//Refill grids
			FillActiveJobs(selectedEng.UserNum);
			FillEngineerMetrics(selectedEng.UserNum);
		}

		///<summary>Resets the date range to today and recalculates the schedule and clock events grids.</summary>
		private void ButtonTodayClick(object sender,RoutedEventArgs e) {
			//reset date variables
			startOfWeek=DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek));
			curWeek=DateTime.Today.AddDays(-(int)(DateTime.Today.DayOfWeek));
			string start=startOfWeek.ToShortDateString();
			string end=startOfWeek.AddDays(7).ToShortDateString();
			DateForSched.Content=start+"-"+end;
			dailyCalendar.SelectedDate=DateTime.Today;
			dailyCalendar.DisplayDate=DateTime.Today;
			OnCalendarDateChanged();
			ClockEventsInRange(startOfWeek.ToShortDateString(),startOfWeek.AddDays(6).ToShortDateString());
			FillEngineerCalendar(start,end);
		}

		///<summary>Resets view visibilities to give the appearence of going back a page. This event will repopulate the view with the engineer tiles.</summary>
		private void ButtonBackClick(object sender,RoutedEventArgs e) {
			//we simply hide and show various controls to give the sense of switching pages
			EngSpecificInfo.Visibility=Visibility.Collapsed;
			GridToolbar.Visibility=Visibility.Collapsed;
			DailyMetrics.Visibility=Visibility.Collapsed;
			buttonGrid.Visibility=Visibility.Visible;
			dailyCalendar.DisplayDate=DateTime.Today;
			dailyCalendar.SelectedDate=DateTime.Today;
			DailyMetrics.Visibility=Visibility.Collapsed;
			GridEngJobs.Visibility=Visibility.Collapsed;
			EngSched.Visibility=Visibility.Visible;
		}

		//TODO: put this in its own model class
		///<summary>This class represents the model needed to fill the desired information for the Engineer tiles.</summary>
		public class EngInformation{
			public string EngName { get; set; }
			public string EngClockStatus { get; set; }
			public string EngWorkStatus { get; set; }
			public List<TextBlock> EngJobs { get; set; }
			//only used for the main tile that appears first in the collection
			public string StatField1 { get; set; }
		}

		//TODO: put this in its own model class
		///<summary>The model class for the engineer tiles.</summary>
		public class EngJobsByAction {
			public string Priority { get; set; }
			public string Category { get; set; }
			public string Owner { get; set; }
			public string Title { get; set; }
		}
	}
}
