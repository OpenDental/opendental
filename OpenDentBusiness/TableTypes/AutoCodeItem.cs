using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenDentBusiness{
	
	///<summary>Corresponds to the autocodeitem table in the database.  There are multiple AutoCodeItems for a given AutoCode.  Each Item has one ADA code.</summary>
	[Serializable()]
	public class AutoCodeItem:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutoCodeItemNum;
		///<summary>FK to autocode.AutoCodeNum</summary>
		public long AutoCodeNum;
		///<summary>Do not use</summary>
		public string OldCode;
		///<summary>FK to procedurecode.CodeNum</summary>
		public long CodeNum;

		///<summary>Only used in the validation section when closing FormAutoCodeEdit.  Will normally be empty.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public List<AutoCodeCond> ListConditions;

		public AutoCodeItem Copy() {
			return (AutoCodeItem)this.MemberwiseClone();
		}
	}





	
	


}









