using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildCareMap:FrmODBase {
		/*
		There was a bug that occured when a child's two most recent entries were at the same time. When one entry was coming to and the other was leaving, the child ended up showing in both the absent grid and a classroom grid. This occured because when a child was moved to a different room, both a leaving and coming to entry were made, which were at the same exact time.
		There was another bug that occured when we had a parent and a staff member check in the same child at the exact same time, one in the check in/out window and one in the map. Say the staff member assigns a child to a room when they are in the absent grid, a log for that child will be created. If at a fraction of a second later a parent also tries to sign in the same child, a leaving entry is created for the room the child was just sent to and then a coming to entry is created for the child's primary room. This leaves us with three entries all at the same time, meaning the same child will show up in two of the classroom grids. This may not be a common scenario, but it can be recreated fairly easily in testing.
		I discussed these issues with AdrianV and JasonS. They suggested using sorting by the primary key of the log table instead of datetime as we are essentially just trying to determine which is the most recent log for a child or teacher. The PKs should increase in order and I do not think we are going to delete any logs as they are a child's documented location history. If there was ever an issue with the daycare or some sort of audit from the state, this information may be important.
		From testing the use of PK for the map sorting, it seems to be much more bullet proof than using datetime. The two bugs above are no longer an issue. It also seems to work fine while having two maps running at once.
		*/
		///<summary>Right click options for the classroom grids.</summary>
		private ContextMenu _contextMenu;
		///<summary>Right click option for the absent children grid to send a child to their primary room.</summary>
		private ContextMenu _contextMenuAbsent;
		///<summary>Gets filled when a grid is right clicked on for the context menu options.</summary>
		private long _childRoomNumClicked;
		private Grid _gridChildRoomClick;

		///<summary></summary>
		public FrmChildCareMap() {
			InitializeComponent();
			Load+=FrmChildren_Load;
			_contextMenu=new ContextMenu(this);
			_contextMenu.Opened+=ContextMenu_Opened;
			_contextMenu.Add(new MenuItem("Remove",menuItemRemove_Click));
			gridChildRoom1.ContextMenu=_contextMenu;
			gridChildRoom2.ContextMenu=_contextMenu;
			gridChildRoom3.ContextMenu=_contextMenu;
			gridChildRoom4.ContextMenu=_contextMenu;
			gridChildRoom5.ContextMenu=_contextMenu;
			gridChildRoom6.ContextMenu=_contextMenu;
			gridChildRoom7.ContextMenu=_contextMenu;
			gridChildRoom8.ContextMenu=_contextMenu;
			_contextMenuAbsent=new ContextMenu(this);
			_contextMenuAbsent.Add(new MenuItem("Send to Primary Classroom",menuItemSendToPrimary_Click));
			gridChildrenAbsent.ContextMenu=_contextMenuAbsent;
		}

		private void FrmChildren_Load(object sender, EventArgs e) {
			StartMaximized=true;
			//TODO: If not a daycare worker, hide buttons
			//Tag each grid with a hardcoded value that represents a ChildRoomNum. Childrooms can be created and edited, but not deleted. If we end up with more than 8 childrooms, then more grids would need to be added to this frm.
			//Tag each grid button with a hardcoded value that represents a ChildRoomNum.
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			gridChildRoom1.Tag=1;
			gridChildRoom1.Title=listChildRooms.Find(x => x.ChildRoomNum==1).RoomId;
			butViewLogs_Grid1.Tag=1;
			butAddChild_Grid1.Tag=1;
			butAddTeacher_Grid1.Tag=1;
			gridChildRoom2.Tag=2;
			gridChildRoom2.Title=listChildRooms.Find(x => x.ChildRoomNum==2).RoomId;
			butViewLogs_Grid2.Tag=2;
			butAddChild_Grid2.Tag=2;
			butAddTeacher_Grid2.Tag=2;
			gridChildRoom3.Tag=3;
			gridChildRoom3.Title=listChildRooms.Find(x => x.ChildRoomNum==3).RoomId;
			butViewLogs_Grid3.Tag=3;
			butAddChild_Grid3.Tag=3;
			butAddTeacher_Grid3.Tag=3;
			gridChildRoom4.Tag=4;
			gridChildRoom4.Title=listChildRooms.Find(x => x.ChildRoomNum==4).RoomId;
			butViewLogs_Grid4.Tag=4;
			butAddChild_Grid4.Tag=4;
			butAddTeacher_Grid4.Tag=4;
			gridChildRoom5.Tag=5;
			gridChildRoom5.Title=listChildRooms.Find(x => x.ChildRoomNum==5).RoomId;
			butViewLogs_Grid5.Tag=5;
			butAddChild_Grid5.Tag=5;
			butAddTeacher_Grid5.Tag=5;
			gridChildRoom6.Tag=6;
			gridChildRoom6.Title=listChildRooms.Find(x => x.ChildRoomNum==6).RoomId;
			butViewLogs_Grid6.Tag=6;
			butAddChild_Grid6.Tag=6;
			butAddTeacher_Grid6.Tag=6;
			gridChildRoom7.Tag=7;
			gridChildRoom7.Title=listChildRooms.Find(x => x.ChildRoomNum==7).RoomId;
			butViewLogs_Grid7.Tag=7;
			butAddChild_Grid7.Tag=7;
			butAddTeacher_Grid7.Tag=7;
			gridChildRoom8.Tag=8;
			gridChildRoom8.Title=listChildRooms.Find(x => x.ChildRoomNum==8).RoomId;
			butViewLogs_Grid8.Tag=8;
			butAddChild_Grid8.Tag=8;
			butAddTeacher_Grid8.Tag=8;
			//Call all fillgrids
			FillAllGrids();
			GlobalFormOpenDental.EventProcessSignalODs+=GlobalFormOpenDental_EventProcessSignalODs;
		}

		private void GlobalFormOpenDental_EventProcessSignalODs(object sender,List<Signalod> listSignalods) {
			for(int i=0;i<listSignalods.Count;i++) {
				if(listSignalods[i].IType!=InvalidType.Children) {
					continue;
				}
				FillAllGrids();
				return;
			}
		}

		private void FillAllGrids() {
			FillGridTeachersAbsent();
			FillGridChildrenAbsent();
			FillGridSpecified(gridChildRoom1);
			FillGridSpecified(gridChildRoom2);
			FillGridSpecified(gridChildRoom3);
			FillGridSpecified(gridChildRoom4);
			FillGridSpecified(gridChildRoom5);
			FillGridSpecified(gridChildRoom6);
			FillGridSpecified(gridChildRoom7);
			FillGridSpecified(gridChildRoom8);
		}

		private void FillGridTeachersAbsent() {
			List<Employee> listEmployeesAbsent=Employees.GetDeepCopy();
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllEmployeesForDate(DateTime.Today.Date);
			List<long> listEmployeeNumsUnique=listChildRoomLogs.FindAll(x => x.EmployeeNum!=0).Select(y => y.EmployeeNum).Distinct().ToList();
			for(int i=0;i<listEmployeeNumsUnique.Count;i++) {
				//Find the most recent log for the given employee ordering by PK
				ChildRoomLog childRoomLog=listChildRoomLogs.FindAll(x => x.EmployeeNum==listEmployeeNumsUnique[i])
					.OrderByDescending(y => y.ChildRoomLogNum).First();
				if(childRoomLog.IsComing==false) {
					continue;//If they are absent, then keep in list
				}
				listEmployeesAbsent.RemoveAll(x => x.EmployeeNum==listEmployeeNumsUnique[i]);
			}
			List<Employee> listEmployeesSorted=listEmployeesAbsent.OrderBy(x => x.LName).ToList();
			//Begin to fill the grid
			gridTeachersUnassigned.BeginUpdate();
			gridTeachersUnassigned.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			gridTeachersUnassigned.Columns.Add(gridColumn);
			gridTeachersUnassigned.ListGridRows.Clear();
			for(int i=0;i<listEmployeesSorted.Count;i++) {
				Employee employee=listEmployeesSorted[i];
				if(employee.IsHidden) {
					continue;
				}
				if(employee.ClockStatus.ToLower()!="working") {
					continue;//Only show teachers who are working
				}
				GridRow gridRow=new GridRow();
				GridCell gridCell=new GridCell();
				gridCell.Text=employee.FName+" "+employee.LName;
				gridCell.ColorBackG=Color.FromRgb(255,240,240);//Match the color in the classroom grids
				gridRow.Cells.Add(gridCell);
				gridRow.Tag=employee;
				gridTeachersUnassigned.ListGridRows.Add(gridRow);
			}
			gridTeachersUnassigned.EndUpdate();
		}

		private void FillGridChildrenAbsent() {
			List<Child> listChildrenAbsent=Children.GetAll();
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllChildrenForDate(DateTime.Now.Date);
			List<long> listChildNumsUnique=listChildRoomLogs.Select(x => x.ChildNum).Distinct().ToList();
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				//Find the most recent log for the given child ordering by PK
				ChildRoomLog childRoomLog=listChildRoomLogs.FindAll(x => x.ChildNum==listChildNumsUnique[i]).OrderByDescending(y => y.ChildRoomLogNum).First();
				if(childRoomLog.IsComing==false) {//If they are absent, then keep in list
					continue;
				}
				listChildrenAbsent.RemoveAll(x => x.ChildNum==listChildNumsUnique[i]);//Remove from absent list since they are present
			}
			List<Child> listChildrenSorted=listChildrenAbsent.OrderBy(x => x.LName).ToList();
			//Begin to fill the grid
			gridChildrenAbsent.BeginUpdate();
			gridChildrenAbsent.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			gridChildrenAbsent.Columns.Add(gridColumn);
			gridChildrenAbsent.ListGridRows.Clear();
			for(int i=0;i<listChildrenSorted.Count;i++) {
				if(listChildrenSorted[i].IsHidden) {
					continue;
				}
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(Children.GetName(listChildrenSorted[i]));
				gridRow.Tag=listChildrenSorted[i];
				gridChildrenAbsent.ListGridRows.Add(gridRow);
			}
			gridChildrenAbsent.EndUpdate();
		}

		private void FillGridSpecified(Grid grid) {
			List<Child> listChildren=Children.GetAll();
			double countChildren=0;
			double countEmployees=0;
			//Get room logs for today for a specific room
			List<ChildRoomLog> listChildRoomLogsToday=ChildRoomLogs.GetChildRoomLogs(long.Parse(grid.Tag.ToString()),DateTime.Now.Date);
			List<long> listEmployeeNumsUnique=listChildRoomLogsToday.FindAll(x => x.EmployeeNum!=0).Select(y => y.EmployeeNum).Distinct().ToList();
			//List of logs for people currently in the room
			List<ChildRoomLog> listChildRoomLogs=new List<ChildRoomLog>();
			//Find the employees in this room
			for(int i=0;i<listEmployeeNumsUnique.Count;i++) {
				//Find the most recent log for the given employee ordering by PK
				ChildRoomLog childRoomLogEmployee=listChildRoomLogsToday.FindAll(x => x.EmployeeNum==listEmployeeNumsUnique[i])
					.OrderByDescending(y => y.ChildRoomLogNum).First();
				if(!childRoomLogEmployee.IsComing) {
					continue;//Employee is not present in this room
				}
				listChildRoomLogs.Add(childRoomLogEmployee);
			}
			List<long> listChildNumsUnique=listChildRoomLogsToday.FindAll(x => x.ChildNum!=0).Select(y => y.ChildNum).Distinct().ToList();
			//Find the children in this room
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				//Find the most recent log for the given child ordering by PK
				ChildRoomLog childRoomLogChild=listChildRoomLogsToday.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.ChildRoomLogNum).First();
				if(!childRoomLogChild.IsComing) {
					continue;//Child is not present in this room
				}
				listChildRoomLogs.Add(childRoomLogChild);
			}
			//Ratio change entries do not need to be removed because they will never be added to the list
			//Begin to fill the grid
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			grid.Columns.Add(gridColumn);
			//gridColumn=new GridColumn("",75);
			//gridColumn. //button later
			//grid.Columns.Add(gridColumn);
			grid.ListGridRows.Clear();
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				GridRow gridRow=new GridRow();
				GridCell gridCell=new GridCell();
				if(listChildRoomLogs[i].ChildNum!=0) {//Child entry
					Child child=listChildren.Find(x => x.ChildNum==listChildRoomLogs[i].ChildNum);
					gridCell.Text=Children.GetName(child);
					countChildren++;
				}
				else {//Employee/teacher entry
					Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==listChildRoomLogs[i].EmployeeNum);
					if(employee.ClockStatus.ToLower()!="working") {
						continue;//Only show teachers who are working
					}
					gridCell.Text=employee.FName+" "+employee.LName;
					gridCell.ColorBackG=Color.FromRgb(255,240,240);//Pale pink to distinguish between teachers and children
					countEmployees++;
				}
				gridRow.Cells.Add(gridCell);
				gridCell=new GridCell();
				//gridCell. //button later
				gridRow.Tag=listChildRoomLogs[i];
				grid.ListGridRows.Add(gridRow);
			}
			if(grid.ListGridRows.Count==0) {//Empty grids do not get a ratio
				grid.EndUpdate();
				return;
			}
			//Final row is the child to teacher ratio
			GridRow gridRowFinal=new GridRow();
			if(countEmployees==0) {//No employees, stop division by 0.
				gridRowFinal.Cells.Add("Current Ratio: ?");
			}
			else {
				gridRowFinal.Cells.Add("Current Ratio: "+(countChildren/countEmployees).ToString());
			}
			grid.ListGridRows.Add(gridRowFinal);
			grid.EndUpdate();
		}

		private void ContextMenu_Opened(object sender,EventArgs e) {
			ContextMenu contextMenu=(ContextMenu)sender;
			Grid grid=(Grid)contextMenu.PlacementTarget;
			_childRoomNumClicked=long.Parse(grid.Tag.ToString());
			_gridChildRoomClick=grid;
		}

		private void menuItemRemove_Click(object sender,EventArgs e) {
			int idxSelected=_gridChildRoomClick.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a row first.");
				return;
			}
			if(_gridChildRoomClick.ListGridRows[idxSelected].Tag==null) {
				MsgBox.Show("This row cannot be removed");
				return;//Every row except the current ratio will have a tag
			}
			ChildRoomLog childRoomLog=(ChildRoomLog)_gridChildRoomClick.ListGridRows[idxSelected].Tag;
			List<ChildRoomLog> listChildRoomLogs=new List<ChildRoomLog>();
			if(childRoomLog.ChildNum!=0) {//Child row selected
				listChildRoomLogs=ChildRoomLogs.GetAllLogsForChild(childRoomLog.ChildNum,DateTime.Now.Date);
			}
			else {//Employee row selected
				listChildRoomLogs=ChildRoomLogs.GetAllLogsForEmployee(childRoomLog.EmployeeNum,DateTime.Now.Date);
			}
			ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
			//Refresh
			FillGridSpecified(_gridChildRoomClick);
			FillGridChildrenAbsent();
			FillGridTeachersAbsent();
		}

		private void menuItemSendToPrimary_Click(object sender,EventArgs e) {
			int idxSelected=gridChildrenAbsent.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a child first.");
				return;
			}
			Child childSelected=(Child)gridChildrenAbsent.ListGridRows[idxSelected].Tag;
			//If for some reason a child is already in a room due to a delay in syncing, create a leaving entry if needed
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForChild(childSelected.ChildNum,DateTime.Now);
			ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
			//Create coming to log for their primary room
			ChildRoomLog childRoomLog=new ChildRoomLog();
			childRoomLog.ChildNum=childSelected.ChildNum;
			childRoomLog.DateTDisplayed=DateTime.Now;
			childRoomLog.DateTEntered=DateTime.Now;
			childRoomLog.ChildRoomNum=childSelected.ChildRoomNumPrimary;
			childRoomLog.IsComing=true;
			ChildRoomLogs.Insert(childRoomLog);
			//Sync and refresh
			Signalods.SetInvalid(InvalidType.Children);
			FillAllGrids();
		}

		private void butChildren_Click(object sender,EventArgs e) {
			FrmChildren frmChildren=new FrmChildren();
			frmChildren.ShowDialog();
		}

		private void butClassrooms_Click(object sender,EventArgs e) {
			FrmChildRooms frmChildRooms=new FrmChildRooms();
			frmChildRooms.ShowDialog();
		}

		private void butParents_Click(object sender,EventArgs e) {
			FrmChildParents frmChildParents=new FrmChildParents();
			frmChildParents.ShowDialog();
		}

		private void butViewLogs_Grid_Click(object sender,EventArgs e) {
			//Determine which room the click was for
			int childRoomNum=(int)((Button)sender).Tag;
			FrmChildRoomLogs frmChildRoomLogs=new FrmChildRoomLogs();
			frmChildRoomLogs.ChildRoomNumInitial=childRoomNum;
			frmChildRoomLogs.ShowDialog();
		}

		private void butAddChild_Grid_Click(object sender,EventArgs e) {
			//Determine which room the click was for
			int childRoomNum=(int)((Button)sender).Tag;
			//Select a child
			FrmChildren frmChildren=new FrmChildren();
			frmChildren.IsSelectionMode=true;
			frmChildren.ShowDialog();
			if(frmChildren.IsDialogCancel) {
				return;//Kick out if no child was selected
			}
			//If the selected child is currently in a room, remove them using a going log
			Child child=Children.GetOne(frmChildren.ChildNumSelected);
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForChild(child.ChildNum,DateTime.Now.Date);
			//OrderBy PK as well in case there are multiple entries with the same time
			ChildRoomLog childRoomLog=listChildRoomLogs.OrderByDescending(x => x.ChildRoomLogNum).FirstOrDefault();
			if(childRoomLog!=null) {
				if(childRoomLog.ChildRoomNum==childRoomNum && childRoomLog.IsComing) {
					MsgBox.Show("The selected child is already in this room.");
					return;//Kick out if they are already in the selected room
				}
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
			}
			//Log the child coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.ChildNum=child.ChildNum;
			//Add one second so there is a difference between the time leaving the old room and entering the new room
			childRoomLogNew.DateTEntered=DateTime.Now.AddSeconds(1);
			childRoomLogNew.DateTDisplayed=DateTime.Now.AddSeconds(1);
			childRoomLogNew.ChildRoomNum=childRoomNum;
			childRoomLogNew.IsComing=true;
			ChildRoomLogs.Insert(childRoomLogNew);
			Signalods.SetInvalid(InvalidType.Children);
			//Refresh
			FillAllGrids();
		}

		private void butAddTeacher_Grid_Click(object sender,EventArgs e) {
			//Determine which room the click was for
			int childRoomNum=(int)((Button)sender).Tag;
			//Select an employee/teacher
			FrmChildTeacherSelect frmChildTeacherSelect=new FrmChildTeacherSelect();
			frmChildTeacherSelect.ShowDialog();
			if(frmChildTeacherSelect.IsDialogCancel) {
				return;//Kick out if no employee/teacher was selected
			}
			//If the selected employee is currently in a room, remove them using a going log
			Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==frmChildTeacherSelect.EmployeeNumSelected);
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForEmployee(employee.EmployeeNum,DateTime.Now.Date);
			//OrderBy PK as well in case there are multiple entries with the same time
			ChildRoomLog childRoomLog=listChildRoomLogs.OrderByDescending(x => x.ChildRoomLogNum).FirstOrDefault();
			if(childRoomLog!=null) {
				if(childRoomLog.ChildRoomNum==childRoomNum && childRoomLog.IsComing) {
					MsgBox.Show("The selected teacher is already in this room.");
					return;//Kick out if they are already in the selected room
				}
				ChildRoomLogs.CreateChildRoomLogLeaving(listChildRoomLogs);
			}
			//Log the teacher coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.EmployeeNum=employee.EmployeeNum;
			//Add one second so there is a difference between the time leaving the old room and entering the new room
			childRoomLogNew.DateTEntered=DateTime.Now.AddSeconds(1);
			childRoomLogNew.DateTDisplayed=DateTime.Now.AddSeconds(1);
			childRoomLogNew.ChildRoomNum=childRoomNum;
			childRoomLogNew.IsComing=true;
			ChildRoomLogs.Insert(childRoomLogNew);
			Signalods.SetInvalid(InvalidType.Children);
			//Refresh
			FillAllGrids();
		}
	}
}