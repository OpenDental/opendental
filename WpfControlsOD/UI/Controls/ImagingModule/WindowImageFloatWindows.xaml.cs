using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenDentBusiness;
using OpenDental;

namespace WpfControls.UI {
/*
Jordan is the only one allowed to edit this file.
*/
	///<summary>This is a menu-style window.  No title bar.  Hovers in place until the user clicks on an action or until it loses focus.</summary>
	public partial class WindowImageFloatWindows:Window {
		#region Fields - Public
		///<summary>These are the lower two points of the button that launched this window, in screen coordinates.  This window will roughly center its top edge on these anchor points and will also omit the outline between these two points so that it looks more like a menu.</summary>
		public Point PointAnchor1;
		///<summary>These are the lower two points of the button that launched this window, in screen coordinates.  This window will roughly center its top edge on these anchor points and will also omit the outline between these two points so that it looks more like a menu.</summary>
		public Point PointAnchor2;
		#endregion Fields - Public

		#region Fields - Private
		private Color _colorBack=Color.FromRgb(224,224,224);
		private Color _colorOutline=Color.FromRgb(140,140,140);
		private Color _colorHover=Color.FromRgb(180,200,220);
		private bool _isClosed;
		//we'll need to do the drawing with parameters instead of hard numbers
		///<summary>10. Margin between screens and around sides.</summary>
		private double _marginOuter=10;
		///<summary>5. Margin within each screen between snap locations.</summary>
		private double _marginInner=5;
		private Path _pathHalf_L;
		private Path _pathHalf_R;
		private Path _pathMax;
		private Path _pathCenter;
		private Path _pathQuarter_UL;
		private Path _pathQuarter_UR;
		private Path _pathQuarter_LL;
		private Path _pathQuarter_LR;
		private Path _pathHalf_L2;
		private Path _pathHalf_R2;
		private Path _pathMax2;
		private Path _pathCenter2;
		private Path _pathQuarter_UL2;
		private Path _pathQuarter_UR2;
		private Path _pathQuarter_LL2;
		private Path _pathQuarter_LR2;
		///<summary>80x48. The size of each "screen" area.</summary>
		private Size _sizeScreen=new Size(80,48);
		#endregion Fields - Private

		#region Fields - Private for Properties
		
		#endregion Fields - Private for Properties

		#region Constructor
		public WindowImageFloatWindows() {
			InitializeComponent();
			border.LostKeyboardFocus+=border_LostKeyboardFocus;
			Closing+=WindowImageFloatWindows_Closing;
			Loaded+=WindowImageFloatWindows_Loaded;
			MouseMove+=WindowImageFloatWindows_MouseMove;
			MouseDown+=WindowImageFloatWindows_MouseDown;
		}
		#endregion Constructor

		#region Events
		///<summary>User clicked one of many buttons.  Bubbles up to FormImageFloat or ControlImageDock, and then to ControlImages, where it's handled.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EnumImageFloatWinButton> EventButClicked=null;
		#endregion Events

		#region Properties

		#endregion Properties

		#region Methods - Event Handlers
		private void border_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//IInputElement iInputElement=Keyboard.FocusedElement;
			if(_isClosed){
				return;
			}
			Close();
		}

		private void butCloseOthers_Click(object sender,EventArgs e) {
			EventButClicked?.Invoke(this,EnumImageFloatWinButton.CloseOthers);
		}

		private void butDockThis_Click(object sender,EventArgs e) {
			EventButClicked?.Invoke(this,EnumImageFloatWinButton.DockThis);
			Close();
			_isClosed=true;//to prevent WindowImageFloatWindows_MouseDown
		}

		private void butShowAll_Click(object sender,EventArgs e) {
			EventButClicked?.Invoke(this,EnumImageFloatWinButton.ShowAll);
		}

		private void WindowImageFloatWindows_Closing(object sender,CancelEventArgs e) {
			_isClosed=true;
		}

