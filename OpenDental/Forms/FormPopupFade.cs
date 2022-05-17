using CodeBase;
using OpenDentBusiness;
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenDental {
	///<summary>Displays a message then begins to fade after 1 second.</summary>
			
	public partial class FormPopupFade:Form {
		///<summary>The number of milliseconds this form has been open.</summary>
		private int _timeSpent;
		///<summary>The number of milliseconds before the form starts to fade.</summary>
		private int _timeBeforeFade=1000;
		public bool _doDisplayCloseButton;
		#region Click Through
		///<summary>Code to make semi-transparent. 255 is fully visible; 0 is fully invisible.</summary>
		//https://stackoverflow.com/questions/1524035/topmost-form-clicking-through-possible
		private byte _alpha;

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
			if(_doDisplayCloseButton) {		
				User32_SetWindowLong(this.Handle,GetWindowLong.GWL_EXSTYLE,wl | (int)ExtendedWindowStyles.WS_EX_LAYERED);
			}
			else {		
				//WS_EX_Layered allows the form to fade away
				//WS_EX_Transparent allows the form to be clicked through e.g. you can't click on anything in the form
				User32_SetWindowLong(this.Handle,GetWindowLong.GWL_EXSTYLE,wl | (int)ExtendedWindowStyles.WS_EX_LAYERED | 
					(int)ExtendedWindowStyles.WS_EX_TRANSPARENT);
			}			
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

		///<summary>Displays a message in popup that fades out slowly.</summary>
		///<param name="sender">For translation.</param>
		///<param name="message">The message to be displayed.</param>
		///<param name="doDisplayClose">Set to true to display a Close button.</param>
		///<param name="doTranslate">Set to false if you don't want to translate the message.</param>
		public static void ShowMessage(object sender,string message,bool doDisplayClose=true,bool doTranslate=true) {
			if(doTranslate) {
				message=Lan.g(sender.GetType().Name,message);
			}
			FormPopupFade FormPF=new FormPopupFade(message,doDisplayClose);
			FormPF.ShowInTaskbar=false;
			FormPF.TopMost=true;
			FormPF.Show();
		}

		private FormPopupFade(string message,bool doDisplayClose) {
			InitializeComponent();
			Lan.F(this);
			_doDisplayCloseButton=doDisplayClose;
			//Creates a border with rounded corners
			if(_doDisplayCloseButton) {
				butClose.Visible=true;
			}
			else {
				this.FormBorderStyle=FormBorderStyle.None;	//Remove x in corner and border around form
				Region=Region.FromHrgn(CreateRoundedRectRgn(0,0,this.Width,this.Height,20,20));
				butClose.Visible=false;
			}	
			labelInfo.Text=message;
			Size sz=new Size(labelInfo.Width, Int32.MaxValue);
			sz=TextRenderer.MeasureText(labelInfo.Text, labelInfo.Font, sz, TextFormatFlags.WordBreak);
			labelInfo.Height=sz.Height;
			//Set position to be centered in form
			Point labelLoc=new Point(labelInfo.Size);
			Point parent=new Point(this.Size);
			if(_doDisplayCloseButton) {
				labelInfo.Location=new Point((parent.X-labelLoc.X)/2, (parent.Y-labelLoc.Y)/2-35);//Bump up text if it has the close button
			}
			else {
				labelInfo.Location=new Point((parent.X-labelLoc.X)/2, (parent.Y-labelLoc.Y)/2);
			}			
			//once placed set it to visible
			labelInfo.Visible=true;
			//start fadeout timer
			//Set a minimum show time of 1000 and maximum of 3000.  The time the box shows depends on how much text there is.
			_timeBeforeFade=Math.Min(Math.Max(1000,message.Length*30),3000);
			timer2.Interval=500/25;
			_alpha=0xFF;	//Set to fully visible
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
			User32_SetLayeredWindowAttributes(this.Handle,this.TransparencyKey.ToArgb(),_alpha,LayeredWindowAttributes.LWA_COLORKEY | LayeredWindowAttributes.LWA_ALPHA);
			_alpha-=10;
			//byte can wrap around, so make it less than or equal to each step
			if(_alpha<=10) {
				timer2.Stop();
				this.Close();
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
	}
}