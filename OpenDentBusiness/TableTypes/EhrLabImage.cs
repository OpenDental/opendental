using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Used to link images to an EHR lab.</summary>
	[Serializable()]
	public class EhrLabImage:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrLabImageNum;
		///<summary>FK to ehrlab.EhrLabNum.</summary>
		public long EhrLabNum;
		///<summary>FK to document.DocNum.  Will be -1 to indicate that lab is expecting image results.</summary>
		public long DocNum;
	}
}
