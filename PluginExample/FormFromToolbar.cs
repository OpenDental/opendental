using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace PluginExample {
	public partial class FormFromToolbar:Form {
		public long PatNum;

		public FormFromToolbar() {
			InitializeComponent();
		}

		private void FormFromToolbar_Load(object sender,EventArgs e) {
			Patient patient=Patients.GetLim(PatNum);
			if(patient!=null) {
				Text="New Form for "+patient.GetNameFL();
			}
		}
	}
}
