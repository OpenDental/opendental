using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using OpenDental.UI;
using OpenDental;
using OpenDentBusiness;
using System.Text.RegularExpressions;

namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizRegKey:SetupWizControl {

		public UserControlSetupWizRegKey() {
			InitializeComponent();
		}

		private void UserControlSetupWizRegKey_Load(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)) {
				butChangeRegKey.Enabled=false;
			}
			FillControls();
		}

		private void FillControls() {
			string regkey = PrefC.GetString(PrefName.RegistrationKey);
			if(regkey.Length==16) {
				textRegKey.Text=regkey.Substring(0,4)+"-"+regkey.Substring(4,4)+"-"+regkey.Substring(8,4)+"-"+regkey.Substring(12,4);
			}
			else {
				textRegKey.Text=regkey;
			}
			IsDone=!string.IsNullOrEmpty(textRegKey.Text);
			this.StrIncomplete = Lan.g("FormSetupWizard","Please click the 'Change' button and type in your registration key.");
			groupProcTools.Enabled=IsDone;
		}

		private void butProcCodeTools_Click(object sender,EventArgs e) {
			using FormProcTools FormP = new FormProcTools();
			FormP.ShowDialog();
			//if(FormP.Changed) {
			//	ProcedureCodes.RefreshCache();
			//}
		}

		private void butChangeRegKey_Click(object sender,EventArgs e) {
			using FormRegistrationKey formR = new FormRegistrationKey();
			formR.ShowDialog();
			DataValid.SetInvalid(InvalidType.Prefs);
			string regkey = PrefC.GetString(PrefName.RegistrationKey);
			if(regkey.Length==16) {
				textRegKey.Text=regkey.Substring(0,4)+"-"+regkey.Substring(4,4)+"-"+regkey.Substring(8,4)+"-"+regkey.Substring(12,4);
			}
			else {
				textRegKey.Text=regkey;
			}
			IsDone=!string.IsNullOrEmpty(textRegKey.Text);
			groupProcTools.Enabled=IsDone;
		}

		private void butAdvanced_Click(object sender,EventArgs e) {
			using FormUpdateSetup FormUP=new FormUpdateSetup();
			FormUP.ShowDialog();
			FillControls();
		}
	}
}
