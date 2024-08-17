using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using OpenDental.UI;
using CodeBase;
using OpenDentBusiness;
using OpenDentalHelp;

namespace OpenDental {
	//Jordan is the only one allowed to alter this file.
	//This code was copied from FormODBase, and then stripped down.

	///<summary>This form is the frame around all WPF controls.</summary>
	public class FormFrame : Form {
		#region Fields - Public
		///<summary>Set to true to use traditional MS borders for all forms.</summary>
		public static bool AreBordersMS;
		///<summary>This will be true for PDF. This prevents dragging away from docked position.</summary>
		public bool IsImageFloatLocked;
		#endregion Fields - Public

		#region Fields - Private
		///<summary>If this is set in an inherited form, then the font of the title will be a different size.  This avoids touching form.Font because of ambient property complexities.</summary>
		protected Font _fontTitle=null;
		///<summary>Only true if FormClosed has been called by the system.</summary>
		private bool _hasClosed=false;
		///<summary>True when form has been shown by the system. Shown occurs last in the forms construction life cycle. The Shown event is only raised the first time a form is displayed.</summary>
		private bool _hasShown=false;
		///<summary>Property backer</summary>
		private bool _isImageFloatDocked=false;
		///<summary></summary>
		private bool _isImageFloatDragging=false;
		///<summary>Property backer</summary>
		private bool _isImageFloatSelected=false;
		///<summary>The thread that is run to check if filter controls have had their changes commited.  If a single control is considered to have commited changes then the thread will only fire the _filterAction once and then will wait for more input.</summary>
		private ODThread _threadFilter;	
		#endregion Fields - Private

		#region Fields - Private - Drawing, Border, Dpi
		private static Color _colorBorder=Color.FromArgb(65,94,154);//the same dark blue color as the grid titles
		private static Color _colorBorderText=Color.White; //Brushes.White;//new SolidBrush(Color.FromArgb(255,255,255));
		private static Color _colorRedX=Color.FromArgb(232,17,35);//181,22,35
		private static Color _colorButtonHot=Color.FromArgb(104,128,163);//slightly lighter blue/gray
		private static Color _colorBorderOutline=Color.Black;
		///<summary>This is the size and location of the Form when mouse down, based in pixels. If multiple screens, it's coordinates of the entire combined desktop.</summary>
		private Rectangle _rectangleOnDesktopMouseDown;
		///<summary>When using UIManager, this is the container for the canvas and all controls. It replaces PanelClient and the client area of the form.</summary>
		public ElementHost ElementHostUI;
		///<summary>If isMouseDown, then one of these is used to specify where.</summary>
		private EnumMouseDownRegion _enumMouseDownRegion;
		///<summary>Dispose handled automatically when this form closes.</summary>
		private FormSnap _formSnap;
		///<summary>The circle around the ?. Disposed.</summary>
		private GraphicsPath _graphicsPathHelp=new GraphicsPath();
		private bool _isDraggingTitle;
		private bool _isHotX;
		private bool _isHotMax;
		private bool _isHotMin;
		private bool _isHotHelp;
		///<summary>This panel is the same size as the form.  This is where all painting and mouse events happen.  The only reason we need to do this instead of painting directly on the form is because of a MS bug. The bug treats large portions of the window as the LowerR drag handle when the window is moved over to a high dpi screen.  This is very easy to duplicate on any simple new project, and it misbehaves across all situations, the only reqirements being high dpi with a dialog. We also use this panel with UIManager, but we cover up the L,R, and B.</summary>
		public PanelDoubleBuffered PanelBorders;
		///<summary>In screen coordinates.  Prevents drawing events unless mouse moves.</summary>
		protected Point _pointMouseScreenPrevious;
		///<summary>In screen coordinates.  For dragging.</summary>
		private Point _pointMouseDownScreen;
		private Rectangle _rectangleButtons;
		private Rectangle _rectangleButX;
		private Rectangle _rectangleButMax;
		protected Rectangle _rectangleButMin;
		private Rectangle _rectangleButHelp;
		///<summary>The circle around the ?. Disposed.</summary>
		private Region _regionButHelp=new Region();
		///<summary>This tracks when we hover over the maximize button to trigger the snap window. Disposed.</summary>
		private System.Windows.Forms.Timer timerHoverSnap;
		private ToolTip _toolTipBorderButtons;
		protected UIManager _uIManager;
		//<summary>Um is short for UIManager. This is how programmers interact with all controls on a form.</summary>
		//protected Um _um;
		///<summary>Keeps track of window state changes.  We use it to restore minimized forms to their previous state.</summary>
		private FormWindowState _windowStateOld;
		#endregion Fields - Private - Drawing, Border, Dpi

		#region Designer
		//InitializeComponent can't be in this base class because it causes changed in the inherited designer form, like center screen.

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				_graphicsPathHelp?.Dispose();
				_regionButHelp?.Dispose();
				timerHoverSnap?.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion Designer

		#region Constructor
		public FormFrame() {
			#region Designer Properties
			//these properties show as default values in the extended designer UIs
			//==Jordan is the only one who edits these because there are undocumented touchy consequences
			//Anything that's done in the Load below will show in the designer of derived forms.
			//For example, a property value here will be interpreted as a default in derived forms, meaning the designer will not preserve the value.
			//As another example, ClientSize can interfere with max size or min size
			this.AutoScaleMode=AutoScaleMode.None;//AutoScaleMode.Font is a huge problem.  Causes double resize.
			//this.ClientSize=new System.Drawing.Size(974,696);
			this.DoubleBuffered=true;
			this.KeyPreview=true;
			//this.MinimumSize=new Size(50,50);//An absurdly minimal size that still very nicely prevents windows from sizing to zero.
			this.Name="Form";
			this.StartPosition=FormStartPosition.CenterScreen;
			this.Text="Form";
			this.BackColor=ColorOD.Background;
			this.Font=new Font("Microsoft Sans Serif",8.25f);//fixes a lot of scaling issues.
			#endregion
			this.FormClosing+=new FormClosingEventHandler(this.ODForm_FormClosing);
			_toolTipBorderButtons=new ToolTip();
			_toolTipBorderButtons.ShowAlways=true;//even if form is disabled
			timerHoverSnap=new Timer();
			timerHoverSnap.Interval=400;//try to get this from Windows
			timerHoverSnap.Tick += timerHoverSnap_Tick;
		}
		#endregion Constructor

		#region Events
		///<summary></summary>
		protected void OnCloseXClicked() {
			CancelEventArgs cancelEventArgs=new CancelEventArgs();
			CloseXClicked?.Invoke(this,cancelEventArgs);
			if(cancelEventArgs.Cancel){
				return;
			}
			DialogResult=DialogResult.Cancel;
			Close();//closes even if it's not a dialog
		}

		[Category("OD")]
		[Description("Occurs when the X is clicked at the upper right of the form. In the event handler, set e.Cancel=true to cancel the close.")]
		public event CancelEventHandler CloseXClicked=null;

		[Category("OD")]
		[Description("Fires when IsImageFloatDocked changes.")]
		public event EventHandler IsImageFloatDockedChanged;
		#endregion Events

		#region Properties
		[Description("Jordan-Written to force this to always be treated as None, regardless of what it shows.")]
		[Category("Layout")]
		[DefaultValue(AutoScaleMode.None)]
		public new AutoScaleMode AutoScaleMode { 
			//AutoScaleMode is an older dpi scaling feature of WinForms.  It was terrible and we ignore it.  In at least one case, having this set to Font caused form resizing issues.  The way this is written forces all forms to be None.  The designer of older forms might still show Font, but that's harmless and ignored.
			get{
				return AutoScaleMode.None;
			}
			set{
				base.AutoScaleMode=AutoScaleMode.None;
			}
		}

		///<summary>Default true. Set to false to hide the help button.  Window 'HelpButton' property is ignored.</summary>
		[Category("OD")]
		[Description("Default true. Set to false to hide the help button.  Windows 'HelpButton' property is ignored.")]
		[DefaultValue(true)]
		public bool HasHelpButton {get;set;}=true;

		///<summary>True when form has been shown by the system.</summary>
		[Browsable(false)]
		public bool HasShown {
			get {
				return _hasShown;
			}
		}

		///<summary>Only true if FormClosed has been called by the system.  No references, but used in IsDisposedOrClosed via reflection.</summary>
		[Browsable(false)]
		public bool HasClosed {
			get {
				return _hasClosed;
			}
		}

		///<summary>Set to true for Kiosk to block user from dragging or clicking.</summary>
		[Category("OD")]
		[Description("Set to true for Kiosk to block user from dragging or clicking.")]
		[DefaultValue(false)]
		public bool IsBorderLocked {get;set;}=false;

