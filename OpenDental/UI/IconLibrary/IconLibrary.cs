using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace OpenDental.UI{
	//Jordan's Instructions to self for maintenance
	//This enum is manually maintained with no automation.  A similar list is stored in the database, which is filled by Adding in Icon Manager.  
	//1. Add enumeration here
	//2. Add in Icon Manager to db.
	//3. Generate individual with no images.
	//4. Add Gen_ files to project
	//5. Generate from main page.
	//6. Add images 
	public enum EnumIcons{
		None,
		Account32,
		Acquire,
		Add,
		Appt32,
		ArrowLeft,
		ArrowRight,
		BreakAptX,
		Chart32,
		Chart32G,
		Chart32W,
		ChartMed32,
		CommLog,
		Complete,
		DeleteX,
		Email,
		Family32,
		ImageSelectorDoc,
		ImageSelectorFile,
		ImageSelectorFolder,
		ImageSelectorFolderWeb,
		ImageSelectorMount,
		ImageSelectorPhoto,
		ImageSelectorXray,
		Imaging32,
		Manage32,
		PatAdd,
		PatDelete,
		Patient,
		PatMoveFam,
		PatSelect,
		PatSetGuarantor,
		Probe,
		Recall,
		Text,
		TreatPlan32,
		TreatPlanMed32,
		Video,
		WebMail
	}

	///<summary></summary>
	public partial class IconLibrary{
		///<summary>This can be set to true if a customer computer is crashing. It forces use of GDI+ and no Direct2D.  Only downside is that icons will be blurry.</summary>
		public static bool OnlyGDI;

		//This didn't work.  The entire bitmap would need to be drawn in Direct2D, and then we might as well draw the whole control in D2D.
		/*
		///<summary>Supply the bitmap on which to draw.  There is no transparency at this interface, so the background should already be painted for the bitmap. The rectangle to draw on is usually just 0,0,bitmap.Width,bitmap.Height.</summary>
		public static void DrawOnBitmap(Bitmap bitmapBack,EnumIcons icon,Rectangle rectangle,bool designMode=false){
			if(icon==EnumIcons.None){
				return;
			}
			if(IsVector(icon) && !designMode){
				Direct2d d=Direct2d.FromBitmap(bitmapBack);
				int width=Width(icon);
				float scale=(float)rectangle.Width/width;
				IconSelector.DrawVectors(d,icon,rectangle.Location,scale);
				d.EndDraw();
				d.Dispose();
			}
			else{
				string base64=IconSelector.GetBase64(icon);
				if(base64==""){
					return;
				}
				Byte[] byteArray=Convert.FromBase64String(base64);
				MemoryStream memoryStream=new MemoryStream(byteArray);
				Bitmap bitmap=(Bitmap)Image.FromStream(memoryStream);
				memoryStream.Dispose();
				using Graphics g=Graphics.FromImage(bitmapBack);
				g.DrawImage(bitmap,rectangle);
				bitmap.Dispose();
				return;
			}
		}*/

		public static void Draw(Direct2d d,EnumIcons icon,Rectangle rectangle){
			if(icon==EnumIcons.None){
				return;
			}
			if(IsVector(icon)){
				int width=Width(icon);
				float scale=(float)rectangle.Width/width;
				IconSelector.DrawVectors(d,icon,rectangle.Location,scale);
			}
			else{
				string base64=IconSelector.GetBase64(icon);
				if(base64==""){
					return;
				}
				Byte[] byteArray=Convert.FromBase64String(base64);
				MemoryStream memoryStream=new MemoryStream(byteArray);
				Bitmap bitmap=(Bitmap)Image.FromStream(memoryStream);
				memoryStream.Dispose();
				d.DrawBitmapImmediate(bitmap,rectangle);
				bitmap.Dispose();
				return;
			}
		}

		///<summary>This works, but there are some bugs: stray lines of pixels when resizing, and halos due to Interpolation of edge pixels.  Graphics and Direct2D don't actually share the same context very well, even when they take turns.</summary>
		public static void Draw(Graphics g,EnumIcons icon,Rectangle rectangle,Color colorGradientTop,Color colorGradientBottom,bool designMode=false){
			if(icon==EnumIcons.None){
				return;
			}
			if(IsVector(icon) && !designMode && !OnlyGDI){
				//This top attempt looked terrible unless using the d.Clear.==========================================================
				/*
				g.InterpolationMode=InterpolationMode.HighQualityBicubic;
				Bitmap bitmap=new Bitmap(rectangle.Width,rectangle.Height);
				Direct2d d=Direct2d.FromBitmap(bitmap);
				d.Clear(SystemColors.Control);
				int width=Width(icon);
				float scale=(float)rectangle.Width/width;
				IconSelector.DrawVectors(d,icon,new Point(0,0),scale);//draw the vectors on the bitmap
				d.EndDraw();
				d.Dispose();
				g.DrawImage(bitmap,rectangle);//draw the bitmap to our main graphics
				bitmap.Dispose();*/
				//===================================================================================================================
				//This works, but leaves a few stray lines of pixels when resizing. It also seems to leave OD hanging on some computers
				/*
				int width=Width(icon);
				float scale=(float)rectangle.Width/width;
				Direct2d d=Direct2d.FromGraphics(g);
				d.BeginDraw();
				IconSelector.DrawVectors(d,icon,rectangle.Location,scale);
				d.EndDraw();
				d.Dispose();*/
				//===================================================================================================================
				//This is a hassle to match up the gradient. So it works fine, but it's just not going to scale well.
				//There are three possible future strategies:
				//1. Draw entire button in DirectX as a bitmap. Would need to also consider caching the bitmap to improve performance. Still a bit clumsy.
				//2. Draw entire button in DirectX using hWnd. Would take a lot of work to get the framework perfected. This could get too complex.
				//3. Stick with this code, redrawing gradients where needed. This might actually be a really good compromise.
				using Bitmap bitmap=new Bitmap(rectangle.Width,rectangle.Height);
				Direct2d d=null;
				try{//need to narrow this scope
					d=Direct2d.FromBitmap(bitmap);
				}
				catch{
					OnlyGDI=true;//from now on
					Draw(g,icon,rectangle,colorGradientTop,colorGradientBottom,designMode);//draws the non-vector version
					d?.Dispose();
					return;
				}
				d.BeginDraw();
				d.CreateGradientBrush(0,colorGradientTop,colorGradientBottom,0,0,0,rectangle.Height);
				//d.CreateGradientBrush(0,Color.Red,Color.Red,0,0,0,rectangle.Height);
				d.FillRectangleGradient(0,0,0,rectangle.Width,rectangle.Height);
				int width=Width(icon);
				float scale=(float)rectangle.Width/width;
				IconSelector.DrawVectors(d,icon,new Point(0,0),scale);//draw the vectors on the bitmap
				d.EndDraw();
				g.DrawImage(bitmap,rectangle);//draw the bitmap to our main graphics
				d.Dispose();
			}
			else{
				string base64=IconSelector.GetBase64(icon);
				if(base64==""){
					return;
				}
				Byte[] byteArray=Convert.FromBase64String(base64);
				MemoryStream memoryStream=new MemoryStream(byteArray);
				Bitmap bitmap=(Bitmap)Image.FromStream(memoryStream);
				memoryStream.Dispose();
				if(icon!=EnumIcons.Chart32G && icon!=EnumIcons.Chart32W){
					g.DrawImage(bitmap,rectangle);
					bitmap.Dispose();
					return;
				}
				//Everything below this point is just for the chart.
				//Chart bitmap needs high quality. Downside is white halo, so draw it slighly smaller, then cover the halo
				GraphicsState graphicsState=g.Save();
				g.InterpolationMode=InterpolationMode.HighQualityBicubic;
				//if(icon==EnumIcons.Chart32G){
					//g.FillRectangle(Brushes.Red,rectangle);
				//}
				//Rectangle rectangleSmaller=Rectangle.Inflate(rectangle,-1,-1);
				g.DrawImage(bitmap,rectangle);
				bitmap.Dispose();
				if(icon==EnumIcons.Chart32G){
					//this is the one with the white edge
					using(Pen pen=new Pen(Color.FromArgb(240,240,240))){
						g.DrawRectangle(pen,rectangle.X+0.75f,rectangle.Y+0.75f,rectangle.Width-1.25f,rectangle.Height-1.25f);
					}
				}
				g.Restore(graphicsState);
			}
		}

		/*
		public static Bitmap GetBitmap(Graphics g,EnumIcons icon,Size size){
			if(IconSelector.IsVector(icon)){
				int width=IconSelector.Width(icon);
				float scale=(float)size.Width/width;
				Bitmap bitmap=new Bitmap(size.Width,size.Height);
				Direct2d d=Direct2d.FromBitmap(bitmap);
				//d.Clear(Color.Transparent);
				IconSelector.DrawVectors(d,icon,new Point(0,0),scale);
				d.EndDraw();
				d.Dispose();
				return bitmap;
			}
			else{
				Byte[] byteArray=Convert.FromBase64String(IconSelector.GetBase64(icon));
				MemoryStream memoryStream=new MemoryStream(byteArray);
				Bitmap bitmap=(Bitmap)Image.FromStream(memoryStream);
				memoryStream.Dispose();
				return bitmap;
			}
		}*/

		///<summary>This draws with the greyed-out look for disabled buttons.</summary>
		public static void DrawDisabled(Graphics g,EnumIcons icon,Rectangle rectangle){
			if(icon==EnumIcons.None){
				return;
			}
			Byte[] byteArray=Convert.FromBase64String(IconSelector.GetBase64(icon));
			MemoryStream memoryStream=new MemoryStream(byteArray);
			Bitmap bitmap=(Bitmap)Image.FromStream(memoryStream);
			memoryStream.Dispose();
			Bitmap bitmapDisabled=new Bitmap(bitmap.Width,bitmap.Height);
			Graphics gfx=Graphics.FromImage(bitmapDisabled);
			ControlPaint.DrawImageDisabled(gfx,bitmap,0,0,ColorOD.Control);
			g.DrawImage(bitmapDisabled,rectangle);
			gfx.Dispose();
			bitmapDisabled.Dispose();
			bitmap.Dispose();
		}

		///<summary>32 or 22</summary>
		public static int Width(EnumIcons icon){
			switch(icon){
				case EnumIcons.Account32:
				case EnumIcons.Appt32:
				case EnumIcons.Family32:
				case EnumIcons.Imaging32:
				case EnumIcons.Manage32:
				case EnumIcons.TreatPlan32:
					return 32;
				default:
					return 22;
			}
		}

		public static bool IsVector(EnumIcons icon){
			switch(icon){
				case EnumIcons.ArrowLeft:
				case EnumIcons.ArrowRight:
				case EnumIcons.ChartMed32:
				case EnumIcons.Chart32:
				case EnumIcons.Chart32G:
				case EnumIcons.Chart32W:
				case EnumIcons.Probe:
				case EnumIcons.TreatPlanMed32:
				case EnumIcons.ImageSelectorDoc:
				case EnumIcons.ImageSelectorPhoto:
				case EnumIcons.ImageSelectorXray:
					return false;
				default:
					return true;
/*
				case EnumIcons.Account32:
				case EnumIcons.Add:
				case EnumIcons.Appt32:
				case EnumIcons.BreakAptX:
				case EnumIcons.CommLog:
				case EnumIcons.Complete:
				case EnumIcons.DeleteX:
				case EnumIcons.Email:
				case EnumIcons.Family32:
				case EnumIcons.Imaging32:
				case EnumIcons.Manage32:
				case EnumIcons.PatAdd:
				case EnumIcons.Patient:
				case EnumIcons.PatMoveFam:
				case EnumIcons.PatSelect:
				case EnumIcons.Recall:
				case EnumIcons.Text:
				case EnumIcons.TreatPlan32:
				case EnumIcons.WebMail:
					return true;
				default:
					return false;*/
			}
		}

	}

		
}


//https://github.com/jingwood/d2dlib/issues/2