using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	///<summary>A panel that does not paint it's background.  This allows for better double buffering and for a true transparent panel.</summary>
	public partial class PanelGraphics:System.Windows.Forms.Panel {
		public PanelGraphics() {
			InitializeComponent();
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			if(DesignMode){
				base.OnPaintBackground(e);
			}
		}
	}
}
