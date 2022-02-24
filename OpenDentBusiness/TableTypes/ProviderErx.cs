using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Tracks which providers have access to eRx based on NPI.  Synchronized with HQ.</summary>
	[Serializable,CrudTable(IsSynchable=true)]
	public class ProviderErx:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProviderErxNum;
		///<summary>FK to patient.PatNum.  Holder of registration key only for HQ record, in customer record this will be 0.</summary>
		public long PatNum;
		///<summary>NPI of a provider from the provider table.  May correspond to multiple records in the provider table.</summary>
		public string NationalProviderID;
		///<summary>Enum:ErxStatus Set to Enabled if the provider with the given NationalProviderID has access to eRx.
		///Pending statuses are treated as if Disabled.</summary>
		public ErxStatus IsEnabled;
		///<summary>True if HQ knows that the provider has completed the Identify Proofing (IDP) process and is allowed access to eRx.
		///A provider can be enabled even when this is false if the provider is an existing provider before version 15.4 (a legacy provider).</summary>
		public bool IsIdentifyProofed;
		///<summary>Set to true if the NationalProviderID has been sent to HQ.  Will be false in customer db until sent.
		///If true, this tells us that the IsEnabled and IsIdentityProofed flags are set according to HQ records.</summary>
		public bool IsSentToHq;
		///<summary>Set to true manually if the customer has completed their EPCS process.</summary>
		public bool IsEpcs;
		///<summary>Enum:ErxOption Identifies which eRx option is being used when asking HQ if they are enabled.</summary>
		public ErxOption ErxType;
		///<summary>User identifier used by the associated ErxType.  Only used by OD HQ.</summary>
		public string UserId;
		///<summary>Only used by OD HQ.</summary>
		public string AccountId;
		///<summary>FK to registrationkey.RegistrationKeyNum.  HQ only, links to the registration key used to make this providererx row.</summary>
		public long RegistrationKeyNum;

		///<summary></summary>
		public ProviderErx Clone() {
			return (ProviderErx)this.MemberwiseClone();
		}

	}
}