using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>AutoCode condition.  Always attached to an AutoCodeItem, which is then, in turn, attached to an autocode.  There is usually only one or two conditions for a given AutoCodeItem.</summary>
	[Serializable()]
	public class AutoCodeCond:TableBase{//
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutoCodeCondNum;
		///<summary>FK to autocodeitem.AutoCodeItemNum.</summary>
		public long AutoCodeItemNum;
		///<summary>Enum:AutoCondition </summary>
		public AutoCondition Cond;

		public AutoCodeCond Copy() {
			return (AutoCodeCond)this.MemberwiseClone();
		}
	}



	

	


}









