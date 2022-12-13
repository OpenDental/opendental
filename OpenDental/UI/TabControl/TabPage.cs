using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	///<summary>Analogous to the MS TabPage.</summary>
	[Designer(typeof(Design.TabPageDesigner))]
	public class TabPage:Panel {
		///<summary>The rectangle for the tab at the top, not the tab page.</summary>
		public Rectangle rectangleTab;
		//public new string Text;

		///<summary></summary>
		public TabPage() {
			DoubleBuffered=true;
		}

		///<summary>Specify text that will go on the tab.</summary>
		public TabPage(string text) {
			DoubleBuffered=true;
			Text=text;
		}

		public override string ToString() {
			return "UI.TabPage2: "+Name;
		}

		[Category("Appearance")]
		[DefaultValue(typeof(Color),"0,0,0,0")]//Color.Empty
		[EditorBrowsable(EditorBrowsableState.Always)]//in the collection editor
		public Color ColorTab{get;set;}=Color.Empty;

		///<summary>Not meaningful for this control, so we ignore it.</summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new DockStyle Dock{
			get;
			set;
		}

		///<summary>Not meaningful for this control, so we ignore it.</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location{
			get=>base.Location;
			set=>value=base.Location;
		}

		//consider overriding the Locked property, which was added by designer

		[Category("Appearance")]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		public new string Text{get;set;}="";

		
	}
}
