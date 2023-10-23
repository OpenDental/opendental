using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenDental.UIManagement {
	/*
	///<summary>Um is short for UIManager. This is how programmers interact with all controls on a form.</summary>
	public class Um {
		public UIManager UIManager_;

		#region ComboBox
		//We will try a synching pattern so that we can keep all our UI.ComboBox (oComboBox) custom functionality.
		//The way the synching will work is that the "real" data is in the oComboBox copy.
		//We show a shadow copy in the WPF ComboBox (wComboBox).
		//SelectedIndices are an exception. wComboBox is responsible for selected indices which are copied to oComboBox as needed.
		public int ComboBox_GetSelectedIndex(string comboBoxName){
			UI.ComboBox oComboBox=(UI.ComboBox)GetControl(comboBoxName);
			if(UIManager_.IsDpiSystem){
				return oComboBox.SelectedIndex;
			}
			ComboBox wComboBox=(ComboBox)GetFrameworkElement(comboBoxName);
			//if(oComboBox.SelectionMode==UI.SelectionMode.MultiExtended){
			//	throw new Exception("GetSelectedIndex is ambiguous when SelectionMode.MultiExtended.");
			//}
			return wComboBox.SelectedIndex;
		}

		///<summary>Specify the text to show. Optionally, specify the object represented by that text. Also, optionally display an abbreviation for each item to display in the selected summary above.</summary>
		public void ComboBox_Items_Add(string comboBoxName,string text,object item=null,string abbr=null){
			UI.ComboBox oComboBox=(UI.ComboBox)GetControl(comboBoxName);
			if(UIManager_.IsDpiSystem){
				oComboBox.Items.Add(text,item,abbr);
				return;
			}
			ComboBox wComboBox=(ComboBox)GetFrameworkElement(comboBoxName);
			oComboBox.Items.Add(text,item,abbr);
			//synch to the UI
			int idxSelected=wComboBox.SelectedIndex;
			wComboBox.Items.Clear();
			for(int i=0;i<oComboBox.Items.Count;i++){
				ComboBoxHelper.Item_Add(wComboBox,oComboBox,oComboBox.Items.GetTextShowingAt(i));
			}
			wComboBox.SelectedIndex=idxSelected;
		}

		public void ComboBox_Items_Clear(string comboBoxName){
			UI.ComboBox oComboBox=(UI.ComboBox)GetControl(comboBoxName);
			if(UIManager_.IsDpiSystem){
				oComboBox.Items.Clear();
				return;
			}
			ComboBox wComboBox=(ComboBox)GetFrameworkElement(comboBoxName);
			oComboBox.Items.Clear();
			wComboBox.Items.Clear();
		}

		public void ComboBox_SetSelectedIndex(string comboBoxName,int idx){
			UI.ComboBox oComboBox=(UI.ComboBox)GetControl(comboBoxName);
			if(UIManager_.IsDpiSystem){
				oComboBox.SelectedIndex=idx;
				return;
			}
			ComboBox wComboBox=(ComboBox)GetFrameworkElement(comboBoxName);
			//if(oComboBox.SelectionMode==UI.SelectionMode.None) {
			//	throw new Exception("SetSelectedIndex is not allowed when SelectionMode.None.");
			//}
			wComboBox.SelectedIndex=idx;
		}
		#endregion ComboBox

		#region Control Get
		public System.Drawing.Color GetBackColor(string controlName){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				return control.BackColor;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.Panel:
					Color color=PanelHelper.GetBackColor(proxy);
					return ColorOD.FromWpf(color);
				default:
					throw new Exception("GetBackColor is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}
		///<summary>Untested</summary>
		public System.Drawing.Size GetClientSize(string controlName){
			System.Windows.Forms.Control control=(System.Windows.Forms.Control)GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				return control.ClientSize;
			}
			Proxy proxy=GetProxy(controlName);
			return UIManager_.GetClientSize(proxy);
		}

		///<summary>Because this is a nullable bool, it works for both two state and three state checkBoxes. For radioButtons and 2-state, just ignore the null.</summary>
		public bool? GetIsChecked(string controlName){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				if(control is OpenDental.UI.CheckBox checkBox){
					if(checkBox.ThreeState){
						if(checkBox.CheckState==System.Windows.Forms.CheckState.Checked){
							return true;
						}
						if(checkBox.CheckState==System.Windows.Forms.CheckState.Unchecked){
							return false;
						}
						return null;
					}
					//2-state
					return checkBox.Checked;
				}
				return ((System.Windows.Forms.RadioButton)control).Checked;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.CheckBox:
					return ((CheckBox)proxy.FrameworkElement_).IsChecked;
				case EnumTypeControl.RadioButton:
					return ((RadioButton)proxy.FrameworkElement_).IsChecked;
				default:
					throw new Exception("GetIsChecked is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public string GetText(string controlName){
			System.Windows.Forms.Control control=(System.Windows.Forms.Control)GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				return control.Text;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.Button:
					return ((Button)proxy.FrameworkElement_).Content.ToString();
				case EnumTypeControl.Label:
					TextBlock textBlock= (TextBlock)((Label)proxy.FrameworkElement_).Content;
					return textBlock.Text;
				case EnumTypeControl.TextBox:
				case EnumTypeControl.ValidDouble:
				case EnumTypeControl.ValidNum:
					return ((TextBox)proxy.FrameworkElement_).Text;
				case EnumTypeControl.ValidDate:
					return ((Wui.TextBoxValidDate)proxy.FrameworkElement_).Text;
				default:
					throw new Exception("GetText is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public System.Drawing.Image PictureBox_GetImage(string controlName){
			System.Windows.Forms.PictureBox pictureBox=(System.Windows.Forms.PictureBox)GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				return pictureBox.Image;
			}
			Proxy proxy=GetProxy(controlName);
			Border border=(Border)proxy.FrameworkElement_;
			Image image=(Image)border.Child;
			BitmapImage bitmapImage=(BitmapImage)image.Source;
			System.Drawing.Bitmap bitmap=(System.Drawing.Bitmap)PictureBoxHelper.ConvertFromWpf(bitmapImage);
			return bitmap;
		}
		#endregion Control Get

		#region Control Set
		public void Focus(string controlName) {
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				control.Focus();
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.TextBox:
					((TextBox)proxy.FrameworkElement_).Focus();
					return;
				default:
					throw new Exception("Focus is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public void PictureBox_SetImage(string controlName, System.Drawing.Bitmap bitmap){
			System.Windows.Forms.PictureBox pictureBox=(System.Windows.Forms.PictureBox)GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				pictureBox.Image=bitmap;
				return;
			}
			Proxy proxy=GetProxy(controlName);
			Border border=((Border)proxy.FrameworkElement_);
			Image image=(Image)border.Child;
			if(bitmap==null) {
				image.Source=null;
				return;
			}
			image.Source=PictureBoxHelper.ConvertBitmapToWpf(bitmap);
			return;
		}

		public void SetBackColor(string controlName, System.Drawing.Color color){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				control.BackColor=color;
				return;
			}
			Color colorNew=ColorOD.ToWpf(color);
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.Panel:
					PanelHelper.SetBackColor(proxy,colorNew);
					return;
				default:
					throw new Exception("SetBackColor is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public void SetEnabled(string controlName, bool isEnabled){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				control.Enabled=isEnabled;
				return;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.Button:
					((Button)proxy.FrameworkElement_).IsEnabled=isEnabled;
					return;
				case EnumTypeControl.CheckBox:
					((CheckBox)proxy.FrameworkElement_).IsEnabled=isEnabled;
					return;
				case EnumTypeControl.Label:
					((Label)proxy.FrameworkElement_).IsEnabled=isEnabled;
					return;
				case EnumTypeControl.ListBox:
					((ListBox)proxy.FrameworkElement_).IsEnabled=isEnabled;
					return;
				default:
					throw new Exception("SetEnabled is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public void SetForeColor(string controlName, System.Drawing.Color color){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				control.ForeColor=color;
				return;
			}
			Color colorNew=ColorOD.ToWpf(color);
			SolidColorBrush solidColorBrush = new SolidColorBrush(colorNew);
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.TextBox:
					((TextBox)proxy.FrameworkElement_).Foreground=solidColorBrush;
					return;
				default:
					throw new Exception("SetForeColor is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		///<summary>Because this is a nullable bool, it works for both two state and three state checkBoxes.</summary>
		public void SetChecked(string controlName,bool? value){
			//Didn't use name SetIsChecked because that felt wrong when sending in a bool.
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				if(control is System.Windows.Forms.CheckBox checkBox){
					if(checkBox.ThreeState){
						if(!value.HasValue){
							checkBox.CheckState=System.Windows.Forms.CheckState.Indeterminate;
							return;
						}
						if(value.Value){
							checkBox.CheckState=System.Windows.Forms.CheckState.Checked;
							return;
						}
						checkBox.CheckState=System.Windows.Forms.CheckState.Unchecked;
						return ;
					}
					//2-state
					checkBox.Checked=value.Value;
					return;
				}
				((System.Windows.Forms.RadioButton)control).Checked=value.Value;
				return;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.CheckBox:
					((CheckBox)proxy.FrameworkElement_).IsChecked=value;
					return;
				case EnumTypeControl.RadioButton:
					((RadioButton)proxy.FrameworkElement_).IsChecked=value;
					return;
				default:
					throw new Exception("SetIsChecked is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public void SetText(string controlName,string value){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				control.Text=value;
				return;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.Button:
					((Button)proxy.FrameworkElement_).Content=value;
					return;
				case EnumTypeControl.Label:
					TextBlock textBlock= (TextBlock)((Label)proxy.FrameworkElement_).Content;
					textBlock.Text=value;
					return;
				case EnumTypeControl.TextBox:
				case EnumTypeControl.ValidDouble:
				case EnumTypeControl.ValidNum:
					((TextBox)proxy.FrameworkElement_).Text=value;
					return;
				case EnumTypeControl.ValidDate:
					((Wui.TextBoxValidDate)proxy.FrameworkElement_).Text=value;
					return;
				default:
					throw new Exception("SetText is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		/// <summary>Sets the visibility of the object. Pass in true to set it visibile, false for hidden</summary>
		public void SetVisible(string controlName, bool isVisible){
			System.Windows.Forms.Control control=GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				control.Visible=isVisible;
				return;
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.Button:
					if(isVisible){
						((Button)proxy.FrameworkElement_).Visibility=Visibility.Visible;
						return;
					}
					((Button)proxy.FrameworkElement_).Visibility=Visibility.Hidden;
					return;
				case EnumTypeControl.CheckBox:
					if(isVisible){
						((CheckBox)proxy.FrameworkElement_).Visibility=Visibility.Visible;
						return;
					}
					((CheckBox)proxy.FrameworkElement_).Visibility=Visibility.Hidden;
					return;
				case EnumTypeControl.Label:
					if(isVisible){
						((Label)proxy.FrameworkElement_).Visibility=Visibility.Visible;
						return;
					}
					((Label)proxy.FrameworkElement_).Visibility=Visibility.Hidden;
					return;
				case EnumTypeControl.PictureBox:
					if(isVisible){
						((Border)proxy.FrameworkElement_).Visibility=Visibility.Visible;
						return;
					}
					((Border)proxy.FrameworkElement_).Visibility=Visibility.Hidden;
					return;
				case EnumTypeControl.ComboBox:
					if(isVisible) {
					((ComboBox)proxy.FrameworkElement_).Visibility=Visibility.Visible;
						return;
					}
					((ComboBox)proxy.FrameworkElement_).Visibility=Visibility.Hidden;
						return;
				default:
					throw new Exception("SetVisible is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}
		#endregion Control Set

		#region Grid
		/// <summary>Need to call this where we would call Begin Update normally in case the user isn't using the UI manager. Does nothing if they are using the UI Manager</summary>
		public void Grid_BeginUpdate(string gridName){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				grid.BeginUpdate();
			}
		}

		public void Grid_ColumnAdd(string gridName,Wui.GridColumn gridColumn){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				UI.GridColumn uIGridColumn=DataGridHelper.ConvertGridCol(gridColumn);
				grid.Columns.Add(uIGridColumn);
				return;
			}
			Wui.DataGrid dataGrid=(Wui.DataGrid)GetFrameworkElement(gridName);
			dataGrid.ColumnAdd(gridColumn);
		}

		public void Grid_ColumnsClear(string gridName){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				grid.Columns.Clear();
				return;
			}
			FrameworkElement frameworkElement=GetFrameworkElement(gridName);
			DataGridHelper.ColumnsClear(frameworkElement);
		}

		/// <summary>Need to call this where we would call End Update normally in case the user isn't using the UI manager. Does nothing if they are using the UI Manager</summary>
		public void Grid_EndUpdate(string gridName){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				grid.EndUpdate();
			}
		}

		public List<Wui.GridRow> Grid_GetListGridRows(string gridName){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid = (UI.GridOD)GetControl(gridName);
				List<Wui.GridRow> listGridRows=DataGridHelper.ConvertGridRowsToWpf(grid.ListGridRows);
				return listGridRows;
			}
			Wui.DataGrid dataGrid=(Wui.DataGrid)GetFrameworkElement(gridName);
			return dataGrid.GetListGridRows();
		}

		public int Grid_GetSelectedIndex(string gridName){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				return grid.GetSelectedIndex();
			}
			Wui.DataGrid dataGrid=(Wui.DataGrid)GetFrameworkElement(gridName);
			return dataGrid.SelectedIndex;
		}

		public T Grid_GetSelectedTag<T>(string gridName){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				return grid.SelectedTag<T>();
			}
			Wui.DataGrid dataGrid=(Wui.DataGrid)GetFrameworkElement(gridName);
			return dataGrid.GetSelectedTag<T>();
		}

		public void Grid_SetListGridRows(string gridName,List<Wui.GridRow> listGridRows){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				List<UI.GridRow> listUIGridRows=DataGridHelper.ConvertGridRowsFromWpf(listGridRows);
				grid.ListGridRows.AddRange(listUIGridRows);
				return;
			}
			Wui.DataGrid dataGrid=(Wui.DataGrid)GetFrameworkElement(gridName);
			dataGrid.SetListGridRows(listGridRows);
		}

		public void Grid_SetSelected(string gridName, int index,bool setValue=true){
			if(UIManager_.IsDpiSystem){
				UI.GridOD grid=(UI.GridOD)GetControl(gridName);
				grid.SetSelected(index,setValue);
				return;
			}
			Wui.DataGrid dataGrid=(Wui.DataGrid)GetFrameworkElement(gridName);
			//dataGrid.SetSelected()
			//DataGridHelper.SetSelectedIndex(dataGrid, index, setValue);
		}
		#endregion Grid

		#region ListBox
		public List<int> ListBox_GetSelectedIndices(string listBoxName){
			UI.ListBox oListBox=(UI.ListBox)GetControl(listBoxName);
			if(UIManager_.IsDpiSystem){
				return oListBox.SelectedIndices;
			}
			Wui.ListBox wListBox=(Wui.ListBox)GetFrameworkElement(listBoxName);
			return wListBox.SelectedIndices;
		}

		public int ListBox_GetSelectedIndex(string listBoxName){
			UI.ListBox oListBox=(UI.ListBox)GetControl(listBoxName);
			if(UIManager_.IsDpiSystem){
				return oListBox.SelectedIndex;
			}
			ListBox wListBox=(ListBox)GetFrameworkElement(listBoxName);
			if(oListBox.SelectionMode==UI.SelectionMode.MultiExtended){
				throw new Exception("GetSelectedIndex is ambiguous when SelectionMode.MultiExtended.");
			}
			return wListBox.SelectedIndex;
		}

		///<summary>Specify the text to show. Optionally, specify the object represented by that text. Also, optionally display an abbreviation for each item to display in the selected summary above.</summary>
		public void ListBox_Items_Add(string listBoxName,string text,object item=null,string abbr=null){
			UI.ListBox oListBox=(UI.ListBox)GetControl(listBoxName);
			if(UIManager_.IsDpiSystem){
				oListBox.Items.Add(text,item,abbr);
				return;
			}
			Wui.ListBox wListBox=(Wui.ListBox)GetFrameworkElement(listBoxName);
			wListBox.Items.Add(text,item,abbr);
		}

		///<summary></summary>
		public void ListBox_Items_AddList(string listBoxName,List<Wui.ListBoxItem> listListBoxItems){
			UI.ListBox oListBox=(UI.ListBox)GetControl(listBoxName);
			if(UIManager_.IsDpiSystem){
				for(int i=0;i<listListBoxItems.Count;i++){
					oListBox.Items.Add(listListBoxItems[i].Text,listListBoxItems[i].Item,listListBoxItems[i].Abbr);
				}
				return;
			}
			Wui.ListBox wListBox=(Wui.ListBox)GetFrameworkElement(listBoxName);
			wListBox.Items.AddList(listListBoxItems);
		}

		///<summary>Deprecated.  We can't do this anymore because the WpfControls library can't internally do language translations.  See boilerplate at the top of WpfControls.ListBox for the new pattern.</summary>
		[Obsolete()]
		public void ListBox_Items_AddEnums<T>(string listBoxName) where T : Enum {
			//no
		}

		public void ListBox_Items_Clear(string listBoxName){
			UI.ListBox oListBox=(UI.ListBox)GetControl(listBoxName);
			if(UIManager_.IsDpiSystem){
				oListBox.Items.Clear();
				return;
			}
			Wui.ListBox wListBox=(Wui.ListBox)GetFrameworkElement(listBoxName);
			wListBox.Items.Clear();
		}

		public void ListBox_SetSelectedIndex(string listBoxName,int idx){
			UI.ListBox oListBox=(UI.ListBox)GetControl(listBoxName);
			if(UIManager_.IsDpiSystem){
				oListBox.SelectedIndex=idx;
				return;
			}
			Wui.ListBox wListBox=(Wui.ListBox)GetFrameworkElement(listBoxName);
			if(oListBox.SelectionMode==UI.SelectionMode.None) {
				throw new Exception("SetSelectedIndex is not allowed when SelectionMode.None.");
			}
			wListBox.SelectedIndex=idx;
		}
		#endregion ListBox

		#region Menu
		///<summary>Use this in the load event handler of the form.</summary>
		public void ContextMenuStrip_SetEventOpening(string parentControlName,CancelEventHandler cancelEventHandler){
			if(UIManager_.IsDpiSystem){
				return;//already handled by the normal eventhandler
			}
			//ContextMenu contextMenu=(ContextMenu)GetFrameworkElement(contextMenuName);//doesn't work because context menu is not a child control of form
			FrameworkElement frameworkElement=GetFrameworkElement(parentControlName);
			frameworkElement.ContextMenuOpening+=(sender,contextMenuEventArgs)=>{
				CancelEventArgs cancelEventArgs=new CancelEventArgs();
				cancelEventHandler?.Invoke(frameworkElement,cancelEventArgs);
				if(cancelEventArgs.Cancel){
					contextMenuEventArgs.Handled=true;
				}
			};
		}

		///<summary>In wpf, context menus are accessed from the parent control that they are attached to rather than from the main form, so you have to pass that in.</summary>
		public void ContextMenuItem_SetVisible(string parentControlName,string contextMenuName,string menuItemName,bool isVisible){
			System.Windows.Forms.ContextMenuStrip fContextMenu=(System.Windows.Forms.ContextMenuStrip)GetComponent(contextMenuName);
			if(UIManager_.IsDpiSystem){
				System.Windows.Forms.ToolStripMenuItem fMenuItemFound=MenuHelper.GetFMenuItem(fContextMenu,menuItemName);
				if(fMenuItemFound is null){
					throw new Exception("menuItemName not found: "+menuItemName);
				}
				fMenuItemFound.Visible=isVisible;
				return;
			}
			FrameworkElement frameworkElement=GetFrameworkElement(parentControlName);
			ContextMenu wContextMenu=frameworkElement.ContextMenu;
			MenuItem wMenuItemFound=MenuHelper.GetWMenuItem(wContextMenu,menuItemName);
			if(wMenuItemFound is null){
				throw new Exception("menuItemName not found: "+menuItemName);
			}
			if(isVisible){
				wMenuItemFound.Visibility=Visibility.Visible;
			}
			else{
				wMenuItemFound.Visibility=Visibility.Collapsed;
			}
		}

		///<summary>Adds a Menu Item to an existing item.</summary>
		public void Menu_AddToItem(string menuName,string nameParent,string menuItemText,EventHandler eventHandlerClick){
			UI.MenuOD menuOD=(UI.MenuOD)GetControl(menuName);
			if(UIManager_.IsDpiSystem){
				UI.MenuItemOD menuItemODParent=menuOD.GetMenuItemByName(nameParent);
				if(menuItemODParent is null){
					throw new Exception("menuItem Name not found: "+nameParent);
				}
				menuItemODParent.Add(new UI.MenuItemOD(menuItemText,eventHandlerClick));
				return;
			}
			Menu wMenu=(Menu)GetFrameworkElement(menuName);
			MenuHelper.AddToItem(wMenu,nameParent,menuItemText,eventHandlerClick);
		}

		///<summary>Adds a Menu Item directly to the main Menu itself.</summary>
		public void Menu_AddToMain(string menuName,string menuItemText,EventHandler eventHandlerClick){
			UI.MenuOD menuOD=(UI.MenuOD)GetControl(menuName);
			if(UIManager_.IsDpiSystem){
				menuOD.Add(new UI.MenuItemOD(menuItemText,eventHandlerClick));
				return;
			}
			Menu wMenu=(Menu)GetFrameworkElement(menuName);
			MenuHelper.AddToMain(wMenu,menuItemText,eventHandlerClick);
		}

		///<summary>This adds a Menu Item that optionally already has children attached.</summary>
		public void Menu_AddTreeToItem(string menuName,string nameParent,UI.MenuItemOD menuItemOD){
			UI.MenuOD menuOD=(UI.MenuOD)GetControl(menuName);
			if(UIManager_.IsDpiSystem){
				UI.MenuItemOD menuItemODParent=menuOD.GetMenuItemByName(nameParent);
				if(menuItemODParent is null){
					throw new Exception("menuItem Name not found: "+nameParent);
				}
				menuItemODParent.Add(menuItemOD);
				return;
			}
			Menu wMenu=(Menu)GetFrameworkElement(menuName);
			MenuHelper.AddTreeToItem(wMenu,nameParent,menuItemOD);
		}

		///<summary>This adds a Menu Item that optionally already has children attached.</summary>
		public void Menu_AddTreeToMain(string menuName,UI.MenuItemOD menuItem){
			UI.MenuOD menuOD=(UI.MenuOD)GetControl(menuName);
			if(UIManager_.IsDpiSystem){
				menuOD.Add(menuItem);
				return;
			}
			Menu wMenu=(Menu)GetFrameworkElement(menuName);
			MenuHelper.AddTreeToMain(wMenu,menuItem);
		}
		#endregion Menu

		#region Misc
		public void LinkLabel_SetEvent_LinkClicked(string linkLabelName,System.Windows.Forms.LinkLabelLinkClickedEventHandler eventHandler){
			if(UIManager_.IsDpiSystem){
				return;//already handled by the normal eventhandler
			}
			//ContextMenu contextMenu=(ContextMenu)GetFrameworkElement(contextMenuName);//doesn't work because context menu is not a child control of form
			Wui.LinkLabel linkLabel=(Wui.LinkLabel)GetFrameworkElement(linkLabelName);
			System.Windows.Forms.LinkLabelLinkClickedEventArgs linkLabelLinkClickedEventArgs=new System.Windows.Forms.LinkLabelLinkClickedEventArgs(new System.Windows.Forms.LinkLabel.Link());
			linkLabel.LinkClicked+=(sender,contextMenuEventArgs)=>
				eventHandler?.Invoke(linkLabel,linkLabelLinkClickedEventArgs);
		}
		#endregion Misc

		#region TextBox
		public void TextBox_SetSelectionStart(string textBoxName,int selectionStart){
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)GetControl(textBoxName);
			if(UIManager_.IsDpiSystem){
				textBox.SelectionStart=selectionStart;
			}
			Proxy proxy=GetProxy(textBoxName);
			((TextBox)proxy.FrameworkElement_).SelectionStart=selectionStart;
		}

		public bool ValidTextBox_IsValid(string controlName){
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				if(textBox is ValidDate validDate){
					return validDate.IsValid();
				}
				else if(textBox is ValidDouble validDouble){
					return validDouble.IsValid();
				}
				else if(textBox is ValidNum validNum){
					return validNum.IsValid();
				}
				else{
					throw new Exception("TextIsValid is not yet implemented for this type: "+textBox.GetType().ToString());//example ValidTime, ValidPhone, etc.
				}
			}
			Proxy proxy=GetProxy(controlName);
			switch(proxy.TypeControl){
				case EnumTypeControl.ValidDate:
					textBox.Text=((Wui.TextBoxValidDate)proxy.FrameworkElement_).Text;
					return ((ValidDate)textBox).IsValid();
				case EnumTypeControl.ValidDouble:
					textBox.Text=((TextBox)proxy.FrameworkElement_).Text;
					return ((ValidDouble)textBox).IsValid();
				case EnumTypeControl.ValidNum:
					textBox.Text=((TextBox)proxy.FrameworkElement_).Text;
					return ((ValidNum)textBox).IsValid();
				default:
					throw new Exception("TextIsValid is not yet implemented for this type: "+proxy.TypeControl.ToString());
			}
		}

		public void TextBox_SetIsReadOnly(string textBoxName,bool isReadOnly) {
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)GetControl(textBoxName);
			if(UIManager_.IsDpiSystem){
				textBox.ReadOnly=isReadOnly;
			}
			Proxy proxy=GetProxy(textBoxName);
			((TextBox)proxy.FrameworkElement_).IsReadOnly=isReadOnly;
		}
		#endregion TextBox

		#region ToolBar
		///<summary></summary>
		public void ToolBar_Add(string nameToolBar,UI.ODToolBarButton odToolBarButton){
			UI.ToolBarOD toolBarOD=(UI.ToolBarOD)GetControl(nameToolBar);
			if(UIManager_.IsDpiSystem){
				toolBarOD.Buttons.Add(odToolBarButton);
				return;
			}
			ToolBar wToolBar=(ToolBar)GetFrameworkElement(nameToolBar);
			ToolBarHelper.Add(wToolBar,odToolBarButton);
		}
		
		public void ToolBar_ButtonsClear(string nameToolBar){
			UI.ToolBarOD toolBarOD=(UI.ToolBarOD)GetControl(nameToolBar);
			if(UIManager_.IsDpiSystem){
				toolBarOD.Buttons.Clear();
				return;
			}
			ToolBar wToolBar=(ToolBar)GetFrameworkElement(nameToolBar);
			wToolBar.Items.Clear();
		}
		#endregion ToolBar

		#region ValidTextBox
		public void ValidDate_SetValue(string controlName,DateTime date) {
			ValidDate validDate=(ValidDate)GetControl(controlName);
			if(UIManager_.IsDpiSystem){
				validDate.Value=date;
				return;
			}
			Proxy proxy=GetProxy(controlName);
			((Wui.TextBoxValidDate)proxy.FrameworkElement_).Value=date;
		}
		#endregion ValidTextBox

		#region private
		private Proxy GetProxy(string controlName){
			Proxy proxy=UIManager_.ListProxies.Find(x=>x.ControlCopy!=null && x.ControlCopy.Name==controlName);
			if(proxy is null){
				throw new Exception("controlName not found: "+controlName);
			}
			return proxy;
		}

		private System.Windows.Forms.Control GetControl(string controlName){
			if(UIManager_.IsDpiSystem){
				List<System.Windows.Forms.Control> listControls=CodeBase.UIHelper.GetAllControls(UIManager_.FormODBase_).ToList();
				System.Windows.Forms.Control control=listControls.Find(x=>x.Name==controlName);
				if(control is null){//must have misspelled it.
					throw new Exception("controlName not found: "+controlName);
				}
				return control;
			}
			Proxy proxy=UIManager_.ListProxies.Find(x=>x.ControlCopy!=null && x.ControlCopy.Name==controlName);
			if(proxy is null){
				throw new Exception("controlName not found: "+controlName);
			}
			return proxy.ControlCopy;
		}

		///<summary>Used for fields that are not controls. Example: contextMenuStrip.</summary>
		private Object GetComponent(string name){
			Type typeForm=UIManager_.FormODBase_.GetType();
			List<FieldInfo> listFieldInfos=typeForm.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
			FieldInfo fieldInfo=listFieldInfos.Find(x=>x.Name==name);
			if(fieldInfo is null){
				throw new Exception("name not found: "+name);
			}
			Object objectRet=fieldInfo.GetValue(UIManager_.FormODBase_);
			return objectRet;
		}

		private FrameworkElement GetFrameworkElement(string controlName){
			Proxy proxy=UIManager_.ListProxies.Find(x=>x.ControlCopy!=null && x.ControlCopy.Name==controlName);
			//already checked for bad controlName
			return proxy.FrameworkElement_;
		}
		#endregion private

	}*/
}
