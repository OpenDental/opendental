using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Branding Profile for eClipboard customization. One (or none) to One relationship with clinics. Allows customers to customize the look of their eClipboard with a Clinic name and Logo. </summary>
	[Serializable]
	public class MobileBrandingProfile:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MobileBrandingProfileNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>The Clinic Name that will be shown on eClipboard CheckIn</summary>
		public string OfficeDescription;
		///<summary>eConnector will fetch this file. Same path for every computer, so maybe use a network shared file. Shows as 90x90 pixels.</summary>
		public string LogoFilePath;
		///<summary>The time that this object was last modified or inserted. Automatically updated by MySQL every time a row is added or changed. Used to determine if eClipboard needs to fetch an updated mobile branding profile. </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;

		///<summary>Memberwise clone</summary>
		public MobileBrandingProfile Copy() {
			return (MobileBrandingProfile)this.MemberwiseClone();
		}
	}
}
