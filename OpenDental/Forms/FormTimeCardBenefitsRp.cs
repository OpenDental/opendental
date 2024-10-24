using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using System.Text;
using CodeBase;
using System.IO;
using OpenDental.Thinfinity;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormTimeCardBenefitRp:FormODBase {
		///<summary>When set by parent, the report will be ran for a single employee. When not set, the report will be ran for all employees.</summary>
		public long EmployeeNum;
		private DateTime _dateNow;
		private DateTime _dateMonthT0;
		private DateTime _dateMonthT1;
		private DateTime _dateMonthT2;
		private Color _colorLightRed;

		public FormTimeCardBenefitRp() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormTimeCardBenefitRp_Load(object sender,EventArgs e) {
			_dateNow = MiscData.GetNowDateTime();
			this.Text+=" - "+_dateNow.ToLongDateString();
			_dateMonthT0 = new DateTime(_dateNow.Year,_dateNow.Month,1); //.AddMonths(-0);
			_dateMonthT1 = _dateMonthT0.AddMonths(-1);
			_dateMonthT2 = _dateMonthT0.AddMonths(-2);
			_colorLightRed=Color.FromArgb(254,235,233); //light red
			FillGridMain();
		}

		private void FillGridMain() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"LName"),100));
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"FName"),100));
			gridMain.Columns.Add(new GridColumn(_dateMonthT2.ToString("MMMM yyyy"),100,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridMain.Columns.Add(new GridColumn(_dateMonthT1.ToString("MMMM yyyy"),100,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			if(!checkIgnore.Checked) {
				gridMain.Columns.Add(new GridColumn(_dateMonthT0.ToString("MMMM yyyy"),100,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g(this,"Letter"),100));
			gridMain.ListGridRows.Clear();
			List<Employee> listEmployees=new List<Employee>();
			List<ClockEvent> listClockEvents=new List<ClockEvent>();
			if(EmployeeNum!=0) {//single employee
				listEmployees.Add(Employees.GetEmp(EmployeeNum));
				listClockEvents=ClockEvents.GetSimpleList(EmployeeNum,_dateMonthT2,_dateMonthT2.AddMonths(3));//get all three months of clock events
			}
			else {//all employees
				listEmployees=Employees.GetDeepCopy(true).OrderBy(x => x.LName).ThenBy(x => x.FName).ToList();
				listClockEvents=ClockEvents.GetAllForPeriod(_dateMonthT2,_dateMonthT2.AddMonths(3));//get all three months of clock events				
			}
			listClockEvents.RemoveAll(x => x.ClockStatus==TimeClockStatus.Break);//remove breaks, they have already been acounted for on the clock events.
			listClockEvents.RemoveAll(x => x.TimeDisplayed2<=x.TimeDisplayed1);//Remove all mal-formed entries with stop time before start time. (also if user has not clocked out.)
			listClockEvents.RemoveAll(x => x.TimeDisplayed1.Date!=x.TimeDisplayed2.Date);//No one works over midnight at ODHQ. If they do, they know to split clock events @ midnight
			List<TimeAdjust> listTimeAdjustAll = TimeAdjusts.GetAllForPeriod(_dateMonthT2,_dateMonthT2.AddMonths(3));
			for (int i=0;i<listEmployees.Count;i++) {
				//Construct each row, then filter out if neccesary.
				GridRow row = new GridRow();
				//Name
				row.Cells.Add(listEmployees[i].LName);
				row.Cells.Add(listEmployees[i].FName);
				//Month T-2 (current month -2 months)
				TimeSpan timeSpan2 = TimeSpan.FromTicks(listClockEvents
					.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum
						&& x.TimeDisplayed1.Year==_dateMonthT2.Year
						&& x.TimeDisplayed1.Month==_dateMonthT2.Month)
					.Select(x => (x.TimeDisplayed2-x.TimeDisplayed1)+(x.AdjustIsOverridden ? x.Adjust : x.AdjustAuto))
					.Sum(x=>x.Ticks));
				timeSpan2=timeSpan2.Add(TimeSpan.FromTicks(listTimeAdjustAll.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum
						&& x.TimeEntry.Year==_dateMonthT2.Year
						&& x.TimeEntry.Month==_dateMonthT2.Month)
						.Sum(x => x.RegHours.Ticks+x.PtoHours.Ticks+x.OTimeHours.Ticks)));
				GridCell gridCell = new GridCell(string.Format("{0:0.00}",Math.Round(timeSpan2.TotalHours,2,MidpointRounding.AwayFromZero)));
				gridCell.ColorBackG = Color.Empty;
				if (timeSpan2.TotalHours<125) {
					gridCell.ColorBackG = _colorLightRed;
				}
				row.Cells.Add(gridCell);
				//Month T-1
				TimeSpan timeSpan1 = TimeSpan.FromTicks(listClockEvents
					.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum
						&& x.TimeDisplayed1.Year==_dateMonthT1.Year
						&& x.TimeDisplayed1.Month==_dateMonthT1.Month)
					.Select(x => (x.TimeDisplayed2-x.TimeDisplayed1)+(x.AdjustIsOverridden ? x.Adjust : x.AdjustAuto))
					.Sum(x => x.Ticks));
				timeSpan1=timeSpan1.Add(TimeSpan.FromTicks(listTimeAdjustAll.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum
						&& x.TimeEntry.Year==_dateMonthT1.Year
						&& x.TimeEntry.Month==_dateMonthT1.Month)
						.Sum(x => x.RegHours.Ticks+x.PtoHours.Ticks+x.OTimeHours.Ticks)));
				gridCell = new GridCell(string.Format("{0:0.00}",Math.Round(timeSpan1.TotalHours,2,MidpointRounding.AwayFromZero)));
				gridCell.ColorBackG = Color.Empty;
				if (timeSpan1.TotalHours<125) {
					gridCell.ColorBackG = _colorLightRed;
				}
				row.Cells.Add(gridCell);
				//Month T-0
				TimeSpan timeSpan0 = TimeSpan.FromTicks(listClockEvents
					.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum
						&& x.TimeDisplayed1.Year==_dateMonthT0.Year
						&& x.TimeDisplayed1.Month==_dateMonthT0.Month)
					.Select(x => (x.TimeDisplayed2-x.TimeDisplayed1)+(x.AdjustIsOverridden ? x.Adjust : x.AdjustAuto))
					.Sum(x => x.Ticks));
				timeSpan0=timeSpan0.Add(TimeSpan.FromTicks(listTimeAdjustAll.FindAll(x => x.EmployeeNum==listEmployees[i].EmployeeNum
						&& x.TimeEntry.Year==_dateMonthT0.Year
						&& x.TimeEntry.Month==_dateMonthT0.Month)
						.Sum(x => x.RegHours.Ticks+x.PtoHours.Ticks+x.OTimeHours.Ticks)));
				if(!checkIgnore.Checked) {
					gridCell = new GridCell(string.Format("{0:0.00}",Math.Round(timeSpan0.TotalHours,2,MidpointRounding.AwayFromZero)));
					gridCell.ColorBackG = Color.Empty;
					if (timeSpan0.TotalHours<125) {
						gridCell.ColorBackG = _colorLightRed;
					}
					row.Cells.Add(gridCell);
				}
				//filter out rows that should not be displaying. Rows should not display only when the most recent month was less than 125 hours, regardless of status of previous months
				if(!checkShowAll.Checked) {
					//Show all is not checked, therefore we must filter out rows with more than 125 hours on the most recent pay period.
					if(!checkIgnore.Checked && timeSpan0.TotalHours>125) {
						//Not ignoring "current month" so check T0 for >125 hours
						continue;
					}
					else if(checkIgnore.Checked && timeSpan1.TotalHours>125) {
						//Ignore current month (because it is probably only partially complete), check the previous months for >125 hours. 
						continue;
					}
				}
				string letterNumber = "";
				if((checkIgnore.Checked?timeSpan2:timeSpan0).TotalHours<125 && timeSpan1.TotalHours<125) {
					//the last two visible month were less than 125 hours. AKA "Send the Second letter"
					letterNumber="Second";
				}
				else if((checkIgnore.Checked?timeSpan1:timeSpan0).TotalHours<125) {
					//the last visible month was less than 125 hours. AKA "Send the First letter"
					letterNumber="First";
				}
				row.Cells.Add(letterNumber);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void checkShowAll_CheckedChanged(object sender,EventArgs e) {
			FillGridMain();
		}

		private void checkIgnore_CheckedChanged(object sender,EventArgs e) {
			FillGridMain();
		}

		#region Print Grid
		private int _pagesPrinted;
		private bool _isHeadingPrinted;

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Payroll benefits report printed."));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds = e.MarginBounds;
			Graphics g = e.Graphics;
			string text;
			using Font fontHeading = new Font("Arial",13,FontStyle.Bold);
			using Font fontSubHeading = new Font("Arial",10,FontStyle.Bold);
			int yPos = rectangleBounds.Top;
			int center = rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Payroll Benefits Report");
				StringFormat stringFormat = StringFormat.GenericDefault;
				stringFormat.Alignment=StringAlignment.Center;
				g.DrawString(text,fontHeading,Brushes.Black,center,yPos,stringFormat);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				g.DrawString(_dateNow.ToString(),fontSubHeading,Brushes.Black,center,yPos,stringFormat);
				yPos+=20;
				_isHeadingPrinted=true;
				stringFormat.Dispose();
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,rectangleBounds,yPos);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}
		#endregion Print Grid

		private void butExportGrid_Click(object sender,EventArgs e) {
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.AppendLine(string.Join(",",gridMain.Columns.Select(x => x.Heading)));
			gridMain.ListGridRows
				.ForEach(row => stringBuilder.AppendLine(string.Join(",",row.Cells.Select(cell => cell.Text.Replace(',','-').Replace('\t',',')))));
			if(ODBuild.IsThinfinity()) {
				string exportFilename="BenefitsRpt_"+_dateNow.ToString("yyyyMMdd")+"_"+DateTime.Now.ToString("hhmm")+".csv";
				string dataString=stringBuilder.ToString();
				ThinfinityUtils.ExportForDownload(exportFilename,dataString);
				return;
			}
			using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			if(folderBrowserDialog.ShowDialog()!=DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath)) {
				MsgBox.Show(this,"Invalid directory.");
				return;
			}
			string filename = ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"BenefitsRpt_"+_dateNow.ToString("yyyyMMdd")+"_"+DateTime.Now.ToString("hhmm")+".csv");
			System.IO.File.WriteAllText(filename,stringBuilder.ToString());
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to open the file now?")) {
				try {
					System.Diagnostics.Process.Start(filename);
				}
				catch(Exception ex) { ex.DoNothing(); }
			}
		}

	}
}