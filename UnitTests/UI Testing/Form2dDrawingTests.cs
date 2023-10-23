using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace UnitTests
{
	public partial class Form2dDrawingTests : OpenDental.FormODBase{
		Direct2d d;

		public Form2dDrawingTests()
		{
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=45;
			d=Direct2d.FromControl(panelD2D);
			d.CreateRenderTarget();
		}

		private void Form2dDrawingTests_Load(object sender, EventArgs e)
		{

		}

		private void panel1_Paint(object sender, PaintEventArgs e){
			/*
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.FillRectangle(Brushes.White,panel1.ClientRectangle);
			g.TranslateTransform(5,5);
			for(int i=0;i<21;i++){
				g.DrawLine(new Pen(Color.Black,1+(i/10f)),0,0,10,10);
				g.TranslateTransform(10,0);
			}
			g.ResetTransform();
			g.TranslateTransform(7,20);
			for(int i=0;i<21;i++){
				GraphicsHelper.DrawLine(g,new Pen(Color.Black,1+(i/10f)),0,0,10,10);
				g.TranslateTransform(10,0);
			}
			g.ResetTransform();
			g.TranslateTransform(9,35);
			for(int i=0;i<21;i++){
				g.DrawLine(new Pen(Color.Black,1+(i/10f)),0,0,0,10);
				g.TranslateTransform(10,0);
			}
			g.ResetTransform();
			g.TranslateTransform(9,50);
			for(int i=0;i<21;i++){
				GraphicsHelper.DrawLine(g,new Pen(Color.Black,1+(i/10f)),0,0,0,10);
				g.TranslateTransform(10,0);
			}
			g.ResetTransform();
			g.TranslateTransform(5,65);
			for(int i=0;i<21;i++){
				GraphicsPath graphicsPath=new GraphicsPath();
				graphicsPath.AddLine(0,0,0,10);
				g.DrawPath(new Pen(Color.Black,1+(i/10f)),graphicsPath);
				graphicsPath.Dispose();
				g.TranslateTransform(10,0);
				//conclusion: drawing paths is not accurate at 1.5px because of GDI+ bug.  Solution: Draw path segments manually.  Paths can still be used for fill.
			}
			g.ResetTransform();
			g.TranslateTransform(5,80);
			for(int i=0;i<21;i++){
				//Pen pen=new Pen(Color.Black,1+(i/10f));
				Pen pen=new Pen(Color.Black,1+(i/10f)+0.01f);//this solves it for 1.5.  Draws fine at 1.51
				RectangleF rectangle=new RectangleF(0,-10,30,30);
				g.DrawArc(pen,rectangle,160,40);
				g.TranslateTransform(10,0);
				//This proves that curves also suffer from the same Pen bug.  
				//Option 1: Draw at 1.51, and to live with scaling failure between 1 and 1.5.
				//Option 2: Switch to Direct2D.  This is what I did.
			}
			g.ResetTransform();
			g.TranslateTransform(5,100);
			Bitmap bitmap=new Bitmap(100,90);
			Direct2d d=Direct2d.FromBitmap(bitmap);
			d.BeginDraw();
			d.Clear(Color.Pink);
			d.DrawLine(Color.Orange,0,0,50,50);
			d.EndDraw();
			d.Dispose();
			g.DrawImage(bitmap,0,0);
			g.ResetTransform();
			g.TranslateTransform(110,100);
			Direct2d d2=Direct2d.FromGraphics(g);
			d2.BeginDraw();
			d2.DrawLine(Color.Purple,110,100,160,150);
			d2.EndDraw();
			d2.Dispose();*/
		}

		private void panelD2D_Paint(object sender, PaintEventArgs e){
			//Items normally at class level or in other methods-------------------------------------------------------------------------
			
			//if(d.DeviceResourcesNeedCreate()){
				
				//d.CreateGradientBrush(0,Color.PaleGreen,Color.LightBlue,0,0,panelD2D.Width,panelD2D.Height);
				//Bitmap bitmap=(Bitmap)Bitmap.FromFile(@"E:\Documents\Shared Projects Subversion\Icons\RawFiles\DeleteX\DeleteX.png");
				//d.CreateBitmap(0,bitmap,22,22);
			//}
			//Items normally found here in paint---------------------------------------------------------------------------------------
			d.BeginDraw();
			d.Clear(Color.PaleGreen);
			
			//d.FillRectangleGradient(gradientNum:0,x:0,y:0,width:panelD2D.Width,height:panelD2D.Height);
			d.DrawText(Color.Black,x:100,y:100,width:100,height:100,8.5f,"This is a test!");
			/*float xPos=5;
			for(int i=0;i<21;i++){
				float thickness=1+(i/10f);
				d.DrawLine(Color.Black,xPos,5,xPos+10,15,thickness);
				d.DrawLine(Color.Black,xPos+.5f,20,xPos+.5f,30,thickness);
				xPos+=10;
			}*/
			d.DrawLine(Color.Blue,0,0,panelD2D.Width,panelD2D.Height);
			//d.DrawBitmap(bitmapNum:0,x:5,y:5,width:22,height:22);
			d.EndDraw();
			//d.Dispose();//normally at class level
		}

		private void Form2dDrawingTests_SizeChanged(object sender,EventArgs e) {
			if(d!=null){
				d.CreateRenderTarget();
			}
		}



	}

}
