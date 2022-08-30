using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using OpenDental.UI;
using System.Threading;
using OpenDentBusiness.WebTypes.WebForms;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;

namespace OpenDental {
	///<summary>This is one of the very few forms that must run at 96 dpi for now, regardless of the monitor resolution.  This is accomplished with Dpi.SetUnaware() just prior to instantiation.  It gets bitmap scaled by Windows.  Layout Manager is still responsible for layout.</summary>
	public partial class FormSheetDefEdit:FormODBase {
		#region Fields - Public
		///<summary>If it's internal, then the edit windows will all be set to IsReadOnly.</summary>
		public bool IsInternal;
		#endregion Fields - Public

		#region Fields - Private
		///<summary>Disposed. A single bitmap that is a composite of all image fields, usually 0 or 1. A bit more efficient than redrawing the actual image fields each time.</summary>
		private Bitmap _bitmapBackground;
		private Color _colorBlue=Color.Blue;//FromArgb(0,30,150);
		private Color _colorGray=ColorOD.Gray(10);//Almost black 
		///<summary>Faded out</summary>
		private Color _colorGrayOutline=ColorOD.Gray(235);
		//private Color _colorBlueOutline=Color.FromArgb(200,230,255);
		private Color _colorBlueOutline=Color.Blue;//FromArgb(50,60,255);
		private Color _colorRed=Color.Red;//FromArgb(150,0,10);
		//<summary>Faded out</summary>
		//private Color _colorRedOutline=Color.FromArgb(255,200,210);
		private Color _colorRedOutline=Color.Red;//FromArgb(255,50,60);
		private static Font _fontTabOrder = new Font("Times New Roman",12f,FontStyle.Regular,GraphicsUnit.Pixel);
		///<summary>Disposed</summary>
		private Graphics _graphicsBackground;
		///<summary>When using auto snap feature this will be set to a positive value, otherwise 0.  This is the width and height of the squares that fieldDefs will snap to when enabled.</summary>
		private int _gridSnapDistance=0;
		///<summary>When you first mouse down, if you clicked on a valid control, this will be false.  For drag selection, this must be true.</summary>
		private bool _hasClickedOnBlankSpace;
		///<summary>This stores the previous calculations so that we don't have to recal unless certain things have changed.  The key is the index of the sheetfield.  The data is an array of objects of different types as seen in the code.</summary>
		private Hashtable _hashTableRtfStringCache=new Hashtable();
		///<summary>Set to true if the web forms have been downloaded or if an error occurred.</summary>
		private bool _hasSheetsDownloaded=false;
		///<summary>Keep track of when the bounds of this window are programmatically changed.
		///Used to help prevent resizing when users are simply moving the window.</summary>
		private bool _hasWindowBoundsChanged=false;
		private Image _imageToothChart;
		private bool _isAltDown;
		private bool _isCtrlDown;
		private bool _isDragging;
		private bool _isMouseDown;
		private bool _isResizing;
		private bool _isTabMode;
		private List<Point> _listPointsOfOriginalControls;
		///<summary>List of static image SheetFieldDefs.  Used to track when images change, thus requiring redrawing the images.</summary>
		private List<SheetFieldDef> _listSheetFieldDefsDisplayedImages=new List<SheetFieldDef>();
		///<summary>This is our 'clipboard' for copy/paste of fields.</summary>
		private List<SheetFieldDef> _listSheetFieldDefsCopyPaste;
		private List<SheetFieldDef> _listSheetFieldDefsTabOrder;
		///<summary>List of X coordinates that will be shown on panelMain. Empty if auto snap is not enabled.</summary>
		private List<int> _listSnapXVals=new List<int>();
		///<summary>List of Y coordinates that will be shown on panelMain. Empty if auto snap is not enabled.</summary>
		private List<int> _listSnapYVals=new List<int>();
		///<summary>Alphabetically sorted by Display list of languages NOT translated to when window loads.</summary>
		private List<SheetDefLanguage> _listSheetDefLanguagesUnused;
		///<summary>Alphabetically sorted by Display list of languages translated to when window loads.</summary>
		private List<SheetDefLanguage> _listSheetDefLanguagesUsed;
		///<summary>Infinite undo levels.  Every time the user changes something, a new level is added to this list. The last item in this list should normally match what the user is seeing. If they "undo", then we pull from the list, tracking the _undoLevel instead of removing from the list so that they can "redo".</summary>
		private List<UndoLevel> _listUndoLevels=new List<UndoLevel>();
		///<summary>If the sheet def is linked to any web sheets, this will hold those web sheet defs.</summary>
		private List<WebForms_SheetDef> _listWebForms_SheetDefs;
		private object _lock=new object();
		private Point _pointMouseCurrentPos;
		///<summary></summary>
		private Point _pointMouseOriginalPos;
		private int _pasteOffset=0;
		///<summary>After each 10 pastes to the upper left origin, this increments 10 to shift the next 10 down.</summary>
		private int _pasteOffsetY=0;
		///<summary>Used for drawing the dashed margin lines. Top 40, bottom 60.</summary>
		private Margins _margins=new Margins(0,0,40,60);
		private Random _random=new Random();
		private Rectangle _rectangleRestoreBoundsOld=new Rectangle();
		private SheetDef _sheetDef;
		private SheetEditMobileCtrl _sheetEditMobileCtrl=new SheetEditMobileCtrl() { Size=new Size(642,758) };
		private SheetFieldDef _sheetFieldDefResizing;
		///<summary>Most sheets will only have the default mode. Dynamic module layouts can have multiple.</summary>
		private SheetFieldLayoutMode _sheetFieldLayoutMode;
		private int _sizeResizeGrabHandle=5;
		///<summary>The original size of the field at the beginning of the resize drag.</summary>
		private Size _sizeResizeOriginal;
		///<summary>0-indexed, starting from the end of the list. 0 represents the last item in the undo list. Since it matches what the viewer sees, we are 0 levels deep. An undo will move us to position 1, which is the second from the end in the list. When we are at level 1, we have 1 redo available.</summary>
		private int _undoLevel;
		private int counterTemp;
		///<summary>If using cloud storage, this keeps track of images that we have already attempted to download so we don't try to download them again when the sheet resizes (Sheets resize several times on load). This prevents OD from trying to download the same SheetFieldDef's image mulitple times when the image is missing from the SheetImages folder.</summary>
		private List<long> _listSheetFieldDefNums=new List<long>();
		#endregion Fields - Private

		#region Constructor
		public FormSheetDefEdit(SheetDef sheetDef) {
			InitializeComponent();
			InitializeLayoutManager(is96dpi:true);
			InitializeComponentSheetEditMobile();
			Lan.F(this);
			_sheetDef=sheetDef;
		}
		#endregion Constructor

		#region Properties
		///<summary>The currently selected languages three letter abbr from comboLanguages. When 'Default' is selected an empty string is returned. When 'Add Ne' is selected NULL is returned.</summary>
		private string GetSelectedLanguageThreeLetters() {
			if(comboLanguages.SelectedIndex<0) {
				//Selection is forced to 1 in InitTranslations(...). Only not set for internal sheet defs or dynamic layout defs.
				return "";
			}
			if(comboLanguages.SelectedIndex==0) {//'Add New'
				return null;
			}
			if(comboLanguages.SelectedIndex==1) {//'Default' or no seleciton
				return "";
			}
			return _listSheetDefLanguagesUsed[comboLanguages.SelectedIndex-2].ThreeLetters;//2 for 'Add New' and 'Default'
		}

		///<summary>The currently selected languages display string from comboLanguages. When 'Default' is selected an empty string is returned. When 'Add New' is selected NULL is returned.</summary>
		private string GetSelectedLanguageDisplay() {
			if(comboLanguages.SelectedIndex<0) {
				//Selection is forced to 1 in InitTranslations(...). Only not set for internal sheet defs or dynamic layout defs.
				return "";
			}
			if(comboLanguages.SelectedIndex-1==0) {//'Add New'
				return null;
			}
			if(comboLanguages.SelectedIndex==1) {//'Default' or no seleciton
				return "";
			}
			return _listSheetDefLanguagesUsed[comboLanguages.SelectedIndex-2].Display;//2 for 'Add New' and 'Default'
		}

		///<summary>Returns a list of all translations that are currently being used in memory. If user selects a translation but it isn't in the DB yet it will be included in this list.</summary>
		private List<string> GetListUsedTranslations() {
			return _sheetDef.SheetFieldDefs.Select(x => x.Language).Distinct().ToList();
		}
		#endregion Properties

