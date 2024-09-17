#region using
using CodeBase;
using ImagingDeviceManager;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
#endregion using

namespace OpenDental{
	///<summary>The Imaging Module.</summary>
	public partial class ControlImages : UserControl{
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>Color for drawing lines and text. This allows color selection to stay consistent for one session as the user switches images. Used for images and mounts. Each mount also internally stores its own colors, which cause this to change when loading a mount.</summary>
		private Color _colorFore=Color.Black;
		///<summary>Color for background of text. Only for images, not mounts (not sure why now. Maybe because that's pulled from the mount, whereas images don't have a field to pull from). Transparent (no background) can be selected and is supported.</summary>
		private Color _colorTextBack=Color.White;
		///<summary>One of these 3 states is active at any time.</summary>
		private EnumCropPanAdj _cropPanAdj;
		private DeviceController _deviceController; 
		///<summary>If the draw toolbar is showing, then one of these 5 modes will be active.</summary>
		private EnumDrawMode _drawMode;
		private Family _familyCur=null;
		private FileSystemWatcher _fileSystemWatcher;
		private FormLauncher _formLauncherVideo;
		private WpfControls.UI.ImageSelector imageSelector;
		///<summary>A list of the forms that are currently floating. Can be empty.  When a form is closed, it is removed from the list at FormImageFloat_FormClosed.</summary>
		private List<FormImageFloat> _listFormImageFloats=new List<FormImageFloat>();
		private bool _initializedOnStartup=false;
		///<summary>Gets set to true when acquiring so that the unmounted bar will show.  Once you click on an image in the tree, this is set back to false.</summary>
		private bool _isAcquiring;
		///<summary>Collapse is toggled with the triangle on the sizer.  Always starts out not collapsed.</summary>
		private bool _isTreeDockerCollapsed=false;
		///<summary>Collapsing while the mouse is down triggers another mouse move, so this prevents taking action on that phantom mouse move.</summary>
		private bool _isTreeDockerCollapsing=false;
		private Patient _patient=null;
		///<summary>Set with each RefreshModuleData, and that's where it's set if it doesn't yet exist.  For now, we are not using _patCur.ImageFolder, because we haven't tested whether it properly updates the patient object.  We don't want to risk using an outdated patient folder path.  And we don't want to waste time refreshing PatCur after every ImageStore.GetPatientFolder().</summary>
		private string _patFolder="";
		///<summary>Prevents too many security logs for this patient.</summary>
		private long _patNumLastSecurityLog=0;
		///<summary>For moving the splitter</summary>
		private Point _pointMouseDown=new Point(0,0);
		///<summary>Maintains same state for the entire session, even as user changes to different images. Does not control showing drawings for Pearl or any other external source.</summary>
		private bool _showDrawingsOD=true;
		///<summary>Maintains same state for the entire session, even as user changes to different images.</summary>
		private bool? _showDrawingsPearlToothParts=null;
		///<summary>Maintains same state for the entire session, even as user changes to different images.</summary>
		private bool? _showDrawingsPearlPolyAnnotations=null;
		///<summary>Maintains same state for the entire session, even as user changes to different images.</summary>
		private bool? _showDrawingsPearlBoxAnnotations=null;
		///<summary>Maintains same state for the entire session, even as user changes to different images.</summary>
		private bool? _showDrawingsPearlMeasurements=null;
		///<summary>Used to display Topaz signatures on Windows.</summary>
		private Control _sigBoxTopaz;
		///<summary>User can control width of tree.  This is stored as the 96dpi equivalent as float for conversion accuracy.  When tree is minimized, this doesn't change, allowing restoration to previous width.  It does not remember width between sessions.  Minimum 0, max 500.</summary>
		private float _widthTree96=228;
		private float _widthTree96StartDrag;
		private WpfControls.UI.UnmountedBar unmountedBar;
		private WpfControls.UI.WindowingSlider windowingSlider;
		private WpfControls.UI.ZoomSlider zoomSlider;
		private WpfControls.UI.ToolBar toolBarMain;
		#endregion Fields - Private

		#region Constructor
		public ControlImages(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			Logger.LogToPath("InitializeComponent",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
			Logger.LogToPath("InitializeComponent",LogPath.Startup,LogPhase.End);
			imageSelector=new WpfControls.UI.ImageSelector();
			elementHostImageSelector.Child=imageSelector;
			//imageSelector.ContextMenu=menuTree;
			imageSelector.MenuClick+=menuTree_Click2;
			imageSelector.DragDropImport+=imageSelector_DragDropImport;
			imageSelector.DraggedToCategory+=imageSelector_DraggedToCategory;
			imageSelector.ItemDoubleClick+=imageSelector_ItemDoubleClick;
			imageSelector.ItemReselected+=imageSelector_ItemReselected;
			imageSelector.SelectionChangeCommitted+=imageSelector_SelectionChangeCommitted;
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
			unmountedBar=new WpfControls.UI.UnmountedBar();
			elementHostUnmountedBar.Child=unmountedBar;
			unmountedBar.HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch;
			unmountedBar.VerticalAlignment=System.Windows.VerticalAlignment.Stretch;
			unmountedBar.EventClose+=unmountedBar_Close;
			unmountedBar.EventRefreshParent+=unmountedBar_RefreshParent;
			unmountedBar.EventRemount+=unmountedBar_Remount;
			unmountedBar.EventRetake+=unmountedBar_Retake;
			windowingSlider=new WpfControls.UI.WindowingSlider();
			elementHostWindowingSlider.Child=windowingSlider;
			windowingSlider.Width=154;
			windowingSlider.IsEnabled=false;
			windowingSlider.Scroll+=windowingSlider_Scroll;
			windowingSlider.ScrollComplete+=windowingSlider_ScrollComplete;
			zoomSlider=new WpfControls.UI.ZoomSlider();
			elementHostZoomSlider.Child=zoomSlider;
			zoomSlider.EventResetTranslation+=zoomSlider_EventResetTranslation;
			zoomSlider.EventZoomed+=zoomSlider_EventZoomed;
			toolBarMain=new WpfControls.UI.ToolBar();
			elementHostToolBarMain.Child=toolBarMain;
			//controlImageDock.Enter+=ControlImageDock_Enter;//doesn't work for clicking. Also, when user changes focus to OD, we don't necessarily need this to focus.
			controlImageDock.EventGotODFocus+=ControlImageDock_GotODFocus;
			controlImageDock.EventImageClosed+=ControlImageDock_ImageClosed;
			controlImageDock.EventButClicked+=DockOrFloat_EventButClicked;
			controlImageDock.EventWinPicked+=DockOrFloat_EventWinClicked;
			controlImageDock.EventPopFloater+=Docker_EventPopFloater;
			controlImageDock.FuncListFloaters=()=>_listFormImageFloats;
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Events
		///<summary>For Ctrl-P for select patient. Won't fire if handled instead by Adobe for print.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<KeyEventArgs> EventKeyDown=null;
		#endregion

		#region Enums
		///<summary>ToolBarButton enumeration instead of strings. For all three toolBars combined. These are used much less than in the past and many of these are no longer used at all.</summary>
		private enum TB{
			Print,
			Delete,
			Info,
			Sign,
			//ScanDoc,
			//ScanMultiDoc,
			//ScanXRay,
			//ScanPhoto,
			//MountAcquire,
			Video,
			Import,
			Export,
			Copy,
			Paste,
			Forms,
			ZoomOne,
			Crop,
			Pan,
			Flip,
			RotateL,
			RotateR,
			Rotate180,
			Adj,
			Size,
			Unmount,
			DrawTool,
			Color,
			Text,
			Line,
			Pen,
			Polygon,
			Eraser,
			ChangeColor,
			EditPoints,
			Measure,
			SetScale,
			Filter,
			Close
		}

		#endregion Enums

		#region Methods - Public
		///<summary>Refreshes list from db, then fills the treeview.  Set keepSelection to true in order to keep the current selection active.</summary>
		public void FillImageSelector(bool keepSelection) {
			if(_patient==null) {
				imageSelector.ClearAll();
				return;
			}
			int scrollVal=imageSelector.ScrollValue;
			List<Def> listDefsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			imageSelector.SetCategories(listDefsImageCats);
			DataSet dataSet=Documents.RefreshForPatient(_patient.PatNum,PrefC.GetBool(PrefName.ImagingOrderDescending));
			imageSelector.SetData(_patient,dataSet.Tables["DocumentList"],keepSelection,_patFolder);
			imageSelector.LoadExpandedPrefs();
			if(keepSelection){
				imageSelector.ScrollValue=scrollVal;
			}
		}

		///<summary>Also does LayoutToolBars. Doesn't do much, be we want to have one for each module.</summary>
		public void InitializeOnStartup(){
			if(_initializedOnStartup) {
				return;
			}
			_initializedOnStartup=true;
			LayoutToolBars();
		}

		///<summary>Key down from FormOpenDental is passed in to allow some keys to work here.  As long as this module is open, all key down events are sent here.</summary>
		public void ControlImages_KeyDown(Keys keys){
			if(_formLauncherVideo!=null && !_formLauncherVideo.IsNullDisposedOrNotVis()){
				_formLauncherVideo.MethodGetVoid("Parent_KeyDown",keys);
			}
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay!=null){
				controlImageDisplay.Parent_KeyDown(keys);
			}
		}
		
		///<summary>Layout the Main and Paint toolbars.</summary>
		public void LayoutToolBars() {
			toolBarMain.Clear();
			toolBarMain.Add(Lan.g(this,"Print"),ToolBarPrint_Click,WpfControls.UI.EnumIcons.Print,tag:TB.Print.ToString());
			toolBarMain.Add(Lan.g(this,"Delete"),ToolBarDelete_Click,WpfControls.UI.EnumIcons.DeleteX,tag:TB.Delete.ToString());
			toolBarMain.Add(Lan.g(this,"Info"),ToolBarInfo_Click,WpfControls.UI.EnumIcons.Info,tag:TB.Info.ToString());
			toolBarMain.Add(Lan.g(this,"Sign"),ToolBarSign_Click,tag:TB.Sign.ToString());
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lan.g(this,"Scan:"),null);
			toolBarMain.Add("",ToolBarScanDoc_Click,WpfControls.UI.EnumIcons.ImageSelectorDoc,toolTipText:Lan.g(this,"Scan Document"));
			toolBarMain.Add("",ToolBarScanMulti_Click,WpfControls.UI.EnumIcons.ScanMulti,toolTipText:Lan.g(this,"Scan Multi-Page Document"));
			toolBarMain.Add("",ToolBarScanXRay_Click,WpfControls.UI.EnumIcons.ScanXray,toolTipText:Lan.g(this,"Scan Radiograph"));
			toolBarMain.Add("",ToolBarScanPhoto_Click,WpfControls.UI.EnumIcons.ScanPhoto,toolTipText:Lan.g(this,"Scan Photo"));
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lan.g(this,"Mount / Acquire"),ToolBarMountAcquire_Click,WpfControls.UI.EnumIcons.Acquire,toolTipText:Lan.g(this,"Create Mount and/or Acquire from device"));
			toolBarMain.Add(Lan.g(this,"Video"),ToolBarVideo_Click,WpfControls.UI.EnumIcons.Video,toolTipText:Lan.g(this,"Intraoral Video Camera"));
			toolBarMain.AddSeparator();
			WpfControls.UI.ContextMenu contextMenuImport = new WpfControls.UI.ContextMenu();
			contextMenuImport.Add(new WpfControls.UI.MenuItem(Lan.g(this,"Import Automatically"),ToolBarImportAuto));
			//toolStripMenuItem.ToolTipText="Import files as they are created in a folder.";//todo? no tooltip available for menus
			toolBarMain.Add(Lan.g(this,"Import"),ToolBarImport_Click,WpfControls.UI.EnumIcons.Import,WpfControls.UI.ToolBarButtonStyle.DropDownButton,Lan.g(this,"Import From File"),contextMenuImport);
			WpfControls.UI.ContextMenu contextMenuExport = new WpfControls.UI.ContextMenu();
			contextMenuExport.Add(new WpfControls.UI.MenuItem(Lan.g(this,"Move to Patient..."),ToolBarMoveToPatient));
			contextMenuExport.Add(new WpfControls.UI.MenuItem(Lan.g(this,"Export TIFF"),ToolBarExportTIFF));
			toolBarMain.Add(Lan.g(this,"Export"),ToolBarExport_Click,WpfControls.UI.EnumIcons.Export,WpfControls.UI.ToolBarButtonStyle.DropDownButton,Lan.g(this,"Export to File"),contextMenuExport,TB.Export.ToString());
			toolBarMain.Add(Lan.g(this,"Copy"),ToolBarCopy_Click,WpfControls.UI.EnumIcons.Copy,toolTipText:Lan.g(this,"Copy displayed image to clipboard"),tag:TB.Copy.ToString());
			toolBarMain.Add(Lan.g(this,"Paste"),ToolBarPaste_Click,WpfControls.UI.EnumIcons.Paste,toolTipText:Lan.g(this,"Paste From Clipboard"));
			WpfControls.UI.ContextMenu contextMenuForms = new WpfControls.UI.ContextMenu();
			string formDir=FileAtoZ.CombinePaths(ImageStore.GetPreferredAtoZpath(),"Forms");
			if(CloudStorage.IsCloudStorage) {
				//Running this asynchronously to not slowdown start up time.
				ODThread odThreadTemplate=new ODThread((o) => {
					List<string> listFiles=CloudStorage.ListFolderContents(formDir);
					foreach(string fileName in listFiles) {
						if(InvokeRequired) {
							Invoke((Action)delegate () {
								contextMenuForms.Add(Path.GetFileName(fileName),new EventHandler(menuForms_Click));
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
							contextMenuForms.Add(fileInfos[i].Name,menuForms_Click);
						}
					}
				}
			}
			toolBarMain.Add(Lan.g(this,"Forms"),ToolBarForms_Click,toolBarButtonStyle:WpfControls.UI.ToolBarButtonStyle.DropDownButton,contextMenuDropDown:contextMenuForms);
			WpfControls.ProgramL.LoadToolBar(toolBarMain,EnumToolBar.ImagingModule,ToolBarProgram_Click);
			//ToolbarPaint-------------------------------------------------------------------------------------
			#region toolbarPaint
			toolBarPaint.Buttons.Clear();
			ODToolBarButton button;
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Fit 1"),-1,Lan.g(this,"Zoom to fit one image"),TB.ZoomOne));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			button=new ODToolBarButton(Lan.g(this,"Crop"),7,"",TB.Crop);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanAdj==EnumCropPanAdj.Crop){
				toolBarPaint.Buttons[TB.Crop.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Pan"),10,"",TB.Pan);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanAdj==EnumCropPanAdj.Pan){
				toolBarPaint.Buttons[TB.Pan.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Adj"),20,Lan.g(this,"Adjust position"),TB.Adj);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanAdj==EnumCropPanAdj.Adj){
				toolBarPaint.Buttons[TB.Adj.ToString()].IsTogglePushed=true;
			}
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Size/Rotation"),-1,Lan.g(this,"Set Size and Rotation"),TB.Size));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"FlipH"),11,Lan.g(this,"Flip Horizontally"),TB.Flip));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"-90"),12,Lan.g(this,"Rotate Left"),TB.RotateL));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"+90"),13,Lan.g(this,"Rotate Right"),TB.RotateR));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"180"),-1,Lan.g(this,"Rotate 180"),TB.Rotate180));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Draw"),-1,Lan.g(this,"Lines and text"),TB.DrawTool));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Unmount"),-1,Lan.g(this,"Move selected image to unmounted area"),TB.Unmount));
			toolBarPaint.Invalidate();
			#endregion toolbarPaint
			UpdateToolbarButtons();
		}

