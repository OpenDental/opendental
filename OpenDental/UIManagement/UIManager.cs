using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;//in PresentationFramework.dll
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;//in PresentationCore.dll
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Shapes;
//using System.Windows.Forms;//no, we will always fully qualify controls here.
using System.Windows.Forms.Integration;//In WindowsFormsIntegration.dll
//Also needed WindowsBase and System.Xaml
using OpenDental.UIManagement;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary>This manages all controls on one window, including drawing, laying out, and event handling.</summary>
	public class UIManager{
/*
Housekeeping:
If VS keeps inserting leading spaces here, go to Options, Text Editor, Advanced, uncheck "Use adaptive formatting".
The XAML debugging toolbar is useless to us, so go to VS, Debugging, XAML Hot Reload, uncheck "Enable in-app toolbar".

Why we need a UIManager:
The WPF designer is 4 times slower than the WinForms designer, so we can't ever use it.  We will continue to do all design in WinForms.
But WPF has huge advantages of speed and power of DirectX at runtime for drawing. 
On 4k monitors, GDI+ is simply too slow because there are 4 times as many pixels, all being calculated on CPU. CPUs are not 4 times faster than they were a few years ago.
Moving forward, we must use DirectX.
DirectX9 uses a declarative retained-mode scene. DirectX11 uses immediate procedural drawing that does not retain a scene.
WPF uses DX9, which is being deprecated. But MS has mapped it to DX11 so it's fairly future proof. The DX9 paradigm is much easier to program against, and we will take advantage of that.
WPF adds a lot of functionality that would be costly to duplicate if we use lower level DirectX. 
Still, we are ignoring most of the WPF code because we just manually draw all the controls ourselves on a canvas.
This is the same strategy as WPF uses. Instead of dozens of Controls, there is just one drawing surface that uses DirectX. Each control does not get a Windows handle.

How the UIManager works:
Programmers are no longer allowed to interact directly with any control. All such interaction must go through the UIManager. This includes all reading and writing of all properties.
All mouse and keyboard events are routed to the UI Manager.
The Winform controls never get a handle and never actually get used at all. Their initial properties are copied, and they are immediately set to null.
Their properties always remain exactly as they were at design time, and any state changes of fields or behavior is stored in new state fields.
The LayoutManager is obsolete once a form switches to using the UIManager. LayoutManager was responsible for moving around all the WinForm controls, but UI Manager must leave them all untouched. But similar math.
We are probably using the same coordinate system for drawing as in WinForms. They didn't change it until DirectX10.
//https://learn.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-coordinates

How to turn it off:
The only way is to add the NoDpi.txt, just like before. This will completely skip UIManager and will use the old WinForm controls. The form will scale according to Windows scale.
If you instead right click OpenDental.exe and override high DPI scaling to be system, it will still use the UIManager instead of WinForms, but it will just be slightly blurry at high dpi.
You can turn UIManager off for one window at a time in the code if that window is malfunctioning: InitializeUIManager(isDpiSystem:true); But then it will only work at 100% MS scaling.

How to Implement (tentative):
In the constructor for any FormODBase, just after InitializeComponent(); replace InitializeLayoutManager(); with InitializeUIManager();
There are a number of existing places where the LayoutManager is used to manually move controls around.
Those places will probably need to be refactored to point instead to the UIManager. It will be an easy refactor.
LayoutManager will only be deprecated after UIManager is implemented on all forms and thoroughly tested.


		*/
		public Canvas Canvas_;
			///<summary>Example 140 to test 140% zoom</summary>
		public float ZoomTest=0;

		private OpenDental.FormODBase _formODBase;
		private List<Proxy> _listProxies;
		//private int _idxHotControl;
		private bool _isDpiSystem;

		#region Fields - Drawing, Border, Dpi
		public Color ColorBorder=Color.FromRgb(65,94,154);//the same dark blue color as the grid titles
		private int _heightTitleBar96=26;
		///<summary>Example 1.5. This is the scale of the current screen for the form that owns this instance of LayoutManager.  It gets combined with ComputerPrefs.LocalComputer.Zoom to create ScaleMy.</summary>
		private float _scaleMS=1;
		private System.Drawing.Size _sizeClientOriginal;
		///<summary>Pulled from ComputerPrefs.LocalComputer.Zoom/100. Example 1.2. Default is 1, and this is never 0.</summary>
		private float _zoomLocal=1;
		#endregion Fields - Drawing, Border, Dpi

		#region Constructor
		public UIManager(FormODBase formODBase,bool isDpiSystem){
			_isDpiSystem=isDpiSystem;
			if(isDpiSystem){
				return;
			}
			_formODBase=formODBase;
			_sizeClientOriginal=formODBase.ClientSize;//permanently remember the original client size before Windows fiddles with it.
			_listProxies=new List<Proxy>();
			//List<System.Windows.Forms.Control> listControls=UIHelper.GetAllControls(formODBase).ToList();
			CreateProxies(control:formODBase,proxyParent:null);
			//for(int i=0;i<listControls.Count;i++){
				//bool isHandleCreated=listControls[i].IsHandleCreated;//false
				//listControls[i].Visible=false;//no, we'll remove entirely
				//We can't leave the original controls in place or people will reference them. We instead want an exception wherever a control is referenced directly.
				//bool isHandleCreated=controlCopy.IsHandleCreated;//false
				//listControls[i]?.Dispose();//probably not necessary since no resources yet.
				//listControls[i]=null;//not allowed
				//PassLayoutManager(listControls[i]);
			//}
			//This section nulls out existing controls.  We may or may not do it this way. We'll see.
			Type type=formODBase.GetType();//example FormUIManagerTests
			formODBase.Controls.Clear();
			FieldInfo[] fieldInfoArray =  type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i=0;i<fieldInfoArray.Length;i++){
				if(!fieldInfoArray[i].FieldType.IsSubclassOf(typeof(System.Windows.Forms.Control))){
					continue;
				}
				//this sets the original controls to null
				fieldInfoArray[i].SetValue(formODBase,null);//tested that this does indeed cause exception if someone later tries to directly reference any control.
			}
		}

		///<summary>Creates proxies for this control and all its children.</summary>
		private void CreateProxies(System.Windows.Forms.Control control,Proxy proxyParent){
			//System.Windows.Forms.Control controlCopy=null;
			Contr96Info contr96Info=null;
			//if(control is OpenDental.FormODBase){
			//	contr96Info=new Contr96Info();
			//	//controlCopy remains null, which means we need to fill Contr96Info a bit differently.
			//	//We only care about a few fields
			//	contr96Info.Name=control.Name;
			//	//?contr96Info.RectangleFBounds96=control.Bounds;
			//	contr96Info.SizeClient96=control.ClientSize;
			//	contr96Info.SizeFont96=control.Font.Size;
			//}
			//else{
			//controlCopy=CopyControl(control);
			contr96Info=new Contr96Info();
			contr96Info.SetFields(control);
			//}
			if(control.Bounds.Height<0 || control.Bounds.Width<0){
				if(control.Parent is System.Windows.Forms.DomainUpDown){//the child textbox has negative height because we made it so short
					return;
				}
			}
			Proxy proxy=new Proxy();
			if(control is OpenDental.FormODBase){
				proxy.TypeControl=EnumTypeControl.Window;
				proxy.Control_=null;
			}
			else if(control is OpenDental.UI.Button button){
				proxy.TypeControl=EnumTypeControl.Button;
				proxy.Control_=ButtonHelper.CreateButton(button,this);
			}
			else if(control is System.Windows.Forms.TextBox textBox){
				proxy.TypeControl=EnumTypeControl.TextBox;
				proxy.Control_=TextBoxHelper.CreateTextBox(textBox,this);
			}
			else{
				throw new Exception("Unsupported control type: "+control.Name);
			}
			proxy.ControlWinForm=control;
			proxy.Contr96Info_=contr96Info;
			proxy.UIManager_=this;
			proxy.ProxyParent=proxyParent;
			_listProxies.Add(proxy);
			for(int i = 0;i<control.Controls.Count;i++) {
				CreateProxies(control.Controls[i],proxyParent: proxy);
			}
		}
		#endregion Constructor

		#region Methods - public Add Move
		public void SetElementHost(){
			if(_isDpiSystem){
				return;
			}
			//Bounds will be set again after this method exits to handle windows scale and zoom
			//These get ignored if maximized:
			_formODBase.Width=_sizeClientOriginal.Width+16;//Warning: this triggers resize event
			_formODBase.Height=_sizeClientOriginal.Height+8+GetHeightTitleBar();
			_formODBase.PanelBorders=new UI.PanelOD();
			_formODBase.PanelBorders.Name="PanelBorders";
			//_formODBase.PanelBorders.Bounds=new System.Drawing.Rectangle(new System.Drawing.Point(0,0),_formODBase.Size);
			//_formODBase.PanelBorders.Dock=System.Windows.Forms.DockStyle.Fill;
			_formODBase.Controls.Add(_formODBase.PanelBorders);
			_formODBase.ElementHostUI = new ElementHost();
			//formODBase.ElementHostUI.Anchor=System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
			//formODBase.ElementHostUI.Dock = System.Windows.Forms.DockStyle.Fill;//no. We want to leave titlebar exposed
			_formODBase.ElementHostUI.Size=new System.Drawing.Size(_formODBase.Width,_formODBase.Height-GetHeightTitleBar());
			_formODBase.ElementHostUI.Location=new System.Drawing.Point(0,GetHeightTitleBar());
			Canvas_=new Canvas();
			_formODBase.ElementHostUI.Child = Canvas_;
			//_formODBase.ElementHostUI. //can't find mouse event for elementHost. I could not solve this problem, so I moved on.
			_formODBase.MouseMove+=FormODBase_MouseMove;
			Canvas_.MouseMove+=Canvas_MouseMove;
			Canvas_.MouseLeftButtonDown+=Canvas__MouseLeftButtonDown;
			Canvas_.MouseUp+=Canvas__MouseUp;
			_formODBase.Controls.Add(_formODBase.ElementHostUI);
			//without a background, mouse events do not get received.
			//todo: cache all brushes
			Canvas_.Background=new SolidColorBrush(ColorOD.GetColorBackground());
			/*
			ButtonProxy canvas2=new ButtonProxy();
			canvas2.Width=300;
			canvas2.Height=200;
			canvas2.Background=Brushes.LightCyan;
			Canvas_.Children.Add(canvas2);
			Button button=new Button();
			button.Background=Brushes.Black;
			button.Width=100;
			button.Height=30;
			canvas2.Children.Add(button);*/
			AddProxyToCanvas(_listProxies[0]);//0 is always the form
		}

		///<summary>Adds this proxy and all its children to the canvas.</summary>
		private void AddProxyToCanvas(Proxy proxy){
			if(proxy.ProxyParent is null){
				//form, don't do anything
			}
			else{
				Canvas_.Children.Add(proxy.Control_);
			}
			List<Proxy> listBaseProxiesChildren=_listProxies.FindAll(x=>x.ProxyParent==proxy);
			for(int i=0;i<listBaseProxiesChildren.Count;i++){
				AddProxyToCanvas(listBaseProxiesChildren[i]);//this is just temporary until we build the drawing framework
			}
		}
		#endregion Methods - public Add Move

		#region Methods - public
		///<summary>This is scaled. 26 at 96dpi.</summary>
		public int GetHeightTitleBar(){
			return Scale(_heightTitleBar96);
		}

		private void FormODBase_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			//doesn't work because elementhost covers everything
			return;
		}

		///<summary>Example 1.8. This is the scale of this form and all its controls, compared to 96dpi as 100%.  It's a combination of _scaleMS and ComputerPrefs.LocalComputer.Zoom.</summary>
		public float ScaleMy(){
			if(ZoomTest!=0){
				return _scaleMS+ZoomTest/100f;
			}
			if(!Db.HasDatabaseConnection()){
				//Cannot continue because ComputerPrefs.LocalComputer will be null and then it will get set to default instead of getting the computer prefs from the database.
				//The symptom of this would be evident as the task dock y position would not be pulled from the database, so it would start in the wrong position.
				return _scaleMS;
			}
			if(ComputerPrefs.IsLocalComputerNull()){
				//This happens repeatedly during a conversion, as the progress bar redraws.
				//If we were to continue, the computerpref table could have a different number of columns, so the query below would fail.
				//This would drastically slow down the conversion.
				//ComputerPrefs.LocalComputer will get set right after the conversion is done.
				return _scaleMS;
			}
			float zoomLocal=_zoomLocal;
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
			float retVal=_scaleMS*zoomLocal;//1.5*1.2=1.8
			return retVal;
		}

		///<summary>Example 1.2. This is the scale for Fonts, which is only ComputerPrefs.LocalComputer.Zoom/100.</summary>
		public float ScaleMyFont(){
			if(ZoomTest!=0){
				return (ZoomTest/100f*_scaleMS);//120/100*1.5=1.8
			}
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

		///<summary>Example 1.5</summary>
		public float GetScaleMS(){
			return _scaleMS;
		}

		///<summary>Example 1.5. Gets combined with zoom to create ScaleMy.</summary>
		public void SetScaleMS(float scaleMS){
			_scaleMS=scaleMS;
		}
		#endregion Methods - public

		#region Methods - public Scaling
		///<summary></summary>
		public int Round(float val){
			return (int)Math.Round(val);
		}

		///<summary>Converts a float or int from 96dpi to current scale.  Rounds to nearest int.</summary>
		public int Scale(float val96){
			return Round(val96*ScaleMy());
		}

		///<summary>Converts a float or int from 96dpi to current scale.</summary>
		public float ScaleF(float val96){
			float retVal=val96*ScaleMy();
			return retVal;
		}

		///<summary>Converts a float or int from 96dpi to current scale, but only (most of) the OD zoom, not the MS scale.</summary>
		public float ScaleFontODZoom(float val96){
			float scaleMyFont=ScaleMyFont();
			float retVal=val96*scaleMyFont;
			return retVal;
		}

		///<summary>Scales a number only by the MS scale component.  This is used after we measure a font that was only scaled by the ODZoom component.</summary>
		public float ScaleMS(float val96){
			float retVal=val96*_scaleMS;
			return retVal;
		}
		#endregion Methods - public Scaling

		#region Methods - Layout
		///<summary>Sets bounds and font of all child controls.  Does not alter bounds of this form itself. This must get called whether IsLayoutMS or not because it also needs to handle FormODBase.AreBordersMS or not.</summary>
		public void LayoutFormBoundsAndFonts(){
			if(_formODBase.PanelBorders==null){
				return;
			}
			if(_formODBase.ElementHostUI==null){
				return;
			}
			if(_formODBase.IsDisposed){
				return;
			}
			int hTitle=GetHeightTitleBar();
			_formODBase.ElementHostUI.Bounds=new System.Drawing.Rectangle(
				x:0,
				y:hTitle,
				width:_formODBase.Width-8,//verified at 150%. The last pixel is covered by the single pixel border.
				height:_formODBase.Height-hTitle-8);//ditto
			//The 16 that was subtracted from width below was verified pixel by pixel at 150% dpi, and it matches perfectly with expectations.
			_formODBase.PanelBorders.Bounds=new System.Drawing.Rectangle(0,0,_formODBase.Width-16,_formODBase.Height-16);
			float scaleInv=1/ScaleMy();
			Matrix matrixOld=Canvas_.LayoutTransform.Value;
			Matrix matrixNew=new ScaleTransform(scaleInv,scaleInv).Value;
			if(matrixOld!=matrixNew){
				Canvas_.LayoutTransform=new ScaleTransform(scaleInv,scaleInv);
			}
			List<Proxy> listProxiesChildren=_listProxies.FindAll(x=>x.ProxyParent==_listProxies[0]);
			for(int i=0;i<listProxiesChildren.Count;i++){
				LayoutThisAndChildren(listProxiesChildren[i]);
			}
		}

		///<summary>Sets bounds and font of all child controls.  Does not alter bounds of this control itself.  The control passed in will already be at the correct size. Intentionally separate from MS layout, which should have no effect on the controls because we removed all their anchors.</summary>
		public void LayoutControlBoundsAndFonts(Control control){
			//LayoutChildren(control);
		}

		///<summary>Recursive. Changes this control and all its children. This is a slightly different strategy than the similar LayoutManager.LayoutChildren. Don't pass in a windowProxy.</summary>
		private void LayoutThisAndChildren(Proxy proxy){
			if(proxy.TypeControl==EnumTypeControl.Window){
				throw new Exception("not allowed");
			}
			Contr96Info contr96InfoParent=proxy.ProxyParent.Contr96Info_;
			Contr96Info contr96Info=proxy.Contr96Info_;
			//FONT=================================
			float scaledFont=ScaleF(ScaleF(contr96Info.SizeFont96));
			float x=contr96Info.RectangleFBounds96.X;
			float y=contr96Info.RectangleFBounds96.Y;
			float width=contr96Info.RectangleFBounds96.Width;
			float height=contr96Info.RectangleFBounds96.Height;
			//Todo: anchors
			//Now, we apply the newly calculated positions to the control, converting to new scale
			proxy.Control_.FontSize=scaledFont;
			proxy.Control_.Width=Scale(width);
			proxy.Control_.Height=Scale(height);
			Canvas.SetLeft(proxy.Control_,Scale(x));
			Canvas.SetTop(proxy.Control_,Scale(y));
			//proxy.UpdateState();
			//control96Info.BoundsLast//not needed since it's impossible to manually move a control
			List<Proxy> listProxiesChildren=_listProxies.FindAll(x=>x.ProxyParent==proxy);
			for(int i=0;i<listProxiesChildren.Count;i++){
				LayoutThisAndChildren(listProxiesChildren[i]);
			}
		}

		///<summary>Passes this LayoutManager object to any of our custom controls so that they have access to the same scale numbers as the form.</summary>
		private void PassLayoutManager(System.Windows.Forms.Control control){
			//stub
		}
		#endregion Methods - Layout

		/*
		private System.Windows.Forms.Control CopyControl<T>(T controlToClone) where T:System.Windows.Forms.Control{
			PropertyInfo[] propertyInfoArray = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Type type=controlToClone.GetType();//example UI.Button
			dynamic controlCopy = Activator.CreateInstance(type);//dynamic variable so that it can take any type
			for(int i=0;i<propertyInfoArray.Length;i++){
				if(!propertyInfoArray[i].CanWrite){
					continue;
				}
				if(propertyInfoArray[i].Name=="WindowTarget"){
					continue;
				}
				if(propertyInfoArray[i].Name=="Parent"){
					continue;//we won't set this or the copy becomes attached to the same control as the original
				}
				propertyInfoArray[i].SetValue(controlCopy, propertyInfoArray[i].GetValue(controlToClone, null), null);
			}
			type=controlToClone.GetType();
			return controlCopy;
		}*/

		#region Methods - private Event Handlers
		private void Canvas__MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Point point=e.GetPosition(Canvas_);
			for(int i=0;i<_listProxies.Count;i++){
				//if(!_listBaseProxies[i].RectBoundsCanvas.Contains(point)){
				//	continue;
				//}
				//_listBaseProxies[i].OnMouseLeftButtonDown(e);
			}
			//This is all an attempt to have mouse move events still fire outside the canvas.
			//Necessary for dragging window quickly.  I gave up and switched the titlebar back to Winforms control.
			//Canvas_.CaptureMouse();//didn't work
			//Mouse.Capture(Canvas_,CaptureMode.SubTree);//didn't work
			//Mouse.Capture(Canvas_);//didn't work
			//e.Handled=true;//didn't help
		}

		private void Canvas_MouseMove(object sender,System.Windows.Input.MouseEventArgs e){
			Point point=e.GetPosition(Canvas_);
			//first loop establishes the idxHotControl
			int idxHotControl=-1;
			for(int i=0;i<_listProxies.Count;i++){
				//if(_listBaseProxies[i].RectBoundsCanvas.Contains(point)){
//todo: nesting, because nested controls would result in 2+ hot controls, and we should only pick the child
					//We will use z-order 1 for the Window, and increment the z order as we lay out nested controls.
					//Then, this method just needs to find the highest z order.
				//	idxHotControl=i;
				//	break;
				//}
			}
			/*if(_idxHotControl!=-1 && idxHotControl!=_idxHotControl){
				_listBaseProxies[_idxHotControl].OnMouseLeave();
			}
			_idxHotControl=idxHotControl;
			if(_idxHotControl==-1){
				return;
			}
			_listBaseProxies[_idxHotControl].OnMouseMove(e);*/
		}

		private void Canvas__MouseUp(object sender,MouseButtonEventArgs e) {
			//This does not fire if mouse was moved outside of Canvas/Window between mousedown and here.
			//MS behaves as follows:
			//MS Mouse up only happens on the same control as mouse down.
			//MS Mouse up does fire if you move outside the control after mouse down, even outside the window. But we don't get an event for outside the window.
			//I have always hated the MS paradigm of forcing us to keep a local variable for isMouseDown based on mouse events.
			//It's not reliable, and causes bugs. For example if a MsgBox comes up while mouse is down, mouse up won't fire, and control gets stuck to mouse.
			//There's a better way. During mouse move or wherever else we care, we will always instead check like this:
			//bool isMouseDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
			//The variable is important for debugging, etc.
			Point point=e.GetPosition(Canvas_);
			for(int i=0;i<_listProxies.Count;i++){
				//if(!_listBaseProxies[i].RectBoundsCanvas.Contains(point)){
				//	continue;
				//}
				//_listBaseProxies[i].OnMouseUp();
			}
		}
		#endregion Methods - private Event Handlers
	}
}



