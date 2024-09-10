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
			comboChildRoom.SelectionChangeCommitted+=comboChildRoom_SelectionChangeCommitted;
			gridMain.CellDoubleClick+=GridMain_CellDoubleClick;
		}

		private void FrmChildRoomLogs_Load(object sender,EventArgs e) {
			textVDateLog.Value=DateTime.Now;
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			comboChildRoom.Items.AddList(listChildRooms,x => x.RoomId);
			comboChildRoom.SetSelectedKey<ChildRoom>(ChildRoomNumInitial,x=>x.ChildRoomNum);
			FillGrid();
		}

		private void FillGrid() {
			if(!textVDateLog.IsValid()) {
				return;
			}
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetChildRoomLogs(
				comboChildRoom.GetSelectedKey<ChildRoom>(x=>x.ChildRoomNum),textVDateLog.Value);
			List<Child> listChildren=Children.GetAll();
			double countChildren=0;
			double countEmployees=0;
			int countUnderTwo=0;
			long childRoomNum=comboChildRoom.GetSelectedKey<ChildRoom>(x=>x.ChildRoomNum);
			double ratioAllowed=ChildRoomLogs.GetPreviousRatio(childRoomNum,textVDateLog.Value);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Child",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teacher",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Time",90);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("In/Out",50,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Under 2",75,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teachers",75,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Allowed Ratio",85,HorizontalAlignment.Center);
			gridMain.Columns.Add(gridColumn);
			ChildRoom childRoom=ChildRooms.GetOne(childRoomNum);
			if(childRoom.Ratio==-1) {//If currenlty a mixed ratio, then total children column
				gridColumn=new GridColumn("Total Children",85,HorizontalAlignment.Center);
				gridMain.Columns.Add(gridColumn);
			}
			else {//If currently not a mixed ratio, then current ratio column
				gridColumn=new GridColumn("Current Ratio",85,HorizontalAlignment.Center);
				gridMain.Columns.Add(gridColumn);
			}
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				GridRow gridRow=new GridRow();
				//A row will be for either a child, an employee(teacher), or a ratio change. The other name row will be set to a default value and the ratio column will be filled
				//Example: If this is a child row, then the Child column will be filled, but the Teacher be an empty string. The Allowed Ratio column will be filled.
				if(listChildRoomLogs[i].ChildNum!=0) {//Child entry
					Child child=listChildren.Find(x => x.ChildNum==listChildRoomLogs[i].ChildNum);
					string childName=Children.GetName(child);
					gridRow.Cells.Add(childName);
					gridRow.Cells.Add("");
					//Find the current number of children
					if(listChildRoomLogs[i].IsComing) {
						countChildren++;
						if(textVDateLog.Value.AddYears(-2)<child.BirthDate) {
							countUnderTwo++;
						}
					}
					else {
						countChildren--;
						if(textVDateLog.Value.AddYears(-2)<child.BirthDate) {
							countUnderTwo--;
						}
					}
				}
				else if(listChildRoomLogs[i].EmployeeNum!=0) {//Employee entry
					gridRow.Cells.Add("");
					Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==listChildRoomLogs[i].EmployeeNum);
					string employeeName=employee.FName+" "+employee.LName;
					gridRow.Cells.Add(employeeName);
					gridRow.ColorBackG=Color.FromRgb(255,240,240);//Match the color in the classroom grids
					//Find the current number of employees
					if(listChildRoomLogs[i].IsComing) {
						countEmployees++;
					}
					else {
						countEmployees--;
					}
				}
				else {//RatioChange entry
					gridRow.Cells.Add("");
					gridRow.Cells.Add("");
				}
				gridRow.Cells.Add(listChildRoomLogs[i].DateTDisplayed.ToLongTimeString());
				if(listChildRoomLogs[i].ChildNum==0 && listChildRoomLogs[i].EmployeeNum==0) {
					gridRow.Cells.Add("");//RatioChange does not get a location status
				}
				else if(listChildRoomLogs[i].IsComing) {
					gridRow.Cells.Add("In");
				}
				else {
					gridRow.Cells.Add("Out");
				}
				if(listChildRoomLogs[i].ChildNum==0 && listChildRoomLogs[i].EmployeeNum==0) {//RatioChange entry
					ratioAllowed=listChildRoomLogs[i].RatioChange;
				}
				if(ratioAllowed==-1) {
					gridRow.Cells.Add(countUnderTwo.ToString());
					//Calculate the number of teachers needed for the current mixed ratio
					double teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:(int)countChildren,childrenUnderTwo:countUnderTwo);
					if(teachersRequired==0) {
						gridRow.Cells.Add(countEmployees+" of 0");
					}
					else {
						GridCell gridCell=new GridCell(countEmployees+" of "+teachersRequired);
						if(countEmployees<teachersRequired) {
							gridCell.ColorText=Colors.Red;
						}
						gridRow.Cells.Add(gridCell);
					}
				}
				else {
					gridRow.Cells.Add("");
					if(ratioAllowed==0) {//Stop division by 0
						gridRow.Cells.Add(countEmployees+" of 0");
					}
					else {
						double teachersRequired=Math.Ceiling(countChildren/ratioAllowed);
						GridCell gridCell=new GridCell(countEmployees+" of "+teachersRequired);
						if(countEmployees<teachersRequired) {
							gridCell.ColorText=Colors.Red;
						}
						gridRow.Cells.Add(gridCell);
					}
				}
				if(ratioAllowed==-1) {
					gridRow.Cells.Add("Mixed");
				}
				else {
					gridRow.Cells.Add(ratioAllowed.ToString());
				}
				if(childRoom.Ratio==-1) {//Total number of kids
					gridRow.Cells.Add(countChildren.ToString());
				}
				else {//Calculate current ratio
					if(countEmployees==0) {//There are no employees. Stops division by 0.
						gridRow.Cells.Add("0");
					}
					else {
						double ratioCurrent=Math.Ceiling(countChildren/countEmployees);//Round up so there are not fractions of children
						GridCell gridCell=new GridCell(ratioCurrent.ToString());
						if(ratioAllowed!=-1 && ratioCurrent>ratioAllowed) {
							gridCell.ColorText=Colors.Red;
						}
						gridRow.Cells.Add(gridCell);
					}
				}
				gridRow.Tag=listChildRoomLogs[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
			gridMain.ScrollToEnd();
		}

		private void GridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			ChildRoomLog childRoomLogSelected=gridMain.SelectedTag<ChildRoomLog>();
			if(childRoomLogSelected.EmployeeNum==0 && childRoomLogSelected.ChildNum==0) {
				MsgBox.Show("Cannot edit a ratio change log.");
				return;
			}
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
			childRoomLog.IsComing=true;
			childRoomLog.ChildRoomNum=comboChildRoom.GetSelectedKey<ChildRoom>(x=>x.ChildRoomNum);
			frmChildRoomLogEdit.ChildRoomLogCur=childRoomLog;
			frmChildRoomLogEdit.ShowDialog();
			if(frmChildRoomLogEdit.IsDialogCancel) {
				return;
			}
			FillGrid();
		}

		private void comboChildRoom_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}
	}
}