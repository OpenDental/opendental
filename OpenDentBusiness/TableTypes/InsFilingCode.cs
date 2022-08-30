using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>An optional field on insplan and claims.  This lets user customize so that they can track insurance types. Only used for e-claims. Typically two characters. Examples: CI for Commercial Insurance, VA for Veterans, or WC for Worker's Comp.</summary>
	[Serializable()]
	public class InsFilingCode : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsFilingCodeNum;
		///<summary>Description of the insurance filing code.</summary>
		public string Descript;
		///<summary>Code for electronic claim.</summary>
		public string EclaimCode;
		///<summary>Display order for this filing code within the UI.  0-indexed.</summary>
		public int ItemOrder;
		///<summary>FK to definition.DefNum.  Reporting Group.</summary>
		public long GroupType;
		///<summary>If set to true, and the patient's secondary insurance plan uses this insfilingcode, the secondary insurance plan will 
		///not be populated on primary e-claims or paper claims.</summary>
		public bool ExcludeOtherCoverageOnPriClaims;

		public InsFilingCode Clone(){
			return (InsFilingCode)this.MemberwiseClone();
		}	

	}
}



