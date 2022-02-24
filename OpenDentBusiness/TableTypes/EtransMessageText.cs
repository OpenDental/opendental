using System;
using System.Collections;

namespace OpenDentBusiness{

	/// <summary>Each row is big.  The entire X12 message text is stored here, since it can be the same for multiple etrans objects, and since the messages can be so big.</summary>
	[Serializable]
	[CrudTable(IsLargeTable=true)]
	public class EtransMessageText:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EtransMessageTextNum;
		///<summary>The entire message text, including carriage returns.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MessageText;

		///<summary></summary>
		public EtransMessageText Copy() {
			return (EtransMessageText)this.MemberwiseClone();
		}

	}

	




}

















