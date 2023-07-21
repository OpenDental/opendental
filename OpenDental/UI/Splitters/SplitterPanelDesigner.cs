using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using PDMPScript;

namespace OpenDental.UI.Design{
	public class SplitterPanelDesigner:ParentControlDesigner{

		public SplitterPanelDesigner(){
			EnableDragDrop(true);
		}

		protected override bool GetHitTest(Point pointScreen){
			/*
			Point point=Control.PointToClient(pointScreen);
			SplitterPanel splitterPanel=Control as SplitterPanel;
			if(splitterPanel.VerticalScroll.Visible && point.X>splitterPanel.Width-18){//no scaling since we're in design
				return true;//so user can scroll
			}
			if(splitterPanel.HorizontalScroll.Visible && point.Y>splitterPanel.Height-18){
				return true;//so user can scroll
			}*/
			return false;//mouse should not be handled by control. This allows us to drag rectangles.
		}

		//public override SelectionRules SelectionRules {
		//	get{
		//		return SelectionRules.Locked;//This prevents user from moving the splitter panel
		//	}
		//}



	}
}
