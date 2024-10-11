using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>Used to set rules for how often a patient should submit an image when checking in for their appointment via eClipboard. Example: insurance card or patient portrait. This is the grid on the right in eClipboard Images window.</summary>
	[Serializable]
	[CrudTable(IsSynchable = true)]
	public class EClipboardImageCaptureDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EClipboardImageCaptureDefNum;
		///<summary>FK to def.DefNum. Should match a DefNum that is in the in 'EClipboard Images' defcat or has (or had) the 'P' (Patient Pictures) usage in the 'Image Categories' defcat. This can be zero if there was no Patient Picture def set.</summary>
		public long DefNum;
		///<summary>True if the rule pertains to the patient self portrait. False if the rule is for an 'Eclipboard images' defcat definition.</summary>
		public bool IsSelfPortrait;
		///<summary>Frequency at which the patient should update the image, in days. If frequency is 0, patient will be prompted to submit image at each checkin.</summary>
		public int FrequencyDays;
		///<summary>FK to clinic.ClinicNum. Clinic the rule pertains to.</summary>
		public long ClinicNum;
		///<summary>Enum:EnumOcrCaptureType 0=Miscellaneous, 1=PrimaryInsFront, 2=PrimaryInsBack, 3=SecondaryInsFront, 4=SecondaryInsBack </summary>
		public EnumOcrCaptureType OcrCaptureType;

		///<summary></summary>
		public EClipboardImageCaptureDef Copy() {
			return (EClipboardImageCaptureDef)this.MemberwiseClone();
		}
	}

	/// <summary> The type of image. Miscelllaneuous is for all images without special behavior. </summary>
	public enum EnumOcrCaptureType {
		/// <summary>0- Catch-All type for imageCaptures without unique behavior </summary>
		[Description("Miscellaneous")]
		Miscellaneous,
		/// <summary>1</summary>
		[Description("Primary Insurance Card Front")]
		PrimaryInsFront,
		/// <summary>2</summary>
		[Description("Primary Insurance Card Back")]
		PrimaryInsBack,
		/// <summary>3</summary>
		[Description("Secondary Insurance Card Front")]
		SecondaryInsFront,
		/// <summary>4</summary>
		[Description("Secondary Insurance Card Back")]
		SecondaryInsBack
	}
}