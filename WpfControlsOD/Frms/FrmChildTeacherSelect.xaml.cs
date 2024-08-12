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
	public partial class FrmChildTeacherSelect:FrmODBase {
		///<summary>This value will be filled when the frm closes with IsDialogOK=true and IsSelectionMode==true.</summary>
		public long EmployeeNumSelected;

		///<summary></summary>
		public FrmChildTeacherSelect() {
			InitializeComponent();
			Load+=FrmChildTeacherSelect_Load;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
		}

		private void FrmChildTeacherSelect_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<Employee> listEmployees=Employees.GetDeepCopy(isShort:true);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("First Name",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Last Name",100);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Status",100);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<listEmployees.Count;i++) {
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(listEmployees[i].FName);
				gridRow.Cells.Add(listEmployees[i].LName);
				gridRow.Cells.Add(listEmployees[i].ClockStatus);
				gridRow.Tag=listEmployees[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(gridMain.SelectedTag<Employee>()==null) {
				return;
			}
			EmployeeNumSelected=gridMain.SelectedTag<Employee>().EmployeeNum;
			IsDialogOK=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Please pick a teacher first.");
				return;
			}
			EmployeeNumSelected=gridMain.SelectedTag<Employee>().EmployeeNum;
			IsDialogOK=true;
		}
	}
}