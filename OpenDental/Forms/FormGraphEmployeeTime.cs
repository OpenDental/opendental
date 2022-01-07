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
		private List<PointF> _listPointsRedTarget;
		private float[] buckets;//a bucket can hold partial people.
		private DateTime _dateShowing;
		private int[] minutesBehind;
		///<summary>Local copy of cache. Only three fields are used: EmployeeNum, EmpName, IsGraphed.</summary>
		private List<PhoneEmpDefault> _listPEDsRaw;
		///<summary>This is what shows in the grid.</summary>
		private List<PhoneEmpDefault> _listPEDsShowing;
		private List<PhoneGraph> _listPhoneGraphs;
		private List<Schedule> _listSchedules;
		///<summary>This one doesn't have an emp Num. Can be null.</summary>
		private PhoneGraph _phoneGraphDay;
		private bool _isAtDailyLimit;

		public FormGraphEmployeeTime(DateTime dateShowing) {
			InitializeComponent();
			InitializeLayoutManager();
			_dateShowing=dateShowing;
			Lan.F(this);
			toolTip.ToolTipTitle=Lan.g(this,"Employees");
			//_listRegions=new List<Region>();
		}

		private void FormGraphEmployeeTime_Load(object sender,EventArgs e) {
			butPrefs.Enabled=Security.IsAuthorized(Permissions.Schedules,true);
			butEditDaily.Enabled=Security.IsAuthorized(Permissions.Schedules,true);
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
			_listPEDsRaw=PhoneEmpDefaults.GetDeepCopy();
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
				int count=_listPhoneGraphs.FindAll(x=>x.PreSchedOff && isCallCenter(x.EmployeeNum) && _listSchedules.Exists(y => y.EmployeeNum==x.EmployeeNum)).Count();
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
			_listPointsRedTarget=new List<PointF>();
			//For each "point", the first val is time of day, and second val is based on a peak of 100
			if(_dateShowing.DayOfWeek==DayOfWeek.Friday) {
				_listPointsRedTarget.Add(new PointF(5f,7));
				_listPointsRedTarget.Add(new PointF(5.5f,19));
				_listPointsRedTarget.Add(new PointF(6.5f,39));
				_listPointsRedTarget.Add(new PointF(7.5f,58));
				_listPointsRedTarget.Add(new PointF(8.5f,89));
				_listPointsRedTarget.Add(new PointF(9f,97));
				_listPointsRedTarget.Add(new PointF(9.5f,100));
				_listPointsRedTarget.Add(new PointF(10f,97));
				_listPointsRedTarget.Add(new PointF(10.5f,97));
				_listPointsRedTarget.Add(new PointF(11.5f,96));
				_listPointsRedTarget.Add(new PointF(12.5f,92));
				_listPointsRedTarget.Add(new PointF(13.5f,88));
				_listPointsRedTarget.Add(new PointF(14.5f,77));
				_listPointsRedTarget.Add(new PointF(15.5f,53));
				_listPointsRedTarget.Add(new PointF(16.5f,43));
				_listPointsRedTarget.Add(new PointF(17f,27));
				_listPointsRedTarget.Add(new PointF(17.5f,13));
				_listPointsRedTarget.Add(new PointF(17.5f,0));
			}
			else if(_dateShowing.DayOfWeek==DayOfWeek.Saturday || _dateShowing.DayOfWeek==DayOfWeek.Sunday) {
				//do nothing, no call curve to show yet
			}
			else {
				_listPointsRedTarget.Add(new PointF(5f,3));
				_listPointsRedTarget.Add(new PointF(5.5f,16));
				_listPointsRedTarget.Add(new PointF(6.5f,39));
				_listPointsRedTarget.Add(new PointF(7.5f,62));
				_listPointsRedTarget.Add(new PointF(8.5f,88));
				_listPointsRedTarget.Add(new PointF(9f,96));
				_listPointsRedTarget.Add(new PointF(9.5f,100));//2000 is peak
				_listPointsRedTarget.Add(new PointF(10f,98));
				_listPointsRedTarget.Add(new PointF(10.5f,96));
				_listPointsRedTarget.Add(new PointF(11.5f,95));
				_listPointsRedTarget.Add(new PointF(12.5f,93));
				_listPointsRedTarget.Add(new PointF(13.5f,92));
				_listPointsRedTarget.Add(new PointF(14.5f,80));
				_listPointsRedTarget.Add(new PointF(15.5f,60));
				_listPointsRedTarget.Add(new PointF(16.5f,38));
				_listPointsRedTarget.Add(new PointF(17.5f,15));
				_listPointsRedTarget.Add(new PointF(18f,5));
				_listPointsRedTarget.Add(new PointF(18f,0));
			}
			buckets=new float[56];//Number of total bucket. 4 buckets per hour * 14 hours = 56 buckets.
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
					PhoneEmpDefault ped=_listPEDsRaw.FirstOrDefault(x=>x.EmployeeNum==_listSchedules[i].EmployeeNum);
					if(ped==null) {//we will default to PhoneEmpDefault.IsGraphed so make sure the default exists
						continue;
					}
					//no entry in PhoneGraph for the employee on this date so use the default
					isGraphed=ped.IsGraphed;
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
			minutesBehind=PhoneMetrics.AverageMinutesBehind(_dateShowing);
			if(ODBuild.IsDebug() && Environment.MachineName=="JORDANHOME"){
				minutesBehind=new int[28];
				for(int i=0;i<28;i++){
					minutesBehind[i]=23;
				}
			}
			panelMain.Invalidate();
		}

		private void AddTimeToBuckets(TimeSpan timeStart,TimeSpan timeStop){
			TimeSpan time1;
			TimeSpan time2;
			TimeSpan delta;
			for(int b=0;b<buckets.Length;b++) {
				//minutes field multiplier is a function of buckets per hour. answers the question, "how many minutes long is each bucket?"
				time1=new TimeSpan(5,0,0) + new TimeSpan(0,b*15,0); 
				time2=new TimeSpan(5,15,0) + new TimeSpan(0,b*15,0);
				//situation 1: this bucket is completely within the start and stop times.
				if(timeStart <= time1 && timeStop >= time2) {
					buckets[b]+=1;
				}
				//situation 2: the start time is after this bucket
				else if(timeStart >= time2) {
					continue;
				}
				//situation 3: the stop time is before this bucket
				else if(timeStop <= time1) {
					continue;
				}
				//situation 4: start time falls within this bucket
				if(timeStart > time1) {
					delta=timeStart - time1;
					//7.5 minutes is half of one bucket.
					if(delta.TotalMinutes > 7.5) { //has to work more than 15 minutes to be considered *in* this bucket
						buckets[b]+=1;
					}
				}
				//situation 5: stop time falls within this bucket
				if(timeStop < time2) {
					delta= time2 - timeStop;
					if(delta.TotalMinutes > 7.5) { //has to work more than 15 minutes to be considered *in* this bucket
						buckets[b]+=1;
					}
				}
			}
		}

		private void PanelMain_Paint(object sender,PaintEventArgs e) {
			//_listRegions.Clear();
			e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
			int heightXLabels=20;
			Rectangle rectangleGraph=new Rectangle(10,0,panelMain.Width-20,panelMain.Height-heightXLabels);//padding of 10 on each side
			e.Graphics.FillRectangle(Brushes.White,panelMain.ClientRectangle);
			if(_listPointsRedTarget==null) {
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
			float barspacing=rectangleGraph.Width/(totalhrs*4f); //4f means number of buckets per hours.  EG... 10 minute granularity = 6f;
			float firstbar=barspacing / 2f;
			float barW=barspacing / 1f;//increase denominator in order to increase spacing between bars. 1f = no space... 2f = full bar space. 1.5f = half bar space.
			SolidBrush brushBlue=new SolidBrush(Color.FromArgb(162,193,222));
			for(int i=0;i<buckets.Length;i++) {
				h=(float)buckets[i]*rectangleGraph.Height/superPeak;
				x=rectangleGraph.X + firstbar + (float)i*barspacing - barW/2f;
				y=rectangleGraph.Y+rectangleGraph.Height-h;
				w=barW;
				RectangleF rc = new RectangleF(x,y,w,h);
				e.Graphics.FillRectangle(Brushes.LightBlue,rc);
			}
			//Line graph in red
			float peakH=rectangleGraph.Height * peak / superPeak;
			Pen redPen=new Pen(Brushes.Red,2f);
			for(int i=0;i<_listPointsRedTarget.Count-1;i++) {
				x1=rectangleGraph.X + ((_listPointsRedTarget[i].X-5f) * rectangleGraph.Width / totalhrs);
				y1=rectangleGraph.Y+rectangleGraph.Height - (_listPointsRedTarget[i].Y / 100f * peakH);
				x2=rectangleGraph.X + ((_listPointsRedTarget[i+1].X-5f) * rectangleGraph.Width / totalhrs);
				y2=rectangleGraph.Y+rectangleGraph.Height - (_listPointsRedTarget[i+1].Y / 100f * peakH);
				e.Graphics.DrawLine(redPen,x1,y1,x2,y2);
			}
			//Minutes behind numbers
			for(int i=0;i<minutesBehind.Length;i++) {
				if(minutesBehind[i]==0) {
					continue;
				}
				str=minutesBehind[i].ToString();
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
				TimeSpan now=DateTime.Now.AddHours(-5).TimeOfDay;
				float shift=(float)now.TotalHours * rectangleGraph.Width / totalhrs;
				x1=rectangleGraph.X + shift;
				y1=rectangleGraph.Y+rectangleGraph.Height;
				x2=rectangleGraph.X + shift;
				y2=rectangleGraph.Y;
				e.Graphics.DrawLine(Pens.Red,x1,y1,x2,y2);
			}
			redPen.Dispose();
			brushBlue.Dispose();
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
			Bitmap bitmap=new Bitmap(panelMain.ClientSize.Width,panelMain.ClientSize.Height);
			panelMain.DrawToBitmap(bitmap,new Rectangle(0,0,bitmap.Width,bitmap.Height));
			e.Graphics.DrawImage(bitmap,50,200);
		}
		#endregion Graph

		private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e){
			
		}

		private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e){
			_dateShowing=monthCalendar1.SelectionStart;
			RefreshDataForDay();
			FillGrid();
		}

		private void buttonLeft_Click(object sender,EventArgs e) {
			_dateShowing=_dateShowing.AddDays(-1);
			monthCalendar1.SelectionStart=_dateShowing;
			RefreshDataForDay();
			FillGrid();
		}

		private void butToday_Click(object sender, EventArgs e){
			_dateShowing=DateTime.Today;
			monthCalendar1.SelectionStart=_dateShowing;
			RefreshDataForDay();
			FillGrid();
		}

		private void buttonRight_Click(object sender,EventArgs e) {
			_dateShowing=_dateShowing.AddDays(1);
			monthCalendar1.SelectionStart=_dateShowing;
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
			_listPEDsShowing=new List<PhoneEmpDefault>();
			for(int i=0;i<_listPEDsRaw.Count;i++){
				if(_listPEDsRaw[i].EmployeeNum==0){
					continue;
				}
				PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==_listPEDsRaw[i].EmployeeNum);
				if(checkScheduled.Checked){
					if(!_listSchedules.Exists(x => x.EmployeeNum==_listPEDsRaw[i].EmployeeNum)){//no normal sched
						if(phoneGraph==null || phoneGraph.DateTimeStart1.Year<1880){//and no override added
							continue;
						}
					}
				}
				if(textFilterName.Text!="" && !_listPEDsRaw[i].EmpName.ToLower().StartsWith(textFilterName.Text.ToLower())){
					continue;
				}
				long empNumSuper=comboSupervisor.GetSelectedKey<Employee>(x=>x.EmployeeNum);
				if(empNumSuper!=0){//Any
					if(_listPEDsRaw[i].EmployeeNum==0){
						continue;
					}
					Employee employee=Employees.GetEmp(_listPEDsRaw[i].EmployeeNum);
					if(employee==null || employee.ReportsTo!=empNumSuper){
						continue;
					}
				}
				if(checkGraphed.Checked){
					if(!_listPEDsRaw[i].IsGraphed){//maybe include a check for override?
						continue;
					}
				}
				if(textTime.Text!=""){
					TimeSpan time;
					bool parsed=TimeSpan.TryParseExact(textTime.Text,"h\\:mm",null,out time);
					if(!parsed){
						parsed=TimeSpan.TryParseExact(textTime.Text,"%h",null,out time);
					}
					if(!parsed){
						return;//don't even finish refreshing the grid if this is invalid
					}
					if(time==TimeSpan.FromHours(1)){
						return;//don't refresh the grid while trying to type a 2 digit hour
					}
					if(radioPM.Checked && time<TimeSpan.FromHours(12)){
						time+=TimeSpan.FromHours(12);
					}
					if(phoneGraph!=null && phoneGraph.DateTimeStart1.Year>1880){
						if(time<phoneGraph.DateTimeStart1.TimeOfDay || time>phoneGraph.DateTimeStop1.TimeOfDay){
							if(time<phoneGraph.DateTimeStart2.TimeOfDay || time>phoneGraph.DateTimeStop2.TimeOfDay){
								continue;//not within either timespan
							}
						}
					}
					else{//use schedule
						if(!_listSchedules.Exists(x => x.EmployeeNum==_listPEDsRaw[i].EmployeeNum && time>=x.StartTime && time<=x.StopTime)){
							continue;
						}
					}
				}
				_listPEDsShowing.Add(_listPEDsRaw[i].Clone());
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
						_listPEDsShowing=_listPEDsShowing.OrderBy(x=>isAbsent(x.EmployeeNum)).ThenBy(x=>x.EmpName).ToList();
						break;
					case 1:
						_listPEDsShowing=_listPEDsShowing.OrderBy(x=>isAbsent(x.EmployeeNum)).ThenBy(x=>GetStartTime(x)).ToList();
						break;
					case 2:
						_listPEDsShowing=_listPEDsShowing.OrderBy(x=>isAbsent(x.EmployeeNum)).ThenBy(x=>GetStopTime(x)).ToList();
						break;
				}
			}
			else{
				switch(listOrder.SelectedIndex){
					case 0:
						_listPEDsShowing=_listPEDsShowing.OrderBy(x=>x.EmpName).ToList();
						break;
					case 1:
						_listPEDsShowing=_listPEDsShowing.OrderBy(x=>GetStartTime(x)).ToList();
						break;
					case 2:
						_listPEDsShowing=_listPEDsShowing.OrderBy(x=>GetStopTime(x)).ToList();
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePhoneGraphDate","Name"),80);
			gridMain.ListGridColumns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Graphed"),55);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Schedule"),220);
			gridMain.ListGridColumns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Presch"),50);
			col.TextAlign=HorizontalAlignment.Center;
			gridMain.ListGridColumns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","Note"),210);
			gridMain.ListGridColumns.Add(col); 
			col=new GridColumn(Lan.g("TablePhoneGraphDate","TimeClock"),120);
			gridMain.ListGridColumns.Add(col); 
			gridMain.ListGridRows.Clear();
			int selectedRow=-1;
			//loop through all employee defaults and create 1 row per employee
			for(int i=0;i<_listPEDsShowing.Count;i++) {
				PhoneEmpDefault phoneEmpDefault=_listPEDsShowing[i];
				List<Schedule> listSchedulesOne=Schedules.GetForEmployee(_listSchedules,phoneEmpDefault.EmployeeNum);			
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
					row.Cells.Add(Schedules.GetCommaDelimStringForScheds(listSchedulesOne)); 
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
			PhoneEmpDefault phoneEmpDefaultLim=(PhoneEmpDefault)gridMain.ListGridRows[e.Row].Tag;
			PhoneGraph phoneGraph=_listPhoneGraphs.Find(x=>x.EmployeeNum==phoneEmpDefaultLim.EmployeeNum);
			if(phoneGraph==null){
				phoneGraph=new PhoneGraph();
				phoneGraph.EmployeeNum=phoneEmpDefaultLim.EmployeeNum;
				phoneGraph.DateEntry=_dateShowing;
				phoneGraph.IsNew=true;
				phoneGraph.IsGraphed=phoneEmpDefaultLim.IsGraphed;
			}
			using FormPhoneGraphEdit formPhoneGraphEdit=new FormPhoneGraphEdit();
			formPhoneGraphEdit.PhoneGraphCur=phoneGraph;
			formPhoneGraphEdit.ListSchedulesEmp=_listSchedules.FindAll(x=>x.EmployeeNum==phoneEmpDefaultLim.EmployeeNum);
			Provider provider=Providers.GetFirstOrDefault(x=>x.FName==phoneEmpDefaultLim.EmpName);
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
			using FormPhoneEmpDefaultEdit FormPED=new FormPhoneEmpDefaultEdit();
			FormPED.PedCur=(PhoneEmpDefault)gridMain.ListGridRows[selectedIdx].Tag;
			FormPED.ShowDialog();
			RefreshDataForDay();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		


	}
}