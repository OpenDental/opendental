using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Stores the list of insurance filing code subtypes.</summary>
	[Serializable()]
	public class InsFilingCodeSubtype : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsFilingCodeSubtypeNum;
		///<summary>FK to insfilingcode.insfilingcodenum</summary>
		public long InsFilingCodeNum;
		///<summary>The description of the insurance filing code subtype.</summary>
		public string Descript;
		
		public InsFilingCodeSubtype Clone(){
			return (InsFilingCodeSubtype)this.MemberwiseClone();
		}	
	}
}


