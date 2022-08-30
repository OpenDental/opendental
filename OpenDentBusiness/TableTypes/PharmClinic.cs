using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Links a pharmacy store to a clinic.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class PharmClinic : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PharmClinicNum;
		///<summary>FK to pharmacy.PharmacyNum.</summary>
		public long PharmacyNum;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;

		///<summary>Default constructor.</summary>
		public PharmClinic() {

		}

		public PharmClinic(long pharmacyNum,long clinicNum) {
			PharmacyNum=pharmacyNum;
			ClinicNum=clinicNum;
		}

		public PharmClinic Copy(){
			return (PharmClinic)this.MemberwiseClone();
		}	
	}
}

