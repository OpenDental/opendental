//using Excel;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Threading;
using OpenDentBusiness;
using OpenDental.Thinfinity;
using CodeBase;
using DataConnectionBase;

namespace OpenDental{
	public partial class FormQuery : FormODBase {
		#region Fields
		///<summary>Set to true if this form should instantly launch a print preview instead of showing the actual "user query" form.</summary>
		public bool IsReport;
		private System.Drawing.Font bodyFont = new System.Drawing.Font("Arial", 8);
		private System.Drawing.Font colCaptFont=new System.Drawing.Font("Arial",8,FontStyle.Bold);
		private System.Drawing.Font subtitleFont=new System.Drawing.Font("Arial",8,FontStyle.Bold);
		private System.Drawing.Font titleFont = new System.Drawing.Font("Arial",14,FontStyle.Bold);
		///<summary>The location of the current page being printed, used to increment through the pages.</summary>
		private int _currentPage {
			get {
				return _pagesPrinted+1;
			}
		}
		///<summary>A dictionary of patient names that only gets filled once and only if necessary.</summary>
		private static Dictionary<long,string> _dictPatientNames;
		///<summary>A hashtable of key: InsPlanNum, value: Carrier Name.</summary>
		private static Hashtable _hashListPlans;
		private bool _headerPrinted;
		private int _linesPrinted;
		private int _pagesPrinted;
		private bool _totalsPrinted;
		///<summary>Tells the form whether to throw, show (interrupt), or suppress exceptions. Set manually in the code below.</summary>
		private QueryExceptionState _queryExceptionStateCur;
		private ReportSimpleGrid _reportSimpleGrid=new ReportSimpleGrid();
		///<summary>The server thread id that the current query is running on.</summary>
		private int _serverThreadID;
		private bool _submitOnLoad;
		private bool _summaryPrinted;
		///<summary>_reportSumpleGrid.TableQ should ALWAYS store the raw table, and _tableHuman will ALWAYS store the human-readable table.
		///This table only gets filled if radioHuman is clicked or checked when run. This way, when switching between the raw and human-readable formats, 
		///we merely have to set the data binding for the grid to their respective data tables instead of rerunning the entire query again.</summary>
		private DataTable _tableHuman;
		private bool _tablePrinted;
		///<summary>User queries run on a separate thread so they can be cancelled.</summary>
		private ODThread _threadQuery;
		private int _totalPages=0;
		private UserQuery _userQueryCur;
		#endregion Fields

		#region Constructor
		///<summary>Can pass in null if not a report.</summary>
		public FormQuery(ReportSimpleGrid report, bool submitQueryOnLoad = false){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			this.Visible=false;
			this.DoubleBuffered=true;
			Lan.F(this,new System.Windows.Forms.Control[] {
				//exclude:
				labelTotPages,
			});
			if(report!=null) {
				this._reportSimpleGrid=report;
			}
			_submitOnLoad=submitQueryOnLoad;
		}
		#endregion Constructor

		#region Events
		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormQueryEdit FormQE=new FormQueryEdit();
			FormQE.UserQueryCur=new UserQuery();
			FormQE.UserQueryCur.QueryText=textQuery.Text;
			FormQE.IsNew=true;
			FormQE.ShowDialog();
			if(FormQE.DialogResult==DialogResult.OK){
				textQuery.Text=FormQE.UserQueryCur.QueryText;
				grid2.CaptionText=FormQE.UserQueryCur.Description;
			}
		}

