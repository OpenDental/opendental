using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Most of these methods are simplified copies of methods found in ControlImagesJ.</summary>
	public class MountHelper {
		///<summary></summary>
		public static void DrawMount(Graphics g,Mount mount,Patient patient,List<MountItem> listMountItems,List<Bitmap> listBitmaps,List<Document> listDocuments,List<ImageDraw> listImageDraws){
			//we're at center of the mount, and working in mount coordinates
			//translate to UL of mount
			g.TranslateTransform(-mount.Width/2f,-mount.Height/2f);
			using SolidBrush solidBrushBack=new SolidBrush(mount.ColorBack);
			g.FillRectangle(solidBrushBack,0,0,mount.Width,mount.Height);
			for(int i=0;i<listMountItems.Count;i++){
				if(listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				DrawMountOne(g,mount,patient,listMountItems[i],listBitmaps[i],listDocuments[i]);
				if(listBitmaps[i] is null){
					DrawMountOneBigNumber(g,listMountItems[i]);
				}
			}
			//SELECT * FROM document WHERE patnum=293 AND mountitemnum > 0
			//outlines:
			using Pen penOutline=new Pen(Color.FromArgb(100,100,100));
			for(int i=0;i<listMountItems.Count;i++){
				if(listMountItems[i].ItemOrder==-1){//unmounted
					continue;
				}
				g.DrawRectangle(penOutline,listMountItems[i].Xpos,listMountItems[i].Ypos,
					listMountItems[i].Width,listMountItems[i].Height);//silver is 50% black
			}
			ImageHelper.DrawDrawings(g,0,listImageDraws);
		}

		private static void DrawMountOne(Graphics g,Mount mount,Patient patient,MountItem mountItem,Bitmap bitmap,Document document){
			GraphicsState graphicsStateMount=g.Save();
			//We are already in mount coords
			//translate from UL of mount to UL of mount item
			g.TranslateTransform(mountItem.Xpos,mountItem.Ypos);
			g.SetClip(new Rectangle(0,0,mountItem.Width,mountItem.Height));
			if(mountItem.TextShowing!=""){
				using SolidBrush solidBrushBack=new SolidBrush(mount.ColorTextBack);
				RectangleF rectangleF=new RectangleF(0,0,mountItem.Width,mountItem.Height);
				g.FillRectangle(solidBrushBack,rectangleF);
				using Font font=new Font(FontFamily.GenericSansSerif,mountItem.FontSize);
				using SolidBrush solidBrushFore=new SolidBrush(mount.ColorFore);
				string str=mountItem.TextShowing;
				str=Patients.ReplacePatient(str,patient);
				str=Mounts.ReplaceMount(str,mount);
				Clinic clinic=Clinics.GetClinic(patient.ClinicNum);
				str=Clinics.ReplaceOffice(str,clinic);
				g.DrawString(str,font,solidBrushFore,rectangleF);
				g.Restore(graphicsStateMount);
				return;
			}
			if(bitmap==null){
				g.Restore(graphicsStateMount);
				return;
			}
			//translate from UL of mount item to center of mount item
			g.TranslateTransform(mountItem.Width/2f,mountItem.Height/2f);
			//We are now working in the center of the crop area
			float scale;
			if(document.CropW > 0 && document.CropH > 0){
				scale=(float)mountItem.Width/document.CropW;//example 100/200=.5 because image needs to be smaller
			}
			else{
				//We don't have a real crop, so this handles the math.
				//We don't have access here to OpenDental.ImageTools.CalcScaleFit, so this is a copy of that code
				Size sizeUnscaled=ImageHelper.CalcSizeFit(bitmap.Width,bitmap.Height,document.DegreesRotated,new Size(mountItem.Width,mountItem.Height));
				//sizeUnscaled is correct proportions
				scale=(float)mountItem.Width/sizeUnscaled.Width;
			}
			g.ScaleTransform(scale,scale);
			//We are in bitmap coords from here down.
			//Translate from center of crop area to center of image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			g.TranslateTransform(-document.CropX,document.CropY);
			//Rotate and flip
			g.RotateTransform(document.DegreesRotated);
			if(document.IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Translate from center of image to UL corner of image
			g.TranslateTransform(-bitmap.Width/2f,-bitmap.Height/2f);
			//g.TranslateTransform(0,-_arrayBitmapsShowing[i].Height/2f);
			//g.InterpolationMode=InterpolationMode.HighQualityBicubic;//smooths image edges, but way too slow
			//Unlikely that anyone will care, but we could always do this just for rotated.  Or we could draw an antialiased rectangle around the edge.
			g.DrawImage(bitmap,0,0,bitmap.Width,bitmap.Height);
			g.Restore(graphicsStateMount);
		}

		private static void DrawMountOneBigNumber(Graphics g,MountItem mountItem){
			if(mountItem.ItemOrder==0){//text item
				return;
			}
			GraphicsState graphicsStateMount=g.Save();
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			g.TranslateTransform(mountItem.Xpos,mountItem.Ypos);//UL of mount position
			g.TranslateTransform(mountItem.Width/2,mountItem.Height/2);//center
			//So that they all look the same, we use the short dimension
			float heightFontPoint=mountItem.Height/15f*(96f/72f);
			if(mountItem.Width<mountItem.Height){
				heightFontPoint=mountItem.Width/15f*(96f/72f);
			}
			using Font font=new Font(FontFamily.GenericSansSerif,heightFontPoint);
			SizeF sizeString=g.MeasureString(mountItem.ItemOrder.ToString(),font);
			g.DrawString(mountItem.ItemOrder.ToString(),font,Brushes.Gray,-sizeString.Width/2,-sizeString.Height/2);
			g.Restore(graphicsStateMount);
		}

		public static Bitmap GetBitmapOfMountFromDb(long mountNum){
			Mount mount=Mounts.GetByNum(mountNum);
			Patient patient=Patients.GetPat(mount.PatNum);
			string patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
			List<ImageDraw> listImageDraws=ImageDraws.RefreshForMount(mount.MountNum);
			List<MountItem> listMountItems=MountItems.GetItemsForMount(mount.MountNum);
			List<Document> listDocuments=Documents.GetDocumentsForMountItems(listMountItems).ToList();
			Bitmap[] bitmapArray=new Bitmap[listDocuments.Count]; 
			for(int i=0;i<listDocuments.Count;i++){
				if(listDocuments[i]==null){
					bitmapArray[i]=null;
					continue;
				}
				bitmapArray[i]=LoadBitmap(isSingleDoc:false,listDocuments[i],patFolder);
				if(bitmapArray[i]!=null) {
					ImageHelper.ConvertCropIfNeeded(listDocuments[i],bitmapArray[i]);
				}
			}
			Bitmap bitmap=GetBitmapOfMount(mount,patient,listMountItems,bitmapArray.ToList(),listDocuments,listImageDraws);
			for(int i=0;i<bitmapArray.Length;i++){
				bitmapArray[i]?.Dispose();
			}
			return bitmap;
		}

		public static Bitmap GetBitmapOfMount(Mount mount,Patient patient,List<MountItem> listMountItems,List<Bitmap> listBitmaps,List<Document> listDocuments,List<ImageDraw> listImageDraws){
			Bitmap bitmap=new Bitmap(mount.Width,mount.Height);
			Graphics g=Graphics.FromImage(bitmap);
			g.TranslateTransform(bitmap.Width/2,bitmap.Height/2);//Center of image
			DrawMount(g,mount,patient,listMountItems,listBitmaps,listDocuments,listImageDraws);
			g.Dispose();
			return bitmap;
		}

		///<summary>Loads bitmap from disk, resizes, applies bright/contrast, and saves it to _arrayBitmapsShowing and/or _bitmapRaw.</summary>
		private static Bitmap LoadBitmap(bool isSingleDoc,Document document,string patFolder){
			//EnumLoadBitmapType loadBitmapType){//we're just going to do OnlyIdx
			Bitmap bitmapTemp=null;
			Bitmap bitmapResult=null;
			BitmapDicom bitmapDicomRaw=null;
			if(document.FileName.EndsWith(".dcm")){
				bitmapDicomRaw=ImageStore.OpenBitmapDicom(document,patFolder);
				if(bitmapDicomRaw==null){
					return null;
				}
			}
			else{
				bitmapTemp=ImageStore.OpenImage(document,patFolder);
				if(bitmapTemp==null){
					return null;
				}
			}
			if(isSingleDoc){
				//always IdxAndRaw
				//single images simply load up the whole unscaled image. Mounts load the whole image, but maybe at a different scale to match mount scale.
				if(document.FileName.EndsWith(".dcm")){
					bitmapResult=DicomHelper.ApplyWindowing(bitmapDicomRaw,document.WindowingMin,document.WindowingMax);
					bitmapTemp?.Dispose();
					return bitmapResult;
				}
				try{
					bitmapResult=new Bitmap(bitmapTemp);//can crash here on a large image (WxHx24 > 250M)
					//jordan I redid my math a few months later, and it's WxHx4, not 24.  Size in memory should not be a problem, 
					//so now I don't know why it chokes on large images or why I've watched my memory usage climb to 1G when loading images.  Revisit.
					ImageHelper.ApplyColorSettings(bitmapResult,document.WindowingMin,document.WindowingMax);
					//_bitmapRaw=new Bitmap(bitmapTemp);//or it can crash here
					bitmapTemp.Dispose();//frees up the lock on the file on disk
				}
				catch{
					//This happens with large images.  Not sure why yet.
					//It could be addressed by holding only one image in memory instead of two.  There are downsides.
					//It could also be addressed by downgrading the image or only showing the image at just enough resolution to match screen pixels.  Difficult.
					//It could also be addressed by compressing one bitmap into memory using a stream. e.g. _bitmapRaw. Seems good.
					//It could also be addressed by putting the raw image on the graphics card with Direct2D instead of having a second image to apply windowing to.
					//Yet another solution would be to maintain ref and lock to actual file on disk.
					//The final solution will be some combination of the above.
					//error message will show on return in SelectTreeNode because _bitmapRaw==null
				}
				finally{
					bitmapTemp.Dispose();//frees up the lock on the file on disk
				}
				return bitmapResult;
			}
			//From here down is mount=================================================================================================
			if(document.FileName.EndsWith(".dcm")){
				bitmapTemp=DicomHelper.ApplyWindowing(bitmapDicomRaw,document.WindowingMin,document.WindowingMax);
				bitmapResult=new Bitmap(bitmapTemp);
			}
			else{
				bitmapResult=new Bitmap(bitmapTemp);
				ImageHelper.ApplyColorSettings(bitmapResult,document.WindowingMin,document.WindowingMax);
			}
			bitmapTemp?.Dispose();//frees up the lock on the file on disk
			//_arrayBitmapsRaw[i].Clone(  //don't ever do it this way. Messes up dpi.
			return bitmapResult;
		}
	}
}
