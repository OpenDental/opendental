using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using MigraDoc.DocumentObjectModel;
using CodeBase;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormReconcileEdit : FormODBase {
		private Reconcile ReconcileCur;
		private List <JournalEntry> _listJournalEntries;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormReconcileEdit(Reconcile reconcile)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ReconcileCur=reconcile;
		}

		private void FormReconcileEdit_Load(object sender,EventArgs e) {
			textDate.Text=ReconcileCur.DateReconcile.ToShortDateString();
			checkLocked.Checked=ReconcileCur.IsLocked;
			textStart.Text=ReconcileCur.StartingBal.ToString("n");
			textEnd.Text=ReconcileCur.EndingBal.ToString("n");
			textTarget.Text=(ReconcileCur.EndingBal-ReconcileCur.StartingBal).ToString("n");
			bool doIncludeUncleared=!ReconcileCur.IsLocked;
			_listJournalEntries=JournalEntries.GetForReconcile(ReconcileCur.AccountNum,doIncludeUncleared,ReconcileCur.ReconcileNum);
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableJournal","Chk #"),60,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Date"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Deposits"),70,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Withdrawals"),75,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn("X",30,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			decimal sum=0;//to avoid cumulative errors.
			for(int i=0;i<_listJournalEntries.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listJournalEntries[i].CheckNumber);
				row.Cells.Add(_listJournalEntries[i].DateDisplayed.ToShortDateString());
				//row.Cells.Add(JournalList[i].Memo);
				//row.Cells.Add(JournalList[i].Splits);
				if(_listJournalEntries[i].DebitAmt==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listJournalEntries[i].DebitAmt.ToString("n"));
					if(_listJournalEntries[i].ReconcileNum!=0){
						sum+=(decimal)_listJournalEntries[i].DebitAmt;
					}
				}
				if(_listJournalEntries[i].CreditAmt==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listJournalEntries[i].CreditAmt.ToString("n"));
					if(_listJournalEntries[i].ReconcileNum!=0){
						sum-=(decimal)_listJournalEntries[i].CreditAmt;
					}
				}
				if(_listJournalEntries[i].ReconcileNum==0){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add("X");
				}
				if(this.textFindAmount.Text.Length>0){
					double amtFound=0.00;
					try {
						amtFound=Convert.ToDouble(textFindAmount.Text);
					} 
					catch {
						//Just a format error in the amount probably, who cares.
						gridMain.ListGridRows.Add(row);
						return;
					}
					if((amtFound==0 && amtFound==_listJournalEntries[i].CreditAmt && amtFound==_listJournalEntries[i].DebitAmt)
						|| (amtFound!=0 && (amtFound==_listJournalEntries[i].CreditAmt || amtFound==_listJournalEntries[i].DebitAmt))) 
					{
						row.ColorBackG=System.Drawing.Color.Yellow;
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			textSum.Text=sum.ToString("n");
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col != 4){
				return;
			}
			if(checkLocked.Checked){
				return;
			}
			if(_listJournalEntries[e.Row].ReconcileNum==0){
				_listJournalEntries[e.Row].ReconcileNum=ReconcileCur.ReconcileNum;
			}
			else{
				_listJournalEntries[e.Row].ReconcileNum=0;
			}
			int rowClicked=e.Row;
			FillGrid();
			gridMain.SetSelected(rowClicked,true);
		}

		private void textStart_TextChanged(object sender,EventArgs e) {
			if(!textStart.IsValid() || !textEnd.IsValid()) {
				return;
			}
			textTarget.Text=(PIn.Double(textEnd.Text)-PIn.Double(textStart.Text)).ToString("n");
		}

		private void textEnd_TextChanged(object sender,EventArgs e) {
			if(!textStart.IsValid() || !textEnd.IsValid()) {
				return;
			}
			textTarget.Text=(PIn.Double(textEnd.Text)-PIn.Double(textStart.Text)).ToString("n");
		}

		private void checkLocked_Click(object sender,EventArgs e) {
			if(checkLocked.Checked){
				if(textTarget.Text != textSum.Text){
					MsgBox.Show(this,"Target change must match sum of transactions.");
					checkLocked.Checked=false;
					return;
				}
			}
			//else{//unchecking
				//need to check permissions here.
			//}
			SaveList();
			bool doIncludeUncleared=!checkLocked.Checked;
			_listJournalEntries=JournalEntries.GetForReconcile(ReconcileCur.AccountNum,doIncludeUncleared,ReconcileCur.ReconcileNum);
			FillGrid();
		}

		///<summary>Saves all changes to JournalList to database.  Can only be called once when closing form.</summary>
		private void SaveList(){
			JournalEntries.SaveList(_listJournalEntries,ReconcileCur.ReconcileNum);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			try{
				Reconciles.Delete(ReconcileCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()
				|| !textStart.IsValid()
				|| !textEnd.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			ReconcileCur.DateReconcile=PIn.Date(textDate.Text);
			ReconcileCur.StartingBal=PIn.Double(textStart.Text);
			ReconcileCur.EndingBal=PIn.Double(textEnd.Text);
			ReconcileCur.IsLocked=checkLocked.Checked;
			Reconciles.Update(ReconcileCur);
			SaveList();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormReconcileEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK){
				return;
			}
			if(!IsNew){
				return;
			}
			for(int i=0;i<_listJournalEntries.Count;i++){
				_listJournalEntries[i].ReconcileNum=0;
			}
			SaveList();//detaches all journal entries.
			Reconciles.Delete(ReconcileCur);
		}

		private void textFindAmount_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void textFindAmount_Leave(object sender,EventArgs e) {
			if(this.textFindAmount.Text!="" && 
				!Regex.IsMatch(this.textFindAmount.Text,"[0-9]+(\\.[0-9]+)?")){
				MessageBox.Show("Invalid amount format in text search amount field.");
			}
		}

		private void butExport_Click(object sender,EventArgs e) {
			//List<Tuple<string,string>> listOtherDetails=new List<Tuple<string,string>>() {
			//	Tuple.Create(labelDate.Text,PIn.Date(textDate.Text).ToShortDateString()),
			//	Tuple.Create(labelStart.Text,PIn.Double(textStart.Text).ToString("n")),
			//	Tuple.Create(labelEnd.Text,PIn.Double(textEnd.Text).ToString("n")),
			//	Tuple.Create(labelTarget.Text,PIn.Double(textTarget.Text).ToString("n")),
			//	Tuple.Create(labelSum.Text,PIn.Double(textSum.Text).ToString("n"))
			//};
			//string msg=
			gridMain.Export(gridMain.Title);//listOtherDetails:listOtherDetails);
			//if(!string.IsNullOrEmpty(msg)) {
			//	MsgBox.Show(this,msg);
			//}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrintDocument printDocument=new PrintDocument();//TODO: Implement ODprintout pattern - MigraDoc
			if(!PrinterL.SetPrinter(printDocument,PrintSituation.Default,0,"Reconcile list printed")) {
				return;//User cancelled.
			}
			printDocument.DefaultPageSettings.Margins=new Margins(25,25,40,40);
			if(printDocument.DefaultPageSettings.PrintableArea.Height==0) {
				printDocument.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			MigraDoc.DocumentObjectModel.Document document=CreatePrintDocument(printDocument);
			MigraDoc.Rendering.Printing.MigraDocPrintDocument migraDocPrintDocument=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
			MigraDoc.Rendering.DocumentRenderer documentRenderer=new MigraDoc.Rendering.DocumentRenderer(document);
			documentRenderer.PrepareDocument();
			migraDocPrintDocument.PrinterSettings=printDocument.PrinterSettings;
			migraDocPrintDocument.Renderer=documentRenderer;
			if(ODBuild.IsDebug()) {
				using FormRpPrintPreview formRpPrintPreview=new FormRpPrintPreview(migraDocPrintDocument);
				formRpPrintPreview.ShowDialog();
				return;
			}
			migraDocPrintDocument.Print();
		}

		private MigraDoc.DocumentObjectModel.Document CreatePrintDocument(PrintDocument pd) {
			string text;
			MigraDoc.DocumentObjectModel.Document document=new MigraDoc.DocumentObjectModel.Document();
			document.DefaultPageSetup.PageWidth=Unit.FromInch((double)pd.DefaultPageSettings.PaperSize.Width/100);
			document.DefaultPageSetup.PageHeight=Unit.FromInch((double)pd.DefaultPageSettings.PaperSize.Height/100);
			document.DefaultPageSetup.TopMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Top/100);
			document.DefaultPageSetup.LeftMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Left/100);
			document.DefaultPageSetup.RightMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Right/100);
			document.DefaultPageSetup.BottomMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Bottom/100);
			MigraDoc.DocumentObjectModel.Section section=document.AddSection();
			section.PageSetup.StartingNumber=1;
			MigraDoc.DocumentObjectModel.Font fontHeading=MigraDocHelper.CreateFont(13,true);
			MigraDoc.DocumentObjectModel.Font fontBodyx=MigraDocHelper.CreateFont(9,false);
			MigraDoc.DocumentObjectModel.Font fontNamex=MigraDocHelper.CreateFont(9,true);
			MigraDoc.DocumentObjectModel.Font fontTotalx=MigraDocHelper.CreateFont(9,true);
			Paragraph paragraphPageNum=new Paragraph();
			paragraphPageNum.AddText(Lan.g(this,"Page")+" ");
			paragraphPageNum.AddPageField();
			paragraphPageNum.AddText(" "+Lan.g(this,"of")+" ");
			paragraphPageNum.AddNumPagesField();
			section.Footers.Primary.Add(paragraphPageNum);
			Paragraph paragraph=section.AddParagraph();
			ParagraphFormat paragraphFormat=new ParagraphFormat();
			paragraphFormat.Alignment=ParagraphAlignment.Center;
			paragraphFormat.Font=MigraDocHelper.CreateFont(14,true);
			paragraph.Format=paragraphFormat;
			//Render the reconcile grid.
			paragraph=section.AddParagraph();
			paragraph.Format.Alignment=ParagraphAlignment.Center;
			paragraph.AddFormattedText(Lan.g(this,"RECONCILE"),fontTotalx);
			paragraph.AddLineBreak();
			text=Accounts.GetAccount(ReconcileCur.AccountNum).Description.ToUpper();
			paragraph.AddFormattedText(text,fontTotalx);
			paragraph.AddLineBreak();
			text=PrefC.GetString(PrefName.PracticeTitle);
			paragraph.AddText(text);
			paragraph.AddLineBreak();
			text=PrefC.GetString(PrefName.PracticePhone);
			if(text.Length==10) {
				text=TelephoneNumbers.ReFormat(text);
			}
			paragraph.AddText(text);
			paragraph.AddLineBreak();
			paragraph.AddText(MiscData.GetNowDateTime().ToShortDateString());
			paragraph.AddLineBreak();
			paragraph.AddText(Lan.g(this,"Reconcile Date")+": "+PIn.Date(textDate.Text).ToShortDateString());
			paragraph.AddLineBreak();
			paragraph.AddText(labelStart.Text+": "+PIn.Double(textStart.Text).ToString("n"));
			paragraph.AddLineBreak();
			paragraph.AddText(labelEnd.Text+": "+PIn.Double(textEnd.Text).ToString("n"));
			MigraDocHelper.InsertSpacer(section,10);
			MigraDocHelper.DrawGrid(section,gridMain);
			return document;
		}
	}
}