using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmApptBreakRequired:FrmODBase {
		///<summary>This will be null if the procedure does not exist.</summary>
		public ProcedureCode ProcedureCodeBrokenSelected;

		public FrmApptBreakRequired() {
			InitializeComponent();
			Lang.F(this);
			PreviewKeyDown+=FrmApptBreakRequired_PreviewKeyDown;
		}

		private void FrmApptBreakRequired_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butMissed.IsAltKey(Key.M,e)) {
				butMissed_Click(this,new EventArgs());
			}
			if(butCancelled.IsAltKey(Key.C,e)) {
				butCancelled_Click(this,new EventArgs());
			}
		}

		private void butMissed_Click(object sender,EventArgs e) {
			if(ProcedureCodes.HasMissedCode()) {
				ProcedureCodeBrokenSelected=ProcedureCodes.GetProcCode("D9986");
			}
			IsDialogOK=true;
		}

		private void butCancelled_Click(object sender,EventArgs e) {
			if(ProcedureCodes.HasCancelledCode()) {
				ProcedureCodeBrokenSelected=ProcedureCodes.GetProcCode("D9987");
			}
			IsDialogOK=true;
		}

	}
}