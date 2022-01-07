using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class MassEmailFilter:UserControl {
		private Dictionary<long,DateTime> _dictMostRecent=new Dictionary<long, DateTime>();
		public MassEmailFilter() {
			InitializeComponent();
			textNumDays.Enabled=checkExcludeWithin.Checked;
		}

		public bool DoIncludePatient(PatientInfo patient) {
			_dictMostRecent.TryGetValue(patient.PatNum,out DateTime dateTimeMostRecent);
			TimeSpan excludeMoreRecentThan=TimeSpan.FromDays(-1);//default to the future, so exclusion is ignored if not checked.
			if(checkExcludeWithin.Checked && textNumDays.IsValid()) {
				excludeMoreRecentThan=TimeSpan.FromDays(PIn.Int(textNumDays.Text));
			}
			return PromotionLogs.DoIncludePatient(patient,excludeMoreRecentThan,dateTimeMostRecent);
		}

		private void checkExcludeWithin_Click(object sender,EventArgs e) {
			textNumDays.Enabled=checkExcludeWithin.Checked;
			if(checkExcludeWithin.Checked) {
				_dictMostRecent=PromotionLogs.GetMostRecentDate().ToDictionary(x => x.PatNum,x => x.DateTime);
			}
			else {
				_dictMostRecent=new Dictionary<long, DateTime>();
			}
		}
	}
}
