using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls.UI{
	///<summary></summary>
	public partial class ToolBar : System.Windows.Controls.UserControl{
/*
Jordan is the only one allowed to edit this file.
Typically anchor it L,T,R at the top of a window in the designer, just under the menu.  
We add all buttons in code, not in the designer.
Boilerplate for adding buttons:

using WpfControls.UI;
//using System.Windows.Controls;//Don't do this

		private void LayoutToolBar() {
			toolBarMain.Clear();//not usually necessary
			toolBarMain.Add(Lans.g(this,"Add"),EnumIcons.Add,Add_Click);
			toolBarMain.AddSeparator();
			ToolBarButton toolBarButton=new ToolBarButton();
			toolBarButton.Text=Lans.g(this,"Edit");
			toolBarButton.ToolTipText=Lans.g(this,"Edit Selected Account");
			toolBarButton.Style=ToolBarButtonStyle.DropDownButton;
			toolBarButton.Click+=Edit_Click;
			toolBarMain.Add(toolBarButton);
			toolBarMain.Add(Lans.g(this,"Info"),"editPencil.gif",Info_Click);
		}

Click event handlers look like this:
		private void Edit_Click(object sender,EventArgs e) { etc.

Notice that there are only 2 ways to add images. Examples of each are above.
1. use an EnumIcon if it has the one you need.
2. otherwise, save a bitmap (usually a png) to WpfControlsOD/Resources as follows:
		a. Right click WpfControlsOD, Properties.
		b. Resources tab, Add Resource, Add Existing File.
		c. The file you want is usually in OpenDental/Resources. Selecting it will make a copy, which is what you want.
		d. In Solution Explorer, Resources folder, find the new file..
		e. Right click, Properties, Build Action: Resource.
WPF doesn't use anything like the WF ImageList, so all of those will go away during the conversion.
For button clicks, the old paradigm was one event for the entire toolbar.
The new way of doing it is a separate event for each button.

*/
		public ToolBar(){
			InitializeComponent();
		}

		#region Properties


		#endregion Properties

		#region Methods
		public void Clear(){
			toolBar.Items.Clear();
		}

		public void Add(ToolBarButton toolBarButton){
			toolBar.Items.Add(toolBarButton);
		}

		public void Add(string text,EnumIcons icon,EventHandler eventHandlerClick){
			ToolBarButton toolBarButton=new ToolBarButton();
			toolBarButton.Text=text;
			toolBarButton.Icon=icon;
			toolBarButton.Click+=eventHandlerClick;
			toolBar.Items.Add(toolBarButton);
		}

		public void Add(string text,string bitmapFileName,EventHandler eventHandlerClick){
			ToolBarButton toolBarButton=new ToolBarButton();
			toolBarButton.Text=text;
			toolBarButton.BitmapFileName=bitmapFileName;
			toolBarButton.Click+=eventHandlerClick;
			toolBar.Items.Add(toolBarButton);
		}

		public void AddSeparator(){

		}
		#endregion Methods


	}

}
