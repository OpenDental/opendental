using System;

namespace OpenDentBusiness {
	///<summary></summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class AlertCategory:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AlertCategoryNum;
		///<summary>False by default, indicates that this is a row that can not be edited or deleted.</summary>
		public bool IsHQCategory;
		///<summary>Name used by HQ to identify the type of alert category this started as, allows us to associate new alerts.</summary>
		public string InternalName;
		///<summary>Name displayed to user when subscribing to alerts categories.</summary>
		public string Description;

		public AlertCategory() {
			
		}

		///<summary></summary>
		public AlertCategory Copy() {
			return (AlertCategory)this.MemberwiseClone();
		}
	}
}

