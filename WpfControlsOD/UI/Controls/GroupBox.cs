using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls.UI{
	///<summary></summary>
	public class GroupBox : ItemsControl{		
		static GroupBox(){
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(typeof(GroupBox)));
			//WidthProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(200.0));
			//HeightProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(200.0));
		}

		public GroupBox(){
			KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);//See notes in FrmTestFocusTabbing.xaml.cs
			Focusable=false;
		}
		//https://stackoverflow.com/questions/9094486/adding-children-to-usercontrol
		//Worked fine, but it wouldn't let me set Name property on child controls of UserControl
		//https://www.codeproject.com/Articles/1056014/WPF-Lookless-Controls
		//the above is not very useful. Don't know why it's there.
		//Children = PART_Host.Children;
		//}
		//The better way to do it is entirely in C# (although I couldn't duplicate this exact strategy):
		//https://social.msdn.microsoft.com/Forums/vstudio/en-US/7e106c07-be99-402a-bb2a-fa4637574b32/how-to-create-a-wpf-usercontrol-with-named-content?forum=wpf
		//Other places also discuss how using XAML creates a namescope, and that's my problem.
		//So just don't use XAML.  The controltemplate still seems to not create a namescope. 

		[Category("OD")]
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			get{
				return TabIndex;
			}
			set{
				TabIndex=value;
				//InvalidateVisual();//if we want the new TabIndex value to show immediately. But there's a performance hit, so no.
			}
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text",typeof(string),ownerType: typeof(GroupBox),new FrameworkPropertyMetadata(""));

		[Category("OD")]
		public string Text {
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty,value);
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
		//#endregion Properties

		protected override int VisualChildrenCount {
			get {
				return base.VisualChildrenCount;
			}
		}

		protected override Visual GetVisualChild(int index) {
			return base.GetVisualChild(index);
		}


	}

	
}



