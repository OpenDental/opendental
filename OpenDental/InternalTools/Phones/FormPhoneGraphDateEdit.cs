using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPhoneGraphDateEdit:FormODBase {
		public DateTime DateEdit;
		private List<Employee> _listEmployeeSupers;

		public FormPhoneGraphDateEdit(DateTime dateEdit,bool showScheduleButton=true) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			butEditSchedule.Visible=showScheduleButton;
			DateEdit=dateEdit;			
		}

		private void FormPhoneGraphDateEdit_Load(object sender,EventArgs e) {
			textPeak.Text=PrefC.GetRaw("GraphEmployeeTimesPeak");
			textSuperPeak.Text=PrefC.GetRaw("GraphEmployeeTimesSuperPeak");
			List<Employee> listEmployees=Employees.GetDeepCopy(isShort:true);
			_listEmployeeSupers=new List<Employee>();
			for(int i=0;i<listEmployees.Count;i++){
				if(listEmployees[i].ReportsTo==0){
					continue;
				}
				if(_listEmployeeSupers.Any(x=>x.EmployeeNum==listEmployees[i].ReportsTo)){
					continue;
				}
				_listEmployeeSupers.Add(Employees.GetEmp(listEmployees[i].ReportsTo));
			}
			_listEmployeeSupers=_listEmployeeSupers.OrderBy(x=>x.FName).ToList();
			string strSupers=PrefC.GetRaw("GraphEmployeeMaxPresched");//list of supervisors
			List<string> listEmpNumsSuper=strSupers.Split(',').ToList();
			for(int i=0;i<_listEmployeeSupers.Count;i++){
				listSupers.Items.Add(_listEmployeeSupers[i].FName);
				if(listEmpNumsSuper.Contains(_listEmployeeSupers[i].EmployeeNum.ToString())){
					listSupers.SetSelected(i);
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			Prefs.UpdateRaw("GraphEmployeeTimesPeak",textPeak.Text);
			Prefs.UpdateRaw("GraphEmployeeTimesSuperPeak",textSuperPeak.Text);
			string strSupers="";
			for(int i=0;i<listSupers.SelectedIndices.Count;i++){
				if(i>0){
					strSupers+=",";
				}
				strSupers+=_listEmployeeSupers[listSupers.SelectedIndices[i]].EmployeeNum.ToString();
			}
			Prefs.UpdateRaw("GraphEmployeeMaxPresched",strSupers);
			Signalods.SetInvalid(InvalidType.Prefs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}