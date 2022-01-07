using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormFamilyHealthEdit:FormODBase {
		public FamilyHealth FamilyHealthCur;
		private DiseaseDef DisDefCur;

		public FormFamilyHealthEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFamilyHealthEdit_Load(object sender,EventArgs e) {
			listRelationship.Items.AddEnums<FamilyRelationship>();
			listRelationship.SelectedIndex=(int)FamilyHealthCur.Relationship;
			if(FamilyHealthCur.IsNew) {
				return; //Don't need to set any of the info below.  All null.
			}
			DisDefCur=DiseaseDefs.GetItem(FamilyHealthCur.DiseaseDefNum);
			//Validation is done when deleting diseaseDefs to make sure they are not in use by FamilyHealths.
			textProblem.Text=DisDefCur.DiseaseName;
			textSnomed.Text=DisDefCur.SnomedCode;
			textName.Text=FamilyHealthCur.PersonName;
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormDiseaseDefs FormD=new FormDiseaseDefs();
			FormD.IsSelectionMode=true;
			FormD.ShowDialog();
			if(FormD.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			DiseaseDef disDef=FormD.ListDiseaseDefsSelected[0];
			if(disDef.SnomedCode=="") {
				MsgBox.Show(this,"Selection must have a SNOMED CT code associated");
				return;
			}
			textProblem.Text=disDef.DiseaseName;
			textSnomed.Text=disDef.SnomedCode;
			DisDefCur=disDef;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(FamilyHealthCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			FamilyHealths.Delete(FamilyHealthCur.FamilyHealthNum);
			SecurityLogs.MakeLogEntry(Permissions.PatFamilyHealthEdit,FamilyHealthCur.PatNum,FamilyHealthCur.PersonName+" "+FamilyHealthCur.Relationship+" deleted");
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listRelationship.SelectedIndex<0) {
				MsgBox.Show(this,"Relationship required.");
				return;
			}
			if(textName.Text.Trim()=="") {
				MsgBox.Show(this,"Name required.");
				return;
			}
			if(DisDefCur==null) {
				MsgBox.Show(this,"Problem required.");
				return;
			}
			FamilyHealthCur.DiseaseDefNum=DisDefCur.DiseaseDefNum;
			FamilyHealthCur.Relationship=listRelationship.GetSelected<FamilyRelationship>();
			FamilyHealthCur.PersonName=textName.Text;
			if(FamilyHealthCur.IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.PatFamilyHealthEdit,FamilyHealthCur.PatNum,FamilyHealthCur.PersonName+" "+FamilyHealthCur.Relationship+" added");
				FamilyHealths.Insert(FamilyHealthCur);
			}
			else {
				FamilyHealths.Update(FamilyHealthCur);
				SecurityLogs.MakeLogEntry(Permissions.PatFamilyHealthEdit,FamilyHealthCur.PatNum,FamilyHealthCur.PersonName+" "+FamilyHealthCur.Relationship+" edited");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}