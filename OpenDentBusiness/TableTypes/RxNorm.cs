using System;

namespace OpenDentBusiness {
	///<summary>RxNorm created from a zip file.</summary>
	[Serializable]
	public class RxNorm:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RxNormNum;
		///<summary>RxNorm Concept universal ID.  Throughout the program, this is actually used as the Primary Key of this table rather than the RxNormNum.</summary>
		public string RxCui;
		///<summary>Multum code.  Only used for crosscoding during import/export with electronic Rx program.  User cannot see multum codes.  Most of the rows in this table do not have an MmslCode and user searches ignore rows with an MmslCode.</summary>
		public string MmslCode;
		///<summary>Only used for RxNorms, not Multums.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Description;

		///<summary></summary>
		public RxNorm Copy() {
			return (RxNorm)this.MemberwiseClone();
		}

	}

}













