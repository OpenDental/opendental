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
			UserGroup userGroupStudent=UserGroups.GetGroup(PrefC.GetLong(PrefName.SecurityGroupForStudents));
			UserGroup userGroupInstructor=UserGroups.GetGroup(PrefC.GetLong(PrefName.SecurityGroupForInstructors));
			if(userGroupStudent!=null) {
				textStudents.Text=userGroupStudent.Description;
			}
			if(userGroupInstructor!=null) {
				textInstructors.Text=userGroupInstructor.Description;
			}
		}

		private void butStudentPicker_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			FrmUserGroupPicker frmUserGroupPicker=new FrmUserGroupPicker();
			frmUserGroupPicker.IsAdminMode=false;
			frmUserGroupPicker.ShowDialog();
			if(!frmUserGroupPicker.IsDialogOK) {
				return;
			}
			DialogResult dialogResult=MessageBox.Show(Lan.g(this,"Update all existing students to this user group?")+"\r\n"
				+Lan.g(this,"Choose No to just save the new default user group for students."),"",MessageBoxButtons.YesNoCancel);
			if(dialogResult==DialogResult.Cancel) {
				return;
			}
			if(dialogResult==DialogResult.Yes) {
				try {
					Userods.UpdateUserGroupsForDentalSchools(frmUserGroupPicker.UserGroupSelected,false);
				}
				catch {
					MsgBox.Show(this,"Cannot move students to the new user group because it would leave no users with the SecurityAdmin permission.  Give the SecurityAdmin permission to at least one user that is in another group or is not flagged as a student.");
					return;
				}
			}
			//For now, only one user group can be defined as the default security group for students/instructors.
			Prefs.UpdateLong(PrefName.SecurityGroupForStudents,frmUserGroupPicker.UserGroupSelected.UserGroupNum);
			textStudents.Text=frmUserGroupPicker.UserGroupSelected.Description;
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void butInstructorPicker_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			FrmUserGroupPicker frmUserGroupPicker=new FrmUserGroupPicker();
			frmUserGroupPicker.IsAdminMode=false;
			frmUserGroupPicker.ShowDialog();
			if(!frmUserGroupPicker.IsDialogOK) {
				return;
			}
			DialogResult dialogResult=MessageBox.Show(Lan.g(this,"Update all existing instructors to this user group?")+"\r\n"
				+Lan.g(this,"Choose No to just save the new default user group for instructors."),"",MessageBoxButtons.YesNoCancel);
			if(dialogResult==DialogResult.Cancel) {
				return;
			}
			if(dialogResult==DialogResult.Yes) {
				try {
					Userods.UpdateUserGroupsForDentalSchools(frmUserGroupPicker.UserGroupSelected,true);
				}
				catch {
					MsgBox.Show(this,"Cannot move instructors to the new user group because it would leave no users with the SecurityAdmin permission.  Give the SecurityAdmin permission to at least one user that is in another group or is not flagged as an instructor.");
					return;
				}
			}
			//For now, only one user group can be defined as the default security group for students/instructors.
			Prefs.UpdateLong(PrefName.SecurityGroupForInstructors,frmUserGroupPicker.UserGroupSelected.UserGroupNum);
			textInstructors.Text=frmUserGroupPicker.UserGroupSelected.Description;
			DataValid.SetInvalid(InvalidType.Prefs);
		}

		private void butGradingScales_Click(object sender,EventArgs e) {
			//GradingScales can be edited and added from here.
			using FormGradingScales formGradingScales=new FormGradingScales();
			formGradingScales.ShowDialog();
		}

		private void butEvaluation_Click(object sender,EventArgs e) {
			//EvaluationDefs can be added and edited from here.
			using FormEvaluationDefs formEvaluationDefs=new FormEvaluationDefs();
			formEvaluationDefs.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}