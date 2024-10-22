using System;
using System.ComponentModel;
using System.Xml.Serialization;

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
		///<summary>Deprecated.</summary>
		public int FrequencyDays;
		///<summary>FK to clinic.ClinicNum. Clinic the rule pertains to.</summary>
		public long ClinicNum;
		///<summary>Enum:EnumOcrCaptureType 0=Miscellaneous, 1=PrimaryInsFront, 2=PrimaryInsBack, 3=SecondaryInsFront, 4=SecondaryInsBack </summary>
		public EnumOcrCaptureType OcrCaptureType;
		///<summary>Enum:EnumEClipFreq 0=Once, 1=EachTime, 2=TimeSpan. The frequency that an image capture will be submitted by patients. ResubmitInterval can only be set if Frequency is TimeSpan.</summary>
		public EnumEClipFreq Frequency;
		///<summary>If Frequency is EnumEClipFreq.TimeSpan, this will indicate the acceptable amount of time (measured in Years and Months) that can pass since the last time the patient has submitted this image capture.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan ResubmitInterval;

		///<summary></summary>
		public EClipboardImageCaptureDef Copy() {
			return (EClipboardImageCaptureDef)this.MemberwiseClone();
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ResubmitInterval",typeof(long))]
		public long ResubmitIntervalXml {
			get {
				return ResubmitInterval.Ticks;
			}
			set {
				ResubmitInterval=TimeSpan.FromTicks(value);
			}
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