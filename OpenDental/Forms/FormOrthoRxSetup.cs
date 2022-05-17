using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoRxSetup:FormODBase {
		public OrthoHardwareSpec OrthoHardwareSpecCur;

		public FormOrthoRxSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoHardwareSpecEdit_Load(object sender,EventArgs e) {
		
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		
	}
}