		#region Methods - Event Handlers
		private void butAddMobileHeader_Click(object sender,EventArgs e) {
			using InputBox inputBox=new InputBox(Lan.g(this,"Mobile Header")) {
				MaxInputTextLength=255,
				ShowDelete=true,
			};
			if(inputBox.ShowDialog()!=DialogResult.OK || inputBox.IsDeleteClicked) {
				return;
			}
			AddNewSheetFieldDef(SheetFieldDef.NewMobileHeader(inputBox.textResult.Text));
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butAddField_Click(object sender,EventArgs e) {
			/*
			using FormSheetFieldDefAdd formSheetFieldDefAdd=new FormSheetFieldDefAdd();
			formSheetFieldDefAdd.SheetDefCur=_sheetDef;
			formSheetFieldDefAdd.SelectedLanguageThreeLetters=GetSelectedLanguageThreeLetters();
			formSheetFieldDefAdd.SheetFieldLayoutMode_=_sheetFieldLayoutMode;
			formSheetFieldDefAdd.ShowDialog();
			if(formSheetFieldDefAdd.DialogResult!=DialogResult.OK){
				return;
			}
			AddNewSheetFieldDef(formSheetFieldDefAdd.SheetFieldDefNew);
			FillFieldList();
			if(formSheetFieldDefAdd.DoRefreshDoubleBuffer){
				RefreshBitmapBackground();
			}
			panelMain.Invalidate();*/
		}

		private void butAlignCenterH_Click(object sender,EventArgs e) {
			if(listBoxFields.SelectedIndices.Count<2) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefsSelected=new List<SheetFieldDef>();
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				listSheetFieldDefsSelected.Add(listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]]);
			}
			if(listSheetFieldDefsSelected.Exists(f1 => listSheetFieldDefsSelected.Exists(f2 => f1.YPos==f2.YPos && f1!=f2))) {
				MsgBox.Show(this,"Cannot align controls. Two or more selected controls will overlap.");
				return;
			}
			float minX=listSheetFieldDefsSelected.Min(d => d.BoundsF.Left);
			float maxX=listSheetFieldDefsSelected.Max(d => d.BoundsF.Right);
			int avgX=(int)((minX+maxX)/2);
			listSheetFieldDefsSelected.ForEach(field => MoveSheetFieldDefs(field,avgX-field.Width/2,field.YPos));
			AddUndoLevel("Align Center");
			panelMain.Invalidate();
		}

		private void butAlignLeft_Click(object sender,EventArgs e) {
			if(listBoxFields.SelectedIndices.Count<2) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefsSelected=new List<SheetFieldDef>();
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				listSheetFieldDefsSelected.Add(listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]]);
			}
			if(listSheetFieldDefsSelected.Exists(f1 => listSheetFieldDefsSelected.Exists(f2 => f1.YPos==f2.YPos && f1!=f2))) {
				MsgBox.Show(this,"Cannot align controls. Two or more selected controls will overlap.");
				return;
			}
			int minX=(int)listSheetFieldDefsSelected.Min(d => d.BoundsF.Left);
			listSheetFieldDefsSelected.ForEach(field => MoveSheetFieldDefs(field,minX,field.YPos));
			AddUndoLevel("Align Left");
			panelMain.Invalidate();
		}

		private void butAlignRight_Click(object sender,EventArgs e) {
			if(listBoxFields.SelectedIndices.Count<2) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefsSelected=new List<SheetFieldDef>();
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				listSheetFieldDefsSelected.Add(listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]]);
			}
			if(listSheetFieldDefsSelected.Exists(f1 => listSheetFieldDefsSelected.Exists(f2 => f1.YPos==f2.YPos && f1!=f2))) {
				MsgBox.Show(this,"Cannot align controls. Two or more selected controls will overlap.");
				return;
			}
			int maxX=(int)listSheetFieldDefsSelected.Max(d => d.BoundsF.Right);
			listSheetFieldDefsSelected.ForEach(field => MoveSheetFieldDefs(field,maxX-field.Width,field.YPos));
			AddUndoLevel("Align Right");
			panelMain.Invalidate();
		}

		/// <summary>When clicked it will set all selected elements' Y coordinates to the smallest Y coordinate in the group, unless two controls have the same X coordinate.</summary>
		private void butAlignTop_Click(object sender,EventArgs e) {
			if(listBoxFields.SelectedIndices.Count<2) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefsSelected=new List<SheetFieldDef>();
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				listSheetFieldDefsSelected.Add(listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]]);
			}
			if(listSheetFieldDefsSelected.Exists(f1 => listSheetFieldDefsSelected.Exists(f2 => f1.XPos==f2.XPos && f1!=f2))) {
				MsgBox.Show(this,"Cannot align controls. Two or more selected controls will overlap.");
				return;
			}
			int minY=(int)listSheetFieldDefsSelected.Min(d => d.BoundsF.Top);
			listSheetFieldDefsSelected.ForEach(field => MoveSheetFieldDefs(field,field.XPos,minY));
			AddUndoLevel("Align Tops");
			panelMain.Invalidate();
		}

		private void butAutoSnap_Click(object sender,EventArgs e) {
			string prompt="Please enter auto snap column width between 10 and 100, or 0 to disable.";
			List<InputBoxParam> listInputBoxParam=new List<InputBoxParam> { 
				new InputBoxParam(InputBoxType.TextBox,prompt,_gridSnapDistance.ToString(),Size.Empty) 
			};
			Func<string,bool> funcOnOkClick=new Func<string, bool>((inputVal)=> { 
				int val=PIn.Int(inputVal,false);
				if(val!=0 && !val.Between(10,100)) {
					MsgBox.Show("Please enter a value between 10 and 100, or 0 to disable.");
					return false;
				}
				return true;
			});
			using InputBox inputBox=new InputBox(listInputBoxParam,funcOnOkClick);
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_listSnapXVals.Clear();
			_listSnapYVals.Clear();
			_gridSnapDistance=PIn.Int(inputBox.textResult.Text);
			panelMain.Invalidate();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butCopy_Click(object sender,EventArgs e) {
			CopyControlsToMemory();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_isTabMode) {
				return;
			}
			if(_sheetDef.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(GetIsDynamicSheetType() && _sheetDef.SheetDefNum==PrefC.GetLong(PrefName.SheetsDefaultChartModule)) {
				MsgBox.Show(this,"This is the current Chart module default layout.\r\nPlease select a new default in Sheet Def Defaults first.");
				return;
			}
			if(!GetSelectedLanguageThreeLetters().IsNullOrEmpty()){//User clicked 'Delete' while viewing a translated view.
				string msg=$"You are about to delete the entire {GetSelectedLanguageDisplay()} language translation for this sheet.\n"
					+"You will only be able to un-do this by clicking Cancel in the Edit Sheet window before clicking OK.\n"
					+"Continue?";
				if(MsgBox.Show(MsgBoxButtons.YesNo,msg)){
					_sheetDef.SheetFieldDefs.RemoveAll(x => x.Language==GetSelectedLanguageThreeLetters());//Remove all translated SheetFieldDefs for current language.
					RefreshLanguagesAndPanelMain();
				}
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire sheet?")) {
				return;
			}
			try {
				SheetDefs.DeleteObject(_sheetDef.SheetDefNum);
				SecurityLogs.MakeLogEntry(Permissions.Setup,0,Lan.g(this,"SheetDef")+" "+_sheetDef.Description+" "+Lan.g(this,"was deleted."));
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butEdit_Click(object sender,EventArgs e) {
			using FormSheetDef formSheetDef=new FormSheetDef();
			formSheetDef.SheetDefCur=_sheetDef;
			if(this.IsInternal) {
				formSheetDef.IsReadOnly=true;
			}
			formSheetDef.ShowDialog();
			if(formSheetDef.DialogResult!=DialogResult.OK) {
				return;
			}
			textDescription.Text=_sheetDef.Description;
			SetPanelMainSize();
			FillFieldList();
			panelMain.Invalidate();
			if(_sheetDef.HasMobileLayout) { //Always open the mobile editor since they want to use the mobile layout.
				ShowMobile();
			}
			AddUndoLevel("Edit Properties");
		}

		///<summary>Change the show/hide state of the mobile designer. Open if it is currently closed or close if it is currently open.</summary>
		private void butMobile_Click(object sender,EventArgs e) {
			if(_sheetEditMobileCtrl.IsFormOpen()){
				HideMobile();
			}
			else{
				ShowMobile();
			}
		}

		private void ShowMobile(){
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
			int widthBoth=Width+_sheetEditMobileCtrl.Width;
			Rectangle rectangleBoth=new Rectangle(
				x:rectangleWorkingArea.Left+rectangleWorkingArea.Width/2-widthBoth/2,
				y:rectangleWorkingArea.Top,
				width:widthBoth,
				height:rectangleWorkingArea.Height);
			Bounds=new Rectangle(rectangleBoth.Left,rectangleBoth.Top,Width,rectangleBoth.Height);
			Rectangle rectangleFloat=new Rectangle(Right,rectangleWorkingArea.Top,_sheetEditMobileCtrl.Width,rectangleWorkingArea.Height);
			_sheetEditMobileCtrl.ShowModeless(_sheetDef.Description,this,rectangleFloat);
			butMobile.Text=Lan.g(this,"Hide Mobile");				
		}

		private void HideMobile(){
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
			Bounds=new Rectangle(rectangleWorkingArea.Left+rectangleWorkingArea.Width/2-Width/2,rectangleWorkingArea.Top,Width,rectangleWorkingArea.Height);
			_sheetEditMobileCtrl.HideModeless();
			butMobile.Text=Lan.g(this,"Show Mobile");
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!VerifyDesign()) {
				return;
			}
			if(!_sheetEditMobileCtrl.MergeMobileSheetFieldDefs(_sheetDef,true,new Action<string>((err) => { MsgBox.Show(this,err); }))) {
				return;
			}
			if(!UpdateWebSheetDef()) {
				return;
			}
			//If we added or subtracted any field defs, increment the revision ID
			SheetDef sheetDefStored=SheetDefs.GetSheetDef(_sheetDef.SheetDefNum,false);
			if(sheetDefStored!=null && sheetDefStored.SheetFieldDefs.Count!=_sheetDef.SheetFieldDefs.Count) {
				_sheetDef.RevID++;
			}
			if(!PromptUpdateEClipboardSheetDefs()) {
				return;
			}
			SheetDefs.InsertOrUpdate(_sheetDef);
			DialogResult=DialogResult.OK;
		}

		private void butPageAdd_Click(object sender,EventArgs e) {
			if(_sheetDef.PageCount>9) {
				MsgBox.Show(this,"More than 10 pages are not allowed.");
				//Maximum PageCount 10, this is an arbitrary number. If this number gets too big there may be issues with the graphics trying to draw the sheet.
				return;
			}
			_sheetDef.PageCount++;
			panelMain.Height=_sheetDef.HeightTotal;
			if(_sheetDef.PageCount>1){
				panelMain.Height-=_margins.Bottom;//first page
				panelMain.Height-=(_sheetDef.PageCount-1)*(_margins.Top+_margins.Bottom);//remaining pages
			}
			RefreshBitmapBackground();
			AddUndoLevel("Page Add");
			panelMain.Invalidate();
		}

		private void butPageRemove_Click(object sender,EventArgs e) {
			if(_sheetDef.PageCount<2) {
				MsgBox.Show(this,"Already at one page minimum.");
				//SheetDefCur.IsMultiPage=false;
				//Minimum PageCount 1
				return;
			}
			_sheetDef.PageCount--;
			//Once sheet defs are enhanced to allow showing users editable margins we can go back to using the SheetDefCur.HeightTotal property.
			//For now we need to use the algorithm that Ryan provided me.  See task #743211 for more information.
			int lowestYPos=SheetUtil.GetLowestYPos(_sheetDef.SheetFieldDefs);
			int arbitraryHeight=lowestYPos-_margins.Bottom;//-60;
			int onePageHeight=_sheetDef.Height;
			if(_sheetDef.IsLandscape){
				onePageHeight=_sheetDef.Width;
			}
			int minimumPageCount=(int)Math.Ceiling((double)arbitraryHeight / (onePageHeight-_margins.Top-_margins.Bottom));
			if(minimumPageCount > _sheetDef.PageCount) { //There are fields that have a YPos and heights that push them past the bottom of the page.
				MsgBox.Show(this,"Cannot remove pages that contain sheet fields.");
				_sheetDef.PageCount++;//Forcefully add a page back.
				return;
			}
			panelMain.Height=LayoutManager.Scale(_sheetDef.HeightTotal);
			if(_sheetDef.PageCount>1){
				panelMain.Height-=_margins.Bottom;//-60 first page
				panelMain.Height-=(_sheetDef.PageCount-1)*(_margins.Top+_margins.Bottom);//-100 remaining pages
			}
			RefreshBitmapBackground();
			AddUndoLevel("Page Remove");
			panelMain.Invalidate();
		}

		private void butPaste_Click(object sender,EventArgs e) {
			PasteControlsFromMemory(new Point(0,0));
		}	

		private void butRedo_Click(object sender,EventArgs e) {
			if(_undoLevel==0){
				MsgBox.Show(this,"No more redo levels available.");
				return;
			}
			int idxTarget=_listUndoLevels.Count-_undoLevel;
			//example count=5, _undoLevel=1, so target is last item, or 5-1=4
			//example count=5, _undoLevel=2, so target is second from end, or 5-2=3
			//_undoLevel=3, target is third from end, or 5-3=2
			_sheetDef=_listUndoLevels[idxTarget].SheetDef_.Copy();
			_sheetDef.SheetFieldDefs=DeepCopy(_listUndoLevels[idxTarget].ListSheetFieldDefs);
			_undoLevel--;
			FillFieldList();
			textDescription.Text=_sheetDef.Description;
			SetPanelMainSize();
			panelMain.Invalidate();
		}

		private void butTabOrder_Click(object sender,EventArgs e) {
			_isTabMode=!_isTabMode;
			if(_isTabMode) {
				butOK.Enabled=false;
				butCancel.Enabled=false;
				butDelete.Enabled=false;
				groupAddField.Enabled=false;
				groupShowField.Enabled=false;
				butCopy.Enabled=false;
				butPaste.Enabled=false;
				butUndo.Enabled=false;
				butRedo.Enabled=false;
				groupAlignH.Enabled=false;
				butAlignTop.Enabled=false;
				butEdit.Enabled=false;
				butAutoSnap.Enabled=false;
				groupBoxSubViews.Enabled=false;
				_listSheetFieldDefsTabOrder=new List<SheetFieldDef>(); //clear or create the list of tab orders.
			}
			else {
				butOK.Enabled=true;
				butCancel.Enabled=true;
				butDelete.Enabled=true;
				groupAddField.Enabled=true;
				groupShowField.Enabled=true;
				butCopy.Enabled=true;
				butPaste.Enabled=true;
				butUndo.Enabled=true;
				butRedo.Enabled=true;
				groupAlignH.Enabled=true;
				butAlignTop.Enabled=true;
				butEdit.Enabled=true;
				butAutoSnap.Enabled=true;
				groupBoxSubViews.Enabled=true;
			}
			panelMain.Invalidate();
		}

		private void butUndo_Click(object sender,EventArgs e) {
			if(_listUndoLevels.Count<_undoLevel+2){
				//Example count=5, so we have levels 0-4 available, starting at the end.
				//If we are currently at level 4, then we cannot undo another level.
				//5<4+2, so return.
				MsgBox.Show(this,"No more undo levels available.");
				return;
			}
			List<long> listSheetFieldDefNumsSelected=listBoxFields.GetListSelected<SheetFieldDef>().Select(x=>x.SheetFieldDefNum).ToList();
			int idxTarget=_listUndoLevels.Count-2-_undoLevel;
			//example count=5, _undoLevel=0, so target is second from end, or 5-2-0=3
			//example count=5, _undoLevel=1, so target is third from end, or 5-2-1=2
			_sheetDef=_listUndoLevels[idxTarget].SheetDef_.Copy();
			_sheetDef.SheetFieldDefs=DeepCopy(_listUndoLevels[idxTarget].ListSheetFieldDefs);
			//but we leave it on the list in case they want to redo
			_undoLevel++;
			FillFieldList();
			for(int i=0;i<listBoxFields.Items.Count;i++){
				//they are copies, so we must compare PKs
				SheetFieldDef sheetFieldDef=(SheetFieldDef)listBoxFields.Items.GetObjectAt(i);
				if(listSheetFieldDefNumsSelected.Contains(sheetFieldDef.SheetFieldDefNum)){
					listBoxFields.SetSelected(i);
				}
			}
			textDescription.Text=_sheetDef.Description;
			SetPanelMainSize();
			panelMain.Invalidate();
		}

		private void checkBlue_Click(object sender,EventArgs e) {
			panelMain.Invalidate();
		}

		private void checkSynchMatchedFields_CheckedChanged(object sender,EventArgs e) {
			_sheetEditMobileCtrl.SyncSheetFieldsWithDefualt=checkSynchMatchedFields.Checked;
		}

		private void comboLanguages_SelectionChangeCommitted(object sender,EventArgs e) {
			string selectedThreeLetterLanguage=GetSelectedLanguageThreeLetters();
			bool wasTranslationAdded=false;
			if(selectedThreeLetterLanguage==null) {//Null when 'Add New' clicked.
				if(!TryAddTranslation(out selectedThreeLetterLanguage)) {//Open prompt to select a new language selection, returns false if user cancels.
					comboLanguages.SelectedIndex=1;//'Default' if user cancels out.
					return;
				}
				wasTranslationAdded=true;
			}
			List<SheetFieldDef> listSheetFieldDefsTranslatedFields=_sheetDef.SheetFieldDefs.FindAll(x => x.Language==selectedThreeLetterLanguage);
			if(listSheetFieldDefsTranslatedFields.IsNullOrEmpty()) {//New translation
				//Create in memory duplicates for every 'Default' SheetFieldDef for the new language.
				_sheetDef.SheetFieldDefs.FindAll(x => x.Language.IsNullOrEmpty())//'Default' aka non-translated SheetFieldDefs
					.ForEach(x => AddNewSheetFieldDefsForTranslations(x,selectedThreeLetterLanguage));
			}
			else {
				//Previously translated
			}
			RefreshLanguagesAndPanelMain(wasTranslationAdded,selectedThreeLetterLanguage);
		}

		private void comboUndo_SelectionChangeCommitted(object sender,EventArgs e) {

		}

		private void FormSheetDefEdit_FormClosing(object sender,FormClosingEventArgs e) {
			foreach(SheetFieldDef sheetFieldDef in _sheetDef.SheetFieldDefs.Where(x => x.FieldType==SheetFieldType.Image)) {
				sheetFieldDef.ImageField?.Dispose();
			}
		}

		private void FormSheetDefEdit_KeyDown(object sender,KeyEventArgs e) {
			List<SheetFieldDef> listSheetFieldDefs=listBoxFields.GetListSelected<SheetFieldDef>();
			bool doRefreshBuffer=false;
			e.Handled=true;
			if(e.KeyCode==Keys.ControlKey && _isCtrlDown) {
				return;//I think the Control Key fires repeatedly while held down
			}
			if(IsInternal) {
				return;
			}
			if(e.Control) {
				_isCtrlDown=true;//and continue below
			}
			if(_isCtrlDown && e.KeyCode==Keys.C) { //CTRL-C
				CopyControlsToMemory();
				return;
			}
			if(_isCtrlDown && e.KeyCode==Keys.V) { //CTRL-V
				PasteControlsFromMemory(new Point(0,0));//this does panel.Invalidate, etc.
				return;
			}
			if(_isCtrlDown && e.KeyCode==Keys.Z) { //CTRL-Z
				butUndo_Click(this,new EventArgs());
				return;
			}
			if(_isCtrlDown && e.KeyCode==Keys.Y) { //CTRL-Y
				butRedo_Click(this,new EventArgs());
				return;
			}
			if(e.Alt) {
				Cursor=Cursors.Cross; //rubber stamp cursor
				_isAltDown=true;
				return;
			}
			if(e.KeyCode==Keys.Delete || e.KeyCode==Keys.Back) {
				if(listBoxFields.SelectedIndices.Count==0) {
					return;
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete selected fields?")) {
					return;
				}
				for(int i=listSheetFieldDefs.Count-1;i>=0;i--) { //iterate backward through list
					SheetFieldDef sheetFieldDef=listSheetFieldDefs[i];
					if(sheetFieldDef.FieldType==SheetFieldType.Image) {
						doRefreshBuffer=true;
					}
					if(sheetFieldDef.FieldType==SheetFieldType.Grid && sheetFieldDef.FieldName=="TreatPlanMain"
						&& _sheetDef.SheetFieldDefs.FindAll(x=>x.FieldType==SheetFieldType.Grid && x.FieldName=="TreatPlanMain").Count==1) 
					{
						MsgBox.Show(this,"Cannot delete the last main grid from treatment plan.");
						continue;//skip this one.
					}
					_sheetDef.SheetFieldDefs.Remove(sheetFieldDef);
				}
				AddUndoLevel("Delete");
				FillFieldList();
				panelMain.Invalidate();
				return;
			}
			if(e.KeyCode.In(Keys.Up,Keys.Down,Keys.Left,Keys.Right)){
				foreach(SheetFieldDef sheetFieldDef in listSheetFieldDefs) {
					int newX=sheetFieldDef.XPos;
					int newY=sheetFieldDef.YPos;
					if(sheetFieldDef.FieldType==SheetFieldType.Image) {
						doRefreshBuffer=true;
					}
					switch(e.KeyCode) {
						case Keys.Up:
							if(e.Shift) {
								newY-=7;
							}
							else {
								newY--;
							}
							break;
						case Keys.Down:
							if(e.Shift) {
								newY+=7;
							}
							else {
								newY++;
							}
							break;
						case Keys.Left:
							if(e.Shift) {
								newX-=7;
							}
							else {
								newX--;
							}
							break;
						case Keys.Right:
							if(e.Shift) {
								newX+=7;
							}
							else {
								newX++;
							}
							break;
						default:
							break;
					}
					AddUndoLevel("Move arrow key");
					MoveSheetFieldDefs(sheetFieldDef,newX,newY);
				}
				panelMain.Invalidate();
			}
		}

		private void FormSheetDefEdit_KeyUp(object sender,KeyEventArgs e) {
			if((e.KeyCode&Keys.ControlKey)==Keys.ControlKey) {
				_isCtrlDown=false;
			}
			if(!e.Alt) {
				Cursor=Cursors.Default;
				_isAltDown=false;
			}
		}

		private void FormSheetDefEdit_Load(object sender,EventArgs e) {
			if(_sheetDef.IsLandscape){
				Width=LayoutManager.Scale(_sheetDef.Height+315);
				Height=LayoutManager.Scale(_sheetDef.Width+65);
			}
			else{
				Width=LayoutManager.Scale(_sheetDef.Width+315);
				Height=LayoutManager.Scale(_sheetDef.Height+65);
			}
			if(Width<LayoutManager.Scale(600)){
				Width=LayoutManager.Scale(600);
			}
			if(Height<LayoutManager.Scale(750)){
				Height=LayoutManager.Scale(750);
			}
			System.Windows.Forms.Screen screen=System.Windows.Forms.Screen.FromHandle(this.Handle);
			if(Width>screen.WorkingArea.Width){
				Width=screen.WorkingArea.Width-4;
			}
			if(Height>screen.WorkingArea.Height){
				Height=screen.WorkingArea.Height-4;
			}
			base.CenterFormOnMonitor();
			_imageToothChart=null;
			if(IsInternal) {
				butDelete.Visible=false;
				butOK.Visible=false;
				butCancel.Text=Lan.g(this,"Close");
				groupAddField.Visible=false;
				groupShowField.Visible=false;
				groupPage.Visible=false;
				groupAlignH.Visible=false;
				butAlignTop.Visible=false;
				linkLabelTips.Visible=false;
				butCopy.Visible=false;
				butPaste.Visible=false;
				butUndo.Visible=false;
				butRedo.Visible=false;
				butTabOrder.Visible=false;
			}
			else {
				labelInternal.Visible=false;
			}
			List<SheetFieldType> listSheetFieldTypes=SheetDefs.GetVisibleButtons(_sheetDef.SheetType);
			butOutputText.Visible=listSheetFieldTypes.Contains(SheetFieldType.OutputText);
			butInputField.Visible=listSheetFieldTypes.Contains(SheetFieldType.InputField);
			butStaticText.Visible=listSheetFieldTypes.Contains(SheetFieldType.StaticText);
			butCheckBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.CheckBox);
			butComboBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.ComboBox);
			butImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.Image);
			butPatImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.PatImage);
			butLine.Visible=listSheetFieldTypes.Contains(SheetFieldType.Line);
			butRectangle.Visible=listSheetFieldTypes.Contains(SheetFieldType.Rectangle);
			butSigBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.SigBox);
			butSigBoxPractice.Visible=listSheetFieldTypes.Contains(SheetFieldType.SigBoxPractice);
			butSpecial.Visible=listSheetFieldTypes.Contains(SheetFieldType.Special);
			butGrid.Visible=listSheetFieldTypes.Contains(SheetFieldType.Grid);
			butScreenChart.Visible=listSheetFieldTypes.Contains(SheetFieldType.ScreenChart);
			labelMobileHeader.Visible=listSheetFieldTypes.Contains(SheetFieldType.MobileHeader);
			checkShowOutputText.Visible=listSheetFieldTypes.Contains(SheetFieldType.OutputText);
			checkShowInputField.Visible=listSheetFieldTypes.Contains(SheetFieldType.InputField);
			checkShowStaticText.Visible=listSheetFieldTypes.Contains(SheetFieldType.StaticText);
			checkShowCheckBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.CheckBox);
			checkShowComboBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.ComboBox);
			checkShowImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.Image);
			checkShowPatImage.Visible=listSheetFieldTypes.Contains(SheetFieldType.PatImage);
			checkShowLine.Visible=listSheetFieldTypes.Contains(SheetFieldType.Line);
			checkShowRectangle.Visible=listSheetFieldTypes.Contains(SheetFieldType.Rectangle);
			checkShowSigBox.Visible=listSheetFieldTypes.Contains(SheetFieldType.SigBox);
			checkShowSigBoxPractice.Visible=listSheetFieldTypes.Contains(SheetFieldType.SigBoxPractice);
			checkShowSpecial.Visible=listSheetFieldTypes.Contains(SheetFieldType.Special);
			checkShowGrid.Visible=listSheetFieldTypes.Contains(SheetFieldType.Grid);
			checkShowScreenChart.Visible=listSheetFieldTypes.Contains(SheetFieldType.ScreenChart);
			checkShowMobileHeader.Visible=listSheetFieldTypes.Contains(SheetFieldType.MobileHeader);
			_sheetEditMobileCtrl.IsReadOnly=IsInternal;
			butMobile.Visible=SheetDefs.IsMobileAllowed(_sheetDef.SheetType);
			if(Sheets.SheetTypeIsSinglePage(_sheetDef.SheetType)) {
				groupPage.Visible=false;
			}
			if(_sheetDef.SheetType==SheetTypeEnum.ChartModule) {
				groupPage.Visible=false;
			}
			SetPanelMainSize();
			//panelMain.Height=_sheetDefCur.HeightTotal;
			//if(_sheetDefCur.PageCount>1){
			//	panelMain.Height-=_sheetDefCur.PageCount*100-40;
			//}
			EnableDashboardWidgetOptions();
			textDescription.Text=_sheetDef.Description;
			if(!TryInitLayoutModes()) {//TryInitLayoutModes() must be called before initial FillFieldList().
				//If we are not associated to a SheetType that uses the above layoutmode logic then setup translations UI.
				RefreshLanguages();//Fill list
				InitTranslations();//Enable and update UI
			}
			FillFieldList();
			panelMain.Invalidate();
			panelMain.Focus();
			if(_sheetDef.HasMobileLayout) { //Always open the mobile editor since they want to use the mobile layout.
				ShowMobile();
			}
			ODThread threadGetWebSheetId=new ODThread(GetWebSheetDefs);
			threadGetWebSheetId.AddExceptionHandler(new ODThread.ExceptionDelegate((Exception ex) => {
				//So that the main thread will know the worker thread is done getting the web sheet defs.
				_hasSheetsDownloaded=true;
			}));
			threadGetWebSheetId.AddExitHandler(new ODThread.WorkerDelegate((ODThread o) => {
				//So that the main thread will know the worker thread is done getting the web sheet defs.
				_hasSheetsDownloaded=true;
			}));
			threadGetWebSheetId.Name="GetWebSheetIdThread";
			threadGetWebSheetId.Start(true);
			this.Text="Sheet Def Edit - Revision "+_sheetDef.RevID.ToString();
			_sheetEditMobileCtrl.SyncSheetFieldsWithDefualt=checkSynchMatchedFields.Checked;
			//textDescription.Focus();
			AddUndoLevel("Base");
		}

		private void FormSheetDefEdit_Shown(object sender, EventArgs e){
			if(_sheetDef.SheetType.In(SheetTypeEnum.TreatmentPlan,SheetTypeEnum.ReferralLetter)){
				SetToothChartImage();
			}
			if(_sheetDef.SheetType==SheetTypeEnum.ChartModule) {
				SetToothChartImage();
			}
			if(_sheetDef.SheetType==SheetTypeEnum.PatientDashboardWidget) {
				SetToothChartImage();
			}
		}

		private void FormSheetDefEdit_Resize(object sender,EventArgs e) {
			//int widthRight=LayoutManager.Scale(168);
			//if(ClientSize.Width-widthRight<1) {//SplitterDistance can never be less than 1.
			//	splitContainer1.SplitterDistance=1;
			//}
			//else {
			//	splitContainer1.SplitterDistance=ClientSize.Width-widthRight;
			//}
			SetPanelMainSize();
		}

		private void FormSheetDefEdit_ResizeEnd(object sender,EventArgs e) {
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
			if(Height>rectangleWorkingArea.Height) {
				//Because we are adjusting this form's bounds manually, we need to save the RestoreBounds information.  This is normally stored after FormODBase adjusts the window after maximizing/restore, however the RestoreBounds information is overrided when we shift the bounds a second time here.
				_rectangleRestoreBoundsOld=RestoreBounds;
				Bounds=new Rectangle(Left,rectangleWorkingArea.Top,Width,rectangleWorkingArea.Height);
				_hasWindowBoundsChanged=true;//Bool to keep track of the fact that we have adjusted the window's bounds a second time
			}
			if(WindowState==FormWindowState.Normal && _hasWindowBoundsChanged) {
				//Restore window to it's original size, also need to revert the adjustment FormODBase does to the height and width via FormODBase.ShrinkWindowBeforeMinMax()
				Bounds=new Rectangle(new Point(_rectangleRestoreBoundsOld.Location.X,_rectangleRestoreBoundsOld.Location.Y),
					new Size(_rectangleRestoreBoundsOld.Width+16,_rectangleRestoreBoundsOld.Height+39));
				_hasWindowBoundsChanged=false;
			}
			SetPanelMainSize();
		}

		private void linkLabelTips_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			if(_isTabMode) {
				return;
			}
			string tips="";
			tips+="The following additional shortcuts and hotkeys are supported:\r\n";
			tips+="\r\n";
			//tips+="CTRL + C : Copy selected field(s).\r\n";
			//tips+="\r\n";
			//tips+="CTRL + V : Paste.\r\n";
			//tips+="\r\n";
			tips+="ALT + Click : 'Rubber stamp' paste to the cursor position.\r\n";
			tips+="\r\n";
			tips+="Click + Drag : Click on a blank space and then drag to group select.\r\n";
			tips+="\r\n";
			tips+="CTRL + Click + Drag : Add a group of fields to the selection.\r\n";
			tips+="\r\n";
			tips+="Delete or Backspace : Delete selected field(s).\r\n";
			MessageBox.Show(Lan.g(this,tips));
		}

		private void listFields_MouseDoubleClick(object sender,MouseEventArgs e) {
			int idx=listBoxFields.IndexFromPoint(e.Location);
			if(idx==-1) {
				return;
			}
			listBoxFields.SelectedIndices.Clear();
			listBoxFields.SetSelected(idx,true);
			panelMain.Invalidate();
			//ListBox is multi-extended so we have to use GetListSelected. Because this is double-click, user should only be able to select a single item anyways, so retrieve first item in list.
			List<SheetFieldDef> listSheetFieldDefs=listBoxFields.GetListSelected<SheetFieldDef>();
			if(listSheetFieldDefs.Count==0) {
				return;
			}
			SheetFieldDef sheetFieldDef=listSheetFieldDefs[0];
			SheetFieldDef SheetFieldDefOld=sheetFieldDef.Copy();
			LaunchEditWindow(sheetFieldDef,isEditMobile:false);
			if(sheetFieldDef.TabOrder!=SheetFieldDefOld.TabOrder) { //otherwise a different control will be selected.
				listBoxFields.SelectedIndices.Clear();
			}
		}
		
		private void listFields_SelectionChangeCommitted(object sender,EventArgs e) {
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			_sheetEditMobileCtrl.SetHighlightedFieldDefs(
				listSheetFieldDefsShowing
				.Select((x,y) => new { sheetFieldDef = x,index = y })
				.Where(x => listBoxFields.SelectedIndices.Contains(x.index))
				.Select(x => x.sheetFieldDef.SheetFieldDefNum).ToList());
			panelMain.Invalidate();
		}

		private void panelMain_MouseDown(object sender,MouseEventArgs e) {
			//splitContainer1.Panel1.Select();
			if(_isAltDown) {
				PasteControlsFromMemory(e.Location);
				return;
			}
			_isMouseDown=true;
			_hasClickedOnBlankSpace=false;
			_pointMouseOriginalPos=e.Location;
			counterTemp=0;
			_pointMouseCurrentPos=e.Location;
			SheetFieldDef sheetFieldDef=HitTest(e.X,e.Y);
			if(_isTabMode) {
				_isMouseDown=false;
				_isCtrlDown=false;
				_isAltDown=false;
				if(sheetFieldDef==null
					//Some of the fields below are redundant and should never be returned from HitTest but are here to explicity exclude them.
				   || sheetFieldDef.FieldType==SheetFieldType.Drawing || sheetFieldDef.FieldType==SheetFieldType.Image || sheetFieldDef.FieldType==SheetFieldType.Line || sheetFieldDef.FieldType==SheetFieldType.OutputText || sheetFieldDef.FieldType==SheetFieldType.Parameter || sheetFieldDef.FieldType==SheetFieldType.PatImage || sheetFieldDef.FieldType==SheetFieldType.Rectangle || sheetFieldDef.FieldType==SheetFieldType.StaticText) {
					return;
				}
				if(_listSheetFieldDefsTabOrder.Contains(sheetFieldDef)) {
					sheetFieldDef.TabOrder=0;
					_listSheetFieldDefsTabOrder.RemoveAt(_listSheetFieldDefsTabOrder.IndexOf(sheetFieldDef));
				}
				else {
					_listSheetFieldDefsTabOrder.Add(sheetFieldDef);
				}
				RenumberTabOrderHelper();
				return;
			}
			_sheetFieldDefResizing=HitTestResize(e.X,e.Y);//this click could be outside the field rect because the resize handle is.
			if(_sheetFieldDefResizing!=null){
				sheetFieldDef=_sheetFieldDefResizing;
				_isResizing=true;
				_sizeResizeOriginal=new Size(_sheetFieldDefResizing.Width,_sheetFieldDefResizing.Height);
			}
			if(sheetFieldDef==null) {
				_hasClickedOnBlankSpace=true;
				if(_isCtrlDown) {
					return; //so that you can add more to the previous selection
				}
				listBoxFields.ClearSelected(); //clear the existing selection
				panelMain.Invalidate();
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			int idx=listSheetFieldDefsShowing.IndexOf(sheetFieldDef);
			if(_isCtrlDown) {
				if(listBoxFields.SelectedIndices.Contains(idx)) {
					listBoxFields.SetSelected(idx,false);
				}
				else {
					listBoxFields.SetSelected(idx);
				}
			}
			else { //Ctrl not down
				if(listBoxFields.SelectedIndices.Contains(idx)) {
					//clicking on the group, probably to start a drag.
				}
				else {
					listBoxFields.SelectedIndices.Clear();
					listBoxFields.SetSelected(idx);
				}
			}
			_listPointsOfOriginalControls=new List<Point>();
			Point point;
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				point=new Point(listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].XPos,listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].YPos);
				_listPointsOfOriginalControls.Add(point);
			}			
			panelMain.Invalidate();
		}

		private void panelMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			if(_isAltDown) {
				return;
			}
			SheetFieldDef sheetFieldDef=HitTest(e.X,e.Y);
			if(sheetFieldDef==null) {
				return;
			}
			//SheetFieldDef fieldold=sheetFieldDef.Copy();
			LaunchEditWindow(sheetFieldDef,isEditMobile:false);
			//if(field.TabOrder!=fieldold.TabOrder) {
			//  listFields.SelectedIndices.Clear();
			//}
			//if(isTabMode) {
			//  if(ListSheetFieldDefsTabOrder.Contains(field)){
			//    ListSheetFieldDefsTabOrder.RemoveAt(ListSheetFieldDefsTabOrder.IndexOf(field));
			//  }
			//  if(field.TabOrder>0 && field.TabOrder<ListSheetFieldDefsTabOrder.Count+1) {
			//    ListSheetFieldDefsTabOrder.Insert(field.TabOrder-1,field);
			//  }
			//  RenumberTabOrderHelper();
			//}
		}

		private void panelMain_MouseMove(object sender,MouseEventArgs e) {
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			if(_isResizing){
				int width=_sizeResizeOriginal.Width+e.X-_pointMouseOriginalPos.X;
				int height=_sizeResizeOriginal.Height+e.Y-_pointMouseOriginalPos.Y;
				if(width<5){
					width=5;
				}
				if(height<5){
					height=5;
				}
				if(_sheetFieldDefResizing.Width==width && _sheetFieldDefResizing.Height==height){
					return;
				}
				ResizeSheetFieldDef(_sheetFieldDefResizing,width,height);
				panelMain.Invalidate();
				return;
			}
			//SheetFieldDef sheetFieldDefHover=HitTest(e.X,e.Y);
			if(_isAltDown){
				Cursor=Cursors.Cross; //rubber stamp cursor
			}
			else if(_isMouseDown && _hasClickedOnBlankSpace){
				//dragging selection rectangle, so no hover or resize
				Cursor=Cursors.Default;
			}
			else if(IsInternal || _isTabMode){
				//no hover or resize
				Cursor=Cursors.Default;
			}
			//else if(sheetFieldDefHover!=null){//we could enhance this to only show hover when selected, but it's too annoying like this
			//	Cursor=Cursors.SizeAll;
			//}
			else if(HitTestResize(e.X,e.Y)!=null){
				Cursor=Cursors.SizeNWSE;
			}
			else{
				Cursor=Cursors.Default;
			}
			if(!_isMouseDown) {
				panelMain.Invalidate();
				return;
			}
			if(IsInternal) {
				return;
			}
			if(_isTabMode) {
				return;
			}
			if(_hasClickedOnBlankSpace) {
				_pointMouseCurrentPos=e.Location;
				panelMain.Invalidate();
				return;
			}
			//dragging
			if(!_isDragging){
				//we wait until they actually start dragging before setting this flag
				_isDragging=true;
			}
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				SheetFieldDef sheetFieldDef=listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]];
				Point pointOriginalLocation=_listPointsOfOriginalControls[i];
				int newX=pointOriginalLocation.X+e.X-_pointMouseOriginalPos.X;
				int newY=pointOriginalLocation.Y+e.Y-_pointMouseOriginalPos.Y;
				//Move equivalent translated fields that have not changed.
				MoveSheetFieldDefs(sheetFieldDef,newX,newY);
			}
			//RefreshBitmapBackground();
			panelMain.Invalidate();
		}

		private void panelMain_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDown=false;
			_listPointsOfOriginalControls=null;
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			if(_hasClickedOnBlankSpace) { //if initial mouse down was not on a control.  ie, if we are dragging to select.
				Rectangle rectangleSelectionBounds=new Rectangle(
					x:Math.Min(_pointMouseOriginalPos.X,_pointMouseCurrentPos.X), 
					y:Math.Min(_pointMouseOriginalPos.Y,_pointMouseCurrentPos.Y), 
					width:Math.Abs(_pointMouseCurrentPos.X-_pointMouseOriginalPos.X), 
					height:Math.Abs(_pointMouseCurrentPos.Y-_pointMouseOriginalPos.Y)); 
				for(int i=0;i<listSheetFieldDefsShowing.Count;i++) {
					SheetFieldDef sheetFieldDef=listSheetFieldDefsShowing[i]; //to speed this process up instead of referencing the array every time.
					if(sheetFieldDef.FieldType==SheetFieldType.Line){
						if(Math.Abs(listSheetFieldDefsShowing[i].Width)<2){
							//vertical line
							Rectangle rectangleTempDefBounds=new Rectangle(sheetFieldDef.Bounds.X-1,sheetFieldDef.Bounds.Y,2,sheetFieldDef.Bounds.Height);
							if(sheetFieldDef.Bounds.Height<0){
								rectangleTempDefBounds=new Rectangle(sheetFieldDef.Bounds.X,sheetFieldDef.Bounds.Y+sheetFieldDef.Bounds.Height,2,-sheetFieldDef.Bounds.Height);
							}
							if(rectangleTempDefBounds.IntersectsWith(rectangleSelectionBounds)) {
								listBoxFields.SetSelected(i,true); 
							}
						}
						if(Math.Abs(listSheetFieldDefsShowing[i].Height)<2){
							//horiz line
							Rectangle rectangleTempDefBounds=new Rectangle(sheetFieldDef.Bounds.X,sheetFieldDef.Bounds.Y-1,sheetFieldDef.Bounds.Width,2);
							if(sheetFieldDef.Bounds.Width<0){
								rectangleTempDefBounds=new Rectangle(sheetFieldDef.Bounds.X+sheetFieldDef.Bounds.Width,sheetFieldDef.Bounds.Y-1,-sheetFieldDef.Bounds.Width,2);
							}
							if(rectangleTempDefBounds.IntersectsWith(rectangleSelectionBounds)) {
								listBoxFields.SetSelected(i,true); 
							}
						}
						//no math for diagonal lines yet
						continue;
					}
					if(sheetFieldDef.FieldType==SheetFieldType.Image || sheetFieldDef.FieldType==SheetFieldType.MobileHeader) {
						//Mobile headers should not be selectable on the main panel. Must use other window.
						continue;
					}
					//If the selection is contained within the "hollow" portion of the rectangle, it shouldn't be selected.
					if(sheetFieldDef.FieldType==SheetFieldType.Rectangle) {
						Rectangle rectangleTempDefBounds=new Rectangle(sheetFieldDef.Bounds.X+4,sheetFieldDef.Bounds.Y+4,sheetFieldDef.Bounds.Width-8,sheetFieldDef.Bounds.Height-8);
						if(rectangleTempDefBounds.Contains(rectangleSelectionBounds)) {
							continue;
						}
					}
					if(sheetFieldDef.BoundsF.IntersectsWith(rectangleSelectionBounds)) {
						listBoxFields.SetSelected(i,true); 
					}
				}
			}
			else if(_gridSnapDistance>0){//Attempt to autosnap field to nearest cell top left corner.
				for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
					SheetFieldDef sheetFieldDef=listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]];
					sheetFieldDef.XPos=_listSnapXVals.LastOrDefault(x => x<=sheetFieldDef.XPos);
					sheetFieldDef.YPos=_listSnapYVals.LastOrDefault(y => y<=sheetFieldDef.YPos);
				}
			}
			if(_isResizing){
				string description="Resize "+listBoxFields.GetStringSelectedItems();//just one item	
				AddUndoLevel(description);
				_isResizing=false;
			}
			if(_isDragging){//done dragging
				string description="Drag ";
				if(listBoxFields.SelectedIndices.Count==1){
					description+=listBoxFields.GetStringSelectedItems();//just one item	
				}
				else{
					description+=listBoxFields.SelectedIndices.Count.ToString()+" items";
				}
				AddUndoLevel(description);
				_isDragging=false;
			}
			_hasClickedOnBlankSpace=false;
			_sheetEditMobileCtrl.SetHighlightedFieldDefs(
				listSheetFieldDefsShowing
				.Select((x,y) => new { sheetFieldDef = x,index = y })
				.Where(x => listBoxFields.SelectedIndices.Contains(x.index))
				.Select(x => x.sheetFieldDef.SheetFieldDefNum).ToList());
			//RefreshBitmapBackground();
			panelMain.Invalidate();
		}

		private void panelMain_Paint(object sender,PaintEventArgs e) {
			using Bitmap bitmapDoubleBuffer=new Bitmap(panelMain.Width,panelMain.Height);
			using Graphics g=Graphics.FromImage(bitmapDoubleBuffer);
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			if(DoRefreshImages()) {
				_listSheetFieldDefsDisplayedImages=listSheetFieldDefsShowing.FindAll(x => x.FieldType==SheetFieldType.Image)
					.Select(x => x.Copy())//Doesn't actually deep copy ImageField, which will be disposed in FormClosing.
					.ToList();
				RefreshBitmapBackground();
			}
			g.DrawImage(_bitmapBackground,0,0);
			if(_gridSnapDistance>0) {
				DrawAutoSnapLines(g);
			}
			DrawFields(_sheetDef,g,onlyDrawImages:false);
			DrawSelectionRectangle(g);
			DrawAlignmentLines(g);
			//Draw pagebreak
			Pen penDashPage=new Pen(Color.Green);
			penDashPage.DashPattern=new float[] {4.0F,3.0F,2.0F,3.0F};
			Pen penDashMargin=new Pen(Color.Green);
			penDashMargin.DashPattern=new float[] {1.0F,5.0F};
			int margins=(_margins.Top+_margins.Bottom);
			for(int i=1;i<_sheetDef.PageCount;i++) {
				//g.DrawLine(pDashMargin,0,i*SheetDefCur.HeightPage-_printMargin.Bottom,SheetDefCur.WidthPage,i*SheetDefCur.HeightPage-_printMargin.Bottom);
				g.DrawLine(penDashPage,0,i*(_sheetDef.HeightPage-margins)+_margins.Top,_sheetDef.WidthPage,i*(_sheetDef.HeightPage-margins)+_margins.Top);
				//g.DrawLine(pDashMargin,0,i*SheetDefCur.HeightPage+_printMargin.Top,SheetDefCur.WidthPage,i*SheetDefCur.HeightPage+_printMargin.Top);
			}
			//End Draw Page Break
			e.Graphics.DrawImage(bitmapDoubleBuffer,0,0);
		}

		private void panelMain_Resize(object sender,EventArgs e) {
			if(_bitmapBackground!=null && panelMain.Size==_bitmapBackground.Size) {
				return;
			}
			if(panelMain.Width<1 || panelMain.Height<1){
				return;
			}
			_graphicsBackground?.Dispose();
			_bitmapBackground?.Dispose();
			_bitmapBackground=new Bitmap(panelMain.Width,panelMain.Height);
			_graphicsBackground=Graphics.FromImage(_bitmapBackground);
			RefreshBitmapBackground();
			panelMain.Invalidate();
		}

		///<summary>Some controls (panels in this case) do not pass key events to the parent (the form in this case) even when the property KeyPreview is set.  Instead, the default key functionality occurs.  An example would be the arrow keys.  By default, arrow keys set focus to the "next" control.  Instead, want all key presses on this form and all of it's child controls to always call the FormSheetDefEdit_KeyDown method.</summary>
		protected override bool ProcessCmdKey(ref Message msg,Keys keyData) {
			FormSheetDefEdit_KeyDown(this,new KeyEventArgs(keyData));
			return true;//This indicates that all keys have been processed.
			//return base.ProcessCmdKey(ref msg,keyData);//We don't need this right now, because no textboxes, for example.
		}

		private void sheetEditMobile_SheetDefSelected(object sender,long sheetFieldDefNum) {
			listBoxFields.ClearSelected();
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			listSheetFieldDefsShowing
				.Select((x,y) => new { sfd = x,index = y,})
				.Where(x => x.sfd.SheetFieldDefNum==sheetFieldDefNum)
				.ForEach(x => listBoxFields.SetSelected(x.index,true));
			_sheetEditMobileCtrl.SetHighlightedFieldDefs(
				listSheetFieldDefsShowing
				.Select((x,y) => new { sheetFieldDef = x,index = y })
				.Where(x => listBoxFields.SelectedIndices.Contains(x.index))
				.Select(x => x.sheetFieldDef.SheetFieldDefNum).ToList());
			panelMain.Invalidate();
		}

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e){
			//LayoutManager.LayoutControlBoundsAndFonts(splitContainer1);
		}
		#endregion Methods - Event Handlers

		#region Methods - Event Handlers - Add Buttons
		private void butOutputText_Click(object sender,EventArgs e) {
			if(SheetFieldsAvailable.GetList(_sheetDef.SheetType,OutInCheck.Out).Count==0) {
				MsgBox.Show(this,"There are no output fields available for this type of sheet.");
				return;
			}
			Font font=new Font(_sheetDef.FontName,_sheetDef.FontSize);
			using FormSheetFieldOutput formSheetFieldOutput=new FormSheetFieldOutput();
			formSheetFieldOutput.IsNew=true;
			formSheetFieldOutput.SheetDefCur=_sheetDef;
			formSheetFieldOutput.SheetFieldDefCur=SheetFieldDef.NewOutput("",_sheetDef.FontSize,_sheetDef.FontName,false,0,0,100,font.Height);
			formSheetFieldOutput.ShowDialog();
			if(formSheetFieldOutput.DialogResult!=DialogResult.OK  || formSheetFieldOutput.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldOutput.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butInputField_Click(object sender,EventArgs e) {
			if(SheetFieldsAvailable.GetList(_sheetDef.SheetType,OutInCheck.In).Count==0) {
				MsgBox.Show(this,"There are no input fields available for this type of sheet.");
				return;
			}
			Font font=new Font(_sheetDef.FontName,_sheetDef.FontSize);
			using FormSheetFieldInput formSheetFieldInput=new FormSheetFieldInput();
			formSheetFieldInput.SheetDefCur=_sheetDef;
			formSheetFieldInput.SheetFieldDefCur=SheetFieldDef.NewInput("",_sheetDef.FontSize,_sheetDef.FontName,false,0,0,100,font.Height);
			formSheetFieldInput.ShowDialog();
			if(formSheetFieldInput.DialogResult!=DialogResult.OK  || formSheetFieldInput.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldInput.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butStaticText_Click(object sender,EventArgs e) {
			Font font=new Font(_sheetDef.FontName,_sheetDef.FontSize);
			using FormSheetFieldStatic formSheetFieldStatic=new FormSheetFieldStatic();
			formSheetFieldStatic.SheetDefCur=_sheetDef;
			formSheetFieldStatic.SheetFieldDefCur=SheetFieldDef.NewStaticText("",_sheetDef.FontSize,_sheetDef.FontName,false,0,0,100,font.Height);
			formSheetFieldStatic.ShowDialog();
			if(formSheetFieldStatic.DialogResult!=DialogResult.OK  || formSheetFieldStatic.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldStatic.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butCheckBox_Click(object sender,EventArgs e) {
			if(SheetFieldsAvailable.GetList(_sheetDef.SheetType,OutInCheck.Check).Count==0) {
				MsgBox.Show(this,"There are no checkbox fields available for this type of sheet.");
				return;
			}
			using FormSheetFieldCheckBox formSheetFieldCheckBox=new FormSheetFieldCheckBox();
			formSheetFieldCheckBox.SheetFieldDefCur=SheetFieldDef.NewCheckBox("",0,0,11,11);
			formSheetFieldCheckBox.SheetFieldDefCur.Language=GetSelectedLanguageThreeLetters();
			formSheetFieldCheckBox.SheetDefCur=_sheetDef;
			formSheetFieldCheckBox.ShowDialog();
			if(formSheetFieldCheckBox.DialogResult!=DialogResult.OK  || formSheetFieldCheckBox.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldCheckBox.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butComboBox_Click(object sender,EventArgs e) {
			using FormSheetFieldComboBox formSheetFieldComboBox=new FormSheetFieldComboBox();
			formSheetFieldComboBox.SheetDefCur=_sheetDef;
			formSheetFieldComboBox.SheetFieldDefCur=SheetFieldDef.NewComboBox("","",0,0);
			formSheetFieldComboBox.ShowDialog();
			if(formSheetFieldComboBox.DialogResult!=DialogResult.OK || formSheetFieldComboBox.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldComboBox.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butImage_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Not allowed because not using AtoZ folder");
				return;
			}
			//Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldImage formSheetFieldImage=new FormSheetFieldImage();
			formSheetFieldImage.SheetDefCur=_sheetDef;
			formSheetFieldImage.SheetFieldDefCur=SheetFieldDef.NewImage("",0,0,100,100);
			formSheetFieldImage.SheetFieldDefCur.Language=GetSelectedLanguageThreeLetters();
			formSheetFieldImage.ShowDialog();
			if(formSheetFieldImage.DialogResult!=DialogResult.OK  || formSheetFieldImage.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldImage.SheetFieldDefCur);
			FillFieldList();
			RefreshBitmapBackground();
			panelMain.Invalidate();
		}

		private void butPatImage_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Not allowed because not using AtoZ folder");
				return;
			}
			//Font font=new Font(SheetDefCur.FontName,SheetDefCur.FontSize);
			using FormSheetFieldPatImage formSheetFieldPatImage=new FormSheetFieldPatImage();
			formSheetFieldPatImage.SheetDefCur=_sheetDef;
			formSheetFieldPatImage.SheetFieldDefCur=SheetFieldDef.NewPatImage(0,0,100,100);
			formSheetFieldPatImage.SheetFieldDefCur.FieldType=SheetFieldType.PatImage;
			formSheetFieldPatImage.ShowDialog();
			if(formSheetFieldPatImage.DialogResult!=DialogResult.OK  || formSheetFieldPatImage.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldPatImage.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butLine_Click(object sender,EventArgs e) {
			using FormSheetFieldLine formSheetFieldLine=new FormSheetFieldLine();
			formSheetFieldLine.SheetDefCur=_sheetDef;
			formSheetFieldLine.SheetFieldDefCur=SheetFieldDef.NewLine(0,0,0,0);
			formSheetFieldLine.ShowDialog();
			if(formSheetFieldLine.DialogResult!=DialogResult.OK  || formSheetFieldLine.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldLine.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butRectangle_Click(object sender,EventArgs e) {
			using FormSheetFieldRect formSheetFieldRect=new FormSheetFieldRect();
			formSheetFieldRect.SheetDefCur=_sheetDef;
			formSheetFieldRect.SheetFieldDefCur=SheetFieldDef.NewRect(0,0,0,0);
			formSheetFieldRect.ShowDialog();
			if(formSheetFieldRect.DialogResult!=DialogResult.OK  || formSheetFieldRect.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldRect.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butSigBox_Click(object sender,EventArgs e) {
			AddSignatureBox(SheetFieldType.SigBox);
		}

		private void butSigBoxPractice_Click(object sender,EventArgs e) {
			AddSignatureBox(SheetFieldType.SigBoxPractice);
		}

		private void AddSignatureBox(SheetFieldType sheetFieldType) {
			using FormSheetFieldSigBox formSheetFieldSigBox=new FormSheetFieldSigBox();
			formSheetFieldSigBox.SheetDefCur=_sheetDef;
			formSheetFieldSigBox.SheetFieldDefCur=SheetFieldDef.NewSigBox(0,0,364,81,sigBox:sheetFieldType);
			formSheetFieldSigBox.ShowDialog();
			if(formSheetFieldSigBox.DialogResult!=DialogResult.OK  || formSheetFieldSigBox.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			AddNewSheetFieldDef(formSheetFieldSigBox.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butSpecial_Click(object sender,EventArgs e) {
			using FormSheetFieldSpecial formSheetFieldSpecial=new FormSheetFieldSpecial();
			formSheetFieldSpecial.SheetDefCur=_sheetDef;
			formSheetFieldSpecial.SheetFieldDefCur=new SheetFieldDef(){IsNew=true };
			formSheetFieldSpecial.LayoutMode=_sheetFieldLayoutMode;
			formSheetFieldSpecial.ShowDialog();
			if(formSheetFieldSpecial.DialogResult!=DialogResult.OK) {
				return;
			}
			bool isDynamicSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(_sheetDef.SheetType).IsDynamic;
			List<SheetFieldDef> listSheetFieldDefsPertinent=_sheetDef.SheetFieldDefs.FindAll(x => x.LayoutMode==_sheetFieldLayoutMode && x.Language==GetSelectedLanguageThreeLetters());
			if(isDynamicSheetType && listSheetFieldDefsPertinent.Any(x => x.FieldName==formSheetFieldSpecial.SheetFieldDefCur.FieldName)) {
				MsgBox.Show(this,"Field already exists.");
				return;
			}
			AddNewSheetFieldDef(formSheetFieldSpecial.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butGrid_Click(object sender,EventArgs e) {
			bool isDynamicSheetType=EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(_sheetDef.SheetType).IsDynamic;
			using FormSheetFieldGrid formSheetFieldGrid=new FormSheetFieldGrid();
			formSheetFieldGrid.SheetDefCur=_sheetDef;
			if(SheetDefs.IsDashboardType(_sheetDef)) {
				//is resized from dialog window.
				formSheetFieldGrid.SheetFieldDefCur=SheetFieldDef.NewGrid(DashApptGrid.SheetFieldName,0,0,100,150,growthBehavior:GrowthBehaviorEnum.None); 
			}
			else {
				using FormSheetFieldGridType formSheetFieldGridType=new FormSheetFieldGridType();
				formSheetFieldGridType.SheetDefCur=_sheetDef;
				formSheetFieldGridType.LayoutMode=_sheetFieldLayoutMode;
				formSheetFieldGridType.ShowDialog();
				if(formSheetFieldGridType.DialogResult!=DialogResult.OK) {
					return;
				}
				formSheetFieldGrid.SheetFieldDefCur=SheetFieldDef.NewGrid(formSheetFieldGridType.SelectedSheetGridType,0,0,100,100); //is resized from dialog window.
				if(isDynamicSheetType) {
					//Grid dimensions in dynamic sheetDefs should be static by default because easiest to understand.
					formSheetFieldGrid.SheetFieldDefCur.GrowthBehavior=GrowthBehaviorEnum.None;
				}
			}
			formSheetFieldGrid.ShowDialog();
			if(formSheetFieldGrid.DialogResult!=DialogResult.OK  || formSheetFieldGrid.SheetFieldDefCur==null) {//SheetFieldDefCur==null if it was Deleted
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsPertinent=_sheetDef.SheetFieldDefs.FindAll(x => x.LayoutMode==_sheetFieldLayoutMode && x.Language==GetSelectedLanguageThreeLetters());
			if(isDynamicSheetType && listSheetFieldDefsPertinent.Any(x => x.FieldName==formSheetFieldGrid.SheetFieldDefCur.FieldName)) {
				MsgBox.Show(this,"Grid already exists.");
				return;
			}
			AddNewSheetFieldDef(formSheetFieldGrid.SheetFieldDefCur);
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butScreenChart_Click(object sender,EventArgs e) {
			string fieldValue="0;d,m,ling;d,m,ling;,,;,,;,,;,,;m,d,ling;m,d,ling;m,d,buc;m,d,buc;,,;,,;,,;,,;d,m,buc;d,m,buc";
			SheetFieldDef sheetFieldDef=null;
			if(!HasChartSealantComplete()) {
				sheetFieldDef=SheetFieldDef.NewScreenChart("ChartSealantComplete",fieldValue,0,0);
			}
			else if(!HasChartSealantTreatment()) {
				sheetFieldDef=SheetFieldDef.NewScreenChart("ChartSealantTreatment",fieldValue,0,0);
			}
			else {
				MsgBox.Show(this,"Only two charts are allowed per screening sheet.");
				return;
			}
			AddNewSheetFieldDef(sheetFieldDef);
			FillFieldList();
			panelMain.Invalidate();
		}

		#endregion Methods - Event Handlers - Add Buttons

		#region Methods - Event Handlers - Show
		private void butShowAll_Click(object sender,EventArgs e) {
			checkShowOutputText.Checked=true;
			checkShowInputField.Checked=true;
			checkShowStaticText.Checked=true;
			checkShowCheckBox.Checked=true;
			checkShowComboBox.Checked=true;
			checkShowImage.Checked=true;
			checkShowPatImage.Checked=true;
			checkShowLine.Checked=true;
			checkShowRectangle.Checked=true;
			checkShowSigBox.Checked=true;
			checkShowSigBoxPractice.Checked=true;
			checkShowSpecial.Checked=true;
			checkShowGrid.Checked=true;
			checkShowScreenChart.Checked=true;
			checkShowMobileHeader.Checked=true;
			FillFieldList();
			panelMain.Invalidate();
		}

		private void butShowNone_Click(object sender,EventArgs e) {
			checkShowOutputText.Checked=false;
			checkShowInputField.Checked=false;
			checkShowStaticText.Checked=false;
			checkShowCheckBox.Checked=false;
			checkShowComboBox.Checked=false;
			checkShowImage.Checked=false;
			checkShowPatImage.Checked=false;
			checkShowLine.Checked=false;
			checkShowRectangle.Checked=false;
			checkShowSigBox.Checked=false;
			checkShowSigBoxPractice.Checked=false;
			checkShowSpecial.Checked=false;
			checkShowGrid.Checked=false;
			checkShowScreenChart.Checked=false;
			checkShowMobileHeader.Checked=false;
			FillFieldList();
			panelMain.Invalidate();
		}

		private void CheckShowChecked(){
			List<SheetFieldDef> listSheetFieldDefsSelected=listBoxFields.GetListSelected<SheetFieldDef>();
			FillFieldList();
			for(int i=0;i<listBoxFields.Items.Count;i++){
				if(listSheetFieldDefsSelected.Contains(listBoxFields.Items.GetObjectAt(i))){
					listBoxFields.SetSelected(i);
				}
			}
			panelMain.Invalidate();
		}

		private void checkShowOutputText_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowInputField_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowStaticText_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowCheckBox_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowComboBox_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowImage_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowPatImage_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowLine_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowRectangle_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowSigBox_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowSigBoxPractice_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowSpecial_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowGrid_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowScreenChart_Click(object sender,EventArgs e) {
			CheckShowChecked();
		}

		private void checkShowMobileHeader_Click(object sender,EventArgs e){
			CheckShowChecked();
		}
		#endregion Methods - Event Handlers - Show

		#region Methods - Private
		///<summary>Use this to add a new item to SheetDefCur.SheetFieldDefs. The new item will be given a random primary key so it can be distinguished from other items.</summary>
		private void AddNewSheetFieldDef(SheetFieldDef sheetFieldDef) {
			sheetFieldDef.LayoutMode=_sheetFieldLayoutMode;
			bool doAddToTranslatedViews=false;
			if(sheetFieldDef.Language.IsNullOrEmpty()){
				//SheetFieldDefs that are added for translations set their Language before calling, do not override.
				sheetFieldDef.Language=GetSelectedLanguageThreeLetters();
			}
			//Only copy given field to translation view when added to 'Default' translation and if Synch matched fields is checked.
			doAddToTranslatedViews=(sheetFieldDef.Language.IsNullOrEmpty() && checkSynchMatchedFields.Checked);
			//This new key is only used for copy and paste function.
			//When this sheet is saved, all sheetfielddefs are deleted and reinserted, so the dummy PKs are harmless.
			//There's a VERY slight chance of PK duplication so try until we find an unused PK.
			int tries=0;
			do {
				sheetFieldDef.SheetFieldDefNum=_random.Next(int.MaxValue);
			} while(_sheetDef.SheetFieldDefs.Any(x => x.SheetFieldDefNum==sheetFieldDef.SheetFieldDefNum) && ++tries<100);
			_sheetDef.SheetFieldDefs.Add(sheetFieldDef);
			if(doAddToTranslatedViews){
				AddNewSheetFieldDefsForTranslations(sheetFieldDef,GetListUsedTranslations().ToArray());
			}
			_sheetEditMobileCtrl.ScrollToSheetFieldDefNum=sheetFieldDef.SheetFieldDefNum;
		}

		///<summary>Creates copies of given defDefault for every given value in listLanguages and inserts them into _sheetDefCur.SheetFieldDefs via AddNewSheeFieldDef(...) Either called when user selects a new language to translate to or when the user adds a SheetFieldDef to the 'Default' view.</summary>
		private void AddNewSheetFieldDefsForTranslations(SheetFieldDef sheetFieldDefDefault,params string[]  arrThreeLetterLanguages){
			foreach(string language in arrThreeLetterLanguages) {
				if(language.IsNullOrEmpty()){//Ignore 'Default' 
					continue;
				}
				int countScreenChartsForLanguage=_sheetDef.SheetFieldDefs.FindAll(x=>x.Language==language && x.FieldType==SheetFieldType.ScreenChart).Count();
				if(sheetFieldDefDefault.FieldType==SheetFieldType.ScreenChart && countScreenChartsForLanguage==2) {
					continue;//only 2 screencharts are allowed per translation
				}
				SheetFieldDef sheetFieldDefCopy=sheetFieldDefDefault.Copy();
				sheetFieldDefCopy.Language=language;
				AddNewSheetFieldDef(sheetFieldDefCopy);
			}
		}

		///<summary>Add an Undo level to the list. This gets called just after the actual change is made to the main list so that the last entry in the list matches current state.</summary>
		private void AddUndoLevel(string description){
			if(_undoLevel>0){
				//This means they did some "undos", and now they are making a normal change.
				//Need to remove the extra items off the end of the list before adding this new one.
				//Example, list is 5 long, _undoLevel=2, so current state is the 3rd from end.
				//Get rid of 2 items which have indices 3 and 4. RemoveRange(5-2,2)or(3,2);
				_listUndoLevels.RemoveRange(_listUndoLevels.Count-_undoLevel,_undoLevel);
				_undoLevel=0;
			}
			UndoLevel undoLevel=new UndoLevel();
			undoLevel.Description=description;
			undoLevel.SheetDef_=_sheetDef.Copy();//does not include list
			undoLevel.ListSheetFieldDefs=DeepCopy(_sheetDef.SheetFieldDefs);		
			_listUndoLevels.Add(undoLevel);
			//FillComboUndo();
			//_undoLevel is always 0 at the end of this method
		}

		private List<SheetFieldDef> DeepCopy(List<SheetFieldDef> listSheetFieldDefs){
			List<SheetFieldDef> listSheetFieldDefsRet=new List<SheetFieldDef>();
			for(int i=0;i<listSheetFieldDefs.Count;i++){
				SheetFieldDef sheetFieldDef=listSheetFieldDefs[i].Copy();//does not copy ImageField
				listSheetFieldDefsRet.Add(sheetFieldDef);
			}
			return listSheetFieldDefsRet;
		}

		///<summary>Moves a SheetFieldDef to a specified position, as well as any matching translated SheetFieldDefs if Sync is enabled. Moves translated fields first.</summary>
		private void MoveSheetFieldDefs(SheetFieldDef sheetFieldDef,int newX,int newY) {
			if(checkSynchMatchedFields.Checked){
				if(HasTranslatedSheetDefS(sheetFieldDef,out List<SheetFieldDef> listSheetFieldDefs)){
					for(int i=0;i<listSheetFieldDefs.Count;i++){
						listSheetFieldDefs[i].XPos=newX;
						listSheetFieldDefs[i].YPos=newY;
					};
				}
			}
			sheetFieldDef.XPos=newX;
			sheetFieldDef.YPos=newY;
		}

		private void ResizeSheetFieldDef(SheetFieldDef sheetFieldDef,int width,int height) {
			if(checkSynchMatchedFields.Checked){
				if(HasTranslatedSheetDefS(sheetFieldDef,out List<SheetFieldDef> listSheetFieldDefs)){
					for(int i=0;i<listSheetFieldDefs.Count;i++){
						listSheetFieldDefs[i].Width=width;
						listSheetFieldDefs[i].Height=height;
					};
				}
			}
			sheetFieldDef.Width=width;
			sheetFieldDef.Height=height;
		}

		private void CopyControlsToMemory() {
			if(_isTabMode) {
				return;
			}
			if(listBoxFields.SelectedIndices.Count==0) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			string strPrompt=Lan.g(this,"The following selected fields can cause conflicts if they are copied:\r\n");
			bool isConflictingfield=false;
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				SheetFieldDef sheetFieldDef=listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]];
				switch(sheetFieldDef.FieldType) {
					case SheetFieldType.Drawing:
					case SheetFieldType.Image:
					case SheetFieldType.Line:
					case SheetFieldType.PatImage:
					//case SheetFieldType.Parameter://would not be seen on the sheet.
					case SheetFieldType.Rectangle:
					case SheetFieldType.SigBox:
					case SheetFieldType.SigBoxPractice:
					case SheetFieldType.StaticText:
					case SheetFieldType.ComboBox:
						break; //it will always be ok to copy the types of fields above.
					case SheetFieldType.CheckBox:
						if(sheetFieldDef.FieldName!="misc") { //custom fields should be okay to copy
							strPrompt+=sheetFieldDef.FieldName+"."+sheetFieldDef.RadioButtonValue+"\r\n";
							isConflictingfield=true;
						}
						break;
					case SheetFieldType.InputField:
					case SheetFieldType.OutputText:
						if(sheetFieldDef.FieldName!="misc") { //custom fields should be okay to copy
							strPrompt+=sheetFieldDef.FieldName+"\r\n";
							isConflictingfield=true;
						}
						break;
				}
			}
			strPrompt+=Lan.g(this,"Would you like to continue anyway?");
			if(isConflictingfield){
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,strPrompt)){
					//This prompt can be too tall, but only solution would be to build our own MessageBox from scratch.
					_isCtrlDown=false;
					return;
				}
			}
			_listSheetFieldDefsCopyPaste=new List<SheetFieldDef>(); //empty the remembered field list
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				_listSheetFieldDefsCopyPaste.Add(listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].Copy()); //fill clipboard with copies of the controls. 
				//It would probably be safe to fill the clipboard with the originals. but it is safer to fill it with copies.
			}
			_pasteOffset=0;
			_pasteOffsetY=0; //reset PasteOffset for pasting a new set of fields.
		}

		private void DrawAutoSnapLines(Graphics g) {
			if(_listSnapXVals.Count==0) {
				int x=0;
				while(x<=panelMain.Width) {
					_listSnapXVals.Add(x);
					x+=_gridSnapDistance;
				}
				int y=0;
				while(y<=panelMain.Height) {
					_listSnapYVals.Add(y);
					y+=_gridSnapDistance;
				}
			}
			using Pen pen=new Pen(ColorOD.Gray(235));//Color.LightGray);//211
			foreach(int x in _listSnapXVals) {//Vertical lines
				g.DrawLine(pen,new Point(x,0),new Point(x,this.Height));
			}
			foreach(int y in _listSnapYVals) {//Horizontal lines
				g.DrawLine(pen,new Point(0,y),new Point(this.Width,y));
			}
		}

		///<summary>If drawImages is true then only image fields will be drawn. Otherwise, all fields but images will be drawn.</summary>
		private void DrawFields(SheetDef sheetDef,Graphics g,bool onlyDrawImages,bool isHighlightEligible=true) {
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.CompositingQuality=CompositingQuality.HighQuality; //This has to be here or the line thicknesses are wrong.
			List<SheetFieldDef> listSheetFieldDefs=GetPertinentSheetFieldDefs(sheetDef);
			string selectedLangauge=GetSelectedLanguageThreeLetters();
			List<SheetFieldDef> listSheetFieldDefsSelected=listBoxFields.GetListSelected<SheetFieldDef>();
			for(int i=0;i<listSheetFieldDefs.Count;i++) {
				SheetFieldDef sheetFieldDef=listSheetFieldDefs[i];
				if(sheetFieldDef.LayoutMode!=_sheetFieldLayoutMode || sheetFieldDef.Language!=selectedLangauge) {
					continue;
				}
				if(!IsTypeShowing(sheetFieldDef.FieldType)){
					continue;
				}
				if(onlyDrawImages) {
					if(sheetFieldDef.FieldType==SheetFieldType.Image) {
						DrawImages(sheetFieldDef,g);
					}
					continue;
				} //end onlyDrawImages
				bool isSelected=listSheetFieldDefsSelected.Contains(sheetFieldDef);
					//listFields.SelectedIndices.Contains(i);
				bool isFieldSelected=(isHighlightEligible && isSelected);
				switch(sheetFieldDef.FieldType) {
					case SheetFieldType.Parameter: //Skip
					case SheetFieldType.MobileHeader: //Skip
					case SheetFieldType.Image: //Handled above
						continue;
					case SheetFieldType.PatImage:
						DrawPatImage(sheetFieldDef,g,isFieldSelected);
						continue;
					case SheetFieldType.Line:
						DrawLine(sheetFieldDef,g,isFieldSelected);
						continue;
					case SheetFieldType.Rectangle:
						DrawRectangle(sheetFieldDef,g,isFieldSelected);
						continue;
					case SheetFieldType.CheckBox:
						DrawCheckBox(sheetFieldDef,g,isFieldSelected);
						continue;
					case SheetFieldType.ComboBox:
						DrawComboBox(sheetFieldDef,g,isFieldSelected);
						DrawTabMode(sheetFieldDef,g,isHighlightEligible);
						continue;
					case SheetFieldType.ScreenChart:
						DrawToothChart(sheetFieldDef,g,isFieldSelected);
						continue;
					case SheetFieldType.SigBox:
					case SheetFieldType.SigBoxPractice:
						DrawSigBox(sheetFieldDef,g,isFieldSelected);
						continue;
					case SheetFieldType.Special:
						DrawSpecial(sheetFieldDef,g,sheetDef.Width,isFieldSelected);
						continue;
					case SheetFieldType.Grid:
						DrawGrid(sheetFieldDef,g,isSelected);
						continue;
					case SheetFieldType.InputField:
					case SheetFieldType.StaticText:
					case SheetFieldType.OutputText:
					default:
						DrawStringField(sheetFieldDef,g,isFieldSelected);
						DrawTabMode(sheetFieldDef,g,isHighlightEligible);
						continue;
				} //end switch
			}
		}

		///<summary>Returns true if _listDisplayedImages is different than _sheetDefCurs image SheetFieldDefs, otherwise false. </summary>
		private bool DoRefreshImages() {
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			//Get the Image fields that will be drawn by DrawFields()->DrawImagesHelper()
			List<SheetFieldDef> listSheetFieldDefs=listSheetFieldDefsShowing.FindAll(x => x.FieldType==SheetFieldType.Image);
			if(listSheetFieldDefs.Select(x => x.SheetFieldDefNum).Except(_listSheetFieldDefsDisplayedImages.Select(x => x.SheetFieldDefNum)).Count()>0) {
				//A field was added.
				return true;
			}
			if(_listSheetFieldDefsDisplayedImages.Select(x=>x.SheetFieldDefNum).Except(listSheetFieldDefs.Select(x=>x.SheetFieldDefNum)).Count()>0) {
				//A field was removed.
				return true;
			}
			foreach(SheetFieldDef sheetFieldDef in listSheetFieldDefs) {//any images altered
				SheetFieldDef sheetFieldDefImgOld=_listSheetFieldDefsDisplayedImages.FirstOrDefault(x => x.SheetFieldDefNum==sheetFieldDef.SheetFieldDefNum);
				if(sheetFieldDefImgOld is null) {
					return true;
				}
				if(!IsEquivilantSheetDef(sheetFieldDef,sheetFieldDefImgOld)) {
					return true;
				}
			}
			return false;
		}

		///<summary>If is PatientDashboardWidget, only certain buttons will show, including DashboardWidget specific buttons.  Otherwise, DashboardWidget specific buttons will be hidden.</summary>
		private void EnableDashboardWidgetOptions() {
			if(_sheetDef.SheetType!=SheetTypeEnum.PatientDashboardWidget){
				return;
			}
			groupPage.Visible=false;
			butAutoSnap.Visible=false;
			butTabOrder.Visible=false;
			butMobile.Visible=false;
		}

		private void FillComboUndo(){
			/*
			comboUndo.Items.Clear();
			for(int i=_listUndoLevels.Count-1;i>=0;i--){//Go backwards to add most recent at top
				//don't show the last item on the list because that just represents current state
				comboUndo.Items.Add(_listUndoLevels[i].Description);
				if(i==20){//only show 20 levels in the dropdown
					break;
				}
			}*/
		}

		private void FillFieldList() {
			listBoxFields.Items.Clear();
			string txt;
			if(GetIsDynamicSheetType()) {
				_sheetDef.SheetFieldDefs=_sheetDef.SheetFieldDefs
					.OrderByDescending(x => x.FieldType==SheetFieldType.Grid)//Grids first
					.ThenBy(x => x.FieldName.Contains("Button")).ToList();//Buttons last, always drawn on top.
			}
			else {
				_sheetDef.SheetFieldDefs.Sort(SheetFieldDefs.CompareTabOrder);
			}
			string selectedLanguage=GetSelectedLanguageThreeLetters();
			foreach(SheetFieldDef sheetFieldDef in _sheetDef.SheetFieldDefs) {
				if(sheetFieldDef.LayoutMode!=_sheetFieldLayoutMode || sheetFieldDef.Language!=selectedLanguage) {//Currently both of these can not be set at the same time.
					continue;
				}
				if(!IsTypeShowing(sheetFieldDef.FieldType)){
					continue;
				}
				switch(sheetFieldDef.FieldType) {
					case SheetFieldType.StaticText:
						txt=sheetFieldDef.FieldValue;
						break;
					case SheetFieldType.Image:
						txt=Lan.g(this,"Image:")+sheetFieldDef.FieldName;
						break;
					case SheetFieldType.PatImage:
						txt=Lan.g(this,"PatImg:")+Defs.GetName(DefCat.ImageCats,PIn.Long(sheetFieldDef.FieldName));
						break;
					case SheetFieldType.Line:
						txt=Lan.g(this,"Line:")+sheetFieldDef.XPos.ToString()+","+sheetFieldDef.YPos.ToString()+","+"W:"+sheetFieldDef.Width.ToString()+","+"H:"+sheetFieldDef.Height.ToString();
						break;
					case SheetFieldType.Rectangle:
						txt=Lan.g(this,"Rect:")+sheetFieldDef.XPos.ToString()+","+sheetFieldDef.YPos.ToString()+","+"W:"+sheetFieldDef.Width.ToString()+","+"H:"+sheetFieldDef.Height.ToString();
						break;
					case SheetFieldType.SigBox:
						txt=Lan.g(this,"Signature Box");
						break;
					case SheetFieldType.SigBoxPractice:
						txt=Lan.g(this,"Practice Signature Box");
						break;
					case SheetFieldType.CheckBox:
						txt=sheetFieldDef.TabOrder.ToString()+": ";
						if(sheetFieldDef.FieldName.StartsWith("allergy:") || sheetFieldDef.FieldName.StartsWith("problem:")) {
							txt+=sheetFieldDef.FieldName.Remove(0,8);
						}
						else {
							txt+=sheetFieldDef.FieldName;
						}
						if(sheetFieldDef.RadioButtonValue!="") {
							if(!sheetFieldDef.UiLabelMobileRadioButton.IsNullOrEmpty()) {
								txt+=" - "+sheetFieldDef.UiLabelMobileRadioButton;
							}
							else {
								txt+=" - "+sheetFieldDef.RadioButtonValue;
							}
						}
						break;
					case SheetFieldType.ComboBox:
						txt=sheetFieldDef.TabOrder > 0 ? sheetFieldDef.TabOrder.ToString()+": " : "";
						txt+=Lan.g(this,"ComboBox:")+sheetFieldDef.XPos.ToString()+","+sheetFieldDef.YPos.ToString()
							+","+"W:"+sheetFieldDef.Width.ToString();
						break;
					case SheetFieldType.InputField:
						txt=sheetFieldDef.TabOrder.ToString()+": "+sheetFieldDef.FieldName;
						break;
					case SheetFieldType.Grid:
						txt="Grid:"+sheetFieldDef.FieldName;
						break;
					case SheetFieldType.MobileHeader:
						txt=Lan.g(this,"Mobile Only:")+" "+sheetFieldDef.UiLabelMobile;
						break;
					default:
						txt=sheetFieldDef.FieldName;
						break;
				} //end switch
				listBoxFields.Items.Add(txt,sheetFieldDef);
			}
			_sheetEditMobileCtrl.UpdateLanguage(selectedLanguage);//This must be called before sheetEditMobile.SheetDef
			_sheetEditMobileCtrl.SheetDef=_sheetDef;
		}

		private bool GetIsDynamicSheetType() {
			return EnumTools.GetAttributeOrDefault<SheetLayoutAttribute>(_sheetDef.SheetType).IsDynamic;
		}

		///<summary>Returns all sheetFieldDefs from sheetDef for the current layout mode, sheetDef defaults to _sheetDefCur.</summary>
		private List<SheetFieldDef> GetPertinentSheetFieldDefs(SheetDef sheetDef=null) {
			if(sheetDef==null) {
				sheetDef=_sheetDef;
			}
			string selectedLanguage=GetSelectedLanguageThreeLetters();
			List<SheetFieldDef> listSheetFieldDefs=new List<SheetFieldDef>();
			for(int i=0;i<sheetDef.SheetFieldDefs.Count;i++){
				if(sheetDef.SheetFieldDefs[i].LayoutMode!=_sheetFieldLayoutMode){
					continue;
				}
				if(sheetDef.SheetFieldDefs[i].Language!=selectedLanguage) {//Currently both of these can not be set at the same time.
					continue;
				}
				if(!IsTypeShowing(sheetDef.SheetFieldDefs[i].FieldType)){
					continue;
				}
				listSheetFieldDefs.Add(sheetDef.SheetFieldDefs[i]);
			}
			return listSheetFieldDefs;
		}

		///<summary>Fills _listWebSheetIds with any webforms_sheetdefs that have the same SheetDefNum of the current SheetDef.</summary>
		private void GetWebSheetDefs(ODThread odThread) {
			//Ignore the certificate errors for the staging machine and development machine
			if(PrefC.GetString(PrefName.WebHostSynchServerURL).In(WebFormL.SynchUrlStaging,WebFormL.SynchUrlDev)) {
				WebFormL.IgnoreCertificateErrors();
			}
			List<WebForms_SheetDef> listWebForms_SheetDefs;
			if(WebForms_SheetDefs.TryDownloadSheetDefs(out listWebForms_SheetDefs)) {
				lock(_lock) {
					_listWebForms_SheetDefs=listWebForms_SheetDefs.Where(x => x.SheetDefNum==_sheetDef.SheetDefNum).ToList();
				}
			}
		}

		private bool HasScreeningChart(bool isTreatmentChart) {
			if(_sheetDef.SheetType!=SheetTypeEnum.Screening) {
				return false;
			}
			List<SheetFieldDef> listSheetFieldDefsPertinent=_sheetDef.SheetFieldDefs.FindAll( x => x.LayoutMode==_sheetFieldLayoutMode && x.Language==GetSelectedLanguageThreeLetters());
			List<SheetFieldDef> listSheetFieldDefs=listSheetFieldDefsPertinent;
			if(listSheetFieldDefs.Count==0) {
				return false;
			}
			string chartName;
			if(isTreatmentChart) {
				chartName="ChartSealantTreatment";
			}
			else {
				chartName="ChartSealantComplete";
			}
			return listSheetFieldDefs.Any(x => x.FieldType==SheetFieldType.ScreenChart && x.FieldName==chartName);
		}

		private bool HasChartSealantComplete() {
			return HasScreeningChart(isTreatmentChart:false);
		}

		private bool HasChartSealantTreatment() {
			return HasScreeningChart(isTreatmentChart:true);
		}

		///<summary>Returns true if given def has equivilant translated values in _sheetDefCur.SheetFieldDefs, otherwise false.</summary>
		private bool HasTranslatedSheetDefS(SheetFieldDef sheetFieldDef,out List<SheetFieldDef> listSheetFieldDefsMatched){
			listSheetFieldDefsMatched=null;
			if(sheetFieldDef.Language.IsNullOrEmpty()){//Method should only be called using 'Default' SheetFieldDefs
				listSheetFieldDefsMatched=_sheetDef.SheetFieldDefs.FindAll(x => !x.Language.IsNullOrEmpty() && IsEquivilantSheetDef(x,sheetFieldDef));
			}
			return (!listSheetFieldDefsMatched.IsNullOrEmpty());
		}

		///<summary>Images will be ignored in the hit test since they frequently fill the entire background.  Diagonal Lines will also be ignored.</summary>
		private SheetFieldDef HitTest(int x,int y) {
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			//Loop from the back of the list since those sheetfielddefs were added last, so they show on top of items at beginning of list.
			for(int i=listSheetFieldDefsShowing.Count-1;i>=0;i--) {
				if(listSheetFieldDefsShowing[i].FieldType==SheetFieldType.Image) {
					continue;
				}
				if(listSheetFieldDefsShowing[i].FieldType==SheetFieldType.Line) {
					if(Math.Abs(listSheetFieldDefsShowing[i].Width)<2){
						//vertical line
						if(Math.Abs(x-listSheetFieldDefsShowing[i].XPos)<2){
							if(listSheetFieldDefsShowing[i].Height<0){
								if(y>listSheetFieldDefsShowing[i].YPos+listSheetFieldDefsShowing[i].Height && y<listSheetFieldDefsShowing[i].YPos){
									return listSheetFieldDefsShowing[i];
								}
							}
							else{
								if(y>listSheetFieldDefsShowing[i].YPos && y<listSheetFieldDefsShowing[i].YPos+listSheetFieldDefsShowing[i].Height){
									return listSheetFieldDefsShowing[i];
								}
							}
						}
					}
					if(Math.Abs(listSheetFieldDefsShowing[i].Height)<2){
						//horiz line
						if(Math.Abs(y-listSheetFieldDefsShowing[i].YPos)<2){
							if(listSheetFieldDefsShowing[i].Width<0){
								if(x>listSheetFieldDefsShowing[i].XPos+listSheetFieldDefsShowing[i].Width && x<listSheetFieldDefsShowing[i].XPos){
									return listSheetFieldDefsShowing[i];
								}
							}
							else{
								if(x>listSheetFieldDefsShowing[i].XPos && x<listSheetFieldDefsShowing[i].XPos+listSheetFieldDefsShowing[i].Width){
									return listSheetFieldDefsShowing[i];
								}
							}
						}
					}
				}	
				Rectangle rectangleFieldDefBounds=listSheetFieldDefsShowing[i].Bounds;
				if(rectangleFieldDefBounds.Contains(x,y)) {
					//Center of a SheetFieldType.Rectangle will not be considered a hit.
					if(listSheetFieldDefsShowing[i].FieldType==SheetFieldType.Rectangle){
						if(new Rectangle(rectangleFieldDefBounds.X+4,rectangleFieldDefBounds.Y+4,rectangleFieldDefBounds.Width-8,rectangleFieldDefBounds.Height-8).Contains(x,y)) {
							continue;
						}
					}
					return listSheetFieldDefsShowing[i];
				}
			}
			return null;
		}

		///<summary>Returns a field if the cursor is in the lower right "resize drag" area of a single selected field.</summary>
		private SheetFieldDef HitTestResize(int x,int y) {
			if(listBoxFields.SelectedIndices.Count!=1){
				return null;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			SheetFieldDef sheetFieldDef=listSheetFieldDefsShowing[listBoxFields.SelectedIndices[0]];
			if(sheetFieldDef.FieldType==SheetFieldType.Image) {
				return null;
			}
			if(sheetFieldDef.FieldType==SheetFieldType.Line) {//can't easily resize a line
				return null;
			}	
			Rectangle rectangleHandleLR=new Rectangle(sheetFieldDef.Bounds.Right-2,sheetFieldDef.Bounds.Bottom-2,_sizeResizeGrabHandle,_sizeResizeGrabHandle);
			if(rectangleHandleLR.Contains(x,y)) {
				return sheetFieldDef;
			}
			return null;
		}

		private void InitializeComponentSheetEditMobile() {
			_sheetEditMobileCtrl.SheetFieldDefSelected+=new System.EventHandler<long>(this.sheetEditMobile_SheetDefSelected);
			_sheetEditMobileCtrl.AddMobileHeader+=new EventHandler(butAddMobileHeader_Click);
			//HasMobileLayout must be kept in sync because both FromSheetDefEdit and SheetEditMobileCtrl can changes its value at any time.
			_sheetEditMobileCtrl.HasMobileLayoutChanged+=new EventHandler<bool>((o,hasMobileLayout) => {
				_sheetDef.HasMobileLayout=hasMobileLayout;
			});
			_sheetEditMobileCtrl.TranslationProvider=new Func<string,string>((s) => { return Lan.g(this,s); });
			_sheetEditMobileCtrl.NewMobileHeader+=new EventHandler<SheetEditMobileCtrl.NewMobileFieldValueArgs>((o,e) => {
				SheetFieldDef sheetFieldDef=_sheetDef.SheetFieldDefs.FirstOrDefault(x => x.SheetFieldDefNum==e.SheetFieldDefNum);
				if(sheetFieldDef!=null) {
					sheetFieldDef.UiLabelMobile=e.NewFieldValue;
					FillFieldList();
				}
			});
			_sheetEditMobileCtrl.NewStaticText+=new EventHandler<SheetEditMobileCtrl.NewMobileFieldValueArgs>((o,e) => {
				SheetFieldDef sheetFieldDef=_sheetDef.SheetFieldDefs.FirstOrDefault(x => x.SheetFieldDefNum==e.SheetFieldDefNum);
				if(sheetFieldDef!=null) {
					sheetFieldDef.FieldValue=e.NewFieldValue;
					FillFieldList();
				}
			});
			_sheetEditMobileCtrl.SheetFieldDefEdit+=new EventHandler<SheetEditMobileCtrl.SheetFieldDefEditArgs>((o,e) => {
				if(!_sheetDef.SheetFieldDefs.Any(x => e.SheetFieldDefNums.Contains(x.SheetFieldDefNum))) {
					return;
				}
				List<SheetFieldDef> listSheetFieldDefs=_sheetDef.SheetFieldDefs.FindAll(x => e.SheetFieldDefNums.Contains(x.SheetFieldDefNum));
				SheetFieldDef sheetFieldDef=listSheetFieldDefs[0];
				//check to see if checkbox and if yes, check to see which one is highlighted or selected to pass in. 
				if(listSheetFieldDefs.Any(x => x.FieldType==SheetFieldType.CheckBox)) {
					List<long> listHighlighted=_sheetEditMobileCtrl.GetHighlightedFieldDefs(listSheetFieldDefs.Select(x => x.SheetFieldDefNum).ToList());//should only come back with 1
					if(listHighlighted.Count > 0) {
						sheetFieldDef=listSheetFieldDefs.FirstOrDefault(x => x.SheetFieldDefNum==listHighlighted.First());
					}
				}
				LaunchEditWindow(sheetFieldDef,isEditMobile:true);
			});
		}

		///<summary>When not using LayoutModes this is called to enable and fill UI.</summary>
		private void InitTranslations(string selectedThreeLetterLanguage=null){
			if(IsInternal){
				groupBoxSubViews.Visible=false;
				return;
			}
			//radioLayoutDefault.Visible=false;
			//radioLayoutTP.Visible=false;
			groupBoxSubViews.Visible=true;
			comboLanguages.Items.Clear();
			comboLanguages.Items.Add(Lan.g(this,"Add New"));
			comboLanguages.Items.Add(Lan.g(this,"Default"));
			comboLanguages.SelectedIndex=1;
			_listSheetDefLanguagesUsed.ForEach(x => comboLanguages.Items.Add(x.Display));
			if(selectedThreeLetterLanguage!=null) {
				comboLanguages.SelectedIndex=_listSheetDefLanguagesUsed.FindIndex(x => x.ThreeLetters==selectedThreeLetterLanguage)+2;//2 for 'Add New' and 'Default'
			}
		}

		///<summary>Returns true if both SheetFieldDefs are determined to be equivalent.</summary>
		private bool IsEquivilantSheetDef(SheetFieldDef sheetFieldDefOne,SheetFieldDef sheetFieldDefTwo){
			return (sheetFieldDefOne.FieldName==sheetFieldDefTwo.FieldName 
				&& sheetFieldDefOne.Bounds==sheetFieldDefTwo.Bounds
				&& (sheetFieldDefOne.FieldType==SheetFieldType.Image ? sheetFieldDefOne.ImageField==sheetFieldDefTwo.ImageField : sheetFieldDefOne.FieldValue==sheetFieldDefTwo.FieldValue));
		}

		private bool IsTypeShowing(SheetFieldType sheetFieldType){
			if(sheetFieldType==SheetFieldType.OutputText){
				return checkShowOutputText.Checked;
			}
			if(sheetFieldType==SheetFieldType.InputField){
				return checkShowInputField.Checked;
			}
			if(sheetFieldType==SheetFieldType.StaticText){
				return checkShowStaticText.Checked;
			}
			if(sheetFieldType==SheetFieldType.CheckBox){
				return checkShowCheckBox.Checked;
			}
			if(sheetFieldType==SheetFieldType.ComboBox){
				return checkShowComboBox.Checked;
			}
			if(sheetFieldType==SheetFieldType.Image){
				return checkShowImage.Checked;
			}
			if(sheetFieldType==SheetFieldType.PatImage){
				return checkShowPatImage.Checked;
			}
			if(sheetFieldType==SheetFieldType.Line){
				return checkShowLine.Checked;
			}
			if(sheetFieldType==SheetFieldType.Rectangle){
				return checkShowRectangle.Checked;
			}
			if(sheetFieldType==SheetFieldType.SigBox){
				return checkShowSigBox.Checked;
			}
			if(sheetFieldType==SheetFieldType.SigBoxPractice){
				return checkShowSigBoxPractice.Checked;
			}
			if(sheetFieldType==SheetFieldType.Special){
				return checkShowSpecial.Checked;
			}
			if(sheetFieldType==SheetFieldType.Grid){
				return checkShowGrid.Checked;
			}
			if(sheetFieldType==SheetFieldType.ScreenChart){
				return checkShowScreenChart.Checked;
			}
			if(sheetFieldType==SheetFieldType.MobileHeader){
				return checkShowMobileHeader.Checked;
			}
			return false;//shouldn't happen
		}

		///<summary>Only for editing fields that already exist.</summary>
		private void LaunchEditWindow(SheetFieldDef sheetFieldDef,bool isEditMobile) {
			//bool refreshBuffer=false;
			SheetFieldDef sheetFieldDefCopy=sheetFieldDef.Copy();//Keep a copy of the unaltered sheetFieldDef for comparison
			bool hasClickedDelete=false;
			//not every field will have been saved to the database, so we can't depend on SheetFieldDefNum.
			int idx=_sheetDef.SheetFieldDefs.IndexOf(sheetFieldDef);
			switch(sheetFieldDef.FieldType) {
				case SheetFieldType.InputField:
					using(FormSheetFieldInput formSheetFieldInput=new FormSheetFieldInput()) {
						formSheetFieldInput.SheetDefCur=_sheetDef;
						formSheetFieldInput.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldInput.IsReadOnly=IsInternal;
						formSheetFieldInput.IsEditMobile=isEditMobile;
						formSheetFieldInput.ShowDialog();
						if(formSheetFieldInput.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldInput.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.OutputText:
					using(FormSheetFieldOutput formSheetFieldOutput=new FormSheetFieldOutput()){
						formSheetFieldOutput.SheetDefCur=_sheetDef;
						formSheetFieldOutput.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldOutput.IsReadOnly=IsInternal;
						formSheetFieldOutput.IsEditMobile=isEditMobile;
						formSheetFieldOutput.ShowDialog();
						if(formSheetFieldOutput.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldOutput.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.StaticText:
					using(FormSheetFieldStatic formSheetFieldStatic=new FormSheetFieldStatic()) {
						formSheetFieldStatic.SheetDefCur=_sheetDef;
						formSheetFieldStatic.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldStatic.IsReadOnly=IsInternal;
						formSheetFieldStatic.IsEditMobile=isEditMobile;
						formSheetFieldStatic.ShowDialog();
						if(formSheetFieldStatic.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldStatic.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.Image:
					using(FormSheetFieldImage formSheetFieldImage=new FormSheetFieldImage()) {
						formSheetFieldImage.SheetDefCur=_sheetDef;
						formSheetFieldImage.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldImage.IsReadOnly=IsInternal;
						formSheetFieldImage.IsEditMobile=isEditMobile;
						formSheetFieldImage.ShowDialog();
						if(formSheetFieldImage.DialogResult!=DialogResult.OK) {
							return;
						}
						//refreshBuffer=true;
						hasClickedDelete=formSheetFieldImage.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.PatImage:
					using(FormSheetFieldPatImage formSheetFieldPatImage=new FormSheetFieldPatImage()) {
						formSheetFieldPatImage.SheetDefCur=_sheetDef;
						formSheetFieldPatImage.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldPatImage.IsReadOnly=IsInternal;
						formSheetFieldPatImage.IsEditMobile=isEditMobile;
						formSheetFieldPatImage.ShowDialog();
						if(formSheetFieldPatImage.DialogResult!=DialogResult.OK) {
							return;
						}
						//refreshBuffer=true;
						hasClickedDelete=formSheetFieldPatImage.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.Line:
					using(FormSheetFieldLine formSheetFieldLine=new FormSheetFieldLine()){
						formSheetFieldLine.SheetDefCur=_sheetDef;
						formSheetFieldLine.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldLine.IsReadOnly=IsInternal;
						formSheetFieldLine.IsEditMobile=isEditMobile;
						formSheetFieldLine.ShowDialog();
						if(formSheetFieldLine.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldLine.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.Rectangle:
					using(FormSheetFieldRect formSheetFieldRect=new FormSheetFieldRect()){
						formSheetFieldRect.SheetDefCur=_sheetDef;
						formSheetFieldRect.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldRect.IsReadOnly=IsInternal;
						formSheetFieldRect.IsEditMobile=isEditMobile;
						formSheetFieldRect.ShowDialog();
						if(formSheetFieldRect.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldRect.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.CheckBox:
					using(FormSheetFieldCheckBox formSheetFieldCheckBox=new FormSheetFieldCheckBox()){
						formSheetFieldCheckBox.SheetDefCur=_sheetDef;
						formSheetFieldCheckBox.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldCheckBox.IsReadOnly=IsInternal;
						formSheetFieldCheckBox.IsEditMobile=isEditMobile;
						formSheetFieldCheckBox.ShowDialog();
						if(formSheetFieldCheckBox.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldCheckBox.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.ComboBox:
					using(FormSheetFieldComboBox formSheetFieldComboBox=new FormSheetFieldComboBox()){
						formSheetFieldComboBox.SheetDefCur=_sheetDef;
						formSheetFieldComboBox.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldComboBox.IsReadOnly=IsInternal;
						formSheetFieldComboBox.IsEditMobile=isEditMobile;
						formSheetFieldComboBox.ShowDialog();
						if(formSheetFieldComboBox.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldComboBox.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.SigBox:
				case SheetFieldType.SigBoxPractice:
					using(FormSheetFieldSigBox formSheetFieldSigBox=new FormSheetFieldSigBox()){
						formSheetFieldSigBox.SheetDefCur=_sheetDef;
						formSheetFieldSigBox.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldSigBox.IsReadOnly=IsInternal;
						formSheetFieldSigBox.IsEditMobile=isEditMobile;
						formSheetFieldSigBox.ShowDialog();
						if(formSheetFieldSigBox.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldSigBox.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.Special:
					using(FormSheetFieldSpecial formSheetFieldSpecial=new FormSheetFieldSpecial()){
						formSheetFieldSpecial.SheetDefCur=_sheetDef;
						formSheetFieldSpecial.LayoutMode=_sheetFieldLayoutMode;
						formSheetFieldSpecial.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldSpecial.IsReadOnly=IsInternal;
						formSheetFieldSpecial.IsEditMobile=isEditMobile;
						formSheetFieldSpecial.ShowDialog();
						if(formSheetFieldSpecial.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldSpecial.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.Grid:
					using(FormSheetFieldGrid formSheetFieldGrid=new FormSheetFieldGrid()){
						formSheetFieldGrid.SheetDefCur=_sheetDef;
						formSheetFieldGrid.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldGrid.IsReadOnly=IsInternal;
						formSheetFieldGrid.IsEditMobile=isEditMobile;
						formSheetFieldGrid.ShowDialog();
						if(formSheetFieldGrid.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldGrid.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.ScreenChart:
					using(FormSheetFieldChart formSheetFieldChart=new FormSheetFieldChart()){
						formSheetFieldChart.SheetDefCur=_sheetDef;
						formSheetFieldChart.SheetFieldDefCur=sheetFieldDef;
						formSheetFieldChart.IsReadOnly=IsInternal;
						formSheetFieldChart.IsEditMobile=isEditMobile;
						formSheetFieldChart.ShowDialog();
						if(formSheetFieldChart.DialogResult!=DialogResult.OK) {
							return;
						}
						hasClickedDelete=formSheetFieldChart.SheetFieldDefCur==null;
					}
					break;
				case SheetFieldType.MobileHeader:
					using(InputBox FormInputBox=new InputBox(Lan.g(this,"Mobile Header"),sheetFieldDef.UiLabelMobile) {MaxInputTextLength=255,ShowDelete=true,}) 
					{
						if(FormInputBox.ShowDialog()!=DialogResult.OK) {
							return;
						}
						if(FormInputBox.IsDeleteClicked) {
							_sheetDef.SheetFieldDefs.RemoveAt(idx);//Deleted
						}
						else {
							sheetFieldDef.UiLabelMobile=FormInputBox.textResult.Text;
						}
					}
					
					break;
			}
			if(sheetFieldDefCopy.FieldType!=SheetFieldType.MobileHeader) {
				if(hasClickedDelete) {
					if(sheetFieldDefCopy.FieldType==SheetFieldType.Image) {
						_sheetDef.SheetFieldDefs[idx].ImageField?.Dispose();
					}
					_sheetDef.SheetFieldDefs.RemoveAt(idx);
					RemoveEquivilantTranslatedSheetDefs(sheetFieldDefCopy);//Remove all SheetFieldDefs that were not change for translated fields.
					//RefreshBitmapBackground();
					AddUndoLevel("Delete");
					panelMain.Invalidate();
				}
				else if(HasTranslatedSheetDefS(sheetFieldDefCopy,out List<SheetFieldDef> listSheetFieldDefsMatched)) {//def was not deleted.
					//Update unchanged, translated SheetFieldDefs to reflect new values.
					for(int i=0;i<listSheetFieldDefsMatched.Count;i++){
						SheetFieldDef sheetFieldDef2=sheetFieldDef.Copy();
						sheetFieldDef2.SheetFieldDefNum=listSheetFieldDefsMatched[i].SheetFieldDefNum;
						sheetFieldDef2.Language=listSheetFieldDefsMatched[i].Language;
						sheetFieldDef2.UiLabelMobile=listSheetFieldDefsMatched[i].UiLabelMobile;
						_sheetDef.SheetFieldDefs.RemoveAll(x => x.SheetFieldDefNum == listSheetFieldDefsMatched[i].SheetFieldDefNum);
						_sheetDef.SheetFieldDefs.Add(sheetFieldDef2);
					}
					AddUndoLevel("Edit");
				}
			}
			if(_isTabMode) {
				if(_listSheetFieldDefsTabOrder.Contains(sheetFieldDef)) {
					_listSheetFieldDefsTabOrder.RemoveAt(_listSheetFieldDefsTabOrder.IndexOf(sheetFieldDef));
				}
				if(sheetFieldDef.TabOrder>0 && sheetFieldDef.TabOrder<=(_listSheetFieldDefsTabOrder.Count+1)) {
					_listSheetFieldDefsTabOrder.Insert(sheetFieldDef.TabOrder-1,sheetFieldDef);
				}
				RenumberTabOrderHelper();
				return;
			}
			//listFields.ClearSelected();
			FillFieldList();
			//if(refreshBuffer) { //Only when image was edited.
			//	RefreshDoubleBuffer();
			//}
			listBoxFields.SetSelectedKey<SheetFieldDef>(sheetFieldDef.SheetFieldDefNum,x => x.SheetFieldDefNum);//ensures selection is still held after edit
			panelMain.Invalidate();
		}

		private void PasteControlsFromMemory(Point pointOrigin) {
			if(_isTabMode) {
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			if(GetIsDynamicSheetType() && _listSheetFieldDefsCopyPaste!=null) {
				//Only paste controls that are valid for _sheetFieldLayoutMode
				List<string> listGridNames=SheetUtil.GetGridsAvailable(_sheetDef.SheetType,_sheetFieldLayoutMode);
				List<SheetFieldDef> listSheetFieldDefsSpecial=SheetFieldsAvailable.GetSpecial(_sheetDef.SheetType,_sheetFieldLayoutMode);
				_listSheetFieldDefsCopyPaste.RemoveAll(
					x =>  listSheetFieldDefsShowing.Any(y => y.FieldName==x.FieldName) //Remove duplicates
					|| (x.FieldType==SheetFieldType.Grid && !listGridNames.Any(y => y==x.FieldName))//Remove invalid grid from paste logic
					|| (x.FieldType==SheetFieldType.Special && !listSheetFieldDefsSpecial.Any(y => y.FieldName==x.FieldName))//Remove invalid special fields from paste logic.
				);
			}
			if(_listSheetFieldDefsCopyPaste==null || _listSheetFieldDefsCopyPaste.Count==0) {
				return;
			}
			if(pointOrigin.X==0 && pointOrigin.Y==0) { //allows for cascading pastes in the upper right hand corner.
				Rectangle rectangle=panelMain.Bounds; //Gives relative position of panel (scroll position)
				int h=panelLeft.Height; //Current resized height/width of parent panel
				int w=panelLeft.Width;
				int maxH=0;
				int maxW=0;
				for(int i=0;i<_listSheetFieldDefsCopyPaste.Count;i++) { //calculate height/width of control to be pasted
					maxH=Math.Max(maxH,_listSheetFieldDefsCopyPaste[i].Height);
					maxW=Math.Max(maxW,_listSheetFieldDefsCopyPaste[i].Width);
				}
				pointOrigin=new Point((-1)*rectangle.X+w/2-maxW/2-10,(-1)*rectangle.Y+h/2-maxH/2-10); //Center: scroll position * (-1) + 1/2 size of window - 1/2 the size of the field - 10 for scroll bar
				pointOrigin.X+=_pasteOffset;
				pointOrigin.Y+=_pasteOffset+_pasteOffsetY;
			}
			listBoxFields.ClearSelected();
			int minX=int.MaxValue;
			int minY=int.MaxValue;
			for(int i=0;i<_listSheetFieldDefsCopyPaste.Count;i++) { //calculate offset
				minX=Math.Min(minX,_listSheetFieldDefsCopyPaste[i].XPos);
				minY=Math.Min(minY,_listSheetFieldDefsCopyPaste[i].YPos);
			}
			List<SheetFieldDef> listSheetFieldDefsNew=new List<SheetFieldDef>();
			for(int i=0;i<_listSheetFieldDefsCopyPaste.Count;i++) { //create new controls
				SheetFieldDef sheetFieldDef=_listSheetFieldDefsCopyPaste[i].Copy();
				sheetFieldDef.XPos=sheetFieldDef.XPos-minX+pointOrigin.X;
				sheetFieldDef.YPos=sheetFieldDef.YPos-minY+pointOrigin.Y;
				AddNewSheetFieldDef(sheetFieldDef);
				listSheetFieldDefsNew.Add(sheetFieldDef);
			}
			if(!_isAltDown) {
				_pasteOffsetY+=((_pasteOffset+10)/100)*10; //this will shift the pastes down 10 pixels every 10 pastes.
				_pasteOffset=(_pasteOffset+10)%100; //cascades and allows for 90 consecutive pastes without overlap
			}
			AddUndoLevel("Paste");
			FillFieldList();
			//Select newly pasted controls.
			for(int i=0;i<listBoxFields.Items.Count;i++){
				if(listSheetFieldDefsNew.Contains(listBoxFields.Items.GetObjectAt(i))){
					listBoxFields.SetSelected(i);
				}
			}
			/*
			listSheetFieldDefsShowing
				.Select((Item,Index) => new { Item,Index })
				.Where(x => listSheetFieldDefsNew.Any(y => y.SheetFieldDefNum==x.Item.SheetFieldDefNum))
				.ForEach(x => { listBoxFields.SetSelected(x.Index,true); });*/
			panelMain.Invalidate();
			//Focus on main form. This ensures that if/when the alt key is released, the proper handler will execute. Fixes bug where user becomes stuck in paste mode.
			this.Focus();
		}

		private bool PromptUpdateEClipboardSheetDefs() {
			List<EClipboardSheetDef> listEClipboardSheetDefs = EClipboardSheetDefs.GetAllForSheetDefForOnceRule(_sheetDef.SheetDefNum);
			if(listEClipboardSheetDefs == null || listEClipboardSheetDefs.Count == 0) {
				return true; // nothing to update, no need to prompt.
			}
			string message = $"At least one eClipboard sheet currently uses this Sheet Def. If you check the box and press 'OK', patients will be forced to fill this form out again the next time they check in using eClipboard.";
			List<InputBoxParam> listInputBoxParams = new List<InputBoxParam>{
				new InputBoxParam(InputBoxType.CheckBox,message,"Force patients to fill out again",new Size(200,80),new Point(x:15,y:30))
			};
			using InputBox inputBox = new InputBox(listInputBoxParams, null);
			inputBox.ShowDialog();
			if(inputBox.DialogResult == DialogResult.Cancel) {
				return false; // don't save the sheet
			}
			if(!inputBox.checkBoxResult.Checked) {
				return true; // save the sheet but don't update override
			}
			message = "Are you sure you want to force all patients to fill out this form again? Clicking 'No' will not update the eClipboard version of this sheet.";
			if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return true; // save sheet, but don't update override
			}
			//got to here, user wants to update eClipboard sheets and force patients to refill them.
			foreach(EClipboardSheetDef eClipboardSheetDef in listEClipboardSheetDefs) {
				eClipboardSheetDef.PrefillStatusOverride=_sheetDef.RevID;
				EClipboardSheetDefs.Update(eClipboardSheetDef);
			}
			return true;
		}

		///<summary>Whenever a user might have edited or moved a background image, this gets called.</summary>
		private void RefreshBitmapBackground() {
			_graphicsBackground.FillRectangle(Brushes.White,0,0,_bitmapBackground.Width,_bitmapBackground.Height);
			DrawFields(_sheetDef,_graphicsBackground,onlyDrawImages:true);
		}

		///<summary>Refreshes local _listUsedLanguages and _listUnusedLanguages data. Called when loading or after user adds a new language for translations.</summary>
		private void RefreshLanguages(string selectedThreeLetterLanguage=null) {
			List<string> listAllLanguages=PrefC.GetString(PrefName.LanguagesUsedByPatients)//Must be before initial InitLayoutModes().
				.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)
				.Where(x => x!=Patients.LANGUAGE_DECLINED_TO_SPECIFY).ToList();
			List<string> listUsedLanguageThreeLetters=_sheetDef.SheetFieldDefs.FindAll(x => !x.Language.IsNullOrEmpty())
				.Select(x => x.Language)
				.Distinct().ToList();
			_listSheetDefLanguagesUsed=listUsedLanguageThreeLetters
				.Select(x => new SheetDefLanguage(x,(MiscUtils.GetCultureFromThreeLetter(x)?.DisplayName??x)))
				.OrderBy(x => x.Display).ToList();
			_listSheetDefLanguagesUnused=listAllLanguages.FindAll(x => !listUsedLanguageThreeLetters.Contains(x))
				.Select(x => new SheetDefLanguage(x,(MiscUtils.GetCultureFromThreeLetter(x)?.DisplayName??x)))
				.OrderBy(x => x.Display).ToList();
			if(selectedThreeLetterLanguage!=null && _listSheetDefLanguagesUnused.Select(x => x.ThreeLetters).Contains(selectedThreeLetterLanguage)) {
				//When the default sheet language does not have any SheetFieldDefs defined, and Add New is selected from comboLanguages, no SheetFieldDefs
				//are copied to the newly selected language, and as a result, _listUsedLanguages does not contain the newly selected language, effectively
				//negating the user's Add New selection.  Therefore, add the selection back here.  (This would mean a translation has SheetFieldDefs while
				//the default language has none.  Use Case: user intends to create a sheet specifically for spanish speakers).
				_listSheetDefLanguagesUsed.Add(new SheetDefLanguage(selectedThreeLetterLanguage
					,(MiscUtils.GetCultureFromThreeLetter(selectedThreeLetterLanguage)?.DisplayName??selectedThreeLetterLanguage)));
			}
		}

		///<summary></summary>
		private void RefreshLanguagesAndPanelMain(bool doRefreshLanguages=true,string selectedThreeLetterLanguage=null){
			if(doRefreshLanguages) {
				RefreshLanguages(selectedThreeLetterLanguage);//Update UI and data after adding a new translation.
				InitTranslations(selectedThreeLetterLanguage);//Selects given selectedThreeLetterLanguage in comboLanguages.
			}
			FillFieldList();//Reflects new fields in field list and merges changes for control sheetEditMobile
			panelMain.Invalidate();
		}

		///<summary>Removes all translated SheetFieldDefs from _sheetDefCur.SheetFieldDefs that are equivilant to given def.</summary>
		private void RemoveEquivilantTranslatedSheetDefs(SheetFieldDef defDefault){
			_sheetDef.SheetFieldDefs.RemoveAll(x => !x.Language.IsNullOrEmpty() && IsEquivilantSheetDef(x,defDefault));
		}

		///<summary>Used To renumber TabOrder on controls</summary>
		private void RenumberTabOrderHelper() {
			for(int i=0;i<_listSheetFieldDefsTabOrder.Count;i++) {
				_listSheetFieldDefsTabOrder[i].TabOrder=i+1; //Start number tab order at 1
			}
			FillFieldList();
			panelMain.Invalidate();
		}

		///<summary>Returns true if user selected a new language via InputBox to translate to, otherwise false.</summary>
		private bool TryAddTranslation(out string selectedLangauge) {
			selectedLangauge=null;
			string prompt=Lan.g(this,"Please select a new language to translate to.");
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam> { 
				new InputBoxParam(InputBoxType.ComboSelect,prompt,_listSheetDefLanguagesUnused.Select(x => x.Display).ToList()) 
			};
			using InputBox formInputBox=new InputBox(listInputBoxParams);
			if(formInputBox.ShowDialog()!=DialogResult.OK) {
				return false;
			}
			selectedLangauge=_listSheetDefLanguagesUnused[formInputBox.SelectedIndex].ThreeLetters;
			return (!selectedLangauge.IsNullOrEmpty());
		}

		private void SetPanelMainSize(){
			Size sizeNew=new Size(LayoutManager.Scale(_sheetDef.Width),LayoutManager.Scale(_sheetDef.Height));
			if(_sheetDef.IsLandscape) {
				sizeNew=new Size(LayoutManager.Scale(_sheetDef.Height),LayoutManager.Scale(_sheetDef.Width));
			}
			sizeNew.Height=LayoutManager.Scale(_sheetDef.HeightTotal);
			if(_sheetDef.PageCount>1){
				sizeNew.Height-=_margins.Bottom;//-60 first page
				sizeNew.Height-=(_sheetDef.PageCount-1)*(_margins.Top+_margins.Bottom);//-100 remaining pages
			}
			if(panelMain.Size==sizeNew){
				//this is called repeatedly during resize, and we also don't want to resize position unless required
				return;
			}
			//If the user has scrolled down/right inside of the splitContainer, the panel's initial location would be negative as it is currently above/left of the current view inside of the form.  Redrawing this down the page by resetting panelMain's location to point 4,4 created a large gap of white space above the newly drawn location.  Resetting the scroll position prevents this.
			panelLeft.VerticalScroll.Value=0;
			panelLeft.HorizontalScroll.Value=0;
			LayoutManager.Move(panelMain,new Rectangle(4,4,sizeNew.Width,sizeNew.Height));
		}

		///<summary>Called during load to set _sheetTypeModeCur and to identify pertinent secondary TreatPlan layout mode. The initial layout will never be a treatment plan layout because the Treatment Plan checkbox is always disabled in Chart initially. Returns true if current _sheetDefCur.SheetTYpe is associated to a dynamic layout SheetType, otherwise false.</summary>
		private bool TryInitLayoutModes() {
			_sheetFieldLayoutMode=SheetFieldLayoutMode.Default;
			switch(_sheetDef.SheetType) {
				case SheetTypeEnum.PatientDashboardWidget:
					groupBoxSubViews.Visible=false;
					comboLanguages.Visible=false;
					checkSynchMatchedFields.Visible=false;
					return true;
				//the intent was to eventually support other modules.
				case SheetTypeEnum.ChartModule:
					if(Programs.UsingEcwTightOrFullMode()) {
						_sheetFieldLayoutMode=SheetFieldLayoutMode.Ecw;
					}
					else if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						_sheetFieldLayoutMode=SheetFieldLayoutMode.MedicalPractice;
					}
					/*SheetFieldLayoutMode sheetFieldLayoutModeTreatPlan;//The TreatmentPlan counterpart layout mode.
					switch(_sheetFieldLayoutMode) {
						default:
						case SheetFieldLayoutMode.Default:
							sheetFieldLayoutModeTreatPlan=SheetFieldLayoutMode.TreatPlan;
							break;
						case SheetFieldLayoutMode.Ecw:
							sheetFieldLayoutModeTreatPlan=SheetFieldLayoutMode.EcwTreatPlan;
							break;
						case SheetFieldLayoutMode.MedicalPractice:
							sheetFieldLayoutModeTreatPlan=SheetFieldLayoutMode.MedicalPracticeTreatPlan;
							break;
					}
					radioLayoutDefault.Tag=_sheetFieldLayoutMode;//Used in radioLayoutDefault_CheckedChanged()
					radioLayoutTP.Tag=sheetFieldLayoutModeTreatPlan;//Used in radioLayoutDefault_CheckedChanged()
					groupBoxSubViews.Text=Lan.g(this,"Layout Modes");//group box defaults to 'Language'
					comboLanguages.Visible=false;
					checkSynchMatchedFields.Visible=false;
					radioLayoutDefault.Checked=true;*/
					return true;
				default:
					return false;
			}
		}

		///<summary>Updates the web sheet defs linked to this sheet def if the user agrees. Returns true if this sheet is okay to be saved to the database.</summary>
		private bool UpdateWebSheetDef() {
			if(_sheetDef.IsNew) {
				//There is no Web Form to sync because they just created this sheet def.
				//Without this return we would show the user that this Sheet Def matches all web forms that are not yet linked to a valid Sheet Def.
				return true;
			}
			Cursor=Cursors.WaitCursor;
			while(!_hasSheetsDownloaded) {//The thread has not finished getting the list. 
				Application.DoEvents();
				Thread.Sleep(100);
			}
			Cursor=Cursors.Default;
			if(_listWebForms_SheetDefs==null || _listWebForms_SheetDefs.Count==0) {//No web forms use this sheet def.
				return true;
			}
			string message=Lan.g(this,"This Sheet Def is used by the following web "+(_listWebForms_SheetDefs.Count==1?"form":"forms"))+":\r\n"
				+string.Join("\r\n",_listWebForms_SheetDefs.Select(x => x.Description))+"\r\n"
				+Lan.g(this,"Do you want to update "+(_listWebForms_SheetDefs.Count==1?"that web form":"those web forms")+"?");
			if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)==DialogResult.No) {
				return true;
			}
			if(!WebFormL.VerifyRequiredFieldsPresent(_sheetDef)) {
				return false;
			}
			Cursor=Cursors.WaitCursor;
			WebFormL.LoadImagesToSheetDef(_sheetDef);
			bool isSuccess=WebFormL.TryAddOrUpdateSheetDef(this,_sheetDef,false,_listWebForms_SheetDefs);
			Cursor=Cursors.Default;
			return isSuccess;
		}

		private bool VerifyDesign() {
			//Keep a temporary list of every medical input and check box so it saves time checking for duplicates.
			List<SheetFieldDef> listSheetFieldDefMedCheckBox=new List<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefInputMedList=new List<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefCheckMedList=new List<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefErroneousDuplicates=new List<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefToDelete=new List<SheetFieldDef>();
			//Verify radio button groups.
			for(int i=0;i<_sheetDef.SheetFieldDefs.Count;i++) {
				SheetFieldDef sheetFieldDef=_sheetDef.SheetFieldDefs[i];
				if(PrefC.GetBool(PrefName.EasyHidePublicHealth) //Public health disabled
					&& _sheetDef.SheetType==SheetTypeEnum.PatientForm //Is Patient Form
					&& sheetFieldDef.FieldName.In("Sexual Orientation","Gender Identity")) //Contains public health fields
				{
					listSheetFieldDefToDelete.Add(sheetFieldDef);
				}
				if(sheetFieldDef.FieldType==SheetFieldType.CheckBox && sheetFieldDef.IsRequired && (sheetFieldDef.RadioButtonGroup!="" //for misc radio groups
				                                                                    || sheetFieldDef.RadioButtonValue!="")) //for built-in radio groups
				{
					//All radio buttons within a group must either all be marked required or all be marked not required. 
					//Not the most efficient check, but there won't usually be more than a few hundred items so the user will not ever notice. We can speed up later if needed.
					for(int j=0;j<_sheetDef.SheetFieldDefs.Count;j++) {
						SheetFieldDef sheetFieldDef2=_sheetDef.SheetFieldDefs[j];
						if(sheetFieldDef2.FieldType==SheetFieldType.CheckBox && !sheetFieldDef2.IsRequired && sheetFieldDef2.RadioButtonGroup.ToLower()==sheetFieldDef.RadioButtonGroup.ToLower() //for misc groups
						   && sheetFieldDef2.FieldName.ToLower()==sheetFieldDef.FieldName.ToLower()) //for misc groups
						{
							MessageBox.Show(Lan.g(this,"Radio buttons in radio button group")+" '"+(sheetFieldDef.RadioButtonGroup==""?sheetFieldDef.FieldName:sheetFieldDef.RadioButtonGroup)+"' "+Lan.g(this,"must all be marked required or all be marked not required."));
							return false;
						}
					}
				}
				if(sheetFieldDef.FieldType==SheetFieldType.CheckBox && (sheetFieldDef.FieldName.StartsWith("allergy:")) || sheetFieldDef.FieldName.StartsWith("problem:")) {
					foreach(SheetFieldDef sheetFieldDefMedCheckBox in listSheetFieldDefMedCheckBox) { //Check for duplicates.
						if(sheetFieldDefMedCheckBox.FieldName==sheetFieldDef.FieldName && sheetFieldDefMedCheckBox.RadioButtonValue==sheetFieldDef.RadioButtonValue && sheetFieldDefMedCheckBox.Language==sheetFieldDef.Language) {
							listSheetFieldDefErroneousDuplicates.Add(sheetFieldDef);
							continue;
						}
					}
					//Not a duplicate so add it to the med chk box list.
					listSheetFieldDefMedCheckBox.Add(sheetFieldDef);
				}
				else if(sheetFieldDef.FieldType==SheetFieldType.InputField && sheetFieldDef.FieldName.StartsWith("inputMed")) {
					foreach(SheetFieldDef sheetFieldDef2 in listSheetFieldDefInputMedList) { //Check for duplicates.
						if(sheetFieldDef2.FieldName==sheetFieldDef.FieldName && sheetFieldDef2.Language==sheetFieldDef.Language) {
							listSheetFieldDefErroneousDuplicates.Add(sheetFieldDef);
							continue;
						}
					}
					//Not a duplicate so add it to the input med list.
					listSheetFieldDefInputMedList.Add(sheetFieldDef);
				}
				else if(sheetFieldDef.FieldType==SheetFieldType.CheckBox && sheetFieldDef.FieldName.StartsWith("checkMed")) {
					foreach(SheetFieldDef sheetFieldDefMedCheckBox in listSheetFieldDefCheckMedList) { //Check for duplicates.
						if(sheetFieldDefMedCheckBox.FieldName==sheetFieldDef.FieldName && sheetFieldDefMedCheckBox.Language==sheetFieldDef.Language) {
							listSheetFieldDefErroneousDuplicates.Add(sheetFieldDef);
							continue;
						}
					}
					//Not a duplicate so add it to the check med list.
					listSheetFieldDefCheckMedList.Add(sheetFieldDef);
				}
			}
			//If any duplicates are found in a language, show a message to the user for all duplicates.
			if(listSheetFieldDefErroneousDuplicates.Count>0) {
				string errorMessage=GetDuplicateSheetFieldDefErrorMessage(listSheetFieldDefErroneousDuplicates);
				MessageBox.Show(errorMessage);
				return false;
			}
			if(listSheetFieldDefToDelete.Count>0) {
				StringBuilder fieldsDeletedMessage=new StringBuilder();
				fieldsDeletedMessage.AppendLine("Public Health is no longer enabled. The following fields have been deleted:");
				listSheetFieldDefToDelete.ForEach(x => { 
					fieldsDeletedMessage.AppendLine(x.FieldName);
					_sheetDef.SheetFieldDefs.Remove(x);
				});
				MessageBox.Show(fieldsDeletedMessage.ToString());
			}
			//Check that each check med has a matching med input.
			if(listSheetFieldDefCheckMedList.Count!=0 && listSheetFieldDefInputMedList.Any(x => !listSheetFieldDefCheckMedList.Exists(y => y.FieldName==x.FieldName.Replace("inputMed","checkMed")))) {
				string inputMedMissingCheckBox=listSheetFieldDefInputMedList.Select(x=>x.FieldName).Where(x=>!listSheetFieldDefCheckMedList.Exists(y => y.FieldName==x.Replace("inputMed","checkMed"))).FirstOrDefault();
				MessageBox.Show(Lan.g(this,"Missing checkMed boxes found")+": '"+inputMedMissingCheckBox.Replace("inputMed","checkMed")+"'. "+Lan.g(this,"All inputMed should have a corresponding checkMed, or all checkMeds should be excluded."));
				return false;
			}
			switch(_sheetDef.SheetType) {
				case SheetTypeEnum.TreatmentPlan:
					if(_sheetDef.SheetFieldDefs.FindAll(x => x.FieldType==SheetFieldType.SigBox).GroupBy(x => x.Language).Any(x => x.ToList().Count!=1)) {
						MessageBox.Show(Lan.g(this,"Treatment plans must have exactly one patient signature box."));
						return false;
					}
					if(_sheetDef.SheetFieldDefs.FindAll(x => x.FieldType==SheetFieldType.SigBoxPractice).GroupBy(x => x.Language).Any(x => x.ToList().Count>1)) {
						MessageBox.Show(Lan.g(this,"Treatment plans cannot have more than one practice signature box."));
						return false;
					}
					if(_sheetDef.SheetFieldDefs.FindAll(x => x.FieldType==SheetFieldType.Grid && x.FieldName=="TreatPlanMain").GroupBy(x => x.Language).Any(x => x.ToList().Count<1)) {
						MessageBox.Show(Lan.g(this,"Treatment plans must have one main grid."));
						return false;
					}
					break;
			}
			//Verify that the current SheetDef can be serialized and deserialized correctly (valid XML).
			try {
				//Make sure that C# can serialize the SheetDef object.
				XmlSerializer xmlSerializer=new XmlSerializer(typeof(SheetDef));
				StringBuilder stringBuilder=new StringBuilder();
				using(XmlWriter xmlWriter=XmlWriter.Create(stringBuilder)) {
					xmlSerializer.Serialize(xmlWriter,_sheetDef);
				}
				//Make sure that C# can deserialize the string representation of the SheetDef object.
				using(TextReader textReader=new StringReader(stringBuilder.ToString())) {
					SheetDef sheetDef=(SheetDef)xmlSerializer.Deserialize(textReader);
				}
			}
			catch(Exception ex) {
				//Warn the user of any problems but let them continue since it is not a guarantee that they will Export / Import this def, use it as a Web Form, etc.
				FriendlyException.Show("There will be a problem with exporting or importing this Sheet Def.",ex);
			}
			return true;
		}

		private string GetDuplicateSheetFieldDefErrorMessage(List<SheetFieldDef> listSheetFieldDefs) {
			List<SheetFieldDef> listSheetFieldDefsCopy=new List<SheetFieldDef>(listSheetFieldDefs);
			//Since the list will contain duplicates, filter out the duplicate so we don't get two messages for the same error.
			listSheetFieldDefsCopy=listSheetFieldDefsCopy.GroupBy(x=>new {x.FieldName, x.FieldType, x.Language }).Select(x=> x.First()).ToList();
			listSheetFieldDefsCopy.OrderBy(x=>x.Language);
			StringBuilder message=new StringBuilder(Lan.g(this,"Duplicate fields found. Only one of the following fields is allowed per language")+":\r\n");
			//If this error occured in a language other than the default.
			for(int i=0;i<listSheetFieldDefsCopy.Count;i++) {
				SheetFieldDef sheetFieldDef=listSheetFieldDefsCopy[i];
				string fieldName=sheetFieldDef.FieldName;
				string fieldType=sheetFieldDef.FieldType.GetDescription();
				string language="";
				if(!string.IsNullOrWhiteSpace(sheetFieldDef.Language)) {
					language=_listSheetDefLanguagesUsed.FirstOrDefault(x=>x.ThreeLetters==sheetFieldDef.Language)?.Display;
					language=string.IsNullOrWhiteSpace(language) ? sheetFieldDef.Language : language;
				}
				message.AppendLine("Field: "+fieldName + "; Type: " + fieldType + (string.IsNullOrWhiteSpace(language)? "" :  "; Language: " + language));
			}
			return message.ToString();
		}
		#endregion Methods - Private

		#region Methods - Draw
		///<summary>Creates a grid with the given columns for grids associated to a dynamic layout sheetDef.</summary>
		private GridOD CreateDynamicGrid(SheetFieldDef fieldDef,List<DisplayField> listColumns) {
			GridOD odGrid=new GridOD();
			odGrid.Width=fieldDef.Width;
			odGrid.TranslationName="";
			odGrid.BeginUpdate();
			odGrid.Columns.Clear();
			foreach(DisplayField column in listColumns) { 
				string colHeader=column.Description;
				if(string.IsNullOrEmpty(colHeader)) {
					colHeader=column.InternalName;
				}
				odGrid.Columns.Add(new GridColumn(colHeader,column.ColumnWidth));
			}
			GridRow row=new GridRow();//Add empty row
			for(int c=0;c<listColumns.Count; c++) {
				row.Cells.Add(" ");//Add empty cell to give the empty row some content
			}
			odGrid.ListGridRows.Add(row);
			odGrid.EndUpdate();//Calls ComputeRows and ComputeColumns, meaning the RowHeights int[] has been filled.
			return odGrid;
		}

		private GridOD CreateGrid(List<DisplayField> columns) {
			GridOD odGrid=new GridOD();
			odGrid.Width=0;
			odGrid.TranslationName="";
			for(int c=0;c<columns.Count;c++) {
				odGrid.Width+=columns[c].ColumnWidth;
			}
			odGrid.BeginUpdate();
			odGrid.Columns.Clear();
			GridColumn col;
			for(int c=0;c<columns.Count;c++) {
				col=new GridColumn(columns[c].Description,columns[c].ColumnWidth);
				odGrid.Columns.Add(col);
			}
			GridRow row=new GridRow(); //Add dummy row
			for(int c=0;c<columns.Count;c++) {
				row.Cells.Add(" "); //add dummy row.
			}
			odGrid.ListGridRows.Add(row);
			odGrid.EndUpdate(); //Calls ComputeRows and ComputeColumns, meaning the RowHeights int[] has been filled.
			return odGrid;
		}

		private void DrawCheckBox(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color color=_colorGray;
			if(checkBlue.Checked){
				color=_colorBlue;
			}
			if(isSelected){
				color=_colorRed;
			}
			Pen pen=new Pen(color,1.6f);
			g.DrawLine(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.XPos+sheetFieldDef.Width-1,sheetFieldDef.YPos+sheetFieldDef.Height-1);
			g.DrawLine(pen,sheetFieldDef.XPos+sheetFieldDef.Width-1,sheetFieldDef.YPos,sheetFieldDef.XPos,sheetFieldDef.YPos+sheetFieldDef.Height-1);
			if(_isTabMode) {
				Rectangle rectTab=new Rectangle(sheetFieldDef.XPos-1, //X
					sheetFieldDef.YPos-1, //Y
					(int)g.MeasureString(sheetFieldDef.TabOrder.ToString(),_fontTabOrder).Width+1, //Width
					12); //height
				if(_listSheetFieldDefsTabOrder.Contains(sheetFieldDef)) { //blue border, white box, blue letters
					g.FillRectangle(Brushes.White,rectTab);
					g.DrawRectangle(Pens.Blue,rectTab);
					g.DrawString(sheetFieldDef.TabOrder.ToString(),_fontTabOrder,Brushes.Blue,rectTab.X,rectTab.Y-1);
				}
				else { //Blue border, blue box, white letters
					g.FillRectangle(Brushes.Blue,rectTab);
					g.DrawString(sheetFieldDef.TabOrder.ToString(),_fontTabOrder,Brushes.White,rectTab.X,rectTab.Y-1);
				}
			}
		}

		private void DrawComboBox(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color color=_colorGray;
			if(checkBlue.Checked){
				color=_colorBlue;
			}
			if(isSelected) {
				color=_colorRed;
			}
			using Pen pen=new Pen(color);
			using SolidBrush solidBrush=new SolidBrush(color);
			g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
			g.DrawString("("+Lan.g(this,"combo box")+")",Font,solidBrush,sheetFieldDef.XPos,sheetFieldDef.YPos);
		}

		private void DrawControlForSheetField(Graphics g,Control control,SheetFieldDef sheetFieldDef) {
			Bitmap bitmap=new Bitmap(control.Width,control.Height);
			control.DrawToBitmap(bitmap,control.Bounds);
			g.DrawImage(bitmap,new Rectangle(sheetFieldDef.XPos,sheetFieldDef.YPos,control.Width,control.Height));
			bitmap?.Dispose();
		}

		private void DrawGrid(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			int heightGridTitle=18;
			int heightGridHeader=15;
			int heightGridRow=13+2;
			List<DisplayField> listDisplayFieldColumns=SheetUtil.GetGridColumnsAvailable(sheetFieldDef.FieldName);
			GridOD grid;
			if(GetIsDynamicSheetType()) {
				grid=CreateDynamicGrid(sheetFieldDef,listDisplayFieldColumns);
				SheetUtil.SetControlSizeAndAnchors(sheetFieldDef,grid,panelMain);
			}
			else {
				grid=CreateGrid(listDisplayFieldColumns);
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			int yPosGrid=sheetFieldDef.YPos;
			SizeF sizeStr;
			bool drawHeaders=true;
			switch(sheetFieldDef.FieldName) {
				#region StatementPayPlan
				case "StatementPayPlan":
				case "StatementDynamicPayPlan":
					string text="Payment Plans";
					if(sheetFieldDef.FieldName=="StatementDynamicPayPlan") {
						text="Dynamic Payment Plans";
					}
					sizeStr=g.MeasureString(text,new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold));
					g.FillRectangle(Brushes.White,sheetFieldDef.XPos,yPosGrid,grid.Width,heightGridTitle);
					g.DrawString(text,new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),new SolidBrush(Color.Black),sheetFieldDef.XPos+(sheetFieldDef.Width-sizeStr.Width)/2,yPosGrid);
					yPosGrid+=heightGridTitle;
					break;
				#endregion
				#region StatementInvoicePayment
				case "StatementInvoicePayment":
					sizeStr=g.MeasureString("Payments",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold));
					g.FillRectangle(Brushes.White,sheetFieldDef.XPos,yPosGrid,grid.Width,heightGridTitle);
					g.DrawString("Payments",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),new SolidBrush(Color.Black),sheetFieldDef.XPos,yPosGrid);
					yPosGrid+=heightGridTitle;
					break;
				#endregion
				#region TreatPlanBenefitsFamily
				case "TreatPlanBenefitsFamily":
					sizeStr=g.MeasureString("Family Insurance Benefits",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold));
					g.FillRectangle(Brushes.White,sheetFieldDef.XPos,yPosGrid,grid.Width,heightGridTitle);
					g.DrawString("Family Insurance Benefits",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),Brushes.Black,sheetFieldDef.XPos+(sheetFieldDef.Width-sizeStr.Width)/2,yPosGrid);
					yPosGrid+=heightGridTitle;
					break;
				#endregion
				#region TreatPlanBenefitsIndividual
				case "TreatPlanBenefitsIndividual":
					sizeStr=g.MeasureString("Individual Insurance Benefits",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold));
					g.FillRectangle(Brushes.White,sheetFieldDef.XPos,yPosGrid,grid.Width,heightGridTitle);
					g.DrawString("Individual Insurance Benefits",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),Brushes.Black,sheetFieldDef.XPos+(sheetFieldDef.Width-sizeStr.Width)/2,yPosGrid);
					yPosGrid+=heightGridTitle;
					break;
				#endregion
				#region EraClaimsPaid
				case "EraClaimsPaid":
					int xPosGrid=sheetFieldDef.XPos;
					SheetDef sheetDefERAGridHeader;
					if(IsInternal) {
						sheetDefERAGridHeader=SheetsInternal.GetSheetDef(SheetTypeEnum.ERAGridHeader);
					}
					else {
						sheetDefERAGridHeader=SheetDefs.GetInternalOrCustom(SheetInternalType.ERAGridHeader);
					}
					sheetDefERAGridHeader.SheetFieldDefs.ForEach(x => {
						x.XPos+=(xPosGrid);
						x.YPos+=(yPosGrid);//-gridHeaderSheetDef.Height);
					});//Make positions relative to this sheet
					DrawFields(sheetDefERAGridHeader,g,onlyDrawImages:false,isHighlightEligible:false);
					yPosGrid+=sheetDefERAGridHeader.Height;
					/*---------------------------------------------------------------------------------------------*/
					sizeStr=g.MeasureString("ERA Claims Paid",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold));
					g.FillRectangle(Brushes.White,xPosGrid,yPosGrid,grid.Width,heightGridTitle);
					g.DrawString("ERA Claims Paid",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),Brushes.Black,xPosGrid+(sheetFieldDef.Width-sizeStr.Width)/2,yPosGrid);
					yPosGrid+=heightGridTitle;
					/*---------------------------------------------------------------------------------------------*/
					grid.SheetDrawHeader(g,sheetFieldDef.XPos,yPosGrid);
					yPosGrid+=heightGridHeader;
					grid.SheetDrawRow(0,g,sheetFieldDef.XPos,yPosGrid,false,true); //a single dummy row.
					yPosGrid+=heightGridRow;
					/*---------------------------------------------------------------------------------------------*/
					List<DisplayField> listDisplayField=SheetUtil.GetGridColumnsAvailable("EraClaimsPaidCodes");
					GridOD gridCode=CreateGrid(listDisplayField);
					gridCode.SheetDrawHeader(g,sheetFieldDef.XPos,yPosGrid+1);
					yPosGrid+=heightGridHeader;
					gridCode.SheetDrawRow(0,g,sheetFieldDef.XPos,yPosGrid,false,true); //a single dummy row.
					yPosGrid+=heightGridRow;
					/*---------------------------------------------------------------------------------------------*/
					drawHeaders=false;
					break;
				#endregion
				#region PatientInfo, ProgressNotes, TreatmentPlans, Procedures
				case "PatientInfo":
				case "ProgressNotes":
				case "TreatmentPlans":
				case "Procedures":
					int gridRemainingHeight; //Used to draw "rows" in the SheetDefEdit
					if(sheetFieldDef.GrowthBehavior.In(GrowthBehaviorEnum.FillDown,GrowthBehaviorEnum.FillRightDown,GrowthBehaviorEnum.FillDownFitColumns)) {
						int lowerBoundary=0;
						switch(sheetFieldDef.GrowthBehavior) {
							case GrowthBehaviorEnum.FillDownFitColumns:
								int minY=0;//Mimics height logic in SheetUserControl.IsControlAbove(...)
								List<SheetFieldDef> listSheetFieldDefAboveControls=listSheetFieldDefsShowing.FindAll(x => IsSheetFieldDefAbove(sheetFieldDef,x));
								if(listSheetFieldDefAboveControls.Count>0) {
									minY=listSheetFieldDefAboveControls.Max(x => (x.YPos+x.Height))+1;
								}
								sheetFieldDef.YPos=minY;
								yPosGrid=minY;
								break;
							case GrowthBehaviorEnum.FillDown:
								List<SheetFieldDef> listSheetFieldDefsBelowControls=listSheetFieldDefsShowing.FindAll(x => IsSheetFieldDefBelow(sheetFieldDef, x));
								if(listSheetFieldDefsBelowControls.Count>0) {
									lowerBoundary=(panelMain.Height-listSheetFieldDefsBelowControls.Min(x => x.YPos)-1);
								}
								break;
						}
						gridRemainingHeight=Math.Max(1,(panelMain.Height-yPosGrid-lowerBoundary));
					}
					else {
						gridRemainingHeight=sheetFieldDef.Height;
					}
					grid.Title=sheetFieldDef.FieldName;
					grid.SheetDrawTitle(g,sheetFieldDef.XPos,yPosGrid);
					yPosGrid+=heightGridTitle;
					gridRemainingHeight-=heightGridTitle;
					grid.SheetDrawHeader(g,sheetFieldDef.XPos,yPosGrid);
					yPosGrid+=heightGridHeader;
					gridRemainingHeight-=heightGridHeader;
					int gridRowHeight=heightGridRow;//Zero until we calculate the height of the first row.
					while(gridRemainingHeight>gridRowHeight) {
						grid.SheetDrawRow(0,g,sheetFieldDef.XPos,yPosGrid,(gridRemainingHeight-gridRowHeight <= gridRowHeight),true);//A single dummy row.
						yPosGrid+=gridRowHeight;
						gridRemainingHeight-=gridRowHeight;
					}
					drawHeaders=false;
					break;
				#endregion
			}
			if(drawHeaders) {
				grid.SheetDrawHeader(g,sheetFieldDef.XPos,yPosGrid);
				yPosGrid+=heightGridHeader;
				grid.SheetDrawRow(0,g,sheetFieldDef.XPos,yPosGrid,false,true); //a single dummy row.
				yPosGrid+=heightGridRow;
			}
			#region drawFooter
			if(sheetFieldDef.FieldName=="StatementPayPlan" || sheetFieldDef.FieldName=="StatementDynamicPayPlan") {
				string text="Payment Plan Amount Due: "+"0.00";
				if(sheetFieldDef.FieldName=="StatementDynamicPayPlan") {
					text="Dynamic Payment Plan Amount Due: "+"0.00";
				}
				RectangleF rectangleF=new RectangleF(sheetFieldDef.Width-sheetFieldDef.Width-60,yPosGrid,sheetFieldDef.Width,heightGridTitle);
				g.FillRectangle(Brushes.White,rectangleF);
				StringFormat sf=new StringFormat();
				sf.Alignment=StringAlignment.Far;
				g.DrawString(text,new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),new SolidBrush(Color.Black),rectangleF,sf);
			}
			if(sheetFieldDef.FieldName=="StatementInvoicePayment") {
				RectangleF rectangleF=new RectangleF(sheetFieldDef.Width-sheetFieldDef.Width-60,yPosGrid,sheetFieldDef.Width,heightGridTitle);
				g.FillRectangle(Brushes.White,rectangleF);
				StringFormat stingFormat=new StringFormat();
				stingFormat.Alignment=StringAlignment.Far;
				if(PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd)) {
					g.DrawString("Total Payments & WriteOffs:  0.00",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),new SolidBrush(Color.Black),rectangleF,stingFormat);
				}
				else {
					g.DrawString("Total Payments:  0.00",new Font(FontFamily.GenericSansSerif,10,FontStyle.Bold),new SolidBrush(Color.Black),rectangleF,stingFormat);
				}
			}
			#endregion
			if(isSelected) {
				//Most dynamic grids do not modify the user set width.
				if(!(GetIsDynamicSheetType() || SheetDefs.IsDashboardType(_sheetDef)) 
					|| sheetFieldDef.GrowthBehavior==GrowthBehaviorEnum.FillDownFitColumns) 
				{
					listDisplayFieldColumns=SheetUtil.GetGridColumnsAvailable(sheetFieldDef.FieldName);
					sheetFieldDef.Width=0;
					for(int c=0;c<listDisplayFieldColumns.Count;c++) {
						sheetFieldDef.Width+=listDisplayFieldColumns[c].ColumnWidth;
					}
				}
				using Pen pen=new Pen(_colorRed,1.6f);
				g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);//hightlight selected
			}
			else if(GetIsDynamicSheetType()) {//Always highlight dynamic module grid defs so user can see min height and width.
				Color color=_colorGray;
				if(checkBlue.Checked){
					color=_colorBlue;
				}
				using Pen pen=new Pen(color);
				g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);//hightlight all dynamic
			}
		}

		private void DrawImages(SheetFieldDef sheetFieldDef,Graphics g) {
			string filePathAndName=ODFileUtils.CombinePaths(SheetUtil.GetImagePath(),sheetFieldDef.FieldName);
			Image image=null;
			try {
				image=new Bitmap(sheetFieldDef.ImageField);
			}
			catch {
				sheetFieldDef.ImageField=null;
			}
			if(sheetFieldDef.ImageField!=null) {
				//image was either not downloaded, or we failed to set the image variable, move to download the image again.
			}
			else if(sheetFieldDef.FieldName=="Patient Info.gif") {
				image=OpenDentBusiness.Properties.Resources.Patient_Info;
			}
			else if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && File.Exists(filePathAndName)) {
				image=Image.FromFile(filePathAndName);
				sheetFieldDef.ImageField?.Dispose();
				sheetFieldDef.ImageField=new Bitmap(image,sheetFieldDef.Width,sheetFieldDef.Height);
			}
			else if(CloudStorage.IsCloudStorage) {
				if(_listSheetFieldDefNums.Contains(sheetFieldDef.SheetFieldDefNum)) {
					//we've attempted to get this image already, do nothing, move along nothing to see here
					return;
				}
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText=Lan.g(CloudStorage.LanThis,"Downloading " + sheetFieldDef.FieldName + "...");
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload taskStateDownload=CloudStorage.DownloadAsync(SheetUtil.GetImagePath(),sheetFieldDef.FieldName,
					new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				_listSheetFieldDefNums.Add(sheetFieldDef.SheetFieldDefNum);
				if(formProgress.ShowDialog()==DialogResult.Cancel) {
					taskStateDownload.DoCancel=true;
					return;
				}
				if(taskStateDownload==null || taskStateDownload.FileContent==null) {
					//File wasn't downloaded, do nothing
				}
				else {
					using(MemoryStream memoryStream=new MemoryStream(taskStateDownload.FileContent)) {
						using(Image imageStream = Image.FromStream(memoryStream)) {
							image=new Bitmap(imageStream,sheetFieldDef.Width,sheetFieldDef.Height);
						}
						sheetFieldDef.ImageField=new Bitmap(image);//So it doesn't have to be downloaded again the next time.
					}
				}
			}
			else {
				if(ODBuild.IsDebug()) {
					Color color=GetOutlineColorForSheetFieldDef(sheetFieldDef,Color.IndianRed);
					using Pen pen=new Pen(color);
					g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
					g.DrawString(Lan.g(this,"Cannot find image")+": "+sheetFieldDef.FieldName,Font,Brushes.Black,sheetFieldDef.XPos,sheetFieldDef.YPos);
				}
				return;
			}
			g.DrawImage(image,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
			if(ODBuild.IsDebug()) {
				Color color=GetOutlineColorForSheetFieldDef(sheetFieldDef,Color.IndianRed);
				using Pen pen=new Pen(color);
				g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
			}
			image?.Dispose();
		}

		private void DrawInsurance(SheetFieldDef sheetFieldDef,Graphics g,Pen pen,SolidBrush solidBrush) {
			Bitmap bitmapIns=new Bitmap(sheetFieldDef.Width,sheetFieldDef.Height);
			Control controlIns;
			switch(sheetFieldDef.FieldName) {
				case "individualInsurance":
					controlIns=new DashIndividualInsurance();
					break;
				case "familyInsurance":
					controlIns=new DashFamilyInsurance();
					break;
				default:
					g.DrawString("("+sheetFieldDef.FieldName+")",Font,solidBrush,sheetFieldDef.XPos
						,sheetFieldDef.YPos);
					//draw rectangle on top of special so that user can see how big the field actually is.
					g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width
						,sheetFieldDef.Height);
					return;
			} //end switch
			controlIns.DrawToBitmap(bitmapIns,new Rectangle(0,0,sheetFieldDef.Width,sheetFieldDef.Height));
			Rectangle rectangleBound=OpenDentBusiness.SheetPrinting.DrawScaledImage(sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height,g,null,bitmapIns);
			Color color=GetOutlineColorForSheetFieldDef(sheetFieldDef,Color.LightGray);
			using Pen penOutline=new Pen(color);
			g.DrawRectangle(penOutline,rectangleBound); //outline insurance grid so user can see how much wasted space there is.
			bitmapIns.Dispose();
		}

		private void DrawLine(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color colorLine=sheetFieldDef.ItemColor;
			Color color=GetOutlineColorForSheetFieldDef(sheetFieldDef,colorLine);
			if(isSelected) {
				color=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorRed,true);
			}
			using Pen pen=new Pen(color);
			g.DrawLine(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.XPos+sheetFieldDef.Width,sheetFieldDef.YPos+sheetFieldDef.Height);
		}

		private void DrawPatImage(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,Color.Black);
			Color colorBrush=_colorGray;
			if(checkBlue.Checked){
				colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorBlue);
				colorBrush=_colorBlue;
			}
			if(isSelected) {
				colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorRed,true);
				colorBrush=_colorRed;
			}
			using Pen pen=new Pen(colorPen);
			using SolidBrush solidBrush=new SolidBrush(colorBrush);
			g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
			if(isSelected && listBoxFields.SelectedIndices.Count==1){
				Rectangle rectangleHandleLR=new Rectangle(sheetFieldDef.Bounds.Right-2,sheetFieldDef.Bounds.Bottom-2,
					_sizeResizeGrabHandle,_sizeResizeGrabHandle);
				using Pen pen2=new Pen(ColorOD.Gray(10));
				g.DrawRectangle(pen2,rectangleHandleLR);
			}
			g.DrawString("PatImage: "+Defs.GetName(DefCat.ImageCats,PIn.Long(sheetFieldDef.FieldName)),Font /*NOT _argsDF.font*/,solidBrush,sheetFieldDef.XPos+1,sheetFieldDef.YPos+1);
		}

		private void DrawRectangle(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color colorLine=sheetFieldDef.ItemColor;
			Color color=GetOutlineColorForSheetFieldDef(sheetFieldDef,colorLine);
			if(isSelected) {
				color=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorRed,true);
			}
			using Pen pen=new Pen(color);
			g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
		}

		///<summary>We need this special function to draw strings just like the RichTextBox control does, because sheet text is displayed using RichTextBoxes within FormSheetFillEdit.
		///Graphics.DrawString() uses a different font spacing than the RichTextBox control does.</summary>
		private void DrawRTFstring(SheetFieldDef field,string str,Font font,Brush brush,Graphics g) {
			str=str.Replace("\r",""); //For some reason '\r' throws off character position calculations.  \n still handles the CRs.
			//Font spacing is different for g.DrawString() as compared to RichTextBox and TextBox controls.
			//We create a RichTextBox here in the same manner as in FormSheetFillEdit, but we only use it to determine where to draw text.
			//We do not add the RichTextBox control to this form, because its background will overwrite everything behind that we have already drawn.
			bool doCalc=true;
			int index=_sheetDef.SheetFieldDefs.IndexOf(field);
			object[] arrayObjectData=(object[])_hashTableRtfStringCache[index.ToString()];
			if(arrayObjectData!=null) { //That field has been calculated
				//If any of the following factors change, then that could potentially change text positions.
				if(field.FontName.CompareTo(arrayObjectData[1])==0 //Has font name changed since last pass?
				   && field.FontSize.CompareTo(arrayObjectData[2])==0 //Has font size changed since last pass?
				   && field.FontIsBold.CompareTo(arrayObjectData[3])==0 //Has font boldness changed since last pass?
				   && field.Width.CompareTo(arrayObjectData[4])==0 //Has field width changed since last pass?
				   && field.Height.CompareTo(arrayObjectData[5])==0 //Has field height changed since last pass?
				   && str.CompareTo(arrayObjectData[6])==0 //Has field text changed since last pass?
				   && field.TextAlign.CompareTo(arrayObjectData[7])==0) //Has field text align changed since last pass?
				{
					doCalc=false; //Nothing has changed. Do not recalculate.
				}
			}
			if(doCalc) {
				//Data has not yet been cached for this text field, or the field has changed and needs to be recalculated.
				//All of these textbox fields are set using the similar logic as GraphicsHelper.CreateTextBoxForSheetDisplay(),
				//so that text in this form matches FormSheetFillEdit.
				RichTextBox richTextBox=new RichTextBox();
				richTextBox.Visible=false;
				richTextBox.BorderStyle=BorderStyle.None;
				richTextBox.ScrollBars=RichTextBoxScrollBars.None;
				richTextBox.SelectionAlignment=field.TextAlign;
				richTextBox.Location=new Point(field.XPos,field.YPos);
				richTextBox.Width=field.Width;
				richTextBox.Height=field.Height;
				richTextBox.Font=font;
				richTextBox.ForeColor=((SolidBrush)brush).Color;
				richTextBox.Multiline=GraphicsHelper.IsTextBoxMultiline(richTextBox,sheetFieldDef:field);
				richTextBox.Text=str;
				Point[] pointArrayPositions=new Point[str.Length];
				for(int j=0;j<str.Length;j++) {
					pointArrayPositions[j]=richTextBox.GetPositionFromCharIndex(j); //This line is slow, so we try to minimize calling it by chaching positions each time there are changes.
				}
				richTextBox.Dispose();
				arrayObjectData=new object[] {pointArrayPositions,field.FontName,field.FontSize,field.FontIsBold,field.Width,field.Height,str,field.TextAlign};
				_hashTableRtfStringCache[index.ToString()]=arrayObjectData;
			}
			Point[] pointArrayCharPositions=(Point[])arrayObjectData[0];
			if(PrefC.GetBool(PrefName.ImeCompositionCompatibility) && !IsWesternChars(str)) {
				//Only draw word by word if the user defined preference is set, and there is at least one character in the string that isn't a "Western" 
				//alphabet character.  Only use right-to-left formatting if the user's computer is set to a right-to-left culture.  This will preserve
				//desired formatting and spacing in as many scenarios as we can unless we later add a textbox specific setting for this type of formatting.
				DrawStringWordByWord(str,font,brush,field,CultureInfo.CurrentCulture.TextInfo.IsRightToLeft,pointArrayCharPositions,g);
			}
			else {
				DrawStringCharByChar(str,font,brush,field,pointArrayCharPositions,g);
			}
		}

		private void DrawSelectionRectangle(Graphics g) {
			if(!_hasClickedOnBlankSpace) {
				return;
			}
			g.DrawRectangle(Pens.Blue,
				//The math functions are used below to account for users clicking and dragging up, down, left, or right.
				x:Math.Min(_pointMouseOriginalPos.X,_pointMouseCurrentPos.X), 
				y:Math.Min(_pointMouseOriginalPos.Y,_pointMouseCurrentPos.Y),
				width:Math.Abs(_pointMouseCurrentPos.X-_pointMouseOriginalPos.X), 
				height:Math.Abs(_pointMouseCurrentPos.Y-_pointMouseOriginalPos.Y)); 
		}

		private void DrawAlignmentLines(Graphics g){
			if(!_isMouseDown) {
				return;
			}
			if(IsInternal) {
				return;
			}
			if(_isTabMode) {
				return;
			}
			if(!_isDragging){
				return;
			}
			List<SheetFieldDef> listSheetFieldDefsShowing=listBoxFields.Items.GetAll<SheetFieldDef>();
			List<SheetFieldDef> listSheetFieldDefsSelected=listBoxFields.GetListSelected<SheetFieldDef>();
			for(int i=0;i<listBoxFields.SelectedIndices.Count;i++) {
				for(int j=0;j<listSheetFieldDefsShowing.Count;j++) {
					if(listSheetFieldDefsSelected.Contains(listSheetFieldDefsShowing[j])){
						continue;//ignore all the other selected group items, including self
					}
					//left:
					if(listSheetFieldDefsShowing[j].XPos==listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].XPos){
						g.DrawLine(Pens.Blue,listSheetFieldDefsShowing[j].XPos,0,listSheetFieldDefsShowing[j].XPos,_sheetDef.Height*_sheetDef.PageCount);
					}
					//right:
					if(listSheetFieldDefsShowing[j].XPos+listSheetFieldDefsShowing[j].Width
						==listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].XPos+listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].Width)
					{
						g.DrawLine(Pens.Blue,listSheetFieldDefsShowing[j].XPos+listSheetFieldDefsShowing[j].Width,0,listSheetFieldDefsShowing[j].XPos+listSheetFieldDefsShowing[j].Width,_sheetDef.Height*_sheetDef.PageCount);
					}
					//top:
					if(listSheetFieldDefsShowing[j].YPos==listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].YPos){
						g.DrawLine(Pens.Blue,0,listSheetFieldDefsShowing[j].YPos,_sheetDef.Width,listSheetFieldDefsShowing[j].YPos);
					}
					//bottom:
					if(listSheetFieldDefsShowing[j].YPos+listSheetFieldDefsShowing[j].Height
						==listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].YPos+listSheetFieldDefsShowing[listBoxFields.SelectedIndices[i]].Height)
					{
						g.DrawLine(Pens.Blue,0,listSheetFieldDefsShowing[j].YPos+listSheetFieldDefsShowing[j].Height,_sheetDef.Width,listSheetFieldDefsShowing[j].YPos+listSheetFieldDefsShowing[j].Height);
					}
				}
			}
		}

		private void DrawSigBox(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorGray);
			Color colorBrush=_colorGray;
			if(isSelected) {
				colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorRed,true);
				colorBrush=_colorRed;
			}
			using Pen pen=new Pen(colorPen);
			using SolidBrush solidBrush=new SolidBrush(colorBrush);
			g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
			g.DrawString((sheetFieldDef.FieldType==SheetFieldType.SigBoxPractice ? "(practice signature box)" : "(signature box)"),Font,solidBrush,
				sheetFieldDef.XPos,sheetFieldDef.YPos);
		}

		private void DrawSpecial(SheetFieldDef sheetFieldDef,Graphics g,int sheetDefWidth,bool isSelected) {
			Color colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorGray);
			Color colorBrush=_colorGray;
			if(isSelected) {
				colorPen=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorRed,true);
				colorBrush=_colorRed;
			}
			using Pen pen=new Pen(colorPen);
			using SolidBrush solidBrush=new SolidBrush(colorBrush);
			Panel panel;
			switch(sheetFieldDef.FieldName) {
				#region toothChart
				case "toothChart":
					if(_imageToothChart!=null){
						Rectangle rectangleBoundingBox=OpenDentBusiness.SheetPrinting.DrawScaledImage(sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height,g,null,_imageToothChart);
						g.DrawRectangle(Pens.LightGray,rectangleBoundingBox); //outline tooth grid so user can see how much wasted space there is.
					}
					break;
				#endregion
				#region toothChartLegend
				case "toothChartLegend":
					List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
					int width;
					if(SheetDefs.IsDashboardType(_sheetDef)) {
						width=sheetFieldDef.Width;
					}
					else {
						width=sheetDefWidth;
					}
					OpenDentBusiness.SheetPrinting.DrawToothChartLegend(sheetFieldDef.XPos,sheetFieldDef.YPos,width,0,listDefs,g,null,SheetDefs.IsDashboardType(_sheetDef));
					break;
				case "familyInsurance":
				case "individualInsurance":
					DrawInsurance(sheetFieldDef,g,pen,solidBrush);
					break;
				#endregion
				#region ChartModuleTabs
				case "ChartModuleTabs":
					TabControl tabControlPrimary=new TabControl();
					tabControlPrimary.Width=sheetFieldDef.Width;
					tabControlPrimary.Height=sheetFieldDef.Height;
					FormOpenDental.S_Contr_TabProcPageTitles.ForEach(x => tabControlPrimary.TabPages.Add(x));
					tabControlPrimary.TabPages[0].Controls.Add(
						new Label() { Text="Controls Show Here",Location=new Point(0,0),AutoSize=true
					});
					tabControlPrimary.SelectTab(0);
					DrawControlForSheetField(g,tabControlPrimary,sheetFieldDef);
					break;
				#endregion
				#region TreatmentNotes
				case "TreatmentNotes":
					//Not using ODtextBox becuase RichTextBox.DrawToBitmap(...) does not work.
					//https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox.drawtobitmap?view=netframework-4.7.2
					//"This method is not relevant for this class."
					TextBox textBoxControlTreatNotes=new TextBox();
					textBoxControlTreatNotes.Multiline=true;
					textBoxControlTreatNotes.BackColor=Color.LightGray;
					textBoxControlTreatNotes.Text="Treatment notes";
					textBoxControlTreatNotes.Width=sheetFieldDef.Width;
					textBoxControlTreatNotes.Height=sheetFieldDef.Height;
					DrawControlForSheetField(g,textBoxControlTreatNotes,sheetFieldDef);
					break;
				#endregion
				#region TrackToothProcDates
				case "TrackToothProcDates":
					Label labelTrack=new Label();
					labelTrack.AutoSize=false;
					labelTrack.TextAlign=ContentAlignment.MiddleRight;
					labelTrack.Text="01/01/2000";
					labelTrack.Width=(int)(g.MeasureString(labelTrack.Text,labelTrack.Font).Width+5);
					labelTrack.Height=sheetFieldDef.Height;
					labelTrack.Location=new Point(0,0);
					TrackBar trackBarControl=new TrackBar();
					trackBarControl.AutoSize=false;
					trackBarControl.Width=(sheetFieldDef.Width-labelTrack.Width);
					trackBarControl.Height=sheetFieldDef.Height;
					trackBarControl.Location=new Point(labelTrack.Width+1,0);
					panel=new Panel();
					panel.Height=sheetFieldDef.Height;
					panel.Width=sheetFieldDef.Width;
					panel.Controls.Add(labelTrack);
					panel.Controls.Add(trackBarControl);
					DrawControlForSheetField(g,panel,sheetFieldDef);
					break;
				#endregion
				#region PanelEcw
				case "PanelEcw":
					Panel panelEcw=new Panel();
					panelEcw.BorderStyle=System.Windows.Forms.BorderStyle.FixedSingle;
					panelEcw.Width=sheetFieldDef.Width;
					if(sheetFieldDef.GrowthBehavior.In(GrowthBehaviorEnum.FillDown,GrowthBehaviorEnum.FillRightDown)) {
						panelEcw.Height=Math.Max(1,(panelMain.Height-sheetFieldDef.YPos));
					}
					else {
						panelEcw.Height=sheetFieldDef.Height;
					}
					Label panelLabel=new Label();
					panelLabel.Text="ECW Browser";
					panelLabel.Location=new Point((panelEcw.Width/2)-panelLabel.Width/2,(panelEcw.Height/2));
					panelEcw.Controls.Add(panelLabel);
					DrawControlForSheetField(g,panelLabel,sheetFieldDef);
					break;
				#endregion
				#region ErxAccess PhoneNums ForeignKey USAKey
				case "ButtonErxAccess":
				case "ButtonPhoneNums":
				case "ButtonForeignKey":
				case "ButtonUSAKey":
				case "ButtonNewTP":
					UI.Button button=new UI.Button();
					if(sheetFieldDef.FieldName=="ButtonNewTP") {
						button.Image=OpenDental.Properties.Resources.Add;
						button.ImageAlign=System.Drawing.ContentAlignment.MiddleLeft;
					}
					button.Enabled=true;
					button.Text=(sheetFieldDef.FieldValue);
					button.Size=new Size(sheetFieldDef.Width,sheetFieldDef.Height);
					DrawControlForSheetField(g,button,sheetFieldDef);
					break;
				#endregion
				#region SetPriorityListBox
				case "SetPriorityListBox":
					Label label=new Label();
					label.AutoSize=false;
					label.Size=new Size(sheetFieldDef.Width,15);
					label.Text="Set Priority";
					label.TextAlign=ContentAlignment.BottomLeft;
					label.Location=new Point(0,0);
					ListBoxOD listBox=new ListBoxOD();
					listBox.Items.Add(Lan.g("ContrChart","No Priority"));
					Defs.GetDefsForCategory(DefCat.TxPriorities,true).ForEach(def => listBox.Items.Add(def.ItemName));
					listBox.Width=sheetFieldDef.Width;
					listBox.Top=label.Bottom;
					panel=new Panel();
					panel.Width=sheetFieldDef.Width;
					if(sheetFieldDef.GrowthBehavior.In(GrowthBehaviorEnum.FillDown,GrowthBehaviorEnum.FillRightDown)) {
						panel.Height=Math.Max(1,(panelMain.Height-sheetFieldDef.YPos)-1);
					}
					else {
						panel.Height=sheetFieldDef.Height;
					}
					int rowCount=((panel.Height-label.Height)/Font.Height);//Truncate to the nearest row, just as the listBox does internally.
					listBox.Height=rowCount*Font.Height;
					panel.Height=Math.Max(1,listBox.Bottom-8);//-8 because with several test values in design mode we noticed the panel was always a fixed value too large.
					panel.Controls.Add(label);
					panel.Controls.Add(listBox);
					DrawControlForSheetField(g,panel,sheetFieldDef);
					break;
				#endregion
				default:
					g.DrawString("(Special:Tooth Grid)",Font,solidBrush,sheetFieldDef.XPos,sheetFieldDef.YPos);
					break;
			} //end switch
			//draw rectangle on top of special so that user can see how big the field actually is.
			g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
		}

		///<summary>Draws the string one char at a time, using specified positions for each char.</summary>
		private void DrawStringCharByChar(string str,Font font,Brush brush,SheetFieldDef sheetFieldDef,Point[] pointArrayCharPositions,Graphics g) {
			//This will draw text below the bottom line if the text is long. This is by design, so the user can see that the text is too big.
			for(int j=0;j<pointArrayCharPositions.Length;j++) {
				g.DrawString(str.Substring(j,1),font,brush,sheetFieldDef.Bounds.X+pointArrayCharPositions[j].X,sheetFieldDef.Bounds.Y+pointArrayCharPositions[j].Y);
			}
		}

		///<summary>Draws input, static, and output.</summary>
		private void DrawStringField(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			FontStyle fontStyle=FontStyle.Regular;
			if(sheetFieldDef.FontIsBold) {
				fontStyle=FontStyle.Bold;
			}
			using Font font=new Font(sheetFieldDef.FontName,sheetFieldDef.FontSize,fontStyle,GraphicsUnit.Point);
			Color colorBrush;
			if(isSelected) {
				Color color=GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorRedOutline,isSelected:true);
				colorBrush=_colorRed;
				using Pen pen=new Pen(color);
				g.DrawRectangle(pen,sheetFieldDef.Bounds);
			}
			else {
				Color color = GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorGrayOutline);
				colorBrush=_colorGray;
				if(checkBlue.Checked){
					color = GetOutlineColorForSheetFieldDef(sheetFieldDef,_colorBlueOutline);
					colorBrush=_colorBlue;
				}
				using Pen pen = new Pen(color);
				g.DrawRectangle(pen,sheetFieldDef.Bounds);
			}
			if(isSelected && listBoxFields.SelectedIndices.Count==1){
				Rectangle rectangleHandleLR=new Rectangle(sheetFieldDef.Bounds.Right-2,sheetFieldDef.Bounds.Bottom-2,
					_sizeResizeGrabHandle,_sizeResizeGrabHandle);
				using Pen pen=new Pen(ColorOD.Gray(10));
				g.DrawRectangle(pen,rectangleHandleLR);
			}
			string str;
			if(sheetFieldDef.FieldType==SheetFieldType.StaticText) {
				str=sheetFieldDef.FieldValue;
				//Static text can have a custom color.
				//Check to see if this text box is selected.  If it is, do not change the color.
				if(!isSelected) {
					colorBrush=sheetFieldDef.ItemColor;
				}
			}
			else {
				str=sheetFieldDef.FieldName;
			}
			using SolidBrush solidBrush=new SolidBrush(colorBrush);
			DrawRTFstring(sheetFieldDef,str,font,solidBrush,g);
		}

		///<summary>Draws the string one word at a time, using specified positions of the first char of each word.  This is important when a language is in use that uses words and/or characters composed from multiple characters, i.e. Arabic, Korean, etc.</summary>
		private void DrawStringWordByWord(string str,Font font,Brush brush,SheetFieldDef sheetFieldDef,bool isRightToLeft,Point[] pointArrayCharPositions,Graphics g) {
			StringFormat stringFormat=new StringFormat();
			if(isRightToLeft) {
				stringFormat.FormatFlags|=StringFormatFlags.DirectionRightToLeft;
			}
			List<string> listWords=SplitStringByWhitespace(str);
			int posIndex=0;
			//This will draw text below the bottom line if the text is long. This is by design, so the user can see that the text is too big.
			foreach(string strCur in listWords) {
				float xPos=sheetFieldDef.Bounds.X+pointArrayCharPositions[posIndex].X;
				float yPos=sheetFieldDef.Bounds.Y+pointArrayCharPositions[posIndex].Y;
				g.DrawString(strCur,font,brush,xPos,yPos,stringFormat);
				posIndex+=strCur.Length;
			}
		}

		private void DrawTabMode(SheetFieldDef sheetFieldDef,Graphics g,bool isHighlightEligible) {
			if(!_isTabMode || !sheetFieldDef.FieldType.In(SheetFieldType.InputField,SheetFieldType.ComboBox)) {
				return;
			}
			Rectangle rectangleTab=new Rectangle(sheetFieldDef.XPos-1, //X
				sheetFieldDef.YPos-1, //Y
				(int)g.MeasureString(sheetFieldDef.TabOrder.ToString(),_fontTabOrder).Width+1, //Width
				12); //height
			if(isHighlightEligible && _listSheetFieldDefsTabOrder.Contains(sheetFieldDef)) { //blue border, white box, blue letters
				g.FillRectangle(Brushes.White,rectangleTab);
				g.DrawRectangle(Pens.Blue,rectangleTab);
				g.DrawString(sheetFieldDef.TabOrder.ToString(),_fontTabOrder,Brushes.Blue,rectangleTab.X,rectangleTab.Y-1);
			}
			else { //Blue border, blue box, white letters
				g.FillRectangle(Brushes.Blue,rectangleTab);
				g.DrawString(sheetFieldDef.TabOrder.ToString(),_fontTabOrder,Brushes.White,rectangleTab.X,rectangleTab.Y-1);
			}
		}

		private void DrawToothChart(SheetFieldDef sheetFieldDef,Graphics g,bool isSelected) {
			Color color=_colorGray;
			if(isSelected) {
				color=_colorRed;
			}
			using Pen pen=new Pen(color);
			using SolidBrush solidBrush=new SolidBrush(color);
			g.DrawRectangle(pen,sheetFieldDef.XPos,sheetFieldDef.YPos,sheetFieldDef.Width,sheetFieldDef.Height);
			string toothChart="("+Lan.g(this,"tooth chart")+" "+sheetFieldDef.FieldName;
			if(sheetFieldDef.FieldValue[0]=='1') {//Primary teeth chart
				toothChart+=" "+Lan.g(this,"primary teeth");
			}
			else {//Permanent teeth chart
				toothChart+=" "+Lan.g(this,"permanent teeth");
			}
			toothChart+=")";
			g.DrawString(toothChart,Font,solidBrush,sheetFieldDef.XPos,sheetFieldDef.YPos);
		}

		///<summary>Returns a color to be used when we want to consider the defs language value.</summary>
		private Color GetOutlineColorForSheetFieldDef(SheetFieldDef sheetFieldDef,Color colorDefault,bool isSelected=false){
			if(sheetFieldDef.Language.IsNullOrEmpty()){//Always use defaultPen when working with a non-translated def.
				return colorDefault;
			}
			if(_sheetDef.SheetFieldDefs.Any(x => x.Language.IsNullOrEmpty() && IsEquivilantSheetDef(x,sheetFieldDef))){
				return _colorGray;
			}
			if(isSelected){
				return _colorRed;
			}
			return Color.Green;
		}

		///<summary>Does not check for vertical overlap.
		///Returns true if targetFieldDef is above the given baseFieldDef such that the YPos is less and the XPos is within baseFieldDefs range.
		///Otherwise returns false.</summary>
		private bool IsSheetFieldDefAbove(SheetFieldDef sheetFieldDef,SheetFieldDef sheetFieldDefOther) {
			//Logic mimics SheetUserControl.IsControlAbove(...)
			return (sheetFieldDefOther.YPos<sheetFieldDef.YPos 
				&& (sheetFieldDefOther.XPos.Between(sheetFieldDef.XPos,(sheetFieldDef.XPos+sheetFieldDef.Width))
				|| (sheetFieldDefOther.XPos+sheetFieldDefOther.Width).Between(sheetFieldDef.XPos,(sheetFieldDef.XPos+sheetFieldDef.Width)))
			);
		}

		private bool IsSheetFieldDefBelow(SheetFieldDef sheetFieldDef,SheetFieldDef sheetFieldDefOther) {
			return (sheetFieldDefOther.YPos>sheetFieldDef.YPos
				&& ((sheetFieldDefOther.XPos.Between(sheetFieldDef.XPos,(sheetFieldDef.XPos+sheetFieldDef.Width))
				|| (sheetFieldDefOther.XPos+sheetFieldDefOther.Width).Between(sheetFieldDef.XPos,(sheetFieldDef.XPos+sheetFieldDef.Width))
				|| sheetFieldDef.XPos.Between(sheetFieldDefOther.XPos,sheetFieldDefOther.XPos+sheetFieldDefOther.Width)
				))
			);
		}

		private static bool IsWesternChars(string inputstring) {
			Regex regex=new Regex(@"[A-Za-z0-9 ?'.,-_=+(){}\[\]\\\t\r\n]");
			MatchCollection matchCollection=regex.Matches(inputstring);
			if(matchCollection.Count.Equals(inputstring.Length)) {
				return true;
			}
			return false;
		}

		private void SetToothChartImage() {
			if(_imageToothChart!=null){
				return;//only designed to run once
			}
			SparksToothChart.ToothChartWrapper toothChartWrapper=new SparksToothChart.ToothChartWrapper();
			ToothChartRelay toothChartRelay= new ToothChartRelay();
			toothChartRelay.SetToothChartWrapper(toothChartWrapper);
			Control controlToothChart=null;//the Sparks3D tooth chart
			if(ToothChartRelay.IsSparks3DPresent){
				controlToothChart=toothChartRelay.GetToothChart();
				controlToothChart.Location=new Point(0,0);
				controlToothChart.Size=new Size(500,370);
				controlToothChart.Visible=true;
				panelLeft.Controls.Add(controlToothChart);
				controlToothChart.BringToFront();
			}
			else{
				toothChartWrapper.Size=new Size(500,370);
				toothChartWrapper.UseHardware=ComputerPrefs.LocalComputer.GraphicsUseHardware;
				toothChartWrapper.PreferredPixelFormatNumber=ComputerPrefs.LocalComputer.PreferredPixelFormatNum;
				//Must be last preference set, last so that all settings are caried through in the reinitialization this line triggers.
				if(ComputerPrefs.LocalComputer.GraphicsSimple==DrawingMode.Simple2D) {
					toothChartWrapper.DrawMode=DrawingMode.Simple2D;
				}
				else if(ComputerPrefs.LocalComputer.GraphicsSimple==DrawingMode.DirectX) {
					toothChartWrapper.DeviceFormat=new SparksToothChart.ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);
					toothChartWrapper.DrawMode=DrawingMode.DirectX;
				}
				else{
					toothChartWrapper.DrawMode=DrawingMode.OpenGL;
				}
				//The preferred pixel format number changes to the selected pixel format number after a context is chosen.
				ComputerPrefs.LocalComputer.PreferredPixelFormatNum=toothChartWrapper.PreferredPixelFormatNumber;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			toothChartRelay.SetToothNumberingNomenclature((ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			toothChartRelay.ColorBackgroundMain=listDefs[14].ItemColor;
			toothChartRelay.ColorText=listDefs[15].ItemColor;
			toothChartRelay.ResetTeeth();
			toothChartRelay.EndUpdate();
			//toothChartRelay.AutoFinish=true;/??
			//no need to dispose beforehand, since this function currently only runs once
			_imageToothChart=toothChartRelay.GetBitmap();
			toothChartWrapper.Dispose();
			if(ToothChartRelay.IsSparks3DPresent){
				controlToothChart.Visible=false;
				panelLeft.Controls.Remove(controlToothChart);
				controlToothChart.Dispose();//only local scope, anyway
			}
			//return imageRetVal;
		}

		/// <summary>Splits up a string into a list of "words" using whitespace as the delimiter.  The whitespace will be included as individual words if
		/// isWhitespaceIncluded is set to true.</summary>
		private List<string> SplitStringByWhitespace(string str,bool isWhitespaceIncluded=true) {
			List<string> listWords=new List<string>();
			StringBuilder stringBuilder=new StringBuilder();
			foreach(char c in str.ToCharArray()) {
				if(!char.IsWhiteSpace(c)) {//Includes tab character.
					stringBuilder.Append(c);
				}
				else {
					if(stringBuilder.Length>0) {//If we encounter consecutive white space characters in a row.
						listWords.Add(stringBuilder.ToString());
					}
					if(isWhitespaceIncluded) {
						listWords.Add(c.ToString());
					}
					stringBuilder.Clear();
				}
			}
			if(stringBuilder.Length>0) {
				listWords.Add(stringBuilder.ToString());//Add the last word.
			}
			return listWords;
		}
		#endregion Methods - Draw

		#region Classes - Nested
		public class UndoLevel{
			///<summary>We do not store the fields as part of this sheetDef because we want to have control over how copies are made.</summary>
			public SheetDef SheetDef_;
			public List<SheetFieldDef> ListSheetFieldDefs;
			public string Description;
			public override string ToString() {
				return Description;
			}
		}




		#endregion Classes - Nested

		
	}

	#region Classes
	internal class SheetDefLanguage {
		public string ThreeLetters { get; set; }
		public string Display { get; set; }

		public SheetDefLanguage(string threeLetters,string display){
			ThreeLetters=threeLetters;
			Display=display;
		}
	}
	#endregion Classes
}