		private void WindowImageFloatWindows_Loaded(object sender,RoutedEventArgs e) {
			Keyboard.Focus(border);
			System.Drawing.Point drawing_PointScreen=new System.Drawing.Point((int)PointAnchor1.X,(int)PointAnchor1.Y);
			System.Windows.Forms.Screen screenThis=System.Windows.Forms.Screen.FromPoint(drawing_PointScreen);
			System.Windows.Forms.Screen screen2=null;
			System.Windows.Forms.Screen[] screenArray=System.Windows.Forms.Screen.AllScreens;
			if(screenArray.Length>1){
				screen2=screenArray[0];
				if(screen2.Bounds==screenThis.Bounds){//probably a better way to do this
					screen2=screenArray[1];
				}
			}
			Typeface typeface=new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
			double pixelsPerDip=VisualTreeHelper.GetDpi(new System.Windows.Controls.Control()).PixelsPerDip;//1 in testing on a 150% scale monitor, which seems wrong.
			FormattedText formattedText = new FormattedText("A",CultureInfo.CurrentCulture,FlowDirection.LeftToRight,typeface,11.5,Brushes.Black,pixelsPerDip);
			double heightFont=formattedText.Height;//15.3 DIPs
			if(screen2 !=null){
				UI.Label label=new Label();
				label.Margin=new Thickness(_marginOuter-2,4,0,0);
				label.Width=150;
				label.Height=heightFont;
				label.Text="This screen:";
				grid.Children.Add(label);
			}
			//screen rectangles----------------------------------------------------------------------------------
			double x=_marginOuter;
			double y=4+heightFont+4;
			double w=(_sizeScreen.Width-_marginInner)/2;
			double h=_sizeScreen.Height;
			//Half is at UL-------------------------------------------------------------------------------------
			_pathHalf_L=CreatePath(x,y,w,h,roundUL:true,roundLL:true);
			CreateCenteredLabel(x,y,w,h,"Left");
			_pathHalf_R=CreatePath(x+w+_marginInner,y,w,_sizeScreen.Height,roundUR:true,roundLR:true);
			CreateCenteredLabel(x+w+_marginInner,y,w,_sizeScreen.Height,"Right");
			//Max and Center at U middle----------------------------------------------------------------------
			x=x+_sizeScreen.Width+_marginOuter;
			w=_sizeScreen.Width;
			h=_sizeScreen.Height;
			_pathMax=CreatePath(x,y,w,h,roundUR:true,roundLR:true,roundUL:true,roundLL:true);
			w=_sizeScreen.Width/5*3;
			h=29;
			_pathCenter=CreatePath(x+_sizeScreen.Width/5,y+10,w,h);
			CreateCenteredLabel(x+_sizeScreen.Width/5,y+10,w,h,"Center");
			//Quarter is at UR-----------------------------------------------------------------------------------
			x=x+_sizeScreen.Width+_marginOuter;
			w=(_sizeScreen.Width-_marginInner)/2;
			h=(_sizeScreen.Height-_marginInner)/2;
			_pathQuarter_UL=CreatePath(x,y,w,h,roundUL:true);
			CreateCenteredLabel(x,y,w,h,"UL");
			_pathQuarter_UR=CreatePath(x+w+_marginInner,y,w,h,roundUR:true);
			CreateCenteredLabel(x+w+_marginInner,y,w,h,"UR");
			_pathQuarter_LL=CreatePath(x,y+h+_marginInner,w,h,roundLL:true);
			CreateCenteredLabel(x,y+h+_marginInner,w,h,"LL");
			_pathQuarter_LR=CreatePath(x+w+_marginInner,y+h+_marginInner,w,h,roundLR:true);
			CreateCenteredLabel(x+w+_marginInner,y+h+_marginInner,w,h,"LR");
			//Second Screen======================================================================================
			if(screen2 !=null){
				x=_marginOuter;
				y+=_sizeScreen.Height+4;
				UI.Label label=new Label();
				label.Margin=new Thickness(x-2,y,0,0);
				label.Width=150;
				label.Height=heightFont;
				label.Text="Other screen:";
				grid.Children.Add(label);
				y+=heightFont+4;
				//Half is at UL-------------------------------------------------------------------------------------
				w=(_sizeScreen.Width-_marginInner)/2;
				h=_sizeScreen.Height;
				_pathHalf_L2=CreatePath(x,y,w,h,roundUL:true,roundLL:true);
				CreateCenteredLabel(x,y,w,h,"Left");
				_pathHalf_R2=CreatePath(x+w+_marginInner,y,w,_sizeScreen.Height,roundUR:true,roundLR:true);
				CreateCenteredLabel(x+w+_marginInner,y,w,_sizeScreen.Height,"Right");
				//Max and Center at U middle----------------------------------------------------------------------
				x=x+_sizeScreen.Width+_marginOuter;
				w=_sizeScreen.Width;
				h=_sizeScreen.Height;
				_pathMax2=CreatePath(x,y,w,h,roundUR:true,roundLR:true,roundUL:true,roundLL:true);
				w=_sizeScreen.Width/5*3;
				h=29;
				_pathCenter2=CreatePath(x+_sizeScreen.Width/5,y+10,w,h);
				CreateCenteredLabel(x+_sizeScreen.Width/5,y+10,w,h,"Center");
				//Quarter is at UR-----------------------------------------------------------------------------------
				x=x+_sizeScreen.Width+_marginOuter;
				w=(_sizeScreen.Width-_marginInner)/2;
				h=(_sizeScreen.Height-_marginInner)/2;
				_pathQuarter_UL2=CreatePath(x,y,w,h,roundUL:true);
				CreateCenteredLabel(x,y,w,h,"UL");
				_pathQuarter_UR2=CreatePath(x+w+_marginInner,y,w,h,roundUR:true);
				CreateCenteredLabel(x+w+_marginInner,y,w,h,"UR");
				_pathQuarter_LL2=CreatePath(x,y+h+_marginInner,w,h,roundLL:true);
				CreateCenteredLabel(x,y+h+_marginInner,w,h,"LL");
				_pathQuarter_LR2=CreatePath(x+w+_marginInner,y+h+_marginInner,w,h,roundLR:true);
				CreateCenteredLabel(x+w+_marginInner,y+h+_marginInner,w,h,"LR");
			}
			//Other controls=====================================================================================
			x=_marginOuter;
			y+=_sizeScreen.Height+_marginOuter;
			butDockThis.Margin=new Thickness(x,y,0,0);
			y+=29;
			butCloseOthers.Margin=new Thickness(x,y,0,0);
			y+=29;
			butShowAll.Margin=new Thickness(x,y,0,0);
			y=butDockThis.Margin.Top;
			x+=_sizeScreen.Width+_marginOuter;
			labelWindows.Margin=new Thickness(x,y,0,0);
			y+=20;
			listBoxWindows.Margin=new Thickness(x,y,0,0);
			listBoxWindows.Width=_marginOuter+_sizeScreen.Width*2;
			/*
			List<string> listFormImageFloats=null;
			if(_formImageFloat!=null){
				listFormImageFloats=_formImageFloat.FuncListFloaters();
			}
			if(_controlImageDock!=null){
				listFormImageFloats=_controlImageDock.FuncListFloaters();
			}
			for(int i = 0;i<listFormImageFloats.Count;i++) {
				listBoxWindows.Items.Add(listFormImageFloats[i]);
				//todo: set selected
				//if(listFormImageFloats[i]==_formImageFloat) {
				//	listBoxWindows.SetSelected(i);
				//}
			}
			listBoxWindows.Height=(int)LayoutManager.ScaleMS(font.Height)*listFormImageFloats.Count+4;//pulled from ListBoxOD.IntegralHeight.
			*/
			//Size and Location of window ==========================================================================
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			double scaleWindows=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			//Width and Height are correctly specified in DIPs:
			Width=_marginOuter*4+_sizeScreen.Width*3;
			double bottomButtons=butShowAll.Margin.Top+butShowAll.Height;
			double bottomList=listBoxWindows.Margin.Top+listBoxWindows.Height;
			if(bottomList>bottomButtons){
				Height=bottomList+_marginOuter;
			}
			else{
				Height=bottomButtons+_marginOuter;
			}
			//Math from here down must use desktop pixels, not 96 DIPs:
			double width=Width*scaleWindows;
			double left=(PointAnchor1.X+PointAnchor2.X)/2-width/2;
			System.Drawing.Rectangle drawing_RectangleBounds=System.Windows.Forms.Screen.GetWorkingArea(drawing_PointScreen);
			if(left+width>drawing_RectangleBounds.Right-10){
				left=drawing_RectangleBounds.Right-width-10;
			}
			double top=PointAnchor1.Y;
			//changing location here does not cause flicker
			Point pointDIP=new Point(left/scaleWindows,top/scaleWindows);
			Left=pointDIP.X;
			Top=pointDIP.Y;
			
		}

