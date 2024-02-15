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

namespace WpfControls.UI{
	/*
Jordan is the only one allowed to edit this file.

How to use the CheckBox control:
-Height should be about 20
-MS has no CheckChanged event, so we don't either.  Use Click.
-If disabled, then Click event won't fire. (still researching this, and we may need to let it fire anyway in some cases)
-We may need to distinguish between mouse events on the box vs text.
-Still need to build hover effect.
-Still need to look into enabled and autocheck.
-Click event handlers usually look like this:
		private void butEdit_Click(object sender,EventArgs e) { etc.

*/
	///<summary></summary>
	public partial class RadioButton : UserControl{
		private EnumCheckAlign _checkAlign;
		private bool _checked=false;
		
		public RadioButton(){
			InitializeComponent();
			//Width=100;
			//Height=20;
			grid.MouseDown+=grid_MouseDown;
			IsEnabledChanged+=RadioButton_IsEnabledChanged;
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
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//ellipse
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(border,0);
					System.Windows.Controls.Grid.SetColumn(textBlock,1);
					textBlock.TextAlignment=TextAlignment.Left;
					border.VerticalAlignment=VerticalAlignment.Top;
					textBlock.VerticalAlignment=VerticalAlignment.Top;
				}
				if(_checkAlign==EnumCheckAlign.MiddleLeft){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//ellipse
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(border,0);
					System.Windows.Controls.Grid.SetColumn(textBlock,1);
					textBlock.TextAlignment=TextAlignment.Left;
					border.VerticalAlignment=VerticalAlignment.Center;
					textBlock.VerticalAlignment=VerticalAlignment.Center;
				}
				if(_checkAlign==EnumCheckAlign.MiddleRight){
					grid.ColumnDefinitions.Clear();
					ColumnDefinition columnDefinition;
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
					grid.ColumnDefinitions.Add(columnDefinition);
					columnDefinition=new ColumnDefinition();
					columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//ellipse
					grid.ColumnDefinitions.Add(columnDefinition);
					System.Windows.Controls.Grid.SetColumn(border,1);
					System.Windows.Controls.Grid.SetColumn(textBlock,0);
					textBlock.TextAlignment=TextAlignment.Right;
					border.VerticalAlignment=VerticalAlignment.Center;
					textBlock.VerticalAlignment=VerticalAlignment.Center;
				}
			}
		}

		[Category("OD")]
		[Description("")]
		[DefaultValue(false)]
		public bool Checked {
			//for convenience, named same as WinForms version.
			get{
				return _checked;
			}
			set{
				if(_checked==value){
					return; //no change
				}
				_checked=value;
				if(!value){//unchecking. This can only happen programmatically
					ellipseSmallBlack.Visibility=Visibility.Collapsed;
					return;
				} 
				//everything below here is for checking it.
				ellipseSmallBlack.Visibility=Visibility.Visible;
				//uncheck all other radio buttons that are siblings
				if(DesignerProperties.GetIsInDesignMode(this)){
					return;
				}
				//List<RadioButton> listRadioButtons=new List<RadioButton>();
				//Type typeParent=Parent.GetType();//UI.Panel, UI.GroupBox, or UserControl
				//Type typeBase=typeParent.BaseType;//ItemsControl
				//Type typeBase2=typeBase.BaseType;//FrameworkElement
				if(Parent is ItemsControl itemsControl){//UI.Panel or UI.GroupBox
					ItemCollection itemCollection=itemsControl.Items;
					for(int i=0;i<itemCollection.Count;i++){
						if(itemCollection[i]==this){
							continue;
						}
						if(itemCollection[i] is RadioButton radioButton){
							radioButton.Checked=false;
						}
					}
					return;
				}
				if(Parent is System.Windows.Controls.Grid grid){
					//grids are always used as the content of FrmODBase, and this code will make it work for all grids
					UIElementCollection uIElementCollection=grid.Children;
					for(int i=0;i<uIElementCollection.Count;i++){
						if(uIElementCollection[i]==this){
							continue;
						}
						if(uIElementCollection[i] is RadioButton radioButton){
							radioButton.Checked=false;
						}
					}
				}
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
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			//For now, this is just to move it down into the OD category,
			//but later, there are plans to enhance it.
			//Because TabIndex is an Advanced Property, we had to give it a new name to keep it out of Advanced Property area.
			get{
				return TabIndex;
			}
			set{
				TabIndex=value;
				//InvalidateVisual();//if we want the new TabIndex value to show immediately. But there's a performance hit, so no.
			}
		}
		
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

		private void grid_MouseDown(object sender,MouseButtonEventArgs e) {
			Checked=true;//this also unchecks any other sibling radiobuttons
			Click?.Invoke(this,new EventArgs());
		}

		private void RadioButton_IsEnabledChanged(object sender,DependencyPropertyChangedEventArgs e) {
			//This is nice because it gets hit when changing the property in the designer.
			SetColors();
		}

		private void SetColors(){
			if(IsEnabled){
				ellipseOutline.Stroke=new SolidColorBrush(Color.FromRgb(111,111,111));//6F6F6F
				textBlock.Foreground=Brushes.Black;//not hit very often. Usually black because of default.
			}
			else{
				ellipseOutline.Stroke=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
				textBlock.Foreground=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(170));
			}
		}
	}
}
