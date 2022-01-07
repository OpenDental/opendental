using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.Imaging.Render;

namespace OpenDentBusiness {
	///<summary></summary>
	public class BitmapDicom {
		///<summary>This is the raw bitmap in 16 bit, prior to windowing.  Displays only support 8 bits, so conversion must still be performed.</summary>
		public ushort[] ArrayDataRaw;
		public int Width;
		public int Height;
		///<summary>The desired scale of the output bitmap.  Rarely used for specific situation.</summary>
		public double Scale;
		///<summary>Usually empty. Only used to temporarily store on import.</summary>
		public int WindowingMin;
		///<summary>Usually empty. Only used to temporarily store on import.</summary>
		public int WindowingMax;
	}

	public class DicomHelper{
		///<summary>Converts the 16 bit bitmap data to an 8 bit bitmap by using the supplied windowing range.  Windowing values can be between 0 and 255.  Can return null.</summary>
		public static Bitmap ApplyWindowing(BitmapDicom bitmapDicom,int windowingMin,int windowingMax){
			Bitmap bitmap=new Bitmap(bitmapDicom.Width,bitmapDicom.Height);
			BitmapData bitmapData=bitmap.LockBits(new Rectangle(0,0,bitmapDicom.Width,bitmapDicom.Height),ImageLockMode.ReadWrite,bitmap.PixelFormat);
			//convert the windowing values to 12 bit versions, which does mean slightly rough increments for now.  This is not an issue because no data degradation.
			//4096 total values, from 0 to 4095.
			//255 x 16 = 4080, which is the max value we will support for windowing for now.
			unsafe {
				//rawData count should be same as pixels.  4 bytes per pixel
				byte* pBytes = (byte*)bitmapData.Scan0.ToPointer();
				for(int i=0;i<bitmapDicom.ArrayDataRaw.Length;i++) {
					byte byteVal=0;
					if(bitmapDicom.ArrayDataRaw[i]/16f <= windowingMin) {
						byteVal = 0;//black
					}
					else if(bitmapDicom.ArrayDataRaw[i]/16f >= windowingMax) {
						byteVal = 255;//white
					}
					else {							
						byteVal = (byte)Math.Round( 255f*(bitmapDicom.ArrayDataRaw[i]/16f - windowingMin) / (windowingMax -windowingMin));
						//Examples using bytes/255: if window is .3 to .5 
						//.301->.005, .4->.5, .499->.995,  
					}
					//ARGB is stored as BGRA on little-endian machine. Alpha most significant, blue least.  Pointer to integer is stored the same.
					pBytes[i*4] =byteVal;//B
					pBytes[i*4+1] =byteVal;//G
					pBytes[i*4+2] =byteVal;//R
					pBytes[i*4+3] =255;//A
				}
			}
			//Marshal.Copy(byteArray, 0, bitmapData.Scan0, byteArray.Length);// Bulk copy pixel data from a byte array
			//Marshal.WriteInt16(bitmapData.Scan0, offsetInBytes, shortValue);// Or, for one pixel at a time
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public static void CalculateWindowingOnImport(BitmapDicom bitmapDicom){
			int[] histogram=new int[4096];
			for(int i=0;i<bitmapDicom.ArrayDataRaw.Length;i++){
				histogram[bitmapDicom.ArrayDataRaw[i]]++;
			}
			histogram[0]=0;//throw out the large number of black pixels
			int max=histogram.Max();
			int idxPeak=0;
			for(int i=0;i<histogram.Length;i++){
				if(histogram[i]==max){
					idxPeak=i;
					break;
				}
			}
			int threshold=150;
			bitmapDicom.WindowingMin=0;
			for(int i=idxPeak;i>=0;i--){//hunt down
				if(histogram[i]<threshold){
					bitmapDicom.WindowingMin=i/16;//converting from 12 bit to 8 bit
					break;
				}
			}
			bitmapDicom.WindowingMax=4095;
			for(int i=idxPeak;i<histogram.Length;i++){//hunt up
				if(histogram[i]<threshold){
					bitmapDicom.WindowingMax=i/16;
					break;
				}
			}
		}

		///<summary>Returns null if something goes wrong.</summary>
		public static BitmapDicom GetFromFile(string filename){
			if(!File.Exists(filename)){
				return null;
			}
			DicomFile dicomFile=DicomFile.Open(filename);
			return GetFromDicomFile(dicomFile);
		}

		///<summary>Returns null if something goes wrong.</summary>
		public static BitmapDicom GetFromBase64(string rawBase64){
			byte[] bytesRaw=Convert.FromBase64String(rawBase64);
			using MemoryStream memoryStream=new MemoryStream(bytesRaw);
			return GetFromStream(memoryStream);
		}

		///<summary>Returns null if something goes wrong.</summary>
		public static BitmapDicom GetFromStream(Stream stream){
			DicomFile dicomFile=DicomFile.Open(stream);
			return GetFromDicomFile(dicomFile);
		}

		private static BitmapDicom GetFromDicomFile(DicomFile dicomFile){
			BitmapDicom bitmapDicom=new BitmapDicom();
			DicomDataset dicomDataset=dicomFile.Dataset;
			string stringPhotometricInterp=dicomDataset.GetSingleValueOrDefault(new DicomTag(0x0028,0x0004),"");
			if(stringPhotometricInterp!="MONOCHROME2"){
				return null;
			}
			ushort bitsAllocated=dicomDataset.GetSingleValueOrDefault<ushort>(new DicomTag(0x0028,0x0100),0);
			if(bitsAllocated!=16){
				return null;
			}
			ushort bitsStored=dicomDataset.GetSingleValueOrDefault<ushort>(new DicomTag(0x0028,0x0101),0);
			if(bitsStored!=12){
				return null;
			}
			bitmapDicom.Height=dicomDataset.GetSingleValueOrDefault(new DicomTag(0x0028,0x0010),0);//rows
			if(bitmapDicom.Height==0){
				return null;
			}
			bitmapDicom.Width=dicomDataset.GetSingleValueOrDefault(new DicomTag(0x0028,0x0011),0);//columns
			if(bitmapDicom.Width==0){
				return null;
			}
			DicomDataset dicomDatasetDecoded=dicomDataset.Clone(DicomTransferSyntax.ExplicitVRLittleEndian);//uses codec to decompress
			DicomPixelData dicomPixelDataDecoded=DicomPixelData.Create(dicomDatasetDecoded,false);
			IPixelData iPixelData=PixelDataFactory.Create(dicomPixelDataDecoded,0);
			if(!(iPixelData is GrayscalePixelDataU16)){
				return null;//we cannot read other formats yet
			}
			bitmapDicom.ArrayDataRaw=new ushort[bitmapDicom.Height*bitmapDicom.Width];
			for(int r=0;r<bitmapDicom.Height;r++){
				for(int c=0;c<bitmapDicom.Width;c++){
					ushort pixelVal=(ushort)iPixelData.GetPixel(c,r);
					bitmapDicom.ArrayDataRaw[r*bitmapDicom.Width+c]=pixelVal;
				}
			}  
			return bitmapDicom;
		}

		/*
		///<summary>Gets the size of the bitmap at the designated file location.</summary>
		public static Size GetSizeBitmap(string filename){
			if(!File.Exists(filename)){
				return default;
			}
			DicomFile dicomFile=DicomFile.Open(filename);
			DicomDataset dicomDataset=dicomFile.Dataset;
			int height=dicomDataset.GetSingleValueOrDefault(new DicomTag(0x0028,0x0010),0);//rows
			int width=dicomDataset.GetSingleValueOrDefault(new DicomTag(0x0028,0x0011),0);//columns
			if(height==0 || width==0){
				return default;
			}
			return new Size(width,height);
		}*/


	}





}
