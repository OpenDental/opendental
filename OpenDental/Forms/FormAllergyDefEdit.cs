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

		private void FormAllergyEdit_Load(object sender,EventArgs e) {
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
		}

		private void butUniiToSelect_Click(object sender,EventArgs e) {
			//using FormSnomeds formS=new FormSnomeds();
			//formS.IsSelectionMode=true;
			//if(formS.ShowDialog()==DialogResult.OK) {
			//	snomedAllergicTo=formS.SelectedSnomed;
			//	//textSnomedAllergicTo.Text=snomedAllergicTo.Description;
			//}
			//TODO: Implement similar code for Unii
		}

		private void butMedicationSelect_Click(object sender,EventArgs e) {
			using FormMedications FormM=new FormMedications();
			FormM.IsSelectionMode=true;
			FormM.ShowDialog();
			if(FormM.DialogResult!=DialogResult.OK){
				return;
			}
			AllergyDefCur.MedicationNum=FormM.SelectedMedicationNum;
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
			StringBuilder notAllowed=new StringBuilder();
			for(int i=0;i<textUnii.Text.Length;i++) {
				if(validChars.IndexOf(textUnii.Text[i])==-1) {//Not found.
					notAllowed.Append(textUnii.Text[i]);
				}
			}
			if(notAllowed.ToString()!="") {
				MessageBox.Show(Lan.g(this,"UNII code has invalid characters: ")+notAllowed);
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
			if(!AllergyDefCur.IsNew) {
				if(!AllergyDefs.DefIsInUse(AllergyDefCur.AllergyDefNum)) {
					AllergyDefs.Delete(AllergyDefCur.AllergyDefNum);
				}
				else {
					MsgBox.Show(this,"Cannot delete allergies in use.");
					return;
				}
			}
			DialogResult=DialogResult.Cancel;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}