		///<summary>True if this form is docked.  Docking is discussed at the top of FormImageFloat.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsImageFloatDocked {
			//We use this instead of the built-in focus/active paradigm because we want the form to be selected even when it doesn't have focus.
			//Active means that it or one of its children has focus, so it's nearly identical to focus, but works better for forms where children have actual focus.
			get{
				return _isImageFloatDocked;
			}
			set{
				if(value){//true
					ShowInTaskbar=false;
				}
				else{//false
					if(!ODBuild.IsWeb()) {
						//This causes the form to refresh and in cloud the attached iframe loses the image and you have to reselect the image to make it show again.
						//Also, there is no need to show in taskbar for cloud
						ShowInTaskbar=true;
					}
				}
				_isImageFloatDocked=value;
				IsImageFloatDockedChanged?.Invoke(this,new EventArgs());
			}
		}

		///<summary>True if this form is the "selected" image float.  Only one should be selected at a time.  Any image float that is not selected  will have the title bar turn white to indicate so.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsImageFloatSelected {
			//We use this instead of the built-in focus/active paradigm because we want the form to be selected even when it doesn't have focus.
			//Active means that it or one of its children has focus, so it's nearly identical to focus, but works better for forms where children have actual focus.
			get{
				return _isImageFloatSelected;
			}
			set{
				_isImageFloatSelected=value;
				PanelBorders?.Invalidate();
			}
		}

		///<summary</summary>
		[Category("Appearance")]
		[Description("The text associated with the control.")]
		[DefaultValue("")]
		public override string Text{
			get{
				return base.Text;
			}
			set{
				base.Text=value;
				PanelBorders?.Invalidate();
			}
		}
		#endregion Properties

		#region Methods - Event Handlers
		///<summary>Fires first for all FormClosing events of this form.</summary>
		private void ODForm_FormClosing(object sender,FormClosingEventArgs e) {
			//FormClosed event added to list of closing events as late as possible.
			//This allows the implementing form to set another FormClosing event to be fired before our base event here. 
			//(Jordan-This is worded wrong. Base.FormClosing will still fire before Derived.FormClosing. Maybe they meant FormClosed?)
			//The advantage is that HasClosed will only be true if ALL FormClosing events have fired for this form.
			this.FormClosed+=ODForm_FormClosed;
		}

		private void ODForm_FormClosed(object sender,FormClosedEventArgs e) {
			if(_threadFilter!=null) {
				_threadFilter.QuitAsync();//It's fine if our thread loop finishes, it protects against unhandled exceptions.
				_threadFilter=null;
				//We don't want an enumeration exception here so don't clear _listFilterControls. It will get garbage collected anyways.
			}
			_hasClosed=true;
		}

		protected override void OnLoad(EventArgs e){
			if(DesignMode){
				return;
			}
			bool isMaximized=false;
			if(WindowState==FormWindowState.Maximized){
				isMaximized=true;
			}
			System.Windows.Forms.Screen screen0=System.Windows.Forms.Screen.FromControl(this);//automatically returns screen that contains largest portion of this form
			int dpiScreen0=Dpi.GetScreenDpi(screen0);//extern DllImport
			float scaleMS=(float)dpiScreen0/96f;//example 1.5
			_uIManager.SetScaleMS(scaleMS);
			_uIManager.SetElementHost();
			PanelBorders.SendToBack();
			PanelBorders.Paint+=PanelBorders_Paint;
			PanelBorders.MouseDoubleClick+=PanelBorders_MouseDoubleClick;
			PanelBorders.MouseLeave+=PanelBorders_MouseLeave;
			PanelBorders.MouseDown+=PanelBorders_MouseDown;
			PanelBorders.MouseMove+=PanelBorders_MouseMove;
			PanelBorders.MouseUp+=PanelBorders_MouseUp;
			PanelBorders.Invalidate();
			if(!isMaximized){
				//int widthBorder=(Width-_uIManager.SizeClientOriginal.Width)/2;//for testing
				//Border thickness will vary based on MS scale.  We could use it here  directly, except that won't work when changing dpi.
				//Here's how it behaves: At 96dpi: 8. At 125%: 9. At 150%: 11. At 200%: 13
				//Not a great pattern to work with, but the following math is accurate in all four of those cases.
				//This same math must be used in other places.
				int widthBorder=(int)Math.Round(8+(5*(_uIManager.GetScaleMS()-1)),MidpointRounding.AwayFromZero);
				int widthNew = _uIManager.Scale(_uIManager.SizeClientOriginal.Width)+widthBorder*2;//includes Zoom
				int heightNew = _uIManager.Scale(_uIManager.SizeClientOriginal.Height)+widthBorder+_uIManager.GetHeightTitleBar();
				Rectangle boundsNew = Bounds;
				boundsNew.X-=(widthNew-Width)/2;
				boundsNew.Y-=(heightNew-Height)/2;
				boundsNew.Width=widthNew;
				boundsNew.Height=heightNew;
				Bounds=boundsNew;
				//panelBorders and ElementHostingUI are laid out in UIManager.LayoutFormBoundsAndFonts()
			}
			_uIManager.LayoutFormBoundsAndFonts();
			base.OnLoad(e);
			ElementHostUI.Select();//this lets the hover effects work properly on menus and toolbars.
			_uIManager.FrmODBaseHosted.Focus();//so that keystrokes will work
		}

		protected override void OnResize(EventArgs e){
			//Even when we draw our own border, this gets fired automatically from the WndProc message.
			base.OnResize(e);
			if(ElementHostUI!=null){
				_uIManager.LayoutFormBoundsAndFonts();
			}
			if(WindowState!=FormWindowState.Minimized) {
				_windowStateOld=WindowState;
			}
			PanelBorders?.Invalidate();
		}

		protected override void OnResizeBegin(EventArgs e){
			//Even when we draw our own border, this gets fired automatically from the WndProc message.
			base.OnResizeBegin(e);
		}

		protected override void OnResizeEnd(EventArgs e){
			//This also normally fires on MouseUp when moving a form.
			//Because we draw our own border, we manually fire this from MouseUp.
			base.OnResizeEnd(e);
			PanelBorders?.Invalidate();
		}

		private void timerHoverSnap_Tick(object sender, EventArgs e){
			//check one more time to make sure we are still actually hovering over the maximize button
			//But this check isn't working for some reason
			//Point pointMouse=PointToClient(Control.MousePosition);
			//if(!_rectangleButMin.Contains(pointMouse)){
			//	timerHoverSnap.Enabled=false;
			//	return;
			//}
			timerHoverSnap.Enabled=false;
			//The code below works fine, but is incomplete, so this feature will be hidden for now
			return;
			if(_formSnap!=null && !_formSnap.IsDisposed){
				_formSnap.Show();
				return;
			}
			_formSnap=new FormSnap();
//todo: close when lose focus.
//todo: close when this form closes.
			_formSnap.PointAnchor=new Point(
				Location.X+_rectangleButMax.Right-_rectangleButMax.Width/2,
				Location.Y+_rectangleButMax.Bottom);
			_formSnap.Show();//not a dialog.  They can click elsewhere
		}
		#endregion Methods - Event Handlers

		#region Methods - Public
		public void CenterFormOnMonitor(){
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;
			this.Location=new Point(
				rectangleWorkingArea.X+rectangleWorkingArea.Width/2-Width/2,
				rectangleWorkingArea.Y+rectangleWorkingArea.Height/2-Height/2
				);
		}

		///<summary>Sets the entire form into "read only" mode by disabling all controls on the form. Pass in any controls that should say enabled (e.g. Cancel button). This can be used to stop users from clicking items they do not have permission for.</summary>
		public void DisableAllExcept(){//params Control[] enabledControls) {
			//todo: implement for wpf
			//foreach(Control ctrl in PanelClient.Controls) {
			//	if(enabledControls.Contains(ctrl)) {
			//		continue;
			//	}
			//	//Attempt to disable the control.
			//	try {
			//		ctrl.Enabled=false;
			//	}
			//	catch(Exception ex) {
			//		//Some controls do not support being disabled.  E.g. the WebBrowser control will throw an exception here.
			//		ex.DoNothing();
			//	}
			//}
		}

		///<summary>Returns true if the form passed in has been disposed or if it extends ODForm and HasClosed is true.</summary>
		public static bool IsDisposedOrClosed(Form form) {
			//todo: implement for wpf
			//if(form.IsDisposed) {//Usually the system will set IsDisposed to true after a form has closed.  Not true for FormHelpBrowser.
			//	return true;
			//}
			//if(form.GetType().GetProperty("HasClosed")!=null) {//Is a Form and has property HasClosed => Assume is an ODForm.
			//	//Difficult to compare type to ODForm, because it is a template class.
			//	if((bool)form.GetType().GetProperty("HasClosed").GetValue(form)) {//This is how we know FormHelpBrowser has closed.
			//		return true;
			//	}
			//}
			return false;
		}

		///<summary>If form is minimized, this restores it to either normal or maximized, depending on previous state.</summary>
		public void Restore() {
			if(WindowState==FormWindowState.Minimized) {
				WindowState=_windowStateOld;
			}
		}

		///<summary>Before minimizing or maximizing a window, we need to reduce width and height by 16 and 39 pixels.  This allows the subsequent restore to be the correct size.  Otherwise, window gets slightly bigger with each restore.</summary>
		public void ShrinkWindowBeforeMinMax() {
			if(AreBordersMS){
				return;//No need to shrink when we don't control the layout
			}
			//Size=new Size(Width-16,Height-39);//these numbers are the MS border widths.
			Size=new Size(Width,Height-31);
		}
		#endregion Methods - Public

		#region Border Drawing
		///<summary>When maximized, this is the additional inset of panelClient on all 4 sides to compensate for the perimeter getting cut off. This doesn't get scaled.</summary>
		protected int MaxInset(){
			/*if(UsingUIManager){
				return 0;
			}
			if(WindowState==FormWindowState.Maximized){
				return LayoutManager.MaxInset;
			}*/
			//Deprecated
			return 0;
		}

		/*
		public struct MARGINS{
			public int leftWidth;
			public int rightWidth;
			public int topHeight;
			public int bottomHeight;
		}

		
		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int attrSize);

		[DllImport("user32.dll")]//this is the 64 bit version 
		private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
		private int GWL_STYLE=-16;//window style
		private int GWL_EXSTYLE=-20;//extended window style
		private int WS_BORDER=0x008;//Thin line border
		private int WS_THICKFRAME=0x0004;
		private int WS_DLGFRAME=0x004;//dialog box with no titlebar
		private int WS_VSCROLL=0x002;

		[DllImport("user32.dll", EntryPoint="GetWindowLong")]//this is the 64 bit version 
		private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError=true)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, Int32 uFlags);
		private int SWP_FRAMECHANGED=0x0020;
		private int SWP_NOMOVE=0x0002;
		private int SWP_NOSIZE=0x0001;
		private int SWP_NOZORDER=0x0004;
		private int SWP_NOOWNERZORDER=0x0200;*/

		public void ShowShadows(){
			//Deprecated
			/*
			if(UsingUIManager){
				//https://stackoverflow.com/questions/2398746/removing-window-border
				//int lStyle = GetWindowLongPtr(Handle, GWL_STYLE).ToInt32();
				//int lStyleNew=lStyle | WS_DLGFRAME;
				//SetWindowLongPtr(Handle,GWL_STYLE,new IntPtr(lStyleNew));
				//SetWindowPos(Handle, IntPtr.Zero, 0,0,0,0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER);
				return;
			}
			if(AreBordersMS){
				return;
			}
			//If this is put at top of constructor, it won't work for dialogs. Not sure why.  Works well in Load, for now.
			int DWMWA_NCRENDERING_POLICY=2;//sets the non-client rendering policy
			int DWMNCRP_ENABLED=2;//The non-client area rendering is enabled; the window style (borderless) is ignored.
			DwmSetWindowAttribute(this.Handle,DWMWA_NCRENDERING_POLICY,ref DWMNCRP_ENABLED, 4);
			MARGINS margins = new MARGINS(){
				bottomHeight = 1,//this pixel of intrusion is not noticed because it's covered by our bottom border.
				leftWidth = 0,
				rightWidth = 0,
				topHeight = 0
			};
			DwmExtendFrameIntoClientArea(this.Handle, ref margins);//creates a tiny border to trigger DWM*/
		}

		public void InitializeFormMaker(FrmODBase frmODBase){
			_uIManager=new UIManager(this,frmODBase);//using this overload avoids creating proxies or any of that
		}

		private void PanelBorders_Paint(object sender, PaintEventArgs e){
			if(DesignMode){
				return;
			}
			if(ElementHostUI==null){
				return;
			}
			Graphics g=e.Graphics;//no dispose ref
			if(Width<1 || Height<1){
				return;
			}
			Color colorBorder=_colorBorder;
			Color colorBorderText=_colorBorderText;
			Color colorButtonHot=_colorButtonHot;
			if(this.GetType().ToString() == "FormImageFloat"){
				//They don't get a choice on these colors because that's too hard
				Color colorFloatBase=Color.FromArgb(65, 94, 154);//This is the default dark blue-gray, same as grid titles
				if(IsImageFloatSelected){
					colorBorder=ColorOD.Mix(colorFloatBase,Color.White,3,1);
					colorBorderText=Color.White;
					colorButtonHot=ColorOD.Mix(colorBorder,colorBorderText,4,1);					
				}
				else{
					colorBorder=ColorOD.Mix(colorFloatBase,Color.White,1,3);
					colorBorderText=Color.Black;
					colorButtonHot=ColorOD.Mix(colorBorder,colorBorderText,10,1);
				}
			}
			using SolidBrush brushBorder=new SolidBrush(colorBorder);
			using SolidBrush brushText=new SolidBrush(colorBorderText); 
			using SolidBrush brushRedX=new SolidBrush(_colorRedX);
			using SolidBrush brushButtonHot=new SolidBrush(colorButtonHot);
			using Pen penButtons=new Pen(colorBorderText);
			using Pen penOutline=new Pen(_colorBorderOutline);
			g.FillRectangle(brushBorder,new Rectangle(0,0,Width,Height));
			/*
			//Deprecated
			//this next rectangle makes the border appear to be 3 pixels instead of 5, while still having a grab handle of 5.
			int widthRemove=Scale(2);
			if(UsingUIManager){
				//no need
			}
			else{
				g.FillRectangle(SystemBrushes.Control,new Rectangle(PanelClient.Left-widthRemove,PanelClient.Top,
					PanelClient.Width+(widthRemove*2),PanelClient.Height+widthRemove));
			}*/
			g.DrawRectangle(penOutline,MaxInset(),MaxInset(),Width-1-MaxInset()*2,Height-1-MaxInset()*2);
			int wEdge=GetHeightTitleBar()+ScaleI(4);//ours are nearly square instead of default wide
			int hEdge=GetHeightTitleBar()-1;
			int xPos=Width-WidthBorder()-MaxInset();
			int yPos=5+MaxInset();
				//1+MaxInset();
			if(ControlBox){//there should always be a controlBox
				xPos-=wEdge;
				_rectangleButX=new Rectangle(xPos,yPos,wEdge,hEdge);
			}
			if(ControlBox && MaximizeBox){
				xPos-=wEdge;
				_rectangleButMax=new Rectangle(xPos,yPos,wEdge,hEdge);
			}
			if(ControlBox && MinimizeBox){
				xPos-=wEdge;
				_rectangleButMin=new Rectangle(xPos,yPos,wEdge,hEdge);
			}
			if(ControlBox && HasHelpButton){
				xPos-=wEdge;
				_rectangleButHelp=new Rectangle(xPos,yPos-2,GetHeightTitleBar()-4,GetHeightTitleBar()-4);
			}
			_rectangleButtons=new Rectangle(xPos,yPos,Width-xPos,GetHeightTitleBar());
			_graphicsPathHelp?.Dispose();
			_graphicsPathHelp=new GraphicsPath();
			_graphicsPathHelp.AddEllipse(_rectangleButHelp);
			_regionButHelp?.Dispose();
			_regionButHelp=new Region(_graphicsPathHelp);
			g.SmoothingMode=SmoothingMode.Default;
			//button glyphs are all 10x10
			int indentTop=ScaleI(5);//7
			int indentLeft=ScaleI(10);
				//indentTop+Scale(3);
			//Button X:
			if(_isHotX){
				g.FillRectangle(brushRedX,_rectangleButX);
			}
			g.SmoothingMode=SmoothingMode.HighQuality;
			//X is higher quality and thicker line.  Due to GDI+ bug, 1.4 line width is not possible, so drawing 2 lines for each to simulate
			int whSymbol=ScaleI(9);
			g.DrawLine(penButtons,_rectangleButX.X+indentLeft-0.2f,_rectangleButX.Y+indentTop,_rectangleButX.X+indentLeft+whSymbol-0.2f,_rectangleButX.Y+indentTop+whSymbol);
			g.DrawLine(penButtons,_rectangleButX.X+indentLeft+0.2f,_rectangleButX.Y+indentTop,_rectangleButX.X+indentLeft+whSymbol+0.2f,_rectangleButX.Y+indentTop+whSymbol);
			g.DrawLine(penButtons,_rectangleButX.X+whSymbol+indentLeft-0.2f,_rectangleButX.Y+indentTop,_rectangleButX.X+indentLeft-0.2f,_rectangleButX.Y+indentTop+whSymbol);
			g.DrawLine(penButtons,_rectangleButX.X+whSymbol+indentLeft+0.2f,_rectangleButX.Y+indentTop,_rectangleButX.X+indentLeft+0.2f,_rectangleButX.Y+indentTop+whSymbol);
			g.SmoothingMode=SmoothingMode.Default;
			//Button Max:
			if(MaximizeBox){
				if(_isHotMax){
					g.FillRectangle(brushButtonHot,_rectangleButMax);
				}
				if(WindowState==FormWindowState.Maximized){//show the double squares, each 8x8
					g.DrawRectangle(penButtons,_rectangleButMax.X+indentLeft,_rectangleButMax.Y+indentTop+ScaleI(2),ScaleI(7),ScaleI(7));
					//second box is behind to the UR of first.  Draw clockwise.
					g.DrawLine(penButtons,_rectangleButMax.X+indentLeft+ScaleI(2),_rectangleButMax.Y+indentTop+ScaleI(2),
						_rectangleButMax.X+indentLeft+ScaleI(2),_rectangleButMax.Y+indentTop);//L
					g.DrawLine(penButtons,_rectangleButMax.X+indentLeft+ScaleI(2),_rectangleButMax.Y+indentTop,
						_rectangleButMax.X+indentLeft+ScaleI(9),_rectangleButMax.Y+indentTop);//T
					g.DrawLine(penButtons,_rectangleButMax.X+indentLeft+ScaleI(9),_rectangleButMax.Y+indentTop,
						_rectangleButMax.X+indentLeft+ScaleI(9),_rectangleButMax.Y+indentTop+ScaleI(7));//R
					g.DrawLine(penButtons,_rectangleButMax.X+indentLeft+ScaleI(9),_rectangleButMax.Y+indentTop+ScaleI(7),
						_rectangleButMax.X+indentLeft+ScaleI(7),_rectangleButMax.Y+indentTop+ScaleI(7));//B
				}
				else{
					g.DrawRectangle(penButtons,_rectangleButMax.X+indentLeft,_rectangleButMax.Y+indentTop,whSymbol,whSymbol);
				}
			}
			//Button Min:
			if(MinimizeBox){
				if(_isHotMin){
					g.FillRectangle(brushButtonHot,_rectangleButMin);
				}
				g.DrawLine(penButtons,_rectangleButMin.X+indentLeft,_rectangleButMin.Y+indentTop+whSymbol/2,_rectangleButMin.X+indentLeft+whSymbol,_rectangleButMin.Y+indentTop+whSymbol/2);
			}
			//Button Help:
			//Had trouble with the ? getting bigger each time drawn.  There's bad logic somewhere, so we will ignore Font.
			float fontSize=ScaleFontODZoom(8.25f);
			float fontSizeMainWindow=ScaleFontODZoom(13f);
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromControl(this);
			System.Windows.Forms.Screen screenPri=System.Windows.Forms.Screen.PrimaryScreen;
			float scaleRatio=1;
			if(screen!=screenPri){//if we are on the secondary screen
				int dpiScreen=Dpi.GetScreenDpi(screen);//extern DllImport. Example 96
				int dpiScreenPri=Dpi.GetScreenDpi(screenPri);//Example 144
				scaleRatio=(float)dpiScreen/dpiScreenPri;//Example .67
				if(scaleRatio!=1){//example, pri screen is 150%, so it makes the fonts on this screen too big
					//We need to make them smaller
					fontSize=fontSize*scaleRatio;
					fontSizeMainWindow=fontSizeMainWindow*scaleRatio;
				}
			}
			using Font font=new Font("Microsoft Sans Serif",fontSize);
			using Font fontMainWindow=new Font("Microsoft Sans Serif",fontSizeMainWindow);
			float heightFont=ScaleMS(font.Height);
			float heightFontMainWindow=ScaleMS(fontMainWindow.Height);
			if(scaleRatio!=1){
				//special edge case where the font is smaller than what we need to measure
				//scale it again?
				heightFont=heightFont/scaleRatio;
				heightFontMainWindow=heightFontMainWindow/scaleRatio;
			}
			if(HasHelpButton){
				if(_isHotHelp){
					g.SmoothingMode=SmoothingMode.HighQuality;
					g.FillPath(brushButtonHot,_graphicsPathHelp);//using a path because can't antialias a region
					g.SmoothingMode=SmoothingMode.Default;
					//g.FillRectangle(_brushButtonHot,_rectangleButQuest);
				}
				g.DrawString("?",font,brushText,_rectangleButHelp.X+ScaleI(7),_rectangleButHelp.Y+ScaleI(5));
			}
			Rectangle rectangleIcon=new Rectangle(MaxInset()+5,MaxInset()+4,ScaleI(20),ScaleI(20));
			if(ShowIcon){
				//for some reason, I couldn't get form.Icon to work.
				Icon icon=WpfControls.Properties.Resources.Icon;
				g.DrawImage(icon.ToBitmap(),rectangleIcon);
			}
			if(_fontTitle==null){
				RectangleF rectangleText=new RectangleF(MaxInset()+ScaleF(30),MaxInset()+ScaleF(7),
				_rectangleButtons.Left-ScaleF(30)-MaxInset()+ScaleF(8),heightFont+2);//The 8 is just so we can get closer to the right
				g.DrawString(this.Text,font,brushText,rectangleText);
			}
			else{
				//We actually ignore the font size sent in
				RectangleF rectangleText=new RectangleF(
					x:MaxInset()+ScaleF(30),
					y:MaxInset()+ScaleF(3),
					width:_rectangleButtons.Left-ScaleF(30)-MaxInset()+ScaleF(8),//The 8 is just so we can get closer to the right
					height:heightFontMainWindow+2);
				g.DrawString(this.Text,fontMainWindow,brushText,rectangleText);
			}
		}

		///<summary>Takes one of the 3 border colors at a time.</summary>
		protected void SetBorderColor(DefCatMiscColors defCatMiscColors,Color color){
			if(defCatMiscColors==DefCatMiscColors.MainBorder){
				_colorBorder=color;
			}
			if(defCatMiscColors==DefCatMiscColors.MainBorderOutline){
				_colorBorderOutline=color;
			}
			if(defCatMiscColors==DefCatMiscColors.MainBorderText){
				_colorBorderText=color;
			}
			if(defCatMiscColors==DefCatMiscColors.MainBorder || defCatMiscColors==DefCatMiscColors.MainBorderText){ 
				_colorButtonHot=Color.FromArgb(
					(3*_colorBorder.R+_colorBorderText.R)/4,//mostly main border color, with just some of the text color
					(3*_colorBorder.G+_colorBorderText.G)/4,
					(3*_colorBorder.B+_colorBorderText.B)/4);
			}
			PanelBorders?.Invalidate();
		}
		#endregion Border Drawing

		#region Border Mouse
		private void PanelBorders_MouseDoubleClick(object sender, MouseEventArgs e){
			base.OnMouseDoubleClick(e);
			if(WindowState==FormWindowState.Maximized){
				return;
			}
			if(e.Y>_uIManager.GetHeightTitleBar()){
				return;
			}		
			if(e.X>_rectangleButtons.X-2){
				return;
			}
			if(Cursor.Current==Cursors.SizeNS) {//The up-down arrows are showing, in this case only maximize the height of the window
				System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromHandle(this.Handle);
				Bounds=new Rectangle(this.Location.X,0,Size.Width,screen.WorkingArea.Height);
				return;
			}
			//double clicked anywhere else:
			if(this.GetType().ToString()=="FormImageFloat" && IsImageFloatLocked){
				MsgBox.Show(this,"PDFs cannot be undocked.  Double click to open in PDF viewer.");
				return;
			}
			//Windows will not reliably restore the size after maximize.  It gets bigger each time.  We need to trick it by resizing the window before maximizing.
			//This does not cause any flicker
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Maximized;
		}

		///<summary>If isMouseDown, then one of these is used to specify where.</summary>
		private enum EnumMouseDownRegion{
			///<summary>Dragging entire form</summary>
			Title,
			Buttons,
			///<summary>Resizing top edge.</summary>
			Top,
			Left,
			Right,
			Bottom,
			UL,
			UR,
			LL,
			LR
		}

		private void PanelBorders_MouseDown(object sender, MouseEventArgs e){
			if(IsBorderLocked){
				return;
			}
			int widthBorder=5;
			if(e.X<MaxInset()+widthBorder+2 && e.Y<MaxInset()+widthBorder+2){//UL corner
				_enumMouseDownRegion=EnumMouseDownRegion.UL;
			}
			else if(e.X>Width-widthBorder-MaxInset()-1 && e.Y<MaxInset()+widthBorder+2){//UR corner
				_enumMouseDownRegion=EnumMouseDownRegion.UR;
			}
			else if(e.X<MaxInset()+widthBorder+2 && e.Y>Height-widthBorder-MaxInset()-3){//LL corner
				_enumMouseDownRegion=EnumMouseDownRegion.LL;
			}
			else if(e.X>Width-widthBorder-MaxInset()-3 && e.Y>Height-widthBorder-MaxInset()-3){//LR corner
				_enumMouseDownRegion=EnumMouseDownRegion.LR;
			}
			else if(e.X<MaxInset()+widthBorder+1){//left
				_enumMouseDownRegion=EnumMouseDownRegion.Left;
			}
			else if(e.X>Width-widthBorder-MaxInset()-1){//right
				_enumMouseDownRegion=EnumMouseDownRegion.Right;
			}
			else if(e.Y>Height-widthBorder-MaxInset()-1){//bottom
				_enumMouseDownRegion=EnumMouseDownRegion.Bottom;
			}
			else if(e.Y<MaxInset()+widthBorder){//top
				_enumMouseDownRegion=EnumMouseDownRegion.Top;
			}
			else if(e.X>_rectangleButtons.X-2){
				_enumMouseDownRegion=EnumMouseDownRegion.Buttons;
			}
			else{
				_enumMouseDownRegion=EnumMouseDownRegion.Title;
			}
			_pointMouseDownScreen=PanelBorders.PointToScreen(e.Location);
			_pointMouseScreenPrevious=PanelBorders.PointToScreen(e.Location);//required, see notes in MouseMove
			_rectangleOnDesktopMouseDown=DesktopBounds;
		}

		private void PanelBorders_MouseLeave(object sender, EventArgs e){
			Cursor=Cursors.Default;
			_isHotX=false;
			_isHotMax=false;
			_isHotMin=false;
			_isHotHelp=false;
			_toolTipBorderButtons.SetToolTip(this,"");
			PanelBorders.Invalidate();
		}

		private void PanelBorders_MouseMove(object sender, MouseEventArgs e){
			if(_uIManager==null){
				return;
			}
			if(IsBorderLocked){
				return;
			}
			MouseButtons mouseButtons=Control.MouseButtons;//introducing variables for debugging
			Point pointMouse=Control.MousePosition;
			if(Control.MouseButtons==MouseButtons.Left){//mouse is down
				if(_pointMouseScreenPrevious==new Point(0,0)){
					//Handles some edge cases:
					//1. If any combobox is open, and user clicks on title.  MouseMove registers with no MouseDown. Can't remember how 0,0 gets set to handle this.  Maybe this was only tested on a window that had not been dragged.
					//2. If FormImageFloat, user clicks Windows button in title, mouseUp does not register. User then clicks title to hide the menu, and that's when MouseMove fires. To get around this, 0,0 gets set in FormImageFloat.PanelBorders_MouseDown.
					return;
				}
				if(_pointMouseScreenPrevious==Control.MousePosition){//no actual mouse movement
					return;
				}
			}
			#region Cursors
			int widthBorder=5;
			if(e.X<MaxInset()+widthBorder+2 && e.Y<MaxInset()+widthBorder+2){//UL corner
				Cursor=Cursors.SizeNWSE;
			}
			else if(e.X>Width-widthBorder-MaxInset()-1 && e.Y<MaxInset()+widthBorder+2){//UR corner
				Cursor=Cursors.SizeNESW;
			}
			else if(e.X<MaxInset()+widthBorder+2 && e.Y>Height-widthBorder-MaxInset()-3){//LL corner
				Cursor=Cursors.SizeNESW;
			}
			else if(e.X>Width-widthBorder-MaxInset()-3 && e.Y>Height-widthBorder-MaxInset()-3){//LR corner
				Cursor=Cursors.SizeNWSE;
			}
			else if(e.X<MaxInset()+widthBorder+1){//left
				Cursor=Cursors.SizeWE;
			}
			else if(e.X>Width-widthBorder-MaxInset()-1){//right
				Cursor=Cursors.SizeWE;
			}
			else if(e.Y<MaxInset()+widthBorder){//&& e.X<_rectangleButtons.X){//top
				Cursor=Cursors.SizeNS;
			}
			else if(e.Y>Height-widthBorder-MaxInset()-1){//bottom
				Cursor=Cursors.SizeNS;
			}
			else{
				Cursor=Cursors.Default;
			}
			#endregion Cursors
			#region Button Hover Effects
			timerHoverSnap.Enabled=false;//we will turn it back on if we are within the max button
			bool isInHelp=false;
			if(HasHelpButton && _regionButHelp!=null){
				try{
					isInHelp=_regionButHelp.IsVisible(e.Location);
				}
				catch{ }//region could be disposed or other similar issues
			}
			if(_rectangleButX.Contains(e.Location)){
				if(!_isHotX){//first time
					_toolTipBorderButtons.InitialDelay=1000;
					_toolTipBorderButtons.SetToolTip(this,Lans.g(this,"Close"));
					_isHotX=true;
					_isHotMax=false;
					_isHotMin=false;
					_isHotHelp=false;
					PanelBorders.Invalidate(_rectangleButtons);
				}
			}
			else if(MaximizeBox && _rectangleButMax.Contains(e.Location)){
				if(!_isHotMax){
					_toolTipBorderButtons.InitialDelay=1000;
					if(WindowState==FormWindowState.Maximized){
						_toolTipBorderButtons.SetToolTip(this,Lans.g(this,"Restore Down"));
					}
					else{
						_toolTipBorderButtons.SetToolTip(this,Lans.g(this,"Maximize"));
					}
					_isHotMax=true;
					_isHotX=false;
					_isHotMin=false;
					_isHotHelp=false;
					PanelBorders.Invalidate(_rectangleButtons);
				}
				//timerHoverSnap.Enabled=false;//was already done above
				timerHoverSnap.Enabled=true;
			}
			else if(MinimizeBox && _rectangleButMin.Contains(e.Location)){
				if(!_isHotMin){
					_toolTipBorderButtons.InitialDelay=1000;
					_toolTipBorderButtons.SetToolTip(this,Lans.g(this,"Minimize"));
					_isHotMin=true;
					_isHotX=false;
					_isHotMax=false;
					_isHotHelp=false;
					PanelBorders.Invalidate(_rectangleButtons);
				}
			}
			else if(isInHelp){
				if(!_isHotHelp){
					_toolTipBorderButtons.InitialDelay=0;
					_toolTipBorderButtons.SetToolTip(this,Lans.g(this,"Help"));
					_isHotHelp=true;
					_isHotX=false;
					_isHotMax=false;
					_isHotMin=false;
					PanelBorders.Invalidate(_rectangleButtons);
				}
			}
			else{
				if(_isHotX || _isHotMax || _isHotMin || _isHotHelp){
					_toolTipBorderButtons.SetToolTip(this,"");
					_isHotX=false;
					_isHotMax=false;
					_isHotMin=false;
					_isHotHelp=false;
					PanelBorders.Invalidate(_rectangleButtons);
				}
			}
			//PanelBorders.Invalidate(false);//this causes flicker for the entire form, especially listbox. Whether true or false.
			//But putting PanelBorders.Invalidate() after SetDesktopBounds below does not cause flicker
			#endregion Button Hover Effects
			#region Taskbar
			if(WindowState==FormWindowState.Maximized && e.Y>Height-MaxInset()-2){
				IntPtr hWnd=FindWindow("Shell_TrayWnd", "");
				if(hWnd!=IntPtr.Zero){
					APPBARDATA appBarData = new APPBARDATA();
					appBarData.cbSize = Marshal.SizeOf(appBarData);
					uint uState=(uint)SHAppBarMessage(ABM_GETSTATE,ref appBarData);
					//I'm not going to bother testing which edge it's on.  If it's hidden, we assume lower edge.  So hidden on other edges probably won't work.
					if(uState==ABS_AUTOHIDE){
						BringWindowToTop(hWnd);
					}
				}
			}
			#endregion Taskbar
			if(Control.MouseButtons!=MouseButtons.Left){
				return;
			}
			#region Dragging
			_pointMouseScreenPrevious=Control.MousePosition;
			Point pointDelta =new Point(Control.MousePosition.X-_pointMouseDownScreen.X,Control.MousePosition.Y-_pointMouseDownScreen.Y);
			if(IsImageFloatDocked){
				if(Math.Abs(pointDelta.X)<3 && Math.Abs(pointDelta.Y)<3){//ignore small drags. This would be annoying if not docked.
					return;
				}
				if(IsImageFloatLocked){
					MsgBox.Show(this,"PDFs cannot be undocked.  Double click to open in PDF viewer.");
					return;
				}
				_isImageFloatDragging=true;
			}
			if(WindowState==FormWindowState.Maximized){
				if(_enumMouseDownRegion==EnumMouseDownRegion.Title){
					//ignore small drags in title because user was probably just clicking and we don't want to trigger a "restore down" from _isMaximized
					if(Math.Abs(pointDelta.X)<3 && Math.Abs(pointDelta.Y)<3){
						return;
					}
					//recalc so that new window will be centered on the mouse instead of jumping to 0,0
					_rectangleOnDesktopMouseDown.X=_rectangleOnDesktopMouseDown.X+MaxInset()+e.X-RestoreBounds.Width/2;
					_rectangleOnDesktopMouseDown.Y+=MaxInset();
				}
				else if(_enumMouseDownRegion==EnumMouseDownRegion.Buttons){
					//Don't restore down.  Ignore any drag.
					return;
				}
				else{
					//ignore restoreBounds.  An improvement will be to set RestoreBounds with WinProc.
					//But not many people will try to resize a maximized window, so there's no rush
					_rectangleOnDesktopMouseDown.X+=MaxInset();
					_rectangleOnDesktopMouseDown.Y+=MaxInset();
					_rectangleOnDesktopMouseDown.Width-=MaxInset()*2;
					_rectangleOnDesktopMouseDown.Height-=MaxInset()*2;
				}
				WindowState=FormWindowState.Normal;
			}
			Rectangle rectangleNew=Rectangle.Empty;
			if(_enumMouseDownRegion==EnumMouseDownRegion.UL){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X+pointDelta.X,_rectangleOnDesktopMouseDown.Y+pointDelta.Y,
					_rectangleOnDesktopMouseDown.Width-pointDelta.X,_rectangleOnDesktopMouseDown.Height-pointDelta.Y);
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.UR){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X,_rectangleOnDesktopMouseDown.Y+pointDelta.Y,
					_rectangleOnDesktopMouseDown.Width+pointDelta.X,_rectangleOnDesktopMouseDown.Height-pointDelta.Y);
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.LL){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X+pointDelta.X,_rectangleOnDesktopMouseDown.Y,
					_rectangleOnDesktopMouseDown.Width-pointDelta.X,_rectangleOnDesktopMouseDown.Height+pointDelta.Y);
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.LR){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X,_rectangleOnDesktopMouseDown.Y,
					_rectangleOnDesktopMouseDown.Width+pointDelta.X,_rectangleOnDesktopMouseDown.Height+pointDelta.Y);
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.Left){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X+pointDelta.X,_rectangleOnDesktopMouseDown.Y,
					_rectangleOnDesktopMouseDown.Width-pointDelta.X,_rectangleOnDesktopMouseDown.Height);
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.Right){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X,_rectangleOnDesktopMouseDown.Y,
					_rectangleOnDesktopMouseDown.Width+pointDelta.X,_rectangleOnDesktopMouseDown.Height);
				//PanelBorders.Invalidate(_rectangleButtons);
				//PanelBorders.Invalidate(new Rectangle(Width-_widthBorder-1,0,_widthBorder+2,Height));
				//We could get fancy like above, but it was found that a simple PanelBorders.Invalidate() works the same.
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.Top){
				//NativeMethods.SetWindowPos_Move(Handle,_desktopBoundsMouseDown.X,_desktopBoundsMouseDown.Y+pointDelta.Y,
				//	_desktopBoundsMouseDown.Width,_desktopBoundsMouseDown.Height-pointDelta.Y);
				//Height=_desktopBoundsMouseDown.Height-pointDelta.Y;
				//OnLayout();
				//PanelBorders.Invalidate();
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X,_rectangleOnDesktopMouseDown.Y+pointDelta.Y,
					_rectangleOnDesktopMouseDown.Width,_rectangleOnDesktopMouseDown.Height-pointDelta.Y);
				//Application.DoEvents();
				//The problem here is that the OK/Close buttons bounce around too much.
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.Bottom){
				rectangleNew=new Rectangle(_rectangleOnDesktopMouseDown.X,_rectangleOnDesktopMouseDown.Y,
					_rectangleOnDesktopMouseDown.Width,_rectangleOnDesktopMouseDown.Height+pointDelta.Y);
			}
			if(_enumMouseDownRegion==EnumMouseDownRegion.Title){
				if(WindowState==FormWindowState.Maximized){
					//ignore small drags in title because user was probably just clicking and we don't want to trigger a "restore down" from _isMaximized
					if(Math.Abs(pointDelta.X)<3 && Math.Abs(pointDelta.Y)<3){
						return;
					}
					//recalc so that new window will be centered on the mouse instead of jumping to 0,0
					_rectangleOnDesktopMouseDown.X=_rectangleOnDesktopMouseDown.X+e.X-RestoreBounds.Width/2;
					WindowState=FormWindowState.Normal;
				}
				//Can't change W or H here, or dragging to screen with different dpi will fail.
				SetDesktopLocation(_rectangleOnDesktopMouseDown.X+pointDelta.X,_rectangleOnDesktopMouseDown.Y+pointDelta.Y);
				//this does get hit sometimes on a simple click in the title bar, probably because window resize or similar.  We handle that later.
				_isDraggingTitle=true;
			}
			if(rectangleNew!=Rectangle.Empty){
				if(rectangleNew.Width<150){
					rectangleNew.Width=150;//without this, it will go down to 135, but not sure where that limit is coded.
				}
				if(rectangleNew.Height<_uIManager.GetHeightTitleBar()+widthBorder){
					rectangleNew.Height=_uIManager.GetHeightTitleBar()+widthBorder;
				}	
				SetDesktopBounds(rectangleNew.X,rectangleNew.Y,rectangleNew.Width,rectangleNew.Height);
			}
			//NativeMethods.SendMessage_SETREDRAW(Handle,true);
			//PanelBorders.Invalidate();//Doesn't work well enough. 
