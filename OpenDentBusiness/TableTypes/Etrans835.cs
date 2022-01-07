using System;

namespace OpenDentBusiness {
	///<summary>Corresponds to an etrans record containing a raw 835 X12 message attached in etransmessagetext table.  This is denoted by etrans.Etype=ERA_835</summary>
	[Serializable]
	public class Etrans835:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long Etrans835Num;
		///<summary>FK to etrans.EtransNum .</summary>
		public long EtransNum;
		///<summary>Up to 60 characters.  Corresponds to X835.PayerName, a read-only field.</summary>
		public string PayerName;
		///<summary>Up to 50 characters.  Corresponds to X835.TransRefNum, a read-only field.</summary>
		public string TransRefNum;
		///<summary>Corresponds to X835.InsPaid, a read-only field.</summary>
		public double InsPaid;
		///<summary>Up to 9 characters.  Corresponds to X835.ControlId, a read-only field.</summary>
		public string ControlId;
		///<summary>Up to 3 characters.  Corresponds to X835._paymentMethodCode, a read-only field.</summary>
		public string PaymentMethodCode;
		///<summary>Up to 100 characters (not based on actual patient name field sizes).
		///Corresponds to Hx835_Claim.PatientName.ToString() if one patient, or says "(#)" if multiple patients to show count.</summary>
		public string PatientName;
		///<summary>Enum:X835Status .  Calculated status.  Only changes when ERA changes.</summary>
		public X835Status Status;
		///<summary>Enum:X835AutoProcessed .  The initial disposition of ERA's that have passed through our auto/semi-auto processing system.</summary>
		public X835AutoProcessed AutoProcessed;
		///<summary>True if a user has acknowledged the auto processed ERA.</summary>
		public bool IsApproved;

		///<summary></summary>
		public Etrans835 Copy(){
			return (Etrans835)this.MemberwiseClone();
		}

	}
	
	///<summary>The initial disposition of ERA's that have passed through our auto/semi-auto processing system.</summary>
	public enum X835AutoProcessed {
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		SemiAutoIncomplete,
		///<summary>2</summary>
		SemiAutoComplete,
		///<summary>3</summary>
		FullAutoIncomplete,
		///<summary>4</summary>
		FullAutoComplete,
	}
}