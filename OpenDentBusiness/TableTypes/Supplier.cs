using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>A company that provides supplies for the office, typically dental supplies.</summary>
	[Serializable()]
	public class Supplier : TableBase {
		/// <summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SupplierNum;
		/// <summary>.</summary>
		public string Name;
		/// <summary>.</summary>
		public string Phone;
		/// <summary>The customer ID that this office uses for transactions with the supplier</summary>
		public string CustomerId;
		/// <summary>Full address to website.  We might make it clickable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Website;
		/// <summary>The username used to log in to the supplier website.</summary>
		public string UserName;
		/// <summary>The password to log in to the supplier website.  Not encrypted or hidden in any way.</summary>
		public string Password;
		/// <summary>Any note regarding supplier.  Could hold address, CC info, etc.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;

		

			
	}

	

}









