using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Thinfinity;
using OpenDental.UI;
using System.Drawing.Imaging;
using CodeBase.Controls;

namespace OpenDental {
	public partial class FormImageFloat:FormODBase {
		/*
		FormODBase.IsImageFloatDocked is where the docked status gets flagged and tracked.  If docked, we keep the window exactly sized to the available space.  Hides automatically.  There can only be either zero or one window docked, and it's always in the 0 position in the list if present. The docked window is the only window that can accept new images when user clicks in the tree at the left. New image replaces existing image, but only if docked.  If user clicks new image, but no dock is present, then it creates a new dock.  If a window is not docked, then it shows in taskbar and does not get automatically moved, resized, or hidden.  To undock, drag the window.
		*/
		#region Fields - Public
		///<summary>Color for drawing lines and text.</summary>
		public Color ColorFore;
		///<summary>Color for background of text. Can be transparent.</summary>
		public Color ColorTextBack;
		///<summary>Tracks when this form was launched from the Chart Module to prevent specific behavior when deleted.</summary>
		public bool DidLaunchFromChartModule;
		public Patient PatientCur=null;
		///<summary></summary>
		public string PatFolder="";
		///<summary>For a few things, an actual reference to the imageSelector of the parent is handy.</summary>
		public ImageSelector ImageSelector_;
		///<summary>Just used to get the list of windows.</summary>
		public List<FormImageFloat> ListFormImageFloats;
		///<summary>The currently selected node type, key, and category.  This is specific to each window.  Can be null if nothing selected, which will cause pasted items to go to DefaultImageCategoryImportFolder pref if a DefNum exists, first category otherwise.</summary>
		private NodeTypeKeyCat _nodeTypeKeyCatSelected;
		#endregion Fields - Public
		
		#region Fields - Private
		///<summary>Typically just one bitmap at idx 0.  Windowing (brightness/contrast) has already been applied. Does not have applied crop, flip, rotation, mount translation, global translation, or zoom.  If it's a mount, there is one for each mount position, and some can be null.</summary>
		private Bitmap[] _bitmapArrayShowing;
		///<summary>Typically just one document at idx 0.  For mounts, there is one for each mount position, which all start out null.</summary>
		private Document[] _documentArrayShowing;
		///<summary>This contains the raw image data for a single selected doc or mount item.  Used for Dicom instead of the usual _bitmapRaw.  Can be null.</summary>
		private BitmapDicom _bitmapDicomRaw;
		///<summary>Only used when a single doc or mount item is selected.  This is the raw image that is the basis for BrightContrast adjustments.  Mount image might be scaled but it won't have any color adj.  If scaled, it's the same ratio as original.  We use width to calc scale, since height could be off by a partial pixel.  Does not have applied crop, flip, rotation, etc.  If the current selected bitmap is a Dicom image, then see _arrayDicomRaw instead.</summary>
		private Bitmap _bitmapRaw;
		///<summary></summary>
		private EnumCropPanAdj _cropPanAdj;
		private Cursor _cursorCrosshair;
		private Cursor _cursorLineAdd;
		private Cursor _cursorLineMove;
		private Cursor _cursorLinePoint;
		private Cursor _cursorLineSubtract;
		private Cursor _cursorMeasure;
		///<summary>Looks like an open hand.</summary>
		private Cursor _cursorPan;
		private Cursor _cursorScale;
		private DateTime _dateTimeMouseDownPanel;
		private EnumDrawMode _drawMode;
		///<summary>Dispose handled automatically when this form closes.</summary>
		private FormImageFloatWindows _formImageFloatWindows;
		///<summary>Used in Line mode and in Line Point editing. The index within _listImageDraws that we are editing.</summary>
		private int _idxImageDraw;
		///<summary>Only for Line Point editing. The index within the imageDraw.DrawingSegment.</summary>
		private int _idxPoint;
		///<summary>The index of the currently selected item within a mount. 0-indexed.  Frequently -1 to indicate no selection. No relation to mountItem.ItemOrder.</summary>
		private int _idxSelectedInMount=-1;
		///<summary>Will normally be -1.</summary>
		private int _idxTextMouseDown=-1;
		///<summary>True when the menu is showing.</summary>
		private bool _isButWindowPressed;
		private bool _isDraggingMount=false;
		///<summary>True if the mouse is currently over the "Windows" button at the top.</summary>
		private bool _isHotButWindow;
		private bool _isLineExtending;
		///<summary>Todo: let Esc key cancel out of this.</summary>
		private bool _isLineStarted;
		private bool _isMouseDownPanel;
		private bool _isDraggingText;
		///<summary>This is the label that goes on panelHover</summary>
		private Label labelHover;
		///<summary>All the ImageDraw rows for this document or mount.</summary>
		private List<ImageDraw> _listImageDraws;
		///<summary>If a mount is currently selected, this is the list of the mount items on it. This is pulled from the database, and then the other two lists (_bitmapArrayShowing and _documentArrayShowing) are set to match this in length so that all three have a 1:1:1 relationship.</summary>
		private List<MountItem> _listMountItems=null;
		///<summary>For drawings and polygons.  This is just a temporary list while the mouse is down.  All points are in bitmap or mount coords. As the mouse is moved this list gets longer. On mouse up, it gets saved to the db, this list of points gets cleared, and panel invalidated.</summary>
		private List<PointF> _listPointFsDrawing=new List<PointF>();
		///<summary>Keeps track of the currently selected mount object (only when a mount is selected).</summary>
		private Mount _mountShowing=null;
		///<summary>For Pearl hover effect. Could be a window, but panel was slightly faster and easier. No logic for crashing into bounds. They can pan.</summary>
		private PanelOD panelHover;
		///<summary>When dragging mount bitmap, this is the current point of the bitmap, in mount coordinates.</summary>
		private Point _pointDragNow;
		///<summary>When dragging mount item, this is the starting point of the center of the mount item where raw image will draw, in mount coordinates.</summary>
		private Point _pointDragStart;
		///<summary>A temporary point where text measurement will show. Also see _stringMeasure.</summary>
		private Point _pointMeasureText;
		private Point _pointMouseDown=new Point(0,0);
		///<summary>Location of text in our memory object in list changes as we drag. Update goes to db on mouse up. Because the original point gets lost, this point is needed in order to calulate new text location.</summary>
		private Point _pointTextOriginal;
		///<summary>This translation is added to the bitmaps showing, based on user drags.  It's in bitmap/mount coords, not screen coords.</summary>
		private Point _pointTranslation;
		///<summary>When mouse down, this is recorded as the _pointTranslation for delta purposes while dragging.</summary>
		private Point _pointTranslationOld;
		///<summary>The bounds of the "Windows" button at the upper right.</summary>
		private Rectangle _rectangleButWindows;
		///<summary>In panel coords.</summary>
		private Rectangle _rectangleCrop;
		///<summary>Does not control showing drawings for Pearl or any other external source.</summary>
		private bool _showDrawingsOD;
		///<summary></summary>
		private bool _showDrawingsPearl;
		///<summary>Temporary text that will show for measurement. Also see _pointMeasureText.</summary>
		private string _stringMeasure;
		///<summary>Displays PDFs.</summary>
		private ODWebView2 _odWebView2=null;
		///<summary>The location of the file that <see cref="_odWebView2" /> has navigated to.</summary>
		private string _odWebView2FilePath=null;
		///<summary>This is needed as we switch betwen windows, in order to set the zoomSlider.</summary>
		private ZoomSliderState _zoomSliderStateInitial;
		///<summary>Property backer.</summary>
		private int _zoomSliderValue;
		private CloudIframe _cloudIframe=null;
		#endregion Fields - Private

		#region Constructor
		public FormImageFloat() {
			InitializeComponent();
			InitializeLayoutManager();
			LayoutManager.ZoomChanged+=LayoutManager_ZoomChanged;
			Lan.F(this);
			_cursorPan=new Cursor(GetType(),"CursorPalm.cur");
			panelMain.Cursor=_cursorPan;
			panelMain.MouseWheel += panelMain_MouseWheel;//here because not browsable in designer
			panelMain.ContextMenu=menuPanelMain;
			if(!ODBuild.IsWeb()) {//WebView2 does not work on Cloud
				_odWebView2=new ODWebView2();//Include a WebView2 object for loading .pdf files.
				_odWebView2.Visible=false;
				_odWebView2.Dock=DockStyle.Fill;
				_odWebView2.DoBlockNavigation=true;
				LayoutManager.Add(_odWebView2,this);
			}
			else if(ODBuild.IsWeb()) {//For cloud, instead use CloudIframe.
				_cloudIframe=new CloudIframe();
				_cloudIframe.Initialize();
				_cloudIframe.HideIframe();
				_cloudIframe.Dock=DockStyle.Fill;
				LayoutManager.Add(_cloudIframe,this);
			}
			_cursorCrosshair=new Cursor(GetType(),"CursorCrosshair.cur");
			_cursorLineAdd=new Cursor(GetType(),"CursorLineAdd.cur");
			_cursorLineMove=new Cursor(GetType(),"CursorLinePoint.cur");
			_cursorLinePoint=new Cursor(GetType(),"CursorLinePoint2.cur");
			_cursorLineSubtract=new Cursor(GetType(),"CursorLineSubtract.cur");
			_cursorMeasure=new Cursor(GetType(),"CursorMeasure.cur");
			_cursorScale=new Cursor(GetType(),"CursorScale.cur");
			panelHover=new PanelOD();
			panelHover.Size=new Size(200,200);
			panelHover.Visible=false;
			LayoutManager.Add(panelHover,panelMain);
			labelHover=new Label();
			labelHover.Size=new Size(180,180);//10 pixels of margin all around
			labelHover.Location=new Point(10,10);
			labelHover.Anchor=AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			LayoutManager.Add(labelHover,panelHover);
		}
		#endregion Constructor

		#region Events
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<ToolBarButtonState> EventEnableToolBarButtons=null;

		///<summary>Use this to trigger a FillTree in the parent. Bool true to keep selection, or false to not select anything.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<bool> EventFillTree=null;

		///<summary>For Ctrl-P for select patient. Won't fire if handled instead by Adobe for print.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<KeyEventArgs> EventKeyDown=null;

		///<summary>Use this to trigger a SelectTreeNode in the parent. When refreshing a mount, it must be done at parent level instead of here in order to include unmounted bar, etc. Pass null to reselect current node.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<NodeTypeAndKey> EventSelectTreeNode=null;

		///<summary>This event bubbles up to the parent, which then sets the property here.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EnumCropPanAdj> EventSetCropPanEditAdj=null;

		///<summary>This event bubbles up to the parent, which then calls SetDrawMode() here.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EnumDrawMode> EventSetDrawMode=null;

		///<summary>This event is fired from SetWindowingSlider, which makes the decisions about the windowing for what's showing.  The event bubbles up to the parent to set windowingSlider min and max.  But there is no property here to affect.  Separately, the parent can call SetWindowingSlider, which will then also fire this event like normal. Finally, in windowingSlider_Scroll, the parent directly sets min and max on the document within this window.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<WindowingEventArgs> EventSetWindowingSlider=null;

		///<summary>This event bubbles up to the parent, which then sets the property here.  This sets the zoom slider so that the middle tick will be at the "fit" zoom.  This is needed any time the image is rotate or cropped, or if the form size changes.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<ZoomSliderState> EventSetZoomSlider=null;

		///<summary>This event is fired when the thumbnail needs to be refreshed.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventThumbnailNeedsRefresh=null;

		///<summary>This event bubbles up to the parent, which then sets the property here.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<float> EventZoomSliderSetByWheel=null;

		///<summary>If you use this event, you must also set the ZoomSliderValue.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<int> EventZoomSliderSetValueAndMax=null;

		///<summary>Bubbles up from child form FormImageFloatWindows. Handled by parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<int> EventWindowClicked=null;

		///<summary>Bubbles up from child form FormImageFloatWindows. Handled by parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventWindowCloseOthers=null;

		///<summary>Bubbles up from child form FormImageFloatWindows. Handled by parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventWindowDockThis=null;

		///<summary>Bubbles up from child form FormImageFloatWindows. Handled by parent form ControlImagesJ.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler EventWindowShowAll=null;

		#endregion Events

		#region Properties
		///<summary>This is needed as we switch betwen windows, in order to set the zoomSlider.</summary>
		public ZoomSliderState ZoomSliderStateInitial{
			get{
				return _zoomSliderStateInitial;
			}
			set{
				_zoomSliderStateInitial=value;
			}
		}

		///<summary>This allows the value to flow down into this window.  To send zoom setting up to parent and zoomSlider, there are a 3 events.</summary>
		public int ZoomSliderValue{
			get{
				return _zoomSliderValue;
			}
			set{
				_zoomSliderValue=value;
				panelMain.Invalidate();
			}
		}
		#endregion Properties

		#region Methods - Public
		///<summary>Disposes and recreates a new CloudIframe control for ODCloud that is hidden.</summary>
		public void ClearPDFBrowser() {
			if(ODBuild.IsWeb()) {
				//ODCloud does not use _odWebView2
				_cloudIframe.HideIframe();
				IsImageFloatLocked=false;
				return;
			}
			IsImageFloatLocked=false;
		}

