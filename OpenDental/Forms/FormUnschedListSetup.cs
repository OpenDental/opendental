using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormUnschedListSetup:FormODBase {

		public FormUnschedListSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormUnschedListSetup_Load(object sender,EventArgs e) {
			int daysPast=PrefC.GetInt(PrefName.UnschedDaysPast);
			if(daysPast!=-1) {
				textDaysPast.Text=daysPast.ToString();
			}
			int daysFuture=PrefC.GetInt(PrefName.UnschedDaysFuture);
			if(daysFuture!=-1) {
				textDaysFuture.Text=daysFuture.ToString();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool isPrefsInvalid=false;
			int unschedDaysPastValue=-1;
			int unschedDaysFutureValue=-1;
			if(!string.IsNullOrWhiteSpace(textDaysPast.Text)) {
				unschedDaysPastValue=PIn.Int(textDaysPast.Text,false);
			}
			if(!string.IsNullOrWhiteSpace(textDaysFuture.Text)) {
				unschedDaysFutureValue=PIn.Int(textDaysFuture.Text,false);
			}
			isPrefsInvalid=Prefs.UpdateInt(PrefName.UnschedDaysPast,unschedDaysPastValue) 
				| Prefs.UpdateInt(PrefName.UnschedDaysFuture,unschedDaysFutureValue);
			if(isPrefsInvalid) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}