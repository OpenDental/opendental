using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormClaimResend:FormODBase {

		public FormClaimResend() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public bool IsClaimReplacement {
			get{ 
				return radioClaimReplacement.Checked; 
			}
		}

		private void radioClaimOriginal_Click(object sender,EventArgs e) {
			radioClaimOriginal.Checked=true;
			radioClaimReplacement.Checked=false;
		}

		private void radioClaimReplacement_Click(object sender,EventArgs e) {
			radioClaimOriginal.Checked=false;
			radioClaimReplacement.Checked=true;
		}

		private void butSend_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}