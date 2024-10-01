using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using OpenDentBusiness;
using CodeBase;
using NHunspell;
using OpenDental;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the TreeView control:
-We now use the WPF terminology for treeViews.
-Instead of TreeNodes, use TreeViewItems.
-The collection is called Items instead of Nodes.
-BeginUpdate and EndUpdate are useless. Just delete them.
-We have wrapped both the native WPF TreeView and the native WPF TreeViewItem, so if any method or property is missing, talk to Jordan about adding it.

*/
	///<summary></summary>
	public partial class TreeView : UserControl{
		#region Fields
		
		#endregion Fields

		#region Constructor
		public TreeView(){
			InitializeComponent();
			//Width=200;
			//Height=400;
			Items=new TreeViewItemCollection(this);
		}
		#endregion Constructor

		#region Events
		
		#endregion Events

		#region Properties
		///<summary>Replacement for SelectedNode.</summary>
		public TreeViewItem SelectedItem{
			get{
				if(treeView.SelectedItem is null){
					return null;
				}
				return (TreeViewItem)treeView.SelectedItem;
			}
			set{
				//MS doesn't include a setter, so we must loop through a flat list of all children
				List<TreeViewItem> listTreeViewItems=Items.GetAllFlat();
				for(int i=0;i<listTreeViewItems.Count;i++){
					if(value is null){
						listTreeViewItems[i].IsSelected=false;
						continue;
					}
					if(value==listTreeViewItems[i]){
						listTreeViewItems[i].IsSelected=true;
						continue;
					}
					listTreeViewItems[i].IsSelected=false;
				}
			}
		}

		///<summary>Replacement for Nodes.</summary>
		public TreeViewItemCollection Items { 
			get;
		} 
		#endregion Properties

		#region Methods - public
		
		#endregion Methods - public

		#region Methods - private event handlers
		
		#endregion Methods - private event handlers

		#region Methods - private
		
		#endregion Methods - private

		#region Classes nested
		///<summary>Nested class for the collection of treeViewItems.</summary>
		public class TreeViewItemCollection{
			///<summary>The treeView that this collection is attached to.</summary>
			private TreeView _treeViewParent;

			public TreeViewItemCollection(TreeView treeViewParent){
				_treeViewParent=treeViewParent;
			}

			public void Add(TreeViewItem treeViewItem){
				_treeViewParent.treeView.Items.Add(treeViewItem);
			}

			public void Clear(){
				_treeViewParent.treeView.Items.Clear();
			}

			public int Count{
				get{
					return _treeViewParent.treeView.Items.Count;
				}
			}

			///<summary>Gets a flat list of all TreeViewItems.</summary>
			public List<TreeViewItem> GetAllFlat(){
				List<TreeViewItem> listTreeViewItems=new List<TreeViewItem>();
				for(int i=0;i<_treeViewParent.treeView.Items.Count;i++){
					listTreeViewItems.Add((TreeViewItem)_treeViewParent.treeView.Items[i]);
					listTreeViewItems.AddRange(GetAllChildren((TreeViewItem)_treeViewParent.treeView.Items[i]));
				}
				return listTreeViewItems;
			}

			///<summary>Recursive. Gets all children in a flat list.</summary>
			private List<TreeViewItem> GetAllChildren(TreeViewItem treeViewItem){
				List<TreeViewItem> listTreeViewItems=new List<TreeViewItem>();
				//do not include self
				for(int i=0;i<treeViewItem.Items.Count;i++){
					listTreeViewItems.Add((TreeViewItem)treeViewItem.Items[i]);
					listTreeViewItems.AddRange(GetAllChildren((TreeViewItem)treeViewItem.Items[i]));
				}
				return listTreeViewItems;
			}

			///<summary></summary>
			public TreeViewItem this[int index]{
				get { 
					return (TreeViewItem)_treeViewParent.treeView.Items[index];
				}
			}
		}
		#endregion Classes nested
	}

	///<summary>Replacement for TreeNode.</summary>
	public class TreeViewItem: System.Windows.Controls.TreeViewItem{
		private EnumIcons _icon;
		private System.Windows.Controls.Grid gridImage;
		private TextBlock textBlock;

		public TreeViewItem(){
			System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
			Header=grid;
			ColumnDefinition columnDefinition;
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(1,GridUnitType.Auto);//image. Width will be zero if no image.
			grid.ColumnDefinitions.Add(columnDefinition);
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
			grid.ColumnDefinitions.Add(columnDefinition);
			gridImage = new System.Windows.Controls.Grid();
			gridImage.VerticalAlignment=VerticalAlignment.Center;
			grid.Children.Add(gridImage);
			textBlock = new TextBlock();
			textBlock.VerticalAlignment=VerticalAlignment.Center;
			grid.Children.Add(textBlock);
			System.Windows.Controls.Grid.SetColumn(textBlock,1);
		}

		public EnumIcons Icon{
			get{
				return _icon;
			}
			set{
				_icon=value;
				gridImage.Children.Clear();
				if(_icon==EnumIcons.None){
					gridImage.Margin=new Thickness(0);
					return;
				}
				int width=16;
				gridImage.Width=width;
				gridImage.Height=width;
				IconLibrary.DrawWpf(_icon,gridImage);
				gridImage.Margin=new Thickness(0,0,right:3,0);
			}
		}

		public string Text{
			get{
				return textBlock.Text;
			}
			set{
				textBlock.Text=value;
			}
		}
	}
}

