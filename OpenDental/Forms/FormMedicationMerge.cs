using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMedicationMerge:FormODBase {
		private Medication _medicationFrom;
		private Medication _medicationInto;

		public FormMedicationMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void CheckUIState() {
			butMerge.Enabled=(textMedNumFrom.Text.Trim()!="" && textMedNumInto.Text.Trim()!="");
		}
		
		private void butChangeMedInto_Click(object sender,EventArgs e) {
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult==DialogResult.OK) {
				_medicationInto=Medications.GetMedication(formMedications.SelectedMedicationNum);
				textGenNumInto.Text=POut.Long(_medicationInto.GenericNum);
				textMedNameInto.Text=_medicationInto.MedName;
				textMedNumInto.Text=POut.Long(_medicationInto.MedicationNum);
				textRxInto.Text=POut.Long(_medicationInto.RxCui);
			}
			CheckUIState();
		}

		private void butChangeMedFrom_Click(object sender,EventArgs e) {
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult==DialogResult.OK) {
				_medicationFrom=Medications.GetMedication(formMedications.SelectedMedicationNum);
				textGenNumFrom.Text=POut.Long(_medicationFrom.GenericNum);
				textMedNameFrom.Text=_medicationFrom.MedName;
				textMedNumFrom.Text=POut.Long(_medicationFrom.MedicationNum);
				textRxFrom.Text=POut.Long(_medicationFrom.RxCui);
			}
			CheckUIState();
		}

		private void butMerge_Click(object sender,EventArgs e) {
			string differentFields="";
			string msgText="";
			if(textMedNumInto.Text==textMedNumFrom.Text) {
				//do not attempt a merge if the same medication was selected twice, or if one of the fields is blank.
				MsgBox.Show(this,"You must select two different medications to merge.");
				return;
			}
			if(_medicationFrom.MedicationNum==_medicationFrom.GenericNum && Medications.IsInUseAsGeneric(_medicationFrom)) {
				msgText=Lan.g(this,"The medication you are merging from is a generic medication associated with other brands")+". "+
					Lan.g(this,"Select a different medication to merge from instead.")+".";
				MessageBox.Show(msgText);
				return;
			}
			if(textMedNameFrom.Text!=textMedNameInto.Text) {
				differentFields+="\r\n"+Lan.g(this,"Medication Name");
			}
			if(textGenNumFrom.Text!=textGenNumInto.Text) {
				differentFields+="\r\n"+Lan.g(this,"GenericNum");
			}
			if(textRxFrom.Text!=textRxInto.Text) {
				differentFields+="\r\n"+Lan.g(this,"RxCui");
			}
			long numPats=Medications.CountPats(_medicationFrom.MedicationNum);
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure?  The results are permanent and cannot be undone.")) {
				return;
			}
			msgText="";
			if(differentFields!="") {
				msgText=Lan.g(this,"The following medication fields do not match")+": "+differentFields+"\r\n";
			}
			msgText+=Lan.g(this,"This change is irreversible")+".  "+Lan.g(this,"This medication is assigned to")+" "+numPats+" "
				+Lan.g(this,"patients")+".  "+Lan.g(this,"Continue anyways?");
			if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
				return;
			}
			long rowsChanged=Medications.Merge(_medicationFrom.MedicationNum,_medicationInto.MedicationNum);
			string logText=Lan.g(this,"Medications merged")+": "+_medicationFrom.MedName+" "+Lan.g(this,"merged into")+" "+_medicationInto.MedName+".\r\n"
			+Lan.g(this,"Rows changed")+": "+POut.Long(rowsChanged);
			SecurityLogs.MakeLogEntry(Permissions.MedicationMerge,0,logText);
			textRxFrom.Clear();
			textMedNumFrom.Clear();
			textMedNameFrom.Clear();
			textGenNumFrom.Clear();
			MsgBox.Show(this,"Done.");
			DataValid.SetInvalid(InvalidType.Medications);
			CheckUIState();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}