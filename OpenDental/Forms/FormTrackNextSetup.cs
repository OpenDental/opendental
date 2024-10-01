using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTrackNextSetup:FormODBase {

		public FormTrackNextSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTrackNextSetup_Load(object sender,EventArgs e) {
			textDaysPast.Text=PrefC.GetLong(PrefName.PlannedApptDaysPast).ToString();
			textDaysFuture.Text=PrefC.GetLong(PrefName.PlannedApptDaysFuture).ToString();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!textDaysPast.IsValid() || !textDaysFuture.IsValid()) {
				MsgBox.Show("Please fix data entry errors first.");
				return;
			}
			int uschedDaysPast=PIn.Int(textDaysPast.Text,false);
			int uschedDaysFuture=PIn.Int(textDaysFuture.Text,false);
			bool hasPrefChanged = false;
			hasPrefChanged |= Prefs.UpdateInt(PrefName.PlannedApptDaysPast,uschedDaysPast);
			hasPrefChanged |= Prefs.UpdateLong(PrefName.PlannedApptDaysFuture,uschedDaysFuture);
			if(hasPrefChanged){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

	}
}