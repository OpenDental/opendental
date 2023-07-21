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
	public partial class FormSplitContainerTests:FormODBase {
		public FormSplitContainerTests() {
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=50;
		}

		private void button4_Click(object sender,EventArgs e) {
			splitContainer1.SplitterDistance=LayoutManager.Scale(220);
			LayoutManager.LayoutControlBoundsAndFonts(splitContainer1);
		}

		private void button1_Click(object sender,EventArgs e) {
			splitContainer1.SplitterDistance=LayoutManager.Scale(88);
			LayoutManager.LayoutControlBoundsAndFonts(splitContainer1);
		}

		private void splitContainer1_SplitterMoved(object sender,SplitterEventArgs e) {
			LayoutManager.LayoutControlBoundsAndFonts(splitContainer1);
			MsgBox.Show("Moved");
		}

		private void button2_Click(object sender,EventArgs e) {
			splitContainerOD.SplitterDistance=LayoutManager.Scale(60);
		}

		private void button3_Click(object sender,EventArgs e) {
			splitContainerOD.SplitterDistance=LayoutManager.Scale(150);
		}

		private void splitContainerOD_SplitterMoved(object sender,EventArgs e) {
			//MsgBox.Show("Moved");
		}
	}
}
