using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailJobTemplateEdit:FormODBase {

		public FormEmailJobTemplateEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailJobTemplateEdit_Load(object sender,EventArgs e) {
			textBodyTextTemplate.Text=PrefC.GetString(PrefName.JobManagerDefaultEmail);
			textPledgeTemplate.Text=PrefC.GetString(PrefName.JobManagerDefaultBillingMsg);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(Prefs.UpdateString(PrefName.JobManagerDefaultEmail,textBodyTextTemplate.Text)
				| Prefs.UpdateString(PrefName.JobManagerDefaultBillingMsg,textPledgeTemplate.Text)) 
			{
				Signalods.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}