using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoRxSelect:FormODBase {
		public OrthoHardwareSpec OrthoHardwareSpecCur;

		public FormOrthoRxSelect() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoRxSelect_Load(object sender,EventArgs e) {
		
		}

		private void butOK_Click(object sender,EventArgs e) {

		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}