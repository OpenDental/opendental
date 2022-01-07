using System;

namespace OpenDentBusiness {
	///<summary>Used in the Central Enterprise Management Tool to link CentralConnections and ConnectionGroups.  Each connection can be in multiple groups.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class ConnGroupAttach:TableBase {
		///<summary>Primary Key</summary>
		[CrudColumn(IsPriKey=true)]
		public long ConnGroupAttachNum;
		///<summary>FK to connectiongroup.ConnectionGroupNum</summary>
		public long ConnectionGroupNum;
		///<summary>FK to centralconnection.CentralConnectionNum</summary>
		public long CentralConnectionNum;

		///<summary></summary>
		public ConnGroupAttach Clone() {
			return (ConnGroupAttach)this.MemberwiseClone();
		}

	}

}
