using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using CodeBase;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace OpenDentBusiness {
	public static class ImageHelper {
		///<summary>Takes in a mount object and finds all the images pertaining to the mount, then concatenates them together into one large, unscaled image and returns that image. Set imageSelected=-1 to unselect all images, or set to an image ordinal to highlight the image. The mount is rendered onto the given bitmapMount, so it must have been appropriately created by CreateBlankMountImage(). One can create a mount template by passing in arrays of zero length.</summary>
		public static void RenderMountImage(Bitmap bitmapMount, List<Bitmap> listBitmaps, List<MountItem> listMountItems, List<Document> listDocuments, int imageSelected) {
			using (Graphics g=Graphics.FromImage(bitmapMount)) {
				//Draw mount encapsulating background rectangle.
				g.Clear(Color.Black);
				RenderMountFrames(bitmapMount, listMountItems, imageSelected);
				for(int i=0;i<listMountItems.Count; i++) {
					g.FillRectangle(Brushes.Black, listMountItems[i].Xpos, listMountItems[i].Ypos,
						listMountItems[i].Width, listMountItems[i].Height);//draw box behind image
					RenderImageIntoMount(bitmapMount, listMountItems[i], listBitmaps[i], listDocuments[i]);
				}
			}
		}

		///<summary>Renders the hollow rectangles which represent the individual image frames into the given mount image.</summary>
		public static void RenderMountFrames(Bitmap bitmapMount,List<MountItem> listMountItems,int imageSelected) {
			using(Graphics g=Graphics.FromImage(bitmapMount)) {
				//Draw image encapsulating background rectangles.
				Pen penBorder=null;
				for(int i=0;i<listMountItems.Count;i++) {
					if(i==imageSelected) {
						penBorder=new Pen(Color.Yellow,2);
					}
					else {
						penBorder=new Pen(Color.SlateGray,2);
					}
					g.DrawRectangle(penBorder,listMountItems[i].Xpos,listMountItems[i].Ypos,listMountItems[i].Width,listMountItems[i].Height);
				}
				penBorder?.Dispose();
			}
		}

		///<summary>Renders the given image using the settings provided by the given document object into the location of the given mountItem object.</summary>
		public static void RenderImageIntoMount(Bitmap bitmapMount, MountItem mountItem, Bitmap bitmapMountItem, Document documentMountItem) {
			if(mountItem==null) {
				return;
			}
			using(Graphics g=Graphics.FromImage(bitmapMount)) {
				g.FillRectangle(Brushes.Black, mountItem.Xpos, mountItem.Ypos, mountItem.Width, mountItem.Height);//draw box behind image
				Bitmap image=ApplyDocumentSettingsToImage(documentMountItem,bitmapMountItem,ImageSettingFlags.ALL);
				if(image==null) {
					return;
				}
				float widthScale=((float)mountItem.Width) / image.Width;
				float heightScale=((float)mountItem.Height) / image.Height;
				float scale;
				if(widthScale < heightScale){
					scale=widthScale;
				}
				else{
					scale=heightScale;
				}
				RectangleF rectangleImage=new RectangleF(0,0,scale*image.Width,scale*image.Height);
				rectangleImage.X = mountItem.Xpos + mountItem.Width / 2 - rectangleImage.Width / 2;
				rectangleImage.Y = mountItem.Ypos + mountItem.Height / 2 - rectangleImage.Height / 2;
				g.DrawImage(image,rectangleImage);
				image.Dispose();
			}
		}

		///<summary>Returns true if the given filename contains a supported file image extension.</summary>
		public static bool HasImageExtension(string fileName) {
			string ext = Path.GetExtension(fileName).ToLower();
			//The following supported bitmap types were found on a microsoft msdn page:
			//==02/25/2014 - Added .tig as an accepted image extention for tigerview enhancement.
			return (ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".bmp" || ext == ".tif" ||
				ext == ".tiff" || ext == ".gif" || ext == ".emf" || ext == ".exif" || ext == ".ico" || ext == ".png" || ext == ".wmf"|| ext == ".tig");
		}

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

		///<summary>Returns a bitmap based on bitmapReference, with typically just windowing (brightness/contrast) applied.  It can also do the crop, rotate, and flip, but that's only when it's not live in the Images module.</summary>
		public static Bitmap ApplyDocumentSettingsToImage2(Document document, Bitmap bitmapReference, ImageSettingFlags imageSettingFlags) {
			if(bitmapReference==null || document==null) {
				return null;
			}
			if(document==null) {//Implies that the image should be returned "unaltered".
				//return (Bitmap)image.Clone();//this would keep the original resolution, which causes problems.
				return new Bitmap(bitmapReference);//resets the resolution to 96, just like it does for docs 15 lines down.
			}
			//CROP-Cropping rectangle is in raw image coordinates.  May be larger or smaller than original image.
			Bitmap bitmapOut=null;
			if((imageSettingFlags & ImageSettingFlags.CROP) != 0 && document.CropW > 0 && document.CropH > 0){
				bitmapOut=new Bitmap(document.CropW,document.CropH,PixelFormat.Format24bppRgb);
				using(Graphics g=Graphics.FromImage(bitmapOut)){
					//there's no scaling. It's just simple translation
					g.DrawImage(bitmapReference,-document.CropX,-document.CropY,bitmapReference.Width,bitmapReference.Height);//without W/H, g tries to scale
				}
			}
			else{//no crop
				bitmapOut=new Bitmap(bitmapReference.Width, bitmapReference.Height,PixelFormat.Format24bppRgb);
				using(Graphics g=Graphics.FromImage(bitmapOut)){
					g.DrawImage(bitmapReference,0,0,bitmapReference.Width,bitmapReference.Height);//without W/H, g tries to scale
				}
			}
			//FLIP AND ROTATE - must match the order of operations in the Images module drawing
			if((imageSettingFlags & ImageSettingFlags.FLIP) != 0) {
				if (document.IsFlipped) {
					bitmapOut.RotateFlip(RotateFlipType.RotateNoneFlipX);
				}
			}
			if((imageSettingFlags & ImageSettingFlags.ROTATE) != 0) {
				if (document.DegreesRotated % 360 == 90) {
					bitmapOut.RotateFlip(RotateFlipType.Rotate90FlipNone);
				}
				else if (document.DegreesRotated % 360 == 180) {
					bitmapOut.RotateFlip(RotateFlipType.Rotate180FlipNone);
				}
				else if (document.DegreesRotated % 360 == 270) {
					bitmapOut.RotateFlip(RotateFlipType.Rotate270FlipNone);
				}
			}
			//WINDOWING (BRIGHTNESS AND CONTRAST)
			if((imageSettingFlags & ImageSettingFlags.COLORFUNCTION) == 0 
				|| document.WindowingMax == 0 || (document.WindowingMax == 255 && document.WindowingMin == 0)) 
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

		///<summary>Applies the document specified cropping, flip, rotation, and windowing (brightness/contrast) to the bitmap and returns the resulting bitmap. Zoom and translation must be handled by the calling code. The returned image is always a new image that can be modified without affecting the original image.</summary>
		public static Bitmap ApplyDocumentSettingsToImage(Document document, Bitmap bitmapReference, ImageSettingFlags imageSettingFlags) {
			if(bitmapReference==null) {
				return null;
			}
			if(document==null) {//No doc implies no operations, implies that the image should be returned "unaltered".
				//return (Bitmap)image.Clone();//this would keep the original resolution, which causes problems.
				return new Bitmap(bitmapReference);//resets the resolution to 96, just like it does for docs 20 lines down.
			}
			//CROP - Implies that the croping rectangle must be saved in raw-image-space coordinates, 
			//with an origin of that equal to the upper left hand portion of the image.
			Rectangle rectangleCrop;
			if((imageSettingFlags & ImageSettingFlags.CROP) != 0 &&	//Crop not requested.
				document.CropW > 0 && document.CropH > 0)//No clip area yet defined, so no clipping is performed.
			{
				//todo: this is all wrong.  It's not supposed to be an intersection.  Crop rectangle can be bigger than original image.
				float[] cropDims = ODMathLib.IntersectRectangles(0, 0, bitmapReference.Width, bitmapReference.Height,//Intersect image rectangle with
					document.CropX, document.CropY, document.CropW, document.CropH);//document crop rectangle.
				if (cropDims.Length == 0) {//The entire image has been cropped away.
					return null;
				}
				//Rounds dims up, so that data is not lost, but possibly not removing all of what was expected.
				rectangleCrop = new Rectangle((int)cropDims[0], (int)cropDims[1],
					(int)Math.Ceiling(cropDims[2]), (int)Math.Ceiling(cropDims[3]));
			}
			else {
				rectangleCrop = new Rectangle(0, 0, bitmapReference.Width, bitmapReference.Height);//No cropping.
			}
			//Always use 32-bit images in memory. We could use 24-bit images here (it works in Windows, but MONO produces
			//output using 32-bit data on a 24-bit image in that case, providing horrible output). Perhaps we can update
			//this when MONO is more fully featured.
			Bitmap bitmapCropped = new Bitmap(rectangleCrop.Width, rectangleCrop.Height, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bitmapCropped);
			Rectangle rectangleCroppedDims = new Rectangle(0, 0, bitmapCropped.Width, bitmapCropped.Height);
			g.DrawImage(bitmapReference, rectangleCroppedDims, rectangleCrop, GraphicsUnit.Pixel);
			g.Dispose();
			//FLIP AND ROTATE - must match the operations in GetDocumentFlippedRotatedMatrix().
			if((imageSettingFlags & ImageSettingFlags.FLIP) != 0) {
				if (document.IsFlipped) {
					bitmapCropped.RotateFlip(RotateFlipType.RotateNoneFlipX);
				}
			}
			if((imageSettingFlags & ImageSettingFlags.ROTATE) != 0) {
				if (document.DegreesRotated % 360 == 90) {
					bitmapCropped.RotateFlip(RotateFlipType.Rotate90FlipNone);
				}
				else if (document.DegreesRotated % 360 == 180) {
					bitmapCropped.RotateFlip(RotateFlipType.Rotate180FlipNone);
				}
				else if (document.DegreesRotated % 360 == 270) {
					bitmapCropped.RotateFlip(RotateFlipType.Rotate270FlipNone);
				}
			}
			//APPLY BRIGHTNESS AND CONTRAST - 
			//TODO: should be updated later for more general functions 
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
					bitmapData = bitmapCropped.LockBits(new Rectangle(0, 0, bitmapCropped.Width, bitmapCropped.Height),
						ImageLockMode.ReadWrite, bitmapCropped.PixelFormat);
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
						bitmapCropped.UnlockBits(bitmapData);
					}
					catch {
					}
				}
			}
			return bitmapCropped;
		}


		///<summary>The returned image is square.</summary>
		public static Bitmap GetBitmapResized(Bitmap bitmapOriginal,int size) {
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

		///<summary>Gets the full sized image, with settings applied from doc.  Throws exceptions.</summary>
		public static Bitmap GetFullImage(Document doc,string patientFolder) {
			//No need to check RemotingRole; no call to db.
			string shortFileName=doc.FileName;
			//If no file name associated with the document, then there cannot be a thumbnail,
			//because thumbnails have the same name as the original image document.
			if(shortFileName.Length<1) {
				throw new ODException("No image file associated with document.");
			}
			string fullName=ODFileUtils.CombinePaths(patientFolder,shortFileName);
			//If the document no longer exists, then there is no corresponding thumbnail image.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !File.Exists(fullName)) {
				throw new ODException("No image file found for document.");
			}
			//If the specified document is not an image return 'not available'.
			if(!ImageHelper.HasImageExtension(fullName)) {
				throw new ODException("Document is not associated to an image file format.");
			}
			Bitmap sourceImage=null;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				sourceImage=new Bitmap(fullName);
			}
			else {//Cloud
				OpenDentalCloud.Core.TaskStateThumbnail state=CloudStorage.GetThumbnail(patientFolder,shortFileName);
				if(state.FileContent!=null) {
					using(MemoryStream stream=new MemoryStream(state.FileContent)) {
						sourceImage=new Bitmap(Image.FromStream(stream));
					}
				}
				else {
					sourceImage=new Bitmap(1,1);
				}
			}
			Bitmap fullImage=ImageHelper.ApplyDocumentSettingsToImage(doc,sourceImage,ImageSettingFlags.ALL);
			sourceImage.Dispose();
			return fullImage;
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
