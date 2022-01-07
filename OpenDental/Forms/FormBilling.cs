using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using OpenDental.Thinfinity;
using OpenDental.NewCrop;

namespace OpenDental{
///<summary></summary>
	public partial class FormBilling : FormODBase {
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		private int _pagesPrinted;
		///<summary>Used in the Activated event.</summary>
		private bool _isPrinting=false;
		private DataTable _table;
		private bool _isInitial=true;
		private bool _ignoreRefreshOnce;
		public long ClinicNumInitial;
		///<summary>Do not pass a list of clinics in.  This list gets filled on load based on the user logged in.  ListClinics is used in other forms so it is public.</summary>
		public List<Clinic> ListClinics;
		private List<EmailAddress> _listEmailAddresses;
		private bool _isActivateFillDisabled;
		private List<long> _listStatementNumsSent;
		private List<long> _listStatementNumsToSkip;
		///<summary>This can be used to interact with FormProgressExtended.</summary>
		private ODProgressExtended _progExtended;
		private bool _hasToShowPdf=false;
		public bool IsHistoryStartMinDate;
		///<summary>The families that are selected when the user hits "Send". The key is the PatNum and the value is its Family.</summary>
		Dictionary<long,Family> _dictionaryFamilies;

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
			//ListClinics can be called even when Clinics is not turned on, therefore it needs to be set to something to avoid a null reference.
			ListClinics=new List<Clinic>();
			_listStatementNumsToSkip=new List<long>();
			if(Clinics.ClinicNum==0) {
				comboClinic.IsAllSelected=true;
			}
			else {
				comboClinic.SelectedClinicNum=Clinics.ClinicNum;
			}
			FillComboEmail();
			_isActivateFillDisabled=false;
		}

