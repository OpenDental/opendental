using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Manufacturer of a vaccine.</summary>
	[Serializable]
	public class DrugManufacturer:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DrugManufacturerNum;
		///<summary>.</summary>
		public string ManufacturerName;
		///<summary>An abbreviation of the manufacturer name.</summary>
		public string ManufacturerCode;//VARCHAR(20)/VARCHAR2(20).

		///<summary></summary>
		public DrugManufacturer Copy() {
			return (DrugManufacturer)this.MemberwiseClone();
		}

	}
}