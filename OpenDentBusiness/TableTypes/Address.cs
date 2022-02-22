using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	///<summary>For extra addresses.  Currently only for tax physical address for customers.</summary>
	public class Address : TableBase {
		///<summary></summary>
		[CrudColumn(IsPriKey=true)]
		public long AddressNum;
		///<summary>Address line one</summary>
		public string Address1;
		///<summary>Address line two</summary>
		public string Address2;
		///<summary></summary>
		public string City;
		///<summary></summary>
		public string State;
		///<summary></summary>
		public string Zip;
		///<summary>FK to patient.PatNum. Keeps track of the patient associated to this address.</summary>
		public long PatNumTaxPhysical;//long name in case we add other kinds
	}
}
