using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls.UI{
	//For usage, see TabControl
	public class TabPage : System.Windows.Controls.TabItem{
		public TabPage(){
			//I'm not going to give tabPages their own TabIndex
			//KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);//already done for the TabControl
			Focusable=false;
		}
	}
}
