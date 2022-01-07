using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormMedPat : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public MedicationPat MedicationPatCur;
		///<summary>Helps enforce the note and start date that must be present for a med order to be valid.</summary>
		public bool IsNewMedOrder;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormMedPat()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedPat_Load(object sender, System.EventArgs e) {
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				labelPatNote.Text="Count, Instructions, and Refills";
				groupOrder.Text="Medication Order";
			}
			//Formulary checks now handled in NewCrop
			//else {
			//	butFormulary.Visible=false;
			//}
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				textRxNormDesc.Text=RxNorms.GetDescByRxCui(MedicationPatCur.RxCui.ToString());
				if(Erx.IsFromErx(MedicationPatCur.ErxGuid)) {//This is a medical order that was automatically created when fetching prescriptions from eRx.
					//We allow the user to change the RxCui on the medical order, because eRx does not always supply an RxCui.
					//Also the MedicaitonNum on the order will always be 0, so there is no link between the order and the medication table.
					//If MedicationOrder was non-zero and we changed the RxCui on the order, then the automatic sync between medicationpat.RxCui and medication.RxCui would be tainted.
					butRxNormSelect.Visible=true;
				}
			}
			else {
				labelRxNorm.Visible=false;
				textRxNormDesc.Visible=false;
			}
			if(MedicationPatCur.MedicationNum==0) {//Probably a medical order created during eRx prescription fetching, but not necessarily.
				textMedName.Text=MedicationPatCur.MedDescript;
				labelGenericNotes.Visible=false;
				labelGenericName.Visible=false;
				textGenericName.Visible=false;
				textMedNote.Visible=false;
				butEdit.Visible=false;
				labelEdit.Visible=false;
			}
			else {
				textMedName.Text=Medications.GetMedication(MedicationPatCur.MedicationNum).MedName;
				textGenericName.Text=Medications.GetGeneric(MedicationPatCur.MedicationNum).MedName;
				textMedNote.Text=Medications.GetGeneric(MedicationPatCur.MedicationNum).Notes;
			}
			comboProv.Items.Add(Lan.g(this,"none"));
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(MedicationPatCur.ProvNum==0){
				comboProv.SelectedIndex=0;
			}
			else{
				for(int i=0;i<_listProviders.Count;i++) {
					if(MedicationPatCur.ProvNum==_listProviders[i].ProvNum) {
						comboProv.SelectedIndex=i+1;
					}
				}
				//if a provider was subsequently hidden, then the combobox may now be -1.
			}
			if(MedicationPatCur.IsCpoe) {
				//We cannot allow the user to change the provider, because our EHR reports use the provider in combination with the IsCpoe flag to report CPOE.
				comboProv.Enabled=false;
			}
			textPatNote.Text=MedicationPatCur.PatNote;
			if(MedicationPatCur.DateStart.Year>1880) {
				textDateStart.Text=MedicationPatCur.DateStart.ToShortDateString();
			}
			if(MedicationPatCur.DateStop.Year>1880) {
				textDateStop.Text=MedicationPatCur.DateStop.ToShortDateString();
			}
			if(IsNew) {
				butEdit.Visible=false;
				labelEdit.Visible=false;
			}
		}

		private void butTodayStart_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.ToShortDateString();
		}

		private void butTodayStop_Click(object sender,EventArgs e) {
			textDateStop.Text=DateTime.Today.ToShortDateString();
		}

		//New Crop now handles formulary checks
		/*private void butFormulary_Click(object sender,EventArgs e) {
			using FormFormularies FormF=new FormFormularies();
			FormF.IsSelectionMode=true;
			FormF.ShowDialog();
			Cursor=Cursors.WaitCursor;
			if(FormF.DialogResult!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			List<FormularyMed> ListMeds=FormularyMeds.GetMedsForFormulary(FormF.SelectedFormularyNum);
			bool medIsInFormulary=false;
			for(int i=0;i<ListMeds.Count;i++) {
				if(ListMeds[i].MedicationNum==MedicationPatCur.MedicationNum) {
					medIsInFormulary=true;
				}
			}
			Cursor=Cursors.Default;
			if(medIsInFormulary){
				MsgBox.Show(this,"This medication is in the selected formulary.");
			}
			else {
				MsgBox.Show(this,"This medication is not in the selected forumulary.");
			}
		}*/

		private void butRxNormSelect_Click(object sender,EventArgs e) {
			using FormRxNorms formRxNorms=new FormRxNorms();
			formRxNorms.IsSelectionMode=true;
			formRxNorms.InitSearchCodeOrDescript=textMedName.Text;
			formRxNorms.ShowDialog();
			if(formRxNorms.DialogResult!=DialogResult.OK) {
				return;
			}
			MedicationPatCur.RxCui=PIn.Long(formRxNorms.SelectedRxNorm.RxCui);
			textRxNormDesc.Text=RxNorms.GetDescByRxCui(MedicationPatCur.RxCui.ToString());
			if(IsNew) {
				textMedName.Text=RxNorms.GetDescByRxCui(MedicationPatCur.RxCui.ToString());
			}
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			Medications.RefreshCache();
			using FormMedicationEdit formMedicationEdit=new FormMedicationEdit();
			Medication medication=Medications.GetMedication(MedicationPatCur.MedicationNum);//The edit button is not visible if MedicationNum=0.
			if(medication==null) {//Possible to delete the medication from a separate WS while medication loaded in memory.
				MsgBox.Show(this,"An error occurred loading medication.");
				return;
			}
			formMedicationEdit.MedicationCur=medication;
			formMedicationEdit.ShowDialog();
			if(formMedicationEdit.DialogResult!=DialogResult.OK){
				return;
			}
			MedicationPatCur.RxCui=formMedicationEdit.MedicationCur.RxCui;
			textMedName.Text=Medications.GetMedication(MedicationPatCur.MedicationNum).MedName;//The edit button is not visible if MedicationNum=0.
			textGenericName.Text=Medications.GetGeneric(MedicationPatCur.MedicationNum).MedName;//The edit button is not visible if MedicationNum=0.
			textMedNote.Text=Medications.GetGeneric(MedicationPatCur.MedicationNum).Notes;//The edit button is not visible if MedicationNum=0.
			textRxNormDesc.Text=RxNorms.GetDescByRxCui(MedicationPatCur.RxCui.ToString());
		}

		private void butRemove_Click(object sender, System.EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove this medication from this patient?  Patient notes will be lost.")){
				return;
			}
			MedicationPats.Delete(MedicationPatCur);
			if(MedicationPatCur.MedicationNum==0) {
				SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,MedicationPatCur.PatNum,MedicationPatCur.MedDescript+" deleted");
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,MedicationPatCur.PatNum,Medications.GetMedication(MedicationPatCur.MedicationNum).MedName+" deleted");
			}
			MedicationPatCur=null;//This prevents other windows trying to use the medication pat after this window has closed.
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDateStart.IsValid() || !textDateStop.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(IsNewMedOrder) {
				if(textPatNote.Text=="" || textDateStart.Text=="") {
					MsgBox.Show(this,"For a new medical order, instructions and a start date are required.");
					return;
				}
			}
			//MedicationPatCur.MedicationNum is already set before entering this window, or else changed up above.
			if(comboProv.SelectedIndex==-1) {
				//don't make any changes to provnum.  ProvNum is a hidden prov.
			}
			else if(comboProv.SelectedIndex==0){
				MedicationPatCur.ProvNum=0;
			}
			else {
				MedicationPatCur.ProvNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			MedicationPatCur.PatNote=textPatNote.Text;
			MedicationPatCur.DateStart=PIn.Date(textDateStart.Text);
			MedicationPatCur.DateStop=PIn.Date(textDateStop.Text);
			if(IsNew){
				MedicationPats.Insert(MedicationPatCur);
				if(MedicationPatCur.MedicationNum==0) {
					SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,MedicationPatCur.PatNum,MedicationPatCur.MedDescript+" added");
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,MedicationPatCur.PatNum,Medications.GetMedication(MedicationPatCur.MedicationNum).MedName+" added");
				}
			}
			else{
				MedicationPats.Update(MedicationPatCur);
				if(MedicationPatCur.MedicationNum==0) {
					SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,MedicationPatCur.PatNum,MedicationPatCur.MedDescript+" edited");
				}
				else {
					SecurityLogs.MakeLogEntry(Permissions.PatMedicationListEdit,MedicationPatCur.PatNum,Medications.GetMedication(MedicationPatCur.MedicationNum).MedName+" edited");
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















