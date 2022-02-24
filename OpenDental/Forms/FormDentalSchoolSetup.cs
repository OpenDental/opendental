using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDentalSchoolSetup:FormODBase {

		public FormDentalSchoolSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDentalSchoolSetup_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			UserGroup studentGroup=UserGroups.GetGroup(PrefC.GetLong(PrefName.SecurityGroupForStudents));
			UserGroup instructorGroup=UserGroups.GetGroup(PrefC.GetLong(PrefName.SecurityGroupForInstructors));
			if(studentGroup!=null) {
				textStudents.Text=studentGroup.Description;
			}
			if(instructorGroup!=null) {
				textInstructors.Text=instructorGroup.Description;
			}
		}

		private void butStudentPicker_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormUserGroupPicker FormUGP=new FormUserGroupPicker();
			FormUGP.IsAdminMode=false;
			FormUGP.ShowDialog();
			if(FormUGP.DialogResult!=DialogResult.OK) {
				return;
			}
			DialogResult diag=MessageBox.Show(Lan.g(this,"Update all existing students to this user group?")+"\r\n"
				+Lan.g(this,"Choose No to just save the new default user group for students."),"",MessageBoxButtons.YesNoCancel);
			if(diag==DialogResult.Cancel) {
				return;
			}
			if(diag==DialogResult.Yes) {
				try {
					Userods.UpdateUserGroupsForDentalSchools(FormUGP.UserGroup,false);
				}
				catch {
					MsgBox.Show(this,"Cannot move students to the new user group because it would leave no users with the SecurityAdmin permission.  Give the SecurityAdmin permission to at least one user that is in another group or is not flagged as a student.");
					return;
				}
			}
			//For now, only one user group can be defined as the default security group for students/instructors.
			Prefs.UpdateLong(PrefName.SecurityGroupForStudents,FormUGP.UserGroup.UserGroupNum);
			textStudents.Text=FormUGP.UserGroup.Description;
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void butInstructorPicker_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormUserGroupPicker FormUGP=new FormUserGroupPicker();
			FormUGP.IsAdminMode=false;
			FormUGP.ShowDialog();
			if(FormUGP.DialogResult!=DialogResult.OK) {
				return;
			}
			DialogResult diag=MessageBox.Show(Lan.g(this,"Update all existing instructors to this user group?")+"\r\n"
				+Lan.g(this,"Choose No to just save the new default user group for instructors."),"",MessageBoxButtons.YesNoCancel);
			if(diag==DialogResult.Cancel) {
				return;
			}
			if(diag==DialogResult.Yes) {
				try {
					Userods.UpdateUserGroupsForDentalSchools(FormUGP.UserGroup,true);
				}
				catch {
					MsgBox.Show(this,"Cannot move instructors to the new user group because it would leave no users with the SecurityAdmin permission.  Give the SecurityAdmin permission to at least one user that is in another group or is not flagged as an instructor.");
					return;
				}
			}
			//For now, only one user group can be defined as the default security group for students/instructors.
			Prefs.UpdateLong(PrefName.SecurityGroupForInstructors,FormUGP.UserGroup.UserGroupNum);
			textInstructors.Text=FormUGP.UserGroup.Description;
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void butGradingScales_Click(object sender,EventArgs e) {
			//GradingScales can be edited and added from here.
			using FormGradingScales FormGS=new FormGradingScales();
			FormGS.ShowDialog();
		}

		private void butEvaluation_Click(object sender,EventArgs e) {
			//EvaluationDefs can be added and edited from here.
			using FormEvaluationDefs FormED=new FormEvaluationDefs();
			FormED.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}