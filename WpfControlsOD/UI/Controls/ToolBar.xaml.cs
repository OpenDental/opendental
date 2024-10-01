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
Height should be 25.
Typically anchor it L,T,R at the top of a window in the designer, just under the menu.  
We add all buttons in code, not in the designer.
Boilerplate for adding buttons:

using WpfControls.UI;
//using System.Windows.Controls;//Don't do this

		private void LayoutToolBar() {
			toolBarMain.Clear();//not usually necessary
			toolBarMain.Add(Lans.g(this,"Add"),Add_Click,EnumIcons.Add);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Edit"),Edit_Click,toolTipText:Lans.g(this,"Edit Selected Account"));
			ContextMenu contextMenu = new ContextMenu();
			contextMenu.Add(new MenuItem("Item1",menuItem1_Click));
			contextMenu.Add(new MenuItem("Item2",menuItem2_Click));
			contextMenu.Add(new MenuItem("Item3",menuItem3_Click));
			toolBarMain.Add(Lans.g(this,"Pick File"),toolBarButtonStyle:ToolBarButtonStyle.DropDownButton,contextMenuDropDown:contextMenu);
		}

Click event handlers look like this:
		private void Edit_Click(object sender,EventArgs e) { etc.

Notice that there is only one way to add an icon, unlike with buttons.
If you need a missing icon, Jordan will need to add it using the IconGenerator project.
WPF doesn't use anything like the WF ImageList, so all of those will go away during the conversion.
For button clicks, the old paradigm was one event for the entire toolbar.
The new way of doing it is a separate event for each button.

*/
		public ToolBar(){
			InitializeComponent();
			if(!DesignerProperties.GetIsInDesignMode(this)){
				stackPanel.Children.Clear();
				border.Background=Brushes.White;
			}
		}

		#region Properties


		#endregion Properties

		#region Methods

		public void Add(ToolBarButton toolBarButton){
			stackPanel.Children.Add(toolBarButton);
		}

		public void Add(string text,EventHandler eventHandlerClick=null,EnumIcons icon=EnumIcons.None,ToolBarButtonStyle toolBarButtonStyle=ToolBarButtonStyle.NormalButton,
			string toolTipText=null,ContextMenu contextMenuDropDown=null,object tag=null)
		{
			ToolBarButton toolBarButton = new ToolBarButton(text,eventHandlerClick,icon,toolBarButtonStyle,toolTipText,contextMenuDropDown,tag);
			stackPanel.Children.Add(toolBarButton);
		}

		//public void Add(string text,string bitmapFileName,EventHandler eventHandlerClick){
		//	ToolBarButton toolBarButton = new ToolBarButton();
		//	toolBarButton.Text=text;
		//	toolBarButton.BitmapFileName=bitmapFileName;
		//	toolBarButton.Click+=eventHandlerClick;
		//	stackPanel.Children.Add(toolBarButton);
		//}

		public void AddSeparator(){
			Rectangle rectangle=new Rectangle();
			rectangle.Width=7;
			rectangle.Height=24;
			rectangle.Fill=Brushes.White;
			stackPanel.Children.Add(rectangle);
		}
		public void Clear(){
			stackPanel.Children.Clear();
		}

		public void SetEnabled(string tag,bool boolVal){
			for(int i=0;i<stackPanel.Children.Count;i++){
				if(stackPanel.Children[i].GetType()!=typeof(ToolBarButton)){
					continue;
				}
				//We tried using enums for tags, but that fails because of equality comparison technical details.
				if(tag!=((ToolBarButton)stackPanel.Children[i]).Tag?.ToString()){
					continue;
				}
				stackPanel.Children[i].IsEnabled=boolVal;
				return;
			}
		}

		public void SetEnabledAll(bool boolVal){
			for(int i=0;i<stackPanel.Children.Count;i++){
				if(stackPanel.Children[i].GetType()!=typeof(ToolBarButton)){
					continue;
				}
				stackPanel.Children[i].IsEnabled=boolVal;
			}
		}
		#endregion Methods


	}

}
