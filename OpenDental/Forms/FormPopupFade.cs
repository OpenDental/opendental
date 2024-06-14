using CodeBase;
using OpenDentBusiness;
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenDental {
	///<summary>Displays a message then begins to fade after 1 second.</summary>

	public partial class FormPopupFade:FormODBase {
		///<summary>The number of milliseconds this form has been open.</summary>
		private int _timeSpent;
		///<summary>The number of milliseconds before the form starts to fade.</summary>
		private int _timeBeforeFade=1000;
		#region Click Through
		///<summary>Code to make semi-transparent. 255 is fully visible; 0 is fully invisible.</summary>
		//https://stackoverflow.com/questions/1524035/topmost-form-clicking-through-possible
		private byte _byteAlpha;

		private enum GetWindowLong {
			//Sets new extended window style
			GWL_EXSTYLE = -20
		}

		private enum ExtendedWindowStyles {
			//Transparent window
			WS_EX_TRANSPARENT = 0x20,
			//Layered window
			//http://msdn.microsoft.com/en-us/library/windows/desktop/ms632599%28v=vs.85%29.aspx#layered
			WS_EX_LAYERED = 0x80000
		}

		private enum LayeredWindowAttributes {
			/// <summary>Use bAlpha to determine the opacity of the layered window.</summary>
			LWA_COLORKEY = 0x1,
			/// <summary>Use crKey as the transparency color.</summary>
			LWA_ALPHA = 0x2
		}

		[DllImport("user32.dll",EntryPoint="GetWindowLong")]
		private static extern int User32_GetWindowLong(IntPtr hWnd, GetWindowLong nIndex);

		[DllImport("user32.dll",EntryPoint = "SetWindowLong")]
		private static extern int User32_SetWindowLong(IntPtr hWnd,GetWindowLong nIndex,int dwNewLong);
		
		[DllImport("user32.dll",EntryPoint = "SetLayeredWindowAttributes")]
		private static extern bool User32_SetLayeredWindowAttributes(IntPtr hWnd,int crKey,byte bAlpha,LayeredWindowAttributes dwFlags);

		//If we use OnLoad, it doesn't correctly display until the form begins to fade
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			//Dynamically set window creation point
			//Click through portion
			int wl=User32_GetWindowLong(this.Handle, GetWindowLong.GWL_EXSTYLE);
			//https://msdn.microsoft.com/en-us/library/windows/desktop/ms633540(v=vs.85).aspx	
			User32_SetWindowLong(this.Handle,GetWindowLong.GWL_EXSTYLE,wl | (int)ExtendedWindowStyles.WS_EX_LAYERED);
			return;
			//WS_EX_Layered allows the form to fade away
			//WS_EX_Transparent allows the form to be clicked through e.g. you can't click on anything in the form
		}
		#endregion
		#region Rounded Corners
		//used to create rounded corners around the form
		//https://stackoverflow.com/questions/10674228/form-with-rounded-borders-in-c
		[DllImport("Gdi32.dll",EntryPoint ="CreateRoundRectRgn")]
		private static extern IntPtr CreateRoundedRectRgn(
			int nLeftRect,
			int nTopRect,
			int nRightRect,
			int nBottomRect,
			int nWidthEllipse,
			int nHeightEllipse
		);
		#endregion

		public FormPopupFade(string message) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			labelInfo.Text=message;
			//start fadeout timer
			//Set a minimum show time of 1000 and maximum of 3000.  The time the box shows depends on how much text there is.
			_timeBeforeFade=Math.Min(Math.Max(1000,message.Length*30),3000);
			timer2.Interval=500/25;
			_byteAlpha=0xFF;	//Set to fully visible
			timer2.Start();
		}

		protected void timer2_Tick(object sender,EventArgs e) {
			//Be fully visible for the specified time then start to fade.
			_timeSpent+=timer2.Interval;
			if(_timeSpent<=_timeBeforeFade) {
				return;
			}
			//Change opactity at a windows level
			//The int crKey (2nd parameter) only looks at the first 24 bits of the variable.  ToArgb returns the color with the alpha in the upper 8 bits.  Because this is ignored, we can just use that instead of bit manipulation
			User32_SetLayeredWindowAttributes(this.Handle,this.TransparencyKey.ToArgb(),_byteAlpha,LayeredWindowAttributes.LWA_COLORKEY | LayeredWindowAttributes.LWA_ALPHA);
			_byteAlpha-=10;
			//byte can wrap around, so make it less than or equal to each step
			if(_byteAlpha<=10) {
				timer2.Stop();
				this.Close();
			}
		}

	}
}