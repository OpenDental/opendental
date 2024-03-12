using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public class PanelSubclassBorder:Panel{
		public Rectangle RectangleButMax;
		private System.ComponentModel.IContainer components = null;
		private const int WM_NCHITTEST = 0x0084;
		private const int HTMAXBUTTON=9;
		private const int WM_SYSCOMMAND= 0x0112;
		private const int SC_MAXIMIZE=0xF030;
		private const int WM_LBUTTONUP=0x0202;
		private const int WM_NCLBUTTONDOWN=0x00A1;//non client area
		private const int WM_NCLBUTTONUP=0x00A2;
		private const int WM_NCMOUSELEAVE=0x02A2;
		private const int WM_MOUSELEAVE=0x02A3;
		private Timer timer;

		public event EventHandler EventMaxClick;
		public event EventHandler EventMouseMoveMax;
		public event EventHandler EventMouseLeave;

		public PanelSubclassBorder() {
			InitializeComponent();
			DoubleBuffered=true;
			timer=new Timer();
			timer.Interval=100;
			timer.Tick+=Timer_Tick;
		}

		private void Timer_Tick(object sender,EventArgs e) {
			//timer starts when user moves over Max button.
			//Every 100ms, it checks to see if user is still over max button.
			//Moving left or right is handled by mouse move on Panel Border,
			//but that doesn't catch the edge case for moving down.
			//Mouse leave doesn't fire, so we are left with this clumsy timer.
			//Once user moves out of Max box, this timer stops ticking.
			Point point=PointToClient(Cursor.Position);
			if(RectangleButMax.Contains(point)){
				//still in the max box. Do nothing.
			}
			else{
				//moved out
				EventMouseLeave?.Invoke(this,new EventArgs());
				timer.Stop();
			}
		}

		protected override void WndProc(ref Message m) {
			#region WM_NCHITTEST
			if(m.Msg==WM_NCHITTEST){
				//This is to get Snap Layouts to work when hovering over the Max button.
				//https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-snap-layout-menu
				//But WM_NCHITTEST must be here instead of in the WndProc for the form because this panel intercepts the msg
				//Fantastic gem here about two different ways to subclass:
				//https://stackoverflow.com/questions/31353202/how-to-fix-borderless-form-resize-with-controls-on-borders-of-the-form/31357074#31357074
				int xScreen=unchecked((short)(long)m.LParam);
				int yScreeen=unchecked((short)((long)m.LParam >> 16));
				Point point=PointToClient(new Point(xScreen,yScreeen));
				if(RectangleButMax.Contains(point)){
					m.Result=new IntPtr(HTMAXBUTTON);
					EventMouseMoveMax?.Invoke(this,new EventArgs());
					timer.Start();
					return;
				}
				else{
					base.WndProc(ref m);
					return;
				}
			}
			#endregion WM_NCHITTEST
			#region WM_NCLBUTTONDOWN
			if(m.Msg==WM_NCLBUTTONDOWN){
				//if(m.WParam!=new IntPtr( SC_MAXIMIZE)){
				//mWParam seems to come through as 0009, which is meaningless.
				//No obvious way to test for max button other than to check the x y coords again.
				//Not needed for now because this only gets hit for maximize.
				EventMaxClick?.Invoke(this,new EventArgs());
				base.WndProc(ref m);
				return;
			}
			#endregion WM_NCLBUTTONDOWN
			base.WndProc(ref m);
		}

		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			timer.Tick-=Timer_Tick;
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}
	}
}
