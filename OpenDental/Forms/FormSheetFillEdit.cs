using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.Thinfinity;
using PdfSharp.Drawing;

namespace OpenDental {
	public delegate void SaveStatementToDocDelegate(Statement stmt,Sheet sheet,DataSet dataSet,string pdfFileName="");
	public partial class FormSheetFillEdit:FormODBase {
		///<summary>Will be null if deleted.</summary>
		public Sheet SheetCur;
		private bool _isMouseDown;
		///<summary>A list of points for a line currently being drawn.  Once the mouse is raised, this list gets cleared.</summary>
		private List<Point> _listPoints;
		///<summary>This holds the main background.</summary>
		private PictureBox pictureBoxMain;
		///<summary>This seems to be the main background that's in pictureBoxMain.</summary>
		private Image _imageDraw;
		public bool IsRxControlled;
		///<summary>When in terminal, some options are not visible.</summary>
		public bool IsInTerminal;
		///<summary>Only used here to draw the dashed margin lines.</summary>
		private Margins _marginsPrint=new Margins(0,0,40,60);
		public bool IsStatement;//Used for statements, do not save a sheet version of the statement.
		public Statement StatementCur;
		public MedLab MedLabCur;
		///<summary>Statements use Sheets needs access to the entire Account data set for measuring grids.  See RefreshPanel()</summary>
		private DataSet _dataSet;
		///<summary>If true, the sheet cannot be edited, deleted, changed patient, printed, or PDFed.
		///The main goal of this setting is to stop the user from being able to do anything with the sheet except view it.
		///It is mainly used when importing web forms so that the user importing the forms can make better informed decisions.</summary>
		public bool IsReadOnly;
		///<summary>True if the user sent an email from this window.</summary>
		public bool HasEmailBeenSent;
		///<summary>A method that will be invoked when printing/email/creating PDF of a statement.</summary>
		public SaveStatementToDocDelegate SaveStatementToDocDelegate;
		private List<SignatureBoxWrapper> _listSignatureBoxWrappers=new List<SignatureBoxWrapper>();
		///<summary>The location where the PDF file has been created.</summary>
		private string _tempPdfFile="";
		///<summary>Creates a unique identifier for this instance of the form. This can be used when creating a thread with a unique group name.</summary>
		private string _uniqueFormIdentifier;
		///<summary>True if the user is auto-saving a patient form.</summary>
		private bool _isAutoSave;
		///<summary>True if export as CSV was checked in FormStatementOptions.cs</summary>
		private bool _doExportCSV;
		///<summary>Indicates to the calling form that the sheet was inserted/updated.</summary>
		public bool DidChangeSheet;
		///<summary>Used to change the text on the "Save" button of the form.</summary>
		private Timer _timerSaveButtonText;

		public void ForceClose() {
			if(this.IsDisposed) {
				return;
			}
			try {
				DialogResult=DialogResult.Cancel;
				Close();
				Dispose(true);
			}
			catch(Exception) { }
		}

		public static void ShowForm(Sheet sheet,FormClosingEventHandler formClosingEventHandler=null,bool isReadOnly=false) {
			FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheet);
			if(formClosingEventHandler!=null) {
				formSheetFillEdit.FormClosing+=formClosingEventHandler;
			}
			formSheetFillEdit.IsReadOnly=isReadOnly;
			formSheetFillEdit.Show();
		}

		///<summary>Use this constructor when displaying a statement.  dataSet should be filled with the data set from AccountModules.GetAccount()</summary>
		public FormSheetFillEdit(Sheet sheet,DataSet dataSet=null,bool doExportCSV=false){
			InitializeComponent();
			InitializeLayoutManager();
			MouseWheel+=FormSheetFillEdit_MouseWheelScroll;
			Lan.F(this);
			SheetCur=sheet;
			_dataSet=dataSet;
			_doExportCSV=doExportCSV;
		}

