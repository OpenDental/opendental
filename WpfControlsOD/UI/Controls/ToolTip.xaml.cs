using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms.Integration;
using OpenDentBusiness;

namespace WpfControls.UI {
/*
Jordan is the only one allowed to edit this file.

Use like this:
private ToolTip toolTip;

//Constructor:
			_toolTip=new ToolTip();
			_toolTip.SetControlAndAction(textBox,ToolTipSetString);//Other controls could be passed in, including this.
			_toolTip.TimeSpanDelay=TimeSpan.FromSeconds(1);

		///<summary>Point should be relative to the control assigned to this tooltip. This is the action that runs from within the tooltip when the mouse moves. This only gets called when the mouse is inside the control.</summary>
		private void ToolTipSetString(FrameworkElement frameworkElement,Point point) {
			//The typical first step is to just show a msg.
			_toolTip.SetString(this,Lan.g(this,"A message to show when hovering"));
			//You can optionally show a different msg for different areas of your control, based on the point that was passed in.
			//And because FrameworkElement (control) is passed in, you can show different messages for different controls.
		}
*/
	///<summary>Kind of like the MS ToolTip, except it can come up instantly and it moves with the mouse. Something similar was also done in ToolTipOD and ControlApptPanel. See boilerplate at top of this file.</summary>
	public partial class ToolTip:Window {
		///<summary>If there's any delay, then the timer starts as soon as the user hovers, so don't make the delay too long. You can even make this extremely short like 10 ms in order to not show the tooltip as long as they are moving.</summary>
		public TimeSpan TimeSpanDelay;
		private Action<FrameworkElement, Point> _actionSetString;
		//private bool _isShowing;
		private DispatcherTimer _dispatcherTimer;
		///<summary>This must only get set from the FE passed in during SetString. This is so that one tooltip can be used for multiple simultaneous controls. Only used for the delay.</summary>
		private FrameworkElement _frameworkElement;

		public ToolTip() {
			InitializeComponent();
			
		}

		///<summary>This tooltip will respond to the mouseMove event of the control it's assigned to. The action should handle the point from the mouseMove with a hit test, and should then call SetString to show or hide the tooltip. An extremely simple action would just always return a certain string without any hit test.</summary>
		public void SetControlAndAction(FrameworkElement frameworkElement,Action<FrameworkElement, Point> actionSetString){
			_actionSetString=actionSetString;
			frameworkElement.MouseMove += control_MouseMove;
			frameworkElement.MouseLeave += control_MouseLeave;
			this.MouseMove+=ToolTip_MouseMove;
		}

		///<summary>This is intended to be called from within the Action that was previously set. The string can show immediately or will use the Delay value. If an empty string is passed in, then visibility will be false. pointInControl is required for non-empty string, and should be relative to the control attached to the tooltip.</summary>
		public void SetString(FrameworkElement frameworkElement, string stringToShow){//,Point? pointInControl=null){
			if(stringToShow==""){
				Visibility=Visibility.Collapsed;
				//_isShowing=false;
				return;
			}
			//if(pointInControl is null){
			//	return;
			//}
			if(frameworkElement.IsKeyboardFocusWithin){//Hide the tooltip when the user clicks on the textbox
				Visibility=Visibility.Collapsed;
				return;
			}
			textBlock.Text=stringToShow;
			double zoom=ComputerPrefs.LocalComputer.Zoom/100f;
			if(zoom==0){
				zoom=1;
			}
			textBlock.FontSize=11.5*zoom;
			//This isn't perfect. If I change zoom, then the font does change here immediately.
			//But 30 lines down, the window Width and Height does not change the first time.
			//It changes the second time. Rare edge case, that is too hard to fix right now.
			if(Visibility==Visibility.Visible || TimeSpanDelay==TimeSpan.Zero){
				//if already showing, change it immediately
				SetVisible(frameworkElement);
				return;
			}
			//was not showing and there's a delay
			//Note that this will get hit repeatedly if the user is moving mouse prior to timer tick.
			//This is good because the text won't show until they pause for a certain amount of time.
			if(_dispatcherTimer!=null){//timer has already started
				_dispatcherTimer.Stop();
				_dispatcherTimer.Start();
				return;
			}
			_frameworkElement=frameworkElement;
			_dispatcherTimer=new DispatcherTimer();
			_dispatcherTimer.Tick+=_dispatcherTimer_Tick;
			_dispatcherTimer.Interval=TimeSpanDelay;
			_dispatcherTimer.Start();
		}

