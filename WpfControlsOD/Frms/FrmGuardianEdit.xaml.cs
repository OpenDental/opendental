using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
/*
 * You must implement Alt key for the Access Key on the button
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |none
   Search for "progress". Any progress bars?                         |none
   Anything in the Tray?                                             |none
   Search for "filter". Any use of SetFilterControlsAndAction?       |none
   If yes, then STOP here. Talk to Jordan for strategy               |none
-Look in the code for any references to other Forms. If those forms  |
   have not been converted, then STOP.  Convert those forms first.   |done
-Will we include TabIndexes?  If so, up to what index?  This applies |
   even if one single control is set so that cursor will start there |10
-Grids: get familiar with properties in bold and with events.        |done
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|done
-Any conversion exceptions? If so, talk to Jordan.                   |none
-In WpfControlsOD/Frms, include the new files in the project.        |done
-Switch to using this checklist in the new Frm. Delete the other one-|done
-Do the red areas and issues at top look fixable? Consider reverting |yes
-Does convert script need any changes instead of fixing manually?    |no
-Fix all the red areas.                                              |done
-Address all the issues at the top. Leave in place for review.       |none
-Verify that all the click events converted automatically.  ...      |done
-Attach all orphaned event handlers to events in constructor.        |done
-Possibly make some labels or other controls slightly bigger due to  |
   font change.                                                      |done
-Change all places where the form is called to now call the new Frm. |done
-If there are more than about 2 or 3 refs, then review first with J. |2 refs
-Test thoroughly                                                     |done
-Are behavior and look absolutely identical? List any variation.     |
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |yes
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|done
-Review with Jordan                                                  |partial
-Commit                                                              |partial
-Delete the old Winform files. That gets reviewed on the next round  |
-Delete this checklist. That also gets reviewed on the next round    |
End of Checklist=========================================================================================================================
*/
	public partial class FrmGuardianEdit:FrmODBase {

		private Guardian _guardian;
		private Family _family;
		private List<string> _listGuardianRelationshipNames;

		public FrmGuardianEdit(Guardian guardian,Family family){
			InitializeComponent();
			_guardian=guardian;
			_family=family;
			Load+=FrmGuardianEdit_Load;
			PreviewKeyDown+=FrmGuardianEdit_PreviewKeyDown;
		}

		private void FrmGuardianEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
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
				comboRelationship.Items.Add(Lang.g("enumGuardianRelationship",_listGuardianRelationshipNames[i]));
				if(_listGuardianRelationshipNames[i]==_guardian.Relationship.ToString()) {
					comboRelationship.SelectedIndex=i;
				}
			}
		}

		private void FrmGuardianEdit_PreviewKeyDown(object sender,KeyEventArgs e){
			if(butSave.IsAltKey(Key.S,e)){
				butSave_Click(this,new EventArgs());
			}
		}

		private void butPick_Click(object sender,EventArgs e) {
			FrmFamilyMemberSelect frmFamilyMemberSelect=new FrmFamilyMemberSelect(_family);
			frmFamilyMemberSelect.ShowDialog();
			if(frmFamilyMemberSelect.IsDialogCancel) {
				return;
			}
			_guardian.PatNumGuardian=frmFamilyMemberSelect.SelectedPatNum;
			textFamilyMember.Text=_family.GetNameInFamFL(_guardian.PatNumGuardian);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_guardian.IsNew) {
				IsDialogCancel=true;
			}
			else {
				Guardians.Delete(_guardian.GuardianNum);
				IsDialogOK=true;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			//PatNumChild already set
			//PatNumGuardian already set
			_guardian.IsGuardian=checkIsGuardian.Checked==true;
			string guardianRelationshipName=comboRelationship.GetSelected<string>();
			List <string> listRelationshipNamesRaw=new List<string>(Enum.GetNames(typeof(GuardianRelationship)));
			_guardian.Relationship=(GuardianRelationship)listRelationshipNamesRaw.IndexOf(guardianRelationshipName);
			if(_guardian.IsNew) {
				Guardians.Insert(_guardian);
			}
			else {
				Guardians.Update(_guardian);
			}
			IsDialogOK=true;
		}
		
	}
}