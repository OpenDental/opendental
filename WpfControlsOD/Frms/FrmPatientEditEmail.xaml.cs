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
	public partial class FrmPatientEditEmail:FrmODBase {
/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |yes, textbox does not have a maxLength property
   Search for "progress". Any progress bars?                         |no
   Anything in the Tray?                                             |no
   Search for "filter". Any use of SetFilterControlsAndAction?       |no
   If yes, then STOP here. Talk to Jordan for strategy               |none
-Look in the code for any references to other Forms. If those forms  |
   have not been converted, then STOP.  Convert those forms first.   |none
-Will we include TabIndexes?  If so, up to what index?  This applies |
   even if one single control is set so that cursor will start there |4
-Grids: get familiar with properties in bold and with events.        |none
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|done
-Any conversion exceptions? If so, talk to Jordan.                   |none
-In WpfControlsOD/Frms, include the new files in the project.        |done
-Switch to using this checklist in the new Frm. Delete the other one-|done
-Do the red areas and issues at top look fixable? Consider reverting |yes
-Does convert script need any changes instead of fixing manually?    |no
-Fix all the red areas.                                              |done
-Address all the issues at the top. Leave in place for review.       |none
-Verify that the Button click events converted automatically.        |done
-Attach all orphaned event handlers to events in constructor.        |done
-Possibly make some labels or other controls slightly bigger due to  |
   font change.                                                      |done
-Change all places where the form is called to now call the new Frm. |done
-If there are more than about 2 or 3 refs, then review first with J. |n/a
-Test thoroughly                                                     |in progress
-Are behavior and look absolutely identical? List any variation.     |
   Exceptions include taborders only applying to textboxes           |
   and minor control color variations if they are not annoying       |yes, except for frm textBox does not have property for MaxLength, which means users can go over char limit when inputting an email. 
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|done
-Review with Jordan                                                  |done
-Commit                                                              |done
-Delete the old Winform files. That gets reviewed on the next round  |done
-Delete this checklist. That also gets reviewed on the next round    |
End of Checklist=========================================================================================================================
*/
		///<summary>Initilizes to the string passed into the form.  Gets set to what the user entered if DialogResult is OK.</summary>
		public string PatientEmails;
		private readonly int _maxChar=100;

		public FrmPatientEditEmail(string email) {
			InitializeComponent();
			PatientEmails=email;
			Load+=FrmPatientEditEmail_Load;
			textEmail.TextChanged+=textEmail_TextChanged;
			Shown+=FrmPatientEditEmail_Shown;
			PreviewKeyDown+=FrmPatientEditEmail_PreviewKeyDown;
		}

		private void FrmPatientEditEmail_Load(object sender,EventArgs e) {
			Lang.F(this);
			textEmail.Text=PatientEmails;
			UpdateCharsRemaining();
		}

		private void FrmPatientEditEmail_Shown(object sender,EventArgs e) {
			textEmail.Focus();
			textEmail.Select(PatientEmails.Length,PatientEmails.Length);
		}

		private void UpdateCharsRemaining() {
			int charsRem=Math.Max(_maxChar-textEmail.Text.Length,0);
			labelCharsRemaining.Text=$"Characters remaining: {charsRem}";
			if(charsRem==0) {
				labelCharsRemaining.ColorText=Colors.Red;
			}
			else {
				labelCharsRemaining.ColorText=Colors.Black;
			}
		}

		private void textEmail_TextChanged(object sender,EventArgs e) {
			UpdateCharsRemaining();
		}

		private void FrmPatientEditEmail_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			PatientEmails=textEmail.Text;
			PatientEmails=PatientEmails.Replace("\r\n","");
			IsDialogOK=true;
		}

	}
}