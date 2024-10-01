using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls.UI{
	//For usage, see TabControl
	public class TabPage : System.Windows.Controls.TabItem{
		public TabPage(){
			//I'm not going to give tabPages their own TabIndex
			//KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);//already done for the TabControl
			//Focusable=false;//no, this would prevent clicking on the header to select.
		}

		#region Properties
		[Category("OD")]
		public string Text {
			get {
				return Header.ToString();
			}
			set {
				Header=value;
			}
		}
		#endregion Properties
	}
}
