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
	public class TabControl : System.Windows.Controls.TabControl{
		public TabControl(){
			ResourceDictionary resourceDictionary=new ResourceDictionary();
			Uri uri=new Uri("WpfControlsOD;component/Themes/TabControlDict.xaml",UriKind.RelativeOrAbsolute);
			resourceDictionary.Source=uri;
			//Resources=resourceDictionary;//this didn't work at all
			Style styleTabControl=resourceDictionary["TabControlStyleOD"] as Style;
			Style=styleTabControl;
			Style styleTabItem=resourceDictionary["TabItemStyleOD"] as Style;
			Resources.Add(typeof(TabItem),styleTabItem);
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



