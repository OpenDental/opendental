using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SparksToothChart {
	///<summary>This will eventually replace ToothChartOpenGL, ToothChartDirectX, and probably ToothChart2D.  All 3D drawing will be done with WPF, which completely avoids all hardware compatibility issues.  We will not do very much refactoring of existing 3D frameworks, but just build this in parallel as much as possible.  This control has one child control that's docked to fill.  The child control hosts the Wpf control.</summary>
	public partial class ToothChartWpf:UserControl {
		public ToothChartWpf() {
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs pe) {
			base.OnPaint(pe);
		}
	}
}
