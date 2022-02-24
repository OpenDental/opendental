using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using PdfSharp.Pdf;

namespace OpenDental {
	public partial class FormRpServiceDateView:FormODBase {
		#region Public Variables
		///<summary>This will be the PatNum or the Guarantor's PatNum.</summary>
		public readonly long PatNum;
		///<summary>Whether or not the window is displaying results for the entire family.</summary>
		public readonly bool IsFamily;
		#endregion
		#region Private Variables
		private bool _headingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;
		private Family _fam;
		#endregion
		
		public FormRpServiceDateView(long patNum,bool isFamily) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			PatNum=patNum;
			IsFamily=isFamily;
			_fam=Patients.GetFamily(patNum);
		}

		private void FormRpServiceDate_Load(object sender,EventArgs e) {
			FillGrid();
			Text=Lans.g(this,"Service Date View -")+" "+_fam.GetPatient(PatNum).GetNameFL()+(IsFamily ? " "+Lans.g(this,"(Family)") : "");
		}

		private void FillGrid() {
			DataTable table=RpServiceDateView.GetData(PatNum,IsFamily,checkDetailedView.Checked);
			gridMain.BeginUpdate();
			//Columns
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Service Date"),90));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Trans Date"),80));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),150));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Reference"),220));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Charge"),80,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Credit"),80,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Prov"),80));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"InsBal"),80,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"AcctBal"),80,HorizontalAlignment.Right));
			//Rows
			gridMain.ListGridRows.Clear();
			DataRow lastRow=table.Select().LastOrDefault();
			foreach(DataRow row in table.Rows) {
				GridRow newRow=new GridRow();
				DateTime serviceDate=PIn.Date(row["Date"].ToString());
				DateTime transDate=PIn.Date(row["Trans Date"].ToString());
				newRow.Cells.Add((serviceDate.Year<1880) ? "" : serviceDate.ToShortDateString());
				newRow.Cells.Add((transDate.Year<1880) ? "" : transDate.ToShortDateString());
				newRow.Cells.Add(row["Patient"].ToString());
				string strReference=row["Reference"].ToString();
				newRow.Cells.Add(strReference);
				bool isUnallocated=strReference.ToLower().Contains("unallocated");
				newRow.Cells.Add(isUnallocated ? "" : PIn.Decimal(row["Charge"].ToString()).ToString("f"));
				newRow.Cells.Add(isUnallocated ? "" : PIn.Decimal(row["Credit"].ToString()).ToString("f"));
				newRow.Cells.Add(row["Pvdr"].ToString());
				decimal insBal=PIn.Decimal(row["InsBal"].ToString());
				decimal acctBal=PIn.Decimal(row["AcctBal"].ToString());
				bool isTotalsRow=row==lastRow || strReference.ToLower().Contains("Total for Date".ToLower());
				bool isProc=row["Type"].ToString().ToLower()=="proc" && checkDetailedView.Checked;
				//Show insBal and acctBal when not on totals row and detailed is checked and either of the amounts are not zero.
				bool showDetailedRow=isTotalsRow || isProc
					|| (checkDetailedView.Checked && (CompareDecimal.IsGreaterThanZero(Math.Abs(insBal)) || CompareDecimal.IsGreaterThanZero(Math.Abs(acctBal))));
				newRow.Cells.Add(showDetailedRow ? insBal.ToString("f") : "");
				newRow.Cells.Add(showDetailedRow ? acctBal.ToString("f") : "");
				newRow.Tag=row;
				if(isTotalsRow) {
					newRow.Bold=true;
				}
				gridMain.ListGridRows.Add(newRow);
			}
			gridMain.EndUpdate();
		}


		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butSavePDFToImages_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"Grid is empty.");
				return;
			}
			//Get image category to save to. First image "Statement(S)" category.
			List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true).Where(x => x.ItemValue.Contains("S")).ToList();
			if(listImageCatDefs.IsNullOrEmpty()) {
				MsgBox.Show(this,"No image category set for Statements.");
				return;
			}
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			CreatePDF(tempFile);
			Patient patCur=_fam.GetPatient(PatNum);
			string rawBase64="";
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				rawBase64=Convert.ToBase64String(File.ReadAllBytes(tempFile));
			}
			Document docSave=new Document();
			docSave.DocNum=Documents.Insert(docSave);
			docSave.ImgType=ImageType.Document;
			docSave.DateCreated=DateTime.Now;
			docSave.PatNum=PatNum;
			docSave.DocCategory=listImageCatDefs.FirstOrDefault().DefNum;
			docSave.Description=$"ServiceDateView"+docSave.DocNum+$"{docSave.DateCreated.Year}_{docSave.DateCreated.Month}_{docSave.DateCreated.Day}";
			docSave.RawBase64=rawBase64;//blank if using AtoZfolder
			string fileName=ODFileUtils.CleanFileName(docSave.Description);
			string filePath=ImageStore.GetPatientFolder(patCur,ImageStore.GetPreferredAtoZpath());
			while(FileAtoZ.Exists(FileAtoZ.CombinePaths(filePath,fileName+".pdf"))) {
				fileName+="x";
			}
			FileAtoZ.Copy(tempFile,ODFileUtils.CombinePaths(filePath,fileName+".pdf"),FileAtoZSourceDestination.LocalToAtoZ);
			docSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
			Documents.Update(docSave);
			try {
				File.Delete(tempFile); //cleanup the temp file.
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			MsgBox.Show(this,"PDF saved successfully.");
		}

		private void CreatePDF(string tempFile) {
			MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(true,PdfFontEmbedding.Always);
			pdfRenderer.Document=CreateDocument();
			pdfRenderer.RenderDocument();
			pdfRenderer.PdfDocument.Save(tempFile);
		}

		private MigraDoc.DocumentObjectModel.Document CreateDocument() {
			MigraDoc.DocumentObjectModel.Document doc= new MigraDoc.DocumentObjectModel.Document();
			doc.DefaultPageSetup.PageWidth=MigraDoc.DocumentObjectModel.Unit.FromInch(8.5);
			doc.DefaultPageSetup.PageHeight=MigraDoc.DocumentObjectModel.Unit.FromInch(11);
			doc.DefaultPageSetup.TopMargin=MigraDoc.DocumentObjectModel.Unit.FromInch(.5);
			doc.DefaultPageSetup.LeftMargin=MigraDoc.DocumentObjectModel.Unit.FromInch(.5);
			doc.DefaultPageSetup.RightMargin=MigraDoc.DocumentObjectModel.Unit.FromInch(.5);
			MigraDoc.DocumentObjectModel.Section section=doc.AddSection();
			MigraDoc.DocumentObjectModel.Font headingFont=MigraDocHelper.CreateFont(13,true);
			MigraDoc.DocumentObjectModel.Font subHeadingFont=MigraDocHelper.CreateFont(10,true);
			#region printHeading
			//Heading---------------------------------------------------------------------------------------------------------------
			MigraDoc.DocumentObjectModel.Paragraph par=section.AddParagraph();
			MigraDoc.DocumentObjectModel.ParagraphFormat parformat=new MigraDoc.DocumentObjectModel.ParagraphFormat();
			parformat.Alignment=MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
			par.Format=parformat;
			string text=Lans.g(this,"Service Date View");
			par.AddFormattedText(text,headingFont);
			par.AddLineBreak();
			//SubHeading---------------------------------------------------------------------------------------------------------------
			text=(IsFamily ? Lans.g(this,"Entire Family:")+" " : "")+$"{_fam.GetNameInFamFL(PatNum)}";
			par.AddFormattedText(text,subHeadingFont);
			par.AddLineBreak();
			text=Lans.g(this,"Date")+" "+DateTime.Now.ToShortDateString();
			par.AddFormattedText(text,subHeadingFont);
			#endregion
			MigraDocHelper.InsertSpacer(section,10);
			section.PageSetup.Orientation=MigraDoc.DocumentObjectModel.Orientation.Landscape;
			MigraDocHelper.DrawGrid(section,gridMain);
			return doc;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			if(gridMain.ListGridRows.Count==0) {
				MsgBox.Show(this,"Grid is empty.");
				return;
			}
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Service date view printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			System.Drawing.Font headingFont=new System.Drawing.Font("Arial",13,FontStyle.Bold);
			System.Drawing.Font subHeadingFont=new System.Drawing.Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text=Lan.g(this,"Service Date View");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=(IsFamily ? Lans.g(this,"Entire Family:")+" " : "")+$"{_fam.GetNameInFamFL(PatNum)}";
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=DateTime.Now.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}