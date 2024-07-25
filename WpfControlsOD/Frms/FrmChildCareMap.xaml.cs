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
			//There was a bug that occured when a child's two most recent entries were at the same time. When one entry was coming to and the other was leaving, the child ended up showing in both the absent grid and a classroom grid. To fix this issue, we add the childNum to the present list if either is an IsComing entry. This is due to the way changing rooms works using right click where a log leaving and a log coming to are created. In case these are at the same time, the coming to entry would be the correct one as that is the room they are currently in.
			//Stores the list of childNums that are present
			List<long> listChildNumsPresent=new List<long>();
			List<long> listChildNumsUnique=listChildRoomLogs.Select(x => x.ChildNum).Distinct().ToList();
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				List<ChildRoomLog> listChildRoomLogsChild=listChildRoomLogs.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).ToList();//Order so most recent entries are first
				//If there is only one entry or the two most recent entries do not have the same time, then we can use the first entry
				if(listChildRoomLogsChild.Count==1 || listChildRoomLogsChild[0].DateTDisplayed!=listChildRoomLogsChild[1].DateTDisplayed) {
					if(listChildRoomLogsChild[0].IsComing) {
						listChildNumsPresent.Add(listChildNumsUnique[i]);//Child is present so add childNum to the list
					}
					continue;
				}
				//The top two entries have the same time so see if either has IsComing true
				if(listChildRoomLogsChild[0].IsComing || listChildRoomLogsChild[1].IsComing) {
					listChildNumsPresent.Add(listChildNumsUnique[i]);
				}
				//If for some reason they are both leaving entries, then this childNum is not added to the present list
			}
			List<Child> listChildrenAbsent=listChildren.FindAll(x => !listChildNumsPresent.Any(y => y==x.ChildNum)).ToList();
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
			//List of logs for people currently in the room
			List<ChildRoomLog> listChildRoomLogs=new List<ChildRoomLog>();
			//Get room logs for today
			List<ChildRoomLog> listChildRoomLogsToday=ChildRoomLogs.GetChildRoomLogs(long.Parse(grid.Tag.ToString()),DateTime.Now.Date);
			List<long> listEmployeeNumsUnique=listChildRoomLogsToday.FindAll(x => x.EmployeeNum!=0).Select(y => y.EmployeeNum).Distinct().ToList();
			for(int i=0;i<listEmployeeNumsUnique.Count;i++) {
				List<ChildRoomLog> listChildRoomLogsEmployee=listChildRoomLogsToday.FindAll(x => x.EmployeeNum==listEmployeeNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).ToList();//Order so the most recent entries are first
				//If there is only one entry or the two most recent entries do not have the same time, then we can use the first entry
				if(listChildRoomLogsEmployee.Count==1 || listChildRoomLogsEmployee[0].DateTDisplayed!=listChildRoomLogsEmployee[1].DateTDisplayed) {
					if(listChildRoomLogsEmployee[0].IsComing) {
						listChildRoomLogs.Add(listChildRoomLogsEmployee[0]);
					}
					continue;
				}
				//The top two entries have the same time so keep the entry where they are entering a room
				if(listChildRoomLogsEmployee[0].IsComing==true) {
					listChildRoomLogs.Add(listChildRoomLogsEmployee[0]);
				}
				else if(listChildRoomLogsEmployee[1].IsComing==true){
					listChildRoomLogs.Add(listChildRoomLogsEmployee[1]);
				}
				//If for some reason they are both leaving entries, then this employee is not added to the list
			}
			List<long> listChildNumsUnique=listChildRoomLogsToday.FindAll(x => x.ChildNum!=0).Select(y => y.ChildNum).Distinct().ToList();
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				List<ChildRoomLog> listChildRoomLogsChild=listChildRoomLogsToday.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).ToList();//Order so the most recent entries are first
				//If there is only one entry or the two most recent entries do not have the same time, then we can use the first entry
				if(listChildRoomLogsChild.Count==1 || listChildRoomLogsChild[0].DateTDisplayed!=listChildRoomLogsChild[1].DateTDisplayed) {
					if(listChildRoomLogsChild[0].IsComing) {
						listChildRoomLogs.Add(listChildRoomLogsChild[0]);
					}
					continue;
				}
				//The top two entries have the same time so keep the entry where they are entering a room
				if(listChildRoomLogsChild[0].IsComing) {
					listChildRoomLogs.Add(listChildRoomLogsChild[0]);
				}
				else if(listChildRoomLogsChild[1].IsComing) {
					listChildRoomLogs.Add(listChildRoomLogsChild[1]);
				}
				//If for some reason they are both leaving entries, then this child is not added to the list
			}
			//Ratio change entries do not need to be removed because they will never be added to the list
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
				//OrderBy IsComing as well in case there are two entries with the same time
				ChildRoomLog childRoomLog=listChildRoomLogs.OrderByDescending(x => x.DateTDisplayed).ThenByDescending(y => y.IsComing).First();//in case there are two with exactly same time
				if(childRoomLog.ChildRoomNum==_childRoomNumClicked && childRoomLog.IsComing) {
					MsgBox.Show("The selected child is already in this room.");
					return;//Kick out if they are already in the selected room
				}
				if(childRoomLog.IsComing) {
					CreateChildRoomLogLeaving(childRoomLog);
				}
			}
			//Log the child coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.ChildNum=child.ChildNum;
			//Add one second so there is a difference between the time leaving the old room and entering the new room
			childRoomLogNew.DateTEntered=DateTime.Now.AddSeconds(1);
			childRoomLogNew.DateTDisplayed=DateTime.Now.AddSeconds(1);
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
				//OrderBy IsComing as well in case there are two entries with the same time
				ChildRoomLog childRoomLog=listChildRoomLogs.OrderByDescending(x => x.DateTDisplayed).ThenByDescending(y => y.IsComing).First();
				if(childRoomLog.ChildRoomNum==_childRoomNumClicked && childRoomLog.IsComing) {
					MsgBox.Show("The selected teacher is already in this room.");
					return;//Kick out if they are already in the selected room
				}
				if(childRoomLog.IsComing) {
					CreateChildRoomLogLeaving(childRoomLog);
				}
			}
			//Log the teacher coming to the selected room
			ChildRoomLog childRoomLogNew=new ChildRoomLog();
			childRoomLogNew.EmployeeNum=employee.EmployeeNum;
			//Add one second so there is a difference between the time leaving the old room and entering the new room
			childRoomLogNew.DateTEntered=DateTime.Now.AddSeconds(1);
			childRoomLogNew.DateTDisplayed=DateTime.Now.AddSeconds(1);
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
			childRoomLogGoing.DateTEntered=DateTime.Now;
			childRoomLogGoing.DateTDisplayed=DateTime.Now;
			childRoomLogGoing.ChildRoomNum=childRoomLogOld.ChildRoomNum;
			childRoomLogGoing.IsComing=false;
			if(childRoomLogOld.EmployeeNum!=0) {//Employee/teacher entry
				childRoomLogGoing.EmployeeNum=childRoomLogOld.EmployeeNum;
			}
			else {//Child entry
				childRoomLogGoing.ChildNum=childRoomLogOld.ChildNum;
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