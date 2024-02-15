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
		private DiseaseDef _diseaseDef;

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
			_diseaseDef=DiseaseDefs.GetItem(FamilyHealthCur.DiseaseDefNum);
			//Validation is done when deleting diseaseDefs to make sure they are not in use by FamilyHealths.
			textProblem.Text=_diseaseDef.DiseaseName;
			textSnomed.Text=_diseaseDef.SnomedCode;
			textName.Text=FamilyHealthCur.PersonName;
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormDiseaseDefs formDiseaseDefs=new FormDiseaseDefs();
			formDiseaseDefs.IsSelectionMode=true;
			formDiseaseDefs.ShowDialog();
			if(formDiseaseDefs.DialogResult!=DialogResult.OK) {
				return;
			}
			//the list should only ever contain one item.
			DiseaseDef diseaseDef=formDiseaseDefs.ListDiseaseDefsSelected[0];
			textProblem.Text=diseaseDef.DiseaseName;
			textSnomed.Text=diseaseDef.SnomedCode;
			_diseaseDef=diseaseDef;
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
			SecurityLogs.MakeLogEntry(EnumPermType.PatFamilyHealthEdit,FamilyHealthCur.PatNum,FamilyHealthCur.PersonName+" "+FamilyHealthCur.Relationship+" deleted");
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(listRelationship.SelectedIndex<0) {
				MsgBox.Show(this,"Relationship required.");
				return;
			}
			if(textName.Text.Trim()=="") {
				MsgBox.Show(this,"Name required.");
				return;
			}
			if(_diseaseDef==null) {
				MsgBox.Show(this,"Problem required.");
				return;
			}
			FamilyHealthCur.DiseaseDefNum=_diseaseDef.DiseaseDefNum;
			FamilyHealthCur.Relationship=listRelationship.GetSelected<FamilyRelationship>();
			FamilyHealthCur.PersonName=textName.Text;
			if(FamilyHealthCur.IsNew) {
				SecurityLogs.MakeLogEntry(EnumPermType.PatFamilyHealthEdit,FamilyHealthCur.PatNum,FamilyHealthCur.PersonName+" "+FamilyHealthCur.Relationship+" added");
				FamilyHealths.Insert(FamilyHealthCur);
			}
			else {
				FamilyHealths.Update(FamilyHealthCur);
				SecurityLogs.MakeLogEntry(EnumPermType.PatFamilyHealthEdit,FamilyHealthCur.PatNum,FamilyHealthCur.PersonName+" "+FamilyHealthCur.Relationship+" edited");
			}
			DialogResult=DialogResult.OK;
		}

	}
}