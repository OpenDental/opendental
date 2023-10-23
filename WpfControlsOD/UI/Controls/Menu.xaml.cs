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
	public partial class Menu : System.Windows.Controls.UserControl{
/*
Jordan is the only one allowed to edit this file.
Typically anchor it L,T,R at the top of a window in the designer. 
Height is 24.
Unlike the MS menu, we don't add menu items in the designer.
You can't add a ContextMenu in the designer. Do it in the constructor.
Instead of ContextMenu.Popup event, use ContextMenu_Opened.
Instead of MenuItem.Enabled, use MenuItem.IsEnabled.
Instead of Visibile false, use Visibility.Collapsed.
If you add a separator, use the actual Separator object instead of a hyphen.
Instead of Menu.MenuItems, use Menu.Items.
Instead of Items.Add(), use Add().
There's an example of how to create and use a ContextMenu in UI.TextRich.xaml.cs. 
It's overly complex because we are changing visibility on menuItems, but it's something.
Boilerplate for adding menu items:

using WpfControls.UI;
//using System.Windows.Controls;//Don't do this

		private void LayoutMenu(){//typically called in Loaded()
			//File-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItem("File",menuItemFile_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItem menuItemReports=new MenuItem("Reports");//Use a local variable if there are children
			menuMain.Add(menuItemReports);
			menuItemReports.Add("Daily",menuItemDaily_Click);//Normal simple pattern when there are no children
			MenuItem menuItemWeekly=new MenuItem("Weekly",menuItemWeekly_Click);//Also use a local variable if you want to set more properties.
			menuItemWeekly.IsChecked=true;//very rare, just for example
			menuItemWeekly.Shortcut="W";
			menuItemReports.Add(menuItemWeekly);
			menuItemReports.AddSeparator();
			_menuItemMonthly=new MenuItem("Monthly",menuItemMonthly_Click);//Use class field when you need access later, for example to set Visibility to Collapsed.
			menuItemReports.Add(_menuItemMonthly);
			//Help-----------------------------------------------------------------------------------------------------------
			menuMain.Add("Help",menuItemHelp_Click);
		}

Signature of the click event handler is like this:
private void MenuItem_Click(object sender,EventArgs e) {
	
*/
		#region Fields
		///<summary>This is for Shortcuts on MenuItems.</summary>
		private OpenDental.FrmODBase _frmODBaseParent;
		#endregion Fields

		#region Constructor
		public Menu(){
			InitializeComponent();
			//Width=100;
			//Height=24;//user won't set vertical to stretch
		}
		#endregion Constructor

		/*
		//this doesn't work because some items are separators
		public List<MenuItem> Items{get;set;}
		*/

		#region Methods - public
		public void Add(MenuItem menuItem){
			menu.Items.Add(menuItem);
			SetFrmParent();
		}

		public void Add(string text,EventHandler click){
			MenuItem menuItem=new MenuItem(text,click);
			menu.Items.Add(menuItem);
			SetFrmParent();
		}

		///<summary>Gets direct descendants, excluding separators.</summary>
		public List<MenuItem> GetMenuItems(){
			List<MenuItem> listMenuItems=new List<MenuItem>();
			for(int i=0;i<menu.Items.Count;i++){
				if(menu.Items[i] is System.Windows.Controls.Separator){
					continue;
				}
				listMenuItems.Add((MenuItem)menu.Items[i]);
			}
			return listMenuItems;
		}
		#endregion Methods - public

		#region Methods - private

		private void _frmODBaseParent_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(Keyboard.Modifiers!=ModifierKeys.Control){
				return;
			}
			if(e.Key==Key.LeftCtrl || e.Key==Key.RightCtrl){
				return;//just the Ctrl key is pressed, not additional key.
			}
			//MessageBox.Show("Key");
			List<MenuItem> listMenuItems=GetFlatListMenuItems(this);
			for(int i=0;i<listMenuItems.Count;i++){
				if(e.Key.ToString().ToLower()!=listMenuItems[i].Shortcut.ToLower()){
					continue;
				}
				listMenuItems[i].RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
			}
		}

		///<summary>Gets a flat list of all menu items. Recursive</summary>
		private List<MenuItem> GetFlatListMenuItems(System.Windows.Controls.Control control){
			List<MenuItem> listMenuItems=new List<MenuItem>();
			List<MenuItem> listMenuItemsDirect=new List<MenuItem>();
			if(control is Menu menu2){//menu is already taken
				listMenuItemsDirect.AddRange(menu2.GetMenuItems());
			}
			else if (control is System.Windows.Controls.ContextMenu contextMenu){
				
			}
			else if(control is MenuItem menuItem){
				listMenuItemsDirect.AddRange(menuItem.GetMenuItems());
			}
			else{
				return listMenuItems;
			}
			for(int i=0;i<listMenuItemsDirect.Count;i++){
				listMenuItems.Add(listMenuItemsDirect[i]);
				List<MenuItem> listMenuItemsChildren=GetFlatListMenuItems(listMenuItemsDirect[i]);
				listMenuItems.AddRange(listMenuItemsChildren);
			}
			return listMenuItems;
		}

		private void SetFrmParent(){
			//We must have the parent Frm in order to handle the Key events for Shortcuts.
			//It's not hard to determine, but the problem is when.
			//Can't put it in the click event because that won't get hit yet.
			//The only method where we can tack it on is in Add.
			//At this point, we are guaranteed to have a parent Frm.
			if(_frmODBaseParent!=null){
				return;//already done
			}
			DependencyObject dependencyObject=VisualTreeHelper.GetParent(this);
			while(true){
				if(dependencyObject is null){
					return;
				}
				if(dependencyObject is OpenDental.FrmODBase frmODBase){
					_frmODBaseParent=frmODBase;
					_frmODBaseParent.PreviewKeyDown+=_frmODBaseParent_PreviewKeyDown;
					return;
				}
				dependencyObject=VisualTreeHelper.GetParent(dependencyObject);
			}
		}
		#endregion Methods - private

	}

	#region Classes external
	///<summary>This is all just to show "Menu" in the designer.</summary>
	public class DesignerVisibilityConverter:IValueConverter {
		public object Convert(object value,Type targetType,object parameter,CultureInfo culture) {
			//Binding was set to self with no path for property
			UIElement uIElement=(UIElement)value;
			bool isDesignMode=DesignerProperties.GetIsInDesignMode(uIElement);
			if(isDesignMode){
				return Visibility.Visible;
			}
			return Visibility.Collapsed;//hidden would reserve space
		}

		public object ConvertBack(object value,Type targetType,object parameter,CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	#endregion Classes external
}
