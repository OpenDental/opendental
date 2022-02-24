using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCertEmployee:FormODBase {
		public CertEmployee CertEmployee;
		public Cert Cert;
		public Employee Employee;

		public FormCertEmployee() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCertEmployee_Load(object sender, EventArgs e){
			//Employee(read only),Cert description(read only), categories(read only), date, note, user
			Employee=Employees.GetEmp(Employee.EmployeeNum);
			textEmployee.Text=Employee.FName+" "+Employee.LName;
			textCertification.Text=Cert.Description;
			textCertCategories.Text=Defs.GetDef(DefCat.CertificationCategories,Cert.CertCategoryNum).ItemName;
			if(!CertEmployee.IsNew) {
				textDateCompleted.Text=CertEmployee.DateCompleted.ToShortDateString();
				textNote.Text=CertEmployee.Note;
			}
		}

		private void butToday_Click(object sender,EventArgs e) {
			textDateCompleted.Text=DateTime.Today.ToShortDateString();
			this.ActiveControl=textNote;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(CertEmployee.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}		
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Certification Completion?")) {
				return;
			}
			CertEmployees.Delete(CertEmployee.CertEmployeeNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDateCompleted.IsValid() || textDateCompleted.Text=="") {
				MsgBox.Show(this,"Please enter a valid date.");
				return;
			}
			if(PIn.Date(textDateCompleted.Text)>DateTime.Today) {
				MsgBox.Show(this,"Date can not be greater than today.");
				return;
			}
			CertEmployee.DateCompleted=PIn.Date(textDateCompleted.Text);
			CertEmployee.Note=PIn.String(textNote.Text);
			CertEmployee.UserNum=Security.CurUser.UserNum;
			if(CertEmployee.IsNew) {
				CertEmployee.CertNum=Cert.CertNum;
				CertEmployee.EmployeeNum=Employee.EmployeeNum;				
				CertEmployees.Insert(CertEmployee);				
			}
			else {
				CertEmployees.Update(CertEmployee);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}