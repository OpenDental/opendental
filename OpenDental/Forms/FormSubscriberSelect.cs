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
		private Family FamCur;
		///<summary>When the form closes with OK, this will contain the patient num selected.</summary>
		public long SelectedPatNum;
		private List<PatientLink> _listMergeLinks;

		///<summary></summary>
		public FormSubscriberSelect(Family famCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			FamCur=famCur;
		}

		private void FormSubscriberSelect_Load(object sender, System.EventArgs e) {
			_listMergeLinks=PatientLinks.GetLinks(FamCur.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			FillList();
		}

		private void FillList() {
			listPats.Items.Clear();
			for(int i=0;i<FamCur.ListPats.Length;i++) {
				if(PatientLinks.WasPatientMerged(FamCur.ListPats[i].PatNum,_listMergeLinks)) {
					continue;//Don't show merged patients
				}
				listPats.Items.Add(FamCur.ListPats[i].GetNameFL()+" ("+Lan.g("enumPatientStatus",
					FamCur.ListPats[i].PatStatus.GetDescription())+")",FamCur.ListPats[i]);
			}
		}
		
		private void listPats_DoubleClick(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			DialogResult=DialogResult.OK;
		}

		private void butMore_Click(object sender, System.EventArgs e) {
			using FormPatientSelect FormP=new FormPatientSelect();
			FormP.SelectionModeOnly=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			SelectedPatNum=FormP.SelectedPatNum;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				MsgBox.Show(this,"Please pick a patient first.");
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





















