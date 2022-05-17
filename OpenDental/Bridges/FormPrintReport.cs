using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CodeBase;

namespace OpenDental {
	///<summary>This class is pretty much only coded for 8.5 inch X 11 inch sheets of paper, but with a little work could be made for more general sizes of paper, perhaps sometime in the future.</summary>
	public partial class FormPrintReport:FormODBase {

		///<summary>If pageNumberFont is set, then the page number is displayed using the page number information.</summary>
		private Font pageNumberFont=null;
		private int totalPages=0;
		///Is set to a non-null value only during printing to a physical printer.
		private Graphics printerGraph=null;
		private Rectangle printerMargins;
		private int pageHeight=900;
		private int curPrintPage=0;
		public delegate void PrintCallback(FormPrintReport fpr);
		public PrintCallback printGenerator=null;
		private bool landscape=false;
		private int numTimesPrinted=0;
		private int minimumTimesToPrint=0;

		public FormPrintReport() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPrintReport_Load(object sender,EventArgs e) {
			PrintCustom();
			CheckWindowSize();
		}

		public void UsePageNumbers(Font font){
			pageNumberFont=font;
		}

		private void PrintCustom(){
			if(printGenerator==null){
				return;
			}
			printPanel.Clear();
			Invoke(printGenerator,new object[] { this });//Call the custom printing code.
			//Print page numbers.
			if(pageNumberFont!=null){
				for(int i=0;i<totalPages;i++){
					string text="Page "+(i+1);
					SizeF size=Graph.MeasureString(text,pageNumberFont);
					Graph.DrawString(text,pageNumberFont,Brushes.Black,
						new PointF(GraphWidth-size.Width,i*(pageHeight+MarginBottom)));
				}
			}
		}

		private int CurPage(){
			return (vScroll.Value-vScroll.Minimum+1)/pageHeight;
		}

		private void CalculatePageOffset(){
			printPanel.Origin=new Point(0,-vScroll.Value);
			labPageNum.Text="Page: "+(CurPage()+1)+"\\"+totalPages;
		}

		private void CalculateVScrollMax(){
			if(totalPages>1){
				vScroll.Maximum=pageHeight*(totalPages-1)-1+vScroll.Minimum;
			}else{
				vScroll.Maximum=vScroll.Minimum;
			}
		}

		private void MoveScrollBar(int amount){
			int val=vScroll.Value+amount;
			if(val<vScroll.Minimum){
				val=vScroll.Minimum;
			}else if(val>vScroll.Maximum){
				val=vScroll.Maximum;
			}
			vScroll.Value=val;
		}

		public int ScrollAmount{
			get{ return vScroll.SmallChange; }
			set{ vScroll.SmallChange=value; }
		}

		///<summary>Window size can change based on landscape mode and possibly other things in the future (perhaps different paper sizes).</summary>
		private void CheckWindowSize(){
			if(landscape){
				this.Width=942;
				PageHeight=650;
			}else{
				this.Width=692;
				PageHeight=900;
			}
			printPanel.Width=this.Width-42;
			vScroll.Location=new Point(this.Width-29,33);
		}

		public bool Landscape{
			get{ return landscape; }
			set{ landscape=value; CheckWindowSize(); }
		}

		public int MarginBottom{
			get{ return (printerGraph!=null)?(1100-printerMargins.Bottom):0; }
		}

		public Graphics Graph{
			get{ return (printerGraph!=null)?printerGraph:printPanel.backBuffer; }
		}

		public int PageHeight {
			get { return pageHeight; }
			set { pageHeight=value; CalculateVScrollMax(); }
		}

		///<summary>Must be set by the external printing algorithm in order to get page numbers working properly.</summary>
		public int TotalPages{
			get{ return totalPages; }
			set { totalPages=value; CalculatePageOffset(); labPageNum.Visible=(totalPages>0); CalculateVScrollMax(); }
		}

		public int GraphWidth{
			get{ return (printerGraph!=null)?printerMargins.Width:printPanel.Width; }
			set{ printPanel.Width=value; }
		}

		public int MinimumTimesToPrint{
			get{ return minimumTimesToPrint; }
			set{ minimumTimesToPrint=value; }
		}

		private void Display(){
			CalculatePageOffset();
			PrintCustom();
			printPanel.Invalidate(true);
		}

		private void butNextPage_Click(object sender,EventArgs e) {
			MoveScrollBar((CurPage()+1)*pageHeight-vScroll.Value);
			Display();
		}

		private void butPreviousPage_Click(object sender,EventArgs e) {
			if(vScroll.Value%pageHeight==0){
				MoveScrollBar(-pageHeight);
			}else{
				MoveScrollBar(CurPage()*pageHeight-vScroll.Value);
			}
			Display();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			curPrintPage=0;
			PrintoutOrientation orient=(landscape?PrintoutOrientation.Landscape:PrintoutOrientation.Portrait);
			if(PrinterL.TryPrint(pd1_PrintPage,printoutOrigin:PrintoutOrigin.AtMargin,printoutOrientation:orient)) {
				numTimesPrinted++;
			}
			printerGraph=null;
			Display();
		}

		private void pd1_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			printerGraph=e.Graphics;
			printerMargins=e.MarginBounds;
			printerGraph.TranslateTransform(0,-curPrintPage*(pageHeight+MarginBottom));
			PrintCustom();
			curPrintPage++;
			e.HasMorePages=(printGenerator!=null)&&(curPrintPage<totalPages);
		}

		private void vScroll_Scroll(object sender,ScrollEventArgs e) {
			Display();
		}

		private void FormPrintReport_FormClosing(object sender,FormClosingEventArgs e) {
			if(numTimesPrinted<minimumTimesToPrint){
				if(MessageBox.Show("WARNING: You should print this document at least "+
					(minimumTimesToPrint==1?"one time.":(minimumTimesToPrint+" times."))+
					"You may not be able to print this document again if you close it now. Are you sure you want to close this document?","",
					MessageBoxButtons.YesNo)==DialogResult.No){
					e.Cancel=true;
					return;
				}
			}
		}

	}
}