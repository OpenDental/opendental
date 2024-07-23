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
		private ContextMenu _contextMenu;
		///<summary>Gets filled when a grid is right clicked on for the context menu options.</summary>
		private long _childRoomNumClicked;
		private Grid _gridChildRoomClick;

		///<summary></summary>
		public FrmChildCareMap() {
			InitializeComponent();
			Load+=FrmChildren_Load;
			_contextMenu=new ContextMenu(this);
			_contextMenu.Opened+=ContextMenu_Opened;
			_contextMenu.Add(new MenuItem("View Logs",menuItemViewLogs_Click));
			_contextMenu.Add(new MenuItem("Add Child",menuItemAddChild_Click));
			_contextMenu.Add(new MenuItem("Add Teacher",menuItemAddChildTeacher_Click));
			_contextMenu.Add(new MenuItem("Remove",menuItemRemove_Click));
			gridChildRoom1.ContextMenu=_contextMenu;
			gridChildRoom2.ContextMenu=_contextMenu;
			gridChildRoom3.ContextMenu=_contextMenu;
			gridChildRoom4.ContextMenu=_contextMenu;
			gridChildRoom5.ContextMenu=_contextMenu;
			gridChildRoom6.ContextMenu=_contextMenu;
			gridChildRoom7.ContextMenu=_contextMenu;
			gridChildRoom8.ContextMenu=_contextMenu;
		}

		private void FrmChildren_Load(object sender, EventArgs e) {
			StartMaximized=true;
			//TODO: If not a daycare worker, hide buttons
			//Tag each grid with a hardcoded value that represents a ChildRoomNum. Childrooms can be created and edited, but not deleted. If we end up with more than 8 childrooms, then more grids would need to be added to this frm.
			gridChildRoom1.Tag=1;
			gridChildRoom2.Tag=2;
			gridChildRoom3.Tag=3;
			gridChildRoom4.Tag=4;
			gridChildRoom5.Tag=5;
			gridChildRoom6.Tag=6;
			gridChildRoom7.Tag=7;
			gridChildRoom8.Tag=8;
			//Call all fillgrids
			FillAllGrids();
		}

		private void FillAllGrids() {
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

		private void FillGridChildrenAbsent() {
			List<Child> listChildren=Children.GetAll();
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllChildrenForDate(DateTime.Now.Date);
			//Use linq to get list of unique ChildNums
			//Loop through those childNums
			//   Get all the entries for that child
			//   If there are two with the exact same time, then what?
			//etc.
			//Find the most recent logs for each child with an entry today
			List<ChildRoomLog> listChildRoomLogsNewest=listChildRoomLogs.GroupBy(x => x.ChildNum)
				.Select(y => y.OrderByDescending(x => x.DateTDisplayed).First()).ToList();
			//Find the logs where the child is present
			List<ChildRoomLog> listChildRoomLogsPresent=listChildRoomLogsNewest.FindAll(x => x.IsComing);
			//Remove children from the list that are present
			List<Child> listChildrenAbsent=listChildren.FindAll(x => !listChildRoomLogsPresent.Any(y => y.ChildNum==x.ChildNum)).ToList();
			//Begin to fill the grid
			gridChildrenAbsent.BeginUpdate();
			gridChildrenAbsent.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			gridChildrenAbsent.Columns.Add(gridColumn);
			gridChildrenAbsent.ListGridRows.Clear();
			for(int i=0;i<listChildrenAbsent.Count;i++) {
				if(listChildrenAbsent[i].IsHidden) {
					continue;
				}
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(Children.GetName(listChildrenAbsent[i]));
				gridRow.Tag=listChildrenAbsent[i];
				gridChildrenAbsent.ListGridRows.Add(gridRow);
			}
			gridChildrenAbsent.EndUpdate();
		}

		private void FillGridSpecified(Grid grid) {
			List<Child> listChildren=Children.GetAll();
			double countChildren=0;
			double countEmployees=0;
			//Get room logs for today
			List<ChildRoomLog> listChildRoomLogsToday=ChildRoomLogs.GetChildRoomLogs(long.Parse(grid.Tag.ToString()),DateTime.Now.Date);
			//Use linq to get list of unique employeeNums
			//Loop through those employees
			//   Get all the entries for that employee
			//   If there are two with the exact same time, then what?
			//etc.
			//Only keep the most recent log for each child or teacher
			List<ChildRoomLog> listChildRoomLogsNewest=listChildRoomLogsToday
				.GroupBy(x => {
					if(x.ChildNum==0)
						return x.EmployeeNum;
					else
						return x.ChildNum;})
				.Select(y => y.OrderByDescending(x => x.DateTDisplayed).First())
				.OrderBy(z => z.ChildNum).ToList();//So that teachers appear first in the list
			//Remove ratio change entries and entries where the person is out
			List<ChildRoomLog> listChildRoomLogs=listChildRoomLogsNewest.FindAll(x => x.IsComing && x.RatioChange==0).ToList();
			//Begin to fill the grid
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			grid.Columns.Add(gridColumn);
			grid.ListGridRows.Clear();
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				GridRow gridRow=new GridRow();
				string name="";
				if(listChildRoomLogs[i].ChildNum!=0) {//Child entry
					Child child=listChildren.Find(x => x.ChildNum==listChildRoomLogs[i].ChildNum);
					name=Children.GetName(child);
					countChildren++;
				}
				else {//Employee/teacher entry
					Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==listChildRoomLogs[i].EmployeeNum);
					name=employee.FName+" "+employee.LName;
					countEmployees++;
				}
				gridRow.Cells.Add(name);
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
				gridRowFinal.Cells.Add("?");
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

		private void menuItemViewLogs_Click(object sender,EventArgs e) {
			FrmChildRoomLogs frmChildRoomLogs=new FrmChildRoomLogs();
			frmChildRoomLogs.ChildRoomNumInitial=_childRoomNumClicked;
			frmChildRoomLogs.ShowDialog();
		}

		private void menuItemAddChild_Click(object sender,EventArgs e) {
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
			if(listChildRoomLogs.Count!=0) {
				ChildRoomLog childRoomLog=listChildRoomLogs.OrderByDescending(x => x.DateTDisplayed).First();
				if(childRoomLog.ChildRoomNum==_childRoomNumClicked && childRoomLog.IsComing) {
					return;//Kick out if they are already in the selected room
				}
				if(childRoomLog.IsComing) {
					CreateChildRoomLogLeaving(childRoomLog);
				}
			}
			//Wait 1 second to avoid issue with logs being at the same time
			Thread.Sleep(1000);
			//Log the child coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.ChildNum=child.ChildNum;
			childRoomLogNew.DateTEntered=DateTime.Now;
			childRoomLogNew.DateTDisplayed=DateTime.Now;
			childRoomLogNew.ChildRoomNum=_childRoomNumClicked;
			childRoomLogNew.IsComing=true;
			ChildRoomLogs.Insert(childRoomLogNew);
			//Refresh
			FillAllGrids();
		}

		private void menuItemAddChildTeacher_Click(object sender,EventArgs e) {
			//Select an employee/teacher
			FrmChildTeacherSelect frmChildTeacherSelect=new FrmChildTeacherSelect();
			frmChildTeacherSelect.ShowDialog();
			if(frmChildTeacherSelect.IsDialogCancel) {
				return;//Kick out if no employee/teacher was selected
			}
			//If the selected employee is currently in a room, remove them using a going log
			Employee employee=Employees.GetFirstOrDefault(x => x.EmployeeNum==frmChildTeacherSelect.EmployeeNumSelected);
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetAllLogsForEmployee(employee.EmployeeNum,DateTime.Now.Date);
			if(listChildRoomLogs.Count!=0) {
				ChildRoomLog childRoomLog=listChildRoomLogs.OrderByDescending(x => x.DateTDisplayed).First();
				if(childRoomLog.ChildRoomNum==_childRoomNumClicked && childRoomLog.IsComing) {
					return;//Kick out if they are already in the selected room
				}
				if(childRoomLog.IsComing) {
					CreateChildRoomLogLeaving(childRoomLog);
				}
			}
			//Wait 1 second to avoid issues with logs being the same time
			//Thread.Sleep(1000);
			//Log the teacher coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.EmployeeNum=employee.EmployeeNum;
			childRoomLogNew.DateTEntered=DateTime.Now;
			childRoomLogNew.DateTDisplayed=DateTime.Now;
			childRoomLogNew.ChildRoomNum=_childRoomNumClicked;
			childRoomLogNew.IsComing=true;
			ChildRoomLogs.Insert(childRoomLogNew);
			//Refresh
			FillAllGrids();
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
			CreateChildRoomLogLeaving(childRoomLog);
			//Refresh
			FillGridSpecified(_gridChildRoomClick);
			FillGridChildrenAbsent();
		}

		///<summary>Creates a ChildRoomLog where IsComing=false based on the log passed in.</summary>
		private void CreateChildRoomLogLeaving(ChildRoomLog childRoomLogOld) {
			ChildRoomLog childRoomLogGoing=new ChildRoomLog();
			if(childRoomLogOld.EmployeeNum!=0) {//Employee/teacher entry
				childRoomLogGoing.EmployeeNum=childRoomLogOld.EmployeeNum;
				childRoomLogGoing.DateTEntered=DateTime.Now;
				childRoomLogGoing.DateTDisplayed=DateTime.Now;
				childRoomLogGoing.ChildRoomNum=childRoomLogOld.ChildRoomNum;
				childRoomLogGoing.IsComing=false;
			}
			else {//Child entry
				childRoomLogGoing.ChildNum=childRoomLogOld.ChildNum;
				childRoomLogGoing.DateTEntered=DateTime.Now;
				childRoomLogGoing.DateTDisplayed=DateTime.Now;
				childRoomLogGoing.ChildRoomNum=childRoomLogOld.ChildRoomNum;
				childRoomLogGoing.IsComing=false;
			}
			ChildRoomLogs.Insert(childRoomLogGoing);
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


	}
}