		private void WindowImageFloatWindows_MouseDown(object sender,MouseButtonEventArgs e) {
			if(_isClosed){
				return;//because this click event always gets fired after a button click event. This one is wrong.
			}
			EnumImageFloatWinButton enumImageFloatWinButton=EnumImageFloatWinButton.None;
			Point point=e.GetPosition(_pathHalf_L);
			if(_pathHalf_L.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Half_L;
			}
			point=e.GetPosition(_pathHalf_R);
			if(_pathHalf_R.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Half_R;
			}
			point=e.GetPosition(_pathCenter);
			if(_pathCenter.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Center;
			}
			point=e.GetPosition(_pathQuarter_UL);
			if(_pathQuarter_UL.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_UL;
			}
			point=e.GetPosition(_pathQuarter_UR);
			if(_pathQuarter_UR.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_UR;
			}
			point=e.GetPosition(_pathQuarter_LL);
			if(_pathQuarter_LL.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_LL;
			}
			point=e.GetPosition(_pathQuarter_LR);
			if(_pathQuarter_LR.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_LR;
			}
			point=e.GetPosition(_pathHalf_L2);
			if(_pathHalf_L2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Half_L2;
			}
			point=e.GetPosition(_pathHalf_R2);
			if(_pathHalf_R2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Half_R2;
			}
			point=e.GetPosition(_pathCenter2);
			if(_pathCenter2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Center2;
			}
			point=e.GetPosition(_pathQuarter_UL2);
			if(_pathQuarter_UL2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_UL2;
			}
			point=e.GetPosition(_pathQuarter_UR2);
			if(_pathQuarter_UR2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_UR2;
			}
			point=e.GetPosition(_pathQuarter_LL2);
			if(_pathQuarter_LL2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_LL2;
			}
			point=e.GetPosition(_pathQuarter_LR2);
			if(_pathQuarter_LR2.Data.FillContains(point)){
				enumImageFloatWinButton=EnumImageFloatWinButton.Quarter_LR2;
			}
			if(enumImageFloatWinButton!=EnumImageFloatWinButton.None){
				EventButClicked?.Invoke(this,enumImageFloatWinButton);
				//MessageBox.Show(this,enumImageFloatWinClick.ToString());//this doesn't work because this window closes
			}
			_isClosed=true;
			Close();//this is needed when the event moves a form to a new location, which doesn't cause a focus change
		}

