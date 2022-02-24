using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormMedicationEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary></summary>
		public bool GenericOnly;
		///<summary></summary>
		private string[] _patNameMeds;
		private string[] _patNameAllergies;
		///<summary></summary>
		private string[] Brands;
		public Medication MedicationCur;

		///<summary></summary>
		public FormMedicationEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void MedicationEdit_Load(object sender, System.EventArgs e) {
			//Medications.RefreshCache() should have already been run.
			FillForm();
		}

		private void FillForm() {
			textMedName.Text=MedicationCur.MedName;
			if(!IsNew) {
				textMedName.ReadOnly=true;
			}
			if(MedicationCur.GenericNum==0) {
				//Probably occurred from a previous bug.  This makes sure we have a generic num that is not 0. 
				MedicationCur.GenericNum=MedicationCur.MedicationNum;
				MsgBox.Show(this, "This medication had a missing generic name.  The generic name has been set to the medication name.");
			}
			if(MedicationCur.MedicationNum==MedicationCur.GenericNum) {
				textGenericName.Text=MedicationCur.MedName;
				textNotes.Text=MedicationCur.Notes;
				textNotes.ReadOnly=false;
				Brands=Medications.GetBrands(MedicationCur.MedicationNum);
				comboBrands.Items.Clear();
				comboBrands.Items.AddRange(Brands);
				if(Brands.Length>0) {
					comboBrands.SelectedIndex=0;
				}
			}
			else {
				textGenericName.Text=Medications.GetMedication(MedicationCur.GenericNum).MedName;
				textNotes.Text=Medications.GetMedication(MedicationCur.GenericNum).Notes;
				textNotes.ReadOnly=true;
				Brands=new string[0];
				comboBrands.Visible=false;
				labelBrands.Visible=false;
			}
			_patNameMeds=Medications.GetPatNamesForMed(MedicationCur.MedicationNum);
			comboPatients.Items.Clear();
			comboPatients.Items.AddRange(_patNameMeds);
			if(_patNameMeds.Length>0) {
				comboPatients.SelectedIndex=0;
			}
			AllergyDef alD=AllergyDefs.GetAllergyDefFromMedication(MedicationCur.MedicationNum);
			if(alD!=null) {
				_patNameAllergies=Allergies.GetPatNamesForAllergy(alD.AllergyDefNum);
				comboPatientAllergy.Items.Clear();
				comboPatientAllergy.Items.AddRange(_patNameAllergies);
				if(_patNameAllergies.Length>0) {
					comboPatientAllergy.SelectedIndex=0;
				}
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				textRxNormDesc.Text=RxNorms.GetDescByRxCui(MedicationCur.RxCui.ToString());
			}
			else {
				labelRxNorm.Visible=false;
				textRxNormDesc.Visible=false;
				butRxNormSelect.Visible=false;
			}
		}
	
		private void textMedName_TextChanged(object sender, System.EventArgs e) {
			//this causes immediate display update with each keypress
			if(MedicationCur.MedicationNum==MedicationCur.GenericNum){
				textGenericName.Text=textMedName.Text;
			}
		}

		private void butRxNorm_Click(object sender,EventArgs e) {
			using FormRxNorms FormRN=new FormRxNorms();
			FormRN.IsSelectionMode=true;
			FormRN.InitSearchCodeOrDescript=textMedName.Text;
			FormRN.ShowDialog();
			if(FormRN.DialogResult!=DialogResult.OK) {
				return;
			}
			MedicationCur.RxCui=PIn.Long(FormRN.SelectedRxNorm.RxCui);
			textRxNormDesc.Text=RxNorms.GetDescByRxCui(MedicationCur.RxCui.ToString());
			if(IsNew) {
				textMedName.Text=RxNorms.GetDescByRxCui(MedicationCur.RxCui.ToString());
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!IsNew) {//Only ask user if they want to delete if not new.
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete this medication?")) {
					return;
				}
			}
			try {
				Medications.Delete(MedicationCur);
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
				return;
			}
			DataValid.SetInvalid(InvalidType.Medications);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//generic num already handled
			MedicationCur.MedName=textMedName.Text;
			if(MedicationCur.MedName=="") {
				MsgBox.Show(this,"Not allowed to save a medication without a Drug Name.");
				return;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				if(MedicationCur.RxCui==0 && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning: RxNorm was not picked.  "
					+"RxNorm uniquely identifies drugs in the United States and helps you keep your medications organized.  "
					+"RxNorm is used to send information to and from eRx if you are using or plan to use eRx.\r\n"
					+"Click OK to continue without an RxNorm, or click Cancel to stay in this window."))
				{
					return;
				}
				else if(MedicationCur.RxCui!=0) {
					List <Medication> listExistingMeds=Medications.GetAllMedsByRxCui(MedicationCur.RxCui);
					if(listExistingMeds.FindAll(x => x.MedicationNum!=MedicationCur.MedicationNum).Count > 0) {
						MsgBox.Show(this,"A medication in the medication list is already using the selected RxNorm.\r\n"
							+"Please select a different RxNorm or use the other medication instead.");
						return;
					}
				}
			}
			if(MedicationCur.MedicationNum==MedicationCur.GenericNum){
				MedicationCur.Notes=textNotes.Text;
			}
			else{
				MedicationCur.Notes="";
			}
			//MedicationCur has its RxCui set when the butRxNormSelect button is pressed.
			//The following behavior must match what happens when the user clicks the RxNorm column in FormMedications to pick RxCui.
			Medications.Update(MedicationCur);
			MedicationPats.UpdateRxCuiForMedication(MedicationCur.MedicationNum,MedicationCur.RxCui);
			DataValid.SetInvalid(InvalidType.Medications);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormMedicationEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK)
				return;
			if(IsNew){
				try {
					Medications.Delete(MedicationCur);
				}
				catch {
					MsgBox.Show(this,"The medication failed to delete due to existing dependencies.");
				}
			}
		}

		

		

		

		

		


	}
}





















