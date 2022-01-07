using System;

namespace OpenDentBusiness {
	///<summary>Medical lab facility that performed the test procedure(s).  Contains data from the ZPS segment.  Each MedLab object can have one to
	///many places of service, each in a repetition of the ZPS segment.  Each repetition will be its own row in this table.</summary>
	[Serializable]
	public class MedLabFacility:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long MedLabFacilityNum;
		///<summary>ZPS.3 - Facility Name.  Medical lab location name that performed the testing.</summary>
		public string FacilityName;
		///<summary>ZPS.4.1 - Facility Address.</summary>
		public string Address;
		///<summary>ZPS.4.3 - Facility City.</summary>
		public string City;
		///<summary>ZPS.4.4 - Facility State or Province.  Upper case state abbreviation.</summary>
		public string State;
		///<summary>ZPS.4.5 - Facility Zip or Postal Code.</summary>
		public string Zip;
		///<summary>ZPS.5 - Facility Phone Number.</summary>
		public string Phone;
		///<summary>ZPS.7.1 - Facility Director Title.</summary>
		public string DirectorTitle;
		///<summary>ZPS.7.2 - Facility Director Last Name.</summary>
		public string DirectorLName;
		///<summary>ZPS.7.3 - Facility Director First Name.</summary>
		public string DirectorFName;

		///<summary></summary>
		public MedLabFacility Copy() {
			return (MedLabFacility)MemberwiseClone();
		}

	}

}