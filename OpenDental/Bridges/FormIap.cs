using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.Bridges;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormIap : FormODBase{
		///<summary>The user will have selected an employer.  This will be the exact text representation of that employer as it is in the iap database.</summary>
		public string selectedPlan;
		private ArrayList list=null;

		///<summary></summary>
		public FormIap()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormIap_Load(object sender, System.EventArgs e) {
			listPlans.Items.Clear();
		}

		private void textPlanSearch_TextChanged(object sender, System.EventArgs e) {
			listPlans.Items.Clear();
			if(textPlanSearch.Text==""){
				return;
			}
			textCarrier.Text="";
			textEmp.Text="";
			textPlanNum.Text="";
			list=Bridges.Iap.GetList(textPlanSearch.Text);
			for(int i=1;i<list.Count;i+=2){
				listPlans.Items.Add(list[i].ToString(),list[i]);
			}
		}

		private void listPlans_SelectedIndexChanged(object sender, System.EventArgs e) {
			textCarrier.Text="";
			textEmp.Text="";
			textPlanNum.Text="";
			if(listPlans.SelectedIndex>=0){
				textPlanNum.Text=list[listPlans.SelectedIndex*2].ToString();
				try{
					Iap.ReadRecord(textPlanNum.Text);
					textCarrier.Text=Iap.ReadField(Iap.Carrier);
					textEmp.Text=Iap.ReadField(Iap.Employer);
				}
				catch(ApplicationException ex){
					MessageBox.Show(ex.Message);
					textCarrier.Text="";
					textEmp.Text="";
					textPlanNum.Text="";
					return;
				}
				finally{
					Iap.CloseDatabase();
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPlans.SelectedIndex==-1){
				MessageBox.Show("Please select a plan first.");
				return;
			}
			selectedPlan=list[listPlans.SelectedIndex*2].ToString();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
		

		


	}
}





