		private void FormBilling_Activated(object sender,EventArgs e) {
			if(IsDisposed) {//Attempted bug fix for an exception which occurred in FillGrid() when the grid was already disposed.
				return;
			}
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper("",progressBarEventType:ProgBarEventType.BringToFront)));
			//this gets fired very frequently, including right in the middle of printing a batch.
			if(_isPrinting){
				return;
			}
			if(_ignoreRefreshOnce) {
				_ignoreRefreshOnce=false;
				return;
			}
			if(_isActivateFillDisabled) {
				return;
			}
			FillGrid();
		}

		///<summary>We will always try to preserve the selected bills as well as the scroll postition.</summary>
		private void FillGrid() {
			if(!textDateStart.IsValid() || !textDateEnd.IsValid()) {
				_ignoreRefreshOnce=true;
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			int scrollPos=gridBill.ScrollValue;
			List<long> selectedKeys=gridBill.SelectedIndices.OfType<int>().Select(x => PIn.Long(((DataRow)gridBill.ListGridRows[x].Tag)["StatementNum"].ToString())).ToList();
			DateTime dateFrom=PIn.Date(textDateStart.Text);
			DateTime dateTo=new DateTime(2200,1,1);
			if(textDateEnd.Text!=""){
				dateTo=PIn.Date(textDateEnd.Text);
			}
			List<long> clinicNums=new List<long>();//an empty list indicates to Statements.GetBilling to run for all clinics
			if(PrefC.HasClinicsEnabled && comboClinic.SelectedClinicNum>0) {
				clinicNums.Add(comboClinic.SelectedClinicNum);
			}
			_table=Statements.GetBilling(radioSent.Checked,comboOrder.SelectedIndex,dateFrom,dateTo,clinicNums);
			gridBill.BeginUpdate();
			gridBill.ListGridColumns.Clear();
			GridColumn column=null;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				column=new GridColumn(Lan.g("TableBilling","Name"),150);
			}
			else {
				column=new GridColumn(Lan.g("TableBilling","Name"),180);
			}
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","BillType"),110);
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","Mode"),80);
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","LastStatement"),100);
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","BalTot"),70,HorizontalAlignment.Right);
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","-InsEst"),70,HorizontalAlignment.Right);
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","=AmtDue"),70,HorizontalAlignment.Right);
			gridBill.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g("TableBilling","PayPlanDue"),70,HorizontalAlignment.Right);
			gridBill.ListGridColumns.Add(column);
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				column=new GridColumn(Lan.g("TableBilling","SF"),30);
				gridBill.ListGridColumns.Add(column);
			}
			gridBill.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_table.Rows.Count;++i) { 
				row=new GridRow();
				row.Cells.Add(_table.Rows[i]["name"].ToString());
				row.Cells.Add(_table.Rows[i]["billingType"].ToString());
				row.Cells.Add(_table.Rows[i]["mode"].ToString());
				row.Cells.Add(_table.Rows[i]["lastStatement"].ToString());
				row.Cells.Add(_table.Rows[i]["balTotal"].ToString());
				row.Cells.Add(_table.Rows[i]["insEst"].ToString());
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_table.Rows[i]["amountDue"].ToString());
				}
				row.Cells.Add(_table.Rows[i]["payPlanDue"].ToString());
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)){
					if(_table.Rows[i]["SuperFamily"].ToString()!="0") {
						row.Cells.Add("X");
					}
					else {
						row.Cells.Add("");
					}
				}
				row.Tag=_table.Rows[i];
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
			labelTotal.Text=Lan.g(this,"Total=")+_table.Rows.Count.ToString();
			labelSelected.Text=Lan.g(this,"Selected=")+gridBill.SelectedIndices.Length.ToString();
		}

		private void FillComboEmail() {
			_listEmailAddresses=EmailAddresses.GetDeepCopy();//Does not include user specific email addresses.
			List<Clinic> listClinicsAll=Clinics.GetDeepCopy();
			for(int i=0;i<listClinicsAll.Count;i++) {//Exclude any email addresses that are associated to a clinic.
				_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==listClinicsAll[i].EmailAddressNum);
			}
			//Exclude default practice email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailDefaultAddressNum));
			//Exclude web mail notification email address.
			_listEmailAddresses.RemoveAll(x => x.EmailAddressNum==PrefC.GetLong(PrefName.EmailNotifyAddressNum));
			comboEmailFrom.Items.Add(Lan.g(this,"Practice/Clinic"));//default
			comboEmailFrom.SelectedIndex=0;
			//Add all email addresses which are not associated to a user, a clinic, or either of the default email addresses.
			for(int i=0;i<_listEmailAddresses.Count;i++) {
				comboEmailFrom.Items.Add(_listEmailAddresses[i].EmailUsername);
			}
			//Add user specific email address if present.
			EmailAddress emailAddressMe=EmailAddresses.GetForUser(Security.CurUser.UserNum);//can be null
			if(emailAddressMe!=null) {
				_listEmailAddresses.Insert(0,emailAddressMe);
				comboEmailFrom.Items.Insert(1,Lan.g(this,"Me")+" <"+emailAddressMe.EmailUsername+">");//Just below Practice/Clinic
			}
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
				_ignoreRefreshOnce=true;
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			FillGrid();
		}

		private void gridBill_CellClick(object sender,ODGridClickEventArgs e) {
			labelSelected.Text=Lan.g(this,"Selected=")+gridBill.SelectedIndices.Length.ToString();
		}

		private void gridBill_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormStatementOptions formStatementOptions=new FormStatementOptions(true);
			Statement statement=Statements.GetStatement(PIn.Long(((DataRow)gridBill.ListGridRows[e.Row].Tag)["StatementNum"].ToString()));
			if(statement==null) {
				MsgBox.Show(this,"The statement has been deleted.");
				return;
			}
			formStatementOptions.StmtCur=statement;
			formStatementOptions.ShowDialog();
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
			FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(patNum),false);
			GotoModule.GotoAccount(0);
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
			FormStatementOptions.StmtList=Statements.GetStatements(listStatementNums);
			FormStatementOptions.ShowDialog();
			//FillGrid happens automatically through Activated event.
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
			if(ODBuild.IsWeb() && ListTools.In(PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic),
					BillingUseElectronicEnum.ClaimX,
					BillingUseElectronicEnum.EDS,
					BillingUseElectronicEnum.POS
				)) 
			{
				MsgBox.Show(this,$"Electronic statements using {PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic).GetDescription()} "
					+"are not available while viewing through the web.");
				return;
			}
			_listStatementNumsSent=new List<long>();
			if(gridBill.SelectedIndices.Length==0){
				MessageBox.Show(Lan.g(this,"Please select items first."));
				return;
			}
			labelPrinted.Text=Lan.g(this,"Printed=")+"0";
			labelEmailed.Text=Lan.g(this,"E-mailed=")+"0";
			labelSentElect.Text=Lan.g(this,"SentElect=")+"0";
			labelTexted.Text=Lan.g(this,"Texted=")+"0";
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Please be prepared to wait up to ten minutes while all the bills get processed.\r\nOnce complete, the pdf print preview will be launched in Adobe Reader.  You will print from that program.  Continue?")){
				return;
			}
			PdfDocument pdfDocumentOutput= new PdfDocument();
			_hasToShowPdf=false;
			DateTime dateTimeNow = MiscData.GetNowDateTime();//used to keep track of the time when this first started.
			int skippedDeleted=0;
			Dictionary<string,int> dictionarySkippedElect=new Dictionary<string,int>();//The error message is the key and the value is the skipped count.
			//Whenever we encounter an error, we add the offending patient and reason to this dict to be potentially shown later.
			List<Dictionary<long,string>>  listDictionariesPatnumsSkipped=new List<Dictionary<long,string>> {
				new Dictionary<long,string>(),//Bad Email address
				new Dictionary<long,string>(),//bad mailing address
				new Dictionary<long,string>()	//misc error
			};
			int countEmailed=0;
			int countPrinted=0;
			int countSentElect=0;
			int countTexted=0;
			SendStatements(listDictionariesPatnumsSkipped,ref countEmailed,ref countPrinted,ref countSentElect,ref pdfDocumentOutput,ref skippedDeleted,ref countTexted);
			_progExtended?.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),"100%",100,100,ProgBarStyle.Blocks,"1")));
			_progExtended?.Close();
			#region Printing Statements
			//now print-------------------------------------------------------------------------------------
			if(_hasToShowPdf) {
				string tempFileOutputDocument = PrefC.GetRandomTempFile(".pdf");
				pdfDocumentOutput.Save(tempFileOutputDocument);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFileOutputDocument);
				}
				else {
					try {
						Process.Start(tempFileOutputDocument);
					}
					catch(Exception ex) {
						MessageBox.Show(Lan.g(this,"Error: Please make sure Adobe Reader is installed.")+ex.Message);
					}
				}
			}
			#endregion
			string message="";
			if(listDictionariesPatnumsSkipped[0].Count>0){
				message+=Lan.g(this,"Skipped due to missing or bad email address:")+" "+listDictionariesPatnumsSkipped[0].Count.ToString()+"\r\n";
			}
			if(listDictionariesPatnumsSkipped[1].Count>0) {
				message+=Lan.g(this,"Skipped due to missing or bad mailing address:")+" "+listDictionariesPatnumsSkipped[1].Count.ToString()+"\r\n";
			}
			if(skippedDeleted>0) {
				message+=Lan.g(this,"Skipped due to being deleted by another user:")+" "+skippedDeleted.ToString()+"\r\n";
			}
			if(listDictionariesPatnumsSkipped[2].Count>0) {
				message+=Lan.g(this,"Skipped due to miscellaneous error")+": "+listDictionariesPatnumsSkipped[2].Count.ToString()+"\r\n";
			}
			message+=Lan.g(this,"Printed:")+" "+countPrinted.ToString()+"\r\n"
				+Lan.g(this,"E-mailed:")+" "+countEmailed.ToString()+"\r\n"
				+Lan.g(this,"SentElect:")+" "+countSentElect.ToString()+"\r\n"
				+Lan.g(this,"Texted:")+" "+countTexted.ToString();
			if(listDictionariesPatnumsSkipped.Count(x => x.Count>0)>0) {
				//Modify original box to have yes/no buttons to see if they want to see who errored out
				message+="\r\n\r\n"+"Would you like to see skipped patnums?";
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,message)) {
					string skippedPatNums="";
					foreach(Dictionary<long,string> dictionarySkipReasons in listDictionariesPatnumsSkipped) {
						foreach(KeyValuePair<long,string> kvp in dictionarySkipReasons) {
							skippedPatNums+=Lans.g(this,"PatNum:")+" "+kvp.Key.ToString()+" - "+kvp.Value.ToString()+"\r\n";
						}
					}
					MsgBoxCopyPaste msgBoxPatErrors=new MsgBoxCopyPaste(skippedPatNums);
					msgBoxPatErrors.Show();
				}
			}
			else {
				//If there were no errors, we simply show this.
				MsgBox.Show(this,message);
			}
			Cursor=Cursors.Default;
			_isPrinting=false;
			FillGrid();//not automatic
		}

		public void SendStatements(List<Dictionary<long,string>> listDictPatnumsSkipped,ref int emailed,ref int printed,ref int sentElect,
			ref PdfDocument outputDocument,ref int skippedDeleted,ref int texted) 
		{
			_progExtended=new ODProgressExtended(ODEventType.Billing,new BillingEvent(),this,tag: new ProgressBarHelper(("Billing Progress")
				,progressBarEventType:ProgBarEventType.Header));
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper("",progressBarEventType:ProgBarEventType.BringToFront)));
			Cursor=Cursors.WaitCursor;
			_isPrinting=true;
			//FormProgressExtended will insert new bars on top. Statment is on bottom, batch middle, and overall on top. 
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n0 / 0","0%",0,100
				,ProgBarStyle.Blocks,"3",isTopHidden:true)));
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n0 / 0","0%",0,1
				,ProgBarStyle.Blocks,"2",isTopHidden:true)));
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),"1%",0,gridBill.SelectedIndices.Length
				,ProgBarStyle.Blocks,"1",isTopHidden:true)));
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Progress Log"),progressBarEventType:ProgBarEventType.ProgressLog)));
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Preparing First Batch")+"..."
				,progressBarEventType:ProgBarEventType.TextMsg)));
			List<long> listStatementNums=gridBill.SelectedTags<DataRow>().Select(x => PIn.Long(x["StatementNum"].ToString())).ToList();
			List<Statement> listStatements=Statements.GetStatements(listStatementNums);
			Statement popUpCheck=listStatements.FirstOrDefault(x => x.Mode_==StatementMode.Electronic);
			//In case the user didn't come directly from FormBillingOptions check the DateRangeFrom on an electronic statement to see if we need to
			//display the warning message. Spot checking to save time. 
			if(popUpCheck!=null && (IsHistoryStartMinDate || popUpCheck.DateRangeFrom.Year<1880)) {
				if(!MsgBox.Show(MsgBoxButtons.YesNo,"Sending statements electronically for all account history could result in many pages. Continue?")) {
					return;
				}
				SecurityLogs.MakeLogEntry(Permissions.Billing,0,"User proceeded with electronic billing for all dates.");
			}
			if(!PrefC.HasClinicsEnabled) {
				//If clinics are enabled, the practice has the option to order statements alphabetically. This would have happened in 
				//Statements.GetStatements. For other practices, we are going to order the statements the way they are displayed in the grid.
				Dictionary<long,int> dictionaryStatementsOrder=new Dictionary<long, int>();
				for(int i=0;i<listStatementNums.Count;i++) {
					dictionaryStatementsOrder[listStatementNums[i]]=i;
				}
				listStatements=listStatements.OrderBy(x => dictionaryStatementsOrder[x.StatementNum]).ToList();
			}
			_dictionaryFamilies=Patients.GetFamilies(listStatements.Select(x => x.PatNum).ToList())
				.SelectMany(fam => fam.ListPats.Select(y => new { y.PatNum,fam }))
				.Distinct()
				.ToDictionary(x => x.PatNum,x => x.fam);
			AddInstallmentPlansToStatements(listStatements);
			//A dictionary of batches of statements.  The key is the batch num which is 1 based (helpful when displaying to the user).
			Dictionary<int,List<Statement>> dictionaryStatementsForSend=new Dictionary<int,List<Statement>>();
			List<long> listClinicNums=new List<long>();
			//If clinics are on, batch by clinics. Otherwise, batch by BillingElectBatchMax.
			if(PrefC.HasClinicsEnabled) {
				//Make each clinic one batch
				for(int i=0;i<listStatements.Count;i++) {
					//Find the clinicnum.
					long clinicNum=_dictionaryFamilies[listStatements[i].PatNum].Guarantor.ClinicNum;
					if(!listClinicNums.Contains(clinicNum)) {
						listClinicNums.Add(clinicNum);
					}
					int index=listClinicNums.IndexOf(clinicNum);
					if(!dictionaryStatementsForSend.TryGetValue(index,out List<Statement> listStatementsBatch)) {
						dictionaryStatementsForSend.Add(index,new List<Statement>());
					}
					dictionaryStatementsForSend[index].Add(listStatements[i]);
				}
			}
			else {
				int maxStmtsPerBatch=PrefC.GetInt(PrefName.BillingElectBatchMax);
				if(maxStmtsPerBatch==0 || PrefC.GetString(PrefName.BillingUseElectronic)=="2" || PrefC.GetString(PrefName.BillingUseElectronic)=="4") {//Max is disabled or Output to File billing option or using EDS.
					maxStmtsPerBatch=gridBill.SelectedIndices.Length;//Make the batch size equal to the list of statements so that we send them all at once.
				}
				int batchCount=-1; //-1 so we can start the dictionary at 0
				for(int i=0;i<listStatements.Count;i++) {
					if(i % maxStmtsPerBatch==0) {
						batchCount++;
						dictionaryStatementsForSend.Add(batchCount,new List<Statement>());
					}
					dictionaryStatementsForSend[batchCount].Add(listStatements[i]);
				}
			}
			int numOfBatchesSent=1;//Start at 1 so that it is better looking in the UI.
			int numOfBatchesTotal=dictionaryStatementsForSend.Count;
			int curStatementsProcessed=0;
			int numOfStatementsInBatch=0;
			string selectedFile=null;
			int curStatementIdx=0;//starting index to display on progress bar
			//TODO: Query the database to get an updated list of unsent bills and compare them to the local list to make sure that we do not resend statements that have already been sent by another user.
			while(numOfBatchesSent<=numOfBatchesTotal) {
				if(!BillingProgressPause()) {
					return;
				}
				curStatementsProcessed = 0;
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Preparing Batch")+" "+numOfBatchesSent
					,progressBarEventType:ProgBarEventType.TextMsg)));
				Dictionary<long,StatementData> dictionaryStatementData=new Dictionary<long,StatementData>();
				List<EbillStatement> listEbillStatements=new List<EbillStatement>();
				List<Statement> listStatementsToSend=dictionaryStatementsForSend[numOfBatchesSent-1]; //-1 because we start on +1 for the batch number
				numOfStatementsInBatch=listStatementsToSend.Count;
				long clinicNum=0;
				if(PrefC.HasClinicsEnabled) {
					clinicNum=listClinicNums[numOfBatchesSent-1]; //-1 because we start on +1 for the batch number
				}
				//Now to print, send eBills, and text messages.  If any return false, the user canceled during execution.
				if(!PrintBatch(numOfBatchesSent,numOfBatchesTotal,listDictPatnumsSkipped,ref emailed,ref printed,ref listEbillStatements
						,ref outputDocument,ref curStatementIdx,ref curStatementsProcessed,ref skippedDeleted,ref listStatementsToSend,ref dictionaryStatementData)
					|| !SendEBills(numOfBatchesSent,numOfBatchesTotal,numOfStatementsInBatch,clinicNum,selectedFile,listDictPatnumsSkipped,ref sentElect,ref listEbillStatements
						,ref curStatementIdx,ref curStatementsProcessed,ref skippedDeleted)
					|| !SendTextMessages(listDictPatnumsSkipped,ref listStatementsToSend,ref texted)) 
				{
					Statements.SyncStatementProdsForMultipleStatements(dictionaryStatementData);
					return;
				}
				Statements.SyncStatementProdsForMultipleStatements(dictionaryStatementData);
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch Completed")+"..."
					,progressBarEventType:ProgBarEventType.TextMsg)));
				numOfBatchesSent++;
			}//End while loop
		}

		///<summary>Concat all the pdf's together to create one print job.
		///Returns false if the printing was canceled</summary>
		public bool PrintBatch(int numOfBatchesSent,int numOfBatchesTotal,List<Dictionary<long,string>> listDictionariesPatnumsSkipped
			,ref int emailed,ref int printed,ref List<EbillStatement> listEbillStatements,ref PdfDocument outputDocument
			,ref int curStatementIdx,ref int curStatementsProcessed,ref int skippedDeleted,ref List<Statement> listStatements
			,ref Dictionary<long,StatementData> dictionaryStatementData) 
		{
			Random random;
			string fileName;
			string filePathAndName;
			string attachPath;
			EmailMessage message;
			EmailAttach emailAttach;
			EmailAddress emailAddress;
			Patient patient;
			string patFolder;
			PdfDocument pdfDocumentInput;
			PdfPage pdfPage;
			string savedPdfPath;
			DataSet dataSet;
			int numOfStatementsInBatch=listStatements.Count;
			int countPdfsPrinted = 0;
			bool isComputeAging=true;//will be false if AgingIsEnterprise and aging was calculated for today already (or successfully runs for today)
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				if(PrefC.GetDate(PrefName.DateLastAging).Date!=MiscData.GetNowDateTime().Date && !RunAgingEnterprise()) {//run aging for all patients
					return false;//if aging fails, don't generate and print statements
				}
				isComputeAging=false;
			}
			foreach(Statement statement in listStatements) {
				if(!BillingProgressPause()) {
					return false;
				}
				curStatementIdx++;
				if(statement==null) {//The statement was probably deleted by another user.
					skippedDeleted++;
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
					continue;
				}
				if(_listStatementNumsToSkip.Contains(statement.StatementNum)) {
					//The user never selected this statement, so we don't need to note why we skipped it.
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
					continue;
				}
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"5%",5,100,ProgBarStyle.Blocks,"3")));
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Generating Single PDFs")+"..."
					,progressBarEventType:ProgBarEventType.TextMsg)));
				Family family=null;
				if(!_dictionaryFamilies.TryGetValue(statement.PatNum,out family)) {
					family=Patients.GetFamily(statement.PatNum);
				}
				patient=family.GetPatient(statement.PatNum);
				patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				dataSet=AccountModules.GetStatementDataSet(statement,isComputeAging,doIncludePatLName:PrefC.IsODHQ);
				if(comboEmailFrom.SelectedIndex==0) { //clinic/practice default
					emailAddress=EmailAddresses.GetByClinic(patient.ClinicNum);
				}
				else { //me or static email address, email address for 'me' is the first one in _listEmailAddresses
					emailAddress=_listEmailAddresses[comboEmailFrom.SelectedIndex-1];//-1 to account for predefined "Clinic/Practice" item in combobox
				}
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"10%",10,100,ProgBarStyle.Blocks,"3")));
				if(statement.Mode_==StatementMode.Email) {
					if(emailAddress.SMTPserver=="") {
						_progExtended.Close();
						MsgBox.Show(this,"You need to enter an SMTP server name in e-mail setup before you can send e-mail.");
						Cursor=Cursors.Default;
						_isPrinting=false;
						//FillGrid();//automatic
						return false;
					}
					if(patient.Email=="") {
						listDictionariesPatnumsSkipped[0][patient.PatNum]=Lan.g(this,"Empty patient Email");
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						continue;
					}
				}
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"15%",15,100,ProgBarStyle.Blocks,"3")));
				statement.IsSent=true;
				statement.DateSent=DateTime.Today;
				#region Print PDFs
				string billingType = PrefC.GetString(PrefName.BillingUseElectronic);
				string tempPdfFile="";
				if(statement.Mode_==StatementMode.Electronic && (billingType=="1" || billingType=="3") && !PrefC.GetBool(PrefName.BillingElectCreatePDF)) {
					//Do not create a pdf
					//Detach the previously created document for the statement if one exists because it may not match what is sent to the patient,
					//and the Statement.DocNum will need to match the StatementProd.DocNum if late charges are going to be created.
					Statements.DetachDocFromStatements(statement.DocNum);
					//DocNum is set to zero for StatementProds when no pdf is created for electronic statements.
					dictionaryStatementData.Add(statement.StatementNum,new StatementData(dataSet,0));
				}
				else {
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
					try {
						tempPdfFile=FormRpStatement.CreateStatementPdfSheets(statement,patient,family,dataSet,true);
						//The above methods return an empty string if they were unable to create a PDF.
						if(tempPdfFile=="") {
							throw new Exception("Error creating statement PDF");
						}
					}
					catch(Exception ex) {
						listDictionariesPatnumsSkipped[2][patient.PatNum]=Lan.g(this,"Error creating PDF")+": "+ex.ToString();
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),
							Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,
							ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",
							curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						continue;
					}
					countPdfsPrinted++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"PDF Created")+"..."
						,progressBarEventType:ProgBarEventType.TextMsg)));
					if(statement.DocNum==0) {
						_progExtended.Close();
						MsgBox.Show(this,"Failed to save PDF.  In Setup, DataPaths, please make sure the top radio button is checked.");
						Cursor=Cursors.Default;
						_isPrinting=false;
						return false;
					}
					dictionaryStatementData.Add(statement.StatementNum,new StatementData(dataSet,statement.DocNum));
				}
				//imageStore = OpenDental.Imaging.ImageStore.GetImageStore(pat);
				//If stmt.DocNum==0, savedPdfPath will be "".  A blank savedPdfPath is fine for electronic statements.
				Document documentStatement=Documents.GetByNum(statement.DocNum);
				if(CloudStorage.IsCloudStorage) {
					if(tempPdfFile != "")
						savedPdfPath=tempPdfFile;//To save time by not having to download it.
					else {
						savedPdfPath=PrefC.GetRandomTempFile("pdf");
						FileAtoZ.Copy(ImageStore.GetFilePath(documentStatement,patFolder),savedPdfPath,FileAtoZSourceDestination.AtoZToLocal,uploadMessage:"Downloading statement...");
					}
				}
				else {
					savedPdfPath=ImageStore.GetFilePath(documentStatement,patFolder);//savedPdfPath is just the filename when using DataStorageType.InDatabase
				}
				if(statement.Mode_==StatementMode.InPerson || statement.Mode_==StatementMode.Mail) {
					_hasToShowPdf=true;
					if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
						byte[] rawData=Convert.FromBase64String(documentStatement.RawBase64);
						using(Stream stream=new MemoryStream(rawData)) {
							pdfDocumentInput=PdfReader.Open(stream,PdfDocumentOpenMode.Import);
							stream.Close();
						}
					}
					else {
						pdfDocumentInput=PdfReader.Open(savedPdfPath,PdfDocumentOpenMode.Import);
					}
					for(int idx = 0;idx<pdfDocumentInput.PageCount;idx++) {
						pdfPage=pdfDocumentInput.Pages[idx];
						outputDocument.AddPage(pdfPage);
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,(((idx/pdfDocumentInput.PageCount)*85)+15)+"%",((idx/pdfDocumentInput.PageCount)*85)+15,100,ProgBarStyle.Blocks,"3")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"PDF Added to Print List")+"..."
							,progressBarEventType:ProgBarEventType.TextMsg)));
					}
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
					printed++;
					labelPrinted.Text=Lan.g(this,"Printed=")+printed.ToString();
					Application.DoEvents();
					_listStatementNumsSent.Add(statement.StatementNum);
					Statements.MarkSent(statement.StatementNum,statement.DateSent);
				}
				#endregion
				#region Preparing Email
				if(statement.Mode_==StatementMode.Email) {
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Preparing Email")+"..."
						,progressBarEventType:ProgBarEventType.TextMsg)));
					attachPath=EmailAttaches.GetAttachPath();
					random=new Random();
					fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+random.Next(1000).ToString()+".pdf";
					filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
					if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
						ImageStore.Export(filePathAndName,documentStatement,patient);
					}
					else {
						FileAtoZ.Copy(savedPdfPath,filePathAndName,FileAtoZSourceDestination.LocalToAtoZ,uploadMessage:"Uploading statement...");
					}
					//Process.Start(filePathAndName);
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"40%",40,100,ProgBarStyle.Blocks,"3")));
					message=Statements.GetEmailMessageForStatement(statement,patient);
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"70%",70,100,ProgBarStyle.Blocks,"3")));
					emailAttach=new EmailAttach();
					emailAttach.DisplayedFileName="Statement.pdf";
					emailAttach.ActualFileName=fileName;
					message.Attachments.Add(emailAttach);
					message.SentOrReceived=EmailSentOrReceived.Sent;
					message.MsgDateTime=DateTime.Now;
					try {
						//If IsCloudStorage==true, then we will end up downloading the file again in EmailMessages.SendEmailUnsecure.
						EmailMessages.SendEmail(message,emailAddress);
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"90%",90,100,ProgBarStyle.Blocks,"3")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"95%",95,100,ProgBarStyle.Blocks,"3")));
						emailed++;
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Email Sent")+"...",
							progressBarEventType:ProgBarEventType.TextMsg)));
						labelEmailed.Text=Lan.g(this,"E-mailed=")+emailed.ToString();
						Application.DoEvents();
					}
					catch(Exception ex) {
						listDictionariesPatnumsSkipped[2][patient.PatNum]=Lan.g(this,"Error sending email")+": "+ex.ToString();
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
						continue;
					}
					_listStatementNumsSent.Add(statement.StatementNum);
					Statements.MarkSent(statement.StatementNum,statement.DateSent);
					try {
						File.Delete(tempPdfFile);
					}
					catch(Exception ex) {
						ex.DoNothing();//Will most likely get cleaned up when the user closes OD.
					}
				}
				#endregion
				#region Preparing E-Bills
				if(statement.Mode_==StatementMode.Electronic) {
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"65%",65,100,ProgBarStyle.Blocks,"3")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Preparing E-Bills")+"...",
						progressBarEventType:ProgBarEventType.TextMsg)));
					Patient guarantor = family.ListPats[0];
					if(guarantor.Address.Trim()=="" || guarantor.City.Trim()=="" || guarantor.State.Trim()=="" || guarantor.Zip.Trim()=="") {
						listDictionariesPatnumsSkipped[1][patient.PatNum]=Lan.g(this,"Error with patient address");
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						continue;
					}
					//Eventually will not use Statement.IsRecipt or Statement.IsInvoice but rather StmtType.Invoice and StmtType.Receipt.
					if(statement.StatementType==StmtType.LimitedStatement || statement.IsReceipt || statement.IsInvoice) {						
						listDictionariesPatnumsSkipped[2][patient.PatNum]=Lan.g(this,"Limited statements, Receipts, and Invoices cannot be sent electronically.");//Misc errors
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),
							Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,
							gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",
							curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						continue;
					}
					EbillStatement ebillStatement = new EbillStatement();
					ebillStatement.family=family;
					ebillStatement.statement=statement;
					long clinicNum = 0;//If clinics are disabled, then all bills will go into the same "bucket"
					if(PrefC.HasClinicsEnabled) {
						clinicNum=family.Guarantor.ClinicNum;
					}
					List<string> listElectErrors = new List<string>();
					if(PrefC.GetString(PrefName.BillingUseElectronic)=="1") {//EHG
						listElectErrors=Bridges.EHG_statements.Validate(clinicNum);
					}
					if(listElectErrors.Count > 0) {
						for(int i = 0;i<listElectErrors.Count;++i) {
								listDictionariesPatnumsSkipped[2][patient.PatNum]=listElectErrors[i];
						}
						curStatementsProcessed++;
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
						_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
						continue;//skip the current statement, since there are errors.
					}
					listEbillStatements.Add(ebillStatement);
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"70%",70,100,ProgBarStyle.Blocks,"3")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"E-Bill Added To Send List")+"..."
						,progressBarEventType:ProgBarEventType.TextMsg)));
				}
				#endregion
			}
			return true;
		}

		///<summary>Attempt to send electronic bills if needed.
		///Returns false if the sending was canceled</summary>
		private bool SendEBills(int numOfBatchesSent,int numOfBatchesTotal,int numOfStatementsInBatch,long clinicNum,string selectedFile,
			List<Dictionary<long,string>> listDictionariesPatnumsSkipped,ref int sentElect,ref List<EbillStatement> listEbillStatements,ref int curStatementIdx,
			ref int curStatementsProcessed,ref int skippedDeleted)
		{
			//Attempt to send electronic bills if needed------------------------------------------------------------
			Family family;
			Patient patient;
			DataSet dataSet;
			bool isComputeAging=true;//will be false if AgingIsEnterprise and aging was calculated for today already (or successfully runs for today)
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				if(PrefC.GetDate(PrefName.DateLastAging).Date!=MiscData.GetNowDateTime().Date && !RunAgingEnterprise()) {//run aging for all patients
					return false;//if aging fails, don't generate and print statements
				}
				isComputeAging=false;
			}
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"80%",80,100,ProgBarStyle.Blocks,"3")));
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Sending E-Bills")+"..."
				,progressBarEventType:ProgBarEventType.TextMsg)));
			if(!BillingProgressPause()) {
				return false;
			}
			if(listEbillStatements.Count==0) {//All statements have been sent for the current clinic.  Nothing more to do.
				return true;
			}
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration=true;
			xmlWriterSettings.Encoding=Encoding.UTF8;
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars="   ";
			StringBuilder strBuildElect = new StringBuilder();
			XmlWriter xmlWriterElect = XmlWriter.Create(strBuildElect,xmlWriterSettings);
			List<long> listElectStmtNums = new List<long>();
			if(PrefC.GetString(PrefName.BillingUseElectronic)=="1") {
				Bridges.EHG_statements.GeneratePracticeInfo(xmlWriterElect,clinicNum);
			}
			else if(PrefC.GetString(PrefName.BillingUseElectronic)=="2") {
				Bridges.POS_statements.GeneratePracticeInfo(xmlWriterElect,clinicNum);
			}
			else if(PrefC.GetString(PrefName.BillingUseElectronic)=="3") {
				Bridges.ClaimX_Statements.GeneratePracticeInfo(xmlWriterElect,clinicNum);
			}
			else if(PrefC.GetString(PrefName.BillingUseElectronic)=="4") {
				Bridges.EDS_Statements.GeneratePracticeInfo(xmlWriterElect,clinicNum);
			}
			int statementCountCur = 0;
			//Generate the statements for each batch.
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"85%",85,100,ProgBarStyle.Blocks,"3")));
			for(int j = listEbillStatements.Count-1;j>=0;j--) {//Construct the string for sending this clinic's ebills
				if(!BillingProgressPause()) {
					return false;
				}
				Statement statementCur = listEbillStatements[j].statement;
				if(statementCur==null) {//The statement was probably deleted by another user.
					skippedDeleted++;
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
					continue;
				}
				if(_listStatementNumsToSkip.Contains(statementCur.StatementNum)) {
					//The user never selected this statement, so we don't need to note why we skipped it.
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
					continue;
				}
				family=listEbillStatements[j].family;
				listEbillStatements.RemoveAt(j);//Remove the statement from our list so that we do not send it again in the next batch.
				patient=family.GetPatient(statementCur.PatNum);
				dataSet=AccountModules.GetStatementDataSet(statementCur,isComputeAging,doIncludePatLName:PrefC.IsODHQ);
				bool statementWritten = true;
				try {
					//Write the statement into a temporary string builder, so that if the statement fails to generate (due to exception),
					//then the partially generated statement will not be added to the strBuildElect.
					StringBuilder strBuildStatement = new StringBuilder();
					using(XmlWriter xmlWriterStatement = XmlWriter.Create(strBuildStatement,xmlWriterElect.Settings)) {
						if(PrefC.GetString(PrefName.BillingUseElectronic)=="0") {
							throw new Exception(Lan.g(this,"\'No billing electronic\' is currently selected in Billing Defaults."));
						}
						else if(PrefC.GetString(PrefName.BillingUseElectronic)=="1") {
							OpenDental.Bridges.EHG_statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
						else if(PrefC.GetString(PrefName.BillingUseElectronic)=="2") {
							OpenDental.Bridges.POS_statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
						else if(PrefC.GetString(PrefName.BillingUseElectronic)=="3") {
							OpenDental.Bridges.ClaimX_Statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
						else if(PrefC.GetString(PrefName.BillingUseElectronic)=="4") {
							Bridges.EDS_Statements.GenerateOneStatement(xmlWriterStatement,statementCur,patient,family,dataSet);
						}
					}
					//Write this statement's XML to the XML document with all the statements.
					using(XmlReader readerStatement = XmlReader.Create(new StringReader(strBuildStatement.ToString()))) {
						xmlWriterElect.WriteNode(readerStatement,true);
					}
				}
				catch(Exception ex) {
					listDictionariesPatnumsSkipped[2][patient.PatNum]=Lan.g(this,"Error sending statement")+": "+ex.ToString();
					statementWritten=false;
				}
				if(statementWritten) {
					listElectStmtNums.Add(statementCur.StatementNum);
					sentElect++;
					statementCountCur++;
				}
			}
			if(statementCountCur==0) {//All statements for this batch were either deleted or had an exception thrown while generating.
				return true;//Go on to next batch
			}
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"90%",90,100,ProgBarStyle.Blocks,"3")));
			if(PrefC.GetString(PrefName.BillingUseElectronic)=="1") {
				xmlWriterElect.Close();
				for(int attempts = 0;attempts<3;attempts++) {
					try {
						Bridges.EHG_statements.Send(strBuildElect.ToString(),clinicNum);
						//loop through all statements in the batch and mark them sent
						for(int i = 0;i<listElectStmtNums.Count;i++) {
							_listStatementNumsSent.Add(listElectStmtNums[i]);
							Statements.MarkSent(listElectStmtNums[i],DateTime.Today);
							curStatementsProcessed++;
							_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
							_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
							_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
							+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
					}
					break;//At this point the batch was successfully sent so there is no need to loop through additional attempts.
				}
				catch(Exception ex) {
					if(attempts<2) {//Don't indicate the error unless it failed on the last attempt.
							continue;//The only thing skipped besides the error message is evaluating if the statement was written, which is wasn't.
						}
						sentElect-=listElectStmtNums.Count;
						if(!listDictionariesPatnumsSkipped[2].ContainsKey(0)) {
							listDictionariesPatnumsSkipped[2][0]="";
						}				
						if(ex.Message.Contains("(404) Not Found")) {//Custom 404 error message
							listDictionariesPatnumsSkipped[2][0]+=Lan.g(this,"The connection to the server could not be established or was lost, or the upload timed out.  "
							+"Ensure your internet connection is working and that your firewall is not blocking this application.  "
							+"If the upload timed out after 10 minutes, try sending 25 statements or less in each batch to reduce upload time.");
						}
						else {//Document any other errors to make troubleshooting much easier.
							listDictionariesPatnumsSkipped[2][0]+=Lan.g(this,ex.Message);
						}
					}
					//This occurs so we can count unsent bills in the overall.
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
				}
			}
			if(PrefC.GetString(PrefName.BillingUseElectronic)=="2") {
				xmlWriterElect.Close();
				string filePath=PrefC.GetString(PrefName.BillingElectStmtOutputPathPos);
				if(Directory.Exists(filePath)) {
					filePath=ODFileUtils.CombinePaths(filePath,"Statements.xml");
				}
				else if(!String.IsNullOrEmpty(selectedFile)) {//Default Output path not set, however, User already chose a filepath to output to on the previous pass.
					filePath=selectedFile;
				}
				else if(selectedFile=="") {//User canceled when prompted for output path, since preference path is invalid.  Skip all batches.
					sentElect-=listElectStmtNums.Count;
					return true;//Go on to next batch
				}
				else {
					//Only bring up save dialog if path is invalid
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.FileName="Statements.xml";
					saveFileDialog.CheckPathExists=true;
					if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
						sentElect-=listElectStmtNums.Count;
						selectedFile="";//To remember that the user canceled the first time through.  User only needs to cancel once to cancel each batch.
						return true;//Go on to next batch
					}
					filePath=saveFileDialog.FileName;
					selectedFile=filePath;
				}
				filePath=GetEbillFilePathForClinic(filePath,clinicNum);
				File.WriteAllText(filePath,strBuildElect.ToString());
				for(int i = 0;i<listElectStmtNums.Count;i++) {
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"10%",10,100,ProgBarStyle.Blocks,"3")));
					_listStatementNumsSent.Add(listElectStmtNums[i]);
					Statements.MarkSent(listElectStmtNums[i],DateTime.Today);
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed /numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
				}
			}
			if(PrefC.GetString(PrefName.BillingUseElectronic)=="3") {
				xmlWriterElect.Close();
				if(!String.IsNullOrEmpty(selectedFile)) {//User already chose a filepath to output to on the previous pass.
					//We will reuse the selectedPath below.
				}
				else if(selectedFile=="") {//User canceled when prompted for output path.  Skip all batches.
					sentElect-=listElectStmtNums.Count;
					return true;//Go on to next batch
				}
				else {//User has not been prompted yet.
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.InitialDirectory=@"C:\StatementX\";//Clint from ExtraDent requested this default path.
					if(!Directory.Exists(saveFileDialog.InitialDirectory)) {
						try {
							Directory.CreateDirectory(saveFileDialog.InitialDirectory);
						}
						catch { }
					}
					saveFileDialog.FileName="Statements.xml";
					if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
						sentElect-=listElectStmtNums.Count;
						selectedFile="";//To remember that the user canceled the first time through.  User only needs to cancel once to cancel each batch.
						return true;//Go on to next batch
					}
					selectedFile=saveFileDialog.FileName;
				}
				string filePath=GetEbillFilePathForClinic(selectedFile,clinicNum);
				File.WriteAllText(filePath,strBuildElect.ToString());
				for(int i = 0;i<listElectStmtNums.Count;i++) {
					_listStatementNumsSent.Add(listElectStmtNums[i]);
					Statements.MarkSent(listElectStmtNums[i],DateTime.Today);
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
				}
			}
			if(PrefC.GetString(PrefName.BillingUseElectronic)=="4") {
				xmlWriterElect.Close();
				string filePath=PrefC.GetString(PrefName.BillingElectStmtOutputPathEds);
				if(Directory.Exists(filePath)) {
					filePath=ODFileUtils.CombinePaths(filePath,"Statements.xml");
				}
				else if(!String.IsNullOrEmpty(selectedFile)) {//Default Output path not set, however, User already chose a filepath to output to on the previous pass.
					filePath=selectedFile;
				}
				else if(selectedFile=="") {//User canceled when prompted for output path, since preference path is invalid.  Skip all batches.
					sentElect-=listElectStmtNums.Count;
					return true;//Go on to next batch
				}
				else {
					using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lan.g(this,"Billing Defaults Output Path value is invalid.\r\nPlease specify the path to save the file to."));
					msgBoxCopyPaste.ShowDialog();
					SaveFileDialog saveFileDialog=new SaveFileDialog();
					saveFileDialog.FileName="Statements.xml";
					saveFileDialog.CheckPathExists=true;
					if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
						sentElect-=listElectStmtNums.Count;
						selectedFile="";//To remember that the user canceled the first time through.  User only needs to cancel once to cancel each batch.
						return true;//Go on to next batch
					}
					filePath=saveFileDialog.FileName;
					selectedFile=filePath;
				}
				filePath=GetEbillFilePathForClinic(filePath,clinicNum);
				File.WriteAllText(filePath,strBuildElect.ToString());
				for(int i=0;i<listElectStmtNums.Count;i++) {
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"10%",10,100,ProgBarStyle.Blocks,"3")));
					_listStatementNumsSent.Add(listElectStmtNums[i]);
					Statements.MarkSent(listElectStmtNums[i],DateTime.Today);
					curStatementsProcessed++;
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Statement")+"\r\n"+curStatementIdx+" / "+gridBill.SelectedIndices.Length,"100%",100,100,ProgBarStyle.Blocks,"3")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Overall"),Math.Ceiling(((double)curStatementIdx/gridBill.SelectedIndices.Length)*100)+"%",curStatementIdx,gridBill.SelectedIndices.Length,ProgBarStyle.Blocks,"1")));
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Batch")+"\r\n"+numOfBatchesSent+" / "
						+numOfBatchesTotal,Math.Ceiling(((double)curStatementsProcessed/numOfStatementsInBatch)*100)+"%",curStatementsProcessed,numOfStatementsInBatch,ProgBarStyle.Blocks,"2")));
				}
			}
			_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"E-Bills Sent")+"..."
				,progressBarEventType:ProgBarEventType.TextMsg)));
			labelSentElect.Text=Lan.g(this,"SentElect=")+sentElect.ToString();
			Application.DoEvents();
			return true;
		}

		///<summary>The filePath is the full path to the output file if the clinics feature is disabled (for a single location practice).</summary>
		private string GetEbillFilePathForClinic(string filePath,long clinicNum) {
			if(!PrefC.HasClinicsEnabled) {
				return filePath;
			}
			string clinicAbbr;
			//Check for zero clinic
			if(clinicNum==0) {
				clinicAbbr=Lan.g(this,"Unassigned");
			}
			else {
				clinicAbbr=Clinics.GetClinic(clinicNum).Abbr;//Abbr is required by our interface, so no need to check if blank.
			}
			string fileName=Path.GetFileNameWithoutExtension(filePath)+'-'+clinicAbbr+Path.GetExtension(filePath);
			return ODFileUtils.CombinePaths(Path.GetDirectoryName(filePath),fileName);
		}

		///<summary>Sends text messages to the current batch of statements.</summary>
		private bool SendTextMessages(List<Dictionary<long,string>> listDictionariesPatnumsSkipped,ref List<Statement> listStatements,ref int texted) {
			List<SmsToMobile> listTextsToSend=new List<SmsToMobile>();
			//Tuple<GuidMessage,StatementNum>
			List<Tuple<string,long>> listStmtNumsToUpdate=new List<Tuple<string,long>>();
			Dictionary<long,PatComm> dictionaryPatComms=Patients.GetPatComms(_dictionaryFamilies.Values.SelectMany(x => x.ListPats).DistinctBy(x => x.PatNum).ToList())
				.ToDictionary(x => x.PatNum,y => y);
			string guidBatch=null;
			foreach(Statement statement in listStatements) {
				if(!BillingProgressPause()) {
					return false;
				}
				if(statement.SmsSendStatus!=AutoCommStatus.SendNotAttempted) {
					continue;
				}
				PatComm patComm;
				if(!dictionaryPatComms.TryGetValue(statement.PatNum,out patComm)) {
					listDictionariesPatnumsSkipped[2][statement.PatNum]=Lan.g(this,"Unable to find patient communication method");
					continue;
				}
				if(!patComm.IsSmsAnOption) {
					continue;
				}
				if(patComm.CommOptOut.IsOptedOut(CommOptOutMode.Text,CommOptOutType.Statements)) {
					listDictionariesPatnumsSkipped[2][statement.PatNum]=Lan.g(this,"Patient is opted out of automated messaging.");
					continue;
				}
				Family family;
				if(!_dictionaryFamilies.TryGetValue(statement.PatNum,out family)) {
					family=Patients.GetFamily(statement.PatNum);
				}
				Patient patient=family.GetPatient(statement.PatNum);
				SmsToMobile textToSend=new SmsToMobile {
					ClinicNum=patient.ClinicNum,
					GuidMessage=Guid.NewGuid().ToString(),
					IsTimeSensitive=false,
					MobilePhoneNumber=patComm.SmsPhone,
					PatNum=statement.PatNum,
					MsgText=Statements.ReplaceVarsForSms(PrefC.GetString(PrefName.BillingDefaultsSmsTemplate),patient,statement),
					MsgType=SmsMessageSource.Statements,
				};
				guidBatch=guidBatch??textToSend.GuidMessage;
				textToSend.GuidBatch=guidBatch;
				listTextsToSend.Add(textToSend);
				listStmtNumsToUpdate.Add(Tuple.Create(textToSend.GuidMessage,statement.StatementNum));
			}
			if(!BillingProgressPause()) {
				return false;
			}
			if(listTextsToSend.Count==0) {
				return true;
			}
			try {
				_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Sending text messages")+"..."
					,progressBarEventType: ProgBarEventType.TextMsg)));
				List<SmsToMobile> listSmsToMobiles=SmsToMobiles.SendSmsMany(listTextsToSend,user:Security.CurUser);
				//listDictPatnumsSkipped[2] tracks errors for patients skipped due to Misc reasons.
				HandleSmsSent(listSmsToMobiles,listStmtNumsToUpdate,listDictionariesPatnumsSkipped[2]);
				texted+=listSmsToMobiles.Where(x => x.SmsStatus!=SmsDeliveryStatus.FailNoCharge).Count();
				labelTexted.Text=Lan.g(this,"Texted=")+texted;
				Application.DoEvents();
			}
			catch(Exception ex) {
				if(!listDictionariesPatnumsSkipped[2].ContainsKey(0)) {
					listDictionariesPatnumsSkipped[2][0]="";
				}
				listDictionariesPatnumsSkipped[2][0]+=Lan.g(this,"Error Sending text messages")+": "+ex.ToString();
				Statements.UpdateSmsSendStatus(listStmtNumsToUpdate.Select(x => x.Item2).ToList(),AutoCommStatus.SendFailed);
			}
			return true;
		}

		private void HandleSmsSent(List<SmsToMobile> listSmsToMobiles,List<Tuple<string,long>> listStmtNumsToUpdate
			,Dictionary<long,string> dictionaryPatnumsSkippedMisc) 
		{
			List<long> listSuccessStatementNums=listSmsToMobiles
				//SmsToMobile that were queued successfully
				.Where(x => x.SmsStatus!=SmsDeliveryStatus.FailNoCharge)
				//That correspond to a statement
				.Select(x => listStmtNumsToUpdate.FirstOrDefault(y => y.Item1==x.GuidMessage)?.Item2??0)
				.Where(x => x!=0)
				.ToList();
			Statements.UpdateSmsSendStatus(listSuccessStatementNums,AutoCommStatus.SendSuccessful);
			List<long> listFailureStatementNums=listSmsToMobiles
				//SmsToMobile that were not queued successfully
				.Where(x => x.SmsStatus==SmsDeliveryStatus.FailNoCharge)
				//That correspond to a statement
				.Select(x => listStmtNumsToUpdate.FirstOrDefault(y => y.Item1==x.GuidMessage)?.Item2??0)
				.Where(x => x!=0)
				.ToList();
			Statements.UpdateSmsSendStatus(listFailureStatementNums,AutoCommStatus.SendFailed);
			//listDictPatnumsSkipped[2] is for displaying miscellaneous (in this case, sms related) errors to user.
			List<SmsToMobile> listSmsToMobileFails = listSmsToMobiles.Where(x => x.SmsStatus==SmsDeliveryStatus.FailNoCharge).ToList();
			for(int i = 0;i<listSmsToMobileFails.Count;++i) { 
				if(!dictionaryPatnumsSkippedMisc.ContainsKey(listSmsToMobileFails[i].PatNum)) {
					dictionaryPatnumsSkippedMisc[listSmsToMobileFails[i].PatNum]="";
				}
				dictionaryPatnumsSkippedMisc[listSmsToMobileFails[i].PatNum]+=Lan.g(this,"Error Sending text messages")+": "+listSmsToMobileFails[i].CustErrorText;
			}
		}

		///<summary>Sets the installment plans field on each of the statements passed in.</summary>
		private void AddInstallmentPlansToStatements(List<Statement> listStatements) {
			Dictionary<long,List<InstallmentPlan>> dictionarySuperFamInstallmentPlans=InstallmentPlans.GetForSuperFams(
				listStatements.Where(x => x.SuperFamily > 0)
					.Select(x => _dictionaryFamilies[x.PatNum].Guarantor.SuperFamily).ToList());
			Dictionary<long,InstallmentPlan> dictionaryFamInstallmentPlans=InstallmentPlans.GetForFams(
				listStatements.Where(x => x.SuperFamily==0)
					.Select(x => _dictionaryFamilies[x.PatNum].Guarantor.PatNum).ToList());
			for(int i = 0;i<listStatements.Count;++i) {
				if(listStatements[i].SuperFamily > 0) {
					if(!dictionarySuperFamInstallmentPlans.TryGetValue(_dictionaryFamilies[listStatements[i].PatNum].Guarantor.SuperFamily,out listStatements[i].ListInstallmentPlans)) {
							listStatements[i].ListInstallmentPlans=new List<InstallmentPlan>();
					}
				}
				else if(dictionaryFamInstallmentPlans.ContainsKey(_dictionaryFamilies[listStatements[i].PatNum].Guarantor.PatNum)) {
						listStatements[i].ListInstallmentPlans=new List<InstallmentPlan> { dictionaryFamInstallmentPlans[_dictionaryFamilies[listStatements[i].PatNum].Guarantor.PatNum] };
				}
				else {
						listStatements[i].ListInstallmentPlans=new List<InstallmentPlan>();
				}
			}
		}

		private bool RunAgingEnterprise() {
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			DateTime dateTimeToday=dateTimeNow.Date;
			DateTime dateTimeLastAging=PrefC.GetDate(PrefName.DateLastAging);
			if(dateTimeLastAging.Date==dateTimeToday) {
				return true;//already ran aging for this date, just move on
			}
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				MessageBox.Show(this,Lan.g(this,"In order to print or send statments, aging must be re-calculated, but you cannot run aging until it has "
					+"finished the current calculations which began on")+" "+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current "
					+"aging process has finished, a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Miscellaneous "
					+"and pressing the 'Clear' button."));
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - Billing");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dateTimeNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				Ledgers.ComputeAging(0,dateTimeToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dateTimeToday,false));
			};
			progressOD.StartingMessage=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dateTimeToday.ToShortDateString()+"...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				Ledgers.AgingExceptionHandler(ex,this);
			}
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			Signalods.SetInvalid(InvalidType.Prefs);
			if(progressOD.IsCancelled){
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - Billing");
			return progressOD.IsSuccess;
		}

		///<summary>Returns true unless the user clicks cancel in the progress window or the list has changed.  The method will wait infinitely if paused from the progress window.</summary>
		public bool BillingProgressPause() {
			//Check to see if the user wants to pause the sending of statements.  If so, wait until they decide to resume.
			List<long> listStatementNumsUnsent=new List<long>();
			_listStatementNumsToSkip=new List<long>();
			bool hasEventFired=false;
			DateTime dateFrom=PIn.Date(textDateStart.Text);
			DateTime dateTo=new DateTime(2200,1,1);
			List<long> clinicNums=new List<long>();//an empty list indicates to Statements.GetBilling to run for all clinics
			while(_progExtended.IsPaused) {
				if(!hasEventFired) {//Don't fire this event more than once.
					_progExtended.Fire(new ODEventArgs(ODEventType.Billing,new ProgressBarHelper(Lan.g(this,"Warning")
						,progressBarEventType:ProgBarEventType.WarningOff)));
					hasEventFired=true;
				}
				Thread.Sleep(100);
				if(!_progExtended.IsPaused) {
					List<long> listStatementNumsSelected=gridBill.ListGridRows.OfType<GridRow>()
						.Select(x =>PIn.Long(((DataRow)x.Tag)["StatementNum"].ToString())).ToList();
					listStatementNumsUnsent=listStatementNumsSelected.FindAll(x => !_listStatementNumsSent.Contains(x)).ToList();
					dateFrom=PIn.Date(textDateStart.Text);
					dateTo=new DateTime(2200,1,1);
					if(textDateEnd.Text!=""){
						dateTo=PIn.Date(textDateEnd.Text);
					}
					if(PrefC.HasClinicsEnabled && comboClinic.SelectedClinicNum>0) {
						clinicNums.Add(comboClinic.SelectedClinicNum);
					}
					DataTable table=Statements.GetBilling(radioSent.Checked,comboOrder.SelectedIndex,dateFrom,dateTo,clinicNums);
					List<long> listStatementNums=table.Select().Select(x => PIn.Long(x["StatementNum"].ToString())).ToList();
					for(int i = 0;i<listStatementNumsUnsent.Count;++i) {
						if(!listStatementNums.Contains(listStatementNumsUnsent[i])) {
							_listStatementNumsToSkip.Add(listStatementNumsUnsent[i]);
						}
					}
				}
				if(_progExtended.IsCanceled) {
					return false;
				}
			}
			//Check to see if the user wants to stop sending statements.
			if(_progExtended.IsCanceled) {
				return false;
			}
			return true;
		}

		private void FormBilling_CloseXClicked(object sender,CancelEventArgs e) {
			//butClose_Click
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(gridBill.ListGridRows.Count==0){
				Close();
			}
			_isActivateFillDisabled=true;
			DialogResult result=MessageBox.Show(Lan.g(this,"You may leave this window open while you work.  If you do close it, do you want to delete all unsent bills?"),
				"",MessageBoxButtons.YesNoCancel);
			if(result==DialogResult.No){
				Close();
				return;
			}
			else if(result==DialogResult.Cancel){
				_isActivateFillDisabled=false;
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
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				foreach(long clinicNum in dictionaryClinicStatmentsToDelete.Keys) {
					ProgressBarEvent.Fire(ODEventType.ProgressBar,
						Lan.g(this,"Deleting")+" "+dictionaryClinicStatmentsToDelete[clinicNum].Count+" "+Lan.g(this,"statements.")+"\r\n"
						+Lan.g(this,"Processed")+" "+runningTotal+" "+Lan.g(this,"out of")+" "+totalCount);
					Statements.DeleteAll(dictionaryClinicStatmentsToDelete[clinicNum]);
					runningTotal+=dictionaryClinicStatmentsToDelete[clinicNum].Count;
				}
				//This is not an accurate permission type.
				SecurityLogs.MakeLogEntry(Permissions.Accounting,0,"Billing: Unsent statements were deleted.");
			};
			progressOD.StartingMessage="Deleting Statements...";
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				FriendlyException.Show(Lan.g(this,"Error deleting statements."),ex);
				return;
			}
			if(dictionaryClinicStatmentsToDelete!=null) {//No error. (jordan I don't understand this line)
				MessageBox.Show(Lan.g(this,"Unsent statements deleted: ")+totalCount);
			}
			if(progressOD.IsCancelled){
				return;
			}
			Close();
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

	public struct EbillStatement {

		public Statement statement;
		public Family family;

	}

}

