		///<summary>Layout the Draw toolbar only.</summary>
		public void LayoutToolBarDraw() {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.ColorFore=_colorFore;
			controlImageDisplay.ColorTextBack=_colorTextBack;
			if(_showDrawingsPearlToothParts is null){//happens once on startup
				if(Programs.IsEnabled(ProgramName.Pearl)){
					long programNum=Programs.GetProgramNum(ProgramName.Pearl);
					ProgramProperty programProperty=ProgramProperties.GetFirstOrDefault(x => x.ProgramNum==programNum && x.PropertyDesc=="Show Pearl annotations by default");
					if(programProperty==null) {
						_showDrawingsPearlToothParts=false;
						_showDrawingsPearlPolyAnnotations=false;
						_showDrawingsPearlBoxAnnotations=false;
						_showDrawingsPearlMeasurements=false;
					}
					else{
						_showDrawingsPearlToothParts=programProperty.PropertyValue=="true";
						_showDrawingsPearlPolyAnnotations=programProperty.PropertyValue=="true";
						_showDrawingsPearlBoxAnnotations=programProperty.PropertyValue=="true";
						_showDrawingsPearlMeasurements=programProperty.PropertyValue=="true";
					}
				}
				else{
					_showDrawingsPearlToothParts=false;
					_showDrawingsPearlPolyAnnotations=false;
					_showDrawingsPearlBoxAnnotations=false;
					_showDrawingsPearlMeasurements=false;
				}
			}
			controlImageDisplay.SetShowDrawings(_showDrawingsOD,_showDrawingsPearlToothParts.Value,_showDrawingsPearlPolyAnnotations.Value,_showDrawingsPearlBoxAnnotations.Value,_showDrawingsPearlMeasurements.Value);
			Bitmap bitmapColor=new Bitmap(22,22);//no using. Pass bitmap to toolbar, which will handle disposing the old one.
			using Graphics g=Graphics.FromImage(bitmapColor);
			g.SmoothingMode=SmoothingMode.HighQuality;
			if(_colorTextBack.ToArgb()==Color.Transparent.ToArgb()){
				if(IsMountShowing()){
					g.Clear(GetMountShowing().ColorBack);
				}
				else{
					//There is no background color for images
					g.Clear(Color.White);
				}
			}
			else{
				g.Clear(_colorTextBack);
			}
			using SolidBrush solidBrush=new SolidBrush(_colorFore);
			RectangleF rectangleF=new RectangleF(0,0,22,22);
			StringFormat stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;
			stringFormat.LineAlignment=StringAlignment.Center;
			Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.ScaleF(12),FontStyle.Bold);
			g.DrawString("A",font,solidBrush,rectangleF,stringFormat);
			stringFormat.Dispose();
			//Toolbar itself------------------------------------------------------------------------
			toolBarDraw.Buttons.Clear();
			ODToolBarButton button;
			button=new ODToolBarButton(Lan.g(this,"Color"),-1,"",TB.Color);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			button.Bitmap=bitmapColor;
			toolBarDraw.Buttons.Add(button);
			//toolBarDraw.Buttons[TB.Color.ToString()].Bitmap=bitmapColor;
			button=new ODToolBarButton(Lan.g(this,"Text"),-1,"",TB.Text);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Text){
				toolBarDraw.Buttons[TB.Text.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Pen"),-1,"",TB.Pen);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Pen){
				toolBarDraw.Buttons[TB.Pen.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Line"),-1,"",TB.Line);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Line){
				toolBarDraw.Buttons[TB.Line.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Polygon"),-1,"",TB.Polygon);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Polygon){
				toolBarDraw.Buttons[TB.Polygon.ToString()].IsTogglePushed=true;
			}
			toolBarDraw.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			if(_drawMode==EnumDrawMode.Line
				|| _drawMode==EnumDrawMode.LineEditPoints
				|| _drawMode==EnumDrawMode.LineMeasure
				|| _drawMode==EnumDrawMode.LineSetScale)
			{
				button=new ODToolBarButton(Lan.g(this,"Edit Points"),-1,"",TB.EditPoints);
				button.Style=ODToolBarButtonStyle.ToggleButton;
				toolBarDraw.Buttons.Add(button);
				button=new ODToolBarButton(Lan.g(this,"Measure"),-1,"",TB.Measure);
				button.Style=ODToolBarButtonStyle.ToggleButton;
				toolBarDraw.Buttons.Add(button);
				button=new ODToolBarButton(Lan.g(this,"Set Scale"),-1,"",TB.SetScale);
				button.Style=ODToolBarButtonStyle.ToggleButton;
				toolBarDraw.Buttons.Add(button);
				toolBarDraw.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			}
			if(_drawMode==EnumDrawMode.LineEditPoints){
				toolBarDraw.Buttons[TB.EditPoints.ToString()].IsTogglePushed=true;
			}
			if(_drawMode==EnumDrawMode.LineMeasure){
				toolBarDraw.Buttons[TB.Measure.ToString()].IsTogglePushed=true;
			}
			if(_drawMode==EnumDrawMode.LineSetScale){
				toolBarDraw.Buttons[TB.SetScale.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Eraser"),-1,"",TB.Eraser);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Eraser){
				toolBarDraw.Buttons[TB.Eraser.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Change Color"),-1,"",TB.ChangeColor);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.ChangeColor){
				toolBarDraw.Buttons[TB.ChangeColor.ToString()].IsTogglePushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Filter"),-1,"",TB.Filter);
			toolBarDraw.Buttons.Add(button);
			toolBarDraw.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarDraw.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"",TB.Close));
			toolBarDraw.Invalidate();
		}
		
		///<summary></summary>
		public void ModuleSelected(long patNum,long docNum=0){
			try {
				RefreshModuleData(patNum);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error accessing images."),ex);
			}
			if(_patient!=null && _patient.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(_patient.PatNum);
				GlobalFormOpenDental.PatientSelected(new Patient(),false);
				try {
					RefreshModuleData(0);
				}
				catch(Exception ex) {//Exception should never get thrown because RefreshModuleData() will return when PatNum is zero.
					FriendlyException.Show(Lan.g(this,"Error accessing images."),ex);
				}
			}
			if(_patient!=null && _patient.PatStatus==PatientStatus.Archived && !Security.IsAuthorized(EnumPermType.ArchivedPatientSelect,suppressMessage:true)) {
				GlobalFormOpenDental.PatientSelected(new Patient(),false);
				try {
					RefreshModuleData(0);
				}
				catch(Exception ex) {//Exception should never throw because RefreshModuleData() will return when PatNum is zero.
					FriendlyException.Show(Lan.g(this,"Error accessing images."),ex);
				}
			}
			RefreshModuleScreen();
			if(docNum!=0) {
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,docNum));
			}
			if(_patient!=null && DatabaseIntegrities.DoShowPopup(_patient.PatNum,EnumModuleType.Imaging)) {
				List<Claim> listClaims=Claims.GetForPat(_patient.PatNum);
				List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(new List<long>(){_patient.PatNum});
				bool areHashesValid=Patients.AreAllHashesValid(_patient,new List<Appointment>(),new List<PayPlan>(),new List<PaySplit>(),listClaims,listClaimProcs);
				if(!areHashesValid) {
					DatabaseIntegrities.AddPatientModuleToCache(_patient.PatNum,EnumModuleType.Imaging); //Add to cached list for next time
					//show popup
					DatabaseIntegrity databaseIntegrity=DatabaseIntegrities.GetModule();
					FrmDatabaseIntegrity frmDatabaseIntegrity=new FrmDatabaseIntegrity();
					frmDatabaseIntegrity.MessageToShow=databaseIntegrity.Message;
					frmDatabaseIntegrity.ShowDialog();
				}
			}
			float scaleZoom=LayoutManager.ScaleMyFont();
			imageSelector.LayoutTransform=new System.Windows.Media.ScaleTransform(scaleZoom,scaleZoom);
			windowingSlider.LayoutTransform=new System.Windows.Media.ScaleTransform(scaleZoom,scaleZoom);
			zoomSlider.LayoutTransform=new System.Windows.Media.ScaleTransform(scaleZoom,scaleZoom);
			toolBarMain.LayoutTransform=new System.Windows.Media.ScaleTransform(scaleZoom,scaleZoom);
			unmountedBar.LayoutTransform=new System.Windows.Media.ScaleTransform(scaleZoom,scaleZoom);
			Plugins.HookAddCode(this,"ContrImages.ModuleSelected_end",patNum,docNum);
		}
		
		///<summary></summary>
		public void ModuleUnselected(){
			_familyCur=null;
			//We will not close any undocked floaters here.
			//So _listFormImageFloats remains valid and still has all the floaters in it, even when we are in the Chart module. 
			//In CloseFloaters below, we close the floaters when changing patients.
			_patNumLastSecurityLog=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			Plugins.HookAddCode(this,"ContrImages.ModuleUnselected_end");
		}

		///<summary>Called when changing patients by any means.  Closes the undocked floating image windows.</summary>
		public void CloseFloaters(){
			for(int i=_listFormImageFloats.Count-1;i>=0;i--){//go backwards
				if(!_listFormImageFloats[i].IsDisposed){
					_listFormImageFloats[i].Close();//remove gets handled automatically here
				}
			}
		}

		///<summary>Launches a new floater. Does not reuse any existing floater or dock. Pass in an existing controlImageDisplay that already has an image in it. This is used when popping a dock to a floater. </summary>
		private void LaunchFloater(ControlImageDisplay controlImageDisplay,int x,int y,int w,int h,bool isMin=false,bool isMax=false){
			//controlImageDisplay is already filled, which means the rest of the module is also already set up properly with the correct patient, etc.
			FormImageFloat formImageFloat=new FormImageFloat();
			formImageFloat.IsImageFloatSelected=true;
			formImageFloat.FuncListFloaters=()=>_listFormImageFloats;
			formImageFloat.FuncDockedTitle=()=>controlImageDock.Text;
			formImageFloat.SetControlImageDisplay(controlImageDisplay);
			SetTitle(controlImageDisplay,formImageFloat);
			formImageFloat.Activated +=FormImageFloat_Activated;
			formImageFloat.FormClosed += FormImageFloat_FormClosed;
			formImageFloat.EventButClicked+=DockOrFloat_EventButClicked;
			formImageFloat.EventWinPicked+=DockOrFloat_EventWinClicked;
			_listFormImageFloats.Add(formImageFloat);
			formImageFloat.Show();//triggers Activated, then imageSelector.SetSelected 
			//Note: Must use Bounds here. Using DesktopBounds makes it shift off by about 200px.
			formImageFloat.Bounds=new Rectangle(x,y,w,h);
			//formImageFloat.RestoreBounds=new Rectangle(x,y,w,h);//can only get
			if(isMax){
				formImageFloat.WindowState=FormWindowState.Maximized;
			}
			formImageFloat.Bounds=new Rectangle(x,y,w,h);//#2
			//the above line can trigger a resize due to dpi change, so once more:
			formImageFloat.Bounds=new Rectangle(x,y,w,h);//#3
			Point pointMousePosDesktop=Cursor.Position;//variable is for debugging
			//Point pointMouseDownLocal=formImageFloat.PointToClient(pointMousePosDesktop);
			//Point pointMouseScreen=formImageFloat.PointToScreen(pointMouseDownLocal);
			//Must use DesktopBounds below because that's how base does it.
			formImageFloat.SimulateMouseDown(pointMousePosDesktop,formImageFloat.DesktopBounds);
			controlImageDisplay.SetZoomSliderToFit();
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			controlImageDock.SetControlImageDisplay(null);//this unfortunately clears the selected item in the image selector
			//controlImageDisplay.ClearObjects();
			//SelectTreeNode must come before Show. Must come after bounds are set for the zoom to be correct.
			SelectTreeNode1(nodeTypeAndKey);
			if(isMin){
				formImageFloat.WindowState=FormWindowState.Minimized;
			}
		}

		///<summary>Launches a new floater. Does not reuse any existing floater or dock. This is used from Chart module when double clicking on a thumbnail.</summary>
		public void LaunchFloaterFromChart(long patNum,long docNum,long mountNum){
			ControlImageDisplay controlImageDisplay=CreateControlImageDisplay();
			_familyCur=Patients.GetFamily(patNum);
			_patient=_familyCur.GetPatient(patNum);
			_patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			controlImageDisplay.PatientCur=_patient;
			controlImageDisplay.PatFolder=_patFolder;
			controlImageDisplay.ZoomSliderValue=zoomSlider.Value;
			controlImageDisplay.DidLaunchFromChartModule=true;
			FormImageFloat formImageFloat=new FormImageFloat();
			formImageFloat.IsImageFloatSelected=true;
			formImageFloat.FuncListFloaters=()=>_listFormImageFloats;
			formImageFloat.FuncDockedTitle=()=>controlImageDock.Text;
			formImageFloat.Activated +=FormImageFloat_Activated;
			formImageFloat.FormClosed += FormImageFloat_FormClosed;
			formImageFloat.EventButClicked+=DockOrFloat_EventButClicked;
			formImageFloat.EventWinPicked+=DockOrFloat_EventWinClicked;
			_listFormImageFloats.Add(formImageFloat);
			System.Windows.Forms.Screen[] screenArray=System.Windows.Forms.Screen.AllScreens;
			Size size=new Size();
			Point location=new Point();
			if(screenArray.Length==1){
				System.Windows.Forms.Screen screen=screenArray[0];
				//show on this screen.
				//I would rather calculate the ratio of the image so that I can make the window the same size.
				//But to do that, I would need to load the bitmap, rotate as needed, and then do the calc.  Maybe later.
				//So make it square, about 50% of height. Push it to the right.  They can resize.
				size=new Size(screen.WorkingArea.Height/2,screen.WorkingArea.Height/2);
				location=new Point(screen.WorkingArea.Right-size.Width,screen.WorkingArea.Top+screen.WorkingArea.Height/4);//centered up and down
				formImageFloat.SetDesktopBounds(location.X,location.Y,size.Width,size.Height);
			}
			else{
				//show on other screen, maximized
				System.Windows.Forms.Screen screen=screenArray[0];
				if(screen==System.Windows.Forms.Screen.FromControl(this)){
					screen=screenArray[1];
				}
				//set a moderate size in case they unmaximize
				size=new Size(screen.WorkingArea.Width/2,screen.WorkingArea.Height/2);
				location=new Point(screen.WorkingArea.Left+screen.WorkingArea.Width/4,screen.WorkingArea.Top+screen.WorkingArea.Height/4);//centered
				//but set the size to the full screen so that the zoom will be correct.
				formImageFloat.SetDesktopBounds(screen.WorkingArea.X,screen.WorkingArea.Y,screen.WorkingArea.Width,screen.WorkingArea.Height);
			}
			NodeTypeAndKey nodeTypeAndKey=null;
			if(docNum>0){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,docNum);
			}
			if(mountNum>0){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Mount,mountNum);
			}
			controlImageDisplay.ClearObjects();
			SetDocumentOrMount(nodeTypeAndKey,controlImageDisplay);//Set _mountShowing prior to SelectTreeNode2() to avoid UE when _mountShowing is null.
			//SelectTreeNode must come before Show. Must come after bounds are set for the zoom to be correct.
			controlImageDisplay.SelectTreeNode2(nodeTypeAndKey);//must come before Show, but after bounds set.
			formImageFloat.SetControlImageDisplay(controlImageDisplay);
			SelectTreeNode1(nodeTypeAndKey,isChartModuleFloater:true);
			formImageFloat.Show();//triggers Activated, then imageSelector.SetSelected
			formImageFloat.SetDesktopBounds(location.X,location.Y,size.Width,size.Height);//#2
			//the above line can trigger a resize due to dpi change, so once more:
			formImageFloat.SetDesktopBounds(location.X,location.Y,size.Width,size.Height);//#3
			if(screenArray.Length>1){
				formImageFloat.WindowState=FormWindowState.Maximized;
			}
			formImageFloat.ControlImageDisplay_.SetZoomSliderToFit();
		}

		///<summary>Fired when user clicks on tree and also for automated selection that's not by mouse, such as image import, image paste, etc.  Can pass in NULL to clear the image.  localPathImported will be set only if using Cloud storage and an image was imported.  We want to use the local version instead of re-downloading what was just uploaded.  nodeObjTag does not need to be ref to same object, but must match type and priKey.</summary>
		public void SelectTreeNode1(NodeTypeAndKey nodeTypeAndKey,string localPathImportedCloud="",bool isChartModuleFloater=false) {
			//Select the node always, but perform additional tasks when necessary (i.e. load an image, or mount).	
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType!=EnumImageNodeType.None){	
				imageSelector.SetSelected(nodeTypeAndKey.NodeType,nodeTypeAndKey.PriKey);//this is redundant when user is clicking, but harmless 
			}
			ControlImageDisplay controlImageDisplay=null;
			if(controlImageDock.ControlImageDisplay_!=null && controlImageDock.ControlImageDisplay_.GetNodeTypeAndKey().IsMatching(nodeTypeAndKey) && !isChartModuleFloater) {//Ignore Image Module Dock if launching a floater in Chart Module
				//The one we want is docked
				controlImageDisplay=controlImageDock.ControlImageDisplay_;
				controlImageDock.Select();
				ControlImageDock_GotODFocus(controlImageDock,new EventArgs());//This enables toolbar buttons, etc
			}
			FormImageFloat formImageFloat=null;
			if(controlImageDisplay==null){
				formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.ControlImageDisplay_.GetNodeTypeAndKey().IsMatching(nodeTypeAndKey));
				if(formImageFloat!=null){
					//The one we want is in a floater
					controlImageDisplay=formImageFloat.ControlImageDisplay_;
					formImageFloat.Select();//This triggers FormImageFloat_Activated which enables toolbar buttons, etc
					if(formImageFloat.WindowState==FormWindowState.Minimized){
						formImageFloat.Restore();
					}
					formImageFloat.BringToFront();
					//when this is called by from imageSelector_SelectionChangeCommitted, additional effort is made there to keep the floater on top.
				}
			}
			if(controlImageDisplay!=null){//found the doc/mount we're after already showing
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				controlImageDisplay.ClearObjects();
				SetDocumentOrMount(nodeTypeAndKey,controlImageDisplay,formImageFloat);
				controlImageDisplay.SelectTreeNode2(nodeTypeAndKey,localPathImportedCloud);
				SetTitle(controlImageDisplay,formImageFloat);
				if(controlImageDisplay.IsDisposed){
					//see note 35 lines down
					return;
				}
				controlImageDisplay.Select();
				if(IsMountShowing()){
					unmountedBar.SetObjects(controlImageDisplay.GetUmountedObjs());
					unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
				}
				SetPanelNoteVisibility();
				_isAcquiring=false;
				SetUnmountedBarVisibility();
				LayoutControls();
				//SetZoomSlider();
				FillSignature();
				controlImageDisplay.EnableToolBarButtons();
				EnableMenuItemTreePrintHelper(controlImageDisplay);
				//Debug.WriteLine("End of SelectTreeNode");
				return;
			}
			//From here down, the doc/mount was not found, so we need to load it up.
			//This always happens on controlImageDock instead of any floater.
			if(nodeTypeAndKey is null 
				|| nodeTypeAndKey.NodeType==EnumImageNodeType.None
				|| nodeTypeAndKey.NodeType==EnumImageNodeType.Category)
			{
//todo: controlImageDisplay.EnableToolBarButtons();
				controlImageDock.SetControlImageDisplay(null);
			}
			else{
				controlImageDisplay=CreateControlImageDisplay();
				//close the draw panel because we are going to change to a different image in the window
				panelDraw.Visible=false;
				controlImageDisplay.PatientCur=_patient;
				controlImageDisplay.PatFolder=_patFolder;
				controlImageDisplay.ZoomSliderValue=zoomSlider.Value;
				controlImageDock.SetControlImageDisplay(controlImageDisplay);
				controlImageDock.IsImageFloatSelected=true;
				SetDrawMode(EnumDrawMode.None);
				//deactivate all floaters
				for(int i=0;i<_listFormImageFloats.Count;i++){
					_listFormImageFloats[i].IsImageFloatSelected=false;
				}
				controlImageDisplay.ClearObjects();
				SetDocumentOrMount(nodeTypeAndKey,controlImageDisplay);
				controlImageDisplay.SelectTreeNode2(nodeTypeAndKey,localPathImportedCloud);
				SetTitle(controlImageDisplay);
				controlImageDock.Select();
				ControlImageDock_GotODFocus(controlImageDock,new EventArgs());//This enables toolbar buttons, etc
				if(IsMountShowing()){
					unmountedBar.SetObjects(controlImageDisplay.GetUmountedObjs());
					unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
				}
				controlImageDisplay.EnableToolBarButtons();
				EnableMenuItemTreePrintHelper(controlImageDisplay);
			}
			SetPanelNoteVisibility();
			_isAcquiring=false;
			SetUnmountedBarVisibility();
			LayoutControls();
			//SetZoomSlider();
			FillSignature();
		}

		///<summary>This loads from db</summary>
		public void SetDocumentOrMount(NodeTypeAndKey nodeTypeAndKey,ControlImageDisplay controlImageDisplay,FormImageFloat formImageFloat=null){
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType==EnumImageNodeType.Document){
				Document document=Documents.GetByNum(nodeTypeAndKey.PriKey,doReturnNullIfNotFound:true);
				if(document==null) {
					//must use specific controlImageDisplay here because we have not yet set selected
					controlImageDisplay.SetDocumentShowing(0,null);
					MsgBox.Show(this,"Document was previously deleted.");
					FillImageSelector(keepSelection:false);
				}
				else{
					controlImageDisplay.SetDocumentShowing(0,document);
				}
			}
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType==EnumImageNodeType.Mount){
				Mount mount=Mounts.GetByNum(nodeTypeAndKey.PriKey);
				controlImageDisplay.SetMountShowing(mount);
			}
		}

		///<summary>This sets the title text of the docked or floater titlebar. This gets done after SetDocumentOrMount. It also must be done after SelectTreeNode or the NodeTypeAndKey won't be set yet.</summary>
		public void SetTitle(ControlImageDisplay controlImageDisplay,FormImageFloat formImageFloat=null){
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			Document document=controlImageDisplay.GetDocumentShowing(0);
			Mount mount=controlImageDisplay.GetMountShowing();
			string text="";
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType==EnumImageNodeType.Document && document!=null){
				text=document.DateCreated.ToShortDateString()+": "+document.Description;
				//no need to show document note in title because it shows as bottom in separate panel.
				if(document.ToothNumbers!=""){
					if(document.ToothNumbers.Contains(",")){
						text+=", "+Lan.g(this,"Teeth")+": ";
					}
					else{
						text+=", "+Lan.g(this,"Tooth")+": ";
					}
					text+=Tooth.DisplayRange(document.ToothNumbers);
				}
			}
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType==EnumImageNodeType.Mount && mount!=null){
				text=mount.DateCreated.ToShortDateString()+": "+mount.Description;
				if(mount.Note!=""){
					text+=", "+mount.Note;
				}
			}
			if(formImageFloat==null){
				controlImageDock.Text=text;
			}
			else{
				formImageFloat.Text=text;
			}
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

		private void butGoTo_Click(object sender,EventArgs e) {
			if(!Directory.Exists(textImportAuto.Text)){
				MsgBox.Show(this,"Folder does not exist.");
				return;
			}
			Process.Start(textImportAuto.Text);
		}

		private void butImportClose_Click(object sender,EventArgs e) {
			panelImportAuto.Visible=false;
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay==null){
				return;
			}
			_isAcquiring=false;
			SetUnmountedBarVisibility();
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
			PreselectFirstItem();//We already did this when clicking toolbar button, but user might have selected a mount after clicking that.
			if(IsMountShowing()){
				List<MountItem> listAvail=GetAvailSlots(1);
				if(listAvail is null){
					//no more available slots, so start saving in unmounted area
				}
				else if(IsMountItemSelected() || GetIdxSelectedInMount()==-1){
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
			_isAcquiring=true;
			SetUnmountedBarVisibility();
			LayoutControls();
		}
		
		private void butStart_Click(object sender, EventArgs e){	
			
		}

		private void ContrImages_Resize(object sender, EventArgs e){
			LayoutControls();
		}

		private void DockOrFloat_EventButClicked(object sender,WpfControls.UI.EnumImageFloatWinButton enumImageFloatWinButton) {
			//MessageBox.Show(this,enumImageFloatWinButton.ToString());//Must specify owner because the floater closes
			ControlImageDisplay controlImageDisplay=null;
			//sender is either controlImageDock or FormImageFloat
			System.Drawing.Point drawing_PointScreen=new Point();
			if(sender is ControlImageDock){
				controlImageDisplay=controlImageDock.ControlImageDisplay_;
				drawing_PointScreen=PointToScreen(new Point(0,0));//UL corner of this control, converted to screen coords
			}
			FormImageFloat formImageFloat=null;
			if(sender is FormImageFloat){
				formImageFloat=(FormImageFloat)sender;
				controlImageDisplay=formImageFloat.ControlImageDisplay_;
				drawing_PointScreen=formImageFloat.PointToScreen(new Point(0,0));//UL corner of that form, converted to screen coords
			}
			//screenThis refers to the screen where the sender is located.
			//screen2 is the "other" screen.
			System.Windows.Forms.Screen screenThis=System.Windows.Forms.Screen.FromPoint(drawing_PointScreen);
			System.Windows.Forms.Screen screen2=null;
			System.Windows.Forms.Screen[] screenArray=System.Windows.Forms.Screen.AllScreens;
			if(screenArray.Length>1){
				screen2=screenArray[0];
				if(screen2.Bounds==screenThis.Bounds){//probably a better way to do this
					screen2=screenArray[1];
				}
			}
			Rectangle rectangle=new Rectangle();
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Minimize
				|| enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Maximize)
			{
				//We want to set a "medium" size first for when they restore from max or min.
				//I would rather calculate the ratio of the image so that I can make the window the same size.
				//But to do that, I would need to load the bitmap, rotate as needed, and then do the calc.  Maybe later.
				//So make it square, about 50% of height. Push it to the right.  They can resize.
				int width=screenThis.WorkingArea.Height/2;
				rectangle=new Rectangle(
					screenThis.WorkingArea.Right-width,
					screenThis.WorkingArea.Top+screenThis.WorkingArea.Height/4,//centered up and down
					width,
					width);
				if(sender is FormImageFloat){
					if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Minimize){
						formImageFloat.WindowState=FormWindowState.Minimized;
					}
					else if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Maximize){
						formImageFloat.WindowState=FormWindowState.Maximized;
					}
					controlImageDisplay.SetZoomSliderToFit();
				}
				else{
					if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Minimize){
						LaunchFloater(controlImageDisplay,rectangle.X,rectangle.Y,rectangle.Width,rectangle.Height,isMin:true);
					}
					if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Maximize){
						LaunchFloater(controlImageDisplay,rectangle.X,rectangle.Y,rectangle.Width,rectangle.Height,isMax:true);
					}
				}
				return;
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.CloseOthers){
				//if(sender is FormImageFloat){
					//controlImageDock.SetControlImageDisplay(null);
					//I decided that it felt better for this button to not close the docked image.
					//So it's implied that it means "other floaters".
				//}
				for(int i=_listFormImageFloats.Count-1;i>=0;i--){//go backwards
					if(_listFormImageFloats[i]==formImageFloat){
						continue;
					}
					if(!_listFormImageFloats[i].IsDisposed){
						_listFormImageFloats[i].Close();//remove gets handled automatically here
					}
				}
				return;
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.DockThis){
				if(sender is FormImageFloat){
					//controlImageDisplay is already filled, which means the rest of the module is also already set up properly with the correct patient, etc.
					controlImageDock.SetControlImageDisplay(controlImageDisplay);
					controlImageDisplay.SetZoomSliderToFit();
					formImageFloat.Close();//remove gets handled automatically here
					//The above causes formImageFloat.Activated to fire, maybe because _windowImageFloatWindows is still open at this point?
					//controlImageDock.IsImageFloatSelected=true;
					NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
					controlImageDock.SetControlImageDisplay(null);//this unfortunately clears the selected item in the image selector
					//controlImageDisplay.ClearObjects();
					//SelectTreeNode must come before Show. Must come after bounds are set for the zoom to be correct.
					SelectTreeNode1(nodeTypeAndKey);
				}
				return;
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.ShowAll){
				for(int i=0;i<_listFormImageFloats.Count;i++){
					if(_listFormImageFloats[i].WindowState==FormWindowState.Minimized){
						_listFormImageFloats[i].Restore();
					}
					_listFormImageFloats[i].BringToFront();
				}
				if(sender is FormImageFloat){
					formImageFloat.BringToFront();
				}
				return;
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Half_L){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left,
					y:screenThis.WorkingArea.Top,
					screenThis.WorkingArea.Width/2,
					screenThis.WorkingArea.Height);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Half_R){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left+screenThis.WorkingArea.Width/2,
					y:screenThis.WorkingArea.Top,
					screenThis.WorkingArea.Width/2,
					screenThis.WorkingArea.Height);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Center){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left+screenThis.WorkingArea.Width/5,
					y:screenThis.WorkingArea.Top+screenThis.WorkingArea.Height/8,
					screenThis.WorkingArea.Width/5*3,
					screenThis.WorkingArea.Height/8*6);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_UL){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left,
					y:screenThis.WorkingArea.Top,
					screenThis.WorkingArea.Width/2,
					screenThis.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_UR){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left+screenThis.WorkingArea.Width/2,
					y:screenThis.WorkingArea.Top,
					screenThis.WorkingArea.Width/2,
					screenThis.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_LL){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left,
					y:screenThis.WorkingArea.Top+screenThis.WorkingArea.Height/2,
					screenThis.WorkingArea.Width/2,
					screenThis.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_LR){
				rectangle=new Rectangle(
					x:screenThis.WorkingArea.Left+screenThis.WorkingArea.Width/2,
					y:screenThis.WorkingArea.Top+screenThis.WorkingArea.Height/2,
					screenThis.WorkingArea.Width/2,
					screenThis.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Half_L2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left,
					y:screen2.WorkingArea.Top,
					screen2.WorkingArea.Width/2,
					screen2.WorkingArea.Height);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Half_R2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left+screen2.WorkingArea.Width/2,
					y:screen2.WorkingArea.Top,
					screen2.WorkingArea.Width/2,
					screen2.WorkingArea.Height);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Center2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left+screen2.WorkingArea.Width/5,
					y:screen2.WorkingArea.Top+screen2.WorkingArea.Height/8,
					screen2.WorkingArea.Width/5*3,
					screen2.WorkingArea.Height/8*6);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_UL2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left,
					y:screen2.WorkingArea.Top,
					screen2.WorkingArea.Width/2,
					screen2.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_UR2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left+screen2.WorkingArea.Width/2,
					y:screen2.WorkingArea.Top,
					screen2.WorkingArea.Width/2,
					screen2.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_LL2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left,
					y:screen2.WorkingArea.Top+screen2.WorkingArea.Height/2,
					screen2.WorkingArea.Width/2,
					screen2.WorkingArea.Height/2);
			}
			if(enumImageFloatWinButton==WpfControls.UI.EnumImageFloatWinButton.Quarter_LR2 && screen2!=null){
				rectangle=new Rectangle(
					x:screen2.WorkingArea.Left+screen2.WorkingArea.Width/2,
					y:screen2.WorkingArea.Top+screen2.WorkingArea.Height/2,
					screen2.WorkingArea.Width/2,
					screen2.WorkingArea.Height/2);
			}
			if(sender is FormImageFloat){
				if(formImageFloat.WindowState==FormWindowState.Maximized){
					formImageFloat.WindowState=FormWindowState.Normal;
				}
				formImageFloat.Bounds=rectangle;
				controlImageDisplay.SetZoomSliderToFit();
			}
			else{
				LaunchFloater(controlImageDisplay,rectangle.X,rectangle.Y,rectangle.Width,rectangle.Height);
			}
		}

		private void DockOrFloat_EventWinClicked(object sender,int idx) {
			if(idx==0){//idx docker
				//It would be hard to bring OD to the front, so we won't do anything.
				return;
			}
			int idxInList=idx-1;
			if(idxInList<0) {
				return;
			}
			if(idxInList>_listFormImageFloats.Count-1) {
				return;
			}
			if(_listFormImageFloats[idxInList].WindowState==FormWindowState.Minimized) {
				_listFormImageFloats[idxInList].Restore();
			}
			_listFormImageFloats[idxInList].BringToFront();
			_listFormImageFloats[idxInList].Select();
		}

		private void Docker_EventPopFloater(object sender,EventArgs e) {
			//calculate rectangle in screen coords
			Point pointUL=PointToScreen(controlImageDock.Location);
			Point pointBR=PointToScreen(new Point(controlImageDock.Right,controlImageDock.Bottom));
			Rectangle rectangle=new Rectangle(
				pointUL.X,
				pointUL.Y,
				pointBR.X-pointUL.X,
				pointBR.Y-pointUL.Y);
			ControlImageDisplay controlImageDisplay=controlImageDock.ControlImageDisplay_;
			LaunchFloater(controlImageDisplay,rectangle.X,rectangle.Y,rectangle.Width,rectangle.Height);
		}

		private void imageSelector_DragDropImport(object sender,DragDropImportEventArgs e) {
			//We already asked the user to confirm.
			//This part will be silent, without additional popups.
			if(e.ListFileNames.Count==0){
				return;
			}
			Document document=null;
			for(int i=0;i<e.ListFileNames.Count;i++){
				document=ImageStore.Import(e.ListFileNames[i],e.DefNumNew,_patient);//Makes log
			}
			FillImageSelector(false);
			NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum);
			SelectTreeNode1(nodeTypeAndKey);
		}

		private void imageSelector_DraggedToCategory(object sender,DragEventArgsOD e) {
			if(e.DocNum!=0){
				//keep in mind that no change as been made to the selection
				Document document=GetDocumentShowing(0);
				//Get the document from the database if the selected FormImageFloat is no longer available or is for a different document.
				if(document==null || document.DocNum!=e.DocNum) {
					document=Documents.GetByNum(e.DocNum,doReturnNullIfNotFound:true);
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
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,document.PatNum,logText,document.DocNum,document.DateTStamp);
				Document docOld=document.Copy();
				document.DocCategory=e.DefNumNew;
				Documents.Update(document,docOld);
			}
			else if(e.MountNum!=0){
				Mount mountShowing=GetMountShowing();
				string mountOriginalCat=Defs.GetDef(DefCat.ImageCats,mountShowing.DocCategory).ItemName;
				string mountNewCat=Defs.GetDef(DefCat.ImageCats,e.DefNumNew).ItemName;
				SecurityLogs.MakeLogEntry(EnumPermType.ImageEdit,mountShowing.PatNum,Lan.g(this,"Mount moved from")+" "+mountOriginalCat+" "
					+Lan.g(this,"to")+" "+mountNewCat);
				mountShowing.DocCategory=e.DefNumNew;
				Mounts.Update(mountShowing);
				Documents.UpdateDocCategoryForMountItems(mountShowing.MountNum,mountShowing.DocCategory);
			}
			FillImageSelector(true);
		}

		private void imageSelector_ItemDoubleClick(object sender,EventArgs e) {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay!=null && !controlImageDisplay.HideWebBrowser()) {
				MsgBox.Show(this,"The PDF viewer is busy loading the document and cannot be opened in its default program yet. Please try again.");
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None
				|| imageSelector.GetSelectedType()==EnumImageNodeType.Category)
			{
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount) {
				FrmMountEdit frmMountEdit=new FrmMountEdit();
				frmMountEdit.MountCur=GetMountShowing();
				frmMountEdit.ShowDialog();//Edits the MountSelected object directly and updates and changes to the database as well.
				//Always reload because layout could have changed
				FillImageSelector(true);//in case description for the mount changed.
				imageSelector.SetSelected(EnumImageNodeType.Mount,GetMountShowing().MountNum);//Need to update _nodeObjTagSelected in case category changed
				controlImageDisplay.ClearObjects();
				NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
				SetDocumentOrMount(nodeTypeAndKey,controlImageDisplay);
				//Jordan 2024-08-05 I suspect we might need to be calling SelectTreeNode1 here instead. Maybe.
				controlImageDisplay.SelectTreeNode2(nodeTypeAndKey);
				unmountedBar.SetObjects(controlImageDisplay.GetUmountedObjs());
				unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
				return;			
			}
			//From here down is Document=================================================================================================
			Document document=GetDocumentShowing(0);
			if(document==null){
				return;//Unexplained error
			}
			string ext=ImageStore.GetExtension(document);
			List<string> listImageExtensions=new List<string> {".bmp",".dcm",".dicom",".gif",".jpg",".jpeg",".png",".tif",".tiff"};
			if(listImageExtensions.Contains(ext)) {
				FrmDocInfo frmDocInfo=new FrmDocInfo(_patient,GetDocumentShowing(0));
				frmDocInfo.ShowDialog();
				if(frmDocInfo.IsDialogCancel) {
					return;
				}
				FillImageSelector(true);
				imageSelector.SetSelected(EnumImageNodeType.Document,GetDocumentShowing(0).DocNum);//Need to update _nodeObjTagSelected in case category changed
				return;
			}
			//from here down is attempting to launch document in separate external software=========================================================
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Images stored directly in database. Export file in order to open with external program.");
				return;//Documents must be stored in the A to Z Folder to open them outside of Open Dental.  Users can use the export button for now.
			}
			//We allow anything which ends with a different extention to be viewed in the windows fax viewer.
			//Specifically, multi-page faxes can be viewed more easily by one of our customers using the fax viewer.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string tempFile=ImageStore.GetFilePath(document,_patFolder);
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else if(ODCloudClient.IsAppStream) {
					CloudClientL.ExportForCloud(tempFile,doPromptForName:false);
				}
				else {
					try {
						string filePath=ImageStore.GetFilePath(document,_patFolder);
						Process.Start(filePath);
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
				}
			}
			else {//Cloud
				//Download document into temp directory for displaying.
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(GetDocumentShowing(0).FileName));
				ProgressWin progressWin=new UI.ProgressWin();
				progressWin.StartingMessage="Downloading...";
				progressWin.ActionMain=() => {
					byte[] byteArray=CloudStorage.Download(_patFolder.Replace("\\","/"),GetDocumentShowing(0).FileName);
					File.WriteAllBytes(tempFile,byteArray);
				};
				progressWin.ShowDialog();
				if(progressWin.IsCancelled){
					return;
				}	
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else if(ODCloudClient.IsAppStream){
					CloudClientL.ExportForCloud(tempFile,doPromptForName:false);
				}
				else {
					Process.Start(tempFile);
				}
			}
		}

		private void FormImageFloat_Activated(object sender, EventArgs e){
			//There is an analogous event handler for controlImageDock called ControlImageDock_GotODFocus.
			//The purpose of this is to react when the floater gets focus.
			//This gets hit frequently, though: every time user clicks a button in toolbar and then goes back in here.
			//So this usually needs to not really do anything.
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			//Debug.WriteLine(DateTime.Now.ToString()+" FormImageFloat_Activated");
			//We want to SetDrawMode(EnumDrawMode.None) and hide the draw toobar, but only if we change the selected floater.
			if(!formImageFloat.IsImageFloatSelected){
				SetCropPanAdj(EnumCropPanAdj.Pan);
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				LayoutControls();
			}
			//deactivate all other floaters and the dock
			controlImageDock.IsImageFloatSelected=false;
			for(int i=0;i<_listFormImageFloats.Count;i++){
				if(_listFormImageFloats[i]!=sender){
					_listFormImageFloats[i].IsImageFloatSelected=false;
				}
			}
			formImageFloat.IsImageFloatSelected=true;
			ControlImageDisplay controlImageDisplay=formImageFloat.ControlImageDisplay_;
			controlImageDisplay.EnableToolBarButtons();
			EnumImageNodeType imageNodeType=controlImageDisplay.GetSelectedType();
			if(ODBuild.IsDebug() && controlImageDisplay.Parent!=null) {
				string parent=controlImageDisplay.Parent.GetType().ToString();
				//Debug.WriteLine("ControlImages.FormImageFloat_Activated, Parent:"+parent);
			}
			long priKey=controlImageDisplay.GetSelectedPriKey();
			imageSelector.SetSelected(imageNodeType,priKey);
			ZoomSliderState zoomSliderState=controlImageDisplay.ZoomSliderStateInitial;
			if(zoomSliderState!=null){
				zoomSlider.SetValueInitialFit(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
				zoomSlider.SetValueAndMax(controlImageDisplay.ZoomSliderValue);
			}
			controlImageDisplay.SetWindowingSlider();
		}
		
		private void ControlImageDock_GotODFocus(object sender, EventArgs e){
			//There is an analogous event handler for each formImageFloat called FormImageFloat_Activated.
			//The purpose of this is to react when the floater gets focus.
			//This gets hit frequently, though: every time user clicks a button in toolbar and then goes back in here.
			//So this usually needs to not really do anything.
			//Also, this does not automatically get raised like FormImageFloat_Activated. We must always manually trigger it,
			//whether through click events or otherwise.
			//Debug.WriteLine(DateTime.Now.ToString()+" ControlImageDock_GotODFocus");
			//We want to SetDrawMode(EnumDrawMode.None) and hide the draw toobar, but only if we change the selected floater.
			if(!controlImageDock.IsImageFloatSelected){
				SetCropPanAdj(EnumCropPanAdj.Pan);
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				LayoutControls();
			}
			//deactivate all floaters
			for(int i=0;i<_listFormImageFloats.Count;i++){
				_listFormImageFloats[i].IsImageFloatSelected=false;
			}
			controlImageDock.IsImageFloatSelected=true;
			ControlImageDisplay controlImageDisplay=controlImageDock.ControlImageDisplay_;
			controlImageDisplay.EnableToolBarButtons();
			EnumImageNodeType imageNodeType=controlImageDisplay.GetSelectedType();
			long priKey=controlImageDisplay.GetSelectedPriKey();
			imageSelector.SetSelected(imageNodeType,priKey);
			ZoomSliderState zoomSliderState=controlImageDisplay.ZoomSliderStateInitial;
			if(zoomSliderState!=null){
				zoomSlider.SetValueInitialFit(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
				zoomSlider.SetValueAndMax(controlImageDisplay.ZoomSliderValue);
			}
			controlImageDisplay.SetWindowingSlider();
		}

		private void FormImageFloat_KeyDown(KeyEventArgs e) {
			EventKeyDown?.Invoke(this,e);
		}

		private void FormImageFloat_FormClosed(object sender, FormClosedEventArgs e){
			//There is analogous event handler for controlImageDock called ControlImageDock_ImageClosed
			//The goal here is to clear out the rest of the UI when an image truly closes.
			//Hopefully, we have done this without causing annoyance when user is switching to another image.
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			SetDrawMode(EnumDrawMode.None);
			panelDraw.Visible=false;
			_listFormImageFloats.Remove(formImageFloat);
			DisableAllToolBarButtons();
			FillImageSelector(false);
			//In case the image had a signature or note:
			SetPanelNoteVisibility();
			SetUnmountedBarVisibility();
			LayoutControls();
			FillSignature();
		}

		private void ControlImageDock_ImageClosed(object sender, EventArgs e){
			//There is analogous event handler for each formImageFloat called FormImageFloat_FormClosed
			//The goal here is to clear out the rest of the UI when an image truly closes.
			//Hopefully, we have done this without causing annoyance when user is switching to another image.
			SetDrawMode(EnumDrawMode.None);
			panelDraw.Visible=false;
			DisableAllToolBarButtons();
			FillImageSelector(false);
			//In case the image had a signature or note:
			SetPanelNoteVisibility();
			SetUnmountedBarVisibility();
			LayoutControls();
			FillSignature();
		}

		private void FormImageFloat_IsImageFloatDockedChanged(object sender,EventArgs e) {
			SetPanelNoteVisibility();
			SetUnmountedBarVisibility();
			LayoutControls();
			FillSignature();
		}

		private void FormImageFloat_SetWindowingSlider(object sender,WindowingEventArgs windowingEventArgs){
			windowingSlider.MinVal=windowingEventArgs.MinVal;
			windowingSlider.MaxVal=windowingEventArgs.MaxVal;
		}

		private void FormImageFloat_WindowClicked(object sender, int idx){
			if(idx>_listFormImageFloats.Count-1){
				return;
			}
			if(_listFormImageFloats[idx].WindowState==FormWindowState.Minimized){
			_listFormImageFloats[idx].Restore();
			}
			_listFormImageFloats[idx].BringToFront();
			_listFormImageFloats[idx].Select();
		}

		private void FormImageFloat_WindowCloseOthers(object sender, EventArgs e){
			if(_listFormImageFloats.Count==0){
				return;
			}
			for(int i=_listFormImageFloats.Count-1;i>=0;i--){//go backwards
				if(_listFormImageFloats[i]==(FormImageFloat)sender){
					continue;
				}
				if(!_listFormImageFloats[i].IsDisposed){
					_listFormImageFloats[i].Close();//remove gets handled automatically here
				}
			}
		}

		private void FormImageFloat_WindowDockThis(object sender, EventArgs e){
			if(sender is ControlImageDock){
				MessageBox.Show(this,Lan.g(this,"Already docked."));//must specify different owner because FormImageFloatWindows will close.
				return;
			}
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			if(controlImageDock.ControlImageDisplay_!=null){
				if(MessageBox.Show(this,Lan.g(this,"Another image is already docked. Dock this one instead?"),"",MessageBoxButtons.YesNo)!=DialogResult.Yes){
					return;
				}
			}
			ControlImageDisplay controlImageDisplay=formImageFloat.ControlImageDisplay_;
			controlImageDock.SetControlImageDisplay(controlImageDisplay);//This replaces the other docked control
			controlImageDisplay.SetZoomSliderToFit();
			formImageFloat.Close();//since controlImageDisplay is no longer a child, it will not be disposed.
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			SelectTreeNode1(nodeTypeAndKey);//This is necessary when this window replaces an old docked window
		}

		private void FormImageFloat_WindowShowAll(object sender, EventArgs e){
			for(int i=0;i<_listFormImageFloats.Count;i++){
				_listFormImageFloats[i].Show();
				if(_listFormImageFloats[i].WindowState==FormWindowState.Minimized){
					_listFormImageFloats[i].Restore();
				}
				_listFormImageFloats[i].BringToFront();
			}
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			formImageFloat.BringToFront();
		}

		private void imageSelector_ItemReselected(object sender,EventArgs e) {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.ClearObjects();
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			FormImageFloat formImageFloat=_listFormImageFloats
				.FirstOrDefault(x=>x.ControlImageDisplay_==controlImageDisplay);//will be null if docked
			SetDocumentOrMount(nodeTypeAndKey,controlImageDisplay,formImageFloat);
			controlImageDisplay.SelectTreeNode2(nodeTypeAndKey);
			SetTitle(controlImageDisplay,formImageFloat);
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount) {
				unmountedBar.SetObjects(controlImageDisplay.GetUmountedObjs());
				unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
			}
			//this resets zoom and pan
		}

		private void imageSelector_SelectionChangeCommitted(object sender,EventArgs e) {
			EnumImageNodeType nodeType=imageSelector.GetSelectedType();
			long priKey=imageSelector.GetSelectedKey();
			NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey(nodeType,priKey);
			SelectTreeNode1(nodeTypeAndKey);
			Application.DoEvents();
			FormImageFloat formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.ControlImageDisplay_.GetNodeTypeAndKey().IsMatching(nodeTypeAndKey));
			if(formImageFloat!=null){
				//The one we want is in a floater
				//There is an unknown event that internally causes main window to come to front.
				//The code below overcomes that.
				formImageFloat.BringToFront();
				formImageFloat.Focus();
				formImageFloat.TopMost=true;
				System.Windows.Forms.Timer timer=new System.Windows.Forms.Timer();
				timer.Interval=100;
				timer.Tick+=(sender,e)=>{
					formImageFloat.TopMost=false;
					timer.Stop();
					timer.Dispose();
				};
				timer.Start();
			}
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
			Document document = null;
			try{
				document=ImageStore.Import(e.FullPath,GetCurrentCategory(),_patient);
			}
			catch(Exception ex) {
				panelImportAuto.Visible=false;
				_fileSystemWatcher.EnableRaisingEvents=false;//will never be null
				MessageBox.Show(Lan.g(this,"Unable to import ")+e.FullPath+": "+ex.Message);
				return;
			}
			if(!IsMountShowing()){//single
				File.Delete(e.FullPath);
				FillImageSelector(false);//Reload and keep new document selected.
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				return;//user can take another single
			}
			//From here down is mount-----------------------------------------------------------------------------
			Bitmap bitmap=(Bitmap)Bitmap.FromFile(e.FullPath);
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();//will always succeed because we verified floater idxSelected in IsMountShowing()
			if(IsMountItemSelected()){
				//They shouldn't have selected an item, but we will try to find an unoccupied spot.
				List<MountItem> listAvail2=GetAvailSlots(1);
				if(listAvail2 is null){//no more available slots, so start saving in unmounted area
					SetIdxSelectedInMount(-1);
				}
				else{
					MountItem mountItem2=controlImageDisplay.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listAvail2[0].MountItemNum);
					if(mountItem2==null){//should not be possible
						return;
					}
					SetIdxSelectedInMount(controlImageDisplay.GetListMountItems().IndexOf(mountItem2));
				}
			}
			int idxSelectedInMount=GetIdxSelectedInMount();
			MountItem mountItem=null;
			if(idxSelectedInMount==-1){
				mountItem=new MountItem();
				mountItem.MountNum=GetMountShowing().MountNum;
				mountItem.ItemOrder=-1;//unmounted
				mountItem.Width=100;//arbitrary; it will scale
				mountItem.Height=100;
				MountItems.Insert(mountItem);
				WpfControls.UI.UnmountedObj unmountedObj=new WpfControls.UI.UnmountedObj();
				unmountedObj.MountItem_=mountItem;
				unmountedObj.Document_=document;
				unmountedObj.SetBitmap(new Bitmap(bitmap));
				unmountedBar.AddObject(unmountedObj);
			}
			else{
				mountItem=controlImageDisplay.GetListMountItems()[idxSelectedInMount];
			}
			controlImageDisplay.DocumentAcquiredForMount(document,bitmap,mountItem,GetMountShowing().FlipOnAcquire);
			bitmap?.Dispose();
			File.Delete(e.FullPath);
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listAvail=GetAvailSlots(1);
			if(listAvail is null){//no more available slots, so start saving in unmounted area
				SetIdxSelectedInMount(-1);
				//panelImportAuto.Visible=false;
				//_fileSystemWatcher.EnableRaisingEvents=false;//will never be null
				//MessageBox.Show(Lan.g(this,"No more available mount positions."));
				return;
			}
			mountItem=controlImageDisplay.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listAvail[0].MountItemNum);
			if(mountItem==null){//should not be possible
				return;
			}
			SetIdxSelectedInMount(controlImageDisplay.GetListMountItems().IndexOf(mountItem));
			//wait for next event to fire
		}

		private void formVideo_BitmapCaptured(object sender, Bitmap bitmap){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.formVideo_BitmapCaptured(sender,bitmap);
		}

		private void menuForms_Click(object sender,System.EventArgs e) {
			string formName=((WpfControls.UI.MenuItem)sender).Text;
			Document doc;
			try {
				doc=ImageStore.ImportForm(formName,GetCurrentCategory(),_patient);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FillImageSelector(false);
			SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			FrmDocInfo frmDocInfo=new FrmDocInfo(_patient,doc,isDocCreate:true);
			frmDocInfo.ShowDialog();//some of the fields might get changed, but not the filename
			if(frmDocInfo.IsDialogCancel) {
				DeleteDocument(false,false,doc);
			}
			else {
				FillImageSelector(true);//Refresh possible changes in the document due to FormD.
			}	
		}

		///<summary>This is the one for WinForms.</summary>
		private void menuTree_Click(object sender,System.EventArgs e) {
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None || imageSelector.GetSelectedType()==EnumImageNodeType.Category){
				return;//Probably the user has no patient selected
			}
			//Categories mostly blocked at the click
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			switch(((MenuItem)sender).Index) {
				case 0://print
					ToolBarPrint_Click(this,new EventArgs());
					break;
				case 1://delete
					controlImageDisplay.ToolBarDelete_Click(this,new EventArgs());
					break;
				case 2://info
					controlImageDisplay.ToolBarInfo_Click(this,new EventArgs());
					break;
			}
		}

		///<summary>This is the one for WPF</summary>
		private void menuTree_Click2(object sender,System.EventArgs e) {
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None || imageSelector.GetSelectedType()==EnumImageNodeType.Category){
				return;//Probably the user has no patient selected
			}
			//Categories mostly blocked at the click
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}

			switch(((WpfControls.UI.MenuItem)sender).Name) {
				case "Print":
					ToolBarPrint_Click(this,new EventArgs());
					break;
				case "Delete":
					controlImageDisplay.ToolBarDelete_Click(this,new EventArgs());
					break;
				case "Info":
					controlImageDisplay.ToolBarInfo_Click(this,new EventArgs());
					break;
			}
		}

		private void panelMain_Paint(object sender,PaintEventArgs e) {
			e.Graphics.Clear(Color.White);
		}

		private void panelNote_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click(this,new EventArgs());
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
			//if(widthNew>500){
			//	widthNew=500;
			//}
			//No more limit
			_widthTree96=widthNew;
			LayoutControls();
			#endregion Dragging
		}

		private void panelSplitter_MouseUp(object sender,MouseEventArgs e) {
			_isTreeDockerCollapsing=false;
			Point pointDelta =new Point(Control.MousePosition.X-_pointMouseDown.X,Control.MousePosition.Y-_pointMouseDown.Y);
			if(pointDelta.X<0 //moved to left any amount
				&& _widthTree96==0 //until tree width collapsed
				&& !_isTreeDockerCollapsed) //and was not previously collapsed
			{//then collapse
				_isTreeDockerCollapsed=true;//auto collapse
				_widthTree96=_widthTree96StartDrag;//remember the width before the drag
				LayoutControls();
				panelSplitter.Invalidate();
			}
			//Save to db.  This happens for every single mouse up.
			//It does not save collapsed/expanded state, but only the expanded width.
			//Saves for this user only.
			UserOdPref userOdPref=UserOdPrefs.GetFirstOrDefault(x=>x.FkeyType==UserOdFkeyType.ImageSelectorWidth && x.UserNum==Security.CurUser.UserNum);
			if(userOdPref is null){
				userOdPref=new UserOdPref();
				userOdPref.UserNum=Security.CurUser.UserNum;
				userOdPref.FkeyType=UserOdFkeyType.ImageSelectorWidth;
			}
			userOdPref.ValueString=_widthTree96.ToString();
			UserOdPrefs.Upsert(userOdPref);
			DataValid.SetInvalid(InvalidType.UserOdPrefs);
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
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.printDocument_PrintPage(sender,e);
		}

		private void sigBox_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click(this,new EventArgs());
		}

		private void sigBoxTopaz_DoubleClick(object sender,EventArgs e) {
			ToolBarSign_Click(this,new EventArgs());
		}

		private void textNote_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click(this,new EventArgs());
		}

		private void unmountedBar_Close(object sender, EventArgs e){
			_isAcquiring=false;
			SetUnmountedBarVisibility();
			LayoutControls();
		}

		private void unmountedBar_RefreshParent(object sender, EventArgs e){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			SelectTreeNode1(nodeTypeAndKey);
		}

		private void unmountedBar_Remount(object sender, WpfControls.UI.UnmountedObj e){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			int idx=controlImageDisplay.GetIdxSelectedInMount();
			if(idx==-1){
				List<MountItem> listAvail=GetAvailSlots(1);
				if(listAvail is null){//no more available slots
					MsgBox.Show(this,"Please select an item in the mount first.");
					return;
				}
				else{
					idx=controlImageDisplay.GetListMountItems().IndexOf(listAvail[0]);
					SetIdxSelectedInMount(idx);
				}
			}
			Document document=controlImageDisplay.GetDocumentShowing(idx);
			if(document!=null){
				//unmount the old document
				MountItem mountItemCopy=controlImageDisplay.GetListMountItems()[idx].Copy();
				mountItemCopy.ItemOrder=-1;
				MountItems.Insert(mountItemCopy);//it will now have a new PK
				document.MountItemNum=mountItemCopy.MountItemNum;
				Documents.Update(document);
			}
			e.Document_.MountItemNum=controlImageDisplay.GetListMountItems()[idx].MountItemNum;
			Documents.Update(e.Document_);
			MountItems.Delete(e.MountItem_);
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			SelectTreeNode1(nodeTypeAndKey);
		}

		private void unmountedBar_Retake(object sender, EventArgs e){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;//should never happen
			}
			int idx=controlImageDisplay.GetIdxSelectedInMount();
			if(idx==-1){
				MsgBox.Show(this,"This is intended to be used in the middle of a series of image acquisitions.");
				return;
			}
			List<MountItem> listMountItems=controlImageDisplay.GetListMountItems();
			MountItem mountItem=listMountItems[idx];
			if(idx==0 && mountItem.ItemOrder==0){
				MsgBox.Show(this,"There is no previous image to retake.");
				return;
			}
			Document document=controlImageDisplay.GetDocumentShowing(idx);
			if(document==null){
				//this is normal. Retake the previous
				if(idx>0) {
					idx--;
				}
				//Test for the previous item being a text(mountItem.ItemOrder=0) or unmounted(mountItem.ItemOrder=-1)
				//This has nothing to do with idx
				mountItem=listMountItems[idx];
				if(mountItem.ItemOrder==0//text
					|| mountItem.ItemOrder==-1)//unmounted
				{
					MsgBox.Show(this,"There is no previous image to retake.");
					return;
				}
				controlImageDisplay.SetIdxSelectedInMount(idx);
				document=controlImageDisplay.GetDocumentShowing(idx);
				if(document==null) {
					MsgBox.Show(this,"There is no previous image to retake.");
					return;
				}
			}
			else{
				//this means they clicked on the one that they want to retake
			}
			MountItem mountItemUnmounted=controlImageDisplay.GetListMountItems()[idx].Copy();
			mountItemUnmounted.ItemOrder=-1;
			MountItems.Insert(mountItemUnmounted);//it will now have a new PK
			document.MountItemNum=mountItemUnmounted.MountItemNum;
			Documents.Update(document);
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			SelectTreeNode1(nodeTypeAndKey);
			//There is now a new mountItem in the unmounted list, so we have to add 1 to the idx just to keep it in the same spot as it already is.
			controlImageDisplay.SetIdxSelectedInMount(idx+1);
			idx=controlImageDisplay.GetIdxSelectedInMount();
			butStart_Click(this,new EventArgs());
		}

		///<summary>Occurs when the slider moves.  UI is typically updated here.  Also see ScrollComplete.</summary>
		private void windowingSlider_Scroll(object sender,EventArgs e) {
			if(IsDocumentShowing()) {
				GetDocumentShowing(0).WindowingMin=windowingSlider.MinVal;
				GetDocumentShowing(0).WindowingMax=windowingSlider.MaxVal;
				InvalidateSettingsColor();
			}
			if(IsMountItemSelected()) {
				int idx=GetIdxSelectedInMount();
				Document document=GetDocumentShowing(idx);
				document.WindowingMin=windowingSlider.MinVal;
				document.WindowingMax=windowingSlider.MaxVal;
				InvalidateSettingsColor();
			}
		}

		///<summary>Occurs when user releases slider after moving.  Database is typically updated here.  Also see Scroll event.</summary>
		private void windowingSlider_ScrollComplete(object sender,EventArgs e) {
			if(IsDocumentShowing()) {
				Documents.Update(GetDocumentShowing(0));
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(0),_patFolder);
				InvalidateSettingsColor();
				ThumbnailRefresh();
			}
			if(IsMountItemSelected()) {
				Documents.Update(GetDocumentShowing(GetIdxSelectedInMount()));
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(GetIdxSelectedInMount()),_patFolder);
				InvalidateSettingsColor();
				//We will not update the mount thumbnail for such a minor change
			}
		}

		private void zoomSlider_EventResetTranslation(object sender, EventArgs e){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.ResetTranslation();
		}

		private void zoomSlider_EventZoomed(object sender, EventArgs e){
			//fires repeatedly while dragging
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.ZoomSliderValue=zoomSlider.Value;
		}
		#endregion Methods - Event Handlers

		#region Methods - Event Handlers Toolbars
		private void ToolBarForms_Click(object sender, EventArgs e){
			MsgBox.Show(this,"Use the dropdown list.  Add forms to the list by copying image files into your A-Z folder, Forms.  Restart the program to see newly added forms.");
		}

		private void toolBarPaint_ButtonClick(object sender, ODToolBarButtonClickEventArgs e){
			if(e.Button.Tag.GetType()!=typeof(TB)) {
				return;
			}
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			switch((TB)e.Button.Tag) {
				case TB.ZoomOne:
					controlImageDisplay.ToolBarZoomOne_Click();
					break;
				case TB.Crop:
					SetDrawMode(EnumDrawMode.None);
					SetCropPanAdj(EnumCropPanAdj.Crop);
					break;
				case TB.Pan:
					SetDrawMode(EnumDrawMode.None);
					SetCropPanAdj(EnumCropPanAdj.Pan);
					break;
				case TB.Adj:
					SetDrawMode(EnumDrawMode.None);
					SetCropPanAdj(EnumCropPanAdj.Adj);
					break;
				case TB.Size:
					controlImageDisplay.ToolBarSize_Click();
					break;
				case TB.Flip:
					controlImageDisplay.ToolBarFlip_Click();
					break;
				case TB.RotateL:
					controlImageDisplay.ToolBarRotateL_Click();
					break;
				case TB.RotateR:
					controlImageDisplay.ToolBarRotateR_Click();
					break;
				case TB.Rotate180:
					controlImageDisplay.ToolBarRotate180_Click();
					break;
				case TB.DrawTool:
					ToolBarDraw_Click();
					break;
				case TB.Unmount:
					ToolBarUnmount_Click();
					break;
			}
		}

		private void ToolBarCopy_Click(object sender, EventArgs e){
			GetOrMakeControlImageDisplay().ToolBarCopy_Click(sender,e);
		}

		private void ToolBarDelete_Click(object sender, EventArgs e){
			GetOrMakeControlImageDisplay().ToolBarDelete_Click(sender,e);
		}

		private void toolBarDraw_ButtonClick(object sender, ODToolBarButtonClickEventArgs e){
			if(e.Button.Tag.GetType()!=typeof(TB)) {
				return;
			}
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			switch((TB)e.Button.Tag) {
				case TB.Color:
					ToolBarColor_Click();
					break;
				case TB.Text:
					SetDrawMode(EnumDrawMode.Text);
					break;
				case TB.Line:
					SetDrawMode(EnumDrawMode.Line);					
					break;
				case TB.Pen:
					SetDrawMode(EnumDrawMode.Pen);	
					break;
				case TB.Polygon:
					SetDrawMode(EnumDrawMode.Polygon);	
					break;
				case TB.Eraser:
					SetDrawMode(EnumDrawMode.Eraser);	
					break;
				case TB.ChangeColor:
					SetDrawMode(EnumDrawMode.ChangeColor);	
					break;
				case TB.EditPoints:
					SetDrawMode(EnumDrawMode.LineEditPoints);
					break;
				case TB.Measure:
					SetDrawMode(EnumDrawMode.LineMeasure);
					break;
				case TB.SetScale:
					SetDrawMode(EnumDrawMode.LineSetScale);
					break;
				case TB.Filter:
					ShowFilterWindow();
					break;
				case TB.Close:
					SetDrawMode(EnumDrawMode.None);
					panelDraw.Visible=false;
					LayoutControls();
					break;
			}
		}

		private void ToolBarExport_Click(object sender, EventArgs e){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			controlImageDisplay.ToolBarExport_Click();
		}

		private void ToolBarExportTIFF(object sender, EventArgs e){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			controlImageDisplay.ToolBarExport_Click(doExportAsTiff:true);
		}

		private void ToolBarImport_Click(object sender, EventArgs e){
			long defNumCategory=imageSelector.GetSelectedCategory();
			EnumImageNodeType enumImageNodeType=imageSelector.GetSelectedType();
			ControlImageDisplay controlImageDisplay=GetOrMakeControlImageDisplay();
			//The above line clears out the category information, so we might need to reset that.
			if(enumImageNodeType==EnumImageNodeType.Category) {
				NodeTypeAndKey nodeTypeAndKeyCategory=new NodeTypeAndKey(enumImageNodeType,defNumCategory);
				controlImageDisplay.ClearObjects();
				controlImageDisplay.SelectTreeNode2(nodeTypeAndKeyCategory);
			}
			controlImageDisplay.ToolBarImport_Click(sender,e);
			//handle the situation where they click cancel and no previous image was selected
			if(!controlImageDock.IsImageFloatSelected){
				return;
			}
			NodeTypeAndKey nodeTypeAndKey=controlImageDock.ControlImageDisplay_.GetNodeTypeAndKey();
			if(nodeTypeAndKey is null){
				return;//not sure how this would happen
			}
			if(nodeTypeAndKey.NodeType==EnumImageNodeType.None){
				SelectTreeNode1(null);//this makes the ControlImageDisplay go away
			}
		}
		
		private void ToolBarInfo_Click(object sender, EventArgs e){
			GetOrMakeControlImageDisplay().ToolBarInfo_Click(sender,e);
		}

		private void ToolBarMoveToPatient(object sender, EventArgs e){
			if(ODBuild.IsThinfinity()) {
				MsgBox.Show(this,"Not yet implemented for Open Dental Cloud.");
				return;
			}
			if(!IsDocumentShowing() && !IsMountShowing()){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(IsMountShowing()) {
				if(!Security.IsAuthorized(EnumPermType.ImageDelete,GetMountShowing().DateCreated)) {
					return;
				}
			}
			else {
				if(!Security.IsAuthorized(EnumPermType.ImageDelete,GetDocumentShowing(0).DateCreated)) {
					return;
				}
			}
			FrmPatientSelect frmPatientSelect=new FrmPatientSelect();
			frmPatientSelect.ShowDialog();
			if(frmPatientSelect.IsDialogCancel){
				return;
			}
			string patFolderOld=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			Document document=null;
			Mount mount=null;
			NodeTypeAndKey nodeTypeAndKeyOriginal=null;
			if(IsDocumentShowing()){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move selected item to the other patient?")){
					return;
				}
				document=GetDocumentShowing(0).Copy();
				nodeTypeAndKeyOriginal=new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum);
			}
			if(IsMountShowing()){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Move the entire mount to the other patient?")){
					return;
				}
				mount=GetMountShowing().Copy();
				nodeTypeAndKeyOriginal=new NodeTypeAndKey(EnumImageNodeType.Mount,mount.MountNum);
			}
			if(GetIdxSelectedInMount()>-1){
				SetIdxSelectedInMount(-1);
			}
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			GlobalFormOpenDental.GoToModule(EnumModuleType.Imaging,patNum:frmPatientSelect.PatNumSelected);
			//The line above will close all undocked floaters and will clear any docked floater.
			//When we select a document/mount, then we could be dealing with a different floater than before.
			NodeTypeAndKey nodeTypeAndKeyCat;
			if(document!=null){
				nodeTypeAndKeyCat=new NodeTypeAndKey(EnumImageNodeType.Category,document.DocCategory);
				SelectTreeNode1(nodeTypeAndKeyCat);
			}
			if(mount!=null){
				nodeTypeAndKeyCat=new NodeTypeAndKey(EnumImageNodeType.Category,mount.DocCategory);
				SelectTreeNode1(nodeTypeAndKeyCat);
			}
			//Get the new docked image for this patient
			controlImageDisplay=GetOrMakeControlImageDisplay();
			controlImageDisplay.ToolBarPasteTypeAndKey(nodeTypeAndKeyOriginal,showDocInfo:false);
			imageSelector.SetSelected(nodeTypeAndKeyOriginal.NodeType,nodeTypeAndKeyOriginal.PriKey);
			//Delete Old=====================================================================================
			if(document!=null){
				try {
					controlImageDisplay.ClearPDFBrowser();//Dispose of the web browser control that has a hold on the PDF that is showing.
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
				Def defDocCategory = Defs.GetDef(DefCat.ImageCats,mount.DocCategory);
				string logText = "Mount Deleted: "+mount.Description+" with category "
					+defDocCategory.ItemName;
				SecurityLogs.MakeLogEntry(EnumPermType.ImageDelete,_patient.PatNum,logText);
			}
		}

		private void ToolBarPaste_Click(object sender, EventArgs e){
			long defNumCategory=imageSelector.GetSelectedCategory();
			EnumImageNodeType enumImageNodeType=imageSelector.GetSelectedType();
			ControlImageDisplay controlImageDisplay=GetOrMakeControlImageDisplay();
			//The above line clears out the category information, so we might need to reset that.
			if(enumImageNodeType==EnumImageNodeType.Category) {
				NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey(enumImageNodeType,defNumCategory);
				controlImageDisplay.ClearObjects();
				controlImageDisplay.SelectTreeNode2(nodeTypeAndKey);
			}
			controlImageDisplay.ToolBarPaste_Click(sender,e);
		}
		
		private void ToolBarPrint_Click(object sender,EventArgs e){
			if(!IsDocumentShowing() && !IsMountShowing()){
				MsgBox.Show(this,"Cannot print. No document currently selected.");
				return;
			}
			if(IsDocumentShowing()){
				if(Path.GetExtension(GetDocumentShowing(0).FileName).ToLower()==".pdf") {//Selected document is PDF, we handle differently than documents that aren't pdf.
					ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();//Will always work because we used IsDocumentShowing
					controlImageDisplay.PdfPrintPreview();
					return;
				}
			}
			PrinterL.TryPrintOrDebugClassicPreview(printDocument_PrintPage,Lan.g(this,"Image printed."));
			if(IsDocumentShowing()){
				SecurityLogs.MakeLogEntry(EnumPermType.Printing,_patient.PatNum,"Patient image "+GetDocumentShowing(0).FileName+" "+GetDocumentShowing(0).Description+" printed");
			}
			if(IsMountShowing()){
				SecurityLogs.MakeLogEntry(EnumPermType.Printing,_patient.PatNum,"Patient mount "+GetMountShowing().Description+" printed");
			}
		}

		private void ToolBarProgram_Click(object sender,EventArgs e){
			Program program=(Program)(((WpfControls.UI.ToolBarButton)sender).Tag);
			if(program.ProgName==ProgramName.Pearl.ToString()){
				//The Pearl button can only work from the Imaging module, so we handle it here.
				//these first few ifs are copied from ProgramL, because we are skipping that and doing our own here.
				//if(patient!=null && PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				//	patient=Patients.GetOriginalPatientForClone(patient);//not sure if this is needed for Imaging. Doesn't seem right.
				//}
				if(!Programs.IsEnabledByHq(program,out string err)) {
					MessageBox.Show(err);
					return;
				}
				if(ODBuild.IsThinfinity() && Programs.GetListDisabledForWeb().Select(x => x.ToString()).Contains(program.ProgName)) {
					MsgBox.Show("ProgramLinks","Bridge is not available while viewing through the web.");
					return;//bridge is not available for web users at this time. 
				}
				if(_showDrawingsPearlToothParts==false
					&& _showDrawingsPearlPolyAnnotations==false
					&& _showDrawingsPearlBoxAnnotations==false
					&& _showDrawingsPearlMeasurements==false)
				{//if no Pearl annotations are set to show right now
					//turn them all on so that user can see result
					_showDrawingsPearlToothParts=true;
					_showDrawingsPearlPolyAnnotations=true;
					_showDrawingsPearlBoxAnnotations=true;
					_showDrawingsPearlMeasurements=true;
				}
				OpenDentBusiness.Bridges.Pearl pearl=new OpenDentBusiness.Bridges.Pearl();
				//some items in these two lists can be null if mount has empty spots in it. The null will exist in the same place in both lists.
				ControlImageDisplay controlImageDisplay=GetOrMakeControlImageDisplay();
				//pearl.Patient_=_patient;
				//pearl.Computer_=ComputerPrefs.LocalComputer.ComputerName;
				pearl.ListBitmaps=controlImageDisplay.GetListBitmaps();
				//List<Document> listDocuments=new List<Document>();
				//for(int i=0;i<pearl.ListBitmaps.Count;i++) {
				//	listDocuments.Add(GetDocumentShowing(i));
				//}
				//pearl.ListDocuments=listDocuments;
				if(IsMountShowing()){
					pearl.DocNum=0;
					pearl.ListMountItems=controlImageDisplay.GetListMountItems();
					pearl.MountNum=GetMountShowing().MountNum;
				}
				else{//one doc
					pearl.DocNum=GetDocumentShowing(0).DocNum;
					pearl.MountNum=0;
				}
				UI.ProgressWin progressOD=new UI.ProgressWin();
				progressOD.ActionMain=pearl.SendOnThread;//not sure how to pass in bitmap as a parameter
				//todo: some math if we want to give user a better idea of how long to wait.
				progressOD.StartingMessage="Communicating with Pearl server. This will take a few moments.";
				progressOD.ShowDialog();
				EnumImageNodeType enumImageNodeType=imageSelector.GetSelectedType();
				long priKey=imageSelector.GetSelectedKey();
				NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey(enumImageNodeType,priKey);
				SelectTreeNode1(nodeTypeAndKey);
			}
			else{
				WpfControls.ProgramL.Execute(program.ProgramNum,_patient);
			}
		}

		///<summary>Valid values for scanType are "doc","xray",and "photo"</summary>
		private void ToolBarScan_Click(ImageType imgType){
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}	
			if(ODEnvironment.IsCloudServer) {
				if(CloudClientL.IsCloudClientRunning()) {
					ToolbarScanWeb(imgType);
				}
				return;
			}
			Cursor=Cursors.WaitCursor;
			Bitmap bitmapScanned=null;
			IntPtr handleDIB=IntPtr.Zero;
			try {
				Twain.ActivateEZTwain();
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
			bool saved=true;
			Document doc = null;
			try {//Create corresponding image file.
				bool doPrintHeading=false;
				if(imgType==ImageType.Radiograph) {
					doPrintHeading=true;
				}
				doc=ImageStore.Import(bitmapScanned,GetCurrentCategory(),imgType,_patient,doPrintHeading:doPrintHeading);
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
				FillImageSelector(false);//Reload and keep new document selected.
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
				FrmDocInfo frmDocInfo=new FrmDocInfo(_patient,GetDocumentShowing(0),isDocCreate:true);
				frmDocInfo.ShowDialog();
				if(frmDocInfo.IsDialogCancel) {
					DeleteDocument(false,false,doc);
				}
				else {
					FillImageSelector(true);//Update tree, in case the new document's icon or category were modified in formDocInfo.
				}
			}
		}

		private void ToolBarScanDoc_Click(object sender,EventArgs e){
			ToolBarScan_Click(ImageType.Document);
		}

		private void ToolBarScanMulti_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}
			if(ODEnvironment.IsCloudServer) {
				if(CloudClientL.IsCloudClientRunning()) {
					ToolbarScanMultiWeb();
				}
				return;
			}
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			try {
				Twain.ActivateEZTwain();
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
			NodeTypeAndKey nodeTypeAndKey=null;
			bool copied=true;
			Document doc=null;
			try {
				doc=ImageStore.Import(tempFile,GetCurrentCategory(),_patient);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ") + ex.Message + ": " + tempFile);
				copied = false;
			}
			if(copied) {
				FillImageSelector(false);
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
				FrmDocInfo frmDocInfo=new FrmDocInfo(_patient,doc,isDocCreate:true);
				frmDocInfo.ShowDialog();//some of the fields might get changed, but not the filename 
				if(frmDocInfo.IsDialogCancel) {
					DeleteDocument(false,false,doc);
				}
				else {
					nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum);
					SetDocumentShowing(0,doc.Copy());
				}
			}
			ImageStore.TryDeleteFile(tempFile
				,actInUseException:(msg) => MsgBox.Show(msg)//Informs user when a 'file is in use' exception occurs.
			);
			//Reselect the last successfully added node when necessary. js This code seems to be copied from import multi.  Simplify it.
			if(doc!=null && !new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum).Equals(nodeTypeAndKey)) {
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
			}
			FillImageSelector(true);
		}

		///<summary>Handles the scan multi click for ODCloud using similar logic to ToolbarScanMulti_Click()</summary>
		private void ToolbarScanMultiWeb() {
			if(!CloudClientL.IsCloudClientRunning()) {
				return;
			}
			//Ask the ODCloudClient to use a scanner on the client's computer
			string tempFile=ODCloudClient.GetImageMultiFromScanner(
				ComputerPrefs.LocalComputer.ScanDocSelectSource,
				ComputerPrefs.LocalComputer.ScanDocShowOptions,
				ComputerPrefs.LocalComputer.ScanDocDuplex,
				ComputerPrefs.LocalComputer.ScanDocGrayscale,
				ComputerPrefs.LocalComputer.ScanDocResolution,
				ComputerPrefs.LocalComputer.ScanDocQuality
			);
			if(tempFile==null) {
				return;//The scan was probably cancelled
			}
			NodeTypeAndKey nodeTypeAndKey=null;
			bool copied=true;
			Document doc=null;
			try {
				doc=ImageStore.Import(tempFile,GetCurrentCategory(),_patient);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ") + ex.Message + ": " + tempFile);
				copied = false;
			}
			if(copied) {
				FillImageSelector(false);
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
				FrmDocInfo frmDocInfo=new FrmDocInfo(_patient,doc,isDocCreate:true);
				frmDocInfo.ShowDialog();//some of the fields might get changed, but not the filename 
				if(frmDocInfo.IsDialogCancel) {
					DeleteDocument(false,false,doc);
				}
				else {
					nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum);
					SetDocumentShowing(0,doc.Copy());
				}
			}
			ImageStore.TryDeleteFile(tempFile
				,actInUseException:(msg) => MsgBox.Show(msg)//Informs user when a 'file is in use' exception occurs.
			);
			//Reselect the last successfully added node when necessary. js This code seems to be copied from import multi.  Simplify it.
			if(doc!=null && !new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum).Equals(nodeTypeAndKey)) {
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
			}
			FillImageSelector(true);
		}

		private void ToolBarScanPhoto_Click(object sender,EventArgs e){
			ToolBarScan_Click(ImageType.Photo);
		}

		///<summary>Handles the scan click for ODCloud using similar logic to ToolbarScan_Click()</summary>
		private void ToolbarScanWeb(ImageType imgType) {
			if(!CloudClientL.IsCloudClientRunning()) {
				return;
			}
			Bitmap bitmapScanned=null;
			try {
				//Ask the ODCloudClient to use a scanner on the client's computer
				bitmapScanned=ODCloudClient.GetImageFromScanner(
					ComputerPrefs.LocalComputer.ScanDocSelectSource,
					ComputerPrefs.LocalComputer.ScanDocShowOptions,
					ComputerPrefs.LocalComputer.ScanDocDuplex,
					ComputerPrefs.LocalComputer.ScanDocGrayscale,
					ComputerPrefs.LocalComputer.ScanDocResolution,
					ComputerPrefs.LocalComputer.ScanDocQuality
				);
			}
			catch (ODException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			if(bitmapScanned==null) {//The scan was probably cancelled.
				return;
			}
			bool saved=true;
			Document doc = null;
			try {//Create corresponding image file.
				bool doPrintHeading=false;
				if(imgType==ImageType.Radiograph) {
					doPrintHeading=true;
				}
				doc=ImageStore.Import(bitmapScanned,GetCurrentCategory(),imgType,_patient,doPrintHeading:doPrintHeading);
			}
			catch(Exception ex) {
				saved=false;
				MessageBox.Show(Lan.g(this,"Unable to save document")+": "+ex.Message);
			}
			if(bitmapScanned!=null) {
				bitmapScanned.Dispose();
				bitmapScanned=null;
			}//===========================
			if(saved) {
				FillImageSelector(false);//Reload and keep new document selected.
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
				FrmDocInfo frmDocInfo=new FrmDocInfo(_patient,GetDocumentShowing(0),isDocCreate:true);
				frmDocInfo.ShowDialog();
				if(frmDocInfo.IsDialogCancel) {
					DeleteDocument(false,false,doc);
				}
				else {
					FillImageSelector(true);//Update tree, in case the new document's icon or category were modified in formDocInfo.
				}
			}
		}

		private void ToolBarScanXRay_Click(object sender,EventArgs e){
			ToolBarScan_Click(ImageType.Radiograph);
		}

		private void ToolBarSign_Click(object sender,EventArgs e){
			if(_listFormImageFloats.Count==0 && controlImageDock.ControlImageDisplay_==null){
				MsgBox.Show(this,"No image selected.");//just in case
				return;
			}
			if(!controlImageDock.IsImageFloatSelected){
				MsgBox.Show(this,"Signature and note only works if the docked image is selected first.");
				return;
			}
			if(imageSelector.GetSelectedType()!=EnumImageNodeType.Document){
				return;
			}
			//Show the underlying panel note box while the signature is being filled.
			panelNote.Visible=true;
			LayoutControls();
			//Display the document signature form.
			FrmDocSign frmDocSign=new FrmDocSign(GetDocumentShowing(0),_patient);//Updates our local document and saves changes to db also.
			Point pointLocal=new Point(panelSplitter.Left,Height);
			Point pointScreen=PointToScreen(pointLocal);
			frmDocSign.PointLLStart=pointScreen;
			frmDocSign.ShowDialog();
			FillImageSelector(true);
			//Adjust visibility of panel note based on changes made to the signature above.
			SetPanelNoteVisibility();
			//Resize controls in our window to adjust for a possible change in the visibility of the panel note control.
			LayoutControls();
			//Update the signature and note with the new data.
			FillSignature();
		}

		public void ToolBarUnmount_Click(){
			if(!IsMountShowing()){
				//should never happen
				MsgBox.Show(this,"Only for mounts.");
				return;
			}
			if(!IsMountItemSelected()){
				MsgBox.Show(this,"Please select an image in the mount first.");
				return;
			}
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;//shouldn't happen
			}
			int idx=controlImageDisplay.GetIdxSelectedInMount();
			MountItem mountItemCopy=controlImageDisplay.GetListMountItems()[idx].Copy();
			mountItemCopy.ItemOrder=-1;
			MountItems.Insert(mountItemCopy);//it will now have a new PK
			Document document=controlImageDisplay.GetDocumentShowing(idx);
			document.MountItemNum=mountItemCopy.MountItemNum;
			Documents.Update(document);
			NodeTypeAndKey nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
			SelectTreeNode1(nodeTypeAndKey);
		}

		private void ToolBarVideo_Click(object sender,EventArgs e){
			if(ODBuild.IsThinfinity()) {
				MsgBox.Show(this,"This feature is not available in Open Dental Cloud.");
				return;
			}
			//If no patient selected, then this button is disabled
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}
			if(!ODBuild.IsTrial()
				&& !OpenDentalHelp.ODHelp.IsEncryptedKeyValid())//always true in debug
			{
				MsgBox.Show(this,"This feature requires an active support plan.");
				return;
			}
			if(_formLauncherVideo is null){
				_formLauncherVideo=new FormLauncher(EnumFormName.FormVideo);
				_formLauncherVideo.SetEvent("BitmapCaptured",formVideo_BitmapCaptured);
			}
			if(_formLauncherVideo.IsNullDisposedOrNotVis()){
				PreselectFirstItem();
				//still might not be one selected, so test each time
				_formLauncherVideo.Show();
				LayoutControls();
				return;
			}
			_formLauncherVideo.RestoreAndFront();
		}
		#endregion Methods - Event Handlers Toolbars

		#region Methods - Private
		public static EnumImgDeviceControlType ConvertToImgDeviceControlType(EnumImgDeviceType imgDeviceType){
			switch(imgDeviceType){
				default:
				case EnumImgDeviceType.TwainRadiograph:
					return EnumImgDeviceControlType.Twain;
				case EnumImgDeviceType.XDR:
					return EnumImgDeviceControlType.XDR;
				case EnumImgDeviceType.TwainMulti:
					return EnumImgDeviceControlType.TwainMulti;
			}
		}

		///<summary>This creates a ControlImageDisplay and attaches the many events. It does not attach it to a parent floater or dock. It does not fill it with any image.</summary>
		private ControlImageDisplay CreateControlImageDisplay(){
			ControlImageDisplay controlImageDisplay=new ControlImageDisplay();
			controlImageDisplay.ImageSelector_=imageSelector;
			controlImageDisplay.EventEnableToolBarButtons+=(sender,toolBarButtonState)=>EnableToolBarButtons(toolBarButtonState);
			controlImageDisplay.EventFillTree+=(sender,keepSelection)=>FillImageSelector(keepSelection);
			controlImageDisplay.EventSelectTreeNode+=(sender,nodeTypeAndKey)=>{
				if(nodeTypeAndKey is null){
					nodeTypeAndKey=controlImageDisplay.GetNodeTypeAndKey();
				}
				SelectTreeNode1(nodeTypeAndKey);
			};
			controlImageDisplay.EventSetCropPanEditAdj+=(sender,cropPanAdj)=>SetCropPanAdj(cropPanAdj);
			controlImageDisplay.EventSetDrawMode+=(sender,drawMode)=>SetDrawMode(drawMode);
			controlImageDisplay.EventSetWindowingSlider+=FormImageFloat_SetWindowingSlider;
			controlImageDisplay.EventThumbnailNeedsRefresh+=(sender,e)=>ThumbnailRefresh();
			//this one now goes through ControlImageDock:
			//controlImageDisplay.EventLaunchFloater+=(sender,e)=>LaunchFloater((ControlImageDisplay)sender,300,300,500,500);
			//events moved to constructor because they only need to be called once
			controlImageDisplay.EventResetZoomSlider+=(sender,zoomSliderState)=>{
				zoomSlider.SetValueInitialFit(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
				((ControlImageDisplay)sender).ZoomSliderValue=zoomSlider.Value;
			};
			controlImageDisplay.EventZoomSliderSetByWheel+=(sender,deltaZoom)=>{
				zoomSlider.SetByWheel(deltaZoom);
				((ControlImageDisplay)sender).ZoomSliderValue=zoomSlider.Value;
			};
			controlImageDisplay.EventZoomSliderSetValueAndMax+=(sender,newVal)=>zoomSlider.SetValueAndMax(newVal);
			//controlImageDisplay.IsImageFloatDockedChanged+=FormImageFloat_IsImageFloatDockedChanged;
			return controlImageDisplay;
		}

		///<summary>Deletes the specified document from the database and refreshes the tree view. Set securityCheck false when creating a new document that might get cancelled.  Document is passed in because it might not be in the tree if the image folder it belongs to is now hidden.</summary>
		private void DeleteDocument(bool isVerbose,bool doSecurityCheck,Document document) {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.DeleteDocument(isVerbose,doSecurityCheck,document);
		}

		///<summary>Enables or disables all the buttons in both toolbars to handle situation where no patient is selected.</summary>
		private void EnableToolBarsPatient(bool enable) {
			toolBarMain.SetEnabledAll(enable);
			//Acquire button?
			for(int i=0;i<toolBarPaint.Buttons.Count;i++) {
				toolBarPaint.Buttons[i].Enabled=enable;
			}
			toolBarPaint.Invalidate();
			windowingSlider.IsEnabled=enable;
			zoomSlider.IsEnabled=enable;
			windowingSlider.Draw();
		}

		///<summary>Not technically all.  There are some buttons that we never disable such as import, scan, acquire, etc.</summary>
		private void DisableAllToolBarButtons() {
			bool enable=false;
			ToolBarButtonState toolBarButtonState=new ToolBarButtonState(print:enable, delete:enable, info:enable, sign:enable, export:enable, copy:enable, brightAndContrast:enable, zoom:enable, zoomOne:enable, crop:enable, pan:enable, adj:enable, size:enable, flip:enable, rotateL:enable, rotateR:enable, rotate180:enable,draw:enable, unmount:enable);
			EnableToolBarButtons(toolBarButtonState);
		}

		///<summary>Defined this way to force future programming to consider which tools are enabled and disabled for every possible tool in the menu.  To prevent bugs, you must always use named arguments.  Called when user clicks on Crop/Pan/Mount buttons, clicks Tree, or clicks around on a mount.</summary>
		private void EnableToolBarButtons(ToolBarButtonState toolBarButtonState){
			//bool print, bool delete, bool info, bool sign, bool export, bool copy, bool brightAndContrast, bool zoom, bool zoomOne, bool crop, bool pan, bool adj, bool size, bool flip, bool rotateL,bool rotateR, bool rotate180) {
			//Some buttons don't show here because they are always enabled as long as there is a patient,
			//including Scan, Import, Paste, Templates, Mounts
			toolBarMain.SetEnabled(TB.Print.ToString(),toolBarButtonState.Print);
			toolBarMain.SetEnabled(TB.Delete.ToString(),toolBarButtonState.Delete);
			toolBarMain.SetEnabled(TB.Info.ToString(),toolBarButtonState.Info);
			toolBarMain.SetEnabled(TB.Sign.ToString(),toolBarButtonState.Sign);
			toolBarMain.SetEnabled(TB.Export.ToString(),toolBarButtonState.Export);
			toolBarMain.SetEnabled(TB.Copy.ToString(),toolBarButtonState.Copy);
			windowingSlider.IsEnabled=toolBarButtonState.BrightAndContrast;
			windowingSlider.Draw();
			zoomSlider.IsEnabled=toolBarButtonState.Zoom;
			if(toolBarPaint.Buttons.Count==0) {
				return;//must be launching a floater from ControlChart prior to initializing this module.
			}
			toolBarPaint.Buttons[TB.ZoomOne.ToString()].Enabled=toolBarButtonState.ZoomOne;
			toolBarPaint.Buttons[TB.Crop.ToString()].Enabled=toolBarButtonState.Crop;
			toolBarPaint.Buttons[TB.Pan.ToString()].Enabled=toolBarButtonState.Pan;
			toolBarPaint.Buttons[TB.Adj.ToString()].Enabled=toolBarButtonState.Adj;
			toolBarPaint.Buttons[TB.Size.ToString()].Enabled=toolBarButtonState.Size;
			toolBarPaint.Buttons[TB.Flip.ToString()].Enabled=toolBarButtonState.Flip;
			toolBarPaint.Buttons[TB.RotateR.ToString()].Enabled=toolBarButtonState.RotateR;
			toolBarPaint.Buttons[TB.RotateL.ToString()].Enabled=toolBarButtonState.RotateL;
			toolBarPaint.Buttons[TB.Rotate180.ToString()].Enabled=toolBarButtonState.Rotate180;
			toolBarPaint.Buttons[TB.DrawTool.ToString()].Enabled=toolBarButtonState.Draw;
			toolBarPaint.Buttons[TB.Unmount.ToString()].Enabled=toolBarButtonState.Unmount;
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
			textNote.Text=GetDocumentShowing(0).Note;
			labelInvalidSig.Visible=false;
			sigBox.Visible=true;
			sigBox.SetTabletState(0);//never accepts input here
			//Topaz box is not supported in Unix, since the required dll is Windows native.
			if(GetDocumentShowing(0).SigIsTopaz) {
				if(GetDocumentShowing(0).Signature!=null && GetDocumentShowing(0).Signature!="") {
					//if(allowTopaz) {	
					sigBox.Visible=false;
					_sigBoxTopaz.Visible=true;
					TopazWrapper.ClearTopaz(_sigBoxTopaz);
					TopazWrapper.SetTopazCompressionMode(_sigBoxTopaz,0);
					TopazWrapper.SetTopazEncryptionMode(_sigBoxTopaz,0);
					TopazWrapper.SetTopazKeyString(_sigBoxTopaz,"0000000000000000");//Clear out the key string
					string keystring=GetHashString(GetDocumentShowing(0));
					TopazWrapper.SetTopazAutoKeyData(_sigBoxTopaz,keystring);
					TopazWrapper.SetTopazEncryptionMode(_sigBoxTopaz,2);//high encryption
					TopazWrapper.SetTopazCompressionMode(_sigBoxTopaz,2);//high compression
					TopazWrapper.SetTopazSigString(_sigBoxTopaz,GetDocumentShowing(0).Signature);
					_sigBoxTopaz.Refresh();
					//If sig is not showing, then setting the Key String to the hashed data. This is the way we used to handle signatures.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						TopazWrapper.SetTopazKeyString(_sigBoxTopaz,"0000000000000000");//Clear out the key string
						TopazWrapper.SetTopazKeyString(_sigBoxTopaz,keystring);
						TopazWrapper.SetTopazSigString(_sigBoxTopaz,GetDocumentShowing(0).Signature);
					}
					//If sig is not showing, then try encryption mode 3 for signatures signed with old SigPlusNet.dll.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						TopazWrapper.SetTopazEncryptionMode(_sigBoxTopaz,3);//Unknown mode (told to use via TopazSystems)
						TopazWrapper.SetTopazSigString(_sigBoxTopaz,GetDocumentShowing(0).Signature);
					}
					//If sig not showing, then try the ANSI paradigm.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						TopazWrapper.FillSignatureANSI(_sigBoxTopaz,keystring,GetDocumentShowing(0).Signature,SignatureBoxWrapper.SigMode.Document);
					}
					//Try reading in the signature using different encodings for keyData.
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						TopazWrapper.FillSignatureEncodings(_sigBoxTopaz,keystring,GetDocumentShowing(0).Signature,SignatureBoxWrapper.SigMode.Document);
					}
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						labelInvalidSig.Visible=true;
					}
					//}
				}
			}
			else {//not topaz
				if(GetDocumentShowing(0).Signature!=null && GetDocumentShowing(0).Signature!="") {
					sigBox.Visible=true;
					_sigBoxTopaz.Visible=false;
					sigBox.ClearTablet();
					sigBox.SetKeyString(GetHashString(GetDocumentShowing(0)));
					sigBox.SetSigString(GetDocumentShowing(0).Signature);
					if(sigBox.NumberOfTabletPoints()==0) {
						labelInvalidSig.Visible=true;
					}
					sigBox.SetTabletState(0);//not accepting input.
				}
			}
		}

		///<summary>Returns a list of available slots, starting with the one selected.  It will loop back around at the end to fill remaining slots.  Will return null if not enough slots.  But if you supply countNeed -1, then it will give you a list of all available.</summary>
		private List<MountItem> GetAvailSlots(int countNeed=-1){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return null;
			}
			return controlImageDisplay.GetAvailSlots(countNeed);
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayBitmapsShowing of the selected floater. Can return null.</summary>
		private Bitmap GetBitmapShowing(int idx) {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return null;
			}
			return controlImageDisplay.GetBitmapShowing(idx);
		}

		///<summary>Gets the DefNum category of the current selection. The current selection can be a folder itself, or a document within a folder. If nothing selected, then it returns the DefNum of first in the list.</summary>
		private long GetCurrentCategory() {
			return imageSelector.GetSelectedCategory();
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayDocumentsShowing of the selected floater. Can return null.</summary>
		private Document GetDocumentShowing(int idx) {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return null;
			}
			return controlImageDisplay.GetDocumentShowing(idx);
		}

		private string GetHashString(Document doc) {
			return ImageStore.GetHashString(doc,_patFolder);
		}

		/// <summary>Returns null if no ControlImageDisplay is currently selected.</summary>
		private ControlImageDisplay GetControlImageDisplaySelected(){
			if(controlImageDock.IsImageFloatSelected){
				return controlImageDock.ControlImageDisplay_;
			}
			FormImageFloat formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.IsImageFloatSelected);
			if(formImageFloat is null){
				return null;
			}
			return formImageFloat.ControlImageDisplay_;
		}

		///<summary>This is generally within IsMountItemSelected.  Gets _idxSelectedInMount from selected floater.</summary>
		private int GetIdxSelectedInMount(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return -1;//shouldn't happen
			}
			return controlImageDisplay.GetIdxSelectedInMount();
		}

		///<summary>Pulls _mountShowing from the selected floater. Can return null.</summary>
		private Mount GetMountShowing() {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return null;
			}
			return controlImageDisplay.GetMountShowing();
		}
		
		/// <summary>Gets an existing ControlImageDisplay or makes a new one. Always returns a valid ControlImageDisplay.</summary>
		private ControlImageDisplay GetOrMakeControlImageDisplay(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay!=null){
				return controlImageDisplay;
			}
			//If no ControlImageDisplay is selected, assume that they want to use the docked one.
			controlImageDisplay=controlImageDock.ControlImageDisplay_;
			if(controlImageDisplay!=null){
				return controlImageDisplay;
			}
			SelectTreeNode1(null);//probably useless, but also harmless
			//make one
			//the code below is a few lines taken from SelectTreeNode1.
			//We may need to add a few more if there are minor bugs
			controlImageDisplay=CreateControlImageDisplay();
			controlImageDisplay.PatientCur=_patient;
			controlImageDisplay.PatFolder=_patFolder;
			controlImageDisplay.ZoomSliderValue=zoomSlider.Value;
			controlImageDock.SetControlImageDisplay(controlImageDisplay);
			controlImageDock.IsImageFloatSelected=true;
			//so now we have a controlImageDisplay with nothing in it. That seems like a bad idea.
			controlImageDock.Select();
			ControlImageDock_GotODFocus(controlImageDock,new EventArgs());//This enables toolbar buttons, etc
			controlImageDisplay.EnableToolBarButtons();
			EnableMenuItemTreePrintHelper(controlImageDisplay);
			return controlImageDisplay;
			//tested all 5 refs:
			//Copy is not enabled
			//Delete is not enabled
			//Import works. Added code to handle the situation where they cancel.
			//Info is not enabled
			//Paste: All three different ways now work, and better than in 24.1. Also tested all three into mount.
		}

		///<summary>Invalidates the color image setting and recalculates.  This is not on a separate thread.  Instead, it's just designed to run no more than about every 300ms, which completely avoids any lockup.</summary>
		private void InvalidateSettingsColor() {
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.InvalidateSettingsColor();
		}

		///<summary>Returns true if a valid document is showing.  This is very different from testing the property _documentShowing, which would return true for mounts.</summary>
		private bool IsDocumentShowing(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return false;
			}
			return controlImageDisplay.IsDocumentShowing();
		}

		///<summary>Returns true if a valid mount is showing.</summary>
		private bool IsMountShowing(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return false;
			}
			return controlImageDisplay.IsMountShowing();
		}

		///<summary>Returns true if a valid mountitem is selected and there's a valid bitmap in that location.</summary>
		private bool IsMountItemSelected(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return false;
			}
			return controlImageDisplay.IsMountItemSelected();
		}

		private void labelInvalidSig_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click(this,new EventArgs());
		}

		private void label15_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click(this,new EventArgs());
		}

		private void label1_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click(this,new EventArgs());
		}

		///<summary>Resizes all controls in the image module to fit inside the current window, including controls which have varying visibility.</summary>
		public void LayoutControls(){
			LayoutManager.Move(elementHostToolBarMain,new Rectangle(0,0,Width,LayoutManager.Scale(25)));
			LayoutManager.Move(elementHostWindowingSlider,new Rectangle(4,elementHostToolBarMain.Bottom+2,LayoutManager.Scale(154),LayoutManager.Scale(20)));
			LayoutManager.Move(elementHostZoomSlider,new Rectangle(elementHostWindowingSlider.Right+4,elementHostToolBarMain.Bottom,LayoutManager.Scale(231),LayoutManager.Scale(25)));
			LayoutManager.Move(toolBarPaint,new Rectangle(elementHostZoomSlider.Right+1,elementHostToolBarMain.Bottom,Width-(elementHostZoomSlider.Right+1),LayoutManager.Scale(25)));
			if(_isTreeDockerCollapsed){
				imageSelector.Visible=false;
				LayoutManager.Move(panelSplitter,new Rectangle(0,toolBarPaint.Bottom,10,Height-toolBarPaint.Bottom));
				panelSplitter.Invalidate();
			}
			else{
				imageSelector.Visible=true;
				if(LayoutManager.Scale(_widthTree96)>Width-LayoutManager.Scale(150)){//this is the min set in FormODBase for FormImageFloat
					_widthTree96=Width-LayoutManager.Scale(150);
				}
				LayoutManager.Move(elementHostImageSelector,new Rectangle(0,toolBarPaint.Bottom,LayoutManager.Scale(_widthTree96),Height-toolBarPaint.Bottom));
				LayoutManager.Move(panelSplitter,new Rectangle(elementHostImageSelector.Width+1,toolBarPaint.Bottom,10,Height-toolBarPaint.Bottom));
				panelSplitter.Invalidate();
			}
			int heightPanelNote=Math.Min(114,Height-toolBarPaint.Bottom);
			if(!panelNote.Visible){
				heightPanelNote=0;
			}
			LayoutManager.Move(panelNote,new Rectangle(
				panelSplitter.Right+1,
				Height-heightPanelNote-1,
				Width-panelSplitter.Right-1,
				heightPanelNote
				));
			int heightUnmountedBar=Math.Min(LayoutManager.Scale(200),Height-toolBarPaint.Bottom-heightPanelNote);
			if(!elementHostUnmountedBar.Visible){
				heightUnmountedBar=0;
			}
			LayoutManager.Move(elementHostUnmountedBar,new Rectangle(
				panelSplitter.Right+1,
				Height-heightPanelNote-heightUnmountedBar-1,
				Width-panelSplitter.Right-1,
				heightUnmountedBar
				));
			int panelMainTop=toolBarPaint.Bottom;
			if(panelImportAuto.Visible){
				LayoutManager.Move(panelImportAuto,new Rectangle(
					panelSplitter.Right+1,
					toolBarPaint.Bottom,
					Width-panelSplitter.Right-1,
					LayoutManager.Scale(44)));	
				panelMainTop=panelImportAuto.Bottom;
			}
			if(panelDraw.Visible){
				LayoutManager.Move(panelDraw,new Rectangle(
					panelSplitter.Right+1,
					toolBarPaint.Bottom,
					Width-panelSplitter.Right-1,
					LayoutManager.Scale(25)));	
				panelMainTop=panelDraw.Bottom;
			}
			LayoutManager.Move(controlImageDock,new Rectangle(
				panelSplitter.Right+1,
				panelMainTop,//used to sit under panelAcquire, but now gets resized
				Width-panelSplitter.Right-1,
				elementHostUnmountedBar.Top-panelMainTop-2));
			//FormOpenDental has minimized, in this case do not set the bounds for FormImageFloat so that it will restore from the taskbar. However, when there is a zoom scale setting in place, we adjust ControlImageJ's font but the parent is not set yet, so this will be null and cause OD to crash.  A null check prevents this.
			if(FindForm()!=null) {
				if(FindForm().WindowState==FormWindowState.Minimized) {
					return;
				}
			}
		}

		protected override void OnHandleDestroyed(EventArgs e){
		
		}

		///<summary>If a mount is showing, and if no item is selected, then this will select the first open item. If one is already selected, but it's occupied, this does not check that.  There is also no guarantee that one will be selected after this because all positions could be full.</summary>
		private void PreselectFirstItem(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.PreselectFirstItem();
		}

		///<summary></summary>
		private void RefreshModuleData(long patNum) {
			if(patNum==0) {
				_familyCur=null;
				_patient=null;
				SelectTreeNode1(null);//Clear selection and image and reset visibilities. Example: clear PDF when switching patients.
				return;
			}
			_familyCur=Patients.GetFamily(patNum);
			_patient=_familyCur.GetPatient(patNum);
			_patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());//This is where the pat folder gets created if it does not yet exist.
			SelectTreeNode1(null);//needs _patCur, etc.
			if(_patNumLastSecurityLog!=patNum) {
				SecurityLogs.MakeLogEntry(EnumPermType.ImagingModule,patNum,"");
				_patNumLastSecurityLog=patNum;
			}
			if(CloudStorage.IsCloudStorage) {
				ProgressWin progressOD=new ProgressWin();
				progressOD.ActionMain=() =>ImageStore.AddMissingFilesToDatabase(_patient);
				progressOD.ShowDialog();
			}
			else{
				ImageStore.AddMissingFilesToDatabase(_patient);
			}
		}

		private void RefreshModuleScreen() {
			UpdateToolbarButtons();
			//ClearObjects();
			for(int i=0;i<_listFormImageFloats.Count;i++){
				_listFormImageFloats[i].Font=Font;
				_listFormImageFloats[i].Invalidate();
			}
			UserOdPref userOdPref=UserOdPrefs.GetFirstOrDefault(x=>x.FkeyType==UserOdFkeyType.ImageSelectorWidth && x.UserNum==Security.CurUser.UserNum);
			if(userOdPref is null){
				_widthTree96=228;
			}
			else{
				_widthTree96=PIn.Float(userOdPref.ValueString);
			}
			LayoutControls();//to handle dpi changes
			toolBarPaint.Invalidate();
			FillImageSelector(false);
		}
		
		///<summary>Sets cursor, sets pushed, sets toolBarMount visible/invisible, and hides panelDraw if not Pan. This is called when user clicks on one of these buttons or an event from a floater can trigger it.</summary>
		private void SetCropPanAdj(EnumCropPanAdj cropPanAdj){
			_cropPanAdj=cropPanAdj;
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay!=null){
				controlImageDisplay.SetCropPanAdj(cropPanAdj);
			}
			if(toolBarPaint.Buttons.Count==0){
				return;//must be launching a floater from ControlChart prior to initializing this module.
			}
			toolBarPaint.Buttons[TB.Crop.ToString()].IsTogglePushed=false;
			toolBarPaint.Buttons[TB.Pan.ToString()].IsTogglePushed=false;
			toolBarPaint.Buttons[TB.Adj.ToString()].IsTogglePushed=false;
			switch(cropPanAdj){
				case EnumCropPanAdj.Crop:
					toolBarPaint.Buttons[TB.Crop.ToString()].IsTogglePushed=true;
					if(panelDraw.Visible){
						panelDraw.Visible=false;
						LayoutControls();
					}
					break;
				case EnumCropPanAdj.Pan:
					toolBarPaint.Buttons[TB.Pan.ToString()].IsTogglePushed=true;
					break;
				case EnumCropPanAdj.Adj:
					toolBarPaint.Buttons[TB.Adj.ToString()].IsTogglePushed=true;
					if(panelDraw.Visible){
						panelDraw.Visible=false;
						LayoutControls();
					}
					break;
			}
			toolBarPaint.Invalidate();
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Sets the element in _arrayDocumentsShowing of the selected floater.</summary>
		private void SetDocumentShowing(int idx,Document document){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			controlImageDisplay.SetDocumentShowing(idx,document);
		}

		///<summary>Sets cursor, sets which button is pushed, and sets color control. This is called when user clicks on one of these buttons or an event from a floater can trigger it. When using this, always consider that you may also need to call SetCropPanAdj(EnumCropPanAdj.Pan) which also hides panelDraw.</summary>
		private void SetDrawMode(EnumDrawMode drawMode){
			_drawMode=drawMode;
			LayoutToolBarDraw();
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay!=null){
				controlImageDisplay.SetDrawMode(drawMode);
			}
		}

		///<summary>This is generally within IsMountItemSelected.  Sets _idxSelectedInMount within selected floater.</summary>
		private void SetIdxSelectedInMount(int idx){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;//shouldn't happen
			}
			controlImageDisplay.SetIdxSelectedInMount(idx);
		}

		///<summary>Sets the panelnote visibility based on the docked image, and whether it has any note or signature data.</summary>
		private void SetPanelNoteVisibility() {
			if(controlImageDock.ControlImageDisplay_==null){//no image is docked.
				panelNote.Visible=false;
				return;
			}
			//FormImageFloat formImageFloat=_listFormImageFloats[0];
			if(!controlImageDock.ControlImageDisplay_.IsDocumentShowing()){
				panelNote.Visible=false;
				return;
			}
			if(!controlImageDock.ControlImageDisplay_.GetDocumentShowing(0).Note.IsNullOrEmpty()){
				panelNote.Visible=true;
				return;
			}
			if(!controlImageDock.ControlImageDisplay_.GetDocumentShowing(0).Signature.IsNullOrEmpty()){
				panelNote.Visible=true;
				return;
			}
			panelNote.Visible=false;
		}

		///<summary>Sets panelUnmounted visibility based on the docked mount, whether it has any unmounted items, and whether we are in the middle of an acquire.</summary>
		private void SetUnmountedBarVisibility(){
			if(controlImageDock.ControlImageDisplay_==null){//no image is docked.
				elementHostUnmountedBar.Visible=false;
				return;
			}
			if(!controlImageDock.ControlImageDisplay_.IsMountShowing()){
				elementHostUnmountedBar.Visible=false;
				return;
			}
			if(_isAcquiring){
				elementHostUnmountedBar.Visible=true;
				return;
			}
			List<WpfControls.UI.UnmountedObj> listUnmountedObjs=controlImageDock.ControlImageDisplay_.GetUmountedObjs();
			if(listUnmountedObjs.Count>0){
				elementHostUnmountedBar.Visible=true;
				return;
			}
			elementHostUnmountedBar.Visible=false;
		}

		private void ShowFilterWindow(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			FrmImageFilter frmImageFilter=new FrmImageFilter();
			frmImageFilter.ShowOD=_showDrawingsOD;
			frmImageFilter.ShowPearlToothParts=_showDrawingsPearlToothParts==true;
			frmImageFilter.ShowPearlPolyAnnotations=_showDrawingsPearlPolyAnnotations==true;
			frmImageFilter.ShowPearlBoxAnnotations=_showDrawingsPearlBoxAnnotations==true;
			frmImageFilter.ShowPearlMeasurements=_showDrawingsPearlMeasurements==true;
			frmImageFilter.ShowDialog();
			if(!frmImageFilter.IsDialogOK){
				return;
			}
			_showDrawingsOD=frmImageFilter.ShowOD;
			_showDrawingsPearlToothParts=frmImageFilter.ShowPearlToothParts;
			_showDrawingsPearlPolyAnnotations=frmImageFilter.ShowPearlPolyAnnotations;
			_showDrawingsPearlBoxAnnotations=frmImageFilter.ShowPearlBoxAnnotations;
			_showDrawingsPearlMeasurements=frmImageFilter.ShowPearlMeasurements;
			controlImageDisplay.SetShowDrawings(_showDrawingsOD,_showDrawingsPearlToothParts.Value,_showDrawingsPearlPolyAnnotations.Value,_showDrawingsPearlBoxAnnotations.Value,_showDrawingsPearlMeasurements.Value);
		}

		private void ThumbnailRefresh(){
			if(IsDocumentShowing()){
				Bitmap bitmapThumb=Documents.CreateNewThumbnail(GetDocumentShowing(0),GetBitmapShowing(0));
				imageSelector.SetThumbnail(bitmapThumb);
			}
			if(IsMountShowing()){
				Bitmap bitmapThumb=Mounts.GetThumbnail(GetMountShowing().MountNum,_patFolder);
				imageSelector.SetThumbnail(bitmapThumb);
			}
		}

		private void ToolBarColor_Click(){
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();
			if(controlImageDisplay is null){
				return;
			}
			using FormImageDrawColor formImageDrawColor=new FormImageDrawColor();
			//Color colorBack=Color.White;
			if(IsMountShowing()){
				formImageDrawColor.IsMount=true;
				formImageDrawColor.ColorBack=GetMountShowing().ColorBack;
			}
			else{
				formImageDrawColor.ColorBack=Color.White;
			}
			formImageDrawColor.ColorFore=_colorFore;
			formImageDrawColor.ColorTextBack=_colorTextBack;//can be transparent
			formImageDrawColor.ShowDialog();
			if(formImageDrawColor.DialogResult!=DialogResult.OK){
				return;
			}
			_colorFore=formImageDrawColor.ColorFore;
			_colorTextBack=formImageDrawColor.ColorTextBack;//can be transparent
			controlImageDisplay.ColorFore=_colorFore;
			controlImageDisplay.ColorTextBack=_colorTextBack;
			LayoutToolBarDraw();//to show the colors to user
		}

		private void ToolBarDraw_Click(){
			//This is not enabled unless formImageFloat.PanelMain is visible (bitmap is showing)
			//or unless a mount is showing.
			if(panelDraw.Visible){//close it, like a toggle
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				LayoutControls();
				return;
			}
			if(panelImportAuto.Visible){
				panelImportAuto.Visible=false;
				if(_fileSystemWatcher!=null){
					_fileSystemWatcher.EnableRaisingEvents=false;
				}
			}
			SetCropPanAdj(EnumCropPanAdj.Pan);
			panelDraw.Visible=true;
			if(IsMountShowing()){
				_colorFore=GetMountShowing().ColorFore;
				_colorTextBack=GetMountShowing().ColorTextBack;//can be transparent
			}
			SetDrawMode(EnumDrawMode.Text);//This does LayoutToolBarDraw(), which also sets button colors
			LayoutControls();
		}

		private void ToolBarImportAuto(object sender, EventArgs e){
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}
			if(panelDraw.Visible){
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
			}
			PreselectFirstItem();
			textImportAuto.Text=PrefC.GetString(PrefName.AutoImportFolder);
			textImportAuto.ReadOnly=false;
			butBrowse.Visible=true;
			butGoTo.Visible=true;
			butImportStart.Visible=true;
			butImportClose.Text=Lan.g(this,"Close");
			panelImportAuto.Visible=true;
			LayoutControls();
		}

		private void ToolBarMountAcquire_Click(object sender, EventArgs e){
			//If no patient selected, then this button is disabled
			if(!Security.IsAuthorized(EnumPermType.ImageCreate)) {
				return;
			}	
			FrmMountAndAcquire frmMountAndAcquire=new FrmMountAndAcquire();
			frmMountAndAcquire.ShowDialog();
			if(frmMountAndAcquire.IsDialogCancel){
				return;
			}
			MountDef mountDef=frmMountAndAcquire.MountDefSelected;
			if(mountDef!=null){
				long defNumCategory;
				if(mountDef.DefaultCat==0){
					//Normal behavior of adding any image: selected or first category
					defNumCategory=GetCurrentCategory();
				}
				else{
					//always use this category no matter what they have selected
					defNumCategory=mountDef.DefaultCat;
				}
				Mount mount=Mounts.CreateMountFromDef(mountDef,_patient.PatNum,defNumCategory);
				Def defDocCategory=Defs.GetDef(DefCat.ImageCats,mount.DocCategory);
				string logText="Mount Created: "+mount.Description+" with category "
					+defDocCategory.ItemName;
				SecurityLogs.MakeLogEntry(EnumPermType.ImageCreate,_patient.PatNum,logText);
				FillImageSelector(false);
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
			}
			ImagingDevice imagingDevice=frmMountAndAcquire.ImagingDeviceSelected;
			if(imagingDevice is null){
				return;
			}
			//Acquire from here down--------------------------------------------------------------------------------------------
			PreselectFirstItem();
			//Might still not be one selected
			_isAcquiring=true;
			SetUnmountedBarVisibility();
			LayoutControls();
			//Below here was previously in the Start button--------------------------------------------------------------------
			if(imagingDevice.DeviceType==EnumImgDeviceType.TwainMulti){
				if(!IsMountShowing()){
					MessageBox.Show(Lan.g(this,"Please create an empty mount first."));
					return;
				}
			}
			_deviceController=new DeviceController();
			_deviceController.ImgDeviceControlType=ConvertToImgDeviceControlType(imagingDevice.DeviceType);
			_deviceController.HandleWindow=Handle;
			_deviceController.ShowTwainUI=imagingDevice.ShowTwainUI;
			_deviceController.TwainName=imagingDevice.TwainName;
			if(ODEnvironment.IsCloudServer) {
				if(!CloudClientL.IsCloudClientRunning()) {
					return;
				}
				if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.Twain){
					try{
						ODCloudClient.TwainInitializeDevice(_deviceController.ShowTwainUI);
					}
					catch(Exception ex){
						MsgBox.Show(ex.Message);
						_deviceController=null;
						return;
					}
					GlobalFormOpenDental.LockODForMountAcquire(isEnabled:false);
					ProgressWin progressWin=new ProgressWin();
					progressWin.ActionMain=() => {
						while(true) {
							ODCloudClient.TwainAcquireBitmapStart(_deviceController.TwainName,doThrowException: true,doShowProgressBar: false);
							Bitmap bitmap=null;
							string scannerState="scanning";
							while(scannerState=="scanning" || scannerState=="setup") {
									//Scan is "setup" when starting up or currently "scanning"
									scannerState=ODCloudClient.CheckBitmapIsAcquired();
									Thread.Sleep(100);
							}
							if(scannerState=="done") {
									//Scan was successful and retrieving the bitmap
									bitmap=ODCloudClient.TwainGetAcquiredBitmap();
							}
							else if(scannerState=="cancelled") {
									//Scan was cancelled
									bitmap?.Dispose();
									bitmap=null;
									break;
							}
							else {
									//Scan had an error and was not successful retrieving the bitmap
									Exception exception=new Exception(scannerState);
									throw exception;
							}
							Thread.Sleep(100);
							if(bitmap==null) {
									break; //Cancel the scanning task
							}
							if(!(bool)this.Invoke((Func<bool>)(()=>PlaceAcquiredBitmapInUI(bitmap)))) {
								break;
							}
							if(!IsMountShowing()) {//single
								break;
							}
						}
						if(!IsMountShowing()) {
							return;
						}
						if(GetMountShowing().AdjModeAfterSeries) {
							this.Invoke(()=> {
								SetCropPanAdj(EnumCropPanAdj.Adj);
								LayoutControls();
							});
						}
					};
					progressWin.ActionCancel=ODCloudClient.TwainCloseScanner;
					progressWin.StartingMessage=Lan.g(this,"Getting Scanned Image(s)")+"...";
					try{
						progressWin.ShowDialog();
					}
					catch(Exception ex){
						if(!ex.Message.Contains("Thread was being aborted.")){
							MessageBox.Show(this, "An error occurred: " + ex.Message);
						}
					}
					finally{
						ODCloudClient.TwainCloseScanner();
						GlobalFormOpenDental.LockODForMountAcquire(isEnabled:true);
						LayoutControls();//To refresh the mount after acquiring images.
					}
					if(progressWin.IsCancelled) {
						GlobalFormOpenDental.LockODForMountAcquire(isEnabled:true);
						LayoutControls();//To refresh the mount after acquiring images.
					}
				}
				return;
			}
			//Below here NOT Thinfinity
			try{
				_deviceController.InitializeDevice();
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				_deviceController=null;
				return;
			}
			if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.TwainMulti){
				AcquireMulti();
				if(GetMountShowing().AdjModeAfterSeries) {
					SetCropPanAdj(EnumCropPanAdj.Adj);
					LayoutControls();
				}
				return;
			}
			while(true){
				Bitmap bitmap=null;
				try{
					bitmap=_deviceController.AcquireBitmap();
				}
				catch(ImagingDeviceException ex){
					if(ex.Message!=""){//If user cancels, there is no text.  We could test e.DeviceStatus instead
						MessageBox.Show(ex.Message);
					}
					//when user cancels, we will keep panelUnmounted open so that they can do things like retake
					break;
				}
				if(!PlaceAcquiredBitmapInUI(bitmap)){
					break;
				}
				if(!IsMountShowing()){//single
					break;
				}
			}
			if(!IsMountShowing()){
				return;
			}
			if(GetMountShowing().AdjModeAfterSeries){
				SetCropPanAdj(EnumCropPanAdj.Adj);
				LayoutControls();
			}
		}

		///<summary></summary>
		private void AcquireMulti(){
			if(!EZTwain.OpenSource(_deviceController.TwainName)){
				return; 
			}
			EZTwain.SetMultiTransfer(true);
			if(!_deviceController.ShowTwainUI){
				EZTwain.SetHideUI(true);
			}
			do{
				IntPtr hdib=EZTwain.Acquire(ParentForm.Handle);
				if(hdib==IntPtr.Zero) {
					break;
				}
				Bitmap bitmap=(Bitmap)EZTwain.DIB_ToImage(hdib);
				PlaceAcquiredBitmapInUI(bitmap);
				EZTwain.DIB_Free(hdib);
			} 
			while(!EZTwain.IsDone());
			EZTwain.CloseSource();
			_deviceController=null;
			//when user cancels, we will keep panelAcquire open so that they can do things like retake
			LayoutControls();
		}
		
		///<summary>Returns true if successful. Returns false if it failed or if no more images should be acquired.</summary>
		private bool PlaceAcquiredBitmapInUI(Bitmap bitmap){
			Document document=null;
			if(bitmap is null){
				return false;
			}
			try {
				document=ImageStore.Import(bitmap,GetCurrentCategory(),ImageType.Radiograph,_patient,mimeType:"image/tiff",doPrintHeading:true);
			}
			catch(Exception ex) {
				bitmap?.Dispose();
				MessageBox.Show(Lan.g(this,"Unable to save")+": "+ex.Message);
				return false;
			}
			if(!IsMountShowing()){//single
				FillImageSelector(false);//Reload and keep new document selected.
				SelectTreeNode1(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				bitmap?.Dispose();
				return true;
			}
			//From here down is mount======================================================================================
			ControlImageDisplay controlImageDisplay=GetControlImageDisplaySelected();//will always succeed because we verified in IsMountShowing
			if(IsMountItemSelected() //They shouldn't have selected an occupied spot.
				|| GetIdxSelectedInMount()==-1)
			{
				//We will try to find an unoccupied spot.
				List<MountItem> listMountItemsAvail2=GetAvailSlots(1);
				if(listMountItemsAvail2 is null){//no more available slots, so start saving in unmounted area
					SetIdxSelectedInMount(-1);
				}
				else{
					MountItem mountItem2=controlImageDisplay.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listMountItemsAvail2[0].MountItemNum);
					if(mountItem2==null){//should not be possible
						return false;
					}
					SetIdxSelectedInMount(controlImageDisplay.GetListMountItems().IndexOf(mountItem2));
				}
			}
			MountItem mountItem=null;
			if(GetIdxSelectedInMount()==-1){
				mountItem=new MountItem();
				mountItem.MountNum=GetMountShowing().MountNum;
				mountItem.ItemOrder=-1;//unmounted
				mountItem.Width=100;//arbitrary; it will scale
				mountItem.Height=100;
				MountItems.Insert(mountItem);
				WpfControls.UI.UnmountedObj unmountedObj=new WpfControls.UI.UnmountedObj();
				unmountedObj.MountItem_=mountItem;
				unmountedObj.Document_=document;
				unmountedObj.SetBitmap(new Bitmap(bitmap));
				unmountedBar.AddObject(unmountedObj);
			}
			else{
				mountItem=controlImageDisplay.GetListMountItems()[GetIdxSelectedInMount()];
			}
			controlImageDisplay.DocumentAcquiredForMount(document,bitmap,mountItem,GetMountShowing().FlipOnAcquire);
			bitmap?.Dispose();
			//Decide if we are done----------------------------------------------------------------
			if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.TwainMulti){
				//any extras will go in unmounted.  Take them all.
				return true;
			}
			List<MountItem> listMountItemsAvail=GetAvailSlots(1);
			if(listMountItemsAvail is null){
				//no more available slots, so we are done
				SetIdxSelectedInMount(-1);
				//unmounted bar will stay visible, so no change to _isAquiring
				LayoutControls();
				return false;
			}
			return true;//ready for another
		}

		/// <summary>Enables or disables menuItemTreePrint depending if the showing document is a PDF or not</summary>
		private void EnableMenuItemTreePrintHelper(ControlImageDisplay controlImageDisplay) {
			if(controlImageDisplay==null || controlImageDisplay.GetDocumentShowing(0)==null) {
				return;
			}
			if(Path.GetExtension(GetDocumentShowing(0).FileName).ToLower()==".pdf"){ 
				menuItemTreePrint.Enabled=false; //pdf printing is handled in Adobe, so users don't need the menu option
				imageSelector.PrintOptionEnabled=false;
			}
			else {
				menuItemTreePrint.Enabled=true;
				imageSelector.PrintOptionEnabled=true;
			}
		}

		///<summary>Enables toolbar buttons if a patient is selected, otherwise disables them.</summary>
		private void UpdateToolbarButtons() {
			if(this.Enabled && _patient!=null) {
				//Enable tools which must always be accessible when a valid patient is selected.
				EnableToolBarsPatient(true);
				DisableAllToolBarButtons();//Disable item specific tools until item chosen.
			}
			else {
				EnableToolBarsPatient(false);//Disable entire menu (besides select patient).
			}
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
	}

	#region External
	public class ToolBarButtonState{
		public bool Print;
		public bool Delete;
		public bool Info;
		public bool Sign;
		public bool Export;
		public bool Copy;
		public bool BrightAndContrast;
		public bool Zoom;
		public bool ZoomOne;
		public bool Crop;
		public bool Pan;
		public bool Adj;
		public bool Size;
		public bool Flip;
		public bool RotateL;
		public bool RotateR;
		public bool Rotate180;
		public bool Draw;
		public bool Unmount;

		public ToolBarButtonState(bool print, bool delete, bool info, bool sign, bool export, bool copy, bool brightAndContrast, bool zoom, bool zoomOne, bool crop, bool pan, bool adj, bool size, bool flip, bool rotateL,bool rotateR, bool rotate180, bool draw,bool unmount)
			{
				Print=print;
				Delete=delete;
				Info=info;
				Sign=sign;
				Export=export;
				Copy=copy;
				BrightAndContrast=brightAndContrast;
				Zoom=zoom;
				ZoomOne=zoomOne;
				Crop=crop;
				Pan=pan;
				Adj=adj;
				Size=size;
				Flip=flip;
				RotateL=rotateL;
				RotateR=rotateR;
				Rotate180=rotate180;
				Draw=draw;
				Unmount=unmount;
		}
	}

	public class ZoomSliderState{
		public System.Windows.Size SizeCanvas;
		public System.Windows.Size SizeImage;
		public float DegreesRotated;

		public ZoomSliderState(System.Windows.Size sizeCanvas,System.Windows.Size sizeImage,float degreesRotated){
			SizeCanvas=sizeCanvas;
			SizeImage=sizeImage;
			DegreesRotated=degreesRotated;
		}
	}

	public class EventArgsImageClick{
		public long PatNum;
		public long MountNum;
		public long DocNum;

		public EventArgsImageClick(long patNum,long mountNum,long docNum){
			PatNum=patNum;
			MountNum=mountNum;
			DocNum=docNum;
		}
	}

	///<summary>3 States.</summary>
	public enum EnumCropPanAdj{
		///<summary>Looks like arrow. Only for docs, not mounts.</summary>
		Crop,
		///<summary>Looks like a hand.</summary>
		Pan,
		///<summary>Cursor is 4 arrows.</summary>
		Adj
	}

	///<summary></summary>
	public enum EnumDrawMode{
		///<summary>This is used in FormImageFloat to indicate that the draw toolbar is not present.  By setting to none, we can accurately test the others without also checking toolbar visibility. Unlike ToothChart, there is no Pointer because we don't generally select objects in images.</summary>
		None,
		///<summary>We allow dragging. User can double click on a text object to edit/delete and single click on an empty spot to add.</summary>
		Text,
		///<summary>No dialog or selection of lines. Eraser and change color affect lines. Click to start a line, second click to end a line.  Or, click and drag, with mouse up finishing the line. To extend a line, click on the last point, then second click to end. No other way to extend a line or add points.</summary>
		Line,
		///<summary>This is a sub-mode of Line. This allows dragging points instead of creating a new line. It also allows adding new points in the middle of a line. The shift key changes the hover cursor to indicate "delete" when over a point.</summary>
		LineEditPoints,
		///<summary>Freehand.</summary>
		Pen,
		///<summary></summary>
		Eraser,
		///<summary></summary>
		ChangeColor,
		///<summary>A sub-mode of Line.</summary>
		LineMeasure,
		///<summary>A sub-mode of Line.</summary>
		LineSetScale,
		///<summary>Always filled. Works just like pen.</summary>
		Polygon
	}

	public enum EnumLoadBitmapType{
		///<summary>Load into _arrayBitmapsShowing[idx] for each item in mount when SelectTreeNode.</summary>
		OnlyIdx,
		///<summary>Load into _arrayBitmapsShowing[idx] and _bitmapRaw when mount image is resized. When using single images rather than mounts, this is the only enum option used, including for SelectTreeNode and cropping.</summary>
		IdxAndRaw,
		///<summary>Load image into _bitmapRaw every time user selects new mount item, but not from disk if no bright/contr yet.</summary>
		OnlyRaw
	}
	#endregion External

}


//Old Bug?
//Mount thumbnails not refreshing?

//Need to document how XVWeb and Suni use the old Images module. (might be done)

//This new Imaging module will never support the following features:
//ClaimPayment/EOB mode.  Old Images module will continue to be used for that.
//EhrAmendment mode.  Can use old images module if it's still required.

//Features that we might add soon:
//Drag mount item, check mouse position to allow longer drags within single position.
//context menu in main panel (see ContrImages.menuMountItem_Opening)
//Yellow outline pixel width could be better: fixed instead of variable
//Switch to Direct2D for hardware speed and effects



