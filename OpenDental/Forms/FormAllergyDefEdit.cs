using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAllergyDefEdit:FormODBase {
		public AllergyDef AllergyDefCur;

		public FormAllergyDefEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAllergyDefEdit_Load(object sender,EventArgs e) {
			textDescription.Text=AllergyDefCur?.Description??"";//set description if available. New allergies can be added with descriptions. 
			if(!AllergyDefCur.IsNew) { 
				checkHidden.Checked=AllergyDefCur.IsHidden;
			}
			for(int i=0;i<Enum.GetNames(typeof(SnomedAllergy)).Length;i++) {
				comboSnomedAllergyType.Items.Add(Enum.GetNames(typeof(SnomedAllergy))[i]);
			}
			comboSnomedAllergyType.SelectedIndex=(int)AllergyDefCur.SnomedType;
			textMedication.Text=Medications.GetDescription(AllergyDefCur.MedicationNum);
			textUnii.Text=AllergyDefCur.UniiCode;
			if(!Security.IsAuthorized(Permissions.AllergyDefEdit)) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
		}

		private void butUniiToSelect_Click(object sender,EventArgs e) {
			//using FormSnomeds formSnowmeds=new FormSnomeds();
			//formSnowmeds.IsSelectionMode=true;
			//if(formSnowmeds.ShowDialog()==DialogResult.OK) {
			//	snomedAllergicTo=formSnowmeds.SelectedSnomed;
			//	//textSnomedAllergicTo.Text=snomedAllergicTo.Description;
			//}
			//TODO: Implement similar code for Unii
		}

		private void butMedicationSelect_Click(object sender,EventArgs e) {
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult!=DialogResult.OK){
				return;
			}
			AllergyDefCur.MedicationNum=formMedications.SelectedMedicationNum;
			textMedication.Text=Medications.GetDescription(AllergyDefCur.MedicationNum);
		}

		private void butNoneUniiTo_Click(object sender,EventArgs e) {
			//TODO: Implement this
		}

		private void butNone_Click(object sender,EventArgs e) {
			AllergyDefCur.MedicationNum=0;
			textMedication.Text="";
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text.Trim()=="") {
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			if(textUnii.Text!="" && textMedication.Text!="") {
				MsgBox.Show(this,"Only one code is allowed per allergy def.");
				return;
			}
			string validChars="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<textUnii.Text.Length;i++) {
				if(validChars.IndexOf(textUnii.Text[i])==-1) {//Not found.
					stringBuilder.Append(textUnii.Text[i]);
				}
			}
			if(stringBuilder.ToString()!="") {
				MessageBox.Show(Lan.g(this,"UNII code has invalid characters: ")+stringBuilder);
				return;
			}
			if(textUnii.Text!="" && textUnii.Text.Length!=10) {
				MsgBox.Show(this,"UNII code must be 10 characters in length.");
				return;
			}
			AllergyDefCur.Description=textDescription.Text;
			AllergyDefCur.IsHidden=checkHidden.Checked;
			AllergyDefCur.SnomedType=(SnomedAllergy)comboSnomedAllergyType.SelectedIndex;
			AllergyDefCur.UniiCode=textUnii.Text;
			//if(snomedAllergicTo!=null) { //TODO: Do UNII check once the table is added
			//	AllergyDefCur.SnomedAllergyTo=snomedAllergicTo.SnomedCode;
			//}
			if(AllergyDefCur.IsNew) {
				AllergyDefs.Insert(AllergyDefCur);
			}
			else {
				AllergyDefs.Update(AllergyDefCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(AllergyDefCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(AllergyDefs.DefIsInUse(AllergyDefCur.AllergyDefNum)) {
				MsgBox.Show(this,"Cannot delete allergies in use.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Allergy?")) {
				return;
			}
			AllergyDefs.Delete(AllergyDefCur.AllergyDefNum);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}