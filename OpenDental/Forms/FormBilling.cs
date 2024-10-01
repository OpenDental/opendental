using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;
using PdfSharp.Pdf.IO;

namespace OpenDental {
	///<summary></summary>
	public partial class FormBilling : FormODBase {
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		private int _pagesPrinted;
		private bool _isInitial=true;
		public List<long> ClinicNumsSelectedInitial=new List<long>();
		/// <summary>Determined based upon choice made in comboClinic from formBillingOptions</summary>
		public bool IsAllSelected=false;
		///<summary>This can be used to interact with FormProgressExtended.</summary>
		private ODProgressExtended _progExtended;
		public bool IsHistoryStartMinDate;
		///<summary>Used to hold this preference's state when it is different from the default.</summary>
		public bool ShowBillTransSinceZero=false;
		
		///<summary></summary>
		public FormBilling(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBilling_Load(object sender,System.EventArgs e) {
			//NOTE: this form can be very slow and reloads all data every time it gains focus.All data is requeried from the DB.
			//Suggestions on how to improve speed are:
			//1) use form-level signal processing to set a bool on this form to determine if it needs to refresh the grid when it regains focus.
			//2) add index to statement (IsSent, Patnum, DateSent)
			//3) split the get billing table query into two smaller querries, one that gets everything except the LastStatement column, and one to select
			//  the LastStatement, PatNum and then stitch them together in C#. 
			//  In testing this improved execution time from 1.3 seconds to return ~2500 rows down to 0.08 for the main query and 0.05 for the LastStatement date
			//  Stitching the data sets together was not tested but should be faster than MySQL.
			labelPrinted.Text=Lan.g(this,"Printed=")+"0";
			labelEmailed.Text=Lan.g(this,"E-mailed=")+"0";
			labelSentElect.Text=Lan.g(this,"SentElect=")+"0";
			labelTexted.Text=Lan.g(this,"Texted=")+"0";
			comboOrder.Items.Add(Lan.g(this,"BillingType"));
			comboOrder.Items.Add(Lan.g(this,"PatientName"));
			comboOrder.SelectedIndex=0;
			if(IsAllSelected) {
				comboClinic.IsAllSelected=true;
			}
			else {
				comboClinic.ListClinicNumsSelected=ClinicNumsSelectedInitial;
			}
			FillComboEmail();
			FillGrid();
		}

		public override void ProcessSignalODs(List<Signalod> listSignalods) {
			if(listSignalods.Any(x=>x.IType==InvalidType.BillingList)){
				FillGrid();//Signals refresh about every 6 seconds so the data validation for the dates shouldn't be an issue here
			}
		}

		///<summary>Use UI selections to get statement datatable. Used to originate billing run, also called again on resume after billing pause.</summary>
		private DataTable GetBillingDataTable() {
			DateTime dateFrom=PIn.Date(textDateStart.Text);
			//If textDateEnd is empty then will assume dateTo is far in future(2200,1,1) in order to get all statements.
			DateTime dateTo=new DateTime(2200,1,1);
			if(textDateEnd.Text!="" && textDateEnd.IsValid()) {
				dateTo=PIn.Date(textDateEnd.Text);
			}
			//An empty list indicates to Statements.GetBilling to run for all clinics.
			List<long> clinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled && comboClinic.ListClinicNumsSelected.Count>0) {
				clinicNums.AddRange(comboClinic.ListClinicNumsSelected);
			}
			return Statements.GetBilling(radioSent.Checked,comboOrder.SelectedIndex,dateFrom,dateTo,clinicNums);
		}

