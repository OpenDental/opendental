//using Excel;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Threading.Tasks;
using System.Linq;
using OpenDental.ReportingComplex;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormUserQuery:FormODBase {
		#region Fields
		///<summary>Set to true if this form should instantly launch a print preview instead of showing the actual "user query" form.</summary>
		public bool IsReport;
		///<summary>A dictionary of patient names that only gets filled once and only if necessary.</summary>
		private static Dictionary<long,string> _dictPatientNames;
		///<summary>A hashtable of key: InsPlanNum, value: Carrier Name.</summary>
		private static Hashtable _hashListPlans;
		///<summary>Tells the form whether to throw, show (interrupt), or suppress exceptions. Set manually in the code below.</summary>
		private QueryExceptionState _queryExceptionStateCur;
		private string _query;
		///<summary>The server thread id that the current query is running on.</summary>
		private int _serverThreadID;
		private bool _submitOnLoad;
		///<summary>This holds the raw data from the query, transformed into a list of lists of strings. this is used to fill the grid.</summary>
		//private Dictionary<string,List<string>> _dictionaryDataHumanReadable;
		private List<List<string>> _listRawData;
		///<summary>User queries run on a separate thread so they can be cancelled.</summary>
		private ODThread _threadQuery;
		private int _totalPages=0;
		private UserQuery _userQueryCur;
		private static List<Def> _listAdjTypes;
		private static List<Def> _listDiagnosis;
		private static List<Def> _listDiscountTypes;
		private static List<Def> _listImageCats;
		private static List<Def> _listPaymentTypes;
		private static List<Def> _listProcCodeCats;
		private static List<Def> _listRecallUnschedStatus;
		private static List<Def> _listBillingTypes;
		private static List<QueryColumn> _listQueryColumns;
		private static bool _timeCardShowSeconds;
		private static bool _timeCardsUseDecilamInsteadOfColon;
		private static bool _getDisplayFieldCategory;
		private static List<string> _listRightAlignColumns = new List<string>{"adjamt", "monthbalance", "claimfee", "inspayest", "inspayamt", "dedapplied", "amount", "payamt", "splitamt", "balance", "procfee", "overridepri", "overridesec", "priestim", "secestim", "procfees", "claimpays", "insest", "paysplits", "adjustments", "bal_0_30", "bal_31_60", "bal_61_90", "balover90", "baltotal", "inswoest" };
		private int _pagesPrinted;
		private bool _headingPrinted;
		private int _headingPrintH;
		private bool _headerPrinted;
		private int _linesPrinted;
		private bool _tablePrinted;
		private HorizontalAlignment _alignmentCur;
		private MenuItem _menuItemGoToPatient;

		private int _currentPage {
			get {
				return _pagesPrinted+1;
			}
		}
		#endregion Fields

		#region Constructor
		public FormUserQuery(string query = "",bool submitQueryOnLoad = false) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			this.DoubleBuffered=true;
			_query = query;
			_submitOnLoad=submitQueryOnLoad;
		}
		#endregion Constructor

		#region Events
		private void FormQuery_Load(object sender,System.EventArgs e) {
			LayoutControls();
			splitContainerQuery.SplitterDistance=LayoutManager.Scale(140);
			splitContainerQuery.FixedPanel=FixedPanel.Panel1;
			comboAlignment.Items.AddEnums<HorizontalAlignment>();
			comboAlignment.SetSelected(0);
			_alignmentCur=comboAlignment.GetSelected<HorizontalAlignment>();
			textQuery.Text = _query;
			if(IsReport) {
				splitContainerQuery.Visible=false;
				Text=Lan.g(this,"Report");
				butPrintPreview.Visible=false;
			}
			else {
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
			_menuItemGoToPatient=new MenuItem();
			_menuItemGoToPatient.Text="Go To Patient";
			_menuItemGoToPatient.Click+=new EventHandler(MenuItemGoToPatient_Click);
			if(_gridResults.ContextMenu==null) {
				_gridResults.ContextMenu=new ContextMenu();
			}
			//essentially filling a cache for the "MakeReadable" method, instead of doing this inside a loop that could take longer. Getting these on-load will not take long and will speed up query processing. 
			_listRawData = new List<List<string>>();
			_listQueryColumns = new List<QueryColumn>();
			_listRecallUnschedStatus = Defs.GetDefsForCategory(DefCat.RecallUnschedStatus);
			_listProcCodeCats = Defs.GetDefsForCategory(DefCat.ProcCodeCats);
			_listPaymentTypes = Defs.GetDefsForCategory(DefCat.PaymentTypes);
			_listImageCats = Defs.GetDefsForCategory(DefCat.ImageCats);
			_listDiscountTypes =Defs.GetDefsForCategory(DefCat.DiscountTypes);
			_listDiagnosis = Defs.GetDefsForCategory(DefCat.Diagnosis);
			_listAdjTypes = Defs.GetDefsForCategory(DefCat.AdjTypes);
			_listBillingTypes = Defs.GetDefsForCategory(DefCat.BillingTypes);
			_hashListPlans=InsPlans.GetHListAll();
			_timeCardShowSeconds = PrefC.GetBool(PrefName.TimeCardShowSeconds);
			_timeCardsUseDecilamInsteadOfColon = PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon);
			if(_submitOnLoad) {
				//Coming from FormOpenDental menu item click for query favorites.  Existence in this list is taken to mean sql in these queries is 
				//considered safe to run.
				SubmitQueryThreaded(true);
			}
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			using FormQueryEdit FormQE=new FormQueryEdit();
			FormQE.UserQueryCur=new UserQuery();
			FormQE.UserQueryCur.QueryText=textQuery.Text;
			FormQE.IsNew=true;
			FormQE.ShowDialog();
			if(FormQE.DialogResult==DialogResult.OK) {
				textQuery.Text=FormQE.UserQueryCur.QueryText;
				_gridResults.Title=FormQE.UserQueryCur.Description;
			}
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void butCopy_Click(object sender,System.EventArgs e) {
			try {
				ODClipboard.SetClipboard(textQuery.Text);
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not copy contents to the clipboard.  Please try again.");
				ex.DoNothing();
			}
		}

		private void butFavorites_Click(object sender,System.EventArgs e) {
			using FormQueryFavorites FormQF=new FormQueryFavorites();
			FormQF.UserQueryCur=_userQueryCur;
			FormQF.ShowDialog();
			if(FormQF.DialogResult!=DialogResult.OK) {
				return;
			}
			textQuery.Text=FormQF.UserQueryCur.QueryText;
			textTitle.Text=FormQF.UserQueryCur.Description;
			_userQueryCur=FormQF.UserQueryCur;
			_query=FormQF.UserQueryCur.QueryText;
			SubmitQueryThreaded();
		}

		private void butPaste_Click(object sender,System.EventArgs e) {
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
			if(iData.GetDataPresent(DataFormats.Text)) {
				textQuery.Text=(String)iData.GetData(DataFormats.Text);
			}
			else {
				MessageBox.Show(Lan.g(this,"Could not retrieve data off the clipboard."));
			}
		}

		private void butSubmit_Click(object sender,System.EventArgs e) {
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
				_query = textQuery.Text;
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

		private void FormQuery_Layout(object sender,System.Windows.Forms.LayoutEventArgs e) {
			//don't layout controls here.  It's far too frequent
		}

		private void LayoutControls() {
			Rectangle rectangle=new Rectangle(3,LayoutManager.Scale(25),ClientSize.Width-5,splitContainerQuery.Panel2.Height-LayoutManager.Scale(25));
			LayoutManager.Move(_gridResults,rectangle);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(_gridResults.ListGridRows.Count == 0 && _gridResults.Columns.Count == 0) {
				MsgBox.Show(MessageBoxButtons.OK,Lan.g(this,"No report to print. Please run a query."));
				return;
			}
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(
				pd_PrintPage,
				Lan.g(this,$"{(String.IsNullOrEmpty(textTitle.Text) ? "User Query" : textTitle.Text)} printed")
			);
			_linesPrinted=0;
		}

		private void butPrintPreview_Click(object sender,EventArgs e) {
			if(_gridResults.ListGridRows.Count == 0 && _gridResults.Columns.Count == 0) {
				MsgBox.Show(MessageBoxButtons.OK,Lan.g(this,"No report to preview. Please run a query."));
				return;
			}
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(
				pd_PrintPage,
				Lan.g(this,$"{(String.IsNullOrEmpty(textTitle.Text) ? "User Query" : textTitle.Text)} previewed"),
				isForcedPreview:true
			);
			_linesPrinted=0;
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
			if(!_headingPrinted) {
				text=Lan.g(this,textTitle.Text);
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				//print today's date
				text = Lan.g(this,"Run On:")+" "+DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=_gridResults.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butExport_Click(object sender,System.EventArgs e) {
			if(_gridResults.ListGridRows.Count==0 && _gridResults.Columns.Count == 0) {
				MessageBox.Show(Lan.g(this,"Please run query first"));
				return;
			}
			string fileName=textTitle.Text;
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
						saveFileDialog2.FileName=textTitle.Text;
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
			try {
				using(StreamWriter sw = new StreamWriter(filePath,false))
				//new FileStream(,FileMode.Create,FileAccess.Write,FileShare.Read)))
				{
					String line="";
					for(int i = 0;i<_gridResults.Columns.Count;i++) {
						string columnCaption=_gridResults.Columns[i].Heading;
						//Check for columns that start with special characters that spreadsheet programs treat differently than simply displaying them.
						if(columnCaption.StartsWith("-") || columnCaption.StartsWith("=")) {
							//Adding a space to the beginning of the cell will trick the spreadsheet program into not treating it uniquely.
							columnCaption=" "+columnCaption;
						}
						line+=columnCaption;
						if(i<_gridResults.Columns.Count-1) {
							line+="\t";
						}
					}
					sw.WriteLine(line);
					string cell;
					for(int i = 0;i<_gridResults.ListGridRows.Count;i++) {
						line="";
						for(int j = 0;j<_gridResults.Columns.Count;j++) {
							cell=_gridResults.ListGridRows[i].Cells[j].Text;
							cell=cell.Replace("\r","");
							cell=cell.Replace("\n","");
							cell=cell.Replace("\t","");
							cell=cell.Replace("\"","");
							line+=cell;
							if(j<_gridResults.Columns.Count-1) {
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
			}
			catch {
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

		///<summary>Formats the current report to be human-readable. Does NOT run the whole query again.</summary>
		private void radioHuman_Click(object sender,System.EventArgs e) {
			FillForm();
		}

		///<summary>Formats the current report to be human-readable. Does NOT run the whole query again.</summary>
		private void radioRaw_Click(object sender,System.EventArgs e) {
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
		private void FillGrid(bool showRawData = false) {
			labelRowCount.Visible = true;
			labelRowCount.Text = $"Row Count: {_listRawData.Count}";
			//gets base widths for columns
			_listQueryColumns.ForEach(x => x.Width = TextRenderer.MeasureText(x.Name,_gridResults.Font).Width + 10);
			List<HorizontalAlignment> listColumnAligments = new List<HorizontalAlignment>(_listQueryColumns.Count);
			_listQueryColumns.ForEach(x => x.Total=0);
			string valueChanged = "";
			_gridResults.BeginUpdate();
			_gridResults.Columns.Clear();
			_gridResults.ListGridRows.Clear();
			//fill rows and calculate widths for columns based on row data
			int rowCount = 1;//row count always starts at 1
			foreach(List<string> row in _listRawData) {
				UI.GridRow gridRow = new UI.GridRow();
				if(checkNumberedRows.Checked) {//add numbered rows if selected
					gridRow.Cells.Add(new UI.GridCell(rowCount++.ToString()));//increment rowCount after filling cell
				}
				int i = 0;
				row.ForEach(
						value => {
							//replace data such as PatNums numbers based on column names if viewing Human Readable mode
							valueChanged = showRawData ? value : ReplaceColumnString(i,value);
							if(_listQueryColumns[i].Name.ToLower() == "patnum") {
								gridRow.Tag = value;
							}
							gridRow.Cells.Add(new UI.GridCell(valueChanged));
							//get widest data in column, used to set column width later. Max column width of 400 pixels (word wrap can be turned on if more space is needed)
							_listQueryColumns[i].Width = Math.Min(Math.Max(_listQueryColumns[i].Width,TextRenderer.MeasureText(valueChanged,_gridResults.Font).Width + 10),400);
							AddToColumnTotal(i,valueChanged);
							i++;
						}
				);
				_gridResults.ListGridRows.Add(gridRow);
			}
			if(_listQueryColumns.Any(x => x.ShowTotal)) {
				UI.GridRow totalsRow = new UI.GridRow();
				if(checkNumberedRows.Checked) {
					totalsRow.Cells.Add(new UI.GridCell());
				}
				_listQueryColumns.ForEach(x => {
					totalsRow.Cells.Add(new UI.GridCell(x.ShowTotal ? x.Total.ToString("F") : ""));
					x.Width = Math.Min(Math.Max(x.Width,TextRenderer.MeasureText(x.Total.ToString("F"),_gridResults.Font).Width + 10),400);//have to add a little extra to fit the full total sometimes
				});
				_gridResults.ListGridRows.Add(totalsRow);
			}
			if(checkNumberedRows.Checked) {
				_gridResults.Columns.Add(new UI.GridColumn("#",60,comboAlignment.GetSelected<HorizontalAlignment>(),sortingStrategy: UI.GridSortingStrategy.AmountParse));
			}
			_listQueryColumns.ForEach(x =>
				_gridResults.Columns.Add(new UI.GridColumn(
					x.Name,
					x.Width, //gets width as determined by the rows
					GetAlignment(x.Name),
					sortingStrategy: x.SortingStrategy))
				);
			_gridResults.WrapText = checkWordWrap.Checked;
			_gridResults.EndUpdate();
		}

		private void AddToColumnTotal(int index,string value) {
			if(_listQueryColumns[index].Name[0] == '$') {
				double parseResult=0;
				_listQueryColumns[index].ShowTotal=true;
				if(Double.TryParse(value, out parseResult)) {
					_listQueryColumns[index].Total+=parseResult;
				}
			}
		}

		private void ClearGrid() {
			_gridResults.BeginUpdate();
			_gridResults.ListGridRows.Clear();
			_gridResults.Columns.Clear();
			_gridResults.EndUpdate();
		}

		//Right aligns certain columns we replace with decimal values based on column title, otherwise defaults to left aligned.
		private HorizontalAlignment GetAlignment(string value) {
			if(value[0] == '$' || _listRightAlignColumns.Contains(value.ToLower()) || value.All(x => char.IsDigit(x))) {
				return HorizontalAlignment.Right;
			}
			return comboAlignment.GetSelected<HorizontalAlignment>();
		}

		//Takes in a column name and a string to be replace. Returns a transformed string based on the column it appears in.
		private static string ReplaceColumnString(int index,string dataString) {
			string columnName = _listQueryColumns[index].Name;
			string retVal = "";
			try {
				columnName = columnName.ToLower();
				if(columnName[0]=='$') {
					retVal=PIn.Double(dataString).ToString("F");
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.AmountParse;
					return retVal;
				}
				if(columnName.StartsWith("date")) {
					if(String.IsNullOrEmpty(dataString) || dataString == "0001-01-01" || dataString == "0") {
						retVal = "";
						return retVal;
					}
					DateTime date=PIn.Date(dataString);
					retVal=date.Year==0001 ? "" : date.ToString("d");
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.DateParse;
					return retVal;
				}
				if(columnName.ToLower().Contains("num")) {
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.AmountParse;
				}
				retVal = dataString;
				switch(columnName) {
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
					retVal=PIn.Bool(dataString).ToString();
					break;
					//date. Some of these are actually handled further up.
					case "adjdate":
					case "baldate":
					//case "dateservice":
					//case "datesent":
					//case "datereceived":
					case "priordate":
					//case "date":
					//case "dateviewing":
					//case "datecreated":
					//case "dateeffective":
					//case "dateterm":
					case "paydate":
					case "procdate":
					case "rxdate":
					case "birthdate":
					case "monthyear":
					case "accidentdate":
					case "orthodate":
					case "checkdate":
					case "screendate":
					//case "datedue":
					//case "dateduecalc":
					//case "datefirstvisit":
					case "mydate"://this is a workaround for the daily payment report
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.DateParse;
					retVal=PIn.Date(dataString).ToString("d");
					break;
					//age
					case "birthdateforage":
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.AmountParse;
					retVal=PatientLogic.DateToAgeString(PIn.Date(dataString));
					break;
					//time 
					case "aptdatetime":
					case "nextschedappt":
					case "starttime":
					case "stoptime":
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.DateParse;
					if(String.IsNullOrEmpty(dataString) || dataString.StartsWith("0001-01-01") || dataString == "0") {
						retVal = "";
						break;
					}
					retVal=DateTime.Parse(dataString).ToString("g");
					break;
					//TimeCardManage
					case "adjevent":
					case "adjreg":
					case "adjotime":
					case "breaktime":
					case "temptotaltime":
					case "tempreghrs":
					case "tempovertime":
					if(_timeCardsUseDecilamInsteadOfColon) {
						_listQueryColumns[index].SortingStrategy = GridSortingStrategy.AmountParse;
						retVal=PIn.Time(dataString).TotalHours.ToString("n");
						break;
					}
					if(_timeCardShowSeconds) {//Colon format with seconds
						_listQueryColumns[index].SortingStrategy = GridSortingStrategy.TimeParse;
						retVal=PIn.Time(dataString).ToStringHmmss();
						break;
					}
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.TimeParse;
					retVal=PIn.Time(dataString).ToStringHmm();
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
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.AmountParse;
					retVal=PIn.Double(dataString).ToString("F");
					break;
					case "toothnum":
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.ToothNumberParse;
					retVal=Tooth.ToInternat(dataString);
					break;
					//definitions:
					case "adjtype":
					//fill list if empty
					retVal=Defs.GetName(DefCat.AdjTypes,PIn.Long(dataString),_listAdjTypes);
					break;
					case "confirmed":
					retVal=Defs.GetValue(DefCat.ApptConfirmed,PIn.Long(dataString));
					break;
					case "dx":
					retVal=Defs.GetName(DefCat.Diagnosis,PIn.Long(dataString),_listDiagnosis);
					break;
					case "discounttype":
					retVal=Defs.GetName(DefCat.DiscountTypes,PIn.Long(dataString),_listDiscountTypes);
					break;
					case "doccategory":
					retVal=Defs.GetName(DefCat.ImageCats,PIn.Long(dataString),_listImageCats);
					break;
					case "op":
					retVal=Operatories.GetAbbrev(PIn.Long(dataString));
					break;
					case "paytype":
					retVal=Defs.GetName(DefCat.PaymentTypes,PIn.Long(dataString),_listPaymentTypes);
					break;
					case "proccat":
					retVal=Defs.GetName(DefCat.ProcCodeCats,PIn.Long(dataString),_listProcCodeCats);
					break;
					case "unschedstatus":
					case "recallstatus":
					retVal=Defs.GetName(DefCat.RecallUnschedStatus,PIn.Long(dataString),_listRecallUnschedStatus);
					break;
					case "billingtype":
					retVal=Defs.GetName(DefCat.BillingTypes,PIn.Long(dataString),_listBillingTypes);
					break;
					//patnums:
					case "patnum":
					case "guarantor":
					case "pripatnum":
					case "secpatnum":
					case "subscriber":
					case "withpat":
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.StringCompare;
					if(_dictPatientNames==null) {
						_dictPatientNames=Patients.GetAllPatientNames();
					}
					long patNum=PIn.Long(dataString);
					if(_dictPatientNames.ContainsKey(patNum)) {
						retVal=_dictPatientNames[patNum];
						break;
					}
					_dictPatientNames[patNum]=Patients.GetLim(patNum).GetNameLF();
					retVal=_dictPatientNames[patNum];
					break;
					//plannums:
					case "plannum":
					case "priplannum":
					case "secplannum":
					if(_hashListPlans.ContainsKey(PIn.Long(dataString)))
						retVal=_hashListPlans[PIn.Long(dataString)].ToString();
					else
						retVal="";
					break;
					//referralnum
					case "referralnum":
					retVal=Referrals.TryGetReferral(PIn.Long(dataString),out Referral referral) ? "" : referral.LName+", "+referral.FName+" "+referral.MName;
					break;
					//enumerations:
					case "aptstatus":
					retVal=((ApptStatus)PIn.Long(dataString)).ToString();
					break;
					case "category":
					//There are several tables that share the same column name... do your best to determine which enum to use.
					if(_getDisplayFieldCategory) {
						retVal=((DisplayFieldCategory)PIn.Long(dataString)).ToString();
					}
					else {
						retVal=((DefCat)PIn.Long(dataString)).ToString();
					}
					break;
					case "renewmonth":
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.DateParse;
					retVal=((Month)PIn.Long(dataString)).ToString();
					break;
					case "patstatus":
					retVal=((PatientStatus)PIn.Long(dataString)).ToString();
					break;
					case "gender":
					retVal=((PatientGender)PIn.Long(dataString)).ToString();
					break;
					//case "lab":
					//	row[column.ColumnName]
					//		=((LabCaseOld)PIn.PInt(dataString)).ToString();
					//  break;
					case "position":
					retVal=((PatientPosition)PIn.Long(dataString)).ToString();
					break;
					case "deductwaivprev":
					case "flocovered":
					case "misstoothexcl":
					case "procstatus":
					retVal=((ProcStat)PIn.Long(dataString)).ToString();
					break;
					case "majorwait":
					case "hascaries":
					case "needssealants":
					case "cariesexperience":
					case "earlychildcaries":
					case "existingsealants":
					case "missingallteeth":
					retVal=((YN)PIn.Long(dataString)).ToString();
					break;
					case "prirelationship":
					case "secrelationship":
					retVal=((Relat)PIn.Long(dataString)).ToString();
					break;
					case "treatarea":
					retVal=((TreatmentArea)PIn.Long(dataString)).ToString();
					break;
					case "specialty":
					retVal=Defs.GetName(DefCat.ProviderSpecialties,PIn.Long(dataString));
					break;
					case "placeservice":
					retVal=((PlaceOfService)PIn.Long(dataString)).ToString();
					break;
					case "employrelated":
					retVal=((YN)PIn.Long(dataString)).ToString();
					break;
					case "schedtype":
					retVal=((ScheduleType)PIn.Long(dataString)).ToString();
					break;
					case "dayofweek":
					retVal=((DayOfWeek)PIn.Long(dataString)).ToString();
					break;
					case "raceOld":
					retVal=((PatientRaceOld)PIn.Long(dataString)).ToString();
					break;
					case "gradelevel":
					retVal=((PatientGrade)PIn.Long(dataString)).ToString();
					break;
					case "urgency":
					retVal=((TreatmentUrgency)PIn.Long(dataString)).ToString();
					break;
					case "reftype":
					retVal=((ReferralType)PIn.Int(dataString)).ToString();
					break;
					//miscellaneous:
					case "provnum":
					case "provhyg":
					case "priprov":
					case "secprov":
					case "provtreat":
					case "provbill":
					retVal=Providers.GetAbbr(PIn.Long(dataString));
					break;

					case "covcatnum":
					retVal=CovCats.GetDesc(PIn.Long(dataString));
					break;
					case "referringprov":
					//					  retVal=CovCats.GetDesc(PIn.PInt(dataString));
					break;
					case "addtime":
					if(dataString!="0")
						retVal+="0";
					break;
					case "feesched":
					case "feeschednum":
					retVal=FeeScheds.GetDescription(PIn.Long(dataString));
					break;
				}//end switch column caption
			}
			catch(Exception e) {
				e.DoNothing();
				return dataString;
			}
			return retVal;
		}

		///<summary>Used to submit user queries in a thread.  Column names will be handled automatically.
		///Set isSqlValidated to true in order to skip SQL saftey validation.</summary>
		public void SubmitQueryThreaded(bool isSqlValidated = false) {
			if(_threadQuery!=null || _serverThreadID!=0) {
				return;//There is already a query executing.
			}
			_getDisplayFieldCategory = _query.ToLower().Contains("displayfield");
			//Clear the grid
			ClearGrid();
			LayoutHelperForState(QueryExecuteState.Executing);
			_queryExceptionStateCur=QueryExceptionState.Throw;
			_threadQuery=new ODThread(OnThreadStart);
			_threadQuery.Name="UserQueryThread";
			_threadQuery.GroupName="UserQueryGroup";
			_threadQuery.AddExitHandler(OnThreadExit);
			_threadQuery.AddExceptionHandler(ExceptionThreadHandler);
			_threadQuery.Start(true); //run it once.
		}

		private void FillDataFromQuery(DataTable table) {
			_listRawData.Clear();
			_listQueryColumns.Clear();
			bool hasPatNum=false;
			foreach(DataColumn column in table.Columns) {
				_listQueryColumns.Add(new QueryColumn(column.ColumnName));
				if(!hasPatNum && column.ColumnName.ToLower() == "patnum") {
					hasPatNum=true;
				}
			}
			if(hasPatNum) {
				if(!_gridResults.ContextMenu.MenuItems.Contains(_menuItemGoToPatient)) {
					_gridResults.ContextMenu.MenuItems.Add(_menuItemGoToPatient);
				}
			}
			else {
				_gridResults.ContextMenu.MenuItems.Remove(_menuItemGoToPatient);
			}
			foreach(DataRow row in table.AsEnumerable()) {
				_listRawData.Add(row.ItemArray.AsEnumerable().Select(x => PIn.ByteArray(x)).ToList());
			}
		}

		public void SetQuery(string query) {
			_query=query;
		}

		private void MenuItemGoToPatient_Click(object sender, EventArgs e) {
			Patient patient = new Patient();
			if(long.TryParse(_gridResults.SelectedTag<string>(),out long patNum)) {
				patient = Patients.GetPat(patNum);
			}
			if(patient.PatNum != 0) {
				FormOpenDental.S_Contr_PatientSelected(patient,true);
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
			if(_listRawData==null) {
				LayoutHelperForState(QueryExecuteState.Idle);
				return;
			}
			Cursor=Cursors.WaitCursor;
			LayoutHelperForState(QueryExecuteState.Formatting);
			ClearGrid();
			FillGrid(radioRaw.Checked);
			SecurityLogs.MakeLogEntry(Permissions.UserQuery,0,textQuery.Text);
			LayoutHelperForState(QueryExecuteState.Idle);
			Cursor=Cursors.Default;
		}

		///<summary>Sets the controls on the form to reflect the execute state passed in.  Typically used to disable all controls while executing.</summary>
		private void LayoutHelperForState(QueryExecuteState executeState) {
			switch(executeState) {
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
				butExport.Enabled=true;
				butPrintPreview.Enabled=true;
				butPrint.Enabled=true;
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
				butExport.Enabled=false;
				butPrintPreview.Enabled=false;
				butPrint.Enabled=false;
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
				butExport.Enabled=false;
				butPrintPreview.Enabled=false;
				butPrint.Enabled=false;
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
			FillDataFromQuery(DataConnectionCancelable.GetTableConAlreadyOpen(_serverThreadID,_query
				,false,checkReportServer.Checked,hasStackTrace,suppressMessage:true));
		}

		private bool UserHasStackTracePref() {
			UserOdPref pref = UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum, UserOdFkeyType.QueryMonitorHasStackTraces);
			if(pref.IsNew) {
				pref.ValueString="0";
				UserOdPrefs.Insert(pref);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
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

		#endregion Methods

		private void checkWordWrap_CheckedChanged(object sender,EventArgs e) {
			FillForm();
		}

		private void checkNumberedRows_Clicked(object sender,EventArgs e) {
			FillForm();
		}

		private void comboAlignment_SelectionChangeCommitted(object sender, EventArgs e) {
			if(comboAlignment.GetSelected<HorizontalAlignment>() != _alignmentCur) {
				_alignmentCur = comboAlignment.GetSelected<HorizontalAlignment>();
				FillForm();
			}
		}


	}
}

public class QueryColumn {
	public string Name;
	public int Width;
	public bool ShowTotal;
	public double Total;
	public GridSortingStrategy SortingStrategy;
	public QueryColumn() {
		//constructor
	}

	public QueryColumn(string name) {
		Name = name;
		SortingStrategy = GridSortingStrategy.StringCompare;
	}
}
