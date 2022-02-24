using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAccounting:FormODBase {
		//private Account[] AccountList;
		private DataTable table;

		///<summary></summary>
		public FormAccounting()
		{
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccounting_Load(object sender,EventArgs e) {
			LayoutMenu();
			LayoutToolBar();
			textDate.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			//Setup-----------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemSetup=new MenuItemOD("Setup");
			menuMain.Add(menuItemSetup);
			LayoutMenuSetup(menuItemSetup);
				//Lock-----------------------------------------------------------------------------------------------------------
				menuMain.Add(new MenuItemOD("Lock",menuItemLock_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemReports=new MenuItemOD("Reports");
			menuMain.Add(menuItemReports);
			menuItemReports.Add("General Ledger Detail",menuItemGL_Click);
			menuItemReports.Add("Balance Sheet",menuItemBalSheet_Click);
			menuItemReports.Add("Profit and Loss",menuItemProfitLoss_Click);
			menuMain.EndUpdate();
		}

		private void LayoutMenuSetup(MenuItemOD menuItemSetup) {
			foreach(AccountingSoftware software in Enum.GetValues(typeof(AccountingSoftware))) {
				menuItemSetup.Add(ODPrimitiveExtensions.GetDescription(software),(o,e) => {
					switch(software) {
						case AccountingSoftware.OpenDental:
							using(FormAccountingSetup FormA=new FormAccountingSetup()){
								FormA.ShowDialog();
							}
							break;
						case AccountingSoftware.QuickBooks:
							if(ODBuild.IsWeb()) {
								MsgBox.Show(this,"QuickBooks is not available while viewing through the web.");
								return;
							}
							using(FormQuickBooksSetup FormQB=new FormQuickBooksSetup()) {
								FormQB.ShowDialog();
							}
							break;
						case AccountingSoftware.QuickBooksOnline:
							if(!Programs.IsEnabled(ProgramName.QuickBooksOnline)) {
								MsgBox.Show(this,"QuickBooks Online must be enabled in Program Links first.");
								return;
							}
							using(FormQuickBooksOnlineSetup FormQBO=new FormQuickBooksOnlineSetup()) {
								FormQBO.ShowDialog();
							}
							break;
					}
				});
			}
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),0,"","Add"));
			//ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Edit"),1,Lan.g(this,"Edit Selected Account"),"Edit"));
			/*ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			button.Style=ODToolBarButtonStyle.Label;
			ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));*/
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Export"),2,Lan.g(this,"Export the Chart of Accounts"),"Export"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
		}

		private void menuItemLock_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
				return;
			}
			using FormAccountingLock FormA=new FormAccountingLock();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK){
				SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,"Accounting Lock Changed");
			}
		}

		private void menuItemGL_Click(object sender,EventArgs e) {
			using FormRpAccountingGenLedg FormA=new FormRpAccountingGenLedg();
			FormA.ShowDialog();
		}

		private void menuItemBalSheet_Click(object sender,EventArgs e) {
			using FormRpAccountingBalanceSheet FormA=new FormRpAccountingBalanceSheet();
			FormA.ShowDialog();
		}

		private void menuItemProfitLoss_Click(object sender, EventArgs e){
			using FormRpAccountingProfitLoss formRpAccountingProfitLoss=new FormRpAccountingProfitLoss();
			formRpAccountingProfitLoss.ShowDialog();
		}
		
		private void ToolBarMain_ButtonClick(object sender,OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()) {
				case "Add":
					Add_Click();
					break;
				case "Edit":
					Edit_Click();
					break;
				case "Close":
					Close();
					break;
				case "Export":
					Export_Click();
					break;
			}
			/*	case "Fwd":
					OnFwd_Click();
					break;
				
			}*/
		}

		private void FillGrid(){
			Accounts.RefreshCache();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableChartOfAccounts","Type"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Description"),170);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Balance"),80,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Bank Number"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableChartOfAccounts","Inactive"),70,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			if(!textDate.IsValid()){//error
				table=Accounts.GetFullList(DateTime.Today,checkInactive.Checked);
			}
			else{
				table=Accounts.GetFullList(PIn.Date(textDate.Text),checkInactive.Checked);
			}
			for(int i=0;i<table.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(table.Rows[i]["type"].ToString());
				row.Cells.Add(table.Rows[i]["Description"].ToString());
				row.Cells.Add(table.Rows[i]["balance"].ToString());
				row.Cells.Add(table.Rows[i]["BankNumber"].ToString());
				row.Cells.Add(table.Rows[i]["inactive"].ToString());				
				if(i<table.Rows.Count-1//if not the last row
					&& table.Rows[i]["type"].ToString() != table.Rows[i+1]["type"].ToString())
				{
					row.ColorLborder=Color.Black;
				}
				row.ColorBackG=Color.FromArgb(PIn.Int(table.Rows[i]["color"].ToString()));
				gridMain.ListGridRows.Add(row);
			}
			/*for(int i=0;i<Accounts.ListLong.Length;i++){
				if(!checkInactive.Checked && Accounts.ListLong[i].Inactive){
					continue;
				}
				row=new ODGridRow();
				row.Cells.Add(Lan.g("enumAccountType",Accounts.ListLong[i].AcctType.ToString()));
				row.Cells.Add(Accounts.ListLong[i].Description);
				if(Accounts.ListLong[i].AcctType==AccountType.Asset){
					row.Cells.Add(Accounts.GetBalance(Accounts.ListLong[i].AccountNum,Accounts.ListLong[i].AcctType).ToString("n"));
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(Accounts.ListLong[i].BankNumber);
				if(Accounts.ListLong[i].Inactive){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(i<Accounts.ListLong.Length-1//if not the last row
					&& Accounts.ListLong[i].AcctType != Accounts.ListLong[i+1].AcctType){
					row.ColorLborder=Color.Black;
				}
				row.Tag=Accounts.ListLong[i].Clone();
				row.ColorBackG=Accounts.ListLong[i].AccountColor;
				gridMain.Rows.Add(row);
			}*/
			gridMain.EndUpdate();
		}

		private void Add_Click() {
			Account acct=new Account();
			acct.AcctType=AccountType.Asset;
			acct.AccountColor=Color.White;
			using FormAccountEdit FormA=new FormAccountEdit(acct);
			FormA.IsNew=true;
			FormA.ShowDialog();
			FillGrid();
		}

		private void Edit_Click() {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please pick an account first.");
				return;
			}
			long acctNum=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["AccountNum"].ToString());
			if(acctNum==0) {
				MsgBox.Show(this,"This account is generated automatically, and cannot be edited.");
				return;
			}
			Account acct=Accounts.GetAccount(acctNum);
			using FormAccountEdit FormA=new FormAccountEdit(acct);
			FormA.ShowDialog();
			FillGrid();
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i]["AccountNum"].ToString()==acctNum.ToString()){
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void Export_Click() {
			gridMain.Export(gridMain.Title);
			//string msg=gridMain.Export(null,new List<Tuple<string,string>>() { Tuple.Create(labelDate.Text,PIn.Date(textDate.Text).ToShortDateString()) });
			//if(!string.IsNullOrEmpty(msg)) {
			//	MsgBox.Show(this,msg);
			//}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long acctNum=PIn.Long(table.Rows[gridMain.GetSelectedIndex()]["AccountNum"].ToString());
			if(acctNum==0) {
				MsgBox.Show(this,"This account is generated automatically, and there is currently no way to view the detail.  It is the sum of all income minus all expenses for all previous years.");
				return;
			}
			DateTime asofDate;
			if(!textDate.IsValid()) {//error
				asofDate=DateTime.Today;
			}
			else{
				asofDate=PIn.Date(textDate.Text);
			}
			Account acct=Accounts.GetAccount(acctNum);
			using FormJournal FormJ=new FormJournal(acct);
			FormJ.InitialAsOfDate=asofDate;
			FormJ.ShowDialog();
			FillGrid();
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["AccountNum"].ToString()==acctNum.ToString()) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void checkInactive_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butToday_Click(object sender,EventArgs e) {
			textDate.Text=DateTime.Today.ToShortDateString();
			FillGrid();
		}

	
	}
}





















