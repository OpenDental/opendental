using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Thinfinity;
using OpenDental.UI;
using System.Drawing.Imaging;
using CodeBase.Controls;
using KnowledgeRequestNotification;
using WpfControls.UI;

namespace OpenDental {
	///<summary>Just a simple container for ControlImageDisplay when it's a floater rather than docked. Functionality that was here has been moved to ControlImageDisplay.</summary>
	public partial class FormImageFloat:FormODBase {
		#region Fields - Public
		public ControlImageDisplay ControlImageDisplay_;
		///<summary>This lets us get a list of all floater windows from ControlImages at the moment when we pop up the window selector.</summary>
		public Func<List<FormImageFloat>> FuncListFloaters;
		///<summary>This lets us get the title of the docker from ControlImages at the moment when we pop up the window selector.This can be null.</summary>
		public Func<string> FuncDockedTitle;
		#endregion Fields - Public
		
		#region Fields - Private
		///<summary></summary>
		private WindowImageFloatWindows _windowImageFloatWindows;
		///<summary>True when ImageFloatWindows is showing.</summary>
		private bool _isButWindowPressed;
		private bool _isClickLocked;
		///<summary>True if the mouse is currently over the "Windows" button at the top.</summary>
		private bool _isHotButWindow;
		///<summary>The bounds of the "Windows" button at the upper right.</summary>
		private Rectangle _rectangleButWindows;
		private System.Windows.Forms.Timer _timer;
		#endregion Fields - Private

		#region Constructor
		public FormImageFloat() {
			InitializeComponent();
			InitializeLayoutManager();
			Size size=this.Size;
		}
		#endregion Constructor

		#region Events
		///<summary>A button was clicked in WindowImageFloatWindows or in title bar. This event must bubble up to ControlImages where it's handled.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EnumImageFloatWinButton> EventButClicked=null;

		///<summary>User clicked on the list to pick a new window.  Bubbles up to ControlImages, where it's handled. The index passed includes the docker in position 0.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<int> EventWinPicked=null;
		#endregion Events

		#region Properties
		
		#endregion Properties

		#region Methods - Public
		public void SetControlImageDisplay(ControlImageDisplay controlImageDisplay){
			//should only be called once, after load.
			ControlImageDisplay_=controlImageDisplay;
			ControlImageDisplay_.EventGotODFocus-=ControlImageDisplay__EventGotODFocus;//safe to use even if that event handler is not attached.
			ControlImageDisplay_.EventGotODFocus+=ControlImageDisplay__EventGotODFocus;
			//Size sizeClient=PanelClient.Size;
			ControlImageDisplay_.Size=ClientRectangle.Size;
			ControlImageDisplay_.Dock=DockStyle.Fill;
			LayoutManager.Add(ControlImageDisplay_,this);
			LayoutManager.LayoutFormBoundsAndFonts(this);
			//Size sizeCtrl=ControlImageDisplay_.Size;
			//Size sizePanel=ControlImageDisplay_.panelMain.Size;
			//LayoutManager.LayoutControlBoundsAndFonts(ControlImageDisplay_);
		}

		///<summary>This tricks the form into thinking that the mouse down originated from within this form. Only used when we pop a floater into existence under the mouse and we want it to stick.</summary>
		public void SimulateMouseDown(Point point,Rectangle rectangleFormBounds){
			//MouseEventArgs mouseEventArgs=new MouseEventArgs(MouseButtons.Left,1,point.X,point.Y,delta:0);
			//base.PanelBorders_MouseDown(this,mouseEventArgs);//can't do this because...
			_pointMouseDownScreen=point;
			_pointMouseScreenPrevious=point;
			_rectangleOnDesktopMouseDown=rectangleFormBounds;
			_enumMouseDownRegion=EnumMouseDownRegion.Title;
			PanelBorders.Capture=true;
		}
		#endregion Methods - Public

