using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>A vaccine definition.  Should not be altered once linked to VaccinePat.</summary>
	[Serializable]
	public class VaccineDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long VaccineDefNum;
		///<summary>RXA-5-1.</summary>
		public string CVXCode;
		///<summary>Name of vaccine.  RXA-5-2.</summary>
		public string VaccineName;
		///<summary>FK to drugmanufacturer.DrugManufacturerNum.</summary>
		public long DrugManufacturerNum;

		///<summary></summary>
		public VaccineDef Copy() {
			return (VaccineDef)this.MemberwiseClone();
		}

	}
}