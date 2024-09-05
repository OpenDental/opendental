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
			_contextMenu.Add(new MenuItem("Remove",menuItemRemove_Click));
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
			listEmployeesSorted.RemoveAll(x => x.IsHidden);//Remove all hidden before the fillgrid
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
			listChildrenSorted.RemoveAll(x => x.IsHidden);//Remove all hidden before the fillgrid
			//Begin to fill the grid
			gridChildrenAbsent.BeginUpdate();
			gridChildrenAbsent.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			gridChildrenAbsent.Columns.Add(gridColumn);
			gridChildrenAbsent.ListGridRows.Clear();
			for(int i=0;i<listChildrenSorted.Count;i++) {
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(Children.GetName(listChildrenSorted[i]));
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
					gridRowFinal.Cells.Add("Ratio: Mixed, Children: "+countChildren+", Under2: "+countUnderTwo+", Teachers: "+countEmployees+"/"+teachersRequired);//Example: Ratio:Mixed, Kids:2, Under2:1, Teachers:1/1
				}
				else {
					string ratio="";
					if(countEmployees==0) {//Stop division by 0
						ratio="?";
					}
					else {
						ratio=(countChildren/countEmployees).ToString();
					}
					if(childRoom.Ratio==0) {//Stop division by 0
						gridRowFinal.Cells.Add("Ratio: "+ratio+", Children: "+countChildren+", Teachers: "+countEmployees+"/?");
					}
					else {
						double teachersRequired=Math.Ceiling(countChildren/childRoom.Ratio);
						gridRowFinal.Cells.Add("Ratio: "+ratio+", Children: "+countChildren+", Teachers: "+countEmployees+"/"+teachersRequired);
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

		private void menuItemRemove_Click(object sender,EventArgs e) {
			int idxSelected=_gridChildRoomClick.GetSelectedIndex();
			if(idxSelected==-1) {
				MsgBox.Show("Select a row first.");
				return;
			}
			ChildRoomLog childRoomLog=(ChildRoomLog)_gridChildRoomClick.ListGridRows[idxSelected].Tag;
			if(childRoomLog.ChildNum==0 && childRoomLog.EmployeeNum==0) {//Final row has a tag with default values
				MsgBox.Show("This row cannot be removed");
				return;
			}
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
			//Do selection after the teacher has been moved
			Grid grid=GetChildRoomGrid(childRoomNum);
			int idx=grid.ListGridRows.FindIndex(x => ((ChildRoomLog)x.Tag).EmployeeNum==employee.EmployeeNum);
			if(idx==-1) {
				return;//If for some reason the EmployeeNum was not found
			}
			grid.SetSelected(idx);
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