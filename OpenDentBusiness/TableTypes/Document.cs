using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	///<summary>Represents a single document in the images module.</summary>
	[Serializable]
	[CrudTable(AuditPerms=CrudAuditPerm.ImageDelete|CrudAuditPerm.ImageEdit,IsLargeTable=true)]
	public class Document:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DocNum;
		/// <summary>Description of the document.</summary>
		public string Description;
		/// <summary>Date/time created.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateCreated;
		///<summary>FK to definition.DefNum. Categories for documents.</summary>
		public long DocCategory;
		/// <summary>FK to patient.PatNum.  The document will be located in the patient folder of this patient.</summary>
		public long PatNum;
		/// <summary>The name of the file. Does not include any directory info.</summary>
		public string FileName;
		/// <summary>Enum:ImageType Document, Radiograph, Photo, File</summary>
		public ImageType ImgType;
		/// <summary>True if flipped horizontally. A vertical flip would be stored as a horizontal flip plus a 180 rotation.</summary>
		public bool IsFlipped;
		/// <summary>Any positive or negative, including decimals.</summary>
		public float DegreesRotated;
		/// <summary>An optional list of tooth numbers. In Db, rigorously formatted as American numbers, and separated by commas.  For display, uses hyphens for sequences.  Very likely supports international tooth numbers, but not tested for that.</summary>
		public string ToothNumbers;
		/// <summary>MediumText, so max length=16M for API upload base64.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Note;
		/// <summary>True if the signature is in Topaz format rather than OD format.</summary>
		public bool SigIsTopaz;
		/// <summary>The encrypted and bound signature in base64 format.  The signature is bound to the byte sequence of the original image.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Signature;
		/// <summary>Crop rectangle X. May be negative. First, image is rotated as needed around center. Then, clipped to this crop rectangle.  X-Y is center of the crop rectangle relative to center of the image, and where positive is to the upper right of the center of the image.</summary>
		public int CropX;
		/// <summary>Crop rectangle Y. May be negative. First, image is rotated as needed around center. Then, clipped to this crop rectangle. X-Y is center of the crop rectangle relative to center of the image, and where positive is to the upper right of the center of the image.</summary>
		public int CropY;
		/// <summary>Crop rectangle Width in original image pixel scale.  May be zero if no cropping.  May be greater than original image width.</summary>
		public int CropW;
		/// <summary>Crop rectangle Height in original image pixel scale.  May be zero if no cropping.  May be greater than original image height.</summary>
		public int CropH;
		/// <summary>The lower value of the "windowing" (contrast/brightness) for radiographs.  Default is 0.  Max is 255.</summary>
		public int WindowingMin;
		/// <summary>The upper value of the "windowing" (contrast/brightness) for radiographs.  Default is 0(no windowing).  Max is 255. For 12 bit images with a max of 4096, the same max of 255 is used here, but it's just scaled proportionally (x16).</summary>
		public int WindowingMax;
		/// <summary>FK to mountitem.MountItemNum. If set, then this image will only show on a mount, not in the main tree. If set to 0, then no mount item is associated with this document.</summary>
		public long MountItemNum;
		/// <summary>Date/time last altered.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>The raw file data encoded as base64.  Only used if there is no AtoZ folder.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string RawBase64;
		///<summary>Thumbnail encoded as base64.  Only present if not using AtoZ folder. 100x100 pixels, jpg, takes around 5.5k.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string Thumbnail;
		///<summary>The primary key associated to a document hosted on an external source.</summary>
		public string ExternalGUID;
		///<summary>Enum:ExternalSourceType None, Dropbox, XVWeb. The source for the corresponding ExternalGUID.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public ExternalSourceType ExternalSource;
		///<summary>FK to provider.ProvNum. Optional. Used for radiographs.</summary>
		public long ProvNum;
		///<summary>Set to true as part of conversion to 21.4. Set back to false once the crop is converted to the new scheme. It would take too long to do this conversion in the normal script because it involves loading each image to obtain width and height. So this is a lazy conversion.</summary>
		public bool IsCropOld;

		///<summary>Returns a copy/clone of this Document.</summary>
		public Document Copy() {
			return (Document)this.MemberwiseClone();
		}
	}
	
		///<summary>Supported sources that help identify what the corresponding ExternalGUID column should be used for.</summary>
		public enum ExternalSourceType {
			///<summary>This is a document that is not stored in an external source.  All documents stored by Open Dental will be this type.</summary>
			None,
			///<summary>This document can be found in a corresponding Dropbox account.</summary>
			Dropbox,
			///<summary>This document is saved from a download from XVWeb program link.</summary>
			XVWeb,
		}
	
}


