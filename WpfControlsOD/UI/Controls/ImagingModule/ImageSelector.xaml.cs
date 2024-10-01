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

How to use the ImageSelector control:
Only used once in Imaging module.

*/
	///<summary></summary>
	public partial class ImageSelector : UserControl{
		#region Fields - Public
		#endregion Fields - Public

		#region Fields -Private
		private ContextMenu contextMenu;
		private Cursor _cursorDrag;
		private DispatcherTimer _dispatcherTimerTreeClick;
		///<summary>If user drags outside the tree and then back in, Windows fires the DragDrop event which we don't want. This variable handles this edge case.</summary>
		private bool _hasDraggedOutsideTheTree;
		///<summary>The index within _listNodeObjTags which is lit up by the hover effect.</summary>
		private int _hoverIndex=-1;
		///<summary>If a msgbox is showing, this gets set to true to prevent the timer from refreshing the selection.</summary>
		private bool _isDragPendingMsgBox;
		///<summary>The only reason we care about mouseDown is for initiating a drag.  Right mouse cannot initiate a drag.</summary>
		private bool _isLeftMouseDownDragging;
		///<summary>This list is passed in.</summary>
		private List<Def> _listDefsImageCats;
		///<summary>This keeps track of which nodes should remain expanded as the user moves between patients, etc.  Sometimes, a node has no children, so it cannot expand, but that will not change the state of this list.  The only actions which affect this list are Collapse All, Expand All, clicking on the +/- buttons, and SetSelected (which expands so that the selected can be seen).  This list is set to all expanded upon startup.</summary>
		private List<long> _listDefNumsExpanded=null;
		///<summary>This is the list of items to draw, including the categories and collapsed items.</summary>
		private List<NodeObjTag> _listNodeObjTags=new List<NodeObjTag>();
		///<summary>Do not compare equivalency.  Only compare type and primary key. Null indicates no selection.  It's a full copy of the original object.</summary>
		private NodeObjTag _nodeObjTagSelected;
		private Patient _patient=null;
		private string _patFolder;
		private ToolTip _toolTip;
		///<summary>Tracks the last user to load ContrImages</summary>
		private long _userNumPrev=-1;

		#endregion Fields -Private

		#region Constructor
		public ImageSelector(){
			InitializeComponent();
			FontSize=11.5;
			FontFamily=new FontFamily("Segoe UI");
			_dispatcherTimerTreeClick=new DispatcherTimer();
			_dispatcherTimerTreeClick.Interval = TimeSpan.FromSeconds(1);
			_dispatcherTimerTreeClick.Tick += new System.EventHandler(dispatcherTimerTreeClick_Tick);
			contextMenu=new ContextMenu(this);//but not assigning it to this control because we want to make it show manually
			MenuItem menuItem=new MenuItem("Print",menuItem_Click);
			menuItem.Name="Print";
			contextMenu.Items.Add(menuItem);
			menuItem=new MenuItem("Delete",menuItem_Click);
			menuItem.Name="Delete";
			contextMenu.Items.Add(menuItem);
			menuItem=new MenuItem("Info",menuItem_Click);
			menuItem.Name="Info";
			contextMenu.Items.Add(menuItem);
			menuItem=new MenuItem("Find Tasks",menuItem_Click);
			menuItem.Name="Tasks";
			contextMenu.Items.Add(menuItem);
			_cursorDrag=new Cursor(new MemoryStream(Properties.Resources.CursorDrag));
			_toolTip=new ToolTip();
			_toolTip.SetControlAndAction(this,ToolTipSetString);
		}
		#endregion Constructor

		#region Events
		///<summary>Occurs after user drags file(s) from a Windows folder into the image selector.</summary>
		[Category("OD")]
		[Description("Occurs after user drags file(s) from a Windows folder into the image selector.")]
		public event EventHandler<DragDropImportEventArgs> DragDropImport;

		///<summary>Occurs after user drags an item to a different category.</summary>
		[Category("OD")]
		[Description("Occurs after user drags an item to a different category.")]
		public event EventHandler<DragEventArgsOD> DraggedToCategory;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when an item is double clicked.")]
		public event EventHandler ItemDoubleClick;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs one second after the current item is clicked again. If the click turns into a double click, then this does not fire. This allows resetting of pan and zoom.")]
		public event EventHandler ItemReselected;

		///<summary>Occurs when user clicks a menu item from the context menu.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks a menu item from the context menu.")]
		public event EventHandler MenuClick;

		///<summary>Occurs when user selects item. Fires even if index does not change.  This allows a refresh action by clicking.  Also fires when a collapse causes deselection.</summary>
		[Category("OD")]
		[Description("Occurs when user selects item. Fires even if index does not change.  This allows a refresh action by clicking.  Also fires when a collapse causes deselection.")]
		public event EventHandler SelectionChangeCommitted;
		#endregion Events

		#region Enums
		///<summary>The Icon to show for the row, like folder or xray.</summary>
		private enum EnumIconShow {
			///<summary>0</summary>  
			Folder,
			///<summary>1</summary>  
			Doc,
			///<summary>2</summary>
			Xray,
			///<summary>3</summary>
			Picture,
			///<summary>4</summary>
			File,
			///<summary>5</summary>
			Mount,
			///<summary>6</summary>
			FolderWeb
		}
		#endregion Enums

		#region Properties - Not Browsable
		///<summary>Set to false to hide the right click context menu.</summary>
		[Browsable(false)]
		public bool IsContextVisible {get;set; } =true;

		///<summary>Set false to disable the Print option in the right click context menu. True by default.</summary>
		[Browsable(false)]
		[DefaultValue(true)]
		public bool PrintOptionEnabled {get;set;} = true;

		///<summary>Gets or sets the value of the vertical scrollbar.  Does all error checking and invalidates.</summary>
		[Browsable(false)]
		[DefaultValue(0)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollValue {
			get {
				return (int)scrollViewer.VerticalOffset;
			}
			set {
				scrollViewer.ScrollToVerticalOffset(value);
			}
		}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}
		#endregion Properties - Not Browsable

		#region Methods - Public
		///<summary>Clear all images and categories in the case that we switch to a different user and the patient is no longer selected.</summary>
		public void ClearAll() {
			_listNodeObjTags.Clear();
			ComputeRows();
			SetColors();
		}

		///<summary>Also clears any selected item, but categories remain selected.</summary>
		public void CollapseAll(){
			if(_listDefNumsExpanded==null){
				return;
			}
			_listDefNumsExpanded.Clear();
			if(_nodeObjTagSelected!=null){
				if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category){
					//leave categories selected because they won't disappear.
				}
				else{
					_nodeObjTagSelected=null;
				}
			}
			ComputeRows();
			ScrollValue=0;
			SetColors();
		}

		public void ExpandAll(){
			if(_listDefNumsExpanded==null){
				return;
			}
			_listDefNumsExpanded.Clear();
			for(int i=0;i<_listDefsImageCats.Count;i++){
				_listDefNumsExpanded.Add(_listDefsImageCats[i].DefNum);
			}
			ComputeRows();
			SetColors();
		}

		///<summary>Gets the DefNum category of the current selection. The current selection can be a folder itself, or a document within a folder. If nothing selected, then it returns the DefNum of DefaultImageCategoryImportFolder pref if one exists, DefNum of the first category in the list otherwise.</summary>
		public long GetSelectedCategory(){
			long imageCategoryDefault=PrefC.GetLong(PrefName.ImageCategoryDefault);
			if(imageCategoryDefault==0) {//No default Image Category set.
				imageCategoryDefault=Defs.GetDefsForCategory(DefCat.ImageCats,true)[0].DefNum;
			}
			if(_nodeObjTagSelected==null){
				return imageCategoryDefault;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document || _nodeObjTagSelected.NodeType==EnumImageNodeType.Mount){
				return _nodeObjTagSelected.DocCategory;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category) {
				return _nodeObjTagSelected.Def.DefNum;
			}
			return imageCategoryDefault;
		}

		///<summary>Usually used after GetSelectedType so that you know what kind of key it is.  If nothing selected, then returns 0.</summary>
		public long GetSelectedKey(){
			if(_nodeObjTagSelected==null){
				return 0;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category){
				return _nodeObjTagSelected.Def.DefNum;
			}
			else if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document){
				return _nodeObjTagSelected.DocNum;
			}
			else if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Mount){
				return _nodeObjTagSelected.MountNum;
			}
			return 0;
		}

		///<summary></summary>
		public EnumImageNodeType GetSelectedType(){
			if(_nodeObjTagSelected==null){
				return EnumImageNodeType.None;
			}
			return _nodeObjTagSelected.NodeType;
		}

		///<summary>This is only used when an item is deleted.  We should switch selection to the item above the deleted item, whether that be another item or a category. Guaranteed to work if starting with a doc/mount because another doc/mount or a category is always above it.</summary>
		public NodeTypeAndKey GetItemAbove(){
			int idxNow=_listNodeObjTags.IndexOf(_listNodeObjTags.FirstOrDefault(x=>x.IsMatching(_nodeObjTagSelected)));
			return new NodeTypeAndKey(_listNodeObjTags[idxNow-1].NodeType,_listNodeObjTags[idxNow-1].GetPriKey());
		}

		///<summary>Loads the expanded image categories for the user on start up or when changing users.</summary>
		public void LoadExpandedPrefs() {
			if(_userNumPrev==Security.CurUser.UserNum) {
				return;//User has not changed.  Maintain expanded nodes.
			}
			CollapseAll();//Start with collapsed nodes
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ImageCategoryExpanded);//Update override list.
			for(int i=0;i<_listDefsImageCats.Count;i++) {
				//Should only be one value with associated Fkey.
				UserOdPref userOdPrefTemp=listUserOdPrefs.FirstOrDefault(x => x.Fkey==_listDefsImageCats[i].DefNum);
				if(userOdPrefTemp!=null) {//User has a expanded preference for this image category.
					if(!_listDefNumsExpanded.Contains(_listDefsImageCats[i].DefNum)) {
						_listDefNumsExpanded.Add(_listDefsImageCats[i].DefNum);
					}				
				}
			}
			_userNumPrev=Security.CurUser.UserNum;//Update the Previous user num.
			ComputeRows();
		}

		public void SetCategories(List<Def> listDefsImageCats){
			_listDefsImageCats=listDefsImageCats;
			if(_listDefNumsExpanded!=null){
				return;
			}
			//initialize first time when opening OD
			_listDefNumsExpanded=new List<long>();
			for(int i=0;i<_listDefsImageCats.Count;i++) {
				_listDefNumsExpanded.Add(_listDefsImageCats[i].DefNum);
			}
		}

		///<summary></summary>
		public void SetData(Patient patient,DataTable table,bool keepSelection,string patFolder){
			_patient=patient;
			_patFolder=patFolder;
			_listNodeObjTags=new List<NodeObjTag>();
			//Categories--------------------------------------------------------------------
			for(int i=0;i<_listDefsImageCats.Count;i++) {
				NodeObjTag nodeObjTag=new NodeObjTag(_listDefsImageCats[i]);
				nodeObjTag.IconShow=EnumIconShow.Folder;
				if(_listDefsImageCats[i].ItemValue.Contains("L")) { 
					nodeObjTag.IconShow=EnumIconShow.FolderWeb;
				}
				if(_listDefsImageCats[i].ItemValue.Contains("M")) {
					nodeObjTag.IsThumbnail=true;
				}
				_listNodeObjTags.Add(nodeObjTag);
			}
			//Docs, etc--------------------------------------------------------------------------
			for(int i=0;i<table.Rows.Count;i++){
				NodeObjTag nodeObjTag=new NodeObjTag(table.Rows[i]);
				nodeObjTag.IconShow=EnumIconShow.Doc;
				if(nodeObjTag.NodeType==EnumImageNodeType.Mount){
					nodeObjTag.IconShow=EnumIconShow.Mount;
				}
				else{//document
					//doc already handled
					//attachment wasn't handled properly in previous versions.  We'll leave it as a doc.
					if(nodeObjTag.ImgType==ImageType.File){
						nodeObjTag.IconShow=EnumIconShow.File;
					}
					if(nodeObjTag.ImgType==ImageType.Photo){
						nodeObjTag.IconShow=EnumIconShow.Picture;
					}
					if(nodeObjTag.ImgType==ImageType.Radiograph){
						nodeObjTag.IconShow=EnumIconShow.Xray;
					}
				}
				NodeObjTag nodeObjTagParent=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==nodeObjTag.DocCategory);
				if(nodeObjTagParent==null){
					continue;//should never happen
				}
				if(nodeObjTagParent.Def.ItemValue.Contains("M")) {
					nodeObjTag.IsThumbnail=true;
				}
				int idxInsert=_listNodeObjTags.IndexOf(nodeObjTagParent)+nodeObjTagParent.CountInCategory+1;
				_listNodeObjTags.Insert(idxInsert,nodeObjTag);
				nodeObjTagParent.CountInCategory++;
				//nodeObjTagParent.ExpandCollapse=NodeObjTag.EnumExpandCollapse.Plus;//start out collapsed
				//later:
				//if(treeNode.Tag.Equals(nodeObjTagSelection)) {
				//	SelectTreeNode((NodeObjTag)treeNode.Tag);
				//}
			}
			if(_nodeObjTagSelected!=null && _nodeObjTagSelected.NodeType==EnumImageNodeType.Category){
				_nodeObjTagSelected=_listNodeObjTags.FirstOrDefault(x=>x.IsMatching(_nodeObjTagSelected));
				//if a category was selected, this keeps it selected
			}
			else if(keepSelection){
				//Make sure selection exists. Works with null.
				//Category might have changed from within info window. This pulls updated version. Null ok.
				_nodeObjTagSelected=_listNodeObjTags.FirstOrDefault(x=>x.IsMatching(_nodeObjTagSelected));
			}
			else{
				_nodeObjTagSelected=null;
			}
			ComputeRows();
			scrollViewer.ScrollToVerticalOffset(0);
		}

		///<summary>Similar to the LoadExpandedPrefs() method, but instead of looking at the user prefs we can pass in a specific list of categories to expand.</summary>
		public void SetExpandedCategories(List<long> listDefNumsToExpand) {
			CollapseAll();
			for(int i=0;i<listDefNumsToExpand.Count;i++) {
				if(!_listDefNumsExpanded.Contains(listDefNumsToExpand[i])) {
					_listDefNumsExpanded.Add(listDefNumsToExpand[i]);
				}	
			}
			ComputeRows();
		}

		public void SetSelected(EnumImageNodeType nodeType,long primaryKey){
			NodeObjTag nodeObjTag=null;
			if(nodeType==EnumImageNodeType.Category){
				nodeObjTag=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==nodeType && x.Def.DefNum==primaryKey);
			}
			else if(nodeType==EnumImageNodeType.Document){
				nodeObjTag=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==nodeType && x.DocNum==primaryKey);
			}
			else if(nodeType==EnumImageNodeType.Mount){
				nodeObjTag=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==nodeType && x.MountNum==primaryKey);
			}
			if(nodeObjTag is null){
				_nodeObjTagSelected=null;
				SetColors();
				return;
			}
			_nodeObjTagSelected=nodeObjTag.Copy();//We want to keep this even when the list gets cleared
			//expand the category node if it's collapsed
			NodeObjTag nodeObjTagParent=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==nodeObjTag.DocCategory);
			if(nodeObjTagParent is null){
				SetColors();
				return;//unknown error
			}
			if(nodeObjTagParent.ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
				//nodeObjTagParent.ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;
				_listDefNumsExpanded.Add(nodeObjTagParent.Def.DefNum);
				ComputeRows();
			}
			SetColors();
		}

		///<summary>Sets the thumbnail showing for the current item. Supply 100x100 bitmap. Only needed when current thumbnail needs to be updated.</summary>
		public void SetThumbnail(System.Drawing.Bitmap bitmap){
			if(_nodeObjTagSelected==null){
				return;
			}
			NodeObjTag nodeObjTag=_listNodeObjTags.Find(x=>x.IsMatching(_nodeObjTagSelected));
			if(nodeObjTag==null){
				return;
			}
			if(!nodeObjTag.IsThumbnail){
				return;
			}
			int index=_listNodeObjTags.IndexOf(nodeObjTag);
			Border border=BorderFromIdx(index);
			if(border is null){
				return;
			}
			System.Windows.Controls.Grid grid=(System.Windows.Controls.Grid)border.Child;
			Image image=(Image)grid.Children.Cast<FrameworkElement>().FirstOrDefault(x=>x.GetType()==typeof(Image));
			if(image is null){
				return;
			}
			BitmapImage bitmapImage =null;
			Document document=Documents.GetByNum(nodeObjTag.DocNum);
			using MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
			memoryStream.Position = 0;
			bitmapImage=new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;//makes it load into memory during EndInit
			bitmapImage.EndInit();
			//bitmapImage.Freeze(); //for use in another thread
			image.Source=bitmapImage;
		}
		#endregion Methods - Public

		#region Methods private
		/// <summary>Pass in an index within _listNodeObjTags.</summary>
		private Border BorderFromIdx(int idx){
			for(int i=0;i<stackPanel.Children.Count;i++){
				if(stackPanel.Children[i] is Border border){
					NodeObjTag nodeObjTag=border.Tag as NodeObjTag;
					if(nodeObjTag is null){
						return null;//won't ever happen
					}
					int index=_listNodeObjTags.IndexOf(nodeObjTag);
					if(index==idx){
						return border;
					}
				}
				if(stackPanel.Children[i] is WrapPanel wrapPanel){
					for(int w=0;w<wrapPanel.Children.Count;w++){
						Border borderW=(Border)wrapPanel.Children[w];
						NodeObjTag nodeObjTag=borderW.Tag as NodeObjTag;
						if(nodeObjTag is null){
							return null;//won't ever happen
						}
						int index=_listNodeObjTags.IndexOf(nodeObjTag);
						if(index==idx){
							return borderW;
						}
					}
				}
			}
			return null;
		}

		private Border BorderFromPoint(Point point){
			Point pointScrollViewer=TranslatePoint(point,scrollViewer);//translate from ImageSelector to ScrollViewer (there are buttons above scrollviewer
			double offset=scrollViewer.VerticalOffset;//example 12 if scrolled down one line
			Point pointRelativeToStack=new Point(pointScrollViewer.X,pointScrollViewer.Y+offset);//example, yRelativeToThis=5, so yRelativeToStack=17.
			for(int i=0;i<stackPanel.Children.Count;i++){
				if(stackPanel.Children[i] is Border border){
					Point pointRelativeToChild=stackPanel.TranslatePoint(pointRelativeToStack,border);//translates from stackPanel to border
					//Example. For the second item, pointRelativeToChild=5, which is within this item.
					if(pointRelativeToChild.Y < 0){
						continue;
					}
					if(pointRelativeToChild.Y > border.ActualHeight){
						continue;
					}
					return border;
				}
				if(stackPanel.Children[i] is WrapPanel wrapPanel){
					for(int w=0;w<wrapPanel.Children.Count;w++){
						Border borderW=(Border)wrapPanel.Children[w];
						Point pointRelativeToChild=stackPanel.TranslatePoint(pointRelativeToStack,borderW);//translates from stackPanel to border
						if(pointRelativeToChild.X < 0 || pointRelativeToChild.Y < 0){
							continue;
						}
						if(pointRelativeToChild.X > borderW.ActualWidth || pointRelativeToChild.Y > borderW.ActualHeight){
							continue;
						}
						return borderW;
					}
				}
			}
			return null;
		}

		///<summary>This is the only place where the ExpandCollapse status of each category gets set.  Then, it computes Xpos, Ypos, Width, and Height of each element, and _heightTotal.  Called when new data is set, after user clicks expand/collapse on any category, or if control is resized.</summary>
		private void ComputeRows(){
			stackPanel.Children.Clear();
			for(int i=0;i<_listNodeObjTags.Count;i++){
				if(_listNodeObjTags[i].NodeType!=EnumImageNodeType.Category){
					continue;
				}
				ComputeRowCategory(_listNodeObjTags[i]);
				List<NodeObjTag> listNodeObjTagsDocs=_listNodeObjTags.FindAll(x=>x.NodeType!=EnumImageNodeType.Category && x.DocCategory==_listNodeObjTags[i].Def.DefNum);
				if(_listNodeObjTags[i].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
					//don't show
					continue;
				}
				if(_listNodeObjTags[i].IsThumbnail){
					ComputeRowsThumbs(listNodeObjTagsDocs);
				}
				else{ 
					ComputeRowsDocs(listNodeObjTagsDocs);
				}
			}
			SetColors();
		}

		private void ComputeRowCategory(NodeObjTag nodeObjTag){
			Border border=new Border();
			border.Height=20;
			border.Tag=nodeObjTag;
			bool hasChildren=_listNodeObjTags.Exists(x=>x.NodeType!=EnumImageNodeType.Category && x.DocCategory==nodeObjTag.Def.DefNum);
			if(hasChildren){
				if(_listDefNumsExpanded!=null && _listDefNumsExpanded.Exists(x=>x==nodeObjTag.Def.DefNum)){
					nodeObjTag.ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;//expanded
				}
				else{
					nodeObjTag.ExpandCollapse=NodeObjTag.EnumExpandCollapse.Plus;
				}
			}
			else{
				nodeObjTag.ExpandCollapse=NodeObjTag.EnumExpandCollapse.None;
			}
			System.Windows.Controls.Grid grid=new System.Windows.Controls.Grid();
			ColumnDefinition columnDefinition;
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(18);//+/-
			grid.ColumnDefinitions.Add(columnDefinition);
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(19);//icon
			grid.ColumnDefinitions.Add(columnDefinition);
			columnDefinition=new ColumnDefinition();
			columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
			grid.ColumnDefinitions.Add(columnDefinition);
			if(nodeObjTag.ExpandCollapse!=NodeObjTag.EnumExpandCollapse.None){//plus or minus
				Canvas canvas=new Canvas();
				//canvas.SnapsToDevicePixels=true;//Doesn't work in ElementHost or at different zooms, so skip
				canvas.VerticalAlignment=VerticalAlignment.Center;
				canvas.HorizontalAlignment=HorizontalAlignment.Center;
				grid.Children.Add(canvas);
				Rectangle rectangle=new Rectangle();
				rectangle.Width=12;
				rectangle.Height=12;
				Canvas.SetLeft(rectangle,-6);
				Canvas.SetTop(rectangle,-6);
				rectangle.Fill=Brushes.White;
				rectangle.Stroke=Brushes.Black;
				rectangle.StrokeThickness=1;
				rectangle.RadiusX=1.5;
				rectangle.RadiusY=1.5;
				canvas.Children.Add(rectangle);
				Line lineH=new Line();
				lineH.X1=-4;
				lineH.X2=4;
				lineH.Stroke=Brushes.Black;
				canvas.Children.Add(lineH);
				if(nodeObjTag.ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
					Line lineV=new Line();
					lineV.Y1=-4;
					lineV.Y2=4;
					lineV.Stroke=Brushes.Black;
					canvas.Children.Add(lineV);
				}
			}
			EnumIcons enumIcons=EnumIcons.ImageSelectorFolder;
			if(nodeObjTag.IconShow==EnumIconShow.FolderWeb){
				enumIcons=EnumIcons.ImageSelectorFolderWeb;
			}
			System.Windows.Controls.Grid gridIcon=new System.Windows.Controls.Grid();
			gridIcon.Width=16;
			gridIcon.Height=16;
			gridIcon.HorizontalAlignment=HorizontalAlignment.Left;
			gridIcon.VerticalAlignment=VerticalAlignment.Top;
			System.Windows.Controls.Grid.SetColumn(gridIcon,1);
			grid.Children.Add(gridIcon);
			IconLibrary.DrawWpf(enumIcons,gridIcon);
			TextBlock textBlock=new TextBlock();
			textBlock.Text=nodeObjTag.Description;
			textBlock.Margin=new Thickness(2,0,0,0);
			textBlock.VerticalAlignment=VerticalAlignment.Center;
			System.Windows.Controls.Grid.SetColumn(textBlock,2);
			grid.Children.Add(textBlock);
			Rectangle rectangleLineTop=new Rectangle();
			rectangleLineTop.Height=1;//this is effectively a line, but easier to draw
			rectangleLineTop.VerticalAlignment=VerticalAlignment.Top;
			rectangleLineTop.Fill=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(110));
			System.Windows.Controls.Grid.SetColumnSpan(rectangleLineTop,3);
			grid.Children.Add(rectangleLineTop);
			//lower line probably isn't needed. Individual images will have the own upper line.
			//The only reason we need a lower line is if we have thumbnails below. Work on this later.
			/*
			Rectangle rectangleLineBottom=new Rectangle();
			rectangleLineBottom.Height=1;
			rectangleLineBottom.VerticalAlignment=VerticalAlignment.Bottom;
			rectangleLineBottom.Fill=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(210));
			System.Windows.Controls.Grid.SetColumnSpan(rectangleLineBottom,3);
			grid.Children.Add(rectangleLineBottom);*/
			border.Child=grid;
			border.AllowDrop=true;
			border.Drop+=Item_Drop;
			border.MouseLeave+=Item_MouseLeave;
			border.MouseLeftButtonDown+=Item_MouseLeftButtonDown;//also double click
			border.MouseLeftButtonUp+=Item_MouseLeftButtonUp;
			border.MouseMove+=Item_MouseMove;
			border.MouseRightButtonDown+=Item_MouseRightButtonDown;
			border.MouseWheel+=Item_MouseWheel;
			stackPanel.Children.Add(border);
		}

		private void ComputeRowsDocs(List<NodeObjTag> listNodeObjTags){
			for(int i=0;i<listNodeObjTags.Count;i++){
				Border border=new Border();
				border.Height=22;
				border.Tag=listNodeObjTags[i];
				System.Windows.Controls.Grid grid=new System.Windows.Controls.Grid();
				ColumnDefinition columnDefinition;
				columnDefinition=new ColumnDefinition();
				columnDefinition.Width=new GridLength(20);//icon
				grid.ColumnDefinitions.Add(columnDefinition);
				columnDefinition=new ColumnDefinition();
				columnDefinition.Width=new GridLength(1,GridUnitType.Star);//text
				grid.ColumnDefinitions.Add(columnDefinition);
				EnumIcons enumIcons=EnumIcons.ImageSelectorDoc;
				switch(listNodeObjTags[i].IconShow){
					//case Folder
					case EnumIconShow.Doc:
						enumIcons=EnumIcons.ImageSelectorDoc;
						break;
					case EnumIconShow.Xray:
						enumIcons=EnumIcons.ImageSelectorXray;
						break;
					case EnumIconShow.Picture:
						enumIcons=EnumIcons.ImageSelectorPhoto;
						break;
					case EnumIconShow.File:
						enumIcons=EnumIcons.ImageSelectorFile;
						break;
					case EnumIconShow.Mount:
						enumIcons=EnumIcons.ImageSelectorMount;
						break;
					//case EnumIconShow.FolderWeb:
				}
				System.Windows.Controls.Grid gridIcon=new System.Windows.Controls.Grid();
				gridIcon.Width=16;
				gridIcon.Height=16;
				gridIcon.HorizontalAlignment=HorizontalAlignment.Left;
				gridIcon.VerticalAlignment=VerticalAlignment.Center;
				grid.Children.Add(gridIcon);
				IconLibrary.DrawWpf(enumIcons,gridIcon);
				TextBlock textBlock=new TextBlock();
				System.Windows.Controls.Grid.SetColumn(textBlock,1);
				textBlock.Text=listNodeObjTags[i].DateCreated.ToShortDateString()+": "+listNodeObjTags[i].Description;
				textBlock.Margin=new Thickness(2,0,0,0);
				textBlock.VerticalAlignment=VerticalAlignment.Center;
				grid.Children.Add(textBlock);
				Rectangle rectangleLineTop=new Rectangle();
				rectangleLineTop.Height=1;
				rectangleLineTop.VerticalAlignment=VerticalAlignment.Top;
				rectangleLineTop.Fill=new SolidColorBrush(OpenDental.ColorOD.Gray_Wpf(210));
				System.Windows.Controls.Grid.SetColumnSpan(rectangleLineTop,2);
				grid.Children.Add(rectangleLineTop);
				border.Child=grid;
				border.AllowDrop=true;
				border.DragEnter+=Item_DragEnter;
				border.Drop+=Item_Drop;
				border.MouseLeave+=Item_MouseLeave;
				border.MouseLeftButtonDown+=Item_MouseLeftButtonDown;//also double click
				border.MouseLeftButtonUp+=Item_MouseLeftButtonUp;
				border.MouseMove+=Item_MouseMove;
				border.MouseRightButtonDown+=Item_MouseRightButtonDown;
				border.MouseWheel+=Item_MouseWheel;
				stackPanel.Children.Add(border);
			}
		}

		private void ComputeRowsThumbs(List<NodeObjTag> listNodeObjTags){
			//The entire group of thumbnails goes into one stackpanel cell.
			WrapPanel wrapPanel=new WrapPanel();//this will stretch horizontally to exactly fill the stackpanel cell.
			wrapPanel.Orientation=Orientation.Horizontal;//It will grow vertically as needed to fit the content.
			for(int i=0;i<listNodeObjTags.Count;i++){
				Border border=new Border();
				border.Tag=listNodeObjTags[i];
				border.Width=110;
				border.Height=100+(16*2)+10;
				System.Windows.Controls.Grid grid=new System.Windows.Controls.Grid();
				RowDefinition rowDefinition;
				rowDefinition=new RowDefinition();
				rowDefinition.Height=new GridLength(5+100);//thumbnail
				grid.RowDefinitions.Add(rowDefinition);
				rowDefinition=new RowDefinition();
				rowDefinition.Height=new GridLength(16);//text1
				grid.RowDefinitions.Add(rowDefinition);
				rowDefinition=new RowDefinition();
				rowDefinition.Height=new GridLength(16);//text2
				grid.RowDefinitions.Add(rowDefinition);
				border.Child=grid;
				Image image=new Image();
				//BitmapImage bitmapImage=null;//new BitmapImage();//get from disk
				//image.Source=bitmapImage;
				image.Width=100;
				image.Height=100;
				image.Margin=new Thickness(5);
				if(listNodeObjTags[i].MountNum>0){
					LoadImageMount(image,listNodeObjTags[i].MountNum);//async, so it doesn't wait
				}
				if(listNodeObjTags[i].DocNum>0){
					LoadImageDoc(image,listNodeObjTags[i].DocNum);//async, so it doesn't wait
				}
				grid.Children.Add(image);
				//until the above is working, I'm using a temp rectangle
				/*Rectangle rectangle=new Rectangle();
				rectangle.Fill=Brushes.Silver;
				rectangle.Width=100;
				rectangle.Height=100;
				rectangle.Margin=new Thickness(5);
				grid.Children.Add(rectangle);*/
				TextBlock textBlock=new TextBlock();
				System.Windows.Controls.Grid.SetRow(textBlock,1);
				textBlock.Text=listNodeObjTags[i].DateCreated.ToShortDateString();
				textBlock.VerticalAlignment=VerticalAlignment.Center;
				textBlock.HorizontalAlignment=HorizontalAlignment.Center;
				grid.Children.Add(textBlock);
				TextBlock textBlock2=new TextBlock();
				textBlock2.Name="textBlockDesc";
				System.Windows.Controls.Grid.SetRow(textBlock2,2);
				textBlock2.Text=listNodeObjTags[i].Description;
				textBlock2.VerticalAlignment=VerticalAlignment.Center;
				textBlock2.HorizontalAlignment=HorizontalAlignment.Center;
				grid.Children.Add(textBlock2);
				border.AllowDrop=true;
				border.DragEnter+=Item_DragEnter;
				border.Drop+=Item_Drop;
				border.MouseLeave+=Item_MouseLeave;
				border.MouseLeftButtonDown+=Item_MouseLeftButtonDown;//also double click
				border.MouseLeftButtonUp+=Item_MouseLeftButtonUp;
				border.MouseMove+=Item_MouseMove;
				border.MouseRightButtonDown+=Item_MouseRightButtonDown;
				border.MouseWheel+=Item_MouseWheel;
				wrapPanel.Children.Add(border);
			}
			stackPanel.Children.Add(wrapPanel);
		}

		///<summary>Supply a border item within this control.  Returns idx within _listNodeObjTags. Never -1.</summary>
		private int IndexFromBorder(Border border){
			NodeObjTag nodeObjTag=border.Tag as NodeObjTag;
			if(nodeObjTag is null){
				return -1;//won't ever happen
			}
			return _listNodeObjTags.IndexOf(nodeObjTag);
		}

		///<summary>Gets the index from within _listNodeObjTags at the specified point. Returns -1 if no index can be selected at that point. Pass in the y pos relative to this control. It can fall outside the control when dragging.</summary>
		private int IndexFromPoint(Point point){
			Border border=BorderFromPoint(point);
			if(border is null){
				return -1;
			}
			return IndexFromBorder(border);
		}

		private async void LoadImageDoc(Image image,long docNum){
			BitmapImage bitmapImage =null;
			await System.Threading.Tasks.Task.Run(()=>{
				Document document=Documents.GetByNum(docNum);
				System.Drawing.Bitmap bitmap=Documents.GetThumbnail(document,_patFolder);
				using MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
				memoryStream.Position = 0;
				bitmapImage=new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;//makes it load into memory during EndInit
				bitmapImage.EndInit();
				bitmapImage.Freeze(); //for use in another thread
			});
			image.Source=bitmapImage;
		}

		private async void LoadImageMount(Image image,long mountNum){
			BitmapImage bitmapImage =null;
			await System.Threading.Tasks.Task.Run(()=>{
				System.Drawing.Bitmap bitmap=Mounts.GetThumbnail(mountNum,_patFolder);
				using MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
				memoryStream.Position = 0;
				bitmapImage=new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;//makes it load into memory during EndInit
				bitmapImage.EndInit();
				bitmapImage.Freeze(); //for use in another thread
			});
			image.Source=bitmapImage;
		}

		///<summary>Save the ExpandAll Categories button setting as a set of user preferences.</summary>
		private void SaveExpandedPrefAll(bool isExpandingAll) {
			//Always delete to remove previous values (prevents duplicates).
			UserOdPrefs.DeleteManyForUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ImageCategoryExpanded);
			if(isExpandingAll) {
				List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				List<UserOdPref> listUserOdPrefs=new List<UserOdPref>();
				for(int i=0;i<listDefs.Count;i++) {
					UserOdPref userPrefCur=new UserOdPref();
					userPrefCur.UserNum=Security.CurUser.UserNum;
					userPrefCur.Fkey=listDefs[i].DefNum;
					userPrefCur.FkeyType=UserOdFkeyType.ImageCategoryExpanded;
					listUserOdPrefs.Add(userPrefCur);
				}
				if(listDefs.Count>0) {
					UserOdPrefs.InsertMany(listUserOdPrefs);
				}
			}
			OpenDental.DataValid.SetInvalid(InvalidType.UserOdPrefs);
		}

		///<summary>Save the Image Category expanded setting as a user preference.</summary>
		private void SaveExpandedPref(long defNum,bool isExpanding) {
			//Always delete to remove previous value (prevents duplicates).
			UserOdPrefs.DeleteForFkey(Security.CurUser.UserNum,UserOdFkeyType.ImageCategoryExpanded,defNum);
			if(isExpanding) {//Insert a row to save an expanded category.
				UserOdPref userPrefCur=new UserOdPref();//Preference to be inserted to override.
				userPrefCur.UserNum=Security.CurUser.UserNum;
				userPrefCur.Fkey=defNum;
				userPrefCur.FkeyType=UserOdFkeyType.ImageCategoryExpanded;
				UserOdPrefs.Insert(userPrefCur);
			}
			OpenDental.DataValid.SetInvalid(InvalidType.UserOdPrefs);
		}

		///<summary>Sets background colors for all rows, based on selected indices and hover.</summary>
		private void SetColors(){
			for(int i=0;i<stackPanel.Children.Count;i++){
				if(stackPanel.Children[i] is Border border){
					int idx=IndexFromBorder(border);
					NodeObjTag nodeObjTag=(NodeObjTag)border.Tag;
					Color colorBack=Colors.White;
					if(nodeObjTag.IsMatching(_nodeObjTagSelected)){//whether category or doc
						colorBack=Color.FromRgb(180,210,255);//this color is specific to this control
					}
					else if(_hoverIndex==idx){
						colorBack=Color.FromRgb(229,239,251);
					}
					else if(nodeObjTag.NodeType==EnumImageNodeType.Category){
						colorBack=OpenDental.ColorOD.Gray_Wpf(240);
					}
					border.Background=new SolidColorBrush(colorBack);
				}
				if(stackPanel.Children[i] is WrapPanel wrapPanel){
					for(int w=0;w<wrapPanel.Children.Count;w++){
						Border borderW=(Border)wrapPanel.Children[w];
						int idx=IndexFromBorder(borderW);
						NodeObjTag nodeObjTag=(NodeObjTag)borderW.Tag;
						Color colorBack=Colors.White;
						if(nodeObjTag.IsMatching(_nodeObjTagSelected)){//whether category or doc
							colorBack=Color.FromRgb(180,210,255);//this color is specific to this control
						}
						else if(_hoverIndex==idx){
							colorBack=Color.FromRgb(229,239,251);
						}
						else if(nodeObjTag.NodeType==EnumImageNodeType.Category){
							colorBack=OpenDental.ColorOD.Gray_Wpf(240);
						}
						borderW.Background=new SolidColorBrush(colorBack);
					}
				}
			}
		}

		///<summary>Does a hit test to determine if over text that is too long to fit.  If so, creates appropriate tooltip text.</summary>
		private void ToolTipSetString(FrameworkElement frameworkElement,Point point) {
			int idx=IndexFromPoint(point);
			//scrollViewer.VerticalOffset>0
			Border border=BorderFromPoint(point);
			if(border is null){
				_toolTip.SetString(this,"");
				return;
			}
			if(idx==-1){
				_toolTip.SetString(this,"");
				return;
			}
			if(!_listNodeObjTags[idx].NodeType.In(EnumImageNodeType.Document,EnumImageNodeType.Mount)){
				_toolTip.SetString(this,"");
				return;
			}
			if(_listNodeObjTags[idx].IsThumbnail){
				System.Windows.Controls.Grid grid2=(System.Windows.Controls.Grid)border.Child;
				TextBlock textBlockDesc=(TextBlock)grid2.Children.Cast<FrameworkElement>().FirstOrDefault(x=>x.Name=="textBlockDesc");
				if(textBlockDesc.ActualWidth <= grid2.ActualWidth){
					_toolTip.SetString(this,"");
					return;
				}
				_toolTip.SetString(this,textBlockDesc.Text);
				return;
			}
			//from here down is not thumbnail
			System.Windows.Controls.Grid grid=(System.Windows.Controls.Grid)border.Child;
			TextBlock textBlock=(TextBlock)grid.Children.Cast<FrameworkElement>().FirstOrDefault(x=>x.GetType()==typeof(TextBlock));
			if(textBlock.ActualWidth <= grid.ColumnDefinitions[1].ActualWidth){
				_toolTip.SetString(this,"");
				return;
			}
			_toolTip.SetString(this,textBlock.Text);//,point);
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private void butCollapse_Click(object sender, EventArgs e){
			CollapseAll();
			SaveExpandedPrefAll(false);
		}

		private void butExpand_Click(object sender, EventArgs e){
			ExpandAll();
			SaveExpandedPrefAll(true);
		}

		private void Item_DragEnter(object sender, DragEventArgs e){
			//WARNING: this will not fire if debugging while VS is in Admin mode
			if(e.Data.GetDataPresent(DataFormats.FileDrop)){
				e.Effects=DragDropEffects.Copy;
			}
			//ignore any other data
			//maybe pay attention to images later?  Although I don't know how you would drag an image in.
		}

		private void Item_Drop(object sender, DragEventArgs e){
			if(_hasDraggedOutsideTheTree) {
				//Ignore the edge case where the drag started here in the image selector.
				_hasDraggedOutsideTheTree=false;
				return;
			}
			int idxItem=IndexFromBorder((Border)sender);
			if(idxItem==-1){
				return;
			}
			if(!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}
			List<string> listFileNames=((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
			if(listFileNames.Count==0){
				return;
			}
			long defNumNew=-1;
			if(_listNodeObjTags[idxItem].NodeType==EnumImageNodeType.Category){
				defNumNew=_listNodeObjTags[idxItem].Def.DefNum;
			}
			if(_listNodeObjTags[idxItem].NodeType==EnumImageNodeType.Document
				|| _listNodeObjTags[idxItem].NodeType==EnumImageNodeType.Mount)
			{
				defNumNew=_listNodeObjTags[idxItem].DocCategory;
			}
			if(defNumNew==-1) {
				return;
			}
			_nodeObjTagSelected=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==defNumNew);
			SetColors();
			SelectionChangeCommitted?.Invoke(this,new EventArgs());
			if(listFileNames.Count==1){
				//no dialog
			}
			else if(!OpenDental.MsgBox.Show(this,OpenDental.MsgBoxButtons.OKCancel,"Copy files into this folder as new documents?")){
				return;
			}
			DragDropImportEventArgs dragDropImportEventArgs=new DragDropImportEventArgs();
			dragDropImportEventArgs.DefNumNew=defNumNew;
			dragDropImportEventArgs.ListFileNames=listFileNames;
			DragDropImport?.Invoke(this,dragDropImportEventArgs);
		}

		private void Item_MouseLeave(object sender, MouseEventArgs e){
			if(_hoverIndex!=-1){
				_hoverIndex=-1;
				SetColors();
			}
			//This doesn't fire when mouse moves out of bounds while dragging.
			//It does fire when button is released, after the mouse up, but that usually doesn't help.
		}

		///<summary>Also handles double click.</summary>
		private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e){
			Border border=(Border)sender;
			int idx=IndexFromBorder(border);
			bool isDoubleClick=e.ClickCount==2;
			if(isDoubleClick){
				_dispatcherTimerTreeClick.IsEnabled=false;
				_isLeftMouseDownDragging=false;//Not dragging if double clicked
				ItemDoubleClick?.Invoke(this,new EventArgs());
				return;
			}
			//single click from here down------------------------------------------------------------------
			_hasDraggedOutsideTheTree=false;
			base.OnMouseDown(e);//I think this is old
			//from here down, clicked on a valid row
			//Expand/Collapse============================================================================================
			if(_listNodeObjTags[idx].NodeType==EnumImageNodeType.Category){
				System.Windows.Controls.Grid grid=(System.Windows.Controls.Grid)border.Child;
				//categories have a grid with 3 columns
				double x=e.GetPosition(border).X;
				bool isFirstCol=x<=grid.ColumnDefinitions[0].ActualWidth;
				if(isFirstCol && _listNodeObjTags[idx].ExpandCollapse!=NodeObjTag.EnumExpandCollapse.None){
					if(_listNodeObjTags[idx].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Minus){
						//Currently expanded, and clicked to collapse
						_listDefNumsExpanded.Remove(_listNodeObjTags[idx].Def.DefNum);
						SaveExpandedPref(_listNodeObjTags[idx].Def.DefNum,false);
						//AfterCollapse?.Invoke(this,_listNodeObjTags[idx].Def.DefNum);
						//_listNodeObjTags[idx].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Plus;
						//collapsing, so none of the children can remain selected
						List<NodeObjTag> _listNodeObjTagChildren=_listNodeObjTags.FindAll(x=>x.DocCategory==_listNodeObjTags[idx].Def.DefNum && x.NodeType!=EnumImageNodeType.Category);
						for(int i=0;i<_listNodeObjTagChildren.Count;i++){
							if(_listNodeObjTagChildren[i].IsMatching(_nodeObjTagSelected)){
								_nodeObjTagSelected=null;
								//SetColors done below
								SelectionChangeCommitted?.Invoke(this,new EventArgs());
							}
						}
					}
					else if(_listNodeObjTags[idx].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
						//Currently collapsed, and clicked to expand
						//AfterExpand?.Invoke(this,_listNodeObjTags[idx].Def.DefNum);
						//_listNodeObjTags[idx].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;
						_listDefNumsExpanded.Add(_listNodeObjTags[idx].Def.DefNum);
						SaveExpandedPref(_listNodeObjTags[idx].Def.DefNum,true);
					}
					ComputeRows();
					SetColors();
					return;//without having selected anything.
				}
			}
			//Select row================================================================================================
			if(_listNodeObjTags[idx].IsMatching(_nodeObjTagSelected)){//On node already selected
				//We refresh on the same node so that user can reset zoom/pan.
				//Can't always refresh immediately because it messes up double click.
				//So we pass off the refresh to a timer that will refresh in one sec.
				//But the timer will be cancelled if the user double clicks.
				//This keeps the UI very fast.
				//But not allowed in cloud storage because they take many seconds and pop up a progress bar that interferes.
				if(CloudStorage.IsCloudStorage){
					_isLeftMouseDownDragging=true;
					return;
				}
				if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document) {
					Document document=Documents.GetByNum(_nodeObjTagSelected.DocNum,doReturnNullIfNotFound:true);
					if(document==null) {
						return;//The document was deleted by another instance of the program.
					}
					if(System.IO.Path.GetExtension(document.FileName).ToLower()==".pdf") {
						_isLeftMouseDownDragging=true;//Still want to be able to drag a PDF file
						return;//no refresh on the same node for pdf
					}
				}
				//if(zoomSlider.Value>1000) {//User clicked many many times on the zoom +.  We need to quickly reset the zoom for them.
				//	timerTreeClick.Interval=1;
				//}
				//else {
				_dispatcherTimerTreeClick.Interval=TimeSpan.FromSeconds(1);
				//}
				_dispatcherTimerTreeClick.IsEnabled=true;
			}
			else {//new node
				_nodeObjTagSelected=_listNodeObjTags[idx].Copy();
				SetColors();
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
			}
			//Drag========================================================================================================
			if(CloudStorage.IsCloudStorage){
				return;//not allowed to drag on a new node. Previously selected node is already handled.
			}
			if(_nodeObjTagSelected is null){
				return;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category){//can't drag a category
				return;
			}
			_isLeftMouseDownDragging=true;
			//However, the drag icon won't show until we actually start moving.
			//And if we don't move very far, nothing happens.			
		}

		private void Item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e){
			Cursor=Cursors.Arrow;
			if(DraggedToCategory==null) {//Dragged to category event handler is not defined. Therefore, this event is intentionally unsupported, like in FormImagePickerPatient.
				return;
			}
			if(!_isLeftMouseDownDragging){
				return;
			}
			//_isLeftMouseDownDragging is actually almost always true.
			//It's really synonymous with isMouseDown, so should nearly always be true.
			_isLeftMouseDownDragging=false;
			Border border=(Border)sender;
			int idx=IndexFromBorder(border);
			if(_nodeObjTagSelected==null 
				|| _nodeObjTagSelected.NodeType==EnumImageNodeType.None
				|| _nodeObjTagSelected.NodeType==EnumImageNodeType.Category)
			{
				return;
			}
			long defNumNew=-1;
			if(_listNodeObjTags[idx].NodeType==EnumImageNodeType.Category){
				defNumNew=_listNodeObjTags[idx].Def.DefNum;
			}
			if(_listNodeObjTags[idx].NodeType==EnumImageNodeType.Document
				|| _listNodeObjTags[idx].NodeType==EnumImageNodeType.Mount)
			{
				defNumNew=_listNodeObjTags[idx].DocCategory;
			}
			if(defNumNew==-1 || defNumNew==_nodeObjTagSelected.DocCategory){
				return;
			}
			_isDragPendingMsgBox=true;
			if(!OpenDental.MsgBox.Show(this,OpenDental.MsgBoxButtons.YesNo,"Move to different category?")){
				_isDragPendingMsgBox=false;
				return;
			}
			_isDragPendingMsgBox=false;
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document){
				DragEventArgsOD dragEventArgs=new DragEventArgsOD();
				dragEventArgs.DocNum=_nodeObjTagSelected.DocNum;
				dragEventArgs.DefNumNew=defNumNew;
				//We leave the current item selected.  It will be reselected automatically, based on type and key, and also refreshed.
				DraggedToCategory?.Invoke(this,dragEventArgs);
				SetSelected(EnumImageNodeType.Document,_nodeObjTagSelected.DocNum);
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Mount){
				DragEventArgsOD dragEventArgs=new DragEventArgsOD();
				dragEventArgs.MountNum=_nodeObjTagSelected.MountNum;
				dragEventArgs.DefNumNew=defNumNew;
				DraggedToCategory?.Invoke(this,dragEventArgs);
				SetSelected(EnumImageNodeType.Mount,_nodeObjTagSelected.MountNum);
			}
		}

		private void Item_MouseMove(object sender, MouseEventArgs e){
			Border border=(Border)sender;
			int hoverIndex=IndexFromBorder(border);
			if(_hoverIndex!=hoverIndex){
				_hoverIndex=hoverIndex;
				SetColors();
			}
			if(!_isLeftMouseDownDragging){
				return;
			}
			if(Mouse.LeftButton!=MouseButtonState.Pressed){
				//So _isLeftMouseDownDragging is supposedly true, but the mouse button is not down.
				//This can happen when a dialog comes up when user clicks on an item in this control.
				//So as soon as the mouse goes back to this control, we need to reset this.
				Cursor=Cursors.Arrow;
				_isLeftMouseDownDragging=false;
				return;
			}
			Cursor=_cursorDrag;
			Point point=e.GetPosition(this);
			Rect rect=new Rect(0,0,ActualWidth,ActualHeight);
			if(rect.Contains(point)){//inside bounds
				return;
			}
			//Moved outside of bounds while dragging==========================================================================
			Cursor=Cursors.Arrow;
			_isLeftMouseDownDragging=false;
			_hasDraggedOutsideTheTree=true;
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				return;
			}
			string patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			string[] stringArray=new string[0];
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document){
				Document document=Documents.GetByNum(_nodeObjTagSelected.DocNum);
				string filePath=ODFileUtils.CombinePaths(patFolder,document.FileName);
				if(!File.Exists(filePath)) {
					return;
				}
				stringArray=new string[1];
				stringArray[0]=filePath;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Mount){
				//todo: fire an event that will tell ControlImages to save the entire mount to a tempfile of name that we pass
				//but we only want to do this once for each drag, so we will have to count.
				return;
			}
			DataObject dataObject=new DataObject();
			dataObject.SetData(DataFormats.FileDrop,stringArray);
			FrameworkElement frameworkElement=sender as FrameworkElement;
			DragDrop.DoDragDrop(frameworkElement,dataObject, DragDropEffects.Copy);
		}

		private void Item_MouseRightButtonDown(object sender, MouseButtonEventArgs e){
			Border border=(Border)sender;
			int idx=IndexFromBorder(border);
			if(_listNodeObjTags[idx].NodeType==EnumImageNodeType.Category){
				return;
			}
			if(_listNodeObjTags[idx].IsMatching(_nodeObjTagSelected)){//On node already selected
				//Do nothing because a large image will cause a pause before menu comes up
			}
			else{
				_nodeObjTagSelected=_listNodeObjTags[idx].Copy();
				SetColors();
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
			}
			if(contextMenu!=null//When using FormImagePickerPatient, the ContextMenu control is not initialized so we need to do a null check to prevent errors.
				&& IsContextVisible)
			{
				contextMenu.PlacementTarget=this;
				//Could add Placement, HorizontalOffset and VerticalOffset to control location.
				contextMenu.IsOpen=true;
			}
			for(int i = 0;i<contextMenu.Items.Count;i++) {
				if(contextMenu.Items[i] is MenuItem menuItem && menuItem.Name=="Print") {
					menuItem.IsEnabled=PrintOptionEnabled;
				}
			}
		}

		private void Item_MouseWheel(object sender, MouseWheelEventArgs e){
			//not necessary. Already built in to the scrollviewer.
		}

		private void dispatcherTimerTreeClick_Tick(object sender, EventArgs e){
			//this timer starts when user clicks on a treenode that's already selected.
			//It gets cancelled if the click turns into a double click.
			if(_isDragPendingMsgBox){
				//A msgbox is showing. Refreshing the selection would hide the msgbox.
				return;
			}
			if(_nodeObjTagSelected==null || _nodeObjTagSelected.NodeType==EnumImageNodeType.None){
				return;
			}
			_dispatcherTimerTreeClick.IsEnabled=false;//only fire once.  Must come before line below or it could go into an infinite loop of msgBoxes.
			ItemReselected?.Invoke(this,new EventArgs());
		}

		private void menuItem_Click(object sender, EventArgs e){
			MenuClick?.Invoke(sender,e);
		}
		#endregion Methods private EventHandlers

		#region Classes - Nested
		///<summary>Compared to the old pattern, this is essentially a combination of a node and a tag. Holds a variety of data for each node, including raw data from the original query, type, text, images, etc. This really isn't a very good pattern. It looks like this because it was adapted from previous pattern. The preferred pattern would be to just have class level lists of defs, docs, etc.</summary>
		private class NodeObjTag{
			///<summary>How many items are in a category.</summary>
			public int CountInCategory;
			///<summary>Straight from the initial module query at GetTreeListTableForPatient. There are not very many columns, so we just go ahead and expose most of the columns as fields here.</summary>
			public DataRow DataRow=null;
			public DateTime DateCreated;
			///<summary>If this is a category folder, this contains the Def.</summary>
			public Def Def=null;
			///<summary>DefNum that this doc or mount belongs in.</summary>
			public long DocCategory;
			///<summary>Set with initial refresh from DataRow.DocNum</summary>
			public long DocNum=0;
			///<summary></summary>
			public EnumIconShow IconShow;
			///<summary>Only for nodes of type 'Document'.</summary>
			public ImageType ImgType;
			///<summary>This item shows a thumbnail and text instead of just text. Both the category and the items within it get this tag assigned.</summary>
			public bool IsThumbnail;
			///<summary>Only for category nodes.  Only gets set inside of ComputeRows.</summary>
			public EnumExpandCollapse ExpandCollapse;
			///<summary>Set with initial refresh from DataRow.MountNum</summary>
			public long MountNum=0;
			///<summary>Category, Document, or Mount.</summary>
			public EnumImageNodeType NodeType;
			///<summary>The text to display in the tree. For non-category, this is set with initial refresh from DataRow.description.</summary>
			public string Description="";

			public NodeObjTag(Def def){
				NodeType=EnumImageNodeType.Category;
				Def=def;
				Description=Def.ItemName;
			}

			public NodeObjTag(DataRow dataRow){
				DataRow=dataRow;
				DocNum=PIn.Long(dataRow["DocNum"].ToString());
				MountNum=PIn.Long(dataRow["MountNum"].ToString());
				DocCategory=PIn.Long(dataRow["DocCategory"].ToString());
				DateCreated=PIn.DateT(dataRow["DateCreated"].ToString());
				Description=PIn.String(dataRow["Description"].ToString());
				if(DocNum!=0){
					NodeType=EnumImageNodeType.Document;
					ImgType=(ImageType)PIn.Int(dataRow["ImgType"].ToString());
				}
				else{//assume mount
					NodeType=EnumImageNodeType.Mount;
				}
			}

			///<summary>The Icon to show for expand/collapse.</summary>
			public enum EnumExpandCollapse {
				///<summary>No children. No icon.</summary>  
				None,
				///<summary>Expanded.</summary>  
				Minus,
				///<summary>Collapsed with children.</summary>
				Plus
			}

			public NodeObjTag Copy(){
				return (NodeObjTag)MemberwiseClone();
			}

			public long GetPriKey(){
				if(NodeType==EnumImageNodeType.None){
					return 0;
				}
				if(NodeType==EnumImageNodeType.Category){
					return Def.DefNum;
				}
				if(NodeType==EnumImageNodeType.Document){
					return DocNum;
				}
				if(NodeType==EnumImageNodeType.Mount){
					return MountNum;
				}
				return 0;
			}

			///<summary>Tests whether the two nodeObjs match, based only on type and pk.</summary>
			public bool IsMatching(NodeObjTag nodeObjTag){
				if(nodeObjTag is null){
					return false;
				}
				if(NodeType!=nodeObjTag.NodeType){
					return false;
				}
				if(NodeType==EnumImageNodeType.Category && Def.DefNum==nodeObjTag.Def.DefNum){
					return true;
				}
				if(NodeType==EnumImageNodeType.Document && DocNum==nodeObjTag.DocNum){
					return true;
				}
				if(NodeType==EnumImageNodeType.Mount && MountNum==nodeObjTag.MountNum){
					return true;
				}
				return false;
			}
		}
		#endregion Classes - Nested
	}
}
