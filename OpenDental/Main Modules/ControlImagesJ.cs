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
	///<summary>The Images Module.  See long comments at bottom of this file.</summary>
	public partial class ControlImagesJ : UserControl{
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		///<summary>Color for drawing lines. Can be overridden (but not changed) by a mount default color. Mount default colors are not a feature yet, so override will only kick in when a black mount is used, and the override will be white.</summary>
		private Color _colorDraw=Color.Black;
		///<summary>Color that foreground text will draw. Can be overridden (but not changed) by a mount default color. Mount default colors are not a feature yet, so override will only kick in when a black mount is used. The override will be white on black.</summary>
		private Color _colorDrawText=Color.Black;
		///<summary>Color for background of text. Can be overridden (but not changed) by a mount default color. Transparent (no background) can be selected and is supported.</summary>
		private Color _colorDrawTextBack=Color.White;
		///<summary>One of these 3 states is active at any time.</summary>
		private EnumCropPanAdj _cropPanAdj;
		private DeviceController _deviceController; 
		///<summary>If the draw toolbar is showing, then one of these 5 modes will be active.</summary>
		private EnumDrawMode _drawMode;
		private Family _familyCur=null;
		private FileSystemWatcher _fileSystemWatcher;
		private FormVideo _formVideo;
		///<summary>A list of the forms that are currently floating. Can be empty.  When a form is closed, it is removed from the list.</summary>
		private List<FormImageFloat> _listFormImageFloats=new List<FormImageFloat>();
		private bool _initializedOnStartup=false;
		///<summary>Collapse is toggled with the triangle on the sizer.  Always starts out not collapsed.</summary>
		private bool _isTreeDockerCollapsed=false;
		///<summary>Collapsing while the mouse is down triggers another mouse move, so this prevents taking action on that phantom mouse move.</summary>
		private bool _isTreeDockerCollapsing=false;
		private Patient _patCur=null;
		///<summary>Set with each RefreshModuleData, and that's where it's set if it doesn't yet exist.  For now, we are not using _patCur.ImageFolder, because we haven't tested whether it properly updates the patient object.  We don't want to risk using an outdated patient folder path.  And we don't want to waste time refreshing PatCur after every ImageStore.GetPatientFolder().</summary>
		private string _patFolder="";
		///<summary>Prevents too many security logs for this patient.</summary>
		private long _patNumLastSecurityLog=0;
		///<summary>For moving the splitter</summary>
		private Point _pointMouseDown=new Point(0,0);
		///<summary>Used to display Topaz signatures on Windows.</summary>
		private Control _sigBoxTopaz;
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

		#region Events
		///<summary>For Ctrl-P for select patient. Won't fire if handled instead by Adobe for print.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<KeyEventArgs> EventKeyDown=null;
		#endregion

		#region Enums
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
			Size,
			DrawTool,
			Text,
			Line,
			Pen,
			Eraser,
			ChangeColor,
			Add,
			Edit,
			EditPoints,
			Close
		}

		#endregion Enums

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
		}

		///<summary>Key down from FormOpenDental is passed here to allow triggering video capture.  As long as this module is open, all key down events are sent here.</summary>
		public void ControlImagesJ_KeyDown(Keys keys){
			if(_formVideo!=null){
				_formVideo.Parent_KeyDown(keys);
			}
		}
		
		///<summary>Layout the Main and Paint toolbars.</summary>
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
			if(_cropPanAdj==EnumCropPanAdj.Crop){
				toolBarPaint.Buttons[TB.Crop.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Pan"),10,"",TB.Pan);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanAdj==EnumCropPanAdj.Pan){
				toolBarPaint.Buttons[TB.Pan.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Adj"),20,Lan.g(this,"Adjust position"),TB.Adj);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarPaint.Buttons.Add(button);
			if(_cropPanAdj==EnumCropPanAdj.Adj){
				toolBarPaint.Buttons[TB.Adj.ToString()].Pushed=true;
			}
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Size"),-1,Lan.g(this,"Set Size"),TB.Size));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"FlipH"),11,Lan.g(this,"Flip Horizontally"),TB.Flip));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"-90"),12,Lan.g(this,"Rotate Left"),TB.RotateL));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"+90"),13,Lan.g(this,"Rotate Right"),TB.RotateR));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"180"),-1,Lan.g(this,"Rotate 180"),TB.Rotate180));
			toolBarPaint.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			toolBarPaint.Buttons.Add(new ODToolBarButton(Lan.g(this,"Draw"),-1,Lan.g(this,"Lines and Annotations"),TB.DrawTool));
			toolBarMain.Invalidate();
			toolBarPaint.Invalidate();
		}

		///<summary>Layout the Draw toolbar only.</summary>
		public void LayoutToolBarDraw() {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			//First, the Color button
			if(_drawMode==EnumDrawMode.Text){
				labelColor.Visible=false;
				butColor.Visible=true;
				butColor.Text="Color";
				LayoutManager.Move(butColor,new Rectangle(0,0,LayoutManager.Scale(54),LayoutManager.Scale(25)));
				if(IsMountShowing() && GetMountShowing().ColorBack.ToArgb()==Color.Black.ToArgb()){
					butColor.ForeColor=Color.White;
					butColor.BackColor=Color.Black;
				}
				else{
					butColor.ForeColor=_colorDrawText;
					butColor.BackColor=_colorDrawTextBack;
				}
				formImageFloat.ColorDrawText=butColor.ForeColor;
				formImageFloat.ColorDrawBackground=butColor.BackColor;
			}
			if(_drawMode==EnumDrawMode.Line
				|| _drawMode==EnumDrawMode.Pen
				|| _drawMode==EnumDrawMode.ChangeColor)
			{
				labelColor.Visible=true;
				butColor.Visible=true;
				butColor.Text="";
				LayoutManager.Move(labelColor,new Rectangle(0,0,LayoutManager.Scale(44),LayoutManager.Scale(25)));
				LayoutManager.Move(butColor,new Rectangle(LayoutManager.Scale(44),0,LayoutManager.Scale(25),LayoutManager.Scale(25)));
				//todo: remember the drawing color at the class level. User can change, and it needs to stick.
				//butColor.ForeColor=Color.Black;//there is no foreground color because no text.
				butColor.BackColor=ColorOD.Control;
			}
			if(_drawMode==EnumDrawMode.Eraser){
				labelColor.Visible=false;
				butColor.Visible=false;
			}
			//Toolbar itself------------------------------------------------------------------------
			toolBarDraw.Buttons.Clear();
			ODToolBarButton button;
			button=new ODToolBarButton(Lan.g(this,"Text"),-1,"",TB.Text);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Text){
				toolBarDraw.Buttons[TB.Text.ToString()].Pushed=true;
			}
			//These are for later:
			/*
			button=new ODToolBarButton(Lan.g(this,"Line"),-1,"",TB.Line);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Line){
				toolBarDraw.Buttons[TB.Line.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Pen"),-1,"",TB.Pen);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Pen){
				toolBarDraw.Buttons[TB.Pen.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Eraser"),-1,"",TB.Eraser);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.Eraser){
				toolBarDraw.Buttons[TB.Eraser.ToString()].Pushed=true;
			}
			button=new ODToolBarButton(Lan.g(this,"Change Color"),-1,"",TB.ChangeColor);
			button.Style=ODToolBarButtonStyle.ToggleButton;
			toolBarDraw.Buttons.Add(button);
			if(_drawMode==EnumDrawMode.ChangeColor){
				toolBarDraw.Buttons[TB.ChangeColor.ToString()].Pushed=true;
			}
			*/
			toolBarDraw.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			/*if(_drawMode==EnumDrawMode.Text){
				toolBarDraw.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),-1,"",TB.Add));
				toolBarDraw.Buttons.Add(new ODToolBarButton(Lan.g(this,"Edit"),-1,"",TB.Edit));
				toolBarDraw.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			}*/
			if(_drawMode==EnumDrawMode.Line){
				button=new ODToolBarButton(Lan.g(this,"Edit Points"),-1,"",TB.EditPoints);
				button.Style=ODToolBarButtonStyle.ToggleButton;
				toolBarDraw.Buttons.Add(button);
				toolBarDraw.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
//todo: toggle state of this button
			}
			toolBarDraw.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"",TB.Close));
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,docNum));
			}
			Plugins.HookAddCode(this,"ContrImages.ModuleSelected_end",patNum,docNum);
		}
		
		///<summary></summary>
		public void ModuleUnselected(){
			_familyCur=null;
			if(_listFormImageFloats.Count>0 && _listFormImageFloats[0].IsImageFloatDocked){
				//Close the docked window
				_listFormImageFloats[0].Close();//removal from list happens automatically here
				//but we will not close any undocked.
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

		///<summary>Fired when user clicks on tree and also for automated selection that's not by mouse, such as image import, image paste, etc.  Can pass in NULL.  localPathImported will be set only if using Cloud storage and an image was imported.  We want to use the local version instead of re-downloading what was just uploaded.  nodeObjTag does not need to be same object, but must match type and priKey.</summary>
		public void SelectTreeNode(NodeTypeAndKey nodeTypeAndKey,string localPathImportedCloud="") {
			//Select the node always, but perform additional tasks when necessary (i.e. load an image, or mount).	
			if(nodeTypeAndKey!=null && nodeTypeAndKey.NodeType!=EnumImageNodeType.None){	
				imageSelector.SetSelected(nodeTypeAndKey.NodeType,nodeTypeAndKey.PriKey);//this is redundant when user is clicking, but harmless 
			}
			FormImageFloat formImageFloat=_listFormImageFloats.FirstOrDefault(x=>x.GetNodeTypeAndKey().IsMatching(nodeTypeAndKey));
			if(formImageFloat!=null){//found the doc/mount we're after already showing in a floater.
				formImageFloat.Select();
				//This triggers FormImageFloat_Activated which enables toolbar buttons, etc
				if(formImageFloat.WindowState==FormWindowState.Minimized){
					formImageFloat.Restore();
				}
				return;
			}
			bool reuseExistingForm=false;
			if(_listFormImageFloats.Count>0 && _listFormImageFloats[0].IsImageFloatDocked){
				reuseExistingForm=true;
				formImageFloat=_listFormImageFloats[0];
				//close the draw panel because we are going to change to a different image in the window
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				//LayoutControls();//happens below
			}
			else{
				formImageFloat=new FormImageFloat();
				formImageFloat.IsImageFloatDocked=true;
				formImageFloat.IsImageFloatSelected=true;
				formImageFloat.ListFormImageFloats=_listFormImageFloats;
				formImageFloat.ImageSelector_=imageSelector;
				formImageFloat.Activated += FormImageFloat_Activated;
				formImageFloat.FormClosed += FormImageFloat_FormClosed;
				formImageFloat.EventEnableToolBarButtons+=(sender,toolBarButtonState)=>EnableToolBarButtons(toolBarButtonState);
				formImageFloat.EventFillTree+=(sender,keepSelection)=>FillTree(keepSelection);
				formImageFloat.EventKeyDown+=(sender,e)=>FormImageFloat_KeyDown(e);
				formImageFloat.EventSetCropPanEditAdj+=(sender,cropPanAdj)=>SetCropPanAdj(cropPanAdj);
				formImageFloat.EventSetWindowingSlider+=FormImageFloat_SetWindowingSlider;
				formImageFloat.EventWindowClicked+=FormImageFloat_WindowClicked;
				formImageFloat.EventWindowCloseOthers += FormImageFloat_WindowCloseOthers;
				formImageFloat.EventWindowDockThis += FormImageFloat_WindowDockThis;
				formImageFloat.EventWindowShowAll += FormImageFloat_WindowShowAll;
				formImageFloat.EventSetZoomSlider+=FormImageFloat_SetZoomSlider;
				formImageFloat.EventZoomSliderSetByWheel+=FormImageFloat_ZoomSliderSetByWheel;
				formImageFloat.EventZoomSliderSetValueAndMax+=(sender,newVal)=>zoomSlider.SetValueAndMax(newVal);
				formImageFloat.IsImageFloatDockedChanged+=FormImageFloat_IsImageFloatDockedChanged;
			}
			formImageFloat.PatientCur=_patCur;
			formImageFloat.PatFolder=_patFolder;
			formImageFloat.ZoomSliderValue=zoomSlider.Value;
			if(reuseExistingForm){
				formImageFloat.SelectTreeNode(nodeTypeAndKey,localPathImportedCloud);
				formImageFloat.Select();
			}
			else{
				_listFormImageFloats.Insert(0,formImageFloat);//docked form must always be at idx 0
				//formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
				//SelectTreeNode must come before show, because that will trigger Activated, then imageSelector.SetSelected
				//But it must come after bounds are set for the zoom to be correct.
				formImageFloat.SelectTreeNode(nodeTypeAndKey,localPathImportedCloud);
				formImageFloat.Show(this);
				formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
			}
			SetPanelNoteVisibility();
			LayoutControls();
			//SetZoomSlider();
			FillSignature();
			formImageFloat.EnableToolBarButtons();
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

		private void butColor_Click(object sender,EventArgs e) {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			using FormImageDrawColor formImageDrawColor=new FormImageDrawColor();
			if(_drawMode==EnumDrawMode.Text){
				formImageDrawColor.IsText=true;
				formImageDrawColor.ColorText=butColor.ForeColor;
				formImageDrawColor.ColorBackground=butColor.BackColor;//can be transparent
			}
			else{
				if(IsMountShowing() && GetMountShowing().ColorBack.ToArgb()==Color.Black.ToArgb()){
					formImageDrawColor.ColorBackground=Color.White;
				}
				else{
					formImageDrawColor.ColorBackground=butColor.BackColor;
				}
			}
			formImageDrawColor.ShowDialog();
			if(formImageDrawColor.DialogResult!=DialogResult.OK){
				return;
			}
			if(_drawMode==EnumDrawMode.Text){
				butColor.ForeColor=formImageDrawColor.ColorText;
				butColor.BackColor=formImageDrawColor.ColorBackground;
				_colorDrawText=butColor.ForeColor;
				_colorDrawTextBack=butColor.BackColor;
				formImageFloat.ColorDrawText=butColor.ForeColor;
				formImageFloat.ColorDrawBackground=butColor.BackColor;//can be transparent
			}
			else{
				butColor.BackColor=formImageDrawColor.ColorBackground;
				_colorDraw=butColor.BackColor;
				formImageFloat.ColorDrawBackground=butColor.BackColor;
			}
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
			PreselectFirstItem();//We already did this when clicking toolbar button, but user might have selected a mount after clicking that.
			//If one is still not selected, they will get a warning below
			if(IsMountShowing()){
				if(IsMountItemSelected() || GetIdxSelectedInMount()==-1){
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
				if(GetIdxSelectedInMount()==-1){
					//user must have clicked outside mount to deselect in the middle of a series
					MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
					return;
				}
			}
			ImagingDevice	imagingDevice=comboDevice.GetSelected<ImagingDevice>();	
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
			_deviceController.BitmapAcquired+=_deviceController_BitmapAcquired;
			_deviceController.StatusReceived+=_deviceController_StatusReceived;
			if(ODBuild.IsWeb()) {
				if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.Twain){
					try{
						ODCloudClient.TwainInitializeDevice(_deviceController.ShowTwainUI);
					}
					catch(Exception ex){
						MsgBox.Show(ex.Message);
						_deviceController=null;
						return;
					}
				}
			}
			else {
				try{
					_deviceController.InitializeDevice();
				}
				catch(Exception ex){
					MsgBox.Show(ex.Message);
					_deviceController=null;
					return;
				}
			}
			butStart.Enabled=false;
			butCancel.Visible=false;//.Text=Lan.g(this,"Stop");
			AcquireBitmap();
			if(ODBuild.IsWeb()) {
				LayoutControls();//To refresh the mount after acquiring images.
			}
		}

		private void ContrImages_Resize(object sender, EventArgs e){
			LayoutControls();
		}

		private void _deviceController_BitmapAcquired(object sender,BitmapAcquireEventArgs e){
			Document document=null;
			//Size sizeBitmap;//=e.SizeBitmap;
			Bitmap bitmap=e.Bitmap;
			BitmapDicom bitmapDicom=null;
			//if(bitmap!=null){
			//	sizeBitmap=bitmap.Size;
			//}
			if(bitmap is null){
				if(e.FilePath.IsNullOrEmpty() || !e.FilePath.EndsWith(".dcm")){
					return;
				}
				bitmapDicom=DicomHelper.GetFromFile(e.FilePath);
				//sizeBitmap=new Size(_bitmapDicomRaw.Width,_bitmapDicomRaw.Height);
				DicomHelper.CalculateWindowingOnImport(bitmapDicom);//happens again 8 lines down in ImageStore.Import, so this could be optimized better. 
				bitmap=DicomHelper.ApplyWindowing(bitmapDicom,bitmapDicom.WindowingMin,bitmapDicom.WindowingMax);
			}
			//if(sizeBitmap.Width==0 || bitmap is null){
			if(bitmap is null){
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
				LayoutControls();
				return;
			}
			if(!IsMountShowing()){//single
				FillTree(false);//Reload and keep new document selected.
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				bitmap?.Dispose();
				_deviceController=null;
				panelAcquire.Visible=false;
				LayoutControls();
				return;
			}
			//From here down is mount======================================================================================
			if(IsMountItemSelected()){
				//user must have clicked onto an occupied position in the middle of a series.
				MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
				return;
			}
			if(GetIdxSelectedInMount()==-1){
				//user must have clicked outside mount to deselect in the middle of a series
				MessageBox.Show(Lan.g(this,"Please select an empty mount position first."));
				return;
			}
			FormImageFloat formImageFloat=GetFormImageFloatSelected();//will always succeed because we verified idxSelected
			formImageFloat.DocumentAcquiredForMount(document,bitmap,bitmapDicom);
			bitmap?.Dispose();
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listAvail=GetAvailSlots(1);
			if(listAvail is null){//no more available slots, so we are done
				SetIdxSelectedInMount(-1);
				_deviceController=null;
				panelAcquire.Visible=false;
				LayoutControls();
				return;
			}
			MountItem mountItem=formImageFloat.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listAvail[0].MountItemNum);
			if(mountItem==null){//should not be possible
				_deviceController=null;
				panelAcquire.Visible=false;
				return;
			}
			SetIdxSelectedInMount(formImageFloat.GetListMountItems().IndexOf(mountItem));
			if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.TwainMulti){
				return;//we are already in a loop
			}
			AcquireBitmap();
		}

		private void _deviceController_StatusReceived(object sender, StatusReceivedEventArgs e){
	
		}

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
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,document.PatNum,logText,document.DocNum,document.DateTStamp);
				Document docOld=document.Copy();
				document.DocCategory=e.DefNumNew;
				Documents.Update(document,docOld);
			}
			else if(e.MountNum!=0){
				Mount mountShowing=GetMountShowing();
				string mountOriginalCat=Defs.GetDef(DefCat.ImageCats,mountShowing.DocCategory).ItemName;
				string mountNewCat=Defs.GetDef(DefCat.ImageCats,e.DefNumNew).ItemName;
				SecurityLogs.MakeLogEntry(Permissions.ImageEdit,mountShowing.PatNum,Lan.g(this,"Mount moved from")+" "+mountOriginalCat+" "
					+Lan.g(this,"to")+" "+mountNewCat);
				mountShowing.DocCategory=e.DefNumNew;
				Mounts.Update(mountShowing);
			}
			FillTree(true);
		}

		private void imageSelector_ItemDoubleClick(object sender,EventArgs e) {
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			formImageFloat.HideWebBrowser();
			if(imageSelector.GetSelectedType()==EnumImageNodeType.None
				|| imageSelector.GetSelectedType()==EnumImageNodeType.Category)
			{
				return;
			}
			if(imageSelector.GetSelectedType()==EnumImageNodeType.Mount) {
				using FormMountEdit formMountEdit=new FormMountEdit(GetMountShowing());
				formMountEdit.ShowDialog();//Edits the MountSelected object directly and updates and changes to the database as well.
				if(formMountEdit.DialogResult==DialogResult.OK){
					FillTree(true);//in case description for the mount changed.
					imageSelector.SetSelected(EnumImageNodeType.Mount,GetMountShowing().MountNum);//Need to update _nodeObjTagSelected in case category changed
				}
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
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing(0));
				formDocInfo.ShowDialog();
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					return;
				}
				FillTree(true);
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
				if(ODBuild.IsWeb()) {
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
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading Document...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(_patFolder.Replace("\\","/")
					,GetDocumentShowing(0).FileName
					,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				formProgress.ShowDialog();
				if(formProgress.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
				}
				else {
					string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(GetDocumentShowing(0).FileName));
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
				zoomSlider.SetValue(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
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
			FillTree(false);
			//In case the image had a signature or note:
			SetPanelNoteVisibility();
			LayoutControls();
			FillSignature();
		}

		private void FormImageFloat_IsImageFloatDockedChanged(object sender,EventArgs e) {
			SetPanelNoteVisibility();
			LayoutControls();
			FillSignature();
		}

		private void FormImageFloat_SetWindowingSlider(object sender,WindowingEventArgs windowingEventArgs){
			windowingSlider.MinVal=windowingEventArgs.MinVal;
			windowingSlider.MaxVal=windowingEventArgs.MaxVal;
		}

		private void FormImageFloat_SetZoomSlider(object sender,ZoomSliderState zoomSliderState){
			zoomSlider.SetValue(zoomSliderState.SizeCanvas,zoomSliderState.SizeImage,zoomSliderState.DegreesRotated);
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
				if(GetIdxSelectedInMount()==-1){
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
			Document document = null;
			try{
				document=ImageStore.Import(e.FullPath,GetCurrentCategory(),_patCur);
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,document.DocNum));
				return;//user can take another single
			}
			//From here down is mount-----------------------------------------------------------------------------
			//already verified idxSelected
			Bitmap bitmap=(Bitmap)Bitmap.FromFile(e.FullPath);
			FormImageFloat formImageFloat=GetFormImageFloatSelected();//will always succeed because we verified idxSelected
			formImageFloat.DocumentAcquiredForMount(document,bitmap,null);
			bitmap?.Dispose();
			File.Delete(e.FullPath);
			//Select next slot position, in preparation for next image.-------------------------------------------------
			List<MountItem> listAvail=GetAvailSlots(1);
			if(listAvail is null){//no more available slots, so we are done
				SetIdxSelectedInMount(-1);
				panelImportAuto.Visible=false;
				_fileSystemWatcher.EnableRaisingEvents=false;//will never be null
				MessageBox.Show(Lan.g(this,"No more available mount positions."));
				return;
			}
			MountItem mountItem=formImageFloat.GetListMountItems().FirstOrDefault(x=>x.MountItemNum==listAvail[0].MountItemNum);
			if(mountItem==null){//should not be possible
				return;
			}
			SetIdxSelectedInMount(formImageFloat.GetListMountItems().IndexOf(mountItem));
			//wait for next event to fire
		}

		private void formVideo_BitmapCaptured(object sender, VideoEventArgs e){
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.formVideo_BitmapCaptured(sender,e);
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
			SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
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
			SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Mount,mount.MountNum));
		}

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
					ToolBarPrint_Click();
					break;
				case 1://delete
					formImageFloat.ToolBarDelete_Click();
					break;
				case 2://info
					formImageFloat.ToolBarInfo_Click();
					break;
			}
		}

		private void panelMain_Paint(object sender,PaintEventArgs e) {
			e.Graphics.Clear(Color.White);
		}

		private void panelNote_DoubleClick(object sender, EventArgs e){
			ToolBarSign_Click();
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
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){
				return;
			}
			formImageFloat.printDocument_PrintPage(sender,e);
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

		private void toolBarMain_ButtonClick(object sender, ODToolBarButtonClickEventArgs e){
			if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,_patCur);
				return;
			}
			if(e.Button.Tag.GetType()!=typeof(TB)) {
				return;//This seems to happen frequently. Maybe they are clicking between buttons.
			}
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat is null){//If no floater is selected
				//assume that they want to use the docked floater.
				formImageFloat=GetFormImageFloatDocked();
				if(formImageFloat is null){//If no floater is docked
					//make one
					SelectTreeNode(null);
					formImageFloat=GetFormImageFloatSelected();
				}
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
					formImageFloat.ToolBarDelete_Click();
					break;
				case TB.Info:
					formImageFloat.ToolBarInfo_Click();
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
					formImageFloat.ToolBarImport_Click();
					break;
				case TB.Export:
					formImageFloat.ToolBarExport_Click();
					break;
				case TB.Copy:
					formImageFloat.ToolBarCopy_Click();
					break;
				case TB.Paste:
					formImageFloat.ToolBarPaste_Click();
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
			}
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
				case TB.Text:
					SetDrawMode(EnumDrawMode.Text);
					break;
				case TB.Line:
					SetDrawMode(EnumDrawMode.Line);					
					break;
				case TB.Pen:
					SetDrawMode(EnumDrawMode.Pen);	
					break;
				case TB.Eraser:
					SetDrawMode(EnumDrawMode.Eraser);	
					break;
				case TB.ChangeColor:
					SetDrawMode(EnumDrawMode.ChangeColor);	
					break;
				case TB.Add:
					
					break;
				case TB.Edit:
					
					break;
				case TB.EditPoints:
					
					break;
				case TB.Close:
					SetDrawMode(EnumDrawMode.None);
					panelDraw.Visible=false;
					LayoutControls();
					break;
			}
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
			}
			if(IsMountItemSelected()) {
				Documents.Update(GetDocumentShowing(GetIdxSelectedInMount()));
				ImageStore.DeleteThumbnailImage(GetDocumentShowing(GetIdxSelectedInMount()),_patFolder);
				InvalidateSettingsColor();
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

		#region Methods - Private
		///<summary>Gets called repeatedly during a series.  This sometimes contains blocking calls. If something goes wrong, then it sets _deviceController to null and closes the panel.  If there are no more slots available, this will not acquire a bitmap.</summary>
		private void AcquireBitmap(){
			if(ODBuild.IsWeb()){
				if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.Twain){
					Bitmap bitmap=ODCloudClient.TwainAcquireBitmap(_deviceController.TwainName);
					_deviceController.InvokeBitmapAcquired(this,new BitmapAcquireEventArgs(bitmap,null));
				}
				return;
			}
			if(_deviceController.ImgDeviceControlType==EnumImgDeviceControlType.TwainMulti){
				if(!EZTwain.OpenSource(_deviceController.TwainName)){
					return; 
				}
				EZTwain.SetMultiTransfer(true);
				if(_deviceController.ShowTwainUI){
					AcquireMulti();
				}
				else{
					EZTwain.SetHideUI(true);
					AcquireMulti();
					//Progress bar did not work
					//UI.ProgressOD progressOD=new UI.ProgressOD();
					//progressOD.ActionMain=AcquireMulti;
					//progressOD.ShowDialogProgress();
					//if(progressOD.IsCancelled){
						//Nothing special to do
						//return;
					//}
				}
				EZTwain.CloseSource();
				//if(EZTwain.LastErrorCode()!=0) {
				//	EZTwain.ReportLastError("Error.");//this is a dlg
				//}
				_deviceController=null;
				panelAcquire.Visible=false;
				LayoutControls();
				return;
			}
			try{
				_deviceController.AcquireBitmap();
				//The above line will fire a BitmapAcquired event
			}
			catch(ImagingDeviceException e){
				//Exceptions are currently only thrown from Twain. XDR raises events instead.
				if(e.Message!=""){//If user cancels, there is no text.  We could test e.DeviceStatus instead
					MessageBox.Show(e.Message);
				}
				//no bitmap to dispose of
				_deviceController=null;
				panelAcquire.Visible=false;
				LayoutControls();
				return;
			}
		}

		///<summary></summary>
		private void AcquireMulti(){
			do{
				IntPtr hdib=EZTwain.Acquire(ParentForm.Handle);
				if(hdib==IntPtr.Zero) {
					break;
				}
				Bitmap bitmap=(Bitmap)EZTwain.DIB_ToImage(hdib);
				_deviceController.InvokeBitmapAcquired(this,new BitmapAcquireEventArgs(bitmap,null));
				EZTwain.DIB_Free(hdib);
			} 
			while(!EZTwain.IsDone());
		}

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
		private void DisableAllToolBarButtons() {
			bool enable=false;
			ToolBarButtonState toolBarButtonState=new ToolBarButtonState(print:enable, delete:enable, info:enable, sign:enable, export:enable, copy:enable, brightAndContrast:enable, zoom:enable, zoomOne:enable, crop:enable, pan:enable, adj:enable, size:enable, flip:enable, rotateL:enable, rotateR:enable, rotate180:enable,draw:enable);
			EnableToolBarButtons(toolBarButtonState);
		}

		///<summary>Defined this way to force future programming to consider which tools are enabled and disabled for every possible tool in the menu.  To prevent bugs, you must always use named arguments.  Called when users clicks on Crop/Pan/Mount buttons, clicks Tree, or clicks around on a mount.</summary>
		private void EnableToolBarButtons(ToolBarButtonState toolBarButtonState){
			//bool print, bool delete, bool info, bool sign, bool export, bool copy, bool brightAndContrast, bool zoom, bool zoomOne, bool crop, bool pan, bool adj, bool size, bool flip, bool rotateL,bool rotateR, bool rotate180) {
			//Some buttons don't show here because they are always enabled as long as there is a patient,
			//including Scan, Import, Paste, Templates, Mounts
			toolBarMain.Buttons[TB.Print.ToString()].Enabled=toolBarButtonState.Print;
			toolBarMain.Buttons[TB.Delete.ToString()].Enabled=toolBarButtonState.Delete;
			toolBarMain.Buttons[TB.Info.ToString()].Enabled=toolBarButtonState.Info;
			toolBarMain.Buttons[TB.Sign.ToString()].Enabled=toolBarButtonState.Sign;
			toolBarMain.Buttons[TB.Export.ToString()].Enabled=toolBarButtonState.Export;
			toolBarMain.Buttons[TB.Copy.ToString()].Enabled=toolBarButtonState.Copy;
			toolBarMain.Invalidate();
			windowingSlider.Enabled=toolBarButtonState.BrightAndContrast;
			windowingSlider.Invalidate();
			zoomSlider.Enabled=toolBarButtonState.Zoom;
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
					if(TopazWrapper.GetTopazNumberOfTabletPoints(_sigBoxTopaz)==0) {
						labelInvalidSig.Visible=true;
					}
					//}
				}
			}
			else {//not topaz
				if(GetDocumentShowing(0).Signature!=null && GetDocumentShowing(0).Signature!="") {
					sigBox.Visible=true;
					//if(allowTopaz) {	
					_sigBoxTopaz.Visible=false;
					//}
					sigBox.ClearTablet();
					//sigBox.SetSigCompressionMode(0);
					//sigBox.SetEncryptionMode(0);
					sigBox.SetKeyString(GetHashString(GetDocumentShowing(0)));
					//"0000000000000000");
					//sigBox.SetAutoKeyData(ProcCur.Note+ProcCur.UserNum.ToString());
					//sigBox.SetEncryptionMode(2);//high encryption
					//sigBox.SetSigCompressionMode(2);//high compression
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
			int panelMainTop=toolBarPaint.Bottom;
			if(panelAcquire.Visible){
				LayoutManager.Move(panelAcquire,new Rectangle(
					panelSplitter.Right+1,
					toolBarPaint.Bottom,
					Width-panelSplitter.Right-1,
					LayoutManager.Scale(23)));
				panelMainTop=panelAcquire.Bottom;
			}
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
				panelNote.Top-panelMainTop-2));	
			FormImageFloat formImageFloat=GetFormImageFloatDocked();
			if(formImageFloat!=null){
				formImageFloat.Bounds=new Rectangle(PointToScreen(panelMain.Location),panelMain.Size);
			}
		}

		protected override void OnHandleDestroyed(EventArgs e){
		
		}

		///<summary>If a mount is showing, and if no item is selected, then this will select the first open item. If one is already selected, but it's full, this does not check that.  There is also no guarantee that one will be selected after this because all positions could be full.</summary>
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
				_patCur=null;
				SelectTreeNode(null);//Clear selection and image and reset visibilities. Example: clear PDF when switching patients.
				return;
			}
			_familyCur=Patients.GetFamily(patNum);
			_patCur=_familyCur.GetPatient(patNum);
			_patFolder=ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath());//This is where the pat folder gets created if it does not yet exist.
			SelectTreeNode(null);//needs _patCur, etc.
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
				DisableAllToolBarButtons();//Disable item specific tools until item chosen.
			}
			else {
				EnableToolBarsPatient(false);//Disable entire menu (besides select patient).
			}
			//ClearObjects();
