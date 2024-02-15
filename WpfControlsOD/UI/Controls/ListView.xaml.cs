using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;//even though they are in this project

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the ListView control:
The purpose of this control it to let the user pick from thumbnails.
The other suggested uses for a normal WPF ListView, like GridView, are not supported by this ListView.
Only supports a single selected index.

*/
	///<summary></summary>
	public partial class ListView : UserControl{
		#region Fields - Public
		//private List<ListViewItem> _listListViewItems=new List<ListViewItem>();
		#endregion Fields - Public

		#region Fields -Private
		///<summary>The index within _listListViewItems which is lit up by the hover effect.</summary>
		private int _hoverIndex=-1;
		private int _selectedIndex=-1;
		#endregion Fields -Private

		#region Constructor
		public ListView(){
			InitializeComponent();
		}
		#endregion Constructor

		#region Events
		
		#endregion Events

		#region Enums
		
		#endregion Enums

		#region Properties - Not Browsable
		public int SelectedIndex{
			//Named after the Winforms property
			get{
				return _selectedIndex;
			}
			set{
				_selectedIndex=value;
				SetColors();
			}
		}
		#endregion Properties - Not Browsable

		#region Methods - Public
		public void AddItem(ListViewItem listViewItem){
			//_listListViewItems.Add(listViewItem);
			Border border=new Border();
			border.Tag=listViewItem;
			border.Width=110;
			border.Height=100+18+10;
			System.Windows.Controls.Grid grid=new System.Windows.Controls.Grid();
			RowDefinition rowDefinition;
			rowDefinition=new RowDefinition();
			rowDefinition.Height=new GridLength(5+100+5);//thumbnail
			grid.RowDefinitions.Add(rowDefinition);
			rowDefinition=new RowDefinition();
			rowDefinition.Height=new GridLength(1,GridUnitType.Star);
			grid.RowDefinitions.Add(rowDefinition);
			border.Child=grid;
			Image image=new Image();
			image.Source=listViewItem.BitmapImage_;
			image.Width=100;
			image.Height=100;
			image.Margin=new Thickness(5);
			grid.Children.Add(image);
			TextBlock textBlock=new TextBlock();
			System.Windows.Controls.Grid.SetRow(textBlock,1);
			textBlock.Text=listViewItem.Text;
			textBlock.VerticalAlignment=VerticalAlignment.Top;
			textBlock.HorizontalAlignment=HorizontalAlignment.Center;
			grid.Children.Add(textBlock);
			border.MouseLeave+=Item_MouseLeave;
			border.MouseLeftButtonDown+=Item_MouseLeftButtonDown;
			border.MouseMove+=Item_MouseMove;
			wrapPanel.Children.Add(border);
		}
		#endregion Methods - Public

		#region Methods private
		///<summary>Sets background colors for all rows, based on selected indices and hover.</summary>
		private void SetColors(){
			for(int i=0;i<wrapPanel.Children.Count;i++){
				Border border=(Border)wrapPanel.Children[i];
				//ListViewItem listViewItem=(ListViewItem)border.Tag;
				Color colorBack=Colors.White;
				if(i==_selectedIndex){
					colorBack=Color.FromRgb(180,210,255);//this color is specific to this control
				}
				else if(i==_hoverIndex){
					colorBack=Color.FromRgb(235,244,255);
				}
				border.Background=new SolidColorBrush(colorBack);
			}
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private void Item_MouseLeave(object sender, MouseEventArgs e){
			if(_hoverIndex!=-1){
				_hoverIndex=-1;
				SetColors();
			}
			//This doesn't fire when mouse moves out of bounds while dragging.
			//It does fire when button is released, after the mouse up, but that usually doesn't help.
		}

		///<summary></summary>
		private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e){
			Border border=(Border)sender;
			int idx=wrapPanel.Children.IndexOf(border);
			_selectedIndex=idx;
			SetColors();
		}

		private void Item_MouseMove(object sender, MouseEventArgs e){
			Border border=(Border)sender;
			int idx=wrapPanel.Children.IndexOf(border);
			if(_hoverIndex!=idx){
				_hoverIndex=idx;
				SetColors();
			}
		}
		#endregion Methods private EventHandlers
	}

	public class ListViewItem{
		public string Text;
		public BitmapImage BitmapImage_;
	}
}
