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
using OpenDental.Drawing;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmChildCareMap:FrmODBase {
		///<summary>Right click options for the classroom grids.</summary>
		private ContextMenu _contextMenu;
		///<summary>Right click option for the absent children grid to send a child to their primary room.</summary>
		private ContextMenu _contextMenuAbsent;
		///<summary>Right click option for the unassigned teachers to clock in from this window.</summary>
		private ContextMenu _contextMenuUnassigned;
		///<summary>Gets filled when a grid is right clicked on for the context menu options.</summary>
		private long _childRoomNumClicked;
		///<summary>Contains the child room grid that was clicked on, mainly used for context menu items. Tag will have what # child room was right clicked.</summary>
		private Grid _gridChildRoomClick;
		private int _idxCharsPrinted;
		private int _pagesPrinted;
		private string _textToPrint;
		///<summary>Set true to disable editing. This should be set true for parents wanting to see where their children are at from ther workstations. False by default.</summary>
		public bool ViewOnly=false;

		///<summary></summary>
		public FrmChildCareMap() {
			InitializeComponent();
			Load+=FrmChildren_Load;
			_contextMenu=new ContextMenu(this);
			_contextMenu.Opened+=ContextMenu_Opened;
			_contextMenu.Add(new MenuItem("Remove",menuItemUnassign_Click));
			_contextMenu.Add(new MenuItem("Clock out",menuItemClockOut_Click));
			_contextMenu.Add(new MenuItem("Break",menuItemBreak_Click));
			gridChildRoom1.ContextMenu=_contextMenu;
			gridChildRoom1.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom2.ContextMenu=_contextMenu;
			gridChildRoom2.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom3.ContextMenu=_contextMenu;
			gridChildRoom3.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom4.ContextMenu=_contextMenu;
			gridChildRoom4.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom5.ContextMenu=_contextMenu;
			gridChildRoom5.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom6.ContextMenu=_contextMenu;
			gridChildRoom6.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom7.ContextMenu=_contextMenu;
			gridChildRoom7.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			gridChildRoom8.ContextMenu=_contextMenu;
			gridChildRoom8.CellDoubleClick+=GridChildRoom_CellDoubleClick;
			_contextMenuAbsent=new ContextMenu(this);
			_contextMenuAbsent.Opened+=ContextMenuAbsent_Opened;
			gridChildrenAbsent.ContextMenu=_contextMenuAbsent;
			_contextMenuUnassigned=new ContextMenu(this);
			_contextMenuUnassigned.Add(new MenuItem("Clock In",menuItemClockIn_Click));
			_contextMenuUnassigned.Add(new MenuItem("Clock Out",menuItemClockOutUnassigned_Click));
			gridTeachersUnassigned.ContextMenu=_contextMenuUnassigned;
		}

		private void FrmChildren_Load(object sender, EventArgs e) {
			StartMaximized=true;
			//Tag each grid with a hardcoded value that represents a ChildRoomNum. Childrooms can be created and edited, but not deleted. If we end up with more than 8 childrooms, then more grids would need to be added to this frm.
			//Tag each grid button with a hardcoded value that represents a ChildRoomNum.
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			gridChildRoom1.Tag=1;
			gridChildRoom1.Title=listChildRooms.Find(x => x.ChildRoomNum==1).RoomId;
			butViewLogs_Grid1.Tag=1;
			butAddChild_Grid1.Tag=1;
			butAddTeacher_Grid1.Tag=1;
			butPrint_Grid1.Tag=1;
			gridChildRoom2.Tag=2;
			gridChildRoom2.Title=listChildRooms.Find(x => x.ChildRoomNum==2).RoomId;
			butViewLogs_Grid2.Tag=2;
			butAddChild_Grid2.Tag=2;
			butAddTeacher_Grid2.Tag=2;
			butPrint_Grid2.Tag=2;
			gridChildRoom3.Tag=3;
			gridChildRoom3.Title=listChildRooms.Find(x => x.ChildRoomNum==3).RoomId;
			butViewLogs_Grid3.Tag=3;
			butAddChild_Grid3.Tag=3;
			butAddTeacher_Grid3.Tag=3;
			butPrint_Grid3.Tag=3;
			gridChildRoom4.Tag=4;
			gridChildRoom4.Title=listChildRooms.Find(x => x.ChildRoomNum==4).RoomId;
			butViewLogs_Grid4.Tag=4;
			butAddChild_Grid4.Tag=4;
			butAddTeacher_Grid4.Tag=4;
			butPrint_Grid4.Tag=4;
			gridChildRoom5.Tag=5;
			gridChildRoom5.Title=listChildRooms.Find(x => x.ChildRoomNum==5).RoomId;
			butViewLogs_Grid5.Tag=5;
			butAddChild_Grid5.Tag=5;
			butAddTeacher_Grid5.Tag=5;
			butPrint_Grid5.Tag=5;
			gridChildRoom6.Tag=6;
			gridChildRoom6.Title=listChildRooms.Find(x => x.ChildRoomNum==6).RoomId;
			butViewLogs_Grid6.Tag=6;
			butAddChild_Grid6.Tag=6;
			butAddTeacher_Grid6.Tag=6;
			butPrint_Grid6.Tag=6;
			gridChildRoom7.Tag=7;
			gridChildRoom7.Title=listChildRooms.Find(x => x.ChildRoomNum==7).RoomId;
			butViewLogs_Grid7.Tag=7;
			butAddChild_Grid7.Tag=7;
			butAddTeacher_Grid7.Tag=7;
			butPrint_Grid7.Tag=7;
			gridChildRoom8.Tag=8;
			gridChildRoom8.Title=listChildRooms.Find(x => x.ChildRoomNum==8).RoomId;
			butViewLogs_Grid8.Tag=8;
			butAddChild_Grid8.Tag=8;
			butAddTeacher_Grid8.Tag=8;
			butPrint_Grid8.Tag=8;
			//Call all fillgrids
			FillAllGrids();
			GlobalFormOpenDental.EventProcessSignalODs+=GlobalFormOpenDental_EventProcessSignalODs;
			if(ViewOnly) {
				SetViewOnly();
			}
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
			FillGridTeachersUnassigned();
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

		private void FillGridTeachersUnassigned() {
			//Remember which teacher was selected after fill grid
			Employee employeeSelected=gridTeachersUnassigned.SelectedTag<Employee>();
			//Get logs of all employees for today
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllEmployeesForDate(DateTime.Today.Date);
			//Get a list of all unique employee nums from today's logs. These are the employees that may be present in a classroom
			List<long> listEmployeeNumsUnique=listChildRoomLogs.FindAll(x => x.EmployeeNum!=0).Select(y => y.EmployeeNum).Distinct().ToList();
			//Track the employees that are absent. Start with all employees and remove ones that are present
			List<Employee> listEmployeesAbsent=Employees.GetDeepCopy();
			for(int i=0;i<listEmployeeNumsUnique.Count;i++) {
				ChildRoomLog childRoomLogMostRecent=listChildRoomLogs.FindAll(x => x.EmployeeNum==listEmployeeNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).First();
				if(childRoomLogMostRecent.ChildRoomNum==0) {
					continue;//Keep in list if they are absent
				}
				listEmployeesAbsent.RemoveAll(x => x.EmployeeNum==listEmployeeNumsUnique[i]);//Remove from list if present in a room
			}
			List<Employee> listEmployeesSorted=listEmployeesAbsent.OrderBy(x => x.LName).ToList();//Sort by last name
			listEmployeesSorted.RemoveAll(x => x.IsHidden);//Remove all hidden employees
			//Begin to fill the grid
			gridTeachersUnassigned.BeginUpdate();
			gridTeachersUnassigned.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",200);
			gridTeachersUnassigned.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Status",100);
			gridTeachersUnassigned.Columns.Add(gridColumn);
			gridTeachersUnassigned.ListGridRows.Clear();
			for(int i=0;i<listEmployeesSorted.Count;i++) {
				Employee employee=listEmployeesSorted[i];
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(employee.FName+" "+employee.LName);
				gridRow.ColorBackG=Color.FromRgb(255,240,240);
				gridRow.Cells.Add(employee.ClockStatus);
				gridRow.Tag=employee;
				gridTeachersUnassigned.ListGridRows.Add(gridRow);
			}
			gridTeachersUnassigned.EndUpdate();
			//Reselect the employee after the fill grid
			if(employeeSelected==null) {
				return;//No employee was selected
			}
			int idx=gridTeachersUnassigned.ListGridRows.FindIndex(x => ((Employee)x.Tag).EmployeeNum==employeeSelected.EmployeeNum);
			if(idx==-1) {
				return;//If for some reason the EmployeeNum was not found
			}
			gridTeachersUnassigned.SetSelected(idx);
		}

		private void FillGridChildrenAbsent() {
			//Remember which child was selected after the fill grid
			Child childSelected=gridChildrenAbsent.SelectedTag<Child>();
			//Get logs of all children for today
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllChildrenForDate(DateTime.Today.Date);
			//Get a list of all unique child nums from today's logs. These are the children that may be present in a classroom
			List<long> listChildNumsUnique=listChildRoomLogs.FindAll(x => x.ChildNum!=0).Select(y => y.ChildNum).Distinct().ToList();
			//Track the children that are absent. Start with all children and remove ones that are present
			List<Child> listChildrenAbsent=Children.GetAll();
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				ChildRoomLog childRoomLogMostRecent=listChildRoomLogs.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).First();
				if(childRoomLogMostRecent.ChildRoomNum==0) {
					continue;//Keep in list if they are absent
				}
				listChildrenAbsent.RemoveAll(x => x.ChildNum==listChildNumsUnique[i]);//Remove from list if present in a room
			}
			List<Child> listChildrenSorted=listChildrenAbsent.OrderBy(x => x.LName).ToList();
			listChildrenSorted.RemoveAll(x => x.IsHidden);//Remove all hidden before the fillgrid
			//Begin to fill the grid
			gridChildrenAbsent.BeginUpdate();
			gridChildrenAbsent.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			gridChildrenAbsent.Columns.Add(gridColumn);
			gridChildrenAbsent.ListGridRows.Clear();
			for(int i=0;i<listChildrenSorted.Count;i++) {
				GridRow gridRow=new GridRow();
				string nameAndAge=Children.GetName(listChildrenSorted[i]);
				int age=DateTime.Today.Year-listChildrenSorted[i].BirthDate.Year;
				if(age==0) {
					nameAndAge+=", <1y";
				}
				else {
					nameAndAge+=", "+age+"y";
				}
				gridRow.Cells.Add(nameAndAge);
				gridRow.Tag=listChildrenSorted[i];
				gridChildrenAbsent.ListGridRows.Add(gridRow);
			}
			gridChildrenAbsent.EndUpdate();
			//Reselect the child after the fill grid
			if(childSelected==null) {
				return;//No child was selected
			}
			int idx=gridChildrenAbsent.ListGridRows.FindIndex(x => ((Child)x.Tag).ChildNum==childSelected.ChildNum);
			if(idx==-1) {
				return;//If for some reason the ChildNum was not found
			}
			gridChildrenAbsent.SetSelected(idx);
		}

		private void FillGridSpecified(Grid grid) {
			//Remember which child or employee was selected after the fill grid
			ChildRoomLog childRoomLogSelected=grid.SelectedTag<ChildRoomLog>();
			List<Child> listChildren=Children.GetAll();
			double countChildren=0;
			double countEmployees=0;
			int countUnderTwo=0;
			long childRoomNum=long.Parse(grid.Tag.ToString());
			//Get room logs for today for a specific room
			List<ChildRoomLog> listChildRoomLogsToday=ChildRoomLogs.GetChildRoomLogsForDate(DateTime.Now.Date);
			List<long> listEmployeeNumsUnique=listChildRoomLogsToday.FindAll(x => x.EmployeeNum!=0).Select(y => y.EmployeeNum).Distinct().ToList();
			//List of logs for people currently in the room
			List<ChildRoomLog> listChildRoomLogs=new List<ChildRoomLog>();
			//Find the employees in this room
			for(int i=0;i<listEmployeeNumsUnique.Count;i++) {
				//Find the most recent log for the given employee ordering by DateTDisplayed
				ChildRoomLog childRoomLogEmployee=listChildRoomLogsToday.FindAll(x => x.EmployeeNum==listEmployeeNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).First();
				if(childRoomLogEmployee.ChildRoomNum!=childRoomNum) {
					continue;//Employee is not present in this room
				}
				listChildRoomLogs.Add(childRoomLogEmployee);
			}
			List<long> listChildNumsUnique=listChildRoomLogsToday.FindAll(x => x.ChildNum!=0).Select(y => y.ChildNum).Distinct().ToList();
			//Find the children in this room
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				//Find the most recent log for the given child ordering by DateTDisplayed
				ChildRoomLog childRoomLogChild=listChildRoomLogsToday.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).First();
				if(childRoomLogChild.ChildRoomNum!=childRoomNum) {
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
			//First row in the grid is always notes
			ChildRoom childRoom=ChildRooms.GetOne(long.Parse(grid.Tag.ToString()));
			GridRow gridRowNotes=new GridRow();
			gridRowNotes.Cells.Add("Notes: "+childRoom.Notes);
			ChildRoomLog childRoomLogFirst=new ChildRoomLog();//First row needs a tag for reselecting row
			childRoomLogFirst.RatioChange=-1;//Differentiate from final row
			gridRowNotes.Tag=childRoomLogFirst;
			grid.ListGridRows.Add(gridRowNotes);
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				GridRow gridRow=new GridRow();
				GridCell gridCell=new GridCell();
				if(listChildRoomLogs[i].ChildNum!=0) {//Child entry
					Child child=listChildren.Find(x => x.ChildNum==listChildRoomLogs[i].ChildNum);
					gridCell.Text=Children.GetName(child);
					string nameAndAge=Children.GetName(child);
					int age=DateTime.Today.Year-child.BirthDate.Year;
					if(age==0) {
						nameAndAge+=", <1y";
					}
					else {
						nameAndAge+=", "+age+"y";
					}
					gridCell.Text=nameAndAge;
					countChildren++;
					if(DateTime.Now.AddYears(-2)<child.BirthDate) {
						countUnderTwo++;
					}
				}
				else {//Employee/teacher entry
					Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==listChildRoomLogs[i].EmployeeNum);
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
			if(grid.ListGridRows.Count>1) {//Empty grids do not get a ratio. There is always a notes row
				//Final row is the child to teacher ratio
				GridRow gridRowFinal=new GridRow();
				if(childRoom.Ratio==-1) {//-1 indicates a mixed ratio
					int teachersRequired=ChildRoomLogs.GetNumberTeachersMixed(totalChildren:(int)countChildren,childrenUnderTwo:countUnderTwo);
					gridRowFinal.Cells.Add("Ratio: Mixed, Children: "+countChildren+", <2: "+countUnderTwo+", Teachers: "+countEmployees+" of "+teachersRequired);//Example: Ratio:Mixed, Kids:2, Under2:1, Teachers:1 of 1
				}
				else {
					if(childRoom.Ratio==0) {//Stop division by 0
						gridRowFinal.Cells.Add("Ratio: "+childRoom.Ratio+", Children: "+countChildren+", Teachers: "+countEmployees+" of 0");
					}
					else {
						double teachersRequired=Math.Ceiling(countChildren/childRoom.Ratio);
						gridRowFinal.Cells.Add("Ratio: "+childRoom.Ratio+", Children: "+countChildren+", Teachers: "+countEmployees+" of "+teachersRequired);
					}
				}
				ChildRoomLog childRoomLogFinal=new ChildRoomLog();//Final row needs a tag for reselecting row
				gridRowFinal.Tag=childRoomLogFinal;
				grid.ListGridRows.Add(gridRowFinal);
			}
			grid.EndUpdate();
			//Reselect the child after the fill grid
			if(childRoomLogSelected==null) {
				return;//No row was selected
			}
			int idx=0;
			if(childRoomLogSelected.RatioChange==-1) {//Notes row
				idx=0;//Always first
			}
			else if(childRoomLogSelected.ChildNum!=0) {//Child row
				idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).ChildNum==childRoomLogSelected.ChildNum);
			}
			else if(childRoomLogSelected.EmployeeNum!=0) {//Employee row
				idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).EmployeeNum==childRoomLogSelected.EmployeeNum);
			}
			else {//Ratio
				idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).ChildNum==0
				&& ((ChildRoomLog)x.Tag).EmployeeNum==0
				&& ((ChildRoomLog)x.Tag).RatioChange==0);
			}
			if(idx==-1) {
				return;//If for some reason the previosly selected row was not found
			}
			grid.SetSelected(idx);
		}

		///<summary>Used for the 8 child grids. Fills _gridChildRoomClick with whatever grid had it's context menu opened.</summary>
		private void ContextMenu_Opened(object sender,EventArgs e) {
			ContextMenu contextMenu=(ContextMenu)sender;
			Grid grid=(Grid)contextMenu.PlacementTarget;
			_childRoomNumClicked=long.Parse(grid.Tag.ToString());
			_gridChildRoomClick=grid;
		}

		///<summary>Instead of setting the menu item text to something generic like "Send to Default Room", we set it here so it can be dynamic for each child's primary room such as "Send to Preschool 1".</summary>
		private void ContextMenuAbsent_Opened(object sender,EventArgs e) {
			_contextMenuAbsent.Items.Clear();//Clear out any items from the last right click
			ContextMenu contextMenu=(ContextMenu)sender;
			Grid grid=(Grid)contextMenu.PlacementTarget;
			Child childSelected=grid.SelectedTag<Child>();
			if(childSelected==null) {
				return;
			}
			string roomId=ChildRooms.GetRoomId(childSelected.ChildRoomNumPrimary);//DB gets hit here
			if(string.IsNullOrEmpty(roomId)) {
				_contextMenuAbsent.Add(new MenuItem("Send to ?",menuItemSendToPrimary_Click));
				return;
			}
			_contextMenuAbsent.Add(new MenuItem("Send to "+roomId,menuItemSendToPrimary_Click));
		}

		///<summary>Create single log entry where the ChildRoomNum is 0 to indicate they are unassigned.</summary>
		private void menuItemUnassign_Click(object sender,EventArgs e) {
			int idxSelected=_gridChildRoomClick.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a row first.");
				return;
			}
			ChildRoomLog childRoomLogSelected=(ChildRoomLog)_gridChildRoomClick.ListGridRows[idxSelected].Tag;
			if(childRoomLogSelected.ChildNum==0 && childRoomLogSelected.EmployeeNum==0) {//Final row has a tag with default values
				MsgBox.Show("This row cannot be removed");
				return;
			}
			ChildRoomLog childRoomLog=new ChildRoomLog();
			childRoomLog.DateTEntered=DateTime.Now;
			childRoomLog.DateTDisplayed=DateTime.Now;
			childRoomLog.ChildRoomNum=0;//Set to 0 to indicate unassigned
			//Either the ChildNum or EmployeeNum will be a non 0 value, not both
			childRoomLog.ChildNum=childRoomLogSelected.ChildNum;
			childRoomLog.EmployeeNum=childRoomLogSelected.EmployeeNum;
			ChildRoomLogs.Insert(childRoomLog);
			//Refresh
			FillGridSpecified(_gridChildRoomClick);
			FillGridChildrenAbsent();
			FillGridTeachersUnassigned();
			//Do selection after the child or teacher has been moved
			int idx=0;
			if(childRoomLog.ChildNum!=0) {//Child was moved
				idx=gridChildrenAbsent.ListGridRows.FindIndex(x => ((Child)x.Tag).ChildNum==childRoomLog.ChildNum);
				if(idx==-1) {
					return;//In case the ChildNum was not found
				}
				gridChildrenAbsent.SetSelected(idx);
				return;
			}
			//Teacher was moved
			idx=gridTeachersUnassigned.ListGridRows.FindIndex(x => ((Employee)x.Tag).EmployeeNum==childRoomLog.EmployeeNum);
			if(idx==-1) {
				return;//In case the EmployeeNum was not found
			}
			gridTeachersUnassigned.SetSelected(idx);
		}

		private void menuItemClockOut_Click(object sender,EventArgs e) {
			ClockOut(_gridChildRoomClick,TimeClockStatus.Home);
		}

		private void menuItemClockOutUnassigned_Click(object sender,EventArgs e) {
			ClockOut(gridTeachersUnassigned,TimeClockStatus.Home);
		}

		private void menuItemBreak_Click(object sender,EventArgs e) {
			ClockOut(_gridChildRoomClick,TimeClockStatus.Break);
		}

		private void ClockOut(Grid grid,TimeClockStatus timeClockStatus) {
			int idxSelected=grid.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a row first.");
				return;
			}
			Employee employee;
			if(grid==gridTeachersUnassigned){
				employee=(Employee)gridTeachersUnassigned.ListGridRows[idxSelected].Tag;
			}
			else{
				ChildRoomLog childRoomLogSelected=(ChildRoomLog)grid.ListGridRows[idxSelected].Tag;
				if(childRoomLogSelected.EmployeeNum==0) {
					MsgBox.Show("Only a teacher can be clocked out or on break.");
					return;
				}
				employee=Employees.GetEmpFromDB(childRoomLogSelected.EmployeeNum);
			}
			if(employee.ClockStatus==timeClockStatus.GetDescription()) {
				MsgBox.Show("This teacher is already clocked out or on break.");
				return;
			}
			//Begin clock out
			OpenDental.UI.ProgressWin progressWin=new OpenDental.UI.ProgressWin();
			progressWin.ShowCancelButton=false;
			progressWin.ActionMain=() => {
				ClockEvents.ClockOut(employee.EmployeeNum,timeClockStatus);
				Thread.Sleep(200);//Wait briefly so that if they quickly clock out again, the timestamps will be far enough apart.
			};
			progressWin.StartingMessage="Processing clock event...";
			try {
				progressWin.ShowDialog();
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				return;
			}
			Employee employeeOld=employee.Copy();
			employee.ClockStatus=timeClockStatus.GetDescription();;
			Employees.UpdateChanged(employee,employeeOld,doInvalidate:true);
			Employees.RefreshCache();//So fill grid can see updated status
			//Create a log to show the employee going to the unassigned grid
			ChildRoomLog childRoomLog=new ChildRoomLog();
			childRoomLog.DateTEntered=DateTime.Now;
			childRoomLog.DateTDisplayed=DateTime.Now;
			childRoomLog.EmployeeNum=employee.EmployeeNum;
			childRoomLog.ChildRoomNum=0;
			ChildRoomLogs.Insert(childRoomLog);
			//Refresh
			if(grid!=gridTeachersUnassigned){
				FillGridSpecified(grid);
			}
			FillGridTeachersUnassigned();
			Signalods.SetInvalid(InvalidType.Children);
			//Reselect teacher after they have been moved
			int idx=gridTeachersUnassigned.ListGridRows.FindIndex(x => ((Employee)x.Tag).EmployeeNum==childRoomLog.EmployeeNum);
			if(idx==-1) {
				return;//In case employee was not found
			}
			gridTeachersUnassigned.SetSelected(idx);
		}

		private void menuItemClockIn_Click(object sender,EventArgs e) {
			//Find employee
			int idxSelected=gridTeachersUnassigned.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a row first.");
				return;
			}
			long employeeNum=((Employee)gridTeachersUnassigned.ListGridRows[idxSelected].Tag).EmployeeNum;
			Employee employee=Employees.GetEmpFromDB(employeeNum);
			if(employee.ClockStatus=="Working") {
				MsgBox.Show("This teacher is already clocked in.");
				return;
			}
			//Begin clock in
			OpenDental.UI.ProgressWin progressWin=new OpenDental.UI.ProgressWin();
			progressWin.ShowCancelButton=false;
			progressWin.ActionMain=() => {
				ClockEvents.ClockIn(employee.EmployeeNum,isAtHome:false);
				Thread.Sleep(1000);//Wait one second so that if they quickly clock out again, the timestamps will be far enough apart.
			};
			progressWin.StartingMessage="Processing clock event...";
			try {
				progressWin.ShowDialog();
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				return;
			}
			Employee employeeOld=employee.Copy();
			employee.ClockStatus="Working";
			Employees.UpdateChanged(employee,employeeOld,doInvalidate:true);
			Employees.RefreshCache();//So fill grid can see updated status
			FillGridTeachersUnassigned();
			Signalods.SetInvalid(InvalidType.Children);
		}

		private void GridChildRoom_CellDoubleClick(object sender,GridClickEventArgs e) {
			Grid grid=(Grid)sender;
			int idxSelected=grid.GetSelectedIndex();
			if(idxSelected!=0) {
				return;//If the selected row is not the notes row, then kick out
			}
			FrmChildRoomEdit frmChildRoomEdit=new FrmChildRoomEdit();
			long childRoomNum=(long)(int)grid.Tag;
			frmChildRoomEdit.ChildRoomCur=ChildRooms.GetOne(childRoomNum);
			frmChildRoomEdit.ShowDialog();
			if(frmChildRoomEdit.IsDialogOK) {
				FillGridSpecified(grid);
			}
		}

		private void menuItemSendToPrimary_Click(object sender,EventArgs e) {
			int idxSelected=gridChildrenAbsent.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a child first.");
				return;
			}
			Child childSelected=(Child)gridChildrenAbsent.ListGridRows[idxSelected].Tag;
			if(childSelected.ChildRoomNumPrimary==0) {
				MsgBox.Show("The selected child does not have a primary room.");
				return;
			}
			//Create coming to log for their primary room
			ChildRoomLog childRoomLog=new ChildRoomLog();
			childRoomLog.ChildNum=childSelected.ChildNum;
			childRoomLog.DateTDisplayed=DateTime.Now;
			childRoomLog.DateTEntered=DateTime.Now;
			childRoomLog.ChildRoomNum=childSelected.ChildRoomNumPrimary;
			ChildRoomLogs.Insert(childRoomLog);
			//Sync and refresh
			Signalods.SetInvalid(InvalidType.Children);
			FillAllGrids();
			//Do selection after the child has been moved
			Grid grid=GetChildRoomGrid(childSelected.ChildRoomNumPrimary);
			int idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).ChildNum==childSelected.ChildNum);
			if(idx==-1) {
				return;//If for some reason the ChildNum was not found
			}
			grid.SetSelected(idx);
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
			//Check if the selected child is already in this room
			Child child=Children.GetOne(frmChildren.ChildNumSelected);
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForChild(child.ChildNum,DateTime.Now.Date);
			ChildRoomLog childRoomLogNewest=listChildRoomLogs.OrderByDescending(x => x.DateTDisplayed).FirstOrDefault();
			if(childRoomLogNewest!=null) {
				if(childRoomLogNewest.ChildRoomNum==childRoomNum) {
					MsgBox.Show("The selected child is already in this room.");
					return;//Kick out if they are already in the selected room
				}
			}
			//Log the child coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.ChildNum=child.ChildNum;
			childRoomLogNew.DateTEntered=DateTime.Now;
			childRoomLogNew.DateTDisplayed=DateTime.Now;
			childRoomLogNew.ChildRoomNum=childRoomNum;
			ChildRoomLogs.Insert(childRoomLogNew);
			Signalods.SetInvalid(InvalidType.Children);
			//Refresh
			FillAllGrids();
			//Do selection after the child has been moved
			Grid grid=GetChildRoomGrid(childRoomNum);
			int idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).ChildNum==child.ChildNum);
			if(idx==-1) {
				return;//If for some reason the ChildNum was not found
			}
			grid.SetSelected(idx);
		}

		private void butAddTeacher_Grid_Click(object sender,EventArgs e) {
			//Determine which room the click was for
			long childRoomNum=(int)((Button)sender).Tag;
			//Select an employee/teacher
			FrmChildTeacherSelect frmChildTeacherSelect=new FrmChildTeacherSelect();
			frmChildTeacherSelect.ShowDialog();
			if(frmChildTeacherSelect.IsDialogCancel) {
				return;//Kick out if no employee/teacher was selected
			}
			//Check if the selected employee is already in this room
			Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==frmChildTeacherSelect.EmployeeNumSelected);
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForEmployee(employee.EmployeeNum,DateTime.Now.Date);
			ChildRoomLog childRoomLogNewest=listChildRoomLogs.OrderByDescending(x => x.DateTDisplayed).FirstOrDefault();
			if(childRoomLogNewest!=null) {
				if(childRoomLogNewest.ChildRoomNum==childRoomNum) {
					MsgBox.Show("The selected teacher is already in this room.");
					return;//Kick out if they are already in the selected room
				}
			}
			//Log the teacher coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.EmployeeNum=employee.EmployeeNum;
			childRoomLogNew.DateTEntered=DateTime.Now;
			childRoomLogNew.DateTDisplayed=DateTime.Now;
			childRoomLogNew.ChildRoomNum=childRoomNum;
			ChildRoomLogs.Insert(childRoomLogNew);
			Signalods.SetInvalid(InvalidType.Children);
			//Refresh
			FillAllGrids();
			//Do selection after the teacher has been moved
			Grid grid=GetChildRoomGrid(childRoomNum);
			int idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).EmployeeNum==employee.EmployeeNum);
			if(idx==-1) {
				return;//If for some reason the EmployeeNum was not found
			}
			grid.SetSelected(idx);
			//Clock in employee
			if(employee.ClockStatus=="Working") {
				return;//Already clocked in
			}
			OpenDental.UI.ProgressWin progressWin=new OpenDental.UI.ProgressWin();
			progressWin.ShowCancelButton=false;
			progressWin.ActionMain=() => {
				ClockEvents.ClockIn(employee.EmployeeNum,isAtHome:false);
				Thread.Sleep(1000);//Wait one second so that if they quickly clock out again, the timestamps will be far enough apart.
			};
			progressWin.StartingMessage="Processing clock event...";
			try {
				progressWin.ShowDialog();
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
				return;
			}
			Employee employeeOld=employee.Copy();
			employee.ClockStatus="Working";
			Employees.UpdateChanged(employee,employeeOld,doInvalidate:true);
			Employees.RefreshCache();
			Signalods.SetInvalid(InvalidType.Children);
		}

		private void butPrint_Grid_Click(object sender,EventArgs e) {
			long childRoomNum=(int)((Button)sender).Tag;
			Grid grid=GetChildRoomGrid(childRoomNum);
			ChildRoom childRoom=ChildRooms.GetOne((int)grid.Tag);
			_textToPrint+=childRoom.RoomId+"\r\n";
			_textToPrint+=DateTime.Now.ToString()+"\r\n";
			for(int i=0;i<grid.ListGridRows.Count;i++) {
				_textToPrint+=grid.ListGridRows[i].Cells[0].Text+"\r\n";
			}
			_pagesPrinted=0;
			_idxCharsPrinted=0;
			Printout printout=new Printout();
			printout.FuncPrintPage=pd_PrintPage;
			printout.thicknessMarginInches=new Thickness(0.5,0.4,0.25,0.4);
			WpfControls.PrinterL.TryPrintOrDebugClassicPreview(printout);
			_pagesPrinted=0;
			_idxCharsPrinted=0;
			_textToPrint="";
		}

		private bool pd_PrintPage(Graphics g) {
			Font font=new Font();
			double yPos=0;
			string text="Page "+(_pagesPrinted+1);//Page number heading
			Size sizeText=g.MeasureString(text,font);
			g.DrawString(text,font,Colors.Black,g.Width-sizeText.Width,yPos);
			yPos+=sizeText.Height+8;
			font=new Font();
			font.Size=12;
			if(_pagesPrinted==0) {
				text=_textToPrint;
			}
			else {
				text=_textToPrint.Substring(_idxCharsPrinted);
			}
			Size sizeAvail=new Size(g.Width,g.Height-yPos);
			int charactersFitted=g.MeasureCharactersFitted(text,font,sizeAvail);
			if(charactersFitted==0) {
				_pagesPrinted++;
				return false;//no more pages
			}
			Rect rect=new Rect(new Point(0,yPos),sizeAvail);
			g.DrawString(text.Substring(0,charactersFitted),font,Colors.Black,rect);
			if(charactersFitted==text.Length) {
				_pagesPrinted++;
				return false;//no more pages
			}
			_idxCharsPrinted=_idxCharsPrinted+charactersFitted;
			_pagesPrinted++;
			return true;//has more pages
		}

		///<summary>Returns the childroom grid based on the childRoomNum passed in. Will return null if the passed in num does not match a grid.</summary>
		/// <param name="childRoomNum"></param>
		/// <returns></returns>
		private Grid GetChildRoomGrid(long childRoomNum) {
			Grid grid;
			switch (childRoomNum) {
				case 1:
					grid=gridChildRoom1;
					break;
				case 2:
					grid=gridChildRoom2;
					break;
				case 3:
					grid=gridChildRoom3;
					break;
				case 4:
					grid=gridChildRoom4;
					break;
				case 5:
					grid=gridChildRoom5;
					break;
				case 6:
					grid=gridChildRoom6;
					break;
				case 7:
					grid=gridChildRoom7;
					break;
				case 8:
					grid=gridChildRoom8;
					break;
				default:
					grid=null;
					break;
			}
			return grid;
		}

		private void butViewOnly_Click(object sender,EventArgs e) {
			SetViewOnly();
		}

		///<summary>Set to view only for maps parents will be able to see but not interact with.</summary>
		private void SetViewOnly() {
			//Disable buttons
			butChildren.Visible=false;
			butClassrooms.Visible=false;
			butParents.Visible=false;
			butViewOnly.Visible=false;
			//Disable grid buttons
			butViewLogs_Grid1.Visible=false;
			butAddChild_Grid1.Visible=false;
			butAddTeacher_Grid1.Visible=false;
			butPrint_Grid1.Visible=false;
			butViewLogs_Grid2.Visible=false;
			butAddChild_Grid2.Visible=false;
			butAddTeacher_Grid2.Visible=false;
			butPrint_Grid2.Visible=false;
			butViewLogs_Grid3.Visible=false;
			butAddChild_Grid3.Visible=false;
			butAddTeacher_Grid3.Visible=false;
			butPrint_Grid3.Visible=false;
			butViewLogs_Grid4.Visible=false;
			butAddChild_Grid4.Visible=false;
			butAddTeacher_Grid4.Visible=false;
			butPrint_Grid4.Visible=false;
			butViewLogs_Grid5.Visible=false;
			butAddChild_Grid5.Visible=false;
			butAddTeacher_Grid5.Visible=false;
			butPrint_Grid5.Visible=false;
			butViewLogs_Grid6.Visible=false;
			butAddChild_Grid6.Visible=false;
			butAddTeacher_Grid6.Visible=false;
			butPrint_Grid6.Visible=false;
			butViewLogs_Grid7.Visible=false;
			butAddChild_Grid7.Visible=false;
			butAddTeacher_Grid7.Visible=false;
			butPrint_Grid7.Visible=false;
			butViewLogs_Grid8.Visible=false;
			butAddChild_Grid8.Visible=false;
			butAddTeacher_Grid8.Visible=false;
			butPrint_Grid8.Visible=false;
			//Disable context menus
			gridTeachersUnassigned.ContextMenuShows=false;
			gridChildrenAbsent.ContextMenuShows=false;
			gridChildRoom1.ContextMenuShows=false;
			gridChildRoom2.ContextMenuShows=false;
			gridChildRoom3.ContextMenuShows=false;
			gridChildRoom4.ContextMenuShows=false;
			gridChildRoom5.ContextMenuShows=false;
			gridChildRoom6.ContextMenuShows=false;
			gridChildRoom7.ContextMenuShows=false;
			gridChildRoom8.ContextMenuShows=false;

		}
	}
}