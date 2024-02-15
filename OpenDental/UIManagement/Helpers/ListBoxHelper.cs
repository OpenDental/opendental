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
	public class ListBoxHelper{
		public static Wui.ListBox CreateListBox(OpenDental.UI.ListBox listBox){
			Wui.ListBox listBoxNew=new Wui.ListBox();
			listBoxNew.Width=listBox.Width;
			listBoxNew.Height=listBox.Height;
			if(listBox.SelectionMode==UI.SelectionMode.MultiExtended) {
				listBoxNew.SelectionMode=Wui.SelectionMode.MultiExtended;
			}
			if(listBox.SelectionMode==UI.SelectionMode.None) {
				listBoxNew.SelectionMode=Wui.SelectionMode.None;
			}
			listBoxNew.IsEnabled=listBox.Enabled;
			for(int i=0;i<listBox.Items.Count;i++){
				listBoxNew.Items.Add(listBox.Items.GetTextShowingAt(i),listBox.Items.GetObjectAt(i));
				
			}
			//string strXaml=Properties.Resources.XamlStyle_ListBoxItem;
			//System.IO.StringReader stringReader = new System.IO.StringReader(strXaml);
			//System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
			//Style style=(Style)XamlReader.Load(xmlReader);
			//listBoxNew.ItemContainerStyle=style;
			return listBoxNew;
		}


		//public static void Item_Add(ListBox wListBox,OpenDental.UI.ListBox oListBox,string txt){
		//	ListBoxItem listBoxItem=new ListBoxItem();
		//	listBoxItem.Content=txt;
		//	float heightFont=oListBox.Font.GetHeight();
		//	listBoxItem.Height=heightFont+.5d;//to exactly match old winforms
		//	listBoxItem.Padding=new Thickness(0);//changing this to 1 looks really terrible.
		//	wListBox.Items.Add(listBoxItem);
		//}

	}*/
}
