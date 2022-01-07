using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace OpenDental.UI{
	public class ImageTools{
		///<summary>Calculates the crop rectangle, in image coords, for expand to fill a mount item.</summary>
		public static Rectangle CalcExpandToFill(Size sizeMountItem,Size sizeImage,int degreesRotated){
			//This assumes only slight rotations from 0,90,etc. To do this properly for rotations such as 30 deg, we would need a more complex algorithm similar to:
			//https://stackoverflow.com/questions/13128859/calculating-point-and-dimensions-of-maximum-rotated-rectangle-inside-rectangle
			//this matrix is GDI, so y origin is at top
			PointF pointFUL=new PointF(-sizeImage.Width/2f,-sizeImage.Height/2f);
			PointF pointFLL=new PointF(-sizeImage.Width/2f,sizeImage.Height/2f);
			Matrix matrix=new Matrix();
			matrix.Rotate(degreesRotated,MatrixOrder.Append);
			PointF[] pointFs={pointFUL,pointFLL};
			matrix.TransformPoints(pointFs);
			pointFUL=pointFs[0];
			pointFLL=pointFs[1];
			float widthResult=(float)Math.Round(Math.Min(Math.Abs(pointFUL.X)*2,Math.Abs(pointFLL.X)*2));
			float heightResult=(float)Math.Min(Math.Abs(pointFUL.Y)*2,Math.Abs(pointFLL.Y)*2);
			//example: widthResult=100,heightResult=50
			float ratioResultWH=widthResult/heightResult;//after the rotation//2
			float ratioMountItemWH=(float)sizeMountItem.Width/sizeMountItem.Height;//240/100=2.4
			if(ratioMountItemWH>ratioResultWH){//mount item is proportionally wider than result
				//so shorten result height
				heightResult=widthResult/ratioMountItemWH;
			}
			else{
				//so narrow result width
				widthResult=heightResult*ratioMountItemWH;
			}
			//now, back from GDI coordinates to our crop center coordinates
			Rectangle rectangle=new Rectangle(
				x:0,
				y:0,
				width:(int)Math.Round(widthResult),
				height:(int)Math.Round(heightResult));
			return rectangle;
		}

		///<summary>A general purpose algorithm that fits the given image size into the given canvas size. Result is a scale relative to 1, no change.  Takes into account degreesRotated to ensure that image is completely within canvas.</summary>
		public static float CalcScaleFit(Size sizeCanvas,Size sizeImage,float degreesRotated){
			Size sizeUnscaled=ImageHelper.CalcSizeFit(sizeImage.Width,sizeImage.Height,degreesRotated,sizeCanvas);
			//sizeUnscaled is correct proportions
			float scale=(float)sizeCanvas.Width/sizeUnscaled.Width;
			return scale;
		}

		///<summary>Converts an existing crop size into a "zoom" of image in a mount, with 100 being perfect fit in mount.  Takes into account degreesRotated.</summary>
		public static double CalcZoomMount(Size sizeMount,Size sizeImage,Size sizeCrop,float degreesRotated){
			if(sizeCrop.Width==0 || sizeCrop.Height==0){
				return 100;//Not zoomed.  Sized to fit.
			}
			Size sizeFit=ImageHelper.CalcSizeFit(sizeImage.Width,sizeImage.Height,degreesRotated,sizeMount);
			float zoom=(float)sizeFit.Width/sizeCrop.Width*100f;
			return zoom;
		}

		///<summary>Takes the user-specified zoom and converts it into a crop size, in image pixels.  The zoom is relative to ideal fit in mount.</summary>
		public static Size CalcZoomToSize(Size sizeMount,Size sizeImage,double zoom,int degreesRotated){
			Size sizeFit=ImageHelper.CalcSizeFit(sizeImage.Width,sizeImage.Height,degreesRotated,sizeMount);
			float widthCrop=(float)(sizeFit.Width/zoom*100f);
			float heightCrop=widthCrop/sizeMount.Width*sizeMount.Height;
			return new Size((int)Math.Round(widthCrop),(int)Math.Round(heightCrop));
		}

		///<summary>We do not dispose of the bitmap here. If crop was needed, then it saves that to the db as well as altering the document passed in.</summary>
		public static void ConvertCropIfNeeded(Document document,Bitmap bitmap){
			if(document is null || bitmap is null){
				return;
			}
			if(!document.IsCropOld){
				return;
			}
			if(document.CropW==0 || document.CropH==0){
				document.IsCropOld=false;
				Documents.Update(document);
				return;
			}
			Size sizeBitmap=bitmap.Size;
			if(document.DegreesRotated==0){
				if(document.IsFlipped){
					document.CropX=sizeBitmap.Width-document.CropX-document.CropW/2;//to center of crop
				}
				else{
					document.CropX=document.CropX+document.CropW/2;//to center of crop
				}
				document.CropX=-sizeBitmap.Width/2+document.CropX;//it's now relative to center of image
				document.CropY=document.CropY+document.CropH/2;//to center of crop
				document.CropY=sizeBitmap.Height/2-document.CropY;//it's now relative to center of image and flipped coord system
				document.IsCropOld=false;
				Documents.Update(document);
			}
			if(document.DegreesRotated==90){
				if(document.IsFlipped){
					int x=sizeBitmap.Height/2 //from center of image to newly located UL corner of image
						-document.CropY-document.CropH/2;//then to center of newly positioned crop
					document.CropY=-sizeBitmap.Width/2 //from center of image to newly located UL corner of image
						+document.CropX+document.CropW/2;//then to center of newly positioned crop
					document.CropX=x;
				}
				else{
					int x=sizeBitmap.Height/2 //from center of image to newly located UL corner of image
						-document.CropY-document.CropH/2;//then to center of newly positioned crop
					document.CropY=sizeBitmap.Width/2 //from center of image to newly located UL corner of image
						-document.CropX-document.CropW/2;//then to center of newly positioned crop
					document.CropX=x;
				}
				int w=document.CropW;
				document.CropW=document.CropH;
				document.CropH=w;
				document.IsCropOld=false;
				Documents.Update(document);
			}
			if(document.DegreesRotated==180){
				if(document.IsFlipped){
					document.CropX=-sizeBitmap.Width/2 //from center of image to newly located UL corner of image
						+document.CropX+document.CropW/2;//then to center of newly positioned crop
					
				}
				else{
					document.CropX=sizeBitmap.Width/2 //from center of image to newly located UL corner of image
						-document.CropX-document.CropW/2;//then to center of newly positioned crop
				}
				document.CropY=-sizeBitmap.Height/2 //from center of image to newly located UL corner of image
					+document.CropY+document.CropH/2;//then to center of newly positioned crop
				document.IsCropOld=false;
				Documents.Update(document);
			}
			if(document.DegreesRotated==270){
				if(document.IsFlipped){
					int x=-sizeBitmap.Height/2 //from center of image to newly located UL corner of image
						+document.CropY+document.CropH/2;//then to center of newly positioned crop
					document.CropY=sizeBitmap.Width/2 //from center of image to newly located UL corner of image
						-document.CropX-document.CropW/2;//then to center of newly positioned crop
					document.CropX=x;
				}
				else{
					int x=-sizeBitmap.Height/2 //from center of image to newly located UL corner of image
						+document.CropY+document.CropH/2;//then to center of newly positioned crop
					document.CropY=-sizeBitmap.Width/2 //from center of image to newly located UL corner of image
						+document.CropX+document.CropW/2;//then to center of newly positioned crop
					document.CropX=x;
				}
				int w=document.CropW;
				document.CropW=document.CropH;
				document.CropH=w;
				document.IsCropOld=false;
				Documents.Update(document);
			}
		}

	}
}
