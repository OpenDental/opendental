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
		private Medication _medFrom;
		private Medication _medInto;

		public FormMedicationMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void CheckUIState() {
			butMerge.Enabled=(textMedNumFrom.Text.Trim()!="" && textMedNumInto.Text.Trim()!="");
		}
		
		private void butChangeMedInto_Click(object sender,EventArgs e) {
			using FormMedications FormM=new FormMedications();
			FormM.IsSelectionMode=true;
			FormM.ShowDialog();
			if(FormM.DialogResult==DialogResult.OK) {
				_medInto=Medications.GetMedication(FormM.SelectedMedicationNum);
				textGenNumInto.Text=POut.Long(_medInto.GenericNum);
				textMedNameInto.Text=_medInto.MedName;
				textMedNumInto.Text=POut.Long(_medInto.MedicationNum);
				textRxInto.Text=POut.Long(_medInto.RxCui);
			}
			CheckUIState();
		}

		private void butChangeMedFrom_Click(object sender,EventArgs e) {
			using FormMedications FormM=new FormMedications();
			FormM.IsSelectionMode=true;
			FormM.ShowDialog();
			if(FormM.DialogResult==DialogResult.OK) {
				_medFrom=Medications.GetMedication(FormM.SelectedMedicationNum);
				textGenNumFrom.Text=POut.Long(_medFrom.GenericNum);
				textMedNameFrom.Text=_medFrom.MedName;
				textMedNumFrom.Text=POut.Long(_medFrom.MedicationNum);
				textRxFrom.Text=POut.Long(_medFrom.RxCui);
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
			if(_medFrom.MedicationNum==_medFrom.GenericNum && Medications.IsInUseAsGeneric(_medFrom)) {
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
			long numPats=Medications.CountPats(_medFrom.MedicationNum);
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
			long rowsChanged=Medications.Merge(_medFrom.MedicationNum,_medInto.MedicationNum);
			string logText=Lan.g(this,"Medications merged")+": "+_medFrom.MedName+" "+Lan.g(this,"merged into")+" "+_medInto.MedName+".\r\n"
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