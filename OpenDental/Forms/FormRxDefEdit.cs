using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public partial class FormRxDefEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private RxDef _rxDef;
		private List<RxAlert> _listRxAlerts;

		///<summary>Must have already saved it to db so that we have a RxDefNum to work with.</summary>
		public FormRxDefEdit(RxDef rxDefCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			_rxDef=rxDefCur.Copy();
		}

		private void FormRxDefEdit_Load(object sender, System.EventArgs e) {
			textDrug.Text=_rxDef.Drug;
			textSig.Text=_rxDef.Sig;
			textDisp.Text=_rxDef.Disp;
			textRefills.Text=_rxDef.Refills;
			textNotes.Text=_rxDef.Notes;
			textPatInstruction.Text=_rxDef.PatientInstruction;
			checkControlled.Checked=_rxDef.IsControlled;
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				checkProcRequired.Enabled=true;
				checkProcRequired.Checked=_rxDef.IsProcRequired;
			}
			FillAlerts();
			FillRxCui();
		}

		private void FillRxCui() {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("US")) {//Not United States
				labelRxNorm.Visible=false;
				textRxCui.Visible=false;
				butRxNormSelect.Visible=false;
				return;
			}
			if(_rxDef.RxCui==0) {
				textRxCui.Text="";
			}
			else {
				textRxCui.Text=_rxDef.RxCui.ToString()+" - "+RxNorms.GetDescByRxCui(_rxDef.RxCui.ToString());
			}
		}

		private void FillAlerts(){
			_listRxAlerts=RxAlerts.Refresh(_rxDef.RxDefNum);
			listAlerts.Items.Clear();
			for(int i=0;i<_listRxAlerts.Count;i++) {
				if(_listRxAlerts[i].DiseaseDefNum>0) {
					listAlerts.Items.Add(DiseaseDefs.GetName(_listRxAlerts[i].DiseaseDefNum));
				}
				if(_listRxAlerts[i].AllergyDefNum>0) {
					AllergyDef allergyDef=AllergyDefs.GetOne(_listRxAlerts[i].AllergyDefNum);
					if(allergyDef!=null) {
						listAlerts.Items.Add(allergyDef.Description);
					}
				}
				if(_listRxAlerts[i].MedicationNum>0) {
					Medications.RefreshCache();
					Medication medication=Medications.GetMedication(_listRxAlerts[i].MedicationNum);
					if(medication!=null) {
						listAlerts.Items.Add(medication.MedName);
					}
				}
			}
		}

		private void listAlerts_DoubleClick(object sender,EventArgs e) {
			if(listAlerts.SelectedIndex==-1) {
				MsgBox.Show(this,"Select at least one Alert.");
				return;
			}
			using FormRxAlertEdit formRxAlertEdit=new FormRxAlertEdit(_listRxAlerts[listAlerts.SelectedIndex],_rxDef);
			formRxAlertEdit.ShowDialog();
			FillAlerts();
		}

		private void butAddProblem_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs();
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.IsMultiSelect=true;
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<formDiseaseDefs.ListDiseaseDefsSelected.Count;i++) {
				RxAlert rxAlert=new RxAlert();
				rxAlert.DiseaseDefNum=formDiseaseDefs.ListDiseaseDefsSelected[i].DiseaseDefNum;
				rxAlert.RxDefNum=_rxDef.RxDefNum;
				RxAlerts.Insert(rxAlert);
			}
			FillAlerts();
		}

		private void butAddMedication_Click(object sender,EventArgs e) {
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult!=DialogResult.OK) {
				return;
			}
			RxAlert rxAlert=new RxAlert();
			rxAlert.MedicationNum=formMedications.SelectedMedicationNum;
			rxAlert.RxDefNum=_rxDef.RxDefNum;
			RxAlerts.Insert(rxAlert);
			FillAlerts();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			using FormAllergySetup formAllergySetup=new FormAllergySetup();
			formAllergySetup.IsSelectionMode=true;
			formAllergySetup.ShowDialog();
			if(formAllergySetup.DialogResult!=DialogResult.OK) {
				return;
			}
			RxAlert rxAlert=new RxAlert();
			rxAlert.AllergyDefNum=formAllergySetup.AllergyDefNumSelected;
			rxAlert.RxDefNum=_rxDef.RxDefNum;
			RxAlerts.Insert(rxAlert);
			FillAlerts();
		}

		private void butRxNormSelect_Click(object sender,EventArgs e) {
			using FormRxNorms formRxNorms=new FormRxNorms();
			formRxNorms.IsSelectionMode=true;
			formRxNorms.InitSearchCodeOrDescript=textDrug.Text;
			formRxNorms.ShowDialog();
			if(formRxNorms.DialogResult!=DialogResult.OK) {
				return;
			}
			_rxDef.RxCui=PIn.Long(formRxNorms.RxNormSelected.RxCui);
			FillRxCui();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this prescription template?")){
				return;
			}
			RxDefs.Delete(_rxDef);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			//RxCui is set when butRxNormSelect is clicked.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US") && _rxDef.RxCui==0) {//United States
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning: RxNorm was not picked.  "
					+"RxNorm uniquely identifies drugs in the United States and helps you keep your medications organized.  "
					+"RxNorm is used to send information to and from eRx if you are using or plan to use eRx.\r\n"
					+"Click OK to continue without an RxNorm, or click Cancel to stay in this window."))
				{
					return;
				}
			}
			_rxDef.Drug=textDrug.Text;
			_rxDef.Sig=textSig.Text;
			_rxDef.Disp=textDisp.Text;
			_rxDef.Refills=textRefills.Text;
			_rxDef.Notes=textNotes.Text;
			_rxDef.IsControlled=checkControlled.Checked;
			_rxDef.IsProcRequired=checkProcRequired.Checked;
			_rxDef.PatientInstruction=textPatInstruction.Text;
			RxDefs.Update(_rxDef);
			DialogResult=DialogResult.OK;
		}

		private void FormRxDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK){
				return;//close as normal
			}
			if(IsNew){
				RxDefs.Delete(_rxDef);
			}
		}

	}
}