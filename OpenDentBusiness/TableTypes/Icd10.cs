using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Other tables generally use the ICD10Code string as their foreign key.  It is implied that these are all ICD10CMs, although that may not be the case in the future.</summary>
	[Serializable]
	public class Icd10:TableBase {
		///<summary>Primary key. Also identical to "Order Number" column in ICD10 documentation.</summary>
		[CrudColumn(IsPriKey=true)]
		public long Icd10Num;
		///<summary>ICD-10-CM or ICD-10-PCS code. Dots are included. Not allowed to edit this column once saved in the database.</summary>
		public string Icd10Code;
		///<summary>Short Description provided by ICD10 documentation.</summary>
		public string Description;
		///<summary>0 if the code is a “header” – not valid for submission on a UB04. 1 if the code is valid for submission on a UB04.</summary>
		public string IsCode;


		///<summary></summary>
		public Icd10 Copy() {
			return (Icd10)this.MemberwiseClone();
		}

	}
}