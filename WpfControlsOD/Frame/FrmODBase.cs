using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
/*
Jordan is the only one allowed to edit this file.

Required heights should always be exactly:
Button=24
ComboBox=21, ComboClinic=21, DatePicker=21, 
Menu=24, 
TextBox(single line)=20, TextVDate=20, TextVDouble=20, TextVInt=20,
ToolBar=25
Suggested heights for some others:
Label=18
RadioButton, CheckBox=20

How to launch a dialog:
FrmAccountEdit frmAccountEdit=new FrmAccountEdit();//don't worry about the "using" statement required in WinForms.
frmAccountEdit.AccountCur=new Account();
frmAccountEdit.ShowDialog();
if(frmAccountEdit.IsDialogOK){
	etc.
}

Or for a non-modal window:
FrmAccounts frmAccounts=new FrmAccounts();
frmAccountEdit.Show();
...many options later, like... 
Account accountSelected=frmAccounts.AccountSelected;
frmAccounts.Close();

VS Environment:
If VS starts inserting leading spaces anywhere, go to Options, Text Editor, Advanced, uncheck "Use adaptive formatting".
The XAML debugging toolbar is useless to us, so go to VS, Debugging, XAML Hot Reload, uncheck "Enable in-app toolbar".
Ctrl-V frequently refuses to paste. To fix, go to Options, Text Editor, General, uncheck "Enable mouse click to perform Go to Definition".

Rules for using WPF:
-No WPF Windows.  Always use a FrmODBase.
-Maintain the anchor strategy that we have always used in WinForms.  Relative positioning is not allowed.
		-Jordan creates all of our base controls. Stock controls are not allowed without Jordan's deep involvement.
		-All of our controls default to Top Left instead the WPF default of stretch. This is important.
		-Drag controls around the window, and the Top and Left margins will automatically change. These are relative to the parent grid, not to siblings.
		-To anchor to the BR, click the anchor icons in the UI (They look like chain links at the edges of the container). Click U and L to unanchor. This automatically flips the anchors to R and B.
		-To anchor to both L and R, click the R anchor icon.  This changes hAlign to stretch and width to auto.  This cannot be done by using the properties panel at the right.  It must be done by clicking on the anchors in the designer.  If you accidentally tried to change it in properties panel, it will malfunction. To fix, edit the XAML to remove the width.  You cannot specify both stretch and width.
		-Controls frequently get stuck to Center alignment in the designer. When that happens, the only solution is to click on the top or left chain link icon to convert to top or left anchor.
		-Everything is a little touchier than WinForms. It takes some practice.
-No binding.  We manually set and get each value in the UI just like always.
-Jordan reviews all UI changes.

How to:
-The various frms are only UserControls, not actual Windows or Forms, but we supply methods and events that make them work identically.
-Go to WpfControlsOD/UI/Controls. Look at the xaml.cs code for each control for instructions at the top on how to implement that control.
-Issues regarding the conversion from WinForms to WPF are discussed over in WpfConverter.
-Fonts throughout are now Segoe UI 11.5, which is about 8.6 points. In spite of similar size, Segoe UI does take up a bit more horizontal space.
-For pixel drawing, we are using the same coordinate system for drawing as in WinForms. They didn't change it until DirectX10.
//https://learn.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-coordinates
-Instead of Windows.Forms.Timer, use System.Windows.Threading.DispatcherTimer.
		Also consider using our built-in FilterControls
-See a few hundred lines down for how to handle key events
-Tab order is set by TabIndexOD, similar to before. But default is a huge int which makes that control last.
		Here's how you set both keyboard and logical focus, usually in the Load event handler: textBox1.Focus();
		The similar Winforms method called Select() does not distinguish between keyboard and logical focus.
		There is no UI tool to set tab order.
		Instead, group the controls in XAML and edit the TabIndexODs in XAML.
		I've set it up so that tabbing is nested, like in WF. But it still might not be able to jump into group controls.
		Checkboxes do not have support for tab index. It's restricted to textboxes unless I see a compelling use case someday.
-Events:
		Button click event should be attached to each button in XAML/properties docker.
		All events other than button clicks should be added to the constructor, not XAML/properties docker. 
		This is so that the references can be found. One shortcoming of WPF is that VS shows zero refs if you enter the events into XAML.
-Control Defaults:
		Names: controls do not need to have names like in WF. You will need to add a name in order to refer to it from code. Default is null.
		Width and height: I tried to set default widths and heights for all controls, but WPF didn't like that at all. The WPF paradigm is stretch to fit, so general defaults not possible.
		Heights of controls are listed at the top of this page and in each control.
		Because width and height have no defaults, adding a new control to a frm is sometimes a pain.
		But this was also true in WinForms. Not too much difference.
		When adding controls, you can't click to drop and you can't drag from the toolbox.
		You should always click and drag to create the rectangle where your control belongs
		Or you can copy/paste an existing control from somewhere.
-Getting mouse pos:
		Point pointRelative=mouseButtonEventArgs(e).GetPosition(control);
		Point pointRelative=System.Windows.Input.Mouse.GetPosition(control);
		The two points below are in full desktop coords, not just screen:
		System.Drawing.Point drawing_pointScreen = System.Windows.Forms.Cursor.Position;//then just use x and y separately.
		Point pointScreen = control.PointToScreen(pointRelative);
-Getting desktop bounds:
		You can convert any point to full desktop coords as explained above
		But WPF has no support for getting desktop bounds, so use WinForms.
		You usually (always?) only want bounds for one screen, but in desktop coords.
		Pass in a point to get the corresponding screen.
		System.Drawing.Point drawing_PointScreen=new System.Drawing.Point((int)pointScreen.X,(int)pointScreen.Y);
		System.Drawing.Rectangle drawing_RectangleBounds=System.Windows.Forms.Screen.GetWorkingArea(drawing_PointScreen);
		Use the resulting xywh values separately or load them into a WPF Rect if needed.
		You can't use the overload for GetWorkingArea that takes a Control because this is WPF.
-Changing Window size/position:
		You must somehow specify desktop coords, not DIPs (96dpi virtual).
		If you use DIPs, then it will look completely wrong on high dpi monitors. 
		At 96 dpi, borders are (LTRB) 8,26,8,8, but those numbers scale with dpi, and could be very wrong at 200%.
		So the client area size also needs to scale with dpi.
		The fundamental way of dealing with size/pos is with actual desktop coords. 
		Any use of DIP contants must be converted to desktop scale. We provide ScaleFormValue() for this purpose.
		Do NOT use frm.Width or frm.Height directly. 
		Instead, always refer to _formFrame which is the wrapper for the Frm usercontrol.
		Do NOT try to set the size/pos of a Frm externally before ShowDialog().
		Unlike WinForms, the Form doesn't even exist yet at this point, so you can't directly change it.
		But we also don't actually support changing size/pos from outside in WinForms either because of scaling issues (see note at the end of FormODBase.OnLoad()).
		To set the size/pos of a new Frm:
			1. Pick a reference point to pass into the Frm. Frequently UL corner of where you want the new Frm to end up.
				But if the form should be "anchored" to LL for example, then you would pass in the LL corner of the Frm.
			2. You will use a public field on the Frm. Always in full desktop coords. Either a System.Drawing.Point or a System.Windows.Point.
			3. Do the math in the parent to calculate your point.
			4. Pass it in to the Frm.
			5. Use it in the Load event handler.
			6. Sometimes, you will also do some math in the Load to further alter your variables used.
			Example: _formFrame.Location=new System.Drawing.Point(PointLLStart.X,PointLLStart.Y-_formFrame.Height+ScaleFormValue(10));
			Example: _formFrame.Width=ScaleFormValue(556);
		Also, see Dpi.cs for notes about how to set a Window or Form position and using Bounds and DesktopBounds.
-Scale and Zoom
		Windows scale is handled automatically in WPF, so you won't normally need it, but if you do: 
				double scale=VisualTreeHelper.GetDpi(this).DpiScaleX;
		Open Dental zoom is largely handled automatically as well, but if you need it:
				(FrmODBase).GetZoom(). Example within a Frm: this.GetZoom().
-Maximum Window Size
		FrmZoom specifies max window size as 1246x735, but that's for the entire form, including borders.
		In WPF, we design without borders, so the max size is 1212x719
		I've made a note about this in FrmZoom, which is the quickest place to look up these numbers.
-Language translation:
		It's very similar to WinForms, but the class name has changed slightly.
		Instead of Lan.g, use Lang.g. 
		At the top of every Frm, use Lang.F to translate all the controls at once. 
		Unlike WinForms, this must go in Load (usually at the top) instead of Ctor.
-Load event can be added in constructor like this:
		Load+=FrmWhatever_Load;
		The eventhandlers would start out like this:
		private void FrmWhatever_Load(object sender,EventArgs e) {
		Don't use WPF Loaded because that's too early.
		That's just the load event for the usercontrol and it's not useful. It's too early.
		Our Load behaves just like the WinForms one you're used to.
		This is where you would change position or size of window to avoid flicker. See section above regarding size/position.
-Shown event can be added in constructor like this:
		Shown+=FrmWhatever_Shown;
		The eventhandler would start out like this:
		private void FrmWhatever_Shown(object sender,EventArgs e) {
		Shown means the Form is actually on the screen and showing. 
		This is useful to show a dialog when you want to ensure the form is showing behind the dialog.
-Form Properties
		_formFrame gives you a reference to the WinForm that's the frame for the Frm usercontrol.
		frmODBase.MinMaxBoxes: set to false to not show either min or max. This is very common for any window that is launched using ShowDialog.
		_formFrame.FormWindowState(get and set), _formFrame.Select, _formFrame.Restore, etc are all available and work as normal.
		HasHelpButton can be set false, but that's rare.
-BitmapImage
		This is the new object for storing bitmaps. You can specify BitmapImage.StreamSource to load any bitmap.
		See OpenDental.Drawing.Graphics.ConvertBitmapFromWpf() and ConvertBitmapToWpf().
		It does not implement the IDisposable interface, so no worry about memory leaks. The GC handles these completely automatically.
		But the GC does wait until the BitmapImage goes out of scope. Set it to null if you want to clean up a long-lived BitmapImage.
-DialogResult
		We use IsDialogOK instead of the old DialogResult enum.
		IsDialogCancel can be used instead if the code would read better. There's no functional difference.
		Setting IsDialogOK (or cancel) automatically calls Close(), just like always.
		In old WinForms, the X at the upper right resulted in DialogResult.Cancel if the form was launched with ShowDialog.
		But it resulted in DialogResult.None if the form was launched with Show.
		In this FrmODBase, if launched with Show, then IsDialogOK is always ignored by the caller, but it will be false.
-Mouse Cursors
		Some slight changes. Use Cursors.Wait instead of Cursors.WaitCursor.
		Use Cursors.Arrow instead of Cursors.Default.
-Mouse Down
		We must frequently track mouse down in two independent ways, and combine them to avoid certain bugs.
		1. _isMouseDown gets set in the mouse down event. This proves that mouse down was performed in our control, not somewhere else.
		2. bool isMouseDown=Mouse.LeftButton==MouseButtonState.Pressed. This proves that mouse is still actually down.
		Without #1, a bug example is a combobox above our control has a dropdown that user clicks on. This causes the dropdown to close, triggering a mouse move in our control while mouse is still down. Using _isMouseDown lets us ignore this because we didn't actually mouse down in our control.
		Without #2, a bug example is that a mouse down triggers a dialog. The mouse up then happens inside that dialog, leaving our boolean stuck in the wrong state. 
		The reason for using a bool variable with #2 is that the state is not preserved during debugging. Mouse events in VS will actually change it for example.
-Mouse Capture
		In WPF, if you drag outside your control, it will stop registered mouse move. This is different than WinForms.
		When you use the suggested solution of CaptureMouse, the capture itself triggers an unwanted mouse move.
		The solution is to do this in mouse down:
				_ignoreMouseMove=true;
				((IInputElement)sender).CaptureMouse();
				_ignoreMouseMove=false;
		and this in mouse move:
				if(_ignoreMouseMove){
					return;
				}
-Signal Processing
		It's common to have a window that needs to be notified of any new incoming signalod from the db.
		In FormODBase, the way to do it was to override ProcessSignalODs
		In WPF, do it like this:
		(in your frm load:)
		GlobalFormOpenDental.EventProcessSignalODs+=GlobalFormOpenDental_EventProcessSignalODs;
		then,
		private void GlobalFormOpenDental_EventProcessSignalODs(object sender,List<Signalod> listSignalods) {
			for(int i=0;i<listSignalods.Count;i++) {
				if(listSignalods[i].IType!=InvalidType.Children) {
					continue;
				}
				FillAllGrids();
				return;
			}
		}

*/
	/// <summary>All WPF windows inherit from this base class.</summary>
	public partial class FrmODBase:System.Windows.Controls.UserControl {
		#region Fields
		private bool _hasHelpButton=true;
		private bool _isDialogOK=false;
		private bool _minMaxBoxes=true;
		private bool _startMaximized=false;
		private string _text="Form";
		///<summary>This is a reference to the Form that frames the Frm.</summary>
		protected FormFrame _formFrame=null;
		private float _zoomLocal=1;
		#endregion Fields

		#region Fields size/pos
		private int _heightPending96=0;
		private int _heightPendingAbs=0;
		private int _widthPending96=0;
		private int _widthPendingAbs=0;
		#endregion Fields size/pos

		#region Constructor
		public FrmODBase() {
			//If no fontSize is set, then Control defines it as 12 points.
			FontFamily=new FontFamily("Segoe UI");
			FontSize=11.5;//This size is inherited by all controls on a FrmODBase.
			//Focusable=true;//so key down will work
			//Focusable=false;//so that focus will fall to the child controls instead of directly on this control
			PreviewKeyDown+=Frm_PreviewKeyDown;
			//KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);//See notes in FrmTestFocusTabbing.xaml.cs
			//I don't think the above line makes sense here. We use it instead in specific controls like GroupBox.
			Unloaded+=FrmODBase_Unloaded;
		}

		private void FrmODBase_Unloaded(object sender,RoutedEventArgs e) {
			_formFrame=null;
		}
		#endregion Constructor

		#region Events
		public event EventHandler Activated;
		public event EventHandler FormClosed;
		public event CancelEventHandler FormClosing;
		///<summary>This behaves just like the Form.Load event that we're all used to. Don't use WPF Loaded because that's too early. That's just for the usercontrol.</summary>
		public event EventHandler Load;
		public event EventHandler Shown;
		#endregion Events

		#region Properties
		///<summary>Default true.</summary>
		[Category("OD")]
		[Description("Default true.")]
		[DefaultValue(true)]
		public bool EscClosesWindow{get;set; }=true;

		[Description("Default true. Set to false to hide the help button.  Form 'HelpButton' property is ignored.")]
		[Category("OD")]
		[DefaultValue(true)]
		///<summary>Default true. Set to false to hide the help button.  Form 'HelpButton' property is ignored.</summary>
		public bool HasHelpButton{
			get{
				return _hasHelpButton;
			}
			set{
				_hasHelpButton=value;
				if(_formFrame!=null){//usually null, so see UIManager Ctor
					_formFrame.HasHelpButton=_hasHelpButton;
				}
			}
		}

		[Browsable(false)]
		///<summary>A wrapper for IsDialogOK to make code easier to read. Setter is not usually used but does work.</summary>
		public bool IsDialogCancel{
			get{
				return !_isDialogOK;
			}
			set{
				_isDialogOK=!value;
				Close();//regardless of the value they set
			}
		}

		[Browsable(false)]
		///<summary>True means OK or Yes.  False means Cancel or No.  Default is false.</summary>
		public bool IsDialogOK{
			get{
				return _isDialogOK;
			}
			set{
				_isDialogOK=value;
				Close();//regardless of the value they set
			}
		}

		[Description("Set false to hide Min and Max boxes at upper right. This should only be used for windows that are purely modal dialog boxes. They will still show in the TaskBar.")]
		[Category("OD")]
		[DefaultValue(true)]
		///<summary>Set false to hide Min and Max boxes at upper right. This should only be used for windows that are purely modal dialog boxes. They will still show in the TaskBar.</summary>
		public bool MinMaxBoxes{
			get{
				return _minMaxBoxes;
			}
			set{
				_minMaxBoxes=value;
				if(_formFrame!=null){//usually null, so see UIManager Ctor
					_formFrame.MinimizeBox=_minMaxBoxes;
					_formFrame.MaximizeBox=_minMaxBoxes;
				}
			}
		}

		[Description("Shows in titlebar of window.")]
		[Category("OD")]
		[DefaultValue("Form")]
		///<summary>Shows in titlebar of window.</summary>
		public string Text{
			get{
				return _text;
			}
			set{
				_text=value;
				if(_formFrame!=null){//usually null, so see UIManager Ctor
					_formFrame.Text=_text;
				}
			}
		}

		[Description("Set to true to show window initially maximized. Set in the Load() instead of designer. Otherwise, the Width and Height that you specified will be ignored when users restores down.")]
		[Category("OD")]
		[DefaultValue(false)]
		///<summary>Set to true to show window initially maximized. Set in the Load() instead of designer. Otherwise, the Width and Height that you specified will be ignored when users restores down.</summary>
		public bool StartMaximized{
			get{
				return _startMaximized;
			}
			set{
				_startMaximized=value;
				if(_formFrame!=null){//usually null, so see UIManager Ctor
					if(_startMaximized){
						_formFrame.WindowState=System.Windows.Forms.FormWindowState.Maximized;
					}
					else{
						_formFrame.WindowState=System.Windows.Forms.FormWindowState.Normal;
					}
				}
			}
		}
		#endregion Properties

		#region Methods - public
		///<summary>Calling Close() from within any Frm or externally will cause the parent FormFrame to close.</summary>
		public void Close() {
			if(_formFrame==null){
				return;//already closed
			}
			_formFrame.Close();
		}

		///<summary>Sets the entire form into "read only" mode by disabling all controls on the form. Pass in any controls that should stay enabled. This can be used to stop users from clicking items they do not have permission for.</summary>
		public void DisableAllExcept(params FrameworkElement[] frameworkElementArrayEnabled) {
			System.Windows.Controls.Grid grid=DrillDownToMainGrid();
			if(grid is null){
				return;
			}
			List<FrameworkElement> listFrameworkElements=grid.Children.Cast<FrameworkElement>().ToList();
			for(int i=0;i<listFrameworkElements.Count;i++){
				if(frameworkElementArrayEnabled.Contains(listFrameworkElements[i])) {
					continue;
				}
				listFrameworkElements[i].IsEnabled=false;
			}
		}

		public void Show(){
			if(_formFrame!=null){
				return;//already showing
			}
			_formFrame=new FormFrame();
			_formFrame.Activated+=FormFrame_Activated;
			_formFrame.Load+=FormFrame_Load;
			_formFrame.Shown+=(sender,e)=>Shown?.Invoke(sender,e);
			_formFrame.FormClosed+=FormFrame_FormClosed;
			_formFrame.FormClosing+=FormFrame_FormClosing;
			_formFrame.CloseXClicked+=_formFrame_CloseXClicked;
			_formFrame.InitializeFormMaker(this);
			_formFrame.Show();
		}

		public bool ShowDialog(){
			if(_formFrame!=null){
				return false;//already showing
			}
			_formFrame=new FormFrame();
			_formFrame.Activated+=FormFrame_Activated;
			_formFrame.Load+=FormFrame_Load;
			_formFrame.Shown+=(sender,e)=>Shown?.Invoke(sender,e);
			_formFrame.FormClosed+=FormFrame_FormClosed;
			_formFrame.FormClosing+=FormFrame_FormClosing;
			_formFrame.CloseXClicked+=_formFrame_CloseXClicked;
			_formFrame.InitializeFormMaker(this);
			_formFrame.ShowDialog();
			//formFrame won't have a meaningful dialogResult.
			//The programmer has already set IsDialogOK from inside the derived frm.
			return _isDialogOK;
		}
		#endregion Methods - public

		#region Methods - private
		///<summary>Does the same thing that DoEvents does in WinForms: refreshes UI even if you're in the middle of a method. It's public static so it can be called from a few places other than FrmODBase.</summary>
		public static void DoEvents() {
			//This was adapted directly from the MS C# manual: Dispatcher.PushFrame,
			//where they specifically suggest using it for DoEvents.
			Func<object,object> func=objF=>{
				((DispatcherFrame)objF).Continue = false;
				return null;
			};
			DispatcherOperationCallback dispatcherOperationCallback=new DispatcherOperationCallback(func);
			DispatcherFrame dispatcherFrame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,dispatcherOperationCallback,dispatcherFrame);
			Dispatcher.PushFrame(dispatcherFrame);
		}

		///<summary>Returns null if something goes wrong.</summary>
		private System.Windows.Controls.Grid DrillDownToMainGrid(){
			//This was optimized for our very specific Frms which are all structured the same.
			int count=VisualTreeHelper.GetChildrenCount(this);
			if(count!=1){
				return null;
			}
			DependencyObject dependencyObject=VisualTreeHelper.GetChild(this,0);
			if(!(dependencyObject is System.Windows.Controls.Border border)){
				return null;
			}
			count=VisualTreeHelper.GetChildrenCount(border);
			if(count!=1){
				return null;
			}
			dependencyObject=VisualTreeHelper.GetChild(border,0);
			if(!(dependencyObject is System.Windows.Controls.ContentPresenter contentPresenter)){
				return null;
			}
			count=VisualTreeHelper.GetChildrenCount(contentPresenter);
			if(count!=1){
				return null;
			}
			dependencyObject=VisualTreeHelper.GetChild(contentPresenter,0);
			if(!(dependencyObject is System.Windows.Controls.Grid grid)){
				return null;
			}
			return grid;
		}

		///<summary>Recursive. This gets a flat list of all nested child controls. "control" means one of our custom controls in the UI namespace. Skip the parameter to get all for this frm.</summary>
		public List<FrameworkElement> GetAllChildControlsFlat(FrameworkElement frameworkElementParent=null){
			//The heirarchy is very complex.
			//It might need to be tweaked in the future.
			//For example, the only non-custom control that the logic considers is a WPF Grid
			if(frameworkElementParent==null || frameworkElementParent==this){
				frameworkElementParent=DrillDownToMainGrid();
			}
			List<FrameworkElement> listFrameworkElements=new List<FrameworkElement>();
			List<FrameworkElement> listFrameworkElementsDirect=GetDirectChildControls(frameworkElementParent);
			for(int i=0;i<listFrameworkElementsDirect.Count;i++){
				listFrameworkElements.Add(listFrameworkElementsDirect[i]);
				List<FrameworkElement> listFrameworkElementsChildren=GetAllChildControlsFlat(listFrameworkElementsDirect[i]);
				listFrameworkElements.AddRange(listFrameworkElementsChildren);
			}
			return listFrameworkElements;
		}

		///<summary>This gets a list of all direct child controls. "control" means one of our custom controls in the UI namespace.</summary>
		private List<FrameworkElement> GetDirectChildControls(FrameworkElement frameworkElementParent){
			//The parent passed in should be the direct parent of all the contained controls, which is a WPF Grid for our Frms.
			//It might need to be tweaked in the future.
			//For example, the only non-custom control that the logic considers is a WPF Grid
			List<FrameworkElement> listFrameworkElements=new List<FrameworkElement>();
			if(frameworkElementParent is System.Windows.Controls.Grid wpfGrid){
				int count=VisualTreeHelper.GetChildrenCount(wpfGrid);
				for(int i=0;i<count;i++){
					DependencyObject dependencyObject=VisualTreeHelper.GetChild(wpfGrid,i);
					if(!IsControl(dependencyObject)){
						continue;
					}
					listFrameworkElements.Add((FrameworkElement)dependencyObject);
				}
				return listFrameworkElements;
			}
			if(frameworkElementParent is GroupBox groupBox){
				for(int i=0;i<groupBox.Items.Count;i++){
					if(!IsControl((DependencyObject)groupBox.Items[i])){
						continue;
					}
					listFrameworkElements.Add((FrameworkElement)groupBox.Items[i]);
				}
				return listFrameworkElements;
			}
			if(frameworkElementParent is Panel panel){
				for(int i=0;i<panel.Items.Count;i++){
					if(!IsControl((DependencyObject)panel.Items[i])){
						continue;
					}
					listFrameworkElements.Add((FrameworkElement)panel.Items[i]);
				}
				return listFrameworkElements;
			}
			if(frameworkElementParent is TabControl tabControl){
				for(int i=0;i<tabControl.Items.Count;i++){
					if(!IsControl((DependencyObject)tabControl.Items[i])){
						continue;
					}
					listFrameworkElements.Add((FrameworkElement)tabControl.Items[i]);
				}
				return listFrameworkElements;
			}
			if(frameworkElementParent is TabPage tabPage){
				if(IsControl((DependencyObject)tabPage.Content)){//typically a panel
					listFrameworkElements.Add((FrameworkElement)tabPage.Content);
				}
				return listFrameworkElements;
			}
			return new List<FrameworkElement>();
		}

		///<summary>Returns true if the object is one of our custom controls in the UI namespace.</summary>
		private bool IsControl(DependencyObject dependencyObject){
			Type type=dependencyObject.GetType();
			if(!type.IsSubclassOf(typeof(FrameworkElement))){
				return false;
			}
			FrameworkElement frameworkElement=(FrameworkElement)dependencyObject;
			string mynamespace=type.Namespace;
			if(mynamespace=="WpfControls.UI"){
				return true;
			}
			return false;
		}

		///<summary>Returns true if any control within this FrmODBase has keyboard focus (not logical focus)</summary>
		private bool IsFocusAlreadySet(){
			IInputElement iInputElement=Keyboard.FocusedElement;
			DependencyObject dependencyObject = iInputElement as DependencyObject;
			if(dependencyObject is null){
				return false;
			}
			//a control with focus would typically be a WPF textBox inside of one of our custom textBox controls.
			while(true){
				if(dependencyObject == null) {
					break;
				}
				if(dependencyObject is FrmODBase) {
					//we found it
					break;
				}
				//if(IsControl(dependencyObject)){
				//not sure what the point would be. Any control with focus on this Frm is enough.
				//	continue;
				//}
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			if(dependencyObject is null){
				//for example, never found FrmODBase and made it to the top
				return false;
			}
			FrmODBase frmODBase=dependencyObject as FrmODBase;
			if(frmODBase!=this){
				return false;
			}
			return true;
		}

		///<summary>Converts a float or int from 96dpi to current scale, including both MS scale and OD zoom.  Rounds to nearest int. This should only be used when adjusting the form size or position. For any other use, check with Jordan.</summary>
		protected int ScaleFormValue(float val96){
			return _formFrame.UIManager_.Scale(val96);
		}

		///<summary>Converts a float or int from 96dpi to current scale, including both MS scale and OD zoom. Always check with Jordan.</summary>
		protected float ScaleF(float val96){
			return _formFrame.UIManager_.ScaleF(val96);
		}

		///<summary>Example 1.2. This zoom is from ComputerPrefs.LocalComputer.Zoom/100. This method is largely copied from LayoutManagerForms.ScaleMyFont. This zoom number is available to all child frms. It's not needed for WPF because that's already handled. But if there is a WinForms control inside of a WindowsFormsHost, then this zoom needs to be used to ScaleTransform the contents. Only example so far is SignatureBoxWrapper.</summary>
		protected float GetZoom(){
			if(!Db.HasDatabaseConnection()){
				//Cannot continue because ComputerPrefs.LocalComputer will be null and then it will get set to default instead of getting the computer prefs from the database.
				//The symptom of this would be evident as the task dock y position would not be pulled from the database, so it would start in the wrong position.
				return 1;
			}
			if(ComputerPrefs.IsLocalComputerNull()){
				//This happens repeatedly during a conversion, as the progress bar redraws.
				//If we were to continue, the computerpref table could have a different number of columns, so the query below would fail.
				//This would drastically slow down the conversion.
				//ComputerPrefs.LocalComputer will get set right after the conversion is done.
				return 1;
			}
			float zoomLocal=_zoomLocal;//example 1.2
			try{
				zoomLocal=ComputerPrefs.LocalComputer.Zoom/100f;
			}
			catch{
				//this fails during version update, for example
			}
			if(zoomLocal==0){
				zoomLocal=1;
			}
			if(zoomLocal!=_zoomLocal){
				_zoomLocal=zoomLocal;
			}
			return zoomLocal;
		}

		///<summary>Keyboard and logical focus. If it can't find anything, then it sets focus on the FrmODBase itself.</summary>
		private void SetFocusToFirst(){
			if(IsFocusAlreadySet()){
				return;
			}
			System.Windows.Controls.Grid grid=DrillDownToMainGrid();
			if(grid is null){
				Focusable=true;
				Focus();//so that Esc and other keystrokes will work
				return;
			}
			Keyboard.ClearFocus();//Clear to ensure that iInputElementFocused can only be an element from this frm
			SetFocusRecursive(grid);
			IInputElement iInputElementFocused=Keyboard.FocusedElement;//should be null or the result of SetFocusRecursive(grid);
			if(iInputElementFocused==null){
				Focusable=true;
				try{
					Focus();//sets keyboard and logical focus to this FrmODBase.
				}
				catch{
					//This needs a try catch because we got a UE:
					//Object reference not set to an instance of an object.
					//When this happened, we were inside WpfControls.UI.FrmProgressAuto.
					//In FormRpOutstandingIns, the Load calls SetFilterControlsAndAction which fills the grid
					//The UE is probably caused by the threading in the old FilterControlsAndAction,
					//so it would probably be fixed if we switch to the new FilterControlsAndAction.
				}
				//no tabIndexes set
			}
			//bool isFocused=frameworkElementMin.Focus();//this is false because the focus was immediately transferred to the nested textBox, etc.
		}

		///<summary>Keyboard and logical focus. For a group control, it looks through the children for the lowest TabIndex. If it finds one, it recursively calls this method again. For non-group, it just sets focus.</summary>
		private void SetFocusRecursive(FrameworkElement frameworkElement){
			if(frameworkElement is System.Windows.Controls.Grid
				|| frameworkElement is GroupBox)
			{
				List<FrameworkElement> listFrameworkElements=GetDirectChildControls(frameworkElement);
				SetFocusLowestInGroup(listFrameworkElements);
			}
			if(frameworkElement is TextBox textBox){
				textBox.Focus();
				textBox.SelectAll();
			}
			if(frameworkElement is TextRich textRich){
				textRich.Focus();
				textRich.SelectAll();
			}
			if(frameworkElement is TextVDate textVDate){
				textVDate.Focus();
				textVDate.SelectAll();
			}
			if(frameworkElement is TextVDouble textVDouble){
				textVDouble.Focus();
				textVDouble.SelectAll();
			}
			if(frameworkElement is TextVInt textVInt){
				textVInt.Focus();
				textVInt.SelectAll();
			}
		}

		private void SetFocusLowestInGroup(List<FrameworkElement> listFrameworkElements){
			int tabIndexMin=int.MaxValue;
			FrameworkElement frameworkElementMin=null;
			for(int i=0;i<listFrameworkElements.Count;i++){
				//we could test for ReadOnly here, but that would add a fair chunk of code.  Instead, engineer should just not set a read only control to be the first tab index.
				int tabIndex=int.MaxValue;
				if(listFrameworkElements[i] is GroupBox groupBox){
					tabIndex=groupBox.TabIndexOD;
				}
				if(listFrameworkElements[i] is TextBox textBox){
					tabIndex=textBox.TabIndexOD;
				}
				if(listFrameworkElements[i] is TextRich textRich){
					tabIndex=textRich.TabIndexOD;
				}
				if(listFrameworkElements[i] is TextVDate textVDate){
					tabIndex=textVDate.TabIndexOD;
				}
				if(listFrameworkElements[i] is TextVDouble textVDouble){
					tabIndex=textVDouble.TabIndexOD;
				}
				if(listFrameworkElements[i] is TextVInt textVInt){
					tabIndex=textVInt.TabIndexOD;
				}
				if(tabIndex<tabIndexMin){
					tabIndexMin=tabIndex;
					frameworkElementMin=listFrameworkElements[i];
				}
			}
			if(frameworkElementMin!=null){
				SetFocusRecursive(frameworkElementMin);//this could be a single control or a container
			}
		}
		#endregion Methods - private

		#region Methods - private Event Handlers
		private void FormFrame_Activated(object sender,EventArgs e) {
			Activated?.Invoke(this,new EventArgs());
		}

		private void _formFrame_CloseXClicked(object sender,CancelEventArgs e) {
		}

		private void FormFrame_FormClosed(object sender,System.Windows.Forms.FormClosedEventArgs e) {
			//I don't care about the CloseReason, so this can be a plain EventArgs
			FormClosed?.Invoke(sender,new EventArgs());
			//_formFrame?.Dispose();//No. This would cause the main OD form to end up behind other program windows.
			//When Close is called, Win32 Dispose automatically gets called. Maybe we're calling it too soon or something.
		}

		private void FormFrame_FormClosing(object sender,System.Windows.Forms.FormClosingEventArgs e) {
			System.ComponentModel.CancelEventArgs cancelEventArgs=new CancelEventArgs();
			FormClosing?.Invoke(sender,cancelEventArgs);
			if(cancelEventArgs.Cancel){
				e.Cancel=true;
			}
		}

		private void FormFrame_Load(object sender,EventArgs e) {
			//Remember that this Load happens after the frm (UserControl) Loaded, which we totally ignore because this one is far more meaningful.
			Load?.Invoke(this,new EventArgs());//This is where the Frm's Load (not Loaded) event happens
			SetFocusToFirst();
			//Queue this action to run when the application becomes idle, which is after rendering.
			//This worked, but it was easier to just do it when the form is shown.
			//Action action=() => Shown?.Invoke(this,EventArgs.Empty);
			//Dispatcher.BeginInvoke(action,DispatcherPriority.ContextIdle);
			//if(_heightPending96!=0){
			//	_formFrame.Height=_formFrame.UIManager_.Scale(_heightPending96);
			//}
			//if(_heightPendingAbs!=0){
			//	_formFrame.Height=_heightPendingAbs;
			//}
			//if(_widthPending96!=0){
			//	_formFrame.Width=_formFrame.UIManager_.Scale(_widthPending96);
			//}
			//if(_widthPendingAbs!=0){
			//	_formFrame.Width=_widthPendingAbs;
			//}
		}

		private void Frm_PreviewKeyDown(object sender,KeyEventArgs e) {
			//We use preview so that this will work even if user is inside a textbox.
			//That textbox would mark a KeyDown event as handled so we wouldn't see it.
			//Preview is a tunneling event which is raised before a bubbling event like KeyDown.
			//Look in Button.xaml.cs for example of how to handle Alt-key combos
			//--------------------------------------------------------------------------------------------------------------
			//Here's an example of an ordinary key event handler for a form.
			//Create an event handler in your frm constructor:
			//PreviewKeyDown+=Frm_PreviewKeyDown;
			//Then:
			//private void Frm_PreviewKeyDown(object sender,KeyEventArgs e) {
			//	if(Keyboard.Modifiers=ModifierKeys.Control && e.Key==Key.C){
						//You might need to test e.SystemKey instead of e.Key.
						//Example is when modifier is Alt because that causes it to be a system command.
			//		butCopy_Click(this,new EventArgs());
			//	}
			//}
			//--------------------------------------------------------------------------------------------------------------
			//Using Esc key to close the form:
			//This was previously done by setting Form.CancelButton to point to the Cancel button.
			//WPF does not have that, so instead, we make Esc be equivalent to clicking the window X in all cases. 
			//It's already built in for you here:
			if(e.Key==Key.Escape) {
				//CancelEventArgs ea=new CancelEventArgs();
				//if(ea.Cancel){
				//	return;
				//}
				if(EscClosesWindow){
					Close();
				}
			}
			//You can always create an event handler for CloseXClicked or FormClosing if you want to warn or block them.
			//--------------------------------------------------------------------------------------------------------------
			//Hooking the Enter key up to the OK button was previously done in WinForms by setting Form.AcceptButton.
			//We didn't use this a lot because multiline textboxes also accept enter, so it's inconsistent.
			//In the appointment edit window, we instead use O for ok and C for cancel, which is weird and non-standard.  It was probably needed for automation.
			//But Enter can work if there are no or few multiline textboxes.
			//To implement Enter, do it in your own KeyDown (not PreviewKeyDown) event handler.  Like this:
			//In the constructor:
			//KeyDown+=Frm_KeyDown;
			//...then...
			//private void Frm_KeyDown(object sender,KeyEventArgs e) {
			//	if(e.Key==Key.Enter) {
			//		butSave_Click(this,new EventArgs());
			//	}
			//}
			//The reason the above lines should not be inside PreviewKeyDown is because then they will intercept Enter used in a multiline comboBox.
			//Instead, just use KeyDown, which is a bubbling event.
			//--------------------------------------------------------------------------------------------------------------
			//There is another detail that is sometimes confused with the above discussion of Enter/Esc for OK/Cancel.
			//In WinForms, there is a setting for button.DialogResult.
			//Our pattern was to always ignore that property and to instead explicitly set DialogResult in the OK and Cancel button event handlers.
			//WPF does not support button.DialogResult anyway, so our existing pattern will continue as it always has.
		}
		#endregion Methods - private Event Handlers
	}
}