		///<summary>Deletes the specified document from the database and refreshes the tree view. Set securityCheck false when creating a new document that might get cancelled.  Document is passed in because it might not be in the tree if the image folder it belongs to is now hidden.</summary>
		public void DeleteDocument(bool isVerbose,bool doSecurityCheck,Document document) {
			if(doSecurityCheck) {
				if(!Security.IsAuthorized(EnumPermType.ImageDelete,document.DateCreated)) {
					return;
				}
			}
			TaskAttachment taskAttachment=TaskAttachments.GetOneByDocNum(document.DocNum);
			if(taskAttachment!=null) {
				MessageBox.Show(Lan.g(this,"This document is attached to task ")+taskAttachment.TaskNum+". "+Lan.g(this,"Detach document from this task before deleting the document."));
				return;
			}
			EhrLab ehrLab=EhrLabImages.GetFirstLabForDocNum(document.DocNum);
			if(ehrLab!=null) {
				string dateStr=ehrLab.ObservationDateTimeStart.PadRight(8,'0').Substring(0,8);//stored in DB as yyyyMMddhhmmss-zzzz
				DateTime dateTime=PIn.Date(dateStr.Substring(4,2)+"/"+dateStr.Substring(6,2)+"/"+dateStr.Substring(0,4));
				MessageBox.Show(Lan.g(this,"This image is attached to a lab order for this patient on "+dateTime.ToShortDateString()+". "+Lan.g(this,"Detach image from this lab order before deleting the image.")));
				return;
			}
			//EnableAllToolBarButtons(false);
			if(isVerbose) {
				if(IsMountItemSelected()){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete item from within the mount?")) {
						return;
					}
				}
				else if(document.ImgType.In(ImageType.Document,ImageType.File)){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete document?")) {
						return;
					}
				}
				else{
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete image?")) {
						return;
					}
				}
			}
			Statements.DetachDocFromStatements(document.DocNum);
			//SelectTreeNode(null);//Release access to current image so it may be properly deleted.
			ClearPDFBrowser();
			Document[] documentArray=new Document[1] { document };	
			try {
				ImageStore.DeleteDocuments(documentArray,PatFolder);
			}
			catch(Exception ex) {  //Image could not be deleted, in use.
				MessageBox.Show(this,ex.Message);
			}
			if(IsDocumentShowing()){
				EventFillTree?.Invoke(this,false);
				SelectTreeNode(null);
				panelMain.Invalidate();
			}
			if(IsMountItemSelected()){
				//need to review with more situations.  What if item isn't filled yet?
				_documentArrayShowing[_idxSelectedInMount]=null;
				_bitmapArrayShowing[_idxSelectedInMount]=null;
				panelMain.Invalidate();
			}
		}

		public void DocumentAcquiredForMount(Document document,Bitmap bitmap,MountItem mountItem,bool flipOnAcquire){
			//_bitmapDicomRaw=bitmapDicom;
			Document documentOld=document.Copy();
			document.MountItemNum=mountItem.MountItemNum;
			if(flipOnAcquire){
				document.IsFlipped=true;
			}
			document.DegreesRotated=mountItem.RotateOnAcquire;
			document.ToothNumbers=mountItem.ToothNumbers;
			Documents.Update(document,documentOld);
			if(mountItem.ItemOrder==-1){//unmounted
				//this is a little clumsy. Will convert all these arrays to list when time.
				List<Document> listDocuments=_documentArrayShowing.ToList();
				listDocuments.Add(document);
				_documentArrayShowing=listDocuments.ToArray();
				List<Bitmap> listBitmaps=_bitmapArrayShowing.ToList();
				listBitmaps.Add(new Bitmap(bitmap));
				_bitmapArrayShowing=listBitmaps.ToArray();
				_listMountItems.Add(mountItem);
				//not going to apply bright/contrast because it's an acquire. Doubt it's needed.
				return;
			}
			//The following lines are from LoadBitmap(OnlyIdx), but optimized for the situation when we already have the bitmap
			SetDocumentShowing(_idxSelectedInMount,document);
			SetBitmapShowing(_idxSelectedInMount,new Bitmap(bitmap));
			//_bitmapRaw: don't load this because it would waste time.  Instead, deselect after loop.
			ImageHelper.ApplyColorSettings(GetBitmapShowing(_idxSelectedInMount),
				GetDocumentShowing(_idxSelectedInMount).WindowingMin,GetDocumentShowing(_idxSelectedInMount).WindowingMax);
			panelMain.Invalidate();
		}

		///<summary>Sets appropriate toolBarButtons for document vs mount vs none/category.</summary>
		public void EnableToolBarButtons(){
			ToolBarButtonState toolBarButtonState;
			if(IsDocumentShowing()){
				//In Web mode, the buttons do not appear when hovering over the PDF, so we need to enable the print toolbar button.
				bool doShowPrint=panelMain.Visible;
				bool doShowExport=!GetDocumentShowing(0).FileName.IsNullOrEmpty();
				toolBarButtonState=new ToolBarButtonState(print:doShowPrint, delete:true, info:true, sign:true, export:doShowExport, copy:panelMain.Visible, brightAndContrast:panelMain.Visible, zoom:panelMain.Visible, zoomOne:false, crop:panelMain.Visible, pan:panelMain.Visible, adj:false, size:panelMain.Visible, flip:panelMain.Visible, rotateL:panelMain.Visible, rotateR:panelMain.Visible, rotate180:panelMain.Visible,draw:panelMain.Visible, unmount:false);
				EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
			}
			else if(IsMountShowing()){
				if(_idxSelectedInMount==-1){//entire mount
					toolBarButtonState=new ToolBarButtonState(print:true, delete:true, info:true, sign:false, export:true, copy:true, brightAndContrast:false, zoom:true, zoomOne:false, crop:false, pan:true, adj:true, size:false, flip:false, rotateL:false, rotateR:false, rotate180:false,draw:true, unmount:false);
					EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
				}
				else{//individual image
					toolBarButtonState=new ToolBarButtonState(print:true, delete:true, info:true, sign:false, export:true, copy:true, brightAndContrast:true, zoom:true, zoomOne:true, crop:false, pan:true, adj:true, size:true, flip:true, rotateL:true, rotateR:true, rotate180:true,draw:true, unmount:true);
					EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
				}
			}
			else{
				//none or category
				//All false.  This is also done in ControlImagesJ.DisableAllToolBarButtons() when module is selected.
				toolBarButtonState=new ToolBarButtonState(print:false, delete:false, info:false, sign:false, export:false, copy:false, brightAndContrast:false, zoom:false, zoomOne:false, crop:false, pan:true, adj:false, size:false, flip:false, rotateL:false, rotateR:false, rotate180:false, draw:false, unmount:false);
				EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
			}
		}

		public void formVideo_BitmapCaptured(object sender, VideoEventArgs e){
			Document document = null;
			long defNumCategory=GetCurrentCategory();
			if(PrefC.GetLong(PrefName.VideoImageCategoryDefault)>0){
				defNumCategory=PrefC.GetLong(PrefName.VideoImageCategoryDefault);
			}
			try {
				//it seems to sometimes fire a second time, with bitmap null
				if(e.Bitmap is null){
					return;
				}
				document=ImageStore.Import(e.Bitmap,defNumCategory,ImageType.Photo,PatientCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save")+": "+ex.Message);
				return;
			}
			if(!IsMountShowing()){//single
				EventFillTree?.Invoke(this,false);//Reload and keep new document selected.
				EventSelectTreeNode?.Invoke(this,new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				return;
			}
			//From here down is mount=================================================================================
			if(IsMountItemSelected()){
				//user must have clicked onto an occupied position in the middle of a series.
				MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
				return;
			}
			if(_idxSelectedInMount==-1){
				//user must have clicked outside mount to deselect
				MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
				return;
			}
			Document documentOld=document.Copy();
			document.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
			Mount mount=GetMountShowing();
			if(mount.FlipOnAcquire){
				document.IsFlipped=true;
			}
			document.DegreesRotated=_listMountItems[_idxSelectedInMount].RotateOnAcquire;
			document.ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
			Documents.Update(document,documentOld);
			//The following lines are from LoadBitmap(OnlyIdx), but optimized for the situation when we already have the bitmap
			_documentArrayShowing[_idxSelectedInMount]=document;
			SetBitmapShowing(_idxSelectedInMount,new Bitmap(e.Bitmap));
			//_bitmapRaw: don't load this because it would waste time.  Instead, deselect after loop.
			ImageHelper.ApplyColorSettings(GetBitmapShowing(_idxSelectedInMount),
				_documentArrayShowing[_idxSelectedInMount].WindowingMin,_documentArrayShowing[_idxSelectedInMount].WindowingMax);
			e.Bitmap?.Dispose();
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listMountItemsAvail=GetAvailSlots(1);
			if(listMountItemsAvail is null){//no more available slots, so we are done
				_idxSelectedInMount=-1;
				panelMain.Invalidate();
				return;
			}
			MountItem mountItem=_listMountItems.FirstOrDefault(x=>x.MountItemNum==listMountItemsAvail[0].MountItemNum);
			if(mountItem==null){//should not be possible
				return;
			}
			_idxSelectedInMount=_listMountItems.IndexOf(mountItem);
			panelMain.Invalidate();
			//wait for next event to fire
		}

		///<summary>Returns a list of available slots, starting with the one selected.  It will loop back around at the end to fill remaining slots.  Will return null if not enough slots.  But if you supply countNeed -1, then it will give you a list of all available.</summary>
		public List<MountItem> GetAvailSlots(int countNeed=-1){
			//make a list of all the empty mount slots
			List<MountItem> listMountItemsAvail=new List<MountItem>();
			int idxItem=_idxSelectedInMount;
			if(idxItem==-1){
				idxItem=0;
			}
			//idxItem could be in the middle of _listMountItems.  
			while(idxItem<_listMountItems.Count){
				if(_documentArrayShowing[idxItem]!=null){
					//occupied
					idxItem++;
					continue;
				}
				if(_listMountItems[idxItem].ItemOrder==0){
					//text items are not available 
					idxItem++;
					continue;
				}
				listMountItemsAvail.Add(_listMountItems[idxItem]);
				idxItem++;
			}
			if(_idxSelectedInMount>0){//Second loop to catch items lower than the first loop
				idxItem=0;
				while(idxItem<_idxSelectedInMount){
					if(_documentArrayShowing[idxItem]!=null){
						idxItem++;
						continue;
					}
					if(_listMountItems[idxItem].ItemOrder==0){
						idxItem++;
						continue;
					}
					listMountItemsAvail.Add(_listMountItems[idxItem]);
					idxItem++;
				}
			}
			if(countNeed!=-1 && listMountItemsAvail.Count<countNeed){
				return null;
			}
			return listMountItemsAvail;
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayBitmapsShowing. Can return null. Windowing (brightness and contrast) is already applied.</summary>
		public Bitmap GetBitmapShowing(int idx) {
			if(_bitmapArrayShowing==null){
				return null;
			}
			if(idx>_bitmapArrayShowing.Length-1){
				return null;
			}
			return _bitmapArrayShowing[idx];
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _documentArrayShowing. Can return null. It is optional to use this instead of using _documentArrayShowing directly. The advantage is that's it's safer in certain situations where _documentArrayShowing might be null or length might be incorrect.</summary>
		public Document GetDocumentShowing(int idx) {
			if(_documentArrayShowing==null){
				return null;
			}
			if(idx>_documentArrayShowing.Length-1){
				return null;
			}
			return _documentArrayShowing[idx];
		}

		public int GetIdxSelectedInMount(){
			return _idxSelectedInMount;
		}


		public List<Bitmap> GetListBitmaps(){
			return _bitmapArrayShowing.ToList();
		}

		public List<FormImageFloat> GetListWindows(){
			return ListFormImageFloats;
			/*
			List<string> listStrings=new List<string>();
			for(int i=0;i<ListFormImageFloats.Count;i++){
				listStrings.Add(ListFormImageFloats[i].Text);
			}
			return listStrings;*/
		}

		public List<MountItem> GetListMountItems(){
			return _listMountItems;
		}

		public Mount GetMountShowing(){
			//When inside this form, it's better to just directly use _mountShowing.
			return _mountShowing;
		}

		///<summary></summary>
		public NodeTypeAndKey GetNodeTypeAndKey(){
			if(_nodeTypeKeyCatSelected==null){
				return new NodeTypeAndKey(EnumImageNodeType.None,0);
			}
			return new NodeTypeAndKey(_nodeTypeKeyCatSelected.ImageNodeType,_nodeTypeKeyCatSelected.PriKey);
		}

		///<summary></summary>
		public EnumImageNodeType GetSelectedType(){
			if(_nodeTypeKeyCatSelected==null){
				return EnumImageNodeType.None;
			}
			return _nodeTypeKeyCatSelected.ImageNodeType;
		}

		///<summary></summary>
		public long GetSelectedPriKey(){
			if(_nodeTypeKeyCatSelected==null){
				return 0;
			}
			return _nodeTypeKeyCatSelected.PriKey;
		}

		public List<UnmountedObject> GetUmountedObjects(){
			List<UnmountedObject> listUnmountedObjects=new List<UnmountedObject>();
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder!=-1){
					continue;
				}
				UnmountedObject unmountedObject=new UnmountedObject();
				unmountedObject.MountItem=_listMountItems[i];
				unmountedObject.Document=_documentArrayShowing[i];//could be null
				unmountedObject.Bitmap=_bitmapArrayShowing[i];//could be null
				listUnmountedObjects.Add(unmountedObject);
			}
			return listUnmountedObjects;
		}

		///<summary>Returns true if the CloudIframe control was hidden for ODCloud. Otherwise; false.</summary>
		public bool HideWebBrowser() {
			if(ODBuild.IsWeb()) {
				//ODCloud uses _cloudIframe instead of _odWebView2
				_cloudIframe.HideIframe();
			}
			IsImageFloatLocked=false;
			return true;
		}

		///<summary>Invalidates the color image setting and recalculates.  This is not on a separate thread.  Instead, it's just designed to run no more than about every 300ms, which completely avoids any lockup.</summary>
		public void InvalidateSettingsColor() {
			if(IsDocumentShowing()){
				//if((imageSettingFlags & ImageSettingFlags.COLORFUNCTION) !=0 || (imageSettingFlags & ImageSettingFlags.CROP) !=0){
				//We don't do any flip, rotate, or crop here
				GetBitmapShowing(0)?.Dispose();
				if(_bitmapRaw!=null){
					SetBitmapShowing(0,ImageHelper.CopyWithBrightContrast(GetDocumentShowing(0),_bitmapRaw));
				}
				if(_bitmapDicomRaw!=null){
					SetBitmapShowing(0,DicomHelper.ApplyWindowing(_bitmapDicomRaw,GetDocumentShowing(0).WindowingMin,GetDocumentShowing(0).WindowingMax));
				}
				panelMain.Invalidate();
			}
			if(IsMountItemSelected()){
				_bitmapArrayShowing[_idxSelectedInMount]?.Dispose();
				if(_bitmapRaw!=null){
					_bitmapArrayShowing[_idxSelectedInMount]=ImageHelper.CopyWithBrightContrast(_documentArrayShowing[_idxSelectedInMount],_bitmapRaw);
				}
				if(_bitmapDicomRaw!=null){
					Bitmap bitmapTemp=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_documentArrayShowing[_idxSelectedInMount].WindowingMin,_documentArrayShowing[_idxSelectedInMount].WindowingMax);
					_bitmapArrayShowing[_idxSelectedInMount]=new Bitmap(bitmapTemp);
					bitmapTemp.Dispose();//this is not efficient
				}
				panelMain.Invalidate();
				//eventually: only invalidate the rectangle of single mount item
			}
		}

		///<summary>Returns true if a bitmap is showing.  False for mounts.</summary>
		public bool IsBitmapShowing(){
			if(GetSelectedType()!=EnumImageNodeType.Document){
				return false;
			}
			if(GetDocumentShowing(0)==null){
				return false;
			}
			if(GetBitmapShowing(0)==null){
				return false;
			}
			return true;
		}

		///<summary>Returns true if a valid document is showing.</summary>
		public bool IsDocumentShowing(){
			if(GetSelectedType()!=EnumImageNodeType.Document){
				return false;
			}
			if(GetDocumentShowing(0)==null){
				return false;
			}
			return true;
		}

		///<summary>Returns true if a valid mount is showing.</summary>
		public bool IsMountShowing(){
			if(GetSelectedType()!=EnumImageNodeType.Mount){
				return false;
			}
			if(_mountShowing==null){
				return false;
			}
			return true;
		}

		///<summary>Returns true if a valid mountitem is selected and there's a valid bitmap in that location.</summary>
		public bool IsMountItemSelected(){
			if(!IsMountShowing()){
				return false;
			}
			if(_idxSelectedInMount==-1){
				return false;
			}
			if(_documentArrayShowing[_idxSelectedInMount]==null){
				return false;
			}
			return true;
		}

		public void Parent_KeyDown(Keys keys){
			//because FormImageFloat_KeyDown would not be reliable enough on its own. This form might not have focus.
			if(keys==Keys.ShiftKey){
				//FormImageFloat_KeyDown(this,new KeyEventArgs(keys));//this bubbled up, and then down, creating infinite loop. So, instead:
				Point pointInPanel=panelMain.PointToClient(Control.MousePosition);
				MouseEventArgs mouseEventArgs=new MouseEventArgs(MouseButtons.Left,1,pointInPanel.X,pointInPanel.Y,0);
				panelMain_MouseMove(this,mouseEventArgs);//to trigger the minus sign
			}
		}

		public void PdfPrintPreview(){
			if(ODBuild.IsWeb()) {
				if(_odWebView2FilePath.IsNullOrEmpty()) {
					SetPdfFilePath(PatFolder,GetDocumentShowing(0).FileName,"","Downloading Document...");
				}
				if(!File.Exists(_odWebView2FilePath)) {
					MessageBox.Show(Lan.g(this,"File not found")+": "+GetDocumentShowing(0).FileName);
					_odWebView2FilePath="";
					return;
				}
				ThinfinityUtils.HandleFile(_odWebView2FilePath);//This will do a PDF preview. WebView2 does not work with cloud.
			}
			//WebView2 has its own built-in pdf preview that is shown when clicking 'Print'.
			SecurityLogs.MakeLogEntry(EnumPermType.Printing,PatientCur.PatNum,"Patient PDF "+GetDocumentShowing(0).FileName+" "+GetDocumentShowing(0).Description+" printed");
		}

		///<summary>If a mount is showing, and if no item is selected, then this will select the first open item. If one is already selected, but it's occupied, this does not check that.  There is also no guarantee that one will be selected after this because all positions could be full.</summary>
		public void PreselectFirstItem(){
			if(!IsMountShowing()){
				return;
			}
			if(_idxSelectedInMount!=-1){//already selected
				return;
			}
			for(int i=0;i<_documentArrayShowing.Length;i++){
				if(_listMountItems[i].ItemOrder==0){
					//don't select text items
					continue;
				}
				if(_documentArrayShowing[i]==null){
					_idxSelectedInMount=i;//preselect the first item available
					break;
				}
			}
			panelMain.Invalidate();
			//still might not be one selected
		}

		public void printDocument_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Graphics g=e.Graphics;
			g.InterpolationMode=InterpolationMode.HighQualityBicubic;
			g.CompositingQuality=CompositingQuality.HighQuality;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.PixelOffsetMode=PixelOffsetMode.HighQuality;
			using Font fontTitle=new Font(FontFamily.GenericSansSerif,12,FontStyle.Bold);
			using Font fontSubTitles=new Font(FontFamily.GenericSansSerif,10);
			int xTitle;
			int yTitle;
			string str;
			int widthStr;
			if(IsDocumentShowing()){
				Document document=GetDocumentShowing(0);
				xTitle=e.MarginBounds.X+e.MarginBounds.Width/2;
				yTitle=e.MarginBounds.Top;
				if(document.ImgType==ImageType.Radiograph) {
					str=PatientCur.GetNameLF();
					widthStr=(int)g.MeasureString(str,fontTitle).Width;
					g.DrawString(str,fontTitle,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontTitle.Height+4;
					str="DOB: "+PatientCur.Birthdate.ToShortDateString();
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
					str=document.DateCreated.ToShortDateString();
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
					if(document.ProvNum!=0) {
						str=Providers.GetFormalName(document.ProvNum);
						widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
						g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
						yTitle+=fontSubTitles.Height;
					}
					if(document.Description!="") {
						str=document.Description;
						widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
						g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
						yTitle+=fontSubTitles.Height;
					}
					if(document.ToothNumbers!="") {
						str=Lan.g(this,"Tooth numbers: ") +document.ToothNumbers;
						widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
						g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
						yTitle+=fontSubTitles.Height;
					}
				}
				yTitle+=20;
				Rectangle rectangleAvail=new Rectangle(e.MarginBounds.X,yTitle,e.MarginBounds.Width,e.MarginBounds.Height-yTitle);
				//translate to center of drawing area (and center of crop area)
				g.TranslateTransform(rectangleAvail.X+rectangleAvail.Width/2,rectangleAvail.Y+rectangleAvail.Height/2);				
				float scaleFactor;
				if(GetDocumentShowing(0).CropW==0){//no crop
					scaleFactor=ImageTools.CalcScaleFit(rectangleAvail.Size,GetBitmapShowing(0).Size,GetDocumentShowing(0).DegreesRotated);
				}
				else{
					scaleFactor=ImageTools.CalcScaleFit(rectangleAvail.Size,new Size(GetDocumentShowing(0).CropW,GetDocumentShowing(0).CropH),GetDocumentShowing(0).DegreesRotated);
				}
				g.ScaleTransform(scaleFactor,scaleFactor);
				//We are now at center of page and crop area, and working in image coords
				DrawDocument(g);
				e.HasMorePages=false;
				return;
			}
			if(!IsMountShowing()){
				e.HasMorePages=false;
				return;
			}
			//mount from here down-------------------------------------------------------------------------------
			if(_idxSelectedInMount>-1 && _documentArrayShowing[_idxSelectedInMount]!=null) {
				//Single mount item
				xTitle=e.MarginBounds.X+e.MarginBounds.Width/2;
				yTitle=e.MarginBounds.Top;
				str=PatientCur.GetNameLF();
				widthStr=(int)g.MeasureString(str,fontTitle).Width;
				g.DrawString(str,fontTitle,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontTitle.Height+4;
				str="DOB: "+PatientCur.Birthdate.ToShortDateString();
				widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
				g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontSubTitles.Height;
				str=_documentArrayShowing[_idxSelectedInMount].DateCreated.ToShortDateString();
				widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
				g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontSubTitles.Height;
				if(_documentArrayShowing[_idxSelectedInMount].ProvNum!=0){
					str=Providers.GetFormalName(_documentArrayShowing[_idxSelectedInMount].ProvNum);
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
				}
				if(_documentArrayShowing[_idxSelectedInMount].Description!=""){
					str=_documentArrayShowing[_idxSelectedInMount].Description;
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
				}
				if(_documentArrayShowing[_idxSelectedInMount].ToothNumbers!=""){
					str=Lan.g(this,"Tooth numbers: ") +_documentArrayShowing[_idxSelectedInMount].ToothNumbers;
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
				}
				yTitle+=20;
				Rectangle rectangleAvail=new Rectangle(e.MarginBounds.X,yTitle,e.MarginBounds.Width,e.MarginBounds.Height-yTitle);
				//translate to center of drawing area (and center of mount item)
				g.TranslateTransform(rectangleAvail.X+rectangleAvail.Width/2,rectangleAvail.Y+rectangleAvail.Height/2);
				float scaleMount=ImageTools.CalcScaleFit(
					sizeCanvas: rectangleAvail.Size,
					sizeImage: new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
					degreesRotated: 0);//the mount item itself is not rotated
				g.ScaleTransform(scaleMount,scaleMount);
				//we are now at center of page/mount item and in mount scale 
				//Move from center of mount item to UL of mount item
				g.TranslateTransform(-_listMountItems[_idxSelectedInMount].Width/2,-_listMountItems[_idxSelectedInMount].Height/2);
				DrawMountOne(g,_idxSelectedInMount,translateToItemPos:false);
				g.SetClip(new Rectangle(0,0,_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height));
				//translate from UL of mount item to UL of mount
				g.TranslateTransform(-_listMountItems[_idxSelectedInMount].Xpos,-_listMountItems[_idxSelectedInMount].Ypos);
				DrawDrawings(g,0);
				e.HasMorePages=false;
				return;
			}
			//entire mount from here down----------------------------------------------------------------------------------------------------------------------------
			//This needs different drawing strategy
			g.TranslateTransform(e.MarginBounds.X+e.MarginBounds.Width/2,e.MarginBounds.Y+e.MarginBounds.Height/2);//Center of page
			int heightTitle=55;
			bool isWide=false;
			if((double)_mountShowing.Width/(_mountShowing.Height+heightTitle) > (double)e.MarginBounds.Width/e.MarginBounds.Height){
				isWide=true;
			}
			float scale;
			if(isWide){//print landscape
				g.RotateTransform(90);
				scale=ImageTools.CalcScaleFit(new Size(e.MarginBounds.Height,e.MarginBounds.Width-heightTitle),new Size(_mountShowing.Width,_mountShowing.Height),0);
			}
			else{
				scale=ImageTools.CalcScaleFit(new Size(e.MarginBounds.Width,e.MarginBounds.Height-heightTitle),new Size(_mountShowing.Width,_mountShowing.Height),0);
			}
			//Title------------------------------------------
			//GraphicsState graphicsStateCenter=g.Save();//if we want to use translation instead of x y math.
			xTitle=0;//from center
			if(isWide){
				yTitle=(int)(-_mountShowing.Height*scale/2)-heightTitle;
			}
			else{
				yTitle=(int)(-_mountShowing.Height*scale/2)-heightTitle;//same for now, but might change
			}
			str=PatientCur.GetNameLF();
			widthStr=(int)g.MeasureString(str,fontTitle).Width;
			g.DrawString(str,fontTitle,Brushes.Black,xTitle-widthStr/2,yTitle);
			yTitle+=fontTitle.Height+4;
			str="DOB: "+PatientCur.Birthdate.ToShortDateString();
			widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
			g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
			yTitle+=fontSubTitles.Height;
			str=_mountShowing.DateCreated.ToShortDateString();
			widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
			g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
			yTitle+=fontSubTitles.Height;
			if(_mountShowing.ProvNum!=0){
				str=Providers.GetFormalName(_mountShowing.ProvNum);
				widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
				g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontSubTitles.Height;
			}
			if(_mountShowing.Description!=""){
				str=_mountShowing.Description;
				widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
				g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontSubTitles.Height;
			}
			//g.Restore(graphicsStateCenter);
			//Image-----------------------------------------
			g.TranslateTransform(0,heightTitle);
			g.ScaleTransform(scale,scale);
			DrawMount(g);
			e.HasMorePages=false;
		}

		public void SelectTreeNode(NodeTypeAndKey nodeTypeAndKey,string localPathImportedCloud="") {
			_pointTranslation=new Point();
			panelMain.Visible=true;
			_cloudIframe?.HideIframe();
			ClearObjects();
			if(_odWebView2!=null) {
				_odWebView2.Visible=false;
				IsImageFloatLocked=false;
			}
			if(nodeTypeAndKey is null){
				Text="";
				return;
			}
			_nodeTypeKeyCatSelected=new NodeTypeKeyCat();
			_nodeTypeKeyCatSelected.ImageNodeType=nodeTypeAndKey.NodeType;
			_nodeTypeKeyCatSelected.PriKey=nodeTypeAndKey.PriKey;
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.None){
				Text="";
			}
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Category){
				_nodeTypeKeyCatSelected.DefNumCategory=nodeTypeAndKey.PriKey;
				Text="";
			}
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Document){
				Document document=Documents.GetByNum(nodeTypeAndKey.PriKey,doReturnNullIfNotFound:true);
				if(document==null) {
					SetDocumentShowing(0,null);
					MsgBox.Show(this,"Document was previously deleted.");
					EventFillTree?.Invoke(this,false);
					return;
				}
				SetDocumentShowing(0,document);
				_listImageDraws=ImageDraws.RefreshForDoc(document.DocNum);
				_nodeTypeKeyCatSelected.DefNumCategory=document.DocCategory;
				Text=document.DateCreated.ToShortDateString()+": "+document.Description;
				//no need to show document note in title because it shows as bottom in separate panel.
				if(document.ToothNumbers!=""){
					if(document.ToothNumbers.Contains(",")){
						Text+=", "+Lan.g(this,"Teeth")+": ";
					}
					else{
						Text+=", "+Lan.g(this,"Tooth")+": ";
					}
					Text+=Tooth.DisplayRange(document.ToothNumbers);
				}
				_bitmapArrayShowing=new Bitmap[1]; 
				if(CloudStorage.IsCloudStorage) {
					ProgressOD progressOD=new ProgressOD();
					progressOD.ActionMain=() =>	LoadBitmap(0,EnumLoadBitmapType.IdxAndRaw);
					progressOD.StartingMessage=Lan.g("ContrImages","Downloading...");
					progressOD.ShowDialogProgress();
					if(progressOD.IsCancelled){
						return;
					}
				}
				else{
					LoadBitmap(0,EnumLoadBitmapType.IdxAndRaw);
				}
				//_bitmapRaw will always be null for PDFs
				//Diverges slightly from the normal use of this event, in that it is fired from SelectTreeNode() rather than ModuleSelected.  Appropriate
				//here because this is the only data in ContrImages that might affect the PatientDashboard, and there is no "LoadData" in this Module.
				PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected
					,new PatientDashboardDataEventArgs() {
						Pat=PatientCur,
						ListDocuments=_documentArrayShowing.ToList(),
						BitmapImagesModule=_bitmapRaw
					}
				);
				DownloadDocumentNoteFile(nodeTypeAndKey);
				if(_bitmapRaw==null && _bitmapDicomRaw==null) {
					panelMain.Visible=false;
					if(ImageHelper.HasImageExtension(document.FileName)) {
						string srcFileName = ODFileUtils.CombinePaths(PatFolder,document.FileName);
						if(File.Exists(srcFileName)) {
							MessageBox.Show(Lan.g(this,"File found but cannot be opened, probably because it's too big:")+srcFileName);
						}
						else {
							MessageBox.Show(Lan.g(this,"File not found")+": " + srcFileName);
						}
					}
					else if(Path.GetExtension(document.FileName).ToLower()==".pdf") {
						if(!PrefC.GetBool(PrefName.PdfLaunchWindow)) {
							LoadPdf(PatFolder,document.FileName,localPathImportedCloud,"Downloading Document...");
						}
					}
				}
				ImageHelper.ConvertCropIfNeeded(document,_bitmapRaw);
				SetWindowingSlider();
				EnableToolBarButtons();
				EventSetCropPanEditAdj?.Invoke(this,EnumCropPanAdj.Pan);
			}
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Mount){
				_mountShowing=Mounts.GetByNum(nodeTypeAndKey.PriKey);
				_listImageDraws=ImageDraws.RefreshForMount(_mountShowing.MountNum);
				_nodeTypeKeyCatSelected.DefNumCategory=_mountShowing.DocCategory;
				Text=_mountShowing.DateCreated.ToShortDateString()+": "+_mountShowing.Description;
				if(_mountShowing.Note!=""){
					Text+=", "+_mountShowing.Note;
				}
				_listMountItems=MountItems.GetItemsForMount(_mountShowing.MountNum);
				_documentArrayShowing=Documents.GetDocumentsForMountItems(_listMountItems);
				_idxSelectedInMount=-1;//No selection to start.
				//_arrayBitmapsRaw=ImageStore.OpenImages(_arrayDocumentsShowing,_patFolder,localPathImportedCloud);
				_bitmapArrayShowing=new Bitmap[_documentArrayShowing.Length]; 
				List<int> listMissingMountNums=new List<int>();//List to count missing images not found in the A-Z folder
				Cursor=Cursors.WaitCursor;
				for(int i=0;i<_documentArrayShowing.Length;i++){
					if(_documentArrayShowing[i]==null){
						_bitmapArrayShowing[i]=null;
						continue;
					}
					if(CloudStorage.IsCloudStorage) {
						//this will flicker since it will be a series of progress bars.  Improve later if needed.
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() =>	LoadBitmap(i,EnumLoadBitmapType.OnlyIdx);
						progressOD.StartingMessage=Lan.g("ContrImages","Downloading...");
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							Cursor=Cursors.Default;
							return;//not sure if we need to do any cleanup
						}
					}
					else{
						LoadBitmap(i,EnumLoadBitmapType.OnlyIdx);
					}
					if(_bitmapArrayShowing[i]==null) {
						listMissingMountNums.Add(i);
					}
					else{
						ImageHelper.ConvertCropIfNeeded(_documentArrayShowing[i],_bitmapArrayShowing[i]);
					}
				}
				if(listMissingMountNums.Count>0) {//Notify user of any files that were unable to load
					string errorMessage=Lan.g(this,"Files not found for mount:")+"\r\n";
					for(int m=0;m<listMissingMountNums.Count;m++) {
						errorMessage+=Lan.g(this,"Mount position ")+(listMissingMountNums[m]+1)+": "+_documentArrayShowing[listMissingMountNums[m]].FileName;
						if(m!=listMissingMountNums.Count-1) {
							errorMessage+="\r\n";
						}
					}
					MsgBox.Show(errorMessage);
				}
				EnableToolBarButtons();
				CreateMountThumbnail();
				EventSetCropPanEditAdj?.Invoke(this,EnumCropPanAdj.Pan);
				Cursor=Cursors.Default;
			}
			SetZoomSlider();
			panelMain.Invalidate();
		}

		///<summary>One of these 3 states is active at any time.  This allows the value to flow down into this window.  To send cropPanAdj up to parent, use the event.</summary>
		public void SetCropPanAdj(EnumCropPanAdj cropPanAdj){
			_cropPanAdj=cropPanAdj;
			switch(_cropPanAdj){
				case EnumCropPanAdj.Crop:
					panelMain.Cursor=Cursors.Arrow;
					break;
				case EnumCropPanAdj.Pan:
					panelMain.Cursor=_cursorPan;
					break;
				case EnumCropPanAdj.Adj:
					panelMain.Cursor=Cursors.SizeAll;
					break;
			}
			panelMain.Invalidate();
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Sets the value of the element in _arrayDocumentsShowing.</summary>
		public void SetDocumentShowing(int idx,Document document){
			if(IsMountShowing()){
				//should be correct length
			}
			else{
				//this covers documents as well as nothing showing
				if(_documentArrayShowing==null || _documentArrayShowing.Length!=1){
					_documentArrayShowing=new Document[1];
				}
			}
			if(_documentArrayShowing.Length>0 && idx<_documentArrayShowing.Length) {
				_documentArrayShowing[idx]=document;
			}
		}

		///<summary>One of these 6 modes is active at any time.  This allows the value to flow down into this window.  Also see EventSetDrawMode, which bubbles requests up to the parent.</summary>
		public void SetDrawMode(EnumDrawMode drawMode){
			_drawMode=drawMode;
			switch(_drawMode){
				case EnumDrawMode.None:
					_idxTextMouseDown=-1;
					panelMain.Cursor=_cursorPan;
					break;
				case EnumDrawMode.Text:
					panelMain.Cursor=Cursors.IBeam;
					break;
				case EnumDrawMode.Line:
					panelMain.Cursor=_cursorCrosshair;
					break;
				case EnumDrawMode.LineEditPoints:
					panelMain.Cursor=_cursorLinePoint;
					break;
				case EnumDrawMode.LineMeasure:
					panelMain.Cursor=_cursorMeasure;
					ToolBarMeasure_Click();
					break;
				case EnumDrawMode.LineSetScale:
					panelMain.Cursor=_cursorScale;
					ToolBarSetScale_Click();
					break;
				case EnumDrawMode.Pen:
				case EnumDrawMode.Polygon://for now, just use the pen
					using(MemoryStream memoryStream=new MemoryStream(Properties.Resources.Pen)){
						panelMain.Cursor=new Cursor(memoryStream);
					}
					break;
				case EnumDrawMode.Eraser:
					using(MemoryStream memoryStream=new MemoryStream(Properties.Resources.EraseCircle)){
						panelMain.Cursor=new Cursor(memoryStream);
					}
					break;
				case EnumDrawMode.ChangeColor:
					using(MemoryStream memoryStream=new MemoryStream(Properties.Resources.ColorChanger)){
						panelMain.Cursor=new Cursor(memoryStream);
					}
					break;
			}
			panelMain.Invalidate();
		}

		public void SetIdxSelectedInMount(int idx){
			if(_documentArrayShowing is null){
				return;
			}
			if(idx > _documentArrayShowing.Length-1){
				return;
			}
			_idxSelectedInMount=idx;
			panelMain.Invalidate();
		}

		public void SetShowDrawings(bool showOD,bool showPearl){
			_showDrawingsOD=showOD;
			_showDrawingsPearl=showPearl;
			panelMain.Invalidate();
		}

		///<summary>This is the part of the flow for taking the current windowing settings for a doc and passing them up to parent so that the slider matches.  Dragging the slider skips all this and the parent sets the windowing directly.</summary>
		public void SetWindowingSlider() {
			WindowingEventArgs windowingEventArgs=new WindowingEventArgs();
			if(IsDocumentShowing()){
				if(GetDocumentShowing(0).WindowingMax==0) {//default
					windowingEventArgs.MinVal=0;
					windowingEventArgs.MaxVal=255;
				}
				else {
					windowingEventArgs.MinVal=GetDocumentShowing(0).WindowingMin;
					windowingEventArgs.MaxVal=GetDocumentShowing(0).WindowingMax;
				}
			}
			else if(IsMountItemSelected()){
				if(_documentArrayShowing[_idxSelectedInMount].WindowingMax==0) {//default
					windowingEventArgs.MinVal=0;
					windowingEventArgs.MaxVal=255;
				}
				else {
					windowingEventArgs.MinVal=_documentArrayShowing[_idxSelectedInMount].WindowingMin;
					windowingEventArgs.MaxVal=_documentArrayShowing[_idxSelectedInMount].WindowingMax;
				}
			}
			else{
				windowingEventArgs.MinVal=0;
				windowingEventArgs.MaxVal=255;
			}
			EventSetWindowingSlider?.Invoke(this,windowingEventArgs);
		}

		///<summary>This sets the zoom slider so that the middle tick will be at the "fit" zoom.  This is needed any time the image is rotate or cropped, or if the form size changes.</summary>
		public void SetZoomSlider(){
			if(IsDocumentShowing()){
				if(_bitmapRaw==null && _bitmapDicomRaw==null){
					//pdf. It will be disabled anyway
					ZoomSliderStateInitial=new ZoomSliderState(panelMain.Size,panelMain.Size,0);//100
					EventSetZoomSlider?.Invoke(this,ZoomSliderStateInitial);
					return;
				}
				if(_bitmapRaw!=null){
					Size sizeImage=_bitmapRaw.Size;
					if(GetDocumentShowing(0).CropW>0){
						sizeImage=new Size(GetDocumentShowing(0).CropW,GetDocumentShowing(0).CropH);
					}
					ZoomSliderStateInitial=new ZoomSliderState(panelMain.Size,sizeImage,GetDocumentShowing(0).DegreesRotated);
					EventSetZoomSlider?.Invoke(this,ZoomSliderStateInitial);
				}
				if(_bitmapDicomRaw!=null){
					Size sizeImage=new Size(_bitmapDicomRaw.Width,_bitmapDicomRaw.Height);
					if(GetDocumentShowing(0).CropW>0){
						sizeImage=new Size(GetDocumentShowing(0).CropW,GetDocumentShowing(0).CropH);
					}
					ZoomSliderStateInitial=new ZoomSliderState(panelMain.Size,sizeImage,GetDocumentShowing(0).DegreesRotated);
					EventSetZoomSlider?.Invoke(this,ZoomSliderStateInitial);
				}
			}
			if(IsMountShowing()){
				ZoomSliderStateInitial=new ZoomSliderState(panelMain.Size,new Size(_mountShowing.Width,_mountShowing.Height),0);
				EventSetZoomSlider?.Invoke(this,ZoomSliderStateInitial);
			}
		}

		///<summary>Three objects will end up on clipboard: 1:NodeTypeAndKey , 2:Bitmap(if it's an image type), 3:FileDrop. For images, the filedrop will point to a temp copy of the saved bitmap.  For non-images, filedrop will point to the original file.</summary>
		public void ToolBarCopy_Click(){
			//not enabled when pdf selected
			NodeTypeAndKey nodeTypeAndKey=null;
			Bitmap bitmapCopy=null;
			string fileName="";
			Cursor=Cursors.WaitCursor;
			if(IsMountItemSelected()){
				if(ImageHelper.HasImageExtension(GetDocumentShowing(_idxSelectedInMount).FileName) || GetDocumentShowing(_idxSelectedInMount).FileName.EndsWith(".dcm"))
				{
					//normal images and dicom will have the bitmap and filedrop pulled from the screen
					nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,_documentArrayShowing[_idxSelectedInMount].DocNum);
					bitmapCopy=new Bitmap(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
					using Graphics g=Graphics.FromImage(bitmapCopy);
					//translate to center of drawing area (and center of mount item)
					g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);
					float scaleMount=ImageTools.CalcScaleFit(
						sizeCanvas: bitmapCopy.Size,
						sizeImage: new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
						degreesRotated: 0);//the mount item itself is not rotated
					g.ScaleTransform(scaleMount,scaleMount);
					//we are now at center of page/mount item and in mount scale 
					//Move from center of mount item to UL of mount item
					g.TranslateTransform(-_listMountItems[_idxSelectedInMount].Width/2,-_listMountItems[_idxSelectedInMount].Height/2);
					DrawMountOne(g,_idxSelectedInMount,translateToItemPos:false);
					g.SetClip(new Rectangle(0,0,_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height));
					//translate from UL of mount item to UL of mount
					g.TranslateTransform(-_listMountItems[_idxSelectedInMount].Xpos,-_listMountItems[_idxSelectedInMount].Ypos);
					DrawDrawings(g,0);
					//file is always copy of image instead of original file, even for dicom.  NodeTypeAndKey still allows true dicom copy/paste.
					fileName=Path.GetTempPath()+_documentArrayShowing[_idxSelectedInMount].FileName;
					bitmapCopy.Save(fileName);
				}
				else{
					//other files such as pdf will lack a bitmapCopy
					fileName=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),GetDocumentShowing(_idxSelectedInMount).FileName);
				}
			}
			else if(IsMountShowing()){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum);
				//Bitmap and file are a composite image of the mount for external paste.
				//NodeTypeAndKey still allows for copying the mount itself, along with all attached mount items, if within OD.
				bitmapCopy=new Bitmap(_mountShowing.Width,_mountShowing.Height);
				using Graphics g=Graphics.FromImage(bitmapCopy);
				g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);//Center of image
				DrawMount(g);
				fileName=Path.GetTempPath()+PatientCur.LName+PatientCur.FName+".jpg";
				bitmapCopy.Save(fileName);
			}
			else if(IsDocumentShowing()){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,GetDocumentShowing(0).DocNum);
				if(ImageHelper.HasImageExtension(GetDocumentShowing(0).FileName) || GetDocumentShowing(0).FileName.EndsWith(".dcm")){
					fileName=Path.GetTempPath()+GetDocumentShowing(0).FileName;
					if(!ImageHelper.HasImageBeenEdited(GetDocumentShowing(0))) {//Do a file copy since no changes were made
						string filePathSource=Documents.GetPath(GetDocumentShowing(0).DocNum);
						while(File.Exists(fileName)) {
							Random random=new Random();
							fileName=Path.GetDirectoryName(fileName)+"\\"+Path.GetFileNameWithoutExtension(fileName)+random.Next(9)+Path.GetExtension(fileName);
						}
						File.Copy(filePathSource,fileName);
						if(ODBuild.IsWeb()){//File path/name will not exist. Still copy the Bitmap so that it can be pasted on local computer.
							bitmapCopy=ImageHelper.CopyWithCropRotate(GetDocumentShowing(0),GetBitmapShowing(0));
						}
					}
					else {
						bitmapCopy=ImageHelper.CopyWithCropRotate(GetDocumentShowing(0),GetBitmapShowing(0));
						using Graphics g=Graphics.FromImage(bitmapCopy);
						//Rectangle rectangleAvail=new Rectangle(e.MarginBounds.X,yTitle,e.MarginBounds.Width,e.MarginBounds.Height-yTitle);
						//translate to center of drawing area (and center of crop area)
						g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);
						//We are now at center of page and crop area, and working in image coords
						DrawDocument(g);//including drawings
						ImageStore.SaveBitmapJpg(bitmapCopy,fileName,quality:40);//60% compression
					}
				}
				else{
					//other files such as pdf will lack a bitmapCopy
					fileName=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),GetDocumentShowing(0).FileName);
				}
			}
			else{
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please make a selection before copying");
				return;
			}
			if(nodeTypeAndKey==null && bitmapCopy==null && fileName=="") {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Error.");
				return;
			}
			System.Windows.DataObject dataObject=new System.Windows.DataObject();
			if(nodeTypeAndKey!=null){
				dataObject.SetData(nodeTypeAndKey);
			}
			if(bitmapCopy!=null){
				dataObject.SetData(DataFormats.Bitmap,bitmapCopy);
			}
			if(fileName!=""){
				string[] stringArray=new string[1];
				stringArray[0]=fileName;
				dataObject.SetData(DataFormats.FileDrop,stringArray);
			}
			if(ODBuild.IsWeb()){
				int nodeType=(int)nodeTypeAndKey.NodeType;
				ODCloudClient.CopyToClipboard(bitmapCopy,fileName,nodeType,nodeTypeAndKey.PriKey);
			}
			else{
				try {
					System.Windows.Clipboard.SetDataObject(dataObject);//System.Windows.Forms.Clipboard fails for Thinfinity
				}
				catch(Exception ex) {
					MsgBox.Show(this,"Could not copy contents to the clipboard.  Please try again.");
					ex.DoNothing();
					return;
				}
			}
			//Can't do this, or the clipboard object goes away.
			//bitmapCopy.Dispose();
			//bitmapCopy=null;
			long patNum=0;
			if(PatientCur!=null) {
				patNum=PatientCur.PatNum;
			}
			Cursor=Cursors.Default;
			if(IsMountItemSelected()){
				SecurityLogs.MakeLogEntry(EnumPermType.Copy,patNum,"Patient image "+_documentArrayShowing[_idxSelectedInMount].FileName+" copied to clipboard");
			}
			else if(IsMountShowing()){
				SecurityLogs.MakeLogEntry(EnumPermType.Copy,patNum,"Patient mount "+_mountShowing.Description+" copied to clipboard");
			}
			else if(IsDocumentShowing()){
				SecurityLogs.MakeLogEntry(EnumPermType.Copy,patNum,"Patient image "+GetDocumentShowing(0).FileName+" copied to clipboard");
			}
		}

		public void ToolBarDelete_Click(){
			if(GetSelectedType()==EnumImageNodeType.None || ImageSelector_.GetSelectedType()==EnumImageNodeType.None){
				MsgBox.Show(this,"No item is currently selected");
				return;
			}
			//can't get to here if category selected
			NodeTypeAndKey nodeTypeAndKeyAbove=ImageSelector_.GetItemAbove();//always successful
			if(IsDocumentShowing()){
				DeleteDocument(true,true,GetDocumentShowing(0));//security is inside this method
				EventSelectTreeNode?.Invoke(this,nodeTypeAndKeyAbove);
				return;
			}
			if(IsMountItemSelected()){
				DeleteDocument(true,true,_documentArrayShowing[_idxSelectedInMount]);
				return;
			}
			if(!IsMountShowing()){
				return;
			}
			bool isEmpty=true;
			List<MountItem> listMountItems=MountItems.GetItemsForMount(_mountShowing.MountNum);
			List<Document> listDocumentsShowing=_documentArrayShowing.Where(x => x!=null).ToList();
			Document[] documentArrayInMountDb=Documents.GetDocumentsForMountItems(listMountItems);
			for(int i=0;i<documentArrayInMountDb.Length;i++) {
				if(documentArrayInMountDb[i]==null){
					continue;
				}
				isEmpty=false;
				Document document=listDocumentsShowing.Find(x=> x.DocNum==documentArrayInMountDb[i].DocNum);
				if(document!=null){//this document from db is showing to user, so no concurrency issue.
					continue;
				}
				MsgBox.Show(this,"This mount is currently open on another workstation.  Please refresh the Imaging Module and try again.");
				return;
			}
			if(!isEmpty){
				if(!Security.IsAuthorized(EnumPermType.ImageDelete,_mountShowing.DateCreated)) {
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete the entire mount and all images?")){
					return;
				}
				for(int i=0;i<_documentArrayShowing.Length;i++){
					if(_documentArrayShowing[i]!=null){
						Statements.DetachDocFromStatements(_documentArrayShowing[i].DocNum);//overkill
					}
				}
				try {
					ImageStore.DeleteDocuments(_documentArrayShowing,PatFolder);
				}
				catch(Exception ex) {  //Image could not be deleted, in use.
					MessageBox.Show(this,ex.Message);
					return;
				}
			}
			Mounts.Delete(_mountShowing);
			Def defDocCategory = Defs.GetDef(DefCat.ImageCats,_mountShowing.DocCategory);
			string logText = "Mount Deleted: "+_mountShowing.Description+" with category "
				+defDocCategory.ItemName;
			SecurityLogs.MakeLogEntry(EnumPermType.ImageDelete,PatientCur.PatNum,logText);
			EventFillTree?.Invoke(this,false);
			EventSelectTreeNode?.Invoke(this,nodeTypeAndKeyAbove);
		}

		public void ToolBarExport_Click(bool doExportAsTiff=false){
			if(ODBuild.IsWeb()) {
				ToolBarExport_ClickWeb();
				return;
			}
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(EnumPermType.ImageExport,GetDocumentShowing(0).DateCreated)) {
					return;
				}		
				bool isTiff = Path.GetExtension(GetDocumentShowing(0).FileName)==".tiff" || Path.GetExtension(GetDocumentShowing(0).FileName)==".tif";
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export a Document";
				saveFileDialog.Filter="All Files|*.*|JPEG|*.jpeg|TIFF|*.tiff|PNG|*.png|GIF|*.gif|BMP|*.bmp";
				if(doExportAsTiff) {
					if(isTiff) {
						saveFileDialog.FileName=GetDocumentShowing(0).FileName;
					}
					else {
						MessageBox.Show(this,"Only allowed when the source file is a .tiff or .tif.");
						return;
					}
				}
				else {//not trying to export as tiff
					if(isTiff) {
						string newFileName=GetDocumentShowing(0).FileName.Replace(Path.GetExtension(GetDocumentShowing(0).FileName), ".jpg");
						saveFileDialog.FileName=newFileName;
					}
					else {
						saveFileDialog.FileName=GetDocumentShowing(0).FileName;
					}
				}
				saveFileDialog.DefaultExt=Path.GetExtension(saveFileDialog.FileName);
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				if(ImageHelper.HasImageExtension(GetDocumentShowing(0).FileName) || GetDocumentShowing(0).FileName.EndsWith(".dcm")){
					using Bitmap bitmapCopy=ImageHelper.CopyWithCropRotate(GetDocumentShowing(0),GetBitmapShowing(0));
					if(bitmapCopy==null){
						MessageBox.Show(Lan.g(this,"Unable to export, file not found."));
						return;
					}
					using Graphics g=Graphics.FromImage(bitmapCopy);
					//Rectangle rectangleAvail=new Rectangle(e.MarginBounds.X,yTitle,e.MarginBounds.Width,e.MarginBounds.Height-yTitle);
					//translate to center of drawing area (and center of crop area)
					g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);
					//We are now at center of page and crop area, and working in image coords
					DrawDocument(g);//including drawings
					ImageStore.SaveBitmap(bitmapCopy,saveFileDialog.FileName,quality:90);
				}
				else{
					//other files such as pdf will lack a bitmapCopy
					try {
						ImageStore.Export(saveFileDialog.FileName,GetDocumentShowing(0),PatientCur);
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Unable to export file, may be in use")+": " + ex.Message);
						return;
					}
				}				
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Exported: "+GetDocumentShowing(0).FileName+" with category "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(EnumPermType.ImageExport,PatientCur.PatNum,logText,GetDocumentShowing(0).DocNum,GetDocumentShowing(0).DateTStamp);
				return;
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(EnumPermType.ImageExport,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				bool isTiff = Path.GetExtension(_documentArrayShowing[_idxSelectedInMount].FileName)==".tiff" || Path.GetExtension(_documentArrayShowing[_idxSelectedInMount].FileName)==".tif";
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export Image";
				saveFileDialog.Filter="All Files|*.*|JPEG|*.jpeg|TIFF|*.tiff|PNG|*.png|GIF|*.gif|BMP|*.bmp";
				if(doExportAsTiff) {
					if(isTiff) {
						saveFileDialog.FileName=_documentArrayShowing[_idxSelectedInMount].FileName;
					}
					else {
						MessageBox.Show(this,"Only allowed when the source file is a .tiff or .tif.");
						return;
					}
				}
				else {//not trying to export as tiff
					if(isTiff) {
						string newFileName=GetDocumentShowing(_idxSelectedInMount).FileName.Replace(Path.GetExtension(GetDocumentShowing(_idxSelectedInMount).FileName), ".jpg");
						saveFileDialog.FileName=newFileName;
					}
					else {
						saveFileDialog.FileName=GetDocumentShowing(_idxSelectedInMount).FileName;
					}
				}
				saveFileDialog.DefaultExt=Path.GetExtension(saveFileDialog.FileName);
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				if(ImageHelper.HasImageExtension(GetDocumentShowing(_idxSelectedInMount).FileName) || GetDocumentShowing(_idxSelectedInMount).FileName.EndsWith(".dcm"))
				{
					//normal images and dicom will have the bitmap pulled from the screen
					using Bitmap bitmapCopy=new Bitmap(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
					using Graphics g=Graphics.FromImage(bitmapCopy);
					//translate to center of drawing area (and center of mount item)
					g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);
					float scaleMount=ImageTools.CalcScaleFit(
						sizeCanvas: bitmapCopy.Size,
						sizeImage: new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
						degreesRotated: 0);//the mount item itself is not rotated
					g.ScaleTransform(scaleMount,scaleMount);
					//we are now at center of page/mount item and in mount scale 
					//Move from center of mount item to UL of mount item
					g.TranslateTransform(-_listMountItems[_idxSelectedInMount].Width/2,-_listMountItems[_idxSelectedInMount].Height/2);
					DrawMountOne(g,_idxSelectedInMount,translateToItemPos:false);
					g.SetClip(new Rectangle(0,0,_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height));
					//translate from UL of mount item to UL of mount
					g.TranslateTransform(-_listMountItems[_idxSelectedInMount].Xpos,-_listMountItems[_idxSelectedInMount].Ypos);
					DrawDrawings(g,0);
					ImageStore.SaveBitmap(bitmapCopy,saveFileDialog.FileName,quality:90);
				}
				else{
					//shouldn't be pdfs, etc in mounts, but just in case:
					try {
						ImageStore.Export(saveFileDialog.FileName,_documentArrayShowing[_idxSelectedInMount],PatientCur);
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Unable to export file, May be in use")+": " + ex.Message);
						return;
					}
				}
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Exported: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(EnumPermType.ImageExport,PatientCur.PatNum,logText,_documentArrayShowing[_idxSelectedInMount].DocNum,
					_documentArrayShowing[_idxSelectedInMount].DateTStamp);
				return;
			}
			if(IsMountShowing()){
				if(!Security.IsAuthorized(EnumPermType.ImageExport,_mountShowing.DateCreated)) {
					return;
				}
				if(doExportAsTiff) {
					MessageBox.Show(this,"Not available for mounts. Export will be jpg.");
					return;
				}
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export Mount";
				saveFileDialog.FileName= Documents.GenerateUniqueFileName(".jpg",PatientCur);//_documentShowing.FileName;
				saveFileDialog.DefaultExt=".jpg";
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				Bitmap bitmapExport=new Bitmap(_mountShowing.Width,_mountShowing.Height);
				Graphics g=Graphics.FromImage(bitmapExport);
				g.TranslateTransform(bitmapExport.Width/2,bitmapExport.Height/2);//Center of image
				DrawMount(g);
				g.Dispose();
				try {
					bitmapExport.Save(saveFileDialog.FileName);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to export file, May be in use")+": " + ex.Message);
					return;
				}
				bitmapExport.Dispose();
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_mountShowing.DocCategory);
				string logText="Mount Exported: "+_mountShowing.Description+" with category "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(EnumPermType.ImageExport,PatientCur.PatNum,logText);
				return;
			}
		}

		public void ToolBarFlip_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				Document documentOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).IsFlipped=!GetDocumentShowing(0).IsFlipped;
				//Document is always flipped and then rotated when drawn, but we want to flip it
				//horizontally no matter what orientation it's in right now
				//This helps with exactly these two situations.  I don't think anyone cares about 92 degrees, for example.
				if(GetDocumentShowing(0).DegreesRotated==90){
					GetDocumentShowing(0).DegreesRotated=270;
				}
				else if(GetDocumentShowing(0).DegreesRotated==270){
					GetDocumentShowing(0).DegreesRotated=90;
				}
				Documents.Update(GetDocumentShowing(0),documentOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with category "
					+defDocCategory.ItemName+" flipped horizontally";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()){ 
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				Document documentOld=_documentArrayShowing[_idxSelectedInMount].Copy();
				_documentArrayShowing[_idxSelectedInMount].IsFlipped=!_documentArrayShowing[_idxSelectedInMount].IsFlipped;
				if(_documentArrayShowing[_idxSelectedInMount].DegreesRotated==90){
					_documentArrayShowing[_idxSelectedInMount].DegreesRotated=270;
				}
				else if(_documentArrayShowing[_idxSelectedInMount].DegreesRotated==270){
					_documentArrayShowing[_idxSelectedInMount].DegreesRotated=90;
				}
				Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
				ImageStore.DeleteThumbnailImage(_documentArrayShowing[_idxSelectedInMount],PatFolder);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" flipped horizontally";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		public void ToolBarImport_Click(){
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}
			//"Alternate" appended to hook name because "ToolBarImport_Click_Start" already exists in ToolBarImportSingle()
			if(Plugins.HookMethod(this,"ContrImages.ToolBarImport_Click_Start_Alternate",PatientCur)) {
				EventFillTree?.Invoke(this,true);
				return;
			}
			if(IsMountShowing()){
				ToolBarImportMount();
			}
			else{//including nothing selected
				ToolBarImportSingle();
			}
		}

		public void ToolBarInfo_Click(){
			if(IsMountItemSelected()){
				if(_documentArrayShowing[_idxSelectedInMount]==null){
					return;//silent fail is fine
				}
				using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,_documentArrayShowing[_idxSelectedInMount]);
				formDocInfo.IsMountItem=true;
				formDocInfo.ShowDialog();
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				panelMain.Invalidate();
				return;
			}
			if(IsMountShowing()) {
				using FormMountEdit formMountEdit=new FormMountEdit();
				formMountEdit.MountCur=_mountShowing;
				formMountEdit.ShowDialog();
				//Always reload because layout could have changed
				Cursor=Cursors.WaitCursor;//because it can take a few seconds to reload.
				EventFillTree?.Invoke(this,true);
				EventSelectTreeNode?.Invoke(this,null);
				Cursor=Cursors.Default;
				return;
			}
			if(IsDocumentShowing()) {
				using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,GetDocumentShowing(0));
				formDocInfo.ShowDialog();
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					return;
				}
				EventFillTree?.Invoke(this,true);
			}
		}

		///<summary>Copy/paste that's entirely within OD will use PK of mount or doc in order to preserve windowing, rotation, crop etc.  Paste from outside OD prefers a bitmap.  If no bitmap, then it will use a filedrop which could include multiple files.</summary>
		public void ToolBarPaste_Click(){
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}
			IDataObject iDataObject=null;
			NodeTypeAndKey nodeTypeAndKey=null;
			if(ODBuild.IsWeb()) {
				if(ODCloudClient.GetNodeTypeAndKey()!=null){
					EnumImageNodeType enumImageNodeTypeCopied=(EnumImageNodeType)ODCloudClient.GetNodeTypeAndKey().nodeType;
					long imagePriKey=ODCloudClient.GetNodeTypeAndKey().imagekey;
					nodeTypeAndKey=new NodeTypeAndKey(enumImageNodeTypeCopied,imagePriKey);					
				}
			}
			else{
				try {
					iDataObject=Clipboard.GetDataObject();
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				if(iDataObject==null){
					MsgBox.Show(this,"Clipboard is empty.");
					return;
				}
				nodeTypeAndKey=(NodeTypeAndKey)iDataObject.GetData(typeof(NodeTypeAndKey));
			}
			if(nodeTypeAndKey!=null){
				ToolBarPasteTypeAndKey(nodeTypeAndKey);
				return;
			}
			Bitmap bitmapPaste=ODClipboard.GetImage();
			string[] stringArrayfileNames=null;
			if(bitmapPaste==null) {//if no bitmap on clipboard, try to get file names
				try {
					stringArrayfileNames=ODClipboard.GetFileDropList();
				}
				catch(Exception ex) {
					ex.DoNothing();//do nothing here, fileNames should remain null and a message box will show below if necessary
				}
			}
			if(bitmapPaste==null && stringArrayfileNames.IsNullOrEmpty()){
				MsgBox.Show(this,"No bitmap or file present on clipboard");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(IsMountShowing()){
				if(bitmapPaste!=null){
					if(_idxSelectedInMount==-1 || _documentArrayShowing[_idxSelectedInMount]!=null){
						Cursor=Cursors.Default;
						MessageBox.Show(Lan.g(this,"Please select an empty mount item, first."));
						return;
					}
					try {
						Document doc=ImageStore.Import(bitmapPaste,GetCurrentCategory(),ImageType.Photo,PatientCur);//Makes log
						Document docOld=doc.Copy();
						doc.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
						doc.ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
						Documents.Update(doc,docOld);
					}
					catch(Exception ex) {
						Cursor=Cursors.Default;
						MessageBox.Show(Lan.g(this,"Unable to paste bitmap: ")+ex.Message);
					}
					Cursor=Cursors.Default;
					EventSelectTreeNode?.Invoke(this,null);
				}
				else{//fileNames
					//fileDrop supports multiple files, and we don't care if they've selected a mount item.
					List<MountItem> listAvail=GetAvailSlots(stringArrayfileNames.Length);
					if(listAvail==null){
						MsgBox.Show("Not enough slots in the mount for the number of files selected.");
						Cursor=Cursors.Default;
						return;
					}
					for(int i=0;i<stringArrayfileNames.Length;i++) {
						Document doc=null;
						try {
							//fileName is full path
							if(CloudStorage.IsCloudStorage) {
								ProgressOD progressOD=new ProgressOD();
								progressOD.ActionMain=() => doc=ImageStore.Import(stringArrayfileNames[i],GetCurrentCategory(),PatientCur);;
								progressOD.ShowDialogProgress();
								if(progressOD.IsCancelled){
									Cursor=Cursors.Default;
									return;//cleanup?
								}
							}
							else{
								doc=ImageStore.Import(stringArrayfileNames[i],GetCurrentCategory(),PatientCur);//Makes log
							}
							Document docOld=doc.Copy();
							doc.MountItemNum=listAvail[i].MountItemNum;
							doc.ToothNumbers=listAvail[i].ToothNumbers;
							Documents.Update(doc,docOld);
						}
						catch(Exception ex) {
							Cursor=Cursors.Default;
							MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+stringArrayfileNames[i]);
							continue;
						}
					}//for					
					Cursor=Cursors.Default;
					EventSelectTreeNode?.Invoke(this,null);
				}//end of filenames
				return;
			}//mount
			//Everything below this line is for one or more documents not in a mount=============================================================================================
			//But we cannot test for this. Needs to work for cat selected, nothing selected, existing doc selected, etc.
			Document document=null;
			nodeTypeAndKey=null;
			if(bitmapPaste!=null){
				try {
					document=ImageStore.Import(bitmapPaste,GetCurrentCategory(),ImageType.Photo,PatientCur);//Makes log entry
				}
				catch {
					Cursor=Cursors.Default;
					MessageBox.Show(Lan.g(this,"Error saving document."));
					return;
				}
				using FormDocInfo formDocInfo = new FormDocInfo(PatientCur,document,isDocCreate: true);
				formDocInfo.TopMost=true;
				formDocInfo.ShowDialog(this);//some of the fields might get changed, but not the filename
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,document);
				}
				else {
					if(document.ImgType==ImageType.Photo) {
						PatientEvent.Fire(ODEventType.Patient,PatientCur);//Possibly updated the patient picture.
					}
					nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum);
					SetDocumentShowing(0,document.Copy());
				}
			}
			else{//files
				for(int i=0;i<stringArrayfileNames.Length;i++) {
					try {
						//fileNames contains full paths
						if(CloudStorage.IsCloudStorage) {
							//this will flicker because multiple progress bars.  Improve later.
							ProgressOD progressOD=new ProgressOD();
							progressOD.ActionMain=() => document=ImageStore.Import(stringArrayfileNames[i],GetCurrentCategory(),PatientCur);
							progressOD.ShowDialogProgress();
							if(progressOD.IsCancelled){
								return;
							}
						}
						else{
							document=ImageStore.Import(stringArrayfileNames[i],GetCurrentCategory(),PatientCur);//Makes log
						}
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+stringArrayfileNames[i]);
						continue;
					}
					using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,document,isDocCreate:true);
					formDocInfo.TopMost=true;
					formDocInfo.ShowDialog(this);//some of the fields might get changed, but not the filename
					if(formDocInfo.DialogResult!=DialogResult.OK) {
						DeleteDocument(false,false,document);
					}
					else {
						if(document.ImgType==ImageType.Photo) {
							PatientEvent.Fire(ODEventType.Patient,PatientCur);//Possibly updated the patient picture.
						}
						nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum);
						SetDocumentShowing(0,document.Copy());
					}
				}//for
			}//files
			EventFillTree?.Invoke(this,false);
			//Select the last successful document that was saved.
			EventSelectTreeNode?.Invoke(this,nodeTypeAndKey);
		}

		public void ToolBarPasteTypeAndKey(NodeTypeAndKey nodeTypeAndKey){
			//Always single item, not series.
			Document document=null;
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Document){//pasting from document
				if(IsMountShowing()){//into mount
					if(_idxSelectedInMount==-1 || _documentArrayShowing[_idxSelectedInMount]!=null){
						//Cursor=Cursors.Default;
						MessageBox.Show(Lan.g(this,"If pasting into a mount, please select an empty mount item, first."));
						return;
					}
				}
				Document documentOriginal=Documents.GetByNum(nodeTypeAndKey.PriKey);
				Patient patientOriginal=Patients.GetPat(documentOriginal.PatNum);
				if(patientOriginal==null || patientOriginal.PatNum==0) {
					MsgBox.Show(this,"Cannot paste content, invalid patient attached to the copied content.");
					return;
				}
				string patientFolder=ImageStore.GetPatientFolder(patientOriginal,ImageStore.GetPreferredAtoZpath());
				if(CloudStorage.IsCloudStorage) {
					byte[] byteArray=ImageStore.GetBytes(documentOriginal,patientFolder);
					document=ImageStore.Import(byteArray,GetCurrentCategory(),documentOriginal.ImgType,PatientCur,fileExtension:Path.GetExtension(documentOriginal.FileName));
				}
				else {
					string sourceFile="";
					try {
						sourceFile=FileAtoZ.CombinePaths(patientFolder,documentOriginal.FileName);
					}
					catch(Exception ex) {
						FriendlyException.Show(Lan.g(this,"Cannot paste content."),ex);
						return;
					}
					//we could be pasting into the same patient or a different patient.
					document=ImageStore.Import(sourceFile,GetCurrentCategory(),PatientCur);
				}
				document.CropH=documentOriginal.CropH;
				document.CropW=documentOriginal.CropW;
				document.CropX=documentOriginal.CropX;
				document.CropY=documentOriginal.CropY;
				document.DegreesRotated=documentOriginal.DegreesRotated;
				document.Description=documentOriginal.Description;
				document.IsFlipped=documentOriginal.IsFlipped;
				document.WindowingMin=documentOriginal.WindowingMin;
				document.WindowingMax=documentOriginal.WindowingMax;
				if(IsMountShowing()){
					document.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
				}
				Documents.Update(document);
				if(IsMountShowing()){//into mount
					EventSelectTreeNode?.Invoke(this,new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
				}
				else{
					EventFillTree?.Invoke(this,false);
					SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				}
			}
			else if(nodeTypeAndKey.NodeType==EnumImageNodeType.Mount){
				Mount mountOriginal=Mounts.GetByNum(nodeTypeAndKey.PriKey);
				Patient patientOriginal=Patients.GetPat(mountOriginal.PatNum);
				Mount mount=mountOriginal.Copy();
				mount.PatNum=PatientCur.PatNum;
				mount.DocCategory=GetCurrentCategory();
				Mounts.Insert(mount);
				List<MountItem> listMountItemsOrig=MountItems.GetItemsForMount(mountOriginal.MountNum);
				for(int i=0;i<listMountItemsOrig.Count;i++){
					MountItem mountItem=listMountItemsOrig[i].Copy();
					mountItem.MountNum=mount.MountNum;
					MountItems.Insert(mountItem);
					Document documentOriginal=Documents.GetDocumentForMountItem(listMountItemsOrig[i].MountItemNum);
					if(documentOriginal is null){
						continue;//some mount items may be empty
					}
					string sourceFile=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(patientOriginal,ImageStore.GetPreferredAtoZpath()),documentOriginal.FileName);
					//we could be pasting into the same patient or a different patient.
					document=ImageStore.Import(sourceFile,GetCurrentCategory(),PatientCur);
					document.CropH=documentOriginal.CropH;
					document.CropW=documentOriginal.CropW;
					document.CropX=documentOriginal.CropX;
					document.CropY=documentOriginal.CropY;
					document.DegreesRotated=documentOriginal.DegreesRotated;
					document.Description=documentOriginal.Description;
					document.IsFlipped=documentOriginal.IsFlipped;
					document.WindowingMin=documentOriginal.WindowingMin;
					document.WindowingMax=documentOriginal.WindowingMax;
					document.MountItemNum=mountItem.MountItemNum;
					Documents.Update(document);
				}
				EventFillTree?.Invoke(this,false);
				EventSelectTreeNode?.Invoke(this,new NodeTypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
			}
		}

		public void ToolBarRotateL_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				if(GetDocumentShowing(0).CropW>0
					&& GetDocumentShowing(0).CropX!=0 && GetDocumentShowing(0).CropY!=0)//but if cropped and centered,then we do allow rotate.
				{
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
				Document documentOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).DegreesRotated-=90;
				while(GetDocumentShowing(0).DegreesRotated<0) {
					GetDocumentShowing(0).DegreesRotated+=360;
				}
				Documents.Update(GetDocumentShowing(0),documentOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with category "
					+defDocCategory.ItemName+" rotated left 90 degrees";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}
				if(_documentArrayShowing[_idxSelectedInMount].CropW>0
					&& _documentArrayShowing[_idxSelectedInMount].CropX!=0 && _documentArrayShowing[_idxSelectedInMount].CropY!=0)
				{
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
				Document documentOld=_documentArrayShowing[_idxSelectedInMount].Copy();
				if(_documentArrayShowing[_idxSelectedInMount].CropW != 0){
					//x and y are zero, and we will leave them alone
					//just swap the width and height
					int w=_documentArrayShowing[_idxSelectedInMount].CropH;
					_documentArrayShowing[_idxSelectedInMount].CropH=_documentArrayShowing[_idxSelectedInMount].CropW;
					_documentArrayShowing[_idxSelectedInMount].CropW=w;
				}
				_documentArrayShowing[_idxSelectedInMount].DegreesRotated-=90;
				while(_documentArrayShowing[_idxSelectedInMount].DegreesRotated<0) {
					_documentArrayShowing[_idxSelectedInMount].DegreesRotated+=360;
				}
				Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
				ImageStore.DeleteThumbnailImage(_documentArrayShowing[_idxSelectedInMount],PatFolder);
				//probably not necessary with existing crop because scale hasn't changed.
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" rotated left 90 degrees";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			ThumbnailRefresh();
			panelMain.Invalidate();
		}

		public void ToolBarRotateR_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}
				if(GetDocumentShowing(0).CropW>0
					&& GetDocumentShowing(0).CropX!=0 && GetDocumentShowing(0).CropY!=0)//but if cropped and centered,then we do allow rotate.
				{
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
				Document documentOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).DegreesRotated+=90;
				GetDocumentShowing(0).DegreesRotated%=360;
				Documents.Update(GetDocumentShowing(0),documentOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with category "
					+defDocCategory.ItemName+" rotated right 90 degrees";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				if(_documentArrayShowing[_idxSelectedInMount].CropW>0
					&& _documentArrayShowing[_idxSelectedInMount].CropX!=0 && _documentArrayShowing[_idxSelectedInMount].CropY!=0)
				{
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
				Document documentOld=_documentArrayShowing[_idxSelectedInMount].Copy();
				if(_documentArrayShowing[_idxSelectedInMount].CropW != 0){
					//x and y are zero, and we will leave them alone
					//just swap the width and height
					int w=_documentArrayShowing[_idxSelectedInMount].CropH;
					_documentArrayShowing[_idxSelectedInMount].CropH=_documentArrayShowing[_idxSelectedInMount].CropW;
					_documentArrayShowing[_idxSelectedInMount].CropW=w;
				}
				_documentArrayShowing[_idxSelectedInMount].DegreesRotated+=90;
				_documentArrayShowing[_idxSelectedInMount].DegreesRotated%=360;
				Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
				ImageStore.DeleteThumbnailImage(_documentArrayShowing[_idxSelectedInMount],PatFolder);
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" rotated right 90 degrees";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			ThumbnailRefresh();
			panelMain.Invalidate();
		}

		public void ToolBarRotate180_Click(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}
				if(GetDocumentShowing(0).CropW>0
					&& GetDocumentShowing(0).CropX!=0 && GetDocumentShowing(0).CropY!=0)//but if cropped and centered,then we do allow rotate.
				{
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
				Document documentOld=GetDocumentShowing(0).Copy();
				if(GetDocumentShowing(0).DegreesRotated>=180){
					GetDocumentShowing(0).DegreesRotated-=180;
				}
				else{
					GetDocumentShowing(0).DegreesRotated+=180;
				}
				Documents.Update(GetDocumentShowing(0),documentOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with category "
					+defDocCategory.ItemName+" rotated 180 degrees";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				if(_documentArrayShowing[_idxSelectedInMount].CropW>0
					&& _documentArrayShowing[_idxSelectedInMount].CropX!=0 && _documentArrayShowing[_idxSelectedInMount].CropY!=0)
				{
					MsgBox.Show(this,"Remove crop before attempting to change rotation.");
					return;
				}
				Document documentOld=_documentArrayShowing[_idxSelectedInMount].Copy();
				if(_documentArrayShowing[_idxSelectedInMount].DegreesRotated>=180){
					_documentArrayShowing[_idxSelectedInMount].DegreesRotated-=180;
				}
				else{
					_documentArrayShowing[_idxSelectedInMount].DegreesRotated+=180;
				}
				Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
				ImageStore.DeleteThumbnailImage(_documentArrayShowing[_idxSelectedInMount],PatFolder);
				//LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);//no change to scale
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" rotated 180 degrees";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
			}
			ThumbnailRefresh();
			panelMain.Invalidate();
		}

		public void ToolBarSize_Click(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[0].DateCreated)) {
					return;
				}	
				if(GetDocumentShowing(0)==null){
					return;
				}
				using FormDocumentSize formDocumentSize=new FormDocumentSize();
				formDocumentSize.DocumentCur=GetDocumentShowing(0);
				if(_bitmapDicomRaw!=null) {
					formDocumentSize.SizeRaw=new Size(_bitmapDicomRaw.Width,_bitmapDicomRaw.Height);
				}
				else {
					formDocumentSize.SizeRaw=_bitmapRaw.Size;
				}
				formDocumentSize.ShowDialog();
				////the form will change DocumentCur and save it
				if(formDocumentSize.DialogResult==DialogResult.OK) {
					EventFillTree?.Invoke(this,true);
					EventSelectTreeNode?.Invoke(this,null);
					Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[0].DocCategory);
					string logText="Document Edited: "+_documentArrayShowing[0].FileName+" with category "
						+defDocCategory.ItemName+" was adjusted using the Size button";
					SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
				}
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				if(_documentArrayShowing[_idxSelectedInMount]==null){
					return;
				}
				using FormDocumentSizeMount formDocumentSizeMount=new FormDocumentSizeMount();
				formDocumentSizeMount.DocumentCur=_documentArrayShowing[_idxSelectedInMount];
				formDocumentSizeMount.SizeRaw=_bitmapArrayShowing[_idxSelectedInMount].Size;
				formDocumentSizeMount.SizeMount=new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
				formDocumentSizeMount.ShowDialog();
				//the form will change DocumentCur and save it
				if(formDocumentSizeMount.DialogResult==DialogResult.OK){
					LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
					Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
					string logText="Document Edited: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
						+_mountShowing.Description+" with category "+defDocCategory.ItemName+" was adjusted using the Size button";
					SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
				}
			}
			ThumbnailRefresh();
			panelMain.Invalidate();
			//ignore mount without selected item because the button shouldn't be enabled
		}

		public void ToolBarZoomOne_Click(){
			if(_idxSelectedInMount==-1){
				return;
			}
			_pointTranslation=new Point(
				_mountShowing.Width/2-_listMountItems[_idxSelectedInMount].Xpos-_listMountItems[_idxSelectedInMount].Width/2,
				_mountShowing.Height/2-_listMountItems[_idxSelectedInMount].Ypos-_listMountItems[_idxSelectedInMount].Height/2);
			int oldZoom=ZoomSliderValue;
			float newZoom=ImageTools.CalcScaleFit(new Size(panelMain.Width,panelMain.Height),
				new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),0);
			int intZoom=(int)(newZoom*95);//convert from fraction to percent and fit in bounds with some padding
			EventZoomSliderSetValueAndMax?.Invoke(this,intZoom);
			ZoomSliderValue=intZoom;
			panelMain.Invalidate();
		}

		public void ZoomSliderFitPressed(){
			_pointTranslation=new Point(0,0);
			panelMain.Invalidate();
		}
		#endregion Methods - Public

		#region Methods - Event Handlers
		private void FormImageFloat_FormClosed(object sender, FormClosedEventArgs e){
			if(_formImageFloatWindows!=null && _formImageFloatWindows.Visible){
				_formImageFloatWindows.Close();
			}
		}

		private void FormImageFloat_KeyDown(object sender,KeyEventArgs e) {
			EventKeyDown?.Invoke(this,e);
			if(e.KeyCode==Keys.Escape){
				if(_isLineStarted){
					_listImageDraws.RemoveAt(_listImageDraws.Count-1);
					_isLineStarted=false;
					_listPointFsDrawing.Clear();
					panelMain.Invalidate();
					_isMouseDownPanel=false;//no necessary
				}
			}
			if(e.KeyCode==Keys.ShiftKey){
				//this is also triggered from Parent_KeyDown in case this window does not have focus
				Point pointInPanel=panelMain.PointToClient(Control.MousePosition);
				MouseEventArgs mouseEventArgs=new MouseEventArgs(MouseButtons.Left,1,pointInPanel.X,pointInPanel.Y,0);
				panelMain_MouseMove(this,mouseEventArgs);//to trigger the minus sign
			}
		}

		private void FormImageFloat_KeyUp(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.ShiftKey){
				Point pointInPanel=panelMain.PointToClient(Control.MousePosition);
				MouseEventArgs mouseEventArgs=new MouseEventArgs(MouseButtons.Left,1,pointInPanel.X,pointInPanel.Y,0);
				panelMain_MouseMove(this,mouseEventArgs);//to trigger the minus sign to go away
			}
		}

		private void FormImageFloat_Load(object sender, EventArgs e){
			PanelBorders.Paint += PanelBorders_Paint;
			PanelBorders.MouseDown += PanelBorders_MouseDown;
			PanelBorders.MouseLeave += PanelBorders_MouseLeave;
			PanelBorders.MouseMove += PanelBorders_MouseMove;
			PanelBorders.MouseUp += PanelBorders_MouseUp;
		}

		private void PanelBorders_MouseLeave(object sender, EventArgs e){
			_isHotButWindow=false;
			PanelBorders.Invalidate();
		}

		private void PanelBorders_MouseDown(object sender, MouseEventArgs e){
			//this fires after FormODBase.MouseDown.
			if(!_rectangleButWindows.Contains(e.Location)){
				return;
			}
			//But this also causes FormODBase mouseUp to not register. User then clicks title to hide the menu, and that's when FormODBase MouseMove fires. 
			_pointMouseScreenPrevious=new Point(0,0);//Gets around the above problem.
			if(_formImageFloatWindows==null || _formImageFloatWindows.IsDisposed){
				_formImageFloatWindows=new FormImageFloatWindows();
				_formImageFloatWindows.LayoutManager=LayoutManager;
				//_formImageFloatWindows.Font=Font;//It will figure out its own font
				_formImageFloatWindows.EventWindowClicked+=_formImageFloatWindows_EventWindowClicked;
				_formImageFloatWindows.EventWindowCloseOthers += _formImageFloatWindows_EventWindowCloseOthers;
				_formImageFloatWindows.EventWindowDockThis += _formImageFloatWindows_EventWindowDockThis;
				_formImageFloatWindows.EventWindowShowAll += _formImageFloatWindows_EventWindowShowAll;
				_formImageFloatWindows.FormClosed+=_formImageFloatWindows_FormClosed;
			}
			_formImageFloatWindows.BringToFront();
			_formImageFloatWindows.PointAnchor1=new Point(Location.X+_rectangleButWindows.Left,Location.Y+_rectangleButWindows.Bottom-LayoutManager.Scale(9));
			_formImageFloatWindows.PointAnchor2=new Point(Location.X+_rectangleButWindows.Right,Location.Y+_rectangleButWindows.Bottom-LayoutManager.Scale(9));
			_formImageFloatWindows.Show(this);//not a dialog.  They can click elsewhere
			_isButWindowPressed=true;
			PanelBorders.Invalidate();
		}

		private void _formImageFloatWindows_EventWindowClicked(object sender, int e){
			EventWindowClicked?.Invoke(this,e);//bubble on up
		}

		private void _formImageFloatWindows_EventWindowCloseOthers(object sender, EventArgs e){
			EventWindowCloseOthers?.Invoke(this,new EventArgs());//bubble on up
		}

		private void _formImageFloatWindows_EventWindowDockThis(object sender, EventArgs e){
			EventWindowDockThis?.Invoke(this,new EventArgs());//bubble on up
		}

		private void _formImageFloatWindows_EventWindowShowAll(object sender, EventArgs e){
			EventWindowShowAll?.Invoke(this,new EventArgs());//bubble on up
		}

		private void _formImageFloatWindows_FormClosed(object sender,FormClosedEventArgs e) {
			_isButWindowPressed=false;
			PanelBorders.Invalidate();
		}

		protected override void OnResizeEnd(EventArgs e) {
			base.OnResizeEnd(e);
			SetZoomSlider();
			//this fires on the mouse up after moving a form or resizing it.
			//It does not fire while resizing, so that's nice.
			//The move could have been to a window of a different dpi.
		}

		private void LayoutManager_ZoomChanged(object sender,EventArgs e) {
			//User just dragged window to the other screen, so we will change the zoom so that it looks the "same" as before.
			/*
			//This math was unnecessarily complex.  Handled now by simple OnResizeEnd.
			float scaleNew=LayoutManager.ScaleMy();
			float zoomOld=ZoomSliderValue;
			float zoomNew=zoomOld/_scalePrevious*scaleNew;
			Size sizeImage;
			int degreesRotated=0;
			if(IsDocumentShowing()){
				if(_bitmapRaw is null){//pdf, for example
					_scalePrevious=LayoutManager.ScaleMy();
					return;
				}
				sizeImage=_bitmapRaw.Size;
				degreesRotated=GetDocumentShowing(0).DegreesRotated;
			}
			else if(IsMountShowing()){
				sizeImage=new Size(_mountShowing.Width,_mountShowing.Height);
			}
			else{
				_scalePrevious=LayoutManager.ScaleMy();
				return;
			}
			//Panel main has not yet changed size, so we will calculate the new size
			Size sizePanelMain=new Size((int)(panelMain.Width/zoomOld*zoomNew),(int)(panelMain.Height/zoomOld*zoomNew));
			ZoomSliderStateInitial=new ZoomSliderState(sizePanelMain,sizeImage,degreesRotated);
			EventSetZoomSlider?.Invoke(this,ZoomSliderStateInitial);
			int intZoom=(int)(zoomNew);//convert from fraction to percent
			EventZoomSliderSetValueAndMax?.Invoke(this,intZoom);
			ZoomSliderValue=intZoom;
			panelMain.Invalidate();
			_scalePrevious=LayoutManager.ScaleMy();//for the next time*/
		}

		private void PanelBorders_MouseMove(object sender, MouseEventArgs e){
			if(_rectangleButWindows.Contains(e.Location)){
				_isHotButWindow=true;//simpler algorithm than the other hot flags
			}
			else{
				_isHotButWindow=false;
			}
			PanelBorders.Invalidate();
		}

		private void PanelBorders_MouseUp(object sender, MouseEventArgs e){
		
		}

		private void menuPanelMain_Click(object sender,System.EventArgs e) {
			if(GetSelectedType()==EnumImageNodeType.None){
				return;//Probably the user has no patient or tree object selected
			}
			switch(((MenuItem)sender).Index) {
				case 0:
					ToolBarCopy_Click();
					break;
				case 1://paste
					ToolBarPaste_Click();
					break;
				case 2:
					ToolBarFlip_Click();
					break;
				case 3:
					ToolBarRotateL_Click();
					break;
				case 4:
					ToolBarRotateR_Click();
					break;
				case 5:
					ToolBarRotate180_Click();
					break;
				case 6://info
					ToolBarDelete_Click();
					break;
				case 7://info
					ToolBarInfo_Click();
					break;
			}
		}

		private void PanelBorders_Paint(object sender, PaintEventArgs e){
			//This paint happens right after the base paint for this panel.
			Graphics g=e.Graphics;
			string strWindows=Lan.g(this,"Windows");
			int widthStr=(int)LayoutManager.ScaleMS(g.MeasureString(strWindows,Font).Width);
			_rectangleButWindows=new Rectangle(
				x:_rectangleButMin.Left-widthStr-LayoutManager.Scale(21),
				y:MaxInset()+LayoutManager.WidthBorder(),
				width:widthStr+LayoutManager.Scale(13),
				height:LayoutManager.GetHeightTitleBar());
			//these colors are copied from FormODBase.PanelBorders_Paint
			Color colorFloatBase=Color.FromArgb(65, 94, 154);//This is the default dark blue-gray, same as grid titles
			Color colorBorder=ColorOD.Mix(colorFloatBase,Color.White,1,3);
			Color colorBorderText=Color.Black;
			Color colorButtonHot=ColorOD.Mix(colorBorder,colorBorderText,10,1);
			if(IsImageFloatSelected){
				colorBorder=ColorOD.Mix(colorFloatBase,Color.White,3,1);
				colorBorderText=Color.White;
				colorButtonHot=ColorOD.Mix(colorBorder,colorBorderText,4,1);					
			}			
			if(_isHotButWindow){
				using SolidBrush solidBrushHover=new SolidBrush(colorButtonHot);
				g.FillRectangle(solidBrushHover,_rectangleButWindows);
			}
			if(_isButWindowPressed){
				g.FillRectangle(Brushes.White,_rectangleButWindows);
				g.DrawRectangle(Pens.Gray,_rectangleButWindows);//the bottom will get cut off
				colorBorderText=Color.Black;
			}
			using SolidBrush solidBrushText=new SolidBrush(colorBorderText);
			g.DrawString("Windows",Font,solidBrushText,new Point(_rectangleButWindows.X+LayoutManager.Scale(6),_rectangleButWindows.Y+LayoutManager.Scale(3)));
		}

		private void panelMain_DragEnter(object sender,DragEventArgs e) {
			//WARNING: this will not fire if debugging while VS is in Admin mode
			if(IsMountShowing()){
				if(e.Data.GetDataPresent(DataFormats.FileDrop)){
					e.Effect=DragDropEffects.Copy;
				}
			}
			//ignore any other data
			//maybe pay attention to images later?  Although I don't know how you would drag an image in.
		}

		private void panelMain_DragDrop(object sender,DragEventArgs e) {
			if(!IsMountShowing()){
				return;
			}
			if(!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}
			string[] stringArrayFiles=(string[])e.Data.GetData(DataFormats.FileDrop);
			if(stringArrayFiles.Length==0){
				return;
			}
			Point pointOver=panelMain.PointToClient(new Point(e.X,e.Y));
			_idxSelectedInMount=HitTestInMount(pointOver.X,pointOver.Y);
			List<MountItem> listMountItemsAvail=GetAvailSlots(stringArrayFiles.Length);
			if(listMountItemsAvail==null){
				MsgBox.Show(this,"Not enough slots in the mount for the number of files selected.");
				return;
			}
			for(int i=0;i<stringArrayFiles.Length;i++) {
				if(!ImageStore.HasImageExtension(stringArrayFiles[i])){
					MsgBox.Show(stringArrayFiles[i]+" "+Lan.g(this,"cannot be imported because it's not an image."));
					return;
				}
			}
			Document document=null;
			for(int i=0;i<stringArrayFiles.Length;i++) {
				try {
					//stringArrayFiles contains full paths
					if(CloudStorage.IsCloudStorage) {
						//this will flicker because multiple progress bars.  Improve later.
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => document=ImageStore.Import(stringArrayFiles[i],GetCurrentCategory(),PatientCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						document=ImageStore.Import(stringArrayFiles[i],GetCurrentCategory(),PatientCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+stringArrayFiles[i]);
					continue;
				}
				Document documentOld=document.Copy();
				document.MountItemNum=listMountItemsAvail[i].MountItemNum;
				document.ToothNumbers=listMountItemsAvail[i].ToothNumbers;
				Documents.Update(document,documentOld);
			}//for
			EventSelectTreeNode?.Invoke(this,null);
		}

		private void panelMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(_drawMode==EnumDrawMode.Text){
				if(_idxTextMouseDown==-1){
					return;
				}
				//double click inside existing text
				using FormImageDrawEdit formImageDrawEdit=new FormImageDrawEdit();
				formImageDrawEdit.ImageDrawCur=_listImageDraws[_idxTextMouseDown];
				if(IsMountShowing()){
					formImageDrawEdit.ColorBack=GetMountShowing().ColorBack;
				}
				else{
					formImageDrawEdit.ColorBack=Color.White;
				}
				formImageDrawEdit.ZoomVal=ZoomSliderValue;
				if(IsBitmapShowing()){
					formImageDrawEdit.SizeBitmap=GetBitmapShowing(0).Size;
				}
				else if(IsMountShowing()){
					formImageDrawEdit.SizeBitmap=new Size(_mountShowing.Width,_mountShowing.Height);
				}
				//else should not happen 
				formImageDrawEdit.ShowDialog();
				if(formImageDrawEdit.DialogResult==DialogResult.OK){
					if(IsBitmapShowing()){
						_listImageDraws=ImageDraws.RefreshForDoc(GetDocumentShowing(0).DocNum);
					}
					if(IsMountShowing()){
						_listImageDraws=ImageDraws.RefreshForMount(GetMountShowing().MountNum);
					}
					//if(formImageDrawEdit.WasDeleted){//could work, but not standard pattern
					//	_listImageDraws.RemoveAt(_idxTextSelected);
					//}
				}
				_idxTextMouseDown=-1;
				panelMain.Invalidate();
				return;
			}
			//From here down is ordinary double click on an empty spot to bring up the Info window.
			if(IsMountShowing()){
				if(_idxSelectedInMount==-1){
					int idx=HitTestInMount(e.X,e.Y,includeText:true);
					if(idx>-1){//double clicked on text
						using FormMountItemEdit formMountItemEdit=new FormMountItemEdit();
						formMountItemEdit.MountItemCur=_listMountItems[idx];
						formMountItemEdit.ShowDialog();
						if(formMountItemEdit.DialogResult==DialogResult.OK){
							panelMain.Invalidate();
						}
					}
					else{
						using FormMountEdit formMountEdit=new FormMountEdit();
						formMountEdit.MountCur=_mountShowing;
						formMountEdit.ShowDialog();
						//Always reload because layout could have changed
						EventFillTree?.Invoke(this,true);
						EventSelectTreeNode?.Invoke(this,null);
					}
				}
				else{//mount item
					if(_documentArrayShowing[_idxSelectedInMount]!=null){
						using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,_documentArrayShowing[_idxSelectedInMount]);
						formDocInfo.IsMountItem=true;
						formDocInfo.ShowDialog();
						//nothing to refresh
					}
				}
			}
			else{
				if(GetDocumentShowing(0)!=null) {
					using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,GetDocumentShowing(0));
					formDocInfo.ShowDialog();
					if(formDocInfo.DialogResult==DialogResult.OK){
						EventFillTree?.Invoke(this,true);
					}
				}
			}
		}

		private void panelMain_MouseDown(object sender, MouseEventArgs e){
			//int idxT=HitTestText(e.Location);
			//PointF pointF=ControlPointToBitmapPointF(e.Location);
			//MsgBox.Show(idxT.ToString());
			//return;
			if(IsDocumentShowing() && _cropPanAdj==EnumCropPanAdj.Crop) {
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentArrayShowing[0].DateCreated)) {
					return;
				}
			}
			for(int i=0;i<menuPanelMain.MenuItems.Count;i++) {
				//If no document or mount selected, context menu will not be visible
				if(IsDocumentShowing() || IsMountShowing()) {
					//Disable these context menu items when opening this from the chart module
					if(DidLaunchFromChartModule && ListTools.In(menuPanelMain.MenuItems[i],menuItemDelete,menuItemPaste)) {
						menuPanelMain.MenuItems[i].Visible=false;
					}
					else {
						menuPanelMain.MenuItems[i].Visible=true;
					}
				}
				else{ 
					menuPanelMain.MenuItems[i].Visible=false;//if no menu items visible, menu won't even show
				}
			}
			if(!IsDocumentShowing() && !IsMountShowing()) {
				return;
			}
			_pointMouseDown=e.Location;  
			_isMouseDownPanel=true;
			_dateTimeMouseDownPanel=DateTime.Now;
			_pointTranslationOld=_pointTranslation;
			if(_drawMode==EnumDrawMode.Text){
				Point pointCursorInBitmap=ControlPointToBitmapPoint(e.Location);
				int idxText=HitTestText(e.Location);
				if(idxText!=-1){
					//mouse down inside existing text
					_pointTextOriginal=_listImageDraws[idxText].GetTextPoint();
					_idxTextMouseDown=idxText;
					//panelMain.Invalidate();
					//mouse down on an existing text item
					//We've already set our variables. Nothing left to do here.
					return;
				}
				//mouse down on an empty spot
				if(!IsMountShowing() && !IsBitmapShowing()){
					//Maybe this is a pdf or something.
					return;
				}
				//The dialog needs to come up now, but that will happen before the mouse up.
				//Here's how we handle that:
				_isMouseDownPanel=false;
				using FormImageDrawEdit formImageDrawEdit=new FormImageDrawEdit();
				formImageDrawEdit.ImageDrawCur=new ImageDraw();
				//Point pointLoc=new Point();
				formImageDrawEdit.ImageDrawCur.SetLocAndText(pointCursorInBitmap,"");
				float fontSizeApparent=8;
				formImageDrawEdit.ImageDrawCur.FontSize=fontSizeApparent/(float)ZoomSliderValue*100f*LayoutManager.ScaleMyFont();
				formImageDrawEdit.ZoomVal=ZoomSliderValue;
				if(IsMountShowing()){
					formImageDrawEdit.ColorBack=GetMountShowing().ColorBack;
				}
				else{
					formImageDrawEdit.ColorBack=Color.White;
				}
				formImageDrawEdit.ImageDrawCur.ColorDraw=ColorFore;
				formImageDrawEdit.ImageDrawCur.ColorBack=ColorTextBack;//can be transparent
				formImageDrawEdit.ImageDrawCur.IsNew=true;
				if(IsBitmapShowing()){
					formImageDrawEdit.ImageDrawCur.DocNum=GetDocumentShowing(0).DocNum;
					formImageDrawEdit.SizeBitmap=GetBitmapShowing(0).Size;
				}
				if(IsMountShowing()){
					formImageDrawEdit.ImageDrawCur.MountNum=_mountShowing.MountNum;
					formImageDrawEdit.SizeBitmap=new Size(_mountShowing.Width,_mountShowing.Height);
				}
				formImageDrawEdit.ShowDialog();
				if(formImageDrawEdit.DialogResult==DialogResult.OK){
					if(IsBitmapShowing()){
						_listImageDraws=ImageDraws.RefreshForDoc(GetDocumentShowing(0).DocNum);
					}
					if(IsMountShowing()){
						_listImageDraws=ImageDraws.RefreshForMount(GetMountShowing().MountNum);
					}
					//formImageDrawEdit.ImageDrawCur.IsNew=false;
					//_listImageDraws.Add(formImageDrawEdit.ImageDrawCur);//this could work, but not standard pattern
					panelMain.Invalidate();
				}
				return;
			}
			if(_drawMode==EnumDrawMode.Pen){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				_listPointFsDrawing.Add(pointFPenInBitmap);
				ImageDraw imageDraw=new ImageDraw();
				imageDraw.DrawType=ImageDrawType.Pen;
				if(IsBitmapShowing()){
					imageDraw.DocNum=GetDocumentShowing(0).DocNum;
				}
				if(IsMountShowing()){
					imageDraw.MountNum=GetMountShowing().MountNum;
				}
				imageDraw.ColorDraw=ColorFore;
				imageDraw.SetDrawingSegment(_listPointFsDrawing);
				//Gets inserted on MouseUp
				_listImageDraws.Add(imageDraw);
				return;
			}
			if(_drawMode==EnumDrawMode.Line){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				if(_isLineStarted){//if this is the second click to finish a line that was already started.
					if(_idxImageDraw==-1) {
						return;
					}
					if(_isLineExtending){
						ImageDraws.Update(_listImageDraws[_idxImageDraw]);
					}
					else{
						ImageDraws.Insert(_listImageDraws[_idxImageDraw]);
					}
					_isLineStarted=false;
					_listPointFsDrawing.Clear();
					_idxImageDraw=-1;
					_isLineExtending=false;
					panelMain.Invalidate();
					return;
				}
				//First click (or drag) from here down.
				//Determine whether they are adding to an existing line.
				float testRadius=3f/(float)ZoomSliderValue*100f;//so the test radius appears the same to the user, regardless of zoom
				for(int i=0;i<_listImageDraws.Count;i++){
					if(_listImageDraws[i].DrawType!=ImageDrawType.Line){
						continue;
					}
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					//Only test the last point. Not allowed to extend from any other point.
					float dist=(float)Math.Sqrt(Math.Pow(pointFPenInBitmap.X-listPointFs[listPointFs.Count-1].X,2)+Math.Pow(pointFPenInBitmap.Y-listPointFs[listPointFs.Count-1].Y,2));
					if(dist>testRadius){
						continue;
					}
					_isLineExtending=true;
					_idxImageDraw=i;
					_listPointFsDrawing=listPointFs;
					_listPointFsDrawing.Add(pointFPenInBitmap);//This is the new point that will be manipulated as we move.
					//imageDraw.SetDrawingSegment(_listPointFsDrawing);//not needed.
					_isLineStarted=true;
					return;
				}
				_isLineExtending=false;
				_listPointFsDrawing.Add(pointFPenInBitmap);
				//add the same point a second time.  This second one is the one that will be manipulated as we move.
				_listPointFsDrawing.Add(pointFPenInBitmap);
				ImageDraw imageDraw=new ImageDraw();
				imageDraw.DrawType=ImageDrawType.Line;
				if(IsBitmapShowing()){
					imageDraw.DocNum=GetDocumentShowing(0).DocNum;
				}
				if(IsMountShowing()){
					imageDraw.MountNum=GetMountShowing().MountNum;
				}
				imageDraw.ColorDraw=ColorFore;
				imageDraw.SetDrawingSegment(_listPointFsDrawing);
				//Gets inserted on second click
				_listImageDraws.Add(imageDraw);
				_idxImageDraw=_listImageDraws.Count-1;
				_isLineStarted=true;
				return;
			}
			if(_drawMode==EnumDrawMode.LineEditPoints){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				float testRadius=3f/(float)ZoomSliderValue*100f;//so the test radius appears the same to the user, regardless of zoom
				_idxImageDraw=-1;
				_idxPoint=-1;
				for(int i=0;i<_listImageDraws.Count;i++){
					if(_listImageDraws[i].DrawType!=ImageDrawType.Line){
						continue;
					}
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					//test individual points
					for(int p=0;p<listPointFs.Count;p++) {
						float dist=(float)Math.Sqrt(Math.Pow(pointFPenInBitmap.X-listPointFs[p].X,2)+Math.Pow(pointFPenInBitmap.Y-listPointFs[p].Y,2));
						if(dist>testRadius){
							continue;
						}
						if(IsShiftDown()){//subtract the point
							listPointFs.RemoveAt(p);
							if(listPointFs.Count==1){//delete the whole line
								ImageDraws.Delete(_listImageDraws[i].ImageDrawNum);
								_listImageDraws.RemoveAt(i);
								panelMain.Cursor=_cursorLinePoint;
								panelMain.Invalidate();
								return;
							}
							_listImageDraws[i].SetDrawingSegment(listPointFs);
							ImageDraws.Update(_listImageDraws[i]);
							panelMain.Cursor=_cursorLinePoint;
							panelMain.Invalidate();
						}
						else{//start dragging the point
							_idxImageDraw=i;
							_idxPoint=p;
						}
						return;
					}
					//No end points were hit, so test line segments
					if(IsShiftDown()){
						continue;//they can only delete points, not add points
					}
					for(int p=1;p<listPointFs.Count;p++) {
						if(!HitTestLine(listPointFs[p-1],listPointFs[p],pointFPenInBitmap,testRadius)){
							continue;
						}
						//add a point
						listPointFs.Insert(p,pointFPenInBitmap);
						_listImageDraws[i].SetDrawingSegment(listPointFs);
						_idxImageDraw=i;
						_idxPoint=p;	
						panelMain.Cursor=_cursorLineMove;
						panelMain.Invalidate();
						return;					
					}
				}
				return;
			}
			if(_drawMode==EnumDrawMode.LineMeasure){
				if(_stringMeasure.IsNullOrEmpty()){
					return;
				}
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				using FormImageDrawEdit formImageDrawEdit=new FormImageDrawEdit();
				formImageDrawEdit.ImageDrawCur=new ImageDraw();
				formImageDrawEdit.ImageDrawCur.SetLocAndText(Point.Round(pointFPenInBitmap),_stringMeasure);
				float fontSizeApparent=9;
				formImageDrawEdit.ImageDrawCur.FontSize=fontSizeApparent/(float)ZoomSliderValue*100f*LayoutManager.ScaleMyFont();
				formImageDrawEdit.ZoomVal=ZoomSliderValue;
				if(IsMountShowing()){
					formImageDrawEdit.ColorBack=GetMountShowing().ColorBack;
				}
				else{
					formImageDrawEdit.ColorBack=Color.White;
				}
				formImageDrawEdit.ImageDrawCur.ColorDraw=ColorFore;
				formImageDrawEdit.ImageDrawCur.ColorBack=ColorTextBack;//can be transparent
				formImageDrawEdit.ImageDrawCur.IsNew=true;
				if(IsBitmapShowing()){
					formImageDrawEdit.ImageDrawCur.DocNum=GetDocumentShowing(0).DocNum;
					formImageDrawEdit.SizeBitmap=GetBitmapShowing(0).Size;
				}
				if(IsMountShowing()){
					formImageDrawEdit.ImageDrawCur.MountNum=_mountShowing.MountNum;
					formImageDrawEdit.SizeBitmap=new Size(_mountShowing.Width,_mountShowing.Height);
				}
				formImageDrawEdit.ShowDialog();
				if(formImageDrawEdit.DialogResult!=DialogResult.OK){
					return;
				}
				if(IsBitmapShowing()){
					_listImageDraws=ImageDraws.RefreshForDoc(GetDocumentShowing(0).DocNum);
				}
				if(IsMountShowing()){
					_listImageDraws=ImageDraws.RefreshForMount(GetMountShowing().MountNum);
				}
				//_listImageDraws.Add(formImageDrawEdit.ImageDrawCur);//this could work, but not standard pattern
				_stringMeasure="";
				_isMouseDownPanel=false;
				panelMain.Invalidate();
				EventSetDrawMode?.Invoke(this,EnumDrawMode.Text);
				return;
			}
			if(_drawMode==EnumDrawMode.LineSetScale){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				float testRadius=3f/(float)ZoomSliderValue*100f;//so the test radius appears the same to the user, regardless of zoom
				for(int i=0;i<_listImageDraws.Count;i++){
					if(_listImageDraws[i].DrawType!=ImageDrawType.Line){
						continue;
					}
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					for(int p=1;p<listPointFs.Count;p++) {
						if(!HitTestLine(listPointFs[p-1],listPointFs[p],pointFPenInBitmap,testRadius)){
							continue;
						}
						using FormImageScale formImageScale=new FormImageScale();
						formImageScale.Pixels=CalcLengthLine(listPointFs);
						formImageScale.ShowDialog();
						if(formImageScale.DialogResult!=DialogResult.OK){
							return;
						}
						//see if we have an existing scale to replace
						ImageDraw imageDraw=null;
						for(int j=0;j<_listImageDraws.Count;j++){
							if(_listImageDraws[j].DrawType==ImageDrawType.ScaleValue){
								imageDraw=_listImageDraws[j];
								break;
							}
						}
						if(imageDraw==null){
							imageDraw=new ImageDraw();
							imageDraw.DrawType=ImageDrawType.ScaleValue;
							if(IsBitmapShowing()){
								imageDraw.DocNum=GetDocumentShowing(0).DocNum;
							}
							if(IsMountShowing()){
								imageDraw.MountNum=_mountShowing.MountNum;
							}
							imageDraw.SetScale(formImageScale.ScaleVal,formImageScale.Decimals,formImageScale.StringUnits);
							ImageDraws.Insert(imageDraw);
							_listImageDraws.Add(imageDraw);
						}
						else{//existing scale
							imageDraw.SetScale(formImageScale.ScaleVal,formImageScale.Decimals,formImageScale.StringUnits);
							ImageDraws.Update(imageDraw);
						}							
						_isMouseDownPanel=false;
						return;
					}
				}
				return;
			}
			if(_drawMode==EnumDrawMode.Eraser){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				EraseAt(pointFPenInBitmap);
				return;
			}
			if(_drawMode==EnumDrawMode.ChangeColor){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				ChangeColorAt(pointFPenInBitmap);
				return;
			}
			if(_drawMode==EnumDrawMode.Polygon){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				_listPointFsDrawing.Add(pointFPenInBitmap);
				ImageDraw imageDraw=new ImageDraw();
				imageDraw.DrawType=ImageDrawType.Polygon;
				if(IsBitmapShowing()){
					imageDraw.DocNum=GetDocumentShowing(0).DocNum;
				}
				if(IsMountShowing()){
					imageDraw.MountNum=GetMountShowing().MountNum;
				}
				imageDraw.ColorDraw=ColorFore;
				imageDraw.SetDrawingSegment(_listPointFsDrawing);
				//Gets inserted on MouseUp
				_listImageDraws.Add(imageDraw);
				return;
			}
			if(!IsMountShowing()){
				return;
			}
			//Mount showing from here down================================================================================================================
			if(e.Button==MouseButtons.Right){
				//normally, we select mount item on mouse up, but for right click, we must do it here
				int idxSelectedInMountOld=_idxSelectedInMount;
				_idxSelectedInMount=HitTestInMount(e.X,e.Y);
				if(_idxSelectedInMount!=idxSelectedInMountOld){
					LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.OnlyRaw);
				}
				EnableToolBarButtons();
				_isDraggingMount=false;
				panelMain.Invalidate();
				SetWindowingSlider();
			}
			//If no item in mount selected, mimic toolbar option availability
			if(_idxSelectedInMount==-1 && e.Button==MouseButtons.Right)	{
				//Copy
				menuItemPaste.Visible=false;//Paste
				menuItemFlipHoriz.Visible=false;//Flip Horizontally
				menuItemRotateLeft.Visible=false;//Rotate Left
				menuItemRotateRight.Visible=false;//Rotate Right
				menuItemRotate180.Visible=false;//Rotate 180
				menuItemDelete.Visible=false;//Delete
				//Info
			}
			else {
				//Disable these context menu items when opening this from the chart module
				if(!DidLaunchFromChartModule) {
					menuItemPaste.Visible=true;
					menuItemDelete.Visible=true;
				}
				menuItemFlipHoriz.Visible=true;
				menuItemRotateLeft.Visible=true;
				menuItemRotateRight.Visible=true;
				menuItemRotate180.Visible=true;
			}
			//if(_panCropMount==EnumPanCropMount.Pan/Adj){
				//handled on mouse up
			//}
			if(_cropPanAdj==EnumCropPanAdj.Adj){
				if(!Security.IsAuthorized(EnumPermType.ImageEdit,_mountShowing.DateCreated)) {
					_isMouseDownPanel=false;
					return;
				}
				Point pointRaw=ControlPointToBitmapPoint(e.Location);
				int idxSelectedInMountOld=_idxSelectedInMount;
				_idxSelectedInMount=-1;
				for(int i=0;i<_listMountItems.Count;i++){
					Rectangle rect=new Rectangle(_listMountItems[i].Xpos,_listMountItems[i].Ypos,_listMountItems[i].Width,_listMountItems[i].Height);
					if(rect.Contains(pointRaw)){
						_idxSelectedInMount=i;
					}
				}
				if(_idxSelectedInMount!=idxSelectedInMountOld){
					LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.OnlyRaw);
				}
				EnableToolBarButtons();
				if(_idxSelectedInMount>-1){//individual image
					if(_documentArrayShowing[_idxSelectedInMount]!=null){
						_isDraggingMount=true;
						//float scaleFactor=zoomSlider.Value/100f;
						//To handle rotation, crop, etc, always measure from center of mount position.  Crop is not involved until mouse up.
						_pointDragNow=new Point(_listMountItems[_idxSelectedInMount].Xpos+_listMountItems[_idxSelectedInMount].Width/2,
							_listMountItems[_idxSelectedInMount].Ypos+_listMountItems[_idxSelectedInMount].Height/2);
						_pointDragStart=_pointDragNow;
					}
				}
				panelMain.Invalidate();
			}
		}

		private void panelMain_MouseLeave(object sender,EventArgs e) {
			panelHover.Visible=false;
		}

		private void panelMain_MouseMove(object sender, MouseEventArgs e){
			//panelMain.Focus();//otherwise, the buttons in the parent form might have focus and then the shift key won't fire
			//That didn't work.  It resulted in automatic floater selection, which is bad.
			//New plan: pass the shift event down from the module.
			if(GetSelectedType()==EnumImageNodeType.None){
				return;
			}
			if(GetSelectedType()==EnumImageNodeType.Category){
				return;
			}
			EnumImageNodeType imageNodeType=GetSelectedType();//for debugging
			Point pointCursorInBitmap=ControlPointToBitmapPoint(e.Location);
			if(_drawMode==EnumDrawMode.Text){
				if(_isMouseDownPanel) {//dragging
					if(_idxTextMouseDown==-1){
						return;
					}
					_isDraggingText=true;
					Point pointDownInBitmap=ControlPointToBitmapPoint(_pointMouseDown);
					Point pointNew=new Point(
						x:_pointTextOriginal.X+pointCursorInBitmap.X-pointDownInBitmap.X,
						y:_pointTextOriginal.Y+pointCursorInBitmap.Y-pointDownInBitmap.Y);
					_listImageDraws[_idxTextMouseDown].SetLoc(pointNew);
					//Will be saved to db on mouse up.
					panelMain.Invalidate();
				}
				else{//not dragging. Just change cursor.
					panelMain.Cursor=Cursors.IBeam;
					int idxText=HitTestText(e.Location);
					if(idxText!=-1){
						panelMain.Cursor=Cursors.SizeAll;
					}
				}
				return;
			}
			if(_drawMode==EnumDrawMode.Pen
				|| _drawMode==EnumDrawMode.Polygon)
			{
				if(!_isMouseDownPanel) {
					return;
				}
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				PointF pointFLast=new PointF();
				if(_listPointFsDrawing.Count>0){
					pointFLast=_listPointFsDrawing[_listPointFsDrawing.Count-1];
				}
				float dist=Math.Abs(pointFLast.X-pointFPenInBitmap.X)+Math.Abs(pointFLast.Y-pointFPenInBitmap.Y);
				if(dist<3){
					return;//to minimize db size
				}
				_listPointFsDrawing.Add(pointFPenInBitmap);
				_listImageDraws[_listImageDraws.Count-1].SetDrawingSegment(_listPointFsDrawing);
				panelMain.Invalidate();
				return;
			}
			if(_drawMode==EnumDrawMode.Line){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				if(!_isLineStarted){
					float testRadius=3f/(float)ZoomSliderValue*100f;//so the test radius appears the same to the user, regardless of zoom
					panelMain.Cursor=_cursorCrosshair;
					for(int i=0;i<_listImageDraws.Count;i++){
						if(_listImageDraws[i].DrawType!=ImageDrawType.Line){
							continue;
						}
						List<PointF> listPointFs=_listImageDraws[i].GetPoints();
						//Only test the last point. Not allowed to extend from any other point.
						float dist=(float)Math.Sqrt(Math.Pow(pointFPenInBitmap.X-listPointFs[listPointFs.Count-1].X,2)+Math.Pow(pointFPenInBitmap.Y-listPointFs[listPointFs.Count-1].Y,2));
						if(dist>testRadius){
							continue;
						}
						panelMain.Cursor=_cursorLineAdd;//indicate to user that they can add a point here.
						return;
					}
					return;
				}
				if(_isMouseDownPanel) {
					//They are dragging their line. No code difference in the mouse move.
				}
				//we want to show the potential line, so manipulate the last point.
				_listPointFsDrawing[_listPointFsDrawing.Count-1]=pointFPenInBitmap;
				if(_listImageDraws!=null && _idxImageDraw!=-1 && _listImageDraws.Count>_idxImageDraw) {//ensure big enough list for index
					_listImageDraws[_idxImageDraw].SetDrawingSegment(_listPointFsDrawing); 
				}
				panelMain.Invalidate();
				return;
			}
			if(_drawMode==EnumDrawMode.LineEditPoints){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				if(_isMouseDownPanel) {
					if(_idxImageDraw==-1 || _idxPoint==-1){
						return;
					}
					List<PointF> listPointFs=_listImageDraws[_idxImageDraw].GetPoints();
					listPointFs[_idxPoint]=pointFPenInBitmap;
					_listImageDraws[_idxImageDraw].SetDrawingSegment(listPointFs);
					panelMain.Invalidate();
					return;
				}
				//From here down, we are just changing the cursor as needed
				float testRadius=3f/(float)ZoomSliderValue*100f;//so the test radius appears the same to the user, regardless of zoom
				panelMain.Cursor=_cursorLinePoint;
				for(int i=0;i<_listImageDraws.Count;i++){
					if(_listImageDraws[i].DrawType!=ImageDrawType.Line){
						continue;
					}
					List<PointF> listPointFs2=_listImageDraws[i].GetPoints();
					//First, test end points
					for(int p=0;p<listPointFs2.Count;p++) {
						float dist=(float)Math.Sqrt(Math.Pow(pointFPenInBitmap.X-listPointFs2[p].X,2)+Math.Pow(pointFPenInBitmap.Y-listPointFs2[p].Y,2));
						if(dist<testRadius){
							if(IsShiftDown()){
								panelMain.Cursor=_cursorLineSubtract;
							}
							else{
								panelMain.Cursor=_cursorLineMove;//indicating they can move this point
							}
							return;
						}
					}
					//No end points were hit, so test line segments
					if(IsShiftDown()){
						continue;//they can only delete points, not add points
					}
					for(int p=1;p<listPointFs2.Count;p++) {
						if(!HitTestLine(listPointFs2[p-1],listPointFs2[p],pointFPenInBitmap,testRadius)){
							continue;
						}
						panelMain.Cursor=_cursorLineAdd;//indicating they can add an intermediate point		
						return;					
					}
				}
				return;
			}			
			if(_drawMode==EnumDrawMode.LineMeasure){
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				float testRadius=3f/(float)ZoomSliderValue*100f;//so the test radius appears the same to the user, regardless of zoom
				Point pointShift=new Point((int)(17f/(float)ZoomSliderValue*100f),(int)(4f/(float)ZoomSliderValue*100f));
				string stringMeasure="";
				for(int i=0;i<_listImageDraws.Count;i++){
					if(_listImageDraws[i].DrawType!=ImageDrawType.Line){
						continue;
					}
					if(!stringMeasure.IsNullOrEmpty()){
						break;//we already found one
					}
					List<PointF> listPointFs2=_listImageDraws[i].GetPoints();
					for(int p=1;p<listPointFs2.Count;p++) {
						if(!HitTestLine(listPointFs2[p-1],listPointFs2[p],pointFPenInBitmap,testRadius)){
							continue;
						}
						float length=CalcLengthLine(listPointFs2);
						//Find the scale
						ImageDraw imageDrawScale=null;
						for(int j=0;j<_listImageDraws.Count;j++){
							if(_listImageDraws[j].DrawType==ImageDrawType.ScaleValue){
								imageDrawScale=_listImageDraws[j];
								break;
							}
						}
						//imageDrawScale is guaranteed to have a value because we checked this in ToolBarMeasure_Click().
						float scaleVal=imageDrawScale.GetScale();
						string stringUnits=imageDrawScale.GetScaleUnits();
						int decimalPlaces=imageDrawScale.GetDecimals();
						float lengthScaled=length/scaleVal;
						stringMeasure=lengthScaled.ToString("f"+decimalPlaces.ToString());
						if(!stringUnits.IsNullOrEmpty()){
							stringMeasure+=" "+stringUnits;
						}
						_pointMeasureText=new Point((int)pointFPenInBitmap.X+pointShift.X,(int)pointFPenInBitmap.Y+pointShift.Y);
						break;					
					}
				}
				if(_stringMeasure != stringMeasure){
					_stringMeasure=stringMeasure;
					panelMain.Invalidate();
				}
				return;
			}
			if(_drawMode==EnumDrawMode.LineSetScale){
				return;
			}
			if(_drawMode==EnumDrawMode.Eraser){
				if(!_isMouseDownPanel) {
					return;
				}
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				EraseAt(pointFPenInBitmap);
				return;
			}
			if(_drawMode==EnumDrawMode.ChangeColor){
				if(!_isMouseDownPanel) {
					return;
				}
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);
				ChangeColorAt(pointFPenInBitmap);
				return;
			}
			if(_cropPanAdj==EnumCropPanAdj.Pan && !_isMouseDownPanel){
				//hover effect
				PointF pointFPenInBitmap=ControlPointToBitmapPointF(e.Location);//either bitmap coords or mount coords
				int idxHover=HitTestHover(pointFPenInBitmap);
				if(idxHover==-1){
					panelHover.Visible=false;
				}
				else{
					//panelHover was initialized in the ctor at set visible false
					if(!panelHover.Visible){
						//just the first time that it's made visible.
						panelHover.Visible=true;
						panelHover.BackColor=Color.Pink;
						using Graphics g=panelHover.CreateGraphics();
						using Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(8.25f));
						//StringFormat stringFormat=new StringFormat();
						//stringFormat.
						SizeF sizeF=g.MeasureString(_listImageDraws[idxHover].Details,font,300);//,stringFormat
						Size sizePanel=new Size(LayoutManager.Scale((int)sizeF.Width+20),LayoutManager.Scale((int)sizeF.Height+25));
						LayoutManager.MoveSize(panelHover,sizePanel);
						labelHover.Text=_listImageDraws[idxHover].Details;
					}
					//this needs to be done whether or not it was already visible
					panelHover.Location=new Point(e.X+LayoutManager.Scale(15),e.Y+LayoutManager.Scale(5));
				}
			}
			if(!_isMouseDownPanel) {
				return;
			}
			float scaleTrans=(float)100/ZoomSliderValue;//example, 200%, 100/200=.5, indicating .5 bitmap pixels for each screen pixel
			if(_cropPanAdj==EnumCropPanAdj.Crop) {
				Rectangle rectangle1=new Rectangle(Math.Min(e.Location.X,_pointMouseDown.X), Math.Min(e.Location.Y,_pointMouseDown.Y),
					Math.Abs(e.Location.X-_pointMouseDown.X),	Math.Abs(e.Location.Y-_pointMouseDown.Y));
				Rectangle rectangle2=new Rectangle(0,0,panelMain.Width-1,panelMain.Height-1);
				_rectangleCrop=Rectangle.Intersect(rectangle1,rectangle2);
				panelMain.Invalidate();
			}
			if(_cropPanAdj==EnumCropPanAdj.Pan){
				//scaleTrans=1;
				int xTrans=(int)(_pointTranslationOld.X+(e.Location.X-_pointMouseDown.X)*scaleTrans);
				int yTrans=(int)(_pointTranslationOld.Y+(e.Location.Y-_pointMouseDown.Y)*scaleTrans);
				_pointTranslation=new Point(xTrans,yTrans);
				panelMain.Invalidate();
			}
			if(_cropPanAdj==EnumCropPanAdj.Adj){
				if(_idxSelectedInMount==-1 || _documentArrayShowing[_idxSelectedInMount]==null){
					return;
				}
				int xTrans=(int)(_pointDragStart.X+(ControlPointToBitmapPoint(e.Location).X-ControlPointToBitmapPoint(_pointMouseDown).X));
				int yTrans=(int)(_pointDragStart.Y+(ControlPointToBitmapPoint(e.Location).Y-ControlPointToBitmapPoint(_pointMouseDown).Y));
				_pointDragNow=new Point(xTrans,yTrans);
				//Rectangle rectClip=new Rectangle(0,0,200,200);
				//panelMain.Invalidate(rectClip);
				panelMain.Invalidate();
			}
		}

		private void panelMain_MouseUp(object sender, MouseEventArgs e){
			if(GetSelectedType()==EnumImageNodeType.None){
				return;
			}
			if(GetSelectedType()==EnumImageNodeType.Category){
				return;
			}
			bool isClick=false;
			if(Math.Abs(e.Location.X-_pointMouseDown.X) <3 
				&& Math.Abs(e.Location.Y-_pointMouseDown.Y) <3
				&& _dateTimeMouseDownPanel.AddMilliseconds(600) > DateTime.Now)//anything longer than a 600 ms mouse down becomes a drag
			{
				isClick=true;
			}
			if(_drawMode==EnumDrawMode.Text){
				if(!_isMouseDownPanel) {
					return;
				}
				if(_idxTextMouseDown!=-1){//releasing from a drag, click, or double click
					if(_isDraggingText){//excludes the click and double click
						_isDraggingText=false;
						ImageDraws.Update(_listImageDraws[_idxTextMouseDown]);
						_idxTextMouseDown=-1;
						panelMain.Invalidate();
					}
				}
				_isMouseDownPanel=false;
				return;
			}
			if(_drawMode==EnumDrawMode.Pen
				|| _drawMode==EnumDrawMode.Polygon)
			{
				if(!_isMouseDownPanel) {
					return;
				}
				if(_listPointFsDrawing.Count<2){
					_listImageDraws.RemoveAt(_listImageDraws.Count-1);
				}
				else{
					ImageDraws.Insert(_listImageDraws[_listImageDraws.Count-1]);
					//no point doing a refresh from db, though
				}
				panelMain.Invalidate();
				_listPointFsDrawing.Clear();
				_isMouseDownPanel=false;
				return;
			}
			if(_drawMode==EnumDrawMode.Line){
				if(Math.Abs(_pointMouseDown.X-e.Location.X)+Math.Abs(_pointMouseDown.Y-e.Location.Y)<3){
					//treat this as the mouse up of a click
					//_isLineStarted will remain true, allowing the second click to end the line
				}
				else{//treat this as the mouse up of a drag
					if(_idxImageDraw==-1) {
						return; //Clicking and dragging to fast
					}
					if(_isLineExtending){
						ImageDraws.Update(_listImageDraws[_idxImageDraw]);
					}
					else{
						ImageDraws.Insert(_listImageDraws[_idxImageDraw]);
					}
					_isLineStarted=false;//it won't respond to a second click
					_isLineExtending=false;
					_listPointFsDrawing.Clear();
					panelMain.Invalidate();
				}
				_isMouseDownPanel=false;
				return;
			}
			if(_drawMode==EnumDrawMode.LineEditPoints){
				_isMouseDownPanel=false;
				if(_idxImageDraw==-1 || _idxPoint==-1){
					return;
				}
				ImageDraws.Update(_listImageDraws[_idxImageDraw]);
				_idxImageDraw=-1;
				_idxPoint=-1;
				return;
			}
			if(_drawMode==EnumDrawMode.LineMeasure){
				_isMouseDownPanel=false;
				return;
			}
			if(_drawMode==EnumDrawMode.LineSetScale){
				_isMouseDownPanel=false;
				return;
			}
			if(_drawMode==EnumDrawMode.Eraser
				|| _drawMode==EnumDrawMode.ChangeColor)
			{
				//do nothing
			}
			_isMouseDownPanel=false;
			if(GetSelectedType()==EnumImageNodeType.Mount && isClick	&& _cropPanAdj==EnumCropPanAdj.Pan){//Adj is handled on mouse down 
				//user clicked on a mount item to highlight it
				int idxSelectedInMountOld=_idxSelectedInMount;
				_idxSelectedInMount=HitTestInMount(e.X,e.Y);
				if(_idxSelectedInMount!=idxSelectedInMountOld){
					LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.OnlyRaw);
				}
				EnableToolBarButtons();
				_isDraggingMount=false;
				panelMain.Invalidate();
				SetWindowingSlider();
				return;
			}
			//Dragging a mount item:
			if(GetSelectedType()==EnumImageNodeType.Mount && !isClick && _cropPanAdj==EnumCropPanAdj.Adj && _isDraggingMount){
				SetWindowingSlider();
				//Calc where the center of the dragged image is, in mount coords, assuming it was same size as mount rect.
				//This won't work perfectly if the mount has gaps between items, so we might need to add a supplemental check for that. Or not.
				//Point pointCenter=new Point(_pointDragNow.X+_listMountItems[_idxSelectedInMount].Width/2,_pointDragNow.Y+_listMountItems[_idxSelectedInMount].Height/2);
				int idxNewPos=HitTestInMount(_pointDragNow.X,_pointDragNow.Y,true);//pointCenter.X,pointCenter.Y);
				if(idxNewPos==-1){
					_isDraggingMount=false;
					panelMain.Invalidate();
					return;
				}
				Document documentOld=_documentArrayShowing[_idxSelectedInMount].Copy();
				if(idxNewPos!=_idxSelectedInMount && _documentArrayShowing[idxNewPos]!=null){
					//user dragged to an occupied spot, so swap
					//to keep it simple, we're going to reset to no crop when we swap them
					long mountItemNumOldPos=_documentArrayShowing[_idxSelectedInMount].MountItemNum;
					_documentArrayShowing[_idxSelectedInMount].MountItemNum=_listMountItems[idxNewPos].MountItemNum;
					_documentArrayShowing[_idxSelectedInMount].CropX=0;
					_documentArrayShowing[_idxSelectedInMount].CropY=0;
					_documentArrayShowing[_idxSelectedInMount].CropW=0;
					_documentArrayShowing[_idxSelectedInMount].CropH=0;
					if(_documentArrayShowing[_idxSelectedInMount].ToothNumbers==_listMountItems[_idxSelectedInMount].ToothNumbers){//only if unchanged by user
						_documentArrayShowing[_idxSelectedInMount].ToothNumbers=_listMountItems[idxNewPos].ToothNumbers;
					}
					Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
					documentOld=_documentArrayShowing[idxNewPos].Copy();
					_documentArrayShowing[idxNewPos].MountItemNum=mountItemNumOldPos;
					_documentArrayShowing[idxNewPos].CropX=0;
					_documentArrayShowing[idxNewPos].CropY=0;
					_documentArrayShowing[idxNewPos].CropW=0;
					_documentArrayShowing[idxNewPos].CropH=0;
					if(_documentArrayShowing[idxNewPos].ToothNumbers==_listMountItems[idxNewPos].ToothNumbers){//only if unchanged by user
						_documentArrayShowing[idxNewPos].ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
					}
					Documents.Update(_documentArrayShowing[idxNewPos],documentOld);
					Bitmap bitmapOldPos=_bitmapArrayShowing[_idxSelectedInMount];//no dispose, just ref
					_bitmapArrayShowing[_idxSelectedInMount]=_bitmapArrayShowing[idxNewPos];
					_bitmapArrayShowing[idxNewPos]=bitmapOldPos;
					Document documentOldPos=_documentArrayShowing[_idxSelectedInMount];
					_documentArrayShowing[_idxSelectedInMount]=_documentArrayShowing[idxNewPos];
					_documentArrayShowing[idxNewPos]=documentOldPos;
					_isDraggingMount=false;
					ThumbnailRefresh();
					panelMain.Invalidate();
					return;
				}
				//calculate the distance of drag that happened, in mount coords.
				int xDrag=ControlPointToBitmapPoint(e.Location).X-ControlPointToBitmapPoint(_pointMouseDown).X;
				int yDrag=ControlPointToBitmapPoint(e.Location).Y-ControlPointToBitmapPoint(_pointMouseDown).Y;
				if(idxNewPos!=_idxSelectedInMount){
					bool movedPosNoCrop=false;//moving to a new mount position, but no crop was specified, then don't add one
					_documentArrayShowing[_idxSelectedInMount].MountItemNum=_listMountItems[idxNewPos].MountItemNum;
					if(_documentArrayShowing[_idxSelectedInMount].ToothNumbers==_listMountItems[_idxSelectedInMount].ToothNumbers){//only if unchanged by user
						_documentArrayShowing[_idxSelectedInMount].ToothNumbers=_listMountItems[idxNewPos].ToothNumbers;
					}
					if(_documentArrayShowing[_idxSelectedInMount].CropW==0 || _documentArrayShowing[_idxSelectedInMount].CropH==0){
						movedPosNoCrop=true;
					}
					Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
					_bitmapArrayShowing[idxNewPos]=_bitmapArrayShowing[_idxSelectedInMount];
					_bitmapArrayShowing[_idxSelectedInMount]=null;
					_documentArrayShowing[idxNewPos]=_documentArrayShowing[_idxSelectedInMount];
					_documentArrayShowing[_idxSelectedInMount]=null;
					//reference point for drag needs to be relative to new mountitem instead of old mountitem, or crop xy gets set wrong
					xDrag+=_listMountItems[_idxSelectedInMount].Xpos+_listMountItems[_idxSelectedInMount].Width/2
						-(_listMountItems[idxNewPos].Xpos+_listMountItems[idxNewPos].Width/2);
					yDrag+=_listMountItems[_idxSelectedInMount].Ypos+_listMountItems[_idxSelectedInMount].Height/2
						-(_listMountItems[idxNewPos].Ypos+_listMountItems[idxNewPos].Height/2);
					_idxSelectedInMount=idxNewPos;
					if(movedPosNoCrop){//not adding crop, so nothing else to do
						_isDraggingMount=false;
						ThumbnailRefresh();
						panelMain.Invalidate();
						return;
					}
					//It's now in new idx, but we still need to fix the crop below
				}
				//if no existing crop, we need to define width and height.
				//It's allowed to have an existing rotation of any arbitrary degree.
				if(_documentArrayShowing[_idxSelectedInMount].CropW==0 || _documentArrayShowing[_idxSelectedInMount].CropH==0){
					Size sizeCrop=ImageHelper.CalcSizeFit(
						widthImage:_bitmapArrayShowing[_idxSelectedInMount].Width,
						heightImage:_bitmapArrayShowing[_idxSelectedInMount].Height,
						degreesRotated:_documentArrayShowing[_idxSelectedInMount].DegreesRotated,
						sizeCanvas:new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height));
					_documentArrayShowing[_idxSelectedInMount].CropW=sizeCrop.Width;
					_documentArrayShowing[_idxSelectedInMount].CropH=sizeCrop.Height;
				}
				//convert the drag to image coords
				float scaleImage;
				if(_documentArrayShowing[_idxSelectedInMount].CropW > 0 && _documentArrayShowing[_idxSelectedInMount].CropH > 0){
					scaleImage=(float)_listMountItems[_idxSelectedInMount].Width/_documentArrayShowing[_idxSelectedInMount].CropW;//example 100/200=.5 because image is smaller
				}
				else{
					//We don't have a real crop, so this handles the math:
					scaleImage=ImageTools.CalcScaleFit(
						sizeCanvas:new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),//this is for proportions
						sizeImage:_bitmapArrayShowing[_idxSelectedInMount].Size,
						degreesRotated:_documentArrayShowing[_idxSelectedInMount].DegreesRotated);
				}
				xDrag=(int)Math.Round(-xDrag/scaleImage);//drag to R, crop moves left
				yDrag=(int)Math.Round(yDrag/scaleImage);//drag down, crop moves up, which is positive
				_documentArrayShowing[_idxSelectedInMount].CropX+=xDrag;
				_documentArrayShowing[_idxSelectedInMount].CropY+=yDrag;
				//make sure the crop area falls within the image
				//The math below uses GDI, positive is to BR, which is flipped vertically from our normal matrix math.
				//We'll still use 0,0 as center, and we'll keep it visually the same orientation.
				Rectangle rectangleBitmap=new Rectangle(-_bitmapArrayShowing[_idxSelectedInMount].Width/2,//left
					-_bitmapArrayShowing[_idxSelectedInMount].Height/2,//up
					_bitmapArrayShowing[_idxSelectedInMount].Width,
					_bitmapArrayShowing[_idxSelectedInMount].Height);
				Region regionBitmap=new Region(rectangleBitmap);
				Matrix matrixRotate=new Matrix();
				matrixRotate.Rotate(_documentArrayShowing[_idxSelectedInMount].DegreesRotated);
				regionBitmap.Transform(matrixRotate);
				Rectangle rectangleCrop=new Rectangle(
					_documentArrayShowing[_idxSelectedInMount].CropX-_documentArrayShowing[_idxSelectedInMount].CropW/2,
					-_documentArrayShowing[_idxSelectedInMount].CropY-_documentArrayShowing[_idxSelectedInMount].CropH/2,
					_documentArrayShowing[_idxSelectedInMount].CropW,
					_documentArrayShowing[_idxSelectedInMount].CropH);
				bool doRectanglesIntersect=regionBitmap.IsVisible(rectangleCrop);
				if(!doRectanglesIntersect){
					//can't zero out here because image would need to be reloaded and is treated differently when cropW=0
					//So we need to pull the image back in closer without changing crop size.
					_documentArrayShowing[_idxSelectedInMount].CropX=0;
					_documentArrayShowing[_idxSelectedInMount].CropY=0;
				}
				Documents.Update(_documentArrayShowing[_idxSelectedInMount],documentOld);
				_isDraggingMount=false;
				ThumbnailRefresh();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" was moved using the Adjust button";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
				panelMain.Invalidate();
				return;
			}
			if(GetSelectedType()==EnumImageNodeType.Mount && _cropPanAdj==EnumCropPanAdj.Adj){
				//catches rest of situations not addressed above for mounts
				_isDraggingMount=false;
				SetWindowingSlider();
				panelMain.Invalidate();
			}
			if(_cropPanAdj==EnumCropPanAdj.Crop){
				if(_rectangleCrop.Width<=0 || _rectangleCrop.Height<=0) {
					return;
				}
				Point pointRaw1=new Point(_rectangleCrop.X,_rectangleCrop.Y);
				Point pointRaw2=new Point(_rectangleCrop.X+_rectangleCrop.Width,_rectangleCrop.Y+_rectangleCrop.Height);
				//untangle so that pointCrop1 is always at the upper left
				Rectangle rectangle=new Rectangle(Math.Min(pointRaw1.X,pointRaw2.X), Math.Min(pointRaw1.Y,pointRaw2.Y),
					Math.Abs(pointRaw1.X-pointRaw2.X),	Math.Abs(pointRaw1.Y-pointRaw2.Y));
				PointF pointFCrop1=new PointF(rectangle.X,rectangle.Y);
				PointF pointFCrop2=new PointF(rectangle.Right,rectangle.Bottom);
				//This is a partial copy of code from ControlPointToBitmapPointF, except without the image rotation
				//Again, each step is "opposite".
				Matrix matrix=new Matrix();
				//to center of screen
				matrix.Translate(-panelMain.Width/2,-panelMain.Height/2,MatrixOrder.Append);
				//scale
				matrix.Scale(1f/ZoomSliderValue*100f,1f/ZoomSliderValue*100f,MatrixOrder.Append);//1 if no scale
				//from center of screen to center of crop area (because _pointTranslation is in image coords
				matrix.Translate(-_pointTranslation.X,-_pointTranslation.Y,MatrixOrder.Append);
				//from center of crop area to center image
				//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
				matrix.Translate(GetDocumentShowing(0).CropX,-GetDocumentShowing(0).CropY,MatrixOrder.Append);
				//so now we are at the center of the image and in image coords.
				PointF[] pointFArray={pointFCrop1,pointFCrop2};
				matrix.TransformPoints(pointFArray);
				//I can't find the matrix command for flip vertically, so we'll do that part manually later.
				pointFCrop1=new PointF(pointFArray[0].X,pointFArray[0].Y);
				pointFCrop2=new PointF(pointFArray[1].X,pointFArray[1].Y);
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Crop to Rectangle?")) {
					_rectangleCrop=new Rectangle();
					panelMain.Invalidate();
					return;
				}
				//MessageBox.Show(rectangleCrop.ToString());	
				Document documentOld=GetDocumentShowing(0).Copy();
				float width=pointFCrop2.X-pointFCrop1.X;
				float height=pointFCrop2.Y-pointFCrop1.Y;
				GetDocumentShowing(0).CropX=(int)Math.Round(pointFCrop1.X+width/2);//center
				GetDocumentShowing(0).CropY=(int)Math.Round(-pointFCrop1.Y-height/2);//flip sign here for our center coord system
				GetDocumentShowing(0).CropW=(int)Math.Round(width);
				GetDocumentShowing(0).CropH=(int)Math.Round(height);
				Documents.Update(GetDocumentShowing(0),documentOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				LoadBitmap(0,EnumLoadBitmapType.IdxAndRaw);
				SetZoomSlider();
				_pointTranslation=new Point();
				_rectangleCrop=new Rectangle();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[0].DocCategory);
				string logText="Document Edited: "+_documentArrayShowing[0].FileName
					+" with category "+defDocCategory.ItemName+" was changed using the Crop button";
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,PatientCur.PatNum,logText);
				panelMain.Invalidate();
			}
		}

		private void panelMain_Paint(object sender, PaintEventArgs e){
			//consider original image, crop, and zoom
			Graphics g=e.Graphics;//alias
			if(IsMountShowing()){
				g.Clear(_mountShowing.ColorBack);
			}
			else{
				g.Clear(Color.White);
			}
			if(GetDocumentShowing(0)==null && _mountShowing==null){
				return;
			}
			//Center screen:
			g.TranslateTransform(panelMain.Width/2,panelMain.Height/2);
			//because of order, scaling is center of panel instead of center of image.
			float scaleFactor=ZoomSliderValue/100f;
			g.ScaleTransform(scaleFactor,scaleFactor);
			//and the user translation must be in image coords rather than panel coords
			g.TranslateTransform(_pointTranslation.X,_pointTranslation.Y);
			try{
				if(IsDocumentShowing() && GetBitmapShowing(0)!=null){
					DrawDocument(g);
				}
				if(IsMountShowing()){
					DrawMount(g);
				}
			}
			catch{
				//one of the objects above might get damaged in the middle of a paint.
			}
		}

		private void panelMain_MouseWheel(object sender, MouseEventArgs e){
			if(GetSelectedType()==EnumImageNodeType.None) {
				return;
			}
			if(GetSelectedType()==EnumImageNodeType.Category) {
				return;
			}
			if(_cropPanAdj==EnumCropPanAdj.Pan){
				if(!IsImageFloatSelected){
					Select();//take focus. This will fire the Activated event, and ControlImagesJ will then set this as the active floater.
				}
				float deltaZoom=ZoomSliderValue*(float)e.Delta/SystemInformation.MouseWheelScrollDelta/8f;//For example, -15
				EventZoomSliderSetByWheel?.Invoke(this,deltaZoom);
				panelMain.Invalidate();
			}
			if(_cropPanAdj==EnumCropPanAdj.Adj){
				//no
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>Calculates in pixels, which separately get converted according to scale.</summary>
		private float CalcLengthLine(List<PointF> listPointFs){
			float lengthTotal=0;
			for(int p=1;p<listPointFs.Count;p++){
				float x1=listPointFs[p-1].X;
				float y1=listPointFs[p-1].Y;
				float x2=listPointFs[p].X;
				float y2=listPointFs[p].Y;
				float lengthLine=(float)Math.Sqrt(Math.Pow(x2-x1,2)+Math.Pow(y2-y1,2));
				lengthTotal+=lengthLine;
			}
			return lengthTotal;
		}

		private void ChangeColorAt(PointF pointF){
			float testRadius=5f/(float)ZoomSliderValue*100f;//example: zoomed out, 5/60*100=8.3, so test radius is larger on the image
			float wandXoffset=2f/(float)ZoomSliderValue*100f;
			float wandYoffset=1f/(float)ZoomSliderValue*100f;
			//The wand tip doesn't quite match cursor target, we add 1 or 2
			PointF pointFWandTip=new PointF(pointF.X+wandXoffset,pointF.Y+wandYoffset);
			//look for any lines that intersect the wand.
			//since the line segments are so short, it's sufficient to check end points.
			for(int i=0;i<_listImageDraws.Count;i++) {
				if(_listImageDraws[i].DrawType==ImageDrawType.Pen
					|| _listImageDraws[i].DrawType==ImageDrawType.Line
					|| _listImageDraws[i].DrawType==ImageDrawType.Polygon)
				{
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					for(int p=1;p<listPointFs.Count;p++) {
						if(!HitTestLine(listPointFs[p-1],listPointFs[p],pointFWandTip,testRadius)){
							continue;
						}
						if(_listImageDraws[i].ColorDraw==ColorFore){
							return;
						}
						_listImageDraws[i].ColorDraw=ColorFore;
						ImageDraws.Update(_listImageDraws[i]);
						panelMain.Invalidate();
						return;
					}
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Polygon){
					//This supports changing color of a polygon by clicking in the middle
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					using GraphicsPath graphicsPath=new GraphicsPath();
					graphicsPath.AddLines(listPointFs.ToArray());
					Region region=new Region(graphicsPath);
					if(region.IsVisible(pointFWandTip.X,pointFWandTip.Y)){//IsVisible really means contains
						if(_listImageDraws[i].ColorDraw==ColorFore){
							return;
						}
						_listImageDraws[i].ColorDraw=ColorFore;
						ImageDraws.Update(_listImageDraws[i]);
						panelMain.Invalidate();
						return;
					}
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Text){
					Point pointText=_listImageDraws[i].GetTextPoint();
					string str=_listImageDraws[i].GetTextString();
					using Font font=new Font(FontFamily.GenericSansSerif,_listImageDraws[i].GetFontSize());
					int widthText=(int)LayoutManager.ScaleMS(TextRenderer.MeasureText(str,font).Width);
					Rectangle rectangleText=new Rectangle(pointText.X,pointText.Y,widthText,font.Height);
					if(!rectangleText.Contains(Point.Round(pointF))){
						continue;
					}
					if(_listImageDraws[i].ColorDraw==ColorFore
						&& _listImageDraws[i].ColorBack==ColorTextBack)
					{
						return;
					}
					_listImageDraws[i].ColorDraw=ColorFore;
					_listImageDraws[i].ColorBack=ColorTextBack;
					ImageDraws.Update(_listImageDraws[i]);
					panelMain.Invalidate();
					return;
				}
			}
		}

		///<summary>Mostly to dispose of the old bitmaps all in one place.</summary>
		private void ClearObjects(){
			_bitmapRaw?.Dispose();
			_bitmapRaw=null;
			if(_bitmapArrayShowing!=null && _bitmapArrayShowing.Length>0){
				for(int i=0;i<_bitmapArrayShowing.Length;i++){
					_bitmapArrayShowing[i]?.Dispose();
					_bitmapArrayShowing[i]=null;
				}
			}
			_bitmapArrayShowing=null;
			SetDocumentShowing(0,null);
			_mountShowing=null;
			panelMain.Invalidate();
		}

		private Point ControlPointToBitmapPoint(Point pointPanel) {
			PointF pointF=ControlPointToBitmapPointF(pointPanel);
			return Point.Round(pointF);
		}

		///<summary>Converts a point in panelMain into a point in _bitmapShowing, with origin at UL.  If mount, then it's coords within entire mount.</summary>
		private PointF ControlPointToBitmapPointF(Point pointPanel) {
			if(!IsDocumentShowing() && !IsMountShowing()) {
				return pointPanel;
			}
			//because we're going backward from all the other rendering, each step is the "opposite".
			Matrix matrix=new Matrix();
			//to center of screen
			matrix.Translate(-panelMain.Width/2,-panelMain.Height/2,MatrixOrder.Append);
			//scale
			matrix.Scale(1f/ZoomSliderValue*100f,1f/ZoomSliderValue*100f,MatrixOrder.Append);//1 if no scale
			//from center of screen to center of crop area (because _pointTranslation is in image coords
			matrix.Translate(-_pointTranslation.X,-_pointTranslation.Y,MatrixOrder.Append);
			if(IsMountShowing()){
				//mounts don't get rotated
				matrix.Translate(_mountShowing.Width/2f,_mountShowing.Height/2f,MatrixOrder.Append);
				PointF[] pointFs2={pointPanel };
				matrix.TransformPoints(pointFs2);
				return pointFs2[0];
			}
			//from center of crop area to center image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			matrix.Translate(GetDocumentShowing(0).CropX,-GetDocumentShowing(0).CropY,MatrixOrder.Append);
			if(GetBitmapShowing(0) is null){
				return Point.Empty;
			}
			//Rotate around center of image
			matrix.Rotate(-GetDocumentShowing(0).DegreesRotated,MatrixOrder.Append);
			if(GetDocumentShowing(0).IsFlipped){
				Matrix matrixFlip=new Matrix(-1,0,0,1,0,0);
				matrix.Multiply(matrixFlip,MatrixOrder.Append);
			}
			//Translate from center of image to UL corner of image
			matrix.Translate(GetBitmapShowing(0).Width/2,GetBitmapShowing(0).Height/2,MatrixOrder.Append);
			PointF[] pointFArray={pointPanel };
			matrix.TransformPoints(pointFArray);
			return pointFArray[0];
		}

		///<summary>The thumbnail is retrieved in Mounts.GetThumbnail. This strategy is different than document because it must be done while we already have the files open.</summary>
		private void CreateMountThumbnail(){
			using Bitmap bitmap=new Bitmap(100,100);
			using Graphics g=Graphics.FromImage(bitmap);
			g.TranslateTransform(50,50);//Center
			//scale it slightly smaller so that the right pixels won't get cut off
			if(_mountShowing.Height==0){
				return;
			}
			float scale=ImageTools.CalcScaleFit(new Size(99,99),new Size(_mountShowing.Width,_mountShowing.Height),0);
			g.ScaleTransform(scale,scale);
			DrawMount(g);
			string pathThumbnails=Path.Combine(PatFolder,"Thumbnails");
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !Directory.Exists(pathThumbnails)) {
				try {
					Directory.CreateDirectory(pathThumbnails);
				} 
				catch{
					return;
				}
			}
			string fileName="Mount"+_mountShowing.MountNum.ToString()+".jpg";
			string fileNameFull=Path.Combine(PatFolder,"Thumbnails",fileName);
			//todo: if storage in db
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				if(File.Exists(fileNameFull)){
					File.Delete(fileNameFull);
				}
				try {
					bitmap.Save(fileNameFull);
				}
				catch(Exception ex){
					ex.DoNothing();
				}
			}
		}

		///<summary>For API. Downloads the file contained in a document's note field, if there is one to be downloaded. A url must be prepended with a "_download_:" prefix or a base64 string with a "_rawBase64_:" prefix, which is removed upon successful download. The new file is saved in the current patient's folder.  </summary>
		private void DownloadDocumentNoteFile(NodeTypeAndKey nodeTypeAndKey) {
			Document document=GetDocumentShowing(0);
			if(document==null) {
				return;
			}
			string downloadPrefix="_download_:";
			string base64Prefix="_rawBase64_:";
			if(!document.Note.StartsWith(downloadPrefix) && !document.Note.StartsWith(base64Prefix)) { //only process notes that contain a url or base64
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			Document documentImported=new Document();
			if(document.Note.StartsWith(downloadPrefix)) {
				string url=document.Note.Substring(downloadPrefix.Length);
				string tempFileName=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),PatientCur.PatNum.ToString()+Path.GetExtension(url));
				WebClient webClient=new WebClient();
				progressOD.StartingMessage=Lan.g(this,"Downloading file from url")+"...";
				progressOD.ActionMain=() => {
					webClient.DownloadFile(url,tempFileName);
					documentImported=ImageStore.Import(tempFileName,document.DocCategory,PatientCur);
					File.Delete(tempFileName);
				};
			}
      else if(document.Note.StartsWith(base64Prefix)) {
				string base64=document.Note.Substring(base64Prefix.Length);
				string extension=StringTools.SubstringBefore(base64,"_");
				base64=base64.Substring(extension.Length+1); //trim extension from base64 string
				byte[] byteArray=Convert.FromBase64String(base64);
				string tempFileName=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),PatientCur.PatNum.ToString()+extension);
				progressOD.StartingMessage=Lan.g(this,"Converting file from base64")+"...";
				progressOD.ActionMain=() => {
					File.WriteAllBytes(tempFileName,byteArray);
					documentImported=ImageStore.Import(tempFileName,document.DocCategory,PatientCur);
					File.Delete(tempFileName);
				};
			}
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception e) {
				MsgBox.Show(this,"Unable to download file.");
				return;
			}
			if(progressOD.IsCancelled) {
				return;
			}
			Documents.Delete(document);
			documentImported.Description=document.Description;
			documentImported.DocCategory=document.DocCategory;
			documentImported.ImgType=document.ImgType;
			documentImported.DateCreated=document.DateCreated;
			documentImported.ToothNumbers=document.ToothNumbers;
			Documents.Update(documentImported);
			EventFillTree?.Invoke(this,false);//updates tree to immediately include new file
			NodeTypeAndKey nodeTypeAndKey2=new NodeTypeAndKey(EnumImageNodeType.Document,documentImported.DocNum);
			EventSelectTreeNode?.Invoke(this,nodeTypeAndKey2);
		}

		///<summary></summary>
		private void DrawDocument(Graphics g){
			Document document=GetDocumentShowing(0);
			//we're at the center of the crop area, and working in image coordinates
			if(document.CropW>0 && document.CropH>0){
				g.SetClip(new Rectangle(-document.CropW/2,-document.CropH/2,document.CropW,document.CropH));
			}
			//Translate from center of crop area to center of image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			g.TranslateTransform(-document.CropX,document.CropY);
			//rotate around center of image
			g.RotateTransform(document.DegreesRotated);
			if(document.IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Translate from center of image to UL corner of image
			g.TranslateTransform(-GetBitmapShowing(0).Width/2,-GetBitmapShowing(0).Height/2);
			g.DrawImage(GetBitmapShowing(0),0,0,GetBitmapShowing(0).Width,GetBitmapShowing(0).Height);
			DrawDrawings(g,-document.DegreesRotated);
			g.ResetClip();
			if(_rectangleCrop.Width>0 && _rectangleCrop.Height>0) {//Drawn last so it's on top.
				g.ResetTransform();//panel coords
				g.DrawRectangle(Pens.Blue,_rectangleCrop);
			}
		}

		///<summary>Used for drawing on screen in real time and also for drawing to printer.</summary>
		private void DrawMount(Graphics g){
			//we're at center of the mount, and working in mount coordinates
			//translate to UL of mount
			g.TranslateTransform(-_mountShowing.Width/2f,-_mountShowing.Height/2f);
			using SolidBrush solidBrushBack=new SolidBrush(_mountShowing.ColorBack);
			g.FillRectangle(solidBrushBack,0,0,_mountShowing.Width,_mountShowing.Height);
			for(int i=0;i<_listMountItems.Count;i++){
				if(_isDraggingMount && i==_idxSelectedInMount){// && _dateTimeMouseDown.AddMilliseconds(600) < DateTime.Now){//_pointDragStart!=_pointDragNow){
					continue;//we'll paint this one after the loop
				}
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				DrawMountOne(g,i);
				DrawMountOneBigNumber(g,i);
			}
			if(_isDraggingMount && _idxSelectedInMount>-1){// && _dateTimeMouseDown.AddMilliseconds(600) < DateTime.Now){//_pointDragStart!=_pointDragNow){
				DrawMountOne(g,_idxSelectedInMount);
			}
			//SELECT * FROM document WHERE patnum=293 AND mountitemnum > 0
			//outlines:
			using Pen penOutline=new Pen(ColorOD.Gray(100));
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				g.DrawRectangle(penOutline,_listMountItems[i].Xpos,_listMountItems[i].Ypos,
					_listMountItems[i].Width,_listMountItems[i].Height);//silver is 50% black
			}
			DrawDrawings(g,0);
			//yellow outline of selected
			if(_idxSelectedInMount!=-1){
				g.DrawRectangle(Pens.Yellow,_listMountItems[_idxSelectedInMount].Xpos,_listMountItems[_idxSelectedInMount].Ypos,
					_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
			}
		}

		private void DrawDrawings(Graphics g,float rotateText){
			for(int i=0;i<_listImageDraws.Count;i++){
				if(!_showDrawingsOD && _listImageDraws[i].ImageAnnotVendor==EnumImageAnnotVendor.OpenDental){
					continue;
				}
				if(!_showDrawingsPearl && _listImageDraws[i].ImageAnnotVendor==EnumImageAnnotVendor.Pearl){
					continue;
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Text){
					GraphicsState graphicsState=g.Save();
					Point point=_listImageDraws[i].GetTextPoint();
					g.TranslateTransform(point.X,point.Y);
					using Font font=new Font(FontFamily.GenericSansSerif,_listImageDraws[i].GetFontSize());
					string str=_listImageDraws[i].GetTextString();
					Size size= g.MeasureString(str,font).ToSize();
					size=new Size((int)LayoutManager.ScaleMS(size.Width),(int)LayoutManager.ScaleMS(size.Height));
					Rectangle rectangle=new Rectangle(new Point(0,0),size);
					g.RotateTransform(rotateText);
					if(_listImageDraws[i].ColorBack!=Color.Empty){
						using SolidBrush brushBack=new SolidBrush(_listImageDraws[i].ColorBack);
						g.FillRectangle(brushBack,rectangle);
					}
					using SolidBrush solidBrush=new SolidBrush(_listImageDraws[i].ColorDraw);
					g.DrawString(str,font,solidBrush,0,0);
					g.Restore(graphicsState);
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Pen
					|| _listImageDraws[i].DrawType==ImageDrawType.Line)
				{
					g.SmoothingMode=SmoothingMode.HighQuality;
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					using Pen pen=new Pen(_listImageDraws[i].ColorDraw,2);//2 pixels wide
					using SolidBrush solidBrush=new SolidBrush(_listImageDraws[i].ColorDraw);
					for(int p=1;p<listPointFs.Count;p++){//start at the second point.
						g.DrawLine(pen,listPointFs[p-1],listPointFs[p]);
						RectangleF rectangleF=new RectangleF(listPointFs[p].X-1,listPointFs[p].Y-1,2,2);
						g.FillEllipse(solidBrush,rectangleF);
					}
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Polygon){
					g.SmoothingMode=SmoothingMode.HighQuality;
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					using SolidBrush solidBrush=new SolidBrush(_listImageDraws[i].ColorDraw);
					g.FillPolygon(solidBrush,listPointFs.ToArray());
				}
			}
			//for the measurement hover effect:
			if(!_stringMeasure.IsNullOrEmpty()){
				SolidBrush solidBrushBack=new SolidBrush(ColorTextBack);
				float fontSize=8f/(float)ZoomSliderValue*100f;
				using Font font=new Font(FontFamily.GenericSansSerif,fontSize);
				Size size= g.MeasureString(_stringMeasure,font).ToSize();
				size=new Size((int)LayoutManager.ScaleMS(size.Width),(int)LayoutManager.ScaleMS(size.Height));
				Rectangle rectangle=new Rectangle(_pointMeasureText,size);
				g.FillRectangle(solidBrushBack,rectangle);				
				SolidBrush solidBrushText=new SolidBrush(ColorFore);
				g.DrawString(_stringMeasure,font,solidBrushText,_pointMeasureText);
			}
		}

		private void DrawMountOne(Graphics g,int i,bool translateToItemPos=true){
			GraphicsState graphicsStateMount=g.Save();
			//We are already in mount coords
			if(translateToItemPos){
				//translate from UL of mount to UL of mount item
				g.TranslateTransform(_listMountItems[i].Xpos,_listMountItems[i].Ypos);
			}
			if(_isDraggingMount && i==_idxSelectedInMount){// && _dateTimeMouseDown.AddMilliseconds(600) < DateTime.Now){//_pointDragStart!=_pointDragNow){
				g.TranslateTransform(_pointDragNow.X-_pointDragStart.X,_pointDragNow.Y-_pointDragStart.Y);
				//combined with the centering below, this has the effect of dragging based on center
				//we show the entire image when dragging, so no clip
			}
			else{
				g.SetClip(new Rectangle(0,0,_listMountItems[i].Width,_listMountItems[i].Height));
			}
			if(_listMountItems[i].TextShowing!=""){
				using SolidBrush solidBrushBack=new SolidBrush(_mountShowing.ColorTextBack);
				RectangleF rectangleF=new RectangleF(0,0,_listMountItems[i].Width,_listMountItems[i].Height);
				g.FillRectangle(solidBrushBack,rectangleF);
				using Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(_listMountItems[i].FontSize));
				using SolidBrush solidBrushFore=new SolidBrush(_mountShowing.ColorFore);
				string str=_listMountItems[i].TextShowing;
				str=Patients.ReplacePatient(str,PatientCur);
				str=Mounts.ReplaceMount(str,_mountShowing);
				Clinic clinic=Clinics.GetClinic(PatientCur.ClinicNum);
				str=Clinics.ReplaceOffice(str,clinic);
				g.DrawString(str,font,solidBrushFore,rectangleF);
				g.Restore(graphicsStateMount);
				return;
			}
			if(_bitmapArrayShowing[i]==null){
				g.Restore(graphicsStateMount);
				return;
			}
			//translate from UL of mount item to center of mount item
			g.TranslateTransform(_listMountItems[i].Width/2f,_listMountItems[i].Height/2f);
			//We are now working in the center of the crop area
			float scale;
			if(_documentArrayShowing[i].CropW > 0 && _documentArrayShowing[i].CropH > 0){
				scale=(float)_listMountItems[i].Width/_documentArrayShowing[i].CropW;//example 100/200=.5 because image needs to be smaller
			}
			else{
				//We don't have a real crop, so this handles the math:
				scale=ImageTools.CalcScaleFit(
					sizeCanvas:new Size(_listMountItems[i].Width,_listMountItems[i].Height),//this is for proportions
					sizeImage:_bitmapArrayShowing[i].Size,
					degreesRotated:_documentArrayShowing[i].DegreesRotated);
			}
			g.ScaleTransform(scale,scale);
			//We are in bitmap coords from here down.
			//Translate from center of crop area to center of image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			g.TranslateTransform(-_documentArrayShowing[i].CropX,_documentArrayShowing[i].CropY);
			//Rotate and flip
			g.RotateTransform(_documentArrayShowing[i].DegreesRotated);
			if(_documentArrayShowing[i].IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Translate from center of image to UL corner of image
			g.TranslateTransform(-_bitmapArrayShowing[i].Width/2f,-_bitmapArrayShowing[i].Height/2f);
			//g.TranslateTransform(0,-_arrayBitmapsShowing[i].Height/2f);
			//g.InterpolationMode=InterpolationMode.HighQualityBicubic;//smooths image edges, but way too slow
			//Unlikely that anyone will care, but we could always do this just for rotated.  Or we could draw an antialiased rectangle around the edge.
			g.DrawImage(_bitmapArrayShowing[i],0,0,_bitmapArrayShowing[i].Width,_bitmapArrayShowing[i].Height);
			g.Restore(graphicsStateMount);
		}

		private void DrawMountOneBigNumber(Graphics g,int i){
			if(_bitmapArrayShowing[i]!=null){
				return;//only draw on empty spots
			}
			if(_listMountItems[i].ItemOrder==0){//text item
				return;
			}
			GraphicsState graphicsStateMount=g.Save();
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			g.TranslateTransform(_listMountItems[i].Xpos,_listMountItems[i].Ypos);//UL of mount position
			g.TranslateTransform(_listMountItems[i].Width/2,_listMountItems[i].Height/2);//center
			//So that they all look the same, we use the short dimension
			float heightFontPoint=_listMountItems[i].Height/15f*(96f/72f);
			if(_listMountItems[i].Width<_listMountItems[i].Height){
				heightFontPoint=_listMountItems[i].Width/15f*(96f/72f)/LayoutManager.ScaleMyFont();
			}
			using Font font=new Font(FontFamily.GenericSansSerif,heightFontPoint);
			SizeF sizeString=g.MeasureString(_listMountItems[i].ItemOrder.ToString(),font);
			sizeString=new Size((int)LayoutManager.ScaleMS(sizeString.Width),(int)LayoutManager.ScaleMS(sizeString.Height));
			g.DrawString(_listMountItems[i].ItemOrder.ToString(),font,Brushes.Gray,-sizeString.Width/2,-sizeString.Height/2);
			g.Restore(graphicsStateMount);
		}

		private void EraseAt(PointF pointF){
			float circleRadius=5f/(float)ZoomSliderValue*100f;//example: zoomed out, 5/60*100=8.3, so eraser circle covers a larger area of image
			float testRadius=6f/(float)ZoomSliderValue*100f;
			int circleDiameter=(int)(12f/(float)ZoomSliderValue*100f);
			PointF pointFCircleCenter=new PointF(pointF.X+circleRadius,pointF.Y+circleRadius);//adjusted because tip is actually at UL of a cursor
			//look for any lines that intersect the "eraser".
			//since the line segments are so short, it's sufficient to check end points.
			for(int i=0;i<_listImageDraws.Count;i++) {
				if(_listImageDraws[i].DrawType==ImageDrawType.Line
					|| _listImageDraws[i].DrawType==ImageDrawType.Pen
					|| _listImageDraws[i].DrawType==ImageDrawType.Polygon)
				{
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					for(int p=1;p<listPointFs.Count;p++) {
						if(!HitTestLine(listPointFs[p-1],listPointFs[p],pointFCircleCenter,testRadius)){
							continue;
						}
						ImageDraws.Delete(_listImageDraws[i].ImageDrawNum);
						_listImageDraws.RemoveAt(i);
						panelMain.Invalidate();
						return;
					}
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Polygon){
					//This supports deleting a polygon by clicking in the middle
					List<PointF> listPointFs=_listImageDraws[i].GetPoints();
					using GraphicsPath graphicsPath=new GraphicsPath();
					graphicsPath.AddLines(listPointFs.ToArray());
					Region region=new Region(graphicsPath);
					if(region.IsVisible(pointFCircleCenter.X,pointFCircleCenter.Y)){//IsVisible really means contains
						ImageDraws.Delete(_listImageDraws[i].ImageDrawNum);
						_listImageDraws.RemoveAt(i);
						panelMain.Invalidate();
						return;
					}
				}
				if(_listImageDraws[i].DrawType==ImageDrawType.Text){
					Point pointText=_listImageDraws[i].GetTextPoint();
					string str=_listImageDraws[i].GetTextString();
					float fontSize=_listImageDraws[i].GetFontSize();
					using Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleFontODZoom(fontSize));
					int widthText=(int)LayoutManager.ScaleMS(TextRenderer.MeasureText(str,font).Width);
					//this isn't really the rectangle where the text is.  It's adjusted for the eraser size.
					int heightFont=(int)LayoutManager.ScaleMS(font.Height);
					Rectangle rectangleText=new Rectangle(pointText.X-circleDiameter,pointText.Y-circleDiameter,
						widthText+circleDiameter,heightFont+circleDiameter);
					if(!rectangleText.Contains(Point.Round(pointF))){
						continue;
					}
					ImageDraws.Delete(_listImageDraws[i].ImageDrawNum);
					_listImageDraws.RemoveAt(i);
					panelMain.Invalidate();
					return;
				}
			}
		}

		///<summary>Gets the DefNum category of the current selection.</summary>
		private long GetCurrentCategory() {
			if(_nodeTypeKeyCatSelected==null || _nodeTypeKeyCatSelected.ImageNodeType==EnumImageNodeType.None || _nodeTypeKeyCatSelected.DefNumCategory==0) {//No Image Category selected.
				long imageCategoryDefault=PrefC.GetLong(PrefName.ImageCategoryDefault);
				if(imageCategoryDefault > 0) {
					return imageCategoryDefault;
				}
				return Defs.GetDefsForCategory(DefCat.ImageCats,true)[0].DefNum;//No default Image Category set, return first category.
			}
			return _nodeTypeKeyCatSelected.DefNumCategory;
		}

		///<summary>Only for Pearl. Pass in bitmap or mount coords. Looks for polygon or square (Line) that meets criteria for hover. Returns index within _listImageDraws, or -1 if no hit.</summary>
		private int HitTestHover(PointF pointfBitmap){
			if(!_showDrawingsPearl){
				return -1;
			}
			for(int i=0;i<_listImageDraws.Count;i++){
				if(_listImageDraws[i].ImageAnnotVendor!=EnumImageAnnotVendor.Pearl){
					continue;
				}
				if(_listImageDraws[i].Details==""){//nothing to show
					continue;
				}
				if(_listImageDraws[i].DrawType!=ImageDrawType.Polygon
					&& _listImageDraws[i].DrawType!=ImageDrawType.Line)//treat it like a closed polygon for hit test
				{
					continue;
				}
				List<PointF> listPointFs=_listImageDraws[i].GetPoints();
				using GraphicsPath graphicsPath=new GraphicsPath();
				graphicsPath.AddLines(listPointFs.ToArray());
				Region region=new Region(graphicsPath);
				if(region.IsVisible(pointfBitmap.X,pointfBitmap.Y)){//IsVisible really means contains
					return i;
				}
			}
			return -1;
		}

		///<summary>Pass in panel or mount coords. Returns index of item in mount, or -1 if no hit.</summary>
		private int HitTestInMount(int x,int y,bool isMountCoords=false,bool includeText=false){
			if(_listMountItems==null){
				return -1;
			}
			Point pointMount;
			if(isMountCoords){
				pointMount=new Point(x,y);
			}
			else{
				pointMount=ControlPointToBitmapPoint(new Point(x,y));
			}
			for(int i=0;i<_listMountItems.Count;i++){
				if(_listMountItems[i].ItemOrder==0){
					if(!includeText){
						continue;
					}
				}
				if(_listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				Rectangle rectangle=new Rectangle(_listMountItems[i].Xpos,_listMountItems[i].Ypos,_listMountItems[i].Width,_listMountItems[i].Height);
				if(rectangle.Contains(pointMount)){
					return i;
				}
			}
			return -1;
		}

		///<summary>Returns true for hit.</summary>
		private bool HitTestLine(PointF pointFEnd1,PointF pointFEnd2,PointF pointFToTest,float testRadius){
			float x1=pointFEnd1.X;
			float y1=pointFEnd1.Y;
			float x2=pointFEnd2.X;
			float y2=pointFEnd2.Y;
			float x0=pointFToTest.X;
			float y0=pointFToTest.Y;
			float lengthLine=(float)Math.Sqrt(Math.Pow(x2-x1,2)+Math.Pow(y2-y1,2));
			float distPointToLine=Math.Abs((x2-x1)*(y1-y0)-(x1-x0)*(y2-y1)) / lengthLine;//this distance is always perpendicular
			if(distPointToLine>testRadius){
				return false;
			}
			//So it's close to the line of infinite length that runs through the two end points.
			//We must now determine if it it's beyond the end points or not.
			//This is done by calculating the maximum distance from the point to either end, which is a triangle hypotenuse.
			float distToEnd1=(float)Math.Sqrt(Math.Pow(x1-x0,2)+Math.Pow(y1-y0,2));
			float distToEnd2=(float)Math.Sqrt(Math.Pow(x2-x0,2)+Math.Pow(y2-y0,2));
			float distMax=(float)Math.Sqrt(Math.Pow(lengthLine,2)+Math.Pow(distPointToLine,2));
			if(distToEnd1<=distMax && distToEnd2<=distMax){
				return true;
			}
			//Intersection point lies outside bounds of the main line, so probably ignore it.
			//But it might be close to an end
			if(distToEnd1>testRadius && distToEnd2>testRadius){
				//nope
				return false;
			}
			//So it's beyond the bounds, but close to an end. 
			return true;
		}
		
		///<summary>Pass in panel coords because of text rotation. Returns index of text item, or -1 if no hit.</summary>
		private int HitTestText(Point pointPanel){
			for(int i=0;i<_listImageDraws.Count;i++){
				if(_listImageDraws[i].DrawType!=ImageDrawType.Text){
					continue;
				}
				//Much of this code is copied from ControlPointToBitmapPoint, which we can't use directly because it's backward.
				//This matrix is only for one purpose: to convert the bitmap point where the text is located to a panel point.
				Matrix matrix=new Matrix();
				//to center of screen
				matrix.Translate(panelMain.Width/2,panelMain.Height/2);
				//scale
				matrix.Scale(ZoomSliderValue/100f,ZoomSliderValue/100f);//1 if no scale
				//we are now at image scale
				//from center of screen to center of crop area (because _pointTranslation is in image coords
				matrix.Translate(_pointTranslation.X,_pointTranslation.Y);
				if(IsDocumentShowing()){
					Document document=GetDocumentShowing(0);
					//from center of crop area to center image
					//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
					matrix.Translate(-document.CropX,document.CropY);
					if(GetBitmapShowing(0) is null){
						continue;
					}
					//Rotate around center of image
					matrix.Rotate(document.DegreesRotated);
					if(document.IsFlipped){
						Matrix matrixFlip=new Matrix(-1,0,0,1,0,0);
						matrix.Multiply(matrixFlip);
					}
					//Translate from center of image to UL corner of image
					matrix.Translate(-GetBitmapShowing(0).Width/2,-GetBitmapShowing(0).Height/2);
				}
				else{
					//mounts don't get rotated
					//from center of mount to UL of mount
					matrix.Translate(-_mountShowing.Width/2f,-_mountShowing.Height/2f);
				}
				Point pointTextInImage=_listImageDraws[i].GetTextPoint();
				PointF[] pointFArray={pointTextInImage };
				matrix.TransformPoints(pointFArray);
				Point pointTextInPanel=Point.Round(pointFArray[0]);
				//Font must be in panel scale rather than image scale
				float sizeFont=LayoutManager.ScaleFontODZoom(_listImageDraws[i].GetFontSize())*ZoomSliderValue/100f;
				using Font font=new Font(FontFamily.GenericSansSerif,sizeFont);
				string str=_listImageDraws[i].GetTextString();
				int widthText=(int)LayoutManager.ScaleMS(TextRenderer.MeasureText(str,font).Width);
				//Using panel coords, define a rectangle where this text is located.
				int heightFont=(int)LayoutManager.ScaleMS(font.Height);
				Rectangle rectangleText=new Rectangle(pointTextInPanel.X,pointTextInPanel.Y,widthText,heightFont);
				//See if the point passed in is within that rectangle
				if(rectangleText.Contains(pointPanel)){
					return i;
				}
			}
			return -1;
		}

		///<summary>Loads bitmap from disk, resizes, applies bright/contrast, and saves it to _arrayBitmapsShowing and/or _bitmapRaw.</summary>
		private void LoadBitmap(int idx,EnumLoadBitmapType loadBitmapType){
			if(idx==-1 || _bitmapArrayShowing==null || idx > _bitmapArrayShowing.Length-1){
				return;
			}
			if(_documentArrayShowing[idx]==null){
				return;
			}
			if(loadBitmapType==EnumLoadBitmapType.OnlyIdx || loadBitmapType==EnumLoadBitmapType.IdxAndRaw){
				_bitmapArrayShowing[idx]?.Dispose();
				_bitmapArrayShowing[idx]=null;
			}
			if(loadBitmapType==EnumLoadBitmapType.IdxAndRaw || loadBitmapType==EnumLoadBitmapType.OnlyRaw){
				_bitmapRaw?.Dispose();
				_bitmapRaw=null;
				_bitmapDicomRaw=null;
			}
			if(loadBitmapType==EnumLoadBitmapType.OnlyRaw){
				if(_documentArrayShowing[idx].WindowingMax==0 || (_documentArrayShowing[idx].WindowingMax==255 && _documentArrayShowing[idx].WindowingMin==0)){
					if(_bitmapArrayShowing[idx]==null){
						return;//we should already have an image showing, so this shouldn't happen
					}
					if(_documentArrayShowing[idx].FileName.EndsWith(".dcm")){
						//This optimization doesn't matter because all dicom images need windowing.  We always get a new one.
						//Obviously, this needs improvement.  We don't want to be reloading images from disk just because user is clicking on item in mount.
					}
					else{
						_bitmapRaw=new Bitmap(_bitmapArrayShowing[idx]);
						return;
					}
				}
			}
			//from here down, we must be getting the image from disk===================================================================================
			Bitmap bitmapTemp=null;
			//BitmapDicom bitmapDicom=null;
			if(_documentArrayShowing[idx].FileName.EndsWith(".dcm")){
				_bitmapDicomRaw=ImageStore.OpenBitmapDicom(_documentArrayShowing[idx],PatFolder);
				if(_bitmapDicomRaw==null){
					return;
				}
			}
			else{
				bitmapTemp=ImageStore.OpenImage(_documentArrayShowing[idx],PatFolder);
				if(bitmapTemp==null){
					return;
				}
			}
			if(_nodeTypeKeyCatSelected.ImageNodeType==EnumImageNodeType.Document){
				//always IdxAndRaw
				//single images simply load up the whole unscaled image. Mounts load the whole image, but maybe at a different scale to match mount scale.
				if(_documentArrayShowing[idx].FileName.EndsWith(".dcm")){
					_bitmapArrayShowing[idx]=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_documentArrayShowing[idx].WindowingMin,_documentArrayShowing[idx].WindowingMax);
					return;
				}
				try{
					_bitmapArrayShowing[idx]=new Bitmap(bitmapTemp);//can crash here on a large image (WxHx24 > 250M)
					//jordan I redid my math a few months later, and it's WxHx4, not 24.  Size in memory should not be a problem, 
					//so now I don't know why it chokes on large images or why I've watched my memory usage climb to 1G when loading images.  Revisit.
					ImageHelper.ApplyColorSettings(_bitmapArrayShowing[idx],_documentArrayShowing[idx].WindowingMin,_documentArrayShowing[idx].WindowingMax);
					_bitmapRaw=new Bitmap(bitmapTemp);//or it can crash here
					bitmapTemp.Dispose();//frees up the lock on the file on disk
				}
				catch{
					//This happens with large images.  Not sure why yet.
					//It could be addressed by holding only one image in memory instead of two.  There are downsides.
					//It could also be addressed by downgrading the image or only showing the image at just enough resolution to match screen pixels.  Difficult.
					//It could also be addressed by compressing one bitmap into memory using a stream. e.g. _bitmapRaw. Seems good.
					//It could also be addressed by putting the raw image on the graphics card with Direct2D instead of having a second image to apply windowing to.
					//Yet another solution would be to maintain ref and lock to actual file on disk.
					//The final solution will be some combination of the above.
					//error message will show on return in SelectTreeNode because _bitmapRaw==null
				}
				finally{
					bitmapTemp.Dispose();//frees up the lock on the file on disk
				}
				return;
			}
			//From here down is mount=================================================================================================
			//double scale;
			//if(_arrayDocumentsShowing[idx].CropW==0){//no crop, so scaled to fit mount item
			//	scale=ImageTools.CalcScaleFit(new Size(_listMountItems[idx].Width,_listMountItems[idx].Height),_arraySizesOriginal[idx],_arrayDocumentsShowing[idx].DegreesRotated);
			//}
			//else{
			//	scale=(double)_listMountItems[idx].Width/_arrayDocumentsShowing[idx].CropW;
			//}
			if(loadBitmapType==EnumLoadBitmapType.OnlyIdx || loadBitmapType==EnumLoadBitmapType.IdxAndRaw){
				if(_documentArrayShowing[idx].FileName.EndsWith(".dcm")){
					bitmapTemp=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_documentArrayShowing[idx].WindowingMin,_documentArrayShowing[idx].WindowingMax);
					_bitmapArrayShowing[idx]=new Bitmap(bitmapTemp);
				}
				else{
					_bitmapArrayShowing[idx]=new Bitmap(bitmapTemp);
					ImageHelper.ApplyColorSettings(_bitmapArrayShowing[idx],_documentArrayShowing[idx].WindowingMin,_documentArrayShowing[idx].WindowingMax);
				}
			}
			if(loadBitmapType==EnumLoadBitmapType.IdxAndRaw || loadBitmapType==EnumLoadBitmapType.OnlyRaw){
				if(_documentArrayShowing[idx].FileName.EndsWith(".dcm")){
					//already got _bitmapDicomRaw near beginning of this method
				}
				else{
					_bitmapRaw=new Bitmap(bitmapTemp);
				}
				//don't apply color
			}
			bitmapTemp?.Dispose();//frees up the lock on the file on disk
			//_arrayBitmapsRaw[i].Clone(  //don't ever do it this way. Messes up dpi.
		}

		///<summary>Displays the PDF in an ODWebView2 control. Downloads the PDF file from the cloud if necessary.</summary>
		private async void LoadPdf(string atoZFolder,string atoZFileName,string localPath,string downloadMessage) {
			try {
				if(_odWebView2!=null) {
					_odWebView2.Visible=true;
				}
				SetPdfFilePath(atoZFolder,atoZFileName,localPath,downloadMessage);
				if(!File.Exists(_odWebView2FilePath)) {
					_odWebView2FilePath="";
					MessageBox.Show(Lan.g(this,"File not found")+": " + atoZFileName);
				}
				else {
					if(ODBuild.IsWeb()) {
						_cloudIframe.ShowIframe();
						_cloudIframe.DisplayFile(_odWebView2FilePath);
						IsImageFloatLocked=true;
						return;
					}
					if(_odWebView2.CoreWebView2==null) {
						await _odWebView2.Init();//Throws exception if Microsoft WebView2 Runtime is not installed so need to have in try-catch.
					}
					_odWebView2.ODWebView2Navigate(_odWebView2FilePath);
					IsImageFloatLocked=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				//An exception can happen if they do not have Microsoft WebView2 Runtime installed.
			}
		}

		private void SetPdfFilePath(string atoZFolder,string atoZFileName,string localPath,string downloadMessage) {
			string pdfFilePath="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				pdfFilePath=ODFileUtils.CombinePaths(atoZFolder,atoZFileName);
			}
			else if(CloudStorage.IsCloudStorage) {
				if(localPath!="") {
					pdfFilePath=localPath;
				}
				else {
					//Download PDF into temp directory for displaying.
					pdfFilePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),GetDocumentShowing(0).DocNum+(PatientCur!=null ? PatientCur.PatNum.ToString() : "")+".pdf");
					//Check to see if the PDF already exists in the temp directory and if it's already in use (likely loaded into the webview already).
					if(!ODFileUtils.IsFileInUse(pdfFilePath)) {
						FileAtoZ.Download(FileAtoZ.CombinePaths(atoZFolder,atoZFileName),pdfFilePath,downloadMessage);
					}
				}
			}
			else {
				pdfFilePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),GetDocumentShowing(0).DocNum+(PatientCur!=null ? PatientCur.PatNum.ToString() : "")+".pdf");
				//Check to see if the PDF already exists in the temp directory and if it's already in use (likely loaded into the webview already).
				if(!ODFileUtils.IsFileInUse(pdfFilePath)) {
					File.WriteAllBytes(pdfFilePath,Convert.FromBase64String(GetDocumentShowing(0).RawBase64));
				}
			}
			_odWebView2FilePath=pdfFilePath;
		}

		///<summary>>Specify 0 when a single document is selected, or specify the idx within a mount.  Sets the element in _arrayBitmapsShowing.</summary>
		private void SetBitmapShowing(int idx,Bitmap bitmap){
			if(IsMountShowing()){
				//should be correct length
			}
			else{
				//this covers documents as well as nothing showing
				if(_bitmapArrayShowing==null || _bitmapArrayShowing.Length!=1){
					_bitmapArrayShowing=new Bitmap[1];
				}
			}
			_bitmapArrayShowing[idx]=bitmap;
		}

		///<summary>Only when clicking on Line Measure in the toolbar.</summary>
		private void ToolBarMeasure_Click(){
			bool hasLine=false;
			bool hasScale=false;
			for(int j=0;j<_listImageDraws.Count;j++){
				if(_listImageDraws[j].DrawType==ImageDrawType.Line){
					hasLine=true;
				}
				if(_listImageDraws[j].DrawType==ImageDrawType.ScaleValue){
					hasScale=true;
				}
			}
			if(!hasLine){
				MsgBox.Show(this,"Draw a line before using measure.");
				EventSetDrawMode(this,EnumDrawMode.Line);
				return;
			}
			if(!hasScale){
				if(IsBitmapShowing()){
					//for mounts, there's no way to add default scale here. It was done when mount was created.
					if(PrefC.GetString(PrefName.ImagingDefaultScaleValue)!=""){
						ImageDraw imageDraw=new ImageDraw();
						imageDraw.DrawType=ImageDrawType.ScaleValue;
						imageDraw.DocNum=GetDocumentShowing(0).DocNum;
						imageDraw.DrawingSegment=PrefC.GetString(PrefName.ImagingDefaultScaleValue);
						ImageDraws.Insert(imageDraw);
						_listImageDraws.Add(imageDraw);
						return;
					}
				}
				MsgBox.Show(this,"A scale must be set before using measure.");
				EventSetDrawMode(this,EnumDrawMode.Line);
				return;
			}
		}

		///<summary>Only when clicking on the button in the toolbar.  The window is separately shown when clicking on a line to set scale.</summary>
		private void ToolBarSetScale_Click(){
			using FormImageScale formImageScale=new FormImageScale();
			//see if we have an existing scale
			ImageDraw imageDraw=null;
			for(int j=0;j<_listImageDraws.Count;j++){
				if(_listImageDraws[j].DrawType==ImageDrawType.ScaleValue){
					imageDraw=_listImageDraws[j];
					break;
				}
			}
			if(IsBitmapShowing()){
				if(imageDraw is null){
					//this works even if no default set
					formImageScale.StringUnits=MountDefs.GetScaleUnits(PrefC.GetString(PrefName.ImagingDefaultScaleValue));
					formImageScale.ScaleVal=MountDefs.GetScale(PrefC.GetString(PrefName.ImagingDefaultScaleValue));
					formImageScale.Decimals=MountDefs.GetDecimals(PrefC.GetString(PrefName.ImagingDefaultScaleValue));
				}
				else{
					formImageScale.ScaleVal=imageDraw.GetScale();
					formImageScale.Decimals=imageDraw.GetDecimals();
					formImageScale.StringUnits=imageDraw.GetScaleUnits();
				}
			}
			if(IsMountShowing()){
				//If the mountDef had a ScaleValue, then it will already have created an ImageDraw when mount created.
				//There's no way to create a default on the fly unless we add a column to mount.
				if(imageDraw!=null){
					formImageScale.ScaleVal=imageDraw.GetScale();
					formImageScale.Decimals=imageDraw.GetDecimals();
					formImageScale.StringUnits=imageDraw.GetScaleUnits();
				}	
			}
			formImageScale.ShowDialog();
			if(formImageScale.DialogResult!=DialogResult.OK){
				return;
			}
			if(imageDraw is null){
				//make a new one
				imageDraw=new ImageDraw();
				imageDraw.DrawType=ImageDrawType.ScaleValue;
				if(IsBitmapShowing()){
					imageDraw.DocNum=GetDocumentShowing(0).DocNum;
				}
				if(IsMountShowing()){
					imageDraw.MountNum=_mountShowing.MountNum;
				}
				imageDraw.SetScale(formImageScale.ScaleVal,formImageScale.Decimals,formImageScale.StringUnits);
				ImageDraws.Insert(imageDraw);
				_listImageDraws.Add(imageDraw);
			}
			else{//edit the existing
				imageDraw.SetScale(formImageScale.ScaleVal,formImageScale.Decimals,formImageScale.StringUnits);
				ImageDraws.Update(imageDraw);
			}
		}

		private bool IsShiftDown() {
			if(Control.ModifierKeys==Keys.Shift){
				return true;
			}
			return false;
		}

		private void ThumbnailRefresh(){
			if(IsMountShowing()){
				int idxSelected=_idxSelectedInMount;
				_idxSelectedInMount=-1;//so that yellow outline will not show
				CreateMountThumbnail();
				_idxSelectedInMount=idxSelected;
				EventThumbnailNeedsRefresh?.Invoke(this,new EventArgs());
			}
			if(IsDocumentShowing()){
				EventThumbnailNeedsRefresh?.Invoke(this,new EventArgs());
			}
		}

		private void ToolBarExport_ClickWeb(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(EnumPermType.ImageExport,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),GetDocumentShowing(0).FileName);
				string docPath=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),GetDocumentShowing(0).FileName);
				FileAtoZ.Copy(docPath,tempFilePath,FileAtoZSourceDestination.AtoZToLocal,"Exporting file...",doOverwrite:true);
				ThinfinityUtils.ExportForDownload(tempFilePath);
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Exported: "+GetDocumentShowing(0).FileName+" with category "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(EnumPermType.ImageExport,PatientCur.PatNum,logText,GetDocumentShowing(0).DocNum,GetDocumentShowing(0).DateTStamp);
				return;
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(EnumPermType.ImageExport,_documentArrayShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),_documentArrayShowing[_idxSelectedInMount].FileName);
				string docPath=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),_documentArrayShowing[_idxSelectedInMount].FileName);
				FileAtoZ.Copy(docPath,tempFilePath,FileAtoZSourceDestination.AtoZToLocal,"Exporting file...",doOverwrite:true);
				ThinfinityUtils.ExportForDownload(tempFilePath);
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_documentArrayShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Exported: "+_documentArrayShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with category "+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(EnumPermType.ImageExport,PatientCur.PatNum,logText,_documentArrayShowing[_idxSelectedInMount].DocNum,
					_documentArrayShowing[_idxSelectedInMount].DateTStamp);
				return;
			}
			if(IsMountShowing()){
				if(!Security.IsAuthorized(EnumPermType.ImageExport,_mountShowing.DateCreated)) {
					return;
				}
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),"mount.jpg");
				Bitmap bitmapExport=new Bitmap(_mountShowing.Width,_mountShowing.Height);
				Graphics g=Graphics.FromImage(bitmapExport);
				g.TranslateTransform(bitmapExport.Width/2,bitmapExport.Height/2);//Center of image
				DrawMount(g);
				g.Dispose();
				bitmapExport.Save(tempFilePath);
				ThinfinityUtils.ExportForDownload(tempFilePath);
				bitmapExport.Dispose();
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_mountShowing.DocCategory);
				string logText="Mount Exported: "+_mountShowing.Description+" with category "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(EnumPermType.ImageExport,PatientCur.PatNum,logText);
				return;
			}
		}

		///<summary>Supports multiple file imports, and user doesn't actually need to select a mount item first.</summary>
		private void ToolBarImportMount(){
			OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=true;
			if(Prefs.GetContainsKey(nameof(PrefName.UseAlternateOpenFileDialogWindow)) && PrefC.GetBool(PrefName.UseAlternateOpenFileDialogWindow)){//Hidden pref, almost always false.
				//We don't know why this makes any difference but people have mentioned this will stop some hanging issues.
				//https://stackoverflow.com/questions/6718148/windows-forms-gui-hangs-when-calling-openfiledialog-showdialog
				openFileDialog.ShowHelp=true;
			}
			openFileDialog.InitialDirectory=PrefC.GetString(PrefName.DefaultImageImportFolder);
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string[] stringArrayFileNames=openFileDialog.FileNames;
			if(stringArrayFileNames.Length<1) {
				return;
			}
			List<MountItem> listMountItemsAvail=GetAvailSlots(stringArrayFileNames.Length);
			if(listMountItemsAvail==null){
				MsgBox.Show("Not enough slots in the mount for the number of files selected.");
				return;
			}
			Document document=null;
			for(int i=0;i<stringArrayFileNames.Length;i++) {
				try {
					//.FileName is full path
					if(CloudStorage.IsCloudStorage) {
						//this will flicker because multiple progress bars.  Improve later.
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => document=ImageStore.Import(stringArrayFileNames[i],GetCurrentCategory(),PatientCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						document=ImageStore.Import(stringArrayFileNames[i],GetCurrentCategory(),PatientCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+stringArrayFileNames[i]);
					continue;
				}
				Document documentOld=document.Copy();
				document.MountItemNum=listMountItemsAvail[i].MountItemNum;
				document.ToothNumbers=listMountItemsAvail[i].ToothNumbers;
				Documents.Update(document,documentOld);
				//this is all far too complicated:
				//_arrayDocumentsShowing[_idxSelectedInMount]=doc.Copy();
				//_arrayBitmapsRaw[_idxSelectedInMount]=ImageStore.OpenImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder,openFileDialog.FileName);
				//_arrayBitmapsShowing[_idxSelectedInMount]=ImageHelper.ApplyDocumentSettingsToImage2(
				//	_arrayDocumentsShowing[_idxSelectedInMount],_arrayBitmapsRaw[_idxSelectedInMount], ImageSettingFlags.CROP | ImageSettingFlags.COLORFUNCTION);
			}
			EventSelectTreeNode?.Invoke(this,null);
			if(stringArrayFileNames.Length==1){
				_idxSelectedInMount=listMountItemsAvail[0].ItemOrder-1;//-1 is to account for 1-indexed vs 0-indexed.
				panelMain.Invalidate();
			}
		}

		///<summary>Not importing to mount.  Still supports multiple imports at once.</summary>
		private void ToolBarImportSingle() {
			if(Plugins.HookMethod(this,"ContrImages.ToolBarImport_Click_Start",PatientCur)) {//Named differently for backwards compatibility
				EventFillTree?.Invoke(this,true);
				return;
			}
			OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=true;
			if(Prefs.GetContainsKey(nameof(PrefName.UseAlternateOpenFileDialogWindow)) && PrefC.GetBool(PrefName.UseAlternateOpenFileDialogWindow)){//Hidden pref, almost always false.
				//We don't know why this makes any difference but people have mentioned this will stop some hanging issues.
				//https://stackoverflow.com/questions/6718148/windows-forms-gui-hangs-when-calling-openfiledialog-showdialog
				openFileDialog.ShowHelp=true;
			}
			openFileDialog.InitialDirectory=PrefC.GetString(PrefName.DefaultImageImportFolder);
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			string[] stringArrayFileNames=openFileDialog.FileNames;
			if(stringArrayFileNames.Length<1) {
				return;
			}
			NodeTypeAndKey nodeTypeAndKey=null;
			Document document=null;
			for(int i=0;i<stringArrayFileNames.Length;i++) {
				try {
					if(CloudStorage.IsCloudStorage) {
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => document=ImageStore.Import(stringArrayFileNames[i],GetCurrentCategory(),PatientCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						document=ImageStore.Import(stringArrayFileNames[i],GetCurrentCategory(),PatientCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+openFileDialog.FileName);
					continue;
				}
				EventFillTree?.Invoke(this,false);
				EventSelectTreeNode?.Invoke(this,new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,document,isDocCreate:true);
				formDocInfo.ShowDialog(this);//some of the fields might get changed, but not the filename
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,document);
				}
				else {
					if(document.ImgType==ImageType.Photo) {
						PatientEvent.Fire(ODEventType.Patient,PatientCur);//Possibly updated the patient picture.
					}
					nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum);
					SetDocumentShowing(0,document.Copy());
					//these two lines are needed in case they change the category
					EventFillTree?.Invoke(this,true);
					EventSelectTreeNode?.Invoke(this,new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				}
			}
			/*todo:
			if(treeMain.SelectedNode!=null) {
				treeMain.SelectedNode.EnsureVisible();
			}*/
		}

		#endregion Methods - Private

		#region Class - Nested
		private class NodeTypeKeyCat{
			public EnumImageNodeType ImageNodeType;
			public long PriKey;
			public long DefNumCategory;
		}

		#endregion Class - Nested

		private void FormImageFloat_LocationChanged(object sender, EventArgs e)
		{
			//for testing
			Rectangle rectangleDTB=this.DesktopBounds;
			Rectangle rectangleB=this.Bounds;
		}
	}

	///<summary></summary>
	public class WindowingEventArgs{
		public int MinVal;
		public int MaxVal;
	}

	

}

