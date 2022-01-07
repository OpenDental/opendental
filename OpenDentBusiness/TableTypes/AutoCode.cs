using System;
using System.Collections;

namespace OpenDentBusiness{
	
	///<summary>An autocode automates entering procedures.  The user only has to pick composite, for instance, and the autocode figures out the code based on the number of surfaces, and posterior vs. anterior.  Autocodes also enforce and suggest changes to a procedure code if the number of surfaces or other properties change.</summary>
	[Serializable()]
	public class AutoCode:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutoCodeNum;
		///<summary>Displays meaningful decription, like "Amalgam".</summary>
		public string Description;
		///<summary>User can hide autocodes</summary>
		public bool IsHidden;
		///<summary>This will be true if user no longer wants to see this autocode message when closing a procedure. This makes it less intrusive, but it can still be used in procedure buttons.</summary>
		public bool LessIntrusive;

		public AutoCode Copy() {
			return (AutoCode)this.MemberwiseClone();
		}
	}	

	


}