Refresh();
			//panelClient.Refresh();//Looked worse than form.Refresh, actually.
Application.DoEvents();//Without this, there are huge drag artifacts, especially dragging top downward
			//ResumeLayout();//doesn't help
			//OnResize(new EventArgs());
			#endregion Dragging
		}

		private void PanelBorders_MouseUp(object sender, MouseEventArgs e){
			int minute=DateTime.Now.Minute;//for setting breakpoints
			if(IsBorderLocked){
				return;
			}
			//variables for debugging-----------------------------------------------------------------------------
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromHandle(this.Handle);
			Point pointMouse=Control.MousePosition;//screen coordinates
			Point pointDesktop=Location;//desktop 
			Rectangle rectangleWorking=screen.WorkingArea;//in screen coords, smaller than screen,Loc in screen coords, size in false 96dpi
			Rectangle rectangleBounds=screen.Bounds;//Loc in screen coords, size in false 96dpi
			//----------------------------------------------------------------------------------------------------
			if(_isImageFloatDragging){
				IsImageFloatDocked=false;
				_isImageFloatDragging=false;
			}
			if(_isDraggingTitle){
				//Because 20.3 is not dpi aware, we do not have access to true screen size.
				_isDraggingTitle=false;
				Point pointDelta =new Point(Control.MousePosition.X-_pointMouseDownScreen.X,Control.MousePosition.Y-_pointMouseDownScreen.Y);
				if(Bounds.Top<screen.Bounds.Top+1 && MaximizeBox && (Math.Abs(pointDelta.X)>3 || Math.Abs(pointDelta.Y)>3)){
					//snap to top to maximize. Only allow if there's a Maximize button.
					//Windows will not reliably restore the size after maximize.  It gets bigger each time.  We need to trick it by resizing the window before maximizing.
					//This does not cause any flicker
					ShrinkWindowBeforeMinMax();
					WindowState=FormWindowState.Maximized;
				}
				else if(screen.Primary && pointMouse.X==screen.WorkingArea.Right-1){
					//snap to right
					Bounds=new Rectangle(screen.WorkingArea.Left+screen.WorkingArea.Width/2,screen.WorkingArea.Top,screen.WorkingArea.Width/2,screen.WorkingArea.Height);					
				}
				else if(screen.Primary && Control.MousePosition.X==screen.WorkingArea.Left){
					//snap to left
					Bounds=new Rectangle(screen.WorkingArea.Left,screen.WorkingArea.Top,screen.WorkingArea.Width/2,screen.WorkingArea.Height);
				}
				OnResizeEnd(new EventArgs());
				return;
			}
			if(_enumMouseDownRegion!=EnumMouseDownRegion.Buttons){
				//an ordinary border drag resize
				//Mouse up can easily be over a button at this point because of drag to different monitor dpi or minimum size limit.
				OnResizeEnd(new EventArgs());
				return;
			}
			if(_rectangleButX.Contains(e.Location)){
				OnCloseXClicked();
				return;
			}
			if(_rectangleButMax.Contains(e.Location) && MaximizeBox){
				if(WindowState==FormWindowState.Maximized){ //restore down
					WindowState=FormWindowState.Normal;
					if(Location==new Point(0,0)){
						Location=new Point(screen.WorkingArea.X+screen.WorkingArea.Width/2-Width/2,screen.WorkingArea.Y+screen.WorkingArea.Height/2-Height/2);
					}
					if(Location.Y<screen.Bounds.Y){
						Location=new Point(Location.X,screen.WorkingArea.Y);
					}
				}
				else{//maximize
					if(this.GetType().ToString()=="FormImageFloat" && IsImageFloatLocked){
						MsgBox.Show(this,"PDFs cannot be undocked.  Double click to open in PDF viewer.");
						return;
					}
					//Windows will not reliably restore the size after maximize.  It gets bigger each time.  We need to trick it by resizing the window before maximizing.
					//This does not cause any flicker
					ShrinkWindowBeforeMinMax();
					if(this.GetType().ToString()=="FormImageFloat"){
						IsImageFloatDocked=false;
						IsImageFloatLocked=false;
					}
					WindowState=FormWindowState.Maximized;
				}
				OnResizeEnd(new EventArgs());
				return;
			}
			if(_rectangleButMin.Contains(e.Location) && MinimizeBox){
				if(this.GetType().ToString()=="FormImageFloat" && IsImageFloatLocked){
					MsgBox.Show(this,"PDFs cannot be undocked.  Double click to open in PDF viewer.");
					return;
				}
				if(WindowState!=FormWindowState.Maximized){//not an issue if starting maximized
					//Windows will not reliably restore the size after minimize.  It gets bigger each time.  We need to trick it by resizing the window before minimizing.
					//This does not cause any flicker
					ShrinkWindowBeforeMinMax();
				}
				WindowState=FormWindowState.Minimized;
				if(this.GetType().ToString()=="FormImageFloat"){
					IsImageFloatDocked=false;
					IsImageFloatLocked=false;
				}
				OnResizeEnd(new EventArgs());
				return;
			}
			if(_regionButHelp.IsVisible(e.Location) && HasHelpButton){//IsVisible really means HitTest
				string formName="";
				if(GetHelpOverride()=="") {
					formName=Name;
				}
				else{
					formName=GetHelpOverride();
				}
				try{
					bool isKeyValid=ODHelp.IsEncryptedKeyValid();//always true in debug
					string manualPageURL=OpenDentalHelp.ODHelp.GetManualPage(formName,PrefC.GetString(PrefName.ProgramVersion),isKeyValid);
					if(ODBuild.IsWeb()) {
						Process.Start(manualPageURL);
					}
					else{
						Process.Start(manualPageURL);
						//todo: fix for wpf:
						//FormHelpBrowser formHelpBrowser=FormHelpBrowser.GetFormHelpBrowser(enableUI:isKeyValid);//If false, then just the Help Feature page shows
						//formHelpBrowser.GoToPage(manualPageURL);
						//formHelpBrowser.Show();
						//UIHelper.ForceBringToFront(formHelpBrowser);
					}
					if(!isKeyValid) {
						//comes up on top of locked browser.
						MsgBox.Show("To use the Open Dental Help feature you must be on support.");
					}
				}
				catch{
				}
				return;
			}
		}
		#endregion Border Mouse

		#region UIManagerTransition
		private int GetHeightTitleBar(){
			return _uIManager.GetHeightTitleBar();
		}

		private int ScaleI(float val96){//Scale is already taken
			return _uIManager.Scale(val96);
		}

		private float ScaleF(float val96){
			return _uIManager.ScaleF(val96);
		}

		private float ScaleFontODZoom(float val96){
			return _uIManager.ScaleFontODZoom(val96);
		}

		private float ScaleMS(float val96){
			return _uIManager.ScaleMS(val96);
		}

		private int WidthBorder(){
			return _uIManager.Scale(15);//this was obtained by trial and error
			//Its only effect is the spacing to the right of the X close button.
		}
		#endregion UIManagerTransition

		#region WndProc
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT{
			public int Left, Top, Right, Bottom;

			public RECT(Rectangle rectangle){
				Left = rectangle.Left;
				Top = rectangle.Top;
				Right = rectangle.Right;
				Bottom = rectangle.Bottom;
			}

			public Rectangle ToRectangle(){
				return Rectangle.FromLTRB(Left, Top, Right, Bottom);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WINDOWPOS{
			public IntPtr hWnd, hWndInsertAfter;
			public int x, y, cx, cy, flags;
		}

		private const int WM_DPICHANGED=0x02E0;
		private const int WM_NCCALCSIZE = 0x83;

		[StructLayout(LayoutKind.Sequential)]
		private struct NCCALCSIZE_PARAMS{
			//All rect coords relative to screen origin.
			public RECT rgrc0;//Starts as new coords of window after move or resize.  Returns new client rectangle.
			public RECT rgrc1;//Starts as old coords of window before resize.  Returns destination rectangle. We don't use.
			public RECT rgrc2;//Starts as client area before move or resize.  Returns source rectangle. We don't use.
			public WINDOWPOS lppos;
		}

		protected override void WndProc(ref Message m) {
			#region WM_DPICHANGED
			if (m.Msg==WM_DPICHANGED){
				if(_uIManager==null){
					base.WndProc(ref m);
					return;
				}
				if(AreBordersMS){
					base.WndProc(ref m);
					return;
				}
				//This is fired by Windows when moving forms between screens and when user changes resolution, etc.
				//We override the auto dpi scaling, which is just awful.  We'll handle it.
				//Use the suggested form size.  MS accurately generates the new bounds when moving between monitors, 
				//except that it doesn't take into account our computerpref.Zoom.
				//?Marshal.AllocHGlobal(Marshal.SizeOf(rectSuggested));
				RECT rectSuggested=(RECT)Marshal.PtrToStructure(m.LParam,typeof(RECT));
				//?Marshal.FreeHGlobal(pRect);
				//DeviceDpi is also unreliable.  Main form reports 96, while some controls on the same form are aware that they are at higher dpi.
				//We can't override that in MS controls, so we don't have total control.
				//RescaleConstantsForDpi seems like it was a failed MS attempt at solving some of this.
				//So we use our own variable: ScaleMy
				int dpiHiLo=m.WParam.ToInt32();
					//(uint)Marshal.PtrToStructure(m.WParam,typeof(uint));
				int dpi=dpiHiLo & 0xffff;//just grab the 2 low bytes
				//The above is not redundant because this form is unaware of DeviceDpi
				//valScreen/ScaleMy();
				SizeF size96Old;
				size96Old=new SizeF(_uIManager.UnscaleF(Width),_uIManager.UnscaleF(Height));
				_uIManager.SetScaleMS(dpi/96f);//example 1.5
				if(WindowState==FormWindowState.Maximized){
					//don't try to resize. This gets hit when user changes resolution or scale of a monitor
					_uIManager.LayoutFormBoundsAndFonts();
				}
				else{
					Rectangle rectangle=new Rectangle(rectSuggested.Left,rectSuggested.Top,rectSuggested.Right-rectSuggested.Left,rectSuggested.Bottom-rectSuggested.Top);
					if(ComputerPrefs.IsLocalComputerNull()){
						//This WndProc can be hit very early, before a db connection, if starting on a high dpi monitor.  
						//We can't get computerpref now, because then it won't be retrieved later, when we do have a db connection.
					}
					else{
						if(ComputerPrefs.LocalComputer.Zoom!=0){
							//make slight adjustments to the suggested rectangle to include Zoom
							int widthNew=_uIManager.Scale(size96Old.Width);
							int heightNew=_uIManager.Scale(size96Old.Height);
							//todo: We shouldn't be changing the rectangle, but it's just a small amount.
							//https://learn.microsoft.com/en-us/windows/win32/hidpi/high-dpi-desktop-application-development-on-windows
							rectangle.X-=(widthNew-rectangle.Width)/2;
							rectangle.Y-=(heightNew-rectangle.Height)/2;
							rectangle.Width=widthNew;
							rectangle.Height=heightNew;
						}
					}
					//This line triggers a series of resize events
					Bounds=rectangle;
				}
				PanelBorders?.Invalidate();
				return;
			}
			#endregion WM_DPICHANGED
			#region WM_NCCALCSIZE
			if (m.Msg==WM_NCCALCSIZE){//client area calculation.
				if(AreBordersMS){
					base.WndProc(ref m);
					return;
				}
				if(m.WParam.Equals(IntPtr.Zero)) {//wParam false
					base.WndProc(ref m);
					return;
				}
				//wParam true
						//base.WndProc(ref m);
				NCCALCSIZE_PARAMS ncCalcSizeParams = (NCCALCSIZE_PARAMS)m.GetLParam(typeof(NCCALCSIZE_PARAMS));
				Rectangle rectangle = ncCalcSizeParams.rgrc0.ToRectangle();//Starts as new coords of window after move or resize.
				//I verified that rectangle==this.Bounds, as it should.
				//Lots of trial and error went into this, but it does match MS now
				//Just like MS, the top resize is within the titlebar, not in the outside shadow.
				//We are leaving an 8 pixel border on L,R,B to show the shadow and allow dragging.
				//See notes in Load
				int widthBorder=(int)Math.Round(8+(5*(_uIManager.GetScaleMS()-1)),MidpointRounding.AwayFromZero);
				rectangle=new Rectangle(rectangle.X+widthBorder,rectangle.Y,rectangle.Width-widthBorder*2,rectangle.Height-widthBorder);
				ncCalcSizeParams.rgrc0 = new RECT(rectangle);
				Marshal.StructureToPtr(ncCalcSizeParams,m.LParam,true);//return screen coords of new window client area
				//else{//wParam true, so we're supposed to indicate valid client area
					/*
					if(WindowState!=FormWindowState.Maximized){
						m.Result = IntPtr.Zero;//this indicates that client should be same size as window if we don't process NCCALCSIZE_PARAMS
						return;
					}
					NCCALCSIZE_PARAMS ncCalcSizeParams=(NCCALCSIZE_PARAMS)m.GetLParam(typeof(NCCALCSIZE_PARAMS));
					Rectangle rectangle=ncCalcSizeParams.rgrc0.ToRectangle();//Starts as new coords of window after move or resize.
					//I verified that rectangle==this.Bounds, as it should.
					//But it's entirely unclear why we are enlarging rectangle (Bounds) instead of our client rectangle, as documentation says we should.
					//Chosing the new size requires the following understanding:
					//If you SetWindowTheme(Handle,string.Empty,string.Empty); in OnHandleCreated, you will see that a non-themed window has a thick 8 px border.
					//This border is actually still present in a modern theme as an invisible area outside the window. A separate shadow hints at its presence.
					//Enlarging the rectangle to the left, for example, does not move the apparent client rect at all but does shrink the sizing handle outside the window.
					//So what's really happening is that the client rectangle is indeed enlarging, and the 1px border is following it.
					//If the rectangle is enlarged until it apparently fills the window, the exterior sizing handle is reduced to nothing.
					//At that point, the shadow also disappears, so we have to manually add it back.
					//Getting this rectangle to the exact pixel to match MS is tough.  Rounding didn't work. Trunc didn't work.
					//rectangle=new Rectangle(rectangle.X-(int)LayoutManager.ScaleF(8),rectangle.Y-(int)LayoutManager.ScaleF(31),rectangle.Width+(int)LayoutManager.ScaleF(16),rectangle.Height+(int)LayoutManager.ScaleF(39));
					//rectangle=new Rectangle(rectangle.X-8,rectangle.Y-31,rectangle.Width+16,rectangle.Height+39);
					rectangle=new Rectangle(rectangle.X+8,rectangle.Y+31,rectangle.Width-16,rectangle.Height-39);
					//I'm assuming for now that all computers are the same here.  This assumption is because non-themed window works same as modern theme.
					ncCalcSizeParams.rgrc0 = new RECT(rectangle);
					Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);//return screen coords of new window client area*/
					//m.Result = IntPtr.Zero;//this indicates that client should be same size as window if we don't process NCCALCSIZE_PARAMS
					//base.WndProc(ref m);
				//}
				return;
				//To learn more about WM_NCCALCSIZE:
				//https://stackoverflow.com/questions/2135068/how-to-set-the-size-of-the-non-client-area-of-a-win32-window-native
			}
			#endregion WM_NCCALCSIZE
			base.WndProc(ref m);
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct APPBARDATA{
			public int cbSize; // initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
			public IntPtr hWnd;
			public uint uCallbackMessage;
			public uint uEdge;
			public RECT rect;
			public int lParam;
		}

		[DllImport("shell32.dll")]
		private static extern IntPtr SHAppBarMessage(uint dwMessage,[In] ref APPBARDATA pData);

		private uint ABM_GETSTATE=4;
		private uint ABS_AUTOHIDE=1;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError=true)]
		static extern bool BringWindowToTop(IntPtr hWnd);
		#endregion WndProc

		#region Signal Processing
		public void ProcessSignals(List<Signalod> listSignals) {
			Logger.LogAction("ODForm.ProcessSignals",LogPath.Signals,() => ProcessSignalODs(listSignals),this.GetType().Name);
		}

		///<summary>Override this if your form cares about signal processing.</summary>
		public virtual void ProcessSignalODs(List<Signalod> listSignals) {
		}
		#endregion Signal Processing

		#region Help
		///<summary>Used to send a different form name to ODHelp.</summary>
		protected virtual string GetHelpOverride(){
			return "";
		}
		#endregion Help

		private void InitializeComponent() {
			this.SuspendLayout();
			// 
			// FormODBase
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Name = "FormODBase";
			this.ResumeLayout(false);

		}

		public class PanelDoubleBuffered:Panel{
			public PanelDoubleBuffered():base() {
				DoubleBuffered=true;
			}
		}
	}

}


