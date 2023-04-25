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

namespace OpenDental.UI.Design{
	public class SplitContainerDesigner:ParentControlDesigner{

		public SplitContainerDesigner(){
			EnableDragDrop(true);
		}

		protected override bool GetHitTest(Point pointScreen){
			Point point=Control.PointToClient(pointScreen);
			UI.SplitContainer splitContainer=(UI.SplitContainer)Control;
			if(splitContainer.Orientation==Orientation.Vertical){
				if(point.X<splitContainer.SplitterDistance){
					return false;//In Panel 1, so should not be handled by splitContainer
				}
				if(point.X>splitContainer.SplitterDistance+splitContainer.SplitterWidth){
					return false;//In Panel 2
				}
				return true;//In the splitter, we want the control to respond to dragging
			}
			//orientation horizontal
			if(point.Y<splitContainer.SplitterDistance){
				return false;
			}
			if(point.Y>splitContainer.SplitterDistance+splitContainer.SplitterWidth){
				return false;
			}
			return true;
		}

	}
}
