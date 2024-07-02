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
			List<ChildTeacher> listChildTeachers=ChildTeachers.GetAll();
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			double countChildren=0;
			double countChildTeachers=0;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("RoomId",60);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Child Name",175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teacher Name",175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Allowed Ratio",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("DateTDisplayed",170);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("In/Out",50);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Current Ratio",100);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				GridRow gridRow=new GridRow();
				ChildRoom childRoom=listChildRooms.Find(x => x.ChildRoomNum==listChildRoomLogs[i].ChildRoomNum);
				gridRow.Cells.Add(childRoom.RoomId);
				//A row will be for either a child, a teacher, or a ratio change. The two that is it not for will be set to default values
				//Example: If this is a child row, then the Child Name column will be filled, but the Teacher Name and Allowed Ratio column will be empty strings
				if(listChildRoomLogs[i].ChildNum!=0) {//Child entry
					Child child=listChildren.Find(x => x.ChildNum==listChildRoomLogs[i].ChildNum);
					string childName=Children.GetName(child);
					gridRow.Cells.Add(childName);
					gridRow.Cells.Add("");
					gridRow.Cells.Add("");
					//Find the current number of children
					if(listChildRoomLogs[i].IsComing) {
						countChildren++;
					}
					else {
						countChildren--;
					}
				}
				else if(listChildRoomLogs[i].ChildTeacherNum!=0) {//ChildTeacher entry
					gridRow.Cells.Add("");
					ChildTeacher childTeacher=listChildTeachers.Find(x => x.ChildTeacherNum==listChildRoomLogs[i].ChildTeacherNum);
					Userod userod=Userods.GetFirstOrDefault(x => x.UserNum==childTeacher.UserNum);
					string teacherName=userod.UserName;
					gridRow.Cells.Add(teacherName);
					gridRow.Cells.Add("");
					//Find the current number of teachers
					if(listChildRoomLogs[i].IsComing) {
						countChildTeachers++;
					}
					else {
						countChildTeachers--;
					}
				}
				else {//RatioChange entry
					gridRow.Cells.Add("");
					gridRow.Cells.Add("");
					gridRow.Cells.Add(listChildRoomLogs[i].RatioChange.ToString());
				}
				GridCell gridCell=new GridCell(listChildRoomLogs[i].DateTDisplayed.ToString());
				if(listChildRoomLogs[i].DateTDisplayed!=listChildRoomLogs[i].DateTEntered) {
					gridCell.ColorBackG=Colors.Red;//Color red if the Displayed and Entered dates do not match
				}
				gridRow.Cells.Add(gridCell);
				if(listChildRoomLogs[i].RatioChange!=0) {
					gridRow.Cells.Add("");//RatioChange does not get a location status
				}
				else if(listChildRoomLogs[i].IsComing) {
					gridRow.Cells.Add("In");
				}
				else {
					gridRow.Cells.Add("Out");
				}
				//Calculate current ratio
				if(countChildTeachers==0 && countChildren==0) {//There are no teachers or children. Stops division by 0.
					gridRow.Cells.Add("0 children, 0 teachers.");
				}
				else if(countChildTeachers==0) {//There are no teachers. Stops division by 0.
					gridCell=new GridCell(countChildren+" children, 0 teachers.");
					gridCell.ColorBackG=Colors.Red;
					gridRow.Cells.Add(gridCell);
				}
				else {
					gridRow.Cells.Add((countChildren/countChildTeachers).ToString()+" children per teacher.");
				}
				gridRow.Tag=listChildRoomLogs[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void comboChildRoom_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}
	}
}