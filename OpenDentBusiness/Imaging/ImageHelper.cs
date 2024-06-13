using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using CodeBase;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;

namespace OpenDentBusiness {
	public static class ImageHelper {
		///<summary>This alters the bitmap in raw memory without creating a copy.</summary>
		public static void ApplyColorSettings(Bitmap bitmap,int windowingMin,int windowingMax){
			if(windowingMax==0 || (windowingMax==255 && windowingMin==0)){
				return;
			}
			byte min=(byte)windowingMin;
			byte max=(byte)windowingMax;
			//todo: if 16 bit, then the above min and max would be a different datatype and proportionally larger
			BitmapData bitmapData = null;
			try {
				bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),ImageLockMode.WriteOnly, bitmap.PixelFormat);
				unsafe {
					byte* pBytes = (byte*)bitmapData.Scan0.ToPointer();
					//Apply to each 8bit color component separately. 
					//Stride is in bytes
					for(int i=0;i<bitmapData.Stride * bitmapData.Height;i++) {
						if(pBytes[i] <= min) {
							pBytes[i] = 0;//black
						}
						else if(pBytes[i] >= max) {
							pBytes[i] = 255;//white
						}
						else {							
							pBytes[i] = (byte)Math.Round( 255f*(pBytes[i] - min) / (max -min));
							//Examples using bytes/255: if window is .3 to .5 
							//.301->.005, .4->.5, .499->.995,  
						}
					}
				}
			}
			catch {
			}
			finally {
				try {
					bitmap.UnlockBits(bitmapData);
				}
				catch {
				}
			}
		}

		///<summary>Applies cropping, flip, rotation, and windowing (brightness/contrast) to the bitmap and returns the resulting bitmap. Zoom and translation must be handled by the calling code. The returned image is always a new image that can be modified without affecting the original image.  No longer honors ImageSettingFlag.  Always performs all actions.</summary>
		public static Bitmap ApplyDocumentSettingsToImage(Document document, Bitmap bitmapRaw, ImageSettingFlags imageSettingFlags) {
			if(bitmapRaw==null) {
				return null;
			}
			if(document==null) {//No doc implies no operations, implies that the image should be returned "unaltered".
				//return (Bitmap)image.Clone();//this would keep the original resolution, which causes problems.
				return new Bitmap(bitmapRaw);//resets the resolution to 96, just like it does for docs 20 lines down.
			}
			Bitmap bitmap=null;
			if(document.CropW > 0 && document.CropH > 0){
				bitmap=new Bitmap(document.CropW,document.CropH);
			}
			else{
				//can be rotated with no crop
				Size size=CalcSizeFit(bitmapRaw.Width,bitmapRaw.Height,document.DegreesRotated);
				bitmap=new Bitmap(size.Width,size.Height);
			}
			using Graphics g=Graphics.FromImage(bitmap);
			g.TranslateTransform(bitmap.Width/2,bitmap.Height/2);//center of crop area
			if(document.CropW>0 && document.CropH>0){
				g.SetClip(new Rectangle(-document.CropW/2,-document.CropH/2,document.CropW,document.CropH));
			}
			//Translate from center of crop area to center of image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			g.TranslateTransform(-document.CropX,document.CropY);
			//rotate around center of image
			g.RotateTransform(document.DegreesRotated);
			if(document.IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Translate from center of image to UL corner of image
			g.TranslateTransform(-bitmapRaw.Width/2,-bitmapRaw.Height/2);
			g.DrawImage(bitmapRaw,0,0,bitmapRaw.Width,bitmapRaw.Height);
			//DrawDrawings(g);//can't do this here because our drawing routine is in OD proper.
			//If needed, it's subsequent step.
			//APPLY BRIGHTNESS AND CONTRAST - 
			//(create inputValues and outputValues from stored db function/table).
			if((imageSettingFlags & ImageSettingFlags.COLORFUNCTION) != 0 &&
				document.WindowingMax != 0 && //Do not apply color function if brightness/contrast have never been set (assume normal settings).
				!(document.WindowingMax == 255 && document.WindowingMin == 0)) {//Don't apply if brightness/contrast settings are normal.
				float[] inputValues = new float[] {
					document.WindowingMin/255f,
					document.WindowingMax/255f,
				};
				float[] outputValues = new float[]{
					0,
					1,
				};
				BitmapData bitmapData = null;
				try {
					bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
						ImageLockMode.ReadWrite, bitmap.PixelFormat);
					unsafe {
						byte* pBytes;
						if(bitmapData.Stride < 0) {//Indicates bitmap is bottom up, but that should never happen
							pBytes = (byte*)bitmapData.Scan0.ToPointer() + bitmapData.Stride * (bitmapData.Height - 1);
						}
						else {
							pBytes = (byte*)bitmapData.Scan0.ToPointer();
						}
						//The following loop goes through each byte of each 32-bit value and applies the color function to it.
						//Thus, the same transformation is performed to all 4 color components equivalently for each pixel.
						for (int i = 0; i < bitmapData.Stride * bitmapData.Height; i++) {
							float colorComponent = pBytes[i] / 255f;
							float rangedOutput;
							if (colorComponent <= inputValues[0]) {
								rangedOutput = outputValues[0];
							}
							else if (colorComponent >= inputValues[inputValues.Length - 1]) {
								rangedOutput = outputValues[outputValues.Length - 1];
							}
							else {
								int j = 0;
								//j never increments here, so this must all be code for some future color enhancement idea that's not documented.
								//Seems like maybe a pixel needs to be between two given input values to get transformed to the same interpolated position between 2 output values
								while (!(inputValues[j] <= colorComponent && colorComponent < inputValues[j + 1])) {
									j++;
								}
								rangedOutput = ((colorComponent - inputValues[j]) * (outputValues[j + 1] - outputValues[j]))
									/ (inputValues[j + 1] - inputValues[j]);
							}
							pBytes[i] = (byte)Math.Round(255 * rangedOutput);
						}
					}
				}
				catch {
				}
				finally {
					try {
						bitmap.UnlockBits(bitmapData);
					}
					catch {
					}
				}
			}
			return bitmap;
		}

		///<summary>For a rotated rectangle, this calculates the cartesian rectangle size that would result in no whitespace around the sides. If you supply a sizeCanvas, then the resulting size will be additionally shrunk to match that proportion.  You would still need to scale it.</summary>
		public static Size CalcSizeExpandToFill(int widthImage,int heightImage,float degreesRotated,Size? sizeCanvas=null){
			//This assumes only slight rotations from 0,90,etc. To do this properly for rotations such as 30 deg, we would need a more complex algorithm similar to:
			//https://stackoverflow.com/questions/13128859/calculating-point-and-dimensions-of-maximum-rotated-rectangle-inside-rectangle
			//this matrix is GDI, so y origin is at top
			PointF pointFUL=new PointF(-widthImage/2f,-heightImage/2f);
			PointF pointFLL=new PointF(-widthImage/2f,heightImage/2f);
			Matrix matrix=new Matrix();
			matrix.Rotate(degreesRotated,MatrixOrder.Append);
			PointF[] pointFs={pointFUL,pointFLL};
			matrix.TransformPoints(pointFs);
			pointFUL=pointFs[0];
			pointFLL=pointFs[1];
			float widthResult=(float)Math.Round(Math.Min(Math.Abs(pointFUL.X)*2,Math.Abs(pointFLL.X)*2));
			float heightResult=(float)Math.Min(Math.Abs(pointFUL.Y)*2,Math.Abs(pointFLL.Y)*2);
			if(sizeCanvas is null){
				return new Size((int)Math.Round(widthResult),(int)Math.Round(heightResult));
			}
			//example: widthResult=100,heightResult=50
			float ratioImageWH=widthResult/heightResult;//after the rotation//2
			float ratioTargetWH=(float)sizeCanvas.Value.Width/sizeCanvas.Value.Height;//240/100=2.4
			if(ratioTargetWH>ratioImageWH){//2.4>2
				return new Size((int)Math.Round(widthResult),(int)Math.Round(widthResult/ratioTargetWH));//100x41
			}
			return new Size((int)Math.Round(heightResult*ratioTargetWH),(int)Math.Round(heightResult));//example: (50*1.7)x50=85x50
		}

		///<summary>For a rotatated rectangle, this calculates the cartesian rectangle size that completely contains it. If you supply a sizeCanvas, then the resulting size will be additionally enlarged to match that proportion.  You would still need to scale it.</summary>
		public static Size CalcSizeFit(int widthImage,int heightImage,float degreesRotated,Size? sizeCanvas=null){
			//this matrix is GDI, so y origin is at top
			PointF pointFUL=new PointF(-widthImage/2f,-heightImage/2f);
			PointF pointFLL=new PointF(-widthImage/2f,heightImage/2f);
			Matrix matrix=new Matrix();
			matrix.Rotate(degreesRotated,MatrixOrder.Append);
			PointF[] pointFs={pointFUL,pointFLL};
			matrix.TransformPoints(pointFs);
			pointFUL=pointFs[0];
			pointFLL=pointFs[1];
			float widthResult=(float)Math.Max(Math.Abs(pointFUL.X)*2,Math.Abs(pointFLL.X)*2);
			float heightResult=(float)Math.Max(Math.Abs(pointFUL.Y)*2,Math.Abs(pointFLL.Y)*2);
			if(sizeCanvas is null){
				return new Size((int)Math.Round(widthResult),(int)Math.Round(heightResult));
			}
			if(sizeCanvas.Value.Width==0){//this would cause ratioTargetWH to be zero, and then UE for later dividing by zero
				return new Size((int)Math.Round(widthResult),(int)Math.Round(heightResult));
			}
			float ratioImageWH=widthResult/heightResult;//after the rotation
			float ratioTargetWH=(float)sizeCanvas.Value.Width/sizeCanvas.Value.Height;
			if(ratioTargetWH>ratioImageWH){//new should be wider
				return new Size((int)Math.Round(heightResult*ratioTargetWH),(int)Math.Round(heightResult));
			}
			return new Size((int)Math.Round(widthResult),(int)Math.Round(widthResult/ratioTargetWH));
		}

		///<summary>In version 21.4, we changed how crop was calculated. This requires db conversion for each crop, but that was impossible in the script. So this is a lazy conversion. We do not dispose of the bitmap here. If crop was needed, then it saves that to the db as well as altering the document passed in.</summary>
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

		///<summary>Returns a bitmap based on bitmapRaw, with just windowing (brightness and contrast) applied.  Used in the live Images module.</summary>
		public static Bitmap CopyWithBrightContrast(Document document, Bitmap bitmapRaw){
			Bitmap bitmapOut=new Bitmap(bitmapRaw);
			if(document.WindowingMax == 0 || (document.WindowingMax == 255 && document.WindowingMin == 0)) 
			{
				return bitmapOut;
			}
			//Dicom images are at 16bit grayscale.  They must be windowed prior to passing in here or info will be lost.
			//Format16bppGrayScale is not supported by C#, but we might use it directly with C++ for faster response
			//Format48bppRgb might also be used.
			//Otherwise, it's a normal C# PixelFormat.Format24bppRgb _or_ Format32bppArgb for color or for 8 bit grayscale.
			byte min=(byte)document.WindowingMin;
			byte max=(byte)document.WindowingMax;
			//todo: if 16 bit, then the above min and max would be a different datatype and proportionally larger
			BitmapData bitmapData = null;
			//try {
				bitmapData = bitmapOut.LockBits(new Rectangle(0, 0, bitmapOut.Width, bitmapOut.Height),ImageLockMode.WriteOnly, bitmapOut.PixelFormat);
				unsafe {
					byte* pBytes = (byte*)bitmapData.Scan0.ToPointer();
					//Apply to each 8bit color component separately. 
					//Stride is in bytes
					for(int i=0;i<bitmapData.Stride * bitmapData.Height;i++) {
						if(pBytes[i] <= min) {
							pBytes[i] = 0;//black
						}
						else if(pBytes[i] >= max) {
							pBytes[i] = 255;//white
						}
						else {							
							pBytes[i] = (byte)Math.Round( 255f*(pBytes[i] - min) / (max -min));
							//Examples using bytes/255: if window is .3 to .5 
							//.301->.005, .4->.5, .499->.995,  
						}
					}
				}
			//}
			//catch {
			//}
			//finally {
			//	try {
					bitmapOut.UnlockBits(bitmapData);
			//	}
			//	catch {
			//	}
			//}
			return bitmapOut;
		}

		///<summary>Returns a bitmap based on bitmapOriginal, which already has windowing (brightness/contrast) applied.  It applies crop, rotate, and flip.</summary>
		public static Bitmap CopyWithCropRotate(Document document, Bitmap bitmapOriginal) {
			if(bitmapOriginal==null || document==null) {
				return null;
			}
			//return (Bitmap)image.Clone();//this would keep the original resolution, which causes problems.
			//return new Bitmap(bitmapRaw);//resets the resolution to 96.
			Bitmap bitmapOut=null;
			if(document.CropW > 0 && document.CropH > 0){
				bitmapOut=new Bitmap(document.CropW,document.CropH);
			}
			else{
				//can be rotated with no crop
				Size size=CalcSizeFit(bitmapOriginal.Width,bitmapOriginal.Height,document.DegreesRotated);
				bitmapOut=new Bitmap(size.Width,size.Height);
			}
			using Graphics g=Graphics.FromImage(bitmapOut);
			g.TranslateTransform(bitmapOut.Width/2,bitmapOut.Height/2);//center of crop area
			if(document.CropW>0 && document.CropH>0){
				g.SetClip(new Rectangle(-document.CropW/2,-document.CropH/2,document.CropW,document.CropH));
			}
			//Translate from center of crop area to center of image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			g.TranslateTransform(-document.CropX,document.CropY);
			//rotate around center of image
			g.RotateTransform(document.DegreesRotated);
			if(document.IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Translate from center of image to UL corner of image
			g.TranslateTransform(-bitmapOriginal.Width/2,-bitmapOriginal.Height/2);
			g.DrawImage(bitmapOriginal,0,0);
			return bitmapOut;
		}

		/// <summary>.</summary>
		public static void DrawDocument(Graphics g,Document document,Bitmap bitmap,List<ImageDraw> listImageDraws){
			//we're at the center of the crop area, and working in image coordinates
			if(document.CropW>0 && document.CropH>0){
				g.SetClip(new Rectangle(-document.CropW/2,-document.CropH/2,document.CropW,document.CropH));
			}
			//Translate from center of crop area to center of image
			//This step is not intuitive because our crop coordinates are positive to UR instead of the LR that we are using here.
			g.TranslateTransform(-document.CropX,document.CropY);
			//rotate around center of image
			g.RotateTransform(document.DegreesRotated);
			if(document.IsFlipped){
				Matrix matrix=new Matrix(-1,0,0,1,0,0);
				g.MultiplyTransform(matrix);
			}
			//Translate from center of image to UL corner of image
			g.TranslateTransform(-bitmap.Width/2,-bitmap.Height/2);
			g.DrawImage(bitmap,0,0,bitmap.Width,bitmap.Height);
			DrawDrawings(g,-document.DegreesRotated,listImageDraws);
			g.ResetClip();
		}

		public static void DrawDrawings(Graphics g,float rotateText,List<ImageDraw> listImageDraws){
			for(int i=0;i<listImageDraws.Count;i++){
				if(listImageDraws[i].DrawType==ImageDrawType.Text){
					GraphicsState graphicsState=g.Save();
					Point point=listImageDraws[i].GetTextPoint();
					g.TranslateTransform(point.X,point.Y);
					using Font font=new Font(FontFamily.GenericSansSerif,listImageDraws[i].FontSize);
					string str=listImageDraws[i].GetTextString();
					Size size= g.MeasureString(str,font).ToSize();
					Rectangle rectangle=new Rectangle(new Point(0,0),size);
					g.RotateTransform(rotateText);
					if(listImageDraws[i].ColorBack!=Color.Empty){
						using SolidBrush brushBack=new SolidBrush(listImageDraws[i].ColorBack);
						g.FillRectangle(brushBack,rectangle);
					}
					using SolidBrush solidBrush=new SolidBrush(listImageDraws[i].ColorDraw);
					g.DrawString(str,font,solidBrush,0,0);
					g.Restore(graphicsState);
				}
				if(listImageDraws[i].DrawType==ImageDrawType.Pen
					|| listImageDraws[i].DrawType==ImageDrawType.Line)
				{
					g.SmoothingMode=SmoothingMode.HighQuality;
					List<PointF> listPointFs=listImageDraws[i].GetPoints();
					using Pen pen=new Pen(listImageDraws[i].ColorDraw,2);//2 pixels wide
					using SolidBrush solidBrush=new SolidBrush(listImageDraws[i].ColorDraw);
					for(int p=1;p<listPointFs.Count;p++){//start at the second point.
						g.DrawLine(pen,listPointFs[p-1],listPointFs[p]);
						RectangleF rectangleF=new RectangleF(listPointFs[p].X-1,listPointFs[p].Y-1,2,2);
						g.FillEllipse(solidBrush,rectangleF);
					}
				}
			}
		}

		///<summary>Applies cropping, flip, and rotation to the bitmap and returns the resulting bitmap.</summary>
		public static Bitmap GetBitmapOfDocument(Document document,Bitmap bitmapShowing,List<ImageDraw> listImageDraws){
			Bitmap bitmap;
			if(document.CropW > 0 && document.CropH > 0) {
				bitmap=new Bitmap(document.CropW,document.CropH);
			}
			else {
				Size size=CalcSizeFit(bitmapShowing.Width,bitmapShowing.Height,document.DegreesRotated);
				bitmap=new Bitmap(size.Width,size.Height);
			}
			Graphics g=Graphics.FromImage(bitmap);
			g.TranslateTransform(bitmap.Width/2,bitmap.Height/2);//Center of image
			DrawDocument(g,document,bitmapShowing,listImageDraws);
			g.Dispose();
			return bitmap;
		}

		///<summary>Applies cropping, flip, and rotation to the bitmap and returns the resulting bitmap.</summary>
		public static Bitmap GetBitmapOfDocumentFromDb(long docNum){
			Document document=Documents.GetByNum(docNum);
			Patient patient=Patients.GetPat(document.PatNum);
			string patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
			List<ImageDraw> listImageDraws=ImageDraws.RefreshForDoc(document.DocNum);
			Bitmap bitmapShowing=LoadBitmap(document,patFolder);
			if(bitmapShowing is null){
				return null;
			}
			ConvertCropIfNeeded(document,bitmapShowing);
			Bitmap bitmapReturn=GetBitmapOfDocument(document,bitmapShowing,listImageDraws);
			bitmapShowing?.Dispose();
			return bitmapReturn;
		}

		///<summary></summary>
		public static Bitmap GetBitmapSquare(Bitmap bitmapOriginal,int size) {
			Bitmap bitmapReturn = new Bitmap(size, size);
			Graphics g = Graphics.FromImage(bitmapReturn);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			if (bitmapOriginal.Height > bitmapOriginal.Width) {//original is too tall
				float ratio = (float)size / (float)bitmapOriginal.Height;
				float w = (float)bitmapOriginal.Width * ratio;
				g.DrawImage(bitmapOriginal, (size - w) / 2f, 0, w, (float)size);
			}
			else {//original is too wide
				float ratio = (float)size / (float)bitmapOriginal.Width;
				float h = (float)bitmapOriginal.Height * ratio;
				g.DrawImage(bitmapOriginal, 0, (size - h) / 2f, (float)size, h);
			}
			g.Dispose();
			return bitmapReturn;
		}

		///<summary>Gets the image from disk, with settings applied from doc.  Throws exceptions.</summary>
		public static Bitmap GetImageCropped(Document document,string patientFolder) {
			//No need to check MiddleTierRole; no call to db.
			string shortFileName=document.FileName;
			//If no file name associated with the document, then there cannot be a thumbnail,
			//because thumbnails have the same name as the original image document.
			if(shortFileName.Length<1) {
				throw new ODException("No image file associated with document.");
			}
			string fileNameFull=ODFileUtils.CombinePaths(patientFolder,shortFileName);
			//If the document no longer exists, then there is no corresponding thumbnail image.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !File.Exists(fileNameFull)) {
				throw new ODException("No image file found for document.");
			}
			//If the specified document is not an image return 'not available'.
			if(!ImageHelper.HasImageExtension(fileNameFull)) {
				throw new ODException("Document is not associated to an image file format.");
			}
			Bitmap bitmapRaw=null;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				bitmapRaw=new Bitmap(fileNameFull);
			}
			else {//Cloud
				OpenDentalCloud.Core.TaskStateThumbnail state=CloudStorage.GetThumbnail(patientFolder,shortFileName);
				if(state.FileContent!=null) {
					using(MemoryStream stream=new MemoryStream(state.FileContent)) {
						bitmapRaw=new Bitmap(Image.FromStream(stream));
					}
				}
				else {
					bitmapRaw=new Bitmap(1,1);
				}
			}
			ImageHelper.ConvertCropIfNeeded(document,bitmapRaw);
			Bitmap bitmapReturn=ApplyDocumentSettingsToImage(document,bitmapRaw,ImageSettingFlags.ALL);
			bitmapRaw?.Dispose();
			return bitmapReturn;
		}

		///<summary>Returns true if the given filename contains a supported file image extension.</summary>
		public static bool HasImageExtension(string fileName) {
			string ext = Path.GetExtension(fileName).ToLower();
			//The following supported bitmap types were found on a microsoft msdn page:
			//==02/25/2014 - Added .tig as an accepted image extention for tigerview enhancement.
			return (ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".bmp" || ext == ".tif" ||
				ext == ".tiff" || ext == ".gif" || ext == ".emf" || ext == ".exif" || ext == ".ico" || ext == ".png" || ext == ".wmf"|| ext == ".tig");
		}

		///<summary>Loads bitmap from disk, resizes, applies bright/contrast. Does not rotate or crop</summary>
		private static Bitmap LoadBitmap(Document document,string patFolder){
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
			//if(isSingleDoc
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
				ApplyColorSettings(bitmapResult,document.WindowingMin,document.WindowingMax);
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

		/// <summary>Returns true if the document is an image that has been cropped, rotated, drawn on, or if the image has any windowing applied (brightness and contrast). Otherwise returns false.</summary>
		public static bool HasImageBeenEdited(Document document) {
			if(document==null || (!HasImageExtension(document.FileName) && !document.FileName.EndsWith(".dcm"))) {//not an image
				return false;
			}
			if(document.CropH!=0 || document.CropW!=0 || document.CropX!=0 || document.CropY!=0 || document.DegreesRotated!=0 || document.IsFlipped==true) {	
				return true;
			}
			if(document.WindowingMin != 0 || (document.WindowingMax != 255 && document.WindowingMax != 0)) {
				return true;
			}
			if(ImageDraws.RefreshForDoc(document.DocNum).Count>0) {
				return true;
			}
			return false;
		}

		

	}

	public enum ImageSettingFlags {
		NONE=0x00,
		ALL=0xFF,
		CROP=0x01,
		FLIP=0x02,
		ROTATE=0x04,
		///<summary>For now, this only applies brightness/contrast to images. It's called color because it could someday include other color filters.</summary>
		COLORFUNCTION=0x08,
	}
}
