using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormJournal:FormODBase {
		private Account _account;
		private bool _headingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;
		private int _prevGridWidth;
		private List<JournalEntry> _listJournalEntry;
		private Dictionary<long,long> _dictionaryTransUsers;
		private OpenDental.UI.GridOD _gridMainPrint;

		//set this externally so that the ending balances will match what's showing in the Chart of Accounts.
		public DateTime InitialAsOfDate;

		///<summary></summary>
		public FormJournal(Account account)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_account=account;
		}

		private void FormJournal_Load(object sender,EventArgs e) {
			DateTime dateFirstofYear=new DateTime(InitialAsOfDate.Year,1,1);
			textDateTo.Text=InitialAsOfDate.ToShortDateString();
			if(_account.AcctType==AccountType.Income || _account.AcctType==AccountType.Expense){
				textDateFrom.Text=dateFirstofYear.ToShortDateString();
			}
			LayoutToolBar();
			FillGrid();
			gridMain.ScrollToEnd();
			_prevGridWidth=gridMain.Width;
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add Entry"),0,"","Add"));
			if(_account.AcctType==AccountType.Asset){
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Reconcile"),-1,"","Reconcile"));
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),1,"","Print"));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Edit"),-1,Lan.g(this,"Edit Selected Account"),"Edit"));
			//ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			//button.Style=ODToolBarButtonStyle.Label;
			//ToolBarMain.Buttons.Add(button);
			//ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Export"),2,Lan.g(this,"Export the Account Grid"),"Export"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
		}

		private void ToolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Add":
					Add_Click();
					break;
				case "Reconcile":
					Reconcile_Click();
					break;
				case "Print":
					//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
					//when it comes from a toolbar click.
					//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
					ToolBarClick toolBarClick=Print_Click;
					this.BeginInvoke(toolBarClick);
					break;
				case "Export":
					Export_Click();
					break;
				case "Close":
					this.Close();
					break;
			}
		}

		private delegate void ToolBarClick();

		private void FillGrid(bool isPrinting=false,bool doRefresh=true) {
			if(!textDateFrom.IsValid() || !textDateTo.IsValid()) {
				return;
			}
			GridOD gridToFill=isPrinting?_gridMainPrint:gridMain;
			gridToFill.BeginUpdate();
			gridToFill.Title=_account.Description+" ("+Lan.g("enumAccountType",_account.AcctType.ToString())+")";
			gridToFill.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableJournal","Chk #"),60,HorizontalAlignment.Center);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Date"),70,HorizontalAlignment.Left);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Memo"),isPrinting?200:220,HorizontalAlignment.Left);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Splits"),isPrinting?200:220,HorizontalAlignment.Left);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Debit"+(Accounts.DebitIsPos(_account.AcctType)?"(+)":"(-)")),70,HorizontalAlignment.Right);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Credit"+(Accounts.DebitIsPos(_account.AcctType)?"(-)":"(+)")),70,HorizontalAlignment.Right);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Balance"),78,HorizontalAlignment.Right);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Created By"),95,HorizontalAlignment.Left);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Last Edited By"),95,HorizontalAlignment.Left);
			gridToFill.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Clear"),40,HorizontalAlignment.Center);
			gridToFill.Columns.Add(col);
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=string.IsNullOrEmpty(textDateTo.Text)?DateTime.MaxValue:PIn.Date(textDateTo.Text);
			double filterAmtFrom=textAmountFrom.IsValid()?PIn.Double(textAmountFrom.Text):0;
			double filterAmtTo=textAmountTo.IsValid()?PIn.Double(textAmountTo.Text):0;
			if(doRefresh || _listJournalEntry==null || _dictionaryTransUsers==null) {
				_listJournalEntry=JournalEntries.GetForAccount(_account.AccountNum);
				_dictionaryTransUsers=Transactions.GetManyTrans(_listJournalEntry.Select(x => x.TransactionNum).ToList())
					.ToDictionary(x => x.TransactionNum,x => x.UserNum);
			}
			gridToFill.ListGridRows.Clear();
			GridRow row;
			decimal bal=0;
			for(int i=0;i<_listJournalEntry.Count;i++) {
				if(_listJournalEntry[i].DateDisplayed>dateTo) {
					break;
				}
				if(new[] { AccountType.Income,AccountType.Expense }.Contains(_account.AcctType) && _listJournalEntry[i].DateDisplayed<dateFrom) {
					continue;//For income and expense accounts, previous balances are not included. Only the current timespan
				}
				//DebitIsPos=true for checking acct, bal+=DebitAmt-CreditAmt
				bal+=(Accounts.DebitIsPos(_account.AcctType)?1:-1)*((decimal)_listJournalEntry[i].DebitAmt-(decimal)_listJournalEntry[i].CreditAmt);
				if(new[] { AccountType.Asset,AccountType.Liability,AccountType.Equity }.Contains(_account.AcctType) && _listJournalEntry[i].DateDisplayed<dateFrom) {
					continue;//For asset, liability, and equity accounts, older entries do affect the current balance
				}
				if(string.IsNullOrEmpty(textAmountTo.Text)) {//The AmountFrom textBox will be treated as an exact amount, not a range.
					if(filterAmtFrom!=0 && filterAmtFrom!=_listJournalEntry[i].CreditAmt && filterAmtFrom!=_listJournalEntry[i].DebitAmt){
						continue;
					}
				}
				else {//AmountTo textBox has a value, treat this as a range.
					//If the first column's amount is not between the range and the second column is set to 0 or if the second column's amount is not between the range and the first column is set to 0 do not show this specific row.
					if(!_listJournalEntry[i].CreditAmt.Between(filterAmtFrom,filterAmtTo,true,true) && _listJournalEntry[i].DebitAmt==0 
						|| !_listJournalEntry[i].DebitAmt.Between(filterAmtFrom,filterAmtTo,true,true) && _listJournalEntry[i].CreditAmt==0) 
					{
						continue;
					}
				}
				if(textFindText.Text!="" && new[] { _listJournalEntry[i].Memo,_listJournalEntry[i].CheckNumber,_listJournalEntry[i].Splits }.All(x => !x.ToUpper().Contains(textFindText.Text.ToUpper()))) {
					continue;//For "Find Text" textbox
				}
				row=new GridRow();
				row.Cells.Add(_listJournalEntry[i].CheckNumber);
				row.Cells.Add(_listJournalEntry[i].DateDisplayed.ToShortDateString());
				row.Cells.Add(_listJournalEntry[i].Memo);
				row.Cells.Add(_listJournalEntry[i].Splits);
				row.Cells.Add(_listJournalEntry[i].DebitAmt==0?"":_listJournalEntry[i].DebitAmt.ToString("n"));
				row.Cells.Add(_listJournalEntry[i].CreditAmt==0?"":_listJournalEntry[i].CreditAmt.ToString("n"));
				row.Cells.Add(bal.ToString("n"));
				long userNum;
				row.Cells.Add(Userods.GetName(_dictionaryTransUsers.TryGetValue(_listJournalEntry[i].TransactionNum,out userNum)?userNum:0));
				row.Cells.Add(Userods.GetName(_listJournalEntry[i].SecUserNumEdit));
				row.Cells.Add(_listJournalEntry[i].ReconcileNum==0?"":"X");
				row.Tag=_listJournalEntry[i].TransactionNum;
				gridToFill.ListGridRows.Add(row);
			}
			gridToFill.EndUpdate();
		}

		private void Add_Click(){
			Transaction transaction=new Transaction();
			transaction.UserNum=Security.CurUser.UserNum;
			Transactions.Insert(transaction);//we now have a TransactionNum, and datetimeEntry has been set
			using FormTransactionEdit formTransactionEdit=new FormTransactionEdit(transaction.TransactionNum,_account.AccountNum);
			formTransactionEdit.IsNew=true;
			formTransactionEdit.ShowDialog();
			if(formTransactionEdit.DialogResult==DialogResult.Cancel){
				//no need to try-catch, since no journal entries were saved.
				Transactions.Delete(transaction);
			}
			FillGrid();
		}

		private void Reconcile_Click() {
			int selectedRow=gridMain.GetSelectedIndex();
			int scrollValue=gridMain.ScrollValue;
			using FormReconciles formReconciles=new FormReconciles(_account.AccountNum);
			formReconciles.ShowDialog();
			FillGrid();
			gridMain.SetSelected(selectedRow,true);
			gridMain.ScrollValue=scrollValue;
		}

		private void Print_Click(){
			_pagesPrinted=0;
			_headingPrinted=false;
			_gridMainPrint=new GridOD() { Width=1050,TranslationName="tableJournal"};
			FillGrid(isPrinting:true,doRefresh:false);
			PrintoutOrientation orient=PrintoutOrientation.Default;
			if(_gridMainPrint.Columns.WidthAll()>800) {
				orient=PrintoutOrientation.Landscape;
			}
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPage,
				Lan.g(this,"Accounting transaction history for")+" "+_account.Description+" "+Lan.g(this,"printed"),
				printoutOrientation:orient
			);
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			//Rectangle bounds=new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			using(Graphics g = e.Graphics)
			using(Font fontHeading=new Font("Arial",13,FontStyle.Bold))
			using(Font fontSubHeading=new Font("Arial",10,FontStyle.Bold)) {
				string text;
				int yPos=rectangleBounds.Top;
				int center=rectangleBounds.X+rectangleBounds.Width/2;
				#region printHeading
				if(!_headingPrinted) {
					text=_account.Description+" ("+Lan.g("enumAccountType",_account.AcctType.ToString())+")";
					g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
					yPos+=(int)g.MeasureString(text,fontHeading).Height;
					text=DateTime.Today.ToShortDateString();
					g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
					yPos+=20;
					_headingPrinted=true;
					_headingPrintH=yPos;
				}
				#endregion
				yPos=_gridMainPrint.PrintPage(g,_pagesPrinted,rectangleBounds,_headingPrintH);
				_pagesPrinted++;
				if(yPos==-1) {
					e.HasMorePages=true;
				}
				else {
					e.HasMorePages=false;
				}
			}
		}

		private void Export_Click() {
			//FillGrid();//this is in case the date range has been changed but the grid has not been refreshed to reflect the new date range, so the date range on the report will match the data in the grid.
			//List<Tuple<string,string>> listOtherDetails=new List<Tuple<string,string>>() {
			//	Tuple.Create(labelDateFrom.Text,PIn.Date(textDateFrom.Text).ToShortDateString()),
			//	Tuple.Create(labelDateTo.Text,PIn.Date(textDateTo.Text).ToShortDateString())
			//};
			gridMain.Export(gridMain.Title);//listOtherDetails:listOtherDetails);
			//if(!string.IsNullOrEmpty(msg)) {
			//	MsgBox.Show(this,msg);
			//}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int selectedRow=e.Row;
			int scrollValue=gridMain.ScrollValue;
			using FormTransactionEdit formTransactionEdit=new FormTransactionEdit((long)gridMain.ListGridRows[e.Row].Tag,_account.AccountNum);
			formTransactionEdit.ShowDialog();
			if(formTransactionEdit.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillGrid();
			gridMain.SetSelected(selectedRow,true);
			gridMain.ScrollValue=scrollValue;
		}

		private void butDropFrom_Click(object sender,EventArgs e) {
			ToggleCalendars();
		}

		private void butDropTo_Click(object sender,EventArgs e) {
			ToggleCalendars();
		}

		private void ToggleCalendars(){
			if(calendarFrom.Visible){
				//hide the calendars
				calendarFrom.Visible=false;
				calendarTo.Visible=false;
				FillGrid(doRefresh:false);
			}
			else{
				//set the date on the calendars to match what's showing in the boxes
				if(textDateFrom.IsValid() && textDateTo.IsValid()) {//if no date errors
					if(textDateFrom.Text==""){
						calendarFrom.SetDate(DateTime.Today);
					}
					else{
						calendarFrom.SetDate(PIn.Date(textDateFrom.Text));
					}
					if(textDateTo.Text=="") {
						calendarTo.SetDate(DateTime.Today);
					}
					else {
						calendarTo.SetDate(PIn.Date(textDateTo.Text));
					}
				}
				//show the calendars
				calendarFrom.Visible=true;
				calendarTo.Visible=true;
			}
		}

		private void calendarFrom_DateSelected(object sender,DateRangeEventArgs e) {
			textDateFrom.Text=calendarFrom.SelectionStart.ToShortDateString();
		}

		private void calendarTo_DateSelected(object sender,DateRangeEventArgs e) {
			textDateTo.Text=calendarTo.SelectionStart.ToShortDateString();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			if(!textDateFrom.IsValid()
				|| !textDateTo.IsValid()
				|| !textAmountFrom.IsValid()
				|| !textAmountTo.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			calendarFrom.Visible=false;
			calendarTo.Visible=false;
			FillGrid();
		}

	}
}





















