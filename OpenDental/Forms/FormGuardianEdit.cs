using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormGuardianEdit:FormODBase {

		private Guardian _guardian;
		private Family _family;
		private List<string> _listGuardianRelationshipNames;

		public FormGuardianEdit(Guardian guardian,Family family){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_guardian=guardian;
			_family=family;
		}

		private void FormGuardianEdit_Load(object sender,EventArgs e) {
			textPatient.Text=_family.GetNameInFamFL(_guardian.PatNumChild);
			if(_guardian.PatNumGuardian!=0) {
				textFamilyMember.Text=_family.GetNameInFamFL(_guardian.PatNumGuardian);
			}
			Patient patientChild=Patients.GetPat(_guardian.PatNumChild);
			if(_guardian.IsNew) {
				if(patientChild.Position==PatientPosition.Child) {
					//True by default if entering relationship from a child patient, because this is how the old guardian feature worked before we added support for other relationship types.
					checkIsGuardian.Checked=true;
				}
			}
			else { //Existing guardian record.
				checkIsGuardian.Checked=_guardian.IsGuardian;
			}
			_listGuardianRelationshipNames=new List<string>(Enum.GetNames(typeof(GuardianRelationship)));
			_listGuardianRelationshipNames.Sort();
			for(int i=0;i<_listGuardianRelationshipNames.Count;i++){
				comboRelationship.Items.Add(Lan.g("enumGuardianRelationship",_listGuardianRelationshipNames[i]));
				if(_listGuardianRelationshipNames[i]==_guardian.Relationship.ToString()) {
					comboRelationship.SelectedIndex=i;
				}
			}
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormFamilyMemberSelect formFamilyMemberSelect=new FormFamilyMemberSelect(_family);
			formFamilyMemberSelect.ShowDialog();
			if(formFamilyMemberSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_guardian.PatNumGuardian=formFamilyMemberSelect.SelectedPatNum;
			textFamilyMember.Text=_family.GetNameInFamFL(_guardian.PatNumGuardian);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_guardian.IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				Guardians.Delete(_guardian.GuardianNum);
				DialogResult=DialogResult.OK;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			//PatNumChild already set
			//PatNumGuardian already set
			_guardian.IsGuardian=checkIsGuardian.Checked;
			string guardianRelationshipName=comboRelationship.GetSelected<string>();
			List <string> listRelationshipNamesRaw=new List<string>(Enum.GetNames(typeof(GuardianRelationship)));
			_guardian.Relationship=(GuardianRelationship)listRelationshipNamesRaw.IndexOf(guardianRelationshipName);
			if(_guardian.IsNew) {
				Guardians.Insert(_guardian);
			}
			else {
				Guardians.Update(_guardian);
			}
			DialogResult=DialogResult.OK;
		}
		
	}
}