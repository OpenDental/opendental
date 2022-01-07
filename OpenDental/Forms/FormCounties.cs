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
	public partial class FormCounties : FormODBase {
		private County[] CountiesList;

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
			CountiesList=Counties.Refresh();
			listCounties.Items.Clear();
			string s="";
			for(int i=0;i<CountiesList.Length;i++){
				s=CountiesList[i].CountyName;
				if(CountiesList[i].CountyCode != ""){
					s+=", "+CountiesList[i].CountyCode;
				}
				listCounties.Items.Add(s);
			}
		}

		private void listCounties_DoubleClick(object sender, System.EventArgs e) {
			if(listCounties.SelectedIndex==-1){
				return;
			}
			using FormCountyEdit FormSE=new FormCountyEdit();
			FormSE.CountyCur=CountiesList[listCounties.SelectedIndex];
			FormSE.ShowDialog();
			if(FormSE.DialogResult!=DialogResult.OK){
				return;
			}
			FillList();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormCountyEdit FormSE=new FormCountyEdit();
			FormSE.IsNew=true;
			FormSE.CountyCur=new County();
			FormSE.ShowDialog();
			if(FormSE.DialogResult!=DialogResult.OK){
				return;
			}
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listCounties.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			County CountyCur=CountiesList[listCounties.SelectedIndex];
			string usedBy=Counties.UsedBy(CountyCur.CountyName);
			if(usedBy != ""){
				MessageBox.Show(Lan.g(this,"Cannot delete County because it is already in use by the following patients: \r")+usedBy);
				return;
			}
			Counties.Delete(CountyCur);
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





















