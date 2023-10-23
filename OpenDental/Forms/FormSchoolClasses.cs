using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormSchoolClasses : FormODBase {
		private List<SchoolClass> _listSchoolClasses;

		///<summary></summary>
		public FormSchoolClasses(){
			InitializeComponent();
			InitializeLayoutManager();
			//Providers.Selected=-1;
			Lan.F(this);
		}

		private void FormSchoolClasses_Load(object sender, System.EventArgs e) {
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			FillList();
		}

		private void FillList(){
			listMain.Items.Clear();
			for(int i=0;i<_listSchoolClasses.Count;i++){
				listMain.Items.Add(_listSchoolClasses[i].GradYear.ToString()+" - "+_listSchoolClasses[i].Descript);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormSchoolClassEdit formSchoolClassEdit=new FormSchoolClassEdit();
			formSchoolClassEdit.SchoolClassCur=new SchoolClass();
			formSchoolClassEdit.IsNew=true;
			formSchoolClassEdit.ShowDialog();
			if(formSchoolClassEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			FillList();
			listMain.SelectedIndex=-1;
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			using FormSchoolClassEdit formSchoolClassEdit=new FormSchoolClassEdit();
			formSchoolClassEdit.SchoolClassCur=_listSchoolClasses[listMain.SelectedIndex];
			formSchoolClassEdit.ShowDialog();
			if(formSchoolClassEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			FillList();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}
