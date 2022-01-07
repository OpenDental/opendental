using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Allows the user to specify DEA number override and other overrides for the provider at the specified 
	///clinic. This is different from the ProviderClinicLink table. That table records which providers are restricted to which clinics.</summary>
	[Serializable]
	[CrudTable(IsSynchable = true)]
	public class ProviderClinic:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProviderClinicNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>The DEA number for this provider and clinic.  The DEA number used to be stored in provider.DEANum.</summary>
		public string DEANum;
		///<summary>License number corresponding to the StateWhereLicensed.  Can include punctuation</summary>
		public string StateLicense;
		///<summary>Provider medical State ID.</summary>
		public string StateRxID;
		///<summary>The state abbreviation where the state license number in the StateLicense field is legally registered.</summary>
		public string StateWhereLicensed;
		///<summary>The merchant number for this provider and clinic.</summary>
		public string CareCreditMerchantId;

		///<summary></summary>
		public ProviderClinic Copy() {
			return (ProviderClinic)this.MemberwiseClone();
		}
	}
}

