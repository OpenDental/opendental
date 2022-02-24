using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDiseaseDefEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public DiseaseDef DiseaseDefCur;
		///<summary>The security log message text for inserting/updating problems.  The messages will be written to the log after syncing changes.</summary>
		public string SecurityLogMsgText;
		private bool _hasDelete;

		///<summary></summary>
		public FormDiseaseDefEdit(DiseaseDef diseaseDefCur,bool hasDelete)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			DiseaseDefCur=diseaseDefCur;
			_hasDelete=hasDelete;
		}

		private void FormDiseaseDefEdit_Load(object sender, System.EventArgs e) {
			textName.Text=DiseaseDefCur.DiseaseName;
			string i9descript=ICD9s.GetCodeAndDescription(DiseaseDefCur.ICD9Code);
			if(i9descript=="") {
				textICD9.Text=DiseaseDefCur.ICD9Code;
			}
			else {
				textICD9.Text=i9descript;
			}
			Icd10 i10=Icd10s.GetByCode(DiseaseDefCur.Icd10Code);
			if(i10==null) {
				textIcd10.Text=DiseaseDefCur.Icd10Code;
			}
			else {
				textIcd10.Text=i10.Icd10Code+"-"+i10.Description;
			}
			string sdescript=Snomeds.GetCodeAndDescription(DiseaseDefCur.SnomedCode);
			if(sdescript=="") {
				textSnomed.Text=DiseaseDefCur.SnomedCode;
			}
			else {
				textSnomed.Text=sdescript;
			}
			checkIsHidden.Checked=DiseaseDefCur.IsHidden;
		}

		private void butSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			FormS.IsSelectionMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			if(DiseaseDefs.ContainsSnomed(FormS.SelectedSnomed.SnomedCode,DiseaseDefCur.DiseaseDefNum)) {//DiseaseDefNum could be zero
				MsgBox.Show(this,"Snomed code already exists in the problems list.");
				return;
			}
			DiseaseDefCur.SnomedCode=FormS.SelectedSnomed.SnomedCode;
			string sdescript=Snomeds.GetCodeAndDescription(FormS.SelectedSnomed.SnomedCode);
			if(sdescript=="") {
				textSnomed.Text=FormS.SelectedSnomed.SnomedCode;
			}
			else {
				textSnomed.Text=sdescript;
			}
		}

		private void butIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s FormI=new FormIcd9s();
			FormI.IsSelectionMode=true;
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK) {
				return;
			}
			if(DiseaseDefs.ContainsICD9(FormI.SelectedIcd9.ICD9Code,DiseaseDefCur.DiseaseDefNum)) {
				MsgBox.Show(this,"ICD-9 code already exists in the problems list.");
				return;
			}
			DiseaseDefCur.ICD9Code=FormI.SelectedIcd9.ICD9Code;
			string i9descript=ICD9s.GetCodeAndDescription(FormI.SelectedIcd9.ICD9Code);
			if(i9descript=="") {
				textICD9.Text=FormI.SelectedIcd9.ICD9Code;
			}
			else {
				textICD9.Text=i9descript;
			}
		}

		private void butIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s FormI=new FormIcd10s();
			FormI.IsSelectionMode=true;
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK) {
				return;
			}
			if(DiseaseDefs.ContainsIcd10(FormI.SelectedIcd10.Icd10Code,DiseaseDefCur.DiseaseDefNum)) {
				MsgBox.Show(this,"ICD-10 code already exists in the problems list.");
				return;
			}
			DiseaseDefCur.Icd10Code=FormI.SelectedIcd10.Icd10Code;
			textIcd10.Text=FormI.SelectedIcd10.Icd10Code+"-"+FormI.SelectedIcd10.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!_hasDelete) {
				MsgBox.Show(this,"This problem def is currently in use and cannot be deleted.");
				return;
			}
			SecurityLogMsgText=DiseaseDefCur.DiseaseName+" "+Lan.g(this,"deleted.");
			DiseaseDefCur=null;//Flags this disease for removal in outside forms.
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textName.Text=="") {
				MsgBox.Show(this,"Not allowed to create a Disease Definition without a description.");
				return;
			}
			//Icd9Code and SnomedCode set on load or on return from code picker forms
			DiseaseDefCur.DiseaseName=textName.Text;
			DiseaseDefCur.IsHidden=checkIsHidden.Checked;
			//Possibly remove this part and let the sync take care of insert/update in FormDiseaseDefs.cs
			if(IsNew) {
				SecurityLogMsgText=DiseaseDefCur.DiseaseName+" added.";
			}
			else{
				SecurityLogMsgText=DiseaseDefCur.DiseaseName;
			}
			//Cache invalidation done in FormDiseaseDefs.cs
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	

		


	}
}





