#region ignore
/*
Why we are switching to WPF:
GDI+ is just getting too old.  I keep running into unsolvable issues when drawing, and I'm just done with it.
Because of the clearly unsolvable high DPI scaling issues, I expect MS to deprecate WinForms. There's just no other way. MS doesn't have the resources to plug a million holes.
WPF uses DirectX, which means the drawing is predictable and clean, with proper layers, transparency, fonts, and antialiasing. No compromises.
In some cases, WPF might be significantly faster, like if we load images onto the GPU and then manipulate them with filters instead of depending on the CPU.
DirectX9 uses a declarative retained-mode scene. DirectX11 uses immediate procedural drawing that does not retain a scene.
WPF uses DX9, which is being deprecated. But MS has mapped it to DX11 so it's fairly future proof. The DX9 paradigm is much easier to program against, and we will take advantage of that. 
WPF controls and classes are rich and very organized. I can build custom controls so fast and with so much less code. It's incredibly fun compared to WinForms.
But there are two major downsides to WPF: 
1. Binding is not powerful enough. When deciding what value to set in a property, I need the power of C# flow, logic, loops, variables, etc. XAML doesn't cut it. We will never use binding or MVVM.
2. The WPF designer. I thought it didn't support anchoring, but it does if you use it a certain way. I worked hard to set alignment defaults so that this is no longer an issue.

Long Term Transition Strategy:
A. (done) Use UI Manager to swap out all controls for WPF on some simple forms. This allows validation of the new custom WPF controls. 
B. (done) Organize a way to load up and use a new form that consists solely of a WPF UserControl.
C. (done) Create a tool that converts one form at a time to a WPF UserControl. The UI Manager becomes obsolete. 
D. (In progress) Gradually convert all forms except FormOpenDental, starting with the simplest forms.
E. Move the main modules over to WPF UserControls.
F. Create a new WPF exe project and manually rebuild FormOpenDental as a WPF Window.
At the moment, we are in steps D, meaning we are designing entire windows in WPF.
Getting firmly on WPF seems to be pretty future proof moving forward, with options including WPF in dotnet 7, WinUI, Avalonia, or whatever else they come up with.


We are using the same coordinate system for drawing as in WinForms. They didn't change it until DirectX10.
//https://learn.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-coordinates
//2024-01-13-I have been hitting some places where drawing is off by half a pixel, so I'm not so sure anymor that it's the same coord system.
//But this is rarely an issue because we rarely draw things from scratch in WPF, instead tending to use borders, etc.

How to turn it off:
There will not be a way to turn it off.  Since we are quickly moving to entire WPF windows, there's no going back. This is a one-way trip.
If something is truly broken beyond repair, then I suppose we could manually bring back the old form. But that would need to be a big bug.

There are a number of existing places where the LayoutManager is used to manually move controls around.
Those places will need to be lightly refactored to removing all scaling code. WPF automatically scales.
LayoutManager will only be deprecated after the conversion to WPF is complete.
*/
#endregion ignore
