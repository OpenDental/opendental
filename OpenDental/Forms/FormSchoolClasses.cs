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
		private List<SchoolClass> _listSchoolCasses;

		///<summary></summary>
		public FormSchoolClasses(){
			InitializeComponent();
			InitializeLayoutManager();
			//Providers.Selected=-1;
			Lan.F(this);
		}

		private void FormSchoolClasses_Load(object sender, System.EventArgs e) {
			_listSchoolCasses=SchoolClasses.GetDeepCopy();
			FillList();
		}

		private void FillList(){
			listMain.Items.Clear();
			for(int i=0;i<_listSchoolCasses.Count;i++){
				listMain.Items.Add(_listSchoolCasses[i].GradYear.ToString()+" - "+_listSchoolCasses[i].Descript);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			SchoolClass cur=new SchoolClass();
			using FormSchoolClassEdit FormS=new FormSchoolClassEdit(cur);
			FormS.IsNew=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolCasses=SchoolClasses.GetDeepCopy();
			FillList();
			listMain.SelectedIndex=-1;
		}

		private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listMain.SelectedIndex==-1) {
				return;
			}
			using FormSchoolClassEdit FormS=new FormSchoolClassEdit(_listSchoolCasses[listMain.SelectedIndex]);
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			_listSchoolCasses=SchoolClasses.GetDeepCopy();
			FillList();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}
	}
}
