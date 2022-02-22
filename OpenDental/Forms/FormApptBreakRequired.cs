using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormApptBreakRequired:FormODBase {
		///<summary>This will be null if the procedure does not exist.</summary>
		public ProcedureCode ProcedureCodeBrokenSelected;

		public FormApptBreakRequired() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);;
		}

		private void butMissed_Click(object sender,EventArgs e) {
			if(ProcedureCodes.HasMissedCode()) {
				ProcedureCodeBrokenSelected=ProcedureCodes.GetProcCode("D9986");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancelled_Click(object sender,EventArgs e) {
			if(ProcedureCodes.HasCancelledCode()) {
				ProcedureCodeBrokenSelected=ProcedureCodes.GetProcCode("D9987");
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}