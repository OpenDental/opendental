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
		public FormDiseaseDefEdit(DiseaseDef diseaseDef,bool hasDelete)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			DiseaseDefCur=diseaseDef;
			_hasDelete=hasDelete;
		}

		private void FormDiseaseDefEdit_Load(object sender,EventArgs e) {
			textName.Text=DiseaseDefCur.DiseaseName;
			string icd9Description=ICD9s.GetCodeAndDescription(DiseaseDefCur.ICD9Code);
			textICD9.Text=icd9Description;
			if(icd9Description=="") {
				textICD9.Text=DiseaseDefCur.ICD9Code;
			}
			textIcd10.Text=DiseaseDefCur.Icd10Code;
			Icd10 icd10=Icd10s.GetByCode(DiseaseDefCur.Icd10Code);
			if(icd10!=null) {
				textIcd10.Text=icd10.Icd10Code+"-"+icd10.Description;
			}
			string snomedDescription=Snomeds.GetCodeAndDescription(DiseaseDefCur.SnomedCode);
			textSnomed.Text=snomedDescription;
			if(snomedDescription=="") {
				textSnomed.Text=DiseaseDefCur.SnomedCode;
			}
			checkIsHidden.Checked=DiseaseDefCur.IsHidden;
		}

		private void butSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			formSnomeds.ShowDialog();
			if(formSnomeds.DialogResult!=DialogResult.OK) {
				return;
			}
			if(DiseaseDefs.ContainsSnomed(formSnomeds.SelectedSnomed.SnomedCode,DiseaseDefCur.DiseaseDefNum)) {//DiseaseDefNum could be zero
				MsgBox.Show(this,"Snomed code already exists in the problems list.");
				return;
			}
			DiseaseDefCur.SnomedCode=formSnomeds.SelectedSnomed.SnomedCode;
			string snomedDescription=Snomeds.GetCodeAndDescription(formSnomeds.SelectedSnomed.SnomedCode);
			textSnomed.Text=snomedDescription;
			if(snomedDescription=="") {
				textSnomed.Text=formSnomeds.SelectedSnomed.SnomedCode;
			}
		}

		private void butIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s formIcd9s=new FormIcd9s();
			formIcd9s.IsSelectionMode=true;
			formIcd9s.ShowDialog();
			if(formIcd9s.DialogResult!=DialogResult.OK) {
				return;
			}
			if(DiseaseDefs.ContainsICD9(formIcd9s.SelectedIcd9.ICD9Code,DiseaseDefCur.DiseaseDefNum)) {
				MsgBox.Show(this,"ICD-9 code already exists in the problems list.");
				return;
			}
			DiseaseDefCur.ICD9Code=formIcd9s.SelectedIcd9.ICD9Code;
			string icd9Description=ICD9s.GetCodeAndDescription(formIcd9s.SelectedIcd9.ICD9Code);
			textICD9.Text=icd9Description;
			if(icd9Description=="") {
				textICD9.Text=formIcd9s.SelectedIcd9.ICD9Code;
			}
		}

		private void butIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s formIcd10s=new FormIcd10s();
			formIcd10s.IsSelectionMode=true;
			formIcd10s.ShowDialog();
			if(formIcd10s.DialogResult!=DialogResult.OK) {
				return;
			}
			if(DiseaseDefs.ContainsIcd10(formIcd10s.SelectedIcd10.Icd10Code,DiseaseDefCur.DiseaseDefNum)) {
				MsgBox.Show(this,"ICD-10 code already exists in the problems list.");
				return;
			}
			DiseaseDefCur.Icd10Code=formIcd10s.SelectedIcd10.Icd10Code;
			textIcd10.Text=formIcd10s.SelectedIcd10.Icd10Code+"-"+formIcd10s.SelectedIcd10.Description;
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
			SecurityLogMsgText=DiseaseDefCur.DiseaseName;
			if(IsNew) {
				SecurityLogMsgText+=" added.";
			}
			//Cache invalidation done in FormDiseaseDefs.cs
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	

		


	}
}





