#region Ignore these notes
//There are many ways to draw a window border.  Here are a few possibilities:
//1.(How we do it) Put all controls into a PanelClient that is slightly smaller than the form.  Eliminate MS borders.  
//  Add PanelBorder, filling the form, behind PanelClient.  This is where drawing and mouse happens.
//  Downsides: Any control.Parent would fail if it's assuming a form, but there aren't many. Switch to control.TopLevelControl, which is guaranteed to be the form.
//  Manual layout uses ClientSize, so ClientSize.get was tweaked to hide the base version and instead use the PanelClient.ClientSize.
//  Flicker when resizing seems to be an unsolvable problem.  People don't resize very often, and many of my favorite UIs flicker when resizing.  So we will live with it.
//  I'm pretty sure there are some Win32 tweaks we can add to remove the kinds of flicker caused by multiple resizes.
//2.Same as above, except draw directly on main form.  This was not possible due to a bug in MS, as described in PanelBorder above.
//3.(Worked but decided against it) Added 4 docked panels to sides to push in any docked controls.  Move all non-docked controls down and right on load. 
//  Downsides: Any manual layoutcould no longer use 0,0 as origin and would be different than it was in designer.
//  Flicker was same as #1.
//  Finally, controls could spill over onto border.
//4.(Worked but decided against it) Overrode WndProc to assign a sizable border.  
//  https://www.codeproject.com/Tips/1081558/Custom-Border-Windows-Style
//  Had same flicker as 1 and 2.  Didn't give me enough control over exact boundaries and behaviors.
//5. Use WndProc to gain access to non-client area for drawing
//  http://geekswithblogs.net/kobush/articles/CustomBorderForms.aspx
//  No code example available.  He says his solution is buggy.
//6. Windows Style Builder? Seems like old tech.
//7. Wrap the WinForm in a WPF form. This would probably work, but probably wouldn't solve the flicker.

//2020-05-05- Issues for later:
//ODProgress and Help both have UI in a separate thread from the main OD thread.
//This is bad.  Both should be deprecated and rewritten.
//ODProgress has tentacles, including ODMessageBox and various code inside this class.
//As long as ODProgress exists, none of the related code can be damaged.
//Another issue to be aware of is how OD forces closing.
//It must force various open windows to close, 
//so there is a fair amount of code scattered around that tries to close windows and then kills them.
//This code will generally stay in place since it's good.
//Signal processing code doesn't really need to be in this class, so we might move it eventually.
//This would allow more forms to process signals, even if they didn't inherit from FormODBase.
//For example, our internal tools.



#endregion Ignore these notes