		private void butBack_Click(object sender, System.EventArgs e){
			if(printPreviewControl2.StartPage==0) { 
				return;
			}
			printPreviewControl2.StartPage--;
			labelTotPages.Text=(printPreviewControl2.StartPage+1).ToString()
				+" / "+_totalPages.ToString();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void butCopy_Click(object sender, System.EventArgs e){
			try {
				ODClipboard.SetClipboard(textQuery.Text);
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard.  Please try again.");
				ex.DoNothing();
			}
		}

		private void butExportExcel_Click(object sender, System.EventArgs e) {
			/*
			saveFileDialog2=new SaveFileDialog();
      saveFileDialog2.AddExtension=true;
			saveFileDialog2.Title=Lan.g(this,"Select Folder to Save File To");
		  if(IsReport){
				saveFileDialog2.FileName=report.Title;
			}
      else{
        saveFileDialog2.FileName=UserQueries.Cur.FileName;
			}
			if(!Directory.Exists( ((Pref)PrefC.HList["ExportPath"]).ValueString )){
				try{
					Directory.CreateDirectory( ((Pref)PrefC.HList["ExportPath"]).ValueString );
					saveFileDialog2.InitialDirectory=((Pref)PrefC.HList["ExportPath"]).ValueString;
				}
				catch{
					//initialDirectory will be blank
				}
			}
			else saveFileDialog2.InitialDirectory=((Pref)PrefC.HList["ExportPath"]).ValueString;
			//saveFileDialog2.DefaultExt="xls";
			//saveFileDialog2.Filter="txt files(*.txt)|*.txt|All files(*.*)|*.*";
      //saveFileDialog2.FilterIndex=1;
		  if(saveFileDialog2.ShowDialog()!=DialogResult.OK){
				saveFileDialog2.Dispose();
	   	  return;
			}
			Excel.Application excel=new Excel.ApplicationClass();
			excel.Workbooks.Add(Missing.Value);
			Worksheet worksheet = (Worksheet) excel.ActiveSheet;
			Range range=(Excel.Range)excel.Cells[1,1];
			range.Value2="test";
			range.Font.Bold=true;
			range=(Excel.Range)excel.Cells[1,2];
			range.ColumnWidth=30;
			range.FormulaR1C1="12345";
			excel.Save(saveFileDialog2.FileName);
			saveFileDialog2.Dispose();
	//this test case worked, so now it is just a matter of finishing this off, and Excel export will be done.
			MessageBox.Show(Lan.g(this,"File created successfully"));
			*/
		}

		private void butExport_Click(object sender, System.EventArgs e){
			if(_reportSimpleGrid.TableQ==null){
				MessageBox.Show(Lan.g(this,"Please run query first"));
				return;
			}
			string fileName=_reportSimpleGrid.Title;
			if(string.IsNullOrEmpty(fileName)) {
				fileName="queryexport.txt";
			}
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
			}
			else {
				saveFileDialog2=new SaveFileDialog();
				saveFileDialog2.AddExtension=true;
				//saveFileDialog2.Title=Lan.g(this,"Select Folder to Save File To");
				if(IsReport) {
					saveFileDialog2.FileName=fileName;
				}
				else {
					if(_userQueryCur==null || _userQueryCur.FileName==null || _userQueryCur.FileName=="")//.FileName==null)
						saveFileDialog2.FileName=_reportSimpleGrid.Title;
					else
						saveFileDialog2.FileName=_userQueryCur.FileName;
				}
				if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					try {
						Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
						saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
					catch {
						//initialDirectory will be blank
					}
				}
				else saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				//saveFileDialog2.DefaultExt="txt";
				saveFileDialog2.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
				saveFileDialog2.FilterIndex=0;
				if(saveFileDialog2.ShowDialog()!=DialogResult.OK) {
					saveFileDialog2.Dispose();
					return;
				}
				filePath=saveFileDialog2.FileName;
				saveFileDialog2.Dispose();
			}
			try{
			  using(StreamWriter sw=new StreamWriter(filePath,false))
					//new FileStream(,FileMode.Create,FileAccess.Write,FileShare.Read)))
				{
					String line="";
					for(int i=0;i<_reportSimpleGrid.ColCaption.Length;i++) {
						string columnCaption=_reportSimpleGrid.ColCaption[i];
						//Check for columns that start with special characters that spreadsheet programs treat differently than simply displaying them.
						if(columnCaption.StartsWith("-") || columnCaption.StartsWith("=")) {
							//Adding a space to the beginning of the cell will trick the spreadsheet program into not treating it uniquely.
							columnCaption=" "+columnCaption;
						}
						line+=columnCaption;
						if(i<_reportSimpleGrid.TableQ.Columns.Count-1){
							line+="\t";
						}
					}
					sw.WriteLine(line);
					string cell;
					for(int i=0;i<_reportSimpleGrid.TableQ.Rows.Count;i++){
						line="";
						for(int j=0;j<_reportSimpleGrid.TableQ.Columns.Count;j++){
							if(radioHuman.Checked && _tableHuman != null) {
								cell=_tableHuman.Rows[i][j].ToString();
							}
							else {
								cell=_reportSimpleGrid.TableQ.Rows[i][j].ToString();
							}
							cell=cell.Replace("\r","");
							cell=cell.Replace("\n","");
							cell=cell.Replace("\t","");
							cell=cell.Replace("\"","");
							line+=cell;
							if(j<_reportSimpleGrid.TableQ.Columns.Count-1){
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
      }
      catch{
        MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MessageBox.Show(Lan.g(this,"File created successfully"));
			}
		}

		private void butFavorites_Click(object sender, System.EventArgs e) {
			using FormQueryFavorites FormQF=new FormQueryFavorites();
			FormQF.UserQueryCur=_userQueryCur;
			FormQF.ShowDialog();
			if(FormQF.DialogResult!=DialogResult.OK) {
				return;
			}
			textQuery.Text=FormQF.UserQueryCur.QueryText;
			textTitle.Text=FormQF.UserQueryCur.Description;
			_userQueryCur=FormQF.UserQueryCur;
			SubmitQueryThreaded();
		}

		private void butFwd_Click(object sender, System.EventArgs e){
			if(printPreviewControl2.StartPage==_totalPages-1) return;
			printPreviewControl2.StartPage++;
			labelTotPages.Text=(printPreviewControl2.StartPage+1).ToString()
				+" / "+_totalPages.ToString();
		}

		private void butPaste_Click(object sender, System.EventArgs e){
			IDataObject iData;
			try {
				if(ODBuild.IsWeb()) {
					textQuery.Text=ODClipboard.GetText();
					return;
				}
				iData=Clipboard.GetDataObject();
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not paste contents from the clipboard.  Please try again.");
				ex.DoNothing();
				return;
			}
			if(iData.GetDataPresent(DataFormats.Text)){
				textQuery.Text=(String)iData.GetData(DataFormats.Text); 
			}
			else{
				MessageBox.Show(Lan.g(this,"Could not retrieve data off the clipboard."));
			}

		}

		private void butPrintPreview_Click(object sender, System.EventArgs e) {
			if(_reportSimpleGrid.TableQ==null) {
				MessageBox.Show(Lan.g(this,"Please run query first"));
				return;
			}
			printPreviewControl2.Visible=true;
			butPrintPreview.Visible=false;
			butQView.Visible=true;
			panelZoom.Visible=true;
			splitContainerQuery.Visible=false;
			_totalPages=0;
			//When generating a large print preview these UI changes happen in a very inconsistent manner and leave some elements looking cutoff.
			//Forcing the form to update UI before generating the preview fixes this problem.
			this.Update();
			PrintReport(true);
			printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Height
				/ODprintout.DefaultPaperSize.Height);
			UIHelper.ForceBringToFront(this);
		}

		private void butPrint_Click(object sender, System.EventArgs e) {
			if(_reportSimpleGrid.TableQ==null) {
				MessageBox.Show(Lan.g(this,"Please run query first"));
				return;
			}
			PrintReport(false);
			if(IsReport){
				DialogResult=DialogResult.Cancel;
			}
			if(printPreviewControl2.Visible) {
				PrintReport(true);//Fix the label that shows the page count.
			}
		}
		
		private void butQView_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.UserQuery)) {
				return;
			}
			printPreviewControl2.Visible=false;
			panelZoom.Visible=false;
			butPrintPreview.Visible=true;
			butQView.Visible=false;
			splitContainerQuery.Visible=true;
		}

		private void butSubmit_Click(object sender, System.EventArgs e) {
			if(butSubmit.Text==Lan.g(this,"Stop Execution")) { //Abort abort!
				//Flag the currently running query to stop.
				if(_threadQuery!=null) {
					_threadQuery.QuitAsync(true);
				}
				//Cancel the query that is currently running on MySQL using a KILL command.
				if(_serverThreadID!=0) {
					_queryExceptionStateCur = QueryExceptionState.Interrupt;
					DataConnectionCancelable.CancelQuery(_serverThreadID);
				}
			}
			else { //run query
				SubmitQueryThreaded();
			}
		}

		///<summary>When the form is closing, try to cancel any running queries.</summary>
		private void FormQuery_FormClosing(object sender,FormClosingEventArgs e) {
			//Set the query exception state to suppress all errors because the user is trying to close the window.
			_queryExceptionStateCur=QueryExceptionState.Suppress;
			//Try to cancel any queries that could be running right now.
			if(_serverThreadID!=0) {
				DataConnectionCancelable.CancelQuery(_serverThreadID,false);
			}
			ODThread.QuitAsyncThreadsByGroupName("UserQueryGroup",true);
		}

