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
Unlike the MS menu, we don't add menu items in the designer.
Boilerplate for adding menu items:

using WpfControls.UI;
//using System.Windows.Controls;//Don't do this

		private void LayoutMenu(){//typically called in Load()
			//File-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItem("File",menuItemFile_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItem menuItemReports=new MenuItem("Reports");//Use a local variable if there are children
			menuMain.Add(menuItemReports);
			menuItemReports.Add("Daily",menuItemDaily_Click);//Normal simple pattern when there are no children
			MenuItem menuItemWeekly=new MenuItem("Weekly",menuItemWeekly_Click);//Also use a local variable if you want to set more properties.
			menuItemWeekly.IsChecked=true;//very rare, just for example
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
		public Menu(){
			InitializeComponent();
		}

		#region Properties


		#endregion Properties

		#region Methods
		public void Add(MenuItem menuItem){
			menu.Items.Add(menuItem);
		}

		public void Add(string text,EventHandler click){
			MenuItem menuItem=new MenuItem(text,click);
			menu.Items.Add(menuItem);
		}
		#endregion Methods

	}

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
}
