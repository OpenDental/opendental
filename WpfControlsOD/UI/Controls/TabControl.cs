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
/*
Jordan is the only one allowed to edit this file.

How to use the TabControl and TabPages:
-Each tabPage should contain a panel, like this:
	<ui:Panel HorizontalAlignment="Stretch" VerticalAlignment="Stretch" >
-TabIndexOD can be set, but it doesn't seem to work yet. Not sure why. You can still tab between textboxes within a tabcontrol once you get into it.

*/
	///<summary></summary>
	public class TabControl : System.Windows.Controls.TabControl{
		public TabControl(){
			ResourceDictionary resourceDictionary=new ResourceDictionary();
			Uri uri=new Uri(//"../../Themes/TabControlDict.xaml",UriKind.Relative);//couldn't get this to work
				"WpfControlsOD;component/Themes/TabControlDict.xaml",UriKind.RelativeOrAbsolute);
			resourceDictionary.Source=uri;
			//Resources=resourceDictionary;//this didn't work at all
			Style styleTabControl=resourceDictionary["TabControlStyleOD"] as Style;
			Style=styleTabControl;
			Style styleTabPage=resourceDictionary["TabPageStyleOD"] as Style;
			Resources.Add(typeof(TabPage),styleTabPage);
			KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);//See notes in FrmTestFocusTabbing.xaml.cs
			Focusable=false;
		}

		[Category("OD")]
		[DefaultValue(int.MaxValue)]
		[Description("Use this instead of TabIndex.")]
		public int TabIndexOD{
			get{
				return TabIndex;
			}
			set{
				TabIndex=value;
			}
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
	}

	
}



