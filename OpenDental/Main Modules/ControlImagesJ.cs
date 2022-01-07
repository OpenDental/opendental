#region using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using OpenDental.Thinfinity;
using ImagingDeviceManager;
using System.Net;
#endregion using

namespace OpenDental{
	///<summary>The Images Module.  See long comments at bottom of this file.</summary>
	public partial class ControlImagesJ : UserControl{
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
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
		private Cursor _cursorPan;
		private DateTime _dateTimeMouseDownPanel;
		private DeviceController _deviceController; 
		private Family _familyCur=null;
		private FileSystemWatcher _fileSystemWatcher;
		private FormVideo _formVideo;
		///<summary>The index of the currently selected item within a mount. 0-indexed.  Frequently -1 to indicate no selection.</summary>
		private int _idxSelectedInMount=-1;
		private bool _initializedOnStartup=false;
		//<summary>For during xray capture to series of mount positions.</summary>
		//private bool _isAcquiringSeries;
		private bool _isDraggingMount=false;
		private bool _isMouseDownPanel;
		///<summary>Collapse is toggled with the triangle on the sizer.  Always starts out not collapsed.</summary>
		private bool _isTreeDockerCollapsed=false;
		///<summary>Collapsing while the mouse is down triggers another mouse move, so this prevents taking action on that phantom mouse move.</summary>
		private bool _isTreeDockerCollapsing=false;
		///<summary>If a mount is currently selected, this is the list of the mount items on it.</summary>
		private List<MountItem> _listMountItems=null;
		///<summary>Keeps track of the currently selected mount object (only when a mount is selected).</summary>
		private Mount _mountShowing=null;
		///<summary>One of these 4 states is active at any time.</summary>
		private EnumCropPanAdj _cropPanEditAdj;
		private Patient _patCur=null;
		///<summary>Set with each RefreshModuleData, and that's where it's set if it doesn't yet exist.  For now, we are not using _patCur.ImageFolder, because we haven't tested whether it properly updates the patient object.  We don't want to risk using an outdated patient folder path.  And we don't want to waste time refreshing PatCur after every ImageStore.GetPatientFolder().</summary>
		private string _patFolder="";
		///<summary>Prevents too many security logs for this patient.</summary>
		private long _patNumLastSecurityLog=0;
		///<summary>When dragging mount item, this is the starting point of the center of the mount item where raw image will draw, in mount coordinates.</summary>
		private Point _pointDragStart;
		///<summary>When dragging mount bitmap, this is the current point of the bitmap, in mount coordinates.</summary>
		private Point _pointDragNow;
		private Point _pointMouseDown=new Point(0,0);
		///<summary>This translation is added to the bitmaps showing, based on user drags.  It's in bitmap/mount coords, not screen coords.</summary>
		private Point _pointTranslation;
		///<summary>When mouse down, this is recorded as the _pointTranslation for delta purposes while dragging.</summary>
		private Point _pointTranslationOld;
		///<summary>In panel coords.</summary>
		private Rectangle _rectangleCrop;
		///<summary>Used to display Topaz signatures on Windows.</summary>
		private Control _sigBoxTopaz;
		///<summary>Displays PDFs.</summary>
		private WebBrowser _webBrowser=null;
		///<summary>The location of the file that <see cref="_webBrowser" /> has navigated to.</summary>
		private string _webBrowserFilePath=null;
		///<summary>User can control width of tree.  This is stored as the 96dpi equivalent as float for conversion accuracy.  When tree is minimized, this doesn't change, allowing restoration to previous width.  It does not remember width between sessions.  Minimum 0, max 500.</summary>
		private float _widthTree96=228;
		private float _widthTree96StartDrag;
		#endregion Fields - Private

