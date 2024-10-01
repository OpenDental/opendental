using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	///<summary>Kind of like the MS ToolTip, except it comes up instantly and moves with the mouse. Something similar was also done in ControlApptPanel.  In that case, a panel was sufficient, but since this needs to spill outside the control, it must be a window. See boilerplate at bottom of this file.</summary>
	public partial class ToolTipOD : Form{
		private Action<Control,Point> _actionSetString;
		public string StringToShow;

		public ToolTipOD(){
			InitializeComponent();
			DoubleBuffered=true;
			Visible=false;
		}

		///<summary>This tooltip will respond to the mouseMove event of the control it's assigned to. The action should handle the point from the mouseMove with a hit test, and should then call SetString to show or hide the tooltip. An extremely simple action would just always return a certain string without any hit test. This can be called repeatedly for different controls as long as the action is the same.</summary>
		public void SetControlAndAction(Control control,Action<Control,Point> actionSetString){
			_actionSetString=actionSetString;
			control.MouseMove += _controlAssigned_MouseMove;
			control.MouseLeave += _controlAssigned_MouseLeave;
		}

		///<summary>This is intended to be called from within the Action that was previously set. The string will show immediately. If an empty string is passed in, then visiblity will be false.</summary>
		public void SetString(Control control,string stringToShow,Font font=null){
			if(stringToShow==""){
				Visible=false;
				return;
			}
			if(font is null){
				throw new Exception("Font required unless string is empty.");
			}
			StringToShow=stringToShow;
			Font=font;
			if(this.IsDisposed) {
				//If the ToolTipOD control has been disposed, return since attempting to create a 'Graphics' object with the handle will throw UE
				return;
			}
			using Graphics g=Graphics.FromHwnd(this.Handle);
			Width=(int)g.MeasureString(stringToShow,Font).Width+5;
			Height=Font.Height+4;
			//no need to worry about LayoutManager because it won't stick around long or be moved to other screen.
			Visible=true;
			Point point=Control.MousePosition;
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromControl(control);
			if(point.Y+20+Height > screen.WorkingArea.Bottom){
				//would spill off bottom
				point.Y-=Height;
			}
			else{
				point.Y+=20;//puts it below the arrow
			}
			if(point.X+Width > screen.WorkingArea.Right){
				//would spill off right of screen
				point.X=screen.WorkingArea.Right-Width;
			}
			Location=point;
			Invalidate();
		}

		private void _controlAssigned_MouseLeave(object sender, EventArgs e){
			Control control=(Control)sender;
			Point pointDesktop=Control.MousePosition;
			if(this.DesktopBounds.Contains(pointDesktop)){
				//left the controlAssigned, but is now within this tooltip
				//Calculate the real point in coordinates of the control assigned
				//Point pointInToolTip=this.PointToClient(pointDesktop);
				//Point pointInControl=new Point(pointInToolTip.X+Left,pointInToolTip.Y+Top);
				Point pointInControl=control.PointToClient(pointDesktop);
				_actionSetString(control,pointInControl);
			}
			else{
				Visible=false;
			}
		}

		private void _controlAssigned_MouseMove(object sender, MouseEventArgs e){
			Control control=(Control)sender;
			if(Control.MouseButtons!=MouseButtons.None){
				//a mouse button is down
				return;
			}
			_actionSetString(control,e.Location);
		}

		/*Couldn't get this to work, so used MouseLeave instead
		private void ToolTipOD_MouseMove(object sender, MouseEventArgs e){
			//Happens when tooltip is showing above arrow, and user moves arrow up.
			//Calculate the real point in coordinates of the control assigned
			Point point=new Point(e.X+Left,e.Y+Top);
			_actionSetString(point);
		}*/

		protected override void OnPaint(PaintEventArgs e){
			base.OnPaint(e);
			Graphics g=e.Graphics;
			g.Clear(Color.LightYellow);
			Rectangle rectangle=new Rectangle(0,0,Width-1,Height-1);
			g.DrawRectangle(Pens.Gray,rectangle);//border
			g.DrawString(StringToShow,Font,Brushes.Black,2,2);
		}
	}
}

/*
Use like this:
private ToolTipOD toolTipOD;

//Constructor:
			toolTipOD=new ToolTipOD();
			toolTipOD.SetControlAndAction(this,ToolTipSetString);

		///<summary></summary>
		private void ToolTipSetString(Point point) {
			//In a more complex usage, you can do a hit test right here to decide what msg to show,
			//and if you don't want to show a msg, then use toolTipOD.SetString("");
			toolTipOD.SetString(Lan.g(this,"A message to show when hovering"),Font);
		}
*/