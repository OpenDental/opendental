using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>A single certification that any employee may complete.</summary>
	[Serializable()]
	public class Cert:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CertNum;
		///<summary>.</summary>
		public string Description;
		///<summary>The exact name of a wiki page.</summary>
		public string WikiPageLink;
		///<summary>0-indexed.  This is a little tricky because a cert can be in multiple categories.  So users can only reorder when they are looking at the entire list of certs not ordered by category.</summary>
		public int ItemOrder;
		/// <summary>If hidden, then this cert won't normally show in the main list.</summary>
		public bool IsHidden;
		///<summary>FK to definition.DefNum.</summary>
		public long CertCategoryNum;

		///<summary></summary>
		public Cert Copy() {
			return (Cert)this.MemberwiseClone();
		}


	}
}




