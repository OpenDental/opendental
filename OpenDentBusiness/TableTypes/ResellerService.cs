using System;

namespace OpenDentBusiness {

	///<summary>An entry in a list of services for a specific reseller to pick from.  To determine which services a certain customer has access to, check the repeating charges table.</summary>
	[Serializable]
	public class ResellerService:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ResellerServiceNum;
		///<summary>FK to reseller.ResellerNum.</summary>
		public long ResellerNum;
		///<summary>FK to procedurecode.CodeNum.</summary>
		public long CodeNum;
		///<summary>Amount of the service.  Might be a default, or might be the same for all customers of the reseller.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.Double, DecimalPlaces=4)]
		public double Fee;
		///<summary>URL for mass email host</summary>
		public string HostedUrl;
		///<summary>Returns a copy of this ResellerService.</summary>
		public ResellerService Copy() {
			return (ResellerService)this.MemberwiseClone();
		}


	}





}













