using System;

namespace OpenDentBusiness {
	///<summary>Logs all changes made to claimproc estimates that are made by the Blue Book feature.</summary>
	[Serializable]
	public class InsBlueBookLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsBlueBookLogNum;
		///<summary>FK to claimproc.ClaimProcNum. The claimproc for which the estimate was changed.</summary>
		public long ClaimProcNum;
		///<summary>The new claimproc.InsEstTotal that was calculated by the Blue Book feature.</summary>
		public double AllowedFee;
		///<summary>The date and time of entry. Not editable by user.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
		public DateTime DateTEntry;
		///<summary>Explanation of how the Blue Book feature obtained the new insurance estimate.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Description;

		///<summary>Returns a copy of this InsBlueBookLog.</summary>
		public InsBlueBookLog Copy() {
			return (InsBlueBookLog)this.MemberwiseClone();
		}
	}
}