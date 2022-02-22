using System;
using System.Drawing;
using OpenDentBusiness;

namespace OpenDental.Bridges {

	public class ApteryxImage {
		//leave items that we don't need commented out
		public long Id { get; set;}
		//public string SOPInstanceUID { get; set;}
		//public long Series { get; set;}
		//public string ImageSopClassUID { get; set;}
		//public string TransferSyntaxUID { get; set;}
		//public long InstanceNumber { get; set;}
		//public long AquistitionNumber { get; set;}
		public DateTime AcquisitionDate { get; set;}
		//public string AnatomicRegion { get; set;}
		//public long Frames { get; set;}
		public long Width { get; set;}
		public long Height { get; set;}
		//public long PixelFormat { get; set;}
		//public string ImageType { get; set;}
		public long FileSize { get; set;}
		//public string Checksum { get; set;}
		//public string Comments { get; set;}
		public string AdultTeeth { get; set;}
		public string DeciduousTeeth { get; set;}
		//public long DpmX { get; set;}
		//public long DpmY { get; set;}
		//public long DpmZ { get; set;}

		public string FormattedTeeth {
			get {
				return Tooth.FormatRangeForDisplay(AdultTeeth+(!string.IsNullOrEmpty(AdultTeeth) && !string.IsNullOrEmpty(DeciduousTeeth) ? "," : "")
					+DeciduousTeeth);
			}
		}
	}

	public class ApteryxThumbnail {
		public ApteryxImage Image;
		public Bitmap Thumbnail;
		public long PatNum;
	}
}
