using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.UI{
	///<summary>This version of ImageSelector only uses GDI+ instead of Direct2D to avoid the various crashing and flickering issues.  Direct2D is behaving very differently in debug versus in the real world.  That could take months to sort out.  Until then, we use this.</summary>
	public partial class ImageSelectorTemp : Control{
		#region Fields - Public
		private LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>Height of the non-scrollable top region, at 96dpi. Contains various tools.</summary>
		private int _heightTop=25;
		///<summary>The total height of the scrollable area, including the parts hidden by scroll.</summary>
		private int _heightTotal;
		///<summary>The index which is lit up by the hover effect.</summary>
		private int _hoverIndex=-1;
		///<summary>The only reason we care about mouseDown is for initiating a drag.  Right mouse cannot initiate a drag.</summary>
		private bool _isLeftMouseDownDragging;
		///<summary>Gets set to true while data is being added and organized to prevent extra painting.</summary>
		private bool _isUpdating;
		///<summary>This list is passed in.</summary>
		private List<Def> _listDefsImageCats;
		///<summary>This keeps track of which nodes should remain expanded as the user moves between patients, etc.  Sometimes, a node has no children, so it cannot expand, but that will not change the state of this list.  The only actions which affect this list are Collapse All, Expand All, clicking on the +/- buttons, and SetSelected (which expands so that the selected can be seen).  This list is set to all expanded upon startup.</summary>
		private List<long> _listDefNumsExpanded=null;
		///<summary>This is the list of items to draw, including the categories and collapsed items.</summary>
		private List<NodeObjTag> _listNodeObjTags=new List<NodeObjTag>();
		///<summary>Do not compare equivalency.  Only compare type and primary key. Null indicates no selection.  It's a full copy of the original object.</summary>
		private NodeObjTag _nodeObjTagSelected;
		private Patient _patCur=null;
		///<summary>Tracks the last user to load ContrImages</summary>
		private long _userNumPrev=-1;
		///<summary>Width of the area at the left for expand/collapse, at 96dpi</summary>
		private int _widthPlusMinus=16;
		#endregion Fields - Private

		#region Controls
		private VScrollBar vScroll;
		private Button butCollapse;
		private Button butExpand;
		private Cursor _cursorDrag;
		private Timer timerTreeClick;
		#endregion Controls

		#region Contructor
		public ImageSelectorTemp(){
			InitializeComponent();
			vScroll=new VScrollBar();
			vScroll.Scroll+=new ScrollEventHandler(vScroll_Scroll);
			Controls.Add(vScroll);
			//ResizeRedraw=true;//no effect
			DoubleBuffered=true;
			butCollapse=new Button();
			butCollapse.Text="Collapse All";
			butCollapse.Size=new Size(78,19);
			butCollapse.Click += new System.EventHandler(butCollapse_Click);
			Controls.Add(butCollapse);
			butExpand=new Button();
			butExpand.Text="Expand All";
			butExpand.Size=new Size(78,19);
			butExpand.Click += new System.EventHandler(butExpand_Click);
			Controls.Add(butExpand);
			_cursorDrag=new Cursor(new MemoryStream(Properties.Resources.CursorDrag));
			timerTreeClick=new Timer(this.components);
			timerTreeClick.Interval = 1000;
			timerTreeClick.Tick += new System.EventHandler(this.timerTreeClick_Tick);
			LayoutControls();
		}
		#endregion Contructor		

		#region Events - Raise
		/*
		///<summary>Occurs after user clicks to collapses a category.  Event contains the DefNum.</summary>
		[Category("OD")]
		[Description("Occurs after user clicks to collapse a category.  Event contains the DefNum.")]
		public event EventHandler<long> AfterCollapse;

		///<summary>Occurs after user clicks to collapses a category.  Event contains the DefNum.</summary>
		[Category("OD")]
		[Description("Occurs after user clicks to expand a category.  Event contains the DefNum.")]
		public event EventHandler<long> AfterExpand;*/

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

		///<summary>Occurs when user selects item. Fires even if index does not change.  This allows a refresh action by clicking.  Also fires when a collapse causes deselection.</summary>
		[Category("OD")]
		[Description("Occurs when user selects item. Fires even if index does not change.  This allows a refresh action by clicking.  Also fires when a collapse causes deselection.")]
		public event EventHandler SelectionChangeCommitted;
		#endregion Events - Raise

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

		#region Properties - Public
		///<summary>Not possible to show this in designer.  We manually control when it shows instead of having it show automatically.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ContextMenu ContextMenu { get; set; }
		#endregion Properties - Public

		#region Properties - Not Browsable

		#endregion Properties - Not Browsable

		#region Methods - Public
		///<summary>Clear all images and categories in the case that we switch to a different user and the patient is no longer selected.</summary>
		public void ClearAll() {
			_listNodeObjTags.Clear();
			ComputeRows();
			LayoutControls();
			Invalidate();
		}

		///<summary>Also clears any selected item, but categories remain selected.</summary>
		public void CollapseAll(){
			if(_listDefNumsExpanded==null){
				return;
			}
			_listDefNumsExpanded.Clear();
			//for(int i=0;i<_listNodeObjTags.Count;i++){
			//	if(_listNodeObjTags[i].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Minus){
			//		_listNodeObjTags[i].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Plus;
			//	}
			//}
			if(_nodeObjTagSelected!=null){
				if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category){
					//leave categories selected because they won't disappear.
				}
				else{
					_nodeObjTagSelected=null;
				}
			}
			ComputeRows();
			LayoutControls();
			vScroll.Value=0;
			Invalidate();
		}

		public void ExpandAll(){
			if(_listDefNumsExpanded==null){
				return;
			}
			_listDefNumsExpanded.Clear();
			for(int i=0;i<_listDefsImageCats.Count;i++){
				_listDefNumsExpanded.Add(_listDefsImageCats[i].DefNum);
			}
			//for(int i=0;i<_listNodeObjTags.Count;i++){
			//	if(_listNodeObjTags[i].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
			//		_listNodeObjTags[i].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;
			//	}
			//}
			ComputeRows();
			LayoutControls();
			Invalidate();
		}

		/*
		public void ExpandNode(long defNum){
			NodeObjTag nodeObjTag=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==defNum);
			if(nodeObjTag is null){
				return;//unknown error
			}
			if(nodeObjTag.ExpandCollapse!=NodeObjTag.EnumExpandCollapse.Plus){
				return;//already expanded or no children
			}
			nodeObjTag.ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;
			ComputeRows();
			LayoutControls();
			Invalidate();
		}*/

		///<summary>Gets the DefNum category of the current selection. The current selection can be a folder itself, or a document within a folder. If nothing selected, then it returns the DefNum of first category in the list.</summary>
		public long GetSelectedCategory(){
			if(_nodeObjTagSelected==null){
				return Defs.GetDefsForCategory(DefCat.ImageCats,true)[0].DefNum;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document || _nodeObjTagSelected.NodeType==EnumImageNodeType.Mount){
				return _nodeObjTagSelected.DocCategory;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category) {
				return _nodeObjTagSelected.Def.DefNum;
			}
			return Defs.GetDefsForCategory(DefCat.ImageCats,true)[0].DefNum;
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

		///<summary>This is used when an item is deleted.  We should switch selection to the item above the deleted item, whether that be another item or a category.</summary>
		public ControlImagesJ.TypeAndKey GetItemAbove(){
			int idxNow=_listNodeObjTags.IndexOf(_listNodeObjTags.FirstOrDefault(x=>x.IsMatching(_nodeObjTagSelected)));
			return new ControlImagesJ.TypeAndKey(_listNodeObjTags[idxNow-1].NodeType,_listNodeObjTags[idxNow-1].GetPriKey());
		}

		///<summary>Loads the expanded image categories for the user on start up or when changing users.</summary>
		public void LoadExpandedPrefs() {
			if(_userNumPrev==Security.CurUser.UserNum) {
				return;//User has not changed.  Maintain expanded nodes.
			}
			CollapseAll();//Start with collapsed nodes
			List<UserOdPref> listUserOdPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.Definition);//Update override list.
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
			LayoutControls();
			Invalidate();
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
		public void SetData(Patient patCur,DataTable table,bool keepSelection){
			_patCur=patCur;
			_isUpdating=true;
			_listNodeObjTags=new List<NodeObjTag>();
			//Categories--------------------------------------------------------------------
			for(int i=0;i<_listDefsImageCats.Count;i++) {
				NodeObjTag nodeObjTag=new NodeObjTag(_listDefsImageCats[i]);
				if(_listDefsImageCats[i].ItemValue.Contains("L")) { 
					//Patient Portal Folder. This image is the only alteration in this entire module for "L" defs.
					nodeObjTag.ImageIndex=7;
				}
				else {
					nodeObjTag.ImageIndex=1;
				}
				_listNodeObjTags.Add(nodeObjTag);
			}
			//Docs, etc--------------------------------------------------------------------------
			for(int i=0;i<table.Rows.Count;i++){
				NodeObjTag nodeObjTag=new NodeObjTag(table.Rows[i]);
				if(nodeObjTag.NodeType==EnumImageNodeType.Document) { 
					nodeObjTag.ImageIndex=2+(int)nodeObjTag.ImgType;//this fails for Attachments
				}
				else {//mount
					nodeObjTag.ImageIndex=6;
				}
				NodeObjTag nodeObjTagParent=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==nodeObjTag.DocCategory);
				if(nodeObjTagParent==null){
					continue;//should never happen
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
			if(keepSelection){
				//Make sure selection exists. Works with null.
				NodeObjTag nodeObjTagMatching=_listNodeObjTags.FirstOrDefault(x=>x.IsMatching(_nodeObjTagSelected));
				if(nodeObjTagMatching is null){
					_nodeObjTagSelected=null;
				}
			}
			else{
				_nodeObjTagSelected=null;
			}
			ComputeRows();
			_isUpdating=false;
			LayoutControls();//because the new rows will change scroll
			vScroll.Value=0;
			Invalidate();
		}

		public void SetLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
			LayoutManager.ZoomChanged+=LayoutManager_ZoomChanged;
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
				Invalidate();
				return;
			}
			_nodeObjTagSelected=nodeObjTag.Copy();//We want to keep this even when the list gets cleared
			//expand the category node if it's collapsed
			NodeObjTag nodeObjTagParent=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==nodeObjTag.DocCategory);
			if(nodeObjTagParent is null){
				Invalidate();
				return;//unknown error
			}
			if(nodeObjTagParent.ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
				//nodeObjTagParent.ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;
				_listDefNumsExpanded.Add(nodeObjTagParent.Def.DefNum);
				ComputeRows();
				LayoutControls();
			}
			Invalidate();
		}
		#endregion Methods - Public

		#region Methods - Event Handlers - Painting
		protected override void OnPaint(PaintEventArgs pe){
			Graphics g=pe.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			if(DesignMode){
				pe.Graphics.Clear(Color.White);//SystemColors.Control);
				pe.Graphics.DrawRectangle(Pens.Gray,0,0,Width-1,Height-1);
				pe.Graphics.DrawString("Image Selector",Font,Brushes.Black,5,5);
				return;
			}
			if(_isUpdating) {
				return;
			}
			g.Clear(Color.White);
			//Main section-----------------------------------------------------------------------------
			for(int i=0;i<_listNodeObjTags.Count;i++){
				if(_listNodeObjTags[i].Height==0){
					continue;
				}
				Color colorBack=Color.White;
				if(_listNodeObjTags[i].NodeType==EnumImageNodeType.Category){
					if(_hoverIndex==i){
						colorBack=ColorOD.Hover;//Color.FromArgb(240,240,248);
					}
					else{
						colorBack=ColorOD.Control;
					}
				}
				else if(_hoverIndex==i){
					colorBack=ColorOD.Hover;
				}
				if(_listNodeObjTags[i].IsMatching(_nodeObjTagSelected)){
					Color colorSelected=Color.FromArgb(180,210,255);//this color is specific to this control
					colorBack=colorSelected;//This overrides hover color.
				}
				if(colorBack!=Color.White){
					using SolidBrush brushBack=new SolidBrush(colorBack);
					g.FillRectangle(brushBack,
						x:1,
						y:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value,
						Width-2,
						height:_listNodeObjTags[i].Height);
				}
				if(_listNodeObjTags[i].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus
					|| _listNodeObjTags[i].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Minus)
				{
					RectangleF rectangleF=new RectangleF(
						x:LayoutManager.Scale(3),
						y:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+LayoutManager.Scale(3),
						width:LayoutManager.Scale(10),
						height:LayoutManager.Scale(10));
					GraphicsPath graphicsPath=GraphicsHelper.GetRoundedPath(rectangleF,radiusCorner:1.5f);
					g.FillPath(Brushes.White,graphicsPath);
					g.DrawPath(Pens.Black,graphicsPath);
					g.DrawLine(Pens.Black,//horiz
						x1:LayoutManager.ScaleF(5f),
						y1:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+LayoutManager.Scale(8),
						x2:LayoutManager.ScaleF(11f),
						y2:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+LayoutManager.Scale(8));
				}
				if(_listNodeObjTags[i].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
					g.DrawLine(Pens.Black,//vert
						x1:LayoutManager.ScaleF(8f),
						y1:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+LayoutManager.Scale(5),
						x2:LayoutManager.ScaleF(8f),
						y2:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+LayoutManager.Scale(11));
				}
				float paddingTopText=LayoutManager.ScaleF(2.5f);
				if(_listNodeObjTags[i].NodeType==EnumImageNodeType.Category){
					EnumIconShow iconShow=EnumIconShow.Folder;
					if(_listNodeObjTags[i].Def.ItemValue.Contains("L")) { 
						iconShow=EnumIconShow.FolderWeb;
					}
					DrawAtlasIcon(g,iconShow,
						x:LayoutManager.Scale(_widthPlusMinus),
						y:LayoutManager.Scale(_heightTop)+(int)_listNodeObjTags[i].Ypos-vScroll.Value);
					g.DrawString(_listNodeObjTags[i].Text,
						Font,
						Brushes.Black,
						new RectangleF(
							x:LayoutManager.Scale(_widthPlusMinus)+LayoutManager.Scale(18),
							y:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+paddingTopText,
							Width,
							height:_listNodeObjTags[i].Height-paddingTopText)
						);
					using Pen penAbove=new Pen(ColorOD.Gray(110));
					g.DrawLine(penAbove,//horiz above
						x1:1,
						y1:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value,
						x2:Width-1,
						y2:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value);
					using Pen penBelow=new Pen(ColorOD.Gray(210));
					g.DrawLine(penBelow,1,//horiz below
						y1:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+_listNodeObjTags[i].Height,
						x2:Width-1,
						y2:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+_listNodeObjTags[i].Height);
				}
				else{
					EnumIconShow iconShow=EnumIconShow.Doc;
					if(_listNodeObjTags[i].NodeType==EnumImageNodeType.Mount){
						iconShow=EnumIconShow.Mount;
					}
					else{//document
						//doc already handled
						//attachment wasn't handled properly in previous versions.  We'll leave it as a doc.
						if(_listNodeObjTags[i].ImgType==ImageType.File){
							iconShow=EnumIconShow.File;
						}
						if(_listNodeObjTags[i].ImgType==ImageType.Photo){
							iconShow=EnumIconShow.Picture;
						}
						if(_listNodeObjTags[i].ImgType==ImageType.Radiograph){
							iconShow=EnumIconShow.Xray;
						}
					}
					DrawAtlasIcon(g,iconShow,x:2,y:LayoutManager.Scale(_heightTop)+(int)_listNodeObjTags[i].Ypos-vScroll.Value);
					g.DrawString(_listNodeObjTags[i].Text,
						Font,
						Brushes.Black,
						new RectangleF(
							x:LayoutManager.Scale(19),
							y:LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+paddingTopText,
							Width,
							height:_listNodeObjTags[i].Height-paddingTopText)
						);
				}
			}
			g.FillRectangle(SystemBrushes.Control,0,0,Width,LayoutManager.Scale(_heightTop));//top
			using Pen penHorizBottom=new Pen(ColorOD.Gray(110));
			g.DrawLine(penHorizBottom,
				x1:1,
				y1:LayoutManager.Scale(_heightTop)+_heightTotal-vScroll.Value,
				x2:Width-1,
				y2:LayoutManager.Scale(_heightTop)+_heightTotal-vScroll.Value);
			using Pen penOutline=new Pen(ColorOD.Outline);
			g.DrawRectangle(penOutline,0,0,Width-1,Height-1);//outline
			int sizeIcon=LayoutManager.Scale(16);
			int width=7*sizeIcon+8*2;
			int height=sizeIcon+4;
			//d.DrawBitmap(0,0,30,width,height);
			//d.EndDraw();
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) {
			//base.OnPaintBackground(pevent);
		}
		#endregion Methods - Event Handlers - Painting

		#region Methods - Event Handlers - Mouse
		protected override void OnDragEnter(DragEventArgs e) {
			//WARNING: this will not fire if debugging while VS is in Admin mode
			if(e.Data.GetDataPresent(DataFormats.FileDrop)){
				e.Effect=DragDropEffects.Copy;
			}
			//ignore any other data
			//maybe pay attention to images later?  Although I don't know how you would drag an image in.
		}

		///<summary>When someone drags files into the image selector from a Windows folder.</summary>
		protected override void OnDragDrop(DragEventArgs e) {
			int idxItem=HitTest(PointToClient(new Point(e.X,e.Y)));
			if(idxItem==-1){
				return;
			}
			if(!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}
			string[] stringArrayFiles=(string[])e.Data.GetData(DataFormats.FileDrop);
			if(stringArrayFiles.Length==0){
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
			Invalidate();
			SelectionChangeCommitted?.Invoke(this,new EventArgs());
			if(stringArrayFiles.Length==1){
				//no dialog
			}
			else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Copy files into this folder as new documents?")){
				return;
			}
			DragDropImportEventArgs dragDropImportEventArgs=new DragDropImportEventArgs();
			dragDropImportEventArgs.DefNumNew=defNumNew;
			dragDropImportEventArgs.ArrayFileNames=stringArrayFiles;
			DragDropImport?.Invoke(this,dragDropImportEventArgs);
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			timerTreeClick.Enabled=false;
			int idx=HitTest(e.Location);
			if(idx==-1){
				return;
			}
			_isLeftMouseDownDragging=false;//Not dragging if double clicked
			ItemDoubleClick?.Invoke(this,new EventArgs());
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(_isLeftMouseDownDragging){//already dragging before this click
				//Ignore this second one.  It could be right click or anything else. 
				return;
			}
			int idx=HitTest(e.Location);
			if(idx==-1){
				_nodeObjTagSelected=null;
				Invalidate();
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
				return;
			}
			//from here down, clicked on a valid row
			//Expand/Collapse============================================================================================
			if(_listNodeObjTags[idx].NodeType==EnumImageNodeType.Category
				&& (e.Button & MouseButtons.Left)==MouseButtons.Left//only left click can expand/collapse
				&& e.X<_widthPlusMinus 
				&& _listNodeObjTags[idx].ExpandCollapse!=NodeObjTag.EnumExpandCollapse.None)
			{
				if(_listNodeObjTags[idx].ExpandCollapse==NodeObjTag.EnumExpandCollapse.Minus){
					//Currently expanded, and clicked to collapse
					_listDefNumsExpanded.Remove(_listNodeObjTags[idx].Def.DefNum);
					SaveExpandedPref(_listNodeObjTags[idx].Def.DefNum,false);
					//AfterCollapse?.Invoke(this,_listNodeObjTags[idx].Def.DefNum);
					//_listNodeObjTags[idx].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Plus;
					//collapsing, so none of the children can remain selected
					List<NodeObjTag> _listNodeObjTagChildren=_listNodeObjTags.FindAll(x=>x.DocCategory==_listNodeObjTags[idx].Def.DefNum);
					for(int i=0;i<_listNodeObjTagChildren.Count;i++){
						if(_listNodeObjTagChildren[i].IsMatching(_nodeObjTagSelected)){
							_nodeObjTagSelected=null;
							Invalidate();
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
				LayoutControls();
				Invalidate();
				return;//without having selected anything. Also ignores right mouse click.
			}
			//Right click context menu=================================================================================
			if((e.Button & MouseButtons.Right)==MouseButtons.Right){
				//Yes, user could left mouse while right mouse was down, running the above code twice.
				if(_listNodeObjTags[idx].NodeType==EnumImageNodeType.Category){
					return;
				}
				if(_listNodeObjTags[idx].IsMatching(_nodeObjTagSelected)){//On node already selected
					//Do nothing because a large image will cause a pause before menu comes up
				}
				else{
					_nodeObjTagSelected=_listNodeObjTags[idx].Copy();
					Invalidate();
					SelectionChangeCommitted?.Invoke(this,new EventArgs());
				}
				ContextMenu.Show(this,e.Location);
				return;
			}
			//Select row================================================================================================
			if(_listNodeObjTags[idx].IsMatching(_nodeObjTagSelected)){//On node already selected
				//We refresh on the same node so that user can reset zoom/pan.
				//Can't always refresh immediately because it messes up double click.
				//So we pass off the refresh to a timer that will refresh in one sec.
				//But the timer will be cancelled if the user double clicks.
				//This keeps the UI very fast.
				if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document) {
					Document document=Documents.GetByNum(_nodeObjTagSelected.DocNum,doReturnNullIfNotFound:true);
					if(document==null) {
						return;//The document was deleted by another instance of the program.
					}
					if(Path.GetExtension(document.FileName).ToLower()==".pdf") {
						_isLeftMouseDownDragging=true;//Still want to be able to drag a PDF file
						return;//no refresh on the same node for pdf
					}
				}
				//if(zoomSlider.Value>1000) {//User clicked many many times on the zoom +.  We need to quickly reset the zoom for them.
				//	timerTreeClick.Interval=1;
				//}
				//else {
				timerTreeClick.Interval=1000;
				//}
				timerTreeClick.Enabled=true;
			}
			else {//new node
				_nodeObjTagSelected=_listNodeObjTags[idx].Copy();
				Invalidate();
				SelectionChangeCommitted?.Invoke(this,new EventArgs());
			}
			//Drag========================================================================================================
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Category){//can't drag a node
				return;
			}
			_isLeftMouseDownDragging=true;
			//However, the drag icon won't show until we actually start moving.
			//And if we don't move very far, nothing happens.			
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(_hoverIndex!=-1){
				_hoverIndex=-1;
				Invalidate();
			}
			//This doesn't fire when mouse moves out of bounds while dragging.
			//It does fire when button is released, after the mouse up, but that usually doesn't help.
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			int hoverIndex=HitTest(e.Location); 
			if(_hoverIndex!=hoverIndex){
				_hoverIndex=hoverIndex;
				Invalidate();
			}
			if(!_isLeftMouseDownDragging){
				return;
			}
			if(Control.MouseButtons!=MouseButtons.Left){
				//So _isLeftMouseDownDragging is supposedly true, but the mouse button is not down.
				//This can happen when a dialog comes up when user clicks on an item in this control.
				//So as soon as the mouse goes back to this control, we need to reset this.
				Cursor=Cursors.Default;
				_isLeftMouseDownDragging=false;
				return;
			}
			Cursor=_cursorDrag;
			//e.Location is relative to the area inside of the ImageSelector Control, therefore we must define this area which includes adding _heightTop to Y.
			Rectangle rectangleBounds=new Rectangle(0,LayoutManager.Scale(_heightTop),Width,Height-LayoutManager.Scale(_heightTop));
			if(rectangleBounds.Contains(e.Location)) {
				return;
			}
			//Moved outside of bounds while dragging==========================================================================
			Cursor=Cursors.Default;
			_isLeftMouseDownDragging=false;
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				return;
			}
			string patFolder=ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath());
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
			DoDragDrop(dataObject, DragDropEffects.Copy);
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if((e.Button & MouseButtons.Left)!=MouseButtons.Left){
				return;//we ignore every mouse up except left
			}
			if(!_isLeftMouseDownDragging){
				return;
			}
			_isLeftMouseDownDragging=false;
			Cursor=Cursors.Default;
			//e.Location is relative to the area inside of the ImageSelector Control, therefore we must define this area which includes adding _heightTop to Y.
			Rectangle rectangleBounds=new Rectangle(0,LayoutManager.Scale(_heightTop),Width,Height-LayoutManager.Scale(_heightTop));
			if(!rectangleBounds.Contains(e.Location)) {//mouse up outside bounds
				return;
			}
			if(_nodeObjTagSelected==null 
				|| _nodeObjTagSelected.NodeType==EnumImageNodeType.None
				|| _nodeObjTagSelected.NodeType==EnumImageNodeType.Category)
			{
				return;
			}	
			int idxUp=HitTest(e.Location);
			if(idxUp==-1){
				return;
			}
			long defNumNew=-1;
			if(_listNodeObjTags[idxUp].NodeType==EnumImageNodeType.Category){
				defNumNew=_listNodeObjTags[idxUp].Def.DefNum;
			}
			if(_listNodeObjTags[idxUp].NodeType==EnumImageNodeType.Document
				|| _listNodeObjTags[idxUp].NodeType==EnumImageNodeType.Mount)
			{
				defNumNew=_listNodeObjTags[idxUp].DocCategory;
			}
			if(defNumNew==-1 || defNumNew==_nodeObjTagSelected.DocCategory){
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Move to different category?")){
				return;
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Document){
				DragEventArgsOD dragEventArgs=new DragEventArgsOD(){DocNum=_nodeObjTagSelected.DocNum,DefNumNew=defNumNew};
				//We leave the current item selected.  It will be reselected automatically, based on type and key, and also refreshed.
				DraggedToCategory?.Invoke(this,dragEventArgs);
				SetSelected(EnumImageNodeType.Document,_nodeObjTagSelected.DocNum);
			}
			if(_nodeObjTagSelected.NodeType==EnumImageNodeType.Mount){
				DragEventArgsOD dragEventArgs=new DragEventArgsOD(){MountNum=_nodeObjTagSelected.MountNum,DefNumNew=defNumNew};
				DraggedToCategory?.Invoke(this,dragEventArgs);
				SetSelected(EnumImageNodeType.Mount,_nodeObjTagSelected.MountNum);
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			if(!vScroll.Enabled) {
				return;
			}
			int newValue=vScroll.Value-e.Delta/3;
			if(newValue>vScroll.Maximum-vScroll.LargeChange) {
				vScroll.Value=vScroll.Maximum-vScroll.LargeChange;
			}
			else if(newValue<vScroll.Minimum) {
				vScroll.Value=0;
			}
			else {
				vScroll.Value=newValue;
			}
			Invalidate();
		}
		#endregion Methods - Event Handlers - Mouse

		#region Methods - Event Handlers
		private void butCollapse_Click(object sender, EventArgs e){
			CollapseAll();
		}

		private void butExpand_Click(object sender, EventArgs e){
			ExpandAll();
		}

		private void LayoutManager_ZoomChanged(object sender,EventArgs e) {
			//if(d is null){
			//	return;
			//}
			//CreateInitialBitmap();
			ComputeRows();//not really needed until we have columns
			LayoutControls();
			Invalidate();
		}

		protected override void OnSizeChanged(EventArgs e) {
			//if(d!=null){
			//	CreateDeviceResources();//tell it about the new size
			//}
			ComputeRows();//not really needed until we have columns
			LayoutControls();
			Invalidate();
		}

		private void timerTreeClick_Tick(object sender, EventArgs e){
			//this timer starts when user clicks on a treenode that's already selected.
			//It gets cancelled if the click turns into a double click.
			if(_nodeObjTagSelected==null || _nodeObjTagSelected.NodeType==EnumImageNodeType.None){
				return;
			}
			timerTreeClick.Enabled=false;//only fire once.  Must come before line below or it could go into an infinite loop of msgBoxes.
			SelectionChangeCommitted?.Invoke(this,new EventArgs());
		}

		private void vScroll_Scroll(object sender,System.Windows.Forms.ScrollEventArgs e) {
			Invalidate();
			this.Focus();
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>This is the only place where the ExpandCollapse status of each category gets set.  Then, it computes Ypos, Height of each row, and _heightTotal.  Called when new data is set or after user clicks expand/collapse on any category.</summary>
		private void ComputeRows(){
			_heightTotal=0;
			int yPos=0;
			int hRow=Font.Height+LayoutManager.Scale(3);
			for(int i=0;i<_listNodeObjTags.Count;i++){
				_listNodeObjTags[i].Ypos=yPos;
				if(_listNodeObjTags[i].NodeType==EnumImageNodeType.Category){//always show categories
					_listNodeObjTags[i].Height=hRow;
					yPos+=hRow;
					bool hasChildren=_listNodeObjTags.Exists(x=>x.NodeType!=EnumImageNodeType.Category && x.DocCategory==_listNodeObjTags[i].Def.DefNum);
					if(hasChildren){
						if(_listDefNumsExpanded!=null && _listDefNumsExpanded.Exists(x=>x==_listNodeObjTags[i].Def.DefNum)){
							_listNodeObjTags[i].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Minus;//expanded
						}
						else{
							_listNodeObjTags[i].ExpandCollapse=NodeObjTag.EnumExpandCollapse.Plus;
						}
					}
					else{
						_listNodeObjTags[i].ExpandCollapse=NodeObjTag.EnumExpandCollapse.None;
					}
					continue;
				}
				NodeObjTag nodeObjTagParent=_listNodeObjTags.FirstOrDefault(x=>x.NodeType==EnumImageNodeType.Category && x.Def.DefNum==_listNodeObjTags[i].DocCategory);
				if(nodeObjTagParent==null){
					continue;//should never happen
				}
				if(nodeObjTagParent.ExpandCollapse==NodeObjTag.EnumExpandCollapse.Plus){
					_listNodeObjTags[i].Height=0;
				}
				else{
					_listNodeObjTags[i].Height=hRow;
					yPos+=hRow;
				}
			}
			_heightTotal=yPos;
		}

		//private void CreateDeviceResources(){
		//	d.CreateRenderTarget();
		//}

		/*
		private void CreateInitialBitmap(){
			
		}*/

		private void DrawAtlasIcon(Graphics g,EnumIconShow iconShow,int x,int y){
			int sizeIcon=LayoutManager.Scale(16);
			int padding=2;
			Color colorGradientTop=ColorOD.Control;
			Color colorGradientBottom=ColorOD.Control;
			switch(iconShow){
				case EnumIconShow.Folder:
					IconLibrary.Draw(g,EnumIcons.ImageSelectorFolder,new Rectangle(x,y,sizeIcon,sizeIcon),colorGradientTop,colorGradientBottom);
					break;
				case EnumIconShow.Doc:
					IconLibrary.Draw(g,EnumIcons.ImageSelectorDoc,new Rectangle(x,y,sizeIcon,sizeIcon),colorGradientTop,colorGradientBottom);
					break;
				case EnumIconShow.Xray:
					IconLibrary.Draw(g,EnumIcons.ImageSelectorXray,new Rectangle(x,y,sizeIcon,sizeIcon),colorGradientTop,colorGradientBottom);
					break;
				case EnumIconShow.Picture:
					IconLibrary.Draw(g,EnumIcons.ImageSelectorPhoto,new Rectangle(x,y,sizeIcon,sizeIcon),colorGradientTop,colorGradientBottom);
					break;
				case EnumIconShow.File:
					//square, a bit smaller than sizeIcon
					int inset=sizeIcon/8;
					using(SolidBrush brush=new SolidBrush(ColorOD.Gray(220))){
						g.FillRectangle(brush,x+inset,y+inset,sizeIcon-inset*2,sizeIcon-inset*2);
					}
					g.FillRectangle(Brushes.DarkBlue,x+inset,y+inset,sizeIcon-inset*2,sizeIcon/4);
					g.DrawRectangle(Pens.Black,x+inset,y+inset,sizeIcon-inset*2,sizeIcon-inset*2);
					break;
				case EnumIconShow.Mount:
					IconLibrary.Draw(g,EnumIcons.ImageSelectorMount,new Rectangle(x,y,sizeIcon,sizeIcon),colorGradientTop,colorGradientBottom);
					break;
				case EnumIconShow.FolderWeb:
					IconLibrary.Draw(g,EnumIcons.ImageSelectorFolderWeb,new Rectangle(x,y,sizeIcon,sizeIcon),colorGradientTop,colorGradientBottom);
					break;
			}
		}

		///<summary>Supply point within this control, usually from mouse coords.  Returns idx within _listRowInfo. Can be -1.</summary>
		private int HitTest(Point point){
			for(int i=0;i<_listNodeObjTags.Count;i++){
				if(_listNodeObjTags[i].Height==0){
					continue;
				}
				if(point.Y<LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value){
					continue;
				}
				if(point.Y>LayoutManager.Scale(_heightTop)+_listNodeObjTags[i].Ypos-vScroll.Value+_listNodeObjTags[i].Height){
					continue;
				}
				return i;
			}
			return -1;
		}

		///<summary>Lays out vertical scroll and a few buttons.  To allow deterministic layout with dynamic columns, we will not consider the width of the vScroll for other layout math.  Therefore, this method can always be run after ComputeRows.</summary>
		private void LayoutControls(){
			if(_isUpdating){
				return;
			}
			//no way to test suspendLayout state.
			if(Width==0 || Height==0){
				return;
			}
			//Scrollbars could use 96dpi or scaled dpi.  We decided to use scaled, which means we can use scroll values directly for drawing, without scaling them
			vScroll.Width=LayoutManager.Scale(17);//scroll width is 17 at 96dpi
			vScroll.Location=new Point(Width-vScroll.Width-1,LayoutManager.Scale(_heightTop)+1);
			vScroll.Height=Height-LayoutManager.Scale(_heightTop)-2;
			if(vScroll.Height<=0) {
				return;
			}
			if(_heightTotal<vScroll.Height) {
				vScroll.Value=0;
				vScroll.Enabled=false;
				vScroll.Visible=false;
			}
			else {
				vScroll.Enabled=true;
				vScroll.Visible=true;
				vScroll.Minimum = 0;
				vScroll.Maximum=_heightTotal;
				vScroll.LargeChange=vScroll.Height;
				vScroll.SmallChange=(int)(14*3.4f);//it's not an even number so that it is obvious to user that rows moved
			}
			//This control is skipped by LayoutManager, so we layout everything ourself
			butCollapse.Location=new Point(1,3);
			butCollapse.Size=new Size(LayoutManager.Scale(78),LayoutManager.Scale(19));
			butExpand.Location=new Point(butCollapse.Right+3,3);
			butExpand.Size=new Size(LayoutManager.Scale(78),LayoutManager.Scale(19));
		}

		///<summary>Save the Image Category expanded setting as a user preference.</summary>
		private void SaveExpandedPref(long defNum,bool isExpanding) {
			//Always delete to remove previous value (prevents duplicates).
			UserOdPrefs.DeleteForFkey(Security.CurUser.UserNum,UserOdFkeyType.Definition,defNum);
			if(isExpanding) {//Insert a row to save an expanded category.
				UserOdPref userPrefCur=new UserOdPref();//Preference to be inserted to override.
				userPrefCur.UserNum=Security.CurUser.UserNum;
				userPrefCur.Fkey=defNum;
				userPrefCur.FkeyType=UserOdFkeyType.Definition;
				UserOdPrefs.Insert(userPrefCur);
			}
		}
		#endregion Methods - Private

		#region Classes - Nested
		///<summary>Compared to the old pattern, this is essentially a combination of a node and a tag. Holds a variety of data for each node, including raw data from the original query, type, text, images, etc.</summary>
		private class NodeObjTag{
			///<summary>How many items are in a category.</summary>
			public int CountInCategory;
			///<summary>Straight from the initial module query at GetTreeListTableForPatient. There are not very many columns, so we just go ahead and expose most of the columns as fields here.</summary>
			public DataRow DataRow=null;
			///<summary>If this is a category folder, this contains the Def.</summary>
			public Def Def=null;
			///<summary>DefNum that this doc or mount belongs in.</summary>
			public long DocCategory;
			///<summary>Set with initial refresh from DataRow.DocNum</summary>
			public long DocNum=0;
			///<summary>The height of this row.  Can be 0 if child of a collapsed category.</summary>
			public float Height;
			///<summary>The index within the image list..</summary>
			public int ImageIndex=-1;
			///<summary>Only for nodes of type 'Document'.</summary>
			public ImageType ImgType;
			///<summary>Only for category nodes.  Only gets set inside of ComputeRows.</summary>
			public EnumExpandCollapse ExpandCollapse;
			///<summary>Set with initial refresh from DataRow.MountNum</summary>
			public long MountNum=0;
			///<summary>Category, Document, or Mount.</summary>
			public EnumImageNodeType NodeType;
			///<summary>The text to display in the tree. For non-category, this is set with initial refresh from DataRow.description.</summary>
			public string Text="";
			///<summary>The y position of the top of this node. Coordinates are from the top of the first row, as it would be without any scrolling.  To paint, add vertical scrolling, _heightTop, etc.</summary>
			public float Ypos;

			public NodeObjTag(Def def){
				NodeType=EnumImageNodeType.Category;
				Def=def;
				Text=Def.ItemName;
			}

			public NodeObjTag(DataRow dataRow){
				DataRow=dataRow;
				DocNum=PIn.Long(dataRow["DocNum"].ToString());
				MountNum=PIn.Long(dataRow["MountNum"].ToString());
				DocCategory=PIn.Long(dataRow["DocCategory"].ToString());
				//DateCreated not used
				Text=PIn.String(dataRow["Description"].ToString());
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
