using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormEmployers : FormODBase {
		private List<Employer> ListEmployers;
		//<summary>Set to true if using this dialog to select an employer.</summary>
		//public bool IsSelectMode;

		///<summary></summary>
		public FormEmployers()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ListEmployers=new List<Employer>();
		}

		private void FormEmployers_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			Employers.RefreshCache();
			ListEmployers.Clear();
			foreach(Employer employer in Employers.GetListDeep()) {
				ListEmployers.Add(employer);
			}
			ListEmployers.Sort(CompareEmployers);
			listEmp.Items.Clear();
			for(int i=0;i<ListEmployers.Count;i++){
				listEmp.Items.Add(ListEmployers[i].EmpName);
				//if(IsSelectMode && ListEmployers[i].EmployerNum==Employers.Cur.EmployerNum){
				//	listEmp.SetSelected(i);
				//}
			}
		}

		private int CompareEmployers(Employer emp1,Employer emp2) {
			return emp1.EmpName.CompareTo(emp2.EmpName);
		}

		private void listEmp_DoubleClick(object sender, System.EventArgs e) {
			if(listEmp.SelectedIndices.Count==0) {
				return;
			}
			//EmployerCur=
			//if(IsSelectMode){
			//	DialogResult=DialogResult.OK;
			//	return;
			//}
			using FormEmployerEdit FormEE=new FormEmployerEdit();
			FormEE.EmployerCur=ListEmployers[listEmp.SelectedIndices[0]];
			FormEE.ShowDialog();
			if(FormEE.DialogResult!=DialogResult.OK)
				return;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormEmployerEdit FormEE=new FormEmployerEdit();
			FormEE.EmployerCur=new Employer();
			FormEE.IsNew=true;
			FormEE.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listEmp.SelectedIndices.Count!=1){
				MessageBox.Show(Lan.g(this,"Please select one item first."));
				return;
			}
			//Employers.Cur=;
			//make sure no dependent patients:
			string dependentNames=Employers.DependentPatients(ListEmployers[listEmp.SelectedIndices[0]]);
			if(dependentNames!=""){
				MessageBox.Show(Lan.g(this,"Not allowed to delete this employer because it it attached to "
					+"the following patients.  You should combine employers instead.")
					+"\r\n\r\n"+dependentNames);
					return;
			}
			//make sure no dependent insplans:
			dependentNames=Employers.DependentInsPlans(ListEmployers[listEmp.SelectedIndices[0]]);
			if(dependentNames!=""){
				MessageBox.Show(Lan.g(this,"Not allowed to delete this employer because it is attached to "
					+"the following insurance plans.  You should combine employers instead.")
					+"\r\n\r\n"+dependentNames);
					return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Employer?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			Employers.Delete(ListEmployers[listEmp.SelectedIndices[0]]);
			FillGrid();
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			if(listEmp.SelectedIndices.Count!=1){
				MessageBox.Show(Lan.g(this,"Please select one item first."));
				return;
			}
			using FormEmployerEdit FormEE=new FormEmployerEdit();
			FormEE.EmployerCur=ListEmployers[listEmp.SelectedIndices[0]];
			FormEE.ShowDialog();
			if(FormEE.DialogResult!=DialogResult.OK)
				return;
			FillGrid();
		}

		private void butCombine_Click(object sender, System.EventArgs e) {
			if(listEmp.SelectedIndices.Count<2){
				MessageBox.Show(Lan.g(this,"Please select multiple items first while holding down the control key."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Combine all these employers into a single employer? This will affect all patients using these employers."),""
				,MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			List<long> employerNums=new List<long>();
			for(int i=0;i<listEmp.SelectedIndices.Count;i++) {
				employerNums.Add(ListEmployers[listEmp.SelectedIndices[i]].EmployerNum);
			}
			Employers.Combine(employerNums);
			FillGrid();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			/*if(IsSelectMode){
				if(listEmp.SelectedIndices.Count!=1){
					Employers.Cur=new Employer();
					//MessageBox.Show(Lan.g(this,"Please select one item first."));
					//return;
				}
				else
					Employers.Cur=ListEmployers[listEmp.SelectedIndices[0]];
			}
			else{
				//update the other computers:
				//DataValid.SetInvalid();//not needed due to intelligent refreshing
			}*/
			DataValid.SetInvalid(InvalidType.Employers);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		


	}
}



























