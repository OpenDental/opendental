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
			int daysPast=PrefC.GetInt(PrefName.PlannedApptDaysPast);
			int daysFuture=PrefC.GetInt(PrefName.PlannedApptDaysFuture);
			if(daysPast!=-1) {
				textDaysPast.Text=daysPast.ToString();
			}
			if(daysFuture!=-1) {
				textDaysFuture.Text=daysFuture.ToString();
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			int daysPast=-1;
			int daysFuture=-1;
			if(textDaysPast.Text!="") {
				try {
					daysPast=int.Parse(textDaysPast.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				if(daysPast<0 || daysPast>10000) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
			}
			if(textDaysFuture.Text!="") {
				try {
					daysFuture=int.Parse(textDaysFuture.Text);
				}
				catch {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				if(daysFuture<0 || daysFuture>10000) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
			}
			//End of validation-------------------------------------------------------
			bool hasPrefChanged = false;
			hasPrefChanged |= Prefs.UpdateInt(PrefName.PlannedApptDaysPast,daysPast);
			hasPrefChanged |= Prefs.UpdateInt(PrefName.PlannedApptDaysFuture,daysFuture);
			if(hasPrefChanged){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

	}
}