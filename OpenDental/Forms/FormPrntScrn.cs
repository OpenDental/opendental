using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormPrntScrn : FormODBase {
		private Bitmap imageTemp;
		private int xPos;//x position of image being printed
		private int yPos;//y position of image being printed
		private int horRes=100;
		private	int vertRes=100;
//		private int startCropX;
//		private int startCropY;
//		private int endCropX;//for cropping will use later
//		private int endCropY;//for cropping will use later
		private int docWidth;
		private int docHeight;
		private int leftBound;
		private int rightBound;
		private int topBound;
		private int bottomBound;
//		private Rectangle recCrop;//for cropping will use later
//    private bool MouseIsDown=false;//for cropping will use later
//		private Graphics g;

		///<summary></summary>
		public FormPrntScrn(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPrntScrn_Load(object sender, System.EventArgs e) {
			try{
			  imageTemp=ODClipboard.GetImage();
				setImageSize();
        PrintReport(true);  //sets image as preview document
			}
			catch{
  	    MessageBox.Show(Lan.g(this,"Before using this tool, you must first save a screen shot by holding the Alt key down and pressing the PrntScrn button which is just above and to the right of the Backspace key.  You will not notice anything happen, but now you will have a screenshot in memory.  Then, open this tool again to view or print your screenshot."));	
				butPrint.Enabled=false;
				butExport.Enabled=false;
				DialogResult=DialogResult.Cancel;	
			}
		}
		
    private void FormPrntScrn_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
				printPreviewControl2.Location=new Point(0,0);
				printPreviewControl2.Height=ClientSize.Height-43;
				printPreviewControl2.Width=ClientSize.Width;	
				docWidth=811;//for cropping will use later
				docHeight=776;//for cropping will use later
				leftBound=(ClientSize.Width-docWidth)/2;//for cropping will use later
				rightBound=((ClientSize.Width-docWidth)/2)+docWidth;//for cropping will use later
				topBound=(ClientSize.Height-docHeight)/2;//for cropping will use later
				bottomBound=((ClientSize.Height-docHeight)/2)+docHeight;//for cropping will use later
		}

		private void setImageSize()  {
			if(imageTemp.Width>750)  {
				horRes+=(int)((imageTemp.Width-750)/8);
      }
			else {
//        horRes-=(int)((750-imageTemp.Width)/8);
				horRes=100;
			}
	    if (imageTemp.Height>1000)  {
        vertRes+=((imageTemp.Height-1000)/8);
      }
			else{
//        vertRes-=((1000-imageTemp.Height)/8);
				vertRes=100;
			}
			if(horRes>vertRes){
				vertRes=horRes;
			}
			else{
				horRes=vertRes;
			}
			imageTemp.SetResolution(horRes,vertRes);  //sets resolution to fit image on screen
		}

		///<summary></summary>
		public void PrintReport(bool justPreview){//TODO: Implement ODprintout pattern
			pd2=new PrintDocument();
			pd2.PrintPage += new PrintPageEventHandler(this.pd2_PrintPage);
//			pd2.DefaultPageSettings.Margins= new Margins(10,40,40,60);
			try {
				if(justPreview){
					printPreviewControl2.Document=pd2;
				}
				else if(PrinterL.SetPrinter(pd2,PrintSituation.Default,0,"Print screen printed")){
					pd2.Print();
				}
			}
			catch{
				MessageBox.Show(Lan.g(this,"Printer not available"));
			}
	  }

		private void butPrint_Click(object sender, System.EventArgs e) {
			PrintReport(false);
			DialogResult=DialogResult.Cancel;			
		}

		private void pd2_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
	//		g=e.Graphics;
			xPos=15;//starting pos
			yPos=(int)27.5;//starting pos
      e.Graphics.DrawImage(imageTemp,xPos,yPos);
  		//e.Graphics.DrawImage(imageTemp,e.Graphics.VisibleClipBounds.Left,e.Graphics.VisibleClipBounds.Top);
		}

		private void butExport_Click(object sender, System.EventArgs e) {
			using SaveFileDialog saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			saveFileDialog2.AddExtension=true;
			saveFileDialog2.Title=Lan.g(this,"Select Folder to Save Image To");
      saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath); 
			saveFileDialog2.DefaultExt="jpg";
			saveFileDialog2.Filter="jpg files(*.jpg)|*.jpg|All files(*.*)|*.*";
      saveFileDialog2.FilterIndex=1;
		  if(saveFileDialog2.ShowDialog()!=DialogResult.OK){
	   	  return;
			}
			try{
        imageTemp.Save(saveFileDialog2.FileName, ImageFormat.Jpeg);
      }
      catch{
        MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));  
			}
		}

		private void butZoomIn_Click(object sender, System.EventArgs e) {
			if(horRes>5){
			  horRes=horRes-(int)Math.Round(horRes*.25);
			  vertRes=vertRes-(int)Math.Round(vertRes*.25);
		  	imageTemp.SetResolution(horRes,vertRes);
				printPreviewControl2.Document=pd2;     
		  }  
			else {
			  horRes=5;
			  vertRes=5;
		  	imageTemp.SetResolution(horRes,vertRes);
				printPreviewControl2.Document=pd2; 
			}
		}

		private void butZoomOut_Click(object sender, System.EventArgs e) {
      if(horRes<1000){
			  horRes=horRes+(int)Math.Round(horRes*.25);
			  vertRes=vertRes+(int)Math.Round(vertRes*.25);
		  	imageTemp.SetResolution(horRes,vertRes);  //sets resolution to fit image on screen
				printPreviewControl2.Document=pd2;     
		  }
			else  {
			  horRes=1000;
			  vertRes=1000;
		  	imageTemp.SetResolution(horRes,vertRes);  //sets resolution to fit image on screen
 				PrintReport(true);   
				printPreviewControl2.Document=pd2; 
			}
		}

		private void printPreviewControl2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
