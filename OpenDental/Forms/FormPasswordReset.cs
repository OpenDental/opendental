using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormPasswordReset : FormODBase {

		///<summary></summary>
		public FormPasswordReset(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRP_Load(object sender, System.EventArgs e) {
			//it does not compromise security to include the hash to the master password in the code
			//because the user must still enter the password, not the hash.
			//masterHash="78sfTin/RP0rI84zv2Xc8Q==";
				//version 3.5: "1251671001032231238111186944262869879186";
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//No longer functional
			//Debug.WriteLine(Userods.EncryptPassword(textMasterPass.Text));
			//if(!Userods.CheckTypedPassword(textMasterPass.Text,masterHash)){
			//	MessageBox.Show(Lan.g(this,"Master password incorrect."));
			//	return;
			//}
			//Security.ResetPassword();
			//PermissionsOld.Refresh();
			//MessageBox.Show(Lan.g(this,"Security Administration permission has been reset."));
			//DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}




	}
}








