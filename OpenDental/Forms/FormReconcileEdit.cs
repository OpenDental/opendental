using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Text.RegularExpressions;
using MigraDoc.DocumentObjectModel;
using System.Drawing.Printing;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormReconcileEdit : FormODBase {
		private Reconcile ReconcileCur;
		private List <JournalEntry> JournalList;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormReconcileEdit(Reconcile reconcileCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ReconcileCur=reconcileCur;
		}

		private void FormReconcileEdit_Load(object sender,EventArgs e) {
			textDate.Text=ReconcileCur.DateReconcile.ToShortDateString();
			checkLocked.Checked=ReconcileCur.IsLocked;
			textStart.Text=ReconcileCur.StartingBal.ToString("n");
			textEnd.Text=ReconcileCur.EndingBal.ToString("n");
			textTarget.Text=(ReconcileCur.EndingBal-ReconcileCur.StartingBal).ToString("n");
			bool includeUncleared=!ReconcileCur.IsLocked;
			JournalList=JournalEntries.GetForReconcile(ReconcileCur.AccountNum,includeUncleared,ReconcileCur.ReconcileNum);
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableJournal","Chk #"),60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Date"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Deposits"),70,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableJournal","Withdrawals"),75,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("X",30,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			decimal sum=0;//to avoid cumulative errors.
			for(int i=0;i<JournalList.Count;i++) {
				row=new GridRow();
				row.Cells.Add(JournalList[i].CheckNumber);
				row.Cells.Add(JournalList[i].DateDisplayed.ToShortDateString());
				//row.Cells.Add(JournalList[i].Memo);
				//row.Cells.Add(JournalList[i].Splits);
				if(JournalList[i].DebitAmt==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(JournalList[i].DebitAmt.ToString("n"));
					if(JournalList[i].ReconcileNum!=0){
						sum+=(decimal)JournalList[i].DebitAmt;
					}
				}
				if(JournalList[i].CreditAmt==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(JournalList[i].CreditAmt.ToString("n"));
					if(JournalList[i].ReconcileNum!=0){
						sum-=(decimal)JournalList[i].CreditAmt;
					}
				}
				if(JournalList[i].ReconcileNum==0){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add("X");
				}
				if(this.textFindAmount.Text.Length>0){
					try {
						double famt=Convert.ToDouble(textFindAmount.Text);
						if((famt==0 && famt==JournalList[i].CreditAmt && famt==JournalList[i].DebitAmt)
							|| (famt!=0 && (famt==JournalList[i].CreditAmt || famt==JournalList[i].DebitAmt))) 
						{
							row.ColorBackG=System.Drawing.Color.Yellow;
						}
					}
					catch {
						//Just a format error in the amount probably, who cares.
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
			if(JournalList[e.Row].ReconcileNum==0){
				JournalList[e.Row].ReconcileNum=ReconcileCur.ReconcileNum;
			}
			else{
				JournalList[e.Row].ReconcileNum=0;
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
			else{//unchecking
				//need to check permissions here.
			}
			SaveList();
			bool includeUncleared=!checkLocked.Checked;
			JournalList=JournalEntries.GetForReconcile(ReconcileCur.AccountNum,includeUncleared,ReconcileCur.ReconcileNum);
			FillGrid();
		}

		///<summary>Saves all changes to JournalList to database.  Can only be called once when closing form.</summary>
		private void SaveList(){
			JournalEntries.SaveList(JournalList,ReconcileCur.ReconcileNum);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			try{
				Reconciles.Delete(ReconcileCur);
				DialogResult=DialogResult.OK;
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
			}
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
			if(IsNew){
				for(int i=0;i<JournalList.Count;i++){
					JournalList[i].ReconcileNum=0;
				}
				SaveList();//detaches all journal entries.
				Reconciles.Delete(ReconcileCur);
			}
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
			PrintDocument pd=new PrintDocument();//TODO: Implement ODprintout pattern - MigraDoc
			if(!PrinterL.SetPrinter(pd,PrintSituation.Default,0,"Reconcile list printed")) {
				return;//User cancelled.
			}
			pd.DefaultPageSettings.Margins=new Margins(25,25,40,40);
			if(pd.DefaultPageSettings.PrintableArea.Height==0) {
				pd.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			MigraDoc.DocumentObjectModel.Document doc=CreatePrintDocument(pd);
			MigraDoc.Rendering.Printing.MigraDocPrintDocument printdoc=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
			MigraDoc.Rendering.DocumentRenderer renderer=new MigraDoc.Rendering.DocumentRenderer(doc);
			renderer.PrepareDocument();
			printdoc.PrinterSettings=pd.PrinterSettings;
			printdoc.Renderer=renderer;
			if(ODBuild.IsDebug()) {
				using FormRpPrintPreview pView=new FormRpPrintPreview(printdoc);
				pView.ShowDialog();
			}
			else {
				printdoc.Print();
			}
		}

		private MigraDoc.DocumentObjectModel.Document CreatePrintDocument(PrintDocument pd) {
			string text;
			MigraDoc.DocumentObjectModel.Document doc=new MigraDoc.DocumentObjectModel.Document();
			doc.DefaultPageSetup.PageWidth=Unit.FromInch((double)pd.DefaultPageSettings.PaperSize.Width/100);
			doc.DefaultPageSetup.PageHeight=Unit.FromInch((double)pd.DefaultPageSettings.PaperSize.Height/100);
			doc.DefaultPageSetup.TopMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Top/100);
			doc.DefaultPageSetup.LeftMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Left/100);
			doc.DefaultPageSetup.RightMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Right/100);
			doc.DefaultPageSetup.BottomMargin=Unit.FromInch((double)pd.DefaultPageSettings.Margins.Bottom/100);
			MigraDoc.DocumentObjectModel.Section section=doc.AddSection();
			section.PageSetup.StartingNumber=1;
			MigraDoc.DocumentObjectModel.Font headingFont=MigraDocHelper.CreateFont(13,true);
			MigraDoc.DocumentObjectModel.Font bodyFontx=MigraDocHelper.CreateFont(9,false);
			MigraDoc.DocumentObjectModel.Font nameFontx=MigraDocHelper.CreateFont(9,true);
			MigraDoc.DocumentObjectModel.Font totalFontx=MigraDocHelper.CreateFont(9,true);
			Paragraph pageNumParag=new Paragraph();
			pageNumParag.AddText(Lan.g(this,"Page")+" ");
			pageNumParag.AddPageField();
			pageNumParag.AddText(" "+Lan.g(this,"of")+" ");
			pageNumParag.AddNumPagesField();
			section.Footers.Primary.Add(pageNumParag);
			Paragraph par=section.AddParagraph();
			ParagraphFormat parformat=new ParagraphFormat();
			parformat.Alignment=ParagraphAlignment.Center;
			parformat.Font=MigraDocHelper.CreateFont(14,true);
			par.Format=parformat;
			//Render the reconcile grid.
			par=section.AddParagraph();
			par.Format.Alignment=ParagraphAlignment.Center;
			par.AddFormattedText(Lan.g(this,"RECONCILE"),totalFontx);
			par.AddLineBreak();
			text=Accounts.GetAccount(ReconcileCur.AccountNum).Description.ToUpper();
			par.AddFormattedText(text,totalFontx);
			par.AddLineBreak();
			text=PrefC.GetString(PrefName.PracticeTitle);
			par.AddText(text);
			par.AddLineBreak();
			text=PrefC.GetString(PrefName.PracticePhone);
			if(text.Length==10&&Application.CurrentCulture.Name=="en-US") {
				text="("+text.Substring(0,3)+")"+text.Substring(3,3)+"-"+text.Substring(6);
			}
			par.AddText(text);
			par.AddLineBreak();
			par.AddText(MiscData.GetNowDateTime().ToShortDateString());
			par.AddLineBreak();
			par.AddText(Lan.g(this,"Reconcile Date")+": "+PIn.Date(textDate.Text).ToShortDateString());
			par.AddLineBreak();
			par.AddText(labelStart.Text+": "+PIn.Double(textStart.Text).ToString("n"));
			par.AddLineBreak();
			par.AddText(labelEnd.Text+": "+PIn.Double(textEnd.Text).ToString("n"));
			MigraDocHelper.InsertSpacer(section,10);
			MigraDocHelper.DrawGrid(section,gridMain);
			return doc;
		}
	}
}