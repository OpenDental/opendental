using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Most insurance plans are organized by employer.  This table keeps track of the list of employers.  The address fields were added at one point, but I don't know why they don't show in the program in order to edit.  Nobody has noticed their absence even though it's been a few years, so for now we are just using the EmpName and not the address.</summary>
	[Serializable]
	public class Employer:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EmployerNum;
		///<summary>Name of the employer.</summary>
		public string EmpName;
		///<summary>.</summary>
		public string Address;
		///<summary>Second line of address.</summary>
		public string Address2;
		///<summary>.</summary>
		public string City;
		///<summary>2 char in the US.</summary>
		public string State;
		///<summary>.</summary>
		public string Zip;
		///<summary>Includes any punctuation.</summary>
		public string Phone;

		public Employer Copy() {
			return (Employer)this.MemberwiseClone();
		}
	}

}













