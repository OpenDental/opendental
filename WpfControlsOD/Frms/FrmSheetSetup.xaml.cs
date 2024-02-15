using System;
using System.Collections.Generic;
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
	///<summary></summary>
	public partial class FrmSheetSetup:FrmODBase {

		private bool _isChanged;

		public FrmSheetSetup() {
			InitializeComponent();
			Load+=FrmSheetSetup_Load;
			PreviewKeyDown+=FrmSheetSetup_PreviewKeyDown;
		}

		private void FrmSheetSetup_Load(object sender,EventArgs e) {
			Lang.F(this);
			checkPatientFormsShowConsent.Checked=PrefC.GetBool(PrefName.PatientFormsShowConsent);
		}

		private void FrmSheetSetup_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}
		
		private void butSave_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.PatientFormsShowConsent,(bool)checkPatientFormsShowConsent.Checked)) {
				_isChanged=true;
			}
			if(_isChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			IsDialogOK=true;
		}

	}
}