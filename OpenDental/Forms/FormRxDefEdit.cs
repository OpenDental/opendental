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
		private RxDef RxDefCur;
		private List<RxAlert> RxAlertList;

		///<summary>Must have already saved it to db so that we have a RxDefNum to work with.</summary>
		public FormRxDefEdit(RxDef rxDefCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
			RxDefCur=rxDefCur.Copy();
		}

		private void FormRxDefEdit_Load(object sender, System.EventArgs e) {
			textDrug.Text=RxDefCur.Drug;
			textSig.Text=RxDefCur.Sig;
			textDisp.Text=RxDefCur.Disp;
			textRefills.Text=RxDefCur.Refills;
			textNotes.Text=RxDefCur.Notes;
			textPatInstruction.Text=RxDefCur.PatientInstruction;
			checkControlled.Checked=RxDefCur.IsControlled;
			if(PrefC.GetBool(PrefName.RxHasProc)) {
				checkProcRequired.Enabled=true;
				checkProcRequired.Checked=RxDefCur.IsProcRequired;
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
			if(RxDefCur.RxCui==0) {
				textRxCui.Text="";
			}
			else {
				textRxCui.Text=RxDefCur.RxCui.ToString()+" - "+RxNorms.GetDescByRxCui(RxDefCur.RxCui.ToString());
			}
		}

		private void FillAlerts(){
			RxAlertList=RxAlerts.Refresh(RxDefCur.RxDefNum);
			listAlerts.Items.Clear();
			for(int i=0;i<RxAlertList.Count;i++) {
				if(RxAlertList[i].DiseaseDefNum>0) {
					listAlerts.Items.Add(DiseaseDefs.GetName(RxAlertList[i].DiseaseDefNum));
				}
				if(RxAlertList[i].AllergyDefNum>0) {
					AllergyDef allergyDef=AllergyDefs.GetOne(RxAlertList[i].AllergyDefNum);
					if(allergyDef!=null) {
						listAlerts.Items.Add(allergyDef.Description);
					}
				}
				if(RxAlertList[i].MedicationNum>0) {
					Medications.RefreshCache();
					Medication med=Medications.GetMedication(RxAlertList[i].MedicationNum);
					if(med!=null) {
						listAlerts.Items.Add(med.MedName);
					}
				}
			}
		}

		private void listAlerts_DoubleClick(object sender,EventArgs e) {
			if(listAlerts.SelectedIndex==-1) {
				MsgBox.Show(this,"Select at least one Alert.");
				return;
			}
			using FormRxAlertEdit FormRAE=new FormRxAlertEdit(RxAlertList[listAlerts.SelectedIndex],RxDefCur);
			FormRAE.ShowDialog();
			FillAlerts();
		}

		private void butAddProblem_Click(object sender,EventArgs e) {
			using FormDiseaseDefs FormD=new FormDiseaseDefs();
			FormD.IsSelectionMode=true;
			FormD.IsMultiSelect=true;
			FormD.ShowDialog();
			if(FormD.DialogResult!=DialogResult.OK) {
				return;
			}
			for(int i=0;i<FormD.ListDiseaseDefsSelected.Count;i++) {
				RxAlert alert=new RxAlert();
				alert.DiseaseDefNum=FormD.ListDiseaseDefsSelected[i].DiseaseDefNum;
				alert.RxDefNum=RxDefCur.RxDefNum;
				RxAlerts.Insert(alert);
			}
			FillAlerts();
		}

		private void butAddMedication_Click(object sender,EventArgs e) {
			using FormMedications FormMED=new FormMedications();
			FormMED.IsSelectionMode=true;
			FormMED.ShowDialog();
			if(FormMED.DialogResult!=DialogResult.OK) {
				return;
			}
			RxAlert alert=new RxAlert();
			alert.MedicationNum=FormMED.SelectedMedicationNum;
			alert.RxDefNum=RxDefCur.RxDefNum;
			RxAlerts.Insert(alert);
			FillAlerts();
		}

		private void butAddAllergy_Click(object sender,EventArgs e) {
			using FormAllergySetup FormAS=new FormAllergySetup();
			FormAS.IsSelectionMode=true;
			FormAS.ShowDialog();
			if(FormAS.DialogResult!=DialogResult.OK) {
				return;
			}
			RxAlert alert=new RxAlert();
			alert.AllergyDefNum=FormAS.SelectedAllergyDefNum;
			alert.RxDefNum=RxDefCur.RxDefNum;
			RxAlerts.Insert(alert);
			FillAlerts();
		}

		private void butRxNormSelect_Click(object sender,EventArgs e) {
			using FormRxNorms FormRN=new FormRxNorms();
			FormRN.IsSelectionMode=true;
			FormRN.InitSearchCodeOrDescript=textDrug.Text;
			FormRN.ShowDialog();
			if(FormRN.DialogResult!=DialogResult.OK) {
				return;
			}
			RxDefCur.RxCui=PIn.Long(FormRN.SelectedRxNorm.RxCui);
			FillRxCui();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this prescription template?")){
				return;
			}
			RxDefs.Delete(RxDefCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//RxCui is set when butRxNormSelect is clicked.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US") && RxDefCur.RxCui==0) {//United States
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning: RxNorm was not picked.  "
					+"RxNorm uniquely identifies drugs in the United States and helps you keep your medications organized.  "
					+"RxNorm is used to send information to and from eRx if you are using or plan to use eRx.\r\n"
					+"Click OK to continue without an RxNorm, or click Cancel to stay in this window."))
				{
					return;
				}
			}
			RxDefCur.Drug=textDrug.Text;
			RxDefCur.Sig=textSig.Text;
			RxDefCur.Disp=textDisp.Text;
			RxDefCur.Refills=textRefills.Text;
			RxDefCur.Notes=textNotes.Text;
			RxDefCur.IsControlled=checkControlled.Checked;
			RxDefCur.IsProcRequired=checkProcRequired.Checked;
			RxDefCur.PatientInstruction=textPatInstruction.Text;
			RxDefs.Update(RxDefCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormRxDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK){
				return;//close as normal
			}
			if(IsNew){
				RxDefs.Delete(RxDefCur);
			}
		}
	}
}
