using System;
using System.Collections;

namespace OpenDentBusiness{
	///<summary>Fee schedule names used to be in the definition table, but now they have their own table.  We are about to have many many more fee schedules as we start automating allowed fees.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true,IsSecurityStamped=true)]
	public class FeeSched : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long FeeSchedNum;
		///<summary>The name of the fee schedule.</summary>
		public string Description;
		///<summary>Enum:FeeScheduleType </summary>
		public FeeScheduleType FeeSchedType;
		///<summary>Unlike with the old definition table, this ItemOrder is not as critical in the caching of data.  The item order is only for fee schedules of the same type.</summary>
		public int ItemOrder;
		///<summary>True if the fee schedule is hidden.  Can't delete fee schedules or change their type once created.</summary>
		public bool IsHidden;
		///<summary>True if the fee schedule is used globally and linked to the HQ. (Localization of the fees are not allowed)</summary>
		public bool IsGlobal;
		///<summary>FK to userod.UserNum.  Set to the user logged in when the row was inserted at SecDateEntry date and time.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.ExcludeFromUpdate)]
		public long SecUserNumEntry;
		///<summary>Timestamp automatically generated and user not allowed to change.  The actual date of entry.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateEntry)]
		public DateTime SecDateEntry;
		///<summary>Automatically updated by MySQL every time a row is added or changed. Could be changed due to user editing, custom queries or program
		///updates.  Not user editable with the UI.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime SecDateTEdit;

		public FeeSched Copy(){
			return (FeeSched)this.MemberwiseClone();
		}	
	}

	///<summary></summary>
	public enum FeeScheduleType{
		///<summary>0</summary>
		Normal,
		///<summary>1</summary>
		CoPay,
		///<summary>2, Formerly named "Allowed"</summary>
		OutNetwork,
		///<summary>3</summary>
		FixedBenefit,
		///<summary>4</summary>
		ManualBlueBook,
	}
}






