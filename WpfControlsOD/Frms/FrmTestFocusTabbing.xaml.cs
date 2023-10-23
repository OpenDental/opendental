using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmTestFocusTabbing:FrmODBase {

		///<summary></summary>
		public FrmTestFocusTabbing() {
			InitializeComponent();
			GotKeyboardFocus+=FrmTestFocusTabbing_GotKeyboardFocus;
			LostKeyboardFocus+=FrmTestFocusTabbing_LostKeyboardFocus;
			PreviewKeyDown+=FrmTestFocusTabbing_PreviewKeyDown;
		}

		private void FrmTestFocusTabbing_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(e.SystemKey==Key.S && Keyboard.Modifiers==ModifierKeys.Alt) {
				MsgBox.Show("Alt-S");
			}
		}

		private void FrmTestFocusTabbing_LostKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//string focusedElement="null";
			//if(Keyboard.FocusedElement!=null){
			//	focusedElement=Keyboard.FocusedElement.ToString();
			//}
			//if(Keyboard.FocusedElement is WpfControls.UI.TextBox textBox){
			//	Debug.WriteLine("LOST KBFocus: Frm. Current focused element: "+textBox.Name);
			//	return;
			//}
			//Debug.WriteLine("LOST KBFocus: Frm. Current focused element: "+focusedElement);
		}

		private void FrmTestFocusTabbing_GotKeyboardFocus(object sender,KeyboardFocusChangedEventArgs e) {
			//Debug.WriteLine("GOT KBFocus: Frm.");
		}

		private void FrmTestFocusTabbing_Loaded(object sender,RoutedEventArgs e) {
			//For the following line to work, UI.TextBox UserControl must have Focusable set to true,
			//usually in the constructor of UI.Textbox or any other control we are interested in.
			//This does not necessarily set the WPF TextBox inside of it without some additional work.
			//Debug.WriteLine("Frm loaded------------------------------------------------------------------------------------------");
			//bool isKBFocused=
			//Focusable=true;//this does work to allow key strokes like Esc and Alt-S to function if there are no focusable controls.
			//textBox1.Focus();
			//if(isKBFocused){
			//	Debug.WriteLine("Setting focus in LOADED was successful.");
			//}
			//else{
			//	Debug.WriteLine("Setting focus in LOADED failed.");
			//}
			//This is also causing textBox1.GotKeyboardFocus to fire twice.
			//FocusManager.SetFocusedElement(this,textBox1);//this might be another option
			//IInputElement iInputElement=Keyboard.FocusedElement;
			//bool isFocused=textBox6.Focus();//this does work, even though result is false
			//iInputElement=Keyboard.FocusedElement;
		}

		private void Button_Click(object sender,EventArgs e) {
			MsgBox.Show("Clicked");
		}
	}
}



/*
More notes:
There's a huge amount of complex nesting in WPF.
TabIndex, as far as I can tell, is global by default.
This means that we can set the tab order of deeply nested controls and they will still be considered.
I think we can limit the scope of the tab order by doing this:
KeyboardNavigation.SetTabNavigation(this,KeyboardNavigationMode.Local);//Default is Continue, which I think means global.
I've put that in the constructor of individual controls like GroupBox and Panel.


*/