		private void FormSheetFillEdit_Load(object sender,EventArgs e) {
			if(SheetCur.IsLandscape){
				Width=Math.Max(SheetCur.Height+190,butOK.Right+27);
				Height=SheetCur.Width+65;
			}
			else{
				Width=Math.Max(SheetCur.Width+190,butOK.Right+27);
				Height=SheetCur.Height+65;
			}
			if(Width>System.Windows.Forms.Screen.FromControl(this).WorkingArea.Width){
				Width=System.Windows.Forms.Screen.FromControl(this).WorkingArea.Width;
			}
			if(Height>System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height){
				Height=System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height;
			}
			CenterFormOnMonitor();
			//Only allow Autosave when there is a non-hidden image category that is flagged for Autosave Form (U) and the sheet being filled out is not in a terminal.
			_isAutoSave=Defs.GetDefsForCategory(DefCat.ImageCats,isShort:true).Any(x => x.ItemValue.Contains("U"));
			if(_isAutoSave && !IsInTerminal) {//only visible if the Autosave Form usage has been set and is not in kiosk mode
				//This will get set depending on the SheetDef for the current Sheet
				SheetDef sheetDefForSheetCur=SheetDefs.GetFirstOrDefault(x=>x.SheetDefNum==SheetCur.SheetDefNum);
				checkSaveToImages.Checked=sheetDefForSheetCur?.AutoCheckSaveImage??false;
				checkSaveToImages.Visible=true;
			}
			if(SheetCur.PatNum==0) {
				checkSaveToImages.Enabled=false;
				checkSaveToImages.Checked=false;
			}
			_listPoints=new List<Point>();
			_uniqueFormIdentifier=MiscUtils.CreateRandomAlphaNumericString(15);//Thread safe random
			Sheets.SetPageMargin(SheetCur,_marginsPrint);
			if(IsInTerminal) {
				labelDateTime.Visible=false;
				textDateTime.Visible=false;
				labelDescription.Visible=false;
				textDescription.Visible=false;
				labelNote.Visible=false;
				textNote.Visible=false;
				labelShowInTerminal.Visible=false;
				textShowInTerminal.Visible=false;
				butToKiosk.Visible=false;
				butPrint.Visible=false;
				butPDF.Visible=false;
				butDelete.Visible=false;
				butChangePat.Visible=false;
				butSimplePrint.Visible=false;
				butEmail.Visible=false;
				butAddField.Visible=false;
				MinimizeBox=false;
				MaximizeBox=false;
				this.TopMost=true;
			}
			if(SheetCur.IsLandscape){
				LayoutManager.MoveWidth(panelMain,SheetCur.Height);//+20 for VScrollBar
				LayoutManager.MoveHeight(panelMain,SheetCur.Width);
			}
			else{
				LayoutManager.MoveWidth(panelMain,SheetCur.Width);
				LayoutManager.MoveHeight(panelMain,SheetCur.Height);
			}
			if(IsStatement) {
				labelDateTime.Visible=false;
				textDateTime.Visible=false;
				labelDescription.Visible=false;
				textDescription.Visible=false;
				labelNote.Visible=false;
				textNote.Visible=false;
				labelShowInTerminal.Visible=false;
				textShowInTerminal.Visible=false;
				butToKiosk.Visible=false;
				if(!ODBuild.IsDebug()) {
					butPrint.Visible=false;
					butPDF.Visible=false;
				}
				butDelete.Visible=false;
				butOK.Visible=false;
				butChangePat.Visible=false;
				checkErase.Visible=false;
				butAddField.Visible=false;
				butCancel.Text="Close";
			}
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {//hide buttons if PP sheet type
				labelShowInTerminal.Visible=false;
				textShowInTerminal.Visible=false;
				butToKiosk.Visible=false;
				textNote.Visible=false;
				labelNote.Visible=false;
				butOK.Visible=false;
				butDelete.Visible=false;
				butChangePat.Visible=false;
				butSave.Visible=true;
				butAddField.Visible=false;
			}
			//Some controls may be on subsequent pages if the SheetFieldDef is multipage.
			LayoutManager.MoveHeight(panelMain,Math.Max(SheetCur.HeightPage,SheetCur.HeightLastField+20));//+20 for Hscrollbar
			textDateTime.Text=SheetCur.DateTimeSheet.ToShortDateString()+" "+SheetCur.DateTimeSheet.ToShortTimeString();
			textDescription.Text=SheetCur.Description;
			textNote.Text=SheetCur.InternalNote;
			if(SheetCur.ShowInTerminal>0) {
				textShowInTerminal.Text=SheetCur.ShowInTerminal.ToString();
			}
			try {
				string strErr=LayoutFields();
				if(!string.IsNullOrWhiteSpace(strErr)) {
					MsgBox.Show(this,strErr);//An invalid SheetField was repaired.
				}
			}
			catch(Exception ex) {
				//If LayoutFields throws, the sheet is invalid. Tell the user and additionally prompt the user to delete the form if they have the 
				//correct permissions and it is not read only mode.
				string message="The sheet that you are trying to edit is invalid. ";
				if(Security.IsAuthorized(Permissions.SheetDelete,true) && !IsReadOnly) {
					message+="Would you like to delete this sheet?";
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
						DeleteSheet(false);
						return;
					}
				}
				else {//The user does not have the permission. Show them the message and return.
					FriendlyException.Show(Lans.g(this,message),ex);
				}
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			if(IsReadOnly) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
				butChangePat.Enabled=false;
				butPrint.Enabled=false;
				butPDF.Enabled=false;
				butSimplePrint.Enabled=false;
				butEmail.Enabled=false;
				butToKiosk.Enabled=false;
				butAddField.Enabled=false;
			}
			if(IsStatement) {
				SelectFirstOptionComboBoxes();
			}
			//If it is an exam and has the permission to save it, enable the save button.
			butSave.Visible=(SheetCur.SheetType==SheetTypeEnum.ExamSheet && butOK.Enabled && butOK.Visible && !IsReadOnly);
			if(SheetCur.IsNew && SheetCur.SheetType!=SheetTypeEnum.PaymentPlan) {//payplan does not get saved to db so sheet is always new
				butChangePat.Enabled=false;
				return;
			}
			if(SheetCur.PatNum!=0 && !Security.IsAuthorized(Permissions.SheetDelete,SheetCur.DateTimeSheet,true,true,0,-1,SheetCur.SheetDefNum,0)) {
				butDelete.Enabled=false;
			}
			if(SheetCur.IsDeleted && !IsStatement && !IsInTerminal) {
				butDelete.Visible=false;
				butRestore.Visible=true;
			}
			List<SheetFieldType> listSheetFieldTypes=SheetDefs.GetVisibleButtons(SheetCur.SheetType);
			//This is just temporary. We can currently only add PatImages in that window, but we plan to add more types later
			if(!listSheetFieldTypes.Contains(SheetFieldType.PatImage)) {
				butAddField.Visible=false;
			}
			//from here on, only applies to existing sheets.
			if(!Security.IsAuthorized(Permissions.SheetEdit,SheetCur.DateTimeSheet,false,false,0,-1,SheetCur.SheetDefNum,0)) {
				butSave.Visible=false;
				panelMain.Enabled=false;
				butOK.Enabled=false;
				butChangePat.Enabled=false;
				return;
			}
			//So user has permission
			bool isSigned=false;
			for(int i=0;i<SheetCur.SheetFields.Count;i++) {
				if(SheetCur.SheetFields[i].FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)
					&& SheetCur.SheetFields[i].FieldValue.Length>1) 
				{
					isSigned=true;
					break;
				}
			}
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
				PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(SheetCur.Parameters,"payplan").ParamValue;
				if(payPlan.Signature!="" && (payPlan.Signature!=null || !payPlan.IsNew)) {
					for(int i=0;i<panelMain.Controls.Count;i++){
						if(panelMain.Controls[i].GetType()!=typeof(SignatureBoxWrapper)) {
							continue;
						}
						if(panelMain.Controls[i].Tag==null) {
							continue;
						}
						//SheetField field;
						//field=(SheetField)control.Tag;
						OpenDental.UI.SignatureBoxWrapper signatureBoxWrapper=(OpenDental.UI.SignatureBoxWrapper)panelMain.Controls[i];
						butUnlock.Visible=true;
						butSave.Visible=false;
						signatureBoxWrapper.Enabled=false;
					}
				}
			}
			if(isSigned) {
				panelMain.Enabled=false;
				butUnlock.Visible=true;
			}
			Plugins.HookAddCode(this, "FormSheetFillEdit_Load_End");
		}

		private void FormSheetFillEdit_Resize(object sender, EventArgs e){
			Size sizePanel=new Size(LayoutManager.Scale(SheetCur.Width),LayoutManager.Scale(SheetCur.Height));
			if(SheetCur.IsLandscape){
				sizePanel=new Size(LayoutManager.Scale(SheetCur.Height),LayoutManager.Scale(SheetCur.Width));
			}
			sizePanel.Height=LayoutManager.Scale(Math.Max(SheetCur.HeightPage,SheetCur.HeightLastField+20));//+20 for Hscrollbar
			//If the user has scrolled down/right inside of panelScroll, panelMain's initial location would be negative as it is currently above/left of the current view inside of the form.  Redrawing this down the page by resetting panelMain's location to point 0,0 created a large gap of white space above the newly drawn location.  Resetting the scroll position prevents this.
			panelScroll.VerticalScroll.Value=0;
			panelScroll.HorizontalScroll.Value=0;
			LayoutManager.Move(panelMain,new Rectangle(new Point(0,0),sizePanel));
		}

		private void FormSheetFillEdit_MouseWheelScroll(object sender,MouseEventArgs e) {
			panelScroll.Focus();
		}

		///<summary>Runs as the final step of loading the form, and also immediately after fields are moved down due to growth.  Returns an error message
		///if a SheetField was found in an invalid state and a repair was possible.</summary>
		private string LayoutFields() {
			string strErr=string.Empty;
			_listSignatureBoxWrappers=new List<SignatureBoxWrapper>();
			List<SheetField> listSheetFieldsSorted=new List<SheetField>();
			listSheetFieldsSorted.AddRange(SheetCur.SheetFields);//Creates a sortable list that will not change sort order of the original list.
			listSheetFieldsSorted.Sort(SheetFields.SortDrawingOrderLayers);
			//Dispose panelMain's controls before clearing them, or else they won't get disposed.
			//Do the disposal in reverse order so anything being removed from the list won't break the loop
			for(int i=panelMain.Controls.Count-1;i>=0;i--) {
				if(panelMain.Controls[i] is PictureBox pictureBox){
					pictureBox.Image?.Dispose();
				}
				panelMain.Controls[i]?.Dispose();
			}
			for(int i=0;i<panelMain.Controls.Count;i++){
				panelMain.Controls[i].Dispose();
			}
			panelMain.Controls.Clear();
			RichTextBox richTextBox;//has to be richtextbox due to MS bug that doesn't show cursor.
			//System.Windows.Forms.Integration.ElementHost elementHost;
			SheetCheckBox sheetCheckBox;
			SheetComboBox sheetComboBox;
			ScreenToothChart screenToothChart;
			//first, draw images---------------------------------------------------------------------------------------
			//might change this to only happen once when first loading form:
			if(pictureBoxMain!=null) {
				if(panelMain.Controls.Contains(pictureBoxMain)) {
					panelMain.Controls.Remove(pictureBoxMain);
				}
				pictureBoxMain.Image.Dispose();
				pictureBoxMain.Dispose();
				pictureBoxMain=null;
			}
			_imageDraw?.Dispose();
			_imageDraw=null;
			pictureBoxMain=new PictureBox();
			if(SheetCur.IsLandscape) {
				//imgDraw=new Bitmap(SheetCur.Height,SheetCur.Width);
				pictureBoxMain.Width=SheetCur.Height;
				pictureBoxMain.Height=SheetCur.Width;
			}
			else {
				//imgDraw=new Bitmap(SheetCur.Width,SheetCur.Height);
				pictureBoxMain.Width=SheetCur.Width;
				pictureBoxMain.Height=SheetCur.Height;
			}
			if(Sheets.CalculatePageCount(SheetCur,_marginsPrint)==1){
				pictureBoxMain.Height=SheetCur.HeightPage;//+10 for HScrollBar
			}
			else {
				int pageCount=0;
				pictureBoxMain.Height=OpenDentBusiness.SheetPrinting.BottomCurPage(SheetCur.HeightLastField,SheetCur,out pageCount);
			}
			//imgDraw.Dispose();//dispose of old image before setting it to a new image.
			_imageDraw=new Bitmap(pictureBoxMain.Width,pictureBoxMain.Height);
			pictureBoxMain.Location=new Point(0,0);
			pictureBoxMain.Image=(Image)_imageDraw.Clone();
			pictureBoxMain.SizeMode=PictureBoxSizeMode.StretchImage;
			LayoutManager.Add(pictureBoxMain,panelMain);
			panelMain.SendToBack();
			using Graphics graphicsPicture=Graphics.FromImage(_imageDraw);
			int yPosPrint=0;
			OpenDentBusiness.SheetPrinting.DrawImages(SheetCur,graphicsPicture,true,ref yPosPrint);
			//Set mouse events for the pictDraw
			pictureBoxMain.MouseDown+=new MouseEventHandler(pictDraw_MouseDown);
			pictureBoxMain.MouseMove+=new MouseEventHandler(pictDraw_MouseMove);
			pictureBoxMain.MouseUp+=new MouseEventHandler(pictDraw_MouseUp);
			//draw drawings, rectangles, and lines, special, and grids.-----------------------------------------------------------------------
			RefreshPanel();
			//PatImages---------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.PatImage){
					continue;
				}
				Bitmap bitmap=null;
				if(SheetCur.SheetFields[i].FieldValue.StartsWith("MountNum:")){
					long mountNum=PIn.Long(SheetCur.SheetFields[i].FieldValue.Substring(9));
					bitmap=MountHelper.GetBitmapOfMountFromDb(mountNum);
				}
				else if(SheetCur.SheetFields[i].FieldValue!=""){
					long docNum=PIn.Long(SheetCur.SheetFields[i].FieldValue);
					bitmap=ImageHelper.GetBitmapOfDocumentFromDb(docNum);
				}
				PictureBox pictureBox=new PictureBox();
				pictureBox.Location=new Point(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos);
				pictureBox.Width=SheetCur.SheetFields[i].Width;
				pictureBox.Height=SheetCur.SheetFields[i].Height;
				pictureBox.Tag=SheetCur.SheetFields[i];
				pictureBox.SizeMode=PictureBoxSizeMode.Zoom;//this is centered instead of UL, but I think that's acceptable.
				pictureBox.DoubleClick+=PictureBox_DoubleClick;
				if(bitmap is null){
					pictureBox.BorderStyle=BorderStyle.FixedSingle;
				}
				pictureBox.Image=bitmap;//picturebox will handle dispose
				LayoutManager.Add(pictureBox,panelMain);
				pictureBox.BringToFront();
				if(bitmap is null){
					Label label=new Label();
					label.Text=Lan.g(this,"Double click to add an image.");
					label.TextAlign=ContentAlignment.MiddleCenter;
					label.Width=pictureBox.Width-4;
					label.Height=pictureBox.Height-4;
					label.Top=pictureBox.Top+2;
					label.Left=pictureBox.Left+2;
					label.Tag=SheetCur.SheetFields[i];
					label.DoubleClick+=PictureBox_DoubleClick;
					LayoutManager.Add(label,panelMain);
					label.BringToFront();
				}
			}
			//draw textboxes----------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.InputField
					&& SheetCur.SheetFields[i].FieldType!=SheetFieldType.OutputText
					&& SheetCur.SheetFields[i].FieldType!=SheetFieldType.StaticText)
				{
					continue;
				}
				if(SheetCur.SheetFields[i].FontSize<=0) {//Sheet did not save correctly.
					SheetCur.SheetFields[i].FontSize=SheetCur.FontSize;//Use a best guess so the user does not completely lose this Sheet's data.
					strErr=$"A text SheetCur.SheetFields[i] was found with an invalid FontSize.  Default size of {SheetCur.FontSize} has been used instead.";
				}
				//WPF RichTextBoxes are no longer being used for kiosk mode, because of indeterminate behavior where selecting textbox input fields would not cause the On-Screen keyboard to
				//pop up when using touch devices such as tablets. Switching back to only using System.Windows.Forms.RichTextBox as they always cause the keyboard to appear. 
				//WPF RichTextBoxes were first introduced here because WinForm RichTextBoxes wouldn't cause the keyboard to appear.
				/*if(IsInTerminal && ODEnvironment.IsTabletMode) {
					#region WPF RichTextBox
					elementHost=CreateWpfRichTextBoxForSheetDisplay(SheetCur.SheetFields[i]);
					((System.Windows.Controls.RichTextBox)elementHost.Child).TextChanged+=new System.Windows.Controls.TextChangedEventHandler(text_TextChanged);
					LayoutManager.Add(elementHost,panelMain);
					elementHost.BringToFront();
					#endregion
				}*/
				#region WinForm RichTextBox
				//Note: Pasting with a WinForms RTB is not overridden and so the user can paste formatted text (even images) into the box.
				//When filling the textbox here, we cannot clip the text in the textbox or else existing signatures will break.
				//The side affect of not clipping is that the edit window will look different if the text is larger than the textbox.
				//However, if the text is bigger than the textbox, the user can see the issue easily by editing the sheet def.
				richTextBox=GraphicsHelper.CreateTextBoxForSheetDisplay(SheetCur.SheetFields[i],false);
				richTextBox.TextChanged+=new EventHandler(text_TextChanged);
				LayoutManager.Add(richTextBox,panelMain);
				richTextBox.BringToFront();
				richTextBox.ReadOnly=SheetCur.SheetFields[i].IsLocked;
				#endregion
			}
			//draw checkboxes----------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.CheckBox) {
					continue;
				}
				sheetCheckBox=new SheetCheckBox();
				if(SheetCur.SheetFields[i].FieldValue=="X") {
					sheetCheckBox.IsChecked=true;
				}
				sheetCheckBox.Location=new Point(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos);
				sheetCheckBox.Width=SheetCur.SheetFields[i].Width;
				sheetCheckBox.Height=SheetCur.SheetFields[i].Height;
				sheetCheckBox.Tag=SheetCur.SheetFields[i];
				sheetCheckBox.MouseUp+=new MouseEventHandler(checkbox_MouseUp);
				sheetCheckBox.KeyUp+=new KeyEventHandler(checkbox_KeyUp);
				sheetCheckBox.TabStop=(SheetCur.SheetFields[i].TabOrder>0);
				sheetCheckBox.TabIndex=SheetCur.SheetFields[i].TabOrder;
				LayoutManager.Add(sheetCheckBox,panelMain);
				sheetCheckBox.BringToFront();
			}
			//draw comboboxes---------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.ComboBox) {
					continue;
				}
				sheetComboBox=new SheetComboBox(SheetCur.SheetFields[i].FieldValue);
				sheetComboBox.Location=new Point(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos);
				sheetComboBox.BackColor=Color.FromArgb(245,245,200);
				sheetComboBox.Width=SheetCur.SheetFields[i].Width;
				sheetComboBox.Height=SheetCur.SheetFields[i].Height;
				sheetComboBox.Tag=SheetCur.SheetFields[i];
				sheetComboBox.TabStop=(SheetCur.SheetFields[i].TabOrder>0);
				sheetComboBox.TabIndex=SheetCur.SheetFields[i].TabOrder;
				sheetComboBox.MouseUp+=new MouseEventHandler(comboBox_MouseUp);
				LayoutManager.Add(sheetComboBox,panelMain);
				sheetComboBox.BringToFront();
			}
			//draw toothcharts--------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.ScreenChart) {
					continue;
				}
				screenToothChart=new ScreenToothChart(SheetCur.SheetFields[i].FieldValue,SheetCur.SheetFields[i].FieldValue[0]=='1');//Need to pass in value here to set tooth chart items.
				screenToothChart.Location=new Point(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos);
				screenToothChart.Width=SheetCur.SheetFields[i].Width;
				screenToothChart.Height=SheetCur.SheetFields[i].Height;
				screenToothChart.Tag=SheetCur.SheetFields[i];
				screenToothChart.Invalidate();
				LayoutManager.Add(screenToothChart,panelMain);
				panelMain.Controls.SetChildIndex(screenToothChart,panelMain.Controls.Count-2);//Ensures it's in the right order but in front of the picture frame.
			}
			//draw signature boxes----------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)) {
					continue;
				}
				if(SheetCur.SheetType==SheetTypeEnum.TreatmentPlan) {
					//==TG 01/12/2016: Removed helper function here after conversation with Ryan.  We never fill any signature boxes for treatment plans in this
					//form.  It is done in FormTPsign, or printed in SheetPrinting.
					MsgBox.Show(this,"Treatment Plan Signatures not currently supported in FormSheetFillEdit.  Contact Support.");
					break;
				}
				OpenDental.UI.SignatureBoxWrapper sigBox=new OpenDental.UI.SignatureBoxWrapper();
				if(SheetCur.SheetFields[i].IsSigProvRestricted 
					&& (Security.CurUser==null || Security.CurUser.UserNum<1 || Security.CurUser.ProvNum<1)) 
				{
					sigBox.Enabled=false;
				}
				if(Security.CurUser!=null
					&& Security.CurUser.UserNum>0 //Is currently a logged in user
					&& SheetCur.SheetFields[i].CanElectronicallySign //If the SheetCur.SheetFields[i] allows for electronic signature
					&& !IsInTerminal) //Electronic signatures are not allowed in kiosk mode
				{
					sigBox.SetAllowDigitalSig(true,true);
				}
				sigBox.Location=new Point(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos);
				sigBox.Width=SheetCur.SheetFields[i].Width;
				sigBox.Height=SheetCur.SheetFields[i].Height;
				if(SheetCur.SheetFields[i].FieldValue.Length>0) {//a signature is present
					bool sigIsTopaz=(SheetCur.SheetFields[i].FieldValue[0]=='1');
					string signature="";
					if(SheetCur.SheetFields[i].FieldValue.Length>1) {
						signature=SheetCur.SheetFields[i].FieldValue.Substring(1);
					}
					string keyData=Sheets.GetSignatureKey(SheetCur);
					sigBox.FillSignature(sigIsTopaz,keyData,signature);
				}
				if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
					PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(SheetCur.Parameters,"payplan").ParamValue;
					if(payPlan.Signature!="") {//a PP sig is present
					string keyData=(string)SheetParameter.GetParamByName(SheetCur.Parameters,"keyData").ParamValue;
						sigBox.FillSignature(payPlan.SigIsTopaz,keyData,payPlan.Signature);
					}
				}
				sigBox.Tag=SheetCur.SheetFields[i];
				sigBox.TabStop=(SheetCur.SheetFields[i].TabOrder>0);
				sigBox.TabIndex=SheetCur.SheetFields[i].TabOrder;
				LayoutManager.Add(sigBox,panelMain);
				sigBox.BringToFront();
				if(sigBox.IsValid && SheetCur.SheetFields[i].FieldValue.Length>0) {
					//According to this form's load function the only pre-requisite for being a "signed" sheet and locking it is that it loads with an existing signature.
					//Based on that if we get this far and there's actually a signature then it's "signed", but this only works with the first form load.
					richTextBox=new RichTextBox();
					richTextBox.BorderStyle=BorderStyle.None;
					richTextBox.TabStop=false;
					richTextBox.Location=new Point(SheetCur.SheetFields[i].XPos+1,SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height-15);
					richTextBox.Width=SheetCur.SheetFields[i].Width-2;
					richTextBox.ScrollBars=RichTextBoxScrollBars.None;
					richTextBox.SelectionAlignment=HorizontalAlignment.Left;
					string signed=sigBox.GetIsTypedFromWebForms() ? "Typed signature in WebForms" : "Signed";
					richTextBox.Text=Lan.g(this,signed)+": "+SheetCur.SheetFields[i].DateTimeSig.ToShortDateString()+" "+SheetCur.SheetFields[i].DateTimeSig.ToShortTimeString();
					richTextBox.Multiline=false;
					richTextBox.Height=14;
					richTextBox.ReadOnly=true;
					LayoutManager.Add(richTextBox,panelMain);
					richTextBox.BringToFront();
				}
				_listSignatureBoxWrappers.Add(sigBox);
			}
			return strErr;
		}

		////WPF RichTextBox is no longer in use, we are only using WinForm RichTextBox. Refer to comment in LayoutFields() when drawing textboxes. Leaving for documentation.
		///<summary>Returns an ElementHost which has it's Child property set to a WPF RichTextBox that is styled specifically for filling out sheets.
		///This is necessary when utilizing tablets so that the "On Screen Keyboard" will automatically pop up when input fields gain focus.
		///Printing and PDF creation will continue to use the WinForm RichTextBox to minimize potential bugs that the WPF control introduces.</summary>
		/*private System.Windows.Forms.Integration.ElementHost CreateWpfRichTextBoxForSheetDisplay(SheetField field) {
			//The WinForm RichTextBox has a bug in it where the on screen keyboard doesn't show when focus has been set to a WinForm RichTextBox.
			//Therefore we need to use the WPF RichTextBox which doesn't have this issue.  An ElementHost is required to use WPF controls in WinForms.
			System.Windows.Forms.Integration.ElementHost elementHost=new System.Windows.Forms.Integration.ElementHost();
			System.Windows.Controls.RichTextBox richTextBox=new System.Windows.Controls.RichTextBox();//Drastically different than WinForm RichTextBox...
			#region Override Base Functionality
			#region Override Paste
			//Take over the ability to paste formatted text into the RTB.  Overriding the application command catches all kinds of ways to paste.
			//E.g. "Ctrl + Shift + V", "Shift + Insert", "Ctrl + V", "Right Click Menu > Paste", etc.
			richTextBox.CommandBindings.Add(new System.Windows.Input.CommandBinding(System.Windows.Input.ApplicationCommands.Paste
				,(s,e) => {
					try {
						//Grab any text from the clipboard in order to programmatically paste it with no formatting.
						object clipboardContent=System.Windows.Clipboard.GetData(DataFormats.Text);//System.Windows.Forms.Clipboard fails for Thinfinity
						if(clipboardContent!=null) {//Can be null if the clipboard has non-text.  E.g. the user copied a picture.
							richTextBox.Selection.Text=clipboardContent.ToString();
							//Now that the text has been pasted in, the WPF RTB will automatically highlight the new text.
							//In order to preserve old behavior, we need put the caret at the end of the currently selected text that was just pasted.
							richTextBox.Selection.Select(richTextBox.Selection.End,richTextBox.Selection.End);
						}
					}
					catch(Exception ex) {
						FriendlyException.Show(Lan.g(this,"Error pasting clipboard content."),ex);
					}
					e.Handled=true;//Do not let the base Paste function to be invoked because it allows pasting formatted text, pictures, etc.
				}));
			#endregion
			#region Override Multiple Newlines on Enter
			//Default WPF RichTextBox behavior for the Enter key is to make the cursor jump two rows instead of one.
			//The user can technically press Shift + Enter instead to get a single row but telling our customers this would wreak havok.
			//The Enter key is entering a paragraph, we just need to make the margin on the paragraph 0.
			System.Windows.Style style=new System.Windows.Style() { TargetType=typeof(System.Windows.Documents.Paragraph) };
			style.Setters.Add(new System.Windows.Setter(System.Windows.Documents.Block.MarginProperty,new System.Windows.Thickness(0)));
			richTextBox.Resources.Add(typeof(System.Windows.Documents.Paragraph),style);
			#endregion
			#region Override Selecting All Text on Focus
			//Tabbing into a WPF RichTextBox with content in is will select all text by default.  However, if you click into it, the highlight is removed
			//and the caret position will be preserved.  But as soon as the text is set programmatically focus goes back to highlighting all text.
			//This is most annoying when we initially load the form with data and start tabbing through fields (highlighting all content in each box).
			//Override GotFocus() in order to preserve old behavior where the caret is automatically placed at the end of the content when tabbed to.
			richTextBox.GotFocus+=(object sender,System.Windows.RoutedEventArgs e) => {
				richTextBox.Selection.Select(richTextBox.Selection.End,richTextBox.Selection.End);
			};
			#endregion
			#endregion
			richTextBox.Margin=new System.Windows.Thickness(0);
			richTextBox.Padding=new System.Windows.Thickness(-3,0,0,0);//-3 to get rid of WPFs arbitrary inner padding.  Works for multiple fonts and sizes.
			elementHost.TabStop=false;//Only input fields allow tab stop (set below for input text).
			richTextBox.BorderThickness=new System.Windows.Thickness(0);
			//Input fields need to have a yellow background so that they stand out to the user.
			if(field.FieldType==SheetFieldType.InputField) {
				richTextBox.Background=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255,245,245,200));
				elementHost.TabStop=(field.TabOrder > 0);
				elementHost.TabIndex=field.TabOrder;
			}
			elementHost.Location=new Point(field.XPos,field.YPos);
			richTextBox.HorizontalScrollBarVisibility=System.Windows.Controls.ScrollBarVisibility.Hidden;
			richTextBox.VerticalScrollBarVisibility=System.Windows.Controls.ScrollBarVisibility.Hidden;
			if(field.ItemColor!=Color.FromArgb(0)) {
				richTextBox.Foreground=new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromArgb(field.ItemColor.A,field.ItemColor.R,field.ItemColor.G,field.ItemColor.B));
			}
			richTextBox.Selection.Text=field.FieldValue??"";
			switch(field.TextAlign) {
				case HorizontalAlignment.Center:
					richTextBox.Document.TextAlignment=System.Windows.TextAlignment.Center;
				break;
				case HorizontalAlignment.Left:
					richTextBox.Document.TextAlignment=System.Windows.TextAlignment.Left;
				break;
				case HorizontalAlignment.Right:
					richTextBox.Document.TextAlignment=System.Windows.TextAlignment.Right;
				break;
			}
			richTextBox.FontStyle=System.Windows.FontStyles.Normal;
			if(field.FontIsBold) {
				richTextBox.FontWeight=System.Windows.FontWeights.Bold;
			}
			richTextBox.FontFamily=new System.Windows.Media.FontFamily(field.FontName);
			//The FontSize needs to be specified in DIPs (device-independent pixels) because WPF treats font size differently than WinForms.
			richTextBox.FontSize=(double)new System.Windows.FontSizeConverter().ConvertFrom(field.FontSize+"pt");
			System.Windows.Documents.Paragraph para=richTextBox.Document.Blocks.FirstBlock as System.Windows.Documents.Paragraph;
			//There is a chance that data can get truncated when loading text into a text box that has AcceptsReturn set to false.
			//E.g. the text "line 1\r\nline 2\r\nline 3" will get truncated to be "line 1"
			//This causes a nasty bug where the user could have filled out the sheet as a Web Form (which does not truncate the text) and signed the sheet.
			//The signature would invalidate if the office were to open the downloaded web form within Open Dental proper and simply click OK.
			//Therefore, only allow InputField text boxes to have AcceptsReturn set to false (which will stop users from making newlines).
			if(field.FieldType==SheetFieldType.InputField && field.Height < para.LineHeight+2) {
				richTextBox.AcceptsReturn=false;
			}
			richTextBox.IsReadOnly=field.IsLocked;
			richTextBox.Tag=field;
			elementHost.Tag=field;
			elementHost.Child=richTextBox;
			elementHost.Width=field.Width;
			elementHost.Height=field.Height;
			return elementHost;
		}*/

		///<summary>For all the combo boxes on the form, selects the first option if nothing is already selected.</summary>
		private void SelectFirstOptionComboBoxes() {
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(SheetComboBox)) {
					continue;
				}
				SheetComboBox sheetComboBox=(SheetComboBox)panelMain.Controls[i];
				if(sheetComboBox.ComboOptions.Length > 0 && sheetComboBox.SelectedOption=="") {
					sheetComboBox.SelectedOption=sheetComboBox.ComboOptions[0];
				}
			}
		}

		private void checkbox_MouseUp(object sender,MouseEventArgs e) {
			FieldValueChanged(sender);
		}

		private void checkbox_KeyUp(object sender,KeyEventArgs e) {
			FieldValueChanged(sender);
		}

		private void comboBox_MouseUp(object sender,MouseEventArgs e) {
			FieldValueChanged(sender);
		}

		private void PictureBox_DoubleClick(object sender,EventArgs e) {
			SheetField sheetField=(SheetField)((Control)sender).Tag;//could be picturebox or label
			using FormSheetFieldEditPatImage formSheetFieldEditPatImage=new FormSheetFieldEditPatImage();
			formSheetFieldEditPatImage.SheetFieldCur=sheetField;
			formSheetFieldEditPatImage.SheetCur=SheetCur;
			formSheetFieldEditPatImage.ShowDialog();
			if(formSheetFieldEditPatImage.DialogResult!=DialogResult.OK){
				return;
			}
			if(formSheetFieldEditPatImage.SheetFieldCur is null){
				SheetFields.DeleteObject(sheetField.SheetFieldNum);
				SheetCur.SheetFields.Remove(sheetField);
			}
			else{
				SheetFields.Update(sheetField);
			}
			FieldValueChanged(sender);//the sender doesn't matter
			LayoutFields();
		}

		private void text_TextChanged(object sender,EventArgs e) {
			timerTextChanged.Stop();
			timerTextChanged.Tag=sender;
			timerTextChanged.Start();
		}

		private void timerTextChanged_Tick(object sender,EventArgs e) {
			timerTextChanged.Stop();
			FieldValueChanged(timerTextChanged.Tag);
			timerTextChanged.Tag=null;
		}

		private void panelScroll_MouseUp(object sender,MouseEventArgs e) {
			panelScroll.Focus();
		}

		private void FormSheetFillEdit_MouseUp(object sender,MouseEventArgs e) {
			panelScroll.Focus();
		}

		///<summary>Triggered when any field value changes.  Also causes fields to grow as needed and deselects other radiobuttons in a group.
		///Will clear all signature boxes if this is not a "new" sheet.</summary>
		private void FieldValueChanged(object sender) {
			for(int i=0;i<_listSignatureBoxWrappers.Count;i++){
				//When field values change, the signature should not show as "invalid" but instead should just be cleared so that the user can re-sign.
				//sigBox.SetInvalid();
				_listSignatureBoxWrappers[i].ClearSignature(false);//The user is purposefully "invalidating" the old signature by changing the contents of the sheet. 
			}
			if(sender.GetType()==typeof(SheetCheckBox)) {
				SheetCheckBox sheetCheckBox=(SheetCheckBox)sender;
				if(sheetCheckBox.Tag==null) {
					return;
				}
				if(!sheetCheckBox.IsChecked) {//if user unchecked a radiobutton, nothing else happens
					return;
				}
				SheetField sheetFieldThis=(SheetField)sheetCheckBox.Tag;
				if(sheetFieldThis.RadioButtonGroup=="" && sheetFieldThis.RadioButtonValue==""){//if it's a checkbox instead of a radiobutton
					return;
				}
				for(int i=0;i<panelMain.Controls.Count;i++){ //set some other radiobuttons to be not checked
					if(panelMain.Controls[i].GetType()!=typeof(SheetCheckBox)) {
						continue;
					}
					if(panelMain.Controls[i].Tag==null) {
						continue;
					}
					if(panelMain.Controls[i]==sender) {
						continue;
					}
					SheetField sheetFieldOther=(SheetField)panelMain.Controls[i].Tag;
					if(sheetFieldThis.FieldName!=sheetFieldOther.FieldName) {//different radio group
						continue;
					}
					//If both checkbox field names are set to "misc" then we instead use the RadioButtonGroup as the actual radio button group name.
					if(sheetFieldThis.FieldName=="misc" && sheetFieldThis.RadioButtonGroup!=sheetFieldOther.RadioButtonGroup){
						continue;
					}
					((SheetCheckBox)panelMain.Controls[i]).IsChecked=false;
				}
				return;
			}
			if(sender.GetType()!=typeof(RichTextBox) && sender.GetType()!=typeof(System.Windows.Controls.RichTextBox)) {//One for WinForms one for WPF
				//since CheckBoxes also trigger this event for sig invalid.
				return;
			}
			//everything below here is for growth calc.
			SheetField sheetField;
			int cursorPos;//remember where we were.
			if(sender.GetType()==typeof(RichTextBox)) {
				RichTextBox richTextBox=(RichTextBox)sender;
				sheetField=(SheetField)richTextBox.Tag;
				sheetField.FieldValue=richTextBox.Text;
				cursorPos=richTextBox.SelectionStart;
			}
			else {//WPF RichTextBox
				System.Windows.Controls.RichTextBox richTextBox=(System.Windows.Controls.RichTextBox)sender;
				sheetField=(SheetField)richTextBox.Tag;
				sheetField.FieldValue=GetTextFromWpfRichTextBox(richTextBox);
				cursorPos=richTextBox.Document.ContentStart.GetOffsetToPosition(richTextBox.CaretPosition);
			}
			if(sheetField.GrowthBehavior==GrowthBehaviorEnum.None){
				return;
			}
			FontStyle fontstyle=FontStyle.Regular;
			if(sheetField.FontIsBold){
				fontstyle=FontStyle.Bold;
			}
			Font font=new Font(sheetField.FontName,sheetField.FontSize,fontstyle);
			int calcH=GraphicsHelper.MeasureStringH(sheetField.FieldValue,font,sheetField.Width,sheetField.TextAlign);
				//(int)(g.MeasureString(fld.FieldValue,font,fld.Width).Height * 1.133f);//Seems to need 2 pixels per line of text to prevent hidden text due to scroll.
			calcH+=font.Height+2;//add one line just in case.
			if(calcH<=sheetField.Height){//no growth needed.  If this is ever removed then SheetUtil.MoveAllUpBelowThis() needs to be considered below.
				return;
			}
			//the field height needs to change, so:
			int amountOfGrowth=calcH-sheetField.Height;
			sheetField.Height=calcH;
			//Growth of entire form.
			LayoutManager.MoveHeight(pictureBoxMain,pictureBoxMain.Height+amountOfGrowth);
			LayoutManager.MoveHeight(panelMain,panelMain.Height+amountOfGrowth);
			int h=_imageDraw.Height+amountOfGrowth;
			int w=_imageDraw.Width;
			_imageDraw.Dispose();
			_imageDraw=new Bitmap(w,h);
			FillFieldsFromControls();//We already changed the value of this field manually, 
				//but if the other fields don't get changed, they will erroneously 'reset'.
			if(sheetField.GrowthBehavior==GrowthBehaviorEnum.DownGlobal) {
				SheetUtil.MoveAllDownBelowThis(SheetCur,sheetField,amountOfGrowth);
			}
			else if(sheetField.GrowthBehavior==GrowthBehaviorEnum.DownLocal) {
				SheetUtil.MoveAllDownWhichIntersect(SheetCur,sheetField,amountOfGrowth);
			}
			LayoutFields();
			//find the original textbox, and put the cursor back where it belongs
			for(int i=0;i<panelMain.Controls.Count;i++){
				//We no longer use ElementHost for WPF RichTextBoxes, only WinForm RichTextBox
				if(panelMain.Controls[i].GetType()!=typeof(RichTextBox) && panelMain.Controls[i].GetType()!=typeof(System.Windows.Forms.Integration.ElementHost)) {
					continue;
				}
				ODException.SwallowAnyException(() => {//Pasting can throw off the WPF RTB and it just doesn't matter enough to crash the system for this.
					if(panelMain.Controls[i].GetType()==typeof(RichTextBox)) {//WinForms
						if((SheetField)(panelMain.Controls[i].Tag)==sheetField) {
							((RichTextBox)panelMain.Controls[i]).Select(cursorPos,0);
							((RichTextBox)panelMain.Controls[i]).Focus();
						}
					}
					else if(panelMain.Controls[i].GetType()==typeof(System.Windows.Forms.Integration.ElementHost)) {//This will never be hit, since we are no longer using WPF RichTextBox. Leaving for documentation
						System.Windows.Forms.Integration.ElementHost elementHost=(System.Windows.Forms.Integration.ElementHost)panelMain.Controls[i];
						if(elementHost.Child==null || elementHost.Child.GetType()!=typeof(System.Windows.Controls.RichTextBox)) {
							return;//Not sure how this could happen.  Nothing to paste into.
						}
						if((SheetField)(panelMain.Controls[i].Tag)==sheetField) {
							System.Windows.Controls.RichTextBox richTextBox=(System.Windows.Controls.RichTextBox)elementHost.Child;
							var index=richTextBox.Document.ContentStart.GetPositionAtOffset(cursorPos);
							richTextBox.Selection.Select(index,index);
							richTextBox.Focus();
						}
					}
				});
			}
		}

		///<summary>Returns all of the text that is currently in the WPF RichTextBox passed in.</summary>
		private string GetTextFromWpfRichTextBox(System.Windows.Controls.RichTextBox richTextBox) {
			//WPF doesn't have a convenient way to determine where the cursor currently is.  We have to do some math to figure it out.
			System.Windows.Documents.TextPointer contentStart=richTextBox.Document.ContentStart;
			System.Windows.Documents.TextPointer contentEnd=richTextBox.Document.ContentEnd;
			//Read in all of the text that is currently present within the RTB.
			System.Windows.Documents.TextRange rangeStartToEnd=new System.Windows.Documents.TextRange(contentStart,contentEnd);
			//New line characters and paragraph breaks are treated as equivalent with respect to this property.
			//Therefore, there will always be an erroneous new line (as \r\n) at the very end of the document (indicating the end of the last paragraph).
			//In order to preserve any new lines that the user typed in (as \n), we have to first trim '\n' and then trim '\r' as to not trim too much.
			return rangeStartToEnd.Text.TrimEnd('\n').TrimEnd('\r');//e.g. this would preserve "test\n\n\n\r\n" as "test\n\n\n"
		}

		private void pictDraw_MouseDown(object sender,MouseEventArgs e) {
			if(IsSignatureStarted()){
				return;
			}
			_isMouseDown=true;
			if(checkErase.Checked){
				return;
			}
			_listPoints.Add(new Point(e.X,e.Y));
		}

		private void pictDraw_MouseEnter(object sender,EventArgs e) {

		}

		private void pictDraw_MouseLeave(object sender,EventArgs e) {

		}

		private void pictDraw_MouseMove(object sender,MouseEventArgs e) {
			if(!_isMouseDown){
				return;
			}
			if(checkErase.Checked){
				//look for any lines that intersect the "eraser".
				//since the line segments are so short, it's sufficient to check end points.
				//Point point;
				string[] stringArrayXy;
				string[] stringArrayPoints;
				float x;
				float y;
				float dist;//the distance between the point being tested and the center of the eraser circle.
				float radius=8f;//by trial and error to achieve best feel.
				PointF pointFEraser=new PointF(e.X+pictureBoxMain.Left+8.49f,e.Y+pictureBoxMain.Top+8.49f);
				for(int i=0;i<SheetCur.SheetFields.Count;i++){
					if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.Drawing){
						continue;
					}
					stringArrayPoints=SheetCur.SheetFields[i].FieldValue.Split(';');
					for(int p=0;p<stringArrayPoints.Length;p++){
						stringArrayXy=stringArrayPoints[p].Split(',');
						if(stringArrayXy.Length==2){
							x=PIn.Float(stringArrayXy[0]);
							y=PIn.Float(stringArrayXy[1]);
							dist=(float)Math.Sqrt(Math.Pow(Math.Abs(x-pointFEraser.X),2)+Math.Pow(Math.Abs(y-pointFEraser.Y),2));
							if(dist<=radius){//testing circle intersection here
								SheetCur.SheetFields.Remove(SheetCur.SheetFields[i]);
								RefreshPanel();
								return;
							}
						}
					}
				}	
				return;
			}
			_listPoints.Add(new Point(e.X,e.Y));
			//RefreshPanel();
			//just add the last line segment instead of redrawing the whole thing.
			using Graphics g=Graphics.FromImage(pictureBoxMain.Image);
			g.SmoothingMode=SmoothingMode.HighQuality;
			Pen pen=new Pen(Brushes.Black,2f);
			int idx=_listPoints.Count-1;
			g.DrawLine(pen,_listPoints[idx-1].X,_listPoints[idx-1].Y,_listPoints[idx].X,_listPoints[idx].Y);
			pictureBoxMain.Invalidate();
		}

		private void pictDraw_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDown=false;
			if(IsSignatureStarted()){
				return;
			}
			if(checkErase.Checked){
				return;
			}
			SheetField sheetField=new SheetField();
			sheetField.FieldType=SheetFieldType.Drawing;
			sheetField.FieldName="";
			sheetField.FieldValue="";
			for(int i=0;i<_listPoints.Count;i++){
				if(i>0){
					sheetField.FieldValue+=";";
				}
				sheetField.FieldValue+=(_listPoints[i].X+pictureBoxMain.Left)+","+(_listPoints[i].Y+pictureBoxMain.Top);
			}
			sheetField.FontName="";
			sheetField.RadioButtonValue="";
			SheetCur.SheetFields.Add(sheetField);
			FieldValueChanged(sender);
			_listPoints.Clear();
			RefreshPanel();
			panelScroll.Focus();
		}

		///<summary>Draws drawings, rectangles, lines, special, and grids</summary>
		private void RefreshPanel(){
			Image image=(Image)_imageDraw.Clone();
			using Graphics g=Graphics.FromImage(image);
			g.SmoothingMode=SmoothingMode.HighQuality;
			//g.CompositingQuality=CompositingQuality.Default;
			Pen pen=new Pen(Brushes.Black,2f);
			Pen pen2=new Pen(Brushes.Black,1f);
			string[] stringArrayPoints;
			List<Point> listPoints;
			Point point;
			string[] stringArrayXy;
			for(int f=0;f<SheetCur.SheetFields.Count;f++){
				if(SheetCur.SheetFields[f].FieldType==SheetFieldType.Drawing){
					stringArrayPoints=SheetCur.SheetFields[f].FieldValue.Split(';');
					listPoints=new List<Point>();
					for(int p=0;p<stringArrayPoints.Length;p++){
						stringArrayXy=stringArrayPoints[p].Split(',');
						if(stringArrayXy.Length==2){
							point=new Point(PIn.Int(stringArrayXy[0]),PIn.Int(stringArrayXy[1]));
							listPoints.Add(point);
						}
					}
					for(int i=1;i<listPoints.Count;i++){
						g.DrawLine(pen,listPoints[i-1].X-pictureBoxMain.Left,
							listPoints[i-1].Y-pictureBoxMain.Top,
							listPoints[i].X-pictureBoxMain.Left,
							listPoints[i].Y-pictureBoxMain.Top);
					}
				}
				if(SheetCur.SheetFields[f].FieldType==SheetFieldType.Line){
					g.DrawLine((SheetCur.SheetFields[f].ItemColor.ToArgb()==Color.FromArgb(0).ToArgb()?pen2:new Pen(SheetCur.SheetFields[f].ItemColor,1))
						,SheetCur.SheetFields[f].XPos-pictureBoxMain.Left,
						SheetCur.SheetFields[f].YPos-pictureBoxMain.Top,
						SheetCur.SheetFields[f].XPos+SheetCur.SheetFields[f].Width-pictureBoxMain.Left,
						SheetCur.SheetFields[f].YPos+SheetCur.SheetFields[f].Height-pictureBoxMain.Top);
				}
				if(SheetCur.SheetFields[f].FieldType==SheetFieldType.Rectangle){
					g.DrawRectangle((SheetCur.SheetFields[f].ItemColor.ToArgb()==Color.FromArgb(0).ToArgb()?pen2:new Pen(SheetCur.SheetFields[f].ItemColor,1)),
						SheetCur.SheetFields[f].XPos-pictureBoxMain.Left,
						SheetCur.SheetFields[f].YPos-pictureBoxMain.Top,
						SheetCur.SheetFields[f].Width,
						SheetCur.SheetFields[f].Height);
				}
			}
			List<SheetField> listSheetFieldsOutput=SheetCur.SheetFields.FindAll(x => x.FieldType==SheetFieldType.OutputText);
			for(int i=0;i<listSheetFieldsOutput.Count;i++){ //rectangles around specific output fields
				switch(SheetCur.SheetType.ToString()+"."+listSheetFieldsOutput[i].FieldName) {
					case "TreatmentPlan.Note":
						g.DrawRectangle(Pens.DarkGray,
							listSheetFieldsOutput[i].XPos-pictureBoxMain.Left-1,
							listSheetFieldsOutput[i].YPos-pictureBoxMain.Top-1,
							listSheetFieldsOutput[i].Width+2,
							listSheetFieldsOutput[i].Height+2);
						break;
				}
			}
			List<SheetField> listSheetFieldsSigBox=SheetCur.SheetFields.FindAll(x => x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice));
			for(int i=0;i<listSheetFieldsSigBox.Count;i++){ //rectangles around specific output fields
				switch(SheetCur.SheetType) {
					case SheetTypeEnum.TreatmentPlan:
						g.DrawRectangle(Pens.Black,listSheetFieldsSigBox[i].XPos-pictureBoxMain.Left-1,listSheetFieldsSigBox[i].YPos-pictureBoxMain.Top-1,listSheetFieldsSigBox[i].Width+2,listSheetFieldsSigBox[i].Height+2);
						break;
				}
			}
			List<SheetField> listSheetFieldsSpecial=SheetCur.SheetFields.FindAll(x => x.FieldType==SheetFieldType.Special);
			for(int i=0;i<listSheetFieldsSpecial.Count;i++){
				OpenDentBusiness.SheetPrinting.DrawFieldSpecial(SheetCur,listSheetFieldsSpecial[i],g,null,0);
			}
			List<SheetField> listSheetFieldsGrid=SheetCur.SheetFields.FindAll(x => x.FieldType==SheetFieldType.Grid);
			for(int i=0;i<listSheetFieldsGrid.Count;i++){
				SheetPrinting.DrawFieldGrid(listSheetFieldsGrid[i],SheetCur,g,null,_dataSet,StatementCur,MedLabCur);
			}
			//Draw pagebreak
			Pen penDashPage=new Pen(Color.Green);
			penDashPage.DashPattern=new float[] { 4.0F,3.0F,2.0F,3.0F };
			Pen penDashMargin=new Pen(Color.Green);
			penDashMargin.DashPattern=new float[] { 1.0F,5.0F };
			int pageCount=Sheets.CalculatePageCount(SheetCur,_marginsPrint);
			int margins=(_marginsPrint.Top+_marginsPrint.Bottom);
			for(int i=1;i<pageCount;i++) {
				//g.DrawLine(pDashMargin,0,i*SheetCur.HeightPage-_printMargin.Bottom,SheetCur.WidthPage,i*SheetCur.HeightPage-_printMargin.Bottom);
				g.DrawLine(penDashPage,0,i*(SheetCur.HeightPage-margins)+_marginsPrint.Top,SheetCur.WidthPage,i*(SheetCur.HeightPage-margins)+_marginsPrint.Top);
				//g.DrawLine(pDashMargin,0,i*SheetCur.HeightPage+_printMargin.Top,SheetCur.WidthPage,i*SheetCur.HeightPage+_printMargin.Top);
			}//End Draw Page Break
			pictureBoxMain.Image.Dispose();
			pictureBoxMain.Image=image;
		}

		private void checkErase_Click(object sender,EventArgs e) {
			if(checkErase.Checked){
				pictureBoxMain.Cursor=new Cursor(GetType(),"EraseCircle.cur");
			}
			else{
				pictureBoxMain.Cursor=Cursors.Default;
			}
		}

		private void panelColor_DoubleClick(object sender,EventArgs e) {

		}

		private void butPrint_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetter();
			validateAndUpdateFonts(SheetCur,true);// validate fonts before printing/creating PDF
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
				SaveSignaturePayPlan();
			}
			else {
				if(!TryToSaveData()) {
					return;
				}
				if(hasSheetFromDb) {
					//We need to refresh SheetCur with the sheet from the database due to signature printing.
					//Without this line, a user could create a new sheet, sign it, click print and the signature would not show correctly.
					SheetCur=Sheets.GetSheet(SheetCur.SheetNum);
				}
			}
			//whether this is a new sheet, or one pulled from the database,
			//it will have the extra parameter we are looking for.
			//A new sheet will also have a PatNum parameter which we will ignore.
			using FormSheetOutputFormat formSheetOutputFormat=new FormSheetOutputFormat();
			if(SheetCur.SheetType==SheetTypeEnum.ReferralSlip
				|| SheetCur.SheetType==SheetTypeEnum.ReferralLetter)
			{
				formSheetOutputFormat.QtyPaperCopies=2;
			}
			else{
				formSheetOutputFormat.QtyPaperCopies=1;
			}
			if(SheetCur.PatNum!=0
				&& SheetCur.SheetType!=SheetTypeEnum.DepositSlip) 
			{
				Patient patient=Patients.GetPat(SheetCur.PatNum);
				if(SheetCur.SheetType==SheetTypeEnum.LabSlip) {
					formSheetOutputFormat.IsForLab=true;//Changes label to "E-mail to Lab:"
					SheetParameter sheetParamLabCaseNum=SheetParameter.GetParamByName(SheetCur.Parameters,"LabCaseNum");//auto populate lab email.
					LabCase labCase=LabCases.GetOne(PIn.Long(sheetParamLabCaseNum.ParamValue.ToString()));
					formSheetOutputFormat.EmailPatOrLabAddress=Laboratories.GetOne(labCase.LaboratoryNum).Email;
				}
				else if(patient.Email!="") {
					formSheetOutputFormat.EmailPatOrLabAddress=patient.Email;
					//No need to email to a patient for sheet types: LabelPatient (0), LabelCarrier (1), LabelReferral (2), ReferralSlip (3), LabelAppointment (4), Rx (5), Consent (6), ReferralLetter (8), ExamSheet (13), DepositSlip (14)
					//The data is too private to email unencrypted for sheet types: PatientForm (9), RoutingSlip (10), MedicalHistory (11), LabSlip (12)
					//A patient might want email for the following sheet types and the data is not very private: PatientLetter (7)
					if(SheetCur.SheetType==SheetTypeEnum.PatientLetter) {
						//This just defines the default selection. The user can manually change selections in FormSheetOutputFormat.
						formSheetOutputFormat.IsEmailPatOrLab=true;
						formSheetOutputFormat.QtyPaperCopies--;
					}
				}
			}
			Referral referral=null;
			if(SheetCur.SheetType==SheetTypeEnum.ReferralSlip
				|| SheetCur.SheetType==SheetTypeEnum.ReferralLetter)
			{
				formSheetOutputFormat.IsEmail2Visible=true;
				SheetParameter parameter=SheetParameter.GetParamByName(SheetCur.Parameters,"ReferralNum");
				if(parameter==null){//it can be null sometimes because of old bug in db.
					formSheetOutputFormat.IsEmail2Visible=false;//prevents trying to attach email to nonexistent referral.
				}
				else{
					long referralNum=PIn.Long(parameter.ParamValue.ToString());
					try{
						referral=Referrals.GetReferral(referralNum);
					}
					catch (Exception ex) {
						MessageBox.Show(ex.Message);
					}
					if(referral!=null && referral.EMail!="") {
						formSheetOutputFormat.Email2Address=referral.EMail;
						formSheetOutputFormat.Email2=true;
						formSheetOutputFormat.QtyPaperCopies--;
					}
				}
			}
			else{
				formSheetOutputFormat.IsEmail2Visible=false;
			}
			formSheetOutputFormat.ShowDialog();
			if(formSheetOutputFormat.DialogResult!=DialogResult.OK) {
				//The user canceled out of printing.  Due to signature printing logic we had to update SheetCur to a new object directly from the database.
				//Therefore, all of the Tag objects on our controls are still linked up to the memory of the old SheetCur object.
				//Simply refresh the sheet which will link the controls back up to the current SheetCur object.
				if(hasSheetFromDb) {//Only re-layout the fields if we actually changed SheetCur to a new object from the db.
					LayoutFields();
				}
				return;
			}
			if(formSheetOutputFormat.QtyPaperCopies>0){
				if(IsStatement) {
					SheetPrinting.Print(SheetCur,_dataSet,1,IsRxControlled,StatementCur,MedLabCur);
				}
				else {
					SheetPrinting.Print(SheetCur,formSheetOutputFormat.QtyPaperCopies,IsRxControlled,StatementCur,MedLabCur);
				}
			}
			string pdfFile="";
			if(formSheetOutputFormat.IsEmailPatOrLab){
				pdfFile=EmailSheet(formSheetOutputFormat.EmailPatOrLabAddress,SheetCur.Description.ToString());//subject could be improved
			}
			if((SheetCur.SheetType==SheetTypeEnum.ReferralSlip || SheetCur.SheetType==SheetTypeEnum.ReferralLetter) && formSheetOutputFormat.Email2) {
				//subject will work even if patnum invalid
				pdfFile=EmailSheet(formSheetOutputFormat.Email2Address,Lan.g(this,"RE: ")+Patients.GetLim(SheetCur.PatNum).GetNameLF());
			}
			if(SheetCur.SheetType==SheetTypeEnum.Statement && SaveStatementToDocDelegate!=null) {
				SaveStatementToDocDelegate(StatementCur,SheetCur,_dataSet,pdfFile);
			}
			if(SheetCur.SheetType==SheetTypeEnum.LabSlip) {
				SaveAsDocument('B',"LabSlipArchive");
			}
			if(_isAutoSave && checkSaveToImages.Checked) {
				SaveAsDocument('U',"PatientForm");
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butSimplePrint_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetter();
			validateAndUpdateFonts(SheetCur,true);// validate fonts before printing/creating PDF
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
				SaveSignaturePayPlan();
			}
			else {
				if(!TryToSaveData()) {
					return;
				}
				if(hasSheetFromDb) {
					//We need to refresh SheetCur with the sheet from the database due to signature printing.
					//Without this line, a user could create a new sheet, sign it, click print and the signature would not show correctly.
					SheetCur=Sheets.GetSheet(SheetCur.SheetNum);
				}
			}
			if(SheetCur==null) {
				//sheet was deleted.
				MsgBox.Show(this,"The sheet has been deleted.");
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			if(IsStatement) {
				SheetPrinting.Print(SheetCur,_dataSet,1,IsRxControlled,StatementCur,MedLabCur);
			}
			else {
				SheetPrinting.Print(SheetCur,1,IsRxControlled,StatementCur,MedLabCur);
			}
			if(SheetCur.SheetType==SheetTypeEnum.Statement && SaveStatementToDocDelegate!=null) {
				SaveStatementToDocDelegate(StatementCur,SheetCur,_dataSet);
			}
			if(SheetCur.SheetType==SheetTypeEnum.LabSlip) {
				SaveAsDocument('B',"LabSlipArchive");
			}
			if(_isAutoSave && checkSaveToImages.Checked) {
				SaveAsDocument('U',"PatientForm");
			}
			if(_doExportCSV) {
				Statements.SaveStatementAsCSV(StatementCur);
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Takes the "To" address and subject and correctly formats an email to the lab or patient.
		///Returns the file path to the pdf that is created.</summary>
		private string EmailSheet(string addr_to,string subject) {
			EmailMessage emailMessage;
			Random rnd=new Random();
			string attachPath=EmailAttaches.GetAttachPath();
			string fileName;
			string filePathAndName;
			EmailAddress emailAddress;
			Patient patient=Patients.GetPat(SheetCur.PatNum);
			if(patient==null) {
				emailAddress=EmailAddresses.GetNewEmailDefault(Security.CurUser.UserNum,Clinics.ClinicNum);
			}
			else {
				emailAddress=EmailAddresses.GetNewEmailDefault(Security.CurUser.UserNum,patient.ClinicNum);
			}
			if(!Security.IsAuthorized(Permissions.EmailSend,false)) {//Still need to return after printing, but not send emails.
				DialogResult=DialogResult.OK;
				Close();
				return "";
			}
			//Format Email
			fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".pdf";
			filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
			string pdfFile;
			if(CloudStorage.IsCloudStorage) {
				pdfFile=PrefC.GetRandomTempFile("pdf");
			}
			else {
				pdfFile=filePathAndName;
			}
			if(!string.IsNullOrEmpty(_tempPdfFile) && File.Exists(_tempPdfFile)) {
				if(CloudStorage.IsCloudStorage) {
					pdfFile=_tempPdfFile;
				}
				else {
					File.Copy(_tempPdfFile,pdfFile);
				}
			}
			else if(IsStatement) {
				SheetPrinting.CreatePdf(SheetCur,pdfFile,StatementCur,_dataSet,MedLabCur);
			}
			else {
				SheetPrinting.CreatePdf(SheetCur,pdfFile,StatementCur,MedLabCur);
			}
			if(CloudStorage.IsCloudStorage) {
				FileAtoZ.Copy(pdfFile,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ);
			}
			emailMessage=new EmailMessage();
			emailMessage.Subject=subject;
			string shortFileName=Regex.Replace(SheetCur.Description.ToString(), @"[^\w'@-_()&]", "");
			EmailMessageSource emailMessageSource=EmailMessageSource.Sheet;
			if(SheetCur.SheetType==SheetTypeEnum.Statement && patient!=null && IsStatement) {
				//Mimics FormStatementOptions.CreateEmailMessage(), we should centralize this in the future.
				emailMessage=Statements.GetEmailMessageForStatement(StatementCur,patient);
				shortFileName="Statement";
				emailMessageSource=EmailMessageSource.Statement;
			}
			emailMessage.PatNum=SheetCur.PatNum;
			emailMessage.ToAddress=addr_to;
			emailMessage.FromAddress=emailAddress.GetFrom();//Can be blank just as it could with the old pref.
			EmailAttach emailAttach=new EmailAttach();
			emailAttach.DisplayedFileName=shortFileName+".pdf";
			emailAttach.ActualFileName=fileName;
			emailMessage.Attachments.Add(emailAttach);
			emailMessage.MsgType=emailMessageSource;
			if(_doExportCSV) {
				rnd=new Random();
				string csvFileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".csv";
				string csvPathAndName=ODFileUtils.CombinePaths(attachPath,csvFileName);
				string csvFilePath=Statements.SaveStatementAsCSV(StatementCur);
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase){
					MsgBox.Show(this,"Could not create email because no AtoZ folder.");
				}
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					File.Copy(csvFilePath,csvPathAndName);
				}
				EmailAttach emailAttachCSV=new EmailAttach();
				emailAttachCSV.DisplayedFileName="Statement.csv";
				emailAttachCSV.ActualFileName=csvFileName;
				emailMessage.Attachments.Add(emailAttachCSV);
			}
			using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,emailAddress);
			formEmailMessageEdit.IsNew=true;
			if(formEmailMessageEdit.ShowDialog()==DialogResult.OK) {
				HasEmailBeenSent=true;
			}
			if(SheetCur.SheetType==SheetTypeEnum.LabSlip) {
				SaveAsDocument('B',"LabSlipArchive");
			}
			DialogResult=DialogResult.OK;
			Close();
			return pdfFile;
		}

		private void butEmail_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetter();
			validateAndUpdateFonts(SheetCur,true);// validate fonts before printing/creating PDF
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
				SaveSignaturePayPlan();
			}
			else {
				if(!TryToSaveData()) {
					return;
				}
				if(hasSheetFromDb) {
					//We need to refresh SheetCur with the sheet from the database due to signature printing.
					//Without this line, a user could create a new sheet, sign it, click print and the signature would not show correctly.
					SheetCur=Sheets.GetSheet(SheetCur.SheetNum);
				}
			}
			//whether this is a new sheet, or one pulled from the database,
			//it will have the extra parameter we are looking for.
			//A new sheet will also have a PatNum parameter which we will ignore.
			Patient patient=null;
			string strEmailAddress="";
			string subject=SheetCur.Description.ToString();
			if(SheetCur.PatNum!=0 && SheetCur.SheetType!=SheetTypeEnum.DepositSlip) {
				patient=Patients.GetPat(SheetCur.PatNum);
				if(SheetCur.SheetType==SheetTypeEnum.LabSlip) {
					SheetParameter sheetParamLabCaseNum=SheetParameter.GetParamByName(SheetCur.Parameters,"LabCaseNum");//auto populate lab email.
					LabCase labCase=LabCases.GetOne(PIn.Long(sheetParamLabCaseNum.ParamValue.ToString()));
					strEmailAddress=Laboratories.GetOne(labCase.LaboratoryNum).Email;
				}
				else if(patient.Email!="") {
					strEmailAddress=patient.Email;
				}
			}
			if(SheetCur.SheetType==SheetTypeEnum.ReferralSlip || SheetCur.SheetType==SheetTypeEnum.ReferralLetter) {
				SheetParameter sheetParameter=SheetParameter.GetParamByName(SheetCur.Parameters,"ReferralNum");
				if(sheetParameter==null) {//it can be null sometimes because of old bug in db.
					strEmailAddress="";//This would be rare, but we would not want to send a referral to the patient when normally it is sent to the doctor.
				}
				else {
					long referralNum=PIn.Long(sheetParameter.ParamValue.ToString());
					Referral referral=null;
					try {
						referral=Referrals.GetReferral(referralNum);
					}
					catch (ApplicationException ex) {
						MessageBox.Show(ex.Message);
					}
					if(referral !=null && referral.EMail!="") {
						strEmailAddress=referral.EMail;
					}
					subject=Lan.g(this,"RE: ")+Patients.GetLim(SheetCur.PatNum).GetNameLF();
				}
			}
			string pdfFile=EmailSheet(strEmailAddress,subject);
			if(HasEmailBeenSent && SheetCur.SheetType==SheetTypeEnum.Statement && SaveStatementToDocDelegate!=null) {
				SaveStatementToDocDelegate(StatementCur,SheetCur,_dataSet,pdfFile);
			}
			if(_isAutoSave && checkSaveToImages.Checked) {
				SaveAsDocument('U',"PatientForm");
			}
		}

		private void butPDF_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetter();
			validateAndUpdateFonts(SheetCur,true);// validate fonts before printing/creating PDF
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
				SaveSignaturePayPlan();
			}
			else {
				if(!TryToSaveData()) {
					return;
				}
				if(hasSheetFromDb && SheetCur.SheetType!=SheetTypeEnum.TreatmentPlan) {
					SheetCur=Sheets.GetSheet(SheetCur.SheetNum);
				}
			}
			string filePathAndName;
			if(!string.IsNullOrEmpty(_tempPdfFile) && File.Exists(_tempPdfFile)) {
				filePathAndName=_tempPdfFile;
			}
			else {
				filePathAndName=PrefC.GetRandomTempFile(".pdf");
				if(IsStatement) {
					SheetPrinting.CreatePdf(SheetCur,filePathAndName,StatementCur,_dataSet,MedLabCur);
				}
				else {
					SheetPrinting.CreatePdf(SheetCur,filePathAndName,StatementCur,MedLabCur);
				}
			}
			try {
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(filePathAndName);
				}
				else {
					Process.Start(filePathAndName);
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Unable to open the file."),ex);
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,SheetCur.Description+" from "+SheetCur.DateTimeSheet.ToShortDateString()+" pdf was created");
			if(SheetCur.SheetType==SheetTypeEnum.Statement && SaveStatementToDocDelegate!=null) {
				SaveStatementToDocDelegate(StatementCur,SheetCur,_dataSet,filePathAndName);
			}
			if(SheetCur.SheetType==SheetTypeEnum.LabSlip) {
				SaveAsDocument('B',"LabSlipArchive");
			}
			if(_isAutoSave && checkSaveToImages.Checked) {
				SaveAsDocument('U',"PatientForm");
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butChangePat_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.IsSelectionModeOnly=true;
			if(formPatientSelect.ShowDialog()==DialogResult.OK) {
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,Lan.g(this,"Sheet with ID")+" "+SheetCur.SheetNum+" "+Lan.g(this,"moved to PatNum")+" "+formPatientSelect.PatNumSelected);
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,formPatientSelect.PatNumSelected,Lan.g(this,"Sheet with ID")+" "+SheetCur.SheetNum+" "+Lan.g(this,"moved from PatNum")+" "+SheetCur.PatNum);
				SheetCur.PatNum=formPatientSelect.PatNumSelected;
				if(SheetCur.PatNum>0) {
					checkSaveToImages.Enabled=true;
				}
			}
		}

		private void butUnlock_Click(object sender,EventArgs e) {
			//we already know the user has permission, because otherwise, button is not visible.
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
				PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(SheetCur.Parameters,"payplan").ParamValue;
				for(int i=0;i<panelMain.Controls.Count;i++){
					if(panelMain.Controls[i].GetType()!=typeof(SignatureBoxWrapper)) {
						continue;
					}
					if(panelMain.Controls[i].Tag==null) {
						continue;
					}
					OpenDental.UI.SignatureBoxWrapper sigBox=(OpenDental.UI.SignatureBoxWrapper)panelMain.Controls[i];
					sigBox.Enabled=true;
				}
				butSave.Visible=true;
			}
			panelMain.Enabled=true;
			butUnlock.Visible=false;
		}

		///<summary>Returns true if SheetCur is new and is a referral letter and has a toothchart or grid sheetfields.</summary>
		private bool IsNewReferralLetter() {
			return SheetCur.IsNew
				&& SheetCur.SheetType==SheetTypeEnum.ReferralLetter
				&& SheetCur.SheetFields.Exists(x => x.FieldName=="ReferralLetterProceduresCompleted" || x.FieldName=="toothChart");
		}

		///<summary>If this sheet is a statement, then the sheet does not actually get saved to the database.</summary>
		private bool TryToSaveData() {
			if(IsStatement) {
				FillFieldsFromComboBoxes();
				return true;//We don't actually save a statement sheet to the database.
			}
			if(!butOK.Enabled) {//if the OK button is not enabled, user does not have permission.
				return true;
			}
			if(!textShowInTerminal.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			DateTime dateTimeSheet=DateTime.MinValue;
			try {
				dateTimeSheet=DateTime.Parse(textDateTime.Text);
			}
			catch(Exception) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			SheetCur.DateTimeSheet=dateTimeSheet;
			SheetCur.Description=textDescription.Text;
			SheetCur.InternalNote=textNote.Text;
			SheetCur.ShowInTerminal=PIn.Byte(textShowInTerminal.Text);
			SheetCur.DateTSheetEdited=DateTime.Now;//Will get overwritten on insert, but used for update.  Fill even if user did not make changes.
			FillFieldsFromControls(true);//But SheetNums will still be 0 for a new sheet.
			bool isNewReferralLetter=IsNewReferralLetter();
			if(SheetCur.IsNew) {
				Sheets.Insert(SheetCur);
				Sheets.SaveParameters(SheetCur);
				SheetCur.IsNew=false;
			}
			else {
				Sheets.Update(SheetCur);
			}
			DidChangeSheet=true;
			for(int i=0;i<SheetCur.SheetFields.Count;i++){//Set all sheetfield.SheetNums
				SheetCur.SheetFields[i].SheetNum=SheetCur.SheetNum;
			}
			if(!isNewReferralLetter) {
				//Don't need to do this for referral letters that have tooth charts or grids because 
				//we don't save their sheet fields. Instead, they are saved and accessed as PDFs. 
				SheetFields.Sync(SheetCur.SheetFields.FindAll(x => !x.FieldType.In(
				SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)),SheetCur.SheetNum,false);//Sync fields before sigBoxes
			}
			List<SheetField> listSheetFieldsSigBoxes=new List<SheetField>();
			//SigBoxes must come after ALL other types in order for the keyData to be in the right order.
			SheetField sheetField;
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(OpenDental.UI.SignatureBoxWrapper)) {
					continue;
				}
				if(panelMain.Controls[i].Tag==null) {
					continue;
				}
				sheetField=(SheetField)panelMain.Controls[i].Tag;
				OpenDental.UI.SignatureBoxWrapper sigBox=(OpenDental.UI.SignatureBoxWrapper)panelMain.Controls[i];
				if(!sigBox.GetSigChanged()) {
					//signature hasn't changed, add to the list of fields
					listSheetFieldsSigBoxes.Add(sheetField);
					continue;
				}
				//signature changed so clear out the current field data from the panelMain.Controls[i].Tag, get the new keyData, and get the signature string for the field
				sheetField.FieldValue="";
				//Refresh the fields so they are in the correct order. Don't need to do this for referral letters that have tooth charts or grids because
				//we don't save their sheet fields. Instead, they are saved and accessed as PDFs. 
				if(!isNewReferralLetter) {
					SheetFields.GetFieldsAndParameters(SheetCur);
				}
				string keyData=Sheets.GetSignatureKey(SheetCur);
				string signature=sigBox.GetSignature(keyData);
				sheetField.DateTimeSig=DateTime.MinValue;
				if(signature!="") {
					//This line of code is more readable, but uses ternary operator
					//field.FieldValue=(sigBox.GetSigIsTopaz()?1:0)+signature;
					sheetField.FieldValue="0";
					if(sigBox.GetSigIsTopaz()) {
						sheetField.FieldValue="1";
					}
					sheetField.FieldValue+=signature;
					if(sigBox.IsValid) {
						//Save date of modified signature in the sheetfield here.
						sheetField.DateTimeSig=DateTime.Now;
					}
				}
				listSheetFieldsSigBoxes.Add(sheetField);
			}
			//Create PDF for referral letter that has a grid and/or tooth chart.
			if(isNewReferralLetter) {
				Patient patient=Patients.GetPat(SheetCur.PatNum);
				try {
					if(!string.IsNullOrEmpty(_tempPdfFile)) {
						File.Delete(_tempPdfFile);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
				//Get a temporary location for the file
				_tempPdfFile=PrefC.GetRandomTempFile(".pdf");
				SheetPrinting.CreatePdf(SheetCur,_tempPdfFile,StatementCur,MedLabCur);
				//Import pdf, this will move the pdf into the correct location for the patient.
				long defNum=Defs.GetByExactName(DefCat.ImageCats,"Letters");
				if(defNum==0) {
					//This will throw an exception if all ImageCats defs are hidden.  However, many other places in this program make the same assumption that
					//at least one of these definitions is not hidden.
					Def def=Defs.GetCatList((int)DefCat.ImageCats).FirstOrDefault(x => !x.IsHidden);
					defNum=def.DefNum;
					MessageBox.Show(this,Lan.g(this,"The Image Category Definition \"Letters\" could not be found.  Referral letter saved to:\r\n")+def.ItemName);
				}
				Document doc=ImageStore.Import(_tempPdfFile,defNum,patient);
				//Update sheetCur with the docnum
				SheetCur.DocNum=doc.DocNum;
				Sheets.Update(SheetCur);
			}
			//now sync SigBoxes
			SheetFields.Sync(listSheetFieldsSigBoxes,SheetCur.SheetNum,true);
			SheetFields.GetFieldsAndParameters(SheetCur);
			//Each (SheetField)control has a tag pointing at a SheetCur.SheetField, and GetFieldsAndParameters() causes us to overwrite SheetCur.SheetFields.
			//This leaves the tag pointing at nothing, so we need to call LayoutFields() to re-link the controls and data.
			LayoutFields();
			if(SheetCur.ShowInTerminal>0) {
				Signalods.SetInvalid(InvalidType.Kiosk);
			}
			return true;
		}

		///<summary>This is always done before the save process.  But it's also done before bumping down fields due to growth behavior.</summary>
		private void FillFieldsFromControls(bool isSave=false){			
			//SheetField field;
			//Images------------------------------------------------------
				//Images can't be changed in this UI
			//RichTextBoxes-----------------------------------------------
			using RichTextBox richTextBoxFormatted=new RichTextBox(); //Used to compare text and only update when the user has changed something.
			for(int i=0;i<panelMain.Controls.Count;i++){
				//We no longer use ElementHost for WPF RichTextBoxes, only WinForm RichTextBox
				if(panelMain.Controls[i].GetType()!=typeof(RichTextBox) && panelMain.Controls[i].GetType()!=typeof(System.Windows.Forms.Integration.ElementHost)) {
					continue;
				}
				if(panelMain.Controls[i].Tag==null) {
					continue;
				}
				if(panelMain.Controls[i].GetType()==typeof(RichTextBox)) {//WinForms
					richTextBoxFormatted.Text="";
					//RichTextBox can alter the string being passed in. This compairison will ensure we are only updating the sheet value when the user has changes something.
					GraphicsHelper.CreateTextBoxForSheetDisplay((SheetField)panelMain.Controls[i].Tag,false,richTextBoxFormatted);
					if(panelMain.Controls[i].Text!=richTextBoxFormatted.Text) {
						((SheetField)panelMain.Controls[i].Tag).FieldValue=panelMain.Controls[i].Text;
					}
				}
				else if(panelMain.Controls[i].GetType()==typeof(System.Windows.Forms.Integration.ElementHost)) {//This will never be hit, since we are no longer using WPF RichTextBox. Leaving for documentation
					System.Windows.Forms.Integration.ElementHost elementHost=(System.Windows.Forms.Integration.ElementHost)panelMain.Controls[i];
					if(elementHost.Child==null || elementHost.Child.GetType()!=typeof(System.Windows.Controls.RichTextBox)) {
						continue;
					}
					System.Windows.Controls.RichTextBox richTextBox=(System.Windows.Controls.RichTextBox)elementHost.Child;
					((SheetField)panelMain.Controls[i].Tag).FieldValue=GetTextFromWpfRichTextBox(richTextBox);
				}
			}
			//CheckBoxes-----------------------------------------------
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(SheetCheckBox)){
					continue;
				}
				if(panelMain.Controls[i].Tag==null){
					continue;
				}
				//field=(SheetField)panelMain.Controls[i].Tag;
				if(((SheetCheckBox)panelMain.Controls[i]).IsChecked){
					((SheetField)panelMain.Controls[i].Tag).FieldValue="X";
				}
				else{
					((SheetField)panelMain.Controls[i].Tag).FieldValue="";
				}
			}
			//ComboBoxes-----------------------------------------------
			FillFieldsFromComboBoxes();
			//ToothChart------------------------------------------------
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(ScreenToothChart)) {
					continue;
				}
				if(panelMain.Controls[i].Tag==null) {
					continue;
				}
				ScreenToothChart screenToothChart=(ScreenToothChart)panelMain.Controls[i];
				List<UserControlScreenTooth> listUserControlScreenTeeth=null;
				if(screenToothChart.IsPrimary) {
					listUserControlScreenTeeth=screenToothChart.GetPrimaryTeeth;
				}
				else {
					listUserControlScreenTeeth=screenToothChart.GetTeeth;
				}
				string value="";
				if(screenToothChart.IsPrimary) {
					value+="1;";
				}
				else {
					value+="0;";
				}
				for(int j=0;j<listUserControlScreenTeeth.Count;j++) {
					if(j > 0) {
						value+=";";//Don't add ';' at very end or it will mess with .Split() logic.
					}
					value+=String.Join(",",listUserControlScreenTeeth[j].GetSelected());
				}
				((SheetField)panelMain.Controls[i].Tag).FieldValue=value;
			}
			//Rectangles and lines-----------------------------------------
				//Rectangles and lines can't be changed in this UI
			//Drawings----------------------------------------------------
				//Drawings data is already saved to fields
			//SigBoxes---------------------------------------------------
				//SigBoxes won't be strictly checked for validity
				//or data saved to the field until it's time to actually save to the database.
		}

		///<summary>Fills the sheet fields with their combo boxes.</summary>
		private void FillFieldsFromComboBoxes() {
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(SheetComboBox)) {
					continue;
				}
				if(panelMain.Controls[i].Tag==null) {
					continue;
				}
				SheetComboBox comboBox=(SheetComboBox)panelMain.Controls[i];
				((SheetField)panelMain.Controls[i].Tag).FieldValue=comboBox.ToFieldValue();
			}
		}

		///<summary>Returns true when all of the sheet fields with IsRequired set to true have a value set. Otherwise, a message box shows and false is returned.</summary>
		private bool VerifyRequiredFields(){
			FillFieldsFromControls();
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].Tag==null){
					continue;
				}
				if(panelMain.Controls[i].GetType()==typeof(RichTextBox)){
					SheetField sheetField=(SheetField)panelMain.Controls[i].Tag;
					if(sheetField.FieldType!=SheetFieldType.InputField){
						continue;
					}
					RichTextBox richTextBoxInput=(RichTextBox)panelMain.Controls[i];
					if(sheetField.IsRequired && richTextBoxInput.Text.Trim()==""){
						if(sheetField.FieldName=="misc" && !string.IsNullOrWhiteSpace(sheetField.ReportableName)) {
							MessageBox.Show(Lan.g(this,"You must enter a value for")+" "+sheetField.ReportableName+" "+Lan.g(this,"before continuing."));
						}
						else {
							MessageBox.Show(Lan.g(this,"You must enter a value for")+" "+sheetField.FieldName+" "+Lan.g(this,"before continuing."));
						}
						return false;			
					}	
				}
				else if(panelMain.Controls[i].GetType()==typeof(System.Windows.Forms.Integration.ElementHost)) {//This will never be hit, since we are no longer using WPF RichTextBox. Leaving for documentation
					SheetField sheetField=(SheetField)panelMain.Controls[i].Tag;
					if(sheetField.FieldType!=SheetFieldType.InputField) {
						continue;
					}
					System.Windows.Forms.Integration.ElementHost elementHost=(System.Windows.Forms.Integration.ElementHost)panelMain.Controls[i];
					if(elementHost.Child==null || elementHost.Child.GetType()!=typeof(System.Windows.Controls.RichTextBox)) {
						continue;
					}
					System.Windows.Controls.RichTextBox richTextBox=(System.Windows.Controls.RichTextBox)elementHost.Child;
					if(sheetField.IsRequired && GetTextFromWpfRichTextBox(richTextBox).Trim()=="") {
						if(sheetField.FieldName=="misc" && !string.IsNullOrWhiteSpace(sheetField.ReportableName)) {
							MessageBox.Show(Lan.g(this,"You must enter a value for")+" "+sheetField.ReportableName+" "+Lan.g(this,"before continuing."));
						}
						else {
							MessageBox.Show(Lan.g(this,"You must enter a value for")+" "+sheetField.FieldName+" "+Lan.g(this,"before continuing."));
						}
						return false;
					}
				}
				else if(panelMain.Controls[i].GetType()==typeof(OpenDental.UI.SignatureBoxWrapper)){
					SheetField sheetField=(SheetField)panelMain.Controls[i].Tag;
					if(!sheetField.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)){
						continue;
					}
					OpenDental.UI.SignatureBoxWrapper sigBox=(OpenDental.UI.SignatureBoxWrapper)panelMain.Controls[i];
					if(sheetField.IsRequired && (!sigBox.IsValid || sigBox.SigIsBlank)){
						MsgBox.Show(this,"Signature required");
						return false;
					}
				}
				else if(panelMain.Controls[i].GetType()==typeof(SheetCheckBox)){//Radio button groups or misc checkboxes
					SheetField sheetField=(SheetField)panelMain.Controls[i].Tag;
					if(sheetField.IsRequired && sheetField.FieldValue!="X"){//required but this one not checked
						//first, checkboxes that are not radiobuttons.  For example, a checkbox at bottom of web form used in place of signature.
						if(sheetField.RadioButtonValue=="" //doesn't belong to a built-in group
							&& sheetField.RadioButtonGroup=="") //doesn't belong to a custom group
						{
							//field.FieldName is always "misc"
							//int widthActual=(SheetCur.IsLandscape?SheetCur.Height:SheetCur.Width);
							//int heightActual=(SheetCur.IsLandscape?SheetCur.Width:SheetCur.Height);
							//int topMidBottom=(heightActual/3)
							MessageBox.Show(Lan.g(this,"You must check the required checkbox."));
							return false;
						}
						else{//then radiobuttons (of both kinds)
							//All radio buttons within a group should either all be marked required or all be marked not required. 
							//Not the most efficient check, but there won't usually be more than a few hundred items so the user will not ever notice. We can speed up later if needed.
							bool isValueSet=false;//we will be checking to see if at least one in the group has a value
							int numGroupButtons=0;//a count of the buttons in the group
							for(int j=0;j<panelMain.Controls.Count;j++){
								if(panelMain.Controls[j].GetType()!=typeof(SheetCheckBox)){
									continue;//skip everything that's not a checkbox
								}
								SheetField sheetField2=(SheetField)panelMain.Controls[j].Tag;
								//whether built-in or custom, this makes sure it's a match.
								//the other comparison will also match because they are empty strings
								if(sheetField2.RadioButtonGroup.ToLower()==sheetField.RadioButtonGroup.ToLower()//if they are in the same group ("" for built-in, some string for custom group)
									&& sheetField2.FieldName==sheetField.FieldName)//"misc" for custom group, some string for built in groups.
								{
									numGroupButtons++;
									if(sheetField2.FieldValue=="X"){
										isValueSet=true;
										break;
									}
								}
							}
							if(numGroupButtons>0 && !isValueSet){//there is not at least one radiobutton in the group that's checked.
								if(sheetField.RadioButtonGroup!="") {//if they are in a custom group
									MessageBox.Show(Lan.g(this,"You must select a value for radio button group")+" '"+sheetField.RadioButtonGroup+"'. ");
								}
								else {
									MessageBox.Show(Lan.g(this,"You must select a value for radio button group")+" '"+sheetField.FieldName+"'. ");
								}
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		private void SaveSignaturePayPlan() {
			PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(SheetCur.Parameters,"payplan").ParamValue;
			string keyData=(string)SheetParameter.GetParamByName(SheetCur.Parameters,"keyData").ParamValue;
			bool isValidSig=true;
			bool hasSigChanged=false;
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(SignatureBoxWrapper)) {
					continue;
				}
				if(panelMain.Controls[i].Tag==null) {
					continue;
				}
				OpenDental.UI.SignatureBoxWrapper signatureBoxWrapper=(OpenDental.UI.SignatureBoxWrapper)panelMain.Controls[i];
				if(!signatureBoxWrapper.IsValid) {//invalid sig. Do not save signature.
					isValidSig=signatureBoxWrapper.IsValid; 
					continue;
				}
				payPlan.Signature=signatureBoxWrapper.GetSignature(keyData);
				if(payPlan.Signature!="") {
					payPlan.SigIsTopaz=false;
					if(signatureBoxWrapper.GetSigIsTopaz()) {
						payPlan.SigIsTopaz=true; ;
					}
				}
				hasSigChanged=signatureBoxWrapper.GetSigChanged();
				PayPlans.Update(payPlan);
			}
			//save .pdf file if payPlan is new or signature has changed and signature is valid
			if((payPlan.IsNew || hasSigChanged) && isValidSig) {
				long category=0;
				//Determine the first category that this PP should be saved to.
				//"A"==payplan; see FormDefEditImages.cs
				//look at ContrTreat.cs to change it to handle more than one
				List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				for(int i = 0;i<listDefs.Count;i++) {
					if(Regex.IsMatch(listDefs[i].ItemValue,@"A")) {
						category=listDefs[i].DefNum;
						break;
					}
				}
				if(category==0) {
					List<Def> listImageCatDefsShort=Defs.GetDefsForCategory(DefCat.ImageCats,true);
					List<Def> listImageCatDefsLong=Defs.GetDefsForCategory(DefCat.ImageCats);
					if(listImageCatDefsShort.Count!=0) {
						category=listImageCatDefsShort[0].DefNum;//put it in the first category.
					}
					else if(listImageCatDefsLong.Count!=0) {//All categories are hidden
						category=listImageCatDefsLong[0].DefNum;//put it in the first category.
					}
					else {
						MsgBox.Show(this,"Error saving document. Unable to find image category.");
						return;
					}
				}
				string filePathAndName=PrefC.GetRandomTempFile(".pdf");
				SheetPrinting.CreatePdf(SheetCur,filePathAndName,null);
				//create doc--------------------------------------------------------------------------------------
				OpenDentBusiness.Document document=null;
				try {
					document=ImageStore.Import(filePathAndName,category,Patients.GetPat(payPlan.PatNum));
				}
				catch {
					MsgBox.Show(this,"Error saving document.");
					return;
				}
				document.Description="PPArchive"+document.DocNum+"_"+DateTime.Now.ToShortDateString();
				document.ImgType=ImageType.Document;
				document.DateCreated=DateTime.Now;
				Documents.Update(document);
				//remove temp file---------------------------------------------------------------------------------
				try {
					System.IO.File.Delete(filePathAndName);
				}
				catch { }
			}
		}

		///<summary>Save the document as PDF in every non-hidden image category with the supplied usage.  genericFileName should typically relate to the SheetType.</summary>
		private bool SaveAsDocument(char charUsage,string genericFileName="") {
			//Get all ImageCat defs for our usage that are not hidden.
			List<Def> listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true).Where(x => x.ItemValue.Contains(charUsage.ToString())).ToList();
			if(listDefsImageCat.IsNullOrEmpty()) {
				return true;
			}
			Patient patient=Patients.GetPat(SheetCur.PatNum);
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			string rawBase64="";
			SheetPrinting.CreatePdf(SheetCur,tempFile,StatementCur);
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));//Todo test this
			}
			for(int i=0;i<listDefsImageCat.Count;i++) {//usually only one, but do allow them to be saved once per image category.
				OpenDentBusiness.Document documentSave=new Document();
				documentSave.DocNum=Documents.Insert(documentSave);
				string fileName=genericFileName+documentSave.DocNum;
				documentSave.ImgType=ImageType.Document;
				documentSave.DateCreated=DateTime.Now;
				documentSave.PatNum=patient.PatNum;
				documentSave.DocCategory=listDefsImageCat[i].DefNum;
				documentSave.Description=fileName;//no extension.
				documentSave.RawBase64=rawBase64;//blank if using AtoZfolder
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					string filePath=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
					while(File.Exists(filePath+"\\"+fileName+".pdf")) {
						fileName+="x";
					}
					File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
				}
				else if(CloudStorage.IsCloudStorage) {
					//Upload file to patient's AtoZ folder
					using FormProgress formProgress=new FormProgress();
					formProgress.DisplayText="Uploading Treatment Plan...";
					formProgress.NumberFormat="F";
					formProgress.NumberMultiplication=1;
					formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					formProgress.TickMS=1000;
					OpenDentalCloud.Core.TaskStateUpload taskStateUpload=CloudStorage.UploadAsync(ImageStore.GetPatientFolder(patient,"")
						,fileName+".pdf"
						,File.ReadAllBytes(tempFile)
						,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
					if(formProgress.ShowDialog()==DialogResult.Cancel) {
						taskStateUpload.DoCancel=true;
						break;
					}
				}
				documentSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
				Documents.Update(documentSave);
			}
			try {
				File.Delete(tempFile); //cleanup the temp file.
			}
			catch(Exception e) {
				e.DoNothing();
			}
			return true;
		}

		private void butToKiosk_Click(object sender,EventArgs e) {
			if(IsNewReferralLetter()) {
				MsgBox.Show(this,"This sheet type cannot be sent to the kiosk.");
				return;//Since we're saving this as a PDF, there wouldn't be anything to show in the kiosk.
			}
			//Sets terminal view number to max(ViewNumber)+1
			int terminalNum=Sheets.GetMaxTerminalNum(SheetCur.PatNum)+1;
			//If terminalNum>255, an exception will be thrown when it's saved because it's stored as a byte.
			textShowInTerminal.Text=Math.Min(terminalNum,255).ToString();
			//This saves the data and updates the sheet in the database, allowing the terminal to fetch it.
			if(!TryToSaveData()){
				return;
			}
			//Push new sheet to eClipboard.
			OpenDentBusiness.WebTypes.PushNotificationUtils.CI_AddSheet(SheetCur.PatNum,SheetCur.SheetNum);
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,SheetCur.Description+" from "+SheetCur.DateTimeSheet.ToShortDateString());
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butAddField_Click(object sender,EventArgs e) {
			using FormSheetFieldAdd formSheetFieldAdd=new FormSheetFieldAdd();
			formSheetFieldAdd.SheetCur=SheetCur;//the field gets added to the sheet inside this form
			//It remains flagged as IsNew, which causes it to be saved to the database when OK button is clicked in this form.
			formSheetFieldAdd.ShowDialog();
			if(formSheetFieldAdd.DialogResult!=DialogResult.OK){
				return;
			}
			FillFieldsFromControls();
			FieldValueChanged(sender);//the sender doesn't matter
			LayoutFields();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			DeleteSheet(true);
		}

		///<summary>Deletes the current sheet and handles closing the form. Prompts the user to confirm if the sheet is currently being used by a 
		///kiosk.</summary>
		private void DeleteSheet(bool doPromptForDelete) {
			if(SheetCur.IsNew){
				DialogResult = DialogResult.Cancel;
				Close();
				return;
			}
			if(doPromptForDelete && !MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete?")) {
				return;
			}
			if(SheetCur.ShowInTerminal > 0) {
				string message="This form has been sent to be filled on a kiosk.  If you continue, the patient will lose any information that is "
					+"on their screen.\r\nContinue anyway?";
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
					return;
				}
			}
			if(SheetCur.SheetType==SheetTypeEnum.Screening) {
				Screens.DeleteForSheet(SheetCur.SheetNum);
			}
			Sheets.Delete(SheetCur.SheetNum,SheetCur.PatNum,SheetCur.ShowInTerminal);
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,SheetCur.Description
				+" "+Lan.g(this,"deleted from")+" "+SheetCur.DateTimeSheet.ToShortDateString());
			if(SheetCur.ShowInTerminal>0) {
				Signalods.SetInvalid(InvalidType.Kiosk);
			}
			SheetCur=null;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {//Payment plan saves and closes the window.
				if(!VerifyRequiredFields()) {
					return;
				}
				if(!DoSaveChanges()) {
					return;
				}
				SaveSignaturePayPlan();
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,SheetCur.Description+" from "+SheetCur.DateTimeSheet.ToShortDateString());
				DialogResult=DialogResult.OK;
				Close();
			}
			else if(SheetCur.SheetType==SheetTypeEnum.ExamSheet) {//Exam sheet saves and keeps the window open
				butSave.Text=Lans.g(this,"Save");//If they decide to click it again, change the button text back
				//Quit any threads still alive so that they do not change the button text too soon.
				ODThread.QuitAsyncThreadsByGroupName("FormSheetFillEdit_ShowSaved_"+_uniqueFormIdentifier);
				if(!VerifyRequiredFields()) {//If invalid, return.
					return;
				}
				if(!DoSaveChanges()) {
					return;
				}
				validateAndUpdateFonts(SheetCur);
				if(TryToSaveData()) {//If saved successful, show saved on the button.
					ShowSaved();
				}
			}
		}

		///<summary>Shows the saved text on the save button for 3 seconds after clicking the save exam button.</summary>
		private void ShowSaved() {
			butSave.Text=Lans.g(this,"Saved!");
			_timerSaveButtonText=new Timer();
			_timerSaveButtonText.Interval=3000;
			_timerSaveButtonText.Tick += new EventHandler(timerChangeSaveButtonText);
			_timerSaveButtonText.Enabled=true;
		}

		private void timerChangeSaveButtonText(object sender,EventArgs e) {
			butSave.Text=Lans.g(this, "Save");
			_timerSaveButtonText.Enabled=false;
		}

		private void butRestore_Click(object sender,EventArgs e) {
			SheetCur.IsDeleted=false;
			ValidateSaveAndExit();
		}

		private void butOK_Click(object sender,EventArgs e) {
			ValidateSaveAndExit();
		}

		private void ValidateSaveAndExit() {
			validateAndUpdateFonts(SheetCur);// validate fonts before saving
			if(!VerifyRequiredFields() || !DoSaveChanges() || !TryToSaveData()){
				return;
			}
			if(_isAutoSave && checkSaveToImages.Checked) {
				SaveAsDocument('U',"PatientForm");
			}
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,SheetCur.Description+" from "+SheetCur.DateTimeSheet.ToShortDateString());
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>Checks to see if edits have been made by another user. If so, user must confirm before saving changes.</summary>
		private bool DoSaveChanges() {
			//The DB sheet may not have truly had any changes for current sheet, but someone opened the sheet and clicked OK.
			if(!SheetCur.IsNew
				&& SheetCur.SheetNum!=0
				&& (Sheets.GetOne(SheetCur.SheetNum)?.DateTSheetEdited??DateTime.MinValue)>SheetCur.DateTSheetEdited
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,
					"There have been changes to this sheet since it was loaded.  If you continue those changes will be overwritten.  Continue anyway?")) {
				return false;
			}
			return true;
		}

		private bool IsSignatureStarted() {
			for(int i=0;i<_listSignatureBoxWrappers.Count;i++){
				if(_listSignatureBoxWrappers[i].IsSigStarted) {
					return true;
				}
			}
			return false;
		}

		///<summary>validates the fonts for use with PDF sharp. If not compatible, sets the font to something that will work.</summary>
		//This is a workaround due to the fact that PDF Sharp does not support TTC fonts.
		private void validateAndUpdateFonts(Sheet sheetCur,bool isPrinting=false) {
			bool hasErrors=false;
			List<string> listStrBadFonts=new List<string>();
			try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
				XFont _=new XFont(sheetCur.FontName,sheetCur.FontSize,XFontStyle.Regular);
			}
			catch {
				hasErrors=true;
				listStrBadFonts.Add(sheetCur.FontName);
				sheetCur.FontName=FontFamily.GenericMonospace.ToString();//font was not compatible with PDFSharp, fill with one we hope is. Same font replacement we use in SheetDrawingJob.cs
			}
			//check every font in fields on the sheet
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType==SheetFieldType.StaticText
					||SheetCur.SheetFields[i].FieldType==SheetFieldType.InputField
					||SheetCur.SheetFields[i].FieldType==SheetFieldType.OutputText) {
					try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
						XFont _=new XFont(SheetCur.SheetFields[i].FontName,SheetCur.SheetFields[i].FontSize,XFontStyle.Regular);
					}
					catch {
						hasErrors=true;
						if(!listStrBadFonts.Contains(SheetCur.SheetFields[i].FontName)) {
							listStrBadFonts.Add(SheetCur.SheetFields[i].FontName);
						}
						SheetCur.SheetFields[i].FontName=FontFamily.GenericMonospace.ToString();//font was not compatible with PDFSharp, fill with one we hope is. Same font replacement we use in SheetDrawingJob.cs
					}
				}
			}
			if(isPrinting && hasErrors) {
				MsgBox.Show(Lan.g(this,$"Form is trying to save or print with unsupported font(s): {string.Join(", ",listStrBadFonts.ToArray())}. Font(s) will be replaced with a generic substitute to allow saving and printing."));
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_listSignatureBoxWrappers!=null) {
				for(int i=0;i<_listSignatureBoxWrappers.Count;i++) {
					//No longer accept input on signature box controls just in case they are currently accepting input.
					//Topaz signature pads need to disable access to the COM or USB port before disposing.
					_listSignatureBoxWrappers[i]?.SetTabletState(0);
				}
			}
		}

	
	}
}