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

namespace OpenDental {
	public partial class FormImageFloat:FormODBase {
		/*
		FormODBase.IsImageFloatDocked is where the docked status gets flagged and tracked.  If docked, we keep the window exactly sized to the available space.  Hides automatically.  There can only be either zero or one window docked, and it's always in the 0 position in the list if present.  The Dock button will not work if a previous window is already docked. This is the only window that can accept new images when user clicks in the tree at the left. New image replaces existing image, but only if docked.  If user clicks new image, but no dock is present, then it creates a new one.  If a window is not docked, then it shows in taskbar and does not get automatically moved, resized, or hidden.  To undock, drag the window.
		*/
		#region Fields - Public
		///<summary>The color of the button text in ControlImagesJ. Only used for text.</summary>
		public Color ColorDrawText;
		///<summary>The color of the button background in ControlImagesJ. Used for text background and also for drawing color.</summary>
		public Color ColorDrawBackground;
		public Patient PatientCur=null;
		///<summary></summary>
		public string PatFolder="";
		///<summary>For a few things, an actual reference to the imageSelector of the parent is handy.</summary>
		public ImageSelector ImageSelector_;
		///<summary>Just used to get the list of windows.</summary>
		public List<FormImageFloat> ListFormImageFloats;
		///<summary>The currently selected node type, key, and category.  This is specific to each window.  Can be null if nothing selected, which will cause pasted items to go to first category.</summary>
		private NodeTypeKeyCat _nodeTypeKeyCatSelected;
		#endregion Fields - Public
		
		#region Fields - Private
		///<summary>Typically just one bitmap at idx 0.  Mount image might be scaled.  Windowing (brightness/contrast) has already been applied. Does not have applied crop, flip, rotation, mount translation, global translation, or zoom.  If it's a mount, there is one for each mount position, and some can be null.</summary>
		private Bitmap[] _arrayBitmapsShowing;
		///<summary>Typically just one document at idx 0.  For mounts, there is one for each mount position, which all start out null.</summary>
		private Document[] _arrayDocumentsShowing;
		///<summary>If any bitmap gets resized for a mount, this is how we keep track of the original size.  Image proportion is the same.  Otherwise, it will just contain the same size as bitmapShowing.  Only used for mounts.</summary>
		private Size[] _arraySizesOriginal;
		///<summary>This contains the raw image data for a single selected doc or mount item.  Used for Dicom instead of the usual _bitmapRaw.  Can be null.</summary>
		private BitmapDicom _bitmapDicomRaw;
		///<summary>Only used when a single doc or mount item is selected.  This is the raw image that is the basis for BrightContrast adjustments.  Mount image might be scaled but it won't have any color adj.  If scaled, it's the same ratio as original.  We use width to calc scale, since height could be off by a partial pixel.  Does not have applied crop, flip, rotation, etc.  If the current selected bitmap is a Dicom image, then see _arrayDicomRaw instead.</summary>
		private Bitmap _bitmapRaw;
		///<summary>Tracker to keep track of how many pages are loaded on our webBrowser object.</summary>
		private int _countWebBrowserLoads=0;
		///<summary></summary>
		private EnumCropPanAdj _cropPanAdj;
		///<summary>Looks like an open hand.</summary>
		private Cursor _cursorPan;
		private DateTime _dateTimeMouseDownPanel;
		private EnumDrawMode _drawMode;
		///<summary>Dispose handled automatically when this form closes.</summary>
		private FormImageFloatWindows _formImageFloatWindows;
		///<summary>The index of the currently selected item within a mount. 0-indexed.  Frequently -1 to indicate no selection.</summary>
		private int _idxSelectedInMount=-1;
		///<summary>Will be -1 if no text drawing object is selected.</summary>
		private int _idxTextSelected=-1;
		///<summary>True when the menu is showing.</summary>
		private bool _isButWindowPressed;
		private bool _isDraggingMount=false;
		///<summary></summary>
		private bool _isExportable;
		///<summary>True if the mouse is currently over the "Windows" button at the top.</summary>
		private bool _isHotButWindow;
		private bool _isMouseDownPanel;
		///<summary>All the ImageDraw rows for this document or mount.</summary>
		private List<ImageDraw> _listImageDraws;
		///<summary>If a mount is currently selected, this is the list of the mount items on it.</summary>
		private List<MountItem> _listMountItems=null;
		///<summary>Keeps track of the currently selected mount object (only when a mount is selected).</summary>
		private Mount _mountShowing=null;
		///<summary>When dragging mount bitmap, this is the current point of the bitmap, in mount coordinates.</summary>
		private Point _pointDragNow;
		///<summary>When dragging mount item, this is the starting point of the center of the mount item where raw image will draw, in mount coordinates.</summary>
		private Point _pointDragStart;
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
		///<summary>Displays PDFs.</summary>
		private WebBrowser _webBrowser=null;
		///<summary>The location of the file that <see cref="_webBrowser" /> has navigated to.</summary>
		private string _webBrowserFilePath=null;
		///<summary>This is needed as we switch betwen windows, in order to set the zoomSlider.</summary>
		private ZoomSliderState _zoomSliderStateInitial;
		///<summary>Property backer.</summary>
		private int _zoomSliderValue;
		#endregion Fields - Private

