using System;
using System.Collections.Generic;
using System.IO;
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
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary>Soon to be deprecated.</summary>
	public class UIManager{

		public Canvas Canvas_;
			///<summary>Example 140 to test 140% zoom</summary>
		public float ZoomTest=0;
		public FormFrame FormFrame_;
		//private int _idxHotControl;
		///<summary>This is true if user has added NoDpi.txt. This turns off the UIManager completely.</summary>
		public bool IsDpiSystem;
		public FrmODBase FrmODBaseHosted;

		#region Fields - Drawing, Border, Dpi
		public Color ColorBorder=Color.FromRgb(65,94,154);//the same dark blue color as the grid titles
		private int _heightTitleBar96=26;
		///<summary>Example 1.5. This is the scale of the current screen for the form that owns this instance of LayoutManager.  It gets combined with ComputerPrefs.LocalComputer.Zoom to create ScaleMy.</summary>
		private float _scaleMS=1;
		public System.Drawing.Size SizeClientOriginal;
		///<summary>Pulled from ComputerPrefs.LocalComputer.Zoom/100. Example 1.2. Default is 1, and this is never 0.</summary>
		private float _zoomLocal=1;
		#endregion Fields - Drawing, Border, Dpi

		#region Constructor
		public UIManager(FormFrame formFrame,FrmODBase frmODBase){
			//for FormMaker
			FormFrame_=formFrame;
			FormFrame_.HasHelpButton=frmODBase.HasHelpButton;
			FormFrame_.MinimizeBox=frmODBase.MinMaxBoxes;
			FormFrame_.MaximizeBox=frmODBase.MinMaxBoxes;
			string classType=frmODBase.GetType().Name;
			if(classType.StartsWith("Frm")){
				classType="Form"+classType.Substring(3);
			}
			FormFrame_.Name=classType;
			if(frmODBase.StartMaximized){
				FormFrame_.WindowState=System.Windows.Forms.FormWindowState.Maximized;
			}
			FormFrame_.Text=frmODBase.Text;
			SizeClientOriginal=new System.Drawing.Size((int)frmODBase.Width,(int)frmODBase.Height);
			FrmODBaseHosted=frmODBase;
		}
		#endregion Constructor

		#region Methods - public Add Move
		public void SetElementHost(){
			if(IsDpiSystem){
				return;
			}
			//These get ignored if maximized:
			//Don't do this here because it will be done after this method exits to handle windows scale and zoom
			//_formODBase.Width=_sizeClientOriginal.Width+16;//Warning: this triggers resize event
			//_formODBase.Height=_sizeClientOriginal.Height+8+_heightTitleBar96;//GetHeightTitleBar();
			FormFrame_.PanelBorders=new PanelSubclassBorder();
			FormFrame_.PanelBorders.Name="PanelBorders";
			if(FormFrame.AreBordersMS){
				FormFrame_.PanelBorders.Visible=false;
			}
			//_formODBase.PanelBorders.Bounds=new System.Drawing.Rectangle(new System.Drawing.Point(0,0),_formODBase.Size);
			//_formODBase.PanelBorders.Dock=System.Windows.Forms.DockStyle.Fill;
			FormFrame_.Controls.Add(FormFrame_.PanelBorders);
			FormFrame_.ElementHostUI = new ElementHost();
			//formODBase.ElementHostUI.Anchor=System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
			//formODBase.ElementHostUI.Dock = System.Windows.Forms.DockStyle.Fill;//no. We want to leave titlebar exposed
			if(FormFrame.AreBordersMS){
				FormFrame_.ElementHostUI.Location=new System.Drawing.Point(0,0);
				FormFrame_.ElementHostUI.Dock=System.Windows.Forms.DockStyle.Fill;
			}
			else{
				FormFrame_.ElementHostUI.Size=new System.Drawing.Size(FormFrame_.Width,FormFrame_.Height-GetHeightTitleBar());
				FormFrame_.ElementHostUI.Location=new System.Drawing.Point(0,GetHeightTitleBar());
			}
			FormFrame_.ElementHostUI.Child=FrmODBaseHosted;
			//margins should be 0 and stretch by default, but just in case:
			FrmODBaseHosted.Margin=new Thickness(0);
			FrmODBaseHosted.HorizontalAlignment=HorizontalAlignment.Stretch;
			FrmODBaseHosted.VerticalAlignment=VerticalAlignment.Stretch;
			FormFrame_.Controls.Add(FormFrame_.ElementHostUI);
		}
		#endregion Methods - public Add Move

		#region Methods - public
		///<summary>This is scaled. 26 at 96dpi.</summary>
		public int GetHeightTitleBar(){
			return Scale(_heightTitleBar96);
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

		///<summary>Converts a float or int from 96dpi to current scale, a combination of MS scale and local zoom.  Rounds to nearest int.</summary>
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

		///<summary>Converts a float or int from current screen dpi to 96dpi.</summary>
		public float UnscaleF(float valScreen){
			return valScreen/ScaleMy();
		}		
		#endregion Methods - public Scaling

		#region Methods - Layout
		///<summary>Sets bounds and font of all child controls.  Does not alter bounds of this form itself. This must get called whether IsLayoutMS or not because it also needs to handle FormODBase.AreBordersMS or not.</summary>
		public void LayoutFormBoundsAndFonts(){
			if(FormFrame_.PanelBorders==null){
				return;
			}
			if(FormFrame_.ElementHostUI==null){
				return;
			}
			if(FormFrame_.IsDisposed){
				return;
			}
//Proxy proxyForm=ListProxies[0];
			//Unlike the way we set this for all other controls, this is reactive for forms because the user sets the form size, not us.
			//If it turns out we need it, here's the pseudocode:
			//proxyForm.Contr96Info_.RectangleF96Now=
			//	new System.Drawing.SizeF(UnscaleF(_formODBase.Size.Width),UnscaleF(_formODBase.Size.Height));
			int hTitle=GetHeightTitleBar();
			//The two calculations below were correct under different math for the form size.
			//but they need to be revised now that we've fixed the form size issues.
			//_formODBase.ElementHostUI.Bounds=new System.Drawing.Rectangle(
			//	x:0,
			//	y:hTitle,
			//	//width:_formODBase.Width-8,//verified at 150%. The last pixel is covered by the single pixel border.
			//	width:_formODBase.Width-16,
			//	height:_formODBase.Height-hTitle-8);//ditto
			////The 16 that was subtracted from width below was verified pixel by pixel at 150% dpi, and it matches perfectly with expectations.
			//_formODBase.PanelBorders.Bounds=new System.Drawing.Rectangle(0,0,_formODBase.Width-16,_formODBase.Height-16);
			//See notes over in FormODBase.Load
			int widthBorder=(int)Math.Round(8+(5*(GetScaleMS()-1)),MidpointRounding.AwayFromZero);
			if(FormFrame.AreBordersMS){
				//ElementHostUI is Dock.Fill
			}
			else if(FormFrame_.WindowState==System.Windows.Forms.FormWindowState.Maximized){
				FormFrame_.ElementHostUI.Bounds=new System.Drawing.Rectangle(
					x: 0,
					y: hTitle+widthBorder,
					width: FormFrame_.Width-widthBorder*2,
					height: FormFrame_.Height-hTitle-widthBorder*2-1);
				//FormFrame window height - title 26 - 8 top/bottom - 1
				//The 1 pixel must be exposed for PanelBorders_MouseMove to get hit.
				//130 lines into that method, there is a section that allows an autohide taskbar to be activated.
				//Jordan-I don't understand this math, but the comments are here for future use.
				//FormFrame_.PanelClient was covering the entire screen, this Z axis overlap was preventing FormFrame_.PanelBorders.Bounds's event PanelBorders_MouseMove from firing.  So we first need to remove 1 pixel from FormFrame_.PanelClient's height, then we need to ensure that FormFrame_.PanelBorders is at least 1 pixel taller so that the bottom pixel is exposed and therefore hoverable for the event.
				//Equation=PanelClient.Height 1053 + y (starting location of title 26 + 8) = 1087 bottom location
				//PanelBorders starting location equals widthborder 8-1 or 7
				//PanelBorders needs a bottom of 1088. Set height equal to PanelBorders bottom 1087 - 6 pixles since PanelBorders starts at y=7 and needs to be 1 pixel taller than PanelClient 1081+7=1088 bottom location.  Now PanelBorders and FormODBase share the same bottom location.
				FormFrame_.PanelBorders.Bounds=new System.Drawing.Rectangle(-1,widthBorder-1,FormFrame_.Width-widthBorder*2+1,FormFrame_.ElementHostUI.Bounds.Bottom-6);//FormODBase_.Height-widthBorder);
			}
			else{
				FormFrame_.ElementHostUI.Bounds=new System.Drawing.Rectangle(
					x: 0,
					y: hTitle,
					width: FormFrame_.Width-widthBorder*2,
					height: FormFrame_.Height-hTitle-widthBorder);
				FormFrame_.PanelBorders.Bounds=new System.Drawing.Rectangle(-1,-1,FormFrame_.Width-widthBorder*2+1,100);//FormODBase_.Height-widthBorder);
			}
			float scaleZoom=ScaleMyFont();//only need to scale by additional zoom.  Canvas automatically scales by MS amount.
			//Matrix matrixOld=Canvas_.LayoutTransform.Value;
			//Matrix matrixNew=new ScaleTransform(scaleZoom,scaleZoom).Value;
			//if(matrixOld!=matrixNew){
				FrmODBaseHosted.LayoutTransform=new ScaleTransform(scaleZoom,scaleZoom);
				//Canvas_.LayoutTransform=new ScaleTransform(scaleZoom,scaleZoom);
			//}
		}
		#endregion Methods - Layout
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
DependencyObject - Visual - UIElement - FrameworkElement - Shape: Rectangle, Line, Ellipse, Path, etc.
DependencyObject - Visual - UIElement - FrameworkElement - Decorator - Border
DependencyObject - Visual - ContainerVisual - DrawingVisual: Lightweight drawing. Host must be derived from FrameworkElement.
DependencyObject - Freezable - Animatable - Drawing: Not sure yet how this differs from DrawingVisual. I think it's pretty much the same.

Tentative plan:
We will use a variety of different WPF controls, all inheriting from FrameworkElement.
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

Here's how to add WPF windows to WinForms projects:
https://stackoverflow.com/questions/3149514/is-it-possible-to-have-a-project-containing-both-winforms-and-wpf

This gem gives some hints about the transition from WF to WPF:
https://learn.microsoft.com/en-us/archive/blogs/wpfsldesigner/layout-techniques-for-windows-forms-developers#content

UserControl is a very good choice, but if it has any visual children, they can't have names. Must use ItemsControl (see GroupBox).
I started putting some ControlTemplates into Generic.xaml because I couldn't get the reference in Generic to point to another file properly.
I can't set styles of MS controls in Generic.xaml because those are not in my assembly.
So for those rare cases (TabControl), another option is to inherit and attach styles in code.
Styles in Generic.xaml also appear to only be available to custom controls, not user controls.

*/