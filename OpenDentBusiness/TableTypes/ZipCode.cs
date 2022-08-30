using System;

namespace OpenDentBusiness{

	///<summary>Zipcodes are also known as postal codes.  Zipcodes are always copied to patient records rather than linked.  So items in this list can be freely altered or deleted without harming patient data.</summary>
	[Serializable]
	public class ZipCode:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ZipCodeNum;
		///<summary>The actual zipcode.</summary>
		public string ZipCodeDigits;
		///<summary>.</summary>
		public string City;
		///<summary>.</summary>
		public string State;
		///<summary>If true, then it will show in the dropdown list in the patient edit window.</summary>
		public bool IsFrequent;

		public ZipCode Copy() {
			return (ZipCode)this.MemberwiseClone();
		}
	}

	

}













