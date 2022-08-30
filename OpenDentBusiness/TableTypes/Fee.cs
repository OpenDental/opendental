using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>There is one entry in this table for each fee for a single procedurecode.  So if there are 5 different fees stored for one procedurecode, then there will be five entries here.</summary>
	[Serializable]
	[CrudTable(IsSynchableBatchWriteMethods=true,IsSecurityStamped=true,HasBatchWriteMethods=true,AuditPerms=CrudAuditPerm.LogFeeEdit)]
	public class Fee:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FeeNum;
		///<summary>The amount usually charged.  If an amount is unknown, then the entire Fee entry is deleted from the database.  
		///The absence of a fee is shown in the user interface as a blank entry.
		///For clinic and/or provider fees, amount can be set to -1 which indicates that their fee should be blank and not use the default fee.</summary>
		public double Amount;
		///<summary>Do not use.</summary>
		public string OldCode;
		///<summary>FK to feesched.FeeSchedNum.</summary>
		public long FeeSched;
		///<summary>Not used.</summary>
		public bool UseDefaultFee;
		///<summary>Not used.</summary>
		public bool UseDefaultCov;
		///<summary>FK to procedurecode.CodeNum.</summary>
		public long CodeNum;
		///<summary>FK to clinic.ClinicNum.  (Used if localization of fees for a feesched is enabled)</summary>
		public long ClinicNum;
		///<summary>FK to provider.ProvNum.  (Used if localization of fees for a feesched is enabled)</summary>
		public long ProvNum;
		///<summary>FK to userod.UserNum.  Gets set automatically to the user logged in when the row is inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		///<summary></summary>
		public Fee Copy(){
			return (Fee)MemberwiseClone();
		}
	}

	///<summary>A better name for this is FeeKey, but that's already in use.  Used for faster fee lookup.</summary>
	public struct FeeKey2{
		public long CodeNum;
		public long FeeSchedNum;
		public FeeKey2(long codeNum,long feeSchedNum){
			CodeNum=codeNum;
			FeeSchedNum=feeSchedNum;
		}
	}
}













