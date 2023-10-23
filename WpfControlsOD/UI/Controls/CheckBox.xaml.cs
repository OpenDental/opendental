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

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the CheckBox control:
-Height should be about 20
-Since Checked is a nullable, you have to explicitly compare like this: Checked==true or Checked==false, not Checked.Value, or (bool)Checked
-MS has no CheckChanged event, so we don't either.  Use Click.
-If disabled, then Click event won't fire. (still researching this, and we may need to let it fire anyway in some cases)
-We may need to distinguish between mouse events on the box vs text.
-Still need to build hover effect.
-Still need to look into enabled and autocheck.
-Click event handlers usually look like this:
		private void butEdit_Click(object sender,EventArgs e) { etc.

*/
	///<summary></summary>
	public partial class CheckBox : UserControl{
		private EnumCheckAlign _checkAlign;
		private bool? _checked=false;
		private bool _isHover=false;
		private bool _isThreeState=false;
		
		public CheckBox(){
			InitializeComponent();
			//Width=100;
			//Height=20;
			grid.MouseDown+=grid_MouseDown;
			grid.MouseMove+=Grid_MouseMove;
			grid.MouseLeave+=Grid_MouseLeave;
			IsEnabledChanged+=CheckBox_IsEnabledChanged;
		}

		public event EventHandler Click;

		#region Properties
		[Category("OD")]
		[Description("")]
		[DefaultValue(EnumCheckAlign.MiddleLeft)]
		public EnumCheckAlign CheckAlign{
			get{
				return _checkAlign;
			}
			set{
				_checkAlign=value;
				if(_checkAlign==EnumCheckAlign.TopLeft){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//box
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(border,0);
					System.Windows.Controls.Grid.SetColumn(textBlock,1);
					textBlock.TextAlignment=TextAlignment.Left;
					textBlock.HorizontalAlignment=HorizontalAlignment.Left;
					border.VerticalAlignment=VerticalAlignment.Top;
					textBlock.VerticalAlignment=VerticalAlignment.Top;
					textBlock.Padding=new Thickness(5,0,0,0);
				}
				if(_checkAlign==EnumCheckAlign.MiddleLeft){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//box
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(border,0);
					System.Windows.Controls.Grid.SetColumn(textBlock,1);
					textBlock.TextAlignment=TextAlignment.Left;
					textBlock.HorizontalAlignment=HorizontalAlignment.Left;
					border.VerticalAlignment=VerticalAlignment.Center;
					textBlock.VerticalAlignment=VerticalAlignment.Center;
					textBlock.Padding=new Thickness(5,0,0,0);
				}
				if(_checkAlign==EnumCheckAlign.MiddleRight){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//box
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(border,1);
					System.Windows.Controls.Grid.SetColumn(textBlock,0);
					textBlock.TextAlignment=TextAlignment.Right;
					textBlock.HorizontalAlignment=HorizontalAlignment.Right;
					border.VerticalAlignment=VerticalAlignment.Center;
					textBlock.VerticalAlignment=VerticalAlignment.Center;
					textBlock.Padding=new Thickness(0,0,5,0);
				}
			}
		}

		///<summary>Since it's a nullable, you have to explicitly compare like this: Checked==true or Checked==false, not Checked.Value, or (bool)Checked.</summary>
		[Category("OD")]
		[Description("Since it's a nullable, you have to explicitly compare like this: Checked==true or Checked==false, not Checked.Value, or (bool)Checked.")]
		[DefaultValue(false)]
		public bool? Checked {
			//for convenience, named same as WinForms version.
			get{
				return _checked;
			}
			set{
				_checked=value;
				SetCheckVis();
			}
		}

		[Category("OD")]
		[DefaultValue(true)]
		public new bool IsEnabled{
			//This doesn't actually ever get hit. 
			//It's just here to move IsEnabled down into the OD category.
			get{
				return base.IsEnabled;
			}
			set{
				base.IsEnabled=value;
			}
		}

		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool IsThreeState {
			get{
				return _isThreeState;
			}
			set{
				_isThreeState=value;
				//MS doesn't enforce anything here.
				//If in 3rd state, and we set to 2 state, Winforms control allows it to stay in 3rd state until clicked.
				//I didn't check the WPF behavior and it doesn't really matter.
				//We won't allow the above scenario.
				if(!_isThreeState && Checked==null){
					_checked=false;
				}
				SetCheckVis();
			}
		}

		//[Category("OD")]
		//[DefaultValue(int.MaxValue)]
		//[Description("Use this instead of TabIndex.")]
		//public int TabIndexOD{
			 //TabIndex is just for textboxes for now.
		//}
		
		[Category("OD")]
		public string Text {
			get =>textBlock.Text;
			set=>textBlock.Text=value;
		}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}
		#endregion Properties

		private void CheckBox_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}
		
		private void Grid_MouseLeave(object sender,MouseEventArgs e) {
			_isHover=false;
			SetColors();
		}

		private void grid_MouseDown(object sender,MouseButtonEventArgs e) {
			//This does not actually fire unless a control inside the grid is clicked.
			//order is unchecked-checked-indeterm
			if(Checked==false){
				Checked=true;
			}
			else if(Checked==null){
				Checked=false;
			}
			//Order is important here because Indeterminate is also checked
			else if(Checked==true){
				if(IsThreeState){
					Checked=null;
				}
				else{
					Checked=false;
				}
			}
			Click?.Invoke(this,new EventArgs());
		}

		private void Grid_MouseMove(object sender,MouseEventArgs e) {
			_isHover=true;
			SetColors();
		}

		private void SetCheckVis(){
			polylineCheck.Visibility=Visibility.Collapsed;
			rectangleIndeterm.Visibility=Visibility.Collapsed;
			if(_isThreeState){
				if(_checked==null){
					rectangleIndeterm.Visibility=Visibility.Visible;
					return;
				}
			}
			//regardless of 3-state:
			if(_checked==true){
				polylineCheck.Visibility=Visibility.Visible;
			}
		}

		private void SetColors(){
			if(_isHover){
				border.Background=new SolidColorBrush(Color.FromRgb(210,239,255));
			}
			else{
				border.Background=Brushes.White;
			}
			if(IsEnabled){
				textBlock.Foreground=Brushes.Black;//not hit very often. Usually black because of default.
				border.BorderBrush=new SolidColorBrush(Color.FromRgb(111,111,111));//#FF6F6F6F
			}
			else{
				textBlock.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
				border.BorderBrush=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
			}
		}
	}

	public enum EnumCheckAlign{
		TopLeft,
		MiddleLeft,
		MiddleRight
	}
}