		#region Constructor
		public FormImageFloat() {
			InitializeComponent();
			InitializeLayoutManager();
			LayoutManager.ZoomChanged+=LayoutManager_ZoomChanged;
			Lan.F(this);
			_cursorPan=new Cursor(GetType(),"CursorPalm.cur");
			panelMain.Cursor=_cursorPan;
			panelMain.MouseWheel += panelMain_MouseWheel;
			panelMain.ContextMenu=menuPanelMain;
			_webBrowser=new WebBrowser();//Include a webBrowser object for loading .pdf files.
			_webBrowser.Visible=false;
			//_webBrowser.Bounds=panelMain.Bounds;
			_webBrowser.Dock=DockStyle.Fill;
			LayoutManager.Add(_webBrowser,this);
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

		///<summary>This event bubbles up to the parent, which then sets the property here.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EnumCropPanAdj> EventSetCropPanEditAdj=null;

		///<summary>This event is fired from SetWindowingSlider, which makes the decisions about the windowing for what's showing.  The event bubbles up to the parent to set windowingSlider min and max.  But there is no property here to affect.  Separately, the parent can call SetWindowingSlider, which will then also fire this event like normal. Finally, in windowingSlider_Scroll, the parent directly sets min and max on the document within this window.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<WindowingEventArgs> EventSetWindowingSlider=null;

		///<summary>This event bubbles up to the parent, which then sets the property here.  This sets the zoom slider so that the middle tick will be at the "fit" zoom.  This is needed any time the image is rotate or cropped, or if the form size changes.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<ZoomSliderState> EventSetZoomSlider=null;

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
		///<summary>Disposes and recreates a new webBrowser control that is hidden.</summary>
		public void ClearPDFBrowser() {
			if(_webBrowser!=null) {
				_webBrowser.Dispose();
				_webBrowser=null;//In order to release the pdf lock on this object, we have to dispose of the webBrowser.
				_webBrowser=new WebBrowser();
				_webBrowser.Bounds=panelMain.Bounds;
				LayoutManager.Add(_webBrowser,this);
				_webBrowser.Visible=false;
				IsImageFloatLocked=false;
			}
		}

		///<summary>Deletes the specified document from the database and refreshes the tree view. Set securityCheck false when creating a new document that might get cancelled.  Document is passed in because it might not be in the tree if the image folder it belongs to is now hidden.</summary>
		public void DeleteDocument(bool isVerbose,bool doSecurityCheck,Document document) {
			if(doSecurityCheck) {
				if(!Security.IsAuthorized(Permissions.ImageDelete,document.DateCreated)) {
					return;
				}
			}
			EhrLab lab=EhrLabImages.GetFirstLabForDocNum(document.DocNum);
			if(lab!=null) {
				string dateSt=lab.ObservationDateTimeStart.PadRight(8,'0').Substring(0,8);//stored in DB as yyyyMMddhhmmss-zzzz
				DateTime dateT=PIn.Date(dateSt.Substring(4,2)+"/"+dateSt.Substring(6,2)+"/"+dateSt.Substring(0,4));
				MessageBox.Show(Lan.g(this,"This image is attached to a lab order for this patient on "+dateT.ToShortDateString()+". "+Lan.g(this,"Detach image from this lab order before deleting the image.")));
				return;
			}
			//EnableAllToolBarButtons(false);
			if(isVerbose) {
				if(IsMountItemSelected()){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete item from within the mount?")) {
						return;
					}
				}
				else if(ListTools.In(document.ImgType,ImageType.Document,ImageType.File)){
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
			Document[] docs=new Document[1] { document };	
			try {
				ImageStore.DeleteDocuments(docs,PatFolder);
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
				_arrayDocumentsShowing[_idxSelectedInMount]=null;
				_arrayBitmapsShowing[_idxSelectedInMount]=null;
				panelMain.Invalidate();
			}
		}

		public void DocumentAcquiredForMount(Document document,Bitmap bitmap,BitmapDicom bitmapDicom){
			_bitmapDicomRaw=bitmapDicom;
			Document docOld=document.Copy();
			document.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
			document.DegreesRotated=_listMountItems[_idxSelectedInMount].RotateOnAcquire;
			document.ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
			Documents.Update(document,docOld);
			//The following lines are from LoadBitmap(OnlyIdx), but optimized for the situation when we already have the bitmap
			SetDocumentShowing(_idxSelectedInMount,document);
			_arraySizesOriginal[_idxSelectedInMount]=bitmap.Size;
			double scale=ZoomSlider.CalcScaleFit(new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
				bitmap.Size,GetDocumentShowing(_idxSelectedInMount).DegreesRotated);
			SetBitmapShowing(_idxSelectedInMount,new Bitmap(bitmap,new Size((int)(bitmap.Width*scale),(int)(bitmap.Height*scale))));
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
				if(_webBrowser!=null && _webBrowser.Visible && ODBuild.IsWeb()) {
					doShowPrint=true;
				}
				toolBarButtonState=new ToolBarButtonState(print:doShowPrint, delete:true, info:true, sign:true, export:_isExportable, copy:panelMain.Visible, brightAndContrast:panelMain.Visible, zoom:panelMain.Visible, zoomOne:false, crop:panelMain.Visible, pan:panelMain.Visible, adj:false, size:panelMain.Visible, flip:panelMain.Visible, rotateL:panelMain.Visible, rotateR:panelMain.Visible, rotate180:panelMain.Visible,draw:panelMain.Visible);
				EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
			}
			else if(IsMountShowing()){
				if(_idxSelectedInMount==-1){//entire mount
					toolBarButtonState=new ToolBarButtonState(print:true, delete:true, info:true, sign:false, export:true, copy:true, brightAndContrast:false, zoom:true, zoomOne:false, crop:false, pan:true, adj:true, size:false, flip:false, rotateL:false, rotateR:false, rotate180:false,draw:true);
					EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
				}
				else{//individual image
					toolBarButtonState=new ToolBarButtonState(print:true, delete:true, info:true, sign:false, export:true, copy:true, brightAndContrast:true, zoom:true, zoomOne:true, crop:false, pan:true, adj:true, size:true, flip:true, rotateL:true, rotateR:true, rotate180:true,draw:true);
					EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
				}
			}
			else{
				//none or category
				//All false.  This is also done in ControlImagesJ.DisableAllToolBarButtons() when module is selected.
				toolBarButtonState=new ToolBarButtonState(print:false, delete:false, info:false, sign:false, export:false, copy:false, brightAndContrast:false, zoom:false, zoomOne:false, crop:false, pan:true, adj:false, size:false, flip:false, rotateL:false, rotateR:false, rotate180:false, draw:false);
				EventEnableToolBarButtons?.Invoke(this,toolBarButtonState);
			}
		}

		public void formVideo_BitmapCaptured(object sender, VideoEventArgs e){
			Document doc = null;
			try {
				//it seems to sometimes fire a second time, with bitmap null
				if(e.Bitmap is null){
					return;
				}
				doc=ImageStore.Import(e.Bitmap,GetCurrentCategory(),ImageType.Photo,PatientCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save")+": "+ex.Message);
				return;
			}
			if(!IsMountShowing()){//single
				EventFillTree?.Invoke(this,false);//Reload and keep new document selected.
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
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
			Document docOld=doc.Copy();
			doc.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
			doc.DegreesRotated=_listMountItems[_idxSelectedInMount].RotateOnAcquire;
			doc.ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
			Documents.Update(doc,docOld);
			//The following lines are from LoadBitmap(OnlyIdx), but optimized for the situation when we already have the bitmap
			_arrayDocumentsShowing[_idxSelectedInMount]=doc;
			//Bitmap bitmap=(Bitmap)Bitmap.FromFile(fileName);
			_arraySizesOriginal[_idxSelectedInMount]=e.Bitmap.Size;
			double scale=ZoomSlider.CalcScaleFit(new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
				e.Bitmap.Size,_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated);
			SetBitmapShowing(_idxSelectedInMount,new Bitmap(e.Bitmap,new Size((int)(e.Bitmap.Width*scale),(int)(e.Bitmap.Height*scale))));
			//_bitmapRaw: don't load this because it would waste time.  Instead, deselect after loop.
			ImageHelper.ApplyColorSettings(GetBitmapShowing(_idxSelectedInMount),
				_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax);
			e.Bitmap?.Dispose();
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listAvail=GetAvailSlots(1);
			if(listAvail is null){//no more available slots, so we are done
				_idxSelectedInMount=-1;
				panelMain.Invalidate();
				return;
			}
			MountItem mountItem=_listMountItems.FirstOrDefault(x=>x.MountItemNum==listAvail[0].MountItemNum);
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
			List<MountItem> listAvail=new List<MountItem>();
			int idxItem=_idxSelectedInMount;
			if(idxItem==-1){
				idxItem=0;
			}
			//idxItem could be in the middle of _listMountItems.  
			while(idxItem<_listMountItems.Count){
				if(_arrayDocumentsShowing[idxItem]!=null){
					//occupied
					idxItem++;
					continue;
				}
				listAvail.Add(_listMountItems[idxItem]);
				idxItem++;
			}
			if(_idxSelectedInMount>0){//Second loop to catch items lower than the first loop
				idxItem=0;
				while(idxItem<_idxSelectedInMount){
					if(_arrayDocumentsShowing[idxItem]!=null){
						idxItem++;
						continue;
					}
					listAvail.Add(_listMountItems[idxItem]);
					idxItem++;
				}
			}
			if(countNeed!=-1 && listAvail.Count<countNeed){
				return null;
			}
			return listAvail;
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayBitmapsShowing. Can return null.</summary>
		public Bitmap GetBitmapShowing(int idx) {
			if(_arrayBitmapsShowing==null){
				return null;
			}
			if(idx>_arrayBitmapsShowing.Length-1){
				return null;
			}
			return _arrayBitmapsShowing[idx];
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayDocumentsShowing. Can return null.</summary>
		public Document GetDocumentShowing(int idx) {
			if(_arrayDocumentsShowing==null){
				return null;
			}
			if(idx>_arrayDocumentsShowing.Length-1){
				return null;
			}
			return _arrayDocumentsShowing[idx];
		}

		public int GetIdxSelectedInMount(){
			return _idxSelectedInMount;
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
			return new NodeTypeAndKey(_nodeTypeKeyCatSelected.NodeType,_nodeTypeKeyCatSelected.PriKey);
		}

		///<summary></summary>
		public EnumImageNodeType GetSelectedType(){
			if(_nodeTypeKeyCatSelected==null){
				return EnumImageNodeType.None;
			}
			return _nodeTypeKeyCatSelected.NodeType;
		}

		///<summary></summary>
		public long GetSelectedPriKey(){
			if(_nodeTypeKeyCatSelected==null){
				return 0;
			}
			return _nodeTypeKeyCatSelected.PriKey;
		}

		public void HideWebBrowser(){
			if(_webBrowser==null) {
				return;
			}
			//This releases control of the current PDF. It was not possible to save in an external PDF viewer unless we clicked on another PDF that would release
			//_webBrowser and control a different PDF. Use in conjunction _webBrowser.Visible=false to not allow the user to click anything on the browser just to be safe.
			_webBrowser.Navigate("about:blank");
			//This prevents users from previewing the PDF in OD at the same time they have it open in an external PDF viewer.
			//There was a strange graphical bug that occurred when the PDF was previewed at the same time the PDF was open in the Adobe Acrobat Reader DC
			//if the Adobe "Enable Protected Mode" option was disabled.  The graphical bug caused many ODButtons and ODGrids to disappear even though their
			//Visible flags were set to true.  Somehow the WndProc() for the form which owned these controls was not calling the OnPaint() method.
			//Thus the bug affected many custom drawn controls.
			_webBrowser.Visible=false;
			IsImageFloatLocked=false;
		}

		///<summary>Invalidates the color image setting and recalculates.  This is not on a separate thread.  Instead, it's just designed to run no more than about every 300ms, which completely avoids any lockup.</summary>
		public void InvalidateSettingsColor() {
			if(IsDocumentShowing()){
				//if((imageSettingFlags & ImageSettingFlags.COLORFUNCTION) !=0 || (imageSettingFlags & ImageSettingFlags.CROP) !=0){
				//We don't do any flip, rotate, or crop here
				GetBitmapShowing(0)?.Dispose();
				if(_bitmapRaw!=null){
					SetBitmapShowing(0,ImageHelper.ApplyDocumentSettingsToImage2(GetDocumentShowing(0),_bitmapRaw, ImageSettingFlags.COLORFUNCTION));
				}
				if(_bitmapDicomRaw!=null){
					SetBitmapShowing(0,DicomHelper.ApplyWindowing(_bitmapDicomRaw,GetDocumentShowing(0).WindowingMin,GetDocumentShowing(0).WindowingMax));
				}
				panelMain.Invalidate();
			}
			if(IsMountItemSelected()){
				_arrayBitmapsShowing[_idxSelectedInMount]?.Dispose();
				if(_bitmapRaw!=null){
					_arrayBitmapsShowing[_idxSelectedInMount]=ImageHelper.ApplyDocumentSettingsToImage2(_arrayDocumentsShowing[_idxSelectedInMount],_bitmapRaw, ImageSettingFlags.COLORFUNCTION);
				}
				if(_bitmapDicomRaw!=null){
					Bitmap bitmapTemp=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax);
					_arrayBitmapsShowing[_idxSelectedInMount]=new Bitmap(bitmapTemp,
						new Size((int)(bitmapTemp.Width*_bitmapDicomRaw.Scale),(int)(bitmapTemp.Height*_bitmapDicomRaw.Scale)));
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
			if(_arrayDocumentsShowing[_idxSelectedInMount]==null){
				return false;
			}
			return true;
		}

		public void PdfPrintPreview(){
			if(ODBuild.IsWeb()) {
				if(_webBrowserFilePath.IsNullOrEmpty()) {
					SetPdfFilePath(PatFolder,GetDocumentShowing(0).FileName,"","Downloading Document...");
				}
				if(!File.Exists(_webBrowserFilePath)) {
					MessageBox.Show(Lan.g(this,"File not found")+": " + GetDocumentShowing(0).FileName);
					_webBrowserFilePath="";
					return;
				}
				ThinfinityUtils.HandleFile(_webBrowserFilePath);//This will do a PDF preview. _webBrowserDocument.ShowPrintPreviewDialog() doesn't work.
			}
			else {
				//This line will not work if _webBrowser.ReadyState==Uninitialized.
				_webBrowser.ShowPrintPreviewDialog();
			}
			SecurityLogs.MakeLogEntry(Permissions.Printing,PatientCur.PatNum,"Patient PDF "+GetDocumentShowing(0).FileName+" "+GetDocumentShowing(0).Description+" printed");
		}

		///<summary>If a mount is showing, and if no item is selected, then this will select the first open item. If one is already selected, but it's full, this does not check that.  There is also no guarantee that one will be selected after this because all positions could be full.</summary>
		public void PreselectFirstItem(){
			if(!IsMountShowing()){
				return;
			}
			if(_idxSelectedInMount!=-1){//already selected
				return;
			}
			for(int i=0;i<_arrayDocumentsShowing.Length;i++){
				if(_arrayDocumentsShowing[i]==null){
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
			Bitmap bitmapPrint=null;
			if(IsDocumentShowing()){
				bitmapPrint=(Bitmap)GetBitmapShowing(0).Clone();
				double ratio2=Math.Min((double)e.MarginBounds.Width/bitmapPrint.Width,(double)e.MarginBounds.Height/bitmapPrint.Height);
				g.DrawImage(bitmapPrint,e.MarginBounds.X,e.MarginBounds.Y,(int)(bitmapPrint.Width*ratio2),(int)(bitmapPrint.Height*ratio2));
				bitmapPrint?.Dispose();
				e.HasMorePages=false;
				return;
			}
			if(!IsMountShowing()){
				e.HasMorePages=false;
				return;
			}
			//mount from here down-------------------------------------------------------------------------------
			using Font fontTitle=new Font(FontFamily.GenericSansSerif,12,FontStyle.Bold);
			using Font fontSubTitles=new Font(FontFamily.GenericSansSerif,10);
			int xTitle;
			int yTitle;
			string str;
			int widthStr;
			if(_idxSelectedInMount>-1 && _arrayDocumentsShowing[_idxSelectedInMount]!=null) {
				//We need to get bitmap from disk because otherwise, the math is too difficult when applying crop.
				//This also makes the image full resolution as a nice side effect.
				Bitmap bitmapTemp=null;
				if(_arrayDocumentsShowing[_idxSelectedInMount].FileName.EndsWith(".dcm")){//dicom
					bitmapTemp=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax);
					bitmapPrint=ImageHelper.ApplyDocumentSettingsToImage2(_arrayDocumentsShowing[_idxSelectedInMount],bitmapTemp,ImageSettingFlags.CROP | ImageSettingFlags.FLIP | ImageSettingFlags.ROTATE);
				}
				else{
					bitmapTemp=ImageStore.OpenImage(_arrayDocumentsShowing[_idxSelectedInMount],PatFolder);
					if(bitmapTemp==null){
						MsgBox.Show("Image not found.");
						e.HasMorePages=false;
						return;
					}
					bitmapPrint=ImageHelper.ApplyDocumentSettingsToImage2(_arrayDocumentsShowing[_idxSelectedInMount],bitmapTemp,ImageSettingFlags.ALL);
				}
				bitmapTemp.Dispose();//frees up the lock on the file on disk
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
				str=_arrayDocumentsShowing[_idxSelectedInMount].DateCreated.ToShortDateString();
				widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
				g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontSubTitles.Height;
				if(_arrayDocumentsShowing[_idxSelectedInMount].ProvNum!=0){
					str=Providers.GetFormalName(_arrayDocumentsShowing[_idxSelectedInMount].ProvNum);
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
				}
				if(_arrayDocumentsShowing[_idxSelectedInMount].Description!=""){
					str=_arrayDocumentsShowing[_idxSelectedInMount].Description;
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
				}
				if(_arrayDocumentsShowing[_idxSelectedInMount].ToothNumbers!=""){
					str=Lan.g(this,"Tooth numbers: ") +_arrayDocumentsShowing[_idxSelectedInMount].ToothNumbers;
					widthStr=(int)g.MeasureString(str,fontSubTitles).Width;
					g.DrawString(str,fontSubTitles,Brushes.Black,xTitle-widthStr/2,yTitle);
					yTitle+=fontSubTitles.Height;
				}
				yTitle+=20;
				Rectangle rectangleAvail=new Rectangle(e.MarginBounds.X,yTitle,e.MarginBounds.Width,e.MarginBounds.Height-yTitle);
				double ratio3=Math.Min((double)rectangleAvail.Width/bitmapPrint.Width,(double)rectangleAvail.Height/bitmapPrint.Height);
				Rectangle rectangleDraw=new Rectangle();
				rectangleDraw.Width=(int)(bitmapPrint.Width*ratio3);
				rectangleDraw.Height=(int)(bitmapPrint.Height*ratio3);
				rectangleDraw.X=rectangleAvail.X+rectangleAvail.Width/2-rectangleDraw.Width/2;//centered
				rectangleDraw.Y=rectangleAvail.Y;
				g.DrawImage(bitmapPrint,rectangleDraw);
				bitmapPrint?.Dispose();
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
				scale=ZoomSlider.CalcScaleFit(new Size(e.MarginBounds.Height,e.MarginBounds.Width-heightTitle),new Size(_mountShowing.Width,_mountShowing.Height),0);
			}
			else{
				scale=ZoomSlider.CalcScaleFit(new Size(e.MarginBounds.Width,e.MarginBounds.Height-heightTitle),new Size(_mountShowing.Width,_mountShowing.Height),0);
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
			ClearObjects();
			_pointTranslation=new Point();
			panelMain.Visible=true;
			if(_webBrowser!=null) {
				_webBrowser.Visible=false;//Clear any previously loaded Acrobat .pdf file.
				IsImageFloatLocked=false;
			}
			if(nodeTypeAndKey is null){
				Text="";
				return;
			}
			_nodeTypeKeyCatSelected=new NodeTypeKeyCat();
			_nodeTypeKeyCatSelected.NodeType=nodeTypeAndKey.NodeType;
			_nodeTypeKeyCatSelected.PriKey=nodeTypeAndKey.PriKey;
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.None){
				Text="";
			}
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Category){
				_nodeTypeKeyCatSelected.DefNumCategory=nodeTypeAndKey.PriKey;
				Text="";
			}
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Document){
				SetDocumentShowing(0,Documents.GetByNum(nodeTypeAndKey.PriKey,doReturnNullIfNotFound:true));
				if(GetDocumentShowing(0)==null) {
					MsgBox.Show(this,"Document was previously deleted.");
					EventFillTree?.Invoke(this,false);
					return;
				}
				_listImageDraws=ImageDraws.RefreshForDoc(GetDocumentShowing(0).DocNum);
				_nodeTypeKeyCatSelected.DefNumCategory=GetDocumentShowing(0).DocCategory;
				Text=GetDocumentShowing(0).DateCreated.ToShortDateString()+": "+GetDocumentShowing(0).Description;
				//no need to show document note in title because it shows as bottom in separate panel.
				if(GetDocumentShowing(0).ToothNumbers!=""){
					if(GetDocumentShowing(0).ToothNumbers.Contains(",")){
						Text+=", "+Lan.g(this,"Teeth")+": ";
					}
					else{
						Text+=", "+Lan.g(this,"Tooth")+": ";
					}
					Text+=Tooth.FormatRangeForDisplay(GetDocumentShowing(0).ToothNumbers);
				}
				_arrayBitmapsShowing=new Bitmap[1]; 
				_arraySizesOriginal=new Size[1];
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
						ListDocuments=_arrayDocumentsShowing.ToList(),
						BitmapImagesModule=_bitmapRaw
					}
				);
				DownloadDocumentNoteFile(nodeTypeAndKey);
				_isExportable=panelMain.Visible;
				if(_bitmapRaw==null) {
					if(ImageHelper.HasImageExtension(GetDocumentShowing(0).FileName)) {
						string srcFileName = ODFileUtils.CombinePaths(PatFolder,GetDocumentShowing(0).FileName);
						if(File.Exists(srcFileName)) {
							MessageBox.Show(Lan.g(this,"File found but cannot be opened, probably because it's too big:")+srcFileName);
						}
						else {
							MessageBox.Show(Lan.g(this,"File not found")+": " + srcFileName);
						}
					}
					else if(Path.GetExtension(GetDocumentShowing(0).FileName).ToLower()==".pdf") {//Adobe acrobat file.
						if(!PrefC.GetBool(PrefName.PdfLaunchWindow)) {
							LoadPdf(PatFolder,GetDocumentShowing(0).FileName,localPathImportedCloud,"Downloading Document...");
						}
					}
				}
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
				_arrayDocumentsShowing=Documents.GetDocumentsForMountItems(_listMountItems);
				_idxSelectedInMount=-1;//No selection to start.
				//_arrayBitmapsRaw=ImageStore.OpenImages(_arrayDocumentsShowing,_patFolder,localPathImportedCloud);
				_arrayBitmapsShowing=new Bitmap[_arrayDocumentsShowing.Length]; 
				_arraySizesOriginal=new Size[_arrayDocumentsShowing.Length];
				List<int> listMissingMountNums=new List<int>();//List to count missing images not found in the A-Z folder
				Cursor=Cursors.WaitCursor;
				for(int i=0;i<_arrayDocumentsShowing.Length;i++){
					if(_arrayDocumentsShowing[i]==null){
						_arrayBitmapsShowing[i]=null;
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
					if(_arrayBitmapsShowing[i]==null) {
						listMissingMountNums.Add(i);
					}
				}
				if(listMissingMountNums.Count>0) {//Notify user of any files that were unable to load
					string errorMessage=Lan.g(this,"Files not found for mount:")+"\r\n";
					for(int m=0;m<listMissingMountNums.Count;m++) {
						errorMessage+=Lan.g(this,"Mount position ")+(listMissingMountNums[m]+1)+": "+_arrayDocumentsShowing[listMissingMountNums[m]].FileName;
						if(m!=listMissingMountNums.Count-1) {
							errorMessage+="\r\n";
						}
					}
					MsgBox.Show(errorMessage);
				}
				EnableToolBarButtons();
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
				if(_arrayDocumentsShowing==null || _arrayDocumentsShowing.Length!=1){
					_arrayDocumentsShowing=new Document[1];
				}
			}
			if(_arrayDocumentsShowing.Length>0 && idx<_arrayDocumentsShowing.Length) {
				_arrayDocumentsShowing[idx]=document;
			}
		}

		///<summary>One of these 6 modes is active at any time.  This allows the value to flow down into this window.</summary>
		public void SetDrawMode(EnumDrawMode drawMode){
			_drawMode=drawMode;
			switch(_drawMode){
				case EnumDrawMode.None:
					_idxTextSelected=-1;
					panelMain.Cursor=_cursorPan;
					break;
				case EnumDrawMode.Text:
					panelMain.Cursor=Cursors.IBeam;
					break;
				case EnumDrawMode.Line:
					panelMain.Cursor=Cursors.Cross;
					break;
				case EnumDrawMode.Pen:
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
			if(_arrayDocumentsShowing is null){
				return;
			}
			if(idx > _arrayDocumentsShowing.Length-1){
				return;
			}
			_idxSelectedInMount=idx;
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
				if(_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax==0) {//default
					windowingEventArgs.MinVal=0;
					windowingEventArgs.MaxVal=255;
				}
				else {
					windowingEventArgs.MinVal=_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin;
					windowingEventArgs.MaxVal=_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax;
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
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,_arrayDocumentsShowing[_idxSelectedInMount].DocNum);
				Bitmap bitmapFromDisk=ImageStore.OpenImage(_arrayDocumentsShowing[_idxSelectedInMount],PatFolder);//returns null if not HasImageExtension
				if(bitmapFromDisk==null){
					//dicom or extremely rare non-image types in the mount item
					fileName=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),_arrayDocumentsShowing[_idxSelectedInMount].FileName);
				}
				else{//valid bitmap
					//we are not going to scale this.  It will be the same size as on disk, except for cropping.
					Bitmap bitmapColored=new Bitmap(bitmapFromDisk);
					bitmapFromDisk.Dispose();//frees up the lock on the file on disk
					ImageHelper.ApplyColorSettings(bitmapColored,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax);
					bitmapCopy=ImageHelper.ApplyDocumentSettingsToImage2(_arrayDocumentsShowing[_idxSelectedInMount],bitmapColored,
						//_arrayBitmapsShowing[_idxSelectedInMount] or _bitmapRaw	//neither of these will work because they are already scaled
						ImageSettingFlags.FLIP | ImageSettingFlags.ROTATE | ImageSettingFlags.CROP);
					bitmapColored.Dispose();
					//file is always copy of image instead of original file, even for dicom.  NodeTypeAndKey still allows true dicom copy/paste.
					fileName=Path.GetTempPath()+_arrayDocumentsShowing[_idxSelectedInMount].FileName;
					bitmapCopy.Save(fileName);
				}
			}
			else if(IsMountShowing()){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum);
				//Bitmap and file are a composite image of the mount for external paste.
				//NodeTypeAndKey still allows for copying the mount itself, along with all attached mount items, if within OD.
				bitmapCopy=new Bitmap(_mountShowing.Width,_mountShowing.Height);
				Graphics g=Graphics.FromImage(bitmapCopy);
				g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);//Center of image
				DrawMount(g);
				g.Dispose();
				fileName=Path.GetTempPath()+PatientCur.LName+PatientCur.FName+".jpg";
				bitmapCopy.Save(fileName);
			}
			else if(IsDocumentShowing()){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,GetDocumentShowing(0).DocNum);
				if(ImageHelper.HasImageExtension(GetDocumentShowing(0).FileName) || GetDocumentShowing(0).FileName.EndsWith(".dcm")){
					//normal images and dicom will have the bitmap and filedrop pulled from the screen
					bitmapCopy=ImageHelper.ApplyDocumentSettingsToImage2(GetDocumentShowing(0),GetBitmapShowing(0),
						ImageSettingFlags.FLIP | ImageSettingFlags.ROTATE | ImageSettingFlags.CROP);
					fileName=Path.GetTempPath()+GetDocumentShowing(0).FileName;
					bitmapCopy.Save(fileName);
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
			DataObject dataObject=new DataObject();
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
			try {
				Clipboard.SetDataObject(dataObject);
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard.  Please try again.");
				ex.DoNothing();
				return;
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
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNum,"Patient image "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" copied to clipboard");
			}
			else if(IsMountShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNum,"Patient mount "+_mountShowing.Description+" copied to clipboard");
			}
			else if(IsDocumentShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNum,"Patient image "+GetDocumentShowing(0).FileName+" copied to clipboard");
			}
		}

		public void ToolBarDelete_Click(){
			if(GetSelectedType()==EnumImageNodeType.None || ImageSelector_.GetSelectedType()==EnumImageNodeType.None) {
				MsgBox.Show(this,"No item is currently selected");
				return;
			}
			//can't get to here if category selected
			NodeTypeAndKey nodeTypeAndKeyAbove=ImageSelector_.GetItemAbove();//always successful
			if(IsDocumentShowing()){
				DeleteDocument(true,true,GetDocumentShowing(0));//security is inside this method
				ImageSelector_.SetSelected(nodeTypeAndKeyAbove.NodeType,nodeTypeAndKeyAbove.PriKey);
				SelectTreeNode(nodeTypeAndKeyAbove);
				return;
			}
			if(IsMountItemSelected()){
				DeleteDocument(true,true,_arrayDocumentsShowing[_idxSelectedInMount]);
				return;
			}
			if(IsMountShowing()){
				bool isEmpty=true;
				for(int i=0;i<_arrayDocumentsShowing.Length;i++){
					if(_arrayDocumentsShowing[i]!=null){
						isEmpty=false;
					}
				}
				if(!isEmpty){
					if(!Security.IsAuthorized(Permissions.ImageDelete,_mountShowing.DateCreated)) {
						return;
					}
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete the entire mount and all images?")){
						return;
					}
					for(int i=0;i<_arrayDocumentsShowing.Length;i++){
						if(_arrayDocumentsShowing[i]!=null){
							Statements.DetachDocFromStatements(_arrayDocumentsShowing[i].DocNum);//overkill
						}
					}
					try {
						ImageStore.DeleteDocuments(_arrayDocumentsShowing,PatFolder);
					}
					catch(Exception ex) {  //Image could not be deleted, in use.
						MessageBox.Show(this,ex.Message);
						return;
					}
				}
				Mounts.Delete(_mountShowing);
				EventFillTree?.Invoke(this,false);
				panelMain.Invalidate();
				ImageSelector_.SetSelected(nodeTypeAndKeyAbove.NodeType,nodeTypeAndKeyAbove.PriKey);
				SelectTreeNode(nodeTypeAndKeyAbove);
			}
		}

		public void ToolBarExport_Click(){
			if(ODBuild.IsWeb()) {
				ToolBarExport_ClickWeb();
				return;
			}
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageExport,GetDocumentShowing(0).DateCreated)) {
					return;
				}		
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export a Document";
				saveFileDialog.FileName=GetDocumentShowing(0).FileName;
				saveFileDialog.DefaultExt=Path.GetExtension(GetDocumentShowing(0).FileName);
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				try {
					ImageStore.Export(saveFileDialog.FileName,GetDocumentShowing(0),PatientCur);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to export file, May be in use")+": " + ex.Message);
					return;
				}
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Exported: "+GetDocumentShowing(0).FileName+" with catgegory "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,PatientCur.PatNum,logText,GetDocumentShowing(0).DocNum,GetDocumentShowing(0).DateTStamp);
				return;
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(Permissions.ImageExport,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}		
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export Image";
				saveFileDialog.FileName=_arrayDocumentsShowing[_idxSelectedInMount].FileName;
				saveFileDialog.DefaultExt=Path.GetExtension(_arrayDocumentsShowing[_idxSelectedInMount].FileName);
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				try {
					ImageStore.Export(saveFileDialog.FileName,_arrayDocumentsShowing[_idxSelectedInMount],PatientCur);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to export file, May be in use")+": " + ex.Message);
					return;
				}
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Exported: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,PatientCur.PatNum,logText,_arrayDocumentsShowing[_idxSelectedInMount].DocNum,
					_arrayDocumentsShowing[_idxSelectedInMount].DateTStamp);
				return;
			}
			if(IsMountShowing()){
				if(!Security.IsAuthorized(Permissions.ImageExport,_mountShowing.DateCreated)) {
					return;
				}
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export Mount";
				saveFileDialog.FileName=".jpg";//_documentShowing.FileName;
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
				string logText="Mount Exported: "+_mountShowing.Description+" with catgegory "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,PatientCur.PatNum,logText);
				return;
			}
		}

		public void ToolBarFlip_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).IsFlipped=!GetDocumentShowing(0).IsFlipped;
				//Document is always flipped and then rotated when drawn, but we want to flip it
				//horizontally no matter what orientation it's in right now
				if(GetDocumentShowing(0).DegreesRotated==90){
					GetDocumentShowing(0).DegreesRotated=270;
				}
				else if(GetDocumentShowing(0).DegreesRotated==270){
					GetDocumentShowing(0).DegreesRotated=90;
				}
				Documents.Update(GetDocumentShowing(0),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with catgegory "
					+defDocCategory.ItemName+" flipped horizontally";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()){ 
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				Document docOld=_arrayDocumentsShowing[_idxSelectedInMount].Copy();
				_arrayDocumentsShowing[_idxSelectedInMount].IsFlipped=!_arrayDocumentsShowing[_idxSelectedInMount].IsFlipped;
				if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated==90){
					_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated=270;
				}
				else if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated==270){
					_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated=90;
				}
				Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],PatFolder);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" flipped horizontally";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		public void ToolBarImport_Click(){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
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
				if(_arrayDocumentsShowing[_idxSelectedInMount]==null){
					return;//silent fail is fine
				}
				using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,_arrayDocumentsShowing[_idxSelectedInMount]);
				formDocInfo.IsMountItem=true;
				formDocInfo.ShowDialog();
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				panelMain.Invalidate();
				return;
			}
			if(IsMountShowing()) {
				using FormMountEdit formMountEdit=new FormMountEdit(_mountShowing);
				formMountEdit.ShowDialog();
				if(formMountEdit.DialogResult!=DialogResult.OK) {
					return;
				}
				Cursor=Cursors.WaitCursor;//because it can take a few seconds to reload.
				EventFillTree?.Invoke(this,true);
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
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			IDataObject iDataObject=null;
			NodeTypeAndKey nodeTypeAndKey=null;
			if(!ODBuild.IsWeb()) {
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
			string[] fileNames=null;
			if(bitmapPaste==null) {//if no bitmap on clipboard, try to get file names
				try {
					fileNames=ODClipboard.GetFileDropList();
				}
				catch(Exception ex) {
					ex.DoNothing();//do nothing here, fileNames should remain null and a message box will show below if necessary
				}
			}
			if(bitmapPaste==null && fileNames.IsNullOrEmpty()){
				MsgBox.Show(this,"No bitmap or file present on clipboard");
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(IsMountShowing()){
				if(bitmapPaste!=null){
					if(_idxSelectedInMount==-1 || _arrayDocumentsShowing[_idxSelectedInMount]!=null){
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
					SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
				}
				else{//fileNames
					//fileDrop supports multiple files, and we don't care if they've selected a mount item.
					List<MountItem> listAvail=GetAvailSlots(fileNames.Length);
					if(listAvail==null){
						MsgBox.Show("Not enough slots in the mount for the number of files selected.");
						Cursor=Cursors.Default;
						return;
					}
					for(int i=0;i<fileNames.Length;i++) {
						Document doc=null;
						try {
							//fileName is full path
							if(CloudStorage.IsCloudStorage) {
								ProgressOD progressOD=new ProgressOD();
								progressOD.ActionMain=() => doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);;
								progressOD.ShowDialogProgress();
								if(progressOD.IsCancelled){
									Cursor=Cursors.Default;
									return;//cleanup?
								}
							}
							else{
								doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);//Makes log
							}
							Document docOld=doc.Copy();
							doc.MountItemNum=listAvail[i].MountItemNum;
							doc.ToothNumbers=listAvail[i].ToothNumbers;
							Documents.Update(doc,docOld);
						}
						catch(Exception ex) {
							Cursor=Cursors.Default;
							MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+fileNames[i]);
							continue;
						}
					}//for					
					Cursor=Cursors.Default;
					SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
				}//end of filenames
				return;
			}//mount
			//Everything below this line is for one or more documents not in a mount=============================================================================================
			//But we cannot test for this. Needs to work for cat selected, nothing selected, existing doc selected, etc.
			Document document=null;
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
				for(int i=0;i<fileNames.Length;i++) {
					try {
						//fileNames contains full paths
						if(CloudStorage.IsCloudStorage) {
							//this will flicker because multiple progress bars.  Improve later.
							ProgressOD progressOD=new ProgressOD();
							progressOD.ActionMain=() => document=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);
							progressOD.ShowDialogProgress();
							if(progressOD.IsCancelled){
								return;
							}
						}
						else{
							document=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);//Makes log
						}
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+fileNames[i]);
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
			SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
		}

		public void ToolBarPasteTypeAndKey(NodeTypeAndKey nodeTypeAndKey){
			//Always single item, not series.
			Document document=null;
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.Document){//pasting from document
				if(IsMountShowing()){//into mount
					if(_idxSelectedInMount==-1 || _arrayDocumentsShowing[_idxSelectedInMount]!=null){
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
				string sourceFile="";
				try {
					sourceFile=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(patientOriginal,ImageStore.GetPreferredAtoZpath()),documentOriginal.FileName);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Cannot paste content."),ex);
					return;
				}
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
				if(IsMountShowing()){
					document.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
				}
				Documents.Update(document);
				if(IsMountShowing()){//into mount
					//FillTree(true);
					SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
					//panelMain.Invalidate();
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
			}
		}

		public void ToolBarRotateL_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).DegreesRotated-=90;
				while(GetDocumentShowing(0).DegreesRotated<0) {
					GetDocumentShowing(0).DegreesRotated+=360;
				}
				Documents.Update(GetDocumentShowing(0),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with catgegory "
					+defDocCategory.ItemName+" rotated left 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}
				Document docOld=_arrayDocumentsShowing[_idxSelectedInMount].Copy();
				if(_arrayDocumentsShowing[_idxSelectedInMount].CropW != 0){
					//find center point of crop area, in image coords
					double xCenter=_arrayDocumentsShowing[_idxSelectedInMount].CropX+_arrayDocumentsShowing[_idxSelectedInMount].CropW/2;
					double yCenter=_arrayDocumentsShowing[_idxSelectedInMount].CropY+_arrayDocumentsShowing[_idxSelectedInMount].CropH/2;
					//ratio changes, causing change to x,y,w,h
					double wNew=_arrayDocumentsShowing[_idxSelectedInMount].CropH;
					double hNew=_arrayDocumentsShowing[_idxSelectedInMount].CropW;
					double xNew=xCenter-wNew/2;
					double yNew=yCenter-hNew/2;
					_arrayDocumentsShowing[_idxSelectedInMount].CropX=(int)xNew;
					_arrayDocumentsShowing[_idxSelectedInMount].CropY=(int)yNew;
					_arrayDocumentsShowing[_idxSelectedInMount].CropW=(int)wNew;
					_arrayDocumentsShowing[_idxSelectedInMount].CropH=(int)hNew;
				}
				_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated-=90;
				while(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated<0) {
					_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated+=360;
				}
				Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],PatFolder);
				//probably not necessary with existing crop because scale hasn't changed.
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" rotated left 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		public void ToolBarRotateR_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).DegreesRotated+=90;
				GetDocumentShowing(0).DegreesRotated%=360;
				Documents.Update(GetDocumentShowing(0),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with catgegory "
					+defDocCategory.ItemName+" rotated right 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				Document docOld=_arrayDocumentsShowing[_idxSelectedInMount].Copy();
				if(_arrayDocumentsShowing[_idxSelectedInMount].CropW != 0){
					//find center point of crop area, in image coords
					double xCenter=_arrayDocumentsShowing[_idxSelectedInMount].CropX+_arrayDocumentsShowing[_idxSelectedInMount].CropW/2;
					double yCenter=_arrayDocumentsShowing[_idxSelectedInMount].CropY+_arrayDocumentsShowing[_idxSelectedInMount].CropH/2;
					//ratio changes, causing change to x,y,w,h
					double wNew=_arrayDocumentsShowing[_idxSelectedInMount].CropH;
					double hNew=_arrayDocumentsShowing[_idxSelectedInMount].CropW;
					double xNew=xCenter-wNew/2;
					double yNew=yCenter-hNew/2;
					_arrayDocumentsShowing[_idxSelectedInMount].CropX=(int)xNew;
					_arrayDocumentsShowing[_idxSelectedInMount].CropY=(int)yNew;
					_arrayDocumentsShowing[_idxSelectedInMount].CropW=(int)wNew;
					_arrayDocumentsShowing[_idxSelectedInMount].CropH=(int)hNew;
				}
				_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated+=90;
				_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated%=360;
				Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],PatFolder);
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" rotated right 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		public void ToolBarRotate180_Click(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing(0).Copy();
				if(GetDocumentShowing(0).DegreesRotated>=180){
					GetDocumentShowing(0).DegreesRotated-=180;
				}
				else{
					GetDocumentShowing(0).DegreesRotated+=180;
				}
				Documents.Update(GetDocumentShowing(0),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Edited: "+GetDocumentShowing(0).FileName+" with catgegory "
					+defDocCategory.ItemName+" rotated 180 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			if(IsMountItemSelected()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				Document docOld=_arrayDocumentsShowing[_idxSelectedInMount].Copy();
				if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated>=180){
					_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated-=180;
				}
				else{
					_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated+=180;
				}
				Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],PatFolder);
				//LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);//no change to scale
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" rotated 180 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		public void ToolBarSize_Click(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[0].DateCreated)) {
					return;
				}	
				if(GetDocumentShowing(0)==null){
					return;
				}
				using FormDocumentSize formDocumentSize=new FormDocumentSize();
				formDocumentSize.DocumentCur=GetDocumentShowing(0);
				formDocumentSize.SizeRaw=_bitmapRaw.Size;
				formDocumentSize.ShowDialog();
				////the form will change DocumentCur and save it
				if(formDocumentSize.DialogResult==DialogResult.OK) {
					EventFillTree?.Invoke(this,true);
					Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[0].DocCategory);
					string logText="Document Edited: "+_arrayDocumentsShowing[0].FileName+" with catgegory "
						+defDocCategory.ItemName+" was adjusted using the Size button";
					SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
				}
				panelMain.Invalidate();
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				if(_arrayDocumentsShowing[_idxSelectedInMount]==null){
					return;
				}
				using FormDocumentSizeMount formDocumentSizeMount=new FormDocumentSizeMount();
				formDocumentSizeMount.DocumentCur=_arrayDocumentsShowing[_idxSelectedInMount];
				formDocumentSizeMount.SizeRaw=_arraySizesOriginal[_idxSelectedInMount];
				formDocumentSizeMount.SizeMount=new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
				formDocumentSizeMount.ShowDialog();
				//the form will change DocumentCur and save it
				if(formDocumentSizeMount.DialogResult==DialogResult.OK){
					LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
					Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
					string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
						+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" was adjusted using the Size button";
					SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
				}
				panelMain.Invalidate();
			}
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
			float newZoom=ZoomSlider.CalcScaleFit(new Size(panelMain.Width,panelMain.Height),
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
			_pointMousePrevious=new Point(0,0);//Gets around the above problem.
			if(_formImageFloatWindows==null || _formImageFloatWindows.IsDisposed){
				_formImageFloatWindows=new FormImageFloatWindows();
				_formImageFloatWindows.ScaleMy=LayoutManager.ScaleMy();
				_formImageFloatWindows.Font=Font;
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
			int widthStr=(int)g.MeasureString(strWindows,Font).Width;
			_rectangleButWindows=new Rectangle(
				x:_rectangleButMin.Left-widthStr-LayoutManager.Scale(21),
				y:MaxInset()+LayoutManager.WidthBorder(),
				width:widthStr+LayoutManager.Scale(13),
				height:LayoutManager.HeightTitleBar());
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
				using SolidBrush brushHover=new SolidBrush(colorButtonHot);
				g.FillRectangle(brushHover,_rectangleButWindows);
			}
			if(_isButWindowPressed){
				g.FillRectangle(Brushes.White,_rectangleButWindows);
				g.DrawRectangle(Pens.Gray,_rectangleButWindows);//the bottom will get cut off
				colorBorderText=Color.Black;
			}
			using SolidBrush brushText=new SolidBrush(colorBorderText);
			g.DrawString("Windows",Font,brushText,new Point(_rectangleButWindows.X+LayoutManager.Scale(6),_rectangleButWindows.Y+LayoutManager.Scale(3)));
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
			Document doc=null;
			for(int i=0;i<stringArrayFiles.Length;i++) {
				try {
					//stringArrayFiles contains full paths
					if(CloudStorage.IsCloudStorage) {
						//this will flicker because multiple progress bars.  Improve later.
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => doc=ImageStore.Import(stringArrayFiles[i],GetCurrentCategory(),PatientCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						doc=ImageStore.Import(stringArrayFiles[i],GetCurrentCategory(),PatientCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+stringArrayFiles[i]);
					continue;
				}
				Document docOld=doc.Copy();
				doc.MountItemNum=listMountItemsAvail[i].MountItemNum;
				doc.ToothNumbers=listMountItemsAvail[i].ToothNumbers;
				Documents.Update(doc,docOld);
			}//for
			SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
		}

		private void panelMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(_drawMode==EnumDrawMode.Text){
				if(_idxTextSelected!=-1){//double click inside existing text
					using FormImageDrawEdit formImageDrawEdit=new FormImageDrawEdit();
					formImageDrawEdit.ImageDrawCur=_listImageDraws[_idxTextSelected];
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
					_idxTextSelected=-1;
					panelMain.Invalidate();
					return;
				}
			}
			//From here down is ordinary double click on an empty spot to bring up the Info window.
			if(IsMountShowing()){
				if(_idxSelectedInMount==-1){
					using FormMountEdit formMountEdit=new FormMountEdit(_mountShowing);
					formMountEdit.ShowDialog();
					if(formMountEdit.DialogResult==DialogResult.OK){
						EventFillTree?.Invoke(this,true);
					}
				}
				else{//mount item
					if(_arrayDocumentsShowing[_idxSelectedInMount]!=null){
						using FormDocInfo formDocInfo=new FormDocInfo(PatientCur,_arrayDocumentsShowing[_idxSelectedInMount]);
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
			if(IsDocumentShowing() && _cropPanAdj==EnumCropPanAdj.Crop) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[0].DateCreated)) {
					return;
				}
			}
			for(int i=0;i<menuPanelMain.MenuItems.Count;i++) {
				//If no document or mount selected, context menu will not be visible
				if(IsDocumentShowing() || IsMountShowing()) {
					menuPanelMain.MenuItems[i].Visible=true;
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
				for(int i=0;i<_listImageDraws.Count;i++){
					if(_listImageDraws[i].DrawType!=ImageDrawType.Text){
						continue;
					}
					PointF pointFText=_listImageDraws[i].GetTextPoint();
					string str=_listImageDraws[i].GetTextString();
					using Font font=new Font(FontFamily.GenericSansSerif,_listImageDraws[i].FontSize);
					int widthText=TextRenderer.MeasureText(str,font).Width;
					Rectangle rectangleText=new Rectangle((int)(pointFText.X),(int)(pointFText.Y),widthText,font.Height);
					if(rectangleText.Contains(pointCursorInBitmap)){//mouse down inside existing text
						_pointTextOriginal=_listImageDraws[i].GetTextPoint();
						_idxTextSelected=i;
						panelMain.Invalidate();
						//mouse down on an existing text item
						//We've already set our variables. Nothing left to do here.
						return;
					}
				}
				//mouse down on an empty spot
				if(_idxTextSelected!=-1){
					//if there was a previously selected text object, just unselect it
					_idxTextSelected=-1;
					panelMain.Invalidate();
					return;
				}
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
				formImageDrawEdit.ImageDrawCur.FontSize=fontSizeApparent/(float)ZoomSliderValue*100f*LayoutManager.ScaleMy();
				formImageDrawEdit.ZoomVal=ZoomSliderValue;
				formImageDrawEdit.ImageDrawCur.ColorDraw=ColorDrawText;
				formImageDrawEdit.ImageDrawCur.ColorBack=ColorDrawBackground;//can be transparent
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
				menuItemPaste.Visible=true;
				menuItemFlipHoriz.Visible=true;
				menuItemRotateLeft.Visible=true;
				menuItemRotateRight.Visible=true;
				menuItemRotate180.Visible=true;
				menuItemDelete.Visible=true;
			}
			//if(_panCropMount==EnumPanCropMount.Pan/Adj){
				//handled on mouse up
			//}
			if(_cropPanAdj==EnumCropPanAdj.Adj){
				if(!Security.IsAuthorized(Permissions.ImageEdit,_mountShowing.DateCreated)) {
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
					if(_arrayDocumentsShowing[_idxSelectedInMount]!=null){
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

		private void panelMain_MouseMove(object sender, MouseEventArgs e){
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
					if(_idxTextSelected!=-1){
						Point pointDownInBitmap=ControlPointToBitmapPoint(_pointMouseDown);
						Point pointNew=new Point(
							x:_pointTextOriginal.X+pointCursorInBitmap.X-pointDownInBitmap.X,
							y:_pointTextOriginal.Y+pointCursorInBitmap.Y-pointDownInBitmap.Y);
						_listImageDraws[_idxTextSelected].SetLoc(pointNew);
						//Will be saved to db on mouse up.
						panelMain.Invalidate();
					}
				}
				else{//not dragging. Just change cursor.
					panelMain.Cursor=Cursors.IBeam;
					for(int i=0;i<_listImageDraws.Count;i++){
						if(_listImageDraws[i].DrawType!=ImageDrawType.Text){
							continue;
						}
						Point pointText=_listImageDraws[i].GetTextPoint();
						string str=_listImageDraws[i].GetTextString();
						using Font font=new Font(FontFamily.GenericSansSerif,_listImageDraws[i].FontSize);
						int widthText=TextRenderer.MeasureText(str,font).Width;
						Rectangle rectangleText=new Rectangle(pointText.X,pointText.Y,widthText,font.Height);
						if(rectangleText.Contains(pointCursorInBitmap)){
							panelMain.Cursor=Cursors.SizeAll;
						}
					}
				}
				return;
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
				if(_idxSelectedInMount==-1 || _arrayDocumentsShowing[_idxSelectedInMount]==null){
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
				if(_isMouseDownPanel) {
					if(_idxTextSelected!=-1){//releasing from a drag, click, or double click
//todo: restrict the save only to drag, not click or double click.
						ImageDraws.Update(_listImageDraws[_idxTextSelected]);
						panelMain.Invalidate();
					}
					_isMouseDownPanel=false;
					return;
				}
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
			if(GetSelectedType()==EnumImageNodeType.Mount && !isClick && _cropPanAdj==EnumCropPanAdj.Adj && _isDraggingMount){
				//user dragged a mount item
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
				Document docOld=_arrayDocumentsShowing[_idxSelectedInMount].Copy();
				if(idxNewPos!=_idxSelectedInMount && _arrayDocumentsShowing[idxNewPos]!=null){
					//user dragged to an occupied spot, so swap
					//to keep it simple, we're going to reset to no crop when we swap them
					long mountItemNumOldPos=_arrayDocumentsShowing[_idxSelectedInMount].MountItemNum;
					_arrayDocumentsShowing[_idxSelectedInMount].MountItemNum=_listMountItems[idxNewPos].MountItemNum;
					_arrayDocumentsShowing[_idxSelectedInMount].CropX=0;
					_arrayDocumentsShowing[_idxSelectedInMount].CropY=0;
					_arrayDocumentsShowing[_idxSelectedInMount].CropW=0;
					_arrayDocumentsShowing[_idxSelectedInMount].CropH=0;
					if(_arrayDocumentsShowing[_idxSelectedInMount].ToothNumbers==_listMountItems[_idxSelectedInMount].ToothNumbers){//only if unchanged by user
						_arrayDocumentsShowing[_idxSelectedInMount].ToothNumbers=_listMountItems[idxNewPos].ToothNumbers;
					}
					Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
					docOld=_arrayDocumentsShowing[idxNewPos].Copy();
					_arrayDocumentsShowing[idxNewPos].MountItemNum=mountItemNumOldPos;
					_arrayDocumentsShowing[idxNewPos].CropX=0;
					_arrayDocumentsShowing[idxNewPos].CropY=0;
					_arrayDocumentsShowing[idxNewPos].CropW=0;
					_arrayDocumentsShowing[idxNewPos].CropH=0;
					if(_arrayDocumentsShowing[idxNewPos].ToothNumbers==_listMountItems[idxNewPos].ToothNumbers){//only if unchanged by user
						_arrayDocumentsShowing[idxNewPos].ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
					}
					Documents.Update(_arrayDocumentsShowing[idxNewPos],docOld);
					Bitmap bitmapOldPos=_arrayBitmapsShowing[_idxSelectedInMount];//no dispose, just ref
					_arrayBitmapsShowing[_idxSelectedInMount]=_arrayBitmapsShowing[idxNewPos];
					_arrayBitmapsShowing[idxNewPos]=bitmapOldPos;
					Document documentOldPos=_arrayDocumentsShowing[_idxSelectedInMount];
					_arrayDocumentsShowing[_idxSelectedInMount]=_arrayDocumentsShowing[idxNewPos];
					_arrayDocumentsShowing[idxNewPos]=documentOldPos;
					_isDraggingMount=false;
					panelMain.Invalidate();
					return;
				}
				//calculate the distance of drag that happened, in MOUNT coords.
				int xDrag=ControlPointToBitmapPoint(e.Location).X-ControlPointToBitmapPoint(_pointMouseDown).X;
				int yDrag=ControlPointToBitmapPoint(e.Location).Y-ControlPointToBitmapPoint(_pointMouseDown).Y;
				if(idxNewPos!=_idxSelectedInMount){
					bool movedPosNoCrop=false;//moving to a new mount position, but no crop was specified, then don't add one
					_arrayDocumentsShowing[_idxSelectedInMount].MountItemNum=_listMountItems[idxNewPos].MountItemNum;
					if(_arrayDocumentsShowing[_idxSelectedInMount].ToothNumbers==_listMountItems[_idxSelectedInMount].ToothNumbers){//only if unchanged by user
						_arrayDocumentsShowing[_idxSelectedInMount].ToothNumbers=_listMountItems[idxNewPos].ToothNumbers;
					}
					if(_arrayDocumentsShowing[_idxSelectedInMount].CropW==0 || _arrayDocumentsShowing[_idxSelectedInMount].CropH==0){
						movedPosNoCrop=true;
					}
					Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
					_arrayBitmapsShowing[idxNewPos]=_arrayBitmapsShowing[_idxSelectedInMount];
					_arrayBitmapsShowing[_idxSelectedInMount]=null;
					_arrayDocumentsShowing[idxNewPos]=_arrayDocumentsShowing[_idxSelectedInMount];
					_arrayDocumentsShowing[_idxSelectedInMount]=null;
					//reference point for drag needs to be relative to new mountitem instead of old mountitem, or crop xy gets set wrong
					xDrag+=_listMountItems[_idxSelectedInMount].Xpos+_listMountItems[_idxSelectedInMount].Width/2
						-(_listMountItems[idxNewPos].Xpos+_listMountItems[idxNewPos].Width/2);
					yDrag+=_listMountItems[_idxSelectedInMount].Ypos+_listMountItems[_idxSelectedInMount].Height/2
						-(_listMountItems[idxNewPos].Ypos+_listMountItems[idxNewPos].Height/2);
					_idxSelectedInMount=idxNewPos;
					if(movedPosNoCrop){//not adding crop, so nothing else to do
						_isDraggingMount=false;
						panelMain.Invalidate();
						return;
					}
					//It's now in new idx, but we still need to fix the crop below
				}
				//if no existing crop, we make one.
				if(_arrayDocumentsShowing[_idxSelectedInMount].CropW==0 || _arrayDocumentsShowing[_idxSelectedInMount].CropH==0){
					float scaleItem=ZoomSlider.CalcScaleFit(new Size(_listMountItems[_idxSelectedInMount].Width, _listMountItems[_idxSelectedInMount].Height), _arraySizesOriginal[_idxSelectedInMount], _arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated);
					if(ListTools.In(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated,0,180)){
						_arrayDocumentsShowing[_idxSelectedInMount].CropX=-((int)(_listMountItems[_idxSelectedInMount].Width/scaleItem)
							-_arraySizesOriginal[_idxSelectedInMount].Width)/2;//neg or 0
						_arrayDocumentsShowing[_idxSelectedInMount].CropY=-((int)(_listMountItems[_idxSelectedInMount].Height/scaleItem)
							-_arraySizesOriginal[_idxSelectedInMount].Height)/2;
						_arrayDocumentsShowing[_idxSelectedInMount].CropW=_arraySizesOriginal[_idxSelectedInMount].Width//in bitmap pixels
							+(int)(_listMountItems[_idxSelectedInMount].Width/scaleItem)-_arraySizesOriginal[_idxSelectedInMount].Width;//pos or 0
						_arrayDocumentsShowing[_idxSelectedInMount].CropH=_arraySizesOriginal[_idxSelectedInMount].Height
							+(int)(_listMountItems[_idxSelectedInMount].Height/scaleItem)-_arraySizesOriginal[_idxSelectedInMount].Height;
					}
					else{//90 or 270
						_arrayDocumentsShowing[_idxSelectedInMount].CropX=-((int)(_listMountItems[_idxSelectedInMount].Height/scaleItem)
							-_arraySizesOriginal[_idxSelectedInMount].Width)/2;
						_arrayDocumentsShowing[_idxSelectedInMount].CropY=-((int)(_listMountItems[_idxSelectedInMount].Width/scaleItem)
							-_arraySizesOriginal[_idxSelectedInMount].Height)/2;
						_arrayDocumentsShowing[_idxSelectedInMount].CropW=_arraySizesOriginal[_idxSelectedInMount].Width
							+(int)(_listMountItems[_idxSelectedInMount].Height/scaleItem)-_arraySizesOriginal[_idxSelectedInMount].Width;
						_arrayDocumentsShowing[_idxSelectedInMount].CropH=_arraySizesOriginal[_idxSelectedInMount].Height
							+(int)(_listMountItems[_idxSelectedInMount].Width/scaleItem)-_arraySizesOriginal[_idxSelectedInMount].Height;
					}

				}
				//now, we have a crop to work with for all items,
				if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated==0){
					float scaleItem=(float)_listMountItems[_idxSelectedInMount].Width/_arrayDocumentsShowing[_idxSelectedInMount].CropW;
					if(_arrayDocumentsShowing[_idxSelectedInMount].IsFlipped){
						_arrayDocumentsShowing[_idxSelectedInMount].CropX+=(int)(xDrag/scaleItem);
					}
					else{
						_arrayDocumentsShowing[_idxSelectedInMount].CropX-=(int)(xDrag/scaleItem);
					}
					_arrayDocumentsShowing[_idxSelectedInMount].CropY-=(int)(yDrag/scaleItem);
				}
				if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated==180){
					float scaleItem=(float)_listMountItems[_idxSelectedInMount].Width/_arrayDocumentsShowing[_idxSelectedInMount].CropW;
					if(_arrayDocumentsShowing[_idxSelectedInMount].IsFlipped){
						_arrayDocumentsShowing[_idxSelectedInMount].CropX-=(int)(xDrag/scaleItem);
					}
					else{
						_arrayDocumentsShowing[_idxSelectedInMount].CropX+=(int)(xDrag/scaleItem);
					}
					_arrayDocumentsShowing[_idxSelectedInMount].CropY+=(int)(yDrag/scaleItem);
				}
				if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated==90){
					float scaleItem=(float)_listMountItems[_idxSelectedInMount].Height/_arrayDocumentsShowing[_idxSelectedInMount].CropW;
					if(_arrayDocumentsShowing[_idxSelectedInMount].IsFlipped){
						_arrayDocumentsShowing[_idxSelectedInMount].CropX+=(int)(yDrag/scaleItem);
					}
					else{
						_arrayDocumentsShowing[_idxSelectedInMount].CropX-=(int)(yDrag/scaleItem);
					}
					_arrayDocumentsShowing[_idxSelectedInMount].CropY+=(int)(xDrag/scaleItem);
				}
				if(_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated==270){
					float scaleItem=(float)_listMountItems[_idxSelectedInMount].Height/_arrayDocumentsShowing[_idxSelectedInMount].CropW;
					if(_arrayDocumentsShowing[_idxSelectedInMount].IsFlipped){
						_arrayDocumentsShowing[_idxSelectedInMount].CropX-=(int)(yDrag/scaleItem);
					}
					else{
						_arrayDocumentsShowing[_idxSelectedInMount].CropX+=(int)(yDrag/scaleItem);
					}
					_arrayDocumentsShowing[_idxSelectedInMount].CropY-=(int)(xDrag/scaleItem);
				}
				//make sure the crop area falls within the image
				Rectangle rectangleBitmap=new Rectangle(0,0,_arraySizesOriginal[_idxSelectedInMount].Width,_arraySizesOriginal[_idxSelectedInMount].Height);
				Rectangle rectangleCrop=new Rectangle(_arrayDocumentsShowing[_idxSelectedInMount].CropX,_arrayDocumentsShowing[_idxSelectedInMount].CropY,
					_arrayDocumentsShowing[_idxSelectedInMount].CropW,_arrayDocumentsShowing[_idxSelectedInMount].CropH);
				if(!rectangleBitmap.IntersectsWith(rectangleCrop)){
					_arrayDocumentsShowing[_idxSelectedInMount].CropX=0;
					_arrayDocumentsShowing[_idxSelectedInMount].CropY=0;
					_arrayDocumentsShowing[_idxSelectedInMount].CropW=0;
					_arrayDocumentsShowing[_idxSelectedInMount].CropH=0;
				}
				Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount],docOld);
				_isDraggingMount=false;
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" was moved using the Adjust button";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
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
				//these two rectangles are in bitmap coords rather then screen
				Rectangle rectangleBitmap;
				if(GetDocumentShowing(0).CropW>0 && GetDocumentShowing(0).CropH>0){
					rectangleBitmap=new Rectangle(GetDocumentShowing(0).CropX,GetDocumentShowing(0).CropY,GetDocumentShowing(0).CropW,GetDocumentShowing(0).CropH);
				}
				else{
					rectangleBitmap=new Rectangle(0,0,GetBitmapShowing(0).Width,GetBitmapShowing(0).Height);
				}
				//need to untangle this rect after running it through the matrices
				Point pointCrop1=ControlPointToBitmapPoint(new Point(_rectangleCrop.X,_rectangleCrop.Y));
				Point pointCrop2=ControlPointToBitmapPoint(new Point(_rectangleCrop.X+_rectangleCrop.Width,_rectangleCrop.Y+_rectangleCrop.Height));
				Rectangle rectangleCrop=new Rectangle(Math.Min(pointCrop1.X,pointCrop2.X), Math.Min(pointCrop1.Y,pointCrop2.Y),
					Math.Abs(pointCrop1.X-pointCrop2.X),	Math.Abs(pointCrop1.Y-pointCrop2.Y));
				rectangleCrop=Rectangle.Intersect(rectangleBitmap,rectangleCrop);
				if(rectangleCrop==new Rectangle()){//crop is entirely outside image
					_rectangleCrop=new Rectangle();
					panelMain.Invalidate();
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Crop to Rectangle?")) {
					_rectangleCrop=new Rectangle();
					panelMain.Invalidate();
					return;
				}
				//MessageBox.Show(rectangleCrop.ToString());	
				Document docOld=GetDocumentShowing(0).Copy();
				GetDocumentShowing(0).CropX=rectangleCrop.X;
				GetDocumentShowing(0).CropY=rectangleCrop.Y;
				GetDocumentShowing(0).CropW=rectangleCrop.Width;
				GetDocumentShowing(0).CropH=rectangleCrop.Height;
				Documents.Update(GetDocumentShowing(0),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),PatFolder);
				LoadBitmap(0,EnumLoadBitmapType.IdxAndRaw);
				SetZoomSlider();
				_pointTranslation=new Point();
				_rectangleCrop=new Rectangle();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[0].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[0].FileName
					+" with catgegory "+defDocCategory.ItemName+" was changed using the Crop button";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,PatientCur.PatNum,logText);
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
		///<summary>Mostly to dispose of the old bitmaps all in one place.</summary>
		private void ClearObjects(){
			_bitmapRaw?.Dispose();
			_bitmapRaw=null;
			if(_arrayBitmapsShowing!=null && _arrayBitmapsShowing.Length>0){
				for(int i=0;i<_arrayBitmapsShowing.Length;i++){
					_arrayBitmapsShowing[i]?.Dispose();
					_arrayBitmapsShowing[i]=null;
				}
			}
			_arrayBitmapsShowing=null;
			SetDocumentShowing(0,null);
			_mountShowing=null;
			panelMain.Invalidate();
		}

		///<summary>Converts a point in panelMain into a point in _bitmapShowing.  If mount, then it's coords within entire mount.</summary>
		private Point ControlPointToBitmapPoint(Point pointPanel) {
			Matrix matrix=new Matrix();
			matrix.Translate(-panelMain.Width/2,-panelMain.Height/2,MatrixOrder.Append);
			matrix.Scale(1f/ZoomSliderValue*100f,1f/ZoomSliderValue*100f,MatrixOrder.Append);//1 if no scale
			matrix.Translate(-_pointTranslation.X,-_pointTranslation.Y,MatrixOrder.Append);
			//We are now at center image.
			if(IsMountShowing()){
				//no rotation or flip
				matrix.Translate(_mountShowing.Width/2f,_mountShowing.Height/2f,MatrixOrder.Append);
			}
			if(IsDocumentShowing()){
				if(GetBitmapShowing(0) is null){
					return Point.Empty;
				}
				matrix.Rotate(-GetDocumentShowing(0).DegreesRotated,MatrixOrder.Append);
				if(GetDocumentShowing(0).IsFlipped){
					Matrix matrixFlip=new Matrix(-1,0,0,1,0,0);
					matrix.Multiply(matrixFlip,MatrixOrder.Append);
				}
				if(GetDocumentShowing(0).CropW>0 && GetDocumentShowing(0).CropH>0){
					matrix.Translate(GetDocumentShowing(0).CropW/2f,GetDocumentShowing(0).CropH/2f,MatrixOrder.Append);//back to UL of cropped area
					matrix.Translate(GetDocumentShowing(0).CropX,GetDocumentShowing(0).CropY,MatrixOrder.Append);//then back to 00 of image
					//We can't really clip here.  We could test below, just before return, or caller could test point.
				}
				else{
					matrix.Translate(GetBitmapShowing(0).Width/2f,GetBitmapShowing(0).Height/2f,MatrixOrder.Append);
				}
			}
			Point[] points={pointPanel };
			matrix.TransformPoints(points);
			return points[0];
		}

		///<summary>Downloads the file contained in a document's note field, if there is one to be downloaded. A url must be prepended with a "_download_:" prefix or a base64 string with a "_rawBase64_:" prefix, which is removed upon successful download. The new file is saved in the current patient's folder.  </summary>
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
			Document importedDocument=new Document();
			if(document.Note.StartsWith(downloadPrefix)) {
				string url=document.Note.Substring(downloadPrefix.Length);
				string tempFileName=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),PatientCur.PatNum.ToString()+Path.GetExtension(url));
				WebClient webClient=new WebClient();
				progressOD.StartingMessage=Lan.g(this,"Downloading file from url")+"...";
				progressOD.ActionMain=() => {
					webClient.DownloadFile(url,tempFileName);
					importedDocument=ImageStore.Import(tempFileName,document.DocCategory,PatientCur);
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
					importedDocument=ImageStore.Import(tempFileName,document.DocCategory,PatientCur);
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
			document.Note=""; //if successful, remove need-to-download flag to prevent repeated downloads
			Documents.Update(document);
			SetDocumentShowing(0,document); //shows updated note text 
			EventFillTree?.Invoke(this,true);//updates tree to immediately include new file
		}

		///<summary></summary>
		private void DrawDocument(Graphics g){
			//we're at the center of the image, and working in image coordinates
			g.RotateTransform(GetDocumentShowing(0).DegreesRotated);
			if(GetDocumentShowing(0).IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Make our 0,0 reference point be the center of the portion of the image that will show:
			if(GetDocumentShowing(0).CropW>0 && GetDocumentShowing(0).CropH>0){
				g.TranslateTransform(-GetDocumentShowing(0).CropW/2,-GetDocumentShowing(0).CropH/2);//back to UL of cropped area
				g.TranslateTransform(-GetDocumentShowing(0).CropX,-GetDocumentShowing(0).CropY);//then back to 00 of image
				g.SetClip(new Rectangle(GetDocumentShowing(0).CropX,GetDocumentShowing(0).CropY,GetDocumentShowing(0).CropW,GetDocumentShowing(0).CropH));
			}
			else{
				g.TranslateTransform(-GetBitmapShowing(0).Width/2,-GetBitmapShowing(0).Height/2);//back to UL corner 00 of image
			}
			g.DrawImage(GetBitmapShowing(0),0,0,GetBitmapShowing(0).Width,GetBitmapShowing(0).Height);
			DrawDrawings(g);
			g.ResetClip();
			if(_rectangleCrop.Width>0 && _rectangleCrop.Height>0) {//Drawn last so it's on top.
				g.ResetTransform();//panel coords
				g.DrawRectangle(Pens.Blue,_rectangleCrop);
			}
		}

		///<summary>Used for drawing on screen in real time and also for drawing to printer.</summary>
		private void DrawMount(Graphics g){
			//we're at center of the mount, and working in mount coordinates
			g.TranslateTransform(-_mountShowing.Width/2f,-_mountShowing.Height/2f);
			using SolidBrush brushBack=new SolidBrush(_mountShowing.ColorBack);
			g.FillRectangle(brushBack,0,0,_mountShowing.Width,_mountShowing.Height);
			for(int i=0;i<_listMountItems.Count;i++){
				if(_isDraggingMount && i==_idxSelectedInMount){// && _dateTimeMouseDown.AddMilliseconds(600) < DateTime.Now){//_pointDragStart!=_pointDragNow){
					continue;//we'll paint this one after the loop
				}
				DrawMountOne(g,i);
				DrawMountOneText(g,i);
			}
			if(_isDraggingMount && _idxSelectedInMount>-1){// && _dateTimeMouseDown.AddMilliseconds(600) < DateTime.Now){//_pointDragStart!=_pointDragNow){
				DrawMountOne(g,_idxSelectedInMount);
			}
			//SELECT * FROM document WHERE patnum=293 AND mountitemnum > 0
			//outlines:
			using Pen penOutline=new Pen(ColorOD.Gray(100));
			for(int i=0;i<_listMountItems.Count;i++){
				g.DrawRectangle(penOutline,_listMountItems[i].Xpos,_listMountItems[i].Ypos,
					_listMountItems[i].Width,_listMountItems[i].Height);//silver is 50% black
			}
			DrawDrawings(g);
			//yellow outline of selected
			if(_idxSelectedInMount!=-1){
				g.DrawRectangle(Pens.Yellow,_listMountItems[_idxSelectedInMount].Xpos,_listMountItems[_idxSelectedInMount].Ypos,
					_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
			}
		}

		private void DrawDrawings(Graphics g){
			for(int i=0;i<_listImageDraws.Count;i++){
				if(_listImageDraws[i].DrawType==ImageDrawType.Text){
					Point point=_listImageDraws[i].GetTextPoint();
					using Font font=new Font(FontFamily.GenericSansSerif,_listImageDraws[i].FontSize);
					string s=_listImageDraws[i].GetTextString();
					Size size= g.MeasureString(s,font).ToSize();
					Rectangle rectangle=new Rectangle(point,size);
					if(_listImageDraws[i].ColorBack!=Color.Empty){
						using SolidBrush brushBack=new SolidBrush(_listImageDraws[i].ColorBack);
						g.FillRectangle(brushBack,rectangle);
					}
					using SolidBrush solidBrush=new SolidBrush(_listImageDraws[i].ColorDraw);
					g.DrawString(s,font,solidBrush,point);
					//if(_idxTextSelected==i){
					//	using Pen pen=new Pen(_listImageDraws[i].ColorDraw);
					//	g.DrawRectangle(pen,rectangle);
					//}
				}
			}
		}

		private void DrawMountOne(Graphics g,int i){
			GraphicsState graphicsStateMount=g.Save();
			g.TranslateTransform(_listMountItems[i].Xpos,_listMountItems[i].Ypos);//UL of mount position
			if(_isDraggingMount && i==_idxSelectedInMount){// && _dateTimeMouseDown.AddMilliseconds(600) < DateTime.Now){//_pointDragStart!=_pointDragNow){
				g.TranslateTransform(_pointDragNow.X-_pointDragStart.X,_pointDragNow.Y-_pointDragStart.Y);
				//combined with the centering below, this has the effect of dragging based on center
				//we show the entire image when dragging, so no clip
			}
			else{
				g.SetClip(new Rectangle(0,0,_listMountItems[i].Width,_listMountItems[i].Height));
			}
			g.TranslateTransform(_listMountItems[i].Width/2,_listMountItems[i].Height/2);//rotate and flip about the center of the mount box
			if(_arrayBitmapsShowing[i]!=null){
				if(_arrayDocumentsShowing[i].CropW > 0 && _arrayDocumentsShowing[i].CropH > 0){
					//We would scale here if bitmap was at original scale, but we've already scaled it when loading.
					//If we had scaled it, we would be in bitmap coords from here down instead of mount coords
					//We are also in the center of the cropped area because of the last two translations that happen just before drawImage
					g.RotateTransform(_arrayDocumentsShowing[i].DegreesRotated);
					if(_arrayDocumentsShowing[i].IsFlipped){
						Matrix matrix=new Matrix(-1,0,0,1,0,0);
						g.MultiplyTransform(matrix);
					}
					if(ListTools.In(_arrayDocumentsShowing[i].DegreesRotated,0,180)){
						g.TranslateTransform(-_listMountItems[i].Width/2,-_listMountItems[i].Height/2);//back to UL corner of cropped area
						float scale=(float)_listMountItems[i].Width/_arrayDocumentsShowing[i].CropW;//example 100/200=.5 because image was already resampled smaller
						g.TranslateTransform(-_arrayDocumentsShowing[i].CropX*scale,-_arrayDocumentsShowing[i].CropY*scale);//then to the 00 of the image
					}
					else{//90,270
						g.TranslateTransform(-_listMountItems[i].Height/2,-_listMountItems[i].Width/2);
						float scale=(float)_listMountItems[i].Height/_arrayDocumentsShowing[i].CropW;
						g.TranslateTransform(-_arrayDocumentsShowing[i].CropX*scale,-_arrayDocumentsShowing[i].CropY*scale);
					}
					g.DrawImage(_arrayBitmapsShowing[i],0,0,_arrayBitmapsShowing[i].Width,_arrayBitmapsShowing[i].Height);
				}
				else{//no crop specified, so just fit it to the mount space
					//g.TranslateTransform(_listMountItems[i].Xpos+_listMountItems[i].Width/2,_listMountItems[i].Ypos+_listMountItems[i].Height/2);//center
					//float scaleItem=ZoomSlider.CalcScaleFit(new Size(_listMountItems[i].Width,_listMountItems[i].Height),
					//	_arrayBitmapsShowing[i].Size,_arrayDocumentsShowing[i].DegreesRotated);
					//g.ScaleTransform(scaleItem,scaleItem);//then, scale to fit
					g.RotateTransform(_arrayDocumentsShowing[i].DegreesRotated);
					if(_arrayDocumentsShowing[i].IsFlipped){
						Matrix matrix=new Matrix(-1,0,0,1,0,0);
						g.MultiplyTransform(matrix);
					}
					g.TranslateTransform(-_arrayBitmapsShowing[i].Width/2,-_arrayBitmapsShowing[i].Height/2);//from center of the image to UL
					g.DrawImage(_arrayBitmapsShowing[i],0,0,_arrayBitmapsShowing[i].Width,_arrayBitmapsShowing[i].Height);
				}
			}
			g.Restore(graphicsStateMount);
		}

		private void DrawMountOneText(Graphics g,int i){
			if(_arrayBitmapsShowing[i]!=null){
				return;//only draw on empty spots
			}
			GraphicsState graphicsStateMount=g.Save();
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			g.TranslateTransform(_listMountItems[i].Xpos,_listMountItems[i].Ypos);//UL of mount position
			g.TranslateTransform(_listMountItems[i].Width/2,_listMountItems[i].Height/2);//center
			//So that they all look the same, we use the short dimension
			float heightFontPoint=_listMountItems[i].Height/15f*(96f/72f);
			if(_listMountItems[i].Width<_listMountItems[i].Height){
				heightFontPoint=_listMountItems[i].Width/15f*(96f/72f);
			}
			using Font font=new Font(FontFamily.GenericSansSerif,heightFontPoint);
			SizeF sizeString=g.MeasureString(_listMountItems[i].ItemOrder.ToString(),font);
			g.DrawString(_listMountItems[i].ItemOrder.ToString(),font,Brushes.Gray,-sizeString.Width/2,-sizeString.Height/2);
			g.Restore(graphicsStateMount);
		}

		///<summary>Gets the DefNum category of the current selection.</summary>
		private long GetCurrentCategory() {
			if(_nodeTypeKeyCatSelected==null){
				return Defs.GetDefsForCategory(DefCat.ImageCats,true)[0].DefNum;
			}
			if(_nodeTypeKeyCatSelected.NodeType==EnumImageNodeType.None){
				return Defs.GetDefsForCategory(DefCat.ImageCats,true)[0].DefNum;
			}
			return _nodeTypeKeyCatSelected.DefNumCategory;
		}

		///<summary>Pass in panel or mount coords. Returns index of item in mount, or -1 if no hit.</summary>
		private int HitTestInMount(int x,int y,bool isMountCoords=false){
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
				Rectangle rect=new Rectangle(_listMountItems[i].Xpos,_listMountItems[i].Ypos,_listMountItems[i].Width,_listMountItems[i].Height);
				if(rect.Contains(pointMount)){
					return i;
				}
			}
			return -1;
		}

		///<summary>Loads bitmap from disk, resizes, applies bright/contrast, and saves it to _arrayBitmapsShowing and/or _bitmapRaw.</summary>
		private void LoadBitmap(int idx,EnumLoadBitmapType loadBitmapType){
			if(idx==-1 || _arrayBitmapsShowing==null || idx > _arrayBitmapsShowing.Length-1){
				return;
			}
			if(_arrayDocumentsShowing[idx]==null){
				return;
			}
			if(loadBitmapType==EnumLoadBitmapType.OnlyIdx || loadBitmapType==EnumLoadBitmapType.IdxAndRaw){
				_arrayBitmapsShowing[idx]?.Dispose();
				_arrayBitmapsShowing[idx]=null;
				_arraySizesOriginal[idx]=new Size();
			}
			if(loadBitmapType==EnumLoadBitmapType.IdxAndRaw || loadBitmapType==EnumLoadBitmapType.OnlyRaw){
				_bitmapRaw?.Dispose();
				_bitmapRaw=null;
				_bitmapDicomRaw=null;
			}
			if(loadBitmapType==EnumLoadBitmapType.OnlyRaw){
				if(_arrayDocumentsShowing[idx].WindowingMax==0 || (_arrayDocumentsShowing[idx].WindowingMax==255 && _arrayDocumentsShowing[idx].WindowingMin==0)){
					if(_arrayBitmapsShowing[idx]==null){
						return;//we should already have an image showing, so this shouldn't happen
					}
					if(_arrayDocumentsShowing[idx].FileName.EndsWith(".dcm")){
						//This optimization doesn't matter because all dicom images need windowing.  We always get a new one.
						//Obviously, this needs improvement.  We don't want to be reloading images from disk just because user is clicking on item in mount.
					}
					else{
						_bitmapRaw=new Bitmap(_arrayBitmapsShowing[idx]);
						return;
					}
				}
			}
			//from here down, we must be getting the image from disk===================================================================================
			Bitmap bitmapTemp=null;
			//BitmapDicom bitmapDicom=null;
			if(_arrayDocumentsShowing[idx].FileName.EndsWith(".dcm")){
				_bitmapDicomRaw=ImageStore.OpenBitmapDicom(_arrayDocumentsShowing[idx],PatFolder);
				if(_bitmapDicomRaw==null){
					return;
				}
			}
			else{
				bitmapTemp=ImageStore.OpenImage(_arrayDocumentsShowing[idx],PatFolder);
				if(bitmapTemp==null){
					return;
				}
			}
			if(_nodeTypeKeyCatSelected.NodeType==EnumImageNodeType.Document){
				//always IdxAndRaw
				//single images simply load up the whole unscaled image. Mounts load the whole image, but maybe at a different scale to match mount scale.
				if(_arrayDocumentsShowing[idx].FileName.EndsWith(".dcm")){
					_arrayBitmapsShowing[idx]=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_arrayDocumentsShowing[idx].WindowingMin,_arrayDocumentsShowing[idx].WindowingMax);
					return;
				}
				try{
					_arrayBitmapsShowing[idx]=new Bitmap(bitmapTemp);//can crash here on a large image (WxHx24 > 250M)
					//jordan I redid my math a few months later, and it's WxHx4, not 24.  Size in memory should not be a problem, 
					//so now I don't know why it chokes on large images or why I've watched my memory usage climb to 1G when loading images.  Revisit.
					ImageHelper.ApplyColorSettings(_arrayBitmapsShowing[idx],_arrayDocumentsShowing[idx].WindowingMin,_arrayDocumentsShowing[idx].WindowingMax);
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
			if(_arrayDocumentsShowing[idx].FileName.EndsWith(".dcm")){
				_arraySizesOriginal[idx]=new Size(_bitmapDicomRaw.Width,_bitmapDicomRaw.Height);
			}
			else{
				_arraySizesOriginal[idx]=bitmapTemp.Size;
			}
			double scale;
			if(_arrayDocumentsShowing[idx].CropW==0){//no crop, so scaled to fit mount item
				scale=ZoomSlider.CalcScaleFit(new Size(_listMountItems[idx].Width,_listMountItems[idx].Height),_arraySizesOriginal[idx],_arrayDocumentsShowing[idx].DegreesRotated);
			}
			else{
				if(ListTools.In(_arrayDocumentsShowing[idx].DegreesRotated,0,180)){
					scale=(double)_listMountItems[idx].Width/_arrayDocumentsShowing[idx].CropW;
				}
				else{//90,270
					//Can't use cropH, because we always assume it's faulty.
					scale=(double)_listMountItems[idx].Height/_arrayDocumentsShowing[idx].CropW;
				}
			}
			if(loadBitmapType==EnumLoadBitmapType.OnlyIdx || loadBitmapType==EnumLoadBitmapType.IdxAndRaw){
				if(_arrayDocumentsShowing[idx].FileName.EndsWith(".dcm")){
					bitmapTemp=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_arrayDocumentsShowing[idx].WindowingMin,_arrayDocumentsShowing[idx].WindowingMax);
					_arrayBitmapsShowing[idx]=new Bitmap(bitmapTemp,new Size((int)(bitmapTemp.Width*scale),(int)(bitmapTemp.Height*scale)));
				}
				else{
					_arrayBitmapsShowing[idx]=new Bitmap(bitmapTemp,new Size((int)(bitmapTemp.Width*scale),(int)(bitmapTemp.Height*scale)));
					ImageHelper.ApplyColorSettings(_arrayBitmapsShowing[idx],_arrayDocumentsShowing[idx].WindowingMin,_arrayDocumentsShowing[idx].WindowingMax);
				}
			}
			if(loadBitmapType==EnumLoadBitmapType.IdxAndRaw || loadBitmapType==EnumLoadBitmapType.OnlyRaw){
				if(_arrayDocumentsShowing[idx].FileName.EndsWith(".dcm")){
					//already got _bitmapDicomRaw near beginning of this method
					_bitmapDicomRaw.Scale=scale;//stash this for later
				}
				else{
					_bitmapRaw=new Bitmap(bitmapTemp,new Size((int)(bitmapTemp.Width*scale),(int)(bitmapTemp.Height*scale)));
				}
				//don't apply color
			}
			bitmapTemp?.Dispose();//frees up the lock on the file on disk
			//_arrayBitmapsRaw[i].Clone(  //don't ever do it this way. Messes up dpi.
		}

		///<summary>Displays the PDF in a web browser. Downloads the PDF file from the cloud if necessary.</summary>
		private void LoadPdf(string atoZFolder,string atoZFileName,string localPath,string downloadMessage) {
			try {
				_webBrowser.Visible=true;
				SetPdfFilePath(atoZFolder,atoZFileName,localPath,downloadMessage);
				if(!File.Exists(_webBrowserFilePath)) {
					MessageBox.Show(Lan.g(this,"File not found")+": " + atoZFileName);
				}
				else {
					if(_countWebBrowserLoads==8) {
						//The webbrowser seems to develop a memory leak if reused many times.
						//We didn't notice this in 20.2 and earlier because we disposed of it each time.
						//But disposing each time can increase chance of crash as described a few lines down.
						//The compromise is to dispose every 8 times.
						_countWebBrowserLoads=0;
						_webBrowser.Dispose();//Release of reference to previous browser happens in background thread in the browser
						_webBrowser=null;
						Thread.Sleep(500);//Gives a little time for reference to previous browser to be released.
						//The bigger the pdf, the longer the dispose seems to take.
						//If clicking very quickly between large pdfs, we can be creating new webBrowser before previous is disposed.
						//This can cause a crash: OD will shut without any warning or message.
						_webBrowser=new WebBrowser();
						_webBrowser.Bounds=panelMain.Bounds;
						LayoutManager.Add(_webBrowser,this);
					}
					Application.DoEvents();//Show the browser control before loading, in case loading a large PDF, so the user can see the preview has started without waiting.
					//Adobe task pane on right will pop up after first reuse of browser (varies depending on pdf), 
					//but Adobe has a setting to remember to hide that.
					//Adobe toolbar on top will need to be manually shown after disposing of browser.
					//Adobe nav pane at left will also pop up after first reuse of browser.
					//The top toolbar and left nav pane issues are so annoying that we must set them here.
					//The downside seems to be that some users need to update to a newer version of Adobe Reader.
					string args="#toolbar=1&navpanes=0";//Show toolbar at top.  Hide nav panes at left. 
					_webBrowser.Navigate(_webBrowserFilePath+args);//The return status of this function doesn't seem to be helpful.
					panelMain.Visible=false;
					_isExportable=true;
					_countWebBrowserLoads++;
					IsImageFloatLocked=true;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				//An exception can happen if they do not have Adobe Acrobat Reader version 8.0 or later installed.
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
					//Check to see if the PDF already exists in the temp directory and if it's already in use (likely loaded into the web browser already).
					if(!ODFileUtils.IsFileInUse(pdfFilePath)) {
						FileAtoZ.Download(FileAtoZ.CombinePaths(atoZFolder,atoZFileName),pdfFilePath,downloadMessage);
					}
				}
			}
			else {
				pdfFilePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),GetDocumentShowing(0).DocNum+(PatientCur!=null ? PatientCur.PatNum.ToString() : "")+".pdf");
				//Check to see if the PDF already exists in the temp directory and if it's already in use (likely loaded into the web browser already).
				if(!ODFileUtils.IsFileInUse(pdfFilePath)) {
					File.WriteAllBytes(pdfFilePath,Convert.FromBase64String(GetDocumentShowing(0).RawBase64));
				}
			}
			_webBrowserFilePath=pdfFilePath;
		}

		///<summary>>Specify 0 when a single document is selected, or specify the idx within a mount.  Sets the element in _arrayBitmapsShowing.</summary>
		private void SetBitmapShowing(int idx,Bitmap bitmap){
			if(IsMountShowing()){
				//should be correct length
			}
			else{
				//this covers documents as well as nothing showing
				if(_arrayBitmapsShowing==null || _arrayBitmapsShowing.Length!=1){
					_arrayBitmapsShowing=new Bitmap[1];
				}
			}
			_arrayBitmapsShowing[idx]=bitmap;
		}

		private void ToolBarExport_ClickWeb(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageExport,GetDocumentShowing(0).DateCreated)) {
					return;
				}	
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),GetDocumentShowing(0).FileName);
				string docPath=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),GetDocumentShowing(0).FileName);
				FileAtoZ.Copy(docPath,tempFilePath,FileAtoZSourceDestination.AtoZToLocal,"Exporting file...");
				ThinfinityUtils.ExportForDownload(tempFilePath);
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing(0).DocCategory);
				string logText="Document Exported: "+GetDocumentShowing(0).FileName+" with catgegory "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,PatientCur.PatNum,logText,GetDocumentShowing(0).DocNum,GetDocumentShowing(0).DateTStamp);
				return;
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(Permissions.ImageExport,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),_arrayDocumentsShowing[_idxSelectedInMount].FileName);
				string docPath=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath()),_arrayDocumentsShowing[_idxSelectedInMount].FileName);
				FileAtoZ.Copy(docPath,tempFilePath,FileAtoZSourceDestination.AtoZToLocal,"Exporting file...");
				ThinfinityUtils.ExportForDownload(tempFilePath);
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Exported: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,PatientCur.PatNum,logText,_arrayDocumentsShowing[_idxSelectedInMount].DocNum,
					_arrayDocumentsShowing[_idxSelectedInMount].DateTStamp);
				return;
			}
			if(IsMountShowing()){
				if(!Security.IsAuthorized(Permissions.ImageExport,_mountShowing.DateCreated)) {
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
				string logText="Mount Exported: "+_mountShowing.Description+" with catgegory "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,PatientCur.PatNum,logText);
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
			string[] fileNames=openFileDialog.FileNames;
			if(fileNames.Length<1) {
				return;
			}
			List<MountItem> listMountItemsAvail=GetAvailSlots(fileNames.Length);
			if(listMountItemsAvail==null){
				MsgBox.Show("Not enough slots in the mount for the number of files selected.");
				return;
			}
			Document doc=null;
			for(int i=0;i<fileNames.Length;i++) {
				try {
					//.FileName is full path
					if(CloudStorage.IsCloudStorage) {
						//this will flicker because multiple progress bars.  Improve later.
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+fileNames[i]);
					continue;
				}
				Document docOld=doc.Copy();
				doc.MountItemNum=listMountItemsAvail[i].MountItemNum;
				doc.ToothNumbers=listMountItemsAvail[i].ToothNumbers;
				Documents.Update(doc,docOld);
				//this is all far too complicated:
				//_arrayDocumentsShowing[_idxSelectedInMount]=doc.Copy();
				//_arrayBitmapsRaw[_idxSelectedInMount]=ImageStore.OpenImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder,openFileDialog.FileName);
				//_arrayBitmapsShowing[_idxSelectedInMount]=ImageHelper.ApplyDocumentSettingsToImage2(
				//	_arrayDocumentsShowing[_idxSelectedInMount],_arrayBitmapsRaw[_idxSelectedInMount], ImageSettingFlags.CROP | ImageSettingFlags.COLORFUNCTION);
			}
			SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
			if(fileNames.Length==1){
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
			string[] fileNames=openFileDialog.FileNames;
			if(fileNames.Length<1) {
				return;
			}
			NodeTypeAndKey nodeTypeAndKey=null;
			Document doc=null;
			for(int i=0;i<fileNames.Length;i++) {
				try {
					if(CloudStorage.IsCloudStorage) {
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),PatientCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+openFileDialog.FileName);
					continue;
				}
				EventFillTree?.Invoke(this,false);
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum),fileNames[i]);
				using FormDocInfo FormD=new FormDocInfo(PatientCur,doc,isDocCreate:true);
				FormD.ShowDialog(this);//some of the fields might get changed, but not the filename
				if(FormD.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,doc);
				}
				else {
					if(doc.ImgType==ImageType.Photo) {
						PatientEvent.Fire(ODEventType.Patient,PatientCur);//Possibly updated the patient picture.
					}
					nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum);
					SetDocumentShowing(0,doc.Copy());
					//these two lines are needed in case they change the category
					EventFillTree?.Invoke(this,true);
					SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum),fileNames[i]);
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
			public EnumImageNodeType NodeType;
			public long PriKey;
			public long DefNumCategory;
		}
		#endregion Class - Nested

	}

	///<summary></summary>
	public class WindowingEventArgs{
		public int MinVal;
		public int MaxVal;
	}

}



