using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormEmployerEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public Employer EmployerCur;

		///<summary></summary>
		public FormEmployerEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmployerEdit_Load(object sender, System.EventArgs e) {
			textEmp.Text=EmployerCur.EmpName;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(String.IsNullOrWhiteSpace(textEmp.Text)) {
				MsgBox.Show(this,"Please enter an employer name.");
				return;
			}
			Employer empOld=EmployerCur.Copy();
			EmployerCur.EmpName=textEmp.Text;
			if(IsNew){
				Employers.Insert(EmployerCur);
				Employers.MakeLog(EmployerCur);
			}
			else{
				Employers.Update(EmployerCur,empOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormEmployerEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK)
				return;
			//if(IsNew)
			//	Employers.Delete(EmployerCur);
		}

		


	}
}





















