using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildRoomLogs:FrmODBase {
		///<summary>The room that was right clicked on in the childcare map window or selected in the combobox filter.</summary>
		public long ChildRoomNumInitial;

		///<summary></summary>
		public FrmChildRoomLogs() {
			InitializeComponent();
			Load+=FrmChildRoomLogs_Load;
			gridMain.CellDoubleClick+=GridMain_CellDoubleClick;
			listBoxChildRooms.SelectionChangeCommitted+=listBoxChildRooms_SelectionChangeCommitted;
			monthlyCalendarLog.DateChanged+=monthlyCalendarLog_DateChanged;
		}

		private void FrmChildRoomLogs_Load(object sender,EventArgs e) {
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			listBoxChildRooms.Items.AddList(listChildRooms,x=>x.RoomId);
			listBoxChildRooms.SetSelectedKey<ChildRoom>(ChildRoomNumInitial,x=>x.ChildRoomNum);
			FillGrid();
		}

		private void FillGrid() {
			//New without IsComing
			DateTime dateSelected=monthlyCalendarLog.GetDateSelected();
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetChildRoomLogsForDate(dateSelected);
			ChildRoom childRoom=listBoxChildRooms.GetSelected<ChildRoom>();
			double ratioAllowed=ChildRoomLogs.GetPreviousRatio(childRoom.ChildRoomNum,dateSelected);
			List<Child> listChildren=Children.GetAll();
			//Keeps track if a person's most recent log was a present log
			List<ChildRoomLog> listChildRoomLogsPresent=new List<ChildRoomLog>();
			double countChildren=0;
			double countEmployees=0;
			int countUnderTwo=0;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Child",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Age",35,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teacher",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Time",70);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("In/Out",50,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Under 2",55,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Total Children",85,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Allowed Ratio",85,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teachers Present",100,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teachers Required",110,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				//A row will be for either a child, an employee(teacher), or a ratio change. The other name row will be set to a default value and the ratio column will be filled
				//Example: If this is a child row, then the Child column will be filled, but the Teacher be an empty string. The Allowed Ratio column will be filled.
				ChildRoomLog childRoomLog=listChildRoomLogs[i];
				if(childRoomLog.ChildRoomNum!=childRoom.ChildRoomNum) {
					//Kick out if this log if for a different room and this person is not present in the current room
					//Otherwise make a row to show them leaving
					if(!listChildRoomLogsPresent.Any(x => x.ChildNum==childRoomLog.ChildNum)) {
						continue;
					}
					else if(!listChildRoomLogsPresent.Any(x => x.EmployeeNum==childRoomLog.EmployeeNum)) {
						continue;
					}
				}
				GridRow gridRow=new GridRow();
				if(childRoomLog.ChildNum!=0) {//Child row
					Child child=listChildren.Find(x => x.ChildNum==listChildRoomLogs[i].ChildNum);
					string childName=Children.GetName(child);
					gridRow.Cells.Add(childName);
					int age=DateTime.Today.Year-child.BirthDate.Year;
					if(age==0) {
						gridRow.Cells.Add("<1");
					}
					else {
						gridRow.Cells.Add(age.ToString());
					}
					gridRow.Cells.Add("");//Skip teacher column
					//Find the current number of children
					if(childRoomLog.ChildRoomNum==childRoom.ChildRoomNum) {
						countChildren++;
						if(dateSelected.AddYears(-2)<child.BirthDate) {
							countUnderTwo++;
						}
					}
					else {//Leaving row
						countChildren--;
						if(dateSelected.AddYears(-2)<child.BirthDate) {
							countUnderTwo--;
						}
					}
				}
				else if(childRoomLog.EmployeeNum!=0) {//Teacher row
					gridRow.Cells.Add("");//Skip child column
					gridRow.Cells.Add("");//Skip age column
					Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==childRoomLog.EmployeeNum);
					string employeeName=employee.FName+" "+employee.LName;
					gridRow.Cells.Add(employeeName);
					gridRow.ColorBackG=Color.FromRgb(255,240,240);//Match the color in the classroom grids
					if(childRoomLog.ChildRoomNum==childRoom.ChildRoomNum) {
						countEmployees++;
					}
					else {//Leaving row
						countEmployees--;
					}
				}
				else {//Ratio row
					gridRow.Cells.Add("");//Skip child column
					gridRow.Cells.Add("");//Skip age column
					gridRow.Cells.Add("");//Skip teacher column
				}
				gridRow.Cells.Add(listChildRoomLogs[i].DateTDisplayed.ToLongTimeString());
				if(listChildRoomLogs[i].ChildNum==0 && listChildRoomLogs[i].EmployeeNum==0) {
					gridRow.Cells.Add("");//RatioChange does not get a location status
				}
				else if(childRoomLog.ChildRoomNum==childRoom.ChildRoomNum) {
					gridRow.Cells.Add("In");
					listChildRoomLogsPresent.Add(childRoomLog);//Add to present list to track person entering
				}
				else {
					gridRow.Cells.Add("Out");
					//Remove from present list to track person leaving
					if(childRoomLog.ChildNum!=0) {
						listChildRoomLogsPresent.RemoveAll(x => x.ChildNum==childRoomLog.ChildNum);
					}
					else {
						listChildRoomLogsPresent.RemoveAll(x => x.EmployeeNum==childRoomLog.EmployeeNum);
					}
				}
				if(listChildRoomLogs[i].ChildNum==0 && listChildRoomLogs[i].EmployeeNum==0) {//RatioChange entry
					ratioAllowed=listChildRoomLogs[i].RatioChange;
				}
				gridRow.Cells.Add(countUnderTwo.ToString());
				gridRow.Cells.Add(countChildren.ToString());
				if(ratioAllowed==-1) {
					gridRow.Cells.Add("Mixed");
				}
				else {
					gridRow.Cells.Add(ratioAllowed.ToString());
				}
				gridRow.Cells.Add(countEmployees.ToString());
				double teachersRequired=0;
				if(ratioAllowed==-1) {
					//Calculate the number of teachers needed for the current mixed ratio
					teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:(int)countChildren,childrenUnderTwo:countUnderTwo);
				}
				else {
					if(ratioAllowed!=0) {//Stop division by 0
						teachersRequired=Math.Ceiling(countChildren/ratioAllowed);
					}
				}
				GridCell gridCell=new GridCell(teachersRequired.ToString());
				if(countEmployees<teachersRequired) {
					gridCell.ColorText=Colors.Red;
				}
				gridRow.Cells.Add(gridCell);
				gridRow.Tag=listChildRoomLogs[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
			gridMain.ScrollToEnd();
		}

		private void GridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			//When double clicking on an "Out" row the log where the person went to another room will open
			ChildRoomLog childRoomLogSelected=gridMain.SelectedTag<ChildRoomLog>();
			FrmChildRoomLogEdit frmChildRoomLogEdit=new FrmChildRoomLogEdit();
			frmChildRoomLogEdit.ChildRoomLogCur=childRoomLogSelected;
			frmChildRoomLogEdit.ShowDialog();
			if(frmChildRoomLogEdit.IsDialogCancel) {
				return;
			}
			FillGrid();
		}

		private void butViewSnapshot_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Select a log first.");
				return;
			}
			ChildRoomLog childRoomLog=gridMain.SelectedTag<ChildRoomLog>();
			FrmChildMapLog frmChildMapLog=new FrmChildMapLog();
			frmChildMapLog.ChildRoomNumInitial=childRoomLog.ChildRoomNum;
			frmChildMapLog.DateTimeInitial=childRoomLog.DateTDisplayed;
			frmChildMapLog.ShowDialog();
		}

		private void butAddLog_Click(object sender,EventArgs e) {
			FrmChildRoomLogEdit frmChildRoomLogEdit=new FrmChildRoomLogEdit();
			ChildRoomLog childRoomLog=new ChildRoomLog();
			childRoomLog.IsNew=true;
			childRoomLog.DateTEntered=DateTime.Now;
			childRoomLog.DateTDisplayed=DateTime.Now;
			childRoomLog.ChildRoomNum=listBoxChildRooms.GetSelected<ChildRoom>().ChildRoomNum;
			frmChildRoomLogEdit.ChildRoomLogCur=childRoomLog;
			frmChildRoomLogEdit.ShowDialog();
			if(frmChildRoomLogEdit.IsDialogCancel) {
				return;
			}
			FillGrid();
		}

		private void listBoxChildRooms_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void monthlyCalendarLog_DateChanged(object sender,EventArgs e) {
			FillGrid();
		}
	}
}