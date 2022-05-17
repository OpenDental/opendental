using System;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class AlertRead:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AlertReadNum;
		///<summary>FK to alertitem.AlertItemNum.</summary>
		public long AlertItemNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;

		public AlertRead() {
			
		}

		public AlertRead(long alertItemNum,long userNum) {
			this.AlertItemNum=alertItemNum;
			this.UserNum=userNum;
		}

		///<summary></summary>
		public AlertRead Copy() {
			return (AlertRead)this.MemberwiseClone();
		}
	}
}