		private void WindowImageFloatWindows_MouseMove(object sender,MouseEventArgs e) {
			SolidColorBrush solidColorBrushBack=new SolidColorBrush(_colorBack);
			SolidColorBrush solidColorBrushHover=new SolidColorBrush(_colorHover);
			Point point = e.GetPosition(_pathHalf_L);
			if(_pathHalf_L.Data.FillContains(point)){
				_pathHalf_L.Fill=solidColorBrushHover;
			}
			else{
				_pathHalf_L.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathHalf_R);
			if(_pathHalf_R.Data.FillContains(point)){
				_pathHalf_R.Fill=solidColorBrushHover;
			}
			else{
				_pathHalf_R.Fill=solidColorBrushBack;
			}
			//no mouse hover for max
			point = e.GetPosition(_pathCenter);
			if(_pathCenter.Data.FillContains(point)){
				_pathCenter.Fill=solidColorBrushHover;
			}
			else{
				_pathCenter.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_UL);
			if(_pathQuarter_UL.Data.FillContains(point)){
				_pathQuarter_UL.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_UL.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_UR);
			if(_pathQuarter_UR.Data.FillContains(point)){
				_pathQuarter_UR.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_UR.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_LL);
			if(_pathQuarter_LL.Data.FillContains(point)){
				_pathQuarter_LL.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_LL.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_LR);
			if(_pathQuarter_LR.Data.FillContains(point)){
				_pathQuarter_LR.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_LR.Fill=solidColorBrushBack;
			}
			if(_pathHalf_L2==null){
				return;
			}
			//second screen buttons===========================================================
			point = e.GetPosition(_pathHalf_L2);
			if(_pathHalf_L2.Data.FillContains(point)){
				_pathHalf_L2.Fill=solidColorBrushHover;
			}
			else{
				_pathHalf_L2.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathHalf_R2);
			if(_pathHalf_R2.Data.FillContains(point)){
				_pathHalf_R2.Fill=solidColorBrushHover;
			}
			else{
				_pathHalf_R2.Fill=solidColorBrushBack;
			}
			//no mouse hover for max
			point = e.GetPosition(_pathCenter2);
			if(_pathCenter2.Data.FillContains(point)){
				_pathCenter2.Fill=solidColorBrushHover;
			}
			else{
				_pathCenter2.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_UL2);
			if(_pathQuarter_UL2.Data.FillContains(point)){
				_pathQuarter_UL2.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_UL2.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_UR2);
			if(_pathQuarter_UR2.Data.FillContains(point)){
				_pathQuarter_UR2.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_UR2.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_LL2);
			if(_pathQuarter_LL2.Data.FillContains(point)){
				_pathQuarter_LL2.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_LL2.Fill=solidColorBrushBack;
			}
			point = e.GetPosition(_pathQuarter_LR2);
			if(_pathQuarter_LR2.Data.FillContains(point)){
				_pathQuarter_LR2.Fill=solidColorBrushHover;
			}
			else{
				_pathQuarter_LR2.Fill=solidColorBrushBack;
			}
		}