		#region Methods - private Event Handlers
		private void ControlImageDisplay__EventGotODFocus(object sender,EventArgs e) {
			ControlImageDisplay controlImageDisplay=sender as ControlImageDisplay;
			Form form=controlImageDisplay.FindForm();
			if(form!=this){
				//The event was still attached even when the controlImageDisplay got moved from this floater
				//I'm not sure of any way to remove the event when moving the control or I would.
				return;
			}
			//IsImageFloatSelected=true;//this is what turns the titlebar blue in the base class
			Select();//Triggers Activated, which then sets IsImageFloatSelected=true
		}

		private void FormImageFloat_FormClosed(object sender, FormClosedEventArgs e){
			if(_windowImageFloatWindows!=null){
				_windowImageFloatWindows.Close();
			}
		}

		private void FormImageFloat_Load(object sender, EventArgs e){
			Size sizeClient=PanelClient.Size;
			Size sizeCtrl=ControlImageDisplay_.Size;
			Size sizePanel=ControlImageDisplay_.panelMain.Size;
			PanelBorders.MouseDown += PanelBorders_MouseDown;
			PanelBorders.MouseLeave += PanelBorders_MouseLeave;
			PanelBorders.MouseMove += PanelBorders_MouseMove;
			PanelBorders.MouseUp += PanelBorders_MouseUp;
			PanelBorders.Paint += PanelBorders_Paint;
		}

		private void FormImageFloat_LocationChanged(object sender, EventArgs e){
			//for testing
			Rectangle rectangleDTB=this.DesktopBounds;
			Rectangle rectangleB=this.Bounds;
		}

		private void _formImageFloatWindows_FormClosed(object sender,FormClosedEventArgs e) {
			_isButWindowPressed=false;
			PanelBorders.Invalidate();
		}

		protected override void OnResizeEnd(EventArgs e) {
			base.OnResizeEnd(e);
			//This used to be done in a much more complex way in LayoutManager_ZoomChanged.
			//This cannot be done in ControlImageDisplay because the method does not exist.
			//Also, we wouldn't need to reset zoom.
			ControlImageDisplay_.SetZoomSliderToFit();
			//this fires on the mouse up after moving a form or resizing it.
			//It does not fire while resizing, so that's nice.
			//The move could have been to a window of a different dpi.
			//It also fires when simply clicking on the titlebar, which is what resets zoom to fit each time. 
		}

		private void PanelBorders_MouseDown(object sender, MouseEventArgs e){
			//this fires after FormODBase.MouseDown.
			if(!_rectangleButWindows.Contains(e.Location)){
				return;
			}
			if(_isClickLocked){
				return;
			}
			//But this also causes FormODBase mouseUp to not register. User then clicks title to hide the menu, and that's when FormODBase MouseMove fires. 
			_pointMouseScreenPrevious=new Point(0,0);//Gets around the above problem.
			if(_isButWindowPressed){
				_isButWindowPressed=false;
				return;
			}
			_isButWindowPressed=true;
			_windowImageFloatWindows=new WindowImageFloatWindows();
			List<FormImageFloat> listFormImageFloats=FuncListFloaters();
			List<string> listStrings = new List<string>();
			string dockedTitle=FuncDockedTitle();
			if(dockedTitle is null) {
				listStrings.Add("(no image docked)");
			}
			else {
				listStrings.Add(dockedTitle);
			}
			for(int i=0;i<listFormImageFloats.Count;i++) {
				listStrings.Add(listFormImageFloats[i].Text);
			}
			_windowImageFloatWindows.ListFloaterTitles=listStrings;
			_windowImageFloatWindows.idxParent=listFormImageFloats.IndexOf(this)+1;//plus 1 because first pos is always occupied by docked
			_windowImageFloatWindows.EventButClicked+=(sender,enumImageFloatWinButton)=> EventButClicked?.Invoke(this,enumImageFloatWinButton);
			_windowImageFloatWindows.EventWinPicked+=(sender,idx)=>EventWinPicked?.Invoke(this,idx);
			_windowImageFloatWindows.Closed+=_windowImageFloatWindows_Closed;
			//Bottom left and right of the button, in screen coords.
			Point pointL=PointToScreen(new Point(_rectangleButWindows.Left,_rectangleButWindows.Bottom-LayoutManager.Scale(9)));
			Point pointR=PointToScreen(new Point(_rectangleButWindows.Right,_rectangleButWindows.Bottom-LayoutManager.Scale(9)));
			System.Windows.Point win_PointL=new System.Windows.Point(pointL.X,pointL.Y);
			System.Windows.Point win_PointR=new System.Windows.Point(pointR.X,pointR.Y);
			_windowImageFloatWindows.PointAnchor1=win_PointL;
			_windowImageFloatWindows.PointAnchor2=win_PointR;
			_windowImageFloatWindows.Show();//not a dialog.  They can click elsewhere
			_isButWindowPressed=true;
			PanelBorders.Invalidate();
		}

