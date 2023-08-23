using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes;

namespace OpenDental{
	///<summary></summary>
	public partial class FormStatementOptions : FormODBase {
		private bool _wasInitiallySent;
		private Patient _patientSuperHead;
		private List<Def> _listDefsImageCat;
		private bool _isFromBilling=false;
		///<summary>Tracks the state of this setting when it differs from the default.</summary>
		public bool ShowBillTransSinceZero;
		///<summary>This is true if on load the single statement IsNew.</summary>
		private bool _isStatementNew;
		public Statement StatementCur;
		///<summary>This will be null for ordinary edits.  But sometimes this window is used to edit bulk statements.  In that case, this list contains the statements being edited.  Must contain at least one item.</summary>
		public List<Statement> ListStatements;

		///<summary></summary>
		public FormStatementOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		public FormStatementOptions(bool isFromBilling=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(isFromBilling) {
				checkSinglePatient.Enabled=false;
			}
			_isFromBilling=isFromBilling;
		}

		private void FormStatementOptions_Load(object sender, System.EventArgs e) {
			if(ListStatements==null){
				if(StatementCur.StatementType==StmtType.LimitedStatement) {//Must be called before SetEnabled below
					checkExcludeTxfr.Visible=true;
				}
				if(StatementCur.IsSent){
					checkIsSent.Checked=true;
					_wasInitiallySent=true;
					SetEnabled(false);
				}
				textDate.Text=StatementCur.DateSent.ToShortDateString();
				//Allow the temporary value of this setting to override the default if there's a difference
				//ShowBillTransSinceZero will have been updated only if _isFromBilling is true (ie the user arrived here from the Billing forms)
				if(_isFromBilling){
					checkBoxBillShowTransSinceZero.Checked=ShowBillTransSinceZero;
				} else {
					checkBoxBillShowTransSinceZero.Checked=PrefC.GetBool(PrefName.BillingShowTransSinceBalZero);
				}
				listMode.Items.Clear();
				listMode.Items.AddEnums<StatementMode>();
				listMode.SetSelectedEnum(StatementCur.Mode_);
				if(StatementCur.Mode_==StatementMode.Electronic) {
					//Automatically select intermingling family and remove that as a selection option.
					checkSinglePatient.Checked=false;
					checkSinglePatient.Enabled=false;
					checkIntermingled.Checked=true;
					checkIntermingled.Enabled=false;
				}
				checkHidePayment.Checked=StatementCur.HidePayment;
				checkSinglePatient.Checked=StatementCur.SinglePatient;
				checkIntermingled.Checked=StatementCur.Intermingled;
				checkIsReceipt.Checked=StatementCur.IsReceipt;
				if(PrefC.IsODHQ){
					checkShowLName.Checked=true;
				}
				if(StatementCur.IsInvoice) {//If they got here with drop down menu invoice item.
					if(CultureInfo.CurrentCulture.Name=="en-US") {
						checkIsInvoiceCopy.Visible=false;
					}
					checkIsInvoice.Checked=true;
					checkIsInvoiceCopy.Checked=StatementCur.IsInvoiceCopy;
					textInvoiceNum.Text=StatementCur.StatementNum.ToString();
					groupDateRange.Visible=false;
					checkIsReceipt.Visible=false;
					checkIntermingled.Visible=false;
					checkBoxBillShowTransSinceZero.Visible=false;
				}
				else {
					groupInvoice.Visible=false;
				}
				if(StatementCur.StatementType==StmtType.LimitedStatement) {
					checkLimited.Checked=true;
					checkLimited.Visible=true;//if limited statement, checkLimited will be visible, but checked and disabled since the user can't change it
					checkSinglePatient.Enabled=false;
					checkIsReceipt.Visible=false;
					groupDateRange.Visible=false;
					//intermingled + singlePatient doesn't make sense and since you can't change the single patient checkbox status with a limited statement,
					//disable the intermingled checkbox as well
					if(checkSinglePatient.Checked) {
						checkIntermingled.Enabled=false;
					}
				}
				if(StatementCur.DateRangeFrom.Year>1880){
					textDateStart.Text=StatementCur.DateRangeFrom.ToShortDateString();
				}
				if(StatementCur.DateRangeTo.Year<2100){
					textDateEnd.Text=StatementCur.DateRangeTo.ToShortDateString();
				}
				textNote.Text=StatementCur.Note;
				textNoteBold.Text=StatementCur.NoteBold;
				if(StatementCur.StatementType!=StmtType.LimitedStatement && PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
					Patient patientGuarantor=Patients.GetFamily(StatementCur.PatNum).ListPats[0];
					_patientSuperHead=Patients.GetPat(patientGuarantor.SuperFamily);
					if(StatementCur.IsNew && !StatementCur.IsSent && patientGuarantor.HasSuperBilling && patientGuarantor.SuperFamily>0 && _patientSuperHead!=null && _patientSuperHead.HasSuperBilling) {
						//Statement not sent, statements use sheets, and guarantor is a member of a superfamily, guarantor and superhead both have superbilling enabled.
						//Enable superfam checkbox.  Only if this is a new statement that hasn't been set as a super statement.  If it's already been marked as a super statement, don't allow user to uncheck the box
						checkSuperStatement.Enabled=true;
					}
					checkSuperStatement.Checked=(StatementCur.SuperFamily!=0 && _patientSuperHead!=null && _patientSuperHead.PatNum==StatementCur.SuperFamily);//check box if superhead statement
				}
				else {//either a limited statement or super family show feature is disabled or statements are not using sheets
					checkSuperStatement.Visible=false;
				}
				if(!_isFromBilling) {
					checkSendSms.Text=Lans.g(this,"Sent text");//The user cannot send a text from this window, so we want to make it clear that they can't.
					checkSendSms.Enabled=false;
					checkSendSms.Checked=StatementCur.SmsSendStatus==AutoCommStatus.SendSuccessful;
				}
				else if(!StatementCur.SmsSendStatus.In(AutoCommStatus.DoNotSend,AutoCommStatus.Undefined)) {
					checkSendSms.Checked=true;
				}
				_isStatementNew=StatementCur.IsNew;
				if(IsLimitedCustomStatement()) {
					LimitedCustomStatementLayoutHelper();
				}
			}
			#region Bulk Edit
			else{
				//DateSent-------------------------------------------------------------------------------------
				textDate.Text="?";
				bool allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].DateSent!=ListStatements[i].DateSent){//if any are different from the first element
						allSame=false;
					}
				}
				if(allSame){
					textDate.Text=ListStatements[0].DateSent.ToShortDateString();
				}
				//IsSent----------------------------------------------------------------------------------------
				checkIsSent.ThreeState=true;
				checkIsSent.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].IsSent!=ListStatements[i].IsSent){
						allSame=false;
					}
				}
				if(allSame){
					checkIsSent.ThreeState=false;
					checkIsSent.CheckState=CheckState.Unchecked;
					checkIsSent.Checked=ListStatements[0].IsSent;
				}
				//Mode------------------------------------------------------------------------------------------
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].Mode_!=ListStatements[i].Mode_){
						allSame=false;
					}
				}
				listMode.Items.Clear();
				listMode.Items.AddEnums<StatementMode>();
				if(allSame) {
					listMode.SetSelectedEnum(ListStatements[0].Mode_);
				}
				if(ListStatements[0].Mode_==StatementMode.Electronic) {
					//Automatically select intermingling family and remove that as a selection option.
					checkSinglePatient.Checked=false;
					checkSinglePatient.Enabled=false;
					checkIntermingled.Checked=true;
					checkIntermingled.Enabled=false;
				}
				//HidePayment------------------------------------------------------------------------------------
				checkHidePayment.ThreeState=true;
				checkHidePayment.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].HidePayment!=ListStatements[i].HidePayment){
						allSame=false;
					}
				}
				if(allSame){
					checkHidePayment.ThreeState=false;
					checkHidePayment.CheckState=CheckState.Unchecked;
					checkHidePayment.Checked=ListStatements[0].HidePayment;
				}
				//SinglePatient------------------------------------------------------------------------------------
				checkSinglePatient.ThreeState=true;
				checkSinglePatient.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].SinglePatient!=ListStatements[i].SinglePatient){
						allSame=false;
					}
				}
				if(allSame){
					checkSinglePatient.ThreeState=false;
					checkSinglePatient.CheckState=CheckState.Unchecked;
					checkSinglePatient.Checked=ListStatements[0].SinglePatient;
				}
				//Intermingled----------------------------------------------------------------------------------------
				checkIntermingled.ThreeState=true;
				checkIntermingled.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].Intermingled!=ListStatements[i].Intermingled){
						allSame=false;
					}
				}
				if(allSame){
					checkIntermingled.ThreeState=false;
					checkIntermingled.CheckState=CheckState.Unchecked;
					checkIntermingled.Checked=ListStatements[0].Intermingled;
				}
				//DateStart-------------------------------------------------------------------------------------
				textDateStart.Text="?";
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].DateRangeFrom!=ListStatements[i].DateRangeFrom){
						allSame=false;
					}
				}
				if(allSame){
					if(ListStatements[0].DateRangeFrom.Year<1880){
						textDateStart.Text="";
					}
					else{
						textDateStart.Text=ListStatements[0].DateRangeFrom.ToShortDateString();
					}
				}
				//DateEnd-------------------------------------------------------------------------------------
				textDateEnd.Text="?";
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].DateRangeTo!=ListStatements[i].DateRangeTo){
						allSame=false;
					}
				}
				if(allSame){
					if(ListStatements[0].DateRangeTo.Year<1880){
						textDateEnd.Text="";
					}
					else{
						textDateEnd.Text=ListStatements[0].DateRangeTo.ToShortDateString();
					}
				}
				//Note----------------------------------------------------------------------------------------
				textNote.Text="?";
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].Note!=ListStatements[i].Note){
						allSame=false;
					}
				}
				if(allSame){
					textNote.Text=ListStatements[0].Note;
				}
				//NoteBold----------------------------------------------------------------------------------------
				textNoteBold.Text="?";
				allSame=true;
				for(int i=0;i<ListStatements.Count;i++){
					if(ListStatements[0].NoteBold!=ListStatements[i].NoteBold){
						allSame=false;
					}
				}
				if(allSame){
					textNoteBold.Text=ListStatements[0].NoteBold;
				}
				butEmail.Enabled=false;
				butPrint.Enabled=false;
				butPreview.Enabled=false;
				butPatPortal.Enabled=false;
				//Send Text Message-------------------------------------------------------------------------------
				checkSendSms.ThreeState=true;
				checkSendSms.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=1;i<ListStatements.Count;i++) {
					if(ListStatements[0].SmsSendStatus!=ListStatements[i].SmsSendStatus) {
						allSame=false;
						break;
					}
				}
				if(allSame) {
					checkSendSms.ThreeState=false;
					checkSendSms.CheckState=CheckState.Unchecked;
					checkSendSms.Checked=(!ListStatements[0].SmsSendStatus.In(AutoCommStatus.DoNotSend,AutoCommStatus.Undefined));
				}
			}
			#endregion Bulk Edit
			_listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			Plugins.HookAddCode(this,"FormStatementOptions_Load_end");
			if(Security.IsAuthorized(Permissions.StatementCSV,true)) {
				checkExportCSV.Visible=true;
			}
		}

		private void butToday_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text=DateTime.Today.ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void but45days_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text=DateTime.Today.AddDays(-45).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void but90days_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text=DateTime.Today.AddDays(-90).ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void butDatesAll_Click(object sender,EventArgs e) {
			SetAccountHistoryControl();
			textDateStart.Text="";
			textDateEnd.Text=DateTime.Today.ToShortDateString();
		}

		private void SetAccountHistoryControl() {
			checkBoxBillShowTransSinceZero.Checked=false;
		}

		private void checkBoxBillShowTransSinceZero_CheckedChanged(object sender,EventArgs e) {
			if(checkBoxBillShowTransSinceZero.Checked) {
				textDateStart.Enabled=false;
				textDateEnd.Enabled=false;
			}
			else {
				textDateStart.Enabled=true;
				textDateEnd.Enabled=true;
			}
			ShowBillTransSinceZero=checkBoxBillShowTransSinceZero.Checked;
		}

		private void checkIsSent_Click(object sender,EventArgs e) {
			if(_wasInitiallySent && !checkIsSent.Checked){//user unchecks the Sent box in order to edit
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning.  This will immediately delete the archived copy of the statement.  Continue anyway?")){
					checkIsSent.Checked=true;
					return;
				}
				SetEnabled(true);
				if(StatementCur.Mode_==StatementMode.Electronic) {
					checkSinglePatient.Checked=false;
					checkSinglePatient.Enabled=false;
					checkIntermingled.Checked=true;
					checkIntermingled.Enabled=false;
				}
				if(StatementCur.IsInvoice) {
					checkIsInvoiceCopy.Checked=false;
				}
				if(StatementCur.StatementType==StmtType.LimitedStatement) {
					checkExcludeTxfr.Visible=true;
				}
				//Delete the archived copy of the statement
				if(StatementCur.DocNum!=0){
					Patient patient=Patients.GetPat(StatementCur.PatNum);
					string filePathPatImg=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
					List<Document> listDocuments=new List<Document>();
					listDocuments.Add(Documents.GetByNum(StatementCur.DocNum,true));
					try {
						ImageStore.DeleteDocuments(listDocuments,filePathPatImg);
						Statements.DetachDocFromStatements(StatementCur.DocNum);
						StatementCur.DocNum=0;
					}
					catch(Exception ex) {  //Image could not be deleted, in use.
						MessageBox.Show(this,ex.Message);
						return;
					}
				}
			}
			else if(ListStatements==null && StatementCur.IsInvoice && checkIsSent.Checked) {
				checkIsInvoiceCopy.Checked=true;
			}
		}

		private void checkSuperStatement_CheckedChanged(object sender,EventArgs e) {
			if(checkSuperStatement.Checked) {
				checkIntermingled.Checked=false;
				checkIntermingled.Enabled=false;
				checkSinglePatient.Checked=false;
				checkSinglePatient.Enabled=false;
			}
			else {
				checkIntermingled.Enabled=true;
				checkSinglePatient.Enabled=true;
			}
		}

		private void SetEnabled(bool boolval){
			textDate.Enabled=boolval;
			listMode.Enabled=boolval;
			checkHidePayment.Enabled=boolval;
			checkSinglePatient.Enabled=boolval;
			checkIntermingled.Enabled=boolval;
			checkIsReceipt.Enabled=boolval;
			groupDateRange.Enabled=boolval;
			textNote.ReadOnly=!boolval;
			textNoteBold.ReadOnly=!boolval;
			groupInvoice.Enabled=boolval;
			checkSuperStatement.Enabled=boolval;
			//These checkboxes don't store their state in the DB. Therefore, they are only helpful for unsent statements and would be misleading if left visible and disabled.
			//E.g. Editing a limited statement that was generated with last names showing would display checkShowLName as unchecked and disabled (which is misleading).
			checkShowLName.Visible=boolval;
			checkExcludeTxfr.Visible=boolval;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			//check if file is available to print if it was already created.  Does not affect first time printing.
			if(StatementCur.DocNum!=0 && checkIsSent.Checked) {
				Patient patient=Patients.GetPat(StatementCur.PatNum);
				string filePathPatImg=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				if(!FileAtoZ.Exists(ImageStore.GetFilePath(Documents.GetByNum(StatementCur.DocNum),filePathPatImg))) { 
					MsgBox.Show(this,"File not found: " + Documents.GetByNum(StatementCur.DocNum).FileName);
					return;
				}
			}
			butPrintSheets();
			if(checkExportCSV.Checked) {
				Statements.SaveStatementAsCSV(StatementCur);
			}
		}

		private void butPrintSheets() {
			_isStatementNew=false;//At this point, the statment will no longer be new. 
			Patient patient = Patients.GetPat(StatementCur.PatNum);
			if(StatementCur.DocNum!=0 && checkIsSent.Checked 
				&& !StatementCur.IsInvoice)//Invoices are always recreated on the fly in order to show "Copy" when needed.
			{
				//launch existing archive pdf. User can click print from within Acrobat.
				LaunchArchivedPdf(patient);
				DialogResult=DialogResult.OK;
				return;
			}
			//was not initially sent, or else user has unchecked the sent box
			if(_wasInitiallySent && checkIsSent.Checked && StatementCur.DocNum==0 
				&& !StatementCur.IsInvoice)//for invoice, we don't notify user that it's a recreation
			{
				MsgBox.Show(this,"There was no archived image of this statement.  The printout will be based on current data.");
			}
			//So create an archive
			if(listMode.GetSelected<StatementMode>()==StatementMode.Email) {
				listMode.SetSelectedEnum(StatementMode.InPerson);
			}
			checkIsSent.Checked=true;
			Cursor=Cursors.WaitCursor;
			Patient patientGuarantor = null;
			if(patient!=null) {
				patientGuarantor = Patients.GetPat(patient.Guarantor);
			}
			if(checkSuperStatement.Checked && patientGuarantor!=null && patientGuarantor.SuperFamily!=0) {
				List<Patient> listPatientsFamilyGuarantors=Patients.GetSuperFamilyGuarantors(patientGuarantor.SuperFamily).FindAll(x => x.HasSuperBilling);
				//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => x.BalTotal>0);
				}
				else {
					listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>=0);
					StatementCur.InsEst=listPatientsFamilyGuarantors.Sum(x => x.InsEst);
				}
				StatementCur.BalTotal=listPatientsFamilyGuarantors.Sum(x => x.BalTotal);
				StatementCur.IsBalValid=true;
			}
			else if(patientGuarantor!=null) {
				StatementCur.BalTotal=patientGuarantor.BalTotal;
				StatementCur.InsEst=patientGuarantor.InsEst;
				StatementCur.IsBalValid=true;
			}
			if(!SaveToDb()) {
				Cursor=Cursors.Default;
				return;
			}
			if(!SaveAsDocument(true)) {
				return;
			}
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
		}

		///<summary>Creates a PDF if necessary and attaches the statement document to the statement.</summary>
		///<param name="pdfFileName">If this is blank, a PDF will be created.</param>
		///<param name="sheet">This sheet will be used to create the PDF. If it is null, the default Statement sheet will be used instead.</param>
		private bool SaveAsDocument(bool doPrintSheet=false,string pdfFileName="",Sheet sheet=null,DataSet dataSet=null) {
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StatementCur);
			string tempPath;
			if(dataSet==null) {
				if(checkSuperStatement.Checked || IsLimitedCustomStatement()) {
					dataSet=AccountModules.GetSuperFamAccount(StatementCur,doIncludePatLName: checkShowLName.Checked,doShowHiddenPaySplits: StatementCur.IsReceipt,doExcludeTxfrs: checkExcludeTxfr.Checked);
				}
				else {
					dataSet=AccountModules.GetAccount(StatementCur.PatNum,StatementCur,doIncludePatLName: checkShowLName.Checked,doShowHiddenPaySplits: StatementCur.IsReceipt,doExcludeTxfrs: checkExcludeTxfr.Checked);
				}
			}
			if(pdfFileName=="") {
				if(sheet==null) {
					sheet=SheetUtil.CreateSheet(sheetDef,StatementCur.PatNum,StatementCur.HidePayment);
					sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StatementCur });
					SheetFiller.FillFields(sheet,dataSet,StatementCur);
					SheetUtil.CalculateHeights(sheet,dataSet,StatementCur);
				}
				tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),StatementCur.PatNum.ToString()+".pdf");
				SheetPrinting.CreatePdf(sheet,tempPath,StatementCur,dataSet:dataSet);
			}
			else {
				tempPath=pdfFileName;
			}
			long category=0;
			for(int i=0;i<_listDefsImageCat.Count;i++) {
				if(Regex.IsMatch(_listDefsImageCat[i].ItemValue,@"S")) {
					category=_listDefsImageCat[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=_listDefsImageCat[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document document=null;
			try {
				document=ImageStore.Import(tempPath,category,Patients.GetPat(StatementCur.PatNum));
			}
			catch {
				MsgBox.Show(this,"Error saving document.");
				//this.Cursor=Cursors.Default;
				return false;
			}
			finally {
				//Delete the temp file since we don't need it anymore.
				try {
					if(pdfFileName=="") {//If they're passing in a PDF file name, they probably have it open somewhere else.
						File.Delete(tempPath);
					}
				}
				catch {
					//Do nothing.  This file will likely get cleaned up later.
				}
			}
			document.ImgType=ImageType.Document;
			if(StatementCur.IsInvoice) {
				document.Description=Lan.g(this,"Invoice");
			}
			else {
				if(StatementCur.IsReceipt==true) {
					document.Description=Lan.g(this,"Receipt");
				}
				else {
					document.Description=Lan.g(this,"Statement");
				}
			}
			//Some customers have wanted to sort their statements in the images module by date and time.  
			//We would need to enhance DateSent to include the time portion.
			StatementCur.DateSent=document.DateCreated;
			StatementCur.DocNum=document.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(StatementCur.StatementNum,document);
			Statements.SyncStatementProdsForStatement(dataSet,StatementCur.StatementNum,StatementCur.DocNum);
			if(doPrintSheet) {
				//Actually print the statement.
				//NOTE: This is printing a "fresh" GDI+ version of the statment which is ever so slightly different than the PDFSharp statment that was saved to disk.
				sheet=SheetUtil.CreateSheet(sheetDef,StatementCur.PatNum,StatementCur.HidePayment);
				sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StatementCur });
				SheetFiller.FillFields(sheet,dataSet,StatementCur);
				SheetUtil.CalculateHeights(sheet,dataSet,StatementCur);
				SheetPrinting.Print(sheet,dataSet,1,false,StatementCur);//use GDI+ printing, which is slightly different than the pdf.
				if(StatementCur.IsInvoice && checkIsInvoiceCopy.Visible) {//for foreign countries
					StatementCur.IsInvoiceCopy=true;
					Statements.Update(StatementCur);
				}
			}
			return true;
		}

		private void butEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			_isStatementNew=false;//At this point, the statment will no longer be new. 
			if(StatementCur.DocNum!=0 && checkIsSent.Checked) {
				//remail existing archive pdf?
				//or maybe tell user they can't do that?
				MsgBox.Show(this,"Statement has already been sent.");
				return;
			}
			//was not initially sent, or else user has unchecked the sent box
			//So create an archive
			if(listMode.GetSelected<StatementMode>()!=StatementMode.Email) {
				listMode.SetSelectedEnum(StatementMode.Email);
			}
			if(!CreatePdfForSheet()){
				Cursor=Cursors.Default;
				checkIsSent.Checked=false;
				return;
			}
			if(!CreateEmailMessage()) {
				Cursor=Cursors.Default;
				checkIsSent.Checked=false;
				return;
			}
			if(StatementCur.IsInvoice && checkIsInvoiceCopy.Visible) {//for foreign countries
				StatementCur.IsInvoiceCopy=true;
			}
			//Email was sent. Update the statement.
			StatementCur.IsSent=checkIsSent.Checked;
			Statements.Update(StatementCur);
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
		}

		private bool CreatePdfForSheet() {
			_isStatementNew=false;//At this point, the statment will no longer be new. 
			Cursor=Cursors.WaitCursor;
			Patient patient=Patients.GetPat(StatementCur.PatNum);
			Patient patientGuarantor=null;
			if(patient!=null) {
				patientGuarantor=Patients.GetPat(patient.Guarantor);
			}
			if((checkSuperStatement.Checked || IsLimitedCustomStatement()) && patientGuarantor!=null && patientGuarantor.SuperFamily!=0) {
				List<Patient> listPatientsFamilyGuarantors=Patients.GetSuperFamilyGuarantors(patientGuarantor.SuperFamily).FindAll(x => x.HasSuperBilling);
				//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => x.BalTotal>0);
				}
				else {
					listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>=0);
					StatementCur.InsEst=listPatientsFamilyGuarantors.Sum(x => x.InsEst);
				}
				StatementCur.BalTotal=listPatientsFamilyGuarantors.Sum(x => x.BalTotal);
				StatementCur.IsBalValid=true;
			}
			else if(patientGuarantor!=null) {
				StatementCur.BalTotal=patientGuarantor.BalTotal;
				StatementCur.InsEst=patientGuarantor.InsEst;
				StatementCur.IsBalValid=true;
			}
			if(!SaveToDb()) {
				return false;
			}
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StatementCur);
			DataSet dataSet=null;
			if(checkSuperStatement.Checked || IsLimitedCustomStatement()) {
				//handled in SaveToDb()
				//StmtCur.SuperFamily=Patients.GetPat(StmtCur.PatNum).SuperFamily;
				//StmtCur.PatNum=StmtCur.SuperFamily;
				dataSet=AccountModules.GetSuperFamAccount(StatementCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StatementCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
			}
			else {
				dataSet=AccountModules.GetAccount(StatementCur.PatNum,StatementCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StatementCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,StatementCur.PatNum,StatementCur.HidePayment);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StatementCur });
			SheetFiller.FillFields(sheet,dataSet,StatementCur);
			SheetUtil.CalculateHeights(sheet,dataSet,StatementCur);
			string tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),StatementCur.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,tempPath,StatementCur,dataSet:dataSet);
			long category=0;
			for(int i=0;i<_listDefsImageCat.Count;i++) {
				if(Regex.IsMatch(_listDefsImageCat[i].ItemValue,@"S")) {
					category=_listDefsImageCat[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=_listDefsImageCat[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document document=null;
			try {
				document=ImageStore.Import(tempPath,category,Patients.GetPat(StatementCur.PatNum));
			}
			catch {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Error saving document.");
				return false;
			}
			document.ImgType=ImageType.Document;
			if(StatementCur.IsInvoice) {
				document.Description=Lan.g(this,"Invoice");
			}
			else {
				if(StatementCur.IsReceipt==true) {
					document.Description=Lan.g(this,"Receipt");
				}
				else {
					document.Description=Lan.g(this,"Statement");
				}
			}
			StatementCur.DateSent=document.DateCreated;
			StatementCur.DocNum=document.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(StatementCur.StatementNum,document);
			Statements.SyncStatementProdsForStatement(dataSet,StatementCur.StatementNum,StatementCur.DocNum);
			checkIsSent.Checked=true;
			Cursor=Cursors.Default;
			return true;
		}

		/// <summary>Also displays the dialog for the email.  Must have already created and attached the pdf.  Returns false if it could not create the email.</summary>
		private bool CreateEmailMessage(){
			string attachPath=EmailAttaches.GetAttachPath();
			Random rnd=new Random();
			string fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".pdf";
			string filePathAndName=ODFileUtils.CombinePaths(attachPath,fileName);
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase){
				MsgBox.Show(this,"Could not create email because no AtoZ folder.");
				return false;
			}
			Patient patient=Patients.GetPat(StatementCur.PatNum);
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string oldPath=ODFileUtils.CombinePaths(ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath()),Documents.GetByNum(StatementCur.DocNum).FileName);
				File.Copy(oldPath,filePathAndName);
			}
			else {//Cloud
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading patient statement...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath())
					,Documents.GetByNum(StatementCur.DocNum).FileName
					,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				if(formProgress.ShowDialog()==DialogResult.Cancel) {
					state.DoCancel=true;
					return false;
				}
				else {
					//Do stuff with state.FileContent
					using FormProgress formProgress2=new FormProgress();
					formProgress2.DisplayText="Uploading patient email...";
					formProgress2.NumberFormat="F";
					formProgress2.NumberMultiplication=1;
					formProgress2.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					formProgress2.TickMS=1000;
					OpenDentalCloud.Core.TaskStateUpload state2=CloudStorage.UploadAsync(attachPath
						,fileName
						,state.FileContent
						,new OpenDentalCloud.ProgressHandler(formProgress2.UpdateProgress));
					if(formProgress2.ShowDialog()==DialogResult.Cancel) {
						state2.DoCancel=true;
						return false;
					}
					else {
						//Upload was successful
					}
				}
			}
			//Process.Start(filePathAndName);
			EmailMessage emailMessage=Statements.GetEmailMessageForStatement(StatementCur,patient);
			EmailAttach emailAttach=new EmailAttach();
			emailAttach.DisplayedFileName="Statement.pdf";
			emailAttach.ActualFileName=fileName;
			emailMessage.Attachments.Add(emailAttach);
			if(checkExportCSV.Checked) {
				rnd=new Random();
				string csvFileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".csv";
				string csvPathAndName=ODFileUtils.CombinePaths(attachPath,csvFileName);
				string csvFilePath=Statements.SaveStatementAsCSV(StatementCur);
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase){
					MsgBox.Show(this,"Could not create email because no AtoZ folder.");
					return false;
				}
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					File.Copy(csvFilePath,csvPathAndName);
				}
				EmailAttach emailAttachCSV=new EmailAttach();
				emailAttachCSV.DisplayedFileName="Statement.csv";
				emailAttachCSV.ActualFileName=csvFileName;
				emailMessage.Attachments.Add(emailAttachCSV);
			}
			using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,EmailAddresses.GetByClinic(patient.ClinicNum));
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.ShowDialog();
			if(formEmailMessageEdit.DialogResult==DialogResult.OK){
				return true;
			}
			return false;
		}
		
		//Returns true if statementCur.LimitedCustomFamily is not None or statementCur is null.
		private bool IsLimitedCustomStatement() {
			if(StatementCur==null) {
				return false;
			}
			return StatementCur.LimitedCustomFamily!=EnumLimitedCustomFamily.None;
		}

		private void butPreview_Click(object sender,EventArgs e) {
			butPreviewSheets();
		}

		private void butPreviewSheets() {
			Patient patient = Patients.GetPat(StatementCur.PatNum);
			if(StatementCur.DocNum!=0 && checkIsSent.Checked) {//initiallySent && checkIsSent.Checked){
				string billingType=PrefC.GetString(PrefName.BillingUseElectronic);
				if(StatementCur.Mode_==StatementMode.Electronic && (billingType=="1" || billingType=="3") && !PrefC.GetBool(PrefName.BillingElectCreatePDF)) {
					MsgBox.Show(this,"PDF's are not saved for electronic billing.  Unable to view.");
					return;
				}
				else {
					LaunchArchivedPdf(patient);
				}
				return;
			}
			//was not initially sent, or else user has unchecked the sent box
			Cursor=Cursors.WaitCursor;
			Patient patientGuarantor = null;
			if(patient!=null) {
				patientGuarantor = Patients.GetPat(patient.Guarantor);
			}
			if((checkSuperStatement.Checked || IsLimitedCustomStatement()) && patientGuarantor!=null && patientGuarantor.SuperFamily!=0) {
				List<Patient> listPatientsFamilyGuarantors=Patients.GetSuperFamilyGuarantors(patientGuarantor.SuperFamily).FindAll(x => x.HasSuperBilling);
				//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => x.BalTotal>0);
				}
				else {
					listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>=0);
					StatementCur.InsEst=listPatientsFamilyGuarantors.Sum(x => x.InsEst);
				}
				StatementCur.BalTotal=listPatientsFamilyGuarantors.Sum(x => x.BalTotal);
				StatementCur.IsBalValid=true;
			}
			else if(patientGuarantor!=null) {
				StatementCur.BalTotal=patientGuarantor.BalTotal;
				StatementCur.InsEst=patientGuarantor.InsEst;
				StatementCur.IsBalValid=true;
			}
			if(!SaveToDb()) {
				Cursor=Cursors.Default;
				return;
			}
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StatementCur);
			DataSet dataSet=null;
			if(checkSuperStatement.Checked || IsLimitedCustomStatement()) {
				//handled in SaveToDb()
				//StmtCur.SuperFamily=Patients.GetPat(StmtCur.PatNum).SuperFamily;
				//StmtCur.PatNum=StmtCur.SuperFamily;
				dataSet=AccountModules.GetSuperFamAccount(StatementCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StatementCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
			}
			else {
				dataSet=AccountModules.GetAccount(StatementCur.PatNum,StatementCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StatementCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,StatementCur.PatNum,StatementCur.HidePayment);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StatementCur });
			SheetFiller.FillFields(sheet,dataSet,StatementCur);
			SheetUtil.CalculateHeights(sheet,dataSet,StatementCur,true);
			Cursor=Cursors.Default;
			//print directly to PDF here, and save it.
			using FormSheetFillEdit formSheetFillEdit=new FormSheetFillEdit(sheet,dataSet,checkExportCSV.Checked);
			formSheetFillEdit.StatementCur=StatementCur;
			formSheetFillEdit.IsStatement=true;
			formSheetFillEdit.SaveStatementToDocDelegate=SaveStatementAsDocument;
			formSheetFillEdit.ShowDialog();
			if(formSheetFillEdit.HasEmailBeenSent) {
				formSheetFillEdit.StatementCur.Mode_=StatementMode.Email;
				listMode.SetSelectedEnum(StatementMode.Email);
			}
		}

		private void SaveStatementAsDocument(Statement statement,Sheet sheet,DataSet dataSet,string pdfFileName) {
			checkIsSent.Checked=SaveAsDocument(pdfFileName:pdfFileName,sheet:sheet,dataSet:dataSet);
			if(checkIsSent.Checked) {
				//if the statement was viewed and it was printed/emailed, then it is no longer new.  
				_isStatementNew=false;
			}
		}

		///<summary>Opens the saved PDF for the document.</summary>
		private void LaunchArchivedPdf(Patient patient) {
			string filePathPatFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
			Document document=Documents.GetByNum(StatementCur.DocNum);
			string fileName=ImageStore.GetFilePath(document,filePathPatFolder);
			if(!FileAtoZ.Exists(fileName)) {
				MessageBox.Show(Lan.g(this,"File not found:")+" "+document.FileName);
				return;
			}
			try {
				FileAtoZ.StartProcess(fileName);
			}
			catch(Exception ex) {
				FriendlyException.Show($"Unable to open the following file: {document.FileName}",ex);
			}
		}

		private void LimitedCustomStatementLayoutHelper() {
			this.DisableAllExcept(butDelete,butPreview,butCancel,butOK,checkIsSent,checkIntermingled,checkExportCSV,checkShowLName,checkExcludeTxfr, checkHidePayment,butPrint,butEmail,butPatPortal,textNote,textNoteBold,listMode,label1,label2,label3,label4,textDate);
			checkSuperStatement.Checked=false;
			checkSinglePatient.Checked=false;
			if(StatementCur.LimitedCustomFamily==EnumLimitedCustomFamily.Patient) {
				checkSinglePatient.Checked=true;
			}
		}

		private void butPatPortal_Click(object sender,EventArgs e) {
			if(!Defs.GetDefsForCategory(DefCat.ImageCats,true).Any(x => x.ItemValue.Contains(ImageCategorySpecial.L.ToString())
				&& x.ItemValue.Contains(ImageCategorySpecial.S.ToString()))) {
				MsgBox.Show(this,"There is no image category used for both Patient Portal and Statements in Setup | Definitions | Image Categories. "
					+"The Statements image category must have both 'Show in Patient Portal' and 'Statements' usage types selected.");
				return;
			}
			if(UserWebs.GetByFKeyAndType(StatementCur.PatNum,UserWebFKeyType.PatientPortal,true)==null) {
				MsgBox.Show(this,"This patient does not have Online Access to the Patient Portal.");
				return;
			}
			//After checking the preference, CreatePdfForSheet() is called, which will try to create a pdf of the sheet
			if(!CreatePdfForSheet()) {
				MsgBox.Show(this,"There was an error creating a PDF for this patient");
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Send an email to the patient notifying them that a statement is available?")) {
				Patient patient=Patients.GetPat(StatementCur.PatNum);
				EmailMessage emailMessage=Statements.GetEmailMessageForPortalStatement(StatementCur,patient);
				using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,EmailAddresses.GetByClinic(patient.ClinicNum));
				formEmailMessageEdit.IsNew=true;
				formEmailMessageEdit.ShowDialog();
				if(formEmailMessageEdit.DialogResult != DialogResult.OK) {
					return;
				}
			}
			StatementCur.IsSent=checkIsSent.Checked;
			Statements.Update(StatementCur);
			DialogResult=DialogResult.OK;
		}

		private void textDate_KeyPress(object sender,KeyPressEventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="fr-CA" || CultureInfo.CurrentCulture.Name=="en-CA") {
				return;//because they use - in their regular dates which interferes with this feature.
			}
			if(e.KeyChar!='+' && e.KeyChar!='-') {
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(textDate.Text);
			}
			catch {
				return;
			}
			int caret=textDate.SelectionStart;
			if(e.KeyChar=='+') {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyChar=='-') {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			textDate.Text=dateDisplayed.ToShortDateString();
			textDate.SelectionStart=caret;
			e.Handled=true;
		}

		private void textDate_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down) {
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(textDate.Text);
			}
			catch {
				return;
			}
			int caret=textDate.SelectionStart;
			if(e.KeyCode==Keys.Up) {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyCode==Keys.Down) {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			textDate.Text=dateDisplayed.ToShortDateString();
			textDate.SelectionStart=caret;
			e.Handled=true;
		}

		private void textDateStart_KeyPress(object sender,KeyPressEventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="fr-CA" || CultureInfo.CurrentCulture.Name=="en-CA") {
				return;//because they use - in their regular dates which interferes with this feature.
			}
			if(e.KeyChar!='+' && e.KeyChar!='-') {
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(textDateStart.Text);
			}
			catch {
				return;
			}
			int caret=textDateStart.SelectionStart;
			if(e.KeyChar=='+') {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyChar=='-') {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			textDateStart.Text=dateDisplayed.ToShortDateString();
			textDateStart.SelectionStart=caret;
			e.Handled=true;
		}

		private void textDateStart_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down) {
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(textDateStart.Text);
			}
			catch {
				return;
			}
			int caret=textDateStart.SelectionStart;
			if(e.KeyCode==Keys.Up) {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyCode==Keys.Down) {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			textDateStart.Text=dateDisplayed.ToShortDateString();
			textDateStart.SelectionStart=caret;
			e.Handled=true;
		}

		private void textDateEnd_KeyPress(object sender,KeyPressEventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="fr-CA" || CultureInfo.CurrentCulture.Name=="en-CA") {
				return;//because they use - in their regular dates which interferes with this feature.
			}
			if(e.KeyChar!='+' && e.KeyChar!='-') {
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(textDateEnd.Text);
			}
			catch {
				return;
			}
			int caret=textDateEnd.SelectionStart;
			if(e.KeyChar=='+') {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyChar=='-') {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			textDateEnd.Text=dateDisplayed.ToShortDateString();
			textDateEnd.SelectionStart=caret;
			e.Handled=true;
		}

		private void textDateEnd_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode!=Keys.Up && e.KeyCode!=Keys.Down) {
				return;
			}
			DateTime dateDisplayed;
			try {
				dateDisplayed=DateTime.Parse(textDateEnd.Text);
			}
			catch {
				return;
			}
			int caret=textDateEnd.SelectionStart;
			if(e.KeyCode==Keys.Up) {
				dateDisplayed=dateDisplayed.AddDays(1);
			}
			if(e.KeyCode==Keys.Down) {
				dateDisplayed=dateDisplayed.AddDays(-1);
			}
			textDateEnd.Text=dateDisplayed.ToShortDateString();
			textDateEnd.SelectionStart=caret;
			e.Handled=true;
		}

		private void textDate_Validating(object sender,CancelEventArgs e) {
			if(textDate.Text=="") {
				return;
			}
			if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en") {
				if(textDate.Text.All(x => char.IsNumber(x))) {
					if(textDate.Text.Length==6) {
						textDate.Text=textDate.Text.Substring(0,2)+"/"+textDate.Text.Substring(2,2)+"/"+textDate.Text.Substring(4,2);
					}
					else if(textDate.Text.Length==8) {
						textDate.Text=textDate.Text.Substring(0,2)+"/"+textDate.Text.Substring(2,2)+"/"+textDate.Text.Substring(4,4);
					}
				}
			}
			DateTime date=DateTime.MinValue;
			try{
				date=DateTime.Parse(textDate.Text);
			}
			catch{ 
				return;	
			}
			if(date.Year>1880) {
				textDate.Text=date.ToString("d");
			}
		}

		private void textDateStart_Validating(object sender,CancelEventArgs e) {
			if(textDateStart.Text=="") {
				return;
			}
			if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en") {
				if(textDateStart.Text.All(x => char.IsNumber(x))) {
					if(textDateStart.Text.Length==6) {
						textDateStart.Text=textDateStart.Text.Substring(0,2)+"/"+textDateStart.Text.Substring(2,2)+"/"+textDateStart.Text.Substring(4,2);
					}
					else if(textDateStart.Text.Length==8) {
						textDateStart.Text=textDateStart.Text.Substring(0,2)+"/"+textDateStart.Text.Substring(2,2)+"/"+textDateStart.Text.Substring(4,4);
					}
				}
			}
			DateTime date=DateTime.MinValue;
			try{
				date=DateTime.Parse(textDate.Text);
			}
			catch{ 
				return;	
			}
			if(date.Year>1880) {
				textDate.Text=date.ToString("d");
			}
		}

		private void textDateEnd_Validating(object sender,CancelEventArgs e) {
			if(textDateEnd.Text=="") {
				return;
			}
			if(CultureInfo.CurrentCulture.TwoLetterISOLanguageName=="en") {
				if(textDateEnd.Text.All(x => char.IsNumber(x))) {
					if(textDateEnd.Text.Length==6) {
						textDateEnd.Text=textDateEnd.Text.Substring(0,2)+"/"+textDateEnd.Text.Substring(2,2)+"/"+textDateEnd.Text.Substring(4,2);
					}
					else if(textDateEnd.Text.Length==8) {
						textDateEnd.Text=textDateEnd.Text.Substring(0,2)+"/"+textDateEnd.Text.Substring(2,2)+"/"+textDateEnd.Text.Substring(4,4);
					}
				}
			}
			DateTime date=DateTime.MinValue;
			try{
				date=DateTime.Parse(textDate.Text);
			}
			catch{ 
				return;	
			}
			if(date.Year>1880) {
				textDate.Text=date.ToString("d");
			}
		}

		private void listMode_Click(object sender,EventArgs e) {
			if(listMode.GetSelected<StatementMode>()==StatementMode.Electronic) {
				//Automatically select intermingling family and remove that as a selection option.
				checkSinglePatient.Checked=false;
				checkSinglePatient.Enabled=false;
				checkIntermingled.Checked=true;
				checkIntermingled.Enabled=false;
			}
			else {
				checkSinglePatient.Enabled=true;
				checkIntermingled.Enabled=true;
			}
			if(_isFromBilling) {//Disable single patient if we started in the billing window
				checkSinglePatient.Checked=false;
				checkSinglePatient.Enabled=false;
			}
			//Disable controls for Limited Custom Statement
			if(IsLimitedCustomStatement()) {
				LimitedCustomStatementLayoutHelper();
			}
		}

		private void checkSinglePatient_Click(object sender,EventArgs e) {
			if(checkSinglePatient.Checked) {
				checkSinglePatient.Checked=true;
				checkIntermingled.Checked=false;
			}
			else {
				if(StatementCur.IsInvoice) {
					checkSinglePatient.Checked=true;
				}
			}
		}

		private void checkIntermingled_Click(object sender,EventArgs e) {
			if(checkIntermingled.Checked) {
				checkSinglePatient.Checked=false;
				checkIntermingled.Checked=true;
			}
		}

		private void checkIsInvoice_Click(object sender,EventArgs e) {
			if(StatementCur.IsInvoice) {
				checkIsInvoice.Checked=true;//don't let them uncheck it.
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ListStatements==null && StatementCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try { 
				if(ListStatements==null){
					Statements.DeleteStatements(new List<Statement> { StatementCur });
				}
				else{//bulk edit
					Statements.DeleteStatements(ListStatements);
				}
			}
			catch(Exception ex){
				FriendlyException.Show(Lan.g(this,"Error deleting statements."),ex);
				return;
			}
			try {
				//If a patient is on a mobile device, then the statement also needs to be removed from there
				List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetAll();
				List<Statement> listStatements=ListStatements??new List<Statement>() {StatementCur};
				for(int i=0; i<listStatements.Count; i++) {
					MobileAppDevice mobileAppDevice=listMobileAppDevices.FirstOrDefault(x => x.PatNum==listStatements[i].PatNum);
					if(mobileAppDevice!=null && mobileAppDevice.LastCheckInActivity>DateTime.Now.AddHours(-1)) {
						PushNotificationUtils.CI_RefreshPayment(mobileAppDevice.MobileAppDeviceNum,listStatements[i].PatNum, out string errorMsg);
					}
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error retrieving patient folder."),ex);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//Do not set IsBalValid, BalTotal, or InsEst here. This would edit old statments.
			if(!SaveToDb()){
				return;
			}
			//If saving a statement that doesn't yet have an image/doc, create one so we can view this in patient portal
			if(ListStatements==null && StatementCur.DocNum==0 && !_isFromBilling) {
				SaveAsDocument(false);//needs to be called after the statement is inserted for the payment plan grid (if present)
			}
			if(checkExportCSV.Checked) {
				Statements.SaveStatementAsCSV(StatementCur); 
			}
			DialogResult=DialogResult.OK;
		}

		private bool SaveToDb(){
			bool isError;
			//Validate Date-------------------------------------------------------------------------------
			isError=false;
			if(textDate.Text==""){//not allowed to be blank.  Other two dates are allowed to be blank.
				if(ListStatements==null){//if editing a List, blank indicates dates vary.
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Please enter a Date.");
					return false;
				}
			}
			else{//"?" not allowed here
				try{
					DateTime.Parse(textDate.Text);
				}
				catch{
					isError=true;
				}
			}
			if(isError){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please fix Date.");
				return false;
			}
			//Validate DateStart-------------------------------------------------------------------------------
			isError=false;
			if(textDateStart.Text==""){
				//no error
			}
			else if(textDateStart.Text=="?"){
				if(ListStatements==null){
					isError=true;
				}
			}
			else{
				try{
					DateTime.Parse(textDateStart.Text);
				}
				catch{
					isError=true;
				}
			}
			if(isError){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please fix Start Date.");
				return false;
			}
			//Validate DateEnd-------------------------------------------------------------------------------
			isError=false;
			if(textDateEnd.Text==""){
				//no error
			}
			else if(textDateEnd.Text=="?"){
				if(ListStatements==null){
					isError=true;
				}
			}
			else{
				try{
					DateTime.Parse(textDateEnd.Text);
				}
				catch{
					isError=true;
				}
			}
			if(isError){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please fix End Date.");
				return false;
			}
			//if(  textDateStart.Text .errorProvider1.GetError(textDateStart)!=""
			//	|| textDateEnd.errorProvider1.GetError(textDateEnd)!=""
			//	|| textDate.errorProvider1.GetError(textDate)!="")
			//{
			//	MsgBox.Show(this,"Please fix data entry errors first.");
			//	return false;
			//}
			if(ListStatements==null){
				if(checkSuperStatement.Checked) {
					StatementCur.PatNum=_patientSuperHead.PatNum;
					StatementCur.SuperFamily=_patientSuperHead.PatNum;
				}
				StatementCur.DateSent=PIn.Date(textDate.Text);
				StatementCur.IsSent=checkIsSent.Checked;
				StatementCur.Mode_=listMode.GetSelected<StatementMode>();
				StatementCur.HidePayment=checkHidePayment.Checked;
				StatementCur.SinglePatient=checkSinglePatient.Checked;
				StatementCur.Intermingled=checkIntermingled.Checked;
				StatementCur.IsReceipt=checkIsReceipt.Checked;
				StatementCur.IsInvoice=checkIsInvoice.Checked;
				StatementCur.StatementType=checkLimited.Checked?StmtType.LimitedStatement:StmtType.NotSet;//right now only either LimitedStatement or NotSet
				StatementCur.DateRangeFrom=PIn.Date(textDateStart.Text);//handles blank
				if(checkBoxBillShowTransSinceZero.Checked) {
					Patient patient=Patients.GetPat(StatementCur.PatNum);
					List<PatAging> listPatAgings=Patients.GetAgingListSimple(new List<long> {}, new List<long> { patient.Guarantor },true);
					DataTable tableBals=Ledgers.GetDateBalanceBegan(listPatAgings,checkSuperStatement.Checked);
					if(tableBals.Rows.Count > 0) {
						DateTime dateFrom=PIn.Date(tableBals.Rows[0]["DateZeroBal"].ToString());
						if(dateFrom==DateTime.MinValue) {//patient has a zero or credit balance.
							StatementCur.DateRangeFrom=DateTime.Now;
						}
						else {
							StatementCur.DateRangeFrom=dateFrom;
						}
					}
				}
				if(textDateEnd.Text==""){
					StatementCur.DateRangeTo=new DateTime(2200,1,1);//max val
				}
				else{
					StatementCur.DateRangeTo=PIn.Date(textDateEnd.Text);
				}
				StatementCur.Note=textNote.Text;
				StatementCur.NoteBold=textNoteBold.Text;
				StatementCur.IsInvoiceCopy=checkIsInvoiceCopy.Checked;
				if(checkSendSms.Checked) {
					if(StatementCur.SmsSendStatus.In(AutoCommStatus.DoNotSend,AutoCommStatus.Undefined)) {
						StatementCur.SmsSendStatus=AutoCommStatus.SendNotAttempted;
					}
				}
				else {
					StatementCur.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				if(StatementCur.IsInvoice || !StatementCur.IsNew) {
					Statements.Update(StatementCur);
					StatementCur.IsNew=false;
				}
				else {//not an invoice and IsNew so insert
					StatementCur.StatementNum=Statements.Insert(StatementCur);
					textInvoiceNum.Text=StatementCur.StatementNum.ToString();
					StatementCur.IsNew=false;//so that if we run this again, it will not do a second insert.
				}
				return true;
			}
			//From here on, we are only working with multiple statements
			DataTable tablePatsDates=new DataTable();
			if(checkBoxBillShowTransSinceZero.Checked) {//populate lookup table of patients and dates
				List<Patient> listPatients=Patients.GetMultPats(ListStatements.Select(x=>x.PatNum).ToList()).ToList();
				List<PatAging> listPatAgings=Patients.GetAgingListSimple(listPatients.Select(x=>x.BillingType).Distinct().ToList(),new List<long> { });
				tablePatsDates=Ledgers.GetDateBalanceBegan(listPatAgings,checkSuperStatement.Checked);
			}
			for(int i=0;i<ListStatements.Count;i++){
				if(textDate.Text!=""){
					ListStatements[i].DateSent=PIn.Date(textDate.Text);
				}
				if(checkIsSent.CheckState!=CheckState.Indeterminate){
					ListStatements[i].IsSent=checkIsSent.Checked;
				}
				if(listMode.SelectedIndex!=-1){
					ListStatements[i].Mode_=listMode.GetSelected<StatementMode>();
				}
				if(checkHidePayment.CheckState!=CheckState.Indeterminate){
					ListStatements[i].HidePayment=checkHidePayment.Checked;
				}
				if(checkSinglePatient.CheckState!=CheckState.Indeterminate){
					ListStatements[i].SinglePatient=checkSinglePatient.Checked;
				}
				if(checkIntermingled.CheckState!=CheckState.Indeterminate){
					ListStatements[i].Intermingled=checkIntermingled.Checked;
				}
				if(checkIsReceipt.CheckState!=CheckState.Indeterminate) {
					ListStatements[i].IsReceipt=checkIsReceipt.Checked;
				}
				if(textDateStart.Text!="?"){
					ListStatements[i].DateRangeFrom=PIn.Date(textDateStart.Text);//handles blank
				}
				if(textDateStart.Text!="?"){
					if(textDateEnd.Text==""){
						ListStatements[i].DateRangeTo=new DateTime(2200,1,1);//max val
					}
					else{
						ListStatements[i].DateRangeTo=PIn.Date(textDateEnd.Text);
					}
				}
				if(textNote.Text!="?"){
					ListStatements[i].Note=textNote.Text;
				}
				if(textNoteBold.Text!="?"){
					ListStatements[i].NoteBold=textNoteBold.Text;
				}
				if(checkBoxBillShowTransSinceZero.Checked) {
					DateTime dateFrom=DateTime.MinValue;
					DataRow[] dataRowArray=tablePatsDates.Select("PatNum='"+ListStatements[i].PatNum.ToString()+"'");
					if(dataRowArray.Length>0){
						dateFrom=PIn.DateT(dataRowArray[0]["DateZeroBal"].ToString());
					}
					ListStatements[i].DateRangeFrom=DateTime.Now;
					if(dateFrom!=DateTime.MinValue) {//patient does not have a zero or credit balance.
						ListStatements[i].DateRangeFrom=dateFrom;
					}
				}
				if(checkSendSms.CheckState!=CheckState.Indeterminate) {
					if(checkSendSms.Checked) {
						if(ListStatements[i].SmsSendStatus.In(AutoCommStatus.DoNotSend,AutoCommStatus.Undefined)) {
							ListStatements[i].SmsSendStatus=AutoCommStatus.SendNotAttempted;
						}
					}
					else {
						ListStatements[i].SmsSendStatus=AutoCommStatus.DoNotSend;
					}
				}
				Statements.Update(ListStatements[i]);//never new
			}
			return true;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(ListStatements==null && _isStatementNew && StatementCur.IsInvoice) {
				try {
					//Since the user just created this, we will let them delete the image.
					Statements.DeleteStatements(new List<Statement> { StatementCur },forceImageDelete:true);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Error retrieving patient folder."),ex);
				}
			}
			DialogResult=DialogResult.Cancel;
		}




	}
}
