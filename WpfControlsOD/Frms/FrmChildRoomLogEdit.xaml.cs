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
	public partial class FrmChildRoomLogEdit:FrmODBase {
		///<summary>Store the child selected until the log is saved.</summary>
		private long _childNumSelected;
		///<summary>Store the employee selected until the log is saved.</summary>
		private long _employeeNumSelected;
		///<summary>Set by parent.</summary>
		public ChildRoomLog ChildRoomLogCur;

		///<summary></summary>
		public FrmChildRoomLogEdit() {
			InitializeComponent();
			Load+=FrmChildRoomLogEdit_Load;
		}

		private void FrmChildRoomLogEdit_Load(object sender,EventArgs e) {
			textDateTEntered.Text=ChildRoomLogCur.DateTEntered.ToString();
			textDateTDisplayed.Text=ChildRoomLogCur.DateTDisplayed.ToString();
			Child child=Children.GetOne(ChildRoomLogCur.ChildNum);
			if(child!=null) {
				textChild.Text=Children.GetName(child);
				_childNumSelected=child.ChildNum;
			}
			Employee employee=Employees.GetEmp(ChildRoomLogCur.EmployeeNum);
			if(employee!=null) {
				textEmployee.Text=employee.FName+" "+employee.LName;
				_employeeNumSelected=employee.EmployeeNum;
			}
			if(ChildRoomLogCur.IsComing) {
				radioIn.Checked=true;
			}
			else {
				radioOut.Checked=true;
			}
			List<ChildRoom> listChildRooms=ChildRooms.GetAll();
			comboChildRoom.Items.AddList(listChildRooms,x => x.RoomId);
			comboChildRoom.SetSelectedKey<ChildRoom>(ChildRoomLogCur.ChildRoomNum,x=>x.ChildRoomNum);//Should be set even when new
		}

		private void butChildSelect_Click(object sender,EventArgs e) {
			FrmChildren frmChildren=new FrmChildren();
			frmChildren.IsSelectionMode=true;
			frmChildren.ShowDialog();
			if(frmChildren.IsDialogCancel) {
				return;
			}
			Child child=Children.GetOne(frmChildren.ChildNumSelected);
			textChild.Text=Children.GetName(child);
			_childNumSelected=child.ChildNum;
		}

		private void butChildClear_Click(object sender,EventArgs e) {
			_childNumSelected=0;
			textChild.Text="";
		}

		private void butEmployeeSelect_Click(object sender,EventArgs e) {
			FrmChildTeacherSelect frmChildTeacherSelect=new FrmChildTeacherSelect();
			frmChildTeacherSelect.ShowDialog();
			if(frmChildTeacherSelect.IsDialogCancel) {
				return;
			}
			Employee employee=Employees.GetEmp(frmChildTeacherSelect.EmployeeNumSelected);
			textEmployee.Text=employee.FName+" "+employee.LName;
			_employeeNumSelected=employee.EmployeeNum;
		}

		private void butEmployeeClear_Click(object sender,EventArgs e) {
			_employeeNumSelected=0;
			textEmployee.Text="";
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ChildRoomLogCur.IsNew) {
				IsDialogCancel=true;
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.YesNo,"Delete Log?")) {
				return;
			}
			ChildRoomLogs.Delete(ChildRoomLogCur.ChildRoomLogNum);
			Signalods.SetInvalid(InvalidType.Children);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender,EventArgs e) {
			//Validation
			if((_childNumSelected!=0 && _employeeNumSelected!=0)
				|| _childNumSelected==0 && _employeeNumSelected==0) {
				MsgBox.Show("The log must be for a child or a teacher not both.");
				return;//Both or none had a selection
			}
			DateTime dateTimeDisplayed;
			try {
				dateTimeDisplayed=DateTime.Parse(textDateTDisplayed.Text);
			}
			catch {
				MsgBox.Show("The datetime entered is invalid.");
				return;
			}
			if(comboChildRoom.SelectedIndex==-1) {
				MsgBox.Show("A classroom must be selected.");
				return;
			}
			ChildRoomLogCur.DateTEntered=DateTime.Parse(textDateTEntered.Text);
			ChildRoomLogCur.DateTDisplayed=dateTimeDisplayed;
			//One of these two will be a non 0 value by this point
			ChildRoomLogCur.ChildNum=_childNumSelected;
			ChildRoomLogCur.EmployeeNum=_employeeNumSelected;
			if(radioIn.Checked){
				ChildRoomLogCur.IsComing=true;
			}
			else{
				ChildRoomLogCur.IsComing=false;
			}
			ChildRoomLogCur.ChildRoomNum=comboChildRoom.GetSelectedKey<ChildRoom>(x=>x.ChildRoomNum);
			if(ChildRoomLogCur.IsNew) {
				ChildRoomLogs.Insert(ChildRoomLogCur);
			}
			else {
				ChildRoomLogs.Update(ChildRoomLogCur);
			}
			Signalods.SetInvalid(InvalidType.Children);
			IsDialogOK=true;
		}
	}
}