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
		///<summary>The room that was right clicked on in the childcare map window.</summary>
		public long ChildRoomNumCur;

		///<summary></summary>
		public FrmChildRoomLogs() {
			InitializeComponent();
			Load+=FrmChildRoomLogs_Load;
		}

		private void FrmChildRoomLogs_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<ChildRoomLog> listChildRoomLogs=ChildRoomLogs.GetChildRoomLogsByChildRoomNum(ChildRoomNumCur);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("RoomId",60);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Child Name",175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Teacher Name",175);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Allowed Ratio",50);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("DateTEntered",100);//remove this
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("DateTDisplayed",100);//make this red if different from DateTEntered
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Is Coming",50);
			gridMain.Columns.Add(gridColumn);
//need current ratio
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listChildRoomLogs.Count;i++) {
				GridRow gridRow=new GridRow();
				string roomId=ChildRooms.GetRoomId(listChildRoomLogs[i].ChildRoomNum);
				//A row will be for either a child, a teacher, or a ratio change. The two that is it not for will be set to default values
				//Example: If this is a child row, then the Child Name column will be filled, but the Teacher Name column will be an empty string and the Ratio Change column will be 0
				string childName=Children.GetName(listChildRoomLogs[i].ChildNum);
				gridRow.Cells.Add(childName);
				long userNum=ChildTeachers.GetOne(listChildRoomLogs[i].ChildTeacherNum).UserNum;
				string teacherName=Userods.GetName(userNum);
				gridRow.Cells.Add(teacherName);
				gridRow.Cells.Add(listChildRoomLogs[i].RatioChange.ToString());
				gridRow.Cells.Add(listChildRoomLogs[i].DateTEntered.ToString());
				gridRow.Cells.Add(listChildRoomLogs[i].DateTDisplayed.ToString());
				if(listChildRoomLogs[i].IsComing) {
					gridRow.Cells.Add("X");
				}
				else {
					gridRow.Cells.Add("");
				}
				gridRow.Tag=listChildRoomLogs[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}


	}
}