		private void FormQuery_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			//don't layout controls here.  It's far too frequent
		}

		private void LayoutControls(){
			LayoutManager.Move(printPreviewControl2,new Rectangle(0,0,ClientSize.Width,ClientSize.Height-LayoutManager.Scale(39)));
			Rectangle rectangle=new Rectangle(3,LayoutManager.Scale(25),ClientSize.Width-5,splitContainerQuery.Panel2.Height-LayoutManager.Scale(25));
			LayoutManager.Move(grid2,rectangle);
		}

		private void FormQuery_Load(object sender, System.EventArgs e) {
			LayoutControls();
			grid2.Font=bodyFont;
			splitContainerQuery.SplitterDistance=LayoutManager.Scale(140);
			splitContainerQuery.FixedPanel=FixedPanel.Panel1;
			if(IsReport){
				printPreviewControl2.Visible=true;
				splitContainerQuery.Visible=false;
				Text=Lan.g(this,"Report");
				butPrintPreview.Visible=false;
				panelZoom.Visible=true;
				PrintReport(true);
				labelTotPages.Text="/ "+_totalPages.ToString();
				printPreviewControl2.Zoom = (double)printPreviewControl2.ClientSize.Height / ODprintout.DefaultPaperSize.Height;
      }
			else {
				printPreviewControl2.Visible=false;
				splitContainerQuery.Visible=true;
				Text=Lan.g(this,"User Query");
			}
			if(!Security.IsAuthorized(Permissions.UserQueryAdmin,true)) { 
				//lock the query textbox, add button, and paste button for users without permissions.
				textQuery.ReadOnly=true;
				butAdd.Enabled=false;
				butPaste.Enabled=false;
			}
			if(string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ReportingServerDbName)) 
				|| string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ReportingServerCompName))) {
				checkReportServer.Visible=false;
			}
			else {//default to report server when one is set up.
				checkReportServer.Visible=true;
				checkReportServer.Checked=true;
			}
			if(_submitOnLoad) {
				//Coming from FormOpenDental menu item click for query favorites.  Existence in this list is taken to mean sql in these queries is 
				//considered safe to run.
				SubmitQueryThreaded(true);
			}
		}

		///<summary>Formats the current report to be human-readable. Does NOT run the whole query again.</summary>
		private void radioHuman_Click(object sender, System.EventArgs e) {
			FillForm();
		}

		///<summary>Formats the current report to be human-readable. Does NOT run the whole query again.</summary>
		private void radioRaw_Click(object sender, System.EventArgs e) {
			FillForm();
		}

		private void splitContainerQuery_SplitterMoved(object sender,SplitterEventArgs e) {
			LayoutControls();
			LayoutManager.LayoutControlBoundsAndFonts(splitContainerQuery);
		}

		///<summary>KeyDown instead of KeyPress because KeyDown provides e.KeyCode and KeyPress only provides e.KeyChar (which is a char value).</summary>
		private void textQuery_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode.ToString()=="F9") {
				butSubmit_Click(null,null);
			}
		}
		#endregion Events
		#region Enums
		///<summary>Enum to determine how to handle exceptions thrown by the _userQuery thread.</summary>
		private enum QueryExceptionState {
			///<summary>1 - means that query execution was interrupted by the user and will display a messagebox stating so.</summary>
			Interrupt,
			///<summary>2 - suppress any exceptions that arise.</summary>
			Suppress,
			///<summary>3 - throw any exceptions that arise (but in a messagebox so it looks better than a UE).</summary>
			Throw
		}

		///<summary>Enum to determine how to handle the controls in this form based on what is currently happening.</summary>
		private enum QueryExecuteState {
			///<summary>1 - Nothing is happening. All controls are available to be clicked, and the Submit button says "Submit Query" on it.</summary>
			Idle,
			///<summary>2 - Query is excecuting. Only the Close button and Submit button is available to be clicked, and the Submit button says "Cancel Query" on it.</summary>
			Executing,
			///<summary>3 - Query is in the process of being displayed. No controls are available to be clicked.</summary>
			Formatting
		}
		#endregion Enums
		#region Methods
		///<summary>Starting to use this externally as well.  OK to pass in a null report. 
		///Pass false to isConvertPatNum if the patnum should be printed as is, true if the patnum should be converted to the patient's full name.</summary>
		public static DataTable MakeReadable(DataTable tableIn,ReportSimpleGrid reportIn,bool doConvertPatNum=true){
			//this can probably be improved upon later for speed
			if(_hashListPlans==null){
				_hashListPlans=InsPlans.GetHListAll();
			}
			DataTable tableOut=tableIn.Clone();//copies just the structure
			for(int j=0;j<tableOut.Columns.Count;j++){
				tableOut.Columns[j].DataType=typeof(string);
			}
			DataRow thisRow;
			//copy data from tableInput to tableOutput while converting to strings
			//string str;
			//Type t;
			for(int i=0;i<tableIn.Rows.Count;i++){
				thisRow=tableOut.NewRow();//new row with new schema
				for(int j=0;j<tableIn.Columns.Count;j++){
					thisRow[j]=PIn.ByteArray(tableIn.Rows[i][j]);
					//str=tableIn.Rows[i][j].ToString();
					//t=tableIn.Rows[i][j].GetType();
					//thisRow[j]=str;
				}
				tableOut.Rows.Add(thisRow);
			}
			DateTime date;
			decimal[] colTotals=new decimal[tableOut.Columns.Count];
			for(int j=0;j<tableOut.Columns.Count;j++){
				for(int i=0;i<tableOut.Rows.Count;i++){
					try{
					if(tableOut.Columns[j].Caption.Substring(0,1)=="$"){
						tableOut.Rows[i][j]=PIn.Double(tableOut.Rows[i][j].ToString()).ToString("F");
						if(reportIn!=null) {
							reportIn.ColAlign[j]=HorizontalAlignment.Right;
							colTotals[j]+=PIn.Decimal(tableOut.Rows[i][j].ToString());
						}
					}
					else if(tableOut.Columns[j].Caption.ToLower().StartsWith("date")){
						date=PIn.Date(tableOut.Rows[i][j].ToString());
						if(date.Year<1880){
							tableOut.Rows[i][j]="";
						}
						else{
							tableOut.Rows[i][j]=date.ToString("d");
						}
					}
					else switch(tableOut.Columns[j].Caption.ToLower())
					{
						//bool
						case "isprosthesis":
						case "ispreventive":
						case "ishidden":
						case "isrecall":
						case "usedefaultfee":
						case "usedefaultcov":
						case "isdiscount":
						case "removetooth":
						case "setrecall":
						case "nobillins":
						case "isprosth":
						case "ishygiene":
						case "issecondary":
						case "orpribool":
						case "orsecbool":
						case "issplit":
  					case "ispreauth":
 					  case "isortho":
            case "releaseinfo":
            case "assignben":
            case "enabled":
            case "issystem":
            case "usingtin":
            case "sigonfile": 
            case "notperson":
            //case "isfrom"://refattach.IsFrom is now refattach.RefType, values 0=ReferralType.RefTo,1=ReferralType.RefFrom,2=ReferralType.RefCustom
							tableOut.Rows[i][j]=PIn.Bool(tableOut.Rows[i][j].ToString()).ToString();
							break;
						//date. Some of these are actually handled further up.
						case "adjdate":
						case "baldate":
						case "dateservice":
						case "datesent":
						case "datereceived":
						case "priordate":
						case "date":
						case "dateviewing":
						case "datecreated":
						case "dateeffective":
						case "dateterm":
						case "paydate":
						case "procdate":
						case "rxdate":
						case "birthdate":
						case "monthyear":
            case "accidentdate":
						case "orthodate":
            case "checkdate":
						case "screendate":
						case "datedue":
						case "dateduecalc":
						case "datefirstvisit":
						case "mydate"://this is a workaround for the daily payment report
							tableOut.Rows[i][j]=PIn.Date(tableOut.Rows[i][j].ToString()).ToString("d");
							break;
						//age
						case "birthdateforage":
							tableOut.Rows[i][j]=PatientLogic.DateToAgeString(PIn.Date(tableOut.Rows[i][j].ToString()));
							break;
						//time 
						case "aptdatetime":
						case "nextschedappt":
						case "starttime":
						case "stoptime":
							tableOut.Rows[i][j]=PIn.DateT(tableOut.Rows[i][j].ToString()).ToString("t")+"   "
								+PIn.DateT(tableOut.Rows[i][j].ToString()).ToString("d");
							break;
						//TimeCardManage
						case "adjevent":
						case "adjreg":
						case "adjotime":
						case "breaktime":
						case "temptotaltime":
						case "tempreghrs":
						case "tempovertime":
							if(PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon)) {
								tableOut.Rows[i][j]=PIn.Time(tableOut.Rows[i][j].ToString()).TotalHours.ToString("n");
							}
							else if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {//Colon format with seconds
								tableOut.Rows[i][j]=PIn.Time(tableOut.Rows[i][j].ToString()).ToStringHmmss();
							}
							else {//Colon format without seconds
								tableOut.Rows[i][j]=PIn.Time(tableOut.Rows[i][j].ToString()).ToStringHmm();
							}
							break;
  					//double
						case "adjamt":
						case "monthbalance":
						case "claimfee":
						case "inspayest":
						case "inspayamt":
						case "dedapplied":
						case "amount":
						case "payamt":
						case "splitamt":
						case "balance":
						case "procfee":
						case "overridepri":
						case "overridesec":
						case "priestim":
						case "secestim":
						case "procfees":
						case "claimpays":
						case "insest":
						case "paysplits":
						case "adjustments":
						case "bal_0_30":
						case "bal_31_60":
						case "bal_61_90":
						case "balover90":
						case "baltotal":
						case "inswoest":
							tableOut.Rows[i][j]=PIn.Double(tableOut.Rows[i][j].ToString()).ToString("F");
							if(reportIn!=null) {
								reportIn.ColAlign[j]=HorizontalAlignment.Right;
								colTotals[j]+=PIn.Decimal(tableOut.Rows[i][j].ToString());
							}
							break;
						case "toothnum":
							tableOut.Rows[i][j]=Tooth.ToInternat(tableOut.Rows[i][j].ToString());
							break;
						//definitions:
						case "adjtype":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.AdjTypes,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "confirmed":
							tableOut.Rows[i][j]
								=Defs.GetValue(DefCat.ApptConfirmed,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "dx":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.Diagnosis,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "discounttype":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.DiscountTypes,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "doccategory":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.ImageCats,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "op":
							tableOut.Rows[i][j]
								=Operatories.GetAbbrev(PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "paytype":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.PaymentTypes,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "proccat":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.ProcCodeCats,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "unschedstatus":
						case "recallstatus":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.RecallUnschedStatus,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "billingtype":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.BillingTypes,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						//patnums:
						case "patnum":
						case "guarantor":
						case "pripatnum":
						case "secpatnum":
						case "subscriber":
						case "withpat":
							if(!doConvertPatNum) {
								break;
							}
							if(_dictPatientNames==null) {
								_dictPatientNames=Patients.GetAllPatientNames();
							}
							long patNum=PIn.Long(tableOut.Rows[i][j].ToString());
							if(_dictPatientNames.ContainsKey(patNum)) {
								tableOut.Rows[i][j]=_dictPatientNames[patNum];
							}
							else {
								tableOut.Rows[i][j]="";
								Patient patNew=Patients.GetLim(patNum);
								if(patNew!=null) {		
									_dictPatientNames[patNew.PatNum]=patNew.GetNameLF();
									tableOut.Rows[i][j]=_dictPatientNames[patNum];
								}
							}
							break;
						//plannums:
						case "plannum":
						case "priplannum":
						case "secplannum":
							if(_hashListPlans.ContainsKey(PIn.Long(tableOut.Rows[i][j].ToString())))
								tableOut.Rows[i][j]=_hashListPlans[PIn.Long(tableOut.Rows[i][j].ToString())];
							else
								tableOut.Rows[i][j]="";
							break;
						//referralnum
						case "referralnum":
							Referral referral=null;
							Referrals.TryGetReferral(PIn.Long(tableOut.Rows[i][j].ToString()),out referral);
							tableOut.Rows[i][j]=(referral==null) ? "" : referral.LName+", "+referral.FName+" "+referral.MName;
							break; 
						//enumerations:
						case "aptstatus":
							tableOut.Rows[i][j]
								=((ApptStatus)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "category":
							//There are several tables that share the same column name... do your best to determine which enum to use.
							if(reportIn!=null && reportIn.Query!=null && reportIn.Query.ToLower().Contains("displayfield")) {
								tableOut.Rows[i][j]=((DisplayFieldCategory)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							}
							else {
								tableOut.Rows[i][j]=((DefCat)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							}			
							break;
						case "renewmonth":
							tableOut.Rows[i][j]=((Month)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "patstatus":
							tableOut.Rows[i][j]
								=((PatientStatus)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "gender":
							tableOut.Rows[i][j]
								=((PatientGender)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						//case "lab":
						//	tableOut.Rows[i][j]
						//		=((LabCaseOld)PIn.PInt(tableOut.Rows[i][j].ToString())).ToString();
						//  break;
						case "position":
							tableOut.Rows[i][j]
								=((PatientPosition)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "deductwaivprev":
						case "flocovered":
						case "misstoothexcl":
						case "procstatus":
							tableOut.Rows[i][j]=((ProcStat)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "majorwait":
						case "hascaries":
						case "needssealants":
						case "cariesexperience":
						case "earlychildcaries":
						case "existingsealants":
						case "missingallteeth":
							tableOut.Rows[i][j]=((YN)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "prirelationship":
						case "secrelationship":
							tableOut.Rows[i][j]=((Relat)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "treatarea":
							tableOut.Rows[i][j]
								=((TreatmentArea)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "specialty":
							tableOut.Rows[i][j]
								=Defs.GetName(DefCat.ProviderSpecialties,PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
						case "placeservice":
							tableOut.Rows[i][j]
								=((PlaceOfService)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
            case "employrelated": 
							tableOut.Rows[i][j]
								=((YN)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
            case "schedtype": 
							tableOut.Rows[i][j]
								=((ScheduleType)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
            case "dayofweek": 
							tableOut.Rows[i][j]
								=((DayOfWeek)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;					
						case "raceOld":
							tableOut.Rows[i][j]
								=((PatientRaceOld)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "gradelevel":
							tableOut.Rows[i][j]
								=((PatientGrade)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "urgency":
							tableOut.Rows[i][j]
								=((TreatmentUrgency)PIn.Long(tableOut.Rows[i][j].ToString())).ToString();
							break;
						case "reftype":
							tableOut.Rows[i][j]=((ReferralType)PIn.Int(tableOut.Rows[i][j].ToString()));
							break;
						//miscellaneous:
						case "provnum":
						case "provhyg":
						case "priprov":
						case "secprov":
            case "provtreat":
            case "provbill":   
							tableOut.Rows[i][j]=Providers.GetAbbr(PIn.Long(tableOut.Rows[i][j].ToString()));
							break;

						case "covcatnum":
							tableOut.Rows[i][j]=CovCats.GetDesc(PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
            case "referringprov": 
	//					  tableOut.Rows[i][j]=CovCats.GetDesc(PIn.PInt(tableOut.Rows[i][j].ToString()));
							break;			
            case "addtime":
							if(tableOut.Rows[i][j].ToString()!="0")
								tableOut.Rows[i][j]+="0";
							break;
						case "feesched":
						case "feeschednum":
							tableOut.Rows[i][j]=FeeScheds.GetDescription(PIn.Long(tableOut.Rows[i][j].ToString()));
							break;
					}//end switch column caption
					}//end try
					catch{
						//return tableOut;
					}
				}//end for i rows
			}//end for j cols
			if(reportIn!=null){
				for(int k=0;k<colTotals.Length;k++){
					reportIn.ColTotal[k]=PIn.Decimal(colTotals[k].ToString("n"));
				}
			}
			return tableOut;
		}

		///<summary>Print the report given ODprintout.CurPrintout. The boolean justpreview determines of the report is send to the printer or not.</summary>
		public bool PrintReport(bool justPreview){
			//TODO: Implement ODprintout pattern - print or debug preview control
			PrintoutOrientation orient=PrintoutOrientation.Default;
			Margins margins=new Margins(25,50,50,60);
			if(_reportSimpleGrid.IsLandscape) {
				orient=PrintoutOrientation.Landscape;
				margins=new Margins(25,120,50,60);
			}
			_pagesPrinted=0;
			_linesPrinted=0;
			ODprintout printout=PrinterL.CreateODprintout(pd2_PrintPage,Lan.g(this,"Query printed"),margins:margins,printoutOrientation:orient);
			if(printout!=null && printout.PrintDoc!=null) {
				//Make sure FormQuery is brought to the foreground when printing is complete, if FormQuery is still open.
				printout.PrintDoc.EndPrint+=(o,e) => {
					if(this!=null && !IsDisposedOrClosed(this)) {
						BringToFront();
						Focus();
					}
				};
			}
			if(ODprintout.CurPrintout.SettingsErrorCode!=PrintoutErrorCode.Success) {
				return false;
			}
			if(justPreview) {
				printPreviewControl2.Document=ODprintout.CurPrintout.PrintDoc;
				return true;
			}
			return PrinterL.TryPrint(ODprintout.CurPrintout);
		}

		///<summary>Clears the DataGridTableStyles for the current grid.</summary>
		public void ResetGrid(){ //not really sure what the point of this is.
			grid2.TableStyles.Clear();
			grid2.SetDataBinding(_reportSimpleGrid.TableQ,"");
			myGridTS = new DataGridTableStyle();
			grid2.TableStyles.Add(myGridTS);
		}

		///<summary>At this point, this only gets called externally when we want to automate the submission of a user query (like for the patients raw report).
		///Most of these reports can also probably use SubmitQueryThreaded() but will stay using SubmitQuery() for now.
		///Oracle also calls this instead of the threaded method.
		///Column names will be handled automatically.</summary>
		public void SubmitQuery(){
			Cursor=Cursors.WaitCursor;
			_tableHuman=null;
			try {
				_reportSimpleGrid.SubmitQuery();
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(Lan.g(this,"Invalid Query")+": "+ex.Message);
				return;
			}
			FillForm();
		}
		
		///<summary>Used to submit user queries in a thread.  Column names will be handled automatically.
		///Set isSqlValidated to true in order to skip SQL saftey validation.</summary>
		public void SubmitQueryThreaded(bool isSqlValidated=false) {
			if(_threadQuery!=null || _serverThreadID!=0) {
				return;//There is already a query executing.
			}
			//Clear the grid
			grid2.TableStyles.Clear();
			grid2.SetDataBinding(new DataTable(),"");
			//then create a new _reportSimpleGrid
			_reportSimpleGrid=new ReportSimpleGrid();
			_reportSimpleGrid.Query=textQuery.Text;
			_reportSimpleGrid.IsSqlValidated=isSqlValidated;
			if(DataConnection.DBtype==DatabaseType.Oracle) { //Can't cancel User queries for Oracle. this is still from the main thread so we should be ok.
				try {
					if(isSqlValidated || Db.IsSqlAllowed(_reportSimpleGrid.Query)) { //Throws Exception
						SubmitQuery();
					}
				}
				catch(Exception e){
					FriendlyException.Show(Lan.g(this,"Error submitting query."),e);
				}
				return;
			}
			_tableHuman=null;
			LayoutHelperForState(QueryExecuteState.Executing);
			_queryExceptionStateCur=QueryExceptionState.Throw;
			_threadQuery=new ODThread(OnThreadStart);
			_threadQuery.Name="UserQueryThread";
			_threadQuery.GroupName="UserQueryGroup";
			_threadQuery.AddExitHandler(OnThreadExit);
			_threadQuery.AddExceptionHandler(ExceptionThreadHandler);
			_threadQuery.Start(true); //run it once.
		}

		///<summary>When used as a report, this is called externally. Used instead of SubmitQuery().
		///Column names can be handled manually by the external form calling this report.</summary>
		public void SubmitReportQuery() {
			Cursor=Cursors.WaitCursor;
			_reportSimpleGrid.SubmitQuery();
			Cursor=Cursors.Default;
			grid2.TableStyles.Clear();
			myGridTS = new DataGridTableStyle();
			grid2.TableStyles.Add(myGridTS);
			_tableHuman=MakeReadable(_reportSimpleGrid.TableQ,_reportSimpleGrid);
			grid2.SetDataBinding(_tableHuman,"");
		}
		///<summary>Sizes the columns based on the the length of the text in the columns of the passed-in DataTable.</summary>
		private void AutoSizeColumns(DataTable table) {
			Graphics grfx=this.CreateGraphics();
			//int colWidth;
			int tempWidth;
			for(int i = 0;i<_reportSimpleGrid.ColWidth.Length;i++) {
				_reportSimpleGrid.ColWidth[i]
					=(int)grfx.MeasureString(table.Columns[i].Caption,grid2.Font).Width;
				for(int j = 0;j<table.Rows.Count;j++) {
					tempWidth=(int)grfx.MeasureString(table.Rows[j][i].ToString(),grid2.Font).Width;
					if(tempWidth>_reportSimpleGrid.ColWidth[i])
						_reportSimpleGrid.ColWidth[i]=tempWidth;
				}
				if(_reportSimpleGrid.ColWidth[i]>400) {
					_reportSimpleGrid.ColWidth[i]=400;
				}
				//I have no idea why this is failing, so we'll just do a check:
				if(myGridTS.GridColumnStyles.Count >= i+1) {
					myGridTS.GridColumnStyles[i].Width=_reportSimpleGrid.ColWidth[i]+12;
				}
				_reportSimpleGrid.ColWidth[i]+=6;//so the columns don't touch
				_reportSimpleGrid.ColPos[i+1]=_reportSimpleGrid.ColPos[i]+_reportSimpleGrid.ColWidth[i];
			}
		}

		///<summary>_userQuery exceptionHandler. Displays a messagebox or suppresses exceptions based on _exceptionStateCur.</summary>
		private void ExceptionThreadHandler(Exception e) {
			this.InvokeIfNotDisposed(() => {
				switch(_queryExceptionStateCur) {
					case QueryExceptionState.Interrupt:
						_queryExceptionStateCur=QueryExceptionState.Suppress;
						MsgBox.Show(this,"Query execution interrupted.");
						break;
					case QueryExceptionState.Suppress:
						//Swallow any errors.
						break;
					case QueryExceptionState.Throw:
					default:
						if(e.Message.ToLower().Contains("critical error")) {
							//we want to to see all the details and stacktrace for critical errors.
							using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(e.Message+"\r\n"+Lans.g(this,"Please call support.")+"\r\n\r\n"+e.StackTrace);
							msgBoxCopyPaste.ShowDialog();
						}
						else {
							MessageBox.Show(Lan.g(this,"Invalid query")+": "+e.Message);
						}
						break;
				}
			});
		}
		
		///<summary>Fills the form with the results of the query and formats it. If the Human Readable checkbox is checked, that logic is also done here.
		///Depending on what was selected, the MakeReadable() method can take a very long time. We may want to include this in the query thread
		///at some point so users can cancel out of this as well.</summary>
		private void FillForm() {
			//if this was called from a different thread, invoke it.
			if(this.InvokeRequired) {
				this.Invoke((Action)(() => { FillForm(); }));
				return;
			}
			//there's nothing in the table, so do nothing.
			if(_reportSimpleGrid.TableQ==null) {
				LayoutHelperForState(QueryExecuteState.Idle);
				return;
			}
			Cursor=Cursors.WaitCursor;
			LayoutHelperForState(QueryExecuteState.Formatting);
			//Set the data binding based on the radio buttons.
			myGridTS = new DataGridTableStyle();
			grid2.TableStyles.Clear();
			grid2.TableStyles.Add(myGridTS);
			if(radioHuman.Checked) {
				//fill the Human Readable table if it is null. (set to null on Submit_Click)
				if(_tableHuman==null) {
					_tableHuman=MakeReadable(_reportSimpleGrid.TableQ,_reportSimpleGrid);
				}
				grid2.SetDataBinding(_tableHuman,"");
				AutoSizeColumns(_tableHuman);
			}
			else {
				grid2.SetDataBinding(_reportSimpleGrid.TableQ,"");
				AutoSizeColumns(_reportSimpleGrid.TableQ);
			}
			_reportSimpleGrid.Title=textTitle.Text;
			_reportSimpleGrid.SubTitle.Clear();
			_reportSimpleGrid.SubTitle.Add(PrefC.GetString(PrefName.PracticeTitle));
			for(int iCol = 0;iCol<_reportSimpleGrid.TableQ.Columns.Count;iCol++) {
				_reportSimpleGrid.ColCaption[iCol]=_reportSimpleGrid.TableQ.Columns[iCol].Caption;
				//again, I don't know why this would fail, so here's a check:
				if(myGridTS.GridColumnStyles.Count >= iCol+1) {
					myGridTS.GridColumnStyles[iCol].Alignment=_reportSimpleGrid.ColAlign[iCol];
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,textQuery.Text);
			LayoutHelperForState(QueryExecuteState.Idle);
			Cursor=Cursors.Default;
		}

		///<summary>Sets the controls on the form to reflect the execute state passed in.  Typically used to disable all controls while executing.</summary>
		private void LayoutHelperForState(QueryExecuteState executeState) {
			switch (executeState) {
				case QueryExecuteState.Idle:
					butSubmit.Text=Lan.g(this,"Submit Query"); //all controls enabled
					this.Text=Lan.g(this,"User Query");
					textQuery.Enabled=true;
					butSubmit.Enabled=true;
					butFavorite.Enabled=true;
					groupBox1.Enabled=true;
					butAdd.Enabled=true;
					butCopy.Enabled=true;
					butPaste.Enabled=true;
					butExportExcel.Enabled=true;
					butExport.Enabled=true;
					butPrintPreview.Enabled=true;
					butPrint.Enabled=true;
					panelZoom.Enabled=true;
					butQView.Enabled=true;
					textTitle.Enabled=true;
					butClose.Enabled=true;
					break;
				case QueryExecuteState.Executing:
					butSubmit.Text=Lan.g(this,"Stop Execution"); //the submit button and the close button should be enabled.
					butClose.Enabled=true;
					this.Text=Lans.g(this,"User Query - Executing Query...");
					textQuery.Enabled=false;
					butFavorite.Enabled=false;
					groupBox1.Enabled=false;
					butAdd.Enabled=false;
					butCopy.Enabled=false;
					butPaste.Enabled=false;
					butExportExcel.Enabled=false;
					butExport.Enabled=false;
					butPrintPreview.Enabled=false;
					butPrint.Enabled=false;
					panelZoom.Enabled=false;
					butQView.Enabled=false;
					textTitle.Enabled=false;
					break;
				case QueryExecuteState.Formatting:
					butSubmit.Text=Lan.g(this,"Submit Query"); //no enabled controls
					this.Text=Lans.g(this,"User Query - Formatting Query...");
					butSubmit.Enabled=false;
					textQuery.Enabled=false;
					butFavorite.Enabled=false;
					groupBox1.Enabled=false;
					butAdd.Enabled=false;
					butCopy.Enabled=false;
					butPaste.Enabled=false;
					butExportExcel.Enabled=false;
					butExport.Enabled=false;
					butPrintPreview.Enabled=false;
					butPrint.Enabled=false;
					panelZoom.Enabled=false;
					butQView.Enabled=false;
					textTitle.Enabled=false;
					butClose.Enabled=false;
					break;
			}
			if(!Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
				//lock the query textbox, add button, and paste button for users without permissions.
				textQuery.ReadOnly=true;
				butAdd.Enabled=false;
				butPaste.Enabled=false;
			}
		}

		private void OnThreadStart(ODThread thread) {
			bool hasStackTrace = UserHasStackTracePref();
			_serverThreadID=DataConnectionCancelable.GetServerThread(checkReportServer.Checked);
			_reportSimpleGrid.TableQ=DataConnectionCancelable.GetTableConAlreadyOpen(_serverThreadID,_reportSimpleGrid.Query
				,_reportSimpleGrid.IsSqlValidated,checkReportServer.Checked,hasStackTrace);
			_reportSimpleGrid.InitializeColumns();
			_reportSimpleGrid.FixColumnNames();
		}

		private bool UserHasStackTracePref() {
			UserOdPref pref = UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum, UserOdFkeyType.QueryMonitorHasStackTraces);
			if(pref.IsNew) {
				pref.ValueString="0";
				UserOdPrefs.Insert(pref);
				return false;
			}
			else {
				if(pref.ValueString=="1") {
					return true;
				}
				return false;
			}
		}

		///<summary>_userQuery exitHandler. Fills the form if the form is not disposed.</summary>
		private void OnThreadExit(ODThread thread) {
			try {
				_threadQuery=null;
				_serverThreadID=0;
				FillForm();
			}
			catch(Exception e) {
				e.DoNothing();
			}
		}

		///<summary>This method contains the logic to print a given page using the grfx Graphics object. It tracks the yPos to extend the data over pages
		///as necessary using a standard paperSize and predefined currentMargins.</summary>
		private void pagePrinter(PrintPageEventArgs ev) {
			//We use this boolean for determining if the current page is requested in a print operation, if not then we skip all draw statements
			bool isPrintablePage=ev.PageSettings.PrinterSettings.PrintRange==PrintRange.AllPages
				|| (ev.PageSettings.PrinterSettings.FromPage<=_currentPage && ev.PageSettings.PrinterSettings.ToPage>=_currentPage);
			Rectangle bounds=ev.MarginBounds;
			float yPos = bounds.Top;
			if(!_headerPrinted){
				if(isPrintablePage){
					ev.Graphics.DrawString(_reportSimpleGrid.Title
						,titleFont,Brushes.Black
						,bounds.Width/2
						-ev.Graphics.MeasureString(_reportSimpleGrid.Title,titleFont).Width/2,yPos);
				}
				yPos+=titleFont.GetHeight(ev.Graphics);
				for(int i=0;i<_reportSimpleGrid.SubTitle.Count;i++){
					if(isPrintablePage) {
						ev.Graphics.DrawString(_reportSimpleGrid.SubTitle[i]
							,subtitleFont,Brushes.Black
							,bounds.Width/2
							-ev.Graphics.MeasureString(_reportSimpleGrid.SubTitle[i],subtitleFont).Width/2,yPos);
					}
					yPos+=subtitleFont.GetHeight(ev.Graphics)+2;
				}
				_headerPrinted=true;
			}
			yPos+=10; 
			if(isPrintablePage) {
				ev.Graphics.DrawString(Lan.g(this,"Date:")+" "+DateTime.Today.ToString("d")
					,bodyFont,Brushes.Black,bounds.Left,yPos);
				ev.Graphics.DrawString(Lan.g(this,"Page:")+" "+(_currentPage).ToString()
					,bodyFont,Brushes.Black
					,bounds.Right
					-ev.Graphics.MeasureString(Lan.g(this,"Page:")+" "+(_currentPage).ToString()
					,bodyFont).Width,yPos);
			}
			yPos+=bodyFont.GetHeight(ev.Graphics)+10;
			if(isPrintablePage) {
				ev.Graphics.DrawLine(new Pen(Color.Black),bounds.Left,yPos-5,bounds.Right,yPos-5);
			}
			//column captions:
			if(isPrintablePage) {
				for(int i=0;i<_reportSimpleGrid.ColCaption.Length;i++){
					if(_reportSimpleGrid.ColAlign[i]==HorizontalAlignment.Right){
						ev.Graphics.DrawString(_reportSimpleGrid.ColCaption[i]
							,colCaptFont,Brushes.Black,new RectangleF(
							bounds.Left+_reportSimpleGrid.ColPos[i+1]
							-ev.Graphics.MeasureString(_reportSimpleGrid.ColCaption[i],colCaptFont).Width,yPos
							,_reportSimpleGrid.ColWidth[i],colCaptFont.GetHeight(ev.Graphics)));
					}
					else{
						ev.Graphics.DrawString(_reportSimpleGrid.ColCaption[i]
							,colCaptFont,Brushes.Black,bounds.Left+_reportSimpleGrid.ColPos[i],yPos);
					}
				}
			}
			yPos+=bodyFont.GetHeight(ev.Graphics)+5;
			float fontHeight=bodyFont.GetHeight(ev.Graphics);
			float yPosTableTop=yPos;
			//table: each loop iteration prints one row in the grid.
			while(yPos<bounds.Top+bounds.Height-18//The 18 is minimum allowance for the line about to print. 
				&& _linesPrinted < _reportSimpleGrid.TableQ.Rows.Count)//Page might finish early on the last page.
			{
				bool isColWrap=PrefC.GetBool(PrefName.ReportsWrapColumns);
				if(isColWrap && yPos > yPosTableTop) {//First row always prints.  Otherwise the row might be pushed to next page if too tall.
					int cellWidth;//Width to be adjusted and used to calculate row height.
					bool isRowTooTall=false;//Bool to indicate if a row we are about to print is too tall for the avaible space on page.
					for(int iCol2=0;iCol2<_reportSimpleGrid.TableQ.Columns.Count;iCol2++){
						if(_reportSimpleGrid.ColAlign[iCol2]==HorizontalAlignment.Right) {
							cellWidth=_reportSimpleGrid.ColWidth[iCol2];
						}
						else {
							cellWidth=_reportSimpleGrid.ColPos[iCol2+1]-_reportSimpleGrid.ColPos[iCol2]+6;
						}
						//Current height of the string with given width.
						string cellText=grid2[_linesPrinted,iCol2].ToString();
						float rectHeight=ev.Graphics.MeasureString(cellText,bodyFont,cellWidth).Height;
						if(yPos+rectHeight > bounds.Bottom) {//Check for if we have enough height to print on current page.
							isRowTooTall=true;
							break;
						}
					}
					if(isRowTooTall) {
						break;//Break so current row goes to next page.
					}
				}
				float rowHeight=fontHeight;//When wrapping, we get the hight of the tallest cell in the row and increase yPos by it.
				for(int iCol=0;iCol<_reportSimpleGrid.TableQ.Columns.Count;iCol++){//For each cell in the row, print the cell contents.
					float cellHeight=rowHeight;
					if(isColWrap) {
						cellHeight=0;//Infinate height.
					}
					int cellWidth=0;
					string cellText=grid2[_linesPrinted,iCol].ToString();
					if(_reportSimpleGrid.ColAlign[iCol]==HorizontalAlignment.Right){
						cellWidth=_reportSimpleGrid.ColWidth[iCol];
						if(isPrintablePage) {
							ev.Graphics.DrawString(cellText
								,bodyFont,Brushes.Black,new RectangleF(
								bounds.Left+_reportSimpleGrid.ColPos[iCol+1]
								-ev.Graphics.MeasureString(cellText,bodyFont).Width-1,yPos
								,cellWidth,cellHeight));
						}
					}
					else{
						cellWidth=_reportSimpleGrid.ColPos[iCol+1]-_reportSimpleGrid.ColPos[iCol]+6;
						if(isPrintablePage) {
							ev.Graphics.DrawString(cellText
								,bodyFont,Brushes.Black,new RectangleF(
								bounds.Left+_reportSimpleGrid.ColPos[iCol],yPos
								,cellWidth
								,cellHeight));
						}
					}
					if(isColWrap) {
						rowHeight=Math.Max(rowHeight,ev.Graphics.MeasureString(cellText,bodyFont,cellWidth).Height);
					}
				}
				yPos+=rowHeight;
				_linesPrinted++;
				if(_linesPrinted==_reportSimpleGrid.TableQ.Rows.Count){
					_tablePrinted=true;
				}
			}
			if(_reportSimpleGrid.TableQ.Rows.Count==0){
				_tablePrinted=true;
			}
			//totals:
			if(_tablePrinted){
				if(yPos<bounds.Bottom){
					if(isPrintablePage) {
						ev.Graphics.DrawLine(new Pen(Color.Black),bounds.Left,yPos+3,bounds.Right,yPos+3);
					}
					yPos+=4;
					for(int iCol=0;iCol<_reportSimpleGrid.TableQ.Columns.Count;iCol++){
						if(_reportSimpleGrid.ColAlign[iCol]==HorizontalAlignment.Right){
							if(_reportSimpleGrid.TableQ.Columns[iCol].Caption.ToLower().StartsWith("count(")) {//"=="count(*)") {
								continue;
							}
							float textWidth=(float)(ev.Graphics.MeasureString
								(_reportSimpleGrid.ColTotal[iCol].ToString("n"),subtitleFont).Width);
							if(isPrintablePage) {
								ev.Graphics.DrawString(_reportSimpleGrid.ColTotal[iCol].ToString("n")
									,subtitleFont,Brushes.Black,new RectangleF(
									bounds.Left+_reportSimpleGrid.ColPos[iCol+1]-textWidth+3,yPos//the 3 is arbitrary
									,textWidth,subtitleFont.GetHeight(ev.Graphics)));
							}
						}
					}
					_totalsPrinted=true;
					yPos+=subtitleFont.GetHeight(ev.Graphics);
				}
			}
			//Summary
			if(_totalsPrinted){
				if(yPos+_reportSimpleGrid.Summary.Count*subtitleFont.GetHeight(ev.Graphics)< bounds.Top+bounds.Height){
					if(isPrintablePage){
						ev.Graphics.DrawLine(new Pen(Color.Black),bounds.Left,yPos+2,bounds.Right,yPos+2);
					}
					yPos+=bodyFont.GetHeight(ev.Graphics);
					for(int i=0;i<_reportSimpleGrid.Summary.Count;i++){
						if(_reportSimpleGrid.SummaryFont!=null && _reportSimpleGrid.SummaryFont!=""){
							Font acctnumFont=new Font(_reportSimpleGrid.SummaryFont,12);
							if(isPrintablePage) {
								ev.Graphics.DrawString(_reportSimpleGrid.Summary[i],acctnumFont,Brushes.Black,bounds.Left,yPos);
							}
							yPos+=acctnumFont.GetHeight(ev.Graphics);
						}
						else{
							if(isPrintablePage) {
								ev.Graphics.DrawString(_reportSimpleGrid.Summary[i],subtitleFont,Brushes.Black,bounds.Left,yPos);
							}
							yPos+=subtitleFont.GetHeight(ev.Graphics);
						}
					}
					_summaryPrinted=true;
				}
			}
			_pagesPrinted++;
		}

		///<summary>raised for each page to be printed.</summary>
		private void pd2_PrintPage(object sender, PrintPageEventArgs ev){
			do {
				pagePrinter(ev);
			} while(ev.PageSettings.PrinterSettings.PrintRange!=PrintRange.AllPages && ev.PageSettings.PrinterSettings.FromPage>=_currentPage);
			//if the summary or the last configured print page has printed, then there are no more pages to print.
			if(_summaryPrinted || (ev.PageSettings.PrinterSettings.PrintRange!=PrintRange.AllPages && ev.PageSettings.PrinterSettings.ToPage<_currentPage)) 
			{ 
				ev.HasMorePages=false;
				_totalPages=_pagesPrinted;
				int pageCur=1;
				if(printPreviewControl2.Visible) {
					pageCur=(printPreviewControl2.StartPage+1);
				}
				labelTotPages.Text=pageCur+" / "+_totalPages.ToString();
				_pagesPrinted=0;
				_linesPrinted=0;
				_headerPrinted=false;
				_tablePrinted=false;
				_totalsPrinted=false;
				_summaryPrinted=false;
			}
			else {//If we reach the end of the document, OR the end of the specificed range, stop printed. Otherwise, continue.
				ev.HasMorePages=true;
			}
		}
		#endregion Methods
	}
}
