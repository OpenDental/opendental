using System;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable()]
	public class StmtLink:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long StmtLinkNum;
		///<summary>FK to statement.StatementNum.</summary>
		public long StatementNum;
		///<summary>Enum:StmtLinkTypes Represents what object FKey corresponds to.</summary>
		public StmtLinkTypes StmtLinkType;
		///<summary>FK to type of PK of another object depending on StmtLinkType value. E.g. procedurelog.ProcNum, paysplit.PaySplitNum, adjustment.AdjNum, etc.</summary>
		public long FKey;


		public StmtLink Copy() {
			return (StmtLink)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum StmtLinkTypes {
		///<summary>0 - Procedure</summary>
		Proc,
		///<summary>1 - Pay split</summary>
		PaySplit,
		///<summary>2 - Adjustment</summary>
		Adj,
		///<summary>3 - ClaimPay</summary>
		ClaimPay
	}
}
