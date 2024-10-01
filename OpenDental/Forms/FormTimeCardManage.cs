using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using OpenDental.Thinfinity;

namespace OpenDental {
	public partial class FormTimeCardManage:FormODBase {
		private int _idxPayPeriodSelected;
		private DateTime _dateStart;
		private DateTime _dateStop;
		private DataTable _tableMain;
		private string _totalTime;
		private string _overTime;
		private string _rate2Time;
		private string _rate3Time;
		private string _ptoTime;
		private string _totalTime2;
		private string _overTime2;
		private string _rate2Time2;
		private string _rate3Time2;
		private string _ptoTime2;
		private string _unpaidProtectedLeaveTime;
		private string _unpaidProtectedLeaveTime2;
		private int _pagesPrinted;
		private bool _isHeadingPrinted;
		private List<Employee> _listEmployees;
		private List<PayPeriod> _listPayPeriods;
		private TimeAdjust _timeAdjustNote;
		private GridOD gridPrint;

		public FormTimeCardManage(List<Employee> listEmployees) {
			InitializeComponent();
			InitializeLayoutManager();
			_listEmployees=listEmployees;
			Lan.F(this);
		}

		private void FormTimeCardManage_Load(object sender,EventArgs e) {
			_idxPayPeriodSelected=PayPeriods.GetForDate(DateTime.Today);
			if(_idxPayPeriodSelected==-1) {
				MsgBox.Show(this,"At least one pay period needs to exist before you can manage time cards.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!PrefC.HasClinicsEnabled) {
				comboClinic.Visible=false;
			}
			else {//clinics
				if(!Security.CurUser.ClinicIsRestricted) {
					comboClinic.IncludeAll=true;
				}
				comboClinic.ClinicNumSelected=Clinics.ClinicNum;
			}
			_listPayPeriods=PayPeriods.GetDeepCopy();
			LayoutMenu();
			FillPayPeriod();
			butTimeCardBenefits.Visible=PrefC.IsODHQ && Security.IsAuthorized(EnumPermType.TimecardsEditAll,true);
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			//Setup-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemReports=new MenuItemOD("Reports");
			menuMain.Add(menuItemReports);
			menuItemReports.Add("Print Grid",butPrintGrid_Click);
			menuItemReports.Add("Export Grid",butExportGrid_Click);
			menuItemReports.Add("Export ADP",butExportADP_Click);
			menuItemReports.Add("Export ADP Run",butExportADPRun_Click);
			menuMain.EndUpdate();
		}

		private void FormTimeCardManage_Shown(object sender,EventArgs e) {
			FillMain();//Is exected after Load() is complete so that the window can finish disiplaying before spending time to run queries.
		}

		///Queries the database
		private GridOD FillMain(bool isForGridPrint=false) {
			GridOD grid;
			if(!isForGridPrint) {
				grid=gridMain;
			}
			else {
				grid=new GridOD();
			}
			long clinicNum=0;
			bool isAll=false;
			if(PrefC.HasClinicsEnabled) {
				if(Security.CurUser.ClinicIsRestricted) {
					clinicNum=comboClinic.ClinicNumSelected;
				}
				else {//All and Headquarters are the first two available options.
					if(comboClinic.IsAllSelected) {
						isAll=true;
					}
					else if(comboClinic.IsUnassignedSelected) {
						//Do nothing since the defaults are this selection
					}
					else {//A specific clinic was selected.
						clinicNum=comboClinic.ClinicNumSelected;
					}
				}
			}
			else {
				isAll=true;
			}
			if(!isForGridPrint) {
				_tableMain=ClockEvents.GetTimeCardManage(_dateStart,_dateStop,clinicNum,isAll);
			}
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Employee"),140);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Total Hrs"),65);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate1"),60);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate1 OT"),65);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),60);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2 OT"),65);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate3"),60);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate3 OT"),65);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PTO"),55);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PL Hrs"),60);
			col.TextAlign=HorizontalAlignment.Right;
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),100);
			if(!isForGridPrint) {
				col.IsWidthDynamic=true;//Dynamic width messes up printed grid.
			}
			grid.Columns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_tableMain.Rows.Count;i++) {
				row=new GridRow();
				//row.Cells.Add(Employees.GetNameFL(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())));
				row.Cells.Add(_tableMain.Rows[i]["lastName"]+", "+_tableMain.Rows[i]["firstName"]);
				if(PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon)) {
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["totalHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate1Hours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate1OTHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate2Hours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate2OTHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate3Hours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate3OTHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["PTOHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["protectedLeaveHours"].ToString()).TotalHours.ToString("n"));
				}
				else if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {//Colon format with seconds
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["totalHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate1Hours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate1OTHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate2Hours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate2OTHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate3Hours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate3OTHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["PTOHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["protectedLeaveHours"].ToString()).ToStringHmmss());
				}
				else {//Colon format without seconds
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["totalHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate1Hours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate1OTHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate2Hours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate2OTHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate3Hours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["rate3OTHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["PTOHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(_tableMain.Rows[i]["protectedLeaveHours"].ToString()).ToStringHmm());
				}
				row.Cells.Add(_tableMain.Rows[i]["Note"].ToString());
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			return grid;
		}

		///<summary>SelectedPayPeriod should already be set.  This simply fills the screen with that data.</summary>
		private void FillPayPeriod() {
			_dateStart=_listPayPeriods[_idxPayPeriodSelected].DateStart;
			_dateStop=_listPayPeriods[_idxPayPeriodSelected].DateStop;
			textDateStart.Text=_dateStart.ToShortDateString();
			textDateStop.Text=_dateStop.ToShortDateString();
			if(_listPayPeriods[_idxPayPeriodSelected].DatePaycheck.Year<1880) {
				textDatePaycheck.Text="";
			}
			else {
				textDatePaycheck.Text=_listPayPeriods[_idxPayPeriodSelected].DatePaycheck.ToShortDateString();
			}
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			//FormTimeCard does some list sorting so we need to break the pass by reference chain.
			List<Employee> listEmployeesCopy=_listEmployees.Select(x => x.Copy()).ToList();
			using FormTimeCard formTimeCard=new FormTimeCard(listEmployeesCopy);
			formTimeCard.IsByLastName=true;
			formTimeCard.EmployeeCur=Employees.GetEmp(PIn.Long(_tableMain.Rows[e.Row]["EmployeeNum"].ToString()));
			formTimeCard.IdxPayPeriodSelected=_idxPayPeriodSelected;
			formTimeCard.ShowDialog();
			FillMain();
		}

		///<summary>This is a modified version of FormTimeCard.FillMain().  It fills one time card per employee.</summary>
		private GridOD GetGridForPrinting(Employee employee) {
			GridOD gridTimeCard=new GridOD();
			gridTimeCard.TranslationName="";
			List<ClockEvent> listClockEvents=ClockEvents.Refresh(employee.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),false);
			List<TimeAdjust> listTimeAdjusts=TimeAdjusts.Refresh(employee.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			#region Hide time card note (we will show it at the top of the printout above the grid)
			DateTime datePayPeriodStart=_listPayPeriods[_idxPayPeriodSelected].DateStart.Date;
			DateTime dateTimeMidnightFirstDayOfPeriod=new DateTime(datePayPeriodStart.Year,datePayPeriodStart.Month,datePayPeriodStart.Day,0,0,0);
			_timeAdjustNote=TimeAdjusts.GetPayPeriodNote(employee.EmployeeNum,dateTimeMidnightFirstDayOfPeriod);
			if(_timeAdjustNote!=null) { //a note row exists for this pay period.
				listTimeAdjusts.RemoveAll(x => x.TimeAdjustNum==_timeAdjustNote.TimeAdjustNum);
			}
			#endregion
			ArrayList arrayListMerged=new ArrayList();
			for(int i=0;i<listClockEvents.Count;i++) {
				arrayListMerged.Add(listClockEvents[i]);
			}
			for(int i=0;i<listTimeAdjusts.Count;i++) {
				arrayListMerged.Add(listTimeAdjusts[i]);
			}
			ObjectDateComparer objectDateComparer=new ObjectDateComparer();
			arrayListMerged.Sort(objectDateComparer);
			#region gridTimeCard Build Columns
			gridTimeCard.BeginUpdate();
			gridTimeCard.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),45);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"In"),60,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Out"),60,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Total"),50,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Adjust"),45,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),45,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate3"),45,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PTO"),45,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"OT"),45,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PL"),45,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			if(PrefC.IsODHQ) {
				col=new GridColumn(Lan.g(this,"WFH"),45,HorizontalAlignment.Right);
				gridTimeCard.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Day"),50,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Week"),50,HorizontalAlignment.Right);
			gridTimeCard.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),50,HorizontalAlignment.Left);
				gridTimeCard.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),70){ IsWidthDynamic=true };
			gridTimeCard.Columns.Add(col);
			#endregion gridTimeCard Build Columns
			gridTimeCard.ListGridRows.Clear();
			GridRow row;
			TimeSpan[] timeSpanArrayWeeklyTotals=new TimeSpan[arrayListMerged.Count];
			TimeSpan timeSpanAltered=new TimeSpan(0);//used to display altered times
			TimeSpan timeSpanOne=new TimeSpan(0);//used to sum one pair of clock-in/clock-out
			TimeSpan timeSpanOneAdj;
			TimeSpan timeSpanOneOT;
			TimeSpan timeSpanDay=new TimeSpan(0);//used for daily totals.
			TimeSpan timeSpanWeek=new TimeSpan(0);//used for weekly totals.
			TimeSpan timeSpanPTO=new TimeSpan(0);//used for PTO totals.
			TimeSpan timeSpanUnpaidProtectedLeave=new TimeSpan(0);//used for Unpaid Protected Leave totals.
			if(arrayListMerged.Count>0){
				timeSpanWeek=ClockEvents.GetWeekTotal(employee.EmployeeNum,GetDateForRow(0,arrayListMerged));
			}
			TimeSpan timeSpanPeriod=new TimeSpan(0);//used to add up totals for entire page.
			TimeSpan timeSpanOT=new TimeSpan(0);//overtime for the entire period
			TimeSpan timeSpanRate2=new TimeSpan(0);//rate2 hours total
			TimeSpan timeSpanRate3=new TimeSpan(0);//rate3 hours total
			Calendar calendar=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule calendarWeekRule=CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			DateTime dateTimeCurrent=DateTime.MinValue;
			DateTime dateTimePrevious=DateTime.MinValue;
			for(int i=0;i<arrayListMerged.Count;i++){
				row=new GridRow();
				Type type=arrayListMerged[i].GetType();
				row.Tag=arrayListMerged[i];
				dateTimePrevious=dateTimeCurrent;
				#region ClockEvent Row
				if(type==typeof(ClockEvent)){
					ClockEvent clockEvent=(ClockEvent)arrayListMerged[i];
					dateTimeCurrent=clockEvent.TimeDisplayed1.Date;
				//Columns 1 and 2 - Date and Day----------------------------------
					if(dateTimeCurrent==dateTimePrevious){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(dateTimeCurrent.ToShortDateString());
						row.Cells.Add(dateTimeCurrent.ToString("ddd"));//Abbreviated name of day
					}
				//Column 3 - In (or Out if break)---------------------------------
					row.Cells.Add(clockEvent.TimeDisplayed1.ToShortTimeString());
					if(clockEvent.TimeEntered1!=clockEvent.TimeDisplayed1){
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
				//Column 4 - Out (or In if break)---------------------------------
					if(clockEvent.TimeDisplayed2.Year<1880){
						row.Cells.Add("");//not clocked out yet
					}
					else{
						row.Cells.Add(clockEvent.TimeDisplayed2.ToShortTimeString());
						if (clockEvent.TimeEntered2!=clockEvent.TimeDisplayed2)
						{
							row.Cells[row.Cells.Count-1].ColorText = Color.Red;
						}
					}
				//Column 5 - Total------------------------------------------------
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
						if(clockEvent.IsWorkingHome){
							row.Cells.Add("X");//Working from home, fill with X
						}
						else {
							row.Cells.Add("");//No WFH on adjustments, leave blank
						}
					}
				//Column 13 (or 12 if no WFH) - Day (daily total)-----------------
					//if this is the last entry for a given date
					if(i==arrayListMerged.Count-1//if this is the last row
						|| GetDateForRow(i+1,arrayListMerged) != dateTimeCurrent)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(timeSpanDay));
						timeSpanDay=new TimeSpan(0);
					}
					else{//not the last entry for the day
						row.Cells.Add("");
					}
				//Column 14 (or 13 if no WFH) - Week (weekly total)---------------
					timeSpanArrayWeeklyTotals[i]=timeSpanWeek;
					//if this is the last entry for a given week
					if(i==arrayListMerged.Count-1//if this is the last row 
						|| calendar.GetWeekOfYear(GetDateForRow(i+1,arrayListMerged),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= calendar.GetWeekOfYear(clockEvent.TimeDisplayed1.Date,calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						row.Cells.Add(ClockEvents.Format(timeSpanWeek));
						timeSpanWeek=new TimeSpan(0);
					}
					else {
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
					TimeAdjust timeAdjust=(TimeAdjust)arrayListMerged[i];
					dateTimeCurrent=timeAdjust.TimeEntry.Date;
				//Columns 1 and 2 - Date and Day----------------------------------
					if(dateTimeCurrent==dateTimePrevious){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(dateTimeCurrent.ToShortDateString());
						row.Cells.Add(dateTimeCurrent.ToString("ddd"));//Abbreviated name of day
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
						row.Cells.Add("");
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
					row.Cells.Add(ClockEvents.Format(timeAdjust.OTimeHours));//10
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
						row.Cells.Add("");//No WFH on adjustments, leave blank
					}
				//Column 13 (or 12 if no WFH) - Day (daily total)-----------------
					//if this is the last entry for a given date
					if(i==arrayListMerged.Count-1//if this is the last row
						|| GetDateForRow(i+1,arrayListMerged) != dateTimeCurrent)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(timeSpanDay));
						timeSpanDay=new TimeSpan(0);
					}
					else{
						row.Cells.Add("");
					}
				//Column 14 (or 13 if no WFH) - Week (weekly total)---------------
					timeSpanArrayWeeklyTotals[i]=timeSpanWeek;
					//if this is the last entry for a given week
					if(i==arrayListMerged.Count-1//if this is the last row 
						|| calendar.GetWeekOfYear(GetDateForRow(i+1,arrayListMerged),calendarWeekRule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
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
					row.Cells[row.Cells.Count-1].ColorText=Color.Red;
				}
				#endregion
				gridTimeCard.ListGridRows.Add(row);
			}
			gridTimeCard.EndUpdate();
			_totalTime=timeSpanPeriod.ToStringHmm();
			_overTime=timeSpanOT.ToStringHmm();
			_rate2Time=timeSpanRate2.ToStringHmm();
			_rate3Time=timeSpanRate3.ToStringHmm();
			_ptoTime=timeSpanPTO.ToStringHmm();
			_unpaidProtectedLeaveTime=timeSpanUnpaidProtectedLeave.ToStringHmm();
			_totalTime2=timeSpanPeriod.TotalHours.ToString("n");
			_overTime2=timeSpanOT.TotalHours.ToString("n");
			_rate2Time2=timeSpanRate2.TotalHours.ToString("n");
			_rate3Time2=timeSpanRate3.TotalHours.ToString("n");
			_ptoTime2=timeSpanPTO.TotalHours.ToString("n");
			_unpaidProtectedLeaveTime2=timeSpanUnpaidProtectedLeave.TotalHours.ToString("n");
			return gridTimeCard;
		}

		private DateTime GetDateForRow(int i,ArrayList arrayListMerged){
			if(arrayListMerged[i].GetType()==typeof(ClockEvent)){
				return ((ClockEvent)arrayListMerged[i]).TimeDisplayed1.Date;
			}
			else if(arrayListMerged[i].GetType()==typeof(TimeAdjust)){
				return ((TimeAdjust)arrayListMerged[i]).TimeEntry.Date;
			}
			return DateTime.MinValue;
		}

		//Prints one timecard for each employee.
		private void butPrintAll_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"No time cards to print.");
				return;
			}
			_pagesPrinted=0;
			PrintoutOrientation printoutOrientation=PrintoutOrientation.Portrait;
			if(PrefC.IsODHQ) {
				printoutOrientation=PrintoutOrientation.Landscape; //Switching for extra WFH column
			}
			PrinterL.TryPreview(pd2_PrintPage,
				Lan.g(this,Lans.g(this,"Employee time cards printed")),
				printoutOrientation:printoutOrientation,
				totalPages:gridMain.ListGridRows.Count
			);
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			PrintEveryTimeCard(sender,e);
		}

		private void PrintEveryTimeCard(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			//A preview of every single emp on their own page will show up. User will print from there.
			Employee employee=Employees.GetEmp(PIn.Long(_tableMain.Rows[_pagesPrinted]["EmployeeNum"].ToString()));
			PrintTimeCard(sender,e,employee,gridMain.ListGridRows.Count);	
		}

		///<summary>Print timecards for selected employees only.</summary>
		private void butPrintSelected_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No employees selected, please select one or more employees or click 'Print All' to print all employees.");
				return;
			}
			_pagesPrinted=0;
			PrintoutOrientation printoutOrientation=PrintoutOrientation.Portrait;
			if(PrefC.IsODHQ) {
				printoutOrientation=PrintoutOrientation.Landscape; //Switching for extra WFH column
			}
			PrinterL.TryPreview(pd2_PrintPageSelective,
				Lan.g(this,"Employee time cards printed"),
				printoutOrientation:printoutOrientation,
				totalPages:gridMain.SelectedIndices.Length
			);
		}

		///<summary>Similar to pd2_PrintPage except it iterates through selected indices instead of all indices.</summary>
		private void pd2_PrintPageSelective(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			PrintEmployeeTimeCard(sender,e);
		}

		private void PrintEmployeeTimeCard(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
			//A preview of every single emp on their own page will show up. User will print from there.
			Employee employee=Employees.GetEmp(PIn.Long(_tableMain.Rows[gridMain.SelectedIndices[_pagesPrinted]]["EmployeeNum"].ToString()));
			PrintTimeCard(sender,e,employee,gridMain.SelectedIndices.Length);
		}

		private void PrintTimeCard(object sender, System.Drawing.Printing.PrintPageEventArgs e, Employee employee,int pagesToPrint) {
			using Graphics g=e.Graphics;
			GridOD gridTimeCard=GetGridForPrinting(employee);
			int linesPrinted=0;
			//Create a timecardgrid for this employee?
			float yPos=75;
			float xPos=55;
			string str;
			using Font font=new Font(FontFamily.GenericSansSerif,8);
			using Font fontTitle=new Font(FontFamily.GenericSansSerif,11,FontStyle.Bold);
			using Font fontHeader=new Font(FontFamily.GenericSansSerif,8,FontStyle.Bold);
			using SolidBrush solidBrush=new SolidBrush(Color.Black);
			using Pen pen=new Pen(Color.Black);
			//Title
			str=employee.FName+" "+employee.LName;
			str+="\r\n"+Lan.g(this,"Note")+": "+_timeAdjustNote?.Note.ToString()??"";
			int threeLineHeight=(int)e.Graphics.MeasureString("1\r\n2\r\n3",fontTitle).Height;
			int marginBothSides=(int)xPos*2;//110
			int heightNoteString=(int)e.Graphics.MeasureString(str,fontTitle,e.PageBounds.Width-marginBothSides).Height;
			int heightRect=Math.Min(heightNoteString,threeLineHeight);
			using StringFormat stringFormatNote=new StringFormat{ Trimming=StringTrimming.Word };
			g.DrawString(str,fontTitle,solidBrush,new RectangleF(xPos,yPos,e.PageBounds.Width-marginBothSides,heightRect),stringFormatNote);
			yPos+=heightRect+5;//+5 pixels for a small space between columns and title area.
			//define columns
			int[] colW=new int[14];
			if(PrefC.HasClinicsEnabled || PrefC.IsODHQ) {
				colW=new int[15];
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				colW=new int[16];
			}
			colW[0]=70;//Date
			colW[1]=45;//Day: Column starts to wrap at 32 pixels, however added padding to 45 to allow room for language translations
			colW[2]=60;//In/Out
			colW[3]=60;//Out/In
			colW[4]=45;//Total
			colW[5]=45;//Adjust: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[6]=45;//Rate 2: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[7]=45;//Rate 3: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[8]=45;//PTO: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[9]=45;//OT: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[10]=45;//PL: Column start to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[11]=45;//Day
			colW[12]=50;//Week
			colW[13]=130;//Note
			if(PrefC.IsODHQ) {
				colW[11]=45;//WFH
				colW[12]=45;//Day
				colW[13]=50;//Week
				colW[14]=300;//Note
			}
			else if(PrefC.HasClinicsEnabled) {
				colW[13]=50;//Clinic
				colW[14]=80;//Note: Reduce width when Clinic column is added so that we do not exceed the margin.
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				colW[14]=100;//Clinic
				colW[15]=200;//Note: Reduce width when Clinic column is added so that we do not exceed the margin.
			}
			int[] colPos=new int[colW.Length+1];
			colPos[0]=45;
			for(int i=1;i<colPos.Length;i++) {
				colPos[i]=colPos[i-1]+colW[i-1];
			}
			string[] ColCaption=new string[14];
			if(PrefC.HasClinicsEnabled || PrefC.IsODHQ) {
				ColCaption=new string[15];
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				ColCaption=new string[16];
			}
			ColCaption[0]=Lan.g(this,"Date");
			ColCaption[1]=Lan.g(this,"Day");
			ColCaption[2]=Lan.g(this,"In");
			ColCaption[3]=Lan.g(this,"Out");
			ColCaption[4]=Lan.g(this,"Total");
			ColCaption[5]=Lan.g(this,"Adjust");
			ColCaption[6]=Lan.g(this,"Rate 2");
			ColCaption[7]=Lan.g(this,"Rate 3");
			ColCaption[8]=Lan.g(this,"PTO");
			ColCaption[9]=Lan.g(this,"OT");
			ColCaption[10]=Lan.g(this,"PL");
			ColCaption[11]=Lan.g(this,"Day");
			ColCaption[12]=Lan.g(this,"Week");
			ColCaption[13]=Lan.g(this,"Note");
			if(PrefC.IsODHQ) {
				ColCaption[11]=Lan.g(this,"WFH");
				ColCaption[12]=Lan.g(this,"Day");
				ColCaption[13]=Lan.g(this,"Week");
				ColCaption[14]=Lan.g(this,"Note");
			}
			else if(PrefC.HasClinicsEnabled) {
				ColCaption[13]=Lan.g(this,"Clinic");
				ColCaption[14]=Lan.g(this,"Note");
			}
			if(PrefC.HasClinicsEnabled && PrefC.IsODHQ) {
				ColCaption[14]=Lan.g(this,"Clinic");
				ColCaption[15]=Lan.g(this,"Note");
			}
			//column headers-----------------------------------------------------------------------------------------
			e.Graphics.FillRectangle(Brushes.LightGray,colPos[0],yPos,colPos[colPos.Length-1]-colPos[0],18);
			e.Graphics.DrawRectangle(pen,colPos[0],yPos,colPos[colPos.Length-1]-colPos[0],18);
			for(int i=1;i<colPos.Length;i++) {
				e.Graphics.DrawLine(new Pen(Color.Black),colPos[i],yPos,colPos[i],yPos+18);
			}
			//Prints the Column Titles
			for(int i=0;i<ColCaption.Length;i++) {
				e.Graphics.DrawString(ColCaption[i],fontHeader,solidBrush,colPos[i]+2,yPos+1);
			}
			yPos+=18;
			while(yPos < e.PageBounds.Height-75-50-32-16 && linesPrinted < gridTimeCard.ListGridRows.Count) {
				for(int i=0;i<colPos.Length-1;i++) {
					if(gridTimeCard.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Empty || gridTimeCard.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Black) {
						e.Graphics.DrawString(gridTimeCard.ListGridRows[linesPrinted].Cells[i].Text,font,solidBrush
							,new RectangleF(colPos[i]+2,yPos,colPos[i+1]-colPos[i]-5,font.GetHeight(e.Graphics)));
					}
					else { //The only other color currently supported is red.
						e.Graphics.DrawString(gridTimeCard.ListGridRows[linesPrinted].Cells[i].Text,font,Brushes.Red
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
			//totals will print on every page for simplicity
			yPos+=10;
			g.DrawString(Lan.g(this,"Regular Time")+": "+_totalTime+" ("+_totalTime2+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Overtime")+": "+_overTime+" ("+_overTime2+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 2 Time")+": "+_rate2Time+" ("+_rate2Time2+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 3 Time")+": "+_rate3Time+" ("+_rate3Time2+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"PTO Time")+": "+_ptoTime+" ("+_ptoTime2+")",fontHeader,solidBrush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Protected Leave")+": "+_unpaidProtectedLeaveTime+" ("+_unpaidProtectedLeaveTime2+")",fontHeader,solidBrush,xPos,yPos);
			_pagesPrinted++;
			if(pagesToPrint==_pagesPrinted) {
				_pagesPrinted=0;
				e.HasMorePages=false;
			}
			else {
				e.HasMorePages=true;
			}
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(_idxPayPeriodSelected==0){
				return;
			}
			_idxPayPeriodSelected--;
			FillPayPeriod();
			FillMain();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(_idxPayPeriodSelected==_listPayPeriods.Count-1) {
				return;
			}
			_idxPayPeriodSelected++;
			FillPayPeriod();
			FillMain();
		}

		private void butDaily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.TimecardsEditAll)) {
				return;
			}
			string errorAllEmployees=TimeCardRules.ValidateOvertimeRules(new List<long>{0});//Validates the "all employees" timecard rules first.
			if(errorAllEmployees.Length>0) {
				MessageBox.Show(errorAllEmployees);
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"No employees selected. Would you like to run calculations for all employees?")) {
					return;
				}
				gridMain.SetAll(true);
			}
			Cursor=Cursors.WaitCursor;
			string aggregateErrors="";
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				try {
					TimeCardRules.CalculateDailyOvertime(Employees.GetEmp(PIn.Long(_tableMain.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()))
						,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					aggregateErrors+=ex.Message+"\r\n";
				}
			}
			Cursor=Cursors.Default;
			//Cache selected indicies, fill grid, reselect indicies.
			List<int> listIndicesSelected=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listIndicesSelected.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listIndicesSelected.Count;i++) {
				gridMain.SetSelected(listIndicesSelected[i],true);
			}
			if(aggregateErrors=="") {
				MsgBox.Show(this,"Done.");
			}
			else {
				MessageBox.Show(this,Lan.g(this,"Time cards were not calculated for some Employees for the following reasons")+":\r\n"+aggregateErrors);
			}
		}

		private void butWeekly_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.TimecardsEditAll)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"No employees selected. Would you like to run calculations for all employees?")) {
					return;
				}
				gridMain.SetAll(true);
			}
			Cursor=Cursors.WaitCursor;
			string aggregateErrors="";
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				try {
					TimeCardRules.CalculateWeeklyOvertime(Employees.GetEmp(PIn.Long(_tableMain.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()))
						,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					aggregateErrors+=ex.Message+"\r\n";
				}
			}
			Cursor=Cursors.Default;
			//Cache selected indices, fill grid, reselect indices.
			List<int> listIndicesSelected=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				listIndicesSelected.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listIndicesSelected.Count;i++) {
				gridMain.SetSelected(listIndicesSelected[i],true);
			}
			//Done or Error messages.
			if(aggregateErrors=="") {
				MsgBox.Show(this,"Done.");
			}
			else {
				MessageBox.Show(this,Lan.g(this,"Time cards were not calculated for some Employees for the following reasons")+":\r\n"+aggregateErrors);
			}
		}

		private void butClearManual_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This cannot be undone. Would you like to continue?")) {
				return;
			}
			//List<Employee> employeesList = new List<Employee>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				try {
					TimeCardRules.ClearManual(PIn.Long(_tableMain.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()),PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
			//Cach selected indicies, fill grid, reselect indicies.
			List<int> listIndicesSelected=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listIndicesSelected.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listIndicesSelected.Count;i++) {
				gridMain.SetSelected(listIndicesSelected[i],true);
			}
		}

		private void butClearAuto_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This cannot be undone, but you can run the Calc buttons again later.  Would you like to continue?")) {
				return;
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				try {
					TimeCardRules.ClearAuto(PIn.Long(_tableMain.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()),PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
			//Cach selected indicies, fill grid, reselect indicies.
			List<int> listIndicesSelected=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listIndicesSelected.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listIndicesSelected.Count;i++) {
				gridMain.SetSelected(listIndicesSelected[i],true);
			}
		}

		///<summary>Print exactly what is showing in gridMain. (Including rows that do not fit in the UI.)</summary>
		private void butPrintGrid_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			gridPrint=FillMain(true);
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Printed employee time card grid."));
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			using Graphics g=e.Graphics;
			string text;
			using Font headingFont=new Font("Arial",13,FontStyle.Bold);
			int y=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			int headingPrintH=0;
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Pay Period")+": "+textDateStart.Text+" - "+textDateStop.Text+"\r\n"
					+Lan.g(this,"Paycheck Date")+": "+textDatePaycheck.Text;
				if(PrefC.HasClinicsEnabled) {
					text+="\r\n"+Lan.g(this,"Clinic")+": ";
					if(Security.CurUser.ClinicIsRestricted) {
						text+=Clinics.GetAbbr(comboClinic.ClinicNumSelected);
					}
					else {//All and Headquarters are the first two available options.
						if(comboClinic.IsAllSelected) {
							text+=Lan.g(this,"All");
						}
						else if(comboClinic.IsUnassignedSelected) {
							text+=Lan.g(this,"Headquarters");
						}
						else {//A specific clinic was selected.
							text+=Clinics.GetAbbr(comboClinic.ClinicNumSelected);
						}
					}
				}
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,y);
				if(PrefC.HasClinicsEnabled) {
					y+=75;//To move the grid down three lines to make room for the header text
				}
				else {
					y+=50;//To move the grid down two lines to make room for the header text
				}
				_isHeadingPrinted=true;
				headingPrintH=y;
			}
			#endregion
			y=gridPrint.PrintPage(g,_pagesPrinted,rectangleBounds,headingPrintH);
			_pagesPrinted++;
			if(y==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillMain();
		}

		///<summary>Exports MainTable (a data table) not the actual OD Grid. This allows for EmployeeNum and ADPNum without having to perform any lookups.</summary>
		private void butExportGrid_Click(object sender,EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if(!ODBuild.IsThinfinity()) {
				if(folderBrowserDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string headers="";
			for(int i=0;i<_tableMain.Columns.Count;i++) {
				if(i>0) {
					headers+="\t";
				}
				headers+=_tableMain.Columns[i].ColumnName;
			}
			stringBuilder.AppendLine(headers);
			for(int i=0;i<_tableMain.Rows.Count;i++) {
				string row="";
				for(int c=0;c<_tableMain.Columns.Count;c++) {
					if(c>0) {
						row+="\t";
					}
					switch(_tableMain.Columns[c].ColumnName) {
						case "PayrollID":
						case "EmployeeNum":
						case "firstName":
						case "lastName":
						case "Note":
							row+=_tableMain.Rows[i][c].ToString().Replace("\t","").Replace("\r\n",";  ");
							break;
						case "totalHours":
						case "rate1Hours":
						case "rate1OTHours":
						case "rate2Hours":
						case "rate2OTHours":
						case "rate3Hours":
						case "rate3OTHours":
						case "PTOHours":
						case "protectedLeaveHours":
							//Time must be formatted differently.
							if(PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon)) {
								row+=PIn.Time(_tableMain.Rows[i][c].ToString()).TotalHours.ToString("n");
							}
							else if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {//Colon format with seconds
								row+=PIn.Time(_tableMain.Rows[i][c].ToString()).ToStringHmmss();
							}
							else {//Colon format without seconds
								row+=PIn.Time(_tableMain.Rows[i][c].ToString()).ToStringHmm();
							}
							break;
						default:
							//should never happen.
							throw new Exception("Unexpected column found in payroll table : "+_tableMain.Columns[c].ColumnName);
					}//end switch
				}//end columns
				stringBuilder.AppendLine(row);
			}
			string fileName="ODPayroll"+DateTime.Now.ToString("yyyyMMdd_hhmmss")+".TXT";
			if(ODBuild.IsThinfinity()) {
				ThinfinityUtils.ExportForDownload(fileName,stringBuilder.ToString());
				return;
			}
			try {
				System.IO.File.WriteAllText(folderBrowserDialog.SelectedPath+"\\"+fileName,stringBuilder.ToString());
				MessageBox.Show(this,Lan.g(this,"File created")+" : "+folderBrowserDialog.SelectedPath+"\\"+fileName);
			}
			catch(Exception ex) {
				MessageBox.Show(this,"File not created:\r\n"+ex.Message);
			}
		}

		///<summary>Validates format and values and provides aggregate error and warning messages. Will save malformed files anyways.
		///Uses Hours 3 ADP columns to represent PTO.</summary>
		private void butExportADP_Click(object sender,EventArgs e) {
			bool hasVisiblePtoDef=false;
			//Check to confirm if any PTO definitions are hidden, if not show these defs on the timecard
			List<Def> listDefsPtoTypes=Defs.GetDefsForCategory(DefCat.TimeCardAdjTypes);
			for(int i=0;i<listDefsPtoTypes.Count();i++) {
				if(!listDefsPtoTypes[i].IsHidden) {
					hasVisiblePtoDef=true;
					break;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string errors="";
			string warnings="";
			string errorIndent="  ";
			stringBuilder.Append("Co Code,Batch ID,File #"+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?",Employee Name":"")+",Rate Code,Reg Hours,O/T Hours");
			if(hasVisiblePtoDef) { 
				stringBuilder.Append(",Hours 3 Code,Hours 3 Amount");
			} 
			stringBuilder.AppendLine();
			string coCode=PrefC.GetString(PrefName.ADPCompanyCode);
			string batchID=_dateStop.ToString("yyyyMMdd");//max 8 characters
			if(coCode.Length<2 || coCode.Length>3){
				errors+=errorIndent+"Company code must be two to three alpha numeric characters long.  Go to Setup>Manage>TimeCards to edit.\r\n";
			}
			coCode=coCode.PadRight(3,'_');//for two digit company codes.
			for(int i=0;i<_tableMain.Rows.Count;i++) {
				string errorsForEmployee="";
				string warningsForEmployee="";
				string fileNum=_tableMain.Rows[i]["PayrollID"].ToString();
				string employeeName="Error";
				if(PIn.Int(fileNum,hasExceptions:false)<51 || PIn.Int(fileNum,hasExceptions:false)>999999) {
					errorsForEmployee+=errorIndent+"Payroll ID must be between 51 and 999999.\r\n";
				}
				else if(fileNum.Length>6) {
					errorsForEmployee+=errorIndent+"Payroll ID must be less than 6 digits long.\r\n";
				}
				else {//pad payrollIDs that are too short. No effect if payroll ID is 6 digits long.
					fileNum=fileNum.PadLeft(6,'0');
				}
				long employeeNum = PIn.Long(_tableMain.Rows[i]["EmployeeNum"].ToString(),hasExceptions:false);
				if (employeeNum!=0) {
					employeeName=Employees.GetNameFL(Employees.GetEmp(employeeNum));
				}
				string r1hours	=(PIn.TSpan(_tableMain.Rows[i]["rate1Hours"  ].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r1hours=="0.00"){//Was changing Exactly 80.00 hours with 8 hours.
					r1hours="";
				}
				string r1OThours=(PIn.TSpan(_tableMain.Rows[i]["rate1OTHours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r1OThours=="0.00") {
					r1OThours="";
				}
				string r2hours	=(PIn.TSpan(_tableMain.Rows[i]["rate2Hours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r2hours=="0.00") {
					r2hours="";
				}
				string r2OThours=(PIn.TSpan(_tableMain.Rows[i]["rate2OTHours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r2OThours=="0.00") {
					r2OThours="";
				}
				string r3hours  =(PIn.TSpan(_tableMain.Rows[i]["rate3Hours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r3hours=="0.00") {
					r3hours="";
				}
				string r3OThours=(PIn.TSpan(_tableMain.Rows[i]["rate3OTHours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r3OThours=="0.00") {
					r3OThours="";
				}
				string ptoHours=(PIn.TSpan(_tableMain.Rows[i]["PTOHours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(ptoHours=="0.00") {
					ptoHours="";
				}
				string textToAdd="";
				if(r1hours!="" || r1OThours!="" || ptoHours!="") {//no entry should be made unless there are actually hours for this employee.
					textToAdd+=coCode+","+batchID+","+fileNum+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?","+employeeName:"")+",,"+r1hours+","+r1OThours;
					if(hasVisiblePtoDef)	{
						if(ptoHours=="") {
							textToAdd+=",,";
						}
						else {
							textToAdd+=","+"PTO"+","+ptoHours;
						}
					} 
					textToAdd+="\r\n";
				}
				if(r2hours!="" || r2OThours!="") {//no entry should be made unless there are actually hours for this employee.
					textToAdd+=coCode+","+batchID+","+fileNum+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?","+employeeName:"")+",2,"+r2hours+","+r2OThours+"\r\n";
				}
				if(r3hours!="" || r3OThours!="") {//no entry should be made unless there are actually hours for this employee.
					textToAdd+=coCode+","+batchID+","+fileNum+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?","+employeeName:"")+",3,"+r3hours+","+r3OThours+"\r\n";
				}
				if(textToAdd=="") {
					warningsForEmployee+=errorIndent+"No clocked hours.\r\n";// for "+Employees.GetNameFL(Employees.GetEmp(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())))+"\r\n";
				}
				else {
					stringBuilder.Append(textToAdd);
				}
				//validate characters in text.  Allowed values are 32 to 91 and 93 to 122----------------------------------------------------------------
				for(int j=0;j<textToAdd.Length;j++) {
					int charAsInt=(int)textToAdd[j];
					//these are the characters explicitly allowed by ADP per their documentation.
					if(charAsInt>=32 && charAsInt<=122 && charAsInt!=92) {//
						continue;//valid character
					}
					if(charAsInt==10 || charAsInt==13) {//CR LF, not allowed as values but allowed to deliniate rows.
						continue;//valid character
					}
					errorsForEmployee+="Invalid character found (ASCII="+charAsInt+"): "+textToAdd.Substring(j,1)+".\r\n";
				}
				//Aggregate employee errors into aggregate error messages.--------------------------------------------------------------------------------
				if(errorsForEmployee!="") {
					errors+=Employees.GetNameFL(Employees.GetEmp(PIn.Long(_tableMain.Rows[i]["EmployeeNum"].ToString())))+":\r\n"+errorsForEmployee+"\r\n";
				}
				if(warningsForEmployee!="") {
					warnings+=Employees.GetNameFL(Employees.GetEmp(PIn.Long(_tableMain.Rows[i]["EmployeeNum"].ToString())))+":\r\n"+warningsForEmployee+"\r\n";
				}
			}
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if(!ODBuild.IsThinfinity()) {
				if(folderBrowserDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			string fileSuffix=GenerateFileSuffix(folderBrowserDialog.SelectedPath,"\\EPI"+coCode);
			try {
				if(ODBuild.IsThinfinity()) {
					string fileName="EPI"+coCode+fileSuffix+".CSV";
					ThinfinityUtils.ExportForDownload(fileName,stringBuilder.ToString());
				}
				else {
					System.IO.File.WriteAllText(folderBrowserDialog.SelectedPath+"\\EPI"+coCode+fileSuffix+".CSV",stringBuilder.ToString());
				}
				if(errors!="") {
					MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(
						"The following errors will prevent ADP from properly processing this export:\r\n"+errors);
					msgBox.Show(this);
				}
				if(warnings!="") {
					MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(
						"The following warnings were detected:\r\n"+warnings);
					msgBox.Show(this);
				}
				MessageBox.Show(this,Lan.g(this,"File created")+" : "+folderBrowserDialog.SelectedPath+"\\EPI"+coCode+fileSuffix+".CSV");
			}
			catch(Exception ex) {
				MessageBox.Show(this,"File not created:\r\n"+ex.Message);
			}
		}

		///<summary>Validates format and values for ADP Run csv file, providing error and warning messages. Will save malformed files anyways. Reports must contain all 11 columns and only one Pay Frequency.
		///Uses VAC Earnings Code and BASE Rate Code to describe PTO.</summary>
		private void butExportADPRun_Click(object sender,EventArgs e) {
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine("##GENERIC## V1.0"); //Specific header row required for all ADP Run reports
			stringBuilder.AppendLine("IID,Pay Frequency,Pay Period Start Date,Pay Period End Date,Employee ID,Earnings Code,Pay Hours,Dollars,Separate Check,Worked In Dept,Rate Code");
			string ADPRunIID=PrefC.GetString(PrefName.ADPRunIID);					// 1 - IID == ADPRunIID
			PayPeriodInterval payPeriodInterval=PrefC.GetEnum<PayPeriodInterval>(PrefName.PayPeriodIntervalSetting);
			string payFrequency=payPeriodInterval.ToString();							// 2 - Pay Frequency: OD only supports Weekly, Biweekly, or Monthly pay period intervals
			string startDate=textDateStart.Text;													// 3 - Pay Period Start
			string endDate=textDateStop.Text;															// 4 - Pay Period End
			string dollars="";																						// 8 - Dollars, not required
			int separateCheck=0;																					// 9 - Separate Check, ADP defaults 0
			string workedDepartment="";																		//10 - Worked Department, not required		
			//Loop through employees to fill out remaining columns
			for(int i=0;i<_tableMain.Rows.Count;i++) {
				DataRow row=_tableMain.Rows[i];
				string textToAdd=ADPRunIID+","+payFrequency.First()+","+startDate+","+endDate+",";
				textToAdd+=row["PayrollID"].ToString()+","; // 5 - Employee ID == Employee.PayrollID					
				#region Columns 6, 7, 11 - Earnings Code, Pay Hours, Rate Code
				//The ADP Run format requires each type of pay (regular, overtime, etc) and rate of pay (BASE, RATE_2, etc) to be on its own row. 
				//Must check each MainTable column to create appropriate rows for each employee. Only create a row if it contains hours.
				string r1hours=(PIn.TSpan(row["rate1Hours"].ToString())).TotalHours.ToString("F2");	//adp run requires 2 digit precision
				if(r1hours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"REG,"+r1hours+","+dollars+","+separateCheck+","+workedDepartment+",BASE");
				}
				string r1OThours=(PIn.TSpan(row["rate1OTHours"].ToString())).TotalHours.ToString("F2");
				if(r1OThours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"OVT,"+r1OThours+","+dollars+","+separateCheck+","+workedDepartment+",BASE");
				}
				string r2hours=(PIn.TSpan(row["rate2Hours"].ToString())).TotalHours.ToString("F2");
				if(r2hours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"REG,"+r2hours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_2");
				}
				string r2OThours=(PIn.TSpan(row["rate2OTHours"].ToString())).TotalHours.ToString("F2");
				if(r2OThours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"OVT,"+r2OThours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_2");
				}
				string r3hours=(PIn.TSpan(row["rate3Hours"].ToString())).TotalHours.ToString("F2");
				if(r3hours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"REG,"+r3hours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_3");
				}
				string r3OThours=(PIn.TSpan(row["rate3OTHours"].ToString())).TotalHours.ToString("F2");
				if(r3OThours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"OVT,"+r3OThours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_3");
				}
				string ptoHours=(PIn.TSpan(row["PTOHours"].ToString())).TotalHours.ToString("F2");
				if(ptoHours!="0.00") {
					stringBuilder.AppendLine(textToAdd+"VAC,"+ptoHours+","+dollars+","+separateCheck+","+workedDepartment+",BASE");
				}
				#endregion
			}//end file contents
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			if(!ODBuild.IsThinfinity()) {
				if(folderBrowserDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			//ADP recommends a filename like "Biweekly-01012021-01152021.csv"
			string fileName=payFrequency+"-"+startDate.Replace("/","")+"-"+endDate.Replace("/","");
			fileName+="-"+GenerateFileSuffix(folderBrowserDialog.SelectedPath,"\\"+fileName+"-");
			//Write to file and inform user of potential errors
			string errors="";
			if(ADPRunIID.IsNullOrEmpty()) {
				errors+="  An IID is required for ADP Run.  Go to Setup>Manage>TimeCards to edit ADPRunIID.\r\n";
			}
			if(!PayFrequencyMatchesDateRange(payPeriodInterval)) {
				errors+="  The pay frequency '"+payPeriodInterval+"' does not match the pay period date range.\r\n";
			}
			try {
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.ExportForDownload(fileName+".CSV",stringBuilder.ToString());
				}
				else {
					System.IO.File.WriteAllText(folderBrowserDialog.SelectedPath+"\\"+fileName+".CSV",stringBuilder.ToString());
				}
			}
			catch(Exception ex) {
				MessageBox.Show(this,"File not created:\r\n"+ex.Message);
				return;
			}
			if(!errors.IsNullOrEmpty()) {
				MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(
					"The following errors will prevent ADP from properly processing this export:\r\n"+errors);
				msgBox.Show(this);
			}
			MessageBox.Show(this,Lan.g(this,"File created")+" : "+folderBrowserDialog.SelectedPath+"\\"+fileName+".CSV");
		}

		///<summary>Creates a suffix for a file if the passed fileName already exists in the directory. Returns a two character alphanumeric string if a file already exists with that name, or an empty string if not. Will return an error message if all 1296 two character alphanumeric strings are already used as suffixes. </summary>
		private string GenerateFileSuffix(string filePath,string fileName) {
			string fileSuffix="";
			//generate suffix from i
			for(int i=0;i<=1297;i++) {//1296=36*36 to represent all acceptable suffixes for file name consisting of two alphanumeric digits; +1 to catch error. (A-Z, 0-9)
				fileSuffix="";
				if(ODEnvironment.IsCloudServer) {
					return ""; //we don't have a way to check if the file exists.
				}
				if(i==1297) {
					 return "NamingError"; //could not find acceptable file name.
				}
				//First character of suffix
				if(i/36<10) {
					fileSuffix+=(i/36);//truncated to int on purpose.  (0 to 9)
				}
				else {
					fileSuffix+=(Char)((i/36)-10+65);//65='A' in ASCII.  (A to Z)
				}
				//Second character of suffix
				if(i%36<10) {
					fileSuffix+=(i%36);//(0 to 9)
				}
				else {
					fileSuffix+=(Char)((i%36)-10+65);//65='A' in ASCII.  (A to Z)
				}
				//File suffix is now a a two digit alphanumeric string
				if(!System.IO.File.Exists(filePath+fileName+fileSuffix+".CSV")) {
					break;
				}
			}
			return fileSuffix;
		}

		///<summary>Checks to see if the pay period interval matches the date range of the current pay period.</summary>
		private bool PayFrequencyMatchesDateRange(PayPeriodInterval payPeriodInterval) {
			DateTime dateStart=PIn.Date(textDateStart.Text);
			DateTime dateEnd=PIn.Date(textDateStop.Text);
			TimeSpan timeSpan=dateEnd-dateStart;
			//Weekly, BiWeekly, Monthly, and SemiMonthly pay intervals.
			if(  (payPeriodInterval==PayPeriodInterval.Weekly && dateStart.AddDays(6)==dateEnd)
				|| (payPeriodInterval==PayPeriodInterval.BiWeekly && dateStart.AddDays(13)==dateEnd)
				|| (payPeriodInterval==PayPeriodInterval.Monthly && dateStart.AddMonths(1).AddDays(-1)==dateEnd)
				|| (payPeriodInterval==PayPeriodInterval.SemiMonthly && (timeSpan.Days>=5 && timeSpan.Days<=26))) //Can have between 5 and 26 days, inclusive.
			{
				return true;
			}
			return false;
		}

		private void butTimeCardBenefits_Click(object sender,EventArgs e) {
			FormTimeCardBenefitRp formTimeCardBenefitRp = new FormTimeCardBenefitRp();
			formTimeCardBenefitRp.Show();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormTimeCardSetup formTimeCardSetup=new FormTimeCardSetup();
			formTimeCardSetup.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,"Time Card Setup");
		}

	}
}