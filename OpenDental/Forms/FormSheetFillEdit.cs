using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
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
		#region Fields - private
		///<summary>Statements use Sheets needs access to the entire Account data set for measuring grids.  See RefreshPanel()</summary>
		private DataSet _dataSet;
		///<summary>True if export as CSV was checked in FormStatementOptions.cs</summary>
		private bool _doExportCSV;
		///<summary>When user clicks on text, we calculate the idx within the string where they clicked.  It's class level so that we can draw it during debugging.</summary>
		private int _idxSelectedChar=-1;
		private bool _isMouseDown;
		///<summary>True if the user is auto-saving a patient form.</summary>
		private bool _isAutoSave;
		///<summary>A list of points for a pen drawing currently being drawn.  Once the mouse is raised, this list gets cleared.</summary>
		private List<Point> _listPoints;
		///<summary>When user clicks on text, this list of rectangles is created, on for each char, so that we can hit test where they clicked.  It's class level so that we can draw the rectangles during debugging.</summary>
		private List<RectangleF> _listRectangleFsChars=new List<RectangleF>();
		private List<SignatureBoxWrapper> _listSignatureBoxWrappers=new List<SignatureBoxWrapper>();
		///<summary>Only used here to draw the dashed margin lines.</summary>
		private Margins _marginsPrint=new Margins(0,0,40,60);
		///<summary>The location where the PDF file has been created.</summary>
		private string _tempPdfFile="";
		///<summary>Used to change the text on the "Save" button of the form.</summary>
		private Timer _timerSaveButtonText;
		///<summary>Creates a unique identifier for this instance of the form. This can be used when creating a thread with a unique group name.</summary>
		private string _uniqueFormIdentifier;
		#endregion Fields - private

		#region Fields - public
		///<summary>Indicates to the calling form that the sheet was inserted/updated.</summary>
		public bool DidChangeSheet;
		///<summary>True if the user sent an email from this window.</summary>
		public bool HasEmailBeenSent;
		///<summary>When in terminal, some options are not visible.</summary>
		public bool IsInTerminal;
		///<summary>If true, the sheet cannot be edited, deleted, changed patient, printed, or PDFed.
		///The main goal of this setting is to stop the user from being able to do anything with the sheet except view it.
		///It is mainly used when importing web forms so that the user importing the forms can make better informed decisions.</summary>
		public bool IsReadOnly;
		public bool IsRxControlled;
		///<summary>Used for statements, do not save a sheet version of the statement.</summary>
		public bool IsStatement;
		public MedLab MedLabCur;
		///<summary>A method that will be invoked when printing/email/creating PDF of a statement.</summary>
		public SaveStatementToDocDelegate SaveStatementToDocDelegate;
		///<summary>Will be null if deleted.</summary>
		public Sheet SheetCur;
		public Statement StatementCur;

		#endregion Fields - public

		#region Constructor
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
		#endregion Constructor

		#region Methods - public
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
		#endregion Methods - public

		#region Methods - Event Handlers
		private void butAddField_Click(object sender,EventArgs e) {
			using FormSheetFieldAdd formSheetFieldAdd=new FormSheetFieldAdd();
			formSheetFieldAdd.SheetCur=SheetCur;//the field gets added to the sheet inside this form
			//It remains flagged as IsNew, which causes it to be saved to the database when OK button is clicked in this form.
			formSheetFieldAdd.ShowDialog();
			if(formSheetFieldAdd.DialogResult!=DialogResult.OK){
				return;
			}
			FillFieldsFromControls();
			LoadImages();
			LayoutFields();
			ClearSigs();
			panelMain.Invalidate();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
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

		private void butDelete_Click(object sender,EventArgs e) {
			DeleteSheet(true);
		}

		private void butEmail_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetterWithProcsOrChart();
			if(!ValidateStateField()) {
				return;
			}
			FixFontsForPdf(SheetCur,true);// validate fonts before printing/creating PDF
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

		private void butOK_Click(object sender,EventArgs e) {
			ValidateSaveAndExit();
		}

		private void butPDF_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetterWithProcsOrChart();
			if(!ValidateStateField()) {
				return;
			}
			FixFontsForPdf(SheetCur,true);// validate fonts before printing/creating PDF
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

		private void butPrint_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetterWithProcsOrChart();
			if(!ValidateStateField()) {
				return;
			}
			FixFontsForPdf(SheetCur,true);// validate fonts before printing/creating PDF
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

		private void butPrintOrEmail_Click(object sender,EventArgs e) {
			//Statements and referral letters with grids or toothcharts are the only sheets that should not refresh from the db before printing.
			bool hasSheetFromDb=!IsStatement && !IsNewReferralLetterWithProcsOrChart();
			if(!ValidateStateField()) {
				return;
			}
			FixFontsForPdf(SheetCur,true);// validate fonts before printing/creating PDF
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

		private void butRestore_Click(object sender,EventArgs e) {
			SheetCur.IsDeleted=false;
			ValidateSaveAndExit();
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ValidateStateField()) {
				return;
			}
			if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {//Payment plan saves and closes the window.
				if(!VerifyRequiredFields()) {
					return;
				}
				if(!OkToSaveBecauseNoOtherEdits()) {
					return;
				}
				SaveSignaturePayPlan();
				SecurityLogs.MakeLogEntry(Permissions.SheetEdit,SheetCur.PatNum,SheetCur.Description+" from "+SheetCur.DateTimeSheet.ToShortDateString());
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			if(SheetCur.SheetType==SheetTypeEnum.ExamSheet) {//Exam sheet saves and keeps the window open
				butSave.Text=Lans.g(this,"Save");//If they decide to click it again, change the button text back
				//Quit any threads still alive so that they do not change the button text too soon.
				ODThread.QuitAsyncThreadsByGroupName("FormSheetFillEdit_ShowSaved_"+_uniqueFormIdentifier);
				if(!VerifyRequiredFields()) {//If invalid, return.
					return;
				}
				if(!OkToSaveBecauseNoOtherEdits()) {
					return;
				}
				FixFontsForPdf(SheetCur);
				if(TryToSaveData()) {//If saved successful, show saved on the button.
					ShowSaved();
				}
			}
		}
		
		private void butToKiosk_Click(object sender,EventArgs e) {
			if(IsNewReferralLetterWithProcsOrChart()) {
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

		private void checkErase_Click(object sender,EventArgs e) {
			if(checkErase.Checked){
				panelMain.Cursor=new Cursor(GetType(),"EraseCircle.cur");
			}
			else{
				panelMain.Cursor=Cursors.Default;
			}
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

		private void FormSheetFillEdit_Load(object sender,EventArgs e) {
			if(SheetCur.IsLandscape){
				Width=(int)Math.Max(LayoutManager.ScaleF(SheetCur.Height+190),butOK.Right+LayoutManager.ScaleF(27));
				Height=(int)LayoutManager.ScaleF(SheetCur.Width+65);
			}
			else{
				Width=(int)Math.Max(LayoutManager.ScaleF(SheetCur.Width+190),butOK.Right+LayoutManager.ScaleF(27));
				Height=(int)LayoutManager.ScaleF(SheetCur.Height+65);
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
				butPrintOrEmail.Visible=false;
				butPDF.Visible=false;
				butDelete.Visible=false;
				butChangePat.Visible=false;
				butPrint.Visible=false;
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
					butPrintOrEmail.Visible=false;
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
			int bottomLastField=0;
			if(SheetCur.SheetFields.Count>0) {
				bottomLastField=SheetCur.SheetFields.Max(x=>x.Bounds.Bottom);
			}
			LayoutManager.MoveHeight(panelMain,LayoutManager.Scale(Math.Max(SheetCur.HeightPage,bottomLastField)));//+20 for Hscrollbar?
			textDateTime.Text=SheetCur.DateTimeSheet.ToShortDateString()+" "+SheetCur.DateTimeSheet.ToShortTimeString();
			textDescription.Text=SheetCur.Description;
			textNote.Text=SheetCur.InternalNote;
			if(SheetCur.ShowInTerminal>0) {
				textShowInTerminal.Text=SheetCur.ShowInTerminal.ToString();
			}
			LoadImages();
			string strErr=LayoutFields();
			if(!string.IsNullOrWhiteSpace(strErr)) {
				MsgBox.Show(this,strErr);//An invalid SheetField was repaired.
			}
			if(IsReadOnly) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
				butChangePat.Enabled=false;
				butPrintOrEmail.Enabled=false;
				butPDF.Enabled=false;
				butPrint.Enabled=false;
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

		private void FormSheetFillEdit_MouseUp(object sender,MouseEventArgs e) {
			//panelScroll.Focus();
		}

		private void FormSheetFillEdit_MouseWheelScroll(object sender,MouseEventArgs e) {
			panelScroll.Focus();
		}
		
		private void menuItemCombo_Click(object sender,EventArgs e) {
			MenuItem menuItem=(MenuItem)sender;
			SheetField sheetField=(SheetField)menuItem.Tag;
			SheetFields.SetComboFieldValue(sheetField,menuItem.Text);
			panelMain.Invalidate();
		}

		private void panelMain_MouseDoubleClick(object sender,MouseEventArgs e) {
			Point pointSheet=new Point(LayoutManager.Unscale(e.X),LayoutManager.Unscale(e.Y));
			SheetField sheetField=HitTest(pointSheet);
			if(sheetField==null) {
				return;
			}
			if(sheetField.FieldType!=SheetFieldType.PatImage){
				return;
			}
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
			ClearSigs();
			sheetField.BitmapLoaded?.Dispose();
			sheetField.BitmapLoaded=null;
			LoadImageOnePat(sheetField);
			panelMain.Invalidate();
		}

		private void panelMain_MouseDown(object sender,MouseEventArgs e) {
			_isMouseDown=true;
			if(checkErase.Checked){
				return;
			}
			Point pointSheet=new Point(LayoutManager.Unscale(e.X),LayoutManager.Unscale(e.Y));
			SheetField sheetField=HitTest(pointSheet);
			if(sheetField?.FieldType==SheetFieldType.CheckBox){
				if(sheetField.FieldValue==""){
					sheetField.FieldValue="X";
					if(sheetField.RadioButtonValue!="" || sheetField.RadioButtonGroup!=""){
						//this is a radioButton, so uncheck others in the group
						for(int i=0;i<SheetCur.SheetFields.Count;i++){
							if(sheetField==SheetCur.SheetFields[i]) {
								continue;//skip self
							}
							if(sheetField.FieldName!=SheetCur.SheetFields[i].FieldName) {
								continue;//not in this radio group
							}
							//If both checkbox field names are set to "misc" then we instead use the RadioButtonGroup as the actual radio button group name.
							if(sheetField.FieldName=="misc" && sheetField.RadioButtonGroup!=SheetCur.SheetFields[i].RadioButtonGroup){
								continue;
							}
							SheetCur.SheetFields[i].FieldValue="";
						}
					}
				}
				else{
					sheetField.FieldValue="";
				}
				ClearSigs();
				panelMain.Invalidate();
				return;
			}
			if(sheetField?.FieldType==SheetFieldType.InputField
				|| sheetField?.FieldType==SheetFieldType.StaticText
				|| sheetField?.FieldType==SheetFieldType.OutputText)
			{
				CreateFloatingTextBox(sheetField,e.Location);
				ClearSigs();
				return;
			}
			if(sheetField?.FieldType==SheetFieldType.PatImage){
				//Can double click, but not draw
				return;
			}
			if(sheetField?.FieldType==SheetFieldType.ComboBox){
				CreateFloatingComboOptions(sheetField);
				ClearSigs();
				return;
			}
			//Can draw everywhere else
			if(_listSignatureBoxWrappers.Any(x=>x.IsSigStarted)) {//if they already started a sig, then prevent drawing so they don't invalidate the sig.
				return;
			}
			_listPoints.Add(pointSheet);
		}

		private void panelMain_MouseMove(object sender,MouseEventArgs e) {
			if(!_isMouseDown){
				return;
			}
			Point pointSheet=new Point(LayoutManager.Unscale(e.X),LayoutManager.Unscale(e.Y));
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
				//Next line is in sheet coords
				PointF pointFEraser=new PointF(pointSheet.X+LayoutManager.Unscale(8.49f),pointSheet.Y+LayoutManager.Unscale(8.49f));
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
								panelMain.Invalidate();
								return;
							}
						}
					}
				}	
				return;
			}
			if(_listPoints.Count==0){
				return;
			}
			//Add to existing pen drawing list
			_listPoints.Add(pointSheet);
			//Directly draw the last segement instead of invalidating because it's a little faster
			using Graphics g=Graphics.FromHwnd(panelMain.Handle);
			g.SmoothingMode=SmoothingMode.HighQuality;
			float scale=LayoutManager.ScaleF(1);
			g.ScaleTransform(scale,scale);
			Pen pen=new Pen(Brushes.Black,2f);			
			int idx=_listPoints.Count-1;
			g.DrawLine(pen,_listPoints[idx-1].X,_listPoints[idx-1].Y,_listPoints[idx].X,_listPoints[idx].Y);
		}

		private void panelMain_MouseUp(object sender,MouseEventArgs e) {
			_isMouseDown=false;
			if(checkErase.Checked){
				return;
			}
			if(_listPoints.Count==0){
				return;
			}
			//Save pen drawing
			SheetField sheetField=new SheetField();
			sheetField.FieldType=SheetFieldType.Drawing;
			sheetField.FieldName="";
			sheetField.FieldValue="";
			for(int i=0;i<_listPoints.Count;i++){
				if(i>0){
					sheetField.FieldValue+=";";
				}
				sheetField.FieldValue+=_listPoints[i].X.ToString()+","+_listPoints[i].Y.ToString();
			}
			sheetField.FontName="";
			sheetField.RadioButtonValue="";
			SheetCur.SheetFields.Add(sheetField);
			ClearSigs();
			_listPoints.Clear();
			panelScroll.Focus();
		}

		private void panelMain_Paint(object sender,PaintEventArgs e) {
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
				//I tried all the rendering hints.  None of them match the popup textboxes.
				//I made more comments about that in CreateFloatingTextBox().
				//AntiAliasGridFit looks blocky
				//ClearTypeGridFit;
				//AntiAlias is not the best quality.  Looks fuzzy.
			g.Clear(Color.White);
			if(DesignMode){
				g.DrawRectangle(Pens.Black,0,0,panelMain.Width-1,panelMain.Height-1);
				return;
			}
			float scale=LayoutManager.ScaleF(1);
			g.ScaleTransform(scale,scale);
			//Static Image------------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.Image)){
					continue;
				}
				if(SheetCur.SheetFields[i].BitmapLoaded is null){
					continue;
				}
				g.DrawImage(SheetCur.SheetFields[i].BitmapLoaded,SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos,SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].Height);
			}
			//PatImage----------------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.PatImage)){
					continue;
				}
				if(SheetCur.SheetFields[i].BitmapLoaded==null){
					g.DrawRectangle(Pens.Black,SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos,SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].Height);
					using Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.UnscaleMS(8.25f));
					float y=SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height/2;
					g.DrawString("Double click to add an image",font,Brushes.Black,SheetCur.SheetFields[i].XPos,y);
					continue;
				}
				Size sizeField=new Size(SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].Height);
				float scaleImage=ImageTools.CalcScaleFit(sizeField,SheetCur.SheetFields[i].BitmapLoaded.Size,0);
				//center the image in the bounds of the field
				g.DrawImage(SheetCur.SheetFields[i].BitmapLoaded,
					SheetCur.SheetFields[i].XPos+(SheetCur.SheetFields[i].Width-SheetCur.SheetFields[i].BitmapLoaded.Width*scaleImage)/2f,
					SheetCur.SheetFields[i].YPos+(SheetCur.SheetFields[i].Height-SheetCur.SheetFields[i].BitmapLoaded.Height*scaleImage)/2f,
					SheetCur.SheetFields[i].BitmapLoaded.Width*scaleImage,
					SheetCur.SheetFields[i].BitmapLoaded.Height*scaleImage);
			}
			//text--------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.InputField,SheetFieldType.OutputText,SheetFieldType.StaticText)){
					continue;
				}
				Rectangle rectangle=new Rectangle(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos,SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].Height);
				if(SheetCur.SheetFields[i].FieldType==SheetFieldType.InputField){
					Color colorBack=Color.FromArgb(245,245,200);
					if(!panelMain.Enabled) {
						colorBack=Color.FromArgb(240,240,240);//light gray for disabled
					}
					using Brush brushBack=new SolidBrush(colorBack);
					g.FillRectangle(brushBack,rectangle);
				}
				FontStyle fontStyle=FontStyle.Regular;
				if(SheetCur.SheetFields[i].FontIsBold) {
					fontStyle=FontStyle.Bold;
				}
				using Font font=new Font(SheetCur.SheetFields[i].FontName,LayoutManager.UnscaleMS(SheetCur.SheetFields[i].FontSize),fontStyle);
				Color colorText=SheetCur.SheetFields[i].ItemColor;
				if(SheetCur.SheetFields[i].FieldType==SheetFieldType.InputField){
					colorText=Color.Black;
				}
				StringFormat stringFormat=new StringFormat();
				stringFormat.Alignment=StringAlignment.Near;
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.InputField){
					if(SheetCur.SheetFields[i].TextAlign==HorizontalAlignment.Center){
						stringFormat.Alignment=StringAlignment.Center;
					}
					if(SheetCur.SheetFields[i].TextAlign==HorizontalAlignment.Right){
						stringFormat.Alignment=StringAlignment.Far;
					}
				}
				using Brush brushText=new SolidBrush(colorText);
				g.DrawString(SheetCur.SheetFields[i].FieldValue,font,brushText,rectangle,stringFormat);
				stringFormat?.Dispose();
			}
			//Checkbox--------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.CheckBox) {
					continue;
				}
				if(SheetCur.SheetFields[i].FieldValue=="") {
					continue;
				}
				//FieldValue is X
				using Pen pen=new Pen(Color.Black);//checkboxes have no color choice, and they've always been drawn with 1 px thickness
				g.DrawLine(pen,SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos,SheetCur.SheetFields[i].XPos+SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height);
				g.DrawLine(pen,SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height,SheetCur.SheetFields[i].XPos+SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].YPos);
			}
			//Combobox-----------------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.ComboBox)){
					continue;
				}
				Rectangle rectangle=new Rectangle(SheetCur.SheetFields[i].XPos,SheetCur.SheetFields[i].YPos,SheetCur.SheetFields[i].Width,SheetCur.SheetFields[i].Height);
				Color colorBack=Color.FromArgb(245,245,200);
				if(!panelMain.Enabled) {
					colorBack=Color.FromArgb(240,240,240);//light gray for disabled
				}
				using Brush brushBack=new SolidBrush(colorBack);
				g.FillRectangle(brushBack,rectangle);
				//Dropdown arrow to the right that indicates that this is comboBox
				float xPosArrowStart=SheetCur.SheetFields[i].XPos+SheetCur.SheetFields[i].Width;
				float yPosArrowStart=SheetCur.SheetFields[i].YPos+(SheetCur.SheetFields[i].Height/2f);
				using Pen _penArrow=new Pen(Color.FromArgb(20,20,20),1.5f);
				g.DrawLine(_penArrow,
					x1:xPosArrowStart-LayoutManager.ScaleF(13),
					y1:yPosArrowStart-LayoutManager.ScaleF(1.5f),
					x2:xPosArrowStart-LayoutManager.ScaleF(9.5f),
					y2:yPosArrowStart+LayoutManager.ScaleF(1.5f));
				g.DrawLine(_penArrow,
					x1:xPosArrowStart-LayoutManager.ScaleF(9.5f),
					y1:yPosArrowStart+LayoutManager.ScaleF(1.5f),
					x2:xPosArrowStart-LayoutManager.ScaleF(6),
					y2:yPosArrowStart-LayoutManager.ScaleF(1.5f));
				//combobox has no font options like fontname, size, color, bold, or align
				using Font font=new Font(FontFamily.GenericSansSerif,LayoutManager.UnscaleMS(8.25f));
				string str=SheetFields.GetComboSelectedOption(SheetCur.SheetFields[i]);
				g.DrawString(str,font,Brushes.Black,rectangle);
			}
			//Line and rectangle------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.Rectangle,SheetFieldType.Line)){
					continue;
				}
				Color colorText=Color.Black;
				if(SheetCur.SheetFields[i].ItemColor.ToArgb()!=0){
					colorText=SheetCur.SheetFields[i].ItemColor;
				}
				using Pen pen=new Pen(colorText,1f);//This 1 gets scaled up slightly if there is any scaling
				if(SheetCur.SheetFields[i].FieldType==SheetFieldType.Line){
					g.DrawLine(pen,
						SheetCur.SheetFields[i].XPos,
						SheetCur.SheetFields[i].YPos,
						SheetCur.SheetFields[i].XPos+SheetCur.SheetFields[i].Width,
						SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height);
				}
				if(SheetCur.SheetFields[i].FieldType==SheetFieldType.Rectangle){
					g.DrawRectangle(pen,
						SheetCur.SheetFields[i].XPos,
						SheetCur.SheetFields[i].YPos,
						SheetCur.SheetFields[i].Width,
						SheetCur.SheetFields[i].Height);
				}
			}
			//Pen drawings-----------------------------------------------------------------------------------------------------------------
			using Pen penDraw=new Pen(Brushes.Black,2f);//This 2 gets scaled up slightly if there is any scaling
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.Drawing)){
					continue;
				}
				string[] stringArrayPoints=SheetCur.SheetFields[i].FieldValue.Split(';');
				List<Point> listPoints=new List<Point>();
				for(int p=0;p<stringArrayPoints.Length;p++){
					string[] stringArrayXy=stringArrayPoints[p].Split(',');
					if(stringArrayXy.Length==2){
						Point point=new Point(PIn.Int(stringArrayXy[0]),PIn.Int(stringArrayXy[1]));
						listPoints.Add(point);
					}
				}
				for(int pt=1;pt<listPoints.Count;pt++){
					g.DrawLine(penDraw,listPoints[pt-1].X,listPoints[pt-1].Y,listPoints[pt].X,listPoints[pt].Y);
				}
			}
			//and the partial drawing with the mouse still down.  This is mostly drawn with graphics commands in MouseMove, but this is a backup
			for(int i=1;i<_listPoints.Count;i++){
				g.DrawLine(penDraw,_listPoints[i-1].X,_listPoints[i-1].Y,_listPoints[i].X,_listPoints[i].Y);
			}
			//Grid--------------------------------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.Grid)){
					continue;
				}
				SheetPrinting.DrawFieldGrid(SheetCur.SheetFields[i],SheetCur,g,null,_dataSet,StatementCur,MedLabCur,scaleMS:LayoutManager.GetScaleMS());
			}
			//Special----------------------------------------------------------------------------------------------------------------------
			///Rare. Referral letter tooth chart seems to the only example because the chart module controls are only shown in FormSheetDefEdit, not FormSheetFillEdit.
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.Special)){
					continue;
				}
				OpenDentBusiness.SheetPrinting.DrawFieldSpecial(SheetCur,SheetCur.SheetFields[i],g,null,0);
			}
			//for testing
			//if(_listRectangleFsChars.Count>0){
			//	g.DrawRectangles(Pens.Red,_listRectangleFsChars.ToArray());
			//}
			//if(_idxSelectedChar>-1){
			//	if(_idxSelectedChar==_listRectangleFsChars.Count){
			//		g.DrawLine(Pens.Blue,_listRectangleFsChars[_idxSelectedChar-1].Right,_listRectangleFsChars[_idxSelectedChar-1].Top,_listRectangleFsChars[_idxSelectedChar-1].Right,_listRectangleFsChars[_idxSelectedChar-1].Bottom);
			//	}
			//	else{
			//		g.DrawLine(Pens.Blue,_listRectangleFsChars[_idxSelectedChar].Left,_listRectangleFsChars[_idxSelectedChar].Top,_listRectangleFsChars[_idxSelectedChar].Left,_listRectangleFsChars[_idxSelectedChar].Bottom);
			//	}
			//}
		}

		private void panelScroll_MouseUp(object sender,MouseEventArgs e) {
			//panelScroll.Focus();
		}

		private void TextBox_PreviewKeyDown(object sender,PreviewKeyDownEventArgs e) {
			//preview is used because Tab doesn't hit KeyDown because it's intercepted by Windows to select the next control
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)sender;
			SheetField sheetField=(SheetField)textBox.Tag;
			if(e.KeyCode==Keys.Tab
				|| e.KeyCode==Keys.Enter && !textBox.Multiline)
			{
				int tabOrder=sheetField.TabOrder;
				sheetField.FieldValue=textBox.Text.Trim();
				textBox.Dispose();
				panelMain.Controls.Remove(textBox);
				if(tabOrder==0){
					return;
				}
				//No, this logic will not wrap around to the first tab order.  Nobody cares.
				List<SheetField> listSheetFields=SheetCur.SheetFields.FindAll(x=>x.TabOrder>tabOrder && x.FieldType==SheetFieldType.InputField).OrderBy(x=>x.TabOrder).ToList();
				if(listSheetFields.Count==0){
					return;
				}
				SheetField sheetFieldNext=listSheetFields[0];
				CreateFloatingTextBox(sheetFieldNext,Point.Empty);
			}
		}

		private void TextBox_LostFocus(object sender,EventArgs e) {
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)sender;
			SheetField sheetField=(SheetField)textBox.Tag;
			sheetField.FieldValue=textBox.Text.Trim();
			//int scroll=panelScroll.VerticalScroll.Value;
			textBox.Dispose();
			panelMain.Controls.Remove(textBox);
			//panelScroll.VerticalScroll.Value=scroll;
		}

		private void textBox_TextChanged(object sender,EventArgs e) {
			timerTextChanged.Stop();
			timerTextChanged.Tag=sender;
			timerTextChanged.Start();
		}

		private void timerTextChanged_Tick(object sender,EventArgs e) {
			timerTextChanged.Stop();
			System.Windows.Forms.TextBox textBox=(System.Windows.Forms.TextBox)timerTextChanged.Tag;
			timerTextChanged.Tag=null;
			SheetField sheetField=(SheetField)textBox.Tag;
			string fieldValue=textBox.Text;
			//sheetField.FieldValue=textBox.Text;
			int cursorPos=textBox.SelectionStart;
			if(sheetField.GrowthBehavior==GrowthBehaviorEnum.None){
				return;
			}
			FontStyle fontstyle=FontStyle.Regular;
			if(sheetField.FontIsBold){
				fontstyle=FontStyle.Bold;
			}
			using Font font=new Font(sheetField.FontName,sheetField.FontSize,fontstyle);
			using Graphics g=this.CreateGraphics();
			//no need worry about scaling here. Not sure why.
			SizeF sizeF=g.MeasureString(fieldValue,font,sheetField.Width);
			int calcH=(int)sizeF.Height;
			calcH+=font.Height+2;//add one line just in case.
			if(calcH<=sheetField.Height){//no growth needed. 
				return;
			}
			//the field height needs to change, so:
			//calculate growth in 96dpi
			int amountOfGrowth=calcH-sheetField.Height;
			sheetField.Height=calcH;
			LayoutManager.MoveHeight(textBox,LayoutManager.Scale(calcH));
			//Growth of entire form.
			//seems like we should instead be changing the sheet height, but that's probably handled somewhere else.
			LayoutManager.MoveHeight(panelMain,panelMain.Height+amountOfGrowth);
			if(sheetField.GrowthBehavior==GrowthBehaviorEnum.DownGlobal) {
				SheetUtil.MoveAllDownBelowThis(SheetCur,sheetField,amountOfGrowth);
			}
			else if(sheetField.GrowthBehavior==GrowthBehaviorEnum.DownLocal) {
				SheetUtil.MoveAllDownWhichIntersect(SheetCur,sheetField,amountOfGrowth);
			}
			RepositionControls();
		}

		private void timerChangeSaveButtonText(object sender,EventArgs e) {
			butSave.Text=Lans.g(this, "Save");
			_timerSaveButtonText.Enabled=false;
		}
		#endregion Methods - Event Handlers

		#region Methods - private
		///<summary>Creates the dropdown list using a context menu.</summary>
		private void CreateFloatingComboOptions(SheetField sheetField){
			ContextMenu contextMenu=new ContextMenu();
			List<string> listItems=SheetFields.GetComboMenuItems(sheetField);
			for(int i=0;i<listItems.Count;i++){
				MenuItem menuItem=new MenuItem(listItems[i],menuItemCombo_Click);
				menuItem.Tag=sheetField;
				contextMenu.MenuItems.Add(menuItem);
			}
			int yPos=LayoutManager.Scale(sheetField.YPos);
			int height=LayoutManager.Scale(sheetField.Height);
			//int scrollVal=panelScroll.VerticalScroll.Value;
			Point point=new Point(LayoutManager.Scale(sheetField.XPos),yPos+height);//+scrollVal);
			contextMenu.Show(panelMain,point);//Can't resize width, it's done according to width of items.
		}

		///<summary>This is for an input field, static text, or output text to edit text, and then it goes away. The point passed in is so that we can put the cursor in the right place.</summary>
		private void CreateFloatingTextBox(SheetField sheetField,Point point){
			//Convert \n that are by themselves to \r\n. Explanation is in SheetsInternal.GetSheetFromResource().
			string text=Regex.Replace(sheetField.FieldValue,@"(?<!\r)\n","\r\n");
			_idxSelectedChar=HitTestChars(sheetField,point,text);
			//panelMain.Invalidate();//for testing
			//return;
			//Now, create the textBox==========================================================================================================================
			System.Windows.Forms.TextBox textBox=new System.Windows.Forms.TextBox();
			textBox.BorderStyle=BorderStyle.None;
			if(sheetField.FieldType==SheetFieldType.InputField){
				textBox.BackColor=Color.FromArgb(245,245,200);
			}
			textBox.TabStop=(sheetField.TabOrder>0);
			textBox.TabIndex=sheetField.TabOrder;
			textBox.Location=new Point(LayoutManager.Scale(sheetField.XPos+2.5f),LayoutManager.Scale(sheetField.YPos));//small adjustments made to help it line up with orig.
			textBox.Width=LayoutManager.Scale(sheetField.Width-1f);//tested this with left and right align
			if(sheetField.ItemColor!=Color.FromArgb(0)){//if not empty
				textBox.ForeColor=sheetField.ItemColor;
			}
			textBox.TextAlign=sheetField.TextAlign;
			FontStyle style=FontStyle.Regular;
			if(sheetField.FontIsBold) {
				style=FontStyle.Bold;
			}
			float fontSize=LayoutManager.Scale(sheetField.FontSize);
			//The textbox reduces spacing between each letter
			//fontSize=fontSize*1.04f;//This is an attempt to make the textbox show exactly the same as the drawing.
			//TextRenderer was added to C# so that we could draw to match the textbox perfectly if we wanted.
			//But we need the other way around.  We don't want to be forced to use TextRenderer everywhere just so a temp textbox matches.
			//The final solution will be to build our own UI.TextBox, which isn't too hard, but can't be immediate.
			textBox.Font=new Font(sheetField.FontName,fontSize,style);
			if(LayoutManager.Scale(sheetField.Height)<=textBox.Font.Height+5 && sheetField.GrowthBehavior==GrowthBehaviorEnum.None) {
				textBox.Multiline=false;
			}
			else{
				textBox.Multiline=true;
			}
			textBox.Text=text;
			textBox.Height=LayoutManager.Scale(sheetField.Height);
			textBox.ScrollBars=ScrollBars.None;//but we do want scrollbars if needed
			textBox.Tag=sheetField;
			textBox.LostFocus+=TextBox_LostFocus;
			textBox.PreviewKeyDown+=TextBox_PreviewKeyDown;
			textBox.TextChanged+=textBox_TextChanged;
			textBox.FontChanged+=TextBox_FontChanged;
			textBox.ReadOnly=sheetField.IsLocked;
			LayoutManager.Add(textBox,panelMain);
			//int scroll=panelScroll.VerticalScroll.Value;
			//panelScroll.VerticalScroll.Value=scroll;
			textBox.Select();
			if(point==Point.Empty//just tabbing over from a previous field, so select end
				|| _idxSelectedChar==-1)
			{
				textBox.SelectionStart=textBox.Text.Length;//end
			}
			else{
				textBox.SelectionStart=_idxSelectedChar;
			}
		}

		private void TextBox_FontChanged(object sender,EventArgs e) {
			
		}

		///<summary>Takes the "To" address and subject and correctly formats an email to the lab or patient. Returns the file path to the pdf that is created.</summary>
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

		///<summary>Old</summary>
		private void FieldValueChanged(object sender) {
			//this old method was broken into ClearSigs() and a big chunk in timerTextChanged_Tick()
		}

		private void ClearSigs(){
			for(int i=0;i<_listSignatureBoxWrappers.Count;i++){
				//When field values change, the signature should not show as "invalid" but instead should just be cleared so that the user can re-sign.
				//sigBox.SetInvalid();
				_listSignatureBoxWrappers[i].ClearSignature(clearTopazTablet:false);//The user is purposefully "invalidating" the old signature by changing the contents of the sheet. 
			}
		}

		///<summary>Pass in a point in sheet coords. This doesn't hit test for everything, but just for items that we might want to click on. Frequently returns null.</summary>
		private SheetField HitTest(Point point){
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				//this is a list of types that we support because user can interact with them
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.CheckBox,SheetFieldType.InputField,SheetFieldType.StaticText,SheetFieldType.OutputText,SheetFieldType.PatImage,SheetFieldType.ComboBox)){
					continue;
				}
				if(point.X<SheetCur.SheetFields[i].XPos
					|| point.X>SheetCur.SheetFields[i].XPos+SheetCur.SheetFields[i].Width
					|| point.Y<SheetCur.SheetFields[i].YPos
					|| point.Y>SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height)
				{
					continue;
				}
				return SheetCur.SheetFields[i];
			}
			return null;
		}

		///<summary>This calculates the index within the string where the user clicked. The point is global and scaled, coords used in calcs are all at unscaled 96 dpi.</summary>
		private int HitTestChars(SheetField sheetField,Point point,string text){
			Point pointUnscaled=new Point(LayoutManager.Unscale(point.X),LayoutManager.Unscale(point.Y));
			_listRectangleFsChars=new List<RectangleF>();
			using Graphics g=this.CreateGraphics();
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			FontStyle fontStyle=FontStyle.Regular;
			if(sheetField.FontIsBold){
				fontStyle=FontStyle.Bold;
			}
			Font font=new Font(sheetField.FontName,sheetField.FontSize,fontStyle);
			//We're only allowed to measure 32 chars at a time
			int cStart=0;//the starting idx of the group of 32 or less chars that we're about to measure
			StringFormat stringFormat = new StringFormat();
			if(sheetField.TextAlign==HorizontalAlignment.Right){
				stringFormat.Alignment=StringAlignment.Far;
			}
			if(sheetField.TextAlign==HorizontalAlignment.Center){
				stringFormat.Alignment=StringAlignment.Center;
			}
			while(true){
				if(cStart>text.Length-1){//example 5>4
					break;
				}
				int length=32;
				if(cStart+length>=text.Length){
					length=text.Length-cStart;
				}
				CharacterRange[] characterRangeArray=new CharacterRange[length];//each CharacterRange is just one char
				for(int i=0;i<characterRangeArray.Length;i++){
					characterRangeArray[i]=new CharacterRange(cStart+i,1);
				}
				stringFormat.SetMeasurableCharacterRanges(characterRangeArray);
				RectangleF rectangleF=new RectangleF(0,0,sheetField.Width,sheetField.Height);
				Region[] regionArray=g.MeasureCharacterRanges(text,font,rectangleF,stringFormat);
				for(int r=0;r<regionArray.Length;r++){
					RectangleF rectangleFBoundsChar=regionArray[r].GetBounds(g);
					//I use global so I can draw them for testing.
					RectangleF rectangleFGlobal=new RectangleF();
					if(rectangleFBoundsChar.Width!=0){
						rectangleFGlobal.X=sheetField.XPos+rectangleFBoundsChar.X;
						rectangleFGlobal.Y=sheetField.YPos+rectangleFBoundsChar.Y;
						rectangleFGlobal.Width=rectangleFBoundsChar.Width;
						rectangleFGlobal.Height=rectangleFBoundsChar.Height;
					}
					_listRectangleFsChars.Add(rectangleFGlobal);
				}//end of r loop
				cStart+=length;
			}//end of while cStart loop
			//now I have a list of rectangles, one for each char.
			int idxSelectedChar=-1;
			for(int i=0;i<_listRectangleFsChars.Count;i++){
				if(!_listRectangleFsChars[i].Contains(pointUnscaled)){
					continue;
				}
				if(pointUnscaled.X<_listRectangleFsChars[i].Left+_listRectangleFsChars[i].Width/2f){//left of center
					idxSelectedChar=i;
				}
				else{
					idxSelectedChar=i+1;
				}
			}
			if(idxSelectedChar!=-1){
				return idxSelectedChar;
			}
			//to the right or left of text, but it could be on any row
			for(int i=0;i<_listRectangleFsChars.Count;i++){
				if(pointUnscaled.Y<_listRectangleFsChars[i].Top || pointUnscaled.Y>_listRectangleFsChars[i].Bottom){
					continue;//wrong row
				}
				if(_listRectangleFsChars[i].Width==0 || _listRectangleFsChars[i].Height==0){
					continue;//carriage returns or newline
				}
				if(pointUnscaled.X<_listRectangleFsChars[i].Left){//clicked to the left of text (if right or center aligned)
					//example 5 chars: 0,1,2,3,4. 3 is currently selected. We're testing 0.
					if(idxSelectedChar==-1 || i<idxSelectedChar){//example 0<3
						idxSelectedChar=i;//example =0
					}
				}
				else{//clicked to the right of text
					//example 5 chars: 0,1,2,3,4. (to the left of) 3 is currently selected. We're testing 4.
					if(i+1>idxSelectedChar){//example 4+1>3
						idxSelectedChar=i+1;//example =4+1. putting it to the right of 4.
					}
					//so if we click to the right of the last visible char 16, then we have selected 17,
					//which is perfect because it keeps us on this line instead of the beginning of the next line (char 19)
				}
			}
			return idxSelectedChar;
		}

		///<summary>Runs as the final step of loading the form.  Returns an error message if a SheetField was found in an invalid state and a repair was possible.</summary>
		private string LayoutFields() {
			//This method is also called LayoutFields in older versions, but this one does a lot less.
			//For example, it does not run after fields are moved down due to growth.  That's handled by RepositionControls().
			//Also, there's not much to lay out because most things are now just drawn from scratch.
			int width=LayoutManager.Scale(SheetCur.Width);
			if(SheetCur.IsLandscape){
				width=LayoutManager.Scale(SheetCur.Height);
			}
			int bottomLastField=0;
			if(SheetCur.SheetFields.Count>0) {
				bottomLastField=SheetCur.SheetFields.Max(x=>x.Bounds.Bottom);
			}
			int height=LayoutManager.Scale(Math.Max(SheetCur.HeightPage,bottomLastField));//+20 for Hscrollbar?
			LayoutManager.MoveSize(panelMain,new Size(width,height));
			string strErr="";
			_listSignatureBoxWrappers=new List<SignatureBoxWrapper>();
			List<SheetField> listSheetFieldsSorted=new List<SheetField>();
			listSheetFieldsSorted.AddRange(SheetCur.SheetFields);//Creates a sortable list that will not change sort order of the original list.
			listSheetFieldsSorted.Sort(SheetFields.SortDrawingOrderLayers);
			//Dispose panelMain's controls before clearing them, or else they won't get disposed.
			//Do the disposal in reverse order so anything being removed from the list won't break the loop
			for(int i=panelMain.Controls.Count-1;i>=0;i--) {
				panelMain.Controls[i]?.Dispose();
			}
			panelMain.Controls.Clear();
			//Text----------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.InputField,SheetFieldType.OutputText,SheetFieldType.StaticText)){
					continue;
				}
				//Just validation
				if(SheetCur.SheetFields[i].FontSize<=0) {//Sheet did not save correctly.
					SheetCur.SheetFields[i].FontSize=SheetCur.FontSize;//Use a best guess so the user does not completely lose this Sheet's data.
					strErr=$"A text SheetCur.SheetFields[i] was found with an invalid FontSize.  Default size of {SheetCur.FontSize} has been used instead.";
				}
				//Drawn in Paint
			}
			//draw screencharts--------------------------------------------------------------------------------------------------
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i].FieldType!=SheetFieldType.ScreenChart) {
					continue;
				}
				ScreenToothChart screenToothChart=new ScreenToothChart(SheetCur.SheetFields[i].FieldValue,SheetCur.SheetFields[i].FieldValue[0]=='1');//Need to pass in value here to set tooth chart items.
				screenToothChart.Location=new Point(LayoutManager.Scale(SheetCur.SheetFields[i].XPos),LayoutManager.Scale(SheetCur.SheetFields[i].YPos));
				screenToothChart.Width=LayoutManager.Scale(SheetCur.SheetFields[i].Width);
				screenToothChart.Height=LayoutManager.Scale(SheetCur.SheetFields[i].Height);
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
					MsgBox.Show(this,"Treatment Plan Signatures are not supported in FormSheetFillEdit.");
					break;
				}
				OpenDental.UI.SignatureBoxWrapper signatureBoxWrapper=new OpenDental.UI.SignatureBoxWrapper();
				if(SheetCur.SheetFields[i].IsSigProvRestricted 
					&& (Security.CurUser==null || Security.CurUser.UserNum<1 || Security.CurUser.ProvNum<1)) 
				{
					signatureBoxWrapper.Enabled=false;
				}
				if(Security.CurUser!=null
					&& Security.CurUser.UserNum>0 //Is currently a logged in user
					&& SheetCur.SheetFields[i].CanElectronicallySign //If the SheetCur.SheetFields[i] allows for electronic signature
					&& !IsInTerminal) //Electronic signatures are not allowed in kiosk mode
				{
					signatureBoxWrapper.SetAllowDigitalSig(true,true);
				}
				signatureBoxWrapper.Location=new Point(LayoutManager.Scale(SheetCur.SheetFields[i].XPos),LayoutManager.Scale(SheetCur.SheetFields[i].YPos));
				signatureBoxWrapper.Width=LayoutManager.Scale(SheetCur.SheetFields[i].Width);
				signatureBoxWrapper.Height=LayoutManager.Scale(SheetCur.SheetFields[i].Height);
				signatureBoxWrapper.SetScaleAndZoom(LayoutManager.GetScaleMS(),LayoutManager.GetZoomLocal());//This is not redundant
				if(SheetCur.SheetFields[i].FieldValue.Length>0) {//a signature is present
					bool sigIsTopaz=(SheetCur.SheetFields[i].FieldValue[0]=='1');
					string signature="";
					if(SheetCur.SheetFields[i].FieldValue.Length>1) {
						signature=SheetCur.SheetFields[i].FieldValue.Substring(1);
					}
					string keyData=Sheets.GetSignatureKey(SheetCur);
					signatureBoxWrapper.FillSignature(sigIsTopaz,keyData,signature);
				}
				if(SheetCur.SheetType==SheetTypeEnum.PaymentPlan) {
					PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(SheetCur.Parameters,"payplan").ParamValue;
					if(payPlan.Signature!="") {//a PP sig is present
					string keyData=(string)SheetParameter.GetParamByName(SheetCur.Parameters,"keyData").ParamValue;
						signatureBoxWrapper.FillSignature(payPlan.SigIsTopaz,keyData,payPlan.Signature);
					}
				}
				signatureBoxWrapper.Tag=SheetCur.SheetFields[i];
				signatureBoxWrapper.TabStop=(SheetCur.SheetFields[i].TabOrder>0);
				signatureBoxWrapper.TabIndex=SheetCur.SheetFields[i].TabOrder;
				LayoutManager.Add(signatureBoxWrapper,panelMain);
				signatureBoxWrapper.BringToFront();
				if(signatureBoxWrapper.IsValid && SheetCur.SheetFields[i].FieldValue.Length>0) {
					//According to this form's load function the only pre-requisite for being a "signed" sheet and locking it is that it loads with an existing signature.
					//Based on that if we get this far and there's actually a signature then it's "signed", but this only works with the first form load.
					RichTextBox richTextBox=new RichTextBox();
					richTextBox.BorderStyle=BorderStyle.None;
					richTextBox.TabStop=false;
					richTextBox.Location=new Point(LayoutManager.Scale(SheetCur.SheetFields[i].XPos)+1,LayoutManager.Scale(SheetCur.SheetFields[i].YPos+SheetCur.SheetFields[i].Height-15));
					richTextBox.Width=SheetCur.SheetFields[i].Width-2;
					richTextBox.ScrollBars=RichTextBoxScrollBars.None;
					richTextBox.SelectionAlignment=HorizontalAlignment.Left;
					string signed=signatureBoxWrapper.GetIsTypedFromWebForms() ? "Typed signature in WebForms" : "Signed";
					richTextBox.Text=Lan.g(this,signed)+": "+SheetCur.SheetFields[i].DateTimeSig.ToShortDateString()+" "+SheetCur.SheetFields[i].DateTimeSig.ToShortTimeString();
					richTextBox.Multiline=false;
					richTextBox.Height=LayoutManager.Scale(14);
					richTextBox.ReadOnly=true;
					richTextBox.Font=new Font("Arial",LayoutManager.ScaleF(8.25f));
					LayoutManager.Add(richTextBox,panelMain);
					richTextBox.BringToFront();
				}
				_listSignatureBoxWrappers.Add(signatureBoxWrapper);
			}
			return strErr;
		}

		///<summary>Loads one image from disk or cloud into sheetFieldDef.ImageField. We call this again if image changes for some reason.  This fails if sheetField.BitmapLoaded already has an image, so you have to clear that bitmap if you want to force a refresh.</summary>
		private void LoadImageOne(SheetField sheetField){
			//This code is from DrawImage() in older versions.
			if(sheetField.BitmapLoaded!=null){
				return;
			}
			if(sheetField.FieldName=="Patient Info.gif") {
				sheetField.BitmapLoaded=OpenDentBusiness.Properties.Resources.Patient_Info;
				return;
			}
			string filePathAndName=ODFileUtils.CombinePaths(SheetUtil.GetImagePath(),sheetField.FieldName);
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && File.Exists(filePathAndName)) {
				Bitmap bitmap=(Bitmap)Image.FromFile(filePathAndName);
				sheetField.BitmapLoaded=new Bitmap(bitmap,sheetField.Width,sheetField.Height);
				bitmap?.Dispose();
				return;
			}
			if(!CloudStorage.IsCloudStorage) {
				return;
			}
			//Cloud storage from here down
			using FormProgress formProgress=new FormProgress();
			formProgress.DisplayText=Lan.g(CloudStorage.LanThis,"Downloading " + sheetField.FieldName + "...");
			formProgress.NumberFormat="F";
			formProgress.NumberMultiplication=1;
			formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
			formProgress.TickMS=1000;
			OpenDentalCloud.Core.TaskStateDownload taskStateDownload=CloudStorage.DownloadAsync(SheetUtil.GetImagePath(),sheetField.FieldName,
				new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
			if(formProgress.ShowDialog()==DialogResult.Cancel) {
				taskStateDownload.DoCancel=true;
				return;
			}
			if(taskStateDownload==null || taskStateDownload.FileContent==null) {
				//File wasn't downloaded, do nothing
				return;
			}
			using MemoryStream memoryStream=new MemoryStream(taskStateDownload.FileContent);
			using Image image = Image.FromStream(memoryStream);
			sheetField.BitmapLoaded=new Bitmap(image,sheetField.Width,sheetField.Height);
		}

		///<summary>Loads one PatImage from disk or cloud into sheetFieldDef.ImageField. We call this again if image changes for some reason.  This fails if sheetField.BitmapLoaded already has an image, so you have to clear that bitmap if you want to force a refresh.</summary>
		private void LoadImageOnePat(SheetField sheetField){
			//This code is from DrawImage() in older versions.
			if(sheetField.BitmapLoaded!=null){
				return;
			}
			if(sheetField.FieldValue==""){
				return;
			}
			if(sheetField.FieldValue.StartsWith("MountNum:")){
				long mountNum=PIn.Long(sheetField.FieldValue.Substring(9));
				sheetField.BitmapLoaded=MountHelper.GetBitmapOfMountFromDb(mountNum);
				return;
			}
			long docNum=PIn.Long(sheetField.FieldValue);
			sheetField.BitmapLoaded=ImageHelper.GetBitmapOfDocumentFromDb(docNum);
		}
		
		///<summary>Just during load.</summary>
		private void LoadImages() {
			for(int i=0;i<SheetCur.SheetFields.Count();i++) {
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.Image)){
					continue;
				}
				LoadImageOne(SheetCur.SheetFields[i]);
			}
			for(int i=0;i<SheetCur.SheetFields.Count();i++) {
				if(!SheetCur.SheetFields[i].FieldType.In(SheetFieldType.PatImage)){
					continue;
				}
				LoadImageOnePat(SheetCur.SheetFields[i]);
			}
		}

		///<summary>This is called after automatic growth to move a few controls to their new locations.  But most of the fields are just quickly drawn and they have no controls.</summary>
		private void RepositionControls(){
			for(int i=0;i<panelMain.Controls.Count;i++){
				SheetField sheetField=(SheetField)panelMain.Controls[i].Tag;
				Point point=new Point(LayoutManager.Scale(sheetField.XPos),LayoutManager.Scale(sheetField.YPos));
				LayoutManager.MoveLocation(panelMain.Controls[i],point);
			}
		}

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

		///<summary>Shows the saved text on the save button for 3 seconds after clicking the save exam button.</summary>
		private void ShowSaved() {
			butSave.Text=Lans.g(this,"Saved!");
			_timerSaveButtonText=new Timer();
			_timerSaveButtonText.Interval=3000;
			_timerSaveButtonText.Tick += new EventHandler(timerChangeSaveButtonText);
			_timerSaveButtonText.Enabled=true;
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

		///<summary>Returns true if SheetCur is new and is a referral letter and has a toothchart or grid sheetfields.</summary>
		private bool IsNewReferralLetterWithProcsOrChart() {
			return SheetCur.IsNew
				&& SheetCur.SheetType==SheetTypeEnum.ReferralLetter
				&& SheetCur.SheetFields.Exists(x => x.FieldName=="ReferralLetterProceduresCompleted" || x.FieldName=="toothChart");
		}

		///<summary>If an error won't allow, then it shows a MsgBox and then returns false.</summary>
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
			FillFieldsFromControls(isSave:true);//this does nothing
			FillFieldsFromScreenings();
			bool isNewReferralLetterWithProcsOrChart=IsNewReferralLetterWithProcsOrChart();
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
			if(!isNewReferralLetterWithProcsOrChart) {
				//Don't need to do this for referral letters that have tooth charts or grids because 
				//we don't save their sheet fields. Instead, they are saved and accessed as PDFs. 
				//Sync fields before sigBoxes
				SheetFields.Sync(SheetCur.SheetFields.FindAll(x => !x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)),SheetCur.SheetNum,isSigBoxOnly:false);
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
				if(!isNewReferralLetterWithProcsOrChart) {
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
			if(isNewReferralLetterWithProcsOrChart) {
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
				//now sync SigBoxes
				SheetFields.Sync(listSheetFieldsSigBoxes,SheetCur.SheetNum,isSigBoxOnly:true);
				//SheetFields.GetFieldsAndParameters(SheetCur);
				//Each (SheetField)control (had in old versions) a tag pointing at a SheetCur.SheetField, and GetFieldsAndParameters() causes us to overwrite SheetCur.SheetFields.
				//This leaves the tag pointing at nothing, so we need to call LayoutFields() to re-link the controls and data.
				//LayoutFields();
				if(SheetCur.ShowInTerminal>0) {
					Signalods.SetInvalid(InvalidType.Kiosk);
				}
				return true;
			}
			//now sync SigBoxes
			SheetFields.Sync(listSheetFieldsSigBoxes,SheetCur.SheetNum,isSigBoxOnly:true);
			SheetFields.GetFieldsAndParameters(SheetCur);
			//Each (SheetField)control has a tag pointing at a SheetCur.SheetField, and GetFieldsAndParameters() causes us to overwrite SheetCur.SheetFields.
			//This leaves the tag pointing at nothing, so we need to call LayoutFields() to re-link the controls and data.
			LayoutFields();
			if(SheetCur.ShowInTerminal>0) {
				Signalods.SetInvalid(InvalidType.Kiosk);
			}
			return true;
		}

		///<summary>Obsolete. Doesn't do anything. Was also done before bumping down fields due to growth behavior.</summary>
		private void FillFieldsFromControls(bool isSave=false){			
			//This is no longer used.
			//Instead, the fields are always kept up-to-date as the user clicks around.
			//There is no longer a list of controls because nearly everything is just drawn,
			//and textBoxes only come up temporarily for one field at a time.
			/*
			//SheetField field;
			//Images------------------------------------------------------
				//Images can't be cha nged in this UI
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
					//RichTextBox can alter the string being passed in. This comparison will ensure we are only updating the sheet value when the user has changed something.
					GraphicsHelper.CreateTextBoxForSheetDisplay((SheetField)panelMain.Controls[i].Tag,false,richTextBoxFormatted);
					//Purposefully trimming leading and trailing whitespace from all input fields.
					//This method gets invoked prior to hashing signature data when saving so this programmatic manipulation is safe to do.
					if(isSave
						&& ((SheetField)panelMain.Controls[i].Tag).FieldType==SheetFieldType.InputField
						&& !((SheetField)panelMain.Controls[i].Tag).FieldValue.IsNullOrEmpty())
					{
						richTextBoxFormatted.Text=richTextBoxFormatted.Text.Trim();
					}
					if(panelMain.Controls[i].Text!=richTextBoxFormatted.Text) {
						((SheetField)panelMain.Controls[i].Tag).FieldValue=panelMain.Controls[i].Text.Trim();
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
			//ScreenChart------------------------------------------------
			//... moved down to FillFieldsFromScreenings()
			//Rectangles and lines-----------------------------------------
				//Rectangles and lines can't be changed in this UI
			//Drawings----------------------------------------------------
				//Drawings data is already saved to fields
			//SigBoxes---------------------------------------------------
				//SigBoxes won't be strictly checked for validity
				//or data saved to the field until it's time to actually save to the database.
			*/
		}

		private void FillFieldsFromScreenings(){
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
		}

		///<summary>Fills the sheet fields with their combo boxes.</summary>
		private void FillFieldsFromComboBoxes() {
			/*
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].GetType()!=typeof(SheetComboBox)) {
					continue;
				}
				if(panelMain.Controls[i].Tag==null) {
					continue;
				}
				SheetComboBox comboBox=(SheetComboBox)panelMain.Controls[i];
				((SheetField)panelMain.Controls[i].Tag).FieldValue=comboBox.ToFieldValue();
			}*/
		}

		///<summary>Returns true when all of the sheet fields with IsRequired set to true have a value set. Otherwise, a message box shows and false is returned.</summary>
		private bool VerifyRequiredFields(){
			//SigBoxes are checked separately due to specific properties not in SheetField
			for(int i=0;i<_listSignatureBoxWrappers.Count;i++) {
				SheetField sheetField=(SheetField)_listSignatureBoxWrappers[i].Tag;
				if(!sheetField.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice)){
					continue;
				}
				OpenDental.UI.SignatureBoxWrapper sigBox= _listSignatureBoxWrappers[i];
				if(sheetField.IsRequired && (!sigBox.IsValid || sigBox.SigIsBlank)){
					MsgBox.Show(this,"Signature required");
					return false;
				}
			}
			for(int i=0;i<SheetCur.SheetFields.Count;i++){
				if(SheetCur.SheetFields[i]==null){
					continue;
				}
				if(!SheetCur.SheetFields[i].IsRequired) {
					continue;
				}
				//SigBoxes are different than InputFields, we do not check them here
				if(SheetCur.SheetFields[i].FieldType==SheetFieldType.InputField){
					SheetField sheetField=SheetCur.SheetFields[i];
					if(string.IsNullOrWhiteSpace(sheetField.FieldValue)){
						if(sheetField.FieldName=="misc" && !string.IsNullOrWhiteSpace(sheetField.ReportableName)) {
							MessageBox.Show(Lan.g(this,"You must enter a value for")+" "+sheetField.ReportableName+" "+Lan.g(this,"before continuing."));
						}
						else {
							MessageBox.Show(Lan.g(this,"You must enter a value for")+" "+sheetField.FieldName+" "+Lan.g(this,"before continuing."));
						}
						return false;
					}	
				}
				else if(SheetCur.SheetFields[i].FieldType==SheetFieldType.CheckBox){//Radio button groups or misc checkboxes
					SheetField sheetField=SheetCur.SheetFields[i];
					if(sheetField.FieldValue!="X"){//required but this one not checked
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
							for(int j=0;j<SheetCur.SheetFields.Count;j++){
								if(SheetCur.SheetFields[j].FieldType!=SheetFieldType.CheckBox){
									continue;//skip everything that's not a checkbox
								}
								SheetField sheetField2=SheetCur.SheetFields[j];
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
			//Check for an explicit image category to potentially override the autosave category.
			SheetDef sheetDef=SheetDefs.GetSheetDef(SheetCur.SheetDefNum, hasExceptions:false);
			if(charUsage=='U' && sheetDef!=null && sheetDef.AutoCheckSaveImageDocCategory!=0) {
				listDefsImageCat.Clear();
				listDefsImageCat.Add(Defs.GetDef(DefCat.ImageCats,sheetDef.AutoCheckSaveImageDocCategory));
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

		///<summary>Deletes the current sheet and handles closing the form. Prompts the user to confirm if the sheet is currently being used by a kiosk.</summary>
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

		private void ValidateSaveAndExit() {
			if(!ValidateStateField()) {
				return;
			}
			FixFontsForPdf(SheetCur);// validate fonts before saving
			if(!VerifyRequiredFields() || !OkToSaveBecauseNoOtherEdits() || !TryToSaveData()){
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
		private bool OkToSaveBecauseNoOtherEdits() {
			//The DB sheet may not have truly had any changes for current sheet, but someone opened the sheet and clicked OK.
			if(SheetCur.IsNew){
				return true;
			}
			if(SheetCur.SheetNum==0){
				return true;
			}
			Sheet sheet=Sheets.GetOne(SheetCur.SheetNum);
			if(sheet==null){
				return true;
			}
			if(sheet.DateTSheetEdited<=SheetCur.DateTSheetEdited){
				return true;
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"There have been changes to this sheet since it was loaded.  If you continue those changes will be overwritten.  Continue anyway?")) {
				return true;
			}
			return false;
		}

		///<summary>validates the fonts for use with PDF sharp. If not compatible, sets the font to something that will work.</summary>
		//This is a workaround due to the fact that PDF Sharp does not support TTC fonts.
		private void FixFontsForPdf(Sheet sheetCur,bool isPrinting=false) {
			if(!isPrinting){
				return;
			}
			bool hasErrors=false;
			List<string> listStrBadFonts=new List<string>();
			try {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
				XFont _=new XFont(sheetCur.FontName,sheetCur.FontSize,XFontStyle.Regular);
			}
			catch {
				hasErrors=true;
				listStrBadFonts.Add(sheetCur.FontName);
				sheetCur.FontName=FontFamily.GenericSansSerif.ToString();//font was not compatible with PDFSharp, fill with one we hope is. Same font replacement we use in SheetDrawingJob.cs
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
						SheetCur.SheetFields[i].FontName=FontFamily.GenericSansSerif.ToString();//font was not compatible with PDFSharp, fill with one we hope is. Same font replacement we use in SheetDrawingJob.cs
					}
				}
			}
			if(hasErrors) {
				string str=Lan.g(this,"Form is trying to save or print with unsupported font(s):");
				str+=" "+string.Join(", ",listStrBadFonts.ToArray())+". ";
				str+=Lan.g(this,"Font(s) will be replaced with a generic substitute to allow saving and printing.");
				MsgBox.Show(str);
			}
		}

		//Currently only for PatientForms, verify that the InputField of "State" is exactly 2 characters in length. 
		private bool ValidateStateField() {
			if(SheetCur.SheetType!=SheetTypeEnum.PatientForm) {
				return true;
			}
			for(int i=0;i<panelMain.Controls.Count;i++){
				if(panelMain.Controls[i].Tag==null){
					continue;
				}
				if(panelMain.Controls[i].GetType()==typeof(RichTextBox)) {
					SheetField sheetField=(SheetField)panelMain.Controls[i].Tag;
					if(sheetField.FieldType!=SheetFieldType.InputField){
						continue;
					}
					if(sheetField.FieldName=="State" && sheetField.FieldValue.Trim().Length!=2 && sheetField.FieldValue.Trim().Length>0) {
						MessageBox.Show(Lan.g(this,"The State field must be exactly two characters in length."));
						return false;
					}
				}
			}
			return true;
		}


		#endregion Methods - private 	
	}
	#region Classes
	///<summary>This prevents it from scrolling automatically when controls are added and take focus.</summary>
	public class PanelScrollManual : Panel{
		protected override Point ScrollToControl(Control controlActive){
			return DisplayRectangle.Location;
		}
	}
	#endregion Classes
}