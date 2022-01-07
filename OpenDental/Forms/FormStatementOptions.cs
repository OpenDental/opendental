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

namespace OpenDental{
	///<summary></summary>
	public partial class FormStatementOptions : FormODBase {
		private bool initiallySent;
		private Patient _superHead;
		private List<Def> _listImageCatDefs;
		private bool _isFromBilling=false;
		///<summary>This is true if on load the single statement IsNew.</summary>
		private bool _isStatementNew;
		public Statement StmtCur;
		///<summary>This will be null for ordinary edits.  But sometimes this window is used to edit bulk statements.  In that case, this list contains the statements being edited.  Must contain at least one item.</summary>
		public List<Statement> StmtList;

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
			if(StmtList==null){
				if(StmtCur.StatementType==StmtType.LimitedStatement) {//Must be called before SetEnabled below
					checkExcludeTxfr.Visible=true;
				}
				if(StmtCur.IsSent){
					checkIsSent.Checked=true;
					initiallySent=true;
					SetEnabled(false);
				}
				textDate.Text=StmtCur.DateSent.ToShortDateString();
				checkBoxBillShowTransSinceZero.Checked=PrefC.GetBool(PrefName.BillingShowTransSinceBalZero);
				listMode.Items.Clear();
				listMode.Items.AddEnums<StatementMode>();
				listMode.SetSelectedEnum(StmtCur.Mode_);
				if(StmtCur.Mode_==StatementMode.Electronic) {
					//Automatically select intermingling family and remove that as a selection option.
					checkSinglePatient.Checked=false;
					checkSinglePatient.Enabled=false;
					checkIntermingled.Checked=true;
					checkIntermingled.Enabled=false;
				}
				checkHidePayment.Checked=StmtCur.HidePayment;
				checkSinglePatient.Checked=StmtCur.SinglePatient;
				checkIntermingled.Checked=StmtCur.Intermingled;
				checkIsReceipt.Checked=StmtCur.IsReceipt;
				if(PrefC.IsODHQ){
					checkShowLName.Checked=true;
				}
				if(StmtCur.IsInvoice) {//If they got here with drop down menu invoice item.
					if(CultureInfo.CurrentCulture.Name=="en-US") {
						checkIsInvoiceCopy.Visible=false;
					}
					checkIsInvoice.Checked=true;
					checkIsInvoiceCopy.Checked=StmtCur.IsInvoiceCopy;
					textInvoiceNum.Text=StmtCur.StatementNum.ToString();
					groupDateRange.Visible=false;
					checkIsReceipt.Visible=false;
					checkIntermingled.Visible=false;
					checkBoxBillShowTransSinceZero.Visible=false;
				}
				else {
					groupInvoice.Visible=false;
				}
				if(StmtCur.StatementType==StmtType.LimitedStatement) {
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
				if(StmtCur.DateRangeFrom.Year>1880){
					textDateStart.Text=StmtCur.DateRangeFrom.ToShortDateString();
				}
				if(StmtCur.DateRangeTo.Year<2100){
					textDateEnd.Text=StmtCur.DateRangeTo.ToShortDateString();
				}
				textNote.Text=StmtCur.Note;
				textNoteBold.Text=StmtCur.NoteBold;
				if(StmtCur.StatementType!=StmtType.LimitedStatement && PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
					Patient guarantor=Patients.GetFamily(StmtCur.PatNum).ListPats[0];
					_superHead=Patients.GetPat(guarantor.SuperFamily);
					if(StmtCur.IsNew && !StmtCur.IsSent && guarantor.HasSuperBilling && guarantor.SuperFamily>0 && _superHead!=null && _superHead.HasSuperBilling) {
						//Statement not sent, statements use sheets, and guarantor is a member of a superfamily, guarantor and superhead both have superbilling enabled.
						//Enable superfam checkbox.  Only if this is a new statement that hasn't been set as a super statement.  If it's already been marked as a super statement, don't allow user to uncheck the box
						checkSuperStatement.Enabled=true;
					}
					checkSuperStatement.Checked=(StmtCur.SuperFamily!=0 && _superHead!=null && _superHead.PatNum==StmtCur.SuperFamily);//check box if superhead statement
				}
				else {//either a limited statement or super family show feature is disabled or statements are not using sheets
					checkSuperStatement.Visible=false;
				}
				if(!_isFromBilling) {
					checkSendSms.Text=Lans.g(this,"Sent text");//The user cannot send a text from this window, so we want to make it clear that they can't.
					checkSendSms.Enabled=false;
					checkSendSms.Checked=StmtCur.SmsSendStatus==AutoCommStatus.SendSuccessful;
				}
				else if(!ListTools.In(StmtCur.SmsSendStatus,AutoCommStatus.DoNotSend,AutoCommStatus.Undefined)) {
					checkSendSms.Checked=true;
				}
				_isStatementNew=StmtCur.IsNew;
			}
			#region Bulk Edit
			else{
				//DateSent-------------------------------------------------------------------------------------
				textDate.Text="?";
				bool allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].DateSent!=StmtList[i].DateSent){//if any are different from the first element
						allSame=false;
					}
				}
				if(allSame){
					textDate.Text=StmtList[0].DateSent.ToShortDateString();
				}
				//IsSent----------------------------------------------------------------------------------------
				checkIsSent.ThreeState=true;
				checkIsSent.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].IsSent!=StmtList[i].IsSent){
						allSame=false;
					}
				}
				if(allSame){
					checkIsSent.ThreeState=false;
					checkIsSent.CheckState=CheckState.Unchecked;
					checkIsSent.Checked=StmtList[0].IsSent;
				}
				//Mode------------------------------------------------------------------------------------------
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].Mode_!=StmtList[i].Mode_){
						allSame=false;
					}
				}
				listMode.Items.Clear();
				listMode.Items.AddEnums<StatementMode>();
				if(allSame) {
					listMode.SetSelectedEnum(StmtList[0].Mode_);
				}
				if(StmtList[0].Mode_==StatementMode.Electronic) {
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
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].HidePayment!=StmtList[i].HidePayment){
						allSame=false;
					}
				}
				if(allSame){
					checkHidePayment.ThreeState=false;
					checkHidePayment.CheckState=CheckState.Unchecked;
					checkHidePayment.Checked=StmtList[0].HidePayment;
				}
				//SinglePatient------------------------------------------------------------------------------------
				checkSinglePatient.ThreeState=true;
				checkSinglePatient.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].SinglePatient!=StmtList[i].SinglePatient){
						allSame=false;
					}
				}
				if(allSame){
					checkSinglePatient.ThreeState=false;
					checkSinglePatient.CheckState=CheckState.Unchecked;
					checkSinglePatient.Checked=StmtList[0].SinglePatient;
				}
				//Intermingled----------------------------------------------------------------------------------------
				checkIntermingled.ThreeState=true;
				checkIntermingled.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].Intermingled!=StmtList[i].Intermingled){
						allSame=false;
					}
				}
				if(allSame){
					checkIntermingled.ThreeState=false;
					checkIntermingled.CheckState=CheckState.Unchecked;
					checkIntermingled.Checked=StmtList[0].Intermingled;
				}
				//DateStart-------------------------------------------------------------------------------------
				textDateStart.Text="?";
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].DateRangeFrom!=StmtList[i].DateRangeFrom){
						allSame=false;
					}
				}
				if(allSame){
					if(StmtList[0].DateRangeFrom.Year<1880){
						textDateStart.Text="";
					}
					else{
						textDateStart.Text=StmtList[0].DateRangeFrom.ToShortDateString();
					}
				}
				//DateEnd-------------------------------------------------------------------------------------
				textDateEnd.Text="?";
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].DateRangeTo!=StmtList[i].DateRangeTo){
						allSame=false;
					}
				}
				if(allSame){
					if(StmtList[0].DateRangeTo.Year<1880){
						textDateEnd.Text="";
					}
					else{
						textDateEnd.Text=StmtList[0].DateRangeTo.ToShortDateString();
					}
				}
				//Note----------------------------------------------------------------------------------------
				textNote.Text="?";
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].Note!=StmtList[i].Note){
						allSame=false;
					}
				}
				if(allSame){
					textNote.Text=StmtList[0].Note;
				}
				//NoteBold----------------------------------------------------------------------------------------
				textNoteBold.Text="?";
				allSame=true;
				for(int i=0;i<StmtList.Count;i++){
					if(StmtList[0].NoteBold!=StmtList[i].NoteBold){
						allSame=false;
					}
				}
				if(allSame){
					textNoteBold.Text=StmtList[0].NoteBold;
				}
				butEmail.Enabled=false;
				butPrint.Enabled=false;
				butPreview.Enabled=false;
				//Send Text Message-------------------------------------------------------------------------------
				checkSendSms.ThreeState=true;
				checkSendSms.CheckState=CheckState.Indeterminate;
				allSame=true;
				for(int i=1;i<StmtList.Count;i++) {
					if(StmtList[0].SmsSendStatus!=StmtList[i].SmsSendStatus) {
						allSame=false;
						break;
					}
				}
				if(allSame) {
					checkSendSms.ThreeState=false;
					checkSendSms.CheckState=CheckState.Unchecked;
					checkSendSms.Checked=(!ListTools.In(StmtList[0].SmsSendStatus,AutoCommStatus.DoNotSend,AutoCommStatus.Undefined));
				}
			}
			#endregion Bulk Edit
			_listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			Plugins.HookAddCode(this,"FormStatementOptions_Load_end");
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
		}

		private void checkIsSent_Click(object sender,EventArgs e) {
			if(initiallySent && !checkIsSent.Checked){//user unchecks the Sent box in order to edit
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Warning.  This will immediately delete the archived copy of the statement.  Continue anyway?")){
					checkIsSent.Checked=true;
					return;
				}
				SetEnabled(true);
				if(StmtCur.Mode_==StatementMode.Electronic) {
					checkSinglePatient.Checked=false;
					checkSinglePatient.Enabled=false;
					checkIntermingled.Checked=true;
					checkIntermingled.Enabled=false;
				}
				if(StmtCur.IsInvoice) {
					checkIsInvoiceCopy.Checked=false;
				}
				if(StmtCur.StatementType==StmtType.LimitedStatement) {
					checkExcludeTxfr.Visible=true;
				}
				//Delete the archived copy of the statement
				if(StmtCur.DocNum!=0){
					Patient pat=Patients.GetPat(StmtCur.PatNum);
					string patFolder=ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath());
					List<Document> listdocs=new List<Document>();
					listdocs.Add(Documents.GetByNum(StmtCur.DocNum,true));
					try {
						ImageStore.DeleteDocuments(listdocs,patFolder);
						Statements.DetachDocFromStatements(StmtCur.DocNum);
						StmtCur.DocNum=0;
					}
					catch(Exception ex) {  //Image could not be deleted, in use.
						MessageBox.Show(this,ex.Message);
						return;
					}
				}
			}
			else if(StmtList==null && StmtCur.IsInvoice && checkIsSent.Checked) {
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
			if(StmtCur.DocNum!=0 && checkIsSent.Checked) {
				Patient pat=Patients.GetPat(StmtCur.PatNum);
				string patFolder=ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath());
				if(!FileAtoZ.Exists(ImageStore.GetFilePath(Documents.GetByNum(StmtCur.DocNum),patFolder))) { 
					MsgBox.Show(this,"File not found: " + Documents.GetByNum(StmtCur.DocNum).FileName);
					return;
				}
			}
			butPrintSheets();
		}

		private void butPrintSheets() {
			_isStatementNew=false;//At this point, the statment will no longer be new. 
			Patient patCur = Patients.GetPat(StmtCur.PatNum);
			if(StmtCur.DocNum!=0 && checkIsSent.Checked 
				&& !StmtCur.IsInvoice)//Invoices are always recreated on the fly in order to show "Copy" when needed.
			{
				//launch existing archive pdf. User can click print from within Acrobat.
				LaunchArchivedPdf(patCur);
			}
			else {//was not initially sent, or else user has unchecked the sent box
				if(initiallySent && checkIsSent.Checked && StmtCur.DocNum==0 
					&& !StmtCur.IsInvoice)//for invoice, we don't notify user that it's a recreation
				{
					MsgBox.Show(this,"There was no archived image of this statement.  The printout will be based on current data.");
				}
				//So create an archive
				if(listMode.GetSelected<StatementMode>()==StatementMode.Email) {
					listMode.SetSelectedEnum(StatementMode.InPerson);
				}
				checkIsSent.Checked=true;
				Cursor=Cursors.WaitCursor;
				Patient guarantor = null;
				if(patCur!=null) {
					guarantor = Patients.GetPat(patCur.Guarantor);
				}
				if(checkSuperStatement.Checked && guarantor!=null && guarantor.SuperFamily!=0) {
					List<Patient> listFamilyGuarantors=Patients.GetSuperFamilyGuarantors(guarantor.SuperFamily).FindAll(x => x.HasSuperBilling);
					//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
					if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
						listFamilyGuarantors=listFamilyGuarantors.FindAll(x => x.BalTotal>0);
					}
					else {
						listFamilyGuarantors=listFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>=0);
						StmtCur.InsEst=listFamilyGuarantors.Sum(x => x.InsEst);
					}
					StmtCur.BalTotal=listFamilyGuarantors.Sum(x => x.BalTotal);
					StmtCur.IsBalValid=true;
				}
				else if(guarantor!=null) {
					StmtCur.BalTotal=guarantor.BalTotal;
					StmtCur.InsEst=guarantor.InsEst;
					StmtCur.IsBalValid=true;
				}
				if(!SaveToDb()) {
					Cursor=Cursors.Default;
					return;
				}
				if(!SaveAsDocument(true)) {
					return;
				}
				Cursor=Cursors.Default;
			}
			DialogResult=DialogResult.OK;
		}

		///<summary>Creates a PDF if necessary and attaches the statement document to the statement.</summary>
		///<param name="pdfFileName">If this is blank, a PDF will be created.</param>
		///<param name="sheet">This sheet will be used to create the PDF. If it is null, the default Statement sheet will be used instead.</param>
		private bool SaveAsDocument(bool printSheet=false,string pdfFileName="",Sheet sheet=null,DataSet dataSet=null) {
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StmtCur);
			string tempPath;
			if(dataSet==null) {
				if(checkSuperStatement.Checked) {
					dataSet=AccountModules.GetSuperFamAccount(StmtCur,doIncludePatLName: checkShowLName.Checked,doShowHiddenPaySplits: StmtCur.IsReceipt,doExcludeTxfrs: checkExcludeTxfr.Checked);
				}
				else {
					dataSet=AccountModules.GetAccount(StmtCur.PatNum,StmtCur,doIncludePatLName: checkShowLName.Checked,doShowHiddenPaySplits: StmtCur.IsReceipt,doExcludeTxfrs: checkExcludeTxfr.Checked);
				}
			}
			if(pdfFileName=="") {
				if(sheet==null) {
					sheet=SheetUtil.CreateSheet(sheetDef,StmtCur.PatNum,StmtCur.HidePayment);
					sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StmtCur });
					SheetFiller.FillFields(sheet,dataSet,StmtCur);
					SheetUtil.CalculateHeights(sheet,dataSet,StmtCur);
				}
				tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),StmtCur.PatNum.ToString()+".pdf");
				SheetPrinting.CreatePdf(sheet,tempPath,StmtCur,dataSet,null);
			}
			else {
				tempPath=pdfFileName;
			}
			long category=0;
			for(int i=0;i<_listImageCatDefs.Count;i++) {
				if(Regex.IsMatch(_listImageCatDefs[i].ItemValue,@"S")) {
					category=_listImageCatDefs[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=_listImageCatDefs[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document docc=null;
			try {
				docc=ImageStore.Import(tempPath,category,Patients.GetPat(StmtCur.PatNum));
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
			docc.ImgType=ImageType.Document;
			if(StmtCur.IsInvoice) {
				docc.Description=Lan.g(this,"Invoice");
			}
			else {
				if(StmtCur.IsReceipt==true) {
					docc.Description=Lan.g(this,"Receipt");
				}
				else {
					docc.Description=Lan.g(this,"Statement");
				}
			}
			//Some customers have wanted to sort their statements in the images module by date and time.  
			//We would need to enhance DateSent to include the time portion.
			docc.DateCreated=StmtCur.DateSent;
			StmtCur.DocNum=docc.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(StmtCur.StatementNum,docc);
			Statements.SyncStatementProdsForStatement(dataSet,StmtCur.StatementNum,StmtCur.DocNum);
			if(printSheet) {
				//Actually print the statement.
				//NOTE: This is printing a "fresh" GDI+ version of the statment which is ever so slightly different than the PDFSharp statment that was saved to disk.
				sheet=SheetUtil.CreateSheet(sheetDef,StmtCur.PatNum,StmtCur.HidePayment);
				sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StmtCur });
				SheetFiller.FillFields(sheet,dataSet,StmtCur);
				SheetUtil.CalculateHeights(sheet,dataSet,StmtCur);
				SheetPrinting.Print(sheet,dataSet,1,false,StmtCur);//use GDI+ printing, which is slightly different than the pdf.
				if(StmtCur.IsInvoice && checkIsInvoiceCopy.Visible) {//for foreign countries
					StmtCur.IsInvoiceCopy=true;
					Statements.Update(StmtCur);
				}
			}
			return true;
		}

		private void butEmail_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			_isStatementNew=false;//At this point, the statment will no longer be new. 
			if(StmtCur.DocNum!=0 && checkIsSent.Checked) {
				//remail existing archive pdf?
				//or maybe tell user they can't do that?
				MsgBox.Show(this,"Statement has already been sent.");
				return;
			}
			else {//was not initially sent, or else user has unchecked the sent box
				//So create an archive
				if(listMode.GetSelected<StatementMode>()!=StatementMode.Email) {
					listMode.SetSelectedEnum(StatementMode.Email);
				}
				if(!CreatePdfForSheet() || !CreateEmailMessage()){
					Cursor=Cursors.Default;
					checkIsSent.Checked=false;
					return;
				}
				if(StmtCur.IsInvoice && checkIsInvoiceCopy.Visible) {//for foreign countries
					StmtCur.IsInvoiceCopy=true;
				}
				//Email was sent. Update the statement.
				StmtCur.IsSent=checkIsSent.Checked;
				Statements.Update(StmtCur);
				Cursor=Cursors.Default;
			}
			DialogResult=DialogResult.OK;
		}

		private bool CreatePdfForSheet() {
			_isStatementNew=false;//At this point, the statment will no longer be new. 
			Cursor=Cursors.WaitCursor;
			Patient patCur=Patients.GetPat(StmtCur.PatNum);
			Patient guarantor=null;
			if(patCur!=null) {
				guarantor=Patients.GetPat(patCur.Guarantor);
			}
			if(checkSuperStatement.Checked && guarantor!=null && guarantor.SuperFamily!=0) {
				List<Patient> listFamilyGuarantors=Patients.GetSuperFamilyGuarantors(guarantor.SuperFamily).FindAll(x => x.HasSuperBilling);
				//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					listFamilyGuarantors=listFamilyGuarantors.FindAll(x => x.BalTotal>0);
				}
				else {
					listFamilyGuarantors=listFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>=0);
					StmtCur.InsEst=listFamilyGuarantors.Sum(x => x.InsEst);
				}
				StmtCur.BalTotal=listFamilyGuarantors.Sum(x => x.BalTotal);
				StmtCur.IsBalValid=true;
			}
			else if(guarantor!=null) {
				StmtCur.BalTotal=guarantor.BalTotal;
				StmtCur.InsEst=guarantor.InsEst;
				StmtCur.IsBalValid=true;
			}
			if(!SaveToDb()) {
				return false;
			}
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StmtCur);
			DataSet dataSet=null;
			if(checkSuperStatement.Checked) {
				//handled in SaveToDb()
				//StmtCur.SuperFamily=Patients.GetPat(StmtCur.PatNum).SuperFamily;
				//StmtCur.PatNum=StmtCur.SuperFamily;
				dataSet=AccountModules.GetSuperFamAccount(StmtCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StmtCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
			}
			else {
				dataSet=AccountModules.GetAccount(StmtCur.PatNum,StmtCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StmtCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,StmtCur.PatNum,StmtCur.HidePayment);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StmtCur });
			SheetFiller.FillFields(sheet,dataSet,StmtCur);
			SheetUtil.CalculateHeights(sheet,dataSet,StmtCur);
			string tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),StmtCur.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,tempPath,StmtCur,dataSet,null);
			long category=0;
			for(int i=0;i<_listImageCatDefs.Count;i++) {
				if(Regex.IsMatch(_listImageCatDefs[i].ItemValue,@"S")) {
					category=_listImageCatDefs[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=_listImageCatDefs[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document docc=null;
			try {
				docc=ImageStore.Import(tempPath,category,Patients.GetPat(StmtCur.PatNum));
			}
			catch {
				MsgBox.Show(this,"Error saving document.");
				return false;
			}
			docc.ImgType=ImageType.Document;
			if(StmtCur.IsInvoice) {
				docc.Description=Lan.g(this,"Invoice");
			}
			else {
				if(StmtCur.IsReceipt==true) {
					docc.Description=Lan.g(this,"Receipt");
				}
				else {
					docc.Description=Lan.g(this,"Statement");
				}
			}
			docc.DateCreated=StmtCur.DateSent;
			StmtCur.DocNum=docc.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(StmtCur.StatementNum,docc);
			Statements.SyncStatementProdsForStatement(dataSet,StmtCur.StatementNum,StmtCur.DocNum);
			checkIsSent.Checked=true;
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
			Patient pat=Patients.GetPat(StmtCur.PatNum);
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string oldPath=ODFileUtils.CombinePaths(ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath()),Documents.GetByNum(StmtCur.DocNum).FileName);
				File.Copy(oldPath,filePathAndName);
			}
			else {//Cloud
				using FormProgress FormP=new FormProgress();
				FormP.DisplayText="Downloading patient statement...";
				FormP.NumberFormat="F";
				FormP.NumberMultiplication=1;
				FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				FormP.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath())
					,Documents.GetByNum(StmtCur.DocNum).FileName
					,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				if(FormP.ShowDialog()==DialogResult.Cancel) {
					state.DoCancel=true;
					return false;
				}
				else {
					//Do stuff with state.FileContent
					using FormProgress FormP2=new FormProgress();
					FormP2.DisplayText="Uploading patient email...";
					FormP2.NumberFormat="F";
					FormP2.NumberMultiplication=1;
					FormP2.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					FormP2.TickMS=1000;
					OpenDentalCloud.Core.TaskStateUpload state2=CloudStorage.UploadAsync(attachPath
						,fileName
						,state.FileContent
						,new OpenDentalCloud.ProgressHandler(FormP2.UpdateProgress));
					if(FormP2.ShowDialog()==DialogResult.Cancel) {
						state2.DoCancel=true;
						return false;
					}
					else {
						//Upload was successful
					}
				}
			}
			//Process.Start(filePathAndName);
			EmailMessage message=Statements.GetEmailMessageForStatement(StmtCur,pat);
			EmailAttach attach=new EmailAttach();
			attach.DisplayedFileName="Statement.pdf";
			attach.ActualFileName=fileName;
			message.Attachments.Add(attach);
			using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message,EmailAddresses.GetByClinic(pat.ClinicNum));
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult==DialogResult.OK){
				return true;
			}
			return false;
		}

		private void butPreview_Click(object sender,EventArgs e) {
			butPreviewSheets();
		}

		private void butPreviewSheets() {
			Patient patCur = Patients.GetPat(StmtCur.PatNum);
			if(StmtCur.DocNum!=0 && checkIsSent.Checked) {//initiallySent && checkIsSent.Checked){
				string billingType=PrefC.GetString(PrefName.BillingUseElectronic);
				if(StmtCur.Mode_==StatementMode.Electronic && (billingType=="1" || billingType=="3") && !PrefC.GetBool(PrefName.BillingElectCreatePDF)) {
					MsgBox.Show(this,"PDF's are not saved for electronic billing.  Unable to view.");
					return;
				}
				else {
					LaunchArchivedPdf(patCur);
				}
			}
			else {//was not initially sent, or else user has unchecked the sent box
				Cursor=Cursors.WaitCursor;
				Patient guarantor = null;
				if(patCur!=null) {
					guarantor = Patients.GetPat(patCur.Guarantor);
				}
				if(checkSuperStatement.Checked && guarantor!=null && guarantor.SuperFamily!=0) {
					List<Patient> listFamilyGuarantors=Patients.GetSuperFamilyGuarantors(guarantor.SuperFamily).FindAll(x => x.HasSuperBilling);
					//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
					if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
						listFamilyGuarantors=listFamilyGuarantors.FindAll(x => x.BalTotal>0);
					}
					else {
						listFamilyGuarantors=listFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>=0);
						StmtCur.InsEst=listFamilyGuarantors.Sum(x => x.InsEst);
					}
					StmtCur.BalTotal=listFamilyGuarantors.Sum(x => x.BalTotal);
					StmtCur.IsBalValid=true;
				}
				else if(guarantor!=null) {
					StmtCur.BalTotal=guarantor.BalTotal;
					StmtCur.InsEst=guarantor.InsEst;
					StmtCur.IsBalValid=true;
				}
				if(!SaveToDb()) {
					Cursor=Cursors.Default;
					return;
				}
				SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StmtCur);
				DataSet dataSet=null;
				if(checkSuperStatement.Checked) {
					//handled in SaveToDb()
					//StmtCur.SuperFamily=Patients.GetPat(StmtCur.PatNum).SuperFamily;
					//StmtCur.PatNum=StmtCur.SuperFamily;
					dataSet=AccountModules.GetSuperFamAccount(StmtCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StmtCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
				}
				else {
					dataSet=AccountModules.GetAccount(StmtCur.PatNum,StmtCur,doIncludePatLName:checkShowLName.Checked,doShowHiddenPaySplits:StmtCur.IsReceipt,doExcludeTxfrs:checkExcludeTxfr.Checked);
				}
				Sheet sheet=SheetUtil.CreateSheet(sheetDef,StmtCur.PatNum,StmtCur.HidePayment);
				sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StmtCur });
				SheetFiller.FillFields(sheet,dataSet,StmtCur);
				SheetUtil.CalculateHeights(sheet,dataSet,StmtCur,true);
				Cursor=Cursors.Default;
				//print directly to PDF here, and save it.
				using FormSheetFillEdit FormSFE=new FormSheetFillEdit(sheet,dataSet);
				FormSFE.Stmt=StmtCur;
				FormSFE.IsStatement=true;
				FormSFE.SaveStatementToDocDelegate=SaveStatementAsDocument;
				FormSFE.ShowDialog();
				if(FormSFE.HasEmailBeenSent) {
					FormSFE.Stmt.Mode_=StatementMode.Email;
					listMode.SetSelectedEnum(StatementMode.Email);
				}
			}
		}

		private void SaveStatementAsDocument(Statement stmt,Sheet sheet,DataSet dataSet,string pdfFileName) {
			checkIsSent.Checked=SaveAsDocument(pdfFileName:pdfFileName,sheet:sheet,dataSet:dataSet);
			if(checkIsSent.Checked) {
				//if the statement was viewed and it was printed/emailed, then it is no longer new.  
				_isStatementNew=false;
			}
		}

		///<summary>Opens the saved PDF for the document.</summary>
		private void LaunchArchivedPdf(Patient patCur) {
			string patFolder=ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath());
			Document doc=Documents.GetByNum(StmtCur.DocNum);
			string fileName=ImageStore.GetFilePath(doc,patFolder);
			if(!FileAtoZ.Exists(fileName)) {
				MessageBox.Show(Lan.g(this,"File not found:")+" "+doc.FileName);
				return;
			}
			try {
				FileAtoZ.StartProcess(fileName);
			}
			catch(Exception ex) {
				FriendlyException.Show($"Unable to open the following file: {doc.FileName}",ex);
			}
		}

		private void butPatPortal_Click(object sender,EventArgs e) {
			if(!Defs.GetDefsForCategory(DefCat.ImageCats,true).Any(x => x.ItemValue.Contains(ImageCategorySpecial.L.ToString())
				&& x.ItemValue.Contains(ImageCategorySpecial.S.ToString()))) {
				MsgBox.Show(this,"There is no image category for Patient Portal and Statements in Setup | Definitions | Image Categories. "
					+"There must be at least one to send portal statements.");
				return;
			}
			if(UserWebs.GetByFKeyAndType(StmtCur.PatNum,UserWebFKeyType.PatientPortal)==null) {
				MsgBox.Show(this,"This patient does not have Online Access to the Patient Portal.");
				return;
			}
			//After checking the preference, CreatePdfForSheet() is called, which will try to create a pdf of the sheet
			if(!CreatePdfForSheet()) {
				MsgBox.Show(this,"There was an error creating a PDF for this patient");
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Send an email to the patient notifying them that a statement in available?")) {
				Patient pat=Patients.GetPat(StmtCur.PatNum);
				EmailMessage message=Statements.GetEmailMessageForPortalStatement(StmtCur,pat);
				using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message,EmailAddresses.GetByClinic(pat.ClinicNum));
				FormE.IsNew=true;
				FormE.ShowDialog();
				if(FormE.DialogResult != DialogResult.OK) {
					return;
				}
				StmtCur.IsSent=checkIsSent.Checked;
				Statements.Update(StmtCur);
			}
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
			try {
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
				if(DateTime.Parse(textDate.Text).Year>1880) {
					textDate.Text=DateTime.Parse(textDate.Text).ToString("d");
				}
			}
			catch { }
		}

		private void textDateStart_Validating(object sender,CancelEventArgs e) {
			try {
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
				if(DateTime.Parse(textDateStart.Text).Year>1880) {
					textDateStart.Text=DateTime.Parse(textDateStart.Text).ToString("d");
				}
			}
			catch { }
		}

		private void textDateEnd_Validating(object sender,CancelEventArgs e) {
			try {
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
				if(DateTime.Parse(textDateEnd.Text).Year>1880) {
					textDateEnd.Text=DateTime.Parse(textDateEnd.Text).ToString("d");
				}
			}
			catch { }
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
		}

		private void checkSinglePatient_Click(object sender,EventArgs e) {
			if(checkSinglePatient.Checked) {
				checkSinglePatient.Checked=true;
				checkIntermingled.Checked=false;
			}
			else {
				if(StmtCur.IsInvoice) {
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
			if(StmtCur.IsInvoice) {
				checkIsInvoice.Checked=true;//don't let them uncheck it.
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(StmtList==null && StmtCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try { 
				if(StmtList==null){
					Statements.DeleteStatements(new List<Statement> { StmtCur });
				}
				else{//bulk edit
					Statements.DeleteStatements(StmtList);
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
			if(StmtList==null && StmtCur.DocNum==0) {
				SaveAsDocument(false);//needs to be called after the statement is inserted for the payment plan grid (if present)
			}
			DialogResult=DialogResult.OK;
		}

		private bool SaveToDb(){
			bool isError;
			//Validate Date-------------------------------------------------------------------------------
			isError=false;
			if(textDate.Text==""){//not allowed to be blank.  Other two dates are allowed to be blank.
				if(StmtList==null){//if editing a List, blank indicates dates vary.
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
				MsgBox.Show(this,"Please fix Date.");
				return false;
			}
			//Validate DateStart-------------------------------------------------------------------------------
			isError=false;
			if(textDateStart.Text==""){
				//no error
			}
			else if(textDateStart.Text=="?"){
				if(StmtList==null){
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
				MsgBox.Show(this,"Please fix Start Date.");
				return false;
			}
			//Validate DateEnd-------------------------------------------------------------------------------
			isError=false;
			if(textDateEnd.Text==""){
				//no error
			}
			else if(textDateEnd.Text=="?"){
				if(StmtList==null){
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
			if(StmtList==null){
				if(checkSuperStatement.Checked) {
					StmtCur.PatNum=_superHead.PatNum;
					StmtCur.SuperFamily=_superHead.PatNum;
				}
				StmtCur.DateSent=PIn.Date(textDate.Text);
				StmtCur.IsSent=checkIsSent.Checked;
				StmtCur.Mode_=listMode.GetSelected<StatementMode>();
				StmtCur.HidePayment=checkHidePayment.Checked;
				StmtCur.SinglePatient=checkSinglePatient.Checked;
				StmtCur.Intermingled=checkIntermingled.Checked;
				StmtCur.IsReceipt=checkIsReceipt.Checked;
				StmtCur.IsInvoice=checkIsInvoice.Checked;
				StmtCur.StatementType=checkLimited.Checked?StmtType.LimitedStatement:StmtType.NotSet;//right now only either LimitedStatement or NotSet
				StmtCur.DateRangeFrom=PIn.Date(textDateStart.Text);//handles blank
				if(checkBoxBillShowTransSinceZero.Checked) {
					DateTime dateAsOf=DateTime.Today;//used to determine when the balance on this date began
					if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {//if aging calculated monthly, use the last aging date instead of today
						dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
					}
					Patient patCur=Patients.GetPat(StmtCur.PatNum);
					List<PatAging> patAges=Patients.GetAgingListSimple(new List<long> {}, new List<long> { patCur.Guarantor },true);
					DataTable tableBals=Ledgers.GetDateBalanceBegan(patAges,dateAsOf,checkSuperStatement.Checked);
					if(tableBals.Rows.Count > 0) {
						DateTime fromDate=PIn.Date(tableBals.Rows[0]["DateZeroBal"].ToString());
						if(fromDate==DateTime.MinValue) {//patient has a zero or credit balance.
							StmtCur.DateRangeFrom=DateTime.Now;
						}
						else {
							StmtCur.DateRangeFrom=fromDate;
						}
					}
				}
				if(textDateEnd.Text==""){
					StmtCur.DateRangeTo=new DateTime(2200,1,1);//max val
				}
				else{
					StmtCur.DateRangeTo=PIn.Date(textDateEnd.Text);
				}
				StmtCur.Note=textNote.Text;
				StmtCur.NoteBold=textNoteBold.Text;
				StmtCur.IsInvoiceCopy=checkIsInvoiceCopy.Checked;
				if(checkSendSms.Checked) {
					if(ListTools.In(StmtCur.SmsSendStatus,AutoCommStatus.DoNotSend,AutoCommStatus.Undefined)) {
						StmtCur.SmsSendStatus=AutoCommStatus.SendNotAttempted;
					}
				}
				else {
					StmtCur.SmsSendStatus=AutoCommStatus.DoNotSend;
				}
				if(StmtCur.IsInvoice || !StmtCur.IsNew) {
					Statements.Update(StmtCur);
					StmtCur.IsNew=false;
				}
				else {//not an invoice and IsNew so insert
					StmtCur.StatementNum=Statements.Insert(StmtCur);
					textInvoiceNum.Text=StmtCur.StatementNum.ToString();
					StmtCur.IsNew=false;//so that if we run this again, it will not do a second insert.
				}
			}
			else{
				Dictionary<long,DateTime> dictPatNumDateBalBegan=new Dictionary<long, DateTime>();
				if(checkBoxBillShowTransSinceZero.Checked) {
					//make lookup dict of key=PatNum, value=DateBalBegan
					DateTime dateAsOf=DateTime.Today;//used to determine when the balance on this date began
					if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {//if aging calculated monthly, use the last aging date instead of today
						dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
					}
					List<Patient> listPatients=Patients.GetMultPats(StmtList.Select(x=>x.PatNum).ToList()).ToList();
					List<PatAging> listPatAges=Patients.GetAgingListSimple(listPatients.Select(x=>x.BillingType).Distinct().ToList(),new List<long> { });
					dictPatNumDateBalBegan=Ledgers.GetDateBalanceBegan(listPatAges,dateAsOf,checkSuperStatement.Checked).Rows.OfType<DataRow>()
						.ToDictionary(x => PIn.Long(x["PatNum"].ToString()),x => PIn.Date(x["DateZeroBal"].ToString()));
				}
				for(int i=0;i<StmtList.Count;i++){
					if(textDate.Text!=""){
						StmtList[i].DateSent=PIn.Date(textDate.Text);
					}
					if(checkIsSent.CheckState!=CheckState.Indeterminate){
						StmtList[i].IsSent=checkIsSent.Checked;
					}
					if(listMode.SelectedIndex!=-1){
						StmtList[i].Mode_=listMode.GetSelected<StatementMode>();
					}
					if(checkHidePayment.CheckState!=CheckState.Indeterminate){
						StmtList[i].HidePayment=checkHidePayment.Checked;
					}
					if(checkSinglePatient.CheckState!=CheckState.Indeterminate){
						StmtList[i].SinglePatient=checkSinglePatient.Checked;
					}
					if(checkIntermingled.CheckState!=CheckState.Indeterminate){
						StmtList[i].Intermingled=checkIntermingled.Checked;
					}
					if(checkIsReceipt.CheckState!=CheckState.Indeterminate) {
						StmtList[i].IsReceipt=checkIsReceipt.Checked;
					}
					if(textDateStart.Text!="?"){
						StmtList[i].DateRangeFrom=PIn.Date(textDateStart.Text);//handles blank
					}
					if(textDateStart.Text!="?"){
						if(textDateEnd.Text==""){
							StmtList[i].DateRangeTo=new DateTime(2200,1,1);//max val
						}
						else{
							StmtList[i].DateRangeTo=PIn.Date(textDateEnd.Text);
						}
					}
					if(textNote.Text!="?"){
						StmtList[i].Note=textNote.Text;
					}
					if(textNoteBold.Text!="?"){
						StmtList[i].NoteBold=textNoteBold.Text;
					}
					if(checkBoxBillShowTransSinceZero.Checked) {
						DateTime fromDate=DateTime.MinValue;
						dictPatNumDateBalBegan.TryGetValue(StmtList[i].PatNum,out fromDate);
						if(fromDate==DateTime.MinValue) {//patient has a zero or credit balance.
							StmtList[i].DateRangeFrom=DateTime.Now;
						}
						else {
							StmtList[i].DateRangeFrom=fromDate;
						}
					}
					if(checkSendSms.CheckState!=CheckState.Indeterminate) {
						if(checkSendSms.Checked) {
							if(ListTools.In(StmtList[i].SmsSendStatus,AutoCommStatus.DoNotSend,AutoCommStatus.Undefined)) {
								StmtList[i].SmsSendStatus=AutoCommStatus.SendNotAttempted;
							}
						}
						else {
							StmtList[i].SmsSendStatus=AutoCommStatus.DoNotSend;
						}
					}
					Statements.Update(StmtList[i]);//never new
				}
			}
			return true;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			if(StmtList==null && _isStatementNew && StmtCur.IsInvoice) {
				try {
					//Since the user just created this, we will let them delete the image.
					Statements.DeleteStatements(new List<Statement> { StmtCur },forceImageDelete:true);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Error retrieving patient folder."),ex);
				}
			}
			DialogResult=DialogResult.Cancel;
		}

	}
}
