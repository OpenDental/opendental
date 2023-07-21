using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormTimeCard:FormODBase {
		///<summary>True to default to viewing breaks. False for regular hours.</summary>
		public bool IsBreaks;
		///<summary>Server time minus local computer time, usually +/- 1 or 2 minutes</summary>
		private TimeSpan _timeSpanDelta;
		private List<ClockEvent> _listClockEvents;
		public int IdxPayPeriodSelected;
		private List<TimeAdjust> _listTimeAdjusts;
		private int _linesPrinted;
		//private OpenDental.UI.PrintPreview printPreview;
		///<summary>An array list of ojects representing the rows in the table. Can be either clockEvents or timeAdjusts.</summary>
		private ArrayList _arrayListMerged;
		///<summary>The running weekly total, whether it gets displayed or not.</summary>
		private TimeSpan[] _timeSpanArrayWeeklyTotals;
		public Employee EmployeeCur;
		///<summary>Used to determine the order to advance through employee timecards in this window.</summary>
		public bool IsByLastName;
		///<summary>Cached list of employees sorted based on IsByLastName</summary>
		private List<Employee> _listEmployees=new List<Employee>();
		///<summary>Filled when FillMain is called and fromDB=true.  If fromDB is false, we used this stored value from before instead to reduce calls to DB.  Because fillgrid does math on weekspan, this is a temporary cache of the last time we calculated it from the database.</summary>
		private TimeSpan _timeSpanStoredWeek;
		private TimeAdjust _timeAdjustNote;
		private List<PayPeriod> _listPayPeriods;

		///<summary>If true, the current employee cannot edit their own time card.</summary>
		private bool _cannotEditOwnTimecard {
			get {
				return _isTimeCardSecurityApplicable &&
					PrefC.GetBool(PrefName.TimecardUsersDontEditOwnCard);
			}
		}

		///<summary>If true, Time Card Security is enabled and should be considered for the current user.</summary>
		private bool _isTimeCardSecurityApplicable {
			get{
				return Security.CurUser!=null &&
				Security.CurUser.EmployeeNum==EmployeeCur.EmployeeNum &&
				PrefC.GetBool(PrefName.TimecardSecurityEnabled);
			} 
		}

		/// <summary>If true, the current employee can only edit their timecard for the current pay period</summary>
		private bool _cannotEditSelectedPayPeriod {
			get {
				return _isTimeCardSecurityApplicable &&
					PrefC.GetBool(PrefName.TimecardUsersCantEditPastPayPeriods) &&
					IdxPayPeriodSelected!=PayPeriods.GetForDate(DateTime.Today);
			}
		}

		///<summary></summary>
		public FormTimeCard(List<Employee> listEmployees)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listEmployees=listEmployees;
		}

		private void FormTimeCard_Load(object sender, System.EventArgs e){
			Initialize(DateTime.Today);
			SortEmployeeList();
			if(Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
				groupEmployee.Visible=true;
			}
		}

		public void SortEmployeeList() {
			if(IsByLastName) {
				_listEmployees.Sort(Employees.SortByLastName);
			}
			else {
				_listEmployees.Sort(Employees.SortByFirstName);
			}
		}

		/// <summary>Returns title of FormTimeCard based on employee name and any restrictions that may apply to editing.</summary>
		private string GetTitle() {
			string textString=Lan.g(this,"Time Card for")+" "+EmployeeCur.FName+" "+EmployeeCur.LName;
			if(_cannotEditOwnTimecard) {
				textString+=Lan.g(this," - You cannot modify your time card");
			}
			else if(_cannotEditSelectedPayPeriod) {
				int currentPayPeriod=PayPeriods.GetForDate(DateTime.Today);
				DateTime startDate=currentPayPeriod>-1 ? _listPayPeriods[currentPayPeriod].DateStart : DateTime.Today;
				textString+=Lan.g(this," - You can only modify your time card for the pay period starting on ")+startDate.ToShortDateString()+Lan.g(this,".");
			}
			return textString;
		}

		public void Initialize(DateTime dateInitial){
			_timeSpanDelta=MiscData.GetNowDateTime()-DateTime.Now;
			if(IdxPayPeriodSelected==0) {
				IdxPayPeriodSelected=PayPeriods.GetForDate(dateInitial);
			}
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {//Breaks turned off, Lunch is now "Break", but maintains Lunch functionality.
				IsBreaks=false;
				groupBox2.Visible=false;
			}
			if(IsBreaks){
				textOvertime.Visible=false;
				labelOvertime.Visible=false;
				butCalcDaily.Visible=false;
				butCalcWeekOT.Visible=false;//butCompute.Visible=false;
				butAdj.Visible=false;
				labelRateTwo.Visible=false;
				textRateTwo.Visible=false;
				textRateTwo2.Visible=false;
				labelRateThree.Visible=false;
				textRateThree.Visible=false;
				textRateThree2.Visible=false;
			}
			radioTimeCard.Checked=!IsBreaks;
			radioBreaks.Checked=IsBreaks;
			_listPayPeriods=PayPeriods.GetDeepCopy();
			FillUi();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(IdxPayPeriodSelected==0){
				return;
			}
			SaveNoteToDb();
			IdxPayPeriodSelected--;
			FillUi();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(IdxPayPeriodSelected==_listPayPeriods.Count-1) {
				return;
			}
			SaveNoteToDb();
			IdxPayPeriodSelected++;
			FillUi();
		}

		private void FillUi() {
			//Check to see if the employee currently logged in can edit this time-card.
			Text=GetTitle();
			FillPayPeriod();
			FillMain(true);
		}

		///<summary>SelectedPayPeriod should already be set.  This simply fills the screen with that data.</summary>
		private void FillPayPeriod(){
			textDateStart.Text=_listPayPeriods[IdxPayPeriodSelected].DateStart.ToShortDateString();
			textDateStop.Text=_listPayPeriods[IdxPayPeriodSelected].DateStop.ToShortDateString();
			if(_listPayPeriods[IdxPayPeriodSelected].DatePaycheck.Year<1880){
				textDatePaycheck.Text="";
			}
			else{
				textDatePaycheck.Text=_listPayPeriods[IdxPayPeriodSelected].DatePaycheck.ToShortDateString();
			}
			//fill the note for the pay period
			_timeAdjustNote=GetOrCreatePayPeriodNote();
			textNote.Text=_timeAdjustNote.Note;
		}

		///<summary>Gets the pay period note from the timeadjust row on midnight of the first day in the pay period from the database,
		///otherwise creates a new TimeAdjust object in memory to be inserted later.</summary>
		private TimeAdjust GetOrCreatePayPeriodNote() {
			DateTime date=_listPayPeriods[IdxPayPeriodSelected].DateStart.Date;
			DateTime dateMidnightFirstDay=new DateTime(date.Year,date.Month,date.Day,0,0,0);
			TimeAdjust timeAdjustNoteRow=TimeAdjusts.GetPayPeriodNote(EmployeeCur.EmployeeNum,dateMidnightFirstDay);
			if(timeAdjustNoteRow==null) {
				timeAdjustNoteRow=new TimeAdjust {
					EmployeeNum=EmployeeCur.EmployeeNum,
					TimeEntry=dateMidnightFirstDay,
					Note="",
					IsAuto=false
				};
			}
			return timeAdjustNoteRow;
		}

		private void radioTimeCard_Click(object sender,EventArgs e) {
			IsBreaks=false;
			textOvertime.Visible=true;
			labelOvertime.Visible=true;
			butCalcDaily.Visible=true;//butDaily.Visible=true;
			butCalcWeekOT.Visible=true;//butCompute.Visible=true;
			butAdj.Visible=true;
			labelRateTwo.Visible=true;
			textRateTwo.Visible=true;
			textRateTwo2.Visible=true;
			labelRateThree.Visible=true;
			textRateThree.Visible=true;
			textRateThree2.Visible=true;
			labelPTO.Visible=true;
			textPTO.Visible=true;
			textPTO2.Visible=true;
			FillMain(true);
		}

		private void radioBreaks_Click(object sender,EventArgs e) {
			IsBreaks=true;
			textOvertime.Visible=false;
			labelOvertime.Visible=false;
			butCalcDaily.Visible=false;//butDaily.Visible=false;
			butCalcWeekOT.Visible=false;//butCompute.Visible=false;
			butAdj.Visible=false;
			labelRateTwo.Visible=false;
			textRateTwo.Visible=false;
			textRateTwo2.Visible=false;
			labelRateThree.Visible=false;
			textRateThree.Visible=false;
			textRateThree2.Visible=false;
			labelPTO.Visible=false;
			textPTO.Visible=false;
			textPTO2.Visible=false;
			//butDaily.Visible=false;
			FillMain(true);
		}

		private DateTime GetDateForRow(int i){
			if(_arrayListMerged[i].GetType()==typeof(ClockEvent)){
				return ((ClockEvent)_arrayListMerged[i]).TimeDisplayed1.Date;
			}
			// Convert 'else if' -> 'if' ?
			else if(_arrayListMerged[i].GetType()==typeof(TimeAdjust)){
				return ((TimeAdjust)_arrayListMerged[i]).TimeEntry.Date;
			}
			return DateTime.MinValue;
		}

		///<summary>fromDB is set to false when it is refreshing every second so that there will be no extra network traffic.</summary>
		private void FillMain(bool fromDB){
			if(fromDB){
				_listClockEvents=ClockEvents.Refresh(EmployeeCur.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),IsBreaks);
				if(IsBreaks){
					_listTimeAdjusts=new List<TimeAdjust>();
				}
				else{
					_listTimeAdjusts=TimeAdjusts.Refresh(EmployeeCur.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
			}
			_listTimeAdjusts.RemoveAll(x => x.TimeAdjustNum==_timeAdjustNote.TimeAdjustNum);//Do not show the note row in the grid.
			_arrayListMerged=new ArrayList();
			for(int i=0;i<_listClockEvents.Count;i++) {
				_arrayListMerged.Add(_listClockEvents[i]);
			}
			for(int i=0;i<_listTimeAdjusts.Count;i++) {
				_arrayListMerged.Add(_listTimeAdjusts[i]);
			}
			IComparer iComparer=new ObjectDateComparer();
			_arrayListMerged.Sort(iComparer);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),45);
			gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g(this,"Altered"),50,HorizontalAlignment.Center);//use red now instead of separate col
			//gridMain.Columns.Add(col);
			if(IsBreaks){
				col=new GridColumn(Lan.g(this,"Out"),64,HorizontalAlignment.Right);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"In"),64,HorizontalAlignment.Right);
				gridMain.Columns.Add(col);
			}
			else{
				col=new GridColumn(Lan.g(this,"In"),64,HorizontalAlignment.Right);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Out"),64,HorizontalAlignment.Right);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Total"),50,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Adjust"),45,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),45,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate3"),45,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PTO"),45,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"OT"),45,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PL"),45,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			if(PrefC.IsODHQ) {
				col=new GridColumn(Lan.g(this,"WFH"),35,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Day"),50,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Week"),50,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),100){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_timeSpanArrayWeeklyTotals=new TimeSpan[_arrayListMerged.Count];
			TimeSpan timeSpanOne=new TimeSpan(0);//used to sum one pair of clock-in/clock-out
			TimeSpan timeSpanOneAdj;
			TimeSpan timeSpanOneOT;
			TimeSpan timeSpanDay=new TimeSpan(0);//used for daily totals.
			TimeSpan timeSpanWeek=new TimeSpan(0);//used for weekly totals.
			TimeSpan timeSpanPTO=new TimeSpan(0);//used for PTO totals.
			TimeSpan timeSpanUnpaidProtectedLeave=new TimeSpan(0);//used for Unpaid Protected Leave totals.
			if(_arrayListMerged.Count>0) {  //Have to check fromDB here because we dont want to call DB every timer tick
				if(fromDB) {
					timeSpanWeek=ClockEvents.GetWeekTotal(EmployeeCur.EmployeeNum,GetDateForRow(0));
					_timeSpanStoredWeek=timeSpanWeek;
				}
				else {
					timeSpanWeek=_timeSpanStoredWeek;
				}
			}
			TimeSpan timeSpanPeriod=new TimeSpan(0);//used to add up totals for entire page.
			TimeSpan timeSpanOT=new TimeSpan(0);//overtime for the entire period
			TimeSpan timeSpanRate2=new TimeSpan(0);//rate2 hours total
			TimeSpan timeSpanRate3=new TimeSpan(0);//rate3 hours total
			Calendar calendar=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule calendarWeekRule=CalendarWeekRule.FirstFullWeek;//CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			DateTime dateTime=DateTime.MinValue;
			DateTime dateTimePrev=DateTime.MinValue;
			Type type;
			ClockEvent clockEvent;
			TimeAdjust timeAdjust;
			for(int i=0;i<_arrayListMerged.Count;i++){
				row=new GridRow();
				type=_arrayListMerged[i].GetType();
				row.Tag=_arrayListMerged[i];
				dateTimePrev=dateTime;
				#region ClockEvent Row
				if(type==typeof(ClockEvent)){
					clockEvent=(ClockEvent)_arrayListMerged[i];
					dateTime=clockEvent.TimeDisplayed1.Date;
				//Columns 1 and 2 - Date and Day----------------------------------
					if(dateTime==dateTimePrev){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(dateTime.ToShortDateString());
						row.Cells.Add(dateTime.ToString("ddd"));//Abbreviated name of day
					}
				//Column 3 - In (or Out if break)---------------------------------
					if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {
						row.Cells.Add(clockEvent.TimeDisplayed1.ToLongTimeString());
					}
					else {
						row.Cells.Add(clockEvent.TimeDisplayed1.ToShortTimeString());
					}
					if (clockEvent.TimeEntered1!=clockEvent.TimeDisplayed1){
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
				//Column 4 - Out (or In if break)---------------------------------
					if(clockEvent.TimeDisplayed2.Year<1880){
						row.Cells.Add("");//not clocked out yet
					}
					else{
						if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {
							row.Cells.Add(clockEvent.TimeDisplayed2.ToLongTimeString());
						}
						else {
							row.Cells.Add(clockEvent.TimeDisplayed2.ToShortTimeString());
						}
						if (clockEvent.TimeEntered2!=clockEvent.TimeDisplayed2){
							row.Cells[row.Cells.Count-1].ColorText = Color.Red;
						}
					}
				//Column 5 - Total------------------------------------------------
					if(IsBreaks){ //breaks
						if(clockEvent.TimeDisplayed2.Year<1880){
							row.Cells.Add("");
						}
						else{
							timeSpanOne=clockEvent.TimeDisplayed2-clockEvent.TimeDisplayed1;
							row.Cells.Add(ClockEvents.Format(timeSpanOne));
							timeSpanDay+=timeSpanOne;
							timeSpanPeriod+=timeSpanOne;
						}
					}
					else{//regular hours
						if(clockEvent.TimeDisplayed2.Year<1880){
							row.Cells.Add("");
						}
						else{
							timeSpanOne=clockEvent.TimeDisplayed2-clockEvent.TimeDisplayed1;
							row.Cells.Add(ClockEvents.Format(timeSpanOne));
							timeSpanDay+=timeSpanOne;
							timeSpanWeek+=timeSpanOne;
							timeSpanPeriod+=timeSpanOne;
						}
					}
				//Column 6 - Adjust-----------------------------------------------
					timeSpanOneAdj=TimeSpan.Zero;
					if(clockEvent.AdjustIsOverridden) {
						timeSpanOneAdj+=clockEvent.Adjust;
					}
					else {
						timeSpanOneAdj+=clockEvent.AdjustAuto;//typically zero
					}
					timeSpanDay+=timeSpanOneAdj;
					timeSpanWeek+=timeSpanOneAdj;
					timeSpanPeriod+=timeSpanOneAdj;
					row.Cells.Add(ClockEvents.Format(timeSpanOneAdj));
					if(clockEvent.AdjustIsOverridden) {
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
				//Column 7 - Rate2------------------------------------------------
					if(clockEvent.Rate2Hours!=TimeSpan.FromHours(-1)) {
						timeSpanRate2+=clockEvent.Rate2Hours;
						row.Cells.Add(ClockEvents.Format(clockEvent.Rate2Hours));
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					else {
						timeSpanRate2+=clockEvent.Rate2Auto;
						row.Cells.Add(ClockEvents.Format(clockEvent.Rate2Auto));
					}
				//Column 8 - Rate3------------------------------------------------
					if(clockEvent.Rate3Hours!=TimeSpan.FromHours(-1)) {
						timeSpanRate3+=clockEvent.Rate3Hours;
						row.Cells.Add(ClockEvents.Format(clockEvent.Rate3Hours));
						row.Cells[row.Cells.Count-1].ColorText=Color.Red;
					}
					else {
						timeSpanRate3+=clockEvent.Rate3Auto;
						row.Cells.Add(ClockEvents.Format(clockEvent.Rate3Auto));
					}
				//Column 9 - PTO--------------------------------------------------
					row.Cells.Add("");//No PTO should exist, leave blank
				//Column 10 - OT--------------------------------------------------
					timeSpanOneOT=TimeSpan.Zero;
					if(clockEvent.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						timeSpanOneOT=clockEvent.OTimeHours;
					}
					else {
						timeSpanOneOT=clockEvent.OTimeAuto;//typically zero
					}
					timeSpanOT+=timeSpanOneOT;
					timeSpanDay-=timeSpanOneOT;
					timeSpanWeek-=timeSpanOneOT;
					timeSpanPeriod-=timeSpanOneOT;
					row.Cells.Add(ClockEvents.Format(timeSpanOneOT));
					if(clockEvent.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
				//Column 11 - PL (Unpaid Protected Leave)-------------------------
					row.Cells.Add("");//No PL should exist, leave blank

				//Column 12 - WFH Working From Home ------------------------------
					if(PrefC.IsODHQ) {
						if(IsBreaks || !clockEvent.IsWorkingHome){
							row.Cells.Add("");//Not Working from home, leave blank
						}
						else {
							row.Cells.Add("X");
						}
					}
				//Column 13 (or 12 if no WFH) - Day (daily total)-----------------
					//if this is the last entry for a given date
					if(i==_arrayListMerged.Count-1//if this is the last row
						|| GetDateForRow(i+1) != dateTime)//or the next row is a different date
					{
						if(IsBreaks){
							if(clockEvent.TimeDisplayed2.Year<1880){//if they have not clocked back in yet from break
								//display the timespan of oneSpan using current time as the other number.
								timeSpanOne=DateTime.Now-clockEvent.TimeDisplayed1+_timeSpanDelta;
								row.Cells.Add(timeSpanOne.ToStringHmmss());
								timeSpanDay+=timeSpanOne;
							}
							else{
								row.Cells.Add(ClockEvents.Format(timeSpanDay));
							}
						}
						else{
							row.Cells.Add(ClockEvents.Format(timeSpanDay));
						}
						timeSpanDay=new TimeSpan(0);
					}
					else{//not the last entry for the day
						row.Cells.Add("");
					}
				//Column 14 (or 13 if no WFH) - Week (weekly total)---------------
					_timeSpanArrayWeeklyTotals[i]=timeSpanWeek;
					if(IsBreaks){
						row.Cells.Add("");
					}
					//if this is the last entry for a given week
					else if(i==_arrayListMerged.Count-1//if this is the last row 
						|| calendar.GetWeekOfYear(GetDateForRow(i+1),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= calendar.GetWeekOfYear(clockEvent.TimeDisplayed1.Date,calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						row.Cells.Add(ClockEvents.Format(timeSpanWeek));
						timeSpanWeek=new TimeSpan(0);
					}
					else {
						//row.Cells.Add(ClockEvents.Format(weekSpan));
						row.Cells.Add("");
					}
				//Column 15 (or 14 if no WFH) - Clinic----------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(clockEvent.ClinicNum));
					}
				//Column 16 (or 15 if no clinics and 14 if also no WFH) - Note----
					row.Cells.Add(clockEvent.Note);
				}
				#endregion
				#region Adjustment Row
				else if(type==typeof(TimeAdjust)){
					timeAdjust=(TimeAdjust)_arrayListMerged[i];
					dateTime=timeAdjust.TimeEntry.Date;
				//Columns 1 and 2 - Date and Day----------------------------------
					if(dateTime==dateTimePrev){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(dateTime.ToShortDateString());
						row.Cells.Add(dateTime.ToString("ddd"));//Abbreviated name of day
					}
				//Column 3 - In (or Out if break)---------------------------------
					row.Cells.Add("");
				//Column 4 - Out (or In if break)---------------------------------
					if(timeAdjust.PtoDefNum==0) {
						row.Cells.Add("(Adjust)");
					}
					else { 
						row.Cells.Add(Defs.GetDef(DefCat.TimeCardAdjTypes,timeAdjust.PtoDefNum).ItemName);
					}
					row.Cells[row.Cells.Count-1].ColorText=Color.Red;
				//Column 5 - Total------------------------------------------------
					row.Cells.Add("");
				//Column 6 - Adjust-----------------------------------------------
					if(timeAdjust.IsUnpaidProtectedLeave) {
						row.Cells.Add("");//7
					}
					else if(timeAdjust.PtoDefNum==0) {
						timeSpanDay+=timeAdjust.RegHours;//might be negative
						timeSpanWeek+=timeAdjust.RegHours;
						timeSpanPeriod+=timeAdjust.RegHours;
						row.Cells.Add(ClockEvents.Format(timeAdjust.RegHours));
					} 
					else {
						timeSpanPTO+=timeAdjust.PtoHours;
						row.Cells.Add("");
					}
				//Column 7 - Rate2------------------------------------------------
					row.Cells.Add("");
				//Column 8 - Rate3------------------------------------------------
					row.Cells.Add("");
				//Column 9 - PTO--------------------------------------------------
					row.Cells.Add(ClockEvents.Format(timeAdjust.PtoHours));
				//Column 10 - OT--------------------------------------------------
					timeSpanOT+=timeAdjust.OTimeHours;
					row.Cells.Add(ClockEvents.Format(timeAdjust.OTimeHours));
				//Column 11 - PL (Unpaid Protected Leave)-------------------------
					if(timeAdjust.IsUnpaidProtectedLeave) {
						row.Cells.Add(ClockEvents.Format(timeAdjust.RegHours));
						timeSpanUnpaidProtectedLeave+=timeAdjust.RegHours;
					}
					else {
						row.Cells.Add("");
					}
				//Column 12 - WFH Working From Home ------------------------------
					if(PrefC.IsODHQ) {
						row.Cells.Add("");
					}
				//Column 13 (or 12 if no WFH) - Day (daily total)-----------------
					//if this is the last entry for a given date
					if(i==_arrayListMerged.Count-1//if this is the last row
						|| GetDateForRow(i+1) != dateTime)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(timeSpanDay));//
						timeSpanDay=new TimeSpan(0);
					}
					else{
						row.Cells.Add("");
					}
				//Column 14 (or 13 if no WFH) - Week (weekly total)---------------
					_timeSpanArrayWeeklyTotals[i]=timeSpanWeek;
					if(IsBreaks){
						row.Cells.Add("");
					}
					//if this is the last entry for a given week
					else if(i==_arrayListMerged.Count-1//if this is the last row 
						|| calendar.GetWeekOfYear(GetDateForRow(i+1),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= calendar.GetWeekOfYear(timeAdjust.TimeEntry.Date,calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						GridCell cell=new GridCell(ClockEvents.Format(timeSpanWeek));
						cell.ColorText=Color.Black;
						row.Cells.Add(cell);
						timeSpanWeek=new TimeSpan(0);
					}
					else {
						row.Cells.Add("");
					}
				//Column 15 (or 14 if no WFH) - Clinic----------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(timeAdjust.ClinicNum));
					}
				//Column 16 (or 15 if no clinics and 14 if also no WFH) - Note----
					row.Cells.Add(timeAdjust.Note);
				}
				#endregion
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(IsBreaks){
				labelRegularTime.Visible=false;
				labelOvertime.Visible=false;
				labelRateTwo.Visible=false;
				labelRateThree.Visible=false;
				labelPTO.Visible=false;
				textTotal.Visible=false;
				textTotal2.Visible=false;
				textOvertime.Visible=false;
				textOvertime2.Visible=false;
				textRateTwo.Visible=false;
				textRateTwo2.Visible=false;
				textRateThree.Visible=false;
				textRateThree2.Visible=false;
				textPTO.Visible=false;
				textPTO2.Visible=false;
				labelUnpaidProtectedLeave.Visible=false;
				textUnpaidProtectedLeave.Visible=false;
				textUnpaidProtectedLeave2.Visible=false;
			}
			else {
				labelRegularTime.Visible=true;
				labelOvertime.Visible=true;
				labelRateTwo.Visible=true;
				labelRateThree.Visible=true;
				textTotal.Visible=true;
				textTotal2.Visible=true;
				textOvertime.Visible=true;
				textOvertime2.Visible=true;
				textRateTwo.Visible=true;
				textRateTwo2.Visible=true;
				textRateThree.Visible=true;
				textRateThree2.Visible=true;
				labelUnpaidProtectedLeave.Visible=true;
				textUnpaidProtectedLeave.Visible=true;
				textUnpaidProtectedLeave2.Visible=true;
				textTotal.Text=timeSpanPeriod.ToStringHmm();
				textOvertime.Text=timeSpanOT.ToStringHmm();
				textRateTwo.Text=timeSpanRate2.ToStringHmm();
				textRateThree.Text=timeSpanRate3.ToStringHmm();
				textPTO.Text=timeSpanPTO.ToStringHmm();
				textUnpaidProtectedLeave.Text=timeSpanUnpaidProtectedLeave.ToStringHmm();
				textTotal2.Text=timeSpanPeriod.TotalHours.ToString("n");
				textOvertime2.Text=timeSpanOT.TotalHours.ToString("n");
				textRateTwo2.Text=timeSpanRate2.TotalHours.ToString("n");
				textRateThree2.Text=timeSpanRate3.TotalHours.ToString("n");
				textPTO2.Text=timeSpanPTO.TotalHours.ToString("n");
				textUnpaidProtectedLeave2.Text=timeSpanUnpaidProtectedLeave.TotalHours.ToString("n");
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_cannotEditOwnTimecard) {
				MsgBox.Show("You do not have permission to modify your time card.");
				return;
			}
			if(_cannotEditSelectedPayPeriod) {
				MsgBox.Show("You do not have permission to modify your past pay periods.");
				return;
			}
			if(Security.CurUser.EmployeeNum!=EmployeeCur.EmployeeNum && !Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				return;
			}
			timerUpdateBreak.Enabled=false;
			if(gridMain.ListGridRows[e.Row].Tag.GetType()==typeof(TimeAdjust)) {
				TimeAdjust adjust=(TimeAdjust)gridMain.ListGridRows[e.Row].Tag;
				//Only users with the ProtectedLeaveAdjustmentEdit permission can edit other user's UPL Time Card Adjustments.
				if(adjust.IsUnpaidProtectedLeave 
					&& Security.CurUser.EmployeeNum!=EmployeeCur.EmployeeNum
					&& !Security.IsAuthorized(Permissions.ProtectedLeaveAdjustmentEdit))
				{
					timerUpdateBreak.Enabled=true;
					return;
				}
				//HQ users without the TimecardsEditAll permission can only edit PTO and UPL Time Card Adjustments.
				if(PrefC.IsODHQ
					&& !adjust.IsUnpaidProtectedLeave
					&& adjust.PtoDefNum==0
					&& !Security.IsAuthorized(Permissions.TimecardsEditAll,suppressMessage:true))
				{
					MsgBox.Show(this,"HQ users without the Edit All Time Cards permission can only edit Time Card Adjustments for PTO or Protected Leave.");
					timerUpdateBreak.Enabled=true;
					return;
				}
				using FormTimeAdjustEdit formTimeAdjustmentEdit=new FormTimeAdjustEdit(adjust);
				formTimeAdjustmentEdit.ShowDialog();
			}
			else {
				ClockEvent ce=(ClockEvent)gridMain.ListGridRows[e.Row].Tag;
				using FormClockEventEdit formClockEventEdit=new FormClockEventEdit(ce);
				formClockEventEdit.ShowDialog();
			}
			FillMain(true);
			timerUpdateBreak.Enabled=true;
		}

		private void butAdj_Click(object sender,EventArgs e) {
			if(Security.CurUser.EmployeeNum!=EmployeeCur.EmployeeNum && !Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				return;
			}
			if(_cannotEditOwnTimecard) {
				MsgBox.Show(this,"You do not have permission to modify your time card.");
				return;
			}
			TimeAdjust adjust=new TimeAdjust();
			adjust.EmployeeNum=EmployeeCur.EmployeeNum;
			if(PrefC.HasClinicsEnabled) {
				adjust.ClinicNum=Clinics.ClinicNum;
			}
			DateTime dateStop=PIn.Date(textDateStop.Text);
			if(DateTime.Today<=dateStop && DateTime.Today>=PIn.Date(textDateStart.Text)) {
				adjust.TimeEntry=DateTime.Now;
			}
			else {
				adjust.TimeEntry=new DateTime(dateStop.Year,dateStop.Month,dateStop.Day,
					DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);
			}
			using FormTimeAdjustEdit FormT=new FormTimeAdjustEdit(adjust);
			FormT.IsNew=true;
			FormT.ShowDialog();
			if(FormT.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillMain(true);
		}

		private void butCalcWeekOT_Click(object sender,EventArgs e) {
			if(_cannotEditOwnTimecard) { 
				MsgBox.Show("You do not have permission to modify your time card.");
				return;
			}
			if(_cannotEditSelectedPayPeriod) {
				MsgBox.Show("You do not have permission to modify your past pay periods.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				return;
			}
			TimeCardRule timeCardRule=TimeCardRules.GetTimeCardRule(EmployeeCur);
			if(timeCardRule!=null && timeCardRule.IsOvertimeExempt) {
				MsgBox.Show(this,"This employee is marked as exempt from receiving overtime hours.");
				return;
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(EmployeeCur,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			}
			FillMain(true);
		}

		private void butCalcDaily_Click(object sender,EventArgs e) {
			//not even visible if viewing breaks.
			//suppress security warning because the main point of this check is to see if users can edit their own time cards
			if(_cannotEditOwnTimecard) { 
				MsgBox.Show("You do not have permission to modify your time card.");
				return;
			}
			if(_cannotEditSelectedPayPeriod) {
				MsgBox.Show("You do not have permission to modify your past pay periods.");
				return;
			}
			else if(!PrefC.GetBool(PrefName.TimecardSecurityEnabled) && !Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				//Security.IsAuthorized() shows the error to the user already.
				return;
			}
			string errors=TimeCardRules.ValidateOvertimeRules(new List<long>{EmployeeCur.EmployeeNum});
			if(errors != "") {
				MessageBox.Show(this,"Please fix the following timecard rule errors first:\r\n"+errors);
				return;
			}
			errors=TimeCardRules.ValidatePayPeriod(EmployeeCur,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			if(errors != "") {
				MessageBox.Show(this,errors);
				return;
			}
			try {
				TimeCardRules.CalculateDailyOvertime(EmployeeCur,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			}
			FillMain(true);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			SaveNoteToDb();
			_linesPrinted=0;
			PrintoutOrientation printoutOrientation=PrintoutOrientation.Portrait;
      if(PrefC.IsODHQ) {
				printoutOrientation=PrintoutOrientation.Landscape; //Switching for extra WFH column
      }
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,
				Lan.g(this,"Time card for")+" "+EmployeeCur.LName+","+EmployeeCur.FName+" "+Lan.g(this,"printed"),
				new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin,
				printoutOrientation:printoutOrientation
			);
		}

		///<summary>raised for each page to be printed.</summary>
		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Graphics g=e.Graphics;
			float yPos=75;
			float xPos=55;
			string str;
			Font font=new Font(FontFamily.GenericSansSerif,8);
			Font fontTitle=new Font(FontFamily.GenericSansSerif,11,FontStyle.Bold);
			Font fontHeader=new Font(FontFamily.GenericSansSerif,8,FontStyle.Bold);
			SolidBrush solidBrush=new SolidBrush(Color.Black);
			Pen pen=new Pen(Color.Black);
			//Title
			str=EmployeeCur.FName+" "+EmployeeCur.LName;
			str+="\r\n"+Lan.g(this,"Note")+": "+_timeAdjustNote.Note.ToString();
			int heightThreeLine=(int)e.Graphics.MeasureString("1\r\n2\r\n3",fontTitle).Height;
			int marginBothSides=(int)xPos*2;//110
			int heightNoteString=(int)e.Graphics.MeasureString(str,fontTitle,e.PageBounds.Width-marginBothSides).Height;
			int heightRect=Math.Min(heightNoteString,heightThreeLine);
			StringFormat stringFormat=new StringFormat{ Trimming=StringTrimming.Word };
			g.DrawString(str,fontTitle,solidBrush,new RectangleF(xPos,yPos,e.PageBounds.Width-marginBothSides,heightRect),stringFormat);
			yPos+=heightRect+5;//+5 pixels for a small space between columns and title area.
			//define columns
			int[] intArrayColW=new int[14];
			if(PrefC.HasClinicsEnabled || PrefC.IsODHQ) {
				intArrayColW=new int[15];
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				intArrayColW=new int[16];
			}
			intArrayColW[0]=70;//Date
			intArrayColW[1]=45;//Day: Column starts to wrap at 32 pixels, however added padding to 45 to allow room for language translations
			intArrayColW[2]=60;//In/Out
			intArrayColW[3]=60;//Out/In
			intArrayColW[4]=45;//Total
			intArrayColW[5]=45;//Adjust: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			intArrayColW[6]=45;//Rate 2: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			intArrayColW[7]=45;//Rate 3: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			intArrayColW[8]=45;//PTO: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			intArrayColW[9]=45;//OT: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			intArrayColW[10]=45;//PL: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			intArrayColW[11]=45;//Day
			intArrayColW[12]=50;//Week
			intArrayColW[13]=130;//Note
			if(PrefC.IsODHQ) {
				intArrayColW[11]=45;//WFH
				intArrayColW[12]=45;//Day
				intArrayColW[13]=50;//Week
				intArrayColW[14]=300;//Note
			}
			else if(PrefC.HasClinicsEnabled) {
				intArrayColW[13]=50;//Clinic
				intArrayColW[14]=80;//Note: Reduce width when Clinic column is added so that we do not exceed the margin.
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				intArrayColW[14]=100;//Clinic
				intArrayColW[15]=200;//Note: Reduce width when Clinic column is added so that we do not exceed the margin.
			}
			int[] intArrayColPos=new int[intArrayColW.Length+1];
			intArrayColPos[0]=45;
			for(int i=1;i<intArrayColPos.Length;i++) {
				intArrayColPos[i]=intArrayColPos[i-1]+intArrayColW[i-1];
			}
			string[] stringArrayColCaption=new string[14];
			if(PrefC.HasClinicsEnabled || PrefC.IsODHQ) {
				stringArrayColCaption=new string[15];
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				stringArrayColCaption=new string[16];
			}
			stringArrayColCaption[0]=Lan.g(this,"Date");
			stringArrayColCaption[1]=Lan.g(this,"Day");
			if(radioBreaks.Checked) {
				stringArrayColCaption[2]=Lan.g(this,"Out");
				stringArrayColCaption[3]=Lan.g(this,"In");
			}
			else {
				stringArrayColCaption[2]=Lan.g(this,"In");
				stringArrayColCaption[3]=Lan.g(this,"Out");
			}
			stringArrayColCaption[4]=Lan.g(this,"Total");
			stringArrayColCaption[5]=Lan.g(this,"Adjust");
			stringArrayColCaption[6]=Lan.g(this,"Rate 2");
			stringArrayColCaption[7]=Lan.g(this,"Rate 3");
			stringArrayColCaption[8]=Lan.g(this,"PTO");
			stringArrayColCaption[9]=Lan.g(this,"OT");
			stringArrayColCaption[10]=Lan.g(this,"PL");
			stringArrayColCaption[11]=Lan.g(this,"Day");
			stringArrayColCaption[12]=Lan.g(this,"Week");
			stringArrayColCaption[13]=Lan.g(this,"Note");
			if(PrefC.IsODHQ) {
				stringArrayColCaption[11]=Lan.g(this,"WFH");
				stringArrayColCaption[12]=Lan.g(this,"Day");
				stringArrayColCaption[13]=Lan.g(this,"Week");
				stringArrayColCaption[14]=Lan.g(this,"Note");
			}
			else if(PrefC.HasClinicsEnabled) {
				stringArrayColCaption[13]=Lan.g(this,"Clinic");
				stringArrayColCaption[14]=Lan.g(this,"Note");
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				stringArrayColCaption[14]=Lan.g(this,"Clinic");
				stringArrayColCaption[15]=Lan.g(this,"Note");
			}
			//column headers-----------------------------------------------------------------------------------------
			e.Graphics.FillRectangle(Brushes.LightGray,intArrayColPos[0],yPos,intArrayColPos[intArrayColPos.Length-1]-intArrayColPos[0],18);
			e.Graphics.DrawRectangle(pen,intArrayColPos[0],yPos,intArrayColPos[intArrayColPos.Length-1]-intArrayColPos[0],18);
			for(int i=1;i<intArrayColPos.Length;i++) {
				e.Graphics.DrawLine(new Pen(Color.Black),intArrayColPos[i],yPos,intArrayColPos[i],yPos+18);
			}
			//Prints the Column Titles
			for(int i=0;i<stringArrayColCaption.Length;i++) {
				e.Graphics.DrawString(stringArrayColCaption[i],fontHeader,solidBrush,intArrayColPos[i]+2,yPos+1);
			}
			yPos+=18;
			while(yPos < e.PageBounds.Height-75-50-32-16 && _linesPrinted < gridMain.ListGridRows.Count) {
				for(int i=0;i<intArrayColPos.Length-1;i++) {
					if(gridMain.ListGridRows[_linesPrinted].Cells[i].ColorText==Color.Empty || gridMain.ListGridRows[_linesPrinted].Cells[i].ColorText==Color.Black) {
						e.Graphics.DrawString(gridMain.ListGridRows[_linesPrinted].Cells[i].Text,font,solidBrush
							,new RectangleF(intArrayColPos[i]+2,yPos,intArrayColPos[i+1]-intArrayColPos[i]-5,font.GetHeight(e.Graphics)));
					}
					else { //The only other color currently supported is red.
						e.Graphics.DrawString(gridMain.ListGridRows[_linesPrinted].Cells[i].Text,font,Brushes.Red
							,new RectangleF(intArrayColPos[i]+2,yPos,intArrayColPos[i+1]-intArrayColPos[i]-5,font.GetHeight(e.Graphics)));
					}
				}
				//Column lines		
				for(int i=0;i<intArrayColPos.Length;i++) {
					e.Graphics.DrawLine(Pens.Gray,intArrayColPos[i],yPos+16,intArrayColPos[i],yPos);
				}
				_linesPrinted++;
				yPos+=16;
				e.Graphics.DrawLine(new Pen(Color.Gray),intArrayColPos[0],yPos,intArrayColPos[intArrayColPos.Length-1],yPos);
			}
			//bottom line
			//e.Graphics.DrawLine(new Pen(Color.Gray),colPos[0],yPos,colPos[colPos.Length-1],yPos);
			//totals will print on every page for simplicity
			yPos+=10;
			g.DrawString(Lan.g(this,"Regular Time")+": "+textTotal.Text+" ("+textTotal2.Text+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Overtime")+": "+textOvertime.Text+" ("+textOvertime2.Text+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 2 Time")+": "+textRateTwo.Text+" ("+textRateTwo2.Text+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 3 Time")+": "+textRateThree.Text+" ("+textRateThree2.Text+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"PTO Time")+": "+textPTO.Text+" ("+textPTO2.Text+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Protected Leave")+": "+textUnpaidProtectedLeave.Text+" ("+textUnpaidProtectedLeave2.Text+")",fontHeader,solidBrush,xPos,yPos);
			if(_linesPrinted==gridMain.ListGridRows.Count) {
				e.HasMorePages=false;
			}
			else {
				e.HasMorePages=true;
			}
		}

		private void butBrowseEmp_Click(object sender,EventArgs e) {
			SaveNoteToDb();
			int empIndex=0;
			for(int i=0;i<_listEmployees.Count;i++) {
				//find current employee index by Employeenum
				if(EmployeeCur.EmployeeNum==_listEmployees[i].EmployeeNum) {
					if(sender.Equals(butPrevEmp)) {
						empIndex=i-1;//go to previous employee in list
					}
					else {
						empIndex=i+1;//go to next employee in list
					}
					empIndex=(empIndex+_listEmployees.Count)%(_listEmployees.Count);//allows wrapping at end of employee list.
					break;
				}
			}
			EmployeeCur=_listEmployees[empIndex];
			FillUi();
		}

		private void SaveNoteToDb() {
			_timeAdjustNote.Note=textNote.Text;
			if(_timeAdjustNote.TimeAdjustNum==0) {//adding a note for the first time.
				if(textNote.Text!="") {//Do not create a row if not needed.
					TimeAdjusts.Insert(_timeAdjustNote);
				}
			}
			else {
				TimeAdjusts.Update(_timeAdjustNote);
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			SaveNoteToDb();
			Close();
		}

		///<summary>This timer updates break times so that you can see your break timer counting.</summary>
		private void timerUpdateBreak_Tick(object sender, System.EventArgs e) {
			if(IsBreaks) {
				int idx = gridMain.GetSelectedIndex();
				FillMain(false);//deselects current index.
				if(idx>-1 && idx<gridMain.ListGridRows.Count) {
					gridMain.SetSelected(idx,true);
				}
			}
		}

		private void FormTimeCard_FormClosing(object sender,FormClosingEventArgs e) {
			timerUpdateBreak.Enabled=false;  //This timer was never being disabled, so it would just keep ticking after the form was closed.
		}

		

		

	

		

		

		

		

		

		

		

		

		


	}
}





















