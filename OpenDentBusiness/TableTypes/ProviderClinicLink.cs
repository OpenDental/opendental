using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	///<summary>This linker table will enable providers to be associated with multiple clinics. If a provider does not have an entry in this table,
	///it means that provider is linked to all clinics. This is different from the ProviderClinic table. That table holds override information for
	///providers for certain clinics.</summary>
	[Serializable]
	[CrudTable(IsSynchable=true)]
	public class ProviderClinicLink:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProviderClinicLinkNum;
		///<summary>FK to provider.ProvNum</summary>
		public long ProvNum;
		///<summary>FK to clinic.ClinicNum. An entry of -1 means the provider is associated to no clinics.</summary>
		public long ClinicNum;

		public ProviderClinicLink() {

		}

		public ProviderClinicLink(long clinicNum,long provNum) {
			ProvNum=provNum;
			ClinicNum=clinicNum;
		}

		///<summary></summary>
		public ProviderClinicLink Copy(){
			return (ProviderClinicLink)this.MemberwiseClone();
		}

	}


}