		///<summary>We will always try to preserve the selected bills as well as the scroll postition.</summary>
		private void FillGrid() {
			int scrollPos=gridBill.ScrollValue;
			List<long> selectedKeys=gridBill.SelectedIndices.OfType<int>().Select(x => PIn.Long(((DataRow)gridBill.ListGridRows[x].Tag)["StatementNum"].ToString())).ToList();
			DataTable table=GetBillingDataTable();
			gridBill.BeginUpdate();
			gridBill.Columns.Clear();
			GridColumn column=null;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				column=new GridColumn(Lan.g("TableBilling","Name"),150);
			}
			else {
				column=new GridColumn(Lan.g("TableBilling","Name"),180);
			}
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","BillType"),110);
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","Mode"),80);
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","LastStatement"),100);
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","BalTot"),70,HorizontalAlignment.Right);
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","-InsEst"),70,HorizontalAlignment.Right);
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","=AmtDue"),70,HorizontalAlignment.Right);
			gridBill.Columns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","PayPlanDue"),70,HorizontalAlignment.Right);
			gridBill.Columns.Add(column);
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				column=new GridColumn(Lan.g("TableBilling","SF"),30);
				gridBill.Columns.Add(column);
			}
			gridBill.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<table.Rows.Count;++i) { 
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["name"].ToString());
				row.Cells.Add(table.Rows[i]["billingType"].ToString());
				row.Cells.Add(table.Rows[i]["mode"].ToString());
				row.Cells.Add(table.Rows[i]["lastStatement"].ToString());
				row.Cells.Add(table.Rows[i]["balTotal"].ToString());
				row.Cells.Add(table.Rows[i]["insEst"].ToString());
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(table.Rows[i]["amountDue"].ToString());
				}
				row.Cells.Add(table.Rows[i]["payPlanDue"].ToString());
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)){
					if(table.Rows[i]["SuperFamily"].ToString()!="0") {
						row.Cells.Add("X");
					}
					else {
						row.Cells.Add("");
					}
				}
				row.Tag=table.Rows[i];
				gridBill.ListGridRows.Add(row);
			}
			gridBill.EndUpdate();
			if(_isInitial){
				gridBill.SetAll(true);
				_isInitial=false;
			}
			else {
				for(int i=0;i<gridBill.ListGridRows.Count;i++) {
					gridBill.SetSelected(i,selectedKeys.Contains(PIn.Long(((DataRow)gridBill.ListGridRows[i].Tag)["StatementNum"].ToString())));
				}
			}
			gridBill.ScrollValue=scrollPos;
			labelTotal.Text=Lan.g(this,"Total=")+table.Rows.Count.ToString();
			labelSelected.Text=Lan.g(this,"Selected=")+gridBill.SelectedIndices.Length.ToString();
		}

		private void FillComboEmail() {
			long curUserNum=Security.CurUser.UserNum;
			List<EmailAddress> listEmailAddresses=EmailAddresses.GetEmailAddressesForComboBoxes(curUserNum);
			comboEmailFrom.Items.Clear();
			comboEmailFrom.Items.AddList(listEmailAddresses,x=>EmailAddresses.GetDisplayStringForComboBox(x,curUserNum));
			comboEmailFrom.SelectedIndex=0;
		}

		private void butAll_Click(object sender, System.EventArgs e) {
			gridBill.SetAll(true);
			labelSelected.Text=Lan.g(this,"Selected=")+gridBill.SelectedIndices.Length.ToString();
		}

		private void butNone_Click(object sender, System.EventArgs e) {	
			gridBill.SetAll(false);
			labelSelected.Text=Lan.g(this,"Selected=")+gridBill.SelectedIndices.Length.ToString();
		}

		private void radioUnsent_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void radioSent_Click(object sender,EventArgs e) {
			textDateStart.Text=DateTime.Today.ToShortDateString();
			textDateEnd.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

		private void comboOrder_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			FillGrid();
		}

		private void gridBill_CellClick(object sender,ODGridClickEventArgs e) {
			labelSelected.Text=Lan.g(this,"Selected=")+gridBill.SelectedIndices.Length.ToString();
		}

		private void gridBill_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Statement statement=Statements.GetStatement(PIn.Long(((DataRow)gridBill.ListGridRows[e.Row].Tag)["StatementNum"].ToString()));
			ShowStatementOptions(statement);
		}

		private void gridBill_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right){
				gridBill.SetAll(false);
			}
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			if(gridBill.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one bill first.");
				return;
			}
			long patNum=PIn.Long(((DataRow)gridBill.ListGridRows[gridBill.GetSelectedIndex()].Tag)["PatNum"].ToString());
			GlobalFormOpenDental.PatientSelected(Patients.GetPat(patNum),false);
			GlobalFormOpenDental.GoToModule(EnumModuleType.Account,patNum:0);
			SendToBack();
		}

		private void butEdit_Click(object sender,EventArgs e) {
			if(gridBill.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one or more bills first.");
				return;
			}
			using FormStatementOptions FormStatementOptions=new FormStatementOptions(true);
			List<long> listStatementNums=new List<long>();
			foreach(int index in gridBill.SelectedIndices) {
				listStatementNums.Add(PIn.Long(((DataRow)gridBill.ListGridRows[index].Tag)["StatementNum"].ToString()));
			}
			List<Statement> listStatementsSelected=Statements.GetStatements(listStatementNums);
			if(listStatementsSelected.Count==1) {
				ShowStatementOptions(listStatementsSelected[0]);
				return;
			}
			//At least 2 statements selected
			int countSkipped=listStatementsSelected.RemoveAll(x => x.LimitedCustomFamily!=EnumLimitedCustomFamily.None);
			string stringWarning=Lan.g(this,"Limited (Custom) Statements must be edited individually.");
			if(listStatementsSelected.IsNullOrEmpty()) {
				MsgBox.Show(stringWarning);
				return;
			}
			if(countSkipped > 0) {
				stringWarning+=$"\r\n{countSkipped} "+Lan.g(this,"item(s) skipped.");
				MsgBox.Show(stringWarning);
			}
			if(listStatementsSelected.Count==1) {
				ShowStatementOptions(listStatementsSelected[0]);
				return;
			}
			FormStatementOptions.ListStatements=listStatementsSelected;
			FormStatementOptions.ShowDialog();
			if(FormStatementOptions.DialogResult==DialogResult.OK){
				FillGrid();
			}
		}

		private void butPrintList_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Billing list printed"));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Billing List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=gridBill.PrintPage(g,_pagesPrinted,bounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butSend_Click(object sender,System.EventArgs e) {
			BillingUseElectronicEnum electronicBillingType=PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic);
			if(ODEnvironment.IsCloudServer && electronicBillingType.In(
					BillingUseElectronicEnum.ClaimX,
					BillingUseElectronicEnum.EDS,
					BillingUseElectronicEnum.POS
				)) 
			{
				MsgBox.Show(this,$"Electronic statements using {electronicBillingType.GetDescription()} "
					+"are not available while using Open Dental Cloud.");
				return;
			}
			if(gridBill.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select items first.");
				return;
			}
			labelPrinted.Text=Lan.g(this,"Printed=")+"0";
			labelEmailed.Text=Lan.g(this,"E-mailed=")+"0";
			labelSentElect.Text=Lan.g(this,"SentElect=")+"0";
			labelTexted.Text=Lan.g(this,"Texted=")+"0";
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Please be prepared to wait up to ten minutes while all the bills get processed.\r\nOnce complete, the pdf print preview will be launched in Adobe Reader.  You will print from that program.  Continue?")){
				return;
			}
			//It is ok to allow statement sending if this text message template is empty because this does not determine if text messages actually get sent.
			//That is determined per each patient's PatComm.IsSmsAnOption. This field will be false if clinic not signed up for texting.
			if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.BillingDefaultsSmsTemplate))) {
				if(!MsgBox.Show(
					this,
					MsgBoxButtons.YesNo,
					"Cannot send blank text messages. Please update the SMS Statements template in Billing Defaults. Statements will still be printed and sent electronically.  Continue?"
					,"Send Statements - Warning"))
				{
					return;
				}
			}
			_progExtended=new ODProgressExtended(this,tag: new ProgressBarHelper(Lan.g(this,"Billing Progress"),progressBarEventType: ProgBarEventType.Header));
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper("",progressBarEventType: ProgBarEventType.BringToFront)));
			Cursor=Cursors.WaitCursor;
			//FormProgressExtended will insert new bars on top. Statment is on bottom, batch middle, and overall on top. 
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n0 / 0","0%",0,100,ProgBarStyle.Blocks,"3",isTopHidden: true)));
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n0 / 0","0%",0,1,ProgBarStyle.Blocks,"2",isTopHidden: true)));
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),"1%",0,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1",isTopHidden: true)));
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Progress Log"),progressBarEventType: ProgBarEventType.ProgressLog)));
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Preparing First Batch")+"...",progressBarEventType: ProgBarEventType.TextMsg)));
			SendStatementsIO sendStatementsIO=new SendStatementsIO();
			sendStatementsIO.Source="FormBilling";
			sendStatementsIO.ListStatementNumsToSend=gridBill.SelectedTags<DataRow>().Select(x => PIn.Long(x["StatementNum"].ToString())).ToList();
			sendStatementsIO.FuncGetIsHistoryStartMinDate=() => { return IsHistoryStartMinDate; };
			sendStatementsIO.FuncAskQuestion=(question) => {
				//use a MessageBox for now until MsgBox hiding behind things is fixed
				return MessageBox.Show(question,"",MessageBoxButtons.YesNo)==DialogResult.Yes;
			};
			sendStatementsIO.ActionPrompt=(prompt,useCopyPasteDialog) => {
				if(useCopyPasteDialog) {
					MsgBoxCopyPaste msgBoxPatErrors=new MsgBoxCopyPaste(prompt);
					msgBoxPatErrors.Show(this);
				}
				else {
					MessageBox.Show(prompt);
				}				
			};
			sendStatementsIO.FuncChooseSaveFile=(initialSaveDirectory) => {
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.FileName="Statements.xml";
				saveFileDialog.CheckPathExists=true;
				if(!string.IsNullOrEmpty(initialSaveDirectory)) {
					saveFileDialog.InitialDirectory=initialSaveDirectory;
				}
				ChooseSaveFile chooseSaveFile=new ChooseSaveFile();
				if(saveFileDialog.ShowDialog()==DialogResult.OK) {
					chooseSaveFile.FileName=saveFileDialog.FileName;
					chooseSaveFile.IsSelectionOk=true;
				}
				return chooseSaveFile;
			};
			sendStatementsIO.ActionProgressEvent=(odEventArgs) => {
				_progExtended?.Fire(odEventArgs);
				labelPrinted.Text=Lan.g(this,"Printed=")+sendStatementsIO.CountStatementsPrinted.ToString();
				labelEmailed.Text=Lan.g(this,"E-mailed=")+sendStatementsIO.CountStatementsEmailed.ToString();
				labelSentElect.Text=Lan.g(this,"SentElect=")+sendStatementsIO.CountStatementsSentElectronic.ToString();
				labelTexted.Text=Lan.g(this,"Texted=")+sendStatementsIO.CountStatmentsSentPayPortalText.ToString();
				Application.DoEvents();
			};
			sendStatementsIO.FuncGetBillingDataTable=() => { return GetBillingDataTable(); };
			sendStatementsIO.FunctGetIsPaused=() => { return _progExtended?.IsPaused??false; };
			sendStatementsIO.FuncGetIsCancelled=() => { return _progExtended?.IsCanceled??true; };
			sendStatementsIO.ActionSleepDuringPause=() => { System.Threading.Thread.Sleep(100); };			
			sendStatementsIO.FuncGetSenderEmailAddress=(clinicNum) => {
				EmailAddress emailAddress;
				if(comboEmailFrom.SelectedIndex==0) { //clinic/practice default
					emailAddress=EmailAddresses.GetByClinic(clinicNum);
				}
				else {
					emailAddress=comboEmailFrom.GetSelected<EmailAddress>();
				}
				//Use clinic's Email Sender Address Override, if present.
				emailAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,clinicNum); 
				return emailAddress;
			};
			sendStatementsIO.ActionSendEmail=(clinicNumPat,emailMessage,emailAddress,useSecureEmail) => {
				if(useSecureEmail) { 
					EmailSecures.InsertMessageThenSend(emailMessage,emailAddress,emailMessage.ToAddress,clinicNumPat);
				}
				else {
					//If IsCloudStorage==true, then we will end up downloading the file again in EmailMessages.SendEmailUnsecure.
					EmailMessages.SendEmail(emailMessage,emailAddress);
				}
			};
			sendStatementsIO.FuncGetPatientPdfPath=(tempPdfFile,filePath) => {
				if(!CloudStorage.IsCloudStorage) {
					//savedPdfPath is just the filename when using DataStorageType.InDatabase
					return filePath;
				}
				//Using cloud.
				if(tempPdfFile!="") {
					//To save time by not having to download it.
					return tempPdfFile;
				}
				//We have not yet downloaded the pdf.
				string savedPdfPath=PrefC.GetRandomTempFile("pdf");
				FileAtoZ.Copy(filePath,savedPdfPath,FileAtoZSourceDestination.AtoZToLocal,uploadMessage: "Downloading statement...");
				return savedPdfPath;
			};
			sendStatementsIO.FuncGetPdfDocument=(rawBase64,savedPdfPath) => {
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					byte[] rawData=Convert.FromBase64String(rawBase64);
					using(Stream stream=new MemoryStream(rawData)) {
						return PdfReader.Open(stream,PdfDocumentOpenMode.Import);
					}
				}
				else {
					return PdfReader.Open(savedPdfPath,PdfDocumentOpenMode.Import);
				}
			};
			sendStatementsIO.FuncGetEmailAttachment=(savedPdfPath,documentStatement,patient) => {
				string attachPath=EmailAttaches.GetAttachPath();
				string fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+ODRandom.Next(1000).ToString()+".pdf";
				string filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					ImageStore.Export(filePathAndName,documentStatement,patient);
				}
				else {
					FileAtoZ.Copy(savedPdfPath,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ,uploadMessage: "Uploading statement...");
				}
				EmailAttach emailAttach=new EmailAttach();
				emailAttach.DisplayedFileName="Statement.pdf";
				emailAttach.ActualFileName=fileName;
				return emailAttach;
			};
			sendStatementsIO.ActionDeleteTempPdfFile=(tempPdfFile) => {
				File.Delete(tempPdfFile);
			};
			sendStatementsIO.FuncSendEhgStatement=(xmlData,clincNum) => {
				OpenDentBusiness.Bridges.EHG_statements.Send(xmlData,sendStatementsIO.CurStatementBatch.ClinicNum,out string alertMsg);
				return alertMsg;
			};
			sendStatementsIO.FuncComputeAging=(dateTimeToday) => {
				//Run ComputeAging in a cancellable window. Must complete without cancel in order to return true and allow billing to continue.
				ProgressWin progressOD=new ProgressWin();
				progressOD.ActionMain=() => {
					Ledgers.ComputeAging(0,dateTimeToday);
				};
				progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dateTimeToday.ToShortDateString()+"...";
				try {
					progressOD.ShowDialog();
				}
				catch(Exception ex) {
					Ledgers.AgingExceptionHandler(ex,this);
				}
				//Exception from ActionMain will cause !IsCancelled and !IsSuccess.
				if(progressOD.IsCancelled) {
					return false;
				}
				return progressOD.IsSuccess;
			};
			BillingL.SendStatements(sendStatementsIO);
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),"100%",100,100,ProgBarStyle.Blocks,"1")));
			_progExtended?.Close();
			#region Printing Statements
			//Check to see if any statements were added to the master document. This is the sum of all InPerson and Mail statements.
			if(sendStatementsIO.PdfMasterDocument!=null) {
				string tempFileOutputDocument = PrefC.GetRandomTempFile(".pdf");
				sendStatementsIO.PdfMasterDocument.Save(tempFileOutputDocument);
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(tempFileOutputDocument);
				}
				else {
					try {
						Process.Start(tempFileOutputDocument);
					}
					catch(Exception ex) {
						MsgBox.Show(Lan.g(this,"Error: Please make sure Adobe Reader is installed.")+ex.Message);
					}
				}
			}
			#endregion
			Cursor=Cursors.Default;
			FillGrid();
		}

		///<summary>Includes a FillGrid().</summary>
		private void ShowStatementOptions(Statement statement) {
			using FormStatementOptions formStatementOptions=new FormStatementOptions(true);
			if(statement==null) {
				MsgBox.Show(this,"The statement has been deleted.");
				FillGrid();
				return;
			}
			formStatementOptions.StatementCur=statement;
			formStatementOptions.ShowBillTransSinceZero=ShowBillTransSinceZero;
			formStatementOptions.ShowDialog();
			//Could be changes even if not hit OK. Example: printing.
			FillGrid();
		}
		
		private void FormBilling_FormClosing(object sender,FormClosingEventArgs e) {
			if(radioSent.Checked || gridBill.ListGridRows.Count==0){
				return;
			}
			//this MessageBox will activate every time the window is closed.
			//If the OD shutdown signal causes this window to close, the msgbox will show for a second or two and then forcefully close anyway.
			//In that case, it will not delete unsent bills.
			DialogResult result=MessageBox.Show(Lan.g(this,"You may leave this window open while you work.  If you do close it, do you want to delete all unsent bills?"),
				"",MessageBoxButtons.YesNoCancel);
			if(result==DialogResult.No){
				return;
			}
			else if(result==DialogResult.Cancel){
				e.Cancel=true;
				return;
			}
			//else yes:
			Dictionary<long,List<long>> dictionaryClinicStatmentsToDelete=null;
			int totalCount=0;
			dictionaryClinicStatmentsToDelete=gridBill.ListGridRows.Select(x => (DataRow)x.Tag)
				.Where(x => x["IsSent"].ToString()=="0")
				.GroupBy(x => PIn.Long(x["ClinicNum"].ToString()),x => PIn.Long(x["StatementNum"].ToString()))
				.ToDictionary(x => x.Key,x => x.ToList());
			totalCount=dictionaryClinicStatmentsToDelete.Values.Sum(x => x.Count);
			int runningTotal=0;
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => { 
				foreach(long clinicNum in dictionaryClinicStatmentsToDelete.Keys) {
					ODEvent.Fire(ODEventType.ProgressBar,
						Lan.g(this,"Deleting")+" "+dictionaryClinicStatmentsToDelete[clinicNum].Count+" "+Lan.g(this,"statements.")+"\r\n"
						+Lan.g(this,"Processed")+" "+runningTotal+" "+Lan.g(this,"out of")+" "+totalCount);
					Statements.DeleteAll(dictionaryClinicStatmentsToDelete[clinicNum]);
					runningTotal+=dictionaryClinicStatmentsToDelete[clinicNum].Count;
				}
				//This is not an accurate permission type.
				SecurityLogs.MakeLogEntry(EnumPermType.Accounting,0,"Billing: Unsent statements were deleted.");
			};
			progressOD.StartingMessage="Deleting Statements...";
			try{
				progressOD.ShowDialog();
			}
			catch(Exception ex){
				FriendlyException.Show(Lan.g(this,"Error deleting statements."),ex);
				return;
			}
			if(dictionaryClinicStatmentsToDelete!=null) {//No error. (jordan I don't understand this line)
				MsgBox.Show(Lan.g(this,"Unsent statements deleted: ")+totalCount);
			}
			if(progressOD.IsCancelled){
				return;
			}
			return;
		}

		private void butDefaults_Click(object sender,EventArgs e) {
			using FormBillingDefaults formBillingDefaults = new FormBillingDefaults();
			formBillingDefaults.IsUserPasswordOnly=true;
			formBillingDefaults.ShowDialog();
		}

		///// <summary></summary>
		//private void butSendEbill_Click(object sender,EventArgs e) {
		//	if (gridBill.SelectedIndices.Length == 0){
		//		MessageBox.Show(Lan.g(this, "Please select items first."));
		//		return;
		//	}
		//	Cursor.Current = Cursors.WaitCursor;
		//	// Populate Array And Open eBill Form
		//	ArrayList PatientList = new ArrayList();
		//	for (int i = 0; i < gridBill.SelectedIndices.Length; i++)
		//			PatientList.Add(PIn.Long(table.Rows[gridBill.SelectedIndices[i]]["PatNum"].ToString()));
		//	// Open eBill form
		//	FormPatienteBill FormPatienteBill = new FormPatienteBill(PatientList); 
		//	FormPatienteBill.ShowDialog();
		//	Cursor.Current = Cursors.Default;
		//}

		private delegate void WarningCallback();

	}
}