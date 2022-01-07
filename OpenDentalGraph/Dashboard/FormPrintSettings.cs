using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using OpenDentBusiness;
using System.Drawing.Printing;
using System.Drawing;
using System.Windows.Media.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDentalGraph {
	public partial class FormPrintSettings:Form {
		private Chart _chartCur;
		private Legend _legend;
		private bool _isLoading=true;
		private int _marginWidth;
		private int _marginHeight;
		private int _xPos;
		private int _yPos;
		private int _pageWidth;
		private int _pageHeight;
		private Bitmap _bmpSheet;
		private XGraphics _xg;
		private Graphics _g;

		public FormPrintSettings(Chart chart,Legend legend) {
			//serialize to string, then de-serialize and store in here. essentially clone the chart.
			_chartCur=new Chart();
			using(System.IO.MemoryStream ms=new System.IO.MemoryStream()) {
				chart.Serializer.Save(ms);
				_chartCur.Serializer.Load(ms); //now we have a copy of the chart that we can manipulate without affecting the parent chart.
			}
			_legend=legend.PrintCopy();
			InitializeComponent();
		}

		private void FormPrintSettings_Load(object sender,EventArgs e) {
			_chartCur.Printing.PrintDocument=new PrintDocument();
			_chartCur.Printing.PrintDocument.PrintPage+=new PrintPageEventHandler(ChartGenericFormat_PrintPage);
			printPreviewControl.Document=new PrintDocument();
			_chartCur.Printing.PrintDocument.OriginAtMargins=true;
			_chartCur.Dock=DockStyle.None;
			_legend.Height=30;//never changes;
			//Default to more or less full screen landscape.
			textWidth.Text="800";
			textHeight.Text="600";
			textMarginHeight.Text="0";
			textMarginWidth.Text="0";
			textXPos.Text="150";
			textYPos.Text="130";
			_isLoading=false;
			//useful for testing chart/legend dimensions
			//_chartCur.BackColor=System.Drawing.Color.SteelBlue;
			//_legend.BackColor=System.Drawing.Color.Yellow;
			MakePage();
		}

		///<summary>Draw to the chart's built-in PrintDocument, then set the PrintPreviewController's document to that PrintDocument. 
		///Besides keeping everything synchronized, this makes it easier to display the print preview and show the printer settings window 
		///when the user clicks Print.</summary>
		private void MakePage() {
			//update private variables
			FillDimensions();
			//check to see if landscape mode was toggled.
			_chartCur.Printing.PrintDocument.DefaultPageSettings.Landscape=checkLandscape.Checked;
			//reset the document margins
			_chartCur.Printing.PrintDocument.DefaultPageSettings.Margins=new System.Drawing.Printing.Margins(_marginWidth,_marginWidth,_marginHeight,_marginHeight);
			//Reset the graphics objects by disposing of them and reinitializing them.
			if(_bmpSheet!=null) {
				_bmpSheet.Dispose();
			}
			if(_xg!=null) {
				_xg.Dispose();
			}
			if(_g!=null) {
				_g.Dispose();
			}
			_bmpSheet=new Bitmap(_pageWidth,_pageHeight);
			_g=Graphics.FromImage(_bmpSheet);
			_xg=XGraphics.FromGraphics(_g,new XSize(_bmpSheet.Width,_bmpSheet.Height));
			//draw both the chart and legend to bitmaps
			using(Bitmap chartbmp = new Bitmap(_chartCur.Width,_chartCur.Height))
			using(Bitmap legendbmp = new Bitmap(_legend.Width,_legend.Height)) {
				_chartCur.DrawToBitmap(chartbmp,new Rectangle(0,0,_chartCur.Width,_chartCur.Height));
				_legend.DrawToBitmap(legendbmp,new Rectangle(0,0,_legend.Width,_legend.Height));
				//draw both bitmaps to another bitmap for the whole sheet.
				_xg.DrawImage(legendbmp,_xPos+_marginWidth,_yPos+_marginHeight+chartbmp.Height,_legend.Width,_legend.Height);
				_xg.DrawImage(chartbmp,_xPos+_marginWidth,_yPos+_marginHeight,_chartCur.Width,_chartCur.Height);
			}
			printPreviewControl.Document=_chartCur.Printing.PrintDocument;
		}

		///<summary>Called from MakePage to fill the private variables of this form when anything changes.</summary>
		private void FillDimensions() {
			//chart width, default to 1.
			try {
				_chartCur.Width=PIn.Int(textWidth.Text);
			}
			catch {
				_chartCur.Width=1;
			}
			if(_chartCur.Width==0) {
				_chartCur.Width=1;
			}
			if(_chartCur.Width>1200) {
				_chartCur.Width=1200;
			}
			_legend.Width=_chartCur.Width;
			//chart height, default to 1.
			try {
				_chartCur.Height=PIn.Int(textHeight.Text);
			}
			catch {
				_chartCur.Height=1;
			}
			if(_chartCur.Height==0) {
				_chartCur.Height=1;
			}
			if(_chartCur.Height>1200) {
				_chartCur.Height=1200;
			}
			//margin width, default to 0.
			try {
				_marginWidth=PIn.Int(textMarginWidth.Text);
			}
			catch {
				_marginWidth=0;
			}
			if(_marginWidth<0) {
				_marginWidth=0;
			}
			if(_marginWidth>1200) {
				_marginWidth=1200;
			}
			//margin height, default to 0.
			try {
				_marginHeight=PIn.Int(textMarginHeight.Text);
			}
			catch {
				_marginHeight=0;
			}
			if(_marginHeight<0) {
				_marginHeight=0;
			}
			if(_marginHeight>1200) {
				_marginHeight=1200;
			}
			//chart X position, default to 0
			try {
				_xPos=PIn.Int(textXPos.Text);
			}
			catch {
				_xPos=0;
			}
			if(_xPos<0) {
				_xPos=0;
			}
			if(_xPos>1200) {
				_xPos=1200;
			}
			//chart Y position, default to 0
			try {
				_yPos=PIn.Int(textYPos.Text);
			}
			catch {
				_yPos=0;
			}
			if(_yPos<0) {
				_yPos=0;
			}
			if(_yPos>1200) {
				_yPos=1200;
			}
			//printpreviewcontrol's document's width and height are always stored as if the page were in portrait mode.
			//this dynamically stores the width and height of the page, with width always being horizontal and height always being vertical.
			//makes the code cleaner as we don't have to put if-checks everywhere that we need the paper size.
			if(checkLandscape.Checked) {
				_pageWidth=printPreviewControl.Document.DefaultPageSettings.PaperSize.Height;
				_pageHeight=printPreviewControl.Document.DefaultPageSettings.PaperSize.Width;
			}
			else {
				_pageWidth=printPreviewControl.Document.DefaultPageSettings.PaperSize.Width;
				_pageHeight=printPreviewControl.Document.DefaultPageSettings.PaperSize.Height;
			}
		}

		private void ChartGenericFormat_PrintPage(object sender,PrintPageEventArgs ev) {
			//draw the sheet bitmap to the chart document
			ev.Graphics.DrawImage(_bmpSheet,0,0,_pageWidth,_pageHeight);
		}

		private void butExport_Click(object sender,EventArgs e) {
			SaveFileDialog sd=new SaveFileDialog() {
				Filter="PDF (*.pdf)|*.pdf",
				FilterIndex=1,
				RestoreDirectory=true,
			};
			if(ODBuild.IsWeb()) {
				sd.FileName=ODFileUtils.CombinePaths(Path.GetTempPath(),"chart_export.pdf");
			}
			else {
				if(sd.ShowDialog()!=DialogResult.OK) {
					return;
				}
			}
			using(PdfDocument pdfDoc=new PdfDocument()) {
				//save the chart into the memoryStream as a BitMap
				try {
					PdfPage page=new PdfPage();
					pdfDoc.Pages.Add(page);
					page.Orientation=checkLandscape.Checked ? PdfSharp.PageOrientation.Landscape : PdfSharp.PageOrientation.Portrait;
					using(XGraphics xGraphics = XGraphics.FromPdfPage(page)) {
						xGraphics.DrawImage(_bmpSheet,0,0,page.Width,page.Height);
					}
					pdfDoc.Save(sd.FileName);
					if(ODBuild.IsWeb()) {
						ThinfinityUtils.ExportForDownload(sd.FileName);
					}
					MessageBox.Show(Lans.g(this,"Chart saved."));
				}
				catch(Exception ex) {
					MessageBox.Show("Chart not saved."+"\r\n"+ex.Source+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				}
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_chartCur.Printing.Print(true);
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void refresh_Event(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			timer1.Stop();
			timer1.Start();
		}
		
		private void checkLandscape_CheckedChanged(object sender,EventArgs e) {
			MakePage();
		}

		private void timer1_Tick(object sender,EventArgs e) {
			timer1.Stop();
			MakePage();
		}
	}
}
