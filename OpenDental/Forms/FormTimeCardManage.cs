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
		private int SelectedPayPeriod;
		private DateTime DateStart;
		private DateTime DateStop;
		private DataTable MainTable;
		private string totalTime;
		private string overTime;
		private string rate2Time;
		private string ptoTime;
		private string totalTime2;
		private string overTime2;
		private string rate2Time2;
		private string ptoTime2;
		private string _unpaidProtectedLeaveTime;
		private string _unpaidProtectedLeaveTime2;
		private int _pagesPrinted;
		private bool HeadingPrinted;
		private List<Employee> _listEmployees;
		private List<PayPeriod> _listPayPeriods;
		private TimeAdjust _timeAdjustNote;
		private GridOD _printGrid;

		public FormTimeCardManage(List<Employee> listEmployees) {
			InitializeComponent();
			InitializeLayoutManager();
			_listEmployees=listEmployees;
			Lan.F(this);
		}

		private void FormTimeCardManage_Load(object sender,EventArgs e) {
			SelectedPayPeriod=PayPeriods.GetForDate(DateTime.Today);
			if(SelectedPayPeriod==-1) {
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
				comboClinic.SelectedClinicNum=Clinics.ClinicNum;
			}
			_listPayPeriods=PayPeriods.GetDeepCopy();
			LayoutMenu();
			FillPayPeriod();
			butTimeCardBenefits.Visible=PrefC.IsODHQ && Security.IsAuthorized(Permissions.TimecardsEditAll,true);
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
					clinicNum=comboClinic.SelectedClinicNum;
				}
				else {//All and Headquarters are the first two available options.
					if(comboClinic.IsAllSelected) {
						isAll=true;
					}
					else if(comboClinic.IsUnassignedSelected) {
						//Do nothing since the defaults are this selection
					}
					else {//A specific clinic was selected.
						clinicNum=comboClinic.SelectedClinicNum;
					}
				}
			}
			else {
				isAll=true;
			}
			if(!isForGridPrint) {
				MainTable=ClockEvents.GetTimeCardManage(DateStart,DateStop,clinicNum,isAll);
			}
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Employee"),140);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Total Hrs"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate1"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate1 OT"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2 OT"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate3 PTO"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PL Hrs"),75);
			col.TextAlign=HorizontalAlignment.Right;
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),140);
			if(!isForGridPrint) {
				col.IsWidthDynamic=true;//Dynamic width messes up printed grid.
			}
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<MainTable.Rows.Count;i++) {
				row=new GridRow();
				//row.Cells.Add(Employees.GetNameFL(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())));
				row.Cells.Add(MainTable.Rows[i]["lastName"]+", "+MainTable.Rows[i]["firstName"]);
				if(PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon)) {
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["totalHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate1Hours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate1OTHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate2Hours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate2OTHours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate3Hours"].ToString()).TotalHours.ToString("n"));
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["protectedLeaveHours"].ToString()).TotalHours.ToString("n"));
				}
				else if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {//Colon format with seconds
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["totalHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate1Hours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate1OTHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate2Hours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate2OTHours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate3Hours"].ToString()).ToStringHmmss());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["protectedLeaveHours"].ToString()).ToStringHmmss());
				}
				else {//Colon format without seconds
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["totalHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate1Hours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate1OTHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate2Hours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate2OTHours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["rate3Hours"].ToString()).ToStringHmm());
					row.Cells.Add(PIn.Time(MainTable.Rows[i]["protectedLeaveHours"].ToString()).ToStringHmm());
				}
				row.Cells.Add(MainTable.Rows[i]["Note"].ToString());
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			return grid;
		}

		///<summary>SelectedPayPeriod should already be set.  This simply fills the screen with that data.</summary>
		private void FillPayPeriod() {
			DateStart=_listPayPeriods[SelectedPayPeriod].DateStart;
			DateStop=_listPayPeriods[SelectedPayPeriod].DateStop;
			textDateStart.Text=DateStart.ToShortDateString();
			textDateStop.Text=DateStop.ToShortDateString();
			if(_listPayPeriods[SelectedPayPeriod].DatePaycheck.Year<1880) {
				textDatePaycheck.Text="";
			}
			else {
				textDatePaycheck.Text=_listPayPeriods[SelectedPayPeriod].DatePaycheck.ToShortDateString();
			}
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			//FormTimeCard does some list sorting so we need to break the pass by reference chain.
			List<Employee> listEmployeesCopy=_listEmployees.Select(x => x.Copy()).ToList();
			using FormTimeCard FormTC=new FormTimeCard(listEmployeesCopy);
			FormTC.IsByLastName=true;
			FormTC.EmployeeCur=Employees.GetEmp(PIn.Long(MainTable.Rows[e.Row]["EmployeeNum"].ToString()));
			FormTC.SelectedPayPeriod=SelectedPayPeriod;
			FormTC.ShowDialog();
			FillMain();
		}

		///<summary>This is a modified version of FormTimeCard.FillMain().  It fills one time card per employee.</summary>
		private GridOD GetGridForPrinting(Employee emp) {
			GridOD gridTimeCard=new GridOD();
			gridTimeCard.TranslationName="";
			List<ClockEvent> clockEventList=ClockEvents.Refresh(emp.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),false);
			List<TimeAdjust> timeAdjustList=TimeAdjusts.Refresh(emp.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			#region Hide time card note (we will show it at the top of the printout above the grid)
			DateTime date=_listPayPeriods[SelectedPayPeriod].DateStart.Date;
			DateTime midnightFirstDay=new DateTime(date.Year,date.Month,date.Day,0,0,0);
			_timeAdjustNote=TimeAdjusts.GetPayPeriodNote(emp.EmployeeNum,midnightFirstDay);
			if(_timeAdjustNote!=null) { //a note row exists for this pay period.
				timeAdjustList.RemoveAll(x => x.TimeAdjustNum==_timeAdjustNote.TimeAdjustNum);
			}
			#endregion
			ArrayList mergedAL=new ArrayList();
			for(int i=0;i<clockEventList.Count;i++) {
				mergedAL.Add(clockEventList[i]);
			}
			for(int i=0;i<timeAdjustList.Count;i++) {
				mergedAL.Add(timeAdjustList[i]);
			}
			IComparer myComparer=new ObjectDateComparer();
			mergedAL.Sort(myComparer);
			gridTimeCard.BeginUpdate();
			gridTimeCard.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),45);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"In"),60,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Out"),60,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Total"),50,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Adjust"),45,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),45,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PTO"),45,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"OT"),45,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PL"),45,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),50,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Week"),50,HorizontalAlignment.Right);
			gridTimeCard.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),50,HorizontalAlignment.Left);
				gridTimeCard.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),70){ IsWidthDynamic=true };
			gridTimeCard.ListGridColumns.Add(col);
			gridTimeCard.ListGridRows.Clear();
			GridRow row;
			TimeSpan[] weeklyTotals=new TimeSpan[mergedAL.Count];
			TimeSpan alteredSpan=new TimeSpan(0);//used to display altered times
			TimeSpan oneSpan=new TimeSpan(0);//used to sum one pair of clock-in/clock-out
			TimeSpan oneAdj;
			TimeSpan oneOT;
			TimeSpan daySpan=new TimeSpan(0);//used for daily totals.
			TimeSpan weekSpan=new TimeSpan(0);//used for weekly totals.
			TimeSpan ptoSpan=new TimeSpan(0);//used for PTO totals.
			TimeSpan unpaidProtectedLeaveSpan=new TimeSpan(0);//used for Unpaid Protected Leave totals.
			if(mergedAL.Count>0){
				weekSpan=ClockEvents.GetWeekTotal(emp.EmployeeNum,GetDateForRow(0,mergedAL));
			}
			TimeSpan periodSpan=new TimeSpan(0);//used to add up totals for entire page.
			TimeSpan otspan=new TimeSpan(0);//overtime for the entire period
			TimeSpan rate2span=new TimeSpan(0);//rate2 hours total
			Calendar cal=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule rule=CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
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
					//altered--------------------------------------
					//deprecated
					//status--------------------------------------
					//row.Cells.Add(clock.ClockStatus.ToString());
					//in------------------------------------------
					row.Cells.Add(clock.TimeDisplayed1.ToShortTimeString());
					if(clock.TimeEntered1!=clock.TimeDisplayed1){
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//out-----------------------------
					if(clock.TimeDisplayed2.Year<1880){
						row.Cells.Add("");//not clocked out yet
					}
					else{
						row.Cells.Add(clock.TimeDisplayed2.ToShortTimeString());
						if (clock.TimeEntered2!=clock.TimeDisplayed2)
						{
							row.Cells[row.Cells.Count-1].ColorText = Color.Red;
						}
					}
					//total-------------------------------
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
						|| GetDateForRow(i+1,mergedAL) != curDate)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(daySpan));
						daySpan=new TimeSpan(0);
					}
					else{//not the last entry for the day
						row.Cells.Add("");
					}
					//Weekly-------------------------------------
					weeklyTotals[i]=weekSpan;
					//if this is the last entry for a given week
					if(i==mergedAL.Count-1//if this is the last row 
						|| cal.GetWeekOfYear(GetDateForRow(i+1,mergedAL),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= cal.GetWeekOfYear(clock.TimeDisplayed1.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						row.Cells.Add(ClockEvents.Format(weekSpan));
						weekSpan=new TimeSpan(0);
					}
					else {
						//row.Cells.Add(ClockEvents.Format(weekSpan));
						row.Cells.Add("");
					}
					//Clinic---------------------------------------
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
					//Deprecated
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
					row.Cells.Add("");//5
					//Adjust------------------------------
					if(adjust.IsUnpaidProtectedLeave) {
						row.Cells.Add("");
					}
					else if(adjust.PtoDefNum==0) {
						daySpan+=adjust.RegHours;//might be negative
						weekSpan+=adjust.RegHours;
						periodSpan+=adjust.RegHours;
						row.Cells.Add(ClockEvents.Format(adjust.RegHours));//6
					} 
					else {
						ptoSpan+=adjust.PtoHours;
						row.Cells.Add("");//6
					}
					//Rate2-------------------------------
					row.Cells.Add("");//7
					//PTO------------------------------
					row.Cells.Add(ClockEvents.Format(adjust.PtoHours));//8
					//Overtime------------------------------
					otspan+=adjust.OTimeHours;
					row.Cells.Add(ClockEvents.Format(adjust.OTimeHours));//9
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
						|| GetDateForRow(i+1,mergedAL) != curDate)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(daySpan));//
						daySpan=new TimeSpan(0);
					}
					else{
						row.Cells.Add("");
					}
					//Weekly-------------------------------------
					weeklyTotals[i]=weekSpan;
					//if this is the last entry for a given week
					if(i==mergedAL.Count-1//if this is the last row 
						|| cal.GetWeekOfYear(GetDateForRow(i+1,mergedAL),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
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
					//Clinic---------------------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(adjust.ClinicNum));
					}
					//Note-----------------------------------------
					row.Cells.Add(adjust.Note);
					row.Cells[row.Cells.Count-1].ColorText=Color.Red;
				}
				gridTimeCard.ListGridRows.Add(row);
			}
			gridTimeCard.EndUpdate();
			totalTime=periodSpan.ToStringHmm();
			overTime=otspan.ToStringHmm();
			rate2Time=rate2span.ToStringHmm();
			ptoTime=ptoSpan.ToStringHmm();
			_unpaidProtectedLeaveTime=unpaidProtectedLeaveSpan.ToStringHmm();
			totalTime2=periodSpan.TotalHours.ToString("n");
			overTime2=otspan.TotalHours.ToString("n");
			rate2Time2=rate2span.TotalHours.ToString("n");
			ptoTime2=ptoSpan.TotalHours.ToString("n");
			_unpaidProtectedLeaveTime2=unpaidProtectedLeaveSpan.TotalHours.ToString("n");
			return gridTimeCard;
		}

		private DateTime GetDateForRow(int i,ArrayList mergedAL){
			if(mergedAL[i].GetType()==typeof(ClockEvent)){
				return ((ClockEvent)mergedAL[i]).TimeDisplayed1.Date;
			}
			else if(mergedAL[i].GetType()==typeof(TimeAdjust)){
				return ((TimeAdjust)mergedAL[i]).TimeEntry.Date;
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
			PrinterL.TryPreview(pd2_PrintPage,
				Lan.g(this,Lans.g(this,"Employee time cards printed")),
				totalPages:gridMain.ListGridRows.Count
			);
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			PrintEveryTimeCard(sender,e);
		}

		private void PrintEveryTimeCard(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			//A preview of every single emp on their own page will show up. User will print from there.
			Employee employeeCur=Employees.GetEmp(PIn.Long(MainTable.Rows[_pagesPrinted]["EmployeeNum"].ToString()));
			PrintTimeCard(sender,e,employeeCur,gridMain.ListGridRows.Count);	
		}

		///<summary>Print timecards for selected employees only.</summary>
		private void butPrintSelected_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"No employees selected, please select one or more employees or click 'Print All' to print all employees.");
				return;
			}
			_pagesPrinted=0;
			PrinterL.TryPreview(pd2_PrintPageSelective,
				Lan.g(this,"Employee time cards printed"),
				totalPages:gridMain.SelectedIndices.Length
			);
		}

		///<summary>Similar to pd2_PrintPage except it iterates through selected indices instead of all indices.</summary>
		private void pd2_PrintPageSelective(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			PrintEmployeeTimeCard(sender,e);
		}

		private void PrintEmployeeTimeCard(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
			//A preview of every single emp on their own page will show up. User will print from there.
			Employee employeeCur=Employees.GetEmp(PIn.Long(MainTable.Rows[gridMain.SelectedIndices[_pagesPrinted]]["EmployeeNum"].ToString()));
			PrintTimeCard(sender,e,employeeCur,gridMain.SelectedIndices.Length);
		}

		private void PrintTimeCard(object sender, System.Drawing.Printing.PrintPageEventArgs e, Employee employeeCur,int pagesToPrint) {
			Graphics g=e.Graphics;
			GridOD timeCardGrid=GetGridForPrinting(employeeCur);
			int linesPrinted=0;
			//Create a timecardgrid for this employee?
			float yPos=75;
			float xPos=55;
			string str;
			Font font=new Font(FontFamily.GenericSansSerif,8);
			Font fontTitle=new Font(FontFamily.GenericSansSerif,11,FontStyle.Bold);
			Font fontHeader=new Font(FontFamily.GenericSansSerif,8,FontStyle.Bold);
			SolidBrush brush=new SolidBrush(Color.Black);
			Pen pen=new Pen(Color.Black);
			//Title
			str=employeeCur.FName+" "+employeeCur.LName;
			str+="\r\n"+Lan.g(this,"Note")+": "+_timeAdjustNote?.Note.ToString()??"";
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
			colW[9]=45;//PL: Column start to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
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
			ColCaption[2]=Lan.g(this,"In");
			ColCaption[3]=Lan.g(this,"Out");
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
			while(yPos < e.PageBounds.Height-75-50-32-16 && linesPrinted < timeCardGrid.ListGridRows.Count) {
				for(int i=0;i<colPos.Length-1;i++) {
					if(timeCardGrid.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Empty || timeCardGrid.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Black) {
						e.Graphics.DrawString(timeCardGrid.ListGridRows[linesPrinted].Cells[i].Text,font,brush
							,new RectangleF(colPos[i]+2,yPos,colPos[i+1]-colPos[i]-5,font.GetHeight(e.Graphics)));
					}
					else { //The only other color currently supported is red.
						e.Graphics.DrawString(timeCardGrid.ListGridRows[linesPrinted].Cells[i].Text,font,Brushes.Red
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
			g.DrawString(Lan.g(this,"Regular Time")+": "+totalTime+" ("+totalTime2+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Overtime")+": "+overTime+" ("+overTime2+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 2 Time")+": "+rate2Time+" ("+rate2Time2+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"PTO Time")+": "+ptoTime+" ("+ptoTime2+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Protected Leave")+": "+_unpaidProtectedLeaveTime+" ("+_unpaidProtectedLeaveTime2+")",fontHeader,brush,xPos,yPos);
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
			if(SelectedPayPeriod==0){
				return;
			}
			SelectedPayPeriod--;
			FillPayPeriod();
			FillMain();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(SelectedPayPeriod==_listPayPeriods.Count-1) {
				return;
			}
			SelectedPayPeriod++;
			FillPayPeriod();
			FillMain();
		}

		private void butDaily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.TimecardsEditAll)) {
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
					TimeCardRules.CalculateDailyOvertime(Employees.GetEmp(PIn.Long(MainTable.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()))
						,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					aggregateErrors+=ex.Message+"\r\n";
				}
			}
			Cursor=Cursors.Default;
			//Cache selected indicies, fill grid, reselect indicies.
			List<int> listSelectedIndexCach=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listSelectedIndexCach.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listSelectedIndexCach.Count;i++) {
				gridMain.SetSelected(listSelectedIndexCach[i],true);
			}
			if(aggregateErrors=="") {
				MsgBox.Show(this,"Done.");
			}
			else {
				MessageBox.Show(this,Lan.g(this,"Time cards were not calculated for some Employees for the following reasons")+":\r\n"+aggregateErrors);
			}
		}

		private void butWeekly_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.TimecardsEditAll)) {
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
					TimeCardRules.CalculateWeeklyOvertime(Employees.GetEmp(PIn.Long(MainTable.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()))
						,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					aggregateErrors+=ex.Message+"\r\n";
				}
			}
			Cursor=Cursors.Default;
			//Cache selected indices, fill grid, reselect indices.
			List<int> listSelectedIndexCach=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				listSelectedIndexCach.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listSelectedIndexCach.Count;i++) {
				gridMain.SetSelected(listSelectedIndexCach[i],true);
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
					TimeCardRules.ClearManual(PIn.Long(MainTable.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()),PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
			//Cach selected indicies, fill grid, reselect indicies.
			List<int> listSelectedIndexCach=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listSelectedIndexCach.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listSelectedIndexCach.Count;i++) {
				gridMain.SetSelected(listSelectedIndexCach[i],true);
			}
		}

		private void butClearAuto_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This cannot be undone, but you can run the Calc buttons again later.  Would you like to continue?")) {
				return;
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				try {
					TimeCardRules.ClearAuto(PIn.Long(MainTable.Rows[gridMain.SelectedIndices[i]]["EmployeeNum"].ToString()),PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
			//Cach selected indicies, fill grid, reselect indicies.
			List<int> listSelectedIndexCach=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listSelectedIndexCach.Add(gridMain.SelectedIndices[i]);
			}
			FillMain();
			for(int i=0;i<listSelectedIndexCach.Count;i++) {
				gridMain.SetSelected(listSelectedIndexCach[i],true);
			}
		}

		///<summary>Print exactly what is showing in gridMain. (Including rows that do not fit in the UI.)</summary>
		private void butPrintGrid_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			HeadingPrinted=false;
			_printGrid=FillMain(true);
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Printed employee time card grid."));
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			int y=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			int headingPrintH=0;
			if(!HeadingPrinted) {
				text=Lan.g(this,"Pay Period")+": "+textDateStart.Text+" - "+textDateStop.Text+"\r\n"
					+Lan.g(this,"Paycheck Date")+": "+textDatePaycheck.Text;
				if(PrefC.HasClinicsEnabled) {
					text+="\r\n"+Lan.g(this,"Clinic")+": ";
					if(Security.CurUser.ClinicIsRestricted) {
						text+=Clinics.GetAbbr(comboClinic.SelectedClinicNum);
					}
					else {//All and Headquarters are the first two available options.
						if(comboClinic.IsAllSelected) {
							text+=Lan.g(this,"All");
						}
						else if(comboClinic.IsUnassignedSelected) {
							text+=Lan.g(this,"Headquarters");
						}
						else {//A specific clinic was selected.
							text+=Clinics.GetAbbr(comboClinic.SelectedClinicNum);
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
				HeadingPrinted=true;
				headingPrintH=y;
			}
			#endregion
			y=_printGrid.PrintPage(g,_pagesPrinted,bounds,headingPrintH);
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
			using FolderBrowserDialog fbd = new FolderBrowserDialog();
			if(!ODBuild.IsWeb()) {
				if(fbd.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			StringBuilder strb = new StringBuilder();
			string headers="";
			for(int i=0;i<MainTable.Columns.Count;i++) {
				headers+=(i>0?"\t":"")+MainTable.Columns[i].ColumnName;
			}
			strb.AppendLine(headers);
			for(int i=0;i<MainTable.Rows.Count;i++) {
				string row="";
				for(int c=0;c<MainTable.Columns.Count;c++) {
					if(c>0) {
						row+="\t";
					}
					switch(MainTable.Columns[c].ColumnName) {
						case "PayrollID":
						case "EmployeeNum":
						case "firstName":
						case "lastName":
						case "Note":
							row+=MainTable.Rows[i][c].ToString().Replace("\t","").Replace("\r\n",";  ");
							break;
						case "totalHours":
						case "rate1Hours":
						case "rate1OTHours":
						case "rate2Hours":
						case "rate2OTHours":
						case "rate3Hours":
						case "protectedLeaveHours":
							//Time must me formatted differently.
							if(PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon)) {
								row+=PIn.Time(MainTable.Rows[i][c].ToString()).TotalHours.ToString("n");
							}
							else if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {//Colon format with seconds
								row+=PIn.Time(MainTable.Rows[i][c].ToString()).ToStringHmmss();
							}
							else {//Colon format without seconds
								row+=PIn.Time(MainTable.Rows[i][c].ToString()).ToStringHmm();
							}
							break;
						default:
							//should never happen.
							throw new Exception("Unexpected column found in payroll table : "+MainTable.Columns[c].ColumnName);
					}//end switch
				}//end columns
				strb.AppendLine(row);
			}
			string fileName="ODPayroll"+DateTime.Now.ToString("yyyyMMdd_hhmmss")+".TXT";
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(fileName,strb.ToString());
				return;
			}
			try {
				System.IO.File.WriteAllText(fbd.SelectedPath+"\\"+fileName,strb.ToString());
				MessageBox.Show(this,Lan.g(this,"File created")+" : "+fbd.SelectedPath+"\\"+fileName);
			}
			catch(Exception ex) {
				MessageBox.Show(this,"File not created:\r\n"+ex.Message);
			}
		}

		///<summary>Validates format and values and provides aggregate error and warning messages. Will save malformed files anyways.</summary>
		private void butExportADP_Click(object sender,EventArgs e) {
			bool hasVisiblePtoDef=false;
			//Check to confirm if any PTO definitions are hidden, if not show these defs on the timecard
			List<Def> listPtoTypes=Defs.GetDefsForCategory(DefCat.TimeCardAdjTypes);
			foreach(Def def in listPtoTypes) {
				if(!def.IsHidden) { 
					hasVisiblePtoDef=true;
					break;
				}
			}
			StringBuilder strb = new StringBuilder();
			string errors="";
			string warnings="";
			string errorIndent="  ";
			strb.Append("Co Code,Batch ID,File #"+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?",Employee Name":"")+",Rate Code,Reg Hours,O/T Hours");
			if(hasVisiblePtoDef) { 
				strb.Append(",Hours 3 Code,Hours 3 Amount");
			} 
			strb.AppendLine();
			string coCode=PrefC.GetString(PrefName.ADPCompanyCode);
			string batchID=DateStop.ToString("yyyyMMdd");//max 8 characters
			if(coCode.Length<2 || coCode.Length>3){
				errors+=errorIndent+"Company code must be two to three alpha numeric characters long.  Go to Setup>Manage>TimeCards to edit.\r\n";
			}
			coCode=coCode.PadRight(3,'_');//for two digit company codes.
			for(int i=0;i<MainTable.Rows.Count;i++) {
				string errorsForEmployee="";
				string warningsForEmployee="";
				string fileNum="";
				string employeeName="";
				fileNum=MainTable.Rows[i]["PayrollID"].ToString();
				try {
					if(PIn.Int(fileNum)<51 || PIn.Int(fileNum)>999999) {
						errorsForEmployee+=errorIndent+"Payroll ID not between 51 and 999999.\r\n";
					}
				}
				catch (Exception ex){
					ex.DoNothing();
					//same error message as above.
					errorsForEmployee+=errorIndent+"Payroll ID not between 51 and 999999.\r\n";
				}
				if(fileNum.Length>6) {
					errorsForEmployee+=errorIndent+"Payroll ID must be less than 6 digits long.\r\n";
				}
				else {//pad payrollIDs that are too short. No effect if payroll ID is 6 digits long.
					fileNum=fileNum.PadLeft(6,'0');
				}
				try {
					employeeName=Employees.GetNameFL(Employees.GetEmp(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())));
				}
				catch {
					employeeName="Error";
				}
				string r1hours	=(PIn.TSpan(MainTable.Rows[i]["rate1Hours"  ].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r1hours=="0.00"){//Was changing Exactly 80.00 hours with 8 hours.
					r1hours="";
				}
				string r1OThours=(PIn.TSpan(MainTable.Rows[i]["rate1OTHours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r1OThours=="0.00") {
					r1OThours="";
				}
				string r2hours	=(PIn.TSpan(MainTable.Rows[i]["rate2Hours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r2hours=="0.00") {
					r2hours="";
				}
				string r2OThours=(PIn.TSpan(MainTable.Rows[i]["rate2OTHours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r2OThours=="0.00") {
					r2OThours="";
				}
				string r3hours=(PIn.TSpan(MainTable.Rows[i]["rate3Hours"].ToString())).TotalHours.ToString("F2");//adp allows 2 digit precision
				if(r3hours=="0.00") {
					r3hours="";
				}
				string textToAdd="";
				if(r1hours!="" || r1OThours!="" || r3hours!="") {//no entry should be made unless there are actually hours for this employee.
					textToAdd+=coCode+","+batchID+","+fileNum+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?","+employeeName:"")+",,"+r1hours+","+r1OThours;
					if(hasVisiblePtoDef)	{
						if(r3hours=="") {
							textToAdd+=",,";
						}
						else {
							textToAdd+=","+"PTO"+","+r3hours;
						}
					} 
					textToAdd+="\r\n";
				}
				if(r2hours!="" || r2OThours!="") {//no entry should be made unless there are actually hours for this employee.
					textToAdd+=coCode+","+batchID+","+fileNum+(PrefC.GetBool(PrefName.TimeCardADPExportIncludesName)?","+employeeName:"")+",2,"+r2hours+","+r2OThours+"\r\n";
				}
				if(textToAdd=="") {
					warningsForEmployee+=errorIndent+"No clocked hours.\r\n";// for "+Employees.GetNameFL(Employees.GetEmp(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())))+"\r\n";
				}
				else {
					strb.Append(textToAdd);
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
					errors+=Employees.GetNameFL(Employees.GetEmp(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())))+":\r\n"+errorsForEmployee+"\r\n";
				}
				if(warningsForEmployee!="") {
					warnings+=Employees.GetNameFL(Employees.GetEmp(PIn.Long(MainTable.Rows[i]["EmployeeNum"].ToString())))+":\r\n"+warningsForEmployee+"\r\n";
				}
			}
			using FolderBrowserDialog fbd = new FolderBrowserDialog();
			if(!ODBuild.IsWeb()) {
				if(fbd.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			string fileSuffix=GenerateFileSuffix(fbd.SelectedPath,"\\EPI"+coCode);
			try {
				if(ODBuild.IsWeb()) {
					string fileName="EPI"+coCode+fileSuffix+".CSV";
					ThinfinityUtils.ExportForDownload(fileName,strb.ToString());
				}
				else {
					System.IO.File.WriteAllText(fbd.SelectedPath+"\\EPI"+coCode+fileSuffix+".CSV",strb.ToString());
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
				MessageBox.Show(this,Lan.g(this,"File created")+" : "+fbd.SelectedPath+"\\EPI"+coCode+fileSuffix+".CSV");
			}
			catch(Exception ex) {
				MessageBox.Show(this,"File not created:\r\n"+ex.Message);
			}
		}

		///<summary>Validates format and values for ADP Run csv file, providing error and warning messages. Will save malformed files anyways. Reports must contain all 11 columns and only one Pay Frequency. </summary>
		private void butExportADPRun_Click(object sender,EventArgs e) {
			StringBuilder strb=new StringBuilder();
			strb.AppendLine("##GENERIC## V1.0"); //Specific header row required for all ADP Run reports
			strb.AppendLine("IID,Pay Frequency,Pay Period Start Date,Pay Period End Date,Employee Time Clock ID,Earnings Code,Pay Hours,Dollars,Separate Check,Worked In Dept,Rate Code");
			string ADPRunIID=PrefC.GetString(PrefName.ADPRunIID);         // 1 - IID == ADPRunIID
			PayPeriodInterval payPeriodInterval=PrefC.GetEnum<PayPeriodInterval>(PrefName.PayPeriodIntervalSetting);
			string payFrequency=payPeriodInterval.ToString();					    // 2 - Pay Frequency: OD only supports Weekly, Biweekly, or Monthly pay period intervals
			string startDate=textDateStart.Text;											 	  // 3 - Pay Period Start
			string endDate=textDateStop.Text;                             // 4 - Pay Period End
			string dollars="";																					  // 8 - Dollars, not required
			int separateCheck=0;																			    // 9 - Separate Check, ADP defaults 0
			string workedDepartment="";																    //10 - Worked Department, not required		
			//Loop through employees to fill out remaining columns
			for(int i=0;i<MainTable.Rows.Count;i++) {
				DataRow row=MainTable.Rows[i];
				string textToAdd=ADPRunIID+","+payFrequency.First()+","+startDate+","+endDate+",";
				textToAdd+=row["EmployeeNum"].ToString()+","; // 5 - Employee Time Clock IID == OD EmployeeNum														
				#region Columns 6, 7, 11 - Earnings Code, Pay Hours, Rate Code
				//The ADP Run format requires each type of pay (regular, overtime, etc) and rate of pay (BASE, RATE_2, etc) to be on its own row. 
				//Must check each MainTable column to create appropriate rows for each employee. Only create a row if it contains hours.
				string r1hours=(PIn.TSpan(row["rate1Hours"].ToString())).TotalHours.ToString("F2");	//adp run requires 2 digit precision
				if(r1hours!="0.00") {
					strb.AppendLine(textToAdd+"REG,"+r1hours+","+dollars+","+separateCheck+","+workedDepartment+",BASE");
				}
				string r1OThours=(PIn.TSpan(row["rate1OTHours"].ToString())).TotalHours.ToString("F2");
				if(r1OThours!="0.00") {
					strb.AppendLine(textToAdd+"OVT,"+r1OThours+","+dollars+","+separateCheck+","+workedDepartment+",BASE");
				}
				string r2hours=(PIn.TSpan(row["rate2Hours"].ToString())).TotalHours.ToString("F2");
				if(r2hours!="0.00") {
					strb.AppendLine(textToAdd+"REG,"+r2hours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_2");
				}
				string r2OThours=(PIn.TSpan(row["rate2OTHours"].ToString())).TotalHours.ToString("F2");
				if(r2OThours!="0.00") {
					strb.AppendLine(textToAdd+"OVT,"+r2OThours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_2");
				}
				string r3hours=(PIn.TSpan(row["rate3Hours"].ToString())).TotalHours.ToString("F2");
				if(r3hours!="0.00") {
					strb.AppendLine(textToAdd+"REG,"+r3hours+","+dollars+","+separateCheck+","+workedDepartment+",RATE_3");
				}
				#endregion
			}//end file contents
			using FolderBrowserDialog fbd=new FolderBrowserDialog();
			if(!ODBuild.IsWeb()) {
				if(fbd.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			//ADP recommends a filename like "Biweekly-01012021-01152021.csv"
			string fileName=payFrequency+"-"+startDate.Replace("/","")+"-"+endDate.Replace("/","");
			fileName+="-"+GenerateFileSuffix(fbd.SelectedPath,"\\"+fileName+"-");
			//Write to file and inform user of potential errors
			string errors="";
			if(ADPRunIID.IsNullOrEmpty()) {
				errors+="  An IID is required for ADP Run.  Go to Setup>Manage>TimeCards to edit ADPRunIID.\r\n";
			}
			if(!PayFrequencyMatchesDateRange(payPeriodInterval)) {
				errors+="  The pay frequency '"+payPeriodInterval+"' does not match the pay period date range.\r\n";
			}
			try {
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.ExportForDownload(fileName+".CSV",strb.ToString());
				}
				else {
					System.IO.File.WriteAllText(fbd.SelectedPath+"\\"+fileName+".CSV",strb.ToString());
				}
				if(!errors.IsNullOrEmpty()) {
					MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(
						"The following errors will prevent ADP from properly processing this export:\r\n"+errors);
					msgBox.Show(this);
				}
				MessageBox.Show(this,Lan.g(this,"File created")+" : "+fbd.SelectedPath+"\\"+fileName+".CSV");
			}
			catch(Exception ex) {
				MessageBox.Show(this,"File not created:\r\n"+ex.Message);
			}
		}

		///<summary>Creates a suffix for a file if the passed fileName already exists in the directory. Returns a two character alphanumeric string if a file already exists with that name, or an empty string if not. Will return an error message if all 1296 two character alphanumeric strings are already used as suffixes. </summary>
		private string GenerateFileSuffix(string filePath,string fileName) {
			string fileSuffix="";
			//generate suffix from i
			for(int i=0;i<=1297;i++) {//1296=36*36 to represent all acceptable suffixes for file name consisting of two alphanumeric digits; +1 to catch error. (A-Z, 0-9)
				fileSuffix="";
				if(ODBuild.IsWeb()) {
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
			DateTime startDate=PIn.Date(textDateStart.Text);
			DateTime endDate=PIn.Date(textDateStop.Text);
			if(  (payPeriodInterval==PayPeriodInterval.Weekly && startDate.AddDays(6)==endDate)
				|| (payPeriodInterval==PayPeriodInterval.BiWeekly && startDate.AddDays(13)==endDate)
				|| (payPeriodInterval==PayPeriodInterval.Monthly && startDate.AddMonths(1).AddDays(-1)==endDate))
			{
				return true;
			}
			return false;
		}

		private void butTimeCardBenefits_Click(object sender,EventArgs e) {
			FormTimeCardBenefitRp FormTCB = new FormTimeCardBenefitRp();
			FormTCB.Show();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormTimeCardSetup FormTCS=new FormTimeCardSetup();
			FormTCS.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Time Card Setup");
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}