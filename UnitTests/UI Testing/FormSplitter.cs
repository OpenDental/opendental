using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;

namespace UnitTests {
	public partial class FormSplitter:FormODBase {
		public FormSplitter() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void textPercent_TextChanged(object sender,EventArgs e) {
			int percent=0;
			try{
				percent=int.Parse(textPercent.Text);
			}
			catch{
				return;
			}
			splitterLR.SetPercent(percent);
			splitterTB.SetPercent(percent);
		}

		private void textLocation_TextChanged(object sender,EventArgs e) {
			int loc=0;
			try{
				loc=int.Parse(textLocation.Text);
			}
			catch{
				return;
			}
			splitterLR.SetLoc(loc);
			splitterTB.SetLoc(loc);
		}
	}
}
