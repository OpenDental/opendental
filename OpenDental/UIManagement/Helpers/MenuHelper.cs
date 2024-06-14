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
	public class MenuHelper{
		#region Methods - public
		public static FrameworkElement CreateMenu(OpenDental.UI.MenuOD menu){
			Menu menuNew=new Menu();
			menuNew.Background=Brushes.White;//just for the main bar. Other backgrounds are set in style.
			menuNew.BorderBrush=Brushes.DarkGray;
			menuNew.BorderThickness=new Thickness(0,0,0,bottom:1);
			string strXaml = Properties.Resources.XamlStyle_MenuItem;
			System.IO.StringReader stringReader = new System.IO.StringReader(strXaml);
			System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
			ResourceDictionary resourceDictionary=(ResourceDictionary)XamlReader.Load(xmlReader);
			//Style styleMenuItem = (Style)XamlReader.Load(xmlReader);
			//menuNew.Resources.Add(typeof(MenuItem),styleMenuItem);
			menuNew.Resources=resourceDictionary;
			return menuNew;
		}

		public static void AddToItem(Menu wMenu,string nameParent,string menuItemText,EventHandler eventHandlerClick){
			MenuItem menuItemParent=null;
			for(int i=0;i<wMenu.Items.Count;i++){//loop through the main menuItems across the top of the menu
				MenuItem menuItemMain=(MenuItem)wMenu.Items[i];
				menuItemParent=GetWMenuItemRecursive(menuItemMain,nameParent);
				if(menuItemParent!=null){
					break;
				}
			}
			if(menuItemParent is null){
				throw new Exception("menuItem name not found: "+nameParent);
			}
			//now we have a MenuItemParent to add to
			MenuItem wMenuItem=new MenuItem();
			//wMenuItem.Background=Brushes.White;
			wMenuItem.Header=menuItemText;
			wMenuItem.Click+=(sender,routedEventArgs)=>eventHandlerClick.Invoke(sender, new EventArgs());
			menuItemParent.Items.Add(wMenuItem);
		}

		public static void AddToMain(Menu menu,string menuItemText,EventHandler eventHandlerClick){
			MenuItem wMenuItem=new MenuItem();
			//wMenuItem.Background=Brushes.White;
			wMenuItem.Height=22;
			wMenuItem.Header=menuItemText;
			wMenuItem.Click+=(sender,routedEventArgs)=>eventHandlerClick.Invoke(sender, new EventArgs());
			menu.Items.Add(wMenuItem);
		}

		public static void AddTreeToItem(Menu wMenu,string nameParent,UI.MenuItemOD menuItemOD){
			MenuItem menuItemParent=null;
			for(int i=0;i<wMenu.Items.Count;i++){//loop through the main menuItems across the top of the menu
				MenuItem menuItemMain=(MenuItem)wMenu.Items[i];
				menuItemParent=GetWMenuItemRecursive(menuItemMain,nameParent);
				if(menuItemParent!=null){
					break;
				}
			}
			if(menuItemParent is null){
				throw new Exception("menuItem name not found: "+nameParent);
			}
			//now we have a MenuItemParent to add to
			MenuItem wMenuItem=new MenuItem();
			//wMenuItem.Background=Brushes.White;
			wMenuItem.Name=menuItemOD.Name;
			wMenuItem.Header=menuItemOD.Text;
			wMenuItem.Click+=(sender,routedEventArgs)=>InvokeClick(menuItemOD);
			for(int i=0;i<menuItemOD.DropDown.Items.Count;i++){
				if(menuItemOD.DropDown.Items[i].GetType()==typeof(System.Windows.Forms.ToolStripSeparator)){
					wMenuItem.Items.Add(new Separator());
					continue;
				}
				UI.MenuItemOD menuItemODChild=(UI.MenuItemOD)menuItemOD.DropDown.Items[i];
				AddToItemRecursive(wMenuItem,menuItemODChild);
			}
			menuItemParent.Items.Add(wMenuItem);
		}

		public static void AddTreeToMain(Menu menu,UI.MenuItemOD menuItemOD){
			MenuItem wMenuItem=new MenuItem();
			//wMenuItem.Background=Brushes.White;
			wMenuItem.Name=menuItemOD.Name;
			wMenuItem.Height=22;
			wMenuItem.Header=menuItemOD.Text;
			wMenuItem.Click+=(sender,routedEventArgs)=>InvokeClick(menuItemOD);
			for(int i=0;i<menuItemOD.DropDown.Items.Count;i++){
				if(menuItemOD.DropDown.Items[i].GetType()==typeof(System.Windows.Forms.ToolStripSeparator)){
					wMenuItem.Items.Add(new Separator());
					continue;
				}
				UI.MenuItemOD menuItemODChild=(UI.MenuItemOD)menuItemOD.DropDown.Items[i];
				AddToItemRecursive(wMenuItem,menuItemODChild);
			}
			menu.Items.Add(wMenuItem);
		}
		#endregion Methods - public

		#region Methods - private
		///<summary></summary>
		private static void AddToItemRecursive(MenuItem wMenuItemParent,UI.MenuItemOD menuItemOD){
			MenuItem wMenuItem=new MenuItem();
			//wMenuItem.Background=Brushes.White;
			wMenuItem.Name=menuItemOD.Name;
			wMenuItem.Header=menuItemOD.Text;
			wMenuItem.Click+=(sender,routedEventArgs)=>InvokeClick(menuItemOD);
			//Add children
			for(int i=0;i<menuItemOD.DropDown.Items.Count;i++){
				if(menuItemOD.DropDown.Items[i].GetType()==typeof(System.Windows.Forms.ToolStripSeparator)){
					wMenuItem.Items.Add(new Separator());
					continue;
				}
				UI.MenuItemOD menuItemODChild=(UI.MenuItemOD)menuItemOD.DropDown.Items[i];
				AddToItemRecursive(wMenuItem,menuItemODChild);
			}
			wMenuItemParent.Items.Add(wMenuItem);
		}

		///<summary>Checks self and children. Returns null if not found.</summary>
		private static MenuItem GetWMenuItemRecursive(MenuItem wMenuItem,string name){
			if(wMenuItem.Name==name){
				return wMenuItem;
			}
			//Children
			for(int i=0;i<wMenuItem.Items.Count;i++){
				if(wMenuItem.Items[i] is Separator){
					continue;
				}
				MenuItem wMenuItemChild=(MenuItem)wMenuItem.Items[i];
				MenuItem wMenuItemFound=GetWMenuItemRecursive(wMenuItemChild,name);
				if(wMenuItemFound!=null){
					return wMenuItemFound;
				}
			}
			return null;
		}

		private static void InvokeClick(UI.MenuItemOD menuItem){
			//largely copied from ControlHelper.InvokeClick, so more comments over there.
			FieldInfo fieldInfo = typeof(System.Windows.Forms.ToolStripItem).GetField("EventClick",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo propertyInfo = typeof(System.Windows.Forms.ToolStripItem).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			EventHandlerList eventHandlerList = propertyInfo.GetValue(menuItem, new object[] { }) as EventHandlerList;
			object eventKey = fieldInfo.GetValue(menuItem);
			Delegate delegateMy = eventHandlerList[eventKey] as Delegate;
			if(delegateMy==null){
				return;
			}
			object[] objectArrayParameters=new object[2];
			objectArrayParameters[0]=menuItem;//sender is ignored
			objectArrayParameters[1]=new System.EventArgs();
			delegateMy.Method.Invoke(delegateMy.Target,objectArrayParameters);
		}
		#endregion Methods - private

		#region Context Menus
		public static ContextMenu CreateContextMenu(System.Windows.Forms.ContextMenuStrip contextMenuStrip){
			ContextMenu contextMenu=new ContextMenu();
			contextMenu.Name=contextMenuStrip.Name;
			for(int i=0;i<contextMenuStrip.Items.Count;i++){
				MenuItem wMenuItem=new MenuItem();
				wMenuItem.Header=contextMenuStrip.Items[i].Text;
				wMenuItem.Name=contextMenuStrip.Items[i].Name;
				System.Windows.Forms.ToolStripItem toolStripItem=contextMenuStrip.Items[i];//can't use index on next line
				wMenuItem.Click+=(sender,routedEventArgs)=>InvokeContextMenuClick(toolStripItem);
				AddContextMenuChildren(wMenuItem,contextMenuStrip.Items[i]);
				contextMenu.Items.Add(wMenuItem);
			}
			//opening event must be done manually in the load of the form.
			return contextMenu;
		}

		///<summary>Pass in a wpf menuItem and a WF toolStripItem, and it will add all children recursively.</summary>
		private static void AddContextMenuChildren(MenuItem wMenuItem,System.Windows.Forms.ToolStripItem toolStripItem){
			System.Windows.Forms.ToolStripMenuItem toolStripMenuItem=(System.Windows.Forms.ToolStripMenuItem)toolStripItem;
			if(!toolStripMenuItem.HasDropDownItems){
				return;
			}
			for(int i=0;i<toolStripMenuItem.DropDownItems.Count;i++){
				MenuItem wMenuItemChild=new MenuItem();
				wMenuItemChild.Header=toolStripMenuItem.DropDownItems[i].Text;
				wMenuItemChild.Name=toolStripMenuItem.DropDownItems[i].Name;
				System.Windows.Forms.ToolStripItem toolStripItemChild=toolStripMenuItem.DropDownItems[i];
				wMenuItemChild.Click+=(sender,routedEventArgs)=>InvokeContextMenuClick(toolStripItemChild);
				AddContextMenuChildren(wMenuItemChild,toolStripMenuItem.DropDownItems[i]);
				wMenuItem.Items.Add(wMenuItemChild);
			}
		}

		public static System.Windows.Forms.ToolStripMenuItem GetFMenuItem(System.Windows.Forms.ContextMenuStrip contextMenuStrip,string menuItemName){
			for(int i=0;i<contextMenuStrip.Items.Count;i++){
				if(contextMenuStrip.Items[i].Name==menuItemName){
					return (System.Windows.Forms.ToolStripMenuItem)contextMenuStrip.Items[i];
				}
				System.Windows.Forms.ToolStripMenuItem fMenuItemFound=GetFMenuItemRecursive((System.Windows.Forms.ToolStripMenuItem)contextMenuStrip.Items[i],menuItemName);
				if(fMenuItemFound!=null){
					return (System.Windows.Forms.ToolStripMenuItem)fMenuItemFound;
				}
			}
			return null;
		}

		///<summary>Checks self and children. Returns null if not found.</summary>
		private static System.Windows.Forms.ToolStripMenuItem GetFMenuItemRecursive(System.Windows.Forms.ToolStripMenuItem fToolStripMenuItem,string menuItemName){
			if(fToolStripMenuItem.Name==menuItemName){
				return fToolStripMenuItem;
			}
			//Children
			for(int i=0;i<fToolStripMenuItem.DropDownItems.Count;i++){
				//if(menuItem.Items[i] is Separator){
				//	continue;
				//}
				System.Windows.Forms.ToolStripMenuItem fMenuItemChild=(System.Windows.Forms.ToolStripMenuItem)fToolStripMenuItem.DropDownItems[i];
				System.Windows.Forms.ToolStripMenuItem fMenuItemFound=GetFMenuItemRecursive(fMenuItemChild,menuItemName);
				if(fMenuItemFound!=null){
					return fMenuItemFound;
				}
			}
			return null;
		}

		public static MenuItem GetWMenuItem(ContextMenu contextMenu,string menuItemName){
			for(int i=0;i<contextMenu.Items.Count;i++){
				if(((MenuItem)(contextMenu.Items[i])).Name==menuItemName){
					return (MenuItem)contextMenu.Items[i];
				}
				MenuItem wMenuItemFound=GetWMenuItemRecursive((MenuItem)contextMenu.Items[i],menuItemName);
				if(wMenuItemFound!=null){
					return wMenuItemFound;
				}
			}
			return null;
		}*/

		/*
		///<summary>Checks self and children. Returns null if not found.</summary>
		private static MenuItem GetWMenuItemRecursive(MenuItem wMenuItem,string menuItemName){
			if(wMenuItem.Name==menuItemName){
				return wMenuItem;
			}
			//Children
			for(int i=0;i<wMenuItem.Items.Count;i++){
				//if(menuItem.Items[i] is Separator){
				//	continue;
				//}
				MenuItem wMenuItemChild=(MenuItem)wMenuItem.Items[i];
				MenuItem wMenuItemFound=GetWMenuItemRecursive(wMenuItemChild,menuItemName);
				if(wMenuItemFound!=null){
					return wMenuItemFound;
				}
			}
			return null;
		}*/

	/*
		private static void InvokeContextMenuClick(System.Windows.Forms.ToolStripItem toolStripItem){
			//largely copied from ControlHelper.InvokeClick, so more comments over there.
			FieldInfo fieldInfo = typeof(System.Windows.Forms.ToolStripItem).GetField("EventClick",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo propertyInfo = typeof(System.Windows.Forms.ToolStripItem).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			EventHandlerList eventHandlerList = propertyInfo.GetValue(toolStripItem, new object[] { }) as EventHandlerList;
			object eventKey = fieldInfo.GetValue(toolStripItem);
			Delegate delegateMy = eventHandlerList[eventKey] as Delegate;
			if(delegateMy==null){
				return;
			}
			object[] objectArrayParameters=new object[2];
			objectArrayParameters[0]=toolStripItem;//sender is ignored
			objectArrayParameters[1]=new System.EventArgs();
			delegateMy.Method.Invoke(delegateMy.Target,objectArrayParameters);
		}
		#endregion Context Menus
	}*/
}
