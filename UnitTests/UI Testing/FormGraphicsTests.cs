using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Forms;
using System.Xml;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;

namespace UnitTests{
	public partial class FormGraphicsTests : FormODBase{
		//This testing can only be done at 96dpi because WinForms doesn't automatically scale. We'll be removing our manual scaling.
		//For later: Print preview will be a stack of pages in a window using their visual objects.
		//So printing might need a different intermediary for converting the drawing commands. Or maybe not.

		private WpfControls.UI.Panel panel;

		public FormGraphicsTests(){
			InitializeComponent();
			InitializeLayoutManager();
			panel=new WpfControls.UI.Panel();
			elementHost.Child=panel;
		}

		private void FormGridTest_Load(object sender, EventArgs e){
			float zoom=LayoutManager.ScaleMyFont();
			panel.LayoutTransform=new System.Windows.Media.ScaleTransform(zoom,zoom);
			OpenDental.Drawing.Graphics g=OpenDental.Drawing.Graphics.ScreenInit(panel);
			PaintCanvas(g);
		}

		private void panelSheetPreview_Paint(object sender,PaintEventArgs e) {
			Graphics g=e.Graphics;
			DrawOld(g);
		}

		private void DrawOld(Graphics g){
			g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.TextRenderingHint=System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			g.DrawString("Hello World",Font,Brushes.Black,0,0);
			g.DrawLine(Pens.Black,5,20,50,50);
			//top row
			Rectangle rectangle=new Rectangle(5,60,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			StringFormat stringFormat=new StringFormat();
			g.DrawString("This is some longer text that should wrap",Font,Brushes.Blue,rectangle,stringFormat);
			rectangle=new Rectangle(105,60,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Center;
			System.Drawing.Font fontBold=new System.Drawing.Font(Font,FontStyle.Bold);
			g.DrawString("This is some longer text that should wrap",fontBold,Brushes.Blue,rectangle,stringFormat);
			rectangle=new Rectangle(205,60,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Far;
			System.Drawing.Font fontUnderlineBold=new System.Drawing.Font(Font,FontStyle.Underline|FontStyle.Bold);
			g.DrawString("This is some longer text that should wrap",fontUnderlineBold,Brushes.Blue,rectangle,stringFormat);
			//middle row
			rectangle=new Rectangle(5,125,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.LineAlignment=StringAlignment.Center;
			stringFormat.Alignment=StringAlignment.Near;
			g.DrawString("This is some longer text that should wrap",Font,Brushes.Blue,rectangle,stringFormat);
			rectangle=new Rectangle(105,125,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.LineAlignment=StringAlignment.Center;
			stringFormat.Alignment=StringAlignment.Center;
			g.DrawString("This is some longer text that should wrap",Font,Brushes.Blue,rectangle,stringFormat);
			rectangle=new Rectangle(205,125,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.LineAlignment=StringAlignment.Center;
			stringFormat.Alignment=StringAlignment.Far;
			g.DrawString("This is some longer text that should wrap",Font,Brushes.Blue,rectangle,stringFormat);
			//bottom row
			rectangle=new Rectangle(5,190,90,60);
			g.FillRectangle(Brushes.DarkGreen,rectangle);
			//g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.LineAlignment=StringAlignment.Far;
			stringFormat.Alignment=StringAlignment.Near;
			g.DrawString("This is some longer text that should wrap",Font,Brushes.White,rectangle,stringFormat);
			rectangle=new Rectangle(105,190,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.LineAlignment=StringAlignment.Far;
			stringFormat.Alignment=StringAlignment.Center;
			g.DrawString("This is some longer text that should wrap",Font,Brushes.Blue,rectangle,stringFormat);
			rectangle=new Rectangle(205,190,90,60);
			g.DrawRectangle(Pens.Black,rectangle);
			stringFormat=new StringFormat();
			stringFormat.LineAlignment=StringAlignment.Far;
			stringFormat.Alignment=StringAlignment.Far;
			g.DrawString("This is some longer text that should wrap",Font,Brushes.Blue,rectangle,stringFormat);
			SizeF sizeF=g.MeasureString("Measure this text",Font,100);
		}

		private void PaintCanvas(OpenDental.Drawing.Graphics g){
			OpenDental.Drawing.Font font=new OpenDental.Drawing.Font();
			g.DrawString("Hello World",font,System.Windows.Media.Colors.Black,0,0);
			g.DrawLine(System.Windows.Media.Colors.Black,5,20,50,50);
			//top row
			System.Windows.Rect rect=new System.Windows.Rect(5,60,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.Blue,rect);
			rect=new System.Windows.Rect(105,60,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			OpenDental.Drawing.Font fontBold=new OpenDental.Drawing.Font();
			fontBold.IsBold=true;
			g.DrawString("This is some longer text that should wrap",fontBold,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Center);
			rect=new System.Windows.Rect(205,60,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			OpenDental.Drawing.Font fontUnderlineBold=new OpenDental.Drawing.Font();
			fontUnderlineBold.IsBold=true;
			fontUnderlineBold.IsUnderline=true;
			g.DrawString("This is some longer text that should wrap",fontUnderlineBold,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Right);
			//middle row
			rect=new System.Windows.Rect(5,125,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Left,
				verticalAlignment:System.Windows.VerticalAlignment.Center);
			rect=new System.Windows.Rect(105,125,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Center,
				verticalAlignment:System.Windows.VerticalAlignment.Center);
			rect=new System.Windows.Rect(205,125,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Right,
				verticalAlignment:System.Windows.VerticalAlignment.Center);
			//bottom row
			rect=new System.Windows.Rect(5,190,90,60);
			g.FillRectangle(System.Windows.Media.Colors.DarkGreen,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.White,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Left,
				verticalAlignment:System.Windows.VerticalAlignment.Bottom);
			rect=new System.Windows.Rect(105,190,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Center,
				verticalAlignment:System.Windows.VerticalAlignment.Bottom);
			rect=new System.Windows.Rect(205,190,90,60);
			g.DrawRectangle(System.Windows.Media.Colors.Black,rect);
			g.DrawString("This is some longer text that should wrap",font,System.Windows.Media.Colors.Blue,rect,
				horizontalAlignment:System.Windows.HorizontalAlignment.Right,
				verticalAlignment:System.Windows.VerticalAlignment.Bottom);
		}

		private void butPrintWPF_Click(object sender,EventArgs e) {
			OpenDental.Drawing.Printout printout=new OpenDental.Drawing.Printout();
			printout.FuncPrintPage=pd_PrintPage2;
			WpfControls.PrinterL.TryPrint(printout);
		}

		///<summary>If you have a control on the screen that you want to print, use this to make a deep copy because it won't let you attach the original to any document.</summary>
		public static T CopyControl<T>(T frameworkElement){
			string stringXaml = XamlWriter.Save(frameworkElement);
			StringReader stringReader = new StringReader(stringXaml);
			XmlReader xmlReader = XmlReader.Create(stringReader);
			return (T)XamlReader.Load(xmlReader);
		}

		private void butPrintOld_Click(object sender,EventArgs e) {
			//PrintDocument printDocument=new PrintDocument();
			//printDocument.PrintPage+=printDocument_PrintPage;
			//printDocument.Print();
			PrinterL.TryPrintOrDebugClassicPreview(printDocument_PrintPage,Lan.g(this,"Image printed."));
		}

		private bool pd_PrintPage2(OpenDental.Drawing.Graphics g) {
			//e.Graphics.DrawString("Hello world",Font,Brushes.Black,0,0);
			PaintCanvas(g);
			return false;//only one page, so no more pages.
		}

		private void printDocument_PrintPage(object sender,PrintPageEventArgs e) {
			//e.Graphics.DrawString("Hello world",Font,Brushes.Black,0,0);
			DrawOld(e.Graphics);
			e.HasMorePages=false;
		}

		private void butPreviewOld_Click(object sender,EventArgs e) {
			
		}

		private void butPreviewWPF_Click(object sender,EventArgs e) {
			OpenDental.Drawing.Printout printout=new OpenDental.Drawing.Printout();
			printout.FuncPrintPage=pd_PrintPage2;
			WpfControls.PrinterL.TryPreview(printout);
		}
	}
}
