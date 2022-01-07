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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTimeCard:FormODBase {
		///<summary>True to default to viewing breaks. False for regular hours.</summary>
		public bool IsBreaks;
		///<summary>Server time minus local computer time, usually +/- 1 or 2 minutes</summary>
		private TimeSpan TimeDelta;
		private List<ClockEvent> ClockEventList;
		public int SelectedPayPeriod;
		private List<TimeAdjust> TimeAdjustList;
		private int linesPrinted;
		//private OpenDental.UI.PrintPreview printPreview;
		///<summary>An array list of ojects representing the rows in the table. Can be either clockEvents or timeAdjusts.</summary>
		private ArrayList mergedAL;
		///<summary>The running weekly total, whether it gets displayed or not.</summary>
		private TimeSpan[] WeeklyTotals;
		public Employee EmployeeCur;
		///<summary>Used to determine the order to advance through employee timecards in this window.</summary>
		public bool IsByLastName;
		///<summary>Cached list of employees sorted based on IsByLastName</summary>
		private List<Employee> _listEmp=new List<Employee>();
		///<summary>Filled when FillMain is called and fromDB=true.  If fromDB is false, we used this stored value from before instead to reduce calls to DB.  Because fillgrid does math on weekspan, this is a temporary cache of the last time we calculated it from the database.</summary>
		private TimeSpan storedWeekSpan;
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
					SelectedPayPeriod!=PayPeriods.GetForDate(DateTime.Today);
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
			_listEmp=listEmployees;
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
				_listEmp.Sort(Employees.SortByLastName);
			}
			else {
				_listEmp.Sort(Employees.SortByFirstName);
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
			TimeDelta=MiscData.GetNowDateTime()-DateTime.Now;
			if(SelectedPayPeriod==0) {
				SelectedPayPeriod=PayPeriods.GetForDate(dateInitial);
			}
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {//Breaks turned off, Lunch is now "Break", but maintains Lunch functionality.
				IsBreaks=false;
				groupBox2.Visible=false;
			}
			if(IsBreaks){
				textOvertime.Visible=false;
				labelOvertime.Visible=false;
				butCalcWeekOT.Visible=false;//butCompute.Visible=false;
				butAdj.Visible=false;
				labelRateTwo.Visible=false;
				textRateTwo.Visible=false;
				textRateTwo2.Visible=false;
			}
			radioTimeCard.Checked=!IsBreaks;
			radioBreaks.Checked=IsBreaks;
			_listPayPeriods=PayPeriods.GetDeepCopy();
			FillUi();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(SelectedPayPeriod==0){
				return;
			}
			SaveNoteToDb();
			SelectedPayPeriod--;
			FillUi();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(SelectedPayPeriod==_listPayPeriods.Count-1) {
				return;
			}
			SaveNoteToDb();
			SelectedPayPeriod++;
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
			textDateStart.Text=_listPayPeriods[SelectedPayPeriod].DateStart.ToShortDateString();
			textDateStop.Text=_listPayPeriods[SelectedPayPeriod].DateStop.ToShortDateString();
			if(_listPayPeriods[SelectedPayPeriod].DatePaycheck.Year<1880){
				textDatePaycheck.Text="";
			}
			else{
				textDatePaycheck.Text=_listPayPeriods[SelectedPayPeriod].DatePaycheck.ToShortDateString();
			}
			//fill the note for the pay period
			_timeAdjustNote=GetOrCreatePayPeriodNote();
			textNote.Text=_timeAdjustNote.Note;
		}

		///<summary>Gets the pay period note from the timeadjust row on midnight of the first day in the pay period from the database,
		///otherwise creates a new TimeAdjust object in memory to be inserted later.</summary>
		private TimeAdjust GetOrCreatePayPeriodNote() {
			DateTime date=_listPayPeriods[SelectedPayPeriod].DateStart.Date;
			DateTime midnightFirstDay=new DateTime(date.Year,date.Month,date.Day,0,0,0);
			TimeAdjust noteRow=TimeAdjusts.GetPayPeriodNote(EmployeeCur.EmployeeNum,midnightFirstDay);
			if(noteRow==null) {
				noteRow=new TimeAdjust {
					EmployeeNum=EmployeeCur.EmployeeNum,
					TimeEntry=midnightFirstDay,
					Note="",
					IsAuto=false
				};
			}
			return noteRow;
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
			labelPTO.Visible=false;
			textPTO.Visible=false;
			textPTO2.Visible=false;
			//butDaily.Visible=false;
			FillMain(true);
		}

		private DateTime GetDateForRow(int i){
			if(mergedAL[i].GetType()==typeof(ClockEvent)){
				return ((ClockEvent)mergedAL[i]).TimeDisplayed1.Date;
			}
			else if(mergedAL[i].GetType()==typeof(TimeAdjust)){
				return ((TimeAdjust)mergedAL[i]).TimeEntry.Date;
			}
			return DateTime.MinValue;
		}

		///<summary>fromDB is set to false when it is refreshing every second so that there will be no extra network traffic.</summary>
		private void FillMain(bool fromDB){
			if(fromDB){
				ClockEventList=ClockEvents.Refresh(EmployeeCur.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),IsBreaks);
				if(IsBreaks){
					TimeAdjustList=new List<TimeAdjust>();
				}
				else{
					TimeAdjustList=TimeAdjusts.Refresh(EmployeeCur.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
			}
			TimeAdjustList.RemoveAll(x => x.TimeAdjustNum==_timeAdjustNote.TimeAdjustNum);//Do not show the note row in the grid.
			mergedAL=new ArrayList();
			for(int i=0;i<ClockEventList.Count;i++) {
				mergedAL.Add(ClockEventList[i]);
			}
			for(int i=0;i<TimeAdjustList.Count;i++) {
				mergedAL.Add(TimeAdjustList[i]);
			}
			IComparer myComparer=new ObjectDateComparer();
			mergedAL.Sort(myComparer);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),45);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn(Lan.g(this,"Altered"),50,HorizontalAlignment.Center);//use red now instead of separate col
			//gridMain.Columns.Add(col);
			if(IsBreaks){
				col=new GridColumn(Lan.g(this,"Out"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"In"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
			}
			else{
				col=new GridColumn(Lan.g(this,"In"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Out"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Total"),50,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Adjust"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PTO"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"OT"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PL"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),50,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Week"),50,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),100){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			WeeklyTotals=new TimeSpan[mergedAL.Count];
			TimeSpan alteredSpan=new TimeSpan(0);//used to display altered times
			TimeSpan oneSpan=new TimeSpan(0);//used to sum one pair of clock-in/clock-out
			TimeSpan oneAdj;
			TimeSpan oneOT;
			TimeSpan daySpan=new TimeSpan(0);//used for daily totals.
			TimeSpan weekSpan=new TimeSpan(0);//used for weekly totals.
			TimeSpan ptoSpan=new TimeSpan(0);//used for PTO totals.
			TimeSpan unpaidProtectedLeaveSpan=new TimeSpan(0);//used for Unpaid Protected Leave totals.
			if(mergedAL.Count>0) {  //Have to check fromDB here because we dont want to call DB every timer tick
				if(fromDB) {
					weekSpan=ClockEvents.GetWeekTotal(EmployeeCur.EmployeeNum,GetDateForRow(0));
					storedWeekSpan=weekSpan;
				}
				else {
					weekSpan=storedWeekSpan;
				}
			}
			TimeSpan periodSpan=new TimeSpan(0);//used to add up totals for entire page.
			TimeSpan otspan=new TimeSpan(0);//overtime for the entire period
			TimeSpan rate2span=new TimeSpan(0);//rate2 hours total
      Calendar cal=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule rule=CalendarWeekRule.FirstFullWeek;//CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			DateTime curDate=DateTime.MinValue;
			DateTime previousDate=DateTime.MinValue;
			Type type;
			ClockEvent clock;
			TimeAdjust adjust;
			for(int i=0;i<mergedAL.Count;i++){
				row=new GridRow();
				type=mergedAL[i].GetType();
				row.Tag=mergedAL[i];
				previousDate=curDate;
				//clock event row---------------------------------------------------------------------------------------------
				if(type==typeof(ClockEvent)){
					clock=(ClockEvent)mergedAL[i];
					curDate=clock.TimeDisplayed1.Date;
					if(curDate==previousDate){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(curDate.ToShortDateString());
						row.Cells.Add(curDate.ToString("ddd"));//Abbreviated name of day
					}
					//in------------------------------------------
					if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {
						row.Cells.Add(clock.TimeDisplayed1.ToLongTimeString());
					}
					else {
						row.Cells.Add(clock.TimeDisplayed1.ToShortTimeString());
					}
					if (clock.TimeEntered1!=clock.TimeDisplayed1){
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//out-----------------------------
					if(clock.TimeDisplayed2.Year<1880){
						row.Cells.Add("");//not clocked out yet
					}
					else{
						if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {
							row.Cells.Add(clock.TimeDisplayed2.ToLongTimeString());
						}
						else {
							row.Cells.Add(clock.TimeDisplayed2.ToShortTimeString());
						}
						if (clock.TimeEntered2!=clock.TimeDisplayed2){
							row.Cells[row.Cells.Count-1].ColorText = Color.Red;
						}
					}
					//total-------------------------------
					if(IsBreaks){ //breaks
						if(clock.TimeDisplayed2.Year<1880){
							row.Cells.Add("");
						}
						else{
							oneSpan=clock.TimeDisplayed2-clock.TimeDisplayed1;
							row.Cells.Add(ClockEvents.Format(oneSpan));
							daySpan+=oneSpan;
							periodSpan+=oneSpan;
						}
					}
					else{//regular hours
						if(clock.TimeDisplayed2.Year<1880){
							row.Cells.Add("");
						}
						else{
							oneSpan=clock.TimeDisplayed2-clock.TimeDisplayed1;
							row.Cells.Add(ClockEvents.Format(oneSpan));
							daySpan+=oneSpan;
							weekSpan+=oneSpan;
							periodSpan+=oneSpan;
						}
					}
					//Adjust---------------------------------
					oneAdj=TimeSpan.Zero;
					if(clock.AdjustIsOverridden) {
						oneAdj+=clock.Adjust;
					}
					else {
						oneAdj+=clock.AdjustAuto;//typically zero
					}
					daySpan+=oneAdj;
					weekSpan+=oneAdj;
					periodSpan+=oneAdj;
					row.Cells.Add(ClockEvents.Format(oneAdj));
					if(clock.AdjustIsOverridden) {
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//Rate2---------------------------------
					if(clock.Rate2Hours!=TimeSpan.FromHours(-1)) {
						rate2span+=clock.Rate2Hours;
						row.Cells.Add(ClockEvents.Format(clock.Rate2Hours));
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					else {
						rate2span+=clock.Rate2Auto;
						row.Cells.Add(ClockEvents.Format(clock.Rate2Auto));
					}
					//PTO------------------------------
					row.Cells.Add("");//No PTO should exist, leave blank
					//Overtime------------------------------
					oneOT=TimeSpan.Zero;
					if(clock.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						oneOT=clock.OTimeHours;
					}
					else {
						oneOT=clock.OTimeAuto;//typically zero
					}
					otspan+=oneOT;
					daySpan-=oneOT;
					weekSpan-=oneOT;
					periodSpan-=oneOT;
					row.Cells.Add(ClockEvents.Format(oneOT));
					if(clock.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//Unpaid Protected Leave (PL)-------------------------------------------------
					row.Cells.Add("");//No PL should exist, leave blank
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==mergedAL.Count-1//if this is the last row
						|| GetDateForRow(i+1) != curDate)//or the next row is a different date
					{
						if(IsBreaks){
							if(clock.TimeDisplayed2.Year<1880){//if they have not clocked back in yet from break
								//display the timespan of oneSpan using current time as the other number.
								oneSpan=DateTime.Now-clock.TimeDisplayed1+TimeDelta;
								row.Cells.Add(oneSpan.ToStringHmmss());
								daySpan+=oneSpan;
							}
							else{
								row.Cells.Add(ClockEvents.Format(daySpan));
							}
						}
						else{
							row.Cells.Add(ClockEvents.Format(daySpan));
						}
						daySpan=new TimeSpan(0);
					}
					else{//not the last entry for the day
						row.Cells.Add("");
					}
					//Weekly-------------------------------------
					WeeklyTotals[i]=weekSpan;
					if(IsBreaks){
						row.Cells.Add("");
					}
					//if this is the last entry for a given week
					else if(i==mergedAL.Count-1//if this is the last row 
						|| cal.GetWeekOfYear(GetDateForRow(i+1),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= cal.GetWeekOfYear(clock.TimeDisplayed1.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						row.Cells.Add(ClockEvents.Format(weekSpan));
						weekSpan=new TimeSpan(0);
					}
					else {
						//row.Cells.Add(ClockEvents.Format(weekSpan));
						row.Cells.Add("");
					}
					//Clinic-----------------------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(clock.ClinicNum));
					}
					//Note-----------------------------------------
					row.Cells.Add(clock.Note);
				}
				//adjustment row--------------------------------------------------------------------------------------
				else if(type==typeof(TimeAdjust)){
					adjust=(TimeAdjust)mergedAL[i];
					curDate=adjust.TimeEntry.Date;
					if(curDate==previousDate){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(curDate.ToShortDateString());
						row.Cells.Add(curDate.ToString("ddd"));//Abbreviated name of day
					}
					//altered--------------------------------------
					//row.Cells.Add(Lan.g(this,"Adjust"));//2
					//row.ColorText=Color.Red;
					//status--------------------------------------
					//row.Cells.Add("");//3
					//in/out------------------------------------------
					row.Cells.Add("");//4
					//time-----------------------------
					if(adjust.PtoDefNum==0) {
						row.Cells.Add("(Adjust)");//5 Out column
					}
					else { 
						row.Cells.Add(Defs.GetDef(DefCat.TimeCardAdjTypes,adjust.PtoDefNum).ItemName);//5
					}
					row.Cells[row.Cells.Count-1].ColorText=Color.Red;
					//total-------------------------------
					row.Cells.Add("");//6
					//Adjust------------------------------
					if(adjust.IsUnpaidProtectedLeave) {
						row.Cells.Add("");//7
					}
					else if(adjust.PtoDefNum==0) {
						daySpan+=adjust.RegHours;//might be negative
						weekSpan+=adjust.RegHours;
						periodSpan+=adjust.RegHours;
						row.Cells.Add(ClockEvents.Format(adjust.RegHours));//7
					} 
					else {
						ptoSpan+=adjust.PtoHours;
						row.Cells.Add("");//7
					}
					//Rate2-------------------------------
					row.Cells.Add("");//8
					//PTO------------------------------
					row.Cells.Add(ClockEvents.Format(adjust.PtoHours));//9
					//Overtime------------------------------
					otspan+=adjust.OTimeHours;
					row.Cells.Add(ClockEvents.Format(adjust.OTimeHours));//10
					//Unpaid Protected Leave (PL)------------------------------------
					if(adjust.IsUnpaidProtectedLeave) {
						row.Cells.Add(ClockEvents.Format(adjust.RegHours));
						unpaidProtectedLeaveSpan+=adjust.RegHours;
					}
					else {
						row.Cells.Add("");
					}
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==mergedAL.Count-1//if this is the last row
						|| GetDateForRow(i+1) != curDate)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(daySpan));//
						daySpan=new TimeSpan(0);
					}
					else{
						row.Cells.Add("");
					}
					//Weekly-------------------------------------
					WeeklyTotals[i]=weekSpan;
					if(IsBreaks){
						row.Cells.Add("");
					}
					//if this is the last entry for a given week
					else if(i==mergedAL.Count-1//if this is the last row 
						|| cal.GetWeekOfYear(GetDateForRow(i+1),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= cal.GetWeekOfYear(adjust.TimeEntry.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						GridCell cell=new GridCell(ClockEvents.Format(weekSpan));
						cell.ColorText=Color.Black;
						row.Cells.Add(cell);
						weekSpan=new TimeSpan(0);
					}
					else {
						row.Cells.Add("");
					}
					//Clinic-----------------------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(adjust.ClinicNum));
					}
					//Note-----------------------------------------
					row.Cells.Add(adjust.Note);
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(IsBreaks){
				labelRegularTime.Visible=false;
				labelOvertime.Visible=false;
				labelRateTwo.Visible=false;
				labelPTO.Visible=false;
				textTotal.Visible=false;
				textTotal2.Visible=false;
				textOvertime.Visible=false;
				textOvertime2.Visible=false;
				textRateTwo.Visible=false;
				textRateTwo2.Visible=false;
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
				textTotal.Visible=true;
				textTotal2.Visible=true;
				textOvertime.Visible=true;
				textOvertime2.Visible=true;
				textRateTwo.Visible=true;
				textRateTwo2.Visible=true;
				labelUnpaidProtectedLeave.Visible=true;
				textUnpaidProtectedLeave.Visible=true;
				textUnpaidProtectedLeave2.Visible=true;
				textTotal.Text=periodSpan.ToStringHmm();
				textOvertime.Text=otspan.ToStringHmm();
				textRateTwo.Text=rate2span.ToStringHmm();
				textPTO.Text=ptoSpan.ToStringHmm();
				textUnpaidProtectedLeave.Text=unpaidProtectedLeaveSpan.ToStringHmm();
				textTotal2.Text=periodSpan.TotalHours.ToString("n");
				textOvertime2.Text=otspan.TotalHours.ToString("n");
				textRateTwo2.Text=rate2span.TotalHours.ToString("n");
				textPTO2.Text=ptoSpan.TotalHours.ToString("n");
				textUnpaidProtectedLeave2.Text=unpaidProtectedLeaveSpan.TotalHours.ToString("n");
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
				using FormTimeAdjustEdit FormT=new FormTimeAdjustEdit(adjust);
				FormT.ShowDialog();
			}
			else {
				ClockEvent ce=(ClockEvent)gridMain.ListGridRows[e.Row].Tag;
				using FormClockEventEdit FormCEE=new FormClockEventEdit(ce);
				FormCEE.ShowDialog();
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
			linesPrinted=0;
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,
				Lan.g(this,"Time card for")+" "+EmployeeCur.LName+","+EmployeeCur.FName+" "+Lan.g(this,"printed"),
				new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
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
			SolidBrush brush=new SolidBrush(Color.Black);
			Pen pen=new Pen(Color.Black);
			//Title
			str=EmployeeCur.FName+" "+EmployeeCur.LName;
			str+="\r\n"+Lan.g(this,"Note")+": "+_timeAdjustNote.Note.ToString();
			int threeLineHeight=(int)e.Graphics.MeasureString("1\r\n2\r\n3",fontTitle).Height;
			int marginBothSides=(int)xPos*2;//110
			int noteStringHeight=(int)e.Graphics.MeasureString(str,fontTitle,e.PageBounds.Width-marginBothSides).Height;
			int rectHeight=Math.Min(noteStringHeight,threeLineHeight);
			StringFormat noteStringFormat=new StringFormat{ Trimming=StringTrimming.Word };
			g.DrawString(str,fontTitle,brush,new RectangleF(xPos,yPos,e.PageBounds.Width-marginBothSides,rectHeight),noteStringFormat);
			yPos+=rectHeight+5;//+5 pixels for a small space between columns and title area.
			//define columns
			int[] colW=new int[13];
			if(PrefC.HasClinicsEnabled) {
				colW=new int[14];
			}
			colW[0]=70;//Date
			colW[1]=45;//Day: Column starts to wrap at 32 pixels, however added padding to 45 to allow room for language translations
			colW[2]=60;//In/Out
			colW[3]=60;//Out/In
			colW[4]=50;//Total
			colW[5]=45;//Adjust: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[6]=45;//Rate 2: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[7]=45;//PTO: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[8]=45;//OT: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[9]=45;//PL: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[10]=50;//Day
			colW[11]=50;//Week
			colW[12]=165;//Note
			if(PrefC.HasClinicsEnabled) {
				colW[12]=50;//Clinic
				colW[13]=115;//Note: Reduce width when Clinic column is added so that we do not exceed the margin.
			}
			int[] colPos=new int[colW.Length+1];
			colPos[0]=45;
			for(int i=1;i<colPos.Length;i++) {
				colPos[i]=colPos[i-1]+colW[i-1];
			}
			string[] ColCaption=new string[13];
			if(PrefC.HasClinicsEnabled) {
				ColCaption=new string[14];
			}
			ColCaption[0]=Lan.g(this,"Date");
			ColCaption[1]=Lan.g(this,"Day");
			if(radioBreaks.Checked) {
				ColCaption[2]=Lan.g(this,"Out");
				ColCaption[3]=Lan.g(this,"In");
			}
			else {
				ColCaption[2]=Lan.g(this,"In");
				ColCaption[3]=Lan.g(this,"Out");
			}
			ColCaption[4]=Lan.g(this,"Total");
			ColCaption[5]=Lan.g(this,"Adjust");
			ColCaption[6]=Lan.g(this,"Rate 2");
			ColCaption[7]=Lan.g(this,"PTO");
			ColCaption[8]=Lan.g(this,"OT");
			ColCaption[9]=Lan.g(this,"PL");
			ColCaption[10]=Lan.g(this,"Day");
			ColCaption[11]=Lan.g(this,"Week");
			ColCaption[12]=Lan.g(this,"Note");
			if(PrefC.HasClinicsEnabled) {
				ColCaption[12]=Lan.g(this,"Clinic");
				ColCaption[13]=Lan.g(this,"Note");
			}
			//column headers-----------------------------------------------------------------------------------------
			e.Graphics.FillRectangle(Brushes.LightGray,colPos[0],yPos,colPos[colPos.Length-1]-colPos[0],18);
			e.Graphics.DrawRectangle(pen,colPos[0],yPos,colPos[colPos.Length-1]-colPos[0],18);
			for(int i=1;i<colPos.Length;i++) {
				e.Graphics.DrawLine(new Pen(Color.Black),colPos[i],yPos,colPos[i],yPos+18);
			}
			//Prints the Column Titles
			for(int i=0;i<ColCaption.Length;i++) {
				e.Graphics.DrawString(ColCaption[i],fontHeader,brush,colPos[i]+2,yPos+1);
			}
			yPos+=18;
			while(yPos < e.PageBounds.Height-75-50-32-16 && linesPrinted < gridMain.ListGridRows.Count) {
				for(int i=0;i<colPos.Length-1;i++) {
					if(gridMain.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Empty || gridMain.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Black) {
						e.Graphics.DrawString(gridMain.ListGridRows[linesPrinted].Cells[i].Text,font,brush
							,new RectangleF(colPos[i]+2,yPos,colPos[i+1]-colPos[i]-5,font.GetHeight(e.Graphics)));
					}
					else { //The only other color currently supported is red.
						e.Graphics.DrawString(gridMain.ListGridRows[linesPrinted].Cells[i].Text,font,Brushes.Red
							,new RectangleF(colPos[i]+2,yPos,colPos[i+1]-colPos[i]-5,font.GetHeight(e.Graphics)));
					}
				}
				//Column lines		
				for(int i=0;i<colPos.Length;i++) {
					e.Graphics.DrawLine(Pens.Gray,colPos[i],yPos+16,colPos[i],yPos);
				}
				linesPrinted++;
				yPos+=16;
				e.Graphics.DrawLine(new Pen(Color.Gray),colPos[0],yPos,colPos[colPos.Length-1],yPos);
			}
			//bottom line
			//e.Graphics.DrawLine(new Pen(Color.Gray),colPos[0],yPos,colPos[colPos.Length-1],yPos);
			//totals will print on every page for simplicity
			yPos+=10;
			g.DrawString(Lan.g(this,"Regular Time")+": "+textTotal.Text+" ("+textTotal2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Overtime")+": "+textOvertime.Text+" ("+textOvertime2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 2 Time")+": "+textRateTwo.Text+" ("+textRateTwo2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"PTO Time")+": "+textPTO.Text+" ("+textPTO2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Protected Leave")+": "+textUnpaidProtectedLeave.Text+" ("+textUnpaidProtectedLeave2.Text+")",fontHeader,brush,xPos,yPos);
			if(linesPrinted==gridMain.ListGridRows.Count) {
				e.HasMorePages=false;
			}
			else {
				e.HasMorePages=true;
			}
		}

		private void butBrowseEmp_Click(object sender,EventArgs e) {
			SaveNoteToDb();
			int empIndex=0;
			for(int i=0;i<_listEmp.Count;i++) {
				//find current employee index by Employeenum
				if(EmployeeCur.EmployeeNum==_listEmp[i].EmployeeNum) {
					if(sender.Equals(butPrevEmp)) {
						empIndex=i-1;//go to previous employee in list
					}
					else {
						empIndex=i+1;//go to next employee in list
					}
					empIndex=(empIndex+_listEmp.Count)%(_listEmp.Count);//allows wrapping at end of employee list.
					break;
				}
			}
			EmployeeCur=_listEmp[empIndex];
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





