		#endregion Methods - Event Handlers

		#region Methods - private
		private void CreateCenteredLabel(double x,double y,double w,double h,string text){
			UI.Label label=new Label();
			label.Margin=new Thickness(x,y,0,0);
			label.Width=w;
			label.Height=h;
			label.HAlign=HorizontalAlignment.Center;
			label.VAlign=VerticalAlignment.Center;
			label.Text=text;
			grid.Children.Add(label);
		}

		private Path CreatePath(double x,double y,double w,double h,bool roundUL=false,bool roundUR=false,bool roundLL=false,bool roundLR=false){
			Rect rectPath=new Rect(0,0,w,h);
			Path path=GraphicsHelper.GetRoundedPathWpf(rectPath,5,roundUL,roundUR,roundLL,roundLR);
			path.Margin=new Thickness(x,y,0,0);
			path.HorizontalAlignment=HorizontalAlignment.Left;
			path.VerticalAlignment=VerticalAlignment.Top;
			path.Stroke=new SolidColorBrush(_colorOutline);
			path.StrokeThickness=1;
			path.Fill=new SolidColorBrush(_colorBack);
			grid.Children.Add(path);
			return path;
		}
		#endregion Methods - private

		
	}

	public enum EnumImageFloatWinButton{
		None,
		Minimize,
		Maximize,
		CloseOthers,
		DockThis,
		ShowAll,
		Half_L,
		Half_R,
		Center,
		Quarter_UL,
		Quarter_UR,
		Quarter_LL,
		Quarter_LR,
		Half_L2,
		Half_R2,
		Center2,
		Quarter_UL2,
		Quarter_UR2,
		Quarter_LL2,
		Quarter_LR2,
	}
}