/*for cropping will use later
  	
		  if(e.X >= leftBound && e.X <= rightBound && e.Y >= topBound && e.Y <= bottomBound)
			  startCropX=e.X;
		    startCropY=e.Y;
			  MouseIsDown=true;
*/				
		}

		private void printPreviewControl2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
/*for cropping will use later
    
		 if(!MouseIsDown){
        return; 
			}
			if(MessageBox.Show("Crop to Rectangle?","",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
			  g.Clear(Color.Black);
				printPreviewControl2.Document=pd2;
        MouseIsDown=false;
				return;
			}
      MouseIsDown=false;
			g.Clear(Color.DarkGray);
			endCropX=e.X;
			endCropY=e.Y;
      //Math.Abs gets the absolute value of an operation. This ensures positive value 
			recCrop=new Rectangle(startCropX,startCropY,(int)Math.Abs(endCropX-startCropX),(int)Math.Abs(endCropY-startCropY));
			MessageBox.Show(imageTemp.Width+"  "+imageTemp.Height);
			imageTemp=imageTemp.Clone(recCrop,PixelFormat.DontCare);
			MessageBox.Show(imageTemp.Width+"  "+imageTemp.Height);
			setImageSize();
      printPreviewControl2.Document=pd2;
*/
		}

		private void printPreviewControl2_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
/*for cropping will use later	
  	
    	textMouseX.Text=e.X.ToString();
			textMouseY.Text=e.Y.ToString();
		  if(MouseIsDown && e.X >= leftBound+xPos && e.X <= rightBound-40 && e.Y >= topBound && e.Y <= bottomBound){
				g=printPreviewControl2.CreateGraphics();
//        g.DrawImage(imageTemp,leftBound,topBound,imageTemp.Width,imageTemp.Height);
//  			g.Clear(Color.DarkGray);
				g.DrawImageUnscaled(imageTemp,(int)(leftBound+xPos-2.0001),(int)(topBound+5.5));//5
        if(e.X>startCropX){
					if(e.Y>startCropY){
				    recCrop=new Rectangle(startCropX,startCropY,(e.X-startCropX),(e.Y-startCropY));
				  }
					else{
				    recCrop=new Rectangle(startCropX,e.Y,(e.X-startCropX),(startCropY-e.Y));
					}
				}//end if(e.X>startCropX)
				else{
					if(e.Y>startCropY){
				    recCrop=new Rectangle(e.X,startCropY,(startCropX-e.X),(e.Y-startCropY));
					}
					else{
				    recCrop=new Rectangle(e.X,e.Y,(startCropX-e.X),(startCropY-e.Y));
					}
				}//end else
			  g.DrawRectangle(Pens.Blue,recCrop);
			}//end of if(MouseIsDown && e.X > leftBound && e.X < rightBound && e.Y > topBound && e.Y < bottomBound)
*/		
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