//2021-10-01-todo:
//(done) Hide device type because XDR is not functional
//Noticed small existing bug in previous versions: When delete an image, it hightlights the one above it, but doesn't show it.

//Todo later, minor issues:
//Image should not jump when starting acquire, but too hard to fix right now.
//Images capture, get rid of stop button?




/*
Old ideas:

//Tabs. Abandoned this idea
//Clicking a new image reuses the current tab, just like Chrome.
//Icon on tab to pin?  Pinned tab won't be reused.
//Maybe no way to unpin.  So there's always one unpinned reusable tab.
//Another icon on tab to close.
//Only a single row of tabs, and they can start to squish if too many.
//Tile could be added later.
//Grab and drag to make floating.
//Create a single dropdown in the lower toolbar called "Windows".  It will have a list of open windows.
//Right click on any tab or window for actions: Pin, Close, Close All But This, 

DockState: abandoned this idea
//Create a single dropdown in the titlebar of each window. It will have a list of options followed by a list of open windows.
//Actions would be Hide Others, Dock This, Cascade All, Tile All, Close Others
//What do we do when cascade and tile are covering up Fill, and we select a new image? 
///<summary>Determines how windows get moved, resized, and hidden.</summary>
	public enum EnumImageDockState{
		///<summary>Shows in taskbar (none of the others do), and does not get automatically moved, resized, or hidden. To create a floater, drag a window from some other state.</summary>
		Float,
		///<summary>Keeps window exactly sized to the available space.  Hides automatically.  Shows in UI as Dock.  There can be multiple Fills, although this would be unusual because the user would have had to click "Dock" on a non-docked window. This is the only state that can accept new images when user clicks in the tree at the left, and if there are multiple, then the top one receives the image. New image replaces existing image, but not for any other state.  If user clicks new image, but no Fill is present, then it creates a new one.</summary>
		Fill,
		///<summary>Keeps the position synched with OD, changes the size to stay within OD by anchoring right and bottom, and hides automatically.  Tabbed would look more modern but would take too long to program.</summary>
		Cascade,
		///<summary>Does extensive math to keep the windows tiled at all times, and hides automatically.</summary>
		Tile
	}*/
