using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormInstructors : FormODBase {
		//private bool changed;
		
		///<summary></summary>
		public FormInstructors(){
			InitializeComponent();
			InitializeLayoutManager();
			//Providers.Selected=-1;
			Lan.F(this);
		}

		private void FormInstructors_Load(object sender, System.EventArgs e) {
			//FillList();
		}

		private void FillList(){
			/*int previousSelected=-1;
			if(listInstructors.SelectedIndex!=-1){
				previousSelected=Instructors.List[listInstructors.SelectedIndex].InstructorNum;
			}
			Instructors.Refresh();
			listInstructors.Items.Clear();
			for(int i=0;i<Instructors.List.Length;i++){
				listInstructors.Items.Add(Instructors.List[i].LName+", "+Instructors.List[i].FName+", "+Instructors.List[i].Suffix);
				if(Instructors.List[i].InstructorNum==previousSelected){
					listInstructors.SelectedIndex=i;
				}
			}*/
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			/*Instructor cur=new Instructor();
			using FormInstructorEdit FormI=new FormInstructorEdit(cur);
			FormI.IsNew=true;
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			FillList();
			listInstructors.SelectedIndex=-1;*/
		}

		private void listInstructors_DoubleClick(object sender, System.EventArgs e) {
			/*if(listInstructors.SelectedIndex==-1)
				return;
			using FormInstructorEdit FormI=new FormInstructorEdit(Instructors.List[listInstructors.SelectedIndex]);
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			FillList();*/
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			//Close();
		}

		private void FormInstructors_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			//if(changed){
			//	DataValid.SetInvalid(InvalidTypes.DentalSchools);
			//}
		}

	}
}
