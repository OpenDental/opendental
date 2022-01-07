using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormGraphEmployeeView : FormODBase {
		private GridRow _gridRowCopied;
		///<summary>A list of all employees.</summary>
		private List<Employee> _listEmployees;
		///<summary>A list of phone graphs for the currently selected employee.</summary>
		private List<PhoneGraph> _listPhoneGraphsEmpCur;
		///<summary>A list of schedules for the currently selected employee.</summary>
		private List<Schedule> _listSchedulesEmpCur;
		///<summary>A list of schedules for all providers within the date range from the odDatePickers.</summary>
		private List<Schedule> _listSchedulesProvs;
		///<summary>ProvNum for the currently selected employee</summary>
		private long _provNum;
		private int _pagesPrinted;
		private bool _headingPrinted;
		private int _headingPrintH;
		
		public FormGraphEmployeeView() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormGraphEmployeeView_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Schedules,true)){
				butCopy.Enabled=false;
				butCopy.Visible=false;
				butPaste.Enabled=false;
				butPaste.Visible=false;
				labelCopyPaste.Visible=false;
			}
			_listEmployees=Employees.GetDeepCopy(true);
			_listEmployees.ForEach(x => listBoxEmployees.Items.Add(x.FName,x));
			listBoxEmployees.SetSelectedKey<Employee>(Security.CurUser.EmployeeNum,x => x.EmployeeNum);
			odDatePickerFrom.SetDateTime(DateTime.Today);
			odDatePickerTo.SetDateTime(DateTime.Today.AddMonths(3));
			FillGrid();
		}

		private void ButClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void ButCopy_Click(object sender,EventArgs e) {
			if(gridMain.SelectedGridRows.Count!=1) {
				MsgBox.Show(this,"Please select one row to copy.");
				return;
			}
			_gridRowCopied=gridMain.SelectedGridRows[0];
		}

		private void ButPaste_Click(object sender,EventArgs e) {
			if(_gridRowCopied==null) {
				MsgBox.Show(this,"Please copy a row first.");
				return;
			}
			DateTime dateTimeEntry=DateTime.Parse(_gridRowCopied.Cells[0].Text);
			//There should only be one phone graph per dateEntry
			PhoneGraph phoneGraphCopied=_listPhoneGraphsEmpCur.Find(x => x.DateEntry==dateTimeEntry);
			List<Schedule> listSchedulesCopied=_listSchedulesProvs.FindAll(x => x.ProvNum==_provNum && x.SchedDate==dateTimeEntry);
			long empNumCur=listBoxEmployees.GetSelected<Employee>().EmployeeNum;
			//For now, only update the phone graph overrides and provider schedules
			for(int i=0; i<gridMain.SelectedGridRows.Count;i++) {
				DateTime dateTimeSelected=DateTime.Parse(gridMain.SelectedGridRows[i].Cells[0].Text);
				//Update phone graphs
				if(phoneGraphCopied!=null) {
					PhoneGraph phoneGraphSelected=_listPhoneGraphsEmpCur.Find(x => x.DateEntry==dateTimeSelected);
					if(phoneGraphSelected==null) {
						phoneGraphSelected=phoneGraphCopied.Copy();//new PhoneGraph();
						phoneGraphSelected.IsNew=true;
						phoneGraphSelected.EmployeeNum=empNumCur;
						phoneGraphSelected.DateEntry=dateTimeSelected;
					}
					else {
						long phoneGraphNum=phoneGraphSelected.PhoneGraphNum;
						phoneGraphSelected=phoneGraphCopied.Copy();
						phoneGraphSelected.PhoneGraphNum=phoneGraphNum;
						phoneGraphSelected.DateEntry=dateTimeSelected;
					}
					phoneGraphSelected.Note=Security.CurUser.UserName+" "+DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();
					PhoneGraphs.InsertOrUpdate(phoneGraphSelected);
				}
				//Update provider schedules (start/stop times, etc)
				List<Schedule> listSchedulesProvToUpdate=_listSchedulesProvs.FindAll(x => x.SchedDate==dateTimeSelected && x.ProvNum==_provNum);
				for(int p=0; p<listSchedulesProvToUpdate.Count; p++) {
					Schedules.Delete(listSchedulesProvToUpdate[p]);//No signal
				}
				for(int p=0; p<listSchedulesCopied.Count; p++) {
					Schedule schedule=new Schedule();
					schedule.ProvNum=_provNum;
					schedule.SchedType=listSchedulesCopied[p].SchedType;
					schedule.SchedDate=dateTimeSelected;
					schedule.Status=listSchedulesCopied[p].Status;
					schedule.StartTime=listSchedulesCopied[p].StartTime;
					schedule.StopTime=listSchedulesCopied[p].StopTime;
					Schedules.Insert(schedule,validate:false,hasSignal:false);
				}
			}
			FillGrid();
		}

		private void ButPrint_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count<1) {
				MsgBox.Show(this,"Employee schedule list is empty.");
				return;
			}
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(PrintPage,Lan.g(this,"Employee schedules printed."),margins:new Margins(25,25,40,40),printoutOrientation:PrintoutOrientation.Landscape);
		}

		private void GridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.Schedules,true)) {
				return;
			}
			GridRow row=gridMain.SelectedGridRows[0];
			DateTime dateTimeCur=DateTime.Parse(row.Cells[0].Text);
			PhoneEmpDefault phoneEmpDefault=(PhoneEmpDefault)gridMain.ListGridRows[e.Row].Tag;
			PhoneGraph phoneGraph=_listPhoneGraphsEmpCur.Find(x=>x.EmployeeNum==phoneEmpDefault?.EmployeeNum && x.DateEntry==dateTimeCur);
			if(phoneGraph==null){
				phoneGraph=new PhoneGraph();
				phoneGraph.EmployeeNum=phoneEmpDefault.EmployeeNum;
				phoneGraph.DateEntry=dateTimeCur;
				phoneGraph.IsNew=true;
				phoneGraph.IsGraphed=phoneEmpDefault.IsGraphed;
			}
			using FormPhoneGraphEdit formPhoneGraphEdit=new FormPhoneGraphEdit();
			formPhoneGraphEdit.PhoneGraphCur=phoneGraph;
			formPhoneGraphEdit.ListSchedulesEmp=_listSchedulesEmpCur.FindAll(x=>x.EmployeeNum==phoneEmpDefault.EmployeeNum && x.SchedDate==dateTimeCur).OrderBy(x => x.StartTime).ToList();
			Provider provider=Providers.GetFirstOrDefault(x=>x.ProvNum==_provNum);
			if(provider!=null) {
				formPhoneGraphEdit.ProvNum=_provNum;
				formPhoneGraphEdit.ListSchedulesProv=_listSchedulesProvs.FindAll(x=>x.ProvNum==provider.ProvNum && x.SchedType==ScheduleType.Provider && x.SchedDate==dateTimeCur);
			}
			formPhoneGraphEdit.ShowDialog();
			if(formPhoneGraphEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
		}

		private void ListBoxEmployees_SelectedIndexChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void OdDatePickerTo_DateTextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void OdDatePickerFrom_DateTextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			_gridRowCopied=null;
			_provNum=0;
			if(odDatePickerFrom.GetDateTime().Year<1880) {
				return;//Stops an issue where removing date, month, or year causes GetDateTime() to return DateTime.MinValue
			}
			TimeSpan timeSpanBetween=odDatePickerTo.GetDateTime()-odDatePickerFrom.GetDateTime();
			if(listBoxEmployees.GetListSelected<Employee>().Count==0) {
				return;
			}
			Employee employeeCur=listBoxEmployees.GetSelected<Employee>();
			Provider providerForEmp=Providers.GetFirstOrDefault(x=>x.FName==employeeCur.FName);
			_listSchedulesEmpCur=Schedules.GetSchedListForDateRange(employeeCur.EmployeeNum,odDatePickerFrom.GetDateTime(),odDatePickerTo.GetDateTime());
			_listSchedulesProvs=Schedules.GetAllForDateRangeAndType(odDatePickerFrom.GetDateTime(),odDatePickerTo.GetDateTime(),ScheduleType.Provider);
			List<Schedule> listSchedulesCurProv=new List<Schedule>();
			if(providerForEmp!=null) {
				_provNum=providerForEmp.ProvNum;
				listSchedulesCurProv=_listSchedulesProvs.FindAll(x => x.ProvNum==_provNum);
			}
			_listPhoneGraphsEmpCur=PhoneGraphs.GetAllForEmployeeNum(employeeCur.EmployeeNum);
			PhoneEmpDefault phoneEmpDefault=PhoneEmpDefaults.GetOne(employeeCur.EmployeeNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableEmployeeTimeDate","Date"),95);
			gridMain.ListGridColumns.Add(col); 
			col=new GridColumn(Lan.g("TableEmployeeTimeDate","Employee Sched"),210);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableEmployeeTimeDate","Employee Overrides"),210);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableEmployeeTimeDate","Provider Sched"),210);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableEmployeeTimeDate","Notes"),120);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0; i<=timeSpanBetween.Days;i++) {
				DateTime dateTimeCur=odDatePickerFrom.GetDateTime().AddDays(i);
				//Date
				GridCell cell=new GridCell();
				row=new GridRow();
				cell.Text=dateTimeCur.ToShortDateString()+" "+dateTimeCur.ToString("ddd");
				if(dateTimeCur.Date==DateTime.Today.Date) {
					cell.Bold=YN.Yes;
				}
				row.Cells.Add(cell);
				List<Schedule> listScheduleEmp=_listSchedulesEmpCur.FindAll(x => x.SchedDate==dateTimeCur).OrderBy(x => x.StartTime).ToList();
				if(listScheduleEmp.Count>0) {
					row.Cells.Add(Schedules.GetCommaDelimStringForScheds(listScheduleEmp));
				}
				else {
					row.Cells.Add("");//Nothing is scheduled
				}
				//Override shift times
				PhoneGraph phoneGraph=_listPhoneGraphsEmpCur.Find(x => x.DateEntry==dateTimeCur);
				if(phoneGraph!=null && phoneGraph.DateTimeStart1.Year>1880){
					row.Cells.Add(PhoneGraphs.GetCommaDelimStringTimes(phoneGraph));
				}
				else if(phoneGraph!=null && phoneGraph.Absent){
					row.Cells.Add("Absent");
				}
				else if(phoneGraph!=null && phoneGraph.PreSchedOff){
					row.Cells.Add("Prescheduled Off");
				}
				else{
					row.Cells.Add("");//Nothing is scheduled
				}
				//Scheduled provider time
				List<Schedule> listSchedulesProv=listSchedulesCurProv.FindAll(x => x.SchedDate==dateTimeCur);
				if(listSchedulesProv.Count>0) {
					row.Cells.Add(Schedules.GetCommaDelimStringForScheds(listSchedulesProv));
				}
				else {
					row.Cells.Add("");
				}
				//Notes
				if(phoneGraph!=null) {
					row.Cells.Add(phoneGraph.Note);
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=phoneEmpDefault;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			int yPos=bounds.Top;
			#region printHeading
			if(!_headingPrinted) {
				text=listBoxEmployees.GetSelected<Employee>().FName+"'s Schedule";
				g.DrawString(text,headingFont,Brushes.Black,550-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				yPos+=5;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			e.HasMorePages=false;//Only print one page
			g.Dispose();

		}




	}
}