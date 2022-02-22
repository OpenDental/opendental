using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentalGraph {
	public partial class ProductionGraphOptionsCtrl:BaseGraphOptionsCtrl {
		public bool IncludeAdjustments {
			get { return checkIncludeAdjustments.Checked; }
			set { checkIncludeAdjustments.Checked=value; }
		}
		public bool IncludeCompletedProcs {
			get { return checkIncludeCompletedProcs.Checked; }
			set { checkIncludeCompletedProcs.Checked=value; }
		}
		public bool IncludeWriteoffs {
			get { return checkIncludeWriteoffs.Checked; }
			set { checkIncludeWriteoffs.Checked=value; }
		}
		public ProductionGraphOptionsCtrl() {
			InitializeComponent();
		}

		public override int GetPanelHeight() {
			return this.Height;
		}

		private void OnProductionGraphInputsChanged(object sender,EventArgs e) {
			OnBaseInputsChanged(sender,e);
		}
	}
}
