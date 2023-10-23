using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfControls;

namespace OpenDental.UIManagement {
	/*
	///<summary>This class is for internal use by UIManagement. Calls from OD proper should use Um.</summary>
	public class ToolBarHelper{
		public static ToolBar CreateToolBar(OpenDental.UI.ToolBarOD toolBarOD){
			//Contains a list of Buttons, which are built very similarly to our normal buttons.
			//Each has a Grid with two columns and one row.  The two cells contain an image and a textBlock.
			ToolBar toolBarNew=new ToolBar();
			toolBarNew.Background=Brushes.White;
			toolBarNew.BorderBrush=Brushes.DarkGray;
			toolBarNew.BorderThickness=new Thickness(0,0,0,bottom:1);
			//Buttons are always added in code instead of in designer.
			string strXaml = Properties.Resources.XamlStyle_ToolBar;
			System.IO.StringReader stringReader = new System.IO.StringReader(strXaml);
			System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
			//ResourceDictionary resourceDictionary=(ResourceDictionary)XamlReader.Load(xmlReader);
			Style styleToolBar = (Style)XamlReader.Load(xmlReader);
			//menuNew.Resources.Add(typeof(MenuItem),styleMenuItem);
			toolBarNew.Style=styleToolBar;
			return toolBarNew;
		}

		public static void Add(ToolBar toolBar,UI.ODToolBarButton odToolBarButton){
			Wui.ToolBarButton toolBarButton=new Wui.ToolBarButton();
			toolBarButton.SetText(odToolBarButton.Text);
			if(odToolBarButton.Icon!=UI.EnumIcons.None){
				//todo: switch from bitmap to vectors
				BitmapImage bitmapImage=UI.IconLibrary.DrawWpf(odToolBarButton.Icon);
				toolBarButton.SetBitmapImage(bitmapImage);
			}
			else if(odToolBarButton.Bitmap!=null){
				BitmapImage bitmapImage=PictureBoxHelper.ConvertBitmapToWpf(odToolBarButton.Bitmap);
				toolBarButton.SetBitmapImage(bitmapImage);
			}
			toolBarButton.Margin=new Thickness(0,0,0,bottom:-0.5);//gets rid of an annoying white line at the bottom
			if(odToolBarButton.Style!=UI.ODToolBarButtonStyle.NormalButton){
				throw new Exception("Toolbar button style not supported: "+odToolBarButton.Style.ToString());
			}
			toolBarButton.Tag=odToolBarButton.Tag;
			if(!string.IsNullOrEmpty(odToolBarButton.ToolTipText)){
				ToolTip toolTip=new ToolTip();
				toolTip.Content=odToolBarButton.ToolTipText;
				toolBarButton.ToolTip=toolTip;
			}
			toolBarButton.Click+=(sender,e)=>odToolBarButton.EventHandlerClick?.Invoke(toolBarButton,new EventArgs());
			toolBar.Items.Add(toolBarButton);
		}

		public static Wui.ToolBarButton CreateButton(UI.TestForWpf testForWpf){
			//ToolBarButton toolBarButton=new ToolBarButton();
			//toolBarButton.SetText("File");
			//if(testForWpf.Bitmap_!=null){
			//	BitmapImage bitmapImage=ControlHelper.ConvertBitmap(testForWpf.Bitmap_);
			//	toolBarButton.SetBitmapImage(bitmapImage);
			//}
			//return toolBarButton;
			return null;
		}
	}*/
}
