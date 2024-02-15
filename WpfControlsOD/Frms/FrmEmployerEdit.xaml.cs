using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmEmployerEdit : FrmODBase {
		///<summary></summary>
		public bool IsNew;
		public Employer EmployerCur;

		///<summary></summary>
		public FrmEmployerEdit()
		{
			InitializeComponent();
			KeyDown+=Frm_KeyDown;
			Load+=FrmEmployerEdit_Load;
			PreviewKeyDown+=FrmEmployerEdit_PreviewKeyDown;
		}

		private void FrmEmployerEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			textEmp.Text=EmployerCur.EmpName;
		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void FrmEmployerEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(String.IsNullOrWhiteSpace(textEmp.Text)) {
				MsgBox.Show(this,"Please enter an employer name.");
				return;
			}
			Employer employerOld=EmployerCur.Copy();
			EmployerCur.EmpName=textEmp.Text;
			if(IsNew){
				Employers.Insert(EmployerCur);
				Employers.MakeLog(EmployerCur);
			}
			else{
				Employers.Update(EmployerCur,employerOld);
			}
			IsDialogOK=true;
		}

	}
}