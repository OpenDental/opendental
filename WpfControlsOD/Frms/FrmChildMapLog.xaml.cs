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
	public partial class FrmChildMapLog:FrmODBase {
		///<summary>The date and time initially set when opening the frm. Gets assigned by parent. Default to now.</summary>
		public DateTime DateTimeInitial=DateTime.Now;
		///<summary>The room initially set when opening the frm. Gets assigned by parent. Default to childroom 1.</summary>
		public long ChildRoomNumInitial=1;

		///<summary></summary>
		public FrmChildMapLog() {
			InitializeComponent();
			Load+=FrmChildMapLog_Load;
			comboClassroom.SelectionChangeCommitted+=ComboClassroom_SelectionChangeCommitted;
		}

		private void FrmChildMapLog_Load(object sender, EventArgs e) {
			textVDate.Value=DateTimeInitial.Date;
			textVTime.Text=DateTimeInitial.ToLongTimeString();
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			comboClassroom.Items.AddList(listChildRooms,x => x.RoomId);
			comboClassroom.SetSelectedKey<ChildRoom>(ChildRoomNumInitial,x=>x.ChildRoomNum);
			FillGrid();
		}

		private void FillGrid(){
			if(!textVDate.IsValid()) {
				return;
			}
			if(!textVTime.IsValid()) {
				return;
			}
			long childRoomNum=comboClassroom.GetSelectedKey<ChildRoom>(x=>x.ChildRoomNum);
			gridMain.Title=ChildRooms.GetRoomId(childRoomNum);
			//Get all logs for the entered date and childRoomNum
			List<ChildRoomLog> listChildRoomLogsForDate=ChildRoomLogs.GetChildRoomLogs(childRoomNum,textVDate.Value);
			//Only keep logs less than/equal to the entered time
			DateTime dateTime=textVDate.Value+DateTime.Parse(textVTime.Text).TimeOfDay;//Combine so the date is correct for filter
			List<ChildRoomLog> listChildRoomLogsFiltered=listChildRoomLogsForDate.FindAll(x => x.DateTDisplayed<=dateTime);
			//Find teachers and children present in this room using the same logic in FrmChildCareMap
			List<ChildRoomLog> listChildRoomLogs=new List<ChildRoomLog>();
			double countEmployees=0;
			double countChildren=0;
			List<long> listEmployeeNumsUnique=listChildRoomLogsFiltered.FindAll(x => x.EmployeeNum!=0)
				.Select(y => y.EmployeeNum).Distinct().ToList();
			for(int i=0;i<listEmployeeNumsUnique.Count;i++) {
				ChildRoomLog childRoomLog=listChildRoomLogsFiltered.FindAll(x => x.EmployeeNum==listEmployeeNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).First();
				if(childRoomLog.ChildRoomNum==0) {
					continue;//Employee is not present in the room
				}
				listChildRoomLogs.Add(childRoomLog);
			}
			List<long> listChildNumsUnique=listChildRoomLogsFiltered.FindAll(x => x.ChildNum!=0).Select(y => y.ChildNum).Distinct().ToList();
			for(int i=0;i<listChildNumsUnique.Count;i++) {
				ChildRoomLog childRoomLog=listChildRoomLogsFiltered.FindAll(x => x.ChildNum==listChildNumsUnique[i])
					.OrderByDescending(y => y.DateTDisplayed).First();
				if(childRoomLog.ChildRoomNum==0) {
					continue;//Child is not present in this room
				}
				listChildRoomLogs.Add(childRoomLog);
			}
			//Begin to fill the grid
			List<Child> listChildren=Children.GetAll();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Name",100);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
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
					gridCell.Text=employee.FName+" "+employee.LName;
					gridCell.ColorBackG=Color.FromRgb(255,240,240);
					countEmployees++;
				}
				gridRow.Cells.Add(gridCell);
				gridRow.Tag=listChildRoomLogs[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			if(gridMain.ListGridRows.Count==0) {//Empty grids do not get a ratio
				gridMain.EndUpdate();
				return;
			}
			//Final row is the child to teacher ratio
			GridRow gridRowFinal=new GridRow();
			if(countEmployees==0) {
				gridRowFinal.Cells.Add("Current Ratio: ?");
			}
			else {
				gridRowFinal.Cells.Add(("Current Ratio: ")+(countChildren/countEmployees).ToString());
			}
			gridMain.ListGridRows.Add(gridRowFinal);
			gridMain.EndUpdate();
		}

		private void ComboClassroom_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void buttonRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}
	}
}