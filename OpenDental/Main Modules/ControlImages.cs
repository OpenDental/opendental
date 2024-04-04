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

namespace OpenDental
{
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
		///<summary>A list of the forms that are currently floating. Can be empty.  When a form is closed, it is removed from the list.</summary>
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
			zoomSlider.FitPressed+=zoomSlider_FitPressed;
			zoomSlider.Zoomed+=zoomSlider_Zoomed;
			toolBarMain=new WpfControls.UI.ToolBar();
			elementHostToolBarMain.Child=toolBarMain;
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
			DataSet dataSet=Documents.RefreshForPatient(new string[] { _patient.PatNum.ToString() });
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
		public void ControlImagesJ_KeyDown(Keys keys){
			if(_formLauncherVideo!=null && !_formLauncherVideo.IsNullOrDisposed()){
				_formLauncherVideo.MethodGetVoid("Parent_KeyDown",keys);
			}
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat!=null){
				formImageFloat.Parent_KeyDown(keys);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.ColorFore=_colorFore;
			formImageFloat.ColorTextBack=_colorTextBack;
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
			formImageFloat.SetShowDrawings(_showDrawingsOD,_showDrawingsPearlToothParts.Value,_showDrawingsPearlPolyAnnotations.Value,_showDrawingsPearlBoxAnnotations.Value,_showDrawingsPearlMeasurements.Value);
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,docNum));
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
			Plugins.HookAddCode(this,"ContrImages.ModuleSelected_end",patNum,docNum);
		}
		
		///<summary></summary>
		public void ModuleUnselected(){
			_familyCur=null;
			if(_listFormImageFloats.Count>0 && _listFormImageFloats[0].IsImageFloatDocked){
				//Close the docked window
				_listFormImageFloats[0].Close();//removal from list happens automatically here
				//but we will not close any undocked.
				//So _listFormImageFloats remains valid and still has all the floaters in it, even when we are in the Chart module. 
				//In CloseFloaters below, we close the floaters when changing patients.
			}
			_patNumLastSecurityLog=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			Plugins.HookAddCode(this,"ContrImages.ModuleUnselected_end");
		}

		///<summary>Called when changing patients by any means.  Closes the undocked floating image windows.</summary>
		public void CloseFloaters(){
			for(int i=_listFormImageFloats.Count-1;i>=0;i--){//go backwards
				//Actually, it also closes the docked window. No problem.
				if(!_listFormImageFloats[i].IsDisposed){
					_listFormImageFloats[i].Close();//remove gets handled automatically here
				}
			}
		}

		///<summary>Called externally. Example, Chart module double click on a thumbnail. Specify either mountNum or docNum.</summary>
		public void LaunchFloater(long patNum,long docNum,long mountNum){
			FormImageFloat formImageFloat=CreateFloater();
			_familyCur=Patients.GetFamily(patNum);
			_patient=_familyCur.GetPatient(patNum);
			_patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			formImageFloat.PatientCur=_patient;
			formImageFloat.PatFolder=_patFolder;
			formImageFloat.ZoomSliderValue=zoomSlider.Value;
			formImageFloat.IsImageFloatDocked=false;
			formImageFloat.DidLaunchFromChartModule=true;
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
			//SelectTreeNode must come before show, because that will trigger Activated, then imageSelector.SetSelected
			//But it must come after bounds are set for the zoom to be correct.
			NodeTypeAndKey nodeTypeAndKey=null;
			if(docNum>0){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Document,docNum);
			}
			if(mountNum>0){
				nodeTypeAndKey=new NodeTypeAndKey(EnumImageNodeType.Mount,mountNum);
			}
			formImageFloat.SelectTreeNode(nodeTypeAndKey);
			formImageFloat.Show();
			formImageFloat.SetDesktopBounds(location.X,location.Y,size.Width,size.Height);//#2
			//the above line can trigger a resize due to dpi change, so once more:
			formImageFloat.SetDesktopBounds(location.X,location.Y,size.Width,size.Height);//#3
			if(screenArray.Length>1){
				formImageFloat.WindowState=FormWindowState.Maximized;
			}
		}

		///<summary>Fired when user clicks on tree and also for automated selection that's not by mouse, such as image import, image paste, etc.  Can pass in NULL.  localPathImported will be set only if using Cloud storage and an image was imported.  We want to use the local version instead of re-downloading what was just uploaded.  nodeObjTag does not need to be same object, but must match type and priKey.</summary>
		public void SelectTreeNode(NodeTypeAndKey nodeTypeAndKey,string localPathImportedCloud="") {
			//Select the node always, but perform additional tasks when necessary (i.e. load an image, or mount).	
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType!=EnumImageNodeType.None){	
				imageSelector.SetSelected(nodeTypeAndKey.NodeType,nodeTypeAndKey.PriKey);//this is redundant when user is clicking, but harmless 
			}
			FormImageFloat formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.GetNodeTypeAndKey().IsMatching(nodeTypeAndKey));
			if(formImageFloat!=null && !formImageFloat.IsDisposed){//found the doc/mount we're after already showing in a floater.
				formImageFloat.Select();
				//This triggers FormImageFloat_Activated which enables toolbar buttons, etc
				if(formImageFloat.WindowState==FormWindowState.Minimized){
					formImageFloat.Restore();
				}
				//if(forceRefresh){
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				formImageFloat.SelectTreeNode(nodeTypeAndKey,localPathImportedCloud);
				if(formImageFloat.IsDisposed){
					//see note 35 lines down
					return;
				}
				formImageFloat.Select();
				if(IsMountShowing()){
					unmountedBar.SetObjects(formImageFloat.GetUmountedObjs());
					unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
				}
				SetPanelNoteVisibility();
				_isAcquiring=false;
				SetUnmountedBarVisibility();
				LayoutControls();
				//SetZoomSlider();
				FillSignature();
				formImageFloat.EnableToolBarButtons();
				EnableMenuItemTreePrintHelper(formImageFloat);
				//}
				return;
			}
			bool reuseExistingForm=false;
			if(_listFormImageFloats.Count>0 && _listFormImageFloats[0].IsImageFloatDocked && !_listFormImageFloats[0].IsDisposed){
				reuseExistingForm=true;
				formImageFloat=_listFormImageFloats[0];
				//close the draw panel because we are going to change to a different image in the window
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				//LayoutControls();//happens below
			}
			else{
				formImageFloat=CreateFloater();
			}
			formImageFloat.PatientCur=_patient;
			formImageFloat.PatFolder=_patFolder;
			formImageFloat.ZoomSliderValue=zoomSlider.Value;
			if(reuseExistingForm){
				formImageFloat.SelectTreeNode(nodeTypeAndKey,localPathImportedCloud);
				if(formImageFloat.IsDisposed){
					//We have reports of many UEs here and a few lines down.
					//These are two completely different impossible ways to end up with a disposed form.
					//Unclear how the form is disposed, but we must handle it.
					return;
				}
				formImageFloat.Select();
			}
			else{
				_listFormImageFloats.Insert(0,formImageFloat);//docked form must always be at idx 0 
				formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
				//SelectTreeNode must come before show, because that will trigger Activated, then imageSelector.SetSelected
				//But it must come after bounds are set for the zoom to be correct.
				formImageFloat.SelectTreeNode(nodeTypeAndKey,localPathImportedCloud);
				if(formImageFloat.IsDisposed){
					//See the comments 13 lines up
					return;
				}
				formImageFloat.Show(this);
			}
			if(IsMountShowing()){
				unmountedBar.SetObjects(formImageFloat.GetUmountedObjs());
				unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
			}
			SetPanelNoteVisibility();
			_isAcquiring=false;
			SetUnmountedBarVisibility();
			LayoutControls();
			//SetZoomSlider();
			FillSignature();
			formImageFloat.EnableToolBarButtons();
			EnableMenuItemTreePrintHelper(formImageFloat);
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

		private void butColor_Click(object sender,EventArgs e) {
			
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat==null){
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
			SelectTreeNode(nodeTypeAndKey);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(!formImageFloat.HideWebBrowser()) {
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
				NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
				formImageFloat.SelectTreeNode(nodeTypeAndKey);
				unmountedBar.SetObjects(formImageFloat.GetUmountedObjs());
				unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
				return;			
			}
			//From here down is Document=================================================================================================
			Document document=GetDocumentShowing(0);
			if(document==null){
				return;//Unexplained error
			}
			string ext=ImageStore.GetExtension(document);
			if(ext==".jpg" || ext==".jpeg" || ext==".gif" 
				|| document.ImgType==ImageType.Radiograph || document.ImgType==ImageType.Photo) 
			{
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
				if(ODBuild.IsThinfinity()) {
					string tempFile=ImageStore.GetFilePath(document,_patFolder);
					ThinfinityUtils.HandleFile(tempFile);
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
				else {
					Process.Start(tempFile);
				}
			}
		}

		private void FormImageFloat_Activated(object sender, EventArgs e){
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			//Debug.WriteLine(DateTime.Now.ToString()+" IsImageFloatSelected:"+formImageFloat.IsImageFloatSelected.ToString());
			//Based on the above debug testing, this gets hit fairly frequently: every time you use a button in the toolbar and then go back into the floater.
			//We want to SetDrawMode(EnumDrawMode.None) and hide the draw toobar, but only if we change the selected floater.
			if(!formImageFloat.IsImageFloatSelected){
				SetCropPanAdj(EnumCropPanAdj.Pan);
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				LayoutControls();
			}
			for(int i=0;i<_listFormImageFloats.Count;i++){
				//deactivate all other floaters
				if(_listFormImageFloats[i]!=sender){
					_listFormImageFloats[i].IsImageFloatSelected=false;
				}
			}
			formImageFloat.IsImageFloatSelected=true;
			formImageFloat.EnableToolBarButtons();
			EnumImageNodeType imageNodeType=formImageFloat.GetSelectedType();
			long priKey=formImageFloat.GetSelectedPriKey();
			imageSelector.SetSelected(imageNodeType,priKey);
			ZoomSliderState zoomSliderState=formImageFloat.ZoomSliderStateInitial;
			if(zoomSliderState!=null){
				zoomSlider.SetValueInitialFit(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
				zoomSlider.SetValueAndMax(formImageFloat.ZoomSliderValue);
			}
			formImageFloat.SetWindowingSlider();
		}

		private void FormImageFloat_KeyDown(KeyEventArgs e) {
			EventKeyDown?.Invoke(this,e);
		}

		private void FormImageFloat_FormClosed(object sender, FormClosedEventArgs e){
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			SetDrawMode(EnumDrawMode.None);
			panelDraw.Visible=false;
			LayoutControls();
			for(int i=0;i<_listFormImageFloats.Count;i++){
				if(_listFormImageFloats[i]==formImageFloat){
					_listFormImageFloats.RemoveAt(i);
					break;
				}
			}
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

		private void FormImageFloat_SetZoomSlider(object sender,ZoomSliderState zoomSliderState){
			zoomSlider.SetValueInitialFit(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
			((FormImageFloat)sender).ZoomSliderValue=zoomSlider.Value;
		}

		private void FormImageFloat_ZoomSliderSetByWheel(object sender,float deltaZoom){
			zoomSlider.SetByWheel(deltaZoom);
			((FormImageFloat)sender).ZoomSliderValue=zoomSlider.Value;
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
			FormImageFloat formImageFloat=(FormImageFloat)sender;
			if(formImageFloat.IsImageFloatDocked){
				MessageBox.Show(this,Lan.g(this,"Already docked."));//must specify different owner because FormImageFloatWindows will close.
				return;
			}
			if(_listFormImageFloats[0].IsImageFloatDocked){
				if(MessageBox.Show(this,Lan.g(this,"Another image is already docked. Dock this one instead?"),"",MessageBoxButtons.YesNo)!=DialogResult.Yes){
					return;
				}
				//Delete the other docked window
				_listFormImageFloats[0].Close();//removal from list happens automatically here
			}
			formImageFloat.IsImageFloatDocked=true;
			formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
			//but then, if moving from the other screen with different dpi, the form will resize, so:
			Application.DoEvents();
			formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
			formImageFloat.SetZoomSlider();
			int idx=_listFormImageFloats.IndexOf(formImageFloat);
			if(idx!=0){
				_listFormImageFloats.RemoveAt(idx);
				_listFormImageFloats.Insert(0,formImageFloat);
			}
			NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
			SelectTreeNode(nodeTypeAndKey);//This is necessary when this window replaces an old docked window
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
			formImageFloat.SelectTreeNode(nodeTypeAndKey);
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount) {
				unmountedBar.SetObjects(formImageFloat.GetUmountedObjs());
				unmountedBar.SetColorBack(ColorOD.ToWpf(GetMountShowing().ColorBack));
			}
			//this resets zoom and pan
		}

		private void imageSelector_SelectionChangeCommitted(object sender,EventArgs e) {
			EnumImageNodeType nodeType=imageSelector.GetSelectedType();
			long priKey=imageSelector.GetSelectedKey();
			NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey(nodeType,priKey);
			SelectTreeNode(nodeTypeAndKey);
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				return;//user can take another single
			}
			//From here down is mount-----------------------------------------------------------------------------
			Bitmap bitmap=(Bitmap)Bitmap.FromFile(e.FullPath);
			FormImageFloat formImageFloat=GetFormImageFloatSelected();//will always succeed because we verified floater idxSelected in IsMountShowing()
			if(IsMountItemSelected()){
				//They shouldn't have selected an item, but we will try to find an unoccupied spot.
				List<MountItem> listAvail2=GetAvailSlots(1);
				if(listAvail2 is null){//no more available slots, so start saving in unmounted area
					SetIdxSelectedInMount(-1);
				}
				else{
					MountItem mountItem2=formImageFloat.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listAvail2[0].MountItemNum);
					if(mountItem2==null){//should not be possible
						return;
					}
					SetIdxSelectedInMount(formImageFloat.GetListMountItems().IndexOf(mountItem2));
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
				mountItem=formImageFloat.GetListMountItems()[idxSelectedInMount];
			}
			formImageFloat.DocumentAcquiredForMount(document,bitmap,mountItem,GetMountShowing().FlipOnAcquire);
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
			mountItem=formImageFloat.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listAvail[0].MountItemNum);
			if(mountItem==null){//should not be possible
				return;
			}
			SetIdxSelectedInMount(formImageFloat.GetListMountItems().IndexOf(mountItem));
			//wait for next event to fire
		}

		private void formVideo_BitmapCaptured(object sender, Bitmap bitmap){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.formVideo_BitmapCaptured(sender,bitmap);
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
			SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			switch(((MenuItem)sender).Index) {
				case 0://print
					ToolBarPrint_Click(this,new EventArgs());
					break;
				case 1://delete
					formImageFloat.ToolBarDelete_Click(this,new EventArgs());
					break;
				case 2://info
					formImageFloat.ToolBarInfo_Click(this,new EventArgs());
					break;
			}
		}

		///<summary>This is the one for WPF</summary>
		private void menuTree_Click2(object sender,System.EventArgs e) {
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None || imageSelector.GetSelectedType()==EnumImageNodeType.Category){
				return;//Probably the user has no patient selected
			}
			//Categories mostly blocked at the click
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}

			switch(((WpfControls.UI.MenuItem)sender).Name) {
				case "Print":
					ToolBarPrint_Click(this,new EventArgs());
					break;
				case "Delete":
					formImageFloat.ToolBarDelete_Click(this,new EventArgs());
					break;
				case "Info":
					formImageFloat.ToolBarInfo_Click(this,new EventArgs());
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.printDocument_PrintPage(sender,e);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
			SelectTreeNode(nodeTypeAndKey);
		}

		private void unmountedBar_Remount(object sender, WpfControls.UI.UnmountedObj e){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			int idx=formImageFloat.GetIdxSelectedInMount();
			if(idx==-1){
				List<MountItem> listAvail=GetAvailSlots(1);
				if(listAvail is null){//no more available slots
					MsgBox.Show(this,"Please select an item in the mount first.");
					return;
				}
				else{
					idx=formImageFloat.GetListMountItems().IndexOf(listAvail[0]);
					SetIdxSelectedInMount(idx);
				}
			}
			Document document=formImageFloat.GetDocumentShowing(idx);
			if(document!=null){
				//unmount the old document
				MountItem mountItemCopy=formImageFloat.GetListMountItems()[idx].Copy();
				mountItemCopy.ItemOrder=-1;
				MountItems.Insert(mountItemCopy);//it will now have a new PK
				document.MountItemNum=mountItemCopy.MountItemNum;
				Documents.Update(document);
			}
			e.Document_.MountItemNum=formImageFloat.GetListMountItems()[idx].MountItemNum;
			Documents.Update(e.Document_);
			MountItems.Delete(e.MountItem_);
			NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
			SelectTreeNode(nodeTypeAndKey);
		}

		private void unmountedBar_Retake(object sender, EventArgs e){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;//should never happen
			}
			int idx=formImageFloat.GetIdxSelectedInMount();
			if(idx==-1){
				MsgBox.Show(this,"This is intended to be used in the middle of a series of image acquisitions. It will unmount the prior image and then re-acquire.");
				return;
			}
			List<MountItem> listMountItems=formImageFloat.GetListMountItems();
			MountItem mountItem=listMountItems[idx];
			if(idx==0 && mountItem.ItemOrder==0){
				MsgBox.Show(this,"There is no previous image to retake.");
				return;
			}
			Document document=formImageFloat.GetDocumentShowing(idx);
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
				formImageFloat.SetIdxSelectedInMount(idx);
				document=formImageFloat.GetDocumentShowing(idx);
				if(document==null) {
					MsgBox.Show(this,"There is no previous image to retake.");
					return;
				}
			}
			else{
				//this means they clicked on the one that they want to retake
			}
			MountItem mountItemUnmounted=formImageFloat.GetListMountItems()[idx].Copy();
			mountItemUnmounted.ItemOrder=-1;
			MountItems.Insert(mountItemUnmounted);//it will now have a new PK
			document.MountItemNum=mountItemUnmounted.MountItemNum;
			Documents.Update(document);
			NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
			SelectTreeNode(nodeTypeAndKey);
			//There is now a new mountItem in the unmounted list, so we have to add 1 to the idx just to keep it in the same spot as it already is.
			formImageFloat.SetIdxSelectedInMount(idx+1);
			idx=formImageFloat.GetIdxSelectedInMount();
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

		private void zoomSlider_FitPressed(object sender, EventArgs e){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.ZoomSliderFitPressed();
		}

		private void zoomSlider_Zoomed(object sender, EventArgs e){
			//fires repeatedly while dragging
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.ZoomSliderValue=zoomSlider.Value;
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			switch((TB)e.Button.Tag) {
				case TB.ZoomOne:
					formImageFloat.ToolBarZoomOne_Click();
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
					formImageFloat.ToolBarSize_Click();
					break;
				case TB.Flip:
					formImageFloat.ToolBarFlip_Click();
					break;
				case TB.RotateL:
					formImageFloat.ToolBarRotateL_Click();
					break;
				case TB.RotateR:
					formImageFloat.ToolBarRotateR_Click();
					break;
				case TB.Rotate180:
					formImageFloat.ToolBarRotate180_Click();
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
			GetOrMakeFloater().ToolBarCopy_Click(sender,e);
		}

		private void ToolBarDelete_Click(object sender, EventArgs e){
			GetOrMakeFloater().ToolBarDelete_Click(sender,e);
		}

		private void toolBarDraw_ButtonClick(object sender, ODToolBarButtonClickEventArgs e){
			if(e.Button.Tag.GetType()!=typeof(TB)) {
				return;
			}
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			formImageFloat.ToolBarExport_Click();
		}

		private void ToolBarExportTIFF(object sender, EventArgs e){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			formImageFloat.ToolBarExport_Click(doExportAsTiff:true);
		}

		private void ToolBarImport_Click(object sender, EventArgs e){
			GetOrMakeFloater().ToolBarImport_Click(sender,e);
		}
		
		private void ToolBarInfo_Click(object sender, EventArgs e){
			GetOrMakeFloater().ToolBarInfo_Click(sender,e);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			GlobalFormOpenDental.GotoImage(frmPatientSelect.PatNumSelected,0);
			//The line above will close all undocked floaters and will clear any docked floater.
			//When we select a document/mount, then we could be dealing with a different floater than before.
			NodeTypeAndKey nodeTypeAndKeyCat;
			if(document!=null){
				nodeTypeAndKeyCat=new NodeTypeAndKey(EnumImageNodeType.Category,document.DocCategory);
				SelectTreeNode(nodeTypeAndKeyCat);
			}
			if(mount!=null){
				nodeTypeAndKeyCat=new NodeTypeAndKey(EnumImageNodeType.Category,mount.DocCategory);
				SelectTreeNode(nodeTypeAndKeyCat);
			}
			//Get the new docked floater for this patient
			formImageFloat=GetFormImageFloatSelected();
			formImageFloat.ToolBarPasteTypeAndKey(nodeTypeAndKeyOriginal);
			imageSelector.SetSelected(nodeTypeAndKeyOriginal.NodeType,nodeTypeAndKeyOriginal.PriKey);
			//Delete Old=====================================================================================
			if(document!=null){
				try {
					formImageFloat.ClearPDFBrowser();//Dispose of the web browser control that has a hold on the PDF that is showing.
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
			GetOrMakeFloater().ToolBarPaste_Click(sender,e);
		}
		
		private void ToolBarPrint_Click(object sender,EventArgs e){
			if(!IsDocumentShowing() && !IsMountShowing()){
				MsgBox.Show(this,"Cannot print. No document currently selected.");
				return;
			}
			if(IsDocumentShowing()){
				if(Path.GetExtension(GetDocumentShowing(0).FileName).ToLower()==".pdf") {//Selected document is PDF, we handle differently than documents that aren't pdf.
					FormImageFloat formImageFloat=GetFormImageFloatSelected();//Will always work because we used IsDocumentShowing
					formImageFloat.PdfPrintPreview();
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
				FormImageFloat formImageFloat=GetOrMakeFloater();
				pearl.ListBitmaps=formImageFloat.GetListBitmaps();
				if(IsMountShowing()){
					pearl.DocNum=0;
					pearl.ListMountItems=formImageFloat.GetListMountItems();
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
				SelectTreeNode(nodeTypeAndKey);
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				FormImageFloat formImageFloat=GetFormImageFloatSelected();
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				FormImageFloat formImageFloat=GetFormImageFloatSelected();
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				FormImageFloat formImageFloat=GetFormImageFloatSelected();
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				FormImageFloat formImageFloat=GetFormImageFloatSelected();
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
			if(_listFormImageFloats.Count==0){
				MsgBox.Show(this,"No image selected.");//just in case
				return;
			}
			if(!_listFormImageFloats[0].IsImageFloatDocked || !_listFormImageFloats[0].IsImageFloatSelected){
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;//shouldn't happen
			}
			int idx=formImageFloat.GetIdxSelectedInMount();
			MountItem mountItemCopy=formImageFloat.GetListMountItems()[idx].Copy();
			mountItemCopy.ItemOrder=-1;
			MountItems.Insert(mountItemCopy);//it will now have a new PK
			Document document=formImageFloat.GetDocumentShowing(idx);
			document.MountItemNum=mountItemCopy.MountItemNum;
			Documents.Update(document);
			NodeTypeAndKey nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
			SelectTreeNode(nodeTypeAndKey);
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
			if(_formLauncherVideo.IsNullOrDisposed()){
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

		private FormImageFloat CreateFloater(){
			FormImageFloat formImageFloat=new FormImageFloat();
			formImageFloat.IsImageFloatDocked=true;
			formImageFloat.IsImageFloatSelected=true;
			formImageFloat.ListFormImageFloats=_listFormImageFloats;
			formImageFloat.ImageSelector_=imageSelector;
			formImageFloat.Activated += FormImageFloat_Activated;
			formImageFloat.FormClosed += FormImageFloat_FormClosed;
			formImageFloat.EventEnableToolBarButtons+=(sender,toolBarButtonState)=>EnableToolBarButtons(toolBarButtonState);
			formImageFloat.EventFillTree+=(sender,keepSelection)=>FillImageSelector(keepSelection);
			formImageFloat.EventKeyDown+=(sender,e)=>FormImageFloat_KeyDown(e);
			formImageFloat.EventSelectTreeNode+=(sender,nodeTypeAndKey)=>{
				if(nodeTypeAndKey is null){
					nodeTypeAndKey=formImageFloat.GetNodeTypeAndKey();
				}
				SelectTreeNode(nodeTypeAndKey);
			};
			formImageFloat.EventSetCropPanEditAdj+=(sender,cropPanAdj)=>SetCropPanAdj(cropPanAdj);
			formImageFloat.EventSetDrawMode+=(sender,drawMode)=>SetDrawMode(drawMode);
			formImageFloat.EventSetWindowingSlider+=FormImageFloat_SetWindowingSlider;
			formImageFloat.EventThumbnailNeedsRefresh+=(sender,e)=>ThumbnailRefresh();
			formImageFloat.EventWindowClicked+=FormImageFloat_WindowClicked;
			formImageFloat.EventWindowCloseOthers += FormImageFloat_WindowCloseOthers;
			formImageFloat.EventWindowDockThis += FormImageFloat_WindowDockThis;
			formImageFloat.EventWindowShowAll += FormImageFloat_WindowShowAll;
			formImageFloat.EventSetZoomSlider+=FormImageFloat_SetZoomSlider;
			formImageFloat.EventZoomSliderSetByWheel+=FormImageFloat_ZoomSliderSetByWheel;
			formImageFloat.EventZoomSliderSetValueAndMax+=(sender,newVal)=>zoomSlider.SetValueAndMax(newVal);
			formImageFloat.IsImageFloatDockedChanged+=FormImageFloat_IsImageFloatDockedChanged;
			return formImageFloat;
		}

		///<summary>Deletes the specified document from the database and refreshes the tree view. Set securityCheck false when creating a new document that might get cancelled.  Document is passed in because it might not be in the tree if the image folder it belongs to is now hidden.</summary>
		private void DeleteDocument(bool isVerbose,bool doSecurityCheck,Document document) {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.DeleteDocument(isVerbose,doSecurityCheck,document);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return null;
			}
			return formImageFloat.GetAvailSlots(countNeed);
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayBitmapsShowing of the selected floater. Can return null.</summary>
		private Bitmap GetBitmapShowing(int idx) {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return null;
			}
			return formImageFloat.GetBitmapShowing(idx);
		}

		///<summary>Gets the DefNum category of the current selection. The current selection can be a folder itself, or a document within a folder. If nothing selected, then it returns the DefNum of first in the list.</summary>
		private long GetCurrentCategory() {
			return imageSelector.GetSelectedCategory();
		}

		///<summary>Specify 0 when a single document is selected, or specify the idx within a mount.  Pulls from _arrayDocumentsShowing of the selected floater. Can return null.</summary>
		private Document GetDocumentShowing(int idx) {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return null;
			}
			return formImageFloat.GetDocumentShowing(idx);
		}

		private string GetHashString(Document doc) {
			return ImageStore.GetHashString(doc,_patFolder);
		}

		/// <summary>Can return null.</summary>
		private FormImageFloat GetFormImageFloatDocked(){
			FormImageFloat formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.IsImageFloatDocked);
			return formImageFloat;
		}

		/// <summary>Can return null.</summary>
		private FormImageFloat GetFormImageFloatSelected(){
			FormImageFloat formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.IsImageFloatSelected);
			return formImageFloat;
		}

		///<summary>This is generally within IsMountItemSelected.  Gets _idxSelectedInMount from selected floater.</summary>
		private int GetIdxSelectedInMount(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return -1;//shouldn't happen
			}
			return formImageFloat.GetIdxSelectedInMount();
		}

		///<summary>Pulls _mountShowing from the selected floater. Can return null.</summary>
		private Mount GetMountShowing() {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return null;
			}
			return formImageFloat.GetMountShowing();
		}
		
		/// <summary>Gets an existing floater or makes a new one. Always returns a valid floater.</summary>
		private FormImageFloat GetOrMakeFloater(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat!=null){
				return formImageFloat;
			}
			//If no floater is selected, assume that they want to use the docked floater.
			formImageFloat=GetFormImageFloatDocked();
			if(formImageFloat!=null){
				return formImageFloat;
			}
			//make one
			SelectTreeNode(null);
			formImageFloat=GetFormImageFloatSelected();
			return formImageFloat;
		}

		///<summary>Invalidates the color image setting and recalculates.  This is not on a separate thread.  Instead, it's just designed to run no more than about every 300ms, which completely avoids any lockup.</summary>
		private void InvalidateSettingsColor() {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.InvalidateSettingsColor();
		}

		///<summary>Returns true if a valid document is showing.  This is very different from testing the property _documentShowing, which would return true for mounts.</summary>
		private bool IsDocumentShowing(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return false;
			}
			return formImageFloat.IsDocumentShowing();
		}

		///<summary>Returns true if a valid mount is showing.</summary>
		private bool IsMountShowing(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return false;
			}
			return formImageFloat.IsMountShowing();
		}

		///<summary>Returns true if a valid mountitem is selected and there's a valid bitmap in that location.</summary>
		private bool IsMountItemSelected(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return false;
			}
			return formImageFloat.IsMountItemSelected();
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
			LayoutManager.Move(panelMain,new Rectangle(
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
			FormImageFloat formImageFloat=GetFormImageFloatDocked();
			if(formImageFloat!=null){
				formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
			}
		}

		protected override void OnHandleDestroyed(EventArgs e){
		
		}

		///<summary>If a mount is showing, and if no item is selected, then this will select the first open item. If one is already selected, but it's occupied, this does not check that.  There is also no guarantee that one will be selected after this because all positions could be full.</summary>
		private void PreselectFirstItem(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.PreselectFirstItem();
		}

		///<summary></summary>
		private void RefreshModuleData(long patNum) {
			if(patNum==0) {
				_familyCur=null;
				_patient=null;
				SelectTreeNode(null);//Clear selection and image and reset visibilities. Example: clear PDF when switching patients.
				return;
			}
			_familyCur=Patients.GetFamily(patNum);
			_patient=_familyCur.GetPatient(patNum);
			_patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());//This is where the pat folder gets created if it does not yet exist.
			SelectTreeNode(null);//needs _patCur, etc.
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat!=null){
				formImageFloat.SetCropPanAdj(cropPanAdj);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.SetDocumentShowing(idx,document);
		}

		///<summary>Sets cursor, sets which button is pushed, and sets color control. This is called when user clicks on one of these buttons or an event from a floater can trigger it. When using this, always consider that you may also need to call SetCropPanAdj(EnumCropPanAdj.Pan) which also hides panelDraw.</summary>
		private void SetDrawMode(EnumDrawMode drawMode){
			_drawMode=drawMode;
			LayoutToolBarDraw();
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat!=null){
				formImageFloat.SetDrawMode(drawMode);
			}
		}

		///<summary>This is generally within IsMountItemSelected.  Sets _idxSelectedInMount within selected floater.</summary>
		private void SetIdxSelectedInMount(int idx){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;//shouldn't happen
			}
			formImageFloat.SetIdxSelectedInMount(idx);
		}

		///<summary>Sets the panelnote visibility based on the docked image, and whether it has any note or signature data.</summary>
		private void SetPanelNoteVisibility() {
			if(_listFormImageFloats.Count<1){
				panelNote.Visible=false;
				return;
			}
			FormImageFloat formImageFloat=_listFormImageFloats[0];
			if(!formImageFloat.IsImageFloatDocked){//no image is docked.
				panelNote.Visible=false;
				return;
			}
			if(!formImageFloat.IsDocumentShowing()){
				panelNote.Visible=false;
				return;
			}
			if(!formImageFloat.GetDocumentShowing(0).Note.IsNullOrEmpty()){
				panelNote.Visible=true;
				return;
			}
			if(!formImageFloat.GetDocumentShowing(0).Signature.IsNullOrEmpty()){
				panelNote.Visible=true;
				return;
			}
			panelNote.Visible=false;
		}

		///<summary>Sets panelUnmounted visibility based on the docked mount, whether it has any unmounted items, and whether we are in the middle of an acquire.</summary>
		private void SetUnmountedBarVisibility(){
			if(_listFormImageFloats.Count<1){
				elementHostUnmountedBar.Visible=false;
				return;
			}
			FormImageFloat formImageFloat=_listFormImageFloats[0];
			if(!formImageFloat.IsImageFloatDocked){//not docked.
				elementHostUnmountedBar.Visible=false;
				return;
			}
			if(!formImageFloat.IsMountShowing()){
				elementHostUnmountedBar.Visible=false;
				return;
			}
			if(_isAcquiring){
				elementHostUnmountedBar.Visible=true;
				return;
			}
			List<WpfControls.UI.UnmountedObj> unmountedObjs=formImageFloat.GetUmountedObjs();
			if(unmountedObjs.Count>0){
				elementHostUnmountedBar.Visible=true;
				return;
			}
			elementHostUnmountedBar.Visible=false;
		}

		private void ShowFilterWindow(){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
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
			formImageFloat.SetShowDrawings(_showDrawingsOD,_showDrawingsPearlToothParts.Value,_showDrawingsPearlPolyAnnotations.Value,_showDrawingsPearlBoxAnnotations.Value,_showDrawingsPearlMeasurements.Value);
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
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
			formImageFloat.ColorFore=_colorFore;
			formImageFloat.ColorTextBack=_colorTextBack;
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				bitmap?.Dispose();
				return true;
			}
			//From here down is mount======================================================================================
			FormImageFloat formImageFloat=GetFormImageFloatSelected();//will always succeed because we verified in IsMountShowing
			if(IsMountItemSelected() //They shouldn't have selected an occupied spot.
				|| GetIdxSelectedInMount()==-1)
			{
				//We will try to find an unoccupied spot.
				List<MountItem> listMountItemsAvail2=GetAvailSlots(1);
				if(listMountItemsAvail2 is null){//no more available slots, so start saving in unmounted area
					SetIdxSelectedInMount(-1);
				}
				else{
					MountItem mountItem2=formImageFloat.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listMountItemsAvail2[0].MountItemNum);
					if(mountItem2==null){//should not be possible
						return false;
					}
					SetIdxSelectedInMount(formImageFloat.GetListMountItems().IndexOf(mountItem2));
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
				mountItem=formImageFloat.GetListMountItems()[GetIdxSelectedInMount()];
			}
			formImageFloat.DocumentAcquiredForMount(document,bitmap,mountItem,GetMountShowing().FlipOnAcquire);
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
		private void EnableMenuItemTreePrintHelper(FormImageFloat formImageFloat) {
			if(formImageFloat==null || formImageFloat.GetDocumentShowing(0)==null) {
				return;
			}
			if(Path.GetExtension(GetDocumentShowing(0).FileName).ToLower()==".pdf"){ 
				menuItemTreePrint.Enabled=false; //pdf printing is handled in Adobe, so users don't need the menu option
			}
			else {
				menuItemTreePrint.Enabled=true;
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


//2022-03-26 Todo:



//Bug?
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



