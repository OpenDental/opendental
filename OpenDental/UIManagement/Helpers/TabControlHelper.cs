using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using CodeBase;

namespace OpenDental.UIManagement {
	/*
	///<summary></summary>
	public class TabControlHelper{
		public static TabControl CreateTabControl(OpenDental.UI.TabControl tabControl){
			//Content is simple grid with one cell
			TabControl tabControlNew=new TabControl();
			//See notes over in listBoxHelper for how this works:
			string strXaml=Properties.Resources.XamlStyle_TabItem;
			System.IO.StringReader stringReader = new System.IO.StringReader(strXaml);
			System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
			Style style=(Style)XamlReader.Load(xmlReader);
			tabControlNew.Resources.Add(typeof(TabItem),style);
			return tabControlNew;
		}


		public static TabItem CreateTabPage(OpenDental.UI.TabPage tabPage,FrameworkElement frameworkElementTabControl){
			TabControl tabControl=(TabControl)frameworkElementTabControl;
			TabItem tabItem=new TabItem();
			tabItem.Header=tabPage.Text;
			Grid gridNew=new Grid();//just one cell
			//gridNew.Background=Brushes.Aqua;
			tabItem.Content=gridNew;
			tabItem.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			tabItem.VerticalContentAlignment=VerticalAlignment.Stretch;
			return tabItem;
		}

		public static void AddPageToTabControl(FrameworkElement frameworkElementTabItem,FrameworkElement frameworkElementTabControl){
			TabControl tabControl=(TabControl)frameworkElementTabControl;
			tabControl.Items.Add(frameworkElementTabItem);
		}

		public static void AddControlToTabPage(FrameworkElement frameworkElementControl,FrameworkElement frameworkElementTabItem){
			TabItem tabItem=(TabItem)frameworkElementTabItem;
			Grid grid=(Grid)tabItem.Content;
			grid.Children.Add(frameworkElementControl);
		}

		public static void SetPositionOnTabPage(FrameworkElement frameworkElementControl,float x,float y){
			Thickness thickness=new Thickness(left:x,top:y,right:0,bottom:0);
			//grid allows absolute positioning relative to grid boundaries.
			//It also announces its desired size to the scrollViewer to allow that to function.
			frameworkElementControl.Margin=thickness;
			frameworkElementControl.VerticalAlignment=VerticalAlignment.Top;
			frameworkElementControl.HorizontalAlignment=HorizontalAlignment.Left;
		}

		public static Size GetClientSize(Proxy proxyTabItem){
			//todo: this is not accurate.  I just keep getting NaN
			TabItem tabItem=(TabItem)proxyTabItem.FrameworkElement_;
			Grid grid=(Grid)tabItem.Content;
			return new Size(grid.Width,grid.Height);
		}
		
	}*/
}
