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
		internal Image SnippedScreenshot=null;

		public FormODSnippingTool() {
			InitializeComponent();
			RelayoutForm();
		}

		public static Image Snip() {
			using FormODSnippingTool snipper=new FormODSnippingTool();
			//Ensure that the form is visible or the user will be very confused.
			snipper.WindowState=FormWindowState.Normal;
			snipper.TopMost=true;
			snipper.ShowDialog();
			if(snipper.DialogResult==DialogResult.OK) {
				return snipper.SnippedScreenshot;
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
			Rectangle totalScreenSize=Rectangle.Empty;
			//We could use virtual screen but that may not work for all windows versions. 
			foreach(System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens) {
				totalScreenSize=Rectangle.Union(totalScreenSize,s.Bounds);
			}
			using(Bitmap currentScreenCapture=new Bitmap(totalScreenSize.Width,totalScreenSize.Height,PixelFormat.Format32bppPArgb)) {
				using(Graphics g=Graphics.FromImage(currentScreenCapture)) {
					//Print screen into currentScreenCapture
					g.CopyFromScreen(0,0,0,0,currentScreenCapture.Size);
				}
				using SnippingWindow FormODS=new SnippingWindow(currentScreenCapture);
				FormODS.ShowDialog();
				if(FormODS.DialogResult==DialogResult.OK) {
					SnippedScreenshot=FormODS.SnippedScreenshot;
					pictureBox1.Image=SnippedScreenshot;
					//Show the parent form again
					this.WindowState=FormWindowState.Normal;
					pictureBox1.Size=SnippedScreenshot.Size;
					//This is a hack to make the snipped image look centered if it is smaller than the forms default size.
					//For whatever reason the form's autosize logic does not trigger when the picturebox is smaller than the form.
					int widthPad=25;
					if(this.Size.Width>pictureBox1.Size.Width+widthPad) {
						pictureBox1.Size=new System.Drawing.Size(this.Size.Width-widthPad,this.pictureBox1.Size.Height);
					}
					RelayoutForm();
				}
				if(FormODS.DialogResult==DialogResult.Cancel) {//Cancel out of the snipper entirely if they cancelled while snipping.
					this.DialogResult=DialogResult.Cancel;
				}
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
			SnippedScreenshot=null;
			DialogResult=DialogResult.Cancel;
		}

		///<summary>This form will outline the entire display with a semi transparent overlay, and contain all snipping logic.</summary>
		private partial class SnippingWindow:Form {

			private Rectangle _rectSelection=new Rectangle();
			private System.Drawing.Point _pointStart;
			public Image SnippedScreenshot=null;

			public SnippingWindow(Image screenShot) {
				this.BackgroundImage=screenShot;
				if(screenShot!=null) {
					this.Size=screenShot.Size;
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
				_rectSelection=new Rectangle(e.Location,new System.Drawing.Size(0,0));
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
				_rectSelection=new Rectangle(x1,y1,x2-x1,y2-y1);
				this.Invalidate();
			}

			protected override void OnMouseUp(MouseEventArgs e) {
				//Complete the snip on mouse-up
				if(_rectSelection.Width<=0 || _rectSelection.Height<=0) {
					return;
				}
				SnippedScreenshot=new Bitmap(_rectSelection.Width,_rectSelection.Height);
				using(Graphics g=Graphics.FromImage(SnippedScreenshot)) {
					//Takes the selected part of the background that was captured earlier and pumps it into SnippedScreenshot through the graphic.
					g.DrawImage(this.BackgroundImage,new Rectangle(0,0,SnippedScreenshot.Width,SnippedScreenshot.Height),
						_rectSelection,GraphicsUnit.Pixel);
				}
				DialogResult=DialogResult.OK;
			}

			protected override bool ProcessCmdKey(ref Message msg,Keys keyData) {
				//Cancel the snip when pressing the escape key.  This matches Microsoft's Snipping Tool
				if(keyData==Keys.Escape) {
					this.DialogResult=DialogResult.Cancel;
				}
				return base.ProcessCmdKey(ref msg,keyData);
			}

			protected override void OnPaint(PaintEventArgs e) {
				//Draw the current selection
				using(Brush semiTransparentWhite=new SolidBrush(Color.FromArgb(120,Color.White))) {
					int xStart=_rectSelection.X;
					int xEnd=_rectSelection.X+_rectSelection.Width;
					int yStart=_rectSelection.Y;
					int yEnd=_rectSelection.Y+_rectSelection.Height;
					//Highlight everything semi transparent white horizontally until reaching the selection's starting x coordinate
					e.Graphics.FillRectangle(semiTransparentWhite,new Rectangle(0,0,xStart,this.Height));
					//Highlight everything semi transparent white horizontally after the selection's ending x coordinate
					e.Graphics.FillRectangle(semiTransparentWhite,new Rectangle(xEnd,0,this.Width-xEnd,this.Height));
					//Highlight the remaining square above the selection semi transparent white vertically
					e.Graphics.FillRectangle(semiTransparentWhite,new Rectangle(xStart,0,xEnd-xStart,yStart));
					//Highlight the remaining square below the selection semi transparent white vertically
					e.Graphics.FillRectangle(semiTransparentWhite,new Rectangle(xStart,yEnd,xEnd-xStart,this.Height-yEnd));
				}
				//Outline the selection with a red border.  This matches Microsoft's snipping tool.
				using(Pen pen=new Pen(Color.Red,2)) {
					e.Graphics.DrawRectangle(pen,_rectSelection);
				}
			}
		}
	}
}
