using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEmailMasterTemplate:FormODBase {

		public FormEmailMasterTemplate() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmailSetupMasterTemplate_Load(object sender,EventArgs e) {
			textMaster.Text=PrefC.GetString(PrefName.EmailMasterTemplate);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(Prefs.UpdateString(PrefName.EmailMasterTemplate,textMaster.Text)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
