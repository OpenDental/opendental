using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormCounties : FormODBase {
		private List<County> _listCounties;

		///<summary></summary>
		public FormCounties()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCounties_Load(object sender, System.EventArgs e) {
			FillList();
		}

		private void FillList(){
			_listCounties=Counties.Refresh(name:"");
			listCounties.Items.Clear();
			string countyInfo="";
			for(int i=0;i<_listCounties.Count;i++){
				countyInfo=_listCounties[i].CountyName;
				if(_listCounties[i].CountyCode != ""){
					countyInfo+=", "+_listCounties[i].CountyCode;
				}
				listCounties.Items.Add(countyInfo);
			}
		}

		private void listCounties_DoubleClick(object sender, System.EventArgs e) {
			if(listCounties.SelectedIndex==-1){
				return;
			}
			using FormCountyEdit formCountyEdit=new FormCountyEdit();
			formCountyEdit.CountyCur=_listCounties[listCounties.SelectedIndex];
			formCountyEdit.ShowDialog();
			if(formCountyEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillList();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormCountyEdit formCountyEdit=new FormCountyEdit();
			formCountyEdit.IsNew=true;
			formCountyEdit.CountyCur=new County();
			formCountyEdit.ShowDialog();
			if(formCountyEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listCounties.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			County county=_listCounties[listCounties.SelectedIndex];
			string usedBy=Counties.UsedBy(county.CountyName);
			if(usedBy != ""){
				MessageBox.Show(Lan.g(this,"Cannot delete County because it is already in use by the following patients: \r")+usedBy);
				return;
			}
			Counties.Delete(county);
			FillList();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















