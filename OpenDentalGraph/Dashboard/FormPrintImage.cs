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
using System.Collections.Generic;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDentalGraph {
	public partial class FormPrintImage:Form {
		//the amount of space between each chart.
		private const int CHART_SPACING=30;
		private DashboardPanelCtrl _dashPanel;
		private Dictionary<Point,Chart> _dictCharts;
		private bool _isLoading=true;
		private int _marginWidth;
		private int _marginHeight;
		private int _imageWidth;
		private int _imageHeight;
		private int _xPos;
		private int _yPos;
		private Bitmap _bmpSheet;
		private Bitmap _bmpImage;
		private XGraphics _xg;
		private Graphics _g;
		
		private Rectangle PageDimensions {
			get {
				return new Rectangle(0,0,printDocument1.DefaultPageSettings.Bounds.Width,printDocument1.DefaultPageSettings.Bounds.Height);
			}
		}

		///<summary>Dictionary of charts to get converted to a bmp.
		///It would be great if this could be replaced with sheets sometime in the future.</summary>
		public FormPrintImage(DashboardPanelCtrl dashPanel) {
			InitializeComponent();
			_dashPanel=dashPanel;
		}

		private void FormPrintSettings_Load(object sender,EventArgs e) {
			printPreviewControl.Document=new PrintDocument();
			_dictCharts=_dashPanel.GetGraphsAsDictionary();
			//figure out whether the printed page should default to landscape or portait
			checkLandscape.Checked=(_dashPanel.Columns*8>_dashPanel.Rows*5);
			printDocument1.DefaultPageSettings.Landscape=checkLandscape.Checked;
			FillDimensions();
			_bmpImage=ConvertToBmp(true);
			//we want the chart to maintain its aspect ratio.
			textWidth.Text=_bmpImage.Width.ToString();
			textHeight.Text=_bmpImage.Height.ToString();
			MakePage(false);
			_isLoading=false;
		}
		
		#region Helper Methods
		///<summary>Converts a dictionary of charts with information about the number of rows and columns into one large bitmap.</summary>
		private Bitmap ConvertToBmp(bool useDefaultSize) {
			//the preliminary size of the entire image. This is the total space we have available to use.
			Rectangle imgBounds;
			int chartWidth;
			int chartHeight;
			//base the chart heights and width on the total space we have to fill the entire image. Charts often look stretched.
			//We want to default the charts to be wider than they are tall. To prevent stretched looking charts.
			//However, in order to render the charts at the highest quality regardless of what height the user inputs, we'll only enforce this while loading.
			if(useDefaultSize) {
				imgBounds = new Rectangle(0,0,PageDimensions.Width-(_marginWidth*2),PageDimensions.Height-(_marginHeight*2));
				chartWidth=((imgBounds.Width - (CHART_SPACING *(_dashPanel.Columns - 1)))/_dashPanel.Columns);
				chartHeight=((imgBounds.Height - (CHART_SPACING *(_dashPanel.Rows - 1)))/_dashPanel.Rows);
				if(chartHeight > (2*chartWidth/3)) {
					chartHeight=(2*chartWidth/3); //make the height shorter.
					imgBounds.Height=(chartHeight *_dashPanel.Rows) + (CHART_SPACING * (_dashPanel.Rows - 1));//now change the size of the total space to match.
				}
			}
			else {
				imgBounds = new Rectangle(0,0,_imageWidth,_imageHeight);
				chartWidth=((imgBounds.Width - (CHART_SPACING *(_dashPanel.Columns- 1)))/_dashPanel.Columns);
				chartHeight=((imgBounds.Height - (CHART_SPACING *(_dashPanel.Rows - 1)))/_dashPanel.Rows);
			}
			chartWidth=Math.Max(chartWidth,1);
			chartHeight=Math.Max(chartHeight,1);
			//below we actually generate the bitmap and draw each chart to it's corresponding location.
			//If they have a chart area without a chart, it won't draw anything.
			Bitmap bmp = new Bitmap(imgBounds.Width,imgBounds.Height);
			for(int row = 0;row <_dashPanel.Rows;row++) {
				for(int col = 0;col <_dashPanel.Columns;col++) {
					Chart chartCur;
					if(!_dictCharts.TryGetValue(new Point(row,col),out chartCur)) {
						continue;
					}
					Chart chartResult = new Chart();
					using(System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
						chartCur.Serializer.Save(ms);
						chartResult.Serializer.Load(ms); //now we have a copy of the chart that we can manipulate without affecting the parent chart.
					}
					chartResult.Width=chartWidth;
					chartResult.Height=chartHeight;
					chartResult.DrawToBitmap(bmp,new Rectangle(((col*chartWidth)+((col)*CHART_SPACING)),((row*chartHeight)+((row)*CHART_SPACING)),chartWidth,chartHeight));
				}
			}
			return bmp;
		}

		///<summary>Draw to the chart's built-in PrintDocument, then set the PrintPreviewController's document to that PrintDocument. 
		///Besides keeping everything synchronized, this makes it easier to display the print preview and show the printer settings window 
		///when the user clicks Print.</summary>
		private void MakePage(bool createBitmap) {
			//check to see if landscape mode was toggled.
			printDocument1.DefaultPageSettings.Landscape=checkLandscape.Checked;
			//update private variables
			FillDimensions();
			if(createBitmap) {
				if(_bmpImage!=null) {
					_bmpImage.Dispose();
				}
				_bmpImage=ConvertToBmp(false);
			}
			Graphics g=Graphics.FromImage(_bmpImage);
			//reset the document margins
			printDocument1.DefaultPageSettings.Margins=new System.Drawing.Printing.Margins(_marginWidth,_marginWidth,_marginHeight,_marginHeight);
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
			_bmpSheet=new Bitmap(PageDimensions.Width,PageDimensions.Height);
			_g=Graphics.FromImage(_bmpSheet);
			_xg=XGraphics.FromGraphics(_g,new XSize(_bmpSheet.Width,_bmpSheet.Height));
			//margins are taken care of because we set them up in DefaultPageSettings.
			_xg.DrawImage(_bmpImage,_xPos,_yPos,_imageWidth,_imageHeight);
			printPreviewControl.Document=printDocument1;
		}

		///<summary>Called from MakePage to fill the private variables of this form when anything changes.</summary>
		private void FillDimensions() {
			Func<string,int,int,int> funcGetIntFromText=new Func<string, int, int, int>((string text,int minVal,int maxVal) => {
				int ret=Math.Max(minVal,PIn.Int(text,false));
				return Math.Min(ret,maxVal);
			});
			_imageWidth=funcGetIntFromText(textWidth.Text,1,10000);
			_imageHeight=funcGetIntFromText(textHeight.Text,1,10000);
			_marginWidth=funcGetIntFromText(textMarginWidth.Text,0,PageDimensions.Width);
			_marginHeight=funcGetIntFromText(textMarginHeight.Text,0,PageDimensions.Height);
			_xPos=PIn.Int(textXPos.Text,false);
			_yPos=PIn.Int(textYPos.Text,false);			
		}
		#endregion

		#region Control Events
		private void ImageGenericFormat_PrintPage(object sender,PrintPageEventArgs ev) {
			ev.Graphics.DrawImage(_bmpSheet,0,0,PageDimensions.Width,PageDimensions.Height);
		}

		private void butExport_Click(object sender,EventArgs e) {
			SaveFileDialog sd=new SaveFileDialog() {
				Filter="PDF (*.pdf)|*.pdf",
				FilterIndex=1,
				RestoreDirectory=true,
			};
			if(ODBuild.IsWeb()) {
				sd.FileName=ODFileUtils.CombinePaths(Path.GetTempPath(),"image_export.pdf");
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
						xGraphics.DrawImage(_bmpSheet,_xPos+_marginWidth,_yPos+_marginHeight,page.Width,page.Height);
					}
					pdfDoc.Save(sd.FileName);
					if(ODBuild.IsWeb()) {
						ThinfinityUtils.ExportForDownload(sd.FileName);
					}
					MessageBox.Show(Lans.g(this,"File saved."));
				}
				catch(Exception ex) {
					MessageBox.Show("File not saved."+"\r\n"+ex.Source+"\r\n"+ex.Message+"\r\n"+ex.StackTrace);
				}
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			printPreviewControl.Document.Print();
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
			if(_isLoading) {
				return;
			}
			MakePage(true);
		}

		private void timer1_Tick(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			timer1.Stop();
			MakePage(true);
		}
		#endregion
	}
}