		private void PanelBorders_MouseLeave(object sender, EventArgs e){
			_isHotButWindow=false;
			PanelBorders.Invalidate();
		}
		
		private void PanelBorders_MouseMove(object sender, MouseEventArgs e){
			if(_rectangleButWindows.Contains(e.Location)){
				_isHotButWindow=true;//simpler algorithm than the other hot flags
			}
			else{
				_isHotButWindow=false;
			}
			PanelBorders.Invalidate();
		}

		private void PanelBorders_MouseUp(object sender, MouseEventArgs e){
			//nothing to do
		}

		private void PanelBorders_Paint(object sender, PaintEventArgs e){
			//This paint happens right after the base paint for this panel.
			Graphics g=e.Graphics;
			string strWindows=Lan.g(this,"Windows");
			int widthStr=(int)LayoutManager.ScaleMS(g.MeasureString(strWindows,Font).Width);
			_rectangleButWindows=new Rectangle(
				x:_rectangleButMin.Left-widthStr-LayoutManager.Scale(21),
				y:MaxInset()+LayoutManager.WidthBorder(),
				width:widthStr+LayoutManager.Scale(13),
				height:LayoutManager.GetHeightTitleBar());
			//these colors are copied from FormODBase.PanelBorders_Paint
			Color colorFloatBase=Color.FromArgb(65, 94, 154);//This is the default dark blue-gray, same as grid titles
			Color colorBorder=ColorOD.Mix(colorFloatBase,Color.White,1,3);
			Color colorBorderText=Color.Black;
			Color colorButtonHot=ColorOD.Mix(colorBorder,colorBorderText,10,1);
			if(IsImageFloatSelected){
				colorBorder=ColorOD.Mix(colorFloatBase,Color.White,3,1);
				colorBorderText=Color.White;
				colorButtonHot=ColorOD.Mix(colorBorder,colorBorderText,4,1);					
			}			
			if(_isHotButWindow){
				using SolidBrush solidBrushHover=new SolidBrush(colorButtonHot);
				g.FillRectangle(solidBrushHover,_rectangleButWindows);
			}
			if(_isButWindowPressed){
				g.FillRectangle(Brushes.White,_rectangleButWindows);
				g.DrawRectangle(Pens.Gray,_rectangleButWindows);//the bottom will get cut off
				colorBorderText=Color.Black;
			}
			using SolidBrush solidBrushText=new SolidBrush(colorBorderText);
			g.DrawString("Windows",Font,solidBrushText,new Point(_rectangleButWindows.X+LayoutManager.Scale(6),_rectangleButWindows.Y+LayoutManager.Scale(3)));
		}

		private void timer_Tick(object sender,EventArgs e) {
			_timer.Stop();
			_isClickLocked=false;
		}

		private void _windowImageFloatWindows_Closed(object sender,EventArgs e) {
			_isButWindowPressed=false;
			//if they clicked on the button to close, prevent that same click from opening the window back up again.
			_isClickLocked=true;
			_timer=new System.Windows.Forms.Timer();
			_timer.Interval=300;
			_timer.Tick+=timer_Tick;
			_timer.Start();
		}
		#endregion Methods - private Event Handlers

		#region Methods - Private

		#endregion Methods - Private
	}

	
}
