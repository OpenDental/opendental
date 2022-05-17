using System;

namespace OpenDentBusiness.Mobile {

	///<summary>One Rx for one patient.</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class RxPatm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long RxNum;
		///<summary>FK to patientm.PatNum.</summary>
		public long PatNum;
		///<summary>Date of Rx.</summary>
		public DateTime RxDate;
		///<summary>Drug name.</summary>
		public string Drug;
		///<summary>Directions.</summary>
		public string Sig;
		///<summary>Amount to dispense.</summary>
		public string Disp;
		///<summary>Number of refills.</summary>
		public string Refills;
		///<summary>FK to providerm.ProvNum.</summary>
		public long ProvNum;

		///<summary></summary>
		public RxPatm Copy() {
			return (RxPatm)this.MemberwiseClone();
		}



	}




}