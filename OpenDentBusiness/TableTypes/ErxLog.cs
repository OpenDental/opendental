using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable]
	public class ErxLog:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ErxLogNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Holds up to 16MB.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string MsgText;
		///<summary>Automatically updated by MySQL every time a row is added or changed.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>FK to provider.ProvNum. The provider that the prescription request was sent by or on behalf of.</summary>
		public long ProvNum;
		///<summary>FK to Userod.UserNum. The user that created the erx.</summary>
		public long UserNum;

		///<summary></summary>
		public ErxLog Clone() {
			return (ErxLog)this.MemberwiseClone();
		}

	}
}
