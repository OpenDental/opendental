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
using CodeBase;

namespace OpenDental {
/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |no
   Search for "progress". Any progress bars?                         |no
   Anything in the Tray?                                             |no
   Search for "filter". Any use of SetFilterControlsAndAction?       |no
   If yes, then STOP here. Talk to Jordan for strategy               |
-Look in the code for any references to other Forms. If those forms  |
   have not been converted, then STOP.  Convert those forms first.   |A Frm is created but not used. FormPatientSelect() L83
-Will we include TabIndexes?  If so, up to what index?  This applies |
   even if one single control is set so that cursor will start there |no
-Grids: get familiar with properties in bold and with events.        |n/a
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|done
-Any conversion exceptions? If so, talk to Jordan.                   |no
-In WpfControlsOD/Frms, include the new files in the project.        |done
-Switch to using this checklist in the new Frm. Delete the other one-|done
-Do the red areas and issues at top look fixable? Consider reverting |yes
-Does convert script need any changes instead of fixing manually?    |no
-Fix all the red areas.                                              |
-Address all the issues at the top. Leave in place for review.       |done
-Verify that all the click events converted automatically.           |done
-Attach all orphaned event handlers to events in constructor.        |done
-Possibly make some labels or other controls slightly bigger due to  |done
   font change.                                                      |
-Change all places where the form is called to now call the new Frm. |done
-If there are more than about 2 or 3 refs, then review first with J. |n/a
-Test thoroughly                                                     |
-Are behavior and look absolutely identical? List any variation.     |yes
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
-Delete this checklist. That also gets reviewed on the next round    |
End of Checklist=========================================================================================================================
*/
	/// <summary>For a given subscriber, this list all their plans.  User then selects one plan from the list or creates a blank plan.</summary>
	public partial class FrmFamilyMemberSelect:FrmODBase {
		private Family _family;
		///<summary>When dialogResult=OK, this will contain the PatNum of the selected pat.</summary>
		public long SelectedPatNum;
		///<summary>When this flag is set, the patient status will appear in parenthesis by the patient's name in the list.</summary>
		private bool _isPatientStatusVisible;

		///<summary></summary>
		public FrmFamilyMemberSelect(Family family,bool isPatientStatusVisible=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_family=family;
			_isPatientStatusVisible=isPatientStatusVisible;
			Load+=FrmFamilyMemberSelect_Load;
			listPats.MouseDoubleClick+=listPats_DoubleClick;
			PreviewKeyDown+=FrmFamilyMemberSelect_PreviewKeyDown;
		}

		private void FrmFamilyMemberSelect_Load(object sender, System.EventArgs e) {
			Lang.F(this);
			for(int i=0;i<_family.ListPats.Length;i++){
				string patientStatus=" ("+Lang.g("enumPatientStatus",_family.ListPats[i].PatStatus.GetDescription())+")";
				listPats.Items.Add(_family.ListPats[i].GetNameFL()+(_isPatientStatusVisible ? patientStatus : ""),_family.ListPats[i]);
			}
		}

		private void listPats_DoubleClick(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			IsDialogOK=true;
		}

		private void butOther_Click(object sender, System.EventArgs e) {
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			//frmPatientSelect.IsSelectionModeOnly=true;
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel) {
				return;
			}
			SelectedPatNum=frmPatientSelect.PatNumSelected;
			IsDialogOK=true;
		}

		private void FrmFamilyMemberSelect_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listPats.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			SelectedPatNum=listPats.GetSelected<Patient>().PatNum;
			IsDialogOK=true;
		}

	}
}