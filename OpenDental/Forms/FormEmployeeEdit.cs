using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormEmployeeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public Employee EmployeeCur;

		///<summary></summary>
		public FormEmployeeEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmployeeEdit_Load(object sender, System.EventArgs e) {
			checkIsHidden.Checked=EmployeeCur.IsHidden;
			textLName.Text=EmployeeCur.LName;
			textFName.Text=EmployeeCur.FName;
			textMI.Text=EmployeeCur.MiddleI;
			textPayrollID.Text=EmployeeCur.PayrollID;
			textPhoneExt.Text=EmployeeCur.PhoneExt.ToString();
			textWirelessPhone.Text=EmployeeCur.WirelessPhone;
			textEmailWork.Text=EmployeeCur.EmailWork;
			textEmailPersonal.Text=EmployeeCur.EmailPersonal;
			checkIsFurloughed.Checked=EmployeeCur.IsFurloughed;
			checkIsWorkingHome.Checked=EmployeeCur.IsWorkingHome;
			comboReportsTo.Items.AddNone<Employee>();
			List<Employee> listEmployees=Employees.GetDeepCopy(isShort:true);//excludes hidden
			comboReportsTo.Items.AddList(listEmployees,x=>x.FName+" "+x.LName);
			comboReportsTo.SetSelectedKey<Employee>(EmployeeCur.ReportsTo,x=>x.EmployeeNum);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			//not new:
			try{
				Employees.Delete(EmployeeCur.EmployeeNum);
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			Employee employeeOld=EmployeeCur.Copy();
			EmployeeCur.IsHidden=checkIsHidden.Checked;
			EmployeeCur.LName=textLName.Text;
			EmployeeCur.FName=textFName.Text;
			EmployeeCur.MiddleI=textMI.Text;
			EmployeeCur.PayrollID=textPayrollID.Text;
			try{
				EmployeeCur.PhoneExt=PIn.Int(textPhoneExt.Text);
			}
			catch{
				EmployeeCur.PhoneExt=0;
			}
			EmployeeCur.WirelessPhone=textWirelessPhone.Text;
			EmployeeCur.EmailWork=textEmailWork.Text;
			EmployeeCur.EmailPersonal=textEmailPersonal.Text;
			EmployeeCur.IsFurloughed=checkIsFurloughed.Checked;
			EmployeeCur.IsWorkingHome=checkIsWorkingHome.Checked;
			EmployeeCur.ReportsTo=comboReportsTo.GetSelectedKey<Employee>(x=>x.EmployeeNum);
			try {
				if(IsNew) {
					Employees.Insert(EmployeeCur);
				}
				else {
					Employees.UpdateChanged(EmployeeCur,employeeOld);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormEmployeeEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			
		}

		

		
		
	}

	
	
}

























