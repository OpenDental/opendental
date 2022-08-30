using System;

namespace OpenDentBusiness {
	///<summary>Keeps track of account details of e-statements per clinic.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class Ebill:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EbillNum;
		///<summary>FK to clinic.ClinicNum</summary>
		public long ClinicNum;
		///<summary>The account number for the e-statement client.</summary>
		public string ClientAcctNumber;
		///<summary>The user name for this particular account.</summary>
		public string ElectUserName;
		///<summary>The password for this particular account.</summary>
		public string ElectPassword;
		///<summary>Enum:EbillAddress </summary>
		public EbillAddress PracticeAddress;
		///<summary>Enum:EbillAddress </summary>
		public EbillAddress RemitAddress;

		public Ebill Copy(){
			return (Ebill)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum EbillAddress {
		///<summary>0</summary>
		PracticePhysical,
		///<summary>1</summary>
		PracticeBilling,
		///<summary>2</summary>
		PracticePayTo,
		///<summary>3</summary>
		ClinicPhysical,
		///<summary>4</summary>
		ClinicBilling,
		///<summary>5</summary>
		ClinicPayTo
	}	
}
