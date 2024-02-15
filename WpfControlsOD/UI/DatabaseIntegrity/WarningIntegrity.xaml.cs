using OpenDental;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenDentBusiness;

namespace WpfControls.UI{
	/*
Jordan is the only one allowed to edit this file.

*/
	///<summary></summary>
	public partial class WarningIntegrity : UserControl{
		///<summary></summary>
		private bool _didShowPopup;
		///<summary>Interior of triangle.</summary>
		private Color _color;
		private DispatcherTimer _timerTriangle;
		private DispatcherTimer _timerPopup;
		private ToolTip _toolTip;
		private DatabaseIntegrity _databaseIntegrity;
		
		public WarningIntegrity(){
			InitializeComponent();
			this.SizeChanged+=this_SizeChanged;
			///255, 128, 0 is pure orange
			_color=Color.FromArgb(255,255,192,128);
			_toolTip=new ToolTip();
			_toolTip.SetControlAndAction(this,ToolTipSetString);
			path.MouseDown+=Path_MouseDown;
			this.Loaded+=this_Loaded;
		}

		private void this_Loaded(object sender,RoutedEventArgs e) {
			ShowPopup();
		}

		private void Path_MouseDown(object sender,MouseButtonEventArgs e) {
			if(_databaseIntegrity is null){
				return;//probably an HQ connection issue, but this probably wouldn't fire anyway.
			}
			FrmDatabaseIntegrity frmDatabaseIntegrity=new FrmDatabaseIntegrity();
			frmDatabaseIntegrity.MessageToShow=_databaseIntegrity.Message.Replace("[Class]",_databaseIntegrity.WarningIntegrityType.ToString());
			frmDatabaseIntegrity.ShowDialog();
		}

		private void this_SizeChanged(object sender,SizeChangedEventArgs e) {
			double ratio=Width/100d;
			ScaleTransform scaleTransform=new ScaleTransform(ratio,ratio);
			canvas.RenderTransform=scaleTransform;
		}

		///<summary>Defines how the triangle will appear based on the EnumIntegrity behavior. Sets and starts a timer for behaviors with dynamic triangles.</summary>
		private void SetTriangleBehavior() {
			int interval;
			if(_databaseIntegrity.Behavior==EnumIntegrityBehavior.TrianglePulse) {
				interval=100;
			}
			else if(_databaseIntegrity.Behavior==EnumIntegrityBehavior.TriangleBlinkSlow) {
				interval=1000;
			}
			else if(_databaseIntegrity.Behavior==EnumIntegrityBehavior.TriangleBlinkFast) {
				interval=500;
			}
			else { //all other behaviors have a static triangle
				return;
			}
			_timerTriangle=new DispatcherTimer();
			_timerTriangle.Interval=TimeSpan.FromMilliseconds(interval);
			_timerTriangle.IsEnabled=true;
			_timerTriangle.Tick+=TimerTriangle_Tick;
		}

		public void SetTypeAndVisibility(EnumWarningIntegrityType warningIntegrityType,bool isHashValid){
			if(isHashValid){
				Visibility=Visibility.Hidden;
				return;
			}
			//We have an invalid hash and we are going to show a triangle. Let's see how aggressive we will get.
			//For testing, GetOneClass is where we set up a dummy object.
			DatabaseIntegrity databaseIntegrity=DatabaseIntegrities.GetOneClass(warningIntegrityType);
			if(databaseIntegrity is null){
				//connection to HQ failed, so just don't show warnings.
				Visibility=Visibility.Hidden;
				return;
			}
			_databaseIntegrity=new DatabaseIntegrity();
			_databaseIntegrity.WarningIntegrityType=warningIntegrityType; //In case of DefaultClass, overwrite with this one
			_databaseIntegrity.Behavior=databaseIntegrity.Behavior;
			_databaseIntegrity.Message=databaseIntegrity.Message;
			//for both popup and triangle, we will show this triangle
			Visibility=Visibility.Visible;
			if(_databaseIntegrity.Behavior==EnumIntegrityBehavior.TriangleRed) {
				_color=Colors.OrangeRed;
			}
			SetTriangleBehavior();
		}

		private void ShowPopup(){
			if(_didShowPopup){
				return;//so that we just show it once
			}
			_didShowPopup=true;
			if(Visibility!=Visibility.Visible){
				return;
			}
			//this timer is no longer necessary in WPF, but it's harmless to leave it.
			_timerPopup=new DispatcherTimer();
			_timerPopup.Interval=TimeSpan.FromMilliseconds(100);
			_timerPopup.Tick+=TimerPopup_Tick;
			_timerPopup.IsEnabled=true;
		}

		private void TimerPopup_Tick(object sender,EventArgs e) {
			_timerPopup.IsEnabled=false;//only happens once
			if(_databaseIntegrity is null){
				return;//probably an HQ connection issue, but this probably wouldn't fire anyway.
			}
			if(_databaseIntegrity.Behavior!=EnumIntegrityBehavior.Popup){
				return;
			}
			FrmDatabaseIntegrity frmDatabaseIntegrity=new FrmDatabaseIntegrity();
			frmDatabaseIntegrity.MessageToShow=_databaseIntegrity.Message.Replace("[Class]",_databaseIntegrity.WarningIntegrityType.ToString());
			frmDatabaseIntegrity.IsPlugin=true;
			frmDatabaseIntegrity.ShowDialog();
		}

		///<summary>Changes the alpha value (transparency) for triangles with dynamic behaviors. Only effects interior region; the border of the triangle can always be seen. This makes the tooltip always available to the user.</summary>
		private void TimerTriangle_Tick(object sender, EventArgs e) {
			int alpha;
			if(_databaseIntegrity.Behavior==EnumIntegrityBehavior.TrianglePulse) {
				if(_color.A%2==0) { //If even, alpha needs to increment
					alpha=_color.A+20; 
				}
				else { //otherwise decrement
					alpha=_color.A-20;
				}
				alpha=Math.Min(255,alpha); //No larger than 255
				alpha=Math.Max(100,alpha); //No smaller than 100 (still paritially visible)
			}
			else {//Otherwise blink
				alpha=Math.Abs(_color.A-255); //either zero(min) or 255(max)
			}
			_color=Color.FromArgb((byte)alpha,_color.R,_color.G,_color.B);
			path.Fill=new SolidColorBrush(_color);
		}

		private void ToolTipSetString(FrameworkElement frameworkElement, Point point) {
			PathGeometry pathGeometry=(PathGeometry)path.Data;
			if(pathGeometry.FillContains(point)){
				_toolTip.SetString(this,Lang.g(this,"Click to learn about Database Integrity"));
				return;
			}
			_toolTip.SetString(this,"");
		}


		}
	}
