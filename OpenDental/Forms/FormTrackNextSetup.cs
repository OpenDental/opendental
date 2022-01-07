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

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDaysPast.IsValid() || !textDaysFuture.IsValid()) {
				MsgBox.Show("Please fix data entry errors first.");
				return;
			}
			int uschedDaysPastValue=PIn.Int(textDaysPast.Text,false);
			int uschedDaysFutureValue=PIn.Int(textDaysFuture.Text,false);
			if(Prefs.UpdateInt(PrefName.PlannedApptDaysPast,uschedDaysPastValue) 
				|Prefs.UpdateLong(PrefName.PlannedApptDaysFuture,uschedDaysFutureValue)) 
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}