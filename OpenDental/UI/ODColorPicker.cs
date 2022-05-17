using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenDental.UI {
	public partial class ODColorPicker:UserControl {
		private Color _backgroundColor;

		public Color BackgroundColor {
			get {
				return _backgroundColor;
			}
			set {
				_backgroundColor=value;
				butColor.BackColor=value;
			}
		}

		public bool AllowTransparentColor {
			set {
				butNone.Visible=value;
			}
		}

		public ODColorPicker() {
			InitializeComponent();
		}

		private void butNone_Click(object sender,EventArgs e) {
			BackgroundColor=Color.Transparent;
		}

		private void butColor_Click(object sender,EventArgs e) {
			colorDialog1.Color=BackgroundColor;
			colorDialog1.ShowDialog();
			BackgroundColor=colorDialog1.Color;
		}
	}
}
