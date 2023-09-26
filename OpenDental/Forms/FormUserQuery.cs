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
		///<summary>A dictionary of patient names that only gets filled once and only if necessary. Jordan OK.</summary>
		private static Dictionary<long,string> _dictionaryPatientNames;
		//private static List<long> _listPatNums;
		///<summary>A dictionary of key: InsPlanNum, value: Carrier Name. Jordan OK.</summary>
		private static Dictionary<long, string> _dictionaryPlanCarrier;
		///<summary>Tells the form whether to throw, show (interrupt), or suppress exceptions. Set manually in the code below.</summary>
		private QueryExceptionState _queryExceptionState;
		private string _query;
		///<summary>The server thread id that the current query is running on.</summary>
		private int _serverThreadID;
		private bool _doSubmitOnLoad;
		private List<List<string>> _listListsRawData;
		///<summary>User queries run on a separate thread so they can be cancelled.</summary>
		private ODThread _odThreadQuery;
		private UserQuery _userQuery;
		private static List<Def> _listDefsAdjTypes;
		private static List<Def> _listDefsDiagnosis;
		private static List<Def> _listDefsDiscountTypes;
		private static List<Def> _listDefsImageCats;
		private static List<Def> _listDefsPaymentTypes;
		private static List<Def> _listDefsProcCodeCats;
		private static List<Def> _listDefsRecallUnschedStatus;
		private static List<Def> _listDefsBillingTypes;
		private static List<QueryColumn> _listQueryColumns;
		private static bool _timeCardShowSeconds;
		private static bool _timeCardsUseDecimalInsteadOfColon;
		private static bool _getDisplayFieldCategory;
		private static List<string> _listColumnsRightAlign = new List<string>{"adjamt", "monthbalance", "claimfee", "inspayest", "inspayamt", "dedapplied", "amount", "payamt", "splitamt", "balance", "procfee", "overridepri", "overridesec", "priestim", "secestim", "procfees", "claimpays", "insest", "paysplits", "adjustments", "bal_0_30", "bal_31_60", "bal_61_90", "balover90", "baltotal", "inswoest" };
		private int _pagesPrinted;
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		private HorizontalAlignment _horizontalAlignment;
		private MenuItem _menuItemGoToPatient;

		#endregion Fields

		#region Constructor
		public FormUserQuery(string query = "",bool submitQueryOnLoad = false,UserQuery userQuery=null) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			this.DoubleBuffered=true;
			_query = query;
			_doSubmitOnLoad=submitQueryOnLoad;
			if(userQuery!=null) {
				_userQuery=userQuery;
			}
		}
		#endregion Constructor

		#region Events
		private void FormQuery_Load(object sender,System.EventArgs e) {
			splitContainer.SplitterDistance=LayoutManager.Scale(140);
			comboAlignment.Items.AddEnums<HorizontalAlignment>();
			comboAlignment.SetSelected(0);
			_horizontalAlignment=comboAlignment.GetSelected<HorizontalAlignment>();
			textQuery.Text = _query;
			if(!Security.IsAuthorized(Permissions.UserQueryAdmin,true)) {
				//lock the query textbox, add button, and paste button for users without permissions.
				textQuery.ReadOnly=true;
				butAdd.Enabled=false;
				butPaste.Enabled=false;
			}
			if(!string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ReportingServerCompName))
				|| !string.IsNullOrWhiteSpace(PrefC.GetString(PrefName.ReportingServerURI)))
			{
				//default to report server when one is set up.
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
			_listListsRawData = new List<List<string>>();
			_listQueryColumns = new List<QueryColumn>();
			_listDefsRecallUnschedStatus = Defs.GetDefsForCategory(DefCat.RecallUnschedStatus);
			_listDefsProcCodeCats = Defs.GetDefsForCategory(DefCat.ProcCodeCats);
			_listDefsPaymentTypes = Defs.GetDefsForCategory(DefCat.PaymentTypes);
			_listDefsImageCats = Defs.GetDefsForCategory(DefCat.ImageCats);
			_listDefsDiscountTypes =Defs.GetDefsForCategory(DefCat.DiscountTypes);
			_listDefsDiagnosis = Defs.GetDefsForCategory(DefCat.Diagnosis);
			_listDefsAdjTypes = Defs.GetDefsForCategory(DefCat.AdjTypes);
			_listDefsBillingTypes = Defs.GetDefsForCategory(DefCat.BillingTypes);
			_dictionaryPlanCarrier=InsPlans.GetDictPlanCarrier();
			_timeCardShowSeconds = PrefC.GetBool(PrefName.TimeCardShowSeconds);
			_timeCardsUseDecimalInsteadOfColon = PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon);
			if(_doSubmitOnLoad) {
				//Coming from FormOpenDental menu item click for query favorites.
				//Existence in this list is taken to mean sql in these queries is considered safe to run.
				bool isAllowed=false;
				try {
					isAllowed=Db.IsSqlAllowed(textQuery.Text);
				}
				catch (Exception ex) {
					MsgBox.Show(this,ex.Message);
					return;
				}
				if(isAllowed) {
					SubmitQueryThreaded(true);
				}
			}
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			using FormQueryEdit formQueryEdit=new FormQueryEdit();
			formQueryEdit.UserQueryCur=new UserQuery();
			formQueryEdit.UserQueryCur.QueryText=textQuery.Text;
			formQueryEdit.IsNew=true;
			formQueryEdit.ShowDialog();
			if(formQueryEdit.DialogResult==DialogResult.OK) {
				textQuery.Text=formQueryEdit.UserQueryCur.QueryText;
				_gridResults.Title=formQueryEdit.UserQueryCur.Description;
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
			using FormQueryFavorites formQueryFavorites=new FormQueryFavorites();
			formQueryFavorites.UserQueryCur=_userQuery;
			formQueryFavorites.ShowDialog();
			if(formQueryFavorites.DialogResult!=DialogResult.OK) {
				return;
			}
			textQuery.Text=formQueryFavorites.UserQueryCur.QueryText;
			textTitle.Text=formQueryFavorites.UserQueryCur.Description;
			_userQuery=formQueryFavorites.UserQueryCur;
			_query=formQueryFavorites.UserQueryCur.QueryText;
			SubmitQueryThreaded();
		}

		private void butPaste_Click(object sender,System.EventArgs e) {
			IDataObject iDataObject;
			try {
				if(ODBuild.IsWeb()) {
					textQuery.Text=ODClipboard.GetText();
					return;
				}
				iDataObject=Clipboard.GetDataObject();
			}
			catch(Exception ex) {
				MsgBox.Show(this,"Could not paste contents from the clipboard.  Please try again.");
				ex.DoNothing();
				return;
			}
			if(iDataObject.GetDataPresent(DataFormats.Text)) {
				textQuery.Text=(String)iDataObject.GetData(DataFormats.Text);
			}
			else {
				MessageBox.Show(Lan.g(this,"Could not retrieve data off the clipboard."));
			}
		}

		private void butSubmit_Click(object sender,System.EventArgs e) {
			if(butSubmit.Text==Lan.g(this,"Stop Execution")) {
				//Flag the currently running query to stop.
				if(_odThreadQuery!=null) {
					_odThreadQuery.QuitAsync(true);
				}
				//Cancel the query that is currently running on MySQL using a KILL command.
				if(_serverThreadID!=0) {
					_queryExceptionState = QueryExceptionState.Interrupt;
					DataConnectionCancelable.CancelQuery(_serverThreadID,useReportServer:checkReportServer.Checked);
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
			_queryExceptionState=QueryExceptionState.Suppress;
			//Try to cancel any queries that could be running right now.
			if(_serverThreadID!=0) {
				DataConnectionCancelable.CancelQuery(_serverThreadID,hasExceptions:false,useReportServer:checkReportServer.Checked);
			}
			ODThread.QuitAsyncThreadsByGroupName("UserQueryGroup",true);
		}

		private void FormQuery_Layout(object sender,System.Windows.Forms.LayoutEventArgs e) {
			//don't layout controls here.  It's far too frequent
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(_gridResults.ListGridRows.Count == 0 && _gridResults.Columns.Count == 0) {
				MsgBox.Show(MessageBoxButtons.OK,Lan.g(this,"No report to print. Please run a query."));
				return;
			}
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(
				pd_PrintPage,
				Lan.g(this,$"{(String.IsNullOrEmpty(textTitle.Text) ? "User Query" : textTitle.Text)} printed")
			);
		}

		private void butPrintPreview_Click(object sender,EventArgs e) {
			if(_gridResults.ListGridRows.Count == 0 && _gridResults.Columns.Count == 0) {
				MsgBox.Show(MessageBoxButtons.OK,Lan.g(this,"No report to preview. Please run a query."));
				return;
			}
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(
				pd_PrintPage,
				Lan.g(this,$"{(String.IsNullOrEmpty(textTitle.Text) ? "User Query" : textTitle.Text)} previewed"),
				isForcedPreview:true
			);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,textTitle.Text);
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				//print today's date
				text = Lan.g(this,"Run On:")+" "+DateTime.Today.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=_gridResults.PrintPage(g,_pagesPrinted,rectangleBounds,_heightHeadingPrint);
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
			string filePath;
			saveFileDialog2=new SaveFileDialog {
				AddExtension=true,
				Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*",
				FilterIndex=0
			};
			if(_userQuery==null || _userQuery.FileName==null || _userQuery.FileName==""){//.FileName==null)
				saveFileDialog2.FileName=textTitle.Text;
			}
			else{
				saveFileDialog2.FileName=_userQuery.FileName;
			}
			if(ODBuild.IsWeb()) {
				if(saveFileDialog2.ShowDialog()!=DialogResult.OK) { 
					return;
				}
				if(saveFileDialog2.FileName.IsNullOrEmpty()) {
					MsgBox.Show("Failed to save the file.");
					return;
				}
				filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),saveFileDialog2.FileName.Split('\\').Last());
			}
			else {
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
				if(saveFileDialog2.ShowDialog()!=DialogResult.OK) {
					saveFileDialog2.Dispose();
					return;
				}
				filePath=saveFileDialog2.FileName;
				saveFileDialog2.Dispose();
			}
			using StreamWriter streamWriter = new StreamWriter(filePath,false);
			//new FileStream(,FileMode.Create,FileAccess.Write,FileShare.Read)))
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
			try {
				streamWriter.WriteLine(line);
			}
			catch {
				MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
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
				try {
					streamWriter.WriteLine(line);
				}
				catch {
					MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
					return;
				}
			}
			streamWriter.Close();
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
			labelRowCount.Text = $"Row Count: {_listListsRawData.Count}";
			//gets base widths for columns
			for(int i = 0;i<_listQueryColumns.Count;i++) {
				_listQueryColumns[i].Width = TextRenderer.MeasureText(_listQueryColumns[i].Name,_gridResults.Font).Width + 10;
			}
			List<HorizontalAlignment> listColumnAligments = new List<HorizontalAlignment>(_listQueryColumns.Count);
			for(int i = 0;i<_listQueryColumns.Count;i++) {
				_listQueryColumns[i].Total=0;
			}
			string valueChanged = "";
			_gridResults.BeginUpdate();
			_gridResults.Columns.Clear();
			_gridResults.ListGridRows.Clear();
			//fill rows and calculate widths for columns based on row data
			int rowCount = 1;//row count always starts at 1
			float localZoom=LayoutManager.ScaleMy();//get the current zoom factor for the local computer
			for(int r = 0;r<_listListsRawData.Count;r++) {
				UI.GridRow gridRow = new UI.GridRow();
				if(checkNumberedRows.Checked) {//add numbered rows if selected
					gridRow.Cells.Add(new UI.GridCell(rowCount++.ToString()));//increment rowCount after filling cell
				}
				for(int c = 0;c<_listListsRawData[r].Count;c++) {
					string value = _listListsRawData[r][c];
					//replace data such as PatNums numbers based on column names if viewing Human Readable mode
					valueChanged = value;
					if (!showRawData) {
						try {
							valueChanged = ReplaceColumnString(c,value);
						}
						catch(FormatException) {
							// We expected that the data was in a particular format for a reserved column name, but it was in an unexpected format.
							// Don't apply special formatting in this case.
						}
					}
					if(_listQueryColumns[c].Name.ToLower() == "patnum") {
						gridRow.Tag = value;
					}
					gridRow.Cells.Add(new UI.GridCell(valueChanged));
					//get widest data in column, used to set column width later. Max column width of 400 pixels (word wrap can be turned on if more space is needed)
					//we also need to remove the zoom component from the text width calculation since this has already been factored in when resizing the grid intially
					int textWidth=(int)(TextRenderer.MeasureText(valueChanged,_gridResults.Font).Width/localZoom)+10;
					_listQueryColumns[c].Width=Math.Min(Math.Max(_listQueryColumns[c].Width,textWidth),400);
					AddToColumnTotal(c,valueChanged);
				}
				_gridResults.ListGridRows.Add(gridRow);
			}
			if(_listQueryColumns.Any(x => x.ShowTotal)) {
				UI.GridRow totalsRow = new UI.GridRow();
				if(checkNumberedRows.Checked) {
					totalsRow.Cells.Add(new UI.GridCell());
				}
				for(int i = 0;i<_listQueryColumns.Count;i++) {
					totalsRow.Cells.Add(new UI.GridCell(_listQueryColumns[i].ShowTotal ? _listQueryColumns[i].Total.ToString("F") : ""));
					_listQueryColumns[i].Width = Math.Min(Math.Max(_listQueryColumns[i].Width,TextRenderer.MeasureText(_listQueryColumns[i].Total.ToString("F"),_gridResults.Font).Width + 10),400);//have to add a little extra to fit the full total sometimes
				}
				_gridResults.ListGridRows.Add(totalsRow);
			}
			if(checkNumberedRows.Checked) {
				_gridResults.Columns.Add(new UI.GridColumn("#",60,comboAlignment.GetSelected<HorizontalAlignment>(),sortingStrategy: UI.GridSortingStrategy.AmountParse));
			}
			for(int i = 0;i<_listQueryColumns.Count;i++) {
				GridColumn col = new GridColumn();
				col.Heading=_listQueryColumns[i].Name;
				col.ColWidth=_listQueryColumns[i].Width; //gets width as determined by the rows
				col.TextAlign=GetAlignment(_listQueryColumns[i].Name);
				col.SortingStrategy=_listQueryColumns[i].SortingStrategy;
				_gridResults.Columns.Add(col);
			}
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
			if(value[0] == '$' || _listColumnsRightAlign.Contains(value.ToLower()) || value.All(x => char.IsDigit(x))) {
				return HorizontalAlignment.Right;
			}
			return comboAlignment.GetSelected<HorizontalAlignment>();
		}

		//Takes in a column name and a string to be replace. Returns a transformed string based on the column it appears in.
		private static string ReplaceColumnString(int index,string dataString) {
			string columnName = _listQueryColumns[index].Name;
			string retVal = "";
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
				case "adjdate"://date. Some of these are actually handled further up.
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
				case "birthdateforage"://age
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
					if(_timeCardsUseDecimalInsteadOfColon) {
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
					retVal=Tooth.Display(dataString);
					break;
				//definitions:
				case "adjtype":
					//fill list if empty
					retVal=Defs.GetName(DefCat.AdjTypes,PIn.Long(dataString),_listDefsAdjTypes);
					break;
				case "confirmed":
					retVal=Defs.GetValue(DefCat.ApptConfirmed,PIn.Long(dataString));
					break;
				case "dx":
					retVal=Defs.GetName(DefCat.Diagnosis,PIn.Long(dataString),_listDefsDiagnosis);
					break;
				case "discounttype":
					retVal=Defs.GetName(DefCat.DiscountTypes,PIn.Long(dataString),_listDefsDiscountTypes);
					break;
				case "doccategory":
					retVal=Defs.GetName(DefCat.ImageCats,PIn.Long(dataString),_listDefsImageCats);
					break;
				case "op":
					retVal=Operatories.GetAbbrev(PIn.Long(dataString));
					break;
				case "paytype":
					retVal=Defs.GetName(DefCat.PaymentTypes,PIn.Long(dataString),_listDefsPaymentTypes);
					break;
				case "proccat":
					retVal=Defs.GetName(DefCat.ProcCodeCats,PIn.Long(dataString),_listDefsProcCodeCats);
					break;
				case "unschedstatus":
				case "recallstatus":
					retVal=Defs.GetName(DefCat.RecallUnschedStatus,PIn.Long(dataString),_listDefsRecallUnschedStatus);
					break;
				case "billingtype":
					retVal=Defs.GetName(DefCat.BillingTypes,PIn.Long(dataString),_listDefsBillingTypes);
					break;
				//patnums:	If you add to the following list of cases, make sure to update the list found in
				//					"private void ExecuteQuery(bool useReportServer,bool wasSqlValidated)".
				case "patnum":
				case "guarantor":
				case "pripatnum":
				case "secpatnum":
				case "subscriber":
				case "withpat":
					_listQueryColumns[index].SortingStrategy = GridSortingStrategy.StringCompare;
					if(_dictionaryPatientNames==null) {
						try {
							_dictionaryPatientNames=Patients.GetDictAllPatientNames();
						}
						catch(Exception e) {
							e.DoNothing();
							return dataString;
						}
					}
					long patNum=PIn.Long(dataString);
					if(_dictionaryPatientNames.ContainsKey(patNum)) {
						retVal=_dictionaryPatientNames[patNum];
						break;
					}
					_dictionaryPatientNames[patNum]=Patients.GetLim(patNum).GetNameLF();
					retVal=_dictionaryPatientNames[patNum];
					break;
				//plannums:
				case "plannum":
				case "priplannum":
				case "secplannum":
					if(_dictionaryPlanCarrier.ContainsKey(PIn.Long(dataString)))
						retVal=_dictionaryPlanCarrier[PIn.Long(dataString)].ToString();
					else
						retVal="";
					break;
				//referralnum
				case "referralnum":
					try {
						Referral referral=Referrals.GetReferral(PIn.Long(dataString));
						retVal=referral.LName+", "+referral.FName+" "+referral.MName;
					}
					catch(Exception e) {
						e.DoNothing();
						return dataString;
					}
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
					//retVal=CovCats.GetDesc(PIn.PInt(dataString));
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
			return retVal;
		}

		///<summary>Used to submit user queries in a thread.  Column names will be handled automatically.
		///Set wasSqlValidated to true in order to skip SQL saftey validation if the query has already been vetted.</summary>
		public void SubmitQueryThreaded(bool wasSqlValidated=false) {
			if(_odThreadQuery!=null || _serverThreadID!=0) {
				return;//There is already a query executing.
			}
			_getDisplayFieldCategory = _query.ToLower().Contains("displayfield");
			//Clear the grid
			ClearGrid();
			LayoutHelperForState(QueryExecuteState.Executing);
			_queryExceptionState=QueryExceptionState.Throw;
			_odThreadQuery=new ODThread((o) => ExecuteQuery(checkReportServer.Checked,wasSqlValidated));
			_odThreadQuery.Name="UserQueryThread";
			_odThreadQuery.GroupName="UserQueryGroup";
			_odThreadQuery.AddExitHandler(OnThreadExit);
			_odThreadQuery.AddExceptionHandler(ExceptionThreadHandler);
			_odThreadQuery.Start(true); //run it once.
		}

		private void FillDataFromQuery(DataTable table) {
			_listListsRawData.Clear();
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
				_listListsRawData.Add(row.ItemArray.AsEnumerable().Select(x => PIn.ByteArray(x)).ToList());
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
				switch(_queryExceptionState) {
					case QueryExceptionState.Interrupt:
						_queryExceptionState=QueryExceptionState.Suppress;
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
			if(_listListsRawData==null) {
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
		private void LayoutHelperForState(QueryExecuteState queryExecuteState) {
			switch(queryExecuteState) {
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

		private void ExecuteQuery(bool useReportServer,bool wasSqlValidated) {
			bool hasStackTrace=UserHasStackTracePref();
			_serverThreadID=DataConnectionCancelable.GetServerThread(useReportServer,false);
			DataTable table=DataConnectionCancelable.GetTableConAlreadyOpen(_serverThreadID,_query,wasSqlValidated,useReportServer,hasStackTrace,suppressMessage:true,useReportServer:useReportServer);
			/*
			List<string> listTemp = new List<string>() { "patnum", "guarantor", "pripatnum", "secpatnum", "subscriber", "withpat" };
			_listPatNums=new List<long>();
			for (int i=0;i<table.Columns.Count;i++) {
				if(!listTemp.Contains(table.Columns[i].ColumnName.ToLower())) {
					continue;
				}
				for (int j = 0;j<table.Rows.Count;j++) {
					long patNum=PIn.Long(table.Rows[i][j].ToString());
					if(_listPatNums.Contains(patNum)) {
						continue;
					}
					_listPatNums.Add(patNum);
				}
			}*/
			FillDataFromQuery(table);
		}

		private bool UserHasStackTracePref() {
			UserOdPref userodPref = UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum, UserOdFkeyType.QueryMonitorHasStackTraces);
			if(userodPref.IsNew) {
				userodPref.ValueString="0";
				UserOdPrefs.Insert(userodPref);
				DataValid.SetInvalid(InvalidType.UserOdPrefs);
				return false;
			}
			if(userodPref.ValueString=="1") {
				return true;
			}
			return false;
		}

		///<summary>_userQuery exitHandler. Fills the form if the form is not disposed.</summary>
		private void OnThreadExit(ODThread odThread) {
			try {
				_odThreadQuery=null;
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
			if(comboAlignment.GetSelected<HorizontalAlignment>() != _horizontalAlignment) {
				_horizontalAlignment = comboAlignment.GetSelected<HorizontalAlignment>();
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