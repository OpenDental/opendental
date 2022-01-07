using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace OpenDental {
	///<summary>This control prevents flicker in a SplitContainer by setting protected style of child panels.</summary>
	public partial class SplitContainerNoFlicker:SplitContainer {
		public SplitContainerNoFlicker() {
			InitializeComponent();
			DoubleBuffered=true; 
		}

	}
}