/*
Ignore these notes:
DependencyObject - Visual: Equivalent level to HWND
DependencyObject - Visual - UIElement: Provides input, focusing, routed events, 
DependencyObject - Visual - UIElement - FrameworkElement: Supports databinding, styles, animation, etc.
DependencyObject - Visual - UIElement - FrameworkElement - Control: 
DependencyObject - Visual - UIElement - FrameworkElement - Control - ContentControl - ButtonBase - Button:
DependencyObject - Visual - UIElement - FrameworkElement - Panel - Canvas: 
DependencyObject - Visual - UIElement - FrameworkElement - Shape: Rectangle, Line, Ellipse, etc.
DependencyObject - Visual - ContainerVisual - DrawingVisual: Lightweight drawing. Host must be derived from FrameworkElement.
DependencyObject - Freezable - Animatable - Drawing: Not sure yet how this differs from DrawingVisual. I think it's pretty much the same.

Tentative plan:
We will use a variety of different WPF controls, all inheriting from Control.
They will mostly be things like Buttons and CheckBoxes.
Some of them might contain a DrawingVisual if we want to draw from scratch. https://stackoverflow.com/questions/4631483/wpf-contentcontrol-content-as-drawingvisual
Some of them might be a FrameworkElement with Shapes on them.
Sometimes we might use a container if we want to group multiple controls.
Run the events off of the controls.

Performance:
This example doesn't really help:
https://stackoverflow.com/questions/18695224/performance-of-shapes-versus-drawingvisual
except that it gives me ideas on how to optimize performance.
Since I want hover effects on scrollable grids, I suspect I will be using Shapes so that I can change their color without a redraw.

Without inheritance, I could:
-Make a Proxy class that handled all the common state fields. This is enough info for all layout.
-Each Proxy instance corresponds to a control.
-Each Proxy instance will store something in its Control field. This is what gets placed in the UI.
-We need an enumeration to tell us which kind of control each proxy represents.
-Yes, there will be a series of "if" or "switch" statements to route the logic that's specific to different types.

*/