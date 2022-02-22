using System;

namespace OpenDentBusiness {
	///<summary>Subscribes a user and optional clinic to specifc alert types.  Users will not get alerts unless they have an entry in this table.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class AlertSub:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AlertSubNum;
		///<summary>FK to userod.UserNum.</summary>
		public long UserNum;
		///<summary>FK to clinic.ClinicNum. Can be 0.</summary>
		public long ClinicNum;
		///<summary>Deprecated.</summary>
		public AlertType Type;
		///<summary>FK to alertcategory.AlertCategoryNum.</summary>
		public long AlertCategoryNum;

		public AlertSub() {
		}

		public AlertSub(long userNum,long clinicNum,long alertCatNum) {
			this.UserNum=userNum;
			this.ClinicNum=clinicNum;
			this.AlertCategoryNum=alertCatNum;
		}

		///<summary></summary>
		public AlertSub Copy() {
			return (AlertSub)this.MemberwiseClone();
		}
	}
}
