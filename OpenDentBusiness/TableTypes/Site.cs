using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Generally used by mobile clinics to track the temporary locations where treatment is performed, such as schools, nursing homes, and community centers.  Replaces the old school table.</summary>
	[Serializable()]
	public class Site : TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SiteNum;
		///<summary>.</summary>
		public string Description;
		///<summary>Notes could include phone, contacts, etc.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary></summary>
		public string Address;
		///<summary>Optional second address line.</summary>
		public string Address2;
		///<summary></summary>
		public string City;
		///<summary>2 Char in USA.  Used to store province for Canadian users.</summary>
		public string State;
		///<summary>Postal code.</summary>
		public string Zip;
		///<summary>FK to provider.ProvNum.  Default provider for the site.</summary>
		public long ProvNum;
		///<summary>Enum:PlaceOfService Describes where the site is located.</summary>
		public PlaceOfService PlaceService;

		public Site Copy() {
			return (Site)this.MemberwiseClone();
		}
	}
}
