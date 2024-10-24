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


/*
Jordan's notes.
Please nobody take action on this stuff yet. 
It's here for me to mull over and then to have a meeting about.
I'll let you know before we actually implement any of this.

Problems I have with this schema:
DefNum is used for dual purpose: either to a patient picture or to an EClipboardImageCapture defcat.
IsSelfPortrait is redundant
If user changes Defs for patient picture by removing the P or moving it, then this points to a folder that is no longer the patient pics folder.
If user changes any of the items in EClipboardImageCapture defcat, then this is now out of synch.

Advantages of current schema:
If multiple clinics are all linked to items in EClipboardImageCapture, then user could change the def to simultaneously update the text for all clinics.
If multiple clinics are used, the user doesn't have to keep entering instructions for each clinic
The EClipboardImageCapture items act as templates even if the user has not yet set up eClipboard
	But the "templates" are not very good and all nearly identical
	We can solve this by adding a way to copy from default for individual clinics.
	And maybe also a way to copy from our internal defaults.
If user removes P from Image Category in Defs, then the eClipboard might not know where to save the patient pic
	This is not a problem. In this case, just save in first category.

Proposed changes:
DefNum: Deprecate.
IsSelfPortrait: We will keep this. With DefNum column gone, this is no longer redundant.
New columns:
	ItemName, Example "Photo ID Front" or "Self Portrait"
	Instructions: Example: "Please take a picture of the front side of your photo ID"
I don't think those two columns should be available for self portrait unless the eClipboard is enhanced to support that text.
We will need a conversion to fill those two fields based on the old DefNums
EClipboardImageCapture gets deprected and hidden

UI:
Need a column to show OCR Capture Type
Edit windows for both grids should come up after using buttons.






*/