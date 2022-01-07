using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace OpenDental {
	///<summary>Derive from this class when you want to drag a control around it's parent.</summary>
	public partial class DraggableControl:UserControl {
		[Category("Drag Attributes")]
		[Description("Turn dragging on or off")]
		public bool AllowDragging { get; set; }
		///<summary>Indicates if this control is currently being dragged</summary>
		private bool IsDragging;
		///<summary>Indicates if mouse is currently down on this control.</summary>
		private bool IsMouseDown;
		///<summary>The current location of the mouse when control is being dragged</summary>
		private Point MouseLocationOnMouseDownStart;

		///<summary>Event thrown when a drag event is complete. User was dragging but then picked the left mouse up or mouse left the control.</summary>
		public event EventHandler DragDone;

		public DraggableControl() {
			InitializeComponent();
		}

		private void UserControlDraggable_MouseDown(object sender,MouseEventArgs e) {
			if(!AllowDragging) {
				return;
			}
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			//set the mouse down flags
			MouseLocationOnMouseDownStart=e.Location;
			IsMouseDown=true;
		}

		private void UserControlDraggable_MouseMove(object sender,MouseEventArgs e) {
			if(!AllowDragging) {
				return;
			}
			if(sender==null) {
				return;
			}
			Control control=(Control)sender;
			if(IsMouseDown) { //move the control as far as we have moved the mouse
				IsDragging=true; //indicate that we are now dragging
				//move this control the same distance as we just moved the mouse
				control.Left+=(e.X - MouseLocationOnMouseDownStart.X);
				control.Top+=(e.Y - MouseLocationOnMouseDownStart.Y);
			}
		}

		private void UserControlDraggable_MouseUp(object sender,MouseEventArgs e) {
			if(!AllowDragging) {
				return;
			}
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			if(IsDragging && DragDone!=null) { //send the event if we just finished dragging
				DragDone(this,new EventArgs());
			}
			//reset the flags
			IsDragging=false;
			IsMouseDown=false;
		}

		private void UserControlDraggable_MouseLeave(object sender,EventArgs e) {
			if(!AllowDragging) {
				return;
			}
			if(IsDragging && DragDone!=null) { //send the event if we were dragging when the mouse left
				DragDone(this,new EventArgs());
			}
			//reset the flags
			IsDragging=false;
			IsMouseDown=false;
		}
	}
}
