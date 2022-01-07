using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>For a given subscriber, this list all their plans.  User then selects one plan from the list or creates a blank plan.</summary>
	public partial class FormFamilyMemberSelect:FormODBase {
		private Family Fam;
		///<summary>When dialogResult=OK, this will contain the PatNum of the selected pat.</summary>
		public long SelectedPatNum;
		///<summary>When this flag is set, the patient status will appear in parenthesis by the patient's name in the list.</summary>
		private bool _isPatientStatusVisible;

		///<summary></summary>
		public FormFamilyMemberSelect(Family fam,bool isPatientStatusVisible=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Fam=fam;
			_isPatientStatusVisible=isPatientStatusVisible;
		}

		private void FormFamilyMemberSelect_Load(object sender, System.EventArgs e) {
			for(int i=0;i<Fam.ListPats.Length;i++){
				string patientStatus=" ("+Lan.g("enumPatientStatus",Fam.ListPats[i].PatStatus.GetDescription())+")";
				listPats.Items.Add(Fam.ListPats[i].GetNameFL()+(_isPatientStatusVisible ? patientStatus : ""),Fam.ListPats[i]);
			}
		}

		private void listPats_DoubleClick(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butOther_Click(object sender, System.EventArgs e) {
			using FormPatientSelect FormPS=new FormPatientSelect();
			FormPS.SelectionModeOnly=true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult!=DialogResult.OK) {
				return;
			}
			SelectedPatNum=FormPS.SelectedPatNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















