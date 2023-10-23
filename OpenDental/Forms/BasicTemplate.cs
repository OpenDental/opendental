using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class BasicTemplate:FormODBase {

		public BasicTemplate() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butSave_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}