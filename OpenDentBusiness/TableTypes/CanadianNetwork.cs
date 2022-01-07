using System;
using System.Collections;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness{

	///<summary>Not user-editable.</summary>
	[Serializable()]
	public class CanadianNetwork:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CanadianNetworkNum;
		///<summary>This will also be the folder name</summary>
		public string Abbrev;
		///<summary>.</summary>
		public string Descript;
		///<summary>A01.  Up to 12 char.</summary>
		public string CanadianTransactionPrefix;
		///<summary>Set to true if this network is in charge of handling all Request for Payment Reconciliation (RPR) transactions for all carriers within this network, as opposed to the individual carriers wihtin the network processing the RPR transactions themselves.</summary>
		public bool CanadianIsRprHandler;

		///<summary></summary>
		public CanadianNetwork Copy() {
			return (CanadianNetwork)this.MemberwiseClone();
		}
	}

	
}




