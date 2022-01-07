using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormGuardianEdit:FormODBase {

		private Guardian _guardianCur;
		private Family _fam;
		private List<string> _listRelationshipNames;

		public FormGuardianEdit(Guardian guardianCur,Family fam){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_guardianCur=guardianCur;
			_fam=fam;
		}

		private void FormGuardianEdit_Load(object sender,EventArgs e) {
			textPatient.Text=_fam.GetNameInFamFL(_guardianCur.PatNumChild);
			if(_guardianCur.PatNumGuardian!=0) {
				textFamilyMember.Text=_fam.GetNameInFamFL(_guardianCur.PatNumGuardian);
			}
			Patient patChild=Patients.GetPat(_guardianCur.PatNumChild);
			if(_guardianCur.IsNew) {
				if(patChild.Position==PatientPosition.Child) {
					//True by default if entering relationship from a child patient, because this is how the old guardian feature worked before we added support for other relationship types.
					checkIsGuardian.Checked=true;
				}
			}
			else { //Existing guardian record.
				checkIsGuardian.Checked=_guardianCur.IsGuardian;
			}
			_listRelationshipNames=new List<string>(Enum.GetNames(typeof(GuardianRelationship)));
			_listRelationshipNames.Sort();
			for(int i=0;i<_listRelationshipNames.Count;i++){
				comboRelationship.Items.Add(Lan.g("enumGuardianRelationship",_listRelationshipNames[i]));
				if(_listRelationshipNames[i]==_guardianCur.Relationship.ToString()) {
					comboRelationship.SelectedIndex=i;
				}
			}
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormFamilyMemberSelect FormF=new FormFamilyMemberSelect(_fam);
			FormF.ShowDialog();
			if(FormF.DialogResult!=DialogResult.OK) {
				return;
			}
			_guardianCur.PatNumGuardian=FormF.SelectedPatNum;
			textFamilyMember.Text=_fam.GetNameInFamFL(_guardianCur.PatNumGuardian);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_guardianCur.IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				Guardians.Delete(_guardianCur.GuardianNum);
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//PatNumChild already set
			//PatNumGuardian already set
			_guardianCur.IsGuardian=checkIsGuardian.Checked;
			string relatName=comboRelationship.Items[comboRelationship.SelectedIndex].ToString();
			List <string> listRelationshipNamesRaw=new List<string>(Enum.GetNames(typeof(GuardianRelationship)));
			_guardianCur.Relationship=(GuardianRelationship)listRelationshipNamesRaw.IndexOf(relatName);
			if(_guardianCur.IsNew) {
				Guardians.Insert(_guardianCur);
			}
			else {
				Guardians.Update(_guardianCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		
	}
}