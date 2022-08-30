using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMapHQPrefs:FormODBase {

		public FormMapHQPrefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFillTriagePref_Load(object sender,EventArgs e) {
			textRedCalls.Text=PrefC.GetInt(PrefName.TriageRedCalls).ToString();
			textRedTime.Text=PrefC.GetInt(PrefName.TriageRedTime).ToString();
			textVMCalls.Text=PrefC.GetInt(PrefName.VoicemailCalls).ToString();
			textVMTime.Text=PrefC.GetInt(PrefName.VoicemailTime).ToString();
			textCallsWarning.Text=PrefC.GetInt(PrefName.TriageCallsWarning).ToString();
			textCalls.Text=PrefC.GetInt(PrefName.TriageCalls).ToString();
			textTimeWarning.Text=PrefC.GetInt(PrefName.TriageTimeWarning).ToString();
			textTime.Text=PrefC.GetInt(PrefName.TriageTime).ToString();
		}

		///<summary>Returns true if a preference changed.  Otherwise; false.</summary>
		private bool SavePrefs() {
			bool hasChanged=false;
			hasChanged |= Prefs.UpdateInt(PrefName.TriageRedCalls,PIn.Int(textRedCalls.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.TriageRedTime,PIn.Int(textRedTime.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.VoicemailCalls,PIn.Int(textVMCalls.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.VoicemailTime,PIn.Int(textVMTime.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.TriageCallsWarning,PIn.Int(textCallsWarning.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.TriageCalls,PIn.Int(textCalls.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.TriageTimeWarning,PIn.Int(textTimeWarning.Text));
			hasChanged |= Prefs.UpdateInt(PrefName.TriageTime,PIn.Int(textTime.Text));
			return hasChanged;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textRedCalls.IsValid()
				|| !textVMCalls.IsValid()
				|| !textCallsWarning.IsValid()
				|| !textCalls.IsValid()
				|| !textRedTime.IsValid()
				|| !textVMTime.IsValid()
				|| !textTimeWarning.IsValid()
				|| !textTime.IsValid())
			{
				MsgBox.Show(this,"All fields must be whole numbers.  Please fix data entry errors first.");
				return;
			}
			if(SavePrefs()) { 
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void labelIntro_Click(object sender,EventArgs e) {

		}
	}
}