using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental
{
	public partial class FormGraphEmployeeTime:FormODBase {
		///<summary>This is the red target line.</summary>
		private List<PointF> _listPointFsRedTarget;
		private float[] _floatArrayBuckets;//a bucket can hold partial people.
		private DateTime _dateShowing;
		private int[] _intArrayMinutesBehind;
		///<summary>Local copy of cache. Only three fields are used: EmployeeNum, EmpName, IsGraphed.</summary>
		private List<PhoneEmpDefault> _listPhoneEmpDefaultsRaw;
		///<summary>This is what shows in the grid.</summary>
		private List<PhoneEmpDefault> _listPhoneEmpDefaultsShowing;
		private List<PhoneGraph> _listPhoneGraphs;
		private List<Schedule> _listSchedules;
		///<summary>This one doesn't have an emp Num. Can be null.</summary>
		private PhoneGraph _phoneGraphDay;
		private bool _isAtDailyLimit;
		private int _pagesPrinted;
		private bool _headingPrinted;
		private int _headingPrintH;

		public FormGraphEmployeeTime(DateTime dateShowing) {
			InitializeComponent();
			InitializeLayoutManager();
			_dateShowing=dateShowing;
			Lan.F(this);
			toolTip.ToolTipTitle=Lan.g(this,"Employees");
			//_listRegions=new List<Region>();
		}

		private void FormGraphEmployeeTime_Load(object sender,EventArgs e) {
			butPrefs.Enabled=Security.IsAuthorized(EnumPermType.Schedules,true);
			butEditDaily.Enabled=Security.IsAuthorized(EnumPermType.Schedules,true);
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			PhoneGraphs.AddMissingEntriesForToday(listPhoneEmpDefaults);
			comboSupervisor.Items.Add("Any",new Employee());
			List<Employee> listEmployees=Employees.GetDeepCopy(isShort:true);
			List<Employee> listEmployeeSupers=new List<Employee>();
			for(int i=0;i<listEmployees.Count;i++){
				if(listEmployees[i].ReportsTo==0){
					continue;
				}
				if(listEmployeeSupers.Any(x=>x.EmployeeNum==listEmployees[i].ReportsTo)){
					continue;
				}
				listEmployeeSupers.Add(Employees.GetEmp(listEmployees[i].ReportsTo));
			}
			listEmployeeSupers=listEmployeeSupers.OrderBy(x=>x.FName).ToList();
			comboSupervisor.Items.AddList(listEmployeeSupers,x=>x.FName);
			comboSupervisor.SetSelected(0);
			listOrder.SelectedIndex=0;
			RefreshDataForDay();
			FillGrid();
			Height=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea.Height;
			Top=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea.Y;
		}

		#region Graph
		private void RefreshDataForDay() {
			_listPhoneEmpDefaultsRaw=PhoneEmpDefaults.GetDeepCopy();
			_listSchedules=Schedules.GetDayList(_dateShowing);
			_listPhoneGraphs=PhoneGraphs.GetAllForDate(_dateShowing);	
			_phoneGraphDay=_listPhoneGraphs.FirstOrDefault(x=>x.EmployeeNum==0);
			_isAtDailyLimit=false;
			string strSupers=PrefC.GetRaw("GraphEmployeeMaxPresched");//list of supervisors
			List<string> listEmpNumsSuper=strSupers.Split(',').ToList();
			bool isCallCenter(long empNum){//local function
				Employee employee=Employees.GetEmp(empNum);
				if(employee is null){
					return false;
				}
				return listEmpNumsSuper.Contains(employee.ReportsTo.ToString());
			}
			if(_phoneGraphDay!=null){
				textNote.Text=_phoneGraphDay.Note;
				textDailyLimit.Text=_phoneGraphDay.DailyLimit.ToString();
				//If the employee doesn't have a schedule, they shouldn't be counted, because they aren't even visible in the grid
				int count=_listPhoneGraphs
					.FindAll(x=>x.PreSchedOff && isCallCenter(x.EmployeeNum) && _listSchedules
					.Exists(y=>y.EmployeeNum==x.EmployeeNum)).Count();
				textDailyLimitSoFar.Text=count.ToString();
				if(_phoneGraphDay.DailyLimit>0 && count>=_phoneGraphDay.DailyLimit){
					_isAtDailyLimit=true;
				}
			}
			else{
				textNote.Text="";
				textDailyLimit.Text="";
				textDailyLimitSoFar.Text="";
			}
			if(_isAtDailyLimit){
				labelLimit.Visible=true;
			}
			else{
				labelLimit.Visible=false;
			}
			int absentCount=_listPhoneGraphs.FindAll(x=>x.Absent==true).Count();
			textAbsent.Text=absentCount.ToString();
			labelDate.Text=_dateShowing.ToString("dddd, MMMM d");
			_listPointFsRedTarget=new List<PointF>();
			//For each "point", the first val is time of day, and second val is based on a peak of 100
			if(_dateShowing.DayOfWeek==DayOfWeek.Friday) {
				_listPointFsRedTarget.Add(new PointF(5f,7));
				_listPointFsRedTarget.Add(new PointF(5.5f,19));
				_listPointFsRedTarget.Add(new PointF(6.5f,39));
				_listPointFsRedTarget.Add(new PointF(7.5f,58));
				_listPointFsRedTarget.Add(new PointF(8.5f,89));
				_listPointFsRedTarget.Add(new PointF(9f,97));
				_listPointFsRedTarget.Add(new PointF(9.5f,100));
				_listPointFsRedTarget.Add(new PointF(10f,97));
				_listPointFsRedTarget.Add(new PointF(10.5f,97));
				_listPointFsRedTarget.Add(new PointF(11.5f,96));
				_listPointFsRedTarget.Add(new PointF(12.5f,92));
				_listPointFsRedTarget.Add(new PointF(13.5f,88));
				_listPointFsRedTarget.Add(new PointF(14.5f,77));
				_listPointFsRedTarget.Add(new PointF(15.5f,53));
				_listPointFsRedTarget.Add(new PointF(16.5f,43));
				_listPointFsRedTarget.Add(new PointF(17f,27));
				_listPointFsRedTarget.Add(new PointF(17.5f,13));
				_listPointFsRedTarget.Add(new PointF(17.5f,0));
			}
			else if(_dateShowing.DayOfWeek==DayOfWeek.Saturday || _dateShowing.DayOfWeek==DayOfWeek.Sunday) {
				//do nothing, no call curve to show yet
			}
			else {
				_listPointFsRedTarget.Add(new PointF(5f,3));
				_listPointFsRedTarget.Add(new PointF(5.5f,16));
				_listPointFsRedTarget.Add(new PointF(6.5f,39));
				_listPointFsRedTarget.Add(new PointF(7.5f,62));
				_listPointFsRedTarget.Add(new PointF(8.5f,88));
				_listPointFsRedTarget.Add(new PointF(9f,96));
				_listPointFsRedTarget.Add(new PointF(9.5f,100));//2000 is peak
				_listPointFsRedTarget.Add(new PointF(10f,98));
				_listPointFsRedTarget.Add(new PointF(10.5f,96));
				_listPointFsRedTarget.Add(new PointF(11.5f,95));
				_listPointFsRedTarget.Add(new PointF(12.5f,93));
				_listPointFsRedTarget.Add(new PointF(13.5f,92));
				_listPointFsRedTarget.Add(new PointF(14.5f,80));
				_listPointFsRedTarget.Add(new PointF(15.5f,60));
				_listPointFsRedTarget.Add(new PointF(16.5f,38));
				_listPointFsRedTarget.Add(new PointF(17.5f,15));
				_listPointFsRedTarget.Add(new PointF(18f,5));
				_listPointFsRedTarget.Add(new PointF(18f,0));
			}
			_floatArrayBuckets=new float[56];//Number of total bucket. 4 buckets per hour * 14 hours = 56 buckets.
			for(int i=0;i<_listSchedules.Count;i++) {
				if(_listSchedules[i].SchedType!=ScheduleType.Employee) {
					continue;
				}
				Employee employee=Employees.GetEmp(_listSchedules[i].EmployeeNum);
				if(employee==null) {//employees will NEVER be deleted. even after they cease to work here. but just in case.
					continue;
				}
				bool isGraphed=false; 
				PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==employee.EmployeeNum);
				if(phoneGraph!=null){
					isGraphed=phoneGraph.IsGraphed;
				}
				if(phoneGraph==null) {//no phone graph entry found (likely for a future date which does not have entries created yet OR past date where current employee didn't work here yet)
					if(_dateShowing<=DateTime.Today) {//no phone graph entry and we are on a past OR current date. if it's not already created then don't graph this employee for this date
						continue;
					}
					//we are on a future date AND we don't have a PhoneGraph entry explicitly set so use the default for this employee
					PhoneEmpDefault phoneEmpDefault=_listPhoneEmpDefaultsRaw.FirstOrDefault(x=>x.EmployeeNum==_listSchedules[i].EmployeeNum);
					if(phoneEmpDefault==null) {//we will default to PhoneEmpDefault.IsGraphed so make sure the default exists
						continue;
					}
					//no entry in PhoneGraph for the employee on this date so use the default
					isGraphed=phoneEmpDefault.IsGraphed;
				}
				if(!isGraphed) {//only care about employees that are being graphed
					continue;
				}
				if(phoneGraph!=null && phoneGraph.DateTimeStart1.Year>1880){
					continue;
					//time overrides are graphed in a separate loop
				}
				AddTimeToBuckets(_listSchedules[i].StartTime,_listSchedules[i].StopTime);
				//break;//just show one sched for debugging.
			}
			for(int i=0;i<_listPhoneGraphs.Count;i++) {
				if(_listPhoneGraphs[i].DateTimeStart1.Year<1880){
					continue;
					//whether it's graphed or not, this loop doesn't care about these
				}
				if(!_listPhoneGraphs[i].IsGraphed){
					continue;
				}
				AddTimeToBuckets(_listPhoneGraphs[i].DateTimeStart1.TimeOfDay,_listPhoneGraphs[i].DateTimeStop1.TimeOfDay);
				if(_listPhoneGraphs[i].DateTimeStart2.Year<1880){
					continue;
				}
				AddTimeToBuckets(_listPhoneGraphs[i].DateTimeStart2.TimeOfDay,_listPhoneGraphs[i].DateTimeStop2.TimeOfDay);
			}
			//missed calls
			//missedCalls=new int[28];
			//List<DateTime> callTimes=PhoneAsterisks.GetMissedCalls(dateShowing);
			//for(int i=0;i<callTimes.Count;i++) {
			//  for(int b=0;b<missedCalls.Length;b++) {
			//    time1=new TimeSpan(5,0,0) + new TimeSpan(0,b*30,0);
			//    time2=new TimeSpan(5,30,0) + new TimeSpan(0,b*30,0);
			//    if(callTimes[i].TimeOfDay >= time1 && callTimes[i].TimeOfDay < time2) {
			//      missedCalls[b]++;
			//    }
			//  }
			//}
			//Minutes Behind
			_intArrayMinutesBehind=PhoneMetrics.AverageMinutesBehind(_dateShowing);
			if(ODBuild.IsDebug() && Environment.MachineName=="JORDANHOME"){
				_intArrayMinutesBehind=new int[28];
				for(int i=0;i<28;i++){
					_intArrayMinutesBehind[i]=23;
				}
			}
			panelMain.Invalidate();
		}

		private void AddTimeToBuckets(TimeSpan timeStart,TimeSpan timeStop){
			TimeSpan timeSpanMin;
			TimeSpan timeSpanMax;
			TimeSpan delta;
			for(int b=0;b<_floatArrayBuckets.Length;b++) {
				//minutes field multiplier is a function of buckets per hour. answers the question, "how many minutes long is each bucket?"
				timeSpanMin=new TimeSpan(5,0,0) + new TimeSpan(0,b*15,0); 
				timeSpanMax=new TimeSpan(5,15,0) + new TimeSpan(0,b*15,0);
				//situation 1: this bucket is completely within the start and stop times.
				if(timeStart <= timeSpanMin && timeStop >= timeSpanMax) {
					_floatArrayBuckets[b]+=1;
				}
				//situation 2: the start time is after this bucket
				else if(timeStart >= timeSpanMax) {
					continue;
				}
				//situation 3: the stop time is before this bucket
				else if(timeStop <= timeSpanMin) {
					continue;
				}
				//situation 4: start time falls within this bucket
				if(timeStart > timeSpanMin) {
					delta=timeStart - timeSpanMin;
					//7.5 minutes is half of one bucket.
					if(delta.TotalMinutes > 7.5) { //has to work more than 15 minutes to be considered *in* this bucket
						_floatArrayBuckets[b]+=1;
					}
				}
				//situation 5: stop time falls within this bucket
				if(timeStop < timeSpanMax) {
					delta= timeSpanMax - timeStop;
					if(delta.TotalMinutes > 7.5) { //has to work more than 15 minutes to be considered *in* this bucket
						_floatArrayBuckets[b]+=1;
					}
				}
			}
		}

		private void PanelMain_Paint(object sender,PaintEventArgs e) {
			//_listRegions.Clear();
			e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
			int heightOfLabels=20;
			Rectangle rectangleGraph=new Rectangle(10,0,panelMain.Width-20,panelMain.Height-heightOfLabels);//padding of 10 on each side
			e.Graphics.FillRectangle(Brushes.White,panelMain.ClientRectangle);
			if(_listPointFsRedTarget==null) {
				return;
			}
			int totalhrs=14;
			float widthBar=rectangleGraph.Width/(totalhrs*4f);
			float widthHour=(float)rectangleGraph.Width/totalhrs;
			float x1;
			float y1;
			float x2;
			float y2;
			//X axis Numbers
			string str;
			float strW;
			for(int i=0;i<totalhrs+1;i++) {
				if(i<8) {
					str=(i+5).ToString();
				}
				else {
					str=(i-7).ToString();
				}
				strW=e.Graphics.MeasureString(str,Font).Width;
				e.Graphics.DrawString(str,Font,Brushes.Black,rectangleGraph.X+i*widthHour-strW/2,rectangleGraph.Bottom+3);
			}
			//find the biggest bar
			float peak=PIn.Int(PrefC.GetRaw("GraphEmployeeTimesPeak"));//The ideal peak.  Each day should look the same, except Friday.
			if(_dateShowing.DayOfWeek==DayOfWeek.Friday) {
				peak=peak*0.95f;//The Friday graph is actually smaller than the other graphs.
			}
			float superPeak=PIn.Int(PrefC.GetRaw("GraphEmployeeTimesSuperPeak"));//the most staff possible to schedule
			float hOne=rectangleGraph.Height/superPeak;
			//draw bars
			float x;
			float y;
			float w;
			float h;
			float barSpacing=rectangleGraph.Width/(totalhrs*4f); //4f means number of buckets per hours.  EG... 10 minute granularity = 6f;
			float firstBar=barSpacing / 2f;
			float barW=barSpacing / 1f;//increase denominator in order to increase spacing between bars. 1f = no space... 2f = full bar space. 1.5f = half bar space.
			using SolidBrush brushBlue=new SolidBrush(Color.FromArgb(162,193,222));
			for(int i=0;i<_floatArrayBuckets.Length;i++) {
				h=(float)_floatArrayBuckets[i]*rectangleGraph.Height/superPeak;
				x=rectangleGraph.X + firstBar + (float)i*barSpacing - barW/2f;
				y=rectangleGraph.Y+rectangleGraph.Height-h;
				w=barW;
				RectangleF rc = new RectangleF(x,y,w,h);
				e.Graphics.FillRectangle(Brushes.LightBlue,rc);
			}
			//Line graph in red
			float peakH=rectangleGraph.Height * peak / superPeak;
			using Pen penRed=new Pen(Brushes.Red,2f);
			for(int i=0;i<_listPointFsRedTarget.Count-1;i++) {
				x1=rectangleGraph.X + ((_listPointFsRedTarget[i].X-5f) * rectangleGraph.Width / totalhrs);
				y1=rectangleGraph.Y+rectangleGraph.Height - (_listPointFsRedTarget[i].Y / 100f * peakH);
				x2=rectangleGraph.X + ((_listPointFsRedTarget[i+1].X-5f) * rectangleGraph.Width / totalhrs);
				y2=rectangleGraph.Y+rectangleGraph.Height - (_listPointFsRedTarget[i+1].Y / 100f * peakH);
				e.Graphics.DrawLine(penRed,x1,y1,x2,y2);
			}
			//Minutes behind numbers
			for(int i=0;i<_intArrayMinutesBehind.Length;i++) {
				if(_intArrayMinutesBehind[i]==0) {
					continue;
				}
				str=_intArrayMinutesBehind[i].ToString();
				strW=e.Graphics.MeasureString(str,Font).Width;
				x1=rectangleGraph.X + barW + ((float)i * rectangleGraph.Width / totalhrs / 2) - strW / 2f;
				y1=rectangleGraph.Bottom-(17);//*2);
				e.Graphics.DrawString(str,Font,Brushes.Red,x1,y1);
			}
			//Grid lines
			for(int i=0;i<totalhrs;i++) {
				e.Graphics.DrawLine(Pens.Black,rectangleGraph.X+i*widthHour,0,rectangleGraph.X+i*widthHour,rectangleGraph.Bottom);//vertical
			}
			e.Graphics.DrawLine(Pens.Black,0,rectangleGraph.Bottom,panelMain.Width,rectangleGraph.Bottom);//horizontal
			//Vertical red line for current time
			if(DateTime.Today.Date==_dateShowing.Date) {
				TimeSpan timeSpanNow=DateTime.Now.AddHours(-5).TimeOfDay;
				float shift=(float)timeSpanNow.TotalHours * rectangleGraph.Width / totalhrs;
				x1=rectangleGraph.X + shift;
				y1=rectangleGraph.Y+rectangleGraph.Height;
				x2=rectangleGraph.X + shift;
				y2=rectangleGraph.Y;
				e.Graphics.DrawLine(Pens.Red,x1,y1,x2,y2);
			}
		}

		private void butPrefs_Click(object sender,EventArgs e) {
			using FormPhoneGraphDateEdit formPhoneGraphDateEdit=new FormPhoneGraphDateEdit(_dateShowing);
			formPhoneGraphDateEdit.ShowDialog();
			RefreshDataForDay(); //always refill, we may have new entries regardless of form dialog result
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrinterL.TryPrint(pd2_PrintPage,Lan.g(this,"Employee time graph printed"));
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			e.Graphics.DrawString(labelDate.Text,labelDate.Font,Brushes.Black,350,120);
			using Bitmap bitmap=new Bitmap(panelMain.ClientSize.Width,panelMain.ClientSize.Height);
			panelMain.DrawToBitmap(bitmap,new Rectangle(0,0,bitmap.Width,bitmap.Height));
			e.Graphics.DrawImage(bitmap,50,200);
		}
		#endregion Graph

		private void monthCalendarOD_DateChanged(object sender, EventArgs e){
			_dateShowing=monthCalendarOD.GetDateSelected();
			RefreshDataForDay();
			FillGrid();
		}

		private void buttonLeft_Click(object sender,EventArgs e) {
			_dateShowing=_dateShowing.AddDays(-1);
			monthCalendarOD.SetDateSelected(_dateShowing);
			RefreshDataForDay();
			FillGrid();
		}

		private void butToday_Click(object sender, EventArgs e){
			_dateShowing=DateTime.Today;
			monthCalendarOD.SetDateSelected(_dateShowing);
			RefreshDataForDay();
			FillGrid();
		}

		private void buttonRight_Click(object sender,EventArgs e) {
			_dateShowing=_dateShowing.AddDays(1);
			monthCalendarOD.SetDateSelected(_dateShowing);
			RefreshDataForDay();
			FillGrid();
		}

		private void ButMySchedule_Click(object sender,EventArgs e) {
			using FormGraphEmployeeView formGraphEmployeeView=new FormGraphEmployeeView();
			formGraphEmployeeView.ShowDialog();
			if(formGraphEmployeeView.DialogResult!=DialogResult.OK){
				return;
			}
		}

		private void textFilterName_TextChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void comboSupervisor_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void checkGraphed_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void checkScheduled_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void textTime_TextChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void radioAM_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void radioPM_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void listOrder_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void checkOffAtTop_Click(object sender, EventArgs e){
			FillGrid();
		}

		private void butRefresh_Click(object sender, EventArgs e){
			RefreshDataForDay();
			FillGrid();
		}

		private void butEditDaily_Click(object sender, EventArgs e){
			using FormPhoneGraphAdminEdit formPhoneGraphAdminEdit=new FormPhoneGraphAdminEdit();
			if(_phoneGraphDay==null){
				_phoneGraphDay=new PhoneGraph();
				_phoneGraphDay.IsNew=true;
				_phoneGraphDay.DateEntry=_dateShowing;
			}
			formPhoneGraphAdminEdit.PhoneGraphCur=_phoneGraphDay;
			formPhoneGraphAdminEdit.ShowDialog();
			if(formPhoneGraphAdminEdit.DialogResult!=DialogResult.OK){
				return;
			}
			RefreshDataForDay();
		}

		private void FillGrid() {
			List<Employee> listEmployees=Employees.GetDeepCopy();
			_listPhoneEmpDefaultsShowing=new List<PhoneEmpDefault>();
			for(int i=0;i<_listPhoneEmpDefaultsRaw.Count;i++){
				if(_listPhoneEmpDefaultsRaw[i].EmployeeNum==0){
					continue;
				}
				PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==_listPhoneEmpDefaultsRaw[i].EmployeeNum);
				if(checkScheduled.Checked){
					if(!_listSchedules.Exists(x => x.EmployeeNum==_listPhoneEmpDefaultsRaw[i].EmployeeNum)){//no normal sched
						if(phoneGraph==null || phoneGraph.DateTimeStart1.Year<1880){//and no override added
							continue;
						}
					}
				}
				if(textFilterName.Text!="" && !_listPhoneEmpDefaultsRaw[i].EmpName.ToLower().StartsWith(textFilterName.Text.ToLower())){
					continue;
				}
				long empNumSuper=comboSupervisor.GetSelectedKey<Employee>(x=>x.EmployeeNum);
				if(empNumSuper!=0){//Any
					if(_listPhoneEmpDefaultsRaw[i].EmployeeNum==0){
						continue;
					}
					Employee employee=Employees.GetEmp(_listPhoneEmpDefaultsRaw[i].EmployeeNum);
					if(employee==null || employee.ReportsTo!=empNumSuper){
						continue;
					}
				}
				if(checkGraphed.Checked){
					if(!_listPhoneEmpDefaultsRaw[i].IsGraphed){//maybe include a check for override?
						continue;
					}
				}
				if(textTime.Text!=""){
					TimeSpan timeSpan;
					bool Isparsed=TimeSpan.TryParseExact(textTime.Text,"h\\:mm",null,out timeSpan);
					if(!Isparsed){
						Isparsed=TimeSpan.TryParseExact(textTime.Text,"%h",null,out timeSpan);
					}
					if(!Isparsed){
						return;//don't even finish refreshing the grid if this is invalid
					}
					if(timeSpan==TimeSpan.FromHours(1)){
						return;//don't refresh the grid while trying to type a 2 digit hour
					}
					if(radioPM.Checked && timeSpan<TimeSpan.FromHours(12)){
						timeSpan+=TimeSpan.FromHours(12);
					}
					if(phoneGraph!=null && phoneGraph.DateTimeStart1.Year>1880){
						if(timeSpan<phoneGraph.DateTimeStart1.TimeOfDay || timeSpan>phoneGraph.DateTimeStop1.TimeOfDay){
							if(timeSpan<phoneGraph.DateTimeStart2.TimeOfDay || timeSpan>phoneGraph.DateTimeStop2.TimeOfDay){
								continue;//not within either timespan
							}
						}
					}
					else{//use schedule
						if(!_listSchedules.Exists(x => x.EmployeeNum==_listPhoneEmpDefaultsRaw[i].EmployeeNum && timeSpan>=x.StartTime && timeSpan<=x.StopTime)){
							continue;
						}
					}
				}
				_listPhoneEmpDefaultsShowing.Add(_listPhoneEmpDefaultsRaw[i].Clone());
			}
			if(checkOffAtTop.Checked){
				int isAbsent(long empNum){
					PhoneGraph phoneGraph = _listPhoneGraphs.Find(x => x.EmployeeNum == empNum);
					if(phoneGraph is null){
						return 2;
					}
					if(phoneGraph.Absent){
						return 1;//below presched
					}
					if(phoneGraph.PreSchedOff){
						return 0;//top most
					}
					return 2;//all the rest
				}
				switch (listOrder.SelectedIndex){
					case 0:
						_listPhoneEmpDefaultsShowing=_listPhoneEmpDefaultsShowing
							.OrderBy(x=>isAbsent(x.EmployeeNum))
							.ThenBy(x=>x.EmpName).ToList();
						break;
					case 1:
						_listPhoneEmpDefaultsShowing=_listPhoneEmpDefaultsShowing
							.OrderBy(x=>isAbsent(x.EmployeeNum))
							.ThenBy(x=>GetStartTime(x)).ToList();
						break;
					case 2:
						_listPhoneEmpDefaultsShowing=_listPhoneEmpDefaultsShowing
							.OrderBy(x=>isAbsent(x.EmployeeNum))
							.ThenBy(x=>GetStopTime(x)).ToList();
						break;
				}
			}
			else{
				switch(listOrder.SelectedIndex){
					case 0:
						_listPhoneEmpDefaultsShowing=_listPhoneEmpDefaultsShowing.OrderBy(x=>x.EmpName).ToList();
						break;
					case 1:
						_listPhoneEmpDefaultsShowing=_listPhoneEmpDefaultsShowing.OrderBy(x=>GetStartTime(x)).ToList();
						break;
					case 2:
						_listPhoneEmpDefaultsShowing=_listPhoneEmpDefaultsShowing.OrderBy(x=>GetStopTime(x)).ToList();
						break;
				}
			}
			long selectedEmployeeNum=-1;
			if(gridMain.ListGridRows.Count>=1 
				&& gridMain.GetSelectedIndex()>=0 
				&& gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag!=null 
				&& gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag is PhoneEmpDefault) 
			{
					selectedEmployeeNum=((PhoneEmpDefault)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).EmployeeNum;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePhoneGraphDate","Name"),80);
			gridMain.Columns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Graphed"),55);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Schedule"),220);
			gridMain.Columns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Presch"),50);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.Columns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Note"),210);
			gridMain.Columns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","TimeClock"),120);
			gridMain.Columns.Add(col); 
			gridMain.ListGridRows.Clear();
			int selectedRow=-1;
			//loop through all employee defaults and create 1 row per employee
			for(int i=0;i<_listPhoneEmpDefaultsShowing.Count;i++) {
				PhoneEmpDefault phoneEmpDefault=_listPhoneEmpDefaultsShowing[i];
				List<Schedule> listSchedulesEmp=Schedules.GetForEmployee(_listSchedules,phoneEmpDefault.EmployeeNum);			
				GridRow row;
				GridCell cell;
				row=new GridRow();
				row.Cells.Add(phoneEmpDefault.EmpName); 
				PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum);
				if(phoneGraph==null){
					row.Cells.Add(phoneEmpDefault.IsGraphed?"X":"");
				}
				else{
					if(phoneEmpDefault.IsGraphed==phoneGraph.IsGraphed){
						row.Cells.Add(phoneEmpDefault.IsGraphed?"X":"");
					}
					else{//override different from default
						GridCell gridCell=new GridCell(phoneGraph.IsGraphed?"X":"\u2014");//dash
						gridCell.ColorText=Color.Red;
						row.Cells.Add(gridCell);
					}
				}
				if(phoneGraph!=null && phoneGraph.DateTimeStart1.Year>1880){
					cell=new GridCell(PhoneGraphs.GetCommaDelimStringTimes(phoneGraph));
					cell.ColorText=Color.Red;
					row.Cells.Add(cell);
				}
				else if(phoneGraph!=null && phoneGraph.Absent){
					cell=new GridCell("Absent");
					cell.ColorText=Color.Red;
					row.Cells.Add(cell);
				}
				else if(phoneGraph!=null && phoneGraph.PreSchedOff){
					cell=new GridCell("Prescheduled Off");
					cell.ColorText=Color.Red;
					row.Cells.Add(cell);
				}
				else{
					row.Cells.Add(Schedules.GetCommaDelimStringForScheds(listSchedulesEmp)); 
				}
				if(phoneGraph==null){
					row.Cells.Add("");
				}
				else{
					if(phoneGraph.PreSchedOff){
						row.Cells.Add("X");
					}
					else if(phoneGraph.PreSchedTimes==EnumPresched.Presched && phoneGraph.DateTimeStart1.Year>1880){
						row.Cells.Add("X");
					}
					else{
						row.Cells.Add("");
					}
				}
				if(phoneGraph==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(phoneGraph.Note);
				}
				Employee employee=listEmployees.FirstOrDefault(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum);
				if(employee==null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(employee.ClockStatus);
				}
				row.Tag=phoneEmpDefault;
				gridMain.ListGridRows.Add(row);
				if(selectedEmployeeNum==phoneEmpDefault.EmployeeNum) {
					selectedRow=i;
				}
			}
			gridMain.EndUpdate();
			if(selectedRow>=0) {
				gridMain.SetSelected(selectedRow,true);
			}
		}

		///<summary>For sorting</summary>
		private TimeSpan GetStartTime(PhoneEmpDefault phoneEmpDefault){
			PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum);
			if(phoneGraph!=null && phoneGraph.DateTimeStart1.Year>1880){
				return phoneGraph.DateTimeStart1.TimeOfDay;
			}
			Schedule schedule=_listSchedules.FindAll(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum).OrderBy(x=>x.StartTime).FirstOrDefault();
			if(schedule!=null){
				return schedule.StartTime;
			}
			return TimeSpan.Zero;
		}

		///<summary>For sorting</summary>
		private TimeSpan GetStopTime(PhoneEmpDefault phoneEmpDefault){
			PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum);
			if(phoneGraph!=null){
				if(phoneGraph.DateTimeStop2.Year>1880){
					return phoneGraph.DateTimeStop2.TimeOfDay;
				}
				if(phoneGraph.DateTimeStop1.Year>1880){
					return phoneGraph.DateTimeStop1.TimeOfDay;
				}
			}
			Schedule schedule=_listSchedules.FindAll(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum).OrderByDescending(x=>x.StartTime).FirstOrDefault();
			if(schedule!=null){
				return schedule.StopTime;
			}
			return TimeSpan.Zero;
		}

		private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e){
			PhoneEmpDefault phoneEmpDefault=(PhoneEmpDefault)gridMain.ListGridRows[e.Row].Tag;
			PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum);
			if(phoneGraph==null){
				phoneGraph=new PhoneGraph();
				phoneGraph.EmployeeNum=phoneEmpDefault.EmployeeNum;
				phoneGraph.DateEntry=_dateShowing;
				phoneGraph.IsNew=true;
				phoneGraph.IsGraphed=phoneEmpDefault.IsGraphed;
			}
			using FormPhoneGraphEdit formPhoneGraphEdit=new FormPhoneGraphEdit();
			formPhoneGraphEdit.PhoneGraphCur=phoneGraph;
			formPhoneGraphEdit.ListSchedulesEmp=_listSchedules.FindAll(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum);
			Provider provider=Providers.GetFirstOrDefault(x=>x.FName==phoneEmpDefault.EmpName);
			if(provider!=null){
				formPhoneGraphEdit.ProvNum=provider.ProvNum;
				formPhoneGraphEdit.ListSchedulesProv=_listSchedules.FindAll(x=>x.ProvNum==provider.ProvNum && x.SchedType==ScheduleType.Provider);
			}
			formPhoneGraphEdit.ShowDialog();
			if(formPhoneGraphEdit.DialogResult!=DialogResult.OK){
				return;
			}
			RefreshDataForDay();
			FillGrid();
		}

		private void butEditDefaults_Click(object sender, EventArgs e){
			int selectedIdx=gridMain.GetSelectedIndex();
			if(selectedIdx==-1){
				MsgBox.Show("Please select an employee first.");
				return;
			}
			using FormPhoneEmpDefaultEdit formPhoneEmpDefaultEdit=new FormPhoneEmpDefaultEdit();
			formPhoneEmpDefaultEdit.PedCur=(PhoneEmpDefault)gridMain.ListGridRows[selectedIdx].Tag;
			formPhoneEmpDefaultEdit.ShowDialog();
			RefreshDataForDay();
			FillGrid();
		}

		private void butPrintList_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Daily Graph List Printed"),PrintoutOrientation.Portrait);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text=$"{gridMain.Title} - Reports To: {comboSupervisor.GetStringSelectedItems()}" ;
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				//print today's date
				text = Lan.g(this,"Run On:")+" "+DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				text=Lan.g(this,"Selected Date:")+" "+_dateShowing.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}