		private void SetVisible(FrameworkElement frameworkElement){
			Visibility=Visibility.Visible;
			//_isShowing=true;
			Topmost = true;
			Activate();
			Topmost = false;
			PresentationSource presentationSource = PresentationSource.FromVisual(frameworkElement);
			double scaleWindows=presentationSource.CompositionTarget.TransformToDevice.M11;//example 1.5. For this screen only.
			//Automatic Window.SizeToContent wasn't working well enough on my 4k monitor. 
			//Also, the Max width of 200 is arbitrary. If we need more control, that could become a property.
			Height=textBlock.ActualHeight/scaleWindows;
			Width=textBlock.ActualWidth/scaleWindows;
			Point pointRelToElement=Mouse.GetPosition(frameworkElement);
			//The line below makes it easier to debug, but adds some complexity.
			//Point pointRelToElement=pointInControl.Value;
			Point pointDesktop=frameworkElement.PointToScreen(pointRelToElement);//this is actually desktop coordinates even though it's not well documented.
			//wpf controls do not have handles, and we must get a handle in order to figure out which screen we're on.
			//wpf controls are always inside an element host at some level because we wrap all Frms in a Windows Form called FormFrame,
			//or else we might sometimes nest a wpf control in an element host along with other WinForm controls.
			//There may be multiple levels of nesting, but probably not.
			//But in all cases, we can traverse up until we hit an element host.
			HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(frameworkElement);
			IntPtr intPtrHandle=hwndSource.Handle;
			//HwndHost hwndHost = hwndSource.RootVisual as HwndHost;//this could be a way to find the actual winform control that is the elementhost.
			//IntPtr intPtrHandle=hwndHost.Handle;
			//Window window=Window.GetWindow(_frameworkElement);//didn't work because no handle
			//IntPtr intPtrHandle=new WindowInteropHelper(window).Handle;
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromHandle(intPtrHandle);
			if(pointDesktop.Y+20+ActualHeight > screen.WorkingArea.Bottom){
				//would spill off bottom
				pointDesktop.Y-=ActualHeight;
			}
			else{
				pointDesktop.Y+=20;//puts it below the arrow
			}
			if(pointDesktop.X+ActualWidth > screen.WorkingArea.Right){
				//would spill off right of screen
				pointDesktop.X=screen.WorkingArea.Right-ActualWidth;
			}
			//Left and Top are the only way to set location,
			//but Windows expects them to be in DIPs, as explained in Dpi.cs
			Point pointDIP=new Point(pointDesktop.X/scaleWindows,pointDesktop.Y/scaleWindows);
			Left=pointDIP.X;
			Top=pointDIP.Y;
		}

		private void _dispatcherTimer_Tick(object sender,EventArgs e) {
			//The timer only ticks once and then it's done.
			//But continual moving will keep resetting the timer so that it doesn't tick until you've held still for a timespan.
			_dispatcherTimer.Stop();
			_dispatcherTimer=null;
			SetVisible(_frameworkElement);
		}

		private void control_MouseLeave(object sender, EventArgs e){
			if(_dispatcherTimer!=null){
				_dispatcherTimer.Stop();
				_dispatcherTimer=null;
			}
			if(Visibility!=Visibility.Visible){
				return;
			}
			FrameworkElement frameworkElement=(FrameworkElement)sender;
			Point pointRelToElement=Mouse.GetPosition(frameworkElement);
			Point pointDesktop=frameworkElement.PointToScreen(pointRelToElement);
			Point pointULDesktop=PointToScreen(new Point(0,0));
			Point pointLRDesktop=PointToScreen(new Point(ActualWidth,ActualHeight));
			Rect rect=new Rect(pointULDesktop,pointLRDesktop);
			if(rect.Contains(pointDesktop)){
				//left the controlAssigned, but is now within this tooltip
				//Calculate the real point in coordinates of the control assigned
				//Point pointInToolTip=this.PointToClient(pointDesktop);
				//Point pointInControl=new Point(pointInToolTip.X+Left,pointInToolTip.Y+Top);
				//Point pointInControl=_frameworkElement.PointFromScreen(pointDesktop);
				_actionSetString(frameworkElement,pointRelToElement);
				return;
			}
			//truly left
			Visibility=Visibility.Collapsed;
		}

		private void control_MouseMove(object sender, MouseEventArgs e){
			if(Mouse.LeftButton==MouseButtonState.Pressed){
				//we only show tooltip when in hover state, not mouse down.
				return;
			}
			if(sender is ComboBox comboBox){
				if(comboBox.IsExpanded){
					return;
				}
			}
			//This can fire repeatedly as user moves mouse.
			//The timer delay is built in to the action.
			FrameworkElement frameworkElement=(FrameworkElement)sender;
			_actionSetString(frameworkElement,e.GetPosition(frameworkElement));
		}
		
		private void ToolTip_MouseMove(object sender,MouseEventArgs e) {
			FrameworkElement frameworkElement=(FrameworkElement)sender;
			Point pointRelToElement=Mouse.GetPosition(frameworkElement);
			Rect rect=new Rect(new Point(0,0),new Point(frameworkElement.ActualWidth,frameworkElement.ActualHeight));
			if(rect.Contains(pointRelToElement)){
				//would be still inside the control, so continue to show the tooltip.
				_actionSetString(frameworkElement,pointRelToElement);
				return;
			}
			//within this tooltip, but would be outside the attached control if the tooltip was not present.
			Visibility=Visibility.Collapsed;
		}		
	}
}
