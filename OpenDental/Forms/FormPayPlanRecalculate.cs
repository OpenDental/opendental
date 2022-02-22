using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormPayPlanRecalculate:FormODBase {

		public bool IsPrepay=true;
		public bool IsRecalculateInterest=true;

		public FormPayPlanRecalculate() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayPlanRecalculate_Load(object sender,EventArgs e) {
			radioPrepay.Checked=IsPrepay;
			checkRecalculateInterest.Checked=IsRecalculateInterest;
		}

		private void butOK_Click(object sender,EventArgs e) {
			IsPrepay=radioPrepay.Checked;
			IsRecalculateInterest=checkRecalculateInterest.Checked;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}