		#region Constructor
		public ControlImagesJ(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			Logger.LogToPath("InitializeComponent",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			Logger.LogToPath("InitializeComponent",LogPath.Startup,LogPhase.End);
			_cursorPan=new Cursor(GetType(),"CursorPalm.cur");
			panelMain.Cursor=_cursorPan;
			panelMain.MouseWheel += panelMain_MouseWheel;
			imageSelector.ContextMenu=menuTree;
			try {
				_sigBoxTopaz=TopazWrapper.GetTopaz();
				LayoutManager.Add(_sigBoxTopaz,panelNote);
				_sigBoxTopaz.Location=sigBox.Location;//new System.Drawing.Point(437,15);
				_sigBoxTopaz.Name="sigBoxTopaz";
				_sigBoxTopaz.Size=new System.Drawing.Size(362,79);
				_sigBoxTopaz.TabIndex=93;
				_sigBoxTopaz.Text="sigPlusNET1";
				_sigBoxTopaz.DoubleClick+=new System.EventHandler(this.sigBoxTopaz_DoubleClick);
				TopazWrapper.SetTopazState(_sigBoxTopaz,0);
			}
			catch (Exception ex){
				Logger.LogToPath("ControlImagesJ Ctor failed: "+ex.Message,LogPath.Startup,LogPhase.Unspecified);
			}
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Enums
		///<summary>3 States.</summary>
		private enum EnumCropPanAdj{
			///<summary>Looks like arrow. Only for docs, not mounts.</summary>
			Crop,
			///<summary>Looks like a hand.</summary>
			Pan,
			///<summary>Cursor is 4 arrows.</summary>
			Adj
		}

		private enum EnumLoadBitmapType{
			///<summary>Load into _arrayBitmapsShowing[idx] for each item in mount when SelectTreeNode.</summary>
			OnlyIdx,
			///<summary>Load into _arrayBitmapsShowing[idx] and _bitmapRaw when mount image is resized. When using single images rather than mounts, this is the only enum option used, including for SelectTreeNode and cropping.</summary>
			IdxAndRaw,
			///<summary>Load image into _bitmapRaw every time user selects new mount item, but not from disk if no bright/contr yet.</summary>
			OnlyRaw
		}

		///<summary>ToolBarButton enumeration instead of strings.  For all three toolBars combined.</summary>
		private enum TB{
			Print,
			Delete,
			Info,
			Sign,
			ScanDoc,
			ScanMultiDoc,
			ScanXRay,
			ScanPhoto,
			Acquire,
			Video,
			Import,
			Export,
			Copy,
			Paste,
			Forms,
			Mounts,
			ZoomOne,
			Crop,
			Pan,
			Flip,
			RotateL,
			RotateR,
			Rotate180,
			Adj,
			Size
		}

		#endregion Enums

		#region Properties
		///<summary>Only used when a single document is selected, not a mount.  Gets the first element in _arrayBitmapsShowing.</summary>
		private Bitmap GetBitmapShowing0() {
			if(_arrayBitmapsShowing==null){
				return null;
			}
			if(_arrayBitmapsShowing.Length==0){
				return null;
			}
			return _arrayBitmapsShowing[0];
		}

		///<summary>Only used when a single document is selected, not a mount.  Sets the first element in _arrayBitmapsShowing.</summary>
		private void SetBitmapShowing0(Bitmap bitmap){
			if(_arrayBitmapsShowing==null || _arrayBitmapsShowing.Length!=1){
				_arrayBitmapsShowing=new Bitmap[1];
			}
			_arrayBitmapsShowing[0]=bitmap;
		}

		///<summary>Only used when a single document is selected, not a mount.  Gets the first element in _arrayDocumentsShowing.</summary>
		private Document GetDocumentShowing0() {
			if(_arrayDocumentsShowing==null){
				return null;
			}
			if(_arrayDocumentsShowing.Length==0){
				return null;
			}
			return _arrayDocumentsShowing[0];
		}
		
		///<summary>Only used when a single document is selected, not a mount.  Sets the first element in _arrayDocumentsShowing.</summary>
		private void SetDocumentShowing0(Document document){
			if(_arrayDocumentsShowing==null || _arrayDocumentsShowing.Length!=1){
				_arrayDocumentsShowing=new Document[1];
			}
			_arrayDocumentsShowing[0]=document;
		}
		#endregion Properties

		#region Methods - Public
		///<summary>Refreshes list from db, then fills the treeview.  Set keepSelection to true in order to keep the current selection active.</summary>
		public void FillTree(bool keepSelection) {
			if(_patCur==null) {
				imageSelector.ClearAll();
				return;
			}
			List<Def> listDefsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			imageSelector.SetCategories(listDefsImageCats);
			DataSet dataSet=Documents.RefreshForPatient(new string[] { _patCur.PatNum.ToString() });
			imageSelector.SetData(_patCur,dataSet.Tables["DocumentList"],keepSelection);
			imageSelector.LoadExpandedPrefs();
		}

		///<summary>Also does LayoutToolBars. Doesn't really do much, but we like to have one for each module.</summary>
		public void InitializeOnStartup(){
			if(_initializedOnStartup) {
				return;
			}
			_initializedOnStartup=true;
			LayoutToolBars();
			_webBrowser=new WebBrowser();//Include a webBrowser object for loading .pdf files.
			_webBrowser.Visible=false;
			_webBrowser.Bounds=panelMain.Bounds;
			LayoutManager.Add(_webBrowser,this);
		}

		///<summary>Key down from FormOpenDental is passed here to allow triggering video capture.  As long as this module is open, all key down events are sent here.</summary>
		public void ControlImagesJ_KeyDown(Keys keys){
			if(_formVideo!=null){
				_formVideo.Parent_KeyDown(keys);
			}
		}
		
		///<summary>Layout both toolbars.</summary>
		public void LayoutToolBars() {
			toolBarMain.Buttons.Clear();
			toolBarPaint.Buttons.Clear();
			ODToolBarButton button;
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),1,Lan.g(this,"Print"),TB.Print));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Delete"),2,Lan.g(this,"Delete"),TB.Delete));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Info"),3,Lan.g(this,"Item Info"),TB.Info));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Sign"),-1,Lan.g(this,"Sign this document"),TB.Sign));
			toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			button=new ODToolBarButton(Lan.g(this,"Scan:"),-1,"","");
			button.Style=ODToolBarButtonStyle.Label;
			toolBarMain.Buttons.Add(button);
			toolBarMain.Buttons.Add(new ODToolBarButton("",14,Lan.g(this,"Scan Document"),TB.ScanDoc));
			toolBarMain.Buttons.Add(new ODToolBarButton("",18,Lan.g(this,"Scan Multi-Page Document"),TB.ScanMultiDoc));
			toolBarMain.Buttons.Add(new ODToolBarButton("",EnumIcons.ImageSelectorXray,Lan.g(this,"Scan Radiograph"),TB.ScanXRay));
			toolBarMain.Buttons.Add(new ODToolBarButton("",EnumIcons.ImageSelectorPhoto,Lan.g(this,"Scan Photo"),TB.ScanPhoto));
			toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Acquire"),EnumIcons.Acquire,Lan.g(this,"Acquire from device"),TB.Acquire));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Video"),EnumIcons.Video,Lan.g(this,"Intraoral Video Camera"),TB.Video));
			toolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			//import
			button=new ODToolBarButton(Lan.g(this,"Import"),5,Lan.g(this,"Import From File"),TB.Import);
			button.Style=ODToolBarButtonStyle.DropDownButton;
			ContextMenuStrip contextMenuStripImport=new ContextMenuStrip();
			ToolStripMenuItem toolStripMenuItem=new ToolStripMenuItem(Lan.g(this,"Import Automatically"));
			toolStripMenuItem.ToolTipText="Import files as they are created in a folder.";
			toolStripMenuItem.Click+=ToolBarImportAuto;
			contextMenuStripImport.Items.Add(toolStripMenuItem);
			button.ContextMenuStripDropDown=contextMenuStripImport;
			toolBarMain.Buttons.Add(button);
			//export
			//toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Export"),19,Lan.g(this,"Export To File"),TB.Export));
			button=new ODToolBarButton(Lan.g(this,"Export"),19,Lan.g(this,"Export To File"),TB.Export);
			button.Style=ODToolBarButtonStyle.DropDownButton;
			ContextMenuStrip contextMenuStripExport=new ContextMenuStrip();
			ToolStripMenuItem toolStripMenuItem2=new ToolStripMenuItem(Lan.g(this,"Move to Patient..."));
			toolStripMenuItem2.Click+=ToolBarMoveToPatient;
			contextMenuStripExport.Items.Add(toolStripMenuItem2);
			button.ContextMenuStripDropDown=contextMenuStripExport;
			toolBarMain.Buttons.Add(button);
			//others
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Copy"),17,Lan.g(this,"Copy displayed image to clipboard"),TB.Copy));
			toolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Paste"),6,Lan.g(this,"Paste From Clipboard"),TB.Paste));
			//Forms:
			button=new ODToolBarButton(Lan.g(this,"Forms"),-1,"",TB.Forms);
			button.Style=ODToolBarButtonStyle.DropDownButton;
			menuForms=new ContextMenu();
			string formDir=FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),"Forms");
			if(CloudStorage.IsCloudStorage) {
				//Running this asynchronously to not slowdown start up time.
				ODThread odThreadTemplate=new ODThread((o) => {
					OpenDentalCloud.Core.TaskStateListFolders state=CloudStorage.ListFolderContents(formDir);
					foreach(string fileName in state.ListFolderPathsDisplay) {
						if(InvokeRequired) {
							Invoke((Action)delegate () {
								menuForms.MenuItems.Add(Path.GetFileName(fileName),new EventHandler(menuForms_Click));
							});
						}
					}
				});
				//Swallow all exceptions and allow thread to exit gracefully.
				odThreadTemplate.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => { }));
				odThreadTemplate.Start(true);
			}
			else {//Not cloud
				if(Directory.Exists(formDir)) {
					DirectoryInfo dirInfo=new DirectoryInfo(formDir);
					FileInfo[] fileInfos=dirInfo.GetFiles();
					for(int i=0;i<fileInfos.Length;i++) {
						if(Documents.IsAcceptableFileName(fileInfos[i].FullName)) {
							menuForms.MenuItems.Add(fileInfos[i].Name,menuForms_Click);
						}
					}
				}
			}
			button.DropDownMenu=menuForms;
			toolBarMain.Buttons.Add(button);
			button=new ODToolBarButton(Lan.g(this,"Mounts"),-1,"",TB.Mounts);
			button.Style=ODToolBarButtonStyle.DropDownButton;
			menuMounts=new ContextMenu();
			List<MountDef> listMountDefs=MountDefs.GetDeepCopy();
			for(int i=0;i<listMountDefs.Count;i++){
				menuMounts.MenuItems.Add(listMountDefs[i].Description,menuMounts_Click);
			}
			button.DropDownMenu=menuMounts;
			toolBarMain.Buttons.Add(button);
			//Program links:
			ProgramL.LoadToolbar(toolBarMain,ToolBarsAvail.ImagesModule);
			//ToolbarPaint-------------------------------------------------------------------------------------
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Fit 1"),-1,Lan.g(this,"Zoom to fit one image"),TB.ZoomOne));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			button=new ODToolBarButton(Lan.g(this,"Crop"),7,"",TB.Crop);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanEditAdj==EnumCropPanAdj.Crop){
				toolBarPaint.Buttons[TB.Crop.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Pan"),10,"",TB.Pan);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanEditAdj==EnumCropPanAdj.Pan){
				toolBarPaint.Buttons[TB.Pan.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Adj"),20,Lan.g(this,"Adjust position"),TB.Adj);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanEditAdj==EnumCropPanAdj.Adj){
				toolBarPaint.Buttons[TB.Adj.ToString()].Pushed=true;
			}
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Size"),-1,Lan.g(this,"Set Size"),TB.Size));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"FlipH"),11,Lan.g(this,"Flip Horizontally"),TB.Flip));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"-90"),12,Lan.g(this,"Rotate Left"),TB.RotateL));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"+90"),13,Lan.g(this,"Rotate Right"),TB.RotateR));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"180"),-1,Lan.g(this,"Rotate 180"),TB.Rotate180));
			toolBarMain.Invalidate();
			toolBarPaint.Invalidate();
			Plugins.HookAddCode(this,"ContrDocs.LayoutToolBar_end",_patCur);
		}
		
		///<summary></summary>
		public void ModuleSelected(long patNum,long docNum=0){
			try {
				RefreshModuleData(patNum);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error accessing images."),ex);
			}
			if(_patCur!=null && _patCur.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(_patCur.PatNum);
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				try {
					RefreshModuleData(0);
				}
				catch(Exception ex) {//Exception should never throw because RefreshModuleData() will return when PatNum is zero.
					FriendlyException.Show(Lan.g(this,"Error accessing images."),ex);
				}
			}
			RefreshModuleScreen();
			if(docNum!=0) {
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,docNum));
			}
			Plugins.HookAddCode(this,"ContrImages.ModuleSelected_end",patNum,docNum);
		}
		
		///<summary></summary>
		public void ModuleUnselected(){
			_familyCur=null;
			//foreach(Control c in this.Controls) {
			//	if(c.GetType()==typeof(WebBrowser)) {//_webBrowserDocument
			//		Controls.Remove(c);
			//		c.Dispose();
			//	}
			//}
			_patNumLastSecurityLog=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			Plugins.HookAddCode(this,"ContrImages.ModuleUnselected_end");
		}

		///<summary>Fired when user clicks on tree and also for automated selection that's not by mouse, such as image import, image paste, etc.  Can pass in NULL.  localPathImported will be set only if using Cloud storage and an image was imported.  We want to use the local version instead of re-downloading what was just uploaded.  nodeObjTag does not need to be same object, but must match type and priKey.</summary>
		public void SelectTreeNode(TypeAndKey typeAndKey,string localPathImportedCloud="") {
			//Select the node always, but perform additional tasks when necessary (i.e. load an image, or mount).	
			if(typeAndKey is null || typeAndKey.NodeType==EnumImageNodeType.None){	
				ClearObjects();
				if(_webBrowser!=null) {
					_webBrowser.Visible=false;//Clear any previously loaded Acrobat .pdf file.
				}
				return;
			}
			else{
				imageSelector.SetSelected(typeAndKey.NodeType,typeAndKey.PriKey);//this is redundant when user is clicking, but harmless 
			}
			ClearObjects();
			_pointTranslation=new Point();
			panelMain.Visible=true;
			if(_webBrowser!=null) {
				_webBrowser.Visible=false;//Clear any previously loaded Acrobat .pdf file.
			}
			if(typeAndKey.NodeType==EnumImageNodeType.Document){
				SetDocumentShowing0(Documents.GetByNum(typeAndKey.PriKey,doReturnNullIfNotFound:true));
				if(GetDocumentShowing0()==null) {
					MsgBox.Show(this,"Document was previously deleted.");
					FillTree(false);
					return;
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
						Pat=_patCur,
						ListDocuments=_arrayDocumentsShowing.ToList(),
						BitmapImagesModule=_bitmapRaw
					}
				);
				DownloadDocumentNoteFile(typeAndKey);
				bool isExportable=panelMain.Visible;
				if(_bitmapRaw==null) {
					if(ImageHelper.HasImageExtension(GetDocumentShowing0().FileName)) {
						string srcFileName = ODFileUtils.CombinePaths(_patFolder,GetDocumentShowing0().FileName);
						if(File.Exists(srcFileName)) {
							MessageBox.Show(Lan.g(this,"File found but cannot be opened, probably because it's too big:")+srcFileName);
						}
						else {
							MessageBox.Show(Lan.g(this,"File not found")+": " + srcFileName);
						}
					}
					else if(Path.GetExtension(GetDocumentShowing0().FileName).ToLower()==".pdf") {//Adobe acrobat file.
						if(!PrefC.GetBool(PrefName.PdfLaunchWindow)) {
							LoadPdf(_patFolder,GetDocumentShowing0().FileName,localPathImportedCloud,ref isExportable,"Downloading Document...");
						}
					}
				}
				SetWindowingSlider();
				//In Web mode the buttons do not appear when hovering over the PDF, so we need to enable the print toolbar button.
				bool doShowPrint=panelMain.Visible;
				if(_webBrowser!=null && _webBrowser.Visible && ODBuild.IsWeb()) {
					doShowPrint=true;
				}
				EnableToolBarButtons(print:doShowPrint, delete:true, info:true, sign:true, export:isExportable, copy:panelMain.Visible, brightAndContrast:panelMain.Visible, zoom:panelMain.Visible, zoomOne:false, crop:panelMain.Visible, pan:panelMain.Visible, adj:false, size:panelMain.Visible, flip:panelMain.Visible, rotateL:panelMain.Visible, rotateR:panelMain.Visible, rotate180:panelMain.Visible);
				SetCropPanEditAdj(EnumCropPanAdj.Pan);
			}
			if(typeAndKey.NodeType==EnumImageNodeType.Mount){
				_mountShowing=Mounts.GetByNum(typeAndKey.PriKey);
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
				EnableToolBarButtonsMount();
				SetCropPanEditAdj(EnumCropPanAdj.Pan);
				Cursor=Cursors.Default;
			}
			SetPanelNoteVisibility();
			LayoutControls();
			SetZoomSlider();
			FillSignature();
			//InvalidateSettingsColor();
			panelMain.Invalidate();
		}

		///<summary>Downloads the file contained in a document's note field, if there is one to be downloaded. A url must be prepended with a "_download_:" prefix or a base64 string with a "_rawBase64_:" prefix, which is removed upon successful download. The new file is saved in the current patient's folder.  </summary>
		private void DownloadDocumentNoteFile(TypeAndKey typeAndKey) {
			Document document=GetDocumentShowing0();
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
				string tempFileName=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),_patCur.PatNum.ToString()+Path.GetExtension(url));
				WebClient webClient=new WebClient();
				progressOD.StartingMessage=Lan.g(this,"Downloading file from url")+"...";
				progressOD.ActionMain=() => {
					webClient.DownloadFile(url,tempFileName);
					importedDocument=ImageStore.Import(tempFileName,document.DocCategory,_patCur);
					File.Delete(tempFileName);
				};
			}
      else if(document.Note.StartsWith(base64Prefix)) {
				string base64=document.Note.Substring(base64Prefix.Length);
				string extension=StringTools.SubstringBefore(base64,"_");
				base64=base64.Substring(extension.Length+1); //trim extension from base64 string
				byte[] byteArray=Convert.FromBase64String(base64);
				string tempFileName=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),_patCur.PatNum.ToString()+extension);
				progressOD.StartingMessage=Lan.g(this,"Converting file from base64")+"...";
				progressOD.ActionMain=() => {
					File.WriteAllBytes(tempFileName,byteArray);
					importedDocument=ImageStore.Import(tempFileName,document.DocCategory,_patCur);
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
			SetDocumentShowing0(document); //shows updated note text 
			FillTree(true); //updates tree to immediately include new file
		}
		#endregion Methods - Public

		#region Methods - Event Handlers
		private void butBrowse_Click(object sender,EventArgs e) {
			FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			if(Directory.Exists(textImportAuto.Text)){
				folderBrowserDialog.SelectedPath=textImportAuto.Text;
			}
			else{
				folderBrowserDialog.SelectedPath=@"C:\";
			}
			DialogResult dialogResult=folderBrowserDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				return;
			}
			if(textImportAuto.Text==""){
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Set as default?")){
					Prefs.UpdateString(PrefName.AutoImportFolder,folderBrowserDialog.SelectedPath);
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
			textImportAuto.Text=folderBrowserDialog.SelectedPath;
		}

		private void butCancel_Click(object sender, EventArgs e){
			//could say Cancel or Stop.  Works the same, either way.
			_deviceController?.DisposeDevice();
			_deviceController=null;
			panelAcquire.Visible=false;
			LayoutControls();
		}

		private void butGoTo_Click(object sender,EventArgs e) {
			if(!Directory.Exists(textImportAuto.Text)){
				MsgBox.Show(this,"Folder does not exist.");
				return;
			}
			Process.Start(textImportAuto.Text);
		}

		private void butImportClose_Click(object sender,EventArgs e) {
			panelImportAuto.Visible=false;
			LayoutControls();
			if(_fileSystemWatcher!=null){
				_fileSystemWatcher.EnableRaisingEvents=false;
			}
		}

		private void butImportStart_Click(object sender,EventArgs e) {
			if(textImportAuto.Text==""){
				MsgBox.Show(this,"Please enter a folder name.");
				return;
			}
			if(!Directory.Exists(textImportAuto.Text)){
				MsgBox.Show(this,"Folder does not exist.");
				return;
			}
			if(IsMountShowing()){
				if(_idxSelectedInMount==-1){
					//We already did this when clicking toolbar button, but user might have selected a mount after clicking that.
					for(int i=0;i<_arrayDocumentsShowing.Length;i++){
						if(_arrayDocumentsShowing[i]==null){
							_idxSelectedInMount=i;//preselect the first item available
							break;
						}
					}
					panelMain.Invalidate();
					//If one is still not selected, they will get a warning below
				}
				if(IsMountItemSelected() || _idxSelectedInMount==-1){
					MsgBox.Show(this,"Please select an empty position in the mount, first.");
					return;
				}
			}
			textImportAuto.ReadOnly=true;
			textImportAuto.BackColor=Color.White;
			butBrowse.Visible=false;
			butGoTo.Visible=false;
			butImportStart.Visible=false;
			butImportClose.Text=Lan.g(this,"Stop");
			if(_fileSystemWatcher==null){
				_fileSystemWatcher=new FileSystemWatcher(textImportAuto.Text);
				_fileSystemWatcher.Created+=_fileSystemWatcher_FileCreated;
			}
			else{
				_fileSystemWatcher.Path=textImportAuto.Text;
			}
			_fileSystemWatcher.EnableRaisingEvents=true;
		}

		private void butStart_Click(object sender, EventArgs e){	
			if(IsMountShowing()){
				if(IsMountItemSelected()){
					//user must have clicked onto an occupied position in the middle of a series.
					MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
					return;
				}
				if(_idxSelectedInMount==-1){
					//user must have clicked outside mount to deselect in the middle of a series
					MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
					return;
				}
			}
			ImagingDevice	imagingDevice=comboDevice.GetSelected<ImagingDevice>();	
			_deviceController=new DeviceController();
			_deviceController.ImgDeviceControlType=ConvertToImgDeviceControlType(imagingDevice.DeviceType);
			_deviceController.HandleWindow=Handle;
			_deviceController.ShowTwainUI=imagingDevice.ShowTwainUI;
			_deviceController.TwainName=imagingDevice.TwainName;
			_deviceController.BitmapAcquired+=_deviceController_BitmapAcquired;
			_deviceController.StatusReceived+=_deviceController_StatusReceived;
			try{
				_deviceController.InitializeDevice();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				_deviceController=null;
				return;
			}
			butStart.Enabled=false;
			butCancel.Text=Lan.g(this,"Stop");
			AcquireBitmap();
		}

		private void ContrImages_Resize(object sender, EventArgs e){
			LayoutControls();
		}

		private void _deviceController_BitmapAcquired(object sender,BitmapAcquireEventArgs e){
			Document document=null;
			Size sizeBitmap=e.SizeBitmap;
			Bitmap bitmap=e.Bitmap;
			if(!e.FilePath.IsNullOrEmpty() && e.FilePath.EndsWith(".dcm")){
				_bitmapDicomRaw=DicomHelper.GetFromFile(e.FilePath);
				sizeBitmap=new Size(_bitmapDicomRaw.Width,_bitmapDicomRaw.Height);
				DicomHelper.CalculateWindowingOnImport(_bitmapDicomRaw);//happens again 8 lines down in ImageStore.Import, so this could be optimized better. 
				bitmap=DicomHelper.ApplyWindowing(_bitmapDicomRaw,_bitmapDicomRaw.WindowingMin,_bitmapDicomRaw.WindowingMax);
			}
			if(sizeBitmap.Width==0 || bitmap is null){
				return;
			}
			try {
				if(!e.FilePath.IsNullOrEmpty()){
					document=ImageStore.Import(e.FilePath,GetCurrentCategory(),_patCur);
					File.Delete(e.FilePath);
				}
				else if(bitmap!=null){
					document=ImageStore.Import(bitmap,GetCurrentCategory(),ImageType.Radiograph,_patCur,mimeType:"image/tiff");
				}
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save")+": "+ex.Message);
				bitmap?.Dispose();
				_deviceController=null;
				panelAcquire.Visible=false;
				return;
			}
			if(!IsMountShowing()){//single
				FillTree(false);//Reload and keep new document selected.
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,document.DocNum));
				bitmap?.Dispose();
				_deviceController=null;
				panelAcquire.Visible=false;
				return;
			}
			//From here down is mount======================================================================================
			if(IsMountItemSelected()){
				//user must have clicked onto an occupied position in the middle of a series.
				MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
				return;
			}
			if(_idxSelectedInMount==-1){
				//user must have clicked outside mount to deselect in the middle of a series
				MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
				return;
			}
			Document docOld=document.Copy();
			document.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
			document.DegreesRotated=_listMountItems[_idxSelectedInMount].RotateOnAcquire;
			document.ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
			Documents.Update(document,docOld);
			//The following lines are from LoadBitmap(OnlyIdx), but optimized for the situation when we already have the bitmap
			_arrayDocumentsShowing[_idxSelectedInMount]=document;
			_arraySizesOriginal[_idxSelectedInMount]=sizeBitmap;
			double scale=ZoomSlider.CalcScaleFit(new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
				sizeBitmap,_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated);
			_arrayBitmapsShowing[_idxSelectedInMount]=new Bitmap(bitmap,new Size((int)(sizeBitmap.Width*scale),(int)(sizeBitmap.Height*scale)));
			//_bitmapRaw: don't load this because it would waste time.  Instead, deselect after loop.
			ImageHelper.ApplyColorSettings(_arrayBitmapsShowing[_idxSelectedInMount],
				_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax);
			bitmap?.Dispose();
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listAvail=GetAvailSlots(1);
			if(listAvail is null){//no more available slots, so we are done
				_idxSelectedInMount=-1;
				panelMain.Invalidate();
				_deviceController=null;
				panelAcquire.Visible=false;
				return;
			}
			MountItem mountItem=_listMountItems.FirstOrDefault(x=>x.MountItemNum==listAvail[0].MountItemNum);
			if(mountItem==null){//should not be possible
				_deviceController=null;
				panelAcquire.Visible=false;
				return;
			}
			_idxSelectedInMount=_listMountItems.IndexOf(mountItem);
			panelMain.Invalidate();
			AcquireBitmap();
		}

		private void _deviceController_StatusReceived(object sender, StatusReceivedEventArgs e){
			
		}

		/*
		private void imageSelector_AfterCollapse(object sender,long e) {
			//e is defNum
			//_listExpandedCats.RemoveAll(x => x==e);
			//UpdateUserOdPrefForImageCat(e,false);
		}

		private void imageSelector_AfterExpand(object sender,long e) {
			//e is defNum
			//if(!_listExpandedCats.Contains(e)) {
			//	_listExpandedCats.Add(e);
			//}
			//UpdateUserOdPrefForImageCat(e,true);
		}*/

		private void imageSelector_DragDropImport(object sender,DragDropImportEventArgs e) {
			//We already asked the user to confirm.
			//This part will be silent, without additional popups.
			if(e.ArrayFileNames.Length==0){
				return;
			}
			Document document=null;
			for(int i=0;i<e.ArrayFileNames.Length;i++){
				document=ImageStore.Import(e.ArrayFileNames[i],e.DefNumNew,_patCur);//Makes log
			}
			FillTree(false);
			TypeAndKey typeAndKey=new TypeAndKey(EnumImageNodeType.Document,document.DocNum);
			SelectTreeNode(typeAndKey);
		}

		private void imageSelector_DraggedToCategory(object sender,DragEventArgsOD e) {
			if(e.DocNum!=0){
				//keep in mind that no change as been made to the selection
				Document document=GetDocumentShowing0();
				//Get the document from the database if the selected document is no longer available or is a different document.
				if(document==null || document.DocNum!=e.DocNum) {
					document=Documents.GetByNum(e.DocNum,doReturnNullIfNotFound: true);
					if(document==null) {//The document being moved was deleted by another instance of the program
						MsgBox.Show(this,"Document was previously deleted.");
						return;
					}
				}
				string docOldCat=Defs.GetDef(DefCat.ImageCats,document.DocCategory).ItemName;
				string docNewCat=Defs.GetDef(DefCat.ImageCats,e.DefNumNew).ItemName;
					string logText=Lan.g(this,"Document moved")+": "+document.FileName;
				if(document.Description!="") {
					string docDescript=document.Description;
					if(docDescript.Length>50) {
						docDescript=docDescript.Substring(0,50);
					}
					logText+=" "+Lan.g(this,"with description")+" "+docDescript;
				}
				logText+=" "+Lan.g(this,"from category")+" "+docOldCat+" "+Lan.g(this,"to category")+" "+docNewCat;
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,document.PatNum,logText,document.DocNum,document.DateTStamp);
				Document docOld=document.Copy();
				document.DocCategory=e.DefNumNew;
				Documents.Update(document,docOld);
			}
			else if(e.MountNum!=0){
				string mountOriginalCat=Defs.GetDef(DefCat.ImageCats,_mountShowing.DocCategory).ItemName;
				string mountNewCat=Defs.GetDef(DefCat.ImageCats,e.DefNumNew).ItemName;
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_mountShowing.PatNum,Lan.g(this,"Mount moved from")+" "+mountOriginalCat+" "
					+Lan.g(this,"to")+" "+mountNewCat);
				_mountShowing.DocCategory=e.DefNumNew;
				Mounts.Update(_mountShowing);
			}
			FillTree(true);
		}

		private void imageSelector_ItemDoubleClick(object sender,EventArgs e) {
			timerTreeClick.Enabled=false;
			if(_webBrowser!=null) {
				//This releases control of the current PDF. It was not possible to save in an external PDF viewer unless we clicked on another PDF that would release
				//_webBrowser and control a different PDF. Use in conjunction _webBrowser.Visible=false to not allow the user to click anything on the browser just to be safe.
				_webBrowser.Navigate("about:blank");
				//This prevents users from previewing the PDF in OD at the same time they have it open in an external PDF viewer.
				//There was a strange graphical bug that occurred when the PDF was previewed at the same time the PDF was open in the Adobe Acrobat Reader DC
				//if the Adobe "Enable Protected Mode" option was disabled.  The graphical bug caused many ODButtons and ODGrids to disappear even though their
				//Visible flags were set to true.  Somehow the WndProc() for the form which owned these controls was not calling the OnPaint() method.
				//Thus the bug affected many custom drawn controls.
				_webBrowser.Visible=false;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None
				|| imageSelector.GetSelectedType()==EnumImageNodeType.Category)
			{
				return;
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Images stored directly in database. Export file in order to open with external program.");
				return;//Documents must be stored in the A to Z Folder to open them outside of Open Dental.  Users can use the export button for now.
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount) {
				using FormMountEdit formMountEdit=new FormMountEdit(_mountShowing);
				formMountEdit.ShowDialog();//Edits the MountSelected object directly and updates and changes to the database as well.
				if(formMountEdit.DialogResult==DialogResult.OK){
					FillTree(true);//in case description for the mount changed.
					imageSelector.SetSelected(EnumImageNodeType.Mount,_mountShowing.MountNum);//Need to update _nodeObjTagSelected in case category changed
				}
				return;
			}
			//From here down is Document=================================================================================================
			if(GetDocumentShowing0()==null){
				return;//Unexplained error
			}
			string ext=ImageStore.GetExtension(GetDocumentShowing0());
			if(ext==".jpg" || ext==".jpeg" || ext==".gif") {
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing0());
				formDocInfo.ShowDialog();
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					return;
				}
				FillTree(true);
				imageSelector.SetSelected(EnumImageNodeType.Document,GetDocumentShowing0().DocNum);//Need to update _nodeObjTagSelected in case category changed
				return;
			}
			//We allow anything which ends with a different extention to be viewed in the windows fax viewer.
			//Specifically, multi-page faxes can be viewed more easily by one of our customers using the fax viewer.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				if(ODBuild.IsWeb()) {
					string tempFile=ImageStore.GetFilePath(GetDocumentShowing0(),_patFolder);
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					try {
						string filePath=ImageStore.GetFilePath(GetDocumentShowing0(),_patFolder);
						Process.Start(filePath);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
			else {//Cloud
				//Download document into temp directory for displaying.
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading Document...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(_patFolder.Replace("\\","/")
					,GetDocumentShowing0().FileName
					,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				formProgress.ShowDialog();
				if(formProgress.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
				}
				else {
					string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(GetDocumentShowing0().FileName));
					File.WriteAllBytes(tempFile,state.FileContent);
					if(ODBuild.IsWeb()) {
						ThinfinityUtils.HandleFile(tempFile);
					}
					else {
						Process.Start(tempFile);
					}
				}
			}
		}

		private void imageSelector_SelectionChangeCommitted(object sender,EventArgs e) {
			EnumImageNodeType nodeType=imageSelector.GetSelectedType();
			long priKey=imageSelector.GetSelectedKey();
			TypeAndKey typeAndKey=new TypeAndKey(nodeType,priKey);
			SelectTreeNode(typeAndKey);
		}

		private void _fileSystemWatcher_FileCreated(object sender,FileSystemEventArgs e) {
			//This event fires in a worker thread even though the MS documentation does not say so.
			if(InvokeRequired){
				Invoke(new MethodInvoker(()=>{_fileSystemWatcher_FileCreated(sender,e);}));
				return;
			}
			if(!File.Exists(e.FullPath)) {
				//When saving a file into the directory being watched, windows may perform multiple operations to save the file, resulting in _fileSystemWatcher detecting mulitple events.
				//In this case, two create events are detected by _fileSystemWatcher when savings screenshots using 'Save As', so this method attempts to import one file twice.
				//The first time passing through this method, this file will exist and if it fails to save in open dental, we will get an error message stating so later in the method.
				//The second time, e.FullPath will not exist because we remove the file from the folder being watched later on in this method after we have saved the image in AtoZ and DB.
				//By this logic we can assume that if e.FullPath no longer exists, we have successfully saved the file and can exit out of this method.
				return;
			}
			if(IsMountShowing()){
				if(IsMountItemSelected()){
					//user must have clicked onto an occupied position in the middle of a series.
					panelImportAuto.Visible=false;
					if(_fileSystemWatcher!=null){
						_fileSystemWatcher.EnableRaisingEvents=false;
					}
					MessageBox.Show(Lan.g(this,"Cannot import to occupied mount position."));
					return;
				}
				if(_idxSelectedInMount==-1){
					//user must have clicked outside mount to deselect, or else we are out of spots
					panelImportAuto.Visible=false;
					if(_fileSystemWatcher!=null){
						_fileSystemWatcher.EnableRaisingEvents=false;
					}
					MessageBox.Show(Lan.g(this,"No mount position selected."));
					return;
				}
			}
			bool isFileAvailable=false;
			int maxAttempts=30;//try for 3 seconds
			var attemptsMade=0;
			while(!isFileAvailable && attemptsMade<=maxAttempts){
				try{
					using (File.Open(e.FullPath, FileMode.Open, FileAccess.ReadWrite)){
						isFileAvailable = true;
					}
				}
				catch{

				}
				attemptsMade++;
				Thread.Sleep(100);
			}
			Document doc = null;
			try{
				doc=ImageStore.Import(e.FullPath,GetCurrentCategory(),_patCur);
			}
			catch(Exception ex) {
				panelImportAuto.Visible=false;
				_fileSystemWatcher.EnableRaisingEvents=false;//will never be null
				MessageBox.Show(Lan.g(this,"Unable to import ")+e.FullPath+": "+ex.Message);
				return;
			}
			if(!IsMountShowing()){//single
				File.Delete(e.FullPath);
				FillTree(false);//Reload and keep new document selected.
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				return;//user can take another single
			}
			//From here down is mount-----------------------------------------------------------------------------
			//already verified idxSelected
			Document docOld=doc.Copy();
			doc.MountItemNum=_listMountItems[_idxSelectedInMount].MountItemNum;
			doc.DegreesRotated=_listMountItems[_idxSelectedInMount].RotateOnAcquire;
			doc.ToothNumbers=_listMountItems[_idxSelectedInMount].ToothNumbers;
			Documents.Update(doc,docOld);
			//The following lines are from LoadBitmap(OnlyIdx), but optimized for the situation when we already have the bitmap
			_arrayDocumentsShowing[_idxSelectedInMount]=doc;
			Bitmap bitmap=(Bitmap)Bitmap.FromFile(e.FullPath);
			_arraySizesOriginal[_idxSelectedInMount]=bitmap.Size;
			double scale=ZoomSlider.CalcScaleFit(new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),
				bitmap.Size,_arrayDocumentsShowing[_idxSelectedInMount].DegreesRotated);
			_arrayBitmapsShowing[_idxSelectedInMount]=new Bitmap(bitmap,new Size((int)(bitmap.Width*scale),(int)(bitmap.Height*scale)));
			//_bitmapRaw: don't load this because it would waste time.  Instead, deselect after loop.
			ImageHelper.ApplyColorSettings(_arrayBitmapsShowing[_idxSelectedInMount],
				_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin,_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax);
			bitmap?.Dispose();
			File.Delete(e.FullPath);
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listAvail=GetAvailSlots(1);
			if(listAvail is null){//no more available slots, so we are done
				_idxSelectedInMount=-1;
				panelMain.Invalidate();
				panelImportAuto.Visible=false;
				_fileSystemWatcher.EnableRaisingEvents=false;//will never be null
				MessageBox.Show(Lan.g(this,"No more available mount positions."));
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

		private void formVideo_BitmapCaptured(object sender, VideoEventArgs e){
			Document doc = null;
			try {
				//it seems to sometimes fire a second time, with bitmap null
				if(e.Bitmap is null){
					return;
				}
				doc=ImageStore.Import(e.Bitmap,GetCurrentCategory(),ImageType.Photo,_patCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save")+": "+ex.Message);
				return;
			}
			if(!IsMountShowing()){//single
				FillTree(false);//Reload and keep new document selected.
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum));
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
			_arrayBitmapsShowing[_idxSelectedInMount]=new Bitmap(e.Bitmap,new Size((int)(e.Bitmap.Width*scale),(int)(e.Bitmap.Height*scale)));
			//_bitmapRaw: don't load this because it would waste time.  Instead, deselect after loop.
			ImageHelper.ApplyColorSettings(_arrayBitmapsShowing[_idxSelectedInMount],
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

		private void menuForms_Click(object sender,System.EventArgs e) {
			string formName=((MenuItem)sender).Text;
			Document doc;
			try {
				doc=ImageStore.ImportForm(formName,GetCurrentCategory(),_patCur);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FillTree(false);
			SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum));
			using FormDocInfo FormD=new FormDocInfo(_patCur,doc,isDocCreate:true);
			FormD.ShowDialog(this);//some of the fields might get changed, but not the filename
			if(FormD.DialogResult!=DialogResult.OK) {
				DeleteDocument(false,false,doc);
			}
			else {
				FillTree(true);//Refresh possible changes in the document due to FormD.
			}	
		}

		private void menuMounts_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}	
			int idx = ((MenuItem)sender).Index;
			List<MountDef> listMountDefs=MountDefs.GetDeepCopy();
			//MsgBox.Show(listMountDefs[idx].Description);
			Mount mount=Mounts.CreateMountFromDef(listMountDefs[idx],_patCur.PatNum,GetCurrentCategory());
			Def defDocCategory=Defs.GetDef(DefCat.ImageCats,mount.DocCategory);
			string logText="Mount Created: "+mount.Description+" with catgegory "
				+defDocCategory.ItemName;
			SecurityLogs.MakeLogEntry(Permissions.ImageCreate,_patCur.PatNum,logText);
			FillTree(false);
			SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
		}

		private void menuTree_Click(object sender,System.EventArgs e) {
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None || imageSelector.GetSelectedType()==EnumImageNodeType.Category){
				return;//Probably the user has no patient selected
			}
			//Categories mostly blocked at the click
			switch(((MenuItem)sender).Index) {
				case 0://print
					ToolBarPrint_Click();
					break;
				case 1://delete
					ToolBarDelete_Click();
					break;
				case 2://info
					ToolBarInfo_Click();
					break;
			}
		}

		private void menuPanelMain_Click(object sender,System.EventArgs e) {
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None){
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
						progressOD.ActionMain=() => doc=ImageStore.Import(stringArrayFiles[i],GetCurrentCategory(),_patCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						doc=ImageStore.Import(stringArrayFiles[i],GetCurrentCategory(),_patCur);//Makes log
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
			SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
		}

		private void panelMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(IsMountShowing()){
				if(_idxSelectedInMount==-1){
					using FormMountEdit formMountEdit=new FormMountEdit(_mountShowing);
					formMountEdit.ShowDialog();
					if(formMountEdit.DialogResult==DialogResult.OK){
						FillTree(true);
					}
				}
				else{//mount item
					if(_arrayDocumentsShowing[_idxSelectedInMount]!=null){
						using FormDocInfo formDocInfo=new FormDocInfo(_patCur,_arrayDocumentsShowing[_idxSelectedInMount]);
						formDocInfo.IsMountItem=true;
						formDocInfo.ShowDialog();
						//nothing to refresh
					}
				}
			}
			else{
				if(GetDocumentShowing0()!=null) {
					using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing0());
					formDocInfo.ShowDialog();
					if(formDocInfo.DialogResult==DialogResult.OK){
						FillTree(true);
					}
				}
			}
		}

		private void panelMain_MouseDown(object sender, MouseEventArgs e){
			if(IsDocumentShowing() && _cropPanEditAdj==EnumCropPanAdj.Crop) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[0].DateCreated)) {
					return;
				}
			}
			for(int i=0;i<menuMainPanel.MenuItems.Count;i++) {
				//If no document or mount selected, context menu will not be visible
				if(IsDocumentShowing() || IsMountShowing()) {
					menuMainPanel.MenuItems[i].Visible=true;
				}
				else{ 
				//if(!IsDocumentShowing() && !IsMountShowing()) {
					menuMainPanel.MenuItems[i].Visible=false;//if no menu items visible, menu won't even show
				}
			}
			_pointMouseDown=e.Location;  
			_isMouseDownPanel=true;
			_dateTimeMouseDownPanel=DateTime.Now;
			_pointTranslationOld=_pointTranslation;
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
				EnableToolBarButtonsMount();
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
			//if(_panCropMount==EnumPanCropMount.Pan/Edit){
				//handled on mouse up
			//}
			if(_cropPanEditAdj==EnumCropPanAdj.Adj){
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
				EnableToolBarButtonsMount();
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
			if(!_isMouseDownPanel) {
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None){
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Category){
				return;
			}
			float scaleTrans=(float)100/zoomSlider.Value;//example, 200%, 100/200=.5, indicating .5 bitmap pixels for each screen pixel
			if(_cropPanEditAdj==EnumCropPanAdj.Crop) {
				Rectangle rectangle1=new Rectangle(Math.Min(e.Location.X,_pointMouseDown.X), Math.Min(e.Location.Y,_pointMouseDown.Y),
					Math.Abs(e.Location.X-_pointMouseDown.X),	Math.Abs(e.Location.Y-_pointMouseDown.Y));
				Rectangle rectangle2=new Rectangle(0,0,panelMain.Width-1,panelMain.Height-1);
				_rectangleCrop=Rectangle.Intersect(rectangle1,rectangle2);
				panelMain.Invalidate();
			}
			if(_cropPanEditAdj==EnumCropPanAdj.Pan){
				//scaleTrans=1;
				int xTrans=(int)(_pointTranslationOld.X+(e.Location.X-_pointMouseDown.X)*scaleTrans);
				int yTrans=(int)(_pointTranslationOld.Y+(e.Location.Y-_pointMouseDown.Y)*scaleTrans);
				_pointTranslation=new Point(xTrans,yTrans);
				panelMain.Invalidate();
			}
			if(_cropPanEditAdj==EnumCropPanAdj.Adj){
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
			_isMouseDownPanel=false;
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None){
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Category){
				return;
			}
			bool isClick=false;
			if(Math.Abs(e.Location.X-_pointMouseDown.X) <3 
				&& Math.Abs(e.Location.Y-_pointMouseDown.Y) <3
				&& _dateTimeMouseDownPanel.AddMilliseconds(600) > DateTime.Now)//anything longer than a 600 ms mouse down becomes a drag
			{
				isClick=true;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount && isClick	&& _cropPanEditAdj==EnumCropPanAdj.Pan){//Adj is handled on mouse down 
				//user clicked on a mount item to highlight it
				int idxSelectedInMountOld=_idxSelectedInMount;
				_idxSelectedInMount=HitTestInMount(e.X,e.Y);
				if(_idxSelectedInMount!=idxSelectedInMountOld){
					LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.OnlyRaw);
				}
				EnableToolBarButtonsMount();
				_isDraggingMount=false;
				panelMain.Invalidate();
				SetWindowingSlider();
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount && !isClick && _cropPanEditAdj==EnumCropPanAdj.Adj && _isDraggingMount){
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
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
				panelMain.Invalidate();
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount && _cropPanEditAdj==EnumCropPanAdj.Adj){
				//catches rest of situations not addressed above for mounts
				_isDraggingMount=false;
				SetWindowingSlider();
				panelMain.Invalidate();
			}
			if(_cropPanEditAdj==EnumCropPanAdj.Crop){
				if(_rectangleCrop.Width<=0 || _rectangleCrop.Height<=0) {
					return;
				}
				//these two rectangles are in bitmap coords rather then screen
				Rectangle rectangleBitmap;
				if(GetDocumentShowing0().CropW>0 && GetDocumentShowing0().CropH>0){
					rectangleBitmap=new Rectangle(GetDocumentShowing0().CropX,GetDocumentShowing0().CropY,GetDocumentShowing0().CropW,GetDocumentShowing0().CropH);
				}
				else{
					rectangleBitmap=new Rectangle(0,0,GetBitmapShowing0().Width,GetBitmapShowing0().Height);
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
				Document docOld=GetDocumentShowing0().Copy();
				GetDocumentShowing0().CropX=rectangleCrop.X;
				GetDocumentShowing0().CropY=rectangleCrop.Y;
				GetDocumentShowing0().CropW=rectangleCrop.Width;
				GetDocumentShowing0().CropH=rectangleCrop.Height;
				Documents.Update(GetDocumentShowing0(),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing0(),_patFolder);
				LoadBitmap(0,EnumLoadBitmapType.IdxAndRaw);
				SetZoomSlider();
				_pointTranslation=new Point();
				_rectangleCrop=new Rectangle();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[0].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[0].FileName
					+" with catgegory "+defDocCategory.ItemName+" was changed using the Crop button";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
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
			if(GetDocumentShowing0()==null && _mountShowing==null){
				return;
			}
			//Center screen:
			g.TranslateTransform(panelMain.Width/2,panelMain.Height/2);
			//because of order, scaling is center of panel instead of center of image.
			float scaleFactor=zoomSlider.Value/100f;
			g.ScaleTransform(scaleFactor,scaleFactor);
			//and the user translation must be in image coords rather than panel coords
			g.TranslateTransform(_pointTranslation.X,_pointTranslation.Y);
			try{
				if(IsDocumentShowing() && GetBitmapShowing0()!=null){
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
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None) {
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Category) {
				return;
			}
			if(_cropPanEditAdj==EnumCropPanAdj.Pan){
				float deltaZoom=zoomSlider.Value*(float)e.Delta/SystemInformation.MouseWheelScrollDelta/8f;//For example, -15
				zoomSlider.SetByWheel(deltaZoom);
				panelMain.Invalidate();
			}
			if(_cropPanEditAdj==EnumCropPanAdj.Adj){
				//no
			}
		}

		private void panelSplitter_MouseDown(object sender,MouseEventArgs e) {
			_pointMouseDown=Control.MousePosition;
			_widthTree96StartDrag=_widthTree96;
			int yCenter=panelSplitter.Height/2;
			int buttonHeight=25;
			if(e.Y<yCenter-buttonHeight/2 || e.Y>yCenter+buttonHeight/2){
				return;
			}
			//clicked on the triangle
			_isTreeDockerCollapsing=true;//this won't toggle back until mouse up
			if(_isTreeDockerCollapsed){
				_isTreeDockerCollapsed=false;
			}
			else{
				_isTreeDockerCollapsed=true;
			}
			LayoutControls();
			panelSplitter.Invalidate();
		}

		private void panelSplitter_MouseLeave(object sender,EventArgs e) {
			Cursor=Cursors.Default;
		}
		
		private void panelSplitter_MouseMove(object sender,MouseEventArgs e) {
			int yCenter=panelSplitter.Height/2;
			int buttonHeight=25;
			if(e.Y<yCenter-buttonHeight/2 || e.Y>yCenter+buttonHeight/2){
				Cursor=Cursors.SizeWE;
			}
			else{
				Cursor=Cursors.Default;
			}
			if(Control.MouseButtons!=MouseButtons.Left){
				return;
			}
			#region Dragging
			if(_isTreeDockerCollapsing){
				return;
			}
			if(_isTreeDockerCollapsed){
				_isTreeDockerCollapsed=false;//undock
				_widthTree96StartDrag=0;
				panelSplitter.Invalidate();
			}
			Point pointDelta =new Point(Control.MousePosition.X-_pointMouseDown.X,Control.MousePosition.Y-_pointMouseDown.Y);
			float xDelta96=LayoutManager.UnscaleF(pointDelta.X);
			float widthNew=_widthTree96StartDrag+xDelta96;
			if(widthNew<0){
				widthNew=0;
			}
			if(widthNew>500){
				widthNew=500;
			}
			_widthTree96=widthNew;
			LayoutControls();
			#endregion Dragging
		}

		private void panelSplitter_MouseUp(object sender,MouseEventArgs e) {
			_isTreeDockerCollapsing=false;
			Point pointDelta =new Point(Control.MousePosition.X-_pointMouseDown.X,Control.MousePosition.Y-_pointMouseDown.Y);
			if(pointDelta.X<0 && _widthTree96==0 && !_isTreeDockerCollapsed){
				_isTreeDockerCollapsed=true;//auto collapse
				_widthTree96=_widthTree96StartDrag;//remember the width before the drag
				LayoutControls();
				panelSplitter.Invalidate();
			}
		}

		private void panelSplitter_Paint(object sender,PaintEventArgs e) {
			e.Graphics.SmoothingMode=SmoothingMode.HighQuality;
			PointF[] points=new PointF[3];
			float lengthSide=15;//LayoutManager.ScaleF(15f);
			float xCenter=panelSplitter.Width/2f;
			float yCenter=panelSplitter.Height/2f;
			if(_isTreeDockerCollapsed){
				xCenter+=0.5f;
				points[0]=new PointF(xCenter-lengthSide/4,yCenter-lengthSide/2);//top
				points[1]=new PointF(xCenter-lengthSide/4,yCenter+lengthSide/2);//bottom
				points[2]=new PointF(xCenter+lengthSide/4,yCenter);//right
			}
			else{
				xCenter-=1f;
				points[0]=new PointF(xCenter+lengthSide/4,yCenter-lengthSide/2);//top
				points[1]=new PointF(xCenter+lengthSide/4,yCenter+lengthSide/2);//bottom
				points[2]=new PointF(xCenter-lengthSide/4,yCenter);//left
			}
			e.Graphics.Clear(ColorOD.Control);
			using SolidBrush brush=new SolidBrush(ColorOD.Gray(70));
			e.Graphics.FillPolygon(brush,points);
		}

		private void printDocument_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Graphics g=e.Graphics;
			g.InterpolationMode=InterpolationMode.HighQualityBicubic;
			g.CompositingQuality=CompositingQuality.HighQuality;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.PixelOffsetMode=PixelOffsetMode.HighQuality;
			Bitmap bitmapPrint=null;
			if(IsDocumentShowing()){
				bitmapPrint=(Bitmap)GetBitmapShowing0().Clone();
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
					bitmapTemp=ImageStore.OpenImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);
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
				str=_patCur.GetNameLF();
				widthStr=(int)g.MeasureString(str,fontTitle).Width;
				g.DrawString(str,fontTitle,Brushes.Black,xTitle-widthStr/2,yTitle);
				yTitle+=fontTitle.Height+4;
				str="DOB: "+_patCur.Birthdate.ToShortDateString();
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
			str=_patCur.GetNameLF();
			widthStr=(int)g.MeasureString(str,fontTitle).Width;
			g.DrawString(str,fontTitle,Brushes.Black,xTitle-widthStr/2,yTitle);
			yTitle+=fontTitle.Height+4;
			str="DOB: "+_patCur.Birthdate.ToShortDateString();
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

		private void timerTreeClick_Tick(object sender, EventArgs e){
			//this timer starts when user clicks on a treenode that's already selected.
			//It gets cancelled if the click turns into a double click.
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None){
				return;
			}
			TypeAndKey typeAndKey=new TypeAndKey(imageSelector.GetSelectedType(),imageSelector.GetSelectedKey()); 
			timerTreeClick.Enabled=false;//only fire once
			SelectTreeNode(typeAndKey);
		}

		private void toolBarMain_ButtonClick(object sender, ODToolBarButtonClickEventArgs e){
			if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,_patCur);
				return;
			}
			if(e.Button.Tag.GetType()!=typeof(TB)) {
				return;//This seems to happen frequently. Maybe they are clicking between buttons.
			}
			switch((TB)e.Button.Tag) {
				case TB.Print:
					//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
					//when it comes from a toolbar click.
					//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
					//ToolBarClick toolClick=ToolBarPrint_Click;
					this.BeginInvoke(ToolBarPrint_Click);//toolClick);
					break;
				case TB.Delete:
					ToolBarDelete_Click();
					break;
				case TB.Info:
					ToolBarInfo_Click();
					break;
				case TB.Sign:
					ToolBarSign_Click();
					break;
				case TB.ScanDoc:
					ToolBarScan_Click("doc");
					break;
				case TB.ScanMultiDoc:
					ToolBarScanMulti_Click();
					break;
				case TB.ScanXRay:
					ToolBarScan_Click("xray");
					break;
				case TB.ScanPhoto:
					ToolBarScan_Click("photo");
					break;
				case TB.Acquire:
					ToolBarAcquire_Click();
					break;
				case TB.Video:
					ToolBarVideo_Click();
					break;
				case TB.Import://import is always enabled
					ToolBarImport_Click();
					break;
				case TB.Export:
					ToolBarExport_Click();
					break;
				case TB.Copy:
					ToolBarCopy_Click();
					break;
				case TB.Paste:
					ToolBarPaste_Click();
					break;
				case TB.Forms:
					MsgBox.Show(this,"Use the dropdown list.  Add forms to the list by copying image files into your A-Z folder, Forms.  Restart the program to see newly added forms.");
					break;
				case TB.Mounts:
					MsgBox.Show(this,"Use the dropdown list.  Manage Mounts from the Setup/Imaging menu.");
					break;
			}
		}

		private void toolBarPaint_ButtonClick(object sender, ODToolBarButtonClickEventArgs e){
			if(e.Button.Tag.GetType()!=typeof(TB)) {
				return;
			}
			switch((TB)e.Button.Tag) {
				case TB.ZoomOne:
					ToolBarZoomOne_Click();
					break;
				case TB.Crop:
					SetCropPanEditAdj(EnumCropPanAdj.Crop);
					break;
				case TB.Pan:
					SetCropPanEditAdj(EnumCropPanAdj.Pan);
					break;
				case TB.Adj:
					SetCropPanEditAdj(EnumCropPanAdj.Adj);
					break;
				case TB.Size:
					ToolBarSize_Click();
					break;
				case TB.Flip:
					ToolBarFlip_Click();
					break;
				case TB.RotateL:
					ToolBarRotateL_Click();
					break;
				case TB.RotateR:
					ToolBarRotateR_Click();
					break;
				case TB.Rotate180:
					ToolBarRotate180_Click();
					break;
			}
		}

		private void treeMain_MouseUp(object sender,MouseEventArgs e) {
			/*
			_isMouseDownTree=false;
			treeMain.Cursor=Cursors.Default;
			if(_nodeObjTagDragging==null) {//
				return;
			}
			//Dragging should only happen with the left mouse button
			if(e.Button!=MouseButtons.Left) {
				return;
			}
			//Compare tree category information
			TreeNode treeNodeNewCat=treeMain.GetNodeAt(e.Location);
			TreeNode treeNodeOldCat=GetTreeNodeByKey(_nodeObjTagDragging);
			if(treeNodeNewCat==null || treeNodeOldCat==null) {
				return;
			}
			NodeObjTag nodeObjTagNewCat=(NodeObjTag)treeNodeNewCat.Tag;
			long nodeNewCatDefNum=0;
			long nodeOldCatDefNum=0;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			if(nodeObjTagNewCat.NodeType==EnumImageNodeType.Category) {
				nodeNewCatDefNum=listDefs[treeNodeNewCat.Index].DefNum;
			}
			else {
				nodeNewCatDefNum=listDefs[treeNodeNewCat.Parent.Index].DefNum;
			}
			nodeOldCatDefNum=listDefs[treeNodeOldCat.Parent.Index].DefNum;
			//If we try to move a category or if the node has not moved categories then return
			if(_nodeObjTagDragging.NodeType==EnumImageNodeType.Category || nodeOldCatDefNum==nodeNewCatDefNum) {
				return;
			}
			if(_nodeObjTagDragging.NodeType==EnumImageNodeType.Mount) {
				Mount mount=Mounts.GetByNum(_nodeObjTagDragging.MountNum);
				string mountOriginalCat=Defs.GetDef(DefCat.ImageCats,mount.DocCategory).ItemName;
				string mountNewCat=Defs.GetDef(DefCat.ImageCats,nodeNewCatDefNum).ItemName;
				mount.DocCategory=nodeNewCatDefNum;
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,mount.PatNum,Lan.g(this,"Mount moved from")+" "+mountOriginalCat+" "
					+Lan.g(this,"to")+" "+mountNewCat);
				Mounts.Update(mount);
			}
			else {
				Document document=Documents.GetByNum(_nodeObjTagDragging.DocNum);
				string docOldCat=Defs.GetDef(DefCat.ImageCats,document.DocCategory).ItemName;
				string docNewCat=Defs.GetDef(DefCat.ImageCats,nodeNewCatDefNum).ItemName;
				document.DocCategory=nodeNewCatDefNum;
				string logText=Lan.g(this,"Document moved")+": "+document.FileName;
				if(document.Description!="") {
					string docDescript=document.Description;
					if(docDescript.Length>50) {
						docDescript=docDescript.Substring(0,50);
					}
					logText+=" "+Lan.g(this,"with description")+" "+docDescript;
				}
				logText+=" "+Lan.g(this,"from category")+" "+docOldCat+" "+Lan.g(this,"to category")+" "+docNewCat;
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,document.PatNum,logText,document.DocNum,document.DateTStamp);
				Documents.Update(document);
			}
			FillTree(true);
			_nodeObjTagDragging=null;*/
		}

		private void windowingSlider_Scroll(object sender,EventArgs e) {
			if(IsDocumentShowing()) {
				GetDocumentShowing0().WindowingMin=windowingSlider.MinVal;
				GetDocumentShowing0().WindowingMax=windowingSlider.MaxVal;
				InvalidateSettingsColor();
			}
			if(IsMountItemSelected()) {
				_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin=windowingSlider.MinVal;
				_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax=windowingSlider.MaxVal;
				InvalidateSettingsColor();
			}
		}

		private void windowingSlider_ScrollComplete(object sender,EventArgs e) {
			if(IsDocumentShowing()) {
				Documents.Update(GetDocumentShowing0());
				ImageStore.DeleteThumbnailImage(GetDocumentShowing0(),_patFolder);
				InvalidateSettingsColor();
			}
			if(IsMountItemSelected()) {
				Documents.Update(_arrayDocumentsShowing[_idxSelectedInMount]);
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);
				InvalidateSettingsColor();
			}
		}

		private void zoomSlider_FitPressed(object sender, EventArgs e){
			_pointTranslation=new Point(0,0);
			panelMain.Invalidate();
		}

		private void zoomSlider_Zoomed(object sender, EventArgs e){
			panelMain.Invalidate();
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>Gets called repeatedly during a series.  This sometimes contains blocking calls. If something goes wrong, then it sets _deviceController to null and closes the panel.  If there are no more slots available, this will not acquire a bitmap.</summary>
		private void AcquireBitmap(){
			//ImagingDevice	imagingDevice=comboDevice.GetSelected<ImagingDevice>();	
			try{
				_deviceController.AcquireBitmap();
			}
			catch(ImagingDeviceException e){
				//Exceptions are currently only thrown from Twain. XDR raises events instead.
				if(e.Message!=""){//If user cancels, there is no text.  We could test e.DeviceStatus instead
					MessageBox.Show(e.Message);
				}
				//no bitmap to dispose of
				_deviceController=null;
				panelAcquire.Visible=false;
				return;
			}
		}

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
			SetDocumentShowing0(null);
			_mountShowing=null;
			panelMain.Invalidate();
		}

		///<summary>Converts a point in panelMain into a point in _bitmapShowing.  If mount, then it's coords within entire mount.</summary>
		private Point ControlPointToBitmapPoint(Point pointPanel) {
			Matrix matrix=new Matrix();
			matrix.Translate(-panelMain.Width/2,-panelMain.Height/2,MatrixOrder.Append);
			matrix.Scale(1f/zoomSlider.Value*100f,1f/zoomSlider.Value*100f,MatrixOrder.Append);//1 if no scale
			matrix.Translate(-_pointTranslation.X,-_pointTranslation.Y,MatrixOrder.Append);
			//We are now at center image.
			if(IsMountShowing()){
				//no rotation or flip
				matrix.Translate(_mountShowing.Width/2f,_mountShowing.Height/2f,MatrixOrder.Append);
			}
			if(IsDocumentShowing()){
				matrix.Rotate(-GetDocumentShowing0().DegreesRotated,MatrixOrder.Append);
				if(GetDocumentShowing0().IsFlipped){
					Matrix matrixFlip=new Matrix(-1,0,0,1,0,0);
					matrix.Multiply(matrixFlip,MatrixOrder.Append);
				}
				if(GetDocumentShowing0().CropW>0 && GetDocumentShowing0().CropH>0){
					matrix.Translate(GetDocumentShowing0().CropW/2f,GetDocumentShowing0().CropH/2f,MatrixOrder.Append);//back to UL of cropped area
					matrix.Translate(GetDocumentShowing0().CropX,GetDocumentShowing0().CropY,MatrixOrder.Append);//then back to 00 of image
					//We can't really clip here.  We could test below, just before return, or caller could test point.
				}
				else{
					matrix.Translate(GetBitmapShowing0().Width/2f,GetBitmapShowing0().Height/2f,MatrixOrder.Append);
				}
			}
			Point[] points={pointPanel };
			matrix.TransformPoints(points);
			return points[0];
		}

		public static EnumImgDeviceControlType ConvertToImgDeviceControlType(EnumImgDeviceType imgDeviceType){
			switch(imgDeviceType){
				default:
				case EnumImgDeviceType.TwainRadiograph:
					return EnumImgDeviceControlType.Twain;
				case EnumImgDeviceType.XDR:
					return EnumImgDeviceControlType.XDR;
			}
		}

		///<summary>Disposes and recreates a new webBrowser control that is hidden.</summary>
		private void ClearPDFBrowser() {
			if(_webBrowser!=null) {
				_webBrowser.Dispose();
				_webBrowser=null;//In order to release the pdf lock on this object, we have to dispose of the webBrowser.
				_webBrowser=new WebBrowser();
				_webBrowser.Bounds=panelMain.Bounds;
				LayoutManager.Add(_webBrowser,this);
				_webBrowser.Visible=false;
			}
		}


		///<summary>Deletes the specified document from the database and refreshes the tree view. Set securityCheck false when creating a new document that might get cancelled.  Document is passed in because it might not be in the tree if the image folder it belongs to is now hidden.</summary>
		private void DeleteDocument(bool isVerbose,bool doSecurityCheck,Document document) {
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
				ImageStore.DeleteDocuments(docs,_patFolder);
			}
			catch(Exception ex) {  //Image could not be deleted, in use.
				MessageBox.Show(this,ex.Message);
			}
			if(IsDocumentShowing()){
				FillTree(false);
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

		///<summary></summary>
		private void DrawDocument(Graphics g){
			//we're at the center of the image, and working in image coordinates
			g.RotateTransform(GetDocumentShowing0().DegreesRotated);
			if(GetDocumentShowing0().IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Make our 0,0 reference point be the center of the portion of the image that will show:
			if(GetDocumentShowing0().CropW>0 && GetDocumentShowing0().CropH>0){
				g.TranslateTransform(-GetDocumentShowing0().CropW/2,-GetDocumentShowing0().CropH/2);//back to UL of cropped area
				g.TranslateTransform(-GetDocumentShowing0().CropX,-GetDocumentShowing0().CropY);//then back to 00 of image
				g.SetClip(new Rectangle(GetDocumentShowing0().CropX,GetDocumentShowing0().CropY,GetDocumentShowing0().CropW,GetDocumentShowing0().CropH));
			}
			else{
				g.TranslateTransform(-GetBitmapShowing0().Width/2,-GetBitmapShowing0().Height/2);//back to UL corner 00 of image
			}
			g.DrawImage(GetBitmapShowing0(),0,0);
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
			//yellow outline of selected
			if(_idxSelectedInMount!=-1){
				g.DrawRectangle(Pens.Yellow,_listMountItems[_idxSelectedInMount].Xpos,_listMountItems[_idxSelectedInMount].Ypos,
					_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height);
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
					g.DrawImage(_arrayBitmapsShowing[i],0,0);
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
					g.DrawImage(_arrayBitmapsShowing[i],0,0);
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

		///<summary>Enables or disables all the buttons in both toolbars to handle situation where no patient is selected.</summary>
		private void EnableToolBarsPatient(bool enable) {
			for(int i=0;i<toolBarMain.Buttons.Count;i++) {
				toolBarMain.Buttons[i].Enabled=enable;
			}
			//Acquire button?
			toolBarMain.Invalidate();
			for(int i=0;i<toolBarPaint.Buttons.Count;i++) {
				toolBarPaint.Buttons[i].Enabled=enable;
			}
			toolBarPaint.Invalidate();
			windowingSlider.Enabled=enable;
			zoomSlider.Enabled=enable;
			windowingSlider.Invalidate();
		}

		///<summary>Not technically all.  There are some buttons that we never disable such as import, scan, acquire, etc.</summary>
		private void EnableAllToolBarButtons(bool enable) {
			EnableToolBarButtons(print:enable, delete:enable, info:enable, sign:enable, export:enable, copy:enable, brightAndContrast:enable, zoom:enable, zoomOne:enable, crop:enable, pan:enable, adj:enable, size:enable, flip:enable, rotateL:enable, rotateR:enable, rotate180:enable);
		}

		///<summary>This is the same thing from many places, so it's centralized.</summary>
		private void EnableToolBarButtonsMount(){
			if(_idxSelectedInMount==-1){//entire mount
				EnableToolBarButtons(print:true, delete:true, info:true, sign:false, export:true, copy:true, brightAndContrast:false, zoom:true, zoomOne:false, crop:false, pan:true, adj:true, size:false, flip:false, rotateL:false, rotateR:false, rotate180:false);
			}
			else{//individual image
				EnableToolBarButtons(print:true, delete:true, info:true, sign:false, export:true, copy:true, brightAndContrast:true, zoom:true, zoomOne:true, crop:false, pan:true, adj:true, size:true, flip:true, rotateL:true, rotateR:true, rotate180:true);
			}
		}

		///<summary>Defined this way to force future programming to consider which tools are enabled and disabled for every possible tool in the menu.  To prevent bugs, you must always use named arguments.  Called when users clicks on Crop/Pan/Mount buttons, clicks Tree, or clicks around on a mount.</summary>
		private void EnableToolBarButtons(bool print, bool delete, bool info, bool sign, bool export, bool copy, bool brightAndContrast, bool zoom, bool zoomOne, bool crop, bool pan, bool adj, bool size, bool flip, bool rotateL,bool rotateR, bool rotate180) {
			//Some buttons don't show here because they are always enabled as long as there is a patient,
			//including Scan, Import, Paste, Templates, Mounts
			toolBarMain.Buttons[TB.Print.ToString()].Enabled=print;
			toolBarMain.Buttons[TB.Delete.ToString()].Enabled=delete;
			toolBarMain.Buttons[TB.Info.ToString()].Enabled=info;
			toolBarMain.Buttons[TB.Sign.ToString()].Enabled=sign;
			toolBarMain.Buttons[TB.Export.ToString()].Enabled=export;
			toolBarMain.Buttons[TB.Copy.ToString()].Enabled=copy;
			toolBarMain.Invalidate();
			windowingSlider.Enabled=brightAndContrast;
			windowingSlider.Invalidate();
			zoomSlider.Enabled=zoom;
			toolBarPaint.Buttons[TB.ZoomOne.ToString()].Enabled=zoomOne;
			toolBarPaint.Buttons[TB.Crop.ToString()].Enabled=crop;
			toolBarPaint.Buttons[TB.Pan.ToString()].Enabled=pan;
			toolBarPaint.Buttons[TB.Adj.ToString()].Enabled=adj;
			toolBarPaint.Buttons[TB.Size.ToString()].Enabled=size;
			toolBarPaint.Buttons[TB.Flip.ToString()].Enabled=flip;
			toolBarPaint.Buttons[TB.RotateR.ToString()].Enabled=rotateR;
			toolBarPaint.Buttons[TB.RotateL.ToString()].Enabled=rotateL;
			toolBarPaint.Buttons[TB.Rotate180.ToString()].Enabled=rotate180;
			toolBarPaint.Invalidate();
			//toolBarMount buttons are always visible
		}

		///<summary>Fills the panelnote control with the current document signature when the panelnote is visible and when a valid document is currently selected.</summary>
		private void FillSignature() {
			if(!IsDocumentShowing()){
				return;
			}
			textNote.Text="";
			sigBox.ClearTablet();
			if(!panelNote.Visible) {
				return;
			}
			textNote.Text=GetDocumentShowing0().Note;
			labelInvalidSig.Visible=false;
			sigBox.Visible=true;
			sigBox.SetTabletState(0);//never accepts input here
			//Topaz box is not supported in Unix, since the required dll is Windows native.
			if(GetDocumentShowing0().SigIsTopaz) {
				if(GetDocumentShowing0().Signature!=null && GetDocumentShowing0().Signature!="") {
					//if(allowTopaz) {	
					sigBox.Visible=false;
					_sigBoxTopaz.Visible=true;
					TopazWrapper.ClearTopaz(_sigBoxTopaz);
					TopazWrapper.SetTopazCompressionMode(_sigBoxTopaz,0);
					TopazWrapper.SetTopazEncryptionMode(_sigBoxTopaz,0);
					TopazWrapper.SetTopazKeyString(_sigBoxTopaz,"0000000000000000");//Clear out the key string
					string keystring=GetHashString(GetDocumentShowing0());
					TopazWrapper.SetTopazAutoKeyData(_sigBoxTopaz,keystring);
					TopazWrapper.SetTopazEncryptionMode(_sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazCompressionMode(_sigBoxTopaz,2);//high compression
					TopazWrapper.SetTopazSigString(_sigBoxTopaz,GetDocumentShowing0().Signature);
					_sigBoxTopaz.Refresh();
					//If sig is not showing, then setting the Key String to the hashed data. This is the way we used to handle signatures.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						TopazWrapper.SetTopazKeyString(_sigBoxTopaz,"0000000000000000");//Clear out the key string
						TopazWrapper.SetTopazKeyString(_sigBoxTopaz,keystring);
						TopazWrapper.SetTopazSigString(_sigBoxTopaz,GetDocumentShowing0().Signature);
					}
					//If sig is not showing, then try encryption mode 3 for signatures signed with old SigPlusNet.dll.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						TopazWrapper.SetTopazEncryptionMode(_sigBoxTopaz,3);//Unknown mode (told to use via TopazSystems)
						TopazWrapper.SetTopazSigString(_sigBoxTopaz,GetDocumentShowing0().Signature);
					}
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						labelInvalidSig.Visible=true;
					}
					//}
				}
			}
			else {//not topaz
				if(GetDocumentShowing0().Signature!=null && GetDocumentShowing0().Signature!="") {
					sigBox.Visible=true;
					//if(allowTopaz) {	
					_sigBoxTopaz.Visible=false;
					//}
					sigBox.ClearTablet();
					//sigBox.SetSigCompressionMode(0);
					//sigBox.SetEncryptionMode(0);
					sigBox.SetKeyString(GetHashString(GetDocumentShowing0()));
					//"0000000000000000");
					//sigBox.SetAutoKeyData(ProcCur.Note+ProcCur.UserNum.ToString());
					//sigBox.SetEncryptionMode(2);//high encryption
					//sigBox.SetSigCompressionMode(2);//high compression
					sigBox.SetSigString(GetDocumentShowing0().Signature);
					if(sigBox.NumberOfTabletPoints()==0) {
						labelInvalidSig.Visible=true;
					}
					sigBox.SetTabletState(0);//not accepting input.
				}
			}
		}

		private string GetHashString(Document doc) {
			return ImageStore.GetHashString(doc,_patFolder);
		}

		///<summary>Gets the DefNum category of the current selection. The current selection can be a folder itself, or a document within a folder. If nothing selected, then it returns the DefNum of first in the list.</summary>
		private long GetCurrentCategory() {
			return imageSelector.GetSelectedCategory();
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

		///<summary>Invalidates the color image setting and recalculates.  This is not on a separate thread.  Instead, it's just designed to run no more than about every 300ms, which completely avoids any lockup.</summary>
		private void InvalidateSettingsColor() {
			if(IsDocumentShowing()){
				//if((imageSettingFlags & ImageSettingFlags.COLORFUNCTION) !=0 || (imageSettingFlags & ImageSettingFlags.CROP) !=0){
				//We don't do any flip, rotate, or crop here
				GetBitmapShowing0()?.Dispose();
				if(_bitmapRaw!=null){
					SetBitmapShowing0(ImageHelper.ApplyDocumentSettingsToImage2(GetDocumentShowing0(),_bitmapRaw, ImageSettingFlags.COLORFUNCTION));
				}
				if(_bitmapDicomRaw!=null){
					SetBitmapShowing0(DicomHelper.ApplyWindowing(_bitmapDicomRaw,GetDocumentShowing0().WindowingMin,GetDocumentShowing0().WindowingMax));
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

		///<summary>Returns true if a valid document is showing.  This is very different from testing the property _documentShowing, which would return true for mounts.</summary>
		private bool IsDocumentShowing(){
			if(imageSelector.GetSelectedType()!=EnumImageNodeType.Document){
				return false;
			}
			if(GetDocumentShowing0()==null){
				return false;
			}
			return true;
		}

		///<summary>Returns true if a valid mount is showing.</summary>
		private bool IsMountShowing(){
			if(imageSelector.GetSelectedType()!=EnumImageNodeType.Mount){
				return false;
			}
			if(_mountShowing==null){
				return false;
			}
			return true;
		}

		///<summary>Returns true if a valid mountitem is selected and there's a valid bitmap in that location.</summary>
		private bool IsMountItemSelected(){
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

		private void labelInvalidSig_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
		}

		private void label15_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
		}

		private void label1_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
		}

		///<summary>Resizes all controls in the image module to fit inside the current window, including controls which have varying visibility.</summary>
		public void LayoutControls(){
			imageSelector.Font=Font;
			LayoutManager.Move(toolBarMain,new Rectangle(0,0,Width,LayoutManager.Scale(25)));
			LayoutManager.Move(windowingSlider,new Rectangle(4,toolBarMain.Bottom+2,LayoutManager.Scale(154),LayoutManager.Scale(20)));
			LayoutManager.Move(zoomSlider,new Rectangle(windowingSlider.Right+4,toolBarMain.Bottom,LayoutManager.Scale(231),LayoutManager.Scale(25)));
			LayoutManager.Move(toolBarPaint,new Rectangle(zoomSlider.Right+1,toolBarMain.Bottom,Width-(zoomSlider.Right+1),LayoutManager.Scale(25)));
			if(_isTreeDockerCollapsed){
				imageSelector.Visible=false;
				//panelTreeTools.Visible=false;
				LayoutManager.Move(panelSplitter,new Rectangle(0,toolBarPaint.Bottom,10,Height-toolBarPaint.Bottom));
				panelSplitter.Invalidate();
			}
			else{
				imageSelector.Visible=true;
				//panelTreeTools.Visible=true;
				//LayoutManager.Move(panelTreeTools,new Rectangle(0,toolBarPaint.Bottom,LayoutManager.Scale(_widthTree96),LayoutManager.Scale(24)));
				LayoutManager.Move(imageSelector,new Rectangle(0,toolBarPaint.Bottom,LayoutManager.Scale(_widthTree96),Height-toolBarPaint.Bottom));
				LayoutManager.Move(panelSplitter,new Rectangle(imageSelector.Width+1,toolBarPaint.Bottom,10,Height-toolBarPaint.Bottom));
				panelSplitter.Invalidate();
				//LayoutManager.Move(imageSelector,new Rectangle(panelSplitter.Right+1,toolBarPaint.Bottom,LayoutManager.Scale(_widthSelector96),Height-toolBarPaint.Bottom));
			}
			int panelNoteHeight=Math.Min(114,Height-toolBarPaint.Bottom);
			if(!panelNote.Visible){
				panelNoteHeight=0;
			}
			LayoutManager.Move(panelNote,new Rectangle(
				panelSplitter.Right+1,
				Height-panelNoteHeight-1,
				Width-panelSplitter.Right-1,
				panelNoteHeight
				));
			if(panelAcquire.Visible){
				LayoutManager.Move(panelAcquire,new Rectangle(
					panelSplitter.Right+1,
					toolBarPaint.Bottom,
					Width-panelSplitter.Right-1,
					LayoutManager.Scale(23)));	
			}
			if(panelImportAuto.Visible){
				LayoutManager.Move(panelImportAuto,new Rectangle(
					panelSplitter.Right+1,
					toolBarPaint.Bottom,
					Width-panelSplitter.Right-1,
					LayoutManager.Scale(44)));	
			}
			LayoutManager.Move(panelMain,new Rectangle(
				panelSplitter.Right+1,
				toolBarPaint.Bottom,//sits under panelAcquire so that the main image doesn't jump
				Width-panelSplitter.Right-1,
				panelNote.Top-toolBarPaint.Bottom-2));	
			if(_webBrowser!=null && _webBrowser.Visible) {
				LayoutManager.Move(_webBrowser,panelMain.Bounds);
			}
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
				_bitmapDicomRaw=ImageStore.OpenBitmapDicom(_arrayDocumentsShowing[idx],_patFolder);
				if(_bitmapDicomRaw==null){
					return;
				}
			}
			else{
				bitmapTemp=ImageStore.OpenImage(_arrayDocumentsShowing[idx],_patFolder);
				if(bitmapTemp==null){
					return;
				}
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Document){
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
		private void LoadPdf(string atoZFolder,string atoZFileName,string localPath,ref bool isExportable,string downloadMessage) {
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
					isExportable=true;
					_countWebBrowserLoads++;
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
					pdfFilePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),GetDocumentShowing0().DocNum+(_patCur!=null ? _patCur.PatNum.ToString() : "")+".pdf");
					//Check to see if the PDF already exists in the temp directory and if it's already in use (likely loaded into the web browser already).
					if(!ODFileUtils.IsFileInUse(pdfFilePath)) {
						FileAtoZ.Download(FileAtoZ.CombinePaths(atoZFolder,atoZFileName),pdfFilePath,downloadMessage);
					}
				}
			}
			else {
				pdfFilePath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),GetDocumentShowing0().DocNum+(_patCur!=null ? _patCur.PatNum.ToString() : "")+".pdf");
				//Check to see if the PDF already exists in the temp directory and if it's already in use (likely loaded into the web browser already).
				if(!ODFileUtils.IsFileInUse(pdfFilePath)) {
					File.WriteAllBytes(pdfFilePath,Convert.FromBase64String(GetDocumentShowing0().RawBase64));
				}
			}
			_webBrowserFilePath=pdfFilePath;
		}

		protected override void OnHandleDestroyed(EventArgs e){
		
		}

		private void panelNote_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
		}

		///<summary></summary>
		private void RefreshModuleData(long patNum) {
			SelectTreeNode(null);//Clear selection and image and reset visibilities. Example: clear PDF when switching patients.
			if(patNum==0) {
				_familyCur=null;
				_patCur=null;
				return;
			}
			_familyCur=Patients.GetFamily(patNum);
			_patCur=_familyCur.GetPatient(patNum);
			_patFolder=ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath());//This is where the pat folder gets created if it does not yet exist.
			if(_patNumLastSecurityLog!=patNum) {
				SecurityLogs.MakeLogEntry(Permissions.ImagingModule,patNum,"");
				_patNumLastSecurityLog=patNum;
			}
			if(CloudStorage.IsCloudStorage) {
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() =>ImageStore.AddMissingFilesToDatabase(_patCur);
				progressOD.ShowDialogProgress();
			}
			else{
				ImageStore.AddMissingFilesToDatabase(_patCur);
			}
		}

		private void RefreshModuleScreen() {
			if(this.Enabled && _patCur!=null) {
				//Enable tools which must always be accessible when a valid patient is selected.
				EnableToolBarsPatient(true);
				//Item specific tools disabled until item chosen.
				EnableAllToolBarButtons(false);
			}
			else {
				EnableToolBarsPatient(false);//Disable entire menu (besides select patient).
			}
			ClearObjects();
			LayoutControls();//to handle dpi changes
			toolBarMain.Invalidate();
			toolBarPaint.Invalidate();
			FillTree(false);
		}

		///<summary>Sets cursor, sets pushed, and sets toolBarMount visible/invisible. This is called when user clicks on one of these buttons or when they select a treeNode.  Can also be called when LayoutToolbars is called from FormOpenDental, and in that case, we don't want to change EnableTreeItemTools.  EnableTreeItemTools is intentionally not centralized here, but is instead based on clicking events.</summary>
		private void SetCropPanEditAdj(EnumCropPanAdj cropPanEditAdj){
			toolBarPaint.Buttons[TB.Crop.ToString()].Pushed=false;
			toolBarPaint.Buttons[TB.Pan.ToString()].Pushed=false;
			toolBarPaint.Buttons[TB.Adj.ToString()].Pushed=false;
			switch(cropPanEditAdj){
				case EnumCropPanAdj.Crop:
					panelMain.Cursor=Cursors.Arrow;
					toolBarPaint.Buttons[TB.Crop.ToString()].Pushed=true;
					break;
				case EnumCropPanAdj.Pan:
					panelMain.Cursor=_cursorPan;
					toolBarPaint.Buttons[TB.Pan.ToString()].Pushed=true;
					break;
				case EnumCropPanAdj.Adj:
					panelMain.Cursor=Cursors.SizeAll;
					toolBarPaint.Buttons[TB.Adj.ToString()].Pushed=true;
					break;
			}
			_cropPanEditAdj=cropPanEditAdj;
			LayoutControls();
			toolBarPaint.Invalidate();
		}

		///<summary>Sets the panelnote visibility based on the given document's signature data and the current operating system.</summary>
		private void SetPanelNoteVisibility() {
			if(!IsDocumentShowing()){
				panelNote.Visible=false;
				return;
			}
			if(!GetDocumentShowing0().Note.IsNullOrEmpty()){
				panelNote.Visible=true;
				return;
			}
			if(!GetDocumentShowing0().Signature.IsNullOrEmpty()){
				panelNote.Visible=true;
				return;
			}
			panelNote.Visible=false;
		}

		private void SetZoomSlider(){
			if(IsDocumentShowing()){
				if(_bitmapRaw==null && _bitmapDicomRaw==null){
					//pdf. It will be disabled anyway
					zoomSlider.SetValue(panelMain.Size,panelMain.Size,0);//100
					return;
				}
				if(_bitmapRaw!=null){
					Size sizeImage=_bitmapRaw.Size;
					if(GetDocumentShowing0().CropW>0){
						sizeImage=new Size(GetDocumentShowing0().CropW,GetDocumentShowing0().CropH);
					}
					zoomSlider.SetValue(panelMain.Size,sizeImage,GetDocumentShowing0().DegreesRotated);
				}
				if(_bitmapDicomRaw!=null){
					Size sizeImage=new Size(_bitmapDicomRaw.Width,_bitmapDicomRaw.Height);
					if(GetDocumentShowing0().CropW>0){
						sizeImage=new Size(GetDocumentShowing0().CropW,GetDocumentShowing0().CropH);
					}
					zoomSlider.SetValue(panelMain.Size,sizeImage,GetDocumentShowing0().DegreesRotated);
				}
			}
			if(IsMountShowing()){
				zoomSlider.SetValue(panelMain.Size,new Size(_mountShowing.Width,_mountShowing.Height),0);
			}
		}

		private void SetWindowingSlider() {
			if(IsDocumentShowing()){
				if(GetDocumentShowing0().WindowingMax==0) {//default
					windowingSlider.MinVal=0;
					windowingSlider.MaxVal=255;
				}
				else {
					windowingSlider.MinVal=GetDocumentShowing0().WindowingMin;
					windowingSlider.MaxVal=GetDocumentShowing0().WindowingMax;
				}
				return;
			}
			if(IsMountItemSelected()){
				if(_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax==0) {//default
					windowingSlider.MinVal=0;
					windowingSlider.MaxVal=255;
				}
				else {
					windowingSlider.MinVal=_arrayDocumentsShowing[_idxSelectedInMount].WindowingMin;
					windowingSlider.MaxVal=_arrayDocumentsShowing[_idxSelectedInMount].WindowingMax;
				}
			}
			else{
				windowingSlider.MinVal=0;
				windowingSlider.MaxVal=255;
			}
		}

		private void sigBox_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
		}

		private void sigBoxTopaz_DoubleClick(object sender,EventArgs e) {
			ToolBarSign_Click();
		}

		private void textNote_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
		}

		private void ToolBarAcquire_Click(){
			//If no patient selected, then this button is disabled
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			if(!ODBuild.IsTrial()
				&& !OpenDentalHelp.ODHelp.IsEncryptedKeyValid())//always true in debug
			{
				MsgBox.Show(this,"This feature requires an active support plan.");
				return;
			}
			if(IsMountShowing()){
				if(_idxSelectedInMount==-1){
					for(int i=0;i<_arrayDocumentsShowing.Length;i++){
						if(_arrayDocumentsShowing[i]==null){
							_idxSelectedInMount=i;//preselect the first item available
							break;
						}
					}
					panelMain.Invalidate();
					//still might not be one selected
				}
				//if(_idxSelectedInMount!=-1 && _arrayDocumentsShowing[_idxSelectedInMount]==null){
				//	_isAcquiringSeries=true;
				//}
			}
			List<ImagingDevice> listImagingDevicesAll=ImagingDevices.GetDeepCopy();
			string workstation=ODEnvironment.MachineName;
			List<ImagingDevice> listImagingDevices=listImagingDevicesAll.FindAll(x=>x.ComputerName=="" || x.ComputerName==workstation);
			comboDevice.Items.Clear();
			comboDevice.Items.AddList(listImagingDevices,x=>x.Description);
			if(listImagingDevices.Count==0){
				MsgBox.Show(this,"Please set up imaging devices first from Main Menu - Setup - Imaging - Devices.");
				return;
			}
			comboDevice.SetSelected(0);
			butStart.Enabled=true;
			butCancel.Text=Lan.g(this,"Cancel");
			panelAcquire.Visible=true;
			panelAcquireColor.Visible=false;
			labelAcquireNotifications.Visible=false;
			LayoutControls();
			//Next will either be Start button or Cancel button
		}

		///<summary>Three objects will end up on clipboard: 1:TypeAndKey , 2:Bitmap(if it's an image type), 3:FileDrop. For images, the filedrop will point to a temp copy of the saved bitmap.  For non-images, filedrop will point to the original file.</summary>
		private void ToolBarCopy_Click(){
			//not enabled when pdf selected
			TypeAndKey typeAndKey=null;
			Bitmap bitmapCopy=null;
			string fileName="";
			Cursor=Cursors.WaitCursor;
			if(IsMountItemSelected()){
				typeAndKey=new TypeAndKey(EnumImageNodeType.Document,_arrayDocumentsShowing[_idxSelectedInMount].DocNum);
				Bitmap bitmapFromDisk=ImageStore.OpenImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);//returns null if not HasImageExtension
				if(bitmapFromDisk==null){
					//dicom or extremely rare non-image types in the mount item
					fileName=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath()),_arrayDocumentsShowing[_idxSelectedInMount].FileName);
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
					//file is always copy of image instead of original file, even for dicom.  TypeAndKey still allows true dicom copy/paste.
					fileName=Path.GetTempPath()+_arrayDocumentsShowing[_idxSelectedInMount].FileName;
					bitmapCopy.Save(fileName);
				}
			}
			else if(IsMountShowing()){
				typeAndKey=new TypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum);
				//Bitmap and file are a composite image of the mount for external paste.
				//TypeAndKey still allows for copying the mount itself, along with all attached mount items, if within OD.
				bitmapCopy=new Bitmap(_mountShowing.Width,_mountShowing.Height);
				Graphics g=Graphics.FromImage(bitmapCopy);
				g.TranslateTransform(bitmapCopy.Width/2,bitmapCopy.Height/2);//Center of image
				DrawMount(g);
				g.Dispose();
				fileName=Path.GetTempPath()+_patCur.LName+_patCur.FName+".jpg";
				bitmapCopy.Save(fileName);
			}
			else if(IsDocumentShowing()){
				typeAndKey=new TypeAndKey(EnumImageNodeType.Document,GetDocumentShowing0().DocNum);
				if(ImageHelper.HasImageExtension(GetDocumentShowing0().FileName) || GetDocumentShowing0().FileName.EndsWith(".dcm")){
					//normal images and dicom will have the bitmap and filedrop pulled from the screen
					bitmapCopy=ImageHelper.ApplyDocumentSettingsToImage2(GetDocumentShowing0(),GetBitmapShowing0(),
						ImageSettingFlags.FLIP | ImageSettingFlags.ROTATE | ImageSettingFlags.CROP);
					fileName=Path.GetTempPath()+GetDocumentShowing0().FileName;
					bitmapCopy.Save(fileName);
				}
				else{
					//other files such as pdf will lack a bitmapCopy
					fileName=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath()),GetDocumentShowing0().FileName);
				}
			}
			else{
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please make a selection before copying");
				return;
			}
			if(typeAndKey==null && bitmapCopy==null && fileName=="") {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Error.");
				return;
			}
			DataObject dataObject=new DataObject();
			if(typeAndKey!=null){
				dataObject.SetData(typeAndKey);
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
			if(_patCur!=null) {
				patNum=_patCur.PatNum;
			}
			Cursor=Cursors.Default;
			if(IsMountItemSelected()){
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNum,"Patient image "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" copied to clipboard");
			}
			else if(IsMountShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNum,"Patient mount "+_mountShowing.Description+" copied to clipboard");
			}
			else if(IsDocumentShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Copy,patNum,"Patient image "+GetDocumentShowing0().FileName+" copied to clipboard");
			}
		}

		private void ToolBarDelete_Click(){
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None){
				MsgBox.Show(this,"No item is currently selected");
				return;
			}
			//can't get to here if category selected
			TypeAndKey typeAndKeyAbove=imageSelector.GetItemAbove();//always successful
			if(IsDocumentShowing()){
				DeleteDocument(true,true,GetDocumentShowing0());//security is inside this method
				imageSelector.SetSelected(typeAndKeyAbove.NodeType,typeAndKeyAbove.PriKey);
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
						ImageStore.DeleteDocuments(_arrayDocumentsShowing,_patFolder);
					}
					catch(Exception ex) {  //Image could not be deleted, in use.
						MessageBox.Show(this,ex.Message);
						return;
					}
				}
				Mounts.Delete(_mountShowing);
				FillTree(false);
				panelMain.Invalidate();
				imageSelector.SetSelected(typeAndKeyAbove.NodeType,typeAndKeyAbove.PriKey);
			}
		}

		private void ToolBarExport_Click(){
			if(ODBuild.IsWeb()) {
				ToolBarExport_ClickWeb();
				return;
			}
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageExport,GetDocumentShowing0().DateCreated)) {
					return;
				}		
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Title="Export a Document";
				saveFileDialog.FileName=GetDocumentShowing0().FileName;
				saveFileDialog.DefaultExt=Path.GetExtension(GetDocumentShowing0().FileName);
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				try {
					ImageStore.Export(saveFileDialog.FileName,GetDocumentShowing0(),_patCur);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to export file, May be in use")+": " + ex.Message);
					return;
				}
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing0().DocCategory);
				string logText="Document Exported: "+GetDocumentShowing0().FileName+" with catgegory "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,_patCur.PatNum,logText,GetDocumentShowing0().DocNum,GetDocumentShowing0().DateTStamp);
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
					ImageStore.Export(saveFileDialog.FileName,_arrayDocumentsShowing[_idxSelectedInMount],_patCur);
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to export file, May be in use")+": " + ex.Message);
					return;
				}
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Exported: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" to "+Path.GetDirectoryName(saveFileDialog.FileName);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,_patCur.PatNum,logText,_arrayDocumentsShowing[_idxSelectedInMount].DocNum,
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
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,_patCur.PatNum,logText);
				return;
			}
		}

		private void ToolBarExport_ClickWeb(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageExport,GetDocumentShowing0().DateCreated)) {
					return;
				}	
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),GetDocumentShowing0().FileName);
				string docPath=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath()),GetDocumentShowing0().FileName);
				FileAtoZ.Copy(docPath,tempFilePath,FileAtoZSourceDestination.AtoZToLocal,"Exporting file...");
				ThinfinityUtils.ExportForDownload(tempFilePath);
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing0().DocCategory);
				string logText="Document Exported: "+GetDocumentShowing0().FileName+" with catgegory "
					+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,_patCur.PatNum,logText,GetDocumentShowing0().DocNum,GetDocumentShowing0().DateTStamp);
				return;
			}
			if(IsMountItemSelected()){
				if(!Security.IsAuthorized(Permissions.ImageExport,_arrayDocumentsShowing[_idxSelectedInMount].DateCreated)) {
					return;
				}	
				string tempFilePath=ODFileUtils.CombinePaths(Path.GetTempPath(),_arrayDocumentsShowing[_idxSelectedInMount].FileName);
				string docPath=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath()),_arrayDocumentsShowing[_idxSelectedInMount].FileName);
				FileAtoZ.Copy(docPath,tempFilePath,FileAtoZSourceDestination.AtoZToLocal,"Exporting file...");
				ThinfinityUtils.ExportForDownload(tempFilePath);
				MsgBox.Show(this,"Done.");
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Exported: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" to "+Path.GetDirectoryName(tempFilePath);
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,_patCur.PatNum,logText,_arrayDocumentsShowing[_idxSelectedInMount].DocNum,
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
				SecurityLogs.MakeLogEntry(Permissions.ImageExport,_patCur.PatNum,logText);
				return;
			}
		}

		private void ToolBarFlip_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing0().DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing0().Copy();
				GetDocumentShowing0().IsFlipped=!GetDocumentShowing0().IsFlipped;
				//Document is always flipped and then rotated when drawn, but we want to flip it
				//horizontally no matter what orientation it's in right now
				if(GetDocumentShowing0().DegreesRotated==90){
					GetDocumentShowing0().DegreesRotated=270;
				}
				else if(GetDocumentShowing0().DegreesRotated==270){
					GetDocumentShowing0().DegreesRotated=90;
				}
				Documents.Update(GetDocumentShowing0(),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing0(),_patFolder);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing0().DocCategory);
				string logText="Document Edited: "+GetDocumentShowing0().FileName+" with catgegory "
					+defDocCategory.ItemName+" flipped horizontally";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
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
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" flipped horizontally";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		private void ToolBarImport_Click(){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			//"Alternate" appended to hook name because "ToolBarImport_Click_Start" already exists in ToolBarImportSingle()
			if(Plugins.HookMethod(this,"ContrImages.ToolBarImport_Click_Start_Alternate",_patCur)) {
				FillTree(true);
				return;
			}
			if(IsMountShowing()){
				ToolBarImportMount();
			}
			else{//including nothing selected
				ToolBarImportSingle();
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
						progressOD.ActionMain=() => doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);//Makes log
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
			SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
			if(fileNames.Length==1){
				_idxSelectedInMount=listMountItemsAvail[0].ItemOrder-1;//-1 is to account for 1-indexed vs 0-indexed.
				panelMain.Invalidate();
			}
		}

		///<summary>Returns a list of available slots, starting with the one selected.  It will loop back around at the end to fill remaining slots.  Will return null if not enough slots.  But if you supply countNeed -1, then it will give you a list of all available.</summary>
		private List<MountItem> GetAvailSlots(int countNeed=-1){
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

		///<summary>Not importing to mount.  Still supports multiple imports at once.</summary>
		private void ToolBarImportSingle() {
			if(Plugins.HookMethod(this,"ContrImages.ToolBarImport_Click_Start",_patCur)) {//Named differently for backwards compatibility
				FillTree(true);
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
			TypeAndKey typeAndKey=null;
			Document doc=null;
			for(int i=0;i<fileNames.Length;i++) {
				try {
					if(CloudStorage.IsCloudStorage) {
						ProgressOD progressOD=new ProgressOD();
						progressOD.ActionMain=() => doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);
						progressOD.ShowDialogProgress();
						if(progressOD.IsCancelled){
							return;
						}
					}
					else{
						doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);//Makes log
					}
				}
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+openFileDialog.FileName);
					continue;
				}
				FillTree(false);
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum),fileNames[i]);
				using FormDocInfo FormD=new FormDocInfo(_patCur,doc,isDocCreate:true);
				FormD.TopMost=true;
				FormD.ShowDialog(this);//some of the fields might get changed, but not the filename
				if(FormD.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,doc);
				}
				else {
					if(doc.ImgType==ImageType.Photo) {
						PatientEvent.Fire(ODEventType.Patient,_patCur);//Possibly updated the patient picture.
					}
					typeAndKey=new TypeAndKey(EnumImageNodeType.Document,doc.DocNum);
					SetDocumentShowing0(doc.Copy());
				}
			}
			FillTree(true);
			/*todo:
			if(treeMain.SelectedNode!=null) {
				treeMain.SelectedNode.EnsureVisible();
			}*/
		}

		private void ToolBarImportAuto(object sender, EventArgs e){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			if(_idxSelectedInMount==-1){
				for(int i=0;i<_arrayDocumentsShowing.Length;i++){
					if(_arrayDocumentsShowing[i]==null){
						_idxSelectedInMount=i;//preselect the first item available
						break;
					}
				}
				panelMain.Invalidate();
				//If one is still not selected, they will get a warning when they click Start
			}
			textImportAuto.Text=PrefC.GetString(PrefName.AutoImportFolder);
			textImportAuto.ReadOnly=false;
			butBrowse.Visible=true;
			butGoTo.Visible=true;
			butImportStart.Visible=true;
			butImportClose.Text=Lan.g(this,"Close");
			panelImportAuto.Visible=true;
			LayoutControls();
		}

		private void ToolBarInfo_Click(){
			if(IsMountItemSelected()){
				if(_arrayDocumentsShowing[_idxSelectedInMount]==null){
					return;//silent fail is fine
				}
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,_arrayDocumentsShowing[_idxSelectedInMount]);
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
				FillTree(true);
				Cursor=Cursors.Default;
				return;
			}
			if(IsDocumentShowing()) {
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing0());
				formDocInfo.ShowDialog();
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					return;
				}
				FillTree(true);
			}
		}

		private void ToolBarMoveToPatient(object sender, EventArgs e){
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Not yet implemented for Open Dental Cloud.");
				return;
			}
			if(!IsDocumentShowing() && !IsMountShowing()){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(IsMountShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageDelete,_mountShowing.DateCreated)) {
					return;
				}
			}
			else {
				if(!Security.IsAuthorized(Permissions.ImageDelete,GetDocumentShowing0().DateCreated)) {
					return;
				}
			}
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK){
				return;
			}
			string patFolderOld=ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath());
			Document document=null;
			Mount mount=null;
			if(IsDocumentShowing()){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move selected item to the other patient?")){
					return;
				}
				document=GetDocumentShowing0().Copy();
			}
			if(IsMountShowing()){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move the entire mount to the other patient?")){
					return;
				}
				mount=_mountShowing.Copy();
			}
			if(_idxSelectedInMount>-1){
				_idxSelectedInMount=-1;
			}
			ToolBarCopy_Click();
			GotoModule.GotoImage(formPatientSelect.SelectedPatNum,0);
			if(document!=null){
				imageSelector.SetSelected(EnumImageNodeType.Category,document.DocCategory);
			}
			if(mount!=null){
				imageSelector.SetSelected(EnumImageNodeType.Category,mount.DocCategory);
			}
			IDataObject iDataObject=Clipboard.GetDataObject();
			if(iDataObject==null){
				MsgBox.Show(this,"Clipboard is empty.");
				return;
			}
			TypeAndKey typeAndKey=(TypeAndKey)iDataObject.GetData(typeof(TypeAndKey));
			if(typeAndKey==null){
				MsgBox.Show(this,"Unknown error.");
				return;
			}
			ToolBarPasteTypeAndKey(typeAndKey);
			//Delete Old=====================================================================================
			if(document!=null){
				try {
					ClearPDFBrowser();//Dispose of the web browser control that has a hold on the PDF that is showing.
					ImageStore.DeleteDocuments(new List<Document> {document},patFolderOld);
				}
				catch(Exception ex) { 
					MsgBox.Show(Lan.g(this,"Could not delete original. ")+ex.Message);
				}
				return;
			}
			if(mount!=null){
				List<MountItem> listMountItems=MountItems.GetItemsForMount(mount.MountNum);
				Document[] documentArray=Documents.GetDocumentsForMountItems(listMountItems);
				try {
					ImageStore.DeleteDocuments(documentArray,patFolderOld);
				}
				catch(Exception ex) { 
					MsgBox.Show(Lan.g(this,"Could not delete original. ")+ex.Message);
					return;
				}
				Mounts.Delete(mount);
			}
		}

		///<summary>Copy/paste that's entirely within OD will use PK of mount or doc in order to preserve windowing, rotation, crop etc.  Paste from outside OD prefers a bitmap.  If no bitmap, then it will use a filedrop which could include multiple files.</summary>
		private void ToolBarPaste_Click(){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			IDataObject iDataObject=null;
			TypeAndKey typeAndKey=null;
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
				typeAndKey=(TypeAndKey)iDataObject.GetData(typeof(TypeAndKey));
			}
			if(typeAndKey!=null){
				ToolBarPasteTypeAndKey(typeAndKey);
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
						Document doc=ImageStore.Import(bitmapPaste,GetCurrentCategory(),ImageType.Photo,_patCur);//Makes log
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
					SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
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
								progressOD.ActionMain=() => doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);;
								progressOD.ShowDialogProgress();
								if(progressOD.IsCancelled){
									Cursor=Cursors.Default;
									return;//cleanup?
								}
							}
							else{
								doc=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);//Makes log
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
					SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
				}//end of filenames
				return;
			}//mount
			//Everything below this line is for one or more documents not in a mount=============================================================================================
			//But we cannot test for this. Needs to work for cat selected, nothing selected, existing doc selected, etc.
			Document document=null;
			if(bitmapPaste!=null){
				try {
					document=ImageStore.Import(bitmapPaste,GetCurrentCategory(),ImageType.Photo,_patCur);//Makes log entry
				}
				catch {
					Cursor=Cursors.Default;
					MessageBox.Show(Lan.g(this,"Error saving document."));
					return;
				}
				using FormDocInfo formDocInfo = new FormDocInfo(_patCur,document,isDocCreate: true);
				formDocInfo.TopMost=true;
				formDocInfo.ShowDialog(this);//some of the fields might get changed, but not the filename
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,document);
				}
				else {
					if(document.ImgType==ImageType.Photo) {
						PatientEvent.Fire(ODEventType.Patient,_patCur);//Possibly updated the patient picture.
					}
					typeAndKey=new TypeAndKey(EnumImageNodeType.Document,document.DocNum);
					SetDocumentShowing0(document.Copy());
				}
			}
			else{//files
				for(int i=0;i<fileNames.Length;i++) {
					try {
						//fileNames contains full paths
						if(CloudStorage.IsCloudStorage) {
							//this will flicker because multiple progress bars.  Improve later.
							ProgressOD progressOD=new ProgressOD();
							progressOD.ActionMain=() => document=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);
							progressOD.ShowDialogProgress();
							if(progressOD.IsCancelled){
								return;
							}
						}
						else{
							document=ImageStore.Import(fileNames[i],GetCurrentCategory(),_patCur);//Makes log
						}
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ")+ex.Message+": "+fileNames[i]);
						continue;
					}
					using FormDocInfo formDocInfo=new FormDocInfo(_patCur,document,isDocCreate:true);
					formDocInfo.TopMost=true;
					formDocInfo.ShowDialog(this);//some of the fields might get changed, but not the filename
					if(formDocInfo.DialogResult!=DialogResult.OK) {
						DeleteDocument(false,false,document);
					}
					else {
						if(document.ImgType==ImageType.Photo) {
							PatientEvent.Fire(ODEventType.Patient,_patCur);//Possibly updated the patient picture.
						}
						typeAndKey=new TypeAndKey(EnumImageNodeType.Document,document.DocNum);
						SetDocumentShowing0(document.Copy());
					}
				}//for
			}//files
			FillTree(false);
			SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,document.DocNum));
		}

		private void ToolBarPasteTypeAndKey(TypeAndKey typeAndKey){
			//Always single item, not series.
			Document document=null;
			if(typeAndKey.NodeType==EnumImageNodeType.Document){//pasting from document
				if(IsMountShowing()){//into mount
					if(_idxSelectedInMount==-1 || _arrayDocumentsShowing[_idxSelectedInMount]!=null){
						//Cursor=Cursors.Default;
						MessageBox.Show(Lan.g(this,"If pasting into a mount, please select an empty mount item, first."));
						return;
					}
				}
				Document documentOriginal=Documents.GetByNum(typeAndKey.PriKey);
				Patient patientOriginal=Patients.GetPat(documentOriginal.PatNum);
				string sourceFile=FileAtoZ.CombinePaths(ImageStore.GetPatientFolder(patientOriginal,ImageStore.GetPreferredAtoZpath()),documentOriginal.FileName);
				//we could be pasting into the same patient or a different patient.
				document=ImageStore.Import(sourceFile,GetCurrentCategory(),_patCur);
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
					SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,_mountShowing.MountNum));
					//panelMain.Invalidate();
				}
				else{
					FillTree(false);
					SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,document.DocNum));
				}
			}
			else if(typeAndKey.NodeType==EnumImageNodeType.Mount){
				Mount mountOriginal=Mounts.GetByNum(typeAndKey.PriKey);
				Patient patientOriginal=Patients.GetPat(mountOriginal.PatNum);
				Mount mount=mountOriginal.Copy();
				mount.PatNum=_patCur.PatNum;
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
					document=ImageStore.Import(sourceFile,GetCurrentCategory(),_patCur);
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
				FillTree(false);
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
			}
		}

		private void ToolBarPrint_Click(){
			if(!IsDocumentShowing() && !IsMountShowing()){
				MsgBox.Show(this,"Cannot print. No document currently selected.");
				return;
			}
			if(IsDocumentShowing()){
				if(Path.GetExtension(GetDocumentShowing0().FileName).ToLower()==".pdf") {//Selected document is PDF, we handle differently than documents that aren't pdf.
					if(ODBuild.IsWeb()) {
						if(_webBrowserFilePath.IsNullOrEmpty()) {
							SetPdfFilePath(_patFolder,GetDocumentShowing0().FileName,"","Downloading Document...");
						}
						if(!File.Exists(_webBrowserFilePath)) {
							MessageBox.Show(Lan.g(this,"File not found")+": "+GetDocumentShowing0().FileName);
							_webBrowserFilePath="";
							return;
						}
						ThinfinityUtils.HandleFile(_webBrowserFilePath);//This will do a PDF preview. _webBrowserDocument.ShowPrintPreviewDialog() doesn't work.
					}
					else {
						//This line will not work if _webBrowser.ReadyState==Uninitialized.
						_webBrowser.ShowPrintPreviewDialog();
					}
					SecurityLogs.MakeLogEntry(Permissions.Printing,_patCur.PatNum,"Patient PDF "+GetDocumentShowing0().FileName+" "+GetDocumentShowing0().Description+" printed");
					return;
				}
			}
			PrinterL.TryPrintOrDebugClassicPreview(printDocument_PrintPage,Lan.g(this,"Image printed."));
			if(IsDocumentShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Printing,_patCur.PatNum,"Patient image "+GetDocumentShowing0().FileName+" "+GetDocumentShowing0().Description+" printed");
			}
			if(IsMountShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Printing,_patCur.PatNum,"Patient mount "+_mountShowing.Description+" printed");
			}
		}

		private void ToolBarRotateL_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing0().DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing0().Copy();
				GetDocumentShowing0().DegreesRotated-=90;
				while(GetDocumentShowing0().DegreesRotated<0) {
					GetDocumentShowing0().DegreesRotated+=360;
				}
				Documents.Update(GetDocumentShowing0(),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing0(),_patFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing0().DocCategory);
				string logText="Document Edited: "+GetDocumentShowing0().FileName+" with catgegory "
					+defDocCategory.ItemName+" rotated left 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
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
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);
				//probably not necessary with existing crop because scale hasn't changed.
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" rotated left 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		private void ToolBarRotateR_Click(){
			if(IsDocumentShowing()) {
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing0().DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing0().Copy();
				GetDocumentShowing0().DegreesRotated+=90;
				GetDocumentShowing0().DegreesRotated%=360;
				Documents.Update(GetDocumentShowing0(),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing0(),_patFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing0().DocCategory);
				string logText="Document Edited: "+GetDocumentShowing0().FileName+" with catgegory "
					+defDocCategory.ItemName+" rotated right 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
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
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);
				LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" rotated right 90 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		private void ToolBarRotate180_Click(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageEdit,GetDocumentShowing0().DateCreated)) {
					return;
				}	
				Document docOld=GetDocumentShowing0().Copy();
				if(GetDocumentShowing0().DegreesRotated>=180){
					GetDocumentShowing0().DegreesRotated-=180;
				}
				else{
					GetDocumentShowing0().DegreesRotated+=180;
				}
				Documents.Update(GetDocumentShowing0(),docOld);
				ImageStore.DeleteThumbnailImage(GetDocumentShowing0(),_patFolder);
				SetZoomSlider();
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,GetDocumentShowing0().DocCategory);
				string logText="Document Edited: "+GetDocumentShowing0().FileName+" with catgegory "
					+defDocCategory.ItemName+" rotated 180 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
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
				ImageStore.DeleteThumbnailImage(_arrayDocumentsShowing[_idxSelectedInMount],_patFolder);
				//LoadBitmap(_idxSelectedInMount,EnumLoadBitmapType.IdxAndRaw);//no change to scale
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[_idxSelectedInMount].DocCategory);
				string logText="Document Edited: "+_arrayDocumentsShowing[_idxSelectedInMount].FileName+" within mount "
					+_mountShowing.Description+" with catgegory "+defDocCategory.ItemName+" rotated 180 degrees";
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
			}
			panelMain.Invalidate();
		}

		///<summary>Valid values for scanType are "doc","xray",and "photo"</summary>
		private void ToolBarScan_Click(string scanType){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}	
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Scanning is not supported in web mode. Please scan outside of the program and import.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			Bitmap bitmapScanned=null;
			IntPtr handleDIB=IntPtr.Zero;
			try {
				xImageDeviceManager.Obfuscator.ActivateEZTwain();
			}
			catch {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"EzTwain4.dll not found.  Please run the setup file in your images folder.");
				return;
			}
			if(IsDisposed) {
				return;//Possibly the form was being closed as the user clicked the scan button. 
			}
			if(ComputerPrefs.LocalComputer.ScanDocSelectSource) {
				if(!EZTwain.SelectImageSource(this.Handle)) {//dialog to select source
					Cursor=Cursors.Default;
					return;//User clicked cancel.
				}
			}
			EZTwain.SetHideUI(!ComputerPrefs.LocalComputer.ScanDocShowOptions);
			if(!EZTwain.OpenDefaultSource()) {//if it can't open the scanner successfully
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Default scanner could not be opened.  Check that the default scanner works from Windows Control Panel and from Windows Fax and Scan.");
				return;
			}
			EZTwain.SetResolution(ComputerPrefs.LocalComputer.ScanDocResolution);
			if(ComputerPrefs.LocalComputer.ScanDocGrayscale) {
				EZTwain.SetPixelType(1);//8-bit grayscale - only set if scanner dialog will not show
			}
			else {
				EZTwain.SetPixelType(2);//24-bit color
			}
			EZTwain.SetJpegQuality(ComputerPrefs.LocalComputer.ScanDocQuality);
			EZTwain.SetXferMech(EZTwain.XFERMECH_MEMORY);
			Cursor=Cursors.Default;
			handleDIB=EZTwain.Acquire(this.Handle);//This is where the options dialog would come up. The settings above will not populate this window.
			int errorCode=EZTwain.LastErrorCode();
			if(errorCode!=0) {
				if(errorCode==(int)EZTwainErrorCode.EZTEC_USER_CANCEL) {//19
					//message="\r\nScanning cancelled.";//do nothing
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_JPEG_DLL) {//22
					MessageBox.Show("Missing dll\r\n\r\nRequired file EZJpeg.dll is missing.");
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_0_PAGES) {//38
					//message="\r\nScanning cancelled.";//do nothing
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_NO_PDF) {//43
					MessageBox.Show("Missing dll\r\n\r\nRequired file EZPdf.dll is missing.");
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_DEVICE_PAPERJAM) {//76
					MessageBox.Show("Paper jam\r\n\r\nPlease check the scanner document feeder and ensure there path is clear of any paper jams.");
					return;
				}
				//else if(errorCode==(int)EZTwainErrorCode.EZTEC_DS_FAILURE) {//5
					//message="Duplex failure\r\n\r\nDuplex mode without scanner options window failed. Try enabling the scanner options window or disabling duplex mode.";
					//The error message above is flat out wrong, at least sometimes.  In many cases, it's a harmless failure to disable, and scan was actually successful.
				//}
				//return;//Error messages should not normally block continuation.
			}
			if(handleDIB==(IntPtr)0) {
				MsgBox.Show(EZTwain.LastErrorText());
				return;
			}
			double xdpi=EZTwain.DIB_XResolution(handleDIB);
			double ydpi=EZTwain.DIB_XResolution(handleDIB);
			IntPtr handleBitmap=EZTwain.DIB_ToDibSection(handleDIB);
			try {
				bitmapScanned=Bitmap.FromHbitmap(handleBitmap);//Sometimes throws 'A generic error occurred in GDI+.'
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error scanning")+": "+ex.Message,ex);
				return;
			}			
			bitmapScanned.SetResolution((float)xdpi,(float)ydpi);
			try {
				Clipboard.SetImage(bitmapScanned);//We do this because a customer requested it, and some customers probably use it.
			}
			catch {
				//Rarely, setting the clipboard image fails, in which case we should ignore the failure because most people do not use this feature.
			}
			ImageType imgType;
			if(scanType=="xray") {
				imgType=ImageType.Radiograph;
			}
			else if(scanType=="photo") {
				imgType=ImageType.Photo;
			}
			else {//Assume document
				imgType=ImageType.Document;
			}
			bool saved=true;
			Document doc = null;
			try {//Create corresponding image file.
				doc=ImageStore.Import(bitmapScanned,GetCurrentCategory(),imgType,_patCur);
			}
			catch(Exception ex) {
				saved=false;
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Unable to save document")+": "+ex.Message);
			}
			if(bitmapScanned!=null) {
				bitmapScanned.Dispose();
				bitmapScanned=null;
			}
			if(handleDIB!=IntPtr.Zero) {
				EZTwain.DIB_Free(handleDIB);
			}
			Cursor=Cursors.Default;
			if(saved) {
				FillTree(false);//Reload and keep new document selected.
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing0(),isDocCreate:true);
				formDocInfo.ShowDialog(this);
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,doc);
				}
				else {
					FillTree(true);//Update tree, in case the new document's icon or category were modified in formDocInfo.
				}
			}
		}

		private void ToolBarScanMulti_Click() {
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			if(ODBuild.IsWeb()) {
				MsgBox.Show(this,"Scanning is not supported in web mode. Please scan outside of the program and import.");
				return;
			}
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			try {
				xImageDeviceManager.Obfuscator.ActivateEZTwain();
			}
			catch {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"EzTwain4.dll not found.  Please run the setup file in your images folder.");
				return;
			}
			if(IsDisposed) {
				return;//Possibly the form was being closed as the user clicked the scan button. 
			}
			if(ComputerPrefs.LocalComputer.ScanDocSelectSource) {
				if(!EZTwain.SelectImageSource(this.Handle)) {
					return;//User clicked cancel.
				}
			}
			//EZTwain.LogFile(7);//Writes at level 7 (very detailed) in the C:\eztwain.log text file. Useful for getting help from EZTwain support on their forum.
			EZTwain.SetHideUI(!ComputerPrefs.LocalComputer.ScanDocShowOptions);
			EZTwain.PDF_SetCompression((int)this.Handle,(int)ComputerPrefs.LocalComputer.ScanDocQuality);
			if(!EZTwain.OpenDefaultSource()) {//if it can't open the scanner successfully
				MsgBox.Show(this,"Default scanner could not be opened.  Check that the default scanner works from Windows Control Panel and from Windows Fax and Scan.");
				Cursor=Cursors.Default;
				return;
			}
			bool duplexEnabled=EZTwain.EnableDuplex(ComputerPrefs.LocalComputer.ScanDocDuplex);//This line seems to cause problems.
			if(ComputerPrefs.LocalComputer.ScanDocGrayscale) {
				EZTwain.SetPixelType(1);//8-bit grayscale
			}
			else {
				EZTwain.SetPixelType(2);//24-bit color
			}
			EZTwain.SetResolution(ComputerPrefs.LocalComputer.ScanDocResolution);
			EZTwain.AcquireMultipageFile(this.Handle,tempFile);//This is where the options dialog will come up if enabled. This will ignore and override the settings above.
			int errorCode=EZTwain.LastErrorCode();
			if(errorCode!=0) {
				if(errorCode==(int)EZTwainErrorCode.EZTEC_USER_CANCEL) {//19
					//message="\r\nScanning cancelled.";//do nothing
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_JPEG_DLL) {//22
					MessageBox.Show("Missing dll\r\n\r\nRequired file EZJpeg.dll is missing.");
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_0_PAGES) {//38
					//message="\r\nScanning cancelled.";//do nothing
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_NO_PDF) {//43
					MessageBox.Show("Missing dll\r\n\r\nRequired file EZPdf.dll is missing.");
					return;
				}
				else if(errorCode==(int)EZTwainErrorCode.EZTEC_DEVICE_PAPERJAM) {//76
					MessageBox.Show("Paper jam\r\n\r\nPlease check the scanner document feeder and ensure the path is clear of any paper jams.");
					return;
				}
				//else if(errorCode==(int)EZTwainErrorCode.EZTEC_DS_FAILURE) {//5
					//message="Duplex failure\r\n\r\nDuplex mode without scanner options window failed. Try enabling the scanner options window or disabling duplex mode.";
					//The error message above is flat out wrong, at least sometimes.  In many cases, it's a harmless failure to disable, and scan was actually successful.
				//}
				MessageBox.Show(Lan.g(this,"Unable to scan. Please make sure you can scan using other software. Error: "+errorCode+" "+EZTwain.LastErrorText()));
				//return;//Error messages should not normally block continuation.
			}
			TypeAndKey typeAndKey=null;
			bool copied=true;
			Document doc=null;
			try {
				doc=ImageStore.Import(tempFile,GetCurrentCategory(),_patCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ") + ex.Message + ": " + tempFile);
				copied = false;
			}
			if(copied) {
				FillTree(false);
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				using FormDocInfo FormD=new FormDocInfo(_patCur,doc,isDocCreate:true);
				FormD.ShowDialog(this);//some of the fields might get changed, but not the filename 
				//Customer complained this window was showing up behind OD.  We changed above line to add a parent form as an attempted fix.
				//If this doesn't solve it we can also try adding FormD.BringToFront to see if it does anything.
				if(FormD.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,doc);
				}
				else {
					typeAndKey=new TypeAndKey(EnumImageNodeType.Document,doc.DocNum);
					SetDocumentShowing0(doc.Copy());
				}
			}
			ImageStore.TryDeleteFile(tempFile
				,actInUseException:(msg) => MsgBox.Show(msg)//Informs user when a 'file is in use' exception occurs.
			);
			//Reselect the last successfully added node when necessary. js This code seems to be copied from import multi.  Simplify it.
			if(doc!=null && !new TypeAndKey(EnumImageNodeType.Document,doc.DocNum).Equals(typeAndKey)) {
				SelectTreeNode(new TypeAndKey(EnumImageNodeType.Document,doc.DocNum));
			}
			FillTree(true);
		}

		private void ToolBarSign_Click(){
			if(imageSelector.GetSelectedType()!=EnumImageNodeType.Document){
				return;
			}
			//Show the underlying panel note box while the signature is being filled.
			panelNote.Visible=true;
			LayoutControls();
			//Display the document signature form.
			using FormDocSign formDocSign=new FormDocSign(GetDocumentShowing0(),_patCur);//Updates our local document and saves changes to db also.
			formDocSign.Location=PointToScreen(new Point(imageSelector.Left,this.ClientRectangle.Bottom-formDocSign.Height));
			formDocSign.Width=Math.Max(0,Math.Min(formDocSign.Width,panelMain.Right-imageSelector.Left));
			formDocSign.ShowDialog();
			FillTree(true);
			//Adjust visibility of panel note based on changes made to the signature above.
			SetPanelNoteVisibility();
			//Resize controls in our window to adjust for a possible change in the visibility of the panel note control.
			LayoutControls();
			//Update the signature and note with the new data.
			FillSignature();
		}

		private void ToolBarSize_Click(){
			if(IsDocumentShowing()){
				if(!Security.IsAuthorized(Permissions.ImageEdit,_arrayDocumentsShowing[0].DateCreated)) {
					return;
				}	
				if(_arrayDocumentsShowing[0]==null){
					return;
				}
				using FormDocumentSize formDocumentSize=new FormDocumentSize();
				formDocumentSize.DocumentCur=_arrayDocumentsShowing[0];
				formDocumentSize.ShowDialog();
				////the form will change DocumentCur and save it
				if(formDocumentSize.DialogResult==DialogResult.OK) {
					FillTree(true);
					Def defDocCategory=Defs.GetDef(DefCat.ImageCats,_arrayDocumentsShowing[0].DocCategory);
					string logText="Document Edited: "+_arrayDocumentsShowing[0].FileName+" with catgegory "
						+defDocCategory.ItemName+" was adjusted using the Size button";
					SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
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
					SecurityLogs.MakeLogEntry(Permissions.ImageEdit,_patCur.PatNum,logText);
				}
				panelMain.Invalidate();
			}
			//ignore mount without selected item because the button shouldn't be enabled
		}

		private void ToolBarVideo_Click(){
			//If no patient selected, then this button is disabled
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			if(!ODBuild.IsTrial()
				&& !OpenDentalHelp.ODHelp.IsEncryptedKeyValid())//always true in debug
			{
				MsgBox.Show(this,"This feature requires an active support plan.");
				return;
			}
			if(_formVideo!=null && _formVideo.Visible){
				MsgBox.Show(this,"Video capture is already running.");
				return;
			}
			if(IsMountShowing()){
				if(_idxSelectedInMount==-1){
					for(int i=0;i<_arrayDocumentsShowing.Length;i++){
						if(_arrayDocumentsShowing[i]==null){
							_idxSelectedInMount=i;//preselect the first item available
							break;
						}
					}
					panelMain.Invalidate();
					//still might not be one selected
				}
				//if(_idxSelectedInMount!=-1 && _arrayDocumentsShowing[_idxSelectedInMount]==null){
				//	_isAcquiringSeries=true;//instead, just test each time
				//}
			}
			_formVideo=new FormVideo();
			_formVideo.BitmapCaptured+=formVideo_BitmapCaptured;
			_formVideo.Show();
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;
			_formVideo.Location=new Point(rectangleWorkingArea.Left+rectangleWorkingArea.Width/2-_formVideo.Width/2,//center L/R
				rectangleWorkingArea.Bottom-_formVideo.Height);//bottom
			LayoutControls();
		}

		private void ToolBarZoomOne_Click(){
			if(_idxSelectedInMount==-1){
				return;
			}
			_pointTranslation=new Point(
				_mountShowing.Width/2-_listMountItems[_idxSelectedInMount].Xpos-_listMountItems[_idxSelectedInMount].Width/2,
				_mountShowing.Height/2-_listMountItems[_idxSelectedInMount].Ypos-_listMountItems[_idxSelectedInMount].Height/2);
			float newZoom=ZoomSlider.CalcScaleFit(new Size(panelMain.Width,panelMain.Height),
				new Size(_listMountItems[_idxSelectedInMount].Width,_listMountItems[_idxSelectedInMount].Height),0);
			zoomSlider.SetValueAndMax(newZoom*95);				
			panelMain.Invalidate();
		}
		#endregion Methods - Private

		#region Methods - Native
		[DllImport("user32.dll", SetLastError=true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		[DllImport("user32.dll")]
		private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")] 
		static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);

		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		#endregion Methods - Native

		#region Classes - Nested
		///<summary>(nested class) Allows comparison of items based on type and key.  </summary>
		[Serializable]
		public class TypeAndKey:ISerializable {
			public EnumImageNodeType NodeType;
			public long PriKey;

			private TypeAndKey() {
			}

			///<summary></summary>
			public TypeAndKey(EnumImageNodeType nodeType,long priKey){
				NodeType=nodeType;
				PriKey=priKey;
			}

			///<summary>The special constructor is used to deserialize values.</summary>
			public TypeAndKey(SerializationInfo info, StreamingContext context){
				NodeType=(EnumImageNodeType)info.GetValue("NodeType", typeof(EnumImageNodeType));
				PriKey=(long)info.GetValue("PriKey",typeof(long));
			}

			///<summary>The method is called on serialization for placing on clipboard.</summary>
			public void GetObjectData(SerializationInfo info, StreamingContext context){
				info.AddValue("NodeType", NodeType, typeof(EnumImageNodeType));
				info.AddValue("PriKey", PriKey, typeof(long));
			}

			///<summary>Tests whether the two typeAndKeys match.</summary>
			public bool IsMatching(TypeAndKey typeAndKey){
				if(typeAndKey is null){
					return false;
				}
				if(NodeType!=typeAndKey.NodeType){
					return false;
				}
				if(PriKey==typeAndKey.PriKey){
					return true;
				}
				return false;
			}
			

			public TypeAndKey Copy(){
				TypeAndKey typeAndKey=new TypeAndKey();
				typeAndKey.NodeType=NodeType;
				typeAndKey.PriKey=PriKey;
				return typeAndKey;
			}
		}





















		#endregion Classes - Nested

	
	}
}


//2020-11-29
//Todo:
//Capture XDR



//Done: 
//Drag/drop from windows folder
//Copy/paste
//Import/Export
//Print
//Pref.ImagesModuleTreeIsCollapsed deprecated
//Move to patient...
//Rotate jpg as imported
//Saving as tiff

//Todo:
//after moving to a new mount position, the size window shows the wrong percent

//Need to document how XVWeb and Suni use the old Images module.

//This new Images module will never support the following features:
//ClaimPayment/EOB mode.  Old Images module will continue to be used for that.
//EhrAmendment mode.  Can use old images module if it's still required.

//Features that we might add soon:
//Drag mount item, check mouse position to allow longer drags within single position.
//context menu in main panel (see ContrImages.menuMountItem_Opening)
//Yellow outline pixel width could be better: fixed instead of variable
//Switch to Direct2D for hardware speed and effects



