using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SparksToothChart;

namespace TestToothChart {
	public partial class FormFullScreen:Form {
		//public ToothChartWrapper toothChartPub;

		public FormFullScreen() {
			InitializeComponent();
		}

		private void FormFullScreen_Load(object sender,EventArgs e) {
			
		}

		private void FormFullScreen_FormClosed(object sender,FormClosedEventArgs e) {
			this.toothChartForBig.Dispose();//Required for DirectX.
		}

	
	}
}
