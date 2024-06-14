using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using WpfControls.UI;

namespace OpenDental {
	///<summary>This control is always present in the Imaging module. It's a container with a header that looks like the title bar of a window. It contains a ControlImageDisplay to show docked images/mounts. When there is no docked image, then ControlImageDisplay=null and this shows as all white with no title bar as an indicator to the user.</summary>
	public partial class ControlImageDock:UserControl {
		#region Fields - public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>Will be null if no image showing.</summary>
		public ControlImageDisplay ControlImageDisplay_=null;
		///<summary>This lets us get a list of all floater windows from ControlImages at the moment when we pop up the window selector.</summary>
		public Func<List<string>> FuncListFloaters;
		#endregion Fields - public

		#region Fields - private
		private DispatcherTimer _dispatcherTimer;
		private WpfControls.UI.ImageDockHeader imageDockHeader;
		private bool _isClickLocked;
		///<summary>Property backer.</summary>
		private bool _isImageFloatSelected;
		///<summary>Gets reused.</summary>
		private WindowImageFloatWindows _windowImageFloatWindows;
		#endregion Fields - private

		#region Constructor
		public ControlImageDock() {
			InitializeComponent();
			imageDockHeader=new WpfControls.UI.ImageDockHeader();
			elementHostImageDockHeader.Child=imageDockHeader;
			imageDockHeader.Close+=imageDockHeader_Close;
			imageDockHeader.Min+=imageDockHeader_Min;
			imageDockHeader.Max+=imageDockHeader_Max;
			imageDockHeader.Win+=imageDockHeader_Win;
		}
		#endregion Constructor

		#region Events
		///<summary>A button was clicked in WindowImageFloatWindows or ImageDockHeader. This event must bubble up to ControlImages where it's handled.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EnumImageFloatWinButton> EventButClicked=null;
		#endregion Events

		#region Properties
		///<summary>This is analogous to FormODBase.IsImageFloatSelected. True if this control is the "selected" image.  Only one should be selected at a time.  Any image that is not selected will have the title bar turn white to indicate so. When ControlImageDisplay is set to null, this gets set to false.</summary>
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
				imageDockHeader.IsSelected=_isImageFloatSelected;
			}
		}

		///<summary>This is what shows in the title bar.</summary>
		public string Text{
			get{
				return imageDockHeader.Text;
			}
			set{
				imageDockHeader.Text=value;
			}
		}
		#endregion Properties

		#region Methods - Public
		///<summary>Pass in null to clear out image.</summary>
		public void SetControlImageDisplay(ControlImageDisplay controlImageDisplay){
			//Can get called repeatedly.
			if(ControlImageDisplay_!=null && ControlImageDisplay_.Parent==this){
				//still here. Wasn't popped out to a floater
				ControlImageDisplay_?.Dispose();
			}
			ControlImageDisplay_=controlImageDisplay;//works for null
			if(controlImageDisplay is null){
				IsImageFloatSelected=false;
				imageDockHeader.IsEmpty=true;
				return;
			}
			imageDockHeader.IsEmpty=false;
			ControlImageDisplay_.Size=ClientRectangle.Size;
			ControlImageDisplay_.Dock=DockStyle.Fill;
			LayoutManager.Add(ControlImageDisplay_,this);
			LayoutManager.LayoutControlBoundsAndFonts(ControlImageDisplay_);
		}
		#endregion Methods - Public

		#region Methods - private
		private void panelHeader_Paint(object sender,PaintEventArgs e) {
			//todo
		}
		#endregion Methods - private

		#region Methods - private event handlers
		private void dispatcherTimer_Tick(object sender,EventArgs e) {
			_dispatcherTimer.Stop();
			_isClickLocked=false;
		}

		private void imageDockHeader_Close(object sender,EventArgs e) {
			if(ControlImageDisplay_ is null){
				return;
			}
			ControlImageDisplay_.OnFillTree(keepSelection:false);
			SetControlImageDisplay(null);
		}

		private void imageDockHeader_Min(object sender,EventArgs e) {
			if(ControlImageDisplay_ is null){
				return;
			}
			ControlImageDisplay_.OnLaunchFloater();
			EventButClicked?.Invoke(this,EnumImageFloatWinButton.Minimize);
		}

		private void imageDockHeader_Max(object sender,EventArgs e) {
			if(ControlImageDisplay_ is null){
				return;
			}
			ControlImageDisplay_.OnLaunchFloater();
			EventButClicked?.Invoke(this,EnumImageFloatWinButton.Maximize);
		}

		private void imageDockHeader_Win(object sender,EventArgs e) {
			if(ControlImageDisplay_ is null){
				return;
			}
			if(_isClickLocked){
				return;
			}
			if(imageDockHeader.IsButWinPressed){
				imageDockHeader.IsButWinPressed=false;
				return;
			}
			imageDockHeader.IsButWinPressed=true;
			_windowImageFloatWindows=new WindowImageFloatWindows();
			_windowImageFloatWindows.EventButClicked+=(sender,enumImageFloatWinButton)=> EventButClicked?.Invoke(this,enumImageFloatWinButton);
			_windowImageFloatWindows.Closed+=_windowImageFloatWindows_Closed;
			//Bottom left and right of the button, in screen coords.
			//Hard code these for now instead of deriving them from imageDockHeader.
			//Our button has right corner at 99 from right, and left corner 159 from right
			//Y is 23 from top of this control
			Point pointLScaled=new Point(Width-LayoutManager.Scale(159),LayoutManager.Scale(23-4));
			Point pointL=PointToScreen(pointLScaled);
			Point pointR=PointToScreen(new Point(Width-LayoutManager.Scale(99),LayoutManager.Scale(23-4)));
			System.Windows.Point win_PointL=new System.Windows.Point(pointL.X,pointL.Y);
			System.Windows.Point win_PointR=new System.Windows.Point(pointR.X,pointR.Y);
			_windowImageFloatWindows.PointAnchor1=win_PointL;
			_windowImageFloatWindows.PointAnchor2=win_PointR;
			_windowImageFloatWindows.Show();//not a dialog.  They can click elsewhere
		}

		private void _windowImageFloatWindows_Closed(object sender,EventArgs e) {
			imageDockHeader.IsButWinPressed=false;
			//if they clicked on the button to close, prevent that same click from opening the window back up again.
			_isClickLocked=true;
			_dispatcherTimer=new DispatcherTimer();
			_dispatcherTimer.Interval=TimeSpan.FromMilliseconds(300);
			_dispatcherTimer.Tick+=dispatcherTimer_Tick;
			_dispatcherTimer.Start();
		}

		


		#endregion Methods - private event handlers
	}
}
