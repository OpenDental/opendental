using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>Only used when adding an insurance plan from ContrFamily.  Lets user select the subscriber from </summary>
	public partial class FormSubscriberSelect : FormODBase {
		private Family _family;
		///<summary>When the form closes with OK, this will contain the patient num selected.</summary>
		public long PatNumSelected;
		private List<PatientLink> _listPatientLinksMerge;

		///<summary></summary>
		public FormSubscriberSelect(Family family)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_family=family;
		}

		private void FormSubscriberSelect_Load(object sender, System.EventArgs e) {
			_listPatientLinksMerge=PatientLinks.GetLinks(_family.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			FillList();
		}

		private void FillList() {
			listPats.Items.Clear();
			for(int i=0;i<_family.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(_family.ListPats[i].PatNum,_listPatientLinksMerge)) {
					continue;//Don't show merged patients
				}
				listPats.Items.Add(_family.ListPats[i].GetNameFL()+" ("+Lan.g("enumPatientStatus",
					_family.ListPats[i].PatStatus.GetDescription())+")",_family.ListPats[i]);
			}
		}
		
		private void listPats_DoubleClick(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				return;
			}
			PatNumSelected=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butMore_Click(object sender, System.EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK){
				return;
			}
			PatNumSelected=formPatientSelect.PatNumSelected;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				MsgBox.Show(this,"Please pick a patient first.");
				return;
			}
			PatNumSelected=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}





















