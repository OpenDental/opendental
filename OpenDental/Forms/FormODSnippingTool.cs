using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This form is the brain of the snipping functionality.  This controls whether or not a user can make a new snip, modify and existing snip, and save/cancel a snip.</summary>
	public partial class FormODSnippingTool:Form {
		internal Image ImageSnippedScreenshot=null;

		public FormODSnippingTool() {
			InitializeComponent();
			RelayoutForm();
		}

		public static Image Snip() {
			using FormODSnippingTool formODSnippingTool=new FormODSnippingTool();
			//Ensure that the form is visible or the user will be very confused.
			formODSnippingTool.WindowState=FormWindowState.Normal;
			formODSnippingTool.TopMost=true;
			formODSnippingTool.ShowDialog();
			if(formODSnippingTool.DialogResult==DialogResult.OK) {
				return formODSnippingTool.ImageSnippedScreenshot;
			}
			return null;
		}

		///<summary>Minimizes the form and calls the child form (SnippingWindow). The child form takes a screenshot of the users
		///monitors and overlays it with an opaque rectangle. The user can then highlight subsections of that rectangle to "snip"
		///that portion of the captured screen. The child form has a public variable that the parent form uses to access the snipped image.</summary>
		private void buttonNewSnip_Click(object sender,EventArgs e) {
			this.WindowState=FormWindowState.Minimized;
			//This arbitrary wait is to give the form time to complete its minimization animation. We do not want this form to show up in the
			//screenshot so this is necessary. We cannot utilize the Form_Resized() event as that also happens too quickly and still catches the window.
			//Because this wait time is machine dependent we may need to change it in the future if customers start to complain.
			System.Threading.Thread.Sleep(300);
			//Creates a rectangle object spanning all screens connected to the system.
			Rectangle rectangleTotalScreen=Rectangle.Empty;
			//We could use virtual screen but that may not work for all windows versions. 
			for(int i=0;i<System.Windows.Forms.Screen.AllScreens.Length;i++) {
				System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.AllScreens[i];
				rectangleTotalScreen=Rectangle.Union(rectangleTotalScreen,screen.Bounds);
			}
			using Bitmap bitmapCurrentScreenCapture=new Bitmap(rectangleTotalScreen.Width,rectangleTotalScreen.Height,PixelFormat.Format32bppPArgb);
			using Graphics g=Graphics.FromImage(bitmapCurrentScreenCapture); 
			//Print screen into currentScreenCapture
			g.CopyFromScreen(0,0,0,0,bitmapCurrentScreenCapture.Size);
			using SnippingWindow snippingWindow=new SnippingWindow(bitmapCurrentScreenCapture);
			snippingWindow.ShowDialog();
			if(snippingWindow.DialogResult==DialogResult.OK) {
				ImageSnippedScreenshot=snippingWindow.ImageSnippedScreenshot;
				pictureBox1.Image=ImageSnippedScreenshot;
				//Show the parent form again
				this.WindowState=FormWindowState.Normal;
				pictureBox1.Size=ImageSnippedScreenshot.Size;
				//This is a hack to make the snipped image look centered if it is smaller than the forms default size.
				//For whatever reason the form's autosize logic does not trigger when the picturebox is smaller than the form.
				int widthPad=25;
				if(this.Size.Width>pictureBox1.Size.Width+widthPad) {
					pictureBox1.Size=new System.Drawing.Size(this.Size.Width-widthPad,this.pictureBox1.Size.Height);
				}
				RelayoutForm();
			}
			if(snippingWindow.DialogResult==DialogResult.Cancel) {//Cancel out of the snipper entirely if they cancelled while snipping.
				this.DialogResult=DialogResult.Cancel;
			}
		}

		///<summary>Just a small helper used to hide the border of the picture box when it has no image.
		///Also hides the OK button when waiting to snip an image.</summary>
		private void RelayoutForm() {
			if(pictureBox1.Image==null) {
				pictureBox1.Visible=false;
				butOk.Visible=false;
			}
			else {
				pictureBox1.Visible=true;
				butOk.Visible=true;
			}
		}
		
		private void buttonOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		//Clears the _snippedScreenshot field to null. Callers of this form should consider this scenario.
		private void buttonCancel_Click(object sender,EventArgs e) {
			ImageSnippedScreenshot=null;
			DialogResult=DialogResult.Cancel;
		}

		///<summary>This form will outline the entire display with a semi transparent overlay, and contain all snipping logic.</summary>
		private partial class SnippingWindow:Form {

			private Rectangle _rectangleSelection=new Rectangle();
			private System.Drawing.Point _pointStart;
			public Image ImageSnippedScreenshot=null;

			public SnippingWindow(Image imageScreenShot) {
				this.BackgroundImage=imageScreenShot;
				if(imageScreenShot!=null) {
					this.Size=imageScreenShot.Size;
				}
				this.ShowInTaskbar=false;
				this.DoubleBuffered=true;
				this.Location=new System.Drawing.Point(0,0);
				this.FormBorderStyle=FormBorderStyle.None;
				this.TopMost=true;
			}

			protected override void OnMouseDown(MouseEventArgs e) {
				// Start the snip on mouse down
				if(e.Button!=MouseButtons.Left) {
					return;
				}
				_pointStart=e.Location;
				_rectangleSelection=new Rectangle(e.Location,new System.Drawing.Size(0,0));
				this.Invalidate();
			}

			protected override void OnMouseMove(MouseEventArgs e) {
				//Modify the selection on mouse move.  Allows the selection to go in any direction
				if(e.Button!=MouseButtons.Left) {
					return;
				}
				//Getting max and min allows the selection to be rotated and dragged in any direction.
				int x1=Math.Min(e.X,_pointStart.X);
				int y1=Math.Min(e.Y,_pointStart.Y);
				int x2=Math.Max(e.X,_pointStart.X);
				int y2=Math.Max(e.Y,_pointStart.Y);
				_rectangleSelection=new Rectangle(x1,y1,x2-x1,y2-y1);
				this.Invalidate();
			}

			protected override void OnMouseUp(MouseEventArgs e) {
				//Complete the snip on mouse-up
				if(_rectangleSelection.Width<=0 || _rectangleSelection.Height<=0) {
					return;
				}
				ImageSnippedScreenshot=new Bitmap(_rectangleSelection.Width,_rectangleSelection.Height);
				using Graphics g=Graphics.FromImage(ImageSnippedScreenshot);
				//Takes the selected part of the background that was captured earlier and pumps it into SnippedScreenshot through the graphic.
				g.DrawImage(this.BackgroundImage,new Rectangle(0,0,ImageSnippedScreenshot.Width,ImageSnippedScreenshot.Height),
					_rectangleSelection,GraphicsUnit.Pixel);
				DialogResult=DialogResult.OK;
			}

			protected override bool ProcessCmdKey(ref Message message,Keys keys) {
				//Cancel the snip when pressing the escape key.  This matches Microsoft's Snipping Tool
				if(keys==Keys.Escape) {
					this.DialogResult=DialogResult.Cancel;
				}
				return base.ProcessCmdKey(ref message,keys);
			}

			protected override void OnPaint(PaintEventArgs e) {
				//Draw the current selection
				using Brush brushSemiTransparentWhite=new SolidBrush(Color.FromArgb(120,Color.White));
				int xStart=_rectangleSelection.X;
				int xEnd=_rectangleSelection.X+_rectangleSelection.Width;
				int yStart=_rectangleSelection.Y;
				int yEnd=_rectangleSelection.Y+_rectangleSelection.Height;
				//Highlight everything semi transparent white horizontally until reaching the selection's starting x coordinate
				e.Graphics.FillRectangle(brushSemiTransparentWhite,new Rectangle(0,0,xStart,this.Height));
				//Highlight everything semi transparent white horizontally after the selection's ending x coordinate
				e.Graphics.FillRectangle(brushSemiTransparentWhite,new Rectangle(xEnd,0,this.Width-xEnd,this.Height));
				//Highlight the remaining square above the selection semi transparent white vertically
				e.Graphics.FillRectangle(brushSemiTransparentWhite,new Rectangle(xStart,0,xEnd-xStart,yStart));
				//Highlight the remaining square below the selection semi transparent white vertically
				e.Graphics.FillRectangle(brushSemiTransparentWhite,new Rectangle(xStart,yEnd,xEnd-xStart,this.Height-yEnd));
				//Outline the selection with a red border.  This matches Microsoft's snipping tool.
				using Pen pen=new Pen(Color.Red,2);
				e.Graphics.DrawRectangle(pen,_rectangleSelection);
			}
		}
	}
}