//todo: dpi for floaters?
			LayoutControls();//to handle dpi changes
			toolBarMain.Invalidate();
			toolBarPaint.Invalidate();
			FillTree(false);
		}
		
		///<summary>Sets cursor, sets pushed, sets toolBarMount visible/invisible, and hides panelDraw if not Pan. This is called when user clicks on one of these buttons or an event from a floater can trigger it.</summary>
		private void SetCropPanAdj(EnumCropPanAdj cropPanAdj){
			toolBarPaint.Buttons[TB.Crop.ToString()].Pushed=false;
			toolBarPaint.Buttons[TB.Pan.ToString()].Pushed=false;
			toolBarPaint.Buttons[TB.Adj.ToString()].Pushed=false;
			switch(cropPanAdj){
				case EnumCropPanAdj.Crop:
					toolBarPaint.Buttons[TB.Crop.ToString()].Pushed=true;
					if(panelDraw.Visible){
						panelDraw.Visible=false;
						LayoutControls();
					}
					break;
				case EnumCropPanAdj.Pan:
					toolBarPaint.Buttons[TB.Pan.ToString()].Pushed=true;
					break;
				case EnumCropPanAdj.Adj:
					toolBarPaint.Buttons[TB.Adj.ToString()].Pushed=true;
					if(panelDraw.Visible){
						panelDraw.Visible=false;
						LayoutControls();
					}
					break;
			}
			_cropPanAdj=cropPanAdj;
			toolBarPaint.Invalidate();
			FormImageFloat formImageFloat=GetFormImageFloatSelected();
			if(formImageFloat!=null){
				formImageFloat.SetCropPanAdj(cropPanAdj);
			}
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
			if(panelImportAuto.Visible){
				panelImportAuto.Visible=false;
				if(_fileSystemWatcher!=null){
					_fileSystemWatcher.EnableRaisingEvents=false;
				}
			}
			if(panelDraw.Visible){
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
			}
			PreselectFirstItem();
			//Might still not be one selected
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

		private void ToolBarDraw_Click(){
			//This is not enabled unless formImageFloat.PanelMain is visible (bitmap is showing)
			//or unless a mount is showing.
			if(panelDraw.Visible){//close it, like a toggle
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
				LayoutControls();
				return;
			}
			if(panelAcquire.Visible){
				_deviceController=null;
				panelAcquire.Visible=false;
			}
			if(panelImportAuto.Visible){
				panelImportAuto.Visible=false;
				if(_fileSystemWatcher!=null){
					_fileSystemWatcher.EnableRaisingEvents=false;
				}
			}
			SetCropPanAdj(EnumCropPanAdj.Pan);
			panelDraw.Visible=true;
			SetDrawMode(EnumDrawMode.Text);//This does LayoutToolBarDraw();
			LayoutControls();
		}

		private void ToolBarImportAuto(object sender, EventArgs e){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			if(panelAcquire.Visible){
				_deviceController=null;
				panelAcquire.Visible=false;
			}
			if(panelDraw.Visible){
				SetDrawMode(EnumDrawMode.None);
				panelDraw.Visible=false;
			}
			PreselectFirstItem();
			//If one is still not selected, they will get a warning when they click Start
			textImportAuto.Text=PrefC.GetString(PrefName.AutoImportFolder);
			textImportAuto.ReadOnly=false;
			butBrowse.Visible=true;
			butGoTo.Visible=true;
			butImportStart.Visible=true;
			butImportClose.Text=Lan.g(this,"Close");
			panelImportAuto.Visible=true;
			LayoutControls();
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
				if(!Security.IsAuthorized(Permissions.ImageDelete,GetMountShowing().DateCreated)) {
					return;
				}
			}
			else {
				if(!Security.IsAuthorized(Permissions.ImageDelete,GetDocumentShowing(0).DateCreated)) {
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
			GotoModule.GotoImage(formPatientSelect.SelectedPatNum,0);
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
			}
		}

		private void ToolBarPrint_Click(){
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
				SecurityLogs.MakeLogEntry(Permissions.Printing,_patCur.PatNum,"Patient image "+GetDocumentShowing(0).FileName+" "+GetDocumentShowing(0).Description+" printed");
			}
			if(IsMountShowing()){
				SecurityLogs.MakeLogEntry(Permissions.Printing,_patCur.PatNum,"Patient mount "+GetMountShowing().Description+" printed");
			}
		}

		///<summary>Handles the scan click for ODCloud using similar logic to ToolbarScan_Click()</summary>
		private void ToolbarScanWeb(string scanType) {
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
				MessageBox.Show(Lan.g(this,"Unable to save document")+": "+ex.Message);
			}
			if(bitmapScanned!=null) {
				bitmapScanned.Dispose();
				bitmapScanned=null;
			}//===========================
			if(saved) {
				FillTree(false);//Reload and keep new document selected.
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing(0),isDocCreate:true);
				formDocInfo.ShowDialog(this);
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,doc);
				}
				else {
					FillTree(true);//Update tree, in case the new document's icon or category were modified in formDocInfo.
				}
			}
		}

		///<summary>Valid values for scanType are "doc","xray",and "photo"</summary>
		private void ToolBarScan_Click(string scanType){
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}	
			if(ODBuild.IsWeb()) {
				ToolbarScanWeb(scanType);
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				using FormDocInfo formDocInfo=new FormDocInfo(_patCur,GetDocumentShowing(0),isDocCreate:true);
				formDocInfo.ShowDialog(this);
				if(formDocInfo.DialogResult!=DialogResult.OK) {
					DeleteDocument(false,false,doc);
				}
				else {
					FillTree(true);//Update tree, in case the new document's icon or category were modified in formDocInfo.
				}
			}
		}

		///<summary>Handles the scan multi click for ODCloud using similar logic to ToolbarScanMulti_Click()</summary>
		private void ToolbarScanMultiWeb() {
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
				doc=ImageStore.Import(tempFile,GetCurrentCategory(),_patCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to copy file, May be in use: ") + ex.Message + ": " + tempFile);
				copied = false;
			}
			if(copied) {
				FillTree(false);
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				using FormDocInfo FormD=new FormDocInfo(_patCur,doc,isDocCreate:true);
				FormD.ShowDialog(this);//some of the fields might get changed, but not the filename 
				//Customer complained this window was showing up behind OD.  We changed above line to add a parent form as an attempted fix.
				//If this doesn't solve it we can also try adding FormD.BringToFront to see if it does anything.
				if(FormD.DialogResult!=DialogResult.OK) {
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
			FillTree(true);
		}

		private void ToolBarScanMulti_Click() {
			if(!Security.IsAuthorized(Permissions.ImageCreate)) {
				return;
			}
			if(ODBuild.IsWeb()) {
				ToolbarScanMultiWeb();
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
			NodeTypeAndKey nodeTypeAndKey=null;
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
				SelectTreeNode(new NodeTypeAndKey(EnumImageNodeType.Document,doc.DocNum));
				using FormDocInfo FormD=new FormDocInfo(_patCur,doc,isDocCreate:true);
				FormD.ShowDialog(this);//some of the fields might get changed, but not the filename 
				//Customer complained this window was showing up behind OD.  We changed above line to add a parent form as an attempted fix.
				//If this doesn't solve it we can also try adding FormD.BringToFront to see if it does anything.
				if(FormD.DialogResult!=DialogResult.OK) {
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
			FillTree(true);
		}

		private void ToolBarSign_Click(){
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
			using FormDocSign formDocSign=new FormDocSign(GetDocumentShowing(0),_patCur);//Updates our local document and saves changes to db also.
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
			PreselectFirstItem();
			//still might not be one selected, so test each time
			_formVideo=new FormVideo();
			_formVideo.BitmapCaptured+=formVideo_BitmapCaptured;
			_formVideo.Show();
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;
			_formVideo.Location=new Point(rectangleWorkingArea.Left+rectangleWorkingArea.Width/2-_formVideo.Width/2,//center L/R
				rectangleWorkingArea.Bottom-_formVideo.Height);//bottom
			LayoutControls();
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

		public ToolBarButtonState(bool print, bool delete, bool info, bool sign, bool export, bool copy, bool brightAndContrast, bool zoom, bool zoomOne, bool crop, bool pan, bool adj, bool size, bool flip, bool rotateL,bool rotateR, bool rotate180, bool draw)
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
		}
	}

	///<summary>Allows comparison of items based on nodetype and key.  </summary>
	[Serializable]
	public class NodeTypeAndKey:ISerializable {
		public EnumImageNodeType NodeType;
		public long PriKey;

		private NodeTypeAndKey() {
		}

		///<summary></summary>
		public NodeTypeAndKey(EnumImageNodeType nodeType,long priKey){
			NodeType=nodeType;
			PriKey=priKey;
		}

		///<summary>The special constructor is used to deserialize values.</summary>
		public NodeTypeAndKey(SerializationInfo info, StreamingContext context){
			NodeType=(EnumImageNodeType)info.GetValue("NodeType", typeof(EnumImageNodeType));
			PriKey=(long)info.GetValue("PriKey",typeof(long));
		}

		///<summary>The method is called on serialization for placing on clipboard.</summary>
		public void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("NodeType", NodeType, typeof(EnumImageNodeType));
			info.AddValue("PriKey", PriKey, typeof(long));
		}

		///<summary>Tests whether the two nodeTypeAndKeys match.</summary>
		public bool IsMatching(NodeTypeAndKey nodeTypeAndKey){
			if(nodeTypeAndKey is null){
				return false;
			}
			if(NodeType!=nodeTypeAndKey.NodeType){
				return false;
			}
			if(PriKey==nodeTypeAndKey.PriKey){
				return true;
			}
			return false;
		}
			

		public NodeTypeAndKey Copy(){
			NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey();
			nodeTypeAndKey.NodeType=NodeType;
			nodeTypeAndKey.PriKey=PriKey;
			return nodeTypeAndKey;
		}
	}

	public class ZoomSliderState{
		public Size SizeCanvas;
		public Size SizeImage;
		public int DegreesRotated;

		public ZoomSliderState(Size sizeCanvas,Size sizeImage,int degreesRotated){
			SizeCanvas=sizeCanvas;
			SizeImage=sizeImage;
			DegreesRotated=degreesRotated;
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
		///<summary>We allow dragging and selecting. Remove the Add button and Edit buttons. User can double click on a text object to edit/delete and single click on an empty spot o add.</summary>
		Text,
		///<summary>This mode has a sub-mode toggle called "Edit Points". This sub-mode allows dragging end points instead of creating a new line. No dialog or selection of lines. Eraser and change color affect lines. Click to start a line, second click to stop a line. To extend a line, click on the last point, then second click to stop. No other way to extend a line or add points.</summary>
		Line,
		///<summary>Freehand.</summary>
		Pen,
		///<summary></summary>
		Eraser,
		///<summary></summary>
		ChangeColor
		//Measure?
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



